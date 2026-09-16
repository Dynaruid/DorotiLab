// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/autocomplete.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public delegate object AutocompleteOptionsBuilder<T>(global::Doroti.Framework.Services.TextEditingValue textEditingValue);

public delegate void AutocompleteOnSelected<T>(T option);

public delegate Widget AutocompleteOptionsViewBuilder<T>(BuildContext context, global::System.Action<T> onSelected, IEnumerable<T> options);

public delegate Widget AutocompleteFieldViewBuilder(BuildContext context, TextEditingController textEditingController, FocusNode focusNode, global::System.Action onFieldSubmitted);

public delegate string AutocompleteOptionToString<T>(T option);

public enum OptionsViewOpenDirection
{
    up,
    down,
    mostSpace
}

public class RawAutocomplete<T> : StatefulWidget
{
    public virtual global::System.Func<BuildContext, TextEditingController, FocusNode, global::System.Action, Widget>? fieldViewBuilder { get; private set; }
    public virtual FocusNode? focusNode { get; private set; }
    public virtual global::System.Func<BuildContext, global::System.Action<T>, IEnumerable<T>, Widget> optionsViewBuilder { get; private set; } = default!;
    public virtual OptionsViewOpenDirection optionsViewOpenDirection { get; private set; } = default!;
    public virtual global::System.Func<T, string> displayStringForOption { get; private set; } = default!;
    public virtual global::System.Action<T>? onSelected { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Services.TextEditingValue, object> optionsBuilder { get; private set; } = default!;
    public virtual TextEditingController? textEditingController { get; private set; }
    public virtual global::Doroti.Framework.Services.TextEditingValue? initialValue { get; private set; }

    public RawAutocomplete(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, global::System.Action<T>, IEnumerable<T>, Widget> optionsViewBuilder = default!, global::System.Func<global::Doroti.Framework.Services.TextEditingValue, object> optionsBuilder = default!, OptionsViewOpenDirection optionsViewOpenDirection = OptionsViewOpenDirection.down, global::System.Func<T, string> displayStringForOption = default!, global::System.Func<BuildContext, TextEditingController, FocusNode, global::System.Action, Widget>? fieldViewBuilder = null, FocusNode? focusNode = null, global::System.Action<T>? onSelected = null, TextEditingController? textEditingController = null, global::Doroti.Framework.Services.TextEditingValue? initialValue = null) : base(key: key)
    {
        global::System.Func<T, string> __displayStringForOption = displayStringForOption ?? new global::System.Func<T, string>((__option) => defaultStringForOption(__option));
        this.optionsViewBuilder = optionsViewBuilder;
        this.optionsBuilder = optionsBuilder;
        this.optionsViewOpenDirection = optionsViewOpenDirection;
        this.displayStringForOption = __displayStringForOption;
        this.fieldViewBuilder = fieldViewBuilder;
        this.focusNode = focusNode;
        this.onSelected = onSelected;
        this.textEditingController = textEditingController;
        this.initialValue = initialValue;
        System.Diagnostics.Debug.Assert(((fieldViewBuilder is not null) || ((((key is not null) && (focusNode is not null)) && (textEditingController is not null)))));
        System.Diagnostics.Debug.Assert((((focusNode is null)) == ((textEditingController is null))));
        System.Diagnostics.Debug.Assert(!(((textEditingController is not null) && (initialValue is not null))));
    }

    public static void onFieldSubmitted<TOption>(GlobalKey<IState> key)
    {
        var rawAutocomplete = ((_RawAutocompleteState__autocomplete<TOption>?)((GlobalKey<IState>)key).currentState!)!;
        rawAutocomplete._onFieldSubmitted();
    }

    public static string defaultStringForOption(object? option)
    {
        return option!.ToString()!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _RawAutocompleteState__autocomplete<T>());
}

internal class _RawAutocompleteState__autocomplete<T> : State<RawAutocomplete<T>>
{
    internal virtual OverlayPortalController _optionsViewController { get; private set; } = new OverlayPortalController(debugLabel: "_RawAutocompleteState");
    internal const long _pageSize = 4L;
    internal virtual bool _hasFocus { get; set; } = default!;
    internal virtual bool _selecting { get; set; } = false;
    internal virtual TextEditingController? _internalTextEditingController { get; set; } = default;
    internal virtual FocusNode? _internalFocusNode { get; set; } = default;
    private bool __late__actionMap_initialized;
    private DartMap<Type, IIntentAction> __late__actionMap = default!;
    internal virtual DartMap<Type, IIntentAction> _actionMap
    {
        get
        {
            if (!__late__actionMap_initialized)
            {
                __late__actionMap = new DartMap<Type, IIntentAction> { [typeof(AutocompletePreviousOptionIntent)] = ((IIntentAction)new _AutocompleteCallbackAction__autocomplete<AutocompletePreviousOptionIntent>(onInvoke: (__arg0) => { ((global::System.Action<AutocompletePreviousOptionIntent>)this._highlightPreviousOption)(__arg0); return default!; }, isEnabledCallback: ((global::System.Func<bool>)(() => this._canShowOptionsView)))), [typeof(AutocompleteNextOptionIntent)] = ((IIntentAction)new _AutocompleteCallbackAction__autocomplete<AutocompleteNextOptionIntent>(onInvoke: (__arg0) => { ((global::System.Action<AutocompleteNextOptionIntent>)this._highlightNextOption)(__arg0); return default!; }, isEnabledCallback: ((global::System.Func<bool>)(() => this._canShowOptionsView)))), [typeof(AutocompleteFirstOptionIntent)] = ((IIntentAction)new _AutocompleteCallbackAction__autocomplete<AutocompleteFirstOptionIntent>(onInvoke: (__arg0) => { ((global::System.Action<AutocompleteFirstOptionIntent>)this._highlightFirstOption)(__arg0); return default!; }, isEnabledCallback: ((global::System.Func<bool>)(() => this._canShowOptionsView)))), [typeof(AutocompleteLastOptionIntent)] = ((IIntentAction)new _AutocompleteCallbackAction__autocomplete<AutocompleteLastOptionIntent>(onInvoke: (__arg0) => { ((global::System.Action<AutocompleteLastOptionIntent>)this._highlightLastOption)(__arg0); return default!; }, isEnabledCallback: ((global::System.Func<bool>)(() => this._canShowOptionsView)))), [typeof(AutocompleteNextPageOptionIntent)] = ((IIntentAction)new _AutocompleteCallbackAction__autocomplete<AutocompleteNextPageOptionIntent>(onInvoke: (__arg0) => { ((global::System.Action<AutocompleteNextPageOptionIntent>)this._highlightNextPageOption)(__arg0); return default!; }, isEnabledCallback: ((global::System.Func<bool>)(() => this._canShowOptionsView)))), [typeof(AutocompletePreviousPageOptionIntent)] = ((IIntentAction)new _AutocompleteCallbackAction__autocomplete<AutocompletePreviousPageOptionIntent>(onInvoke: (__arg0) => { ((global::System.Action<AutocompletePreviousPageOptionIntent>)this._highlightPreviousPageOption)(__arg0); return default!; }, isEnabledCallback: ((global::System.Func<bool>)(() => this._canShowOptionsView)))), [typeof(DismissIntent)] = ((IIntentAction)new CallbackAction<DismissIntent>(onInvoke: (global::System.Func<DismissIntent, object?>)this._hideOptions)) };
                __late__actionMap_initialized = true;
            }
            return __late__actionMap;
        }
    }
    internal virtual IEnumerable<T> _options { get; set; } = Enumerable.Empty<T>();
    internal virtual T? _selection { get; set; } = default;
    internal virtual string? _lastFieldText { get; set; } = default;
    internal virtual global::Doroti.Framework.Foundation.ValueNotifier<long> _highlightedOptionIndex { get; private set; } = new global::Doroti.Framework.Foundation.ValueNotifier<long>(0L);
    internal static DartMap<ShortcutActivator, Intent> _appleShortcuts = new DartMap<ShortcutActivator, Intent> { [new SingleActivator(LogicalKeyboardKey.arrowUp, meta: true)] = ((Intent)new AutocompleteFirstOptionIntent()), [new SingleActivator(LogicalKeyboardKey.arrowDown, meta: true)] = ((Intent)new AutocompleteLastOptionIntent()) };
    internal static DartMap<ShortcutActivator, Intent> _nonAppleShortcuts = new DartMap<ShortcutActivator, Intent> { [new SingleActivator(LogicalKeyboardKey.arrowUp, control: true)] = ((Intent)new AutocompleteFirstOptionIntent()), [new SingleActivator(LogicalKeyboardKey.arrowDown, control: true)] = ((Intent)new AutocompleteLastOptionIntent()) };
    internal static DartMap<ShortcutActivator, Intent> _commonShortcuts = new DartMap<ShortcutActivator, Intent> { [new SingleActivator(LogicalKeyboardKey.arrowUp)] = ((Intent)new AutocompletePreviousOptionIntent()), [new SingleActivator(LogicalKeyboardKey.arrowDown)] = ((Intent)new AutocompleteNextOptionIntent()), [new SingleActivator(LogicalKeyboardKey.pageUp)] = ((Intent)new AutocompletePreviousPageOptionIntent()), [new SingleActivator(LogicalKeyboardKey.pageDown)] = ((Intent)new AutocompleteNextPageOptionIntent()) };
    internal virtual long _onChangedCallId { get; set; } = 0L;
    internal static double _kMinUsableHeight = ConstantsLibrary.kMinInteractiveDimension;

