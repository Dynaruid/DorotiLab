using Doroti.Runtime;

namespace Doroti.Ui;

public delegate void PlatformMessageResponseCallback(ReadOnlyMemory<byte>? data);

public delegate ValueTask<ReadOnlyMemory<byte>?> PlatformMessageHandler(
    ReadOnlyMemory<byte>? data,
    CancellationToken cancellationToken
);

public interface IPlatformMessageHostCapability
{
    ValueTask<ReadOnlyMemory<byte>?> SendAsync(
        string channel,
        ReadOnlyMemory<byte>? data,
        CancellationToken cancellationToken = default
    );

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

    /// <summary>Queries availability without reading content when the platform supports it.</summary>
    async ValueTask<bool> HasClipboardTextAsync(CancellationToken cancellationToken = default) =>
        !string.IsNullOrEmpty(await GetClipboardTextAsync(cancellationToken));

    void SetCursor(DorotiMouseCursorKind cursor);
}

public readonly record struct DorotiTextSelection(int baseOffset, int extentOffset);

public readonly record struct DorotiTextEditingState(
    string text,
    DorotiTextSelection selection,
    DorotiTextSelection? composingRange
);

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
    string? actionLabel = null,
    bool enableInteractiveSelection = true
);

/// <summary>
/// Host-neutral text metrics for the native editing endpoint. Visual text is
/// still rendered by the framework, but matching DOM metrics keep browser
/// selection, IME, and context-menu hit testing aligned with the editable.
/// </summary>
public readonly record struct DorotiTextInputStyle(
    string? fontFamily,
    double? fontSize,
    FontWeight? fontWeight,
    TextDirection textDirection,
    TextAlign textAlign,
    double? letterSpacing,
    double? wordSpacing,
    double? lineHeight
);

public enum DorotiFloatingCursorPhase
{
    start,
    update,
    end,
}

/// <summary>Logical pixel displacement from the native keyboard drag origin.</summary>
public readonly record struct DorotiFloatingCursorEvent(
    DorotiFloatingCursorPhase phase,
    Offset offset
);

/// <summary>Host-backed IME transport. Editing policy remains in Flutter Services.</summary>
public interface ITextInputHostCapability
{
    event Action<DorotiTextEditingState>? EditingStateChanged;

    event Action<DorotiTextInputAction>? ActionPerformed;

    event Action<DorotiFloatingCursorEvent>? FloatingCursorChanged
    {
        add { }
        remove { }
    }

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

    void UpdateConfiguration(DorotiTextInputConfiguration configuration) { }

    void UpdateState(DorotiTextEditingState state);

    void SetStyle(DorotiTextInputStyle style) { }

    /// <summary>
    /// Places the native editing endpoint over the framework editable. Browser
    /// hosts use this geometry so native text services, including the browser
    /// context menu, operate on the same text and selection as EditableText.
    /// </summary>
    void SetEditableSizeAndTransform(Size logicalSize, Matrix4 transform) { }

    void SetCaretRect(Rect logicalRect);

    /// <summary>Requests that the native text input UI become visible for the attached client.</summary>
    void ShowTextInput() { }

    /// <summary>Hides native text input UI without changing the framework focus owner.</summary>
    void HideTextInput() { }

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
    private readonly Dictionary<string, Func<ByteData?, Action<ByteData?>, Future>> _listeners =
        new(StringComparer.Ordinal);

    public ChannelBuffers(DorotiView view) =>
        _view = view ?? throw new ArgumentNullException(nameof(view));

    public ValueTask<ReadOnlyMemory<byte>?> push(
        string channel,
        ReadOnlyMemory<byte>? data,
        DorotiUiInvocation invocation,
        CancellationToken cancellationToken = default
    ) => _view.SendPlatformMessageAsync(channel, data, invocation, cancellationToken);

