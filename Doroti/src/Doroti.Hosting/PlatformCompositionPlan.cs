using Doroti.Ui;

namespace Doroti.Hosting;

public abstract record PlatformCompositionPart(int PaintOrder);
public sealed record PlatformRasterSegment(int PaintOrder, IReadOnlyList<SceneCommand> Commands) : PlatformCompositionPart(PaintOrder);
public sealed record PlatformNativeSegment(PlatformViewPlacement Placement) : PlatformCompositionPart(Placement.PaintOrder);
public sealed record PlatformShieldSegment(PlatformInputShield Shield) : PlatformCompositionPart(Shield.PaintOrder);
public sealed record PlatformBackdropSegment(int PaintOrder, Rect Bounds, double SigmaX, double SigmaY) : PlatformCompositionPart(PaintOrder);

/// <summary>Immutable plan plus native lifetime leases. Dispose only after presentation resources retire.</summary>
public sealed class PlatformCompositionPlan : IDisposable
{
    private IDisposable[]? _leases;
    internal PlatformCompositionPlan(PlatformCompositionToken token, IEnumerable<PlatformCompositionPart> parts, IDisposable[] leases)
    {
        Token = token;
        Parts = Array.AsReadOnly(parts.ToArray());
        _leases = leases;
    }
    public PlatformCompositionToken Token { get; }
    public IReadOnlyList<PlatformCompositionPart> Parts { get; }
    public bool HasNativeContent => Parts.Any(part => part is PlatformNativeSegment);
    public bool RasterCaptureIncludesNative => !HasNativeContent;
    public bool IsDisposed => Volatile.Read(ref _leases) is null;
    public void Dispose()
    {
        foreach (var lease in Interlocked.Exchange(ref _leases, null) ?? []) lease.Dispose();
    }
}

/// <summary>Scene ordering and effect validation. Does not claim a host has displayed the result.</summary>
public static class PlatformCompositionPlanner
{
    public const int MaximumNativeViews = 16;
    public const int MaximumRasterSegments = MaximumNativeViews + 1;
    private sealed record State(PlatformViewTransform Transform, Rect? Clip, string? Unsupported);

    public static PlatformCompositionPlan Build(Scene scene, PlatformCompositionToken token, PlatformViewCoordinator coordinator,
        PlatformViewComposition composition = PlatformViewComposition.InterleavedComposition, bool allowNativeBackdrop = false)
    {
        ArgumentNullException.ThrowIfNull(scene);
        ObjectDisposedException.ThrowIf(scene.debugDisposed, scene);
        if (scene.viewId != token.OwnerViewId) throw new InvalidOperationException("Platform composition scene owner is invalid.");
        return Build(scene.Commands, token, coordinator, composition, allowNativeBackdrop);
    }

