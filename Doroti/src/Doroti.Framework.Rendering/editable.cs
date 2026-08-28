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
            List<global::Doroti.Ui.LineMetrics> newLineMetrics = ((RenderEditable)this._editable)._textPainter.computeLineMetrics();
            if (!DartRuntimePrimitives.Identical(newLineMetrics, this._lineMetrics))
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
        MapEntry<global::Doroti.Ui.Offset, global::Doroti.Ui.TextPosition>? cachedPosition = this._positionCache.GetValueOrDefault(lineNumber);
        if ((cachedPosition is not null))
        {
            MapEntry<Offset, TextPosition> cachedPosition__6901__value6954 = DartRuntimePrimitives.RequireValue(cachedPosition);
            return DartRuntimePrimitives.RequireValue(cachedPosition__6901__value6954);
        }
        DartRuntimePrimitives.Assert(() => (lineNumber != this._currentLine));
        var newOffset = new global::Doroti.Ui.Offset(this._currentOffset.dx, this._lineMetrics[(int)(lineNumber)].baseline);
        global::Doroti.Ui.TextPosition closestPosition = ((RenderEditable)this._editable)._textPainter.getPositionForOffset(newOffset);
        var position = new MapEntry<global::Doroti.Ui.Offset, global::Doroti.Ui.TextPosition>(newOffset, closestPosition);
        this._positionCache[lineNumber] = position;
        return position;
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
        MapEntry<global::Doroti.Ui.Offset, global::Doroti.Ui.TextPosition> position = _getTextPositionForLine((this._currentLine + 1L));
        _currentLine += 1L;
        _currentOffset = position.key;
        _currentTextPosition = position.value;
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
        MapEntry<global::Doroti.Ui.Offset, global::Doroti.Ui.TextPosition> position = _getTextPositionForLine((this._currentLine - 1L));
        _currentLine -= 1L;
        _currentOffset = position.key;
        _currentTextPosition = position.value;
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool moveByOffset(double offset)
    {
        global::Doroti.Ui.Offset initialOffset = this._currentOffset;
        if ((offset >= 0.0))
        {
            while ((this._currentOffset.dy < (initialOffset.dy + offset)))
            {
                if (!moveNext())
                {
                    break;
                }
            }
        }
        else
        {
            while ((this._currentOffset.dy > (initialOffset.dy + offset)))
            {
                if (!movePrevious())
                {
                    break;
                }
            }
        }
        return (!object.Equals(initialOffset, this._currentOffset));
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
        System.Diagnostics.Debug.Assert((!this._showCursor.value || (cursorColor is not null)));
        this._selectionPainter.highlightColor = selectionColor;
        this._selectionPainter.highlightedRange = selection;
        this._selectionPainter.selectionHeightStyle = selectionHeightStyle;
        this._selectionPainter.selectionWidthStyle = selectionWidthStyle;
        this._autocorrectHighlightPainter.highlightColor = promptRectColor;
        this._autocorrectHighlightPainter.highlightedRange = promptRectRange;
        this._caretPainter.caretColor = cursorColor;
        this._caretPainter.cursorRadius = cursorRadius;
        this._caretPainter.cursorOffset = cursorOffset;
        this._caretPainter.backgroundCursorColor = backgroundCursorColor;
        _updateForegroundPainter(foregroundPainter);
        _updatePainter(painter);
        addAll(children);
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
        _CompositeRenderEditablePainter__editable effectivePainter = ((newPainter is null) ? this._builtInForegroundPainters : new _CompositeRenderEditablePainter__editable(painters: new List<RenderEditablePainter> { this._builtInForegroundPainters, newPainter }));
        if ((this._foregroundRenderObject is null))
        {
            var foregroundRenderObject = new _RenderEditableCustomPaint__editable(painter: effectivePainter);
            adoptChild(foregroundRenderObject);
            _foregroundRenderObject = foregroundRenderObject;
        }
        else
        {
            this._foregroundRenderObject?.painter = effectivePainter;
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
        _CompositeRenderEditablePainter__editable effectivePainter = ((newPainter is null) ? this._builtInPainters : new _CompositeRenderEditablePainter__editable(painters: new List<RenderEditablePainter> { this._builtInPainters, newPainter }));
        if ((this._backgroundRenderObject is null))
        {
            var backgroundRenderObject = new _RenderEditableCustomPaint__editable(painter: effectivePainter);
            adoptChild(backgroundRenderObject);
            _backgroundRenderObject = backgroundRenderObject;
        }
        else
        {
            this._backgroundRenderObject?.painter = effectivePainter;
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
        var painters = new List<RenderEditablePainter>();
        if (this.paintCursorAboveText)
        {
            painters.Add(this._caretPainter);
        }
        return new _CompositeRenderEditablePainter__editable(painters: painters);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _CompositeRenderEditablePainter__editable _builtInPainters => _cachedBuiltInPainters ??= _createBuiltInPainters();
    internal virtual _CompositeRenderEditablePainter__editable _createBuiltInPainters()
    {
        var painters = new List<RenderEditablePainter> { this._autocorrectHighlightPainter, this._selectionPainter };
        if (!this.paintCursorAboveText)
        {
            painters.Add(this._caretPainter);
        }
        return new _CompositeRenderEditablePainter__editable(painters: painters);
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
        global::Doroti.Ui.Offset caretOffset = this._textPainter.getOffsetForCaret(position, this._caretPrototype);
        global::Doroti.Ui.Offset caretOffsetTranslated = caretOffset.translate(0.0, verticalOffset);
        return this._textPainter.getPositionForOffset(caretOffsetTranslated);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TextSelection getLineAtOffset(TextPosition position)
    {
        global::Doroti.Ui.TextRange line = this._textPainter.getLineBoundary(position);
        if (this.obscureText)
        {
            return new TextSelection(baseOffset: 0L, extentOffset: this.plainText.Length);
        }
        return new TextSelection(baseOffset: line.start, extentOffset: line.end);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.TextRange getWordBoundary(TextPosition position)
    {
        return this._textPainter.getWordBoundary(position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.TextPosition getTextPositionAbove(TextPosition position)
    {
        double preferredLineHeightLocal = ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).preferredLineHeight;
        double verticalOffset = (-0.5 * preferredLineHeightLocal);
        return _getTextPositionVertical(position, verticalOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.TextPosition getTextPositionBelow(TextPosition position)
    {
        double preferredLineHeightLocal = ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).preferredLineHeight;
        double verticalOffset = (1.5 * preferredLineHeightLocal);
        return _getTextPositionVertical(position, verticalOffset);
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
        global::Doroti.Ui.Rect visibleRegion = (Offset.zero & size);
        global::Doroti.Ui.Offset startOffset = this._textPainter.getOffsetForCaret(new global::Doroti.Ui.TextPosition(offset: this.selection!.start, affinity: this.selection!.affinity), this._caretPrototype);
        var visibleRegionSlop = 0.5;
        this._selectionStartInViewport.value = visibleRegion.inflate(visibleRegionSlop).contains((startOffset + effectiveOffset));
        global::Doroti.Ui.Offset endOffset = this._textPainter.getOffsetForCaret(new global::Doroti.Ui.TextPosition(offset: this.selection!.end, affinity: this.selection!.affinity), this._caretPrototype);
        this._selectionEndInViewport.value = visibleRegion.inflate(visibleRegionSlop).contains((endOffset + effectiveOffset));
    }

    internal virtual void _setTextEditingValue(TextEditingValue newValue, SelectionChangedCause cause)
    {
        this.textSelectionDelegate.userUpdateTextEditingValue(newValue, cause);
    }

    internal virtual void _setSelection(TextSelection nextSelection, SelectionChangedCause cause)
    {
        if (nextSelection.isValid)
        {
            long textLength = this.textSelectionDelegate.textEditingValue.text.Length;
            nextSelection = nextSelection.copyWith(baseOffset: Math.Min(nextSelection.baseOffset, textLength), extentOffset: Math.Min(nextSelection.extentOffset, textLength));
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
                var buffer = new StringBuffer();
                var offset = 0L;
                var attributesLocal = new List<global::Doroti.Ui.StringAttribute>();
                foreach (global::Doroti.Framework.Painting.InlineSpanSemanticsInformation infoLocal in this._semanticsInfo!)
                {
                    string label = (((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)infoLocal).semanticsLabel ?? ((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)infoLocal).text);
                    foreach (global::Doroti.Ui.StringAttribute infoAttribute in ((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)infoLocal).stringAttributes)
                    {
                        global::Doroti.Ui.TextRange originalRange = infoAttribute.range;
                        attributesLocal.Add(infoAttribute.copy(range: new global::Doroti.Ui.TextRange(start: (offset + originalRange.start), end: (offset + originalRange.end))));
                    }
                    buffer.write(label);
                    offset += label.Length;
                }
                _cachedAttributedValue = new global::Doroti.Framework.Semantics.AttributedString(buffer.ToString(), attributes: attributesLocal);
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
        var newChildren = new List<global::Doroti.Framework.Semantics.SemanticsNode>();
        global::Doroti.Ui.TextDirection currentDirection = this.textDirection;
        global::Doroti.Ui.Rect currentRect = default!;
        var ordinal = 0.0;
        var start = 0L;
        var placeholderIndex = 0L;
        var childIndex = 0L;
        RenderBox? child = firstChild;
        var newChildCache = new DartMap<Key, global::Doroti.Framework.Semantics.SemanticsNode>();
        _cachedCombinedSemanticsInfos ??= global::Doroti.Framework.Painting.Inline_spanLibrary.combineSemanticsInfo(this._semanticsInfo!);
        foreach (global::Doroti.Framework.Painting.InlineSpanSemanticsInformation info in this._cachedCombinedSemanticsInfos!)
        {
            var selection = new TextSelection(baseOffset: start, extentOffset: (start + ((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info).text.Length));
            start += ((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info).text.Length;
            if (((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info).isPlaceholder)
            {
                while (((children.Count() > childIndex) && children.elementAt(childIndex).isTagged(new PlaceholderSpanIndexSemanticsTag(placeholderIndex))))
                {
                    global::Doroti.Framework.Semantics.SemanticsNode childNode = children.elementAt(childIndex);
                    var parentDataLocal = ((TextParentData?)(object?)child!.parentData!)!;
                    DartRuntimePrimitives.Assert(() => (((TextParentData)parentDataLocal).offset is not null));
                    newChildren.Add(childNode);
                    childIndex += 1L;
                }
                child = childAfter(child!);
                placeholderIndex += 1L;
            }
            else
            {
                var initialDirection = currentDirection;
                List<global::Doroti.Ui.TextBox> rects = this._textPainter.getBoxesForSelection(selection);
                if ((checked((long)(rects.Count)) == 0))
                {
                    continue;
                }
                global::Doroti.Ui.Rect rectLocal = rects.First().toRect();
                currentDirection = rects.First().direction;
                foreach (global::Doroti.Ui.TextBox textBox in rects.skip(1L))
                {
                    rectLocal = rectLocal.expandToInclude(textBox.toRect());
                    currentDirection = textBox.direction;
                }
                rectLocal = global::Doroti.Ui.Rect.fromLTWH(Math.Max(0.0, rectLocal.left), Math.Max(0.0, rectLocal.top), Math.Min(rectLocal.width, ((BoxConstraints)constraints).maxWidth), Math.Min(rectLocal.height, ((BoxConstraints)constraints).maxHeight));
                currentRect = global::Doroti.Ui.Rect.fromLTRB((rectLocal.left.floorToDouble() - 4.0), (rectLocal.top.floorToDouble() - 4.0), (rectLocal.right.ceilToDouble() + 4.0), (rectLocal.bottom.ceilToDouble() + 4.0));
                var configuration = ((Func<global::Doroti.Framework.Semantics.SemanticsConfiguration>)(() =>
{
    var __cascade = new global::Doroti.Framework.Semantics.SemanticsConfiguration();
    __cascade.sortKey = new global::Doroti.Framework.Semantics.OrdinalSortKey(ordinal++);
    __cascade.textDirection = initialDirection;
    __cascade.attributedLabel = new global::Doroti.Framework.Semantics.AttributedString((((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info).semanticsLabel ?? ((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info).text), attributes: ((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info).stringAttributes);
    return __cascade;
}))();
                switch (((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info).recognizer)
                {
                    case TapGestureRecognizer { onTap: Action handler } __object55475:
                        {
                            if ((handler is not null))
                            {
                                configuration.onTap = handler;
                                configuration.isLink = true;
                            }
                            break;
                        }
                    case DoubleTapGestureRecognizer { onDoubleTap: Action handlerLocal } __object55548:
                        {
                            if ((handlerLocal is not null))
                            {
                                configuration.onTap = handlerLocal;
                                configuration.isLink = true;
                            }
                            break;
                        }
                    case LongPressGestureRecognizer { onLongPress: Action onLongPressLocal } __object55770:
                        {
                            if ((onLongPressLocal is not null))
                            {
                                configuration.onLongPress = onLongPressLocal;
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
                    global::Doroti.Ui.Rect paintRect = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Semantics.SemanticsNode)node).parentPaintClipRect).intersect(currentRect);
                    configuration.isHidden = (paintRect.isEmpty && !currentRect.isEmpty);
                }
                global::Doroti.Framework.Semantics.SemanticsNode newChild = default!;
                if (((((long?)(this._cachedChildNodes?.Count)) is { } __count56386 ? __count56386 != 0 : (bool?)null) ?? false))
                {
                    newChild = this._cachedChildNodes!.remove(this._cachedChildNodes!.Keys.First())!;
                }
                else
                {
                    var keyLocal = new UniqueKey();
                    newChild = new global::Doroti.Framework.Semantics.SemanticsNode(key: keyLocal, showOnScreen: _createShowOnScreenFor(keyLocal));
                }
                ((Func<global::Doroti.Framework.Semantics.SemanticsNode>)(() =>
{
    var __cascade = newChild;
    __cascade.updateWith(config: configuration);
    __cascade.rect = currentRect;
    return __cascade;
}))();
                newChildCache[((global::Doroti.Framework.Semantics.SemanticsNode)newChild).key!] = newChild;
                newChildren.Add(newChild);
            }
        }
        _cachedChildNodes = newChildCache.cast<Key, global::Doroti.Framework.Semantics.SemanticsNode>();
        node.updateWith(config: config, childrenInInversePaintOrder: newChildren);
    }

    internal virtual Action? _createShowOnScreenFor(Key key)
    {
        return (() =>
        {
            global::Doroti.Framework.Semantics.SemanticsNode node = this._cachedChildNodes!.GetValueOrDefault(key)!;
            showOnScreen(descendant: this, rect: ((global::Doroti.Framework.Semantics.SemanticsNode)node).rect);
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
        long? extentOffsetLocal = this._textPainter.getOffsetAfter(this.selection!.extentOffset);
        if ((extentOffsetLocal is null))
        {
            return;
        }
        long baseOffsetLocal = (!extendSelection ? DartRuntimePrimitives.RequireValue(extentOffsetLocal) : this.selection!.baseOffset);
        _setSelection(new TextSelection(baseOffset: baseOffsetLocal, extentOffset: DartRuntimePrimitives.RequireValue(extentOffsetLocal)), SelectionChangedCause.keyboard);
    }

    internal virtual void _handleMoveCursorBackwardByCharacter(bool extendSelection)
    {
        DartRuntimePrimitives.Assert(() => (this.selection is not null));
        long? extentOffsetLocal = this._textPainter.getOffsetBefore(this.selection!.extentOffset);
        if ((extentOffsetLocal is null))
        {
            return;
        }
        long baseOffsetLocal = (!extendSelection ? DartRuntimePrimitives.RequireValue(extentOffsetLocal) : this.selection!.baseOffset);
        _setSelection(new TextSelection(baseOffset: baseOffsetLocal, extentOffset: DartRuntimePrimitives.RequireValue(extentOffsetLocal)), SelectionChangedCause.keyboard);
    }

    internal virtual void _handleMoveCursorForwardByWord(bool extendSelection)
    {
        DartRuntimePrimitives.Assert(() => (this.selection is not null));
        global::Doroti.Ui.TextRange currentWord = this._textPainter.getWordBoundary(this.selection!.extent);
        global::Doroti.Ui.TextRange? nextWord = _getNextWord(currentWord.end);
        if ((nextWord is null))
        {
            return;
        }
        long baseOffsetLocal = (extendSelection ? this.selection!.baseOffset : nextWord.start);
        _setSelection(new TextSelection(baseOffset: baseOffsetLocal, extentOffset: nextWord.start), SelectionChangedCause.keyboard);
    }

    internal virtual void _handleMoveCursorBackwardByWord(bool extendSelection)
    {
        DartRuntimePrimitives.Assert(() => (this.selection is not null));
        global::Doroti.Ui.TextRange currentWord = this._textPainter.getWordBoundary(this.selection!.extent);
        global::Doroti.Ui.TextRange? previousWord = _getPreviousWord((currentWord.start - 1L));
        if ((previousWord is null))
        {
            return;
        }
        long baseOffsetLocal = (extendSelection ? this.selection!.baseOffset : previousWord.start);
        _setSelection(new TextSelection(baseOffset: baseOffsetLocal, extentOffset: previousWord.start), SelectionChangedCause.keyboard);
    }

    internal virtual global::Doroti.Ui.TextRange? _getNextWord(long offset)
    {
        while (true)
        {
            global::Doroti.Ui.TextRange range = this._textPainter.getWordBoundary(new global::Doroti.Ui.TextPosition(offset: offset));
            if ((!range.isValid || range.isCollapsed))
            {
                return null;
            }
            if (!_onlyWhitespace(range))
            {
                return range;
            }
            offset = range.end;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextRange? _getPreviousWord(long offset)
    {
        while ((offset >= 0L))
        {
            global::Doroti.Ui.TextRange range = this._textPainter.getWordBoundary(new global::Doroti.Ui.TextPosition(offset: offset));
            if ((!range.isValid || range.isCollapsed))
            {
                return null;
            }
            if (!_onlyWhitespace(range))
            {
                return range;
            }
            offset = (range.start - 1L);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _onlyWhitespace(TextRange range)
    {
        for (long i = range.start; (i < range.end); i++)
        {
            long codeUnit = DartRuntimePrimitives.RequireValue(this.text!.codeUnitAt(i));
            if (!TextLayoutMetrics.isWhitespace(codeUnit))
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
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            child.attach(owner);
            var childParentData = ((TextParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
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
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            child.detach();
            var childParentData = ((TextParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        this._foregroundRenderObject?.detach();
        this._backgroundRenderObject?.detach();
    }

    public override void redepthChildren()
    {
        RenderObject? foregroundChild = this._foregroundRenderObject;
        RenderObject? backgroundChild = this._backgroundRenderObject;
        if ((foregroundChild is not null))
        {
            redepthChild(foregroundChild);
        }
        if ((backgroundChild is not null))
        {
            redepthChild(backgroundChild);
        }
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            redepthChild(child);
            var childParentData = ((TextParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderObject? foregroundChild = this._foregroundRenderObject;
        RenderObject? backgroundChild = this._backgroundRenderObject;
        if ((foregroundChild is not null))
        {
            visitor(foregroundChild);
        }
        if ((backgroundChild is not null))
        {
            visitor(backgroundChild);
        }
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            visitor(child);
            var childParentData = ((TextParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
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
        global::Doroti.Ui.Offset paintOffset = this._paintOffset;
        List<global::Doroti.Ui.TextBox> boxes = (selection.isCollapsed ? new List<global::Doroti.Ui.TextBox>() : this._textPainter.getBoxesForSelection(selection, boxHeightStyle: this.selectionHeightStyle, boxWidthStyle: this.selectionWidthStyle));
        if ((checked((long)(boxes.Count)) == 0))
        {
            global::Doroti.Ui.Offset caretOffset = this._textPainter.getOffsetForCaret(selection.extent, this._caretPrototype);
            global::Doroti.Ui.Offset startLocal = ((new global::Doroti.Ui.Offset(0.0, this.preferredLineHeight) + caretOffset) + paintOffset);
            return new List<TextSelectionPoint> { new TextSelectionPoint(startLocal, null) };
        }
        else
        {
            global::Doroti.Ui.Offset startAlternate = (new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble(boxes.First().start, 0, ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).size.width), boxes.First().bottom) + paintOffset);
            global::Doroti.Ui.Offset endLocal = (new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble(boxes.Last().end, 0, ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).size.width), boxes.Last().bottom) + paintOffset);
            return new List<TextSelectionPoint> { new TextSelectionPoint(startAlternate, boxes.First().direction), new TextSelectionPoint(endLocal, boxes.Last().direction) };
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
        List<global::Doroti.Ui.TextBox> boxes = this._textPainter.getBoxesForSelection(new TextSelection(baseOffset: range.start, extentOffset: range.end), boxHeightStyle: this.selectionHeightStyle, boxWidthStyle: this.selectionWidthStyle);
        return System.Linq.Enumerable.Aggregate(boxes, (Rect?)null, ((accum, incoming) => (accum?.expandToInclude(incoming.toRect()) ?? incoming.toRect())))?.shift(this._paintOffset);
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
        global::Doroti.Ui.Rect caretPrototype = this._caretPrototype;
        global::Doroti.Ui.Offset caretOffset = this._textPainter.getOffsetForCaret(caretPosition, caretPrototype);
        global::Doroti.Ui.Rect caretRect = caretPrototype.shift((caretOffset + this.cursorOffset));
        double scrollableWidth = Math.Max((((global::Doroti.Framework.Painting.TextPainter)this._textPainter).width + this._caretMargin), size.width);
        double caretX = Dart_uiLibrary.clampDouble(caretRect.left, 0, Math.Max((scrollableWidth - this._caretMargin), 0));
        caretRect = (new global::Doroti.Ui.Offset(caretX, caretRect.top) & caretRect.size);
        double fullHeight = this._textPainter.getFullHeightForCaret(caretPosition, caretPrototype);
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case var __constant67807 when object.Equals(__constant67807, TargetPlatform.iOS):
            case var __constant67838 when object.Equals(__constant67838, TargetPlatform.macOS):
                {
                    double heightDiff = (fullHeight - caretRect.height);
                    caretRect = global::Doroti.Ui.Rect.fromLTWH(caretRect.left, (caretRect.top + (heightDiff / 2L)), caretRect.width, caretRect.height);
                    break;
                }
            case var __constant68160 when object.Equals(__constant68160, TargetPlatform.android):
            case var __constant68195 when object.Equals(__constant68195, TargetPlatform.fuchsia):
            case var __constant68230 when object.Equals(__constant68230, TargetPlatform.linux):
            case var __constant68263 when object.Equals(__constant68263, TargetPlatform.windows):
                {
                    double caretHeight = this.cursorHeight;
                    double heightDiffLocal = (fullHeight - caretHeight);
                    caretRect = global::Doroti.Ui.Rect.fromLTWH(caretRect.left, ((caretRect.top - EditableLibrary._kCaretHeightOffset) + (heightDiffLocal / 2L)), caretRect.width, caretHeight);
                    break;
                }
        }
        caretRect = caretRect.shift(this._paintOffset);
        return caretRect.shift(_snapToPhysicalPixel(caretRect.topLeft));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        List<global::Doroti.Framework.Painting.PlaceholderDimensions> placeholderDimensions = layoutInlineChildren(double.PositiveInfinity, ((Func<RenderBox, BoxConstraints, Size>)((child, constraints) => new global::Doroti.Ui.Size(child.getMinIntrinsicWidth(double.PositiveInfinity), 0.0))), (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline);
        var (minWidthLocal, maxWidthLocal) = _adjustConstraints();
        return (((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textIntrinsics;
    __cascade.setPlaceholderDimensions(placeholderDimensions);
    __cascade.layout(minWidth: minWidthLocal, maxWidth: maxWidthLocal);
    return __cascade;
}))()).minIntrinsicWidth;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        List<global::Doroti.Framework.Painting.PlaceholderDimensions> placeholderDimensions = layoutInlineChildren(double.PositiveInfinity, ((Func<RenderBox, BoxConstraints, Size>)((child, constraints) => new global::Doroti.Ui.Size(child.getMaxIntrinsicWidth(double.PositiveInfinity), 0.0))), (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline);
        var (minWidthLocal, maxWidthLocal) = _adjustConstraints();
        return ((((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textIntrinsics;
    __cascade.setPlaceholderDimensions(placeholderDimensions);
    __cascade.layout(minWidth: minWidthLocal, maxWidth: maxWidthLocal);
    return __cascade;
}))()).maxIntrinsicWidth + this._caretMargin);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double preferredLineHeight => ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).preferredLineHeight;
    internal virtual long _countHardLineBreaks(string text)
    {
        long? cachedValue = this._cachedLineBreakCount;
        if ((cachedValue is not null))
        {
            long cachedValue__70677__value70722 = DartRuntimePrimitives.RequireValue(cachedValue);
            return DartRuntimePrimitives.RequireValue(cachedValue__70677__value70722);
        }
        var count = 0L;
        for (var index = 0L; (index < text.Length); index += 1L)
        {
            switch (text.codeUnitAt(index))
            {
                case 10L:
                case 133L:
                case 11L:
                case 12L:
                case 8232L:
                case 8233L:
                    {
                        count += 1L;
                        break;
                    }
            }
        }
        return DartRuntimePrimitives.RequireValue(_cachedLineBreakCount = count);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _preferredHeight(double width)
    {
        long? maxLinesLocal = this.maxLines;
        long? minLinesLocal = (this.minLines ?? maxLinesLocal);
        double minHeight = (this.preferredLineHeight * ((minLinesLocal ?? 0L)));
        DartRuntimePrimitives.Assert(() => ((maxLinesLocal != 1L) || (((global::Doroti.Framework.Painting.TextPainter)this._textIntrinsics).maxLines == 1L)));
        if ((maxLinesLocal is null))
        {
            double estimatedHeight = default!;
            if ((width == double.PositiveInfinity))
            {
                estimatedHeight = (this.preferredLineHeight * ((_countHardLineBreaks(this.plainText) + 1L)));
            }
            else
            {
                var (minWidthLocal, maxWidthLocal) = _adjustConstraints(maxWidth: width);
                estimatedHeight = (((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textIntrinsics;
    __cascade.layout(minWidth: minWidthLocal, maxWidth: maxWidthLocal);
    return __cascade;
}))()).height;
            }
            return Math.Max(estimatedHeight, minHeight);
        }
        if ((DartRuntimePrimitives.RequireValue(maxLinesLocal) == 1L))
        {
            var (minWidthAlternate, maxWidthAlternate) = _adjustConstraints(maxWidth: width);
            return (((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textIntrinsics;
    __cascade.layout(minWidth: minWidthAlternate, maxWidth: maxWidthAlternate);
    return __cascade;
}))()).height;
        }
        if ((minLinesLocal == DartRuntimePrimitives.RequireValue(maxLinesLocal)))
        {
            return minHeight;
        }
        double maxHeight = (this.preferredLineHeight * DartRuntimePrimitives.RequireValue(maxLinesLocal));
        var (minWidthNested, maxWidthNested) = _adjustConstraints(maxWidth: width);
        return Dart_uiLibrary.clampDouble((((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textIntrinsics;
    __cascade.layout(minWidth: minWidthNested, maxWidth: maxWidthNested);
    return __cascade;
}))()).height, minHeight, maxHeight);
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
        global::Doroti.Ui.Offset effectivePosition = (position - this._paintOffset);
        global::Doroti.Ui.GlyphInfo? glyph = this._textPainter.getClosestGlyphForOffset(effectivePosition);
        global::Doroti.Framework.Painting.InlineSpan? spanHit = (((glyph is not null) && glyph.graphemeClusterLayoutBounds.contains(effectivePosition)) ? ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).text!.getSpanForPosition(new global::Doroti.Ui.TextPosition(offset: glyph.graphemeClusterCodeUnitRange.start)) : null);
        switch (spanHit)
        {
            case HitTestTarget span:
                {
                    result.add(new HitTestEntry<HitTestTarget>(span));
                    return true;
                }
            default:
                {
                    return hitTestInlineChildren(result, effectivePosition);
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
        global::Doroti.Ui.TextPosition fromPosition = this._textPainter.getPositionForOffset((globalToLocal(from) - this._paintOffset));
        global::Doroti.Ui.TextPosition? toPosition = ((to is null) ? null : this._textPainter.getPositionForOffset((globalToLocal(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(to))) - this._paintOffset)));
        long baseOffsetLocal = fromPosition.offset;
        long extentOffsetLocal = (toPosition?.offset ?? fromPosition.offset);
        var newSelection = new TextSelection(baseOffset: baseOffsetLocal, extentOffset: extentOffsetLocal, affinity: fromPosition.affinity);
        _setSelection(newSelection, cause);
    }

    public virtual global::Doroti.Framework.Painting.WordBoundary wordBoundaries => ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).wordBoundaries;
    public virtual void selectWord(SelectionChangedCause cause)
    {
        selectWordsInRange(from: DartRuntimePrimitives.RequireValue(this._lastTapDownPosition), cause: cause);
    }

    public virtual void selectWordsInRange(Offset from, Offset? to = null, SelectionChangedCause cause = default!)
    {
        _computeTextMetricsIfNeeded();
        global::Doroti.Ui.TextPosition fromPosition = this._textPainter.getPositionForOffset((globalToLocal(from) - this._paintOffset));
        TextSelection fromWord = getWordAtOffset(fromPosition);
        global::Doroti.Ui.TextPosition toPosition = ((to is null) ? fromPosition : this._textPainter.getPositionForOffset((globalToLocal(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(to))) - this._paintOffset)));
        TextSelection toWord = ((object.Equals(toPosition, fromPosition)) ? fromWord : getWordAtOffset(toPosition));
        bool isFromWordBeforeToWord = (fromWord.start < toWord.end);
        _setSelection(new TextSelection(baseOffset: (isFromWordBeforeToWord ? fromWord.@base.offset : fromWord.extent.offset), extentOffset: (isFromWordBeforeToWord ? toWord.extent.offset : toWord.@base.offset), affinity: fromWord.affinity), cause);
    }

    public virtual void selectWordEdge(SelectionChangedCause cause)
    {
        _computeTextMetricsIfNeeded();
        DartRuntimePrimitives.Assert(() => (this._lastTapDownPosition is not null));
        global::Doroti.Ui.TextPosition position = this._textPainter.getPositionForOffset((globalToLocal(DartRuntimePrimitives.RequireValue(this._lastTapDownPosition)) - this._paintOffset));
        global::Doroti.Ui.TextRange word = this._textPainter.getWordBoundary(position);
        TextSelection newSelection = default!;
        if ((position.offset <= word.start))
        {
            newSelection = TextSelection.CreateCollapsed(offset: word.start);
        }
        else
        {
            newSelection = TextSelection.CreateCollapsed(offset: word.end, affinity: TextAffinity.upstream);
        }
        _setSelection(newSelection, cause);
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
        global::Doroti.Ui.TextRange word = this._textPainter.getWordBoundary(position);
        long effectiveOffset = default!;
        switch (position.affinity)
        {
            case TextAffinity.upstream:
                {
                    effectiveOffset = (position.offset - 1L);
                    break;
                }
            case TextAffinity.downstream:
                {
                    effectiveOffset = position.offset;
                    break;
                }
        }
        DartRuntimePrimitives.Assert(() => (effectiveOffset >= 0L));
        if (((effectiveOffset > 0L) && TextLayoutMetrics.isWhitespace(this.plainText.codeUnitAt(effectiveOffset))))
        {
            global::Doroti.Ui.TextRange? previousWord = _getPreviousWord(word.start);
            switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
            {
                case var __constant83254 when object.Equals(__constant83254, TargetPlatform.iOS):
                    {
                        if ((previousWord is null))
                        {
                            global::Doroti.Ui.TextRange? nextWord = _getNextWord(word.start);
                            if ((nextWord is null))
                            {
                                return TextSelection.CreateCollapsed(offset: position.offset);
                            }
                            return new TextSelection(baseOffset: position.offset, extentOffset: nextWord.end);
                        }
                        return new TextSelection(baseOffset: previousWord.start, extentOffset: position.offset);
                    }
                case var __constant83710 when object.Equals(__constant83710, TargetPlatform.android):
                    {
                        if (this.readOnly)
                        {
                            if ((previousWord is null))
                            {
                                return new TextSelection(baseOffset: position.offset, extentOffset: (position.offset + 1L));
                            }
                            return new TextSelection(baseOffset: previousWord.start, extentOffset: position.offset);
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
        return new TextSelection(baseOffset: word.start, extentOffset: word.end);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (double, double) _adjustConstraints(double minWidth = 0.0, double maxWidth = double.PositiveInfinity)
    {
        double availableMaxWidth = Math.Max(0.0, (maxWidth - this._caretMargin));
        double availableMinWidth = Math.Min(minWidth, availableMaxWidth);
        return ((this.forceLine ? availableMaxWidth : availableMinWidth), (this._isMultiline ? availableMaxWidth : double.PositiveInfinity));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _computeTextMetricsIfNeeded()
    {
        var (minWidthLocal, maxWidthLocal) = _adjustConstraints(minWidth: ((BoxConstraints)constraints).minWidth, maxWidth: ((BoxConstraints)constraints).maxWidth);
        this._textPainter.layout(minWidth: minWidthLocal, maxWidth: maxWidthLocal);
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
        global::Doroti.Ui.Offset globalOffset = localToGlobal(sourceOffset);
        double pixelMultiple = (1.0 / this._devicePixelRatio);
        return new global::Doroti.Ui.Offset((double.IsFinite(globalOffset.dx) ? ((((globalOffset.dx / pixelMultiple)).round() * pixelMultiple) - globalOffset.dx) : 0), (double.IsFinite(globalOffset.dy) ? ((((globalOffset.dy / pixelMultiple)).round() * pixelMultiple) - globalOffset.dy) : 0));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        var (minWidthLocal, maxWidthLocal) = _adjustConstraints(minWidth: ((BoxConstraints)constraints).minWidth, maxWidth: ((BoxConstraints)constraints).maxWidth);
        ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textIntrinsics;
    __cascade.setPlaceholderDimensions(layoutInlineChildren(((BoxConstraints)constraints).maxWidth, (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.dryLayoutChild, (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline));
    __cascade.layout(minWidth: minWidthLocal, maxWidth: maxWidthLocal);
    return __cascade;
}))();
        double widthLocal = (this.forceLine ? ((BoxConstraints)constraints).maxWidth : constraints.constrainWidth((((global::Doroti.Framework.Painting.TextPainter)this._textIntrinsics).size.width + this._caretMargin)));
        return new global::Doroti.Ui.Size(widthLocal, constraints.constrainHeight(_preferredHeight(((BoxConstraints)constraints).maxWidth)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        var (minWidthLocal, maxWidthLocal) = _adjustConstraints(minWidth: ((BoxConstraints)constraints).minWidth, maxWidth: ((BoxConstraints)constraints).maxWidth);
        ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textIntrinsics;
    __cascade.setPlaceholderDimensions(layoutInlineChildren(((BoxConstraints)constraints).maxWidth, (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.dryLayoutChild, (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline));
    __cascade.layout(minWidth: minWidthLocal, maxWidth: maxWidthLocal);
    return __cascade;
}))();
        return this._textIntrinsics.computeDistanceToActualBaseline(baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraintsLocal = this.constraints;
        _placeholderDimensions = layoutInlineChildren(((BoxConstraints)constraintsLocal).maxWidth, (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.layoutChild, (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getBaseline);
        var (minWidthLocal, maxWidthLocal) = _adjustConstraints(minWidth: ((BoxConstraints)constraintsLocal).minWidth, maxWidth: ((BoxConstraints)constraintsLocal).maxWidth);
        ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textPainter;
    __cascade.setPlaceholderDimensions(this._placeholderDimensions);
    __cascade.layout(minWidth: minWidthLocal, maxWidth: maxWidthLocal);
    return __cascade;
}))();
        positionInlineChildren(((global::Doroti.Framework.Painting.TextPainter)this._textPainter).inlinePlaceholderBoxes!);
        _computeCaretPrototype();
        double widthLocal = (this.forceLine ? ((BoxConstraints)constraintsLocal).maxWidth : constraintsLocal.constrainWidth((((global::Doroti.Framework.Painting.TextPainter)this._textPainter).width + this._caretMargin)));
        DartRuntimePrimitives.Assert(() => ((this.maxLines != 1L) || (((global::Doroti.Framework.Painting.TextPainter)this._textPainter).maxLines == 1L)));
        double preferredHeight = (this.maxLines switch { null => Math.Max(((global::Doroti.Framework.Painting.TextPainter)this._textPainter).height, (this.preferredLineHeight * ((this.minLines ?? 0L)))), 1L => ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).height, long maxLinesLocal => Dart_uiLibrary.clampDouble(((global::Doroti.Framework.Painting.TextPainter)this._textPainter).height, (this.preferredLineHeight * ((this.minLines ?? DartRuntimePrimitives.RequireValue(maxLinesLocal)))), (this.preferredLineHeight * DartRuntimePrimitives.RequireValue(maxLinesLocal))) });
        size = new global::Doroti.Ui.Size(widthLocal, constraintsLocal.constrainHeight(preferredHeight));
        var contentSize = new global::Doroti.Ui.Size((((global::Doroti.Framework.Painting.TextPainter)this._textPainter).width + this._caretMargin), ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).height);
        var painterConstraints = BoxConstraints.CreateTight(contentSize);
        this._foregroundRenderObject?.layout(painterConstraints);
        this._backgroundRenderObject?.layout(painterConstraints);
        _maxScrollExtent = _getMaxScrollExtent(contentSize);
        this.offset.applyViewportDimension(this._viewportExtent);
        this.offset.applyContentDimensions(0.0, this._maxScrollExtent);
    }

    internal static global::Doroti.Ui.Offset _calculateAdjustedCursorOffset(Offset offset, Rect boundingRects)
    {
        double adjustedX = Dart_uiLibrary.clampDouble(offset.dx, boundingRects.left, boundingRects.right);
        double adjustedY = Dart_uiLibrary.clampDouble(offset.dy, boundingRects.top, boundingRects.bottom);
        return new global::Doroti.Ui.Offset(adjustedX, adjustedY);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Offset calculateBoundedFloatingCursorOffset(Offset rawCursorOffset, bool? shouldResetOrigin = null)
    {
        global::Doroti.Ui.Offset deltaPosition = Offset.zero;
        double topBound = -((global::Doroti.Framework.Painting.EdgeInsets)this.floatingCursorAddedMargin).top;
        double bottomBound = ((Math.Min(size.height, ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).height) - this.preferredLineHeight) + ((global::Doroti.Framework.Painting.EdgeInsets)this.floatingCursorAddedMargin).bottom);
        double leftBound = -((global::Doroti.Framework.Painting.EdgeInsets)this.floatingCursorAddedMargin).left;
        double rightBound = (Math.Min(size.width, ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).width) + ((global::Doroti.Framework.Painting.EdgeInsets)this.floatingCursorAddedMargin).right);
        var boundingRects = global::Doroti.Ui.Rect.fromLTRB(leftBound, topBound, rightBound, bottomBound);
        if ((shouldResetOrigin is not null))
        {
            bool shouldResetOrigin__value92495 = DartRuntimePrimitives.RequireValue(shouldResetOrigin);
            _shouldResetOrigin = DartRuntimePrimitives.RequireValue(shouldResetOrigin__value92495);
        }
        if (!this._shouldResetOrigin)
        {
            return _calculateAdjustedCursorOffset(rawCursorOffset, boundingRects);
        }
        if ((this._previousOffset is not null))
        {
            deltaPosition = (rawCursorOffset - DartRuntimePrimitives.RequireValue(this._previousOffset));
        }
        if ((this._resetOriginOnLeft && (deltaPosition.dx > 0L)))
        {
            _relativeOrigin = new global::Doroti.Ui.Offset((rawCursorOffset.dx - boundingRects.left), this._relativeOrigin.dy);
            _resetOriginOnLeft = false;
        }
        else
        {
            if ((this._resetOriginOnRight && (deltaPosition.dx < 0L)))
            {
                _relativeOrigin = new global::Doroti.Ui.Offset((rawCursorOffset.dx - boundingRects.right), this._relativeOrigin.dy);
                _resetOriginOnRight = false;
            }
        }
        if ((this._resetOriginOnTop && (deltaPosition.dy > 0L)))
        {
            _relativeOrigin = new global::Doroti.Ui.Offset(this._relativeOrigin.dx, (rawCursorOffset.dy - boundingRects.top));
            _resetOriginOnTop = false;
        }
        else
        {
            if ((this._resetOriginOnBottom && (deltaPosition.dy < 0L)))
            {
                _relativeOrigin = new global::Doroti.Ui.Offset(this._relativeOrigin.dx, (rawCursorOffset.dy - boundingRects.bottom));
                _resetOriginOnBottom = false;
            }
        }
        double currentX = (rawCursorOffset.dx - this._relativeOrigin.dx);
        double currentY = (rawCursorOffset.dy - this._relativeOrigin.dy);
        global::Doroti.Ui.Offset adjustedOffset = _calculateAdjustedCursorOffset(new global::Doroti.Ui.Offset(currentX, currentY), boundingRects);
        if (((currentX < boundingRects.left) && (deltaPosition.dx < 0L)))
        {
            _resetOriginOnLeft = true;
        }
        else
        {
            if (((currentX > boundingRects.right) && (deltaPosition.dx > 0L)))
            {
                _resetOriginOnRight = true;
            }
        }
        if (((currentY < boundingRects.top) && (deltaPosition.dy < 0L)))
        {
            _resetOriginOnTop = true;
        }
        else
        {
            if (((currentY > boundingRects.bottom) && (deltaPosition.dy > 0L)))
            {
                _resetOriginOnBottom = true;
            }
        }
        _previousOffset = rawCursorOffset;
        return adjustedOffset;
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
            double? animationValue = this._resetFloatingCursorAnimationValue;
            global::Doroti.Framework.Painting.EdgeInsets sizeAdjustment = ((animationValue is not null) ? EdgeInsets.lerp(EditableLibrary._kFloatingCursorSizeIncrease, global::Doroti.Framework.Painting.EdgeInsets.zero, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(animationValue)))! : EditableLibrary._kFloatingCursorSizeIncrease);
            this._caretPainter.floatingCursorRect = sizeAdjustment.inflateRect(this._caretPrototype).shift(boundedOffset);
        }
        else
        {
            this._caretPainter.floatingCursorRect = null;
        }
        this._caretPainter.showRegularCaret = (this._resetFloatingCursorAnimationValue is null);
    }

    internal virtual MapEntry<long, global::Doroti.Ui.Offset> _lineNumberFor(TextPosition startPosition, List<LineMetrics> metrics)
    {
        global::Doroti.Ui.Offset offsetLocal = this._textPainter.getOffsetForCaret(startPosition, Rect.zero);
        foreach (var lineMetrics in metrics)
        {
            if ((lineMetrics.baseline > offsetLocal.dy))
            {
                return new MapEntry<long, global::Doroti.Ui.Offset>(lineMetrics.lineNumber, new global::Doroti.Ui.Offset(offsetLocal.dx, lineMetrics.baseline));
            }
        }
        DartRuntimePrimitives.Assert(() => (startPosition.offset == 0L));
        return new MapEntry<long, global::Doroti.Ui.Offset>(Math.Max(0L, (checked((long)(metrics.Count)) - 1L)), new global::Doroti.Ui.Offset(offsetLocal.dx, ((checked((long)(metrics.Count)) != 0) ? (metrics.Last().baseline + metrics.Last().descent) : 0.0)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual VerticalCaretMovementRun startVerticalCaretMovement(TextPosition startPosition)
    {
        List<global::Doroti.Ui.LineMetrics> metrics = this._textPainter.computeLineMetrics();
        MapEntry<long, global::Doroti.Ui.Offset> currentLine = _lineNumberFor(startPosition, metrics);
        return new VerticalCaretMovementRun(this, metrics, startPosition, currentLine.key, currentLine.value);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paintContents(PaintingContext context, Offset offset)
    {
        global::Doroti.Ui.Offset effectiveOffset = (offset + this._paintOffset);
        if (((this.selection is not null) && !this._floatingCursorOn))
        {
            _updateSelectionExtentsVisibility(effectiveOffset);
        }
        RenderBox? foregroundChild = this._foregroundRenderObject;
        RenderBox? backgroundChild = this._backgroundRenderObject;
        if ((backgroundChild is not null))
        {
            context.paintChild(backgroundChild, offset);
        }
        this._textPainter.paint(((PaintingContext)context).canvas, effectiveOffset);
        paintInlineChildren(context, effectiveOffset);
        if ((foregroundChild is not null))
        {
            context.paintChild(foregroundChild, offset);
        }
    }

    internal virtual void _paintHandleLayers(PaintingContext context, List<TextSelectionPoint> endpoints, Offset offset)
    {
        global::Doroti.Ui.Offset startPoint = endpoints[(int)(0L)].point;
        startPoint = new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble(startPoint.dx, 0.0, size.width), Dart_uiLibrary.clampDouble(startPoint.dy, 0.0, size.height));
        this._leaderLayerHandler.layer = new LeaderLayer(link: this.startHandleLayerLink, offset: (startPoint + offset));
        context.pushLayer(((LayerHandle<LeaderLayer>)this._leaderLayerHandler).layer!, (Action<PaintingContext, Offset>)base.paint, Offset.zero);
        if ((checked((long)(endpoints.Count)) == 2L))
        {
            global::Doroti.Ui.Offset endPoint = endpoints[(int)(1L)].point;
            endPoint = new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble(endPoint.dx, 0.0, size.width), Dart_uiLibrary.clampDouble(endPoint.dy, 0.0, size.height));
            context.pushLayer(new LeaderLayer(link: this.endHandleLayerLink, offset: (endPoint + offset)), (Action<PaintingContext, Offset>)base.paint, Offset.zero);
        }
        else
        {
            if (this.selection!.isCollapsed)
            {
                context.pushLayer(new LeaderLayer(link: this.endHandleLayerLink, offset: (startPoint + offset)), (Action<PaintingContext, Offset>)base.paint, Offset.zero);
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
        TextSelection? selectionLocal = this.selection;
        if (((selectionLocal is not null) && selectionLocal.isValid))
        {
            _paintHandleLayers(context, getEndpointsForSelection(selectionLocal), offset);
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
        var childParentData = ((TextParentData?)(object?)child.parentData!)!;
        while ((childParentData.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.previousSibling, child)));
            child = childParentData.previousSibling!;
            childParentData = ((TextParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((TextParentData?)(object?)child.parentData!)!;
        while ((childParentData.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.nextSibling, child)));
            child = childParentData.nextSibling!;
            childParentData = ((TextParentData?)(object?)child.parentData!)!;
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
        var childParentData = ((TextParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData = ((TextParentData?)(object?)this._firstChild!.parentData!)!;
                firstChildParentData.previousSibling = child;
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
            var afterParentData = ((TextParentData?)(object?)after.parentData!)!;
            if ((afterParentData.nextSibling is null))
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(after, this._lastChild)));
                childParentData.previousSibling = after;
                afterParentData.nextSibling = child;
                this._lastChild = child;
            }
            else
            {
                childParentData.nextSibling = afterParentData.nextSibling;
                childParentData.previousSibling = after;
                var childPreviousSiblingParentData = ((TextParentData?)(object?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((TextParentData?)(object?)childParentData.nextSibling!.parentData!)!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData.nextSibling, child)));
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
        var childParentData = ((TextParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: this._firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: this._lastChild));
        DartRuntimePrimitives.Assert(() => (this._childCount >= 0L));
        if ((childParentData.previousSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._firstChild, child)));
            this._firstChild = childParentData.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData = ((TextParentData?)(object?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if ((childParentData.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((TextParentData?)(object?)childParentData.nextSibling!.parentData!)!;
            childNextSiblingParentData.previousSibling = childParentData.previousSibling;
        }
        childParentData.previousSibling = null;
        childParentData.nextSibling = null;
        this._childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            var childParentData = ((TextParentData?)(object?)child.parentData!)!;
            RenderBox? next = childParentData.nextSibling;
            childParentData.previousSibling = null;
            childParentData.nextSibling = null;
            dropChild(child);
            child = next;
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
        var childParentData = ((TextParentData?)(object?)child.parentData!)!;
        if ((object.Equals(childParentData.previousSibling, after)))
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
        var childParentData = ((TextParentData?)(object?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((TextParentData?)(object?)child.parentData!)!;
        return childParentData.nextSibling;
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
        var constraints = new BoxConstraints(maxWidth: maxWidth);
        return new List<global::Doroti.Framework.Painting.PlaceholderDimensions>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void positionInlineChildren(List<TextBox> boxes)
    {
        RenderBox? child = firstChild;
        foreach (var box in boxes)
        {
            if ((child is null))
            {
                DartRuntimePrimitives.Assert(() =>
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Invalid number of boxes provided to positionInlineChildren."), new ErrorDescription($"The number of boxes ({checked((long)(boxes.Count))}) exceeds the number of child render objects ({childCount}). " + "Each box corresponds to a child, but there are not enough children to position all boxes."), new ErrorHint("This error typically occurs when a custom InlineSpan implementation returns a list of boxes " + "that is longer than the number of inline children. Ensure that the number of boxes returned " + "by `computeLineMetrics` or similar methods does not exceed the number of children."), new DiagnosticsProperty<RenderObject>("The RenderParagraph receiving the boxes", this, style: DiagnosticsTreeStyle.errorProperty) });
                    });
                return;
            }
            var textParentData = ((TextParentData?)(object?)child.parentData!)!;
            textParentData._offset = new global::Doroti.Ui.Offset(box.left, box.top);
            child = childAfter(child);
        }
        while ((child is not null))
        {
            var textParentDataLocal = ((TextParentData?)(object?)child.parentData!)!;
            textParentDataLocal._offset = null;
            child = childAfter(child);
        }
    }

    public virtual void defaultApplyPaintTransform(RenderBox child, Matrix4 transform)
    {
        var childParentData = ((TextParentData?)(object?)child.parentData!)!;
        global::Doroti.Ui.Offset? offsetLocal = ((TextParentData)childParentData).offset;
        if ((offsetLocal is null))
        {
            transform.setZero();
        }
        else
        {
            transform.translateByDouble(DartRuntimePrimitives.RequireValue(offsetLocal).dx, DartRuntimePrimitives.RequireValue(offsetLocal).dy, 0, 1);
        }
    }

    public virtual void paintInlineChildren(PaintingContext context, Offset offset)
    {
        RenderBox? child = firstChild;
        while ((child is not null))
        {
            var childParentData = ((TextParentData?)(object?)child.parentData!)!;
            global::Doroti.Ui.Offset? childOffset = ((TextParentData)childParentData).offset;
            if ((childOffset is null))
            {
                return;
            }
            context.paintChild(child, (DartRuntimePrimitives.RequireValue(childOffset) + offset));
            child = childAfter(child);
        }
    }

    public virtual bool hitTestInlineChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child = firstChild;
        while ((child is not null))
        {
            var childParentData = ((TextParentData?)(object?)child.parentData!)!;
            global::Doroti.Ui.Offset? childOffset = ((TextParentData)childParentData).offset;
            if ((childOffset is null))
            {
                return false;
            }
            bool isHit = result.addWithPaintOffset(offset: DartRuntimePrimitives.RequireValue(childOffset), position: position, hitTest: ((Func<BoxHitTestResult, Offset, bool>)((result, transformed) => child!.hitTest(result, position: transformed))));
            if (isHit)
            {
                return true;
            }
            child = childAfter(child);
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
            RenderEditablePainter? oldPainter = this.painter;
            _painter = newValue;
            if ((newValue?.shouldRepaint(oldPainter) ?? true))
            {
                markNeedsPaint();
            }
            if (attached)
            {
                oldPainter?.removeListener(markNeedsPaint);
                newValue?.addListener(markNeedsPaint);
            }
        }
    }
    public override void paint(PaintingContext context, Offset offset)
    {
        RenderEditable? parentLocal = this.parent;
        DartRuntimePrimitives.Assert(() => (parentLocal is not null));
        RenderEditablePainter? painterLocal = this.painter;
        if (((painterLocal is not null) && (parentLocal is not null)))
        {
            parentLocal._computeTextMetricsIfNeeded();
            painterLocal.paint(((PaintingContext)context).canvas, size, parentLocal);
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
        global::Doroti.Ui.TextRange? range = this.highlightedRange;
        global::Doroti.Ui.Color? colorLocal = this.highlightColor;
        if ((((range is null) || (colorLocal is null)) || range.isCollapsed))
        {
            return;
        }
        this.highlightPaint.color = colorLocal;
        global::Doroti.Framework.Painting.TextPainter textPainter = ((RenderEditable)renderEditable)._textPainter;
        HashSet<global::Doroti.Ui.TextBox> boxes = textPainter.getBoxesForSelection(new TextSelection(baseOffset: range.start, extentOffset: range.end), boxHeightStyle: this.selectionHeightStyle, boxWidthStyle: this.selectionWidthStyle).toSet();
        foreach (var box in boxes)
        {
            canvas.drawRect(box.toRect().shift(((RenderEditable)renderEditable)._paintOffset).intersect(global::Doroti.Ui.Rect.fromLTWH(0, 0, ((global::Doroti.Framework.Painting.TextPainter)textPainter).width, ((global::Doroti.Framework.Painting.TextPainter)textPainter).height)), this.highlightPaint);
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
        global::Doroti.Ui.Rect integralRect = renderEditable.getLocalRectForCaret(textPosition);
        if (this.shouldPaint)
        {
            if ((this.floatingCursorRect is not null))
            {
                double distanceSquaredLocal = ((DartRuntimePrimitives.RequireValue(this.floatingCursorRect).center - integralRect.center)).distanceSquared;
                if ((distanceSquaredLocal < EditableLibrary._kShortestDistanceSquaredWithFloatingAndRegularCursors))
                {
                    return;
                }
            }
            global::Doroti.Ui.Radius? radius = this.cursorRadius;
            this.caretPaint.color = caretColor;
            if ((radius is null))
            {
                canvas.drawRect(integralRect, this.caretPaint);
            }
            else
            {
                var caretRRect = global::Doroti.Ui.RRect.fromRectAndRadius(integralRect, DartRuntimePrimitives.RequireValue(radius));
                canvas.drawRRect(caretRRect, this.caretPaint);
            }
        }
    }

    public override void paint(Canvas canvas, Size size, RenderEditable renderEditable)
    {
        TextSelection? selectionLocal = ((RenderEditable)renderEditable).selection;
        if ((((selectionLocal is null) || !selectionLocal.isCollapsed) || !selectionLocal.isValid))
        {
            return;
        }
        global::Doroti.Ui.Rect? floatingCursorRectLocal = this.floatingCursorRect;
        global::Doroti.Ui.Color? caretColorLocal = ((floatingCursorRectLocal is null) ? this.caretColor : (this.showRegularCaret ? this.backgroundCursorColor : null));
        global::Doroti.Ui.TextPosition caretTextPosition = ((floatingCursorRectLocal is null) ? selectionLocal.extent : ((RenderEditable)renderEditable)._floatingCursorTextPosition);
        if ((caretColorLocal is not null))
        {
            paintRegularCursor(canvas, renderEditable, caretColorLocal, caretTextPosition);
        }
        global::Doroti.Ui.Color? floatingCursorColor = this.caretColor?.withOpacity(0.75);
        if ((((floatingCursorRectLocal is null) || (floatingCursorColor is null)) || !this.shouldPaint))
        {
            return;
        }
        canvas.drawRRect(global::Doroti.Ui.RRect.fromRectAndRadius(DartRuntimePrimitives.RequireValue(floatingCursorRectLocal), EditableLibrary._kFloatingCursorRadius), ((Func<Paint>)(() =>
{
    var __cascade = this.floatingCursorPaint;
    __cascade.color = floatingCursorColor;
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

    public override void addListener(Action listener)
    {
        foreach (RenderEditablePainter painter in this.painters)
        {
            painter.addListener(listener);
        }
    }

    public override void removeListener(Action listener)
    {
        foreach (RenderEditablePainter painter in this.painters)
        {
            painter.removeListener(listener);
        }
    }

    public override void paint(Canvas canvas, Size size, RenderEditable renderEditable)
    {
        foreach (RenderEditablePainter painter in this.painters)
        {
            painter.paint(canvas, size, renderEditable);
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
        IEnumerator<RenderEditablePainter> oldPainters = ((_CompositeRenderEditablePainter__editable)((_CompositeRenderEditablePainter__editable)oldDelegate)).painters.GetEnumerator();
        IEnumerator<RenderEditablePainter> newPainters = this.painters.GetEnumerator();
        while ((oldPainters.MoveNext() && newPainters.MoveNext()))
        {
            if (newPainters.Current.shouldRepaint(oldPainters.Current))
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
