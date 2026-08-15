using System.Globalization;
using System.Runtime.InteropServices;
using Doroti.Core;
using Doroti.Flutter.Hosting;
using Doroti.Flutter.Ui;
using Doroti.Host.Desktop;
using Doroti.Platform;
using GraphicsOffset = Doroti.Graphics.Offset;
using GraphicsSize = Doroti.Graphics.Size;
using PlatformAppLifecycleState = Doroti.Flutter.Ui.AppLifecycleState;
using PlatformKeyData = Doroti.Flutter.Ui.KeyData;
using PlatformMessageHandler = Doroti.Flutter.Ui.PlatformMessageHandler;
using PlatformPointerData = Doroti.Flutter.Ui.PointerData;
using UiSize = Doroti.Flutter.Ui.Size;

namespace Doroti.Host.Desktop.Flutter;

/// <summary>Composition root joining managed dart:ui contracts to the source-ported desktop host.</summary>
public sealed class DesktopFlutterHost : IDisposable
{
    private readonly DesktopWindowBackend _backend;
    private readonly HashSet<FlutterView> _views = [];
    private readonly Dictionary<ulong, DesktopGraphicsAndSemanticsCapabilities> _graphics = [];
    private readonly Dictionary<ulong, DesktopFlutterViewCapability> _windows = [];
    private readonly Dictionary<ulong, FlutterHostSession> _sessions = [];
    private bool _disposed;

    public DesktopFlutterHost(DesktopWindowBackend backend) =>
        _backend = backend ?? throw new ArgumentNullException(nameof(backend));

    public FlutterView CreateView(
        PlatformDispatcher dispatcher,
        ulong viewId,
        FlutterViewConfiguration configuration,
        FlutterApplicationBoundary? application = null)
    {
        ArgumentNullException.ThrowIfNull(dispatcher);
        ArgumentNullException.ThrowIfNull(configuration);
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!configuration.logicalSize.IsFinite || configuration.logicalSize.IsEmpty)
        {
            throw new ArgumentOutOfRangeException(nameof(configuration));
        }

        var window = new DesktopFlutterViewCapability(_backend, viewId, configuration);
        var graphics = new DesktopGraphicsAndSemanticsCapabilities(viewId, window.Window);
        var hasPlatformServices = window.TryCreatePlatformServicesCapability(out var platformServices);
        var platformMessages = new DesktopPlatformMessageCapability(
            hasPlatformServices ? platformServices : null);
        var targetIdentity = $"{RuntimeInformation.RuntimeIdentifier}/win32-wgl";
        var capabilities = new FlutterViewCapabilities(targetIdentity)
            .Register<IViewHostCapability>(FlutterCapabilityIds.WindowLifecycle, window)
            .Register<IViewHostCapability>(FlutterCapabilityIds.ViewLifecycleMetrics, window)
            .Register<IInputHostCapability>(FlutterCapabilityIds.InputEvents, window)
            .Register<IPlatformEnvironmentHostCapability>(FlutterCapabilityIds.PlatformEnvironment, new DesktopPlatformEnvironmentCapability())
            .Register<ISceneHostCapability>(FlutterCapabilityIds.GraphicsScene, graphics)
            .Register<IParagraphHostCapability>(FlutterCapabilityIds.GraphicsText, graphics)
            .Register<IImageHostCapability>(FlutterCapabilityIds.GraphicsImage, graphics)
            .Register<ISemanticsHostCapability>(FlutterCapabilityIds.AccessibilitySemantics, graphics);

        if (application is null)
        {
            capabilities.Register<IPlatformMessageHostCapability>(FlutterCapabilityIds.PlatformMessaging, platformMessages);
        }
        else
        {
            application.Configure(capabilities, platformMessages);
        }

        if (window.TryCreateFrameCapability(out var frame))
        {
            capabilities.Register<IFrameHostCapability>(FlutterCapabilityIds.ViewFrameDispatch, frame);
        }
        if (hasPlatformServices)
        {
            capabilities.Register<IPlatformServicesHostCapability>(FlutterCapabilityIds.PlatformServices, platformServices);
        }
        if (window.TryCreateTextInputCapability(out var textInput))
        {
            capabilities.Register<ITextInputHostCapability>(FlutterCapabilityIds.TextInput, textInput);
        }

