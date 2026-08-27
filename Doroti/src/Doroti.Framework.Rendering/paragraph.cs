// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/paragraph.dart
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

internal delegate void _TextBoundaryRecord__paragraph();

internal delegate (TextPosition boundaryEnd, TextPosition boundaryStart) _TextBoundaryAtPosition__paragraph(TextPosition position);

internal delegate (TextPosition boundaryEnd, TextPosition boundaryStart) _TextBoundaryAtPositionInText__paragraph(TextPosition position, string text);

public static partial class ParagraphLibrary
{
    internal static string _kEllipsis = "…";
}

public class PlaceholderSpanIndexSemanticsTag : global::Doroti.Framework.Semantics.SemanticsTag
{
    public virtual long index { get; private set; } = default!;

    public PlaceholderSpanIndexSemanticsTag(long index) : base($"PlaceholderSpanIndexSemanticsTag({index})")
    {
        this.index = index;
    }

    public override bool Equals(object? other)
    {
        var __other = other as PlaceholderSpanIndexSemanticsTag;
        if (__other is null) return false;
        return ((__other is PlaceholderSpanIndexSemanticsTag) && (((PlaceholderSpanIndexSemanticsTag)((PlaceholderSpanIndexSemanticsTag)__other)).index == this.index));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(typeof(PlaceholderSpanIndexSemanticsTag), this.index);
}

public class TextParentData : ParentData, ContainerParentDataMixin<RenderBox>
{
    internal virtual Offset? _offset { get; set; } = default;
    public virtual global::Doroti.Framework.Painting.PlaceholderSpan? span { get; set; } = default;
    public virtual RenderBox? previousSibling { get; set; } = default;
    public virtual RenderBox? nextSibling { get; set; } = default;

    public virtual global::Doroti.Ui.Offset? offset => this._offset;
    public override void detach()
    {
        span = null;
        _offset = null;
        DartRuntimePrimitives.Assert(() => (this.previousSibling is null));
        DartRuntimePrimitives.Assert(() => (this.nextSibling is null));
        base.detach();
    }

    public override string ToString() => $"widget: {this.span}, {((this.offset is null) ? "not laid out" : $"offset: {this.offset}")}";
}

public interface RenderInlineChildrenContainerDefaults
{
    public void setupParentData(RenderObject child);
    public static global::Doroti.Framework.Painting.PlaceholderDimensions _layoutChild(RenderBox child, BoxConstraints childConstraints, Func<RenderBox, BoxConstraints, Size> layoutChild, Func<RenderBox, BoxConstraints, TextBaseline, double?> getBaseline)
    {
        var parentDataLocal = ((TextParentData?)(object?)child.parentData!)!;
        global::Doroti.Framework.Painting.PlaceholderSpan? spanLocal = ((TextParentData)parentDataLocal).span;
        DartRuntimePrimitives.Assert(() => (spanLocal is not null));
        return ((spanLocal is null) ? global::Doroti.Framework.Painting.PlaceholderDimensions.empty : new global::Doroti.Framework.Painting.PlaceholderDimensions(size: layoutChild(child, childConstraints), alignment: ((global::Doroti.Framework.Painting.PlaceholderSpan)spanLocal).alignment, baseline: ((global::Doroti.Framework.Painting.PlaceholderSpan)spanLocal).baseline, baselineOffset: (((global::Doroti.Framework.Painting.PlaceholderSpan)spanLocal).alignment switch { Dart_uiLibrary.PlaceholderAlignment.aboveBaseline or Dart_uiLibrary.PlaceholderAlignment.belowBaseline or Dart_uiLibrary.PlaceholderAlignment.bottom or Dart_uiLibrary.PlaceholderAlignment.middle => null, Dart_uiLibrary.PlaceholderAlignment.top => null, Dart_uiLibrary.PlaceholderAlignment.baseline => getBaseline(child, childConstraints, DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Painting.PlaceholderSpan)spanLocal).baseline)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })));
    }
    public List<global::Doroti.Framework.Painting.PlaceholderDimensions> layoutInlineChildren(double maxWidth, Func<RenderBox, BoxConstraints, Size> layoutChild, Func<RenderBox, BoxConstraints, TextBaseline, double?> getChildBaseline);
    public void positionInlineChildren(List<TextBox> boxes);
    public void defaultApplyPaintTransform(RenderBox child, Matrix4 transform);
    public void paintInlineChildren(PaintingContext context, Offset offset);
    public bool hitTestInlineChildren(BoxHitTestResult result, Offset position);
}

internal class _UnspecifiedTextScaler__paragraph : global::Doroti.Framework.Painting.TextScaler
{
    internal _UnspecifiedTextScaler__paragraph()
    {
    }

    public override double textScaleFactor => throw new NotImplementedException();
    public override double scale(double fontSize) => throw new NotImplementedException();
}

