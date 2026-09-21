// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/widget_state.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

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

    internal _WidgetStateCombo__widget_state(
        WidgetStatesConstraint first,
        WidgetStatesConstraint second
    )
    {
        this.first = first;
        this.second = second;
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(first, second));

    public virtual bool isSatisfiedBy(HashSet<WidgetState> states) =>
        throw new NotSupportedException();

    public virtual WidgetStatesConstraint op_BitwiseAnd(WidgetStatesConstraint other) =>
        DartRuntimePrimitives.ConvertValue<WidgetStatesConstraint>(
            new _WidgetStateAnd__widget_state(this, other)
        );

    public virtual WidgetStatesConstraint op_BitwiseOr(WidgetStatesConstraint other) =>
        DartRuntimePrimitives.ConvertValue<WidgetStatesConstraint>(
            new _WidgetStateOr__widget_state(this, other)
        );

    public virtual WidgetStatesConstraint op_OnesComplement() =>
        DartRuntimePrimitives.ConvertValue<WidgetStatesConstraint>(
            new _WidgetStateNot__widget_state(this)
        );
}

internal class _WidgetStateAnd__widget_state : _WidgetStateCombo__widget_state
{
    internal _WidgetStateAnd__widget_state(
        WidgetStatesConstraint first,
        WidgetStatesConstraint second
    )
        : base(first, second) { }

    public override bool isSatisfiedBy(HashSet<WidgetState> states)
    {
        return first.isSatisfiedBy(states) && second.isSatisfiedBy(states);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _WidgetStateAnd__widget_state;
        if (__other is null)
        {
            return false;
        }

        return (__other is _WidgetStateAnd__widget_state)
            && Equals(__other.first, first)
            && Equals(__other.second, second);
    }

    public override string ToString() => $"({first} & {second})";

    public override int GetHashCode() => base.GetHashCode();
}

internal class _WidgetStateOr__widget_state : _WidgetStateCombo__widget_state
{
    internal _WidgetStateOr__widget_state(
        WidgetStatesConstraint first,
        WidgetStatesConstraint second
    )
        : base(first, second) { }

    public override bool isSatisfiedBy(HashSet<WidgetState> states)
    {
        return first.isSatisfiedBy(states) || second.isSatisfiedBy(states);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _WidgetStateOr__widget_state;
        if (__other is null)
        {
            return false;
        }

        return (__other is _WidgetStateOr__widget_state)
            && Equals(__other.first, first)
            && Equals(__other.second, second);
    }

    public override string ToString() => $"({first} | {second})";

    public override int GetHashCode() => base.GetHashCode();
}

internal class _WidgetStateNot__widget_state : WidgetStatesConstraint
{
    public virtual WidgetStatesConstraint value { get; private set; } = default!;

    internal _WidgetStateNot__widget_state(WidgetStatesConstraint value)
    {
        this.value = value;
    }

    public virtual bool isSatisfiedBy(HashSet<WidgetState> states) => !value.isSatisfiedBy(states);

    public override bool Equals(object? other)
    {
        var __other = other as _WidgetStateNot__widget_state;
        if (__other is null)
        {
            return false;
        }

        return (__other is _WidgetStateNot__widget_state) && Equals(__other.value, value);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(value?.GetHashCode() ?? 0);

    public override string ToString() => $"~{value}";

    public virtual WidgetStatesConstraint op_BitwiseAnd(WidgetStatesConstraint other) =>
        DartRuntimePrimitives.ConvertValue<WidgetStatesConstraint>(
            new _WidgetStateAnd__widget_state(this, other)
        );

    public virtual WidgetStatesConstraint op_BitwiseOr(WidgetStatesConstraint other) =>
        DartRuntimePrimitives.ConvertValue<WidgetStatesConstraint>(
            new _WidgetStateOr__widget_state(this, other)
        );

    public virtual WidgetStatesConstraint op_OnesComplement() =>
        DartRuntimePrimitives.ConvertValue<WidgetStatesConstraint>(
            new _WidgetStateNot__widget_state(this)
        );
}

internal class _AnyWidgetStates__widget_state : WidgetStatesConstraint
{
    internal _AnyWidgetStates__widget_state() { }

