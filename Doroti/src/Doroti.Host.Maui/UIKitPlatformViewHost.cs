#if IOS && !MACCATALYST
using UIKit;
using Foundation;
using CoreGraphics;
using Doroti.Hosting;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;
using Rect = Doroti.Ui.Rect;

namespace Doroti.Host.Maui;

/// <summary>Owner-local Metal/native interleaving in a layer-backed sibling of MTKView.</summary>
internal sealed class UIKitPlatformViewHost : IDisposable
{
    internal static PlatformEffectSupport MaterialEffects => UIKitPlatformBlurView.Support;
    private readonly List<UIKitPlatformBlurView> _effects = [];
    private PlatformBackdropSegment[] _visibleEffects = [];

    private sealed class OverlayView : UIView
    {
        public OverlayView() { Opaque = false; ClipsToBounds = true; BackgroundColor = UIColor.Clear; }
        public override UIView? HitTest(CGPoint point, UIEvent? evt)
        {
            if (Hidden || !UserInteractionEnabled || !Bounds.Contains(point)) return null;
            foreach (var child in Subviews.OrderByDescending(child => child.Layer.ZPosition))
                if (!child.Hidden && child.HitTest(child.ConvertPointFromView(point, this), evt) is { } hit) return hit;
            return null;
        }
    }

    private sealed class ShieldView(DorotiGraphiteView surface) : UIView
    {
        public override UIView? HitTest(CGPoint point, UIEvent? evt) =>
            !Hidden && Bounds.Contains(point) ? surface.Handler?.PlatformView as UIView : null;
    }

    private readonly DorotiGraphiteView _surface;
    private readonly MauiTextInputBridge _textInput;
    private OverlayView? _overlay;
    private UIWindow? _window;
    private Brightness _brightness = Brightness.light;

    private PlatformViewCoordinator? _coordinator;
    private PlatformCompositionSession? _session;
    private readonly CommitPresenter _presenter = new();
    private long _compositionFrame;
    private PlatformViewPlacement[] _placements = [];
    private readonly List<UIKitPlatformRasterSurface> _rasters = [];
    private readonly List<ShieldView> _shields = [];
    private PlatformInputShield[] _visibleShields = [];
    private (UIKitPlatformRasterSurface Slot, int Order, CGRect Bounds)[] _visibleRasters = [];
    private PreparedFrame? _pending;
    private bool _disposed;

    internal UIKitPlatformViewHost(DorotiGraphiteView surface, MauiTextInputBridge textInput)
    { _surface = surface; _textInput = textInput; }

    internal IEnumerable<IPlatformViewFactory> CreateFactories() =>
        new[] { "doroti/native-button", "doroti/native-editor", "doroti/webview" }
            .Select(type => new UIKitPlatformViewFactory(GetContainer, type, _textInput.YieldUIKitNativeFocus));

    internal bool IsConfigured => _coordinator is not null;
    internal bool HasComposition => _placements.Length != 0 || _visibleRasters.Length != 0 ||
        _visibleShields.Length != 0 || _visibleEffects.Length != 0 || _pending is not null;

    internal void Configure(PlatformViewCoordinator coordinator) => _coordinator = coordinator;

    private UIView GetContainer()
    {
        UIKitPlatformViewDispatcher.VerifyThread();
        ObjectDisposedException.ThrowIf(_disposed, this);
        var native = (_surface.Handler?.PlatformView as DorotiUIKitGraphiteView) ?? throw new InvalidOperationException("UIKit render surface is not attached.");
        var parent = native.Superview ?? throw new InvalidOperationException("UIKit render surface has no container.");
        if (_window is not null && native.Window is not null && native.Window != _window)
            throw new InvalidOperationException("An UIKit PlatformView owner cannot move between windows.");
        _window ??= native.Window;
        _overlay ??= new OverlayView();
        _overlay.OverrideUserInterfaceStyle = _brightness == Brightness.dark ? UIUserInterfaceStyle.Dark : UIUserInterfaceStyle.Light;
        if (_overlay.Superview != parent)
        {
            _overlay.RemoveFromSuperview();
            parent.InsertSubviewAbove(_overlay, native);
        }
        _overlay.Frame = native.Frame;
        return _overlay;
    }

