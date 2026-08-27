// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/selectable_region.dart
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

public static partial class Selectable_regionLibrary
{
    internal static HashSet<PointerDeviceKind> _kLongPressSelectionDevices = new HashSet<PointerDeviceKind> { PointerDeviceKind.touch, PointerDeviceKind.stylus, PointerDeviceKind.invertedStylus };
}

public static partial class Selectable_regionLibrary
{
    internal static double _kSelectableVerticalComparingThreshold = 3.0;
}

public class SelectableRegion : StatefulWidget
{
    public virtual TextMagnifierConfiguration magnifierConfiguration { get; private set; } = default!;
    public virtual FocusNode? focusNode { get; private set; }
    public virtual Widget child { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, SelectableRegionState, Widget>? contextMenuBuilder { get; private set; }
    public virtual TextSelectionControls selectionControls { get; private set; } = default!;
    public virtual global::System.Action<global::Doroti.Framework.Rendering.SelectedContent?>? onSelectionChanged { get; private set; }

    public SelectableRegion(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, SelectableRegionState, Widget>? contextMenuBuilder = null, FocusNode? focusNode = null, TextMagnifierConfiguration magnifierConfiguration = default!, global::System.Action<global::Doroti.Framework.Rendering.SelectedContent?>? onSelectionChanged = null, TextSelectionControls selectionControls = default!, Widget child = default!) : base(key: key)
    {
        TextMagnifierConfiguration __magnifierConfiguration = magnifierConfiguration ?? TextMagnifierConfiguration.disabled;
        this.contextMenuBuilder = contextMenuBuilder;
        this.focusNode = focusNode;
        this.magnifierConfiguration = __magnifierConfiguration;
        this.onSelectionChanged = onSelectionChanged;
        this.selectionControls = selectionControls;
        this.child = child;
    }

    public static List<ContextMenuButtonItem> getSelectableButtonItems(global::Doroti.Framework.Rendering.SelectionGeometry selectionGeometry, global::System.Action onCopy, global::System.Action onSelectAll, global::System.Action? onShare)
    {
        var canCopy = (object.Equals(((global::Doroti.Framework.Rendering.SelectionGeometry)selectionGeometry).status, global::Doroti.Framework.Rendering.SelectionStatus.uncollapsed));
        bool canSelectAll = ((global::Doroti.Framework.Rendering.SelectionGeometry)selectionGeometry).hasContent;
        bool platformCanShare = (!global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb && (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform switch { global::Doroti.Framework.Foundation.TargetPlatform.android => (object.Equals(((global::Doroti.Framework.Rendering.SelectionGeometry)selectionGeometry).status, global::Doroti.Framework.Rendering.SelectionStatus.uncollapsed)), global::Doroti.Framework.Foundation.TargetPlatform.macOS or global::Doroti.Framework.Foundation.TargetPlatform.fuchsia or global::Doroti.Framework.Foundation.TargetPlatform.linux => false, global::Doroti.Framework.Foundation.TargetPlatform.windows => false, global::Doroti.Framework.Foundation.TargetPlatform.iOS => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        bool canShare = ((onShare is not null) && platformCanShare);
        var showShareBeforeSelectAll = (object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.android));
        return new List<ContextMenuButtonItem>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new SelectableRegionState());
}

public class SelectableRegionState : State<SelectableRegion>, global::Doroti.Framework.Services.TextSelectionDelegate, global::Doroti.Framework.Rendering.SelectionRegistrar
{
    private bool __late__actions_initialized;
    private DartMap<Type, dynamic> __late__actions = default!;
    internal virtual DartMap<Type, dynamic> _actions
    {
        get
        {
            if (!__late__actions_initialized)
            {
                __late__actions = new DartMap<Type, dynamic> { [typeof(SelectAllTextIntent)] = _makeOverridable(new _SelectAllAction__selectable_region(this)), [typeof(CopySelectionTextIntent)] = _makeOverridable(new _CopySelectionAction__selectable_region(this)), [typeof(ExtendSelectionToNextWordBoundaryOrCaretLocationIntent)] = _makeOverridable(new _GranularlyExtendSelectionAction__selectable_region<ExtendSelectionToNextWordBoundaryOrCaretLocationIntent>(this, granularity: global::Doroti.Framework.Rendering.TextGranularity.word)), [typeof(ExpandSelectionToDocumentBoundaryIntent)] = _makeOverridable(new _GranularlyExtendSelectionAction__selectable_region<ExpandSelectionToDocumentBoundaryIntent>(this, granularity: global::Doroti.Framework.Rendering.TextGranularity.document)), [typeof(ExpandSelectionToLineBreakIntent)] = _makeOverridable(new _GranularlyExtendSelectionAction__selectable_region<ExpandSelectionToLineBreakIntent>(this, granularity: global::Doroti.Framework.Rendering.TextGranularity.line)), [typeof(ExtendSelectionByCharacterIntent)] = _makeOverridable(new _GranularlyExtendCaretSelectionAction__selectable_region<ExtendSelectionByCharacterIntent>(this, granularity: global::Doroti.Framework.Rendering.TextGranularity.character)), [typeof(ExtendSelectionToNextWordBoundaryIntent)] = _makeOverridable(new _GranularlyExtendCaretSelectionAction__selectable_region<ExtendSelectionToNextWordBoundaryIntent>(this, granularity: global::Doroti.Framework.Rendering.TextGranularity.word)), [typeof(ExtendSelectionToLineBreakIntent)] = _makeOverridable(new _GranularlyExtendCaretSelectionAction__selectable_region<ExtendSelectionToLineBreakIntent>(this, granularity: global::Doroti.Framework.Rendering.TextGranularity.line)), [typeof(ExtendSelectionVerticallyToAdjacentLineIntent)] = _makeOverridable(new _DirectionallyExtendCaretSelectionAction__selectable_region<ExtendSelectionVerticallyToAdjacentLineIntent>(this)), [typeof(ExtendSelectionToDocumentBoundaryIntent)] = _makeOverridable(new _GranularlyExtendCaretSelectionAction__selectable_region<ExtendSelectionToDocumentBoundaryIntent>(this, granularity: global::Doroti.Framework.Rendering.TextGranularity.document)), [typeof(DismissIntent)] = new CallbackAction<DismissIntent>(onInvoke: (global::System.Func<DismissIntent, object?>)this._hideToolbarIfVisible) };
                __late__actions_initialized = true;
            }
            return __late__actions;
        }
    }
    internal virtual DartMap<Type, dynamic> _gestureRecognizers { get; private set; } = new DartMap<Type, dynamic>();
    internal virtual SelectionOverlay? _selectionOverlay { get; set; } = default;
    internal virtual global::Doroti.Framework.Rendering.LayerLink _startHandleLayerLink { get; private set; } = new global::Doroti.Framework.Rendering.LayerLink();
    internal virtual global::Doroti.Framework.Rendering.LayerLink _endHandleLayerLink { get; private set; } = new global::Doroti.Framework.Rendering.LayerLink();
    internal virtual global::Doroti.Framework.Rendering.LayerLink _toolbarLayerLink { get; private set; } = new global::Doroti.Framework.Rendering.LayerLink();
    internal virtual StaticSelectionContainerDelegate _selectionDelegate { get; private set; } = new StaticSelectionContainerDelegate();
    internal virtual global::Doroti.Framework.Rendering.Selectable? _selectable { get; set; } = default;
    internal virtual Orientation? _lastOrientation { get; set; } = default;
    internal virtual global::Doroti.Framework.Rendering.SelectedContent? _lastSelectedContent { get; set; } = default;
    internal virtual global::Doroti.Framework.Services.ProcessTextService _processTextService { get; private set; } = ((global::Doroti.Framework.Services.ProcessTextService)(object?)new global::Doroti.Framework.Services.DefaultProcessTextService());
    internal virtual List<global::Doroti.Framework.Services.ProcessTextAction> _processTextActions { get; private set; } = new List<global::Doroti.Framework.Services.ProcessTextAction>();
    internal virtual FocusNode? _localFocusNode { get; set; } = default;
    internal virtual _SelectableRegionSelectionStatusNotifier__selectable_region _selectionStatusNotifier { get; private set; } = new _SelectableRegionSelectionStatusNotifier__selectable_region();
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
    public virtual global::Doroti.Framework.Services.TextEditingValue textEditingValue { get; set; } = new global::Doroti.Framework.Services.TextEditingValue(text: "_");

    internal virtual bool _hasSelectionOverlayGeometry => DartRuntimePrimitives.ConvertValue<bool>(((this._selectionDelegate.value.startSelectionPoint is not null) || (this._selectionDelegate.value.endSelectionPoint is not null)));
    internal virtual bool _webContextMenuEnabled => DartRuntimePrimitives.ConvertValue<bool>((((global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb && global::Doroti.Framework.Services.BrowserContextMenu.enabled) && (!object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.android))) && (!object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS))));
    public virtual SelectionOverlay? selectionOverlay => this._selectionOverlay;
    internal virtual FocusNode _focusNode => DartRuntimePrimitives.ConvertValue<FocusNode>((((SelectableRegion)(object)this.widget).focusNode ?? (_localFocusNode ??= new FocusNode(debugLabel: "SelectableRegion"))));
    public override void initState()
    {
        base.initState();
        this._focusNode.addListener(() => this._handleFocusChanged());
        _initMouseGestureRecognizer();
        _initTouchGestureRecognizer();
        this._gestureRecognizers[typeof(global::Doroti.Framework.Gestures.TapGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.TapGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.TapGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.TapGestureRecognizer(debugOwner: this))), ((global::System.Action<global::Doroti.Framework.Gestures.TapGestureRecognizer>)((instance) =>
        {
            instance.onSecondaryTapDown = (global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>)this._handleRightClickDown;
        })));
        DartRuntimePrimitives.Ignore(_initProcessTextActions());
    }