    /// <summary>Plans the immutable commands retained by a product renderer.</summary>
    public static PlatformCompositionPlan Build(IReadOnlyList<SceneCommand> commands, PlatformCompositionToken token,
        PlatformViewCoordinator coordinator, PlatformViewComposition composition = PlatformViewComposition.InterleavedComposition,
        bool allowNativeBackdrop = false)
    {
        ArgumentNullException.ThrowIfNull(commands);
        if (composition is not (PlatformViewComposition.NativeOverlay or PlatformViewComposition.InterleavedComposition))
            throw new ArgumentOutOfRangeException(nameof(composition));
        if (coordinator.OwnerViewId != token.OwnerViewId || token.ViewEpoch < 0 || token.FrameNumber < 0 || token.SurfaceGeneration < 0)
            throw new InvalidOperationException("Platform composition token/scene owner is invalid.");
        if (!double.IsFinite(token.DeviceScaleX) || !double.IsFinite(token.DeviceScaleY) || token.DeviceScaleX <= 0 || token.DeviceScaleY <= 0)
            throw new InvalidOperationException("Platform composition device scale is invalid.");
        var flattened = new List<SceneCommand>();
        Flatten(commands, flattened, token.OwnerViewId, 0);
        var nativeCount = flattened.Count(command => command.Operation == "platformView");
        if (nativeCount == 0 && !flattened.Any(command => command.Operation == "inputShield"))
            return new(token, [new PlatformRasterSegment(0, Array.AsReadOnly(flattened.ToArray()))], []);
        if (nativeCount > MaximumNativeViews) throw Failure("native/overlay limit exceeded");
        // Backdrop sampling may reach earlier native siblings even outside their scope.
        if (nativeCount != 0 && flattened.Any(command => command.Operation == "backdropFilter") &&
            (!allowNativeBackdrop || composition != PlatformViewComposition.InterleavedComposition))
            throw Failure("backdrop sampling across native content is unsupported");

        var parts = new List<PlatformCompositionPart>();
        var raster = new List<SceneCommand>();
        var scopes = new List<SceneCommand>();
        var states = new Stack<State>();
        // RenderView's retained root already scales into physical pixels. Native placements
        // are logical, while raster segments retain the original physical scene transforms.
        var state = new State(new(1 / token.DeviceScaleX, 0, 0, 1 / token.DeviceScaleY, 0, 0), null, null);
        var handles = new HashSet<PlatformViewHandle>();
        var precedingNativeBounds = new List<Rect>();
        foreach (var command in flattened)
        {
            if (allowNativeBackdrop && nativeCount != 0 && command.Operation == "backdropFilter")
            {
                if (command.HostPayload is not SceneBackdropFilterPayload backdrop || state.Clip is not { } clip ||
                    state.Unsupported is not null || !state.Transform.IsAxisAligned ||
                    backdrop.BlendMode != BlendMode.srcOver || backdrop.BackdropId is not null ||
                    backdrop.Filter is not { Outer: null, Inner: null, ColorFilter: null, Matrix4: null, Shader: null, TileMode: TileMode.clamp } filter ||
                    !double.IsFinite(filter.SigmaX) || !double.IsFinite(filter.SigmaY) ||
                    filter.SigmaX <= 0 || filter.SigmaY <= 0 || filter.SigmaX > 32 || filter.SigmaY > 32)
                    throw Failure("native backdrop requires a clipped, ungrouped srcOver Gaussian blur with sigma in (0,32]");
                if (parts.Any(p => p is PlatformBackdropSegment)) throw Failure("one native backdrop region per frame is supported");
                FlushRaster();
                parts.Add(new PlatformBackdropSegment(parts.Count, clip,
                    filter.SigmaX * state.Transform.M11, filter.SigmaY * state.Transform.M22));
                // The host draws the filtered backdrop. Keep a balanced no-op scope
                // for its sharp foreground child, and reject native children inside it.
                var scope = new SceneCommand("offset", null) { HostPayload = new SceneOffsetPayload(0, 0) };
                states.Push(state); scopes.Add(scope); raster.AddRange(scopes);
                state = state with { Unsupported = "native views inside a backdrop scope are unsupported" };
                continue;
            }
            if (command.Operation == "platformView")
            {
                if (command.HostPayload is not ScenePlatformViewPayload native) throw Failure("untyped native payload");
                if (native.Handle.OwnerViewId != token.OwnerViewId || coordinator.Resolve(native.Handle.InstanceId) != native.Handle)
                    throw Failure($"stale native identity {native.Handle}");
                if (!handles.Add(native.Handle)) throw Failure($"simultaneous multiple attachment of {native.Handle}");
                if (state.Unsupported is not null) throw Failure(state.Unsupported);
                FlushRaster();
                var placement = new PlatformViewPlacement(native.Handle, native.Bounds, state.Transform, state.Clip, parts.Count,
                    !native.Bounds.isEmpty && !(state.Clip?.isEmpty ?? false));
                placement.Validate();
                coordinator.ValidatePlacementSupport(placement, composition == PlatformViewComposition.InterleavedComposition);
                parts.Add(new PlatformNativeSegment(placement));
                if (composition == PlatformViewComposition.NativeOverlay && placement.Visible)
                {
                    if (!placement.Transform.IsAxisAligned) throw Failure("NativeOverlay requires axis-aligned native bounds");
                    var a = placement.Transform.Map(placement.Bounds.topLeft);
                    var b = placement.Transform.Map(placement.Bounds.bottomRight);
                    var bounds = new Rect(a.dx, a.dy, b.dx, b.dy);
                    if (placement.Clip is { } clip) bounds = bounds.intersect(clip);
                    if (!bounds.isEmpty) precedingNativeBounds.Add(bounds);
                }
                raster.AddRange(scopes);
                continue;
            }
            if (command.Operation == "inputShield")
            {
                if (command.HostPayload is not SceneInputShieldPayload shield) throw Failure("untyped input shield");
                // Opacity affects the raster but not framework hit testing. Modal
                // fade transitions may contain shields, while native views inside
                // such a saveLayer remain explicitly unsupported.
                if (state.Unsupported is not null && state.Unsupported != "opacity across native content is unsupported")
                    throw Failure(state.Unsupported);
                FlushRaster();
                parts.Add(new PlatformShieldSegment(new(shield.Bounds, state.Transform, state.Clip, parts.Count, shield.Debug)));
                raster.AddRange(scopes);
                continue;
            }
            if (command.Operation == "pop")
            {
                if (states.Count == 0) throw Failure("unbalanced scene pop");
                state = states.Pop();
                scopes.RemoveAt(scopes.Count - 1);
            }
            else if (IsScope(command.Operation))
            {
                states.Push(state);
                scopes.Add(command);
                state = Apply(state, command);
            }
            else if (precedingNativeBounds.Count != 0 &&
                !(command.HostPayload is ScenePicturePayload { Commands.Count: 0 }))
            {
                // A picture's cull hint is not a clip (Canvas currently ignores cullRect).
                // Unknown draw/effect bounds must not silently move foreground below HWNDs.
                // Only an enclosing, supported rectangular clip proves non-overlap.
                if (state.Unsupported is not null || state.Clip is not { } clip ||
                    precedingNativeBounds.Any(bounds => !bounds.intersect(clip).isEmpty))
                    throw Failure("NativeOverlay cannot prove that foreground raster avoids earlier native content; interleaved composition or an explicit disjoint rectangular clip is required");
            }
            raster.Add(command);
        }
        if (states.Count != 0) throw Failure("unclosed scene scopes");
        FlushRaster();
        if (parts.Count(part => part is PlatformRasterSegment) > MaximumRasterSegments)
            throw Failure("raster segment limit exceeded");
        if (composition == PlatformViewComposition.NativeOverlay)
        {
            if (nativeCount != 0 && parts.Any(part => part is PlatformShieldSegment))
                throw Failure("NativeOverlay cannot display foreground UI protected by an input shield; interleaved composition is required");
            var occupied = new List<Rect>();
            foreach (var native in parts.OfType<PlatformNativeSegment>())
            {
                var placement = native.Placement;
                if (!placement.Visible) continue;
                if (!placement.Transform.IsAxisAligned) throw Failure("NativeOverlay requires axis-aligned native bounds");
                var a = placement.Transform.Map(placement.Bounds.topLeft);
                var b = placement.Transform.Map(placement.Bounds.bottomRight);
                var bounds = new Rect(a.dx, a.dy, b.dx, b.dy);
                if (placement.Clip is { } clip) bounds = bounds.intersect(clip);
                if (bounds.isEmpty) continue;
                if (occupied.Any(previous => !previous.intersect(bounds).isEmpty))
                    throw Failure("NativeOverlay requires non-overlapping native regions");
                occupied.Add(bounds);
            }
        }
        var leases = new List<IDisposable>();
        try
        {
            foreach (var handle in handles) leases.Add(coordinator.Retain(handle));
            return new(token, parts, leases.ToArray());
        }
        catch { foreach (var lease in leases) lease.Dispose(); throw; }

        void FlushRaster()
        {
            // Reopen and close transform/clip scopes for every independent raster surface.
            // Group effects crossing native boundaries have already been rejected.
            var commands = raster.Concat(scopes.Select(_ => new SceneCommand("pop", null))).ToArray();
            parts.Add(new PlatformRasterSegment(parts.Count, Array.AsReadOnly(commands)));
            raster.Clear();
        }
        DorotiCapabilityException Failure(string reason) => new(DorotiCapabilityIds.PlatformViews, token.OwnerViewId,
            DartUiInvocation.Managed("PlatformCompositionPlanner"), $"frame={token.FrameNumber}; epoch={token.ViewEpoch}; {reason}");
    }

