// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/system_context_menu.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class SystemContextMenu : StatefulWidget
{
    public virtual Rect anchor { get; private set; } = default!;
    public virtual List<IOSSystemContextMenuItem> items { get; private set; } = default!;
    public virtual Action? onSystemHide { get; private set; }

    public SystemContextMenu(Key? key = null, Rect anchor = default!, List<IOSSystemContextMenuItem> items = default!, Action? onSystemHide = null) : base(key: key)
    {
        this.anchor = anchor;
        this.items = items;
        this.onSystemHide = onSystemHide;
    }

    public static SystemContextMenu CreateEditableText(Key? key = null, EditableTextState editableTextState = default!, List<IOSSystemContextMenuItem>? items = null)
    {
        var (startGlyphHeight, endGlyphHeight) = editableTextState.getGlyphHeights();
        return new SystemContextMenu(key: key, anchor: TextSelectionToolbarAnchors.getSelectionRect(editableTextState.renderEditable, startGlyphHeight, endGlyphHeight, editableTextState.renderEditable.getEndpointsForSelection(editableTextState.textEditingValue.selection)), items: items ?? getDefaultItems(editableTextState), onSystemHide: () => { editableTextState.hideToolbar(false); });
    }

    public static bool isSupported(BuildContext context)
    {
        return Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS) && (MediaQuery.maybeSupportsShowingSystemContextMenu(context) ?? false);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool isSupportedByField(EditableTextState editableTextState)
    {
        return !editableTextState.widget.readOnly && isSupported(editableTextState.context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static List<IOSSystemContextMenuItem> getDefaultItems(EditableTextState editableTextState)
    {
        var items = new List<IOSSystemContextMenuItem>();
        foreach (ContextMenuButtonItem button in editableTextState.contextMenuButtonItems)
        {
            switch (button.type)
            {
                case ContextMenuButtonType.copy:
                    {
                        items.Add(new IOSSystemContextMenuItemCopy());
                        break;
                    }
                case ContextMenuButtonType.cut:
                    {
                        items.Add(new IOSSystemContextMenuItemCut());
                        break;
                    }
                case ContextMenuButtonType.paste:
                    {
                        items.Add(new IOSSystemContextMenuItemPaste());
                        break;
                    }
                case ContextMenuButtonType.selectAll:
                    {
                        items.Add(new IOSSystemContextMenuItemSelectAll());
                        break;
                    }
                case ContextMenuButtonType.lookUp:
                    {
                        items.Add(new IOSSystemContextMenuItemLookUp());
                        break;
                    }
                case ContextMenuButtonType.searchWeb:
                    {
                        items.Add(new IOSSystemContextMenuItemSearchWeb());
                        break;
                    }
                case ContextMenuButtonType.share:
                    {
                        items.Add(new IOSSystemContextMenuItemShare());
                        break;
                    }
                case ContextMenuButtonType.liveTextInput:
                    {
                        items.Add(new IOSSystemContextMenuItemLiveText());
                        break;
                    }
                case ContextMenuButtonType.delete:
                case ContextMenuButtonType.custom:
                    break;
            }
        }
        return items;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SystemContextMenuState__system_context_menu());
}

internal class _SystemContextMenuState__system_context_menu : State<SystemContextMenu>
{
    internal virtual SystemContextMenuController _systemContextMenuController { get; private set; } = default!;

    public override void initState()
    {
        base.initState();
        _systemContextMenuController = new SystemContextMenuController(onSystemHide: () => widget.onSystemHide?.Invoke());
    }

    public override void dispose()
    {
        _systemContextMenuController.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => SystemContextMenu.isSupported(context));
        if (Enumerable.Any(widget.items))
        {
            WidgetsLocalizations localizations = WidgetsLocalizations.of(context);
            List<IOSSystemContextMenuItemData> itemDatas = widget.items.map((item) => item.getData(localizations)).ToList().ToList();
            DartRuntimePrimitives.Ignore(_systemContextMenuController.showWithItems(widget.anchor, itemDatas));
        }
        return SizedBox.CreateShrink();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class IOSSystemContextMenuItem
{
    protected IOSSystemContextMenuItem()
    {
    }

    public virtual string? title => DartRuntimePrimitives.ConvertValue<string>(null);
    public abstract IOSSystemContextMenuItemData getData(WidgetsLocalizations localizations);
    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(title?.GetHashCode() ?? 0);
    public override bool Equals(object? other)
    {
        var __other = other as IOSSystemContextMenuItem;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is IOSSystemContextMenuItem) && (__other.title == title);
    }

}

public class IOSSystemContextMenuItemCopy : IOSSystemContextMenuItem
{
    public IOSSystemContextMenuItemCopy()
    {
    }

    public override IOSSystemContextMenuItemDataCopy getData(WidgetsLocalizations localizations)
    {
        return new IOSSystemContextMenuItemDataCopy();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class IOSSystemContextMenuItemCut : IOSSystemContextMenuItem
{
    public IOSSystemContextMenuItemCut()
    {
    }

    public override IOSSystemContextMenuItemDataCut getData(WidgetsLocalizations localizations)
    {
        return new IOSSystemContextMenuItemDataCut();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class IOSSystemContextMenuItemPaste : IOSSystemContextMenuItem
{
    public IOSSystemContextMenuItemPaste()
    {
    }

    public override IOSSystemContextMenuItemDataPaste getData(WidgetsLocalizations localizations)
    {
        return new IOSSystemContextMenuItemDataPaste();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class IOSSystemContextMenuItemSelectAll : IOSSystemContextMenuItem
{
    public IOSSystemContextMenuItemSelectAll()
    {
    }

    public override IOSSystemContextMenuItemDataSelectAll getData(WidgetsLocalizations localizations)
    {
        return new IOSSystemContextMenuItemDataSelectAll();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class IOSSystemContextMenuItemLookUp : IOSSystemContextMenuItem, Diagnosticable
{
    private string? __field_title = default!;
    public override string? title { get => __field_title; }

    public IOSSystemContextMenuItemLookUp(string? title = null)
    {
        __field_title = title;
    }

    public override IOSSystemContextMenuItemDataLookUp getData(WidgetsLocalizations localizations)
    {
        return new IOSSystemContextMenuItemDataLookUp(title: title ?? localizations.lookUpButtonLabel);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new DiagnosticsProperty<string>("title", title));
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class IOSSystemContextMenuItemSearchWeb : IOSSystemContextMenuItem, Diagnosticable
{
    private string? __field_title = default!;
    public override string? title { get => __field_title; }

    public IOSSystemContextMenuItemSearchWeb(string? title = null)
    {
        __field_title = title;
    }

    public override IOSSystemContextMenuItemDataSearchWeb getData(WidgetsLocalizations localizations)
    {
        return new IOSSystemContextMenuItemDataSearchWeb(title: title ?? localizations.searchWebButtonLabel);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new DiagnosticsProperty<string>("title", title));
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class IOSSystemContextMenuItemShare : IOSSystemContextMenuItem, Diagnosticable
{
    private string? __field_title = default!;
    public override string? title { get => __field_title; }

    public IOSSystemContextMenuItemShare(string? title = null)
    {
        __field_title = title;
    }

    public override IOSSystemContextMenuItemDataShare getData(WidgetsLocalizations localizations)
    {
        return new IOSSystemContextMenuItemDataShare(title: title ?? localizations.shareButtonLabel);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new StringProperty("title", title));
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class IOSSystemContextMenuItemLiveText : IOSSystemContextMenuItem
{
    public IOSSystemContextMenuItemLiveText()
    {
    }

    public override IOSSystemContextMenuItemData getData(WidgetsLocalizations localizations)
    {
        return new IOSSystemContextMenuItemDataLiveText();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class IOSSystemContextMenuItemCustom : IOSSystemContextMenuItem, Diagnosticable
{
    private string? __field_title = default!;
    public override string? title { get => __field_title; }
    public virtual Action onPressed { get; private set; } = default!;

    public IOSSystemContextMenuItemCustom(string title, Action onPressed)
    {
        __field_title = title;
        this.onPressed = onPressed;
    }

    public override IOSSystemContextMenuItemData getData(WidgetsLocalizations localizations)
    {
        return new IOSSystemContextMenuItemDataCustom(title: DartRuntimePrimitives.RequireReference(title), onPressed: () => onPressed());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(title, onPressed));
    public override bool Equals(object? other)
    {
        var __other = other as IOSSystemContextMenuItemCustom;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return (__other is IOSSystemContextMenuItemCustom) && (__other.title == title) && Equals(__other.onPressed, onPressed);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new StringProperty("title", title));
        properties.add(ObjectFlagProperty<Action>.CreateHas("onPressed", onPressed));
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

