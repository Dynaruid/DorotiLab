// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/decoration_image.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public enum ImageRepeat
{
    repeat,
    repeatX,
    repeatY,
    noRepeat,
}

public class DecorationImage
{
    public virtual IImageProvider image { get; private set; } = default!;
    public virtual Action<object, System.Diagnostics.StackTrace?>? onError { get; private set; }
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

    public DecorationImage(
        IImageProvider image,
        Action<object, System.Diagnostics.StackTrace?>? onError = null,
        ColorFilter? colorFilter = null,
        BoxFit? fit = null,
        AlignmentGeometry alignment = default!,
        Rect? centerSlice = null,
        ImageRepeat repeat = ImageRepeat.noRepeat,
        bool matchTextDirection = false,
        double scale = 1.0,
        double opacity = 1.0,
        FilterQuality filterQuality = FilterQuality.medium,
        bool invertColors = false,
        bool isAntiAlias = false
    )
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
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is DecorationImage)
            && Equals(__other.image, image)
            && Equals(__other.colorFilter, colorFilter)
            && Equals(__other.fit, fit)
            && Equals(__other.alignment, alignment)
            && Equals(__other.centerSlice, centerSlice)
            && Equals(__other.repeat, repeat)
            && (__other.matchTextDirection == matchTextDirection)
            && (__other.scale == scale)
            && (__other.opacity == opacity)
            && Equals(__other.filterQuality, filterQuality)
            && (__other.invertColors == invertColors)
            && (__other.isAntiAlias == isAntiAlias);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(
            image,
            colorFilter,
            fit,
            alignment,
            centerSlice,
            repeat,
            matchTextDirection,
            scale,
            opacity,
            filterQuality,
            invertColors,
            isAntiAlias
        );

    public override string ToString()
    {
        var properties = new List<string>
        {
            $"{image}",
            $"{alignment}",
            $"scale {scale.toStringAsFixed(1L)}",
            $"opacity {opacity.toStringAsFixed(1L)}",
            $"{filterQuality}",
        };
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "DecorationImage")}({string.Join(", ", properties)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DecorationImage? lerp(DecorationImage? a, DecorationImage? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b) || (t == 0.0))
        {
            return a;
        }
        if (t == 1.0)
        {
            return b;
        }
        return new _BlendedDecorationImage__decoration_image(a, b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public interface DecorationImagePainter
{
    public void paint(
        Canvas canvas,
        Rect rect,
        Path? clipPath,
        ImageConfiguration configuration,
        double blend = 1.0,
        BlendMode blendMode = BlendMode.srcOver
    );
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

    public virtual void paint(
        Canvas canvas,
        Rect rect,
        Path? clipPath,
        ImageConfiguration configuration,
        double blend = 1.0,
        BlendMode blendMode = BlendMode.srcOver
    )
    {
        var flipHorizontallyLocal = false;
        if (_details.matchTextDirection)
        {
            DartRuntimePrimitives.Assert(() =>
            {
                if (configuration.textDirection is null)
                {
                    throw new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary(
                                "DecorationImage.matchTextDirection can only be used when a TextDirection is available."
                            ),
                            new ErrorDescription(
                                "When DecorationImagePainter.paint() was called, there was no text direction provided "
                                    + "in the ImageConfiguration object to match."
                            ),
                            new DiagnosticsProperty<DecorationImage>(
                                "The DecorationImage was",
                                _details,
                                style: DiagnosticsTreeStyle.errorProperty
                            ),
                            new DiagnosticsProperty<ImageConfiguration>(
                                "The ImageConfiguration was",
                                configuration,
                                style: DiagnosticsTreeStyle.errorProperty
                            ),
                        }
                    );
                }
                return true;
            });
            if (Equals(configuration.textDirection, TextDirection.rtl))
            {
                flipHorizontallyLocal = true;
            }
        }
        ImageStream newImageStream = _details.image.resolve(configuration);
        if (!Equals(newImageStream.key, _imageStream?.key))
        {
            var listener = new ImageStreamListener(_handleImage, onError: _details.onError);
            _imageStream?.removeListener(listener);
            _imageStream = newImageStream;
            _imageStream!.addListener(listener);
        }
        if (_image is null)
        {
            return;
        }
        if (clipPath is not null)
        {
            canvas.save();
            canvas.clipPath(clipPath);
        }
        Decoration_imageLibrary.paintImage(
            canvas: canvas,
            rect: rect,
            image: _image!.image,
            debugImageLabel: _image!.debugLabel,
            scale: _details.scale * _image!.scale,
            colorFilter: _details.colorFilter,
            fit: _details.fit,
            alignment: _details.alignment.resolve(configuration.textDirection),
            centerSlice: _details.centerSlice,
            repeat: _details.repeat,
            flipHorizontally: flipHorizontallyLocal,
            opacity: _details.opacity * blend,
            filterQuality: _details.filterQuality,
            invertColors: _details.invertColors,
            isAntiAlias: _details.isAntiAlias,
            blendMode: blendMode
        );
        if (clipPath is not null)
        {
            canvas.restore();
        }
    }

    internal virtual void _handleImage(ImageInfo value, bool synchronousCall)
    {
        if (Equals(_image, value))
        {
            return;
        }
        if ((_image is not null) && _image!.isCloneOf(value))
        {
            value.dispose();
            return;
        }
        _image?.dispose();
        _image = value;
        if (!synchronousCall)
        {
            _onChanged();
        }
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            Foundation.DebugLibrary.debugMaybeDispatchDisposed(this)
        );
        _imageStream?.removeListener(
            new ImageStreamListener(_handleImage, onError: _details.onError)
        );
        _image?.dispose();
        _image = null;
    }

    public override string ToString()
    {
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "DecorationImagePainter")}(stream: {_imageStream}, image: {_image}) for {_details}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Decoration_imageLibrary
{
    internal static DartMap<string, ImageSizeInfo> _pendingImageSizeInfo =
        new DartMap<string, ImageSizeInfo>();
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
            _lastFrameImageSizeInfo = new HashSet<ImageSizeInfo>();
            return true;
        });
    }
}

