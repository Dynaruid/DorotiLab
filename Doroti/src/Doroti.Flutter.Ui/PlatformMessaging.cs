using Doroti.Flutter.Runtime;

namespace Doroti.Flutter.Ui;

public delegate void PlatformMessageResponseCallback(ReadOnlyMemory<byte>? data);

public delegate ValueTask<ReadOnlyMemory<byte>?> PlatformMessageHandler(
    ReadOnlyMemory<byte>? data,
    CancellationToken cancellationToken);

public interface IPlatformMessageHostCapability
{
    ValueTask<ReadOnlyMemory<byte>?> SendAsync(
        string channel,
        ReadOnlyMemory<byte>? data,
        CancellationToken cancellationToken = default);

    void SetMessageHandler(string channel, PlatformMessageHandler? handler);
}

public interface IClipboardHostCapability
{
    ValueTask<string?> GetTextAsync(CancellationToken cancellationToken = default);

    ValueTask SetTextAsync(string text, CancellationToken cancellationToken = default);
}

public enum FlutterMouseCursorKind
{
    basic,
    click,
    forbidden,
    wait,
    progress,
    contextMenu,
    help,
    text,
    verticalText,
    cell,
    precise,
    move,
    grab,
    grabbing,
    noDrop,
    alias,
    copy,
    disappearing,
    allScroll,
    resizeLeftRight,
    resizeUpDown,
    resizeUpLeftDownRight,
    resizeUpRightDownLeft,
    resizeUp,
    resizeDown,
    resizeLeft,
    resizeRight,
    resizeUpLeft,
    resizeUpRight,
    resizeDownLeft,
    resizeDownRight,
    resizeColumn,
    resizeRow,
    zoomIn,
    zoomOut,
    none,
}

/// <summary>Native platform services used by Flutter Services without exposing platform types.</summary>
public interface IPlatformServicesHostCapability
{
    ValueTask<string?> GetClipboardTextAsync(CancellationToken cancellationToken = default);

    ValueTask SetClipboardTextAsync(string text, CancellationToken cancellationToken = default);

    void SetCursor(FlutterMouseCursorKind cursor);
}

public readonly record struct FlutterTextSelection(int baseOffset, int extentOffset);

public readonly record struct FlutterTextEditingState(
    string text,
    FlutterTextSelection selection,
    FlutterTextSelection? composingRange);

public enum FlutterTextInputAction
{
    none,
    done,
    next,
    previous,
    newline,
}

/// <summary>Avalonia-backed IME transport. Editing policy remains in Flutter Services.</summary>
public interface ITextInputHostCapability
{
    event Action<FlutterTextEditingState>? EditingStateChanged;

    event Action<FlutterTextInputAction>? ActionPerformed;

    void SetClient(FlutterTextEditingState initialState);

    void UpdateState(FlutterTextEditingState state);

    void SetCaretRect(Rect logicalRect);

    void ClearClient();
}

public sealed class RootIsolateToken
{
    private RootIsolateToken(Guid value) => Value = value;

    public Guid Value { get; }

    public static RootIsolateToken Create() => new(Guid.NewGuid());
    public static RootIsolateToken instance { get; } = Create();
}

/// <summary>Per-view channel handlers; buffering policy remains owned by generated Flutter Services.</summary>
public sealed class ChannelBuffers
{
    private readonly FlutterView _view;
    private readonly Dictionary<string, Func<ByteData?, Action<ByteData?>, Future>> _listeners = new(StringComparer.Ordinal);

    public ChannelBuffers(FlutterView view) => _view = view ?? throw new ArgumentNullException(nameof(view));

    public ValueTask<ReadOnlyMemory<byte>?> push(
        string channel,
        ReadOnlyMemory<byte>? data,
        DartUiInvocation invocation,
        CancellationToken cancellationToken = default) =>
        _view.SendPlatformMessageAsync(channel, data, invocation, cancellationToken);

    public Future push(string channel, ByteData? data, Action<ByteData?> callback)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);
        ArgumentNullException.ThrowIfNull(callback);
        if (!_listeners.TryGetValue(channel, out var listener))
            return Future.error(new FlutterCapabilityException(
                FlutterCapabilityIds.PlatformMessaging, _view.viewId,
                DartUiInvocation.Managed($"dart:ui#ChannelBuffers.push({channel})"),
                "no framework listener is registered for the channel"));
        return listener(data, callback);
    }

    public void setListener(string channel, Func<ByteData?, Action<ByteData?>, Future> listener)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);
        ArgumentNullException.ThrowIfNull(listener);
        _listeners[channel] = listener;
    }

    public void clearListener(string channel) => _listeners.Remove(channel);
}

