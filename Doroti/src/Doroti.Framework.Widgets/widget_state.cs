// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/widget_state.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public interface WidgetStatesConstraint
{
    public bool isSatisfiedBy(HashSet<WidgetState> states);
    public WidgetStatesConstraint op_BitwiseAnd(WidgetStatesConstraint other);
    public WidgetStatesConstraint op_BitwiseOr(WidgetStatesConstraint other);
    public WidgetStatesConstraint op_OnesComplement();
}

internal abstract class _WidgetStateCombo__widget_state : WidgetStatesConstraint
{
    public virtual WidgetStatesConstraint first { get; private set; } = default!;
    public virtual WidgetStatesConstraint second { get; private set; } = default!;

    internal _WidgetStateCombo__widget_state(WidgetStatesConstraint first, WidgetStatesConstraint second)
    {
        this.first = first;
        this.second = second;
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.first, this.second));
    public virtual bool isSatisfiedBy(HashSet<WidgetState> states) => throw new NotSupportedException();
    public virtual WidgetStatesConstraint op_BitwiseAnd(WidgetStatesConstraint other) => DartRuntimePrimitives.ConvertValue<WidgetStatesConstraint>(new _WidgetStateAnd__widget_state(this, other));
    public virtual WidgetStatesConstraint op_BitwiseOr(WidgetStatesConstraint other) => DartRuntimePrimitives.ConvertValue<WidgetStatesConstraint>(new _WidgetStateOr__widget_state(this, other));
    public virtual WidgetStatesConstraint op_OnesComplement() => DartRuntimePrimitives.ConvertValue<WidgetStatesConstraint>(new _WidgetStateNot__widget_state(this));
}

internal class _WidgetStateAnd__widget_state : _WidgetStateCombo__widget_state
{
    internal _WidgetStateAnd__widget_state(WidgetStatesConstraint first, WidgetStatesConstraint second) : base(first, second)
    {
    }

    public virtual bool isSatisfiedBy(HashSet<WidgetState> states)
    {
        return (this.first.isSatisfiedBy(states) && this.second.isSatisfiedBy(states));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _WidgetStateAnd__widget_state;
        if (__other is null) return false;
        return (((__other is _WidgetStateAnd__widget_state) && (object.Equals(((_WidgetStateAnd__widget_state)__other).first, this.first))) && (object.Equals(((_WidgetStateAnd__widget_state)__other).second, this.second)));
    }

    public override string ToString() => $"({this.first} & {this.second})";
}

internal class _WidgetStateOr__widget_state : _WidgetStateCombo__widget_state
{
    internal _WidgetStateOr__widget_state(WidgetStatesConstraint first, WidgetStatesConstraint second) : base(first, second)
    {
    }

    public virtual bool isSatisfiedBy(HashSet<WidgetState> states)
    {
        return (this.first.isSatisfiedBy(states) || this.second.isSatisfiedBy(states));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _WidgetStateOr__widget_state;
        if (__other is null) return false;
        return (((__other is _WidgetStateOr__widget_state) && (object.Equals(((_WidgetStateOr__widget_state)__other).first, this.first))) && (object.Equals(((_WidgetStateOr__widget_state)__other).second, this.second)));
    }

    public override string ToString() => $"({this.first} | {this.second})";
}

internal class _WidgetStateNot__widget_state : WidgetStatesConstraint
{
    public virtual WidgetStatesConstraint value { get; private set; } = default!;

    internal _WidgetStateNot__widget_state(WidgetStatesConstraint value)
    {
        this.value = value;
    }

