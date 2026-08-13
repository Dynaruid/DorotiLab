// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/dropdown_menu.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public delegate List<DropdownMenuEntry<T>> FilterCallback<T>(List<DropdownMenuEntry<T>> entries, string filter);

public delegate long? SearchCallback<T>(List<DropdownMenuEntry<T>> entries, string query);

public delegate InputDecoration DropdownMenuDecorationBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.MenuController controller);

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
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? labelWidget { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? leadingIcon { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? trailingIcon { get; private set; }
    public virtual bool enabled { get; private set; } = default!;
    public virtual ButtonStyle? style { get; private set; }

    public DropdownMenuEntry(T value, string label, global::Doroti.Generated.Framework.Widgets.Widget? labelWidget = null, global::Doroti.Generated.Framework.Widgets.Widget? leadingIcon = null, global::Doroti.Generated.Framework.Widgets.Widget? trailingIcon = null, bool enabled = true, ButtonStyle? style = null)
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

public class DropdownMenu<T> : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual bool enabled { get; private set; } = default!;
    public virtual double? width { get; private set; }
    public virtual double? menuHeight { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? leadingIcon { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? trailingIcon { get; private set; }
    public virtual bool showTrailingIcon { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode? trailingIconFocusNode { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? label { get; private set; }
    public virtual string? hintText { get; private set; }
    public virtual string? helperText { get; private set; }
    public virtual string? errorText { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? selectedTrailingIcon { get; private set; }
    public virtual bool enableFilter { get; private set; } = default!;
    public virtual bool enableSearch { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Services.TextInputType? keyboardType { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? textStyle { get; private set; }
    public virtual TextAlign textAlign { get; private set; } = default!;
    internal virtual object? _inputDecorationTheme { get; private set; }
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.MenuController, InputDecoration>? decorationBuilder { get; private set; }
    public virtual MenuStyle? menuStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.TextEditingController? controller { get; private set; }
    public virtual T? initialSelection { get; private set; }
    public virtual global::System.Action<T?>? onSelected { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool? requestFocusOnTap { get; private set; }
    public virtual bool selectOnly { get; private set; } = default!;
    public virtual List<DropdownMenuEntry<T>> dropdownMenuEntries { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? expandedInsets { get; private set; }
    public virtual global::System.Func<List<DropdownMenuEntry<T>>, string, List<DropdownMenuEntry<T>>>? filterCallback { get; private set; }
    public virtual global::System.Func<List<DropdownMenuEntry<T>>, string, long?>? searchCallback { get; private set; }
    public virtual List<global::Doroti.Generated.Framework.Services.TextInputFormatter>? inputFormatters { get; private set; }
    public virtual Offset? alignmentOffset { get; private set; }
    public virtual DropdownMenuCloseBehavior closeBehavior { get; private set; } = default!;
    public virtual long? maxLines { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.TextInputAction? textInputAction { get; private set; }
    public virtual double? cursorHeight { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.MenuController? menuController { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets scrollPadding { get; private set; } = default!;

    public DropdownMenu(global::Doroti.Generated.Framework.Foundation.Key? key = null, bool enabled = true, double? width = null, double? menuHeight = null, global::Doroti.Generated.Framework.Widgets.Widget? leadingIcon = null, global::Doroti.Generated.Framework.Widgets.Widget? trailingIcon = null, bool showTrailingIcon = true, global::Doroti.Generated.Framework.Widgets.FocusNode? trailingIconFocusNode = null, global::Doroti.Generated.Framework.Widgets.Widget? label = null, string? hintText = null, string? helperText = null, string? errorText = null, global::Doroti.Generated.Framework.Widgets.Widget? selectedTrailingIcon = null, bool enableFilter = false, bool enableSearch = true, global::Doroti.Generated.Framework.Services.TextInputType? keyboardType = null, global::Doroti.Generated.Framework.Painting.TextStyle? textStyle = null, TextAlign textAlign = TextAlign.start, object? inputDecorationTheme = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.MenuController, InputDecoration>? decorationBuilder = null, MenuStyle? menuStyle = null, global::Doroti.Generated.Framework.Widgets.TextEditingController? controller = null, T? initialSelection = default, global::System.Action<T?>? onSelected = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool? requestFocusOnTap = null, bool selectOnly = false, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? expandedInsets = null, global::System.Func<List<DropdownMenuEntry<T>>, string, List<DropdownMenuEntry<T>>>? filterCallback = null, global::System.Func<List<DropdownMenuEntry<T>>, string, long?>? searchCallback = null, Offset? alignmentOffset = null, List<DropdownMenuEntry<T>> dropdownMenuEntries = default!, List<global::Doroti.Generated.Framework.Services.TextInputFormatter>? inputFormatters = null, DropdownMenuCloseBehavior closeBehavior = DropdownMenuCloseBehavior.all, long? maxLines = 1, global::Doroti.Generated.Framework.Services.TextInputAction? textInputAction = null, double? cursorHeight = null, string? restorationId = null, global::Doroti.Generated.Framework.Widgets.MenuController? menuController = null, global::Doroti.Generated.Framework.Painting.EdgeInsets scrollPadding = default!) : base(key: key)
    {
        global::Doroti.Generated.Framework.Painting.EdgeInsets __scrollPadding = scrollPadding ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(20.0);
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

internal class _DropdownMenuState__dropdown_menu<T> : global::Doroti.Generated.Framework.Widgets.State<DropdownMenu<T>>
{
    internal static DartMap<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent> _editableShortcuts = new DartMap<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent> { [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowLeft)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new global::Doroti.Generated.Framework.Widgets.ExtendSelectionByCharacterIntent(forward: false, collapseSelection: true)), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowRight)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new global::Doroti.Generated.Framework.Widgets.ExtendSelectionByCharacterIntent(forward: true, collapseSelection: true)), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowUp)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new _ArrowUpIntent__dropdown_menu()), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowDown)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new _ArrowDownIntent__dropdown_menu()) };
    internal static DartMap<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent> _selectOnlyShortcuts = new DartMap<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent> { [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowUp)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new _ArrowUpIntent__dropdown_menu()), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowDown)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new _ArrowDownIntent__dropdown_menu()), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.enter)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new _EnterIntent__dropdown_menu()) };
    internal virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> _anchorKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> _leadingKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create();
    public virtual List<global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>> buttonItemKeys { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Widgets.MenuController _controller { get; set; } = default!;
    internal virtual bool _enableFilter { get; set; } = false;
    internal virtual bool _enableSearch { get; set; } = default!;
    public virtual List<DropdownMenuEntry<T>> filteredEntries { get; set; } = default!;
    internal virtual List<global::Doroti.Generated.Framework.Widgets.Widget>? _initialMenu { get; set; } = default;
    public virtual long? currentHighlight { get; set; } = default;
    public virtual double? leadingPadding { get; set; } = default;
    internal virtual bool _menuHasEnabledItem { get; set; } = false;
    internal virtual global::Doroti.Generated.Framework.Widgets.TextEditingController? _localTextEditingController { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Widgets.FocusNode _internalFocusNode { get; private set; } = new global::Doroti.Generated.Framework.Widgets.FocusNode();
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStatesController? _highlightedItemStatesController { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Widgets.FocusNode? _localTrailingIconButtonFocusNode { get; set; } = default;

    internal virtual global::Doroti.Generated.Framework.Widgets.TextEditingController _effectiveTextEditingController => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.TextEditingController>((((DropdownMenu<T>)(object)this.widget).controller ?? (_localTextEditingController ??= new global::Doroti.Generated.Framework.Widgets.TextEditingController())));
    internal virtual global::Doroti.Generated.Framework.Widgets.FocusNode _trailingIconButtonFocusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.FocusNode>((((DropdownMenu<T>)(object)this.widget).trailingIconFocusNode ?? (_localTrailingIconButtonFocusNode ??= new global::Doroti.Generated.Framework.Widgets.FocusNode())));
    public override void initState()
    {
        base.initState();
        _enableSearch = ((DropdownMenu<T>)(object)this.widget).enableSearch;
        filteredEntries = ((DropdownMenu<T>)(object)this.widget).dropdownMenuEntries;
        buttonItemKeys = DartRuntimePrimitives.CreateList<global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>>(checked((long)(this.filteredEntries.Count)), ((index) => global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create()));
        _menuHasEnabledItem = this.filteredEntries.any(((entry) => ((DropdownMenuEntry<T>)entry).enabled));
        long index__28492 = this.filteredEntries.indexWhere(((entry) => EqualityComparer<T>.Default.Equals(((DropdownMenuEntry<T>)entry).value, ((DropdownMenu<T>)(object)this.widget).initialSelection)));
        if ((index__28492 != -1L))
        {
            this._effectiveTextEditingController.value = new global::Doroti.Generated.Framework.Services.TextEditingValue(text: this.filteredEntries[(int)(index__28492)].label, selection: global::Doroti.Generated.Framework.Services.TextSelection.CreateCollapsed(offset: this.filteredEntries[(int)(index__28492)].label.Length));
        }
        refreshLeadingPadding();
        _controller = (((DropdownMenu<T>)(object)this.widget).menuController ?? new global::Doroti.Generated.Framework.Widgets.MenuController());
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
            buttonItemKeys = DartRuntimePrimitives.CreateList<global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>>(checked((long)(this.filteredEntries.Count)), ((index) => global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create()));
            _menuHasEnabledItem = this.filteredEntries.any(((entry) => ((DropdownMenuEntry<T>)entry).enabled));
        }
        if ((!object.Equals(((DropdownMenu<T>)oldWidget).leadingIcon, ((DropdownMenu<T>)(object)this.widget).leadingIcon)))
        {
            refreshLeadingPadding();
        }
        if (!EqualityComparer<T>.Default.Equals(((DropdownMenu<T>)oldWidget).initialSelection, ((DropdownMenu<T>)(object)this.widget).initialSelection))
        {
            long index__30361 = this.filteredEntries.indexWhere(((entry) => EqualityComparer<T>.Default.Equals(((DropdownMenuEntry<T>)entry).value, ((DropdownMenu<T>)(object)this.widget).initialSelection)));
            if ((index__30361 != -1L))
            {
                this._effectiveTextEditingController.value = new global::Doroti.Generated.Framework.Services.TextEditingValue(text: this.filteredEntries[(int)(index__30361)].label, selection: global::Doroti.Generated.Framework.Services.TextSelection.CreateCollapsed(offset: this.filteredEntries[(int)(index__30361)].label.Length));
            }
        }
        if ((!object.Equals(((DropdownMenu<T>)oldWidget).menuController, ((DropdownMenu<T>)(object)this.widget).menuController)))
        {
            _controller = (((DropdownMenu<T>)(object)this.widget).menuController ?? new global::Doroti.Generated.Framework.Widgets.MenuController());
        }
    }

    public virtual bool canRequestFocus()
    {
        return ((((DropdownMenu<T>)(object)this.widget).focusNode?.canRequestFocus ?? ((DropdownMenu<T>)(object)this.widget).requestFocusOnTap) ?? (Theme.of(this.context).platform switch { global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS or global::Doroti.Generated.Framework.Foundation.TargetPlatform.android => false, global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia => false, global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS or global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux => true, global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows => true, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool selectOnly => ((DropdownMenu<T>)(object)this.widget).selectOnly;
    public virtual bool isButton => DartRuntimePrimitives.ConvertValue<bool>((!canRequestFocus() || this.selectOnly));
    public virtual void refreshLeadingPadding()
    {
        global::Doroti.Generated.Framework.Widgets.WidgetsBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) => {
if (!this.mounted)
{
    return;
}
setState(((global::System.Action)(() => {
leadingPadding = getWidth(this._leadingKey);
})));
})), debugLabel: "DropdownMenu.refreshLeadingPadding");
    }

    public virtual void scrollToHighlight()
    {
        global::Doroti.Generated.Framework.Widgets.WidgetsBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) => {
global::Doroti.Generated.Framework.Widgets.BuildContext? highlightContext__31715 = this.buttonItemKeys[(int)(DartRuntimePrimitives.RequireValue(this.currentHighlight))].currentContext;
if ((highlightContext__31715 is not null))
{
    DartRuntimePrimitives.Ignore(Scrollable.of(highlightContext__31715).position.ensureVisible(highlightContext__31715.findRenderObject()!));
}
})), debugLabel: "DropdownMenu.scrollToHighlight");
    }

    public virtual double? getWidth(global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> key)
    {
        global::Doroti.Generated.Framework.Widgets.BuildContext? context__32072 = ((global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>)key).currentContext;
        if ((context__32072 is not null))
        {
            var box__32141 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)context__32072.findRenderObject()!)!;
            return (((global::Doroti.Generated.Framework.Rendering.RenderBox)box__32141).hasSize ? ((global::Doroti.Generated.Framework.Rendering.RenderBox)box__32141).size.width : null);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<DropdownMenuEntry<T>> filter(List<DropdownMenuEntry<T>> entries, global::Doroti.Generated.Framework.Widgets.TextEditingController textEditingController)
    {
        string filterText__32416 = ((global::Doroti.Generated.Framework.Widgets.TextEditingController)textEditingController).text.toLowerCase();
        return entries.where(((entry) => ((DropdownMenuEntry<T>)entry).label.toLowerCase().contains(filterText__32416))).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _shouldUpdateCurrentHighlight(List<DropdownMenuEntry<T>> entries)
    {
        string searchText__32701 = this._effectiveTextEditingController.value.text.toLowerCase();
        if ((searchText__32701.Length == 0))
        {
            return true;
        }
        if (((this.currentHighlight is null) || (DartRuntimePrimitives.RequireValue(this.currentHighlight) >= checked((long)(entries.Count)))))
        {
            return true;
        }
        if (entries[(int)(DartRuntimePrimitives.RequireValue(this.currentHighlight))].label.toLowerCase().contains(searchText__32701))
        {
            return false;
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long? search(List<DropdownMenuEntry<T>> entries, global::Doroti.Generated.Framework.Widgets.TextEditingController textEditingController)
    {
        string searchText__33307 = textEditingController.value.text.toLowerCase();
        if ((searchText__33307.Length == 0))
        {
            return null;
        }
        long index__33438 = entries.indexWhere(((entry) => ((DropdownMenuEntry<T>)entry).label.toLowerCase().contains(searchText__33307)));
        return ((index__33438 != -1L) ? index__33438 : null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<global::Doroti.Generated.Framework.Widgets.Widget> _buildButtons(List<DropdownMenuEntry<T>> filteredEntries, TextDirection textDirection, long? focusedIndex = null, bool enableScrollToHighlight = true, bool excludeSemantics = false, bool? useMaterial3 = null)
    {
        double effectiveInputStartGap__33864 = ((useMaterial3 ?? false) ? Dropdown_menuLibrary._kInputStartGap : 0.0);
        var result__33946 = new List<global::Doroti.Generated.Framework.Widgets.Widget>();
        for (var i__33980 = 0L; (i__33980 < checked((long)(filteredEntries.Count))); i__33980++)
        {
            DropdownMenuEntry<T> entry__34055 = filteredEntries[(int)(i__33980)];
            double padding__34534 = ((((DropdownMenuEntry<T>)entry__34055).leadingIcon is null) ? ((this.leadingPadding ?? Dropdown_menuLibrary._kDefaultHorizontalPadding)) : Dropdown_menuLibrary._kDefaultHorizontalPadding);
            ButtonStyle effectiveStyle__34687 = ((((DropdownMenuEntry<T>)entry__34055).style ?? (ButtonStyle)MenuItemButton.styleFrom(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: padding__34534, end: Dropdown_menuLibrary._kDefaultHorizontalPadding))));
            ButtonStyle? themeStyle__34902 = MenuButtonTheme.of(this.context).style;
            global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>? effectiveForegroundColor__34992 = ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>?)(object?)(((DropdownMenuEntry<T>)entry__34055).style?.foregroundColor ?? themeStyle__34902?.foregroundColor));
            global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>? effectiveIconColor__35131 = ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>?)(object?)(((DropdownMenuEntry<T>)entry__34055).style?.iconColor ?? themeStyle__34902?.iconColor));
            global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>? effectiveOverlayColor__35252 = ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>?)(object?)(((DropdownMenuEntry<T>)entry__34055).style?.overlayColor ?? themeStyle__34902?.overlayColor));
            global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>? effectiveBackgroundColor__35382 = ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>?)(object?)(((DropdownMenuEntry<T>)entry__34055).style?.backgroundColor ?? themeStyle__34902?.backgroundColor));
            bool entryIsSelected__35710 = (((DropdownMenuEntry<T>)entry__34055).enabled && (i__33980 == focusedIndex));
            if (entryIsSelected__35710)
            {
                this._highlightedItemStatesController?.dispose();
                _highlightedItemStatesController = new global::Doroti.Generated.Framework.Widgets.WidgetStatesController(new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> { global::Doroti.Generated.Framework.Widgets.WidgetState.focused });
                ButtonStyle defaultStyle__36204 = ((ButtonStyle)(object?)new MenuItemButton().defaultStyleOf(this.context));
                Color? resolveFocusedColor(global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? colorStateProperty)
                {
                    return ((Color?)(object?)colorStateProperty?.resolve(new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> { global::Doroti.Generated.Framework.Widgets.WidgetState.focused }));
                    throw new InvalidOperationException("Dart control flow completed without a value.");
                }
                global::Doroti.Flutter.Ui.Color focusedForegroundColor__36467 = ((global::Doroti.Flutter.Ui.Color)(object?)resolveFocusedColor((effectiveForegroundColor__34992 ?? defaultStyle__36204.foregroundColor!))!);
                global::Doroti.Flutter.Ui.Color focusedIconColor__36614 = ((global::Doroti.Flutter.Ui.Color)(object?)resolveFocusedColor((effectiveIconColor__35131 ?? defaultStyle__36204.iconColor!))!);
                global::Doroti.Flutter.Ui.Color focusedOverlayColor__36743 = ((global::Doroti.Flutter.Ui.Color)(object?)resolveFocusedColor((effectiveOverlayColor__35252 ?? defaultStyle__36204.overlayColor!))!);
                global::Doroti.Flutter.Ui.Color focusedBackgroundColor__37026 = ((global::Doroti.Flutter.Ui.Color)(object?)(resolveFocusedColor(effectiveBackgroundColor__35382) ?? Theme.of(this.context).colorScheme.onSurface.withOpacity(0.12)));
                effectiveStyle__34687 = effectiveStyle__34687.copyWith(backgroundColor: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Color>(focusedBackgroundColor__37026), foregroundColor: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Color>(focusedForegroundColor__36467), iconColor: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Color>(focusedIconColor__36614), overlayColor: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Color>(focusedOverlayColor__36743));
            }
            else
            {
                effectiveStyle__34687 = effectiveStyle__34687.copyWith(backgroundColor: effectiveBackgroundColor__35382, foregroundColor: effectiveForegroundColor__34992, iconColor: effectiveIconColor__35131, overlayColor: effectiveOverlayColor__35252);
            }
            global::Doroti.Generated.Framework.Widgets.Widget label__37855 = (((DropdownMenuEntry<T>)entry__34055).labelWidget ?? new global::Doroti.Generated.Framework.Widgets.Text(((DropdownMenuEntry<T>)entry__34055).label));
            if ((((DropdownMenu<T>)(object)this.widget).width is not null))
            {
                double horizontalPadding__37958 = ((padding__34534 + Dropdown_menuLibrary._kDefaultHorizontalPadding) + effectiveInputStartGap__33864);
                label__37855 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(maxWidth: (DartRuntimePrimitives.RequireValue(((DropdownMenu<T>)(object)this.widget).width) - horizontalPadding__37958)), child: label__37855));
            }
            global::Doroti.Generated.Framework.Widgets.Widget menuItemButton__38232 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ExcludeFocus(child: new global::Doroti.Generated.Framework.Widgets.ExcludeSemantics(excluding: excludeSemantics, child: new MenuItemButton(key: (enableScrollToHighlight ? this.buttonItemKeys[(int)(i__33980)] : null), statesController: (entryIsSelected__35710 ? this._highlightedItemStatesController : null), style: effectiveStyle__34687, leadingIcon: ((DropdownMenuEntry<T>)entry__34055).leadingIcon, trailingIcon: ((DropdownMenuEntry<T>)entry__34055).trailingIcon, closeOnActivate: (object.Equals(((DropdownMenu<T>)(object)this.widget).closeBehavior, DropdownMenuCloseBehavior.all)), onPressed: ((global::System.Action)((((DropdownMenuEntry<T>)entry__34055).enabled && ((DropdownMenu<T>)(object)this.widget).enabled) ? (() => {
if (!this.mounted)
{
    ((DropdownMenu<T>)(object)this.widget).controller?.value = new global::Doroti.Generated.Framework.Services.TextEditingValue(text: ((DropdownMenuEntry<T>)entry__34055).label, selection: global::Doroti.Generated.Framework.Services.TextSelection.CreateCollapsed(offset: ((DropdownMenuEntry<T>)entry__34055).label.Length));
    ((DropdownMenu<T>)(object)this.widget).onSelected?.Invoke(((DropdownMenuEntry<T>)entry__34055).value);
    return;
}
this._effectiveTextEditingController.value = new global::Doroti.Generated.Framework.Services.TextEditingValue(text: ((DropdownMenuEntry<T>)entry__34055).label, selection: global::Doroti.Generated.Framework.Services.TextSelection.CreateCollapsed(offset: ((DropdownMenuEntry<T>)entry__34055).label.Length));
currentHighlight = (((DropdownMenu<T>)(object)this.widget).enableSearch ? i__33980 : null);
((DropdownMenu<T>)(object)this.widget).onSelected?.Invoke(((DropdownMenuEntry<T>)entry__34055).value);
_enableFilter = false;
if ((object.Equals(((DropdownMenu<T>)(object)this.widget).closeBehavior, DropdownMenuCloseBehavior.self)))
{
    this._controller.close();
}
}) : null)), requestFocusOnHover: false, child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: effectiveInputStartGap__33864), child: label__37855)))));
            result__33946.Add(menuItemButton__38232);
        }
        return result__33946;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleUpKey(_ArrowUpIntent__dropdown_menu __unused0)
    {
        setState(((global::System.Action)(() => {
if (((!((DropdownMenu<T>)(object)this.widget).enabled || !this._menuHasEnabledItem) || !((global::Doroti.Generated.Framework.Widgets.MenuController)this._controller).isOpen))
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
string currentLabel__41369 = this.filteredEntries[(int)(DartRuntimePrimitives.RequireValue(this.currentHighlight))].label;
this._effectiveTextEditingController.value = new global::Doroti.Generated.Framework.Services.TextEditingValue(text: currentLabel__41369, selection: global::Doroti.Generated.Framework.Services.TextSelection.CreateCollapsed(offset: currentLabel__41369.Length));
})));
    }

    public virtual void handleDownKey(_ArrowDownIntent__dropdown_menu __unused0)
    {
        setState(((global::System.Action)(() => {
if (((!((DropdownMenu<T>)(object)this.widget).enabled || !this._menuHasEnabledItem) || !((global::Doroti.Generated.Framework.Widgets.MenuController)this._controller).isOpen))
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
string currentLabel__42102 = this.filteredEntries[(int)(DartRuntimePrimitives.RequireValue(this.currentHighlight))].label;
this._effectiveTextEditingController.value = new global::Doroti.Generated.Framework.Services.TextEditingValue(text: currentLabel__42102, selection: global::Doroti.Generated.Framework.Services.TextSelection.CreateCollapsed(offset: currentLabel__42102.Length));
})));
    }

    public virtual void handleEnterKey(_EnterIntent__dropdown_menu __unused0)
    {
        if ((this.selectOnly && !((global::Doroti.Generated.Framework.Widgets.MenuController)this._controller).isOpen))
        {
            this._controller.open();
            return;
        }
        _handleSubmitted();
    }

    public virtual void handlePressed(global::Doroti.Generated.Framework.Widgets.MenuController controller, bool focusForKeyboard = true)
    {
        if (((global::Doroti.Generated.Framework.Widgets.MenuController)controller).isOpen)
        {
            currentHighlight = null;
            controller.close();
        }
        else
        {
            filteredEntries = ((DropdownMenu<T>)(object)this.widget).dropdownMenuEntries;
            if ((((global::Doroti.Generated.Framework.Widgets.TextEditingController)this._effectiveTextEditingController).text.Length != 0))
            {
                _enableFilter = false;
            }
            controller.open();
            if (focusForKeyboard)
            {
                this._internalFocusNode.requestFocus();
            }
        }
        setState(((global::System.Action)(() => {
})));
    }

    internal virtual void _handleSubmitted()
    {
        if ((this.currentHighlight is not null))
        {
            DropdownMenuEntry<T> entry__43097 = this.filteredEntries[(int)(DartRuntimePrimitives.RequireValue(this.currentHighlight))];
            if (((DropdownMenuEntry<T>)entry__43097).enabled)
            {
                this._effectiveTextEditingController.value = new global::Doroti.Generated.Framework.Services.TextEditingValue(text: ((DropdownMenuEntry<T>)entry__43097).label, selection: global::Doroti.Generated.Framework.Services.TextSelection.CreateCollapsed(offset: ((DropdownMenuEntry<T>)entry__43097).label.Length));
                ((DropdownMenu<T>)(object)this.widget).onSelected?.Invoke(((DropdownMenuEntry<T>)entry__43097).value);
            }
        }
        else
        {
            if (((global::Doroti.Generated.Framework.Widgets.MenuController)this._controller).isOpen)
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

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        bool useMaterial3__43665 = Theme.of(context).useMaterial3;
        global::Doroti.Flutter.Ui.TextDirection textDirection__43736 = Directionality.of(context);
        _initialMenu ??= _buildButtons(((DropdownMenu<T>)(object)this.widget).dropdownMenuEntries, textDirection__43736, enableScrollToHighlight: false, excludeSemantics: true, useMaterial3: DartRuntimePrimitives.RequireValue(useMaterial3__43665));
        DropdownMenuThemeData theme__44092 = DropdownMenuTheme.of(context);
        DropdownMenuThemeData defaults__44163 = ((DropdownMenuThemeData)(object?)new _DropdownMenuDefaultsM3__dropdown_menu(context));
        if (this._enableFilter)
        {
            filteredEntries = ((((DropdownMenu<T>)(object)this.widget).filterCallback is null ? filter(((DropdownMenu<T>)(object)this.widget).dropdownMenuEntries, this._effectiveTextEditingController) : ((DropdownMenu<T>)(object)this.widget).filterCallback.Invoke(this.filteredEntries, ((global::Doroti.Generated.Framework.Widgets.TextEditingController)this._effectiveTextEditingController).text)));
        }
        _menuHasEnabledItem = this.filteredEntries.any(((entry) => ((DropdownMenuEntry<T>)entry).enabled));
        if (this._enableSearch)
        {
            if ((((DropdownMenu<T>)(object)this.widget).searchCallback is not null))
            {
                currentHighlight = ((DropdownMenu<T>)(object)this.widget).searchCallback!(this.filteredEntries, ((global::Doroti.Generated.Framework.Widgets.TextEditingController)this._effectiveTextEditingController).text);
            }
            else
            {
                bool shouldUpdateCurrentHighlight__44773 = _shouldUpdateCurrentHighlight(this.filteredEntries);
                if (shouldUpdateCurrentHighlight__44773)
                {
                    currentHighlight = search(this.filteredEntries, this._effectiveTextEditingController);
                }
            }
            if ((this.currentHighlight is not null))
            {
                scrollToHighlight();
            }
        }
        List<global::Doroti.Generated.Framework.Widgets.Widget> menu__45106 = ((List<global::Doroti.Generated.Framework.Widgets.Widget>)(object?)_buildButtons(this.filteredEntries, textDirection__43736, focusedIndex: this.currentHighlight, useMaterial3: DartRuntimePrimitives.RequireValue(useMaterial3__43665)));
        global::Doroti.Generated.Framework.Painting.TextStyle? baseTextStyle__45273 = ((((DropdownMenu<T>)(object)this.widget).textStyle ?? theme__44092.textStyle) ?? defaults__44163.textStyle);
        global::Doroti.Flutter.Ui.Color? disabledColor__45365 = ((global::Doroti.Flutter.Ui.Color?)(object?)(theme__44092.disabledColor ?? defaults__44163.disabledColor));
        global::Doroti.Generated.Framework.Painting.TextStyle? effectiveTextStyle__45449 = (((DropdownMenu<T>)(object)this.widget).enabled ? baseTextStyle__45273 : (baseTextStyle__45273?.copyWith(color: disabledColor__45365) ?? new global::Doroti.Generated.Framework.Painting.TextStyle(color: disabledColor__45365)));
        MenuStyle? effectiveMenuStyle__45617 = ((((DropdownMenu<T>)(object)this.widget).menuStyle ?? theme__44092.menuStyle) ?? defaults__44163.menuStyle!);
        double? anchorWidth__45717 = getWidth(this._anchorKey);
        if ((((DropdownMenu<T>)(object)this.widget).width is not null))
        {
            effectiveMenuStyle__45617 = effectiveMenuStyle__45617.copyWith(minimumSize: WidgetStateProperty.resolveWith<global::Doroti.Flutter.Ui.Size?>((states) => {
double? effectiveMaximumWidth__45953 = effectiveMenuStyle__45617!.maximumSize?.resolve(states)?.width;
return new global::Doroti.Flutter.Ui.Size(Math.Min(DartRuntimePrimitives.RequireValue(((DropdownMenu<T>)(object)this.widget).width), (effectiveMaximumWidth__45953 ?? DartRuntimePrimitives.RequireValue(((DropdownMenu<T>)(object)this.widget).width))), 0.0);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
        }
        else
        {
            if ((anchorWidth__45717 is not null))
            {
                double anchorWidth__45717__value46193 = DartRuntimePrimitives.RequireValue(anchorWidth__45717);
                effectiveMenuStyle__45617 = effectiveMenuStyle__45617.copyWith(minimumSize: WidgetStateProperty.resolveWith<global::Doroti.Flutter.Ui.Size?>((states) => {
double? effectiveMaximumWidth__46384 = effectiveMenuStyle__45617!.maximumSize?.resolve(states)?.width;
return new global::Doroti.Flutter.Ui.Size(Math.Min(DartRuntimePrimitives.RequireValue(anchorWidth__45717__value46193), (effectiveMaximumWidth__46384 ?? DartRuntimePrimitives.RequireValue(anchorWidth__45717__value46193))), 0.0);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            }
        }
        if ((((DropdownMenu<T>)(object)this.widget).menuHeight is not null))
        {
            effectiveMenuStyle__45617 = effectiveMenuStyle__45617.copyWith(maximumSize: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Size>(new global::Doroti.Flutter.Ui.Size(double.PositiveInfinity, DartRuntimePrimitives.RequireValue(((DropdownMenu<T>)(object)this.widget).menuHeight))));
        }
        InputDecorationThemeData effectiveInputDecorationTheme__46851 = ((((DropdownMenu<T>)(object)this.widget).inputDecorationTheme ?? theme__44092.inputDecorationTheme) ?? defaults__44163.inputDecorationTheme!);
        global::Doroti.Generated.Framework.Services.MouseCursor? effectiveMouseCursor__47008 = ((global::Doroti.Generated.Framework.Services.MouseCursor?)(object?)(((object)((DropdownMenu<T>)(object)this.widget).enabled) switch { true => (this.isButton ? global::Doroti.Generated.Framework.Services.SystemMouseCursors.click : global::Doroti.Generated.Framework.Services.SystemMouseCursors.text), false => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Services.SystemMouseCursor>(null), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        global::Doroti.Generated.Framework.Widgets.Widget menuAnchor__47174 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new MenuAnchor(style: effectiveMenuStyle__45617, alignmentOffset: ((DropdownMenu<T>)(object)this.widget).alignmentOffset, reservedPadding: global::Doroti.Generated.Framework.Painting.EdgeInsets.zero, controller: this._controller, menuChildren: menu__45106, crossAxisUnconstrained: false, builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.MenuController, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>?)((context, controller, child) => {
DartRuntimePrimitives.Assert(() => (this._initialMenu is not null));
global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.MenuController, InputDecoration> decorationBuilder__47577 = ((((DropdownMenu<T>)(object)this.widget).decorationBuilder ?? (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.MenuController, InputDecoration>)this._buildDefaultDecoration));
InputDecoration decoration__47686 = decorationBuilder__47577(context, controller);
if ((((InputDecoration)decoration__47686).suffixIcon is null))
{
    decoration__47686 = decoration__47686.copyWith(suffixIcon: _buildDefaultSuffixIcon(context, controller));
}
InputDecoration effectiveDecoration__48040 = ((InputDecoration)(object?)decoration__47686.applyDefaults(effectiveInputDecorationTheme__46851));
InputDecoration textFieldDecoration__48170 = ((((InputDecoration)effectiveDecoration__48040).prefixIcon is null) ? effectiveDecoration__48040 : effectiveDecoration__48040.copyWith(prefixIcon: new global::Doroti.Generated.Framework.Widgets.SizedBox(key: this._leadingKey, child: ((InputDecoration)effectiveDecoration__48040).prefixIcon)));
MaterialLocalizations localizations__48566 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
global::Doroti.Generated.Framework.Widgets.Widget textField__48638 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(button: this.isButton, hint: ((object.Equals(Theme.of(context).platform, global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS)) ? (((global::Doroti.Generated.Framework.Widgets.MenuController)this._controller).isOpen ? ((MaterialLocalizations)localizations__48566).collapsedHint : ((MaterialLocalizations)localizations__48566).expandedHint) : null), expanded: ((global::Doroti.Generated.Framework.Widgets.MenuController)this._controller).isOpen, onExpand: ((global::System.Action)(((global::Doroti.Generated.Framework.Widgets.MenuController)this._controller).isOpen ? null : (() => {
this._controller.open();
}))), onCollapse: ((global::System.Action)(!((global::Doroti.Generated.Framework.Widgets.MenuController)this._controller).isOpen ? null : (() => {
this._controller.close();
}))), child: new global::Doroti.Generated.Framework.Widgets.ExcludeSemantics(excluding: (this.isButton && global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb), child: new TextField(key: this._anchorKey, enabled: ((DropdownMenu<T>)(object)this.widget).enabled, mouseCursor: effectiveMouseCursor__47008, focusNode: ((DropdownMenu<T>)(object)this.widget).focusNode, canRequestFocus: canRequestFocus(), enableInteractiveSelection: !this.isButton, readOnly: this.isButton, keyboardType: ((DropdownMenu<T>)(object)this.widget).keyboardType, textAlign: ((DropdownMenu<T>)(object)this.widget).textAlign, textAlignVertical: global::Doroti.Generated.Framework.Painting.TextAlignVertical.center, maxLines: ((DropdownMenu<T>)(object)this.widget).maxLines, textInputAction: ((DropdownMenu<T>)(object)this.widget).textInputAction, cursorHeight: ((DropdownMenu<T>)(object)this.widget).cursorHeight, style: effectiveTextStyle__45449, controller: this._effectiveTextEditingController, onSubmitted: ((_) => { _handleSubmitted(); }), onTap: ((global::System.Action)(!((DropdownMenu<T>)(object)this.widget).enabled ? null : (() => {
handlePressed(controller, focusForKeyboard: !canRequestFocus());
}))), onChanged: ((text) => {
controller.open();
setState(((global::System.Action)(() => {
filteredEntries = ((DropdownMenu<T>)(object)this.widget).dropdownMenuEntries;
_enableFilter = ((DropdownMenu<T>)(object)this.widget).enableFilter;
_enableSearch = ((DropdownMenu<T>)(object)this.widget).enableSearch;
})));
}), inputFormatters: ((DropdownMenu<T>)(object)this.widget).inputFormatters, decoration: textFieldDecoration__48170, restorationId: ((DropdownMenu<T>)(object)this.widget).restorationId, scrollPadding: ((DropdownMenu<T>)(object)this.widget).scrollPadding))));
global::Doroti.Generated.Framework.Widgets.Widget? effectiveLabel__51312 = (((InputDecoration)effectiveDecoration__48040).label ?? (((((InputDecoration)effectiveDecoration__48040).labelText is not null) ? new global::Doroti.Generated.Framework.Widgets.Text(((InputDecoration)effectiveDecoration__48040).labelText!) : null)));
global::Doroti.Generated.Framework.Widgets.Widget body__51699 = ((((DropdownMenu<T>)(object)this.widget).expandedInsets is not null) ? textField__48638 : new _DropdownMenuBody__dropdown_menu(width: ((DropdownMenu<T>)(object)this.widget).width, children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection52521 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection52521.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(textField__48638)); __collection52521.AddRange(this._initialMenu!); if ((effectiveLabel__51312 is not null)) { __collection52521.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.ExcludeSemantics(child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 4.0), child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: effectiveTextStyle__45449!, child: effectiveLabel__51312))))); } __collection52521.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>((((InputDecoration)effectiveDecoration__48040).suffixIcon ?? global::Doroti.Generated.Framework.Widgets.SizedBox.CreateShrink()))); __collection52521.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(8.0), child: (((InputDecoration)effectiveDecoration__48040).prefixIcon ?? global::Doroti.Generated.Framework.Widgets.SizedBox.CreateShrink())))); return __collection52521; }))()));
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Shortcuts(shortcuts: (this.selectOnly ? _selectOnlyShortcuts : _editableShortcuts), child: body__51699));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        if (((DropdownMenu<T>)(object)this.widget).expandedInsets is global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding__53820)
        {
            menuAnchor__47174 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: padding__53820.clamp(global::Doroti.Generated.Framework.Painting.EdgeInsets.zero, global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(left: double.PositiveInfinity, right: double.PositiveInfinity).add(global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(end: double.PositiveInfinity, start: double.PositiveInfinity))), child: menuAnchor__47174));
        }
        menuAnchor__47174 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.topStart, widthFactor: 1.0, heightFactor: 1.0, child: menuAnchor__47174));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Actions(actions: new DartMap<Type, dynamic> { [typeof(_ArrowUpIntent__dropdown_menu)] = new global::Doroti.Generated.Framework.Widgets.CallbackAction<_ArrowUpIntent__dropdown_menu>(onInvoke: (__arg0) => { ((global::System.Action<_ArrowUpIntent__dropdown_menu>)this.handleUpKey)(__arg0); return default!; }), [typeof(_ArrowDownIntent__dropdown_menu)] = new global::Doroti.Generated.Framework.Widgets.CallbackAction<_ArrowDownIntent__dropdown_menu>(onInvoke: (__arg0) => { ((global::System.Action<_ArrowDownIntent__dropdown_menu>)this.handleDownKey)(__arg0); return default!; }), [typeof(_EnterIntent__dropdown_menu)] = new global::Doroti.Generated.Framework.Widgets.CallbackAction<_EnterIntent__dropdown_menu>(onInvoke: (__arg0) => { ((global::System.Action<_EnterIntent__dropdown_menu>)this.handleEnterKey)(__arg0); return default!; }), [typeof(global::Doroti.Generated.Framework.Widgets.DismissIntent)] = new global::Doroti.Generated.Framework.Widgets.DismissMenuAction(controller: this._controller) }, child: new global::Doroti.Generated.Framework.Widgets.Stack(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Shortcuts(shortcuts: new DartMap<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent> { [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowUp)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new _ArrowUpIntent__dropdown_menu()), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowDown)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new _ArrowDownIntent__dropdown_menu()), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.enter)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new _EnterIntent__dropdown_menu()), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.escape)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new global::Doroti.Generated.Framework.Widgets.DismissIntent()) }, child: new global::Doroti.Generated.Framework.Widgets.Focus(focusNode: this._internalFocusNode, skipTraversal: true, child: global::Doroti.Generated.Framework.Widgets.SizedBox.CreateShrink()))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(menuAnchor__47174) })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual InputDecoration _buildDefaultDecoration(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.MenuController controller)
    {
        return new InputDecoration(label: ((DropdownMenu<T>)(object)this.widget).label, hintText: ((DropdownMenu<T>)(object)this.widget).hintText, helperText: ((DropdownMenu<T>)(object)this.widget).helperText, errorText: ((DropdownMenu<T>)(object)this.widget).errorText, prefixIcon: ((DropdownMenu<T>)(object)this.widget).leadingIcon, suffixIcon: _buildDefaultSuffixIcon(context, controller));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget? _buildDefaultSuffixIcon(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.MenuController controller)
    {
        bool isCollapsed__56223 = (((DropdownMenu<T>)(object)this.widget).inputDecorationTheme?.isCollapsed ?? false);
        return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)(((DropdownMenu<T>)(object)this.widget).showTrailingIcon ? new global::Doroti.Generated.Framework.Widgets.Padding(padding: (isCollapsed__56223 ? global::Doroti.Generated.Framework.Painting.EdgeInsets.zero : global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(4.0)), child: new global::Doroti.Generated.Framework.Widgets.ExcludeSemantics(excluding: this.isButton, child: new IconButton(focusNode: this._trailingIconButtonFocusNode, isSelected: ((global::Doroti.Generated.Framework.Widgets.MenuController)controller).isOpen, constraints: ((DropdownMenu<T>)(object)this.widget).inputDecorationTheme?.suffixIconConstraints, padding: (isCollapsed__56223 ? global::Doroti.Generated.Framework.Painting.EdgeInsets.zero : null), icon: (((DropdownMenu<T>)(object)this.widget).trailingIcon ?? new global::Doroti.Generated.Framework.Widgets.Icon(Icons.arrow_drop_down)), selectedIcon: (((DropdownMenu<T>)(object)this.widget).selectedTrailingIcon ?? new global::Doroti.Generated.Framework.Widgets.Icon(Icons.arrow_drop_up)), onPressed: ((global::System.Action)(!((DropdownMenu<T>)(object)this.widget).enabled ? null : (() => {
handlePressed(controller);
})))))) : null));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _ArrowUpIntent__dropdown_menu : global::Doroti.Generated.Framework.Widgets.Intent
{
    internal _ArrowUpIntent__dropdown_menu()
    {
    }

}