    internal virtual TextEditingController _textEditingController
    {
        get
        {
            return (((RawAutocomplete<T>)this.widget).textEditingController ?? (_internalTextEditingController ??= ((Func<TextEditingController>)(() =>
{
    var __cascade = new TextEditingController();
    __cascade.addListener(this._onChangedFieldListener);
    return __cascade;
}))()));
        }
    }
    internal virtual FocusNode _focusNode
    {
        get
        {
            return (((RawAutocomplete<T>)this.widget).focusNode ?? (_internalFocusNode ??= ((Func<FocusNode>)(() =>
{
    var __cascade = new FocusNode();
    __cascade.addListener(this._onFocusChange);
    return __cascade;
}))()));
        }
    }
    internal static DartMap<ShortcutActivator, Intent> _shortcuts => new DartMap<ShortcutActivator, Intent>();
    internal virtual bool _canShowOptionsView => DartRuntimePrimitives.ConvertValue<bool>((((FocusNode)this._focusNode).hasFocus && Enumerable.Any(this._options)));
    internal virtual void _onFocusChange()
    {
        if ((((FocusNode)this._focusNode).hasFocus != this._hasFocus))
        {
            _hasFocus = ((FocusNode)this._focusNode).hasFocus;
            _updateOptionsViewVisibility();
        }
    }

