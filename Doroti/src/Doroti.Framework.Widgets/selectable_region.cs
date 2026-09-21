// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/selectable_region.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public static partial class Selectable_regionLibrary
{
    internal static HashSet<PointerDeviceKind> _kLongPressSelectionDevices =
        new HashSet<PointerDeviceKind>
        {
            PointerDeviceKind.touch,
            PointerDeviceKind.stylus,
            PointerDeviceKind.invertedStylus,
        };
}

public static partial class Selectable_regionLibrary
{
    internal static double _kSelectableVerticalComparingThreshold = 3.0;
}

public class SelectableRegion : StatefulWidget
{
    public virtual TextMagnifierConfiguration magnifierConfiguration { get; private set; } =
        default!;
    public virtual FocusNode? focusNode { get; private set; }
    public virtual Widget child { get; private set; } = default!;
    public virtual Func<BuildContext, SelectableRegionState, Widget>? contextMenuBuilder
    {
        get;
        private set;
    }
    public virtual TextSelectionControls selectionControls { get; private set; } = default!;
    public virtual Action<SelectedContent?>? onSelectionChanged { get; private set; }

    public SelectableRegion(
        Key? key = null,
        Func<BuildContext, SelectableRegionState, Widget>? contextMenuBuilder = null,
        FocusNode? focusNode = null,
        TextMagnifierConfiguration magnifierConfiguration = default!,
        Action<SelectedContent?>? onSelectionChanged = null,
        TextSelectionControls selectionControls = default!,
        Widget child = default!
    )
        : base(key: key)
    {
        TextMagnifierConfiguration __magnifierConfiguration =
            magnifierConfiguration ?? TextMagnifierConfiguration.disabled;
        this.contextMenuBuilder = contextMenuBuilder;
        this.focusNode = focusNode;
        this.magnifierConfiguration = __magnifierConfiguration;
        this.onSelectionChanged = onSelectionChanged;
        this.selectionControls = selectionControls;
        this.child = child;
    }

    public static List<ContextMenuButtonItem> getSelectableButtonItems(
        SelectionGeometry selectionGeometry,
        Action onCopy,
        Action onSelectAll,
        Action? onShare
    )
    {
        var canCopy = Equals(selectionGeometry.status, SelectionStatus.uncollapsed);
        bool canSelectAll = selectionGeometry.hasContent;
        bool platformCanShare =
            !Foundation.ConstantsLibrary.kIsWeb
            && (
                PlatformLibrary.defaultTargetPlatform switch
                {
                    TargetPlatform.android => Equals(
                        selectionGeometry.status,
                        SelectionStatus.uncollapsed
                    ),
                    TargetPlatform.macOS or TargetPlatform.fuchsia or TargetPlatform.linux => false,
                    TargetPlatform.windows => false,
                    TargetPlatform.iOS => false,
                    _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
                }
            );
        bool canShare = (onShare is not null) && platformCanShare;
        var showShareBeforeSelectAll = Equals(
            PlatformLibrary.defaultTargetPlatform,
            TargetPlatform.android
        );
        return new List<ContextMenuButtonItem>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new SelectableRegionState());
}