    private static void Flatten(IReadOnlyList<SceneCommand> commands, List<SceneCommand> output, ulong owner, int depth)
    {
        if (depth > 256) throw new InvalidDataException("Retained scene nesting exceeds the platform-view limit.");
        foreach (var command in commands)
        {
            if (command.Operation == "retained")
            {
                if (command.HostPayload is not SceneRetainedPayload retained || retained.ViewId != owner || retained.Generation <= 0)
                    throw new InvalidDataException("Retained scene owner/generation is invalid.");
                Flatten(retained.Commands, output, owner, depth + 1);
            }
            else output.Add(command);
        }
    }
    private static bool IsScope(string operation) => operation is "offset" or "transform" or "clipRect" or "clipRRect" or
        "clipRSuperellipse" or "clipPath" or "opacity" or "colorFilter" or "imageFilter" or "shaderMask" or "backdropFilter";

    private static State Apply(State state, SceneCommand command)
    {
        switch (command.HostPayload)
        {
            case SceneOpacityPayload opacity:
                return state with {
                    Transform = state.Transform.ThenLocal(new(1, 0, 0, 1, opacity.Offset.dx, opacity.Offset.dy)),
                    Unsupported = state.Unsupported ?? "opacity across native content is unsupported"
                };
            case SceneOffsetPayload offset:
                return state with { Transform = state.Transform.ThenLocal(new(1, 0, 0, 1, offset.Dx, offset.Dy)) };
            case SceneTransformPayload transform:
                var m = transform.Matrix4;
                if (m.Count != 16 || m.Any(value => !double.IsFinite(value)) || m[2] != 0 || m[3] != 0 || m[6] != 0 || m[7] != 0 ||
                    m[8] != 0 || m[9] != 0 || m[10] != 1 || m[11] != 0 || m[14] != 0 || m[15] != 1)
                    return state with { Unsupported = "perspective/3D transform is unsupported" };
                return state with { Transform = state.Transform.ThenLocal(new(m[0], m[1], m[4], m[5], m[12], m[13])) };
            case SceneClipRectPayload clip:
                if (clip.Behavior == Clip.none) return state;
                if (clip.Behavior == Clip.antiAliasWithSaveLayer) return state with { Unsupported = "saveLayer clip across native content is unsupported" };
                if (!state.Transform.IsAxisAligned) return state with { Unsupported = "transformed non-rectangular clip is unsupported" };
                var a = state.Transform.Map(clip.Rect.topLeft);
                var b = state.Transform.Map(clip.Rect.bottomRight);
                var bounds = new Rect(a.dx, a.dy, b.dx, b.dy);
                return state with { Clip = state.Clip is { } parent ? parent.intersect(bounds) : bounds };
            default: return state with { Unsupported = $"{command.Operation} across native content is unsupported" };
        }
    }
}

