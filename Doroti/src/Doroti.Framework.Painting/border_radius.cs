// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/border_radius.dart
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

public abstract class BorderRadiusGeometry
{
    public static BorderRadiusGeometry zero = BorderRadius.zero;

    protected BorderRadiusGeometry()
    {
    }

    public static BorderRadiusGeometry CreateAll(Radius radius)
        => BorderRadius.CreateAll(radius);

    public static BorderRadiusGeometry CreateCircular(double radius)
        => BorderRadius.CreateCircular(radius);

    public static BorderRadiusGeometry CreateHorizontal(Radius? left = null, Radius? right = null, Radius? start = null, Radius? end = null)
    {
        DartRuntimePrimitives.Assert(() => ((((left is null) && (right is null))) || (((start is null) && (end is null)))));
        if (((start is not null) || (end is not null)))
        {
            return BorderRadiusDirectional.CreateHorizontal(start: (start ?? Radius.zero), end: (end ?? Radius.zero));
        }
        return BorderRadius.CreateHorizontal(left: (left ?? Radius.zero), right: (right ?? Radius.zero));
    }

    public static BorderRadiusGeometry CreateOnly(Radius topLeft = default!, Radius topRight = default!, Radius bottomLeft = default!, Radius bottomRight = default!)
        => new BorderRadius(topLeft, topRight, bottomLeft, bottomRight);

    public static BorderRadiusGeometry CreateDirectional(Radius topStart = default!, Radius topEnd = default!, Radius bottomStart = default!, Radius bottomEnd = default!)
        => new BorderRadiusDirectional(topStart, topEnd, bottomStart, bottomEnd);

    public static BorderRadiusGeometry CreateVertical(Radius top = default!, Radius bottom = default!)
        => BorderRadius.CreateVertical(top, bottom);

