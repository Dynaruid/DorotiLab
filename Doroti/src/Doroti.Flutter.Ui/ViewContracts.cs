namespace Doroti.Flutter.Ui;

public sealed record DisplayCornerRadii(
    double topLeft,
    double topRight,
    double bottomRight,
    double bottomLeft);

public readonly record struct Offset(double dx, double dy)
{
    public static Offset zero { get; } = new(0, 0);
    public static Offset infinite { get; } = new(double.PositiveInfinity, double.PositiveInfinity);
    public static Offset fromSize(Size value) => new(value.width, value.height);
    public static Offset? lerp(Offset? a, Offset? b, double t)
    {
        if (a is null && b is null) return null;
        var begin = a ?? zero;
        var end = b ?? zero;
        return new Offset(begin.dx + ((end.dx - begin.dx) * t), begin.dy + ((end.dy - begin.dy) * t));
    }

    public bool IsFinite => double.IsFinite(dx) && double.IsFinite(dy);
    public bool isFinite => IsFinite;
    public double distanceSquared => (dx * dx) + (dy * dy);
    public double distance => Math.Sqrt(distanceSquared);
    public double direction => Math.Atan2(dy, dx);
    public static Offset operator +(Offset left, Offset right) => new(left.dx + right.dx, left.dy + right.dy);
    public static Offset operator -(Offset left, Offset right) => new(left.dx - right.dx, left.dy - right.dy);
    public static Offset operator -(Offset value) => new(-value.dx, -value.dy);
    public static Offset operator *(Offset value, double operand) => new(value.dx * operand, value.dy * operand);
    public static Offset operator /(Offset value, double operand) => new(value.dx / operand, value.dy / operand);
    public static Rect operator &(Offset offset, Size size) => new(offset.dx, offset.dy, offset.dx + size.width, offset.dy + size.height);
    public Offset translate(double translateX, double translateY) => new(dx + translateX, dy + translateY);
    public Offset scale(double scaleX, double scaleY) => new(dx * scaleX, dy * scaleY);
}

public class Size : IEquatable<Size>
{
    public Size(double dimension) : this(dimension, dimension) { }

    public Size(double width, double height)
    {
        this.width = width;
        this.height = height;
    }

    protected Size(Size source) : this(source.width, source.height) { }

    public double width { get; }
    public double height { get; }
    public static Size zero { get; } = new(0, 0);
    public static Size infinite { get; } = new(double.PositiveInfinity, double.PositiveInfinity);
    public static Size fromOffset(Offset value) => new(value.dx, value.dy);
    public static Size square(double dimension) => new(dimension, dimension);
    public static Size CreateSquare(double dimension) => square(dimension);
    public static Size? lerp(Size? a, Size? b, double t)
    {
        if (a is null && b is null) return null;
        var begin = a ?? zero;
        var end = b ?? zero;
        return new Size(begin.width + ((end.width - begin.width) * t), begin.height + ((end.height - begin.height) * t));
    }
    public static Size fromWidth(double width) => new(width, double.PositiveInfinity);
    public static Size fromHeight(double height) => new(double.PositiveInfinity, height);

    public bool IsFinite => double.IsFinite(width) && double.IsFinite(height);

