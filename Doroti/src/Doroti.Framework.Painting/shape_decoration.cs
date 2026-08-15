// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/shape_decoration.dart
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

public class ShapeDecoration : Decoration
{
    public virtual Color? color { get; private set; }
    public virtual Gradient? gradient { get; private set; }
    public virtual DecorationImage? image { get; private set; }
    public virtual List<BoxShadow>? shadows { get; private set; }
    public virtual ShapeBorder shape { get; private set; } = default!;

    public ShapeDecoration(Color? color = null, DecorationImage? image = null, Gradient? gradient = null, List<BoxShadow>? shadows = null, ShapeBorder shape = default!)
    {
        this.color = color;
        this.image = image;
        this.gradient = gradient;
        this.shadows = shadows;
        this.shape = shape;
        System.Diagnostics.Debug.Assert(!(((color is not null) && (gradient is not null))));
    }

    public static ShapeDecoration CreateFromBoxDecoration(BoxDecoration source)
    {
        ShapeBorder shape__3290 = default!;
        switch (((BoxDecoration)source).shape)
        {
            case BoxShape.circle:
                {
                    if ((((BoxDecoration)source).border is not null))
                    {
                        DartRuntimePrimitives.Assert(() => ((BoxDecoration)source).border!.isUniform);
                        shape__3290 = new CircleBorder(side: ((BoxDecoration)source).border!.top);
                    }
                    else
                    {
                        shape__3290 = new CircleBorder();
                    }
                    break;
                }
            case BoxShape.rectangle:
                {
                    if ((((BoxDecoration)source).borderRadius is not null))
                    {
                        DartRuntimePrimitives.Assert(() => ((((BoxDecoration)source).border is null) || ((BoxDecoration)source).border!.isUniform));
                        shape__3290 = new RoundedRectangleBorder(side: (((BoxDecoration)source).border?.top ?? BorderSide.none), borderRadius: ((BoxDecoration)source).borderRadius!);
                    }
                    else
                    {
                        shape__3290 = (((BoxDecoration)source).border ?? new Border());
                    }
                    break;
                }
        }
        return new ShapeDecoration(color: ((BoxDecoration)source).color, image: ((BoxDecoration)source).image, gradient: ((BoxDecoration)source).gradient, shadows: ((BoxDecoration)source).boxShadow, shape: shape__3290);
    }