    internal abstract global::Doroti.Ui.Radius _topLeft { get; }
    internal abstract global::Doroti.Ui.Radius _topRight { get; }
    internal abstract global::Doroti.Ui.Radius _bottomLeft { get; }
    internal abstract global::Doroti.Ui.Radius _bottomRight { get; }
    internal abstract global::Doroti.Ui.Radius _topStart { get; }
    internal abstract global::Doroti.Ui.Radius _topEnd { get; }
    internal abstract global::Doroti.Ui.Radius _bottomStart { get; }
    internal abstract global::Doroti.Ui.Radius _bottomEnd { get; }
    public virtual BorderRadiusGeometry subtract(BorderRadiusGeometry other)
    {
        return new _MixedBorderRadius__border_radius((this._topLeft - ((BorderRadiusGeometry)other)._topLeft), (this._topRight - ((BorderRadiusGeometry)other)._topRight), (this._bottomLeft - ((BorderRadiusGeometry)other)._bottomLeft), (this._bottomRight - ((BorderRadiusGeometry)other)._bottomRight), (this._topStart - ((BorderRadiusGeometry)other)._topStart), (this._topEnd - ((BorderRadiusGeometry)other)._topEnd), (this._bottomStart - ((BorderRadiusGeometry)other)._bottomStart), (this._bottomEnd - ((BorderRadiusGeometry)other)._bottomEnd));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BorderRadiusGeometry add(BorderRadiusGeometry other)
    {
        return new _MixedBorderRadius__border_radius((this._topLeft + ((BorderRadiusGeometry)other)._topLeft), (this._topRight + ((BorderRadiusGeometry)other)._topRight), (this._bottomLeft + ((BorderRadiusGeometry)other)._bottomLeft), (this._bottomRight + ((BorderRadiusGeometry)other)._bottomRight), (this._topStart + ((BorderRadiusGeometry)other)._topStart), (this._topEnd + ((BorderRadiusGeometry)other)._topEnd), (this._bottomStart + ((BorderRadiusGeometry)other)._bottomStart), (this._bottomEnd + ((BorderRadiusGeometry)other)._bottomEnd));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract BorderRadiusGeometry op_Subtract();
    public abstract BorderRadiusGeometry op_Multiply(double other);
    public abstract BorderRadiusGeometry op_Divide(double other);
    public abstract BorderRadiusGeometry ___(double other);
    public abstract BorderRadiusGeometry __(double other);
    public static BorderRadiusGeometry? lerp(BorderRadiusGeometry? a, BorderRadiusGeometry? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        a ??= BorderRadius.zero;
        b ??= BorderRadius.zero;
        return a.add(((b.subtract(a)).op_Multiply(t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract BorderRadius resolve(TextDirection? direction);
    public override string ToString()
    {
        string? visual = default!;
        string? logical = default!;
        if ((((object.Equals(this._topLeft, this._topRight)) && (object.Equals(this._topRight, this._bottomLeft))) && (object.Equals(this._bottomLeft, this._bottomRight))))
        {
            if ((!object.Equals(this._topLeft, Radius.zero)))
            {
                if ((this._topLeft.x == this._topLeft.y))
                {
                    visual = $"BorderRadius.circular({this._topLeft.x.toStringAsFixed(1L)})";
                }
                else
                {
                    visual = $"BorderRadius.all({this._topLeft})";
                }
            }
        }
        else
        {
            var result = new StringBuffer();
            result.write("BorderRadius.only(");
            var comma = false;
            if ((!object.Equals(this._topLeft, Radius.zero)))
            {
                result.write($"topLeft: {this._topLeft}");
                comma = true;
            }
            if ((!object.Equals(this._topRight, Radius.zero)))
            {
                if (comma)
                {
                    result.write(", ");
                }
                result.write($"topRight: {this._topRight}");
                comma = true;
            }
            if ((!object.Equals(this._bottomLeft, Radius.zero)))
            {
                if (comma)
                {
                    result.write(", ");
                }
                result.write($"bottomLeft: {this._bottomLeft}");
                comma = true;
            }
            if ((!object.Equals(this._bottomRight, Radius.zero)))
            {
                if (comma)
                {
                    result.write(", ");
                }
                result.write($"bottomRight: {this._bottomRight}");
            }
            result.write(")");
            visual = result.ToString();
        }
        if ((((object.Equals(this._topStart, this._topEnd)) && (object.Equals(this._topEnd, this._bottomEnd))) && (object.Equals(this._bottomEnd, this._bottomStart))))
        {
            if ((!object.Equals(this._topStart, Radius.zero)))
            {
                if ((this._topStart.x == this._topStart.y))
                {
                    logical = $"BorderRadiusDirectional.circular({this._topStart.x.toStringAsFixed(1L)})";
                }
                else
                {
                    logical = $"BorderRadiusDirectional.all({this._topStart})";
                }
            }
        }
        else
        {
            var resultLocal = new StringBuffer();
            resultLocal.write("BorderRadiusDirectional.only(");
            var commaLocal = false;
            if ((!object.Equals(this._topStart, Radius.zero)))
            {
                resultLocal.write($"topStart: {this._topStart}");
                commaLocal = true;
            }
            if ((!object.Equals(this._topEnd, Radius.zero)))
            {
                if (commaLocal)
                {
                    resultLocal.write(", ");
                }
                resultLocal.write($"topEnd: {this._topEnd}");
                commaLocal = true;
            }
            if ((!object.Equals(this._bottomStart, Radius.zero)))
            {
                if (commaLocal)
                {
                    resultLocal.write(", ");
                }
                resultLocal.write($"bottomStart: {this._bottomStart}");
                commaLocal = true;
            }
            if ((!object.Equals(this._bottomEnd, Radius.zero)))
            {
                if (commaLocal)
                {
                    resultLocal.write(", ");
                }
                resultLocal.write($"bottomEnd: {this._bottomEnd}");
            }
            resultLocal.write(")");
            logical = resultLocal.ToString();
        }
        if (((visual is not null) && (logical is not null)))
        {
            return $"{visual} + {logical}";
        }
        return ((visual ?? logical) ?? "BorderRadius.zero");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as BorderRadiusGeometry;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((__other is BorderRadiusGeometry) && (object.Equals(((BorderRadiusGeometry)((BorderRadiusGeometry)__other))._topLeft, this._topLeft))) && (object.Equals(((BorderRadiusGeometry)((BorderRadiusGeometry)__other))._topRight, this._topRight))) && (object.Equals(((BorderRadiusGeometry)((BorderRadiusGeometry)__other))._bottomLeft, this._bottomLeft))) && (object.Equals(((BorderRadiusGeometry)((BorderRadiusGeometry)__other))._bottomRight, this._bottomRight))) && (object.Equals(((BorderRadiusGeometry)((BorderRadiusGeometry)__other))._topStart, this._topStart))) && (object.Equals(((BorderRadiusGeometry)((BorderRadiusGeometry)__other))._topEnd, this._topEnd))) && (object.Equals(((BorderRadiusGeometry)((BorderRadiusGeometry)__other))._bottomStart, this._bottomStart))) && (object.Equals(((BorderRadiusGeometry)((BorderRadiusGeometry)__other))._bottomEnd, this._bottomEnd)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this._topLeft, this._topRight, this._bottomLeft, this._bottomRight, this._topStart, this._topEnd, this._bottomStart, this._bottomEnd);
}

public class BorderRadius : BorderRadiusGeometry
{
    public static BorderRadius zero = BorderRadius.CreateAll(Radius.zero);
    public virtual Radius topLeft { get; private set; } = default!;
    public virtual Radius topRight { get; private set; } = default!;
    public virtual Radius bottomLeft { get; private set; } = default!;
    public virtual Radius bottomRight { get; private set; } = default!;