    public bool IsEmpty => width <= 0 || height <= 0;
    public bool isEmpty => IsEmpty;
    public bool isFinite => IsFinite;
    public double shortestSide => Math.Min(Math.Abs(width), Math.Abs(height));
    public double longestSide => Math.Max(Math.Abs(width), Math.Abs(height));
    public Size flipped => new(height, width);
    public Offset bottomRight(Offset origin) => new(origin.dx + width, origin.dy + height);
    public Offset bottomLeft(Offset origin) => new(origin.dx, origin.dy + height);
    public Offset topRight(Offset origin) => new(origin.dx + width, origin.dy);
    public Offset topLeft(Offset origin) => origin;
    public Offset center(Offset origin) => new(origin.dx + (width / 2), origin.dy + (height / 2));
    public Offset centerLeft(Offset origin) => new(origin.dx, origin.dy + (height / 2));
    public Offset centerRight(Offset origin) => new(origin.dx + width, origin.dy + (height / 2));
    public bool contains(Offset point) =>
        point.dx >= 0 && point.dx < width && point.dy >= 0 && point.dy < height;
    public static Size operator +(Size left, Offset right) => new(left.width + right.dx, left.height + right.dy);
    public static Size operator -(Size left, Offset right) => new(left.width - right.dx, left.height - right.dy);
    public static Offset operator -(Size left, Size right) => new(left.width - right.width, left.height - right.height);
    public static Size operator *(Size value, double operand) => new(value.width * operand, value.height * operand);
    public static Size operator /(Size value, double operand) => new(value.width / operand, value.height / operand);
    public static bool operator <(Size left, Size right) => left.width < right.width && left.height < right.height;
    public static bool operator >(Size left, Size right) => left.width > right.width && left.height > right.height;
    public static bool operator <=(Size left, Size right) => left.width <= right.width && left.height <= right.height;
    public static bool operator >=(Size left, Size right) => left.width >= right.width && left.height >= right.height;
    public bool Equals(Size? other) => other is not null && width.Equals(other.width) && height.Equals(other.height);
    public override bool Equals(object? obj) => obj is Size other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(width, height);
    public static bool operator ==(Size? left, Size? right) => Equals(left, right);
    public static bool operator !=(Size? left, Size? right) => !Equals(left, right);
}

public readonly record struct ViewConstraints(double minWidth, double maxWidth, double minHeight, double maxHeight)
{
    public static ViewConstraints tight(Size size) => new(size.width, size.width, size.height, size.height);
}

public readonly record struct Rect(double left, double top, double right, double bottom)
{
    public static Rect zero { get; } = new(0, 0, 0, 0);
    public static Rect largest { get; } = new(double.MinValue, double.MinValue, double.MaxValue, double.MaxValue);
    public static Rect fromLTWH(double left, double top, double width, double height) => new(left, top, left + width, top + height);
    public static Rect fromLTRB(double left, double top, double right, double bottom) => new(left, top, right, bottom);
    public static Rect fromCircle(Offset center, double radius) => new(center.dx - radius, center.dy - radius, center.dx + radius, center.dy + radius);
    public static Rect fromCenter(Offset center, double width, double height) => new(center.dx - width / 2, center.dy - height / 2, center.dx + width / 2, center.dy + height / 2);
    public static Rect fromPoints(Offset a, Offset b) => new(Math.Min(a.dx, b.dx), Math.Min(a.dy, b.dy), Math.Max(a.dx, b.dx), Math.Max(a.dy, b.dy));
    public double width => right - left;

    public double height => bottom - top;

    public bool IsFinite => double.IsFinite(left) && double.IsFinite(top) && double.IsFinite(right) && double.IsFinite(bottom);
    public bool isFinite => IsFinite;
    public bool hasNaN => double.IsNaN(left) || double.IsNaN(top) || double.IsNaN(right) || double.IsNaN(bottom);
    public bool isInfinite => double.IsInfinity(left) || double.IsInfinity(top) || double.IsInfinity(right) || double.IsInfinity(bottom);
    public bool isEmpty => left >= right || top >= bottom;
    public Size size => new(width, height);
    public Offset center => new((left + right) / 2, (top + bottom) / 2);
    public Offset topLeft => new(left, top);
    public Offset topRight => new(right, top);
    public Offset bottomLeft => new(left, bottom);
    public Offset bottomRight => new(right, bottom);
    public Offset centerLeft => new(left, (top + bottom) / 2);
    public Offset centerRight => new(right, (top + bottom) / 2);
    public Offset topCenter => new((left + right) / 2, top);
    public Offset bottomCenter => new((left + right) / 2, bottom);
    public double shortestSide => Math.Min(Math.Abs(width), Math.Abs(height));
    public double longestSide => Math.Max(Math.Abs(width), Math.Abs(height));
    public Rect inflate(double delta) => new(left - delta, top - delta, right + delta, bottom + delta);
    public Rect deflate(double delta) => inflate(-delta);
    public Rect shift(Offset offset) => new(left + offset.dx, top + offset.dy, right + offset.dx, bottom + offset.dy);
    public Rect translate(double translateX, double translateY) => shift(new(translateX, translateY));
    public Rect intersect(Rect other) => new(Math.Max(left, other.left), Math.Max(top, other.top), Math.Min(right, other.right), Math.Min(bottom, other.bottom));
    public Rect expandToInclude(Rect other) => new(Math.Min(left, other.left), Math.Min(top, other.top), Math.Max(right, other.right), Math.Max(bottom, other.bottom));
    public bool overlaps(Rect other) => left < other.right && other.left < right && top < other.bottom && other.top < bottom;
    public bool contains(Offset offset) => offset.dx >= left && offset.dx < right && offset.dy >= top && offset.dy < bottom;
}

