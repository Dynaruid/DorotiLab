// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/desktop_text_selection_toolbar_button.dart
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

public static partial class Desktop_text_selection_toolbar_buttonLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _kToolbarButtonFontStyle = new global::Doroti.Framework.Painting.TextStyle(inherit: false, fontSize: 14.0, letterSpacing: -0.15, fontWeight: FontWeight.w400);
}

public static partial class Desktop_text_selection_toolbar_buttonLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsets _kToolbarButtonPadding = new global::Doroti.Framework.Painting.EdgeInsets(20.0, 0.0, 20.0, 3.0);
}

public class DesktopTextSelectionToolbarButton : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    public DesktopTextSelectionToolbarButton(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action? onPressed = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key)
    {
        this.onPressed = onPressed;
        this.child = child;
    }

    public static DesktopTextSelectionToolbarButton CreateText(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.BuildContext context = default!, global::System.Action? onPressed = default!, string text = default!)
    {
        var __instance = new DesktopTextSelectionToolbarButton(key: key, onPressed: onPressed, child: default!);
        __instance.onPressed = onPressed;
        __instance.child = new global::Doroti.Framework.Widgets.Text(text, overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis, style: Desktop_text_selection_toolbar_buttonLibrary._kToolbarButtonFontStyle.copyWith(color: ((object.Equals(Theme.of(context).colorScheme.brightness, Brightness.dark)) ? Colors.white : Colors.black87)));
        return __instance;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        var isDark = (object.Equals(theme.colorScheme.brightness, Brightness.dark));
        global::Doroti.Ui.Color foregroundColorLocal = ((global::Doroti.Ui.Color)(object?)(isDark ? Colors.white : Colors.black87));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SizedBox(width: double.PositiveInfinity, child: new TextButton(style: TextButton.styleFrom(alignment: global::Doroti.Framework.Painting.Alignment.centerLeft, enabledMouseCursor: global::Doroti.Framework.Services.SystemMouseCursors.basic, disabledMouseCursor: global::Doroti.Framework.Services.SystemMouseCursors.basic, foregroundColor: foregroundColorLocal, shape: new global::Doroti.Framework.Painting.RoundedRectangleBorder(), minimumSize: new global::Doroti.Ui.Size(ConstantsLibrary.kMinInteractiveDimension, 36.0), padding: Desktop_text_selection_toolbar_buttonLibrary._kToolbarButtonPadding), onPressed: this.onPressed, child: this.child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
