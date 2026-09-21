// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/box_shadow.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public class BoxShadow : Shadow
{
    public virtual double spreadRadius { get; private set; } = default!;
    public virtual BlurStyle blurStyle { get; private set; } = default!;

    public BoxShadow(
        Color color = default!,
        Offset offset = default,
        double blurRadius = 0.0,
        double spreadRadius = 0.0,
        BlurStyle blurStyle = BlurStyle.normal
    )
        : base(color: color ?? new Color(0xFF000000L), offset: offset, blurRadius: blurRadius)
    {
        this.spreadRadius = spreadRadius;
        this.blurStyle = blurStyle;
    }

    public new virtual Paint toPaint()
    {
        var result = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = color;
                    __cascade.maskFilter = MaskFilter.blur(blurStyle, blurSigma);
                    return __cascade;
                }
            )
        )();
        DartRuntimePrimitives.Assert(() =>
        {
            if (DebugLibrary.debugDisableShadows)
            {
                result.maskFilter = null;
            }
            return true;
        });
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual BoxShadow scale(double factor)
    {
        return new BoxShadow(
            color: color,
            offset: (offset) * factor,
            blurRadius: (blurRadius) * factor,
            spreadRadius: spreadRadius * factor,
            blurStyle: blurStyle
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual BoxShadow copyWith(
        Color? color = null,
        Offset? offset = null,
        double? blurRadius = null,
        double? spreadRadius = null,
        BlurStyle? blurStyle = null
    )
    {
        return new BoxShadow(
            color: color ?? this.color,
            offset: offset ?? this.offset,
            blurRadius: blurRadius ?? this.blurRadius,
            spreadRadius: spreadRadius ?? this.spreadRadius,
            blurStyle: blurStyle ?? this.blurStyle
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static BoxShadow? lerp(BoxShadow? a, BoxShadow? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if (a is null)
        {
            return b!.scale(t);
        }
        if (b is null)
        {
            return a.scale(1.0 - t);
        }
        return new BoxShadow(
            color: DorotiUiLibrary.Color.lerp(a.color, b.color, t)!,
            offset: (
                DorotiUiLibrary.Offset.lerp(a.offset, b.offset, t)
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            blurRadius: (
                DorotiUiLibrary.lerpDouble(a.blurRadius, b.blurRadius, t)
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            spreadRadius: (
                DorotiUiLibrary.lerpDouble(a.spreadRadius, b.spreadRadius, t)
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            blurStyle: Equals(a.blurStyle, BlurStyle.normal) ? b.blurStyle : a.blurStyle
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static List<BoxShadow>? lerpList(List<BoxShadow>? a, List<BoxShadow>? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        a ??= new List<BoxShadow>();
        b ??= new List<BoxShadow>();
        long commonLength = Math.Min(checked(a.Count), checked((long)b.Count));
        return new List<BoxShadow>();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as BoxShadow;
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
        return (__other is BoxShadow)
            && Equals(__other.color, color)
            && Equals(__other.offset, (offset))
            && (__other.blurRadius == (blurRadius))
            && (__other.spreadRadius == spreadRadius)
            && Equals(__other.blurStyle, blurStyle);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(color, (offset), (blurRadius), spreadRadius, blurStyle);

    public override string ToString() =>
        $"BoxShadow({color}, {(offset)}, {Foundation.DebugLibrary.debugFormatDouble((blurRadius))}, {Foundation.DebugLibrary.debugFormatDouble(spreadRadius)}, {blurStyle})";
}