public readonly record struct ViewPadding(double left, double top, double right, double bottom)
{
    public static ViewPadding zero { get; } = new(0, 0, 0, 0);
}

public enum AppLifecycleState
{
    detached,
    resumed,
    inactive,
    hidden,
    paused,
}

public enum Brightness
{
    dark,
    light,
}

/// <summary>The host operating system exposed through the platform.environment capability.</summary>
public enum HostOperatingSystem
{
    unknown,
    android,
    fuchsia,
    iOS,
    linux,
    macOS,
    windows,
}

public readonly record struct Locale(string languageCode, string? countryCode = null, string? scriptCode = null)
{
    public string toLanguageTag() => string.Join('-', new[] { languageCode, scriptCode, countryCode }.Where(value => !string.IsNullOrEmpty(value)));
}

public sealed record ViewMetrics(
    Size physicalSize,
    double devicePixelRatio,
    ViewPadding viewPadding,
    ViewPadding viewInsets,
    ViewPadding systemGestureInsets,
    AppLifecycleState lifecycleState,
    long generation,
    long surfaceGeneration)
{
    public Size logicalSize => devicePixelRatio > 0
        ? new(physicalSize.width / devicePixelRatio, physicalSize.height / devicePixelRatio)
        : Size.zero;
}

public sealed record PlatformConfiguration(
    IReadOnlyList<Locale> locales,
    Brightness platformBrightness,
    bool alwaysUse24HourFormat,
    bool nativeSpellCheckServiceDefined,
    HostOperatingSystem operatingSystem = HostOperatingSystem.unknown,
    double textScaleFactor = 1.0,
    bool supportsShowingSystemContextMenu = false,
    double? lineHeightScaleFactorOverride = null,
    double? letterSpacingOverride = null,
    double? wordSpacingOverride = null,
    double? paragraphSpacingOverride = null);

/// <summary>
/// Carries the active view's host-neutral platform configuration while framework callbacks run.
/// It deliberately carries no window, native handle, or concrete host object.
/// </summary>
public static class PlatformEnvironmentContext
{
    private static readonly AsyncLocal<PlatformConfiguration?> CurrentValue = new();

    public static PlatformConfiguration current => CurrentValue.Value ??
        throw new FlutterCapabilityException(
            FlutterCapabilityIds.PlatformEnvironment,
            null,
            DartUiInvocation.Managed("dart:ui#PlatformConfiguration.operatingSystem"),
            "no active Flutter view callback supplied platform.environment");

    public static IDisposable Enter(PlatformConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        var previous = CurrentValue.Value;
        CurrentValue.Value = configuration;
        return new Scope(previous);
    }

    private sealed class Scope(PlatformConfiguration? previous) : IDisposable
    {
        private PlatformConfiguration? _previous = previous;
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            CurrentValue.Value = _previous;
            _previous = null;
        }
    }
}

public sealed record Display(ulong id, Size size, double devicePixelRatio, double refreshRate);

public sealed record ViewConfiguration(
    Size physicalConstraints,
    double devicePixelRatio,
    ViewPadding viewPadding,
    ViewPadding viewInsets,
    ViewPadding systemGestureInsets);

public enum FramePhase
{
    vsyncStart,
    buildStart,
    buildFinish,
    rasterStart,
    rasterFinish,
    rasterFinishWallTime,
}

