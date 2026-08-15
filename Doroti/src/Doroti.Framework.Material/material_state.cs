// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/material_state.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public delegate void MaterialState();

public delegate T MaterialPropertyResolver<T>(HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> states);

public delegate void MaterialStateColor();

public delegate void MaterialStateMouseCursor();

public delegate void MaterialStateBorderSide();

public delegate void MaterialStateOutlinedBorder();

public delegate void MaterialStateTextStyle();

public abstract class MaterialStateOutlineInputBorder : OutlineInputBorder, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<InputBorder>
{
    protected MaterialStateOutlineInputBorder()
    {
    }

    public static MaterialStateOutlineInputBorder CreateResolveWith(global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, InputBorder> callback)
        => ((MaterialStateOutlineInputBorder)(object?)new _MaterialStateOutlineInputBorder__material_state(callback));

    public abstract InputBorder resolve(HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> states);
}

internal class _MaterialStateOutlineInputBorder__material_state : MaterialStateOutlineInputBorder
{
    internal virtual global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, InputBorder> _resolve { get; private set; } = default!;

    internal _MaterialStateOutlineInputBorder__material_state(global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, InputBorder> _resolve)
    {
        this._resolve = _resolve;
    }

    public override InputBorder resolve(HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> states) => this._resolve(states);
}

public abstract class MaterialStateUnderlineInputBorder : UnderlineInputBorder, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<InputBorder>
{
    protected MaterialStateUnderlineInputBorder()
    {
    }

    public static MaterialStateUnderlineInputBorder CreateResolveWith(global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, InputBorder> callback)
        => ((MaterialStateUnderlineInputBorder)(object?)new _MaterialStateUnderlineInputBorder__material_state(callback));

    public abstract InputBorder resolve(HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> states);
}

internal class _MaterialStateUnderlineInputBorder__material_state : MaterialStateUnderlineInputBorder
{
    internal virtual global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, InputBorder> _resolve { get; private set; } = default!;

    internal _MaterialStateUnderlineInputBorder__material_state(global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, InputBorder> _resolve)
    {
        this._resolve = _resolve;
    }

    public override InputBorder resolve(HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> states) => this._resolve(states);
}

public abstract class WidgetStateInputBorder : InputBorder, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<InputBorder>
{
    public WidgetStateInputBorder() { }

    public static WidgetStateInputBorder CreateResolveWith(global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, InputBorder> callback)
        => ((WidgetStateInputBorder)(object?)new _WidgetStateInputBorder__material_state(callback));

    public static WidgetStateInputBorder CreateFromMap(DartMap<global::Doroti.Generated.Framework.Widgets.WidgetStatesConstraint, InputBorder> map)
        => ((WidgetStateInputBorder)(object?)new _WidgetInputBorderMapper__material_state(map));

    public virtual InputBorder resolve(HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> states) => throw new NotSupportedException();
}

internal class _WidgetStateInputBorder__material_state : OutlineInputBorder
{
    internal virtual global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, InputBorder> _resolve { get; private set; } = default!;

    internal _WidgetStateInputBorder__material_state(global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, InputBorder> _resolve)
    {
        this._resolve = _resolve;
    }

    public virtual InputBorder resolve(HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> states) => this._resolve(states);
}

internal class _WidgetInputBorderMapper__material_state : global::Doroti.Generated.Framework.Widgets.WidgetStateMapper<InputBorder>
{
    internal _WidgetInputBorderMapper__material_state(DartMap<global::Doroti.Generated.Framework.Widgets.WidgetStatesConstraint, InputBorder> map) : base(map)
    {
    }

}