public static class Dart_uiLibrary
{
    public const double kTextHeightNone = -1;

    public static class PluginUtilities
    {
        public static global::Doroti.Flutter.Ui.CallbackHandle? getCallbackHandle(Delegate callback) =>
            global::Doroti.Flutter.Ui.PluginUtilities.getCallbackHandle(callback);

        public static Delegate? getCallbackFromHandle(global::Doroti.Flutter.Ui.CallbackHandle handle) =>
            global::Doroti.Flutter.Ui.PluginUtilities.getCallbackFromHandle(handle);
    }

    public static class SemanticsHitTestBehavior
    {
        public static global::Doroti.Flutter.Ui.SemanticsHitTestBehavior defer =>
            global::Doroti.Flutter.Ui.SemanticsHitTestBehavior.defer;
        public static global::Doroti.Flutter.Ui.SemanticsHitTestBehavior opaque =>
            global::Doroti.Flutter.Ui.SemanticsHitTestBehavior.opaque;
        public static global::Doroti.Flutter.Ui.SemanticsHitTestBehavior transparent =>
            global::Doroti.Flutter.Ui.SemanticsHitTestBehavior.transparent;
    }

    public static class SemanticsInputType
    {
        public const global::Doroti.Flutter.Ui.SemanticsInputType none = global::Doroti.Flutter.Ui.SemanticsInputType.none;
        public const global::Doroti.Flutter.Ui.SemanticsInputType text = global::Doroti.Flutter.Ui.SemanticsInputType.text;
        public const global::Doroti.Flutter.Ui.SemanticsInputType url = global::Doroti.Flutter.Ui.SemanticsInputType.url;
        public const global::Doroti.Flutter.Ui.SemanticsInputType phone = global::Doroti.Flutter.Ui.SemanticsInputType.phone;
        public const global::Doroti.Flutter.Ui.SemanticsInputType search = global::Doroti.Flutter.Ui.SemanticsInputType.search;
        public const global::Doroti.Flutter.Ui.SemanticsInputType email = global::Doroti.Flutter.Ui.SemanticsInputType.email;
    }

    public static Future<global::Doroti.Flutter.Ui.Codec> instantiateImageCodecFromBuffer(
        global::Doroti.Flutter.Ui.ImmutableBuffer buffer,
        long? targetWidth = null,
        long? targetHeight = null,
        bool allowUpscaling = false) =>
        Future<global::Doroti.Flutter.Ui.Codec>.fromTask(DecodeImageCodecAsync(
            buffer,
            image => targetWidth is null && targetHeight is null
                ? null
                : new global::Doroti.Flutter.Ui.TargetImageSize(targetWidth ?? image.width, targetHeight ?? image.height),
            allowUpscaling,
            "dart:ui#instantiateImageCodecFromBuffer"));

    public static Future<global::Doroti.Flutter.Ui.Codec> instantiateImageCodecWithSize(
        global::Doroti.Flutter.Ui.ImmutableBuffer buffer,
        Func<long, long, global::Doroti.Flutter.Ui.TargetImageSize>? getTargetSize = null) =>
        Future<global::Doroti.Flutter.Ui.Codec>.fromTask(DecodeImageCodecAsync(
            buffer,
            image => getTargetSize?.Invoke(image.width, image.height),
            allowUpscaling: true,
            "dart:ui#instantiateImageCodecWithSize"));

