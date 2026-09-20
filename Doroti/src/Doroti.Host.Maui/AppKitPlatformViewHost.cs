#if MACOS
using AppKit;
using CoreGraphics;
using Doroti.Hosting;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;
using Rect = Doroti.Ui.Rect;

namespace Doroti.Host.Maui;

/// <summary>Owner-local Metal/native interleaving in a layer-backed sibling of MTKView.</summary>
internal sealed class AppKitPlatformViewHost : IDisposable
{
    private sealed class OverlayView : NSView
    {
        public OverlayView() { WantsLayer = true; Layer!.MasksToBounds = true; }
        public override bool IsFlipped => true;
        public override NSView? HitTest(CGPoint point)
        {
            if (Hidden) return null;
            var local = ConvertPointFromView(point, Superview);
            if (!Bounds.Contains(local)) return null;
            // CALayer z ordering and AppKit's default subview hit ordering are independent.
            foreach (var child in Subviews.OrderByDescending(child => child.Layer?.ZPosition ?? 0))
                if (!child.Hidden && child.HitTest(local) is { } hit) return hit;
            return null;
        }
    }

    private sealed class ShieldView(DorotiMacOSMetalSurface surface) : NSView
    {
        public override NSView? HitTest(CGPoint point) => !Hidden && Frame.Contains(point) ? surface.NativeView : null;
    }

    private readonly DorotiMacOSMetalSurface _surface;
    private readonly MauiTextInputBridge _textInput;
    private OverlayView? _overlay;
    private NSWindow? _window;
    private Brightness _brightness = Brightness.light;
    private Brightness? _appliedBrightness;
    private PlatformViewCoordinator? _coordinator;
    private PlatformCompositionSession? _session;
    private readonly CommitPresenter _presenter = new();
    private long _compositionFrame;
    private PlatformViewPlacement[] _placements = [];
    private readonly List<AppKitPlatformRasterSurface> _rasters = [];
    private readonly List<ShieldView> _shields = [];
    private readonly List<AppKitPlatformBlurView> _effects = [];
    private PlatformBackdropSegment[] _visibleEffects = [];
    private PlatformInputShield[] _visibleShields = [];
    private (AppKitPlatformRasterSurface Slot, int Order, CGRect Bounds)[] _visibleRasters = [];
    private PreparedFrame? _pending;
    private bool _disposed;

    internal AppKitPlatformViewHost(DorotiMacOSMetalSurface surface, MauiTextInputBridge textInput)
    { _surface = surface; _textInput = textInput; }

    internal IEnumerable<IPlatformViewFactory> CreateFactories(Func<IApplicationResourceHostCapability> resources) =>
    [new AppKitPlatformViewFactory(GetContainer, false, _textInput.YieldMacOSNativeFocus, RestoreFocus, interleaved: true),
     new AppKitPlatformViewFactory(GetContainer, true, _textInput.YieldMacOSNativeFocus, RestoreFocus, interleaved: true),
     new AppKitPlatformViewFactory(GetContainer, "doroti/webview", _textInput.YieldMacOSNativeFocus, RestoreFocus, interleaved: true, resources: resources)];

    internal bool HasComposition => _placements.Length != 0 || _visibleRasters.Length != 0 ||
        _visibleShields.Length != 0 || _visibleEffects.Length != 0 || _pending is not null;

    internal void Configure(PlatformViewCoordinator coordinator) => _coordinator = coordinator;

    private NSView GetContainer()
    {
        AppKitPlatformViewDispatcher.VerifyThread();
        ObjectDisposedException.ThrowIf(_disposed, this);
        var native = _surface.NativeView ?? throw new InvalidOperationException("AppKit render surface is not attached.");
        var parent = native.Superview ?? throw new InvalidOperationException("AppKit render surface has no container.");
        if (_window is not null && native.Window is not null && native.Window != _window)
            throw new InvalidOperationException("An AppKit PlatformView owner cannot move between windows.");
        _window ??= native.Window;
        _overlay ??= new OverlayView();
        if (_appliedBrightness != _brightness)
        {
            _overlay.Appearance = NSAppearance.GetAppearance(_brightness == Brightness.dark ? NSAppearance.NameDarkAqua : NSAppearance.NameAqua);
            _appliedBrightness = _brightness;
        }
        if (_overlay.Superview != parent)
        {
            _overlay.RemoveFromSuperview();
            parent.AddSubview(_overlay, NSWindowOrderingMode.Above, native);
        }
        _overlay.Frame = native.Frame;
        return _overlay;
    }

