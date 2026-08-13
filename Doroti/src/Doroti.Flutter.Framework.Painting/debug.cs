// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/debug.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Painting;

public static partial class DebugLibrary
{
    public static bool debugDisableShadows = false;
}

public delegate global::Doroti.Flutter.Runtime.HttpClient HttpClientProvider();

public static partial class DebugLibrary
{
    public static Func<global::Doroti.Flutter.Runtime.HttpClient>? debugNetworkImageHttpClientProvider;
}

public delegate void PaintImageCallback(ImageSizeInfo info);

public class ImageSizeInfo
{
    public virtual string? source { get; private set; }
    public virtual Size displaySize { get; private set; } = default!;
    public virtual Size imageSize { get; private set; } = default!;

    public ImageSizeInfo(string? source = null, Size displaySize = default!, Size imageSize = default!)
    {
        this.source = source;
        this.displaySize = displaySize;
        this.imageSize = imageSize;
    }

    public virtual long displaySizeInBytes => _sizeToBytes(this.displaySize);
    public virtual long decodedSizeInBytes => _sizeToBytes(this.imageSize);
    internal virtual long _sizeToBytes(Size size)
    {
        return ((((size.width * size.height) * 4L) * ((4L / 3L)))).toInt();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DartMap<string, object?> toJson()
    {
        return new DartMap<string, object?> { ["source"] = this.source, ["displaySize"] = new DartMap<string, object?> { ["width"] = this.displaySize.width, ["height"] = this.displaySize.height }, ["imageSize"] = new DartMap<string, object?> { ["width"] = this.imageSize.width, ["height"] = this.imageSize.height }, ["displaySizeInBytes"] = this.displaySizeInBytes, ["decodedSizeInBytes"] = this.decodedSizeInBytes };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as ImageSizeInfo;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((__other is ImageSizeInfo) && (((ImageSizeInfo)((ImageSizeInfo)__other)).source == this.source)) && (object.Equals(((ImageSizeInfo)((ImageSizeInfo)__other)).imageSize, this.imageSize))) && (object.Equals(((ImageSizeInfo)((ImageSizeInfo)__other)).displaySize, this.displaySize)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.source, this.displaySize, this.imageSize);
    public override string ToString() => $"ImageSizeInfo({this.source}, imageSize: {this.imageSize}, displaySize: {this.displaySize})";
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
    internal static long _imageOverheadAllowanceDefault = (128L * 1024L);
}

public static partial class DebugLibrary
{
    public static long debugImageOverheadAllowance = DebugLibrary._imageOverheadAllowanceDefault;
}

public static partial class DebugLibrary
{
    public static bool debugAssertAllPaintingVarsUnset(string reason, bool debugDisableShadowsOverride = false)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((((((DebugLibrary.debugDisableShadows != debugDisableShadowsOverride) || (DebugLibrary.debugNetworkImageHttpClientProvider is not null)) || (DebugLibrary.debugOnPaintImage is not null)) || DebugLibrary.debugInvertOversizedImages) || (DebugLibrary.debugImageOverheadAllowance != DebugLibrary._imageOverheadAllowanceDefault)))
                {
                    throw new FlutterError(reason);
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public delegate bool ShaderWarmUpPictureCallback(Picture picture);

public delegate bool ShaderWarmUpImageCallback(Image image);

public static partial class DebugLibrary
{
    public static Func<Picture, bool> debugCaptureShaderWarmUpPicture = DebugLibrary._defaultPictureCapture;
}

public static partial class DebugLibrary
{
    internal static bool _defaultPictureCapture(Picture picture) => true;
}

public static partial class DebugLibrary
{
    public static Func<Image, bool> debugCaptureShaderWarmUpImage = DebugLibrary._defaultImageCapture;
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
                if ((direction is null))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("No TextDirection found."), new ErrorDescription($"To resolve {target} properties, it must be provided with a TextDirection."), new ErrorHint($"This error usually occurs when {target} is used in a widget without " + "a Directionality ancestor."), new ErrorHint("Typically, the Directionality widget is introduced by the MaterialApp " + "or WidgetsApp widget at the top of your application widget tree. It " + "determines the ambient reading direction and is used, for example, to " + "determine how to lay out text, how to interpret \"start\" and \"end\" " + "values, and to resolve EdgeInsetsDirectional, " + "AlignmentDirectional, and other *Directional objects.") });
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

