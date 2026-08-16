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
        var canCopy__12928 = (object.Equals(((global::Doroti.Framework.Rendering.SelectionGeometry)selectionGeometry).status, global::Doroti.Framework.Rendering.SelectionStatus.uncollapsed));
        bool canSelectAll__13010 = ((global::Doroti.Framework.Rendering.SelectionGeometry)selectionGeometry).hasContent;
        bool platformCanShare__13123 = (!global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb && (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform switch { global::Doroti.Framework.Foundation.TargetPlatform.android => (object.Equals(((global::Doroti.Framework.Rendering.SelectionGeometry)selectionGeometry).status, global::Doroti.Framework.Rendering.SelectionStatus.uncollapsed)), global::Doroti.Framework.Foundation.TargetPlatform.macOS or global::Doroti.Framework.Foundation.TargetPlatform.fuchsia or global::Doroti.Framework.Foundation.TargetPlatform.linux => false, global::Doroti.Framework.Foundation.TargetPlatform.windows => false, global::Doroti.Framework.Foundation.TargetPlatform.iOS => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        bool canShare__13790 = ((onShare is not null) && platformCanShare__13123);
        var showShareBeforeSelectAll__13918 = (object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.android));
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
        this._gestureRecognizers[typeof(global::Doroti.Framework.Gestures.TapGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.TapGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.TapGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.TapGestureRecognizer(debugOwner: this))), ((global::System.Action<global::Doroti.Framework.Gestures.TapGestureRecognizer>)((instance) => {
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
        Orientation orientation__20389 = MediaQuery.orientationOf(this.context);
        if ((this._lastOrientation is null))
        {
            _lastOrientation = orientation__20389;
            return;
        }
        if ((!object.Equals(orientation__20389, this._lastOrientation)))
        {
            _lastOrientation = orientation__20389;
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
        global::Doroti.Framework.Rendering.SelectionGeometry geometry__22896 = this._selectionDelegate.value;
        global::Doroti.Framework.Services.TextSelection selection__22957 = (((global::Doroti.Framework.Rendering.SelectionGeometry)geometry__22896).status switch { global::Doroti.Framework.Rendering.SelectionStatus.uncollapsed => new global::Doroti.Framework.Services.TextSelection(baseOffset: 0L, extentOffset: 1L), global::Doroti.Framework.Rendering.SelectionStatus.collapsed => new global::Doroti.Framework.Services.TextSelection(baseOffset: 0L, extentOffset: 1L), global::Doroti.Framework.Rendering.SelectionStatus.none => global::Doroti.Framework.Services.TextSelection.CreateCollapsed(offset: 1L), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        textEditingValue = new global::Doroti.Framework.Services.TextEditingValue(text: "__", selection: selection__22957);
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
        var maxConsecutiveTap__25100 = 3L;
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                {
                    if (((this._lastPointerDeviceKind is not null) && (!object.Equals(this._lastPointerDeviceKind, PointerDeviceKind.mouse))))
                    {
                        maxConsecutiveTap__25100 = 2L;
                    }
                    return ((rawCount <= maxConsecutiveTap__25100) ? rawCount : ((((rawCount % maxConsecutiveTap__25100) == 0L) ? maxConsecutiveTap__25100 : (rawCount % maxConsecutiveTap__25100))));
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                {
                    return ((rawCount <= maxConsecutiveTap__25100) ? rawCount : ((((rawCount % maxConsecutiveTap__25100) == 0L) ? maxConsecutiveTap__25100 : (rawCount % maxConsecutiveTap__25100))));
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    return Math.Min(rawCount, maxConsecutiveTap__25100);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _initMouseGestureRecognizer()
    {
        this._gestureRecognizers[typeof(global::Doroti.Framework.Gestures.TapAndPanGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.TapAndPanGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.TapAndPanGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.TapAndPanGestureRecognizer(debugOwner: this, supportedDevices: new HashSet<PointerDeviceKind> { PointerDeviceKind.mouse }))), ((global::System.Action<global::Doroti.Framework.Gestures.TapAndPanGestureRecognizer>)((instance) => {
DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.TapAndPanGestureRecognizer>)(() =>
{            var __cascade = instance;
            __cascade.onTapTrackStart = this._onTapTrackStart;
            __cascade.onTapTrackReset = this._onTapTrackReset;
            __cascade.onTapDown = this._startNewMouseSelectionGesture;
            __cascade.onTapUp = this._handleMouseTapUp;
            __cascade.onDragStart = this._handleMouseDragStart;
            __cascade.onDragUpdate = this._handleMouseDragUpdate;
            __cascade.onDragEnd = this._handleMouseDragEnd;
            __cascade.onCancel = this.clearSelection;
            __cascade.dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.down;
            return __cascade;        }))());
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
        this._gestureRecognizers[typeof(global::Doroti.Framework.Gestures.TapAndHorizontalDragGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.TapAndHorizontalDragGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.TapAndHorizontalDragGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.TapAndHorizontalDragGestureRecognizer(debugOwner: this, supportedDevices: System.Enum.GetValues<PointerDeviceKind>().ToList().where(((device) => {
return (!object.Equals(device, PointerDeviceKind.mouse));
throw new InvalidOperationException("Dart closure completed without a value.");
})).toSet()))), ((global::System.Action<global::Doroti.Framework.Gestures.TapAndHorizontalDragGestureRecognizer>)((instance) => {
DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.TapAndHorizontalDragGestureRecognizer>)(() =>
{            var __cascade = instance;
            __cascade.eagerVictoryOnDrag = (!object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS));
            __cascade.onTapDown = this._startNewMouseSelectionGesture;
            __cascade.onTapUp = this._handleMouseTapUp;
            __cascade.onDragStart = this._handleMouseDragStart;
            __cascade.onDragUpdate = this._handleMouseDragUpdate;
            __cascade.onDragEnd = this._handleMouseDragEnd;
            __cascade.onCancel = this.clearSelection;
            __cascade.dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.down;
            return __cascade;        }))());
})));
        this._gestureRecognizers[typeof(global::Doroti.Framework.Gestures.LongPressGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.LongPressGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.LongPressGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.LongPressGestureRecognizer(debugOwner: this, supportedDevices: Selectable_regionLibrary._kLongPressSelectionDevices))), ((global::System.Action<global::Doroti.Framework.Gestures.LongPressGestureRecognizer>)((instance) => {
DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.LongPressGestureRecognizer>)(() =>
{            var __cascade = instance;
            __cascade.onLongPressStart = this._handleTouchLongPressStart;
            __cascade.onLongPressMoveUpdate = this._handleTouchLongPressMoveUpdate;
            __cascade.onLongPressEnd = this._handleTouchLongPressEnd;
            return __cascade;        }))());
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
                                bool isShiftPressedValid__32233 = (this._isShiftPressed && (this._selectionDelegate.value.startSelectionPoint is not null));
                                if (isShiftPressedValid__32233)
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
        bool isPointerPrecise__38694 = SelectableRegionState._isPrecisePointerDevice(DartRuntimePrimitives.RequireValue(this._lastPointerDeviceKind));
        bool shouldShowSelectionOverlayOnMobile__39052 = !isPointerPrecise__38694;
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                {
                    if (shouldShowSelectionOverlayOnMobile__39052)
                    {
                        _showHandles();
                        _showToolbar();
                    }
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    if (shouldShowSelectionOverlayOnMobile__39052)
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
            bool toolbarIsVisible__40099 = (this._selectionOverlay?.toolbarIsVisible ?? false);
            if (toolbarIsVisible__40099)
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
                    bool isPointerPrecise__40938 = SelectableRegionState._isPrecisePointerDevice(((global::Doroti.Framework.Gestures.TapDragUpDetails)details).kind);
                    switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
                    {
                        case global::Doroti.Framework.Foundation.TargetPlatform.android:
                        case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                            {
                                if (!isPointerPrecise__40938)
                                {
                                    _showHandles();
                                    _showToolbar();
                                }
                                break;
                            }
                        case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                            {
                                if (!isPointerPrecise__40938)
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
        global::Doroti.Framework.Rendering.SelectedContent? content__42306 = ((global::Doroti.Framework.Rendering.SelectedContent?)(object?)this._selectable?.getSelectedContent());
        if ((this._lastSelectedContent?.plainText != content__42306?.plainText))
        {
            _lastSelectedContent = content__42306;
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
        foreach (global::Doroti.Ui.Rect selectionRect__43815 in this._selectionDelegate.value.selectionRects)
        {
            Matrix4 transform__43895 = ((Matrix4)(object?)this._selectable!.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)));
            global::Doroti.Ui.Rect globalRect__43959 = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(transform__43895, selectionRect__43815));
            if (globalRect__43959.contains(globalPosition))
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleRightClickDown(global::Doroti.Framework.Gestures.TapDownDetails details)
    {
        global::Doroti.Ui.Offset? previousSecondaryTapDownPosition__44205 = ((global::Doroti.Ui.Offset?)(object?)this._lastSecondaryTapDownPosition);
        bool toolbarIsVisible__44286 = (this._selectionOverlay?.toolbarIsVisible ?? false);
        _lastSecondaryTapDownPosition = ((global::Doroti.Framework.Gestures.TapDownDetails)details).globalPosition;
        this._focusNode.requestFocus();
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    bool lastSecondaryTapDownPositionWasOnActiveSelection__44748 = _positionIsOnActiveSelection(globalPosition: ((global::Doroti.Framework.Gestures.TapDownDetails)details).globalPosition);
                    if (lastSecondaryTapDownPositionWasOnActiveSelection__44748)
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
                    if (((object.Equals(previousSecondaryTapDownPosition__44205, this._lastSecondaryTapDownPosition)) && toolbarIsVisible__44286))
                    {
                        hideToolbar();
                        return;
                    }
                    _selectWordAt(offset: DartRuntimePrimitives.RequireValue(this._lastSecondaryTapDownPosition));
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                {
                    if (toolbarIsVisible__44286)
                    {
                        hideToolbar();
                        return;
                    }
                    bool lastSecondaryTapDownPositionWasOnActiveSelection__46006 = _positionIsOnActiveSelection(globalPosition: ((global::Doroti.Framework.Gestures.TapDownDetails)details).globalPosition);
                    if (!lastSecondaryTapDownPositionWasOnActiveSelection__46006)
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
            global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timeStamp) => {
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
        bool draggingHandles__48379 = ((this._selectionOverlay is not null) && ((this._selectionOverlay!.isDraggingStartHandle || this._selectionOverlay!.isDraggingEndHandle)));
        if (!draggingHandles__48379)
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
            global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timeStamp) => {
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
        global::Doroti.Ui.Offset localPosition__50870 = ((global::Doroti.Ui.Offset)(object?)this._selectionDelegate.value.startSelectionPoint!.localPosition);
        Matrix4 globalTransform__50965 = ((Matrix4)(object?)this._selectable!.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)));
        _selectionStartHandleDragPosition = MatrixUtils.transformPoint(globalTransform__50965, localPosition__50870);
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
        global::Doroti.Ui.Offset localPosition__52237 = ((global::Doroti.Ui.Offset)(object?)this._selectionDelegate.value.endSelectionPoint!.localPosition);
        Matrix4 globalTransform__52330 = ((Matrix4)(object?)this._selectable!.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)));
        _selectionEndHandleDragPosition = MatrixUtils.transformPoint(globalTransform__52330, localPosition__52237);
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
        Vector3 globalTransform__53564 = this._selectable!.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)).getTranslation();
        var globalTransformAsOffset__53644 = new global::Doroti.Ui.Offset(globalTransform__53564.x, globalTransform__53564.y);
        global::Doroti.Ui.Offset globalSelectionPointPosition__53733 = ((global::Doroti.Ui.Offset)(object?)(((global::Doroti.Framework.Rendering.SelectionPoint)selectionPoint).localPosition + globalTransformAsOffset__53644));
        var caretRect__53838 = global::Doroti.Ui.Rect.fromLTWH(globalSelectionPointPosition__53733.dx, (globalSelectionPointPosition__53733.dy - ((global::Doroti.Framework.Rendering.SelectionPoint)selectionPoint).lineHeight), 0, ((global::Doroti.Framework.Rendering.SelectionPoint)selectionPoint).lineHeight);
        return new MagnifierInfo(globalGesturePosition: globalGesturePosition, caretRect: caretRect__53838, fieldBounds: (globalTransformAsOffset__53644 & this._selectable!.size), currentLineBoundaries: (globalTransformAsOffset__53644 & this._selectable!.size));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _createSelectionOverlay()
    {
        DartRuntimePrimitives.Assert(() => this._hasSelectionOverlayGeometry);
        if ((this._selectionOverlay is not null))
        {
            return;
        }
        global::Doroti.Framework.Rendering.SelectionPoint? start__54437 = this._selectionDelegate.value.startSelectionPoint;
        global::Doroti.Framework.Rendering.SelectionPoint? end__54517 = this._selectionDelegate.value.endSelectionPoint;
        _selectionOverlay = new SelectionOverlay(context: this.context, debugRequiredFor: this.widget, startHandleType: (start__54437?.handleType ?? global::Doroti.Framework.Rendering.TextSelectionHandleType.collapsed), lineHeightAtStart: (start__54437?.lineHeight ?? end__54517!.lineHeight), onStartHandleDragStart: (global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>)this._handleSelectionStartHandleDragStart, onStartHandleDragUpdate: (global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)this._handleSelectionStartHandleDragUpdate, onStartHandleDragEnd: (global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>)this._onAnyDragEnd, endHandleType: (end__54517?.handleType ?? global::Doroti.Framework.Rendering.TextSelectionHandleType.collapsed), lineHeightAtEnd: (end__54517?.lineHeight ?? start__54437!.lineHeight), onEndHandleDragStart: (global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>)this._handleSelectionEndHandleDragStart, onEndHandleDragUpdate: (global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)this._handleSelectionEndHandleDragUpdate, onEndHandleDragEnd: (global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>)this._onAnyDragEnd, selectionEndpoints: this.selectionEndpoints, selectionControls: ((SelectableRegion)(object)this.widget).selectionControls, selectionDelegate: this, clipboardStatus: ((ClipboardStatusNotifier)(object)null), startHandleLayerLink: this._startHandleLayerLink, endHandleLayerLink: this._endHandleLayerLink, toolbarLayerLink: this._toolbarLayerLink, magnifierConfiguration: ((SelectableRegion)(object)this.widget).magnifierConfiguration);
    }

    internal virtual void _updateSelectionOverlay()
    {
        if ((this._selectionOverlay is null))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => this._hasSelectionOverlayGeometry);
        global::Doroti.Framework.Rendering.SelectionPoint? start__55826 = this._selectionDelegate.value.startSelectionPoint;
        global::Doroti.Framework.Rendering.SelectionPoint? end__55906 = this._selectionDelegate.value.endSelectionPoint;
        DartRuntimePrimitives.Ignore(((Func<SelectionOverlay>)(() =>
{            var __cascade = this._selectionOverlay!;
            __cascade.startHandleType = (start__55826?.handleType ?? global::Doroti.Framework.Rendering.TextSelectionHandleType.left);
            __cascade.lineHeightAtStart = (start__55826?.lineHeight ?? end__55906!.lineHeight);
            __cascade.endHandleType = (end__55906?.handleType ?? global::Doroti.Framework.Rendering.TextSelectionHandleType.right);
            __cascade.lineHeightAtEnd = (end__55906?.lineHeight ?? start__55826!.lineHeight);
            __cascade.selectionEndpoints = this.selectionEndpoints;
            return __cascade;        }))());
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
        this._selectionOverlay!.showToolbar(context: this.context, contextMenuBuilder: ((global::System.Func<BuildContext, Widget>?)((context) => {
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
        global::Doroti.Framework.Rendering.SelectedContent? data__66534 = ((global::Doroti.Framework.Rendering.SelectedContent?)(object?)this._selectable?.getSelectedContent());
        if ((data__66534 is null))
        {
            return;
        }
        await Clipboard.setData(new global::Doroti.Framework.Services.ClipboardData(text: ((global::Doroti.Framework.Rendering.SelectedContent)data__66534).plainText));
    }

    internal async virtual Future _share()
    {
        global::Doroti.Framework.Rendering.SelectedContent? data__66750 = ((global::Doroti.Framework.Rendering.SelectedContent?)(object?)this._selectable?.getSelectedContent());
        if ((data__66750 is null))
        {
            return;
        }
        await global::Doroti.Framework.Services.SystemChannels.platform.invokeMethod<object>("Share.invoke", ((global::Doroti.Framework.Rendering.SelectedContent)data__66750).plainText);
    }

    public virtual TextSelectionToolbarAnchors contextMenuAnchors
    {
        get
        {
            if ((this._lastSecondaryTapDownPosition is not null))
            {
                var anchors__67246 = new TextSelectionToolbarAnchors(primaryAnchor: DartRuntimePrimitives.RequireValue(this._lastSecondaryTapDownPosition));
                _lastSecondaryTapDownPosition = null;
                return anchors__67246;
            }
            var renderBox__67586 = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)this.context.findRenderObject()!)!;
            return TextSelectionToolbarAnchors.CreateFromSelection(renderBox: renderBox__67586, startGlyphHeight: this.startGlyphHeight, endGlyphHeight: this.endGlyphHeight, selectionEndpoints: this.selectionEndpoints);
            return default!;
        }
    }
    internal virtual bool _determineIsAdjustingSelectionEnd(bool forward)
    {
        if ((this._adjustingSelectionEnd is not null))
        {
            return DartRuntimePrimitives.RequireValue(this._adjustingSelectionEnd);
        }
        bool isReversed__68050 = default!;
        global::Doroti.Framework.Rendering.SelectionPoint start__68087 = this._selectionDelegate.value.startSelectionPoint!;
        global::Doroti.Framework.Rendering.SelectionPoint end__68167 = this._selectionDelegate.value.endSelectionPoint!;
        if ((((global::Doroti.Framework.Rendering.SelectionPoint)start__68087).localPosition.dy > ((global::Doroti.Framework.Rendering.SelectionPoint)end__68167).localPosition.dy))
        {
            isReversed__68050 = true;
        }
        else
        {
            if ((((global::Doroti.Framework.Rendering.SelectionPoint)start__68087).localPosition.dy < ((global::Doroti.Framework.Rendering.SelectionPoint)end__68167).localPosition.dy))
            {
                isReversed__68050 = false;
            }
            else
            {
                isReversed__68050 = (((global::Doroti.Framework.Rendering.SelectionPoint)start__68087).localPosition.dx > ((global::Doroti.Framework.Rendering.SelectionPoint)end__68167).localPosition.dx);
            }
        }
        return DartRuntimePrimitives.RequireValue(_adjustingSelectionEnd = (forward != isReversed__68050));
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
        bool adjustingSelectionExtend__69361 = _determineIsAdjustingSelectionEnd(forward);
        global::Doroti.Framework.Rendering.SelectionPoint baseLinePoint__69457 = (adjustingSelectionExtend__69361 ? this._selectionDelegate.value.endSelectionPoint! : this._selectionDelegate.value.startSelectionPoint!);
        _directionalHorizontalBaseline ??= ((global::Doroti.Framework.Rendering.SelectionPoint)baseLinePoint__69457).localPosition.dx;
        global::Doroti.Ui.Offset globalSelectionPointOffset__69697 = ((global::Doroti.Ui.Offset)(object?)MatrixUtils.transformPoint(((Matrix4)((dynamic)this.context.findRenderObject()!).getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null))), new global::Doroti.Ui.Offset(DartRuntimePrimitives.RequireValue(this._directionalHorizontalBaseline), 0)));
        this._selectable?.dispatchSelectionEvent(new global::Doroti.Framework.Rendering.DirectionallyExtendSelectionEvent(isEnd: DartRuntimePrimitives.RequireValue(this._adjustingSelectionEnd), direction: (forward ? global::Doroti.Framework.Rendering.SelectionExtendDirection.nextLine : global::Doroti.Framework.Rendering.SelectionExtendDirection.previousLine), dx: globalSelectionPointOffset__69697.dx));
        _updateSelectedContentIfNeeded();
        this._selectionStatusNotifier.value = SelectableRegionSelectionStatus.changing;
        _finalizeSelectableRegionStatus();
    }

    public virtual List<ContextMenuButtonItem> contextMenuButtonItems
    {
        get
        {
            return ((Func<List<ContextMenuButtonItem>>)(() =>
{            var __cascade = SelectableRegion.getSelectableButtonItems(selectionGeometry: this._selectionDelegate.value, onCopy: ((global::System.Action)(() => {
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
})), onSelectAll: ((global::System.Action)(() => {
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
})), onShare: ((global::System.Action)(() => {
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
            return __cascade;        }))();
            return default!;
        }
    }
    internal virtual List<ContextMenuButtonItem> _textProcessingActionButtonItems
    {
        get
        {
            var buttonItems__73199 = new List<ContextMenuButtonItem>();
            global::Doroti.Framework.Rendering.SelectedContent? data__73267 = ((global::Doroti.Framework.Rendering.SelectedContent?)(object?)this._selectable?.getSelectedContent());
            if ((data__73267 is null))
            {
                return buttonItems__73199;
            }
            foreach (global::Doroti.Framework.Services.ProcessTextAction action__73399 in this._processTextActions)
            {
                buttonItems__73199.Add(new ContextMenuButtonItem(label: ((global::Doroti.Framework.Services.ProcessTextAction)action__73399).label, onPressed: ((global::System.Action)(async () => {
string selectedText__73574 = ((global::Doroti.Framework.Rendering.SelectedContent)data__73267).plainText;
if ((selectedText__73574.Length != 0))
{
    await this._processTextService.processTextAction(((global::Doroti.Framework.Services.ProcessTextAction)action__73399).id, selectedText__73574, true);
    hideToolbar();
}
}))));
            }
            return buttonItems__73199;
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
            global::Doroti.Framework.Rendering.SelectionPoint? start__74334 = this._selectionDelegate.value.startSelectionPoint;
            global::Doroti.Framework.Rendering.SelectionPoint? end__74414 = this._selectionDelegate.value.endSelectionPoint;
            List<global::Doroti.Framework.Rendering.TextSelectionPoint> points__74498 = default!;
            global::Doroti.Ui.Offset startLocalPosition__74523 = ((global::Doroti.Ui.Offset)(object?)(start__74334?.localPosition ?? end__74414!.localPosition));
            global::Doroti.Ui.Offset endLocalPosition__74605 = ((global::Doroti.Ui.Offset)(object?)(end__74414?.localPosition ?? start__74334!.localPosition));
            if ((startLocalPosition__74523.dy > endLocalPosition__74605.dy))
            {
                points__74498 = new List<global::Doroti.Framework.Rendering.TextSelectionPoint> { new global::Doroti.Framework.Rendering.TextSelectionPoint(endLocalPosition__74605, TextDirection.ltr), new global::Doroti.Framework.Rendering.TextSelectionPoint(startLocalPosition__74523, TextDirection.ltr) };
            }
            else
            {
                points__74498 = new List<global::Doroti.Framework.Rendering.TextSelectionPoint> { new global::Doroti.Framework.Rendering.TextSelectionPoint(startLocalPosition__74523, TextDirection.ltr), new global::Doroti.Framework.Rendering.TextSelectionPoint(endLocalPosition__74605, TextDirection.ltr) };
            }
            return points__74498;
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
        Widget result__79288 = ((Widget)(object?)new SelectableRegionSelectionStatusScope(selectionStatusNotifier: this._selectionStatusNotifier, child: new SelectionContainer(registrar: this, @delegate: this._selectionDelegate, child: ((SelectableRegion)(object)this.widget).child)));
        if (this._webContextMenuEnabled)
        {
            result__79288 = DartRuntimePrimitives.ConvertValue<Widget>(new PlatformSelectableRegionContextMenuIo(child: result__79288));
        }
        return ((Widget)(object?)new TapRegion(groupId: typeof(SelectableRegion), onTapOutside: ((global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>)((@event) => {
if (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb)
{
    this._focusNode.unfocus();
}
})), child: new CompositedTransformTarget(link: this._toolbarLayerLink, child: new RawGestureDetector(gestures: this._gestureRecognizers, behavior: global::Doroti.Framework.Rendering.HitTestBehavior.translucent, excludeFromSemantics: true, child: new Actions(actions: this._actions, child: Focus.CreateWithExternalFocusNode(includeSemantics: false, focusNode: this._focusNode, child: result__79288))))));
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
        if (this.callingAction is object callingAction__81081)
        {
            return ((dynamic)callingAction__81081).invoke(intent);
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
        long start__86948 = Math.Min(this.currentSelectionStartIndex, this.currentSelectionEndIndex);
        long end__87029 = Math.Max(this.currentSelectionStartIndex, this.currentSelectionEndIndex);
        for (var index__87107 = start__86948; (index__87107 <= end__87029); index__87107 += 1L)
        {
            didReceiveSelectionEventFor(selectable: this.selectables[(int)(index__87107)]);
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
            global::Doroti.Framework.Rendering.Selectable start__88061 = this.selectables[(int)(this.currentSelectionStartIndex)];
            global::Doroti.Ui.Offset localStartEdge__88129 = ((global::Doroti.Ui.Offset)(object?)(start__88061.value.startSelectionPoint!.localPosition + new global::Doroti.Ui.Offset(0, (-start__88061.value.startSelectionPoint!.lineHeight / 2L))));
            updateLastSelectionEdgeLocation(globalSelectionEdgeLocation: MatrixUtils.transformPoint(start__88061.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)), localStartEdge__88129), forEnd: false);
        }
        if (((this.currentSelectionEndIndex != -1L) && this.selectables[(int)(this.currentSelectionEndIndex)].value.hasSelection))
        {
            global::Doroti.Framework.Rendering.Selectable end__88626 = this.selectables[(int)(this.currentSelectionEndIndex)];
            global::Doroti.Ui.Offset localEndEdge__88690 = ((global::Doroti.Ui.Offset)(object?)(end__88626.value.endSelectionPoint!.localPosition + new global::Doroti.Ui.Offset(0, (-end__88626.value.endSelectionPoint!.lineHeight / 2L))));
            updateLastSelectionEdgeLocation(globalSelectionEdgeLocation: MatrixUtils.transformPoint(end__88626.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)), localEndEdge__88690), forEnd: true);
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
        global::Doroti.Framework.Rendering.SelectionResult result__90287 = base.handleSelectAll(@event);
        didReceiveSelectionBoundaryEvents();
        return result__90287;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.SelectionResult handleSelectWord(global::Doroti.Framework.Rendering.SelectWordSelectionEvent @event)
    {
        global::Doroti.Framework.Rendering.SelectionResult result__90498 = base.handleSelectWord(@event);
        didReceiveSelectionBoundaryEvents();
        return result__90498;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.SelectionResult handleSelectParagraph(global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent @event)
    {
        global::Doroti.Framework.Rendering.SelectionResult result__90720 = base.handleSelectParagraph(@event);
        didReceiveSelectionBoundaryEvents();
        return result__90720;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.SelectionResult handleClearSelection(global::Doroti.Framework.Rendering.ClearSelectionEvent @event)
    {
        global::Doroti.Framework.Rendering.SelectionResult result__90936 = base.handleClearSelection(@event);
        clearInternalSelectionState();
        return result__90936;
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
            var synthesizedEvent__93145 = global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent.CreateForEnd(globalPosition: DartRuntimePrimitives.RequireValue(this._lastEndEdgeUpdateGlobalPosition));
            if ((this.currentSelectionEndIndex == -1L))
            {
                handleSelectionEdgeUpdate(synthesizedEvent__93145);
            }
            selectable.dispatchSelectionEvent(synthesizedEvent__93145);
        }
        if (((this._lastStartEdgeUpdateGlobalPosition is not null) && this._hasReceivedStartEvent.Add(selectable)))
        {
            var synthesizedEvent__93543 = new global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent(globalPosition: DartRuntimePrimitives.RequireValue(this._lastStartEdgeUpdateGlobalPosition));
            if ((this.currentSelectionStartIndex == -1L))
            {
                handleSelectionEdgeUpdate(synthesizedEvent__93543);
            }
            selectable.dispatchSelectionEvent(synthesizedEvent__93543);
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
        HashSet<global::Doroti.Framework.Rendering.Selectable> selectableSet__94304 = this.selectables.toSet();
        this._hasReceivedEndEvent.removeWhere(((selectable) => !selectableSet__94304.Contains(selectable)));
        this._hasReceivedStartEvent.removeWhere(((selectable) => !selectableSet__94304.Contains(selectable)));
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
        List<global::Doroti.Framework.Rendering.Selectable> mergingSelectables__98387 = ((Func<List<global::Doroti.Framework.Rendering.Selectable>>)(() =>
{            var __cascade = this._additions.ToList();
            __cascade.sort(this.compareOrder);
            return __cascade;        }))().ToList();
        List<global::Doroti.Framework.Rendering.Selectable> existingSelectables__98476 = this.selectables.ToList();
        selectables = new List<global::Doroti.Framework.Rendering.Selectable>();
        var mergingIndex__98553 = 0L;
        var existingIndex__98579 = 0L;
        long selectionStartIndex__98606 = this.currentSelectionStartIndex;
        long selectionEndIndex__98664 = this.currentSelectionEndIndex;
        while (((mergingIndex__98553 < checked((long)(mergingSelectables__98387.Count))) || (existingIndex__98579 < checked((long)(existingSelectables__98476.Count)))))
        {
            if (((mergingIndex__98553 >= checked((long)(mergingSelectables__98387.Count))) || (((existingIndex__98579 < checked((long)(existingSelectables__98476.Count))) && (this.compareOrder(existingSelectables__98476[(int)(existingIndex__98579)], mergingSelectables__98387[(int)(mergingIndex__98553)]) < 0L)))))
            {
                if ((existingIndex__98579 == this.currentSelectionStartIndex))
                {
                    selectionStartIndex__98606 = checked((long)(this.selectables.Count));
                }
                if ((existingIndex__98579 == this.currentSelectionEndIndex))
                {
                    selectionEndIndex__98664 = checked((long)(this.selectables.Count));
                }
                this.selectables.Add(existingSelectables__98476[(int)(existingIndex__98579)]);
                existingIndex__98579 += 1L;
                continue;
            }
            global::Doroti.Framework.Rendering.Selectable mergingSelectable__99565 = mergingSelectables__98387[(int)(mergingIndex__98553)];
            if (((existingIndex__98579 < Math.Max(this.currentSelectionStartIndex, this.currentSelectionEndIndex)) && (existingIndex__98579 > Math.Min(this.currentSelectionStartIndex, this.currentSelectionEndIndex))))
            {
                ensureChildUpdated(mergingSelectable__99565);
            }
            mergingSelectable__99565.addListener(() => this._handleSelectableGeometryChange());
            this.selectables.Add(mergingSelectable__99565);
            mergingIndex__98553 += 1L;
        }
        DartRuntimePrimitives.Assert(() => (((mergingIndex__98553 == checked((long)(mergingSelectables__98387.Count))) && (existingIndex__98579 == checked((long)(existingSelectables__98476.Count)))) && (checked((long)(this.selectables.Count)) == (existingIndex__98579 + mergingIndex__98553))));
        DartRuntimePrimitives.Assert(() => ((selectionStartIndex__98606 >= -1L) || (selectionStartIndex__98606 < checked((long)(this.selectables.Count)))));
        DartRuntimePrimitives.Assert(() => ((selectionEndIndex__98664 >= -1L) || (selectionEndIndex__98664 < checked((long)(this.selectables.Count)))));
        DartRuntimePrimitives.Assert(() => (((this.currentSelectionStartIndex == -1L)) == ((selectionStartIndex__98606 == -1L))));
        DartRuntimePrimitives.Assert(() => (((this.currentSelectionEndIndex == -1L)) == ((selectionEndIndex__98664 == -1L))));
        currentSelectionEndIndex = selectionEndIndex__98664;
        currentSelectionStartIndex = selectionStartIndex__98606;
        _additions = new HashSet<global::Doroti.Framework.Rendering.Selectable>();
    }

    internal virtual void _removeSelectable(global::Doroti.Framework.Rendering.Selectable selectable)
    {
        DartRuntimePrimitives.Assert(() => this.selectables.Contains(selectable), () => (object?)"The selectable is not in this registrar.");
        long index__100870 = ((long)((dynamic)this.selectables).IndexOf(selectable));
        this.selectables.removeAt(index__100870);
        if ((index__100870 <= this.currentSelectionEndIndex))
        {
            currentSelectionEndIndex -= 1L;
        }
        if ((index__100870 <= this.currentSelectionStartIndex))
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
        global::Doroti.Framework.Rendering.SelectionGeometry newValue__101696 = ((global::Doroti.Framework.Rendering.SelectionGeometry)(object?)getSelectionGeometry());
        if ((!object.Equals(this._selectionGeometry, newValue__101696)))
        {
            _selectionGeometry = newValue__101696;
            notifyListeners();
        }
        _updateHandleLayersAndOwners();
    }

    internal static global::Doroti.Ui.Rect _getBoundingBox(global::Doroti.Framework.Rendering.Selectable selectable)
    {
        global::Doroti.Ui.Rect result__101946 = ((global::Doroti.Ui.Rect)(object?)((global::Doroti.Framework.Rendering.Selectable)selectable).boundingBoxes.First());
        for (var index__102000 = 1L; (index__102000 < checked((long)(((global::Doroti.Framework.Rendering.Selectable)selectable).boundingBoxes.Count))); index__102000 += 1L)
        {
            result__101946 = result__101946.expandToInclude(((global::Doroti.Framework.Rendering.Selectable)selectable).boundingBoxes[(int)(index__102000)]);
        }
        return result__101946;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Comparison<global::Doroti.Framework.Rendering.Selectable> compareOrder => new Comparison<global::Doroti.Framework.Rendering.Selectable>((left, right) => checked((int)_compareScreenOrder(left, right)));
    internal static long _compareScreenOrder(global::Doroti.Framework.Rendering.Selectable a, global::Doroti.Framework.Rendering.Selectable b)
    {
        global::Doroti.Ui.Rect rectA__102472 = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(a.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)), MultiSelectableSelectionContainerDelegate._getBoundingBox(a)));
        global::Doroti.Ui.Rect rectB__102566 = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(b.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)), MultiSelectableSelectionContainerDelegate._getBoundingBox(b)));
        long result__102659 = MultiSelectableSelectionContainerDelegate._compareVertically(rectA__102472, rectB__102566);
        if ((result__102659 != 0L))
        {
            return result__102659;
        }
        return MultiSelectableSelectionContainerDelegate._compareHorizontally(rectA__102472, rectB__102566);
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
        global::Doroti.Framework.Rendering.SelectionGeometry startGeometry__105529 = this.selectables[(int)(this.currentSelectionStartIndex)].value;
        bool forwardSelection__105607 = (this.currentSelectionEndIndex >= this.currentSelectionStartIndex);
        long startIndexWalker__105690 = this.currentSelectionStartIndex;
        while (((startIndexWalker__105690 != this.currentSelectionEndIndex) && (((global::Doroti.Framework.Rendering.SelectionGeometry)startGeometry__105529).startSelectionPoint is null)))
        {
            startIndexWalker__105690 += (forwardSelection__105607 ? 1L : -1L);
            startGeometry__105529 = this.selectables[(int)(startIndexWalker__105690)].value;
        }
        global::Doroti.Framework.Rendering.SelectionPoint? startPoint__105988 = default!;
        if ((((global::Doroti.Framework.Rendering.SelectionGeometry)startGeometry__105529).startSelectionPoint is not null))
        {
            Matrix4 startTransform__106073 = ((Matrix4)(object?)getTransformFrom(this.selectables[(int)(startIndexWalker__105690)]));
            global::Doroti.Ui.Offset start__106158 = ((global::Doroti.Ui.Offset)(object?)MatrixUtils.transformPoint(startTransform__106073, ((global::Doroti.Framework.Rendering.SelectionGeometry)startGeometry__105529).startSelectionPoint!.localPosition));
            if (start__106158.isFinite)
            {
                startPoint__105988 = new global::Doroti.Framework.Rendering.SelectionPoint(localPosition: start__106158, lineHeight: ((global::Doroti.Framework.Rendering.SelectionGeometry)startGeometry__105529).startSelectionPoint!.lineHeight, handleType: ((global::Doroti.Framework.Rendering.SelectionGeometry)startGeometry__105529).startSelectionPoint!.handleType);
            }
        }
        global::Doroti.Framework.Rendering.SelectionGeometry endGeometry__106678 = this.selectables[(int)(this.currentSelectionEndIndex)].value;
        long endIndexWalker__106745 = this.currentSelectionEndIndex;
        while (((endIndexWalker__106745 != this.currentSelectionStartIndex) && (((global::Doroti.Framework.Rendering.SelectionGeometry)endGeometry__106678).endSelectionPoint is null)))
        {
            endIndexWalker__106745 += (forwardSelection__105607 ? -1L : 1L);
            endGeometry__106678 = this.selectables[(int)(endIndexWalker__106745)].value;
        }
        global::Doroti.Framework.Rendering.SelectionPoint? endPoint__107020 = default!;
        if ((((global::Doroti.Framework.Rendering.SelectionGeometry)endGeometry__106678).endSelectionPoint is not null))
        {
            Matrix4 endTransform__107099 = ((Matrix4)(object?)getTransformFrom(this.selectables[(int)(endIndexWalker__106745)]));
            global::Doroti.Ui.Offset end__107180 = ((global::Doroti.Ui.Offset)(object?)MatrixUtils.transformPoint(endTransform__107099, ((global::Doroti.Framework.Rendering.SelectionGeometry)endGeometry__106678).endSelectionPoint!.localPosition));
            if (end__107180.isFinite)
            {
                endPoint__107020 = new global::Doroti.Framework.Rendering.SelectionPoint(localPosition: end__107180, lineHeight: ((global::Doroti.Framework.Rendering.SelectionGeometry)endGeometry__106678).endSelectionPoint!.lineHeight, handleType: ((global::Doroti.Framework.Rendering.SelectionGeometry)endGeometry__106678).endSelectionPoint!.handleType);
            }
        }
        var selectionRects__107752 = new List<global::Doroti.Ui.Rect>();
        global::Doroti.Ui.Rect? drawableArea__107795 = ((global::Doroti.Ui.Rect?)(object?)(this.hasSize ? global::Doroti.Ui.Rect.fromLTWH(0, 0, this.containerSize.width, this.containerSize.height) : null));
        for (long index__107920 = this.currentSelectionStartIndex; (index__107920 <= this.currentSelectionEndIndex); index__107920++)
        {
            List<global::Doroti.Ui.Rect> currSelectableSelectionRects__108025 = this.selectables[(int)(index__107920)].value.selectionRects.Cast<global::Doroti.Ui.Rect>().ToList();
            List<global::Doroti.Ui.Rect> selectionRectsWithinDrawableArea__108120 = currSelectableSelectionRects__108025.map<Rect, Rect>(((selectionRect) => {
Matrix4 transform__108248 = ((Matrix4)(object?)getTransformFrom(this.selectables[(int)(index__107920)]));
global::Doroti.Ui.Rect localRect__108321 = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(transform__108248, selectionRect));
return (drawableArea__107795?.intersect(localRect__108321) ?? localRect__108321);
throw new InvalidOperationException("Dart closure completed without a value.");
})).where(((selectionRect) => {
return (selectionRect.isFinite && !selectionRect.isEmpty);
throw new InvalidOperationException("Dart closure completed without a value.");
})).ToList().Cast<global::Doroti.Ui.Rect>().ToList();
            selectionRects__107752.AddRange(selectionRectsWithinDrawableArea__108120.Cast<Rect>());
        }
        return new global::Doroti.Framework.Rendering.SelectionGeometry(startSelectionPoint: startPoint__105988, endSelectionPoint: endPoint__107020, selectionRects: selectionRects__107752, status: ((!object.Equals(startGeometry__105529, endGeometry__106678)) ? global::Doroti.Framework.Rendering.SelectionStatus.uncollapsed : ((global::Doroti.Framework.Rendering.SelectionGeometry)startGeometry__105529).status), hasContent: true);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long _adjustSelectionIndexBasedOnSelectionGeometry(long currentIndex, long towardIndex)
    {
        bool forward__109610 = (towardIndex > currentIndex);
        while (((currentIndex != towardIndex) && (!object.Equals(this.selectables[(int)(currentIndex)].value.status, global::Doroti.Framework.Rendering.SelectionStatus.uncollapsed))))
        {
            currentIndex += (forward__109610 ? 1L : -1L);
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
        global::Doroti.Framework.Rendering.LayerLink? effectiveStartHandle__110533 = this._startHandleLayer;
        global::Doroti.Framework.Rendering.LayerLink? effectiveEndHandle__110590 = this._endHandleLayer;
        if (((effectiveStartHandle__110533 is not null) || (effectiveEndHandle__110590 is not null)))
        {
            global::Doroti.Ui.Rect? drawableArea__110716 = ((global::Doroti.Ui.Rect?)(object?)(this.hasSize ? global::Doroti.Ui.Rect.fromLTWH(0, 0, this.containerSize.width, this.containerSize.height).inflate(_kSelectionHandleDrawableAreaPadding) : null));
            bool hideStartHandle__110966 = (((((global::Doroti.Framework.Rendering.SelectionGeometry)this.value).startSelectionPoint is null) || (drawableArea__110716 is null)) || !DartRuntimePrimitives.RequireValue(drawableArea__110716).contains(((global::Doroti.Framework.Rendering.SelectionGeometry)this.value).startSelectionPoint!.localPosition));
            bool hideEndHandle__111158 = (((((global::Doroti.Framework.Rendering.SelectionGeometry)this.value).endSelectionPoint is null) || (drawableArea__110716 is null)) || !DartRuntimePrimitives.RequireValue(drawableArea__110716).contains(((global::Doroti.Framework.Rendering.SelectionGeometry)this.value).endSelectionPoint!.localPosition));
            effectiveStartHandle__110533 = (hideStartHandle__110966 ? null : this._startHandleLayer);
            effectiveEndHandle__110590 = (hideEndHandle__111158 ? null : this._endHandleLayer);
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
            this._startHandleLayerOwner!.pushHandleLayers(effectiveStartHandle__110533, effectiveEndHandle__110590);
            return;
        }
        this._startHandleLayerOwner!.pushHandleLayers(effectiveStartHandle__110533, ((global::Doroti.Framework.Rendering.LayerLink)(object)null));
        _endHandleLayerOwner = this.selectables[(int)(this.currentSelectionEndIndex)];
        this._endHandleLayerOwner!.pushHandleLayers(((global::Doroti.Framework.Rendering.LayerLink)(object)null), effectiveEndHandle__110590);
    }

    public virtual global::Doroti.Framework.Rendering.SelectedContent? getSelectedContent()
    {
        var selections__112871 = new List<global::Doroti.Framework.Rendering.SelectedContent>();
        if (!System.Linq.Enumerable.Any(selections__112871))
        {
            return ((global::Doroti.Framework.Rendering.SelectedContent)(object)null);
        }
        var buffer__113113 = new StringBuffer();
        foreach (var selection__113153 in selections__112871)
        {
            buffer__113113.write(((global::Doroti.Framework.Rendering.SelectedContent)selection__113153).plainText);
        }
        return new global::Doroti.Framework.Rendering.SelectedContent(plainText: buffer__113113.ToString());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long contentLength => System.Linq.Enumerable.Aggregate(this.selectables, (long)0L, ((sum, selectable) => (sum + selectable.contentLength)));
    internal virtual global::Doroti.Framework.Rendering.SelectedContentRange? _calculateLocalRange(List<(long contentLength, global::Doroti.Framework.Rendering.SelectedContentRange? range)> selections)
    {
        if (((this.currentSelectionStartIndex == -1L) || (this.currentSelectionEndIndex == -1L)))
        {
            return ((global::Doroti.Framework.Rendering.SelectedContentRange)(object)null);
        }
        var startOffset__114172 = 0L;
        var endOffset__114197 = 0L;
        var foundStart__114220 = false;
        bool forwardSelection__114249 = (this.currentSelectionEndIndex >= this.currentSelectionStartIndex);
        if ((this.currentSelectionEndIndex == this.currentSelectionStartIndex))
        {
            global::Doroti.Framework.Rendering.SelectedContentRange rangeAtSelectableInSelection__114649 = this.selectables[(int)(this.currentSelectionStartIndex)].getSelection()!;
            forwardSelection__114249 = (((global::Doroti.Framework.Rendering.SelectedContentRange)rangeAtSelectableInSelection__114649).endOffset >= ((global::Doroti.Framework.Rendering.SelectedContentRange)rangeAtSelectableInSelection__114649).startOffset);
        }
        for (var index__114885 = 0L; (index__114885 < checked((long)(selections.Count))); index__114885++)
        {
            (long contentLength, global::Doroti.Framework.Rendering.SelectedContentRange? range) selection__114961 = selections[(int)(index__114885)];
            if ((selection__114961.range is null))
            {
                if (foundStart__114220)
                {
                    return new global::Doroti.Framework.Rendering.SelectedContentRange(startOffset: (forwardSelection__114249 ? startOffset__114172 : endOffset__114197), endOffset: (forwardSelection__114249 ? endOffset__114197 : startOffset__114172));
                }
                startOffset__114172 += selection__114961.contentLength;
                endOffset__114197 = startOffset__114172;
                continue;
            }
            long selectionStartNormalized__115376 = Math.Min(selection__114961.range!.startOffset, selection__114961.range!.endOffset);
            long selectionEndNormalized__115507 = Math.Max(selection__114961.range!.startOffset, selection__114961.range!.endOffset);
            if (!foundStart__114220)
            {
                startOffset__114172 += selectionStartNormalized__115376;
                endOffset__114197 = (startOffset__114172 + ((selectionEndNormalized__115507 - selectionStartNormalized__115376)).abs());
                foundStart__114220 = true;
            }
            else
            {
                endOffset__114197 += ((selectionEndNormalized__115507 - selectionStartNormalized__115376)).abs();
            }
        }
        DartRuntimePrimitives.Assert(() => foundStart__114220, () => (object?)"The start of the selection has not been found despite this selection delegate having an existing currentSelectionStartIndex and currentSelectionEndIndex.");
        return new global::Doroti.Framework.Rendering.SelectedContentRange(startOffset: (forwardSelection__114249 ? startOffset__114172 : endOffset__114197), endOffset: (forwardSelection__114249 ? endOffset__114197 : startOffset__114172));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.SelectedContentRange? getSelection()
    {
        var selections__116615 = new List<(long contentLength, global::Doroti.Framework.Rendering.SelectedContentRange? range)>();
        return ((global::Doroti.Framework.Rendering.SelectedContentRange?)(object?)_calculateLocalRange(selections__116615));
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
            long skipIndex__117406 = ((this.currentSelectionStartIndex == -1L) ? this.currentSelectionEndIndex : this.currentSelectionStartIndex);
            _clearSelectables(skipIndex: DartRuntimePrimitives.RequireValue(skipIndex__117406));
            return;
        }
        long skipStart__117609 = Math.Min(this.currentSelectionStartIndex, this.currentSelectionEndIndex);
        long skipEnd__117694 = Math.Max(this.currentSelectionStartIndex, this.currentSelectionEndIndex);
        for (var index__117776 = 0L; (index__117776 < checked((long)(this.selectables.Count))); index__117776 += 1L)
        {
            if (((index__117776 >= skipStart__117609) && (index__117776 <= skipEnd__117694)))
            {
                continue;
            }
            dispatchSelectionEventToChild(this.selectables[(int)(index__117776)], new global::Doroti.Framework.Rendering.ClearSelectionEvent());
        }
    }

    public virtual global::Doroti.Framework.Rendering.SelectionResult handleSelectAll(global::Doroti.Framework.Rendering.SelectAllSelectionEvent @event)
    {
        foreach (global::Doroti.Framework.Rendering.Selectable selectable__118159 in this.selectables)
        {
            dispatchSelectionEventToChild(selectable__118159, @event);
        }
        currentSelectionStartIndex = 0L;
        currentSelectionEndIndex = (checked((long)(this.selectables.Count)) - 1L);
        return global::Doroti.Framework.Rendering.SelectionResult.none;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _clearSelectables(long? skipIndex = null)
    {
        for (var i__118547 = 0L; (i__118547 < checked((long)(this.selectables.Count))); i__118547++)
        {
            if ((i__118547 == skipIndex))
            {
                continue;
            }
            dispatchSelectionEventToChild(this.selectables[(int)(i__118547)], new global::Doroti.Framework.Rendering.ClearSelectionEvent());
        }
    }

    internal virtual global::Doroti.Framework.Rendering.SelectionResult _handleSelectBoundary(global::Doroti.Framework.Rendering.SelectionEvent @event)
    {
        DartRuntimePrimitives.Assert(() => ((@event is global::Doroti.Framework.Rendering.SelectWordSelectionEvent) || (@event is global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent)), () => (object?)"This method should only be given selection events that select text boundaries.");
        global::Doroti.Ui.Offset effectiveGlobalPosition__119003 = ((global::Doroti.Ui.Offset)(object?)(@event switch { global::Doroti.Framework.Rendering.SelectWordSelectionEvent { globalPosition: object globalPosition__119084 } __object119052 => globalPosition__119084, global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent { globalPosition: object globalPosition__119162 } __object119125 => globalPosition__119162, _ => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Unsupported selection event: {@event}")) }));
        global::Doroti.Framework.Rendering.SelectionResult? lastSelectionResult__119296 = default!;
        double minDistanceSquared__119408 = double.PositiveInfinity;
        var nearestIndex__119454 = 0L;
        for (var index__119485 = 0L; (index__119485 < checked((long)(this.selectables.Count))); index__119485 += 1L)
        {
            var globalRectsContainPosition__119548 = false;
            Matrix4 transform__119604 = ((Matrix4)(object?)this.selectables[(int)(index__119485)].getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)));
            foreach (global::Doroti.Ui.Rect rect__119679 in this.selectables[(int)(index__119485)].boundingBoxes)
            {
                global::Doroti.Ui.Rect globalRect__119742 = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(transform__119604, rect__119679));
                if (globalRect__119742.contains(effectiveGlobalPosition__119003))
                {
                    globalRectsContainPosition__119548 = true;
                    break;
                }
                double dx__119952 = (effectiveGlobalPosition__119003.dx - Dart_uiLibrary.clampDouble(effectiveGlobalPosition__119003.dx, globalRect__119742.left, globalRect__119742.right));
                double dy__120107 = (effectiveGlobalPosition__119003.dy - Dart_uiLibrary.clampDouble(effectiveGlobalPosition__119003.dy, globalRect__119742.top, globalRect__119742.bottom));
                double distanceSquared__120262 = ((dx__119952 * dx__119952) + (dy__120107 * dy__120107));
                if ((distanceSquared__120262 < minDistanceSquared__119408))
                {
                    minDistanceSquared__119408 = distanceSquared__120262;
                    nearestIndex__119454 = index__119485;
                }
            }
            if (globalRectsContainPosition__119548)
            {
                global::Doroti.Framework.Rendering.SelectionGeometry existingGeometry__120521 = this.selectables[(int)(index__119485)].value;
                lastSelectionResult__119296 = dispatchSelectionEventToChild(this.selectables[(int)(index__119485)], @event);
                if (((index__119485 == (checked((long)(this.selectables.Count)) - 1L)) && (object.Equals(DartRuntimePrimitives.RequireValue(lastSelectionResult__119296), global::Doroti.Framework.Rendering.SelectionResult.next))))
                {
                    return global::Doroti.Framework.Rendering.SelectionResult.next;
                }
                if ((object.Equals(DartRuntimePrimitives.RequireValue(lastSelectionResult__119296), global::Doroti.Framework.Rendering.SelectionResult.next)))
                {
                    continue;
                }
                if (((index__119485 == 0L) && (object.Equals(DartRuntimePrimitives.RequireValue(lastSelectionResult__119296), global::Doroti.Framework.Rendering.SelectionResult.previous))))
                {
                    return global::Doroti.Framework.Rendering.SelectionResult.previous;
                }
                if ((!object.Equals(this.selectables[(int)(index__119485)].value, existingGeometry__120521)))
                {
                    _clearSelectables(skipIndex: index__119485);
                    currentSelectionStartIndex = currentSelectionEndIndex = index__119485;
                }
                return global::Doroti.Framework.Rendering.SelectionResult.end;
            }
            else
            {
                if ((object.Equals(lastSelectionResult__119296, global::Doroti.Framework.Rendering.SelectionResult.next)))
                {
                    currentSelectionStartIndex = currentSelectionEndIndex = (index__119485 - 1L);
                    return global::Doroti.Framework.Rendering.SelectionResult.end;
                }
            }
        }
        DartRuntimePrimitives.Assert(() => (lastSelectionResult__119296 is null));
        if (System.Linq.Enumerable.Any(this.selectables))
        {
            global::Doroti.Framework.Rendering.SelectionGeometry existingGeometry__121882 = this.selectables[(int)(nearestIndex__119454)].value;
            dispatchSelectionEventToChild(this.selectables[(int)(nearestIndex__119454)], @event);
            if ((!object.Equals(this.selectables[(int)(nearestIndex__119454)].value, existingGeometry__121882)))
            {
                _clearSelectables(skipIndex: nearestIndex__119454);
                currentSelectionStartIndex = currentSelectionEndIndex = nearestIndex__119454;
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
        foreach (global::Doroti.Framework.Rendering.Selectable selectable__123061 in this.selectables)
        {
            dispatchSelectionEventToChild(selectable__123061, @event);
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
        long targetIndex__123775 = (((global::Doroti.Framework.Rendering.GranularlyExtendSelectionEvent)@event).isEnd ? this.currentSelectionEndIndex : this.currentSelectionStartIndex);
        global::Doroti.Framework.Rendering.SelectionResult result__123878 = dispatchSelectionEventToChild(this.selectables[(int)(targetIndex__123775)], @event);
        if (((global::Doroti.Framework.Rendering.GranularlyExtendSelectionEvent)@event).forward)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(result__123878, global::Doroti.Framework.Rendering.SelectionResult.previous)));
            while (((targetIndex__123775 < (checked((long)(this.selectables.Count)) - 1L)) && (object.Equals(result__123878, global::Doroti.Framework.Rendering.SelectionResult.next))))
            {
                targetIndex__123775 += 1L;
                result__123878 = dispatchSelectionEventToChild(this.selectables[(int)(targetIndex__123775)], @event);
                DartRuntimePrimitives.Assert(() => (!object.Equals(result__123878, global::Doroti.Framework.Rendering.SelectionResult.previous)));
            }
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(result__123878, global::Doroti.Framework.Rendering.SelectionResult.next)));
            while (((targetIndex__123775 > 0L) && (object.Equals(result__123878, global::Doroti.Framework.Rendering.SelectionResult.previous))))
            {
                targetIndex__123775 -= 1L;
                result__123878 = dispatchSelectionEventToChild(this.selectables[(int)(targetIndex__123775)], @event);
                DartRuntimePrimitives.Assert(() => (!object.Equals(result__123878, global::Doroti.Framework.Rendering.SelectionResult.next)));
            }
        }
        if (((global::Doroti.Framework.Rendering.GranularlyExtendSelectionEvent)@event).isEnd)
        {
            currentSelectionEndIndex = targetIndex__123775;
        }
        else
        {
            currentSelectionStartIndex = targetIndex__123775;
        }
        return result__123878;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.SelectionResult handleDirectionallyExtendSelection(global::Doroti.Framework.Rendering.DirectionallyExtendSelectionEvent @event)
    {
        DartRuntimePrimitives.Assert(() => (((this.currentSelectionStartIndex == -1L)) == ((this.currentSelectionEndIndex == -1L))));
        if ((this.currentSelectionStartIndex == -1L))
        {
            currentSelectionStartIndex = currentSelectionEndIndex = (((global::Doroti.Framework.Rendering.DirectionallyExtendSelectionEvent)@event).direction switch { global::Doroti.Framework.Rendering.SelectionExtendDirection.previousLine => (checked((long)(this.selectables.Count)) - 1L), global::Doroti.Framework.Rendering.SelectionExtendDirection.backward => (checked((long)(this.selectables.Count)) - 1L), global::Doroti.Framework.Rendering.SelectionExtendDirection.nextLine => 0L, global::Doroti.Framework.Rendering.SelectionExtendDirection.forward => 0L, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        }
        long targetIndex__125352 = (((global::Doroti.Framework.Rendering.DirectionallyExtendSelectionEvent)@event).isEnd ? this.currentSelectionEndIndex : this.currentSelectionStartIndex);
        global::Doroti.Framework.Rendering.SelectionResult result__125455 = dispatchSelectionEventToChild(this.selectables[(int)(targetIndex__125352)], @event);
        switch (((global::Doroti.Framework.Rendering.DirectionallyExtendSelectionEvent)@event).direction)
        {
            case global::Doroti.Framework.Rendering.SelectionExtendDirection.previousLine:
                {
                    DartRuntimePrimitives.Assert(() => ((object.Equals(result__125455, global::Doroti.Framework.Rendering.SelectionResult.end)) || (object.Equals(result__125455, global::Doroti.Framework.Rendering.SelectionResult.previous))));
                    if ((object.Equals(result__125455, global::Doroti.Framework.Rendering.SelectionResult.previous)))
                    {
                        if ((targetIndex__125352 > 0L))
                        {
                            targetIndex__125352 -= 1L;
                            result__125455 = dispatchSelectionEventToChild(this.selectables[(int)(targetIndex__125352)], @event.copyWith(direction: global::Doroti.Framework.Rendering.SelectionExtendDirection.backward));
                            DartRuntimePrimitives.Assert(() => (object.Equals(result__125455, global::Doroti.Framework.Rendering.SelectionResult.end)));
                        }
                    }
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionExtendDirection.nextLine:
                {
                    DartRuntimePrimitives.Assert(() => ((object.Equals(result__125455, global::Doroti.Framework.Rendering.SelectionResult.end)) || (object.Equals(result__125455, global::Doroti.Framework.Rendering.SelectionResult.next))));
                    if ((object.Equals(result__125455, global::Doroti.Framework.Rendering.SelectionResult.next)))
                    {
                        if ((targetIndex__125352 < (checked((long)(this.selectables.Count)) - 1L)))
                        {
                            targetIndex__125352 += 1L;
                            result__125455 = dispatchSelectionEventToChild(this.selectables[(int)(targetIndex__125352)], @event.copyWith(direction: global::Doroti.Framework.Rendering.SelectionExtendDirection.forward));
                            DartRuntimePrimitives.Assert(() => (object.Equals(result__125455, global::Doroti.Framework.Rendering.SelectionResult.end)));
                        }
                    }
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionExtendDirection.forward:
            case global::Doroti.Framework.Rendering.SelectionExtendDirection.backward:
                {
                    DartRuntimePrimitives.Assert(() => (object.Equals(result__125455, global::Doroti.Framework.Rendering.SelectionResult.end)));
                    break;
                }
        }
        if (((global::Doroti.Framework.Rendering.DirectionallyExtendSelectionEvent)@event).isEnd)
        {
            currentSelectionEndIndex = targetIndex__125352;
        }
        else
        {
            currentSelectionStartIndex = targetIndex__125352;
        }
        return result__125455;
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
        var selectionWillBeInProgress__127441 = (@event is not global::Doroti.Framework.Rendering.ClearSelectionEvent);
        if ((!this._selectionInProgress && selectionWillBeInProgress__127441))
        {
            this.selectables.sort(this.compareOrder);
        }
        _selectionInProgress = selectionWillBeInProgress__127441;
        _isHandlingSelectionEvent = true;
        global::Doroti.Framework.Rendering.SelectionResult result__127782 = default!;
        switch (((global::Doroti.Framework.Rendering.SelectionEvent)@event).type)
        {
            case global::Doroti.Framework.Rendering.SelectionEventType.startEdgeUpdate:
            case global::Doroti.Framework.Rendering.SelectionEventType.endEdgeUpdate:
                {
                    _extendSelectionInProgress = false;
                    result__127782 = handleSelectionEdgeUpdate(((global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent?)(object?)@event)!);
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionEventType.clear:
                {
                    _extendSelectionInProgress = false;
                    result__127782 = handleClearSelection(((global::Doroti.Framework.Rendering.ClearSelectionEvent?)(object?)@event)!);
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionEventType.selectAll:
                {
                    _extendSelectionInProgress = false;
                    result__127782 = handleSelectAll(((global::Doroti.Framework.Rendering.SelectAllSelectionEvent?)(object?)@event)!);
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionEventType.selectWord:
                {
                    _extendSelectionInProgress = false;
                    result__127782 = handleSelectWord(((global::Doroti.Framework.Rendering.SelectWordSelectionEvent?)(object?)@event)!);
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionEventType.selectParagraph:
                {
                    _extendSelectionInProgress = false;
                    result__127782 = handleSelectParagraph(((global::Doroti.Framework.Rendering.SelectParagraphSelectionEvent?)(object?)@event)!);
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionEventType.granularlyExtendSelection:
                {
                    _extendSelectionInProgress = true;
                    result__127782 = handleGranularlyExtendSelection(((global::Doroti.Framework.Rendering.GranularlyExtendSelectionEvent?)(object?)@event)!);
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionEventType.directionallyExtendSelection:
                {
                    _extendSelectionInProgress = true;
                    result__127782 = handleDirectionallyExtendSelection(((global::Doroti.Framework.Rendering.DirectionallyExtendSelectionEvent?)(object?)@event)!);
                    break;
                }
        }
        _isHandlingSelectionEvent = false;
        _updateSelectionGeometry();
        return result__127782;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void dispose()
    {
        foreach (global::Doroti.Framework.Rendering.Selectable selectable__129210 in this.selectables)
        {
            selectable__129210.removeListener(() => this._handleSelectableGeometryChange());
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
        var newIndex__131012 = -1L;
        var hasFoundEdgeIndex__131035 = false;
        global::Doroti.Framework.Rendering.SelectionResult? result__131083 = default!;
        bool? forward__131101 = default!;
        long oppositeEdgeIndex__131278 = (isEnd ? this.currentSelectionStartIndex : this.currentSelectionEndIndex);
        long index__131369 = Math.Max(oppositeEdgeIndex__131278, 0L);
        while (((index__131369 >= 0L) && (index__131369 < checked((long)(this.selectables.Count)))))
        {
            global::Doroti.Framework.Rendering.Selectable child__131483 = this.selectables[(int)(index__131369)];
            global::Doroti.Framework.Rendering.SelectionResult childResult__131539 = dispatchSelectionEventToChild(child__131483, @event);
            switch (childResult__131539)
            {
                case global::Doroti.Framework.Rendering.SelectionResult.next:
                    {
                        if ((forward__131101 == false))
                        {
                            hasFoundEdgeIndex__131035 = true;
                            result__131083 = global::Doroti.Framework.Rendering.SelectionResult.end;
                        }
                        else
                        {
                            forward__131101 = true;
                            newIndex__131012 = index__131369;
                        }
                        break;
                    }
                case global::Doroti.Framework.Rendering.SelectionResult.none:
                    {
                        newIndex__131012 = index__131369;
                        break;
                    }
                case global::Doroti.Framework.Rendering.SelectionResult.end:
                    {
                        newIndex__131012 = index__131369;
                        result__131083 = global::Doroti.Framework.Rendering.SelectionResult.end;
                        hasFoundEdgeIndex__131035 = true;
                        break;
                    }
                case global::Doroti.Framework.Rendering.SelectionResult.previous:
                    {
                        if ((index__131369 == 0L))
                        {
                            hasFoundEdgeIndex__131035 = true;
                            newIndex__131012 = 0L;
                            result__131083 = global::Doroti.Framework.Rendering.SelectionResult.previous;
                            break;
                        }
                        if ((forward__131101 ?? false))
                        {
                            hasFoundEdgeIndex__131035 = true;
                            result__131083 = global::Doroti.Framework.Rendering.SelectionResult.end;
                        }
                        else
                        {
                            forward__131101 = false;
                            newIndex__131012 = index__131369;
                        }
                        break;
                    }
                case global::Doroti.Framework.Rendering.SelectionResult.pending:
                    {
                        newIndex__131012 = index__131369;
                        result__131083 = global::Doroti.Framework.Rendering.SelectionResult.pending;
                        hasFoundEdgeIndex__131035 = true;
                        break;
                    }
            }
            if (hasFoundEdgeIndex__131035)
            {
                break;
            }
            index__131369 += (((forward__131101 ?? true)) ? 1L : -1L);
        }
        if ((newIndex__131012 == -1L))
        {
            DartRuntimePrimitives.Assert(() => !System.Linq.Enumerable.Any(this.selectables));
            return global::Doroti.Framework.Rendering.SelectionResult.none;
        }
        if (isEnd)
        {
            currentSelectionEndIndex = newIndex__131012;
        }
        else
        {
            currentSelectionStartIndex = newIndex__131012;
        }
        _flushInactiveSelections();
        return (result__131083 ?? global::Doroti.Framework.Rendering.SelectionResult.next);
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
        global::Doroti.Framework.Rendering.SelectionResult? finalResult__134047 = default!;
        var isCurrentEdgeWithinViewport__134647 = (isEnd ? (((global::Doroti.Framework.Rendering.SelectionGeometry)this._selectionGeometry).endSelectionPoint is not null) : (((global::Doroti.Framework.Rendering.SelectionGeometry)this._selectionGeometry).startSelectionPoint is not null));
        var isOppositeEdgeWithinViewport__134806 = (isEnd ? (((global::Doroti.Framework.Rendering.SelectionGeometry)this._selectionGeometry).startSelectionPoint is not null) : (((global::Doroti.Framework.Rendering.SelectionGeometry)this._selectionGeometry).endSelectionPoint is not null));
        long newIndex__134964 = ((isEnd, isCurrentEdgeWithinViewport__134647, isOppositeEdgeWithinViewport__134806) switch { (true, true, true) => this.currentSelectionEndIndex, (true, true, false) => this.currentSelectionEndIndex, (true, false, true) => this.currentSelectionStartIndex, (true, false, false) => 0L, (false, true, true) => this.currentSelectionStartIndex, (false, true, false) => this.currentSelectionStartIndex, (false, false, true) => this.currentSelectionEndIndex, (false, false, false) => 0L });
        bool? forward__135474 = default!;
        global::Doroti.Framework.Rendering.SelectionResult currentSelectableResult__135508 = default!;
        while ((((newIndex__134964 < checked((long)(this.selectables.Count))) && (newIndex__134964 >= 0L)) && (finalResult__134047 is null)))
        {
            currentSelectableResult__135508 = dispatchSelectionEventToChild(this.selectables[(int)(newIndex__134964)], @event);
            switch (currentSelectableResult__135508)
            {
                case global::Doroti.Framework.Rendering.SelectionResult.end:
                case global::Doroti.Framework.Rendering.SelectionResult.pending:
                case global::Doroti.Framework.Rendering.SelectionResult.none:
                    {
                        finalResult__134047 = currentSelectableResult__135508;
                        break;
                    }
                case global::Doroti.Framework.Rendering.SelectionResult.next:
                    {
                        if ((forward__135474 == false))
                        {
                            newIndex__134964 += 1L;
                            finalResult__134047 = global::Doroti.Framework.Rendering.SelectionResult.end;
                        }
                        else
                        {
                            if ((newIndex__134964 == (checked((long)(this.selectables.Count)) - 1L)))
                            {
                                finalResult__134047 = currentSelectableResult__135508;
                            }
                            else
                            {
                                forward__135474 = true;
                                newIndex__134964 += 1L;
                            }
                        }
                        break;
                    }
                case global::Doroti.Framework.Rendering.SelectionResult.previous:
                    {
                        if ((forward__135474 ?? false))
                        {
                            newIndex__134964 -= 1L;
                            finalResult__134047 = global::Doroti.Framework.Rendering.SelectionResult.end;
                        }
                        else
                        {
                            if ((newIndex__134964 == 0L))
                            {
                                finalResult__134047 = currentSelectableResult__135508;
                            }
                            else
                            {
                                forward__135474 = false;
                                newIndex__134964 -= 1L;
                            }
                        }
                        break;
                    }
            }
        }
        if (isEnd)
        {
            currentSelectionEndIndex = newIndex__134964;
        }
        else
        {
            currentSelectionStartIndex = newIndex__134964;
        }
        _flushInactiveSelections();
        return DartRuntimePrimitives.RequireValue(finalResult__134047);
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

