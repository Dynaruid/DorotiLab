// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/text_selection.dart
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

public delegate Widget ToolbarBuilder(BuildContext context, Widget child);

public class ToolbarItemsParentData : global::Doroti.Framework.Rendering.ContainerBoxParentData<global::Doroti.Framework.Rendering.RenderBox>
{
    public virtual bool shouldPaint { get; set; } = false;

    public override string ToString() => $"{base.ToString()}; shouldPaint={this.shouldPaint}";
}

public abstract class TextSelectionControls
{
    public abstract Widget buildHandle(BuildContext context, global::Doroti.Framework.Rendering.TextSelectionHandleType type, double textLineHeight, global::System.Action? onTap = null);
    public abstract global::Doroti.Ui.Offset getHandleAnchor(global::Doroti.Framework.Rendering.TextSelectionHandleType type, double textLineHeight);
    public abstract Widget buildToolbar(BuildContext context, Rect globalEditableRegion, double textLineHeight, Offset selectionMidpoint, List<global::Doroti.Framework.Rendering.TextSelectionPoint> endpoints, global::Doroti.Framework.Services.TextSelectionDelegate @delegate, global::Doroti.Framework.Foundation.ValueListenable<ClipboardStatus>? clipboardStatus, Offset? lastSecondaryTapDownPosition);
    public abstract global::Doroti.Ui.Size getHandleSize(double textLineHeight);
    public virtual bool canCut(global::Doroti.Framework.Services.TextSelectionDelegate @delegate)
    {
        return (((global::Doroti.Framework.Services.TextSelectionDelegate)@delegate).cutEnabled && !((global::Doroti.Framework.Services.TextSelectionDelegate)@delegate).textEditingValue.selection.isCollapsed);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool canCopy(global::Doroti.Framework.Services.TextSelectionDelegate @delegate)
    {
        return (((global::Doroti.Framework.Services.TextSelectionDelegate)@delegate).copyEnabled && !((global::Doroti.Framework.Services.TextSelectionDelegate)@delegate).textEditingValue.selection.isCollapsed);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool canPaste(global::Doroti.Framework.Services.TextSelectionDelegate @delegate)
    {
        return ((global::Doroti.Framework.Services.TextSelectionDelegate)@delegate).pasteEnabled;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool canSelectAll(global::Doroti.Framework.Services.TextSelectionDelegate @delegate)
    {
        return ((((global::Doroti.Framework.Services.TextSelectionDelegate)@delegate).selectAllEnabled && (((global::Doroti.Framework.Services.TextSelectionDelegate)@delegate).textEditingValue.text.Length != 0)) && ((global::Doroti.Framework.Services.TextSelectionDelegate)@delegate).textEditingValue.selection.isCollapsed);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleCut(global::Doroti.Framework.Services.TextSelectionDelegate @delegate)
    {
        @delegate.cutSelection(global::Doroti.Framework.Services.SelectionChangedCause.toolbar);
    }

    public virtual void handleCopy(global::Doroti.Framework.Services.TextSelectionDelegate @delegate)
    {
        @delegate.copySelection(global::Doroti.Framework.Services.SelectionChangedCause.toolbar);
    }

    public async virtual Future handlePaste(global::Doroti.Framework.Services.TextSelectionDelegate @delegate)
    {
        await @delegate.pasteText(global::Doroti.Framework.Services.SelectionChangedCause.toolbar);
    }

    public virtual void handleSelectAll(global::Doroti.Framework.Services.TextSelectionDelegate @delegate)
    {
        @delegate.selectAll(global::Doroti.Framework.Services.SelectionChangedCause.toolbar);
    }

}

public class EmptyTextSelectionControls : TextSelectionControls
{
    public override Size getHandleSize(double textLineHeight) => Size.zero;
    public override Widget buildToolbar(BuildContext context, Rect globalEditableRegion, double textLineHeight, Offset selectionMidpoint, List<global::Doroti.Framework.Rendering.TextSelectionPoint> endpoints, global::Doroti.Framework.Services.TextSelectionDelegate @delegate, global::Doroti.Framework.Foundation.ValueListenable<ClipboardStatus>? clipboardStatus, Offset? lastSecondaryTapDownPosition) => DartRuntimePrimitives.ConvertValue<Widget>(SizedBox.CreateShrink());
    public override Widget buildHandle(BuildContext context, global::Doroti.Framework.Rendering.TextSelectionHandleType type, double textLineHeight, global::System.Action? onTap = null)
    {
        return ((Widget)(object?)SizedBox.CreateShrink());
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
    public static TextSelectionControls emptyTextSelectionControls = ((TextSelectionControls)(object?)new EmptyTextSelectionControls());
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

    public TextSelectionOverlay(global::Doroti.Framework.Services.TextEditingValue value, BuildContext context, Widget? debugRequiredFor = null, global::Doroti.Framework.Rendering.LayerLink toolbarLayerLink = default!, global::Doroti.Framework.Rendering.LayerLink startHandleLayerLink = default!, global::Doroti.Framework.Rendering.LayerLink endHandleLayerLink = default!, global::Doroti.Framework.Rendering.RenderEditable renderObject = default!, TextSelectionControls? selectionControls = null, bool handlesVisible = false, global::Doroti.Framework.Services.TextSelectionDelegate selectionDelegate = default!, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, global::System.Action? onSelectionHandleTapped = null, ClipboardStatusNotifier? clipboardStatus = null, global::System.Func<BuildContext, Widget>? contextMenuBuilder = null, TextMagnifierConfiguration magnifierConfiguration = default!)
    {
        this.context = context;
        this.renderObject = renderObject;
        this.selectionControls = selectionControls;
        this.selectionDelegate = selectionDelegate;
        this.contextMenuBuilder = contextMenuBuilder;
        this._handlesVisible = handlesVisible;
        this._value = value;
        this.renderObject.selectionStartInViewport.addListener(this._updateTextSelectionOverlayVisibilities);
        this.renderObject.selectionEndInViewport.addListener(this._updateTextSelectionOverlayVisibilities);
        this._updateTextSelectionOverlayVisibilities();
        this._selectionOverlay = new SelectionOverlay(
            magnifierConfiguration: magnifierConfiguration, context: context, debugRequiredFor: debugRequiredFor,
            startHandleType: global::Doroti.Framework.Rendering.TextSelectionHandleType.collapsed,
            startHandlesVisible: this._effectiveStartHandleVisibility, lineHeightAtStart: 0.0,
            onStartHandleDragStart: this._handleSelectionStartHandleDragStart, onStartHandleDragUpdate: this._handleSelectionStartHandleDragUpdate,
            onStartHandleDragEnd: this._handleAnyDragEnd, endHandleType: global::Doroti.Framework.Rendering.TextSelectionHandleType.collapsed,
            endHandlesVisible: this._effectiveEndHandleVisibility, lineHeightAtEnd: 0.0,
            onEndHandleDragStart: this._handleSelectionEndHandleDragStart, onEndHandleDragUpdate: this._handleSelectionEndHandleDragUpdate,
            onEndHandleDragEnd: this._handleAnyDragEnd, toolbarVisible: this._effectiveToolbarVisibility,
            selectionEndpoints: new List<global::Doroti.Framework.Rendering.TextSelectionPoint>(), selectionControls: selectionControls,
            selectionDelegate: selectionDelegate, clipboardStatus: clipboardStatus, startHandleLayerLink: startHandleLayerLink,
            endHandleLayerLink: endHandleLayerLink, toolbarLayerLink: toolbarLayerLink, onSelectionHandleTapped: onSelectionHandleTapped,
            dragStartBehavior: dragStartBehavior, toolbarLocation: renderObject.lastSecondaryTapDownPosition);
    }

    public virtual global::Doroti.Framework.Services.TextEditingValue value => this._value;
    internal virtual global::Doroti.Framework.Services.TextSelection _selection => ((global::Doroti.Framework.Services.TextEditingValue)this._value).selection;
    internal virtual void _updateTextSelectionOverlayVisibilities()
    {
        this._effectiveStartHandleVisibility.value = (this._handlesVisible && ((global::Doroti.Framework.Rendering.RenderEditable)this.renderObject).selectionStartInViewport.value);
        this._effectiveEndHandleVisibility.value = (this._handlesVisible && ((global::Doroti.Framework.Rendering.RenderEditable)this.renderObject).selectionEndInViewport.value);
        this._effectiveToolbarVisibility.value = (((global::Doroti.Framework.Rendering.RenderEditable)this.renderObject).selectionStartInViewport.value || ((global::Doroti.Framework.Rendering.RenderEditable)this.renderObject).selectionEndInViewport.value);
    }

    public virtual bool handlesVisible
    {
        get => this._handlesVisible;
        set
        {
            var visible = value;
            if ((this._handlesVisible == visible))
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
        this._selectionOverlay.showHandles();
    }

    public virtual void hideHandles() => this._selectionOverlay.hideHandles();
    public virtual void showToolbar()
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Framework.Scheduler.SchedulerPhase.persistentCallbacks)), () => (object?)"showToolbar must not be called during the build or layout phase.");
        _updateSelectionOverlay();
        if (((this.selectionControls is not null) && (this.selectionControls is not TextSelectionHandleControls)))
        {
            this._selectionOverlay.showToolbar();
            return;
        }
        if ((this.contextMenuBuilder is null))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => ((BuildContext)this.context).mounted);
        this._selectionOverlay.showToolbar(context: this.context, contextMenuBuilder: (global::System.Func<BuildContext, Widget>?)this.contextMenuBuilder);
        return;
    }

    public virtual void showSpellCheckSuggestionsToolbar(global::System.Func<BuildContext, Widget> spellCheckSuggestionsToolbarBuilder)
    {
        _updateSelectionOverlay();
        DartRuntimePrimitives.Assert(() => ((BuildContext)this.context).mounted);
        this._selectionOverlay.showSpellCheckSuggestionsToolbar(context: this.context, builder: (global::System.Func<BuildContext, Widget>)spellCheckSuggestionsToolbarBuilder);
        hideHandles();
    }

    public virtual void showMagnifier(Offset positionToShow)
    {
        global::Doroti.Ui.TextPosition position = ((global::Doroti.Ui.TextPosition)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.TextPosition>(((TextPosition)((dynamic)this.renderObject).getPositionForPoint(positionToShow))));
        _updateSelectionOverlay();
        this._selectionOverlay.showMagnifier(_buildMagnifier(currentTextPosition: position, globalGesturePosition: positionToShow, renderEditable: this.renderObject));
    }

    public virtual void updateMagnifier(Offset positionToShow)
    {
        global::Doroti.Ui.TextPosition position = ((global::Doroti.Ui.TextPosition)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.TextPosition>(((TextPosition)((dynamic)this.renderObject).getPositionForPoint(positionToShow))));
        _updateSelectionOverlay();
        this._selectionOverlay.updateMagnifier(_buildMagnifier(currentTextPosition: position, globalGesturePosition: positionToShow, renderEditable: this.renderObject));
    }

    public virtual void hideMagnifier()
    {
        this._selectionOverlay.hideMagnifier();
    }

    public virtual void update(global::Doroti.Framework.Services.TextEditingValue newValue)
    {
        if ((object.Equals(this._value, newValue)))
        {
            return;
        }
        _value = newValue;
        _updateSelectionOverlay();
        this._selectionOverlay.markNeedsBuild();
    }

    internal virtual void _updateSelectionOverlay()
    {
        List<global::Doroti.Framework.Rendering.TextSelectionPoint> endpoints = ((List<global::Doroti.Framework.Rendering.TextSelectionPoint>)(object?)DartRuntimePrimitives.ConvertValue<List<global::Doroti.Framework.Rendering.TextSelectionPoint>>(((List<global::Doroti.Framework.Rendering.TextSelectionPoint>)((dynamic)this.renderObject).getEndpointsForSelection(this._selection))));
        DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(endpoints));
        global::Doroti.Framework.Rendering.TextSelectionHandleType startHandleTypeLocal = default!;
        global::Doroti.Framework.Rendering.TextSelectionHandleType endHandleTypeLocal = default!;
        if (this._selection.isCollapsed)
        {
            startHandleTypeLocal = global::Doroti.Framework.Rendering.TextSelectionHandleType.collapsed;
            endHandleTypeLocal = global::Doroti.Framework.Rendering.TextSelectionHandleType.collapsed;
        }
        else
        {
            global::Doroti.Ui.TextDirection textDirectionLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.TextDirection>(((global::Doroti.Framework.Rendering.RenderEditable)this.renderObject).textDirection);
            var preferRenderObjectDirectionForSelectionHandles = (object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS));
            global::Doroti.Ui.TextDirection startHandleDirection = default!;
            global::Doroti.Ui.TextDirection endHandleDirection = default!;
            if ((preferRenderObjectDirectionForSelectionHandles || (checked((long)(endpoints.Count)) < 2L)))
            {
                startHandleDirection = textDirectionLocal;
                endHandleDirection = textDirectionLocal;
            }
            else
            {
                startHandleDirection = (endpoints.First().direction ?? textDirectionLocal);
                endHandleDirection = (endpoints.Last().direction ?? textDirectionLocal);
            }
            startHandleTypeLocal = (startHandleDirection switch { TextDirection.ltr => global::Doroti.Framework.Rendering.TextSelectionHandleType.left, TextDirection.rtl => global::Doroti.Framework.Rendering.TextSelectionHandleType.right, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            endHandleTypeLocal = (endHandleDirection switch { TextDirection.ltr => global::Doroti.Framework.Rendering.TextSelectionHandleType.right, TextDirection.rtl => global::Doroti.Framework.Rendering.TextSelectionHandleType.left, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        }
        DartRuntimePrimitives.Ignore(((Func<SelectionOverlay>)(() =>
{
    var __cascade = this._selectionOverlay;
    __cascade.startHandleType = startHandleTypeLocal;
    __cascade.lineHeightAtStart = _getStartGlyphHeight();
    __cascade.endHandleType = endHandleTypeLocal;
    __cascade.lineHeightAtEnd = _getEndGlyphHeight();
    __cascade.selectionEndpoints = endpoints;
    __cascade.toolbarLocation = ((global::Doroti.Framework.Rendering.RenderEditable)this.renderObject).lastSecondaryTapDownPosition;
    return __cascade;
}))());
    }

    public virtual void updateForScroll()
    {
        _updateSelectionOverlay();
        this._selectionOverlay.markNeedsBuild();
    }

    public virtual bool handlesAreVisible => DartRuntimePrimitives.ConvertValue<bool>(((((SelectionOverlay)this._selectionOverlay)._handles is not null) && this.handlesVisible));
    public virtual bool toolbarIsVisible => ((SelectionOverlay)this._selectionOverlay).toolbarIsVisible;
    public virtual bool magnifierIsVisible => ((SelectionOverlay)this._selectionOverlay).magnifierIsVisible;
    public virtual bool magnifierExists => ((SelectionOverlay)this._selectionOverlay).magnifierExists;
    public virtual bool spellCheckToolbarIsVisible => ((SelectionOverlay)this._selectionOverlay)._spellCheckToolbarController.isShown;
    public virtual void hide() => this._selectionOverlay.hide();
    public virtual void hideToolbar() => this._selectionOverlay.hideToolbar();
    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this._selectionOverlay.dispose();
        ((global::Doroti.Framework.Rendering.RenderEditable)this.renderObject).selectionStartInViewport.removeListener(() => this._updateTextSelectionOverlayVisibilities());
        ((global::Doroti.Framework.Rendering.RenderEditable)this.renderObject).selectionEndInViewport.removeListener(() => this._updateTextSelectionOverlayVisibilities());
        this._effectiveToolbarVisibility.dispose();
        this._effectiveStartHandleVisibility.dispose();
        this._effectiveEndHandleVisibility.dispose();
        hideToolbar();
    }

    internal virtual double _getStartGlyphHeight()
    {
        string currText = ((global::Doroti.Framework.Services.TextSelectionDelegate)this.selectionDelegate).textEditingValue.text;
        long firstSelectedGraphemeExtent = default!;
        global::Doroti.Ui.Rect? startHandleRect = default!;
        if ((((((global::Doroti.Framework.Rendering.RenderEditable)this.renderObject).plainText == currText) && this._selection.isValid) && !this._selection.isCollapsed))
        {
            string selectedGraphemes = this._selection.textInside(currText);
            firstSelectedGraphemeExtent = selectedGraphemes.characters().first.Length;
            startHandleRect = ((Rect?)((dynamic)this.renderObject).getRectForComposingRange(new global::Doroti.Ui.TextRange(start: this._selection.start, end: (this._selection.start + firstSelectedGraphemeExtent))));
        }
        return ((startHandleRect?.height ?? (double)((global::Doroti.Framework.Rendering.RenderEditable)this.renderObject).preferredLineHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getEndGlyphHeight()
    {
        string currText = ((global::Doroti.Framework.Services.TextSelectionDelegate)this.selectionDelegate).textEditingValue.text;
        long lastSelectedGraphemeExtent = default!;
        global::Doroti.Ui.Rect? endHandleRect = default!;
        if ((((((global::Doroti.Framework.Rendering.RenderEditable)this.renderObject).plainText == currText) && this._selection.isValid) && !this._selection.isCollapsed))
        {
            string selectedGraphemes = this._selection.textInside(currText);
            lastSelectedGraphemeExtent = selectedGraphemes.characters().last.Length;
            endHandleRect = ((Rect?)((dynamic)this.renderObject).getRectForComposingRange(new global::Doroti.Ui.TextRange(start: (this._selection.end - lastSelectedGraphemeExtent), end: this._selection.end)));
        }
        return ((endHandleRect?.height ?? (double)((global::Doroti.Framework.Rendering.RenderEditable)this.renderObject).preferredLineHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual MagnifierInfo _buildMagnifier(global::Doroti.Framework.Rendering.RenderEditable renderEditable, Offset globalGesturePosition, TextPosition currentTextPosition)
    {
        global::Doroti.Framework.Services.TextSelection lineAtOffset = ((global::Doroti.Framework.Services.TextSelection)(object?)renderEditable.getLineAtOffset(currentTextPosition));
        var positionAtEndOfLine = new global::Doroti.Ui.TextPosition(offset: ((global::Doroti.Framework.Services.TextSelection)lineAtOffset).extentOffset, affinity: TextAffinity.upstream);
        var positionAtBeginningOfLine = new global::Doroti.Ui.TextPosition(offset: ((global::Doroti.Framework.Services.TextSelection)lineAtOffset).baseOffset);
        var localLineBoundaries = global::Doroti.Ui.Rect.fromPoints(renderEditable.getLocalRectForCaret(positionAtBeginningOfLine).topCenter, renderEditable.getLocalRectForCaret(positionAtEndOfLine).bottomCenter);
        var overlay = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)Overlay.of(this.context, rootOverlay: true).context.findRenderObject())!;
        Matrix4 transformToOverlay = ((Matrix4)(object?)renderEditable.getTransformTo(overlay));
        global::Doroti.Ui.Rect overlayLineBoundaries = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(transformToOverlay, localLineBoundaries));
        global::Doroti.Ui.Rect localCaretRect = ((global::Doroti.Ui.Rect)(object?)renderEditable.getLocalRectForCaret(currentTextPosition));
        global::Doroti.Ui.Rect overlayCaretRect = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(transformToOverlay, localCaretRect));
        global::Doroti.Ui.Offset overlayGesturePosition = ((global::Doroti.Ui.Offset)(object?)(((Offset?)((dynamic)overlay)?.globalToLocal(globalGesturePosition)) ?? globalGesturePosition));
        return new MagnifierInfo(fieldBounds: MatrixUtils.transformRect(transformToOverlay, renderEditable.paintBounds), globalGesturePosition: overlayGesturePosition, caretRect: overlayCaretRect, currentLineBoundaries: overlayLineBoundaries);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleSelectionEndHandleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        if (!this.renderObject.attached)
        {
            return;
        }
        _endHandleDragPosition = ((global::Doroti.Framework.Gestures.DragStartDetails)details).globalPosition.dy;
        double centerOfLineLocal = DartRuntimePrimitives.ConvertValue<double>((((SelectionOverlay)this._selectionOverlay).selectionEndpoints.Last().point.dy - (((global::Doroti.Framework.Rendering.RenderEditable)this.renderObject).preferredLineHeight / 2L)));
        double centerOfLineGlobal = DartRuntimePrimitives.ConvertValue<double>(((Offset)((dynamic)this.renderObject).localToGlobal(new global::Doroti.Ui.Offset(0.0, centerOfLineLocal))).dy);
        _endHandleDragTarget = (centerOfLineGlobal - ((global::Doroti.Framework.Gestures.DragStartDetails)details).globalPosition.dy);
        global::Doroti.Ui.TextPosition position = ((global::Doroti.Ui.TextPosition)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.TextPosition>(((TextPosition)((dynamic)this.renderObject).getPositionForPoint(new global::Doroti.Ui.Offset(((global::Doroti.Framework.Gestures.DragStartDetails)details).globalPosition.dx, centerOfLineGlobal)))));
        if (((object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS)) || (object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.macOS))))
        {
            _dragStartSelection ??= this._selection;
        }
        this._selectionOverlay.showMagnifier(_buildMagnifier(currentTextPosition: position, globalGesturePosition: ((global::Doroti.Framework.Gestures.DragStartDetails)details).globalPosition, renderEditable: this.renderObject));
    }

    internal virtual double? _getHandleDy(double dragDy, double handleDy)
    {
        double preferredLineHeightLocal = DartRuntimePrimitives.ConvertValue<double>(((global::Doroti.Framework.Rendering.RenderEditable)this.renderObject).preferredLineHeight);
        DartRuntimePrimitives.Assert(() => double.IsFinite(preferredLineHeightLocal), () => (object?)"Preferred line height is expected to always be finite.");
        if ((((preferredLineHeightLocal <= 0.0) || !double.IsFinite(dragDy)) || !double.IsFinite(handleDy)))
        {
            return null;
        }
        double distanceDragged = (dragDy - handleDy);
        var dragDirection = ((distanceDragged < 0.0) ? -1L : 1L);
        long linesDragged = (dragDirection * ((distanceDragged.abs() / preferredLineHeightLocal)).floor());
        return (handleDy + (linesDragged * preferredLineHeightLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleSelectionEndHandleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        if (!this.renderObject.attached)
        {
            return;
        }
        global::Doroti.Ui.Offset localPosition = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(((Offset)((dynamic)this.renderObject).globalToLocal(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).globalPosition))));
        double? nextEndHandleDragPositionLocal = DartRuntimePrimitives.ConvertValue<double>(_getHandleDy(localPosition.dy, ((Offset)((dynamic)this.renderObject).globalToLocal(new global::Doroti.Ui.Offset(0.0, this._endHandleDragPosition))).dy));
        if ((nextEndHandleDragPositionLocal is null))
        {
            return;
        }
        _endHandleDragPosition = ((Offset)((dynamic)this.renderObject).localToGlobal(new global::Doroti.Ui.Offset(0.0, DartRuntimePrimitives.RequireValue(nextEndHandleDragPositionLocal)))).dy;
        var handleTargetGlobal = new global::Doroti.Ui.Offset(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).globalPosition.dx, (this._endHandleDragPosition + this._endHandleDragTarget));
        global::Doroti.Ui.TextPosition position = ((global::Doroti.Ui.TextPosition)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.TextPosition>(((TextPosition)((dynamic)this.renderObject).getPositionForPoint(handleTargetGlobal))));
        global::Doroti.Framework.Services.TextSelection newSelection = default!;
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    DartRuntimePrimitives.Assert(() => (this._dragStartSelection is not null));
                    if (this._dragStartSelection!.isCollapsed)
                    {
                        this._selectionOverlay.updateMagnifier(_buildMagnifier(currentTextPosition: position, globalGesturePosition: ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).globalPosition, renderEditable: this.renderObject));
                        var currentSelection = global::Doroti.Framework.Services.TextSelection.CreateFromPosition(position);
                        _handleSelectionHandleChanged(currentSelection);
                        return;
                    }
                    bool dragStartSelectionNormalized = (this._dragStartSelection!.extentOffset >= this._dragStartSelection!.baseOffset);
                    newSelection = new global::Doroti.Framework.Services.TextSelection(baseOffset: (dragStartSelectionNormalized ? this._dragStartSelection!.baseOffset : this._dragStartSelection!.extentOffset), extentOffset: position.offset);
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    if (this._selection.isCollapsed)
                    {
                        this._selectionOverlay.updateMagnifier(_buildMagnifier(currentTextPosition: position, globalGesturePosition: ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).globalPosition, renderEditable: this.renderObject));
                        var currentSelectionLocal = global::Doroti.Framework.Services.TextSelection.CreateFromPosition(position);
                        _handleSelectionHandleChanged(currentSelectionLocal);
                        return;
                    }
                    newSelection = new global::Doroti.Framework.Services.TextSelection(baseOffset: ((global::Doroti.Framework.Services.TextSelection)this._selection).baseOffset, extentOffset: position.offset);
                    if ((((global::Doroti.Framework.Services.TextSelection)newSelection).baseOffset >= ((global::Doroti.Framework.Services.TextSelection)newSelection).extentOffset))
                    {
                        return;
                    }
                    break;
                }
        }
        _handleSelectionHandleChanged(newSelection);
        this._selectionOverlay.updateMagnifier(_buildMagnifier(currentTextPosition: ((global::Doroti.Framework.Services.TextSelection)newSelection).extent, globalGesturePosition: ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).globalPosition, renderEditable: this.renderObject));
    }

    internal virtual void _handleSelectionStartHandleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        if (!this.renderObject.attached)
        {
            return;
        }
        _startHandleDragPosition = ((global::Doroti.Framework.Gestures.DragStartDetails)details).globalPosition.dy;
        double centerOfLineLocal = DartRuntimePrimitives.ConvertValue<double>((((SelectionOverlay)this._selectionOverlay).selectionEndpoints.First().point.dy - (((global::Doroti.Framework.Rendering.RenderEditable)this.renderObject).preferredLineHeight / 2L)));
        double centerOfLineGlobal = DartRuntimePrimitives.ConvertValue<double>(((Offset)((dynamic)this.renderObject).localToGlobal(new global::Doroti.Ui.Offset(0.0, centerOfLineLocal))).dy);
        _startHandleDragTarget = (centerOfLineGlobal - ((global::Doroti.Framework.Gestures.DragStartDetails)details).globalPosition.dy);
        global::Doroti.Ui.TextPosition position = ((global::Doroti.Ui.TextPosition)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.TextPosition>(((TextPosition)((dynamic)this.renderObject).getPositionForPoint(new global::Doroti.Ui.Offset(((global::Doroti.Framework.Gestures.DragStartDetails)details).globalPosition.dx, centerOfLineGlobal)))));
        if (((object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS)) || (object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.macOS))))
        {
            _dragStartSelection ??= this._selection;
        }
        this._selectionOverlay.showMagnifier(_buildMagnifier(currentTextPosition: position, globalGesturePosition: ((global::Doroti.Framework.Gestures.DragStartDetails)details).globalPosition, renderEditable: this.renderObject));
    }

    internal virtual void _handleSelectionStartHandleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        if (!this.renderObject.attached)
        {
            return;
        }
        global::Doroti.Ui.Offset localPosition = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(((Offset)((dynamic)this.renderObject).globalToLocal(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).globalPosition))));
        double? nextStartHandleDragPositionLocal = DartRuntimePrimitives.ConvertValue<double>(_getHandleDy(localPosition.dy, ((Offset)((dynamic)this.renderObject).globalToLocal(new global::Doroti.Ui.Offset(0.0, this._startHandleDragPosition))).dy));
        if ((nextStartHandleDragPositionLocal is null))
        {
            return;
        }
        _startHandleDragPosition = ((Offset)((dynamic)this.renderObject).localToGlobal(new global::Doroti.Ui.Offset(0.0, DartRuntimePrimitives.RequireValue(nextStartHandleDragPositionLocal)))).dy;
        var handleTargetGlobal = new global::Doroti.Ui.Offset(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).globalPosition.dx, (this._startHandleDragPosition + this._startHandleDragTarget));
        global::Doroti.Ui.TextPosition position = ((global::Doroti.Ui.TextPosition)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.TextPosition>(((TextPosition)((dynamic)this.renderObject).getPositionForPoint(handleTargetGlobal))));
        global::Doroti.Framework.Services.TextSelection newSelection = default!;
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    DartRuntimePrimitives.Assert(() => (this._dragStartSelection is not null));
                    if (this._dragStartSelection!.isCollapsed)
                    {
                        this._selectionOverlay.updateMagnifier(_buildMagnifier(currentTextPosition: position, globalGesturePosition: ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).globalPosition, renderEditable: this.renderObject));
                        var currentSelection = global::Doroti.Framework.Services.TextSelection.CreateFromPosition(position);
                        _handleSelectionHandleChanged(currentSelection);
                        return;
                    }
                    bool dragStartSelectionNormalized = (this._dragStartSelection!.extentOffset >= this._dragStartSelection!.baseOffset);
                    newSelection = new global::Doroti.Framework.Services.TextSelection(baseOffset: (dragStartSelectionNormalized ? this._dragStartSelection!.extentOffset : this._dragStartSelection!.baseOffset), extentOffset: position.offset);
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    if (this._selection.isCollapsed)
                    {
                        this._selectionOverlay.updateMagnifier(_buildMagnifier(currentTextPosition: position, globalGesturePosition: ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).globalPosition, renderEditable: this.renderObject));
                        var currentSelectionLocal = global::Doroti.Framework.Services.TextSelection.CreateFromPosition(position);
                        _handleSelectionHandleChanged(currentSelectionLocal);
                        return;
                    }
                    newSelection = new global::Doroti.Framework.Services.TextSelection(baseOffset: position.offset, extentOffset: ((global::Doroti.Framework.Services.TextSelection)this._selection).extentOffset);
                    if ((((global::Doroti.Framework.Services.TextSelection)newSelection).baseOffset >= ((global::Doroti.Framework.Services.TextSelection)newSelection).extentOffset))
                    {
                        return;
                    }
                    break;
                }
        }
        this._selectionOverlay.updateMagnifier(_buildMagnifier(currentTextPosition: ((((global::Doroti.Framework.Services.TextSelection)newSelection).extent.offset < ((global::Doroti.Framework.Services.TextSelection)newSelection).@base.offset) ? ((global::Doroti.Framework.Services.TextSelection)newSelection).extent : ((global::Doroti.Framework.Services.TextSelection)newSelection).@base), globalGesturePosition: ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).globalPosition, renderEditable: this.renderObject));
        _handleSelectionHandleChanged(newSelection);
    }

    internal virtual void _handleAnyDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        if (!((BuildContext)this.context).mounted)
        {
            return;
        }
        _dragStartSelection = null;
        bool draggingHandles = (((SelectionOverlay)this._selectionOverlay).isDraggingStartHandle || ((SelectionOverlay)this._selectionOverlay).isDraggingEndHandle);
        if ((this.selectionControls is not TextSelectionHandleControls))
        {
            if (!draggingHandles)
            {
                this._selectionOverlay.hideMagnifier();
                if (!this._selection.isCollapsed)
                {
                    this._selectionOverlay.showToolbar();
                }
            }
            return;
        }
        if (!draggingHandles)
        {
            this._selectionOverlay.hideMagnifier();
            if (!this._selection.isCollapsed)
            {
                this._selectionOverlay.showToolbar(context: this.context, contextMenuBuilder: (global::System.Func<BuildContext, Widget>?)this.contextMenuBuilder);
            }
        }
    }

    internal virtual void _handleSelectionHandleChanged(global::Doroti.Framework.Services.TextSelection newSelection)
    {
        this.selectionDelegate.userUpdateTextEditingValue(this._value.copyWith(selection: newSelection), global::Doroti.Framework.Services.SelectionChangedCause.drag);
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

    public SelectionOverlay(BuildContext context, Widget? debugRequiredFor = null, global::Doroti.Framework.Rendering.TextSelectionHandleType startHandleType = default!, double lineHeightAtStart = default!, global::Doroti.Framework.Foundation.ValueListenable<bool>? startHandlesVisible = null, global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>? onStartHandleDragStart = null, global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>? onStartHandleDragUpdate = null, global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>? onStartHandleDragEnd = null, global::Doroti.Framework.Rendering.TextSelectionHandleType endHandleType = default!, double lineHeightAtEnd = default!, global::Doroti.Framework.Foundation.ValueListenable<bool>? endHandlesVisible = null, global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>? onEndHandleDragStart = null, global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>? onEndHandleDragUpdate = null, global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>? onEndHandleDragEnd = null, global::Doroti.Framework.Foundation.ValueListenable<bool>? toolbarVisible = null, List<global::Doroti.Framework.Rendering.TextSelectionPoint> selectionEndpoints = default!, TextSelectionControls? selectionControls = default!, global::Doroti.Framework.Services.TextSelectionDelegate? selectionDelegate = default!, ClipboardStatusNotifier? clipboardStatus = default!, global::Doroti.Framework.Rendering.LayerLink startHandleLayerLink = default!, global::Doroti.Framework.Rendering.LayerLink endHandleLayerLink = default!, global::Doroti.Framework.Rendering.LayerLink toolbarLayerLink = default!, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, global::System.Action? onSelectionHandleTapped = null, Offset? toolbarLocation = null, TextMagnifierConfiguration magnifierConfiguration = default!)
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
        this._startHandleType = startHandleType;
        this._lineHeightAtStart = lineHeightAtStart;
        this._endHandleType = endHandleType;
        this._lineHeightAtEnd = lineHeightAtEnd;
        this._selectionEndpoints = selectionEndpoints;
        this._toolbarLocation = toolbarLocation;
        System.Diagnostics.Debug.Assert(global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasOverlay(context));
    }

    public virtual bool toolbarIsVisible
    {
        get
        {
            return ((this.selectionControls is TextSelectionHandleControls) ? (((ContextMenuController)this._contextMenuController).isShown || ((ContextMenuController)this._spellCheckToolbarController).isShown) : ((this._toolbar is not null) || ((ContextMenuController)this._spellCheckToolbarController).isShown));
            return default!;
        }
    }
    public virtual bool magnifierIsVisible => ((MagnifierController)this._magnifierController).shown;
    public virtual bool magnifierExists => DartRuntimePrimitives.ConvertValue<bool>((((MagnifierController)this._magnifierController).overlayEntry is not null));
    public virtual void showMagnifier(MagnifierInfo initialMagnifierInfo)
    {
        if ((((MagnifierController)this._magnifierController).overlayEntry is not null))
        {
            return;
        }
        if (this.toolbarIsVisible)
        {
            hideToolbar();
        }
        this._magnifierInfo.value = initialMagnifierInfo;
        Widget? builtMagnifier = this.magnifierConfiguration.magnifierBuilder(this.context, this._magnifierController, this._magnifierInfo);
        if ((builtMagnifier is null))
        {
            return;
        }
        DartRuntimePrimitives.Ignore(this._magnifierController.show(context: this.context, below: (((TextMagnifierConfiguration)this.magnifierConfiguration).shouldDisplayHandlesInMagnifier ? null : this._handles?.start), builder: ((global::System.Func<BuildContext, Widget>)((_) => builtMagnifier))));
    }

    public virtual void hideMagnifier()
    {
        if ((((MagnifierController)this._magnifierController).overlayEntry is null))
        {
            return;
        }
        DartRuntimePrimitives.Ignore(this._magnifierController.hide());
    }

    public virtual global::Doroti.Framework.Rendering.TextSelectionHandleType startHandleType
    {
        get => this._startHandleType;
        set
        {
            var __value = value;
            if ((object.Equals(this._startHandleType, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _startHandleType = DartRuntimePrimitives.RequireValue(__value);
            markNeedsBuild();
        }
    }
    public virtual double lineHeightAtStart
    {
        get => this._lineHeightAtStart;
        set
        {
            var __value = value;
            if ((this._lineHeightAtStart == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _lineHeightAtStart = DartRuntimePrimitives.RequireValue(__value);
            markNeedsBuild();
        }
    }
    public virtual bool isDraggingStartHandle => DartRuntimePrimitives.ConvertValue<bool>((this._isDraggingStartHandle || this._startHandleDragInProgress));
    internal virtual bool _canDragStartHandle => DartRuntimePrimitives.ConvertValue<bool>((!this._isDraggingEndHandle || ((((!object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS)) && (!object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.macOS))) && !global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb))));
    internal virtual void _handleStartHandleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        DartRuntimePrimitives.Assert(() => !this._isDraggingStartHandle);
        if ((this._handles is null))
        {
            _isDraggingStartHandle = false;
            return;
        }
        _startHandleDragInProgress = true;
        if (!this._canDragStartHandle)
        {
            return;
        }
        _isDraggingStartHandle = (object.Equals(((global::Doroti.Framework.Gestures.DragStartDetails)details).kind, PointerDeviceKind.touch));
        this.onStartHandleDragStart?.Invoke(details);
    }

    internal virtual void _handleStartHandleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        if ((this._handles is null))
        {
            _isDraggingStartHandle = false;
            return;
        }
        if (!this._canDragStartHandle)
        {
            return;
        }
        if (!this._isDraggingStartHandle)
        {
            _isDraggingStartHandle = (object.Equals(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).kind, PointerDeviceKind.touch));
            var startDetails = new global::Doroti.Framework.Gestures.DragStartDetails(globalPosition: ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).globalPosition, localPosition: ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).localPosition, sourceTimeStamp: ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).sourceTimeStamp, kind: ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).kind);
            this.onStartHandleDragStart?.Invoke(startDetails);
        }
        this.onStartHandleDragUpdate?.Invoke(details);
    }

    internal virtual void _handleStartHandleDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        _isDraggingStartHandle = false;
        if ((this._handles is null))
        {
            return;
        }
        _startHandleDragInProgress = false;
        if (!this._canDragStartHandle)
        {
            return;
        }
        this.onStartHandleDragEnd?.Invoke(details);
    }

    public virtual global::Doroti.Framework.Rendering.TextSelectionHandleType endHandleType
    {
        get => this._endHandleType;
        set
        {
            var __value = value;
            if ((object.Equals(this._endHandleType, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _endHandleType = DartRuntimePrimitives.RequireValue(__value);
            markNeedsBuild();
        }
    }
    public virtual double lineHeightAtEnd
    {
        get => this._lineHeightAtEnd;
        set
        {
            var __value = value;
            if ((this._lineHeightAtEnd == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _lineHeightAtEnd = DartRuntimePrimitives.RequireValue(__value);
            markNeedsBuild();
        }
    }
    public virtual bool isDraggingEndHandle => DartRuntimePrimitives.ConvertValue<bool>((this._isDraggingEndHandle || this._endHandleDragInProgress));
    internal virtual bool _canDragEndHandle => DartRuntimePrimitives.ConvertValue<bool>((!this._isDraggingStartHandle || ((((!object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS)) && (!object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.macOS))) && !global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb))));
    internal virtual void _handleEndHandleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        DartRuntimePrimitives.Assert(() => !this._isDraggingEndHandle);
        if ((this._handles is null))
        {
            _isDraggingEndHandle = false;
            return;
        }
        _endHandleDragInProgress = true;
        if (!this._canDragEndHandle)
        {
            return;
        }
        _isDraggingEndHandle = (object.Equals(((global::Doroti.Framework.Gestures.DragStartDetails)details).kind, PointerDeviceKind.touch));
        this.onEndHandleDragStart?.Invoke(details);
    }

    internal virtual void _handleEndHandleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        if ((this._handles is null))
        {
            _isDraggingEndHandle = false;
            return;
        }
        if (!this._canDragEndHandle)
        {
            return;
        }
        if (!this._isDraggingEndHandle)
        {
            _isDraggingEndHandle = (object.Equals(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).kind, PointerDeviceKind.touch));
            var startDetails = new global::Doroti.Framework.Gestures.DragStartDetails(globalPosition: ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).globalPosition, localPosition: ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).localPosition, sourceTimeStamp: ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).sourceTimeStamp, kind: ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).kind);
            this.onEndHandleDragStart?.Invoke(startDetails);
        }
        this.onEndHandleDragUpdate?.Invoke(details);
    }

    internal virtual void _handleEndHandleDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        _isDraggingEndHandle = false;
        if ((this._handles is null))
        {
            return;
        }
        _endHandleDragInProgress = false;
        if (!this._canDragEndHandle)
        {
            return;
        }
        this.onEndHandleDragEnd?.Invoke(details);
    }

    public virtual List<global::Doroti.Framework.Rendering.TextSelectionPoint> selectionEndpoints
    {
        get => this._selectionEndpoints;
        set
        {
            var __value = value;
            if (!global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals(this._selectionEndpoints, __value))
            {
                markNeedsBuild();
                if ((this._isDraggingEndHandle || this._isDraggingStartHandle))
                {
                    switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
                    {
                        case global::Doroti.Framework.Foundation.TargetPlatform.android:
                            {
                                DartRuntimePrimitives.Ignore(HapticFeedback.selectionClick());
                                break;
                            }
                        case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                        case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                        case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                        case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                        case global::Doroti.Framework.Foundation.TargetPlatform.windows:
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
        get => this._toolbarLocation;
        set
        {
            var __value = value;
            if ((object.Equals(this._toolbarLocation, __value)))
            {
                return;
            }
            _toolbarLocation = __value;
            markNeedsBuild();
        }
    }
    public virtual void showHandles()
    {
        if ((this._handles is not null))
        {
            return;
        }
        OverlayState overlay = ((OverlayState)(object?)Overlay.of(this.context, rootOverlay: true, debugRequiredFor: this.debugRequiredFor));
        CapturedThemes capturedThemes = ((CapturedThemes)(object?)InheritedTheme.capture(from: this.context, to: overlay.context));
        _handles = (end: new OverlayEntry(builder: ((global::System.Func<BuildContext, Widget>)((context) =>
        {
            return ((Widget)(object?)capturedThemes.wrap(_buildEndHandle(context)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))), start: new OverlayEntry(builder: ((global::System.Func<BuildContext, Widget>)((context) =>
        {
            return ((Widget)(object?)capturedThemes.wrap(_buildStartHandle(context)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
        overlay.insertAll(new List<OverlayEntry> { DartRuntimePrimitives.RequireValue(this._handles).start, DartRuntimePrimitives.RequireValue(this._handles).end }.Cast<OverlayEntry>());
    }

    public virtual void hideHandles()
    {
        if ((this._handles is not null))
        {
            DartRuntimePrimitives.RequireValue(this._handles).start.remove();
            DartRuntimePrimitives.RequireValue(this._handles).start.dispose();
            DartRuntimePrimitives.RequireValue(this._handles).end.remove();
            DartRuntimePrimitives.RequireValue(this._handles).end.dispose();
            _handles = null;
        }
    }

    public virtual void showToolbar(BuildContext? context = null, global::System.Func<BuildContext, Widget>? contextMenuBuilder = null)
    {
        if ((contextMenuBuilder is null))
        {
            if ((this._toolbar is not null))
            {
                return;
            }
            _toolbar = new OverlayEntry(builder: (global::System.Func<BuildContext, Widget>)this._buildToolbar);
            Overlay.of(this.context, rootOverlay: true, debugRequiredFor: this.debugRequiredFor).insert(this._toolbar!, above: this._handles?.end);
            return;
        }
        if ((context is null))
        {
            return;
        }
        var renderBox = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)context.findRenderObject()!)!;
        this._contextMenuController.show(context: context, contextMenuBuilder: ((global::System.Func<BuildContext, Widget>)((context) =>
        {
            return ((Widget)(object?)new _SelectionToolbarWrapper__text_selection(visibility: this.toolbarVisible, layerLink: this.toolbarLayerLink, offset: -((Offset)((dynamic)renderBox).localToGlobal(Offset.zero)), child: contextMenuBuilder(context)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
    }

    public virtual void showSpellCheckSuggestionsToolbar(BuildContext? context = null, global::System.Func<BuildContext, Widget> builder = default!)
    {
        if ((context is null))
        {
            return;
        }
        var renderBox = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)context.findRenderObject()!)!;
        this._spellCheckToolbarController.show(context: context, contextMenuBuilder: ((global::System.Func<BuildContext, Widget>)((context) =>
        {
            return ((Widget)(object?)new _SelectionToolbarWrapper__text_selection(layerLink: this.toolbarLayerLink, offset: -((Offset)((dynamic)renderBox).localToGlobal(Offset.zero)), child: builder(context)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
    }

    public virtual void markNeedsBuild()
    {
        if (((this._handles is null) && (this._toolbar is null)))
        {
            return;
        }
        if ((object.Equals(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Framework.Scheduler.SchedulerPhase.persistentCallbacks)))
        {
            if (this._buildScheduled)
            {
                return;
            }
            _buildScheduled = true;
            global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((duration) =>
            {
                _buildScheduled = false;
                this._handles?.start.markNeedsBuild();
                this._handles?.end.markNeedsBuild();
                this._toolbar?.markNeedsBuild();
                if (((ContextMenuController)this._contextMenuController).isShown)
                {
                    this._contextMenuController.markNeedsBuild();
                }
                else
                {
                    if (((ContextMenuController)this._spellCheckToolbarController).isShown)
                    {
                        this._spellCheckToolbarController.markNeedsBuild();
                    }
                }
            })), debugLabel: "SelectionOverlay.markNeedsBuild");
        }
        else
        {
            if ((this._handles is not null))
            {
                DartRuntimePrimitives.RequireValue(this._handles).start.markNeedsBuild();
                DartRuntimePrimitives.RequireValue(this._handles).end.markNeedsBuild();
            }
            this._toolbar?.markNeedsBuild();
            if (((ContextMenuController)this._contextMenuController).isShown)
            {
                this._contextMenuController.markNeedsBuild();
            }
            else
            {
                if (((ContextMenuController)this._spellCheckToolbarController).isShown)
                {
                    this._spellCheckToolbarController.markNeedsBuild();
                }
            }
        }
    }

    public virtual void hide()
    {
        DartRuntimePrimitives.Ignore(this._magnifierController.hide());
        hideHandles();
        if ((((this._toolbar is not null) || ((ContextMenuController)this._contextMenuController).isShown) || ((ContextMenuController)this._spellCheckToolbarController).isShown))
        {
            hideToolbar();
        }
    }

    public virtual void hideToolbar()
    {
        this._contextMenuController.remove();
        this._spellCheckToolbarController.remove();
        if ((this._toolbar is null))
        {
            return;
        }
        this._toolbar?.remove();
        this._toolbar?.dispose();
        _toolbar = null;
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        hide();
        this._magnifierInfo.dispose();
    }

    internal virtual Widget _buildStartHandle(BuildContext context)
    {
        Widget handle = default!;
        TextSelectionControls? selectionControlsLocal = this.selectionControls;
        if (((selectionControlsLocal is null) || (((object.Equals(this._startHandleType, global::Doroti.Framework.Rendering.TextSelectionHandleType.collapsed)) && this._isDraggingEndHandle))))
        {
            handle = DartRuntimePrimitives.ConvertValue<Widget>(SizedBox.CreateShrink());
        }
        else
        {
            handle = DartRuntimePrimitives.ConvertValue<Widget>(new _SelectionHandleOverlay__text_selection(type: this._startHandleType, handleLayerLink: this.startHandleLayerLink, onSelectionHandleTapped: () => this.onSelectionHandleTapped(), onSelectionHandleDragStart: (global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>)this._handleStartHandleDragStart, onSelectionHandleDragUpdate: (global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)this._handleStartHandleDragUpdate, onSelectionHandleDragEnd: (global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>)this._handleStartHandleDragEnd, selectionControls: selectionControlsLocal, visibility: this.startHandlesVisible, preferredLineHeight: this._lineHeightAtStart, dragStartBehavior: this.dragStartBehavior));
        }
        return ((Widget)(object?)new TapRegion(groupId: typeof(SelectableRegion), child: new TextFieldTapRegion(child: new ExcludeSemantics(child: handle))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildEndHandle(BuildContext context)
    {
        Widget handle = default!;
        TextSelectionControls? selectionControlsLocal = this.selectionControls;
        if ((((selectionControlsLocal is null) || (((object.Equals(this._endHandleType, global::Doroti.Framework.Rendering.TextSelectionHandleType.collapsed)) && this._isDraggingStartHandle))) || ((((object.Equals(this._endHandleType, global::Doroti.Framework.Rendering.TextSelectionHandleType.collapsed)) && !this._isDraggingStartHandle) && !this._isDraggingEndHandle))))
        {
            handle = DartRuntimePrimitives.ConvertValue<Widget>(SizedBox.CreateShrink());
        }
        else
        {
            handle = DartRuntimePrimitives.ConvertValue<Widget>(new _SelectionHandleOverlay__text_selection(type: this._endHandleType, handleLayerLink: this.endHandleLayerLink, onSelectionHandleTapped: () => this.onSelectionHandleTapped(), onSelectionHandleDragStart: (global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>)this._handleEndHandleDragStart, onSelectionHandleDragUpdate: (global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)this._handleEndHandleDragUpdate, onSelectionHandleDragEnd: (global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>)this._handleEndHandleDragEnd, selectionControls: selectionControlsLocal, visibility: this.endHandlesVisible, preferredLineHeight: this._lineHeightAtEnd, dragStartBehavior: this.dragStartBehavior));
        }
        return ((Widget)(object?)new TapRegion(groupId: typeof(SelectableRegion), child: new TextFieldTapRegion(child: new ExcludeSemantics(child: handle))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildToolbar(BuildContext context)
    {
        if ((this.selectionControls is null))
        {
            return ((Widget)(object?)SizedBox.CreateShrink());
        }
        DartRuntimePrimitives.Assert(() => (this.selectionDelegate is not null), () => (object?)"If not using contextMenuBuilder, must pass selectionDelegate.");
        var renderBox = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)this.context.findRenderObject()!)!;
        var editingRegion = global::Doroti.Ui.Rect.fromPoints(((Offset)((dynamic)renderBox).localToGlobal(Offset.zero)), ((Offset)((dynamic)renderBox).localToGlobal(((global::Doroti.Framework.Rendering.RenderBox)renderBox).size.bottomRight(Offset.zero))));
        bool isMultiline = ((this.selectionEndpoints.Last().point.dy - this.selectionEndpoints.First().point.dy) > (this.lineHeightAtEnd / 2L));
        double midX = (isMultiline ? (editingRegion.width / 2L) : (((this.selectionEndpoints.First().point.dx + this.selectionEndpoints.Last().point.dx)) / 2L));
        var midpoint = new global::Doroti.Ui.Offset(midX, (this.selectionEndpoints.First().point.dy - this.lineHeightAtStart));
        return ((Widget)(object?)new _SelectionToolbarWrapper__text_selection(visibility: this.toolbarVisible, layerLink: this.toolbarLayerLink, offset: -editingRegion.topLeft, child: new Builder(builder: ((global::System.Func<BuildContext, Widget>)((context) =>
        {
            return ((Widget)(object?)this.selectionControls!.buildToolbar(context, editingRegion, this.lineHeightAtStart, midpoint, this.selectionEndpoints, this.selectionDelegate!, this.clipboardStatus, this.toolbarLocation));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void updateMagnifier(MagnifierInfo magnifierInfo)
    {
        if ((((MagnifierController)this._magnifierController).overlayEntry is null))
        {
            return;
        }
        this._magnifierInfo.value = magnifierInfo;
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

    internal virtual global::Doroti.Framework.Animation.Animation<double> _opacity => ((global::Doroti.Framework.Animation.AnimationController)this._controller).view;
    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Framework.Animation.AnimationController(duration: SelectionOverlay.fadeDuration, vsync: this);
        _toolbarVisibilityChanged();
        ((_SelectionToolbarWrapper__text_selection)this.widget).visibility?.addListener(() => this._toolbarVisibilityChanged());
    }

    public override void didUpdateWidget(_SelectionToolbarWrapper__text_selection oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((object.Equals(((_SelectionToolbarWrapper__text_selection)oldWidget).visibility, ((_SelectionToolbarWrapper__text_selection)this.widget).visibility)))
        {
            return;
        }
        ((_SelectionToolbarWrapper__text_selection)oldWidget).visibility?.removeListener(() => this._toolbarVisibilityChanged());
        _toolbarVisibilityChanged();
        ((_SelectionToolbarWrapper__text_selection)this.widget).visibility?.addListener(() => this._toolbarVisibilityChanged());
    }

    public override void dispose()
    {
        ((_SelectionToolbarWrapper__text_selection)this.widget).visibility?.removeListener(() => this._toolbarVisibilityChanged());
        this._controller.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this._ticker is null) || !this._ticker!.isActive))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _toolbarVisibilityChanged()
    {
        if ((((_SelectionToolbarWrapper__text_selection)this.widget).visibility?.value ?? true))
        {
            this._controller.forward();
        }
        else
        {
            this._controller.reverse();
        }
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new TapRegion(groupId: typeof(SelectableRegion), child: new TextFieldTapRegion(child: new Directionality(textDirection: Directionality.of(this.context), child: new FadeTransition(opacity: this._opacity, child: new CompositedTransformFollower(link: ((_SelectionToolbarWrapper__text_selection)this.widget).layerLink, showWhenUnlinked: false, offset: ((_SelectionToolbarWrapper__text_selection)this.widget).offset, child: ((_SelectionToolbarWrapper__text_selection)this.widget).child))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._ticker is null))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this.GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._ticker = new global::Doroti.Framework.Scheduler.Ticker((global::System.Action<Duration>)onTick, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
        _updateTickerModeNotifier();
        _updateTicker();
        return this._ticker!;
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
        TickerModeData values = this._tickerModeNotifier!.value;
        if ((this._ticker is not null))
        {
            this._ticker!.muted = !((TickerModeData)values).enabled;
            this._ticker!.forceFrames = ((TickerModeData)values).forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        newNotifier.addListener(() => this._updateTicker());
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = ((this._ticker?.isActive, this._ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) });
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", this._ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
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

    internal _SelectionHandleOverlay__text_selection(global::Doroti.Framework.Rendering.TextSelectionHandleType type, global::Doroti.Framework.Rendering.LayerLink handleLayerLink, global::System.Action? onSelectionHandleTapped = null, global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>? onSelectionHandleDragStart = null, global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>? onSelectionHandleDragUpdate = null, global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>? onSelectionHandleDragEnd = null, TextSelectionControls selectionControls = default!, global::Doroti.Framework.Foundation.ValueListenable<bool>? visibility = null, double preferredLineHeight = default!, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start)
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

    internal virtual global::Doroti.Framework.Animation.Animation<double> _opacity => ((global::Doroti.Framework.Animation.AnimationController)this._controller).view;
    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Framework.Animation.AnimationController(duration: SelectionOverlay.fadeDuration, vsync: this);
        _handleVisibilityChanged();
        ((_SelectionHandleOverlay__text_selection)this.widget).visibility?.addListener(() => this._handleVisibilityChanged());
    }

    internal virtual void _handleVisibilityChanged()
    {
        if ((((_SelectionHandleOverlay__text_selection)this.widget).visibility?.value ?? true))
        {
            this._controller.forward();
        }
        else
        {
            this._controller.reverse();
        }
    }

    internal virtual global::Doroti.Ui.Rect _getHandleRect(global::Doroti.Framework.Rendering.TextSelectionHandleType type, double preferredLineHeight)
    {
        global::Doroti.Ui.Size handleSize = ((global::Doroti.Ui.Size)(object?)((_SelectionHandleOverlay__text_selection)this.widget).selectionControls.getHandleSize(preferredLineHeight));
        return global::Doroti.Ui.Rect.fromLTWH(0.0, 0.0, handleSize.width, handleSize.height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didUpdateWidget(_SelectionHandleOverlay__text_selection oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        ((_SelectionHandleOverlay__text_selection)oldWidget).visibility?.removeListener(() => this._handleVisibilityChanged());
        _handleVisibilityChanged();
        ((_SelectionHandleOverlay__text_selection)this.widget).visibility?.addListener(() => this._handleVisibilityChanged());
    }

    public override void dispose()
    {
        ((_SelectionHandleOverlay__text_selection)this.widget).visibility?.removeListener(() => this._handleVisibilityChanged());
        this._controller.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this._ticker is null) || !this._ticker!.isActive))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        global::Doroti.Ui.Rect handleRect = ((global::Doroti.Ui.Rect)(object?)_getHandleRect(((_SelectionHandleOverlay__text_selection)this.widget).type, ((_SelectionHandleOverlay__text_selection)this.widget).preferredLineHeight));
        global::Doroti.Ui.Rect interactiveRect = ((global::Doroti.Ui.Rect)(object?)(handleRect.isEmpty ? handleRect : handleRect.expandToInclude(global::Doroti.Ui.Rect.fromCircle(center: ((Offset)((dynamic)handleRect).center), radius: (global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension / 2L)))));
        global::Doroti.Framework.Rendering.RelativeRect paddingLocal = (interactiveRect.isEmpty ? global::Doroti.Framework.Rendering.RelativeRect.fill : new global::Doroti.Framework.Rendering.RelativeRect(Math.Max((((interactiveRect.width - handleRect.width)) / 2L), 0), Math.Max((((interactiveRect.height - handleRect.height)) / 2L), 0), Math.Max((((interactiveRect.width - handleRect.width)) / 2L), 0), Math.Max((((interactiveRect.height - handleRect.height)) / 2L), 0)));
        global::Doroti.Ui.Offset handleAnchor = ((global::Doroti.Ui.Offset)(object?)((_SelectionHandleOverlay__text_selection)this.widget).selectionControls.getHandleAnchor(((_SelectionHandleOverlay__text_selection)this.widget).type, ((_SelectionHandleOverlay__text_selection)this.widget).preferredLineHeight));
        bool eagerlyAcceptDragWhenCollapsed = ((object.Equals(((_SelectionHandleOverlay__text_selection)this.widget).type, global::Doroti.Framework.Rendering.TextSelectionHandleType.collapsed)) && (object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS)));
        return ((Widget)(object?)new CompositedTransformFollower(link: ((_SelectionHandleOverlay__text_selection)this.widget).handleLayerLink, offset: (-handleAnchor - new global::Doroti.Ui.Offset(((global::Doroti.Framework.Rendering.RelativeRect)paddingLocal).left, ((global::Doroti.Framework.Rendering.RelativeRect)paddingLocal).top)), showWhenUnlinked: false, child: new FadeTransition(opacity: this._opacity, child: new SizedBox(width: interactiveRect.width, height: interactiveRect.height, child: new Align(alignment: global::Doroti.Framework.Painting.Alignment.topLeft, child: new RawGestureDetector(behavior: global::Doroti.Framework.Rendering.HitTestBehavior.translucent, gestures: new DartMap<Type, dynamic>
        {
            [typeof(global::Doroti.Framework.Gestures.PanGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.PanGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.PanGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.PanGestureRecognizer(debugOwner: this, supportedDevices: new HashSet<PointerDeviceKind> { PointerDeviceKind.touch, PointerDeviceKind.stylus, PointerDeviceKind.unknown }))), ((global::System.Action<global::Doroti.Framework.Gestures.PanGestureRecognizer>)((instance) =>
            {
                DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.PanGestureRecognizer>)(() =>
                {
                    var __cascade = instance;
                    __cascade.dragStartBehavior = ((_SelectionHandleOverlay__text_selection)this.widget).dragStartBehavior;
                    __cascade.gestureSettings = (eagerlyAcceptDragWhenCollapsed ? new global::Doroti.Framework.Gestures.DeviceGestureSettings(touchSlop: 1.0) : null);
                    __cascade.onStart = ((_SelectionHandleOverlay__text_selection)this.widget).onSelectionHandleDragStart;
                    __cascade.onUpdate = ((_SelectionHandleOverlay__text_selection)this.widget).onSelectionHandleDragUpdate;
                    __cascade.onEnd = ((_SelectionHandleOverlay__text_selection)this.widget).onSelectionHandleDragEnd;
                    return __cascade;
                }))());
            })))
        }, child: new Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(left: ((global::Doroti.Framework.Rendering.RelativeRect)paddingLocal).left, top: ((global::Doroti.Framework.Rendering.RelativeRect)paddingLocal).top, right: ((global::Doroti.Framework.Rendering.RelativeRect)paddingLocal).right, bottom: ((global::Doroti.Framework.Rendering.RelativeRect)paddingLocal).bottom), child: ((_SelectionHandleOverlay__text_selection)this.widget).selectionControls.buildHandle(context, ((_SelectionHandleOverlay__text_selection)this.widget).type, ((_SelectionHandleOverlay__text_selection)this.widget).preferredLineHeight, () => ((_SelectionHandleOverlay__text_selection)this.widget).onSelectionHandleTapped()))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._ticker is null))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this.GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._ticker = new global::Doroti.Framework.Scheduler.Ticker((global::System.Action<Duration>)onTick, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
        _updateTickerModeNotifier();
        _updateTicker();
        return this._ticker!;
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
        TickerModeData values = this._tickerModeNotifier!.value;
        if ((this._ticker is not null))
        {
            this._ticker!.muted = !((TickerModeData)values).enabled;
            this._ticker!.forceFrames = ((TickerModeData)values).forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        newNotifier.addListener(() => this._updateTicker());
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = ((this._ticker?.isActive, this._ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) });
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", this._ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
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
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    this.editableText.showMagnifier(positionToShow);
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                break;
        }
    }

    internal virtual void _hideMagnifierIfSupportedByPlatform()
    {
        if (!this._isEditableTextMounted)
        {
            return;
        }
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    this.editableText.hideMagnifier();
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                break;
        }
    }

    internal virtual bool _lastSecondaryTapWasOnSelection
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).lastSecondaryTapDownPosition is not null));
            if ((((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).selection is null))
            {
                return false;
            }
            global::Doroti.Ui.TextPosition textPosition = ((global::Doroti.Ui.TextPosition)(object?)((TextPosition)((dynamic)this.renderEditable).getPositionForPoint(DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).lastSecondaryTapDownPosition))));
            return ((((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).selection!.start <= textPosition.offset) && (((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).selection!.end >= textPosition.offset));
            return default!;
        }
    }
    internal virtual bool _positionWasOnSelectionExclusive(TextPosition textPosition)
    {
        global::Doroti.Framework.Services.TextSelection? selectionLocal = ((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).selection;
        if ((selectionLocal is null))
        {
            return false;
        }
        return ((selectionLocal.start < textPosition.offset) && (selectionLocal.end > textPosition.offset));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _positionWasOnSelectionInclusive(TextPosition textPosition)
    {
        global::Doroti.Framework.Services.TextSelection? selectionLocal = ((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).selection;
        if ((selectionLocal is null))
        {
            return false;
        }
        return ((selectionLocal.start <= textPosition.offset) && (selectionLocal.end >= textPosition.offset));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _expandSelection(Offset offset, global::Doroti.Framework.Services.SelectionChangedCause cause, global::Doroti.Framework.Services.TextSelection? fromSelection = null)
    {
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).selection?.baseOffset is not null));
        global::Doroti.Ui.TextPosition tappedPosition = ((global::Doroti.Ui.TextPosition)(object?)((TextPosition)((dynamic)this.renderEditable).getPositionForPoint(offset)));
        global::Doroti.Framework.Services.TextSelection selectionLocal = (fromSelection ?? ((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).selection!);
        bool baseIsCloser = (((tappedPosition.offset - ((global::Doroti.Framework.Services.TextSelection)selectionLocal).baseOffset)).abs() < ((tappedPosition.offset - ((global::Doroti.Framework.Services.TextSelection)selectionLocal).extentOffset)).abs());
        global::Doroti.Framework.Services.TextSelection nextSelection = ((global::Doroti.Framework.Services.TextSelection)(object?)selectionLocal.copyWith(baseOffset: (baseIsCloser ? ((global::Doroti.Framework.Services.TextSelection)selectionLocal).extentOffset : ((global::Doroti.Framework.Services.TextSelection)selectionLocal).baseOffset), extentOffset: tappedPosition.offset));
        this.editableText.userUpdateTextEditingValue(((EditableTextState)this.editableText).textEditingValue.copyWith(selection: nextSelection), DartRuntimePrimitives.RequireValue(cause));
    }

    internal virtual void _extendSelection(Offset offset, global::Doroti.Framework.Services.SelectionChangedCause cause)
    {
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).selection?.baseOffset is not null));
        global::Doroti.Ui.TextPosition tappedPosition = ((global::Doroti.Ui.TextPosition)(object?)((TextPosition)((dynamic)this.renderEditable).getPositionForPoint(offset)));
        global::Doroti.Framework.Services.TextSelection selectionLocal = ((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).selection!;
        global::Doroti.Framework.Services.TextSelection nextSelection = ((global::Doroti.Framework.Services.TextSelection)(object?)selectionLocal.copyWith(extentOffset: tappedPosition.offset));
        this.editableText.userUpdateTextEditingValue(((EditableTextState)this.editableText).textEditingValue.copyWith(selection: nextSelection), DartRuntimePrimitives.RequireValue(cause));
    }

    public virtual bool shouldShowSelectionToolbar => this._shouldShowSelectionToolbar;
    public virtual bool shouldShowSelectionHandles => this._shouldShowSelectionHandles;
    public virtual EditableTextState editableText => DartRuntimePrimitives.ConvertValue<EditableTextState>(((TextSelectionGestureDetectorBuilderDelegate)this.@delegate).editableTextKey.currentState!);
    public virtual global::Doroti.Framework.Rendering.RenderEditable renderEditable => ((EditableTextState)this.editableText).renderEditable;
    internal virtual bool _isEditableTextMounted => DartRuntimePrimitives.ConvertValue<bool>((((TextSelectionGestureDetectorBuilderDelegate)this.@delegate).editableTextKey.currentContext?.mounted ?? false));
    internal virtual double _scrollPosition
    {
        get
        {
            ScrollableState? scrollableState = ((((TextSelectionGestureDetectorBuilderDelegate)this.@delegate).editableTextKey.currentContext is null) ? null : Scrollable.maybeOf(((TextSelectionGestureDetectorBuilderDelegate)this.@delegate).editableTextKey.currentContext!));
            return ((scrollableState is null) ? 0.0 : ((ScrollableState)scrollableState).position.pixels);
            return default!;
        }
    }
    internal virtual global::Doroti.Framework.Painting.AxisDirection? _scrollDirection
    {
        get
        {
            ScrollableState? scrollableState = ((((TextSelectionGestureDetectorBuilderDelegate)this.@delegate).editableTextKey.currentContext is null) ? null : Scrollable.maybeOf(((TextSelectionGestureDetectorBuilderDelegate)this.@delegate).editableTextKey.currentContext!));
            return scrollableState?.axisDirection;
            return default!;
        }
    }
    public virtual void onTapTrackStart()
    {
        _isShiftPressed = System.Linq.Enumerable.Any(global::Doroti.Framework.Services.HardwareKeyboard.instance.logicalKeysPressed.intersection(new HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> { global::Doroti.Framework.Services.LogicalKeyboardKey.shiftLeft, global::Doroti.Framework.Services.LogicalKeyboardKey.shiftRight }));
    }

    public virtual void onTapTrackReset()
    {
        _isShiftPressed = false;
    }

    public virtual void onTapDown(global::Doroti.Framework.Gestures.TapDragDownDetails details)
    {
        if (!((TextSelectionGestureDetectorBuilderDelegate)this.@delegate).selectionEnabled)
        {
            return;
        }
        this.renderEditable.handleTapDown(new global::Doroti.Framework.Gestures.TapDownDetails(globalPosition: ((global::Doroti.Framework.Gestures.TapDragDownDetails)details).globalPosition));
        global::Doroti.Ui.PointerDeviceKind? kindLocal = ((global::Doroti.Framework.Gestures.TapDragDownDetails)details).kind;
        _shouldShowSelectionToolbar = (((kindLocal is null) || (object.Equals(DartRuntimePrimitives.RequireValue(kindLocal), PointerDeviceKind.touch))) || (object.Equals(DartRuntimePrimitives.RequireValue(kindLocal), PointerDeviceKind.stylus)));
        _shouldShowSelectionHandles = this._shouldShowSelectionToolbar;
        bool isShiftPressedValid = (this._isShiftPressed && (((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).selection?.baseOffset is not null));
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
                {
                    if (this.editableText.widget.stylusHandwritingEnabled)
                    {
                        bool stylusEnabled = (kindLocal switch { PointerDeviceKind.stylus => this.editableText.widget.stylusHandwritingEnabled, PointerDeviceKind.invertedStylus => this.editableText.widget.stylusHandwritingEnabled, _ => false });
                        if (stylusEnabled)
                        {
                            DartRuntimePrimitives.Ignore(Scribe.isFeatureAvailable().then((global::System.Action<bool>)((isAvailable) =>
                            {
                                if (isAvailable)
                                {
                                    this.renderEditable.selectPosition(cause: global::Doroti.Framework.Services.SelectionChangedCause.stylusHandwriting);
                                    DartRuntimePrimitives.Ignore(Scribe.startStylusHandwriting());
                                }
                            })));
                        }
                    }
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    this.editableText.hideToolbar();
                    if (isShiftPressedValid)
                    {
                        global::Doroti.Framework.Services.TextSelection? fromSelection = (((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).hasFocus ? null : global::Doroti.Framework.Services.TextSelection.CreateCollapsed(offset: 0L));
                        _expandSelection(((global::Doroti.Framework.Gestures.TapDragDownDetails)details).globalPosition, global::Doroti.Framework.Services.SelectionChangedCause.tap, fromSelection);
                        return;
                    }
                    this.renderEditable.selectPosition(cause: global::Doroti.Framework.Services.SelectionChangedCause.tap);
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    this.editableText.hideToolbar();
                    if (isShiftPressedValid)
                    {
                        _extendSelection(((global::Doroti.Framework.Gestures.TapDragDownDetails)details).globalPosition, global::Doroti.Framework.Services.SelectionChangedCause.tap);
                        return;
                    }
                    this.renderEditable.selectPosition(cause: global::Doroti.Framework.Services.SelectionChangedCause.tap);
                    break;
                }
        }
    }

    public virtual void onForcePressStart(global::Doroti.Framework.Gestures.ForcePressDetails details)
    {
        DartRuntimePrimitives.Assert(() => ((TextSelectionGestureDetectorBuilderDelegate)this.@delegate).forcePressEnabled);
        _shouldShowSelectionToolbar = true;
        if (!((TextSelectionGestureDetectorBuilderDelegate)this.@delegate).selectionEnabled)
        {
            return;
        }
        this.renderEditable.selectWordsInRange(from: ((global::Doroti.Framework.Gestures.ForcePressDetails)details).globalPosition, cause: global::Doroti.Framework.Services.SelectionChangedCause.forcePress);
        this.editableText.showToolbar();
    }

    public virtual void onForcePressEnd(global::Doroti.Framework.Gestures.ForcePressDetails details)
    {
        DartRuntimePrimitives.Assert(() => ((TextSelectionGestureDetectorBuilderDelegate)this.@delegate).forcePressEnabled);
        this.renderEditable.selectWordsInRange(from: ((global::Doroti.Framework.Gestures.ForcePressDetails)details).globalPosition, cause: global::Doroti.Framework.Services.SelectionChangedCause.forcePress);
        if (this.shouldShowSelectionToolbar)
        {
            this.editableText.showToolbar();
        }
    }

    public virtual bool onUserTapAlwaysCalled => false;
    public virtual void onUserTap()
    {
    }

    public virtual void onSingleTapUp(global::Doroti.Framework.Gestures.TapDragUpDetails details)
    {
        if (!((TextSelectionGestureDetectorBuilderDelegate)this.@delegate).selectionEnabled)
        {
            this.editableText.requestKeyboard();
            return;
        }
        bool isShiftPressedValid = (this._isShiftPressed && (((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).selection?.baseOffset is not null));
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
                {
                    this.editableText.hideToolbar(false);
                    if (isShiftPressedValid)
                    {
                        _extendSelection(((global::Doroti.Framework.Gestures.TapDragUpDetails)details).globalPosition, global::Doroti.Framework.Services.SelectionChangedCause.tap);
                        return;
                    }
                    this.renderEditable.selectPosition(cause: global::Doroti.Framework.Services.SelectionChangedCause.tap);
                    this.editableText.showSpellCheckSuggestionsToolbar();
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                {
                    this.editableText.hideToolbar(false);
                    if (isShiftPressedValid)
                    {
                        _extendSelection(((global::Doroti.Framework.Gestures.TapDragUpDetails)details).globalPosition, global::Doroti.Framework.Services.SelectionChangedCause.tap);
                        return;
                    }
                    this.renderEditable.selectPosition(cause: global::Doroti.Framework.Services.SelectionChangedCause.tap);
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    if (isShiftPressedValid)
                    {
                        global::Doroti.Framework.Services.TextSelection? fromSelection = (((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).hasFocus ? null : global::Doroti.Framework.Services.TextSelection.CreateCollapsed(offset: 0L));
                        _expandSelection(((global::Doroti.Framework.Gestures.TapDragUpDetails)details).globalPosition, global::Doroti.Framework.Services.SelectionChangedCause.tap, fromSelection);
                        return;
                    }
                    switch (((global::Doroti.Framework.Gestures.TapDragUpDetails)details).kind)
                    {
                        case PointerDeviceKind.mouse:
                        case PointerDeviceKind.trackpad:
                        case PointerDeviceKind.stylus:
                        case PointerDeviceKind.invertedStylus:
                            {
                                this.renderEditable.selectPosition(cause: global::Doroti.Framework.Services.SelectionChangedCause.tap);
                                this.editableText.hideToolbar();
                                break;
                            }
                        case PointerDeviceKind.touch:
                        case PointerDeviceKind.unknown:
                            {
                                global::Doroti.Framework.Services.TextSelection previousSelection = (((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).selection ?? ((EditableTextState)this.editableText).textEditingValue.selection);
                                global::Doroti.Ui.TextPosition textPosition = ((global::Doroti.Ui.TextPosition)(object?)((TextPosition)((dynamic)this.renderEditable).getPositionForPoint(((global::Doroti.Framework.Gestures.TapDragUpDetails)details).globalPosition)));
                                var isAffinityTheSame = (object.Equals(textPosition.affinity, ((global::Doroti.Framework.Services.TextSelection)previousSelection).affinity));
                                var wordAtCursorIndexIsMisspelled = (this.editableText.findSuggestionSpanAtCursorIndex(textPosition.offset) is not null);
                                if (wordAtCursorIndexIsMisspelled)
                                {
                                    this.renderEditable.selectWord(cause: global::Doroti.Framework.Services.SelectionChangedCause.tap);
                                    if ((!object.Equals(previousSelection, ((EditableTextState)this.editableText).textEditingValue.selection)))
                                    {
                                        this.editableText.showSpellCheckSuggestionsToolbar();
                                    }
                                    else
                                    {
                                        this.editableText.toggleToolbar(false);
                                    }
                                }
                                else
                                {
                                    if ((((((_positionWasOnSelectionExclusive(textPosition) && !previousSelection.isCollapsed)) || ((((_positionWasOnSelectionInclusive(textPosition) && previousSelection.isCollapsed) && isAffinityTheSame) && !((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).readOnly)))) && ((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).hasFocus))
                                    {
                                        this.editableText.toggleToolbar(false);
                                    }
                                    else
                                    {
                                        this.renderEditable.selectWordEdge(cause: global::Doroti.Framework.Services.SelectionChangedCause.tap);
                                        if ((((object.Equals(previousSelection, ((EditableTextState)this.editableText).textEditingValue.selection)) && ((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).hasFocus) && !((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).readOnly))
                                        {
                                            this.editableText.toggleToolbar(false);
                                        }
                                        else
                                        {
                                            this.editableText.hideToolbar(false);
                                        }
                                    }
                                }
                                break;
                            }
                    }
                    break;
                }
        }
        this.editableText.requestKeyboard();
    }

    public virtual void onSingleTapCancel()
    {
    }

    public virtual void onSingleLongTapStart(global::Doroti.Framework.Gestures.LongPressStartDetails details)
    {
        if (!((TextSelectionGestureDetectorBuilderDelegate)this.@delegate).selectionEnabled)
        {
            return;
        }
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    if (!((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).hasFocus)
                    {
                        _longPressStartedWithoutFocus = true;
                        this.renderEditable.selectWord(cause: global::Doroti.Framework.Services.SelectionChangedCause.longPress);
                    }
                    else
                    {
                        if (((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).readOnly)
                        {
                            this.renderEditable.selectWord(cause: global::Doroti.Framework.Services.SelectionChangedCause.longPress);
                            if (this.editableText.context.mounted)
                            {
                                DartRuntimePrimitives.Ignore(Feedback.forLongPress(this.editableText.context));
                            }
                        }
                        else
                        {
                            this.renderEditable.selectPositionAt(from: ((global::Doroti.Framework.Gestures.LongPressStartDetails)details).globalPosition, cause: global::Doroti.Framework.Services.SelectionChangedCause.longPress);
                            var cursorPoint = new global::Doroti.Framework.Services.RawFloatingCursorPoint(state: global::Doroti.Framework.Services.FloatingCursorDragState.Start, startLocation: (((Offset)((dynamic)this.renderEditable).globalToLocal(((global::Doroti.Framework.Gestures.LongPressStartDetails)details).globalPosition)), new global::Doroti.Ui.TextPosition(offset: ((EditableTextState)this.editableText).textEditingValue.selection.baseOffset, affinity: ((EditableTextState)this.editableText).textEditingValue.selection.affinity)), offset: Offset.zero);
                            this.editableText.updateFloatingCursor(cursorPoint);
                        }
                    }
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    this.renderEditable.selectWord(cause: global::Doroti.Framework.Services.SelectionChangedCause.longPress);
                    if (this.editableText.context.mounted)
                    {
                        DartRuntimePrimitives.Ignore(Feedback.forLongPress(this.editableText.context));
                    }
                    break;
                }
        }
        _showMagnifierIfSupportedByPlatform(((global::Doroti.Framework.Gestures.LongPressStartDetails)details).globalPosition);
        _dragStartViewportOffset = ((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).offset.pixels;
        _dragStartScrollOffset = this._scrollPosition;
    }

    public virtual void onSingleLongTapMoveUpdate(global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails details)
    {
        if (!((TextSelectionGestureDetectorBuilderDelegate)this.@delegate).selectionEnabled)
        {
            return;
        }
        var editableOffset = ((((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).maxLines == 1L) ? new global::Doroti.Ui.Offset((((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).offset.pixels - this._dragStartViewportOffset), 0.0) : new global::Doroti.Ui.Offset(0.0, (((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).offset.pixels - this._dragStartViewportOffset)));
        global::Doroti.Ui.Offset scrollableOffset = ((global::Doroti.Ui.Offset)(object?)(global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis((this._scrollDirection ?? global::Doroti.Framework.Painting.AxisDirection.left)) switch { global::Doroti.Framework.Painting.Axis.horizontal => new global::Doroti.Ui.Offset((this._scrollPosition - this._dragStartScrollOffset), 0.0), global::Doroti.Framework.Painting.Axis.vertical => new global::Doroti.Ui.Offset(0.0, (this._scrollPosition - this._dragStartScrollOffset)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    if ((this._longPressStartedWithoutFocus || ((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).readOnly))
                    {
                        this.renderEditable.selectWordsInRange(from: (((((global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails)details).globalPosition - ((global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails)details).offsetFromOrigin) - editableOffset) - scrollableOffset), to: ((global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails)details).globalPosition, cause: global::Doroti.Framework.Services.SelectionChangedCause.longPress);
                    }
                    else
                    {
                        this.renderEditable.selectPositionAt(from: ((global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails)details).globalPosition, cause: global::Doroti.Framework.Services.SelectionChangedCause.longPress);
                        var cursorPoint = new global::Doroti.Framework.Services.RawFloatingCursorPoint(state: global::Doroti.Framework.Services.FloatingCursorDragState.Update, offset: ((global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails)details).offsetFromOrigin);
                        this.editableText.updateFloatingCursor(cursorPoint);
                    }
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    this.renderEditable.selectWordsInRange(from: (((((global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails)details).globalPosition - ((global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails)details).offsetFromOrigin) - editableOffset) - scrollableOffset), to: ((global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails)details).globalPosition, cause: global::Doroti.Framework.Services.SelectionChangedCause.longPress);
                    break;
                }
        }
        _showMagnifierIfSupportedByPlatform(((global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails)details).globalPosition);
    }

    public virtual void onSingleLongTapEnd(global::Doroti.Framework.Gestures.LongPressEndDetails details)
    {
        _onSingleLongTapEndOrCancel();
        if (this.shouldShowSelectionToolbar)
        {
            this.editableText.showToolbar();
        }
    }

    public virtual void onSingleLongTapCancel()
    {
        _onSingleLongTapEndOrCancel();
    }

    public virtual void onSecondaryTap()
    {
        if (!((TextSelectionGestureDetectorBuilderDelegate)this.@delegate).selectionEnabled)
        {
            return;
        }
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    if ((!this._lastSecondaryTapWasOnSelection || !((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).hasFocus))
                    {
                        this.renderEditable.selectWord(cause: global::Doroti.Framework.Services.SelectionChangedCause.tap);
                    }
                    if (this.shouldShowSelectionToolbar)
                    {
                        this.editableText.hideToolbar();
                        this.editableText.showToolbar();
                    }
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    if (!((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).hasFocus)
                    {
                        this.renderEditable.selectPosition(cause: global::Doroti.Framework.Services.SelectionChangedCause.tap);
                    }
                    this.editableText.toggleToolbar();
                    break;
                }
        }
    }

    public virtual void onSecondaryTapDown(global::Doroti.Framework.Gestures.TapDownDetails details)
    {
        this.renderEditable.handleSecondaryTapDown(new global::Doroti.Framework.Gestures.TapDownDetails(globalPosition: ((global::Doroti.Framework.Gestures.TapDownDetails)details).globalPosition));
        _shouldShowSelectionToolbar = true;
        _shouldShowSelectionHandles = (((((global::Doroti.Framework.Gestures.TapDownDetails)details).kind is null) || (object.Equals(((global::Doroti.Framework.Gestures.TapDownDetails)details).kind, PointerDeviceKind.touch))) || (object.Equals(((global::Doroti.Framework.Gestures.TapDownDetails)details).kind, PointerDeviceKind.stylus)));
    }

    public virtual void onDoubleTapDown(global::Doroti.Framework.Gestures.TapDragDownDetails details)
    {
        if (((TextSelectionGestureDetectorBuilderDelegate)this.@delegate).selectionEnabled)
        {
            this.renderEditable.selectWord(cause: global::Doroti.Framework.Services.SelectionChangedCause.doubleTap);
            if (this.shouldShowSelectionToolbar)
            {
                this.editableText.showToolbar();
            }
        }
    }

    internal virtual void _onSingleLongTapEndOrCancel()
    {
        _hideMagnifierIfSupportedByPlatform();
        _longPressStartedWithoutFocus = false;
        _dragStartViewportOffset = 0.0;
        _dragStartScrollOffset = 0.0;
        if ((((this._isEditableTextMounted && (object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS))) && ((TextSelectionGestureDetectorBuilderDelegate)this.@delegate).selectionEnabled) && ((EditableTextState)this.editableText).textEditingValue.selection.isCollapsed))
        {
            var cursorPoint = new global::Doroti.Framework.Services.RawFloatingCursorPoint(state: global::Doroti.Framework.Services.FloatingCursorDragState.End);
            this.editableText.updateFloatingCursor(cursorPoint);
        }
    }

    internal virtual void _selectParagraphsInRange(Offset from, Offset? to = null, global::Doroti.Framework.Services.SelectionChangedCause? cause = null)
    {
        global::Doroti.Framework.Services.TextBoundary paragraphBoundary = ((global::Doroti.Framework.Services.TextBoundary)(object?)new global::Doroti.Framework.Services.ParagraphBoundary(((EditableTextState)this.editableText).textEditingValue.text));
        _selectTextBoundariesInRange(boundary: paragraphBoundary, from: from, to: to, cause: cause);
    }

    internal virtual void _selectLinesInRange(Offset from, Offset? to = null, global::Doroti.Framework.Services.SelectionChangedCause? cause = null)
    {
        global::Doroti.Framework.Services.TextBoundary lineBoundary = ((global::Doroti.Framework.Services.TextBoundary)(object?)new global::Doroti.Framework.Services.LineBoundary(this.renderEditable));
        _selectTextBoundariesInRange(boundary: lineBoundary, from: from, to: to, cause: cause);
    }

    internal virtual global::Doroti.Ui.TextRange _moveToTextBoundary(TextPosition extent, global::Doroti.Framework.Services.TextBoundary textBoundary)
    {
        DartRuntimePrimitives.Assert(() => (extent.offset >= 0L));
        long startLocal = (textBoundary.getLeadingTextBoundaryAt(((extent.offset == ((EditableTextState)this.editableText).textEditingValue.text.Length) ? (extent.offset - 1L) : extent.offset)) ?? 0L);
        long endLocal = (textBoundary.getTrailingTextBoundaryAt(extent.offset) ?? ((EditableTextState)this.editableText).textEditingValue.text.Length);
        return ((global::Doroti.Ui.TextRange)(object?)new global::Doroti.Ui.TextRange(start: startLocal, end: endLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _selectTextBoundariesInRange(global::Doroti.Framework.Services.TextBoundary boundary, Offset from, Offset? to = null, global::Doroti.Framework.Services.SelectionChangedCause? cause = null)
    {
        global::Doroti.Ui.TextPosition fromPosition = ((global::Doroti.Ui.TextPosition)(object?)((TextPosition)((dynamic)this.renderEditable).getPositionForPoint(from)));
        global::Doroti.Ui.TextRange fromRange = ((global::Doroti.Ui.TextRange)(object?)_moveToTextBoundary(fromPosition, boundary));
        global::Doroti.Ui.TextPosition toPosition = ((global::Doroti.Ui.TextPosition)(object?)((to is null) ? fromPosition : ((TextPosition)((dynamic)this.renderEditable).getPositionForPoint(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(to))))));
        global::Doroti.Ui.TextRange toRange = ((global::Doroti.Ui.TextRange)(object?)((object.Equals(toPosition, fromPosition)) ? fromRange : _moveToTextBoundary(toPosition, boundary)));
        bool isFromBoundaryBeforeToBoundary = (fromRange.start < toRange.end);
        var newSelection = (isFromBoundaryBeforeToBoundary ? new global::Doroti.Framework.Services.TextSelection(baseOffset: fromRange.start, extentOffset: toRange.end) : new global::Doroti.Framework.Services.TextSelection(baseOffset: fromRange.end, extentOffset: toRange.start));
        this.editableText.userUpdateTextEditingValue(((EditableTextState)this.editableText).textEditingValue.copyWith(selection: newSelection), DartRuntimePrimitives.RequireValue(cause));
    }

    public virtual void onTripleTapDown(global::Doroti.Framework.Gestures.TapDragDownDetails details)
    {
        if (!((TextSelectionGestureDetectorBuilderDelegate)this.@delegate).selectionEnabled)
        {
            return;
        }
        if ((((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).maxLines == 1L))
        {
            this.editableText.selectAll(global::Doroti.Framework.Services.SelectionChangedCause.tap);
        }
        else
        {
            switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
            {
                case global::Doroti.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                    {
                        _selectParagraphsInRange(from: ((global::Doroti.Framework.Gestures.TapDragDownDetails)details).globalPosition, cause: global::Doroti.Framework.Services.SelectionChangedCause.tap);
                        break;
                    }
                case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                    {
                        _selectLinesInRange(from: ((global::Doroti.Framework.Gestures.TapDragDownDetails)details).globalPosition, cause: global::Doroti.Framework.Services.SelectionChangedCause.tap);
                        break;
                    }
            }
        }
        if (this.shouldShowSelectionToolbar)
        {
            this.editableText.showToolbar();
        }
    }

    public virtual void onDragSelectionStart(global::Doroti.Framework.Gestures.TapDragStartDetails details)
    {
        if (!((TextSelectionGestureDetectorBuilderDelegate)this.@delegate).selectionEnabled)
        {
            return;
        }
        global::Doroti.Ui.PointerDeviceKind? kindLocal = ((global::Doroti.Framework.Gestures.TapDragStartDetails)details).kind;
        _shouldShowSelectionToolbar = (((kindLocal is null) || (object.Equals(DartRuntimePrimitives.RequireValue(kindLocal), PointerDeviceKind.touch))) || (object.Equals(DartRuntimePrimitives.RequireValue(kindLocal), PointerDeviceKind.stylus)));
        _shouldShowSelectionHandles = this._shouldShowSelectionToolbar;
        _dragStartSelection = ((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).selection;
        _dragStartScrollOffset = this._scrollPosition;
        _dragStartViewportOffset = ((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).offset.pixels;
        if ((_TextSelectionGestureDetectorState__text_selection._getEffectiveConsecutiveTapCount(((global::Doroti.Framework.Gestures.TapDragStartDetails)details).consecutiveTapCount) > 1L))
        {
            return;
        }
        if (((this._isShiftPressed && (((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).selection is not null)) && ((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).selection!.isValid))
        {
            switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
            {
                case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                    {
                        _expandSelection(((global::Doroti.Framework.Gestures.TapDragStartDetails)details).globalPosition, global::Doroti.Framework.Services.SelectionChangedCause.drag);
                        break;
                    }
                case global::Doroti.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                    {
                        _extendSelection(((global::Doroti.Framework.Gestures.TapDragStartDetails)details).globalPosition, global::Doroti.Framework.Services.SelectionChangedCause.drag);
                        break;
                    }
            }
        }
        else
        {
            switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
            {
                case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                    {
                        switch (((global::Doroti.Framework.Gestures.TapDragStartDetails)details).kind)
                        {
                            case PointerDeviceKind.mouse:
                            case PointerDeviceKind.trackpad:
                                {
                                    this.renderEditable.selectPositionAt(from: ((global::Doroti.Framework.Gestures.TapDragStartDetails)details).globalPosition, cause: global::Doroti.Framework.Services.SelectionChangedCause.drag);
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
                case global::Doroti.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                    {
                        switch (((global::Doroti.Framework.Gestures.TapDragStartDetails)details).kind)
                        {
                            case PointerDeviceKind.mouse:
                            case PointerDeviceKind.trackpad:
                                {
                                    this.renderEditable.selectPositionAt(from: ((global::Doroti.Framework.Gestures.TapDragStartDetails)details).globalPosition, cause: global::Doroti.Framework.Services.SelectionChangedCause.drag);
                                    break;
                                }
                            case PointerDeviceKind.stylus:
                            case PointerDeviceKind.invertedStylus:
                            case PointerDeviceKind.touch:
                            case PointerDeviceKind.unknown:
                                {
                                    if (((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).hasFocus)
                                    {
                                        this.renderEditable.selectPositionAt(from: ((global::Doroti.Framework.Gestures.TapDragStartDetails)details).globalPosition, cause: global::Doroti.Framework.Services.SelectionChangedCause.drag);
                                        _showMagnifierIfSupportedByPlatform(((global::Doroti.Framework.Gestures.TapDragStartDetails)details).globalPosition);
                                    }
                                    break;
                                }
                            case null:
                                break;
                        }
                        break;
                    }
                case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                    {
                        this.renderEditable.selectPositionAt(from: ((global::Doroti.Framework.Gestures.TapDragStartDetails)details).globalPosition, cause: global::Doroti.Framework.Services.SelectionChangedCause.drag);
                        break;
                    }
            }
        }
    }

    public virtual void onDragSelectionUpdate(global::Doroti.Framework.Gestures.TapDragUpdateDetails details)
    {
        if (!((TextSelectionGestureDetectorBuilderDelegate)this.@delegate).selectionEnabled)
        {
            return;
        }
        if (!this._isShiftPressed)
        {
            var editableOffset = ((((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).maxLines == 1L) ? new global::Doroti.Ui.Offset((((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).offset.pixels - this._dragStartViewportOffset), 0.0) : new global::Doroti.Ui.Offset(0.0, (((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).offset.pixels - this._dragStartViewportOffset)));
            global::Doroti.Ui.Offset scrollableOffset = ((global::Doroti.Ui.Offset)(object?)(global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis((this._scrollDirection ?? global::Doroti.Framework.Painting.AxisDirection.left)) switch { global::Doroti.Framework.Painting.Axis.horizontal => new global::Doroti.Ui.Offset((this._scrollPosition - this._dragStartScrollOffset), 0.0), global::Doroti.Framework.Painting.Axis.vertical => new global::Doroti.Ui.Offset(0.0, (this._scrollPosition - this._dragStartScrollOffset)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
            global::Doroti.Ui.Offset dragStartGlobalPosition = ((global::Doroti.Ui.Offset)(object?)(((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).globalPosition - ((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).offsetFromOrigin));
            if ((_TextSelectionGestureDetectorState__text_selection._getEffectiveConsecutiveTapCount(((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).consecutiveTapCount) == 2L))
            {
                this.renderEditable.selectWordsInRange(from: ((dragStartGlobalPosition - editableOffset) - scrollableOffset), to: ((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).globalPosition, cause: global::Doroti.Framework.Services.SelectionChangedCause.drag);
                switch (((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).kind)
                {
                    case PointerDeviceKind.stylus:
                    case PointerDeviceKind.invertedStylus:
                    case PointerDeviceKind.touch:
                    case PointerDeviceKind.unknown:
                        {
                            _showMagnifierIfSupportedByPlatform(((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).globalPosition);
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
            if ((_TextSelectionGestureDetectorState__text_selection._getEffectiveConsecutiveTapCount(((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).consecutiveTapCount) == 3L))
            {
                switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
                {
                    case global::Doroti.Framework.Foundation.TargetPlatform.android:
                    case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                    case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                        {
                            switch (((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).kind)
                            {
                                case PointerDeviceKind.mouse:
                                case PointerDeviceKind.trackpad:
                                    {
                                        _selectParagraphsInRange(from: ((dragStartGlobalPosition - editableOffset) - scrollableOffset), to: ((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).globalPosition, cause: global::Doroti.Framework.Services.SelectionChangedCause.drag);
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
                    case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                        {
                            _selectLinesInRange(from: ((dragStartGlobalPosition - editableOffset) - scrollableOffset), to: ((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).globalPosition, cause: global::Doroti.Framework.Services.SelectionChangedCause.drag);
                            return;
                        }
                    case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                    case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                        {
                            _selectParagraphsInRange(from: ((dragStartGlobalPosition - editableOffset) - scrollableOffset), to: ((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).globalPosition, cause: global::Doroti.Framework.Services.SelectionChangedCause.drag);
                            return;
                        }
                    default:
                        throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                }
            }
            switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
            {
                case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                    {
                        switch (((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).kind)
                        {
                            case PointerDeviceKind.mouse:
                            case PointerDeviceKind.trackpad:
                                {
                                    this.renderEditable.selectPositionAt(from: ((dragStartGlobalPosition - editableOffset) - scrollableOffset), to: ((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).globalPosition, cause: global::Doroti.Framework.Services.SelectionChangedCause.drag);
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
                case global::Doroti.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                    {
                        switch (((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).kind)
                        {
                            case PointerDeviceKind.mouse:
                            case PointerDeviceKind.trackpad:
                            case PointerDeviceKind.stylus:
                            case PointerDeviceKind.invertedStylus:
                                {
                                    this.renderEditable.selectPositionAt(from: ((dragStartGlobalPosition - editableOffset) - scrollableOffset), to: ((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).globalPosition, cause: global::Doroti.Framework.Services.SelectionChangedCause.drag);
                                    return;
                                }
                            case PointerDeviceKind.touch:
                            case PointerDeviceKind.unknown:
                                {
                                    if (((global::Doroti.Framework.Rendering.RenderEditable)this.renderEditable).hasFocus)
                                    {
                                        this.renderEditable.selectPositionAt(from: ((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).globalPosition, cause: global::Doroti.Framework.Services.SelectionChangedCause.drag);
                                        _showMagnifierIfSupportedByPlatform(((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).globalPosition);
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
                case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                    {
                        this.renderEditable.selectPositionAt(from: ((dragStartGlobalPosition - editableOffset) - scrollableOffset), to: ((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).globalPosition, cause: global::Doroti.Framework.Services.SelectionChangedCause.drag);
                        return;
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
        if ((this._dragStartSelection!.isCollapsed || (((!object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS)) && (!object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.macOS))))))
        {
            _extendSelection(((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).globalPosition, global::Doroti.Framework.Services.SelectionChangedCause.drag);
            return;
        }
        global::Doroti.Framework.Services.TextSelection selectionLocal = ((EditableTextState)this.editableText).textEditingValue.selection;
        global::Doroti.Ui.TextPosition nextExtent = ((global::Doroti.Ui.TextPosition)(object?)((TextPosition)((dynamic)this.renderEditable).getPositionForPoint(((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).globalPosition)));
        bool isShiftTapDragSelectionForward = (this._dragStartSelection!.baseOffset < this._dragStartSelection!.extentOffset);
        bool isInverted = (isShiftTapDragSelectionForward ? (nextExtent.offset < this._dragStartSelection!.baseOffset) : (nextExtent.offset > this._dragStartSelection!.baseOffset));
        if ((isInverted && (((global::Doroti.Framework.Services.TextSelection)selectionLocal).baseOffset == this._dragStartSelection!.baseOffset)))
        {
            this.editableText.userUpdateTextEditingValue(((EditableTextState)this.editableText).textEditingValue.copyWith(selection: new global::Doroti.Framework.Services.TextSelection(baseOffset: this._dragStartSelection!.extentOffset, extentOffset: nextExtent.offset)), global::Doroti.Framework.Services.SelectionChangedCause.drag);
        }
        else
        {
            if (((!isInverted && (nextExtent.offset != this._dragStartSelection!.baseOffset)) && (((global::Doroti.Framework.Services.TextSelection)selectionLocal).baseOffset != this._dragStartSelection!.baseOffset)))
            {
                this.editableText.userUpdateTextEditingValue(((EditableTextState)this.editableText).textEditingValue.copyWith(selection: new global::Doroti.Framework.Services.TextSelection(baseOffset: this._dragStartSelection!.baseOffset, extentOffset: nextExtent.offset)), global::Doroti.Framework.Services.SelectionChangedCause.drag);
            }
            else
            {
                _extendSelection(((global::Doroti.Framework.Gestures.TapDragUpdateDetails)details).globalPosition, global::Doroti.Framework.Services.SelectionChangedCause.drag);
            }
        }
    }

    public virtual void onDragSelectionEnd(global::Doroti.Framework.Gestures.TapDragEndDetails details)
    {
        if ((this._shouldShowSelectionToolbar && (_TextSelectionGestureDetectorState__text_selection._getEffectiveConsecutiveTapCount(((global::Doroti.Framework.Gestures.TapDragEndDetails)details).consecutiveTapCount) == 2L)))
        {
            this.editableText.showToolbar();
        }
        if (this._isShiftPressed)
        {
            _dragStartSelection = null;
        }
        _hideMagnifierIfSupportedByPlatform();
    }

    public virtual Widget buildGestureDetector(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Rendering.HitTestBehavior? behavior = null, Widget child = default!)
    {
        return ((Widget)(object?)new TextSelectionGestureDetector(key: key, onTapTrackStart: () => this.onTapTrackStart(), onTapTrackReset: () => this.onTapTrackReset(), onTapDown: (global::System.Action<global::Doroti.Framework.Gestures.TapDragDownDetails>)this.onTapDown, onForcePressStart: ((global::System.Action<global::Doroti.Framework.Gestures.ForcePressDetails>)(((TextSelectionGestureDetectorBuilderDelegate)this.@delegate).forcePressEnabled ? this.onForcePressStart : null)), onForcePressEnd: ((global::System.Action<global::Doroti.Framework.Gestures.ForcePressDetails>)(((TextSelectionGestureDetectorBuilderDelegate)this.@delegate).forcePressEnabled ? this.onForcePressEnd : null)), onSecondaryTap: () => this.onSecondaryTap(), onSecondaryTapDown: (global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>)this.onSecondaryTapDown, onSingleTapUp: (global::System.Action<global::Doroti.Framework.Gestures.TapDragUpDetails>)this.onSingleTapUp, onSingleTapCancel: () => this.onSingleTapCancel(), onUserTap: () => this.onUserTap(), onSingleLongTapStart: (global::System.Action<global::Doroti.Framework.Gestures.LongPressStartDetails>)this.onSingleLongTapStart, onSingleLongTapMoveUpdate: (global::System.Action<global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails>)this.onSingleLongTapMoveUpdate, onSingleLongTapEnd: (global::System.Action<global::Doroti.Framework.Gestures.LongPressEndDetails>)this.onSingleLongTapEnd, onSingleLongTapCancel: () => this.onSingleLongTapCancel(), onDoubleTapDown: (global::System.Action<global::Doroti.Framework.Gestures.TapDragDownDetails>)this.onDoubleTapDown, onTripleTapDown: (global::System.Action<global::Doroti.Framework.Gestures.TapDragDownDetails>)this.onTripleTapDown, onDragSelectionStart: (global::System.Action<global::Doroti.Framework.Gestures.TapDragStartDetails>)this.onDragSelectionStart, onDragSelectionUpdate: (global::System.Action<global::Doroti.Framework.Gestures.TapDragUpdateDetails>)this.onDragSelectionUpdate, onDragSelectionEnd: (global::System.Action<global::Doroti.Framework.Gestures.TapDragEndDetails>)this.onDragSelectionEnd, onUserTapAlwaysCalled: this.onUserTapAlwaysCalled, behavior: behavior, child: child));
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
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                {
                    return ((rawCount <= 3L) ? rawCount : ((((rawCount % 3L) == 0L) ? 3L : (rawCount % 3L))));
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    return Math.Min(rawCount, 3L);
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    return ((rawCount < 2L) ? rawCount : (2L + (rawCount % 2L)));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleTapTrackStart()
    {
        ((TextSelectionGestureDetector)this.widget).onTapTrackStart?.Invoke();
    }

    internal virtual void _handleTapTrackReset()
    {
        ((TextSelectionGestureDetector)this.widget).onTapTrackReset?.Invoke();
    }

    internal virtual void _handleTapDown(global::Doroti.Framework.Gestures.TapDragDownDetails details)
    {
        ((TextSelectionGestureDetector)this.widget).onTapDown?.Invoke(details);
        if ((_TextSelectionGestureDetectorState__text_selection._getEffectiveConsecutiveTapCount(((global::Doroti.Framework.Gestures.TapDragDownDetails)details).consecutiveTapCount) == 2L))
        {
            ((TextSelectionGestureDetector)this.widget).onDoubleTapDown?.Invoke(details);
            return;
        }
        if ((_TextSelectionGestureDetectorState__text_selection._getEffectiveConsecutiveTapCount(((global::Doroti.Framework.Gestures.TapDragDownDetails)details).consecutiveTapCount) == 3L))
        {
            ((TextSelectionGestureDetector)this.widget).onTripleTapDown?.Invoke(details);
            return;
        }
    }

    internal virtual void _handleTapUp(global::Doroti.Framework.Gestures.TapDragUpDetails details)
    {
        if ((_TextSelectionGestureDetectorState__text_selection._getEffectiveConsecutiveTapCount(((global::Doroti.Framework.Gestures.TapDragUpDetails)details).consecutiveTapCount) == 1L))
        {
            ((TextSelectionGestureDetector)this.widget).onSingleTapUp?.Invoke(details);
            ((TextSelectionGestureDetector)this.widget).onUserTap?.Invoke();
        }
        else
        {
            if (((TextSelectionGestureDetector)this.widget).onUserTapAlwaysCalled)
            {
                ((TextSelectionGestureDetector)this.widget).onUserTap?.Invoke();
            }
        }
    }

    internal virtual void _handleTapCancel()
    {
        ((TextSelectionGestureDetector)this.widget).onSingleTapCancel?.Invoke();
    }

    internal virtual void _handleDragStart(global::Doroti.Framework.Gestures.TapDragStartDetails details)
    {
        ((TextSelectionGestureDetector)this.widget).onDragSelectionStart?.Invoke(details);
    }

    internal virtual void _handleDragUpdate(global::Doroti.Framework.Gestures.TapDragUpdateDetails details)
    {
        ((TextSelectionGestureDetector)this.widget).onDragSelectionUpdate?.Invoke(details);
    }

    internal virtual void _handleDragEnd(global::Doroti.Framework.Gestures.TapDragEndDetails details)
    {
        ((TextSelectionGestureDetector)this.widget).onDragSelectionEnd?.Invoke(details);
    }

    internal virtual void _forcePressStarted(global::Doroti.Framework.Gestures.ForcePressDetails details)
    {
        ((TextSelectionGestureDetector)this.widget).onForcePressStart?.Invoke(details);
    }

    internal virtual void _forcePressEnded(global::Doroti.Framework.Gestures.ForcePressDetails details)
    {
        ((TextSelectionGestureDetector)this.widget).onForcePressEnd?.Invoke(details);
    }

    internal virtual void _handleLongPressStart(global::Doroti.Framework.Gestures.LongPressStartDetails details)
    {
        ((TextSelectionGestureDetector)this.widget).onSingleLongTapStart?.Invoke(details);
    }

    internal virtual void _handleLongPressMoveUpdate(global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails details)
    {
        ((TextSelectionGestureDetector)this.widget).onSingleLongTapMoveUpdate?.Invoke(details);
    }

    internal virtual void _handleLongPressEnd(global::Doroti.Framework.Gestures.LongPressEndDetails details)
    {
        ((TextSelectionGestureDetector)this.widget).onSingleLongTapEnd?.Invoke(details);
    }

    internal virtual void _handleLongPressCancel()
    {
        ((TextSelectionGestureDetector)this.widget).onSingleLongTapCancel?.Invoke();
    }

    public override Widget build(BuildContext context)
    {
        var gesturesLocal = new DartMap<Type, dynamic>();
        gesturesLocal[typeof(global::Doroti.Framework.Gestures.TapGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.TapGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.TapGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.TapGestureRecognizer(debugOwner: this))), ((global::System.Action<global::Doroti.Framework.Gestures.TapGestureRecognizer>)((instance) =>
        {
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.TapGestureRecognizer>)(() =>
            {
                var __cascade = instance;
                __cascade.onSecondaryTap = ((TextSelectionGestureDetector)this.widget).onSecondaryTap;
                __cascade.onSecondaryTapDown = ((TextSelectionGestureDetector)this.widget).onSecondaryTapDown;
                return __cascade;
            }))());
        })));
        if (((((((TextSelectionGestureDetector)this.widget).onSingleLongTapStart is not null) || (((TextSelectionGestureDetector)this.widget).onSingleLongTapMoveUpdate is not null)) || (((TextSelectionGestureDetector)this.widget).onSingleLongTapEnd is not null)) || (((TextSelectionGestureDetector)this.widget).onSingleLongTapCancel is not null)))
        {
            gesturesLocal[typeof(global::Doroti.Framework.Gestures.LongPressGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.LongPressGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.LongPressGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.LongPressGestureRecognizer(debugOwner: this, supportedDevices: new HashSet<PointerDeviceKind> { PointerDeviceKind.touch }))), ((global::System.Action<global::Doroti.Framework.Gestures.LongPressGestureRecognizer>)((instance) =>
            {
                DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.LongPressGestureRecognizer>)(() =>
                {
                    var __cascade = instance;
                    __cascade.onLongPressStart = this._handleLongPressStart;
                    __cascade.onLongPressMoveUpdate = this._handleLongPressMoveUpdate;
                    __cascade.onLongPressEnd = this._handleLongPressEnd;
                    __cascade.onLongPressCancel = this._handleLongPressCancel;
                    return __cascade;
                }))());
            })));
        }
        if ((((((TextSelectionGestureDetector)this.widget).onDragSelectionStart is not null) || (((TextSelectionGestureDetector)this.widget).onDragSelectionUpdate is not null)) || (((TextSelectionGestureDetector)this.widget).onDragSelectionEnd is not null)))
        {
            switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
            {
                case global::Doroti.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                    {
                        gesturesLocal[typeof(global::Doroti.Framework.Gestures.TapAndHorizontalDragGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.TapAndHorizontalDragGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.TapAndHorizontalDragGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.TapAndHorizontalDragGestureRecognizer(debugOwner: this))), ((global::System.Action<global::Doroti.Framework.Gestures.TapAndHorizontalDragGestureRecognizer>)((instance) =>
                        {
                            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.TapAndHorizontalDragGestureRecognizer>)(() =>
                            {
                                var __cascade = instance;
                                __cascade.dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.down;
                                __cascade.eagerVictoryOnDrag = (!object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS));
                                __cascade.onTapTrackStart = this._handleTapTrackStart;
                                __cascade.onTapTrackReset = this._handleTapTrackReset;
                                __cascade.onTapDown = this._handleTapDown;
                                __cascade.onDragStart = this._handleDragStart;
                                __cascade.onDragUpdate = this._handleDragUpdate;
                                __cascade.onDragEnd = this._handleDragEnd;
                                __cascade.onTapUp = this._handleTapUp;
                                __cascade.onCancel = this._handleTapCancel;
                                return __cascade;
                            }))());
                        })));
                        break;
                    }
                case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                    {
                        gesturesLocal[typeof(global::Doroti.Framework.Gestures.TapAndPanGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.TapAndPanGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.TapAndPanGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.TapAndPanGestureRecognizer(debugOwner: this))), ((global::System.Action<global::Doroti.Framework.Gestures.TapAndPanGestureRecognizer>)((instance) =>
                        {
                            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.TapAndPanGestureRecognizer>)(() =>
                            {
                                var __cascade = instance;
                                __cascade.dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.down;
                                __cascade.onTapTrackStart = this._handleTapTrackStart;
                                __cascade.onTapTrackReset = this._handleTapTrackReset;
                                __cascade.onTapDown = this._handleTapDown;
                                __cascade.onDragStart = this._handleDragStart;
                                __cascade.onDragUpdate = this._handleDragUpdate;
                                __cascade.onDragEnd = this._handleDragEnd;
                                __cascade.onTapUp = this._handleTapUp;
                                __cascade.onCancel = this._handleTapCancel;
                                return __cascade;
                            }))());
                        })));
                        break;
                    }
            }
        }
        if (((((TextSelectionGestureDetector)this.widget).onForcePressStart is not null) || (((TextSelectionGestureDetector)this.widget).onForcePressEnd is not null)))
        {
            gesturesLocal[typeof(global::Doroti.Framework.Gestures.ForcePressGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.ForcePressGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.ForcePressGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.ForcePressGestureRecognizer(debugOwner: this))), ((global::System.Action<global::Doroti.Framework.Gestures.ForcePressGestureRecognizer>)((instance) =>
            {
                DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.ForcePressGestureRecognizer>)(() =>
                {
                    var __cascade = instance;
                    __cascade.onStart = ((global::System.Action<global::Doroti.Framework.Gestures.ForcePressDetails>)((((TextSelectionGestureDetector)this.widget).onForcePressStart is not null) ? this._forcePressStarted : null));
                    __cascade.onEnd = ((global::System.Action<global::Doroti.Framework.Gestures.ForcePressDetails>)((((TextSelectionGestureDetector)this.widget).onForcePressEnd is not null) ? this._forcePressEnded : null));
                    return __cascade;
                }))());
            })));
        }
        return ((Widget)(object?)new RawGestureDetector(gestures: gesturesLocal, excludeFromSemantics: true, behavior: ((TextSelectionGestureDetector)this.widget).behavior, child: ((TextSelectionGestureDetector)this.widget).child));
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
        if (this._disposed)
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
            if (this._disposed)
            {
                return;
            }
            value = ClipboardStatus.unknown;
            return;
        }
        ClipboardStatus nextStatus = (hasStringsLocal ? ClipboardStatus.pasteable : ClipboardStatus.notPasteable);
        if (this._disposed)
        {
            return;
        }
        value = nextStatus;
    }

    public virtual void addListener(global::System.Action listener)
    {
        if (!this.hasListeners)
        {
            WidgetsBinding.instance.addObserver(this);
        }
        if ((object.Equals(this.value, ClipboardStatus.unknown)))
        {
            DartRuntimePrimitives.Ignore(update());
        }
        base.addListener(() => listener());
    }

    public virtual void removeListener(global::System.Action listener)
    {
        base.removeListener(() => listener());
        if ((!this._disposed && !this.hasListeners))
        {
            WidgetsBinding.instance.removeObserver(this);
        }
    }

    public virtual void didChangeAppLifecycleState(AppLifecycleState state)
    {
        switch (state)
        {
            case var __constant148294 when (object.Equals(__constant148294, AppLifecycleState.resumed)):
                {
                    DartRuntimePrimitives.Ignore(update());
                    break;
                }
            case var __constant148350 when (object.Equals(__constant148350, AppLifecycleState.detached)):
            case var __constant148389 when (object.Equals(__constant148389, AppLifecycleState.inactive)):
            case var __constant148428 when (object.Equals(__constant148428, AppLifecycleState.hidden)):
            case var __constant148465 when (object.Equals(__constant148465, AppLifecycleState.paused)):
                {
                    break;
                }
        }
    }

    public virtual void dispose()
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
        if (this._disposed)
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
            if ((this._disposed || (object.Equals(this.value, LiveTextInputStatus.unknown))))
            {
                return;
            }
            value = LiveTextInputStatus.unknown;
            return;
        }
        LiveTextInputStatus nextStatus = (isLiveTextInputEnabled ? LiveTextInputStatus.enabled : LiveTextInputStatus.disabled);
        if ((this._disposed || (object.Equals(nextStatus, this.value))))
        {
            return;
        }
        value = nextStatus;
    }

    public virtual void addListener(global::System.Action listener)
    {
        if (!this.hasListeners)
        {
            WidgetsBinding.instance.addObserver(this);
        }
        if ((object.Equals(this.value, LiveTextInputStatus.unknown)))
        {
            DartRuntimePrimitives.Ignore(update());
        }
        base.addListener(() => listener());
    }

    public virtual void removeListener(global::System.Action listener)
    {
        base.removeListener(() => listener());
        if ((!this._disposed && !this.hasListeners))
        {
            WidgetsBinding.instance.removeObserver(this);
        }
    }

    public virtual void didChangeAppLifecycleState(AppLifecycleState state)
    {
        switch (state)
        {
            case var __constant151548 when (object.Equals(__constant151548, AppLifecycleState.resumed)):
                {
                    DartRuntimePrimitives.Ignore(update());
                    break;
                }
            case var __constant151604 when (object.Equals(__constant151604, AppLifecycleState.detached)):
            case var __constant151643 when (object.Equals(__constant151643, AppLifecycleState.inactive)):
            case var __constant151682 when (object.Equals(__constant151682, AppLifecycleState.paused)):
            case var __constant151719 when (object.Equals(__constant151719, AppLifecycleState.hidden)):
                break;
        }
    }

    public virtual void dispose()
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

public abstract class TextSelectionHandleControls : TextSelectionControls
{
    public override Widget buildToolbar(BuildContext context, Rect globalEditableRegion, double textLineHeight, Offset selectionMidpoint, List<global::Doroti.Framework.Rendering.TextSelectionPoint> endpoints, global::Doroti.Framework.Services.TextSelectionDelegate @delegate, global::Doroti.Framework.Foundation.ValueListenable<ClipboardStatus>? clipboardStatus, Offset? lastSecondaryTapDownPosition) => DartRuntimePrimitives.ConvertValue<Widget>(SizedBox.CreateShrink());
    public virtual bool canCut(global::Doroti.Framework.Services.TextSelectionDelegate @delegate) => false;
    public virtual bool canCopy(global::Doroti.Framework.Services.TextSelectionDelegate @delegate) => false;
    public virtual bool canPaste(global::Doroti.Framework.Services.TextSelectionDelegate @delegate) => false;
    public virtual bool canSelectAll(global::Doroti.Framework.Services.TextSelectionDelegate @delegate) => false;
    public virtual void handleCut(global::Doroti.Framework.Services.TextSelectionDelegate @delegate, ClipboardStatusNotifier? clipboardStatus = null)
    {
    }

    public virtual void handleCopy(global::Doroti.Framework.Services.TextSelectionDelegate @delegate, ClipboardStatusNotifier? clipboardStatus = null)
    {
    }

    public async virtual Future handlePaste(global::Doroti.Framework.Services.TextSelectionDelegate @delegate)
    {
    }

    public virtual void handleSelectAll(global::Doroti.Framework.Services.TextSelectionDelegate @delegate)
    {
    }

}
