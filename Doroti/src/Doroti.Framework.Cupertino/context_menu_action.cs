// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/context_menu_action.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public class CupertinoContextMenuAction : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual bool isDefaultAction { get; private set; } = default!;
    public virtual bool isDestructiveAction { get; private set; } = default!;
    public virtual Action? onPressed { get; private set; }
    public virtual IconData? trailingIcon { get; private set; }

    public CupertinoContextMenuAction(
        Key? key = null,
        Widget child = default!,
        bool isDefaultAction = false,
        bool isDestructiveAction = false,
        Action? onPressed = null,
        IconData? trailingIcon = null
    )
        : base(key: key)
    {
        this.child = child;
        this.isDefaultAction = isDefaultAction;
        this.isDestructiveAction = isDestructiveAction;
        this.onPressed = onPressed;
        this.trailingIcon = trailingIcon;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _CupertinoContextMenuActionState__context_menu_action()
        );
}

internal class _CupertinoContextMenuActionState__context_menu_action
    : State<CupertinoContextMenuAction>
{
    internal static Color _kBackgroundColorPressed = new CupertinoDynamicColor(
        color: new Color(4292730333L),
        darkColor: new Color(4282335040L)
    );
    internal const double _kButtonHeight = 43;
    internal static TextStyle _kActionSheetActionStyle = new TextStyle(
        fontFamily: "CupertinoSystemText",
        inherit: false,
        fontSize: 16.0,
        fontWeight: FontWeight.w400,
        color: CupertinoColors.black,
        textBaseline: TextBaseline.alphabetic
    );
    internal virtual GlobalKey<IState> _globalKey { get; private set; } =
        GlobalKey<IState>.Create();
    internal virtual bool _isPressed { get; set; } = false;

    public virtual void onTapDown(Gestures.TapDownDetails details)
    {
        setState(() =>
        {
            _isPressed = true;
        });
    }

    public virtual void onTapUp(Gestures.TapUpDetails details)
    {
        setState(() =>
        {
            _isPressed = false;
        });
    }

    public virtual void onTapCancel()
    {
        setState(() =>
        {
            _isPressed = false;
        });
    }

    internal virtual TextStyle _textStyle
    {
        get
        {
            if (widget.isDefaultAction)
            {
                return _kActionSheetActionStyle.copyWith(
                    color: CupertinoDynamicColor.resolve(CupertinoColors.label, context),
                    fontWeight: FontWeight.w600
                );
            }
            if (widget.isDestructiveAction)
            {
                return _kActionSheetActionStyle.copyWith(color: CupertinoColors.destructiveRed);
            }
            return _kActionSheetActionStyle.copyWith(
                color: CupertinoDynamicColor.resolve(CupertinoColors.label, context)
            );
        }
    }

    public override Widget build(BuildContext context)
    {
        return new MouseRegion(
            cursor: ((widget.onPressed is not null) && Foundation.ConstantsLibrary.kIsWeb)
                ? SystemMouseCursors.click
                : MouseCursor.defer,
            child: new GestureDetector(
                key: _globalKey,
                onTapDown: onTapDown,
                onTapUp: onTapUp,
                onTapCancel: () => onTapCancel(),
                onTap: widget.onPressed,
                behavior: HitTestBehavior.opaque,
                child: new ConstrainedBox(
                    constraints: new BoxConstraints(minHeight: _kButtonHeight),
                    child: new Widgets.Semantics(
                        button: true,
                        child: new ColoredBox(
                            color: _isPressed
                                ? CupertinoDynamicColor.resolve(_kBackgroundColorPressed, context)
                                : CupertinoDynamicColor.resolve(
                                    CupertinoContextMenu.kBackgroundColor,
                                    context
                                ),
                            child: new Padding(
                                padding: new EdgeInsets(15.5, 8.0, 17.5, 8.0),
                                child: new DefaultTextStyle(
                                    style: _textStyle,
                                    child: new Row(
                                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                        children: (
                                            (Func<List<Widget>>)(
                                                () =>
                                                {
                                                    var __collection4107 = new List<Widget>();
                                                    __collection4107.Add(
                                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                                            new Flexible(child: widget.child)
                                                        )
                                                    );
                                                    if (widget.trailingIcon is not null)
                                                    {
                                                        __collection4107.Add(
                                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                                new Icon(
                                                                    widget.trailingIcon,
                                                                    color: _textStyle.color,
                                                                    size: 21.0
                                                                )
                                                            )
                                                        );
                                                    }
                                                    return __collection4107;
                                                }
                                            )
                                        )()
                                    )
                                )
                            )
                        )
                    )
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
