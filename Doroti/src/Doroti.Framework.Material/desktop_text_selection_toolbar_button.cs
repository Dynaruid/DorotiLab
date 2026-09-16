// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/desktop_text_selection_toolbar_button.dart

using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class Desktop_text_selection_toolbar_buttonLibrary
{
    internal static TextStyle _kToolbarButtonFontStyle = new TextStyle(inherit: false, fontSize: 14.0, letterSpacing: -0.15, fontWeight: FontWeight.w400);
}

public static partial class Desktop_text_selection_toolbar_buttonLibrary
{
    internal static EdgeInsets _kToolbarButtonPadding = new EdgeInsets(20.0, 0.0, 20.0, 3.0);
}

public class DesktopTextSelectionToolbarButton : StatelessWidget
{
    public virtual Action? onPressed { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public DesktopTextSelectionToolbarButton(Key? key = null, Action? onPressed = default!, Widget child = default!) : base(key: key)
    {
        this.onPressed = onPressed;
        this.child = child;
    }

    public static DesktopTextSelectionToolbarButton CreateText(Key? key = null, BuildContext context = default!, Action? onPressed = default!, string text = default!)
    {
        var __instance = new DesktopTextSelectionToolbarButton(key: key, onPressed: onPressed, child: default!);
        __instance.onPressed = onPressed;
        __instance.child = new Text(text, overflow: TextOverflow.ellipsis, style: Desktop_text_selection_toolbar_buttonLibrary._kToolbarButtonFontStyle.copyWith(color: Equals(Theme.of(context).colorScheme.brightness, Brightness.dark) ? Colors.white : Colors.black87));
        return __instance;
    }

    public override Widget build(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        var isDark = Equals(theme.colorScheme.brightness, Brightness.dark);
        Color foregroundColorLocal = isDark ? Colors.white : Colors.black87;
        return new SizedBox(width: double.PositiveInfinity, child: new TextButton(style: TextButton.styleFrom(alignment: Alignment.centerLeft, enabledMouseCursor: SystemMouseCursors.basic, disabledMouseCursor: SystemMouseCursors.basic, foregroundColor: foregroundColorLocal, shape: new RoundedRectangleBorder(), minimumSize: new Size(ConstantsLibrary.kMinInteractiveDimension, 36.0), padding: Desktop_text_selection_toolbar_buttonLibrary._kToolbarButtonPadding), onPressed: onPressed, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
