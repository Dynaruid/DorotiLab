// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/debug.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public static partial class DebugLibrary
{
    public static bool debugDisableShadows = false;
}

public delegate Runtime.HttpClient HttpClientProvider();

public static partial class DebugLibrary
{
    public static Func<Runtime.HttpClient>? debugNetworkImageHttpClientProvider;
}

public delegate void PaintImageCallback(ImageSizeInfo info);

public class ImageSizeInfo
{
    public virtual string? source { get; private set; }
    public virtual Size displaySize { get; private set; } = default!;
    public virtual Size imageSize { get; private set; } = default!;

    public ImageSizeInfo(
        string? source = null,
        Size displaySize = default!,
        Size imageSize = default!
    )
    {
        this.source = source;
        this.displaySize = displaySize;
        this.imageSize = imageSize;
    }

    public virtual long displaySizeInBytes => _sizeToBytes(displaySize);
    public virtual long decodedSizeInBytes => _sizeToBytes(imageSize);

    internal virtual long _sizeToBytes(Size size)
    {
        return (size.width * size.height * 4L * (4L / 3L)).toInt();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual DartMap<string, object?> toJson()
    {
        return new DartMap<string, object?>
        {
            ["source"] = source,
            ["displaySize"] = new DartMap<string, object?>
            {
                ["width"] = displaySize.width,
                ["height"] = displaySize.height,
            },
            ["imageSize"] = new DartMap<string, object?>
            {
                ["width"] = imageSize.width,
                ["height"] = imageSize.height,
            },
            ["displaySizeInBytes"] = displaySizeInBytes,
            ["decodedSizeInBytes"] = decodedSizeInBytes,
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as ImageSizeInfo;
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ImageSizeInfo)
            && (__other.source == source)
            && Equals(__other.imageSize, imageSize)
            && Equals(__other.displaySize, displaySize);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(source, displaySize, imageSize);

    public override string ToString() =>
        $"ImageSizeInfo({source}, imageSize: {imageSize}, displaySize: {displaySize})";
}

public static partial class DebugLibrary
{
    public static Action<ImageSizeInfo>? debugOnPaintImage;
}

public static partial class DebugLibrary
{
    public static bool debugInvertOversizedImages = false;
}

public static partial class DebugLibrary
{
    internal static long _imageOverheadAllowanceDefault = 128L * 1024L;
}

public static partial class DebugLibrary
{
    public static long debugImageOverheadAllowance = _imageOverheadAllowanceDefault;
}

public static partial class DebugLibrary
{
    public static bool debugAssertAllPaintingVarsUnset(
        string reason,
        bool debugDisableShadowsOverride = false
    )
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (
                (debugDisableShadows != debugDisableShadowsOverride)
                || (debugNetworkImageHttpClientProvider is not null)
                || (debugOnPaintImage is not null)
                || debugInvertOversizedImages
                || (debugImageOverheadAllowance != _imageOverheadAllowanceDefault)
            )
            {
                throw new FlutterError(reason);
            }
            return true;
        });
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public delegate bool ShaderWarmUpPictureCallback(Picture picture);

public delegate bool ShaderWarmUpImageCallback(Image image);

public static partial class DebugLibrary
{
    public static Func<Picture, bool> debugCaptureShaderWarmUpPicture = _defaultPictureCapture;
}

public static partial class DebugLibrary
{
    internal static bool _defaultPictureCapture(Picture picture) => true;
}

public static partial class DebugLibrary
{
    public static Func<Image, bool> debugCaptureShaderWarmUpImage = _defaultImageCapture;
}

public static partial class DebugLibrary
{
    internal static bool _defaultImageCapture(Image image) => true;
}

public static partial class DebugLibrary
{
    public static bool debugCheckCanResolveTextDirection(TextDirection? direction, string target)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (direction is null)
            {
                throw new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary("No TextDirection found."),
                        new ErrorDescription(
                            $"To resolve {target} properties, it must be provided with a TextDirection."
                        ),
                        new ErrorHint(
                            $"This error usually occurs when {target} is used in a widget without "
                                + "a Directionality ancestor."
                        ),
                        new ErrorHint(
                            "Typically, the Directionality widget is introduced by the MaterialApp "
                                + "or WidgetsApp widget at the top of your application widget tree. It "
                                + "determines the ambient reading direction and is used, for example, to "
                                + "determine how to lay out text, how to interpret \"start\" and \"end\" "
                                + "values, and to resolve EdgeInsetsDirectional, "
                                + "AlignmentDirectional, and other *Directional objects."
                        ),
                    }
                );
            }
            return true;
        });
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
