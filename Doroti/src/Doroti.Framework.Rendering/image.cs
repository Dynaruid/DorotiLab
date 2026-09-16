// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/image.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public class RenderImage : RenderBox
{
    internal virtual Alignment? _resolvedAlignment { get; set; } = default;
    internal virtual bool? _flipHorizontally { get; set; } = default;
    internal virtual Image? _image { get; set; } = default;
    public virtual string? debugImageLabel { get; set; } = default;
    internal virtual double? _width { get; set; } = default;
    internal virtual double? _height { get; set; } = default;
    internal virtual double _scale { get; set; } = default!;
    internal virtual ColorFilter? _colorFilter { get; set; } = default;
    internal virtual Color? _color { get; set; } = default;
    internal virtual Animation<double>? _opacity { get; set; } = default;
    internal virtual FilterQuality _filterQuality { get; set; } = default!;
    internal virtual BlendMode? _colorBlendMode { get; set; } = default;
    internal virtual BlendMode _blendMode { get; set; } = default!;
    internal virtual BoxFit? _fit { get; set; } = default;
    internal virtual AlignmentGeometry _alignment { get; set; } = default!;
    internal virtual ImageRepeat _repeat { get; set; } = default!;
    internal virtual Rect? _centerSlice { get; set; } = default;
    internal virtual bool _invertColors { get; set; } = default!;
    internal virtual bool _matchTextDirection { get; set; } = default!;
    internal virtual TextDirection? _textDirection { get; set; } = default;
    internal virtual bool _isAntiAlias { get; set; } = default!;

    public RenderImage(Image? image = null, string? debugImageLabel = null, double? width = null, double? height = null, double scale = 1.0, Color? color = null, Animation<double>? opacity = null, BlendMode? colorBlendMode = null, BoxFit? fit = null, AlignmentGeometry alignment = default!, ImageRepeat repeat = ImageRepeat.noRepeat, Rect? centerSlice = null, bool matchTextDirection = false, TextDirection? textDirection = null, bool invertColors = false, bool isAntiAlias = false, FilterQuality filterQuality = FilterQuality.medium, BlendMode blendMode = BlendMode.srcOver)
    {
        AlignmentGeometry __alignment = alignment ?? Alignment.center;
        this.debugImageLabel = debugImageLabel;
        _image = image;
        _width = width;
        _height = height;
        _scale = scale;
        _color = color;
        _opacity = opacity;
        _colorBlendMode = colorBlendMode;
        _fit = fit;
        _alignment = __alignment;
        _repeat = repeat;
        _centerSlice = centerSlice;
        _matchTextDirection = matchTextDirection;
        _invertColors = invertColors;
        _textDirection = textDirection;
        _isAntiAlias = isAntiAlias;
        _filterQuality = filterQuality;
        _blendMode = blendMode;
    }

    internal virtual void _resolve()
    {
        if (_resolvedAlignment is not null)
        {
            return;
        }
        _resolvedAlignment = alignment.resolve(textDirection);
        _flipHorizontally = matchTextDirection && Equals(textDirection, TextDirection.rtl);
    }

    internal virtual void _markNeedResolution()
    {
        _resolvedAlignment = null;
        _flipHorizontally = null;
        markNeedsPaint();
    }

    public virtual Image? image
    {
        get => _image;
        set
        {
            var __value = value is null ? null : (Image)(object)value;
            if (Equals(__value, _image))
            {
                return;
            }
            if ((__value is not null) && (_image is not null) && __value.isCloneOf(_image!))
            {
                __value.dispose();
                return;
            }
            bool sizeChanged = (_image?.width != __value?.width) || (_image?.height != __value?.height);
            _image?.dispose();
            _image = __value;
            markNeedsPaint();
            if (sizeChanged && ((_width is null) || (_height is null)))
            {
                markNeedsLayout();
            }
        }
    }
    public virtual double? width
    {
        get => _width;
        set
        {
            var __value = value;
            if (__value == _width)
            {
                return;
            }
            _width = __value;
            markNeedsLayout();
        }
    }
    public virtual double? height
    {
        get => _height;
        set
        {
            var __value = value;
            if (__value == _height)
            {
                return;
            }
            _height = __value;
            markNeedsLayout();
        }
    }
    public virtual double scale
    {
        get => _scale;
        set
        {
            var __value = value;
            if (DartRuntimePrimitives.RequireValue(__value) == _scale)
            {
                return;
            }
            _scale = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    internal virtual void _updateColorFilter()
    {
        if (_color is null)
        {
            _colorFilter = null;
        }
        else
        {
            _colorFilter = ColorFilter.mode(_color!, _colorBlendMode ?? BlendMode.srcIn);
        }
    }

    public virtual Color? color
    {
        get => _color;
        set
        {
            var __value = value is null ? null : (Color)(object)value;
            if (Equals(__value, _color))
            {
                return;
            }
            _color = __value;
            _updateColorFilter();
            markNeedsPaint();
        }
    }
    public virtual Animation<double>? opacity
    {
        get => _opacity;
        set
        {
            var __value = value;
            if (Equals(__value, _opacity))
            {
                return;
            }
            if (attached)
            {
                _opacity?.removeListener(markNeedsPaint);
            }
            _opacity = __value;
            if (attached)
            {
                __value?.addListener(markNeedsPaint);
            }
        }
    }
    public virtual FilterQuality filterQuality
    {
        get => _filterQuality;
        set
        {
            var __value = value;
            if (Equals(DartRuntimePrimitives.RequireValue(__value), _filterQuality))
            {
                return;
            }
            _filterQuality = DartRuntimePrimitives.RequireValue(__value);
            markNeedsPaint();
        }
    }
    public virtual BlendMode? colorBlendMode
    {
        get => _colorBlendMode;
        set
        {
            var __value = value;
            if (Equals(__value, _colorBlendMode))
            {
                return;
            }
            _colorBlendMode = __value;
            _updateColorFilter();
            markNeedsPaint();
        }
    }
    public virtual BlendMode blendMode
    {
        get => _blendMode;
        set
        {
            var __value = value;
            if (Equals(DartRuntimePrimitives.RequireValue(__value), _blendMode))
            {
                return;
            }
            _blendMode = DartRuntimePrimitives.RequireValue(__value);
            markNeedsPaint();
        }
    }
    public virtual BoxFit? fit
    {
        get => _fit;
        set
        {
            var __value = value;
            if (Equals(__value, _fit))
            {
                return;
            }
            _fit = __value;
            markNeedsPaint();
        }
    }
    public virtual AlignmentGeometry alignment
    {
        get => _alignment;
        set
        {
            var __value = value;
            if (Equals(__value, _alignment))
            {
                return;
            }
            _alignment = __value;
            _markNeedResolution();
        }
    }
    public virtual ImageRepeat repeat
    {
        get => _repeat;
        set
        {
            var __value = value;
            if (Equals(DartRuntimePrimitives.RequireValue(__value), _repeat))
            {
                return;
            }
            _repeat = DartRuntimePrimitives.RequireValue(__value);
            markNeedsPaint();
        }
    }
    public virtual Rect? centerSlice
    {
        get => _centerSlice;
        set
        {
            var __value = value;
            if (Equals(__value, _centerSlice))
            {
                return;
            }
            _centerSlice = __value;
            markNeedsPaint();
        }
    }
    public virtual bool invertColors
    {
        get => _invertColors;
        set
        {
            var __value = value;
            if (DartRuntimePrimitives.RequireValue(__value) == _invertColors)
            {
                return;
            }
            _invertColors = DartRuntimePrimitives.RequireValue(__value);
            markNeedsPaint();
        }
    }
    public virtual bool matchTextDirection
    {
        get => _matchTextDirection;
        set
        {
            var __value = value;
            if (DartRuntimePrimitives.RequireValue(__value) == _matchTextDirection)
            {
                return;
            }
            _matchTextDirection = DartRuntimePrimitives.RequireValue(__value);
            _markNeedResolution();
        }
    }
    public virtual TextDirection? textDirection
    {
        get => _textDirection;
        set
        {
            var __value = value;
            if (Equals(_textDirection, __value))
            {
                return;
            }
            _textDirection = __value;
            _markNeedResolution();
        }
    }
    public virtual bool isAntiAlias
    {
        get => _isAntiAlias;
        set
        {
            var __value = value;
            if (_isAntiAlias == DartRuntimePrimitives.RequireValue(__value))
            {
                return;
            }
            _isAntiAlias = DartRuntimePrimitives.RequireValue(__value);
            markNeedsPaint();
        }
    }
    internal virtual Size _sizeForConstraints(BoxConstraints constraints)
    {
        constraints = BoxConstraints.CreateTightFor(width: _width, height: _height).enforce(constraints);
        if (_image is null)
        {
            return constraints.smallest;
        }
        return constraints.constrainSizeAndAttemptToPreserveAspectRatio(new Size(_image!.width.toDouble() / _scale, _image!.height.toDouble() / _scale));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        DartRuntimePrimitives.Assert(() => height >= 0.0);
        if ((_width is null) && (_height is null))
        {
            return 0.0;
        }
        return _sizeForConstraints(BoxConstraints.CreateTightForFinite(height: DartRuntimePrimitives.RequireValue(height))).width;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        DartRuntimePrimitives.Assert(() => height >= 0.0);
        return _sizeForConstraints(BoxConstraints.CreateTightForFinite(height: DartRuntimePrimitives.RequireValue(height))).width;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        DartRuntimePrimitives.Assert(() => width >= 0.0);
        if ((_width is null) && (_height is null))
        {
            return 0.0;
        }
        return _sizeForConstraints(BoxConstraints.CreateTightForFinite(width: DartRuntimePrimitives.RequireValue(width))).height;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        DartRuntimePrimitives.Assert(() => width >= 0.0);
        return _sizeForConstraints(BoxConstraints.CreateTightForFinite(width: DartRuntimePrimitives.RequireValue(width))).height;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestSelf(Offset position) => true;
    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _sizeForConstraints(constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = _sizeForConstraints(constraints);
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        _opacity?.addListener(markNeedsPaint);
    }

    public override void detach()
    {
        _opacity?.removeListener(markNeedsPaint);
        base.detach();
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (_image is null)
        {
            return;
        }
        _resolve();
        DartRuntimePrimitives.Assert(() => _resolvedAlignment is not null);
        DartRuntimePrimitives.Assert(() => _flipHorizontally is not null);
        Decoration_imageLibrary.paintImage(canvas: context.canvas, rect: offset & size, image: _image!, debugImageLabel: debugImageLabel, scale: _scale, opacity: _opacity?.value ?? 1.0, colorFilter: _colorFilter, fit: _fit, alignment: _resolvedAlignment!, centerSlice: _centerSlice, repeat: _repeat, flipHorizontally: DartRuntimePrimitives.RequireValue(_flipHorizontally), invertColors: invertColors, filterQuality: _filterQuality, isAntiAlias: _isAntiAlias, blendMode: _blendMode);
    }

    public override void dispose()
    {
        _image?.dispose();
        _image = null;
        base.dispose();
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Image>("image", image));
        properties.add(new DoubleProperty("width", width, defaultValue: null));
        properties.add(new DoubleProperty("height", height, defaultValue: null));
        properties.add(new DoubleProperty("scale", scale, defaultValue: 1.0));
        properties.add(new ColorProperty("color", color, defaultValue: null));
        properties.add(new DiagnosticsProperty<Animation<double>?>("opacity", opacity, defaultValue: null));
        properties.add(new EnumProperty<BlendMode>("colorBlendMode", colorBlendMode, defaultValue: null));
        properties.add(new EnumProperty<BoxFit>("fit", fit, defaultValue: null));
        properties.add(new DiagnosticsProperty<AlignmentGeometry>("alignment", alignment, defaultValue: null));
        properties.add(new EnumProperty<ImageRepeat>("repeat", repeat, defaultValue: ImageRepeat.noRepeat));
        properties.add(new DiagnosticsProperty<Rect>("centerSlice", centerSlice, defaultValue: null));
        properties.add(new FlagProperty("matchTextDirection", value: matchTextDirection, ifTrue: "match text direction"));
        properties.add(new EnumProperty<TextDirection>("textDirection", textDirection, defaultValue: null));
        properties.add(new DiagnosticsProperty<bool>("invertColors", invertColors));
        properties.add(new EnumProperty<FilterQuality>("filterQuality", filterQuality));
        properties.add(new EnumProperty<BlendMode>("blendMode", blendMode, defaultValue: BlendMode.srcOver));
    }

}

