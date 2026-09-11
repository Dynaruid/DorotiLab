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
    private PlatformViewPlacement[] _placements = [];
    private readonly List<AppKitPlatformRasterSurface> _rasters = [];
    private readonly List<ShieldView> _shields = [];
    private PlatformInputShield[] _visibleShields = [];
    private (AppKitPlatformRasterSurface Slot, int Order)[] _visibleRasters = [];
    private PreparedFrame? _pending;
    private bool _disposed;

    internal AppKitPlatformViewHost(DorotiMacOSMetalSurface surface, MauiTextInputBridge textInput)
    { _surface = surface; _textInput = textInput; }

    internal IEnumerable<IPlatformViewFactory> CreateFactories() =>
    [new AppKitPlatformViewFactory(GetContainer, false, _textInput.YieldMacOSNativeFocus, RestoreFocus, interleaved: true),
     new AppKitPlatformViewFactory(GetContainer, true, _textInput.YieldMacOSNativeFocus, RestoreFocus, interleaved: true)];

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
        if (_placements.Length == 0 && _visibleRasters.Length == 0 && _visibleShields.Length == 0 && !HasPlatformCommands(commands))
        {
            renderer.DrawPlatformRasterSegment(canvas, commands, width, height);
            return;
        }
        var token = new PlatformCompositionToken(descriptor.ViewId, descriptor.MetricsGeneration,
            descriptor.SceneSequence, descriptor.ResizeTargetGeneration, descriptor.DeviceScaleX, descriptor.DeviceScaleY);
        var plan = PlatformCompositionPlanner.Build(commands, token, coordinator, PlatformViewComposition.InterleavedComposition);
        var frames = new List<AppKitPlatformRasterSurface.RasterFrame>();
        try
        {
            if (plan.Parts.OfType<PlatformShieldSegment>().Any(part => !part.Shield.Transform.IsAxisAligned))
                throw new InvalidOperationException("AppKit input shields require axis-aligned rectangular bounds.");
            var segments = plan.Parts.OfType<PlatformRasterSegment>().ToArray();
            renderer.DrawPlatformRasterSegment(canvas, segments[0].Commands, width, height);
            var native = _surface.NativeView!;
            for (var index = 1; index < segments.Length; index++)
            {
                if (_rasters.Count < index) _rasters.Add(native.CreatePlatformRasterSurface());
                frames.Add(_rasters[index - 1].Prepare(renderer, segments[index], width, height, descriptor.DeviceScaleX));
            }
            _pending = new PreparedFrame(this, plan, frames.ToArray());
        }
        catch { foreach (var frame in frames) frame.Dispose(); plan.Dispose(); throw; }
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

    private void ApplyLayers((AppKitPlatformRasterSurface Slot, int Order)[] rasters, PlatformInputShield[] shields)
    {
        var parent = GetContainer();
        foreach (var slot in _rasters) slot.Hidden = true;
        foreach (var (slot, order) in rasters)
        {
            if (slot.Superview != parent) parent.AddSubview(slot);
            slot.Frame = parent.Bounds;
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
            view.Hidden = true;
            if (index >= shields.Length) continue;
            var shield = shields[index];
            var a = shield.Transform.Map(shield.Bounds.topLeft);
            var b = shield.Transform.Map(shield.Bounds.bottomRight);
            var bounds = new Rect(a.dx, a.dy, b.dx, b.dy);
            if (shield.Clip is { } clip) bounds = bounds.intersect(clip);
            bounds = bounds.intersect(Rect.fromLTWH(0, 0, parent.Bounds.Width, parent.Bounds.Height));
            if (bounds.isEmpty) continue;
            view.Frame = new CGRect(bounds.left, bounds.top, bounds.width, bounds.height);
            view.Layer!.ZPosition = shield.PaintOrder;
            view.Hidden = false;
        }
        _visibleRasters = rasters;
        _visibleShields = shields;
    }

    internal sealed class PreparedFrame(AppKitPlatformViewHost host, PlatformCompositionPlan plan,
        AppKitPlatformRasterSurface.RasterFrame[] frames) : IDisposable
    {
        private readonly PlatformViewPlacement[] _previous = host._placements;
        private readonly (AppKitPlatformRasterSurface Slot, int Order)[] _previousRasters = host._visibleRasters;
        private readonly PlatformInputShield[] _previousShields = host._visibleShields;
        private readonly PlatformViewPlacement[] _next = plan.Parts.OfType<PlatformNativeSegment>().Select(part => part.Placement).ToArray();
        private bool _committed;
        public bool HasSubmitted => frames.Any(frame => frame.Submitted);
        public void Submit() { foreach (var frame in frames) frame.Submit(); }
        public void Present() { foreach (var frame in frames) frame.Present(); }
        public void Commit()
        {
            try
            {
                host.Apply(_next);
                host.ApplyLayers(frames.Select(frame => (frame.Slot, frame.PaintOrder)).ToArray(),
                    plan.Parts.OfType<PlatformShieldSegment>().Select(part => part.Shield).ToArray());
                _committed = true;
            }
            catch { Rollback(); throw; }
        }
        public void Rollback()
        {
            // Include partially applied new attachments when restoring the old frame.
            host._placements = _next;
            try { host.Apply(_previous); host.ApplyLayers(_previousRasters, _previousShields); }
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
        public void Dispose() { foreach (var frame in frames) frame.Dispose(); plan.Dispose(); }
        public void Abort() { if (_committed) Rollback(); }
    }

    internal void DetachSurface()
    {
        CancelPending();
        foreach (var slot in _rasters) slot.Retire();
        _rasters.Clear();
        foreach (var shield in _shields) { shield.RemoveFromSuperview(); shield.Dispose(); }
        _shields.Clear();
        _visibleRasters = [];
        _visibleShields = [];
        _overlay?.RemoveFromSuperview();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        DetachSurface();
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