    public virtual bool isSatisfiedBy(HashSet<WidgetState> states) => true;

    public override string ToString() => "WidgetState.any";

    public virtual WidgetStatesConstraint op_BitwiseAnd(WidgetStatesConstraint other) =>
        DartRuntimePrimitives.ConvertValue<WidgetStatesConstraint>(
            new _WidgetStateAnd__widget_state(this, other)
        );

    public virtual WidgetStatesConstraint op_BitwiseOr(WidgetStatesConstraint other) =>
        DartRuntimePrimitives.ConvertValue<WidgetStatesConstraint>(
            new _WidgetStateOr__widget_state(this, other)
        );

    public virtual WidgetStatesConstraint op_OnesComplement() =>
        DartRuntimePrimitives.ConvertValue<WidgetStatesConstraint>(
            new _WidgetStateNot__widget_state(this)
        );
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
    error,
}

public static class WidgetStateMembers
{
    public static bool isSatisfiedBy(this WidgetState value, HashSet<WidgetState> states) =>
        states.Contains(value);

    public static WidgetStatesConstraint asConstraint(this WidgetState value) =>
        new _SingleWidgetStateConstraint(value);

    public static WidgetStatesConstraint any => new _AnyWidgetStates__widget_state();

    private sealed class _SingleWidgetStateConstraint(WidgetState value) : WidgetStatesConstraint
    {
        public bool isSatisfiedBy(HashSet<WidgetState> states) => states.Contains(value);

        public WidgetStatesConstraint op_BitwiseAnd(WidgetStatesConstraint other) =>
            new _WidgetStateAnd__widget_state(this, other);

        public WidgetStatesConstraint op_BitwiseOr(WidgetStatesConstraint other) =>
            new _WidgetStateOr__widget_state(this, other);

        public WidgetStatesConstraint op_OnesComplement() =>
            new _WidgetStateNot__widget_state(this);

        public override string ToString() => $"WidgetState.{value}";
    }
}

public delegate T WidgetPropertyResolver<T>(HashSet<WidgetState> states);

public abstract class WidgetStateColor : Color, WidgetStateProperty<Color>
{
    public static WidgetStateColor transparent = new _WidgetStateColorTransparent__widget_state();

    protected WidgetStateColor(long defaultValue)
        : base(defaultValue) { }

    public static WidgetStateColor CreateResolveWith(Func<HashSet<WidgetState>, Color> callback) =>
        new _WidgetStateColor__widget_state(callback);

    public static WidgetStateColor CreateFromMap(DartMap<WidgetStatesConstraint, Color> map) =>
        new _WidgetStateColorMapper__widget_state(map);

    public abstract Color resolve(HashSet<WidgetState> states);
}

internal class _WidgetStateColor__widget_state : WidgetStateColor
{
    internal virtual Func<HashSet<WidgetState>, Color> _resolve { get; private set; } = default!;
    internal static HashSet<WidgetState> _defaultStates = new HashSet<WidgetState>();

    internal _WidgetStateColor__widget_state(Func<HashSet<WidgetState>, Color> _resolve)
        : base(_resolve(_defaultStates).value)
    {
        this._resolve = _resolve;
    }

    public override Color resolve(HashSet<WidgetState> states) => _resolve(states);
}

internal class _WidgetStateColorTransparent__widget_state : WidgetStateColor
{
    internal _WidgetStateColorTransparent__widget_state()
        : base(0L) { }

