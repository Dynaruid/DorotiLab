// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/image.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Rendering;

public class RenderImage : RenderBox
{
    internal virtual global::Doroti.Generated.Framework.Painting.Alignment? _resolvedAlignment { get; set; } = default;
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
    internal virtual global::Doroti.Generated.Framework.Painting.BoxFit? _fit { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry _alignment { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Painting.ImageRepeat _repeat { get; set; } = default!;
    internal virtual Rect? _centerSlice { get; set; } = default;
    internal virtual bool _invertColors { get; set; } = default!;
    internal virtual bool _matchTextDirection { get; set; } = default!;
    internal virtual TextDirection? _textDirection { get; set; } = default;
    internal virtual bool _isAntiAlias { get; set; } = default!;

    public RenderImage(Image? image = null, string? debugImageLabel = null, double? width = null, double? height = null, double scale = 1.0, Color? color = null, Animation<double>? opacity = null, BlendMode? colorBlendMode = null, global::Doroti.Generated.Framework.Painting.BoxFit? fit = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Generated.Framework.Painting.ImageRepeat repeat = ImageRepeat.noRepeat, Rect? centerSlice = null, bool matchTextDirection = false, TextDirection? textDirection = null, bool invertColors = false, bool isAntiAlias = false, FilterQuality filterQuality = FilterQuality.medium, BlendMode blendMode = BlendMode.srcOver)
    {
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
        this.debugImageLabel = debugImageLabel;
        this._image = image;
        this._width = width;
        this._height = height;
        this._scale = scale;
        this._color = color;
        this._opacity = opacity;
        this._colorBlendMode = colorBlendMode;
        this._fit = fit;
        this._alignment = __alignment;
        this._repeat = repeat;
        this._centerSlice = centerSlice;
        this._matchTextDirection = matchTextDirection;
        this._invertColors = invertColors;
        this._textDirection = textDirection;
        this._isAntiAlias = isAntiAlias;
        this._filterQuality = filterQuality;
        this._blendMode = blendMode;
    }

    internal virtual void _resolve()
    {
        if ((this._resolvedAlignment is not null))
        {
            return;
        }
        _resolvedAlignment = this.alignment.resolve(this.textDirection);
        _flipHorizontally = (this.matchTextDirection && (object.Equals(this.textDirection, TextDirection.rtl)));
    }

    internal virtual void _markNeedResolution()
    {
        _resolvedAlignment = null;
        _flipHorizontally = null;
        markNeedsPaint();
    }

    public virtual global::Doroti.Ui.Image? image
    {
        get => this._image;
        set
        {
            var __value = value is null ? null : (Image)(object)value;
            if ((object.Equals(__value, this._image)))
            {
                return;
            }
            if ((((__value is not null) && (this._image is not null)) && __value.isCloneOf(this._image!)))
            {
                __value.dispose();
                return;
            }
            bool sizeChanged__2889 = ((this._image?.width != __value?.width) || (this._image?.height != __value?.height));
            this._image?.dispose();
            _image = __value;
            markNeedsPaint();
            if ((sizeChanged__2889 && (((this._width is null) || (this._height is null)))))
            {
                markNeedsLayout();
            }
        }
    }
    public virtual double? width
    {
        get => this._width;
        set
        {
            var __value = value;
            if ((__value == this._width))
            {
                return;
            }
            _width = __value;
            markNeedsLayout();
        }
    }
    public virtual double? height
    {
        get => this._height;
        set
        {
            var __value = value;
            if ((__value == this._height))
            {
                return;
            }
            _height = __value;
            markNeedsLayout();
        }
    }
    public virtual double scale
    {
        get => this._scale;
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._scale))
            {
                return;
            }
            _scale = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    internal virtual void _updateColorFilter()
    {
        if ((this._color is null))
        {
            _colorFilter = null;
        }
        else
        {
            _colorFilter = global::Doroti.Ui.ColorFilter.mode(this._color!, (this._colorBlendMode ?? BlendMode.srcIn));
        }
    }

    public virtual global::Doroti.Ui.Color? color
    {
        get => this._color;
        set
        {
            var __value = value is null ? null : (Color)(object)value;
            if ((object.Equals(__value, this._color)))
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
        get => this._opacity;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._opacity)))
            {
                return;
            }
            if (attached)
            {
                this._opacity?.removeListener(markNeedsPaint);
            }
            _opacity = __value;
            if (attached)
            {
                __value?.addListener(markNeedsPaint);
            }
        }
    }
    public virtual global::Doroti.Ui.FilterQuality filterQuality
    {
        get => this._filterQuality;
        set
        {
            var __value = value;
            if ((object.Equals(DartRuntimePrimitives.RequireValue(__value), this._filterQuality)))
            {
                return;
            }
            _filterQuality = DartRuntimePrimitives.RequireValue(__value);
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Ui.BlendMode? colorBlendMode
    {
        get => this._colorBlendMode;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._colorBlendMode)))
            {
                return;
            }
            _colorBlendMode = __value;
            _updateColorFilter();
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Ui.BlendMode blendMode
    {
        get => this._blendMode;
        set
        {
            var __value = value;
            if ((object.Equals(DartRuntimePrimitives.RequireValue(__value), this._blendMode)))
            {
                return;
            }
            _blendMode = DartRuntimePrimitives.RequireValue(__value);
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Generated.Framework.Painting.BoxFit? fit
    {
        get => this._fit;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._fit)))
            {
                return;
            }
            _fit = __value;
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment
    {
        get => this._alignment;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._alignment)))
            {
                return;
            }
            _alignment = __value;
            _markNeedResolution();
        }
    }
    public virtual global::Doroti.Generated.Framework.Painting.ImageRepeat repeat
    {
        get => this._repeat;
        set
        {
            var __value = value;
            if ((object.Equals(DartRuntimePrimitives.RequireValue(__value), this._repeat)))
            {
                return;
            }
            _repeat = DartRuntimePrimitives.RequireValue(__value);
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Ui.Rect? centerSlice
    {
        get => this._centerSlice;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._centerSlice)))
            {
                return;
            }
            _centerSlice = __value;
            markNeedsPaint();
        }
    }
    public virtual bool invertColors
    {
        get => this._invertColors;
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._invertColors))
            {
                return;
            }
            _invertColors = DartRuntimePrimitives.RequireValue(__value);
            markNeedsPaint();
        }
    }
    public virtual bool matchTextDirection
    {
        get => this._matchTextDirection;
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._matchTextDirection))
            {
                return;
            }
            _matchTextDirection = DartRuntimePrimitives.RequireValue(__value);
            _markNeedResolution();
        }
    }
    public virtual global::Doroti.Ui.TextDirection? textDirection
    {
        get => this._textDirection;
        set
        {
            var __value = value;
            if ((object.Equals(this._textDirection, __value)))
            {
                return;
            }
            _textDirection = __value;
            _markNeedResolution();
        }
    }
    public virtual bool isAntiAlias
    {
        get => this._isAntiAlias;
        set
        {
            var __value = value;
            if ((this._isAntiAlias == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _isAntiAlias = DartRuntimePrimitives.RequireValue(__value);
            markNeedsPaint();
        }
    }
    internal virtual global::Doroti.Ui.Size _sizeForConstraints(BoxConstraints constraints)
    {
        constraints = BoxConstraints.CreateTightFor(width: this._width, height: this._height).enforce(constraints);
        if ((this._image is null))
        {
            return ((BoxConstraints)constraints).smallest;
        }
        return constraints.constrainSizeAndAttemptToPreserveAspectRatio(new global::Doroti.Ui.Size((this._image!.width.toDouble() / this._scale), (this._image!.height.toDouble() / this._scale)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        DartRuntimePrimitives.Assert(() => (height >= 0.0));
        if (((this._width is null) && (this._height is null)))
        {
            return 0.0;
        }
        return _sizeForConstraints(BoxConstraints.CreateTightForFinite(height: DartRuntimePrimitives.RequireValue(height))).width;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        DartRuntimePrimitives.Assert(() => (height >= 0.0));
        return _sizeForConstraints(BoxConstraints.CreateTightForFinite(height: DartRuntimePrimitives.RequireValue(height))).width;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        DartRuntimePrimitives.Assert(() => (width >= 0.0));
        if (((this._width is null) && (this._height is null)))
        {
            return 0.0;
        }
        return _sizeForConstraints(BoxConstraints.CreateTightForFinite(width: DartRuntimePrimitives.RequireValue(width))).height;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        DartRuntimePrimitives.Assert(() => (width >= 0.0));
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
        this._opacity?.addListener(markNeedsPaint);
    }

    public override void detach()
    {
        this._opacity?.removeListener(markNeedsPaint);
        base.detach();
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((this._image is null))
        {
            return;
        }
        _resolve();
        DartRuntimePrimitives.Assert(() => (this._resolvedAlignment is not null));
        DartRuntimePrimitives.Assert(() => (this._flipHorizontally is not null));
        global::Doroti.Generated.Framework.Painting.Decoration_imageLibrary.paintImage(canvas: ((PaintingContext)context).canvas, rect: (offset & size), image: this._image!, debugImageLabel: this.debugImageLabel, scale: this._scale, opacity: (this._opacity?.value ?? 1.0), colorFilter: this._colorFilter, fit: this._fit, alignment: this._resolvedAlignment!, centerSlice: this._centerSlice, repeat: this._repeat, flipHorizontally: DartRuntimePrimitives.RequireValue(this._flipHorizontally), invertColors: this.invertColors, filterQuality: this._filterQuality, isAntiAlias: this._isAntiAlias, blendMode: this._blendMode);
    }

    public override void dispose()
    {
        this._image?.dispose();
        _image = null;
        base.dispose();
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Image>("image", this.image));
        properties.add(new DoubleProperty("width", this.width, defaultValue: null));
        properties.add(new DoubleProperty("height", this.height, defaultValue: null));
        properties.add(new DoubleProperty("scale", this.scale, defaultValue: 1.0));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("color", this.color, defaultValue: null));
        properties.add(new DiagnosticsProperty<Animation<double>?>("opacity", this.opacity, defaultValue: null));
        properties.add(new EnumProperty<global::Doroti.Ui.BlendMode>("colorBlendMode", this.colorBlendMode, defaultValue: null));
        properties.add(new EnumProperty<global::Doroti.Generated.Framework.Painting.BoxFit>("fit", this.fit, defaultValue: null));
        properties.add(new DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>("alignment", this.alignment, defaultValue: null));
        properties.add(new EnumProperty<global::Doroti.Generated.Framework.Painting.ImageRepeat>("repeat", this.repeat, defaultValue: global::Doroti.Generated.Framework.Painting.ImageRepeat.noRepeat));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Rect>("centerSlice", this.centerSlice, defaultValue: null));
        properties.add(new FlagProperty("matchTextDirection", value: this.matchTextDirection, ifTrue: "match text direction"));
        properties.add(new EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
        properties.add(new DiagnosticsProperty<bool>("invertColors", this.invertColors));
        properties.add(new EnumProperty<global::Doroti.Ui.FilterQuality>("filterQuality", this.filterQuality));
        properties.add(new EnumProperty<global::Doroti.Ui.BlendMode>("blendMode", this.blendMode, defaultValue: BlendMode.srcOver));
    }

}