    public Future push(string channel, ByteData? data, Action<ByteData?> callback)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);
        ArgumentNullException.ThrowIfNull(callback);
        if (!_listeners.TryGetValue(channel, out var listener))
        {
            return Future.error(
                new DorotiCapabilityException(
                    DorotiCapabilityIds.PlatformMessaging,
                    _view.viewId,
                    DorotiUiInvocation.Managed($"Doroti.Ui#ChannelBuffers.push({channel})"),
                    "no framework listener is registered for the channel"
                )
            );
        }

        return listener(data, callback);
    }

    public void setListener(string channel, Func<ByteData?, Action<ByteData?>, Future> listener)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);
        ArgumentNullException.ThrowIfNull(listener);
        _listeners[channel] = listener;
    }

    public void clearListener(string channel) => _listeners.Remove(channel);

    internal Future? NotifyFramework(string channel, ByteData data) =>
        _listeners.TryGetValue(channel, out var listener) ? listener(data, _ => { }) : null;
}

public static class DorotiUiLibrary
{
    public const double kTextHeightNone = -1;

    public static class PluginUtilities
    {
        public static CallbackHandle? getCallbackHandle(Delegate callback) =>
            Ui.PluginUtilities.getCallbackHandle(callback);

        public static Delegate? getCallbackFromHandle(CallbackHandle handle) =>
            Ui.PluginUtilities.getCallbackFromHandle(handle);
    }

    public static class SemanticsHitTestBehavior
    {
        public static Ui.SemanticsHitTestBehavior defer => Ui.SemanticsHitTestBehavior.defer;
        public static Ui.SemanticsHitTestBehavior opaque => Ui.SemanticsHitTestBehavior.opaque;
        public static Ui.SemanticsHitTestBehavior transparent =>
            Ui.SemanticsHitTestBehavior.transparent;
    }

    public static class SemanticsInputType
    {
        public const Ui.SemanticsInputType none = Ui.SemanticsInputType.none;
        public const Ui.SemanticsInputType text = Ui.SemanticsInputType.text;
        public const Ui.SemanticsInputType url = Ui.SemanticsInputType.url;
        public const Ui.SemanticsInputType phone = Ui.SemanticsInputType.phone;
        public const Ui.SemanticsInputType search = Ui.SemanticsInputType.search;
        public const Ui.SemanticsInputType email = Ui.SemanticsInputType.email;
    }

    public static Future<Codec> instantiateImageCodecFromBuffer(
        Ui.ImmutableBuffer buffer,
        long? targetWidth = null,
        long? targetHeight = null,
        bool allowUpscaling = false
    ) =>
        Future<Codec>.fromTask(
            DecodeImageCodecAsync(
                buffer,
                (width, height) => new TargetImageSize(targetWidth, targetHeight),
                allowUpscaling,
                "Doroti.Ui#instantiateImageCodecFromBuffer"
            )
        );

    public static Future<Codec> instantiateImageCodecWithSize(
        Ui.ImmutableBuffer buffer,
        Func<long, long, TargetImageSize>? getTargetSize = null
    ) =>
        Future<Codec>.fromTask(
            DecodeImageCodecAsync(
                buffer,
                (width, height) => getTargetSize?.Invoke(width, height),
                allowUpscaling: true,
                "Doroti.Ui#instantiateImageCodecWithSize"
            )
        );

    private static async Task<Codec> DecodeImageCodecAsync(
        Ui.ImmutableBuffer buffer,
        Func<long, long, TargetImageSize?> targetSize,
        bool allowUpscaling,
        string elementId
    )
    {
        ArgumentNullException.ThrowIfNull(buffer);
        var dispatcher = Ui.PlatformDispatcher.instance;
        var view =
            dispatcher.implicitView
            ?? dispatcher.views.FirstOrDefault()
            ?? throw new DorotiCapabilityException(
                DorotiCapabilityIds.GraphicsImage,
                null,
                DorotiUiInvocation.Managed(elementId),
                "image decoding requires an attached DorotiView"
            );
        var image = await view.DecodeSizedImageAsync(
            buffer.asMemory(),
            targetSize,
            allowUpscaling,
            DorotiUiInvocation.Managed(elementId)
        );
        return new Codec([new FrameInfo(image, Duration.zero)]);
    }

    public static double? lerpDouble(double? a, double? b, double t)
    {
        if (a is null && b is null)
        {
            return null;
        }
        return (a ?? 0) + (((b ?? 0) - (a ?? 0)) * t);
    }

    public static double clampDouble(double value, double min, double max) =>
        Math.Clamp(value, min, max);

