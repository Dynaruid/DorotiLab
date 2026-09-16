// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/border_radius.dart
using Doroti.Runtime;
using Doroti.Ui;

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
        DartRuntimePrimitives.Assert(() => (left is null) && (right is null) || (start is null) && (end is null));
        if ((start is not null) || (end is not null))
        {
            return BorderRadiusDirectional.CreateHorizontal(start: start ?? Radius.zero, end: end ?? Radius.zero);
        }
        return BorderRadius.CreateHorizontal(left: left ?? Radius.zero, right: right ?? Radius.zero);
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
        return new _MixedBorderRadius__border_radius(_topLeft - other._topLeft, _topRight - other._topRight, _bottomLeft - other._bottomLeft, _bottomRight - other._bottomRight, _topStart - other._topStart, _topEnd - other._topEnd, _bottomStart - other._bottomStart, _bottomEnd - other._bottomEnd);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BorderRadiusGeometry add(BorderRadiusGeometry other)
    {
        return new _MixedBorderRadius__border_radius(_topLeft + other._topLeft, _topRight + other._topRight, _bottomLeft + other._bottomLeft, _bottomRight + other._bottomRight, _topStart + other._topStart, _topEnd + other._topEnd, _bottomStart + other._bottomStart, _bottomEnd + other._bottomEnd);
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
        return a.add(b.subtract(a).op_Multiply(t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract BorderRadius resolve(TextDirection? direction);
    public override string ToString()
    {
        string? visual = default!;
        string? logical = default!;
        if (Equals(_topLeft, _topRight) && Equals(_topRight, _bottomLeft) && Equals(_bottomLeft, _bottomRight))
        {
            if (!Equals(_topLeft, Radius.zero))
            {
                if (_topLeft.x == _topLeft.y)
                {
                    visual = $"BorderRadius.circular({_topLeft.x.toStringAsFixed(1L)})";
                }
                else
                {
                    visual = $"BorderRadius.all({_topLeft})";
                }
            }
        }
        else
        {
            var result = new StringBuffer();
            result.write("BorderRadius.only(");
            var comma = false;
            if (!Equals(_topLeft, Radius.zero))
            {
                result.write($"topLeft: {_topLeft}");
                comma = true;
            }
            if (!Equals(_topRight, Radius.zero))
            {
                if (comma)
                {
                    result.write(", ");
                }
                result.write($"topRight: {_topRight}");
                comma = true;
            }
            if (!Equals(_bottomLeft, Radius.zero))
            {
                if (comma)
                {
                    result.write(", ");
                }
                result.write($"bottomLeft: {_bottomLeft}");
                comma = true;
            }
            if (!Equals(_bottomRight, Radius.zero))
            {
                if (comma)
                {
                    result.write(", ");
                }
                result.write($"bottomRight: {_bottomRight}");
            }
            result.write(")");
            visual = result.ToString();
        }
        if (Equals(_topStart, _topEnd) && Equals(_topEnd, _bottomEnd) && Equals(_bottomEnd, _bottomStart))
        {
            if (!Equals(_topStart, Radius.zero))
            {
                if (_topStart.x == _topStart.y)
                {
                    logical = $"BorderRadiusDirectional.circular({_topStart.x.toStringAsFixed(1L)})";
                }
                else
                {
                    logical = $"BorderRadiusDirectional.all({_topStart})";
                }
            }
        }
        else
        {
            var resultLocal = new StringBuffer();
            resultLocal.write("BorderRadiusDirectional.only(");
            var commaLocal = false;
            if (!Equals(_topStart, Radius.zero))
            {
                resultLocal.write($"topStart: {_topStart}");
                commaLocal = true;
            }
            if (!Equals(_topEnd, Radius.zero))
            {
                if (commaLocal)
                {
                    resultLocal.write(", ");
                }
                resultLocal.write($"topEnd: {_topEnd}");
                commaLocal = true;
            }
            if (!Equals(_bottomStart, Radius.zero))
            {
                if (commaLocal)
                {
                    resultLocal.write(", ");
                }
                resultLocal.write($"bottomStart: {_bottomStart}");
                commaLocal = true;
            }
            if (!Equals(_bottomEnd, Radius.zero))
            {
                if (commaLocal)
                {
                    resultLocal.write(", ");
                }
                resultLocal.write($"bottomEnd: {_bottomEnd}");
            }
            resultLocal.write(")");
            logical = resultLocal.ToString();
        }
        if ((visual is not null) && (logical is not null))
        {
            return $"{visual} + {logical}";
        }
        return (visual ?? logical) ?? "BorderRadius.zero";
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
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is BorderRadiusGeometry) && Equals(__other._topLeft, _topLeft) && Equals(__other._topRight, _topRight) && Equals(__other._bottomLeft, _bottomLeft) && Equals(__other._bottomRight, _bottomRight) && Equals(__other._topStart, _topStart) && Equals(__other._topEnd, _topEnd) && Equals(__other._bottomStart, _bottomStart) && Equals(__other._bottomEnd, _bottomEnd);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(_topLeft, _topRight, _bottomLeft, _bottomRight, _topStart, _topEnd, _bottomStart, _bottomEnd);
}

public class BorderRadius : BorderRadiusGeometry
{
    public new static BorderRadius zero = CreateAll(Radius.zero);
    public virtual Radius topLeft { get; private set; } = default!;
    public virtual Radius topRight { get; private set; } = default!;
    public virtual Radius bottomLeft { get; private set; } = default!;
    public virtual Radius bottomRight { get; private set; } = default!;

    public new static BorderRadius CreateAll(Radius radius)
    {
        return new BorderRadius(topLeft: radius, topRight: radius, bottomLeft: radius, bottomRight: radius);
    }

    public new static BorderRadius CreateCircular(double radius)
    {
        return CreateAll(Radius.circular(radius));
    }

    public new static BorderRadius CreateVertical(Radius top = default, Radius bottom = default)
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
        return new BorderRadius(topLeft: topLeft ?? this.topLeft, topRight: topRight ?? this.topRight, bottomLeft: bottomLeft ?? this.bottomLeft, bottomRight: bottomRight ?? this.bottomRight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override Radius _topLeft => topLeft;
    internal override Radius _topRight => topRight;
    internal override Radius _bottomLeft => bottomLeft;
    internal override Radius _bottomRight => bottomRight;
    internal override Radius _topStart => Radius.zero;
    internal override Radius _topEnd => Radius.zero;
    internal override Radius _bottomStart => Radius.zero;
    internal override Radius _bottomEnd => Radius.zero;
    public virtual global::Doroti.Ui.RRect toRRect(Rect rect)
    {
        return RRect.fromRectAndCorners(rect, topLeft: topLeft.clamp(minimum: Radius.zero), topRight: topRight.clamp(minimum: Radius.zero), bottomLeft: bottomLeft.clamp(minimum: Radius.zero), bottomRight: bottomRight.clamp(minimum: Radius.zero));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.RSuperellipse toRSuperellipse(Rect rect)
    {
        return RSuperellipse.fromRectAndCorners(rect, topLeft: topLeft.clamp(minimum: Radius.zero), topRight: topRight.clamp(minimum: Radius.zero), bottomLeft: bottomLeft.clamp(minimum: Radius.zero), bottomRight: bottomRight.clamp(minimum: Radius.zero));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadiusGeometry subtract(BorderRadiusGeometry other)
    {
        if (other is BorderRadius)
        {
            BorderRadius other__as16879 = (BorderRadius)other;
            return op_Subtract(other__as16879);
        }
        return base.subtract(other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadiusGeometry add(BorderRadiusGeometry other)
    {
        if (other is BorderRadius)
        {
            BorderRadius other__as17053 = (BorderRadius)other;
            return op_Add(other__as17053);
        }
        return base.add(other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BorderRadius op_Subtract(BorderRadius other)
    {
        return new BorderRadius(topLeft: topLeft - other.topLeft, topRight: topRight - other.topRight, bottomLeft: bottomLeft - other.bottomLeft, bottomRight: bottomRight - other.bottomRight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BorderRadius op_Add(BorderRadius other)
    {
        return new BorderRadius(topLeft: topLeft + other.topLeft, topRight: topRight + other.topRight, bottomLeft: bottomLeft + other.bottomLeft, bottomRight: bottomRight + other.bottomRight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadius op_Subtract()
    {
        return new BorderRadius(topLeft: -topLeft, topRight: -topRight, bottomLeft: -bottomLeft, bottomRight: -bottomRight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadius op_Multiply(double other)
    {
        return new BorderRadius(topLeft: topLeft * other, topRight: topRight * other, bottomLeft: bottomLeft * other, bottomRight: bottomRight * other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadius op_Divide(double other)
    {
        return new BorderRadius(topLeft: topLeft / other, topRight: topRight / other, bottomLeft: bottomLeft / other, bottomRight: bottomRight / other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadius ___(double other)
    {
        return new BorderRadius(topLeft: topLeft.___(other), topRight: topRight.___(other), bottomLeft: bottomLeft.___(other), bottomRight: bottomRight.___(other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadius __(double other)
    {
        return new BorderRadius(topLeft: topLeft % other, topRight: topRight % other, bottomLeft: bottomLeft % other, bottomRight: bottomRight % other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BorderRadius? lerp(BorderRadius? a, BorderRadius? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if (a is null)
        {
            return b!.op_Multiply(t);
        }
        if (b is null)
        {
            return a.op_Multiply(1.0 - t);
        }
        return new BorderRadius(topLeft: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Radius.lerp(a.topLeft, b.topLeft, t)), topRight: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Radius.lerp(a.topRight, b.topRight, t)), bottomLeft: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Radius.lerp(a.bottomLeft, b.bottomLeft, t)), bottomRight: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Radius.lerp(a.bottomRight, b.bottomRight, t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadius resolve(TextDirection? direction) => this;
}

public class BorderRadiusDirectional : BorderRadiusGeometry
{
    public new static BorderRadiusDirectional zero = CreateAll(Radius.zero);
    public virtual Radius topStart { get; private set; } = default!;
    public virtual Radius topEnd { get; private set; } = default!;
    public virtual Radius bottomStart { get; private set; } = default!;
    public virtual Radius bottomEnd { get; private set; } = default!;

    public new static BorderRadiusDirectional CreateAll(Radius radius)
    {
        return new BorderRadiusDirectional(topStart: radius, topEnd: radius, bottomStart: radius, bottomEnd: radius);
    }

    public new static BorderRadiusDirectional CreateCircular(double radius)
    {
        return CreateAll(Radius.circular(radius));
    }

    public new static BorderRadiusDirectional CreateVertical(Radius top = default, Radius bottom = default)
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

    internal override Radius _topStart => topStart;
    internal override Radius _topEnd => topEnd;
    internal override Radius _bottomStart => bottomStart;
    internal override Radius _bottomEnd => bottomEnd;
    internal override Radius _topLeft => Radius.zero;
    internal override Radius _topRight => Radius.zero;
    internal override Radius _bottomLeft => Radius.zero;
    internal override Radius _bottomRight => Radius.zero;
    public override BorderRadiusGeometry subtract(BorderRadiusGeometry other)
    {
        if (other is BorderRadiusDirectional)
        {
            BorderRadiusDirectional other__as23144 = (BorderRadiusDirectional)other;
            return op_Subtract(other__as23144);
        }
        return base.subtract(other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadiusGeometry add(BorderRadiusGeometry other)
    {
        if (other is BorderRadiusDirectional)
        {
            BorderRadiusDirectional other__as23329 = (BorderRadiusDirectional)other;
            return op_Add(other__as23329);
        }
        return base.add(other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BorderRadiusDirectional op_Subtract(BorderRadiusDirectional other)
    {
        return new BorderRadiusDirectional(topStart: topStart - other.topStart, topEnd: topEnd - other.topEnd, bottomStart: bottomStart - other.bottomStart, bottomEnd: bottomEnd - other.bottomEnd);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BorderRadiusDirectional op_Add(BorderRadiusDirectional other)
    {
        return new BorderRadiusDirectional(topStart: topStart + other.topStart, topEnd: topEnd + other.topEnd, bottomStart: bottomStart + other.bottomStart, bottomEnd: bottomEnd + other.bottomEnd);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadiusDirectional op_Subtract()
    {
        return new BorderRadiusDirectional(topStart: -topStart, topEnd: -topEnd, bottomStart: -bottomStart, bottomEnd: -bottomEnd);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadiusDirectional op_Multiply(double other)
    {
        return new BorderRadiusDirectional(topStart: topStart * other, topEnd: topEnd * other, bottomStart: bottomStart * other, bottomEnd: bottomEnd * other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadiusDirectional op_Divide(double other)
    {
        return new BorderRadiusDirectional(topStart: topStart / other, topEnd: topEnd / other, bottomStart: bottomStart / other, bottomEnd: bottomEnd / other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadiusDirectional ___(double other)
    {
        return new BorderRadiusDirectional(topStart: topStart.___(other), topEnd: topEnd.___(other), bottomStart: bottomStart.___(other), bottomEnd: bottomEnd.___(other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadiusDirectional __(double other)
    {
        return new BorderRadiusDirectional(topStart: topStart % other, topEnd: topEnd % other, bottomStart: bottomStart % other, bottomEnd: bottomEnd % other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BorderRadiusDirectional? lerp(BorderRadiusDirectional? a, BorderRadiusDirectional? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if (a is null)
        {
            return b!.op_Multiply(t);
        }
        if (b is null)
        {
            return a.op_Multiply(1.0 - t);
        }
        return new BorderRadiusDirectional(topStart: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Radius.lerp(a.topStart, b.topStart, t)), topEnd: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Radius.lerp(a.topEnd, b.topEnd, t)), bottomStart: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Radius.lerp(a.bottomStart, b.bottomStart, t)), bottomEnd: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Radius.lerp(a.bottomEnd, b.bottomEnd, t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadius resolve(TextDirection? direction)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckCanResolveTextDirection(direction, $"{typeof(BorderRadiusDirectional)}"));
        switch (DartRuntimePrimitives.RequireValue(direction))
        {
            case TextDirection.rtl:
                {
                    return new BorderRadius(topLeft: topEnd, topRight: topStart, bottomLeft: bottomEnd, bottomRight: bottomStart);
                }
            case TextDirection.ltr:
                {
                    return new BorderRadius(topLeft: topStart, topRight: topEnd, bottomLeft: bottomStart, bottomRight: bottomEnd);
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
        __field__topLeft = _topLeft;
        __field__topRight = _topRight;
        __field__bottomLeft = _bottomLeft;
        __field__bottomRight = _bottomRight;
        __field__topStart = _topStart;
        __field__topEnd = _topEnd;
        __field__bottomStart = _bottomStart;
        __field__bottomEnd = _bottomEnd;
    }

    public override _MixedBorderRadius__border_radius op_Subtract()
    {
        return new _MixedBorderRadius__border_radius(-_topLeft, -_topRight, -_bottomLeft, -_bottomRight, -_topStart, -_topEnd, -_bottomStart, -_bottomEnd);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedBorderRadius__border_radius op_Multiply(double other)
    {
        return new _MixedBorderRadius__border_radius(_topLeft * other, _topRight * other, _bottomLeft * other, _bottomRight * other, _topStart * other, _topEnd * other, _bottomStart * other, _bottomEnd * other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedBorderRadius__border_radius op_Divide(double other)
    {
        return new _MixedBorderRadius__border_radius(_topLeft / other, _topRight / other, _bottomLeft / other, _bottomRight / other, _topStart / other, _topEnd / other, _bottomStart / other, _bottomEnd / other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedBorderRadius__border_radius ___(double other)
    {
        return new _MixedBorderRadius__border_radius(_topLeft.___(other), _topRight.___(other), _bottomLeft.___(other), _bottomRight.___(other), _topStart.___(other), _topEnd.___(other), _bottomStart.___(other), _bottomEnd.___(other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedBorderRadius__border_radius __(double other)
    {
        return new _MixedBorderRadius__border_radius(_topLeft % other, _topRight % other, _bottomLeft % other, _bottomRight % other, _topStart % other, _topEnd % other, _bottomStart % other, _bottomEnd % other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderRadius resolve(TextDirection? direction)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckCanResolveTextDirection(direction, $"{typeof(_MixedBorderRadius__border_radius)}"));
        switch (DartRuntimePrimitives.RequireValue(direction))
        {
            case TextDirection.rtl:
                {
                    return new BorderRadius(topLeft: _topLeft + _topEnd, topRight: _topRight + _topStart, bottomLeft: _bottomLeft + _bottomEnd, bottomRight: _bottomRight + _bottomStart);
                }
            case TextDirection.ltr:
                {
                    return new BorderRadius(topLeft: _topLeft + _topStart, topRight: _topRight + _topEnd, bottomLeft: _bottomLeft + _bottomStart, bottomRight: _bottomRight + _bottomEnd);
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