    private void RestoreFocus()
    {
        // Attachment operations already run on the main thread. A queued focus restoration
        // could steal focus from another native control selected later in the same batch.
        if (!_disposed && _surface.NativeView is { Window: { } window } native)
            window.MakeFirstResponder(native);
    }

    internal void Draw(SkiaSceneRenderer renderer, SKCanvas canvas, IReadOnlyList<SceneCommand> commands,
        DorotiFrameDescriptor descriptor, int width, int height, Brightness brightness)
    {
        AppKitPlatformViewDispatcher.VerifyThread();
        ObjectDisposedException.ThrowIf(_disposed, this);
        CancelPending();
        _brightness = brightness;
        var coordinator = _coordinator ?? throw new InvalidOperationException("AppKit PlatformView coordinator is missing.");
        if (_placements.Length == 0 && _visibleRasters.Length == 0 && _visibleShields.Length == 0 && _visibleEffects.Length == 0 && !HasPlatformCommands(commands))
        {
            renderer.DrawPlatformRasterSegment(canvas, commands, width, height);
            return;
        }
        var token = new PlatformCompositionToken(descriptor.ViewId, descriptor.MetricsGeneration,
            ++_compositionFrame, descriptor.ResizeTargetGeneration, descriptor.DeviceScaleX, descriptor.DeviceScaleY);
        var plan = PlatformCompositionPlanner.Build(commands, token, coordinator, PlatformViewComposition.InterleavedComposition, AppKitPlatformBlurView.Support);
        var frames = new List<AppKitPlatformRasterSurface.RasterFrame>();
        var gpuLeases = new List<IDisposable>();
        try
        {
            if (plan.Parts.OfType<PlatformShieldSegment>().Any(part => !part.Shield.Transform.IsAxisAligned))
                throw new InvalidOperationException("AppKit input shields require axis-aligned rectangular bounds.");
            foreach (var part in plan.Parts.OfType<PlatformNativeSegment>()) gpuLeases.Add(coordinator.Retain(part.Placement.Handle));
            foreach (var effect in plan.Parts.OfType<PlatformBackdropSegment>()) AppKitPlatformBlurView.Validate(effect);
            var segments = plan.Parts.OfType<PlatformRasterSegment>().ToArray();
            if (checked((long)width * height * 4 * 3 * Math.Max(_rasters.Count, segments.Length - 1)) > 256L * 1024 * 1024)
                throw new NotSupportedException("AppKit platform raster storage exceeds 256 MiB.");
            renderer.DrawPlatformRasterSegment(canvas, segments[0].Commands, width, height);
            var native = _surface.NativeView!;
            for (var index = 1; index < segments.Length; index++)
            {
                if (_rasters.Count < index) _rasters.Add(native.CreatePlatformRasterSurface());
                frames.Add(_rasters[index - 1].Prepare(renderer, segments[index], width, height, plan.Token));
            }
            _pending = new PreparedFrame(this, plan, frames.ToArray(), gpuLeases.ToArray());
        }
        catch { foreach (var frame in frames) frame.Dispose(); foreach (var lease in gpuLeases) lease.Dispose(); plan.Dispose(); throw; }
    }

    private static bool HasPlatformCommands(IReadOnlyList<SceneCommand> commands, int depth = 0)
    {
        if (depth > 256) throw new InvalidDataException("Retained scene nesting exceeds the platform-view limit.");
        return commands.Any(command => command.Operation is "platformView" or "inputShield" ||
            command.HostPayload is SceneRetainedPayload retained && HasPlatformCommands(retained.Commands, depth + 1));
    }

    internal PreparedFrame? TakePending() { var frame = _pending; _pending = null; return frame; }
    internal void CancelPending() { _pending?.Dispose(); _pending = null; }

    // A render callback cannot suspend inside AppKit's draw transaction. Cancel queued operations
    // on contention instead of blocking the main thread behind a retiring instance.
    private static void RunNow(Func<CancellationToken, ValueTask> action)
    {
        using var cancellation = new CancellationTokenSource();
        var task = action(cancellation.Token);
        if (!task.IsCompleted)
        {
            cancellation.Cancel();
            _ = ObserveCancellation(task);
            throw new InvalidOperationException("AppKit native placement is busy; the frame was not committed.");
        }
        task.GetAwaiter().GetResult();
    }
    private static async Task ObserveCancellation(ValueTask task)
    {
        try { await task; }
        catch (OperationCanceledException) { }
        catch (Exception error) { System.Diagnostics.Trace.TraceError(error.ToString()); }
    }