    public static double? clampDouble(double? value, double min, double max) =>
        value is null ? null : Math.Clamp(value.Value, min, max);

    public static class Color
    {
        public static long getAlphaFromOpacity(double opacity) =>
            (long)Math.Round(Math.Clamp(opacity, 0.0, 1.0) * 255.0);

        public static Ui.Color? lerp(Ui.Color? a, Ui.Color? b, double t)
        {
            if (a is null && b is null)
            {
                return null;
            }

            var left = a ?? new Ui.Color(0U);
            var right = b ?? new Ui.Color(0U);
            return Ui.Color.fromARGB(
                LerpChannel(left.alpha, right.alpha, t),
                LerpChannel(left.red, right.red, t),
                LerpChannel(left.green, right.green, t),
                LerpChannel(left.blue, right.blue, t)
            );
        }

        public static Ui.Color alphaBlend(Ui.Color foreground, Ui.Color background)
        {
            var foregroundAlpha = foreground.alpha / 255.0;
            var backgroundAlpha = background.alpha / 255.0;
            var outputAlpha = foregroundAlpha + (backgroundAlpha * (1.0 - foregroundAlpha));
            if (outputAlpha <= 0)
            {
                return Ui.Color.fromARGB(0, 0, 0, 0);
            }

            long Blend(int foregroundChannel, int backgroundChannel) =>
                checked(
                    (long)
                        Math.Round(
                            (
                                (foregroundChannel * foregroundAlpha)
                                + (backgroundChannel * backgroundAlpha * (1.0 - foregroundAlpha))
                            ) / outputAlpha
                        )
                );
            return Ui.Color.fromARGB(
                checked((long)Math.Round(outputAlpha * 255.0)),
                Blend(foreground.red, background.red),
                Blend(foreground.green, background.green),
                Blend(foreground.blue, background.blue)
            );
        }

        private static long LerpChannel(int left, int right, double t) =>
            (long)Math.Round(left + ((right - left) * t));
    }

    public static class Offset
    {
        public static Ui.Offset? lerp(Ui.Offset? a, Ui.Offset? b, double t)
        {
            if (a is null && b is null)
            {
                return null;
            }

            var left = a ?? Ui.Offset.zero;
            var right = b ?? Ui.Offset.zero;
            return new(left.dx + ((right.dx - left.dx) * t), left.dy + ((right.dy - left.dy) * t));
        }
    }

    public static class Radius
    {
        public static Ui.Radius? lerp(Ui.Radius? a, Ui.Radius? b, double t)
        {
            if (a is null && b is null)
            {
                return null;
            }

            var left = a ?? Ui.Radius.zero;
            var right = b ?? Ui.Radius.zero;
            return new(left.x + ((right.x - left.x) * t), left.y + ((right.y - left.y) * t));
        }
    }

    public static class Path
    {
        public static Ui.Path combine(PathOperation operation, Ui.Path path1, Ui.Path path2)
        {
            var result = new Ui.Path();
            result.addPath(path1, Ui.Offset.zero);
            result.addPath(path2, Ui.Offset.zero);
            return result;
        }
    }

    public static class FontWeight
    {
        public static Ui.FontWeight? lerp(Ui.FontWeight? a, Ui.FontWeight? b, double t)
        {
            if (a is null && b is null)
            {
                return null;
            }

            var left = a?.value ?? 0;
            var right = b?.value ?? 0;
            return new((int)Math.Round(left + ((right - left) * t)));
        }
    }

    public static class Shadow
    {
        public static List<Ui.Shadow>? lerpList(
            IReadOnlyList<Ui.Shadow>? a,
            IReadOnlyList<Ui.Shadow>? b,
            double t
        ) => (t < 0.5 ? a : b)?.ToList();
    }

    public static class FontVariation
    {
        public static Ui.FontVariation? lerp(Ui.FontVariation? a, Ui.FontVariation? b, double t)
        {
            if (a is null && b is null)
            {
                return null;
            }

            var axis = a?.axis ?? b!.axis;
            var left = a?.value ?? 0;
            var right = b?.value ?? 0;
            return new(axis, left + ((right - left) * t));
        }
    }

