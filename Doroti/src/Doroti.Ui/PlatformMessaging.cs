using Doroti.Runtime;

namespace Doroti.Ui;

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

public enum DorotiMouseCursorKind
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

    void SetCursor(DorotiMouseCursorKind cursor);
}

public readonly record struct DorotiTextSelection(int baseOffset, int extentOffset);

public readonly record struct DorotiTextEditingState(
    string text,
    DorotiTextSelection selection,
    DorotiTextSelection? composingRange);

public enum DorotiTextInputAction
{
    none,
    unspecified,
    done,
    go,
    search,
    send,
    next,
    previous,
    continueAction,
    join,
    route,
    emergencyCall,
    newline,
}

public enum DorotiTextInputType
{
    text,
    multiline,
    number,
    phone,
    datetime,
    emailAddress,
    url,
    visiblePassword,
    name,
    streetAddress,
    none,
    webSearch,
    twitter,
}

public enum DorotiTextCapitalization
{
    words,
    sentences,
    characters,
    none,
}

/// <summary>Host-neutral IME policy supplied by Flutter Services.</summary>
public readonly record struct DorotiTextInputConfiguration(
    DorotiTextInputType inputType,
    DorotiTextInputAction inputAction,
    DorotiTextCapitalization textCapitalization,
    bool readOnly,
    bool obscureText,
    bool autocorrect,
    bool enableSuggestions,
    string? actionLabel = null);

/// <summary>Host-backed IME transport. Editing policy remains in Flutter Services.</summary>
public interface ITextInputHostCapability
{
    event Action<DorotiTextEditingState>? EditingStateChanged;

    event Action<DorotiTextInputAction>? ActionPerformed;

    /// <summary>
    /// Raised when the native endpoint closes an attached text connection
    /// without a framework clear request (for example, a Web view losing
    /// focus to another application).
    /// </summary>
    event Action? ConnectionClosed
    {
        add { }
        remove { }
    }

    void SetClient(DorotiTextInputConfiguration configuration, DorotiTextEditingState initialState);

    void UpdateState(DorotiTextEditingState state);

    /// <summary>
    /// Places the native editing endpoint over the framework editable. Browser
    /// hosts use this geometry so native text services, including the browser
    /// context menu, operate on the same text and selection as EditableText.
    /// </summary>
    void SetEditableSizeAndTransform(Size logicalSize, Matrix4 transform)
    {
    }

    void SetCaretRect(Rect logicalRect);

    /// <summary>Requests that the native text input UI become visible for the attached client.</summary>
    void ShowTextInput()
    {
    }

    /// <summary>Hides native text input UI without changing the framework focus owner.</summary>
    void HideTextInput()
    {
    }

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
    private readonly DorotiView _view;
    private readonly Dictionary<string, Func<ByteData?, Action<ByteData?>, Future>> _listeners = new(StringComparer.Ordinal);