public class _ArrowDownIntent__dropdown_menu : global::Doroti.Generated.Framework.Widgets.Intent
{
    internal _ArrowDownIntent__dropdown_menu()
    {
    }

}

public class _EnterIntent__dropdown_menu : global::Doroti.Generated.Framework.Widgets.Intent
{
    internal _EnterIntent__dropdown_menu()
    {
    }

}

internal class _DropdownMenuBody__dropdown_menu : global::Doroti.Generated.Framework.Widgets.MultiChildRenderObjectWidget
{
    public virtual double? width { get; private set; }

    internal _DropdownMenuBody__dropdown_menu(List<global::Doroti.Generated.Framework.Widgets.Widget> children = default!, double? width = null) : base(children: children ?? new List<global::Doroti.Generated.Framework.Widgets.Widget>())
    {
        this.width = width;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderDropdownMenuBody__dropdown_menu(width: this.width));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderDropdownMenuBody__dropdown_menu)(object)renderObject;
        __renderObject.width = this.width;
    }

}

internal class _DropdownMenuBodyParentData__dropdown_menu : global::Doroti.Generated.Framework.Rendering.ContainerBoxParentData<global::Doroti.Generated.Framework.Rendering.RenderBox>
{
}

public class _RenderDropdownMenuBody__dropdown_menu : global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.ContainerRenderObjectMixin<global::Doroti.Generated.Framework.Rendering.RenderBox, _DropdownMenuBodyParentData__dropdown_menu>, global::Doroti.Generated.Framework.Rendering.RenderBoxContainerDefaultsMixin<global::Doroti.Generated.Framework.Rendering.RenderBox, _DropdownMenuBodyParentData__dropdown_menu>
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
    public override void setupParentData(global::Doroti.Generated.Framework.Rendering.RenderObject child)
    {
        var __child = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)child;
        if ((__child.parentData is not _DropdownMenuBodyParentData__dropdown_menu))
        {
            __child.parentData = new _DropdownMenuBodyParentData__dropdown_menu();
        }
    }

    public override void performLayout()
    {
        global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints__59353 = this.constraints;
        var maxWidth__59393 = 0.0;
        double? maxHeight__59421 = default!;
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__59447 = this.firstChild;
        double intrinsicWidth__59485 = ((this.width ?? (double)getMaxIntrinsicWidth(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints__59353).maxHeight)));
        double widthConstraint__59573 = Math.Min(intrinsicWidth__59485, ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints__59353).maxWidth);
        var innerConstraints__59649 = new global::Doroti.Generated.Framework.Rendering.BoxConstraints(maxWidth: widthConstraint__59573, maxHeight: getMaxIntrinsicHeight(widthConstraint__59573));
        while ((child__59447 is not null))
        {
            if ((object.Equals(child__59447, this.firstChild)))
            {
                child__59447.layout(innerConstraints__59649, parentUsesSize: true);
                maxHeight__59421 ??= ((global::Doroti.Generated.Framework.Rendering.RenderBox)child__59447).size.height;
                var childParentData__59959 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child__59447.parentData!)!;
                DartRuntimePrimitives.Assert(() => (object.Equals(child__59447.parentData, childParentData__59959)));
                child__59447 = childParentData__59959.nextSibling;
                continue;
            }
            child__59447.layout(innerConstraints__59649, parentUsesSize: true);
            var childParentData__60223 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child__59447.parentData!)!;
            childParentData__60223.offset = Offset.zero;
            maxWidth__59393 = Math.Max(maxWidth__59393, ((global::Doroti.Generated.Framework.Rendering.RenderBox)child__59447).size.width);
            maxHeight__59421 ??= ((global::Doroti.Generated.Framework.Rendering.RenderBox)child__59447).size.height;
            DartRuntimePrimitives.Assert(() => (object.Equals(child__59447.parentData, childParentData__60223)));
            child__59447 = childParentData__60223.nextSibling;
        }
        DartRuntimePrimitives.Assert(() => (maxHeight__59421 is not null));
        maxWidth__59393 = Math.Max(Dropdown_menuLibrary._kMinimumWidth, maxWidth__59393);
        size = constraints__59353.constrain(new global::Doroti.Flutter.Ui.Size((this.width ?? maxWidth__59393), DartRuntimePrimitives.RequireValue(maxHeight__59421)));
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__60776 = this.firstChild;
        if ((child__60776 is not null))
        {
            var childParentData__60833 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child__60776.parentData!)!;
            context.paintChild(child__60776, (offset + childParentData__60833.offset));
        }
    }

    public override Size computeDryLayout(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        var maxWidth__61052 = 0.0;
        double? maxHeight__61080 = default!;
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__61106 = this.firstChild;
        double intrinsicWidth__61143 = ((this.width ?? (double)getMaxIntrinsicWidth(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxHeight)));
        double widthConstraint__61231 = Math.Min(intrinsicWidth__61143, ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxWidth);
        var innerConstraints__61307 = new global::Doroti.Generated.Framework.Rendering.BoxConstraints(maxWidth: widthConstraint__61231, maxHeight: getMaxIntrinsicHeight(widthConstraint__61231));
        while ((child__61106 is not null))
        {
            global::Doroti.Flutter.Ui.Size childSize__61485 = ((global::Doroti.Flutter.Ui.Size)(object?)child__61106.getDryLayout(innerConstraints__61307));
            if ((!object.Equals(child__61106, this.firstChild)))
            {
                maxWidth__61052 = Math.Max(maxWidth__61052, childSize__61485.width);
            }
            var childParentData__61756 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child__61106.parentData!)!;
            maxHeight__61080 ??= childSize__61485.height;
            child__61106 = childParentData__61756.nextSibling;
        }
        DartRuntimePrimitives.Assert(() => (maxHeight__61080 is not null));
        maxWidth__61052 = Math.Max(Dropdown_menuLibrary._kMinimumWidth, maxWidth__61052);
        return constraints.constrain(new global::Doroti.Flutter.Ui.Size((this.width ?? maxWidth__61052), DartRuntimePrimitives.RequireValue(maxHeight__61080)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__62148 = this.firstChild;
        double width__62179 = 0;
        while ((child__62148 is not null))
        {
            if ((object.Equals(child__62148, this.firstChild)))
            {
                var childParentData__62265 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child__62148.parentData!)!;
                child__62148 = childParentData__62265.nextSibling;
                continue;
            }
            double minIntrinsicWidth__62423 = child__62148.getMinIntrinsicWidth(height);
            if ((object.Equals(child__62148, this.lastChild)))
            {
                width__62179 += minIntrinsicWidth__62423;
            }
            if ((object.Equals(child__62148, childBefore(this.lastChild!))))
            {
                width__62179 += minIntrinsicWidth__62423;
            }
            width__62179 = Math.Max(DartRuntimePrimitives.RequireValue(width__62179), minIntrinsicWidth__62423);
            var childParentData__62788 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child__62148.parentData!)!;
            child__62148 = childParentData__62788.nextSibling;
        }
        return Math.Max(DartRuntimePrimitives.RequireValue(width__62179), Dropdown_menuLibrary._kMinimumWidth);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__63033 = this.firstChild;
        double width__63064 = 0;
        while ((child__63033 is not null))
        {
            if ((object.Equals(child__63033, this.firstChild)))
            {
                var childParentData__63150 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child__63033.parentData!)!;
                child__63033 = childParentData__63150.nextSibling;
                continue;
            }
            double maxIntrinsicWidth__63308 = child__63033.getMaxIntrinsicWidth(height);
            if ((object.Equals(child__63033, this.lastChild)))
            {
                width__63064 += maxIntrinsicWidth__63308;
            }
            if ((object.Equals(child__63033, childBefore(this.lastChild!))))
            {
                width__63064 += maxIntrinsicWidth__63308;
            }
            width__63064 = Math.Max(DartRuntimePrimitives.RequireValue(width__63064), maxIntrinsicWidth__63308);
            var childParentData__63673 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child__63033.parentData!)!;
            child__63033 = childParentData__63673.nextSibling;
        }
        return Math.Max(DartRuntimePrimitives.RequireValue(width__63064), Dropdown_menuLibrary._kMinimumWidth);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__63924 = this.firstChild;
        double width__63955 = 0;
        if ((child__63924 is not null))
        {
            width__63955 = Math.Max(DartRuntimePrimitives.RequireValue(width__63955), child__63924.getMinIntrinsicHeight(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(width__63955))));
        }
        return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(width__63955));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__64171 = this.firstChild;
        double width__64202 = 0;
        if ((child__64171 is not null))
        {
            width__64202 = Math.Max(DartRuntimePrimitives.RequireValue(width__64202), child__64171.getMaxIntrinsicHeight(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(width__64202))));
        }
        return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(width__64202));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(global::Doroti.Generated.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__64445 = this.firstChild;
        if ((child__64445 is not null))
        {
            var childParentData__64502 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child__64445.parentData!)!;
            bool isHit__64587 = result.addWithPaintOffset(offset: childParentData__64502.offset, position: position, hitTest: ((global::System.Func<global::Doroti.Generated.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, transformed) => {
DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData__64502.offset))));
return child__64445.hitTest(result, position: transformed);
throw new InvalidOperationException("Dart closure completed without a value.");
})));
            if (isHit__64587)
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void visitChildrenForSemantics(global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject> visitor)
    {
        visitChildren(((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)((renderObjectChild) => {
var child__65228 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)renderObjectChild)!;
if ((object.Equals(child__65228, this.firstChild)))
{
    visitor(((global::Doroti.Generated.Framework.Rendering.RenderBox)renderObjectChild));
}
})));
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173585 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
        while ((childParentData__173585.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173585.previousSibling, child)));
            child = childParentData__173585.previousSibling!;
            childParentData__173585 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173981 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
        while ((childParentData__173981.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173981.nextSibling, child)));
            child = childParentData__173981.nextSibling!;
            childParentData__173981 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
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
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new global::Doroti.Generated.Framework.Foundation.ErrorSpacer(), new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderBox)} child was created by", this.debugCreator, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Generated.Framework.Foundation.ErrorSpacer(), new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((RenderObject)child).debugCreator, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData__175971 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData__175971.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData__175971.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData__175971.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData__176343 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)this._firstChild!.parentData!)!;
                firstChildParentData__176343.previousSibling = child;
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
            var afterParentData__176766 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)after.parentData!)!;
            if ((afterParentData__176766.nextSibling is null))
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(after, this._lastChild)));
                childParentData__175971.previousSibling = after;
                afterParentData__176766.nextSibling = child;
                this._lastChild = child;
            }
            else
            {
                childParentData__175971.nextSibling = afterParentData__176766.nextSibling;
                childParentData__175971.previousSibling = after;
                var childPreviousSiblingParentData__177424 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)childParentData__175971.previousSibling!.parentData!)!;
                var childNextSiblingParentData__177547 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)childParentData__175971.nextSibling!.parentData!)!;
                childPreviousSiblingParentData__177424.nextSibling = child;
                childNextSiblingParentData__177547.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData__176766.nextSibling, child)));
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
        var childParentData__179226 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: this._firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: this._lastChild));
        DartRuntimePrimitives.Assert(() => (this._childCount >= 0L));
        if ((childParentData__179226.previousSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._firstChild, child)));
            this._firstChild = childParentData__179226.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData__179613 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)childParentData__179226.previousSibling!.parentData!)!;
            childPreviousSiblingParentData__179613.nextSibling = childParentData__179226.nextSibling;
        }
        if ((childParentData__179226.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData__179226.previousSibling;
        }
        else
        {
            var childNextSiblingParentData__179965 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)childParentData__179226.nextSibling!.parentData!)!;
            childNextSiblingParentData__179965.previousSibling = childParentData__179226.previousSibling;
        }
        childParentData__179226.previousSibling = null;
        childParentData__179226.nextSibling = null;
        this._childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child__180623 = this._firstChild;
        while ((child__180623 is not null))
        {
            var childParentData__180684 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child__180623.parentData!)!;
            RenderBox? next__180762 = childParentData__180684.nextSibling;
            childParentData__180684.previousSibling = null;
            childParentData__180684.nextSibling = null;
            dropChild(child__180623);
            child__180623 = next__180762;
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
        var childParentData__181479 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
        if ((object.Equals(childParentData__181479.previousSibling, after)))
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
        RenderBox? child__181803 = this._firstChild;
        while ((child__181803 is not null))
        {
            ((dynamic)child__181803).attach(owner);
            var childParentData__181891 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child__181803.parentData!)!;
            child__181803 = childParentData__181891.nextSibling;
        }
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child__182065 = this._firstChild;
        while ((child__182065 is not null))
        {
            ((dynamic)child__182065).detach();
            var childParentData__182148 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child__182065.parentData!)!;
            child__182065 = childParentData__182148.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child__182311 = this._firstChild;
        while ((child__182311 is not null))
        {
            redepthChild(child__182311);
            var childParentData__182399 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child__182311.parentData!)!;
            child__182311 = childParentData__182399.nextSibling;
        }
    }

    public override void visitChildren(global::System.Action<RenderObject> visitor)
    {
        RenderBox? child__182587 = this._firstChild;
        while ((child__182587 is not null))
        {
            visitor(child__182587);
            var childParentData__182670 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child__182587.parentData!)!;
            child__182587 = childParentData__182670.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183103 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
        return childParentData__183103.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183356 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child.parentData!)!;
        return childParentData__183356.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var children__183528 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>();
        if ((this.firstChild is not null))
        {
            RenderBox child__183606 = this.firstChild!;
            var count__183637 = 1L;
            while (true)
            {
                children__183528.Add(((Diagnosticable)child__183606).toDiagnosticsNode(name: $"child__183606 {count__183637}"));
                if ((object.Equals(child__183606, this.lastChild)))
                {
                    break;
                }
                count__183637 += 1L;
                var childParentData__183833 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child__183606.parentData!)!;
                child__183606 = childParentData__183833.nextSibling!;
            }
        }
        return children__183528;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToFirstActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        RenderBox? child__138717 = this.firstChild;
        while ((child__138717 is not null))
        {
            var childParentData__138777 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child__138717.parentData!)!;
            double? result__138852 = child__138717.getDistanceToActualBaseline(baseline);
            if ((result__138852 is not null))
            {
                double result__138852__value138916 = DartRuntimePrimitives.RequireValue(result__138852);
                return (DartRuntimePrimitives.RequireValue(result__138852__value138916) + childParentData__138777.offset.dy);
            }
            child__138717 = childParentData__138777.nextSibling;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToHighestActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !this.debugNeedsLayout);
        BaselineOffset minBaseline__139372 = BaselineOffset.noBaseline;
        RenderBox? child__139428 = this.firstChild;
        while ((child__139428 is not null))
        {
            var childParentData__139488 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child__139428.parentData!)!;
            BaselineOffset candidate__139570 = (new BaselineOffset(child__139428.getDistanceToActualBaseline(baseline)).op_Add(childParentData__139488.offset.dy));
            minBaseline__139372 = minBaseline__139372.minOf(candidate__139570);
            child__139428 = childParentData__139488.nextSibling;
        }
        return minBaseline__139372.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool defaultHitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child__140279 = this.lastChild;
        while ((child__140279 is not null))
        {
            var childParentData__140418 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child__140279.parentData!)!;
            bool isHit__140490 = result.addWithPaintOffset(offset: childParentData__140418.offset, position: position, hitTest: ((global::System.Func<BoxHitTestResult, Offset, bool>)((result, transformed) => {
DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData__140418.offset))));
return child__140279!.hitTest(result, position: transformed);
throw new InvalidOperationException("Dart closure completed without a value.");
})));
            if (isHit__140490)
            {
                return true;
            }
            child__140279 = childParentData__140418.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void defaultPaint(PaintingContext context, Offset offset)
    {
        RenderBox? child__141240 = this.firstChild;
        while ((child__141240 is not null))
        {
            var childParentData__141300 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child__141240.parentData!)!;
            context.paintChild(child__141240, (childParentData__141300.offset + offset));
            child__141240 = childParentData__141300.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result__141793 = new List<RenderBox>();
        RenderBox? child__141832 = this.firstChild;
        while ((child__141832 is not null))
        {
            var childParentData__141892 = ((_DropdownMenuBodyParentData__dropdown_menu?)(object?)child__141832.parentData!)!;
            result__141793.Add(((RenderBox?)(object?)child__141832)!);
            child__141832 = childParentData__141892.nextSibling;
        }
        return result__141793;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DropdownMenuDefaultsM3__dropdown_menu : DropdownMenuThemeData
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
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

    internal _DropdownMenuDefaultsM3__dropdown_menu(global::Doroti.Generated.Framework.Widgets.BuildContext context) : base(disabledColor: Theme.of(context).colorScheme.onSurface.withOpacity(0.38))
    {
        this.context = context;
    }

    public override global::Doroti.Generated.Framework.Painting.TextStyle? textStyle => this._theme.textTheme.bodyLarge;
    public virtual MenuStyle menuStyle
    {
        get
        {
            return new MenuStyle(minimumSize: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Size>(new global::Doroti.Flutter.Ui.Size(Dropdown_menuLibrary._kMinimumWidth, 0.0)), maximumSize: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Size>(Size.infinite), visualDensity: VisualDensity.standard);
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
