// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/desktop_text_selection_toolbar_button.dart
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

public static partial class Desktop_text_selection_toolbar_buttonLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _kToolbarButtonFontStyle = new global::Doroti.Framework.Painting.TextStyle(inherit: false, fontSize: 14.0, letterSpacing: -0.15, fontWeight: FontWeight.w400);
}

public static partial class Desktop_text_selection_toolbar_buttonLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsets _kToolbarButtonPadding = new global::Doroti.Framework.Painting.EdgeInsets(8.0, 2.0, 8.0, 5.0);
}

public class CupertinoDesktopTextSelectionToolbarButton : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }
    public virtual global::Doroti.Framework.Widgets.ContextMenuButtonItem? buttonItem { get; private set; }
    public virtual string? text { get; private set; }

    public CupertinoDesktopTextSelectionToolbarButton(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action? onPressed = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key)
    {
        this.onPressed = onPressed;
        this.child = child;
        this.buttonItem = null;
        this.text = null;
    }

    public static CupertinoDesktopTextSelectionToolbarButton CreateText(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action? onPressed = default!, string? text = default!)
    {
        var __instance = new CupertinoDesktopTextSelectionToolbarButton(key: key, onPressed: onPressed, child: default!);
        __instance.onPressed = onPressed;
        __instance.text = text;
        __instance.buttonItem = null;
        __instance.child = null;
        return __instance;
    }

    public static CupertinoDesktopTextSelectionToolbarButton CreateButtonItem(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.ContextMenuButtonItem buttonItem = default!)
    {
        var __instance = new CupertinoDesktopTextSelectionToolbarButton(key: key, onPressed: default!, child: default!);
        __instance.buttonItem = buttonItem;
        __instance.onPressed = buttonItem.onPressed;
        __instance.text = null;
        __instance.child = null;
        return __instance;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoDesktopTextSelectionToolbarButtonState__desktop_text_selection_toolbar_button());
}

internal class _CupertinoDesktopTextSelectionToolbarButtonState__desktop_text_selection_toolbar_button : global::Doroti.Framework.Widgets.State<CupertinoDesktopTextSelectionToolbarButton>
{
    internal virtual bool _isHovered { get; set; } = false;

    internal virtual void _onEnter(global::Doroti.Framework.Gestures.PointerEnterEvent @event)
    {
        setState(((global::System.Action)(() =>
        {
            _isHovered = true;
        })));
    }

    internal virtual void _onExit(global::Doroti.Framework.Gestures.PointerExitEvent @event)
    {
        setState(((global::System.Action)(() =>
        {
            _isHovered = false;
        })));
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.Widget childLocal = (((CupertinoDesktopTextSelectionToolbarButton)this.widget).child ?? new global::Doroti.Framework.Widgets.Text(((((CupertinoDesktopTextSelectionToolbarButton)this.widget).text ?? (string)CupertinoTextSelectionToolbarButton.getButtonLabel(context, ((CupertinoDesktopTextSelectionToolbarButton)this.widget).buttonItem!))), overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis, style: Desktop_text_selection_toolbar_buttonLibrary._kToolbarButtonFontStyle.copyWith(color: (this._isHovered ? CupertinoTheme.of(context).primaryContrastingColor : new CupertinoDynamicColor(color: CupertinoColors.black, darkColor: CupertinoColors.white).resolveFrom(context)))));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SizedBox(width: double.PositiveInfinity, child: new global::Doroti.Framework.Widgets.MouseRegion(onEnter: (global::System.Action<global::Doroti.Framework.Gestures.PointerEnterEvent>)this._onEnter, onExit: (global::System.Action<global::Doroti.Framework.Gestures.PointerExitEvent>)this._onExit, child: new CupertinoButton(alignment: global::Doroti.Framework.Painting.Alignment.centerLeft, borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(4.0)), color: (this._isHovered ? CupertinoTheme.of(context).primaryColor : null), minSize: 0.0, onPressed: ((CupertinoDesktopTextSelectionToolbarButton)this.widget).onPressed, padding: Desktop_text_selection_toolbar_buttonLibrary._kToolbarButtonPadding, pressedOpacity: 0.7, child: childLocal))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