    public static BorderRadius CreateAll(Radius radius)
    {
        return new BorderRadius(topLeft: radius, topRight: radius, bottomLeft: radius, bottomRight: radius);
    }

    public static BorderRadius CreateCircular(double radius)
    {
        return BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(radius));
    }

    public static BorderRadius CreateVertical(Radius top = default, Radius bottom = default)
    {
        return new BorderRadius(topLeft: top, topRight: top, bottomLeft: bottom, bottomRight: bottom);
    }

    public static BorderRadius CreateHorizontal(Radius left = default, Radius right = default)
    {
        return new BorderRadius(topLeft: left, topRight: right, bottomLeft: left, bottomRight: right);
    }

    public BorderRadius(Radius topLeft = default, Radius topRight = default, Radius bottomLeft = default, Radius bottomRight = default)
    {
        this.topLeft = topLeft;
        this.topRight = topRight;
        this.bottomLeft = bottomLeft;
        this.bottomRight = bottomRight;
    }

    public virtual BorderRadius copyWith(Radius? topLeft = null, Radius? topRight = null, Radius? bottomLeft = null, Radius? bottomRight = null)
    {
        return new BorderRadius(topLeft: (topLeft ?? this.topLeft), topRight: (topRight ?? this.topRight), bottomLeft: (bottomLeft ?? this.bottomLeft), bottomRight: (bottomRight ?? this.bottomRight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override Radius _topLeft => this.topLeft;
    internal override Radius _topRight => this.topRight;
    internal override Radius _bottomLeft => this.bottomLeft;
    internal override Radius _bottomRight => this.bottomRight;
    internal override Radius _topStart => Radius.zero;
    internal override Radius _topEnd => Radius.zero;
    internal override Radius _bottomStart => Radius.zero;
    internal override Radius _bottomEnd => Radius.zero;
    public virtual global::Doroti.Ui.RRect toRRect(Rect rect)
    {
        return global::Doroti.Ui.RRect.fromRectAndCorners(rect, topLeft: this.topLeft.clamp(minimum: Radius.zero), topRight: this.topRight.clamp(minimum: Radius.zero), bottomLeft: this.bottomLeft.clamp(minimum: Radius.zero), bottomRight: this.bottomRight.clamp(minimum: Radius.zero));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.RSuperellipse toRSuperellipse(Rect rect)
    {
        return global::Doroti.Ui.RSuperellipse.fromRectAndCorners(rect, topLeft: this.topLeft.clamp(minimum: Radius.zero), topRight: this.topRight.clamp(minimum: Radius.zero), bottomLeft: this.bottomLeft.clamp(minimum: Radius.zero), bottomRight: this.bottomRight.clamp(minimum: Radius.zero));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadiusGeometry subtract(BorderRadiusGeometry other)
    {
        if ((other is BorderRadius))
        {
            BorderRadius other__as16879 = (BorderRadius)other;
            return (this.op_Subtract(((BorderRadius)other__as16879)));
        }
        return base.subtract(other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadiusGeometry add(BorderRadiusGeometry other)
    {
        if ((other is BorderRadius))
        {
            BorderRadius other__as17053 = (BorderRadius)other;
            return (this.op_Add(((BorderRadius)other__as17053)));
        }
        return base.add(other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BorderRadius op_Subtract(BorderRadius other)
    {
        return new BorderRadius(topLeft: (this.topLeft - ((BorderRadius)other).topLeft), topRight: (this.topRight - ((BorderRadius)other).topRight), bottomLeft: (this.bottomLeft - ((BorderRadius)other).bottomLeft), bottomRight: (this.bottomRight - ((BorderRadius)other).bottomRight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BorderRadius op_Add(BorderRadius other)
    {
        return new BorderRadius(topLeft: (this.topLeft + ((BorderRadius)other).topLeft), topRight: (this.topRight + ((BorderRadius)other).topRight), bottomLeft: (this.bottomLeft + ((BorderRadius)other).bottomLeft), bottomRight: (this.bottomRight + ((BorderRadius)other).bottomRight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadius op_Subtract()
    {
        return new BorderRadius(topLeft: -this.topLeft, topRight: -this.topRight, bottomLeft: -this.bottomLeft, bottomRight: -this.bottomRight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadius op_Multiply(double other)
    {
        return new BorderRadius(topLeft: (this.topLeft * other), topRight: (this.topRight * other), bottomLeft: (this.bottomLeft * other), bottomRight: (this.bottomRight * other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadius op_Divide(double other)
    {
        return new BorderRadius(topLeft: (this.topLeft / other), topRight: (this.topRight / other), bottomLeft: (this.bottomLeft / other), bottomRight: (this.bottomRight / other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadius ___(double other)
    {
        return new BorderRadius(topLeft: (this.topLeft.___(other)), topRight: (this.topRight.___(other)), bottomLeft: (this.bottomLeft.___(other)), bottomRight: (this.bottomRight.___(other)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadius __(double other)
    {
        return new BorderRadius(topLeft: (this.topLeft % other), topRight: (this.topRight % other), bottomLeft: (this.bottomLeft % other), bottomRight: (this.bottomRight % other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BorderRadius? lerp(BorderRadius? a, BorderRadius? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if ((a is null))
        {
            return (b!.op_Multiply(t));
        }
        if ((b is null))
        {
            return (a.op_Multiply(((1.0 - t))));
        }
        return new BorderRadius(topLeft: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Radius.lerp(((BorderRadius)a).topLeft, ((BorderRadius)b).topLeft, t)), topRight: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Radius.lerp(((BorderRadius)a).topRight, ((BorderRadius)b).topRight, t)), bottomLeft: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Radius.lerp(((BorderRadius)a).bottomLeft, ((BorderRadius)b).bottomLeft, t)), bottomRight: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Radius.lerp(((BorderRadius)a).bottomRight, ((BorderRadius)b).bottomRight, t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadius resolve(TextDirection? direction) => this;
}

public class BorderRadiusDirectional : BorderRadiusGeometry
{
    public static BorderRadiusDirectional zero = BorderRadiusDirectional.CreateAll(Radius.zero);
    public virtual Radius topStart { get; private set; } = default!;
    public virtual Radius topEnd { get; private set; } = default!;
    public virtual Radius bottomStart { get; private set; } = default!;
    public virtual Radius bottomEnd { get; private set; } = default!;

    public static BorderRadiusDirectional CreateAll(Radius radius)
    {
        return new BorderRadiusDirectional(topStart: radius, topEnd: radius, bottomStart: radius, bottomEnd: radius);
    }

    public static BorderRadiusDirectional CreateCircular(double radius)
    {
        return BorderRadiusDirectional.CreateAll(global::Doroti.Ui.Radius.circular(radius));
    }

    public static BorderRadiusDirectional CreateVertical(Radius top = default, Radius bottom = default)
    {
        return new BorderRadiusDirectional(topStart: top, topEnd: top, bottomStart: bottom, bottomEnd: bottom);
    }

    public static BorderRadiusDirectional CreateHorizontal(Radius start = default, Radius end = default)
    {
        return new BorderRadiusDirectional(topStart: start, topEnd: end, bottomStart: start, bottomEnd: end);
    }

    public BorderRadiusDirectional(Radius topStart = default, Radius topEnd = default, Radius bottomStart = default, Radius bottomEnd = default)
    {
        this.topStart = topStart;
        this.topEnd = topEnd;
        this.bottomStart = bottomStart;
        this.bottomEnd = bottomEnd;
    }

    internal override Radius _topStart => this.topStart;
    internal override Radius _topEnd => this.topEnd;
    internal override Radius _bottomStart => this.bottomStart;
    internal override Radius _bottomEnd => this.bottomEnd;
    internal override Radius _topLeft => Radius.zero;
    internal override Radius _topRight => Radius.zero;
    internal override Radius _bottomLeft => Radius.zero;
    internal override Radius _bottomRight => Radius.zero;
    public override BorderRadiusGeometry subtract(BorderRadiusGeometry other)
    {
        if ((other is BorderRadiusDirectional))
        {
            BorderRadiusDirectional other__as23144 = (BorderRadiusDirectional)other;
            return (this.op_Subtract(((BorderRadiusDirectional)other__as23144)));
        }
        return base.subtract(other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadiusGeometry add(BorderRadiusGeometry other)
    {
        if ((other is BorderRadiusDirectional))
        {
            BorderRadiusDirectional other__as23329 = (BorderRadiusDirectional)other;
            return (this.op_Add(((BorderRadiusDirectional)other__as23329)));
        }
        return base.add(other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BorderRadiusDirectional op_Subtract(BorderRadiusDirectional other)
    {
        return new BorderRadiusDirectional(topStart: (this.topStart - ((BorderRadiusDirectional)other).topStart), topEnd: (this.topEnd - ((BorderRadiusDirectional)other).topEnd), bottomStart: (this.bottomStart - ((BorderRadiusDirectional)other).bottomStart), bottomEnd: (this.bottomEnd - ((BorderRadiusDirectional)other).bottomEnd));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BorderRadiusDirectional op_Add(BorderRadiusDirectional other)
    {
        return new BorderRadiusDirectional(topStart: (this.topStart + ((BorderRadiusDirectional)other).topStart), topEnd: (this.topEnd + ((BorderRadiusDirectional)other).topEnd), bottomStart: (this.bottomStart + ((BorderRadiusDirectional)other).bottomStart), bottomEnd: (this.bottomEnd + ((BorderRadiusDirectional)other).bottomEnd));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadiusDirectional op_Subtract()
    {
        return new BorderRadiusDirectional(topStart: -this.topStart, topEnd: -this.topEnd, bottomStart: -this.bottomStart, bottomEnd: -this.bottomEnd);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadiusDirectional op_Multiply(double other)
    {
        return new BorderRadiusDirectional(topStart: (this.topStart * other), topEnd: (this.topEnd * other), bottomStart: (this.bottomStart * other), bottomEnd: (this.bottomEnd * other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadiusDirectional op_Divide(double other)
    {
        return new BorderRadiusDirectional(topStart: (this.topStart / other), topEnd: (this.topEnd / other), bottomStart: (this.bottomStart / other), bottomEnd: (this.bottomEnd / other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadiusDirectional ___(double other)
    {
        return new BorderRadiusDirectional(topStart: (this.topStart.___(other)), topEnd: (this.topEnd.___(other)), bottomStart: (this.bottomStart.___(other)), bottomEnd: (this.bottomEnd.___(other)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadiusDirectional __(double other)
    {
        return new BorderRadiusDirectional(topStart: (this.topStart % other), topEnd: (this.topEnd % other), bottomStart: (this.bottomStart % other), bottomEnd: (this.bottomEnd % other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BorderRadiusDirectional? lerp(BorderRadiusDirectional? a, BorderRadiusDirectional? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if ((a is null))
        {
            return (b!.op_Multiply(t));
        }
        if ((b is null))
        {
            return (a.op_Multiply(((1.0 - t))));
        }
        return new BorderRadiusDirectional(topStart: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Radius.lerp(((BorderRadiusDirectional)a).topStart, ((BorderRadiusDirectional)b).topStart, t)), topEnd: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Radius.lerp(((BorderRadiusDirectional)a).topEnd, ((BorderRadiusDirectional)b).topEnd, t)), bottomStart: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Radius.lerp(((BorderRadiusDirectional)a).bottomStart, ((BorderRadiusDirectional)b).bottomStart, t)), bottomEnd: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Radius.lerp(((BorderRadiusDirectional)a).bottomEnd, ((BorderRadiusDirectional)b).bottomEnd, t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadius resolve(TextDirection? direction)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Painting.DebugLibrary.debugCheckCanResolveTextDirection(direction, $"{typeof(BorderRadiusDirectional)}"));
        switch (DartRuntimePrimitives.RequireValue(direction))
        {
            case TextDirection.rtl:
                {
                    return new BorderRadius(topLeft: this.topEnd, topRight: this.topStart, bottomLeft: this.bottomEnd, bottomRight: this.bottomStart);
                }
            case TextDirection.ltr:
                {
                    return new BorderRadius(topLeft: this.topStart, topRight: this.topEnd, bottomLeft: this.bottomStart, bottomRight: this.bottomEnd);
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MixedBorderRadius__border_radius : BorderRadiusGeometry
{
    private Radius __field__topLeft = default!;
    internal override Radius _topLeft { get => __field__topLeft; }
    private Radius __field__topRight = default!;
    internal override Radius _topRight { get => __field__topRight; }
    private Radius __field__bottomLeft = default!;
    internal override Radius _bottomLeft { get => __field__bottomLeft; }
    private Radius __field__bottomRight = default!;
    internal override Radius _bottomRight { get => __field__bottomRight; }
    private Radius __field__topStart = default!;
    internal override Radius _topStart { get => __field__topStart; }
    private Radius __field__topEnd = default!;
    internal override Radius _topEnd { get => __field__topEnd; }
    private Radius __field__bottomStart = default!;
    internal override Radius _bottomStart { get => __field__bottomStart; }
    private Radius __field__bottomEnd = default!;
    internal override Radius _bottomEnd { get => __field__bottomEnd; }

    internal _MixedBorderRadius__border_radius(Radius _topLeft, Radius _topRight, Radius _bottomLeft, Radius _bottomRight, Radius _topStart, Radius _topEnd, Radius _bottomStart, Radius _bottomEnd)
    {
        this.__field__topLeft = _topLeft;
        this.__field__topRight = _topRight;
        this.__field__bottomLeft = _bottomLeft;
        this.__field__bottomRight = _bottomRight;
        this.__field__topStart = _topStart;
        this.__field__topEnd = _topEnd;
        this.__field__bottomStart = _bottomStart;
        this.__field__bottomEnd = _bottomEnd;
    }

    public override _MixedBorderRadius__border_radius op_Subtract()
    {
        return new _MixedBorderRadius__border_radius(-this._topLeft, -this._topRight, -this._bottomLeft, -this._bottomRight, -this._topStart, -this._topEnd, -this._bottomStart, -this._bottomEnd);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedBorderRadius__border_radius op_Multiply(double other)
    {
        return new _MixedBorderRadius__border_radius((this._topLeft * other), (this._topRight * other), (this._bottomLeft * other), (this._bottomRight * other), (this._topStart * other), (this._topEnd * other), (this._bottomStart * other), (this._bottomEnd * other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedBorderRadius__border_radius op_Divide(double other)
    {
        return new _MixedBorderRadius__border_radius((this._topLeft / other), (this._topRight / other), (this._bottomLeft / other), (this._bottomRight / other), (this._topStart / other), (this._topEnd / other), (this._bottomStart / other), (this._bottomEnd / other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedBorderRadius__border_radius ___(double other)
    {
        return new _MixedBorderRadius__border_radius((this._topLeft.___(other)), (this._topRight.___(other)), (this._bottomLeft.___(other)), (this._bottomRight.___(other)), (this._topStart.___(other)), (this._topEnd.___(other)), (this._bottomStart.___(other)), (this._bottomEnd.___(other)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedBorderRadius__border_radius __(double other)
    {
        return new _MixedBorderRadius__border_radius((this._topLeft % other), (this._topRight % other), (this._bottomLeft % other), (this._bottomRight % other), (this._topStart % other), (this._topEnd % other), (this._bottomStart % other), (this._bottomEnd % other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadius resolve(TextDirection? direction)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Painting.DebugLibrary.debugCheckCanResolveTextDirection(direction, $"{typeof(_MixedBorderRadius__border_radius)}"));
        switch (DartRuntimePrimitives.RequireValue(direction))
        {
            case TextDirection.rtl:
                {
                    return new BorderRadius(topLeft: (this._topLeft + this._topEnd), topRight: (this._topRight + this._topStart), bottomLeft: (this._bottomLeft + this._bottomEnd), bottomRight: (this._bottomRight + this._bottomStart));
                }
            case TextDirection.ltr:
                {
                    return new BorderRadius(topLeft: (this._topLeft + this._topStart), topRight: (this._topRight + this._topEnd), bottomLeft: (this._bottomLeft + this._bottomStart), bottomRight: (this._bottomRight + this._bottomEnd));
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

