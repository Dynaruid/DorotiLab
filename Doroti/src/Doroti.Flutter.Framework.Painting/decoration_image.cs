// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/decoration_image.dart
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

public enum ImageRepeat
{
    repeat,
    repeatX,
    repeatY,
    noRepeat
}

public class DecorationImage
{
    public virtual dynamic image { get; private set; } = default!;
    public virtual Action<object, global::System.Diagnostics.StackTrace?>? onError { get; private set; }
    public virtual ColorFilter? colorFilter { get; private set; }
    public virtual BoxFit? fit { get; private set; }
    public virtual AlignmentGeometry alignment { get; private set; } = default!;
    public virtual Rect? centerSlice { get; private set; }
    public virtual ImageRepeat repeat { get; private set; } = default!;
    public virtual bool matchTextDirection { get; private set; } = default!;
    public virtual double scale { get; private set; } = default!;
    public virtual double opacity { get; private set; } = default!;
    public virtual FilterQuality filterQuality { get; private set; } = default!;
    public virtual bool invertColors { get; private set; } = default!;
    public virtual bool isAntiAlias { get; private set; } = default!;
    public DecorationImage() { }


    public DecorationImage(dynamic image, Action<object, global::System.Diagnostics.StackTrace?>? onError = null, ColorFilter? colorFilter = null, BoxFit? fit = null, AlignmentGeometry alignment = default!, Rect? centerSlice = null, ImageRepeat repeat = ImageRepeat.noRepeat, bool matchTextDirection = false, double scale = 1.0, double opacity = 1.0, FilterQuality filterQuality = FilterQuality.medium, bool invertColors = false, bool isAntiAlias = false)
    {
        AlignmentGeometry __alignment = alignment ?? Alignment.center;
        this.image = image;
        this.onError = onError;
        this.colorFilter = colorFilter;
        this.fit = fit;
        this.alignment = __alignment;
        this.centerSlice = centerSlice;
        this.repeat = repeat;
        this.matchTextDirection = matchTextDirection;
        this.scale = scale;
        this.opacity = opacity;
        this.filterQuality = filterQuality;
        this.invertColors = invertColors;
        this.isAntiAlias = isAntiAlias;
    }

