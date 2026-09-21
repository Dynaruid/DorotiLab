// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/editable.dart
using Doroti.Runtime;
using Doroti.Ui;

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
    internal static EdgeInsets _kFloatingCursorSizeIncrease = EdgeInsets.CreateSymmetric(
        horizontal: 0.5,
        vertical: 1.0
    );
}

public static partial class EditableLibrary
{
    internal static Radius _kFloatingCursorRadius = Radius.circular(1.0);
}

public static partial class EditableLibrary
{
    internal static double _kShortestDistanceSquaredWithFloatingAndRegularCursors = 15.0 * 15.0;
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
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is TextSelectionPoint)
            && Equals(__other.point, point)
            && Equals(__other.direction, direction);
    }

    public override string ToString()
    {
        return direction switch
        {
            TextDirection.ltr => $"{point}-ltr",
            TextDirection.rtl => $"{point}-rtl",
            null => $"{point}",
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(point, direction);
}

public class VerticalCaretMovementRun : IEnumerator<TextPosition>
{
    internal virtual Offset _currentOffset { get; set; } = default!;
    internal virtual long _currentLine { get; set; } = default!;
    internal virtual TextPosition _currentTextPosition { get; set; } = default!;
    internal virtual List<LineMetrics> _lineMetrics { get; private set; } = default!;
    internal virtual RenderEditable _editable { get; private set; } = default!;
    internal virtual bool _isValid { get; set; } = true;
    internal virtual DartMap<long, MapEntry<Offset, TextPosition>> _positionCache
    {
        get;
        private set;
    } = new DartMap<long, MapEntry<Offset, TextPosition>>();

    public VerticalCaretMovementRun(
        RenderEditable _editable,
        List<LineMetrics> _lineMetrics,
        TextPosition _currentTextPosition,
        long _currentLine,
        Offset _currentOffset
    )
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
            if (!_isValid)
            {
                return false;
            }
            List<LineMetrics> newLineMetrics = _editable._textPainter.computeLineMetrics();
            if (!DartRuntimePrimitives.Identical(newLineMetrics, _lineMetrics))
            {
                _isValid = false;
            }
            return _isValid;
        }
    }

    internal virtual MapEntry<Offset, TextPosition> _getTextPositionForLine(long lineNumber)
    {
        DartRuntimePrimitives.Assert(() => isValid);
        DartRuntimePrimitives.Assert(() => lineNumber >= 0L);
        MapEntry<Offset, TextPosition>? cachedPosition = _positionCache.GetValueOrDefault(
            lineNumber
        );
        if (cachedPosition is not null)
        {
            MapEntry<Offset, TextPosition> cachedPosition__6901__value6954 = (
                cachedPosition
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            return (cachedPosition__6901__value6954);
        }
        DartRuntimePrimitives.Assert(() => lineNumber != _currentLine);
        var newOffset = new Offset(_currentOffset.dx, _lineMetrics[(int)lineNumber].baseline);
        TextPosition closestPosition = _editable._textPainter.getPositionForOffset(newOffset);
        var position = new MapEntry<Offset, TextPosition>(newOffset, closestPosition);
        _positionCache[lineNumber] = position;
        return position;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual TextPosition current
    {
        get
        {
            DartRuntimePrimitives.Assert(() => isValid);
            return _currentTextPosition;
        }
    }

    public virtual bool moveNext()
    {
        DartRuntimePrimitives.Assert(() => isValid);
        if ((_currentLine + 1L) >= checked(_lineMetrics.Count))
        {
            return false;
        }
        MapEntry<Offset, TextPosition> position = _getTextPositionForLine(_currentLine + 1L);
        _currentLine += 1L;
        _currentOffset = position.key;
        _currentTextPosition = position.value;
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool movePrevious()
    {
        DartRuntimePrimitives.Assert(() => isValid);
        if (_currentLine <= 0L)
        {
            return false;
        }
        MapEntry<Offset, TextPosition> position = _getTextPositionForLine(_currentLine - 1L);
        _currentLine -= 1L;
        _currentOffset = position.key;
        _currentTextPosition = position.value;
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool moveByOffset(double offset)
    {
        Offset initialOffset = _currentOffset;
        if (offset >= 0.0)
        {
            while (_currentOffset.dy < (initialOffset.dy + offset))
            {
                if (!moveNext())
                {
                    break;
                }
            }
        }
        else
        {
            while (_currentOffset.dy > (initialOffset.dy + offset))
            {
                if (!movePrevious())
                {
                    break;
                }
            }
        }
        return !Equals(initialOffset, _currentOffset);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    TextPosition IEnumerator<TextPosition>.Current => current;
    object System.Collections.IEnumerator.Current => current!;

    bool System.Collections.IEnumerator.MoveNext() => moveNext();

    void System.Collections.IEnumerator.Reset() => throw new NotSupportedException();

    void IDisposable.Dispose() { }
}

public class RenderEditable
    : RenderBox,
        RelayoutWhenSystemFontsChangeMixin,
        ContainerRenderObjectMixin<RenderBox, TextParentData>,
        RenderInlineChildrenContainerDefaults,
        TextLayoutMetrics
{
    internal virtual _RenderEditableCustomPaint__editable? _foregroundRenderObject { get; set; } =
        default;
    internal virtual _RenderEditableCustomPaint__editable? _backgroundRenderObject { get; set; } =
        default;
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
    internal virtual _TextHighlightPainter__editable _selectionPainter { get; private set; } =
        new _TextHighlightPainter__editable();
    internal virtual _TextHighlightPainter__editable _autocorrectHighlightPainter
    {
        get;
        private set;
    } = new _TextHighlightPainter__editable();
    internal virtual _CompositeRenderEditablePainter__editable? _cachedBuiltInForegroundPainters { get; set; } =
        default;
    internal virtual _CompositeRenderEditablePainter__editable? _cachedBuiltInPainters { get; set; } =
        default;
    public virtual bool ignorePointer { get; set; } = default!;
    internal virtual double _devicePixelRatio { get; set; } = default!;
    internal virtual string _obscuringCharacter { get; set; } = default!;
    internal virtual bool _obscureText { get; set; } = default!;
    public virtual TextSelectionDelegate textSelectionDelegate { get; set; } = default!;
    internal virtual ValueNotifier<bool> _selectionStartInViewport { get; private set; } =
        new ValueNotifier<bool>(true);
    internal virtual ValueNotifier<bool> _selectionEndInViewport { get; private set; } =
        new ValueNotifier<bool>(true);
    internal virtual TextPainter _textPainter { get; private set; } = default!;
    internal virtual AttributedString? _cachedAttributedValue { get; set; } = default;
    internal virtual List<InlineSpanSemanticsInformation>? _cachedCombinedSemanticsInfos { get; set; } =
        default;
    internal virtual TextPainter? _textIntrinsicsCache { get; set; } = default;
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
    public virtual EdgeInsets floatingCursorAddedMargin { get; set; } = default!;
    internal virtual bool _floatingCursorOn { get; set; } = false;
    internal virtual TextPosition _floatingCursorTextPosition { get; set; } = default!;
    internal virtual bool? _enableInteractiveSelection { get; set; } = default;
    internal virtual double _maxScrollExtent { get; set; } = 0;
    internal virtual Clip _clipBehavior { get; set; } = Clip.hardEdge;
    internal virtual List<InlineSpanSemanticsInformation>? _semanticsInfo { get; set; } = default;
    internal virtual DartMap<Key, SemanticsNode>? _cachedChildNodes { get; set; } = default;
    internal virtual long? _cachedLineBreakCount { get; set; } = default;
    internal virtual TapGestureRecognizer _tap { get; set; } = default!;
    internal virtual LongPressGestureRecognizer _longPress { get; set; } = default!;
    internal virtual Offset? _lastTapDownPosition { get; set; } = default;
    internal virtual Offset? _lastSecondaryTapDownPosition { get; set; } = default;
    internal virtual List<PlaceholderDimensions>? _placeholderDimensions { get; set; } = default;
    internal virtual Rect _caretPrototype { get; set; } = default!;
    internal virtual Offset _relativeOrigin { get; set; } = Offset.zero;
    internal virtual Offset? _previousOffset { get; set; } = default;
    internal virtual bool _shouldResetOrigin { get; set; } = true;
    internal virtual bool _resetOriginOnLeft { get; set; } = false;
    internal virtual bool _resetOriginOnRight { get; set; } = false;
    internal virtual bool _resetOriginOnTop { get; set; } = false;
    internal virtual bool _resetOriginOnBottom { get; set; } = false;
    internal virtual double? _resetFloatingCursorAnimationValue { get; set; } = default;
    internal virtual LayerHandle<LeaderLayer> _leaderLayerHandler { get; private set; } =
        new LayerHandle<LeaderLayer>();
    internal virtual LayerHandle<ClipRectLayer> _clipRectLayer { get; private set; } =
        new LayerHandle<ClipRectLayer>();
    public virtual bool _hasPendingSystemFontsDidChangeCallBack { get; set; } = false;
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    public RenderEditable(
        InlineSpan? text = null,
        TextDirection textDirection = default!,
        TextAlign textAlign = TextAlign.start,
        Color? cursorColor = null,
        Color? backgroundCursorColor = null,
        ValueNotifier<bool>? showCursor = null,
        bool? hasFocus = null,
        LayerLink startHandleLayerLink = default!,
        LayerLink endHandleLayerLink = default!,
        long? maxLines = 1,
        long? minLines = null,
        bool expands = false,
        Painting.StrutStyle? strutStyle = null,
        Color? selectionColor = null,
        double textScaleFactor = 1.0,
        TextScaler textScaler = default!,
        TextSelection? selection = null,
        ViewportOffset offset = default!,
        bool ignorePointer = false,
        bool readOnly = false,
        bool forceLine = true,
        TextHeightBehavior? textHeightBehavior = null,
        TextWidthBasis textWidthBasis = TextWidthBasis.parent,
        string obscuringCharacter = "•",
        bool obscureText = false,
        Locale? locale = null,
        double cursorWidth = 1.0,
        double? cursorHeight = null,
        Radius? cursorRadius = null,
        bool paintCursorAboveText = false,
        Offset cursorOffset = default,
        double devicePixelRatio = 1.0,
        BoxHeightStyle selectionHeightStyle = BoxHeightStyle.max,
        BoxWidthStyle selectionWidthStyle = BoxWidthStyle.max,
        bool? enableInteractiveSelection = null,
        EdgeInsets floatingCursorAddedMargin = default!,
        TextRange? promptRectRange = null,
        Color? promptRectColor = null,
        Clip clipBehavior = Clip.hardEdge,
        TextSelectionDelegate textSelectionDelegate = default!,
        RenderEditablePainter? painter = null,
        RenderEditablePainter? foregroundPainter = null,
        List<RenderBox>? children = null
    )
    {
        TextScaler __textScaler = textScaler ?? TextScaler.noScaling;
        EdgeInsets __floatingCursorAddedMargin =
            floatingCursorAddedMargin ?? new EdgeInsets(4, 4, 4, 5);
        this.ignorePointer = ignorePointer;
        this.floatingCursorAddedMargin = __floatingCursorAddedMargin;
        this.textSelectionDelegate = textSelectionDelegate;
        _textPainter = new TextPainter(
            text: text,
            textAlign: textAlign,
            textDirection: textDirection,
            textScaler: Equals(textScaler, TextScaler.noScaling)
                ? TextScaler.CreateLinear(textScaleFactor)
                : textScaler,
            locale: locale,
            maxLines: (maxLines == 1L) ? 1L : null,
            strutStyle: strutStyle,
            textHeightBehavior: textHeightBehavior,
            textWidthBasis: textWidthBasis
        );
        _showCursor = showCursor ?? new ValueNotifier<bool>(false);
        _maxLines = maxLines;
        _minLines = minLines;
        _expands = expands;
        _selection = selection;
        _offset = offset;
        _cursorWidth = cursorWidth;
        _cursorHeight = cursorHeight;
        _paintCursorOnTop = paintCursorAboveText;
        _enableInteractiveSelection = enableInteractiveSelection;
        _devicePixelRatio = devicePixelRatio;
        _startHandleLayerLink = startHandleLayerLink;
        _endHandleLayerLink = endHandleLayerLink;
        _obscuringCharacter = obscuringCharacter;
        _obscureText = obscureText;
        _readOnly = readOnly;
        _forceLine = forceLine;
        _clipBehavior = clipBehavior;
        _hasFocus = hasFocus ?? false;
        _disposeShowCursor = showCursor is null;
        System.Diagnostics.Debug.Assert(
            (maxLines is null)
                || (
                    (
                        maxLines
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) > 0L
                )
        );
        System.Diagnostics.Debug.Assert(
            (minLines is null)
                || (
                    (
                        minLines
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) > 0L
                )
        );
        System.Diagnostics.Debug.Assert(
            maxLines is null
                || minLines is null
                || maxLines
                    >= (
                        minLines
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
        );
        System.Diagnostics.Debug.Assert(!expands || ((maxLines is null) && (minLines is null)));
        System.Diagnostics.Debug.Assert(
            DartRuntimePrimitives.Identical(__textScaler, TextScaler.noScaling)
                || (textScaleFactor == 1.0)
        );
        System.Diagnostics.Debug.Assert(obscuringCharacter.characters().Count == 1L);
        System.Diagnostics.Debug.Assert(cursorWidth >= 0.0);
        System.Diagnostics.Debug.Assert((cursorHeight is null) || (cursorHeight >= 0.0));
        System.Diagnostics.Debug.Assert(!_showCursor.value || (cursorColor is not null));
        _selectionPainter.highlightColor = selectionColor;
        _selectionPainter.highlightedRange = selection;
        _selectionPainter.selectionHeightStyle = selectionHeightStyle;
        _selectionPainter.selectionWidthStyle = selectionWidthStyle;
        _autocorrectHighlightPainter.highlightColor = promptRectColor;
        _autocorrectHighlightPainter.highlightedRange = promptRectRange;
        _caretPainter.caretColor = cursorColor;
        _caretPainter.cursorRadius = cursorRadius;
        _caretPainter.cursorOffset = cursorOffset;
        _caretPainter.backgroundCursorColor = backgroundCursorColor;
        _updateForegroundPainter(foregroundPainter);
        _updatePainter(painter);
        addAll(children);
    }

    public override void dispose()
    {
        _leaderLayerHandler.layer = null;
        _foregroundRenderObject?.dispose();
        _foregroundRenderObject = null;
        _backgroundRenderObject?.dispose();
        _backgroundRenderObject = null;
        _clipRectLayer.layer = null;
        _cachedBuiltInForegroundPainters?.dispose();
        _cachedBuiltInPainters?.dispose();
        _selectionStartInViewport.dispose();
        _selectionEndInViewport.dispose();
        _autocorrectHighlightPainter.dispose();
        _selectionPainter.dispose();
        _caretPainter.dispose();
        _textPainter.dispose();
        _textIntrinsicsCache?.dispose();
        if (_disposeShowCursor)
        {
            _showCursor.dispose();
            _disposeShowCursor = false;
        }
        base.dispose();
    }

    internal virtual void _updateForegroundPainter(RenderEditablePainter? newPainter)
    {
        _CompositeRenderEditablePainter__editable effectivePainter =
            (newPainter is null)
                ? _builtInForegroundPainters
                : new _CompositeRenderEditablePainter__editable(
                    painters: new List<RenderEditablePainter>
                    {
                        _builtInForegroundPainters,
                        newPainter,
                    }
                );
        if (_foregroundRenderObject is null)
        {
            var foregroundRenderObject = new _RenderEditableCustomPaint__editable(
                painter: effectivePainter
            );
            adoptChild(foregroundRenderObject);
            _foregroundRenderObject = foregroundRenderObject;
        }
        else
        {
            _foregroundRenderObject?.painter = effectivePainter;
        }
        _foregroundPainter = newPainter;
    }

    public virtual RenderEditablePainter? foregroundPainter
    {
        get => _foregroundPainter;
        set
        {
            var newPainter = value;
            if (Equals(newPainter, _foregroundPainter))
            {
                return;
            }
            _updateForegroundPainter(newPainter);
        }
    }

    internal virtual void _updatePainter(RenderEditablePainter? newPainter)
    {
        _CompositeRenderEditablePainter__editable effectivePainter =
            (newPainter is null)
                ? _builtInPainters
                : new _CompositeRenderEditablePainter__editable(
                    painters: new List<RenderEditablePainter> { _builtInPainters, newPainter }
                );
        if (_backgroundRenderObject is null)
        {
            var backgroundRenderObject = new _RenderEditableCustomPaint__editable(
                painter: effectivePainter
            );
            adoptChild(backgroundRenderObject);
            _backgroundRenderObject = backgroundRenderObject;
        }
        else
        {
            _backgroundRenderObject?.painter = effectivePainter;
        }
        _painter = newPainter;
    }

    public virtual RenderEditablePainter? painter
    {
        get => _painter;
        set
        {
            var newPainter = value;
            if (Equals(newPainter, _painter))
            {
                return;
            }
            _updatePainter(newPainter);
        }
    }
    internal virtual _CompositeRenderEditablePainter__editable _builtInForegroundPainters =>
        _cachedBuiltInForegroundPainters ??= _createBuiltInForegroundPainters();

    internal virtual _CompositeRenderEditablePainter__editable _createBuiltInForegroundPainters()
    {
        var painters = new List<RenderEditablePainter>();
        if (paintCursorAboveText)
        {
            painters.Add(_caretPainter);
        }
        return new _CompositeRenderEditablePainter__editable(painters: painters);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual _CompositeRenderEditablePainter__editable _builtInPainters =>
        _cachedBuiltInPainters ??= _createBuiltInPainters();

    internal virtual _CompositeRenderEditablePainter__editable _createBuiltInPainters()
    {
        var painters = new List<RenderEditablePainter>
        {
            _autocorrectHighlightPainter,
            _selectionPainter,
        };
        if (!paintCursorAboveText)
        {
            painters.Add(_caretPainter);
        }
        return new _CompositeRenderEditablePainter__editable(painters: painters);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual TextHeightBehavior? textHeightBehavior
    {
        get => _textPainter.textHeightBehavior;
        set
        {
            var __value = value is null ? null : value;
            if (Equals(_textPainter.textHeightBehavior, __value))
            {
                return;
            }
            _textPainter.textHeightBehavior = __value;
            markNeedsLayout();
        }
    }
    public virtual TextWidthBasis textWidthBasis
    {
        get => _textPainter.textWidthBasis;
        set
        {
            var __value = value;
            if (Equals(_textPainter.textWidthBasis, (__value)))
            {
                return;
            }
            _textPainter.textWidthBasis = (__value);
            markNeedsLayout();
        }
    }
    public virtual double devicePixelRatio
    {
        get => _devicePixelRatio;
        set
        {
            var __value = value;
            if (devicePixelRatio == (__value))
            {
                return;
            }
            _devicePixelRatio = (__value);
            markNeedsLayout();
        }
    }
    public virtual string obscuringCharacter
    {
        get => _obscuringCharacter;
        set
        {
            var __value = value;
            if (_obscuringCharacter == __value)
            {
                return;
            }
            DartRuntimePrimitives.Assert(() => __value.characters().Count == 1L);
            _obscuringCharacter = __value;
            markNeedsLayout();
        }
    }
    public virtual bool obscureText
    {
        get => _obscureText;
        set
        {
            var __value = value;
            if (_obscureText == (__value))
            {
                return;
            }
            _obscureText = (__value);
            _cachedAttributedValue = null;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual BoxHeightStyle selectionHeightStyle
    {
        get => _selectionPainter.selectionHeightStyle;
        set
        {
            var __value = value;
            _selectionPainter.selectionHeightStyle = (__value);
        }
    }
    public virtual BoxWidthStyle selectionWidthStyle
    {
        get => _selectionPainter.selectionWidthStyle;
        set
        {
            var __value = value;
            _selectionPainter.selectionWidthStyle = (__value);
        }
    }
    public virtual ValueListenable<bool> selectionStartInViewport => _selectionStartInViewport;
    public virtual ValueListenable<bool> selectionEndInViewport => _selectionEndInViewport;

    internal virtual TextPosition _getTextPositionVertical(
        TextPosition position,
        double verticalOffset
    )
    {
        Offset caretOffset = _textPainter.getOffsetForCaret(position, _caretPrototype);
        Offset caretOffsetTranslated = caretOffset.translate(0.0, verticalOffset);
        return _textPainter.getPositionForOffset(caretOffsetTranslated);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual TextSelection getLineAtOffset(TextPosition position)
    {
        TextRange line = _textPainter.getLineBoundary(position);
        if (obscureText)
        {
            return new TextSelection(baseOffset: 0L, extentOffset: plainText.Length);
        }
        return new TextSelection(baseOffset: line.start, extentOffset: line.end);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual TextRange getWordBoundary(TextPosition position)
    {
        return _textPainter.getWordBoundary(position);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual TextPosition getTextPositionAbove(TextPosition position)
    {
        double preferredLineHeightLocal = _textPainter.preferredLineHeight;
        double verticalOffset = -0.5 * preferredLineHeightLocal;
        return _getTextPositionVertical(position, verticalOffset);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual TextPosition getTextPositionBelow(TextPosition position)
    {
        double preferredLineHeightLocal = _textPainter.preferredLineHeight;
        double verticalOffset = 1.5 * preferredLineHeightLocal;
        return _getTextPositionVertical(position, verticalOffset);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _updateSelectionExtentsVisibility(Offset effectiveOffset)
    {
        DartRuntimePrimitives.Assert(() => selection is not null);
        if (!selection!.isValid)
        {
            _selectionStartInViewport.value = false;
            _selectionEndInViewport.value = false;
            return;
        }
        Rect visibleRegion = Offset.zero & size;
        Offset startOffset = _textPainter.getOffsetForCaret(
            new TextPosition(offset: selection!.start, affinity: selection!.affinity),
            _caretPrototype
        );
        var visibleRegionSlop = 0.5;
        _selectionStartInViewport.value = visibleRegion
            .inflate(visibleRegionSlop)
            .contains(startOffset + effectiveOffset);
        Offset endOffset = _textPainter.getOffsetForCaret(
            new TextPosition(offset: selection!.end, affinity: selection!.affinity),
            _caretPrototype
        );
        _selectionEndInViewport.value = visibleRegion
            .inflate(visibleRegionSlop)
            .contains(endOffset + effectiveOffset);
    }

    internal virtual void _setTextEditingValue(
        TextEditingValue newValue,
        SelectionChangedCause cause
    )
    {
        textSelectionDelegate.userUpdateTextEditingValue(newValue, cause);
    }

    internal virtual void _setSelection(TextSelection nextSelection, SelectionChangedCause cause)
    {
        if (nextSelection.isValid)
        {
            long textLength = textSelectionDelegate.textEditingValue.text.Length;
            nextSelection = nextSelection.copyWith(
                baseOffset: Math.Min(nextSelection.baseOffset, textLength),
                extentOffset: Math.Min(nextSelection.extentOffset, textLength)
            );
        }
        _setTextEditingValue(
            textSelectionDelegate.textEditingValue.copyWith(selection: nextSelection),
            cause
        );
    }

    public override void markNeedsPaint()
    {
        base.markNeedsPaint();
        _foregroundRenderObject?.markNeedsPaint();
        _backgroundRenderObject?.markNeedsPaint();
    }

    public virtual void systemFontsDidChange()
    {
        markNeedsLayout();
        _textPainter.markNeedsLayout();
    }

    public virtual string plainText => _textPainter.plainText;
    public virtual InlineSpan? text
    {
        get => _textPainter.text;
        set
        {
            var __value = value;
            if (Equals(_textPainter.text, __value))
            {
                return;
            }
            _cachedLineBreakCount = null;
            _textPainter.text = __value;
            _cachedAttributedValue = null;
            _cachedCombinedSemanticsInfos = null;
            markNeedsLayout();
            markNeedsSemanticsUpdate();
        }
    }
    internal virtual TextPainter _textIntrinsics
    {
        get
        {
            return (
                (Func<TextPainter>)(
                    () =>
                    {
                        var __cascade = _textIntrinsicsCache ??= new TextPainter();
                        __cascade.text = _textPainter.text;
                        __cascade.textAlign = _textPainter.textAlign;
                        __cascade.textDirection = _textPainter.textDirection;
                        __cascade.textScaler = _textPainter.textScaler;
                        __cascade.maxLines = _textPainter.maxLines;
                        __cascade.ellipsis = _textPainter.ellipsis;
                        __cascade.locale = _textPainter.locale;
                        __cascade.strutStyle = _textPainter.strutStyle;
                        __cascade.textWidthBasis = _textPainter.textWidthBasis;
                        __cascade.textHeightBehavior = _textPainter.textHeightBehavior;
                        return __cascade;
                    }
                )
            )();
        }
    }
    public virtual TextAlign textAlign
    {
        get => _textPainter.textAlign;
        set
        {
            var __value = value;
            if (Equals(_textPainter.textAlign, (__value)))
            {
                return;
            }
            _textPainter.textAlign = (__value);
            markNeedsLayout();
        }
    }
    public virtual TextDirection textDirection
    {
        get =>
            (
                _textPainter.textDirection
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        set
        {
            var __value = value;
            if (Equals(_textPainter.textDirection, (__value)))
            {
                return;
            }
            _textPainter.textDirection = (__value);
            markNeedsLayout();
            markNeedsSemanticsUpdate();
        }
    }
    public virtual Locale? locale
    {
        get => _textPainter.locale;
        set
        {
            var __value = value;
            if (Equals(_textPainter.locale, __value))
            {
                return;
            }
            _textPainter.locale = __value;
            markNeedsLayout();
        }
    }
    public virtual Painting.StrutStyle? strutStyle
    {
        get => _textPainter.strutStyle;
        set
        {
            var __value = value;
            if (Equals(_textPainter.strutStyle, __value))
            {
                return;
            }
            _textPainter.strutStyle = __value;
            markNeedsLayout();
        }
    }
    public virtual Color? cursorColor
    {
        get => _caretPainter.caretColor;
        set
        {
            var __value = value is null ? null : value;
            _caretPainter.caretColor = __value;
        }
    }
    public virtual Color? backgroundCursorColor
    {
        get => _caretPainter.backgroundCursorColor;
        set
        {
            var __value = value is null ? null : value;
            _caretPainter.backgroundCursorColor = __value;
        }
    }
    public virtual ValueNotifier<bool> showCursor
    {
        get => _showCursor;
        set
        {
            var __value = value;
            if (Equals(_showCursor, __value))
            {
                return;
            }
            if (attached)
            {
                _showCursor.removeListener(_showHideCursor);
            }
            if (_disposeShowCursor)
            {
                _showCursor.dispose();
                _disposeShowCursor = false;
            }
            _showCursor = __value;
            if (attached)
            {
                _showHideCursor();
                _showCursor.addListener(_showHideCursor);
            }
        }
    }

    internal virtual void _showHideCursor()
    {
        _caretPainter.shouldPaint = showCursor.value;
    }

    public virtual bool hasFocus
    {
        get => _hasFocus;
        set
        {
            var __value = value;
            if (_hasFocus == (__value))
            {
                return;
            }
            _hasFocus = (__value);
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool forceLine
    {
        get => _forceLine;
        set
        {
            var __value = value;
            if (_forceLine == (__value))
            {
                return;
            }
            _forceLine = (__value);
            markNeedsLayout();
        }
    }
    public virtual bool readOnly
    {
        get => _readOnly;
        set
        {
            var __value = value;
            if (_readOnly == (__value))
            {
                return;
            }
            _readOnly = (__value);
            markNeedsSemanticsUpdate();
        }
    }
    public virtual long? maxLines
    {
        get => _maxLines;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() =>
                (__value is null)
                || (
                    (
                        __value
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) > 0L
                )
            );
            if (maxLines == __value)
            {
                return;
            }
            _maxLines = __value;
            _textPainter.maxLines = (__value == 1L) ? 1L : null;
            markNeedsLayout();
        }
    }
    public virtual long? minLines
    {
        get => _minLines;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() =>
                (__value is null)
                || (
                    (
                        __value
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) > 0L
                )
            );
            if (minLines == __value)
            {
                return;
            }
            _minLines = __value;
            markNeedsLayout();
        }
    }
    public virtual bool expands
    {
        get => _expands;
        set
        {
            var __value = value;
            if (expands == (__value))
            {
                return;
            }
            _expands = (__value);
            markNeedsLayout();
        }
    }
    public virtual Color? selectionColor
    {
        get => _selectionPainter.highlightColor;
        set
        {
            var __value = value is null ? null : value;
            _selectionPainter.highlightColor = __value;
        }
    }
    public virtual double textScaleFactor
    {
        get => _textPainter.textScaleFactor;
        set
        {
            var __value = value;
            textScaler = TextScaler.CreateLinear((__value));
        }
    }
    public virtual TextScaler textScaler
    {
        get => _textPainter.textScaler;
        set
        {
            var __value = value;
            if (Equals(_textPainter.textScaler, __value))
            {
                return;
            }
            _textPainter.textScaler = __value;
            markNeedsLayout();
        }
    }
    public virtual TextSelection? selection
    {
        get => _selection;
        set
        {
            var __value = value;
            if (Equals(_selection, __value))
            {
                return;
            }
            _selection = __value;
            _selectionPainter.highlightedRange = __value;
            markNeedsPaint();
            markNeedsSemanticsUpdate();
        }
    }
    public virtual ViewportOffset offset
    {
        get => _offset;
        set
        {
            var __value = value;
            if (Equals(_offset, __value))
            {
                return;
            }
            if (attached)
            {
                _offset.removeListener(markNeedsPaint);
            }
            _offset = __value;
            if (attached)
            {
                _offset.addListener(markNeedsPaint);
            }
            markNeedsLayout();
        }
    }
    public virtual double cursorWidth
    {
        get => _cursorWidth;
        set
        {
            var __value = value;
            if (_cursorWidth == (__value))
            {
                return;
            }
            _cursorWidth = (__value);
            markNeedsLayout();
        }
    }
    public virtual double cursorHeight
    {
        get => _cursorHeight ?? preferredLineHeight;
        set => setCursorHeight(value);
    }

    public virtual void setCursorHeight(double? value)
    {
        if (_cursorHeight == value)
        {
            return;
        }
        _cursorHeight = value;
        markNeedsLayout();
    }

    public virtual bool paintCursorAboveText
    {
        get => _paintCursorOnTop;
        set
        {
            var __value = value;
            if (_paintCursorOnTop == (__value))
            {
                return;
            }
            _paintCursorOnTop = (__value);
            _cachedBuiltInForegroundPainters = null;
            _cachedBuiltInPainters = null;
            _updateForegroundPainter(_foregroundPainter);
            _updatePainter(_painter);
        }
    }
    public virtual Offset cursorOffset
    {
        get => _caretPainter.cursorOffset;
        set
        {
            var __value = value;
            _caretPainter.cursorOffset = (__value);
        }
    }
    public virtual Radius? cursorRadius
    {
        get => _caretPainter.cursorRadius;
        set
        {
            var __value = value;
            _caretPainter.cursorRadius = __value;
        }
    }
    public virtual LayerLink startHandleLayerLink
    {
        get => _startHandleLayerLink;
        set
        {
            var __value = value;
            if (Equals(_startHandleLayerLink, __value))
            {
                return;
            }
            _startHandleLayerLink = __value;
            markNeedsPaint();
        }
    }
    public virtual LayerLink endHandleLayerLink
    {
        get => _endHandleLayerLink;
        set
        {
            var __value = value;
            if (Equals(_endHandleLayerLink, __value))
            {
                return;
            }
            _endHandleLayerLink = __value;
            markNeedsPaint();
        }
    }
    public virtual bool floatingCursorOn => _floatingCursorOn;
    public virtual bool? enableInteractiveSelection
    {
        get => _enableInteractiveSelection;
        set
        {
            var __value = value;
            if (_enableInteractiveSelection == __value)
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
        get { return enableInteractiveSelection ?? !obscureText; }
    }
    public virtual Color? promptRectColor
    {
        get => _autocorrectHighlightPainter.highlightColor;
        set
        {
            var newValue = value is null ? null : value;
            _autocorrectHighlightPainter.highlightColor = newValue;
        }
    }

    public virtual void setPromptRectRange(TextRange? newRange)
    {
        _autocorrectHighlightPainter.highlightedRange = newRange;
    }

    public virtual double maxScrollExtent => _maxScrollExtent;
    internal virtual double _caretMargin => EditableLibrary._kCaretGap + cursorWidth;
    public virtual Clip clipBehavior
    {
        get => _clipBehavior;
        set
        {
            var __value = value;
            if (!Equals((__value), _clipBehavior))
            {
                _clipBehavior = (__value);
                markNeedsPaint();
                markNeedsSemanticsUpdate();
            }
        }
    }

    public virtual List<TextBox> getBoxesForSelection(TextSelection selection)
    {
        _computeTextMetricsIfNeeded();
        return _textPainter
            .getBoxesForSelection(
                selection,
                boxHeightStyle: selectionHeightStyle,
                boxWidthStyle: selectionWidthStyle
            )
            .map(
                (textBox) =>
                    new TextBox(
                        textBox.left + _paintOffset.dx,
                        textBox.top + _paintOffset.dy,
                        textBox.right + _paintOffset.dx,
                        textBox.bottom + _paintOffset.dy,
                        textBox.direction
                    )
            )
            .ToList();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        _semanticsInfo = _textPainter.text!.getSemanticsInformation();
        if (
            _semanticsInfo!.any((info) => info.recognizer is not null)
            && (!Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.macOS))
        )
        {
            DartRuntimePrimitives.Assert(() => readOnly && !obscureText);
            (
                (Func<SemanticsConfiguration>)(
                    () =>
                    {
                        var __cascade = config;
                        __cascade.isSemanticBoundary = true;
                        __cascade.explicitChildNodes = true;
                        return __cascade;
                    }
                )
            )();
            return;
        }
        if (_cachedAttributedValue is null)
        {
            if (obscureText)
            {
                _cachedAttributedValue = new AttributedString(
                    DartCoreExtensions.repeat(obscuringCharacter, plainText.Length)
                );
            }
            else
            {
                var buffer = new StringBuffer();
                var offset = 0L;
                var attributesLocal = new List<StringAttribute>();
                foreach (InlineSpanSemanticsInformation infoLocal in _semanticsInfo!)
                {
                    string label = infoLocal.semanticsLabel ?? infoLocal.text;
                    foreach (StringAttribute infoAttribute in infoLocal.stringAttributes)
                    {
                        TextRange originalRange = infoAttribute.range;
                        attributesLocal.Add(
                            infoAttribute.copy(
                                range: new TextRange(
                                    start: offset + originalRange.start,
                                    end: offset + originalRange.end
                                )
                            )
                        );
                    }
                    buffer.write(label);
                    offset += label.Length;
                }
                _cachedAttributedValue = new AttributedString(
                    buffer.ToString(),
                    attributes: attributesLocal
                );
            }
        }
        (
            (Func<SemanticsConfiguration>)(
                () =>
                {
                    var __cascade = config;
                    __cascade.attributedValue = _cachedAttributedValue!;
                    __cascade.isObscured = obscureText;
                    __cascade.isMultiline = _isMultiline;
                    __cascade.textDirection = textDirection;
                    __cascade.isFocused = hasFocus;
                    __cascade.isFocusable = true;
                    __cascade.isTextField = true;
                    __cascade.isReadOnly = readOnly;
                    __cascade.inputType = DorotiUiLibrary.SemanticsInputType.text;
                    return __cascade;
                }
            )
        )();
        if (hasFocus && selectionEnabled)
        {
            config.onSetSelection = _handleSetSelection;
        }
        if (hasFocus && !readOnly)
        {
            config.onSetText = _handleSetText;
        }
        if (selectionEnabled && (selection?.isValid ?? false))
        {
            config.textSelection = selection;
            if (_textPainter.getOffsetBefore(selection!.extentOffset) is not null)
            {
                (
                    (Func<SemanticsConfiguration>)(
                        () =>
                        {
                            var __cascade = config;
                            __cascade.onMoveCursorBackwardByWord = _handleMoveCursorBackwardByWord;
                            __cascade.onMoveCursorBackwardByCharacter =
                                _handleMoveCursorBackwardByCharacter;
                            return __cascade;
                        }
                    )
                )();
            }
            if (_textPainter.getOffsetAfter(selection!.extentOffset) is not null)
            {
                (
                    (Func<SemanticsConfiguration>)(
                        () =>
                        {
                            var __cascade = config;
                            __cascade.onMoveCursorForwardByWord = _handleMoveCursorForwardByWord;
                            __cascade.onMoveCursorForwardByCharacter =
                                _handleMoveCursorForwardByCharacter;
                            return __cascade;
                        }
                    )
                )();
            }
        }
    }

    internal virtual void _handleSetText(string text)
    {
        textSelectionDelegate.userUpdateTextEditingValue(
            new TextEditingValue(
                text: text,
                selection: TextSelection.CreateCollapsed(offset: text.Length)
            ),
            SelectionChangedCause.keyboard
        );
    }

    public override void assembleSemanticsNode(
        SemanticsNode node,
        SemanticsConfiguration config,
        IEnumerable<SemanticsNode> children
    )
    {
        DartRuntimePrimitives.Assert(() =>
            (_semanticsInfo is not null) && (checked((long)_semanticsInfo!.Count) != 0)
        );
        var newChildren = new List<SemanticsNode>();
        TextDirection currentDirection = textDirection;
        Rect currentRect = default!;
        var ordinal = 0.0;
        var start = 0L;
        var placeholderIndex = 0L;
        var childIndex = 0L;
        RenderBox? child = firstChild;
        var newChildCache = new DartMap<Key, SemanticsNode>();
        _cachedCombinedSemanticsInfos ??= Inline_spanLibrary.combineSemanticsInfo(_semanticsInfo!);
        foreach (InlineSpanSemanticsInformation info in _cachedCombinedSemanticsInfos!)
        {
            var selection = new TextSelection(
                baseOffset: start,
                extentOffset: start + info.text.Length
            );
            start += info.text.Length;
            if (info.isPlaceholder)
            {
                while (
                    (children.Count() > childIndex)
                    && children
                        .elementAt(childIndex)
                        .isTagged(new PlaceholderSpanIndexSemanticsTag(placeholderIndex))
                )
                {
                    SemanticsNode childNode = children.elementAt(childIndex);
                    var parentDataLocal = ((TextParentData?)child!.parentData!)!;
                    DartRuntimePrimitives.Assert(() => parentDataLocal.offset is not null);
                    newChildren.Add(childNode);
                    childIndex += 1L;
                }
                child = childAfter(child!);
                placeholderIndex += 1L;
            }
            else
            {
                var initialDirection = currentDirection;
                List<TextBox> rects = _textPainter.getBoxesForSelection(selection);
                if (checked((long)rects.Count) == 0)
                {
                    continue;
                }
                Rect rectLocal = rects.First().toRect();
                currentDirection = rects.First().direction;
                foreach (TextBox textBox in rects.skip(1L))
                {
                    rectLocal = rectLocal.expandToInclude(textBox.toRect());
                    currentDirection = textBox.direction;
                }
                rectLocal = Rect.fromLTWH(
                    Math.Max(0.0, rectLocal.left),
                    Math.Max(0.0, rectLocal.top),
                    Math.Min(rectLocal.width, constraints.maxWidth),
                    Math.Min(rectLocal.height, constraints.maxHeight)
                );
                currentRect = Rect.fromLTRB(
                    rectLocal.left.floorToDouble() - 4.0,
                    rectLocal.top.floorToDouble() - 4.0,
                    rectLocal.right.ceilToDouble() + 4.0,
                    rectLocal.bottom.ceilToDouble() + 4.0
                );
                var configuration = (
                    (Func<SemanticsConfiguration>)(
                        () =>
                        {
                            var __cascade = new SemanticsConfiguration();
                            __cascade.sortKey = new OrdinalSortKey(ordinal++);
                            __cascade.textDirection = initialDirection;
                            __cascade.attributedLabel = new AttributedString(
                                info.semanticsLabel ?? info.text,
                                attributes: info.stringAttributes
                            );
                            return __cascade;
                        }
                    )
                )();
                switch (info.recognizer)
                {
                    case TapGestureRecognizer { onTap: Action handler } __object55475:
                    {
                        if (handler is not null)
                        {
                            configuration.onTap = handler;
                            configuration.isLink = true;
                        }
                        break;
                    }
                    case DoubleTapGestureRecognizer
                    {
                        onDoubleTap: Action handlerLocal
                    } __object55548:
                    {
                        if (handlerLocal is not null)
                        {
                            configuration.onTap = handlerLocal;
                            configuration.isLink = true;
                        }
                        break;
                    }
                    case LongPressGestureRecognizer
                    {
                        onLongPress: Action onLongPressLocal
                    } __object55770:
                    {
                        if (onLongPressLocal is not null)
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
                if (node.parentPaintClipRect is not null)
                {
                    Rect paintRect = (
                        node.parentPaintClipRect
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ).intersect(currentRect);
                    configuration.isHidden = paintRect.isEmpty && !currentRect.isEmpty;
                }
                SemanticsNode newChild = default!;
                if (
                    (
                        ((long?)(_cachedChildNodes?.Count)) is { } __count56386
                            ? __count56386 != 0
                            : (bool?)null
                    ) ?? false
                )
                {
                    newChild = _cachedChildNodes!.remove(_cachedChildNodes!.Keys.First())!;
                }
                else
                {
                    var keyLocal = new UniqueKey();
                    newChild = new SemanticsNode(
                        key: keyLocal,
                        showOnScreen: _createShowOnScreenFor(keyLocal)
                    );
                }
                (
                    (Func<SemanticsNode>)(
                        () =>
                        {
                            var __cascade = newChild;
                            __cascade.updateWith(config: configuration);
                            __cascade.rect = currentRect;
                            return __cascade;
                        }
                    )
                )();
                newChildCache[newChild.key!] = newChild;
                newChildren.Add(newChild);
            }
        }
        _cachedChildNodes = newChildCache.cast<Key, SemanticsNode>();
        node.updateWith(config: config, childrenInInversePaintOrder: newChildren);
    }

    internal virtual Action? _createShowOnScreenFor(Key key)
    {
        return () =>
        {
            SemanticsNode node = _cachedChildNodes!.GetValueOrDefault(key)!;
            showOnScreen(descendant: this, rect: node.rect);
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _handleSetSelection(TextSelection selection)
    {
        _setSelection(selection, SelectionChangedCause.keyboard);
    }

    internal virtual void _handleMoveCursorForwardByCharacter(bool extendSelection)
    {
        DartRuntimePrimitives.Assert(() => selection is not null);
        long? extentOffsetLocal = _textPainter.getOffsetAfter(selection!.extentOffset);
        if (extentOffsetLocal is null)
        {
            return;
        }
        long baseOffsetLocal = !extendSelection
            ? (
                extentOffsetLocal
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
            : selection!.baseOffset;
        _setSelection(
            new TextSelection(
                baseOffset: baseOffsetLocal,
                extentOffset: (
                    extentOffsetLocal
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            ),
            SelectionChangedCause.keyboard
        );
    }

    internal virtual void _handleMoveCursorBackwardByCharacter(bool extendSelection)
    {
        DartRuntimePrimitives.Assert(() => selection is not null);
        long? extentOffsetLocal = _textPainter.getOffsetBefore(selection!.extentOffset);
        if (extentOffsetLocal is null)
        {
            return;
        }
        long baseOffsetLocal = !extendSelection
            ? (
                extentOffsetLocal
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
            : selection!.baseOffset;
        _setSelection(
            new TextSelection(
                baseOffset: baseOffsetLocal,
                extentOffset: (
                    extentOffsetLocal
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            ),
            SelectionChangedCause.keyboard
        );
    }

    internal virtual void _handleMoveCursorForwardByWord(bool extendSelection)
    {
        DartRuntimePrimitives.Assert(() => selection is not null);
        TextRange currentWord = _textPainter.getWordBoundary(selection!.extent);
        TextRange? nextWord = _getNextWord(currentWord.end);
        if (nextWord is null)
        {
            return;
        }
        long baseOffsetLocal = extendSelection ? selection!.baseOffset : nextWord.start;
        _setSelection(
            new TextSelection(baseOffset: baseOffsetLocal, extentOffset: nextWord.start),
            SelectionChangedCause.keyboard
        );
    }

    internal virtual void _handleMoveCursorBackwardByWord(bool extendSelection)
    {
        DartRuntimePrimitives.Assert(() => selection is not null);
        TextRange currentWord = _textPainter.getWordBoundary(selection!.extent);
        TextRange? previousWord = _getPreviousWord(currentWord.start - 1L);
        if (previousWord is null)
        {
            return;
        }
        long baseOffsetLocal = extendSelection ? selection!.baseOffset : previousWord.start;
        _setSelection(
            new TextSelection(baseOffset: baseOffsetLocal, extentOffset: previousWord.start),
            SelectionChangedCause.keyboard
        );
    }

    internal virtual TextRange? _getNextWord(long offset)
    {
        while (true)
        {
            TextRange range = _textPainter.getWordBoundary(new TextPosition(offset: offset));
            if (!range.isValid || range.isCollapsed)
            {
                return null;
            }
            if (!_onlyWhitespace(range))
            {
                return range;
            }
            offset = range.end;
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual TextRange? _getPreviousWord(long offset)
    {
        while (offset >= 0L)
        {
            TextRange range = _textPainter.getWordBoundary(new TextPosition(offset: offset));
            if (!range.isValid || range.isCollapsed)
            {
                return null;
            }
            if (!_onlyWhitespace(range))
            {
                return range;
            }
            offset = range.start - 1L;
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual bool _onlyWhitespace(TextRange range)
    {
        for (long i = range.start; i < range.end; i++)
        {
            long codeUnit = (
                text!.codeUnitAt(i)
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            if (!TextLayoutMetrics.isWhitespace(codeUnit))
            {
                return false;
            }
        }
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            child.attach(owner);
            var childParentData = ((TextParentData?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        _foregroundRenderObject?.attach(owner);
        _backgroundRenderObject?.attach(owner);
        _tap = (
            (Func<TapGestureRecognizer>)(
                () =>
                {
                    var __cascade = new TapGestureRecognizer(debugOwner: this);
                    __cascade.onTapDown = _handleTapDown;
                    __cascade.onTap = _handleTap;
                    return __cascade;
                }
            )
        )();
        _longPress = (
            (Func<LongPressGestureRecognizer>)(
                () =>
                {
                    var __cascade = new LongPressGestureRecognizer(debugOwner: this);
                    __cascade.onLongPress = _handleLongPress;
                    return __cascade;
                }
            )
        )();
        _offset.addListener(markNeedsPaint);
        _showHideCursor();
        _showCursor.addListener(_showHideCursor);
    }

    public override void detach()
    {
        _tap.dispose();
        _longPress.dispose();
        _offset.removeListener(markNeedsPaint);
        _showCursor.removeListener(_showHideCursor);
        base.detach();
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            child.detach();
            var childParentData = ((TextParentData?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        _foregroundRenderObject?.detach();
        _backgroundRenderObject?.detach();
    }

    public override void redepthChildren()
    {
        RenderObject? foregroundChild = _foregroundRenderObject;
        RenderObject? backgroundChild = _backgroundRenderObject;
        if (foregroundChild is not null)
        {
            redepthChild(foregroundChild);
        }
        if (backgroundChild is not null)
        {
            redepthChild(backgroundChild);
        }
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            redepthChild(child);
            var childParentData = ((TextParentData?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderObject? foregroundChild = _foregroundRenderObject;
        RenderObject? backgroundChild = _backgroundRenderObject;
        if (foregroundChild is not null)
        {
            visitor(foregroundChild);
        }
        if (backgroundChild is not null)
        {
            visitor(backgroundChild);
        }
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            visitor(child);
            var childParentData = ((TextParentData?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    internal virtual bool _isMultiline => maxLines != 1L;
    internal virtual Axis _viewportAxis => _isMultiline ? Axis.vertical : Axis.horizontal;
    internal virtual Offset _paintOffset =>
        _viewportAxis switch
        {
            Axis.horizontal => new Offset(-offset.pixels, 0.0),
            Axis.vertical => new Offset(0.0, -offset.pixels),
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
    internal virtual double _viewportExtent
    {
        get
        {
            DartRuntimePrimitives.Assert(() => hasSize);
            return _viewportAxis switch
            {
                Axis.horizontal => size.width,
                Axis.vertical => size.height,
                _ => throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
            };
        }
    }

    internal virtual double _getMaxScrollExtent(Size contentSize)
    {
        DartRuntimePrimitives.Assert(() => hasSize);
        return _viewportAxis switch
        {
            Axis.horizontal => Math.Max(0.0, contentSize.width - size.width),
            Axis.vertical => Math.Max(0.0, contentSize.height - size.height),
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual bool _hasVisualOverflow =>
        (_maxScrollExtent > 0L) || (!Equals(_paintOffset, Offset.zero));

    public virtual List<TextSelectionPoint> getEndpointsForSelection(TextSelection selection)
    {
        _computeTextMetricsIfNeeded();
        Offset paintOffset = _paintOffset;
        List<TextBox> boxes = selection.isCollapsed
            ? new List<TextBox>()
            : _textPainter.getBoxesForSelection(
                selection,
                boxHeightStyle: selectionHeightStyle,
                boxWidthStyle: selectionWidthStyle
            );
        if (checked((long)boxes.Count) == 0)
        {
            Offset caretOffset = _textPainter.getOffsetForCaret(selection.extent, _caretPrototype);
            Offset startLocal = new Offset(0.0, preferredLineHeight) + caretOffset + paintOffset;
            if (
                selection.isCollapsed
                && PlatformLibrary.defaultTargetPlatform == TargetPlatform.android
            )
            {
                // The Material insertion handle points at the caret's center, not
                // its leading edge. Use the painted rect so custom cursor widths,
                // offsets, scrolling, and physical-pixel snapping stay aligned.
                startLocal = new Offset(
                    getLocalRectForCaret(selection.extent).center.dx,
                    startLocal.dy
                );
            }
            return new List<TextSelectionPoint> { new TextSelectionPoint(startLocal, null) };
        }
        else
        {
            Offset startAlternate =
                new Offset(
                    DorotiUiLibrary.clampDouble(boxes.First().start, 0, _textPainter.size.width),
                    boxes.First().bottom
                ) + paintOffset;
            Offset endLocal =
                new Offset(
                    DorotiUiLibrary.clampDouble(boxes.Last().end, 0, _textPainter.size.width),
                    boxes.Last().bottom
                ) + paintOffset;
            return new List<TextSelectionPoint>
            {
                new TextSelectionPoint(startAlternate, boxes.First().direction),
                new TextSelectionPoint(endLocal, boxes.Last().direction),
            };
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Rect? getRectForComposingRange(TextRange range)
    {
        if (!range.isValid || range.isCollapsed)
        {
            return null;
        }
        _computeTextMetricsIfNeeded();
        List<TextBox> boxes = _textPainter.getBoxesForSelection(
            new TextSelection(baseOffset: range.start, extentOffset: range.end),
            boxHeightStyle: selectionHeightStyle,
            boxWidthStyle: selectionWidthStyle
        );
        return Enumerable
            .Aggregate(
                boxes,
                (Rect?)null,
                (accum, incoming) => accum?.expandToInclude(incoming.toRect()) ?? incoming.toRect()
            )
            ?.shift(_paintOffset);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual TextPosition getPositionForPoint(Offset globalPosition)
    {
        _computeTextMetricsIfNeeded();
        return _textPainter.getPositionForOffset(globalToLocal(globalPosition) - _paintOffset);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Rect getLocalRectForCaret(TextPosition caretPosition)
    {
        _computeTextMetricsIfNeeded();
        Rect caretPrototype = _caretPrototype;
        Offset caretOffset = _textPainter.getOffsetForCaret(caretPosition, caretPrototype);
        Rect caretRect = caretPrototype.shift(caretOffset + cursorOffset);
        double scrollableWidth = Math.Max(_textPainter.width + _caretMargin, size.width);
        double caretX = DorotiUiLibrary.clampDouble(
            caretRect.left,
            0,
            Math.Max(scrollableWidth - _caretMargin, 0)
        );
        caretRect = new Offset(caretX, caretRect.top) & caretRect.size;
        double fullHeight = _textPainter.getFullHeightForCaret(caretPosition, caretPrototype);
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case var __constant67807 when Equals(__constant67807, TargetPlatform.iOS):
            case var __constant67838 when Equals(__constant67838, TargetPlatform.macOS):
            {
                double heightDiff = fullHeight - caretRect.height;
                caretRect = Rect.fromLTWH(
                    caretRect.left,
                    caretRect.top + (heightDiff / 2L),
                    caretRect.width,
                    caretRect.height
                );
                break;
            }
            case var __constant68160 when Equals(__constant68160, TargetPlatform.android):
            case var __constant68195 when Equals(__constant68195, TargetPlatform.fuchsia):
            case var __constant68230 when Equals(__constant68230, TargetPlatform.linux):
            case var __constant68263 when Equals(__constant68263, TargetPlatform.windows):
            {
                double caretHeight = cursorHeight;
                double heightDiffLocal = fullHeight - caretHeight;
                caretRect = Rect.fromLTWH(
                    caretRect.left,
                    caretRect.top - EditableLibrary._kCaretHeightOffset + (heightDiffLocal / 2L),
                    caretRect.width,
                    caretHeight
                );
                break;
            }
        }
        caretRect = caretRect.shift(_paintOffset);
        return caretRect.shift(_snapToPhysicalPixel(caretRect.topLeft));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        List<PlaceholderDimensions> placeholderDimensions = layoutInlineChildren(
            double.PositiveInfinity,
            (child, constraints) =>
                new Size(child.getMinIntrinsicWidth(double.PositiveInfinity), 0.0),
            ChildLayoutHelper.getDryBaseline
        );
        var (minWidthLocal, maxWidthLocal) = _adjustConstraints();
        return (
            (Func<TextPainter>)(
                () =>
                {
                    var __cascade = _textIntrinsics;
                    __cascade.setPlaceholderDimensions(placeholderDimensions);
                    __cascade.layout(minWidth: minWidthLocal, maxWidth: maxWidthLocal);
                    return __cascade;
                }
            )
        )().minIntrinsicWidth;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        List<PlaceholderDimensions> placeholderDimensions = layoutInlineChildren(
            double.PositiveInfinity,
            (child, constraints) =>
                new Size(child.getMaxIntrinsicWidth(double.PositiveInfinity), 0.0),
            ChildLayoutHelper.getDryBaseline
        );
        var (minWidthLocal, maxWidthLocal) = _adjustConstraints();
        return (
                (Func<TextPainter>)(
                    () =>
                    {
                        var __cascade = _textIntrinsics;
                        __cascade.setPlaceholderDimensions(placeholderDimensions);
                        __cascade.layout(minWidth: minWidthLocal, maxWidth: maxWidthLocal);
                        return __cascade;
                    }
                )
            )().maxIntrinsicWidth + _caretMargin;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual double preferredLineHeight => _textPainter.preferredLineHeight;

    internal virtual long _countHardLineBreaks(string text)
    {
        long? cachedValue = _cachedLineBreakCount;
        if (cachedValue is not null)
        {
            long cachedValue__70677__value70722 = (
                cachedValue
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            return (cachedValue__70677__value70722);
        }
        var count = 0L;
        for (var index = 0L; index < text.Length; index += 1L)
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
        _cachedLineBreakCount = count;
        return count;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual double _preferredHeight(double width)
    {
        long? maxLinesLocal = maxLines;
        long? minLinesLocal = minLines ?? maxLinesLocal;
        double minHeight = preferredLineHeight * (minLinesLocal ?? 0L);
        DartRuntimePrimitives.Assert(() =>
            (maxLinesLocal != 1L) || (_textIntrinsics.maxLines == 1L)
        );
        if (maxLinesLocal is null)
        {
            double estimatedHeight = default!;
            if (width == double.PositiveInfinity)
            {
                estimatedHeight = preferredLineHeight * (_countHardLineBreaks(plainText) + 1L);
            }
            else
            {
                var (minWidthLocal, maxWidthLocal) = _adjustConstraints(maxWidth: width);
                estimatedHeight = (
                    (Func<TextPainter>)(
                        () =>
                        {
                            var __cascade = _textIntrinsics;
                            __cascade.layout(minWidth: minWidthLocal, maxWidth: maxWidthLocal);
                            return __cascade;
                        }
                    )
                )().height;
            }
            return Math.Max(estimatedHeight, minHeight);
        }
        if (
            (
                maxLinesLocal
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ) == 1L
        )
        {
            var (minWidthAlternate, maxWidthAlternate) = _adjustConstraints(maxWidth: width);
            return (
                (Func<TextPainter>)(
                    () =>
                    {
                        var __cascade = _textIntrinsics;
                        __cascade.layout(minWidth: minWidthAlternate, maxWidth: maxWidthAlternate);
                        return __cascade;
                    }
                )
            )().height;
        }
        if (
            minLinesLocal
            == (
                maxLinesLocal
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        )
        {
            return minHeight;
        }
        double maxHeight =
            preferredLineHeight
            * (
                maxLinesLocal
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        var (minWidthNested, maxWidthNested) = _adjustConstraints(maxWidth: width);
        return DorotiUiLibrary.clampDouble(
            (
                (Func<TextPainter>)(
                    () =>
                    {
                        var __cascade = _textIntrinsics;
                        __cascade.layout(minWidth: minWidthNested, maxWidth: maxWidthNested);
                        return __cascade;
                    }
                )
            )().height,
            minHeight,
            maxHeight
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMinIntrinsicHeight(double width) => getMaxIntrinsicHeight(width);

    public override double computeMaxIntrinsicHeight(double width)
    {
        _textIntrinsics.setPlaceholderDimensions(
            layoutInlineChildren(
                width,
                ChildLayoutHelper.dryLayoutChild,
                ChildLayoutHelper.getDryBaseline
            )
        );
        return _preferredHeight(width);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        _computeTextMetricsIfNeeded();
        return _textPainter.computeDistanceToActualBaseline(baseline);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool hitTestSelf(Offset position) => true;

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        Offset effectivePosition = position - _paintOffset;
        GlyphInfo? glyph = _textPainter.getClosestGlyphForOffset(effectivePosition);
        InlineSpan? spanHit =
            ((glyph is not null) && glyph.graphemeClusterLayoutBounds.contains(effectivePosition))
                ? _textPainter.text!.getSpanForPosition(
                    new TextPosition(offset: glyph.graphemeClusterCodeUnitRange.start)
                )
                : null;
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
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void handleEvent(PointerEvent @event, HitTestEntry<HitTestTarget> entry)
    {
        DartRuntimePrimitives.Assert(() => debugHandleEvent(@event, entry));
        if (@event is Gestures.PointerDownEvent)
        {
            Gestures.PointerDownEvent @event__as74778 = (Gestures.PointerDownEvent)@event;
            DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
            if (!ignorePointer)
            {
                _tap.addPointer(@event__as74778);
                _longPress.addPointer(@event__as74778);
            }
        }
    }

    public virtual Offset? lastSecondaryTapDownPosition => _lastSecondaryTapDownPosition;

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
        DartRuntimePrimitives.Assert(() => !ignorePointer);
        handleTapDown(details);
    }

    public virtual void handleTap()
    {
        selectPosition(cause: SelectionChangedCause.tap);
    }

    internal virtual void _handleTap()
    {
        DartRuntimePrimitives.Assert(() => !ignorePointer);
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
        DartRuntimePrimitives.Assert(() => !ignorePointer);
        handleLongPress();
    }

    public virtual void selectPosition(SelectionChangedCause cause)
    {
        selectPositionAt(
            from: (
                _lastTapDownPosition
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            cause: cause
        );
    }

    public virtual void selectPositionAt(
        Offset from,
        Offset? to = null,
        SelectionChangedCause cause = default!
    )
    {
        _computeTextMetricsIfNeeded();
        TextPosition fromPosition = _textPainter.getPositionForOffset(
            globalToLocal(from) - _paintOffset
        );
        TextPosition? toPosition =
            (to is null)
                ? null
                : _textPainter.getPositionForOffset(
                    globalToLocal(
                        (
                            (
                                to
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            )
                        )
                    ) - _paintOffset
                );
        long baseOffsetLocal = fromPosition.offset;
        long extentOffsetLocal = toPosition?.offset ?? fromPosition.offset;
        var newSelection = new TextSelection(
            baseOffset: baseOffsetLocal,
            extentOffset: extentOffsetLocal,
            affinity: fromPosition.affinity
        );
        _setSelection(newSelection, cause);
    }

    public virtual WordBoundary wordBoundaries => _textPainter.wordBoundaries;

    public virtual void selectWord(SelectionChangedCause cause)
    {
        selectWordsInRange(
            from: (
                _lastTapDownPosition
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            cause: cause
        );
    }

    public virtual void selectWordsInRange(
        Offset from,
        Offset? to = null,
        SelectionChangedCause cause = default!
    )
    {
        _computeTextMetricsIfNeeded();
        TextPosition fromPosition = _textPainter.getPositionForOffset(
            globalToLocal(from) - _paintOffset
        );
        TextSelection fromWord = getWordAtOffset(fromPosition);
        TextPosition toPosition =
            (to is null)
                ? fromPosition
                : _textPainter.getPositionForOffset(
                    globalToLocal(
                        (
                            (
                                to
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            )
                        )
                    ) - _paintOffset
                );
        TextSelection toWord = Equals(toPosition, fromPosition)
            ? fromWord
            : getWordAtOffset(toPosition);
        bool isFromWordBeforeToWord = fromWord.start < toWord.end;
        _setSelection(
            new TextSelection(
                baseOffset: isFromWordBeforeToWord ? fromWord.@base.offset : fromWord.extent.offset,
                extentOffset: isFromWordBeforeToWord ? toWord.extent.offset : toWord.@base.offset,
                affinity: fromWord.affinity
            ),
            cause
        );
    }

    public virtual void selectWordEdge(SelectionChangedCause cause)
    {
        _computeTextMetricsIfNeeded();
        DartRuntimePrimitives.Assert(() => _lastTapDownPosition is not null);
        TextPosition position = _textPainter.getPositionForOffset(
            globalToLocal(
                (
                    _lastTapDownPosition
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            ) - _paintOffset
        );
        TextRange word = _textPainter.getWordBoundary(position);
        TextSelection newSelection = default!;
        if (position.offset <= word.start)
        {
            newSelection = TextSelection.CreateCollapsed(offset: word.start);
        }
        else
        {
            newSelection = TextSelection.CreateCollapsed(
                offset: word.end,
                affinity: TextAffinity.upstream
            );
        }
        _setSelection(newSelection, cause);
    }

    public virtual TextSelection getWordAtOffset(TextPosition position)
    {
        if (position.offset >= plainText.Length)
        {
            return TextSelection.CreateFromPosition(
                new TextPosition(offset: plainText.Length, affinity: TextAffinity.upstream)
            );
        }
        if (obscureText)
        {
            return new TextSelection(baseOffset: 0L, extentOffset: plainText.Length);
        }
        TextRange word = _textPainter.getWordBoundary(position);
        long effectiveOffset = default!;
        switch (position.affinity)
        {
            case TextAffinity.upstream:
            {
                effectiveOffset = position.offset - 1L;
                break;
            }
            case TextAffinity.downstream:
            {
                effectiveOffset = position.offset;
                break;
            }
        }
        DartRuntimePrimitives.Assert(() => effectiveOffset >= 0L);
        if (
            (effectiveOffset > 0L)
            && TextLayoutMetrics.isWhitespace(plainText.codeUnitAt(effectiveOffset))
        )
        {
            TextRange? previousWord = _getPreviousWord(word.start);
            switch (PlatformLibrary.defaultTargetPlatform)
            {
                case var __constant83254 when Equals(__constant83254, TargetPlatform.iOS):
                {
                    if (previousWord is null)
                    {
                        TextRange? nextWord = _getNextWord(word.start);
                        if (nextWord is null)
                        {
                            return TextSelection.CreateCollapsed(offset: position.offset);
                        }
                        return new TextSelection(
                            baseOffset: position.offset,
                            extentOffset: nextWord.end
                        );
                    }
                    return new TextSelection(
                        baseOffset: previousWord.start,
                        extentOffset: position.offset
                    );
                }
                case var __constant83710 when Equals(__constant83710, TargetPlatform.android):
                {
                    if (readOnly)
                    {
                        if (previousWord is null)
                        {
                            return new TextSelection(
                                baseOffset: position.offset,
                                extentOffset: position.offset + 1L
                            );
                        }
                        return new TextSelection(
                            baseOffset: previousWord.start,
                            extentOffset: position.offset
                        );
                    }
                    break;
                }
                case var __constant84036 when Equals(__constant84036, TargetPlatform.fuchsia):
                case var __constant84073 when Equals(__constant84073, TargetPlatform.macOS):
                case var __constant84108 when Equals(__constant84108, TargetPlatform.linux):
                case var __constant84143 when Equals(__constant84143, TargetPlatform.windows):
                {
                    break;
                }
            }
        }
        return new TextSelection(baseOffset: word.start, extentOffset: word.end);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual (double, double) _adjustConstraints(
        double minWidth = 0.0,
        double maxWidth = double.PositiveInfinity
    )
    {
        double availableMaxWidth = Math.Max(0.0, maxWidth - _caretMargin);
        double availableMinWidth = Math.Min(minWidth, availableMaxWidth);
        return (
            forceLine ? availableMaxWidth : availableMinWidth,
            _isMultiline ? availableMaxWidth : double.PositiveInfinity
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _computeTextMetricsIfNeeded()
    {
        var (minWidthLocal, maxWidthLocal) = _adjustConstraints(
            minWidth: constraints.minWidth,
            maxWidth: constraints.maxWidth
        );
        _textPainter.layout(minWidth: minWidthLocal, maxWidth: maxWidthLocal);
    }

    internal virtual void _computeCaretPrototype()
    {
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case var __constant86723 when Equals(__constant86723, TargetPlatform.iOS):
            case var __constant86754 when Equals(__constant86754, TargetPlatform.macOS):
            {
                _caretPrototype = Rect.fromLTWH(0.0, 0.0, cursorWidth, cursorHeight + 2L);
                break;
            }
            case var __constant86869 when Equals(__constant86869, TargetPlatform.android):
            case var __constant86904 when Equals(__constant86904, TargetPlatform.fuchsia):
            case var __constant86939 when Equals(__constant86939, TargetPlatform.linux):
            case var __constant86972 when Equals(__constant86972, TargetPlatform.windows):
            {
                _caretPrototype = Rect.fromLTWH(
                    0.0,
                    EditableLibrary._kCaretHeightOffset,
                    cursorWidth,
                    cursorHeight - (2.0 * EditableLibrary._kCaretHeightOffset)
                );
                break;
            }
        }
    }

    internal virtual Offset _snapToPhysicalPixel(Offset sourceOffset)
    {
        Offset globalOffset = localToGlobal(sourceOffset);
        double pixelMultiple = 1.0 / _devicePixelRatio;
        return new Offset(
            double.IsFinite(globalOffset.dx)
                ? (((globalOffset.dx / pixelMultiple).round() * pixelMultiple) - globalOffset.dx)
                : 0,
            double.IsFinite(globalOffset.dy)
                ? (((globalOffset.dy / pixelMultiple).round() * pixelMultiple) - globalOffset.dy)
                : 0
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        var (minWidthLocal, maxWidthLocal) = _adjustConstraints(
            minWidth: constraints.minWidth,
            maxWidth: constraints.maxWidth
        );
        (
            (Func<TextPainter>)(
                () =>
                {
                    var __cascade = _textIntrinsics;
                    __cascade.setPlaceholderDimensions(
                        layoutInlineChildren(
                            constraints.maxWidth,
                            ChildLayoutHelper.dryLayoutChild,
                            ChildLayoutHelper.getDryBaseline
                        )
                    );
                    __cascade.layout(minWidth: minWidthLocal, maxWidth: maxWidthLocal);
                    return __cascade;
                }
            )
        )();
        double widthLocal = forceLine
            ? constraints.maxWidth
            : constraints.constrainWidth(_textIntrinsics.size.width + _caretMargin);
        return new Size(
            widthLocal,
            constraints.constrainHeight(_preferredHeight(constraints.maxWidth))
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        var (minWidthLocal, maxWidthLocal) = _adjustConstraints(
            minWidth: constraints.minWidth,
            maxWidth: constraints.maxWidth
        );
        (
            (Func<TextPainter>)(
                () =>
                {
                    var __cascade = _textIntrinsics;
                    __cascade.setPlaceholderDimensions(
                        layoutInlineChildren(
                            constraints.maxWidth,
                            ChildLayoutHelper.dryLayoutChild,
                            ChildLayoutHelper.getDryBaseline
                        )
                    );
                    __cascade.layout(minWidth: minWidthLocal, maxWidth: maxWidthLocal);
                    return __cascade;
                }
            )
        )();
        return _textIntrinsics.computeDistanceToActualBaseline(baseline);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraintsLocal = constraints;
        _placeholderDimensions = layoutInlineChildren(
            constraintsLocal.maxWidth,
            ChildLayoutHelper.layoutChild,
            ChildLayoutHelper.getBaseline
        );
        var (minWidthLocal, maxWidthLocal) = _adjustConstraints(
            minWidth: constraintsLocal.minWidth,
            maxWidth: constraintsLocal.maxWidth
        );
        (
            (Func<TextPainter>)(
                () =>
                {
                    var __cascade = _textPainter;
                    __cascade.setPlaceholderDimensions(_placeholderDimensions);
                    __cascade.layout(minWidth: minWidthLocal, maxWidth: maxWidthLocal);
                    return __cascade;
                }
            )
        )();
        positionInlineChildren(_textPainter.inlinePlaceholderBoxes!);
        _computeCaretPrototype();
        double widthLocal = forceLine
            ? constraintsLocal.maxWidth
            : constraintsLocal.constrainWidth(_textPainter.width + _caretMargin);
        DartRuntimePrimitives.Assert(() => (maxLines != 1L) || (_textPainter.maxLines == 1L));
        double preferredHeight = maxLines switch
        {
            null => Math.Max(_textPainter.height, preferredLineHeight * (minLines ?? 0L)),
            1L => _textPainter.height,
            long maxLinesLocal => DorotiUiLibrary.clampDouble(
                _textPainter.height,
                preferredLineHeight * (minLines ?? (maxLinesLocal)),
                preferredLineHeight * (maxLinesLocal)
            ),
        };
        size = new Size(widthLocal, constraintsLocal.constrainHeight(preferredHeight));
        var contentSize = new Size(_textPainter.width + _caretMargin, _textPainter.height);
        var painterConstraints = BoxConstraints.CreateTight(contentSize);
        _foregroundRenderObject?.layout(painterConstraints);
        _backgroundRenderObject?.layout(painterConstraints);
        _maxScrollExtent = _getMaxScrollExtent(contentSize);
        offset.applyViewportDimension(_viewportExtent);
        offset.applyContentDimensions(0.0, _maxScrollExtent);
    }

    internal static Offset _calculateAdjustedCursorOffset(Offset offset, Rect boundingRects)
    {
        double adjustedX = DorotiUiLibrary.clampDouble(
            offset.dx,
            boundingRects.left,
            boundingRects.right
        );
        double adjustedY = DorotiUiLibrary.clampDouble(
            offset.dy,
            boundingRects.top,
            boundingRects.bottom
        );
        return new Offset(adjustedX, adjustedY);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Offset calculateBoundedFloatingCursorOffset(
        Offset rawCursorOffset,
        bool? shouldResetOrigin = null
    )
    {
        Offset deltaPosition = Offset.zero;
        double topBound = -floatingCursorAddedMargin.top;
        double bottomBound =
            Math.Min(size.height, _textPainter.height)
            - preferredLineHeight
            + floatingCursorAddedMargin.bottom;
        double leftBound = -floatingCursorAddedMargin.left;
        double rightBound =
            Math.Min(size.width, _textPainter.width) + floatingCursorAddedMargin.right;
        var boundingRects = Rect.fromLTRB(leftBound, topBound, rightBound, bottomBound);
        if (shouldResetOrigin is not null)
        {
            bool shouldResetOrigin__value92495 = (
                shouldResetOrigin
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            _shouldResetOrigin = (shouldResetOrigin__value92495);
        }
        if (!_shouldResetOrigin)
        {
            return _calculateAdjustedCursorOffset(rawCursorOffset, boundingRects);
        }
        if (_previousOffset is not null)
        {
            deltaPosition =
                rawCursorOffset
                - (
                    _previousOffset
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
        }
        if (_resetOriginOnLeft && (deltaPosition.dx > 0L))
        {
            _relativeOrigin = new Offset(
                rawCursorOffset.dx - boundingRects.left,
                _relativeOrigin.dy
            );
            _resetOriginOnLeft = false;
        }
        else
        {
            if (_resetOriginOnRight && (deltaPosition.dx < 0L))
            {
                _relativeOrigin = new Offset(
                    rawCursorOffset.dx - boundingRects.right,
                    _relativeOrigin.dy
                );
                _resetOriginOnRight = false;
            }
        }
        if (_resetOriginOnTop && (deltaPosition.dy > 0L))
        {
            _relativeOrigin = new Offset(
                _relativeOrigin.dx,
                rawCursorOffset.dy - boundingRects.top
            );
            _resetOriginOnTop = false;
        }
        else
        {
            if (_resetOriginOnBottom && (deltaPosition.dy < 0L))
            {
                _relativeOrigin = new Offset(
                    _relativeOrigin.dx,
                    rawCursorOffset.dy - boundingRects.bottom
                );
                _resetOriginOnBottom = false;
            }
        }
        double currentX = rawCursorOffset.dx - _relativeOrigin.dx;
        double currentY = rawCursorOffset.dy - _relativeOrigin.dy;
        Offset adjustedOffset = _calculateAdjustedCursorOffset(
            new Offset(currentX, currentY),
            boundingRects
        );
        if ((currentX < boundingRects.left) && (deltaPosition.dx < 0L))
        {
            _resetOriginOnLeft = true;
        }
        else
        {
            if ((currentX > boundingRects.right) && (deltaPosition.dx > 0L))
            {
                _resetOriginOnRight = true;
            }
        }
        if ((currentY < boundingRects.top) && (deltaPosition.dy < 0L))
        {
            _resetOriginOnTop = true;
        }
        else
        {
            if ((currentY > boundingRects.bottom) && (deltaPosition.dy > 0L))
            {
                _resetOriginOnBottom = true;
            }
        }
        _previousOffset = rawCursorOffset;
        return adjustedOffset;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void setFloatingCursor(
        FloatingCursorDragState state,
        Offset boundedOffset,
        TextPosition lastTextPosition,
        double? resetLerpValue = null
    )
    {
        if (Equals(state, FloatingCursorDragState.End))
        {
            _relativeOrigin = Offset.zero;
            _previousOffset = null;
            _shouldResetOrigin = true;
            _resetOriginOnBottom = false;
            _resetOriginOnTop = false;
            _resetOriginOnRight = false;
            _resetOriginOnBottom = false;
        }
        _floatingCursorOn = !Equals(state, FloatingCursorDragState.End);
        _resetFloatingCursorAnimationValue = resetLerpValue;
        if (_floatingCursorOn)
        {
            _floatingCursorTextPosition = lastTextPosition;
            double? animationValue = _resetFloatingCursorAnimationValue;
            EdgeInsets sizeAdjustment =
                (animationValue is not null)
                    ? EdgeInsets.lerp(
                        EditableLibrary._kFloatingCursorSizeIncrease,
                        EdgeInsets.zero,
                        (
                            (
                                animationValue
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            )
                        )
                    )!
                    : EditableLibrary._kFloatingCursorSizeIncrease;
            _caretPainter.floatingCursorRect = sizeAdjustment
                .inflateRect(_caretPrototype)
                .shift(boundedOffset);
        }
        else
        {
            _caretPainter.floatingCursorRect = null;
        }
        _caretPainter.showRegularCaret = _resetFloatingCursorAnimationValue is null;
    }

    internal virtual MapEntry<long, Offset> _lineNumberFor(
        TextPosition startPosition,
        List<LineMetrics> metrics
    )
    {
        Offset offsetLocal = _textPainter.getOffsetForCaret(startPosition, Rect.zero);
        foreach (var lineMetrics in metrics)
        {
            if (lineMetrics.baseline > offsetLocal.dy)
            {
                return new MapEntry<long, Offset>(
                    lineMetrics.lineNumber,
                    new Offset(offsetLocal.dx, lineMetrics.baseline)
                );
            }
        }
        DartRuntimePrimitives.Assert(() => startPosition.offset == 0L);
        return new MapEntry<long, Offset>(
            Math.Max(0L, checked(metrics.Count) - 1L),
            new Offset(
                offsetLocal.dx,
                (checked((long)metrics.Count) != 0)
                    ? (metrics.Last().baseline + metrics.Last().descent)
                    : 0.0
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual VerticalCaretMovementRun startVerticalCaretMovement(TextPosition startPosition)
    {
        List<LineMetrics> metrics = _textPainter.computeLineMetrics();
        MapEntry<long, Offset> currentLine = _lineNumberFor(startPosition, metrics);
        return new VerticalCaretMovementRun(
            this,
            metrics,
            startPosition,
            currentLine.key,
            currentLine.value
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _paintContents(PaintingContext context, Offset offset)
    {
        Offset effectiveOffset = offset + _paintOffset;
        if ((selection is not null) && !_floatingCursorOn)
        {
            _updateSelectionExtentsVisibility(effectiveOffset);
        }
        RenderBox? foregroundChild = _foregroundRenderObject;
        RenderBox? backgroundChild = _backgroundRenderObject;
        if (backgroundChild is not null)
        {
            context.paintChild(backgroundChild, offset);
        }
        _textPainter.paint(context.canvas, effectiveOffset);
        paintInlineChildren(context, effectiveOffset);
        if (foregroundChild is not null)
        {
            context.paintChild(foregroundChild, offset);
        }
    }

    internal virtual void _paintHandleLayers(
        PaintingContext context,
        List<TextSelectionPoint> endpoints,
        Offset offset
    )
    {
        Offset startPoint = endpoints[(int)0L].point;
        startPoint = new Offset(
            DorotiUiLibrary.clampDouble(startPoint.dx, 0.0, size.width),
            DorotiUiLibrary.clampDouble(startPoint.dy, 0.0, size.height)
        );
        _leaderLayerHandler.layer = new LeaderLayer(
            link: startHandleLayerLink,
            offset: startPoint + offset
        );
        context.pushLayer(_leaderLayerHandler.layer!, base.paint, Offset.zero);
        if (checked(endpoints.Count) == 2L)
        {
            Offset endPoint = endpoints[(int)1L].point;
            endPoint = new Offset(
                DorotiUiLibrary.clampDouble(endPoint.dx, 0.0, size.width),
                DorotiUiLibrary.clampDouble(endPoint.dy, 0.0, size.height)
            );
            context.pushLayer(
                new LeaderLayer(link: endHandleLayerLink, offset: endPoint + offset),
                base.paint,
                Offset.zero
            );
        }
        else
        {
            if (selection!.isCollapsed)
            {
                context.pushLayer(
                    new LeaderLayer(link: endHandleLayerLink, offset: startPoint + offset),
                    base.paint,
                    Offset.zero
                );
            }
        }
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)child;
        if (Equals(__child, _foregroundRenderObject) || Equals(__child, _backgroundRenderObject))
        {
            return;
        }
        defaultApplyPaintTransform(__child, transform);
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        _computeTextMetricsIfNeeded();
        if (_hasVisualOverflow && (!Equals(clipBehavior, Clip.none)))
        {
            _clipRectLayer.layer = context.pushClipRect(
                needsCompositing,
                offset,
                Offset.zero & size,
                _paintContents,
                clipBehavior: clipBehavior,
                oldLayer: _clipRectLayer.layer
            );
        }
        else
        {
            _clipRectLayer.layer = null;
            _paintContents(context, offset);
        }
        TextSelection? selectionLocal = selection;
        if ((selectionLocal is not null) && selectionLocal.isValid)
        {
            _paintHandleLayers(context, getEndpointsForSelection(selectionLocal), offset);
        }
    }

    public override Rect? describeApproximatePaintClip(RenderObject child)
    {
        switch (clipBehavior)
        {
            case Clip.none:
            {
                return null;
            }
            case Clip.hardEdge:
            case Clip.antiAlias:
            case Clip.antiAliasWithSaveLayer:
            {
                return _hasVisualOverflow ? (Offset.zero & size) : null;
            }
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new ColorProperty("cursorColor", cursorColor));
        properties.add(new DiagnosticsProperty<ValueNotifier<bool>>("showCursor", showCursor));
        properties.add(new IntProperty("maxLines", maxLines));
        properties.add(new IntProperty("minLines", minLines));
        properties.add(new DiagnosticsProperty<bool>("expands", expands, defaultValue: false));
        properties.add(new ColorProperty("selectionColor", selectionColor));
        properties.add(
            new DiagnosticsProperty<TextScaler>(
                "textScaler",
                textScaler,
                defaultValue: TextScaler.noScaling
            )
        );
        properties.add(new DiagnosticsProperty<Locale>("locale", locale, defaultValue: null));
        properties.add(new DiagnosticsProperty<TextSelection>("selection", selection));
        properties.add(new DiagnosticsProperty<ViewportOffset>("offset", offset));
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        return new List<DiagnosticsNode>();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void _scheduleSystemFontsUpdate()
    {
        if (_hasPendingSystemFontsDidChangeCallBack)
        {
            return;
        }
        _hasPendingSystemFontsDidChangeCallBack = true;
        SchedulerBinding.instance.scheduleFrameCallback(
            (timeStamp) =>
            {
                DartRuntimePrimitives.Assert(() => _hasPendingSystemFontsDidChangeCallBack);
                _hasPendingSystemFontsDidChangeCallBack = false;
                DartRuntimePrimitives.Assert(() => attached || (debugDisposed ?? true));
                if (attached)
                {
                    systemFontsDidChange();
                }
            }
        );
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((TextParentData?)child.parentData!)!;
        while (childParentData.previousSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.previousSibling, child));
            child = childParentData.previousSibling!;
            childParentData = ((TextParentData?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((TextParentData?)child.parentData!)!;
        while (childParentData.nextSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.nextSibling, child));
            child = childParentData.nextSibling!;
            childParentData = ((TextParentData?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual long childCount => _childCount;

    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (child is not RenderBox)
            {
                throw new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            $"A {GetType()} expected a child of type {typeof(RenderBox)} but received a "
                                + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."
                        ),
                        new ErrorDescription(
                            "RenderObjects expect specific types of children because they "
                                + "coordinate with their children during layout and paint. For "
                                + "example, a RenderSliver cannot be the child of a RenderBox because "
                                + "a RenderSliver does not understand the RenderBox layout protocol."
                        ),
                        new ErrorSpacer(),
                        new DiagnosticsProperty<object?>(
                            $"The {GetType()} that expected a {typeof(RenderBox)} child was created by",
                            debugCreator,
                            style: DiagnosticsTreeStyle.errorProperty
                        ),
                        new ErrorSpacer(),
                        new DiagnosticsProperty<object?>(
                            $"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type "
                                + "was created by",
                            child.debugCreator,
                            style: DiagnosticsTreeStyle.errorProperty
                        ),
                    }
                );
            }
            return true;
        });
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData = ((TextParentData?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => childParentData.nextSibling is null);
        DartRuntimePrimitives.Assert(() => childParentData.previousSibling is null);
        _childCount += 1L;
        DartRuntimePrimitives.Assert(() => _childCount > 0L);
        if (after is null)
        {
            childParentData.nextSibling = _firstChild;
            if (_firstChild is not null)
            {
                var firstChildParentData = ((TextParentData?)_firstChild!.parentData!)!;
                firstChildParentData.previousSibling = child;
            }
            _firstChild = child;
            _lastChild ??= child;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => _firstChild is not null);
            DartRuntimePrimitives.Assert(() => _lastChild is not null);
            DartRuntimePrimitives.Assert(() =>
                _debugUltimatePreviousSiblingOf(after, equals: _firstChild)
            );
            DartRuntimePrimitives.Assert(() =>
                _debugUltimateNextSiblingOf(after, equals: _lastChild)
            );
            var afterParentData = ((TextParentData?)after.parentData!)!;
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
                var childPreviousSiblingParentData = (
                    (TextParentData?)childParentData.previousSibling!.parentData!
                )!;
                var childNextSiblingParentData = (
                    (TextParentData?)childParentData.nextSibling!.parentData!
                )!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => Equals(afterParentData.nextSibling, child));
            }
        }
    }

    public virtual void insert(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(child, this));
        DartRuntimePrimitives.Assert(() => !Equals(after, this));
        DartRuntimePrimitives.Assert(() => !Equals(child, after));
        DartRuntimePrimitives.Assert(() => !Equals(child, _firstChild));
        DartRuntimePrimitives.Assert(() => !Equals(child, _lastChild));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => child.parentData is TextParentData);
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderBox child)
    {
        insert(child, after: _lastChild);
    }

    public virtual void addAll(List<RenderBox>? children)
    {
        children?.forEach(add);
    }

    public virtual void _removeFromChildList(RenderBox child)
    {
        var childParentData = ((TextParentData?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() =>
            _debugUltimatePreviousSiblingOf(child, equals: _firstChild)
        );
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: _lastChild));
        DartRuntimePrimitives.Assert(() => _childCount >= 0L);
        if (childParentData.previousSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_firstChild, child));
            _firstChild = childParentData.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData = (
                (TextParentData?)childParentData.previousSibling!.parentData!
            )!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if (childParentData.nextSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_lastChild, child));
            _lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = (
                (TextParentData?)childParentData.nextSibling!.parentData!
            )!;
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
            var childParentData = ((TextParentData?)child.parentData!)!;
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
        var childParentData = ((TextParentData?)child.parentData!)!;
        if (Equals(childParentData.previousSibling, after))
        {
            return;
        }
        _removeFromChildList(child);
        _insertIntoChildList(child, after: after);
        markNeedsLayout();
    }

    public virtual RenderBox? firstChild => _firstChild;
    public virtual RenderBox? lastChild => _lastChild;

    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((TextParentData?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((TextParentData?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void setupParentData(RenderObject child)
    {
        var __child = (RenderBox)child;
        if (__child.parentData is not TextParentData)
        {
            __child.parentData = new TextParentData();
        }
    }

    public virtual List<PlaceholderDimensions> layoutInlineChildren(
        double maxWidth,
        Func<RenderBox, BoxConstraints, Size> layoutChild,
        Func<RenderBox, BoxConstraints, TextBaseline, double?> getChildBaseline
    )
    {
        var constraints = new BoxConstraints(maxWidth: maxWidth);
        return new List<PlaceholderDimensions>();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void positionInlineChildren(List<TextBox> boxes)
    {
        RenderBox? child = firstChild;
        foreach (var box in boxes)
        {
            if (child is null)
            {
                DartRuntimePrimitives.Assert(() =>
                {
                    throw new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary(
                                "Invalid number of boxes provided to positionInlineChildren."
                            ),
                            new ErrorDescription(
                                $"The number of boxes ({checked((long)boxes.Count)}) exceeds the number of child render objects ({childCount}). "
                                    + "Each box corresponds to a child, but there are not enough children to position all boxes."
                            ),
                            new ErrorHint(
                                "This error typically occurs when a custom InlineSpan implementation returns a list of boxes "
                                    + "that is longer than the number of inline children. Ensure that the number of boxes returned "
                                    + "by `computeLineMetrics` or similar methods does not exceed the number of children."
                            ),
                            new DiagnosticsProperty<RenderObject>(
                                "The RenderParagraph receiving the boxes",
                                this,
                                style: DiagnosticsTreeStyle.errorProperty
                            ),
                        }
                    );
                });
                return;
            }
            var textParentData = ((TextParentData?)child.parentData!)!;
            textParentData._offset = new Offset(box.left, box.top);
            child = childAfter(child);
        }
        while (child is not null)
        {
            var textParentDataLocal = ((TextParentData?)child.parentData!)!;
            textParentDataLocal._offset = null;
            child = childAfter(child);
        }
    }

    public virtual void defaultApplyPaintTransform(RenderBox child, Matrix4 transform)
    {
        var childParentData = ((TextParentData?)child.parentData!)!;
        Offset? offsetLocal = childParentData.offset;
        if (offsetLocal is null)
        {
            transform.setZero();
        }
        else
        {
            transform.translateByDouble(
                (
                    offsetLocal
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ).dx,
                (
                    offsetLocal
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ).dy,
                0,
                1
            );
        }
    }

    public virtual void paintInlineChildren(PaintingContext context, Offset offset)
    {
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((TextParentData?)child.parentData!)!;
            Offset? childOffset = childParentData.offset;
            if (childOffset is null)
            {
                return;
            }
            context.paintChild(
                child,
                (
                    childOffset
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ) + offset
            );
            child = childAfter(child);
        }
    }

    public virtual bool hitTestInlineChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((TextParentData?)child.parentData!)!;
            Offset? childOffset = childParentData.offset;
            if (childOffset is null)
            {
                return false;
            }
            bool isHit = result.addWithPaintOffset(
                offset: (
                    childOffset
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                position: position,
                hitTest: (result, transformed) => child!.hitTest(result, position: transformed)
            );
            if (isHit)
            {
                return true;
            }
            child = childAfter(child);
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _RenderEditableCustomPaint__editable : RenderBox
{
    internal virtual RenderEditablePainter? _painter { get; set; } = default;

    internal _RenderEditableCustomPaint__editable(RenderEditablePainter? painter = null)
    {
        _painter = painter;
    }

    public override RenderEditable? parent => ((RenderEditable?)base.parent)!;
    public override bool isRepaintBoundary => true;
    public override bool sizedByParent => true;
    public virtual RenderEditablePainter? painter
    {
        get => _painter;
        set
        {
            var newValue = value;
            if (Equals(newValue, painter))
            {
                return;
            }
            RenderEditablePainter? oldPainter = painter;
            _painter = newValue;
            if (newValue?.shouldRepaint(oldPainter) ?? true)
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
        RenderEditable? parentLocal = parent;
        DartRuntimePrimitives.Assert(() => parentLocal is not null);
        RenderEditablePainter? painterLocal = painter;
        if ((painterLocal is not null) && (parentLocal is not null))
        {
            parentLocal._computeTextMetricsIfNeeded();
            painterLocal.paint(context.canvas, size, parentLocal);
        }
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        _painter?.addListener(markNeedsPaint);
    }

    public override void detach()
    {
        _painter?.removeListener(markNeedsPaint);
        base.detach();
    }

    public override Size computeDryLayout(BoxConstraints constraints) => constraints.biggest;
}

public abstract class RenderEditablePainter : ChangeNotifier
{
    public abstract bool shouldRepaint(RenderEditablePainter? oldDelegate);
    public abstract void paint(Canvas canvas, Size size, RenderEditable renderEditable);
}

internal class _TextHighlightPainter__editable : RenderEditablePainter
{
    public virtual Paint highlightPaint { get; private set; } = new Paint();
    internal virtual Color? _highlightColor { get; set; } = default;
    internal virtual TextRange? _highlightedRange { get; set; } = default;
    internal virtual BoxHeightStyle _selectionHeightStyle { get; set; } =
        DorotiUiLibrary.BoxHeightStyle.tight;
    internal virtual BoxWidthStyle _selectionWidthStyle { get; set; } =
        DorotiUiLibrary.BoxWidthStyle.tight;

    internal _TextHighlightPainter__editable(
        TextRange? highlightedRange = null,
        Color? highlightColor = null
    )
    {
        _highlightedRange = highlightedRange;
        _highlightColor = highlightColor;
    }

    public virtual Color? highlightColor
    {
        get => _highlightColor;
        set
        {
            var newValue = value is null ? null : value;
            if (Equals(newValue, _highlightColor))
            {
                return;
            }
            _highlightColor = newValue;
            notifyListeners();
        }
    }
    public virtual TextRange? highlightedRange
    {
        get => _highlightedRange;
        set
        {
            var newValue = value is null ? null : value;
            if (Equals(newValue, _highlightedRange))
            {
                return;
            }
            _highlightedRange = newValue;
            notifyListeners();
        }
    }
    public virtual BoxHeightStyle selectionHeightStyle
    {
        get => _selectionHeightStyle;
        set
        {
            var __value = value;
            if (Equals(_selectionHeightStyle, __value))
            {
                return;
            }
            _selectionHeightStyle = __value;
            notifyListeners();
        }
    }
    public virtual BoxWidthStyle selectionWidthStyle
    {
        get => _selectionWidthStyle;
        set
        {
            var __value = value;
            if (Equals(_selectionWidthStyle, __value))
            {
                return;
            }
            _selectionWidthStyle = __value;
            notifyListeners();
        }
    }

    public override void paint(Canvas canvas, Size size, RenderEditable renderEditable)
    {
        TextRange? range = highlightedRange;
        Color? colorLocal = highlightColor;
        if ((range is null) || (colorLocal is null) || range.isCollapsed)
        {
            return;
        }
        highlightPaint.color = colorLocal;
        TextPainter textPainter = renderEditable._textPainter;
        HashSet<TextBox> boxes = textPainter
            .getBoxesForSelection(
                new TextSelection(baseOffset: range.start, extentOffset: range.end),
                boxHeightStyle: selectionHeightStyle,
                boxWidthStyle: selectionWidthStyle
            )
            .toSet();
        foreach (var box in boxes)
        {
            canvas.drawRect(
                box.toRect()
                    .shift(renderEditable._paintOffset)
                    .intersect(Rect.fromLTWH(0, 0, textPainter.width, textPainter.height)),
                highlightPaint
            );
        }
    }

    public override bool shouldRepaint(RenderEditablePainter? oldDelegate)
    {
        if (DartRuntimePrimitives.Identical(oldDelegate, this))
        {
            return false;
        }
        if (oldDelegate is null)
        {
            return (highlightColor is not null) && (highlightedRange is not null);
        }
        return (oldDelegate is not _TextHighlightPainter__editable)
            || (
                !Equals(
                    ((_TextHighlightPainter__editable)oldDelegate).highlightColor,
                    highlightColor
                )
            )
            || (
                !Equals(
                    ((_TextHighlightPainter__editable)oldDelegate).highlightedRange,
                    highlightedRange
                )
            )
            || (
                !Equals(
                    ((_TextHighlightPainter__editable)oldDelegate).selectionHeightStyle,
                    selectionHeightStyle
                )
            )
            || (
                !Equals(
                    ((_TextHighlightPainter__editable)oldDelegate).selectionWidthStyle,
                    selectionWidthStyle
                )
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _CaretPainter__editable : RenderEditablePainter
{
    internal virtual bool _shouldPaint { get; set; } = true;
    public virtual bool showRegularCaret { get; set; } = false;
    public virtual Paint caretPaint { get; private set; } = new Paint();
    private bool __late_floatingCursorPaint_initialized;
    private Paint __late_floatingCursorPaint = default!;
    public virtual Paint floatingCursorPaint
    {
        get
        {
            if (!__late_floatingCursorPaint_initialized)
            {
                __late_floatingCursorPaint = new Paint();
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

    internal _CaretPainter__editable() { }

    public virtual bool shouldPaint
    {
        get => _shouldPaint;
        set
        {
            var __value = value;
            if (shouldPaint == (__value))
            {
                return;
            }
            _shouldPaint = (__value);
            notifyListeners();
        }
    }
    public virtual Color? caretColor
    {
        get => _caretColor;
        set
        {
            var __value = value is null ? null : value;
            if (caretColor?.value == __value?.value)
            {
                return;
            }
            _caretColor = __value;
            notifyListeners();
        }
    }
    public virtual Radius? cursorRadius
    {
        get => _cursorRadius;
        set
        {
            var __value = value;
            if (Equals(_cursorRadius, __value))
            {
                return;
            }
            _cursorRadius = __value;
            notifyListeners();
        }
    }
    public virtual Offset cursorOffset
    {
        get => _cursorOffset;
        set
        {
            var __value = value;
            if (Equals(_cursorOffset, (__value)))
            {
                return;
            }
            _cursorOffset = (__value);
            notifyListeners();
        }
    }
    public virtual Color? backgroundCursorColor
    {
        get => _backgroundCursorColor;
        set
        {
            var __value = value is null ? null : value;
            if (backgroundCursorColor?.value == __value?.value)
            {
                return;
            }
            _backgroundCursorColor = __value;
            if (showRegularCaret)
            {
                notifyListeners();
            }
        }
    }
    public virtual Rect? floatingCursorRect
    {
        get => _floatingCursorRect;
        set
        {
            var __value = value;
            if (Equals(_floatingCursorRect, __value))
            {
                return;
            }
            _floatingCursorRect = __value;
            notifyListeners();
        }
    }

    public virtual void paintRegularCursor(
        Canvas canvas,
        RenderEditable renderEditable,
        Color caretColor,
        TextPosition textPosition
    )
    {
        Rect integralRect = renderEditable.getLocalRectForCaret(textPosition);
        if (shouldPaint)
        {
            if (floatingCursorRect is not null)
            {
                double distanceSquaredLocal = (
                    (
                        floatingCursorRect
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ).center - integralRect.center
                ).distanceSquared;
                if (
                    distanceSquaredLocal
                    < EditableLibrary._kShortestDistanceSquaredWithFloatingAndRegularCursors
                )
                {
                    return;
                }
            }
            Radius? radius = cursorRadius;
            caretPaint.color = caretColor;
            if (radius is null)
            {
                canvas.drawRect(integralRect, caretPaint);
            }
            else
            {
                var caretRRect = RRect.fromRectAndRadius(
                    integralRect,
                    (
                        radius
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                );
                canvas.drawRRect(caretRRect, caretPaint);
            }
        }
    }

    public override void paint(Canvas canvas, Size size, RenderEditable renderEditable)
    {
        TextSelection? selectionLocal = renderEditable.selection;
        if ((selectionLocal is null) || !selectionLocal.isCollapsed || !selectionLocal.isValid)
        {
            return;
        }
        Rect? floatingCursorRectLocal = floatingCursorRect;
        Color? caretColorLocal =
            (floatingCursorRectLocal is null)
                ? caretColor
                : (showRegularCaret ? backgroundCursorColor : null);
        TextPosition caretTextPosition =
            (floatingCursorRectLocal is null)
                ? selectionLocal.extent
                : renderEditable._floatingCursorTextPosition;
        if (caretColorLocal is not null)
        {
            paintRegularCursor(canvas, renderEditable, caretColorLocal, caretTextPosition);
        }
        Color? floatingCursorColor = caretColor?.withOpacity(0.75);
        if ((floatingCursorRectLocal is null) || (floatingCursorColor is null) || !shouldPaint)
        {
            return;
        }
        canvas.drawRRect(
            RRect.fromRectAndRadius(
                (
                    floatingCursorRectLocal
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                EditableLibrary._kFloatingCursorRadius
            ),
            (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = floatingCursorPaint;
                        __cascade.color = floatingCursorColor;
                        return __cascade;
                    }
                )
            )()
        );
    }

    public override bool shouldRepaint(RenderEditablePainter? oldDelegate)
    {
        if (DartRuntimePrimitives.Identical(this, oldDelegate))
        {
            return false;
        }
        if (oldDelegate is null)
        {
            return shouldPaint;
        }
        return (oldDelegate is not _CaretPainter__editable)
            || (((_CaretPainter__editable)oldDelegate).shouldPaint != shouldPaint)
            || (((_CaretPainter__editable)oldDelegate).showRegularCaret != showRegularCaret)
            || (!Equals(((_CaretPainter__editable)oldDelegate).caretColor, caretColor))
            || (!Equals(((_CaretPainter__editable)oldDelegate).cursorRadius, cursorRadius))
            || (!Equals(((_CaretPainter__editable)oldDelegate).cursorOffset, cursorOffset))
            || (
                !Equals(
                    ((_CaretPainter__editable)oldDelegate).backgroundCursorColor,
                    backgroundCursorColor
                )
            )
            || (
                !Equals(
                    ((_CaretPainter__editable)oldDelegate).floatingCursorRect,
                    floatingCursorRect
                )
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        foreach (RenderEditablePainter painter in painters)
        {
            painter.addListener(listener);
        }
    }

    public override void removeListener(Action listener)
    {
        foreach (RenderEditablePainter painter in painters)
        {
            painter.removeListener(listener);
        }
    }

    public override void paint(Canvas canvas, Size size, RenderEditable renderEditable)
    {
        foreach (RenderEditablePainter painter in painters)
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
        if (
            (oldDelegate is not _CompositeRenderEditablePainter__editable)
            || (
                checked(((_CompositeRenderEditablePainter__editable)oldDelegate).painters.Count)
                != checked((long)painters.Count)
            )
        )
        {
            return true;
        }
        IEnumerator<RenderEditablePainter> oldPainters = (
            (_CompositeRenderEditablePainter__editable)oldDelegate
        ).painters.GetEnumerator();
        IEnumerator<RenderEditablePainter> newPainters = painters.GetEnumerator();
        while (oldPainters.MoveNext() && newPainters.MoveNext())
        {
            if (newPainters.Current.shouldRepaint(oldPainters.Current))
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
