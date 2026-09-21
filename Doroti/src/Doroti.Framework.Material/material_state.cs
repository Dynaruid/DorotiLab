// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/material_state.dart

using Doroti.Runtime;

namespace Doroti.Framework.Material;

public delegate void MaterialState();

public delegate T MaterialPropertyResolver<T>(HashSet<WidgetState> states);

public delegate void MaterialStateColor();

public delegate void MaterialStateMouseCursor();

public delegate void MaterialStateBorderSide();

public delegate void MaterialStateOutlinedBorder();

public delegate void MaterialStateTextStyle();

public abstract class MaterialStateOutlineInputBorder
    : OutlineInputBorder,
        WidgetStateProperty<InputBorder>
{
    protected MaterialStateOutlineInputBorder() { }

    public static MaterialStateOutlineInputBorder CreateResolveWith(
        Func<HashSet<WidgetState>, InputBorder> callback
    ) => new _MaterialStateOutlineInputBorder__material_state(callback);

    public abstract InputBorder resolve(HashSet<WidgetState> states);
}

internal class _MaterialStateOutlineInputBorder__material_state : MaterialStateOutlineInputBorder
{
    internal virtual Func<HashSet<WidgetState>, InputBorder> _resolve { get; private set; } =
        default!;

    internal _MaterialStateOutlineInputBorder__material_state(
        Func<HashSet<WidgetState>, InputBorder> _resolve
    )
    {
        this._resolve = _resolve;
    }

    public override InputBorder resolve(HashSet<WidgetState> states) => _resolve(states);
}

public abstract class MaterialStateUnderlineInputBorder
    : UnderlineInputBorder,
        WidgetStateProperty<InputBorder>
{
    protected MaterialStateUnderlineInputBorder() { }

    public static MaterialStateUnderlineInputBorder CreateResolveWith(
        Func<HashSet<WidgetState>, InputBorder> callback
    ) => new _MaterialStateUnderlineInputBorder__material_state(callback);

    public abstract InputBorder resolve(HashSet<WidgetState> states);
}

internal class _MaterialStateUnderlineInputBorder__material_state
    : MaterialStateUnderlineInputBorder
{
    internal virtual Func<HashSet<WidgetState>, InputBorder> _resolve { get; private set; } =
        default!;

    internal _MaterialStateUnderlineInputBorder__material_state(
        Func<HashSet<WidgetState>, InputBorder> _resolve
    )
    {
        this._resolve = _resolve;
    }

    public override InputBorder resolve(HashSet<WidgetState> states) => _resolve(states);
}

public abstract class WidgetStateInputBorder : InputBorder, WidgetStateProperty<InputBorder>
{
    public WidgetStateInputBorder() { }

    public static WidgetStateInputBorder CreateResolveWith(
        Func<HashSet<WidgetState>, InputBorder> callback
    ) => new _WidgetStateInputBorder__material_state(callback);

    public static WidgetStateInputBorder CreateFromMap(
        DartMap<WidgetStatesConstraint, InputBorder> map
    ) => new _WidgetInputBorderMapper__material_state(map);

    public virtual InputBorder resolve(HashSet<WidgetState> states) =>
        throw new NotSupportedException();
}