        try
        {
            var view = dispatcher.RegisterView(viewId, capabilities);
            _views.Add(view);
            _graphics.Add(viewId, graphics);
            _windows.Add(viewId, window);
            return view;
        }
        catch
        {
            capabilities.Dispose();
            throw;
        }
    }

    public DesktopFlutterFrameDiagnostics GetFrameDiagnostics(ulong viewId) =>
        _graphics.TryGetValue(viewId, out var graphics)
            ? graphics.Diagnostics
            : throw new KeyNotFoundException($"Flutter view {viewId} is not registered with this host.");

    public DesktopFlutterFoundationDiagnostics GetFoundationDiagnostics() => new(
        _views.Count,
        _graphics.Count,
        _sessions.Count,
        _views.Select(view => view.targetIdentity).Distinct(StringComparer.Ordinal).ToArray());

    /// <summary>Captures the target-neutral diagnostic hooks consumed by packaged smoke tests and DorotiDemoApp.</summary>
    public DesktopFlutterTargetDiagnostics GetTargetDiagnostics(ulong viewId)
    {
        if (!_graphics.TryGetValue(viewId, out var graphics) || !_windows.TryGetValue(viewId, out var window))
        {
            throw new KeyNotFoundException($"Flutter view {viewId} is not registered with this host.");
        }
        var view = _views.Single(item => item.viewId == viewId);
        return window.CaptureDiagnostics(graphics.Diagnostics, view.targetIdentity, view.registeredCapabilityIds);
    }

    public DesktopFlutterRetainedDiagnostics GetRetainedDiagnosticsForValidation(ulong viewId) =>
        _graphics.TryGetValue(viewId, out var graphics)
            ? graphics.RetainedDiagnostics
            : throw new KeyNotFoundException($"Flutter view {viewId} is not registered with this host.");

    public nint GetNativeWindowHandle(ulong viewId) =>
        _graphics.TryGetValue(viewId, out var graphics)
            ? graphics.NativeWindowHandle
            : throw new KeyNotFoundException($"Flutter view {viewId} is not registered with this host.");

    public SemanticsTreeSnapshot? GetSemanticsSnapshotForValidation(ulong viewId)
    {
        if (!_windows.TryGetValue(viewId, out var window) ||
            !window.Window.TryGetFeature<IAccessibilityDiagnostics>(out var accessibility) ||
            accessibility is null)
        {
            throw new NotSupportedException("The Flutter view does not expose semantics diagnostics.");
        }
        return accessibility.LastSnapshot;
    }

    public Task<DesktopFlutterPixelReadback> CaptureNextFrameAsync(ulong viewId) =>
        _graphics.TryGetValue(viewId, out var graphics)
            ? graphics.CaptureNextFrameAsync()
            : Task.FromException<DesktopFlutterPixelReadback>(
                new KeyNotFoundException($"Flutter view {viewId} is not registered with this host."));

    public void FailNextGpuFrameForValidation(ulong viewId)
    {
        if (!_graphics.TryGetValue(viewId, out var graphics))
        {
            throw new KeyNotFoundException($"Flutter view {viewId} is not registered with this host.");
        }
        graphics.FailNextFrameForValidation();
    }

    public void PostPointerTapForValidation(ulong viewId, double logicalX, double logicalY)
    {
        if (!_windows.TryGetValue(viewId, out var window))
        {
            throw new KeyNotFoundException($"Flutter view {viewId} is not registered with this host.");
        }
        if (!window.Window.TryGetFeature<IWindowInputTestController>(out var input) || input is null)
        {
            throw new NotSupportedException("The Flutter view does not expose native input validation.");
        }
        // A physical click focuses its top-level view before pointer dispatch.
        // Keep the native message-queue validation path aligned with that
        // ordering even when the view did not previously own keyboard focus.
        if (window.Window.TryGetFeature<IWindowFocusController>(out var focus) && focus is not null)
        {
            focus.RequestFocus(true);
        }
        input.PostPointerTap(new GraphicsOffset(logicalX, logicalY));
    }

    public void PostPointerMoveForValidation(ulong viewId, double logicalX, double logicalY) =>
        RequireValidationInput(viewId).PostPointerMove(new GraphicsOffset(logicalX, logicalY));

    public void PostPointerLeaveForValidation(ulong viewId, double logicalX, double logicalY) =>
        RequireValidationInput(viewId).PostPointerLeave(new GraphicsOffset(logicalX, logicalY));

    public void PostPointerDownForValidation(ulong viewId, double logicalX, double logicalY) =>
        RequireValidationInput(viewId).PostPointerDown(new GraphicsOffset(logicalX, logicalY));

    public void PostPointerUpForValidation(ulong viewId, double logicalX, double logicalY) =>
        RequireValidationInput(viewId).PostPointerUp(new GraphicsOffset(logicalX, logicalY));

    public void PostPointerDragForValidation(
        ulong viewId,
        double logicalStartX,
        double logicalStartY,
        double logicalEndX,
        double logicalEndY)
    {
        var input = RequireValidationInput(viewId);
        input.PostPointerDrag(
            new GraphicsOffset(logicalStartX, logicalStartY),
            new GraphicsOffset(logicalEndX, logicalEndY));
    }

    public void PostPointerWheelForValidation(
        ulong viewId,
        double logicalX,
        double logicalY,
        double wheelDeltaX,
        double wheelDeltaY)
    {
        var input = RequireValidationInput(viewId);
        input.PostPointerWheel(
            new GraphicsOffset(logicalX, logicalY),
            new GraphicsOffset(wheelDeltaX, wheelDeltaY));
    }

    public void PostKeyboardActivationForValidation(ulong viewId, uint logicalKey) =>
        RequireValidationInput(viewId).PostKeyboardActivation(logicalKey);

    public void PostTextInputForValidation(ulong viewId, string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        RequireValidationInput(viewId).PostTextInput(text);
    }

    private IWindowInputTestController RequireValidationInput(ulong viewId)
    {
        if (!_windows.TryGetValue(viewId, out var window))
        {
            throw new KeyNotFoundException($"Flutter view {viewId} is not registered with this host.");
        }
        if (!window.Window.TryGetFeature<IWindowInputTestController>(out var input) || input is null)
        {
            throw new NotSupportedException("The Flutter view does not expose native input validation.");
        }
        return input;
    }

    public FlutterView CreateView(
        FlutterHostSession session,
        ulong viewId,
        FlutterViewConfiguration configuration,
        FlutterApplicationBoundary? application = null)
    {
        ArgumentNullException.ThrowIfNull(session);
        if (session.state != FlutterHostSessionState.running)
        {
            throw new InvalidOperationException("The Flutter host session must be running before a view is created.");
        }
        var view = CreateView(session.dispatcher, viewId, configuration, application);
        try
        {
            session.AttachView(view);
            _sessions.Add(viewId, session);
            return view;
        }
        catch
        {
            _views.Remove(view);
            _graphics.Remove(viewId);
            _windows.Remove(viewId);
            view.Dispose();
            throw;
        }
    }

    /// <summary>Returns the source-ported platform services used by live validation clients.</summary>
    public IPlatformServicesHostCapability GetPlatformServicesForValidation(ulong viewId)
    {
        if (!_windows.TryGetValue(viewId, out var window) ||
            !window.TryCreatePlatformServicesCapability(out var capability))
        {
            throw new NotSupportedException($"Flutter view {viewId} does not expose platform services validation.");
        }
        return capability;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        foreach (var view in _views.ToArray())
        {
            if (_sessions.Remove(view.viewId, out var session))
            {
                session.DetachView(view);
            }
            view.Dispose();
        }
        _views.Clear();
        _graphics.Clear();
        _windows.Clear();
    }
}

