// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/autocomplete.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public delegate object AutocompleteOptionsBuilder<T>(TextEditingValue textEditingValue);

public delegate void AutocompleteOnSelected<T>(T option);

public delegate Widget AutocompleteOptionsViewBuilder<T>(
    BuildContext context,
    Action<T> onSelected,
    IEnumerable<T> options
);

public delegate Widget AutocompleteFieldViewBuilder(
    BuildContext context,
    TextEditingController textEditingController,
    FocusNode focusNode,
    Action onFieldSubmitted
);

public delegate string AutocompleteOptionToString<T>(T option);

public enum OptionsViewOpenDirection
{
    up,
    down,
    mostSpace,
}

public class RawAutocomplete<T> : StatefulWidget
{
    public virtual Func<
        BuildContext,
        TextEditingController,
        FocusNode,
        Action,
        Widget
    >? fieldViewBuilder { get; private set; }
    public virtual FocusNode? focusNode { get; private set; }
    public virtual Func<BuildContext, Action<T>, IEnumerable<T>, Widget> optionsViewBuilder
    {
        get;
        private set;
    } = default!;
    public virtual OptionsViewOpenDirection optionsViewOpenDirection { get; private set; } =
        default!;
    public virtual Func<T, string> displayStringForOption { get; private set; } = default!;
    public virtual Action<T>? onSelected { get; private set; }
    public virtual Func<TextEditingValue, object> optionsBuilder { get; private set; } = default!;
    public virtual TextEditingController? textEditingController { get; private set; }
    public virtual TextEditingValue? initialValue { get; private set; }

    public RawAutocomplete(
        Key? key = null,
        Func<BuildContext, Action<T>, IEnumerable<T>, Widget> optionsViewBuilder = default!,
        Func<TextEditingValue, object> optionsBuilder = default!,
        OptionsViewOpenDirection optionsViewOpenDirection = OptionsViewOpenDirection.down,
        Func<T, string> displayStringForOption = default!,
        Func<BuildContext, TextEditingController, FocusNode, Action, Widget>? fieldViewBuilder =
            null,
        FocusNode? focusNode = null,
        Action<T>? onSelected = null,
        TextEditingController? textEditingController = null,
        TextEditingValue? initialValue = null
    )
        : base(key: key)
    {
        Func<T, string> __displayStringForOption =
            displayStringForOption
            ?? new Func<T, string>((__option) => defaultStringForOption(__option));
        this.optionsViewBuilder = optionsViewBuilder;
        this.optionsBuilder = optionsBuilder;
        this.optionsViewOpenDirection = optionsViewOpenDirection;
        this.displayStringForOption = __displayStringForOption;
        this.fieldViewBuilder = fieldViewBuilder;
        this.focusNode = focusNode;
        this.onSelected = onSelected;
        this.textEditingController = textEditingController;
        this.initialValue = initialValue;
        System.Diagnostics.Debug.Assert(
            (fieldViewBuilder is not null)
                || (
                    (key is not null)
                    && (focusNode is not null)
                    && (textEditingController is not null)
                )
        );
        System.Diagnostics.Debug.Assert((focusNode is null) == (textEditingController is null));
        System.Diagnostics.Debug.Assert(
            !((textEditingController is not null) && (initialValue is not null))
        );
    }

    public static void onFieldSubmitted<TOption>(GlobalKey<IState> key)
    {
        var rawAutocomplete = ((_RawAutocompleteState__autocomplete<TOption>?)key.currentState!)!;
        rawAutocomplete._onFieldSubmitted();
    }

    public static string defaultStringForOption(object? option)
    {
        return option!.ToString()!;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _RawAutocompleteState__autocomplete<T>());
}

