// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/grid_tile.dart
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

public class GridTile : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? header { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? footer { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;

    public GridTile(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget? header = null, global::Doroti.Generated.Framework.Widgets.Widget? footer = null, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key)
    {
        this.header = header;
        this.footer = footer;
        this.child = child;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        if (((this.header is null) && (this.footer is null)))
        {
            return this.child;
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Stack(children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection1501 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection1501.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(global::Doroti.Generated.Framework.Widgets.Positioned.CreateFill(child: this.child))); if ((this.header is not null)) { __collection1501.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Positioned(top: 0.0, left: 0.0, right: 0.0, child: this.header!))); } if ((this.footer is not null)) { __collection1501.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Positioned(left: 0.0, bottom: 0.0, right: 0.0, child: this.footer!))); } return __collection1501; }))()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
