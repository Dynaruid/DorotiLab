using Doroti.Graphics;

namespace Doroti.Platform;

public readonly record struct WindowId(ulong Value);

public readonly record struct DisplayId(ulong Value);

/// <summary>A connected display and its usable bounds in physical desktop pixels.</summary>
public readonly record struct DisplayInfo(DisplayId Id, Rect WorkArea);

public readonly record struct WindowConfiguration(string Title, Size InitialSize);

public readonly record struct WindowMetrics(
    Size LogicalSize,
    Size PixelSize,
    double ScaleFactor,
    bool IsMinimized,
    long Generation = 0,
    long ScaleGeneration = 0,
    long SurfaceGeneration = 0)
{
    public WindowMetrics(Size logicalSize, double scaleFactor, bool isMinimized, long generation = 0)
        : this(
            logicalSize,
            PixelExtentPolicy.ToPixelSize(logicalSize, scaleFactor),
            scaleFactor,
            isMinimized,
            generation,
            generation,
            generation)
    {
    }
}

public interface IWindowBackend
{
    IWindow CreateWindow(WindowConfiguration configuration, IWindowEventSink eventSink);
}

public interface IWindow : IDisposable
{
    WindowId Id { get; }

    WindowMetrics Metrics { get; }

    bool IsClosed { get; }

    IRawInputSource RawInput { get; }

    ITextInputConnection TextInput { get; }

    ICursorController Cursor { get; }

    void Show();

    void Resize(Size logicalSize);

    void SetMinimized(bool minimized);

    void Close();

    bool TryGetFeature<TFeature>(out TFeature? feature)
        where TFeature : class;
}

/// <summary>Read-only native identity used only by external diagnostics and automation validation.</summary>
public interface INativeWindowHandleDiagnostics
{
    nint Handle { get; }
}

public interface IWindowEventSink
{
    void OnMetricsChanged(WindowId window, WindowMetrics metrics);

    void OnCloseRequested(WindowId window);

    void OnClosed(WindowId window);
}

/// <summary>A backend-neutral BGRA8888 target used to bridge a window and a software renderer.</summary>
public interface IBgra8888FramebufferTarget
{
    WindowMetrics Metrics { get; }

    void Present(ReadOnlySpan<byte> pixels, int width, int height, int rowBytes);
}

/// <summary>A backend-neutral window target that can create a thread-affine OpenGL context.</summary>
public interface IOpenGlWindowTarget
{
    WindowMetrics Metrics { get; }

    IOpenGlWindowContext CreateContext();
}

/// <summary>Owns an OpenGL context without exposing HWND, WGL, or renderer-specific types.</summary>
public interface IOpenGlWindowContext : IDisposable
{
    string Renderer { get; }

    string Version { get; }

    bool IsHardwareAccelerated { get; }

    IDisposable MakeCurrent();

    void Present();
}

/// <summary>Optional window capability used by target-machine multi-display validation.</summary>
public interface IWindowPlacementController
{
    IReadOnlyList<DisplayInfo> Displays { get; }

    void MoveToDisplay(DisplayId display);
}

/// <summary>Optional window capability for framework-originated focus requests.</summary>
public interface IWindowFocusController
{
    void RequestFocus(bool focused);
}

/// <summary>Optional target-validation capability that posts input through the native window message queue.</summary>
public interface IWindowInputTestController
{
    void PostPointerMove(Offset logicalPosition);

    void PostPointerLeave(Offset logicalPosition);

    void PostPointerDown(Offset logicalPosition);

    void PostPointerUp(Offset logicalPosition);

    void PostPointerTap(Offset logicalPosition);

    void PostPointerDrag(Offset logicalStart, Offset logicalEnd);

    void PostPointerWheel(Offset logicalPosition, Offset wheelDelta);

    void PostPointerCaptureLoss(Offset logicalPosition);

    void PostKeyboardActivation(uint logicalKey);

    void PostTextInput(string text);
}

/// <summary>Optional A1 target diagnostic proving that window, pointer, and caret data share one metrics generation.</summary>
public readonly record struct WindowCoordinateSnapshot(
    long Generation,
    Size LogicalClientSize,
    Size PhysicalClientSize,
    Offset? LastPointerLogical,
    Offset? LastPointerPhysical,
    Rect? CaretLogical,
    Rect? CaretPhysical);

public interface IWindowCoordinateDiagnostics
{
    WindowCoordinateSnapshot Coordinates { get; }
}

/// <summary>Backend-neutral lifetime counters for native window and graphics-context ownership.</summary>
public readonly record struct NativeResourceSnapshot(
    int ActiveWindows,
    long WindowsCreated,
    long WindowsReleased,
    int ActiveOpenGlContexts,
    long OpenGlContextsCreated,
    long OpenGlContextsReleased)
{
    public bool IsBalanced =>
        ActiveWindows == 0 &&
        ActiveOpenGlContexts == 0 &&
        WindowsCreated == WindowsReleased &&
        OpenGlContextsCreated == OpenGlContextsReleased;
}

/// <summary>Optional diagnostic capability whose snapshot remains readable after resource disposal.</summary>
public interface INativeResourceDiagnostics
{
    NativeResourceSnapshot Snapshot { get; }
}
