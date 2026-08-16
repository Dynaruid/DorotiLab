// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/editable.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Rendering;

public static partial class EditableLibrary
{
    internal static double _kCaretGap = 1.0;
}

public static partial class EditableLibrary
{
    internal static double _kCaretHeightOffset = 2.0;
}

public static partial class EditableLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsets _kFloatingCursorSizeIncrease = global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 0.5, vertical: 1.0);
}

public static partial class EditableLibrary
{
    internal static Radius _kFloatingCursorRadius = global::Doroti.Ui.Radius.circular(1.0);
}

public static partial class EditableLibrary
{
    internal static double _kShortestDistanceSquaredWithFloatingAndRegularCursors = (15.0 * 15.0);
}

public class TextSelectionPoint
{
    public virtual Offset point { get; private set; } = default!;
    public virtual TextDirection? direction { get; private set; }

    public TextSelectionPoint(Offset point, TextDirection? direction)
    {
        this.point = point;
        this.direction = direction;
    }

    public override bool Equals(object? other)
    {
        var __other = other as TextSelectionPoint;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is TextSelectionPoint) && (object.Equals(((TextSelectionPoint)((TextSelectionPoint)__other)).point, this.point))) && (object.Equals(((TextSelectionPoint)((TextSelectionPoint)__other)).direction, this.direction)));
    }

    public override string ToString()
    {
        return (this.direction switch { TextDirection.ltr => $"{this.point}-ltr", TextDirection.rtl => $"{this.point}-rtl", null => $"{this.point}", _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.point, this.direction);
}

public class VerticalCaretMovementRun : IEnumerator<TextPosition>
{
    internal virtual Offset _currentOffset { get; set; } = default!;
    internal virtual long _currentLine { get; set; } = default!;
    internal virtual TextPosition _currentTextPosition { get; set; } = default!;
    internal virtual List<LineMetrics> _lineMetrics { get; private set; } = default!;
    internal virtual RenderEditable _editable { get; private set; } = default!;
    internal virtual bool _isValid { get; set; } = true;
    internal virtual DartMap<long, MapEntry<Offset, TextPosition>> _positionCache { get; private set; } = new DartMap<long, MapEntry<Offset, TextPosition>>();

    public VerticalCaretMovementRun(RenderEditable _editable, List<LineMetrics> _lineMetrics, TextPosition _currentTextPosition, long _currentLine, Offset _currentOffset)
    {
        this._editable = _editable;
        this._lineMetrics = _lineMetrics;
        this._currentTextPosition = _currentTextPosition;
        this._currentLine = _currentLine;
        this._currentOffset = _currentOffset;
    }

    public virtual bool isValid
    {
        get
        {
            if (!this._isValid)
            {
                return false;
            }
            List<global::Doroti.Ui.LineMetrics> newLineMetrics__6310 = ((RenderEditable)this._editable)._textPainter.computeLineMetrics();
            if (!DartRuntimePrimitives.Identical(newLineMetrics__6310, this._lineMetrics))
            {
                _isValid = false;
            }
            return this._isValid;
            return default!;
        }
    }
    internal virtual MapEntry<global::Doroti.Ui.Offset, global::Doroti.Ui.TextPosition> _getTextPositionForLine(long lineNumber)
    {
        DartRuntimePrimitives.Assert(() => this.isValid);
        DartRuntimePrimitives.Assert(() => (lineNumber >= 0L));
        MapEntry<global::Doroti.Ui.Offset, global::Doroti.Ui.TextPosition>? cachedPosition__6901 = this._positionCache.GetValueOrDefault(lineNumber);
        if ((cachedPosition__6901 is not null))
        {
            MapEntry<Offset, TextPosition> cachedPosition__6901__value6954 = DartRuntimePrimitives.RequireValue(cachedPosition__6901);
            return DartRuntimePrimitives.RequireValue(cachedPosition__6901__value6954);
        }
        DartRuntimePrimitives.Assert(() => (lineNumber != this._currentLine));
        var newOffset__7066 = new global::Doroti.Ui.Offset(this._currentOffset.dx, this._lineMetrics[(int)(lineNumber)].baseline);
        global::Doroti.Ui.TextPosition closestPosition__7163 = ((RenderEditable)this._editable)._textPainter.getPositionForOffset(newOffset__7066);
        var position__7247 = new MapEntry<global::Doroti.Ui.Offset, global::Doroti.Ui.TextPosition>(newOffset__7066, closestPosition__7163);
        this._positionCache[lineNumber] = position__7247;
        return position__7247;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.TextPosition current
    {
        get
        {
            DartRuntimePrimitives.Assert(() => this.isValid);
            return this._currentTextPosition;
            return default!;
        }
    }
    public virtual bool moveNext()
    {
        DartRuntimePrimitives.Assert(() => this.isValid);
        if (((this._currentLine + 1L) >= checked((long)(this._lineMetrics.Count))))
        {
            return false;
        }
        MapEntry<global::Doroti.Ui.Offset, global::Doroti.Ui.TextPosition> position__7658 = _getTextPositionForLine((this._currentLine + 1L));
        _currentLine += 1L;
        _currentOffset = position__7658.key;
        _currentTextPosition = position__7658.value;
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool movePrevious()
    {
        DartRuntimePrimitives.Assert(() => this.isValid);
        if ((this._currentLine <= 0L))
        {
            return false;
        }
        MapEntry<global::Doroti.Ui.Offset, global::Doroti.Ui.TextPosition> position__8079 = _getTextPositionForLine((this._currentLine - 1L));
        _currentLine -= 1L;
        _currentOffset = position__8079.key;
        _currentTextPosition = position__8079.value;
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool moveByOffset(double offset)
    {
        global::Doroti.Ui.Offset initialOffset__8539 = this._currentOffset;
        if ((offset >= 0.0))
        {
            while ((this._currentOffset.dy < (initialOffset__8539.dy + offset)))
            {
                if (!moveNext())
                {
                    break;
                }
            }
        }
        else
        {
            while ((this._currentOffset.dy > (initialOffset__8539.dy + offset)))
            {
                if (!movePrevious())
                {
                    break;
                }
            }
        }
        return (!object.Equals(initialOffset__8539, this._currentOffset));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    TextPosition IEnumerator<TextPosition>.Current => current;
    object System.Collections.IEnumerator.Current => current!;
    bool System.Collections.IEnumerator.MoveNext() => moveNext();
    void System.Collections.IEnumerator.Reset() => throw new NotSupportedException();
    void IDisposable.Dispose() { }
}

public class RenderEditable : RenderBox, RelayoutWhenSystemFontsChangeMixin, ContainerRenderObjectMixin<RenderBox, TextParentData>, RenderInlineChildrenContainerDefaults, TextLayoutMetrics
{
    internal virtual _RenderEditableCustomPaint__editable? _foregroundRenderObject { get; set; } = default;
    internal virtual _RenderEditableCustomPaint__editable? _backgroundRenderObject { get; set; } = default;
    internal virtual RenderEditablePainter? _foregroundPainter { get; set; } = default;
    internal virtual RenderEditablePainter? _painter { get; set; } = default;
    private bool __late__caretPainter_initialized;
    private _CaretPainter__editable __late__caretPainter = default!;
    internal virtual _CaretPainter__editable _caretPainter
    {
        get
        {
            if (!__late__caretPainter_initialized)
            {
                __late__caretPainter = new _CaretPainter__editable();
                __late__caretPainter_initialized = true;
            }
            return __late__caretPainter;
        }
    }
    internal virtual _TextHighlightPainter__editable _selectionPainter { get; private set; } = new _TextHighlightPainter__editable();
    internal virtual _TextHighlightPainter__editable _autocorrectHighlightPainter { get; private set; } = new _TextHighlightPainter__editable();
    internal virtual _CompositeRenderEditablePainter__editable? _cachedBuiltInForegroundPainters { get; set; } = default;
    internal virtual _CompositeRenderEditablePainter__editable? _cachedBuiltInPainters { get; set; } = default;
    public virtual bool ignorePointer { get; set; } = default!;
    internal virtual double _devicePixelRatio { get; set; } = default!;
    internal virtual string _obscuringCharacter { get; set; } = default!;
    internal virtual bool _obscureText { get; set; } = default!;
    public virtual TextSelectionDelegate textSelectionDelegate { get; set; } = default!;
    internal virtual ValueNotifier<bool> _selectionStartInViewport { get; private set; } = new ValueNotifier<bool>(true);
    internal virtual ValueNotifier<bool> _selectionEndInViewport { get; private set; } = new ValueNotifier<bool>(true);
    internal virtual global::Doroti.Framework.Painting.TextPainter _textPainter { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Semantics.AttributedString? _cachedAttributedValue { get; set; } = default;
    internal virtual List<global::Doroti.Framework.Painting.InlineSpanSemanticsInformation>? _cachedCombinedSemanticsInfos { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.TextPainter? _textIntrinsicsCache { get; set; } = default;
    internal virtual bool _disposeShowCursor { get; set; } = default!;
    internal virtual ValueNotifier<bool> _showCursor { get; set; } = default!;
    internal virtual bool _hasFocus { get; set; } = false;
    internal virtual bool _forceLine { get; set; } = false;
    internal virtual bool _readOnly { get; set; } = false;
    internal virtual long? _maxLines { get; set; } = default;
    internal virtual long? _minLines { get; set; } = default;
    internal virtual bool _expands { get; set; } = default!;
    internal virtual TextSelection? _selection { get; set; } = default;
    internal virtual ViewportOffset _offset { get; set; } = default!;
    internal virtual double _cursorWidth { get; set; } = 1.0;
    internal virtual double? _cursorHeight { get; set; } = default;
    internal virtual bool _paintCursorOnTop { get; set; } = default!;
    internal virtual LayerLink _startHandleLayerLink { get; set; } = default!;
    internal virtual LayerLink _endHandleLayerLink { get; set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsets floatingCursorAddedMargin { get; set; } = default!;
    internal virtual bool _floatingCursorOn { get; set; } = false;
    internal virtual TextPosition _floatingCursorTextPosition { get; set; } = default!;
    internal virtual bool? _enableInteractiveSelection { get; set; } = default;
    internal virtual double _maxScrollExtent { get; set; } = 0;
    internal virtual Clip _clipBehavior { get; set; } = Clip.hardEdge;
    internal virtual List<global::Doroti.Framework.Painting.InlineSpanSemanticsInformation>? _semanticsInfo { get; set; } = default;
    internal virtual DartMap<Key, global::Doroti.Framework.Semantics.SemanticsNode>? _cachedChildNodes { get; set; } = default;
    internal virtual long? _cachedLineBreakCount { get; set; } = default;
    internal virtual TapGestureRecognizer _tap { get; set; } = default!;
    internal virtual LongPressGestureRecognizer _longPress { get; set; } = default!;
    internal virtual Offset? _lastTapDownPosition { get; set; } = default;
    internal virtual Offset? _lastSecondaryTapDownPosition { get; set; } = default;
    internal virtual List<global::Doroti.Framework.Painting.PlaceholderDimensions>? _placeholderDimensions { get; set; } = default;
    internal virtual Rect _caretPrototype { get; set; } = default!;
    internal virtual Offset _relativeOrigin { get; set; } = Offset.zero;
    internal virtual Offset? _previousOffset { get; set; } = default;
    internal virtual bool _shouldResetOrigin { get; set; } = true;
    internal virtual bool _resetOriginOnLeft { get; set; } = false;
    internal virtual bool _resetOriginOnRight { get; set; } = false;
    internal virtual bool _resetOriginOnTop { get; set; } = false;
    internal virtual bool _resetOriginOnBottom { get; set; } = false;
    internal virtual double? _resetFloatingCursorAnimationValue { get; set; } = default;
    internal virtual LayerHandle<LeaderLayer> _leaderLayerHandler { get; private set; } = new LayerHandle<LeaderLayer>();
    internal virtual LayerHandle<ClipRectLayer> _clipRectLayer { get; private set; } = new LayerHandle<ClipRectLayer>();
    public virtual bool _hasPendingSystemFontsDidChangeCallBack { get; set; } = false;
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    public RenderEditable(global::Doroti.Framework.Painting.InlineSpan? text = null, TextDirection textDirection = default!, TextAlign textAlign = TextAlign.start, Color? cursorColor = null, Color? backgroundCursorColor = null, ValueNotifier<bool>? showCursor = null, bool? hasFocus = null, LayerLink startHandleLayerLink = default!, LayerLink endHandleLayerLink = default!, long? maxLines = 1, long? minLines = null, bool expands = false, global::Doroti.Framework.Painting.StrutStyle? strutStyle = null, Color? selectionColor = null, double textScaleFactor = 1.0, global::Doroti.Framework.Painting.TextScaler textScaler = default!, TextSelection? selection = null, ViewportOffset offset = default!, bool ignorePointer = false, bool readOnly = false, bool forceLine = true, TextHeightBehavior? textHeightBehavior = null, global::Doroti.Framework.Painting.TextWidthBasis textWidthBasis = TextWidthBasis.parent, string obscuringCharacter = "•", bool obscureText = false, Locale? locale = null, double cursorWidth = 1.0, double? cursorHeight = null, Radius? cursorRadius = null, bool paintCursorAboveText = false, Offset cursorOffset = default, double devicePixelRatio = 1.0, BoxHeightStyle selectionHeightStyle = BoxHeightStyle.max, BoxWidthStyle selectionWidthStyle = BoxWidthStyle.max, bool? enableInteractiveSelection = null, global::Doroti.Framework.Painting.EdgeInsets floatingCursorAddedMargin = default!, TextRange? promptRectRange = null, Color? promptRectColor = null, Clip clipBehavior = Clip.hardEdge, TextSelectionDelegate textSelectionDelegate = default!, RenderEditablePainter? painter = null, RenderEditablePainter? foregroundPainter = null, List<RenderBox>? children = null)
    {
        global::Doroti.Framework.Painting.TextScaler __textScaler = textScaler ?? global::Doroti.Framework.Painting.TextScaler.noScaling;
        global::Doroti.Framework.Painting.EdgeInsets __floatingCursorAddedMargin = floatingCursorAddedMargin ?? new global::Doroti.Framework.Painting.EdgeInsets(4, 4, 4, 5);
        this.ignorePointer = ignorePointer;
        this.floatingCursorAddedMargin = __floatingCursorAddedMargin;
        this.textSelectionDelegate = textSelectionDelegate;
        this._textPainter = new global::Doroti.Framework.Painting.TextPainter(text: text, textAlign: textAlign, textDirection: textDirection, textScaler: ((object.Equals(textScaler, global::Doroti.Framework.Painting.TextScaler.noScaling)) ? global::Doroti.Framework.Painting.TextScaler.CreateLinear(textScaleFactor) : textScaler), locale: locale, maxLines: ((maxLines == 1L) ? 1L : null), strutStyle: strutStyle, textHeightBehavior: textHeightBehavior, textWidthBasis: textWidthBasis);
        this._showCursor = (showCursor ?? new ValueNotifier<bool>(false));
        this._maxLines = maxLines;
        this._minLines = minLines;
        this._expands = expands;
        this._selection = selection;
        this._offset = offset;
        this._cursorWidth = cursorWidth;
        this._cursorHeight = cursorHeight;
        this._paintCursorOnTop = paintCursorAboveText;
        this._enableInteractiveSelection = enableInteractiveSelection;
        this._devicePixelRatio = devicePixelRatio;
        this._startHandleLayerLink = startHandleLayerLink;
        this._endHandleLayerLink = endHandleLayerLink;
        this._obscuringCharacter = obscuringCharacter;
        this._obscureText = obscureText;
        this._readOnly = readOnly;
        this._forceLine = forceLine;
        this._clipBehavior = clipBehavior;
        this._hasFocus = (hasFocus ?? false);
        this._disposeShowCursor = (showCursor is null);
        System.Diagnostics.Debug.Assert(((maxLines is null) || (DartRuntimePrimitives.RequireValue(maxLines) > 0L)));
        System.Diagnostics.Debug.Assert(((minLines is null) || (DartRuntimePrimitives.RequireValue(minLines) > 0L)));
        System.Diagnostics.Debug.Assert(((((maxLines is null)) || ((minLines is null))) || ((maxLines >= DartRuntimePrimitives.RequireValue(minLines)))));
        System.Diagnostics.Debug.Assert((!expands || (((maxLines is null) && (minLines is null)))));
        System.Diagnostics.Debug.Assert((DartRuntimePrimitives.Identical(__textScaler, global::Doroti.Framework.Painting.TextScaler.noScaling) || (textScaleFactor == 1.0)));
        System.Diagnostics.Debug.Assert((obscuringCharacter.characters().Count == 1L));
        System.Diagnostics.Debug.Assert((cursorWidth >= 0.0));
        System.Diagnostics.Debug.Assert(((cursorHeight is null) || (cursorHeight >= 0.0)));
    }

    public override void dispose()
    {
        this._leaderLayerHandler.layer = null;
        this._foregroundRenderObject?.dispose();
        _foregroundRenderObject = null;
        this._backgroundRenderObject?.dispose();
        _backgroundRenderObject = null;
        this._clipRectLayer.layer = null;
        this._cachedBuiltInForegroundPainters?.dispose();
        this._cachedBuiltInPainters?.dispose();
        this._selectionStartInViewport.dispose();
        this._selectionEndInViewport.dispose();
        this._autocorrectHighlightPainter.dispose();
        this._selectionPainter.dispose();
        this._caretPainter.dispose();
        this._textPainter.dispose();
        this._textIntrinsicsCache?.dispose();
        if (this._disposeShowCursor)
        {
            this._showCursor.dispose();
            _disposeShowCursor = false;
        }
        base.dispose();
    }

    internal virtual void _updateForegroundPainter(RenderEditablePainter? newPainter)
    {
        _CompositeRenderEditablePainter__editable effectivePainter__16223 = ((newPainter is null) ? this._builtInForegroundPainters : new _CompositeRenderEditablePainter__editable(painters: new List<RenderEditablePainter> { this._builtInForegroundPainters, newPainter }));
        if ((this._foregroundRenderObject is null))
        {
            var foregroundRenderObject__16497 = new _RenderEditableCustomPaint__editable(painter: effectivePainter__16223);
            adoptChild(foregroundRenderObject__16497);
            _foregroundRenderObject = foregroundRenderObject__16497;
        }
        else
        {
            this._foregroundRenderObject?.painter = effectivePainter__16223;
        }
        _foregroundPainter = newPainter;
    }

    public virtual RenderEditablePainter? foregroundPainter
    {
        get => this._foregroundPainter;
        set
        {
            var newPainter = value;
            if ((object.Equals(newPainter, this._foregroundPainter)))
            {
                return;
            }
            _updateForegroundPainter(newPainter);
        }
    }
    internal virtual void _updatePainter(RenderEditablePainter? newPainter)
    {
        _CompositeRenderEditablePainter__editable effectivePainter__17482 = ((newPainter is null) ? this._builtInPainters : new _CompositeRenderEditablePainter__editable(painters: new List<RenderEditablePainter> { this._builtInPainters, newPainter }));
        if ((this._backgroundRenderObject is null))
        {
            var backgroundRenderObject__17736 = new _RenderEditableCustomPaint__editable(painter: effectivePainter__17482);
            adoptChild(backgroundRenderObject__17736);
            _backgroundRenderObject = backgroundRenderObject__17736;
        }
        else
        {
            this._backgroundRenderObject?.painter = effectivePainter__17482;
        }
        _painter = newPainter;
    }

    public virtual RenderEditablePainter? painter
    {
        get => this._painter;
        set
        {
            var newPainter = value;
            if ((object.Equals(newPainter, this._painter)))
            {
                return;
            }
            _updatePainter(newPainter);
        }
    }
    internal virtual _CompositeRenderEditablePainter__editable _builtInForegroundPainters => _cachedBuiltInForegroundPainters ??= _createBuiltInForegroundPainters();
    internal virtual _CompositeRenderEditablePainter__editable _createBuiltInForegroundPainters()
    {
        return new _CompositeRenderEditablePainter__editable(painters: new List<RenderEditablePainter>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _CompositeRenderEditablePainter__editable _builtInPainters => _cachedBuiltInPainters ??= _createBuiltInPainters();
    internal virtual _CompositeRenderEditablePainter__editable _createBuiltInPainters()
    {
        return new _CompositeRenderEditablePainter__editable(painters: new List<RenderEditablePainter> { this._autocorrectHighlightPainter, this._selectionPainter });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.TextHeightBehavior? textHeightBehavior
    {
        get => ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).textHeightBehavior;
        set
        {
            var __value = value is null ? null : (TextHeightBehavior)(object)value;
            if ((object.Equals(((global::Doroti.Framework.Painting.TextPainter)this._textPainter).textHeightBehavior, __value)))
            {
                return;
            }
            this._textPainter.textHeightBehavior = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.TextWidthBasis textWidthBasis
    {
        get => ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).textWidthBasis;
        set
        {
            var __value = value;
            if ((object.Equals(((global::Doroti.Framework.Painting.TextPainter)this._textPainter).textWidthBasis, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            this._textPainter.textWidthBasis = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual double devicePixelRatio
    {
        get => this._devicePixelRatio;
        set
        {
            var __value = value;
            if ((this.devicePixelRatio == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _devicePixelRatio = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual string obscuringCharacter
    {
        get => this._obscuringCharacter;
        set
        {
            var __value = value;
            if ((this._obscuringCharacter == __value))
            {
                return;
            }
            DartRuntimePrimitives.Assert(() => (__value.characters().Count == 1L));
            _obscuringCharacter = __value;
            markNeedsLayout();
        }
    }
    public virtual bool obscureText
    {
        get => this._obscureText;
        set
        {
            var __value = value;
            if ((this._obscureText == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _obscureText = DartRuntimePrimitives.RequireValue(__value);
            _cachedAttributedValue = null;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual global::Doroti.Ui.BoxHeightStyle selectionHeightStyle
    {
        get => ((_TextHighlightPainter__editable)this._selectionPainter).selectionHeightStyle;
        set
        {
            var __value = value;
            this._selectionPainter.selectionHeightStyle = DartRuntimePrimitives.RequireValue(__value);
        }
    }
    public virtual global::Doroti.Ui.BoxWidthStyle selectionWidthStyle
    {
        get => ((_TextHighlightPainter__editable)this._selectionPainter).selectionWidthStyle;
        set
        {
            var __value = value;
            this._selectionPainter.selectionWidthStyle = DartRuntimePrimitives.RequireValue(__value);
        }
    }
    public virtual ValueListenable<bool> selectionStartInViewport => this._selectionStartInViewport;
    public virtual ValueListenable<bool> selectionEndInViewport => this._selectionEndInViewport;
    internal virtual global::Doroti.Ui.TextPosition _getTextPositionVertical(TextPosition position, double verticalOffset)
    {
        global::Doroti.Ui.Offset caretOffset__24556 = this._textPainter.getOffsetForCaret(position, this._caretPrototype);
        global::Doroti.Ui.Offset caretOffsetTranslated__24646 = caretOffset__24556.translate(0.0, verticalOffset);
        return this._textPainter.getPositionForOffset(caretOffsetTranslated__24646);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TextSelection getLineAtOffset(TextPosition position)
    {
        global::Doroti.Ui.TextRange line__24974 = this._textPainter.getLineBoundary(position);
        if (this.obscureText)
        {
            return new TextSelection(baseOffset: 0L, extentOffset: this.plainText.Length);
        }
        return new TextSelection(baseOffset: line__24974.start, extentOffset: line__24974.end);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.TextRange getWordBoundary(TextPosition position)
    {
        return this._textPainter.getWordBoundary(position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.TextPosition getTextPositionAbove(TextPosition position)
    {
        double preferredLineHeight__25835 = ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).preferredLineHeight;
        double verticalOffset__25908 = (-0.5 * preferredLineHeight__25835);
        return _getTextPositionVertical(position, verticalOffset__25908);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.TextPosition getTextPositionBelow(TextPosition position)
    {
        double preferredLineHeight__26394 = ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).preferredLineHeight;
        double verticalOffset__26467 = (1.5 * preferredLineHeight__26394);
        return _getTextPositionVertical(position, verticalOffset__26467);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateSelectionExtentsVisibility(Offset effectiveOffset)
    {
        DartRuntimePrimitives.Assert(() => (this.selection is not null));
        if (!this.selection!.isValid)
        {
            this._selectionStartInViewport.value = false;
            this._selectionEndInViewport.value = false;
            return;
        }
        global::Doroti.Ui.Rect visibleRegion__26864 = (Offset.zero & size);
        global::Doroti.Ui.Offset startOffset__26918 = this._textPainter.getOffsetForCaret(new global::Doroti.Ui.TextPosition(offset: this.selection!.start, affinity: this.selection!.affinity), this._caretPrototype);
        var visibleRegionSlop__27526 = 0.5;
        this._selectionStartInViewport.value = visibleRegion__26864.inflate(visibleRegionSlop__27526).contains((startOffset__26918 + effectiveOffset));
        global::Doroti.Ui.Offset endOffset__27707 = this._textPainter.getOffsetForCaret(new global::Doroti.Ui.TextPosition(offset: this.selection!.end, affinity: this.selection!.affinity), this._caretPrototype);
        this._selectionEndInViewport.value = visibleRegion__26864.inflate(visibleRegionSlop__27526).contains((endOffset__27707 + effectiveOffset));
    }

    internal virtual void _setTextEditingValue(TextEditingValue newValue, SelectionChangedCause cause)
    {
        this.textSelectionDelegate.userUpdateTextEditingValue(newValue, cause);
    }

    internal virtual void _setSelection(TextSelection nextSelection, SelectionChangedCause cause)
    {
        if (nextSelection.isValid)
        {
            long textLength__28824 = this.textSelectionDelegate.textEditingValue.text.Length;
            nextSelection = nextSelection.copyWith(baseOffset: Math.Min(nextSelection.baseOffset, textLength__28824), extentOffset: Math.Min(nextSelection.extentOffset, textLength__28824));
        }
        _setTextEditingValue(this.textSelectionDelegate.textEditingValue.copyWith(selection: nextSelection), cause);
    }

    public override void markNeedsPaint()
    {
        base.markNeedsPaint();
        this._foregroundRenderObject?.markNeedsPaint();
        this._backgroundRenderObject?.markNeedsPaint();
    }

    public virtual void systemFontsDidChange()
    {
        markNeedsLayout();
        this._textPainter.markNeedsLayout();
    }

    public virtual string plainText => ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).plainText;
    public virtual global::Doroti.Framework.Painting.InlineSpan? text
    {
        get => ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).text;
        set
        {
            var __value = value;
            if ((object.Equals(((global::Doroti.Framework.Painting.TextPainter)this._textPainter).text, __value)))
            {
                return;
            }
            _cachedLineBreakCount = null;
            this._textPainter.text = __value;
            _cachedAttributedValue = null;
            _cachedCombinedSemanticsInfos = null;
            markNeedsLayout();
            markNeedsSemanticsUpdate();
        }
    }
    internal virtual global::Doroti.Framework.Painting.TextPainter _textIntrinsics
    {
        get
        {
            return ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = (_textIntrinsicsCache ??= new global::Doroti.Framework.Painting.TextPainter());
    __cascade.text = ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).text;
    __cascade.textAlign = ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).textAlign;
    __cascade.textDirection = ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).textDirection;
    __cascade.textScaler = ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).textScaler;
    __cascade.maxLines = ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).maxLines;
    __cascade.ellipsis = ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).ellipsis;
    __cascade.locale = ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).locale;
    __cascade.strutStyle = ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).strutStyle;
    __cascade.textWidthBasis = ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).textWidthBasis;
    __cascade.textHeightBehavior = ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).textHeightBehavior;
    return __cascade;
}))();
            return default!;
        }
    }
    public virtual global::Doroti.Ui.TextAlign textAlign
    {
        get => ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).textAlign;
        set
        {
            var __value = value;
            if ((object.Equals(((global::Doroti.Framework.Painting.TextPainter)this._textPainter).textAlign, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            this._textPainter.textAlign = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Painting.TextPainter)this._textPainter).textDirection);
        set
        {
            var __value = value;
            if ((object.Equals(((global::Doroti.Framework.Painting.TextPainter)this._textPainter).textDirection, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            this._textPainter.textDirection = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
            markNeedsSemanticsUpdate();
        }
    }
    public virtual global::Doroti.Ui.Locale? locale
    {
        get => ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).locale;
        set
        {
            var __value = value;
            if ((object.Equals(((global::Doroti.Framework.Painting.TextPainter)this._textPainter).locale, __value)))
            {
                return;
            }
            this._textPainter.locale = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.StrutStyle? strutStyle
    {
        get => ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).strutStyle;
        set
        {
            var __value = value;
            if ((object.Equals(((global::Doroti.Framework.Painting.TextPainter)this._textPainter).strutStyle, __value)))
            {
                return;
            }
            this._textPainter.strutStyle = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.Color? cursorColor
    {
        get => ((_CaretPainter__editable)this._caretPainter).caretColor;
        set
        {
            var __value = value is null ? null : (Color)(object)value;
            this._caretPainter.caretColor = __value;
        }
    }
    public virtual global::Doroti.Ui.Color? backgroundCursorColor
    {
        get => ((_CaretPainter__editable)this._caretPainter).backgroundCursorColor;
        set
        {
            var __value = value is null ? null : (Color)(object)value;
            this._caretPainter.backgroundCursorColor = __value;
        }
    }
    public virtual ValueNotifier<bool> showCursor
    {
        get => this._showCursor;
        set
        {
            var __value = value;
            if ((object.Equals(this._showCursor, __value)))
            {
                return;
            }
            if (attached)
            {
                this._showCursor.removeListener(this._showHideCursor);
            }
            if (this._disposeShowCursor)
            {
                this._showCursor.dispose();
                _disposeShowCursor = false;
            }
            _showCursor = __value;
            if (attached)
            {
                _showHideCursor();
                this._showCursor.addListener(this._showHideCursor);
            }
        }
    }
    internal virtual void _showHideCursor()
    {
        this._caretPainter.shouldPaint = this.showCursor.value;
    }

    public virtual bool hasFocus
    {
        get => this._hasFocus;
        set
        {
            var __value = value;
            if ((this._hasFocus == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _hasFocus = DartRuntimePrimitives.RequireValue(__value);
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool forceLine
    {
        get => this._forceLine;
        set
        {
            var __value = value;
            if ((this._forceLine == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _forceLine = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual bool readOnly
    {
        get => this._readOnly;
        set
        {
            var __value = value;
            if ((this._readOnly == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _readOnly = DartRuntimePrimitives.RequireValue(__value);
            markNeedsSemanticsUpdate();
        }
    }
    public virtual long? maxLines
    {
        get => this._maxLines;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((__value is null) || (DartRuntimePrimitives.RequireValue(__value) > 0L)));
            if ((this.maxLines == __value))
            {
                return;
            }
            _maxLines = __value;
            this._textPainter.maxLines = ((__value == 1L) ? 1L : null);
            markNeedsLayout();
        }
    }
    public virtual long? minLines
    {
        get => this._minLines;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((__value is null) || (DartRuntimePrimitives.RequireValue(__value) > 0L)));
            if ((this.minLines == __value))
            {
                return;
            }
            _minLines = __value;
            markNeedsLayout();
        }
    }
    public virtual bool expands
    {
        get => this._expands;
        set
        {
            var __value = value;
            if ((this.expands == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _expands = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.Color? selectionColor
    {
        get => ((_TextHighlightPainter__editable)this._selectionPainter).highlightColor;
        set
        {
            var __value = value is null ? null : (Color)(object)value;
            this._selectionPainter.highlightColor = __value;
        }
    }
    public virtual double textScaleFactor
    {
        get => ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).textScaleFactor;
        set
        {
            var __value = value;
            textScaler = global::Doroti.Framework.Painting.TextScaler.CreateLinear(DartRuntimePrimitives.RequireValue(__value));
        }
    }
    public virtual global::Doroti.Framework.Painting.TextScaler textScaler
    {
        get => ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).textScaler;
        set
        {
            var __value = value;
            if ((object.Equals(((global::Doroti.Framework.Painting.TextPainter)this._textPainter).textScaler, __value)))
            {
                return;
            }
            this._textPainter.textScaler = __value;
            markNeedsLayout();
        }
    }
    public virtual TextSelection? selection
    {
        get => this._selection;
        set
        {
            var __value = value;
            if ((object.Equals(this._selection, __value)))
            {
                return;
            }
            _selection = __value;
            this._selectionPainter.highlightedRange = __value;
            markNeedsPaint();
            markNeedsSemanticsUpdate();
        }
    }
    public virtual ViewportOffset offset
    {
        get => this._offset;
        set
        {
            var __value = value;
            if ((object.Equals(this._offset, __value)))
            {
                return;
            }
            if (attached)
            {
                this._offset.removeListener(this.markNeedsPaint);
            }
            _offset = __value;
            if (attached)
            {
                this._offset.addListener(this.markNeedsPaint);
            }
            markNeedsLayout();
        }
    }
    public virtual double cursorWidth
    {
        get => this._cursorWidth;
        set
        {
            var __value = value;
            if ((this._cursorWidth == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _cursorWidth = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual double cursorHeight
    {
        get => (this._cursorHeight ?? this.preferredLineHeight);
        set => setCursorHeight(value);
    }
    public virtual void setCursorHeight(double? value)
    {
        if (this._cursorHeight == value)
        {
            return;
        }
        _cursorHeight = value;
        markNeedsLayout();
    }
    public virtual bool paintCursorAboveText
    {
        get => this._paintCursorOnTop;
        set
        {
            var __value = value;
            if ((this._paintCursorOnTop == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _paintCursorOnTop = DartRuntimePrimitives.RequireValue(__value);
            _cachedBuiltInForegroundPainters = null;
            _cachedBuiltInPainters = null;
            _updateForegroundPainter(this._foregroundPainter);
            _updatePainter(this._painter);
        }
    }
    public virtual global::Doroti.Ui.Offset cursorOffset
    {
        get => ((_CaretPainter__editable)this._caretPainter).cursorOffset;
        set
        {
            var __value = value;
            this._caretPainter.cursorOffset = DartRuntimePrimitives.RequireValue(__value);
        }
    }
    public virtual global::Doroti.Ui.Radius? cursorRadius
    {
        get => ((_CaretPainter__editable)this._caretPainter).cursorRadius;
        set
        {
            var __value = value;
            this._caretPainter.cursorRadius = __value;
        }
    }
    public virtual LayerLink startHandleLayerLink
    {
        get => this._startHandleLayerLink;
        set
        {
            var __value = value;
            if ((object.Equals(this._startHandleLayerLink, __value)))
            {
                return;
            }
            _startHandleLayerLink = __value;
            markNeedsPaint();
        }
    }
    public virtual LayerLink endHandleLayerLink
    {
        get => this._endHandleLayerLink;
        set
        {
            var __value = value;
            if ((object.Equals(this._endHandleLayerLink, __value)))
            {
                return;
            }
            _endHandleLayerLink = __value;
            markNeedsPaint();
        }
    }
    public virtual bool floatingCursorOn => this._floatingCursorOn;
    public virtual bool? enableInteractiveSelection
    {
        get => this._enableInteractiveSelection;
        set
        {
            var __value = value;
            if ((this._enableInteractiveSelection == __value))
            {
                return;
            }
            _enableInteractiveSelection = __value;
            markNeedsLayout();
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool selectionEnabled
    {
        get
        {
            return (this.enableInteractiveSelection ?? !this.obscureText);
            return default!;
        }
    }
    public virtual global::Doroti.Ui.Color? promptRectColor
    {
        get => ((_TextHighlightPainter__editable)this._autocorrectHighlightPainter).highlightColor;
        set
        {
            var newValue = value is null ? null : (Color)(object)value;
            this._autocorrectHighlightPainter.highlightColor = newValue;
        }
    }
    public virtual void setPromptRectRange(TextRange? newRange)
    {
        this._autocorrectHighlightPainter.highlightedRange = newRange;
    }

    public virtual double maxScrollExtent => this._maxScrollExtent;
    internal virtual double _caretMargin => (EditableLibrary._kCaretGap + this.cursorWidth);
    public virtual global::Doroti.Ui.Clip clipBehavior
    {
        get => this._clipBehavior;
        set
        {
            var __value = value;
            if ((!object.Equals(DartRuntimePrimitives.RequireValue(__value), this._clipBehavior)))
            {
                _clipBehavior = DartRuntimePrimitives.RequireValue(__value);
                markNeedsPaint();
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual List<global::Doroti.Ui.TextBox> getBoxesForSelection(TextSelection selection)
    {
        _computeTextMetricsIfNeeded();
        return this._textPainter.getBoxesForSelection(selection, boxHeightStyle: this.selectionHeightStyle, boxWidthStyle: this.selectionWidthStyle).map<TextBox, TextBox>(((textBox) => new global::Doroti.Ui.TextBox((textBox.left + this._paintOffset.dx), (textBox.top + this._paintOffset.dy), (textBox.right + this._paintOffset.dx), (textBox.bottom + this._paintOffset.dy), textBox.direction))).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        _semanticsInfo = ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).text!.getSemanticsInformation();
        if ((this._semanticsInfo!.any(((info) => (((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info).recognizer is not null))) && (!object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, TargetPlatform.macOS))))
        {
            DartRuntimePrimitives.Assert(() => (this.readOnly && !this.obscureText));
            ((Func<global::Doroti.Framework.Semantics.SemanticsConfiguration>)(() =>
{
    var __cascade = config;
    __cascade.isSemanticBoundary = true;
    __cascade.explicitChildNodes = true;
    return __cascade;
}))();
            return;
        }
        if ((this._cachedAttributedValue is null))
        {
            if (this.obscureText)
            {
                _cachedAttributedValue = new global::Doroti.Framework.Semantics.AttributedString(DartCoreExtensions.repeat(this.obscuringCharacter, this.plainText.Length));
            }
            else
            {
                var buffer__50059 = new StringBuffer();
                var offset__50096 = 0L;
                var attributes__50122 = new List<global::Doroti.Ui.StringAttribute>();
                foreach (global::Doroti.Framework.Painting.InlineSpanSemanticsInformation info__50206 in this._semanticsInfo!)
                {
                    string label__50256 = (((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info__50206).semanticsLabel ?? ((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info__50206).text);
                    foreach (global::Doroti.Ui.StringAttribute infoAttribute__50335 in ((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info__50206).stringAttributes)
                    {
                        global::Doroti.Ui.TextRange originalRange__50405 = infoAttribute__50335.range;
                        attributes__50122.Add(infoAttribute__50335.copy(range: new global::Doroti.Ui.TextRange(start: (offset__50096 + originalRange__50405.start), end: (offset__50096 + originalRange__50405.end))));
                    }
                    buffer__50059.write(label__50256);
                    offset__50096 += label__50256.Length;
                }
                _cachedAttributedValue = new global::Doroti.Framework.Semantics.AttributedString(buffer__50059.ToString(), attributes: attributes__50122);
            }
        }
        ((Func<global::Doroti.Framework.Semantics.SemanticsConfiguration>)(() =>
{
    var __cascade = config;
    __cascade.attributedValue = this._cachedAttributedValue!;
    __cascade.isObscured = this.obscureText;
    __cascade.isMultiline = this._isMultiline;
    __cascade.textDirection = this.textDirection;
    __cascade.isFocused = this.hasFocus;
    __cascade.isFocusable = true;
    __cascade.isTextField = true;
    __cascade.isReadOnly = this.readOnly;
    __cascade.inputType = Dart_uiLibrary.SemanticsInputType.text;
    return __cascade;
}))();
        if ((this.hasFocus && this.selectionEnabled))
        {
            config.onSetSelection = this._handleSetSelection;
        }
        if ((this.hasFocus && !this.readOnly))
        {
            config.onSetText = this._handleSetText;
        }
        if ((this.selectionEnabled && ((this.selection?.isValid ?? false))))
        {
            config.textSelection = this.selection;
            if ((this._textPainter.getOffsetBefore(this.selection!.extentOffset) is not null))
            {
                ((Func<global::Doroti.Framework.Semantics.SemanticsConfiguration>)(() =>
{
    var __cascade = config;
    __cascade.onMoveCursorBackwardByWord = this._handleMoveCursorBackwardByWord;
    __cascade.onMoveCursorBackwardByCharacter = this._handleMoveCursorBackwardByCharacter;
    return __cascade;
}))();
            }
            if ((this._textPainter.getOffsetAfter(this.selection!.extentOffset) is not null))
            {
                ((Func<global::Doroti.Framework.Semantics.SemanticsConfiguration>)(() =>
{
    var __cascade = config;
    __cascade.onMoveCursorForwardByWord = this._handleMoveCursorForwardByWord;
    __cascade.onMoveCursorForwardByCharacter = this._handleMoveCursorForwardByCharacter;
    return __cascade;
}))();
            }
        }
    }

    internal virtual void _handleSetText(string text)
    {
        this.textSelectionDelegate.userUpdateTextEditingValue(new TextEditingValue(text: text, selection: TextSelection.CreateCollapsed(offset: text.Length)), SelectionChangedCause.keyboard);
    }

    public override void assembleSemanticsNode(global::Doroti.Framework.Semantics.SemanticsNode node, global::Doroti.Framework.Semantics.SemanticsConfiguration config, IEnumerable<global::Doroti.Framework.Semantics.SemanticsNode> children)
    {
        DartRuntimePrimitives.Assert(() => ((this._semanticsInfo is not null) && (checked((long)(this._semanticsInfo!.Count)) != 0)));
        var newChildren__52630 = new List<global::Doroti.Framework.Semantics.SemanticsNode>();
        global::Doroti.Ui.TextDirection currentDirection__52681 = this.textDirection;
        global::Doroti.Ui.Rect currentRect__52724 = default!;
        var ordinal__52745 = 0.0;
        var start__52768 = 0L;
        var placeholderIndex__52787 = 0L;
        var childIndex__52817 = 0L;
        RenderBox? child__52848 = firstChild;
        var newChildCache__52878 = new DartMap<Key, global::Doroti.Framework.Semantics.SemanticsNode>();
        _cachedCombinedSemanticsInfos ??= global::Doroti.Framework.Painting.Inline_spanLibrary.combineSemanticsInfo(this._semanticsInfo!);
        foreach (global::Doroti.Framework.Painting.InlineSpanSemanticsInformation info__53041 in this._cachedCombinedSemanticsInfos!)
        {
            var selection__53095 = new TextSelection(baseOffset: start__52768, extentOffset: (start__52768 + ((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info__53041).text.Length));
            start__52768 += ((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info__53041).text.Length;
            if (((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info__53041).isPlaceholder)
            {
                while (((children.Count() > childIndex__52817) && children.elementAt(childIndex__52817).isTagged(new PlaceholderSpanIndexSemanticsTag(placeholderIndex__52787))))
                {
                    global::Doroti.Framework.Semantics.SemanticsNode childNode__53614 = children.elementAt(childIndex__52817);
                    var parentData__53674 = ((TextParentData?)(object?)child__52848!.parentData!)!;
                    DartRuntimePrimitives.Assert(() => (((TextParentData)parentData__53674).offset is not null));
                    newChildren__52630.Add(childNode__53614);
                    childIndex__52817 += 1L;
                }
                child__52848 = childAfter(child__52848!);
                placeholderIndex__52787 += 1L;
            }
            else
            {
                var initialDirection__53941 = currentDirection__52681;
                List<global::Doroti.Ui.TextBox> rects__54009 = this._textPainter.getBoxesForSelection(selection__53095);
                if ((checked((long)(rects__54009.Count)) == 0))
                {
                    continue;
                }
                global::Doroti.Ui.Rect rect__54135 = rects__54009.First().toRect();
                currentDirection__52681 = rects__54009.First().direction;
                foreach (global::Doroti.Ui.TextBox textBox__54244 in rects__54009.skip(1L))
                {
                    rect__54135 = rect__54135.expandToInclude(textBox__54244.toRect());
                    currentDirection__52681 = textBox__54244.direction;
                }
                rect__54135 = global::Doroti.Ui.Rect.fromLTWH(Math.Max(0.0, rect__54135.left), Math.Max(0.0, rect__54135.top), Math.Min(rect__54135.width, ((BoxConstraints)constraints).maxWidth), Math.Min(rect__54135.height, ((BoxConstraints)constraints).maxHeight));
                currentRect__52724 = global::Doroti.Ui.Rect.fromLTRB((rect__54135.left.floorToDouble() - 4.0), (rect__54135.top.floorToDouble() - 4.0), (rect__54135.right.ceilToDouble() + 4.0), (rect__54135.bottom.ceilToDouble() + 4.0));
                var configuration__55137 = ((Func<global::Doroti.Framework.Semantics.SemanticsConfiguration>)(() =>
{
    var __cascade = new global::Doroti.Framework.Semantics.SemanticsConfiguration();
    __cascade.sortKey = new global::Doroti.Framework.Semantics.OrdinalSortKey(ordinal__52745++);
    __cascade.textDirection = initialDirection__53941;
    __cascade.attributedLabel = new global::Doroti.Framework.Semantics.AttributedString((((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info__53041).semanticsLabel ?? ((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info__53041).text), attributes: ((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info__53041).stringAttributes);
    return __cascade;
}))();
                switch (((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info__53041).recognizer)
                {
                    case TapGestureRecognizer { onTap: Action handler__55523 } __object55475:
                        {
                            if ((handler__55523 is not null))
                            {
                                configuration__55137.onTap = handler__55523;
                                configuration__55137.isLink = true;
                            }
                            break;
                        }
                    case DoubleTapGestureRecognizer { onDoubleTap: Action handler__55608 } __object55548:
                        {
                            if ((handler__55608 is not null))
                            {
                                configuration__55137.onTap = handler__55608;
                                configuration__55137.isLink = true;
                            }
                            break;
                        }
                    case LongPressGestureRecognizer { onLongPress: Action onLongPress__55842 } __object55770:
                        {
                            if ((onLongPress__55842 is not null))
                            {
                                configuration__55137.onLongPress = onLongPress__55842;
                            }
                            break;
                        }
                    case null:
                        {
                            break;
                        }
                    default:
                        {
                            DartRuntimePrimitives.Assert(() => false);
                            break;
                        }
                }
                if ((((global::Doroti.Framework.Semantics.SemanticsNode)node).parentPaintClipRect is not null))
                {
                    global::Doroti.Ui.Rect paintRect__56181 = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsNode)node).parentPaintClipRect).intersect(currentRect__52724);
                    configuration__55137.isHidden = (paintRect__56181.isEmpty && !currentRect__52724.isEmpty);
                }
                global::Doroti.Framework.Semantics.SemanticsNode newChild__56364 = default!;
                if (((((long?)(this._cachedChildNodes?.Count)) is { } __count56386 ? __count56386 != 0 : (bool?)null) ?? false))
                {
                    newChild__56364 = this._cachedChildNodes!.remove(this._cachedChildNodes!.Keys.First())!;
                }
                else
                {
                    var key__56541 = new UniqueKey();
                    newChild__56364 = new global::Doroti.Framework.Semantics.SemanticsNode(key: key__56541, showOnScreen: _createShowOnScreenFor(key__56541));
                }
                ((Func<global::Doroti.Framework.Semantics.SemanticsNode>)(() =>
{
    var __cascade = newChild__56364;
    __cascade.updateWith(config: configuration__55137);
    __cascade.rect = currentRect__52724;
    return __cascade;
}))();
                newChildCache__52878[((global::Doroti.Framework.Semantics.SemanticsNode)newChild__56364).key!] = newChild__56364;
                newChildren__52630.Add(newChild__56364);
            }
        }
        _cachedChildNodes = newChildCache__52878.cast<Key, global::Doroti.Framework.Semantics.SemanticsNode>();
        node.updateWith(config: config, childrenInInversePaintOrder: newChildren__52630);
    }

    internal virtual Action? _createShowOnScreenFor(Key key)
    {
        return (() =>
        {
            global::Doroti.Framework.Semantics.SemanticsNode node__57067 = this._cachedChildNodes!.GetValueOrDefault(key)!;
            showOnScreen(descendant: this, rect: ((global::Doroti.Framework.Semantics.SemanticsNode)node__57067).rect);
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleSetSelection(TextSelection selection)
    {
        _setSelection(selection, SelectionChangedCause.keyboard);
    }

    internal virtual void _handleMoveCursorForwardByCharacter(bool extendSelection)
    {
        DartRuntimePrimitives.Assert(() => (this.selection is not null));
        long? extentOffset__57607 = this._textPainter.getOffsetAfter(this.selection!.extentOffset);
        if ((extentOffset__57607 is null))
        {
            return;
        }
        long baseOffset__57742 = (!extendSelection ? DartRuntimePrimitives.RequireValue(extentOffset__57607) : this.selection!.baseOffset);
        _setSelection(new TextSelection(baseOffset: baseOffset__57742, extentOffset: DartRuntimePrimitives.RequireValue(extentOffset__57607)), SelectionChangedCause.keyboard);
    }

    internal virtual void _handleMoveCursorBackwardByCharacter(bool extendSelection)
    {
        DartRuntimePrimitives.Assert(() => (this.selection is not null));
        long? extentOffset__58068 = this._textPainter.getOffsetBefore(this.selection!.extentOffset);
        if ((extentOffset__58068 is null))
        {
            return;
        }
        long baseOffset__58204 = (!extendSelection ? DartRuntimePrimitives.RequireValue(extentOffset__58068) : this.selection!.baseOffset);
        _setSelection(new TextSelection(baseOffset: baseOffset__58204, extentOffset: DartRuntimePrimitives.RequireValue(extentOffset__58068)), SelectionChangedCause.keyboard);
    }

    internal virtual void _handleMoveCursorForwardByWord(bool extendSelection)
    {
        DartRuntimePrimitives.Assert(() => (this.selection is not null));
        global::Doroti.Ui.TextRange currentWord__58529 = this._textPainter.getWordBoundary(this.selection!.extent);
        global::Doroti.Ui.TextRange? nextWord__58613 = _getNextWord(currentWord__58529.end);
        if ((nextWord__58613 is null))
        {
            return;
        }
        long baseOffset__58717 = (extendSelection ? this.selection!.baseOffset : nextWord__58613.start);
        _setSelection(new TextSelection(baseOffset: baseOffset__58717, extentOffset: nextWord__58613.start), SelectionChangedCause.keyboard);
    }

    internal virtual void _handleMoveCursorBackwardByWord(bool extendSelection)
    {
        DartRuntimePrimitives.Assert(() => (this.selection is not null));
        global::Doroti.Ui.TextRange currentWord__59046 = this._textPainter.getWordBoundary(this.selection!.extent);
        global::Doroti.Ui.TextRange? previousWord__59130 = _getPreviousWord((currentWord__59046.start - 1L));
        if ((previousWord__59130 is null))
        {
            return;
        }
        long baseOffset__59252 = (extendSelection ? this.selection!.baseOffset : previousWord__59130.start);
        _setSelection(new TextSelection(baseOffset: baseOffset__59252, extentOffset: previousWord__59130.start), SelectionChangedCause.keyboard);
    }

    internal virtual global::Doroti.Ui.TextRange? _getNextWord(long offset)
    {
        while (true)
        {
            global::Doroti.Ui.TextRange range__59556 = this._textPainter.getWordBoundary(new global::Doroti.Ui.TextPosition(offset: offset));
            if ((!range__59556.isValid || range__59556.isCollapsed))
            {
                return null;
            }
            if (!_onlyWhitespace(range__59556))
            {
                return range__59556;
            }
            offset = range__59556.end;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextRange? _getPreviousWord(long offset)
    {
        while ((offset >= 0L))
        {
            global::Doroti.Ui.TextRange range__59898 = this._textPainter.getWordBoundary(new global::Doroti.Ui.TextPosition(offset: offset));
            if ((!range__59898.isValid || range__59898.isCollapsed))
            {
                return null;
            }
            if (!_onlyWhitespace(range__59898))
            {
                return range__59898;
            }
            offset = (range__59898.start - 1L);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _onlyWhitespace(TextRange range)
    {
        for (long i__60538 = range.start; (i__60538 < range.end); i__60538++)
        {
            long codeUnit__60593 = DartRuntimePrimitives.RequireValue(this.text!.codeUnitAt(i__60538));
            if (!TextLayoutMetrics.isWhitespace(codeUnit__60593))
            {
                return false;
            }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        RenderBox? child__181803 = this._firstChild;
        while ((child__181803 is not null))
        {
            child__181803.attach(owner);
            var childParentData__181891 = ((TextParentData?)(object?)child__181803.parentData!)!;
            child__181803 = childParentData__181891.nextSibling;
        }
        this._foregroundRenderObject?.attach(owner);
        this._backgroundRenderObject?.attach(owner);
        _tap = ((Func<TapGestureRecognizer>)(() =>
{
    var __cascade = new TapGestureRecognizer(debugOwner: this);
    __cascade.onTapDown = this._handleTapDown;
    __cascade.onTap = this._handleTap;
    return __cascade;
}))();
        _longPress = ((Func<LongPressGestureRecognizer>)(() =>
{
    var __cascade = new LongPressGestureRecognizer(debugOwner: this);
    __cascade.onLongPress = this._handleLongPress;
    return __cascade;
}))();
        this._offset.addListener(this.markNeedsPaint);
        _showHideCursor();
        this._showCursor.addListener(this._showHideCursor);
    }

    public override void detach()
    {
        this._tap.dispose();
        this._longPress.dispose();
        this._offset.removeListener(this.markNeedsPaint);
        this._showCursor.removeListener(this._showHideCursor);
        base.detach();
        RenderBox? child__182065 = this._firstChild;
        while ((child__182065 is not null))
        {
            child__182065.detach();
            var childParentData__182148 = ((TextParentData?)(object?)child__182065.parentData!)!;
            child__182065 = childParentData__182148.nextSibling;
        }
        this._foregroundRenderObject?.detach();
        this._backgroundRenderObject?.detach();
    }

    public override void redepthChildren()
    {
        RenderObject? foregroundChild__61560 = this._foregroundRenderObject;
        RenderObject? backgroundChild__61627 = this._backgroundRenderObject;
        if ((foregroundChild__61560 is not null))
        {
            redepthChild(foregroundChild__61560);
        }
        if ((backgroundChild__61627 is not null))
        {
            redepthChild(backgroundChild__61627);
        }
        RenderBox? child__182311 = this._firstChild;
        while ((child__182311 is not null))
        {
            redepthChild(child__182311);
            var childParentData__182399 = ((TextParentData?)(object?)child__182311.parentData!)!;
            child__182311 = childParentData__182399.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderObject? foregroundChild__61948 = this._foregroundRenderObject;
        RenderObject? backgroundChild__62015 = this._backgroundRenderObject;
        if ((foregroundChild__61948 is not null))
        {
            visitor(foregroundChild__61948);
        }
        if ((backgroundChild__62015 is not null))
        {
            visitor(backgroundChild__62015);
        }
        RenderBox? child__182587 = this._firstChild;
        while ((child__182587 is not null))
        {
            visitor(child__182587);
            var childParentData__182670 = ((TextParentData?)(object?)child__182587.parentData!)!;
            child__182587 = childParentData__182670.nextSibling;
        }
    }

    internal virtual bool _isMultiline => (this.maxLines != 1L);
    internal virtual global::Doroti.Framework.Painting.Axis _viewportAxis => (this._isMultiline ? global::Doroti.Framework.Painting.Axis.vertical : global::Doroti.Framework.Painting.Axis.horizontal);
    internal virtual global::Doroti.Ui.Offset _paintOffset => (this._viewportAxis switch { global::Doroti.Framework.Painting.Axis.horizontal => new global::Doroti.Ui.Offset(-((ViewportOffset)this.offset).pixels, 0.0), global::Doroti.Framework.Painting.Axis.vertical => new global::Doroti.Ui.Offset(0.0, -((ViewportOffset)this.offset).pixels), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    internal virtual double _viewportExtent
    {
        get
        {
            DartRuntimePrimitives.Assert(() => hasSize);
            return (this._viewportAxis switch { global::Doroti.Framework.Painting.Axis.horizontal => size.width, global::Doroti.Framework.Painting.Axis.vertical => size.height, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    internal virtual double _getMaxScrollExtent(Size contentSize)
    {
        DartRuntimePrimitives.Assert(() => hasSize);
        return (this._viewportAxis switch { global::Doroti.Framework.Painting.Axis.horizontal => Math.Max(0.0, (contentSize.width - size.width)), global::Doroti.Framework.Painting.Axis.vertical => Math.Max(0.0, (contentSize.height - size.height)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _hasVisualOverflow => ((this._maxScrollExtent > 0L) || (!object.Equals(this._paintOffset, Offset.zero)));
    public virtual List<TextSelectionPoint> getEndpointsForSelection(TextSelection selection)
    {
        _computeTextMetricsIfNeeded();
        global::Doroti.Ui.Offset paintOffset__63964 = this._paintOffset;
        List<global::Doroti.Ui.TextBox> boxes__64020 = (selection.isCollapsed ? new List<global::Doroti.Ui.TextBox>() : this._textPainter.getBoxesForSelection(selection, boxHeightStyle: this.selectionHeightStyle, boxWidthStyle: this.selectionWidthStyle));
        if ((checked((long)(boxes__64020.Count)) == 0))
        {
            global::Doroti.Ui.Offset caretOffset__64372 = this._textPainter.getOffsetForCaret(selection.extent, this._caretPrototype);
            global::Doroti.Ui.Offset start__64472 = ((new global::Doroti.Ui.Offset(0.0, this.preferredLineHeight) + caretOffset__64372) + paintOffset__63964);
            return new List<TextSelectionPoint> { new TextSelectionPoint(start__64472, null) };
        }
        else
        {
            global::Doroti.Ui.Offset start__64642 = (new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble(boxes__64020.First().start, 0, ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).size.width), boxes__64020.First().bottom) + paintOffset__63964);
            global::Doroti.Ui.Offset end__64791 = (new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble(boxes__64020.Last().end, 0, ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).size.width), boxes__64020.Last().bottom) + paintOffset__63964);
            return new List<TextSelectionPoint> { new TextSelectionPoint(start__64642, boxes__64020.First().direction), new TextSelectionPoint(end__64791, boxes__64020.Last().direction) };
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Rect? getRectForComposingRange(TextRange range)
    {
        if ((!range.isValid || range.isCollapsed))
        {
            return null;
        }
        _computeTextMetricsIfNeeded();
        List<global::Doroti.Ui.TextBox> boxes__65618 = this._textPainter.getBoxesForSelection(new TextSelection(baseOffset: range.start, extentOffset: range.end), boxHeightStyle: this.selectionHeightStyle, boxWidthStyle: this.selectionWidthStyle);
        return System.Linq.Enumerable.Aggregate(boxes__65618, (Rect?)null, ((accum, incoming) => (accum?.expandToInclude(incoming.toRect()) ?? incoming.toRect())))?.shift(this._paintOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.TextPosition getPositionForPoint(Offset globalPosition)
    {
        _computeTextMetricsIfNeeded();
        return this._textPainter.getPositionForOffset((globalToLocal(globalPosition) - this._paintOffset));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Rect getLocalRectForCaret(TextPosition caretPosition)
    {
        _computeTextMetricsIfNeeded();
        global::Doroti.Ui.Rect caretPrototype__67177 = this._caretPrototype;
        global::Doroti.Ui.Offset caretOffset__67228 = this._textPainter.getOffsetForCaret(caretPosition, caretPrototype__67177);
        global::Doroti.Ui.Rect caretRect__67314 = caretPrototype__67177.shift((caretOffset__67228 + this.cursorOffset));
        double scrollableWidth__67393 = Math.Max((((global::Doroti.Framework.Painting.TextPainter)this._textPainter).width + this._caretMargin), size.width);
        double caretX__67486 = Dart_uiLibrary.clampDouble(caretRect__67314.left, 0, Math.Max((scrollableWidth__67393 - this._caretMargin), 0));
        caretRect__67314 = (new global::Doroti.Ui.Offset(caretX__67486, caretRect__67314.top) & caretRect__67314.size);
        double fullHeight__67679 = this._textPainter.getFullHeightForCaret(caretPosition, caretPrototype__67177);
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case var __constant67807 when object.Equals(__constant67807, TargetPlatform.iOS):
            case var __constant67838 when object.Equals(__constant67838, TargetPlatform.macOS):
                {
                    double heightDiff__67936 = (fullHeight__67679 - caretRect__67314.height);
                    caretRect__67314 = global::Doroti.Ui.Rect.fromLTWH(caretRect__67314.left, (caretRect__67314.top + (heightDiff__67936 / 2L)), caretRect__67314.width, caretRect__67314.height);
                    break;
                }
            case var __constant68160 when object.Equals(__constant68160, TargetPlatform.android):
            case var __constant68195 when object.Equals(__constant68195, TargetPlatform.fuchsia):
            case var __constant68230 when object.Equals(__constant68230, TargetPlatform.linux):
            case var __constant68263 when object.Equals(__constant68263, TargetPlatform.windows):
                {
                    double caretHeight__68556 = this.cursorHeight;
                    double heightDiff__68660 = (fullHeight__67679 - caretHeight__68556);
                    caretRect__67314 = global::Doroti.Ui.Rect.fromLTWH(caretRect__67314.left, ((caretRect__67314.top - EditableLibrary._kCaretHeightOffset) + (heightDiff__68660 / 2L)), caretRect__67314.width, caretHeight__68556);
                    break;
                }
        }
        caretRect__67314 = caretRect__67314.shift(this._paintOffset);
        return caretRect__67314.shift(_snapToPhysicalPixel(caretRect__67314.topLeft));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        List<global::Doroti.Framework.Painting.PlaceholderDimensions> placeholderDimensions__69114 = layoutInlineChildren(double.PositiveInfinity, ((Func<RenderBox, BoxConstraints, Size>)((child, constraints) => new global::Doroti.Ui.Size(child.getMinIntrinsicWidth(double.PositiveInfinity), 0.0))), (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline);
        var (minWidth__69369, maxWidth__69386) = _adjustConstraints();
        return (((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textIntrinsics;
    __cascade.setPlaceholderDimensions(placeholderDimensions__69114);
    __cascade.layout(minWidth: minWidth__69369, maxWidth: maxWidth__69386);
    return __cascade;
}))()).minIntrinsicWidth;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        List<global::Doroti.Framework.Painting.PlaceholderDimensions> placeholderDimensions__69702 = layoutInlineChildren(double.PositiveInfinity, ((Func<RenderBox, BoxConstraints, Size>)((child, constraints) => new global::Doroti.Ui.Size(child.getMaxIntrinsicWidth(double.PositiveInfinity), 0.0))), (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline);
        var (minWidth__70104, maxWidth__70121) = _adjustConstraints();
        return ((((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textIntrinsics;
    __cascade.setPlaceholderDimensions(placeholderDimensions__69702);
    __cascade.layout(minWidth: minWidth__70104, maxWidth: maxWidth__70121);
    return __cascade;
}))()).maxIntrinsicWidth + this._caretMargin);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double preferredLineHeight => ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).preferredLineHeight;
    internal virtual long _countHardLineBreaks(string text)
    {
        long? cachedValue__70677 = this._cachedLineBreakCount;
        if ((cachedValue__70677 is not null))
        {
            long cachedValue__70677__value70722 = DartRuntimePrimitives.RequireValue(cachedValue__70677);
            return DartRuntimePrimitives.RequireValue(cachedValue__70677__value70722);
        }
        var count__70785 = 0L;
        for (var index__70809 = 0L; (index__70809 < text.Length); index__70809 += 1L)
        {
            switch (text.codeUnitAt(index__70809))
            {
                case 10L:
                case 133L:
                case 11L:
                case 12L:
                case 8232L:
                case 8233L:
                    {
                        count__70785 += 1L;
                        break;
                    }
            }
        }
        return DartRuntimePrimitives.RequireValue(_cachedLineBreakCount = count__70785);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _preferredHeight(double width)
    {
        long? maxLines__71239 = this.maxLines;
        long? minLines__71280 = (this.minLines ?? maxLines__71239);
        double minHeight__71335 = (this.preferredLineHeight * ((minLines__71280 ?? 0L)));
        DartRuntimePrimitives.Assert(() => ((maxLines__71239 != 1L) || (((global::Doroti.Framework.Painting.TextPainter)this._textIntrinsics).maxLines == 1L)));
        if ((maxLines__71239 is null))
        {
            double estimatedHeight__71494 = default!;
            if ((width == double.PositiveInfinity))
            {
                estimatedHeight__71494 = (this.preferredLineHeight * ((_countHardLineBreaks(this.plainText) + 1L)));
            }
            else
            {
                var (minWidth__71673, maxWidth__71690) = _adjustConstraints(maxWidth: width);
                estimatedHeight__71494 = (((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textIntrinsics;
    __cascade.layout(minWidth: minWidth__71673, maxWidth: maxWidth__71690);
    return __cascade;
}))()).height;
            }
            return Math.Max(estimatedHeight__71494, minHeight__71335);
        }
        if ((DartRuntimePrimitives.RequireValue(maxLines__71239) == 1L))
        {
            var (minWidth__72341, maxWidth__72358) = _adjustConstraints(maxWidth: width);
            return (((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textIntrinsics;
    __cascade.layout(minWidth: minWidth__72341, maxWidth: maxWidth__72358);
    return __cascade;
}))()).height;
        }
        if ((minLines__71280 == DartRuntimePrimitives.RequireValue(maxLines__71239)))
        {
            return minHeight__71335;
        }
        double maxHeight__72579 = (this.preferredLineHeight * DartRuntimePrimitives.RequireValue(maxLines__71239));
        var (minWidth__72641, maxWidth__72658) = _adjustConstraints(maxWidth: width);
        return Dart_uiLibrary.clampDouble((((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textIntrinsics;
    __cascade.layout(minWidth: minWidth__72641, maxWidth: maxWidth__72658);
    return __cascade;
}))()).height, minHeight__71335, maxHeight__72579);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width) => getMaxIntrinsicHeight(width);
    public override double computeMaxIntrinsicHeight(double width)
    {
        this._textIntrinsics.setPlaceholderDimensions(layoutInlineChildren(width, (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.dryLayoutChild, (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline));
        return _preferredHeight(width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        _computeTextMetricsIfNeeded();
        return this._textPainter.computeDistanceToActualBaseline(baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestSelf(Offset position) => true;
    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        global::Doroti.Ui.Offset effectivePosition__73608 = (position - this._paintOffset);
        global::Doroti.Ui.GlyphInfo? glyph__73674 = this._textPainter.getClosestGlyphForOffset(effectivePosition__73608);
        global::Doroti.Framework.Painting.InlineSpan? spanHit__74108 = (((glyph__73674 is not null) && glyph__73674.graphemeClusterLayoutBounds.contains(effectivePosition__73608)) ? ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).text!.getSpanForPosition(new global::Doroti.Ui.TextPosition(offset: glyph__73674.graphemeClusterCodeUnitRange.start)) : null);
        switch (spanHit__74108)
        {
            case HitTestTarget span__74412:
                {
                    result.add(new HitTestEntry<HitTestTarget>(span__74412));
                    return true;
                }
            default:
                {
                    return hitTestInlineChildren(result, effectivePosition__73608);
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void handleEvent(global::Doroti.Framework.Gestures.PointerEvent @event, HitTestEntry<HitTestTarget> entry)
    {
        DartRuntimePrimitives.Assert(() => debugHandleEvent(@event, entry));
        if ((@event is global::Doroti.Framework.Gestures.PointerDownEvent))
        {
            global::Doroti.Framework.Gestures.PointerDownEvent @event__as74778 = (global::Doroti.Framework.Gestures.PointerDownEvent)@event;
            DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
            if (!this.ignorePointer)
            {
                this._tap.addPointer((global::Doroti.Framework.Gestures.PointerDownEvent)(object)@event__as74778);
                this._longPress.addPointer((global::Doroti.Framework.Gestures.PointerDownEvent)(object)@event__as74778);
            }
        }
    }

    public virtual global::Doroti.Ui.Offset? lastSecondaryTapDownPosition => this._lastSecondaryTapDownPosition;
    public virtual void handleSecondaryTapDown(TapDownDetails details)
    {
        _lastTapDownPosition = details.globalPosition;
        _lastSecondaryTapDownPosition = details.globalPosition;
    }

    public virtual void handleTapDown(TapDownDetails details)
    {
        _lastTapDownPosition = details.globalPosition;
    }

    internal virtual void _handleTapDown(TapDownDetails details)
    {
        DartRuntimePrimitives.Assert(() => !this.ignorePointer);
        handleTapDown(details);
    }

    public virtual void handleTap()
    {
        selectPosition(cause: SelectionChangedCause.tap);
    }

    internal virtual void _handleTap()
    {
        DartRuntimePrimitives.Assert(() => !this.ignorePointer);
        handleTap();
    }

    public virtual void handleDoubleTap()
    {
        selectWord(cause: SelectionChangedCause.doubleTap);
    }

    public virtual void handleLongPress()
    {
        selectWord(cause: SelectionChangedCause.longPress);
    }

    internal virtual void _handleLongPress()
    {
        DartRuntimePrimitives.Assert(() => !this.ignorePointer);
        handleLongPress();
    }

    public virtual void selectPosition(SelectionChangedCause cause)
    {
        selectPositionAt(from: DartRuntimePrimitives.RequireValue(this._lastTapDownPosition), cause: cause);
    }

    public virtual void selectPositionAt(Offset from, Offset? to = null, SelectionChangedCause cause = default!)
    {
        _computeTextMetricsIfNeeded();
        global::Doroti.Ui.TextPosition fromPosition__78521 = this._textPainter.getPositionForOffset((globalToLocal(from) - this._paintOffset));
        global::Doroti.Ui.TextPosition? toPosition__78644 = ((to is null) ? null : this._textPainter.getPositionForOffset((globalToLocal(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(to))) - this._paintOffset)));
        long baseOffset__78777 = fromPosition__78521.offset;
        long extentOffset__78825 = (toPosition__78644?.offset ?? fromPosition__78521.offset);
        var newSelection__78894 = new TextSelection(baseOffset: baseOffset__78777, extentOffset: extentOffset__78825, affinity: fromPosition__78521.affinity);
        _setSelection(newSelection__78894, cause);
    }

    public virtual global::Doroti.Framework.Painting.WordBoundary wordBoundaries => ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).wordBoundaries;
    public virtual void selectWord(SelectionChangedCause cause)
    {
        selectWordsInRange(from: DartRuntimePrimitives.RequireValue(this._lastTapDownPosition), cause: cause);
    }

    public virtual void selectWordsInRange(Offset from, Offset? to = null, SelectionChangedCause cause = default!)
    {
        _computeTextMetricsIfNeeded();
        global::Doroti.Ui.TextPosition fromPosition__80033 = this._textPainter.getPositionForOffset((globalToLocal(from) - this._paintOffset));
        TextSelection fromWord__80156 = getWordAtOffset(fromPosition__80033);
        global::Doroti.Ui.TextPosition toPosition__80221 = ((to is null) ? fromPosition__80033 : this._textPainter.getPositionForOffset((globalToLocal(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(to))) - this._paintOffset)));
        TextSelection toWord__80371 = ((object.Equals(toPosition__80221, fromPosition__80033)) ? fromWord__80156 : getWordAtOffset(toPosition__80221));
        bool isFromWordBeforeToWord__80480 = (fromWord__80156.start < toWord__80371.end);
        _setSelection(new TextSelection(baseOffset: (isFromWordBeforeToWord__80480 ? fromWord__80156.@base.offset : fromWord__80156.extent.offset), extentOffset: (isFromWordBeforeToWord__80480 ? toWord__80371.extent.offset : toWord__80371.@base.offset), affinity: fromWord__80156.affinity), cause);
    }

    public virtual void selectWordEdge(SelectionChangedCause cause)
    {
        _computeTextMetricsIfNeeded();
        DartRuntimePrimitives.Assert(() => (this._lastTapDownPosition is not null));
        global::Doroti.Ui.TextPosition position__81121 = this._textPainter.getPositionForOffset((globalToLocal(DartRuntimePrimitives.RequireValue(this._lastTapDownPosition)) - this._paintOffset));
        global::Doroti.Ui.TextRange word__81253 = this._textPainter.getWordBoundary(position__81121);
        TextSelection newSelection__81323 = default!;
        if ((position__81121.offset <= word__81253.start))
        {
            newSelection__81323 = TextSelection.CreateCollapsed(offset: word__81253.start);
        }
        else
        {
            newSelection__81323 = TextSelection.CreateCollapsed(offset: word__81253.end, affinity: TextAffinity.upstream);
        }
        _setSelection(newSelection__81323, cause);
    }

    public virtual TextSelection getWordAtOffset(TextPosition position)
    {
        if ((position.offset >= this.plainText.Length))
        {
            return TextSelection.CreateFromPosition(new global::Doroti.Ui.TextPosition(offset: this.plainText.Length, affinity: TextAffinity.upstream));
        }
        if (this.obscureText)
        {
            return new TextSelection(baseOffset: 0L, extentOffset: this.plainText.Length);
        }
        global::Doroti.Ui.TextRange word__82243 = this._textPainter.getWordBoundary(position);
        long effectiveOffset__82304 = default!;
        switch (position.affinity)
        {
            case TextAffinity.upstream:
                {
                    effectiveOffset__82304 = (position.offset - 1L);
                    break;
                }
            case TextAffinity.downstream:
                {
                    effectiveOffset__82304 = position.offset;
                    break;
                }
        }
        DartRuntimePrimitives.Assert(() => (effectiveOffset__82304 >= 0L));
        if (((effectiveOffset__82304 > 0L) && TextLayoutMetrics.isWhitespace(this.plainText.codeUnitAt(effectiveOffset__82304))))
        {
            global::Doroti.Ui.TextRange? previousWord__83157 = _getPreviousWord(word__82243.start);
            switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
            {
                case var __constant83254 when object.Equals(__constant83254, TargetPlatform.iOS):
                    {
                        if ((previousWord__83157 is null))
                        {
                            global::Doroti.Ui.TextRange? nextWord__83341 = _getNextWord(word__82243.start);
                            if ((nextWord__83341 is null))
                            {
                                return TextSelection.CreateCollapsed(offset: position.offset);
                            }
                            return new TextSelection(baseOffset: position.offset, extentOffset: nextWord__83341.end);
                        }
                        return new TextSelection(baseOffset: previousWord__83157.start, extentOffset: position.offset);
                    }
                case var __constant83710 when object.Equals(__constant83710, TargetPlatform.android):
                    {
                        if (this.readOnly)
                        {
                            if ((previousWord__83157 is null))
                            {
                                return new TextSelection(baseOffset: position.offset, extentOffset: (position.offset + 1L));
                            }
                            return new TextSelection(baseOffset: previousWord__83157.start, extentOffset: position.offset);
                        }
                        break;
                    }
                case var __constant84036 when object.Equals(__constant84036, TargetPlatform.fuchsia):
                case var __constant84073 when object.Equals(__constant84073, TargetPlatform.macOS):
                case var __constant84108 when object.Equals(__constant84108, TargetPlatform.linux):
                case var __constant84143 when object.Equals(__constant84143, TargetPlatform.windows):
                    {
                        break;
                    }
            }
        }
        return new TextSelection(baseOffset: word__82243.start, extentOffset: word__82243.end);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (double, double) _adjustConstraints(double minWidth = 0.0, double maxWidth = double.PositiveInfinity)
    {
        double availableMaxWidth__84793 = Math.Max(0.0, (maxWidth - this._caretMargin));
        double availableMinWidth__84870 = Math.Min(minWidth, availableMaxWidth__84793);
        return ((this.forceLine ? availableMaxWidth__84793 : availableMinWidth__84870), (this._isMultiline ? availableMaxWidth__84793 : double.PositiveInfinity));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _computeTextMetricsIfNeeded()
    {
        var (minWidth__86160, maxWidth__86177) = _adjustConstraints(minWidth: ((BoxConstraints)constraints).minWidth, maxWidth: ((BoxConstraints)constraints).maxWidth);
        this._textPainter.layout(minWidth: minWidth__86160, maxWidth: maxWidth__86177);
    }

    internal virtual void _computeCaretPrototype()
    {
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case var __constant86723 when object.Equals(__constant86723, TargetPlatform.iOS):
            case var __constant86754 when object.Equals(__constant86754, TargetPlatform.macOS):
                {
                    _caretPrototype = global::Doroti.Ui.Rect.fromLTWH(0.0, 0.0, this.cursorWidth, (this.cursorHeight + 2L));
                    break;
                }
            case var __constant86869 when object.Equals(__constant86869, TargetPlatform.android):
            case var __constant86904 when object.Equals(__constant86904, TargetPlatform.fuchsia):
            case var __constant86939 when object.Equals(__constant86939, TargetPlatform.linux):
            case var __constant86972 when object.Equals(__constant86972, TargetPlatform.windows):
                {
                    _caretPrototype = global::Doroti.Ui.Rect.fromLTWH(0.0, EditableLibrary._kCaretHeightOffset, this.cursorWidth, (this.cursorHeight - (2.0 * EditableLibrary._kCaretHeightOffset)));
                    break;
                }
        }
    }

    internal virtual global::Doroti.Ui.Offset _snapToPhysicalPixel(Offset sourceOffset)
    {
        global::Doroti.Ui.Offset globalOffset__87359 = localToGlobal(sourceOffset);
        double pixelMultiple__87420 = (1.0 / this._devicePixelRatio);
        return new global::Doroti.Ui.Offset((double.IsFinite(globalOffset__87359.dx) ? ((((globalOffset__87359.dx / pixelMultiple__87420)).round() * pixelMultiple__87420) - globalOffset__87359.dx) : 0), (double.IsFinite(globalOffset__87359.dy) ? ((((globalOffset__87359.dy / pixelMultiple__87420)).round() * pixelMultiple__87420) - globalOffset__87359.dy) : 0));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        var (minWidth__87867, maxWidth__87884) = _adjustConstraints(minWidth: ((BoxConstraints)constraints).minWidth, maxWidth: ((BoxConstraints)constraints).maxWidth);
        ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textIntrinsics;
    __cascade.setPlaceholderDimensions(layoutInlineChildren(((BoxConstraints)constraints).maxWidth, (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.dryLayoutChild, (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline));
    __cascade.layout(minWidth: minWidth__87867, maxWidth: maxWidth__87884);
    return __cascade;
}))();
        double width__88295 = (this.forceLine ? ((BoxConstraints)constraints).maxWidth : constraints.constrainWidth((((global::Doroti.Framework.Painting.TextPainter)this._textIntrinsics).size.width + this._caretMargin)));
        return new global::Doroti.Ui.Size(width__88295, constraints.constrainHeight(_preferredHeight(((BoxConstraints)constraints).maxWidth)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        var (minWidth__88644, maxWidth__88661) = _adjustConstraints(minWidth: ((BoxConstraints)constraints).minWidth, maxWidth: ((BoxConstraints)constraints).maxWidth);
        ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textIntrinsics;
    __cascade.setPlaceholderDimensions(layoutInlineChildren(((BoxConstraints)constraints).maxWidth, (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.dryLayoutChild, (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline));
    __cascade.layout(minWidth: minWidth__88644, maxWidth: maxWidth__88661);
    return __cascade;
}))();
        return this._textIntrinsics.computeDistanceToActualBaseline(baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraints__89192 = this.constraints;
        _placeholderDimensions = layoutInlineChildren(((BoxConstraints)constraints__89192).maxWidth, (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.layoutChild, (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getBaseline);
        var (minWidth__89402, maxWidth__89419) = _adjustConstraints(minWidth: ((BoxConstraints)constraints__89192).minWidth, maxWidth: ((BoxConstraints)constraints__89192).maxWidth);
        ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textPainter;
    __cascade.setPlaceholderDimensions(this._placeholderDimensions);
    __cascade.layout(minWidth: minWidth__89402, maxWidth: maxWidth__89419);
    return __cascade;
}))();
        positionInlineChildren(((global::Doroti.Framework.Painting.TextPainter)this._textPainter).inlinePlaceholderBoxes!);
        _computeCaretPrototype();
        double width__89778 = (this.forceLine ? ((BoxConstraints)constraints__89192).maxWidth : constraints__89192.constrainWidth((((global::Doroti.Framework.Painting.TextPainter)this._textPainter).width + this._caretMargin)));
        DartRuntimePrimitives.Assert(() => ((this.maxLines != 1L) || (((global::Doroti.Framework.Painting.TextPainter)this._textPainter).maxLines == 1L)));
        double preferredHeight__89974 = (this.maxLines switch { null => Math.Max(((global::Doroti.Framework.Painting.TextPainter)this._textPainter).height, (this.preferredLineHeight * ((this.minLines ?? 0L)))), 1L => ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).height, long maxLines__90144 => Dart_uiLibrary.clampDouble(((global::Doroti.Framework.Painting.TextPainter)this._textPainter).height, (this.preferredLineHeight * ((this.minLines ?? DartRuntimePrimitives.RequireValue(maxLines__90144)))), (this.preferredLineHeight * DartRuntimePrimitives.RequireValue(maxLines__90144))) });
        size = new global::Doroti.Ui.Size(width__89778, constraints__89192.constrainHeight(preferredHeight__89974));
        var contentSize__90389 = new global::Doroti.Ui.Size((((global::Doroti.Framework.Painting.TextPainter)this._textPainter).width + this._caretMargin), ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).height);
        var painterConstraints__90476 = BoxConstraints.CreateTight(contentSize__90389);
        this._foregroundRenderObject?.layout(painterConstraints__90476);
        this._backgroundRenderObject?.layout(painterConstraints__90476);
        _maxScrollExtent = _getMaxScrollExtent(contentSize__90389);
        this.offset.applyViewportDimension(this._viewportExtent);
        this.offset.applyContentDimensions(0.0, this._maxScrollExtent);
    }

    internal static global::Doroti.Ui.Offset _calculateAdjustedCursorOffset(Offset offset, Rect boundingRects)
    {
        double adjustedX__91441 = Dart_uiLibrary.clampDouble(offset.dx, boundingRects.left, boundingRects.right);
        double adjustedY__91535 = Dart_uiLibrary.clampDouble(offset.dy, boundingRects.top, boundingRects.bottom);
        return new global::Doroti.Ui.Offset(adjustedX__91441, adjustedY__91535);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Offset calculateBoundedFloatingCursorOffset(Offset rawCursorOffset, bool? shouldResetOrigin = null)
    {
        global::Doroti.Ui.Offset deltaPosition__91978 = Offset.zero;
        double topBound__92024 = -((global::Doroti.Framework.Painting.EdgeInsets)this.floatingCursorAddedMargin).top;
        double bottomBound__92084 = ((Math.Min(size.height, ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).height) - this.preferredLineHeight) + ((global::Doroti.Framework.Painting.EdgeInsets)this.floatingCursorAddedMargin).bottom);
        double leftBound__92240 = -((global::Doroti.Framework.Painting.EdgeInsets)this.floatingCursorAddedMargin).left;
        double rightBound__92302 = (Math.Min(size.width, ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).width) + ((global::Doroti.Framework.Painting.EdgeInsets)this.floatingCursorAddedMargin).right);
        var boundingRects__92409 = global::Doroti.Ui.Rect.fromLTRB(leftBound__92240, topBound__92024, rightBound__92302, bottomBound__92084);
        if ((shouldResetOrigin is not null))
        {
            bool shouldResetOrigin__value92495 = DartRuntimePrimitives.RequireValue(shouldResetOrigin);
            _shouldResetOrigin = DartRuntimePrimitives.RequireValue(shouldResetOrigin__value92495);
        }
        if (!this._shouldResetOrigin)
        {
            return _calculateAdjustedCursorOffset(rawCursorOffset, boundingRects__92409);
        }
        if ((this._previousOffset is not null))
        {
            deltaPosition__91978 = (rawCursorOffset - DartRuntimePrimitives.RequireValue(this._previousOffset));
        }
        if ((this._resetOriginOnLeft && (deltaPosition__91978.dx > 0L)))
        {
            _relativeOrigin = new global::Doroti.Ui.Offset((rawCursorOffset.dx - boundingRects__92409.left), this._relativeOrigin.dy);
            _resetOriginOnLeft = false;
        }
        else
        {
            if ((this._resetOriginOnRight && (deltaPosition__91978.dx < 0L)))
            {
                _relativeOrigin = new global::Doroti.Ui.Offset((rawCursorOffset.dx - boundingRects__92409.right), this._relativeOrigin.dy);
                _resetOriginOnRight = false;
            }
        }
        if ((this._resetOriginOnTop && (deltaPosition__91978.dy > 0L)))
        {
            _relativeOrigin = new global::Doroti.Ui.Offset(this._relativeOrigin.dx, (rawCursorOffset.dy - boundingRects__92409.top));
            _resetOriginOnTop = false;
        }
        else
        {
            if ((this._resetOriginOnBottom && (deltaPosition__91978.dy < 0L)))
            {
                _relativeOrigin = new global::Doroti.Ui.Offset(this._relativeOrigin.dx, (rawCursorOffset.dy - boundingRects__92409.bottom));
                _resetOriginOnBottom = false;
            }
        }
        double currentX__93721 = (rawCursorOffset.dx - this._relativeOrigin.dx);
        double currentY__93790 = (rawCursorOffset.dy - this._relativeOrigin.dy);
        global::Doroti.Ui.Offset adjustedOffset__93859 = _calculateAdjustedCursorOffset(new global::Doroti.Ui.Offset(currentX__93721, currentY__93790), boundingRects__92409);
        if (((currentX__93721 < boundingRects__92409.left) && (deltaPosition__91978.dx < 0L)))
        {
            _resetOriginOnLeft = true;
        }
        else
        {
            if (((currentX__93721 > boundingRects__92409.right) && (deltaPosition__91978.dx > 0L)))
            {
                _resetOriginOnRight = true;
            }
        }
        if (((currentY__93790 < boundingRects__92409.top) && (deltaPosition__91978.dy < 0L)))
        {
            _resetOriginOnTop = true;
        }
        else
        {
            if (((currentY__93790 > boundingRects__92409.bottom) && (deltaPosition__91978.dy > 0L)))
            {
                _resetOriginOnBottom = true;
            }
        }
        _previousOffset = rawCursorOffset;
        return adjustedOffset__93859;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void setFloatingCursor(FloatingCursorDragState state, Offset boundedOffset, TextPosition lastTextPosition, double? resetLerpValue = null)
    {
        if ((object.Equals(state, FloatingCursorDragState.End)))
        {
            _relativeOrigin = Offset.zero;
            _previousOffset = null;
            _shouldResetOrigin = true;
            _resetOriginOnBottom = false;
            _resetOriginOnTop = false;
            _resetOriginOnRight = false;
            _resetOriginOnBottom = false;
        }
        _floatingCursorOn = (!object.Equals(state, FloatingCursorDragState.End));
        _resetFloatingCursorAnimationValue = resetLerpValue;
        if (this._floatingCursorOn)
        {
            _floatingCursorTextPosition = lastTextPosition;
            double? animationValue__95373 = this._resetFloatingCursorAnimationValue;
            global::Doroti.Framework.Painting.EdgeInsets sizeAdjustment__95449 = ((animationValue__95373 is not null) ? EdgeInsets.lerp(EditableLibrary._kFloatingCursorSizeIncrease, global::Doroti.Framework.Painting.EdgeInsets.zero, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(animationValue__95373)))! : EditableLibrary._kFloatingCursorSizeIncrease);
            this._caretPainter.floatingCursorRect = sizeAdjustment__95449.inflateRect(this._caretPrototype).shift(boundedOffset);
        }
        else
        {
            this._caretPainter.floatingCursorRect = null;
        }
        this._caretPainter.showRegularCaret = (this._resetFloatingCursorAnimationValue is null);
    }

    internal virtual MapEntry<long, global::Doroti.Ui.Offset> _lineNumberFor(TextPosition startPosition, List<LineMetrics> metrics)
    {
        global::Doroti.Ui.Offset offset__96141 = this._textPainter.getOffsetForCaret(startPosition, Rect.zero);
        foreach (var lineMetrics__96223 in metrics)
        {
            if ((lineMetrics__96223.baseline > offset__96141.dy))
            {
                return new MapEntry<long, global::Doroti.Ui.Offset>(lineMetrics__96223.lineNumber, new global::Doroti.Ui.Offset(offset__96141.dx, lineMetrics__96223.baseline));
            }
        }
        DartRuntimePrimitives.Assert(() => (startPosition.offset == 0L));
        return new MapEntry<long, global::Doroti.Ui.Offset>(Math.Max(0L, (checked((long)(metrics.Count)) - 1L)), new global::Doroti.Ui.Offset(offset__96141.dx, ((checked((long)(metrics.Count)) != 0) ? (metrics.Last().baseline + metrics.Last().descent) : 0.0)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual VerticalCaretMovementRun startVerticalCaretMovement(TextPosition startPosition)
    {
        List<global::Doroti.Ui.LineMetrics> metrics__97528 = this._textPainter.computeLineMetrics();
        MapEntry<long, global::Doroti.Ui.Offset> currentLine__97605 = _lineNumberFor(startPosition, metrics__97528);
        return new VerticalCaretMovementRun(this, metrics__97528, startPosition, currentLine__97605.key, currentLine__97605.value);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paintContents(PaintingContext context, Offset offset)
    {
        global::Doroti.Ui.Offset effectiveOffset__97887 = (offset + this._paintOffset);
        if (((this.selection is not null) && !this._floatingCursorOn))
        {
            _updateSelectionExtentsVisibility(effectiveOffset__97887);
        }
        RenderBox? foregroundChild__98066 = this._foregroundRenderObject;
        RenderBox? backgroundChild__98130 = this._backgroundRenderObject;
        if ((backgroundChild__98130 is not null))
        {
            context.paintChild(backgroundChild__98130, offset);
        }
        this._textPainter.paint(((PaintingContext)context).canvas, effectiveOffset__97887);
        paintInlineChildren(context, effectiveOffset__97887);
        if ((foregroundChild__98066 is not null))
        {
            context.paintChild(foregroundChild__98066, offset);
        }
    }

    internal virtual void _paintHandleLayers(PaintingContext context, List<TextSelectionPoint> endpoints, Offset offset)
    {
        global::Doroti.Ui.Offset startPoint__98835 = endpoints[(int)(0L)].point;
        startPoint__98835 = new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble(startPoint__98835.dx, 0.0, size.width), Dart_uiLibrary.clampDouble(startPoint__98835.dy, 0.0, size.height));
        this._leaderLayerHandler.layer = new LeaderLayer(link: this.startHandleLayerLink, offset: (startPoint__98835 + offset));
        context.pushLayer(((LayerHandle<LeaderLayer>)this._leaderLayerHandler).layer!, (Action<PaintingContext, Offset>)base.paint, Offset.zero);
        if ((checked((long)(endpoints.Count)) == 2L))
        {
            global::Doroti.Ui.Offset endPoint__99247 = endpoints[(int)(1L)].point;
            endPoint__99247 = new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble(endPoint__99247.dx, 0.0, size.width), Dart_uiLibrary.clampDouble(endPoint__99247.dy, 0.0, size.height));
            context.pushLayer(new LeaderLayer(link: this.endHandleLayerLink, offset: (endPoint__99247 + offset)), (Action<PaintingContext, Offset>)base.paint, Offset.zero);
        }
        else
        {
            if (this.selection!.isCollapsed)
            {
                context.pushLayer(new LeaderLayer(link: this.endHandleLayerLink, offset: (startPoint__98835 + offset)), (Action<PaintingContext, Offset>)base.paint, Offset.zero);
            }
        }
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)(object)child;
        if (((object.Equals(__child, this._foregroundRenderObject)) || (object.Equals(__child, this._backgroundRenderObject))))
        {
            return;
        }
        defaultApplyPaintTransform(__child, transform);
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        _computeTextMetricsIfNeeded();
        if ((this._hasVisualOverflow && (!object.Equals(this.clipBehavior, Clip.none))))
        {
            this._clipRectLayer.layer = context.pushClipRect(needsCompositing, offset, (Offset.zero & size), (Action<PaintingContext, Offset>)this._paintContents, clipBehavior: this.clipBehavior, oldLayer: ((LayerHandle<ClipRectLayer>)this._clipRectLayer).layer);
        }
        else
        {
            this._clipRectLayer.layer = null;
            _paintContents(context, offset);
        }
        TextSelection? selection__100510 = this.selection;
        if (((selection__100510 is not null) && selection__100510.isValid))
        {
            _paintHandleLayers(context, getEndpointsForSelection(selection__100510), offset);
        }
    }

    public override Rect? describeApproximatePaintClip(RenderObject child)
    {
        switch (this.clipBehavior)
        {
            case Clip.none:
                {
                    return null;
                }
            case Clip.hardEdge:
            case Clip.antiAlias:
            case Clip.antiAliasWithSaveLayer:
                {
                    return (this._hasVisualOverflow ? (Offset.zero & size) : null);
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("cursorColor", this.cursorColor));
        properties.add(new DiagnosticsProperty<ValueNotifier<bool>>("showCursor", this.showCursor));
        properties.add(new IntProperty("maxLines", this.maxLines));
        properties.add(new IntProperty("minLines", this.minLines));
        properties.add(new DiagnosticsProperty<bool>("expands", this.expands, defaultValue: false));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("selectionColor", this.selectionColor));
        properties.add(new DiagnosticsProperty<global::Doroti.Framework.Painting.TextScaler>("textScaler", this.textScaler, defaultValue: global::Doroti.Framework.Painting.TextScaler.noScaling));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Locale>("locale", this.locale, defaultValue: null));
        properties.add(new DiagnosticsProperty<TextSelection>("selection", this.selection));
        properties.add(new DiagnosticsProperty<ViewportOffset>("offset", this.offset));
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        return new List<DiagnosticsNode>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _scheduleSystemFontsUpdate()
    {
        if (this._hasPendingSystemFontsDidChangeCallBack)
        {
            return;
        }
        this._hasPendingSystemFontsDidChangeCallBack = true;
        SchedulerBinding.instance.scheduleFrameCallback(((timeStamp) =>
        {
            DartRuntimePrimitives.Assert(() => this._hasPendingSystemFontsDidChangeCallBack);
            this._hasPendingSystemFontsDidChangeCallBack = false;
            DartRuntimePrimitives.Assert(() => (attached || ((debugDisposed ?? true))));
            if (attached)
            {
                systemFontsDidChange();
            }
        }));
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173585 = ((TextParentData?)(object?)child.parentData!)!;
        while ((childParentData__173585.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173585.previousSibling, child)));
            child = childParentData__173585.previousSibling!;
            childParentData__173585 = ((TextParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173981 = ((TextParentData?)(object?)child.parentData!)!;
        while ((childParentData__173981.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173981.nextSibling, child)));
            child = childParentData__173981.nextSibling!;
            childParentData__173981 = ((TextParentData?)(object?)child.parentData!)!;
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
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderBox)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((RenderObject)child).debugCreator, style: DiagnosticsTreeStyle.errorProperty) });
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData__175971 = ((TextParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData__175971.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData__175971.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData__175971.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData__176343 = ((TextParentData?)(object?)this._firstChild!.parentData!)!;
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
            var afterParentData__176766 = ((TextParentData?)(object?)after.parentData!)!;
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
                var childPreviousSiblingParentData__177424 = ((TextParentData?)(object?)childParentData__175971.previousSibling!.parentData!)!;
                var childNextSiblingParentData__177547 = ((TextParentData?)(object?)childParentData__175971.nextSibling!.parentData!)!;
                childPreviousSiblingParentData__177424.nextSibling = child;
                childNextSiblingParentData__177547.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData__176766.nextSibling, child)));
            }
        }
    }

    public virtual void insert(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._firstChild)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._lastChild)));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => (child.parentData is TextParentData));
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderBox child)
    {
        insert(child, after: this._lastChild);
    }

    public virtual void addAll(List<RenderBox>? children)
    {
        children?.forEach(this.add);
    }

    public virtual void _removeFromChildList(RenderBox child)
    {
        var childParentData__179226 = ((TextParentData?)(object?)child.parentData!)!;
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
            var childPreviousSiblingParentData__179613 = ((TextParentData?)(object?)childParentData__179226.previousSibling!.parentData!)!;
            childPreviousSiblingParentData__179613.nextSibling = childParentData__179226.nextSibling;
        }
        if ((childParentData__179226.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData__179226.previousSibling;
        }
        else
        {
            var childNextSiblingParentData__179965 = ((TextParentData?)(object?)childParentData__179226.nextSibling!.parentData!)!;
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
            var childParentData__180684 = ((TextParentData?)(object?)child__180623.parentData!)!;
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
        var childParentData__181479 = ((TextParentData?)(object?)child.parentData!)!;
        if ((object.Equals(childParentData__181479.previousSibling, after)))
        {
            return;
        }
        _removeFromChildList(child);
        _insertIntoChildList(child, after: after);
        markNeedsLayout();
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183103 = ((TextParentData?)(object?)child.parentData!)!;
        return childParentData__183103.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183356 = ((TextParentData?)(object?)child.parentData!)!;
        return childParentData__183356.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void setupParentData(RenderObject child)
    {
        var __child = (RenderBox)(object)child;
        if ((__child.parentData is not TextParentData))
        {
            __child.parentData = new TextParentData();
        }
    }

    public virtual List<global::Doroti.Framework.Painting.PlaceholderDimensions> layoutInlineChildren(double maxWidth, Func<RenderBox, BoxConstraints, Size> layoutChild, Func<RenderBox, BoxConstraints, TextBaseline, double?> getChildBaseline)
    {
        var constraints__7015 = new BoxConstraints(maxWidth: maxWidth);
        return new List<global::Doroti.Framework.Painting.PlaceholderDimensions>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void positionInlineChildren(List<TextBox> boxes)
    {
        RenderBox? child__7901 = firstChild;
        foreach (var box__7936 in boxes)
        {
            if ((child__7901 is null))
            {
                DartRuntimePrimitives.Assert(() =>
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Invalid number of boxes provided to positionInlineChildren."), new ErrorDescription($"The number of boxes ({checked((long)(boxes.Count))}) exceeds the number of child render objects ({childCount}). " + "Each box corresponds to a child, but there are not enough children to position all boxes."), new ErrorHint("This error typically occurs when a custom InlineSpan implementation returns a list of boxes " + "that is longer than the number of inline children. Ensure that the number of boxes returned " + "by `computeLineMetrics` or similar methods does not exceed the number of children."), new DiagnosticsProperty<RenderObject>("The RenderParagraph receiving the boxes", this, style: DiagnosticsTreeStyle.errorProperty) });
                    });
                return;
            }
            var textParentData__9027 = ((TextParentData?)(object?)child__7901.parentData!)!;
            textParentData__9027._offset = new global::Doroti.Ui.Offset(box__7936.left, box__7936.top);
            child__7901 = childAfter(child__7901);
        }
        while ((child__7901 is not null))
        {
            var textParentData__9218 = ((TextParentData?)(object?)child__7901.parentData!)!;
            textParentData__9218._offset = null;
            child__7901 = childAfter(child__7901);
        }
    }

    public virtual void defaultApplyPaintTransform(RenderBox child, Matrix4 transform)
    {
        var childParentData__9711 = ((TextParentData?)(object?)child.parentData!)!;
        global::Doroti.Ui.Offset? offset__9784 = ((TextParentData)childParentData__9711).offset;
        if ((offset__9784 is null))
        {
            transform.setZero();
        }
        else
        {
            transform.translateByDouble(DartRuntimePrimitives.RequireValue(offset__9784).dx, DartRuntimePrimitives.RequireValue(offset__9784).dy, 0, 1);
        }
    }

    public virtual void paintInlineChildren(PaintingContext context, Offset offset)
    {
        RenderBox? child__10190 = firstChild;
        while ((child__10190 is not null))
        {
            var childParentData__10250 = ((TextParentData?)(object?)child__10190.parentData!)!;
            global::Doroti.Ui.Offset? childOffset__10325 = ((TextParentData)childParentData__10250).offset;
            if ((childOffset__10325 is null))
            {
                return;
            }
            context.paintChild(child__10190, (DartRuntimePrimitives.RequireValue(childOffset__10325) + offset));
            child__10190 = childAfter(child__10190);
        }
    }

    public virtual bool hitTestInlineChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child__10772 = firstChild;
        while ((child__10772 is not null))
        {
            var childParentData__10832 = ((TextParentData?)(object?)child__10772.parentData!)!;
            global::Doroti.Ui.Offset? childOffset__10907 = ((TextParentData)childParentData__10832).offset;
            if ((childOffset__10907 is null))
            {
                return false;
            }
            bool isHit__11025 = result.addWithPaintOffset(offset: DartRuntimePrimitives.RequireValue(childOffset__10907), position: position, hitTest: ((Func<BoxHitTestResult, Offset, bool>)((result, transformed) => child__10772!.hitTest(result, position: transformed))));
            if (isHit__11025)
            {
                return true;
            }
            child__10772 = childAfter(child__10772);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _RenderEditableCustomPaint__editable : RenderBox
{
    internal virtual RenderEditablePainter? _painter { get; set; } = default;

    internal _RenderEditableCustomPaint__editable(RenderEditablePainter? painter = null)
    {
        this._painter = painter;
    }

    public override RenderEditable? parent => ((RenderEditable?)(object?)base.parent)!;
    public override bool isRepaintBoundary => true;
    public override bool sizedByParent => true;
    public virtual RenderEditablePainter? painter
    {
        get => this._painter;
        set
        {
            var newValue = value;
            if ((object.Equals(newValue, this.painter)))
            {
                return;
            }
            RenderEditablePainter? oldPainter__102748 = this.painter;
            _painter = newValue;
            if ((newValue?.shouldRepaint(oldPainter__102748) ?? true))
            {
                markNeedsPaint();
            }
            if (attached)
            {
                oldPainter__102748?.removeListener(markNeedsPaint);
                newValue?.addListener(markNeedsPaint);
            }
        }
    }
    public override void paint(PaintingContext context, Offset offset)
    {
        RenderEditable? parent__103101 = this.parent;
        DartRuntimePrimitives.Assert(() => (parent__103101 is not null));
        RenderEditablePainter? painter__103184 = this.painter;
        if (((painter__103184 is not null) && (parent__103101 is not null)))
        {
            parent__103101._computeTextMetricsIfNeeded();
            painter__103184.paint(((PaintingContext)context).canvas, size, parent__103101);
        }
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        this._painter?.addListener(markNeedsPaint);
    }

    public override void detach()
    {
        this._painter?.removeListener(markNeedsPaint);
        base.detach();
    }

    public override Size computeDryLayout(BoxConstraints constraints) => ((BoxConstraints)constraints).biggest;
}

public abstract class RenderEditablePainter : ChangeNotifier
{
    public abstract bool shouldRepaint(RenderEditablePainter? oldDelegate);
    public abstract void paint(Canvas canvas, Size size, RenderEditable renderEditable);
}

internal class _TextHighlightPainter__editable : RenderEditablePainter
{
    public virtual Paint highlightPaint { get; private set; } = new global::Doroti.Ui.Paint();
    internal virtual Color? _highlightColor { get; set; } = default;
    internal virtual TextRange? _highlightedRange { get; set; } = default;
    internal virtual BoxHeightStyle _selectionHeightStyle { get; set; } = Dart_uiLibrary.BoxHeightStyle.tight;
    internal virtual BoxWidthStyle _selectionWidthStyle { get; set; } = Dart_uiLibrary.BoxWidthStyle.tight;

    internal _TextHighlightPainter__editable(TextRange? highlightedRange = null, Color? highlightColor = null)
    {
        this._highlightedRange = highlightedRange;
        this._highlightColor = highlightColor;
    }

    public virtual global::Doroti.Ui.Color? highlightColor
    {
        get => this._highlightColor;
        set
        {
            var newValue = value is null ? null : (Color)(object)value;
            if ((object.Equals(newValue, this._highlightColor)))
            {
                return;
            }
            _highlightColor = newValue;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.TextRange? highlightedRange
    {
        get => this._highlightedRange;
        set
        {
            var newValue = value is null ? null : (TextRange)(object)value;
            if ((object.Equals(newValue, this._highlightedRange)))
            {
                return;
            }
            _highlightedRange = newValue;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.BoxHeightStyle selectionHeightStyle
    {
        get => this._selectionHeightStyle;
        set
        {
            var __value = value;
            if ((object.Equals(this._selectionHeightStyle, __value)))
            {
                return;
            }
            _selectionHeightStyle = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.BoxWidthStyle selectionWidthStyle
    {
        get => this._selectionWidthStyle;
        set
        {
            var __value = value;
            if ((object.Equals(this._selectionWidthStyle, __value)))
            {
                return;
            }
            _selectionWidthStyle = __value;
            notifyListeners();
        }
    }
    public override void paint(Canvas canvas, Size size, RenderEditable renderEditable)
    {
        global::Doroti.Ui.TextRange? range__108113 = this.highlightedRange;
        global::Doroti.Ui.Color? color__108156 = this.highlightColor;
        if ((((range__108113 is null) || (color__108156 is null)) || range__108113.isCollapsed))
        {
            return;
        }
        this.highlightPaint.color = color__108156;
        global::Doroti.Framework.Painting.TextPainter textPainter__108320 = ((RenderEditable)renderEditable)._textPainter;
        HashSet<global::Doroti.Ui.TextBox> boxes__108386 = textPainter__108320.getBoxesForSelection(new TextSelection(baseOffset: range__108113.start, extentOffset: range__108113.end), boxHeightStyle: this.selectionHeightStyle, boxWidthStyle: this.selectionWidthStyle).toSet();
        foreach (var box__108650 in boxes__108386)
        {
            canvas.drawRect(box__108650.toRect().shift(((RenderEditable)renderEditable)._paintOffset).intersect(global::Doroti.Ui.Rect.fromLTWH(0, 0, ((global::Doroti.Framework.Painting.TextPainter)textPainter__108320).width, ((global::Doroti.Framework.Painting.TextPainter)textPainter__108320).height)), this.highlightPaint);
        }
    }

    public override bool shouldRepaint(RenderEditablePainter? oldDelegate)
    {
        if (DartRuntimePrimitives.Identical(oldDelegate, this))
        {
            return false;
        }
        if ((oldDelegate is null))
        {
            return ((this.highlightColor is not null) && (this.highlightedRange is not null));
        }
        return (((((oldDelegate is not _TextHighlightPainter__editable) || (!object.Equals(((_TextHighlightPainter__editable)((_TextHighlightPainter__editable)oldDelegate)).highlightColor, this.highlightColor))) || (!object.Equals(((_TextHighlightPainter__editable)((_TextHighlightPainter__editable)oldDelegate)).highlightedRange, this.highlightedRange))) || (!object.Equals(((_TextHighlightPainter__editable)((_TextHighlightPainter__editable)oldDelegate)).selectionHeightStyle, this.selectionHeightStyle))) || (!object.Equals(((_TextHighlightPainter__editable)((_TextHighlightPainter__editable)oldDelegate)).selectionWidthStyle, this.selectionWidthStyle)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CaretPainter__editable : RenderEditablePainter
{
    internal virtual bool _shouldPaint { get; set; } = true;
    public virtual bool showRegularCaret { get; set; } = false;
    public virtual Paint caretPaint { get; private set; } = new global::Doroti.Ui.Paint();
    private bool __late_floatingCursorPaint_initialized;
    private Paint __late_floatingCursorPaint = default!;
    public virtual Paint floatingCursorPaint
    {
        get
        {
            if (!__late_floatingCursorPaint_initialized)
            {
                __late_floatingCursorPaint = new global::Doroti.Ui.Paint();
                __late_floatingCursorPaint_initialized = true;
            }
            return __late_floatingCursorPaint;
        }
    }
    internal virtual Color? _caretColor { get; set; } = default;
    internal virtual Radius? _cursorRadius { get; set; } = default;
    internal virtual Offset _cursorOffset { get; set; } = Offset.zero;
    internal virtual Color? _backgroundCursorColor { get; set; } = default;
    internal virtual Rect? _floatingCursorRect { get; set; } = default;

    internal _CaretPainter__editable()
    {
    }

    public virtual bool shouldPaint
    {
        get => this._shouldPaint;
        set
        {
            var __value = value;
            if ((this.shouldPaint == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _shouldPaint = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color? caretColor
    {
        get => this._caretColor;
        set
        {
            var __value = value is null ? null : (Color)(object)value;
            if ((this.caretColor?.value == __value?.value))
            {
                return;
            }
            _caretColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Radius? cursorRadius
    {
        get => this._cursorRadius;
        set
        {
            var __value = value;
            if ((object.Equals(this._cursorRadius, __value)))
            {
                return;
            }
            _cursorRadius = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Offset cursorOffset
    {
        get => this._cursorOffset;
        set
        {
            var __value = value;
            if ((object.Equals(this._cursorOffset, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _cursorOffset = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color? backgroundCursorColor
    {
        get => this._backgroundCursorColor;
        set
        {
            var __value = value is null ? null : (Color)(object)value;
            if ((this.backgroundCursorColor?.value == __value?.value))
            {
                return;
            }
            _backgroundCursorColor = __value;
            if (this.showRegularCaret)
            {
                notifyListeners();
            }
        }
    }
    public virtual global::Doroti.Ui.Rect? floatingCursorRect
    {
        get => this._floatingCursorRect;
        set
        {
            var __value = value;
            if ((object.Equals(this._floatingCursorRect, __value)))
            {
                return;
            }
            _floatingCursorRect = __value;
            notifyListeners();
        }
    }
    public virtual void paintRegularCursor(Canvas canvas, RenderEditable renderEditable, Color caretColor, TextPosition textPosition)
    {
        global::Doroti.Ui.Rect integralRect__111411 = renderEditable.getLocalRectForCaret(textPosition);
        if (this.shouldPaint)
        {
            if ((this.floatingCursorRect is not null))
            {
                double distanceSquared__111561 = ((DartRuntimePrimitives.RequireValue(this.floatingCursorRect).center - integralRect__111411.center)).distanceSquared;
                if ((distanceSquared__111561 < EditableLibrary._kShortestDistanceSquaredWithFloatingAndRegularCursors))
                {
                    return;
                }
            }
            global::Doroti.Ui.Radius? radius__111803 = this.cursorRadius;
            this.caretPaint.color = caretColor;
            if ((radius__111803 is null))
            {
                canvas.drawRect(integralRect__111411, this.caretPaint);
            }
            else
            {
                var caretRRect__111971 = global::Doroti.Ui.RRect.fromRectAndRadius(integralRect__111411, DartRuntimePrimitives.RequireValue(radius__111803));
                canvas.drawRRect(caretRRect__111971, this.caretPaint);
            }
        }
    }

    public override void paint(Canvas canvas, Size size, RenderEditable renderEditable)
    {
        TextSelection? selection__112278 = ((RenderEditable)renderEditable).selection;
        if ((((selection__112278 is null) || !selection__112278.isCollapsed) || !selection__112278.isValid))
        {
            return;
        }
        global::Doroti.Ui.Rect? floatingCursorRect__112431 = this.floatingCursorRect;
        global::Doroti.Ui.Color? caretColor__112495 = ((floatingCursorRect__112431 is null) ? this.caretColor : (this.showRegularCaret ? this.backgroundCursorColor : null));
        global::Doroti.Ui.TextPosition caretTextPosition__112659 = ((floatingCursorRect__112431 is null) ? selection__112278.extent : ((RenderEditable)renderEditable)._floatingCursorTextPosition);
        if ((caretColor__112495 is not null))
        {
            paintRegularCursor(canvas, renderEditable, caretColor__112495, caretTextPosition__112659);
        }
        global::Doroti.Ui.Color? floatingCursorColor__112923 = this.caretColor?.withOpacity(0.75);
        if ((((floatingCursorRect__112431 is null) || (floatingCursorColor__112923 is null)) || !this.shouldPaint))
        {
            return;
        }
        canvas.drawRRect(global::Doroti.Ui.RRect.fromRectAndRadius(DartRuntimePrimitives.RequireValue(floatingCursorRect__112431), EditableLibrary._kFloatingCursorRadius), ((Func<Paint>)(() =>
{
    var __cascade = this.floatingCursorPaint;
    __cascade.color = floatingCursorColor__112923;
    return __cascade;
}))());
    }

    public override bool shouldRepaint(RenderEditablePainter? oldDelegate)
    {
        if (DartRuntimePrimitives.Identical(this, oldDelegate))
        {
            return false;
        }
        if ((oldDelegate is null))
        {
            return this.shouldPaint;
        }
        return ((((((((oldDelegate is not _CaretPainter__editable) || (((_CaretPainter__editable)((_CaretPainter__editable)oldDelegate)).shouldPaint != this.shouldPaint)) || (((_CaretPainter__editable)((_CaretPainter__editable)oldDelegate)).showRegularCaret != this.showRegularCaret)) || (!object.Equals(((_CaretPainter__editable)((_CaretPainter__editable)oldDelegate)).caretColor, this.caretColor))) || (!object.Equals(((_CaretPainter__editable)((_CaretPainter__editable)oldDelegate)).cursorRadius, this.cursorRadius))) || (!object.Equals(((_CaretPainter__editable)((_CaretPainter__editable)oldDelegate)).cursorOffset, this.cursorOffset))) || (!object.Equals(((_CaretPainter__editable)((_CaretPainter__editable)oldDelegate)).backgroundCursorColor, this.backgroundCursorColor))) || (!object.Equals(((_CaretPainter__editable)((_CaretPainter__editable)oldDelegate)).floatingCursorRect, this.floatingCursorRect)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CompositeRenderEditablePainter__editable : RenderEditablePainter
{
    public virtual List<RenderEditablePainter> painters { get; private set; } = default!;

    internal _CompositeRenderEditablePainter__editable(List<RenderEditablePainter> painters)
    {
        this.painters = painters;
    }

    public virtual void addListener(Action listener)
    {
        foreach (RenderEditablePainter painter__114194 in this.painters)
        {
            painter__114194.addListener(listener);
        }
    }

    public virtual void removeListener(Action listener)
    {
        foreach (RenderEditablePainter painter__114361 in this.painters)
        {
            painter__114361.removeListener(listener);
        }
    }

    public override void paint(Canvas canvas, Size size, RenderEditable renderEditable)
    {
        foreach (RenderEditablePainter painter__114556 in this.painters)
        {
            painter__114556.paint(canvas, size, renderEditable);
        }
    }

    public override bool shouldRepaint(RenderEditablePainter? oldDelegate)
    {
        if (DartRuntimePrimitives.Identical(oldDelegate, this))
        {
            return false;
        }
        if (((oldDelegate is not _CompositeRenderEditablePainter__editable) || (checked((long)(((_CompositeRenderEditablePainter__editable)((_CompositeRenderEditablePainter__editable)oldDelegate)).painters.Count)) != checked((long)(this.painters.Count)))))
        {
            return true;
        }
        IEnumerator<RenderEditablePainter> oldPainters__114963 = ((_CompositeRenderEditablePainter__editable)((_CompositeRenderEditablePainter__editable)oldDelegate)).painters.GetEnumerator();
        IEnumerator<RenderEditablePainter> newPainters__115050 = this.painters.GetEnumerator();
        while ((oldPainters__114963.MoveNext() && newPainters__115050.MoveNext()))
        {
            if (newPainters__115050.Current.shouldRepaint(oldPainters__114963.Current))
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
