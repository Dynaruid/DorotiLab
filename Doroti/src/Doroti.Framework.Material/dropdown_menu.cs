// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/dropdown_menu.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public delegate List<DropdownMenuEntry<T>> FilterCallback<T>(List<DropdownMenuEntry<T>> entries, string filter);

public delegate long? SearchCallback<T>(List<DropdownMenuEntry<T>> entries, string query);

public delegate InputDecoration DropdownMenuDecorationBuilder(BuildContext context, MenuController controller);

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
    public virtual Widget? labelWidget { get; private set; }
    public virtual Widget? leadingIcon { get; private set; }
    public virtual Widget? trailingIcon { get; private set; }
    public virtual bool enabled { get; private set; } = default!;
    public virtual ButtonStyle? style { get; private set; }

    public DropdownMenuEntry(T value, string label, Widget? labelWidget = null, Widget? leadingIcon = null, Widget? trailingIcon = null, bool enabled = true, ButtonStyle? style = null)
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

public class DropdownMenu<T> : StatefulWidget
{
    public virtual bool enabled { get; private set; } = default!;
    public virtual double? width { get; private set; }
    public virtual double? menuHeight { get; private set; }
    public virtual Widget? leadingIcon { get; private set; }
    public virtual Widget? trailingIcon { get; private set; }
    public virtual bool showTrailingIcon { get; private set; } = default!;
    public virtual FocusNode? trailingIconFocusNode { get; private set; }
    public virtual Widget? label { get; private set; }
    public virtual string? hintText { get; private set; }
    public virtual string? helperText { get; private set; }
    public virtual string? errorText { get; private set; }
    public virtual Widget? selectedTrailingIcon { get; private set; }
    public virtual bool enableFilter { get; private set; } = default!;
    public virtual bool enableSearch { get; private set; } = default!;
    public virtual TextInputType? keyboardType { get; private set; }
    public virtual TextStyle? textStyle { get; private set; }
    public virtual TextAlign textAlign { get; private set; } = default!;
    internal virtual object? _inputDecorationTheme { get; private set; }
    public virtual Func<BuildContext, MenuController, InputDecoration>? decorationBuilder { get; private set; }
    public virtual MenuStyle? menuStyle { get; private set; }
    public virtual TextEditingController? controller { get; private set; }
    public virtual T? initialSelection { get; private set; }
    public virtual Action<T?>? onSelected { get; private set; }
    public virtual FocusNode? focusNode { get; private set; }
    public virtual bool? requestFocusOnTap { get; private set; }
    public virtual bool selectOnly { get; private set; } = default!;
    public virtual List<DropdownMenuEntry<T>> dropdownMenuEntries { get; private set; } = default!;
    public virtual EdgeInsetsGeometry? expandedInsets { get; private set; }
    public virtual Func<List<DropdownMenuEntry<T>>, string, List<DropdownMenuEntry<T>>>? filterCallback { get; private set; }
    public virtual Func<List<DropdownMenuEntry<T>>, string, long?>? searchCallback { get; private set; }
    public virtual List<TextInputFormatter>? inputFormatters { get; private set; }
    public virtual Offset? alignmentOffset { get; private set; }
    public virtual DropdownMenuCloseBehavior closeBehavior { get; private set; } = default!;
    public virtual long? maxLines { get; private set; }
    public virtual TextInputAction? textInputAction { get; private set; }
    public virtual double? cursorHeight { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual MenuController? menuController { get; private set; }
    public virtual EdgeInsets scrollPadding { get; private set; } = default!;

    public DropdownMenu(Key? key = null, bool enabled = true, double? width = null, double? menuHeight = null, Widget? leadingIcon = null, Widget? trailingIcon = null, bool showTrailingIcon = true, FocusNode? trailingIconFocusNode = null, Widget? label = null, string? hintText = null, string? helperText = null, string? errorText = null, Widget? selectedTrailingIcon = null, bool enableFilter = false, bool enableSearch = true, TextInputType? keyboardType = null, TextStyle? textStyle = null, TextAlign textAlign = TextAlign.start, object? inputDecorationTheme = null, Func<BuildContext, MenuController, InputDecoration>? decorationBuilder = null, MenuStyle? menuStyle = null, TextEditingController? controller = null, T? initialSelection = default, Action<T?>? onSelected = null, FocusNode? focusNode = null, bool? requestFocusOnTap = null, bool selectOnly = false, EdgeInsetsGeometry? expandedInsets = null, Func<List<DropdownMenuEntry<T>>, string, List<DropdownMenuEntry<T>>>? filterCallback = null, Func<List<DropdownMenuEntry<T>>, string, long?>? searchCallback = null, Offset? alignmentOffset = null, List<DropdownMenuEntry<T>> dropdownMenuEntries = default!, List<TextInputFormatter>? inputFormatters = null, DropdownMenuCloseBehavior closeBehavior = DropdownMenuCloseBehavior.all, long? maxLines = 1, TextInputAction? textInputAction = null, double? cursorHeight = null, string? restorationId = null, MenuController? menuController = null, EdgeInsets scrollPadding = default!) : base(key: key)
    {
        EdgeInsets __scrollPadding = scrollPadding ?? EdgeInsets.CreateAll(20.0);
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
        _inputDecorationTheme = inputDecorationTheme;
        System.Diagnostics.Debug.Assert((filterCallback is null) || enableFilter);
        System.Diagnostics.Debug.Assert((inputDecorationTheme is null) || (inputDecorationTheme is InputDecorationTheme) || (inputDecorationTheme is InputDecorationThemeData));
        System.Diagnostics.Debug.Assert((trailingIconFocusNode is null) || showTrailingIcon);
        System.Diagnostics.Debug.Assert((decorationBuilder is null) || (label is null) && (hintText is null) && (helperText is null) && (errorText is null));
    }

    public virtual InputDecorationThemeData? inputDecorationTheme
    {
        get
        {
            if (_inputDecorationTheme is null)
            {
                return default;
            }
            return _inputDecorationTheme is InputDecorationTheme theme ? theme.data : (InputDecorationThemeData)_inputDecorationTheme;
        }
    }
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DropdownMenuState__dropdown_menu<T>());
}

