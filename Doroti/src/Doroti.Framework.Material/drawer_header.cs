// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/drawer_header.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public static partial class Drawer_headerLibrary
{
    internal static double _kDrawerHeaderHeight = (160.0 + 1.0);
}

public class DrawerHeader : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Painting.Decoration? decoration { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin { get; private set; }
    public virtual Duration duration { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Curve curve { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }

    public DrawerHeader(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.Decoration? decoration = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin = default!, global::Doroti.Framework.Painting.EdgeInsetsGeometry padding = default!, Duration? duration = null, global::Doroti.Framework.Animation.Curve curve = default!, global::Doroti.Framework.Widgets.Widget? child = default!) : base(key: key)
    {
        global::Doroti.Framework.Painting.EdgeInsetsGeometry? __margin = margin ?? global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(bottom: 8.0);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __padding = padding ?? new global::Doroti.Framework.Painting.EdgeInsets(16.0, 16.0, 16.0, 8.0);
        Duration __duration = duration ?? Duration.Create(milliseconds: 250);
        global::Doroti.Framework.Animation.Curve __curve = curve ?? global::Doroti.Framework.Animation.Curves.fastOutSlowIn;
        this.decoration = decoration;
        this.margin = __margin;
        this.padding = __padding;
        this.duration = __duration;
        this.curve = __curve;
        this.child = child;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        ThemeData theme__2776 = Theme.of(context);
        double statusBarHeight__2820 = MediaQuery.paddingOf(context).top;
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Container(height: (statusBarHeight__2820 + Drawer_headerLibrary._kDrawerHeaderHeight), margin: this.margin, decoration: new global::Doroti.Framework.Painting.BoxDecoration(border: new global::Doroti.Framework.Painting.Border(bottom: Divider.createBorderSide(context))), child: new global::Doroti.Framework.Widgets.AnimatedContainer(padding: this.padding.add(global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(top: statusBarHeight__2820)), decoration: this.decoration, duration: DartRuntimePrimitives.RequireValue(this.duration), curve: this.curve, child: ((this.child is null) ? null : new global::Doroti.Framework.Widgets.DefaultTextStyle(style: theme__2776.textTheme.bodyLarge!, child: global::Doroti.Framework.Widgets.MediaQuery.CreateRemovePadding(context: context, removeTop: true, child: this.child!))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