    internal virtual void _updateOptionsViewVisibility()
    {
        if (this._canShowOptionsView)
        {
            this._optionsViewController.show();
        }
        else
        {
            if (((OverlayPortalController)this._optionsViewController).isShowing)
            {
                this._optionsViewController.hide();
            }
        }
    }

    internal virtual void _announceSemantics(bool resultsAvailable)
    {
        if (!MediaQuery.supportsAnnounceOf(this.context))
        {
            return;
        }
        WidgetsLocalizations localizations = ((WidgetsLocalizations)WidgetsLocalizations.of(this.context));
        string optionsHint = (resultsAvailable ? ((WidgetsLocalizations)localizations).searchResultsFound : ((WidgetsLocalizations)localizations).noResultsFound);
        DartRuntimePrimitives.Ignore(SemanticsService.sendAnnouncement(View.of(this.context), optionsHint, ((WidgetsLocalizations)localizations).textDirection).catchError(((global::System.Action<object, global::System.Diagnostics.StackTrace?>)((exception, stack) =>
        {
            FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception, stack: stack, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while sending semantics announcement")));
        }))));
    }

    private void _onChangedFieldListener() => _ = _onChangedField();

    internal async virtual Future _onChangedField()
    {
        if (this._selecting)
        {
            return;
        }
        global::Doroti.Framework.Services.TextEditingValue valueLocal = ((global::Doroti.Framework.Services.TextEditingValue)this._textEditingController.value);
        var shouldUpdateOptions = false;
        if ((((global::Doroti.Framework.Services.TextEditingValue)valueLocal).text != this._lastFieldText))
        {
            shouldUpdateOptions = true;
            _onChangedCallId += 1L;
        }
        _lastFieldText = ((global::Doroti.Framework.Services.TextEditingValue)valueLocal).text;
        long callId = this._onChangedCallId;
        IEnumerable<T> options = await DartAsyncRuntime.AwaitFutureOrValue<IEnumerable<T>>(this.widget.optionsBuilder(valueLocal));
        if (!this.mounted)
        {
            return;
        }
        if (((callId != this._onChangedCallId) || !shouldUpdateOptions))
        {
            return;
        }
        if ((!Enumerable.Any(this._options) != !Enumerable.Any(options)))
        {
            _announceSemantics(Enumerable.Any(options));
        }
        _options = options;
        _updateHighlight(((global::Doroti.Framework.Foundation.ValueNotifier<long>)this._highlightedOptionIndex).value);
        T? selection = this._selection;
        if (((selection is not null) && (((global::Doroti.Framework.Services.TextEditingValue)valueLocal).text != this.widget.displayStringForOption(selection))))
        {
            _selection = default(T);
        }
        _updateOptionsViewVisibility();
    }