public class RenderParagraph : RenderBox, ContainerRenderObjectMixin<RenderBox, TextParentData>, RenderInlineChildrenContainerDefaults, RelayoutWhenSystemFontsChangeMixin
{
    internal static string _placeholderCharacter = char.ConvertFromUtf32(checked((int)global::Doroti.Framework.Painting.PlaceholderSpan.placeholderCodeUnit));
    internal virtual global::Doroti.Framework.Painting.TextPainter _textPainter { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Painting.TextPainter? _textIntrinsicsCache { get; set; } = default;
    internal virtual List<global::Doroti.Framework.Semantics.AttributedString>? _cachedAttributedLabels { get; set; } = default;
    internal virtual List<global::Doroti.Framework.Painting.InlineSpanSemanticsInformation>? _cachedCombinedSemanticsInfos { get; set; } = default;
    internal virtual List<_SelectableFragment__paragraph>? _lastSelectableFragments { get; set; } = default;
    internal virtual SelectionRegistrar? _registrar { get; set; } = default;
    internal virtual bool _softWrap { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.TextOverflow _overflow { get; set; } = default!;
    internal virtual double _devicePixelRatio { get; set; } = default!;
    internal virtual Color? _selectionColor { get; set; } = default;
    internal virtual bool _needsClipping { get; set; } = false;
    internal virtual Shader? _overflowShader { get; set; } = default;
    internal virtual List<global::Doroti.Framework.Painting.PlaceholderDimensions>? _placeholderDimensions { get; set; } = default;
    internal virtual List<global::Doroti.Framework.Painting.InlineSpanSemanticsInformation>? _semanticsInfo { get; set; } = default;
    internal virtual DartMap<Key, global::Doroti.Framework.Semantics.SemanticsNode>? _cachedChildNodes { get; set; } = default;
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;
    public virtual bool _hasPendingSystemFontsDidChangeCallBack { get; set; } = false;

    public RenderParagraph(global::Doroti.Framework.Painting.InlineSpan text, TextAlign textAlign = TextAlign.start, TextDirection textDirection = default!, bool softWrap = true, global::Doroti.Framework.Painting.TextOverflow overflow = TextOverflow.clip, double textScaleFactor = 1.0, global::Doroti.Framework.Painting.TextScaler textScaler = default!, long? maxLines = null, Locale? locale = null, global::Doroti.Framework.Painting.StrutStyle? strutStyle = null, global::Doroti.Framework.Painting.TextWidthBasis textWidthBasis = TextWidthBasis.parent, TextHeightBehavior? textHeightBehavior = null, List<RenderBox>? children = null, Color? selectionColor = null, SelectionRegistrar? registrar = null, double devicePixelRatio = 1.0)
    {
        global::Doroti.Framework.Painting.TextScaler __textScaler = textScaler ?? new _UnspecifiedTextScaler__paragraph();
        this._softWrap = softWrap;
        this._overflow = overflow;
        this._devicePixelRatio = devicePixelRatio;
        this._selectionColor = selectionColor;
        this._textPainter = new global::Doroti.Framework.Painting.TextPainter(text: text, textAlign: textAlign, textDirection: textDirection, textScaler: ((object.Equals(textScaler, new _UnspecifiedTextScaler__paragraph())) ? global::Doroti.Framework.Painting.TextScaler.CreateLinear(textScaleFactor) : textScaler), maxLines: maxLines, ellipsis: ((object.Equals(overflow, global::Doroti.Framework.Painting.TextOverflow.ellipsis)) ? ParagraphLibrary._kEllipsis : null), locale: locale, strutStyle: strutStyle, textWidthBasis: textWidthBasis, textHeightBehavior: textHeightBehavior);
        System.Diagnostics.Debug.Assert(text.debugAssertIsValid());
        System.Diagnostics.Debug.Assert(((maxLines is null) || (DartRuntimePrimitives.RequireValue(maxLines) > 0L)));
        System.Diagnostics.Debug.Assert((DartRuntimePrimitives.Identical(__textScaler, new _UnspecifiedTextScaler__paragraph()) || (textScaleFactor == 1.0)));
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
    public virtual global::Doroti.Framework.Painting.InlineSpan text
    {
        get => ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).text!;
        set
        {
            var __value = value;
            switch (((global::Doroti.Framework.Painting.TextPainter)this._textPainter).text!.compareTo(__value))
            {
                case global::Doroti.Framework.Painting.RenderComparison.identical:
                    {
                        return;
                    }
                case global::Doroti.Framework.Painting.RenderComparison.metadata:
                    {
                        this._textPainter.text = __value;
                        _cachedCombinedSemanticsInfos = null;
                        markNeedsSemanticsUpdate();
                        break;
                    }
                case global::Doroti.Framework.Painting.RenderComparison.paint:
                    {
                        this._textPainter.text = __value;
                        _cachedAttributedLabels = null;
                        _cachedCombinedSemanticsInfos = null;
                        markNeedsPaint();
                        markNeedsSemanticsUpdate();
                        break;
                    }
                case global::Doroti.Framework.Painting.RenderComparison.layout:
                    {
                        this._textPainter.text = __value;
                        _overflowShader = null;
                        _cachedAttributedLabels = null;
                        _cachedCombinedSemanticsInfos = null;
                        markNeedsLayout();
                        _removeSelectionRegistrarSubscription();
                        _disposeSelectableFragments();
                        _updateSelectionRegistrarSubscription();
                        break;
                    }
            }
        }
    }
    public virtual List<TextSelection> selections
    {
        get
        {
            if ((this._lastSelectableFragments is null))
            {
                return new List<TextSelection>();
            }
            var results = new List<TextSelection>();
            foreach (_SelectableFragment__paragraph fragment in this._lastSelectableFragments!)
            {
                if (((((_SelectableFragment__paragraph)fragment)._textSelectionStart is not null) && (((_SelectableFragment__paragraph)fragment)._textSelectionEnd is not null)))
                {
                    results.Add(new TextSelection(baseOffset: ((_SelectableFragment__paragraph)fragment)._textSelectionStart!.offset, extentOffset: ((_SelectableFragment__paragraph)fragment)._textSelectionEnd!.offset));
                }
            }
            return results;
            return default!;
        }
    }
    public virtual SelectionRegistrar? registrar
    {
        get => this._registrar;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._registrar)))
            {
                return;
            }
            _removeSelectionRegistrarSubscription();
            _disposeSelectableFragments();
            _registrar = __value;
            _updateSelectionRegistrarSubscription();
        }
    }
    internal virtual void _updateSelectionRegistrarSubscription()
    {
        if ((this._registrar is null))
        {
            return;
        }
        _lastSelectableFragments ??= _getSelectableFragments();
        this._lastSelectableFragments!.forEach(this._registrar!.add);
        if ((checked((long)(this._lastSelectableFragments!.Count)) != 0))
        {
            markNeedsCompositingBitsUpdate();
        }
    }

    internal virtual void _removeSelectionRegistrarSubscription()
    {
        if (((this._registrar is null) || (this._lastSelectableFragments is null)))
        {
            return;
        }
        this._lastSelectableFragments!.forEach(this._registrar!.remove);
    }

    internal virtual List<_SelectableFragment__paragraph> _getSelectableFragments()
    {
        string plainText = this.text.toPlainText(includeSemanticsLabels: false);
        var result = new List<_SelectableFragment__paragraph>();
        var startLocal = 0L;
        while ((startLocal < plainText.Length))
        {
            long endLocal = plainText.IndexOf(_placeholderCharacter, checked((int)(startLocal)));
            if ((startLocal != endLocal))
            {
                if ((endLocal == -1L))
                {
                    endLocal = plainText.Length;
                }
                result.Add(new _SelectableFragment__paragraph(paragraph: this, range: new global::Doroti.Ui.TextRange(start: startLocal, end: endLocal), fullText: plainText));
                startLocal = endLocal;
            }
            startLocal += 1L;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool selectableBelongsToParagraph(Selectable selectable)
    {
        if ((this._lastSelectableFragments is null))
        {
            return false;
        }
        return this._lastSelectableFragments!.Contains(selectable);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _disposeSelectableFragments()
    {
        if ((this._lastSelectableFragments is null))
        {
            return;
        }
        foreach (_SelectableFragment__paragraph fragment in this._lastSelectableFragments!)
        {
            fragment.dispose();
        }
        _lastSelectableFragments = null;
    }

    public override bool alwaysNeedsCompositing => ((((long?)(this._lastSelectableFragments?.Count)) is { } __count19316 ? __count19316 != 0 : (bool?)null) ?? false);
    public override void markNeedsLayout()
    {
        this._lastSelectableFragments?.forEach(((element) => element.didChangeParagraphLayout()));
        base.markNeedsLayout();
    }

    public override void dispose()
    {
        _removeSelectionRegistrarSubscription();
        _disposeSelectableFragments();
        this._textPainter.dispose();
        this._textIntrinsicsCache?.dispose();
        base.dispose();
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
            markNeedsPaint();
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
        }
    }
    public virtual bool softWrap
    {
        get => this._softWrap;
        set
        {
            var __value = value;
            if ((this._softWrap == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _softWrap = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.TextOverflow overflow
    {
        get => this._overflow;
        set
        {
            var __value = value;
            if ((object.Equals(this._overflow, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _overflow = DartRuntimePrimitives.RequireValue(__value);
            this._textPainter.ellipsis = ((object.Equals(DartRuntimePrimitives.RequireValue(__value), global::Doroti.Framework.Painting.TextOverflow.ellipsis)) ? ParagraphLibrary._kEllipsis : null);
            markNeedsLayout();
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
            _overflowShader = null;
            markNeedsLayout();
        }
    }
    public virtual double devicePixelRatio
    {
        get => this._devicePixelRatio;
        set
        {
            var __value = value;
            if ((this._devicePixelRatio == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _devicePixelRatio = DartRuntimePrimitives.RequireValue(__value);
            if (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb)
            {
                markNeedsPaint();
            }
        }
    }
    public virtual long? maxLines
    {
        get => ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).maxLines;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((__value is null) || (DartRuntimePrimitives.RequireValue(__value) > 0L)));
            if ((((global::Doroti.Framework.Painting.TextPainter)this._textPainter).maxLines == __value))
            {
                return;
            }
            this._textPainter.maxLines = __value;
            _overflowShader = null;
            markNeedsLayout();
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
            _overflowShader = null;
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
            _overflowShader = null;
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
            _overflowShader = null;
            markNeedsLayout();
        }
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
            _overflowShader = null;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.Color? selectionColor
    {
        get => this._selectionColor;
        set
        {
            var __value = value is null ? null : (Color)(object)value;
            if ((object.Equals(this._selectionColor, __value)))
            {
                return;
            }
            _selectionColor = __value;
            if ((this._lastSelectableFragments?.any(((fragment) => ((_SelectableFragment__paragraph)fragment).value.hasSelection)) ?? false))
            {
                markNeedsPaint();
            }
        }
    }
    internal virtual global::Doroti.Ui.Offset _getOffsetForPosition(TextPosition position)
    {
        return (getOffsetForCaret(position, Rect.zero) + new global::Doroti.Ui.Offset(0, getFullHeightForCaret(position)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        List<global::Doroti.Framework.Painting.PlaceholderDimensions> placeholderDimensions = layoutInlineChildren(double.PositiveInfinity, ((Func<RenderBox, BoxConstraints, Size>)((child, constraints) => new global::Doroti.Ui.Size(child.getMinIntrinsicWidth(double.PositiveInfinity), 0.0))), (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline);
        return (((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textIntrinsics;
    __cascade.setPlaceholderDimensions(placeholderDimensions);
    __cascade.layout();
    return __cascade;
}))()).minIntrinsicWidth;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        List<global::Doroti.Framework.Painting.PlaceholderDimensions> placeholderDimensions = layoutInlineChildren(double.PositiveInfinity, ((Func<RenderBox, BoxConstraints, Size>)((child, constraints) => new global::Doroti.Ui.Size(child.getMaxIntrinsicWidth(double.PositiveInfinity), 0.0))), (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline);
        return (((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textIntrinsics;
    __cascade.setPlaceholderDimensions(placeholderDimensions);
    __cascade.layout();
    return __cascade;
}))()).maxIntrinsicWidth;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double preferredLineHeight => ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).preferredLineHeight;
    internal virtual double _computeIntrinsicHeight(double width)
    {
        return (((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textIntrinsics;
    __cascade.setPlaceholderDimensions(layoutInlineChildren(width, (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.dryLayoutChild, (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline));
    __cascade.layout(minWidth: width, maxWidth: _adjustMaxWidth(width));
    return __cascade;
}))()).height;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return _computeIntrinsicHeight(width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return _computeIntrinsicHeight(width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestSelf(Offset position) => true;
    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        global::Doroti.Ui.GlyphInfo? glyph = this._textPainter.getClosestGlyphForOffset(position);
        global::Doroti.Framework.Painting.InlineSpan? spanHit = (((glyph is not null) && glyph.graphemeClusterLayoutBounds.contains(position)) ? ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).text!.getSpanForPosition(new global::Doroti.Ui.TextPosition(offset: glyph.graphemeClusterCodeUnitRange.start)) : null);
        switch (spanHit)
        {
            case HitTestTarget span:
                {
                    result.add(new HitTestEntry<HitTestTarget>(span));
                    return true;
                }
            default:
                {
                    return hitTestInlineChildren(result, position);
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool debugHasOverflowShader => (this._overflowShader is not null);
    public virtual void systemFontsDidChange()
    {
        markNeedsLayout();
        this._textPainter.markNeedsLayout();
    }

    internal virtual double _adjustMaxWidth(double maxWidth)
    {
        return ((this.softWrap || (object.Equals(this.overflow, global::Doroti.Framework.Painting.TextOverflow.ellipsis))) ? maxWidth : double.PositiveInfinity);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _layoutTextWithConstraints(BoxConstraints constraints)
    {
        ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textPainter;
    __cascade.setPlaceholderDimensions(this._placeholderDimensions);
    __cascade.layout(minWidth: ((BoxConstraints)constraints).minWidth, maxWidth: _adjustMaxWidth(((BoxConstraints)constraints).maxWidth));
    return __cascade;
}))();
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        global::Doroti.Ui.Size sizeLocal = (((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textIntrinsics;
    __cascade.setPlaceholderDimensions(layoutInlineChildren(((BoxConstraints)constraints).maxWidth, (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.dryLayoutChild, (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline));
    __cascade.layout(minWidth: ((BoxConstraints)constraints).minWidth, maxWidth: _adjustMaxWidth(((BoxConstraints)constraints).maxWidth));
    return __cascade;
}))()).size;
        return constraints.constrain(sizeLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        DartRuntimePrimitives.Assert(() => constraints.debugAssertIsValid());
        _layoutTextWithConstraints(constraints);
        return this._textPainter.computeDistanceToActualBaseline(TextBaseline.alphabetic);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => constraints.debugAssertIsValid());
        ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textIntrinsics;
    __cascade.setPlaceholderDimensions(layoutInlineChildren(((BoxConstraints)constraints).maxWidth, (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.dryLayoutChild, (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline));
    __cascade.layout(minWidth: ((BoxConstraints)constraints).minWidth, maxWidth: _adjustMaxWidth(((BoxConstraints)constraints).maxWidth));
    return __cascade;
}))();
        return this._textIntrinsics.computeDistanceToActualBaseline(TextBaseline.alphabetic);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        this._lastSelectableFragments?.forEach(((element) => element.didChangeParagraphLayout()));
        BoxConstraints constraintsLocal = this.constraints;
        _placeholderDimensions = layoutInlineChildren(((BoxConstraints)constraintsLocal).maxWidth, (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.layoutChild, (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getBaseline);
        _layoutTextWithConstraints(constraintsLocal);
        positionInlineChildren(((global::Doroti.Framework.Painting.TextPainter)this._textPainter).inlinePlaceholderBoxes!);
        global::Doroti.Ui.Size textSize = ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).size;
        size = constraintsLocal.constrain(textSize);
        bool didOverflowHeight = ((size.height < textSize.height) || ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).didExceedMaxLines);
        bool didOverflowWidth = (size.width < textSize.width);
        bool hasVisualOverflow = (didOverflowWidth || didOverflowHeight);
        if (hasVisualOverflow)
        {
            switch (this._overflow)
            {
                case global::Doroti.Framework.Painting.TextOverflow.visible:
                    {
                        _needsClipping = false;
                        _overflowShader = null;
                        break;
                    }
                case global::Doroti.Framework.Painting.TextOverflow.clip:
                case global::Doroti.Framework.Painting.TextOverflow.ellipsis:
                    {
                        _needsClipping = true;
                        _overflowShader = null;
                        break;
                    }
                case global::Doroti.Framework.Painting.TextOverflow.fade:
                    {
                        _needsClipping = true;
                        var fadeSizePainter = ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = new global::Doroti.Framework.Painting.TextPainter(text: new global::Doroti.Framework.Painting.TextSpan(style: ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).text!.style, text: "…"), textDirection: this.textDirection, textScaler: this.textScaler, locale: this.locale);
    __cascade.layout();
    return __cascade;
}))();
                        if (didOverflowWidth)
                        {
                            var (fadeStart, fadeEnd) = (this.textDirection switch { TextDirection.rtl => (((double, double))((((global::Doroti.Framework.Painting.TextPainter)fadeSizePainter).width, 0.0))), TextDirection.ltr => (((double, double))(((size.width - ((global::Doroti.Framework.Painting.TextPainter)fadeSizePainter).width), size.width))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                            _overflowShader = global::Doroti.Ui.Gradient.linear(new global::Doroti.Ui.Offset(fadeStart, 0.0), new global::Doroti.Ui.Offset(fadeEnd, 0.0), new List<global::Doroti.Ui.Color> { new global::Doroti.Ui.Color(4294967295L), new global::Doroti.Ui.Color(16777215L) });
                        }
                        else
                        {
                            double fadeEndLocal = size.height;
                            double fadeStartLocal = (fadeEndLocal - (((global::Doroti.Framework.Painting.TextPainter)fadeSizePainter).height / 2.0));
                            _overflowShader = global::Doroti.Ui.Gradient.linear(new global::Doroti.Ui.Offset(0.0, fadeStartLocal), new global::Doroti.Ui.Offset(0.0, fadeEndLocal), new List<global::Doroti.Ui.Color> { new global::Doroti.Ui.Color(4294967295L), new global::Doroti.Ui.Color(16777215L) });
                        }
                        fadeSizePainter.dispose();
                        break;
                    }
            }
        }
        else
        {
            _needsClipping = false;
            _overflowShader = null;
        }
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)(object)child;
        defaultApplyPaintTransform(__child, transform);
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        _layoutTextWithConstraints(constraints);
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Framework.Rendering.DebugLibrary.debugRepaintTextRainbowEnabled)
                {
                    var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = global::Doroti.Framework.Rendering.DebugLibrary.debugCurrentRepaintColor.toColor();
    return __cascade;
}))();
                    ((PaintingContext)context).canvas.drawRect((offset & size), paintLocal);
                }
                return true;
            });
        if ((this._lastSelectableFragments is not null))
        {
            if (this._needsClipping)
            {
                ((PaintingContext)context).canvas.save();
                ((PaintingContext)context).canvas.clipRect((offset & size));
            }
            foreach (_SelectableFragment__paragraph fragment in this._lastSelectableFragments!)
            {
                fragment.paintSelection(context, offset);
            }
            if (this._needsClipping)
            {
                ((PaintingContext)context).canvas.restore();
            }
        }
        if (this._needsClipping)
        {
            global::Doroti.Ui.Rect bounds = (offset & size);
            if ((this._overflowShader is not null))
            {
                ((PaintingContext)context).canvas.saveLayer(bounds, new global::Doroti.Ui.Paint());
            }
            else
            {
                ((PaintingContext)context).canvas.save();
            }
            ((PaintingContext)context).canvas.clipRect(bounds);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                this._textPainter.debugPaintTextLayoutBoxes = global::Doroti.Framework.Rendering.DebugLibrary.debugPaintTextLayoutBoxes;
                return true;
            });
        this._textPainter.paint(((PaintingContext)context).canvas, offset);
        paintInlineChildren(context, offset);
        if (this._needsClipping)
        {
            if ((this._overflowShader is not null))
            {
                ((PaintingContext)context).canvas.translate(offset.dx, offset.dy);
                var paintAlternate = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.blendMode = BlendMode.modulate;
    __cascade.shader = this._overflowShader;
    return __cascade;
}))();
                ((PaintingContext)context).canvas.drawRect((Offset.zero & size), paintAlternate);
            }
            ((PaintingContext)context).canvas.restore();
        }
        if ((this._lastSelectableFragments is not null))
        {
            foreach (_SelectableFragment__paragraph fragmentLocal in this._lastSelectableFragments!)
            {
                fragmentLocal.paintHandles(context, offset);
            }
        }
    }

    public virtual global::Doroti.Ui.Offset getOffsetForCaret(TextPosition position, Rect caretPrototype)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        _layoutTextWithConstraints(constraints);
        return this._textPainter.getOffsetForCaret(position, caretPrototype);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getFullHeightForCaret(TextPosition position)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        _layoutTextWithConstraints(constraints);
        return this._textPainter.getFullHeightForCaret(position, Rect.zero);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<global::Doroti.Ui.TextBox> getBoxesForSelection(TextSelection selection, BoxHeightStyle boxHeightStyle = BoxHeightStyle.tight, BoxWidthStyle boxWidthStyle = BoxWidthStyle.tight)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        _layoutTextWithConstraints(constraints);
        return this._textPainter.getBoxesForSelection(selection, boxHeightStyle: boxHeightStyle, boxWidthStyle: boxWidthStyle);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.TextPosition getPositionForOffset(Offset offset)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        _layoutTextWithConstraints(constraints);
        return this._textPainter.getPositionForOffset(offset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.TextRange getWordBoundary(TextPosition position)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        _layoutTextWithConstraints(constraints);
        return this._textPainter.getWordBoundary(position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextRange _getLineAtOffset(TextPosition position) => this._textPainter.getLineBoundary(position);
    internal virtual global::Doroti.Ui.TextPosition _getTextPositionAbove(TextPosition position)
    {
        double preferredLineHeightLocal = ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).preferredLineHeight;
        double verticalOffset = (-0.5 * preferredLineHeightLocal);
        return _getTextPositionVertical(position, verticalOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _getTextPositionBelow(TextPosition position)
    {
        double preferredLineHeightLocal = ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).preferredLineHeight;
        double verticalOffset = (1.5 * preferredLineHeightLocal);
        return _getTextPositionVertical(position, verticalOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _getTextPositionVertical(TextPosition position, double verticalOffset)
    {
        global::Doroti.Ui.Offset caretOffset = this._textPainter.getOffsetForCaret(position, Rect.zero);
        global::Doroti.Ui.Offset caretOffsetTranslated = caretOffset.translate(0.0, verticalOffset);
        return this._textPainter.getPositionForOffset(caretOffsetTranslated);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Size textSize
    {
        get
        {
            DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
            return ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).size;
            return default!;
        }
    }
    public virtual bool didExceedMaxLines
    {
        get
        {
            DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
            return ((global::Doroti.Framework.Painting.TextPainter)this._textPainter).didExceedMaxLines;
            return default!;
        }
    }
    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        _semanticsInfo = this.text.getSemanticsInformation();
        var needsAssembleSemanticsNode = false;
        var needsChildConfigurationsDelegate = false;
        foreach (global::Doroti.Framework.Painting.InlineSpanSemanticsInformation info in this._semanticsInfo!)
        {
            if (((((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info).recognizer is not null) || (((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info).semanticsIdentifier is not null)))
            {
                needsAssembleSemanticsNode = true;
                break;
            }
            needsChildConfigurationsDelegate = (needsChildConfigurationsDelegate || ((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info).isPlaceholder);
        }
        if (needsAssembleSemanticsNode)
        {
            config.explicitChildNodes = true;
            config.isSemanticBoundary = true;
        }
        else
        {
            if (needsChildConfigurationsDelegate)
            {
                config.childConfigurationsDelegate = this._childSemanticsConfigurationsDelegate;
            }
            else
            {
                if ((this._cachedAttributedLabels is null))
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
                    _cachedAttributedLabels = new List<global::Doroti.Framework.Semantics.AttributedString> { new global::Doroti.Framework.Semantics.AttributedString(buffer.ToString(), attributes: attributesLocal) };
                }
                config.attributedLabel = this._cachedAttributedLabels![(int)(0L)];
                config.textDirection = this.textDirection;
            }
        }
    }

    internal virtual global::Doroti.Framework.Semantics.ChildSemanticsConfigurationsResult _childSemanticsConfigurationsDelegate(List<global::Doroti.Framework.Semantics.SemanticsConfiguration> childConfigs)
    {
        var builder = new global::Doroti.Framework.Semantics.ChildSemanticsConfigurationsResultBuilder();
        var placeholderIndex = 0L;
        var childConfigsIndex = 0L;
        var attributedLabelCacheIndex = 0L;
        _cachedCombinedSemanticsInfos ??= global::Doroti.Framework.Painting.Inline_spanLibrary.combineSemanticsInfo(this._semanticsInfo!);
        foreach (global::Doroti.Framework.Painting.InlineSpanSemanticsInformation info in this._cachedCombinedSemanticsInfos!)
        {
            if (((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info).isPlaceholder)
            {
                while (((childConfigsIndex < checked((long)(childConfigs.Count))) && _childConfigBelongsToPlaceholder(childConfigs[(int)(childConfigsIndex)], placeholderIndex)))
                {
                    builder.markAsMergeUp(childConfigs[(int)(childConfigsIndex)]);
                    childConfigsIndex += 1L;
                }
                placeholderIndex += 1L;
            }
            else
            {
                builder.markAsMergeUp(_createSemanticsConfigForTextInfo(info, attributedLabelCacheIndex));
                attributedLabelCacheIndex += 1L;
            }
        }
        return builder.build();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static bool _childConfigBelongsToPlaceholder(global::Doroti.Framework.Semantics.SemanticsConfiguration childConfig, long placeholderIndex)
    {
        IEnumerable<global::Doroti.Framework.Semantics.SemanticsTag>? tags = ((global::Doroti.Framework.Semantics.SemanticsConfiguration)childConfig).tagsForChildren;
        if ((tags is null))
        {
            return false;
        }
        foreach (global::Doroti.Framework.Semantics.SemanticsTag tag in tags)
        {
            if ((tag is PlaceholderSpanIndexSemanticsTag))
            {
                PlaceholderSpanIndexSemanticsTag tag__45698__as45723 = (PlaceholderSpanIndexSemanticsTag)tag;
                return (((PlaceholderSpanIndexSemanticsTag)((PlaceholderSpanIndexSemanticsTag)tag__45698__as45723)).index == placeholderIndex);
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Semantics.SemanticsConfiguration _createSemanticsConfigForTextInfo(global::Doroti.Framework.Painting.InlineSpanSemanticsInformation textInfo, long cacheIndex)
    {
        DartRuntimePrimitives.Assert(() => !((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)textInfo).requiresOwnNode);
        List<global::Doroti.Framework.Semantics.AttributedString> cachedStrings = _cachedAttributedLabels ??= new List<global::Doroti.Framework.Semantics.AttributedString>();
        DartRuntimePrimitives.Assert(() => (cacheIndex <= checked((long)(cachedStrings.Count))));
        bool hasCache = (cacheIndex < checked((long)(cachedStrings.Count)));
        global::Doroti.Framework.Semantics.AttributedString attributedLabelLocal = default!;
        if (hasCache)
        {
            attributedLabelLocal = cachedStrings[(int)(cacheIndex)];
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (checked((long)(cachedStrings.Count)) == cacheIndex));
            attributedLabelLocal = new global::Doroti.Framework.Semantics.AttributedString((((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)textInfo).semanticsLabel ?? ((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)textInfo).text), attributes: ((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)textInfo).stringAttributes);
            cachedStrings.Add(attributedLabelLocal);
        }
        return ((Func<global::Doroti.Framework.Semantics.SemanticsConfiguration>)(() =>
{
    var __cascade = new global::Doroti.Framework.Semantics.SemanticsConfiguration();
    __cascade.textDirection = this.textDirection;
    __cascade.attributedLabel = attributedLabelLocal;
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
                    if ((((TextParentData)parentDataLocal).offset is not null))
                    {
                        newChildren.Add(childNode);
                    }
                    childIndex += 1L;
                }
                child = childAfter(child!);
                placeholderIndex += 1L;
            }
            else
            {
                var initialDirection = currentDirection;
                List<global::Doroti.Ui.TextBox> rects = getBoxesForSelection(selection);
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
    __cascade.identifier = (((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info).semanticsIdentifier ?? "");
    __cascade.attributedLabel = new global::Doroti.Framework.Semantics.AttributedString((((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info).semanticsLabel ?? ((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info).text), attributes: ((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info).stringAttributes);
    return __cascade;
}))();
                switch (((global::Doroti.Framework.Painting.InlineSpanSemanticsInformation)info).recognizer)
                {
                    case TapGestureRecognizer { onTap: Action handler } __object50228:
                        {
                            if ((handler is not null))
                            {
                                configuration.onTap = handler;
                                configuration.isLink = true;
                            }
                            break;
                        }
                    case DoubleTapGestureRecognizer { onDoubleTap: Action handlerLocal } __object50301:
                        {
                            if ((handlerLocal is not null))
                            {
                                configuration.onTap = handlerLocal;
                                configuration.isLink = true;
                            }
                            break;
                        }
                    case LongPressGestureRecognizer { onLongPress: Action onLongPressLocal } __object50523:
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
                if (((((long?)(this._cachedChildNodes?.Count)) is { } __count51134 ? __count51134 != 0 : (bool?)null) ?? false))
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
        DartRuntimePrimitives.Assert(() => (childIndex == children.Count()));
        DartRuntimePrimitives.Assert(() => (child is null));
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

    public override void clearSemantics()
    {
        base.clearSemantics();
        _cachedChildNodes = null;
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        return new List<DiagnosticsNode> { ((Diagnosticable)this.text).toDiagnosticsNode(name: "text", style: DiagnosticsTreeStyle.transition) };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<global::Doroti.Ui.TextAlign>("textAlign", this.textAlign));
        properties.add(new EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection));
        properties.add(new FlagProperty("softWrap", value: this.softWrap, ifTrue: "wrapping at box width", ifFalse: "no wrapping except at line break characters", showName: true));
        properties.add(new EnumProperty<global::Doroti.Framework.Painting.TextOverflow>("overflow", this.overflow));
        properties.add(new DiagnosticsProperty<global::Doroti.Framework.Painting.TextScaler>("textScaler", this.textScaler, defaultValue: global::Doroti.Framework.Painting.TextScaler.noScaling));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Locale>("locale", this.locale, defaultValue: null));
        properties.add(new IntProperty("maxLines", this.maxLines, ifNull: "unlimited"));
        properties.add(new DoubleProperty("devicePixelRatio", this.devicePixelRatio, defaultValue: 1.0));
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
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            child.detach();
            var childParentData = ((TextParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void redepthChildren()
    {
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
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            visitor(child);
            var childParentData = ((TextParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
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

}

internal class _SelectableFragment__paragraph : ChangeNotifier, Selectable, Diagnosticable, TextLayoutMetrics
{
    public virtual TextRange range { get; private set; } = default!;
    public virtual RenderParagraph paragraph { get; private set; } = default!;
    public virtual string fullText { get; private set; } = default!;
    internal virtual TextPosition? _textSelectionStart { get; set; } = default;
    internal virtual TextPosition? _textSelectionEnd { get; set; } = default;
    internal virtual bool _selectableContainsOriginTextBoundary { get; set; } = false;
    internal virtual LayerLink? _startHandleLayerLink { get; set; } = default;
    internal virtual LayerLink? _endHandleLayerLink { get; set; } = default;
    internal virtual SelectionGeometry _selectionGeometry { get; set; } = default!;
    internal static string _placeholderCharacter = char.ConvertFromUtf32(checked((int)global::Doroti.Framework.Painting.PlaceholderSpan.placeholderCodeUnit));
    internal static long _placeholderLength = _placeholderCharacter.Length;
    internal virtual List<Rect>? _cachedBoundingBoxes { get; set; } = default;
    internal virtual Rect? _cachedRect { get; set; } = default;

    internal _SelectableFragment__paragraph(RenderParagraph paragraph, string fullText, TextRange range)
    {
        this.paragraph = paragraph;
        this.fullText = fullText;
        this.range = range;
        System.Diagnostics.Debug.Assert(((range.isValid && !range.isCollapsed) && range.isNormalized));
    }

    public virtual SelectionGeometry value => this._selectionGeometry;
    internal virtual void _updateSelectionGeometry()
    {
        SelectionGeometry newValue = _getSelectionGeometry();
        if ((object.Equals(this._selectionGeometry, newValue)))
        {
            return;
        }
        _selectionGeometry = newValue;
        notifyListeners();
    }

    internal virtual SelectionGeometry _getSelectionGeometry()
    {
        if (((this._textSelectionStart is null) || (this._textSelectionEnd is null)))
        {
            return new SelectionGeometry(status: SelectionStatus.none, hasContent: true);
        }
        long selectionStart = this._textSelectionStart!.offset;
        long selectionEnd = this._textSelectionEnd!.offset;
        bool isReversed = (selectionStart > selectionEnd);
        global::Doroti.Ui.Offset startOffsetInParagraphCoordinates = this.paragraph._getOffsetForPosition(this._textSelectionStart!);
        global::Doroti.Ui.Offset endOffsetInParagraphCoordinates = ((selectionStart == selectionEnd) ? startOffsetInParagraphCoordinates : this.paragraph._getOffsetForPosition(this._textSelectionEnd!));
        var flipHandles = (isReversed != ((object.Equals(TextDirection.rtl, ((RenderParagraph)this.paragraph).textDirection))));
        var selection = new TextSelection(baseOffset: selectionStart, extentOffset: selectionEnd);
        var selectionRectsLocal = new List<global::Doroti.Ui.Rect>();
        foreach (global::Doroti.Ui.TextBox textBox in this.paragraph.getBoxesForSelection(selection))
        {
            selectionRectsLocal.Add(textBox.toRect());
        }
        var selectionCollapsed = (selectionStart == selectionEnd);
        var (startSelectionHandleType, endSelectionHandleType) = ((selectionCollapsed, flipHandles) switch { (true, _) => (((TextSelectionHandleType, TextSelectionHandleType))((TextSelectionHandleType.collapsed, TextSelectionHandleType.collapsed))), (false, true) => (((TextSelectionHandleType, TextSelectionHandleType))((TextSelectionHandleType.right, TextSelectionHandleType.left))), (false, false) => (((TextSelectionHandleType, TextSelectionHandleType))((TextSelectionHandleType.left, TextSelectionHandleType.right))) });
        return new SelectionGeometry(startSelectionPoint: new SelectionPoint(localPosition: startOffsetInParagraphCoordinates, lineHeight: ((RenderParagraph)this.paragraph)._textPainter.preferredLineHeight, handleType: startSelectionHandleType), endSelectionPoint: new SelectionPoint(localPosition: endOffsetInParagraphCoordinates, lineHeight: ((RenderParagraph)this.paragraph)._textPainter.preferredLineHeight, handleType: endSelectionHandleType), selectionRects: selectionRectsLocal, status: (selectionCollapsed ? SelectionStatus.collapsed : SelectionStatus.uncollapsed), hasContent: true);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SelectionResult dispatchSelectionEvent(SelectionEvent @event)
    {
        SelectionResult result = default!;
        global::Doroti.Ui.TextPosition? existingSelectionStart = this._textSelectionStart;
        global::Doroti.Ui.TextPosition? existingSelectionEnd = this._textSelectionEnd;
        switch (((SelectionEvent)@event).type)
        {
            case SelectionEventType.startEdgeUpdate:
            case SelectionEventType.endEdgeUpdate:
                {
                    var edgeUpdate = ((SelectionEdgeUpdateEvent?)(object?)@event)!;
                    TextGranularity granularityLocal = ((SelectionEdgeUpdateEvent)((SelectionEdgeUpdateEvent)@event)).granularity;
                    switch (granularityLocal)
                    {
                        case TextGranularity.character:
                            {
                                result = _updateSelectionEdge(((SelectionEdgeUpdateEvent)edgeUpdate).globalPosition, isEnd: (object.Equals(edgeUpdate.type, SelectionEventType.endEdgeUpdate)));
                                break;
                            }
                        case TextGranularity.word:
                            {
                                result = _updateSelectionEdgeByTextBoundary(((SelectionEdgeUpdateEvent)edgeUpdate).globalPosition, isEnd: (object.Equals(edgeUpdate.type, SelectionEventType.endEdgeUpdate)), getTextBoundary: (Func<TextPosition, (TextPosition boundaryEnd, TextPosition boundaryStart)>)this._getWordBoundaryAtPosition);
                                break;
                            }
                        case TextGranularity.paragraph:
                            {
                                result = _updateSelectionEdgeByMultiSelectableTextBoundary(((SelectionEdgeUpdateEvent)edgeUpdate).globalPosition, isEnd: (object.Equals(edgeUpdate.type, SelectionEventType.endEdgeUpdate)), getTextBoundary: (Func<TextPosition, string, (TextPosition boundaryEnd, TextPosition boundaryStart)>)this._getParagraphBoundaryAtPosition, getClampedTextBoundary: (Func<TextPosition, (TextPosition boundaryEnd, TextPosition boundaryStart)>)this._getClampedParagraphBoundaryAtPosition);
                                break;
                            }
                        case TextGranularity.document:
                        case TextGranularity.line:
                            {
                                DartRuntimePrimitives.Assert(() => false);
                                break;
                            }
                    }
                    break;
                }
            case SelectionEventType.clear:
                {
                    result = _handleClearSelection();
                    break;
                }
            case SelectionEventType.selectAll:
                {
                    result = _handleSelectAll();
                    break;
                }
            case SelectionEventType.selectWord:
                {
                    var selectWordLocal = ((SelectWordSelectionEvent?)(object?)@event)!;
                    result = _handleSelectWord(((SelectWordSelectionEvent)selectWordLocal).globalPosition);
                    break;
                }
            case SelectionEventType.selectParagraph:
                {
                    var selectParagraphLocal = ((SelectParagraphSelectionEvent?)(object?)@event)!;
                    if (((SelectParagraphSelectionEvent)selectParagraphLocal).absorb)
                    {
                        _handleSelectAll();
                        result = SelectionResult.next;
                        _selectableContainsOriginTextBoundary = true;
                    }
                    else
                    {
                        result = _handleSelectParagraph(((SelectParagraphSelectionEvent)selectParagraphLocal).globalPosition);
                    }
                    break;
                }
            case SelectionEventType.granularlyExtendSelection:
                {
                    var granularlyExtendSelectionLocal = ((GranularlyExtendSelectionEvent?)(object?)@event)!;
                    result = _handleGranularlyExtendSelection(((GranularlyExtendSelectionEvent)granularlyExtendSelectionLocal).forward, ((GranularlyExtendSelectionEvent)granularlyExtendSelectionLocal).isEnd, ((GranularlyExtendSelectionEvent)granularlyExtendSelectionLocal).granularity);
                    break;
                }
            case SelectionEventType.directionallyExtendSelection:
                {
                    var directionallyExtendSelectionLocal = ((DirectionallyExtendSelectionEvent?)(object?)@event)!;
                    result = _handleDirectionallyExtendSelection(((DirectionallyExtendSelectionEvent)directionallyExtendSelectionLocal).dx, ((DirectionallyExtendSelectionEvent)directionallyExtendSelectionLocal).isEnd, ((DirectionallyExtendSelectionEvent)directionallyExtendSelectionLocal).direction);
                    break;
                }
        }
        if (((!object.Equals(existingSelectionStart, this._textSelectionStart)) || (!object.Equals(existingSelectionEnd, this._textSelectionEnd))))
        {
            _didChangeSelection();
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SelectedContent? getSelectedContent()
    {
        if (((this._textSelectionStart is null) || (this._textSelectionEnd is null)))
        {
            return null;
        }
        long start = Math.Min(this._textSelectionStart!.offset, this._textSelectionEnd!.offset);
        long end = Math.Max(this._textSelectionStart!.offset, this._textSelectionEnd!.offset);
        return new SelectedContent(plainText: this.fullText.substring(start, end));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SelectedContentRange? getSelection()
    {
        if (((this._textSelectionStart is null) || (this._textSelectionEnd is null)))
        {
            return null;
        }
        return new SelectedContentRange(startOffset: this._textSelectionStart!.offset, endOffset: this._textSelectionEnd!.offset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _didChangeSelection()
    {
        this.paragraph.markNeedsPaint();
        _updateSelectionGeometry();
    }

    internal virtual global::Doroti.Ui.TextPosition _updateSelectionStartEdgeByTextBoundary((TextPosition boundaryEnd, TextPosition boundaryStart)? textBoundary, Func<TextPosition, (TextPosition boundaryEnd, TextPosition boundaryStart)> getTextBoundary, TextPosition position, TextPosition? existingSelectionStart, TextPosition? existingSelectionEnd)
    {
        global::Doroti.Ui.TextPosition? targetPosition = default!;
        if ((textBoundary is not null))
        {
            (TextPosition boundaryEnd, TextPosition boundaryStart) textBoundary__value61232 = DartRuntimePrimitives.RequireValue(textBoundary);
            DartRuntimePrimitives.Assert(() => ((DartRuntimePrimitives.RequireValue(textBoundary__value61232).boundaryStart.offset >= this.range.start) && (DartRuntimePrimitives.RequireValue(textBoundary__value61232).boundaryEnd.offset <= this.range.end)));
            if (((this._selectableContainsOriginTextBoundary && (existingSelectionStart is not null)) && (existingSelectionEnd is not null)))
            {
                var isSamePosition = (position.offset == existingSelectionEnd.offset);
                bool isSelectionInverted = (existingSelectionStart.offset > existingSelectionEnd.offset);
                bool shouldSwapEdges = (!isSamePosition && ((isSelectionInverted != ((position.offset > existingSelectionEnd.offset)))));
                if (shouldSwapEdges)
                {
                    if ((position.offset < existingSelectionEnd.offset))
                    {
                        targetPosition = DartRuntimePrimitives.RequireValue(textBoundary__value61232).boundaryStart;
                    }
                    else
                    {
                        targetPosition = DartRuntimePrimitives.RequireValue(textBoundary__value61232).boundaryEnd;
                    }
                    (TextPosition boundaryEnd, TextPosition boundaryStart) localTextBoundary = getTextBoundary(existingSelectionEnd);
                    DartRuntimePrimitives.Assert(() => ((localTextBoundary.boundaryStart.offset >= this.range.start) && (localTextBoundary.boundaryEnd.offset <= this.range.end)));
                    _setSelectionPosition(((existingSelectionEnd.offset == localTextBoundary.boundaryStart.offset) ? localTextBoundary.boundaryEnd : localTextBoundary.boundaryStart), isEnd: true);
                }
                else
                {
                    if ((position.offset < existingSelectionEnd.offset))
                    {
                        targetPosition = DartRuntimePrimitives.RequireValue(textBoundary__value61232).boundaryStart;
                    }
                    else
                    {
                        if ((position.offset > existingSelectionEnd.offset))
                        {
                            targetPosition = DartRuntimePrimitives.RequireValue(textBoundary__value61232).boundaryEnd;
                        }
                        else
                        {
                            targetPosition = existingSelectionStart;
                        }
                    }
                }
            }
            else
            {
                if ((existingSelectionEnd is not null))
                {
                    if ((position.offset < existingSelectionEnd.offset))
                    {
                        targetPosition = DartRuntimePrimitives.RequireValue(textBoundary__value61232).boundaryStart;
                    }
                    else
                    {
                        targetPosition = DartRuntimePrimitives.RequireValue(textBoundary__value61232).boundaryEnd;
                    }
                }
                else
                {
                    targetPosition = _closestTextBoundary(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(textBoundary__value61232)), position);
                }
            }
        }
        else
        {
            if (((this._selectableContainsOriginTextBoundary && (existingSelectionStart is not null)) && (existingSelectionEnd is not null)))
            {
                var isSamePositionLocal = (position.offset == existingSelectionEnd.offset);
                bool isSelectionInvertedLocal = (existingSelectionStart.offset > existingSelectionEnd.offset);
                bool shouldSwapEdgesLocal = (!isSamePositionLocal && ((isSelectionInvertedLocal != ((position.offset > existingSelectionEnd.offset)))));
                if (shouldSwapEdgesLocal)
                {
                    (TextPosition boundaryEnd, TextPosition boundaryStart) localTextBoundaryLocal = getTextBoundary(existingSelectionEnd);
                    DartRuntimePrimitives.Assert(() => ((localTextBoundaryLocal.boundaryStart.offset >= this.range.start) && (localTextBoundaryLocal.boundaryEnd.offset <= this.range.end)));
                    _setSelectionPosition((isSelectionInvertedLocal ? localTextBoundaryLocal.boundaryEnd : localTextBoundaryLocal.boundaryStart), isEnd: true);
                }
            }
        }
        return (targetPosition ?? position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _updateSelectionEndEdgeByTextBoundary((TextPosition boundaryEnd, TextPosition boundaryStart)? textBoundary, Func<TextPosition, (TextPosition boundaryEnd, TextPosition boundaryStart)> getTextBoundary, TextPosition position, TextPosition? existingSelectionStart, TextPosition? existingSelectionEnd)
    {
        global::Doroti.Ui.TextPosition? targetPosition = default!;
        if ((textBoundary is not null))
        {
            (TextPosition boundaryEnd, TextPosition boundaryStart) textBoundary__value65725 = DartRuntimePrimitives.RequireValue(textBoundary);
            DartRuntimePrimitives.Assert(() => ((DartRuntimePrimitives.RequireValue(textBoundary__value65725).boundaryStart.offset >= this.range.start) && (DartRuntimePrimitives.RequireValue(textBoundary__value65725).boundaryEnd.offset <= this.range.end)));
            if (((this._selectableContainsOriginTextBoundary && (existingSelectionStart is not null)) && (existingSelectionEnd is not null)))
            {
                var isSamePosition = (position.offset == existingSelectionStart.offset);
                bool isSelectionInverted = (existingSelectionStart.offset > existingSelectionEnd.offset);
                bool shouldSwapEdges = (!isSamePosition && ((isSelectionInverted != ((position.offset < existingSelectionStart.offset)))));
                if (shouldSwapEdges)
                {
                    if ((position.offset < existingSelectionStart.offset))
                    {
                        targetPosition = DartRuntimePrimitives.RequireValue(textBoundary__value65725).boundaryStart;
                    }
                    else
                    {
                        targetPosition = DartRuntimePrimitives.RequireValue(textBoundary__value65725).boundaryEnd;
                    }
                    (TextPosition boundaryEnd, TextPosition boundaryStart) localTextBoundary = getTextBoundary(existingSelectionStart);
                    DartRuntimePrimitives.Assert(() => ((localTextBoundary.boundaryStart.offset >= this.range.start) && (localTextBoundary.boundaryEnd.offset <= this.range.end)));
                    _setSelectionPosition(((existingSelectionStart.offset == localTextBoundary.boundaryStart.offset) ? localTextBoundary.boundaryEnd : localTextBoundary.boundaryStart), isEnd: false);
                }
                else
                {
                    if ((position.offset < existingSelectionStart.offset))
                    {
                        targetPosition = DartRuntimePrimitives.RequireValue(textBoundary__value65725).boundaryStart;
                    }
                    else
                    {
                        if ((position.offset > existingSelectionStart.offset))
                        {
                            targetPosition = DartRuntimePrimitives.RequireValue(textBoundary__value65725).boundaryEnd;
                        }
                        else
                        {
                            targetPosition = existingSelectionEnd;
                        }
                    }
                }
            }
            else
            {
                if ((existingSelectionStart is not null))
                {
                    if ((position.offset < existingSelectionStart.offset))
                    {
                        targetPosition = DartRuntimePrimitives.RequireValue(textBoundary__value65725).boundaryStart;
                    }
                    else
                    {
                        targetPosition = DartRuntimePrimitives.RequireValue(textBoundary__value65725).boundaryEnd;
                    }
                }
                else
                {
                    targetPosition = _closestTextBoundary(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(textBoundary__value65725)), position);
                }
            }
        }
        else
        {
            if (((this._selectableContainsOriginTextBoundary && (existingSelectionStart is not null)) && (existingSelectionEnd is not null)))
            {
                var isSamePositionLocal = (position.offset == existingSelectionStart.offset);
                bool isSelectionInvertedLocal = (existingSelectionStart.offset > existingSelectionEnd.offset);
                bool shouldSwapEdgesLocal = ((isSelectionInvertedLocal != ((position.offset < existingSelectionStart.offset))) || isSamePositionLocal);
                if (shouldSwapEdgesLocal)
                {
                    (TextPosition boundaryEnd, TextPosition boundaryStart) localTextBoundaryLocal = getTextBoundary(existingSelectionStart);
                    DartRuntimePrimitives.Assert(() => ((localTextBoundaryLocal.boundaryStart.offset >= this.range.start) && (localTextBoundaryLocal.boundaryEnd.offset <= this.range.end)));
                    _setSelectionPosition((isSelectionInvertedLocal ? localTextBoundaryLocal.boundaryStart : localTextBoundaryLocal.boundaryEnd), isEnd: false);
                }
            }
        }
        return (targetPosition ?? position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _updateSelectionEdgeByTextBoundary(Offset globalPosition, bool isEnd, Func<TextPosition, (TextPosition boundaryEnd, TextPosition boundaryStart)> getTextBoundary)
    {
        global::Doroti.Ui.TextPosition? existingSelectionStart = this._textSelectionStart;
        global::Doroti.Ui.TextPosition? existingSelectionEnd = this._textSelectionEnd;
        _setSelectionPosition(null, isEnd: isEnd);
        Matrix4 transform = this.paragraph.getTransformTo(null);
        transform.invert();
        global::Doroti.Ui.Offset localPosition = MatrixUtils.transformPoint(transform, globalPosition);
        if (this._rect.isEmpty)
        {
            SelectionResult result = SelectionUtils.getResultBasedOnRect(this._rect, localPosition);
            _setSelectionPosition(((object.Equals(result, SelectionResult.next)) ? new global::Doroti.Ui.TextPosition(offset: this.range.end) : new global::Doroti.Ui.TextPosition(offset: this.range.start, affinity: TextAffinity.upstream)), isEnd: isEnd);
            return result;
        }
        global::Doroti.Ui.Offset adjustedOffset = SelectionUtils.adjustDragOffset(this._rect, localPosition, direction: ((RenderParagraph)this.paragraph).textDirection);
        global::Doroti.Ui.TextPosition position = this.paragraph.getPositionForOffset(adjustedOffset);
        (TextPosition boundaryEnd, TextPosition boundaryStart)? textBoundary = (this._rect.contains(localPosition) ? getTextBoundary(position) : null);
        if (((textBoundary is not null) && ((((DartRuntimePrimitives.RequireValue(textBoundary).boundaryStart.offset < this.range.start) && (DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd.offset <= this.range.start)) || ((DartRuntimePrimitives.RequireValue(textBoundary).boundaryStart.offset >= this.range.end) && (DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd.offset > this.range.end))))))
        {
            (TextPosition boundaryEnd, TextPosition boundaryStart) textBoundary__71646__value71751 = DartRuntimePrimitives.RequireValue(textBoundary);
            textBoundary = null;
        }
        global::Doroti.Ui.TextPosition targetPosition = _clampTextPosition((isEnd ? _updateSelectionEndEdgeByTextBoundary(textBoundary, (Func<TextPosition, (TextPosition boundaryEnd, TextPosition boundaryStart)>)getTextBoundary, position, existingSelectionStart, existingSelectionEnd) : _updateSelectionStartEdgeByTextBoundary(textBoundary, (Func<TextPosition, (TextPosition boundaryEnd, TextPosition boundaryStart)>)getTextBoundary, position, existingSelectionStart, existingSelectionEnd)));
        _setSelectionPosition(targetPosition, isEnd: isEnd);
        if ((targetPosition.offset == this.range.end))
        {
            return SelectionResult.next;
        }
        if ((targetPosition.offset == this.range.start))
        {
            return SelectionResult.previous;
        }
        return SelectionUtils.getResultBasedOnRect(this._rect, localPosition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _updateSelectionEdge(Offset globalPosition, bool isEnd)
    {
        _setSelectionPosition(null, isEnd: isEnd);
        Matrix4 transform = this.paragraph.getTransformTo(null);
        transform.invert();
        global::Doroti.Ui.Offset localPosition = MatrixUtils.transformPoint(transform, globalPosition);
        if (this._rect.isEmpty)
        {
            SelectionResult result = SelectionUtils.getResultBasedOnRect(this._rect, localPosition);
            _setSelectionPosition(((object.Equals(result, SelectionResult.next)) ? new global::Doroti.Ui.TextPosition(offset: this.range.end) : new global::Doroti.Ui.TextPosition(offset: this.range.start, affinity: TextAffinity.upstream)), isEnd: isEnd);
            return result;
        }
        global::Doroti.Ui.Offset adjustedOffset = SelectionUtils.adjustDragOffset(this._rect, localPosition, direction: ((RenderParagraph)this.paragraph).textDirection);
        global::Doroti.Ui.TextPosition position = _clampTextPosition(this.paragraph.getPositionForOffset(adjustedOffset));
        _setSelectionPosition(position, isEnd: isEnd);
        if ((position.offset == this.range.end))
        {
            return SelectionResult.next;
        }
        if ((position.offset == this.range.start))
        {
            return SelectionResult.previous;
        }
        return SelectionUtils.getResultBasedOnRect(this._rect, localPosition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult? _updateSelectionStartEdgeByMultiSelectableTextBoundary(Func<TextPosition, string, (TextPosition boundaryEnd, TextPosition boundaryStart)> getTextBoundary, bool paragraphContainsPosition, TextPosition position, TextPosition? existingSelectionStart, TextPosition? existingSelectionEnd)
    {
        var isEndLocal = false;
        if (((this._selectableContainsOriginTextBoundary && (existingSelectionStart is not null)) && (existingSelectionEnd is not null)))
        {
            bool forwardSelection = (existingSelectionEnd.offset >= existingSelectionStart.offset);
            if (paragraphContainsPosition)
            {
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPosition = getTextBoundary(position, this.fullText);
                (TextPosition boundaryEnd, TextPosition boundaryStart) originTextBoundary = getTextBoundary((forwardSelection ? new global::Doroti.Ui.TextPosition(offset: (existingSelectionEnd.offset - 1L), affinity: existingSelectionEnd.affinity) : existingSelectionEnd), this.fullText);
                global::Doroti.Ui.TextPosition targetPosition = default!;
                long pivotOffset = (forwardSelection ? originTextBoundary.boundaryEnd.offset : originTextBoundary.boundaryStart.offset);
                var shouldSwapEdges = (!forwardSelection != ((position.offset > pivotOffset)));
                if ((position.offset < pivotOffset))
                {
                    targetPosition = boundaryAtPosition.boundaryStart;
                }
                else
                {
                    if ((position.offset > pivotOffset))
                    {
                        targetPosition = boundaryAtPosition.boundaryEnd;
                    }
                    else
                    {
                        targetPosition = (forwardSelection ? existingSelectionStart : existingSelectionEnd);
                    }
                }
                if (shouldSwapEdges)
                {
                    _setSelectionPosition(_clampTextPosition((forwardSelection ? originTextBoundary.boundaryStart : originTextBoundary.boundaryEnd)), isEnd: true);
                }
                _setSelectionPosition(_clampTextPosition(targetPosition), isEnd: isEndLocal);
                bool finalSelectionIsForward = (this._textSelectionEnd!.offset >= this._textSelectionStart!.offset);
                if (((boundaryAtPosition.boundaryStart.offset > this.range.end) && (boundaryAtPosition.boundaryEnd.offset > this.range.end)))
                {
                    return SelectionResult.next;
                }
                if (((boundaryAtPosition.boundaryStart.offset < this.range.start) && (boundaryAtPosition.boundaryEnd.offset < this.range.start)))
                {
                    return SelectionResult.previous;
                }
                if (finalSelectionIsForward)
                {
                    if ((boundaryAtPosition.boundaryStart.offset >= originTextBoundary.boundaryStart.offset))
                    {
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPosition.boundaryStart.offset < originTextBoundary.boundaryStart.offset))
                    {
                        return SelectionResult.previous;
                    }
                }
                else
                {
                    if ((boundaryAtPosition.boundaryEnd.offset <= originTextBoundary.boundaryEnd.offset))
                    {
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPosition.boundaryEnd.offset > originTextBoundary.boundaryEnd.offset))
                    {
                        return SelectionResult.next;
                    }
                }
            }
            else
            {
                global::Doroti.Ui.TextPosition clampedPosition = _clampTextPosition(position);
                (TextPosition boundaryEnd, TextPosition boundaryStart) originTextBoundaryLocal = getTextBoundary((forwardSelection ? new global::Doroti.Ui.TextPosition(offset: (existingSelectionEnd.offset - 1L), affinity: existingSelectionEnd.affinity) : existingSelectionEnd), this.fullText);
                if ((forwardSelection && (clampedPosition.offset == this.range.start)))
                {
                    _setSelectionPosition(clampedPosition, isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
                if ((!forwardSelection && (clampedPosition.offset == this.range.end)))
                {
                    _setSelectionPosition(clampedPosition, isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if ((forwardSelection && (clampedPosition.offset == this.range.end)))
                {
                    _setSelectionPosition(_clampTextPosition(originTextBoundaryLocal.boundaryStart), isEnd: true);
                    _setSelectionPosition(clampedPosition, isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if ((!forwardSelection && (clampedPosition.offset == this.range.start)))
                {
                    _setSelectionPosition(_clampTextPosition(originTextBoundaryLocal.boundaryEnd), isEnd: true);
                    _setSelectionPosition(clampedPosition, isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
            }
        }
        else
        {
            var positionOnPlaceholder = (this.paragraph.getWordBoundary(position).textInside(this.fullText) == _placeholderCharacter);
            if ((!paragraphContainsPosition || positionOnPlaceholder))
            {
                return null;
            }
            if ((existingSelectionEnd is not null))
            {
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPositionLocal = getTextBoundary(position, this.fullText);
                bool backwardSelection = ((((existingSelectionStart is null) && (existingSelectionEnd.offset == this.range.start)) || ((object.Equals(existingSelectionStart, existingSelectionEnd)) && (existingSelectionEnd.offset == this.range.start))) || ((existingSelectionStart is not null) && (existingSelectionStart.offset > existingSelectionEnd.offset)));
                if (((boundaryAtPositionLocal.boundaryStart.offset < this.range.start) && (boundaryAtPositionLocal.boundaryEnd.offset < this.range.start)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
                if (((boundaryAtPositionLocal.boundaryStart.offset > this.range.end) && (boundaryAtPositionLocal.boundaryEnd.offset > this.range.end)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if (backwardSelection)
                {
                    if ((boundaryAtPositionLocal.boundaryEnd.offset <= this.range.end))
                    {
                        _setSelectionPosition(_clampTextPosition(boundaryAtPositionLocal.boundaryEnd), isEnd: isEndLocal);
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPositionLocal.boundaryEnd.offset > this.range.end))
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEndLocal);
                        return SelectionResult.next;
                    }
                }
                else
                {
                    _setSelectionPosition(_clampTextPosition(boundaryAtPositionLocal.boundaryStart), isEnd: isEndLocal);
                    if ((boundaryAtPositionLocal.boundaryStart.offset < this.range.start))
                    {
                        return SelectionResult.previous;
                    }
                    if ((boundaryAtPositionLocal.boundaryStart.offset >= this.range.start))
                    {
                        return SelectionResult.end;
                    }
                }
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult? _updateSelectionEndEdgeByMultiSelectableTextBoundary(Func<TextPosition, string, (TextPosition boundaryEnd, TextPosition boundaryStart)> getTextBoundary, bool paragraphContainsPosition, TextPosition position, TextPosition? existingSelectionStart, TextPosition? existingSelectionEnd)
    {
        var isEndLocal = true;
        if (((this._selectableContainsOriginTextBoundary && (existingSelectionStart is not null)) && (existingSelectionEnd is not null)))
        {
            bool forwardSelection = (existingSelectionEnd.offset >= existingSelectionStart.offset);
            if (paragraphContainsPosition)
            {
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPosition = getTextBoundary(position, this.fullText);
                (TextPosition boundaryEnd, TextPosition boundaryStart) originTextBoundary = getTextBoundary((forwardSelection ? existingSelectionStart : new global::Doroti.Ui.TextPosition(offset: (existingSelectionStart.offset - 1L), affinity: existingSelectionStart.affinity)), this.fullText);
                global::Doroti.Ui.TextPosition targetPosition = default!;
                long pivotOffset = (forwardSelection ? originTextBoundary.boundaryStart.offset : originTextBoundary.boundaryEnd.offset);
                var shouldSwapEdges = (!forwardSelection != ((position.offset < pivotOffset)));
                if ((position.offset < pivotOffset))
                {
                    targetPosition = boundaryAtPosition.boundaryStart;
                }
                else
                {
                    if ((position.offset > pivotOffset))
                    {
                        targetPosition = boundaryAtPosition.boundaryEnd;
                    }
                    else
                    {
                        targetPosition = (forwardSelection ? existingSelectionEnd : existingSelectionStart);
                    }
                }
                if (shouldSwapEdges)
                {
                    _setSelectionPosition(_clampTextPosition((forwardSelection ? originTextBoundary.boundaryEnd : originTextBoundary.boundaryStart)), isEnd: false);
                }
                _setSelectionPosition(_clampTextPosition(targetPosition), isEnd: isEndLocal);
                bool finalSelectionIsForward = (this._textSelectionEnd!.offset >= this._textSelectionStart!.offset);
                if (((boundaryAtPosition.boundaryStart.offset > this.range.end) && (boundaryAtPosition.boundaryEnd.offset > this.range.end)))
                {
                    return SelectionResult.next;
                }
                if (((boundaryAtPosition.boundaryStart.offset < this.range.start) && (boundaryAtPosition.boundaryEnd.offset < this.range.start)))
                {
                    return SelectionResult.previous;
                }
                if (finalSelectionIsForward)
                {
                    if ((boundaryAtPosition.boundaryEnd.offset <= originTextBoundary.boundaryEnd.offset))
                    {
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPosition.boundaryEnd.offset > originTextBoundary.boundaryEnd.offset))
                    {
                        return SelectionResult.next;
                    }
                }
                else
                {
                    if ((boundaryAtPosition.boundaryStart.offset >= originTextBoundary.boundaryStart.offset))
                    {
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPosition.boundaryStart.offset < originTextBoundary.boundaryStart.offset))
                    {
                        return SelectionResult.previous;
                    }
                }
            }
            else
            {
                global::Doroti.Ui.TextPosition clampedPosition = _clampTextPosition(position);
                (TextPosition boundaryEnd, TextPosition boundaryStart) originTextBoundaryLocal = getTextBoundary((forwardSelection ? existingSelectionStart : new global::Doroti.Ui.TextPosition(offset: (existingSelectionStart.offset - 1L), affinity: existingSelectionStart.affinity)), this.fullText);
                if ((forwardSelection && (clampedPosition.offset == this.range.start)))
                {
                    _setSelectionPosition(_clampTextPosition(originTextBoundaryLocal.boundaryEnd), isEnd: false);
                    _setSelectionPosition(clampedPosition, isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
                if ((!forwardSelection && (clampedPosition.offset == this.range.end)))
                {
                    _setSelectionPosition(_clampTextPosition(originTextBoundaryLocal.boundaryStart), isEnd: false);
                    _setSelectionPosition(clampedPosition, isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if ((forwardSelection && (clampedPosition.offset == this.range.end)))
                {
                    _setSelectionPosition(clampedPosition, isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if ((!forwardSelection && (clampedPosition.offset == this.range.start)))
                {
                    _setSelectionPosition(clampedPosition, isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
            }
        }
        else
        {
            var positionOnPlaceholder = (this.paragraph.getWordBoundary(position).textInside(this.fullText) == _placeholderCharacter);
            if ((!paragraphContainsPosition || positionOnPlaceholder))
            {
                return null;
            }
            if ((existingSelectionStart is not null))
            {
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPositionLocal = getTextBoundary(position, this.fullText);
                bool backwardSelection = ((((existingSelectionEnd is null) && (existingSelectionStart.offset == this.range.end)) || ((object.Equals(existingSelectionStart, existingSelectionEnd)) && (existingSelectionStart.offset == this.range.end))) || ((existingSelectionEnd is not null) && (existingSelectionStart.offset > existingSelectionEnd.offset)));
                if (((boundaryAtPositionLocal.boundaryStart.offset < this.range.start) && (boundaryAtPositionLocal.boundaryEnd.offset < this.range.start)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
                if (((boundaryAtPositionLocal.boundaryStart.offset > this.range.end) && (boundaryAtPositionLocal.boundaryEnd.offset > this.range.end)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if (backwardSelection)
                {
                    _setSelectionPosition(_clampTextPosition(boundaryAtPositionLocal.boundaryStart), isEnd: isEndLocal);
                    if ((boundaryAtPositionLocal.boundaryStart.offset < this.range.start))
                    {
                        return SelectionResult.previous;
                    }
                    if ((boundaryAtPositionLocal.boundaryStart.offset >= this.range.start))
                    {
                        return SelectionResult.end;
                    }
                }
                else
                {
                    if ((boundaryAtPositionLocal.boundaryEnd.offset <= this.range.end))
                    {
                        _setSelectionPosition(_clampTextPosition(boundaryAtPositionLocal.boundaryEnd), isEnd: isEndLocal);
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPositionLocal.boundaryEnd.offset > this.range.end))
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEndLocal);
                        return SelectionResult.next;
                    }
                }
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult? _updateSelectionStartEdgeAtPlaceholderByMultiSelectableTextBoundary(Func<TextPosition, string, (TextPosition boundaryEnd, TextPosition boundaryStart)> getTextBoundary, Offset globalPosition, bool paragraphContainsPosition, TextPosition position, TextPosition? existingSelectionStart, TextPosition? existingSelectionEnd)
    {
        var isEndLocal = false;
        if (((this._selectableContainsOriginTextBoundary && (existingSelectionStart is not null)) && (existingSelectionEnd is not null)))
        {
            bool forwardSelection = (existingSelectionEnd.offset >= existingSelectionStart.offset);
            RenderParagraph originParagraph = _getOriginParagraph();
            var fragmentBelongsToOriginParagraph = (object.Equals(originParagraph, this.paragraph));
            if (fragmentBelongsToOriginParagraph)
            {
                return _updateSelectionStartEdgeByMultiSelectableTextBoundary((Func<TextPosition, string, (TextPosition boundaryEnd, TextPosition boundaryStart)>)getTextBoundary, paragraphContainsPosition, position, existingSelectionStart, existingSelectionEnd);
            }
            Matrix4 originTransform = originParagraph.getTransformTo(null);
            originTransform.invert();
            global::Doroti.Ui.Offset originParagraphLocalPosition = MatrixUtils.transformPoint(originTransform, globalPosition);
            bool positionWithinOriginParagraph = originParagraph.paintBounds.contains(originParagraphLocalPosition);
            global::Doroti.Ui.TextPosition positionRelativeToOriginParagraph = originParagraph.getPositionForOffset(originParagraphLocalPosition);
            if (positionWithinOriginParagraph)
            {
                string originText = ((RenderParagraph)originParagraph).text.toPlainText(includeSemanticsLabels: false);
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPosition = getTextBoundary(positionRelativeToOriginParagraph, originText);
                (TextPosition boundaryEnd, TextPosition boundaryStart) originTextBoundary = getTextBoundary(_getPositionInParagraph(originParagraph), originText);
                global::Doroti.Ui.TextPosition targetPosition = default!;
                long pivotOffset = (forwardSelection ? originTextBoundary.boundaryEnd.offset : originTextBoundary.boundaryStart.offset);
                var shouldSwapEdges = (!forwardSelection != ((positionRelativeToOriginParagraph.offset > pivotOffset)));
                if ((positionRelativeToOriginParagraph.offset < pivotOffset))
                {
                    targetPosition = boundaryAtPosition.boundaryStart;
                }
                else
                {
                    if ((positionRelativeToOriginParagraph.offset > pivotOffset))
                    {
                        targetPosition = boundaryAtPosition.boundaryEnd;
                    }
                    else
                    {
                        targetPosition = existingSelectionStart;
                    }
                }
                if (shouldSwapEdges)
                {
                    _setSelectionPosition(existingSelectionStart, isEnd: true);
                }
                _setSelectionPosition(_clampTextPosition(targetPosition), isEnd: isEndLocal);
                bool finalSelectionIsForward = (this._textSelectionEnd!.offset >= this._textSelectionStart!.offset);
                global::Doroti.Ui.TextPosition originParagraphPlaceholderTextPosition = _getPositionInParagraph(originParagraph);
                var originParagraphPlaceholderRange = new global::Doroti.Ui.TextRange(start: originParagraphPlaceholderTextPosition.offset, end: (originParagraphPlaceholderTextPosition.offset + _placeholderLength));
                if (((boundaryAtPosition.boundaryStart.offset > originParagraphPlaceholderRange.end) && (boundaryAtPosition.boundaryEnd.offset > originParagraphPlaceholderRange.end)))
                {
                    return SelectionResult.next;
                }
                if (((boundaryAtPosition.boundaryStart.offset < originParagraphPlaceholderRange.start) && (boundaryAtPosition.boundaryEnd.offset < originParagraphPlaceholderRange.start)))
                {
                    return SelectionResult.previous;
                }
                if (finalSelectionIsForward)
                {
                    if ((boundaryAtPosition.boundaryEnd.offset <= originTextBoundary.boundaryEnd.offset))
                    {
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPosition.boundaryEnd.offset > originTextBoundary.boundaryEnd.offset))
                    {
                        return SelectionResult.next;
                    }
                }
                else
                {
                    if ((boundaryAtPosition.boundaryStart.offset >= originTextBoundary.boundaryStart.offset))
                    {
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPosition.boundaryStart.offset < originTextBoundary.boundaryStart.offset))
                    {
                        return SelectionResult.previous;
                    }
                }
            }
            else
            {
                global::Doroti.Ui.Offset adjustedOffset = SelectionUtils.adjustDragOffset(originParagraph.paintBounds, originParagraphLocalPosition, direction: ((RenderParagraph)this.paragraph).textDirection);
                global::Doroti.Ui.TextPosition adjustedPositionRelativeToOriginParagraph = originParagraph.getPositionForOffset(adjustedOffset);
                global::Doroti.Ui.TextPosition originParagraphPlaceholderTextPositionLocal = _getPositionInParagraph(originParagraph);
                var originParagraphPlaceholderRangeLocal = new global::Doroti.Ui.TextRange(start: originParagraphPlaceholderTextPositionLocal.offset, end: (originParagraphPlaceholderTextPositionLocal.offset + _placeholderLength));
                if ((forwardSelection && (adjustedPositionRelativeToOriginParagraph.offset <= originParagraphPlaceholderRangeLocal.start)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
                if ((!forwardSelection && (adjustedPositionRelativeToOriginParagraph.offset >= originParagraphPlaceholderRangeLocal.end)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if ((forwardSelection && (adjustedPositionRelativeToOriginParagraph.offset >= originParagraphPlaceholderRangeLocal.end)))
                {
                    _setSelectionPosition(existingSelectionStart, isEnd: true);
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if ((!forwardSelection && (adjustedPositionRelativeToOriginParagraph.offset <= originParagraphPlaceholderRangeLocal.start)))
                {
                    _setSelectionPosition(existingSelectionStart, isEnd: true);
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
            }
        }
        else
        {
            if (paragraphContainsPosition)
            {
                return _updateSelectionStartEdgeByMultiSelectableTextBoundary((Func<TextPosition, string, (TextPosition boundaryEnd, TextPosition boundaryStart)>)getTextBoundary, paragraphContainsPosition, position, existingSelectionStart, existingSelectionEnd);
            }
            if ((existingSelectionEnd is not null))
            {
                (RenderParagraph paragraph, global::Doroti.Ui.Offset localPosition)? targetDetails = _getParagraphContainingPosition(globalPosition);
                if ((targetDetails is null))
                {
                    return null;
                }
                RenderParagraph targetParagraph = DartRuntimePrimitives.RequireValue(targetDetails).paragraph;
                global::Doroti.Ui.TextPosition positionRelativeToTargetParagraph = targetParagraph.getPositionForOffset(DartRuntimePrimitives.RequireValue(targetDetails).localPosition);
                string targetText = ((RenderParagraph)targetParagraph).text.toPlainText(includeSemanticsLabels: false);
                var positionOnPlaceholder = (targetParagraph.getWordBoundary(positionRelativeToTargetParagraph).textInside(targetText) == _placeholderCharacter);
                if (positionOnPlaceholder)
                {
                    return null;
                }
                bool backwardSelection = ((((existingSelectionStart is null) && (existingSelectionEnd.offset == this.range.start)) || ((object.Equals(existingSelectionStart, existingSelectionEnd)) && (existingSelectionEnd.offset == this.range.start))) || ((existingSelectionStart is not null) && (existingSelectionStart.offset > existingSelectionEnd.offset)));
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPositionRelativeToTargetParagraph = getTextBoundary(positionRelativeToTargetParagraph, targetText);
                global::Doroti.Ui.TextPosition targetParagraphPlaceholderTextPosition = _getPositionInParagraph(targetParagraph);
                var targetParagraphPlaceholderRange = new global::Doroti.Ui.TextRange(start: targetParagraphPlaceholderTextPosition.offset, end: (targetParagraphPlaceholderTextPosition.offset + _placeholderLength));
                if (((boundaryAtPositionRelativeToTargetParagraph.boundaryStart.offset < targetParagraphPlaceholderRange.start) && (boundaryAtPositionRelativeToTargetParagraph.boundaryEnd.offset < targetParagraphPlaceholderRange.start)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
                if (((boundaryAtPositionRelativeToTargetParagraph.boundaryStart.offset > targetParagraphPlaceholderRange.end) && (boundaryAtPositionRelativeToTargetParagraph.boundaryEnd.offset > targetParagraphPlaceholderRange.end)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if (backwardSelection)
                {
                    if ((boundaryAtPositionRelativeToTargetParagraph.boundaryEnd.offset <= targetParagraphPlaceholderRange.end))
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEndLocal);
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPositionRelativeToTargetParagraph.boundaryEnd.offset > targetParagraphPlaceholderRange.end))
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEndLocal);
                        return SelectionResult.next;
                    }
                }
                else
                {
                    if ((boundaryAtPositionRelativeToTargetParagraph.boundaryStart.offset >= targetParagraphPlaceholderRange.start))
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEndLocal);
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPositionRelativeToTargetParagraph.boundaryStart.offset < targetParagraphPlaceholderRange.start))
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEndLocal);
                        return SelectionResult.previous;
                    }
                }
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult? _updateSelectionEndEdgeAtPlaceholderByMultiSelectableTextBoundary(Func<TextPosition, string, (TextPosition boundaryEnd, TextPosition boundaryStart)> getTextBoundary, Offset globalPosition, bool paragraphContainsPosition, TextPosition position, TextPosition? existingSelectionStart, TextPosition? existingSelectionEnd)
    {
        var isEndLocal = true;
        if (((this._selectableContainsOriginTextBoundary && (existingSelectionStart is not null)) && (existingSelectionEnd is not null)))
        {
            bool forwardSelection = (existingSelectionEnd.offset >= existingSelectionStart.offset);
            RenderParagraph originParagraph = _getOriginParagraph();
            var fragmentBelongsToOriginParagraph = (object.Equals(originParagraph, this.paragraph));
            if (fragmentBelongsToOriginParagraph)
            {
                return _updateSelectionEndEdgeByMultiSelectableTextBoundary((Func<TextPosition, string, (TextPosition boundaryEnd, TextPosition boundaryStart)>)getTextBoundary, paragraphContainsPosition, position, existingSelectionStart, existingSelectionEnd);
            }
            Matrix4 originTransform = originParagraph.getTransformTo(null);
            originTransform.invert();
            global::Doroti.Ui.Offset originParagraphLocalPosition = MatrixUtils.transformPoint(originTransform, globalPosition);
            bool positionWithinOriginParagraph = originParagraph.paintBounds.contains(originParagraphLocalPosition);
            global::Doroti.Ui.TextPosition positionRelativeToOriginParagraph = originParagraph.getPositionForOffset(originParagraphLocalPosition);
            if (positionWithinOriginParagraph)
            {
                string originText = ((RenderParagraph)originParagraph).text.toPlainText(includeSemanticsLabels: false);
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPosition = getTextBoundary(positionRelativeToOriginParagraph, originText);
                (TextPosition boundaryEnd, TextPosition boundaryStart) originTextBoundary = getTextBoundary(_getPositionInParagraph(originParagraph), originText);
                global::Doroti.Ui.TextPosition targetPosition = default!;
                long pivotOffset = (forwardSelection ? originTextBoundary.boundaryStart.offset : originTextBoundary.boundaryEnd.offset);
                var shouldSwapEdges = (!forwardSelection != ((positionRelativeToOriginParagraph.offset < pivotOffset)));
                if ((positionRelativeToOriginParagraph.offset < pivotOffset))
                {
                    targetPosition = boundaryAtPosition.boundaryStart;
                }
                else
                {
                    if ((positionRelativeToOriginParagraph.offset > pivotOffset))
                    {
                        targetPosition = boundaryAtPosition.boundaryEnd;
                    }
                    else
                    {
                        targetPosition = existingSelectionEnd;
                    }
                }
                if (shouldSwapEdges)
                {
                    _setSelectionPosition(existingSelectionEnd, isEnd: false);
                }
                _setSelectionPosition(_clampTextPosition(targetPosition), isEnd: isEndLocal);
                bool finalSelectionIsForward = (this._textSelectionEnd!.offset >= this._textSelectionStart!.offset);
                global::Doroti.Ui.TextPosition originParagraphPlaceholderTextPosition = _getPositionInParagraph(originParagraph);
                var originParagraphPlaceholderRange = new global::Doroti.Ui.TextRange(start: originParagraphPlaceholderTextPosition.offset, end: (originParagraphPlaceholderTextPosition.offset + _placeholderLength));
                if (((boundaryAtPosition.boundaryStart.offset > originParagraphPlaceholderRange.end) && (boundaryAtPosition.boundaryEnd.offset > originParagraphPlaceholderRange.end)))
                {
                    return SelectionResult.next;
                }
                if (((boundaryAtPosition.boundaryStart.offset < originParagraphPlaceholderRange.start) && (boundaryAtPosition.boundaryEnd.offset < originParagraphPlaceholderRange.start)))
                {
                    return SelectionResult.previous;
                }
                if (finalSelectionIsForward)
                {
                    if ((boundaryAtPosition.boundaryEnd.offset <= originTextBoundary.boundaryEnd.offset))
                    {
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPosition.boundaryEnd.offset > originTextBoundary.boundaryEnd.offset))
                    {
                        return SelectionResult.next;
                    }
                }
                else
                {
                    if ((boundaryAtPosition.boundaryStart.offset >= originTextBoundary.boundaryStart.offset))
                    {
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPosition.boundaryStart.offset < originTextBoundary.boundaryStart.offset))
                    {
                        return SelectionResult.previous;
                    }
                }
            }
            else
            {
                global::Doroti.Ui.Offset adjustedOffset = SelectionUtils.adjustDragOffset(originParagraph.paintBounds, originParagraphLocalPosition, direction: ((RenderParagraph)this.paragraph).textDirection);
                global::Doroti.Ui.TextPosition adjustedPositionRelativeToOriginParagraph = originParagraph.getPositionForOffset(adjustedOffset);
                global::Doroti.Ui.TextPosition originParagraphPlaceholderTextPositionLocal = _getPositionInParagraph(originParagraph);
                var originParagraphPlaceholderRangeLocal = new global::Doroti.Ui.TextRange(start: originParagraphPlaceholderTextPositionLocal.offset, end: (originParagraphPlaceholderTextPositionLocal.offset + _placeholderLength));
                if ((forwardSelection && (adjustedPositionRelativeToOriginParagraph.offset <= originParagraphPlaceholderRangeLocal.start)))
                {
                    _setSelectionPosition(existingSelectionEnd, isEnd: false);
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
                if ((!forwardSelection && (adjustedPositionRelativeToOriginParagraph.offset >= originParagraphPlaceholderRangeLocal.end)))
                {
                    _setSelectionPosition(existingSelectionEnd, isEnd: false);
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if ((forwardSelection && (adjustedPositionRelativeToOriginParagraph.offset >= originParagraphPlaceholderRangeLocal.end)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if ((!forwardSelection && (adjustedPositionRelativeToOriginParagraph.offset <= originParagraphPlaceholderRangeLocal.start)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
            }
        }
        else
        {
            if (paragraphContainsPosition)
            {
                return _updateSelectionEndEdgeByMultiSelectableTextBoundary((Func<TextPosition, string, (TextPosition boundaryEnd, TextPosition boundaryStart)>)getTextBoundary, paragraphContainsPosition, position, existingSelectionStart, existingSelectionEnd);
            }
            if ((existingSelectionStart is not null))
            {
                (RenderParagraph paragraph, global::Doroti.Ui.Offset localPosition)? targetDetails = _getParagraphContainingPosition(globalPosition);
                if ((targetDetails is null))
                {
                    return null;
                }
                RenderParagraph targetParagraph = DartRuntimePrimitives.RequireValue(targetDetails).paragraph;
                global::Doroti.Ui.TextPosition positionRelativeToTargetParagraph = targetParagraph.getPositionForOffset(DartRuntimePrimitives.RequireValue(targetDetails).localPosition);
                string targetText = ((RenderParagraph)targetParagraph).text.toPlainText(includeSemanticsLabels: false);
                var positionOnPlaceholder = (targetParagraph.getWordBoundary(positionRelativeToTargetParagraph).textInside(targetText) == _placeholderCharacter);
                if (positionOnPlaceholder)
                {
                    return null;
                }
                bool backwardSelection = ((((existingSelectionEnd is null) && (existingSelectionStart.offset == this.range.end)) || ((object.Equals(existingSelectionStart, existingSelectionEnd)) && (existingSelectionStart.offset == this.range.end))) || ((existingSelectionEnd is not null) && (existingSelectionStart.offset > existingSelectionEnd.offset)));
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPositionRelativeToTargetParagraph = getTextBoundary(positionRelativeToTargetParagraph, targetText);
                global::Doroti.Ui.TextPosition targetParagraphPlaceholderTextPosition = _getPositionInParagraph(targetParagraph);
                var targetParagraphPlaceholderRange = new global::Doroti.Ui.TextRange(start: targetParagraphPlaceholderTextPosition.offset, end: (targetParagraphPlaceholderTextPosition.offset + _placeholderLength));
                if (((boundaryAtPositionRelativeToTargetParagraph.boundaryStart.offset < targetParagraphPlaceholderRange.start) && (boundaryAtPositionRelativeToTargetParagraph.boundaryEnd.offset < targetParagraphPlaceholderRange.start)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
                if (((boundaryAtPositionRelativeToTargetParagraph.boundaryStart.offset > targetParagraphPlaceholderRange.end) && (boundaryAtPositionRelativeToTargetParagraph.boundaryEnd.offset > targetParagraphPlaceholderRange.end)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if (backwardSelection)
                {
                    if ((boundaryAtPositionRelativeToTargetParagraph.boundaryStart.offset >= targetParagraphPlaceholderRange.start))
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEndLocal);
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPositionRelativeToTargetParagraph.boundaryStart.offset < targetParagraphPlaceholderRange.start))
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEndLocal);
                        return SelectionResult.previous;
                    }
                }
                else
                {
                    if ((boundaryAtPositionRelativeToTargetParagraph.boundaryEnd.offset <= targetParagraphPlaceholderRange.end))
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEndLocal);
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPositionRelativeToTargetParagraph.boundaryEnd.offset > targetParagraphPlaceholderRange.end))
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEndLocal);
                        return SelectionResult.next;
                    }
                }
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _updateSelectionEdgeByMultiSelectableTextBoundary(Offset globalPosition, bool isEnd, Func<TextPosition, string, (TextPosition boundaryEnd, TextPosition boundaryStart)> getTextBoundary, Func<TextPosition, (TextPosition boundaryEnd, TextPosition boundaryStart)> getClampedTextBoundary)
    {
        global::Doroti.Ui.TextPosition? existingSelectionStart = this._textSelectionStart;
        global::Doroti.Ui.TextPosition? existingSelectionEnd = this._textSelectionEnd;
        _setSelectionPosition(null, isEnd: isEnd);
        Matrix4 transform = this.paragraph.getTransformTo(null);
        transform.invert();
        global::Doroti.Ui.Offset localPosition = MatrixUtils.transformPoint(transform, globalPosition);
        if (this._rect.isEmpty)
        {
            SelectionResult result = SelectionUtils.getResultBasedOnRect(this._rect, localPosition);
            _setSelectionPosition(((object.Equals(result, SelectionResult.next)) ? new global::Doroti.Ui.TextPosition(offset: this.range.end) : new global::Doroti.Ui.TextPosition(offset: this.range.start, affinity: TextAffinity.upstream)), isEnd: isEnd);
            return result;
        }
        global::Doroti.Ui.Offset adjustedOffset = SelectionUtils.adjustDragOffset(this._rect, localPosition, direction: ((RenderParagraph)this.paragraph).textDirection);
        global::Doroti.Ui.Offset adjustedOffsetRelativeToParagraph = SelectionUtils.adjustDragOffset(this.paragraph.paintBounds, localPosition, direction: ((RenderParagraph)this.paragraph).textDirection);
        global::Doroti.Ui.TextPosition position = this.paragraph.getPositionForOffset(adjustedOffset);
        global::Doroti.Ui.TextPosition positionInFullText = this.paragraph.getPositionForOffset(adjustedOffsetRelativeToParagraph);
        SelectionResult? resultLocal = default!;
        if (_isPlaceholder())
        {
            resultLocal = (isEnd ? _updateSelectionEndEdgeAtPlaceholderByMultiSelectableTextBoundary((Func<TextPosition, string, (TextPosition boundaryEnd, TextPosition boundaryStart)>)getTextBoundary, globalPosition, this.paragraph.paintBounds.contains(localPosition), positionInFullText, existingSelectionStart, existingSelectionEnd) : _updateSelectionStartEdgeAtPlaceholderByMultiSelectableTextBoundary((Func<TextPosition, string, (TextPosition boundaryEnd, TextPosition boundaryStart)>)getTextBoundary, globalPosition, this.paragraph.paintBounds.contains(localPosition), positionInFullText, existingSelectionStart, existingSelectionEnd));
        }
        else
        {
            resultLocal = (isEnd ? _updateSelectionEndEdgeByMultiSelectableTextBoundary((Func<TextPosition, string, (TextPosition boundaryEnd, TextPosition boundaryStart)>)getTextBoundary, this.paragraph.paintBounds.contains(localPosition), positionInFullText, existingSelectionStart, existingSelectionEnd) : _updateSelectionStartEdgeByMultiSelectableTextBoundary((Func<TextPosition, string, (TextPosition boundaryEnd, TextPosition boundaryStart)>)getTextBoundary, this.paragraph.paintBounds.contains(localPosition), positionInFullText, existingSelectionStart, existingSelectionEnd));
        }
        if ((resultLocal is not null))
        {
            SelectionResult result__118831__value120148 = DartRuntimePrimitives.RequireValue(resultLocal);
            return DartRuntimePrimitives.RequireValue(result__118831__value120148);
        }
        (TextPosition boundaryEnd, TextPosition boundaryStart)? textBoundary = (_boundingBoxesContains(localPosition) ? getClampedTextBoundary(position) : null);
        if (((textBoundary is not null) && ((((DartRuntimePrimitives.RequireValue(textBoundary).boundaryStart.offset < this.range.start) && (DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd.offset <= this.range.start)) || ((DartRuntimePrimitives.RequireValue(textBoundary).boundaryStart.offset >= this.range.end) && (DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd.offset > this.range.end))))))
        {
            (TextPosition boundaryEnd, TextPosition boundaryStart) textBoundary__120500__value120620 = DartRuntimePrimitives.RequireValue(textBoundary);
            textBoundary = null;
        }
        global::Doroti.Ui.TextPosition targetPosition = _clampTextPosition((isEnd ? _updateSelectionEndEdgeByTextBoundary(textBoundary, (Func<TextPosition, (TextPosition boundaryEnd, TextPosition boundaryStart)>)getClampedTextBoundary, position, existingSelectionStart, existingSelectionEnd) : _updateSelectionStartEdgeByTextBoundary(textBoundary, (Func<TextPosition, (TextPosition boundaryEnd, TextPosition boundaryStart)>)getClampedTextBoundary, position, existingSelectionStart, existingSelectionEnd)));
        _setSelectionPosition(targetPosition, isEnd: isEnd);
        if ((targetPosition.offset == this.range.end))
        {
            return SelectionResult.next;
        }
        if ((targetPosition.offset == this.range.start))
        {
            return SelectionResult.previous;
        }
        return SelectionUtils.getResultBasedOnRect(this._rect, localPosition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _closestTextBoundary((TextPosition boundaryEnd, TextPosition boundaryStart) textBoundary, TextPosition position)
    {
        long differenceA = ((position.offset - DartRuntimePrimitives.RequireValue(textBoundary).boundaryStart.offset)).abs();
        long differenceB = ((position.offset - DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd.offset)).abs();
        return ((differenceA < differenceB) ? DartRuntimePrimitives.RequireValue(textBoundary).boundaryStart : DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isPlaceholder()
    {
        RenderObject? current = this.paragraph.parent;
        while ((current is not null))
        {
            if ((current is RenderParagraph))
            {
                RenderParagraph current__122853__as122921 = (RenderParagraph)current;
                return true;
            }
            current = ((RenderObject)current).parent;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual RenderParagraph _getOriginParagraph()
    {
        DartRuntimePrimitives.Assert(() => this._selectableContainsOriginTextBoundary);
        RenderObject? current = this.paragraph.parent;
        RenderParagraph? originParagraph = default!;
        while ((current is not null))
        {
            if ((current is RenderParagraph))
            {
                RenderParagraph current__123508__as123614 = (RenderParagraph)current;
                if ((((RenderParagraph)((RenderParagraph)current__123508__as123614))._lastSelectableFragments is not null))
                {
                    var paragraphContainsOriginTextBoundary = false;
                    foreach (_SelectableFragment__paragraph fragment in ((RenderParagraph)((RenderParagraph)current__123508__as123614))._lastSelectableFragments!)
                    {
                        if (((_SelectableFragment__paragraph)fragment)._selectableContainsOriginTextBoundary)
                        {
                            paragraphContainsOriginTextBoundary = true;
                            originParagraph = ((RenderParagraph)current__123508__as123614);
                            break;
                        }
                    }
                    if (!paragraphContainsOriginTextBoundary)
                    {
                        return (originParagraph ?? this.paragraph);
                    }
                }
            }
            current = ((RenderObject)current).parent;
        }
        return (originParagraph ?? this.paragraph);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (RenderParagraph paragraph, global::Doroti.Ui.Offset localPosition)? _getParagraphContainingPosition(Offset globalPosition)
    {
        RenderObject? current = this.paragraph;
        while ((current is not null))
        {
            if ((current is RenderParagraph))
            {
                RenderParagraph current__124717__as124778 = (RenderParagraph)current;
                Matrix4 currentTransform = ((RenderParagraph)current__124717__as124778).getTransformTo(null);
                currentTransform.invert();
                global::Doroti.Ui.Offset currentParagraphLocalPosition = MatrixUtils.transformPoint(currentTransform, globalPosition);
                bool positionWithinCurrentParagraph = ((RenderParagraph)current__124717__as124778).paintBounds.contains(currentParagraphLocalPosition);
                if (positionWithinCurrentParagraph)
                {
                    return (paragraph: ((RenderParagraph)current__124717__as124778), localPosition: currentParagraphLocalPosition);
                }
            }
            current = ((RenderObject)current).parent;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _boundingBoxesContains(Offset position)
    {
        foreach (global::Doroti.Ui.Rect rect in this.boundingBoxes)
        {
            if (rect.contains(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(position))))
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _clampTextPosition(TextPosition position)
    {
        if (((position.offset > this.range.end) || (((position.offset == this.range.end) && (object.Equals(position.affinity, TextAffinity.downstream))))))
        {
            return new global::Doroti.Ui.TextPosition(offset: this.range.end, affinity: TextAffinity.upstream);
        }
        if ((position.offset < this.range.start))
        {
            return new global::Doroti.Ui.TextPosition(offset: this.range.start);
        }
        return position;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _setSelectionPosition(TextPosition? position, bool isEnd)
    {
        if (isEnd)
        {
            _textSelectionEnd = position;
        }
        else
        {
            _textSelectionStart = position;
        }
    }

    internal virtual SelectionResult _handleClearSelection()
    {
        _textSelectionStart = null;
        _textSelectionEnd = null;
        _selectableContainsOriginTextBoundary = false;
        return SelectionResult.none;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _handleSelectAll()
    {
        _textSelectionStart = new global::Doroti.Ui.TextPosition(offset: this.range.start);
        _textSelectionEnd = new global::Doroti.Ui.TextPosition(offset: this.range.end, affinity: TextAffinity.upstream);
        return SelectionResult.none;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _handleSelectTextBoundary((TextPosition boundaryEnd, TextPosition boundaryStart) textBoundary)
    {
        if (((DartRuntimePrimitives.RequireValue(textBoundary).boundaryStart.offset < this.range.start) && (textBoundary.boundaryEnd.offset <= this.range.start)))
        {
            return SelectionResult.previous;
        }
        else
        {
            if (((textBoundary.boundaryStart.offset >= this.range.end) && (DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd.offset > this.range.end)))
            {
                return SelectionResult.next;
            }
        }
        DartRuntimePrimitives.Assert(() => ((textBoundary.boundaryStart.offset >= this.range.start) && (textBoundary.boundaryEnd.offset <= this.range.end)));
        _textSelectionStart = DartRuntimePrimitives.RequireValue(textBoundary).boundaryStart;
        _textSelectionEnd = DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd;
        _selectableContainsOriginTextBoundary = true;
        return SelectionResult.end;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextRange? _intersect(TextRange a, TextRange b)
    {
        DartRuntimePrimitives.Assert(() => a.isNormalized);
        DartRuntimePrimitives.Assert(() => b.isNormalized);
        long startMax = Math.Max(a.start, b.start);
        long endMin = Math.Min(a.end, b.end);
        if ((startMax <= endMin))
        {
            return new global::Doroti.Ui.TextRange(start: startMax, end: endMin);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _handleSelectMultiFragmentTextBoundary((TextPosition boundaryEnd, TextPosition boundaryStart) textBoundary)
    {
        if (((DartRuntimePrimitives.RequireValue(textBoundary).boundaryStart.offset < this.range.start) && (textBoundary.boundaryEnd.offset <= this.range.start)))
        {
            return SelectionResult.previous;
        }
        else
        {
            if (((textBoundary.boundaryStart.offset >= this.range.end) && (DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd.offset > this.range.end)))
            {
                return SelectionResult.next;
            }
        }
        var boundaryAsRange = new global::Doroti.Ui.TextRange(start: DartRuntimePrimitives.RequireValue(textBoundary).boundaryStart.offset, end: DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd.offset);
        global::Doroti.Ui.TextRange? intersectRange = _intersect(this.range, boundaryAsRange);
        if ((intersectRange is not null))
        {
            _textSelectionStart = new global::Doroti.Ui.TextPosition(offset: intersectRange.start);
            _textSelectionEnd = new global::Doroti.Ui.TextPosition(offset: intersectRange.end);
            _selectableContainsOriginTextBoundary = true;
            if ((this.range.end < DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd.offset))
            {
                return SelectionResult.next;
            }
            return SelectionResult.end;
        }
        return SelectionResult.none;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (TextPosition boundaryEnd, TextPosition boundaryStart) _adjustTextBoundaryAtPosition(TextRange textBoundary, TextPosition position)
    {
        global::Doroti.Ui.TextPosition startLocal = default!;
        global::Doroti.Ui.TextPosition endLocal = default!;
        if ((position.offset > textBoundary.end))
        {
            startLocal = endLocal = new global::Doroti.Ui.TextPosition(offset: position.offset);
        }
        else
        {
            startLocal = new global::Doroti.Ui.TextPosition(offset: textBoundary.start);
            endLocal = new global::Doroti.Ui.TextPosition(offset: textBoundary.end, affinity: TextAffinity.upstream);
        }
        return (boundaryEnd: endLocal, boundaryStart: startLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _handleSelectWord(Offset globalPosition)
    {
        global::Doroti.Ui.TextPosition position = this.paragraph.getPositionForOffset(this.paragraph.globalToLocal(globalPosition));
        if ((_positionIsWithinCurrentSelection(position) && (!object.Equals(this._textSelectionStart, this._textSelectionEnd))))
        {
            return SelectionResult.end;
        }
        (TextPosition boundaryEnd, TextPosition boundaryStart) wordBoundary = _getWordBoundaryAtPosition(position);
        return _handleSelectTextBoundary(wordBoundary);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (TextPosition boundaryEnd, TextPosition boundaryStart) _getWordBoundaryAtPosition(TextPosition position)
    {
        global::Doroti.Ui.TextRange word = this.paragraph.getWordBoundary(position);
        DartRuntimePrimitives.Assert(() => word.isNormalized);
        return _adjustTextBoundaryAtPosition(word, position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _handleSelectParagraph(Offset globalPosition)
    {
        global::Doroti.Ui.Offset localPosition = this.paragraph.globalToLocal(globalPosition);
        global::Doroti.Ui.TextPosition position = this.paragraph.getPositionForOffset(localPosition);
        (TextPosition boundaryEnd, TextPosition boundaryStart) paragraphBoundary = _getParagraphBoundaryAtPosition(position, this.fullText);
        return _handleSelectMultiFragmentTextBoundary(paragraphBoundary);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _getPositionInParagraph(RenderParagraph targetParagraph)
    {
        Matrix4 transform = this.paragraph.getTransformTo(targetParagraph);
        global::Doroti.Ui.Offset localCenter = this.paragraph.paintBounds.centerLeft;
        global::Doroti.Ui.Offset localPos = MatrixUtils.transformPoint(transform, localCenter);
        global::Doroti.Ui.TextPosition position = targetParagraph.getPositionForOffset(localPos);
        return position;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (TextPosition boundaryEnd, TextPosition boundaryStart) _getParagraphBoundaryAtPosition(TextPosition position, string text)
    {
        var paragraphBoundary = new ParagraphBoundary(text);
        long paragraphStart = (paragraphBoundary.getLeadingTextBoundaryAt((((position.offset == text.Length) || (object.Equals(position.affinity, TextAffinity.upstream))) ? (position.offset - 1L) : position.offset)) ?? 0L);
        long paragraphEnd = (paragraphBoundary.getTrailingTextBoundaryAt(position.offset) ?? text.Length);
        var paragraphRange = new global::Doroti.Ui.TextRange(start: paragraphStart, end: paragraphEnd);
        DartRuntimePrimitives.Assert(() => paragraphRange.isNormalized);
        return _adjustTextBoundaryAtPosition(paragraphRange, position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (TextPosition boundaryEnd, TextPosition boundaryStart) _getClampedParagraphBoundaryAtPosition(TextPosition position)
    {
        var paragraphBoundary = new ParagraphBoundary(this.fullText);
        long paragraphStart = (paragraphBoundary.getLeadingTextBoundaryAt((((position.offset == this.fullText.Length) || (object.Equals(position.affinity, TextAffinity.upstream))) ? (position.offset - 1L) : position.offset)) ?? 0L);
        long paragraphEnd = (paragraphBoundary.getTrailingTextBoundaryAt(position.offset) ?? this.fullText.Length);
        paragraphStart = ((paragraphStart < this.range.start) ? this.range.start : ((paragraphStart > this.range.end) ? this.range.end : paragraphStart));
        paragraphEnd = ((paragraphEnd > this.range.end) ? this.range.end : ((paragraphEnd < this.range.start) ? this.range.start : paragraphEnd));
        var paragraphRange = new global::Doroti.Ui.TextRange(start: paragraphStart, end: paragraphEnd);
        DartRuntimePrimitives.Assert(() => paragraphRange.isNormalized);
        return _adjustTextBoundaryAtPosition(paragraphRange, position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _handleDirectionallyExtendSelection(double horizontalBaseline, bool isExtent, SelectionExtendDirection movement)
    {
        Matrix4 transform = this.paragraph.getTransformTo(null);
        if ((transform.invert() == 0.0))
        {
            switch (movement)
            {
                case SelectionExtendDirection.previousLine:
                case SelectionExtendDirection.backward:
                    {
                        return SelectionResult.previous;
                    }
                case SelectionExtendDirection.nextLine:
                case SelectionExtendDirection.forward:
                    {
                        return SelectionResult.next;
                    }
            }
        }
        double baselineInParagraphCoordinates = MatrixUtils.transformPoint(transform, new global::Doroti.Ui.Offset(horizontalBaseline, 0)).dx;
        DartRuntimePrimitives.Assert(() => !double.IsNaN(baselineInParagraphCoordinates));
        global::Doroti.Ui.TextPosition newPosition = default!;
        SelectionResult result = default!;
        switch (movement)
        {
            case SelectionExtendDirection.previousLine:
            case SelectionExtendDirection.nextLine:
                {
                    DartRuntimePrimitives.Assert(() => ((this._textSelectionEnd is not null) && (this._textSelectionStart is not null)));
                    global::Doroti.Ui.TextPosition targetedEdge = (isExtent ? this._textSelectionEnd! : this._textSelectionStart!);
                    MapEntry<global::Doroti.Ui.TextPosition, SelectionResult> moveResult = _handleVerticalMovement(targetedEdge, horizontalBaselineInParagraphCoordinates: baselineInParagraphCoordinates, below: (object.Equals(movement, SelectionExtendDirection.nextLine)));
                    newPosition = moveResult.key;
                    result = moveResult.value;
                    break;
                }
            case SelectionExtendDirection.forward:
            case SelectionExtendDirection.backward:
                {
                    _textSelectionEnd ??= ((object.Equals(movement, SelectionExtendDirection.forward)) ? new global::Doroti.Ui.TextPosition(offset: this.range.start) : new global::Doroti.Ui.TextPosition(offset: this.range.end, affinity: TextAffinity.upstream));
                    _textSelectionStart ??= this._textSelectionEnd;
                    global::Doroti.Ui.TextPosition targetedEdgeLocal = (isExtent ? this._textSelectionEnd! : this._textSelectionStart!);
                    global::Doroti.Ui.Offset edgeOffsetInParagraphCoordinates = this.paragraph._getOffsetForPosition(targetedEdgeLocal);
                    var baselineOffsetInParagraphCoordinates = new global::Doroti.Ui.Offset(baselineInParagraphCoordinates, (edgeOffsetInParagraphCoordinates.dy - (((RenderParagraph)this.paragraph)._textPainter.preferredLineHeight / 2L)));
                    newPosition = this.paragraph.getPositionForOffset(baselineOffsetInParagraphCoordinates);
                    result = SelectionResult.end;
                    break;
                }
        }
        if (isExtent)
        {
            _textSelectionEnd = newPosition;
        }
        else
        {
            _textSelectionStart = newPosition;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _handleGranularlyExtendSelection(bool forward, bool isExtent, TextGranularity granularity)
    {
        _textSelectionEnd ??= (forward ? new global::Doroti.Ui.TextPosition(offset: this.range.start) : new global::Doroti.Ui.TextPosition(offset: this.range.end, affinity: TextAffinity.upstream));
        _textSelectionStart ??= this._textSelectionEnd;
        global::Doroti.Ui.TextPosition targetedEdge = (isExtent ? this._textSelectionEnd! : this._textSelectionStart!);
        if ((forward && ((targetedEdge.offset == this.range.end))))
        {
            return SelectionResult.next;
        }
        if ((!forward && ((targetedEdge.offset == this.range.start))))
        {
            return SelectionResult.previous;
        }
        SelectionResult result = default!;
        global::Doroti.Ui.TextPosition newPosition = default!;
        switch (granularity)
        {
            case TextGranularity.character:
                {
                    string text = this.range.textInside(this.fullText);
                    newPosition = _moveBeyondTextBoundaryAtDirection(targetedEdge, forward, new CharacterBoundary(text));
                    result = SelectionResult.end;
                    break;
                }
            case TextGranularity.word:
                {
                    TextBoundary textBoundary = ((RenderParagraph)this.paragraph)._textPainter.wordBoundaries.moveByWordBoundary;
                    newPosition = _moveBeyondTextBoundaryAtDirection(targetedEdge, forward, textBoundary);
                    result = SelectionResult.end;
                    break;
                }
            case TextGranularity.paragraph:
                {
                    string textLocal = this.range.textInside(this.fullText);
                    newPosition = _moveBeyondTextBoundaryAtDirection(targetedEdge, forward, new ParagraphBoundary(textLocal));
                    result = SelectionResult.end;
                    break;
                }
            case TextGranularity.line:
                {
                    newPosition = _moveToTextBoundaryAtDirection(targetedEdge, forward, new LineBoundary(this));
                    result = SelectionResult.end;
                    break;
                }
            case TextGranularity.document:
                {
                    string textAlternate = this.range.textInside(this.fullText);
                    newPosition = _moveBeyondTextBoundaryAtDirection(targetedEdge, forward, new DocumentBoundary(textAlternate));
                    if ((forward && (newPosition.offset == this.range.end)))
                    {
                        result = SelectionResult.next;
                    }
                    else
                    {
                        if ((!forward && (newPosition.offset == this.range.start)))
                        {
                            result = SelectionResult.previous;
                        }
                        else
                        {
                            result = SelectionResult.end;
                        }
                    }
                    break;
                }
        }
        if (isExtent)
        {
            _textSelectionEnd = newPosition;
        }
        else
        {
            _textSelectionStart = newPosition;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _moveBeyondTextBoundaryAtDirection(TextPosition end, bool forward, TextBoundary textBoundary)
    {
        long newOffset = (forward ? (textBoundary.getTrailingTextBoundaryAt(end.offset) ?? this.range.end) : (textBoundary.getLeadingTextBoundaryAt((end.offset - 1L)) ?? this.range.start));
        return new global::Doroti.Ui.TextPosition(offset: newOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _moveToTextBoundaryAtDirection(TextPosition end, bool forward, TextBoundary textBoundary)
    {
        DartRuntimePrimitives.Assert(() => (end.offset >= 0L));
        long caretOffset = default!;
        switch (end.affinity)
        {
            case TextAffinity.upstream:
                {
                    if (((end.offset < 1L) && !forward))
                    {
                        DartRuntimePrimitives.Assert(() => (end.offset == 0L));
                        return new global::Doroti.Ui.TextPosition(offset: 0L);
                    }
                    var characterBoundary = new CharacterBoundary(this.fullText);
                    caretOffset = (Math.Max(0L, (characterBoundary.getLeadingTextBoundaryAt((this.range.start + end.offset)) ?? this.range.start)) - 1L);
                    break;
                }
            case TextAffinity.downstream:
                {
                    caretOffset = end.offset;
                    break;
                }
        }
        long offsetLocal = (forward ? (textBoundary.getTrailingTextBoundaryAt(caretOffset) ?? this.range.end) : (textBoundary.getLeadingTextBoundaryAt(caretOffset) ?? this.range.start));
        return new global::Doroti.Ui.TextPosition(offset: offsetLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual MapEntry<global::Doroti.Ui.TextPosition, SelectionResult> _handleVerticalMovement(TextPosition position, double horizontalBaselineInParagraphCoordinates, bool below)
    {
        List<global::Doroti.Ui.LineMetrics> lines = ((RenderParagraph)this.paragraph)._textPainter.computeLineMetrics();
        global::Doroti.Ui.Offset offsetLocal = this.paragraph.getOffsetForCaret(position, Rect.zero);
        long currentLine = (checked((long)(lines.Count)) - 1L);
        foreach (var lineMetrics in lines)
        {
            if ((lineMetrics.baseline > offsetLocal.dy))
            {
                currentLine = lineMetrics.lineNumber;
                break;
            }
        }
        global::Doroti.Ui.TextPosition newPosition = default!;
        if ((below && (currentLine == (checked((long)(lines.Count)) - 1L))))
        {
            newPosition = new global::Doroti.Ui.TextPosition(offset: this.range.end, affinity: TextAffinity.upstream);
        }
        else
        {
            if ((!below && (currentLine == 0L)))
            {
                newPosition = new global::Doroti.Ui.TextPosition(offset: this.range.start);
            }
            else
            {
                long newLine = (below ? (currentLine + 1L) : (currentLine - 1L));
                newPosition = _clampTextPosition(this.paragraph.getPositionForOffset(new global::Doroti.Ui.Offset(horizontalBaselineInParagraphCoordinates, lines[(int)(newLine)].baseline)));
            }
        }
        SelectionResult result = default!;
        if ((newPosition.offset == this.range.start))
        {
            result = SelectionResult.previous;
        }
        else
        {
            if ((newPosition.offset == this.range.end))
            {
                result = SelectionResult.next;
            }
            else
            {
                result = SelectionResult.end;
            }
        }
        DartRuntimePrimitives.Assert(() => ((!object.Equals(result, SelectionResult.next)) || below));
        DartRuntimePrimitives.Assert(() => ((!object.Equals(result, SelectionResult.previous)) || !below));
        return new MapEntry<global::Doroti.Ui.TextPosition, SelectionResult>(newPosition, result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _positionIsWithinCurrentSelection(TextPosition position)
    {
        if (((this._textSelectionStart is null) || (this._textSelectionEnd is null)))
        {
            return false;
        }
        global::Doroti.Ui.TextPosition currentStart = default!;
        global::Doroti.Ui.TextPosition currentEnd = default!;
        if ((_compareTextPositions(this._textSelectionStart!, this._textSelectionEnd!) > 0L))
        {
            currentStart = this._textSelectionStart!;
            currentEnd = this._textSelectionEnd!;
        }
        else
        {
            currentStart = this._textSelectionEnd!;
            currentEnd = this._textSelectionStart!;
        }
        return ((_compareTextPositions(currentStart, position) >= 0L) && (_compareTextPositions(currentEnd, position) <= 0L));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static long _compareTextPositions(TextPosition position, TextPosition otherPosition)
    {
        if ((position.offset < otherPosition.offset))
        {
            return 1L;
        }
        else
        {
            if ((position.offset > otherPosition.offset))
            {
                return -1L;
            }
            else
            {
                if ((object.Equals(position.affinity, otherPosition.affinity)))
                {
                    return 0L;
                }
                else
                {
                    return ((object.Equals(position.affinity, TextAffinity.upstream)) ? 1L : -1L);
                }
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Matrix4 getTransformTo(RenderObject? ancestor)
    {
        return this.paragraph.getTransformTo(ancestor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void pushHandleLayers(LayerLink? startHandle, LayerLink? endHandle)
    {
        if (!this.paragraph.attached)
        {
            DartRuntimePrimitives.Assert(() => ((startHandle is null) && (endHandle is null)));
            return;
        }
        if ((!object.Equals(this._startHandleLayerLink, startHandle)))
        {
            _startHandleLayerLink = startHandle;
            this.paragraph.markNeedsPaint();
        }
        if ((!object.Equals(this._endHandleLayerLink, endHandle)))
        {
            _endHandleLayerLink = endHandle;
            this.paragraph.markNeedsPaint();
        }
    }

    public virtual List<Rect> boundingBoxes
    {
        get
        {
            if ((this._cachedBoundingBoxes is null))
            {
                List<global::Doroti.Ui.TextBox> boxes = this.paragraph.getBoxesForSelection(new TextSelection(baseOffset: this.range.start, extentOffset: this.range.end), boxHeightStyle: BoxHeightStyle.max);
                if ((checked((long)(boxes.Count)) != 0))
                {
                    _cachedBoundingBoxes = new List<global::Doroti.Ui.Rect>();
                    foreach (var textBox in boxes)
                    {
                        this._cachedBoundingBoxes!.Add(textBox.toRect());
                    }
                }
                else
                {
                    global::Doroti.Ui.Offset offsetLocal = this.paragraph._getOffsetForPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start));
                    var rect = global::Doroti.Ui.Rect.fromPoints(offsetLocal, offsetLocal.translate(0, -((RenderParagraph)this.paragraph)._textPainter.preferredLineHeight));
                    _cachedBoundingBoxes = new List<global::Doroti.Ui.Rect> { rect };
                }
            }
            return this._cachedBoundingBoxes!;
            return default!;
        }
    }
    internal virtual global::Doroti.Ui.Rect _rect
    {
        get
        {
            if ((this._cachedRect is null))
            {
                List<global::Doroti.Ui.TextBox> boxes = this.paragraph.getBoxesForSelection(new TextSelection(baseOffset: this.range.start, extentOffset: this.range.end), boxHeightStyle: BoxHeightStyle.max);
                if ((checked((long)(boxes.Count)) != 0))
                {
                    global::Doroti.Ui.Rect result = boxes.First().toRect();
                    for (var index = 1L; (index < checked((long)(boxes.Count))); index += 1L)
                    {
                        result = result.expandToInclude(boxes[(int)(index)].toRect());
                    }
                    _cachedRect = result;
                }
                else
                {
                    global::Doroti.Ui.Offset offsetLocal = this.paragraph._getOffsetForPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start));
                    _cachedRect = global::Doroti.Ui.Rect.fromPoints(offsetLocal, offsetLocal.translate(0, -((RenderParagraph)this.paragraph)._textPainter.preferredLineHeight));
                }
            }
            return DartRuntimePrimitives.RequireValue(this._cachedRect);
            return default!;
        }
    }
    public virtual void didChangeParagraphLayout()
    {
        _cachedRect = null;
        _cachedBoundingBoxes = null;
    }

    public virtual long contentLength => (this.range.end - this.range.start);
    public virtual Size size
    {
        get
        {
            return this._rect.size;
            return default!;
        }
    }
    public virtual void paintSelection(PaintingContext context, Offset offset)
    {
        if (((this._textSelectionStart is null) || (this._textSelectionEnd is null)))
        {
            return;
        }
        if ((((RenderParagraph)this.paragraph).selectionColor is not null))
        {
            var selection = new TextSelection(baseOffset: this._textSelectionStart!.offset, extentOffset: this._textSelectionEnd!.offset);
            var selectionPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.style = PaintingStyle.fill;
    __cascade.color = ((RenderParagraph)this.paragraph).selectionColor!;
    return __cascade;
}))();
            foreach (global::Doroti.Ui.TextBox textBox in this.paragraph.getBoxesForSelection(selection))
            {
                ((PaintingContext)context).canvas.drawRect(textBox.toRect().shift(offset), selectionPaint);
            }
        }
    }

    public virtual void paintHandles(PaintingContext context, Offset offset)
    {
        if (((this._textSelectionStart is null) || (this._textSelectionEnd is null)))
        {
            return;
        }
        if (((this._startHandleLayerLink is not null) && (((SelectionGeometry)this.value).startSelectionPoint is not null)))
        {
            context.pushLayer(new LeaderLayer(link: this._startHandleLayerLink!, offset: (offset + ((SelectionGeometry)this.value).startSelectionPoint!.localPosition)), ((Action<PaintingContext, Offset>)((context, offset) =>
            {
            })), Offset.zero);
        }
        if (((this._endHandleLayerLink is not null) && (((SelectionGeometry)this.value).endSelectionPoint is not null)))
        {
            context.pushLayer(new LeaderLayer(link: this._endHandleLayerLink!, offset: (offset + ((SelectionGeometry)this.value).endSelectionPoint!.localPosition)), ((Action<PaintingContext, Offset>)((context, offset) =>
            {
            })), Offset.zero);
        }
    }

    public virtual TextSelection getLineAtOffset(TextPosition position)
    {
        global::Doroti.Ui.TextRange line = this.paragraph._getLineAtOffset(position);
        long startLocal = line.start.clamp(this.range.start, this.range.end);
        long endLocal = line.end.clamp(this.range.start, this.range.end);
        return new TextSelection(baseOffset: startLocal, extentOffset: endLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.TextPosition getTextPositionAbove(TextPosition position)
    {
        return _clampTextPosition(this.paragraph._getTextPositionAbove(position));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.TextPosition getTextPositionBelow(TextPosition position)
    {
        return _clampTextPosition(this.paragraph._getTextPositionBelow(position));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.TextRange getWordBoundary(TextPosition position) => this.paragraph.getWordBoundary(position);
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<string>("textInsideRange", this.range.textInside(this.fullText)));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.TextRange>("range", this.range));
        properties.add(new DiagnosticsProperty<string>("fullText", this.fullText));
    }

}

