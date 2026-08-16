// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/system_context_menu.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public class SystemContextMenu : StatefulWidget
{
    public virtual Rect anchor { get; private set; } = default!;
    public virtual List<IOSSystemContextMenuItem> items { get; private set; } = default!;
    public virtual global::System.Action? onSystemHide { get; private set; }

    public SystemContextMenu(global::Doroti.Framework.Foundation.Key? key = null, Rect anchor = default!, List<IOSSystemContextMenuItem> items = default!, global::System.Action? onSystemHide = null) : base(key: key)
    {
        this.anchor = anchor;
        this.items = items;
        this.onSystemHide = onSystemHide;
    }

    public static SystemContextMenu CreateEditableText(global::Doroti.Framework.Foundation.Key? key = null, EditableTextState editableTextState = default!, List<IOSSystemContextMenuItem>? items = null)
    {
        var (startGlyphHeight__2775, endGlyphHeight__2816) = editableTextState.getGlyphHeights();
        return new SystemContextMenu(key: key, anchor: TextSelectionToolbarAnchors.getSelectionRect(((EditableTextState)editableTextState).renderEditable, startGlyphHeight__2775, endGlyphHeight__2816, ((List<global::Doroti.Framework.Rendering.TextSelectionPoint>)((dynamic)((EditableTextState)editableTextState).renderEditable).getEndpointsForSelection(((EditableTextState)editableTextState).textEditingValue.selection))), items: ((items ?? (List<IOSSystemContextMenuItem>)SystemContextMenu.getDefaultItems(editableTextState))), onSystemHide: ((global::System.Action)(() => { editableTextState.hideToolbar(false); })));
    }

    public static bool isSupported(BuildContext context)
    {
        return ((object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS)) && ((MediaQuery.maybeSupportsShowingSystemContextMenu(context) ?? false)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool isSupportedByField(EditableTextState editableTextState)
    {
        return (!editableTextState.widget.readOnly && SystemContextMenu.isSupported(editableTextState.context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static List<IOSSystemContextMenuItem> getDefaultItems(EditableTextState editableTextState)
    {
        var items__6245 = new List<IOSSystemContextMenuItem>();
        foreach (ContextMenuButtonItem button__6411 in ((EditableTextState)editableTextState).contextMenuButtonItems)
        {
            switch (((ContextMenuButtonItem)button__6411).type)
            {
                case ContextMenuButtonType.copy:
                    {
                        items__6245.Add(new IOSSystemContextMenuItemCopy());
                        break;
                    }
                case ContextMenuButtonType.cut:
                    {
                        items__6245.Add(new IOSSystemContextMenuItemCut());
                        break;
                    }
                case ContextMenuButtonType.paste:
                    {
                        items__6245.Add(new IOSSystemContextMenuItemPaste());
                        break;
                    }
                case ContextMenuButtonType.selectAll:
                    {
                        items__6245.Add(new IOSSystemContextMenuItemSelectAll());
                        break;
                    }
                case ContextMenuButtonType.lookUp:
                    {
                        items__6245.Add(new IOSSystemContextMenuItemLookUp());
                        break;
                    }
                case ContextMenuButtonType.searchWeb:
                    {
                        items__6245.Add(new IOSSystemContextMenuItemSearchWeb());
                        break;
                    }
                case ContextMenuButtonType.share:
                    {
                        items__6245.Add(new IOSSystemContextMenuItemShare());
                        break;
                    }
                case ContextMenuButtonType.liveTextInput:
                    {
                        items__6245.Add(new IOSSystemContextMenuItemLiveText());
                        break;
                    }
                case ContextMenuButtonType.delete:
                case ContextMenuButtonType.custom:
                    break;
            }
        }
        return items__6245;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SystemContextMenuState__system_context_menu());
}

internal class _SystemContextMenuState__system_context_menu : State<SystemContextMenu>
{
    internal virtual global::Doroti.Framework.Services.SystemContextMenuController _systemContextMenuController { get; private set; } = default!;

    public override void initState()
    {
        base.initState();
        _systemContextMenuController = new global::Doroti.Framework.Services.SystemContextMenuController(onSystemHide: () => ((SystemContextMenu)this.widget).onSystemHide());
    }

    public override void dispose()
    {
        this._systemContextMenuController.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => SystemContextMenu.isSupported(context));
        if (System.Linq.Enumerable.Any(((SystemContextMenu)this.widget).items))
        {
            WidgetsLocalizations localizations__8340 = ((WidgetsLocalizations)(object?)WidgetsLocalizations.of(context));
            List<global::Doroti.Framework.Services.IOSSystemContextMenuItemData> itemDatas__8437 = ((SystemContextMenu)this.widget).items.map<IOSSystemContextMenuItem, global::Doroti.Framework.Services.IOSSystemContextMenuItemData>(((item) => item.getData(localizations__8340))).ToList().ToList();
            DartRuntimePrimitives.Ignore(this._systemContextMenuController.showWithItems(((SystemContextMenu)this.widget).anchor, itemDatas__8437));
        }
        return ((Widget)(object?)SizedBox.CreateShrink());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class IOSSystemContextMenuItem
{
    protected IOSSystemContextMenuItem()
    {
    }

    public virtual string? title => DartRuntimePrimitives.ConvertValue<string>(null);
    public abstract global::Doroti.Framework.Services.IOSSystemContextMenuItemData getData(WidgetsLocalizations localizations);
    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(this.title.GetHashCode());
    public override bool Equals(object? other)
    {
        var __other = other as IOSSystemContextMenuItem;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((__other is IOSSystemContextMenuItem) && (((IOSSystemContextMenuItem)((IOSSystemContextMenuItem)__other)).title == this.title));
    }

}

public class IOSSystemContextMenuItemCopy : IOSSystemContextMenuItem
{
    public IOSSystemContextMenuItemCopy()
    {
    }

    public override global::Doroti.Framework.Services.IOSSystemContextMenuItemDataCopy getData(WidgetsLocalizations localizations)
    {
        return new global::Doroti.Framework.Services.IOSSystemContextMenuItemDataCopy();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class IOSSystemContextMenuItemCut : IOSSystemContextMenuItem
{
    public IOSSystemContextMenuItemCut()
    {
    }

    public override global::Doroti.Framework.Services.IOSSystemContextMenuItemDataCut getData(WidgetsLocalizations localizations)
    {
        return new global::Doroti.Framework.Services.IOSSystemContextMenuItemDataCut();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class IOSSystemContextMenuItemPaste : IOSSystemContextMenuItem
{
    public IOSSystemContextMenuItemPaste()
    {
    }

    public override global::Doroti.Framework.Services.IOSSystemContextMenuItemDataPaste getData(WidgetsLocalizations localizations)
    {
        return new global::Doroti.Framework.Services.IOSSystemContextMenuItemDataPaste();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class IOSSystemContextMenuItemSelectAll : IOSSystemContextMenuItem
{
    public IOSSystemContextMenuItemSelectAll()
    {
    }

    public override global::Doroti.Framework.Services.IOSSystemContextMenuItemDataSelectAll getData(WidgetsLocalizations localizations)
    {
        return new global::Doroti.Framework.Services.IOSSystemContextMenuItemDataSelectAll();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class IOSSystemContextMenuItemLookUp : IOSSystemContextMenuItem, global::Doroti.Framework.Foundation.Diagnosticable
{
    private string? __field_title = default!;
    public override string? title { get => __field_title; }

    public IOSSystemContextMenuItemLookUp(string? title = null)
    {
        this.__field_title = title;
    }

    public override global::Doroti.Framework.Services.IOSSystemContextMenuItemDataLookUp getData(WidgetsLocalizations localizations)
    {
        return new global::Doroti.Framework.Services.IOSSystemContextMenuItemDataLookUp(title: ((this.title ?? (string)((WidgetsLocalizations)localizations).lookUpButtonLabel)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<string>("title", this.title));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class IOSSystemContextMenuItemSearchWeb : IOSSystemContextMenuItem, global::Doroti.Framework.Foundation.Diagnosticable
{
    private string? __field_title = default!;
    public override string? title { get => __field_title; }

    public IOSSystemContextMenuItemSearchWeb(string? title = null)
    {
        this.__field_title = title;
    }

    public override global::Doroti.Framework.Services.IOSSystemContextMenuItemDataSearchWeb getData(WidgetsLocalizations localizations)
    {
        return new global::Doroti.Framework.Services.IOSSystemContextMenuItemDataSearchWeb(title: ((this.title ?? (string)((WidgetsLocalizations)localizations).searchWebButtonLabel)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<string>("title", this.title));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class IOSSystemContextMenuItemShare : IOSSystemContextMenuItem, global::Doroti.Framework.Foundation.Diagnosticable
{
    private string? __field_title = default!;
    public override string? title { get => __field_title; }

    public IOSSystemContextMenuItemShare(string? title = null)
    {
        this.__field_title = title;
    }

    public override global::Doroti.Framework.Services.IOSSystemContextMenuItemDataShare getData(WidgetsLocalizations localizations)
    {
        return new global::Doroti.Framework.Services.IOSSystemContextMenuItemDataShare(title: ((this.title ?? (string)((WidgetsLocalizations)localizations).shareButtonLabel)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("title", this.title));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class IOSSystemContextMenuItemLiveText : IOSSystemContextMenuItem
{
    public IOSSystemContextMenuItemLiveText()
    {
    }

    public override global::Doroti.Framework.Services.IOSSystemContextMenuItemData getData(WidgetsLocalizations localizations)
    {
        return ((global::Doroti.Framework.Services.IOSSystemContextMenuItemData)(object?)new global::Doroti.Framework.Services.IOSSystemContextMenuItemDataLiveText());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class IOSSystemContextMenuItemCustom : IOSSystemContextMenuItem, global::Doroti.Framework.Foundation.Diagnosticable
{
    private string? __field_title = default!;
    public override string? title { get => __field_title; }
    public virtual global::System.Action onPressed { get; private set; } = default!;

    public IOSSystemContextMenuItemCustom(string title, global::System.Action onPressed)
    {
        this.__field_title = title;
        this.onPressed = onPressed;
    }

    public override global::Doroti.Framework.Services.IOSSystemContextMenuItemData getData(WidgetsLocalizations localizations)
    {
        return ((global::Doroti.Framework.Services.IOSSystemContextMenuItemData)(object?)new global::Doroti.Framework.Services.IOSSystemContextMenuItemDataCustom(title: this.title, onPressed: () => this.onPressed()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.title, this.onPressed));
    public override bool Equals(object? other)
    {
        var __other = other as IOSSystemContextMenuItemCustom;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return (((__other is IOSSystemContextMenuItemCustom) && (((IOSSystemContextMenuItemCustom)((IOSSystemContextMenuItemCustom)__other)).title == this.title)) && (object.Equals((global::System.Action)((IOSSystemContextMenuItemCustom)((IOSSystemContextMenuItemCustom)__other)).onPressed, (global::System.Action)this.onPressed)));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("title", this.title));
        properties.add(global::Doroti.Framework.Foundation.ObjectFlagProperty<global::System.Action>.CreateHas("onPressed", this.onPressed));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

