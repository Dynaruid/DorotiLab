#if WINDOWS
using Doroti.Host.SharedWindows;
using Doroti.Hosting;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using SkiaSharp;
using Canvas = Microsoft.UI.Xaml.Controls.Canvas;

namespace Doroti.Host.Maui;

/// <summary>Owner-local WinUI overlays. Raster/native interleaving is explicitly unsupported.</summary>
internal sealed class WindowsMauiPlatformViewHost : IDisposable
{
    private readonly DorotiWindowsDxgiSurface _surface;
    private readonly MauiTextInputBridge _textInput;
    internal readonly WindowsPlatformViewDispatcher Dispatcher = new();
    private readonly Canvas _overlay = new();
    private DorotiWindowsDxgiHost? _parent;
    private PlatformViewCoordinator? _coordinator;
    private PlatformViewHandle[] _visible = [];
    private long _frame;
    private long _attachment;
    private nint _window;
    private volatile bool _enabled;
    private volatile bool _disposed;

    internal WindowsMauiPlatformViewHost(DorotiWindowsDxgiSurface surface, MauiTextInputBridge textInput)
    {
        _surface = surface;
        _textInput = textInput;
        Canvas.SetZIndex(_overlay, 100);
    }

    internal bool Enabled => _enabled && !_disposed;
    internal Canvas Container
    {
        get
        {
            Dispatcher.VerifyThread();
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (!_enabled || _parent is null || _surface.WindowHandle == 0)
                throw new InvalidOperationException("The MAUI PlatformView surface is not attached.");
            var window = _surface.WindowHandle;
            if (_window != 0 && _window != window)
                throw new InvalidOperationException("A MAUI PlatformView owner cannot move between windows.");
            _window = window;
            return _overlay;
        }
    }

    internal IEnumerable<IPlatformViewFactory> CreateFactories(Func<IApplicationResourceHostCapability> resources) =>
        new[] { "doroti/native-button", "doroti/native-editor", "doroti/webview" }
            .Select(type => new WindowsMauiPlatformViewFactory(this, type, resources));

    internal void Configure(PlatformViewCoordinator coordinator)
    {
        Dispatcher.VerifyThread();
        _coordinator = coordinator;
        coordinator.SceneSupportEvaluator = request =>
        {
            if (request.HasForegroundRaster || request.HasBackdrop) return new(false, "MAUI overlay scenes cannot interleave foreground raster/backdrop.");
            if (request.Items is not { } items) return new(false, "Bounds/order are required to preflight disjoint MAUI overlays.");
            var bounds = new List<Doroti.Ui.Rect>();
            foreach (var item in items)
            {
                if (!item.Bounds.IsFinite || !item.Transform.IsFinite || item.Transform.M11 != 1 || item.Transform.M22 != 1
                    || item.Transform.M12 != 0 || item.Transform.M21 != 0) return new(false, "Only finite translation/rectangular clip is supported.");
                var region = item.Bounds.shift(new(item.Transform.Dx, item.Transform.Dy));
                if (item.Clip is { } clip) region = region.intersect(clip);
                if (bounds.Any(other => other.overlaps(region))) return new(false, "Native overlay regions must be disjoint.");
                bounds.Add(region);
            }
            return new(true);
        };
        _enabled = true;
        if (_surface.NativeHost is { } parent) Connect(parent);
    }

    internal void Connect(DorotiWindowsDxgiHost parent)
    {
        Dispatcher.VerifyThread();
        if (!Enabled || _parent == parent) return;
        var window = _surface.WindowHandle;
        if (_window != 0 && window != 0 && window != _window)
            throw new InvalidOperationException("A MAUI PlatformView owner cannot move between windows.");
        if (window != 0) _window = window;
        if (_parent is { } old) Disconnect(old);
        _parent = parent;
        parent.Children.Add(_overlay);
        parent.SizeChanged += SizeChanged;
        UpdateClip();
        _attachment++;
    }

    internal void Disconnect(DorotiWindowsDxgiHost parent)
    {
        Dispatcher.VerifyThread();
        if (_parent != parent) return;
        _attachment++;
        parent.SizeChanged -= SizeChanged;
        parent.Children.Remove(_overlay);
        _parent = null;
    }