    public virtual DecorationImagePainter createPainter(Action onChanged)
    {
        return new _DecorationImagePainter__decoration_image(this, onChanged);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as DecorationImage;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((((((__other is DecorationImage) && (object.Equals(((DecorationImage)((DecorationImage)__other)).image, this.image))) && (object.Equals(((DecorationImage)((DecorationImage)__other)).colorFilter, this.colorFilter))) && (object.Equals(((DecorationImage)((DecorationImage)__other)).fit, this.fit))) && (object.Equals(((DecorationImage)((DecorationImage)__other)).alignment, this.alignment))) && (object.Equals(((DecorationImage)((DecorationImage)__other)).centerSlice, this.centerSlice))) && (object.Equals(((DecorationImage)((DecorationImage)__other)).repeat, this.repeat))) && (((DecorationImage)((DecorationImage)__other)).matchTextDirection == this.matchTextDirection)) && (((DecorationImage)((DecorationImage)__other)).scale == this.scale)) && (((DecorationImage)((DecorationImage)__other)).opacity == this.opacity)) && (object.Equals(((DecorationImage)((DecorationImage)__other)).filterQuality, this.filterQuality))) && (((DecorationImage)((DecorationImage)__other)).invertColors == this.invertColors)) && (((DecorationImage)((DecorationImage)__other)).isAntiAlias == this.isAntiAlias));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.image, this.colorFilter, this.fit, this.alignment, this.centerSlice, this.repeat, this.matchTextDirection, this.scale, this.opacity, this.filterQuality, this.invertColors, this.isAntiAlias);
    public override string ToString()
    {
        var properties__7894 = new List<string> { $"{this.image}", $"{this.alignment}", $"scale {this.scale.toStringAsFixed(1L)}", $"opacity {this.opacity.toStringAsFixed(1L)}", $"{this.filterQuality}" };
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "DecorationImage"))}({string.Join(", ", properties__7894)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DecorationImage? lerp(DecorationImage? a, DecorationImage? b, double t)
    {
        if ((DartRuntimePrimitives.Identical(a, b) || (t == 0.0)))
        {
            return a;
        }
        if ((t == 1.0))
        {
            return b;
        }
        return new _BlendedDecorationImage__decoration_image(a, b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public interface DecorationImagePainter
{
    public void paint(Canvas canvas, Rect rect, Path? clipPath, ImageConfiguration configuration, double blend = 1.0, BlendMode blendMode = BlendMode.srcOver);
    public void dispose();
}

internal class _DecorationImagePainter__decoration_image : DecorationImagePainter
{
    internal virtual DecorationImage _details { get; private set; } = default!;
    internal virtual Action _onChanged { get; private set; } = default!;
    internal virtual ImageStream? _imageStream { get; set; } = default;
    internal virtual ImageInfo? _image { get; set; } = default;

    internal _DecorationImagePainter__decoration_image(DecorationImage _details, Action _onChanged)
    {
        this._details = _details;
        this._onChanged = _onChanged;
    }

    public virtual void paint(Canvas canvas, Rect rect, Path? clipPath, ImageConfiguration configuration, double blend = 1.0, BlendMode blendMode = BlendMode.srcOver)
    {
        var flipHorizontally__12145 = false;
        if (((DecorationImage)this._details).matchTextDirection)
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((((ImageConfiguration)configuration).textDirection is null))
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("DecorationImage.matchTextDirection can only be used when a TextDirection is available."), new ErrorDescription("When DecorationImagePainter.paint() was called, there was no text direction provided " + "in the ImageConfiguration object to match."), new DiagnosticsProperty<DecorationImage>("The DecorationImage was", this._details, style: DiagnosticsTreeStyle.errorProperty), new DiagnosticsProperty<ImageConfiguration>("The ImageConfiguration was", configuration, style: DiagnosticsTreeStyle.errorProperty) });
                    }
                    return true;
                });
            if ((object.Equals(((ImageConfiguration)configuration).textDirection, TextDirection.rtl)))
            {
                flipHorizontally__12145 = true;
            }
        }
        ImageStream newImageStream__13381 = ((DecorationImage)this._details).image.resolve(configuration);
        if ((!object.Equals(((ImageStream)newImageStream__13381).key, this._imageStream?.key)))
        {
            var listener__13500 = new ImageStreamListener(this._handleImage, onError: ((DecorationImage)this._details).onError);
            this._imageStream?.removeListener(listener__13500);
            _imageStream = newImageStream__13381;
            this._imageStream!.addListener(listener__13500);
        }
        if ((this._image is null))
        {
            return;
        }
        if ((clipPath is not null))
        {
            canvas.save();
            canvas.clipPath(clipPath);
        }
        Decoration_imageLibrary.paintImage(canvas: canvas, rect: rect, image: this._image!.image, debugImageLabel: this._image!.debugLabel, scale: (((DecorationImage)this._details).scale * this._image!.scale), colorFilter: ((DecorationImage)this._details).colorFilter, fit: ((DecorationImage)this._details).fit, alignment: ((DecorationImage)this._details).alignment.resolve(((ImageConfiguration)configuration).textDirection), centerSlice: ((DecorationImage)this._details).centerSlice, repeat: ((DecorationImage)this._details).repeat, flipHorizontally: flipHorizontally__12145, opacity: (((DecorationImage)this._details).opacity * blend), filterQuality: ((DecorationImage)this._details).filterQuality, invertColors: ((DecorationImage)this._details).invertColors, isAntiAlias: ((DecorationImage)this._details).isAntiAlias, blendMode: blendMode);
        if ((clipPath is not null))
        {
            canvas.restore();
        }
    }

    internal virtual void _handleImage(ImageInfo value, bool synchronousCall)
    {
        if ((object.Equals(this._image, value)))
        {
            return;
        }
        if (((this._image is not null) && this._image!.isCloneOf(value)))
        {
            value.dispose();
            return;
        }
        this._image?.dispose();
        _image = value;
        if (!synchronousCall)
        {
            this._onChanged();
        }
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this._imageStream?.removeListener(new ImageStreamListener(this._handleImage, onError: ((DecorationImage)this._details).onError));
        this._image?.dispose();
        _image = null;
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "DecorationImagePainter"))}(stream: {this._imageStream}, image: {this._image}) for {this._details}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Decoration_imageLibrary
{
    internal static DartMap<string, ImageSizeInfo> _pendingImageSizeInfo = new DartMap<string, ImageSizeInfo>();
}