    private void Apply(PlatformViewPlacement[] placements)
    {
        if (placements.Length != 0) GetContainer();
        var coordinator = _coordinator!;
        foreach (var placement in placements) RunNow(token => coordinator.AttachAsync(placement, token));
        var handles = placements.Select(placement => placement.Handle).ToHashSet();
        foreach (var old in _placements.Where(old => !handles.Contains(old.Handle)))
        {
            try { RunNow(token => coordinator.DetachAsync(old.Handle, token)); }
            catch (DorotiCapabilityException) { /* Removed instances already disabled input and hide independently. */ }
        }
        _placements = placements;
        if (_overlay is { } overlay) overlay.Hidden = false;
        _overlay?.Window?.RecalculateKeyViewLoop();
    }

    private void ApplyLayers((AppKitPlatformRasterSurface Slot, int Order, CGRect Bounds)[] rasters, PlatformInputShield[] shields, PlatformBackdropSegment[] effects)
    {
        var parent = GetContainer();
        foreach (var slot in _rasters)
            if (!rasters.Any(raster => ReferenceEquals(raster.Slot, slot))) slot.Hidden = true;
        foreach (var (slot, order, bounds) in rasters)
        {
            if (slot.Superview != parent) parent.AddSubview(slot);
            if (slot.Frame != bounds) slot.Frame = bounds;
            slot.Layer!.ZPosition = order;
            slot.Hidden = false;
        }
        while (_shields.Count < shields.Length)
        {
            var shield = new ShieldView(_surface) { WantsLayer = true };
            _shields.Add(shield);
            parent.AddSubview(shield);
        }
        for (var index = 0; index < _shields.Count; index++)
        {
            var view = _shields[index];
            if (index >= shields.Length) { view.Hidden = true; continue; }
            var shield = shields[index];
            var a = shield.Transform.Map(shield.Bounds.topLeft);
            var b = shield.Transform.Map(shield.Bounds.bottomRight);
            var bounds = new Rect(a.dx, a.dy, b.dx, b.dy);
            if (shield.Clip is { } clip) bounds = bounds.intersect(clip);
            bounds = bounds.intersect(Rect.fromLTWH(0, 0, parent.Bounds.Width, parent.Bounds.Height));
            if (bounds.isEmpty) { view.Hidden = true; continue; }
            view.Frame = new CGRect(bounds.left, bounds.top, bounds.width, bounds.height);
            view.Layer!.ZPosition = shield.PaintOrder;
            view.Hidden = false;
        }
        while (_effects.Count < effects.Length)
        {
            var view = new AppKitPlatformBlurView();
            _effects.Add(view); parent.AddSubview(view);
        }
        for (var index = 0; index < _effects.Count; index++)
        {
            var view = _effects[index];
            if (index >= effects.Length) { view.Deactivate(); continue; }
            view.Hidden = false;
            var effect = effects[index];
            var bounds = effect.Bounds.intersect(Rect.fromLTWH(0, 0, parent.Bounds.Width, parent.Bounds.Height));
            view.Frame = new CGRect(bounds.left, bounds.top, Math.Max(0, bounds.width), Math.Max(0, bounds.height));
            view.Layer!.ZPosition = effect.PaintOrder;
            view.SetEffect(effect);
        }
        // Core Image samples AppKit sibling order as well as CALayer order.
        var children = parent.Subviews;
        var ordered = children.OrderBy(child => child.Layer?.ZPosition ?? 0).ToArray();
        if (!children.SequenceEqual(ordered))
        {
            NSView? previous = null;
            foreach (var child in ordered)
            {
                parent.AddSubview(child, NSWindowOrderingMode.Above, previous);
                previous = child;
            }
        }
        _visibleEffects = effects;
        _visibleRasters = rasters;
        _visibleShields = shields;
    }

