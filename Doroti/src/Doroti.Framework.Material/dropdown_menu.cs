// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/dropdown_menu.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public delegate List<DropdownMenuEntry<T>> FilterCallback<T>(List<DropdownMenuEntry<T>> entries, string filter);

public delegate long? SearchCallback<T>(List<DropdownMenuEntry<T>> entries, string query);

public delegate InputDecoration DropdownMenuDecorationBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.MenuController controller);

public static partial class Dropdown_menuLibrary
{
    internal static double _kMinimumWidth = 112.0;
}

public static partial class Dropdown_menuLibrary
{
    internal static double _kDefaultHorizontalPadding = 12.0;
}

public static partial class Dropdown_menuLibrary
{
    internal static double _kInputStartGap = 4.0;
}

public class DropdownMenuEntry<T>
{
    public virtual T value { get; private set; } = default!;
    public virtual string label { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? labelWidget { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? leadingIcon { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? trailingIcon { get; private set; }
    public virtual bool enabled { get; private set; } = default!;
    public virtual ButtonStyle? style { get; private set; }

    public DropdownMenuEntry(T value, string label, global::Doroti.Framework.Widgets.Widget? labelWidget = null, global::Doroti.Framework.Widgets.Widget? leadingIcon = null, global::Doroti.Framework.Widgets.Widget? trailingIcon = null, bool enabled = true, ButtonStyle? style = null)
    {
        this.value = value;
        this.label = label;
        this.labelWidget = labelWidget;
        this.leadingIcon = leadingIcon;
        this.trailingIcon = trailingIcon;
        this.enabled = enabled;
        this.style = style;
    }

}

public enum DropdownMenuCloseBehavior
{
    all,
    self,
    none
}

public class DropdownMenu<T> : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual bool enabled { get; private set; } = default!;
    public virtual double? width { get; private set; }
    public virtual double? menuHeight { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? leadingIcon { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? trailingIcon { get; private set; }
    public virtual bool showTrailingIcon { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.FocusNode? trailingIconFocusNode { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? label { get; private set; }
    public virtual string? hintText { get; private set; }
    public virtual string? helperText { get; private set; }
    public virtual string? errorText { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? selectedTrailingIcon { get; private set; }
    public virtual bool enableFilter { get; private set; } = default!;
    public virtual bool enableSearch { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.TextInputType? keyboardType { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? textStyle { get; private set; }
    public virtual TextAlign textAlign { get; private set; } = default!;
    internal virtual object? _inputDecorationTheme { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.MenuController, InputDecoration>? decorationBuilder { get; private set; }
    public virtual MenuStyle? menuStyle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.TextEditingController? controller { get; private set; }
    public virtual T? initialSelection { get; private set; }
    public virtual global::System.Action<T?>? onSelected { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool? requestFocusOnTap { get; private set; }
    public virtual bool selectOnly { get; private set; } = default!;
    public virtual List<DropdownMenuEntry<T>> dropdownMenuEntries { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? expandedInsets { get; private set; }
    public virtual global::System.Func<List<DropdownMenuEntry<T>>, string, List<DropdownMenuEntry<T>>>? filterCallback { get; private set; }
    public virtual global::System.Func<List<DropdownMenuEntry<T>>, string, long?>? searchCallback { get; private set; }
    public virtual List<global::Doroti.Framework.Services.TextInputFormatter>? inputFormatters { get; private set; }
    public virtual Offset? alignmentOffset { get; private set; }
    public virtual DropdownMenuCloseBehavior closeBehavior { get; private set; } = default!;
    public virtual long? maxLines { get; private set; }
    public virtual global::Doroti.Framework.Services.TextInputAction? textInputAction { get; private set; }
    public virtual double? cursorHeight { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual global::Doroti.Framework.Widgets.MenuController? menuController { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsets scrollPadding { get; private set; } = default!;

    public DropdownMenu(global::Doroti.Framework.Foundation.Key? key = null, bool enabled = true, double? width = null, double? menuHeight = null, global::Doroti.Framework.Widgets.Widget? leadingIcon = null, global::Doroti.Framework.Widgets.Widget? trailingIcon = null, bool showTrailingIcon = true, global::Doroti.Framework.Widgets.FocusNode? trailingIconFocusNode = null, global::Doroti.Framework.Widgets.Widget? label = null, string? hintText = null, string? helperText = null, string? errorText = null, global::Doroti.Framework.Widgets.Widget? selectedTrailingIcon = null, bool enableFilter = false, bool enableSearch = true, global::Doroti.Framework.Services.TextInputType? keyboardType = null, global::Doroti.Framework.Painting.TextStyle? textStyle = null, TextAlign textAlign = TextAlign.start, object? inputDecorationTheme = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.MenuController, InputDecoration>? decorationBuilder = null, MenuStyle? menuStyle = null, global::Doroti.Framework.Widgets.TextEditingController? controller = null, T? initialSelection = default, global::System.Action<T?>? onSelected = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool? requestFocusOnTap = null, bool selectOnly = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? expandedInsets = null, global::System.Func<List<DropdownMenuEntry<T>>, string, List<DropdownMenuEntry<T>>>? filterCallback = null, global::System.Func<List<DropdownMenuEntry<T>>, string, long?>? searchCallback = null, Offset? alignmentOffset = null, List<DropdownMenuEntry<T>> dropdownMenuEntries = default!, List<global::Doroti.Framework.Services.TextInputFormatter>? inputFormatters = null, DropdownMenuCloseBehavior closeBehavior = DropdownMenuCloseBehavior.all, long? maxLines = 1, global::Doroti.Framework.Services.TextInputAction? textInputAction = null, double? cursorHeight = null, string? restorationId = null, global::Doroti.Framework.Widgets.MenuController? menuController = null, global::Doroti.Framework.Painting.EdgeInsets scrollPadding = default!) : base(key: key)
    {
        global::Doroti.Framework.Painting.EdgeInsets __scrollPadding = scrollPadding ?? global::Doroti.Framework.Painting.EdgeInsets.CreateAll(20.0);
        this.enabled = enabled;
        this.width = width;
        this.menuHeight = menuHeight;
        this.leadingIcon = leadingIcon;
        this.trailingIcon = trailingIcon;
        this.showTrailingIcon = showTrailingIcon;
        this.trailingIconFocusNode = trailingIconFocusNode;
        this.label = label;
        this.hintText = hintText;
        this.helperText = helperText;
        this.errorText = errorText;
        this.selectedTrailingIcon = selectedTrailingIcon;
        this.enableFilter = enableFilter;
        this.enableSearch = enableSearch;
        this.keyboardType = keyboardType;
        this.textStyle = textStyle;
        this.textAlign = textAlign;
        this.decorationBuilder = decorationBuilder;
        this.menuStyle = menuStyle;
        this.controller = controller;
        this.initialSelection = initialSelection;
        this.onSelected = onSelected;
        this.focusNode = focusNode;
        this.requestFocusOnTap = requestFocusOnTap;
        this.selectOnly = selectOnly;
        this.expandedInsets = expandedInsets;
        this.filterCallback = filterCallback;
        this.searchCallback = searchCallback;
        this.alignmentOffset = alignmentOffset;
        this.dropdownMenuEntries = dropdownMenuEntries;
        this.inputFormatters = inputFormatters;
        this.closeBehavior = closeBehavior;
        this.maxLines = maxLines;
        this.textInputAction = textInputAction;
        this.cursorHeight = cursorHeight;
        this.restorationId = restorationId;
        this.menuController = menuController;
        this.scrollPadding = __scrollPadding;
        this._inputDecorationTheme = inputDecorationTheme;
        System.Diagnostics.Debug.Assert(((filterCallback is null) || enableFilter));
        System.Diagnostics.Debug.Assert(((inputDecorationTheme is null) || (((inputDecorationTheme is InputDecorationTheme) || (inputDecorationTheme is InputDecorationThemeData)))));
        System.Diagnostics.Debug.Assert(((trailingIconFocusNode is null) || showTrailingIcon));
        System.Diagnostics.Debug.Assert(((decorationBuilder is null) || (((((label is null) && (hintText is null)) && (helperText is null)) && (errorText is null)))));
    }

    public virtual InputDecorationThemeData? inputDecorationTheme
    {
        get
        {
            if ((this._inputDecorationTheme is null))
            {
                return default;
            }
            return DartRuntimePrimitives.ConvertValue<InputDecorationThemeData>(this._inputDecorationTheme);
            return default!;
        }
    }
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DropdownMenuState__dropdown_menu<T>());
}

internal class _DropdownMenuState__dropdown_menu<T> : global::Doroti.Framework.Widgets.State<DropdownMenu<T>>
{
    internal static DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> _editableShortcuts = new DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> { [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowLeft)] = ((global::Doroti.Framework.Widgets.Intent)(object?)new global::Doroti.Framework.Widgets.ExtendSelectionByCharacterIntent(forward: false, collapseSelection: true)), [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowRight)] = ((global::Doroti.Framework.Widgets.Intent)(object?)new global::Doroti.Framework.Widgets.ExtendSelectionByCharacterIntent(forward: true, collapseSelection: true)), [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowUp)] = ((global::Doroti.Framework.Widgets.Intent)(object?)new _ArrowUpIntent__dropdown_menu()), [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowDown)] = ((global::Doroti.Framework.Widgets.Intent)(object?)new _ArrowDownIntent__dropdown_menu()) };
    internal static DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> _selectOnlyShortcuts = new DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> { [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowUp)] = ((global::Doroti.Framework.Widgets.Intent)(object?)new _ArrowUpIntent__dropdown_menu()), [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowDown)] = ((global::Doroti.Framework.Widgets.Intent)(object?)new _ArrowDownIntent__dropdown_menu()), [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.enter)] = ((global::Doroti.Framework.Widgets.Intent)(object?)new _EnterIntent__dropdown_menu()) };
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _anchorKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _leadingKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create();
    public virtual List<global::Doroti.Framework.Widgets.GlobalKey<IState>> buttonItemKeys { get; set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.MenuController _controller { get; set; } = default!;
    internal virtual bool _enableFilter { get; set; } = false;
    internal virtual bool _enableSearch { get; set; } = default!;
    public virtual List<DropdownMenuEntry<T>> filteredEntries { get; set; } = default!;
    internal virtual List<global::Doroti.Framework.Widgets.Widget>? _initialMenu { get; set; } = default;
    public virtual long? currentHighlight { get; set; } = default;
    public virtual double? leadingPadding { get; set; } = default;
    internal virtual bool _menuHasEnabledItem { get; set; } = false;
    internal virtual global::Doroti.Framework.Widgets.TextEditingController? _localTextEditingController { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.FocusNode _internalFocusNode { get; private set; } = new global::Doroti.Framework.Widgets.FocusNode();
    internal virtual global::Doroti.Framework.Widgets.WidgetStatesController? _highlightedItemStatesController { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.FocusNode? _localTrailingIconButtonFocusNode { get; set; } = default;

    internal virtual global::Doroti.Framework.Widgets.TextEditingController _effectiveTextEditingController => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.TextEditingController>((((DropdownMenu<T>)(object)this.widget).controller ?? (_localTextEditingController ??= new global::Doroti.Framework.Widgets.TextEditingController())));
    internal virtual global::Doroti.Framework.Widgets.FocusNode _trailingIconButtonFocusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.FocusNode>((((DropdownMenu<T>)(object)this.widget).trailingIconFocusNode ?? (_localTrailingIconButtonFocusNode ??= new global::Doroti.Framework.Widgets.FocusNode())));
    public override void initState()
    {
        base.initState();
        _enableSearch = ((DropdownMenu<T>)(object)this.widget).enableSearch;
        filteredEntries = ((DropdownMenu<T>)(object)this.widget).dropdownMenuEntries;
        buttonItemKeys = DartRuntimePrimitives.CreateList<global::Doroti.Framework.Widgets.GlobalKey<IState>>(checked((long)(this.filteredEntries.Count)), ((index) => global::Doroti.Framework.Widgets.GlobalKey<IState>.Create()));
        _menuHasEnabledItem = this.filteredEntries.any(((entry) => ((DropdownMenuEntry<T>)entry).enabled));
        long indexLocal = this.filteredEntries.indexWhere(((entry) => EqualityComparer<T>.Default.Equals(((DropdownMenuEntry<T>)entry).value, ((DropdownMenu<T>)(object)this.widget).initialSelection)));
        if ((indexLocal != -1L))
        {
            this._effectiveTextEditingController.value = new global::Doroti.Framework.Services.TextEditingValue(text: this.filteredEntries[(int)(indexLocal)].label, selection: global::Doroti.Framework.Services.TextSelection.CreateCollapsed(offset: this.filteredEntries[(int)(indexLocal)].label.Length));
        }
        refreshLeadingPadding();
        _controller = (((DropdownMenu<T>)(object)this.widget).menuController ?? new global::Doroti.Framework.Widgets.MenuController());
    }

    public override void dispose()
    {
        this._localTextEditingController?.dispose();
        _localTextEditingController = null;
        this._internalFocusNode.dispose();
        this._localTrailingIconButtonFocusNode?.dispose();
        _localTrailingIconButtonFocusNode = null;
        this._highlightedItemStatesController?.dispose();
        base.dispose();
    }

    public override void didUpdateWidget(DropdownMenu<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((DropdownMenu<T>)oldWidget).controller, ((DropdownMenu<T>)(object)this.widget).controller)))
        {
            this._localTextEditingController?.dispose();
            _localTextEditingController = null;
        }
        if ((((DropdownMenu<T>)oldWidget).enableFilter != ((DropdownMenu<T>)(object)this.widget).enableFilter))
        {
            if (!((DropdownMenu<T>)(object)this.widget).enableFilter)
            {
                _enableFilter = false;
            }
        }
        if ((((DropdownMenu<T>)oldWidget).enableSearch != ((DropdownMenu<T>)(object)this.widget).enableSearch))
        {
            if (!((DropdownMenu<T>)(object)this.widget).enableSearch)
            {
                _enableSearch = ((DropdownMenu<T>)(object)this.widget).enableSearch;
                currentHighlight = null;
            }
        }
        if ((!object.Equals(((DropdownMenu<T>)oldWidget).dropdownMenuEntries, ((DropdownMenu<T>)(object)this.widget).dropdownMenuEntries)))
        {
            currentHighlight = null;
            filteredEntries = ((DropdownMenu<T>)(object)this.widget).dropdownMenuEntries;
            buttonItemKeys = DartRuntimePrimitives.CreateList<global::Doroti.Framework.Widgets.GlobalKey<IState>>(checked((long)(this.filteredEntries.Count)), ((index) => global::Doroti.Framework.Widgets.GlobalKey<IState>.Create()));
            _menuHasEnabledItem = this.filteredEntries.any(((entry) => ((DropdownMenuEntry<T>)entry).enabled));
        }
        if ((!object.Equals(((DropdownMenu<T>)oldWidget).leadingIcon, ((DropdownMenu<T>)(object)this.widget).leadingIcon)))
        {
            refreshLeadingPadding();
        }
        if (!EqualityComparer<T>.Default.Equals(((DropdownMenu<T>)oldWidget).initialSelection, ((DropdownMenu<T>)(object)this.widget).initialSelection))
        {
            long indexLocal = this.filteredEntries.indexWhere(((entry) => EqualityComparer<T>.Default.Equals(((DropdownMenuEntry<T>)entry).value, ((DropdownMenu<T>)(object)this.widget).initialSelection)));
            if ((indexLocal != -1L))
            {
                this._effectiveTextEditingController.value = new global::Doroti.Framework.Services.TextEditingValue(text: this.filteredEntries[(int)(indexLocal)].label, selection: global::Doroti.Framework.Services.TextSelection.CreateCollapsed(offset: this.filteredEntries[(int)(indexLocal)].label.Length));
            }
        }
        if ((!object.Equals(((DropdownMenu<T>)oldWidget).menuController, ((DropdownMenu<T>)(object)this.widget).menuController)))
        {
            _controller = (((DropdownMenu<T>)(object)this.widget).menuController ?? new global::Doroti.Framework.Widgets.MenuController());
        }
    }

    public virtual bool canRequestFocus()
    {
        return ((((DropdownMenu<T>)(object)this.widget).focusNode?.canRequestFocus ?? ((DropdownMenu<T>)(object)this.widget).requestFocusOnTap) ?? (Theme.of(this.context).platform switch { global::Doroti.Framework.Foundation.TargetPlatform.iOS or global::Doroti.Framework.Foundation.TargetPlatform.android => false, global::Doroti.Framework.Foundation.TargetPlatform.fuchsia => false, global::Doroti.Framework.Foundation.TargetPlatform.macOS or global::Doroti.Framework.Foundation.TargetPlatform.linux => true, global::Doroti.Framework.Foundation.TargetPlatform.windows => true, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool selectOnly => ((DropdownMenu<T>)(object)this.widget).selectOnly;
    public virtual bool isButton => DartRuntimePrimitives.ConvertValue<bool>((!canRequestFocus() || this.selectOnly));
    public virtual void refreshLeadingPadding()
    {
        global::Doroti.Framework.Widgets.WidgetsBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) =>
        {
            if (!this.mounted)
            {
                return;
            }
            setState(((global::System.Action)(() =>
            {
                leadingPadding = getWidth(this._leadingKey);
            })));
        })), debugLabel: "DropdownMenu.refreshLeadingPadding");
    }

    public virtual void scrollToHighlight()
    {
        global::Doroti.Framework.Widgets.WidgetsBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) =>
        {
            global::Doroti.Framework.Widgets.BuildContext? highlightContext = this.buttonItemKeys[(int)(DartRuntimePrimitives.RequireValue(this.currentHighlight))].currentContext;
            if ((highlightContext is not null))
            {
                DartRuntimePrimitives.Ignore(Scrollable.of(highlightContext).position.ensureVisible(highlightContext.findRenderObject()!));
            }
        })), debugLabel: "DropdownMenu.scrollToHighlight");
    }

    public virtual double? getWidth(global::Doroti.Framework.Widgets.GlobalKey<IState> key)
    {
        global::Doroti.Framework.Widgets.BuildContext? context = ((global::Doroti.Framework.Widgets.GlobalKey<IState>)key).currentContext;
        if ((context is not null))
        {
            var box = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)context.findRenderObject()!)!;
            return (((global::Doroti.Framework.Rendering.RenderBox)box).hasSize ? ((global::Doroti.Framework.Rendering.RenderBox)box).size.width : null);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<DropdownMenuEntry<T>> filter(List<DropdownMenuEntry<T>> entries, global::Doroti.Framework.Widgets.TextEditingController textEditingController)
    {
        string filterText = ((global::Doroti.Framework.Widgets.TextEditingController)textEditingController).text.toLowerCase();
        return entries.where(((entry) => ((DropdownMenuEntry<T>)entry).label.toLowerCase().contains(filterText))).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _shouldUpdateCurrentHighlight(List<DropdownMenuEntry<T>> entries)
    {
        string searchText = this._effectiveTextEditingController.value.text.toLowerCase();
        if ((searchText.Length == 0))
        {
            return true;
        }
        if (((this.currentHighlight is null) || (DartRuntimePrimitives.RequireValue(this.currentHighlight) >= checked((long)(entries.Count)))))
        {
            return true;
        }
        if (entries[(int)(DartRuntimePrimitives.RequireValue(this.currentHighlight))].label.toLowerCase().contains(searchText))
        {
            return false;
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long? search(List<DropdownMenuEntry<T>> entries, global::Doroti.Framework.Widgets.TextEditingController textEditingController)
    {
        string searchText = textEditingController.value.text.toLowerCase();
        if ((searchText.Length == 0))
        {
            return null;
        }
        long index = entries.indexWhere(((entry) => ((DropdownMenuEntry<T>)entry).label.toLowerCase().contains(searchText)));
        return ((index != -1L) ? index : null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<global::Doroti.Framework.Widgets.Widget> _buildButtons(List<DropdownMenuEntry<T>> filteredEntries, TextDirection textDirection, long? focusedIndex = null, bool enableScrollToHighlight = true, bool excludeSemantics = false, bool? useMaterial3 = null)
    {
        double effectiveInputStartGap = ((useMaterial3 ?? false) ? Dropdown_menuLibrary._kInputStartGap : 0.0);
        var result = new List<global::Doroti.Framework.Widgets.Widget>();
        for (var i = 0L; (i < checked((long)(filteredEntries.Count))); i++)
        {
            DropdownMenuEntry<T> entry = filteredEntries[(int)(i)];
            double paddingLocal = ((((DropdownMenuEntry<T>)entry).leadingIcon is null) ? ((this.leadingPadding ?? Dropdown_menuLibrary._kDefaultHorizontalPadding)) : Dropdown_menuLibrary._kDefaultHorizontalPadding);
            ButtonStyle effectiveStyle = ((((DropdownMenuEntry<T>)entry).style ?? (ButtonStyle)MenuItemButton.styleFrom(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: paddingLocal, end: Dropdown_menuLibrary._kDefaultHorizontalPadding))));
            ButtonStyle? themeStyle = MenuButtonTheme.of(this.context).style;
            global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? effectiveForegroundColor = ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>?)(object?)(((DropdownMenuEntry<T>)entry).style?.foregroundColor ?? themeStyle?.foregroundColor));
            global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? effectiveIconColor = ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>?)(object?)(((DropdownMenuEntry<T>)entry).style?.iconColor ?? themeStyle?.iconColor));
            global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? effectiveOverlayColor = ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>?)(object?)(((DropdownMenuEntry<T>)entry).style?.overlayColor ?? themeStyle?.overlayColor));
            global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? effectiveBackgroundColor = ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>?)(object?)(((DropdownMenuEntry<T>)entry).style?.backgroundColor ?? themeStyle?.backgroundColor));
            bool entryIsSelected = (((DropdownMenuEntry<T>)entry).enabled && (i == focusedIndex));
            if (entryIsSelected)
            {
                this._highlightedItemStatesController?.dispose();
                _highlightedItemStatesController = new global::Doroti.Framework.Widgets.WidgetStatesController(new HashSet<global::Doroti.Framework.Widgets.WidgetState> { global::Doroti.Framework.Widgets.WidgetState.focused });
                ButtonStyle defaultStyle = ((ButtonStyle)(object?)new MenuItemButton().defaultStyleOf(this.context));
                Color? resolveFocusedColor(global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? colorStateProperty)
                {
                    return ((Color?)(object?)colorStateProperty?.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState> { global::Doroti.Framework.Widgets.WidgetState.focused }));
                    throw new InvalidOperationException("Dart control flow completed without a value.");
                }
                global::Doroti.Ui.Color focusedForegroundColor = ((global::Doroti.Ui.Color)(object?)resolveFocusedColor((effectiveForegroundColor ?? defaultStyle.foregroundColor!))!);
                global::Doroti.Ui.Color focusedIconColor = ((global::Doroti.Ui.Color)(object?)resolveFocusedColor((effectiveIconColor ?? defaultStyle.iconColor!))!);
                global::Doroti.Ui.Color focusedOverlayColor = ((global::Doroti.Ui.Color)(object?)resolveFocusedColor((effectiveOverlayColor ?? defaultStyle.overlayColor!))!);
                global::Doroti.Ui.Color focusedBackgroundColor = ((global::Doroti.Ui.Color)(object?)(resolveFocusedColor(effectiveBackgroundColor) ?? Theme.of(this.context).colorScheme.onSurface.withOpacity(0.12)));
                effectiveStyle = effectiveStyle.copyWith(backgroundColor: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Color>(focusedBackgroundColor), foregroundColor: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Color>(focusedForegroundColor), iconColor: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Color>(focusedIconColor), overlayColor: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Color>(focusedOverlayColor));
            }
            else
            {
                effectiveStyle = effectiveStyle.copyWith(backgroundColor: effectiveBackgroundColor, foregroundColor: effectiveForegroundColor, iconColor: effectiveIconColor, overlayColor: effectiveOverlayColor);
            }
            global::Doroti.Framework.Widgets.Widget labelLocal = (((DropdownMenuEntry<T>)entry).labelWidget ?? new global::Doroti.Framework.Widgets.Text(((DropdownMenuEntry<T>)entry).label));
            if ((((DropdownMenu<T>)(object)this.widget).width is not null))
            {
                double horizontalPadding = ((paddingLocal + Dropdown_menuLibrary._kDefaultHorizontalPadding) + effectiveInputStartGap);
                labelLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(maxWidth: (DartRuntimePrimitives.RequireValue(((DropdownMenu<T>)(object)this.widget).width) - horizontalPadding)), child: labelLocal));
            }
            global::Doroti.Framework.Widgets.Widget menuItemButton = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ExcludeFocus(child: new global::Doroti.Framework.Widgets.ExcludeSemantics(excluding: excludeSemantics, child: new MenuItemButton(key: (enableScrollToHighlight ? this.buttonItemKeys[(int)(i)] : null), statesController: (entryIsSelected ? this._highlightedItemStatesController : null), style: effectiveStyle, leadingIcon: ((DropdownMenuEntry<T>)entry).leadingIcon, trailingIcon: ((DropdownMenuEntry<T>)entry).trailingIcon, closeOnActivate: (object.Equals(((DropdownMenu<T>)(object)this.widget).closeBehavior, DropdownMenuCloseBehavior.all)), onPressed: ((global::System.Action)((((DropdownMenuEntry<T>)entry).enabled && ((DropdownMenu<T>)(object)this.widget).enabled) ? (() =>
            {
                if (!this.mounted)
                {
                    ((DropdownMenu<T>)(object)this.widget).controller?.value = new global::Doroti.Framework.Services.TextEditingValue(text: ((DropdownMenuEntry<T>)entry).label, selection: global::Doroti.Framework.Services.TextSelection.CreateCollapsed(offset: ((DropdownMenuEntry<T>)entry).label.Length));
                    ((DropdownMenu<T>)(object)this.widget).onSelected?.Invoke(((DropdownMenuEntry<T>)entry).value);
                    return;
                }
                this._effectiveTextEditingController.value = new global::Doroti.Framework.Services.TextEditingValue(text: ((DropdownMenuEntry<T>)entry).label, selection: global::Doroti.Framework.Services.TextSelection.CreateCollapsed(offset: ((DropdownMenuEntry<T>)entry).label.Length));
                currentHighlight = (((DropdownMenu<T>)(object)this.widget).enableSearch ? i : null);
                ((DropdownMenu<T>)(object)this.widget).onSelected?.Invoke(((DropdownMenuEntry<T>)entry).value);
                _enableFilter = false;
                if ((object.Equals(((DropdownMenu<T>)(object)this.widget).closeBehavior, DropdownMenuCloseBehavior.self)))
                {
                    this._controller.close();
                }
            }) : null)), requestFocusOnHover: false, child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: effectiveInputStartGap), child: labelLocal)))));
            result.Add(menuItemButton);
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleUpKey(_ArrowUpIntent__dropdown_menu __unused0)
    {
        setState(((global::System.Action)(() =>
        {
            if (((!((DropdownMenu<T>)(object)this.widget).enabled || !this._menuHasEnabledItem) || !((global::Doroti.Framework.Widgets.MenuController)this._controller).isOpen))
            {
                return;
            }
            _enableFilter = false;
            _enableSearch = false;
            currentHighlight ??= 0L;
            currentHighlight = (((DartRuntimePrimitives.RequireValue(this.currentHighlight) - 1L)) % checked((long)(this.filteredEntries.Count)));
            while (!this.filteredEntries[(int)(DartRuntimePrimitives.RequireValue(this.currentHighlight))].enabled)
            {
                currentHighlight = (((DartRuntimePrimitives.RequireValue(this.currentHighlight) - 1L)) % checked((long)(this.filteredEntries.Count)));
            }
            string currentLabel = this.filteredEntries[(int)(DartRuntimePrimitives.RequireValue(this.currentHighlight))].label;
            this._effectiveTextEditingController.value = new global::Doroti.Framework.Services.TextEditingValue(text: currentLabel, selection: global::Doroti.Framework.Services.TextSelection.CreateCollapsed(offset: currentLabel.Length));
        })));
    }

    public virtual void handleDownKey(_ArrowDownIntent__dropdown_menu __unused0)
    {
        setState(((global::System.Action)(() =>
        {
            if (((!((DropdownMenu<T>)(object)this.widget).enabled || !this._menuHasEnabledItem) || !((global::Doroti.Framework.Widgets.MenuController)this._controller).isOpen))
            {
                return;
            }
            _enableFilter = false;
            _enableSearch = false;
            currentHighlight ??= -1L;
            currentHighlight = (((DartRuntimePrimitives.RequireValue(this.currentHighlight) + 1L)) % checked((long)(this.filteredEntries.Count)));
            while (!this.filteredEntries[(int)(DartRuntimePrimitives.RequireValue(this.currentHighlight))].enabled)
            {
                currentHighlight = (((DartRuntimePrimitives.RequireValue(this.currentHighlight) + 1L)) % checked((long)(this.filteredEntries.Count)));
            }
            string currentLabel = this.filteredEntries[(int)(DartRuntimePrimitives.RequireValue(this.currentHighlight))].label;
            this._effectiveTextEditingController.value = new global::Doroti.Framework.Services.TextEditingValue(text: currentLabel, selection: global::Doroti.Framework.Services.TextSelection.CreateCollapsed(offset: currentLabel.Length));
        })));
    }

    public virtual void handleEnterKey(_EnterIntent__dropdown_menu __unused0)
    {
        if ((this.selectOnly && !((global::Doroti.Framework.Widgets.MenuController)this._controller).isOpen))
        {
            this._controller.open();
            return;
        }
        _handleSubmitted();
    }

    public virtual void handlePressed(global::Doroti.Framework.Widgets.MenuController controller, bool focusForKeyboard = true)
    {
        if (((global::Doroti.Framework.Widgets.MenuController)controller).isOpen)
        {
            currentHighlight = null;
            controller.close();
        }
        else
        {
            filteredEntries = ((DropdownMenu<T>)(object)this.widget).dropdownMenuEntries;
            if ((((global::Doroti.Framework.Widgets.TextEditingController)this._effectiveTextEditingController).text.Length != 0))
            {
                _enableFilter = false;
            }
            controller.open();
            if (focusForKeyboard)
            {
                this._internalFocusNode.requestFocus();
            }
        }
        setState(((global::System.Action)(() =>
        {
        })));
    }

    internal virtual void _handleSubmitted()
    {
        if ((this.currentHighlight is not null))
        {
            DropdownMenuEntry<T> entry = this.filteredEntries[(int)(DartRuntimePrimitives.RequireValue(this.currentHighlight))];
            if (((DropdownMenuEntry<T>)entry).enabled)
            {
                this._effectiveTextEditingController.value = new global::Doroti.Framework.Services.TextEditingValue(text: ((DropdownMenuEntry<T>)entry).label, selection: global::Doroti.Framework.Services.TextSelection.CreateCollapsed(offset: ((DropdownMenuEntry<T>)entry).label.Length));
                ((DropdownMenu<T>)(object)this.widget).onSelected?.Invoke(((DropdownMenuEntry<T>)entry).value);
            }
        }
        else
        {
            if (((global::Doroti.Framework.Widgets.MenuController)this._controller).isOpen)
            {
                ((DropdownMenu<T>)(object)this.widget).onSelected?.Invoke(default);
            }
        }
        if (!((DropdownMenu<T>)(object)this.widget).enableSearch)
        {
            currentHighlight = null;
        }
        this._controller.close();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        bool useMaterial3Local = Theme.of(context).useMaterial3;
        global::Doroti.Ui.TextDirection textDirection = Directionality.of(context);
        _initialMenu ??= _buildButtons(((DropdownMenu<T>)(object)this.widget).dropdownMenuEntries, textDirection, enableScrollToHighlight: false, excludeSemantics: true, useMaterial3: DartRuntimePrimitives.RequireValue(useMaterial3Local));
        DropdownMenuThemeData theme = DropdownMenuTheme.of(context);
        DropdownMenuThemeData defaults = ((DropdownMenuThemeData)(object?)new _DropdownMenuDefaultsM3__dropdown_menu(context));
        if (this._enableFilter)
        {
            filteredEntries = ((((DropdownMenu<T>)(object)this.widget).filterCallback is null ? filter(((DropdownMenu<T>)(object)this.widget).dropdownMenuEntries, this._effectiveTextEditingController) : ((DropdownMenu<T>)(object)this.widget).filterCallback.Invoke(this.filteredEntries, ((global::Doroti.Framework.Widgets.TextEditingController)this._effectiveTextEditingController).text)));
        }
        _menuHasEnabledItem = this.filteredEntries.any(((entry) => ((DropdownMenuEntry<T>)entry).enabled));
        if (this._enableSearch)
        {
            if ((((DropdownMenu<T>)(object)this.widget).searchCallback is not null))
            {
                currentHighlight = ((DropdownMenu<T>)(object)this.widget).searchCallback!(this.filteredEntries, ((global::Doroti.Framework.Widgets.TextEditingController)this._effectiveTextEditingController).text);
            }
            else
            {
                bool shouldUpdateCurrentHighlight = _shouldUpdateCurrentHighlight(this.filteredEntries);
                if (shouldUpdateCurrentHighlight)
                {
                    currentHighlight = search(this.filteredEntries, this._effectiveTextEditingController);
                }
            }
            if ((this.currentHighlight is not null))
            {
                scrollToHighlight();
            }
        }
        List<global::Doroti.Framework.Widgets.Widget> menu = ((List<global::Doroti.Framework.Widgets.Widget>)(object?)_buildButtons(this.filteredEntries, textDirection, focusedIndex: this.currentHighlight, useMaterial3: DartRuntimePrimitives.RequireValue(useMaterial3Local)));
        global::Doroti.Framework.Painting.TextStyle? baseTextStyle = ((((DropdownMenu<T>)(object)this.widget).textStyle ?? theme.textStyle) ?? defaults.textStyle);
        global::Doroti.Ui.Color? disabledColorLocal = ((global::Doroti.Ui.Color?)(object?)(theme.disabledColor ?? defaults.disabledColor));
        global::Doroti.Framework.Painting.TextStyle? effectiveTextStyle = (((DropdownMenu<T>)(object)this.widget).enabled ? baseTextStyle : (baseTextStyle?.copyWith(color: disabledColorLocal) ?? new global::Doroti.Framework.Painting.TextStyle(color: disabledColorLocal)));
        MenuStyle? effectiveMenuStyle = ((((DropdownMenu<T>)(object)this.widget).menuStyle ?? theme.menuStyle) ?? defaults.menuStyle!);
        double? anchorWidth = getWidth(this._anchorKey);
        if ((((DropdownMenu<T>)(object)this.widget).width is not null))
        {
            effectiveMenuStyle = effectiveMenuStyle.copyWith(minimumSize: WidgetStateProperty.resolveWith<global::Doroti.Ui.Size?>((states) =>
            {
                double? effectiveMaximumWidth = effectiveMenuStyle!.maximumSize?.resolve(states)?.width;
                return new global::Doroti.Ui.Size(Math.Min(DartRuntimePrimitives.RequireValue(((DropdownMenu<T>)(object)this.widget).width), (effectiveMaximumWidth ?? DartRuntimePrimitives.RequireValue(((DropdownMenu<T>)(object)this.widget).width))), 0.0);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
        }
        else
        {
            if ((anchorWidth is not null))
            {
                double anchorWidth__45717__value46193 = DartRuntimePrimitives.RequireValue(anchorWidth);
                effectiveMenuStyle = effectiveMenuStyle.copyWith(minimumSize: WidgetStateProperty.resolveWith<global::Doroti.Ui.Size?>((states) =>
                {
                    double? effectiveMaximumWidthLocal = effectiveMenuStyle!.maximumSize?.resolve(states)?.width;
                    return new global::Doroti.Ui.Size(Math.Min(DartRuntimePrimitives.RequireValue(anchorWidth__45717__value46193), (effectiveMaximumWidthLocal ?? DartRuntimePrimitives.RequireValue(anchorWidth__45717__value46193))), 0.0);
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }));
            }
        }
        if ((((DropdownMenu<T>)(object)this.widget).menuHeight is not null))
        {
            effectiveMenuStyle = effectiveMenuStyle.copyWith(maximumSize: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Size>(new global::Doroti.Ui.Size(double.PositiveInfinity, DartRuntimePrimitives.RequireValue(((DropdownMenu<T>)(object)this.widget).menuHeight))));
        }
        InputDecorationThemeData effectiveInputDecorationTheme = ((((DropdownMenu<T>)(object)this.widget).inputDecorationTheme ?? theme.inputDecorationTheme) ?? defaults.inputDecorationTheme!);
        global::Doroti.Framework.Services.MouseCursor? effectiveMouseCursor = ((global::Doroti.Framework.Services.MouseCursor?)(object?)(((object)((DropdownMenu<T>)(object)this.widget).enabled) switch { true => (this.isButton ? global::Doroti.Framework.Services.SystemMouseCursors.click : global::Doroti.Framework.Services.SystemMouseCursors.text), false => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Services.SystemMouseCursor>(null), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        global::Doroti.Framework.Widgets.Widget menuAnchor = ((global::Doroti.Framework.Widgets.Widget)(object?)new MenuAnchor(style: effectiveMenuStyle, alignmentOffset: ((DropdownMenu<T>)(object)this.widget).alignmentOffset, reservedPadding: global::Doroti.Framework.Painting.EdgeInsets.zero, controller: this._controller, menuChildren: menu, crossAxisUnconstrained: false, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.MenuController, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>?)((context, controller, child) =>
        {
            DartRuntimePrimitives.Assert(() => (this._initialMenu is not null));
            global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.MenuController, InputDecoration> decorationBuilderLocal = ((((DropdownMenu<T>)(object)this.widget).decorationBuilder ?? (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.MenuController, InputDecoration>)this._buildDefaultDecoration));
            InputDecoration decorationLocal = decorationBuilderLocal(context, controller);
            if ((((InputDecoration)decorationLocal).suffixIcon is null))
            {
                decorationLocal = decorationLocal.copyWith(suffixIcon: _buildDefaultSuffixIcon(context, controller));
            }
            InputDecoration effectiveDecoration = ((InputDecoration)(object?)decorationLocal.applyDefaults(effectiveInputDecorationTheme));
            InputDecoration textFieldDecoration = ((((InputDecoration)effectiveDecoration).prefixIcon is null) ? effectiveDecoration : effectiveDecoration.copyWith(prefixIcon: new global::Doroti.Framework.Widgets.SizedBox(key: this._leadingKey, child: ((InputDecoration)effectiveDecoration).prefixIcon)));
            MaterialLocalizations localizations = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
            global::Doroti.Framework.Widgets.Widget textField = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(button: this.isButton, hint: ((object.Equals(Theme.of(context).platform, global::Doroti.Framework.Foundation.TargetPlatform.iOS)) ? (((global::Doroti.Framework.Widgets.MenuController)this._controller).isOpen ? ((MaterialLocalizations)localizations).collapsedHint : ((MaterialLocalizations)localizations).expandedHint) : null), expanded: ((global::Doroti.Framework.Widgets.MenuController)this._controller).isOpen, onExpand: ((global::System.Action)(((global::Doroti.Framework.Widgets.MenuController)this._controller).isOpen ? null : (() =>
            {
                this._controller.open();
            }))), onCollapse: ((global::System.Action)(!((global::Doroti.Framework.Widgets.MenuController)this._controller).isOpen ? null : (() =>
            {
                this._controller.close();
            }))), child: new global::Doroti.Framework.Widgets.ExcludeSemantics(excluding: (this.isButton && global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb), child: new TextField(key: this._anchorKey, enabled: ((DropdownMenu<T>)(object)this.widget).enabled, mouseCursor: effectiveMouseCursor, focusNode: ((DropdownMenu<T>)(object)this.widget).focusNode, canRequestFocus: canRequestFocus(), enableInteractiveSelection: !this.isButton, readOnly: this.isButton, keyboardType: ((DropdownMenu<T>)(object)this.widget).keyboardType, textAlign: ((DropdownMenu<T>)(object)this.widget).textAlign, textAlignVertical: global::Doroti.Framework.Painting.TextAlignVertical.center, maxLines: ((DropdownMenu<T>)(object)this.widget).maxLines, textInputAction: ((DropdownMenu<T>)(object)this.widget).textInputAction, cursorHeight: ((DropdownMenu<T>)(object)this.widget).cursorHeight, style: effectiveTextStyle, controller: this._effectiveTextEditingController, onSubmitted: ((_) => { _handleSubmitted(); }), onTap: ((global::System.Action)(!((DropdownMenu<T>)(object)this.widget).enabled ? null : (() =>
            {
                handlePressed(controller, focusForKeyboard: !canRequestFocus());
            }))), onChanged: ((text) =>
            {
                controller.open();
                setState(((global::System.Action)(() =>
                {
                    filteredEntries = ((DropdownMenu<T>)(object)this.widget).dropdownMenuEntries;
                    _enableFilter = ((DropdownMenu<T>)(object)this.widget).enableFilter;
                    _enableSearch = ((DropdownMenu<T>)(object)this.widget).enableSearch;
                })));
            }), inputFormatters: ((DropdownMenu<T>)(object)this.widget).inputFormatters, decoration: textFieldDecoration, restorationId: ((DropdownMenu<T>)(object)this.widget).restorationId, scrollPadding: ((DropdownMenu<T>)(object)this.widget).scrollPadding))));
            global::Doroti.Framework.Widgets.Widget? effectiveLabel = (((InputDecoration)effectiveDecoration).label ?? (((((InputDecoration)effectiveDecoration).labelText is not null) ? new global::Doroti.Framework.Widgets.Text(((InputDecoration)effectiveDecoration).labelText!) : null)));
            global::Doroti.Framework.Widgets.Widget body = ((((DropdownMenu<T>)(object)this.widget).expandedInsets is not null) ? textField : new _DropdownMenuBody__dropdown_menu(width: ((DropdownMenu<T>)(object)this.widget).width, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection52521 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection52521.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(textField)); __collection52521.AddRange(this._initialMenu!); if ((effectiveLabel is not null)) { __collection52521.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ExcludeSemantics(child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 4.0), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: effectiveTextStyle!, child: effectiveLabel))))); } __collection52521.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>((((InputDecoration)effectiveDecoration).suffixIcon ?? global::Doroti.Framework.Widgets.SizedBox.CreateShrink()))); __collection52521.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateAll(8.0), child: (((InputDecoration)effectiveDecoration).prefixIcon ?? global::Doroti.Framework.Widgets.SizedBox.CreateShrink())))); return __collection52521; }))()));
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Shortcuts(shortcuts: (this.selectOnly ? _selectOnlyShortcuts : _editableShortcuts), child: body));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
        if (((DropdownMenu<T>)(object)this.widget).expandedInsets is global::Doroti.Framework.Painting.EdgeInsetsGeometry paddingLocal)
        {
            menuAnchor = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: paddingLocal.clamp(global::Doroti.Framework.Painting.EdgeInsets.zero, global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(left: double.PositiveInfinity, right: double.PositiveInfinity).add(global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(end: double.PositiveInfinity, start: double.PositiveInfinity))), child: menuAnchor));
        }
        menuAnchor = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.AlignmentDirectional.topStart, widthFactor: 1.0, heightFactor: 1.0, child: menuAnchor));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Actions(actions: new DartMap<Type, dynamic> { [typeof(_ArrowUpIntent__dropdown_menu)] = new global::Doroti.Framework.Widgets.CallbackAction<_ArrowUpIntent__dropdown_menu>(onInvoke: (__arg0) => { ((global::System.Action<_ArrowUpIntent__dropdown_menu>)this.handleUpKey)(__arg0); return default!; }), [typeof(_ArrowDownIntent__dropdown_menu)] = new global::Doroti.Framework.Widgets.CallbackAction<_ArrowDownIntent__dropdown_menu>(onInvoke: (__arg0) => { ((global::System.Action<_ArrowDownIntent__dropdown_menu>)this.handleDownKey)(__arg0); return default!; }), [typeof(_EnterIntent__dropdown_menu)] = new global::Doroti.Framework.Widgets.CallbackAction<_EnterIntent__dropdown_menu>(onInvoke: (__arg0) => { ((global::System.Action<_EnterIntent__dropdown_menu>)this.handleEnterKey)(__arg0); return default!; }), [typeof(global::Doroti.Framework.Widgets.DismissIntent)] = new global::Doroti.Framework.Widgets.DismissMenuAction(controller: this._controller) }, child: new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Shortcuts(shortcuts: new DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> { [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowUp)] = ((global::Doroti.Framework.Widgets.Intent)(object?)new _ArrowUpIntent__dropdown_menu()), [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowDown)] = ((global::Doroti.Framework.Widgets.Intent)(object?)new _ArrowDownIntent__dropdown_menu()), [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.enter)] = ((global::Doroti.Framework.Widgets.Intent)(object?)new _EnterIntent__dropdown_menu()), [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.escape)] = ((global::Doroti.Framework.Widgets.Intent)(object?)new global::Doroti.Framework.Widgets.DismissIntent()) }, child: new global::Doroti.Framework.Widgets.Focus(focusNode: this._internalFocusNode, skipTraversal: true, child: global::Doroti.Framework.Widgets.SizedBox.CreateShrink()))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(menuAnchor) })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual InputDecoration _buildDefaultDecoration(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.MenuController controller)
    {
        return new InputDecoration(label: ((DropdownMenu<T>)(object)this.widget).label, hintText: ((DropdownMenu<T>)(object)this.widget).hintText, helperText: ((DropdownMenu<T>)(object)this.widget).helperText, errorText: ((DropdownMenu<T>)(object)this.widget).errorText, prefixIcon: ((DropdownMenu<T>)(object)this.widget).leadingIcon, suffixIcon: _buildDefaultSuffixIcon(context, controller));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget? _buildDefaultSuffixIcon(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.MenuController controller)
    {
        bool isCollapsedLocal = (((DropdownMenu<T>)(object)this.widget).inputDecorationTheme?.isCollapsed ?? false);
        return ((global::Doroti.Framework.Widgets.Widget?)(object?)(((DropdownMenu<T>)(object)this.widget).showTrailingIcon ? new global::Doroti.Framework.Widgets.Padding(padding: (isCollapsedLocal ? global::Doroti.Framework.Painting.EdgeInsets.zero : global::Doroti.Framework.Painting.EdgeInsets.CreateAll(4.0)), child: new global::Doroti.Framework.Widgets.ExcludeSemantics(excluding: this.isButton, child: new IconButton(focusNode: this._trailingIconButtonFocusNode, isSelected: ((global::Doroti.Framework.Widgets.MenuController)controller).isOpen, constraints: ((DropdownMenu<T>)(object)this.widget).inputDecorationTheme?.suffixIconConstraints, padding: (isCollapsedLocal ? global::Doroti.Framework.Painting.EdgeInsets.zero : null), icon: (((DropdownMenu<T>)(object)this.widget).trailingIcon ?? new global::Doroti.Framework.Widgets.Icon(Icons.arrow_drop_down)), selectedIcon: (((DropdownMenu<T>)(object)this.widget).selectedTrailingIcon ?? new global::Doroti.Framework.Widgets.Icon(Icons.arrow_drop_up)), onPressed: ((global::System.Action)(!((DropdownMenu<T>)(object)this.widget).enabled ? null : (() =>
        {
            handlePressed(controller);
        })))))) : null));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _ArrowUpIntent__dropdown_menu : global::Doroti.Framework.Widgets.Intent
{
    internal _ArrowUpIntent__dropdown_menu()
    {
    }

}

public class _ArrowDownIntent__dropdown_menu : global::Doroti.Framework.Widgets.Intent
{
    internal _ArrowDownIntent__dropdown_menu()
    {
    }

}

public class _EnterIntent__dropdown_menu : global::Doroti.Framework.Widgets.Intent
{
    internal _EnterIntent__dropdown_menu()
    {
    }

}

internal class _DropdownMenuBody__dropdown_menu : global::Doroti.Framework.Widgets.MultiChildRenderObjectWidget
{
    public virtual double? width { get; private set; }

    internal _DropdownMenuBody__dropdown_menu(List<global::Doroti.Framework.Widgets.Widget> children = default!, double? width = null) : base(children: children ?? new List<global::Doroti.Framework.Widgets.Widget>())
    {
        this.width = width;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderDropdownMenuBody__dropdown_menu(width: this.width));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderDropdownMenuBody__dropdown_menu)(object)renderObject;
        __renderObject.width = this.width;
    }

}