    public override Path getClipPath(Rect rect, TextDirection textDirection)
    {
        return this.shape.getOuterPath(rect, textDirection: DartRuntimePrimitives.RequireValue(textDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsGeometry padding => ((ShapeBorder)this.shape).dimensions;
    public override bool isComplex => (this.shadows is not null);
    public override ShapeDecoration? lerpFrom(Decoration? a, double t)
    {
        return (a switch { BoxDecoration __object6540 => ShapeDecoration.lerp(ShapeDecoration.CreateFromBoxDecoration(((BoxDecoration)__object6540)), this, t), ShapeDecoration __typed6634 => ShapeDecoration.lerp(((ShapeDecoration?)__typed6634), this, t), _ => ((ShapeDecoration?)(object?)base.lerpFrom(a, t))! });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeDecoration? lerpTo(Decoration? b, double t)
    {
        return (b switch { BoxDecoration __object6850 => ShapeDecoration.lerp(this, ShapeDecoration.CreateFromBoxDecoration(((BoxDecoration)__object6850)), t), ShapeDecoration __typed6944 => ShapeDecoration.lerp(this, ((ShapeDecoration?)__typed6944), t), _ => ((ShapeDecoration?)(object?)base.lerpTo(b, t))! });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ShapeDecoration? lerp(ShapeDecoration? a, ShapeDecoration? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if (((a is not null) && (b is not null)))
        {
            if ((t == 0.0))
            {
                return a;
            }
            if ((t == 1.0))
            {
                return b;
            }
        }
        Gradient? aGradient__8622 = a?.gradient;
        Gradient? bGradient__8661 = b?.gradient;
        if ((((aGradient__8622 is null) && (bGradient__8661 is not null)) && (a?.color is not null)))
        {
            aGradient__8622 = bGradient__8661.fromColor(a!.color!);
        }
        else
        {
            if ((((bGradient__8661 is null) && (aGradient__8622 is not null)) && (b?.color is not null)))
            {
                bGradient__8661 = aGradient__8622.fromColor(b!.color!);
            }
        }
        Gradient? gradient__8959 = Gradient.lerp(aGradient__8622, bGradient__8661, t);
        return new ShapeDecoration(color: ((gradient__8959 is null) ? Dart_uiLibrary.Color.lerp(a?.color, b?.color, t) : null), gradient: gradient__8959, image: DecorationImage.lerp(a?.image, b?.image, t), shadows: BoxShadow.lerpList(a?.shadows, b?.shadows, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t)!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as ShapeDecoration;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((__other is ShapeDecoration) && (object.Equals(((ShapeDecoration)((ShapeDecoration)__other)).color, this.color))) && (object.Equals(((ShapeDecoration)((ShapeDecoration)__other)).gradient, this.gradient))) && (object.Equals(((ShapeDecoration)((ShapeDecoration)__other)).image, this.image))) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals<BoxShadow>(((ShapeDecoration)((ShapeDecoration)__other)).shadows, this.shadows)) && (object.Equals(((ShapeDecoration)((ShapeDecoration)__other)).shape, this.shape)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.color, this.gradient, this.image, this.shape, ((this.shadows is null) ? null : FoundationRuntimePorts.ObjectHashAll(this.shadows!)));
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.defaultDiagnosticsTreeStyle = DiagnosticsTreeStyle.whitespace;
        properties.add(new ColorProperty("color", this.color, defaultValue: null));
        properties.add(new DiagnosticsProperty<Gradient>("gradient", this.gradient, defaultValue: null));
        properties.add(new DiagnosticsProperty<DecorationImage>("image", this.image, defaultValue: null));
        properties.add(new IterableProperty<BoxShadow>("shadows", this.shadows, defaultValue: null, style: DiagnosticsTreeStyle.whitespace));
        properties.add(new DiagnosticsProperty<ShapeBorder>("shape", this.shape));
    }

    public override bool hitTest(Size size, Offset position, TextDirection? textDirection = null)
    {
        return this.shape.hitTest((Offset.zero & size), position, textDirection: textDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BoxPainter createBoxPainter(Action onChanged = default!)
    {
        DartRuntimePrimitives.Assert(() => ((onChanged is not null) || (this.image is null)));
        return new _ShapeDecorationPainter__shape_decoration(this, onChanged!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ShapeDecorationPainter__shape_decoration : BoxPainter
{
    internal virtual ShapeDecoration _decoration { get; private set; } = default!;
    internal virtual Rect? _lastRect { get; set; } = default;
    internal virtual TextDirection? _lastTextDirection { get; set; } = default;
    internal virtual Path _outerPath { get; set; } = default!;
    internal virtual Path? _innerPath { get; set; } = default;
    internal virtual Paint? _interiorPaint { get; set; } = default;
    internal virtual long? _shadowCount { get; set; } = default;
    internal virtual List<Rect> _shadowBounds { get; set; } = default!;
    internal virtual List<Path> _shadowPaths { get; set; } = default!;
    internal virtual List<Paint> _shadowPaints { get; set; } = default!;
    internal virtual DecorationImagePainter? _imagePainter { get; set; } = default;

    internal _ShapeDecorationPainter__shape_decoration(ShapeDecoration _decoration, Action onChanged) : base(onChanged)
    {
        this._decoration = _decoration;
    }

    public override Action? onChanged => base.onChanged!;
    internal virtual void _precache(Rect rect, TextDirection? textDirection)
    {
        if (((object.Equals(rect, this._lastRect)) && (object.Equals(textDirection, this._lastTextDirection))))
        {
            return;
        }
        if (((this._interiorPaint is null) && (((((ShapeDecoration)this._decoration).color is not null) || (((ShapeDecoration)this._decoration).gradient is not null)))))
        {
            _interiorPaint = new global::Doroti.Ui.Paint();
            if ((((ShapeDecoration)this._decoration).color is not null))
            {
                this._interiorPaint!.color = ((ShapeDecoration)this._decoration).color!;
            }
        }
        if ((((ShapeDecoration)this._decoration).gradient is not null))
        {
            this._interiorPaint!.shader = ((ShapeDecoration)this._decoration).gradient!.createShader(rect, textDirection: textDirection);
        }
        if ((((ShapeDecoration)this._decoration).shadows is not null))
        {
            if ((this._shadowCount is null))
            {
                _shadowCount = checked((long)(((ShapeDecoration)this._decoration).shadows!.Count));
                _shadowPaints = ((ShapeDecoration)this._decoration).shadows!
                    .Select(shadow => shadow.toPaint())
                    .ToList();
            }
            if (((ShapeDecoration)this._decoration).shape.preferPaintInterior)
            {
                _shadowBounds = ((ShapeDecoration)this._decoration).shadows!
                    .Select(shadow => rect.shift(shadow.offset).inflate(shadow.spreadRadius))
                    .ToList();
            }
            else
            {
                _shadowPaths = ((ShapeDecoration)this._decoration).shadows!
                    .Select(shadow => ((ShapeDecoration)this._decoration).shape.getOuterPath(
                        rect.shift(shadow.offset).inflate(shadow.spreadRadius),
                        textDirection: textDirection))
                    .ToList();
            }
        }
        if ((!((ShapeDecoration)this._decoration).shape.preferPaintInterior && (((this._interiorPaint is not null) || (this._shadowCount is not null)))))
        {
            _outerPath = ((ShapeDecoration)this._decoration).shape.getOuterPath(rect, textDirection: textDirection);
        }
        if ((((ShapeDecoration)this._decoration).image is not null))
        {
            _innerPath = ((ShapeDecoration)this._decoration).shape.getInnerPath(rect, textDirection: textDirection);
        }
        _lastRect = rect;
        _lastTextDirection = textDirection;
    }

    internal virtual void _paintShadows(Canvas canvas, Rect rect, TextDirection? textDirection)
    {
        bool debugHandleDisabledShadowStart(Canvas canvas, BoxShadow boxShadow, Path path)
        {
            if ((global::Doroti.Generated.Framework.Painting.DebugLibrary.debugDisableShadows && (object.Equals(((BoxShadow)boxShadow).blurStyle, BlurStyle.outer))))
            {
                canvas.save();
                var clipPath__14580 = new global::Doroti.Ui.Path();
                clipPath__14580.fillType = PathFillType.evenOdd;
                clipPath__14580.addRect(Rect.largest);
                clipPath__14580.addPath(path, Offset.zero);
                canvas.clipPath(clipPath__14580);
            }
            return true;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool debugHandleDisabledShadowEnd(Canvas canvas, BoxShadow boxShadow)
        {
            if ((global::Doroti.Generated.Framework.Painting.DebugLibrary.debugDisableShadows && (object.Equals(((BoxShadow)boxShadow).blurStyle, BlurStyle.outer))))
            {
                canvas.restore();
            }
            return true;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        if ((this._shadowCount is not null))
        {
            if (((ShapeDecoration)this._decoration).shape.preferPaintInterior)
            {
                for (var index__15114 = 0L; (index__15114 < DartRuntimePrimitives.RequireValue(this._shadowCount)); index__15114 += 1L)
                {
                    DartRuntimePrimitives.Assert(() => debugHandleDisabledShadowStart(canvas, ((ShapeDecoration)this._decoration).shadows![(int)(index__15114)], ((ShapeDecoration)this._decoration).shape.getOuterPath(this._shadowBounds[(int)(index__15114)], textDirection: textDirection)));
                    ((ShapeDecoration)this._decoration).shape.paintInterior(canvas, this._shadowBounds[(int)(index__15114)], this._shadowPaints[(int)(index__15114)], textDirection: textDirection);
                    DartRuntimePrimitives.Assert(() => debugHandleDisabledShadowEnd(canvas, ((ShapeDecoration)this._decoration).shadows![(int)(index__15114)]));
                }
            }
            else
            {
                for (var index__15728 = 0L; (index__15728 < DartRuntimePrimitives.RequireValue(this._shadowCount)); index__15728 += 1L)
                {
                    DartRuntimePrimitives.Assert(() => debugHandleDisabledShadowStart(canvas, ((ShapeDecoration)this._decoration).shadows![(int)(index__15728)], this._shadowPaths[(int)(index__15728)]));
                    canvas.drawPath(this._shadowPaths[(int)(index__15728)], this._shadowPaints[(int)(index__15728)]);
                    DartRuntimePrimitives.Assert(() => debugHandleDisabledShadowEnd(canvas, ((ShapeDecoration)this._decoration).shadows![(int)(index__15728)]));
                }
            }
        }
    }

    internal virtual void _paintInterior(Canvas canvas, Rect rect, TextDirection? textDirection)
    {
        if ((this._interiorPaint is not null))
        {
            if (((ShapeDecoration)this._decoration).shape.preferPaintInterior)
            {
                global::Doroti.Ui.Rect adjustedRect__16492 = _adjustedRectOnOutlinedBorder(rect);
                ((ShapeDecoration)this._decoration).shape.paintInterior(canvas, adjustedRect__16492, this._interiorPaint!, textDirection: textDirection);
            }
            else
            {
                canvas.drawPath(this._outerPath, this._interiorPaint!);
            }
        }
    }

    internal virtual global::Doroti.Ui.Rect _adjustedRectOnOutlinedBorder(Rect rect)
    {
        if (((((ShapeDecoration)this._decoration).shape is OutlinedBorder) && (((ShapeDecoration)this._decoration).color is not null)))
        {
            BorderSide side__16942 = (((OutlinedBorder?)(object?)((ShapeDecoration)this._decoration).shape)!).side;
            if (((((BorderSide)side__16942).color.alpha == 255L) && (object.Equals(((BorderSide)side__16942).style, BorderStyle.solid))))
            {
                return rect.deflate((((BorderSide)side__16942).strokeInset / 2L));
            }
        }
        return rect;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paintImage(Canvas canvas, ImageConfiguration configuration)
    {
        if ((((ShapeDecoration)this._decoration).image is null))
        {
            return;
        }
        _imagePainter ??= ((ShapeDecoration)this._decoration).image!.createPainter((Action)this.onChanged);
        this._imagePainter!.paint(canvas, DartRuntimePrimitives.RequireValue(this._lastRect), this._innerPath, configuration);
    }

    public override void dispose()
    {
        this._imagePainter?.dispose();
        base.dispose();
    }

    public override void paint(Canvas canvas, Offset offset, ImageConfiguration configuration)
    {
        DartRuntimePrimitives.Assert(() => (((ImageConfiguration)configuration).size is not null));
        global::Doroti.Ui.Rect rect__17698 = (offset & DartRuntimePrimitives.RequireValue(((ImageConfiguration)configuration).size));
        global::Doroti.Ui.TextDirection? textDirection__17760 = ((ImageConfiguration)configuration).textDirection;
        _precache(rect__17698, textDirection__17760);
        _paintShadows(canvas, rect__17698, textDirection__17760);
        _paintInterior(canvas, rect__17698, textDirection__17760);
        _paintImage(canvas, configuration);
        ((ShapeDecoration)this._decoration).shape.paint(canvas, rect__17698, textDirection: textDirection__17760);
    }

}