internal class _DropdownMenuState__dropdown_menu<T> : State<DropdownMenu<T>>
{
    internal static DartMap<ShortcutActivator, Intent> _editableShortcuts = new DartMap<ShortcutActivator, Intent> { [new SingleActivator(LogicalKeyboardKey.arrowLeft)] = new ExtendSelectionByCharacterIntent(forward: false, collapseSelection: true), [new SingleActivator(LogicalKeyboardKey.arrowRight)] = new ExtendSelectionByCharacterIntent(forward: true, collapseSelection: true), [new SingleActivator(LogicalKeyboardKey.arrowUp)] = new _ArrowUpIntent__dropdown_menu(), [new SingleActivator(LogicalKeyboardKey.arrowDown)] = new _ArrowDownIntent__dropdown_menu() };
    internal static DartMap<ShortcutActivator, Intent> _selectOnlyShortcuts = new DartMap<ShortcutActivator, Intent> { [new SingleActivator(LogicalKeyboardKey.arrowUp)] = new _ArrowUpIntent__dropdown_menu(), [new SingleActivator(LogicalKeyboardKey.arrowDown)] = new _ArrowDownIntent__dropdown_menu(), [new SingleActivator(LogicalKeyboardKey.enter)] = new _EnterIntent__dropdown_menu() };
    internal virtual GlobalKey<IState> _anchorKey { get; private set; } = GlobalKey<IState>.Create();
    internal virtual GlobalKey<IState> _leadingKey { get; private set; } = GlobalKey<IState>.Create();
    public virtual List<GlobalKey<IState>> buttonItemKeys { get; set; } = default!;
    internal virtual MenuController _controller { get; set; } = default!;
    internal virtual bool _enableFilter { get; set; } = false;
    internal virtual bool _enableSearch { get; set; } = default!;
    public virtual List<DropdownMenuEntry<T>> filteredEntries { get; set; } = default!;
    internal virtual List<Widget>? _initialMenu { get; set; } = default;
    public virtual long? currentHighlight { get; set; } = default;
    public virtual double? leadingPadding { get; set; } = default;
    internal virtual bool _menuHasEnabledItem { get; set; } = false;
    internal virtual TextEditingController? _localTextEditingController { get; set; } = default;
    internal virtual FocusNode _internalFocusNode { get; private set; } = new FocusNode();
    internal virtual WidgetStatesController? _highlightedItemStatesController { get; set; } = default;
    internal virtual FocusNode? _localTrailingIconButtonFocusNode { get; set; } = default;

    internal virtual TextEditingController _effectiveTextEditingController => DartRuntimePrimitives.ConvertValue<TextEditingController>(widget.controller ?? (_localTextEditingController ??= new TextEditingController()));
    internal virtual FocusNode _trailingIconButtonFocusNode => DartRuntimePrimitives.ConvertValue<FocusNode>(widget.trailingIconFocusNode ?? (_localTrailingIconButtonFocusNode ??= new FocusNode()));
    public override void initState()
    {
        base.initState();
        _enableSearch = widget.enableSearch;
        filteredEntries = widget.dropdownMenuEntries;
        buttonItemKeys = DartRuntimePrimitives.CreateList(checked(filteredEntries.Count), (index) => GlobalKey<IState>.Create());
        _menuHasEnabledItem = filteredEntries.any((entry) => entry.enabled);
        long indexLocal = filteredEntries.indexWhere((entry) => EqualityComparer<T>.Default.Equals(entry.value, widget.initialSelection));
        if (indexLocal != -1L)
        {
            _effectiveTextEditingController.value = new TextEditingValue(text: filteredEntries[(int)indexLocal].label, selection: TextSelection.CreateCollapsed(offset: filteredEntries[(int)indexLocal].label.Length));
        }
        refreshLeadingPadding();
        _controller = widget.menuController ?? new MenuController();
    }

    public override void dispose()
    {
        _localTextEditingController?.dispose();
        _localTextEditingController = null;
        _internalFocusNode.dispose();
        _localTrailingIconButtonFocusNode?.dispose();
        _localTrailingIconButtonFocusNode = null;
        _highlightedItemStatesController?.dispose();
        base.dispose();
    }