    internal virtual void _onFieldSubmitted()
    {
        if (((OverlayPortalController)this._optionsViewController).isShowing)
        {
            _select(this._options.elementAt(((global::Doroti.Framework.Foundation.ValueNotifier<long>)this._highlightedOptionIndex).value));
        }
    }

    internal virtual void _select(T nextSelection)
    {
        if (EqualityComparer<T>.Default.Equals(nextSelection, this._selection))
        {
            return;
        }
        _selecting = true;
        _selection = nextSelection;
        string selectionString = this.widget.displayStringForOption(nextSelection);
        this._textEditingController.value = new global::Doroti.Framework.Services.TextEditingValue(selection: TextSelection.CreateCollapsed(offset: selectionString.Length), text: selectionString);
        _lastFieldText = selectionString;
        ((RawAutocomplete<T>)this.widget).onSelected?.Invoke(nextSelection);
        if (((OverlayPortalController)this._optionsViewController).isShowing)
        {
            this._optionsViewController.hide();
        }
        _selecting = false;
    }

    internal virtual void _updateHighlight(long nextIndex)
    {
        this._highlightedOptionIndex.value = (!Enumerable.Any(this._options) ? 0L : nextIndex.clamp(0L, (this._options.Count() - 1L)));
    }

    internal virtual void _highlightPreviousOption(AutocompletePreviousOptionIntent intent)
    {
        _highlightOption((((global::Doroti.Framework.Foundation.ValueNotifier<long>)this._highlightedOptionIndex).value - 1L));
    }

    internal virtual void _highlightNextOption(AutocompleteNextOptionIntent intent)
    {
        _highlightOption((((global::Doroti.Framework.Foundation.ValueNotifier<long>)this._highlightedOptionIndex).value + 1L));
    }

    internal virtual void _highlightFirstOption(AutocompleteFirstOptionIntent intent)
    {
        _highlightOption(0L);
    }

    internal virtual void _highlightLastOption(AutocompleteLastOptionIntent intent)
    {
        _highlightOption((this._options.Count() - 1L));
    }

    internal virtual void _highlightNextPageOption(AutocompleteNextPageOptionIntent intent)
    {
        _highlightOption((((global::Doroti.Framework.Foundation.ValueNotifier<long>)this._highlightedOptionIndex).value + _pageSize));
    }

    internal virtual void _highlightPreviousPageOption(AutocompletePreviousPageOptionIntent intent)
    {
        _highlightOption((((global::Doroti.Framework.Foundation.ValueNotifier<long>)this._highlightedOptionIndex).value - _pageSize));
    }

    internal virtual void _highlightOption(long index)
    {
        DartRuntimePrimitives.Assert(() => this._canShowOptionsView);
        _updateOptionsViewVisibility();
        DartRuntimePrimitives.Assert(() => ((OverlayPortalController)this._optionsViewController).isShowing);
        _updateHighlight(index);
    }