internal class _RawAutocompleteState__autocomplete<T> : State<RawAutocomplete<T>>
{
    internal virtual OverlayPortalController _optionsViewController { get; private set; } =
        new OverlayPortalController(debugLabel: "_RawAutocompleteState");
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
                __late__actionMap = new DartMap<Type, IIntentAction>
                {
                    [typeof(AutocompletePreviousOptionIntent)] =
                        new _AutocompleteCallbackAction__autocomplete<AutocompletePreviousOptionIntent>(
                            onInvoke: (__arg0) =>
                            {
                                (
                                    (Action<AutocompletePreviousOptionIntent>)
                                        _highlightPreviousOption
                                )(__arg0);
                                return default!;
                            },
                            isEnabledCallback: () => _canShowOptionsView
                        ),
                    [typeof(AutocompleteNextOptionIntent)] =
                        new _AutocompleteCallbackAction__autocomplete<AutocompleteNextOptionIntent>(
                            onInvoke: (__arg0) =>
                            {
                                ((Action<AutocompleteNextOptionIntent>)_highlightNextOption)(
                                    __arg0
                                );
                                return default!;
                            },
                            isEnabledCallback: () => _canShowOptionsView
                        ),
                    [typeof(AutocompleteFirstOptionIntent)] =
                        new _AutocompleteCallbackAction__autocomplete<AutocompleteFirstOptionIntent>(
                            onInvoke: (__arg0) =>
                            {
                                ((Action<AutocompleteFirstOptionIntent>)_highlightFirstOption)(
                                    __arg0
                                );
                                return default!;
                            },
                            isEnabledCallback: () => _canShowOptionsView
                        ),
                    [typeof(AutocompleteLastOptionIntent)] =
                        new _AutocompleteCallbackAction__autocomplete<AutocompleteLastOptionIntent>(
                            onInvoke: (__arg0) =>
                            {
                                ((Action<AutocompleteLastOptionIntent>)_highlightLastOption)(
                                    __arg0
                                );
                                return default!;
                            },
                            isEnabledCallback: () => _canShowOptionsView
                        ),
                    [typeof(AutocompleteNextPageOptionIntent)] =
                        new _AutocompleteCallbackAction__autocomplete<AutocompleteNextPageOptionIntent>(
                            onInvoke: (__arg0) =>
                            {
                                (
                                    (Action<AutocompleteNextPageOptionIntent>)
                                        _highlightNextPageOption
                                )(__arg0);
                                return default!;
                            },
                            isEnabledCallback: () => _canShowOptionsView
                        ),
                    [typeof(AutocompletePreviousPageOptionIntent)] =
                        new _AutocompleteCallbackAction__autocomplete<AutocompletePreviousPageOptionIntent>(
                            onInvoke: (__arg0) =>
                            {
                                (
                                    (Action<AutocompletePreviousPageOptionIntent>)
                                        _highlightPreviousPageOption
                                )(__arg0);
                                return default!;
                            },
                            isEnabledCallback: () => _canShowOptionsView
                        ),
                    [typeof(DismissIntent)] = new CallbackAction<DismissIntent>(
                        onInvoke: _hideOptions
                    ),
                };
                __late__actionMap_initialized = true;
            }
            return __late__actionMap;
        }
    }
    internal virtual IEnumerable<T> _options { get; set; } = Enumerable.Empty<T>();
    internal virtual T? _selection { get; set; } = default;
    internal virtual string? _lastFieldText { get; set; } = default;
    internal virtual ValueNotifier<long> _highlightedOptionIndex { get; private set; } =
        new ValueNotifier<long>(0L);
    internal static DartMap<ShortcutActivator, Intent> _appleShortcuts = new DartMap<
        ShortcutActivator,
        Intent
    >
    {
        [new SingleActivator(LogicalKeyboardKey.arrowUp, meta: true)] =
            new AutocompleteFirstOptionIntent(),
        [new SingleActivator(LogicalKeyboardKey.arrowDown, meta: true)] =
            new AutocompleteLastOptionIntent(),
    };
    internal static DartMap<ShortcutActivator, Intent> _nonAppleShortcuts = new DartMap<
        ShortcutActivator,
        Intent
    >
    {
        [new SingleActivator(LogicalKeyboardKey.arrowUp, control: true)] =
            new AutocompleteFirstOptionIntent(),
        [new SingleActivator(LogicalKeyboardKey.arrowDown, control: true)] =
            new AutocompleteLastOptionIntent(),
    };
    internal static DartMap<ShortcutActivator, Intent> _commonShortcuts = new DartMap<
        ShortcutActivator,
        Intent
    >
    {
        [new SingleActivator(LogicalKeyboardKey.arrowUp)] = new AutocompletePreviousOptionIntent(),
        [new SingleActivator(LogicalKeyboardKey.arrowDown)] = new AutocompleteNextOptionIntent(),
        [new SingleActivator(LogicalKeyboardKey.pageUp)] =
            new AutocompletePreviousPageOptionIntent(),
        [new SingleActivator(LogicalKeyboardKey.pageDown)] = new AutocompleteNextPageOptionIntent(),
    };
    internal virtual long _onChangedCallId { get; set; } = 0L;
    internal static double _kMinUsableHeight = ConstantsLibrary.kMinInteractiveDimension;

    internal virtual TextEditingController _textEditingController
    {
        get
        {
            return widget.textEditingController
                ?? (
                    _internalTextEditingController ??= (
                        (Func<TextEditingController>)(
                            () =>
                            {
                                var __cascade = new TextEditingController();
                                __cascade.addListener(_onChangedFieldListener);
                                return __cascade;
                            }
                        )
                    )()
                );
        }
    }
    internal virtual FocusNode _focusNode
    {
        get
        {
            return widget.focusNode
                ?? (
                    _internalFocusNode ??= (
                        (Func<FocusNode>)(
                            () =>
                            {
                                var __cascade = new FocusNode();
                                __cascade.addListener(_onFocusChange);
                                return __cascade;
                            }
                        )
                    )()
                );
        }
    }
    internal static DartMap<ShortcutActivator, Intent> _shortcuts =>
        new DartMap<ShortcutActivator, Intent>();
    internal virtual bool _canShowOptionsView =>
        DartRuntimePrimitives.ConvertValue<bool>(_focusNode.hasFocus && Enumerable.Any(_options));

    internal virtual void _onFocusChange()
    {
        if (_focusNode.hasFocus != _hasFocus)
        {
            _hasFocus = _focusNode.hasFocus;
            _updateOptionsViewVisibility();
        }
    }

    internal virtual void _updateOptionsViewVisibility()
    {
        if (_canShowOptionsView)
        {
            _optionsViewController.show();
        }
        else
        {
            if (_optionsViewController.isShowing)
            {
                _optionsViewController.hide();
            }
        }
    }

    internal virtual void _announceSemantics(bool resultsAvailable)
    {
        if (!MediaQuery.supportsAnnounceOf(context))
        {
            return;
        }
        WidgetsLocalizations localizations = WidgetsLocalizations.of(context);
        string optionsHint = resultsAvailable
            ? localizations.searchResultsFound
            : localizations.noResultsFound;
        DartRuntimePrimitives.Ignore(
            SemanticsService
                .sendAnnouncement(View.of(context), optionsHint, localizations.textDirection)
                .catchError(
                    (exception, stack) =>
                    {
                        FlutterError.reportError(
                            new FlutterErrorDetails(
                                exception: exception,
                                stack: stack,
                                library: "widgets library",
                                context: new ErrorDescription(
                                    "while sending semantics announcement"
                                )
                            )
                        );
                    }
                )
        );
    }

    private void _onChangedFieldListener() => _ = _onChangedField();

    internal virtual async Future _onChangedField()
    {
        if (_selecting)
        {
            return;
        }
        TextEditingValue valueLocal = _textEditingController.value;
        var shouldUpdateOptions = false;
        if (valueLocal.text != _lastFieldText)
        {
            shouldUpdateOptions = true;
            _onChangedCallId += 1L;
        }
        _lastFieldText = valueLocal.text;
        long callId = _onChangedCallId;
        IEnumerable<T> options = await DartAsyncRuntime.AwaitFutureOrValue<IEnumerable<T>>(
            widget.optionsBuilder(valueLocal)
        );
        if (!mounted)
        {
            return;
        }
        if ((callId != _onChangedCallId) || !shouldUpdateOptions)
        {
            return;
        }
        if (!Enumerable.Any(_options) != !Enumerable.Any(options))
        {
            _announceSemantics(Enumerable.Any(options));
        }
        _options = options;
        _updateHighlight(_highlightedOptionIndex.value);
        T? selection = _selection;
        if (
            (selection is not null) && (valueLocal.text != widget.displayStringForOption(selection))
        )
        {
            _selection = default(T);
        }
        _updateOptionsViewVisibility();
    }

    internal virtual void _onFieldSubmitted()
    {
        if (_optionsViewController.isShowing)
        {
            _select(_options.elementAt(_highlightedOptionIndex.value));
        }
    }

    internal virtual void _select(T nextSelection)
    {
        if (EqualityComparer<T>.Default.Equals(nextSelection, _selection))
        {
            return;
        }
        _selecting = true;
        _selection = nextSelection;
        string selectionString = widget.displayStringForOption(nextSelection);
        _textEditingController.value = new TextEditingValue(
            selection: TextSelection.CreateCollapsed(offset: selectionString.Length),
            text: selectionString
        );
        _lastFieldText = selectionString;
        widget.onSelected?.Invoke(nextSelection);
        if (_optionsViewController.isShowing)
        {
            _optionsViewController.hide();
        }
        _selecting = false;
    }

    internal virtual void _updateHighlight(long nextIndex)
    {
        _highlightedOptionIndex.value = !Enumerable.Any(_options)
            ? 0L
            : nextIndex.clamp(0L, _options.Count() - 1L);
    }

    internal virtual void _highlightPreviousOption(AutocompletePreviousOptionIntent intent)
    {
        _highlightOption(_highlightedOptionIndex.value - 1L);
    }

    internal virtual void _highlightNextOption(AutocompleteNextOptionIntent intent)
    {
        _highlightOption(_highlightedOptionIndex.value + 1L);
    }

    internal virtual void _highlightFirstOption(AutocompleteFirstOptionIntent intent)
    {
        _highlightOption(0L);
    }

    internal virtual void _highlightLastOption(AutocompleteLastOptionIntent intent)
    {
        _highlightOption(_options.Count() - 1L);
    }

    internal virtual void _highlightNextPageOption(AutocompleteNextPageOptionIntent intent)
    {
        _highlightOption(_highlightedOptionIndex.value + _pageSize);
    }

    internal virtual void _highlightPreviousPageOption(AutocompletePreviousPageOptionIntent intent)
    {
        _highlightOption(_highlightedOptionIndex.value - _pageSize);
    }

    internal virtual void _highlightOption(long index)
    {
        DartRuntimePrimitives.Assert(() => _canShowOptionsView);
        _updateOptionsViewVisibility();
        DartRuntimePrimitives.Assert(() => _optionsViewController.isShowing);
        _updateHighlight(index);
    }

    internal virtual object? _hideOptions(DismissIntent intent)
    {
        if (_optionsViewController.isShowing)
        {
            _optionsViewController.hide();
            return null;
        }
        else
        {
            return Actions.invoke(context, intent);
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildOptionsView(
        BuildContext context,
        OverlayChildLayoutInfo layoutInfo
    )
    {
        if (layoutInfo.childPaintTransform.determinant == 0.0)
        {
            return SizedBox.CreateShrink();
        }
        Size fieldSize = layoutInfo.childSize;
        Matrix4 invertTransform = (
            (Func<Matrix4>)(
                () =>
                {
                    var __cascade = layoutInfo.childPaintTransform.clone();
                    __cascade.invert();
                    return __cascade;
                }
            )
        )();
        EdgeInsets mediaQueryPadding = MediaQuery.paddingOf(context);
        EdgeInsets viewInsets = MediaQuery.viewInsetsOf(context);
        Rect overlayRect = mediaQueryPadding.deflateRect(
            viewInsets.deflateRect(Offset.zero & layoutInfo.overlaySize)
        );
        Rect overlayRectInField = MatrixUtils.transformRect(invertTransform, overlayRect);
        double spaceAbove = -overlayRectInField.top;
        double spaceBelow = overlayRectInField.bottom - fieldSize.height;
        bool opensUp = widget.optionsViewOpenDirection switch
        {
            OptionsViewOpenDirection.up => true,
            OptionsViewOpenDirection.down => false,
            OptionsViewOpenDirection.mostSpace => spaceAbove > spaceBelow,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        double optionsViewMaxHeight = opensUp
            ? -overlayRectInField.top
            : (overlayRectInField.bottom - fieldSize.height);
        var optionsViewBoundingBox = new Size(
            fieldSize.width,
            Math.Max(optionsViewMaxHeight, _kMinUsableHeight)
        );
        double originY = opensUp
            ? overlayRectInField.top
            : (overlayRectInField.bottom - optionsViewBoundingBox.height);
        Matrix4 transformLocal = (
            (Func<Matrix4>)(
                () =>
                {
                    var __cascade = layoutInfo.childPaintTransform.clone();
                    __cascade.translateByDouble(0.0, originY, 0, 1);
                    return __cascade;
                }
            )
        )();
        Widget childLocal = new Builder(
            builder: (context) => widget.optionsViewBuilder(context, _select, _options)
        );
        return new Transform(
            transform: transformLocal,
            child: new Align(
                alignment: Alignment.topLeft,
                child: new ConstrainedBox(
                    constraints: BoxConstraints.CreateTight(optionsViewBoundingBox),
                    child: new Align(
                        alignment: opensUp
                            ? AlignmentDirectional.bottomStart
                            : AlignmentDirectional.topStart,
                        child: new TextFieldTapRegion(
                            child: new AutocompleteHighlightedOption(
                                highlightIndexNotifier: _highlightedOptionIndex,
                                child: new ExcludeFocus(child: childLocal)
                            )
                        )
                    )
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void initState()
    {
        base.initState();
        TextEditingController initialController =
            widget.textEditingController
            ?? (
                _internalTextEditingController = TextEditingController.CreateFromValue(
                    widget.initialValue
                )
            );
        initialController.addListener(_onChangedFieldListener);
        _hasFocus = _focusNode.hasFocus;
        widget.focusNode?.addListener(_onFocusChange);
    }

    public override void didUpdateWidget(RawAutocomplete<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (
            !DartRuntimePrimitives.Identical(
                oldWidget.textEditingController,
                widget.textEditingController
            )
        )
        {
            oldWidget.textEditingController?.removeListener(_onChangedFieldListener);
            if (oldWidget.textEditingController is null)
            {
                _internalTextEditingController?.dispose();
                _internalTextEditingController = null;
            }
            widget.textEditingController?.addListener(_onChangedFieldListener);
        }
        if (!DartRuntimePrimitives.Identical(oldWidget.focusNode, widget.focusNode))
        {
            oldWidget.focusNode?.removeListener(_updateOptionsViewVisibility);
            if (oldWidget.focusNode is null)
            {
                _internalFocusNode?.dispose();
                _internalFocusNode = null;
            }
            widget.focusNode?.addListener(_updateOptionsViewVisibility);
        }
    }

    public override void dispose()
    {
        widget.textEditingController?.removeListener(_onChangedFieldListener);
        _internalTextEditingController?.dispose();
        widget.focusNode?.removeListener(_updateOptionsViewVisibility);
        _internalFocusNode?.dispose();
        _highlightedOptionIndex.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        Widget fieldView = widget.fieldViewBuilder is null
            ? new SizedBox(width: double.PositiveInfinity, height: 0.0)
            : widget.fieldViewBuilder.Invoke(
                context,
                _textEditingController,
                _focusNode,
                _onFieldSubmitted
            );
        return OverlayPortal.CreateOverlayChildLayoutBuilder(
            controller: _optionsViewController,
            overlayChildBuilder: _buildOptionsView,
            child: new TextFieldTapRegion(
                child: new Shortcuts(
                    shortcuts: _shortcuts,
                    child: new Actions(actions: _actionMap.cast<Type, dynamic>(), child: fieldView)
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _AutocompleteCallbackAction__autocomplete<T> : CallbackAction<T>
    where T : Intent
{
    public virtual Func<bool> isEnabledCallback { get; private set; } = default!;

    internal _AutocompleteCallbackAction__autocomplete(
        Func<T, object?> onInvoke,
        Func<bool> isEnabledCallback
    )
        : base(onInvoke: onInvoke)
    {
        this.isEnabledCallback = isEnabledCallback;
    }

    public override bool isEnabled(T intent, BuildContext? context = null) => isEnabledCallback();

    public override bool consumesKey(T intent) => isEnabled(intent);
}

public class AutocompletePreviousOptionIntent : Intent
{
    public AutocompletePreviousOptionIntent() { }
}

public class AutocompleteNextOptionIntent : Intent
{
    public AutocompleteNextOptionIntent() { }
}

public class AutocompleteFirstOptionIntent : Intent
{
    public AutocompleteFirstOptionIntent() { }
}

public class AutocompleteLastOptionIntent : Intent
{
    public AutocompleteLastOptionIntent() { }
}

public class AutocompleteNextPageOptionIntent : Intent
{
    public AutocompleteNextPageOptionIntent() { }
}

public class AutocompletePreviousPageOptionIntent : Intent
{
    public AutocompletePreviousPageOptionIntent() { }
}

public class AutocompleteHighlightedOption : InheritedNotifier<ValueNotifier<long>>
{
    public AutocompleteHighlightedOption(
        Key? key = null,
        ValueNotifier<long> highlightIndexNotifier = default!,
        Widget child = default!
    )
        : base(key: key, child: child, notifier: highlightIndexNotifier) { }

    public static long of(BuildContext context)
    {
        return context
                .dependOnInheritedWidgetOfExactType<AutocompleteHighlightedOption>()
                ?.notifier?.value
            ?? 0L;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