/// <summary>Prepared host transaction. Commit must preserve the old frame on failure.</summary>
public interface IPreparedPlatformComposition : IAsyncDisposable
{
    ValueTask CommitAsync(CancellationToken cancellationToken);
    Task Retirement { get; }
}
public interface IPlatformCompositionPresenter
{
    ValueTask<IPreparedPlatformComposition> PrepareAsync(PlatformCompositionPlan plan, CancellationToken cancellationToken);
}

/// <summary>Serializes atomic commits and rejects stale frame/epoch/surface batches.</summary>
public sealed class PlatformCompositionSession(ulong ownerViewId, IPlatformCompositionPresenter presenter) : IAsyncDisposable
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly List<Task> _retirements = [];
    private long _epoch;
    private long _surfaceGeneration;
    private long _frame = -1;
    private bool _closed;

    public async ValueTask SetEpochAsync(long epoch, long surfaceGeneration)
    {
        await _gate.WaitAsync().ConfigureAwait(false);
        try
        {
            ObjectDisposedException.ThrowIf(_closed, this);
            if (epoch < _epoch || surfaceGeneration < _surfaceGeneration) throw new InvalidOperationException("Composition epoch cannot regress.");
            if (epoch != _epoch || surfaceGeneration != _surfaceGeneration) _frame = -1;
            _epoch = epoch;
            _surfaceGeneration = surfaceGeneration;
        }
        finally { _gate.Release(); }
    }

    /// <summary>Takes ownership of the plan, including on stale/failed submission.</summary>
    public async ValueTask SubmitAsync(PlatformCompositionPlan plan, CancellationToken cancellationToken = default)
    {
        IPreparedPlatformComposition? prepared = null;
        bool entered = false;
        try
        {
            await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
            entered = true;
            ObjectDisposedException.ThrowIf(_closed || plan.IsDisposed, this);
            var token = plan.Token;
            if (token.OwnerViewId != ownerViewId || token.ViewEpoch != _epoch || token.SurfaceGeneration != _surfaceGeneration || token.FrameNumber <= _frame)
                throw new InvalidOperationException("Stale platform composition token.");
            prepared = await presenter.PrepareAsync(plan, cancellationToken).ConfigureAwait(false);
            await prepared.CommitAsync(cancellationToken).ConfigureAwait(false);
            _frame = token.FrameNumber;
            _retirements.RemoveAll(task => task.IsCompletedSuccessfully);
            _retirements.Add(RetireAsync(prepared, plan));
            prepared = null;
            plan = null!;
        }
        finally
        {
            try
            {
                if (prepared is not null) await prepared.DisposeAsync().ConfigureAwait(false);
            }
            finally
            {
                plan?.Dispose();
                if (entered) _gate.Release();
            }
        }
    }
    private static async Task RetireAsync(IPreparedPlatformComposition prepared, PlatformCompositionPlan plan)
    {
        try { await prepared.Retirement.ConfigureAwait(false); }
        finally
        {
            try { await prepared.DisposeAsync().ConfigureAwait(false); }
            finally { plan.Dispose(); }
        }
    }
    public async ValueTask DisposeAsync()
    {
        Task[] pending;
        await _gate.WaitAsync().ConfigureAwait(false);
        try { _closed = true; pending = _retirements.ToArray(); }
        finally { _gate.Release(); }
        await Task.WhenAll(pending).ConfigureAwait(false);
    }
}
