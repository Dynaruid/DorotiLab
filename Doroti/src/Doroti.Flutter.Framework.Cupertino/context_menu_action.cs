// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/cupertino/context_menu_action.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Cupertino;

public class CupertinoContextMenuAction : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual bool isDefaultAction { get; private set; } = default!;
    public virtual bool isDestructiveAction { get; private set; } = default!;
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.IconData? trailingIcon { get; private set; }

    public CupertinoContextMenuAction(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget child = default!, bool isDefaultAction = false, bool isDestructiveAction = false, global::System.Action? onPressed = null, global::Doroti.Generated.Framework.Widgets.IconData? trailingIcon = null) : base(key: key)
    {
        this.child = child;
        this.isDefaultAction = isDefaultAction;
        this.isDestructiveAction = isDestructiveAction;
        this.onPressed = onPressed;
        this.trailingIcon = trailingIcon;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoContextMenuActionState__context_menu_action());
}

internal class _CupertinoContextMenuActionState__context_menu_action : global::Doroti.Generated.Framework.Widgets.State<CupertinoContextMenuAction>
{
    internal static Color _kBackgroundColorPressed = ((Color)(object?)new CupertinoDynamicColor(color: new global::Doroti.Flutter.Ui.Color(4292730333L), darkColor: new global::Doroti.Flutter.Ui.Color(4282335040L)));
    internal const double _kButtonHeight = 43;
    internal static global::Doroti.Generated.Framework.Painting.TextStyle _kActionSheetActionStyle = new global::Doroti.Generated.Framework.Painting.TextStyle(fontFamily: "CupertinoSystemText", inherit: false, fontSize: 16.0, fontWeight: FontWeight.w400, color: CupertinoColors.black, textBaseline: TextBaseline.alphabetic);
    internal virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> _globalKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual bool _isPressed { get; set; } = false;

    public virtual void onTapDown(global::Doroti.Generated.Framework.Gestures.TapDownDetails details)
    {
        setState(((global::System.Action)(() => {
_isPressed = true;
})));
    }

    public virtual void onTapUp(global::Doroti.Generated.Framework.Gestures.TapUpDetails details)
    {
        setState(((global::System.Action)(() => {
_isPressed = false;
})));
    }

    public virtual void onTapCancel()
    {
        setState(((global::System.Action)(() => {
_isPressed = false;
})));
    }

    internal virtual global::Doroti.Generated.Framework.Painting.TextStyle _textStyle
    {
        get
        {
            if (((CupertinoContextMenuAction)this.widget).isDefaultAction)
            {
                return ((global::Doroti.Generated.Framework.Painting.TextStyle)(object?)_kActionSheetActionStyle.copyWith(color: CupertinoDynamicColor.resolve(CupertinoColors.label, this.context), fontWeight: FontWeight.w600));
            }
            if (((CupertinoContextMenuAction)this.widget).isDestructiveAction)
            {
                return ((global::Doroti.Generated.Framework.Painting.TextStyle)(object?)_kActionSheetActionStyle.copyWith(color: CupertinoColors.destructiveRed));
            }
            return ((global::Doroti.Generated.Framework.Painting.TextStyle)(object?)_kActionSheetActionStyle.copyWith(color: CupertinoDynamicColor.resolve(CupertinoColors.label, this.context)));
            return default!;
        }
    }
    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.MouseRegion(cursor: (((((CupertinoContextMenuAction)this.widget).onPressed is not null) && global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb) ? global::Doroti.Generated.Framework.Services.SystemMouseCursors.click : global::Doroti.Generated.Framework.Services.MouseCursor.defer), child: new global::Doroti.Generated.Framework.Widgets.GestureDetector(key: this._globalKey, onTapDown: (global::System.Action<global::Doroti.Generated.Framework.Gestures.TapDownDetails>)this.onTapDown, onTapUp: (global::System.Action<global::Doroti.Generated.Framework.Gestures.TapUpDetails>)this.onTapUp, onTapCancel: () => this.onTapCancel(), onTap: () => ((CupertinoContextMenuAction)this.widget).onPressed(), behavior: global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque, child: new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(minHeight: _kButtonHeight), child: new global::Doroti.Generated.Framework.Widgets.Semantics(button: true, child: new global::Doroti.Generated.Framework.Widgets.ColoredBox(color: (this._isPressed ? CupertinoDynamicColor.resolve(_kBackgroundColorPressed, context) : CupertinoDynamicColor.resolve(CupertinoContextMenu.kBackgroundColor, context)), child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: new global::Doroti.Generated.Framework.Painting.EdgeInsets(15.5, 8.0, 17.5, 8.0), child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: this._textStyle, child: new global::Doroti.Generated.Framework.Widgets.Row(mainAxisAlignment: global::Doroti.Generated.Framework.Rendering.MainAxisAlignment.spaceBetween, children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection4107 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection4107.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Flexible(child: ((CupertinoContextMenuAction)this.widget).child))); if ((((CupertinoContextMenuAction)this.widget).trailingIcon is not null)) { __collection4107.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Icon(((CupertinoContextMenuAction)this.widget).trailingIcon, color: ((global::Doroti.Generated.Framework.Painting.TextStyle)this._textStyle).color, size: 21.0))); } return __collection4107; }))())))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