public sealed record FrameTiming(
    TimeSpan vsyncStart,
    TimeSpan buildStart,
    TimeSpan buildFinish,
    TimeSpan rasterStart,
    TimeSpan rasterFinish,
    long frameNumber = 0)
{
    public long timestampInMicroseconds(FramePhase phase) => checked((long)(phase switch
    {
        FramePhase.vsyncStart => vsyncStart,
        FramePhase.buildStart => buildStart,
        FramePhase.buildFinish => buildFinish,
        FramePhase.rasterStart => rasterStart,
        FramePhase.rasterFinish or FramePhase.rasterFinishWallTime => rasterFinish,
        _ => throw new ArgumentOutOfRangeException(nameof(phase)),
    }).TotalMicroseconds);

    public Doroti.Flutter.Runtime.Duration totalSpan => (Doroti.Flutter.Runtime.Duration)(rasterFinish - vsyncStart);
    public Doroti.Flutter.Runtime.Duration buildDuration => (Doroti.Flutter.Runtime.Duration)(buildFinish - buildStart);
    public Doroti.Flutter.Runtime.Duration rasterDuration => (Doroti.Flutter.Runtime.Duration)(rasterFinish - rasterStart);
    public Doroti.Flutter.Runtime.Duration vsyncOverhead => (Doroti.Flutter.Runtime.Duration)(buildStart - vsyncStart);
}

public delegate void TimingsCallback(IReadOnlyList<FrameTiming> timings);

/// <summary>dart:ui hit-test request delivered to [PlatformDispatcher.onHitTest].</summary>
public sealed class HitTestRequest
{
    public HitTestRequest(FlutterView view, Offset offset)
    {
        ArgumentNullException.ThrowIfNull(view);
        this.view = view;
        this.offset = offset;
    }

    public FlutterView view { get; }
    public Offset offset { get; }
}

/// <summary>dart:ui hit-test response from the framework callback.</summary>
public sealed class HitTestResponse
{
    public static readonly HitTestResponse empty = new(hasPlatformView: false);

    public HitTestResponse(bool hasPlatformView)
    {
        this.hasPlatformView = hasPlatformView;
    }

    public bool hasPlatformView { get; }
}

public delegate HitTestResponse HitTestCallback(HitTestRequest request);

public sealed record AccessibilityFeatures(
    bool accessibleNavigation,
    bool invertColors,
    bool disableAnimations,
    bool boldText,
    bool highContrast,
    bool onOffSwitchLabels,
    bool supportsAnnounce,
    bool reduceMotion = false);

public sealed record GestureSettings(double? physicalTouchSlop = null);

public enum PointerChange
{
    cancel,
    add,
    remove,
    hover,
    down,
    move,
    up,
    panZoomStart,
    panZoomUpdate,
    panZoomEnd,
}

public enum PointerSignalKind
{
    none,
    scroll,
    scrollInertiaCancel,
    scale,
    unknown,
}

public enum PointerDeviceKind
{
    touch,
    mouse,
    stylus,
    invertedStylus,
    trackpad,
    unknown,
}

public readonly record struct PointerData(
    ulong viewId,
    TimeSpan timeStamp,
    PointerChange change,
    PointerDeviceKind kind,
    ulong device,
    double physicalX,
    double physicalY,
    double physicalDeltaX,
    double physicalDeltaY,
    long buttons,
    double scrollDeltaX = 0,
    double scrollDeltaY = 0,
    PointerSignalKind? signalKind = null,
    ulong pointerIdentifier = 0,
    ulong embedderId = 0,
    bool obscured = false,
    bool synthesized = false,
    double pressure = 1,
    double pressureMin = 1,
    double pressureMax = 1,
    double distance = 0,
    double distanceMax = 0,
    double size = 0,
    double radiusMajor = 0,
    double radiusMinor = 0,
    double radiusMin = 0,
    double radiusMax = 0,
    double orientation = 0,
    double tilt = 0,
    long platformData = 0,
    double panX = 0,
    double panY = 0,
    double panDeltaX = 0,
    double panDeltaY = 0,
    double scale = 1,
    double rotation = 0,
    Action<bool>? respond = null);

public sealed record PointerDataPacket(IReadOnlyList<PointerData> data);

public enum KeyEventType
{
    down,
    up,
    repeat,
}

public readonly record struct KeyData(
    ulong viewId,
    TimeSpan timeStamp,
    KeyEventType type,
    long physical,
    long logical,
    bool synthesized,
    string? character = null)
{
    public KeyEventDeviceType deviceType => KeyEventDeviceType.keyboard;
}

