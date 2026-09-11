using Doroti.Ui;

namespace Doroti.Hosting;

public abstract record PlatformCompositionPart(int PaintOrder);
public sealed record PlatformRasterSegment(int PaintOrder, IReadOnlyList<SceneCommand> Commands) : PlatformCompositionPart(PaintOrder);
public sealed record PlatformNativeSegment(PlatformViewPlacement Placement) : PlatformCompositionPart(Placement.PaintOrder);
public sealed record PlatformShieldSegment(PlatformInputShield Shield) : PlatformCompositionPart(Shield.PaintOrder);

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

    public static PlatformCompositionPlan Build(Scene scene, PlatformCompositionToken token, PlatformViewCoordinator coordinator)
    {
        ArgumentNullException.ThrowIfNull(scene);
        ObjectDisposedException.ThrowIf(scene.debugDisposed, scene);
        if (scene.viewId != token.OwnerViewId || coordinator.OwnerViewId != token.OwnerViewId || token.ViewEpoch < 0 || token.FrameNumber < 0 || token.SurfaceGeneration < 0)
            throw new InvalidOperationException("Platform composition token/scene owner is invalid.");
        if (!double.IsFinite(token.DeviceScaleX) || !double.IsFinite(token.DeviceScaleY) || token.DeviceScaleX <= 0 || token.DeviceScaleY <= 0)
            throw new InvalidOperationException("Platform composition device scale is invalid.");
        var flattened = new List<SceneCommand>();
        Flatten(scene.Commands, flattened, scene.viewId, 0);
        var nativeCount = flattened.Count(command => command.Operation == "platformView");
        if (nativeCount == 0 && !flattened.Any(command => command.Operation == "inputShield"))
            return new(token, [new PlatformRasterSegment(0, Array.AsReadOnly(flattened.ToArray()))], []);
        if (nativeCount > MaximumNativeViews) throw Failure("native/overlay limit exceeded");
        // Backdrop sampling may reach earlier native siblings even outside their scope.
        if (nativeCount != 0 && flattened.Any(command => command.Operation == "backdropFilter"))
            throw Failure("backdrop sampling across native content is unsupported");

        var parts = new List<PlatformCompositionPart>();
        var raster = new List<SceneCommand>();
        var scopes = new List<SceneCommand>();
        var states = new Stack<State>();
        // RenderView's retained root already scales into physical pixels. Native placements
        // are logical, while raster segments retain the original physical scene transforms.
        var state = new State(new(1 / token.DeviceScaleX, 0, 0, 1 / token.DeviceScaleY, 0, 0), null, null);
        var handles = new HashSet<PlatformViewHandle>();
        foreach (var command in flattened)
        {
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
                coordinator.ValidatePlacementSupport(placement, true);
                parts.Add(new PlatformNativeSegment(placement));
                raster.AddRange(scopes);
                continue;
            }
            if (command.Operation == "inputShield")
            {
                if (command.HostPayload is not SceneInputShieldPayload shield) throw Failure("untyped input shield");
                if (state.Unsupported is not null) throw Failure(state.Unsupported);
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
            raster.Add(command);
        }
        if (states.Count != 0) throw Failure("unclosed scene scopes");
        FlushRaster();
        if (parts.Count(part => part is PlatformRasterSegment) > MaximumRasterSegments)
            throw Failure("raster segment limit exceeded");
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