public sealed record DesktopFlutterFoundationDiagnostics(
    int RegisteredViews,
    int FlutterSurfaces,
    int SessionAttachedViews,
    IReadOnlyList<string> TargetIdentities);

public sealed record DesktopFlutterInputDiagnostics(
    InputCapabilities Capabilities,
    long PointerPackets,
    long KeyPackets,
    long FocusChanges,
    long MetricsChanges);

public sealed record DesktopFlutterAutomationDiagnostics(long? Generation, int NodeCount);

public sealed record DesktopFlutterCursorDiagnostics(
    long Requests,
    FlutterMouseCursorKind? LastRequested);

public sealed record DesktopFlutterPixelReadback(
    ulong FrameId,
    int Width,
    int Height,
    int RowBytes,
    byte[] Bgra8888Pixels);

public sealed record DesktopFlutterTargetDiagnostics(
    string SchemaVersion,
    string TargetIdentity,
    IReadOnlyList<string> CapabilityIds,
    DesktopFlutterFrameDiagnostics Frame,
    DesktopFlutterInputDiagnostics Input,
    DesktopFlutterCursorDiagnostics Cursor,
    DesktopFlutterAutomationDiagnostics Automation,
    WindowCoordinateSnapshot Coordinates,
    NativeResourceSnapshot Resources);

