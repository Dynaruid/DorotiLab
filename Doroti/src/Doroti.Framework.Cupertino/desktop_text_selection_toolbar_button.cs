// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/desktop_text_selection_toolbar_button.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class Desktop_text_selection_toolbar_buttonLibrary
{
    internal static TextStyle _kToolbarButtonFontStyle = new TextStyle(
        inherit: false,
        fontSize: 14.0,
        letterSpacing: -0.15,
        fontWeight: FontWeight.w400
    );
}

public static partial class Desktop_text_selection_toolbar_buttonLibrary
{
    internal static EdgeInsets _kToolbarButtonPadding = new EdgeInsets(8.0, 2.0, 8.0, 5.0);
}

public class CupertinoDesktopTextSelectionToolbarButton : StatefulWidget
{
    public virtual Action? onPressed { get; private set; }
    public virtual Widget? child { get; private set; }
    public virtual ContextMenuButtonItem? buttonItem { get; private set; }
    public virtual string? text { get; private set; }

    public CupertinoDesktopTextSelectionToolbarButton(
        Key? key = null,
        Action? onPressed = default!,
        Widget child = default!
    )
        : base(key: key)
    {
        this.onPressed = onPressed;
        this.child = child;
        buttonItem = null;
        text = null;
    }

    public static CupertinoDesktopTextSelectionToolbarButton CreateText(
        Key? key = null,
        Action? onPressed = default!,
        string? text = default!
    )
    {
        var __instance = new CupertinoDesktopTextSelectionToolbarButton(
            key: key,
            onPressed: onPressed,
            child: default!
        );
        __instance.onPressed = onPressed;
        __instance.text = text;
        __instance.buttonItem = null;
        __instance.child = null;
        return __instance;
    }

    public static CupertinoDesktopTextSelectionToolbarButton CreateButtonItem(
        Key? key = null,
        ContextMenuButtonItem buttonItem = default!
    )
    {
        var __instance = new CupertinoDesktopTextSelectionToolbarButton(
            key: key,
            onPressed: default!,
            child: default!
        );
        __instance.buttonItem = buttonItem;
        __instance.onPressed = buttonItem.onPressed;
        __instance.text = null;
        __instance.child = null;
        return __instance;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _CupertinoDesktopTextSelectionToolbarButtonState__desktop_text_selection_toolbar_button()
        );
}

internal class _CupertinoDesktopTextSelectionToolbarButtonState__desktop_text_selection_toolbar_button
    : State<CupertinoDesktopTextSelectionToolbarButton>
{
    internal virtual bool _isHovered { get; set; } = false;

    internal virtual void _onEnter(Gestures.PointerEnterEvent @event)
    {
        setState(() =>
        {
            _isHovered = true;
        });
    }

    internal virtual void _onExit(Gestures.PointerExitEvent @event)
    {
        setState(() =>
        {
            _isHovered = false;
        });
    }

    public override Widget build(BuildContext context)
    {
        Widget childLocal =
            widget.child
            ?? new Text(
                widget.text
                    ?? CupertinoTextSelectionToolbarButton.getButtonLabel(
                        context,
                        widget.buttonItem!
                    ),
                overflow: TextOverflow.ellipsis,
                style: Desktop_text_selection_toolbar_buttonLibrary._kToolbarButtonFontStyle.copyWith(
                    color: _isHovered
                        ? CupertinoTheme.of(context).primaryContrastingColor
                        : new CupertinoDynamicColor(
                            color: CupertinoColors.black,
                            darkColor: CupertinoColors.white
                        ).resolveFrom(context)
                )
            );
        return new SizedBox(
            width: double.PositiveInfinity,
            child: new MouseRegion(
                onEnter: _onEnter,
                onExit: _onExit,
                child: new CupertinoButton(
                    alignment: Alignment.centerLeft,
                    borderRadius: BorderRadius.CreateAll(Radius.circular(4.0)),
                    color: _isHovered ? CupertinoTheme.of(context).primaryColor : null,
                    minSize: 0.0,
                    onPressed: widget.onPressed,
                    padding: Desktop_text_selection_toolbar_buttonLibrary._kToolbarButtonPadding,
                    pressedOpacity: 0.7,
                    child: childLocal
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