    public virtual bool isSatisfiedBy(HashSet<WidgetState> states) => !this.value.isSatisfiedBy(states);
    public override bool Equals(object? other)
    {
        var __other = other as _WidgetStateNot__widget_state;
        if (__other is null) return false;
        return ((__other is _WidgetStateNot__widget_state) && (object.Equals(((_WidgetStateNot__widget_state)((_WidgetStateNot__widget_state)__other)).value, this.value)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(this.value.GetHashCode());
    public override string ToString() => $"~{this.value}";
    public virtual WidgetStatesConstraint op_BitwiseAnd(WidgetStatesConstraint other) => DartRuntimePrimitives.ConvertValue<WidgetStatesConstraint>(new _WidgetStateAnd__widget_state(this, other));
    public virtual WidgetStatesConstraint op_BitwiseOr(WidgetStatesConstraint other) => DartRuntimePrimitives.ConvertValue<WidgetStatesConstraint>(new _WidgetStateOr__widget_state(this, other));
    public virtual WidgetStatesConstraint op_OnesComplement() => DartRuntimePrimitives.ConvertValue<WidgetStatesConstraint>(new _WidgetStateNot__widget_state(this));
}

internal class _AnyWidgetStates__widget_state : WidgetStatesConstraint
{

    internal _AnyWidgetStates__widget_state()
    {
    }

    public virtual bool isSatisfiedBy(HashSet<WidgetState> states) => true;
    public override string ToString() => "WidgetState.any";
    public virtual WidgetStatesConstraint op_BitwiseAnd(WidgetStatesConstraint other) => DartRuntimePrimitives.ConvertValue<WidgetStatesConstraint>(new _WidgetStateAnd__widget_state(this, other));
    public virtual WidgetStatesConstraint op_BitwiseOr(WidgetStatesConstraint other) => DartRuntimePrimitives.ConvertValue<WidgetStatesConstraint>(new _WidgetStateOr__widget_state(this, other));
    public virtual WidgetStatesConstraint op_OnesComplement() => DartRuntimePrimitives.ConvertValue<WidgetStatesConstraint>(new _WidgetStateNot__widget_state(this));
}

public enum WidgetState
{
    hovered,
    focused,
    pressed,
    dragged,
    selected,
    scrolledUnder,
    disabled,
    error
}

public static class WidgetStateMembers
{
    public static bool isSatisfiedBy(this WidgetState value, HashSet<WidgetState> states) => states.Contains(value);
    public static WidgetStatesConstraint asConstraint(this WidgetState value) => new _SingleWidgetStateConstraint(value);
    public static WidgetStatesConstraint any => new _AnyWidgetStates__widget_state();

    private sealed class _SingleWidgetStateConstraint(WidgetState value) : WidgetStatesConstraint
    {
        public bool isSatisfiedBy(HashSet<WidgetState> states) => states.Contains(value);
        public WidgetStatesConstraint op_BitwiseAnd(WidgetStatesConstraint other) => new _WidgetStateAnd__widget_state(this, other);
        public WidgetStatesConstraint op_BitwiseOr(WidgetStatesConstraint other) => new _WidgetStateOr__widget_state(this, other);
        public WidgetStatesConstraint op_OnesComplement() => new _WidgetStateNot__widget_state(this);
        public override string ToString() => $"WidgetState.{value}";
    }
}

public delegate T WidgetPropertyResolver<T>(HashSet<WidgetState> states);

public abstract class WidgetStateColor : Color, WidgetStateProperty<Color>
{
    public static WidgetStateColor transparent = ((WidgetStateColor)(object?)new _WidgetStateColorTransparent__widget_state());

    protected WidgetStateColor(long defaultValue) : base(defaultValue)
    {
    }

    public static WidgetStateColor CreateResolveWith(global::System.Func<HashSet<WidgetState>, Color> callback)
        => ((WidgetStateColor)(object?)new _WidgetStateColor__widget_state(callback));

    public static WidgetStateColor CreateFromMap(DartMap<WidgetStatesConstraint, Color> map)
        => ((WidgetStateColor)(object?)new _WidgetStateColorMapper__widget_state(map));

    public abstract Color resolve(HashSet<WidgetState> states);
}

internal class _WidgetStateColor__widget_state : WidgetStateColor
{
    internal virtual global::System.Func<HashSet<WidgetState>, Color> _resolve { get; private set; } = default!;
    internal static HashSet<WidgetState> _defaultStates = new HashSet<WidgetState>();

    internal _WidgetStateColor__widget_state(global::System.Func<HashSet<WidgetState>, Color> _resolve) : base(_resolve(_defaultStates).value)
    {
        this._resolve = _resolve;
    }

    public override Color resolve(HashSet<WidgetState> states) => this._resolve(states);
}

internal class _WidgetStateColorTransparent__widget_state : WidgetStateColor
{
    internal _WidgetStateColorTransparent__widget_state() : base(0L)
    {
    }

    public override Color resolve(HashSet<WidgetState> states) => new global::Doroti.Ui.Color(0L);
}

internal class _WidgetStateColorMapper__widget_state : WidgetStateMapper<Color>
{
    internal _WidgetStateColorMapper__widget_state(DartMap<WidgetStatesConstraint, Color> map) : base(map)
    {
    }

}

public abstract class WidgetStateMouseCursor : global::Doroti.Generated.Framework.Services.MouseCursor, WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor>
{
    public static WidgetStateMouseCursor clickable = WidgetStateMouseCursor.CreateResolveWith((global::System.Func<HashSet<WidgetState>, global::Doroti.Generated.Framework.Services.MouseCursor>)_clickable, debugDescription: "WidgetStateMouseCursor(clickable)");
    public static WidgetStateMouseCursor adaptiveClickable = WidgetStateMouseCursor.CreateResolveWith((global::System.Func<HashSet<WidgetState>, global::Doroti.Generated.Framework.Services.MouseCursor>)_adaptiveClickable, debugDescription: "WidgetStateMouseCursor(adaptiveClickable)");
    public static WidgetStateMouseCursor textable = WidgetStateMouseCursor.CreateResolveWith((global::System.Func<HashSet<WidgetState>, global::Doroti.Generated.Framework.Services.MouseCursor>)_textable, debugDescription: "WidgetStateMouseCursor(textable)");

    protected WidgetStateMouseCursor()
    {
    }

    public static WidgetStateMouseCursor CreateResolveWith(global::System.Func<HashSet<WidgetState>, global::Doroti.Generated.Framework.Services.MouseCursor> callback, string debugDescription = default!)
        => ((WidgetStateMouseCursor)(object?)new _WidgetStateMouseCursor__widget_state(callback, debugDescription));

    public static WidgetStateMouseCursor CreateFromMap(DartMap<WidgetStatesConstraint, global::Doroti.Generated.Framework.Services.MouseCursor> map)
        => ((WidgetStateMouseCursor)(object?)new _WidgetMouseCursorMapper__widget_state(map));

    public override global::Doroti.Generated.Framework.Services.MouseCursorSession createSession(long device)
    {
        return ((global::Doroti.Generated.Framework.Services.MouseCursorSession)(object?)resolve(new HashSet<WidgetState>()).createSession(device));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static WidgetStateMouseCursor CreateFromMap(DartMap<WidgetState, global::Doroti.Generated.Framework.Services.MouseCursor> map) =>
        CreateFromMap(WidgetStateMapAdapters.toConstraints(map));

    public abstract global::Doroti.Generated.Framework.Services.MouseCursor resolve(HashSet<WidgetState> states);
    internal static global::Doroti.Generated.Framework.Services.MouseCursor _clickable(HashSet<WidgetState> states)
    {
        if (states.Contains(WidgetState.disabled))
        {
            return ((global::Doroti.Generated.Framework.Services.MouseCursor)(object?)global::Doroti.Generated.Framework.Services.SystemMouseCursors.basic);
        }
        return ((global::Doroti.Generated.Framework.Services.MouseCursor)(object?)global::Doroti.Generated.Framework.Services.SystemMouseCursors.click);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Generated.Framework.Services.MouseCursor _adaptiveClickable(HashSet<WidgetState> states)
    {
        if (states.Contains(WidgetState.disabled))
        {
            return ((global::Doroti.Generated.Framework.Services.MouseCursor)(object?)global::Doroti.Generated.Framework.Services.SystemMouseCursors.basic);
        }
        return ((global::Doroti.Generated.Framework.Services.MouseCursor)(object?)(global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb ? global::Doroti.Generated.Framework.Services.SystemMouseCursors.click : global::Doroti.Generated.Framework.Services.SystemMouseCursors.basic));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Generated.Framework.Services.MouseCursor _textable(HashSet<WidgetState> states)
    {
        if (states.Contains(WidgetState.disabled))
        {
            return ((global::Doroti.Generated.Framework.Services.MouseCursor)(object?)global::Doroti.Generated.Framework.Services.SystemMouseCursors.basic);
        }
        return ((global::Doroti.Generated.Framework.Services.MouseCursor)(object?)global::Doroti.Generated.Framework.Services.SystemMouseCursors.text);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _WidgetStateMouseCursor__widget_state : WidgetStateMouseCursor
{
    internal virtual global::System.Func<HashSet<WidgetState>, global::Doroti.Generated.Framework.Services.MouseCursor> _resolve { get; private set; } = default!;
    private string __field_debugDescription = default!;
    public override string debugDescription { get => __field_debugDescription; }

    internal _WidgetStateMouseCursor__widget_state(global::System.Func<HashSet<WidgetState>, global::Doroti.Generated.Framework.Services.MouseCursor> _resolve, string debugDescription = "WidgetStateMouseCursor()")
    {
        this._resolve = _resolve;
        this.__field_debugDescription = debugDescription;
    }

    public override global::Doroti.Generated.Framework.Services.MouseCursor resolve(HashSet<WidgetState> states) => this._resolve(states);
}

internal class _WidgetMouseCursorMapper__widget_state : WidgetStateMapper<global::Doroti.Generated.Framework.Services.MouseCursor>
{
    internal _WidgetMouseCursorMapper__widget_state(DartMap<WidgetStatesConstraint, global::Doroti.Generated.Framework.Services.MouseCursor> map) : base(map)
    {
    }

}

public abstract class WidgetStateBorderSide : global::Doroti.Generated.Framework.Painting.BorderSide, WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>
{
    protected WidgetStateBorderSide()
    {
    }

    public static WidgetStateBorderSide CreateResolveWith(global::System.Func<HashSet<WidgetState>, global::Doroti.Generated.Framework.Painting.BorderSide?> callback)
        => ((WidgetStateBorderSide)(object?)new _WidgetStateBorderSide__widget_state(callback));

    public static WidgetStateBorderSide CreateFromMap(DartMap<WidgetStatesConstraint, global::Doroti.Generated.Framework.Painting.BorderSide?> map)
        => ((WidgetStateBorderSide)(object?)new _WidgetBorderSideMapper__widget_state(map));

    public abstract global::Doroti.Generated.Framework.Painting.BorderSide? resolve(HashSet<WidgetState> states);
    public static WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>? lerp(WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>? a, WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>? b, double t)
    {
        if (((a is null) && (b is null)))
        {
            return ((WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>)(object)null);
        }
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return ((WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>?)(object?)new _LerpSides__widget_state(a, b, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _LerpSides__widget_state : WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>
{
    public virtual WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>? a { get; private set; }
    public virtual WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>? b { get; private set; }
    public virtual double t { get; private set; } = default!;

    internal _LerpSides__widget_state(WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>? a, WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>? b, double t)
    {
        this.a = a;
        this.b = b;
        this.t = t;
    }

    public virtual global::Doroti.Generated.Framework.Painting.BorderSide? resolve(HashSet<WidgetState> states)
    {
        global::Doroti.Generated.Framework.Painting.BorderSide? resolvedA__22323 = ((global::Doroti.Generated.Framework.Painting.BorderSide?)(object?)this.a?.resolve(states));
        global::Doroti.Generated.Framework.Painting.BorderSide? resolvedB__22377 = ((global::Doroti.Generated.Framework.Painting.BorderSide?)(object?)this.b?.resolve(states));
        if (((resolvedA__22323 is null) && (resolvedB__22377 is null)))
        {
            return ((global::Doroti.Generated.Framework.Painting.BorderSide)(object)null);
        }
        if ((resolvedA__22323 is null))
        {
            return ((global::Doroti.Generated.Framework.Painting.BorderSide?)(object?)BorderSide.lerp(new global::Doroti.Generated.Framework.Painting.BorderSide(width: 0, color: resolvedB__22377!.color.withAlpha(0L)), resolvedB__22377, this.t));
        }
        if ((resolvedB__22377 is null))
        {
            return ((global::Doroti.Generated.Framework.Painting.BorderSide?)(object?)BorderSide.lerp(resolvedA__22323, new global::Doroti.Generated.Framework.Painting.BorderSide(width: 0, color: ((global::Doroti.Generated.Framework.Painting.BorderSide)resolvedA__22323).color.withAlpha(0L)), this.t));
        }
        return ((global::Doroti.Generated.Framework.Painting.BorderSide?)(object?)BorderSide.lerp(resolvedA__22323, resolvedB__22377, this.t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _WidgetStateBorderSide__widget_state : WidgetStateBorderSide
{
    internal virtual global::System.Func<HashSet<WidgetState>, global::Doroti.Generated.Framework.Painting.BorderSide?> _resolve { get; private set; } = default!;

    internal _WidgetStateBorderSide__widget_state(global::System.Func<HashSet<WidgetState>, global::Doroti.Generated.Framework.Painting.BorderSide?> _resolve)
    {
        this._resolve = _resolve;
    }

    public override global::Doroti.Generated.Framework.Painting.BorderSide? resolve(HashSet<WidgetState> states) => this._resolve(states);
}

internal class _WidgetBorderSideMapper__widget_state : WidgetStateMapper<global::Doroti.Generated.Framework.Painting.BorderSide?>
{
    internal _WidgetBorderSideMapper__widget_state(DartMap<WidgetStatesConstraint, global::Doroti.Generated.Framework.Painting.BorderSide?> map) : base(map)
    {
    }

}

public abstract class WidgetStateOutlinedBorder : global::Doroti.Generated.Framework.Painting.OutlinedBorder, WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>
{
    protected WidgetStateOutlinedBorder()
    {
    }

    public static WidgetStateOutlinedBorder CreateResolveWith(global::System.Func<HashSet<WidgetState>, global::Doroti.Generated.Framework.Painting.OutlinedBorder?> callback)
        => ((WidgetStateOutlinedBorder)(object?)new _WidgetStateOutlinedBorder__widget_state(callback));

    public static WidgetStateOutlinedBorder CreateFromMap(DartMap<WidgetStatesConstraint, global::Doroti.Generated.Framework.Painting.OutlinedBorder?> map)
        => ((WidgetStateOutlinedBorder)(object?)new _WidgetOutlinedBorderMapper__widget_state(map));

    public abstract global::Doroti.Generated.Framework.Painting.OutlinedBorder? resolve(HashSet<WidgetState> states);
}

internal class _WidgetStateOutlinedBorder__widget_state : global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder
{
    internal virtual global::System.Func<HashSet<WidgetState>, global::Doroti.Generated.Framework.Painting.OutlinedBorder?> _resolve { get; private set; } = default!;

    internal _WidgetStateOutlinedBorder__widget_state(global::System.Func<HashSet<WidgetState>, global::Doroti.Generated.Framework.Painting.OutlinedBorder?> _resolve)
    {
        this._resolve = _resolve;
    }

    public virtual global::Doroti.Generated.Framework.Painting.OutlinedBorder? resolve(HashSet<WidgetState> states) => this._resolve(states);
}

internal class _WidgetOutlinedBorderMapper__widget_state : WidgetStateMapper<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>
{
    internal _WidgetOutlinedBorderMapper__widget_state(DartMap<WidgetStatesConstraint, global::Doroti.Generated.Framework.Painting.OutlinedBorder?> map) : base(map)
    {
    }

}

public abstract class WidgetStateTextStyle : global::Doroti.Generated.Framework.Painting.TextStyle, WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle>
{
    protected WidgetStateTextStyle()
    {
    }

    public static WidgetStateTextStyle CreateResolveWith(global::System.Func<HashSet<WidgetState>, global::Doroti.Generated.Framework.Painting.TextStyle> callback)
        => ((WidgetStateTextStyle)(object?)new _WidgetStateTextStyle__widget_state(callback));

    public static WidgetStateTextStyle CreateFromMap(DartMap<WidgetStatesConstraint, global::Doroti.Generated.Framework.Painting.TextStyle> map)
        => ((WidgetStateTextStyle)(object?)new _WidgetTextStyleMapper__widget_state(map));

    public abstract global::Doroti.Generated.Framework.Painting.TextStyle resolve(HashSet<WidgetState> states);
}

internal class _WidgetStateTextStyle__widget_state : WidgetStateTextStyle
{
    internal virtual global::System.Func<HashSet<WidgetState>, global::Doroti.Generated.Framework.Painting.TextStyle> _resolve { get; private set; } = default!;

    internal _WidgetStateTextStyle__widget_state(global::System.Func<HashSet<WidgetState>, global::Doroti.Generated.Framework.Painting.TextStyle> _resolve)
    {
        this._resolve = _resolve;
    }

    public override global::Doroti.Generated.Framework.Painting.TextStyle resolve(HashSet<WidgetState> states) => this._resolve(states);
}

internal class _WidgetTextStyleMapper__widget_state : WidgetStateMapper<global::Doroti.Generated.Framework.Painting.TextStyle>
{
    internal _WidgetTextStyleMapper__widget_state(DartMap<WidgetStatesConstraint, global::Doroti.Generated.Framework.Painting.TextStyle> map) : base(map)
    {
    }

}

public static class WidgetStateProperty
{
    public static T resolveAs<T>(T value, HashSet<WidgetState> states) => WidgetStateProperty<T>.resolveAs(value, states);
    public static WidgetStateProperty<T> resolveWith<T>(global::System.Func<HashSet<WidgetState>, T> callback) => WidgetStateProperty<T>.resolveWith(callback);
    public static WidgetStateProperty<T> all<T>(T value) => WidgetStateProperty<T>.all(value);
    public static WidgetStateProperty<T?>? lerp<T>(WidgetStateProperty<T>? a, WidgetStateProperty<T>? b, double t, global::System.Func<T?, T?, double, T?> lerpFunction) => WidgetStateProperty<T>.lerp(a, b, t, lerpFunction);
}

public interface WidgetStateProperty<T>
{
    public static WidgetStateProperty<T> CreateFromMap(DartMap<WidgetStatesConstraint, T> map)
        => ((WidgetStateProperty<T>)(object?)new WidgetStateMapper<T>(map));

    public static WidgetStateProperty<T> CreateFromMap(DartMap<WidgetState, T> map)
        => CreateFromMap(WidgetStateMapAdapters.toConstraints(map));

    public static T resolveAs<T>(T value, HashSet<WidgetState> states)
    {
        if ((value is WidgetStateProperty<T>))
        {
            WidgetStateProperty<T> value__as32591 = (WidgetStateProperty<T>)value;
            WidgetStateProperty<T> property__32661 = ((WidgetStateProperty<T>)(object?)value__as32591);
            return ((T)(object?)property__32661.resolve(states));
        }
        return value;
    }
    public static WidgetStateProperty<T> resolveWith<T>(global::System.Func<HashSet<WidgetState>, T> callback) => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<T>>(new _WidgetStatePropertyWith__widget_state<T>((global::System.Func<HashSet<WidgetState>, T>)callback));
    public static WidgetStateProperty<T> all<T>(T value) => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<T>>(new WidgetStatePropertyAll<T>(value));
    public static WidgetStateProperty<T?>? lerp<T>(WidgetStateProperty<T>? a, WidgetStateProperty<T>? b, double t, global::System.Func<T?, T?, double, T?> lerpFunction)
    {
        if (((a is null) && (b is null)))
        {
            return default;
        }
        return ((WidgetStateProperty<T?>?)(object?)new _LerpProperties__widget_state<T>(a, b, t, (global::System.Func<T?, T?, double, T?>)lerpFunction));
    }
    public T resolve(HashSet<WidgetState> states);
}

internal static class WidgetStateMapAdapters
{
    internal static DartMap<WidgetStatesConstraint, T> toConstraints<T>(DartMap<WidgetState, T> map)
    {
        var result = new DartMap<WidgetStatesConstraint, T>();
        foreach (var entry in map) result[entry.Key.asConstraint()] = entry.Value;
        return result;
    }
}

internal class _LerpProperties__widget_state<T> : WidgetStateProperty<T?>
{
    public virtual WidgetStateProperty<T>? a { get; private set; }
    public virtual WidgetStateProperty<T>? b { get; private set; }
    public virtual double t { get; private set; } = default!;
    public virtual global::System.Func<T?, T?, double, T?> lerpFunction { get; private set; } = default!;

    internal _LerpProperties__widget_state(WidgetStateProperty<T>? a, WidgetStateProperty<T>? b, double t, global::System.Func<T?, T?, double, T?> lerpFunction)
    {
        this.a = a;
        this.b = b;
        this.t = t;
        this.lerpFunction = lerpFunction;
    }

    public virtual T? resolve(HashSet<WidgetState> states)
    {
        T? resolvedA__34542 = ((T?)(object?)DartRuntimePrimitives.NullAware(this.a, __target => __target.resolve(states)));
        T? resolvedB__34587 = ((T?)(object?)DartRuntimePrimitives.NullAware(this.b, __target => __target.resolve(states)));
        return this.lerpFunction(resolvedA__34542, resolvedB__34587, this.t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _WidgetStatePropertyWith__widget_state<T> : WidgetStateProperty<T>
{
    internal virtual global::System.Func<HashSet<WidgetState>, T> _resolve { get; private set; } = default!;

    internal _WidgetStatePropertyWith__widget_state(global::System.Func<HashSet<WidgetState>, T> _resolve)
    {
        this._resolve = _resolve;
    }

    public virtual T resolve(HashSet<WidgetState> states) => this._resolve(states);
}

public delegate void WidgetStateMap<T>();

public class WidgetStateMapper<T> : global::Doroti.Generated.Framework.Foundation.Diagnosticable, WidgetStateProperty<T>
{
    internal virtual DartMap<WidgetStatesConstraint, T> _map { get; private set; } = default!;

    public WidgetStateMapper(DartMap<WidgetStatesConstraint, T> map)
    {
        this._map = map;
    }

    public virtual T resolve(HashSet<WidgetState> states)
    {
        foreach (MapEntry<WidgetStatesConstraint, T> entry__38429 in this._map.entries)
        {
            if (entry__38429.key.isSatisfiedBy(states))
            {
                return entry__38429.value;
            }
        }
        try
        {
            return ((T?)(object?)null)!;
        }
        catch (TypeError)
        {
            throw DartRuntimePrimitives.AsException(new DartArgumentError($"The current set of widget states is {states}.\n" + "None of the provided map keys matched this set, " + $"and the type \"{typeof(T)}\" is non-nullable.\n" + $"Consider using \"WidgetStateMapper<{typeof(T)}?>()\", " + "or adding the \"WidgetState.any\" key to this map."));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as WidgetStateMapper<T>;
        if (__other is null) return false;
        return ((__other is WidgetStateMapper<T>) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.mapEquals(this._map, ((WidgetStateMapper<T>)((WidgetStateMapper<T>)__other))._map));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(new MapEquality<WidgetStatesConstraint, T>().hash(this._map));
    public virtual string ToString(global::Doroti.Generated.Framework.Foundation.DiagnosticLevel minLevel = global::Doroti.Generated.Framework.Foundation.DiagnosticLevel.info)
    {
        return $"WidgetStateMapper<{typeof(T)}>({this._map})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual dynamic noSuchMethod(global::Doroti.Runtime.Invocation invocation)
    {
        throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"There was an attempt to access the \"{invocation.memberName}\" " + $"field of a WidgetStateMapper<{typeof(T)}> object."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this}"), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("WidgetStateProperty objects should only be used " + "in places that document their support."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Double-check whether the map was used in a place that " + "documents support for WidgetStateProperty objects. If so, " + "please file a bug report. (The https://pub.dev/ page for a package " + "contains a link to \"View/report issues\".)") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<WidgetStateMap<T>>("map", this._map));
    }

    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class WidgetStatePropertyAll<T> : WidgetStateProperty<T>
{
    public virtual T value { get; private set; } = default!;

    public WidgetStatePropertyAll(T value)
    {
        this.value = value;
    }

    public virtual T resolve(HashSet<WidgetState> states) => this.value;
    public override string ToString()
    {
        if ((this.value is double))
        {
            double value__as40867 = (double)(object)value!;
            return $"WidgetStatePropertyAll({(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(((double)(object)this.value)))})";
        }
        else
        {
            return $"WidgetStatePropertyAll({this.value})";
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as WidgetStatePropertyAll<T>;
        if (__other is null) return false;
        return (((__other is WidgetStatePropertyAll<T>) && (object.Equals(DartRuntimePrimitives.RuntimeType(((WidgetStatePropertyAll<T>)__other)), this.GetType()))) && EqualityComparer<T>.Default.Equals(((WidgetStatePropertyAll<T>)((WidgetStatePropertyAll<T>)__other)).value, this.value));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(this.value.GetHashCode());
}

public class WidgetStatesController : global::Doroti.Generated.Framework.Foundation.ValueNotifier<HashSet<WidgetState>>
{
    public WidgetStatesController(HashSet<WidgetState>? value = null) : base(new HashSet<WidgetState>())
    {
    }

    public virtual void update(WidgetState state, bool add)
    {
        bool valueChanged__43279 = (add ? this.value.Add(state) : this.value.Remove(state));
        if (valueChanged__43279)
        {
            notifyListeners();
        }
    }

}