internal sealed class DesktopFlutterViewCapability :
    IViewHostCapability,
    IInputHostCapability,
    IViewFocusRequestCapability,
    IWindowEventSink,
    IRawInputSink
{
    private readonly ulong _viewId;
    private readonly IWindow _window;
    private readonly Action<Action> _postUi;
    private readonly Dictionary<ulong, GraphicsOffset> _lastPointerPositions = [];
    private PlatformAppLifecycleState _lifecycle = PlatformAppLifecycleState.detached;
    private bool _focused;
    private bool _minimized;
    private long _pointerPackets;
    private long _keyPackets;
    private long _focusChanges;
    private long _metricsChanges;
    private DesktopPlatformServicesCapability? _platformServices;
    private bool _disposed;

    internal DesktopFlutterViewCapability(
        DesktopWindowBackend backend,
        ulong viewId,
        FlutterViewConfiguration configuration)
    {
        _viewId = viewId;
        _postUi = backend.Post;
        _window = backend.CreateWindow(
            new(configuration.title, new GraphicsSize(configuration.logicalSize.width, configuration.logicalSize.height)),
            this);
        _minimized = _window.Metrics.IsMinimized;
        _lifecycle = _minimized ? PlatformAppLifecycleState.hidden : PlatformAppLifecycleState.inactive;
        _window.RawInput.Attach(this);
    }

    internal IWindow Window => _window;

    internal DesktopFlutterTargetDiagnostics CaptureDiagnostics(
        DesktopFlutterFrameDiagnostics frame,
        string targetIdentity,
        IReadOnlyCollection<string> capabilityIds)
    {
        if (!_window.TryGetFeature<IWindowCoordinateDiagnostics>(out var coordinates) || coordinates is null ||
            !_window.TryGetFeature<INativeResourceDiagnostics>(out var resources) || resources is null ||
            !_window.TryGetFeature<IAccessibilityDiagnostics>(out var accessibility) || accessibility is null)
        {
            throw new NotSupportedException("The active target does not expose the G5-6 diagnostic closure.");
        }
        var snapshot = accessibility.LastSnapshot;
        return new(
            "doroti.desktop-flutter-target-diagnostics/v1",
            targetIdentity,
            capabilityIds.Order(StringComparer.Ordinal).ToArray(),
            frame,
            new(_window.RawInput.Capabilities, _pointerPackets, _keyPackets, _focusChanges, _metricsChanges),
            _platformServices?.Diagnostics ?? new(0, null),
            new(snapshot?.Generation, snapshot is null ? 0 : CountNodes(snapshot.Root)),
            coordinates.Coordinates,
            resources.Snapshot);
    }

    public ViewMetrics Metrics => Convert(_window.Metrics, _lifecycle);

    public event Action<ViewMetrics>? MetricsChanged;

    public event Action<PlatformAppLifecycleState>? LifecycleChanged;

    public event Action? CloseRequested;

    public event Action? Closed;

    public event Action<PointerDataPacket>? PointerData;

    public event Action<PlatformKeyData>? KeyData;

    public event Action<RawFocusData>? FocusData;

    public void RequestFocus(ViewFocusState state, ViewFocusDirection direction)
    {
        _ = direction;
        if (!_window.TryGetFeature<IWindowFocusController>(out var focus) || focus is null)
        {
            throw new NotSupportedException("The active desktop window does not expose focus control.");
        }
        focus.RequestFocus(state == ViewFocusState.focused);
    }

    public void Show() => _window.Show();

    public void Resize(UiSize logicalSize) => _window.Resize(new(logicalSize.width, logicalSize.height));

    public void Close() => _window.Close();

    public bool TryCreateFrameCapability(out IFrameHostCapability capability)
    {
        if (_window.TryGetFeature<IFrameDispatcher>(out var dispatcher) && dispatcher is not null)
        {
            capability = new DesktopFrameCapability(dispatcher);
            return true;
        }
        capability = null!;
        return false;
    }

    public bool TryCreatePlatformServicesCapability(out IPlatformServicesHostCapability capability)
    {
        if (_platformServices is not null)
        {
            capability = _platformServices;
            return true;
        }
        if (_window.TryGetFeature<IClipboard>(out var clipboard) && clipboard is not null &&
            _window.TryGetFeature<ICursorController>(out var cursor) && cursor is not null)
        {
            _platformServices = new DesktopPlatformServicesCapability(clipboard, cursor, _window.Id);
            capability = _platformServices;
            return true;
        }
        capability = null!;
        return false;
    }

    public bool TryCreateTextInputCapability(out ITextInputHostCapability capability)
    {
        if (_window.TryGetFeature<ITextInputConnection>(out var connection) && connection is not null &&
            _window.TryGetFeature<ITextInputGeometry>(out var geometry) && geometry is not null)
        {
            capability = new DesktopTextInputCapability(connection, geometry);
            return true;
        }
        capability = null!;
        return false;
    }

    public void OnMetricsChanged(WindowId window, WindowMetrics metrics)
    {
        if (!_disposed && window == _window.Id)
        {
            _metricsChanges++;
            _minimized = metrics.IsMinimized;
            var nextLifecycle = CurrentLifecycle();
            var lifecycleChanged = _lifecycle != nextLifecycle;
            _lifecycle = nextLifecycle;
            MetricsChanged?.Invoke(Convert(metrics, nextLifecycle));
            if (lifecycleChanged)
            {
                LifecycleChanged?.Invoke(nextLifecycle);
            }
        }
    }

    public void OnCloseRequested(WindowId window)
    {
        if (!_disposed && window == _window.Id)
        {
            CloseRequested?.Invoke();
        }
    }

    public void OnClosed(WindowId window)
    {
        if (!_disposed && window == _window.Id)
        {
            EmitLifecycle(PlatformAppLifecycleState.detached);
            Closed?.Invoke();
        }
    }

    public void OnPointer(RawPointerEvent input)
    {
        if (_disposed || input.Window != _window.Id)
        {
            return;
        }
        _pointerPackets++;
        var scale = _window.Metrics.ScaleFactor;
        _lastPointerPositions.TryGetValue(input.DeviceId, out var previous);
        var delta = input.Position - previous;
        if (input.Phase is PointerPhase.Added or PointerPhase.Removed or PointerPhase.Cancelled)
        {
            delta = GraphicsOffset.Zero;
        }
        if (input.Phase is PointerPhase.Removed or PointerPhase.Cancelled)
        {
            _lastPointerPositions.Remove(input.DeviceId);
        }
        else
        {
            _lastPointerPositions[input.DeviceId] = input.Position;
        }
        var packet = new PointerDataPacket([
            new PlatformPointerData(
                _viewId,
                input.Timestamp,
                input.Phase switch
                {
                    PointerPhase.Added => PointerChange.add,
                    PointerPhase.Hover => PointerChange.hover,
                    PointerPhase.Down => PointerChange.down,
                    PointerPhase.Move => PointerChange.move,
                    PointerPhase.Up => PointerChange.up,
                    PointerPhase.Removed => PointerChange.remove,
                    _ => PointerChange.cancel,
                },
                input.DeviceKind switch
                {
                    Doroti.Platform.PointerDeviceKind.Mouse => Doroti.Flutter.Ui.PointerDeviceKind.mouse,
                    Doroti.Platform.PointerDeviceKind.Touch => Doroti.Flutter.Ui.PointerDeviceKind.touch,
                    Doroti.Platform.PointerDeviceKind.Pen => Doroti.Flutter.Ui.PointerDeviceKind.stylus,
                    _ => Doroti.Flutter.Ui.PointerDeviceKind.unknown,
                },
                input.DeviceId,
                input.Position.X * scale,
                input.Position.Y * scale,
                delta.X * scale,
                delta.Y * scale,
                input.Buttons,
                input.ScrollDelta.X * scale,
                input.ScrollDelta.Y * scale,
                signalKind: input.ScrollDelta == GraphicsOffset.Zero
                    ? PointerSignalKind.none
                    : PointerSignalKind.scroll),
        ]);
        // Win32 can deliver pointer messages re-entrantly from ShowWindow, UpdateWindow,
        // or frame presentation. Flutter engine events belong to the next UI event turn;
        // dispatching them inline can call setState while the framework is building.
        _postUi(() =>
        {
            if (!_disposed)
            {
                PointerData?.Invoke(packet);
            }
        });
    }

    public void OnKey(RawKeyEvent input)
    {
        if (!_disposed && input.Window == _window.Id)
        {
            _keyPackets++;
            KeyData?.Invoke(new(
                _viewId,
                input.Timestamp,
                input.Phase switch
                {
                    KeyPhase.Down => KeyEventType.down,
                    KeyPhase.Up => KeyEventType.up,
                    _ => KeyEventType.repeat,
                },
                input.PhysicalKey,
                input.LogicalKey,
                synthesized: false));
        }
    }

    public void OnFocus(RawFocusEvent input)
    {
        if (!_disposed && input.Window == _window.Id)
        {
            _focusChanges++;
            _focused = input.IsFocused;
            var focus = new RawFocusData(_viewId, input.IsFocused, input.Timestamp);
            var lifecycle = CurrentLifecycle();
            _postUi(() =>
            {
                if (!_disposed)
                {
                    FocusData?.Invoke(focus);
                    EmitLifecycle(lifecycle);
                }
            });
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _window.RawInput.Detach(this);
        _window.Dispose();
    }

    private PlatformAppLifecycleState CurrentLifecycle() => _minimized
        ? PlatformAppLifecycleState.hidden
        : _focused ? PlatformAppLifecycleState.resumed : PlatformAppLifecycleState.inactive;

    private void EmitLifecycle(PlatformAppLifecycleState state)
    {
        if (_lifecycle == state)
        {
            return;
        }
        _lifecycle = state;
        LifecycleChanged?.Invoke(state);
    }

    private static ViewMetrics Convert(WindowMetrics metrics, PlatformAppLifecycleState lifecycle) => new(
        new(metrics.PixelSize.Width, metrics.PixelSize.Height),
        metrics.ScaleFactor,
        ViewPadding.zero,
        ViewPadding.zero,
        ViewPadding.zero,
        lifecycle,
        metrics.Generation,
        metrics.SurfaceGeneration);

    private static int CountNodes(SemanticsNodeSnapshot node) =>
        1 + node.Children.Sum(CountNodes);
}

internal sealed class DesktopFrameCapability(IFrameDispatcher dispatcher) : IFrameHostCapability
{
    public void ScheduleFrame(Action<TimeSpan> callback) => dispatcher.ScheduleFrame(callback);
}

internal sealed class DesktopPlatformServicesCapability(
    IClipboard clipboard,
    ICursorController cursor,
    WindowId window) : IPlatformServicesHostCapability
{
    private long _cursorRequests;
    private FlutterMouseCursorKind? _lastCursor;

    internal DesktopFlutterCursorDiagnostics Diagnostics => new(
        Interlocked.Read(ref _cursorRequests),
        _lastCursor);
    public async ValueTask<string?> GetClipboardTextAsync(CancellationToken cancellationToken = default)
    {
        var result = await clipboard.GetTextAsync(cancellationToken).ConfigureAwait(false);
        return result.Success ? result.Text : throw new InvalidOperationException(result.Diagnostic ?? "Clipboard read failed.");
    }

    public async ValueTask SetClipboardTextAsync(string text, CancellationToken cancellationToken = default)
    {
        var result = await clipboard.SetTextAsync(text, cancellationToken).ConfigureAwait(false);
        if (!result.Success)
        {
            throw new InvalidOperationException(result.Diagnostic ?? "Clipboard write failed.");
        }
    }

    public void SetCursor(FlutterMouseCursorKind value)
    {
        cursor.SetCursor(window, value switch
        {
        FlutterMouseCursorKind.basic => CursorKind.Basic,
        FlutterMouseCursorKind.click => CursorKind.Click,
        FlutterMouseCursorKind.forbidden => CursorKind.Forbidden,
        FlutterMouseCursorKind.wait => CursorKind.Wait,
        FlutterMouseCursorKind.progress => CursorKind.Progress,
        FlutterMouseCursorKind.contextMenu => CursorKind.ContextMenu,
        FlutterMouseCursorKind.help => CursorKind.Help,
        FlutterMouseCursorKind.text => CursorKind.Text,
        FlutterMouseCursorKind.verticalText => CursorKind.VerticalText,
        FlutterMouseCursorKind.cell => CursorKind.Cell,
        FlutterMouseCursorKind.precise => CursorKind.Precise,
        FlutterMouseCursorKind.move => CursorKind.Move,
        FlutterMouseCursorKind.grab => CursorKind.Grab,
        FlutterMouseCursorKind.grabbing => CursorKind.Grabbing,
        FlutterMouseCursorKind.noDrop => CursorKind.NoDrop,
        FlutterMouseCursorKind.alias => CursorKind.Alias,
        FlutterMouseCursorKind.copy => CursorKind.Copy,
        FlutterMouseCursorKind.disappearing => CursorKind.Disappearing,
        FlutterMouseCursorKind.allScroll => CursorKind.AllScroll,
        FlutterMouseCursorKind.resizeLeftRight => CursorKind.ResizeLeftRight,
        FlutterMouseCursorKind.resizeUpDown => CursorKind.ResizeUpDown,
        FlutterMouseCursorKind.resizeUpLeftDownRight => CursorKind.ResizeUpLeftDownRight,
        FlutterMouseCursorKind.resizeUpRightDownLeft => CursorKind.ResizeUpRightDownLeft,
        FlutterMouseCursorKind.resizeUp => CursorKind.ResizeUp,
        FlutterMouseCursorKind.resizeDown => CursorKind.ResizeDown,
        FlutterMouseCursorKind.resizeLeft => CursorKind.ResizeLeft,
        FlutterMouseCursorKind.resizeRight => CursorKind.ResizeRight,
        FlutterMouseCursorKind.resizeUpLeft => CursorKind.ResizeUpLeft,
        FlutterMouseCursorKind.resizeUpRight => CursorKind.ResizeUpRight,
        FlutterMouseCursorKind.resizeDownLeft => CursorKind.ResizeDownLeft,
        FlutterMouseCursorKind.resizeDownRight => CursorKind.ResizeDownRight,
        FlutterMouseCursorKind.resizeColumn => CursorKind.ResizeColumn,
        FlutterMouseCursorKind.resizeRow => CursorKind.ResizeRow,
        FlutterMouseCursorKind.zoomIn => CursorKind.ZoomIn,
        FlutterMouseCursorKind.zoomOut => CursorKind.ZoomOut,
        FlutterMouseCursorKind.none => CursorKind.Hidden,
            _ => throw new ArgumentOutOfRangeException(nameof(value)),
        });
        _lastCursor = value;
        Interlocked.Increment(ref _cursorRequests);
    }
}

internal sealed class DesktopTextInputCapability : ITextInputHostCapability, ITextInputClient, IDisposable
{
    private readonly ITextInputConnection _connection;
    private readonly ITextInputGeometry _geometry;
    private bool _disposed;

    internal DesktopTextInputCapability(ITextInputConnection connection, ITextInputGeometry geometry)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _geometry = geometry ?? throw new ArgumentNullException(nameof(geometry));
    }

    public event Action<FlutterTextEditingState>? EditingStateChanged;

    public event Action<FlutterTextInputAction>? ActionPerformed;

    public void SetClient(FlutterTextEditingState initialState)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _connection.SetClient(this, ToPlatform(initialState));
    }

    public void UpdateState(FlutterTextEditingState state)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _connection.UpdateState(ToPlatform(state));
    }

    public void SetCaretRect(Doroti.Flutter.Ui.Rect logicalRect)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _geometry.SetCaretRect(new(logicalRect.left, logicalRect.top, logicalRect.right, logicalRect.bottom));
    }

    public void ClearClient()
    {
        if (!_disposed)
        {
            _connection.ClearClient();
        }
    }

    public void UpdateEditingState(TextEditingState state) => EditingStateChanged?.Invoke(new(
        state.Text,
        new(state.Selection.BaseOffset, state.Selection.ExtentOffset),
        state.ComposingRange is { } composing ? new FlutterTextSelection(composing.BaseOffset, composing.ExtentOffset) : null));

    public void PerformAction(Doroti.Platform.TextInputAction action) => ActionPerformed?.Invoke(action switch
    {
        Doroti.Platform.TextInputAction.Done => FlutterTextInputAction.done,
        Doroti.Platform.TextInputAction.Next => FlutterTextInputAction.next,
        Doroti.Platform.TextInputAction.Previous => FlutterTextInputAction.previous,
        Doroti.Platform.TextInputAction.NewLine => FlutterTextInputAction.newline,
        _ => FlutterTextInputAction.none,
    });

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _connection.ClearClient();
        _disposed = true;
    }

    private static TextEditingState ToPlatform(FlutterTextEditingState state) => new(
        state.text,
        new(state.selection.baseOffset, state.selection.extentOffset),
        state.composingRange is { } composing ? new TextSelection(composing.baseOffset, composing.extentOffset) : null);
}

