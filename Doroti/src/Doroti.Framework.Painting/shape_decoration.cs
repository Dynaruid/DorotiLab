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

namespace Doroti.Framework.Painting;

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
        ShapeBorder shapeLocal = default!;
        switch (((BoxDecoration)source).shape)
        {
            case BoxShape.circle:
                {
                    if ((((BoxDecoration)source).border is not null))
                    {
                        DartRuntimePrimitives.Assert(() => ((BoxDecoration)source).border!.isUniform);
                        shapeLocal = new CircleBorder(side: ((BoxDecoration)source).border!.top);
                    }
                    else
                    {
                        shapeLocal = new CircleBorder();
                    }
                    break;
                }
            case BoxShape.rectangle:
                {
                    if ((((BoxDecoration)source).borderRadius is not null))
                    {
                        DartRuntimePrimitives.Assert(() => ((((BoxDecoration)source).border is null) || ((BoxDecoration)source).border!.isUniform));
                        shapeLocal = new RoundedRectangleBorder(side: (((BoxDecoration)source).border?.top ?? BorderSide.none), borderRadius: ((BoxDecoration)source).borderRadius!);
                    }
                    else
                    {
                        shapeLocal = (((BoxDecoration)source).border ?? new Border());
                    }
                    break;
                }
        }
        return new ShapeDecoration(color: ((BoxDecoration)source).color, image: ((BoxDecoration)source).image, gradient: ((BoxDecoration)source).gradient, shadows: ((BoxDecoration)source).boxShadow, shape: shapeLocal);
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
        Gradient? aGradient = a?.gradient;
        Gradient? bGradient = b?.gradient;
        if ((((aGradient is null) && (bGradient is not null)) && (a?.color is not null)))
        {
            aGradient = bGradient.fromColor(a!.color!);
        }
        else
        {
            if ((((bGradient is null) && (aGradient is not null)) && (b?.color is not null)))
            {
                bGradient = aGradient.fromColor(b!.color!);
            }
        }
        Gradient? gradientLocal = Gradient.lerp(aGradient, bGradient, t);
        return new ShapeDecoration(color: ((gradientLocal is null) ? Dart_uiLibrary.Color.lerp(a?.color, b?.color, t) : null), gradient: gradientLocal, image: DecorationImage.lerp(a?.image, b?.image, t), shadows: BoxShadow.lerpList(a?.shadows, b?.shadows, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t)!);
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
        return ((((((__other is ShapeDecoration) && (object.Equals(((ShapeDecoration)((ShapeDecoration)__other)).color, this.color))) && (object.Equals(((ShapeDecoration)((ShapeDecoration)__other)).gradient, this.gradient))) && (object.Equals(((ShapeDecoration)((ShapeDecoration)__other)).image, this.image))) && global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals<BoxShadow>(((ShapeDecoration)((ShapeDecoration)__other)).shadows, this.shadows)) && (object.Equals(((ShapeDecoration)((ShapeDecoration)__other)).shape, this.shape)));
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
            if ((global::Doroti.Framework.Painting.DebugLibrary.debugDisableShadows && (object.Equals(((BoxShadow)boxShadow).blurStyle, BlurStyle.outer))))
            {
                canvas.save();
                var clipPathLocal = new global::Doroti.Ui.Path();
                clipPathLocal.fillType = PathFillType.evenOdd;
                clipPathLocal.addRect(Rect.largest);
                clipPathLocal.addPath(path, Offset.zero);
                canvas.clipPath(clipPathLocal);
            }
            return true;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool debugHandleDisabledShadowEnd(Canvas canvas, BoxShadow boxShadow)
        {
            if ((global::Doroti.Framework.Painting.DebugLibrary.debugDisableShadows && (object.Equals(((BoxShadow)boxShadow).blurStyle, BlurStyle.outer))))
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
                for (var index = 0L; (index < DartRuntimePrimitives.RequireValue(this._shadowCount)); index += 1L)
                {
                    DartRuntimePrimitives.Assert(() => debugHandleDisabledShadowStart(canvas, ((ShapeDecoration)this._decoration).shadows![(int)(index)], ((ShapeDecoration)this._decoration).shape.getOuterPath(this._shadowBounds[(int)(index)], textDirection: textDirection)));
                    ((ShapeDecoration)this._decoration).shape.paintInterior(canvas, this._shadowBounds[(int)(index)], this._shadowPaints[(int)(index)], textDirection: textDirection);
                    DartRuntimePrimitives.Assert(() => debugHandleDisabledShadowEnd(canvas, ((ShapeDecoration)this._decoration).shadows![(int)(index)]));
                }
            }
            else
            {
                for (var indexLocal = 0L; (indexLocal < DartRuntimePrimitives.RequireValue(this._shadowCount)); indexLocal += 1L)
                {
                    DartRuntimePrimitives.Assert(() => debugHandleDisabledShadowStart(canvas, ((ShapeDecoration)this._decoration).shadows![(int)(indexLocal)], this._shadowPaths[(int)(indexLocal)]));
                    canvas.drawPath(this._shadowPaths[(int)(indexLocal)], this._shadowPaints[(int)(indexLocal)]);
                    DartRuntimePrimitives.Assert(() => debugHandleDisabledShadowEnd(canvas, ((ShapeDecoration)this._decoration).shadows![(int)(indexLocal)]));
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
                global::Doroti.Ui.Rect adjustedRect = _adjustedRectOnOutlinedBorder(rect);
                ((ShapeDecoration)this._decoration).shape.paintInterior(canvas, adjustedRect, this._interiorPaint!, textDirection: textDirection);
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
            BorderSide sideLocal = (((OutlinedBorder?)(object?)((ShapeDecoration)this._decoration).shape)!).side;
            if (((((BorderSide)sideLocal).color.alpha == 255L) && (object.Equals(((BorderSide)sideLocal).style, BorderStyle.solid))))
            {
                return rect.deflate((((BorderSide)sideLocal).strokeInset / 2L));
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
        global::Doroti.Ui.Rect rect = (offset & DartRuntimePrimitives.RequireValue(((ImageConfiguration)configuration).size));
        global::Doroti.Ui.TextDirection? textDirectionLocal = ((ImageConfiguration)configuration).textDirection;
        _precache(rect, textDirectionLocal);
        _paintShadows(canvas, rect, textDirectionLocal);
        _paintInterior(canvas, rect, textDirectionLocal);
        _paintImage(canvas, configuration);
        ((ShapeDecoration)this._decoration).shape.paint(canvas, rect, textDirection: textDirectionLocal);
    }

}