    public static class PlaceholderAlignment
    {
        public const Ui.PlaceholderAlignment baseline = Ui.PlaceholderAlignment.baseline;
        public const Ui.PlaceholderAlignment aboveBaseline = Ui.PlaceholderAlignment.aboveBaseline;
        public const Ui.PlaceholderAlignment belowBaseline = Ui.PlaceholderAlignment.belowBaseline;
        public const Ui.PlaceholderAlignment top = Ui.PlaceholderAlignment.top;
        public const Ui.PlaceholderAlignment bottom = Ui.PlaceholderAlignment.bottom;
        public const Ui.PlaceholderAlignment middle = Ui.PlaceholderAlignment.middle;
    }

    public static class BoxHeightStyle
    {
        public const Ui.BoxHeightStyle tight = Ui.BoxHeightStyle.tight;
        public const Ui.BoxHeightStyle max = Ui.BoxHeightStyle.max;
        public const Ui.BoxHeightStyle includeLineSpacingMiddle =
            Ui.BoxHeightStyle.includeLineSpacingMiddle;
        public const Ui.BoxHeightStyle includeLineSpacingTop =
            Ui.BoxHeightStyle.includeLineSpacingTop;
        public const Ui.BoxHeightStyle includeLineSpacingBottom =
            Ui.BoxHeightStyle.includeLineSpacingBottom;
        public const Ui.BoxHeightStyle strut = Ui.BoxHeightStyle.strut;
    }

    public static class BoxWidthStyle
    {
        public const Ui.BoxWidthStyle tight = Ui.BoxWidthStyle.tight;
        public const Ui.BoxWidthStyle max = Ui.BoxWidthStyle.max;
    }

    public static class TileMode
    {
        public const Ui.TileMode clamp = Ui.TileMode.clamp;
        public const Ui.TileMode repeated = Ui.TileMode.repeated;
        public const Ui.TileMode mirror = Ui.TileMode.mirror;
        public const Ui.TileMode decal = Ui.TileMode.decal;
    }

    public static class Size
    {
        public static Ui.Size? lerp(Ui.Size? a, Ui.Size? b, double t)
        {
            if (a is null && b is null)
            {
                return null;
            }

            var left = a ?? Ui.Size.zero;
            var right = b ?? Ui.Size.zero;
            return new(
                left.width + ((right.width - left.width) * t),
                left.height + ((right.height - left.height) * t)
            );
        }
    }

    public static class Rect
    {
        public static Ui.Rect? lerp(Ui.Rect? a, Ui.Rect? b, double t)
        {
            if (a is null && b is null)
            {
                return null;
            }

            var left = a ?? Ui.Rect.zero;
            var right = b ?? Ui.Rect.zero;
            return new(
                left.left + ((right.left - left.left) * t),
                left.top + ((right.top - left.top) * t),
                left.right + ((right.right - left.right) * t),
                left.bottom + ((right.bottom - left.bottom) * t)
            );
        }
    }

    public static ChannelBuffers channelBuffers => Ui.PlatformDispatcher.instance.channelBuffers;

    public static async Future loadFontFromList(Uint8List list, string? fontFamily = null)
    {
        ArgumentNullException.ThrowIfNull(list);
        var dispatcher = PlatformDispatcher.instance;
        var view =
            dispatcher.implicitView
            ?? dispatcher.views.FirstOrDefault()
            ?? throw new DorotiCapabilityException(
                DorotiCapabilityIds.GraphicsFont,
                null,
                DorotiUiInvocation.Managed("loadFontFromList"),
                "font loading requires an attached view"
            );
        await view.RequireCapability<IFontHostCapability>(
                DorotiCapabilityIds.GraphicsFont,
                DorotiUiInvocation.Managed("loadFontFromList")
            )
            .RegisterFontAsync(new ByteData(list).asMemory(), fontFamily);
        var notification = dispatcher.channelBuffers.NotifyFramework(
            "flutter/system",
            new ByteData(
                new Uint8List(System.Text.Encoding.UTF8.GetBytes("{\"type\":\"fontsChange\"}"))
            )
        );
        if (notification is not null)
        {
            await notification;
        }
    }

    public static class RootIsolateToken
    {
        public static Ui.RootIsolateToken instance => Ui.RootIsolateToken.instance;
    }