public static partial class Decoration_imageLibrary
{
    internal static HashSet<ImageSizeInfo> _lastFrameImageSizeInfo = new HashSet<ImageSizeInfo>();
}

public static partial class Decoration_imageLibrary
{
    public static void debugFlushLastFrameImageSizeInfo()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                Decoration_imageLibrary._lastFrameImageSizeInfo = new HashSet<ImageSizeInfo>();
                return true;
            });
    }
}

public static partial class Decoration_imageLibrary
{
    public static void paintImage(Canvas canvas, Rect rect, Image image, string? debugImageLabel = null, double scale = 1.0, double opacity = 1.0, ColorFilter? colorFilter = null, BoxFit? fit = null, Alignment alignment = default!, Rect? centerSlice = null, ImageRepeat repeat = ImageRepeat.noRepeat, bool flipHorizontally = false, bool invertColors = false, FilterQuality filterQuality = FilterQuality.medium, bool isAntiAlias = false, BlendMode blendMode = BlendMode.srcOver)
    {
        DartRuntimePrimitives.Assert(() => ((((long?)(global::Doroti.Flutter.Ui.Image.debugGetOpenHandleStackTraces()?.Count)) is { } __count19740 ? __count19740 != 0 : (bool?)null) ?? true));
        if (rect.isEmpty)
        {
            return;
        }
        global::Doroti.Flutter.Ui.Size outputSize__20013 = rect.size;
        var inputSize__20043 = new global::Doroti.Flutter.Ui.Size(image.width.toDouble(), image.height.toDouble());
        global::Doroti.Flutter.Ui.Offset? sliceBorder__20120 = default!;
        if ((centerSlice is not null))
        {
            Rect centerSlice__value20139 = DartRuntimePrimitives.RequireValue(centerSlice);
            sliceBorder__20120 = ((inputSize__20043 / scale) - DartRuntimePrimitives.RequireValue(centerSlice__value20139).size);
            outputSize__20013 = (outputSize__20013 - DartRuntimePrimitives.RequireValue(sliceBorder__20120));
            inputSize__20043 = (inputSize__20043 - (DartRuntimePrimitives.RequireValue(sliceBorder__20120) * scale));
        }
        fit ??= ((centerSlice is null) ? BoxFit.scaleDown : BoxFit.fill);
        DartRuntimePrimitives.Assert(() => ((centerSlice is null) || (((!object.Equals(DartRuntimePrimitives.RequireValue(fit), BoxFit.none)) && (!object.Equals(DartRuntimePrimitives.RequireValue(fit), BoxFit.cover))))));
        FittedSizes fittedSizes__20502 = global::Doroti.Generated.Framework.Painting.Box_fitLibrary.applyBoxFit(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(fit)), (inputSize__20043 / scale), outputSize__20013);
        global::Doroti.Flutter.Ui.Size sourceSize__20578 = (((FittedSizes)fittedSizes__20502).source * scale);
        global::Doroti.Flutter.Ui.Size destinationSize__20626 = ((FittedSizes)fittedSizes__20502).destination;
        if ((centerSlice is not null))
        {
            Rect centerSlice__value20675 = DartRuntimePrimitives.RequireValue(centerSlice);
            outputSize__20013 += DartRuntimePrimitives.RequireValue(sliceBorder__20120);
            destinationSize__20626 += DartRuntimePrimitives.RequireValue(sliceBorder__20120);
            DartRuntimePrimitives.Assert(() => (object.Equals(sourceSize__20578, inputSize__20043)));
        }
        if (((!object.Equals(repeat, ImageRepeat.noRepeat)) && (object.Equals(destinationSize__20626, outputSize__20013))))
        {
            repeat = ImageRepeat.noRepeat;
        }
        var paint__21275 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Ui.Paint();
    __cascade.isAntiAlias = isAntiAlias;
    return __cascade;
}))();
        if ((colorFilter is not null))
        {
            paint__21275.colorFilter = colorFilter;
        }
        paint__21275.color = global::Doroti.Flutter.Ui.Color.fromRGBO(0L, 0L, 0L, Dart_uiLibrary.clampDouble(opacity, 0.0, 1.0));
        paint__21275.filterQuality = filterQuality;
        paint__21275.invertColors = invertColors;
        paint__21275.blendMode = blendMode;
        double halfWidthDelta__21584 = (((outputSize__20013.width - destinationSize__20626.width)) / 2.0);
        double halfHeightDelta__21666 = (((outputSize__20013.height - destinationSize__20626.height)) / 2.0);
        double dx__21751 = (halfWidthDelta__21584 + (((flipHorizontally ? -((Alignment)alignment).x : ((Alignment)alignment).x)) * halfWidthDelta__21584));
        double dy__21860 = (halfHeightDelta__21666 + (((Alignment)alignment).y * halfHeightDelta__21666));
        global::Doroti.Flutter.Ui.Offset destinationPosition__21929 = rect.topLeft.translate(dx__21751, dy__21860);
        global::Doroti.Flutter.Ui.Rect destinationRect__21996 = (destinationPosition__21929 & destinationSize__20626);
        var invertedCanvas__22141 = false;
        if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            double maxDevicePixelRatio__22909 = System.Linq.Enumerable.Aggregate(PaintingBinding.instance.platformDispatcher.views, (double)0.0, ((previousValue, view) => Math.Max(previousValue, view.devicePixelRatio)));
            var sizeInfo__23116 = new ImageSizeInfo(source: (debugImageLabel ?? $"<Unknown Image({image.width}×{image.height})>"), imageSize: new global::Doroti.Flutter.Ui.Size(image.width.toDouble(), image.height.toDouble()), displaySize: (outputSize__20013 * maxDevicePixelRatio__22909));
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((global::Doroti.Generated.Framework.Painting.DebugLibrary.debugInvertOversizedImages && (((ImageSizeInfo)sizeInfo__23116).decodedSizeInBytes > (((ImageSizeInfo)sizeInfo__23116).displaySizeInBytes + global::Doroti.Generated.Framework.Painting.DebugLibrary.debugImageOverheadAllowance))))
                    {
                        long overheadInKilobytes__23602 = (checked((long)(((((ImageSizeInfo)sizeInfo__23116).decodedSizeInBytes - ((ImageSizeInfo)sizeInfo__23116).displaySizeInBytes)) / 1024L)));
                        long outputWidth__23723 = ((ImageSizeInfo)sizeInfo__23116).displaySize.width.toInt();
                        long outputHeight__23791 = ((ImageSizeInfo)sizeInfo__23116).displaySize.height.toInt();
                        FlutterError.reportError(new FlutterErrorDetails(exception: $"Image {debugImageLabel} has a display size of " + $"{outputWidth__23723}×{outputHeight__23791} but a decode size of " + $"{image.width}×{image.height}, which uses an additional " + $"{overheadInKilobytes__23602}KB (assuming a device pixel ratio of " + $"{maxDevicePixelRatio__22909}).\n\n" + "Consider resizing the asset ahead of time, supplying a cacheWidth " + $"parameter of {outputWidth__23723}, a cacheHeight parameter of " + $"{outputHeight__23791}, or using a ResizeImage.", library: "painting library", context: new ErrorDescription("while painting an image")));
                        canvas.saveLayer(destinationRect__21996, ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Ui.Paint();
    __cascade.colorFilter = global::Doroti.Flutter.Ui.ColorFilter.matrix(new List<double> { -1, 0, 0, 0, 255, 0, -1, 0, 0, 255, 0, 0, -1, 0, 255, 0, 0, 0, 1, 0 });
    return __cascade;
}))());
                        double dy__25222 = -((rect.top + (rect.height / 2.0)));
                        canvas.translate(0.0, -dy__25222);
                        canvas.scale(1.0, -1.0);
                        canvas.translate(0.0, dy__25222);
                        invertedCanvas__22141 = true;
                    }
                    return true;
                });
            if (!Decoration_imageLibrary._lastFrameImageSizeInfo.Contains(sizeInfo__23116))
            {
                ImageSizeInfo? existingSizeInfo__25597 = Decoration_imageLibrary._pendingImageSizeInfo.GetValueOrDefault(DartRuntimePrimitives.RequireReference(((ImageSizeInfo)sizeInfo__23116).source));
                if (((existingSizeInfo__25597 is null) || (((ImageSizeInfo)existingSizeInfo__25597).displaySizeInBytes < ((ImageSizeInfo)sizeInfo__23116).displaySizeInBytes)))
                {
                    Decoration_imageLibrary._pendingImageSizeInfo[((ImageSizeInfo)sizeInfo__23116).source!] = sizeInfo__23116;
                }
                global::Doroti.Generated.Framework.Painting.DebugLibrary.debugOnPaintImage?.Invoke(sizeInfo__23116);
                SchedulerBinding.instance.addPostFrameCallback(((timeStamp) =>
                {
                    Decoration_imageLibrary._lastFrameImageSizeInfo = Decoration_imageLibrary._pendingImageSizeInfo.Values.toSet();
                    if ((checked((long)(Decoration_imageLibrary._pendingImageSizeInfo.Count)) == 0))
                    {
                        return;
                    }
                    Dart_developerLibrary.postEvent("Flutter.ImageSizesForFrame", new DartMap<string, object>());
                    Decoration_imageLibrary._pendingImageSizeInfo = new DartMap<string, ImageSizeInfo>();
                }), debugLabel: "paintImage.recordImageSizes");
            }
        }
        bool needSave__26468 = (((centerSlice is not null) || (!object.Equals(repeat, ImageRepeat.noRepeat))) || flipHorizontally);
        if (needSave__26468)
        {
            canvas.save();
        }
        if ((!object.Equals(repeat, ImageRepeat.noRepeat)))
        {
            canvas.clipRect(rect);
        }
        if (flipHorizontally)
        {
            double dx__26709 = -((rect.left + (rect.width / 2.0)));
            canvas.translate(-dx__26709, 0.0);
            canvas.scale(-1.0, 1.0);
            canvas.translate(dx__26709, 0.0);
        }
        if ((centerSlice is null))
        {
            global::Doroti.Flutter.Ui.Rect sourceRect__26887 = alignment.inscribe(sourceSize__20578, (Offset.zero & inputSize__20043));
            if ((object.Equals(repeat, ImageRepeat.noRepeat)))
            {
                canvas.drawImageRect(image, sourceRect__26887, destinationRect__21996, paint__21275);
            }
            else
            {
                foreach (global::Doroti.Flutter.Ui.Rect tileRect__27105 in Decoration_imageLibrary._generateImageTileRects(rect, destinationRect__21996, repeat))
                {
                    canvas.drawImageRect(image, sourceRect__26887, tileRect__27105, paint__21275);
                }
            }
        }
        else
        {
            canvas.scale((1L / scale));
            if ((object.Equals(repeat, ImageRepeat.noRepeat)))
            {
                canvas.drawImageNine(image, Decoration_imageLibrary._scaleRect(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(centerSlice)), scale), Decoration_imageLibrary._scaleRect(destinationRect__21996, scale), paint__21275);
            }
            else
            {
                foreach (global::Doroti.Flutter.Ui.Rect tileRect__27523 in Decoration_imageLibrary._generateImageTileRects(rect, destinationRect__21996, repeat))
                {
                    canvas.drawImageNine(image, Decoration_imageLibrary._scaleRect(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(centerSlice)), scale), Decoration_imageLibrary._scaleRect(tileRect__27523, scale), paint__21275);
                }
            }
        }
        if (needSave__26468)
        {
            canvas.restore();
        }
        if (invertedCanvas__22141)
        {
            canvas.restore();
        }
    }
}