internal class _DropdownMenuBodyParentData__dropdown_menu : global::Doroti.Framework.Rendering.ContainerBoxParentData<global::Doroti.Framework.Rendering.RenderBox>
{
}

public class _RenderDropdownMenuBody__dropdown_menu : global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.ContainerRenderObjectMixin<global::Doroti.Framework.Rendering.RenderBox, _DropdownMenuBodyParentData__dropdown_menu>, global::Doroti.Framework.Rendering.RenderBoxContainerDefaultsMixin<global::Doroti.Framework.Rendering.RenderBox, _DropdownMenuBodyParentData__dropdown_menu>
{
    internal virtual double? _width { get; set; } = default;
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    internal _RenderDropdownMenuBody__dropdown_menu(double? width = null)
    {
        this._width = width;
    }

    public virtual double? width
    {
        get => this._width;
        set
        {
            var __value = value;
            if ((this._width == __value))
            {
                return;
            }
            _width = __value;
            markNeedsLayout();
        }
    }
    public override void setupParentData(global::Doroti.Framework.Rendering.RenderObject child)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        if ((__child.parentData is not _DropdownMenuBodyParentData__dropdown_menu))
        {
            __child.parentData = new _DropdownMenuBodyParentData__dropdown_menu();
        }
    }

    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.BoxConstraints constraintsLocal = this.constraints;
        var maxWidthLocal = 0.0;
        double? maxHeightLocal = default!;
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        double intrinsicWidth = ((this.width ?? (double)getMaxIntrinsicWidth(((global::Doroti.Framework.Rendering.BoxConstraints)constraintsLocal).maxHeight)));
        double widthConstraint = Math.Min(intrinsicWidth, ((global::Doroti.Framework.Rendering.BoxConstraints)constraintsLocal).maxWidth);
        var innerConstraints = new global::Doroti.Framework.Rendering.BoxConstraints(maxWidth: widthConstraint, maxHeight: getMaxIntrinsicHeight(widthConstraint));
        while ((child is not null))
        {
            if ((object.Equals(child, this.firstChild)))
            {
                child.layout(innerConstraints, parentUsesSize: true);
                maxHeightLocal ??= ((global::Doroti.Framework.Rendering.RenderBox)child).size.height;
                var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
                DartRuntimePrimitives.Assert(() => (object.Equals(child.parentData, childParentData)));
                child = childParentData.nextSibling;
                continue;
            }
            child.layout(innerConstraints, parentUsesSize: true);
            var childParentDataLocal = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
            childParentDataLocal.offset = Offset.zero;
            maxWidthLocal = Math.Max(maxWidthLocal, ((global::Doroti.Framework.Rendering.RenderBox)child).size.width);
            maxHeightLocal ??= ((global::Doroti.Framework.Rendering.RenderBox)child).size.height;
            DartRuntimePrimitives.Assert(() => (object.Equals(child.parentData, childParentDataLocal)));
            child = childParentDataLocal.nextSibling;
        }
        DartRuntimePrimitives.Assert(() => (maxHeightLocal is not null));
        maxWidthLocal = Math.Max(Dropdown_menuLibrary._kMinimumWidth, maxWidthLocal);
        size = constraintsLocal.constrain(new global::Doroti.Ui.Size((this.width ?? maxWidthLocal), DartRuntimePrimitives.RequireValue(maxHeightLocal)));
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        if ((child is not null))
        {
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
            context.paintChild(child, (offset + childParentData.offset));
        }
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        var maxWidthLocal = 0.0;
        double? maxHeightLocal = default!;
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        double intrinsicWidth = ((this.width ?? (double)getMaxIntrinsicWidth(((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxHeight)));
        double widthConstraint = Math.Min(intrinsicWidth, ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth);
        var innerConstraints = new global::Doroti.Framework.Rendering.BoxConstraints(maxWidth: widthConstraint, maxHeight: getMaxIntrinsicHeight(widthConstraint));
        while ((child is not null))
        {
            global::Doroti.Ui.Size childSize = ((global::Doroti.Ui.Size)(object?)child.getDryLayout(innerConstraints));
            if ((!object.Equals(child, this.firstChild)))
            {
                maxWidthLocal = Math.Max(maxWidthLocal, childSize.width);
            }
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
            maxHeightLocal ??= childSize.height;
            child = childParentData.nextSibling;
        }
        DartRuntimePrimitives.Assert(() => (maxHeightLocal is not null));
        maxWidthLocal = Math.Max(Dropdown_menuLibrary._kMinimumWidth, maxWidthLocal);
        return constraints.constrain(new global::Doroti.Ui.Size((this.width ?? maxWidthLocal), DartRuntimePrimitives.RequireValue(maxHeightLocal)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        double width = 0;
        while ((child is not null))
        {
            if ((object.Equals(child, this.firstChild)))
            {
                var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
                child = childParentData.nextSibling;
                continue;
            }
            double minIntrinsicWidth = child.getMinIntrinsicWidth(height);
            if ((object.Equals(child, this.lastChild)))
            {
                width += minIntrinsicWidth;
            }
            if ((object.Equals(child, childBefore(this.lastChild!))))
            {
                width += minIntrinsicWidth;
            }
            width = Math.Max(DartRuntimePrimitives.RequireValue(width), minIntrinsicWidth);
            var childParentDataLocal = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
            child = childParentDataLocal.nextSibling;
        }
        return Math.Max(DartRuntimePrimitives.RequireValue(width), Dropdown_menuLibrary._kMinimumWidth);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        double width = 0;
        while ((child is not null))
        {
            if ((object.Equals(child, this.firstChild)))
            {
                var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
                child = childParentData.nextSibling;
                continue;
            }
            double maxIntrinsicWidth = child.getMaxIntrinsicWidth(height);
            if ((object.Equals(child, this.lastChild)))
            {
                width += maxIntrinsicWidth;
            }
            if ((object.Equals(child, childBefore(this.lastChild!))))
            {
                width += maxIntrinsicWidth;
            }
            width = Math.Max(DartRuntimePrimitives.RequireValue(width), maxIntrinsicWidth);
            var childParentDataLocal = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
            child = childParentDataLocal.nextSibling;
        }
        return Math.Max(DartRuntimePrimitives.RequireValue(width), Dropdown_menuLibrary._kMinimumWidth);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        double widthLocal = 0;
        if ((child is not null))
        {
            widthLocal = Math.Max(DartRuntimePrimitives.RequireValue(widthLocal), child.getMinIntrinsicHeight(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(widthLocal))));
        }
        return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(widthLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        double widthLocal = 0;
        if ((child is not null))
        {
            widthLocal = Math.Max(DartRuntimePrimitives.RequireValue(widthLocal), child.getMaxIntrinsicHeight(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(widthLocal))));
        }
        return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(widthLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = this.firstChild;
        if ((child is not null))
        {
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
            bool isHit = result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: ((global::System.Func<global::Doroti.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData.offset))));
                return child.hitTest(result, position: transformed);
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            if (isHit)
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void visitChildrenForSemantics(global::System.Action<global::Doroti.Framework.Rendering.RenderObject> visitor)
    {
        visitChildren(((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)((renderObjectChild) =>
        {
            var child = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
            if ((object.Equals(child, this.firstChild)))
            {
                visitor(((global::Doroti.Framework.Rendering.RenderBox)renderObjectChild));
            }
        })));
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
        while ((childParentData.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.previousSibling, child)));
            child = childParentData.previousSibling!;
            childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
        while ((childParentData.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.nextSibling, child)));
            child = childParentData.nextSibling!;
            childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long childCount => this._childCount;
    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((child is not RenderBox))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new global::Doroti.Framework.Foundation.ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderBox)} child was created by", this.debugCreator, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((RenderObject)child).debugCreator, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)this._firstChild!.parentData!)!;
                firstChildParentData.previousSibling = child;
            }
            this._firstChild = child;
            this._lastChild ??= child;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (this._firstChild is not null));
            DartRuntimePrimitives.Assert(() => (this._lastChild is not null));
            DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(after, equals: this._firstChild));
            DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(after, equals: this._lastChild));
            var afterParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)after.parentData!)!;
            if ((afterParentData.nextSibling is null))
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(after, this._lastChild)));
                childParentData.previousSibling = after;
                afterParentData.nextSibling = child;
                this._lastChild = child;
            }
            else
            {
                childParentData.nextSibling = afterParentData.nextSibling;
                childParentData.previousSibling = after;
                var childPreviousSiblingParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)childParentData.nextSibling!.parentData!)!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData.nextSibling, child)));
            }
        }
    }

    public virtual void insert(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)), () => (object?)"A RenderObject cannot be inserted into itself.");
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)), () => (object?)"A RenderObject cannot simultaneously be both the parent and the sibling of another RenderObject.");
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)), () => (object?)"A RenderObject cannot be inserted after itself.");
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._firstChild)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._lastChild)));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => (child.parentData is _DropdownMenuBodyParentData__dropdown_menu), () => (object?)$"A child of {this.GetType()} has parentData of type {DartRuntimePrimitives.RuntimeType(child.parentData)}, " + $"which does not conform to {typeof(_DropdownMenuBodyParentData__dropdown_menu)}. Class using ContainerRenderObjectMixin " + $"should override setupParentData() to set parentData to type {typeof(_DropdownMenuBodyParentData__dropdown_menu)}.");
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderBox child)
    {
        insert(child, after: this._lastChild);
    }

    public virtual void addAll(List<RenderBox>? children)
    {
        children?.forEach((__arg0) => ((global::System.Action<RenderBox>)this.add)(__arg0));
    }

    public virtual void _removeFromChildList(RenderBox child)
    {
        var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: this._firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: this._lastChild));
        DartRuntimePrimitives.Assert(() => (this._childCount >= 0L));
        if ((childParentData.previousSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._firstChild, child)));
            this._firstChild = childParentData.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if ((childParentData.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)childParentData.nextSibling!.parentData!)!;
            childNextSiblingParentData.previousSibling = childParentData.previousSibling;
        }
        childParentData.previousSibling = null;
        childParentData.nextSibling = null;
        this._childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
            RenderBox? next = childParentData.nextSibling;
            childParentData.previousSibling = null;
            childParentData.nextSibling = null;
            dropChild(child);
            child = next;
        }
        this._firstChild = null;
        this._lastChild = null;
        this._childCount = 0L;
    }

    public virtual void move(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)));
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
        if ((object.Equals(childParentData.previousSibling, after)))
        {
            return;
        }
        _removeFromChildList(child);
        _insertIntoChildList(child, after: after);
        markNeedsLayout();
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            ((dynamic)child).attach(owner);
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            ((dynamic)child).detach();
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            redepthChild(child);
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(global::System.Action<RenderObject> visitor)
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            visitor(child);
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        if ((this.firstChild is not null))
        {
            RenderBox child = this.firstChild!;
            var count = 1L;
            while (true)
            {
                children.Add(((Diagnosticable)child).toDiagnosticsNode(name: $"child__183606 {count}"));
                if ((object.Equals(child, this.lastChild)))
                {
                    break;
                }
                count += 1L;
                var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
                child = childParentData.nextSibling!;
            }
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToFirstActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
            double? result = child.getDistanceToActualBaseline(baseline);
            if ((result is not null))
            {
                double result__138852__value138916 = DartRuntimePrimitives.RequireValue(result);
                return (DartRuntimePrimitives.RequireValue(result__138852__value138916) + childParentData.offset.dy);
            }
            child = childParentData.nextSibling;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToHighestActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        BaselineOffset minBaseline = BaselineOffset.noBaseline;
        RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
            BaselineOffset candidate = (new BaselineOffset(child.getDistanceToActualBaseline(baseline)).op_Add(childParentData.offset.dy));
            minBaseline = minBaseline.minOf(candidate);
            child = childParentData.nextSibling;
        }
        return minBaseline.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool defaultHitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child = this.lastChild;
        while ((child is not null))
        {
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
            bool isHit = result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: ((global::System.Func<BoxHitTestResult, Offset, bool>)((result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData.offset))));
                return child!.hitTest(result, position: transformed);
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            if (isHit)
            {
                return true;
            }
            child = childParentData.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void defaultPaint(PaintingContext context, Offset offset)
    {
        RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
            context.paintChild(child, (childParentData.offset + offset));
            child = childParentData.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result = new List<RenderBox>();
        RenderBox? child = this.firstChild;
        while ((child is not null))
        {
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
            result.Add(((RenderBox?)(object?)child)!);
            child = childParentData.nextSibling;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DropdownMenuDefaultsM3__dropdown_menu : DropdownMenuThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(this.context);
                __late__theme_initialized = true;
            }
            return __late__theme;
        }
    }

    internal _DropdownMenuDefaultsM3__dropdown_menu(global::Doroti.Framework.Widgets.BuildContext context) : base(disabledColor: Theme.of(context).colorScheme.onSurface.withOpacity(0.38))
    {
        this.context = context;
    }

    public override global::Doroti.Framework.Painting.TextStyle? textStyle => this._theme.textTheme.bodyLarge;
    public virtual MenuStyle menuStyle
    {
        get
        {
            return new MenuStyle(minimumSize: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Size>(new global::Doroti.Ui.Size(Dropdown_menuLibrary._kMinimumWidth, 0.0)), maximumSize: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Size>(Size.infinite), visualDensity: VisualDensity.standard);
            return default!;
        }
    }
    public virtual InputDecorationThemeData inputDecorationTheme
    {
        get
        {
            return new InputDecorationThemeData(border: new OutlineInputBorder());
            return default!;
        }
    }
}
