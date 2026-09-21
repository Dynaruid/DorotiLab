// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/default_selection_style.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class DefaultSelectionStyle : InheritedTheme
{
    public static Color defaultColor = new Color(2155905152L);
    public virtual Color? cursorColor { get; private set; }
    public virtual Color? selectionColor { get; private set; }
    public virtual MouseCursor? mouseCursor { get; private set; }

    public DefaultSelectionStyle(
        Key? key = null,
        Color? cursorColor = null,
        Color? selectionColor = null,
        MouseCursor? mouseCursor = null,
        Widget child = default!
    )
        : base(key: key, child: child)
    {
        this.cursorColor = cursorColor;
        this.selectionColor = selectionColor;
        this.mouseCursor = mouseCursor;
    }

    public static DefaultSelectionStyle CreateFallback(Key? key = null)
    {
        var __instance = new DefaultSelectionStyle(key, default!, default!, default!, default!);
        __instance.cursorColor = null;
        __instance.selectionColor = null;
        __instance.mouseCursor = null;
        return __instance;
    }

    public static Widget merge(
        Key? key = null,
        Color? cursorColor = null,
        Color? selectionColor = null,
        MouseCursor? mouseCursor = null,
        Widget child = default!
    )
    {
        return new Builder(
            builder: (context) =>
            {
                DefaultSelectionStyle parent = of(context);
                return new DefaultSelectionStyle(
                    key: key,
                    cursorColor: cursorColor ?? parent.cursorColor,
                    selectionColor: selectionColor ?? parent.selectionColor,
                    mouseCursor: mouseCursor ?? parent.mouseCursor,
                    child: child
                );
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DefaultSelectionStyle of(BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<DefaultSelectionStyle>()
            ?? CreateFallback();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new DefaultSelectionStyle(
            cursorColor: cursorColor,
            selectionColor: selectionColor,
            mouseCursor: mouseCursor,
            child: child
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (DefaultSelectionStyle)oldWidget;
        return (!Equals(cursorColor, __oldWidget.cursorColor))
            || (!Equals(selectionColor, __oldWidget.selectionColor))
            || (!Equals(mouseCursor, __oldWidget.mouseCursor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _NullWidget__default_selection_style : StatelessWidget
{
    internal _NullWidget__default_selection_style() { }

    public override Widget build(BuildContext context)
    {
        throw DartRuntimePrimitives.AsException(
            FlutterError.Create(
                "A DefaultSelectionStyle constructed with DefaultSelectionStyle.fallback cannot be incorporated into the widget tree, "
                    + "it is meant only to provide a fallback value returned by DefaultSelectionStyle.of() "
                    + "when no enclosing default selection style is present in a BuildContext."
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
