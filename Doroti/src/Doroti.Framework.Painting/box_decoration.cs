// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/box_decoration.dart
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

namespace Doroti.Generated.Framework.Painting;

public class BoxDecoration : Decoration
{
    public virtual Color? color { get; private set; }
    public virtual DecorationImage? image { get; private set; }
    public virtual BoxBorder? border { get; private set; }
    public virtual BorderRadiusGeometry? borderRadius { get; private set; }
    public virtual List<BoxShadow>? boxShadow { get; private set; }
    public virtual Gradient? gradient { get; private set; }
    public virtual BlendMode? backgroundBlendMode { get; private set; }
    public virtual BoxShape shape { get; private set; } = default!;

    public BoxDecoration(Color? color = null, DecorationImage? image = null, BoxBorder? border = null, BorderRadiusGeometry? borderRadius = null, List<BoxShadow>? boxShadow = null, Gradient? gradient = null, BlendMode? backgroundBlendMode = null, BoxShape shape = BoxShape.rectangle)
    {
        this.color = color;
        this.image = image;
        this.border = border;
        this.borderRadius = borderRadius;
        this.boxShadow = boxShadow;
        this.gradient = gradient;
        this.backgroundBlendMode = backgroundBlendMode;
        this.shape = shape;
        System.Diagnostics.Debug.Assert((((backgroundBlendMode is null) || (color is not null)) || (gradient is not null)));
    }