    public static class KeyEventType
    {
        public const Ui.KeyEventType down = Ui.KeyEventType.down;
        public const Ui.KeyEventType up = Ui.KeyEventType.up;
        public const Ui.KeyEventType repeat = Ui.KeyEventType.repeat;
    }

    public static class PointerChange
    {
        public const Ui.PointerChange cancel = Ui.PointerChange.cancel;
        public const Ui.PointerChange add = Ui.PointerChange.add;
        public const Ui.PointerChange remove = Ui.PointerChange.remove;
        public const Ui.PointerChange hover = Ui.PointerChange.hover;
        public const Ui.PointerChange down = Ui.PointerChange.down;
        public const Ui.PointerChange move = Ui.PointerChange.move;
        public const Ui.PointerChange up = Ui.PointerChange.up;
        public const Ui.PointerChange panZoomStart = Ui.PointerChange.panZoomStart;
        public const Ui.PointerChange panZoomUpdate = Ui.PointerChange.panZoomUpdate;
        public const Ui.PointerChange panZoomEnd = Ui.PointerChange.panZoomEnd;
    }

    public static class PointerSignalKind
    {
        public const Ui.PointerSignalKind none = Ui.PointerSignalKind.none;
        public const Ui.PointerSignalKind scroll = Ui.PointerSignalKind.scroll;
        public const Ui.PointerSignalKind scrollInertiaCancel =
            Ui.PointerSignalKind.scrollInertiaCancel;
        public const Ui.PointerSignalKind scale = Ui.PointerSignalKind.scale;
        public const Ui.PointerSignalKind unknown = Ui.PointerSignalKind.unknown;
    }

    public static class ImmutableBuffer
    {
        public static Future<Ui.ImmutableBuffer> fromUint8List(Uint8List bytes) =>
            Ui.ImmutableBuffer.fromUint8List(bytes);

        public static Future<Ui.ImmutableBuffer> fromAsset(string key) =>
            Ui.ImmutableBuffer.fromAsset(key);

        public static Future<Ui.ImmutableBuffer> fromFilePath(string path) =>
            Ui.ImmutableBuffer.fromFilePath(path);
    }

    public static class FragmentProgram
    {
        public static Future<Ui.FragmentProgram> fromAsset(string assetKey) =>
            Ui.FragmentProgram.fromAsset(assetKey);
    }

    public static class PointMode
    {
        public const Ui.PointMode points = Ui.PointMode.points;
        public const Ui.PointMode lines = Ui.PointMode.lines;
        public const Ui.PointMode polygon = Ui.PointMode.polygon;
    }

    public static class KeyEventDeviceType
    {
        public const Ui.KeyEventDeviceType keyboard = Ui.KeyEventDeviceType.keyboard;
        public const Ui.KeyEventDeviceType directionalPad = Ui.KeyEventDeviceType.directionalPad;
        public const Ui.KeyEventDeviceType gamepad = Ui.KeyEventDeviceType.gamepad;
        public const Ui.KeyEventDeviceType joystick = Ui.KeyEventDeviceType.joystick;
        public const Ui.KeyEventDeviceType hdmi = Ui.KeyEventDeviceType.hdmi;
    }

    public static class AppExitResponse
    {
        public const Ui.AppExitResponse exit = Ui.AppExitResponse.exit;
        public const Ui.AppExitResponse cancel = Ui.AppExitResponse.cancel;
    }

    public static class PlatformDispatcher
    {
        public static Ui.PlatformDispatcher instance => Ui.PlatformDispatcher.instance;
    }

    public static HitTestResponse HitTestResponse(bool hasPlatformView) =>
        new(hasPlatformView: hasPlatformView);

    public static HitTestRequest HitTestRequest(DorotiView view, Ui.Offset offset) =>
        new(view: view, offset: offset);
}

public sealed class PlatformMessage
{
    private PlatformMessageResponseCallback? _responseCallback;

    public PlatformMessage(
        ulong viewId,
        string channel,
        ReadOnlyMemory<byte>? data,
        PlatformMessageResponseCallback? responseCallback
    )
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
            throw new InvalidOperationException(
                "A platform message response may complete exactly once."
            );
        }
        callback(response);
    }
}
