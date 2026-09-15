// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/desktop_text_selection_toolbar_button.dart
#pragma warning disable CS8600, CS8603
using Doroti.Ui;

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