    public virtual BoxDecoration copyWith(Color? color = null, DecorationImage? image = null, BoxBorder? border = null, BorderRadiusGeometry? borderRadius = null, List<BoxShadow>? boxShadow = null, Gradient? gradient = null, BlendMode? backgroundBlendMode = null, BoxShape? shape = null)
    {
        return new BoxDecoration(color: (color ?? this.color), image: (image ?? this.image), border: (border ?? this.border), borderRadius: (borderRadius ?? this.borderRadius), boxShadow: (boxShadow ?? this.boxShadow), gradient: (gradient ?? this.gradient), backgroundBlendMode: (backgroundBlendMode ?? this.backgroundBlendMode), shape: (shape ?? this.shape));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool debugAssertIsValid()
    {
        DartRuntimePrimitives.Assert(() => ((!object.Equals(this.shape, BoxShape.circle)) || (this.borderRadius is null)));
        return base.debugAssertIsValid();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsGeometry padding => (this.border?.dimensions ?? EdgeInsets.zero);
    public override Path getClipPath(Rect rect, TextDirection textDirection)
    {
        switch (this.shape)
        {
            case BoxShape.circle:
                {
                    global::Doroti.Ui.Offset center__8185 = rect.center;
                    double radius__8228 = (rect.shortestSide / 2.0);
                    var square__8276 = global::Doroti.Ui.Rect.fromCircle(center: center__8185, radius: radius__8228);
                    return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addOval(square__8276);
    return __cascade;
}))();
                }
            case BoxShape.rectangle:
                {
                    if ((this.borderRadius is not null))
                    {
                        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRRect(this.borderRadius!.resolve(DartRuntimePrimitives.RequireValue(textDirection)).toRRect(rect));
    return __cascade;
}))();
                    }
                    return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRect(rect);
    return __cascade;
}))();
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxDecoration scale(double factor)
    {
        return new BoxDecoration(color: Dart_uiLibrary.Color.lerp(null, this.color, factor), image: DecorationImage.lerp(null, this.image, factor), border: BoxBorder.lerp(null, this.border, factor), borderRadius: BorderRadiusGeometry.lerp(null, this.borderRadius, factor), boxShadow: BoxShadow.lerpList(null, this.boxShadow, factor), gradient: this.gradient?.scale(factor), shape: this.shape);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isComplex => (this.boxShadow is not null);
    public override BoxDecoration? lerpFrom(Decoration? a, double t) => (a switch { null => scale(t), BoxDecoration __object9248 => BoxDecoration.lerp(((BoxDecoration)__object9248), this, t), _ => ((BoxDecoration?)(object?)base.lerpFrom(a, t))! });
    public override BoxDecoration? lerpTo(Decoration? b, double t) => (b switch { null => scale((1.0 - t)), BoxDecoration __object9463 => BoxDecoration.lerp(this, ((BoxDecoration)__object9463), t), _ => ((BoxDecoration?)(object?)base.lerpTo(b, t))! });
    public static BoxDecoration? lerp(BoxDecoration? a, BoxDecoration? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if ((a is null))
        {
            return b!.scale(t);
        }
        if ((b is null))
        {
            return a.scale((1.0 - t));
        }
        if ((t == 0.0))
        {
            return a;
        }
        if ((t == 1.0))
        {
            return b;
        }
        return new BoxDecoration(color: Dart_uiLibrary.Color.lerp(((BoxDecoration)a).color, ((BoxDecoration)b).color, t), image: DecorationImage.lerp(((BoxDecoration)a).image, ((BoxDecoration)b).image, t), border: BoxBorder.lerp(((BoxDecoration)a).border, ((BoxDecoration)b).border, t), borderRadius: BorderRadiusGeometry.lerp(((BoxDecoration)a).borderRadius, ((BoxDecoration)b).borderRadius, t), boxShadow: BoxShadow.lerpList(((BoxDecoration)a).boxShadow, ((BoxDecoration)b).boxShadow, t), gradient: Gradient.lerp(((BoxDecoration)a).gradient, ((BoxDecoration)b).gradient, t), shape: ((t < 0.5) ? ((BoxDecoration)a).shape : ((BoxDecoration)b).shape));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as BoxDecoration;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((__other is BoxDecoration) && (object.Equals(((BoxDecoration)((BoxDecoration)__other)).color, this.color))) && (object.Equals(((BoxDecoration)((BoxDecoration)__other)).image, this.image))) && (object.Equals(((BoxDecoration)((BoxDecoration)__other)).border, this.border))) && (object.Equals(((BoxDecoration)((BoxDecoration)__other)).borderRadius, this.borderRadius))) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals<BoxShadow>(((BoxDecoration)((BoxDecoration)__other)).boxShadow, this.boxShadow)) && (object.Equals(((BoxDecoration)((BoxDecoration)__other)).gradient, this.gradient))) && (object.Equals(((BoxDecoration)((BoxDecoration)__other)).backgroundBlendMode, this.backgroundBlendMode))) && (object.Equals(((BoxDecoration)((BoxDecoration)__other)).shape, this.shape)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.color, this.image, this.border, this.borderRadius, ((this.boxShadow is null) ? null : FoundationRuntimePorts.ObjectHashAll(this.boxShadow!)), this.gradient, this.backgroundBlendMode, this.shape);
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        ((Func<DiagnosticPropertiesBuilder>)(() =>
{
    var __cascade = properties;
    __cascade.defaultDiagnosticsTreeStyle = DiagnosticsTreeStyle.whitespace;
    __cascade.emptyBodyDescription = "<no decorations specified>";
    return __cascade;
}))();
        properties.add(new ColorProperty("color", this.color, defaultValue: null));
        properties.add(new DiagnosticsProperty<DecorationImage>("image", this.image, defaultValue: null));
        properties.add(new DiagnosticsProperty<BoxBorder>("border", this.border, defaultValue: null));
        properties.add(new DiagnosticsProperty<BorderRadiusGeometry>("borderRadius", this.borderRadius, defaultValue: null));
        properties.add(new IterableProperty<BoxShadow>("boxShadow", this.boxShadow, defaultValue: null, style: DiagnosticsTreeStyle.whitespace));
        properties.add(new DiagnosticsProperty<Gradient>("gradient", this.gradient, defaultValue: null));
        properties.add(new EnumProperty<BoxShape>("shape", this.shape, defaultValue: BoxShape.rectangle));
    }

    public override bool hitTest(Size size, Offset position, TextDirection? textDirection = null)
    {
        DartRuntimePrimitives.Assert(() => ((Offset.zero & size)).contains(position));
        switch (this.shape)
        {
            case BoxShape.rectangle:
                {
                    if ((this.borderRadius is not null))
                    {
                        global::Doroti.Ui.RRect bounds__13482 = this.borderRadius!.resolve(textDirection).toRRect((Offset.zero & size));
                        return bounds__13482.contains(position);
                    }
                    return true;
                }
            case BoxShape.circle:
                {
                    global::Doroti.Ui.Offset center__13743 = size.center(Offset.zero);
                    double radius__13799 = (Math.Min(size.width, size.height) / 2.0);
                    return (((position - center__13743)).distanceSquared <= (radius__13799 * radius__13799));
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BoxPainter createBoxPainter(Action onChanged = default!)
    {
        DartRuntimePrimitives.Assert(() => ((onChanged is not null) || (this.image is null)));
        return new _BoxDecorationPainter__box_decoration(this, onChanged);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _BoxDecorationPainter__box_decoration : BoxPainter
{
    internal virtual BoxDecoration _decoration { get; private set; } = default!;
    internal virtual Paint? _cachedBackgroundPaint { get; set; } = default;
    internal virtual Rect? _rectForCachedBackgroundPaint { get; set; } = default;
    internal virtual DecorationImagePainter? _imagePainter { get; set; } = default;

    internal _BoxDecorationPainter__box_decoration(BoxDecoration _decoration, Action? onChanged) : base(onChanged)
    {
        this._decoration = _decoration;
    }

    internal virtual global::Doroti.Ui.Paint _getBackgroundPaint(Rect rect, TextDirection? textDirection)
    {
        DartRuntimePrimitives.Assert(() => ((((BoxDecoration)this._decoration).gradient is not null) || (this._rectForCachedBackgroundPaint is null)));
        if (((this._cachedBackgroundPaint is null) || (((((BoxDecoration)this._decoration).gradient is not null) && (!object.Equals(this._rectForCachedBackgroundPaint, rect))))))
        {
            var paint__14758 = new global::Doroti.Ui.Paint();
            if ((((BoxDecoration)this._decoration).backgroundBlendMode is not null))
            {
                paint__14758.blendMode = DartRuntimePrimitives.RequireValue(((BoxDecoration)this._decoration).backgroundBlendMode);
            }
            if ((((BoxDecoration)this._decoration).color is not null))
            {
                paint__14758.color = ((BoxDecoration)this._decoration).color!;
            }
            if ((((BoxDecoration)this._decoration).gradient is not null))
            {
                paint__14758.shader = ((BoxDecoration)this._decoration).gradient!.createShader(rect, textDirection: textDirection);
                _rectForCachedBackgroundPaint = rect;
            }
            _cachedBackgroundPaint = paint__14758;
        }
        return this._cachedBackgroundPaint!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paintBox(Canvas canvas, Rect rect, Paint paint, TextDirection? textDirection)
    {
        switch (((BoxDecoration)this._decoration).shape)
        {
            case BoxShape.circle:
                {
                    DartRuntimePrimitives.Assert(() => (((BoxDecoration)this._decoration).borderRadius is null));
                    global::Doroti.Ui.Offset center__15608 = rect.center;
                    double radius__15651 = (rect.shortestSide / 2.0);
                    canvas.drawCircle(center__15608, radius__15651, paint);
                    break;
                }
            case BoxShape.rectangle:
                {
                    if (((((BoxDecoration)this._decoration).borderRadius is null) || (object.Equals(((BoxDecoration)this._decoration).borderRadius, BorderRadius.zero))))
                    {
                        canvas.drawRect(rect, paint);
                    }
                    else
                    {
                        canvas.drawRRect(((BoxDecoration)this._decoration).borderRadius!.resolve(textDirection).toRRect(rect), paint);
                    }
                    break;
                }
        }
    }

    internal virtual void _paintShadows(Canvas canvas, Rect rect, TextDirection? textDirection)
    {
        if ((((BoxDecoration)this._decoration).boxShadow is null))
        {
            return;
        }
        foreach (BoxShadow boxShadow__16205 in ((BoxDecoration)this._decoration).boxShadow!)
        {
            global::Doroti.Ui.Paint paint__16262 = boxShadow__16205.toPaint();
            global::Doroti.Ui.Rect bounds__16308 = rect.shift(boxShadow__16205.offset).inflate(((BoxShadow)boxShadow__16205).spreadRadius);
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((global::Doroti.Generated.Framework.Painting.DebugLibrary.debugDisableShadows && (object.Equals(((BoxShadow)boxShadow__16205).blurStyle, BlurStyle.outer))))
                    {
                        canvas.save();
                        canvas.clipRect(bounds__16308);
                    }
                    return true;
                });
            _paintBox(canvas, bounds__16308, paint__16262, textDirection);
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((global::Doroti.Generated.Framework.Painting.DebugLibrary.debugDisableShadows && (object.Equals(((BoxShadow)boxShadow__16205).blurStyle, BlurStyle.outer))))
                    {
                        canvas.restore();
                    }
                    return true;
                });
        }
    }

    internal virtual void _paintBackgroundColor(Canvas canvas, Rect rect, TextDirection? textDirection)
    {
        if (((((BoxDecoration)this._decoration).color is not null) || (((BoxDecoration)this._decoration).gradient is not null)))
        {
            global::Doroti.Ui.Rect adjustedRect__17136 = _adjustedRectOnOutlinedBorder(rect, textDirection);
            _paintBox(canvas, adjustedRect__17136, _getBackgroundPaint(rect, textDirection), textDirection);
        }
    }

    internal virtual double _calculateAdjustedSide(BorderSide side)
    {
        if (((((BorderSide)side).color.alpha == 255L) && (object.Equals(((BorderSide)side).style, BorderStyle.solid))))
        {
            return ((BorderSide)side).strokeInset;
        }
        return 0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Rect _adjustedRectOnOutlinedBorder(Rect rect, TextDirection? textDirection)
    {
        if ((((BoxDecoration)this._decoration).border is null))
        {
            return rect;
        }
        if ((((BoxDecoration)this._decoration).border is Border))
        {
            var border__17683 = ((Border?)(object?)((BoxDecoration)this._decoration).border!)!;
            EdgeInsets insets__17747 = (new EdgeInsets(_calculateAdjustedSide(((Border)border__17683).left), _calculateAdjustedSide(((Border)border__17683).top), _calculateAdjustedSide(((Border)border__17683).right), _calculateAdjustedSide(((Border)border__17683).bottom)).op_Divide(2));
            return global::Doroti.Ui.Rect.fromLTRB((rect.left + ((EdgeInsets)insets__17747).left), (rect.top + ((EdgeInsets)insets__17747).top), (rect.right - ((EdgeInsets)insets__17747).right), (rect.bottom - ((EdgeInsets)insets__17747).bottom));
        }
        else
        {
            if (((((BoxDecoration)this._decoration).border is BorderDirectional) && (textDirection is not null)))
            {
                TextDirection textDirection__value18244 = DartRuntimePrimitives.RequireValue(textDirection);
                var border__18281 = ((BorderDirectional?)(object?)((BoxDecoration)this._decoration).border!)!;
                BorderSide leftSide__18355 = ((object.Equals(DartRuntimePrimitives.RequireValue(textDirection__value18244), TextDirection.rtl)) ? ((BorderDirectional)border__18281).end : ((BorderDirectional)border__18281).start);
                BorderSide rightSide__18453 = ((object.Equals(DartRuntimePrimitives.RequireValue(textDirection__value18244), TextDirection.rtl)) ? ((BorderDirectional)border__18281).start : ((BorderDirectional)border__18281).end);
                EdgeInsets insets__18553 = (new EdgeInsets(_calculateAdjustedSide(leftSide__18355), _calculateAdjustedSide(((BorderDirectional)border__18281).top), _calculateAdjustedSide(rightSide__18453), _calculateAdjustedSide(((BorderDirectional)border__18281).bottom)).op_Divide(2));
                return global::Doroti.Ui.Rect.fromLTRB((rect.left + ((EdgeInsets)insets__18553).left), (rect.top + ((EdgeInsets)insets__18553).top), (rect.right - ((EdgeInsets)insets__18553).right), (rect.bottom - ((EdgeInsets)insets__18553).bottom));
            }
        }
        return rect;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paintBackgroundImage(Canvas canvas, Rect rect, ImageConfiguration configuration)
    {
        if ((((BoxDecoration)this._decoration).image is null))
        {
            return;
        }
        _imagePainter ??= ((BoxDecoration)this._decoration).image!.createPainter(onChanged!);
        global::Doroti.Ui.Path? clipPath__19281 = default!;
        switch (((BoxDecoration)this._decoration).shape)
        {
            case BoxShape.circle:
                {
                    DartRuntimePrimitives.Assert(() => (((BoxDecoration)this._decoration).borderRadius is null));
                    global::Doroti.Ui.Offset center__19549 = rect.center;
                    double radius__19592 = (rect.shortestSide / 2.0);
                    var square__19640 = global::Doroti.Ui.Rect.fromCircle(center: center__19549, radius: radius__19592);
                    clipPath__19281 = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addOval(square__19640);
    return __cascade;
}))();
                    break;
                }
            case BoxShape.rectangle:
                {
                    if ((((BoxDecoration)this._decoration).borderRadius is not null))
                    {
                        clipPath__19281 = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRRect(((BoxDecoration)this._decoration).borderRadius!.resolve(((ImageConfiguration)configuration).textDirection).toRRect(rect));
    return __cascade;
}))();
                    }
                    break;
                }
        }
        this._imagePainter!.paint(canvas, rect, clipPath__19281, configuration);
    }

    public override void dispose()
    {
        this._imagePainter?.dispose();
        base.dispose();
    }

    public override void paint(Canvas canvas, Offset offset, ImageConfiguration configuration)
    {
        DartRuntimePrimitives.Assert(() => (((ImageConfiguration)configuration).size is not null));
        global::Doroti.Ui.Rect rect__20375 = (offset & DartRuntimePrimitives.RequireValue(((ImageConfiguration)configuration).size));
        global::Doroti.Ui.TextDirection? textDirection__20437 = ((ImageConfiguration)configuration).textDirection;
        _paintShadows(canvas, rect__20375, textDirection__20437);
        _paintBackgroundColor(canvas, rect__20375, textDirection__20437);
        _paintBackgroundImage(canvas, rect__20375, configuration);
        ((BoxDecoration)this._decoration).border?.paint(canvas, rect__20375, shape: ((BoxDecoration)this._decoration).shape, borderRadius: ((BoxDecoration)this._decoration).borderRadius?.resolve(textDirection__20437), textDirection: ((ImageConfiguration)configuration).textDirection);
    }

    public override string ToString()
    {
        return $"BoxPainter for {this._decoration}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

