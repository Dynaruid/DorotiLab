// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/desktop_text_selection_toolbar.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public static partial class Desktop_text_selection_toolbarLibrary
{
    internal static double _kToolbarScreenPadding = 8.0;
}

public static partial class Desktop_text_selection_toolbarLibrary
{
    internal static double _kToolbarWidth = 222.0;
}

public class DesktopTextSelectionToolbar : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual Offset anchor { get; private set; } = default!;
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget> children { get; private set; } = default!;

    public DesktopTextSelectionToolbar(global::Doroti.Generated.Framework.Foundation.Key? key = null, Offset anchor = default!, List<global::Doroti.Generated.Framework.Widgets.Widget> children = default!) : base(key: key)
    {
        this.anchor = anchor;
        this.children = children;
        System.Diagnostics.Debug.Assert((checked((long)(children.Count)) > 0L));
    }

    internal static global::Doroti.Generated.Framework.Widgets.Widget _defaultToolbarBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.SizedBox(width: Desktop_text_selection_toolbarLibrary._kToolbarWidth, child: new Material(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Flutter.Ui.Radius.circular(7.0)), clipBehavior: Clip.antiAlias, elevation: 1.0, type: MaterialType.card, child: child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        double paddingAbove__2303 = (MediaQuery.paddingOf(context).top + Desktop_text_selection_toolbarLibrary._kToolbarScreenPadding);
        var localAdjustment__2388 = new global::Doroti.Flutter.Ui.Offset(Desktop_text_selection_toolbarLibrary._kToolbarScreenPadding, paddingAbove__2303);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Padding(padding: new global::Doroti.Generated.Framework.Painting.EdgeInsets(Desktop_text_selection_toolbarLibrary._kToolbarScreenPadding, paddingAbove__2303, Desktop_text_selection_toolbarLibrary._kToolbarScreenPadding, Desktop_text_selection_toolbarLibrary._kToolbarScreenPadding), child: new global::Doroti.Generated.Framework.Widgets.CustomSingleChildLayout(@delegate: new global::Doroti.Generated.Framework.Widgets.DesktopTextSelectionToolbarLayoutDelegate(anchor: (this.anchor - localAdjustment__2388)), child: DesktopTextSelectionToolbar._defaultToolbarBuilder(context, new global::Doroti.Generated.Framework.Widgets.Column(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, children: this.children)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
