// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/text_painter.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public static partial class Text_painterLibrary
{
    public static double kDefaultFontSize = 14.0;
}

public enum TextOverflow
{
    clip,
    fade,
    ellipsis,
    visible,
}

public class PlaceholderDimensions
{
    public static PlaceholderDimensions empty = new PlaceholderDimensions(
        size: Size.zero,
        alignment: Dart_uiLibrary.PlaceholderAlignment.bottom
    );
    public virtual Size size { get; private set; } = default!;
    public virtual PlaceholderAlignment alignment { get; private set; } = default!;
    public virtual double? baselineOffset { get; private set; }
    public virtual TextBaseline? baseline { get; private set; }

    public PlaceholderDimensions(
        Size size,
        PlaceholderAlignment alignment,
        TextBaseline? baseline = null,
        double? baselineOffset = null
    )
    {
        this.size = size;
        this.alignment = alignment;
        this.baseline = baseline;
        this.baselineOffset = baselineOffset;
    }

    public override bool Equals(object? other)
    {
        var __other = other as PlaceholderDimensions;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return (__other is PlaceholderDimensions)
            && Equals(__other.size, size)
            && Equals(__other.alignment, alignment)
            && Equals(__other.baseline, baseline)
            && (__other.baselineOffset == baselineOffset);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(size, alignment, baseline, baselineOffset);

    public override string ToString()
    {
        return alignment switch
        {
            Dart_uiLibrary.PlaceholderAlignment.top
            or Dart_uiLibrary.PlaceholderAlignment.bottom
            or Dart_uiLibrary.PlaceholderAlignment.middle
            or Dart_uiLibrary.PlaceholderAlignment.aboveBaseline =>
                $"PlaceholderDimensions({size}, {alignment})",
            Dart_uiLibrary.PlaceholderAlignment.belowBaseline =>
                $"PlaceholderDimensions({size}, {alignment})",
            Dart_uiLibrary.PlaceholderAlignment.baseline =>
                $"PlaceholderDimensions({size}, {alignment}({baselineOffset} from top))",
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public enum TextWidthBasis
{
    parent,
    longestLine,
}

public class WordBoundary : TextBoundary
{
    internal virtual InlineSpan _text { get; private set; } = default!;
    internal virtual Paragraph _paragraph { get; private set; } = default!;
    internal static RegExp _regExpSpaceSeparatorOrPunctuation = new RegExp(
        "[\\p{Space_Separator}\\p{Punctuation}]",
        unicode: true
    );
    private bool __late_moveByWordBoundary_initialized;
    private TextBoundary __late_moveByWordBoundary = default!;
    public virtual TextBoundary moveByWordBoundary
    {
        get
        {
            if (!__late_moveByWordBoundary_initialized)
            {
                __late_moveByWordBoundary = new _UntilTextBoundary__text_painter(
                    this,
                    _skipSpacesAndPunctuations
                );
                __late_moveByWordBoundary_initialized = true;
            }
            return __late_moveByWordBoundary;
        }
    }

    public WordBoundary(InlineSpan _text, Paragraph _paragraph)
    {
        this._text = _text;
        this._paragraph = _paragraph;
    }

    public override TextRange getTextBoundaryAt(long position) =>
        _paragraph.getWordBoundary(new TextPosition(offset: Math.Max(position, 0L)));

    internal static long _codePointFromSurrogates(long highSurrogate, long lowSurrogate)
    {
        DartRuntimePrimitives.Assert(() => TextPainter.isHighSurrogate(highSurrogate));
        DartRuntimePrimitives.Assert(() => TextPainter.isLowSurrogate(lowSurrogate));
        long @base = 65536L - (55296L << (int)10L) - 56320L;
        return (highSurrogate << (int)10L) + lowSurrogate + @base;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual long? _codePointAt(long index)
    {
        long? codeUnitAtIndex = _text.codeUnitAt(index);
        if (codeUnitAtIndex is null)
        {
            return null;
        }
        return (
            (
                codeUnitAtIndex
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ) & 64512L
        ) switch
        {
            55296L => _codePointFromSurrogates(
                (
                    (
                        codeUnitAtIndex
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                ),
                (
                    _text.codeUnitAt(index + 1L)
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            ),
            56320L => _codePointFromSurrogates(
                (
                    _text.codeUnitAt(index - 1L)
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                (
                    (
                        codeUnitAtIndex
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                )
            ),
            _ => (
                codeUnitAtIndex
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static bool _isNewline(long codePoint)
    {
        return codePoint switch
        {
            10L or 133L or 11L or 12L or 8232L => true,
            8233L => true,
            _ => false,
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual bool _skipSpacesAndPunctuations(long offset, bool forward)
    {
        long? innerCodePoint = _codePointAt(forward ? (offset - 1L) : offset);
        long? outerCodeUnit = _text.codeUnitAt(forward ? offset : (offset - 1L));
        bool hardBreakRulesApply =
            (innerCodePoint is null)
            || (outerCodeUnit is null)
            || _isNewline(
                (
                    (
                        innerCodePoint
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                )
            )
            || _isNewline(
                (
                    (
                        outerCodeUnit
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                )
            );
        return hardBreakRulesApply
            || !_regExpSpaceSeparatorOrPunctuation.hasMatch(
                char.ConvertFromUtf32(
                    checked(
                        (int)(
                            innerCodePoint
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        )
                    )
                )
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _UntilTextBoundary__text_painter : TextBoundary
{
    internal virtual Func<long, bool, bool> _predicate { get; private set; } = default!;
    internal virtual TextBoundary _textBoundary { get; private set; } = default!;

    internal _UntilTextBoundary__text_painter(
        TextBoundary _textBoundary,
        Func<long, bool, bool> _predicate
    )
    {
        this._textBoundary = _textBoundary;
        this._predicate = _predicate;
    }

    public override long? getLeadingTextBoundaryAt(long position)
    {
        if (position < 0L)
        {
            return null;
        }
        long? offset = _textBoundary.getLeadingTextBoundaryAt(position);
        return (
            (offset is null)
            || _predicate(
                (
                    offset
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                false
            )
        )
            ? offset
            : getLeadingTextBoundaryAt(
                (
                    offset
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ) - 1L
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override long? getTrailingTextBoundaryAt(long position)
    {
        long? offset = _textBoundary.getTrailingTextBoundaryAt(Math.Max(position, 0L));
        return (
            (offset is null)
            || _predicate(
                (
                    offset
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                true
            )
        )
            ? offset
            : getTrailingTextBoundaryAt(
                (
                    (
                        offset
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                )
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _TextLayout__text_painter
{
    public virtual TextDirection writingDirection { get; private set; } = default!;
    internal virtual TextPainter _painter { get; private set; } = default!;
    internal virtual Paragraph _paragraph { get; set; } = default!;
    internal static RegExp _regExpSpaceSeparators = new RegExp(
        "\\p{Space_Separator}",
        unicode: true
    );
    private bool __late__endOfTextCaretMetrics_initialized;
    private _LineCaretMetrics__text_painter __late__endOfTextCaretMetrics = default!;
    internal virtual _LineCaretMetrics__text_painter _endOfTextCaretMetrics
    {
        get
        {
            if (!__late__endOfTextCaretMetrics_initialized)
            {
                __late__endOfTextCaretMetrics = _computeEndOfTextCaretAnchorOffset();
                __late__endOfTextCaretMetrics_initialized = true;
            }
            return __late__endOfTextCaretMetrics;
        }
    }

    internal _TextLayout__text_painter(
        Paragraph _paragraph,
        TextDirection writingDirection,
        TextPainter _painter
    )
    {
        this._paragraph = _paragraph;
        this.writingDirection = writingDirection;
        this._painter = _painter;
    }

    public virtual bool debugDisposed => _paragraph.debugDisposed;
    public virtual double width => _paragraph.width;
    public virtual double height => _paragraph.height;
    public virtual double minIntrinsicLineExtent => _paragraph.minIntrinsicWidth;
    public virtual double maxIntrinsicLineExtent => _paragraph.maxIntrinsicWidth;
    public virtual double longestLine => _paragraph.longestLine;

    public virtual double getDistanceToBaseline(TextBaseline baseline)
    {
        return baseline switch
        {
            TextBaseline.alphabetic => _paragraph.alphabeticBaseline,
            TextBaseline.ideographic => _paragraph.ideographicBaseline,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual _LineCaretMetrics__text_painter _computeEndOfTextCaretAnchorOffset()
    {
        string rawString = _painter.plainText;
        long lastLineIndex = _paragraph.numberOfLines - 1L;
        DartRuntimePrimitives.Assert(() => lastLineIndex >= 0L);
        LineMetrics lineMetrics = _paragraph.getLineMetricsAt(lastLineIndex)!;
        string lastCodeUnit = rawString[(int)(rawString.Length - 1L)].ToString();
        bool hasTrailingSpaces = lastCodeUnit.codeUnitAt(0L) switch
        {
            9L => true,
            160L or 8199L => false,
            8239L => false,
            _ => _regExpSpaceSeparators.hasMatch(lastCodeUnit),
        };
        double baselineLocal = lineMetrics.baseline;
        double dx = default!;
        double heightLocal = default!;
        GlyphInfo? lastGlyph = _paragraph.getGlyphInfoAt(rawString.Length - 1L);
        if (hasTrailingSpaces && (lastGlyph is not null))
        {
            Rect glyphBounds = lastGlyph.graphemeClusterLayoutBounds;
            DartRuntimePrimitives.Assert(() => !glyphBounds.isEmpty);
            dx = writingDirection switch
            {
                TextDirection.ltr => glyphBounds.right,
                TextDirection.rtl => glyphBounds.left,
                _ => throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
            };
            heightLocal = glyphBounds.height;
        }
        else
        {
            dx = writingDirection switch
            {
                TextDirection.ltr => lineMetrics.left + lineMetrics.width,
                TextDirection.rtl => lineMetrics.left,
                _ => throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
            };
            heightLocal = lineMetrics.height;
        }
        return new _LineCaretMetrics__text_painter(
            offset: new Offset(dx, baselineLocal),
            writingDirection: writingDirection,
            height: heightLocal
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual double _contentWidthFor(
        double minWidth,
        double maxWidth,
        TextWidthBasis widthBasis
    )
    {
        return widthBasis switch
        {
            TextWidthBasis.longestLine => Dart_uiLibrary.clampDouble(
                longestLine,
                minWidth,
                maxWidth
            ),
            TextWidthBasis.parent => Dart_uiLibrary.clampDouble(
                maxIntrinsicLineExtent,
                minWidth,
                maxWidth
            ),
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _TextPainterLayoutCacheWithOffset__text_painter
{
    public virtual _TextLayout__text_painter layout { get; private set; } = default!;
    public virtual double layoutMaxWidth { get; private set; } = default!;
    public virtual double contentWidth { get; set; } = default!;
    public virtual double textAlignment { get; private set; } = default!;
    internal virtual List<TextBox>? _cachedInlinePlaceholderBoxes { get; set; } = default;
    internal virtual List<LineMetrics>? _cachedLineMetrics { get; set; } = default;
    internal virtual long? _previousCaretPositionKey { get; set; } = default;

    internal _TextPainterLayoutCacheWithOffset__text_painter(
        _TextLayout__text_painter layout,
        double textAlignment,
        double layoutMaxWidth,
        double contentWidth
    )
    {
        this.layout = layout;
        this.textAlignment = textAlignment;
        this.layoutMaxWidth = layoutMaxWidth;
        this.contentWidth = contentWidth;
        System.Diagnostics.Debug.Assert((textAlignment >= 0.0) && (textAlignment <= 1.0));
        System.Diagnostics.Debug.Assert(!double.IsNaN(layoutMaxWidth));
        System.Diagnostics.Debug.Assert(!double.IsNaN(contentWidth));
    }

    public virtual Offset paintOffset
    {
        get
        {
            if (textAlignment == 0L)
            {
                return Offset.zero;
            }
            if (!double.IsFinite(paragraph.width))
            {
                return new Offset(double.PositiveInfinity, 0.0);
            }
            double dx = textAlignment * (contentWidth - paragraph.width);
            DartRuntimePrimitives.Assert(() => !double.IsNaN(dx));
            return new Offset(dx, 0);
        }
    }
    public virtual Paragraph paragraph => layout._paragraph;

    internal virtual bool _resizeToFit(double minWidth, double maxWidth, TextWidthBasis widthBasis)
    {
        DartRuntimePrimitives.Assert(() => double.IsFinite(layout.maxIntrinsicLineExtent));
        DartRuntimePrimitives.Assert(() => minWidth <= maxWidth);
        if ((maxWidth == contentWidth) && (minWidth == contentWidth))
        {
            contentWidth = layout._contentWidthFor(minWidth, maxWidth, widthBasis);
            return true;
        }
        if (
            !double.IsFinite(paintOffset.dx)
            && !double.IsFinite(paragraph.width)
            && double.IsFinite(minWidth)
        )
        {
            DartRuntimePrimitives.Assert(() => paintOffset.dx == double.PositiveInfinity);
            DartRuntimePrimitives.Assert(() => paragraph.width == double.PositiveInfinity);
            return false;
        }
        double maxIntrinsicWidthLocal = paragraph.maxIntrinsicWidth;
        bool skipLineBreaking =
            (maxWidth == layoutMaxWidth)
            || (
                (
                    paragraph.width - maxIntrinsicWidthLocal
                    > -Foundation.ConstantsLibrary.precisionErrorTolerance
                )
                && (
                    maxWidth - maxIntrinsicWidthLocal
                    > -Foundation.ConstantsLibrary.precisionErrorTolerance
                )
            );
        if (skipLineBreaking)
        {
            contentWidth = layout._contentWidthFor(minWidth, maxWidth, widthBasis);
            return true;
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual List<TextBox> inlinePlaceholderBoxes =>
        _cachedInlinePlaceholderBoxes ??= paragraph.getBoxesForPlaceholders();
    public virtual List<LineMetrics> lineMetrics =>
        _cachedLineMetrics ??= paragraph.computeLineMetrics();
}

internal class _LineCaretMetrics__text_painter
{
    public virtual Offset offset { get; private set; } = default!;
    public virtual TextDirection writingDirection { get; private set; } = default!;
    public virtual double height { get; private set; } = default!;

    internal _LineCaretMetrics__text_painter(
        Offset offset,
        TextDirection writingDirection,
        double height
    )
    {
        this.offset = offset;
        this.writingDirection = writingDirection;
        this.height = height;
    }

    public virtual _LineCaretMetrics__text_painter shift(Offset offset)
    {
        return Equals(offset, Offset.zero)
            ? this
            : new _LineCaretMetrics__text_painter(
                offset: offset + this.offset,
                writingDirection: writingDirection,
                height: height
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class TextPainter
{
    internal virtual bool _debugNeedsRelayout { get; set; } = true;
    internal virtual _TextPainterLayoutCacheWithOffset__text_painter? _layoutCache { get; set; } =
        default;
    internal virtual bool _rebuildParagraphForPaint { get; set; } = true;
    internal virtual System.Diagnostics.StackTrace? _debugMarkNeedsLayoutCallStack { get; set; } =
        default;
    internal virtual InlineSpan? _text { get; set; } = default;
    internal virtual string? _cachedPlainText { get; set; } = default;
    internal virtual TextAlign _textAlign { get; set; } = default!;
    internal virtual TextDirection? _textDirection { get; set; } = default;
    internal virtual TextScaler _textScaler { get; set; } = default!;
    internal virtual string? _ellipsis { get; set; } = default;
    internal virtual Locale? _locale { get; set; } = default;
    internal virtual long? _maxLines { get; set; } = default;
    internal virtual StrutStyle? _strutStyle { get; set; } = default;
    internal virtual TextWidthBasis _textWidthBasis { get; set; } = default!;
    internal virtual TextHeightBehavior? _textHeightBehavior { get; set; } = default;
    internal virtual List<PlaceholderDimensions>? _placeholderDimensions { get; set; } = default;
    internal virtual Paragraph? _layoutTemplate { get; set; } = default;
    public virtual bool debugPaintTextLayoutBoxes { get; set; } = false;
    internal virtual _LineCaretMetrics__text_painter _caretMetrics { get; set; } = default!;
    internal virtual bool _disposed { get; set; } = false;

    public TextPainter(
        InlineSpan? text = null,
        TextAlign textAlign = TextAlign.start,
        TextDirection? textDirection = null,
        double textScaleFactor = 1.0,
        TextScaler? textScaler = null,
        long? maxLines = null,
        string? ellipsis = null,
        Locale? locale = null,
        StrutStyle? strutStyle = null,
        TextWidthBasis textWidthBasis = TextWidthBasis.parent,
        TextHeightBehavior? textHeightBehavior = null
    )
    {
        TextScaler __textScaler = textScaler ?? new _UnspecifiedTextScaler__text_painter();
        _text = text;
        _textAlign = textAlign;
        _textDirection = textDirection;
        _textScaler = textScaler is null or _UnspecifiedTextScaler__text_painter
            ? TextScaler.CreateLinear(textScaleFactor)
            : textScaler;
        _maxLines = maxLines;
        _ellipsis = ellipsis;
        _locale = locale;
        _strutStyle = strutStyle;
        _textWidthBasis = textWidthBasis;
        _textHeightBehavior = textHeightBehavior;
        System.Diagnostics.Debug.Assert((text is null) || text.debugAssertIsValid());
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
            textScaleFactor == 1.0 || textScaler is null or _UnspecifiedTextScaler__text_painter
        );
    }

    public static double computeWidth(
        InlineSpan text,
        TextDirection textDirection,
        TextAlign textAlign = TextAlign.start,
        double textScaleFactor = 1.0,
        TextScaler? textScaler = null,
        long? maxLines = null,
        string? ellipsis = null,
        Locale? locale = null,
        StrutStyle? strutStyle = null,
        TextWidthBasis textWidthBasis = TextWidthBasis.parent,
        TextHeightBehavior? textHeightBehavior = null,
        double minWidth = 0.0,
        double maxWidth = double.PositiveInfinity
    )
    {
        DartRuntimePrimitives.Assert(() =>
            (textScaleFactor == 1.0)
            || DartRuntimePrimitives.Identical(textScaler, TextScaler.noScaling)
        );
        var painter = (
            (Func<TextPainter>)(
                () =>
                {
                    var __cascade = new TextPainter(
                        text: text,
                        textAlign: textAlign,
                        textDirection: (textDirection),
                        textScaler: Equals(textScaler, TextScaler.noScaling)
                            ? TextScaler.CreateLinear(textScaleFactor)
                            : textScaler,
                        maxLines: maxLines,
                        ellipsis: ellipsis,
                        locale: locale,
                        strutStyle: strutStyle,
                        textWidthBasis: textWidthBasis,
                        textHeightBehavior: textHeightBehavior
                    );
                    __cascade.layout(minWidth: minWidth, maxWidth: maxWidth);
                    return __cascade;
                }
            )
        )();
        try
        {
            return painter.width;
        }
        finally
        {
            painter.dispose();
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static double computeMaxIntrinsicWidth(
        InlineSpan text,
        TextDirection textDirection,
        TextAlign textAlign = TextAlign.start,
        double textScaleFactor = 1.0,
        TextScaler? textScaler = null,
        long? maxLines = null,
        string? ellipsis = null,
        Locale? locale = null,
        StrutStyle? strutStyle = null,
        TextWidthBasis textWidthBasis = TextWidthBasis.parent,
        TextHeightBehavior? textHeightBehavior = null,
        double minWidth = 0.0,
        double maxWidth = double.PositiveInfinity
    )
    {
        DartRuntimePrimitives.Assert(() =>
            (textScaleFactor == 1.0)
            || DartRuntimePrimitives.Identical(textScaler, TextScaler.noScaling)
        );
        var painter = (
            (Func<TextPainter>)(
                () =>
                {
                    var __cascade = new TextPainter(
                        text: text,
                        textAlign: textAlign,
                        textDirection: (textDirection),
                        textScaler: Equals(textScaler, TextScaler.noScaling)
                            ? TextScaler.CreateLinear(textScaleFactor)
                            : textScaler,
                        maxLines: maxLines,
                        ellipsis: ellipsis,
                        locale: locale,
                        strutStyle: strutStyle,
                        textWidthBasis: textWidthBasis,
                        textHeightBehavior: textHeightBehavior
                    );
                    __cascade.layout(minWidth: minWidth, maxWidth: maxWidth);
                    return __cascade;
                }
            )
        )();
        try
        {
            return painter.maxIntrinsicWidth;
        }
        finally
        {
            painter.dispose();
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual bool _debugAssertTextLayoutIsValid
    {
        get
        {
            DartRuntimePrimitives.Assert(() => !debugDisposed);
            if (_layoutCache is null)
            {
                throw new FlutterError(
                    new List<DiagnosticsNode> { new ErrorSummary("Text layout not available") }
                );
            }
            return true;
        }
    }

    public virtual void markNeedsLayout()
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (_layoutCache is not null)
            {
                _debugMarkNeedsLayoutCallStack ??= new System.Diagnostics.StackTrace(true);
            }
            return true;
        });
        _layoutCache?.paragraph.dispose();
        _layoutCache = null;
    }

    public virtual InlineSpan? text
    {
        get => _text;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is null) || __value.debugAssertIsValid());
            if (Equals(_text, __value))
            {
                return;
            }
            if (!Equals(_text?.style, __value?.style))
            {
                _layoutTemplate?.dispose();
                _layoutTemplate = null;
            }
            RenderComparison comparison =
                (__value is null)
                    ? RenderComparison.layout
                    : (_text?.compareTo(__value) ?? RenderComparison.layout);
            _text = __value;
            _cachedPlainText = null;
            if (
                FoundationRuntimePorts.EnumIndex(comparison)
                >= FoundationRuntimePorts.EnumIndex(RenderComparison.layout)
            )
            {
                markNeedsLayout();
            }
            else
            {
                if (
                    FoundationRuntimePorts.EnumIndex(comparison)
                    >= FoundationRuntimePorts.EnumIndex(RenderComparison.paint)
                )
                {
                    _rebuildParagraphForPaint = true;
                }
            }
        }
    }
    public virtual string plainText
    {
        get
        {
            _cachedPlainText ??= _text?.toPlainText(includeSemanticsLabels: false);
            return _cachedPlainText ?? "";
        }
    }
    public virtual TextAlign textAlign
    {
        get => _textAlign;
        set
        {
            var __value = value;
            if (Equals(_textAlign, (__value)))
            {
                return;
            }
            _textAlign = (__value);
            markNeedsLayout();
        }
    }
    public virtual TextDirection? textDirection
    {
        get => _textDirection;
        set
        {
            var __value = value;
            if (Equals(_textDirection, __value))
            {
                return;
            }
            _textDirection = __value;
            markNeedsLayout();
            _layoutTemplate?.dispose();
            _layoutTemplate = null;
        }
    }
    public virtual double textScaleFactor
    {
        get => textScaler.textScaleFactor;
        set
        {
            var __value = value;
            textScaler = TextScaler.CreateLinear((__value));
        }
    }
    public virtual TextScaler textScaler
    {
        get => _textScaler;
        set
        {
            var __value = value;
            if (Equals(__value, _textScaler))
            {
                return;
            }
            _textScaler = __value;
            markNeedsLayout();
            _layoutTemplate?.dispose();
            _layoutTemplate = null;
        }
    }
    public virtual string? ellipsis
    {
        get => _ellipsis;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is null) || (__value.Length != 0));
            if (_ellipsis == __value)
            {
                return;
            }
            _ellipsis = __value;
            markNeedsLayout();
        }
    }
    public virtual Locale? locale
    {
        get => _locale;
        set
        {
            var __value = value;
            if (Equals(_locale, __value))
            {
                return;
            }
            _locale = __value;
            markNeedsLayout();
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
            if (_maxLines == __value)
            {
                return;
            }
            _maxLines = __value;
            markNeedsLayout();
        }
    }
    public virtual StrutStyle? strutStyle
    {
        get => _strutStyle;
        set
        {
            var __value = value;
            if (Equals(_strutStyle, __value))
            {
                return;
            }
            _strutStyle = __value;
            markNeedsLayout();
        }
    }
    public virtual TextWidthBasis textWidthBasis
    {
        get => _textWidthBasis;
        set
        {
            var __value = value;
            if (Equals(_textWidthBasis, (__value)))
            {
                return;
            }
            DartRuntimePrimitives.Assert(() =>
            {
                return _debugNeedsRelayout = true;
            });
            _textWidthBasis = (__value);
        }
    }
    public virtual TextHeightBehavior? textHeightBehavior
    {
        get => _textHeightBehavior;
        set
        {
            var __value = value is null ? null : value;
            if (Equals(_textHeightBehavior, __value))
            {
                return;
            }
            _textHeightBehavior = __value;
            markNeedsLayout();
        }
    }
    public virtual List<TextBox>? inlinePlaceholderBoxes
    {
        get
        {
            _TextPainterLayoutCacheWithOffset__text_painter? layout = _layoutCache;
            if (layout is null)
            {
                return null;
            }
            Offset offset = layout.paintOffset;
            if (!double.IsFinite(offset.dx) || !double.IsFinite(offset.dy))
            {
                return new List<TextBox>();
            }
            List<TextBox> rawBoxes = layout.inlinePlaceholderBoxes;
            if (Equals(offset, Offset.zero))
            {
                return rawBoxes;
            }
            return rawBoxes.map((box) => _shiftTextBox(box, offset)).ToList();
        }
    }

    public virtual void setPlaceholderDimensions(List<PlaceholderDimensions>? value)
    {
        if (
            (value is null)
            || (checked((long)value.Count) == 0)
            || CollectionsLibrary.listEquals(value, _placeholderDimensions)
        )
        {
            return;
        }
        DartRuntimePrimitives.Assert(() =>
        {
            var placeholderCount = 0L;
            text!.visitChildren(
                (span) =>
                {
                    if (span is PlaceholderSpan)
                    {
                        placeholderCount += 1L;
                    }
                    return checked(value.Count) >= placeholderCount;
                }
            );
            return placeholderCount == checked(value.Count);
        });
        _placeholderDimensions = value;
        markNeedsLayout();
    }

    internal virtual ParagraphStyle _createParagraphStyle(TextAlign? textAlignOverride = null)
    {
        DartRuntimePrimitives.Assert(() => textDirection is not null);
        TextStyle baseStyle = _text?.style ?? new TextStyle();
        return baseStyle.getParagraphStyle(
            textAlign: textAlignOverride ?? textAlign,
            textDirection: textDirection,
            textScaler: textScaler,
            maxLines: _maxLines,
            textHeightBehavior: _textHeightBehavior,
            ellipsis: _ellipsis,
            locale: _locale,
            strutStyle: _strutStyle
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Paragraph _createLayoutTemplate()
    {
        var builder = new ParagraphBuilder(_createParagraphStyle(TextAlign.left));
        Ui.TextStyle? textStyle = text?.style?.getTextStyle(textScaler: textScaler);
        if (textStyle is not null)
        {
            builder.pushStyle(textStyle);
        }
        builder.addText(" ");
        return (
            (Func<Paragraph>)(
                () =>
                {
                    var __cascade = builder.build();
                    __cascade.layout(new ParagraphConstraints(width: double.PositiveInfinity));
                    return __cascade;
                }
            )
        )();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Paragraph _getOrCreateLayoutTemplate() =>
        _layoutTemplate ??= _createLayoutTemplate();

    public virtual double preferredLineHeight => _getOrCreateLayoutTemplate().height;
    public virtual double minIntrinsicWidth
    {
        get
        {
            DartRuntimePrimitives.Assert(() => _debugAssertTextLayoutIsValid);
            return _layoutCache!.layout.minIntrinsicLineExtent;
        }
    }
    public virtual double maxIntrinsicWidth
    {
        get
        {
            DartRuntimePrimitives.Assert(() => _debugAssertTextLayoutIsValid);
            return _layoutCache!.layout.maxIntrinsicLineExtent;
        }
    }
    public virtual double width
    {
        get
        {
            DartRuntimePrimitives.Assert(() => _debugAssertTextLayoutIsValid);
            DartRuntimePrimitives.Assert(() => !_debugNeedsRelayout);
            return _layoutCache!.contentWidth;
        }
    }
    public virtual double height
    {
        get
        {
            DartRuntimePrimitives.Assert(() => _debugAssertTextLayoutIsValid);
            return _layoutCache!.layout.height;
        }
    }
    public virtual Size size
    {
        get
        {
            DartRuntimePrimitives.Assert(() => _debugAssertTextLayoutIsValid);
            DartRuntimePrimitives.Assert(() => !_debugNeedsRelayout);
            return new Size(width, height);
        }
    }

    public virtual double computeDistanceToActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertTextLayoutIsValid);
        return _layoutCache!.layout.getDistanceToBaseline(baseline);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool didExceedMaxLines
    {
        get
        {
            DartRuntimePrimitives.Assert(() => _debugAssertTextLayoutIsValid);
            return _layoutCache!.paragraph.didExceedMaxLines;
        }
    }

    internal virtual Paragraph _createParagraph(InlineSpan text)
    {
        var builder = new ParagraphBuilder(_createParagraphStyle());
        text.build(builder, textScaler: textScaler, dimensions: _placeholderDimensions);
        DartRuntimePrimitives.Assert(() =>
        {
            _debugMarkNeedsLayoutCallStack = null;
            return true;
        });
        _rebuildParagraphForPaint = false;
        return builder.build();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void layout(double minWidth = 0.0, double maxWidth = double.PositiveInfinity)
    {
        DartRuntimePrimitives.Assert(() => !double.IsNaN(maxWidth));
        DartRuntimePrimitives.Assert(() => !double.IsNaN(minWidth));
        DartRuntimePrimitives.Assert(() =>
        {
            _debugNeedsRelayout = false;
            return true;
        });
        _TextPainterLayoutCacheWithOffset__text_painter? cachedLayout = _layoutCache;
        if (
            (cachedLayout is not null)
            && cachedLayout._resizeToFit(minWidth, maxWidth, textWidthBasis)
        )
        {
            return;
        }
        InlineSpan? textLocal = text;
        if (textLocal is null)
        {
            throw new InvalidOperationException(
                "TextPainter.text must be set to a non-null value before using the TextPainter."
            );
        }
        TextDirection? textDirectionLocal = textDirection;
        if (textDirectionLocal is null)
        {
            throw new InvalidOperationException(
                "TextPainter.textDirection must be set to a non-null value before using the TextPainter."
            );
        }
        double paintOffsetAlignment = _computePaintOffsetFraction(
            textAlign,
            (
                (
                    textDirectionLocal
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            )
        );
        bool adjustMaxWidth = !double.IsFinite(maxWidth) && (paintOffsetAlignment != 0L);
        double? adjustedMaxWidth = !adjustMaxWidth
            ? maxWidth
            : cachedLayout?.layout.maxIntrinsicLineExtent;
        double layoutMaxWidth = adjustedMaxWidth ?? maxWidth;
        Paragraph paragraphLocal = (
            (Func<Paragraph>)(
                () =>
                {
                    var __cascade = cachedLayout?.paragraph ?? _createParagraph(textLocal);
                    __cascade.layout(new ParagraphConstraints(width: layoutMaxWidth));
                    return __cascade;
                }
            )
        )();
        var layoutLocal = new _TextLayout__text_painter(
            paragraphLocal,
            (
                textDirectionLocal
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            this
        );
        double contentWidth = layoutLocal._contentWidthFor(minWidth, maxWidth, textWidthBasis);
        _TextPainterLayoutCacheWithOffset__text_painter newLayoutCache = default!;
        if ((adjustedMaxWidth is null) && double.IsFinite(minWidth))
        {
            DartRuntimePrimitives.Assert(() => double.IsInfinity(maxWidth));
            double newInputWidth = layoutLocal.maxIntrinsicLineExtent;
            paragraphLocal.layout(new ParagraphConstraints(width: newInputWidth));
            newLayoutCache = new _TextPainterLayoutCacheWithOffset__text_painter(
                layoutLocal,
                paintOffsetAlignment,
                newInputWidth,
                contentWidth
            );
        }
        else
        {
            newLayoutCache = new _TextPainterLayoutCacheWithOffset__text_painter(
                layoutLocal,
                paintOffsetAlignment,
                layoutMaxWidth,
                contentWidth
            );
        }
        _layoutCache = newLayoutCache;
    }

    public virtual void paint(Canvas canvas, Offset offset)
    {
        _TextPainterLayoutCacheWithOffset__text_painter? layoutCache = _layoutCache;
        if (layoutCache is null)
        {
            throw new InvalidOperationException(
                "TextPainter.paint called when text geometry was not yet calculated.\n"
                    + "Please call layout() before paint() to position the text before painting it."
            );
        }
        if (
            !double.IsFinite(layoutCache.paintOffset.dx)
            || !double.IsFinite(layoutCache.paintOffset.dy)
        )
        {
            return;
        }
        if (_rebuildParagraphForPaint)
        {
            Size? debugSize = default!;
            DartRuntimePrimitives.Assert(() =>
            {
                debugSize = size;
                return true;
            });
            Paragraph paragraphLocal = layoutCache.paragraph;
            DartRuntimePrimitives.Assert(() => !double.IsNaN(layoutCache.layoutMaxWidth));
            layoutCache.layout._paragraph = (
                (Func<Paragraph>)(
                    () =>
                    {
                        var __cascade = _createParagraph(text!);
                        __cascade.layout(
                            new ParagraphConstraints(width: layoutCache.layoutMaxWidth)
                        );
                        return __cascade;
                    }
                )
            )();
            DartRuntimePrimitives.Assert(() =>
                paragraphLocal.width == layoutCache.layout._paragraph.width
            );
            paragraphLocal.dispose();
            DartRuntimePrimitives.Assert(() => Equals(debugSize, size));
        }
        DartRuntimePrimitives.Assert(() => !_rebuildParagraphForPaint);
        DartRuntimePrimitives.Assert(() =>
            !debugPaintTextLayoutBoxes
            || _debugPaintCharacterLayoutBoxes(canvas, layoutCache, offset)
        );
        canvas.drawParagraph(layoutCache.paragraph, offset + layoutCache.paintOffset);
    }

    internal virtual bool _debugPaintCharacterLayoutBoxes(
        Canvas canvas,
        _TextPainterLayoutCacheWithOffset__text_painter layout,
        Offset offset
    )
    {
        var paint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.style = PaintingStyle.stroke;
                    __cascade.strokeWidth = 1.0;
                    __cascade.color = new Color(4278255615L);
                    return __cascade;
                }
            )
        )();
        List<TextBox> textBoxes = getBoxesForSelection(
            new TextSelection(baseOffset: 0L, extentOffset: plainText.Length)
        );
        foreach (var textBox in textBoxes)
        {
            canvas.drawRect(textBox.toRect().shift(offset), paint);
        }
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static bool _isUTF16(long value)
    {
        return (value >= 0L) && (value <= 1048575L);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static bool isHighSurrogate(long value)
    {
        DartRuntimePrimitives.Assert(() => _isUTF16(((value))));
        return ((value) & 64512L) == 55296L;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static bool isLowSurrogate(long value)
    {
        DartRuntimePrimitives.Assert(() => _isUTF16(((value))));
        return ((value) & 64512L) == 56320L;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual long? getOffsetAfter(long offset)
    {
        long? nextCodeUnit = _text!.codeUnitAt(offset);
        if (nextCodeUnit is null)
        {
            return null;
        }
        return isHighSurrogate(
            (
                (
                    nextCodeUnit
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            )
        )
            ? (offset + 2L)
            : (offset + 1L);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual long? getOffsetBefore(long offset)
    {
        long? prevCodeUnit = _text!.codeUnitAt(offset - 1L);
        if (prevCodeUnit is null)
        {
            return null;
        }
        return isLowSurrogate(
            (
                (
                    prevCodeUnit
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            )
        )
            ? (offset - 2L)
            : (offset - 1L);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static double _computePaintOffsetFraction(
        TextAlign textAlign,
        TextDirection textDirection
    )
    {
        return (textAlign, (textDirection)) switch
        {
            (TextAlign.left, _) => 0.0,
            (TextAlign.right, _) => 1.0,
            (TextAlign.center, _) => 0.5,
            (TextAlign.start or TextAlign.justify, TextDirection.ltr) => 0.0,
            (TextAlign.start or TextAlign.justify, TextDirection.rtl) => 1.0,
            (TextAlign.end, TextDirection.ltr) => 1.0,
            (TextAlign.end, TextDirection.rtl) => 0.0,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Offset getOffsetForCaret(TextPosition position, Rect caretPrototype)
    {
        _TextPainterLayoutCacheWithOffset__text_painter layoutCache = _layoutCache!;
        _LineCaretMetrics__text_painter? caretMetrics = _computeCaretMetrics(position);
        if (caretMetrics is null)
        {
            double paintOffsetAlignment = _computePaintOffsetFraction(
                textAlign,
                (
                    textDirection
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            );
            double dxLocal =
                (paintOffsetAlignment == 0L)
                    ? 0
                    : (paintOffsetAlignment * layoutCache.contentWidth);
            return new Offset(dxLocal, 0.0);
        }
        Offset rawOffset = caretMetrics switch
        {
            _LineCaretMetrics__text_painter
            {
                writingDirection: TextDirection.ltr,
                offset: Offset offsetLocal
            } __object55102 => offsetLocal,
            _LineCaretMetrics__text_painter
            {
                writingDirection: TextDirection.rtl,
                offset: Offset offsetAlternate
            } __object55196 => new Offset(
                offsetAlternate.dx - caretPrototype.width,
                offsetAlternate.dy
            ),
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        double adjustedDx = Dart_uiLibrary.clampDouble(
            rawOffset.dx + layoutCache.paintOffset.dx,
            0,
            layoutCache.contentWidth
        );
        return new Offset(adjustedDx, rawOffset.dy + layoutCache.paintOffset.dy);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual bool _strutDisabled =>
        strutStyle switch
        {
            null => true,
            var __constant56323 when Equals(__constant56323, StrutStyle.disabled) => true,
            StrutStyle { fontSize: double fontSizeLocal } __object56356 => fontSizeLocal == 0.0,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };

    public virtual double getFullHeightForCaret(TextPosition position, Rect caretPrototype)
    {
        if (_strutDisabled)
        {
            double? heightFromCaretMetrics = _computeCaretMetrics(position)?.height;
            if (heightFromCaretMetrics is not null)
            {
                double heightFromCaretMetrics__56763__value56838 = (
                    heightFromCaretMetrics
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
                return (heightFromCaretMetrics__56763__value56838);
            }
        }
        List<TextBox> boxes = _getOrCreateLayoutTemplate()
            .getBoxesForRange(0L, 1L, boxHeightStyle: BoxHeightStyle.strut);
        if (checked((long)boxes.Count) == 0)
        {
            return preferredLineHeight;
        }
        return boxes.Single().toRect().height;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual bool _isNewlineAtOffset(long offset) =>
        (0L <= offset)
        && (offset < plainText.Length)
        && WordBoundary._isNewline(plainText.codeUnitAt(offset));

    internal virtual _LineCaretMetrics__text_painter? _computeCaretMetrics(TextPosition position)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertTextLayoutIsValid);
        DartRuntimePrimitives.Assert(() => !_debugNeedsRelayout);
        _TextPainterLayoutCacheWithOffset__text_painter cachedLayout = _layoutCache!;
        if (cachedLayout.paragraph.numberOfLines < 1L)
        {
            return null;
        }
        var (offsetLocal, anchorToLeadingEdge) = position switch
        {
            TextPosition { offset: 0L } __object60679 => (0L, true),
            TextPosition
            {
                offset: long offsetAlternate,
                affinity: TextAffinity.downstream
            } __object60854 => (offsetAlternate, true),
            TextPosition
            {
                offset: long offsetNested,
                affinity: TextAffinity.upstream
            } __object60946 when _isNewlineAtOffset(offsetNested - 1L) => (offsetNested, true),
            TextPosition
            {
                offset: long offsetCurrent,
                affinity: TextAffinity.upstream
            } __object61090 => (offsetCurrent - 1L, false),
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        long caretPositionCacheKey = anchorToLeadingEdge ? offsetLocal : (-offsetLocal - 1L);
        if (caretPositionCacheKey == cachedLayout._previousCaretPositionKey)
        {
            return _caretMetrics;
        }
        GlyphInfo? glyphInfo = cachedLayout.paragraph.getGlyphInfoAt(offsetLocal);
        if (glyphInfo is null)
        {
            Paragraph template = _getOrCreateLayoutTemplate();
            DartRuntimePrimitives.Assert(() => template.numberOfLines == 1L);
            double baselineOffset = template.getLineMetricsAt(0L)!.baseline;
            return cachedLayout.layout._endOfTextCaretMetrics.shift(
                new Offset(0.0, -baselineOffset)
            );
        }
        TextRange graphemeRange = glyphInfo.graphemeClusterCodeUnitRange;
        if (graphemeRange.isCollapsed)
        {
            DartRuntimePrimitives.Assert(() => graphemeRange.start == 0L);
            return _computeCaretMetrics(new TextPosition(offset: offsetLocal + 1L));
        }
        if (anchorToLeadingEdge && (graphemeRange.start != offsetLocal))
        {
            DartRuntimePrimitives.Assert(() => graphemeRange.end > (graphemeRange.start + 1L));
            return _computeCaretMetrics(new TextPosition(offset: graphemeRange.end));
        }
        _LineCaretMetrics__text_painter metrics = default!;
        List<TextBox> boxes = cachedLayout.paragraph.getBoxesForRange(
            graphemeRange.start,
            graphemeRange.end,
            boxHeightStyle: Dart_uiLibrary.BoxHeightStyle.strut
        );
        bool anchorToLeft = glyphInfo.writingDirection switch
        {
            TextDirection.ltr => anchorToLeadingEdge,
            TextDirection.rtl => !anchorToLeadingEdge,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        TextBox box = anchorToLeft ? boxes.First() : boxes.Last();
        metrics = new _LineCaretMetrics__text_painter(
            offset: new Offset(anchorToLeft ? box.left : box.right, box.top),
            writingDirection: box.direction,
            height: box.bottom - box.top
        );
        cachedLayout._previousCaretPositionKey = caretPositionCacheKey;
        return _caretMetrics = metrics;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual List<TextBox> getBoxesForSelection(
        TextSelection selection,
        BoxHeightStyle boxHeightStyle = BoxHeightStyle.tight,
        BoxWidthStyle boxWidthStyle = BoxWidthStyle.tight
    )
    {
        DartRuntimePrimitives.Assert(() => _debugAssertTextLayoutIsValid);
        DartRuntimePrimitives.Assert(() => selection.isValid);
        DartRuntimePrimitives.Assert(() => !_debugNeedsRelayout);
        _TextPainterLayoutCacheWithOffset__text_painter cachedLayout = _layoutCache!;
        Offset offset = cachedLayout.paintOffset;
        if (!double.IsFinite(offset.dx) || !double.IsFinite(offset.dy))
        {
            return new List<TextBox>();
        }
        List<TextBox> boxes = cachedLayout.paragraph.getBoxesForRange(
            selection.start,
            selection.end,
            boxHeightStyle: boxHeightStyle,
            boxWidthStyle: boxWidthStyle
        );
        return Equals(offset, Offset.zero)
            ? boxes
            : boxes.map((box) => _shiftTextBox(box, offset)).ToList();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual GlyphInfo? getClosestGlyphForOffset(Offset offset)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertTextLayoutIsValid);
        DartRuntimePrimitives.Assert(() => !_debugNeedsRelayout);
        _TextPainterLayoutCacheWithOffset__text_painter cachedLayout = _layoutCache!;
        GlyphInfo? rawGlyphInfo = cachedLayout.paragraph.getClosestGlyphInfoForOffset(
            offset - cachedLayout.paintOffset
        );
        if ((rawGlyphInfo is null) || Equals(cachedLayout.paintOffset, Offset.zero))
        {
            return rawGlyphInfo;
        }
        return new GlyphInfo(
            rawGlyphInfo.graphemeClusterLayoutBounds.shift(cachedLayout.paintOffset),
            rawGlyphInfo.graphemeClusterCodeUnitRange,
            rawGlyphInfo.writingDirection
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual TextPosition getPositionForOffset(Offset offset)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertTextLayoutIsValid);
        DartRuntimePrimitives.Assert(() => !_debugNeedsRelayout);
        _TextPainterLayoutCacheWithOffset__text_painter cachedLayout = _layoutCache!;
        return cachedLayout.paragraph.getPositionForOffset(offset - cachedLayout.paintOffset);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual TextRange getWordBoundary(TextPosition position)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertTextLayoutIsValid);
        return _layoutCache!.paragraph.getWordBoundary(position);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual WordBoundary wordBoundaries => new WordBoundary(text!, _layoutCache!.paragraph);

    public virtual TextRange getLineBoundary(TextPosition position)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertTextLayoutIsValid);
        return _layoutCache!.paragraph.getLineBoundary(position);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static LineMetrics _shiftLineMetrics(LineMetrics metrics, Offset offset)
    {
        DartRuntimePrimitives.Assert(() => double.IsFinite(offset.dx));
        DartRuntimePrimitives.Assert(() => double.IsFinite(offset.dy));
        return new LineMetrics(
            hardBreak: metrics.hardBreak,
            ascent: metrics.ascent,
            descent: metrics.descent,
            unscaledAscent: metrics.unscaledAscent,
            height: metrics.height,
            width: metrics.width,
            left: metrics.left + offset.dx,
            baseline: metrics.baseline + offset.dy,
            lineNumber: metrics.lineNumber
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static TextBox _shiftTextBox(TextBox box, Offset offset)
    {
        DartRuntimePrimitives.Assert(() => double.IsFinite(offset.dx));
        DartRuntimePrimitives.Assert(() => double.IsFinite(offset.dy));
        return new TextBox(
            box.left + offset.dx,
            box.top + offset.dy,
            box.right + offset.dx,
            box.bottom + offset.dy,
            box.direction
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual List<LineMetrics> computeLineMetrics()
    {
        DartRuntimePrimitives.Assert(() => _debugAssertTextLayoutIsValid);
        DartRuntimePrimitives.Assert(() => !_debugNeedsRelayout);
        _TextPainterLayoutCacheWithOffset__text_painter layout = _layoutCache!;
        Offset offset = layout.paintOffset;
        if (!double.IsFinite(offset.dx) || !double.IsFinite(offset.dy))
        {
            return new List<LineMetrics>();
        }
        List<LineMetrics> rawMetrics = layout.lineMetrics;
        return Equals(offset, Offset.zero)
            ? rawMetrics
            : rawMetrics.map((metrics) => _shiftLineMetrics(metrics, offset)).ToList();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool debugDisposed
    {
        get
        {
            bool? disposed = default!;
            DartRuntimePrimitives.Assert(() =>
            {
                disposed = _disposed;
                return true;
            });
            return disposed
                ?? throw new InvalidOperationException(
                    "debugDisposed only available when asserts are on."
                );
        }
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => !debugDisposed);
        DartRuntimePrimitives.Assert(() =>
        {
            _disposed = true;
            return true;
        });
        DartRuntimePrimitives.Assert(() =>
            Foundation.DebugLibrary.debugMaybeDispatchDisposed(this)
        );
        _layoutTemplate?.dispose();
        _layoutTemplate = null;
        _layoutCache?.paragraph.dispose();
        _layoutCache = null;
        _text = null;
    }
}

internal class _UnspecifiedTextScaler__text_painter : TextScaler
{
    internal _UnspecifiedTextScaler__text_painter() { }

    public override double textScaleFactor => throw new NotImplementedException();

    public override double scale(double fontSize) => throw new NotImplementedException();
}