    internal async virtual Future _initProcessTextActions()
    {
        this._processTextActions.Clear();
        this._processTextActions.AddRange((await this._processTextService.queryTextActions()).Cast<global::Doroti.Framework.Services.ProcessTextAction>());
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    return;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        Orientation orientation = MediaQuery.orientationOf(this.context);
        if ((this._lastOrientation is null))
        {
            _lastOrientation = orientation;
            return;
        }
        if ((!object.Equals(orientation, this._lastOrientation)))
        {
            _lastOrientation = orientation;
            hideToolbar((object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.android)));
        }
    }

    public override void didUpdateWidget(SelectableRegion oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((SelectableRegion)(object)this.widget).focusNode, ((SelectableRegion)oldWidget).focusNode)))
        {
            if (((((SelectableRegion)oldWidget).focusNode is null) && (((SelectableRegion)(object)this.widget).focusNode is not null)))
            {
                this._localFocusNode?.removeListener(() => this._handleFocusChanged());
                this._localFocusNode?.dispose();
                _localFocusNode = null;
            }
            else
            {
                if (((((SelectableRegion)(object)this.widget).focusNode is null) && (((SelectableRegion)oldWidget).focusNode is not null)))
                {
                    ((SelectableRegion)oldWidget).focusNode!.removeListener(() => this._handleFocusChanged());
                }
            }
            this._focusNode.addListener(() => this._handleFocusChanged());
            if ((((FocusNode)this._focusNode).hasFocus != ((SelectableRegion)oldWidget).focusNode?.hasFocus))
            {
                _handleFocusChanged();
            }
        }
    }

    internal virtual Action<T> _makeOverridable<T>(Action<T> defaultAction) where T : Intent
    {
        return Action<T>.CreateOverridable(context: this.context, defaultAction: defaultAction);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleFocusChanged()
    {
        if (!((FocusNode)this._focusNode).hasFocus)
        {
            if (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb)
            {
                PlatformSelectableRegionContextMenuIo.detach(this._selectionDelegate);
            }
            if ((object.Equals(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.lifecycleState, AppLifecycleState.resumed)))
            {
                clearSelection();
                this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                _finalizeSelectableRegionStatus();
            }
        }
        else
        {
            if (this._webContextMenuEnabled)
            {
                PlatformSelectableRegionContextMenuIo.attach(this._selectionDelegate);
            }
        }
    }

    internal virtual void _updateSelectionStatus()
    {
        global::Doroti.Framework.Rendering.SelectionGeometry geometry = this._selectionDelegate.value;
        global::Doroti.Framework.Services.TextSelection selectionLocal = (((global::Doroti.Framework.Rendering.SelectionGeometry)geometry).status switch { global::Doroti.Framework.Rendering.SelectionStatus.uncollapsed => new global::Doroti.Framework.Services.TextSelection(baseOffset: 0L, extentOffset: 1L), global::Doroti.Framework.Rendering.SelectionStatus.collapsed => new global::Doroti.Framework.Services.TextSelection(baseOffset: 0L, extentOffset: 1L), global::Doroti.Framework.Rendering.SelectionStatus.none => global::Doroti.Framework.Services.TextSelection.CreateCollapsed(offset: 1L), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        textEditingValue = new global::Doroti.Framework.Services.TextEditingValue(text: "__", selection: selectionLocal);
        if (this._hasSelectionOverlayGeometry)
        {
            _updateSelectionOverlay();
        }
        else
        {
            this._selectionOverlay?.dispose();
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
        if ((!object.Equals(((_SelectableRegionSelectionStatusNotifier__selectable_region)this._selectionStatusNotifier).value, SelectableRegionSelectionStatus.changing)))
        {
            return;
        }
        this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.finalized;
    }

    internal virtual long _getEffectiveConsecutiveTapCount(long rawCount)
    {
        var maxConsecutiveTap = 3L;
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                {
                    if (((this._lastPointerDeviceKind is not null) && (!object.Equals(this._lastPointerDeviceKind, PointerDeviceKind.mouse))))
                    {
                        maxConsecutiveTap = 2L;
                    }
                    return ((rawCount <= maxConsecutiveTap) ? rawCount : ((((rawCount % maxConsecutiveTap) == 0L) ? maxConsecutiveTap : (rawCount % maxConsecutiveTap))));
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                {
                    return ((rawCount <= maxConsecutiveTap) ? rawCount : ((((rawCount % maxConsecutiveTap) == 0L) ? maxConsecutiveTap : (rawCount % maxConsecutiveTap))));
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
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
        this._gestureRecognizers[typeof(global::Doroti.Framework.Gestures.TapAndPanGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.TapAndPanGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.TapAndPanGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.TapAndPanGestureRecognizer(debugOwner: this, supportedDevices: new HashSet<PointerDeviceKind> { PointerDeviceKind.mouse }))), ((global::System.Action<global::Doroti.Framework.Gestures.TapAndPanGestureRecognizer>)((instance) =>
        {
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.TapAndPanGestureRecognizer>)(() =>
            {
                var __cascade = instance;
                __cascade.onTapTrackStart = this._onTapTrackStart;
                __cascade.onTapTrackReset = this._onTapTrackReset;
                __cascade.onTapDown = this._startNewMouseSelectionGesture;
                __cascade.onTapUp = this._handleMouseTapUp;
                __cascade.onDragStart = this._handleMouseDragStart;
                __cascade.onDragUpdate = this._handleMouseDragUpdate;
                __cascade.onDragEnd = this._handleMouseDragEnd;
                __cascade.onCancel = this.clearSelection;
                __cascade.dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.down;
                return __cascade;
            }))());
        })));
    }

    internal virtual void _onTapTrackStart()
    {
        _isShiftPressed = System.Linq.Enumerable.Any(global::Doroti.Framework.Services.HardwareKeyboard.instance.logicalKeysPressed.intersection(new HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> { global::Doroti.Framework.Services.LogicalKeyboardKey.shiftLeft, global::Doroti.Framework.Services.LogicalKeyboardKey.shiftRight }));
    }

    internal virtual void _onTapTrackReset()
    {
        _isShiftPressed = false;
    }

    internal virtual void _initTouchGestureRecognizer()
    {
        this._gestureRecognizers[typeof(global::Doroti.Framework.Gestures.TapAndHorizontalDragGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.TapAndHorizontalDragGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.TapAndHorizontalDragGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.TapAndHorizontalDragGestureRecognizer(debugOwner: this, supportedDevices: System.Enum.GetValues<PointerDeviceKind>().ToList().where(((device) =>
        {
            return (!object.Equals(device, PointerDeviceKind.mouse));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })).toSet()))), ((global::System.Action<global::Doroti.Framework.Gestures.TapAndHorizontalDragGestureRecognizer>)((instance) =>
        {
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.TapAndHorizontalDragGestureRecognizer>)(() =>
            {
                var __cascade = instance;
                __cascade.eagerVictoryOnDrag = (!object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS));
                __cascade.onTapDown = this._startNewMouseSelectionGesture;
                __cascade.onTapUp = this._handleMouseTapUp;
                __cascade.onDragStart = this._handleMouseDragStart;
                __cascade.onDragUpdate = this._handleMouseDragUpdate;
                __cascade.onDragEnd = this._handleMouseDragEnd;
                __cascade.onCancel = this.clearSelection;
                __cascade.dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.down;
                return __cascade;
            }))());
        })));
        this._gestureRecognizers[typeof(global::Doroti.Framework.Gestures.LongPressGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.LongPressGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.LongPressGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.LongPressGestureRecognizer(debugOwner: this, supportedDevices: Selectable_regionLibrary._kLongPressSelectionDevices))), ((global::System.Action<global::Doroti.Framework.Gestures.LongPressGestureRecognizer>)((instance) =>
        {
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.LongPressGestureRecognizer>)(() =>
            {
                var __cascade = instance;
                __cascade.onLongPressStart = this._handleTouchLongPressStart;
                __cascade.onLongPressMoveUpdate = this._handleTouchLongPressMoveUpdate;
                __cascade.onLongPressEnd = this._handleTouchLongPressEnd;
                return __cascade;
            }))());
        })));
    }

    internal virtual void _startNewMouseSelectionGesture(global::Doroti.Framework.Gestures.TapDragDownDetails details)
    {
        _lastPointerDeviceKind = ((global::Doroti.Framework.Gestures.TapDragDownDetails)details).kind;
        switch (_getEffectiveConsecutiveTapCount(((global::Doroti.Framework.Gestures.TapDragDownDetails)details).consecutiveTapCount))
        {
            case 1L:
                {
                    this._focusNode.requestFocus();
                    switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
                    {
                        case global::Doroti.Framework.Foundation.TargetPlatform.android:
                        case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                        case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                            {
                                break;
                            }
                        case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                        case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                        case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                            {
                                hideToolbar();
                                bool isShiftPressedValid = (this._isShiftPressed && (this._selectionDelegate.value.startSelectionPoint is not null));
                                if (isShiftPressedValid)
                                {
                                    _selectEndTo(offset: ((global::Doroti.Framework.Gestures.TapDragDownDetails)details).globalPosition);
                                    this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                                    break;
                                }
                                clearSelection();
                                _collapseSelectionAt(offset: ((global::Doroti.Framework.Gestures.TapDragDownDetails)details).globalPosition);
                                this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                                break;
                            }
                    }
                    break;
                }
            case 2L:
                {
                    switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
                    {
                        case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                            {
                                if (((global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb && (((global::Doroti.Framework.Gestures.TapDragDownDetails)details).kind is not null)) && !SelectableRegionState._isPrecisePointerDevice(DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Gestures.TapDragDownDetails)details).kind))))
                                {
                                    _doubleTapOffset = ((global::Doroti.Framework.Gestures.TapDragDownDetails)details).globalPosition;
                                    break;
                                }
                                _selectWordAt(offset: ((global::Doroti.Framework.Gestures.TapDragDownDetails)details).globalPosition);
                                this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                                if (((((global::Doroti.Framework.Gestures.TapDragDownDetails)details).kind is not null) && !SelectableRegionState._isPrecisePointerDevice(DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Gestures.TapDragDownDetails)details).kind))))
                                {
                                    _showHandles();
                                }
                                break;
                            }
                        case global::Doroti.Framework.Foundation.TargetPlatform.android:
                        case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                        case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                        case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                        case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                            {
                                _selectWordAt(offset: ((global::Doroti.Framework.Gestures.TapDragDownDetails)details).globalPosition);
                                this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                                break;
                            }
                    }
                    break;
                }
            case 3L:
                {
                    switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
                    {
                        case global::Doroti.Framework.Foundation.TargetPlatform.android:
                        case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                        case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                            {
                                if (((((global::Doroti.Framework.Gestures.TapDragDownDetails)details).kind is not null) && SelectableRegionState._isPrecisePointerDevice(DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Gestures.TapDragDownDetails)details).kind))))
                                {
                                    _selectParagraphAt(offset: ((global::Doroti.Framework.Gestures.TapDragDownDetails)details).globalPosition);
                                    this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                                }
                                break;
                            }
                        case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                        case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                        case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                            {
                                _selectParagraphAt(offset: ((global::Doroti.Framework.Gestures.TapDragDownDetails)details).globalPosition);
                                this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                                break;
                            }
                    }
                    break;
                }
        }
        _updateSelectedContentIfNeeded();
    }

    internal virtual void _handleMouseDragStart(global::Doroti.Framework.Gestures.TapDragStartDetails details)
    {
        switch (_getEffectiveConsecutiveTapCount(((global::Doroti.Framework.Gestures.TapDragStartDetails)details).consecutiveTapCount))
        {
            case 1L:
                {
                    if (((((global::Doroti.Framework.Gestures.TapDragStartDetails)details).kind is not null) && !SelectableRegionState._isPrecisePointerDevice(DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Gestures.TapDragStartDetails)details).kind))))
                    {
                        return;
                    }
                    _selectStartTo(offset: ((global::Doroti.Framework.Gestures.TapDragStartDetails)details).globalPosition);
                    this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                    break;
                }
        }
        _updateSelectedContentIfNeeded();
    }

    internal virtual void _handleMouseDragUpdate(global::Doroti.Framework.Gestures.TapDragUpdateDetails details)
    {
        switch (_getEffectiveConsecutiveTapCount(((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).consecutiveTapCount))
        {
            case 1L:
                {
                    if (((((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).kind is not null) && !SelectableRegionState._isPrecisePointerDevice(DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).kind))))
                    {
                        return;
                    }
                    _selectEndTo(offset: ((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).globalPosition, continuous: true);
                    this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                    break;
                }
            case 2L:
                {
                    switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
                    {
                        case global::Doroti.Framework.Foundation.TargetPlatform.android:
                        case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                            {
                                if ((!global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb || ((((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).kind is not null) && SelectableRegionState._isPrecisePointerDevice(DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).kind)))))
                                {
                                    _selectEndTo(offset: ((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).globalPosition, continuous: true, textGranularity: global::Doroti.Framework.Rendering.TextGranularity.word);
                                    this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                                }
                                break;
                            }
                        case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                            {
                                if ((((global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb && (((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).kind is not null)) && !SelectableRegionState._isPrecisePointerDevice(DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).kind))) && (this._doubleTapOffset is not null)))
                                {
                                    _selectWordAt(offset: DartRuntimePrimitives.RequireValue(this._doubleTapOffset));
                                    _doubleTapOffset = null;
                                }
                                _selectEndTo(offset: ((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).globalPosition, continuous: true, textGranularity: global::Doroti.Framework.Rendering.TextGranularity.word);
                                this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                                if (((((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).kind is not null) && !SelectableRegionState._isPrecisePointerDevice(DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).kind))))
                                {
                                    _showHandles();
                                }
                                break;
                            }
                        case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                        case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                        case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                            {
                                _selectEndTo(offset: ((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).globalPosition, continuous: true, textGranularity: global::Doroti.Framework.Rendering.TextGranularity.word);
                                this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                                break;
                            }
                    }
                    break;
                }
            case 3L:
                {
                    switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
                    {
                        case global::Doroti.Framework.Foundation.TargetPlatform.android:
                        case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                        case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                            {
                                if (((((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).kind is not null) && SelectableRegionState._isPrecisePointerDevice(DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).kind))))
                                {
                                    _selectEndTo(offset: ((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).globalPosition, continuous: true, textGranularity: global::Doroti.Framework.Rendering.TextGranularity.paragraph);
                                    this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                                }
                                break;
                            }
                        case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                        case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                        case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                            {
                                _selectEndTo(offset: ((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).globalPosition, continuous: true, textGranularity: global::Doroti.Framework.Rendering.TextGranularity.paragraph);
                                this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                                break;
                            }
                    }
                    break;
                }
        }
        _updateSelectedContentIfNeeded();
    }

    internal virtual void _handleMouseDragEnd(global::Doroti.Framework.Gestures.TapDragEndDetails details)
    {
        DartRuntimePrimitives.Assert(() => (this._lastPointerDeviceKind is not null));
        bool isPointerPrecise = SelectableRegionState._isPrecisePointerDevice(DartRuntimePrimitives.RequireValue(this._lastPointerDeviceKind));
        bool shouldShowSelectionOverlayOnMobile = !isPointerPrecise;
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                {
                    if (shouldShowSelectionOverlayOnMobile)
                    {
                        _showHandles();
                        _showToolbar();
                    }
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    if (shouldShowSelectionOverlayOnMobile)
                    {
                        _showToolbar();
                    }
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    break;
                }
        }
        _finalizeSelection();
        _updateSelectedContentIfNeeded();
        _finalizeSelectableRegionStatus();
    }

    internal virtual void _handleMouseTapUp(global::Doroti.Framework.Gestures.TapDragUpDetails details)
    {
        if (((object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS)) && _positionIsOnActiveSelection(globalPosition: ((global::Doroti.Framework.Gestures.TapDragUpDetails)details).globalPosition)))
        {
            bool toolbarIsVisibleLocal = (this._selectionOverlay?.toolbarIsVisible ?? false);
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
        switch (_getEffectiveConsecutiveTapCount(((global::Doroti.Framework.Gestures.TapDragUpDetails)details).consecutiveTapCount))
        {
            case 1L:
                {
                    switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
                    {
                        case global::Doroti.Framework.Foundation.TargetPlatform.android:
                        case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                        case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                            {
                                hideToolbar();
                                _collapseSelectionAt(offset: ((global::Doroti.Framework.Gestures.TapDragUpDetails)details).globalPosition);
                                this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                                break;
                            }
                        case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                        case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                        case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                            break;
                    }
                    break;
                }
            case 2L:
                {
                    bool isPointerPrecise = SelectableRegionState._isPrecisePointerDevice(((global::Doroti.Framework.Gestures.TapDragUpDetails)details).kind);
                    switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
                    {
                        case global::Doroti.Framework.Foundation.TargetPlatform.android:
                        case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                            {
                                if (!isPointerPrecise)
                                {
                                    _showHandles();
                                    _showToolbar();
                                }
                                break;
                            }
                        case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                            {
                                if (!isPointerPrecise)
                                {
                                    if (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb)
                                    {
                                        break;
                                    }
                                    _showToolbar();
                                }
                                break;
                            }
                        case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                        case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                        case global::Doroti.Framework.Foundation.TargetPlatform.windows:
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
        if ((((SelectableRegion)(object)this.widget).onSelectionChanged is null))
        {
            return;
        }
        global::Doroti.Framework.Rendering.SelectedContent? content = ((global::Doroti.Framework.Rendering.SelectedContent?)(object?)this._selectable?.getSelectedContent());
        if ((this._lastSelectedContent?.plainText != content?.plainText))
        {
            _lastSelectedContent = content;
            ((SelectableRegion)(object)this.widget).onSelectionChanged!?.Invoke(this._lastSelectedContent);
        }
    }

    internal virtual void _handleTouchLongPressStart(global::Doroti.Framework.Gestures.LongPressStartDetails details)
    {
        DartRuntimePrimitives.Ignore(HapticFeedback.selectionClick());
        this._focusNode.requestFocus();
        _selectWordAt(offset: ((global::Doroti.Framework.Gestures.LongPressStartDetails)details).globalPosition);
        this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
        if ((!object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.android)))
        {
            _showHandles();
        }
        _updateSelectedContentIfNeeded();
    }

    internal virtual void _handleTouchLongPressMoveUpdate(global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails details)
    {
        _selectEndTo(offset: ((global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails)details).globalPosition, textGranularity: global::Doroti.Framework.Rendering.TextGranularity.word);
        this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
        _updateSelectedContentIfNeeded();
    }

    internal virtual void _handleTouchLongPressEnd(global::Doroti.Framework.Gestures.LongPressEndDetails details)
    {
        _finalizeSelection();
        _updateSelectedContentIfNeeded();
        _finalizeSelectableRegionStatus();
        if ((object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.android)))
        {
            _showHandles();
        }
        _showToolbar();
    }

    internal virtual bool _positionIsOnActiveSelection(Offset globalPosition)
    {
        foreach (global::Doroti.Ui.Rect selectionRect in this._selectionDelegate.value.selectionRects)
        {
            Matrix4 transform = ((Matrix4)(object?)this._selectable!.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)));
            global::Doroti.Ui.Rect globalRect = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(transform, selectionRect));
            if (globalRect.contains(globalPosition))
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleRightClickDown(global::Doroti.Framework.Gestures.TapDownDetails details)
    {
        global::Doroti.Ui.Offset? previousSecondaryTapDownPosition = ((global::Doroti.Ui.Offset?)(object?)this._lastSecondaryTapDownPosition);
        bool toolbarIsVisibleLocal = (this._selectionOverlay?.toolbarIsVisible ?? false);
        _lastSecondaryTapDownPosition = ((global::Doroti.Framework.Gestures.TapDownDetails)details).globalPosition;
        this._focusNode.requestFocus();
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    bool lastSecondaryTapDownPositionWasOnActiveSelection = _positionIsOnActiveSelection(globalPosition: ((global::Doroti.Framework.Gestures.TapDownDetails)details).globalPosition);
                    if (lastSecondaryTapDownPositionWasOnActiveSelection)
                    {
                        _lastSecondaryTapDownPosition = ((global::Doroti.Framework.Gestures.TapDownDetails)details).globalPosition;
                        _showHandles();
                        _showToolbar(location: this._lastSecondaryTapDownPosition);
                        _updateSelectedContentIfNeeded();
                        return;
                    }
                    _collapseSelectionAt(offset: DartRuntimePrimitives.RequireValue(this._lastSecondaryTapDownPosition));
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    _selectWordAt(offset: DartRuntimePrimitives.RequireValue(this._lastSecondaryTapDownPosition));
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    if (((object.Equals(previousSecondaryTapDownPosition, this._lastSecondaryTapDownPosition)) && toolbarIsVisibleLocal))
                    {
                        hideToolbar();
                        return;
                    }
                    _selectWordAt(offset: DartRuntimePrimitives.RequireValue(this._lastSecondaryTapDownPosition));
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                {
                    if (toolbarIsVisibleLocal)
                    {
                        hideToolbar();
                        return;
                    }
                    bool lastSecondaryTapDownPositionWasOnActiveSelectionLocal = _positionIsOnActiveSelection(globalPosition: ((global::Doroti.Framework.Gestures.TapDownDetails)details).globalPosition);
                    if (!lastSecondaryTapDownPositionWasOnActiveSelectionLocal)
                    {
                        _collapseSelectionAt(offset: DartRuntimePrimitives.RequireValue(this._lastSecondaryTapDownPosition));
                    }
                    break;
                }
        }
        this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
        _finalizeSelectableRegionStatus();
        _lastSecondaryTapDownPosition = ((global::Doroti.Framework.Gestures.TapDownDetails)details).globalPosition;
        _showHandles();
        _showToolbar(location: this._lastSecondaryTapDownPosition);
        _updateSelectedContentIfNeeded();
    }

    internal virtual bool _userDraggingSelectionEnd => DartRuntimePrimitives.ConvertValue<bool>((this._selectionEndPosition is not null));
    internal virtual void _triggerSelectionEndEdgeUpdate(global::Doroti.Framework.Rendering.TextGranularity? textGranularity = null)
    {
        if ((this._scheduledSelectionEndEdgeUpdate || !this._userDraggingSelectionEnd))
        {
            return;
        }
        if ((object.Equals(this._selectable?.dispatchSelectionEvent(global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent.CreateForEnd(globalPosition: DartRuntimePrimitives.RequireValue(this._selectionEndPosition), granularity: textGranularity)), global::Doroti.Framework.Rendering.SelectionResult.pending)))
        {
            _scheduledSelectionEndEdgeUpdate = true;
            global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timeStamp) =>
            {
                if (!this._scheduledSelectionEndEdgeUpdate)
                {
                    return;
                }
                _scheduledSelectionEndEdgeUpdate = false;
                _triggerSelectionEndEdgeUpdate(textGranularity: textGranularity);
            })), debugLabel: "SelectableRegion.endEdgeUpdate");
            return;
        }
    }

    internal virtual void _onAnyDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        bool draggingHandles = ((this._selectionOverlay is not null) && ((this._selectionOverlay!.isDraggingStartHandle || this._selectionOverlay!.isDraggingEndHandle)));
        if (!draggingHandles)
        {
            this._selectionOverlay!.hideMagnifier();
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

    internal virtual bool _userDraggingSelectionStart => DartRuntimePrimitives.ConvertValue<bool>((this._selectionStartPosition is not null));
    internal virtual void _triggerSelectionStartEdgeUpdate(global::Doroti.Framework.Rendering.TextGranularity? textGranularity = null)
    {
        if ((this._scheduledSelectionStartEdgeUpdate || !this._userDraggingSelectionStart))
        {
            return;
        }
        if ((object.Equals(this._selectable?.dispatchSelectionEvent(new global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent(globalPosition: DartRuntimePrimitives.RequireValue(this._selectionStartPosition), granularity: textGranularity)), global::Doroti.Framework.Rendering.SelectionResult.pending)))
        {
            _scheduledSelectionStartEdgeUpdate = true;
            global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timeStamp) =>
            {
                if (!this._scheduledSelectionStartEdgeUpdate)
                {
                    return;
                }
                _scheduledSelectionStartEdgeUpdate = false;
                _triggerSelectionStartEdgeUpdate(textGranularity: textGranularity);
            })), debugLabel: "SelectableRegion.startEdgeUpdate");
            return;
        }
    }

    internal virtual void _stopSelectionStartEdgeUpdate()
    {
        _scheduledSelectionStartEdgeUpdate = false;
        _selectionEndPosition = null;
    }

    internal virtual void _handleSelectionStartHandleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        DartRuntimePrimitives.Assert(() => (this._selectionDelegate.value.startSelectionPoint is not null));
        global::Doroti.Ui.Offset localPositionLocal = ((global::Doroti.Ui.Offset)(object?)this._selectionDelegate.value.startSelectionPoint!.localPosition);
        Matrix4 globalTransform = ((Matrix4)(object?)this._selectable!.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)));
        _selectionStartHandleDragPosition = MatrixUtils.transformPoint(globalTransform, localPositionLocal);
        this._selectionOverlay!.showMagnifier(_buildInfoForMagnifier(((global::Doroti.Framework.Gestures.DragStartDetails)details).globalPosition, this._selectionDelegate.value.startSelectionPoint!));
        _updateSelectedContentIfNeeded();
    }

    internal virtual void _handleSelectionStartHandleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        _selectionStartHandleDragPosition = (this._selectionStartHandleDragPosition + ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).delta);
        _selectionStartPosition = (this._selectionStartHandleDragPosition - new global::Doroti.Ui.Offset(0, (this._selectionDelegate.value.startSelectionPoint!.lineHeight / 2L)));
        _triggerSelectionStartEdgeUpdate();
        this._selectionOverlay!.updateMagnifier(_buildInfoForMagnifier(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).globalPosition, this._selectionDelegate.value.startSelectionPoint!));
        _updateSelectedContentIfNeeded();
        this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
    }

    internal virtual void _handleSelectionEndHandleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        DartRuntimePrimitives.Assert(() => (this._selectionDelegate.value.endSelectionPoint is not null));
        global::Doroti.Ui.Offset localPositionLocal = ((global::Doroti.Ui.Offset)(object?)this._selectionDelegate.value.endSelectionPoint!.localPosition);
        Matrix4 globalTransform = ((Matrix4)(object?)this._selectable!.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)));
        _selectionEndHandleDragPosition = MatrixUtils.transformPoint(globalTransform, localPositionLocal);
        this._selectionOverlay!.showMagnifier(_buildInfoForMagnifier(((global::Doroti.Framework.Gestures.DragStartDetails)details).globalPosition, this._selectionDelegate.value.endSelectionPoint!));
        _updateSelectedContentIfNeeded();
    }

    internal virtual void _handleSelectionEndHandleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        _selectionEndHandleDragPosition = (this._selectionEndHandleDragPosition + ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).delta);
        _selectionEndPosition = (this._selectionEndHandleDragPosition - new global::Doroti.Ui.Offset(0, (this._selectionDelegate.value.endSelectionPoint!.lineHeight / 2L)));
        _triggerSelectionEndEdgeUpdate();
        this._selectionOverlay!.updateMagnifier(_buildInfoForMagnifier(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).globalPosition, this._selectionDelegate.value.endSelectionPoint!));
        _updateSelectedContentIfNeeded();
        this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
    }

    internal virtual MagnifierInfo _buildInfoForMagnifier(Offset globalGesturePosition, global::Doroti.Framework.Rendering.SelectionPoint selectionPoint)
    {
        Vector3 globalTransform = this._selectable!.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)).getTranslation();
        var globalTransformAsOffset = new global::Doroti.Ui.Offset(globalTransform.x, globalTransform.y);
        global::Doroti.Ui.Offset globalSelectionPointPosition = ((global::Doroti.Ui.Offset)(object?)(((global::Doroti.Framework.Rendering.SelectionPoint)selectionPoint).localPosition + globalTransformAsOffset));
        var caretRectLocal = global::Doroti.Ui.Rect.fromLTWH(globalSelectionPointPosition.dx, (globalSelectionPointPosition.dy - ((global::Doroti.Framework.Rendering.SelectionPoint)selectionPoint).lineHeight), 0, ((global::Doroti.Framework.Rendering.SelectionPoint)selectionPoint).lineHeight);
        return new MagnifierInfo(globalGesturePosition: globalGesturePosition, caretRect: caretRectLocal, fieldBounds: (globalTransformAsOffset & this._selectable!.size), currentLineBoundaries: (globalTransformAsOffset & this._selectable!.size));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _createSelectionOverlay()
    {
        DartRuntimePrimitives.Assert(() => this._hasSelectionOverlayGeometry);
        if ((this._selectionOverlay is not null))
        {
            return;
        }
        global::Doroti.Framework.Rendering.SelectionPoint? start = this._selectionDelegate.value.startSelectionPoint;
        global::Doroti.Framework.Rendering.SelectionPoint? end = this._selectionDelegate.value.endSelectionPoint;
        _selectionOverlay = new SelectionOverlay(context: this.context, debugRequiredFor: this.widget, startHandleType: (start?.handleType ?? global::Doroti.Framework.Rendering.TextSelectionHandleType.collapsed), lineHeightAtStart: (start?.lineHeight ?? end!.lineHeight), onStartHandleDragStart: (global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>)this._handleSelectionStartHandleDragStart, onStartHandleDragUpdate: (global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)this._handleSelectionStartHandleDragUpdate, onStartHandleDragEnd: (global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>)this._onAnyDragEnd, endHandleType: (end?.handleType ?? global::Doroti.Framework.Rendering.TextSelectionHandleType.collapsed), lineHeightAtEnd: (end?.lineHeight ?? start!.lineHeight), onEndHandleDragStart: (global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>)this._handleSelectionEndHandleDragStart, onEndHandleDragUpdate: (global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)this._handleSelectionEndHandleDragUpdate, onEndHandleDragEnd: (global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>)this._onAnyDragEnd, selectionEndpoints: this.selectionEndpoints, selectionControls: ((SelectableRegion)(object)this.widget).selectionControls, selectionDelegate: this, clipboardStatus: ((ClipboardStatusNotifier)(object)null), startHandleLayerLink: this._startHandleLayerLink, endHandleLayerLink: this._endHandleLayerLink, toolbarLayerLink: this._toolbarLayerLink, magnifierConfiguration: ((SelectableRegion)(object)this.widget).magnifierConfiguration);
    }

    internal virtual void _updateSelectionOverlay()
    {
        if ((this._selectionOverlay is null))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => this._hasSelectionOverlayGeometry);
        global::Doroti.Framework.Rendering.SelectionPoint? start = this._selectionDelegate.value.startSelectionPoint;
        global::Doroti.Framework.Rendering.SelectionPoint? end = this._selectionDelegate.value.endSelectionPoint;
        DartRuntimePrimitives.Ignore(((Func<SelectionOverlay>)(() =>
{
    var __cascade = this._selectionOverlay!;
    __cascade.startHandleType = (start?.handleType ?? global::Doroti.Framework.Rendering.TextSelectionHandleType.left);
    __cascade.lineHeightAtStart = (start?.lineHeight ?? end!.lineHeight);
    __cascade.endHandleType = (end?.handleType ?? global::Doroti.Framework.Rendering.TextSelectionHandleType.right);
    __cascade.lineHeightAtEnd = (end?.lineHeight ?? start!.lineHeight);
    __cascade.selectionEndpoints = this.selectionEndpoints;
    return __cascade;
}))());
    }

    internal virtual bool _showHandles()
    {
        if ((this._selectionOverlay is not null))
        {
            this._selectionOverlay!.showHandles();
            return true;
        }
        if (!this._hasSelectionOverlayGeometry)
        {
            return false;
        }
        _createSelectionOverlay();
        this._selectionOverlay!.showHandles();
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _showToolbar(Offset? location = null)
    {
        if ((!this._hasSelectionOverlayGeometry && (this._selectionOverlay is null)))
        {
            return false;
        }
        if (this._webContextMenuEnabled)
        {
            return false;
        }
        if ((this._selectionOverlay is null))
        {
            _createSelectionOverlay();
        }
        this._selectionOverlay!.toolbarLocation = location;
        if ((((SelectableRegion)(object)this.widget).selectionControls is not TextSelectionHandleControls))
        {
            this._selectionOverlay!.showToolbar();
            return true;
        }
        this._selectionOverlay!.hideToolbar();
        this._selectionOverlay!.showToolbar(context: this.context, contextMenuBuilder: ((global::System.Func<BuildContext, Widget>?)((context) =>
        {
            return ((SelectableRegion)(object)this.widget).contextMenuBuilder!(context, this);
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _selectEndTo(Offset offset, bool continuous = false, global::Doroti.Framework.Rendering.TextGranularity? textGranularity = null)
    {
        if (!continuous)
        {
            this._selectable?.dispatchSelectionEvent(global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent.CreateForEnd(globalPosition: offset, granularity: textGranularity));
            return;
        }
        if ((!object.Equals(this._selectionEndPosition, offset)))
        {
            _selectionEndPosition = offset;
            _triggerSelectionEndEdgeUpdate(textGranularity: textGranularity);
        }
    }

    internal virtual void _selectStartTo(Offset offset, bool continuous = false, global::Doroti.Framework.Rendering.TextGranularity? textGranularity = null)
    {
        if (!continuous)
        {
            this._selectable?.dispatchSelectionEvent(new global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent(globalPosition: offset, granularity: textGranularity));
            return;
        }
        if ((!object.Equals(this._selectionStartPosition, offset)))
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
        this._selectable?.dispatchSelectionEvent(new global::Doroti.Framework.Rendering.SelectWordSelectionEvent(globalPosition: offset));
    }

    internal virtual void _selectParagraphAt(Offset offset)
    {
        _finalizeSelection();
        this._selectable?.dispatchSelectionEvent(new global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent(globalPosition: offset));
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
        this._selectable?.dispatchSelectionEvent(new global::Doroti.Framework.Rendering.ClearSelectionEvent());
        _updateSelectedContentIfNeeded();
    }

    internal async virtual Future _copy()
    {
        global::Doroti.Framework.Rendering.SelectedContent? data = ((global::Doroti.Framework.Rendering.SelectedContent?)(object?)this._selectable?.getSelectedContent());
        if ((data is null))
        {
            return;
        }
        await Clipboard.setData(new global::Doroti.Framework.Services.ClipboardData(text: ((global::Doroti.Framework.Rendering.SelectedContent)data).plainText));
    }

    internal async virtual Future _share()
    {
        global::Doroti.Framework.Rendering.SelectedContent? data = ((global::Doroti.Framework.Rendering.SelectedContent?)(object?)this._selectable?.getSelectedContent());
        if ((data is null))
        {
            return;
        }
        await global::Doroti.Framework.Services.SystemChannels.platform.invokeMethod<object>("Share.invoke", ((global::Doroti.Framework.Rendering.SelectedContent)data).plainText);
    }

    public virtual TextSelectionToolbarAnchors contextMenuAnchors
    {
        get
        {
            if ((this._lastSecondaryTapDownPosition is not null))
            {
                var anchors = new TextSelectionToolbarAnchors(primaryAnchor: DartRuntimePrimitives.RequireValue(this._lastSecondaryTapDownPosition));
                _lastSecondaryTapDownPosition = null;
                return anchors;
            }
            var renderBoxLocal = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)this.context.findRenderObject()!)!;
            return TextSelectionToolbarAnchors.CreateFromSelection(renderBox: renderBoxLocal, startGlyphHeight: this.startGlyphHeight, endGlyphHeight: this.endGlyphHeight, selectionEndpoints: this.selectionEndpoints);
            return default!;
        }
    }
    internal virtual bool _determineIsAdjustingSelectionEnd(bool forward)
    {
        if ((this._adjustingSelectionEnd is not null))
        {
            return DartRuntimePrimitives.RequireValue(this._adjustingSelectionEnd);
        }
        bool isReversed = default!;
        global::Doroti.Framework.Rendering.SelectionPoint start = this._selectionDelegate.value.startSelectionPoint!;
        global::Doroti.Framework.Rendering.SelectionPoint end = this._selectionDelegate.value.endSelectionPoint!;
        if ((((global::Doroti.Framework.Rendering.SelectionPoint)start).localPosition.dy > ((global::Doroti.Framework.Rendering.SelectionPoint)end).localPosition.dy))
        {
            isReversed = true;
        }
        else
        {
            if ((((global::Doroti.Framework.Rendering.SelectionPoint)start).localPosition.dy < ((global::Doroti.Framework.Rendering.SelectionPoint)end).localPosition.dy))
            {
                isReversed = false;
            }
            else
            {
                isReversed = (((global::Doroti.Framework.Rendering.SelectionPoint)start).localPosition.dx > ((global::Doroti.Framework.Rendering.SelectionPoint)end).localPosition.dx);
            }
        }
        return DartRuntimePrimitives.RequireValue(_adjustingSelectionEnd = (forward != isReversed));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _granularlyExtendSelection(global::Doroti.Framework.Rendering.TextGranularity granularity, bool forward)
    {
        _directionalHorizontalBaseline = null;
        if (!this._selectionDelegate.value.hasSelection)
        {
            return;
        }
        this._selectable?.dispatchSelectionEvent(new global::Doroti.Framework.Rendering.GranularlyExtendSelectionEvent(forward: forward, isEnd: _determineIsAdjustingSelectionEnd(forward), granularity: granularity));
        _updateSelectedContentIfNeeded();
        this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
        _finalizeSelectableRegionStatus();
    }

    internal virtual void _directionallyExtendSelection(bool forward)
    {
        if (!this._selectionDelegate.value.hasSelection)
        {
            return;
        }
        bool adjustingSelectionExtend = _determineIsAdjustingSelectionEnd(forward);
        global::Doroti.Framework.Rendering.SelectionPoint baseLinePoint = (adjustingSelectionExtend ? this._selectionDelegate.value.endSelectionPoint! : this._selectionDelegate.value.startSelectionPoint!);
        _directionalHorizontalBaseline ??= ((global::Doroti.Framework.Rendering.SelectionPoint)baseLinePoint).localPosition.dx;
        global::Doroti.Ui.Offset globalSelectionPointOffset = ((global::Doroti.Ui.Offset)(object?)MatrixUtils.transformPoint(((Matrix4)((dynamic)this.context.findRenderObject()!).getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null))), new global::Doroti.Ui.Offset(DartRuntimePrimitives.RequireValue(this._directionalHorizontalBaseline), 0)));
        this._selectable?.dispatchSelectionEvent(new global::Doroti.Framework.Rendering.DirectionallyExtendSelectionEvent(isEnd: DartRuntimePrimitives.RequireValue(this._adjustingSelectionEnd), direction: (forward ? global::Doroti.Framework.Rendering.SelectionExtendDirection.nextLine : global::Doroti.Framework.Rendering.SelectionExtendDirection.previousLine), dx: globalSelectionPointOffset.dx));
        _updateSelectedContentIfNeeded();
        this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
        _finalizeSelectableRegionStatus();
    }

    public virtual List<ContextMenuButtonItem> contextMenuButtonItems
    {
        get
        {
            return ((Func<List<ContextMenuButtonItem>>)(() =>
{
    var __cascade = SelectableRegion.getSelectableButtonItems(selectionGeometry: this._selectionDelegate.value, onCopy: ((global::System.Action)(() =>
    {
        DartRuntimePrimitives.Ignore(_copy());
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                {
                    clearSelection();
                    this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                    _finalizeSelectableRegionStatus();
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    hideToolbar(false);
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    hideToolbar();
                    break;
                }
        }
    })), onSelectAll: ((global::System.Action)(() =>
    {
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                {
                    selectAll(global::Doroti.Framework.Services.SelectionChangedCause.toolbar);
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    selectAll();
                    hideToolbar();
                    break;
                }
        }
    })), onShare: ((global::System.Action)(() =>
    {
        DartRuntimePrimitives.Ignore(_share());
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                {
                    clearSelection();
                    this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
                    _finalizeSelectableRegionStatus();
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    hideToolbar(false);
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    hideToolbar();
                    break;
                }
        }
    })));
    __cascade.AddRange(this._textProcessingActionButtonItems.Cast<ContextMenuButtonItem>());
    return __cascade;
}))();
            return default!;
        }
    }
    internal virtual List<ContextMenuButtonItem> _textProcessingActionButtonItems
    {
        get
        {
            var buttonItems = new List<ContextMenuButtonItem>();
            global::Doroti.Framework.Rendering.SelectedContent? data = ((global::Doroti.Framework.Rendering.SelectedContent?)(object?)this._selectable?.getSelectedContent());
            if ((data is null))
            {
                return buttonItems;
            }
            foreach (global::Doroti.Framework.Services.ProcessTextAction action in this._processTextActions)
            {
                buttonItems.Add(new ContextMenuButtonItem(label: ((global::Doroti.Framework.Services.ProcessTextAction)action).label, onPressed: ((global::System.Action)(async () =>
                {
                    string selectedText = ((global::Doroti.Framework.Rendering.SelectedContent)data).plainText;
                    if ((selectedText.Length != 0))
                    {
                        await this._processTextService.processTextAction(((global::Doroti.Framework.Services.ProcessTextAction)action).id, selectedText, true);
                        hideToolbar();
                    }
                }))));
            }
            return buttonItems;
            return default!;
        }
    }
    public virtual double startGlyphHeight
    {
        get
        {
            return this._selectionDelegate.value.startSelectionPoint!.lineHeight;
            return default!;
        }
    }
    public virtual double endGlyphHeight
    {
        get
        {
            return this._selectionDelegate.value.endSelectionPoint!.lineHeight;
            return default!;
        }
    }
    public virtual List<global::Doroti.Framework.Rendering.TextSelectionPoint> selectionEndpoints
    {
        get
        {
            global::Doroti.Framework.Rendering.SelectionPoint? start = this._selectionDelegate.value.startSelectionPoint;
            global::Doroti.Framework.Rendering.SelectionPoint? end = this._selectionDelegate.value.endSelectionPoint;
            List<global::Doroti.Framework.Rendering.TextSelectionPoint> points = default!;
            global::Doroti.Ui.Offset startLocalPosition = ((global::Doroti.Ui.Offset)(object?)(start?.localPosition ?? end!.localPosition));
            global::Doroti.Ui.Offset endLocalPosition = ((global::Doroti.Ui.Offset)(object?)(end?.localPosition ?? start!.localPosition));
            if ((startLocalPosition.dy > endLocalPosition.dy))
            {
                points = new List<global::Doroti.Framework.Rendering.TextSelectionPoint> { new global::Doroti.Framework.Rendering.TextSelectionPoint(endLocalPosition, TextDirection.ltr), new global::Doroti.Framework.Rendering.TextSelectionPoint(startLocalPosition, TextDirection.ltr) };
            }
            else
            {
                points = new List<global::Doroti.Framework.Rendering.TextSelectionPoint> { new global::Doroti.Framework.Rendering.TextSelectionPoint(startLocalPosition, TextDirection.ltr), new global::Doroti.Framework.Rendering.TextSelectionPoint(endLocalPosition, TextDirection.ltr) };
            }
            return points;
            return default!;
        }
    }
    public virtual bool cutEnabled => false;
    public virtual bool pasteEnabled => false;
    public virtual void hideToolbar(bool hideHandles = true)
    {
        this._selectionOverlay?.hideToolbar();
        if (hideHandles)
        {
            this._selectionOverlay?.hideHandles();
        }
    }

    internal virtual object? _hideToolbarIfVisible(DismissIntent intent)
    {
        if ((this._selectionOverlay?.toolbarIsVisible ?? false))
        {
            hideToolbar(false);
            return null;
        }
        return Actions.invoke(this.context, intent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void selectAll(global::Doroti.Framework.Services.SelectionChangedCause cause = default!)
    {
        clearSelection();
        this._selectable?.dispatchSelectionEvent(new global::Doroti.Framework.Rendering.SelectAllSelectionEvent());
        if ((object.Equals(cause, global::Doroti.Framework.Services.SelectionChangedCause.toolbar)))
        {
            _showHandles();
            _showToolbar();
        }
        _updateSelectedContentIfNeeded();
        this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
        _finalizeSelectableRegionStatus();
    }

    public virtual void copySelection(global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
        DartRuntimePrimitives.Ignore(_copy());
        clearSelection();
        this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
        _finalizeSelectableRegionStatus();
    }

    public virtual void bringIntoView(TextPosition position)
    {
    }

    public virtual void cutSelection(global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public virtual void userUpdateTextEditingValue(global::Doroti.Framework.Services.TextEditingValue value, global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
    }

    public async virtual Future pasteText(global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public virtual void add(global::Doroti.Framework.Rendering.Selectable selectable)
    {
        DartRuntimePrimitives.Assert(() => (this._selectable is null));
        _selectable = selectable;
        this._selectable!.addListener(() => this._updateSelectionStatus());
        this._selectable!.pushHandleLayers(this._startHandleLayerLink, this._endHandleLayerLink);
    }

    public virtual void remove(global::Doroti.Framework.Rendering.Selectable selectable)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(this._selectable, selectable)));
        this._selectable!.removeListener(() => this._updateSelectionStatus());
        this._selectable!.pushHandleLayers(((global::Doroti.Framework.Rendering.LayerLink)(object)null), ((global::Doroti.Framework.Rendering.LayerLink)(object)null));
        _selectable = null;
    }

    public override void dispose()
    {
        this._selectable?.removeListener(() => this._updateSelectionStatus());
        this._selectable?.pushHandleLayers(((global::Doroti.Framework.Rendering.LayerLink)(object)null), ((global::Doroti.Framework.Rendering.LayerLink)(object)null));
        if (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb)
        {
            PlatformSelectableRegionContextMenuIo.detach(this._selectionDelegate);
        }
        this._selectionDelegate.dispose();
        this._selectionStatusNotifier.dispose();
        this._selectionOverlay?.hideMagnifier();
        this._selectionOverlay?.dispose();
        _selectionOverlay = null;
        ((SelectableRegion)(object)this.widget).focusNode?.removeListener(() => this._handleFocusChanged());
        this._localFocusNode?.removeListener(() => this._handleFocusChanged());
        this._localFocusNode?.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasOverlay(context));
        Widget result = ((Widget)(object?)new SelectableRegionSelectionStatusScope(selectionStatusNotifier: this._selectionStatusNotifier, child: new SelectionContainer(registrar: this, @delegate: this._selectionDelegate, child: ((SelectableRegion)(object)this.widget).child)));
        if (this._webContextMenuEnabled)
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(new PlatformSelectableRegionContextMenuIo(child: result));
        }
        return ((Widget)(object?)new TapRegion(groupId: typeof(SelectableRegion), onTapOutside: ((global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>)((@event) =>
        {
            if (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb)
            {
                this._focusNode.unfocus();
            }
        })), child: new CompositedTransformTarget(link: this._toolbarLayerLink, child: new RawGestureDetector(gestures: this._gestureRecognizers, behavior: global::Doroti.Framework.Rendering.HitTestBehavior.translucent, excludeFromSemantics: true, child: new Actions(actions: this._actions, child: Focus.CreateWithExternalFocusNode(includeSemantics: false, focusNode: this._focusNode, child: result))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool copyEnabled => true;
    public virtual bool selectAllEnabled => true;
    public virtual bool lookUpEnabled => true;
    public virtual bool searchWebEnabled => true;
    public virtual bool shareEnabled => true;
    public virtual bool liveTextInputEnabled => false;
}

internal abstract class _NonOverrideAction__selectable_region<T> : ContextAction<T> where T : Intent
{
    public abstract object? invokeAction(T intent, BuildContext? context = null);
    public override object? invoke(T intent, BuildContext? context = null)
    {
        if (this.callingAction is object callingActionLocal)
        {
            return ((dynamic)callingActionLocal).invoke(intent);
        }
        return invokeAction(intent, context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SelectAllAction__selectable_region : _NonOverrideAction__selectable_region<SelectAllTextIntent>
{
    public virtual SelectableRegionState state { get; private set; } = default!;

    internal _SelectAllAction__selectable_region(SelectableRegionState state)
    {
        this.state = state;
    }

    public override object? invokeAction(SelectAllTextIntent intent, BuildContext? context = null)
    {
        this.state.selectAll(global::Doroti.Framework.Services.SelectionChangedCause.keyboard);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CopySelectionAction__selectable_region : _NonOverrideAction__selectable_region<CopySelectionTextIntent>
{
    public virtual SelectableRegionState state { get; private set; } = default!;

    internal _CopySelectionAction__selectable_region(SelectableRegionState state)
    {
        this.state = state;
    }

    public override object? invokeAction(CopySelectionTextIntent intent, BuildContext? context = null)
    {
        DartRuntimePrimitives.Ignore(this.state._copy());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _GranularlyExtendSelectionAction__selectable_region<T> : _NonOverrideAction__selectable_region<T> where T : DirectionalTextEditingIntent
{
    public virtual SelectableRegionState state { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.TextGranularity granularity { get; private set; } = default!;

    internal _GranularlyExtendSelectionAction__selectable_region(SelectableRegionState state, global::Doroti.Framework.Rendering.TextGranularity granularity)
    {
        this.state = state;
        this.granularity = granularity;
    }

    public override object? invokeAction(T intent, BuildContext? context = null)
    {
        this.state._granularlyExtendSelection(this.granularity, ((DirectionalTextEditingIntent)(object)intent).forward);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _GranularlyExtendCaretSelectionAction__selectable_region<T> : _NonOverrideAction__selectable_region<T> where T : DirectionalCaretMovementIntent
{
    public virtual SelectableRegionState state { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.TextGranularity granularity { get; private set; } = default!;

    internal _GranularlyExtendCaretSelectionAction__selectable_region(SelectableRegionState state, global::Doroti.Framework.Rendering.TextGranularity granularity)
    {
        this.state = state;
        this.granularity = granularity;
    }

    public override object? invokeAction(T intent, BuildContext? context = null)
    {
        if (((DirectionalCaretMovementIntent)(object)intent).collapseSelection)
        {
            return default!;
        }
        this.state._granularlyExtendSelection(this.granularity, ((DirectionalTextEditingIntent)(object)intent).forward);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DirectionallyExtendCaretSelectionAction__selectable_region<T> : _NonOverrideAction__selectable_region<T> where T : DirectionalCaretMovementIntent
{
    public virtual SelectableRegionState state { get; private set; } = default!;

    internal _DirectionallyExtendCaretSelectionAction__selectable_region(SelectableRegionState state)
    {
        this.state = state;
    }

    public override object? invokeAction(T intent, BuildContext? context = null)
    {
        if (((DirectionalCaretMovementIntent)(object)intent).collapseSelection)
        {
            return default!;
        }
        this.state._directionallyExtendSelection(((DirectionalTextEditingIntent)(object)intent).forward);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class StaticSelectionContainerDelegate : MultiSelectableSelectionContainerDelegate
{
    internal virtual HashSet<global::Doroti.Framework.Rendering.Selectable> _hasReceivedStartEvent { get; private set; } = new HashSet<global::Doroti.Framework.Rendering.Selectable>();
    internal virtual HashSet<global::Doroti.Framework.Rendering.Selectable> _hasReceivedEndEvent { get; private set; } = new HashSet<global::Doroti.Framework.Rendering.Selectable>();
    internal virtual Offset? _lastStartEdgeUpdateGlobalPosition { get; set; } = default;
    internal virtual Offset? _lastEndEdgeUpdateGlobalPosition { get; set; } = default;

    public virtual void didReceiveSelectionEventFor(global::Doroti.Framework.Rendering.Selectable selectable, bool? forEnd = null)
    {
        switch (forEnd)
        {
            case true:
                {
                    this._hasReceivedEndEvent.Add(selectable);
                    break;
                }
            case false:
                {
                    this._hasReceivedStartEvent.Add(selectable);
                    break;
                }
            case null:
                {
                    this._hasReceivedStartEvent.Add(selectable);
                    this._hasReceivedEndEvent.Add(selectable);
                    break;
                }
        }
    }

    public virtual void didReceiveSelectionBoundaryEvents()
    {
        if (((this.currentSelectionStartIndex == -1L) || (this.currentSelectionEndIndex == -1L)))
        {
            return;
        }
        long start = Math.Min(this.currentSelectionStartIndex, this.currentSelectionEndIndex);
        long end = Math.Max(this.currentSelectionStartIndex, this.currentSelectionEndIndex);
        for (var index = start; (index <= end); index += 1L)
        {
            didReceiveSelectionEventFor(selectable: this.selectables[(int)(index)]);
        }
        _updateLastSelectionEdgeLocationsFromGeometries();
    }

    public virtual void updateLastSelectionEdgeLocation(Offset globalSelectionEdgeLocation, bool forEnd)
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
        if (((this.currentSelectionStartIndex != -1L) && this.selectables[(int)(this.currentSelectionStartIndex)].value.hasSelection))
        {
            global::Doroti.Framework.Rendering.Selectable start = this.selectables[(int)(this.currentSelectionStartIndex)];
            global::Doroti.Ui.Offset localStartEdge = ((global::Doroti.Ui.Offset)(object?)(start.value.startSelectionPoint!.localPosition + new global::Doroti.Ui.Offset(0, (-start.value.startSelectionPoint!.lineHeight / 2L))));
            updateLastSelectionEdgeLocation(globalSelectionEdgeLocation: MatrixUtils.transformPoint(start.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)), localStartEdge), forEnd: false);
        }
        if (((this.currentSelectionEndIndex != -1L) && this.selectables[(int)(this.currentSelectionEndIndex)].value.hasSelection))
        {
            global::Doroti.Framework.Rendering.Selectable end = this.selectables[(int)(this.currentSelectionEndIndex)];
            global::Doroti.Ui.Offset localEndEdge = ((global::Doroti.Ui.Offset)(object?)(end.value.endSelectionPoint!.localPosition + new global::Doroti.Ui.Offset(0, (-end.value.endSelectionPoint!.lineHeight / 2L))));
            updateLastSelectionEdgeLocation(globalSelectionEdgeLocation: MatrixUtils.transformPoint(end.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)), localEndEdge), forEnd: true);
        }
    }

    public virtual void clearInternalSelectionState()
    {
        this.selectables.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Rendering.Selectable>)this.clearInternalSelectionStateForSelectable)(__arg0));
        _lastStartEdgeUpdateGlobalPosition = null;
        _lastEndEdgeUpdateGlobalPosition = null;
    }

    public virtual void clearInternalSelectionStateForSelectable(global::Doroti.Framework.Rendering.Selectable selectable)
    {
        this._hasReceivedStartEvent.Remove(selectable);
        this._hasReceivedEndEvent.Remove(selectable);
    }

    public override void remove(global::Doroti.Framework.Rendering.Selectable selectable)
    {
        clearInternalSelectionStateForSelectable(selectable);
        base.remove(selectable);
    }

    public override global::Doroti.Framework.Rendering.SelectionResult handleSelectAll(global::Doroti.Framework.Rendering.SelectAllSelectionEvent @event)
    {
        global::Doroti.Framework.Rendering.SelectionResult result = base.handleSelectAll(@event);
        didReceiveSelectionBoundaryEvents();
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.SelectionResult handleSelectWord(global::Doroti.Framework.Rendering.SelectWordSelectionEvent @event)
    {
        global::Doroti.Framework.Rendering.SelectionResult result = base.handleSelectWord(@event);
        didReceiveSelectionBoundaryEvents();
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.SelectionResult handleSelectParagraph(global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent @event)
    {
        global::Doroti.Framework.Rendering.SelectionResult result = base.handleSelectParagraph(@event);
        didReceiveSelectionBoundaryEvents();
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.SelectionResult handleClearSelection(global::Doroti.Framework.Rendering.ClearSelectionEvent @event)
    {
        global::Doroti.Framework.Rendering.SelectionResult result = base.handleClearSelection(@event);
        clearInternalSelectionState();
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.SelectionResult handleSelectionEdgeUpdate(global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent @event)
    {
        updateLastSelectionEdgeLocation(globalSelectionEdgeLocation: ((global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent)@event).globalPosition, forEnd: (object.Equals(@event.type, global::Doroti.Framework.Rendering.SelectionEventType.endEdgeUpdate)));
        return base.handleSelectionEdgeUpdate(@event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        clearInternalSelectionState();
        base.dispose();
    }

    public override global::Doroti.Framework.Rendering.SelectionResult dispatchSelectionEventToChild(global::Doroti.Framework.Rendering.Selectable selectable, global::Doroti.Framework.Rendering.SelectionEvent @event)
    {
        switch (((global::Doroti.Framework.Rendering.SelectionEvent)@event).type)
        {
            case global::Doroti.Framework.Rendering.SelectionEventType.startEdgeUpdate:
                {
                    didReceiveSelectionEventFor(selectable: selectable, forEnd: false);
                    ensureChildUpdated(selectable);
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionEventType.endEdgeUpdate:
                {
                    didReceiveSelectionEventFor(selectable: selectable, forEnd: true);
                    ensureChildUpdated(selectable);
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionEventType.clear:
                {
                    clearInternalSelectionStateForSelectable(selectable);
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionEventType.selectAll:
            case global::Doroti.Framework.Rendering.SelectionEventType.selectWord:
            case global::Doroti.Framework.Rendering.SelectionEventType.selectParagraph:
                {
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionEventType.granularlyExtendSelection:
            case global::Doroti.Framework.Rendering.SelectionEventType.directionallyExtendSelection:
                {
                    didReceiveSelectionEventFor(selectable: selectable);
                    ensureChildUpdated(selectable);
                    break;
                }
        }
        return base.dispatchSelectionEventToChild(selectable, @event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void ensureChildUpdated(global::Doroti.Framework.Rendering.Selectable selectable)
    {
        if (((this._lastEndEdgeUpdateGlobalPosition is not null) && this._hasReceivedEndEvent.Add(selectable)))
        {
            var synthesizedEvent = global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent.CreateForEnd(globalPosition: DartRuntimePrimitives.RequireValue(this._lastEndEdgeUpdateGlobalPosition));
            if ((this.currentSelectionEndIndex == -1L))
            {
                handleSelectionEdgeUpdate(synthesizedEvent);
            }
            selectable.dispatchSelectionEvent(synthesizedEvent);
        }
        if (((this._lastStartEdgeUpdateGlobalPosition is not null) && this._hasReceivedStartEvent.Add(selectable)))
        {
            var synthesizedEventLocal = new global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent(globalPosition: DartRuntimePrimitives.RequireValue(this._lastStartEdgeUpdateGlobalPosition));
            if ((this.currentSelectionStartIndex == -1L))
            {
                handleSelectionEdgeUpdate(synthesizedEventLocal);
            }
            selectable.dispatchSelectionEvent(synthesizedEventLocal);
        }
    }

    public override void didChangeSelectables()
    {
        if ((this._lastEndEdgeUpdateGlobalPosition is not null))
        {
            handleSelectionEdgeUpdate(global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent.CreateForEnd(globalPosition: DartRuntimePrimitives.RequireValue(this._lastEndEdgeUpdateGlobalPosition)));
        }
        if ((this._lastStartEdgeUpdateGlobalPosition is not null))
        {
            handleSelectionEdgeUpdate(new global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent(globalPosition: DartRuntimePrimitives.RequireValue(this._lastStartEdgeUpdateGlobalPosition)));
        }
        HashSet<global::Doroti.Framework.Rendering.Selectable> selectableSet = this.selectables.toSet();
        this._hasReceivedEndEvent.removeWhere(((selectable) => !selectableSet.Contains(selectable)));
        this._hasReceivedStartEvent.removeWhere(((selectable) => !selectableSet.Contains(selectable)));
        base.didChangeSelectables();
    }

}

public abstract class MultiSelectableSelectionContainerDelegate : SelectionContainerDelegate
{
    public virtual List<global::Doroti.Framework.Rendering.Selectable> selectables { get; set; } = new List<global::Doroti.Framework.Rendering.Selectable>();
    internal const double _kSelectionHandleDrawableAreaPadding = 5.0;
    public virtual long currentSelectionEndIndex { get; set; } = -1L;
    public virtual long currentSelectionStartIndex { get; set; } = -1L;
    internal virtual global::Doroti.Framework.Rendering.LayerLink? _startHandleLayer { get; set; } = default;
    internal virtual global::Doroti.Framework.Rendering.Selectable? _startHandleLayerOwner { get; set; } = default;
    internal virtual global::Doroti.Framework.Rendering.LayerLink? _endHandleLayer { get; set; } = default;
    internal virtual global::Doroti.Framework.Rendering.Selectable? _endHandleLayerOwner { get; set; } = default;
    internal virtual bool _isHandlingSelectionEvent { get; set; } = false;
    internal virtual bool _scheduledSelectableUpdate { get; set; } = false;
    internal virtual bool _selectionInProgress { get; set; } = false;
    internal virtual HashSet<global::Doroti.Framework.Rendering.Selectable> _additions { get; set; } = new HashSet<global::Doroti.Framework.Rendering.Selectable>();
    internal virtual bool _extendSelectionInProgress { get; set; } = false;
    internal virtual global::Doroti.Framework.Rendering.SelectionGeometry _selectionGeometry { get; set; } = new global::Doroti.Framework.Rendering.SelectionGeometry(hasContent: false, status: global::Doroti.Framework.Rendering.SelectionStatus.none);

    protected MultiSelectableSelectionContainerDelegate()
    {
    }

    public virtual void add(global::Doroti.Framework.Rendering.Selectable selectable)
    {
        DartRuntimePrimitives.Assert(() => !this.selectables.Contains(selectable));
        this._additions.Add(selectable);
        _scheduleSelectableUpdate();
    }

    public virtual void remove(global::Doroti.Framework.Rendering.Selectable selectable)
    {
        if (this._additions.Remove(selectable))
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
        if (!this._scheduledSelectableUpdate)
        {
            _scheduledSelectableUpdate = true;
            void runScheduledTask(Duration? duration = null)
            {
                if (!this._scheduledSelectableUpdate)
                {
                    return;
                }
                _scheduledSelectableUpdate = false;
                _updateSelectables();
            }
            if ((object.Equals(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Framework.Scheduler.SchedulerPhase.postFrameCallbacks)))
            {
                DartAsyncRuntime.scheduleMicrotask(runScheduledTask);
            }
            else
            {
                global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback((__arg0) => ((global::System.Action<Duration?>)runScheduledTask)(DartRuntimePrimitives.ConvertValue<Duration>(__arg0)), debugLabel: "SelectionContainer.runScheduledTask");
            }
        }
    }

    internal virtual void _updateSelectables()
    {
        if (System.Linq.Enumerable.Any(this._additions))
        {
            _flushAdditions();
        }
        didChangeSelectables();
    }

    internal virtual void _flushAdditions()
    {
        List<global::Doroti.Framework.Rendering.Selectable> mergingSelectables = ((Func<List<global::Doroti.Framework.Rendering.Selectable>>)(() =>
{
    var __cascade = this._additions.ToList();
    __cascade.sort(this.compareOrder);
    return __cascade;
}))().ToList();
        List<global::Doroti.Framework.Rendering.Selectable> existingSelectables = this.selectables.ToList();
        selectables = new List<global::Doroti.Framework.Rendering.Selectable>();
        var mergingIndex = 0L;
        var existingIndex = 0L;
        long selectionStartIndex = this.currentSelectionStartIndex;
        long selectionEndIndex = this.currentSelectionEndIndex;
        while (((mergingIndex < checked((long)(mergingSelectables.Count))) || (existingIndex < checked((long)(existingSelectables.Count)))))
        {
            if (((mergingIndex >= checked((long)(mergingSelectables.Count))) || (((existingIndex < checked((long)(existingSelectables.Count))) && (this.compareOrder(existingSelectables[(int)(existingIndex)], mergingSelectables[(int)(mergingIndex)]) < 0L)))))
            {
                if ((existingIndex == this.currentSelectionStartIndex))
                {
                    selectionStartIndex = checked((long)(this.selectables.Count));
                }
                if ((existingIndex == this.currentSelectionEndIndex))
                {
                    selectionEndIndex = checked((long)(this.selectables.Count));
                }
                this.selectables.Add(existingSelectables[(int)(existingIndex)]);
                existingIndex += 1L;
                continue;
            }
            global::Doroti.Framework.Rendering.Selectable mergingSelectable = mergingSelectables[(int)(mergingIndex)];
            if (((existingIndex < Math.Max(this.currentSelectionStartIndex, this.currentSelectionEndIndex)) && (existingIndex > Math.Min(this.currentSelectionStartIndex, this.currentSelectionEndIndex))))
            {
                ensureChildUpdated(mergingSelectable);
            }
            mergingSelectable.addListener(() => this._handleSelectableGeometryChange());
            this.selectables.Add(mergingSelectable);
            mergingIndex += 1L;
        }
        DartRuntimePrimitives.Assert(() => (((mergingIndex == checked((long)(mergingSelectables.Count))) && (existingIndex == checked((long)(existingSelectables.Count)))) && (checked((long)(this.selectables.Count)) == (existingIndex + mergingIndex))));
        DartRuntimePrimitives.Assert(() => ((selectionStartIndex >= -1L) || (selectionStartIndex < checked((long)(this.selectables.Count)))));
        DartRuntimePrimitives.Assert(() => ((selectionEndIndex >= -1L) || (selectionEndIndex < checked((long)(this.selectables.Count)))));
        DartRuntimePrimitives.Assert(() => (((this.currentSelectionStartIndex == -1L)) == ((selectionStartIndex == -1L))));
        DartRuntimePrimitives.Assert(() => (((this.currentSelectionEndIndex == -1L)) == ((selectionEndIndex == -1L))));
        currentSelectionEndIndex = selectionEndIndex;
        currentSelectionStartIndex = selectionStartIndex;
        _additions = new HashSet<global::Doroti.Framework.Rendering.Selectable>();
    }

    internal virtual void _removeSelectable(global::Doroti.Framework.Rendering.Selectable selectable)
    {
        DartRuntimePrimitives.Assert(() => this.selectables.Contains(selectable), () => (object?)"The selectable is not in this registrar.");
        long index = ((long)((dynamic)this.selectables).IndexOf(selectable));
        this.selectables.removeAt(index);
        if ((index <= this.currentSelectionEndIndex))
        {
            currentSelectionEndIndex -= 1L;
        }
        if ((index <= this.currentSelectionStartIndex))
        {
            currentSelectionStartIndex -= 1L;
        }
        selectable.removeListener(() => this._handleSelectableGeometryChange());
    }

    public virtual void didChangeSelectables()
    {
        _updateSelectionGeometry();
    }

    public virtual global::Doroti.Framework.Rendering.SelectionGeometry value => this._selectionGeometry;
    internal virtual void _updateSelectionGeometry()
    {
        global::Doroti.Framework.Rendering.SelectionGeometry newValue = ((global::Doroti.Framework.Rendering.SelectionGeometry)(object?)getSelectionGeometry());
        if ((!object.Equals(this._selectionGeometry, newValue)))
        {
            _selectionGeometry = newValue;
            notifyListeners();
        }
        _updateHandleLayersAndOwners();
    }

    internal static global::Doroti.Ui.Rect _getBoundingBox(global::Doroti.Framework.Rendering.Selectable selectable)
    {
        global::Doroti.Ui.Rect result = ((global::Doroti.Ui.Rect)(object?)((global::Doroti.Framework.Rendering.Selectable)selectable).boundingBoxes.First());
        for (var index = 1L; (index < checked((long)(((global::Doroti.Framework.Rendering.Selectable)selectable).boundingBoxes.Count))); index += 1L)
        {
            result = result.expandToInclude(((global::Doroti.Framework.Rendering.Selectable)selectable).boundingBoxes[(int)(index)]);
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Comparison<global::Doroti.Framework.Rendering.Selectable> compareOrder => new Comparison<global::Doroti.Framework.Rendering.Selectable>((left, right) => checked((int)_compareScreenOrder(left, right)));
    internal static long _compareScreenOrder(global::Doroti.Framework.Rendering.Selectable a, global::Doroti.Framework.Rendering.Selectable b)
    {
        global::Doroti.Ui.Rect rectA = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(a.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)), MultiSelectableSelectionContainerDelegate._getBoundingBox(a)));
        global::Doroti.Ui.Rect rectB = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(b.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)), MultiSelectableSelectionContainerDelegate._getBoundingBox(b)));
        long result = MultiSelectableSelectionContainerDelegate._compareVertically(rectA, rectB);
        if ((result != 0L))
        {
            return result;
        }
        return MultiSelectableSelectionContainerDelegate._compareHorizontally(rectA, rectB);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static long _compareVertically(Rect a, Rect b)
    {
        if ((((((a.top - b.top) < Selectable_regionLibrary._kSelectableVerticalComparingThreshold) && ((a.bottom - b.bottom) > -Selectable_regionLibrary._kSelectableVerticalComparingThreshold))) || ((((b.top - a.top) < Selectable_regionLibrary._kSelectableVerticalComparingThreshold) && ((b.bottom - a.bottom) > -Selectable_regionLibrary._kSelectableVerticalComparingThreshold)))))
        {
            return 0L;
        }
        if ((((a.top - b.top)).abs() > Selectable_regionLibrary._kSelectableVerticalComparingThreshold))
        {
            return ((a.top > b.top) ? 1L : -1L);
        }
        return ((a.bottom > b.bottom) ? 1L : -1L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static long _compareHorizontally(Rect a, Rect b)
    {
        if ((((a.left - b.left) < global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance) && ((a.right - b.right) > -global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)))
        {
            return -1L;
        }
        if ((((b.left - a.left) < global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance) && ((b.right - a.right) > -global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)))
        {
            return 1L;
        }
        if ((((a.left - b.left)).abs() > global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
        {
            return ((a.left > b.left) ? 1L : -1L);
        }
        return ((a.right > b.right) ? 1L : -1L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleSelectableGeometryChange()
    {
        if (this._isHandlingSelectionEvent)
        {
            return;
        }
        _updateSelectionGeometry();
    }

    public virtual global::Doroti.Framework.Rendering.SelectionGeometry getSelectionGeometry()
    {
        if ((((this.currentSelectionEndIndex == -1L) || (this.currentSelectionStartIndex == -1L)) || !System.Linq.Enumerable.Any(this.selectables)))
        {
            return new global::Doroti.Framework.Rendering.SelectionGeometry(status: global::Doroti.Framework.Rendering.SelectionStatus.none, hasContent: System.Linq.Enumerable.Any(this.selectables));
        }
        if (!this._extendSelectionInProgress)
        {
            currentSelectionStartIndex = _adjustSelectionIndexBasedOnSelectionGeometry(this.currentSelectionStartIndex, this.currentSelectionEndIndex);
            currentSelectionEndIndex = _adjustSelectionIndexBasedOnSelectionGeometry(this.currentSelectionEndIndex, this.currentSelectionStartIndex);
        }
        global::Doroti.Framework.Rendering.SelectionGeometry startGeometry = this.selectables[(int)(this.currentSelectionStartIndex)].value;
        bool forwardSelection = (this.currentSelectionEndIndex >= this.currentSelectionStartIndex);
        long startIndexWalker = this.currentSelectionStartIndex;
        while (((startIndexWalker != this.currentSelectionEndIndex) && (((global::Doroti.Framework.Rendering.SelectionGeometry)startGeometry).startSelectionPoint is null)))
        {
            startIndexWalker += (forwardSelection ? 1L : -1L);
            startGeometry = this.selectables[(int)(startIndexWalker)].value;
        }
        global::Doroti.Framework.Rendering.SelectionPoint? startPoint = default!;
        if ((((global::Doroti.Framework.Rendering.SelectionGeometry)startGeometry).startSelectionPoint is not null))
        {
            Matrix4 startTransform = ((Matrix4)(object?)getTransformFrom(this.selectables[(int)(startIndexWalker)]));
            global::Doroti.Ui.Offset start = ((global::Doroti.Ui.Offset)(object?)MatrixUtils.transformPoint(startTransform, ((global::Doroti.Framework.Rendering.SelectionGeometry)startGeometry).startSelectionPoint!.localPosition));
            if (start.isFinite)
            {
                startPoint = new global::Doroti.Framework.Rendering.SelectionPoint(localPosition: start, lineHeight: ((global::Doroti.Framework.Rendering.SelectionGeometry)startGeometry).startSelectionPoint!.lineHeight, handleType: ((global::Doroti.Framework.Rendering.SelectionGeometry)startGeometry).startSelectionPoint!.handleType);
            }
        }
        global::Doroti.Framework.Rendering.SelectionGeometry endGeometry = this.selectables[(int)(this.currentSelectionEndIndex)].value;
        long endIndexWalker = this.currentSelectionEndIndex;
        while (((endIndexWalker != this.currentSelectionStartIndex) && (((global::Doroti.Framework.Rendering.SelectionGeometry)endGeometry).endSelectionPoint is null)))
        {
            endIndexWalker += (forwardSelection ? -1L : 1L);
            endGeometry = this.selectables[(int)(endIndexWalker)].value;
        }
        global::Doroti.Framework.Rendering.SelectionPoint? endPoint = default!;
        if ((((global::Doroti.Framework.Rendering.SelectionGeometry)endGeometry).endSelectionPoint is not null))
        {
            Matrix4 endTransform = ((Matrix4)(object?)getTransformFrom(this.selectables[(int)(endIndexWalker)]));
            global::Doroti.Ui.Offset end = ((global::Doroti.Ui.Offset)(object?)MatrixUtils.transformPoint(endTransform, ((global::Doroti.Framework.Rendering.SelectionGeometry)endGeometry).endSelectionPoint!.localPosition));
            if (end.isFinite)
            {
                endPoint = new global::Doroti.Framework.Rendering.SelectionPoint(localPosition: end, lineHeight: ((global::Doroti.Framework.Rendering.SelectionGeometry)endGeometry).endSelectionPoint!.lineHeight, handleType: ((global::Doroti.Framework.Rendering.SelectionGeometry)endGeometry).endSelectionPoint!.handleType);
            }
        }
        var selectionRectsLocal = new List<global::Doroti.Ui.Rect>();
        global::Doroti.Ui.Rect? drawableArea = ((global::Doroti.Ui.Rect?)(object?)(this.hasSize ? global::Doroti.Ui.Rect.fromLTWH(0, 0, this.containerSize.width, this.containerSize.height) : null));
        for (long index = this.currentSelectionStartIndex; (index <= this.currentSelectionEndIndex); index++)
        {
            List<global::Doroti.Ui.Rect> currSelectableSelectionRects = this.selectables[(int)(index)].value.selectionRects.Cast<global::Doroti.Ui.Rect>().ToList();
            List<global::Doroti.Ui.Rect> selectionRectsWithinDrawableArea = currSelectableSelectionRects.map<Rect, Rect>(((selectionRect) =>
            {
                Matrix4 transform = ((Matrix4)(object?)getTransformFrom(this.selectables[(int)(index)]));
                global::Doroti.Ui.Rect localRect = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(transform, selectionRect));
                return (drawableArea?.intersect(localRect) ?? localRect);
                throw new InvalidOperationException("Dart closure completed without a value.");
            })).where(((selectionRect) =>
            {
                return (selectionRect.isFinite && !selectionRect.isEmpty);
                throw new InvalidOperationException("Dart closure completed without a value.");
            })).ToList().Cast<global::Doroti.Ui.Rect>().ToList();
            selectionRectsLocal.AddRange(selectionRectsWithinDrawableArea.Cast<Rect>());
        }
        return new global::Doroti.Framework.Rendering.SelectionGeometry(startSelectionPoint: startPoint, endSelectionPoint: endPoint, selectionRects: selectionRectsLocal, status: ((!object.Equals(startGeometry, endGeometry)) ? global::Doroti.Framework.Rendering.SelectionStatus.uncollapsed : ((global::Doroti.Framework.Rendering.SelectionGeometry)startGeometry).status), hasContent: true);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long _adjustSelectionIndexBasedOnSelectionGeometry(long currentIndex, long towardIndex)
    {
        bool forward = (towardIndex > currentIndex);
        while (((currentIndex != towardIndex) && (!object.Equals(this.selectables[(int)(currentIndex)].value.status, global::Doroti.Framework.Rendering.SelectionStatus.uncollapsed))))
        {
            currentIndex += (forward ? 1L : -1L);
        }
        return currentIndex;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void pushHandleLayers(global::Doroti.Framework.Rendering.LayerLink? startHandle, global::Doroti.Framework.Rendering.LayerLink? endHandle)
    {
        if (((object.Equals(this._startHandleLayer, startHandle)) && (object.Equals(this._endHandleLayer, endHandle))))
        {
            return;
        }
        _startHandleLayer = startHandle;
        _endHandleLayer = endHandle;
        _updateHandleLayersAndOwners();
    }

    internal virtual void _updateHandleLayersAndOwners()
    {
        global::Doroti.Framework.Rendering.LayerLink? effectiveStartHandle = this._startHandleLayer;
        global::Doroti.Framework.Rendering.LayerLink? effectiveEndHandle = this._endHandleLayer;
        if (((effectiveStartHandle is not null) || (effectiveEndHandle is not null)))
        {
            global::Doroti.Ui.Rect? drawableArea = ((global::Doroti.Ui.Rect?)(object?)(this.hasSize ? global::Doroti.Ui.Rect.fromLTWH(0, 0, this.containerSize.width, this.containerSize.height).inflate(_kSelectionHandleDrawableAreaPadding) : null));
            bool hideStartHandle = (((((global::Doroti.Framework.Rendering.SelectionGeometry)this.value).startSelectionPoint is null) || (drawableArea is null)) || !DartRuntimePrimitives.RequireValue(drawableArea).contains(((global::Doroti.Framework.Rendering.SelectionGeometry)this.value).startSelectionPoint!.localPosition));
            bool hideEndHandle = (((((global::Doroti.Framework.Rendering.SelectionGeometry)this.value).endSelectionPoint is null) || (drawableArea is null)) || !DartRuntimePrimitives.RequireValue(drawableArea).contains(((global::Doroti.Framework.Rendering.SelectionGeometry)this.value).endSelectionPoint!.localPosition));
            effectiveStartHandle = (hideStartHandle ? null : this._startHandleLayer);
            effectiveEndHandle = (hideEndHandle ? null : this._endHandleLayer);
        }
        if (((this.currentSelectionStartIndex == -1L) || (this.currentSelectionEndIndex == -1L)))
        {
            if ((this._startHandleLayerOwner is not null))
            {
                this._startHandleLayerOwner!.pushHandleLayers(((global::Doroti.Framework.Rendering.LayerLink)(object)null), ((global::Doroti.Framework.Rendering.LayerLink)(object)null));
                _startHandleLayerOwner = null;
            }
            if ((this._endHandleLayerOwner is not null))
            {
                this._endHandleLayerOwner!.pushHandleLayers(((global::Doroti.Framework.Rendering.LayerLink)(object)null), ((global::Doroti.Framework.Rendering.LayerLink)(object)null));
                _endHandleLayerOwner = null;
            }
            return;
        }
        if ((!object.Equals(this.selectables[(int)(this.currentSelectionStartIndex)], this._startHandleLayerOwner)))
        {
            this._startHandleLayerOwner?.pushHandleLayers(((global::Doroti.Framework.Rendering.LayerLink)(object)null), ((global::Doroti.Framework.Rendering.LayerLink)(object)null));
        }
        if ((!object.Equals(this.selectables[(int)(this.currentSelectionEndIndex)], this._endHandleLayerOwner)))
        {
            this._endHandleLayerOwner?.pushHandleLayers(((global::Doroti.Framework.Rendering.LayerLink)(object)null), ((global::Doroti.Framework.Rendering.LayerLink)(object)null));
        }
        _startHandleLayerOwner = this.selectables[(int)(this.currentSelectionStartIndex)];
        if ((this.currentSelectionStartIndex == this.currentSelectionEndIndex))
        {
            _endHandleLayerOwner = this._startHandleLayerOwner;
            this._startHandleLayerOwner!.pushHandleLayers(effectiveStartHandle, effectiveEndHandle);
            return;
        }
        this._startHandleLayerOwner!.pushHandleLayers(effectiveStartHandle, ((global::Doroti.Framework.Rendering.LayerLink)(object)null));
        _endHandleLayerOwner = this.selectables[(int)(this.currentSelectionEndIndex)];
        this._endHandleLayerOwner!.pushHandleLayers(((global::Doroti.Framework.Rendering.LayerLink)(object)null), effectiveEndHandle);
    }

    public virtual global::Doroti.Framework.Rendering.SelectedContent? getSelectedContent()
    {
        var selections = new List<global::Doroti.Framework.Rendering.SelectedContent>();
        if (!System.Linq.Enumerable.Any(selections))
        {
            return ((global::Doroti.Framework.Rendering.SelectedContent)(object)null);
        }
        var buffer = new StringBuffer();
        foreach (var selection in selections)
        {
            buffer.write(((global::Doroti.Framework.Rendering.SelectedContent)selection).plainText);
        }
        return new global::Doroti.Framework.Rendering.SelectedContent(plainText: buffer.ToString());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long contentLength => System.Linq.Enumerable.Aggregate(this.selectables, (long)0L, ((sum, selectable) => (sum + selectable.contentLength)));
    internal virtual global::Doroti.Framework.Rendering.SelectedContentRange? _calculateLocalRange(List<(long contentLength, global::Doroti.Framework.Rendering.SelectedContentRange? range)> selections)
    {
        if (((this.currentSelectionStartIndex == -1L) || (this.currentSelectionEndIndex == -1L)))
        {
            return ((global::Doroti.Framework.Rendering.SelectedContentRange)(object)null);
        }
        var startOffsetLocal = 0L;
        var endOffsetLocal = 0L;
        var foundStart = false;
        bool forwardSelection = (this.currentSelectionEndIndex >= this.currentSelectionStartIndex);
        if ((this.currentSelectionEndIndex == this.currentSelectionStartIndex))
        {
            global::Doroti.Framework.Rendering.SelectedContentRange rangeAtSelectableInSelection = this.selectables[(int)(this.currentSelectionStartIndex)].getSelection()!;
            forwardSelection = (((global::Doroti.Framework.Rendering.SelectedContentRange)rangeAtSelectableInSelection).endOffset >= ((global::Doroti.Framework.Rendering.SelectedContentRange)rangeAtSelectableInSelection).startOffset);
        }
        for (var index = 0L; (index < checked((long)(selections.Count))); index++)
        {
            (long contentLength, global::Doroti.Framework.Rendering.SelectedContentRange? range) selection = selections[(int)(index)];
            if ((selection.range is null))
            {
                if (foundStart)
                {
                    return new global::Doroti.Framework.Rendering.SelectedContentRange(startOffset: (forwardSelection ? startOffsetLocal : endOffsetLocal), endOffset: (forwardSelection ? endOffsetLocal : startOffsetLocal));
                }
                startOffsetLocal += selection.contentLength;
                endOffsetLocal = startOffsetLocal;
                continue;
            }
            long selectionStartNormalized = Math.Min(selection.range!.startOffset, selection.range!.endOffset);
            long selectionEndNormalized = Math.Max(selection.range!.startOffset, selection.range!.endOffset);
            if (!foundStart)
            {
                startOffsetLocal += selectionStartNormalized;
                endOffsetLocal = (startOffsetLocal + ((selectionEndNormalized - selectionStartNormalized)).abs());
                foundStart = true;
            }
            else
            {
                endOffsetLocal += ((selectionEndNormalized - selectionStartNormalized)).abs();
            }
        }
        DartRuntimePrimitives.Assert(() => foundStart, () => (object?)"The start of the selection has not been found despite this selection delegate having an existing currentSelectionStartIndex and currentSelectionEndIndex.");
        return new global::Doroti.Framework.Rendering.SelectedContentRange(startOffset: (forwardSelection ? startOffsetLocal : endOffsetLocal), endOffset: (forwardSelection ? endOffsetLocal : startOffsetLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.SelectedContentRange? getSelection()
    {
        var selections = new List<(long contentLength, global::Doroti.Framework.Rendering.SelectedContentRange? range)>();
        return ((global::Doroti.Framework.Rendering.SelectedContentRange?)(object?)_calculateLocalRange(selections));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _flushInactiveSelections()
    {
        if (((this.currentSelectionStartIndex == -1L) && (this.currentSelectionEndIndex == -1L)))
        {
            return;
        }
        if (((this.currentSelectionStartIndex == -1L) || (this.currentSelectionEndIndex == -1L)))
        {
            long skipIndexLocal = ((this.currentSelectionStartIndex == -1L) ? this.currentSelectionEndIndex : this.currentSelectionStartIndex);
            _clearSelectables(skipIndex: DartRuntimePrimitives.RequireValue(skipIndexLocal));
            return;
        }
        long skipStart = Math.Min(this.currentSelectionStartIndex, this.currentSelectionEndIndex);
        long skipEnd = Math.Max(this.currentSelectionStartIndex, this.currentSelectionEndIndex);
        for (var index = 0L; (index < checked((long)(this.selectables.Count))); index += 1L)
        {
            if (((index >= skipStart) && (index <= skipEnd)))
            {
                continue;
            }
            dispatchSelectionEventToChild(this.selectables[(int)(index)], new global::Doroti.Framework.Rendering.ClearSelectionEvent());
        }
    }

    public virtual global::Doroti.Framework.Rendering.SelectionResult handleSelectAll(global::Doroti.Framework.Rendering.SelectAllSelectionEvent @event)
    {
        foreach (global::Doroti.Framework.Rendering.Selectable selectable in this.selectables)
        {
            dispatchSelectionEventToChild(selectable, @event);
        }
        currentSelectionStartIndex = 0L;
        currentSelectionEndIndex = (checked((long)(this.selectables.Count)) - 1L);
        return global::Doroti.Framework.Rendering.SelectionResult.none;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _clearSelectables(long? skipIndex = null)
    {
        for (var i = 0L; (i < checked((long)(this.selectables.Count))); i++)
        {
            if ((i == skipIndex))
            {
                continue;
            }
            dispatchSelectionEventToChild(this.selectables[(int)(i)], new global::Doroti.Framework.Rendering.ClearSelectionEvent());
        }
    }

    internal virtual global::Doroti.Framework.Rendering.SelectionResult _handleSelectBoundary(global::Doroti.Framework.Rendering.SelectionEvent @event)
    {
        DartRuntimePrimitives.Assert(() => ((@event is global::Doroti.Framework.Rendering.SelectWordSelectionEvent) || (@event is global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent)), () => (object?)"This method should only be given selection events that select text boundaries.");
        global::Doroti.Ui.Offset effectiveGlobalPosition = ((global::Doroti.Ui.Offset)(object?)(@event switch { global::Doroti.Framework.Rendering.SelectWordSelectionEvent { globalPosition: object globalPositionLocal } __object119052 => globalPositionLocal, global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent { globalPosition: object globalPositionAlternate } __object119125 => globalPositionAlternate, _ => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Unsupported selection event: {@event}")) }));
        global::Doroti.Framework.Rendering.SelectionResult? lastSelectionResult = default!;
        double minDistanceSquared = double.PositiveInfinity;
        var nearestIndex = 0L;
        for (var index = 0L; (index < checked((long)(this.selectables.Count))); index += 1L)
        {
            var globalRectsContainPosition = false;
            Matrix4 transform = ((Matrix4)(object?)this.selectables[(int)(index)].getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)));
            foreach (global::Doroti.Ui.Rect rect in this.selectables[(int)(index)].boundingBoxes)
            {
                global::Doroti.Ui.Rect globalRect = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(transform, rect));
                if (globalRect.contains(effectiveGlobalPosition))
                {
                    globalRectsContainPosition = true;
                    break;
                }
                double dxLocal = (effectiveGlobalPosition.dx - Dart_uiLibrary.clampDouble(effectiveGlobalPosition.dx, globalRect.left, globalRect.right));
                double dyLocal = (effectiveGlobalPosition.dy - Dart_uiLibrary.clampDouble(effectiveGlobalPosition.dy, globalRect.top, globalRect.bottom));
                double distanceSquared = ((dxLocal * dxLocal) + (dyLocal * dyLocal));
                if ((distanceSquared < minDistanceSquared))
                {
                    minDistanceSquared = distanceSquared;
                    nearestIndex = index;
                }
            }
            if (globalRectsContainPosition)
            {
                global::Doroti.Framework.Rendering.SelectionGeometry existingGeometry = this.selectables[(int)(index)].value;
                lastSelectionResult = dispatchSelectionEventToChild(this.selectables[(int)(index)], @event);
                if (((index == (checked((long)(this.selectables.Count)) - 1L)) && (object.Equals(DartRuntimePrimitives.RequireValue(lastSelectionResult), global::Doroti.Framework.Rendering.SelectionResult.next))))
                {
                    return global::Doroti.Framework.Rendering.SelectionResult.next;
                }
                if ((object.Equals(DartRuntimePrimitives.RequireValue(lastSelectionResult), global::Doroti.Framework.Rendering.SelectionResult.next)))
                {
                    continue;
                }
                if (((index == 0L) && (object.Equals(DartRuntimePrimitives.RequireValue(lastSelectionResult), global::Doroti.Framework.Rendering.SelectionResult.previous))))
                {
                    return global::Doroti.Framework.Rendering.SelectionResult.previous;
                }
                if ((!object.Equals(this.selectables[(int)(index)].value, existingGeometry)))
                {
                    _clearSelectables(skipIndex: index);
                    currentSelectionStartIndex = currentSelectionEndIndex = index;
                }
                return global::Doroti.Framework.Rendering.SelectionResult.end;
            }
            else
            {
                if ((object.Equals(lastSelectionResult, global::Doroti.Framework.Rendering.SelectionResult.next)))
                {
                    currentSelectionStartIndex = currentSelectionEndIndex = (index - 1L);
                    return global::Doroti.Framework.Rendering.SelectionResult.end;
                }
            }
        }
        DartRuntimePrimitives.Assert(() => (lastSelectionResult is null));
        if (System.Linq.Enumerable.Any(this.selectables))
        {
            global::Doroti.Framework.Rendering.SelectionGeometry existingGeometryLocal = this.selectables[(int)(nearestIndex)].value;
            dispatchSelectionEventToChild(this.selectables[(int)(nearestIndex)], @event);
            if ((!object.Equals(this.selectables[(int)(nearestIndex)].value, existingGeometryLocal)))
            {
                _clearSelectables(skipIndex: nearestIndex);
                currentSelectionStartIndex = currentSelectionEndIndex = nearestIndex;
            }
        }
        return global::Doroti.Framework.Rendering.SelectionResult.end;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.SelectionResult handleSelectWord(global::Doroti.Framework.Rendering.SelectWordSelectionEvent @event)
    {
        return _handleSelectBoundary(@event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.SelectionResult handleSelectParagraph(global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent @event)
    {
        return _handleSelectBoundary(@event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.SelectionResult handleClearSelection(global::Doroti.Framework.Rendering.ClearSelectionEvent @event)
    {
        foreach (global::Doroti.Framework.Rendering.Selectable selectable in this.selectables)
        {
            dispatchSelectionEventToChild(selectable, @event);
        }
        currentSelectionEndIndex = -1L;
        currentSelectionStartIndex = -1L;
        return global::Doroti.Framework.Rendering.SelectionResult.none;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.SelectionResult handleGranularlyExtendSelection(global::Doroti.Framework.Rendering.GranularlyExtendSelectionEvent @event)
    {
        DartRuntimePrimitives.Assert(() => (((this.currentSelectionStartIndex == -1L)) == ((this.currentSelectionEndIndex == -1L))));
        if ((this.currentSelectionStartIndex == -1L))
        {
            if (((global::Doroti.Framework.Rendering.GranularlyExtendSelectionEvent)@event).forward)
            {
                currentSelectionStartIndex = currentSelectionEndIndex = 0L;
            }
            else
            {
                currentSelectionStartIndex = currentSelectionEndIndex = (checked((long)(this.selectables.Count)) - 1L);
            }
        }
        long targetIndex = (((global::Doroti.Framework.Rendering.GranularlyExtendSelectionEvent)@event).isEnd ? this.currentSelectionEndIndex : this.currentSelectionStartIndex);
        global::Doroti.Framework.Rendering.SelectionResult result = dispatchSelectionEventToChild(this.selectables[(int)(targetIndex)], @event);
        if (((global::Doroti.Framework.Rendering.GranularlyExtendSelectionEvent)@event).forward)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(result, global::Doroti.Framework.Rendering.SelectionResult.previous)));
            while (((targetIndex < (checked((long)(this.selectables.Count)) - 1L)) && (object.Equals(result, global::Doroti.Framework.Rendering.SelectionResult.next))))
            {
                targetIndex += 1L;
                result = dispatchSelectionEventToChild(this.selectables[(int)(targetIndex)], @event);
                DartRuntimePrimitives.Assert(() => (!object.Equals(result, global::Doroti.Framework.Rendering.SelectionResult.previous)));
            }
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(result, global::Doroti.Framework.Rendering.SelectionResult.next)));
            while (((targetIndex > 0L) && (object.Equals(result, global::Doroti.Framework.Rendering.SelectionResult.previous))))
            {
                targetIndex -= 1L;
                result = dispatchSelectionEventToChild(this.selectables[(int)(targetIndex)], @event);
                DartRuntimePrimitives.Assert(() => (!object.Equals(result, global::Doroti.Framework.Rendering.SelectionResult.next)));
            }
        }
        if (((global::Doroti.Framework.Rendering.GranularlyExtendSelectionEvent)@event).isEnd)
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

    public virtual global::Doroti.Framework.Rendering.SelectionResult handleDirectionallyExtendSelection(global::Doroti.Framework.Rendering.DirectionallyExtendSelectionEvent @event)
    {
        DartRuntimePrimitives.Assert(() => (((this.currentSelectionStartIndex == -1L)) == ((this.currentSelectionEndIndex == -1L))));
        if ((this.currentSelectionStartIndex == -1L))
        {
            currentSelectionStartIndex = currentSelectionEndIndex = (((global::Doroti.Framework.Rendering.DirectionallyExtendSelectionEvent)@event).direction switch { global::Doroti.Framework.Rendering.SelectionExtendDirection.previousLine => (checked((long)(this.selectables.Count)) - 1L), global::Doroti.Framework.Rendering.SelectionExtendDirection.backward => (checked((long)(this.selectables.Count)) - 1L), global::Doroti.Framework.Rendering.SelectionExtendDirection.nextLine => 0L, global::Doroti.Framework.Rendering.SelectionExtendDirection.forward => 0L, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        }
        long targetIndex = (((global::Doroti.Framework.Rendering.DirectionallyExtendSelectionEvent)@event).isEnd ? this.currentSelectionEndIndex : this.currentSelectionStartIndex);
        global::Doroti.Framework.Rendering.SelectionResult result = dispatchSelectionEventToChild(this.selectables[(int)(targetIndex)], @event);
        switch (((global::Doroti.Framework.Rendering.DirectionallyExtendSelectionEvent)@event).direction)
        {
            case global::Doroti.Framework.Rendering.SelectionExtendDirection.previousLine:
                {
                    DartRuntimePrimitives.Assert(() => ((object.Equals(result, global::Doroti.Framework.Rendering.SelectionResult.end)) || (object.Equals(result, global::Doroti.Framework.Rendering.SelectionResult.previous))));
                    if ((object.Equals(result, global::Doroti.Framework.Rendering.SelectionResult.previous)))
                    {
                        if ((targetIndex > 0L))
                        {
                            targetIndex -= 1L;
                            result = dispatchSelectionEventToChild(this.selectables[(int)(targetIndex)], @event.copyWith(direction: global::Doroti.Framework.Rendering.SelectionExtendDirection.backward));
                            DartRuntimePrimitives.Assert(() => (object.Equals(result, global::Doroti.Framework.Rendering.SelectionResult.end)));
                        }
                    }
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionExtendDirection.nextLine:
                {
                    DartRuntimePrimitives.Assert(() => ((object.Equals(result, global::Doroti.Framework.Rendering.SelectionResult.end)) || (object.Equals(result, global::Doroti.Framework.Rendering.SelectionResult.next))));
                    if ((object.Equals(result, global::Doroti.Framework.Rendering.SelectionResult.next)))
                    {
                        if ((targetIndex < (checked((long)(this.selectables.Count)) - 1L)))
                        {
                            targetIndex += 1L;
                            result = dispatchSelectionEventToChild(this.selectables[(int)(targetIndex)], @event.copyWith(direction: global::Doroti.Framework.Rendering.SelectionExtendDirection.forward));
                            DartRuntimePrimitives.Assert(() => (object.Equals(result, global::Doroti.Framework.Rendering.SelectionResult.end)));
                        }
                    }
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionExtendDirection.forward:
            case global::Doroti.Framework.Rendering.SelectionExtendDirection.backward:
                {
                    DartRuntimePrimitives.Assert(() => (object.Equals(result, global::Doroti.Framework.Rendering.SelectionResult.end)));
                    break;
                }
        }
        if (((global::Doroti.Framework.Rendering.DirectionallyExtendSelectionEvent)@event).isEnd)
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

    public virtual global::Doroti.Framework.Rendering.SelectionResult handleSelectionEdgeUpdate(global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent @event)
    {
        if ((object.Equals(@event.type, global::Doroti.Framework.Rendering.SelectionEventType.endEdgeUpdate)))
        {
            return ((this.currentSelectionEndIndex == -1L) ? _initSelection(@event, isEnd: true) : _adjustSelection(@event, isEnd: true));
        }
        return ((this.currentSelectionStartIndex == -1L) ? _initSelection(@event, isEnd: false) : _adjustSelection(@event, isEnd: false));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.SelectionResult dispatchSelectionEvent(global::Doroti.Framework.Rendering.SelectionEvent @event)
    {
        var selectionWillBeInProgress = (@event is not global::Doroti.Framework.Rendering.ClearSelectionEvent);
        if ((!this._selectionInProgress && selectionWillBeInProgress))
        {
            this.selectables.sort(this.compareOrder);
        }
        _selectionInProgress = selectionWillBeInProgress;
        _isHandlingSelectionEvent = true;
        global::Doroti.Framework.Rendering.SelectionResult result = default!;
        switch (((global::Doroti.Framework.Rendering.SelectionEvent)@event).type)
        {
            case global::Doroti.Framework.Rendering.SelectionEventType.startEdgeUpdate:
            case global::Doroti.Framework.Rendering.SelectionEventType.endEdgeUpdate:
                {
                    _extendSelectionInProgress = false;
                    result = handleSelectionEdgeUpdate(((global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent?)(object?)@event)!);
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionEventType.clear:
                {
                    _extendSelectionInProgress = false;
                    result = handleClearSelection(((global::Doroti.Framework.Rendering.ClearSelectionEvent?)(object?)@event)!);
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionEventType.selectAll:
                {
                    _extendSelectionInProgress = false;
                    result = handleSelectAll(((global::Doroti.Framework.Rendering.SelectAllSelectionEvent?)(object?)@event)!);
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionEventType.selectWord:
                {
                    _extendSelectionInProgress = false;
                    result = handleSelectWord(((global::Doroti.Framework.Rendering.SelectWordSelectionEvent?)(object?)@event)!);
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionEventType.selectParagraph:
                {
                    _extendSelectionInProgress = false;
                    result = handleSelectParagraph(((global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent?)(object?)@event)!);
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionEventType.granularlyExtendSelection:
                {
                    _extendSelectionInProgress = true;
                    result = handleGranularlyExtendSelection(((global::Doroti.Framework.Rendering.GranularlyExtendSelectionEvent?)(object?)@event)!);
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionEventType.directionallyExtendSelection:
                {
                    _extendSelectionInProgress = true;
                    result = handleDirectionallyExtendSelection(((global::Doroti.Framework.Rendering.DirectionallyExtendSelectionEvent?)(object?)@event)!);
                    break;
                }
        }
        _isHandlingSelectionEvent = false;
        _updateSelectionGeometry();
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void dispose()
    {
        foreach (global::Doroti.Framework.Rendering.Selectable selectable in this.selectables)
        {
            selectable.removeListener(() => this._handleSelectableGeometryChange());
        }
        selectables = new List<global::Doroti.Framework.Rendering.Selectable>();
        _scheduledSelectableUpdate = false;
        base.dispose();
    }

    public abstract void ensureChildUpdated(global::Doroti.Framework.Rendering.Selectable selectable);
    public virtual global::Doroti.Framework.Rendering.SelectionResult dispatchSelectionEventToChild(global::Doroti.Framework.Rendering.Selectable selectable, global::Doroti.Framework.Rendering.SelectionEvent @event)
    {
        return selectable.dispatchSelectionEvent(@event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Rendering.SelectionResult _initSelection(global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent @event, bool isEnd)
    {
        DartRuntimePrimitives.Assert(() => (((isEnd && (this.currentSelectionEndIndex == -1L))) || ((!isEnd && (this.currentSelectionStartIndex == -1L)))));
        var newIndex = -1L;
        var hasFoundEdgeIndex = false;
        global::Doroti.Framework.Rendering.SelectionResult? result = default!;
        bool? forward = default!;
        long oppositeEdgeIndex = (isEnd ? this.currentSelectionStartIndex : this.currentSelectionEndIndex);
        long index = Math.Max(oppositeEdgeIndex, 0L);
        while (((index >= 0L) && (index < checked((long)(this.selectables.Count)))))
        {
            global::Doroti.Framework.Rendering.Selectable child = this.selectables[(int)(index)];
            global::Doroti.Framework.Rendering.SelectionResult childResult = dispatchSelectionEventToChild(child, @event);
            switch (childResult)
            {
                case global::Doroti.Framework.Rendering.SelectionResult.next:
                    {
                        if ((forward == false))
                        {
                            hasFoundEdgeIndex = true;
                            result = global::Doroti.Framework.Rendering.SelectionResult.end;
                        }
                        else
                        {
                            forward = true;
                            newIndex = index;
                        }
                        break;
                    }
                case global::Doroti.Framework.Rendering.SelectionResult.none:
                    {
                        newIndex = index;
                        break;
                    }
                case global::Doroti.Framework.Rendering.SelectionResult.end:
                    {
                        newIndex = index;
                        result = global::Doroti.Framework.Rendering.SelectionResult.end;
                        hasFoundEdgeIndex = true;
                        break;
                    }
                case global::Doroti.Framework.Rendering.SelectionResult.previous:
                    {
                        if ((index == 0L))
                        {
                            hasFoundEdgeIndex = true;
                            newIndex = 0L;
                            result = global::Doroti.Framework.Rendering.SelectionResult.previous;
                            break;
                        }
                        if ((forward ?? false))
                        {
                            hasFoundEdgeIndex = true;
                            result = global::Doroti.Framework.Rendering.SelectionResult.end;
                        }
                        else
                        {
                            forward = false;
                            newIndex = index;
                        }
                        break;
                    }
                case global::Doroti.Framework.Rendering.SelectionResult.pending:
                    {
                        newIndex = index;
                        result = global::Doroti.Framework.Rendering.SelectionResult.pending;
                        hasFoundEdgeIndex = true;
                        break;
                    }
            }
            if (hasFoundEdgeIndex)
            {
                break;
            }
            index += (((forward ?? true)) ? 1L : -1L);
        }
        if ((newIndex == -1L))
        {
            DartRuntimePrimitives.Assert(() => !System.Linq.Enumerable.Any(this.selectables));
            return global::Doroti.Framework.Rendering.SelectionResult.none;
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
        return (result ?? global::Doroti.Framework.Rendering.SelectionResult.next);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Rendering.SelectionResult _adjustSelection(global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent @event, bool isEnd)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (isEnd)
                {
                    DartRuntimePrimitives.Assert(() => ((this.currentSelectionEndIndex < checked((long)(this.selectables.Count))) && (this.currentSelectionEndIndex >= 0L)));
                    return true;
                }
                DartRuntimePrimitives.Assert(() => ((this.currentSelectionStartIndex < checked((long)(this.selectables.Count))) && (this.currentSelectionStartIndex >= 0L)));
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        global::Doroti.Framework.Rendering.SelectionResult? finalResult = default!;
        var isCurrentEdgeWithinViewport = (isEnd ? (((global::Doroti.Framework.Rendering.SelectionGeometry)this._selectionGeometry).endSelectionPoint is not null) : (((global::Doroti.Framework.Rendering.SelectionGeometry)this._selectionGeometry).startSelectionPoint is not null));
        var isOppositeEdgeWithinViewport = (isEnd ? (((global::Doroti.Framework.Rendering.SelectionGeometry)this._selectionGeometry).startSelectionPoint is not null) : (((global::Doroti.Framework.Rendering.SelectionGeometry)this._selectionGeometry).endSelectionPoint is not null));
        long newIndex = ((isEnd, isCurrentEdgeWithinViewport, isOppositeEdgeWithinViewport) switch { (true, true, true) => this.currentSelectionEndIndex, (true, true, false) => this.currentSelectionEndIndex, (true, false, true) => this.currentSelectionStartIndex, (true, false, false) => 0L, (false, true, true) => this.currentSelectionStartIndex, (false, true, false) => this.currentSelectionStartIndex, (false, false, true) => this.currentSelectionEndIndex, (false, false, false) => 0L });
        bool? forward = default!;
        global::Doroti.Framework.Rendering.SelectionResult currentSelectableResult = default!;
        while ((((newIndex < checked((long)(this.selectables.Count))) && (newIndex >= 0L)) && (finalResult is null)))
        {
            currentSelectableResult = dispatchSelectionEventToChild(this.selectables[(int)(newIndex)], @event);
            switch (currentSelectableResult)
            {
                case global::Doroti.Framework.Rendering.SelectionResult.end:
                case global::Doroti.Framework.Rendering.SelectionResult.pending:
                case global::Doroti.Framework.Rendering.SelectionResult.none:
                    {
                        finalResult = currentSelectableResult;
                        break;
                    }
                case global::Doroti.Framework.Rendering.SelectionResult.next:
                    {
                        if ((forward == false))
                        {
                            newIndex += 1L;
                            finalResult = global::Doroti.Framework.Rendering.SelectionResult.end;
                        }
                        else
                        {
                            if ((newIndex == (checked((long)(this.selectables.Count)) - 1L)))
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
                case global::Doroti.Framework.Rendering.SelectionResult.previous:
                    {
                        if ((forward ?? false))
                        {
                            newIndex -= 1L;
                            finalResult = global::Doroti.Framework.Rendering.SelectionResult.end;
                        }
                        else
                        {
                            if ((newIndex == 0L))
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

public delegate Widget SelectableRegionContextMenuBuilder(BuildContext context, SelectableRegionState selectableRegionState);

public enum SelectableRegionSelectionStatus
{
    changing,
    finalized
}

internal class _SelectableRegionSelectionStatusNotifier__selectable_region : global::Doroti.Framework.Foundation.ChangeNotifier, global::Doroti.Framework.Foundation.ValueListenable<SelectableRegionSelectionStatus>
{
    internal virtual SelectableRegionSelectionStatus _selectableRegionSelectionStatus { get; set; } = SelectableRegionSelectionStatus.finalized;

    internal _SelectableRegionSelectionStatusNotifier__selectable_region()
    {
    }

    public virtual SelectableRegionSelectionStatus value
    {
        get => this._selectableRegionSelectionStatus;
        set
        {
            var newStatus = value;
            DartRuntimePrimitives.Assert(() => (((object.Equals(newStatus, SelectableRegionSelectionStatus.finalized)) && (object.Equals(this.value, SelectableRegionSelectionStatus.changing))) || (object.Equals(newStatus, SelectableRegionSelectionStatus.changing))), () => (object?)"Attempting to finalize the selection when it is already finalized.");
            _selectableRegionSelectionStatus = newStatus;
            notifyListeners();
        }
    }
}

public class SelectableRegionSelectionStatusScope : InheritedWidget
{
    public virtual global::Doroti.Framework.Foundation.ValueListenable<SelectableRegionSelectionStatus> selectionStatusNotifier { get; private set; } = default!;

    public SelectableRegionSelectionStatusScope(global::Doroti.Framework.Foundation.ValueListenable<SelectableRegionSelectionStatus> selectionStatusNotifier, Widget child) : base(child: child)
    {
        this.selectionStatusNotifier = selectionStatusNotifier;
    }

    public static global::Doroti.Framework.Foundation.ValueListenable<SelectableRegionSelectionStatus>? maybeOf(BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<SelectableRegionSelectionStatusScope>()?.selectionStatusNotifier;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (SelectableRegionSelectionStatusScope)(object)oldWidget;
        return (!object.Equals(this.selectionStatusNotifier, ((SelectableRegionSelectionStatusScope)__oldWidget).selectionStatusNotifier));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SelectionListener : StatefulWidget
{
    public virtual SelectionListenerNotifier selectionNotifier { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public SelectionListener(global::Doroti.Framework.Foundation.Key? key = null, SelectionListenerNotifier selectionNotifier = default!, Widget child = default!) : base(key: key)
    {
        this.selectionNotifier = selectionNotifier;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SelectionListenerState__selectable_region());
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
                __late__selectionDelegate = new _SelectionListenerDelegate__selectable_region(selectionNotifier: ((SelectionListener)(object)this.widget).selectionNotifier);
                __late__selectionDelegate_initialized = true;
            }
            return __late__selectionDelegate;
        }
    }

    public override void didUpdateWidget(SelectionListener oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((SelectionListener)oldWidget).selectionNotifier, ((SelectionListener)(object)this.widget).selectionNotifier)))
        {
            this._selectionDelegate._setNotifier(((SelectionListener)(object)this.widget).selectionNotifier);
        }
    }

    public override void dispose()
    {
        this._selectionDelegate.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new SelectionContainer(@delegate: this._selectionDelegate, child: ((SelectionListener)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SelectionListenerDelegate__selectable_region : StaticSelectionContainerDelegate, SelectionDetails
{
    internal virtual global::Doroti.Framework.Rendering.SelectionGeometry? _initialSelectionGeometry { get; set; } = default;
    internal virtual SelectionListenerNotifier _selectionNotifier { get; set; } = default!;

    internal _SelectionListenerDelegate__selectable_region(SelectionListenerNotifier selectionNotifier)
    {
        this._selectionNotifier = selectionNotifier;
    }

    internal virtual void _setNotifier(SelectionListenerNotifier newNotifier)
    {
        this._selectionNotifier._unregisterSelectionListenerDelegate();
        _selectionNotifier = newNotifier;
        this._selectionNotifier._registerSelectionListenerDelegate(this);
    }

    public virtual void notifyListeners()
    {
        base.notifyListeners();
        if (((this._initialSelectionGeometry is null) && !((global::Doroti.Framework.Rendering.SelectionGeometry)this.value).hasSelection))
        {
            _initialSelectionGeometry = this.value;
            return;
        }
        this._selectionNotifier.notifyListeners();
    }

    public override void dispose()
    {
        this._selectionNotifier._unregisterSelectionListenerDelegate();
        _initialSelectionGeometry = null;
        base.dispose();
    }

    public virtual global::Doroti.Framework.Rendering.SelectedContentRange? range => getSelection();
    public virtual global::Doroti.Framework.Rendering.SelectionStatus status => ((global::Doroti.Framework.Rendering.SelectionGeometry)this.value).status;
}

public interface SelectionDetails
{
    public global::Doroti.Framework.Rendering.SelectedContentRange? range { get; }
    public global::Doroti.Framework.Rendering.SelectionStatus status { get; }
}

public class SelectionListenerNotifier : global::Doroti.Framework.Foundation.ChangeNotifier
{
    internal virtual _SelectionListenerDelegate__selectable_region? _selectionDelegate { get; set; } = default;

    public virtual SelectionDetails selection => DartRuntimePrimitives.ConvertValue<SelectionDetails>((this._selectionDelegate ?? throw new Exception("Selection client has not been registered to this notifier.")));
    public virtual bool registered => DartRuntimePrimitives.ConvertValue<bool>((this._selectionDelegate is not null));
    internal virtual void _registerSelectionListenerDelegate(_SelectionListenerDelegate__selectable_region selectionDelegate)
    {
        DartRuntimePrimitives.Assert(() => !this.registered, () => (object?)"This SelectionListenerNotifier is already registered to another SelectionListener. Try providing a new SelectionListenerNotifier.");
        _selectionDelegate = selectionDelegate;
    }

    internal virtual void _unregisterSelectionListenerDelegate()
    {
        _selectionDelegate = null;
    }

    public virtual void dispose()
    {
        _unregisterSelectionListenerDelegate();
        base.dispose();
    }

    public virtual void addListener(global::System.Action listener)
    {
        base.addListener(() => listener());
    }

}

