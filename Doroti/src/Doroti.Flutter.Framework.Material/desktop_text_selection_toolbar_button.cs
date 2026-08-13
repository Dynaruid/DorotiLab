// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/desktop_text_selection_toolbar_button.dart
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

public static partial class Desktop_text_selection_toolbar_buttonLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.TextStyle _kToolbarButtonFontStyle = new global::Doroti.Generated.Framework.Painting.TextStyle(inherit: false, fontSize: 14.0, letterSpacing: -0.15, fontWeight: FontWeight.w400);
}

public static partial class Desktop_text_selection_toolbar_buttonLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.EdgeInsets _kToolbarButtonPadding = new global::Doroti.Generated.Framework.Painting.EdgeInsets(20.0, 0.0, 20.0, 3.0);
}

public class DesktopTextSelectionToolbarButton : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;

    public DesktopTextSelectionToolbarButton(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Action? onPressed = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key)
    {
        this.onPressed = onPressed;
        this.child = child;
    }

    public static DesktopTextSelectionToolbarButton CreateText(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.BuildContext context = default!, global::System.Action? onPressed = default!, string text = default!)
    {
        var __instance = new DesktopTextSelectionToolbarButton(key: key, onPressed: onPressed, child: default!);
        __instance.onPressed = onPressed;
        __instance.child = new global::Doroti.Generated.Framework.Widgets.Text(text, overflow: global::Doroti.Generated.Framework.Painting.TextOverflow.ellipsis, style: Desktop_text_selection_toolbar_buttonLibrary._kToolbarButtonFontStyle.copyWith(color: ((object.Equals(Theme.of(context).colorScheme.brightness, Brightness.dark)) ? Colors.white : Colors.black87)));
        return __instance;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__1776 = Theme.of(context);
        var isDark__1813 = (object.Equals(theme__1776.colorScheme.brightness, Brightness.dark));
        global::Doroti.Flutter.Ui.Color foregroundColor__1887 = ((global::Doroti.Flutter.Ui.Color)(object?)(isDark__1813 ? Colors.white : Colors.black87));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.SizedBox(width: double.PositiveInfinity, child: new TextButton(style: TextButton.styleFrom(alignment: global::Doroti.Generated.Framework.Painting.Alignment.centerLeft, enabledMouseCursor: global::Doroti.Generated.Framework.Services.SystemMouseCursors.basic, disabledMouseCursor: global::Doroti.Generated.Framework.Services.SystemMouseCursors.basic, foregroundColor: foregroundColor__1887, shape: new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(), minimumSize: new global::Doroti.Flutter.Ui.Size(ConstantsLibrary.kMinInteractiveDimension, 36.0), padding: Desktop_text_selection_toolbar_buttonLibrary._kToolbarButtonPadding), onPressed: this.onPressed, child: this.child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