public static class Platform
{
    public static bool isIOS => PlatformEnvironmentContext.current.operatingSystem == HostOperatingSystem.iOS;
    public static bool isAndroid => PlatformEnvironmentContext.current.operatingSystem == HostOperatingSystem.android;
    public static bool isWindows => PlatformEnvironmentContext.current.operatingSystem == HostOperatingSystem.windows;
    public static bool isLinux => PlatformEnvironmentContext.current.operatingSystem == HostOperatingSystem.linux;
    public static bool isMacOS => PlatformEnvironmentContext.current.operatingSystem == HostOperatingSystem.macOS;
    public static Doroti.Flutter.Runtime.DartMap<string, string> environment
    {
        get
        {
            var result = new Doroti.Flutter.Runtime.DartMap<string, string>();
            foreach (System.Collections.DictionaryEntry item in Environment.GetEnvironmentVariables())
                result[item.Key.ToString()!] = item.Value?.ToString() ?? string.Empty;
            return result;
        }
    }
    public static string operatingSystem => PlatformEnvironmentContext.current.operatingSystem switch
    {
        HostOperatingSystem.iOS => "ios",
        HostOperatingSystem.macOS => "macos",
        var value => value.ToString(),
    };
    public static string pathSeparator => System.IO.Path.DirectorySeparatorChar.ToString();
    public static string resolvedExecutable => Environment.ProcessPath ?? string.Empty;
}

public sealed record CallbackHandle(long rawHandle)
{
    public long toRawHandle() => rawHandle;
}

/// <summary>
/// Process-local callback handles used by generated framework code. A host may persist the raw
/// value, but it is deliberately valid only for the lifetime of this managed process.
/// </summary>
public static class PluginUtilities
{
    private static readonly object Gate = new();
    private static readonly Dictionary<Delegate, CallbackHandle> Forward = new();
    private static readonly Dictionary<CallbackHandle, Delegate> Backward = new();
    private static long _nextHandle;

    public static CallbackHandle? getCallbackHandle(Delegate callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        lock (Gate)
        {
            if (Forward.TryGetValue(callback, out var existing))
            {
                return existing;
            }

            var handle = new CallbackHandle(checked(++_nextHandle));
            Forward[callback] = handle;
            Backward[handle] = callback;
            return handle;
        }
    }

    public static Delegate? getCallbackFromHandle(CallbackHandle handle)
    {
        ArgumentNullException.ThrowIfNull(handle);
        lock (Gate)
        {
            return Backward.GetValueOrDefault(handle);
        }
    }
}

public readonly record struct RawFocusData(ulong viewId, bool isFocused, TimeSpan timeStamp);

public interface IViewHostCapability : IDisposable
{
    ViewMetrics Metrics { get; }

    event Action<ViewMetrics>? MetricsChanged;

    event Action<AppLifecycleState>? LifecycleChanged;

    event Action? CloseRequested;

    event Action? Closed;

    void Show();

    void Resize(Size logicalSize);

    void Close();
}

public interface IInputHostCapability
{
    event Action<PointerDataPacket>? PointerData;

    event Action<KeyData>? KeyData;

    event Action<RawFocusData>? FocusData;
}

public interface IViewFocusRequestCapability
{
    void RequestFocus(ViewFocusState state, ViewFocusDirection direction);
}

public interface IFrameHostCapability
{
    void ScheduleFrame(Action<TimeSpan> callback);
}

public interface IPlatformEnvironmentHostCapability
{
    PlatformConfiguration Configuration { get; }

    event Action<PlatformConfiguration>? ConfigurationChanged;
}

public sealed record FlutterViewConfiguration(string title, Size logicalSize);

/// <summary>Legacy window facade bound to one explicit view rather than a process-global current window.</summary>
public sealed class SingletonFlutterWindow(FlutterView view)
{
    private readonly FlutterView _view = view ?? throw new ArgumentNullException(nameof(view));

    public Size physicalSize => _view.metrics.physicalSize;

    public double devicePixelRatio => _view.metrics.devicePixelRatio;

    public ViewPadding viewPadding => _view.metrics.viewPadding;

    public ViewPadding viewInsets => _view.metrics.viewInsets;
}
