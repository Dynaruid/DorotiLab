// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/text_painter.dart
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
    visible
}

public class PlaceholderDimensions
{
    public static PlaceholderDimensions empty = new PlaceholderDimensions(size: Size.zero, alignment: Dart_uiLibrary.PlaceholderAlignment.bottom);
    public virtual Size size { get; private set; } = default!;
    public virtual PlaceholderAlignment alignment { get; private set; } = default!;
    public virtual double? baselineOffset { get; private set; }
    public virtual TextBaseline? baseline { get; private set; }

    public PlaceholderDimensions(Size size, PlaceholderAlignment alignment, TextBaseline? baseline = null, double? baselineOffset = null)
    {
        this.size = size;
        this.alignment = alignment;
        this.baseline = baseline;
        this.baselineOffset = baselineOffset;
    }

    public override bool Equals(object? other)
    {
        var __other = other as PlaceholderDimensions;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return (((((__other is PlaceholderDimensions) && (object.Equals(((PlaceholderDimensions)((PlaceholderDimensions)__other)).size, this.size))) && (object.Equals(((PlaceholderDimensions)((PlaceholderDimensions)__other)).alignment, this.alignment))) && (object.Equals(((PlaceholderDimensions)((PlaceholderDimensions)__other)).baseline, this.baseline))) && (((PlaceholderDimensions)((PlaceholderDimensions)__other)).baselineOffset == this.baselineOffset));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.size, this.alignment, this.baseline, this.baselineOffset);
    public override string ToString()
    {
        return (this.alignment switch { Dart_uiLibrary.PlaceholderAlignment.top or Dart_uiLibrary.PlaceholderAlignment.bottom or Dart_uiLibrary.PlaceholderAlignment.middle or Dart_uiLibrary.PlaceholderAlignment.aboveBaseline => $"PlaceholderDimensions({this.size}, {this.alignment})", Dart_uiLibrary.PlaceholderAlignment.belowBaseline => $"PlaceholderDimensions({this.size}, {this.alignment})", Dart_uiLibrary.PlaceholderAlignment.baseline => $"PlaceholderDimensions({this.size}, {this.alignment}({this.baselineOffset} from top))", _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum TextWidthBasis
{
    parent,
    longestLine
}

public class WordBoundary : TextBoundary
{
    internal virtual InlineSpan _text { get; private set; } = default!;
    internal virtual Paragraph _paragraph { get; private set; } = default!;
    internal static RegExp _regExpSpaceSeparatorOrPunctuation = new RegExp("[\\p{Space_Separator}\\p{Punctuation}]", unicode: true);
    private bool __late_moveByWordBoundary_initialized;
    private TextBoundary __late_moveByWordBoundary = default!;
    public virtual TextBoundary moveByWordBoundary
    {
        get
        {
            if (!__late_moveByWordBoundary_initialized)
            {
                __late_moveByWordBoundary = new _UntilTextBoundary__text_painter(this, this._skipSpacesAndPunctuations);
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

    public virtual global::Doroti.Ui.TextRange getTextBoundaryAt(long position) => this._paragraph.getWordBoundary(new global::Doroti.Ui.TextPosition(offset: Math.Max(position, 0L)));
    internal static long _codePointFromSurrogates(long highSurrogate, long lowSurrogate)
    {
        DartRuntimePrimitives.Assert(() => TextPainter.isHighSurrogate(highSurrogate));
        DartRuntimePrimitives.Assert(() => TextPainter.isLowSurrogate(lowSurrogate));
        long @base__6815 = ((65536L - ((55296L << (int)(10L)))) - 56320L);
        return ((((highSurrogate << (int)(10L))) + lowSurrogate) + @base__6815);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long? _codePointAt(long index)
    {
        long? codeUnitAtIndex__7044 = this._text.codeUnitAt(index);
        if ((codeUnitAtIndex__7044 is null))
        {
            return null;
        }
        return ((DartRuntimePrimitives.RequireValue(codeUnitAtIndex__7044) & 64512L) switch { 55296L => _codePointFromSurrogates(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(codeUnitAtIndex__7044)), DartRuntimePrimitives.RequireValue(this._text.codeUnitAt((index + 1L)))), 56320L => _codePointFromSurrogates(DartRuntimePrimitives.RequireValue(this._text.codeUnitAt((index - 1L))), DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(codeUnitAtIndex__7044))), _ => DartRuntimePrimitives.RequireValue(codeUnitAtIndex__7044) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static bool _isNewline(long codePoint)
    {
        return (codePoint switch { 10L or 133L or 11L or 12L or 8232L => true, 8233L => true, _ => false });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _skipSpacesAndPunctuations(long offset, bool forward)
    {
        long? innerCodePoint__8175 = _codePointAt((forward ? (offset - 1L) : offset));
        long? outerCodeUnit__8252 = this._text.codeUnitAt((forward ? offset : (offset - 1L)));
        bool hardBreakRulesApply__8741 = ((((innerCodePoint__8175 is null) || (outerCodeUnit__8252 is null)) || _isNewline(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(innerCodePoint__8175)))) || _isNewline(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(outerCodeUnit__8252))));
        return (hardBreakRulesApply__8741 || !_regExpSpaceSeparatorOrPunctuation.hasMatch(char.ConvertFromUtf32(checked((int)DartRuntimePrimitives.RequireValue(innerCodePoint__8175)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _UntilTextBoundary__text_painter : TextBoundary
{
    internal virtual Func<long, bool, bool> _predicate { get; private set; } = default!;
    internal virtual TextBoundary _textBoundary { get; private set; } = default!;

    internal _UntilTextBoundary__text_painter(TextBoundary _textBoundary, Func<long, bool, bool> _predicate)
    {
        this._textBoundary = _textBoundary;
        this._predicate = _predicate;
    }

    public virtual long? getLeadingTextBoundaryAt(long position)
    {
        if ((position < 0L))
        {
            return null;
        }
        long? offset__10158 = this._textBoundary.getLeadingTextBoundaryAt(position);
        return (((offset__10158 is null) || this._predicate(DartRuntimePrimitives.RequireValue(offset__10158), false)) ? offset__10158 : getLeadingTextBoundaryAt((DartRuntimePrimitives.RequireValue(offset__10158) - 1L)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long? getTrailingTextBoundaryAt(long position)
    {
        long? offset__10418 = this._textBoundary.getTrailingTextBoundaryAt(Math.Max(position, 0L));
        return (((offset__10418 is null) || this._predicate(DartRuntimePrimitives.RequireValue(offset__10418), true)) ? offset__10418 : getTrailingTextBoundaryAt(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(offset__10418))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TextLayout__text_painter
{
    public virtual TextDirection writingDirection { get; private set; } = default!;
    internal virtual TextPainter _painter { get; private set; } = default!;
    internal virtual Paragraph _paragraph { get; set; } = default!;
    internal static RegExp _regExpSpaceSeparators = new RegExp("\\p{Space_Separator}", unicode: true);
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

    internal _TextLayout__text_painter(Paragraph _paragraph, TextDirection writingDirection, TextPainter _painter)
    {
        this._paragraph = _paragraph;
        this.writingDirection = writingDirection;
        this._painter = _painter;
    }

    public virtual bool debugDisposed => this._paragraph.debugDisposed;
    public virtual double width => this._paragraph.width;
    public virtual double height => this._paragraph.height;
    public virtual double minIntrinsicLineExtent => this._paragraph.minIntrinsicWidth;
    public virtual double maxIntrinsicLineExtent => this._paragraph.maxIntrinsicWidth;
    public virtual double longestLine => this._paragraph.longestLine;
    public virtual double getDistanceToBaseline(TextBaseline baseline)
    {
        return (baseline switch { TextBaseline.alphabetic => this._paragraph.alphabeticBaseline, TextBaseline.ideographic => this._paragraph.ideographicBaseline, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _LineCaretMetrics__text_painter _computeEndOfTextCaretAnchorOffset()
    {
        string rawString__13685 = ((TextPainter)this._painter).plainText;
        long lastLineIndex__13731 = (this._paragraph.numberOfLines - 1L);
        DartRuntimePrimitives.Assert(() => (lastLineIndex__13731 >= 0L));
        global::Doroti.Ui.LineMetrics lineMetrics__13834 = this._paragraph.getLineMetricsAt(lastLineIndex__13731)!;
        string lastCodeUnit__14441 = rawString__13685[(int)((rawString__13685.Length - 1L))].ToString();
        bool hasTrailingSpaces__14504 = (lastCodeUnit__14441.codeUnitAt(0L) switch { 9L => true, 160L or 8199L => false, 8239L => false, _ => _regExpSpaceSeparators.hasMatch(lastCodeUnit__14441) });
        double baseline__14799 = lineMetrics__13834.baseline;
        double dx__14849 = default!;
        double height__14870 = default!;
        global::Doroti.Ui.GlyphInfo? lastGlyph__14907 = this._paragraph.getGlyphInfoAt((rawString__13685.Length - 1L));
        if ((hasTrailingSpaces__14504 && (lastGlyph__14907 is not null)))
        {
            global::Doroti.Ui.Rect glyphBounds__15171 = lastGlyph__14907.graphemeClusterLayoutBounds;
            DartRuntimePrimitives.Assert(() => !glyphBounds__15171.isEmpty);
            dx__14849 = (this.writingDirection switch { TextDirection.ltr => glyphBounds__15171.right, TextDirection.rtl => glyphBounds__15171.left, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            height__14870 = glyphBounds__15171.height;
        }
        else
        {
            dx__14849 = (this.writingDirection switch { TextDirection.ltr => (lineMetrics__13834.left + lineMetrics__13834.width), TextDirection.rtl => lineMetrics__13834.left, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            height__14870 = lineMetrics__13834.height;
        }
        return new _LineCaretMetrics__text_painter(offset: new global::Doroti.Ui.Offset(dx__14849, baseline__14799), writingDirection: this.writingDirection, height: height__14870);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _contentWidthFor(double minWidth, double maxWidth, TextWidthBasis widthBasis)
    {
        return (widthBasis switch { TextWidthBasis.longestLine => Dart_uiLibrary.clampDouble(this.longestLine, minWidth, maxWidth), TextWidthBasis.parent => Dart_uiLibrary.clampDouble(this.maxIntrinsicLineExtent, minWidth, maxWidth), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
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

    internal _TextPainterLayoutCacheWithOffset__text_painter(_TextLayout__text_painter layout, double textAlignment, double layoutMaxWidth, double contentWidth)
    {
        this.layout = layout;
        this.textAlignment = textAlignment;
        this.layoutMaxWidth = layoutMaxWidth;
        this.contentWidth = contentWidth;
        System.Diagnostics.Debug.Assert(((textAlignment >= 0.0) && (textAlignment <= 1.0)));
        System.Diagnostics.Debug.Assert(!double.IsNaN(layoutMaxWidth));
        System.Diagnostics.Debug.Assert(!double.IsNaN(contentWidth));
    }

    public virtual global::Doroti.Ui.Offset paintOffset
    {
        get
        {
            if ((this.textAlignment == 0L))
            {
                return Offset.zero;
            }
            if (!double.IsFinite(this.paragraph.width))
            {
                return new global::Doroti.Ui.Offset(double.PositiveInfinity, 0.0);
            }
            double dx__17434 = (this.textAlignment * ((this.contentWidth - this.paragraph.width)));
            DartRuntimePrimitives.Assert(() => !double.IsNaN(dx__17434));
            return new global::Doroti.Ui.Offset(dx__17434, 0);
            return default!;
        }
    }
    public virtual global::Doroti.Ui.Paragraph paragraph => ((_TextLayout__text_painter)this.layout)._paragraph;
    internal virtual bool _resizeToFit(double minWidth, double maxWidth, TextWidthBasis widthBasis)
    {
        DartRuntimePrimitives.Assert(() => double.IsFinite(((_TextLayout__text_painter)this.layout).maxIntrinsicLineExtent));
        DartRuntimePrimitives.Assert(() => (minWidth <= maxWidth));
        if (((maxWidth == this.contentWidth) && (minWidth == this.contentWidth)))
        {
            contentWidth = this.layout._contentWidthFor(minWidth, maxWidth, widthBasis);
            return true;
        }
        if (((!double.IsFinite(this.paintOffset.dx) && !double.IsFinite(this.paragraph.width)) && double.IsFinite(minWidth)))
        {
            DartRuntimePrimitives.Assert(() => (this.paintOffset.dx == double.PositiveInfinity));
            DartRuntimePrimitives.Assert(() => (this.paragraph.width == double.PositiveInfinity));
            return false;
        }
        double maxIntrinsicWidth__19391 = this.paragraph.maxIntrinsicWidth;
        bool skipLineBreaking__19558 = ((maxWidth == this.layoutMaxWidth) || (((((this.paragraph.width - maxIntrinsicWidth__19391)) > -global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance) && (((maxWidth - maxIntrinsicWidth__19391)) > -global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))));
        if (skipLineBreaking__19558)
        {
            contentWidth = this.layout._contentWidthFor(minWidth, maxWidth, widthBasis);
            return true;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<global::Doroti.Ui.TextBox> inlinePlaceholderBoxes => _cachedInlinePlaceholderBoxes ??= this.paragraph.getBoxesForPlaceholders();
    public virtual List<global::Doroti.Ui.LineMetrics> lineMetrics => _cachedLineMetrics ??= this.paragraph.computeLineMetrics();
}

internal class _LineCaretMetrics__text_painter
{
    public virtual Offset offset { get; private set; } = default!;
    public virtual TextDirection writingDirection { get; private set; } = default!;
    public virtual double height { get; private set; } = default!;

    internal _LineCaretMetrics__text_painter(Offset offset, TextDirection writingDirection, double height)
    {
        this.offset = offset;
        this.writingDirection = writingDirection;
        this.height = height;
    }

    public virtual _LineCaretMetrics__text_painter shift(Offset offset)
    {
        return ((object.Equals(offset, Offset.zero)) ? this : new _LineCaretMetrics__text_painter(offset: (offset + this.offset), writingDirection: this.writingDirection, height: this.height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class TextPainter
{
    internal virtual bool _debugNeedsRelayout { get; set; } = true;
    internal virtual _TextPainterLayoutCacheWithOffset__text_painter? _layoutCache { get; set; } = default;
    internal virtual bool _rebuildParagraphForPaint { get; set; } = true;
    internal virtual global::System.Diagnostics.StackTrace? _debugMarkNeedsLayoutCallStack { get; set; } = default;
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

    public TextPainter(InlineSpan? text = null, TextAlign textAlign = TextAlign.start, TextDirection? textDirection = null, double textScaleFactor = 1.0, TextScaler textScaler = default!, long? maxLines = null, string? ellipsis = null, Locale? locale = null, StrutStyle? strutStyle = null, TextWidthBasis textWidthBasis = TextWidthBasis.parent, TextHeightBehavior? textHeightBehavior = null)
    {
        TextScaler __textScaler = textScaler ?? new _UnspecifiedTextScaler__text_painter();
        this._text = text;
        this._textAlign = textAlign;
        this._textDirection = textDirection;
        this._textScaler = ((object.Equals(textScaler, new _UnspecifiedTextScaler__text_painter())) ? TextScaler.CreateLinear(textScaleFactor) : textScaler);
        this._maxLines = maxLines;
        this._ellipsis = ellipsis;
        this._locale = locale;
        this._strutStyle = strutStyle;
        this._textWidthBasis = textWidthBasis;
        this._textHeightBehavior = textHeightBehavior;
        System.Diagnostics.Debug.Assert(((text is null) || text.debugAssertIsValid()));
        System.Diagnostics.Debug.Assert(((maxLines is null) || (DartRuntimePrimitives.RequireValue(maxLines) > 0L)));
        System.Diagnostics.Debug.Assert(((textScaleFactor == 1.0) || DartRuntimePrimitives.Identical(__textScaler, new _UnspecifiedTextScaler__text_painter())));
    }

    public static double computeWidth(InlineSpan text, TextDirection textDirection, TextAlign textAlign = TextAlign.start, double textScaleFactor = 1.0, TextScaler textScaler = default!, long? maxLines = null, string? ellipsis = null, Locale? locale = null, StrutStyle? strutStyle = null, TextWidthBasis textWidthBasis = TextWidthBasis.parent, TextHeightBehavior? textHeightBehavior = null, double minWidth = 0.0, double maxWidth = double.PositiveInfinity)
    {
        DartRuntimePrimitives.Assert(() => ((textScaleFactor == 1.0) || DartRuntimePrimitives.Identical(textScaler, TextScaler.noScaling)));
        var painter__25686 = ((Func<TextPainter>)(() =>
{
    var __cascade = new TextPainter(text: text, textAlign: textAlign, textDirection: DartRuntimePrimitives.RequireValue(textDirection), textScaler: ((object.Equals(textScaler, TextScaler.noScaling)) ? TextScaler.CreateLinear(textScaleFactor) : textScaler), maxLines: maxLines, ellipsis: ellipsis, locale: locale, strutStyle: strutStyle, textWidthBasis: textWidthBasis, textHeightBehavior: textHeightBehavior);
    __cascade.layout(minWidth: minWidth, maxWidth: maxWidth);
    return __cascade;
}))();
        try
        {
            return ((TextPainter)painter__25686).width;
        }
        finally
        {
            painter__25686.dispose();
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static double computeMaxIntrinsicWidth(InlineSpan text, TextDirection textDirection, TextAlign textAlign = TextAlign.start, double textScaleFactor = 1.0, TextScaler textScaler = default!, long? maxLines = null, string? ellipsis = null, Locale? locale = null, StrutStyle? strutStyle = null, TextWidthBasis textWidthBasis = TextWidthBasis.parent, TextHeightBehavior? textHeightBehavior = null, double minWidth = 0.0, double maxWidth = double.PositiveInfinity)
    {
        DartRuntimePrimitives.Assert(() => ((textScaleFactor == 1.0) || DartRuntimePrimitives.Identical(textScaler, TextScaler.noScaling)));
        var painter__27638 = ((Func<TextPainter>)(() =>
{
    var __cascade = new TextPainter(text: text, textAlign: textAlign, textDirection: DartRuntimePrimitives.RequireValue(textDirection), textScaler: ((object.Equals(textScaler, TextScaler.noScaling)) ? TextScaler.CreateLinear(textScaleFactor) : textScaler), maxLines: maxLines, ellipsis: ellipsis, locale: locale, strutStyle: strutStyle, textWidthBasis: textWidthBasis, textHeightBehavior: textHeightBehavior);
    __cascade.layout(minWidth: minWidth, maxWidth: maxWidth);
    return __cascade;
}))();
        try
        {
            return ((TextPainter)painter__27638).maxIntrinsicWidth;
        }
        finally
        {
            painter__27638.dispose();
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugAssertTextLayoutIsValid
    {
        get
        {
            DartRuntimePrimitives.Assert(() => !this.debugDisposed);
            if ((this._layoutCache is null))
            {
                throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Text layout not available") });
            }
            return true;
            return default!;
        }
    }
    public virtual void markNeedsLayout()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._layoutCache is not null))
                {
                    _debugMarkNeedsLayoutCallStack ??= new global::System.Diagnostics.StackTrace(true);
                }
                return true;
            });
        this._layoutCache?.paragraph.dispose();
        _layoutCache = null;
    }

    public virtual InlineSpan? text
    {
        get => this._text;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((__value is null) || __value.debugAssertIsValid()));
            if ((object.Equals(this._text, __value)))
            {
                return;
            }
            if ((!object.Equals(this._text?.style, __value?.style)))
            {
                this._layoutTemplate?.dispose();
                _layoutTemplate = null;
            }
            RenderComparison comparison__30807 = ((__value is null) ? RenderComparison.layout : (this._text?.compareTo(__value) ?? RenderComparison.layout));
            _text = __value;
            _cachedPlainText = null;
            if ((FoundationRuntimePorts.EnumIndex(comparison__30807) >= FoundationRuntimePorts.EnumIndex(RenderComparison.layout)))
            {
                markNeedsLayout();
            }
            else
            {
                if ((FoundationRuntimePorts.EnumIndex(comparison__30807) >= FoundationRuntimePorts.EnumIndex(RenderComparison.paint)))
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
            _cachedPlainText ??= this._text?.toPlainText(includeSemanticsLabels: false);
            return (this._cachedPlainText ?? "");
            return default!;
        }
    }
    public virtual global::Doroti.Ui.TextAlign textAlign
    {
        get => this._textAlign;
        set
        {
            var __value = value;
            if ((object.Equals(this._textAlign, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _textAlign = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextDirection? textDirection
    {
        get => this._textDirection;
        set
        {
            var __value = value;
            if ((object.Equals(this._textDirection, __value)))
            {
                return;
            }
            _textDirection = __value;
            markNeedsLayout();
            this._layoutTemplate?.dispose();
            _layoutTemplate = null;
        }
    }
    public virtual double textScaleFactor
    {
        get => ((TextScaler)this.textScaler).textScaleFactor;
        set
        {
            var __value = value;
            textScaler = TextScaler.CreateLinear(DartRuntimePrimitives.RequireValue(__value));
        }
    }
    public virtual TextScaler textScaler
    {
        get => this._textScaler;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._textScaler)))
            {
                return;
            }
            _textScaler = __value;
            markNeedsLayout();
            this._layoutTemplate?.dispose();
            _layoutTemplate = null;
        }
    }
    public virtual string? ellipsis
    {
        get => this._ellipsis;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((__value is null) || (__value.Length != 0)));
            if ((this._ellipsis == __value))
            {
                return;
            }
            _ellipsis = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.Locale? locale
    {
        get => this._locale;
        set
        {
            var __value = value;
            if ((object.Equals(this._locale, __value)))
            {
                return;
            }
            _locale = __value;
            markNeedsLayout();
        }
    }
    public virtual long? maxLines
    {
        get => this._maxLines;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((__value is null) || (DartRuntimePrimitives.RequireValue(__value) > 0L)));
            if ((this._maxLines == __value))
            {
                return;
            }
            _maxLines = __value;
            markNeedsLayout();
        }
    }
    public virtual StrutStyle? strutStyle
    {
        get => this._strutStyle;
        set
        {
            var __value = value;
            if ((object.Equals(this._strutStyle, __value)))
            {
                return;
            }
            _strutStyle = __value;
            markNeedsLayout();
        }
    }
    public virtual TextWidthBasis textWidthBasis
    {
        get => this._textWidthBasis;
        set
        {
            var __value = value;
            if ((object.Equals(this._textWidthBasis, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            DartRuntimePrimitives.Assert(() =>
                {
                    return _debugNeedsRelayout = true;
                });
            _textWidthBasis = DartRuntimePrimitives.RequireValue(__value);
        }
    }
    public virtual global::Doroti.Ui.TextHeightBehavior? textHeightBehavior
    {
        get => this._textHeightBehavior;
        set
        {
            var __value = value is null ? null : (TextHeightBehavior)(object)value;
            if ((object.Equals(this._textHeightBehavior, __value)))
            {
                return;
            }
            _textHeightBehavior = __value;
            markNeedsLayout();
        }
    }
    public virtual List<global::Doroti.Ui.TextBox>? inlinePlaceholderBoxes
    {
        get
        {
            _TextPainterLayoutCacheWithOffset__text_painter? layout__38748 = this._layoutCache;
            if ((layout__38748 is null))
            {
                return null;
            }
            global::Doroti.Ui.Offset offset__38839 = ((_TextPainterLayoutCacheWithOffset__text_painter)layout__38748).paintOffset;
            if ((!double.IsFinite(offset__38839.dx) || !double.IsFinite(offset__38839.dy)))
            {
                return new List<global::Doroti.Ui.TextBox>();
            }
            List<global::Doroti.Ui.TextBox> rawBoxes__38978 = ((_TextPainterLayoutCacheWithOffset__text_painter)layout__38748).inlinePlaceholderBoxes;
            if ((object.Equals(offset__38839, Offset.zero)))
            {
                return rawBoxes__38978;
            }
            return rawBoxes__38978.map<TextBox, TextBox>(((box) => _shiftTextBox(box, offset__38839))).ToList();
            return default!;
        }
    }
    public virtual void setPlaceholderDimensions(List<PlaceholderDimensions>? value)
    {
        if ((((value is null) || (checked((long)(value.Count)) == 0)) || global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals(value, this._placeholderDimensions)))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                var placeholderCount__39830 = 0L;
                this.text!.visitChildren(((Func<InlineSpan, bool>)((span) =>
                {
                    if ((span is PlaceholderSpan))
                    {
                        placeholderCount__39830 += 1L;
                    }
                    return (checked((long)(value.Count)) >= placeholderCount__39830);
                    return default;
                })));
                return (placeholderCount__39830 == checked((long)(value.Count)));
            });
        _placeholderDimensions = value;
        markNeedsLayout();
    }

    internal virtual global::Doroti.Ui.ParagraphStyle _createParagraphStyle(TextAlign? textAlignOverride = null)
    {
        DartRuntimePrimitives.Assert(() => (this.textDirection is not null));
        TextStyle baseStyle__40457 = (this._text?.style ?? new TextStyle());
        return baseStyle__40457.getParagraphStyle(textAlign: (textAlignOverride ?? this.textAlign), textDirection: this.textDirection, textScaler: this.textScaler, maxLines: this._maxLines, textHeightBehavior: this._textHeightBehavior, ellipsis: this._ellipsis, locale: this._locale, strutStyle: this._strutStyle);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Paragraph _createLayoutTemplate()
    {
        var builder__40910 = new global::Doroti.Ui.ParagraphBuilder(_createParagraphStyle(TextAlign.left));
        global::Doroti.Ui.TextStyle? textStyle__41067 = this.text?.style?.getTextStyle(textScaler: this.textScaler);
        if ((textStyle__41067 is not null))
        {
            builder__40910.pushStyle(textStyle__41067);
        }
        builder__40910.addText(" ");
        return ((Func<Paragraph>)(() =>
{
    var __cascade = builder__40910.build();
    __cascade.layout(new global::Doroti.Ui.ParagraphConstraints(width: double.PositiveInfinity));
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Paragraph _getOrCreateLayoutTemplate() => _layoutTemplate ??= _createLayoutTemplate();
    public virtual double preferredLineHeight => _getOrCreateLayoutTemplate().height;
    public virtual double minIntrinsicWidth
    {
        get
        {
            DartRuntimePrimitives.Assert(() => this._debugAssertTextLayoutIsValid);
            return this._layoutCache!.layout.minIntrinsicLineExtent;
            return default!;
        }
    }
    public virtual double maxIntrinsicWidth
    {
        get
        {
            DartRuntimePrimitives.Assert(() => this._debugAssertTextLayoutIsValid);
            return this._layoutCache!.layout.maxIntrinsicLineExtent;
            return default!;
        }
    }
    public virtual double width
    {
        get
        {
            DartRuntimePrimitives.Assert(() => this._debugAssertTextLayoutIsValid);
            DartRuntimePrimitives.Assert(() => !this._debugNeedsRelayout);
            return this._layoutCache!.contentWidth;
            return default!;
        }
    }
    public virtual double height
    {
        get
        {
            DartRuntimePrimitives.Assert(() => this._debugAssertTextLayoutIsValid);
            return this._layoutCache!.layout.height;
            return default!;
        }
    }
    public virtual global::Doroti.Ui.Size size
    {
        get
        {
            DartRuntimePrimitives.Assert(() => this._debugAssertTextLayoutIsValid);
            DartRuntimePrimitives.Assert(() => !this._debugNeedsRelayout);
            return new global::Doroti.Ui.Size(this.width, this.height);
            return default!;
        }
    }
    public virtual double computeDistanceToActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => this._debugAssertTextLayoutIsValid);
        return this._layoutCache!.layout.getDistanceToBaseline(baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool didExceedMaxLines
    {
        get
        {
            DartRuntimePrimitives.Assert(() => this._debugAssertTextLayoutIsValid);
            return this._layoutCache!.paragraph.didExceedMaxLines;
            return default!;
        }
    }
    internal virtual global::Doroti.Ui.Paragraph _createParagraph(InlineSpan text)
    {
        var builder__44548 = new global::Doroti.Ui.ParagraphBuilder(_createParagraphStyle());
        text.build(builder__44548, textScaler: this.textScaler, dimensions: this._placeholderDimensions);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugMarkNeedsLayoutCallStack = null;
                return true;
            });
        _rebuildParagraphForPaint = false;
        return builder__44548.build();
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
        _TextPainterLayoutCacheWithOffset__text_painter? cachedLayout__45571 = this._layoutCache;
        if (((cachedLayout__45571 is not null) && cachedLayout__45571._resizeToFit(minWidth, maxWidth, this.textWidthBasis)))
        {
            return;
        }
        InlineSpan? text__45740 = this.text;
        if ((text__45740 is null))
        {
            throw new InvalidOperationException("TextPainter.text must be set to a non-null value before using the TextPainter.");
        }
        global::Doroti.Ui.TextDirection? textDirection__45936 = this.textDirection;
        if ((textDirection__45936 is null))
        {
            throw new InvalidOperationException("TextPainter.textDirection must be set to a non-null value before using the TextPainter.");
        }
        double paintOffsetAlignment__46161 = _computePaintOffsetFraction(this.textAlign, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(textDirection__45936)));
        bool adjustMaxWidth__46434 = (!double.IsFinite(maxWidth) && (paintOffsetAlignment__46161 != 0L));
        double? adjustedMaxWidth__46518 = (!adjustMaxWidth__46434 ? maxWidth : cachedLayout__45571?.layout.maxIntrinsicLineExtent);
        double layoutMaxWidth__46644 = (adjustedMaxWidth__46518 ?? maxWidth);
        global::Doroti.Ui.Paragraph paragraph__47147 = ((Func<Paragraph>)(() =>
{
    var __cascade = ((cachedLayout__45571?.paragraph ?? _createParagraph(text__45740)));
    __cascade.layout(new global::Doroti.Ui.ParagraphConstraints(width: layoutMaxWidth__46644));
    return __cascade;
}))();
        var layout__47285 = new _TextLayout__text_painter(paragraph__47147, DartRuntimePrimitives.RequireValue(textDirection__45936), this);
        double contentWidth__47358 = layout__47285._contentWidthFor(minWidth, maxWidth, this.textWidthBasis);
        _TextPainterLayoutCacheWithOffset__text_painter newLayoutCache__47479 = default!;
        if (((adjustedMaxWidth__46518 is null) && double.IsFinite(minWidth)))
        {
            DartRuntimePrimitives.Assert(() => double.IsInfinity(maxWidth));
            double newInputWidth__47789 = ((_TextLayout__text_painter)layout__47285).maxIntrinsicLineExtent;
            paragraph__47147.layout(new global::Doroti.Ui.ParagraphConstraints(width: newInputWidth__47789));
            newLayoutCache__47479 = new _TextPainterLayoutCacheWithOffset__text_painter(layout__47285, paintOffsetAlignment__46161, newInputWidth__47789, contentWidth__47358);
        }
        else
        {
            newLayoutCache__47479 = new _TextPainterLayoutCacheWithOffset__text_painter(layout__47285, paintOffsetAlignment__46161, layoutMaxWidth__46644, contentWidth__47358);
        }
        _layoutCache = newLayoutCache__47479;
    }

    public virtual void paint(Canvas canvas, Offset offset)
    {
        _TextPainterLayoutCacheWithOffset__text_painter? layoutCache__49757 = this._layoutCache;
        if ((layoutCache__49757 is null))
        {
            throw new InvalidOperationException("TextPainter.paint called when text geometry was not yet calculated.\n" + "Please call layout() before paint() to position the text before painting it.");
        }
        if ((!double.IsFinite(((_TextPainterLayoutCacheWithOffset__text_painter)layoutCache__49757).paintOffset.dx) || !double.IsFinite(((_TextPainterLayoutCacheWithOffset__text_painter)layoutCache__49757).paintOffset.dy)))
        {
            return;
        }
        if (this._rebuildParagraphForPaint)
        {
            global::Doroti.Ui.Size? debugSize__50182 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    debugSize__50182 = this.size;
                    return true;
                });
            global::Doroti.Ui.Paragraph paragraph__50296 = ((_TextPainterLayoutCacheWithOffset__text_painter)layoutCache__49757).paragraph;
            DartRuntimePrimitives.Assert(() => !double.IsNaN(((_TextPainterLayoutCacheWithOffset__text_painter)layoutCache__49757).layoutMaxWidth));
            ((_TextPainterLayoutCacheWithOffset__text_painter)layoutCache__49757).layout._paragraph = ((Func<Paragraph>)(() =>
{
    var __cascade = _createParagraph(this.text!);
    __cascade.layout(new global::Doroti.Ui.ParagraphConstraints(width: ((_TextPainterLayoutCacheWithOffset__text_painter)layoutCache__49757).layoutMaxWidth));
    return __cascade;
}))();
            DartRuntimePrimitives.Assert(() => (paragraph__50296.width == ((_TextPainterLayoutCacheWithOffset__text_painter)layoutCache__49757).layout._paragraph.width));
            paragraph__50296.dispose();
            DartRuntimePrimitives.Assert(() => (object.Equals(debugSize__50182, this.size)));
        }
        DartRuntimePrimitives.Assert(() => !this._rebuildParagraphForPaint);
        DartRuntimePrimitives.Assert(() => (!this.debugPaintTextLayoutBoxes || _debugPaintCharacterLayoutBoxes(canvas, layoutCache__49757, offset)));
        canvas.drawParagraph(((_TextPainterLayoutCacheWithOffset__text_painter)layoutCache__49757).paragraph, (offset + ((_TextPainterLayoutCacheWithOffset__text_painter)layoutCache__49757).paintOffset));
    }

    internal virtual bool _debugPaintCharacterLayoutBoxes(Canvas canvas, _TextPainterLayoutCacheWithOffset__text_painter layout, Offset offset)
    {
        var paint__51229 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = 1.0;
    __cascade.color = new global::Doroti.Ui.Color(4278255615L);
    return __cascade;
}))();
        List<global::Doroti.Ui.TextBox> textBoxes__51373 = getBoxesForSelection(new TextSelection(baseOffset: 0L, extentOffset: this.plainText.Length));
        foreach (var textBox__51497 in textBoxes__51373)
        {
            canvas.drawRect(textBox__51497.toRect().shift(offset), paint__51229);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static bool _isUTF16(long value)
    {
        return ((value >= 0L) && (value <= 1048575L));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool isHighSurrogate(long value)
    {
        DartRuntimePrimitives.Assert(() => _isUTF16(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(value))));
        return ((DartRuntimePrimitives.RequireValue(value) & 64512L) == 55296L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool isLowSurrogate(long value)
    {
        DartRuntimePrimitives.Assert(() => _isUTF16(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(value))));
        return ((DartRuntimePrimitives.RequireValue(value) & 64512L) == 56320L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long? getOffsetAfter(long offset)
    {
        long? nextCodeUnit__52902 = this._text!.codeUnitAt(offset);
        if ((nextCodeUnit__52902 is null))
        {
            return null;
        }
        return (isHighSurrogate(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(nextCodeUnit__52902))) ? (offset + 2L) : (offset + 1L));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long? getOffsetBefore(long offset)
    {
        long? prevCodeUnit__53384 = this._text!.codeUnitAt((offset - 1L));
        if ((prevCodeUnit__53384 is null))
        {
            return null;
        }
        return (isLowSurrogate(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(prevCodeUnit__53384))) ? (offset - 2L) : (offset - 1L));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _computePaintOffsetFraction(TextAlign textAlign, TextDirection textDirection)
    {
        return ((textAlign, DartRuntimePrimitives.RequireValue(textDirection)) switch { (TextAlign.left, _) => 0.0, (TextAlign.right, _) => 1.0, (TextAlign.center, _) => 0.5, (TextAlign.start or TextAlign.justify, TextDirection.ltr) => 0.0, (TextAlign.start or TextAlign.justify, TextDirection.rtl) => 1.0, (TextAlign.end, TextDirection.ltr) => 1.0, (TextAlign.end, TextDirection.rtl) => 0.0, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Offset getOffsetForCaret(TextPosition position, Rect caretPrototype)
    {
        _TextPainterLayoutCacheWithOffset__text_painter layoutCache__54447 = this._layoutCache!;
        _LineCaretMetrics__text_painter? caretMetrics__54505 = _computeCaretMetrics(position);
        if ((caretMetrics__54505 is null))
        {
            double paintOffsetAlignment__54604 = _computePaintOffsetFraction(this.textAlign, DartRuntimePrimitives.RequireValue(this.textDirection));
            double dx__54900 = ((paintOffsetAlignment__54604 == 0L) ? 0 : (paintOffsetAlignment__54604 * ((_TextPainterLayoutCacheWithOffset__text_painter)layoutCache__54447).contentWidth));
            return new global::Doroti.Ui.Offset(dx__54900, 0.0);
        }
        global::Doroti.Ui.Offset rawOffset__55060 = (caretMetrics__54505 switch { _LineCaretMetrics__text_painter { writingDirection: TextDirection.ltr, offset: global::Doroti.Ui.Offset offset__55171 } __object55102 => offset__55171, _LineCaretMetrics__text_painter { writingDirection: TextDirection.rtl, offset: global::Doroti.Ui.Offset offset__55265 } __object55196 => new global::Doroti.Ui.Offset((offset__55265.dx - caretPrototype.width), offset__55265.dy), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        double adjustedDx__55719 = Dart_uiLibrary.clampDouble((rawOffset__55060.dx + ((_TextPainterLayoutCacheWithOffset__text_painter)layoutCache__54447).paintOffset.dx), 0, ((_TextPainterLayoutCacheWithOffset__text_painter)layoutCache__54447).contentWidth);
        return new global::Doroti.Ui.Offset(adjustedDx__55719, (rawOffset__55060.dy + ((_TextPainterLayoutCacheWithOffset__text_painter)layoutCache__54447).paintOffset.dy));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _strutDisabled => (this.strutStyle switch { null => true, var __constant56323 when object.Equals(__constant56323, StrutStyle.disabled) => true, StrutStyle { fontSize: double fontSize__56382 } __object56356 => (fontSize__56382 == 0.0), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    public virtual double getFullHeightForCaret(TextPosition position, Rect caretPrototype)
    {
        if (this._strutDisabled)
        {
            double? heightFromCaretMetrics__56763 = _computeCaretMetrics(position)?.height;
            if ((heightFromCaretMetrics__56763 is not null))
            {
                double heightFromCaretMetrics__56763__value56838 = DartRuntimePrimitives.RequireValue(heightFromCaretMetrics__56763);
                return DartRuntimePrimitives.RequireValue(heightFromCaretMetrics__56763__value56838);
            }
        }
        List<global::Doroti.Ui.TextBox> boxes__56949 = _getOrCreateLayoutTemplate().getBoxesForRange(0L, 1L, boxHeightStyle: BoxHeightStyle.strut);
        if ((checked((long)(boxes__56949.Count)) == 0))
        {
            return this.preferredLineHeight;
        }
        return boxes__56949.Single().toRect().height;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isNewlineAtOffset(long offset) => (((0L <= offset) && (offset < this.plainText.Length)) && WordBoundary._isNewline(this.plainText.codeUnitAt(offset)));
    internal virtual _LineCaretMetrics__text_painter? _computeCaretMetrics(TextPosition position)
    {
        DartRuntimePrimitives.Assert(() => this._debugAssertTextLayoutIsValid);
        DartRuntimePrimitives.Assert(() => !this._debugNeedsRelayout);
        _TextPainterLayoutCacheWithOffset__text_painter cachedLayout__60322 = this._layoutCache!;
        if ((((_TextPainterLayoutCacheWithOffset__text_painter)cachedLayout__60322).paragraph.numberOfLines < 1L))
        {
            return null;
        }
        var (offset__60617, anchorToLeadingEdge__60630) = (position switch { global::Doroti.Ui.TextPosition { offset: 0L } __object60679 => (((long, bool))((0L, true))), global::Doroti.Ui.TextPosition { offset: long offset__60878, affinity: TextAffinity.downstream } __object60854 => (((long, bool))((offset__60878, true))), global::Doroti.Ui.TextPosition { offset: long offset__60970, affinity: TextAffinity.upstream } __object60946 when _isNewlineAtOffset((offset__60970 - 1L)) => (((long, bool))((offset__60970, true))), global::Doroti.Ui.TextPosition { offset: long offset__61114, affinity: TextAffinity.upstream } __object61090 => (((long, bool))(((offset__61114 - 1L), false))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        long caretPositionCacheKey__61201 = (anchorToLeadingEdge__60630 ? offset__60617 : (-offset__60617 - 1L));
        if ((caretPositionCacheKey__61201 == ((_TextPainterLayoutCacheWithOffset__text_painter)cachedLayout__60322)._previousCaretPositionKey))
        {
            return this._caretMetrics;
        }
        global::Doroti.Ui.GlyphInfo? glyphInfo__61403 = ((_TextPainterLayoutCacheWithOffset__text_painter)cachedLayout__60322).paragraph.getGlyphInfoAt(offset__60617);
        if ((glyphInfo__61403 is null))
        {
            global::Doroti.Ui.Paragraph template__61807 = _getOrCreateLayoutTemplate();
            DartRuntimePrimitives.Assert(() => (template__61807.numberOfLines == 1L));
            double baselineOffset__61910 = template__61807.getLineMetricsAt(0L)!.baseline;
            return ((_TextPainterLayoutCacheWithOffset__text_painter)cachedLayout__60322).layout._endOfTextCaretMetrics.shift(new global::Doroti.Ui.Offset(0.0, -baselineOffset__61910));
        }
        global::Doroti.Ui.TextRange graphemeRange__62087 = glyphInfo__61403.graphemeClusterCodeUnitRange;
        if (graphemeRange__62087.isCollapsed)
        {
            DartRuntimePrimitives.Assert(() => (graphemeRange__62087.start == 0L));
            return _computeCaretMetrics(new global::Doroti.Ui.TextPosition(offset: (offset__60617 + 1L)));
        }
        if ((anchorToLeadingEdge__60630 && (graphemeRange__62087.start != offset__60617)))
        {
            DartRuntimePrimitives.Assert(() => (graphemeRange__62087.end > (graphemeRange__62087.start + 1L)));
            return _computeCaretMetrics(new global::Doroti.Ui.TextPosition(offset: graphemeRange__62087.end));
        }
        _LineCaretMetrics__text_painter metrics__62869 = default!;
        List<global::Doroti.Ui.TextBox> boxes__62902 = ((_TextPainterLayoutCacheWithOffset__text_painter)cachedLayout__60322).paragraph.getBoxesForRange(graphemeRange__62087.start, graphemeRange__62087.end, boxHeightStyle: Dart_uiLibrary.BoxHeightStyle.strut);
        bool anchorToLeft__63073 = (glyphInfo__61403.writingDirection switch { TextDirection.ltr => anchorToLeadingEdge__60630, TextDirection.rtl => !anchorToLeadingEdge__60630, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Ui.TextBox box__63248 = (anchorToLeft__63073 ? boxes__62902.First() : boxes__62902.Last());
        metrics__62869 = new _LineCaretMetrics__text_painter(offset: new global::Doroti.Ui.Offset((anchorToLeft__63073 ? box__63248.left : box__63248.right), box__63248.top), writingDirection: box__63248.direction, height: (box__63248.bottom - box__63248.top));
        cachedLayout__60322._previousCaretPositionKey = caretPositionCacheKey__61201;
        return _caretMetrics = metrics__62869;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<global::Doroti.Ui.TextBox> getBoxesForSelection(TextSelection selection, BoxHeightStyle boxHeightStyle = BoxHeightStyle.tight, BoxWidthStyle boxWidthStyle = BoxWidthStyle.tight)
    {
        DartRuntimePrimitives.Assert(() => this._debugAssertTextLayoutIsValid);
        DartRuntimePrimitives.Assert(() => selection.isValid);
        DartRuntimePrimitives.Assert(() => !this._debugNeedsRelayout);
        _TextPainterLayoutCacheWithOffset__text_painter cachedLayout__64821 = this._layoutCache!;
        global::Doroti.Ui.Offset offset__64868 = ((_TextPainterLayoutCacheWithOffset__text_painter)cachedLayout__64821).paintOffset;
        if ((!double.IsFinite(offset__64868.dx) || !double.IsFinite(offset__64868.dy)))
        {
            return new List<global::Doroti.Ui.TextBox>();
        }
        List<global::Doroti.Ui.TextBox> boxes__65013 = ((_TextPainterLayoutCacheWithOffset__text_painter)cachedLayout__64821).paragraph.getBoxesForRange(selection.start, selection.end, boxHeightStyle: boxHeightStyle, boxWidthStyle: boxWidthStyle);
        return ((object.Equals(offset__64868, Offset.zero)) ? boxes__65013 : boxes__65013.map<TextBox, TextBox>(((box) => _shiftTextBox(box, offset__64868))).ToList());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.GlyphInfo? getClosestGlyphForOffset(Offset offset)
    {
        DartRuntimePrimitives.Assert(() => this._debugAssertTextLayoutIsValid);
        DartRuntimePrimitives.Assert(() => !this._debugNeedsRelayout);
        _TextPainterLayoutCacheWithOffset__text_painter cachedLayout__65854 = this._layoutCache!;
        global::Doroti.Ui.GlyphInfo? rawGlyphInfo__65908 = ((_TextPainterLayoutCacheWithOffset__text_painter)cachedLayout__65854).paragraph.getClosestGlyphInfoForOffset((offset - ((_TextPainterLayoutCacheWithOffset__text_painter)cachedLayout__65854).paintOffset));
        if (((rawGlyphInfo__65908 is null) || (object.Equals(((_TextPainterLayoutCacheWithOffset__text_painter)cachedLayout__65854).paintOffset, Offset.zero))))
        {
            return rawGlyphInfo__65908;
        }
        return new global::Doroti.Ui.GlyphInfo(rawGlyphInfo__65908.graphemeClusterLayoutBounds.shift(((_TextPainterLayoutCacheWithOffset__text_painter)cachedLayout__65854).paintOffset), rawGlyphInfo__65908.graphemeClusterCodeUnitRange, rawGlyphInfo__65908.writingDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.TextPosition getPositionForOffset(Offset offset)
    {
        DartRuntimePrimitives.Assert(() => this._debugAssertTextLayoutIsValid);
        DartRuntimePrimitives.Assert(() => !this._debugNeedsRelayout);
        _TextPainterLayoutCacheWithOffset__text_painter cachedLayout__66588 = this._layoutCache!;
        return ((_TextPainterLayoutCacheWithOffset__text_painter)cachedLayout__66588).paragraph.getPositionForOffset((offset - ((_TextPainterLayoutCacheWithOffset__text_painter)cachedLayout__66588).paintOffset));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.TextRange getWordBoundary(TextPosition position)
    {
        DartRuntimePrimitives.Assert(() => this._debugAssertTextLayoutIsValid);
        return this._layoutCache!.paragraph.getWordBoundary(position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual WordBoundary wordBoundaries => new WordBoundary(this.text!, this._layoutCache!.paragraph);
    public virtual global::Doroti.Ui.TextRange getLineBoundary(TextPosition position)
    {
        DartRuntimePrimitives.Assert(() => this._debugAssertTextLayoutIsValid);
        return this._layoutCache!.paragraph.getLineBoundary(position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.LineMetrics _shiftLineMetrics(LineMetrics metrics, Offset offset)
    {
        DartRuntimePrimitives.Assert(() => double.IsFinite(offset.dx));
        DartRuntimePrimitives.Assert(() => double.IsFinite(offset.dy));
        return new global::Doroti.Ui.LineMetrics(hardBreak: metrics.hardBreak, ascent: metrics.ascent, descent: metrics.descent, unscaledAscent: metrics.unscaledAscent, height: metrics.height, width: metrics.width, left: (metrics.left + offset.dx), baseline: (metrics.baseline + offset.dy), lineNumber: metrics.lineNumber);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.TextBox _shiftTextBox(TextBox box, Offset offset)
    {
        DartRuntimePrimitives.Assert(() => double.IsFinite(offset.dx));
        DartRuntimePrimitives.Assert(() => double.IsFinite(offset.dy));
        return new global::Doroti.Ui.TextBox((box.left + offset.dx), (box.top + offset.dy), (box.right + offset.dx), (box.bottom + offset.dy), box.direction);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<global::Doroti.Ui.LineMetrics> computeLineMetrics()
    {
        DartRuntimePrimitives.Assert(() => this._debugAssertTextLayoutIsValid);
        DartRuntimePrimitives.Assert(() => !this._debugNeedsRelayout);
        _TextPainterLayoutCacheWithOffset__text_painter layout__69725 = this._layoutCache!;
        global::Doroti.Ui.Offset offset__69766 = ((_TextPainterLayoutCacheWithOffset__text_painter)layout__69725).paintOffset;
        if ((!double.IsFinite(offset__69766.dx) || !double.IsFinite(offset__69766.dy)))
        {
            return new List<global::Doroti.Ui.LineMetrics>();
        }
        List<global::Doroti.Ui.LineMetrics> rawMetrics__69925 = ((_TextPainterLayoutCacheWithOffset__text_painter)layout__69725).lineMetrics;
        return ((object.Equals(offset__69766, Offset.zero)) ? rawMetrics__69925 : rawMetrics__69925.map<LineMetrics, LineMetrics>(((metrics) => _shiftLineMetrics(metrics, offset__69766))).ToList());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool debugDisposed
    {
        get
        {
            bool? disposed__70328 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    disposed__70328 = this._disposed;
                    return true;
                });
            return (disposed__70328 ?? throw new InvalidOperationException("debugDisposed only available when asserts are on."));
            return default!;
        }
    }
    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => !this.debugDisposed);
        DartRuntimePrimitives.Assert(() =>
            {
                _disposed = true;
                return true;
            });
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this._layoutTemplate?.dispose();
        _layoutTemplate = null;
        this._layoutCache?.paragraph.dispose();
        _layoutCache = null;
        _text = null;
    }

}

internal class _UnspecifiedTextScaler__text_painter : TextScaler
{
    internal _UnspecifiedTextScaler__text_painter()
    {
    }

    public override double textScaleFactor => throw new NotImplementedException();
    public override double scale(double fontSize) => throw new NotImplementedException();
}