public static partial class Decoration_imageLibrary
{
    internal static IEnumerable<Rect> _generateImageTileRects(Rect outputRect, Rect fundamentalRect, ImageRepeat repeat)
    {
        var startX__27971 = 0L;
        var startY__27989 = 0L;
        var stopX__28007 = 0L;
        var stopY__28024 = 0L;
        double strideX__28050 = fundamentalRect.width;
        double strideY__28098 = fundamentalRect.height;
        if (((object.Equals(repeat, ImageRepeat.repeat)) || (object.Equals(repeat, ImageRepeat.repeatX))))
        {
            startX__27971 = ((((outputRect.left - fundamentalRect.left)) / strideX__28050)).floor();
            stopX__28007 = ((((outputRect.right - fundamentalRect.right)) / strideX__28050)).ceil();
        }
        if (((object.Equals(repeat, ImageRepeat.repeat)) || (object.Equals(repeat, ImageRepeat.repeatY))))
        {
            startY__27989 = ((((outputRect.top - fundamentalRect.top)) / strideY__28098)).floor();
            stopY__28024 = ((((outputRect.bottom - fundamentalRect.bottom)) / strideY__28098)).ceil();
        }
        return new List<global::Doroti.Flutter.Ui.Rect>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Decoration_imageLibrary
{
    internal static Rect _scaleRect(Rect rect, double scale) => global::Doroti.Flutter.Ui.Rect.fromLTRB((rect.left * scale), (rect.top * scale), (rect.right * scale), (rect.bottom * scale));
}

internal class _BlendedDecorationImage__decoration_image : DecorationImage
{
    public virtual DecorationImage? a { get; private set; }
    public virtual DecorationImage? b { get; private set; }
    public virtual double t { get; private set; } = default!;

    internal _BlendedDecorationImage__decoration_image(DecorationImage? a, DecorationImage? b, double t)
    {
        this.a = a;
        this.b = b;
        this.t = t;
        System.Diagnostics.Debug.Assert(((a is not null) || (b is not null)));
    }

    public override dynamic image => (this.b?.image ?? this.a!.image);
    public override Action<object, global::System.Diagnostics.StackTrace?>? onError => (this.b?.onError ?? this.a!.onError);
    public override ColorFilter? colorFilter => (this.b?.colorFilter ?? this.a!.colorFilter);
    public override BoxFit? fit => (this.b?.fit ?? this.a!.fit);
    public override AlignmentGeometry alignment => (this.b?.alignment ?? this.a!.alignment);
    public override Rect? centerSlice => (this.b?.centerSlice ?? this.a!.centerSlice);
    public override ImageRepeat repeat => (this.b?.repeat ?? this.a!.repeat);
    public override bool matchTextDirection => (this.b?.matchTextDirection ?? this.a!.matchTextDirection);
    public override double scale => (this.b?.scale ?? this.a!.scale);
    public override double opacity => (this.b?.opacity ?? this.a!.opacity);
    public override FilterQuality filterQuality => (this.b?.filterQuality ?? this.a!.filterQuality);
    public override bool invertColors => (this.b?.invertColors ?? this.a!.invertColors);
    public override bool isAntiAlias => (this.b?.isAntiAlias ?? this.a!.isAntiAlias);
    public override DecorationImagePainter createPainter(Action onChanged)
    {
        return new _BlendedDecorationImagePainter__decoration_image(this.a?.createPainter(onChanged), this.b?.createPainter(onChanged), this.t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _BlendedDecorationImage__decoration_image;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((__other is _BlendedDecorationImage__decoration_image) && (object.Equals(((_BlendedDecorationImage__decoration_image)((_BlendedDecorationImage__decoration_image)__other)).a, this.a))) && (object.Equals(((_BlendedDecorationImage__decoration_image)((_BlendedDecorationImage__decoration_image)__other)).b, this.b))) && (((_BlendedDecorationImage__decoration_image)((_BlendedDecorationImage__decoration_image)__other)).t == this.t));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.a, this.b, this.t);
    public override string ToString()
    {
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "_BlendedDecorationImage"))}({this.a}, {this.b}, {this.t})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _BlendedDecorationImagePainter__decoration_image : DecorationImagePainter
{
    public virtual DecorationImagePainter? a { get; private set; }
    public virtual DecorationImagePainter? b { get; private set; }
    public virtual double t { get; private set; } = default!;

    internal _BlendedDecorationImagePainter__decoration_image(DecorationImagePainter? a, DecorationImagePainter? b, double t)
    {
        this.a = a;
        this.b = b;
        this.t = t;
    }

    public virtual void paint(Canvas canvas, Rect rect, Path? clipPath, ImageConfiguration configuration, double blend = 1.0, BlendMode blendMode = BlendMode.srcOver)
    {
        canvas.saveLayer(null, new global::Doroti.Flutter.Ui.Paint());
        this.a?.paint(canvas, rect, clipPath, configuration, blend: (blend * ((1.0 - this.t))), blendMode: blendMode);
        this.b?.paint(canvas, rect, clipPath, configuration, blend: (blend * this.t), blendMode: ((this.a is not null) ? BlendMode.plus : blendMode));
        canvas.restore();
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this.a?.dispose();
        this.b?.dispose();
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "_BlendedDecorationImagePainter"))}({this.a}, {this.b}, {this.t})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