    public override Color resolve(HashSet<WidgetState> states) => new Color(0L);
}

public abstract class WidgetStateMouseCursor : MouseCursor, WidgetStateProperty<MouseCursor>
{
    public static WidgetStateMouseCursor clickable = CreateResolveWith(
        _clickable,
        debugDescription: "WidgetStateMouseCursor(clickable)"
    );
    public static WidgetStateMouseCursor adaptiveClickable = CreateResolveWith(
        _adaptiveClickable,
        debugDescription: "WidgetStateMouseCursor(adaptiveClickable)"
    );
    public static WidgetStateMouseCursor textable = CreateResolveWith(
        _textable,
        debugDescription: "WidgetStateMouseCursor(textable)"
    );

    protected WidgetStateMouseCursor() { }

    public static WidgetStateMouseCursor CreateResolveWith(
        Func<HashSet<WidgetState>, MouseCursor> callback,
        string debugDescription = default!
    ) => new _WidgetStateMouseCursor__widget_state(callback, debugDescription);

    public static WidgetStateMouseCursor CreateFromMap(
        DartMap<WidgetStatesConstraint, MouseCursor> map
    ) => new _WidgetMouseCursorMapper__widget_state(map);

    public override MouseCursorSession createSession(long device)
    {
        return resolve(new HashSet<WidgetState>()).createSession(device);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static WidgetStateMouseCursor CreateFromMap(DartMap<WidgetState, MouseCursor> map) =>
        CreateFromMap(WidgetStateMapAdapters.toConstraints(map));

    public abstract MouseCursor resolve(HashSet<WidgetState> states);

    internal static MouseCursor _clickable(HashSet<WidgetState> states)
    {
        if (states.Contains(WidgetState.disabled))
        {
            return SystemMouseCursors.basic;
        }
        return SystemMouseCursors.click;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static MouseCursor _adaptiveClickable(HashSet<WidgetState> states)
    {
        if (states.Contains(WidgetState.disabled))
        {
            return SystemMouseCursors.basic;
        }
        return Foundation.ConstantsLibrary.kIsWeb
            ? SystemMouseCursors.click
            : SystemMouseCursors.basic;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static MouseCursor _textable(HashSet<WidgetState> states)
    {
        if (states.Contains(WidgetState.disabled))
        {
            return SystemMouseCursors.basic;
        }
        return SystemMouseCursors.text;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _WidgetStateMouseCursor__widget_state : WidgetStateMouseCursor
{
    internal virtual Func<HashSet<WidgetState>, MouseCursor> _resolve { get; private set; } =
        default!;
    private string __field_debugDescription = default!;
    public override string debugDescription
    {
        get => __field_debugDescription;
    }

    internal _WidgetStateMouseCursor__widget_state(
        Func<HashSet<WidgetState>, MouseCursor> _resolve,
        string debugDescription = "WidgetStateMouseCursor()"
    )
    {
        this._resolve = _resolve;
        __field_debugDescription = debugDescription;
    }

    public override MouseCursor resolve(HashSet<WidgetState> states) => _resolve(states);
}

public abstract class WidgetStateBorderSide : BorderSide, WidgetStateProperty<BorderSide?>
{
    protected WidgetStateBorderSide() { }

    public static WidgetStateBorderSide CreateResolveWith(
        Func<HashSet<WidgetState>, BorderSide?> callback
    ) => new _WidgetStateBorderSide__widget_state(callback);

    public static WidgetStateBorderSide CreateFromMap(
        DartMap<WidgetStatesConstraint, BorderSide?> map
    ) => new _WidgetBorderSideMapper__widget_state(map);

    public abstract BorderSide? resolve(HashSet<WidgetState> states);

    public static WidgetStateProperty<BorderSide?>? lerp(
        WidgetStateProperty<BorderSide?>? a,
        WidgetStateProperty<BorderSide?>? b,
        double t
    )
    {
        if ((a is null) && (b is null))
        {
            return null;
        }
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return (WidgetStateProperty<BorderSide?>?)new _LerpSides__widget_state(a, b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _LerpSides__widget_state : WidgetStateProperty<BorderSide?>
{
    public virtual WidgetStateProperty<BorderSide?>? a { get; private set; }
    public virtual WidgetStateProperty<BorderSide?>? b { get; private set; }
    public virtual double t { get; private set; } = default!;

    internal _LerpSides__widget_state(
        WidgetStateProperty<BorderSide?>? a,
        WidgetStateProperty<BorderSide?>? b,
        double t
    )
    {
        this.a = a;
        this.b = b;
        this.t = t;
    }

    public virtual BorderSide? resolve(HashSet<WidgetState> states)
    {
        BorderSide? resolvedA = a?.resolve(states);
        BorderSide? resolvedB = b?.resolve(states);
        if ((resolvedA is null) && (resolvedB is null))
        {
            return null;
        }
        if (resolvedA is null)
        {
            return (BorderSide?)
                BorderSide.lerp(
                    new BorderSide(width: 0, color: resolvedB!.color.withAlpha(0L)),
                    resolvedB,
                    t
                );
        }
        if (resolvedB is null)
        {
            return (BorderSide?)
                BorderSide.lerp(
                    resolvedA,
                    new BorderSide(width: 0, color: resolvedA.color.withAlpha(0L)),
                    t
                );
        }
        return (BorderSide?)BorderSide.lerp(resolvedA, resolvedB, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _WidgetStateBorderSide__widget_state : WidgetStateBorderSide
{
    internal virtual Func<HashSet<WidgetState>, BorderSide?> _resolve { get; private set; } =
        default!;

    internal _WidgetStateBorderSide__widget_state(Func<HashSet<WidgetState>, BorderSide?> _resolve)
    {
        this._resolve = _resolve;
    }

    public override BorderSide? resolve(HashSet<WidgetState> states) => _resolve(states);
}

public abstract class WidgetStateOutlinedBorder
    : OutlinedBorder,
        WidgetStateProperty<OutlinedBorder?>
{
    protected WidgetStateOutlinedBorder() { }

    public static WidgetStateOutlinedBorder CreateResolveWith(
        Func<HashSet<WidgetState>, OutlinedBorder?> callback
    ) => new _WidgetStateOutlinedBorder__widget_state(callback);

    public static WidgetStateOutlinedBorder CreateFromMap(
        DartMap<WidgetStatesConstraint, OutlinedBorder?> map
    ) => new _WidgetOutlinedBorderMapper__widget_state(map);

    public abstract OutlinedBorder? resolve(HashSet<WidgetState> states);
}

public abstract class WidgetStateTextStyle : TextStyle, WidgetStateProperty<TextStyle>
{
    protected WidgetStateTextStyle() { }

    public static WidgetStateTextStyle CreateResolveWith(
        Func<HashSet<WidgetState>, TextStyle> callback
    ) => new _WidgetStateTextStyle__widget_state(callback);

    public static WidgetStateTextStyle CreateFromMap(
        DartMap<WidgetStatesConstraint, TextStyle> map
    ) => new _WidgetTextStyleMapper__widget_state(map);

    public abstract TextStyle resolve(HashSet<WidgetState> states);
}

internal class _WidgetStateTextStyle__widget_state : WidgetStateTextStyle
{
    internal virtual Func<HashSet<WidgetState>, TextStyle> _resolve { get; private set; } =
        default!;

    internal _WidgetStateTextStyle__widget_state(Func<HashSet<WidgetState>, TextStyle> _resolve)
    {
        this._resolve = _resolve;
    }

    public override TextStyle resolve(HashSet<WidgetState> states) => _resolve(states);
}

public static class WidgetStateProperty
{
    public static T resolveAs<T>(T value, HashSet<WidgetState> states) =>
        WidgetStateProperty<T>.resolveAs(value, states);

    public static WidgetStateProperty<T> resolveWith<T>(Func<HashSet<WidgetState>, T> callback) =>
        WidgetStateProperty<T>.resolveWith(callback);

    public static WidgetStateProperty<T> all<T>(T value) => WidgetStateProperty<T>.all(value);

    public static WidgetStateProperty<T?>? lerp<T>(
        WidgetStateProperty<T>? a,
        WidgetStateProperty<T>? b,
        double t,
        Func<T?, T?, double, T?> lerpFunction
    ) => WidgetStateProperty<T>.lerp(a, b, t, lerpFunction);
}

public interface WidgetStateProperty<out T>
{
    public static WidgetStateProperty<T> CreateFromMap(DartMap<WidgetStatesConstraint, T> map) =>
        new WidgetStateMapper<T>(map);

    public static WidgetStateProperty<T> CreateFromMap(DartMap<WidgetState, T> map) =>
        CreateFromMap(WidgetStateMapAdapters.toConstraints(map));

    public static TValue resolveAs<TValue>(TValue value, HashSet<WidgetState> states)
    {
        if (value is WidgetStateProperty<TValue>)
        {
            WidgetStateProperty<TValue> value__as32591 = (WidgetStateProperty<TValue>)value;
            WidgetStateProperty<TValue> @property = value__as32591;
            return @property.resolve(states);
        }
        return value;
    }
    public static WidgetStateProperty<TValue> resolveWith<TValue>(
        Func<HashSet<WidgetState>, TValue> callback
    ) =>
        DartRuntimePrimitives.ConvertValue<WidgetStateProperty<TValue>>(
            new _WidgetStatePropertyWith__widget_state<TValue>(callback)
        );
    public static WidgetStateProperty<TValue> all<TValue>(TValue value) =>
        DartRuntimePrimitives.ConvertValue<WidgetStateProperty<TValue>>(
            new WidgetStatePropertyAll<TValue>(value)
        );
    public static WidgetStateProperty<TValue?>? lerp<TValue>(
        WidgetStateProperty<TValue>? a,
        WidgetStateProperty<TValue>? b,
        double t,
        Func<TValue?, TValue?, double, TValue?> lerpFunction
    )
    {
        if ((a is null) && (b is null))
        {
            return default;
        }
        return (WidgetStateProperty<TValue?>?)
            new _LerpProperties__widget_state<TValue>(a, b, t, lerpFunction);
    }
    public T resolve(HashSet<WidgetState> states);
}

internal static class WidgetStateMapAdapters
{
    internal static DartMap<WidgetStatesConstraint, T> toConstraints<T>(DartMap<WidgetState, T> map)
    {
        var result = new DartMap<WidgetStatesConstraint, T>();
        foreach (var entry in map)
        {
            result[entry.Key.asConstraint()] = entry.Value;
        }

        return result;
    }
}

internal class _LerpProperties__widget_state<T> : WidgetStateProperty<T?>
{
    public virtual WidgetStateProperty<T>? a { get; private set; }
    public virtual WidgetStateProperty<T>? b { get; private set; }
    public virtual double t { get; private set; } = default!;
    public virtual Func<T?, T?, double, T?> lerpFunction { get; private set; } = default!;

    internal _LerpProperties__widget_state(
        WidgetStateProperty<T>? a,
        WidgetStateProperty<T>? b,
        double t,
        Func<T?, T?, double, T?> lerpFunction
    )
    {
        this.a = a;
        this.b = b;
        this.t = t;
        this.lerpFunction = lerpFunction;
    }

    public virtual T? resolve(HashSet<WidgetState> states)
    {
        T? resolvedA = DartRuntimePrimitives.NullAware(a, __target => __target.resolve(states));
        T? resolvedB = DartRuntimePrimitives.NullAware(b, __target => __target.resolve(states));
        return lerpFunction(resolvedA, resolvedB, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _WidgetStatePropertyWith__widget_state<T> : WidgetStateProperty<T>
{
    internal virtual Func<HashSet<WidgetState>, T> _resolve { get; private set; } = default!;

    internal _WidgetStatePropertyWith__widget_state(Func<HashSet<WidgetState>, T> _resolve)
    {
        this._resolve = _resolve;
    }

    public virtual T resolve(HashSet<WidgetState> states) => _resolve(states);
}

public delegate void WidgetStateMap<T>();

public class WidgetStateMapper<T> : Diagnosticable, WidgetStateProperty<T>, IWidgetStateMapping<T>
{
    internal virtual DartMap<WidgetStatesConstraint, T> _map { get; private set; } = default!;

    DartMap<WidgetStatesConstraint, T> IWidgetStateMapping<T>.Mapping => _map;

    public WidgetStateMapper(DartMap<WidgetStatesConstraint, T> map)
    {
        _map = map;
    }

    public virtual T resolve(HashSet<WidgetState> states)
    {
        foreach (MapEntry<WidgetStatesConstraint, T> entry in _map.entries)
        {
            if (entry.key.isSatisfiedBy(states))
            {
                return entry.value;
            }
        }
        try
        {
            return ((T?)(object?)null)!;
        }
        catch (TypeError)
        {
            throw DartRuntimePrimitives.AsException(
                new DartArgumentError(
                    $"The current set of widget states is {states}.\n"
                        + "None of the provided map keys matched this set, "
                        + $"and the type \"{typeof(T)}\" is non-nullable.\n"
                        + $"Consider using \"WidgetStateMapper<{typeof(T)}?>()\", "
                        + "or adding the \"WidgetState.any\" key to this map."
                )
            );
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other) =>
        other is IWidgetStateMapping<T> mapping
        && CollectionsLibrary.mapEquals(_map, mapping.Mapping);

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            new MapEquality<WidgetStatesConstraint, T>().hash(_map)
        );

    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        return $"WidgetStateMapper<{typeof(T)}>({_map})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual dynamic noSuchMethod(Invocation invocation)
    {
        throw DartRuntimePrimitives.AsException(
            new FlutterError(
                new List<DiagnosticsNode>
                {
                    new ErrorSummary(
                        $"There was an attempt to access the \"{invocation.memberName}\" "
                            + $"field of a WidgetStateMapper<{typeof(T)}> object."
                    ),
                    new ErrorDescription($"{this}"),
                    new ErrorDescription(
                        "WidgetStateProperty objects should only be used "
                            + "in places that document their support."
                    ),
                    new ErrorHint(
                        "Double-check whether the map was used in a place that "
                            + "documents support for WidgetStateProperty objects. If so, "
                            + "please file a bug report. (The https://pub.dev/ page for a package "
                            + "contains a link to \"View/report issues\".)"
                    ),
                }
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new DiagnosticsProperty<WidgetStateMap<T>>("map", _map));
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);

    public virtual DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = null
    )
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
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

    public virtual T resolve(HashSet<WidgetState> states) => value;

    public override string ToString()
    {
        if (value is double)
        {
            double value__as40867 = (double)(object)value!;
            return $"WidgetStatePropertyAll({Foundation.DebugLibrary.debugFormatDouble((double)(object)value)})";
        }
        else
        {
            return $"WidgetStatePropertyAll({value})";
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as WidgetStatePropertyAll<T>;
        if (__other is null)
        {
            return false;
        }

        return (__other is WidgetStatePropertyAll<T>)
            && Equals(DartRuntimePrimitives.RuntimeType(__other), GetType())
            && EqualityComparer<T>.Default.Equals(__other.value, value);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(value?.GetHashCode() ?? 0);
}

public class WidgetStatesController : ValueNotifier<HashSet<WidgetState>>
{
    public WidgetStatesController(HashSet<WidgetState>? value = null)
        : base(new HashSet<WidgetState>()) { }

    public virtual void update(WidgetState state, bool add)
    {
        bool valueChanged = add ? value.Add(state) : value.Remove(state);
        if (valueChanged)
        {
            notifyListeners();
        }
    }
}