    private void SizeChanged(object sender, SizeChangedEventArgs args) => UpdateClip();
    private void UpdateClip()
    {
        if (_parent is { } parent)
            _overlay.Clip = new RectangleGeometry { Rect = new(0, 0, Math.Max(0, parent.ActualWidth), Math.Max(0, parent.ActualHeight)) };
    }

    internal void YieldTextInput() => _textInput.YieldWindowsNativeFocus();
    internal void RestoreFocus() => _surface.RequestFocus(true);

    internal bool OwnsElement(object source)
    {
        var element = source as DependencyObject;
        while (element is not null)
        {
            if (element == _overlay) return true;
            element = VisualTreeHelper.GetParent(element);
        }
        return false;
    }

    internal void Draw(SkiaSceneRenderer renderer, SKCanvas canvas, IReadOnlyList<SceneCommand> commands,
        DorotiFrameDescriptor descriptor, int width, int height)
    {
        var frame = Interlocked.Increment(ref _frame);
        if (Volatile.Read(ref _visible).Length == 0 && !HasPlatformCommands(commands))
        {
            renderer.DrawPlatformRasterSegment(canvas, commands, width, height);
            return;
        }
        // Planning/rasterization run on the raster worker. All XAML calls are queued to the owner.
        var token = new PlatformCompositionToken(descriptor.ViewId, descriptor.MetricsGeneration,
            frame, descriptor.ResizeTargetGeneration, descriptor.DeviceScaleX, descriptor.DeviceScaleY);
        PlatformCompositionPlan plan;
        try { plan = PlatformCompositionPlanner.Build(commands, token, _coordinator!, PlatformViewComposition.NativeOverlay); }
        catch (DorotiCapabilityException) when (_coordinator!.ReferencesRetiredHandle(commands))
        {
            // Removing a widget disables its native instance before a replacement scene
            // arrives. Its retained replay is superseded, not a graphics/device failure.
            throw new DorotiFrameSupersededException("Windows MAUI scene references a retiring native view.");
        }
        try
        {
            foreach (var raster in plan.Parts.OfType<PlatformRasterSegment>())
                renderer.DrawPlatformRasterSegment(canvas, raster.Commands, width, height);
            var attachment = Volatile.Read(ref _attachment);
            var work = Dispatcher.InvokeAsync(async () =>
            {
                try
                {
                    if (_disposed || _parent is null || attachment != _attachment || token.FrameNumber != Volatile.Read(ref _frame)) return;
                    var placements = plan.Parts.OfType<PlatformNativeSegment>().Select(part => part.Placement).ToArray();
                    var next = placements.Select(p => p.Handle).ToArray();
                    foreach (var handle in _visible.Except(next))
                    {
                        try
                        {
                            if (_coordinator!.GetState(handle) is PlatformViewState.Attached or PlatformViewState.Hidden)
                                await _coordinator.DetachAsync(handle);
                        }
                        catch (DorotiCapabilityException) { /* Already retired; keep applying the live replacement. */ }
                    }
                    foreach (var placement in placements) await _coordinator!.AttachAsync(placement);
                    _coordinator!.RecordPlacementReceipt(plan);
                    _visible = next;
                }
                finally { plan.Dispose(); }
            });
            _ = Observe(work);
        }
        catch { plan.Dispose(); throw; }
    }

    private static bool HasPlatformCommands(IReadOnlyList<SceneCommand> commands) => commands.Any(command =>
        command.Operation is "platformView" or "inputShield"
        || command.RetainedCommands is { } retainedCommands && HasPlatformCommands(retainedCommands));

    private static async Task Observe(ValueTask work)
    {
        try { await work; }
        catch (DorotiCapabilityException) { /* A removed widget cannot be revived by a queued overlay frame. */ }
        catch (Exception error) { DorotiMauiSurface.WriteFailure(error); }
    }

    public void Dispose()
    {
        Dispatcher.VerifyThread();
        if (_disposed) return;
        _disposed = true;
        _attachment++;
        if (_parent is { } parent) Disconnect(parent);
        // Coordinator disposal belongs to the application boundary; keep its UI queue alive until it drains.
        Dispatcher.DrainShutdown(_coordinator?.DisposeAsync().AsTask() ?? Task.CompletedTask);
        _overlay.Children.Clear();
        Dispatcher.Dispose();
    }
}
#endif