public class SelectableRegionState
    : State<SelectableRegion>,
        TextSelectionDelegate,
        SelectionRegistrar
{
    private bool __late__actions_initialized;
    private DartMap<Type, dynamic> __late__actions = default!;
    internal virtual DartMap<Type, dynamic> _actions
    {
        get
        {
            if (!__late__actions_initialized)
            {
                __late__actions = new DartMap<Type, dynamic>
                {
                    [typeof(SelectAllTextIntent)] = _makeOverridable(
                        new _SelectAllAction__selectable_region(this)
                    ),
                    [typeof(CopySelectionTextIntent)] = _makeOverridable(
                        new _CopySelectionAction__selectable_region(this)
                    ),
                    [typeof(ExtendSelectionToNextWordBoundaryOrCaretLocationIntent)] =
                        _makeOverridable(
                            new _GranularlyExtendSelectionAction__selectable_region<ExtendSelectionToNextWordBoundaryOrCaretLocationIntent>(
                                this,
                                granularity: TextGranularity.word
                            )
                        ),
                    [typeof(ExpandSelectionToDocumentBoundaryIntent)] = _makeOverridable(
                        new _GranularlyExtendSelectionAction__selectable_region<ExpandSelectionToDocumentBoundaryIntent>(
                            this,
                            granularity: TextGranularity.document
                        )
                    ),
                    [typeof(ExpandSelectionToLineBreakIntent)] = _makeOverridable(
                        new _GranularlyExtendSelectionAction__selectable_region<ExpandSelectionToLineBreakIntent>(
                            this,
                            granularity: TextGranularity.line
                        )
                    ),
                    [typeof(ExtendSelectionByCharacterIntent)] = _makeOverridable(
                        new _GranularlyExtendCaretSelectionAction__selectable_region<ExtendSelectionByCharacterIntent>(
                            this,
                            granularity: TextGranularity.character
                        )
                    ),
                    [typeof(ExtendSelectionToNextWordBoundaryIntent)] = _makeOverridable(
                        new _GranularlyExtendCaretSelectionAction__selectable_region<ExtendSelectionToNextWordBoundaryIntent>(
                            this,
                            granularity: TextGranularity.word
                        )
                    ),
                    [typeof(ExtendSelectionToLineBreakIntent)] = _makeOverridable(
                        new _GranularlyExtendCaretSelectionAction__selectable_region<ExtendSelectionToLineBreakIntent>(
                            this,
                            granularity: TextGranularity.line
                        )
                    ),
                    [typeof(ExtendSelectionVerticallyToAdjacentLineIntent)] = _makeOverridable(
                        new _DirectionallyExtendCaretSelectionAction__selectable_region<ExtendSelectionVerticallyToAdjacentLineIntent>(
                            this
                        )
                    ),
                    [typeof(ExtendSelectionToDocumentBoundaryIntent)] = _makeOverridable(
                        new _GranularlyExtendCaretSelectionAction__selectable_region<ExtendSelectionToDocumentBoundaryIntent>(
                            this,
                            granularity: TextGranularity.document
                        )
                    ),
                    [typeof(DismissIntent)] = new CallbackAction<DismissIntent>(
                        onInvoke: _hideToolbarIfVisible
                    ),
                };
                __late__actions_initialized = true;
            }
            return __late__actions;
        }
    }
    internal virtual DartMap<Type, dynamic> _gestureRecognizers { get; private set; } =
        new DartMap<Type, dynamic>();
    internal virtual SelectionOverlay? _selectionOverlay { get; set; } = default;
    internal virtual LayerLink _startHandleLayerLink { get; private set; } = new LayerLink();
    internal virtual LayerLink _endHandleLayerLink { get; private set; } = new LayerLink();
    internal virtual LayerLink _toolbarLayerLink { get; private set; } = new LayerLink();
    internal virtual StaticSelectionContainerDelegate _selectionDelegate { get; private set; } =
        new StaticSelectionContainerDelegate();
    internal virtual Selectable? _selectable { get; set; } = default;
    internal virtual Orientation? _lastOrientation { get; set; } = default;
    internal virtual SelectedContent? _lastSelectedContent { get; set; } = default;
    internal virtual ProcessTextService _processTextService { get; private set; } =
        new DefaultProcessTextService();
    internal virtual List<ProcessTextAction> _processTextActions { get; private set; } =
        new List<ProcessTextAction>();
    internal virtual FocusNode? _localFocusNode { get; set; } = default;
    internal virtual _SelectableRegionSelectionStatusNotifier__selectable_region _selectionStatusNotifier
    {
        get;
        private set;
    } = new _SelectableRegionSelectionStatusNotifier__selectable_region();
    internal virtual bool _isShiftPressed { get; set; } = false;
    internal virtual Offset? _lastSecondaryTapDownPosition { get; set; } = default;
    internal virtual PointerDeviceKind? _lastPointerDeviceKind { get; set; } = default;
    internal virtual Offset? _doubleTapOffset { get; set; } = default;
    internal virtual Offset? _selectionEndPosition { get; set; } = default;
    internal virtual bool _scheduledSelectionEndEdgeUpdate { get; set; } = false;
    internal virtual Offset? _selectionStartPosition { get; set; } = default;
    internal virtual bool _scheduledSelectionStartEdgeUpdate { get; set; } = false;
    internal virtual Offset _selectionStartHandleDragPosition { get; set; } = default!;
    internal virtual Offset _selectionEndHandleDragPosition { get; set; } = default!;
    internal virtual bool? _adjustingSelectionEnd { get; set; } = default;
    internal virtual double? _directionalHorizontalBaseline { get; set; } = default;
    public virtual TextEditingValue textEditingValue { get; set; } =
        new TextEditingValue(text: "_");

    internal virtual bool _hasSelectionOverlayGeometry =>
        DartRuntimePrimitives.ConvertValue<bool>(
            (_selectionDelegate.value.startSelectionPoint is not null)
                || (_selectionDelegate.value.endSelectionPoint is not null)
        );
    internal virtual bool _webContextMenuEnabled =>
        DartRuntimePrimitives.ConvertValue<bool>(
            Foundation.ConstantsLibrary.kIsWeb
                && BrowserContextMenu.enabled
                && (!Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.android))
                && (!Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS))
        );
    public virtual SelectionOverlay? selectionOverlay => _selectionOverlay;
    internal virtual FocusNode _focusNode =>
        DartRuntimePrimitives.ConvertValue<FocusNode>(
            widget.focusNode ?? (_localFocusNode ??= new FocusNode(debugLabel: "SelectableRegion"))
        );

    public override void initState()
    {
        base.initState();
        _focusNode.addListener(_handleFocusChanged);
        _initMouseGestureRecognizer();
        _initTouchGestureRecognizer();
        _gestureRecognizers[typeof(TapGestureRecognizer)] =
            new GestureRecognizerFactoryWithHandlers<TapGestureRecognizer>(
                () => new TapGestureRecognizer(debugOwner: this),
                (instance) =>
                {
                    instance.onSecondaryTapDown = _handleRightClickDown;
                }
            );
        DartRuntimePrimitives.Ignore(_initProcessTextActions());
    }

    internal virtual async Future _initProcessTextActions()
    {
        _processTextActions.Clear();
        _processTextActions.AddRange(
            (await _processTextService.queryTextActions()).Cast<ProcessTextAction>()
        );
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.android:
            case TargetPlatform.iOS:
            {
                break;
            }
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.macOS:
            case TargetPlatform.windows:
            {
                return;
            }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        Orientation orientation = MediaQuery.orientationOf(context);
        if (_lastOrientation is null)
        {
            _lastOrientation = orientation;
            return;
        }
        if (!Equals(orientation, _lastOrientation))
        {
            _lastOrientation = orientation;
            hideToolbar(Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.android));
        }
    }

    public override void didUpdateWidget(SelectableRegion oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.focusNode, oldWidget.focusNode))
        {
            if ((oldWidget.focusNode is null) && (widget.focusNode is not null))
            {
                _localFocusNode?.removeListener(_handleFocusChanged);
                _localFocusNode?.dispose();
                _localFocusNode = null;
            }
            else
            {
                if ((widget.focusNode is null) && (oldWidget.focusNode is not null))
                {
                    oldWidget.focusNode!.removeListener(_handleFocusChanged);
                }
            }
            _focusNode.addListener(_handleFocusChanged);
            if (_focusNode.hasFocus != oldWidget.focusNode?.hasFocus)
            {
                _handleFocusChanged();
            }
        }
    }

    internal virtual IntentAction<T> _makeOverridable<T>(IntentAction<T> defaultAction)
        where T : Intent
    {
        return IntentAction<T>.CreateOverridable(context: context, defaultAction: defaultAction);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleFocusChanged()
    {
        if (!_focusNode.hasFocus)
        {
            if (Foundation.ConstantsLibrary.kIsWeb)
            {
                PlatformSelectableRegionContextMenuIo.detach(_selectionDelegate);
            }
            if (
                Equals(
                    Scheduler.SchedulerBinding.instance.lifecycleState,
                    AppLifecycleState.resumed
                )
            )
            {
                clearSelection();
                _selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                _finalizeSelectableRegionStatus();
            }
        }
        else
        {
            if (_webContextMenuEnabled)
            {
                PlatformSelectableRegionContextMenuIo.attach(_selectionDelegate);
            }
        }
    }

    internal virtual void _updateSelectionStatus()
    {
        SelectionGeometry geometry = _selectionDelegate.value;
        TextSelection selectionLocal = geometry.status switch
        {
            SelectionStatus.uncollapsed => new TextSelection(baseOffset: 0L, extentOffset: 1L),
            SelectionStatus.collapsed => new TextSelection(baseOffset: 0L, extentOffset: 1L),
            SelectionStatus.none => TextSelection.CreateCollapsed(offset: 1L),
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        textEditingValue = new TextEditingValue(text: "__", selection: selectionLocal);
        if (_hasSelectionOverlayGeometry)
        {
            _updateSelectionOverlay();
        }
        else
        {
            _selectionOverlay?.dispose();
            _selectionOverlay = null;
        }
    }

    internal static bool _isPrecisePointerDevice(PointerDeviceKind pointerDeviceKind)
    {
        switch (pointerDeviceKind)
        {
            case PointerDeviceKind.mouse:
            {
                return true;
            }
            case PointerDeviceKind.trackpad:
            case PointerDeviceKind.stylus:
            case PointerDeviceKind.invertedStylus:
            case PointerDeviceKind.touch:
            case PointerDeviceKind.unknown:
            {
                return false;
            }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _finalizeSelectableRegionStatus()
    {
        if (!Equals(_selectionStatusNotifier.value, SelectableRegionSelectionStatus.changing))
        {
            return;
        }
        _selectionStatusNotifier.value = SelectableRegionSelectionStatus.finalized;
    }

    internal virtual long _getEffectiveConsecutiveTapCount(long rawCount)
    {
        var maxConsecutiveTap = 3L;
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            {
                if (
                    (_lastPointerDeviceKind is not null)
                    && (!Equals(_lastPointerDeviceKind, PointerDeviceKind.mouse))
                )
                {
                    maxConsecutiveTap = 2L;
                }
                return (rawCount <= maxConsecutiveTap)
                    ? rawCount
                    : (
                        ((rawCount % maxConsecutiveTap) == 0L)
                            ? maxConsecutiveTap
                            : (rawCount % maxConsecutiveTap)
                    );
            }
            case TargetPlatform.linux:
            {
                return (rawCount <= maxConsecutiveTap)
                    ? rawCount
                    : (
                        ((rawCount % maxConsecutiveTap) == 0L)
                            ? maxConsecutiveTap
                            : (rawCount % maxConsecutiveTap)
                    );
            }
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
            case TargetPlatform.windows:
            {
                return Math.Min(rawCount, maxConsecutiveTap);
            }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _initMouseGestureRecognizer()
    {
        _gestureRecognizers[typeof(TapAndPanGestureRecognizer)] =
            new GestureRecognizerFactoryWithHandlers<TapAndPanGestureRecognizer>(
                () =>
                    new TapAndPanGestureRecognizer(
                        debugOwner: this,
                        supportedDevices: new HashSet<PointerDeviceKind> { PointerDeviceKind.mouse }
                    ),
                (instance) =>
                {
                    DartRuntimePrimitives.Ignore(
                        (
                            (Func<TapAndPanGestureRecognizer>)(
                                () =>
                                {
                                    var __cascade = instance;
                                    __cascade.onTapTrackStart = _onTapTrackStart;
                                    __cascade.onTapTrackReset = _onTapTrackReset;
                                    __cascade.onTapDown = _startNewMouseSelectionGesture;
                                    __cascade.onTapUp = _handleMouseTapUp;
                                    __cascade.onDragStart = _handleMouseDragStart;
                                    __cascade.onDragUpdate = _handleMouseDragUpdate;
                                    __cascade.onDragEnd = _handleMouseDragEnd;
                                    __cascade.onCancel = clearSelection;
                                    __cascade.dragStartBehavior = DragStartBehavior.down;
                                    return __cascade;
                                }
                            )
                        )()
                    );
                }
            );
    }

    internal virtual void _onTapTrackStart()
    {
        _isShiftPressed = Enumerable.Any(
            HardwareKeyboard.instance.logicalKeysPressed.intersection(
                new HashSet<LogicalKeyboardKey>
                {
                    LogicalKeyboardKey.shiftLeft,
                    LogicalKeyboardKey.shiftRight,
                }
            )
        );
    }

    internal virtual void _onTapTrackReset()
    {
        _isShiftPressed = false;
    }

    internal virtual void _initTouchGestureRecognizer()
    {
        _gestureRecognizers[typeof(TapAndHorizontalDragGestureRecognizer)] =
            new GestureRecognizerFactoryWithHandlers<TapAndHorizontalDragGestureRecognizer>(
                () =>
                    new TapAndHorizontalDragGestureRecognizer(
                        debugOwner: this,
                        supportedDevices: Enum.GetValues<PointerDeviceKind>()
                            .ToList()
                            .where(
                                (device) =>
                                {
                                    return !Equals(device, PointerDeviceKind.mouse);
                                    throw new InvalidOperationException(
                                        "Dart closure completed without a value."
                                    );
                                }
                            )
                            .toSet()
                    ),
                (instance) =>
                {
                    DartRuntimePrimitives.Ignore(
                        (
                            (Func<TapAndHorizontalDragGestureRecognizer>)(
                                () =>
                                {
                                    var __cascade = instance;
                                    __cascade.eagerVictoryOnDrag = !Equals(
                                        PlatformLibrary.defaultTargetPlatform,
                                        TargetPlatform.iOS
                                    );
                                    __cascade.onTapDown = _startNewMouseSelectionGesture;
                                    __cascade.onTapUp = _handleMouseTapUp;
                                    __cascade.onDragStart = _handleMouseDragStart;
                                    __cascade.onDragUpdate = _handleMouseDragUpdate;
                                    __cascade.onDragEnd = _handleMouseDragEnd;
                                    __cascade.onCancel = clearSelection;
                                    __cascade.dragStartBehavior = DragStartBehavior.down;
                                    return __cascade;
                                }
                            )
                        )()
                    );
                }
            );
        _gestureRecognizers[typeof(LongPressGestureRecognizer)] =
            new GestureRecognizerFactoryWithHandlers<LongPressGestureRecognizer>(
                () =>
                    new LongPressGestureRecognizer(
                        debugOwner: this,
                        supportedDevices: Selectable_regionLibrary._kLongPressSelectionDevices
                    ),
                (instance) =>
                {
                    DartRuntimePrimitives.Ignore(
                        (
                            (Func<LongPressGestureRecognizer>)(
                                () =>
                                {
                                    var __cascade = instance;
                                    __cascade.onLongPressStart = _handleTouchLongPressStart;
                                    __cascade.onLongPressMoveUpdate =
                                        _handleTouchLongPressMoveUpdate;
                                    __cascade.onLongPressEnd = _handleTouchLongPressEnd;
                                    return __cascade;
                                }
                            )
                        )()
                    );
                }
            );
    }

    internal virtual void _startNewMouseSelectionGesture(TapDragDownDetails details)
    {
        _lastPointerDeviceKind = details.kind;
        switch (_getEffectiveConsecutiveTapCount(details.consecutiveTapCount))
        {
            case 1L:
            {
                _focusNode.requestFocus();
                switch (PlatformLibrary.defaultTargetPlatform)
                {
                    case TargetPlatform.android:
                    case TargetPlatform.fuchsia:
                    case TargetPlatform.iOS:
                    {
                        break;
                    }
                    case TargetPlatform.macOS:
                    case TargetPlatform.linux:
                    case TargetPlatform.windows:
                    {
                        hideToolbar();
                        bool isShiftPressedValid =
                            _isShiftPressed
                            && (_selectionDelegate.value.startSelectionPoint is not null);
                        if (isShiftPressedValid)
                        {
                            _selectEndTo(offset: details.globalPosition);
                            _selectionStatusNotifier.value =
                                SelectableRegionSelectionStatus.changing;
                            break;
                        }
                        clearSelection();
                        _collapseSelectionAt(offset: details.globalPosition);
                        _selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                        break;
                    }
                }
                break;
            }
            case 2L:
            {
                switch (PlatformLibrary.defaultTargetPlatform)
                {
                    case TargetPlatform.iOS:
                    {
                        if (
                            Foundation.ConstantsLibrary.kIsWeb
                            && (details.kind is not null)
                            && !_isPrecisePointerDevice(
                                DartRuntimePrimitives.RequireValue(details.kind)
                            )
                        )
                        {
                            _doubleTapOffset = details.globalPosition;
                            break;
                        }
                        _selectWordAt(offset: details.globalPosition);
                        _selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                        if (
                            (details.kind is not null)
                            && !_isPrecisePointerDevice(
                                DartRuntimePrimitives.RequireValue(details.kind)
                            )
                        )
                        {
                            _showHandles();
                        }
                        break;
                    }
                    case TargetPlatform.android:
                    case TargetPlatform.fuchsia:
                    case TargetPlatform.macOS:
                    case TargetPlatform.linux:
                    case TargetPlatform.windows:
                    {
                        _selectWordAt(offset: details.globalPosition);
                        _selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                        break;
                    }
                }
                break;
            }
            case 3L:
            {
                switch (PlatformLibrary.defaultTargetPlatform)
                {
                    case TargetPlatform.android:
                    case TargetPlatform.fuchsia:
                    case TargetPlatform.iOS:
                    {
                        if (
                            (details.kind is not null)
                            && _isPrecisePointerDevice(
                                DartRuntimePrimitives.RequireValue(details.kind)
                            )
                        )
                        {
                            _selectParagraphAt(offset: details.globalPosition);
                            _selectionStatusNotifier.value =
                                SelectableRegionSelectionStatus.changing;
                        }
                        break;
                    }
                    case TargetPlatform.macOS:
                    case TargetPlatform.linux:
                    case TargetPlatform.windows:
                    {
                        _selectParagraphAt(offset: details.globalPosition);
                        _selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                        break;
                    }
                }
                break;
            }
        }
        _updateSelectedContentIfNeeded();
    }

    internal virtual void _handleMouseDragStart(TapDragStartDetails details)
    {
        switch (_getEffectiveConsecutiveTapCount(details.consecutiveTapCount))
        {
            case 1L:
            {
                if (
                    (details.kind is not null)
                    && !_isPrecisePointerDevice(DartRuntimePrimitives.RequireValue(details.kind))
                )
                {
                    return;
                }
                _selectStartTo(offset: details.globalPosition);
                _selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                break;
            }
        }
        _updateSelectedContentIfNeeded();
    }

    internal virtual void _handleMouseDragUpdate(TapDragUpdateDetails details)
    {
        switch (_getEffectiveConsecutiveTapCount(details.consecutiveTapCount))
        {
            case 1L:
            {
                if (
                    (details.kind is not null)
                    && !_isPrecisePointerDevice(DartRuntimePrimitives.RequireValue(details.kind))
                )
                {
                    return;
                }
                _selectEndTo(offset: details.globalPosition, continuous: true);
                _selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                break;
            }
            case 2L:
            {
                switch (PlatformLibrary.defaultTargetPlatform)
                {
                    case TargetPlatform.android:
                    case TargetPlatform.fuchsia:
                    {
                        if (
                            !Foundation.ConstantsLibrary.kIsWeb
                            || (
                                (details.kind is not null)
                                && _isPrecisePointerDevice(
                                    DartRuntimePrimitives.RequireValue(details.kind)
                                )
                            )
                        )
                        {
                            _selectEndTo(
                                offset: details.globalPosition,
                                continuous: true,
                                textGranularity: TextGranularity.word
                            );
                            _selectionStatusNotifier.value =
                                SelectableRegionSelectionStatus.changing;
                        }
                        break;
                    }
                    case TargetPlatform.iOS:
                    {
                        if (
                            Foundation.ConstantsLibrary.kIsWeb
                            && (details.kind is not null)
                            && !_isPrecisePointerDevice(
                                DartRuntimePrimitives.RequireValue(details.kind)
                            )
                            && (_doubleTapOffset is not null)
                        )
                        {
                            _selectWordAt(
                                offset: DartRuntimePrimitives.RequireValue(_doubleTapOffset)
                            );
                            _doubleTapOffset = null;
                        }
                        _selectEndTo(
                            offset: details.globalPosition,
                            continuous: true,
                            textGranularity: TextGranularity.word
                        );
                        _selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                        if (
                            (details.kind is not null)
                            && !_isPrecisePointerDevice(
                                DartRuntimePrimitives.RequireValue(details.kind)
                            )
                        )
                        {
                            _showHandles();
                        }
                        break;
                    }
                    case TargetPlatform.macOS:
                    case TargetPlatform.linux:
                    case TargetPlatform.windows:
                    {
                        _selectEndTo(
                            offset: details.globalPosition,
                            continuous: true,
                            textGranularity: TextGranularity.word
                        );
                        _selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                        break;
                    }
                }
                break;
            }
            case 3L:
            {
                switch (PlatformLibrary.defaultTargetPlatform)
                {
                    case TargetPlatform.android:
                    case TargetPlatform.fuchsia:
                    case TargetPlatform.iOS:
                    {
                        if (
                            (details.kind is not null)
                            && _isPrecisePointerDevice(
                                DartRuntimePrimitives.RequireValue(details.kind)
                            )
                        )
                        {
                            _selectEndTo(
                                offset: details.globalPosition,
                                continuous: true,
                                textGranularity: TextGranularity.paragraph
                            );
                            _selectionStatusNotifier.value =
                                SelectableRegionSelectionStatus.changing;
                        }
                        break;
                    }
                    case TargetPlatform.macOS:
                    case TargetPlatform.linux:
                    case TargetPlatform.windows:
                    {
                        _selectEndTo(
                            offset: details.globalPosition,
                            continuous: true,
                            textGranularity: TextGranularity.paragraph
                        );
                        _selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                        break;
                    }
                }
                break;
            }
        }
        _updateSelectedContentIfNeeded();
    }

    internal virtual void _handleMouseDragEnd(TapDragEndDetails details)
    {
        DartRuntimePrimitives.Assert(() => _lastPointerDeviceKind is not null);
        bool isPointerPrecise = _isPrecisePointerDevice(
            DartRuntimePrimitives.RequireValue(_lastPointerDeviceKind)
        );
        bool shouldShowSelectionOverlayOnMobile = !isPointerPrecise;
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            {
                if (shouldShowSelectionOverlayOnMobile)
                {
                    _showHandles();
                    _showToolbar();
                }
                break;
            }
            case TargetPlatform.iOS:
            {
                if (shouldShowSelectionOverlayOnMobile)
                {
                    _showToolbar();
                }
                break;
            }
            case TargetPlatform.macOS:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            {
                break;
            }
        }
        _finalizeSelection();
        _updateSelectedContentIfNeeded();
        _finalizeSelectableRegionStatus();
    }

    internal virtual void _handleMouseTapUp(TapDragUpDetails details)
    {
        if (
            Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS)
            && _positionIsOnActiveSelection(globalPosition: details.globalPosition)
        )
        {
            bool toolbarIsVisibleLocal = _selectionOverlay?.toolbarIsVisible ?? false;
            if (toolbarIsVisibleLocal)
            {
                hideToolbar(false);
            }
            else
            {
                _showToolbar();
            }
            return;
        }
        switch (_getEffectiveConsecutiveTapCount(details.consecutiveTapCount))
        {
            case 1L:
            {
                switch (PlatformLibrary.defaultTargetPlatform)
                {
                    case TargetPlatform.android:
                    case TargetPlatform.fuchsia:
                    case TargetPlatform.iOS:
                    {
                        hideToolbar();
                        _collapseSelectionAt(offset: details.globalPosition);
                        _selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                        break;
                    }
                    case TargetPlatform.macOS:
                    case TargetPlatform.linux:
                    case TargetPlatform.windows:
                        break;
                }
                break;
            }
            case 2L:
            {
                bool isPointerPrecise = _isPrecisePointerDevice(details.kind);
                switch (PlatformLibrary.defaultTargetPlatform)
                {
                    case TargetPlatform.android:
                    case TargetPlatform.fuchsia:
                    {
                        if (!isPointerPrecise)
                        {
                            _showHandles();
                            _showToolbar();
                        }
                        break;
                    }
                    case TargetPlatform.iOS:
                    {
                        if (!isPointerPrecise)
                        {
                            if (Foundation.ConstantsLibrary.kIsWeb)
                            {
                                break;
                            }
                            _showToolbar();
                        }
                        break;
                    }
                    case TargetPlatform.macOS:
                    case TargetPlatform.linux:
                    case TargetPlatform.windows:
                    {
                        break;
                    }
                }
                break;
            }
        }
        _finalizeSelectableRegionStatus();
        _updateSelectedContentIfNeeded();
    }

    internal virtual void _updateSelectedContentIfNeeded()
    {
        if (widget.onSelectionChanged is null)
        {
            return;
        }
        SelectedContent? content = _selectable?.getSelectedContent();
        if (_lastSelectedContent?.plainText != content?.plainText)
        {
            _lastSelectedContent = content;
            widget.onSelectionChanged!?.Invoke(_lastSelectedContent);
        }
    }

    internal virtual void _handleTouchLongPressStart(LongPressStartDetails details)
    {
        DartRuntimePrimitives.Ignore(HapticFeedback.selectionClick());
        _focusNode.requestFocus();
        _selectWordAt(offset: details.globalPosition);
        _selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
        if (!Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.android))
        {
            _showHandles();
        }
        _updateSelectedContentIfNeeded();
    }

    internal virtual void _handleTouchLongPressMoveUpdate(LongPressMoveUpdateDetails details)
    {
        _selectEndTo(offset: details.globalPosition, textGranularity: TextGranularity.word);
        _selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
        _updateSelectedContentIfNeeded();
    }

    internal virtual void _handleTouchLongPressEnd(LongPressEndDetails details)
    {
        _finalizeSelection();
        _updateSelectedContentIfNeeded();
        _finalizeSelectableRegionStatus();
        if (Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.android))
        {
            _showHandles();
        }
        _showToolbar();
    }

    internal virtual bool _positionIsOnActiveSelection(Offset globalPosition)
    {
        foreach (Rect selectionRect in _selectionDelegate.value.selectionRects)
        {
            Matrix4 transform = _selectable!.getTransformTo(null);
            Rect globalRect = MatrixUtils.transformRect(transform, selectionRect);
            if (globalRect.contains(globalPosition))
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleRightClickDown(TapDownDetails details)
    {
        Offset? previousSecondaryTapDownPosition = _lastSecondaryTapDownPosition;
        bool toolbarIsVisibleLocal = _selectionOverlay?.toolbarIsVisible ?? false;
        _lastSecondaryTapDownPosition = details.globalPosition;
        _focusNode.requestFocus();
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.windows:
            {
                bool lastSecondaryTapDownPositionWasOnActiveSelection =
                    _positionIsOnActiveSelection(globalPosition: details.globalPosition);
                if (lastSecondaryTapDownPositionWasOnActiveSelection)
                {
                    _lastSecondaryTapDownPosition = details.globalPosition;
                    _showHandles();
                    _showToolbar(location: _lastSecondaryTapDownPosition);
                    _updateSelectedContentIfNeeded();
                    return;
                }
                _collapseSelectionAt(
                    offset: DartRuntimePrimitives.RequireValue(_lastSecondaryTapDownPosition)
                );
                break;
            }
            case TargetPlatform.iOS:
            {
                _selectWordAt(
                    offset: DartRuntimePrimitives.RequireValue(_lastSecondaryTapDownPosition)
                );
                break;
            }
            case TargetPlatform.macOS:
            {
                if (
                    Equals(previousSecondaryTapDownPosition, _lastSecondaryTapDownPosition)
                    && toolbarIsVisibleLocal
                )
                {
                    hideToolbar();
                    return;
                }
                _selectWordAt(
                    offset: DartRuntimePrimitives.RequireValue(_lastSecondaryTapDownPosition)
                );
                break;
            }
            case TargetPlatform.linux:
            {
                if (toolbarIsVisibleLocal)
                {
                    hideToolbar();
                    return;
                }
                bool lastSecondaryTapDownPositionWasOnActiveSelectionLocal =
                    _positionIsOnActiveSelection(globalPosition: details.globalPosition);
                if (!lastSecondaryTapDownPositionWasOnActiveSelectionLocal)
                {
                    _collapseSelectionAt(
                        offset: DartRuntimePrimitives.RequireValue(_lastSecondaryTapDownPosition)
                    );
                }
                break;
            }
        }
        _selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
        _finalizeSelectableRegionStatus();
        _lastSecondaryTapDownPosition = details.globalPosition;
        _showHandles();
        _showToolbar(location: _lastSecondaryTapDownPosition);
        _updateSelectedContentIfNeeded();
    }

    internal virtual bool _userDraggingSelectionEnd =>
        DartRuntimePrimitives.ConvertValue<bool>(_selectionEndPosition is not null);

    internal virtual void _triggerSelectionEndEdgeUpdate(TextGranularity? textGranularity = null)
    {
        if (_scheduledSelectionEndEdgeUpdate || !_userDraggingSelectionEnd)
        {
            return;
        }
        if (
            Equals(
                _selectable?.dispatchSelectionEvent(
                    SelectionEdgeUpdateEvent.CreateForEnd(
                        globalPosition: DartRuntimePrimitives.RequireValue(_selectionEndPosition),
                        granularity: textGranularity
                    )
                ),
                SelectionResult.pending
            )
        )
        {
            _scheduledSelectionEndEdgeUpdate = true;
            Scheduler.SchedulerBinding.instance.addPostFrameCallback(
                (timeStamp) =>
                {
                    if (!_scheduledSelectionEndEdgeUpdate)
                    {
                        return;
                    }
                    _scheduledSelectionEndEdgeUpdate = false;
                    _triggerSelectionEndEdgeUpdate(textGranularity: textGranularity);
                },
                debugLabel: "SelectableRegion.endEdgeUpdate"
            );
            return;
        }
    }

    internal virtual void _onAnyDragEnd(DragEndDetails details)
    {
        bool draggingHandles =
            (_selectionOverlay is not null)
            && (_selectionOverlay!.isDraggingStartHandle || _selectionOverlay!.isDraggingEndHandle);
        if (!draggingHandles)
        {
            _selectionOverlay!.hideMagnifier();
            _showToolbar();
        }
        _finalizeSelection();
        _updateSelectedContentIfNeeded();
        _finalizeSelectableRegionStatus();
    }

    internal virtual void _stopSelectionEndEdgeUpdate()
    {
        _scheduledSelectionEndEdgeUpdate = false;
        _selectionEndPosition = null;
    }

    internal virtual bool _userDraggingSelectionStart =>
        DartRuntimePrimitives.ConvertValue<bool>(_selectionStartPosition is not null);

    internal virtual void _triggerSelectionStartEdgeUpdate(TextGranularity? textGranularity = null)
    {
        if (_scheduledSelectionStartEdgeUpdate || !_userDraggingSelectionStart)
        {
            return;
        }
        if (
            Equals(
                _selectable?.dispatchSelectionEvent(
                    new SelectionEdgeUpdateEvent(
                        globalPosition: DartRuntimePrimitives.RequireValue(_selectionStartPosition),
                        granularity: textGranularity
                    )
                ),
                SelectionResult.pending
            )
        )
        {
            _scheduledSelectionStartEdgeUpdate = true;
            Scheduler.SchedulerBinding.instance.addPostFrameCallback(
                (timeStamp) =>
                {
                    if (!_scheduledSelectionStartEdgeUpdate)
                    {
                        return;
                    }
                    _scheduledSelectionStartEdgeUpdate = false;
                    _triggerSelectionStartEdgeUpdate(textGranularity: textGranularity);
                },
                debugLabel: "SelectableRegion.startEdgeUpdate"
            );
            return;
        }
    }

    internal virtual void _stopSelectionStartEdgeUpdate()
    {
        _scheduledSelectionStartEdgeUpdate = false;
        _selectionEndPosition = null;
    }

    internal virtual void _handleSelectionStartHandleDragStart(DragStartDetails details)
    {
        DartRuntimePrimitives.Assert(() =>
            _selectionDelegate.value.startSelectionPoint is not null
        );
        Offset localPositionLocal = _selectionDelegate.value.startSelectionPoint!.localPosition;
        Matrix4 globalTransform = _selectable!.getTransformTo(null);
        _selectionStartHandleDragPosition = MatrixUtils.transformPoint(
            globalTransform,
            localPositionLocal
        );
        _selectionOverlay!.showMagnifier(
            _buildInfoForMagnifier(
                details.globalPosition,
                _selectionDelegate.value.startSelectionPoint!
            )
        );
        _updateSelectedContentIfNeeded();
    }

    internal virtual void _handleSelectionStartHandleDragUpdate(DragUpdateDetails details)
    {
        _selectionStartHandleDragPosition = _selectionStartHandleDragPosition + details.delta;
        _selectionStartPosition =
            _selectionStartHandleDragPosition
            - new Offset(0, _selectionDelegate.value.startSelectionPoint!.lineHeight / 2L);
        _triggerSelectionStartEdgeUpdate();
        _selectionOverlay!.updateMagnifier(
            _buildInfoForMagnifier(
                details.globalPosition,
                _selectionDelegate.value.startSelectionPoint!
            )
        );
        _updateSelectedContentIfNeeded();
        _selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
    }

    internal virtual void _handleSelectionEndHandleDragStart(DragStartDetails details)
    {
        DartRuntimePrimitives.Assert(() => _selectionDelegate.value.endSelectionPoint is not null);
        Offset localPositionLocal = _selectionDelegate.value.endSelectionPoint!.localPosition;
        Matrix4 globalTransform = _selectable!.getTransformTo(null);
        _selectionEndHandleDragPosition = MatrixUtils.transformPoint(
            globalTransform,
            localPositionLocal
        );
        _selectionOverlay!.showMagnifier(
            _buildInfoForMagnifier(
                details.globalPosition,
                _selectionDelegate.value.endSelectionPoint!
            )
        );
        _updateSelectedContentIfNeeded();
    }

    internal virtual void _handleSelectionEndHandleDragUpdate(DragUpdateDetails details)
    {
        _selectionEndHandleDragPosition = _selectionEndHandleDragPosition + details.delta;
        _selectionEndPosition =
            _selectionEndHandleDragPosition
            - new Offset(0, _selectionDelegate.value.endSelectionPoint!.lineHeight / 2L);
        _triggerSelectionEndEdgeUpdate();
        _selectionOverlay!.updateMagnifier(
            _buildInfoForMagnifier(
                details.globalPosition,
                _selectionDelegate.value.endSelectionPoint!
            )
        );
        _updateSelectedContentIfNeeded();
        _selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
    }

    internal virtual MagnifierInfo _buildInfoForMagnifier(
        Offset globalGesturePosition,
        SelectionPoint selectionPoint
    )
    {
        Vector3 globalTransform = _selectable!.getTransformTo(null).getTranslation();
        var globalTransformAsOffset = new Offset(globalTransform.x, globalTransform.y);
        Offset globalSelectionPointPosition =
            selectionPoint.localPosition + globalTransformAsOffset;
        var caretRectLocal = Rect.fromLTWH(
            globalSelectionPointPosition.dx,
            globalSelectionPointPosition.dy - selectionPoint.lineHeight,
            0,
            selectionPoint.lineHeight
        );
        return new MagnifierInfo(
            globalGesturePosition: globalGesturePosition,
            caretRect: caretRectLocal,
            fieldBounds: globalTransformAsOffset & _selectable!.size,
            currentLineBoundaries: globalTransformAsOffset & _selectable!.size
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _createSelectionOverlay()
    {
        DartRuntimePrimitives.Assert(() => _hasSelectionOverlayGeometry);
        if (_selectionOverlay is not null)
        {
            return;
        }
        SelectionPoint? start = _selectionDelegate.value.startSelectionPoint;
        SelectionPoint? end = _selectionDelegate.value.endSelectionPoint;
        _selectionOverlay = new SelectionOverlay(
            context: context,
            debugRequiredFor: widget,
            startHandleType: start?.handleType ?? TextSelectionHandleType.collapsed,
            lineHeightAtStart: start?.lineHeight ?? end!.lineHeight,
            onStartHandleDragStart: _handleSelectionStartHandleDragStart,
            onStartHandleDragUpdate: _handleSelectionStartHandleDragUpdate,
            onStartHandleDragEnd: _onAnyDragEnd,
            endHandleType: end?.handleType ?? TextSelectionHandleType.collapsed,
            lineHeightAtEnd: end?.lineHeight ?? start!.lineHeight,
            onEndHandleDragStart: _handleSelectionEndHandleDragStart,
            onEndHandleDragUpdate: _handleSelectionEndHandleDragUpdate,
            onEndHandleDragEnd: _onAnyDragEnd,
            selectionEndpoints: selectionEndpoints,
            selectionControls: widget.selectionControls,
            selectionDelegate: this,
            clipboardStatus: null,
            startHandleLayerLink: _startHandleLayerLink,
            endHandleLayerLink: _endHandleLayerLink,
            toolbarLayerLink: _toolbarLayerLink,
            magnifierConfiguration: widget.magnifierConfiguration
        );
    }

    internal virtual void _updateSelectionOverlay()
    {
        if (_selectionOverlay is null)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => _hasSelectionOverlayGeometry);
        SelectionPoint? start = _selectionDelegate.value.startSelectionPoint;
        SelectionPoint? end = _selectionDelegate.value.endSelectionPoint;
        DartRuntimePrimitives.Ignore(
            (
                (Func<SelectionOverlay>)(
                    () =>
                    {
                        var __cascade = _selectionOverlay!;
                        __cascade.startHandleType =
                            start?.handleType ?? TextSelectionHandleType.left;
                        __cascade.lineHeightAtStart = start?.lineHeight ?? end!.lineHeight;
                        __cascade.endHandleType = end?.handleType ?? TextSelectionHandleType.right;
                        __cascade.lineHeightAtEnd = end?.lineHeight ?? start!.lineHeight;
                        __cascade.selectionEndpoints = selectionEndpoints;
                        return __cascade;
                    }
                )
            )()
        );
    }

    internal virtual bool _showHandles()
    {
        if (_selectionOverlay is not null)
        {
            _selectionOverlay!.showHandles();
            return true;
        }
        if (!_hasSelectionOverlayGeometry)
        {
            return false;
        }
        _createSelectionOverlay();
        _selectionOverlay!.showHandles();
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _showToolbar(Offset? location = null)
    {
        if (!_hasSelectionOverlayGeometry && (_selectionOverlay is null))
        {
            return false;
        }
        if (_webContextMenuEnabled)
        {
            return false;
        }
        if (_selectionOverlay is null)
        {
            _createSelectionOverlay();
        }
        _selectionOverlay!.toolbarLocation = location;
        if (widget.selectionControls is not TextSelectionHandleControls)
        {
            _selectionOverlay!.showToolbar();
            return true;
        }
        _selectionOverlay!.hideToolbar();
        _selectionOverlay!.showToolbar(
            context: context,
            contextMenuBuilder: (context) =>
            {
                return widget.contextMenuBuilder!(context, this);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _selectEndTo(
        Offset offset,
        bool continuous = false,
        TextGranularity? textGranularity = null
    )
    {
        if (!continuous)
        {
            _selectable?.dispatchSelectionEvent(
                SelectionEdgeUpdateEvent.CreateForEnd(
                    globalPosition: offset,
                    granularity: textGranularity
                )
            );
            return;
        }
        if (!Equals(_selectionEndPosition, offset))
        {
            _selectionEndPosition = offset;
            _triggerSelectionEndEdgeUpdate(textGranularity: textGranularity);
        }
    }

    internal virtual void _selectStartTo(
        Offset offset,
        bool continuous = false,
        TextGranularity? textGranularity = null
    )
    {
        if (!continuous)
        {
            _selectable?.dispatchSelectionEvent(
                new SelectionEdgeUpdateEvent(globalPosition: offset, granularity: textGranularity)
            );
            return;
        }
        if (!Equals(_selectionStartPosition, offset))
        {
            _selectionStartPosition = offset;
            _triggerSelectionStartEdgeUpdate(textGranularity: textGranularity);
        }
    }

    internal virtual void _collapseSelectionAt(Offset offset)
    {
        _finalizeSelection();
        _selectStartTo(offset: offset);
        _selectEndTo(offset: offset);
    }

    internal virtual void _selectWordAt(Offset offset)
    {
        _finalizeSelection();
        _selectable?.dispatchSelectionEvent(new SelectWordSelectionEvent(globalPosition: offset));
    }

    internal virtual void _selectParagraphAt(Offset offset)
    {
        _finalizeSelection();
        _selectable?.dispatchSelectionEvent(
            new SelectParagraphSelectionEvent(globalPosition: offset)
        );
    }

    internal virtual void _finalizeSelection()
    {
        _stopSelectionEndEdgeUpdate();
        _stopSelectionStartEdgeUpdate();
    }

    public virtual void clearSelection()
    {
        _finalizeSelection();
        _directionalHorizontalBaseline = null;
        _adjustingSelectionEnd = null;
        _selectable?.dispatchSelectionEvent(new ClearSelectionEvent());
        _updateSelectedContentIfNeeded();
    }

    internal virtual async Future _copy()
    {
        SelectedContent? data = _selectable?.getSelectedContent();
        if (data is null)
        {
            return;
        }
        await Clipboard.setData(new ClipboardData(text: data.plainText));
    }

    internal virtual async Future _share()
    {
        SelectedContent? data = _selectable?.getSelectedContent();
        if (data is null)
        {
            return;
        }
        await SystemChannels.platform.invokeMethod<object>("Share.invoke", data.plainText);
    }

    public virtual TextSelectionToolbarAnchors contextMenuAnchors
    {
        get
        {
            if (_lastSecondaryTapDownPosition is not null)
            {
                var anchors = new TextSelectionToolbarAnchors(
                    primaryAnchor: DartRuntimePrimitives.RequireValue(_lastSecondaryTapDownPosition)
                );
                _lastSecondaryTapDownPosition = null;
                return anchors;
            }
            var renderBoxLocal = ((RenderBox?)context.findRenderObject()!)!;
            return TextSelectionToolbarAnchors.CreateFromSelection(
                renderBox: renderBoxLocal,
                startGlyphHeight: startGlyphHeight,
                endGlyphHeight: endGlyphHeight,
                selectionEndpoints: selectionEndpoints
            );
        }
    }

    internal virtual bool _determineIsAdjustingSelectionEnd(bool forward)
    {
        if (_adjustingSelectionEnd is not null)
        {
            return DartRuntimePrimitives.RequireValue(_adjustingSelectionEnd);
        }
        bool isReversed = default!;
        SelectionPoint start = _selectionDelegate.value.startSelectionPoint!;
        SelectionPoint end = _selectionDelegate.value.endSelectionPoint!;
        if (start.localPosition.dy > end.localPosition.dy)
        {
            isReversed = true;
        }
        else
        {
            if (start.localPosition.dy < end.localPosition.dy)
            {
                isReversed = false;
            }
            else
            {
                isReversed = start.localPosition.dx > end.localPosition.dx;
            }
        }
        return DartRuntimePrimitives.RequireValue(_adjustingSelectionEnd = forward != isReversed);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _granularlyExtendSelection(TextGranularity granularity, bool forward)
    {
        _directionalHorizontalBaseline = null;
        if (!_selectionDelegate.value.hasSelection)
        {
            return;
        }
        _selectable?.dispatchSelectionEvent(
            new GranularlyExtendSelectionEvent(
                forward: forward,
                isEnd: _determineIsAdjustingSelectionEnd(forward),
                granularity: granularity
            )
        );
        _updateSelectedContentIfNeeded();
        _selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
        _finalizeSelectableRegionStatus();
    }

    internal virtual void _directionallyExtendSelection(bool forward)
    {
        if (!_selectionDelegate.value.hasSelection)
        {
            return;
        }
        bool adjustingSelectionExtend = _determineIsAdjustingSelectionEnd(forward);
        SelectionPoint baseLinePoint = adjustingSelectionExtend
            ? _selectionDelegate.value.endSelectionPoint!
            : _selectionDelegate.value.startSelectionPoint!;
        _directionalHorizontalBaseline ??= baseLinePoint.localPosition.dx;
        Offset globalSelectionPointOffset = MatrixUtils.transformPoint(
            context.findRenderObject()!.getTransformTo(null),
            new Offset(DartRuntimePrimitives.RequireValue(_directionalHorizontalBaseline), 0)
        );
        _selectable?.dispatchSelectionEvent(
            new DirectionallyExtendSelectionEvent(
                isEnd: DartRuntimePrimitives.RequireValue(_adjustingSelectionEnd),
                direction: forward
                    ? SelectionExtendDirection.nextLine
                    : SelectionExtendDirection.previousLine,
                dx: globalSelectionPointOffset.dx
            )
        );
        _updateSelectedContentIfNeeded();
        _selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
        _finalizeSelectableRegionStatus();
    }

    public virtual List<ContextMenuButtonItem> contextMenuButtonItems
    {
        get
        {
            return (
                (Func<List<ContextMenuButtonItem>>)(
                    () =>
                    {
                        var __cascade = SelectableRegion.getSelectableButtonItems(
                            selectionGeometry: _selectionDelegate.value,
                            onCopy: () =>
                            {
                                DartRuntimePrimitives.Ignore(_copy());
                                switch (PlatformLibrary.defaultTargetPlatform)
                                {
                                    case TargetPlatform.android:
                                    case TargetPlatform.fuchsia:
                                    {
                                        clearSelection();
                                        _selectionStatusNotifier.value =
                                            SelectableRegionSelectionStatus.changing;
                                        _finalizeSelectableRegionStatus();
                                        break;
                                    }
                                    case TargetPlatform.iOS:
                                    {
                                        hideToolbar(false);
                                        break;
                                    }
                                    case TargetPlatform.linux:
                                    case TargetPlatform.macOS:
                                    case TargetPlatform.windows:
                                    {
                                        hideToolbar();
                                        break;
                                    }
                                }
                            },
                            onSelectAll: () =>
                            {
                                switch (PlatformLibrary.defaultTargetPlatform)
                                {
                                    case TargetPlatform.android:
                                    case TargetPlatform.iOS:
                                    case TargetPlatform.fuchsia:
                                    {
                                        selectAll(SelectionChangedCause.toolbar);
                                        break;
                                    }
                                    case TargetPlatform.linux:
                                    case TargetPlatform.macOS:
                                    case TargetPlatform.windows:
                                    {
                                        selectAll();
                                        hideToolbar();
                                        break;
                                    }
                                }
                            },
                            onShare: () =>
                            {
                                DartRuntimePrimitives.Ignore(_share());
                                switch (PlatformLibrary.defaultTargetPlatform)
                                {
                                    case TargetPlatform.android:
                                    case TargetPlatform.fuchsia:
                                    {
                                        clearSelection();
                                        _selectionStatusNotifier.value =
                                            SelectableRegionSelectionStatus.changing;
                                        _finalizeSelectableRegionStatus();
                                        break;
                                    }
                                    case TargetPlatform.iOS:
                                    {
                                        hideToolbar(false);
                                        break;
                                    }
                                    case TargetPlatform.linux:
                                    case TargetPlatform.macOS:
                                    case TargetPlatform.windows:
                                    {
                                        hideToolbar();
                                        break;
                                    }
                                }
                            }
                        );
                        __cascade.AddRange(
                            _textProcessingActionButtonItems.Cast<ContextMenuButtonItem>()
                        );
                        return __cascade;
                    }
                )
            )();
        }
    }
    internal virtual List<ContextMenuButtonItem> _textProcessingActionButtonItems
    {
        get
        {
            var buttonItems = new List<ContextMenuButtonItem>();
            SelectedContent? data = _selectable?.getSelectedContent();
            if (data is null)
            {
                return buttonItems;
            }
            foreach (ProcessTextAction action in _processTextActions)
            {
                buttonItems.Add(
                    new ContextMenuButtonItem(
                        label: action.label,
                        onPressed: async () =>
                        {
                            string selectedText = data.plainText;
                            if (selectedText.Length != 0)
                            {
                                await _processTextService.processTextAction(
                                    action.id,
                                    selectedText,
                                    true
                                );
                                hideToolbar();
                            }
                        }
                    )
                );
            }
            return buttonItems;
        }
    }
    public virtual double startGlyphHeight
    {
        get { return _selectionDelegate.value.startSelectionPoint!.lineHeight; }
    }
    public virtual double endGlyphHeight
    {
        get { return _selectionDelegate.value.endSelectionPoint!.lineHeight; }
    }
    public virtual List<TextSelectionPoint> selectionEndpoints
    {
        get
        {
            SelectionPoint? start = _selectionDelegate.value.startSelectionPoint;
            SelectionPoint? end = _selectionDelegate.value.endSelectionPoint;
            List<TextSelectionPoint> points = default!;
            Offset startLocalPosition = start?.localPosition ?? end!.localPosition;
            Offset endLocalPosition = end?.localPosition ?? start!.localPosition;
            if (startLocalPosition.dy > endLocalPosition.dy)
            {
                points = new List<TextSelectionPoint>
                {
                    new TextSelectionPoint(endLocalPosition, TextDirection.ltr),
                    new TextSelectionPoint(startLocalPosition, TextDirection.ltr),
                };
            }
            else
            {
                points = new List<TextSelectionPoint>
                {
                    new TextSelectionPoint(startLocalPosition, TextDirection.ltr),
                    new TextSelectionPoint(endLocalPosition, TextDirection.ltr),
                };
            }
            return points;
        }
    }
    public virtual bool cutEnabled => false;
    public virtual bool pasteEnabled => false;

    public virtual void hideToolbar(bool hideHandles = true)
    {
        _selectionOverlay?.hideToolbar();
        if (hideHandles)
        {
            _selectionOverlay?.hideHandles();
        }
    }

    internal virtual object? _hideToolbarIfVisible(DismissIntent intent)
    {
        if (_selectionOverlay?.toolbarIsVisible ?? false)
        {
            hideToolbar(false);
            return null;
        }
        return Actions.invoke(context, intent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void selectAll(SelectionChangedCause cause = default!)
    {
        clearSelection();
        _selectable?.dispatchSelectionEvent(new SelectAllSelectionEvent());
        if (Equals(cause, SelectionChangedCause.toolbar))
        {
            _showHandles();
            _showToolbar();
        }
        _updateSelectedContentIfNeeded();
        _selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
        _finalizeSelectableRegionStatus();
    }

    public virtual void copySelection(SelectionChangedCause cause)
    {
        DartRuntimePrimitives.Ignore(_copy());
        clearSelection();
        _selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
        _finalizeSelectableRegionStatus();
    }

    public virtual void bringIntoView(TextPosition position) { }

    public virtual void cutSelection(SelectionChangedCause cause)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public virtual void userUpdateTextEditingValue(
        TextEditingValue value,
        SelectionChangedCause cause
    ) { }

    public virtual async Future pasteText(SelectionChangedCause cause)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public virtual void add(Selectable selectable)
    {
        DartRuntimePrimitives.Assert(() => _selectable is null);
        _selectable = selectable;
        _selectable!.addListener(_updateSelectionStatus);
        _selectable!.pushHandleLayers(_startHandleLayerLink, _endHandleLayerLink);
    }

    public virtual void remove(Selectable selectable)
    {
        DartRuntimePrimitives.Assert(() => Equals(_selectable, selectable));
        _selectable!.removeListener(_updateSelectionStatus);
        _selectable!.pushHandleLayers(null, null);
        _selectable = null;
    }

    public override void dispose()
    {
        _selectable?.removeListener(_updateSelectionStatus);
        _selectable?.pushHandleLayers(null, null);
        if (Foundation.ConstantsLibrary.kIsWeb)
        {
            PlatformSelectableRegionContextMenuIo.detach(_selectionDelegate);
        }
        _selectionDelegate.dispose();
        _selectionStatusNotifier.dispose();
        _selectionOverlay?.hideMagnifier();
        _selectionOverlay?.dispose();
        _selectionOverlay = null;
        widget.focusNode?.removeListener(_handleFocusChanged);
        _localFocusNode?.removeListener(_handleFocusChanged);
        _localFocusNode?.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasOverlay(context));
        Widget result = new SelectableRegionSelectionStatusScope(
            selectionStatusNotifier: _selectionStatusNotifier,
            child: new SelectionContainer(
                registrar: this,
                @delegate: _selectionDelegate,
                child: widget.child
            )
        );
        if (_webContextMenuEnabled)
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(
                new PlatformSelectableRegionContextMenuIo(child: result)
            );
        }
        return new TapRegion(
            groupId: typeof(SelectableRegion),
            onTapOutside: (@event) =>
            {
                if (Foundation.ConstantsLibrary.kIsWeb)
                {
                    _focusNode.unfocus();
                }
            },
            child: new CompositedTransformTarget(
                link: _toolbarLayerLink,
                child: new RawGestureDetector(
                    gestures: _gestureRecognizers,
                    behavior: HitTestBehavior.translucent,
                    excludeFromSemantics: true,
                    child: new Actions(
                        actions: _actions,
                        child: Focus.CreateWithExternalFocusNode(
                            includeSemantics: false,
                            focusNode: _focusNode,
                            child: result
                        )
                    )
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool copyEnabled => true;
    public virtual bool selectAllEnabled => true;
    public virtual bool lookUpEnabled => true;
    public virtual bool searchWebEnabled => true;
    public virtual bool shareEnabled => true;
    public virtual bool liveTextInputEnabled => false;
}

internal abstract class _NonOverrideAction__selectable_region<T> : ContextAction<T>
    where T : Intent
{
    public abstract object? invokeAction(T intent, BuildContext? context = null);

    public override object? invoke(T intent, BuildContext? context = null)
    {
        if (callingAction is IntentAction<T> callingActionLocal)
        {
            return callingActionLocal.invoke(intent);
        }
        return invokeAction(intent, context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _SelectAllAction__selectable_region
    : _NonOverrideAction__selectable_region<SelectAllTextIntent>
{
    public virtual SelectableRegionState state { get; private set; } = default!;

    internal _SelectAllAction__selectable_region(SelectableRegionState state)
    {
        this.state = state;
    }

    public override object? invokeAction(SelectAllTextIntent intent, BuildContext? context = null)
    {
        state.selectAll(SelectionChangedCause.keyboard);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _CopySelectionAction__selectable_region
    : _NonOverrideAction__selectable_region<CopySelectionTextIntent>
{
    public virtual SelectableRegionState state { get; private set; } = default!;

    internal _CopySelectionAction__selectable_region(SelectableRegionState state)
    {
        this.state = state;
    }

    public override object? invokeAction(
        CopySelectionTextIntent intent,
        BuildContext? context = null
    )
    {
        DartRuntimePrimitives.Ignore(state._copy());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _GranularlyExtendSelectionAction__selectable_region<T>
    : _NonOverrideAction__selectable_region<T>
    where T : DirectionalTextEditingIntent
{
    public virtual SelectableRegionState state { get; private set; } = default!;
    public virtual TextGranularity granularity { get; private set; } = default!;

    internal _GranularlyExtendSelectionAction__selectable_region(
        SelectableRegionState state,
        TextGranularity granularity
    )
    {
        this.state = state;
        this.granularity = granularity;
    }

    public override object? invokeAction(T intent, BuildContext? context = null)
    {
        state._granularlyExtendSelection(granularity, intent.forward);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _GranularlyExtendCaretSelectionAction__selectable_region<T>
    : _NonOverrideAction__selectable_region<T>
    where T : DirectionalCaretMovementIntent
{
    public virtual SelectableRegionState state { get; private set; } = default!;
    public virtual TextGranularity granularity { get; private set; } = default!;

    internal _GranularlyExtendCaretSelectionAction__selectable_region(
        SelectableRegionState state,
        TextGranularity granularity
    )
    {
        this.state = state;
        this.granularity = granularity;
    }

    public override object? invokeAction(T intent, BuildContext? context = null)
    {
        if (intent.collapseSelection)
        {
            return default!;
        }
        state._granularlyExtendSelection(granularity, intent.forward);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _DirectionallyExtendCaretSelectionAction__selectable_region<T>
    : _NonOverrideAction__selectable_region<T>
    where T : DirectionalCaretMovementIntent
{
    public virtual SelectableRegionState state { get; private set; } = default!;

    internal _DirectionallyExtendCaretSelectionAction__selectable_region(
        SelectableRegionState state
    )
    {
        this.state = state;
    }

    public override object? invokeAction(T intent, BuildContext? context = null)
    {
        if (intent.collapseSelection)
        {
            return default!;
        }
        state._directionallyExtendSelection(intent.forward);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class StaticSelectionContainerDelegate : MultiSelectableSelectionContainerDelegate
{
    internal virtual HashSet<Selectable> _hasReceivedStartEvent { get; private set; } =
        new HashSet<Selectable>();
    internal virtual HashSet<Selectable> _hasReceivedEndEvent { get; private set; } =
        new HashSet<Selectable>();
    internal virtual Offset? _lastStartEdgeUpdateGlobalPosition { get; set; } = default;
    internal virtual Offset? _lastEndEdgeUpdateGlobalPosition { get; set; } = default;

    public virtual void didReceiveSelectionEventFor(Selectable selectable, bool? forEnd = null)
    {
        switch (forEnd)
        {
            case true:
            {
                _hasReceivedEndEvent.Add(selectable);
                break;
            }
            case false:
            {
                _hasReceivedStartEvent.Add(selectable);
                break;
            }
            case null:
            {
                _hasReceivedStartEvent.Add(selectable);
                _hasReceivedEndEvent.Add(selectable);
                break;
            }
        }
    }

    public virtual void didReceiveSelectionBoundaryEvents()
    {
        if ((currentSelectionStartIndex == -1L) || (currentSelectionEndIndex == -1L))
        {
            return;
        }
        long start = Math.Min(currentSelectionStartIndex, currentSelectionEndIndex);
        long end = Math.Max(currentSelectionStartIndex, currentSelectionEndIndex);
        for (var index = start; index <= end; index += 1L)
        {
            didReceiveSelectionEventFor(selectable: selectables[(int)index]);
        }
        _updateLastSelectionEdgeLocationsFromGeometries();
    }

    public virtual void updateLastSelectionEdgeLocation(
        Offset globalSelectionEdgeLocation,
        bool forEnd
    )
    {
        if (DartRuntimePrimitives.RequireValue(forEnd))
        {
            _lastEndEdgeUpdateGlobalPosition = globalSelectionEdgeLocation;
        }
        else
        {
            _lastStartEdgeUpdateGlobalPosition = globalSelectionEdgeLocation;
        }
    }

    internal virtual void _updateLastSelectionEdgeLocationsFromGeometries()
    {
        if (
            (currentSelectionStartIndex != -1L)
            && selectables[(int)currentSelectionStartIndex].value.hasSelection
        )
        {
            Selectable start = selectables[(int)currentSelectionStartIndex];
            Offset localStartEdge =
                start.value.startSelectionPoint!.localPosition
                + new Offset(0, -start.value.startSelectionPoint!.lineHeight / 2L);
            updateLastSelectionEdgeLocation(
                globalSelectionEdgeLocation: MatrixUtils.transformPoint(
                    start.getTransformTo(null),
                    localStartEdge
                ),
                forEnd: false
            );
        }
        if (
            (currentSelectionEndIndex != -1L)
            && selectables[(int)currentSelectionEndIndex].value.hasSelection
        )
        {
            Selectable end = selectables[(int)currentSelectionEndIndex];
            Offset localEndEdge =
                end.value.endSelectionPoint!.localPosition
                + new Offset(0, -end.value.endSelectionPoint!.lineHeight / 2L);
            updateLastSelectionEdgeLocation(
                globalSelectionEdgeLocation: MatrixUtils.transformPoint(
                    end.getTransformTo(null),
                    localEndEdge
                ),
                forEnd: true
            );
        }
    }

    public virtual void clearInternalSelectionState()
    {
        selectables.forEach(
            (__arg0) => ((Action<Selectable>)clearInternalSelectionStateForSelectable)(__arg0)
        );
        _lastStartEdgeUpdateGlobalPosition = null;
        _lastEndEdgeUpdateGlobalPosition = null;
    }

    public virtual void clearInternalSelectionStateForSelectable(Selectable selectable)
    {
        _hasReceivedStartEvent.Remove(selectable);
        _hasReceivedEndEvent.Remove(selectable);
    }

    public override void remove(Selectable selectable)
    {
        clearInternalSelectionStateForSelectable(selectable);
        base.remove(selectable);
    }

    public override SelectionResult handleSelectAll(SelectAllSelectionEvent @event)
    {
        SelectionResult result = base.handleSelectAll(@event);
        didReceiveSelectionBoundaryEvents();
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override SelectionResult handleSelectWord(SelectWordSelectionEvent @event)
    {
        SelectionResult result = base.handleSelectWord(@event);
        didReceiveSelectionBoundaryEvents();
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override SelectionResult handleSelectParagraph(SelectParagraphSelectionEvent @event)
    {
        SelectionResult result = base.handleSelectParagraph(@event);
        didReceiveSelectionBoundaryEvents();
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override SelectionResult handleClearSelection(ClearSelectionEvent @event)
    {
        SelectionResult result = base.handleClearSelection(@event);
        clearInternalSelectionState();
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override SelectionResult handleSelectionEdgeUpdate(SelectionEdgeUpdateEvent @event)
    {
        updateLastSelectionEdgeLocation(
            globalSelectionEdgeLocation: @event.globalPosition,
            forEnd: Equals(@event.type, SelectionEventType.endEdgeUpdate)
        );
        return base.handleSelectionEdgeUpdate(@event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        clearInternalSelectionState();
        base.dispose();
    }

    public override SelectionResult dispatchSelectionEventToChild(
        Selectable selectable,
        SelectionEvent @event
    )
    {
        switch (@event.type)
        {
            case SelectionEventType.startEdgeUpdate:
            {
                didReceiveSelectionEventFor(selectable: selectable, forEnd: false);
                ensureChildUpdated(selectable);
                break;
            }
            case SelectionEventType.endEdgeUpdate:
            {
                didReceiveSelectionEventFor(selectable: selectable, forEnd: true);
                ensureChildUpdated(selectable);
                break;
            }
            case SelectionEventType.clear:
            {
                clearInternalSelectionStateForSelectable(selectable);
                break;
            }
            case SelectionEventType.selectAll:
            case SelectionEventType.selectWord:
            case SelectionEventType.selectParagraph:
            {
                break;
            }
            case SelectionEventType.granularlyExtendSelection:
            case SelectionEventType.directionallyExtendSelection:
            {
                didReceiveSelectionEventFor(selectable: selectable);
                ensureChildUpdated(selectable);
                break;
            }
        }
        return base.dispatchSelectionEventToChild(selectable, @event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void ensureChildUpdated(Selectable selectable)
    {
        if ((_lastEndEdgeUpdateGlobalPosition is not null) && _hasReceivedEndEvent.Add(selectable))
        {
            var synthesizedEvent = SelectionEdgeUpdateEvent.CreateForEnd(
                globalPosition: DartRuntimePrimitives.RequireValue(_lastEndEdgeUpdateGlobalPosition)
            );
            if (currentSelectionEndIndex == -1L)
            {
                handleSelectionEdgeUpdate(synthesizedEvent);
            }
            selectable.dispatchSelectionEvent(synthesizedEvent);
        }
        if (
            (_lastStartEdgeUpdateGlobalPosition is not null)
            && _hasReceivedStartEvent.Add(selectable)
        )
        {
            var synthesizedEventLocal = new SelectionEdgeUpdateEvent(
                globalPosition: DartRuntimePrimitives.RequireValue(
                    _lastStartEdgeUpdateGlobalPosition
                )
            );
            if (currentSelectionStartIndex == -1L)
            {
                handleSelectionEdgeUpdate(synthesizedEventLocal);
            }
            selectable.dispatchSelectionEvent(synthesizedEventLocal);
        }
    }

    public override void didChangeSelectables()
    {
        if (_lastEndEdgeUpdateGlobalPosition is not null)
        {
            handleSelectionEdgeUpdate(
                SelectionEdgeUpdateEvent.CreateForEnd(
                    globalPosition: DartRuntimePrimitives.RequireValue(
                        _lastEndEdgeUpdateGlobalPosition
                    )
                )
            );
        }
        if (_lastStartEdgeUpdateGlobalPosition is not null)
        {
            handleSelectionEdgeUpdate(
                new SelectionEdgeUpdateEvent(
                    globalPosition: DartRuntimePrimitives.RequireValue(
                        _lastStartEdgeUpdateGlobalPosition
                    )
                )
            );
        }
        HashSet<Selectable> selectableSet = selectables.toSet();
        _hasReceivedEndEvent.removeWhere((selectable) => !selectableSet.Contains(selectable));
        _hasReceivedStartEvent.removeWhere((selectable) => !selectableSet.Contains(selectable));
        base.didChangeSelectables();
    }
}

public abstract class MultiSelectableSelectionContainerDelegate : SelectionContainerDelegate
{
    public virtual List<Selectable> selectables { get; set; } = new List<Selectable>();
    internal const double _kSelectionHandleDrawableAreaPadding = 5.0;
    public virtual long currentSelectionEndIndex { get; set; } = -1L;
    public virtual long currentSelectionStartIndex { get; set; } = -1L;
    internal virtual LayerLink? _startHandleLayer { get; set; } = default;
    internal virtual Selectable? _startHandleLayerOwner { get; set; } = default;
    internal virtual LayerLink? _endHandleLayer { get; set; } = default;
    internal virtual Selectable? _endHandleLayerOwner { get; set; } = default;
    internal virtual bool _isHandlingSelectionEvent { get; set; } = false;
    internal virtual bool _scheduledSelectableUpdate { get; set; } = false;
    internal virtual bool _selectionInProgress { get; set; } = false;
    internal virtual HashSet<Selectable> _additions { get; set; } = new HashSet<Selectable>();
    internal virtual bool _extendSelectionInProgress { get; set; } = false;
    internal virtual SelectionGeometry _selectionGeometry { get; set; } =
        new SelectionGeometry(hasContent: false, status: SelectionStatus.none);

    protected MultiSelectableSelectionContainerDelegate() { }

    public override void add(Selectable selectable)
    {
        DartRuntimePrimitives.Assert(() => !selectables.Contains(selectable));
        _additions.Add(selectable);
        _scheduleSelectableUpdate();
    }

    public override void remove(Selectable selectable)
    {
        if (_additions.Remove(selectable))
        {
            return;
        }
        _removeSelectable(selectable);
        _scheduleSelectableUpdate();
    }

    public virtual void layoutDidChange()
    {
        _updateSelectionGeometry();
    }

    internal virtual void _scheduleSelectableUpdate()
    {
        if (!_scheduledSelectableUpdate)
        {
            _scheduledSelectableUpdate = true;
            void runScheduledTask(Duration? duration = null)
            {
                if (!_scheduledSelectableUpdate)
                {
                    return;
                }
                _scheduledSelectableUpdate = false;
                _updateSelectables();
            }
            if (
                Equals(
                    Scheduler.SchedulerBinding.instance.schedulerPhase,
                    Scheduler.SchedulerPhase.postFrameCallbacks
                )
            )
            {
                DartAsyncRuntime.scheduleMicrotask(runScheduledTask);
            }
            else
            {
                Scheduler.SchedulerBinding.instance.addPostFrameCallback(
                    (__arg0) =>
                        ((Action<Duration?>)runScheduledTask)(
                            DartRuntimePrimitives.ConvertValue<Duration>(__arg0)
                        ),
                    debugLabel: "SelectionContainer.runScheduledTask"
                );
            }
        }
    }

    internal virtual void _updateSelectables()
    {
        if (Enumerable.Any(_additions))
        {
            _flushAdditions();
        }
        didChangeSelectables();
    }

    internal virtual void _flushAdditions()
    {
        List<Selectable> mergingSelectables = (
            (Func<List<Selectable>>)(
                () =>
                {
                    var __cascade = _additions.ToList();
                    __cascade.sort(compareOrder);
                    return __cascade;
                }
            )
        )().ToList();
        List<Selectable> existingSelectables = selectables.ToList();
        selectables = new List<Selectable>();
        var mergingIndex = 0L;
        var existingIndex = 0L;
        long selectionStartIndex = currentSelectionStartIndex;
        long selectionEndIndex = currentSelectionEndIndex;
        while (
            (mergingIndex < checked(mergingSelectables.Count))
            || (existingIndex < checked(existingSelectables.Count))
        )
        {
            if (
                (mergingIndex >= checked(mergingSelectables.Count))
                || (
                    (existingIndex < checked(existingSelectables.Count))
                    && (
                        compareOrder(
                            existingSelectables[(int)existingIndex],
                            mergingSelectables[(int)mergingIndex]
                        ) < 0L
                    )
                )
            )
            {
                if (existingIndex == currentSelectionStartIndex)
                {
                    selectionStartIndex = checked(selectables.Count);
                }
                if (existingIndex == currentSelectionEndIndex)
                {
                    selectionEndIndex = checked(selectables.Count);
                }
                selectables.Add(existingSelectables[(int)existingIndex]);
                existingIndex += 1L;
                continue;
            }
            Selectable mergingSelectable = mergingSelectables[(int)mergingIndex];
            if (
                (existingIndex < Math.Max(currentSelectionStartIndex, currentSelectionEndIndex))
                && (existingIndex > Math.Min(currentSelectionStartIndex, currentSelectionEndIndex))
            )
            {
                ensureChildUpdated(mergingSelectable);
            }
            mergingSelectable.addListener(_handleSelectableGeometryChange);
            selectables.Add(mergingSelectable);
            mergingIndex += 1L;
        }
        DartRuntimePrimitives.Assert(() =>
            (mergingIndex == checked(mergingSelectables.Count))
            && (existingIndex == checked(existingSelectables.Count))
            && (checked(selectables.Count) == (existingIndex + mergingIndex))
        );
        DartRuntimePrimitives.Assert(() =>
            (selectionStartIndex >= -1L) || (selectionStartIndex < checked(selectables.Count))
        );
        DartRuntimePrimitives.Assert(() =>
            (selectionEndIndex >= -1L) || (selectionEndIndex < checked(selectables.Count))
        );
        DartRuntimePrimitives.Assert(() =>
            currentSelectionStartIndex == -1L == (selectionStartIndex == -1L)
        );
        DartRuntimePrimitives.Assert(() =>
            currentSelectionEndIndex == -1L == (selectionEndIndex == -1L)
        );
        currentSelectionEndIndex = selectionEndIndex;
        currentSelectionStartIndex = selectionStartIndex;
        _additions = new HashSet<Selectable>();
    }

    internal virtual void _removeSelectable(Selectable selectable)
    {
        DartRuntimePrimitives.Assert(
            () => selectables.Contains(selectable),
            () => (object?)"The selectable is not in this registrar."
        );
        long index = selectables.IndexOf(selectable);
        selectables.removeAt(index);
        if (index <= currentSelectionEndIndex)
        {
            currentSelectionEndIndex -= 1L;
        }
        if (index <= currentSelectionStartIndex)
        {
            currentSelectionStartIndex -= 1L;
        }
        selectable.removeListener(_handleSelectableGeometryChange);
    }

    public virtual void didChangeSelectables()
    {
        _updateSelectionGeometry();
    }

    public override SelectionGeometry value => _selectionGeometry;

    internal virtual void _updateSelectionGeometry()
    {
        SelectionGeometry newValue = getSelectionGeometry();
        if (!Equals(_selectionGeometry, newValue))
        {
            _selectionGeometry = newValue;
            notifyListeners();
        }
        _updateHandleLayersAndOwners();
    }

    internal static Rect _getBoundingBox(Selectable selectable)
    {
        Rect result = selectable.boundingBoxes.First();
        for (var index = 1L; index < checked(selectable.boundingBoxes.Count); index += 1L)
        {
            result = result.expandToInclude(selectable.boundingBoxes[(int)index]);
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Comparison<Selectable> compareOrder =>
        new Comparison<Selectable>((left, right) => checked((int)_compareScreenOrder(left, right)));

    internal static long _compareScreenOrder(Selectable a, Selectable b)
    {
        Rect rectA = MatrixUtils.transformRect(a.getTransformTo(null), _getBoundingBox(a));
        Rect rectB = MatrixUtils.transformRect(b.getTransformTo(null), _getBoundingBox(b));
        long result = _compareVertically(rectA, rectB);
        if (result != 0L)
        {
            return result;
        }
        return _compareHorizontally(rectA, rectB);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static long _compareVertically(Rect a, Rect b)
    {
        if (
            (
                ((a.top - b.top) < Selectable_regionLibrary._kSelectableVerticalComparingThreshold)
                && (
                    (a.bottom - b.bottom)
                    > -Selectable_regionLibrary._kSelectableVerticalComparingThreshold
                )
            )
            || (
                ((b.top - a.top) < Selectable_regionLibrary._kSelectableVerticalComparingThreshold)
                && (
                    (b.bottom - a.bottom)
                    > -Selectable_regionLibrary._kSelectableVerticalComparingThreshold
                )
            )
        )
        {
            return 0L;
        }
        if ((a.top - b.top).abs() > Selectable_regionLibrary._kSelectableVerticalComparingThreshold)
        {
            return (a.top > b.top) ? 1L : -1L;
        }
        return (a.bottom > b.bottom) ? 1L : -1L;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static long _compareHorizontally(Rect a, Rect b)
    {
        if (
            ((a.left - b.left) < Foundation.ConstantsLibrary.precisionErrorTolerance)
            && ((a.right - b.right) > -Foundation.ConstantsLibrary.precisionErrorTolerance)
        )
        {
            return -1L;
        }
        if (
            ((b.left - a.left) < Foundation.ConstantsLibrary.precisionErrorTolerance)
            && ((b.right - a.right) > -Foundation.ConstantsLibrary.precisionErrorTolerance)
        )
        {
            return 1L;
        }
        if ((a.left - b.left).abs() > Foundation.ConstantsLibrary.precisionErrorTolerance)
        {
            return (a.left > b.left) ? 1L : -1L;
        }
        return (a.right > b.right) ? 1L : -1L;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleSelectableGeometryChange()
    {
        if (_isHandlingSelectionEvent)
        {
            return;
        }
        _updateSelectionGeometry();
    }

    public virtual SelectionGeometry getSelectionGeometry()
    {
        if (
            (currentSelectionEndIndex == -1L)
            || (currentSelectionStartIndex == -1L)
            || !Enumerable.Any(selectables)
        )
        {
            return new SelectionGeometry(
                status: SelectionStatus.none,
                hasContent: Enumerable.Any(selectables)
            );
        }
        if (!_extendSelectionInProgress)
        {
            currentSelectionStartIndex = _adjustSelectionIndexBasedOnSelectionGeometry(
                currentSelectionStartIndex,
                currentSelectionEndIndex
            );
            currentSelectionEndIndex = _adjustSelectionIndexBasedOnSelectionGeometry(
                currentSelectionEndIndex,
                currentSelectionStartIndex
            );
        }
        SelectionGeometry startGeometry = selectables[(int)currentSelectionStartIndex].value;
        bool forwardSelection = currentSelectionEndIndex >= currentSelectionStartIndex;
        long startIndexWalker = currentSelectionStartIndex;
        while (
            (startIndexWalker != currentSelectionEndIndex)
            && (startGeometry.startSelectionPoint is null)
        )
        {
            startIndexWalker += forwardSelection ? 1L : -1L;
            startGeometry = selectables[(int)startIndexWalker].value;
        }
        SelectionPoint? startPoint = default!;
        if (startGeometry.startSelectionPoint is not null)
        {
            Matrix4 startTransform = getTransformFrom(selectables[(int)startIndexWalker]);
            Offset start = MatrixUtils.transformPoint(
                startTransform,
                startGeometry.startSelectionPoint!.localPosition
            );
            if (start.isFinite)
            {
                startPoint = new SelectionPoint(
                    localPosition: start,
                    lineHeight: startGeometry.startSelectionPoint!.lineHeight,
                    handleType: startGeometry.startSelectionPoint!.handleType
                );
            }
        }
        SelectionGeometry endGeometry = selectables[(int)currentSelectionEndIndex].value;
        long endIndexWalker = currentSelectionEndIndex;
        while (
            (endIndexWalker != currentSelectionStartIndex)
            && (endGeometry.endSelectionPoint is null)
        )
        {
            endIndexWalker += forwardSelection ? -1L : 1L;
            endGeometry = selectables[(int)endIndexWalker].value;
        }
        SelectionPoint? endPoint = default!;
        if (endGeometry.endSelectionPoint is not null)
        {
            Matrix4 endTransform = getTransformFrom(selectables[(int)endIndexWalker]);
            Offset end = MatrixUtils.transformPoint(
                endTransform,
                endGeometry.endSelectionPoint!.localPosition
            );
            if (end.isFinite)
            {
                endPoint = new SelectionPoint(
                    localPosition: end,
                    lineHeight: endGeometry.endSelectionPoint!.lineHeight,
                    handleType: endGeometry.endSelectionPoint!.handleType
                );
            }
        }
        var selectionRectsLocal = new List<Rect>();
        Rect? drawableArea = (Rect?)
            (object?)(
                hasSize ? Rect.fromLTWH(0, 0, containerSize.width, containerSize.height) : null
            );
        for (long index = currentSelectionStartIndex; index <= currentSelectionEndIndex; index++)
        {
            List<Rect> currSelectableSelectionRects = selectables[(int)index]
                .value.selectionRects.Cast<Rect>()
                .ToList();
            List<Rect> selectionRectsWithinDrawableArea = currSelectableSelectionRects
                .map(
                    (selectionRect) =>
                    {
                        Matrix4 transform = getTransformFrom(selectables[(int)index]);
                        Rect localRect = MatrixUtils.transformRect(transform, selectionRect);
                        return drawableArea?.intersect(localRect) ?? localRect;
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                )
                .where(
                    (selectionRect) =>
                    {
                        return selectionRect.isFinite && !selectionRect.isEmpty;
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                )
                .ToList()
                .Cast<Rect>()
                .ToList();
            selectionRectsLocal.AddRange(selectionRectsWithinDrawableArea.Cast<Rect>());
        }
        return new SelectionGeometry(
            startSelectionPoint: startPoint,
            endSelectionPoint: endPoint,
            selectionRects: selectionRectsLocal,
            status: (!Equals(startGeometry, endGeometry))
                ? SelectionStatus.uncollapsed
                : startGeometry.status,
            hasContent: true
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long _adjustSelectionIndexBasedOnSelectionGeometry(
        long currentIndex,
        long towardIndex
    )
    {
        bool forward = towardIndex > currentIndex;
        while (
            (currentIndex != towardIndex)
            && (!Equals(selectables[(int)currentIndex].value.status, SelectionStatus.uncollapsed))
        )
        {
            currentIndex += forward ? 1L : -1L;
        }
        return currentIndex;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void pushHandleLayers(LayerLink? startHandle, LayerLink? endHandle)
    {
        if (Equals(_startHandleLayer, startHandle) && Equals(_endHandleLayer, endHandle))
        {
            return;
        }
        _startHandleLayer = startHandle;
        _endHandleLayer = endHandle;
        _updateHandleLayersAndOwners();
    }

    internal virtual void _updateHandleLayersAndOwners()
    {
        LayerLink? effectiveStartHandle = _startHandleLayer;
        LayerLink? effectiveEndHandle = _endHandleLayer;
        if ((effectiveStartHandle is not null) || (effectiveEndHandle is not null))
        {
            Rect? drawableArea = (Rect?)
                (object?)(
                    hasSize
                        ? Rect.fromLTWH(0, 0, containerSize.width, containerSize.height)
                            .inflate(_kSelectionHandleDrawableAreaPadding)
                        : null
                );
            bool hideStartHandle =
                (value.startSelectionPoint is null)
                || (drawableArea is null)
                || !DartRuntimePrimitives
                    .RequireValue(drawableArea)
                    .contains(value.startSelectionPoint!.localPosition);
            bool hideEndHandle =
                (value.endSelectionPoint is null)
                || (drawableArea is null)
                || !DartRuntimePrimitives
                    .RequireValue(drawableArea)
                    .contains(value.endSelectionPoint!.localPosition);
            effectiveStartHandle = hideStartHandle ? null : _startHandleLayer;
            effectiveEndHandle = hideEndHandle ? null : _endHandleLayer;
        }
        if ((currentSelectionStartIndex == -1L) || (currentSelectionEndIndex == -1L))
        {
            if (_startHandleLayerOwner is not null)
            {
                _startHandleLayerOwner!.pushHandleLayers(null, null);
                _startHandleLayerOwner = null;
            }
            if (_endHandleLayerOwner is not null)
            {
                _endHandleLayerOwner!.pushHandleLayers(null, null);
                _endHandleLayerOwner = null;
            }
            return;
        }
        if (!Equals(selectables[(int)currentSelectionStartIndex], _startHandleLayerOwner))
        {
            _startHandleLayerOwner?.pushHandleLayers(null, null);
        }
        if (!Equals(selectables[(int)currentSelectionEndIndex], _endHandleLayerOwner))
        {
            _endHandleLayerOwner?.pushHandleLayers(null, null);
        }
        _startHandleLayerOwner = selectables[(int)currentSelectionStartIndex];
        if (currentSelectionStartIndex == currentSelectionEndIndex)
        {
            _endHandleLayerOwner = _startHandleLayerOwner;
            _startHandleLayerOwner!.pushHandleLayers(effectiveStartHandle, effectiveEndHandle);
            return;
        }
        _startHandleLayerOwner!.pushHandleLayers(effectiveStartHandle, null);
        _endHandleLayerOwner = selectables[(int)currentSelectionEndIndex];
        _endHandleLayerOwner!.pushHandleLayers(null, effectiveEndHandle);
    }

    public override SelectedContent? getSelectedContent()
    {
        var selections = new List<SelectedContent>();
        if (!Enumerable.Any(selections))
        {
            return null;
        }
        var buffer = new StringBuffer();
        foreach (var selection in selections)
        {
            buffer.write(selection.plainText);
        }
        return new SelectedContent(plainText: buffer.ToString());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long contentLength =>
        Enumerable.Aggregate(selectables, 0L, (sum, selectable) => sum + selectable.contentLength);

    internal virtual SelectedContentRange? _calculateLocalRange(
        List<(long contentLength, SelectedContentRange? range)> selections
    )
    {
        if ((currentSelectionStartIndex == -1L) || (currentSelectionEndIndex == -1L))
        {
            return null;
        }
        var startOffsetLocal = 0L;
        var endOffsetLocal = 0L;
        var foundStart = false;
        bool forwardSelection = currentSelectionEndIndex >= currentSelectionStartIndex;
        if (currentSelectionEndIndex == currentSelectionStartIndex)
        {
            SelectedContentRange rangeAtSelectableInSelection = selectables[
                (int)currentSelectionStartIndex
            ]
                .getSelection()!;
            forwardSelection =
                rangeAtSelectableInSelection.endOffset >= rangeAtSelectableInSelection.startOffset;
        }
        for (var index = 0L; index < checked(selections.Count); index++)
        {
            (long contentLength, SelectedContentRange? range) selection = selections[(int)index];
            if (selection.range is null)
            {
                if (foundStart)
                {
                    return new SelectedContentRange(
                        startOffset: forwardSelection ? startOffsetLocal : endOffsetLocal,
                        endOffset: forwardSelection ? endOffsetLocal : startOffsetLocal
                    );
                }
                startOffsetLocal += selection.contentLength;
                endOffsetLocal = startOffsetLocal;
                continue;
            }
            long selectionStartNormalized = Math.Min(
                selection.range!.startOffset,
                selection.range!.endOffset
            );
            long selectionEndNormalized = Math.Max(
                selection.range!.startOffset,
                selection.range!.endOffset
            );
            if (!foundStart)
            {
                startOffsetLocal += selectionStartNormalized;
                endOffsetLocal =
                    startOffsetLocal + (selectionEndNormalized - selectionStartNormalized).abs();
                foundStart = true;
            }
            else
            {
                endOffsetLocal += (selectionEndNormalized - selectionStartNormalized).abs();
            }
        }
        DartRuntimePrimitives.Assert(
            () => foundStart,
            () =>
                (object?)
                    "The start of the selection has not been found despite this selection delegate having an existing currentSelectionStartIndex and currentSelectionEndIndex."
        );
        return new SelectedContentRange(
            startOffset: forwardSelection ? startOffsetLocal : endOffsetLocal,
            endOffset: forwardSelection ? endOffsetLocal : startOffsetLocal
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override SelectedContentRange? getSelection()
    {
        var selections = new List<(long contentLength, SelectedContentRange? range)>();
        return _calculateLocalRange(selections);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _flushInactiveSelections()
    {
        if ((currentSelectionStartIndex == -1L) && (currentSelectionEndIndex == -1L))
        {
            return;
        }
        if ((currentSelectionStartIndex == -1L) || (currentSelectionEndIndex == -1L))
        {
            long skipIndexLocal =
                (currentSelectionStartIndex == -1L)
                    ? currentSelectionEndIndex
                    : currentSelectionStartIndex;
            _clearSelectables(skipIndex: DartRuntimePrimitives.RequireValue(skipIndexLocal));
            return;
        }
        long skipStart = Math.Min(currentSelectionStartIndex, currentSelectionEndIndex);
        long skipEnd = Math.Max(currentSelectionStartIndex, currentSelectionEndIndex);
        for (var index = 0L; index < checked(selectables.Count); index += 1L)
        {
            if ((index >= skipStart) && (index <= skipEnd))
            {
                continue;
            }
            dispatchSelectionEventToChild(selectables[(int)index], new ClearSelectionEvent());
        }
    }

    public virtual SelectionResult handleSelectAll(SelectAllSelectionEvent @event)
    {
        foreach (Selectable selectable in selectables)
        {
            dispatchSelectionEventToChild(selectable, @event);
        }
        currentSelectionStartIndex = 0L;
        currentSelectionEndIndex = checked(selectables.Count) - 1L;
        return SelectionResult.none;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _clearSelectables(long? skipIndex = null)
    {
        for (var i = 0L; i < checked(selectables.Count); i++)
        {
            if (i == skipIndex)
            {
                continue;
            }
            dispatchSelectionEventToChild(selectables[(int)i], new ClearSelectionEvent());
        }
    }

    internal virtual SelectionResult _handleSelectBoundary(SelectionEvent @event)
    {
        DartRuntimePrimitives.Assert(
            () => (@event is SelectWordSelectionEvent) || (@event is SelectParagraphSelectionEvent),
            () =>
                (object?)
                    "This method should only be given selection events that select text boundaries."
        );
        Offset effectiveGlobalPosition = @event switch
        {
            SelectWordSelectionEvent
            {
                globalPosition: Offset globalPositionLocal
            } __object119052 => globalPositionLocal,
            SelectParagraphSelectionEvent
            {
                globalPosition: Offset globalPositionAlternate
            } __object119125 => globalPositionAlternate,
            _ => throw DartRuntimePrimitives.AsException(
                new DartArgumentError($"Unsupported selection event: {@event}")
            ),
        };
        SelectionResult? lastSelectionResult = default!;
        double minDistanceSquared = double.PositiveInfinity;
        var nearestIndex = 0L;
        for (var index = 0L; index < checked(selectables.Count); index += 1L)
        {
            var globalRectsContainPosition = false;
            Matrix4 transform = selectables[(int)index].getTransformTo(null);
            foreach (Rect rect in selectables[(int)index].boundingBoxes)
            {
                Rect globalRect = MatrixUtils.transformRect(transform, rect);
                if (globalRect.contains(effectiveGlobalPosition))
                {
                    globalRectsContainPosition = true;
                    break;
                }
                double dxLocal =
                    effectiveGlobalPosition.dx
                    - Dart_uiLibrary.clampDouble(
                        effectiveGlobalPosition.dx,
                        globalRect.left,
                        globalRect.right
                    );
                double dyLocal =
                    effectiveGlobalPosition.dy
                    - Dart_uiLibrary.clampDouble(
                        effectiveGlobalPosition.dy,
                        globalRect.top,
                        globalRect.bottom
                    );
                double distanceSquared = (dxLocal * dxLocal) + (dyLocal * dyLocal);
                if (distanceSquared < minDistanceSquared)
                {
                    minDistanceSquared = distanceSquared;
                    nearestIndex = index;
                }
            }
            if (globalRectsContainPosition)
            {
                SelectionGeometry existingGeometry = selectables[(int)index].value;
                lastSelectionResult = dispatchSelectionEventToChild(
                    selectables[(int)index],
                    @event
                );
                if (
                    (index == (checked(selectables.Count) - 1L))
                    && Equals(
                        DartRuntimePrimitives.RequireValue(lastSelectionResult),
                        SelectionResult.next
                    )
                )
                {
                    return SelectionResult.next;
                }
                if (
                    Equals(
                        DartRuntimePrimitives.RequireValue(lastSelectionResult),
                        SelectionResult.next
                    )
                )
                {
                    continue;
                }
                if (
                    (index == 0L)
                    && Equals(
                        DartRuntimePrimitives.RequireValue(lastSelectionResult),
                        SelectionResult.previous
                    )
                )
                {
                    return SelectionResult.previous;
                }
                if (!Equals(selectables[(int)index].value, existingGeometry))
                {
                    _clearSelectables(skipIndex: index);
                    currentSelectionStartIndex = currentSelectionEndIndex = index;
                }
                return SelectionResult.end;
            }
            else
            {
                if (Equals(lastSelectionResult, SelectionResult.next))
                {
                    currentSelectionStartIndex = currentSelectionEndIndex = index - 1L;
                    return SelectionResult.end;
                }
            }
        }
        DartRuntimePrimitives.Assert(() => lastSelectionResult is null);
        if (Enumerable.Any(selectables))
        {
            SelectionGeometry existingGeometryLocal = selectables[(int)nearestIndex].value;
            dispatchSelectionEventToChild(selectables[(int)nearestIndex], @event);
            if (!Equals(selectables[(int)nearestIndex].value, existingGeometryLocal))
            {
                _clearSelectables(skipIndex: nearestIndex);
                currentSelectionStartIndex = currentSelectionEndIndex = nearestIndex;
            }
        }
        return SelectionResult.end;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SelectionResult handleSelectWord(SelectWordSelectionEvent @event)
    {
        return _handleSelectBoundary(@event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SelectionResult handleSelectParagraph(SelectParagraphSelectionEvent @event)
    {
        return _handleSelectBoundary(@event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SelectionResult handleClearSelection(ClearSelectionEvent @event)
    {
        foreach (Selectable selectable in selectables)
        {
            dispatchSelectionEventToChild(selectable, @event);
        }
        currentSelectionEndIndex = -1L;
        currentSelectionStartIndex = -1L;
        return SelectionResult.none;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SelectionResult handleGranularlyExtendSelection(
        GranularlyExtendSelectionEvent @event
    )
    {
        DartRuntimePrimitives.Assert(() =>
            currentSelectionStartIndex == -1L == (currentSelectionEndIndex == -1L)
        );
        if (currentSelectionStartIndex == -1L)
        {
            if (@event.forward)
            {
                currentSelectionStartIndex = currentSelectionEndIndex = 0L;
            }
            else
            {
                currentSelectionStartIndex = currentSelectionEndIndex =
                    checked(selectables.Count) - 1L;
            }
        }
        long targetIndex = @event.isEnd ? currentSelectionEndIndex : currentSelectionStartIndex;
        SelectionResult result = dispatchSelectionEventToChild(
            selectables[(int)targetIndex],
            @event
        );
        if (@event.forward)
        {
            DartRuntimePrimitives.Assert(() => !Equals(result, SelectionResult.previous));
            while (
                (targetIndex < (checked(selectables.Count) - 1L))
                && Equals(result, SelectionResult.next)
            )
            {
                targetIndex += 1L;
                result = dispatchSelectionEventToChild(selectables[(int)targetIndex], @event);
                DartRuntimePrimitives.Assert(() => !Equals(result, SelectionResult.previous));
            }
        }
        else
        {
            DartRuntimePrimitives.Assert(() => !Equals(result, SelectionResult.next));
            while ((targetIndex > 0L) && Equals(result, SelectionResult.previous))
            {
                targetIndex -= 1L;
                result = dispatchSelectionEventToChild(selectables[(int)targetIndex], @event);
                DartRuntimePrimitives.Assert(() => !Equals(result, SelectionResult.next));
            }
        }
        if (@event.isEnd)
        {
            currentSelectionEndIndex = targetIndex;
        }
        else
        {
            currentSelectionStartIndex = targetIndex;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SelectionResult handleDirectionallyExtendSelection(
        DirectionallyExtendSelectionEvent @event
    )
    {
        DartRuntimePrimitives.Assert(() =>
            currentSelectionStartIndex == -1L == (currentSelectionEndIndex == -1L)
        );
        if (currentSelectionStartIndex == -1L)
        {
            currentSelectionStartIndex = currentSelectionEndIndex = @event.direction switch
            {
                SelectionExtendDirection.previousLine => checked(selectables.Count) - 1L,
                SelectionExtendDirection.backward => checked(selectables.Count) - 1L,
                SelectionExtendDirection.nextLine => 0L,
                SelectionExtendDirection.forward => 0L,
                _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
            };
        }
        long targetIndex = @event.isEnd ? currentSelectionEndIndex : currentSelectionStartIndex;
        SelectionResult result = dispatchSelectionEventToChild(
            selectables[(int)targetIndex],
            @event
        );
        switch (@event.direction)
        {
            case SelectionExtendDirection.previousLine:
            {
                DartRuntimePrimitives.Assert(() =>
                    Equals(result, SelectionResult.end) || Equals(result, SelectionResult.previous)
                );
                if (Equals(result, SelectionResult.previous))
                {
                    if (targetIndex > 0L)
                    {
                        targetIndex -= 1L;
                        result = dispatchSelectionEventToChild(
                            selectables[(int)targetIndex],
                            @event.copyWith(direction: SelectionExtendDirection.backward)
                        );
                        DartRuntimePrimitives.Assert(() => Equals(result, SelectionResult.end));
                    }
                }
                break;
            }
            case SelectionExtendDirection.nextLine:
            {
                DartRuntimePrimitives.Assert(() =>
                    Equals(result, SelectionResult.end) || Equals(result, SelectionResult.next)
                );
                if (Equals(result, SelectionResult.next))
                {
                    if (targetIndex < (checked(selectables.Count) - 1L))
                    {
                        targetIndex += 1L;
                        result = dispatchSelectionEventToChild(
                            selectables[(int)targetIndex],
                            @event.copyWith(direction: SelectionExtendDirection.forward)
                        );
                        DartRuntimePrimitives.Assert(() => Equals(result, SelectionResult.end));
                    }
                }
                break;
            }
            case SelectionExtendDirection.forward:
            case SelectionExtendDirection.backward:
            {
                DartRuntimePrimitives.Assert(() => Equals(result, SelectionResult.end));
                break;
            }
        }
        if (@event.isEnd)
        {
            currentSelectionEndIndex = targetIndex;
        }
        else
        {
            currentSelectionStartIndex = targetIndex;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SelectionResult handleSelectionEdgeUpdate(SelectionEdgeUpdateEvent @event)
    {
        if (Equals(@event.type, SelectionEventType.endEdgeUpdate))
        {
            return (currentSelectionEndIndex == -1L)
                ? _initSelection(@event, isEnd: true)
                : _adjustSelection(@event, isEnd: true);
        }
        return (currentSelectionStartIndex == -1L)
            ? _initSelection(@event, isEnd: false)
            : _adjustSelection(@event, isEnd: false);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override SelectionResult dispatchSelectionEvent(SelectionEvent @event)
    {
        var selectionWillBeInProgress = @event is not ClearSelectionEvent;
        if (!_selectionInProgress && selectionWillBeInProgress)
        {
            selectables.sort(compareOrder);
        }
        _selectionInProgress = selectionWillBeInProgress;
        _isHandlingSelectionEvent = true;
        SelectionResult result = default!;
        switch (@event.type)
        {
            case SelectionEventType.startEdgeUpdate:
            case SelectionEventType.endEdgeUpdate:
            {
                _extendSelectionInProgress = false;
                result = handleSelectionEdgeUpdate(((SelectionEdgeUpdateEvent?)@event)!);
                break;
            }
            case SelectionEventType.clear:
            {
                _extendSelectionInProgress = false;
                result = handleClearSelection(((ClearSelectionEvent?)@event)!);
                break;
            }
            case SelectionEventType.selectAll:
            {
                _extendSelectionInProgress = false;
                result = handleSelectAll(((SelectAllSelectionEvent?)@event)!);
                break;
            }
            case SelectionEventType.selectWord:
            {
                _extendSelectionInProgress = false;
                result = handleSelectWord(((SelectWordSelectionEvent?)@event)!);
                break;
            }
            case SelectionEventType.selectParagraph:
            {
                _extendSelectionInProgress = false;
                result = handleSelectParagraph(((SelectParagraphSelectionEvent?)@event)!);
                break;
            }
            case SelectionEventType.granularlyExtendSelection:
            {
                _extendSelectionInProgress = true;
                result = handleGranularlyExtendSelection(
                    ((GranularlyExtendSelectionEvent?)@event)!
                );
                break;
            }
            case SelectionEventType.directionallyExtendSelection:
            {
                _extendSelectionInProgress = true;
                result = handleDirectionallyExtendSelection(
                    ((DirectionallyExtendSelectionEvent?)@event)!
                );
                break;
            }
        }
        _isHandlingSelectionEvent = false;
        _updateSelectionGeometry();
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        foreach (Selectable selectable in selectables)
        {
            selectable.removeListener(_handleSelectableGeometryChange);
        }
        selectables = new List<Selectable>();
        _scheduledSelectableUpdate = false;
        base.dispose();
    }

    public abstract void ensureChildUpdated(Selectable selectable);

    public virtual SelectionResult dispatchSelectionEventToChild(
        Selectable selectable,
        SelectionEvent @event
    )
    {
        return selectable.dispatchSelectionEvent(@event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _initSelection(SelectionEdgeUpdateEvent @event, bool isEnd)
    {
        DartRuntimePrimitives.Assert(() =>
            (isEnd && (currentSelectionEndIndex == -1L))
            || (!isEnd && (currentSelectionStartIndex == -1L))
        );
        var newIndex = -1L;
        var hasFoundEdgeIndex = false;
        SelectionResult? result = default!;
        bool? forward = default!;
        long oppositeEdgeIndex = isEnd ? currentSelectionStartIndex : currentSelectionEndIndex;
        long index = Math.Max(oppositeEdgeIndex, 0L);
        while ((index >= 0L) && (index < checked(selectables.Count)))
        {
            Selectable child = selectables[(int)index];
            SelectionResult childResult = dispatchSelectionEventToChild(child, @event);
            switch (childResult)
            {
                case SelectionResult.next:
                {
                    if (forward == false)
                    {
                        hasFoundEdgeIndex = true;
                        result = SelectionResult.end;
                    }
                    else
                    {
                        forward = true;
                        newIndex = index;
                    }
                    break;
                }
                case SelectionResult.none:
                {
                    newIndex = index;
                    break;
                }
                case SelectionResult.end:
                {
                    newIndex = index;
                    result = SelectionResult.end;
                    hasFoundEdgeIndex = true;
                    break;
                }
                case SelectionResult.previous:
                {
                    if (index == 0L)
                    {
                        hasFoundEdgeIndex = true;
                        newIndex = 0L;
                        result = SelectionResult.previous;
                        break;
                    }
                    if (forward ?? false)
                    {
                        hasFoundEdgeIndex = true;
                        result = SelectionResult.end;
                    }
                    else
                    {
                        forward = false;
                        newIndex = index;
                    }
                    break;
                }
                case SelectionResult.pending:
                {
                    newIndex = index;
                    result = SelectionResult.pending;
                    hasFoundEdgeIndex = true;
                    break;
                }
            }
            if (hasFoundEdgeIndex)
            {
                break;
            }
            index += (forward ?? true) ? 1L : -1L;
        }
        if (newIndex == -1L)
        {
            DartRuntimePrimitives.Assert(() => !Enumerable.Any(selectables));
            return SelectionResult.none;
        }
        if (isEnd)
        {
            currentSelectionEndIndex = newIndex;
        }
        else
        {
            currentSelectionStartIndex = newIndex;
        }
        _flushInactiveSelections();
        return result ?? SelectionResult.next;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _adjustSelection(SelectionEdgeUpdateEvent @event, bool isEnd)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (isEnd)
            {
                DartRuntimePrimitives.Assert(() =>
                    (currentSelectionEndIndex < checked(selectables.Count))
                    && (currentSelectionEndIndex >= 0L)
                );
                return true;
            }
            DartRuntimePrimitives.Assert(() =>
                (currentSelectionStartIndex < checked(selectables.Count))
                && (currentSelectionStartIndex >= 0L)
            );
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        SelectionResult? finalResult = default!;
        var isCurrentEdgeWithinViewport = isEnd
            ? (_selectionGeometry.endSelectionPoint is not null)
            : (_selectionGeometry.startSelectionPoint is not null);
        var isOppositeEdgeWithinViewport = isEnd
            ? (_selectionGeometry.startSelectionPoint is not null)
            : (_selectionGeometry.endSelectionPoint is not null);
        long newIndex = (isEnd, isCurrentEdgeWithinViewport, isOppositeEdgeWithinViewport) switch
        {
            (true, true, true) => currentSelectionEndIndex,
            (true, true, false) => currentSelectionEndIndex,
            (true, false, true) => currentSelectionStartIndex,
            (true, false, false) => 0L,
            (false, true, true) => currentSelectionStartIndex,
            (false, true, false) => currentSelectionStartIndex,
            (false, false, true) => currentSelectionEndIndex,
            (false, false, false) => 0L,
        };
        bool? forward = default!;
        SelectionResult currentSelectableResult = default!;
        while ((newIndex < checked(selectables.Count)) && (newIndex >= 0L) && (finalResult is null))
        {
            currentSelectableResult = dispatchSelectionEventToChild(
                selectables[(int)newIndex],
                @event
            );
            switch (currentSelectableResult)
            {
                case SelectionResult.end:
                case SelectionResult.pending:
                case SelectionResult.none:
                {
                    finalResult = currentSelectableResult;
                    break;
                }
                case SelectionResult.next:
                {
                    if (forward == false)
                    {
                        newIndex += 1L;
                        finalResult = SelectionResult.end;
                    }
                    else
                    {
                        if (newIndex == (checked(selectables.Count) - 1L))
                        {
                            finalResult = currentSelectableResult;
                        }
                        else
                        {
                            forward = true;
                            newIndex += 1L;
                        }
                    }
                    break;
                }
                case SelectionResult.previous:
                {
                    if (forward ?? false)
                    {
                        newIndex -= 1L;
                        finalResult = SelectionResult.end;
                    }
                    else
                    {
                        if (newIndex == 0L)
                        {
                            finalResult = currentSelectableResult;
                        }
                        else
                        {
                            forward = false;
                            newIndex -= 1L;
                        }
                    }
                    break;
                }
            }
        }
        if (isEnd)
        {
            currentSelectionEndIndex = newIndex;
        }
        else
        {
            currentSelectionStartIndex = newIndex;
        }
        _flushInactiveSelections();
        return DartRuntimePrimitives.RequireValue(finalResult);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal delegate void _SelectionInfo__selectable_region();

public delegate Widget SelectableRegionContextMenuBuilder(
    BuildContext context,
    SelectableRegionState selectableRegionState
);

public enum SelectableRegionSelectionStatus
{
    changing,
    finalized,
}

internal class _SelectableRegionSelectionStatusNotifier__selectable_region
    : ChangeNotifier,
        ValueListenable<SelectableRegionSelectionStatus>
{
    internal virtual SelectableRegionSelectionStatus _selectableRegionSelectionStatus { get; set; } =
        SelectableRegionSelectionStatus.finalized;

    internal _SelectableRegionSelectionStatusNotifier__selectable_region() { }

    public virtual SelectableRegionSelectionStatus value
    {
        get => _selectableRegionSelectionStatus;
        set
        {
            var newStatus = value;
            DartRuntimePrimitives.Assert(
                () =>
                    (
                        Equals(newStatus, SelectableRegionSelectionStatus.finalized)
                        && Equals(this.value, SelectableRegionSelectionStatus.changing)
                    ) || Equals(newStatus, SelectableRegionSelectionStatus.changing),
                () => (object?)"Attempting to finalize the selection when it is already finalized."
            );
            _selectableRegionSelectionStatus = newStatus;
            notifyListeners();
        }
    }
}

public class SelectableRegionSelectionStatusScope : InheritedWidget
{
    public virtual ValueListenable<SelectableRegionSelectionStatus> selectionStatusNotifier
    {
        get;
        private set;
    } = default!;

    public SelectableRegionSelectionStatusScope(
        ValueListenable<SelectableRegionSelectionStatus> selectionStatusNotifier,
        Widget child
    )
        : base(child: child)
    {
        this.selectionStatusNotifier = selectionStatusNotifier;
    }

    public static ValueListenable<SelectableRegionSelectionStatus>? maybeOf(BuildContext context)
    {
        return context
            .dependOnInheritedWidgetOfExactType<SelectableRegionSelectionStatusScope>()
            ?.selectionStatusNotifier;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (SelectableRegionSelectionStatusScope)oldWidget;
        return !Equals(selectionStatusNotifier, __oldWidget.selectionStatusNotifier);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class SelectionListener : StatefulWidget
{
    public virtual SelectionListenerNotifier selectionNotifier { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public SelectionListener(
        Key? key = null,
        SelectionListenerNotifier selectionNotifier = default!,
        Widget child = default!
    )
        : base(key: key)
    {
        this.selectionNotifier = selectionNotifier;
        this.child = child;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _SelectionListenerState__selectable_region()
        );
}

internal class _SelectionListenerState__selectable_region : State<SelectionListener>
{
    private bool __late__selectionDelegate_initialized;
    private _SelectionListenerDelegate__selectable_region __late__selectionDelegate = default!;
    internal virtual _SelectionListenerDelegate__selectable_region _selectionDelegate
    {
        get
        {
            if (!__late__selectionDelegate_initialized)
            {
                __late__selectionDelegate = new _SelectionListenerDelegate__selectable_region(
                    selectionNotifier: widget.selectionNotifier
                );
                __late__selectionDelegate_initialized = true;
            }
            return __late__selectionDelegate;
        }
    }

    public override void didUpdateWidget(SelectionListener oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(oldWidget.selectionNotifier, widget.selectionNotifier))
        {
            _selectionDelegate._setNotifier(widget.selectionNotifier);
        }
    }

    public override void dispose()
    {
        _selectionDelegate.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return new SelectionContainer(@delegate: _selectionDelegate, child: widget.child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _SelectionListenerDelegate__selectable_region
    : StaticSelectionContainerDelegate,
        SelectionDetails
{
    internal virtual SelectionGeometry? _initialSelectionGeometry { get; set; } = default;
    internal virtual SelectionListenerNotifier _selectionNotifier { get; set; } = default!;

    internal _SelectionListenerDelegate__selectable_region(
        SelectionListenerNotifier selectionNotifier
    )
    {
        _selectionNotifier = selectionNotifier;
    }

    internal virtual void _setNotifier(SelectionListenerNotifier newNotifier)
    {
        _selectionNotifier._unregisterSelectionListenerDelegate();
        _selectionNotifier = newNotifier;
        _selectionNotifier._registerSelectionListenerDelegate(this);
    }

    public override void notifyListeners()
    {
        base.notifyListeners();
        if ((_initialSelectionGeometry is null) && !value.hasSelection)
        {
            _initialSelectionGeometry = value;
            return;
        }
        _selectionNotifier.notifyListeners();
    }

    public override void dispose()
    {
        _selectionNotifier._unregisterSelectionListenerDelegate();
        _initialSelectionGeometry = null;
        base.dispose();
    }

    public virtual SelectedContentRange? range => getSelection();
    public virtual SelectionStatus status => value.status;
}

public interface SelectionDetails
{
    public SelectedContentRange? range { get; }
    public SelectionStatus status { get; }
}

public class SelectionListenerNotifier : ChangeNotifier
{
    internal virtual _SelectionListenerDelegate__selectable_region? _selectionDelegate { get; set; } =
        default;

    public virtual SelectionDetails selection =>
        DartRuntimePrimitives.ConvertValue<SelectionDetails>(
            _selectionDelegate
                ?? throw new Exception("Selection client has not been registered to this notifier.")
        );
    public virtual bool registered =>
        DartRuntimePrimitives.ConvertValue<bool>(_selectionDelegate is not null);

    internal virtual void _registerSelectionListenerDelegate(
        _SelectionListenerDelegate__selectable_region selectionDelegate
    )
    {
        DartRuntimePrimitives.Assert(
            () => !registered,
            () =>
                (object?)
                    "This SelectionListenerNotifier is already registered to another SelectionListener. Try providing a new SelectionListenerNotifier."
        );
        _selectionDelegate = selectionDelegate;
    }

    internal virtual void _unregisterSelectionListenerDelegate()
    {
        _selectionDelegate = null;
    }

    public override void dispose()
    {
        _unregisterSelectionListenerDelegate();
        base.dispose();
    }

    public override void addListener(Action listener)
    {
        base.addListener(listener);
    }
}