    internal void Draw(SkiaSceneRenderer renderer, SKCanvas canvas, IReadOnlyList<SceneCommand> commands,
        DorotiFrameDescriptor descriptor, int width, int height, Brightness brightness)
    {
        UIKitPlatformViewDispatcher.VerifyThread();
        ObjectDisposedException.ThrowIf(_disposed, this);
        CancelPending();
        _brightness = brightness;
        var coordinator = _coordinator ?? throw new InvalidOperationException("UIKit PlatformView coordinator is missing.");
        if (_placements.Length == 0 && _visibleRasters.Length == 0 && _visibleShields.Length == 0 && !HasPlatformCommands(commands))
        {
            renderer.DrawPlatformRasterSegment(canvas, commands, width, height);
            return;
        }
        var token = new PlatformCompositionToken(descriptor.ViewId, descriptor.MetricsGeneration,
            ++_compositionFrame, descriptor.ResizeTargetGeneration, descriptor.DeviceScaleX, descriptor.DeviceScaleY);
        var plan = PlatformCompositionPlanner.Build(commands, token, coordinator, PlatformViewComposition.InterleavedComposition, MaterialEffects);
        var frames = new List<UIKitPlatformRasterSurface.RasterFrame>();
        var gpuLeases = new List<IDisposable>();
        try
        {
            // The session may reject before invoking its presenter. Submitted segment
            // work still needs native lifetime protection until the Metal marker retires.
            foreach (var nativePart in plan.Parts.OfType<PlatformNativeSegment>())
                gpuLeases.Add(coordinator.Retain(nativePart.Placement.Handle));
            if (plan.Parts.OfType<PlatformShieldSegment>().Any(part => !part.Shield.Transform.IsAxisAligned))
                throw new InvalidOperationException("UIKit input shields require axis-aligned rectangular bounds.");
            foreach (var effect in plan.Parts.OfType<PlatformBackdropSegment>())
            {
                effect.Style?.Validate();
                if (effect.Style?.Match != PlatformEffectMatchPolicy.MatchCommon || effect.Style.Saturation != 1 || effect.SigmaX != effect.SigmaY)
                    throw new NotSupportedException("UIKit material interpolation requires isotropic PlatformEffect MatchCommon; ExactSigma/saturation are not supported.");
                if (UIAccessibility.IsReduceTransparencyEnabled)
                    throw new NotSupportedException("Reduce Transparency is enabled; request SolidTint explicitly instead of reporting a material as blur.");
            }
            var segments = plan.Parts.OfType<PlatformRasterSegment>().ToArray();
            renderer.DrawPlatformRasterSegment(canvas, segments[0].Commands, width, height);
            // Triple-buffered segment layers are retained across frames; cap their total storage.
            if (checked((long)width * height * 4 * 3 * Math.Max(_rasters.Count, segments.Length - 1)) > 256L * 1024 * 1024)
                throw new NotSupportedException("UIKit platform raster storage exceeds 256 MiB.");
            var native = (_surface.Handler?.PlatformView as DorotiUIKitGraphiteView)!;
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

    // A render callback cannot suspend inside UIKit's draw transaction. Cancel queued operations
    // on contention instead of blocking the main thread behind a retiring instance.
    private static void RunNow(Func<CancellationToken, ValueTask> action)
    {
        using var cancellation = new CancellationTokenSource();
        var task = action(cancellation.Token);
        if (!task.IsCompleted)
        {
            cancellation.Cancel();
            _ = ObserveCancellation(task);
            throw new InvalidOperationException("UIKit native placement is busy; the frame was not committed.");
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

    }

    private void ApplyLayers((UIKitPlatformRasterSurface Slot, int Order, CGRect Bounds)[] rasters, PlatformInputShield[] shields, PlatformBackdropSegment[] effects)
    {
        var parent = GetContainer();
        foreach (var slot in _rasters)
            if (!rasters.Any(raster => ReferenceEquals(raster.Slot, slot))) slot.Hidden = true;
        foreach (var (slot, order, bounds) in rasters)
        {
            if (slot.Superview != parent) parent.AddSubview(slot);
            if (slot.Frame != bounds) slot.Frame = bounds;
            slot.LayoutIfNeeded();
            slot.Layer!.ZPosition = order;
            slot.Hidden = false;
        }
        while (_shields.Count < shields.Length)
        {
            var shield = new ShieldView(_surface);
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
            var effect = new UIKitPlatformBlurView();
            _effects.Add(effect); parent.AddSubview(effect);
        }
        for (var index = 0; index < _effects.Count; index++)
        {
            var view = _effects[index];
            view.Hidden = index >= effects.Length;
            if (view.Hidden) continue;
            var effect = effects[index];
            var bounds = effect.Bounds.intersect(Rect.fromLTWH(0, 0, parent.Bounds.Width, parent.Bounds.Height));
            view.Frame = new CGRect(bounds.left, bounds.top, Math.Max(0, bounds.width), Math.Max(0, bounds.height));
            view.Layer.ZPosition = effect.PaintOrder;
            view.SetSigma(effect.SigmaX);
        }
        // Keep the actual UIKit hierarchy in paint order as well as CALayer order. Material
        // backdrop sampling and accessibility traverse this hierarchy, not our hit-test loop.
        var children = parent.Subviews;
        var ordered = children.OrderBy(child => child.Layer.ZPosition).ToArray();
        if (!children.SequenceEqual(ordered))
            foreach (var child in ordered) parent.BringSubviewToFront(child);
        _visibleEffects = effects;
        _visibleRasters = rasters;
        _visibleShields = shields;
    }

    internal sealed class PreparedFrame(UIKitPlatformViewHost host, PlatformCompositionPlan plan,
        UIKitPlatformRasterSurface.RasterFrame[] frames, IDisposable[] gpuLeases) : IDisposable, IPreparedPlatformComposition
    {
        private readonly PlatformViewPlacement[] _previous = host._placements;
        private readonly (UIKitPlatformRasterSurface Slot, int Order, CGRect Bounds)[] _previousRasters = host._visibleRasters;
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
            if (!epoch.IsCompleted) throw new InvalidOperationException("UIKit composition epoch cannot wait on the UI thread.");
            epoch.GetAwaiter().GetResult();
            host._presenter.Prepared = this;
            _sessionOwned = true;
            try
            {
                var submission = host._session.SubmitAsync(plan);
                if (!submission.IsCompleted) throw new InvalidOperationException("UIKit native commit unexpectedly suspended.");
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
            return ValueTask.FromResult<IPreparedPlatformComposition>(Prepared ?? throw new InvalidOperationException("No UIKit frame prepared."));
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
        _surface.PlatformViews = null;
        DetachSurface();
        if (_session is { } session) _ = session.DisposeAsync();
        // Instance cleanup runs through the dispatcher; keep their native parent alive until then.
        var overlay = _overlay; _overlay = null;
        if (_coordinator is { } coordinator) _ = ReleaseAsync(coordinator, overlay);
        else overlay?.Dispose();
    }
    private static async Task ReleaseAsync(PlatformViewCoordinator coordinator, UIView? overlay)
    {
        try { await coordinator.DisposeAsync(); }
        catch (Exception error) { System.Diagnostics.Trace.TraceError(error.ToString()); }
        finally
        {
            await new UIKitPlatformViewDispatcher().InvokeAsync(() => { overlay?.Dispose(); return ValueTask.CompletedTask; });
        }
    }
}
#endif