public static partial class Decoration_imageLibrary
{
    public static void paintImage(
        Canvas canvas,
        Rect rect,
        Image image,
        string? debugImageLabel = null,
        double scale = 1.0,
        double opacity = 1.0,
        ColorFilter? colorFilter = null,
        BoxFit? fit = null,
        Alignment alignment = default!,
        Rect? centerSlice = null,
        ImageRepeat repeat = ImageRepeat.noRepeat,
        bool flipHorizontally = false,
        bool invertColors = false,
        FilterQuality filterQuality = FilterQuality.medium,
        bool isAntiAlias = false,
        BlendMode blendMode = BlendMode.srcOver
    )
    {
        alignment ??= Alignment.center;
        DartRuntimePrimitives.Assert(() => !image.debugDisposed);
        if (rect.isEmpty)
        {
            return;
        }
        Size outputSize = rect.size;
        var inputSize = new Size(image.width.toDouble(), image.height.toDouble());
        Offset? sliceBorder = default!;
        if (centerSlice is not null)
        {
            Rect centerSlice__value20139 = (
                centerSlice
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
            sliceBorder = (inputSize / scale) - (centerSlice__value20139).size;
            outputSize =
                outputSize
                - (
                    sliceBorder
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                );
            inputSize =
                inputSize
                - (
                    (
                        sliceBorder
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) * scale
                );
        }
        fit ??= ((centerSlice is null) ? BoxFit.scaleDown : BoxFit.fill);
        DartRuntimePrimitives.Assert(() =>
            (centerSlice is null)
            || (
                (
                    !Equals(
                        (
                            fit
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        ),
                        BoxFit.none
                    )
                )
                && (
                    !Equals(
                        (
                            fit
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        ),
                        BoxFit.cover
                    )
                )
            )
        );
        FittedSizes fittedSizes = Box_fitLibrary.applyBoxFit(
            (
                (
                    fit
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                )
            ),
            inputSize / scale,
            outputSize
        );
        Size sourceSize = fittedSizes.source * scale;
        Size destinationSize = fittedSizes.destination;
        if (centerSlice is not null)
        {
            Rect centerSlice__value20675 = (
                centerSlice
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
            outputSize += (
                sliceBorder
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
            destinationSize += (
                sliceBorder
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
            DartRuntimePrimitives.Assert(() => Equals(sourceSize, inputSize));
        }
        if ((!Equals(repeat, ImageRepeat.noRepeat)) && Equals(destinationSize, outputSize))
        {
            repeat = ImageRepeat.noRepeat;
        }
        var paint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.isAntiAlias = isAntiAlias;
                    return __cascade;
                }
            )
        )();
        if (colorFilter is not null)
        {
            paint.colorFilter = colorFilter;
        }
        paint.color = Color.fromRGBO(0L, 0L, 0L, Dart_uiLibrary.clampDouble(opacity, 0.0, 1.0));
        paint.filterQuality = filterQuality;
        paint.invertColors = invertColors;
        paint.blendMode = blendMode;
        double halfWidthDelta = (outputSize.width - destinationSize.width) / 2.0;
        double halfHeightDelta = (outputSize.height - destinationSize.height) / 2.0;
        double dx =
            halfWidthDelta + ((flipHorizontally ? -alignment.x : alignment.x) * halfWidthDelta);
        double dy = halfHeightDelta + (alignment.y * halfHeightDelta);
        Offset destinationPosition = rect.topLeft.translate(dx, dy);
        Rect destinationRect = destinationPosition & destinationSize;
        var invertedCanvas = false;
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            double maxDevicePixelRatio = Enumerable.Aggregate(
                PaintingBinding.instance.platformDispatcher.views,
                (double)0.0,
                (previousValue, view) => Math.Max(previousValue, view.devicePixelRatio)
            );
            var sizeInfo = new ImageSizeInfo(
                source: debugImageLabel ?? $"<Unknown Image({image.width}×{image.height})>",
                imageSize: new Size(image.width.toDouble(), image.height.toDouble()),
                displaySize: outputSize * maxDevicePixelRatio
            );
            DartRuntimePrimitives.Assert(() =>
            {
                if (
                    DebugLibrary.debugInvertOversizedImages
                    && (
                        sizeInfo.decodedSizeInBytes
                        > (sizeInfo.displaySizeInBytes + DebugLibrary.debugImageOverheadAllowance)
                    )
                )
                {
                    long overheadInKilobytes = checked(
                        (sizeInfo.decodedSizeInBytes - sizeInfo.displaySizeInBytes) / 1024L
                    );
                    long outputWidth = sizeInfo.displaySize.width.toInt();
                    long outputHeight = sizeInfo.displaySize.height.toInt();
                    FlutterError.reportError(
                        new FlutterErrorDetails(
                            exception: $"Image {debugImageLabel} has a display size of "
                                + $"{outputWidth}×{outputHeight} but a decode size of "
                                + $"{image.width}×{image.height}, which uses an additional "
                                + $"{overheadInKilobytes}KB (assuming a device pixel ratio of "
                                + $"{maxDevicePixelRatio}).\n\n"
                                + "Consider resizing the asset ahead of time, supplying a cacheWidth "
                                + $"parameter of {outputWidth}, a cacheHeight parameter of "
                                + $"{outputHeight}, or using a ResizeImage.",
                            library: "painting library",
                            context: new ErrorDescription("while painting an image")
                        )
                    );
                    canvas.saveLayer(
                        destinationRect,
                        (
                            (Func<Paint>)(
                                () =>
                                {
                                    var __cascade = new Paint();
                                    __cascade.colorFilter = ColorFilter.matrix(
                                        new List<double>
                                        {
                                            -1,
                                            0,
                                            0,
                                            0,
                                            255,
                                            0,
                                            -1,
                                            0,
                                            0,
                                            255,
                                            0,
                                            0,
                                            -1,
                                            0,
                                            255,
                                            0,
                                            0,
                                            0,
                                            1,
                                            0,
                                        }
                                    );
                                    return __cascade;
                                }
                            )
                        )()
                    );
                    double dyLocal = -(rect.top + (rect.height / 2.0));
                    canvas.translate(0.0, -dyLocal);
                    canvas.scale(1.0, -1.0);
                    canvas.translate(0.0, dyLocal);
                    invertedCanvas = true;
                }
                return true;
            });
            if (!_lastFrameImageSizeInfo.Contains(sizeInfo))
            {
                ImageSizeInfo? existingSizeInfo = _pendingImageSizeInfo.GetValueOrDefault(
                    DartRuntimePrimitives.RequireReference(sizeInfo.source)
                );
                if (
                    (existingSizeInfo is null)
                    || (existingSizeInfo.displaySizeInBytes < sizeInfo.displaySizeInBytes)
                )
                {
                    _pendingImageSizeInfo[sizeInfo.source!] = sizeInfo;
                }
                DebugLibrary.debugOnPaintImage?.Invoke(sizeInfo);
                SchedulerBinding.instance.addPostFrameCallback(
                    (timeStamp) =>
                    {
                        _lastFrameImageSizeInfo = _pendingImageSizeInfo.Values.toSet();
                        if (checked((long)_pendingImageSizeInfo.Count) == 0)
                        {
                            return;
                        }
                        Dart_developerLibrary.postEvent(
                            "Flutter.ImageSizesForFrame",
                            new DartMap<string, object>()
                        );
                        _pendingImageSizeInfo = new DartMap<string, ImageSizeInfo>();
                    },
                    debugLabel: "paintImage.recordImageSizes"
                );
            }
        }
        bool needSave =
            (centerSlice is not null)
            || (!Equals(repeat, ImageRepeat.noRepeat))
            || flipHorizontally;
        if (needSave)
        {
            canvas.save();
        }
        if (!Equals(repeat, ImageRepeat.noRepeat))
        {
            canvas.clipRect(rect);
        }
        if (flipHorizontally)
        {
            double dxLocal = -(rect.left + (rect.width / 2.0));
            canvas.translate(-dxLocal, 0.0);
            canvas.scale(-1.0, 1.0);
            canvas.translate(dxLocal, 0.0);
        }
        if (centerSlice is null)
        {
            Rect sourceRect = alignment.inscribe(sourceSize, Offset.zero & inputSize);
            if (Equals(repeat, ImageRepeat.noRepeat))
            {
                canvas.drawImageRect(image, sourceRect, destinationRect, paint);
            }
            else
            {
                foreach (Rect tileRect in _generateImageTileRects(rect, destinationRect, repeat))
                {
                    canvas.drawImageRect(image, sourceRect, tileRect, paint);
                }
            }
        }
        else
        {
            canvas.scale(1L / scale);
            if (Equals(repeat, ImageRepeat.noRepeat))
            {
                canvas.drawImageNine(
                    image,
                    _scaleRect(
                        (
                            (
                                centerSlice
                                ?? throw new global::System.NullReferenceException(
                                    "Dart null assertion failed."
                                )
                            )
                        ),
                        scale
                    ),
                    _scaleRect(destinationRect, scale),
                    paint
                );
            }
            else
            {
                foreach (
                    Rect tileRectLocal in _generateImageTileRects(rect, destinationRect, repeat)
                )
                {
                    canvas.drawImageNine(
                        image,
                        _scaleRect(
                            (
                                (
                                    centerSlice
                                    ?? throw new global::System.NullReferenceException(
                                        "Dart null assertion failed."
                                    )
                                )
                            ),
                            scale
                        ),
                        _scaleRect(tileRectLocal, scale),
                        paint
                    );
                }
            }
        }
        if (needSave)
        {
            canvas.restore();
        }
        if (invertedCanvas)
        {
            canvas.restore();
        }
    }
}