    private static async Task<global::Doroti.Flutter.Ui.Codec> DecodeImageCodecAsync(
        global::Doroti.Flutter.Ui.ImmutableBuffer buffer,
        Func<global::Doroti.Flutter.Ui.Image, global::Doroti.Flutter.Ui.TargetImageSize?> targetSize,
        bool allowUpscaling,
        string elementId)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        var dispatcher = global::Doroti.Flutter.Ui.PlatformDispatcher.instance;
        var view = dispatcher.implicitView ?? dispatcher.views.FirstOrDefault()
            ?? throw new FlutterCapabilityException(
                FlutterCapabilityIds.GraphicsImage,
                null,
                DartUiInvocation.Managed(elementId),
                "image decoding requires an attached FlutterView");
        var image = await view.DecodeImageAsync(buffer.asMemory(), DartUiInvocation.Managed(elementId));
        var requested = targetSize(image);
        if (requested is not null &&
            (requested.width != image.width || requested.height != image.height) &&
            (!allowUpscaling || requested.width > image.width || requested.height > image.height))
        {
            image.Dispose();
            throw new FlutterCapabilityException(
                FlutterCapabilityIds.GraphicsImage,
                view.viewId,
                DartUiInvocation.Managed(elementId),
                $"the registered image capability cannot satisfy requested size {requested.width}x{requested.height}");
        }
        return new global::Doroti.Flutter.Ui.Codec([
            new global::Doroti.Flutter.Ui.FrameInfo(image, global::Doroti.Flutter.Runtime.Duration.zero),
        ]);
    }

    public static double? lerpDouble(double? a, double? b, double t)
    {
        if (a is null && b is null)
        {
            return null;
        }
        return (a ?? 0) + (((b ?? 0) - (a ?? 0)) * t);
    }

    public static double clampDouble(double value, double min, double max) => Math.Clamp(value, min, max);

    public static double? clampDouble(double? value, double min, double max) =>
        value is null ? null : Math.Clamp(value.Value, min, max);

    public static class Color
    {
        public static long getAlphaFromOpacity(double opacity) =>
            (long)Math.Round(Math.Clamp(opacity, 0.0, 1.0) * 255.0);

        public static global::Doroti.Flutter.Ui.Color? lerp(
            global::Doroti.Flutter.Ui.Color? a,
            global::Doroti.Flutter.Ui.Color? b,
            double t)
        {
            if (a is null && b is null) return null;
            var left = a ?? new global::Doroti.Flutter.Ui.Color(0U);
            var right = b ?? new global::Doroti.Flutter.Ui.Color(0U);
            return global::Doroti.Flutter.Ui.Color.fromARGB(
                LerpChannel(left.alpha, right.alpha, t),
                LerpChannel(left.red, right.red, t),
                LerpChannel(left.green, right.green, t),
                LerpChannel(left.blue, right.blue, t));
        }

        public static global::Doroti.Flutter.Ui.Color alphaBlend(
            global::Doroti.Flutter.Ui.Color foreground,
            global::Doroti.Flutter.Ui.Color background)
        {
            var foregroundAlpha = foreground.alpha / 255.0;
            var backgroundAlpha = background.alpha / 255.0;
            var outputAlpha = foregroundAlpha + backgroundAlpha * (1.0 - foregroundAlpha);
            if (outputAlpha <= 0) return global::Doroti.Flutter.Ui.Color.fromARGB(0, 0, 0, 0);
            long Blend(int foregroundChannel, int backgroundChannel) => checked((long)Math.Round(
                (foregroundChannel * foregroundAlpha + backgroundChannel * backgroundAlpha * (1.0 - foregroundAlpha)) / outputAlpha));
            return global::Doroti.Flutter.Ui.Color.fromARGB(
                checked((long)Math.Round(outputAlpha * 255.0)),
                Blend(foreground.red, background.red),
                Blend(foreground.green, background.green),
                Blend(foreground.blue, background.blue));
        }

        private static long LerpChannel(int left, int right, double t) =>
            (long)Math.Round(left + ((right - left) * t));
    }

    public static class Offset
    {
        public static global::Doroti.Flutter.Ui.Offset? lerp(global::Doroti.Flutter.Ui.Offset? a, global::Doroti.Flutter.Ui.Offset? b, double t)
        {
            if (a is null && b is null) return null;
            var left = a ?? global::Doroti.Flutter.Ui.Offset.zero;
            var right = b ?? global::Doroti.Flutter.Ui.Offset.zero;
            return new(left.dx + ((right.dx - left.dx) * t), left.dy + ((right.dy - left.dy) * t));
        }
    }

    public static class Radius
    {
        public static global::Doroti.Flutter.Ui.Radius? lerp(global::Doroti.Flutter.Ui.Radius? a, global::Doroti.Flutter.Ui.Radius? b, double t)
        {
            if (a is null && b is null) return null;
            var left = a ?? global::Doroti.Flutter.Ui.Radius.zero;
            var right = b ?? global::Doroti.Flutter.Ui.Radius.zero;
            return new(left.x + ((right.x - left.x) * t), left.y + ((right.y - left.y) * t));
        }
    }

    public static class Path
    {
        public static global::Doroti.Flutter.Ui.Path combine(
            global::Doroti.Flutter.Ui.PathOperation operation,
            global::Doroti.Flutter.Ui.Path path1,
            global::Doroti.Flutter.Ui.Path path2)
        {
            var result = new global::Doroti.Flutter.Ui.Path();
            result.addPath(path1, global::Doroti.Flutter.Ui.Offset.zero);
            result.addPath(path2, global::Doroti.Flutter.Ui.Offset.zero);
            return result;
        }
    }

    public static class FontWeight
    {
        public static global::Doroti.Flutter.Ui.FontWeight? lerp(global::Doroti.Flutter.Ui.FontWeight? a, global::Doroti.Flutter.Ui.FontWeight? b, double t)
        {
            if (a is null && b is null) return null;
            var left = a?.value ?? 0;
            var right = b?.value ?? 0;
            return new((int)Math.Round(left + ((right - left) * t)));
        }
    }

    public static class Shadow
    {
        public static List<global::Doroti.Flutter.Ui.Shadow>? lerpList(
            IReadOnlyList<global::Doroti.Flutter.Ui.Shadow>? a,
            IReadOnlyList<global::Doroti.Flutter.Ui.Shadow>? b,
            double t) => (t < 0.5 ? a : b)?.ToList();
    }

    public static class FontVariation
    {
        public static global::Doroti.Flutter.Ui.FontVariation? lerp(
            global::Doroti.Flutter.Ui.FontVariation? a,
            global::Doroti.Flutter.Ui.FontVariation? b,
            double t)
        {
            if (a is null && b is null) return null;
            var axis = a?.axis ?? b!.axis;
            var left = a?.value ?? 0;
            var right = b?.value ?? 0;
            return new(axis, left + ((right - left) * t));
        }
    }

    public static class PlaceholderAlignment
    {
        public const global::Doroti.Flutter.Ui.PlaceholderAlignment baseline = global::Doroti.Flutter.Ui.PlaceholderAlignment.baseline;
        public const global::Doroti.Flutter.Ui.PlaceholderAlignment aboveBaseline = global::Doroti.Flutter.Ui.PlaceholderAlignment.aboveBaseline;
        public const global::Doroti.Flutter.Ui.PlaceholderAlignment belowBaseline = global::Doroti.Flutter.Ui.PlaceholderAlignment.belowBaseline;
        public const global::Doroti.Flutter.Ui.PlaceholderAlignment top = global::Doroti.Flutter.Ui.PlaceholderAlignment.top;
        public const global::Doroti.Flutter.Ui.PlaceholderAlignment bottom = global::Doroti.Flutter.Ui.PlaceholderAlignment.bottom;
        public const global::Doroti.Flutter.Ui.PlaceholderAlignment middle = global::Doroti.Flutter.Ui.PlaceholderAlignment.middle;
    }

    public static class BoxHeightStyle
    {
        public const global::Doroti.Flutter.Ui.BoxHeightStyle tight = global::Doroti.Flutter.Ui.BoxHeightStyle.tight;
        public const global::Doroti.Flutter.Ui.BoxHeightStyle max = global::Doroti.Flutter.Ui.BoxHeightStyle.max;
        public const global::Doroti.Flutter.Ui.BoxHeightStyle includeLineSpacingMiddle = global::Doroti.Flutter.Ui.BoxHeightStyle.includeLineSpacingMiddle;
        public const global::Doroti.Flutter.Ui.BoxHeightStyle includeLineSpacingTop = global::Doroti.Flutter.Ui.BoxHeightStyle.includeLineSpacingTop;
        public const global::Doroti.Flutter.Ui.BoxHeightStyle includeLineSpacingBottom = global::Doroti.Flutter.Ui.BoxHeightStyle.includeLineSpacingBottom;
        public const global::Doroti.Flutter.Ui.BoxHeightStyle strut = global::Doroti.Flutter.Ui.BoxHeightStyle.strut;
    }

    public static class BoxWidthStyle
    {
        public const global::Doroti.Flutter.Ui.BoxWidthStyle tight = global::Doroti.Flutter.Ui.BoxWidthStyle.tight;
        public const global::Doroti.Flutter.Ui.BoxWidthStyle max = global::Doroti.Flutter.Ui.BoxWidthStyle.max;
    }

    public static class TileMode
    {
        public const global::Doroti.Flutter.Ui.TileMode clamp = global::Doroti.Flutter.Ui.TileMode.clamp;
        public const global::Doroti.Flutter.Ui.TileMode repeated = global::Doroti.Flutter.Ui.TileMode.repeated;
        public const global::Doroti.Flutter.Ui.TileMode mirror = global::Doroti.Flutter.Ui.TileMode.mirror;
        public const global::Doroti.Flutter.Ui.TileMode decal = global::Doroti.Flutter.Ui.TileMode.decal;
    }

    public static class Size
    {
        public static global::Doroti.Flutter.Ui.Size? lerp(
            global::Doroti.Flutter.Ui.Size? a,
            global::Doroti.Flutter.Ui.Size? b,
            double t)
        {
            if (a is null && b is null) return null;
            var left = a ?? global::Doroti.Flutter.Ui.Size.zero;
            var right = b ?? global::Doroti.Flutter.Ui.Size.zero;
            return new(left.width + ((right.width - left.width) * t), left.height + ((right.height - left.height) * t));
        }
    }

    public static class Rect
    {
        public static global::Doroti.Flutter.Ui.Rect? lerp(
            global::Doroti.Flutter.Ui.Rect? a,
            global::Doroti.Flutter.Ui.Rect? b,
            double t)
        {
            if (a is null && b is null) return null;
            var left = a ?? global::Doroti.Flutter.Ui.Rect.zero;
            var right = b ?? global::Doroti.Flutter.Ui.Rect.zero;
            return new(
                left.left + ((right.left - left.left) * t),
                left.top + ((right.top - left.top) * t),
                left.right + ((right.right - left.right) * t),
                left.bottom + ((right.bottom - left.bottom) * t));
        }
    }

    public static ChannelBuffers channelBuffers => global::Doroti.Flutter.Ui.PlatformDispatcher.instance.channelBuffers;

    public static Doroti.Flutter.Runtime.Future loadFontFromList(
        Doroti.Flutter.Runtime.Uint8List list,
        string? fontFamily = null)
    {
        ArgumentNullException.ThrowIfNull(list);
        _ = fontFamily;
        return Doroti.Flutter.Runtime.Future.value();
    }

    public static class RootIsolateToken
    {
        public static global::Doroti.Flutter.Ui.RootIsolateToken instance => global::Doroti.Flutter.Ui.RootIsolateToken.instance;
    }

    public static class KeyEventType
    {
        public const global::Doroti.Flutter.Ui.KeyEventType down = global::Doroti.Flutter.Ui.KeyEventType.down;
        public const global::Doroti.Flutter.Ui.KeyEventType up = global::Doroti.Flutter.Ui.KeyEventType.up;
        public const global::Doroti.Flutter.Ui.KeyEventType repeat = global::Doroti.Flutter.Ui.KeyEventType.repeat;
    }

    public static class PointerChange
    {
        public const global::Doroti.Flutter.Ui.PointerChange cancel = global::Doroti.Flutter.Ui.PointerChange.cancel;
        public const global::Doroti.Flutter.Ui.PointerChange add = global::Doroti.Flutter.Ui.PointerChange.add;
        public const global::Doroti.Flutter.Ui.PointerChange remove = global::Doroti.Flutter.Ui.PointerChange.remove;
        public const global::Doroti.Flutter.Ui.PointerChange hover = global::Doroti.Flutter.Ui.PointerChange.hover;
        public const global::Doroti.Flutter.Ui.PointerChange down = global::Doroti.Flutter.Ui.PointerChange.down;
        public const global::Doroti.Flutter.Ui.PointerChange move = global::Doroti.Flutter.Ui.PointerChange.move;
        public const global::Doroti.Flutter.Ui.PointerChange up = global::Doroti.Flutter.Ui.PointerChange.up;
        public const global::Doroti.Flutter.Ui.PointerChange panZoomStart = global::Doroti.Flutter.Ui.PointerChange.panZoomStart;
        public const global::Doroti.Flutter.Ui.PointerChange panZoomUpdate = global::Doroti.Flutter.Ui.PointerChange.panZoomUpdate;
        public const global::Doroti.Flutter.Ui.PointerChange panZoomEnd = global::Doroti.Flutter.Ui.PointerChange.panZoomEnd;
    }

    public static class PointerSignalKind
    {
        public const global::Doroti.Flutter.Ui.PointerSignalKind none = global::Doroti.Flutter.Ui.PointerSignalKind.none;
        public const global::Doroti.Flutter.Ui.PointerSignalKind scroll = global::Doroti.Flutter.Ui.PointerSignalKind.scroll;
        public const global::Doroti.Flutter.Ui.PointerSignalKind scrollInertiaCancel = global::Doroti.Flutter.Ui.PointerSignalKind.scrollInertiaCancel;
        public const global::Doroti.Flutter.Ui.PointerSignalKind scale = global::Doroti.Flutter.Ui.PointerSignalKind.scale;
        public const global::Doroti.Flutter.Ui.PointerSignalKind unknown = global::Doroti.Flutter.Ui.PointerSignalKind.unknown;
    }

    public static class ImmutableBuffer
    {
        public static Future<global::Doroti.Flutter.Ui.ImmutableBuffer> fromUint8List(Uint8List bytes) =>
            global::Doroti.Flutter.Ui.ImmutableBuffer.fromUint8List(bytes);
        public static Future<global::Doroti.Flutter.Ui.ImmutableBuffer> fromAsset(string key) =>
            global::Doroti.Flutter.Ui.ImmutableBuffer.fromAsset(key);
        public static Future<global::Doroti.Flutter.Ui.ImmutableBuffer> fromFilePath(string path) =>
            global::Doroti.Flutter.Ui.ImmutableBuffer.fromFilePath(path);
    }

    public static class FragmentProgram
    {
        public static Future<global::Doroti.Flutter.Ui.FragmentProgram> fromAsset(string assetKey) =>
            global::Doroti.Flutter.Ui.FragmentProgram.fromAsset(assetKey);
    }

    public static class PointMode
    {
        public const global::Doroti.Flutter.Ui.PointMode points = global::Doroti.Flutter.Ui.PointMode.points;
        public const global::Doroti.Flutter.Ui.PointMode lines = global::Doroti.Flutter.Ui.PointMode.lines;
        public const global::Doroti.Flutter.Ui.PointMode polygon = global::Doroti.Flutter.Ui.PointMode.polygon;
    }

    public static class KeyEventDeviceType
    {
        public const global::Doroti.Flutter.Ui.KeyEventDeviceType keyboard = global::Doroti.Flutter.Ui.KeyEventDeviceType.keyboard;
        public const global::Doroti.Flutter.Ui.KeyEventDeviceType directionalPad = global::Doroti.Flutter.Ui.KeyEventDeviceType.directionalPad;
        public const global::Doroti.Flutter.Ui.KeyEventDeviceType gamepad = global::Doroti.Flutter.Ui.KeyEventDeviceType.gamepad;
        public const global::Doroti.Flutter.Ui.KeyEventDeviceType joystick = global::Doroti.Flutter.Ui.KeyEventDeviceType.joystick;
        public const global::Doroti.Flutter.Ui.KeyEventDeviceType hdmi = global::Doroti.Flutter.Ui.KeyEventDeviceType.hdmi;
    }

    public static class AppExitResponse
    {
        public const global::Doroti.Flutter.Ui.AppExitResponse exit = global::Doroti.Flutter.Ui.AppExitResponse.exit;
        public const global::Doroti.Flutter.Ui.AppExitResponse cancel = global::Doroti.Flutter.Ui.AppExitResponse.cancel;
    }

    public static class PlatformDispatcher
    {
        public static global::Doroti.Flutter.Ui.PlatformDispatcher instance => global::Doroti.Flutter.Ui.PlatformDispatcher.instance;
    }

    public static HitTestResponse HitTestResponse(bool hasPlatformView) =>
        new(hasPlatformView: hasPlatformView);

    public static HitTestRequest HitTestRequest(FlutterView view, global::Doroti.Flutter.Ui.Offset offset) =>
        new(view: view, offset: offset);
}

public sealed class PlatformMessage
{
    private PlatformMessageResponseCallback? _responseCallback;

    public PlatformMessage(
        ulong viewId,
        string channel,
        ReadOnlyMemory<byte>? data,
        PlatformMessageResponseCallback? responseCallback)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);
        this.viewId = viewId;
        this.channel = channel;
        this.data = data;
        _responseCallback = responseCallback;
    }

    public ulong viewId { get; }

    public string channel { get; }

    public ReadOnlyMemory<byte>? data { get; }

    public void Complete(ReadOnlyMemory<byte>? response)
    {
        var callback = Interlocked.Exchange(ref _responseCallback, null);
        if (callback is null)
        {
            throw new InvalidOperationException("A platform message response may complete exactly once.");
        }
        callback(response);
    }
}