    internal virtual object? _hideOptions(DismissIntent intent)
    {
        if (((OverlayPortalController)this._optionsViewController).isShowing)
        {
            this._optionsViewController.hide();
            return null;
        }
        else
        {
            return Actions.invoke(this.context, intent);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildOptionsView(BuildContext context, OverlayChildLayoutInfo layoutInfo)
    {
        if ((((OverlayChildLayoutInfo)layoutInfo).childPaintTransform.determinant == 0.0))
        {
            return ((Widget)SizedBox.CreateShrink());
        }
        global::Doroti.Ui.Size fieldSize = ((global::Doroti.Ui.Size)((OverlayChildLayoutInfo)layoutInfo).childSize);
        Matrix4 invertTransform = ((Func<Matrix4>)(() =>
{
    var __cascade = ((OverlayChildLayoutInfo)layoutInfo).childPaintTransform.clone();
    __cascade.invert();
    return __cascade;
}))();
        global::Doroti.Framework.Painting.EdgeInsets mediaQueryPadding = ((global::Doroti.Framework.Painting.EdgeInsets)MediaQuery.paddingOf(context));
        global::Doroti.Framework.Painting.EdgeInsets viewInsets = ((global::Doroti.Framework.Painting.EdgeInsets)MediaQuery.viewInsetsOf(context));
        global::Doroti.Ui.Rect overlayRect = ((global::Doroti.Ui.Rect)mediaQueryPadding.deflateRect(viewInsets.deflateRect((Offset.zero & ((OverlayChildLayoutInfo)layoutInfo).overlaySize))));
        global::Doroti.Ui.Rect overlayRectInField = ((global::Doroti.Ui.Rect)MatrixUtils.transformRect(invertTransform, overlayRect));
        double spaceAbove = -overlayRectInField.top;
        double spaceBelow = (overlayRectInField.bottom - fieldSize.height);
        bool opensUp = (((RawAutocomplete<T>)this.widget).optionsViewOpenDirection switch { OptionsViewOpenDirection.up => true, OptionsViewOpenDirection.down => false, OptionsViewOpenDirection.mostSpace => (spaceAbove > spaceBelow), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        double optionsViewMaxHeight = (opensUp ? -overlayRectInField.top : (overlayRectInField.bottom - fieldSize.height));
        var optionsViewBoundingBox = new global::Doroti.Ui.Size(fieldSize.width, Math.Max(optionsViewMaxHeight, _kMinUsableHeight));
        double originY = (opensUp ? overlayRectInField.top : (overlayRectInField.bottom - optionsViewBoundingBox.height));
        Matrix4 transformLocal = ((Func<Matrix4>)(() =>
{
    var __cascade = ((OverlayChildLayoutInfo)layoutInfo).childPaintTransform.clone();
    __cascade.translateByDouble(0.0, originY, 0, 1);
    return __cascade;
}))();
        Widget childLocal = ((Widget)new Builder(builder: ((global::System.Func<BuildContext, Widget>)((context) => this.widget.optionsViewBuilder(context, this._select, this._options)))));
        return ((Widget)new Transform(transform: transformLocal, child: new Align(alignment: Alignment.topLeft, child: new ConstrainedBox(constraints: BoxConstraints.CreateTight(optionsViewBoundingBox), child: new Align(alignment: (opensUp ? AlignmentDirectional.bottomStart : AlignmentDirectional.topStart), child: new TextFieldTapRegion(child: new AutocompleteHighlightedOption(highlightIndexNotifier: this._highlightedOptionIndex, child: new ExcludeFocus(child: childLocal))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void initState()
    {
        base.initState();
        TextEditingController initialController = (((RawAutocomplete<T>)this.widget).textEditingController ?? (_internalTextEditingController = TextEditingController.CreateFromValue(((RawAutocomplete<T>)this.widget).initialValue)));
        initialController.addListener(this._onChangedFieldListener);
        _hasFocus = ((FocusNode)this._focusNode).hasFocus;
        ((RawAutocomplete<T>)this.widget).focusNode?.addListener(this._onFocusChange);
    }

    public override void didUpdateWidget(RawAutocomplete<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!DartRuntimePrimitives.Identical(((RawAutocomplete<T>)oldWidget).textEditingController, ((RawAutocomplete<T>)this.widget).textEditingController))
        {
            ((RawAutocomplete<T>)oldWidget).textEditingController?.removeListener(this._onChangedFieldListener);
            if ((((RawAutocomplete<T>)oldWidget).textEditingController is null))
            {
                this._internalTextEditingController?.dispose();
                _internalTextEditingController = null;
            }
            ((RawAutocomplete<T>)this.widget).textEditingController?.addListener(this._onChangedFieldListener);
        }
        if (!DartRuntimePrimitives.Identical(((RawAutocomplete<T>)oldWidget).focusNode, ((RawAutocomplete<T>)this.widget).focusNode))
        {
            ((RawAutocomplete<T>)oldWidget).focusNode?.removeListener(this._updateOptionsViewVisibility);
            if ((((RawAutocomplete<T>)oldWidget).focusNode is null))
            {
                this._internalFocusNode?.dispose();
                _internalFocusNode = null;
            }
            ((RawAutocomplete<T>)this.widget).focusNode?.addListener(this._updateOptionsViewVisibility);
        }
    }

    public override void dispose()
    {
        ((RawAutocomplete<T>)this.widget).textEditingController?.removeListener(this._onChangedFieldListener);
        this._internalTextEditingController?.dispose();
        ((RawAutocomplete<T>)this.widget).focusNode?.removeListener(this._updateOptionsViewVisibility);
        this._internalFocusNode?.dispose();
        this._highlightedOptionIndex.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        Widget fieldView = ((((RawAutocomplete<T>)this.widget).fieldViewBuilder is null ? new SizedBox(width: double.PositiveInfinity, height: 0.0) : ((RawAutocomplete<T>)this.widget).fieldViewBuilder.Invoke(context, this._textEditingController, this._focusNode, this._onFieldSubmitted)));
        return ((Widget)OverlayPortal.CreateOverlayChildLayoutBuilder(controller: this._optionsViewController, overlayChildBuilder: this._buildOptionsView, child: new TextFieldTapRegion(child: new Shortcuts(shortcuts: _shortcuts, child: new Actions(actions: this._actionMap.cast<Type, dynamic>(), child: fieldView)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AutocompleteCallbackAction__autocomplete<T> : CallbackAction<T> where T : Intent
{
    public virtual global::System.Func<bool> isEnabledCallback { get; private set; } = default!;

    internal _AutocompleteCallbackAction__autocomplete(global::System.Func<T, object?> onInvoke, global::System.Func<bool> isEnabledCallback) : base(onInvoke: onInvoke)
    {
        this.isEnabledCallback = isEnabledCallback;
    }

    public override bool isEnabled(T intent, BuildContext? context = null) => this.isEnabledCallback();
    public override bool consumesKey(T intent) => isEnabled(intent);
}

public class AutocompletePreviousOptionIntent : Intent
{
    public AutocompletePreviousOptionIntent()
    {
    }

}

public class AutocompleteNextOptionIntent : Intent
{
    public AutocompleteNextOptionIntent()
    {
    }

}

public class AutocompleteFirstOptionIntent : Intent
{
    public AutocompleteFirstOptionIntent()
    {
    }

}

public class AutocompleteLastOptionIntent : Intent
{
    public AutocompleteLastOptionIntent()
    {
    }

}

public class AutocompleteNextPageOptionIntent : Intent
{
    public AutocompleteNextPageOptionIntent()
    {
    }

}

public class AutocompletePreviousPageOptionIntent : Intent
{
    public AutocompletePreviousPageOptionIntent()
    {
    }

}

public class AutocompleteHighlightedOption : InheritedNotifier<global::Doroti.Framework.Foundation.ValueNotifier<long>>
{
    public AutocompleteHighlightedOption(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Foundation.ValueNotifier<long> highlightIndexNotifier = default!, Widget child = default!) : base(key: key, child: child, notifier: highlightIndexNotifier)
    {
    }

    public static long of(BuildContext context)
    {
        return (context.dependOnInheritedWidgetOfExactType<AutocompleteHighlightedOption>()?.notifier?.value ?? 0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