public static partial class Decoration_imageLibrary
{
    internal static IEnumerable<Rect> _generateImageTileRects(
        Rect outputRect,
        Rect fundamentalRect,
        ImageRepeat repeat
    )
    {
        var startX = 0L;
        var startY = 0L;
        var stopX = 0L;
        var stopY = 0L;
        double strideX = fundamentalRect.width;
        double strideY = fundamentalRect.height;
        if (Equals(repeat, ImageRepeat.repeat) || Equals(repeat, ImageRepeat.repeatX))
        {
            startX = ((outputRect.left - fundamentalRect.left) / strideX).floor();
            stopX = ((outputRect.right - fundamentalRect.right) / strideX).ceil();
        }
        if (Equals(repeat, ImageRepeat.repeat) || Equals(repeat, ImageRepeat.repeatY))
        {
            startY = ((outputRect.top - fundamentalRect.top) / strideY).floor();
            stopY = ((outputRect.bottom - fundamentalRect.bottom) / strideY).ceil();
        }
        return new List<Rect>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Decoration_imageLibrary
{
    internal static Rect _scaleRect(Rect rect, double scale) =>
        Rect.fromLTRB(rect.left * scale, rect.top * scale, rect.right * scale, rect.bottom * scale);
}

internal class _BlendedDecorationImage__decoration_image : DecorationImage
{
    public virtual DecorationImage? a { get; private set; }
    public virtual DecorationImage? b { get; private set; }
    public virtual double t { get; private set; } = default!;

    internal _BlendedDecorationImage__decoration_image(
        DecorationImage? a,
        DecorationImage? b,
        double t
    )
    {
        this.a = a;
        this.b = b;
        this.t = t;
        System.Diagnostics.Debug.Assert((a is not null) || (b is not null));
    }

    public override IImageProvider image => b?.image ?? a!.image;
    public override Action<object, System.Diagnostics.StackTrace?>? onError =>
        b?.onError ?? a!.onError;
    public override ColorFilter? colorFilter => b?.colorFilter ?? a!.colorFilter;
    public override BoxFit? fit => b?.fit ?? a!.fit;
    public override AlignmentGeometry alignment => b?.alignment ?? a!.alignment;
    public override Rect? centerSlice => b?.centerSlice ?? a!.centerSlice;
    public override ImageRepeat repeat => b?.repeat ?? a!.repeat;
    public override bool matchTextDirection => b?.matchTextDirection ?? a!.matchTextDirection;
    public override double scale => b?.scale ?? a!.scale;
    public override double opacity => b?.opacity ?? a!.opacity;
    public override FilterQuality filterQuality => b?.filterQuality ?? a!.filterQuality;
    public override bool invertColors => b?.invertColors ?? a!.invertColors;
    public override bool isAntiAlias => b?.isAntiAlias ?? a!.isAntiAlias;

    public override DecorationImagePainter createPainter(Action onChanged)
    {
        return new _BlendedDecorationImagePainter__decoration_image(
            a?.createPainter(onChanged),
            b?.createPainter(onChanged),
            t
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _BlendedDecorationImage__decoration_image;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is _BlendedDecorationImage__decoration_image)
            && Equals(__other.a, a)
            && Equals(__other.b, b)
            && (__other.t == t);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(a, b, t);

    public override string ToString()
    {
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "_BlendedDecorationImage")}({a}, {b}, {t})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _BlendedDecorationImagePainter__decoration_image : DecorationImagePainter
{
    public virtual DecorationImagePainter? a { get; private set; }
    public virtual DecorationImagePainter? b { get; private set; }
    public virtual double t { get; private set; } = default!;

    internal _BlendedDecorationImagePainter__decoration_image(
        DecorationImagePainter? a,
        DecorationImagePainter? b,
        double t
    )
    {
        this.a = a;
        this.b = b;
        this.t = t;
    }

    public virtual void paint(
        Canvas canvas,
        Rect rect,
        Path? clipPath,
        ImageConfiguration configuration,
        double blend = 1.0,
        BlendMode blendMode = BlendMode.srcOver
    )
    {
        canvas.saveLayer(null, new Paint());
        a?.paint(
            canvas,
            rect,
            clipPath,
            configuration,
            blend: blend * (1.0 - t),
            blendMode: blendMode
        );
        b?.paint(
            canvas,
            rect,
            clipPath,
            configuration,
            blend: blend * t,
            blendMode: (a is not null) ? BlendMode.plus : blendMode
        );
        canvas.restore();
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            Foundation.DebugLibrary.debugMaybeDispatchDisposed(this)
        );
        a?.dispose();
        b?.dispose();
    }

    public override string ToString()
    {
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "_BlendedDecorationImagePainter")}({a}, {b}, {t})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