internal sealed class DesktopPlatformMessageCapability : IPlatformMessageHostCapability
{
    private const string MouseCursorChannel = "flutter/mousecursor";
    private readonly object _gate = new();
    private readonly Dictionary<string, PlatformMessageHandler> _handlers = new(StringComparer.Ordinal);

    internal DesktopPlatformMessageCapability(IPlatformServicesHostCapability? platformServices)
    {
        if (platformServices is not null)
        {
            _handlers.Add(MouseCursorChannel, (data, cancellationToken) =>
                HandleMouseCursorAsync(platformServices, data, cancellationToken));
        }
    }

    public ValueTask<ReadOnlyMemory<byte>?> SendAsync(
        string channel,
        ReadOnlyMemory<byte>? data,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);
        PlatformMessageHandler? handler;
        lock (_gate)
        {
            _handlers.TryGetValue(channel, out handler);
        }
        return handler is null
            ? ValueTask.FromResult<ReadOnlyMemory<byte>?>(null)
            : handler(data, cancellationToken);
    }

    public void SetMessageHandler(string channel, PlatformMessageHandler? handler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);
        lock (_gate)
        {
            if (handler is null)
            {
                _handlers.Remove(channel);
            }
            else
            {
                _handlers[channel] = handler;
            }
        }
    }

    private static ValueTask<ReadOnlyMemory<byte>?> HandleMouseCursorAsync(
        IPlatformServicesHostCapability platformServices,
        ReadOnlyMemory<byte>? data,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            var call = DesktopStandardMethodCodec.DecodeMethodCall(data);
            if (!string.Equals(call.Method, "activateSystemCursor", StringComparison.Ordinal))
            {
                return ValueTask.FromResult<ReadOnlyMemory<byte>?>(DesktopStandardMethodCodec.EncodeErrorEnvelope(
                    "unimplemented",
                    $"Unsupported flutter/mousecursor method '{call.Method}'."));
            }
            if (call.Arguments is not IReadOnlyDictionary<object, object?> arguments ||
                !arguments.TryGetValue("kind", out var kindValue) || kindValue is not string kind ||
                !Enum.TryParse<FlutterMouseCursorKind>(kind, ignoreCase: false, out var cursor))
            {
                return ValueTask.FromResult<ReadOnlyMemory<byte>?>(DesktopStandardMethodCodec.EncodeErrorEnvelope(
                    "unsupported-cursor",
                    "flutter/mousecursor requires a supported string 'kind'."));
            }
            platformServices.SetCursor(cursor);
            return ValueTask.FromResult<ReadOnlyMemory<byte>?>(DesktopStandardMethodCodec.EncodeSuccessEnvelope());
        }
        catch (Exception exception) when (exception is FormatException or ArgumentException)
        {
            return ValueTask.FromResult<ReadOnlyMemory<byte>?>(DesktopStandardMethodCodec.EncodeErrorEnvelope(
                "invalid-message",
                $"{exception.Message} payload={(data.HasValue ? Convert.ToHexString(data.Value.Span) : string.Empty)}"));
        }
    }
}

internal sealed class DesktopPlatformEnvironmentCapability : IPlatformEnvironmentHostCapability
{
    public PlatformConfiguration Configuration { get; } = new(
        [ToLocale(CultureInfo.CurrentUICulture)],
        Brightness.light,
        CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.Contains('H', StringComparison.Ordinal),
        nativeSpellCheckServiceDefined: false,
        operatingSystem: HostOperatingSystem.windows);

    public event Action<PlatformConfiguration>? ConfigurationChanged
    {
        add { }
        remove { }
    }

    private static Locale ToLocale(CultureInfo culture) => new(
        culture.TwoLetterISOLanguageName,
        culture.Name.Contains('-', StringComparison.Ordinal) ? culture.Name[(culture.Name.LastIndexOf('-') + 1)..] : null);
}
