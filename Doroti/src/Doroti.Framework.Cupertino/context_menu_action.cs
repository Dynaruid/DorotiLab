// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/context_menu_action.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public class CupertinoContextMenuAction : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual bool isDefaultAction { get; private set; } = default!;
    public virtual bool isDestructiveAction { get; private set; } = default!;
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual global::Doroti.Framework.Widgets.IconData? trailingIcon { get; private set; }

    public CupertinoContextMenuAction(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget child = default!, bool isDefaultAction = false, bool isDestructiveAction = false, global::System.Action? onPressed = null, global::Doroti.Framework.Widgets.IconData? trailingIcon = null) : base(key: key)
    {
        this.child = child;
        this.isDefaultAction = isDefaultAction;
        this.isDestructiveAction = isDestructiveAction;
        this.onPressed = onPressed;
        this.trailingIcon = trailingIcon;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoContextMenuActionState__context_menu_action());
}

internal class _CupertinoContextMenuActionState__context_menu_action : global::Doroti.Framework.Widgets.State<CupertinoContextMenuAction>
{
    internal static Color _kBackgroundColorPressed = new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(4292730333L), darkColor: new global::Doroti.Ui.Color(4282335040L));
    internal const double _kButtonHeight = 43;
    internal static global::Doroti.Framework.Painting.TextStyle _kActionSheetActionStyle = new global::Doroti.Framework.Painting.TextStyle(fontFamily: "CupertinoSystemText", inherit: false, fontSize: 16.0, fontWeight: FontWeight.w400, color: CupertinoColors.black, textBaseline: TextBaseline.alphabetic);
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _globalKey { get; private set; } = GlobalKey<IState>.Create();
    internal virtual bool _isPressed { get; set; } = false;

    public virtual void onTapDown(global::Doroti.Framework.Gestures.TapDownDetails details)
    {
        setState(() =>
        {
            _isPressed = true;
        });
    }

    public virtual void onTapUp(global::Doroti.Framework.Gestures.TapUpDetails details)
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

    internal virtual global::Doroti.Framework.Painting.TextStyle _textStyle
    {
        get
        {
            if (widget.isDefaultAction)
            {
                return _kActionSheetActionStyle.copyWith(color: CupertinoDynamicColor.resolve(CupertinoColors.label, context), fontWeight: FontWeight.w600);
            }
            if (widget.isDestructiveAction)
            {
                return _kActionSheetActionStyle.copyWith(color: CupertinoColors.destructiveRed);
            }
            return _kActionSheetActionStyle.copyWith(color: CupertinoDynamicColor.resolve(CupertinoColors.label, context));
        }
    }
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.MouseRegion(cursor: ((widget.onPressed is not null) && Foundation.ConstantsLibrary.kIsWeb) ? SystemMouseCursors.click : MouseCursor.defer, child: new global::Doroti.Framework.Widgets.GestureDetector(key: _globalKey, onTapDown: onTapDown, onTapUp: onTapUp, onTapCancel: () => onTapCancel(), onTap: widget.onPressed, behavior: HitTestBehavior.opaque, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minHeight: _kButtonHeight), child: new global::Doroti.Framework.Widgets.Semantics(button: true, child: new global::Doroti.Framework.Widgets.ColoredBox(color: _isPressed ? CupertinoDynamicColor.resolve(_kBackgroundColorPressed, context) : CupertinoDynamicColor.resolve(CupertinoContextMenu.kBackgroundColor, context), child: new global::Doroti.Framework.Widgets.Padding(padding: new global::Doroti.Framework.Painting.EdgeInsets(15.5, 8.0, 17.5, 8.0), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: _textStyle, child: new global::Doroti.Framework.Widgets.Row(mainAxisAlignment: MainAxisAlignment.spaceBetween, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection4107 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection4107.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: widget.child))); if (widget.trailingIcon is not null) { __collection4107.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Icon(widget.trailingIcon, color: _textStyle.color, size: 21.0))); } return __collection4107; }))()))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