    internal sealed class PreparedFrame(AppKitPlatformViewHost host, PlatformCompositionPlan plan,
        AppKitPlatformRasterSurface.RasterFrame[] frames, IDisposable[] gpuLeases) : IDisposable, IPreparedPlatformComposition
    {
        private readonly PlatformViewPlacement[] _previous = host._placements;
        private readonly (AppKitPlatformRasterSurface Slot, int Order, CGRect Bounds)[] _previousRasters = host._visibleRasters;
        private readonly PlatformInputShield[] _previousShields = host._visibleShields;
        private readonly PlatformBackdropSegment[] _previousEffects = host._visibleEffects;
        private readonly PlatformViewPlacement[] _next = plan.Parts.OfType<PlatformNativeSegment>().Select(part => part.Placement).ToArray();
        private bool _committed;
        private bool _disposed;
        private bool _sessionOwned;
        private readonly TaskCompletionSource _retired = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public Task Retirement => _retired.Task;
        public bool HasSubmitted => frames.Any(frame => frame.Submitted);
        public void Submit() { foreach (var frame in frames) frame.Submit(); }
        public void Present() { foreach (var frame in frames) frame.Present(); }
        public void Commit()
        {
            host._session ??= new(plan.Token.OwnerViewId, host._presenter);
            var epoch = host._session.SetEpochAsync(plan.Token.ViewEpoch, plan.Token.SurfaceGeneration);
            if (!epoch.IsCompleted) throw new InvalidOperationException("AppKit composition epoch cannot wait on the UI thread.");
            epoch.GetAwaiter().GetResult();
            host._presenter.Prepared = this;
            _sessionOwned = true;
            try
            {
                var submission = host._session.SubmitAsync(plan);
                if (!submission.IsCompleted) throw new InvalidOperationException("AppKit native commit unexpectedly suspended.");
                submission.GetAwaiter().GetResult();
            }
            finally { host._presenter.Prepared = null; }
        }
        public ValueTask CommitAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested(); CommitNative(); return ValueTask.CompletedTask;
        }
        private void CommitNative()
        {
            try
            {
                host.Apply(_next);
                host.ApplyLayers(frames.Select(frame => (frame.Slot, frame.PaintOrder, frame.Bounds)).ToArray(),
                    plan.Parts.OfType<PlatformShieldSegment>().Select(part => part.Shield).ToArray(),
                    plan.Parts.OfType<PlatformBackdropSegment>().ToArray());
                _committed = true;
            }
            catch { Rollback(); throw; }
        }
        public void Rollback()
        {
            // Include partially applied new attachments when restoring the old frame.
            host._placements = _next;
            try { host.Apply(_previous); host.ApplyLayers(_previousRasters, _previousShields, _previousEffects); }
            catch
            {
                // A concurrent disposal can make rollback impossible. Hide the batch instead of
                // leaving a partially applied frame interactive.
                if (host._overlay is { } overlay) overlay.Hidden = true;
                host._placements = [];
                throw;
            }
            _committed = false;
        }
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            try { foreach (var frame in frames) frame.Dispose(); }
            finally { foreach (var lease in gpuLeases) lease.Dispose(); _retired.TrySetResult(); if (!_sessionOwned) plan.Dispose(); }
        }
        // Native/GPU resources are retired by the product frame owner above.
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        public void Abort() { if (_committed) Rollback(); }
    }

    private sealed class CommitPresenter : IPlatformCompositionPresenter
    {
        internal PreparedFrame? Prepared;
        public ValueTask<IPreparedPlatformComposition> PrepareAsync(PlatformCompositionPlan plan, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult<IPreparedPlatformComposition>(Prepared ?? throw new InvalidOperationException("No AppKit frame prepared."));
        }
    }

    internal void DetachSurface()
    {
        CancelPending();
        foreach (var slot in _rasters) slot.Retire();
        _rasters.Clear();
        foreach (var shield in _shields) { shield.RemoveFromSuperview(); shield.Dispose(); }
        _shields.Clear();
        foreach (var effect in _effects) { effect.RemoveFromSuperview(); effect.Dispose(); }
        _effects.Clear();
        _visibleEffects = [];
        _visibleRasters = [];
        _visibleShields = [];
        _overlay?.RemoveFromSuperview();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        DetachSurface();
        if (_session is { } session) _ = session.DisposeAsync();
        // Instance cleanup runs through the dispatcher; keep their native parent alive until then.
        var overlay = _overlay; _overlay = null;
        if (_coordinator is { } coordinator) _ = ReleaseAsync(coordinator, overlay);
        else overlay?.Dispose();
    }
    private static async Task ReleaseAsync(PlatformViewCoordinator coordinator, NSView? overlay)
    {
        try { await coordinator.DisposeAsync(); }
        catch (Exception error) { System.Diagnostics.Trace.TraceError(error.ToString()); }
        finally
        {
            await new AppKitPlatformViewDispatcher().InvokeAsync(() => { overlay?.Dispose(); return ValueTask.CompletedTask; });
        }
    }
}
#endif
