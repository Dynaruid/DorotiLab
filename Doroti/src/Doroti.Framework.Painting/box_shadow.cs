// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/box_shadow.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public class BoxShadow : Shadow
{
    public virtual double spreadRadius { get; private set; } = default!;
    public virtual BlurStyle blurStyle { get; private set; } = default!;

    public BoxShadow(Color color = default!, Offset offset = default, double blurRadius = 0.0, double spreadRadius = 0.0, BlurStyle blurStyle = BlurStyle.normal) : base(color: color ?? new Color(0xFF000000L), offset: offset, blurRadius: blurRadius)
    {
        this.spreadRadius = spreadRadius;
        this.blurStyle = blurStyle;
    }

    public new virtual global::Doroti.Ui.Paint toPaint()
    {
        var result = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = color;
    __cascade.maskFilter = MaskFilter.blur(this.blurStyle, blurSigma);
    return __cascade;
}))();
        DartRuntimePrimitives.Assert(() =>
            {
                if (DebugLibrary.debugDisableShadows)
                {
                    result.maskFilter = null;
                }
                return true;
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxShadow scale(double factor)
    {
        return new BoxShadow(color: color, offset: (DartRuntimePrimitives.RequireValue(offset) * factor), blurRadius: (DartRuntimePrimitives.RequireValue(blurRadius) * factor), spreadRadius: (this.spreadRadius * factor), blurStyle: this.blurStyle);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BoxShadow copyWith(Color? color = null, Offset? offset = null, double? blurRadius = null, double? spreadRadius = null, BlurStyle? blurStyle = null)
    {
        return new BoxShadow(color: (color ?? this.color), offset: (offset ?? this.offset), blurRadius: (blurRadius ?? this.blurRadius), spreadRadius: (spreadRadius ?? this.spreadRadius), blurStyle: (blurStyle ?? this.blurStyle));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BoxShadow? lerp(BoxShadow? a, BoxShadow? b, double t)
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
        return new BoxShadow(color: Dart_uiLibrary.Color.lerp(a.color, b.color, t)!, offset: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(a.offset, b.offset, t)), blurRadius: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a.blurRadius, b.blurRadius, t)), spreadRadius: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((BoxShadow)a).spreadRadius, ((BoxShadow)b).spreadRadius, t)), blurStyle: ((Equals(((BoxShadow)a).blurStyle, BlurStyle.normal)) ? ((BoxShadow)b).blurStyle : ((BoxShadow)a).blurStyle));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static List<BoxShadow>? lerpList(List<BoxShadow>? a, List<BoxShadow>? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        a ??= new List<BoxShadow>();
        b ??= new List<BoxShadow>();
        long commonLength = Math.Min(checked((long)(a.Count)), checked((long)(b.Count)));
        return new List<BoxShadow>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as BoxShadow;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((__other is BoxShadow) && (Equals(((BoxShadow)__other).color, color))) && (Equals(((BoxShadow)__other).offset, DartRuntimePrimitives.RequireValue(offset)))) && (((BoxShadow)__other).blurRadius == DartRuntimePrimitives.RequireValue(blurRadius))) && (((BoxShadow)((BoxShadow)__other)).spreadRadius == this.spreadRadius)) && (Equals(((BoxShadow)((BoxShadow)__other)).blurStyle, this.blurStyle)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(color, DartRuntimePrimitives.RequireValue(offset), DartRuntimePrimitives.RequireValue(blurRadius), this.spreadRadius, this.blurStyle);
    public override string ToString() => $"BoxShadow({color}, {DartRuntimePrimitives.RequireValue(offset)}, {(Foundation.DebugLibrary.debugFormatDouble(DartRuntimePrimitives.RequireValue(blurRadius)))}, {(Foundation.DebugLibrary.debugFormatDouble(this.spreadRadius))}, {this.blurStyle})";
}

