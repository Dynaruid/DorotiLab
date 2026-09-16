// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/text_selection.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public delegate Widget ToolbarBuilder(BuildContext context, Widget child);

public class ToolbarItemsParentData : global::Doroti.Framework.Rendering.ContainerBoxParentData<global::Doroti.Framework.Rendering.RenderBox>
{
    public virtual bool shouldPaint { get; set; } = false;

    public override string ToString() => $"{base.ToString()}; shouldPaint={shouldPaint}";
}

public abstract class TextSelectionControls
{
    public abstract Widget buildHandle(BuildContext context, global::Doroti.Framework.Rendering.TextSelectionHandleType type, double textLineHeight, global::System.Action? onTap = null);
    public abstract global::Doroti.Ui.Offset getHandleAnchor(global::Doroti.Framework.Rendering.TextSelectionHandleType type, double textLineHeight);
    public abstract Widget buildToolbar(BuildContext context, Rect globalEditableRegion, double textLineHeight, Offset selectionMidpoint, List<global::Doroti.Framework.Rendering.TextSelectionPoint> endpoints, global::Doroti.Framework.Services.TextSelectionDelegate @delegate, global::Doroti.Framework.Foundation.ValueListenable<ClipboardStatus>? clipboardStatus, Offset? lastSecondaryTapDownPosition);
    public abstract global::Doroti.Ui.Size getHandleSize(double textLineHeight);
    public virtual bool canCut(global::Doroti.Framework.Services.TextSelectionDelegate @delegate)
    {
        return @delegate.cutEnabled && !@delegate.textEditingValue.selection.isCollapsed;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool canCopy(global::Doroti.Framework.Services.TextSelectionDelegate @delegate)
    {
        return @delegate.copyEnabled && !@delegate.textEditingValue.selection.isCollapsed;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool canPaste(global::Doroti.Framework.Services.TextSelectionDelegate @delegate)
    {
        return @delegate.pasteEnabled;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool canSelectAll(global::Doroti.Framework.Services.TextSelectionDelegate @delegate)
    {
        return @delegate.selectAllEnabled && (@delegate.textEditingValue.text.Length != 0) && @delegate.textEditingValue.selection.isCollapsed;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleCut(global::Doroti.Framework.Services.TextSelectionDelegate @delegate)
    {
        @delegate.cutSelection(SelectionChangedCause.toolbar);
    }

    public virtual void handleCopy(global::Doroti.Framework.Services.TextSelectionDelegate @delegate)
    {
        @delegate.copySelection(SelectionChangedCause.toolbar);
    }

    public async virtual Future handlePaste(global::Doroti.Framework.Services.TextSelectionDelegate @delegate)
    {
        await @delegate.pasteText(SelectionChangedCause.toolbar);
    }

    public virtual void handleSelectAll(global::Doroti.Framework.Services.TextSelectionDelegate @delegate)
    {
        @delegate.selectAll(SelectionChangedCause.toolbar);
    }

}

public class EmptyTextSelectionControls : TextSelectionControls
{
    public override Size getHandleSize(double textLineHeight) => Size.zero;
    public override Widget buildToolbar(BuildContext context, Rect globalEditableRegion, double textLineHeight, Offset selectionMidpoint, List<global::Doroti.Framework.Rendering.TextSelectionPoint> endpoints, global::Doroti.Framework.Services.TextSelectionDelegate @delegate, global::Doroti.Framework.Foundation.ValueListenable<ClipboardStatus>? clipboardStatus, Offset? lastSecondaryTapDownPosition) => DartRuntimePrimitives.ConvertValue<Widget>(SizedBox.CreateShrink());
    public override Widget buildHandle(BuildContext context, global::Doroti.Framework.Rendering.TextSelectionHandleType type, double textLineHeight, global::System.Action? onTap = null)
    {
        return SizedBox.CreateShrink();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Offset getHandleAnchor(global::Doroti.Framework.Rendering.TextSelectionHandleType type, double textLineHeight)
    {
        return Offset.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Text_selectionLibrary
{
    public static TextSelectionControls emptyTextSelectionControls = new EmptyTextSelectionControls();
}

public class TextSelectionOverlay
{
    public virtual BuildContext context { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.RenderEditable renderObject { get; private set; } = default!;
    public virtual TextSelectionControls? selectionControls { get; private set; }
    public virtual global::Doroti.Framework.Services.TextSelectionDelegate selectionDelegate { get; private set; } = default!;
    internal virtual SelectionOverlay _selectionOverlay { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, Widget>? contextMenuBuilder { get; private set; }
    internal virtual global::Doroti.Framework.Services.TextEditingValue _value { get; set; } = default!;
    internal virtual global::Doroti.Framework.Foundation.ValueNotifier<bool> _effectiveStartHandleVisibility { get; private set; } = new global::Doroti.Framework.Foundation.ValueNotifier<bool>(false);
    internal virtual global::Doroti.Framework.Foundation.ValueNotifier<bool> _effectiveEndHandleVisibility { get; private set; } = new global::Doroti.Framework.Foundation.ValueNotifier<bool>(false);
    internal virtual global::Doroti.Framework.Foundation.ValueNotifier<bool> _effectiveToolbarVisibility { get; private set; } = new global::Doroti.Framework.Foundation.ValueNotifier<bool>(false);
    internal virtual bool _handlesVisible { get; set; } = false;
    internal virtual double _endHandleDragPosition { get; set; } = default!;
    internal virtual double _endHandleDragTarget { get; set; } = default!;
    internal virtual global::Doroti.Framework.Services.TextSelection? _dragStartSelection { get; set; } = default;
    internal virtual double _startHandleDragPosition { get; set; } = default!;
    internal virtual double _startHandleDragTarget { get; set; } = default!;

    public TextSelectionOverlay(global::Doroti.Framework.Services.TextEditingValue value, BuildContext context, Widget? debugRequiredFor = null, global::Doroti.Framework.Rendering.LayerLink toolbarLayerLink = default!, global::Doroti.Framework.Rendering.LayerLink startHandleLayerLink = default!, global::Doroti.Framework.Rendering.LayerLink endHandleLayerLink = default!, global::Doroti.Framework.Rendering.RenderEditable renderObject = default!, TextSelectionControls? selectionControls = null, bool handlesVisible = false, global::Doroti.Framework.Services.TextSelectionDelegate selectionDelegate = default!, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = DragStartBehavior.start, global::System.Action? onSelectionHandleTapped = null, ClipboardStatusNotifier? clipboardStatus = null, global::System.Func<BuildContext, Widget>? contextMenuBuilder = null, TextMagnifierConfiguration magnifierConfiguration = default!)
    {
        this.context = context;
        this.renderObject = renderObject;
        this.selectionControls = selectionControls;
        this.selectionDelegate = selectionDelegate;
        this.contextMenuBuilder = contextMenuBuilder;
        _handlesVisible = handlesVisible;
        _value = value;
        this.renderObject.selectionStartInViewport.addListener(_updateTextSelectionOverlayVisibilities);
        this.renderObject.selectionEndInViewport.addListener(_updateTextSelectionOverlayVisibilities);
        _updateTextSelectionOverlayVisibilities();
        _selectionOverlay = new SelectionOverlay(
            magnifierConfiguration: magnifierConfiguration, context: context, debugRequiredFor: debugRequiredFor,
            startHandleType: TextSelectionHandleType.collapsed,
            startHandlesVisible: _effectiveStartHandleVisibility, lineHeightAtStart: 0.0,
            onStartHandleDragStart: _handleSelectionStartHandleDragStart, onStartHandleDragUpdate: _handleSelectionStartHandleDragUpdate,
            onStartHandleDragEnd: _handleAnyDragEnd, endHandleType: TextSelectionHandleType.collapsed,
            endHandlesVisible: _effectiveEndHandleVisibility, lineHeightAtEnd: 0.0,
            onEndHandleDragStart: _handleSelectionEndHandleDragStart, onEndHandleDragUpdate: _handleSelectionEndHandleDragUpdate,
            onEndHandleDragEnd: _handleAnyDragEnd, toolbarVisible: _effectiveToolbarVisibility,
            selectionEndpoints: new List<global::Doroti.Framework.Rendering.TextSelectionPoint>(), selectionControls: selectionControls,
            selectionDelegate: selectionDelegate, clipboardStatus: clipboardStatus, startHandleLayerLink: startHandleLayerLink,
            endHandleLayerLink: endHandleLayerLink, toolbarLayerLink: toolbarLayerLink, onSelectionHandleTapped: onSelectionHandleTapped,
            dragStartBehavior: dragStartBehavior, toolbarLocation: renderObject.lastSecondaryTapDownPosition);
    }

    public virtual global::Doroti.Framework.Services.TextEditingValue value => _value;
    internal virtual global::Doroti.Framework.Services.TextSelection _selection => _value.selection;
    internal virtual void _updateTextSelectionOverlayVisibilities()
    {
        _effectiveStartHandleVisibility.value = _handlesVisible && renderObject.selectionStartInViewport.value;
        _effectiveEndHandleVisibility.value = _handlesVisible && renderObject.selectionEndInViewport.value;
        _effectiveToolbarVisibility.value = renderObject.selectionStartInViewport.value || renderObject.selectionEndInViewport.value;
    }

    public virtual bool handlesVisible
    {
        get => _handlesVisible;
        set
        {
            var visible = value;
            if (_handlesVisible == visible)
            {
                return;
            }
            _handlesVisible = visible;
            _updateTextSelectionOverlayVisibilities();
        }
    }
    public virtual void showHandles()
    {
        _updateSelectionOverlay();
        _selectionOverlay.showHandles();
    }

    public virtual void hideHandles() => _selectionOverlay.hideHandles();
    public virtual void showToolbar()
    {
        DartRuntimePrimitives.Assert(() => !Equals(Scheduler.SchedulerBinding.instance.schedulerPhase, Scheduler.SchedulerPhase.persistentCallbacks), () => (object?)"showToolbar must not be called during the build or layout phase.");
        _updateSelectionOverlay();
        if ((selectionControls is not null) && (selectionControls is not TextSelectionHandleControls))
        {
            _selectionOverlay.showToolbar();
            return;
        }
        if (contextMenuBuilder is null)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => context.mounted);
        _selectionOverlay.showToolbar(context: context, contextMenuBuilder: (global::System.Func<BuildContext, Widget>?)contextMenuBuilder);
        return;
    }

    public virtual void showSpellCheckSuggestionsToolbar(global::System.Func<BuildContext, Widget> spellCheckSuggestionsToolbarBuilder)
    {
        _updateSelectionOverlay();
        DartRuntimePrimitives.Assert(() => context.mounted);
        _selectionOverlay.showSpellCheckSuggestionsToolbar(context: context, builder: spellCheckSuggestionsToolbarBuilder);
        hideHandles();
    }

    public virtual void showMagnifier(Offset positionToShow)
    {
        global::Doroti.Ui.TextPosition position = DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.TextPosition>(renderObject.getPositionForPoint(positionToShow));
        _updateSelectionOverlay();
        _selectionOverlay.showMagnifier(_buildMagnifier(currentTextPosition: position, globalGesturePosition: positionToShow, renderEditable: renderObject));
    }

    public virtual void updateMagnifier(Offset positionToShow)
    {
        global::Doroti.Ui.TextPosition position = DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.TextPosition>(renderObject.getPositionForPoint(positionToShow));
        _updateSelectionOverlay();
        _selectionOverlay.updateMagnifier(_buildMagnifier(currentTextPosition: position, globalGesturePosition: positionToShow, renderEditable: renderObject));
    }

    public virtual void hideMagnifier()
    {
        _selectionOverlay.hideMagnifier();
    }

    public virtual void update(global::Doroti.Framework.Services.TextEditingValue newValue)
    {
        if (Equals(_value, newValue))
        {
            return;
        }
        _value = newValue;
        _updateSelectionOverlay();
        _selectionOverlay.markNeedsBuild();
    }

    internal virtual void _updateSelectionOverlay()
    {
        List<global::Doroti.Framework.Rendering.TextSelectionPoint> endpoints = DartRuntimePrimitives.ConvertValue<List<global::Doroti.Framework.Rendering.TextSelectionPoint>>(renderObject.getEndpointsForSelection(_selection));
        DartRuntimePrimitives.Assert(() => Enumerable.Any(endpoints));
        global::Doroti.Framework.Rendering.TextSelectionHandleType startHandleTypeLocal = default!;
        global::Doroti.Framework.Rendering.TextSelectionHandleType endHandleTypeLocal = default!;
        if (_selection.isCollapsed)
        {
            startHandleTypeLocal = TextSelectionHandleType.collapsed;
            endHandleTypeLocal = TextSelectionHandleType.collapsed;
        }
        else
        {
            global::Doroti.Ui.TextDirection textDirectionLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.TextDirection>(renderObject.textDirection);
            var preferRenderObjectDirectionForSelectionHandles = Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS);
            global::Doroti.Ui.TextDirection startHandleDirection = default!;
            global::Doroti.Ui.TextDirection endHandleDirection = default!;
            if (preferRenderObjectDirectionForSelectionHandles || (checked(endpoints.Count) < 2L))
            {
                startHandleDirection = textDirectionLocal;
                endHandleDirection = textDirectionLocal;
            }
            else
            {
                startHandleDirection = endpoints.First().direction ?? textDirectionLocal;
                endHandleDirection = endpoints.Last().direction ?? textDirectionLocal;
            }
            startHandleTypeLocal = startHandleDirection switch { TextDirection.ltr => TextSelectionHandleType.left, TextDirection.rtl => TextSelectionHandleType.right, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            endHandleTypeLocal = endHandleDirection switch { TextDirection.ltr => TextSelectionHandleType.right, TextDirection.rtl => TextSelectionHandleType.left, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        }
        DartRuntimePrimitives.Ignore(((Func<SelectionOverlay>)(() =>
{
    var __cascade = _selectionOverlay;
    __cascade.startHandleType = startHandleTypeLocal;
    __cascade.lineHeightAtStart = _getStartGlyphHeight();
    __cascade.endHandleType = endHandleTypeLocal;
    __cascade.lineHeightAtEnd = _getEndGlyphHeight();
    __cascade.selectionEndpoints = endpoints;
    __cascade.toolbarLocation = renderObject.lastSecondaryTapDownPosition;
    return __cascade;
}))());
    }

    public virtual void updateForScroll()
    {
        _updateSelectionOverlay();
        _selectionOverlay.markNeedsBuild();
    }

    public virtual void markNeedsBuild()
    {
        _selectionOverlay.markNeedsBuild();
    }

    public virtual bool handlesAreVisible => DartRuntimePrimitives.ConvertValue<bool>((_selectionOverlay._handles is not null) && handlesVisible);
    public virtual bool toolbarIsVisible => _selectionOverlay.toolbarIsVisible;
    public virtual bool magnifierIsVisible => _selectionOverlay.magnifierIsVisible;
    public virtual bool magnifierExists => _selectionOverlay.magnifierExists;
    public virtual bool spellCheckToolbarIsVisible => _selectionOverlay._spellCheckToolbarController.isShown;
    public virtual void hide() => _selectionOverlay.hide();
    public virtual void hideToolbar() => _selectionOverlay.hideToolbar();
    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        _selectionOverlay.dispose();
        renderObject.selectionStartInViewport.removeListener(_updateTextSelectionOverlayVisibilities);
        renderObject.selectionEndInViewport.removeListener(_updateTextSelectionOverlayVisibilities);
        _effectiveToolbarVisibility.dispose();
        _effectiveStartHandleVisibility.dispose();
        _effectiveEndHandleVisibility.dispose();
        hideToolbar();
    }

    internal virtual double _getStartGlyphHeight()
    {
        string currText = selectionDelegate.textEditingValue.text;
        long firstSelectedGraphemeExtent = default!;
        global::Doroti.Ui.Rect? startHandleRect = default!;
        if ((renderObject.plainText == currText) && _selection.isValid && !_selection.isCollapsed)
        {
            string selectedGraphemes = _selection.textInside(currText);
            firstSelectedGraphemeExtent = selectedGraphemes.characters().first.Length;
            startHandleRect = renderObject.getRectForComposingRange(new global::Doroti.Ui.TextRange(start: _selection.start, end: _selection.start + firstSelectedGraphemeExtent));
        }
        return startHandleRect?.height ?? (double)renderObject.preferredLineHeight;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getEndGlyphHeight()
    {
        string currText = selectionDelegate.textEditingValue.text;
        long lastSelectedGraphemeExtent = default!;
        global::Doroti.Ui.Rect? endHandleRect = default!;
        if ((renderObject.plainText == currText) && _selection.isValid && !_selection.isCollapsed)
        {
            string selectedGraphemes = _selection.textInside(currText);
            lastSelectedGraphemeExtent = selectedGraphemes.characters().last.Length;
            endHandleRect = renderObject.getRectForComposingRange(new global::Doroti.Ui.TextRange(start: _selection.end - lastSelectedGraphemeExtent, end: _selection.end));
        }
        return endHandleRect?.height ?? (double)renderObject.preferredLineHeight;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual MagnifierInfo _buildMagnifier(global::Doroti.Framework.Rendering.RenderEditable renderEditable, Offset globalGesturePosition, TextPosition currentTextPosition)
    {
        global::Doroti.Framework.Services.TextSelection lineAtOffset = renderEditable.getLineAtOffset(currentTextPosition);
        var positionAtEndOfLine = new global::Doroti.Ui.TextPosition(offset: lineAtOffset.extentOffset, affinity: TextAffinity.upstream);
        var positionAtBeginningOfLine = new global::Doroti.Ui.TextPosition(offset: lineAtOffset.baseOffset);
        var localLineBoundaries = Rect.fromPoints(renderEditable.getLocalRectForCaret(positionAtBeginningOfLine).topCenter, renderEditable.getLocalRectForCaret(positionAtEndOfLine).bottomCenter);
        var overlay = ((global::Doroti.Framework.Rendering.RenderBox?)Overlay.of(context, rootOverlay: true).context.findRenderObject())!;
        Matrix4 transformToOverlay = renderEditable.getTransformTo(overlay);
        global::Doroti.Ui.Rect overlayLineBoundaries = MatrixUtils.transformRect(transformToOverlay, localLineBoundaries);
        global::Doroti.Ui.Rect localCaretRect = renderEditable.getLocalRectForCaret(currentTextPosition);
        global::Doroti.Ui.Rect overlayCaretRect = MatrixUtils.transformRect(transformToOverlay, localCaretRect);
        global::Doroti.Ui.Offset overlayGesturePosition = (overlay?.globalToLocal(globalGesturePosition)) ?? globalGesturePosition;
        return new MagnifierInfo(fieldBounds: MatrixUtils.transformRect(transformToOverlay, renderEditable.paintBounds), globalGesturePosition: overlayGesturePosition, caretRect: overlayCaretRect, currentLineBoundaries: overlayLineBoundaries);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleSelectionEndHandleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        if (!renderObject.attached)
        {
            return;
        }
        _endHandleDragPosition = details.globalPosition.dy;
        double centerOfLineLocal = DartRuntimePrimitives.ConvertValue<double>(_selectionOverlay.selectionEndpoints.Last().point.dy - (renderObject.preferredLineHeight / 2L));
        double centerOfLineGlobal = DartRuntimePrimitives.ConvertValue<double>(renderObject.localToGlobal(new global::Doroti.Ui.Offset(0.0, centerOfLineLocal)).dy);
        _endHandleDragTarget = centerOfLineGlobal - details.globalPosition.dy;
        global::Doroti.Ui.TextPosition position = DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.TextPosition>(renderObject.getPositionForPoint(new global::Doroti.Ui.Offset(details.globalPosition.dx, centerOfLineGlobal)));
        if (Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS) || Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.macOS))
        {
            _dragStartSelection ??= _selection;
        }
        _selectionOverlay.showMagnifier(_buildMagnifier(currentTextPosition: position, globalGesturePosition: details.globalPosition, renderEditable: renderObject));
    }

    internal virtual double? _getHandleDy(double dragDy, double handleDy)
    {
        double preferredLineHeightLocal = DartRuntimePrimitives.ConvertValue<double>(renderObject.preferredLineHeight);
        DartRuntimePrimitives.Assert(() => double.IsFinite(preferredLineHeightLocal), () => (object?)"Preferred line height is expected to always be finite.");
        if ((preferredLineHeightLocal <= 0.0) || !double.IsFinite(dragDy) || !double.IsFinite(handleDy))
        {
            return null;
        }
        double distanceDragged = dragDy - handleDy;
        var dragDirection = (distanceDragged < 0.0) ? -1L : 1L;
        long linesDragged = dragDirection * (distanceDragged.abs() / preferredLineHeightLocal).floor();
        return handleDy + (linesDragged * preferredLineHeightLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleSelectionEndHandleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        if (!renderObject.attached)
        {
            return;
        }
        global::Doroti.Ui.Offset localPosition = DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(renderObject.globalToLocal(details.globalPosition));
        double? nextEndHandleDragPositionLocal = DartRuntimePrimitives.ConvertValue<double>(_getHandleDy(localPosition.dy, renderObject.globalToLocal(new global::Doroti.Ui.Offset(0.0, _endHandleDragPosition)).dy));
        if (nextEndHandleDragPositionLocal is null)
        {
            return;
        }
        _endHandleDragPosition = renderObject.localToGlobal(new global::Doroti.Ui.Offset(0.0, DartRuntimePrimitives.RequireValue(nextEndHandleDragPositionLocal))).dy;
        var handleTargetGlobal = new global::Doroti.Ui.Offset(details.globalPosition.dx, _endHandleDragPosition + _endHandleDragTarget);
        global::Doroti.Ui.TextPosition position = DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.TextPosition>(renderObject.getPositionForPoint(handleTargetGlobal));
        global::Doroti.Framework.Services.TextSelection newSelection = default!;
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
                {
                    DartRuntimePrimitives.Assert(() => _dragStartSelection is not null);
                    if (_dragStartSelection!.isCollapsed)
                    {
                        _selectionOverlay.updateMagnifier(_buildMagnifier(currentTextPosition: position, globalGesturePosition: details.globalPosition, renderEditable: renderObject));
                        var currentSelection = TextSelection.CreateFromPosition(position);
                        _handleSelectionHandleChanged(currentSelection);
                        return;
                    }
                    bool dragStartSelectionNormalized = _dragStartSelection!.extentOffset >= _dragStartSelection!.baseOffset;
                    newSelection = new global::Doroti.Framework.Services.TextSelection(baseOffset: dragStartSelectionNormalized ? _dragStartSelection!.baseOffset : _dragStartSelection!.extentOffset, extentOffset: position.offset);
                    break;
                }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
                {
                    if (_selection.isCollapsed)
                    {
                        _selectionOverlay.updateMagnifier(_buildMagnifier(currentTextPosition: position, globalGesturePosition: details.globalPosition, renderEditable: renderObject));
                        var currentSelectionLocal = TextSelection.CreateFromPosition(position);
                        _handleSelectionHandleChanged(currentSelectionLocal);
                        return;
                    }
                    newSelection = new global::Doroti.Framework.Services.TextSelection(baseOffset: _selection.baseOffset, extentOffset: position.offset);
                    if (newSelection.baseOffset >= newSelection.extentOffset)
                    {
                        return;
                    }
                    break;
                }
        }
        _handleSelectionHandleChanged(newSelection);
        _selectionOverlay.updateMagnifier(_buildMagnifier(currentTextPosition: newSelection.extent, globalGesturePosition: details.globalPosition, renderEditable: renderObject));
    }

    internal virtual void _handleSelectionStartHandleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        if (!renderObject.attached)
        {
            return;
        }
        _startHandleDragPosition = details.globalPosition.dy;
        double centerOfLineLocal = DartRuntimePrimitives.ConvertValue<double>(_selectionOverlay.selectionEndpoints.First().point.dy - (renderObject.preferredLineHeight / 2L));
        double centerOfLineGlobal = DartRuntimePrimitives.ConvertValue<double>(renderObject.localToGlobal(new global::Doroti.Ui.Offset(0.0, centerOfLineLocal)).dy);
        _startHandleDragTarget = centerOfLineGlobal - details.globalPosition.dy;
        global::Doroti.Ui.TextPosition position = DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.TextPosition>(renderObject.getPositionForPoint(new global::Doroti.Ui.Offset(details.globalPosition.dx, centerOfLineGlobal)));
        if (Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS) || Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.macOS))
        {
            _dragStartSelection ??= _selection;
        }
        _selectionOverlay.showMagnifier(_buildMagnifier(currentTextPosition: position, globalGesturePosition: details.globalPosition, renderEditable: renderObject));
    }

    internal virtual void _handleSelectionStartHandleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        if (!renderObject.attached)
        {
            return;
        }
        global::Doroti.Ui.Offset localPosition = DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(renderObject.globalToLocal(details.globalPosition));
        double? nextStartHandleDragPositionLocal = DartRuntimePrimitives.ConvertValue<double>(_getHandleDy(localPosition.dy, renderObject.globalToLocal(new global::Doroti.Ui.Offset(0.0, _startHandleDragPosition)).dy));
        if (nextStartHandleDragPositionLocal is null)
        {
            return;
        }
        _startHandleDragPosition = renderObject.localToGlobal(new global::Doroti.Ui.Offset(0.0, DartRuntimePrimitives.RequireValue(nextStartHandleDragPositionLocal))).dy;
        var handleTargetGlobal = new global::Doroti.Ui.Offset(details.globalPosition.dx, _startHandleDragPosition + _startHandleDragTarget);
        global::Doroti.Ui.TextPosition position = DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.TextPosition>(renderObject.getPositionForPoint(handleTargetGlobal));
        global::Doroti.Framework.Services.TextSelection newSelection = default!;
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
                {
                    DartRuntimePrimitives.Assert(() => _dragStartSelection is not null);
                    if (_dragStartSelection!.isCollapsed)
                    {
                        _selectionOverlay.updateMagnifier(_buildMagnifier(currentTextPosition: position, globalGesturePosition: details.globalPosition, renderEditable: renderObject));
                        var currentSelection = TextSelection.CreateFromPosition(position);
                        _handleSelectionHandleChanged(currentSelection);
                        return;
                    }
                    bool dragStartSelectionNormalized = _dragStartSelection!.extentOffset >= _dragStartSelection!.baseOffset;
                    newSelection = new global::Doroti.Framework.Services.TextSelection(baseOffset: dragStartSelectionNormalized ? _dragStartSelection!.extentOffset : _dragStartSelection!.baseOffset, extentOffset: position.offset);
                    break;
                }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
                {
                    if (_selection.isCollapsed)
                    {
                        _selectionOverlay.updateMagnifier(_buildMagnifier(currentTextPosition: position, globalGesturePosition: details.globalPosition, renderEditable: renderObject));
                        var currentSelectionLocal = TextSelection.CreateFromPosition(position);
                        _handleSelectionHandleChanged(currentSelectionLocal);
                        return;
                    }
                    newSelection = new global::Doroti.Framework.Services.TextSelection(baseOffset: position.offset, extentOffset: _selection.extentOffset);
                    if (newSelection.baseOffset >= newSelection.extentOffset)
                    {
                        return;
                    }
                    break;
                }
        }
        _selectionOverlay.updateMagnifier(_buildMagnifier(currentTextPosition: (newSelection.extent.offset < newSelection.@base.offset) ? newSelection.extent : newSelection.@base, globalGesturePosition: details.globalPosition, renderEditable: renderObject));
        _handleSelectionHandleChanged(newSelection);
    }

    internal virtual void _handleAnyDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        if (!context.mounted)
        {
            return;
        }
        _dragStartSelection = null;
        bool draggingHandles = _selectionOverlay.isDraggingStartHandle || _selectionOverlay.isDraggingEndHandle;
        if (selectionControls is not TextSelectionHandleControls)
        {
            if (!draggingHandles)
            {
                _selectionOverlay.hideMagnifier();
                if (!_selection.isCollapsed)
                {
                    _selectionOverlay.showToolbar();
                }
            }
            return;
        }
        if (!draggingHandles)
        {
            _selectionOverlay.hideMagnifier();
            if (!_selection.isCollapsed)
            {
                _selectionOverlay.showToolbar(context: context, contextMenuBuilder: contextMenuBuilder);
            }
        }
    }

    internal virtual void _handleSelectionHandleChanged(global::Doroti.Framework.Services.TextSelection newSelection)
    {
        selectionDelegate.userUpdateTextEditingValue(_value.copyWith(selection: newSelection), SelectionChangedCause.drag);
    }

}

public class SelectionOverlay
{
    public virtual BuildContext context { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Foundation.ValueNotifier<MagnifierInfo> _magnifierInfo { get; private set; } = new global::Doroti.Framework.Foundation.ValueNotifier<MagnifierInfo>(MagnifierInfo.empty);
    internal virtual MagnifierController _magnifierController { get; private set; } = new MagnifierController();
    public virtual TextMagnifierConfiguration magnifierConfiguration { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Rendering.TextSelectionHandleType _startHandleType { get; set; } = default!;
    internal virtual double _lineHeightAtStart { get; set; } = default!;
    internal virtual bool _startHandleDragInProgress { get; set; } = false;
    internal virtual bool _isDraggingStartHandle { get; set; } = false;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<bool>? startHandlesVisible { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>? onStartHandleDragStart { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>? onStartHandleDragUpdate { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>? onStartHandleDragEnd { get; private set; }
    internal virtual global::Doroti.Framework.Rendering.TextSelectionHandleType _endHandleType { get; set; } = default!;
    internal virtual double _lineHeightAtEnd { get; set; } = default!;
    internal virtual bool _endHandleDragInProgress { get; set; } = false;
    internal virtual bool _isDraggingEndHandle { get; set; } = false;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<bool>? endHandlesVisible { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>? onEndHandleDragStart { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>? onEndHandleDragUpdate { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>? onEndHandleDragEnd { get; private set; }
    public virtual global::Doroti.Framework.Foundation.ValueListenable<bool>? toolbarVisible { get; private set; }
    internal virtual List<global::Doroti.Framework.Rendering.TextSelectionPoint> _selectionEndpoints { get; set; } = default!;
    public virtual Widget? debugRequiredFor { get; private set; }
    public virtual global::Doroti.Framework.Rendering.LayerLink toolbarLayerLink { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.LayerLink startHandleLayerLink { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.LayerLink endHandleLayerLink { get; private set; } = default!;
    public virtual TextSelectionControls? selectionControls { get; private set; }
    public virtual global::Doroti.Framework.Services.TextSelectionDelegate? selectionDelegate { get; private set; }
    public virtual global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual global::System.Action? onSelectionHandleTapped { get; private set; }
    public virtual ClipboardStatusNotifier? clipboardStatus { get; private set; }
    internal virtual Offset? _toolbarLocation { get; set; } = default;
    public static Duration fadeDuration = Duration.Create(milliseconds: 150L);
    internal virtual (OverlayEntry end, OverlayEntry start)? _handles { get; set; } = default;
    internal virtual OverlayEntry? _toolbar { get; set; } = default;
    internal virtual ContextMenuController _contextMenuController { get; private set; } = new ContextMenuController();
    internal virtual ContextMenuController _spellCheckToolbarController { get; private set; } = new ContextMenuController();
    internal virtual bool _buildScheduled { get; set; } = false;

    public SelectionOverlay(BuildContext context, Widget? debugRequiredFor = null, global::Doroti.Framework.Rendering.TextSelectionHandleType startHandleType = default!, double lineHeightAtStart = default!, global::Doroti.Framework.Foundation.ValueListenable<bool>? startHandlesVisible = null, global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>? onStartHandleDragStart = null, global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>? onStartHandleDragUpdate = null, global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>? onStartHandleDragEnd = null, global::Doroti.Framework.Rendering.TextSelectionHandleType endHandleType = default!, double lineHeightAtEnd = default!, global::Doroti.Framework.Foundation.ValueListenable<bool>? endHandlesVisible = null, global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>? onEndHandleDragStart = null, global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>? onEndHandleDragUpdate = null, global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>? onEndHandleDragEnd = null, global::Doroti.Framework.Foundation.ValueListenable<bool>? toolbarVisible = null, List<global::Doroti.Framework.Rendering.TextSelectionPoint> selectionEndpoints = default!, TextSelectionControls? selectionControls = default!, global::Doroti.Framework.Services.TextSelectionDelegate? selectionDelegate = default!, ClipboardStatusNotifier? clipboardStatus = default!, global::Doroti.Framework.Rendering.LayerLink startHandleLayerLink = default!, global::Doroti.Framework.Rendering.LayerLink endHandleLayerLink = default!, global::Doroti.Framework.Rendering.LayerLink toolbarLayerLink = default!, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = DragStartBehavior.start, global::System.Action? onSelectionHandleTapped = null, Offset? toolbarLocation = null, TextMagnifierConfiguration magnifierConfiguration = default!)
    {
        TextMagnifierConfiguration __magnifierConfiguration = magnifierConfiguration ?? TextMagnifierConfiguration.disabled;
        this.context = context;
        this.debugRequiredFor = debugRequiredFor;
        this.startHandlesVisible = startHandlesVisible;
        this.onStartHandleDragStart = onStartHandleDragStart;
        this.onStartHandleDragUpdate = onStartHandleDragUpdate;
        this.onStartHandleDragEnd = onStartHandleDragEnd;
        this.endHandlesVisible = endHandlesVisible;
        this.onEndHandleDragStart = onEndHandleDragStart;
        this.onEndHandleDragUpdate = onEndHandleDragUpdate;
        this.onEndHandleDragEnd = onEndHandleDragEnd;
        this.toolbarVisible = toolbarVisible;
        this.selectionControls = selectionControls;
        this.selectionDelegate = selectionDelegate;
        this.clipboardStatus = clipboardStatus;
        this.startHandleLayerLink = startHandleLayerLink;
        this.endHandleLayerLink = endHandleLayerLink;
        this.toolbarLayerLink = toolbarLayerLink;
        this.dragStartBehavior = dragStartBehavior;
        this.onSelectionHandleTapped = onSelectionHandleTapped;
        this.magnifierConfiguration = __magnifierConfiguration;
        _startHandleType = startHandleType;
        _lineHeightAtStart = lineHeightAtStart;
        _endHandleType = endHandleType;
        _lineHeightAtEnd = lineHeightAtEnd;
        _selectionEndpoints = selectionEndpoints;
        _toolbarLocation = toolbarLocation;
        System.Diagnostics.Debug.Assert(DebugLibrary.debugCheckHasOverlay(context));
    }

    public virtual bool toolbarIsVisible
    {
        get
        {
            return (selectionControls is TextSelectionHandleControls) ? (_contextMenuController.isShown || _spellCheckToolbarController.isShown) : ((_toolbar is not null) || _spellCheckToolbarController.isShown);
        }
    }
    public virtual bool magnifierIsVisible => _magnifierController.shown;
    public virtual bool magnifierExists => DartRuntimePrimitives.ConvertValue<bool>(_magnifierController.overlayEntry is not null);
    public virtual void showMagnifier(MagnifierInfo initialMagnifierInfo)
    {
        if (_magnifierController.overlayEntry is not null)
        {
            return;
        }
        if (toolbarIsVisible)
        {
            hideToolbar();
        }
        _magnifierInfo.value = initialMagnifierInfo;
        Widget? builtMagnifier = magnifierConfiguration.magnifierBuilder(context, _magnifierController, _magnifierInfo);
        if (builtMagnifier is null)
        {
            return;
        }
        DartRuntimePrimitives.Ignore(_magnifierController.show(context: context, below: magnifierConfiguration.shouldDisplayHandlesInMagnifier ? null : _handles?.start, builder: (_) => builtMagnifier));
    }

    public virtual void hideMagnifier()
    {
        if (_magnifierController.overlayEntry is null)
        {
            return;
        }
        DartRuntimePrimitives.Ignore(_magnifierController.hide());
    }

    public virtual global::Doroti.Framework.Rendering.TextSelectionHandleType startHandleType
    {
        get => _startHandleType;
        set
        {
            var __value = value;
            if (Equals(_startHandleType, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _startHandleType = DartRuntimePrimitives.RequireValue(__value);
            markNeedsBuild();
        }
    }
    public virtual double lineHeightAtStart
    {
        get => _lineHeightAtStart;
        set
        {
            var __value = value;
            if (_lineHeightAtStart == DartRuntimePrimitives.RequireValue(__value))
            {
                return;
            }
            _lineHeightAtStart = DartRuntimePrimitives.RequireValue(__value);
            markNeedsBuild();
        }
    }
    public virtual bool isDraggingStartHandle => DartRuntimePrimitives.ConvertValue<bool>(_isDraggingStartHandle || _startHandleDragInProgress);
    internal virtual bool _canDragStartHandle => DartRuntimePrimitives.ConvertValue<bool>(!_isDraggingEndHandle || (!Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS)) && (!Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.macOS)) && !Foundation.ConstantsLibrary.kIsWeb);
    internal virtual void _handleStartHandleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        DartRuntimePrimitives.Assert(() => !_isDraggingStartHandle);
        if (_handles is null)
        {
            _isDraggingStartHandle = false;
            return;
        }
        _startHandleDragInProgress = true;
        if (!_canDragStartHandle)
        {
            return;
        }
        _isDraggingStartHandle = Equals(details.kind, PointerDeviceKind.touch);
        onStartHandleDragStart?.Invoke(details);
    }

    internal virtual void _handleStartHandleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        if (_handles is null)
        {
            _isDraggingStartHandle = false;
            return;
        }
        if (!_canDragStartHandle)
        {
            return;
        }
        if (!_isDraggingStartHandle)
        {
            _isDraggingStartHandle = Equals(details.kind, PointerDeviceKind.touch);
            var startDetails = new global::Doroti.Framework.Gestures.DragStartDetails(globalPosition: details.globalPosition, localPosition: details.localPosition, sourceTimeStamp: details.sourceTimeStamp, kind: details.kind);
            onStartHandleDragStart?.Invoke(startDetails);
        }
        onStartHandleDragUpdate?.Invoke(details);
    }

    internal virtual void _handleStartHandleDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        _isDraggingStartHandle = false;
        if (_handles is null)
        {
            return;
        }
        _startHandleDragInProgress = false;
        if (!_canDragStartHandle)
        {
            return;
        }
        onStartHandleDragEnd?.Invoke(details);
    }

    public virtual global::Doroti.Framework.Rendering.TextSelectionHandleType endHandleType
    {
        get => _endHandleType;
        set
        {
            var __value = value;
            if (Equals(_endHandleType, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _endHandleType = DartRuntimePrimitives.RequireValue(__value);
            markNeedsBuild();
        }
    }
    public virtual double lineHeightAtEnd
    {
        get => _lineHeightAtEnd;
        set
        {
            var __value = value;
            if (_lineHeightAtEnd == DartRuntimePrimitives.RequireValue(__value))
            {
                return;
            }
            _lineHeightAtEnd = DartRuntimePrimitives.RequireValue(__value);
            markNeedsBuild();
        }
    }
    public virtual bool isDraggingEndHandle => DartRuntimePrimitives.ConvertValue<bool>(_isDraggingEndHandle || _endHandleDragInProgress);
    internal virtual bool _canDragEndHandle => DartRuntimePrimitives.ConvertValue<bool>(!_isDraggingStartHandle || (!Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS)) && (!Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.macOS)) && !Foundation.ConstantsLibrary.kIsWeb);
    internal virtual void _handleEndHandleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        DartRuntimePrimitives.Assert(() => !_isDraggingEndHandle);
        if (_handles is null)
        {
            _isDraggingEndHandle = false;
            return;
        }
        _endHandleDragInProgress = true;
        if (!_canDragEndHandle)
        {
            return;
        }
        _isDraggingEndHandle = Equals(details.kind, PointerDeviceKind.touch);
        onEndHandleDragStart?.Invoke(details);
    }

    internal virtual void _handleEndHandleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        if (_handles is null)
        {
            _isDraggingEndHandle = false;
            return;
        }
        if (!_canDragEndHandle)
        {
            return;
        }
        if (!_isDraggingEndHandle)
        {
            _isDraggingEndHandle = Equals(details.kind, PointerDeviceKind.touch);
            var startDetails = new global::Doroti.Framework.Gestures.DragStartDetails(globalPosition: details.globalPosition, localPosition: details.localPosition, sourceTimeStamp: details.sourceTimeStamp, kind: details.kind);
            onEndHandleDragStart?.Invoke(startDetails);
        }
        onEndHandleDragUpdate?.Invoke(details);
    }

    internal virtual void _handleEndHandleDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        _isDraggingEndHandle = false;
        if (_handles is null)
        {
            return;
        }
        _endHandleDragInProgress = false;
        if (!_canDragEndHandle)
        {
            return;
        }
        onEndHandleDragEnd?.Invoke(details);
    }

    public virtual List<global::Doroti.Framework.Rendering.TextSelectionPoint> selectionEndpoints
    {
        get => _selectionEndpoints;
        set
        {
            var __value = value;
            if (!CollectionsLibrary.listEquals(_selectionEndpoints, __value))
            {
                markNeedsBuild();
                if (_isDraggingEndHandle || _isDraggingStartHandle)
                {
                    switch (PlatformLibrary.defaultTargetPlatform)
                    {
                        case TargetPlatform.android:
                            {
                                DartRuntimePrimitives.Ignore(HapticFeedback.selectionClick());
                                break;
                            }
                        case TargetPlatform.fuchsia:
                        case TargetPlatform.iOS:
                        case TargetPlatform.linux:
                        case TargetPlatform.macOS:
                        case TargetPlatform.windows:
                            {
                                break;
                            }
                    }
                }
            }
            _selectionEndpoints = __value;
        }
    }
    public virtual global::Doroti.Ui.Offset? toolbarLocation
    {
        get => _toolbarLocation;
        set
        {
            var __value = value;
            if (Equals(_toolbarLocation, __value))
            {
                return;
            }
            _toolbarLocation = __value;
            markNeedsBuild();
        }
    }
    public virtual void showHandles()
    {
        if (_handles is not null)
        {
            return;
        }
        OverlayState overlay = Overlay.of(context, rootOverlay: true, debugRequiredFor: debugRequiredFor);
        CapturedThemes capturedThemes = InheritedTheme.capture(from: context, to: overlay.context);
        _handles = (end: new OverlayEntry(builder: (context) =>
        {
            return capturedThemes.wrap(_buildEndHandle(context));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }), start: new OverlayEntry(builder: (context) =>
        {
            return capturedThemes.wrap(_buildStartHandle(context));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        overlay.insertAll(new List<OverlayEntry> { DartRuntimePrimitives.RequireValue(_handles).start, DartRuntimePrimitives.RequireValue(_handles).end }.Cast<OverlayEntry>());
    }

    public virtual void hideHandles()
    {
        if (_handles is not null)
        {
            DartRuntimePrimitives.RequireValue(_handles).start.remove();
            DartRuntimePrimitives.RequireValue(_handles).start.dispose();
            DartRuntimePrimitives.RequireValue(_handles).end.remove();
            DartRuntimePrimitives.RequireValue(_handles).end.dispose();
            _handles = null;
        }
    }

    public virtual void showToolbar(BuildContext? context = null, global::System.Func<BuildContext, Widget>? contextMenuBuilder = null)
    {
        if (contextMenuBuilder is null)
        {
            if (_toolbar is not null)
            {
                return;
            }
            _toolbar = new OverlayEntry(builder: _buildToolbar);
            Overlay.of(this.context, rootOverlay: true, debugRequiredFor: debugRequiredFor).insert(_toolbar!, above: _handles?.end);
            return;
        }
        if (context is null)
        {
            return;
        }
        var renderBox = ((global::Doroti.Framework.Rendering.RenderBox?)context.findRenderObject()!)!;
        _contextMenuController.show(context: context, contextMenuBuilder: (context) =>
        {
            return new _SelectionToolbarWrapper__text_selection(visibility: toolbarVisible, layerLink: toolbarLayerLink, offset: -renderBox.localToGlobal(Offset.zero), child: contextMenuBuilder(context));
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
    }

    public virtual void showSpellCheckSuggestionsToolbar(BuildContext? context = null, global::System.Func<BuildContext, Widget> builder = default!)
    {
        if (context is null)
        {
            return;
        }
        var renderBox = ((global::Doroti.Framework.Rendering.RenderBox?)context.findRenderObject()!)!;
        _spellCheckToolbarController.show(context: context, contextMenuBuilder: (context) =>
        {
            return new _SelectionToolbarWrapper__text_selection(layerLink: toolbarLayerLink, offset: -renderBox.localToGlobal(Offset.zero), child: builder(context));
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
    }

    public virtual void markNeedsBuild()
    {
        if ((_handles is null) && (_toolbar is null) && !_contextMenuController.isShown && !_spellCheckToolbarController.isShown)
        {
            return;
        }
        if (Equals(Scheduler.SchedulerBinding.instance.schedulerPhase, Scheduler.SchedulerPhase.persistentCallbacks))
        {
            if (_buildScheduled)
            {
                return;
            }
            _buildScheduled = true;
            Scheduler.SchedulerBinding.instance.addPostFrameCallback((duration) =>
            {
                _buildScheduled = false;
                _handles?.start.markNeedsBuild();
                _handles?.end.markNeedsBuild();
                _toolbar?.markNeedsBuild();
                if (_contextMenuController.isShown)
                {
                    _contextMenuController.markNeedsBuild();
                }
                else
                {
                    if (_spellCheckToolbarController.isShown)
                    {
                        _spellCheckToolbarController.markNeedsBuild();
                    }
                }
            }, debugLabel: "SelectionOverlay.markNeedsBuild");
        }
        else
        {
            if (_handles is not null)
            {
                DartRuntimePrimitives.RequireValue(_handles).start.markNeedsBuild();
                DartRuntimePrimitives.RequireValue(_handles).end.markNeedsBuild();
            }
            _toolbar?.markNeedsBuild();
            if (_contextMenuController.isShown)
            {
                _contextMenuController.markNeedsBuild();
            }
            else
            {
                if (_spellCheckToolbarController.isShown)
                {
                    _spellCheckToolbarController.markNeedsBuild();
                }
            }
        }
    }

    public virtual void hide()
    {
        DartRuntimePrimitives.Ignore(_magnifierController.hide());
        hideHandles();
        if ((_toolbar is not null) || _contextMenuController.isShown || _spellCheckToolbarController.isShown)
        {
            hideToolbar();
        }
    }

    public virtual void hideToolbar()
    {
        _contextMenuController.remove();
        _spellCheckToolbarController.remove();
        if (_toolbar is null)
        {
            return;
        }
        _toolbar?.remove();
        _toolbar?.dispose();
        _toolbar = null;
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        hide();
        _magnifierInfo.dispose();
    }

    internal virtual Widget _buildStartHandle(BuildContext context)
    {
        Widget handle = default!;
        TextSelectionControls? selectionControlsLocal = selectionControls;
        if ((selectionControlsLocal is null) || Equals(_startHandleType, TextSelectionHandleType.collapsed) && _isDraggingEndHandle)
        {
            handle = DartRuntimePrimitives.ConvertValue<Widget>(SizedBox.CreateShrink());
        }
        else
        {
            handle = DartRuntimePrimitives.ConvertValue<Widget>(new _SelectionHandleOverlay__text_selection(type: _startHandleType, handleLayerLink: startHandleLayerLink, onSelectionHandleTapped: onSelectionHandleTapped, onSelectionHandleDragStart: _handleStartHandleDragStart, onSelectionHandleDragUpdate: _handleStartHandleDragUpdate, onSelectionHandleDragEnd: _handleStartHandleDragEnd, selectionControls: selectionControlsLocal, visibility: startHandlesVisible, preferredLineHeight: _lineHeightAtStart, dragStartBehavior: dragStartBehavior));
        }
        return new TapRegion(groupId: typeof(SelectableRegion), child: new TextFieldTapRegion(child: new ExcludeSemantics(child: handle)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildEndHandle(BuildContext context)
    {
        Widget handle = default!;
        TextSelectionControls? selectionControlsLocal = selectionControls;
        if ((selectionControlsLocal is null) || Equals(_endHandleType, TextSelectionHandleType.collapsed) && _isDraggingStartHandle || Equals(_endHandleType, TextSelectionHandleType.collapsed) && !_isDraggingStartHandle && !_isDraggingEndHandle)
        {
            handle = DartRuntimePrimitives.ConvertValue<Widget>(SizedBox.CreateShrink());
        }
        else
        {
            handle = DartRuntimePrimitives.ConvertValue<Widget>(new _SelectionHandleOverlay__text_selection(type: _endHandleType, handleLayerLink: endHandleLayerLink, onSelectionHandleTapped: onSelectionHandleTapped, onSelectionHandleDragStart: _handleEndHandleDragStart, onSelectionHandleDragUpdate: _handleEndHandleDragUpdate, onSelectionHandleDragEnd: _handleEndHandleDragEnd, selectionControls: selectionControlsLocal, visibility: endHandlesVisible, preferredLineHeight: _lineHeightAtEnd, dragStartBehavior: dragStartBehavior));
        }
        return new TapRegion(groupId: typeof(SelectableRegion), child: new TextFieldTapRegion(child: new ExcludeSemantics(child: handle)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildToolbar(BuildContext context)
    {
        if (selectionControls is null)
        {
            return SizedBox.CreateShrink();
        }
        DartRuntimePrimitives.Assert(() => selectionDelegate is not null, () => (object?)"If not using contextMenuBuilder, must pass selectionDelegate.");
        var renderBox = ((global::Doroti.Framework.Rendering.RenderBox?)this.context.findRenderObject()!)!;
        var editingRegion = Rect.fromPoints(renderBox.localToGlobal(Offset.zero), renderBox.localToGlobal(renderBox.size.bottomRight(Offset.zero)));
        bool isMultiline = (selectionEndpoints.Last().point.dy - selectionEndpoints.First().point.dy) > (lineHeightAtEnd / 2L);
        double midX = isMultiline ? (editingRegion.width / 2L) : ((selectionEndpoints.First().point.dx + selectionEndpoints.Last().point.dx) / 2L);
        var midpoint = new global::Doroti.Ui.Offset(midX, selectionEndpoints.First().point.dy - lineHeightAtStart);
        return new _SelectionToolbarWrapper__text_selection(visibility: toolbarVisible, layerLink: toolbarLayerLink, offset: -editingRegion.topLeft, child: new Builder(builder: (context) =>
        {
            return selectionControls!.buildToolbar(context, editingRegion, lineHeightAtStart, midpoint, selectionEndpoints, selectionDelegate!, clipboardStatus, toolbarLocation);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void updateMagnifier(MagnifierInfo magnifierInfo)
    {
        if (_magnifierController.overlayEntry is null)
        {
            return;
        }
        _magnifierInfo.value = magnifierInfo;
    }

}

public class _SelectionToolbarWrapper__text_selection : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual Offset offset { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.LayerLink layerLink { get; private set; } = default!;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<bool>? visibility { get; private set; }

    internal _SelectionToolbarWrapper__text_selection(global::Doroti.Framework.Foundation.ValueListenable<bool>? visibility = null, global::Doroti.Framework.Rendering.LayerLink layerLink = default!, Offset offset = default!, Widget child = default!)
    {
        this.visibility = visibility;
        this.layerLink = layerLink;
        this.offset = offset;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SelectionToolbarWrapperState__text_selection());
}

internal class _SelectionToolbarWrapperState__text_selection : State<_SelectionToolbarWrapper__text_selection>, SingleTickerProviderStateMixin<_SelectionToolbarWrapper__text_selection>
{
    internal virtual global::Doroti.Framework.Animation.AnimationController _controller { get; set; } = default!;
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual global::Doroti.Framework.Animation.Animation<double> _opacity => _controller.view;
    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Framework.Animation.AnimationController(duration: SelectionOverlay.fadeDuration, vsync: this);
        _toolbarVisibilityChanged();
        widget.visibility?.addListener(_toolbarVisibilityChanged);
    }

    public override void didUpdateWidget(_SelectionToolbarWrapper__text_selection oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (Equals(oldWidget.visibility, widget.visibility))
        {
            return;
        }
        oldWidget.visibility?.removeListener(_toolbarVisibilityChanged);
        _toolbarVisibilityChanged();
        widget.visibility?.addListener(_toolbarVisibilityChanged);
    }

    public override void dispose()
    {
        widget.visibility?.removeListener(_toolbarVisibilityChanged);
        _controller.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((_ticker is null) || !_ticker!.isActive)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), _ticker!.describeForError("The offending ticker was") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _tickerModeNotifier?.removeListener(_updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _toolbarVisibilityChanged()
    {
        if (widget.visibility?.value ?? true)
        {
            _controller.forward();
        }
        else
        {
            _controller.reverse();
        }
    }

    public override Widget build(BuildContext context)
    {
        return new TapRegion(groupId: typeof(SelectableRegion), child: new TextFieldTapRegion(child: new Directionality(textDirection: Directionality.of(this.context), child: new FadeTransition(opacity: _opacity, child: new CompositedTransformFollower(link: widget.layerLink, showWhenUnlinked: false, offset: widget.offset, child: widget.child)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (_ticker is null)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _ticker = new global::Doroti.Framework.Scheduler.Ticker(onTick, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
        _updateTickerModeNotifier();
        _updateTicker();
        return _ticker!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTicker();
    }

    public virtual void _updateTicker()
    {
        TickerModeData values = _tickerModeNotifier!.value;
        if (_ticker is not null)
        {
            _ticker!.muted = !values.enabled;
            _ticker!.forceFrames = values.forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTicker);
        newNotifier.addListener(_updateTicker);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) };
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", _ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
    }

}

public class _SelectionHandleOverlay__text_selection : StatefulWidget
{
    public virtual global::Doroti.Framework.Rendering.LayerLink handleLayerLink { get; private set; } = default!;
    public virtual global::System.Action? onSelectionHandleTapped { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>? onSelectionHandleDragStart { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>? onSelectionHandleDragUpdate { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>? onSelectionHandleDragEnd { get; private set; }
    public virtual TextSelectionControls selectionControls { get; private set; } = default!;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<bool>? visibility { get; private set; }
    public virtual double preferredLineHeight { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.TextSelectionHandleType type { get; private set; } = default!;
    public virtual global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;

    internal _SelectionHandleOverlay__text_selection(global::Doroti.Framework.Rendering.TextSelectionHandleType type, global::Doroti.Framework.Rendering.LayerLink handleLayerLink, global::System.Action? onSelectionHandleTapped = null, global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>? onSelectionHandleDragStart = null, global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>? onSelectionHandleDragUpdate = null, global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>? onSelectionHandleDragEnd = null, TextSelectionControls selectionControls = default!, global::Doroti.Framework.Foundation.ValueListenable<bool>? visibility = null, double preferredLineHeight = default!, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = DragStartBehavior.start)
    {
        this.type = type;
        this.handleLayerLink = handleLayerLink;
        this.onSelectionHandleTapped = onSelectionHandleTapped;
        this.onSelectionHandleDragStart = onSelectionHandleDragStart;
        this.onSelectionHandleDragUpdate = onSelectionHandleDragUpdate;
        this.onSelectionHandleDragEnd = onSelectionHandleDragEnd;
        this.selectionControls = selectionControls;
        this.visibility = visibility;
        this.preferredLineHeight = preferredLineHeight;
        this.dragStartBehavior = dragStartBehavior;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SelectionHandleOverlayState__text_selection());
}

internal class _SelectionHandleOverlayState__text_selection : State<_SelectionHandleOverlay__text_selection>, SingleTickerProviderStateMixin<_SelectionHandleOverlay__text_selection>
{
    internal virtual global::Doroti.Framework.Animation.AnimationController _controller { get; set; } = default!;
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual global::Doroti.Framework.Animation.Animation<double> _opacity => _controller.view;
    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Framework.Animation.AnimationController(duration: SelectionOverlay.fadeDuration, vsync: this);
        _handleVisibilityChanged();
        widget.visibility?.addListener(_handleVisibilityChanged);
    }

    internal virtual void _handleVisibilityChanged()
    {
        if (widget.visibility?.value ?? true)
        {
            _controller.forward();
        }
        else
        {
            _controller.reverse();
        }
    }

    internal virtual global::Doroti.Ui.Rect _getHandleRect(global::Doroti.Framework.Rendering.TextSelectionHandleType type, double preferredLineHeight)
    {
        global::Doroti.Ui.Size handleSize = widget.selectionControls.getHandleSize(preferredLineHeight);
        return Rect.fromLTWH(0.0, 0.0, handleSize.width, handleSize.height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didUpdateWidget(_SelectionHandleOverlay__text_selection oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        oldWidget.visibility?.removeListener(_handleVisibilityChanged);
        _handleVisibilityChanged();
        widget.visibility?.addListener(_handleVisibilityChanged);
    }

    public override void dispose()
    {
        widget.visibility?.removeListener(_handleVisibilityChanged);
        _controller.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((_ticker is null) || !_ticker!.isActive)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), _ticker!.describeForError("The offending ticker was") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _tickerModeNotifier?.removeListener(_updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        global::Doroti.Ui.Rect handleRect = _getHandleRect(widget.type, widget.preferredLineHeight);
        global::Doroti.Ui.Rect interactiveRect = handleRect.isEmpty ? handleRect : handleRect.expandToInclude(Rect.fromCircle(center: handleRect.center, radius: ConstantsLibrary.kMinInteractiveDimension / 2L));
        global::Doroti.Framework.Rendering.RelativeRect paddingLocal = interactiveRect.isEmpty ? RelativeRect.fill : new global::Doroti.Framework.Rendering.RelativeRect(Math.Max((interactiveRect.width - handleRect.width) / 2L, 0), Math.Max((interactiveRect.height - handleRect.height) / 2L, 0), Math.Max((interactiveRect.width - handleRect.width) / 2L, 0), Math.Max((interactiveRect.height - handleRect.height) / 2L, 0));
        global::Doroti.Ui.Offset handleAnchor = widget.selectionControls.getHandleAnchor(widget.type, widget.preferredLineHeight);
        bool eagerlyAcceptDragWhenCollapsed = Equals(widget.type, TextSelectionHandleType.collapsed) && Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS);
        return new CompositedTransformFollower(link: widget.handleLayerLink, offset: -handleAnchor - new global::Doroti.Ui.Offset(paddingLocal.left, paddingLocal.top), showWhenUnlinked: false, child: new FadeTransition(opacity: _opacity, child: new SizedBox(width: interactiveRect.width, height: interactiveRect.height, child: new Align(alignment: Alignment.topLeft, child: new RawGestureDetector(behavior: HitTestBehavior.translucent, gestures: new DartMap<Type, dynamic>
        {
            [typeof(global::Doroti.Framework.Gestures.PanGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.PanGestureRecognizer>(() => new global::Doroti.Framework.Gestures.PanGestureRecognizer(debugOwner: this, supportedDevices: new HashSet<PointerDeviceKind> { PointerDeviceKind.touch, PointerDeviceKind.stylus, PointerDeviceKind.unknown }), (instance) =>
            {
                DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.PanGestureRecognizer>)(() =>
                {
                    var __cascade = instance;
                    __cascade.dragStartBehavior = widget.dragStartBehavior;
                    __cascade.gestureSettings = eagerlyAcceptDragWhenCollapsed ? new global::Doroti.Framework.Gestures.DeviceGestureSettings(touchSlop: 1.0) : null;
                    __cascade.onStart = widget.onSelectionHandleDragStart;
                    __cascade.onUpdate = widget.onSelectionHandleDragUpdate;
                    __cascade.onEnd = widget.onSelectionHandleDragEnd;
                    return __cascade;
                }))());
            })
        }, child: new Padding(padding: EdgeInsets.CreateOnly(left: paddingLocal.left, top: paddingLocal.top, right: paddingLocal.right, bottom: paddingLocal.bottom), child: widget.selectionControls.buildHandle(context, widget.type, widget.preferredLineHeight, () => widget.onSelectionHandleTapped?.Invoke())))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (_ticker is null)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _ticker = new global::Doroti.Framework.Scheduler.Ticker(onTick, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
        _updateTickerModeNotifier();
        _updateTicker();
        return _ticker!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTicker();
    }

    public virtual void _updateTicker()
    {
        TickerModeData values = _tickerModeNotifier!.value;
        if (_ticker is not null)
        {
            _ticker!.muted = !values.enabled;
            _ticker!.forceFrames = values.forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTicker);
        newNotifier.addListener(_updateTicker);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) };
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", _ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
    }

}

public interface TextSelectionGestureDetectorBuilderDelegate
{
    public GlobalKey<EditableTextState> editableTextKey { get; }
    public bool forcePressEnabled { get; }
    public bool selectionEnabled { get; }
}

public class TextSelectionGestureDetectorBuilder
{
    public virtual TextSelectionGestureDetectorBuilderDelegate @delegate { get; private set; } = default!;
    internal virtual bool _shouldShowSelectionToolbar { get; set; } = true;
    internal virtual bool _shouldShowSelectionHandles { get; set; } = true;
    internal virtual bool _isShiftPressed { get; set; } = false;
    internal virtual double _dragStartScrollOffset { get; set; } = 0.0;
    internal virtual double _dragStartViewportOffset { get; set; } = 0.0;
    internal virtual global::Doroti.Framework.Services.TextSelection? _dragStartSelection { get; set; } = default;
    internal virtual bool _longPressStartedWithoutFocus { get; set; } = false;

    public TextSelectionGestureDetectorBuilder(TextSelectionGestureDetectorBuilderDelegate @delegate)
    {
        this.@delegate = @delegate;
    }

    internal virtual void _showMagnifierIfSupportedByPlatform(Offset positionToShow)
    {
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.android:
            case TargetPlatform.iOS:
                {
                    editableText.showMagnifier(positionToShow);
                    break;
                }
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.macOS:
            case TargetPlatform.windows:
                break;
        }
    }

    internal virtual void _hideMagnifierIfSupportedByPlatform()
    {
        if (!_isEditableTextMounted)
        {
            return;
        }
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.android:
            case TargetPlatform.iOS:
                {
                    editableText.hideMagnifier();
                    break;
                }
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.macOS:
            case TargetPlatform.windows:
                break;
        }
    }

    internal virtual bool _lastSecondaryTapWasOnSelection
    {
        get
        {
            DartRuntimePrimitives.Assert(() => renderEditable.lastSecondaryTapDownPosition is not null);
            if (renderEditable.selection is null)
            {
                return false;
            }
            global::Doroti.Ui.TextPosition textPosition = renderEditable.getPositionForPoint(DartRuntimePrimitives.RequireValue(renderEditable.lastSecondaryTapDownPosition));
            return (renderEditable.selection!.start <= textPosition.offset) && (renderEditable.selection!.end >= textPosition.offset);
        }
    }
    internal virtual bool _positionWasOnSelectionExclusive(TextPosition textPosition)
    {
        global::Doroti.Framework.Services.TextSelection? selectionLocal = renderEditable.selection;
        if (selectionLocal is null)
        {
            return false;
        }
        return (selectionLocal.start < textPosition.offset) && (selectionLocal.end > textPosition.offset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _positionWasOnSelectionInclusive(TextPosition textPosition)
    {
        global::Doroti.Framework.Services.TextSelection? selectionLocal = renderEditable.selection;
        if (selectionLocal is null)
        {
            return false;
        }
        return (selectionLocal.start <= textPosition.offset) && (selectionLocal.end >= textPosition.offset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _expandSelection(Offset offset, global::Doroti.Framework.Services.SelectionChangedCause cause, global::Doroti.Framework.Services.TextSelection? fromSelection = null)
    {
        DartRuntimePrimitives.Assert(() => renderEditable.selection?.baseOffset is not null);
        global::Doroti.Ui.TextPosition tappedPosition = renderEditable.getPositionForPoint(offset);
        global::Doroti.Framework.Services.TextSelection selectionLocal = fromSelection ?? renderEditable.selection!;
        bool baseIsCloser = (tappedPosition.offset - selectionLocal.baseOffset).abs() < (tappedPosition.offset - selectionLocal.extentOffset).abs();
        global::Doroti.Framework.Services.TextSelection nextSelection = selectionLocal.copyWith(baseOffset: baseIsCloser ? selectionLocal.extentOffset : selectionLocal.baseOffset, extentOffset: tappedPosition.offset);
        editableText.userUpdateTextEditingValue(editableText.textEditingValue.copyWith(selection: nextSelection), DartRuntimePrimitives.RequireValue(cause));
    }

    internal virtual void _extendSelection(Offset offset, global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
        DartRuntimePrimitives.Assert(() => renderEditable.selection?.baseOffset is not null);
        global::Doroti.Ui.TextPosition tappedPosition = renderEditable.getPositionForPoint(offset);
        global::Doroti.Framework.Services.TextSelection selectionLocal = renderEditable.selection!;
        global::Doroti.Framework.Services.TextSelection nextSelection = selectionLocal.copyWith(extentOffset: tappedPosition.offset);
        editableText.userUpdateTextEditingValue(editableText.textEditingValue.copyWith(selection: nextSelection), DartRuntimePrimitives.RequireValue(cause));
    }

    public virtual bool shouldShowSelectionToolbar => _shouldShowSelectionToolbar;
    public virtual bool shouldShowSelectionHandles => _shouldShowSelectionHandles;
    public virtual EditableTextState editableText => DartRuntimePrimitives.ConvertValue<EditableTextState>(@delegate.editableTextKey.currentState!);
    public virtual global::Doroti.Framework.Rendering.RenderEditable renderEditable => editableText.renderEditable;
    internal virtual bool _isEditableTextMounted => DartRuntimePrimitives.ConvertValue<bool>(@delegate.editableTextKey.currentContext?.mounted ?? false);
    internal virtual double _scrollPosition
    {
        get
        {
            ScrollableState? scrollableState = (@delegate.editableTextKey.currentContext is null) ? null : Scrollable.maybeOf(@delegate.editableTextKey.currentContext!);
            return (scrollableState is null) ? 0.0 : scrollableState.position.pixels;
        }
    }
    internal virtual global::Doroti.Framework.Painting.AxisDirection? _scrollDirection
    {
        get
        {
            ScrollableState? scrollableState = (@delegate.editableTextKey.currentContext is null) ? null : Scrollable.maybeOf(@delegate.editableTextKey.currentContext!);
            return scrollableState?.axisDirection;
        }
    }
    public virtual void onTapTrackStart()
    {
        _isShiftPressed = Enumerable.Any(HardwareKeyboard.instance.logicalKeysPressed.intersection(new HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> { LogicalKeyboardKey.shiftLeft, LogicalKeyboardKey.shiftRight }));
    }

    public virtual void onTapTrackReset()
    {
        _isShiftPressed = false;
    }

    public virtual void onTapDown(global::Doroti.Framework.Gestures.TapDragDownDetails details)
    {
        if (!@delegate.selectionEnabled)
        {
            return;
        }
        renderEditable.handleTapDown(new global::Doroti.Framework.Gestures.TapDownDetails(globalPosition: details.globalPosition));
        global::Doroti.Ui.PointerDeviceKind? kindLocal = details.kind;
        _shouldShowSelectionToolbar = (kindLocal is null) || Equals(DartRuntimePrimitives.RequireValue(kindLocal), PointerDeviceKind.touch) || Equals(DartRuntimePrimitives.RequireValue(kindLocal), PointerDeviceKind.stylus);
        _shouldShowSelectionHandles = _shouldShowSelectionToolbar;
        bool isShiftPressedValid = _isShiftPressed && (renderEditable.selection?.baseOffset is not null);
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.android:
                {
                    if (editableText.widget.stylusHandwritingEnabled)
                    {
                        bool stylusEnabled = kindLocal switch { PointerDeviceKind.stylus => editableText.widget.stylusHandwritingEnabled, PointerDeviceKind.invertedStylus => editableText.widget.stylusHandwritingEnabled, _ => false };
                        if (stylusEnabled)
                        {
                            DartRuntimePrimitives.Ignore(Scribe.isFeatureAvailable().then((isAvailable) =>
                            {
                                if (isAvailable)
                                {
                                    renderEditable.selectPosition(cause: SelectionChangedCause.stylusHandwriting);
                                    DartRuntimePrimitives.Ignore(Scribe.startStylusHandwriting());
                                }
                            }));
                        }
                    }
                    break;
                }
            case TargetPlatform.fuchsia:
            case TargetPlatform.iOS:
                {
                    break;
                }
            case TargetPlatform.macOS:
                {
                    editableText.hideToolbar();
                    if (isShiftPressedValid)
                    {
                        global::Doroti.Framework.Services.TextSelection? fromSelection = renderEditable.hasFocus ? null : TextSelection.CreateCollapsed(offset: 0L);
                        _expandSelection(details.globalPosition, SelectionChangedCause.tap, fromSelection);
                        return;
                    }
                    renderEditable.selectPosition(cause: SelectionChangedCause.tap);
                    break;
                }
            case TargetPlatform.linux:
            case TargetPlatform.windows:
                {
                    editableText.hideToolbar();
                    if (isShiftPressedValid)
                    {
                        _extendSelection(details.globalPosition, SelectionChangedCause.tap);
                        return;
                    }
                    renderEditable.selectPosition(cause: SelectionChangedCause.tap);
                    break;
                }
        }
    }

    public virtual void onForcePressStart(global::Doroti.Framework.Gestures.ForcePressDetails details)
    {
        DartRuntimePrimitives.Assert(() => @delegate.forcePressEnabled);
        _shouldShowSelectionToolbar = true;
        if (!@delegate.selectionEnabled)
        {
            return;
        }
        renderEditable.selectWordsInRange(from: details.globalPosition, cause: SelectionChangedCause.forcePress);
        editableText.showToolbar();
    }

    public virtual void onForcePressEnd(global::Doroti.Framework.Gestures.ForcePressDetails details)
    {
        DartRuntimePrimitives.Assert(() => @delegate.forcePressEnabled);
        renderEditable.selectWordsInRange(from: details.globalPosition, cause: SelectionChangedCause.forcePress);
        if (shouldShowSelectionToolbar)
        {
            editableText.showToolbar();
        }
    }

    public virtual bool onUserTapAlwaysCalled => false;
    public virtual void onUserTap()
    {
    }

    public virtual void onSingleTapUp(global::Doroti.Framework.Gestures.TapDragUpDetails details)
    {
        if (!@delegate.selectionEnabled)
        {
            editableText.requestKeyboard();
            return;
        }
        bool isShiftPressedValid = _isShiftPressed && (renderEditable.selection?.baseOffset is not null);
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.linux:
            case TargetPlatform.macOS:
            case TargetPlatform.windows:
                {
                    break;
                }
            case TargetPlatform.android:
                {
                    editableText.hideToolbar(false);
                    if (isShiftPressedValid)
                    {
                        _extendSelection(details.globalPosition, SelectionChangedCause.tap);
                        return;
                    }
                    renderEditable.selectPosition(cause: SelectionChangedCause.tap);
                    editableText.showSpellCheckSuggestionsToolbar();
                    break;
                }
            case TargetPlatform.fuchsia:
                {
                    editableText.hideToolbar(false);
                    if (isShiftPressedValid)
                    {
                        _extendSelection(details.globalPosition, SelectionChangedCause.tap);
                        return;
                    }
                    renderEditable.selectPosition(cause: SelectionChangedCause.tap);
                    break;
                }
            case TargetPlatform.iOS:
                {
                    if (isShiftPressedValid)
                    {
                        global::Doroti.Framework.Services.TextSelection? fromSelection = renderEditable.hasFocus ? null : TextSelection.CreateCollapsed(offset: 0L);
                        _expandSelection(details.globalPosition, SelectionChangedCause.tap, fromSelection);
                        return;
                    }
                    switch (details.kind)
                    {
                        case PointerDeviceKind.mouse:
                        case PointerDeviceKind.trackpad:
                        case PointerDeviceKind.stylus:
                        case PointerDeviceKind.invertedStylus:
                            {
                                renderEditable.selectPosition(cause: SelectionChangedCause.tap);
                                editableText.hideToolbar();
                                break;
                            }
                        case PointerDeviceKind.touch:
                        case PointerDeviceKind.unknown:
                            {
                                global::Doroti.Framework.Services.TextSelection previousSelection = renderEditable.selection ?? editableText.textEditingValue.selection;
                                global::Doroti.Ui.TextPosition textPosition = renderEditable.getPositionForPoint(details.globalPosition);
                                var isAffinityTheSame = Equals(textPosition.affinity, previousSelection.affinity);
                                var wordAtCursorIndexIsMisspelled = editableText.findSuggestionSpanAtCursorIndex(textPosition.offset) is not null;
                                if (wordAtCursorIndexIsMisspelled)
                                {
                                    renderEditable.selectWord(cause: SelectionChangedCause.tap);
                                    if (!Equals(previousSelection, editableText.textEditingValue.selection))
                                    {
                                        editableText.showSpellCheckSuggestionsToolbar();
                                    }
                                    else
                                    {
                                        editableText.toggleToolbar(false);
                                    }
                                }
                                else
                                {
                                    if ((_positionWasOnSelectionExclusive(textPosition) && !previousSelection.isCollapsed || _positionWasOnSelectionInclusive(textPosition) && previousSelection.isCollapsed && isAffinityTheSame && !renderEditable.readOnly) && renderEditable.hasFocus)
                                    {
                                        editableText.toggleToolbar(false);
                                    }
                                    else
                                    {
                                        renderEditable.selectWordEdge(cause: SelectionChangedCause.tap);
                                        if (Equals(previousSelection, editableText.textEditingValue.selection) && renderEditable.hasFocus && !renderEditable.readOnly)
                                        {
                                            editableText.toggleToolbar(false);
                                        }
                                        else
                                        {
                                            editableText.hideToolbar(false);
                                        }
                                    }
                                }
                                break;
                            }
                    }
                    break;
                }
        }
        editableText.requestKeyboard();
    }

    public virtual void onSingleTapCancel()
    {
    }

    public virtual void onSingleLongTapStart(global::Doroti.Framework.Gestures.LongPressStartDetails details)
    {
        if (!@delegate.selectionEnabled)
        {
            return;
        }
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
                {
                    if (!renderEditable.hasFocus)
                    {
                        _longPressStartedWithoutFocus = true;
                        renderEditable.selectWord(cause: SelectionChangedCause.longPress);
                    }
                    else
                    {
                        if (renderEditable.readOnly)
                        {
                            renderEditable.selectWord(cause: SelectionChangedCause.longPress);
                            if (editableText.context.mounted)
                            {
                                DartRuntimePrimitives.Ignore(Feedback.forLongPress(editableText.context));
                            }
                        }
                        else
                        {
                            renderEditable.selectPositionAt(from: details.globalPosition, cause: SelectionChangedCause.longPress);
                            var cursorPoint = new global::Doroti.Framework.Services.RawFloatingCursorPoint(state: FloatingCursorDragState.Start, startLocation: (renderEditable.globalToLocal(details.globalPosition), new global::Doroti.Ui.TextPosition(offset: editableText.textEditingValue.selection.baseOffset, affinity: editableText.textEditingValue.selection.affinity)), offset: Offset.zero);
                            editableText.updateFloatingCursor(cursorPoint);
                        }
                    }
                    break;
                }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
                {
                    renderEditable.selectWord(cause: SelectionChangedCause.longPress);
                    if (editableText.context.mounted)
                    {
                        DartRuntimePrimitives.Ignore(Feedback.forLongPress(editableText.context));
                    }
                    break;
                }
        }
        _showMagnifierIfSupportedByPlatform(details.globalPosition);
        _dragStartViewportOffset = renderEditable.offset.pixels;
        _dragStartScrollOffset = _scrollPosition;
    }

    public virtual void onSingleLongTapMoveUpdate(global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails details)
    {
        if (!@delegate.selectionEnabled)
        {
            return;
        }
        var editableOffset = (renderEditable.maxLines == 1L) ? new global::Doroti.Ui.Offset(renderEditable.offset.pixels - _dragStartViewportOffset, 0.0) : new global::Doroti.Ui.Offset(0.0, renderEditable.offset.pixels - _dragStartViewportOffset);
        global::Doroti.Ui.Offset scrollableOffset = Basic_typesLibrary.axisDirectionToAxis(_scrollDirection ?? AxisDirection.left) switch { Axis.horizontal => new global::Doroti.Ui.Offset(_scrollPosition - _dragStartScrollOffset, 0.0), Axis.vertical => new global::Doroti.Ui.Offset(0.0, _scrollPosition - _dragStartScrollOffset), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
                {
                    if (_longPressStartedWithoutFocus || renderEditable.readOnly)
                    {
                        renderEditable.selectWordsInRange(from: details.globalPosition - details.offsetFromOrigin - editableOffset - scrollableOffset, to: details.globalPosition, cause: SelectionChangedCause.longPress);
                    }
                    else
                    {
                        renderEditable.selectPositionAt(from: details.globalPosition, cause: SelectionChangedCause.longPress);
                        var cursorPoint = new global::Doroti.Framework.Services.RawFloatingCursorPoint(state: FloatingCursorDragState.Update, offset: details.offsetFromOrigin);
                        editableText.updateFloatingCursor(cursorPoint);
                    }
                    break;
                }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
                {
                    renderEditable.selectWordsInRange(from: details.globalPosition - details.offsetFromOrigin - editableOffset - scrollableOffset, to: details.globalPosition, cause: SelectionChangedCause.longPress);
                    break;
                }
        }
        _showMagnifierIfSupportedByPlatform(details.globalPosition);
    }

    public virtual void onSingleLongTapEnd(global::Doroti.Framework.Gestures.LongPressEndDetails details)
    {
        _onSingleLongTapEndOrCancel();
        if (shouldShowSelectionToolbar)
        {
            editableText.showToolbar();
        }
    }

    public virtual void onSingleLongTapCancel()
    {
        _onSingleLongTapEndOrCancel();
    }

    public virtual void onSecondaryTap()
    {
        if (!@delegate.selectionEnabled)
        {
            return;
        }
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
                {
                    if (!_lastSecondaryTapWasOnSelection || !renderEditable.hasFocus)
                    {
                        renderEditable.selectWord(cause: SelectionChangedCause.tap);
                    }
                    if (shouldShowSelectionToolbar)
                    {
                        editableText.hideToolbar();
                        editableText.showToolbar();
                    }
                    break;
                }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
                {
                    if (!renderEditable.hasFocus)
                    {
                        renderEditable.selectPosition(cause: SelectionChangedCause.tap);
                    }
                    editableText.toggleToolbar();
                    break;
                }
        }
    }

    public virtual void onSecondaryTapDown(global::Doroti.Framework.Gestures.TapDownDetails details)
    {
        renderEditable.handleSecondaryTapDown(new global::Doroti.Framework.Gestures.TapDownDetails(globalPosition: details.globalPosition));
        _shouldShowSelectionToolbar = true;
        _shouldShowSelectionHandles = (details.kind is null) || Equals(details.kind, PointerDeviceKind.touch) || Equals(details.kind, PointerDeviceKind.stylus);
    }

    public virtual void onDoubleTapDown(global::Doroti.Framework.Gestures.TapDragDownDetails details)
    {
        if (@delegate.selectionEnabled)
        {
            renderEditable.selectWord(cause: SelectionChangedCause.doubleTap);
            if (shouldShowSelectionToolbar)
            {
                editableText.showToolbar();
            }
        }
    }

    internal virtual void _onSingleLongTapEndOrCancel()
    {
        _hideMagnifierIfSupportedByPlatform();
        _longPressStartedWithoutFocus = false;
        _dragStartViewportOffset = 0.0;
        _dragStartScrollOffset = 0.0;
        if (_isEditableTextMounted && Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS) && @delegate.selectionEnabled && editableText.textEditingValue.selection.isCollapsed)
        {
            var cursorPoint = new global::Doroti.Framework.Services.RawFloatingCursorPoint(state: FloatingCursorDragState.End);
            editableText.updateFloatingCursor(cursorPoint);
        }
    }

    internal virtual void _selectParagraphsInRange(Offset from, Offset? to = null, global::Doroti.Framework.Services.SelectionChangedCause? cause = null)
    {
        global::Doroti.Framework.Services.TextBoundary paragraphBoundary = new global::Doroti.Framework.Services.ParagraphBoundary(editableText.textEditingValue.text);
        _selectTextBoundariesInRange(boundary: paragraphBoundary, from: from, to: to, cause: cause);
    }

    internal virtual void _selectLinesInRange(Offset from, Offset? to = null, global::Doroti.Framework.Services.SelectionChangedCause? cause = null)
    {
        global::Doroti.Framework.Services.TextBoundary lineBoundary = new global::Doroti.Framework.Services.LineBoundary(renderEditable);
        _selectTextBoundariesInRange(boundary: lineBoundary, from: from, to: to, cause: cause);
    }

    internal virtual global::Doroti.Ui.TextRange _moveToTextBoundary(TextPosition extent, global::Doroti.Framework.Services.TextBoundary textBoundary)
    {
        DartRuntimePrimitives.Assert(() => extent.offset >= 0L);
        long startLocal = textBoundary.getLeadingTextBoundaryAt((extent.offset == editableText.textEditingValue.text.Length) ? (extent.offset - 1L) : extent.offset) ?? 0L;
        long endLocal = textBoundary.getTrailingTextBoundaryAt(extent.offset) ?? editableText.textEditingValue.text.Length;
        return new global::Doroti.Ui.TextRange(start: startLocal, end: endLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _selectTextBoundariesInRange(global::Doroti.Framework.Services.TextBoundary boundary, Offset from, Offset? to = null, global::Doroti.Framework.Services.SelectionChangedCause? cause = null)
    {
        global::Doroti.Ui.TextPosition fromPosition = renderEditable.getPositionForPoint(from);
        global::Doroti.Ui.TextRange fromRange = _moveToTextBoundary(fromPosition, boundary);
        global::Doroti.Ui.TextPosition toPosition = (to is null) ? fromPosition : renderEditable.getPositionForPoint(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(to)));
        global::Doroti.Ui.TextRange toRange = Equals(toPosition, fromPosition) ? fromRange : _moveToTextBoundary(toPosition, boundary);
        bool isFromBoundaryBeforeToBoundary = fromRange.start < toRange.end;
        var newSelection = isFromBoundaryBeforeToBoundary ? new global::Doroti.Framework.Services.TextSelection(baseOffset: fromRange.start, extentOffset: toRange.end) : new global::Doroti.Framework.Services.TextSelection(baseOffset: fromRange.end, extentOffset: toRange.start);
        editableText.userUpdateTextEditingValue(editableText.textEditingValue.copyWith(selection: newSelection), DartRuntimePrimitives.RequireValue(cause));
    }

    public virtual void onTripleTapDown(global::Doroti.Framework.Gestures.TapDragDownDetails details)
    {
        if (!@delegate.selectionEnabled)
        {
            return;
        }
        if (renderEditable.maxLines == 1L)
        {
            editableText.selectAll(SelectionChangedCause.tap);
        }
        else
        {
            switch (PlatformLibrary.defaultTargetPlatform)
            {
                case TargetPlatform.android:
                case TargetPlatform.fuchsia:
                case TargetPlatform.iOS:
                case TargetPlatform.macOS:
                case TargetPlatform.windows:
                    {
                        _selectParagraphsInRange(from: details.globalPosition, cause: SelectionChangedCause.tap);
                        break;
                    }
                case TargetPlatform.linux:
                    {
                        _selectLinesInRange(from: details.globalPosition, cause: SelectionChangedCause.tap);
                        break;
                    }
            }
        }
        if (shouldShowSelectionToolbar)
        {
            editableText.showToolbar();
        }
    }

    public virtual void onDragSelectionStart(global::Doroti.Framework.Gestures.TapDragStartDetails details)
    {
        if (!@delegate.selectionEnabled)
        {
            return;
        }
        global::Doroti.Ui.PointerDeviceKind? kindLocal = details.kind;
        _shouldShowSelectionToolbar = (kindLocal is null) || Equals(DartRuntimePrimitives.RequireValue(kindLocal), PointerDeviceKind.touch) || Equals(DartRuntimePrimitives.RequireValue(kindLocal), PointerDeviceKind.stylus);
        _shouldShowSelectionHandles = _shouldShowSelectionToolbar;
        _dragStartSelection = renderEditable.selection;
        _dragStartScrollOffset = _scrollPosition;
        _dragStartViewportOffset = renderEditable.offset.pixels;
        if (_TextSelectionGestureDetectorState__text_selection._getEffectiveConsecutiveTapCount(details.consecutiveTapCount) > 1L)
        {
            return;
        }
        if (_isShiftPressed && (renderEditable.selection is not null) && renderEditable.selection!.isValid)
        {
            switch (PlatformLibrary.defaultTargetPlatform)
            {
                case TargetPlatform.iOS:
                case TargetPlatform.macOS:
                    {
                        _expandSelection(details.globalPosition, SelectionChangedCause.drag);
                        break;
                    }
                case TargetPlatform.android:
                case TargetPlatform.fuchsia:
                case TargetPlatform.linux:
                case TargetPlatform.windows:
                    {
                        _extendSelection(details.globalPosition, SelectionChangedCause.drag);
                        break;
                    }
            }
        }
        else
        {
            switch (PlatformLibrary.defaultTargetPlatform)
            {
                case TargetPlatform.iOS:
                    {
                        switch (details.kind)
                        {
                            case PointerDeviceKind.mouse:
                            case PointerDeviceKind.trackpad:
                                {
                                    renderEditable.selectPositionAt(from: details.globalPosition, cause: SelectionChangedCause.drag);
                                    break;
                                }
                            case PointerDeviceKind.stylus:
                            case PointerDeviceKind.invertedStylus:
                            case PointerDeviceKind.touch:
                            case PointerDeviceKind.unknown:
                            case null:
                                break;
                        }
                        break;
                    }
                case TargetPlatform.android:
                case TargetPlatform.fuchsia:
                    {
                        switch (details.kind)
                        {
                            case PointerDeviceKind.mouse:
                            case PointerDeviceKind.trackpad:
                                {
                                    renderEditable.selectPositionAt(from: details.globalPosition, cause: SelectionChangedCause.drag);
                                    break;
                                }
                            case PointerDeviceKind.stylus:
                            case PointerDeviceKind.invertedStylus:
                            case PointerDeviceKind.touch:
                            case PointerDeviceKind.unknown:
                                {
                                    if (renderEditable.hasFocus)
                                    {
                                        renderEditable.selectPositionAt(from: details.globalPosition, cause: SelectionChangedCause.drag);
                                        _showMagnifierIfSupportedByPlatform(details.globalPosition);
                                    }
                                    break;
                                }
                            case null:
                                break;
                        }
                        break;
                    }
                case TargetPlatform.linux:
                case TargetPlatform.macOS:
                case TargetPlatform.windows:
                    {
                        renderEditable.selectPositionAt(from: details.globalPosition, cause: SelectionChangedCause.drag);
                        break;
                    }
            }
        }
    }

    public virtual void onDragSelectionUpdate(global::Doroti.Framework.Gestures.TapDragUpdateDetails details)
    {
        if (!@delegate.selectionEnabled)
        {
            return;
        }
        if (!_isShiftPressed)
        {
            var editableOffset = (renderEditable.maxLines == 1L) ? new global::Doroti.Ui.Offset(renderEditable.offset.pixels - _dragStartViewportOffset, 0.0) : new global::Doroti.Ui.Offset(0.0, renderEditable.offset.pixels - _dragStartViewportOffset);
            global::Doroti.Ui.Offset scrollableOffset = Basic_typesLibrary.axisDirectionToAxis(_scrollDirection ?? AxisDirection.left) switch { Axis.horizontal => new global::Doroti.Ui.Offset(_scrollPosition - _dragStartScrollOffset, 0.0), Axis.vertical => new global::Doroti.Ui.Offset(0.0, _scrollPosition - _dragStartScrollOffset), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            global::Doroti.Ui.Offset dragStartGlobalPosition = details.globalPosition - details.offsetFromOrigin;
            if (_TextSelectionGestureDetectorState__text_selection._getEffectiveConsecutiveTapCount(details.consecutiveTapCount) == 2L)
            {
                renderEditable.selectWordsInRange(from: dragStartGlobalPosition - editableOffset - scrollableOffset, to: details.globalPosition, cause: SelectionChangedCause.drag);
                switch (details.kind)
                {
                    case PointerDeviceKind.stylus:
                    case PointerDeviceKind.invertedStylus:
                    case PointerDeviceKind.touch:
                    case PointerDeviceKind.unknown:
                        {
                            _showMagnifierIfSupportedByPlatform(details.globalPosition);
                            return;
                        }
                    case PointerDeviceKind.mouse:
                    case PointerDeviceKind.trackpad:
                    case null:
                        {
                            return;
                        }
                    default:
                        throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                }
            }
            if (_TextSelectionGestureDetectorState__text_selection._getEffectiveConsecutiveTapCount(details.consecutiveTapCount) == 3L)
            {
                switch (PlatformLibrary.defaultTargetPlatform)
                {
                    case TargetPlatform.android:
                    case TargetPlatform.fuchsia:
                    case TargetPlatform.iOS:
                        {
                            switch (details.kind)
                            {
                                case PointerDeviceKind.mouse:
                                case PointerDeviceKind.trackpad:
                                    {
                                        _selectParagraphsInRange(from: dragStartGlobalPosition - editableOffset - scrollableOffset, to: details.globalPosition, cause: SelectionChangedCause.drag);
                                        return;
                                    }
                                case PointerDeviceKind.stylus:
                                case PointerDeviceKind.invertedStylus:
                                case PointerDeviceKind.touch:
                                case PointerDeviceKind.unknown:
                                case null:
                                    {
                                        break;
                                    }
                                default:
                                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                            }
                            return;
                        }
                    case TargetPlatform.linux:
                        {
                            _selectLinesInRange(from: dragStartGlobalPosition - editableOffset - scrollableOffset, to: details.globalPosition, cause: SelectionChangedCause.drag);
                            return;
                        }
                    case TargetPlatform.windows:
                    case TargetPlatform.macOS:
                        {
                            _selectParagraphsInRange(from: dragStartGlobalPosition - editableOffset - scrollableOffset, to: details.globalPosition, cause: SelectionChangedCause.drag);
                            return;
                        }
                    default:
                        throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                }
            }
            switch (PlatformLibrary.defaultTargetPlatform)
            {
                case TargetPlatform.iOS:
                    {
                        switch (details.kind)
                        {
                            case PointerDeviceKind.mouse:
                            case PointerDeviceKind.trackpad:
                                {
                                    renderEditable.selectPositionAt(from: dragStartGlobalPosition - editableOffset - scrollableOffset, to: details.globalPosition, cause: SelectionChangedCause.drag);
                                    return;
                                }
                            case PointerDeviceKind.stylus:
                            case PointerDeviceKind.invertedStylus:
                            case PointerDeviceKind.touch:
                            case PointerDeviceKind.unknown:
                            case null:
                                {
                                    break;
                                }
                            default:
                                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                        }
                        return;
                    }
                case TargetPlatform.android:
                case TargetPlatform.fuchsia:
                    {
                        switch (details.kind)
                        {
                            case PointerDeviceKind.mouse:
                            case PointerDeviceKind.trackpad:
                            case PointerDeviceKind.stylus:
                            case PointerDeviceKind.invertedStylus:
                                {
                                    renderEditable.selectPositionAt(from: dragStartGlobalPosition - editableOffset - scrollableOffset, to: details.globalPosition, cause: SelectionChangedCause.drag);
                                    return;
                                }
                            case PointerDeviceKind.touch:
                            case PointerDeviceKind.unknown:
                                {
                                    if (renderEditable.hasFocus)
                                    {
                                        renderEditable.selectPositionAt(from: details.globalPosition, cause: SelectionChangedCause.drag);
                                        _showMagnifierIfSupportedByPlatform(details.globalPosition);
                                        return;
                                    }
                                    break;
                                }
                            case null:
                                {
                                    break;
                                }
                        }
                        return;
                    }
                case TargetPlatform.macOS:
                case TargetPlatform.linux:
                case TargetPlatform.windows:
                    {
                        renderEditable.selectPositionAt(from: dragStartGlobalPosition - editableOffset - scrollableOffset, to: details.globalPosition, cause: SelectionChangedCause.drag);
                        return;
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
        if (_dragStartSelection!.isCollapsed || (!Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS)) && (!Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.macOS)))
        {
            _extendSelection(details.globalPosition, SelectionChangedCause.drag);
            return;
        }
        global::Doroti.Framework.Services.TextSelection selectionLocal = editableText.textEditingValue.selection;
        global::Doroti.Ui.TextPosition nextExtent = renderEditable.getPositionForPoint(details.globalPosition);
        bool isShiftTapDragSelectionForward = _dragStartSelection!.baseOffset < _dragStartSelection!.extentOffset;
        bool isInverted = isShiftTapDragSelectionForward ? (nextExtent.offset < _dragStartSelection!.baseOffset) : (nextExtent.offset > _dragStartSelection!.baseOffset);
        if (isInverted && (selectionLocal.baseOffset == _dragStartSelection!.baseOffset))
        {
            editableText.userUpdateTextEditingValue(editableText.textEditingValue.copyWith(selection: new global::Doroti.Framework.Services.TextSelection(baseOffset: _dragStartSelection!.extentOffset, extentOffset: nextExtent.offset)), SelectionChangedCause.drag);
        }
        else
        {
            if (!isInverted && (nextExtent.offset != _dragStartSelection!.baseOffset) && (selectionLocal.baseOffset != _dragStartSelection!.baseOffset))
            {
                editableText.userUpdateTextEditingValue(editableText.textEditingValue.copyWith(selection: new global::Doroti.Framework.Services.TextSelection(baseOffset: _dragStartSelection!.baseOffset, extentOffset: nextExtent.offset)), SelectionChangedCause.drag);
            }
            else
            {
                _extendSelection(details.globalPosition, SelectionChangedCause.drag);
            }
        }
    }

    public virtual void onDragSelectionEnd(global::Doroti.Framework.Gestures.TapDragEndDetails details)
    {
        if (_shouldShowSelectionToolbar && (_TextSelectionGestureDetectorState__text_selection._getEffectiveConsecutiveTapCount(details.consecutiveTapCount) == 2L))
        {
            editableText.showToolbar();
        }
        if (_isShiftPressed)
        {
            _dragStartSelection = null;
        }
        _hideMagnifierIfSupportedByPlatform();
    }

    public virtual Widget buildGestureDetector(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Rendering.HitTestBehavior? behavior = null, Widget child = default!)
    {
        return new TextSelectionGestureDetector(key: key, onTapTrackStart: () => onTapTrackStart(), onTapTrackReset: () => onTapTrackReset(), onTapDown: onTapDown, onForcePressStart: @delegate.forcePressEnabled ? onForcePressStart : null, onForcePressEnd: @delegate.forcePressEnabled ? onForcePressEnd : null, onSecondaryTap: () => onSecondaryTap(), onSecondaryTapDown: onSecondaryTapDown, onSingleTapUp: onSingleTapUp, onSingleTapCancel: () => onSingleTapCancel(), onUserTap: () => onUserTap(), onSingleLongTapStart: onSingleLongTapStart, onSingleLongTapMoveUpdate: onSingleLongTapMoveUpdate, onSingleLongTapEnd: onSingleLongTapEnd, onSingleLongTapCancel: () => onSingleLongTapCancel(), onDoubleTapDown: onDoubleTapDown, onTripleTapDown: onTripleTapDown, onDragSelectionStart: onDragSelectionStart, onDragSelectionUpdate: onDragSelectionUpdate, onDragSelectionEnd: onDragSelectionEnd, onUserTapAlwaysCalled: onUserTapAlwaysCalled, behavior: behavior, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class TextSelectionGestureDetector : StatefulWidget
{
    public virtual global::System.Action? onTapTrackStart { get; private set; }
    public virtual global::System.Action? onTapTrackReset { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.TapDragDownDetails>? onTapDown { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.ForcePressDetails>? onForcePressStart { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.ForcePressDetails>? onForcePressEnd { get; private set; }
    public virtual global::System.Action? onSecondaryTap { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>? onSecondaryTapDown { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.TapDragUpDetails>? onSingleTapUp { get; private set; }
    public virtual global::System.Action? onSingleTapCancel { get; private set; }
    public virtual global::System.Action? onUserTap { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.LongPressStartDetails>? onSingleLongTapStart { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails>? onSingleLongTapMoveUpdate { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.LongPressEndDetails>? onSingleLongTapEnd { get; private set; }
    public virtual global::System.Action? onSingleLongTapCancel { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.TapDragDownDetails>? onDoubleTapDown { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.TapDragDownDetails>? onTripleTapDown { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.TapDragStartDetails>? onDragSelectionStart { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.TapDragUpdateDetails>? onDragSelectionUpdate { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.TapDragEndDetails>? onDragSelectionEnd { get; private set; }
    public virtual bool onUserTapAlwaysCalled { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.HitTestBehavior? behavior { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public TextSelectionGestureDetector(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action? onTapTrackStart = null, global::System.Action? onTapTrackReset = null, global::System.Action<global::Doroti.Framework.Gestures.TapDragDownDetails>? onTapDown = null, global::System.Action<global::Doroti.Framework.Gestures.ForcePressDetails>? onForcePressStart = null, global::System.Action<global::Doroti.Framework.Gestures.ForcePressDetails>? onForcePressEnd = null, global::System.Action? onSecondaryTap = null, global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>? onSecondaryTapDown = null, global::System.Action<global::Doroti.Framework.Gestures.TapDragUpDetails>? onSingleTapUp = null, global::System.Action? onSingleTapCancel = null, global::System.Action? onUserTap = null, global::System.Action<global::Doroti.Framework.Gestures.LongPressStartDetails>? onSingleLongTapStart = null, global::System.Action<global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails>? onSingleLongTapMoveUpdate = null, global::System.Action<global::Doroti.Framework.Gestures.LongPressEndDetails>? onSingleLongTapEnd = null, global::System.Action? onSingleLongTapCancel = null, global::System.Action<global::Doroti.Framework.Gestures.TapDragDownDetails>? onDoubleTapDown = null, global::System.Action<global::Doroti.Framework.Gestures.TapDragDownDetails>? onTripleTapDown = null, global::System.Action<global::Doroti.Framework.Gestures.TapDragStartDetails>? onDragSelectionStart = null, global::System.Action<global::Doroti.Framework.Gestures.TapDragUpdateDetails>? onDragSelectionUpdate = null, global::System.Action<global::Doroti.Framework.Gestures.TapDragEndDetails>? onDragSelectionEnd = null, bool onUserTapAlwaysCalled = false, global::Doroti.Framework.Rendering.HitTestBehavior? behavior = null, Widget child = default!) : base(key: key)
    {
        this.onTapTrackStart = onTapTrackStart;
        this.onTapTrackReset = onTapTrackReset;
        this.onTapDown = onTapDown;
        this.onForcePressStart = onForcePressStart;
        this.onForcePressEnd = onForcePressEnd;
        this.onSecondaryTap = onSecondaryTap;
        this.onSecondaryTapDown = onSecondaryTapDown;
        this.onSingleTapUp = onSingleTapUp;
        this.onSingleTapCancel = onSingleTapCancel;
        this.onUserTap = onUserTap;
        this.onSingleLongTapStart = onSingleLongTapStart;
        this.onSingleLongTapMoveUpdate = onSingleLongTapMoveUpdate;
        this.onSingleLongTapEnd = onSingleLongTapEnd;
        this.onSingleLongTapCancel = onSingleLongTapCancel;
        this.onDoubleTapDown = onDoubleTapDown;
        this.onTripleTapDown = onTripleTapDown;
        this.onDragSelectionStart = onDragSelectionStart;
        this.onDragSelectionUpdate = onDragSelectionUpdate;
        this.onDragSelectionEnd = onDragSelectionEnd;
        this.onUserTapAlwaysCalled = onUserTapAlwaysCalled;
        this.behavior = behavior;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _TextSelectionGestureDetectorState__text_selection());
}

internal class _TextSelectionGestureDetectorState__text_selection : State<TextSelectionGestureDetector>
{
    internal static long _getEffectiveConsecutiveTapCount(long rawCount)
    {
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
                {
                    return (rawCount <= 3L) ? rawCount : (((rawCount % 3L) == 0L) ? 3L : (rawCount % 3L));
                }
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
                {
                    return Math.Min(rawCount, 3L);
                }
            case TargetPlatform.windows:
                {
                    return (rawCount < 2L) ? rawCount : (2L + (rawCount % 2L));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleTapTrackStart()
    {
        widget.onTapTrackStart?.Invoke();
    }

    internal virtual void _handleTapTrackReset()
    {
        widget.onTapTrackReset?.Invoke();
    }

    internal virtual void _handleTapDown(global::Doroti.Framework.Gestures.TapDragDownDetails details)
    {
        widget.onTapDown?.Invoke(details);
        if (_getEffectiveConsecutiveTapCount(details.consecutiveTapCount) == 2L)
        {
            widget.onDoubleTapDown?.Invoke(details);
            return;
        }
        if (_getEffectiveConsecutiveTapCount(details.consecutiveTapCount) == 3L)
        {
            widget.onTripleTapDown?.Invoke(details);
            return;
        }
    }

    internal virtual void _handleTapUp(global::Doroti.Framework.Gestures.TapDragUpDetails details)
    {
        if (_getEffectiveConsecutiveTapCount(details.consecutiveTapCount) == 1L)
        {
            widget.onSingleTapUp?.Invoke(details);
            widget.onUserTap?.Invoke();
        }
        else
        {
            if (widget.onUserTapAlwaysCalled)
            {
                widget.onUserTap?.Invoke();
            }
        }
    }

    internal virtual void _handleTapCancel()
    {
        widget.onSingleTapCancel?.Invoke();
    }

    internal virtual void _handleDragStart(global::Doroti.Framework.Gestures.TapDragStartDetails details)
    {
        widget.onDragSelectionStart?.Invoke(details);
    }

    internal virtual void _handleDragUpdate(global::Doroti.Framework.Gestures.TapDragUpdateDetails details)
    {
        widget.onDragSelectionUpdate?.Invoke(details);
    }

    internal virtual void _handleDragEnd(global::Doroti.Framework.Gestures.TapDragEndDetails details)
    {
        widget.onDragSelectionEnd?.Invoke(details);
    }

    internal virtual void _forcePressStarted(global::Doroti.Framework.Gestures.ForcePressDetails details)
    {
        widget.onForcePressStart?.Invoke(details);
    }

    internal virtual void _forcePressEnded(global::Doroti.Framework.Gestures.ForcePressDetails details)
    {
        widget.onForcePressEnd?.Invoke(details);
    }

    internal virtual void _handleLongPressStart(global::Doroti.Framework.Gestures.LongPressStartDetails details)
    {
        widget.onSingleLongTapStart?.Invoke(details);
    }

    internal virtual void _handleLongPressMoveUpdate(global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails details)
    {
        widget.onSingleLongTapMoveUpdate?.Invoke(details);
    }

    internal virtual void _handleLongPressEnd(global::Doroti.Framework.Gestures.LongPressEndDetails details)
    {
        widget.onSingleLongTapEnd?.Invoke(details);
    }

    internal virtual void _handleLongPressCancel()
    {
        widget.onSingleLongTapCancel?.Invoke();
    }

    public override Widget build(BuildContext context)
    {
        var gesturesLocal = new DartMap<Type, dynamic>();
        gesturesLocal[typeof(global::Doroti.Framework.Gestures.TapGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.TapGestureRecognizer>(() => new global::Doroti.Framework.Gestures.TapGestureRecognizer(debugOwner: this), (instance) =>
        {
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.TapGestureRecognizer>)(() =>
            {
                var __cascade = instance;
                __cascade.onSecondaryTap = widget.onSecondaryTap;
                __cascade.onSecondaryTapDown = widget.onSecondaryTapDown;
                return __cascade;
            }))());
        });
        if ((widget.onSingleLongTapStart is not null) || (widget.onSingleLongTapMoveUpdate is not null) || (widget.onSingleLongTapEnd is not null) || (widget.onSingleLongTapCancel is not null))
        {
            gesturesLocal[typeof(global::Doroti.Framework.Gestures.LongPressGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.LongPressGestureRecognizer>(() => new global::Doroti.Framework.Gestures.LongPressGestureRecognizer(debugOwner: this, supportedDevices: new HashSet<PointerDeviceKind> { PointerDeviceKind.touch }), (instance) =>
            {
                DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.LongPressGestureRecognizer>)(() =>
                {
                    var __cascade = instance;
                    __cascade.onLongPressStart = _handleLongPressStart;
                    __cascade.onLongPressMoveUpdate = _handleLongPressMoveUpdate;
                    __cascade.onLongPressEnd = _handleLongPressEnd;
                    __cascade.onLongPressCancel = _handleLongPressCancel;
                    return __cascade;
                }))());
            });
        }
        if ((widget.onDragSelectionStart is not null) || (widget.onDragSelectionUpdate is not null) || (widget.onDragSelectionEnd is not null))
        {
            switch (PlatformLibrary.defaultTargetPlatform)
            {
                case TargetPlatform.android:
                case TargetPlatform.fuchsia:
                case TargetPlatform.iOS:
                    {
                        gesturesLocal[typeof(global::Doroti.Framework.Gestures.TapAndHorizontalDragGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.TapAndHorizontalDragGestureRecognizer>(() => new global::Doroti.Framework.Gestures.TapAndHorizontalDragGestureRecognizer(debugOwner: this), (instance) =>
                        {
                            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.TapAndHorizontalDragGestureRecognizer>)(() =>
                            {
                                var __cascade = instance;
                                __cascade.dragStartBehavior = DragStartBehavior.down;
                                __cascade.eagerVictoryOnDrag = !Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS);
                                __cascade.onTapTrackStart = _handleTapTrackStart;
                                __cascade.onTapTrackReset = _handleTapTrackReset;
                                __cascade.onTapDown = _handleTapDown;
                                __cascade.onDragStart = _handleDragStart;
                                __cascade.onDragUpdate = _handleDragUpdate;
                                __cascade.onDragEnd = _handleDragEnd;
                                __cascade.onTapUp = _handleTapUp;
                                __cascade.onCancel = _handleTapCancel;
                                return __cascade;
                            }))());
                        });
                        break;
                    }
                case TargetPlatform.linux:
                case TargetPlatform.macOS:
                case TargetPlatform.windows:
                    {
                        gesturesLocal[typeof(global::Doroti.Framework.Gestures.TapAndPanGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.TapAndPanGestureRecognizer>(() => new global::Doroti.Framework.Gestures.TapAndPanGestureRecognizer(debugOwner: this), (instance) =>
                        {
                            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.TapAndPanGestureRecognizer>)(() =>
                            {
                                var __cascade = instance;
                                __cascade.dragStartBehavior = DragStartBehavior.down;
                                __cascade.onTapTrackStart = _handleTapTrackStart;
                                __cascade.onTapTrackReset = _handleTapTrackReset;
                                __cascade.onTapDown = _handleTapDown;
                                __cascade.onDragStart = _handleDragStart;
                                __cascade.onDragUpdate = _handleDragUpdate;
                                __cascade.onDragEnd = _handleDragEnd;
                                __cascade.onTapUp = _handleTapUp;
                                __cascade.onCancel = _handleTapCancel;
                                return __cascade;
                            }))());
                        });
                        break;
                    }
            }
        }
        if ((widget.onForcePressStart is not null) || (widget.onForcePressEnd is not null))
        {
            gesturesLocal[typeof(global::Doroti.Framework.Gestures.ForcePressGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.ForcePressGestureRecognizer>(() => new global::Doroti.Framework.Gestures.ForcePressGestureRecognizer(debugOwner: this), (instance) =>
            {
                DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.ForcePressGestureRecognizer>)(() =>
                {
                    var __cascade = instance;
                    __cascade.onStart = (widget.onForcePressStart is not null) ? _forcePressStarted : null;
                    __cascade.onEnd = (widget.onForcePressEnd is not null) ? _forcePressEnded : null;
                    return __cascade;
                }))());
            });
        }
        return new RawGestureDetector(gestures: gesturesLocal, excludeFromSemantics: true, behavior: widget.behavior, child: widget.child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ClipboardStatusNotifier : global::Doroti.Framework.Foundation.ValueNotifier<ClipboardStatus>, WidgetsBindingObserver
{
    internal virtual bool _disposed { get; set; } = false;

    public ClipboardStatusNotifier(ClipboardStatus value = ClipboardStatus.unknown) : base(value)
    {
    }

    public async virtual Future update()
    {
        if (_disposed)
        {
            return;
        }
        bool hasStringsLocal = default!;
        try
        {
            hasStringsLocal = await Clipboard.hasStrings();
        }
        catch (Exception exceptionLocal)
        {
            var stackLocal = new System.Diagnostics.StackTrace();
            FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exceptionLocal, stack: stackLocal, library: "widget library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while checking if the clipboard has strings")));
            if (_disposed)
            {
                return;
            }
            value = ClipboardStatus.unknown;
            return;
        }
        ClipboardStatus nextStatus = hasStringsLocal ? ClipboardStatus.pasteable : ClipboardStatus.notPasteable;
        if (_disposed)
        {
            return;
        }
        value = nextStatus;
    }

    public override void addListener(global::System.Action listener)
    {
        if (!hasListeners)
        {
            WidgetsBinding.instance.addObserver(this);
        }
        if (Equals(value, ClipboardStatus.unknown))
        {
            DartRuntimePrimitives.Ignore(update());
        }
        base.addListener(listener);
    }

    public override void removeListener(global::System.Action listener)
    {
        base.removeListener(listener);
        if (!_disposed && !hasListeners)
        {
            WidgetsBinding.instance.removeObserver(this);
        }
    }

    public virtual void didChangeAppLifecycleState(AppLifecycleState state)
    {
        switch (state)
        {
            case var __constant148294 when Equals(__constant148294, AppLifecycleState.resumed):
                {
                    DartRuntimePrimitives.Ignore(update());
                    break;
                }
            case var __constant148350 when Equals(__constant148350, AppLifecycleState.detached):
            case var __constant148389 when Equals(__constant148389, AppLifecycleState.inactive):
            case var __constant148428 when Equals(__constant148428, AppLifecycleState.hidden):
            case var __constant148465 when Equals(__constant148465, AppLifecycleState.paused):
                {
                    break;
                }
        }
    }

    public override void dispose()
    {
        WidgetsBinding.instance.removeObserver(this);
        _disposed = true;
        base.dispose();
    }

}

public enum ClipboardStatus
{
    pasteable,
    unknown,
    notPasteable
}

public class LiveTextInputStatusNotifier : global::Doroti.Framework.Foundation.ValueNotifier<LiveTextInputStatus>, WidgetsBindingObserver
{
    internal virtual bool _disposed { get; set; } = false;

    public LiveTextInputStatusNotifier(LiveTextInputStatus value = LiveTextInputStatus.unknown) : base(value)
    {
    }

    public async virtual Future update()
    {
        if (_disposed)
        {
            return;
        }
        bool isLiveTextInputEnabled = default!;
        try
        {
            isLiveTextInputEnabled = await LiveText.isLiveTextInputAvailable();
        }
        catch (Exception exceptionLocal)
        {
            var stackLocal = new System.Diagnostics.StackTrace();
            FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exceptionLocal, stack: stackLocal, library: "widget library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while checking the availability of Live Text input")));
            if (_disposed || Equals(value, LiveTextInputStatus.unknown))
            {
                return;
            }
            value = LiveTextInputStatus.unknown;
            return;
        }
        LiveTextInputStatus nextStatus = isLiveTextInputEnabled ? LiveTextInputStatus.enabled : LiveTextInputStatus.disabled;
        if (_disposed || Equals(nextStatus, value))
        {
            return;
        }
        value = nextStatus;
    }

    public override void addListener(global::System.Action listener)
    {
        if (!hasListeners)
        {
            WidgetsBinding.instance.addObserver(this);
        }
        if (Equals(value, LiveTextInputStatus.unknown))
        {
            DartRuntimePrimitives.Ignore(update());
        }
        base.addListener(listener);
    }

    public override void removeListener(global::System.Action listener)
    {
        base.removeListener(listener);
        if (!_disposed && !hasListeners)
        {
            WidgetsBinding.instance.removeObserver(this);
        }
    }

    public virtual void didChangeAppLifecycleState(AppLifecycleState state)
    {
        switch (state)
        {
            case var __constant151548 when Equals(__constant151548, AppLifecycleState.resumed):
                {
                    DartRuntimePrimitives.Ignore(update());
                    break;
                }
            case var __constant151604 when Equals(__constant151604, AppLifecycleState.detached):
            case var __constant151643 when Equals(__constant151643, AppLifecycleState.inactive):
            case var __constant151682 when Equals(__constant151682, AppLifecycleState.paused):
            case var __constant151719 when Equals(__constant151719, AppLifecycleState.hidden):
                break;
        }
    }

    public override void dispose()
    {
        WidgetsBinding.instance.removeObserver(this);
        _disposed = true;
        base.dispose();
    }

}

public enum LiveTextInputStatus
{
    enabled,
    unknown,
    disabled
}

// Dart models this as a member-less mixin applied after a concrete selection
// controls superclass. A CLR interface preserves that marker relationship so
// EditableText can select the contextMenuBuilder path without multiple
// inheritance.
public interface TextSelectionHandleControls
{
}
