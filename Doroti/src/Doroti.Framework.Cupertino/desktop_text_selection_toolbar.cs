// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/desktop_text_selection_toolbar.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Cupertino;

public static partial class Desktop_text_selection_toolbarLibrary
{
    internal static double _kToolbarScreenPadding = 8.0;
}

public static partial class Desktop_text_selection_toolbarLibrary
{
    internal static double _kToolbarSaturationBoost = 3;
}

public static partial class Desktop_text_selection_toolbarLibrary
{
    internal static double _kToolbarBlurSigma = 20;
}

public static partial class Desktop_text_selection_toolbarLibrary
{
    internal static double _kToolbarWidth = 222.0;
}

public static partial class Desktop_text_selection_toolbarLibrary
{
    internal static Radius _kToolbarBorderRadius = global::Doroti.Ui.Radius.circular(8.0);
}

public static partial class Desktop_text_selection_toolbarLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsets _kToolbarPadding = global::Doroti.Framework.Painting.EdgeInsets.CreateAll(6.0);
}

public static partial class Desktop_text_selection_toolbarLibrary
{
    internal static List<global::Doroti.Framework.Painting.BoxShadow> _kToolbarShadow = new List<global::Doroti.Framework.Painting.BoxShadow> { new global::Doroti.Framework.Painting.BoxShadow(color: global::Doroti.Ui.Color.fromARGB(60L, 0L, 0L, 0L), blurRadius: 10.0, spreadRadius: 0.5, offset: new global::Doroti.Ui.Offset(0.0, 4.0)) };
}

public static partial class Desktop_text_selection_toolbarLibrary
{
    internal static CupertinoDynamicColor _kToolbarBorderColor = new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(4290295992L), darkColor: new global::Doroti.Ui.Color(4284177243L));
}

public static partial class Desktop_text_selection_toolbarLibrary
{
    internal static CupertinoDynamicColor _kToolbarBackgroundColor = new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(3003121663L), darkColor: new global::Doroti.Ui.Color(2989502512L));
}

public class CupertinoDesktopTextSelectionToolbar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual Offset anchor { get; private set; } = default!;
    public virtual List<global::Doroti.Framework.Widgets.Widget> children { get; private set; } = default!;

    public CupertinoDesktopTextSelectionToolbar(global::Doroti.Framework.Foundation.Key? key = null, Offset anchor = default!, List<global::Doroti.Framework.Widgets.Widget> children = default!) : base(key: key)
    {
        this.anchor = anchor;
        this.children = children;
        System.Diagnostics.Debug.Assert((checked((long)(children.Count)) > 0L));
    }

    internal static List<double> _matrixWithSaturation(double saturation)
    {
        double r__2743 = (0.213 * ((1L - saturation)));
        double g__2790 = (0.715 * ((1L - saturation)));
        double b__2837 = (0.072 * ((1L - saturation)));
        return new List<double> { (r__2743 + saturation), g__2790, b__2837, 0, 0, r__2743, (g__2790 + saturation), b__2837, 0, 0, r__2743, g__2790, (b__2837 + saturation), 0, 0, 0, 0, 0, 1, 0 };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Framework.Widgets.Widget _defaultToolbarBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Container(width: Desktop_text_selection_toolbarLibrary._kToolbarWidth, clipBehavior: Clip.hardEdge, decoration: new global::Doroti.Framework.Painting.ShapeDecoration(shadows: Desktop_text_selection_toolbarLibrary._kToolbarShadow, shape: new global::Doroti.Framework.Painting.RoundedSuperellipseBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(Desktop_text_selection_toolbarLibrary._kToolbarBorderRadius))), child: new global::Doroti.Framework.Widgets.BackdropFilter(filter: new global::Doroti.Ui.ImageFilter(outer: global::Doroti.Ui.ColorFilter.matrix(CupertinoDesktopTextSelectionToolbar._matrixWithSaturation(Desktop_text_selection_toolbarLibrary._kToolbarSaturationBoost)), inner: new global::Doroti.Ui.ImageFilter(sigmaX: Desktop_text_selection_toolbarLibrary._kToolbarBlurSigma, sigmaY: Desktop_text_selection_toolbarLibrary._kToolbarBlurSigma)), child: new global::Doroti.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Framework.Painting.ShapeDecoration(color: Desktop_text_selection_toolbarLibrary._kToolbarBackgroundColor.resolveFrom(context), shape: new global::Doroti.Framework.Painting.RoundedSuperellipseBorder(side: new global::Doroti.Framework.Painting.BorderSide(color: Desktop_text_selection_toolbarLibrary._kToolbarBorderColor.resolveFrom(context)), borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(Desktop_text_selection_toolbarLibrary._kToolbarBorderRadius))), child: new global::Doroti.Framework.Widgets.Padding(padding: Desktop_text_selection_toolbarLibrary._kToolbarPadding, child: child)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        double paddingAbove__4682 = (MediaQuery.paddingOf(context).top + Desktop_text_selection_toolbarLibrary._kToolbarScreenPadding);
        var localAdjustment__4767 = new global::Doroti.Ui.Offset(Desktop_text_selection_toolbarLibrary._kToolbarScreenPadding, paddingAbove__4682);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: new global::Doroti.Framework.Painting.EdgeInsets(Desktop_text_selection_toolbarLibrary._kToolbarScreenPadding, paddingAbove__4682, Desktop_text_selection_toolbarLibrary._kToolbarScreenPadding, Desktop_text_selection_toolbarLibrary._kToolbarScreenPadding), child: new global::Doroti.Framework.Widgets.CustomSingleChildLayout(@delegate: new global::Doroti.Framework.Widgets.DesktopTextSelectionToolbarLayoutDelegate(anchor: (this.anchor - localAdjustment__4767)), child: CupertinoDesktopTextSelectionToolbar._defaultToolbarBuilder(context, new global::Doroti.Framework.Widgets.Column(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, children: this.children)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