    public override void didUpdateWidget(DropdownMenu<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(oldWidget.controller, widget.controller))
        {
            _localTextEditingController?.dispose();
            _localTextEditingController = null;
        }
        if (oldWidget.enableFilter != widget.enableFilter)
        {
            if (!widget.enableFilter)
            {
                _enableFilter = false;
            }
        }
        if (oldWidget.enableSearch != widget.enableSearch)
        {
            if (!widget.enableSearch)
            {
                _enableSearch = widget.enableSearch;
                currentHighlight = null;
            }
        }
        if (!Equals(oldWidget.dropdownMenuEntries, widget.dropdownMenuEntries))
        {
            currentHighlight = null;
            filteredEntries = widget.dropdownMenuEntries;
            buttonItemKeys = DartRuntimePrimitives.CreateList(checked(filteredEntries.Count), (index) => GlobalKey<IState>.Create());
            _menuHasEnabledItem = filteredEntries.any((entry) => entry.enabled);
        }
        if (!Equals(oldWidget.leadingIcon, widget.leadingIcon))
        {
            refreshLeadingPadding();
        }
        if (!EqualityComparer<T>.Default.Equals(oldWidget.initialSelection, widget.initialSelection))
        {
            long indexLocal = filteredEntries.indexWhere((entry) => EqualityComparer<T>.Default.Equals(entry.value, widget.initialSelection));
            if (indexLocal != -1L)
            {
                _effectiveTextEditingController.value = new TextEditingValue(text: filteredEntries[(int)indexLocal].label, selection: TextSelection.CreateCollapsed(offset: filteredEntries[(int)indexLocal].label.Length));
            }
        }
        if (!Equals(oldWidget.menuController, widget.menuController))
        {
            _controller = widget.menuController ?? new MenuController();
        }
    }

    public virtual bool canRequestFocus()
    {
        return (widget.focusNode?.canRequestFocus ?? widget.requestFocusOnTap) ?? (Theme.of(context).platform switch { TargetPlatform.iOS or TargetPlatform.android => false, TargetPlatform.fuchsia => false, TargetPlatform.macOS or TargetPlatform.linux => true, TargetPlatform.windows => true, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool selectOnly => widget.selectOnly;
    public virtual bool isButton => DartRuntimePrimitives.ConvertValue<bool>(!canRequestFocus() || selectOnly);
    public virtual void refreshLeadingPadding()
    {
        WidgetsBinding.instance.addPostFrameCallback((_) =>
        {
            if (!mounted)
            {
                return;
            }
            setState(() =>
            {
                leadingPadding = getWidth(_leadingKey);
            });
        }, debugLabel: "DropdownMenu.refreshLeadingPadding");
    }

    public virtual void scrollToHighlight()
    {
        WidgetsBinding.instance.addPostFrameCallback((_) =>
        {
            BuildContext? highlightContext = buttonItemKeys[(int)DartRuntimePrimitives.RequireValue(currentHighlight)].currentContext;
            if (highlightContext is not null)
            {
                DartRuntimePrimitives.Ignore(Scrollable.of(highlightContext).position.ensureVisible(highlightContext.findRenderObject()!));
            }
        }, debugLabel: "DropdownMenu.scrollToHighlight");
    }

    public virtual double? getWidth(GlobalKey<IState> key)
    {
        BuildContext? context = key.currentContext;
        if (context is not null)
        {
            var box = ((RenderBox?)context.findRenderObject()!)!;
            return box.hasSize ? box.size.width : null;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<DropdownMenuEntry<T>> filter(List<DropdownMenuEntry<T>> entries, TextEditingController textEditingController)
    {
        string filterText = textEditingController.text.toLowerCase();
        return entries.where((entry) => entry.label.toLowerCase().contains(filterText)).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _shouldUpdateCurrentHighlight(List<DropdownMenuEntry<T>> entries)
    {
        string searchText = _effectiveTextEditingController.value.text.toLowerCase();
        if (searchText.Length == 0)
        {
            return true;
        }
        if ((currentHighlight is null) || (DartRuntimePrimitives.RequireValue(currentHighlight) >= checked(entries.Count)))
        {
            return true;
        }
        if (entries[(int)DartRuntimePrimitives.RequireValue(currentHighlight)].label.toLowerCase().contains(searchText))
        {
            return false;
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long? search(List<DropdownMenuEntry<T>> entries, TextEditingController textEditingController)
    {
        string searchText = textEditingController.value.text.toLowerCase();
        if (searchText.Length == 0)
        {
            return null;
        }
        long index = entries.indexWhere((entry) => entry.label.toLowerCase().contains(searchText));
        return (index != -1L) ? index : null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<Widget> _buildButtons(List<DropdownMenuEntry<T>> filteredEntries, TextDirection textDirection, long? focusedIndex = null, bool enableScrollToHighlight = true, bool excludeSemantics = false)
    {
        double effectiveInputStartGap = Dropdown_menuLibrary._kInputStartGap;
        var result = new List<Widget>();
        for (var i = 0L; i < checked(filteredEntries.Count); i++)
        {
            DropdownMenuEntry<T> entry = filteredEntries[(int)i];
            double paddingLocal = (entry.leadingIcon is null) ? (leadingPadding ?? Dropdown_menuLibrary._kDefaultHorizontalPadding) : Dropdown_menuLibrary._kDefaultHorizontalPadding;
            ButtonStyle effectiveStyle = entry.style ?? MenuItemButton.styleFrom(padding: EdgeInsetsDirectional.CreateOnly(start: paddingLocal, end: Dropdown_menuLibrary._kDefaultHorizontalPadding));
            ButtonStyle? themeStyle = MenuButtonTheme.of(context).style;
            WidgetStateProperty<Color?>? effectiveForegroundColor = entry.style?.foregroundColor ?? themeStyle?.foregroundColor;
            WidgetStateProperty<Color?>? effectiveIconColor = entry.style?.iconColor ?? themeStyle?.iconColor;
            WidgetStateProperty<Color?>? effectiveOverlayColor = entry.style?.overlayColor ?? themeStyle?.overlayColor;
            WidgetStateProperty<Color?>? effectiveBackgroundColor = entry.style?.backgroundColor ?? themeStyle?.backgroundColor;
            bool entryIsSelected = entry.enabled && (i == focusedIndex);
            if (entryIsSelected)
            {
                _highlightedItemStatesController?.dispose();
                _highlightedItemStatesController = new WidgetStatesController(new HashSet<WidgetState> { WidgetState.focused });
                ButtonStyle defaultStyle = new MenuItemButton().defaultStyleOf(context);
                Color? resolveFocusedColor(WidgetStateProperty<Color?>? colorStateProperty)
                {
                    return colorStateProperty?.resolve(new HashSet<WidgetState> { WidgetState.focused });
                    throw new InvalidOperationException("Dart control flow completed without a value.");
                }
                Color focusedForegroundColor = resolveFocusedColor(effectiveForegroundColor ?? defaultStyle.foregroundColor!)!;
                Color focusedIconColor = resolveFocusedColor(effectiveIconColor ?? defaultStyle.iconColor!)!;
                Color focusedOverlayColor = resolveFocusedColor(effectiveOverlayColor ?? defaultStyle.overlayColor!)!;
                Color focusedBackgroundColor = resolveFocusedColor(effectiveBackgroundColor) ?? Theme.of(context).colorScheme.onSurface.withOpacity(0.12);
                effectiveStyle = effectiveStyle.copyWith(backgroundColor: new WidgetStatePropertyAll<Color>(focusedBackgroundColor), foregroundColor: new WidgetStatePropertyAll<Color>(focusedForegroundColor), iconColor: new WidgetStatePropertyAll<Color>(focusedIconColor), overlayColor: new WidgetStatePropertyAll<Color>(focusedOverlayColor));
            }
            else
            {
                effectiveStyle = effectiveStyle.copyWith(backgroundColor: effectiveBackgroundColor, foregroundColor: effectiveForegroundColor, iconColor: effectiveIconColor, overlayColor: effectiveOverlayColor);
            }
            Widget labelLocal = entry.labelWidget ?? new Text(entry.label);
            if (widget.width is not null)
            {
                double horizontalPadding = paddingLocal + Dropdown_menuLibrary._kDefaultHorizontalPadding + effectiveInputStartGap;
                labelLocal = DartRuntimePrimitives.ConvertValue<Widget>(new ConstrainedBox(constraints: new BoxConstraints(maxWidth: DartRuntimePrimitives.RequireValue(widget.width) - horizontalPadding), child: labelLocal));
            }
            Widget menuItemButton = new ExcludeFocus(child: new ExcludeSemantics(excluding: excludeSemantics, child: new MenuItemButton(key: enableScrollToHighlight ? buttonItemKeys[(int)i] : null, statesController: entryIsSelected ? _highlightedItemStatesController : null, style: effectiveStyle, leadingIcon: entry.leadingIcon, trailingIcon: entry.trailingIcon, closeOnActivate: Equals(widget.closeBehavior, DropdownMenuCloseBehavior.all), onPressed: (entry.enabled && widget.enabled) ? (() =>
            {
                if (!mounted)
                {
                    widget.controller?.value = new global::Doroti.Framework.Services.TextEditingValue(text: entry.label, selection: TextSelection.CreateCollapsed(offset: entry.label.Length));
                    widget.onSelected?.Invoke(entry.value);
                    return;
                }
                _effectiveTextEditingController.value = new global::Doroti.Framework.Services.TextEditingValue(text: entry.label, selection: TextSelection.CreateCollapsed(offset: entry.label.Length));
                currentHighlight = widget.enableSearch ? i : null;
                widget.onSelected?.Invoke(entry.value);
                _enableFilter = false;
                if (Equals(widget.closeBehavior, DropdownMenuCloseBehavior.self))
                {
                    _controller.close();
                }
            }) : null, requestFocusOnHover: false, child: new Padding(padding: EdgeInsetsDirectional.CreateOnly(start: effectiveInputStartGap), child: labelLocal))));
            result.Add(menuItemButton);
        }
        return result;
    }

    public virtual void handleUpKey(_ArrowUpIntent__dropdown_menu __unused0)
    {
        setState(() =>
        {
            if (!widget.enabled || !_menuHasEnabledItem || !_controller.isOpen)
            {
                return;
            }
            _enableFilter = false;
            _enableSearch = false;
            currentHighlight ??= 0L;
            currentHighlight = (DartRuntimePrimitives.RequireValue(currentHighlight) - 1L) % checked(filteredEntries.Count);
            while (!filteredEntries[(int)DartRuntimePrimitives.RequireValue(currentHighlight)].enabled)
            {
                currentHighlight = (DartRuntimePrimitives.RequireValue(currentHighlight) - 1L) % checked(filteredEntries.Count);
            }
            string currentLabel = filteredEntries[(int)DartRuntimePrimitives.RequireValue(currentHighlight)].label;
            _effectiveTextEditingController.value = new TextEditingValue(text: currentLabel, selection: TextSelection.CreateCollapsed(offset: currentLabel.Length));
        });
    }

    public virtual void handleDownKey(_ArrowDownIntent__dropdown_menu __unused0)
    {
        setState(() =>
        {
            if (!widget.enabled || !_menuHasEnabledItem || !_controller.isOpen)
            {
                return;
            }
            _enableFilter = false;
            _enableSearch = false;
            currentHighlight ??= -1L;
            currentHighlight = (DartRuntimePrimitives.RequireValue(currentHighlight) + 1L) % checked(filteredEntries.Count);
            while (!filteredEntries[(int)DartRuntimePrimitives.RequireValue(currentHighlight)].enabled)
            {
                currentHighlight = (DartRuntimePrimitives.RequireValue(currentHighlight) + 1L) % checked(filteredEntries.Count);
            }
            string currentLabel = filteredEntries[(int)DartRuntimePrimitives.RequireValue(currentHighlight)].label;
            _effectiveTextEditingController.value = new TextEditingValue(text: currentLabel, selection: TextSelection.CreateCollapsed(offset: currentLabel.Length));
        });
    }

    public virtual void handleEnterKey(_EnterIntent__dropdown_menu __unused0)
    {
        if (selectOnly && !_controller.isOpen)
        {
            _controller.open();
            return;
        }
        _handleSubmitted();
    }

    public virtual void handlePressed(MenuController controller, bool focusForKeyboard = true)
    {
        if (controller.isOpen)
        {
            currentHighlight = null;
            controller.close();
        }
        else
        {
            filteredEntries = widget.dropdownMenuEntries;
            if (_effectiveTextEditingController.text.Length != 0)
            {
                _enableFilter = false;
            }
            controller.open();
            if (focusForKeyboard)
            {
                _internalFocusNode.requestFocus();
            }
        }
        setState(() =>
        {
        });
    }

    internal virtual void _handleSubmitted()
    {
        if (currentHighlight is not null)
        {
            DropdownMenuEntry<T> entry = filteredEntries[(int)DartRuntimePrimitives.RequireValue(currentHighlight)];
            if (entry.enabled)
            {
                _effectiveTextEditingController.value = new TextEditingValue(text: entry.label, selection: TextSelection.CreateCollapsed(offset: entry.label.Length));
                widget.onSelected?.Invoke(entry.value);
            }
        }
        else
        {
            if (_controller.isOpen)
            {
                widget.onSelected?.Invoke(default);
            }
        }
        if (!widget.enableSearch)
        {
            currentHighlight = null;
        }
        _controller.close();
    }

    public override Widget build(BuildContext context)
    {
        TextDirection textDirection = Directionality.of(context);
        _initialMenu ??= _buildButtons(widget.dropdownMenuEntries, textDirection, enableScrollToHighlight: false, excludeSemantics: true);
        DropdownMenuThemeData theme = DropdownMenuTheme.of(context);
        DropdownMenuThemeData defaults = new _DropdownMenuDefaultsM3__dropdown_menu(context);
        if (_enableFilter)
        {
            filteredEntries = widget.filterCallback is null ? filter(widget.dropdownMenuEntries, _effectiveTextEditingController) : widget.filterCallback.Invoke(filteredEntries, _effectiveTextEditingController.text);
        }
        _menuHasEnabledItem = filteredEntries.any((entry) => entry.enabled);
        if (_enableSearch)
        {
            if (widget.searchCallback is not null)
            {
                currentHighlight = widget.searchCallback!(filteredEntries, _effectiveTextEditingController.text);
            }
            else
            {
                bool shouldUpdateCurrentHighlight = _shouldUpdateCurrentHighlight(filteredEntries);
                if (shouldUpdateCurrentHighlight)
                {
                    currentHighlight = search(filteredEntries, _effectiveTextEditingController);
                }
            }
            if (currentHighlight is not null)
            {
                scrollToHighlight();
            }
        }
        List<Widget> menu = _buildButtons(filteredEntries, textDirection, focusedIndex: currentHighlight);
        TextStyle? baseTextStyle = (widget.textStyle ?? theme.textStyle) ?? defaults.textStyle;
        Color? disabledColorLocal = theme.disabledColor ?? defaults.disabledColor;
        TextStyle? effectiveTextStyle = widget.enabled ? baseTextStyle : (baseTextStyle?.copyWith(color: disabledColorLocal) ?? new TextStyle(color: disabledColorLocal));
        MenuStyle? effectiveMenuStyle = (widget.menuStyle ?? theme.menuStyle) ?? defaults.menuStyle!;
        double? anchorWidth = getWidth(_anchorKey);
        if (widget.width is not null)
        {
            effectiveMenuStyle = effectiveMenuStyle.copyWith(minimumSize: WidgetStateProperty.resolveWith<Size?>((states) =>
            {
                double? effectiveMaximumWidth = effectiveMenuStyle!.maximumSize?.resolve(states)?.width;
                return new Size(Math.Min(DartRuntimePrimitives.RequireValue(widget.width), effectiveMaximumWidth ?? DartRuntimePrimitives.RequireValue(widget.width)), 0.0);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
        }
        else
        {
            if (anchorWidth is not null)
            {
                double anchorWidth__45717__value46193 = DartRuntimePrimitives.RequireValue(anchorWidth);
                effectiveMenuStyle = effectiveMenuStyle.copyWith(minimumSize: WidgetStateProperty.resolveWith<Size?>((states) =>
                {
                    double? effectiveMaximumWidthLocal = effectiveMenuStyle!.maximumSize?.resolve(states)?.width;
                    return new Size(Math.Min(DartRuntimePrimitives.RequireValue(anchorWidth__45717__value46193), effectiveMaximumWidthLocal ?? DartRuntimePrimitives.RequireValue(anchorWidth__45717__value46193)), 0.0);
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }));
            }
        }
        if (widget.menuHeight is not null)
        {
            effectiveMenuStyle = effectiveMenuStyle.copyWith(maximumSize: new WidgetStatePropertyAll<Size>(new Size(double.PositiveInfinity, DartRuntimePrimitives.RequireValue(widget.menuHeight))));
        }
        InputDecorationThemeData effectiveInputDecorationTheme = (widget.inputDecorationTheme ?? theme.inputDecorationTheme) ?? defaults.inputDecorationTheme!;
        MouseCursor? effectiveMouseCursor = (MouseCursor?)((object)widget.enabled switch { true => isButton ? SystemMouseCursors.click : SystemMouseCursors.text, false => DartRuntimePrimitives.ConvertValue<SystemMouseCursor>(null), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        Widget menuAnchor = new MenuAnchor(style: effectiveMenuStyle, alignmentOffset: widget.alignmentOffset, reservedPadding: EdgeInsets.zero, controller: _controller, menuChildren: menu, crossAxisUnconstrained: false, builder: (context, controller, child) =>
        {
            DartRuntimePrimitives.Assert(() => _initialMenu is not null);
            Func<BuildContext, MenuController, InputDecoration> decorationBuilderLocal = widget.decorationBuilder ?? _buildDefaultDecoration;
            InputDecoration decorationLocal = decorationBuilderLocal(context, controller);
            if (decorationLocal.suffixIcon is null)
            {
                decorationLocal = decorationLocal.copyWith(suffixIcon: _buildDefaultSuffixIcon(context, controller));
            }
            InputDecoration effectiveDecoration = decorationLocal.applyDefaults(effectiveInputDecorationTheme);
            InputDecoration textFieldDecoration = (effectiveDecoration.prefixIcon is null) ? effectiveDecoration : effectiveDecoration.copyWith(prefixIcon: new SizedBox(key: _leadingKey, child: effectiveDecoration.prefixIcon));
            MaterialLocalizations localizations = MaterialLocalizations.of(context);
            Widget textField = new Widgets.Semantics(button: isButton, hint: Equals(Theme.of(context).platform, TargetPlatform.iOS) ? (_controller.isOpen ? localizations.collapsedHint : localizations.expandedHint) : null, expanded: _controller.isOpen, onExpand: _controller.isOpen ? null : (() =>
            {
                _controller.open();
            }), onCollapse: !_controller.isOpen ? null : (() =>
            {
                _controller.close();
            }), child: new ExcludeSemantics(excluding: isButton && Foundation.ConstantsLibrary.kIsWeb, child: new TextField(key: _anchorKey, enabled: widget.enabled, mouseCursor: effectiveMouseCursor, focusNode: widget.focusNode, canRequestFocus: canRequestFocus(), enableInteractiveSelection: !isButton, readOnly: isButton, keyboardType: widget.keyboardType, textAlign: widget.textAlign, textAlignVertical: TextAlignVertical.center, maxLines: widget.maxLines, textInputAction: widget.textInputAction, cursorHeight: widget.cursorHeight, style: effectiveTextStyle, controller: _effectiveTextEditingController, onSubmitted: (_) => { _handleSubmitted(); }, onTap: !widget.enabled ? null : (() =>
            {
                handlePressed(controller, focusForKeyboard: !canRequestFocus());
            }), onChanged: (text) =>
            {
                controller.open();
                setState(() =>
                {
                    filteredEntries = widget.dropdownMenuEntries;
                    _enableFilter = widget.enableFilter;
                    _enableSearch = widget.enableSearch;
                });
            }, inputFormatters: widget.inputFormatters, decoration: textFieldDecoration, restorationId: widget.restorationId, scrollPadding: widget.scrollPadding)));
            Widget? effectiveLabel = effectiveDecoration.label ?? ((effectiveDecoration.labelText is not null) ? new Text(effectiveDecoration.labelText!) : null);
            Widget body = (widget.expandedInsets is not null) ? textField : new _DropdownMenuBody__dropdown_menu(width: widget.width, children: ((Func<List<Widget>>)(() => { var __collection52521 = new List<Widget>(); __collection52521.Add(DartRuntimePrimitives.ConvertValue<Widget>(textField)); __collection52521.AddRange(_initialMenu!); if (effectiveLabel is not null) { __collection52521.Add(DartRuntimePrimitives.ConvertValue<Widget>(new ExcludeSemantics(child: new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 4.0), child: new DefaultTextStyle(style: effectiveTextStyle!, child: effectiveLabel))))); } __collection52521.Add(DartRuntimePrimitives.ConvertValue<Widget>(effectiveDecoration.suffixIcon ?? SizedBox.CreateShrink())); __collection52521.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Padding(padding: EdgeInsets.CreateAll(8.0), child: effectiveDecoration.prefixIcon ?? SizedBox.CreateShrink()))); return __collection52521; }))());
            return new Shortcuts(shortcuts: selectOnly ? _selectOnlyShortcuts : _editableShortcuts, child: body);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        if (widget.expandedInsets is EdgeInsetsGeometry paddingLocal)
        {
            menuAnchor = DartRuntimePrimitives.ConvertValue<Widget>(new Padding(padding: paddingLocal.clamp(EdgeInsets.zero, EdgeInsets.CreateOnly(left: double.PositiveInfinity, right: double.PositiveInfinity).add(EdgeInsetsDirectional.CreateOnly(end: double.PositiveInfinity, start: double.PositiveInfinity))), child: menuAnchor));
        }
        menuAnchor = DartRuntimePrimitives.ConvertValue<Widget>(new Align(alignment: AlignmentDirectional.topStart, widthFactor: 1.0, heightFactor: 1.0, child: menuAnchor));
        return new Actions(actions: new DartMap<Type, dynamic> { [typeof(_ArrowUpIntent__dropdown_menu)] = new CallbackAction<_ArrowUpIntent__dropdown_menu>(onInvoke: (__arg0) => { ((Action<_ArrowUpIntent__dropdown_menu>)handleUpKey)(__arg0); return default!; }), [typeof(_ArrowDownIntent__dropdown_menu)] = new CallbackAction<_ArrowDownIntent__dropdown_menu>(onInvoke: (__arg0) => { ((Action<_ArrowDownIntent__dropdown_menu>)handleDownKey)(__arg0); return default!; }), [typeof(_EnterIntent__dropdown_menu)] = new CallbackAction<_EnterIntent__dropdown_menu>(onInvoke: (__arg0) => { ((Action<_EnterIntent__dropdown_menu>)handleEnterKey)(__arg0); return default!; }), [typeof(DismissIntent)] = new DismissMenuAction(controller: _controller) }, child: new Stack(children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new Shortcuts(shortcuts: new DartMap<ShortcutActivator, Intent> { [new SingleActivator(LogicalKeyboardKey.arrowUp)] = new _ArrowUpIntent__dropdown_menu(), [new SingleActivator(LogicalKeyboardKey.arrowDown)] = new _ArrowDownIntent__dropdown_menu(), [new SingleActivator(LogicalKeyboardKey.enter)] = new _EnterIntent__dropdown_menu(), [new SingleActivator(LogicalKeyboardKey.escape)] = new DismissIntent() }, child: new Focus(focusNode: _internalFocusNode, skipTraversal: true, child: SizedBox.CreateShrink()))), DartRuntimePrimitives.ConvertValue<Widget>(menuAnchor) }));
    }

    internal virtual InputDecoration _buildDefaultDecoration(BuildContext context, MenuController controller)
    {
        return new InputDecoration(label: widget.label, hintText: widget.hintText, helperText: widget.helperText, errorText: widget.errorText, prefixIcon: widget.leadingIcon, suffixIcon: _buildDefaultSuffixIcon(context, controller));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget? _buildDefaultSuffixIcon(BuildContext context, MenuController controller)
    {
        bool isCollapsedLocal = widget.inputDecorationTheme?.isCollapsed ?? false;
        return widget.showTrailingIcon ? new Padding(padding: isCollapsedLocal ? EdgeInsets.zero : EdgeInsets.CreateAll(4.0), child: new ExcludeSemantics(excluding: isButton, child: new IconButton(focusNode: _trailingIconButtonFocusNode, isSelected: controller.isOpen, constraints: widget.inputDecorationTheme?.suffixIconConstraints, padding: isCollapsedLocal ? EdgeInsets.zero : null, icon: widget.trailingIcon ?? new Icon(Icons.arrow_drop_down), selectedIcon: widget.selectedTrailingIcon ?? new Icon(Icons.arrow_drop_up), onPressed: !widget.enabled ? null : (() =>
        {
            handlePressed(controller);
        })))) : null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _ArrowUpIntent__dropdown_menu : Intent
{
    internal _ArrowUpIntent__dropdown_menu()
    {
    }

}

public class _ArrowDownIntent__dropdown_menu : Intent
{
    internal _ArrowDownIntent__dropdown_menu()
    {
    }

}

public class _EnterIntent__dropdown_menu : Intent
{
    internal _EnterIntent__dropdown_menu()
    {
    }

}

internal class _DropdownMenuBody__dropdown_menu : MultiChildRenderObjectWidget
{
    public virtual double? width { get; private set; }

    internal _DropdownMenuBody__dropdown_menu(List<Widget> children = default!, double? width = null) : base(children: children ?? new List<Widget>())
    {
        this.width = width;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderDropdownMenuBody__dropdown_menu(width: width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderDropdownMenuBody__dropdown_menu)renderObject;
        __renderObject.width = width;
    }

}

internal class _DropdownMenuBodyParentData__dropdown_menu : ContainerBoxParentData<RenderBox>
{
}

public class _RenderDropdownMenuBody__dropdown_menu : RenderBox, ContainerRenderObjectMixin<RenderBox, _DropdownMenuBodyParentData__dropdown_menu>, RenderBoxContainerDefaultsMixin<RenderBox, _DropdownMenuBodyParentData__dropdown_menu>
{
    internal virtual double? _width { get; set; } = default;
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    internal _RenderDropdownMenuBody__dropdown_menu(double? width = null)
    {
        _width = width;
    }

    public virtual double? width
    {
        get => _width;
        set
        {
            var __value = value;
            if (_width == __value)
            {
                return;
            }
            _width = __value;
            markNeedsLayout();
        }
    }
    public override void setupParentData(RenderObject child)
    {
        var __child = (RenderBox)child;
        if (__child.parentData is not _DropdownMenuBodyParentData__dropdown_menu)
        {
            __child.parentData = new _DropdownMenuBodyParentData__dropdown_menu();
        }
    }

    public override void performLayout()
    {
        BoxConstraints constraintsLocal = constraints;
        var maxWidthLocal = 0.0;
        double? maxHeightLocal = default!;
        RenderBox? child = firstChild;
        double intrinsicWidth = width ?? (double)getMaxIntrinsicWidth(constraintsLocal.maxHeight);
        double widthConstraint = Math.Min(intrinsicWidth, constraintsLocal.maxWidth);
        var innerConstraints = new BoxConstraints(maxWidth: widthConstraint, maxHeight: getMaxIntrinsicHeight(widthConstraint));
        while (child is not null)
        {
            if (Equals(child, firstChild))
            {
                child.layout(innerConstraints, parentUsesSize: true);
                maxHeightLocal ??= child.size.height;
                var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
                DartRuntimePrimitives.Assert(() => Equals(child.parentData, childParentData));
                child = childParentData.nextSibling;
                continue;
            }
            child.layout(innerConstraints, parentUsesSize: true);
            var childParentDataLocal = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
            childParentDataLocal.offset = Offset.zero;
            maxWidthLocal = Math.Max(maxWidthLocal, child.size.width);
            maxHeightLocal ??= child.size.height;
            DartRuntimePrimitives.Assert(() => Equals(child.parentData, childParentDataLocal));
            child = childParentDataLocal.nextSibling;
        }
        DartRuntimePrimitives.Assert(() => maxHeightLocal is not null);
        maxWidthLocal = Math.Max(Dropdown_menuLibrary._kMinimumWidth, maxWidthLocal);
        size = constraintsLocal.constrain(new Size(width ?? maxWidthLocal, DartRuntimePrimitives.RequireValue(maxHeightLocal)));
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        RenderBox? child = firstChild;
        if (child is not null)
        {
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
            context.paintChild(child, offset + childParentData.offset);
        }
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        var maxWidthLocal = 0.0;
        double? maxHeightLocal = default!;
        RenderBox? child = firstChild;
        double intrinsicWidth = width ?? (double)getMaxIntrinsicWidth(constraints.maxHeight);
        double widthConstraint = Math.Min(intrinsicWidth, constraints.maxWidth);
        var innerConstraints = new BoxConstraints(maxWidth: widthConstraint, maxHeight: getMaxIntrinsicHeight(widthConstraint));
        while (child is not null)
        {
            Size childSize = child.getDryLayout(innerConstraints);
            if (!Equals(child, firstChild))
            {
                maxWidthLocal = Math.Max(maxWidthLocal, childSize.width);
            }
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
            maxHeightLocal ??= childSize.height;
            child = childParentData.nextSibling;
        }
        DartRuntimePrimitives.Assert(() => maxHeightLocal is not null);
        maxWidthLocal = Math.Max(Dropdown_menuLibrary._kMinimumWidth, maxWidthLocal);
        return constraints.constrain(new Size(width ?? maxWidthLocal, DartRuntimePrimitives.RequireValue(maxHeightLocal)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        RenderBox? child = firstChild;
        double width = 0;
        while (child is not null)
        {
            if (Equals(child, firstChild))
            {
                var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
                child = childParentData.nextSibling;
                continue;
            }
            double minIntrinsicWidth = child.getMinIntrinsicWidth(height);
            if (Equals(child, lastChild))
            {
                width += minIntrinsicWidth;
            }
            if (Equals(child, childBefore(lastChild!)))
            {
                width += minIntrinsicWidth;
            }
            width = Math.Max(DartRuntimePrimitives.RequireValue(width), minIntrinsicWidth);
            var childParentDataLocal = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
            child = childParentDataLocal.nextSibling;
        }
        return Math.Max(DartRuntimePrimitives.RequireValue(width), Dropdown_menuLibrary._kMinimumWidth);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        RenderBox? child = firstChild;
        double width = 0;
        while (child is not null)
        {
            if (Equals(child, firstChild))
            {
                var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
                child = childParentData.nextSibling;
                continue;
            }
            double maxIntrinsicWidth = child.getMaxIntrinsicWidth(height);
            if (Equals(child, lastChild))
            {
                width += maxIntrinsicWidth;
            }
            if (Equals(child, childBefore(lastChild!)))
            {
                width += maxIntrinsicWidth;
            }
            width = Math.Max(DartRuntimePrimitives.RequireValue(width), maxIntrinsicWidth);
            var childParentDataLocal = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
            child = childParentDataLocal.nextSibling;
        }
        return Math.Max(DartRuntimePrimitives.RequireValue(width), Dropdown_menuLibrary._kMinimumWidth);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        RenderBox? child = firstChild;
        double widthLocal = 0;
        if (child is not null)
        {
            widthLocal = Math.Max(DartRuntimePrimitives.RequireValue(widthLocal), child.getMinIntrinsicHeight(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(widthLocal))));
        }
        return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(widthLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        RenderBox? child = firstChild;
        double widthLocal = 0;
        if (child is not null)
        {
            widthLocal = Math.Max(DartRuntimePrimitives.RequireValue(widthLocal), child.getMaxIntrinsicHeight(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(widthLocal))));
        }
        return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(widthLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child = firstChild;
        if (child is not null)
        {
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
            bool isHit = result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: (result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => Equals(transformed, position - childParentData.offset));
                return child.hitTest(result, position: transformed);
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
            if (isHit)
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        visitChildren((renderObjectChild) =>
        {
            var child = ((RenderBox?)renderObjectChild)!;
            if (Equals(child, firstChild))
            {
                visitor((RenderBox)renderObjectChild);
            }
        });
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
        while (childParentData.previousSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.previousSibling, child));
            child = childParentData.previousSibling!;
            childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
        while (childParentData.nextSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.nextSibling, child));
            child = childParentData.nextSibling!;
            childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long childCount => _childCount;
    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (child is not RenderBox)
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {GetType()} that expected a {typeof(RenderBox)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", child.debugCreator, style: DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => childParentData.nextSibling is null);
        DartRuntimePrimitives.Assert(() => childParentData.previousSibling is null);
        _childCount += 1L;
        DartRuntimePrimitives.Assert(() => _childCount > 0L);
        if (after is null)
        {
            childParentData.nextSibling = _firstChild;
            if (_firstChild is not null)
            {
                var firstChildParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)_firstChild!.parentData!)!;
                firstChildParentData.previousSibling = child;
            }
            _firstChild = child;
            _lastChild ??= child;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => _firstChild is not null);
            DartRuntimePrimitives.Assert(() => _lastChild is not null);
            DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(after, equals: _firstChild));
            DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(after, equals: _lastChild));
            var afterParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)after.parentData!)!;
            if (afterParentData.nextSibling is null)
            {
                DartRuntimePrimitives.Assert(() => Equals(after, _lastChild));
                childParentData.previousSibling = after;
                afterParentData.nextSibling = child;
                _lastChild = child;
            }
            else
            {
                childParentData.nextSibling = afterParentData.nextSibling;
                childParentData.previousSibling = after;
                var childPreviousSiblingParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)childParentData.nextSibling!.parentData!)!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => Equals(afterParentData.nextSibling, child));
            }
        }
    }

    public virtual void insert(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(child, this), () => (object?)"A RenderObject cannot be inserted into itself.");
        DartRuntimePrimitives.Assert(() => !Equals(after, this), () => (object?)"A RenderObject cannot simultaneously be both the parent and the sibling of another RenderObject.");
        DartRuntimePrimitives.Assert(() => !Equals(child, after), () => (object?)"A RenderObject cannot be inserted after itself.");
        DartRuntimePrimitives.Assert(() => !Equals(child, _firstChild));
        DartRuntimePrimitives.Assert(() => !Equals(child, _lastChild));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => child.parentData is _DropdownMenuBodyParentData__dropdown_menu, () => (object?)$"A child of {GetType()} has parentData of type {DartRuntimePrimitives.RuntimeType(child.parentData)}, " + $"which does not conform to {typeof(_DropdownMenuBodyParentData__dropdown_menu)}. Class using ContainerRenderObjectMixin " + $"should override setupParentData() to set parentData to type {typeof(_DropdownMenuBodyParentData__dropdown_menu)}.");
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderBox child)
    {
        insert(child, after: _lastChild);
    }

    public virtual void addAll(List<RenderBox>? children)
    {
        children?.forEach((__arg0) => ((Action<RenderBox>)add)(__arg0));
    }

    public virtual void _removeFromChildList(RenderBox child)
    {
        var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: _firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: _lastChild));
        DartRuntimePrimitives.Assert(() => _childCount >= 0L);
        if (childParentData.previousSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_firstChild, child));
            _firstChild = childParentData.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if (childParentData.nextSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_lastChild, child));
            _lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)childParentData.nextSibling!.parentData!)!;
            childNextSiblingParentData.previousSibling = childParentData.previousSibling;
        }
        childParentData.previousSibling = null;
        childParentData.nextSibling = null;
        _childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
            RenderBox? next = childParentData.nextSibling;
            childParentData.previousSibling = null;
            childParentData.nextSibling = null;
            dropChild(child);
            child = next;
        }
        _firstChild = null;
        _lastChild = null;
        _childCount = 0L;
    }

    public virtual void move(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(child, this));
        DartRuntimePrimitives.Assert(() => !Equals(after, this));
        DartRuntimePrimitives.Assert(() => !Equals(child, after));
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
        if (Equals(childParentData.previousSibling, after))
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
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            child.attach(owner);
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            child.detach();
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            redepthChild(child);
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            visitor(child);
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => _firstChild;
    public virtual RenderBox? lastChild => _lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<DiagnosticsNode>();
        if (firstChild is not null)
        {
            RenderBox child = firstChild!;
            var count = 1L;
            while (true)
            {
                children.Add(((Diagnosticable)child).toDiagnosticsNode(name: $"child__183606 {count}"));
                if (Equals(child, lastChild))
                {
                    break;
                }
                count += 1L;
                var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
                child = childParentData.nextSibling!;
            }
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToFirstActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
            double? result = child.getDistanceToActualBaseline(baseline);
            if (result is not null)
            {
                double result__138852__value138916 = DartRuntimePrimitives.RequireValue(result);
                return DartRuntimePrimitives.RequireValue(result__138852__value138916) + childParentData.offset.dy;
            }
            child = childParentData.nextSibling;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToHighestActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        BaselineOffset minBaseline = BaselineOffset.noBaseline;
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
            BaselineOffset candidate = new BaselineOffset(child.getDistanceToActualBaseline(baseline)).op_Add(childParentData.offset.dy);
            minBaseline = minBaseline.minOf(candidate);
            child = childParentData.nextSibling;
        }
        return minBaseline.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool defaultHitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child = lastChild;
        while (child is not null)
        {
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
            bool isHit = result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: (result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => Equals(transformed, position - childParentData.offset));
                return child!.hitTest(result, position: transformed);
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
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
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
            context.paintChild(child, childParentData.offset + offset);
            child = childParentData.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result = new List<RenderBox>();
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((_DropdownMenuBodyParentData__dropdown_menu?)child.parentData!)!;
            result.Add(child!);
            child = childParentData.nextSibling;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DropdownMenuDefaultsM3__dropdown_menu : DropdownMenuThemeData
{
    public virtual BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(context);
                __late__theme_initialized = true;
            }
            return __late__theme;
        }
    }

    internal _DropdownMenuDefaultsM3__dropdown_menu(BuildContext context) : base(disabledColor: Theme.of(context).colorScheme.onSurface.withOpacity(0.38))
    {
        this.context = context;
    }

    public override TextStyle? textStyle => _theme.textTheme.bodyLarge;
    public override MenuStyle menuStyle
    {
        get
        {
            return new MenuStyle(minimumSize: new WidgetStatePropertyAll<Size>(new Size(Dropdown_menuLibrary._kMinimumWidth, 0.0)), maximumSize: new WidgetStatePropertyAll<Size>(Size.infinite), visualDensity: VisualDensity.standard);
        }
    }
    public override InputDecorationThemeData inputDecorationTheme
    {
        get
        {
            return new InputDecorationThemeData(border: new OutlineInputBorder());
        }
    }
}