    public ChannelBuffers(DorotiView view) => _view = view ?? throw new ArgumentNullException(nameof(view));

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
            return Future.error(new DorotiCapabilityException(
                DorotiCapabilityIds.PlatformMessaging, _view.viewId,
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
        public static global::Doroti.Ui.CallbackHandle? getCallbackHandle(Delegate callback) =>
            global::Doroti.Ui.PluginUtilities.getCallbackHandle(callback);

        public static Delegate? getCallbackFromHandle(global::Doroti.Ui.CallbackHandle handle) =>
            global::Doroti.Ui.PluginUtilities.getCallbackFromHandle(handle);
    }

    public static class SemanticsHitTestBehavior
    {
        public static global::Doroti.Ui.SemanticsHitTestBehavior defer =>
            global::Doroti.Ui.SemanticsHitTestBehavior.defer;
        public static global::Doroti.Ui.SemanticsHitTestBehavior opaque =>
            global::Doroti.Ui.SemanticsHitTestBehavior.opaque;
        public static global::Doroti.Ui.SemanticsHitTestBehavior transparent =>
            global::Doroti.Ui.SemanticsHitTestBehavior.transparent;
    }

    public static class SemanticsInputType
    {
        public const global::Doroti.Ui.SemanticsInputType none = global::Doroti.Ui.SemanticsInputType.none;
        public const global::Doroti.Ui.SemanticsInputType text = global::Doroti.Ui.SemanticsInputType.text;
        public const global::Doroti.Ui.SemanticsInputType url = global::Doroti.Ui.SemanticsInputType.url;
        public const global::Doroti.Ui.SemanticsInputType phone = global::Doroti.Ui.SemanticsInputType.phone;
        public const global::Doroti.Ui.SemanticsInputType search = global::Doroti.Ui.SemanticsInputType.search;
        public const global::Doroti.Ui.SemanticsInputType email = global::Doroti.Ui.SemanticsInputType.email;
    }

    public static Future<global::Doroti.Ui.Codec> instantiateImageCodecFromBuffer(
        global::Doroti.Ui.ImmutableBuffer buffer,
        long? targetWidth = null,
        long? targetHeight = null,
        bool allowUpscaling = false) =>
        Future<global::Doroti.Ui.Codec>.fromTask(DecodeImageCodecAsync(
            buffer,
            image => targetWidth is null && targetHeight is null
                ? null
                : new global::Doroti.Ui.TargetImageSize(targetWidth ?? image.width, targetHeight ?? image.height),
            allowUpscaling,
            "dart:ui#instantiateImageCodecFromBuffer"));

    public static Future<global::Doroti.Ui.Codec> instantiateImageCodecWithSize(
        global::Doroti.Ui.ImmutableBuffer buffer,
        Func<long, long, global::Doroti.Ui.TargetImageSize>? getTargetSize = null) =>
        Future<global::Doroti.Ui.Codec>.fromTask(DecodeImageCodecAsync(
            buffer,
            image => getTargetSize?.Invoke(image.width, image.height),
            allowUpscaling: true,
            "dart:ui#instantiateImageCodecWithSize"));

    private static async Task<global::Doroti.Ui.Codec> DecodeImageCodecAsync(
        global::Doroti.Ui.ImmutableBuffer buffer,
        Func<global::Doroti.Ui.Image, global::Doroti.Ui.TargetImageSize?> targetSize,
        bool allowUpscaling,
        string elementId)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        var dispatcher = global::Doroti.Ui.PlatformDispatcher.instance;
        var view = dispatcher.implicitView ?? dispatcher.views.FirstOrDefault()
            ?? throw new DorotiCapabilityException(
                DorotiCapabilityIds.GraphicsImage,
                null,
                DartUiInvocation.Managed(elementId),
                "image decoding requires an attached DorotiView");
        var image = await view.DecodeImageAsync(buffer.asMemory(), DartUiInvocation.Managed(elementId));
        var requested = targetSize(image);
        if (requested is not null &&
            (requested.width != image.width || requested.height != image.height) &&
            (!allowUpscaling || requested.width > image.width || requested.height > image.height))
        {
            image.Dispose();
            throw new DorotiCapabilityException(
                DorotiCapabilityIds.GraphicsImage,
                view.viewId,
                DartUiInvocation.Managed(elementId),
                $"the registered image capability cannot satisfy requested size {requested.width}x{requested.height}");
        }
        return new global::Doroti.Ui.Codec([
            new global::Doroti.Ui.FrameInfo(image, global::Doroti.Runtime.Duration.zero),
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

        public static global::Doroti.Ui.Color? lerp(
            global::Doroti.Ui.Color? a,
            global::Doroti.Ui.Color? b,
            double t)
        {
            if (a is null && b is null) return null;
            var left = a ?? new global::Doroti.Ui.Color(0U);
            var right = b ?? new global::Doroti.Ui.Color(0U);
            return global::Doroti.Ui.Color.fromARGB(
                LerpChannel(left.alpha, right.alpha, t),
                LerpChannel(left.red, right.red, t),
                LerpChannel(left.green, right.green, t),
                LerpChannel(left.blue, right.blue, t));
        }

        public static global::Doroti.Ui.Color alphaBlend(
            global::Doroti.Ui.Color foreground,
            global::Doroti.Ui.Color background)
        {
            var foregroundAlpha = foreground.alpha / 255.0;
            var backgroundAlpha = background.alpha / 255.0;
            var outputAlpha = foregroundAlpha + backgroundAlpha * (1.0 - foregroundAlpha);
            if (outputAlpha <= 0) return global::Doroti.Ui.Color.fromARGB(0, 0, 0, 0);
            long Blend(int foregroundChannel, int backgroundChannel) => checked((long)Math.Round(
                (foregroundChannel * foregroundAlpha + backgroundChannel * backgroundAlpha * (1.0 - foregroundAlpha)) / outputAlpha));
            return global::Doroti.Ui.Color.fromARGB(
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
        public static global::Doroti.Ui.Offset? lerp(global::Doroti.Ui.Offset? a, global::Doroti.Ui.Offset? b, double t)
        {
            if (a is null && b is null) return null;
            var left = a ?? global::Doroti.Ui.Offset.zero;
            var right = b ?? global::Doroti.Ui.Offset.zero;
            return new(left.dx + ((right.dx - left.dx) * t), left.dy + ((right.dy - left.dy) * t));
        }
    }

    public static class Radius
    {
        public static global::Doroti.Ui.Radius? lerp(global::Doroti.Ui.Radius? a, global::Doroti.Ui.Radius? b, double t)
        {
            if (a is null && b is null) return null;
            var left = a ?? global::Doroti.Ui.Radius.zero;
            var right = b ?? global::Doroti.Ui.Radius.zero;
            return new(left.x + ((right.x - left.x) * t), left.y + ((right.y - left.y) * t));
        }
    }

    public static class Path
    {
        public static global::Doroti.Ui.Path combine(
            global::Doroti.Ui.PathOperation operation,
            global::Doroti.Ui.Path path1,
            global::Doroti.Ui.Path path2)
        {
            var result = new global::Doroti.Ui.Path();
            result.addPath(path1, global::Doroti.Ui.Offset.zero);
            result.addPath(path2, global::Doroti.Ui.Offset.zero);
            return result;
        }
    }

    public static class FontWeight
    {
        public static global::Doroti.Ui.FontWeight? lerp(global::Doroti.Ui.FontWeight? a, global::Doroti.Ui.FontWeight? b, double t)
        {
            if (a is null && b is null) return null;
            var left = a?.value ?? 0;
            var right = b?.value ?? 0;
            return new((int)Math.Round(left + ((right - left) * t)));
        }
    }

    public static class Shadow
    {
        public static List<global::Doroti.Ui.Shadow>? lerpList(
            IReadOnlyList<global::Doroti.Ui.Shadow>? a,
            IReadOnlyList<global::Doroti.Ui.Shadow>? b,
            double t) => (t < 0.5 ? a : b)?.ToList();
    }

    public static class FontVariation
    {
        public static global::Doroti.Ui.FontVariation? lerp(
            global::Doroti.Ui.FontVariation? a,
            global::Doroti.Ui.FontVariation? b,
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
        public const global::Doroti.Ui.PlaceholderAlignment baseline = global::Doroti.Ui.PlaceholderAlignment.baseline;
        public const global::Doroti.Ui.PlaceholderAlignment aboveBaseline = global::Doroti.Ui.PlaceholderAlignment.aboveBaseline;
        public const global::Doroti.Ui.PlaceholderAlignment belowBaseline = global::Doroti.Ui.PlaceholderAlignment.belowBaseline;
        public const global::Doroti.Ui.PlaceholderAlignment top = global::Doroti.Ui.PlaceholderAlignment.top;
        public const global::Doroti.Ui.PlaceholderAlignment bottom = global::Doroti.Ui.PlaceholderAlignment.bottom;
        public const global::Doroti.Ui.PlaceholderAlignment middle = global::Doroti.Ui.PlaceholderAlignment.middle;
    }

    public static class BoxHeightStyle
    {
        public const global::Doroti.Ui.BoxHeightStyle tight = global::Doroti.Ui.BoxHeightStyle.tight;
        public const global::Doroti.Ui.BoxHeightStyle max = global::Doroti.Ui.BoxHeightStyle.max;
        public const global::Doroti.Ui.BoxHeightStyle includeLineSpacingMiddle = global::Doroti.Ui.BoxHeightStyle.includeLineSpacingMiddle;
        public const global::Doroti.Ui.BoxHeightStyle includeLineSpacingTop = global::Doroti.Ui.BoxHeightStyle.includeLineSpacingTop;
        public const global::Doroti.Ui.BoxHeightStyle includeLineSpacingBottom = global::Doroti.Ui.BoxHeightStyle.includeLineSpacingBottom;
        public const global::Doroti.Ui.BoxHeightStyle strut = global::Doroti.Ui.BoxHeightStyle.strut;
    }

    public static class BoxWidthStyle
    {
        public const global::Doroti.Ui.BoxWidthStyle tight = global::Doroti.Ui.BoxWidthStyle.tight;
        public const global::Doroti.Ui.BoxWidthStyle max = global::Doroti.Ui.BoxWidthStyle.max;
    }

    public static class TileMode
    {
        public const global::Doroti.Ui.TileMode clamp = global::Doroti.Ui.TileMode.clamp;
        public const global::Doroti.Ui.TileMode repeated = global::Doroti.Ui.TileMode.repeated;
        public const global::Doroti.Ui.TileMode mirror = global::Doroti.Ui.TileMode.mirror;
        public const global::Doroti.Ui.TileMode decal = global::Doroti.Ui.TileMode.decal;
    }

    public static class Size
    {
        public static global::Doroti.Ui.Size? lerp(
            global::Doroti.Ui.Size? a,
            global::Doroti.Ui.Size? b,
            double t)
        {
            if (a is null && b is null) return null;
            var left = a ?? global::Doroti.Ui.Size.zero;
            var right = b ?? global::Doroti.Ui.Size.zero;
            return new(left.width + ((right.width - left.width) * t), left.height + ((right.height - left.height) * t));
        }
    }

    public static class Rect
    {
        public static global::Doroti.Ui.Rect? lerp(
            global::Doroti.Ui.Rect? a,
            global::Doroti.Ui.Rect? b,
            double t)
        {
            if (a is null && b is null) return null;
            var left = a ?? global::Doroti.Ui.Rect.zero;
            var right = b ?? global::Doroti.Ui.Rect.zero;
            return new(
                left.left + ((right.left - left.left) * t),
                left.top + ((right.top - left.top) * t),
                left.right + ((right.right - left.right) * t),
                left.bottom + ((right.bottom - left.bottom) * t));
        }
    }

    public static ChannelBuffers channelBuffers => global::Doroti.Ui.PlatformDispatcher.instance.channelBuffers;

    public static Doroti.Runtime.Future loadFontFromList(
        Doroti.Runtime.Uint8List list,
        string? fontFamily = null)
    {
        ArgumentNullException.ThrowIfNull(list);
        _ = fontFamily;
        return Doroti.Runtime.Future.value();
    }

    public static class RootIsolateToken
    {
        public static global::Doroti.Ui.RootIsolateToken instance => global::Doroti.Ui.RootIsolateToken.instance;
    }

    public static class KeyEventType
    {
        public const global::Doroti.Ui.KeyEventType down = global::Doroti.Ui.KeyEventType.down;
        public const global::Doroti.Ui.KeyEventType up = global::Doroti.Ui.KeyEventType.up;
        public const global::Doroti.Ui.KeyEventType repeat = global::Doroti.Ui.KeyEventType.repeat;
    }

    public static class PointerChange
    {
        public const global::Doroti.Ui.PointerChange cancel = global::Doroti.Ui.PointerChange.cancel;
        public const global::Doroti.Ui.PointerChange add = global::Doroti.Ui.PointerChange.add;
        public const global::Doroti.Ui.PointerChange remove = global::Doroti.Ui.PointerChange.remove;
        public const global::Doroti.Ui.PointerChange hover = global::Doroti.Ui.PointerChange.hover;
        public const global::Doroti.Ui.PointerChange down = global::Doroti.Ui.PointerChange.down;
        public const global::Doroti.Ui.PointerChange move = global::Doroti.Ui.PointerChange.move;
        public const global::Doroti.Ui.PointerChange up = global::Doroti.Ui.PointerChange.up;
        public const global::Doroti.Ui.PointerChange panZoomStart = global::Doroti.Ui.PointerChange.panZoomStart;
        public const global::Doroti.Ui.PointerChange panZoomUpdate = global::Doroti.Ui.PointerChange.panZoomUpdate;
        public const global::Doroti.Ui.PointerChange panZoomEnd = global::Doroti.Ui.PointerChange.panZoomEnd;
    }

    public static class PointerSignalKind
    {
        public const global::Doroti.Ui.PointerSignalKind none = global::Doroti.Ui.PointerSignalKind.none;
        public const global::Doroti.Ui.PointerSignalKind scroll = global::Doroti.Ui.PointerSignalKind.scroll;
        public const global::Doroti.Ui.PointerSignalKind scrollInertiaCancel = global::Doroti.Ui.PointerSignalKind.scrollInertiaCancel;
        public const global::Doroti.Ui.PointerSignalKind scale = global::Doroti.Ui.PointerSignalKind.scale;
        public const global::Doroti.Ui.PointerSignalKind unknown = global::Doroti.Ui.PointerSignalKind.unknown;
    }

    public static class ImmutableBuffer
    {
        public static Future<global::Doroti.Ui.ImmutableBuffer> fromUint8List(Uint8List bytes) =>
            global::Doroti.Ui.ImmutableBuffer.fromUint8List(bytes);
        public static Future<global::Doroti.Ui.ImmutableBuffer> fromAsset(string key) =>
            global::Doroti.Ui.ImmutableBuffer.fromAsset(key);
        public static Future<global::Doroti.Ui.ImmutableBuffer> fromFilePath(string path) =>
            global::Doroti.Ui.ImmutableBuffer.fromFilePath(path);
    }

    public static class FragmentProgram
    {
        public static Future<global::Doroti.Ui.FragmentProgram> fromAsset(string assetKey) =>
            global::Doroti.Ui.FragmentProgram.fromAsset(assetKey);
    }

    public static class PointMode
    {
        public const global::Doroti.Ui.PointMode points = global::Doroti.Ui.PointMode.points;
        public const global::Doroti.Ui.PointMode lines = global::Doroti.Ui.PointMode.lines;
        public const global::Doroti.Ui.PointMode polygon = global::Doroti.Ui.PointMode.polygon;
    }

    public static class KeyEventDeviceType
    {
        public const global::Doroti.Ui.KeyEventDeviceType keyboard = global::Doroti.Ui.KeyEventDeviceType.keyboard;
        public const global::Doroti.Ui.KeyEventDeviceType directionalPad = global::Doroti.Ui.KeyEventDeviceType.directionalPad;
        public const global::Doroti.Ui.KeyEventDeviceType gamepad = global::Doroti.Ui.KeyEventDeviceType.gamepad;
        public const global::Doroti.Ui.KeyEventDeviceType joystick = global::Doroti.Ui.KeyEventDeviceType.joystick;
        public const global::Doroti.Ui.KeyEventDeviceType hdmi = global::Doroti.Ui.KeyEventDeviceType.hdmi;
    }

    public static class AppExitResponse
    {
        public const global::Doroti.Ui.AppExitResponse exit = global::Doroti.Ui.AppExitResponse.exit;
        public const global::Doroti.Ui.AppExitResponse cancel = global::Doroti.Ui.AppExitResponse.cancel;
    }

    public static class PlatformDispatcher
    {
        public static global::Doroti.Ui.PlatformDispatcher instance => global::Doroti.Ui.PlatformDispatcher.instance;
    }

    public static HitTestResponse HitTestResponse(bool hasPlatformView) =>
        new(hasPlatformView: hasPlatformView);

    public static HitTestRequest HitTestRequest(DorotiView view, global::Doroti.Ui.Offset offset) =>
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
