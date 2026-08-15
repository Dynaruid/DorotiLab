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

namespace Doroti.Generated.Framework.Rendering;

internal delegate void _TextBoundaryRecord__paragraph();

internal delegate (TextPosition boundaryEnd, TextPosition boundaryStart) _TextBoundaryAtPosition__paragraph(TextPosition position);

internal delegate (TextPosition boundaryEnd, TextPosition boundaryStart) _TextBoundaryAtPositionInText__paragraph(TextPosition position, string text);

public static partial class ParagraphLibrary
{
    internal static string _kEllipsis = "…";
}

public class PlaceholderSpanIndexSemanticsTag : global::Doroti.Generated.Framework.Semantics.SemanticsTag
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
    public virtual global::Doroti.Generated.Framework.Painting.PlaceholderSpan? span { get; set; } = default;
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
    public static global::Doroti.Generated.Framework.Painting.PlaceholderDimensions _layoutChild(RenderBox child, BoxConstraints childConstraints, Func<RenderBox, BoxConstraints, Size> layoutChild, Func<RenderBox, BoxConstraints, TextBaseline, double?> getBaseline)
    {
        var parentData__5133 = ((TextParentData?)(object?)child.parentData!)!;
        global::Doroti.Generated.Framework.Painting.PlaceholderSpan? span__5210 = ((TextParentData)parentData__5133).span;
        DartRuntimePrimitives.Assert(() => (span__5210 is not null));
        return ((span__5210 is null) ? global::Doroti.Generated.Framework.Painting.PlaceholderDimensions.empty : new global::Doroti.Generated.Framework.Painting.PlaceholderDimensions(size: layoutChild(child, childConstraints), alignment: ((global::Doroti.Generated.Framework.Painting.PlaceholderSpan)span__5210).alignment, baseline: ((global::Doroti.Generated.Framework.Painting.PlaceholderSpan)span__5210).baseline, baselineOffset: (((global::Doroti.Generated.Framework.Painting.PlaceholderSpan)span__5210).alignment switch { Dart_uiLibrary.PlaceholderAlignment.aboveBaseline or Dart_uiLibrary.PlaceholderAlignment.belowBaseline or Dart_uiLibrary.PlaceholderAlignment.bottom or Dart_uiLibrary.PlaceholderAlignment.middle => null, Dart_uiLibrary.PlaceholderAlignment.top => null, Dart_uiLibrary.PlaceholderAlignment.baseline => getBaseline(child, childConstraints, DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Painting.PlaceholderSpan)span__5210).baseline)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })));
    }
    public List<global::Doroti.Generated.Framework.Painting.PlaceholderDimensions> layoutInlineChildren(double maxWidth, Func<RenderBox, BoxConstraints, Size> layoutChild, Func<RenderBox, BoxConstraints, TextBaseline, double?> getChildBaseline);
    public void positionInlineChildren(List<TextBox> boxes);
    public void defaultApplyPaintTransform(RenderBox child, Matrix4 transform);
    public void paintInlineChildren(PaintingContext context, Offset offset);
    public bool hitTestInlineChildren(BoxHitTestResult result, Offset position);
}

internal class _UnspecifiedTextScaler__paragraph : global::Doroti.Generated.Framework.Painting.TextScaler
{
    internal _UnspecifiedTextScaler__paragraph()
    {
    }

    public override double textScaleFactor => throw new NotImplementedException();
    public override double scale(double fontSize) => throw new NotImplementedException();
}

public class RenderParagraph : RenderBox, ContainerRenderObjectMixin<RenderBox, TextParentData>, RenderInlineChildrenContainerDefaults, RelayoutWhenSystemFontsChangeMixin
{
    internal static string _placeholderCharacter = char.ConvertFromUtf32(checked((int)global::Doroti.Generated.Framework.Painting.PlaceholderSpan.placeholderCodeUnit));
    internal virtual global::Doroti.Generated.Framework.Painting.TextPainter _textPainter { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Painting.TextPainter? _textIntrinsicsCache { get; set; } = default;
    internal virtual List<global::Doroti.Generated.Framework.Semantics.AttributedString>? _cachedAttributedLabels { get; set; } = default;
    internal virtual List<global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation>? _cachedCombinedSemanticsInfos { get; set; } = default;
    internal virtual List<_SelectableFragment__paragraph>? _lastSelectableFragments { get; set; } = default;
    internal virtual SelectionRegistrar? _registrar { get; set; } = default;
    internal virtual bool _softWrap { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Painting.TextOverflow _overflow { get; set; } = default!;
    internal virtual double _devicePixelRatio { get; set; } = default!;
    internal virtual Color? _selectionColor { get; set; } = default;
    internal virtual bool _needsClipping { get; set; } = false;
    internal virtual Shader? _overflowShader { get; set; } = default;
    internal virtual List<global::Doroti.Generated.Framework.Painting.PlaceholderDimensions>? _placeholderDimensions { get; set; } = default;
    internal virtual List<global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation>? _semanticsInfo { get; set; } = default;
    internal virtual DartMap<Key, global::Doroti.Generated.Framework.Semantics.SemanticsNode>? _cachedChildNodes { get; set; } = default;
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;
    public virtual bool _hasPendingSystemFontsDidChangeCallBack { get; set; } = false;

    public RenderParagraph(global::Doroti.Generated.Framework.Painting.InlineSpan text, TextAlign textAlign = TextAlign.start, TextDirection textDirection = default!, bool softWrap = true, global::Doroti.Generated.Framework.Painting.TextOverflow overflow = TextOverflow.clip, double textScaleFactor = 1.0, global::Doroti.Generated.Framework.Painting.TextScaler textScaler = default!, long? maxLines = null, Locale? locale = null, global::Doroti.Generated.Framework.Painting.StrutStyle? strutStyle = null, global::Doroti.Generated.Framework.Painting.TextWidthBasis textWidthBasis = TextWidthBasis.parent, TextHeightBehavior? textHeightBehavior = null, List<RenderBox>? children = null, Color? selectionColor = null, SelectionRegistrar? registrar = null, double devicePixelRatio = 1.0)
    {
        global::Doroti.Generated.Framework.Painting.TextScaler __textScaler = textScaler ?? new _UnspecifiedTextScaler__paragraph();
        this._softWrap = softWrap;
        this._overflow = overflow;
        this._devicePixelRatio = devicePixelRatio;
        this._selectionColor = selectionColor;
        this._textPainter = new global::Doroti.Generated.Framework.Painting.TextPainter(text: text, textAlign: textAlign, textDirection: textDirection, textScaler: ((object.Equals(textScaler, new _UnspecifiedTextScaler__paragraph())) ? global::Doroti.Generated.Framework.Painting.TextScaler.CreateLinear(textScaleFactor) : textScaler), maxLines: maxLines, ellipsis: ((object.Equals(overflow, global::Doroti.Generated.Framework.Painting.TextOverflow.ellipsis)) ? ParagraphLibrary._kEllipsis : null), locale: locale, strutStyle: strutStyle, textWidthBasis: textWidthBasis, textHeightBehavior: textHeightBehavior);
        System.Diagnostics.Debug.Assert(text.debugAssertIsValid());
        System.Diagnostics.Debug.Assert(((maxLines is null) || (DartRuntimePrimitives.RequireValue(maxLines) > 0L)));
        System.Diagnostics.Debug.Assert((DartRuntimePrimitives.Identical(__textScaler, new _UnspecifiedTextScaler__paragraph()) || (textScaleFactor == 1.0)));
    }

    internal virtual global::Doroti.Generated.Framework.Painting.TextPainter _textIntrinsics
    {
        get
        {
            return ((Func<global::Doroti.Generated.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = (_textIntrinsicsCache ??= new global::Doroti.Generated.Framework.Painting.TextPainter());
    __cascade.text = ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).text;
    __cascade.textAlign = ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).textAlign;
    __cascade.textDirection = ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).textDirection;
    __cascade.textScaler = ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).textScaler;
    __cascade.maxLines = ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).maxLines;
    __cascade.ellipsis = ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).ellipsis;
    __cascade.locale = ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).locale;
    __cascade.strutStyle = ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).strutStyle;
    __cascade.textWidthBasis = ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).textWidthBasis;
    __cascade.textHeightBehavior = ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).textHeightBehavior;
    return __cascade;
}))();
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Painting.InlineSpan text
    {
        get => ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).text!;
        set
        {
            var __value = value;
            switch (((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).text!.compareTo(__value))
            {
                case global::Doroti.Generated.Framework.Painting.RenderComparison.identical:
                    {
                        return;
                    }
                case global::Doroti.Generated.Framework.Painting.RenderComparison.metadata:
                    {
                        this._textPainter.text = __value;
                        _cachedCombinedSemanticsInfos = null;
                        markNeedsSemanticsUpdate();
                        break;
                    }
                case global::Doroti.Generated.Framework.Painting.RenderComparison.paint:
                    {
                        this._textPainter.text = __value;
                        _cachedAttributedLabels = null;
                        _cachedCombinedSemanticsInfos = null;
                        markNeedsPaint();
                        markNeedsSemanticsUpdate();
                        break;
                    }
                case global::Doroti.Generated.Framework.Painting.RenderComparison.layout:
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
            var results__16334 = new List<TextSelection>();
            foreach (_SelectableFragment__paragraph fragment__16398 in this._lastSelectableFragments!)
            {
                if (((((_SelectableFragment__paragraph)fragment__16398)._textSelectionStart is not null) && (((_SelectableFragment__paragraph)fragment__16398)._textSelectionEnd is not null)))
                {
                    results__16334.Add(new TextSelection(baseOffset: ((_SelectableFragment__paragraph)fragment__16398)._textSelectionStart!.offset, extentOffset: ((_SelectableFragment__paragraph)fragment__16398)._textSelectionEnd!.offset));
                }
            }
            return results__16334;
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
        string plainText__18000 = this.text.toPlainText(includeSemanticsLabels: false);
        var result__18071 = new List<_SelectableFragment__paragraph>();
        var start__18113 = 0L;
        while ((start__18113 < plainText__18000.Length))
        {
            long end__18173 = plainText__18000.IndexOf(_placeholderCharacter, checked((int)(start__18113)));
            if ((start__18113 != end__18173))
            {
                if ((end__18173 == -1L))
                {
                    end__18173 = plainText__18000.Length;
                }
                result__18071.Add(new _SelectableFragment__paragraph(paragraph: this, range: new global::Doroti.Ui.TextRange(start: start__18113, end: end__18173), fullText: plainText__18000));
                start__18113 = end__18173;
            }
            start__18113 += 1L;
        }
        return result__18071;
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
        foreach (_SelectableFragment__paragraph fragment__19152 in this._lastSelectableFragments!)
        {
            fragment__19152.dispose();
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
        get => ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).textAlign;
        set
        {
            var __value = value;
            if ((object.Equals(((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).textAlign, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            this._textPainter.textAlign = DartRuntimePrimitives.RequireValue(__value);
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).textDirection);
        set
        {
            var __value = value;
            if ((object.Equals(((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).textDirection, DartRuntimePrimitives.RequireValue(__value))))
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
    public virtual global::Doroti.Generated.Framework.Painting.TextOverflow overflow
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
            this._textPainter.ellipsis = ((object.Equals(DartRuntimePrimitives.RequireValue(__value), global::Doroti.Generated.Framework.Painting.TextOverflow.ellipsis)) ? ParagraphLibrary._kEllipsis : null);
            markNeedsLayout();
        }
    }
    public virtual double textScaleFactor
    {
        get => ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).textScaleFactor;
        set
        {
            var __value = value;
            textScaler = global::Doroti.Generated.Framework.Painting.TextScaler.CreateLinear(DartRuntimePrimitives.RequireValue(__value));
        }
    }
    public virtual global::Doroti.Generated.Framework.Painting.TextScaler textScaler
    {
        get => ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).textScaler;
        set
        {
            var __value = value;
            if ((object.Equals(((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).textScaler, __value)))
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
            if (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb)
            {
                markNeedsPaint();
            }
        }
    }
    public virtual long? maxLines
    {
        get => ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).maxLines;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((__value is null) || (DartRuntimePrimitives.RequireValue(__value) > 0L)));
            if ((((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).maxLines == __value))
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
        get => ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).locale;
        set
        {
            var __value = value;
            if ((object.Equals(((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).locale, __value)))
            {
                return;
            }
            this._textPainter.locale = __value;
            _overflowShader = null;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Generated.Framework.Painting.StrutStyle? strutStyle
    {
        get => ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).strutStyle;
        set
        {
            var __value = value;
            if ((object.Equals(((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).strutStyle, __value)))
            {
                return;
            }
            this._textPainter.strutStyle = __value;
            _overflowShader = null;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Generated.Framework.Painting.TextWidthBasis textWidthBasis
    {
        get => ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).textWidthBasis;
        set
        {
            var __value = value;
            if ((object.Equals(((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).textWidthBasis, DartRuntimePrimitives.RequireValue(__value))))
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
        get => ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).textHeightBehavior;
        set
        {
            var __value = value is null ? null : (TextHeightBehavior)(object)value;
            if ((object.Equals(((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).textHeightBehavior, __value)))
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
        List<global::Doroti.Generated.Framework.Painting.PlaceholderDimensions> placeholderDimensions__26445 = layoutInlineChildren(double.PositiveInfinity, ((Func<RenderBox, BoxConstraints, Size>)((child, constraints) => new global::Doroti.Ui.Size(child.getMinIntrinsicWidth(double.PositiveInfinity), 0.0))), (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline);
        return (((Func<global::Doroti.Generated.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textIntrinsics;
    __cascade.setPlaceholderDimensions(placeholderDimensions__26445);
    __cascade.layout();
    return __cascade;
}))()).minIntrinsicWidth;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        List<global::Doroti.Generated.Framework.Painting.PlaceholderDimensions> placeholderDimensions__26926 = layoutInlineChildren(double.PositiveInfinity, ((Func<RenderBox, BoxConstraints, Size>)((child, constraints) => new global::Doroti.Ui.Size(child.getMaxIntrinsicWidth(double.PositiveInfinity), 0.0))), (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline);
        return (((Func<global::Doroti.Generated.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textIntrinsics;
    __cascade.setPlaceholderDimensions(placeholderDimensions__26926);
    __cascade.layout();
    return __cascade;
}))()).maxIntrinsicWidth;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double preferredLineHeight => ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).preferredLineHeight;
    internal virtual double _computeIntrinsicHeight(double width)
    {
        return (((Func<global::Doroti.Generated.Framework.Painting.TextPainter>)(() =>
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
        global::Doroti.Ui.GlyphInfo? glyph__28489 = this._textPainter.getClosestGlyphForOffset(position);
        global::Doroti.Generated.Framework.Painting.InlineSpan? spanHit__28914 = (((glyph__28489 is not null) && glyph__28489.graphemeClusterLayoutBounds.contains(position)) ? ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).text!.getSpanForPosition(new global::Doroti.Ui.TextPosition(offset: glyph__28489.graphemeClusterCodeUnitRange.start)) : null);
        switch (spanHit__28914)
        {
            case HitTestTarget span__29209:
                {
                    result.add(new HitTestEntry<HitTestTarget>(span__29209));
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
        return ((this.softWrap || (object.Equals(this.overflow, global::Doroti.Generated.Framework.Painting.TextOverflow.ellipsis))) ? maxWidth : double.PositiveInfinity);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _layoutTextWithConstraints(BoxConstraints constraints)
    {
        ((Func<global::Doroti.Generated.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textPainter;
    __cascade.setPlaceholderDimensions(this._placeholderDimensions);
    __cascade.layout(minWidth: ((BoxConstraints)constraints).minWidth, maxWidth: _adjustMaxWidth(((BoxConstraints)constraints).maxWidth));
    return __cascade;
}))();
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        global::Doroti.Ui.Size size__30627 = (((Func<global::Doroti.Generated.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._textIntrinsics;
    __cascade.setPlaceholderDimensions(layoutInlineChildren(((BoxConstraints)constraints).maxWidth, (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.dryLayoutChild, (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline));
    __cascade.layout(minWidth: ((BoxConstraints)constraints).minWidth, maxWidth: _adjustMaxWidth(((BoxConstraints)constraints).maxWidth));
    return __cascade;
}))()).size;
        return constraints.constrain(size__30627);
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
        ((Func<global::Doroti.Generated.Framework.Painting.TextPainter>)(() =>
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
        BoxConstraints constraints__32594 = this.constraints;
        _placeholderDimensions = layoutInlineChildren(((BoxConstraints)constraints__32594).maxWidth, (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.layoutChild, (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getBaseline);
        _layoutTextWithConstraints(constraints__32594);
        positionInlineChildren(((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).inlinePlaceholderBoxes!);
        global::Doroti.Ui.Size textSize__32913 = ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).size;
        size = constraints__32594.constrain(textSize__32913);
        bool didOverflowHeight__33003 = ((size.height < textSize__32913.height) || ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).didExceedMaxLines);
        bool didOverflowWidth__33103 = (size.width < textSize__32913.width);
        bool hasVisualOverflow__33547 = (didOverflowWidth__33103 || didOverflowHeight__33003);
        if (hasVisualOverflow__33547)
        {
            switch (this._overflow)
            {
                case global::Doroti.Generated.Framework.Painting.TextOverflow.visible:
                    {
                        _needsClipping = false;
                        _overflowShader = null;
                        break;
                    }
                case global::Doroti.Generated.Framework.Painting.TextOverflow.clip:
                case global::Doroti.Generated.Framework.Painting.TextOverflow.ellipsis:
                    {
                        _needsClipping = true;
                        _overflowShader = null;
                        break;
                    }
                case global::Doroti.Generated.Framework.Painting.TextOverflow.fade:
                    {
                        _needsClipping = true;
                        var fadeSizePainter__33981 = ((Func<global::Doroti.Generated.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = new global::Doroti.Generated.Framework.Painting.TextPainter(text: new global::Doroti.Generated.Framework.Painting.TextSpan(style: ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).text!.style, text: "…"), textDirection: this.textDirection, textScaler: this.textScaler, locale: this.locale);
    __cascade.layout();
    return __cascade;
}))();
                        if (didOverflowWidth__33103)
                        {
                            var (fadeStart__34278, fadeEnd__34296) = (this.textDirection switch { TextDirection.rtl => (((double, double))((((global::Doroti.Generated.Framework.Painting.TextPainter)fadeSizePainter__33981).width, 0.0))), TextDirection.ltr => (((double, double))(((size.width - ((global::Doroti.Generated.Framework.Painting.TextPainter)fadeSizePainter__33981).width), size.width))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                            _overflowShader = global::Doroti.Ui.Gradient.linear(new global::Doroti.Ui.Offset(fadeStart__34278, 0.0), new global::Doroti.Ui.Offset(fadeEnd__34296, 0.0), new List<global::Doroti.Ui.Color> { new global::Doroti.Ui.Color(4294967295L), new global::Doroti.Ui.Color(16777215L) });
                        }
                        else
                        {
                            double fadeEnd__34753 = size.height;
                            double fadeStart__34801 = (fadeEnd__34753 - (((global::Doroti.Generated.Framework.Painting.TextPainter)fadeSizePainter__33981).height / 2.0));
                            _overflowShader = global::Doroti.Ui.Gradient.linear(new global::Doroti.Ui.Offset(0.0, fadeStart__34801), new global::Doroti.Ui.Offset(0.0, fadeEnd__34753), new List<global::Doroti.Ui.Color> { new global::Doroti.Ui.Color(4294967295L), new global::Doroti.Ui.Color(16777215L) });
                        }
                        fadeSizePainter__33981.dispose();
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
                if (global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugRepaintTextRainbowEnabled)
                {
                    var paint__35729 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugCurrentRepaintColor.toColor();
    return __cascade;
}))();
                    ((PaintingContext)context).canvas.drawRect((offset & size), paint__35729);
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
            foreach (_SelectableFragment__paragraph fragment__36079 in this._lastSelectableFragments!)
            {
                fragment__36079.paintSelection(context, offset);
            }
            if (this._needsClipping)
            {
                ((PaintingContext)context).canvas.restore();
            }
        }
        if (this._needsClipping)
        {
            global::Doroti.Ui.Rect bounds__36298 = (offset & size);
            if ((this._overflowShader is not null))
            {
                ((PaintingContext)context).canvas.saveLayer(bounds__36298, new global::Doroti.Ui.Paint());
            }
            else
            {
                ((PaintingContext)context).canvas.save();
            }
            ((PaintingContext)context).canvas.clipRect(bounds__36298);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                this._textPainter.debugPaintTextLayoutBoxes = global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugPaintTextLayoutBoxes;
                return true;
            });
        this._textPainter.paint(((PaintingContext)context).canvas, offset);
        paintInlineChildren(context, offset);
        if (this._needsClipping)
        {
            if ((this._overflowShader is not null))
            {
                ((PaintingContext)context).canvas.translate(offset.dx, offset.dy);
                var paint__36994 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.blendMode = BlendMode.modulate;
    __cascade.shader = this._overflowShader;
    return __cascade;
}))();
                ((PaintingContext)context).canvas.drawRect((Offset.zero & size), paint__36994);
            }
            ((PaintingContext)context).canvas.restore();
        }
        if ((this._lastSelectableFragments is not null))
        {
            foreach (_SelectableFragment__paragraph fragment__37279 in this._lastSelectableFragments!)
            {
                fragment__37279.paintHandles(context, offset);
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
        double preferredLineHeight__40319 = ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).preferredLineHeight;
        double verticalOffset__40392 = (-0.5 * preferredLineHeight__40319);
        return _getTextPositionVertical(position, verticalOffset__40392);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _getTextPositionBelow(TextPosition position)
    {
        double preferredLineHeight__40658 = ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).preferredLineHeight;
        double verticalOffset__40731 = (1.5 * preferredLineHeight__40658);
        return _getTextPositionVertical(position, verticalOffset__40731);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _getTextPositionVertical(TextPosition position, double verticalOffset)
    {
        global::Doroti.Ui.Offset caretOffset__40948 = this._textPainter.getOffsetForCaret(position, Rect.zero);
        global::Doroti.Ui.Offset caretOffsetTranslated__41032 = caretOffset__40948.translate(0.0, verticalOffset);
        return this._textPainter.getPositionForOffset(caretOffsetTranslated__41032);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Size textSize
    {
        get
        {
            DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
            return ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).size;
            return default!;
        }
    }
    public virtual bool didExceedMaxLines
    {
        get
        {
            DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
            return ((global::Doroti.Generated.Framework.Painting.TextPainter)this._textPainter).didExceedMaxLines;
            return default!;
        }
    }
    public override void describeSemanticsConfiguration(global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        _semanticsInfo = this.text.getSemanticsInformation();
        var needsAssembleSemanticsNode__42289 = false;
        var needsChildConfigurationsDelegate__42333 = false;
        foreach (global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation info__42421 in this._semanticsInfo!)
        {
            if (((((global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation)info__42421).recognizer is not null) || (((global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation)info__42421).semanticsIdentifier is not null)))
            {
                needsAssembleSemanticsNode__42289 = true;
                break;
            }
            needsChildConfigurationsDelegate__42333 = (needsChildConfigurationsDelegate__42333 || ((global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation)info__42421).isPlaceholder);
        }
        if (needsAssembleSemanticsNode__42289)
        {
            config.explicitChildNodes = true;
            config.isSemanticBoundary = true;
        }
        else
        {
            if (needsChildConfigurationsDelegate__42333)
            {
                config.childConfigurationsDelegate = this._childSemanticsConfigurationsDelegate;
            }
            else
            {
                if ((this._cachedAttributedLabels is null))
                {
                    var buffer__43014 = new StringBuffer();
                    var offset__43051 = 0L;
                    var attributes__43077 = new List<global::Doroti.Ui.StringAttribute>();
                    foreach (global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation info__43161 in this._semanticsInfo!)
                    {
                        string label__43211 = (((global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation)info__43161).semanticsLabel ?? ((global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation)info__43161).text);
                        foreach (global::Doroti.Ui.StringAttribute infoAttribute__43290 in ((global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation)info__43161).stringAttributes)
                        {
                            global::Doroti.Ui.TextRange originalRange__43360 = infoAttribute__43290.range;
                            attributes__43077.Add(infoAttribute__43290.copy(range: new global::Doroti.Ui.TextRange(start: (offset__43051 + originalRange__43360.start), end: (offset__43051 + originalRange__43360.end))));
                        }
                        buffer__43014.write(label__43211);
                        offset__43051 += label__43211.Length;
                    }
                    _cachedAttributedLabels = new List<global::Doroti.Generated.Framework.Semantics.AttributedString> { new global::Doroti.Generated.Framework.Semantics.AttributedString(buffer__43014.ToString(), attributes: attributes__43077) };
                }
                config.attributedLabel = this._cachedAttributedLabels![(int)(0L)];
                config.textDirection = this.textDirection;
            }
        }
    }

    internal virtual global::Doroti.Generated.Framework.Semantics.ChildSemanticsConfigurationsResult _childSemanticsConfigurationsDelegate(List<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration> childConfigs)
    {
        var builder__44135 = new global::Doroti.Generated.Framework.Semantics.ChildSemanticsConfigurationsResultBuilder();
        var placeholderIndex__44198 = 0L;
        var childConfigsIndex__44228 = 0L;
        var attributedLabelCacheIndex__44259 = 0L;
        _cachedCombinedSemanticsInfos ??= global::Doroti.Generated.Framework.Painting.Inline_spanLibrary.combineSemanticsInfo(this._semanticsInfo!);
        foreach (global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation info__44413 in this._cachedCombinedSemanticsInfos!)
        {
            if (((global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation)info__44413).isPlaceholder)
            {
                while (((childConfigsIndex__44228 < checked((long)(childConfigs.Count))) && _childConfigBelongsToPlaceholder(childConfigs[(int)(childConfigsIndex__44228)], placeholderIndex__44198)))
                {
                    builder__44135.markAsMergeUp(childConfigs[(int)(childConfigsIndex__44228)]);
                    childConfigsIndex__44228 += 1L;
                }
                placeholderIndex__44198 += 1L;
            }
            else
            {
                builder__44135.markAsMergeUp(_createSemanticsConfigForTextInfo(info__44413, attributedLabelCacheIndex__44259));
                attributedLabelCacheIndex__44259 += 1L;
            }
        }
        return builder__44135.build();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static bool _childConfigBelongsToPlaceholder(global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration childConfig, long placeholderIndex)
    {
        IEnumerable<global::Doroti.Generated.Framework.Semantics.SemanticsTag>? tags__45584 = ((global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration)childConfig).tagsForChildren;
        if ((tags__45584 is null))
        {
            return false;
        }
        foreach (global::Doroti.Generated.Framework.Semantics.SemanticsTag tag__45698 in tags__45584)
        {
            if ((tag__45698 is PlaceholderSpanIndexSemanticsTag))
            {
                PlaceholderSpanIndexSemanticsTag tag__45698__as45723 = (PlaceholderSpanIndexSemanticsTag)tag__45698;
                return (((PlaceholderSpanIndexSemanticsTag)((PlaceholderSpanIndexSemanticsTag)tag__45698__as45723)).index == placeholderIndex);
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration _createSemanticsConfigForTextInfo(global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation textInfo, long cacheIndex)
    {
        DartRuntimePrimitives.Assert(() => !((global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation)textInfo).requiresOwnNode);
        List<global::Doroti.Generated.Framework.Semantics.AttributedString> cachedStrings__46052 = _cachedAttributedLabels ??= new List<global::Doroti.Generated.Framework.Semantics.AttributedString>();
        DartRuntimePrimitives.Assert(() => (cacheIndex <= checked((long)(cachedStrings__46052.Count))));
        bool hasCache__46181 = (cacheIndex < checked((long)(cachedStrings__46052.Count)));
        global::Doroti.Generated.Framework.Semantics.AttributedString attributedLabel__46254 = default!;
        if (hasCache__46181)
        {
            attributedLabel__46254 = cachedStrings__46052[(int)(cacheIndex)];
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (checked((long)(cachedStrings__46052.Count)) == cacheIndex));
            attributedLabel__46254 = new global::Doroti.Generated.Framework.Semantics.AttributedString((((global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation)textInfo).semanticsLabel ?? ((global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation)textInfo).text), attributes: ((global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation)textInfo).stringAttributes);
            cachedStrings__46052.Add(attributedLabel__46254);
        }
        return ((Func<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration>)(() =>
{
    var __cascade = new global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration();
    __cascade.textDirection = this.textDirection;
    __cascade.attributedLabel = attributedLabel__46254;
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void assembleSemanticsNode(global::Doroti.Generated.Framework.Semantics.SemanticsNode node, global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration config, IEnumerable<global::Doroti.Generated.Framework.Semantics.SemanticsNode> children)
    {
        DartRuntimePrimitives.Assert(() => ((this._semanticsInfo is not null) && (checked((long)(this._semanticsInfo!.Count)) != 0)));
        var newChildren__47251 = new List<global::Doroti.Generated.Framework.Semantics.SemanticsNode>();
        global::Doroti.Ui.TextDirection currentDirection__47302 = this.textDirection;
        global::Doroti.Ui.Rect currentRect__47345 = default!;
        var ordinal__47366 = 0.0;
        var start__47389 = 0L;
        var placeholderIndex__47408 = 0L;
        var childIndex__47438 = 0L;
        RenderBox? child__47469 = firstChild;
        var newChildCache__47499 = new DartMap<Key, global::Doroti.Generated.Framework.Semantics.SemanticsNode>();
        _cachedCombinedSemanticsInfos ??= global::Doroti.Generated.Framework.Painting.Inline_spanLibrary.combineSemanticsInfo(this._semanticsInfo!);
        foreach (global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation info__47662 in this._cachedCombinedSemanticsInfos!)
        {
            var selection__47716 = new TextSelection(baseOffset: start__47389, extentOffset: (start__47389 + ((global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation)info__47662).text.Length));
            start__47389 += ((global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation)info__47662).text.Length;
            if (((global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation)info__47662).isPlaceholder)
            {
                while (((children.Count() > childIndex__47438) && children.elementAt(childIndex__47438).isTagged(new PlaceholderSpanIndexSemanticsTag(placeholderIndex__47408))))
                {
                    global::Doroti.Generated.Framework.Semantics.SemanticsNode childNode__48235 = children.elementAt(childIndex__47438);
                    var parentData__48295 = ((TextParentData?)(object?)child__47469!.parentData!)!;
                    if ((((TextParentData)parentData__48295).offset is not null))
                    {
                        newChildren__47251.Add(childNode__48235);
                    }
                    childIndex__47438 += 1L;
                }
                child__47469 = childAfter(child__47469!);
                placeholderIndex__47408 += 1L;
            }
            else
            {
                var initialDirection__48651 = currentDirection__47302;
                List<global::Doroti.Ui.TextBox> rects__48719 = getBoxesForSelection(selection__47716);
                if ((checked((long)(rects__48719.Count)) == 0))
                {
                    continue;
                }
                global::Doroti.Ui.Rect rect__48832 = rects__48719.First().toRect();
                currentDirection__47302 = rects__48719.First().direction;
                foreach (global::Doroti.Ui.TextBox textBox__48941 in rects__48719.skip(1L))
                {
                    rect__48832 = rect__48832.expandToInclude(textBox__48941.toRect());
                    currentDirection__47302 = textBox__48941.direction;
                }
                rect__48832 = global::Doroti.Ui.Rect.fromLTWH(Math.Max(0.0, rect__48832.left), Math.Max(0.0, rect__48832.top), Math.Min(rect__48832.width, ((BoxConstraints)constraints).maxWidth), Math.Min(rect__48832.height, ((BoxConstraints)constraints).maxHeight));
                currentRect__47345 = global::Doroti.Ui.Rect.fromLTRB((rect__48832.left.floorToDouble() - 4.0), (rect__48832.top.floorToDouble() - 4.0), (rect__48832.right.ceilToDouble() + 4.0), (rect__48832.bottom.ceilToDouble() + 4.0));
                var configuration__49834 = ((Func<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration>)(() =>
{
    var __cascade = new global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration();
    __cascade.sortKey = new global::Doroti.Generated.Framework.Semantics.OrdinalSortKey(ordinal__47366++);
    __cascade.textDirection = initialDirection__48651;
    __cascade.identifier = (((global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation)info__47662).semanticsIdentifier ?? "");
    __cascade.attributedLabel = new global::Doroti.Generated.Framework.Semantics.AttributedString((((global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation)info__47662).semanticsLabel ?? ((global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation)info__47662).text), attributes: ((global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation)info__47662).stringAttributes);
    return __cascade;
}))();
                switch (((global::Doroti.Generated.Framework.Painting.InlineSpanSemanticsInformation)info__47662).recognizer)
                {
                    case TapGestureRecognizer { onTap: Action handler__50276 } __object50228:
                        {
                            if ((handler__50276 is not null))
                            {
                                configuration__49834.onTap = handler__50276;
                                configuration__49834.isLink = true;
                            }
                            break;
                        }
                    case DoubleTapGestureRecognizer { onDoubleTap: Action handler__50361 } __object50301:
                        {
                            if ((handler__50361 is not null))
                            {
                                configuration__49834.onTap = handler__50361;
                                configuration__49834.isLink = true;
                            }
                            break;
                        }
                    case LongPressGestureRecognizer { onLongPress: Action onLongPress__50595 } __object50523:
                        {
                            if ((onLongPress__50595 is not null))
                            {
                                configuration__49834.onLongPress = onLongPress__50595;
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
                if ((((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).parentPaintClipRect is not null))
                {
                    global::Doroti.Ui.Rect paintRect__50934 = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).parentPaintClipRect).intersect(currentRect__47345);
                    configuration__49834.isHidden = (paintRect__50934.isEmpty && !currentRect__47345.isEmpty);
                }
                global::Doroti.Generated.Framework.Semantics.SemanticsNode newChild__51112 = default!;
                if (((((long?)(this._cachedChildNodes?.Count)) is { } __count51134 ? __count51134 != 0 : (bool?)null) ?? false))
                {
                    newChild__51112 = this._cachedChildNodes!.remove(this._cachedChildNodes!.Keys.First())!;
                }
                else
                {
                    var key__51289 = new UniqueKey();
                    newChild__51112 = new global::Doroti.Generated.Framework.Semantics.SemanticsNode(key: key__51289, showOnScreen: _createShowOnScreenFor(key__51289));
                }
                ((Func<global::Doroti.Generated.Framework.Semantics.SemanticsNode>)(() =>
{
    var __cascade = newChild__51112;
    __cascade.updateWith(config: configuration__49834);
    __cascade.rect = currentRect__47345;
    return __cascade;
}))();
                newChildCache__47499[((global::Doroti.Generated.Framework.Semantics.SemanticsNode)newChild__51112).key!] = newChild__51112;
                newChildren__47251.Add(newChild__51112);
            }
        }
        DartRuntimePrimitives.Assert(() => (childIndex__47438 == children.Count()));
        DartRuntimePrimitives.Assert(() => (child__47469 is null));
        _cachedChildNodes = newChildCache__47499.cast<Key, global::Doroti.Generated.Framework.Semantics.SemanticsNode>();
        node.updateWith(config: config, childrenInInversePaintOrder: newChildren__47251);
    }

    internal virtual Action? _createShowOnScreenFor(Key key)
    {
        return (() =>
        {
            global::Doroti.Generated.Framework.Semantics.SemanticsNode node__51948 = this._cachedChildNodes!.GetValueOrDefault(key)!;
            showOnScreen(descendant: this, rect: ((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node__51948).rect);
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
        properties.add(new EnumProperty<global::Doroti.Generated.Framework.Painting.TextOverflow>("overflow", this.overflow));
        properties.add(new DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextScaler>("textScaler", this.textScaler, defaultValue: global::Doroti.Generated.Framework.Painting.TextScaler.noScaling));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Locale>("locale", this.locale, defaultValue: null));
        properties.add(new IntProperty("maxLines", this.maxLines, ifNull: "unlimited"));
        properties.add(new DoubleProperty("devicePixelRatio", this.devicePixelRatio, defaultValue: 1.0));
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
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child__182065 = this._firstChild;
        while ((child__182065 is not null))
        {
            child__182065.detach();
            var childParentData__182148 = ((TextParentData?)(object?)child__182065.parentData!)!;
            child__182065 = childParentData__182148.nextSibling;
        }
    }

    public override void redepthChildren()
    {
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
        RenderBox? child__182587 = this._firstChild;
        while ((child__182587 is not null))
        {
            visitor(child__182587);
            var childParentData__182670 = ((TextParentData?)(object?)child__182587.parentData!)!;
            child__182587 = childParentData__182670.nextSibling;
        }
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

    public virtual List<global::Doroti.Generated.Framework.Painting.PlaceholderDimensions> layoutInlineChildren(double maxWidth, Func<RenderBox, BoxConstraints, Size> layoutChild, Func<RenderBox, BoxConstraints, TextBaseline, double?> getChildBaseline)
    {
        var constraints__7015 = new BoxConstraints(maxWidth: maxWidth);
        return new List<global::Doroti.Generated.Framework.Painting.PlaceholderDimensions>();
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
    internal static string _placeholderCharacter = char.ConvertFromUtf32(checked((int)global::Doroti.Generated.Framework.Painting.PlaceholderSpan.placeholderCodeUnit));
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
        SelectionGeometry newValue__54577 = _getSelectionGeometry();
        if ((object.Equals(this._selectionGeometry, newValue__54577)))
        {
            return;
        }
        _selectionGeometry = newValue__54577;
        notifyListeners();
    }

    internal virtual SelectionGeometry _getSelectionGeometry()
    {
        if (((this._textSelectionStart is null) || (this._textSelectionEnd is null)))
        {
            return new SelectionGeometry(status: SelectionStatus.none, hasContent: true);
        }
        long selectionStart__54960 = this._textSelectionStart!.offset;
        long selectionEnd__55020 = this._textSelectionEnd!.offset;
        bool isReversed__55077 = (selectionStart__54960 > selectionEnd__55020);
        global::Doroti.Ui.Offset startOffsetInParagraphCoordinates__55138 = this.paragraph._getOffsetForPosition(this._textSelectionStart!);
        global::Doroti.Ui.Offset endOffsetInParagraphCoordinates__55259 = ((selectionStart__54960 == selectionEnd__55020) ? startOffsetInParagraphCoordinates__55138 : this.paragraph._getOffsetForPosition(this._textSelectionEnd!));
        var flipHandles__55441 = (isReversed__55077 != ((object.Equals(TextDirection.rtl, ((RenderParagraph)this.paragraph).textDirection))));
        var selection__55527 = new TextSelection(baseOffset: selectionStart__54960, extentOffset: selectionEnd__55020);
        var selectionRects__55620 = new List<global::Doroti.Ui.Rect>();
        foreach (global::Doroti.Ui.TextBox textBox__55670 in this.paragraph.getBoxesForSelection(selection__55527))
        {
            selectionRects__55620.Add(textBox__55670.toRect());
        }
        var selectionCollapsed__55786 = (selectionStart__54960 == selectionEnd__55020);
        var (startSelectionHandleType__55881, endSelectionHandleType__55937) = ((selectionCollapsed__55786, flipHandles__55441) switch { (true, _) => (((TextSelectionHandleType, TextSelectionHandleType))((TextSelectionHandleType.collapsed, TextSelectionHandleType.collapsed))), (false, true) => (((TextSelectionHandleType, TextSelectionHandleType))((TextSelectionHandleType.right, TextSelectionHandleType.left))), (false, false) => (((TextSelectionHandleType, TextSelectionHandleType))((TextSelectionHandleType.left, TextSelectionHandleType.right))) });
        return new SelectionGeometry(startSelectionPoint: new SelectionPoint(localPosition: startOffsetInParagraphCoordinates__55138, lineHeight: ((RenderParagraph)this.paragraph)._textPainter.preferredLineHeight, handleType: startSelectionHandleType__55881), endSelectionPoint: new SelectionPoint(localPosition: endOffsetInParagraphCoordinates__55259, lineHeight: ((RenderParagraph)this.paragraph)._textPainter.preferredLineHeight, handleType: endSelectionHandleType__55937), selectionRects: selectionRects__55620, status: (selectionCollapsed__55786 ? SelectionStatus.collapsed : SelectionStatus.uncollapsed), hasContent: true);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SelectionResult dispatchSelectionEvent(SelectionEvent @event)
    {
        SelectionResult result__57092 = default!;
        global::Doroti.Ui.TextPosition? existingSelectionStart__57124 = this._textSelectionStart;
        global::Doroti.Ui.TextPosition? existingSelectionEnd__57194 = this._textSelectionEnd;
        switch (((SelectionEvent)@event).type)
        {
            case SelectionEventType.startEdgeUpdate:
            case SelectionEventType.endEdgeUpdate:
                {
                    var edgeUpdate__57368 = ((SelectionEdgeUpdateEvent?)(object?)@event)!;
                    TextGranularity granularity__57446 = ((SelectionEdgeUpdateEvent)((SelectionEdgeUpdateEvent)@event)).granularity;
                    switch (granularity__57446)
                    {
                        case TextGranularity.character:
                            {
                                result__57092 = _updateSelectionEdge(((SelectionEdgeUpdateEvent)edgeUpdate__57368).globalPosition, isEnd: (object.Equals(edgeUpdate__57368.type, SelectionEventType.endEdgeUpdate)));
                                break;
                            }
                        case TextGranularity.word:
                            {
                                result__57092 = _updateSelectionEdgeByTextBoundary(((SelectionEdgeUpdateEvent)edgeUpdate__57368).globalPosition, isEnd: (object.Equals(edgeUpdate__57368.type, SelectionEventType.endEdgeUpdate)), getTextBoundary: (Func<TextPosition, (TextPosition boundaryEnd, TextPosition boundaryStart)>)this._getWordBoundaryAtPosition);
                                break;
                            }
                        case TextGranularity.paragraph:
                            {
                                result__57092 = _updateSelectionEdgeByMultiSelectableTextBoundary(((SelectionEdgeUpdateEvent)edgeUpdate__57368).globalPosition, isEnd: (object.Equals(edgeUpdate__57368.type, SelectionEventType.endEdgeUpdate)), getTextBoundary: (Func<TextPosition, string, (TextPosition boundaryEnd, TextPosition boundaryStart)>)this._getParagraphBoundaryAtPosition, getClampedTextBoundary: (Func<TextPosition, (TextPosition boundaryEnd, TextPosition boundaryStart)>)this._getClampedParagraphBoundaryAtPosition);
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
                    result__57092 = _handleClearSelection();
                    break;
                }
            case SelectionEventType.selectAll:
                {
                    result__57092 = _handleSelectAll();
                    break;
                }
            case SelectionEventType.selectWord:
                {
                    var selectWord__58790 = ((SelectWordSelectionEvent?)(object?)@event)!;
                    result__57092 = _handleSelectWord(((SelectWordSelectionEvent)selectWord__58790).globalPosition);
                    break;
                }
            case SelectionEventType.selectParagraph:
                {
                    var selectParagraph__58962 = ((SelectParagraphSelectionEvent?)(object?)@event)!;
                    if (((SelectParagraphSelectionEvent)selectParagraph__58962).absorb)
                    {
                        _handleSelectAll();
                        result__57092 = SelectionResult.next;
                        _selectableContainsOriginTextBoundary = true;
                    }
                    else
                    {
                        result__57092 = _handleSelectParagraph(((SelectParagraphSelectionEvent)selectParagraph__58962).globalPosition);
                    }
                    break;
                }
            case SelectionEventType.granularlyExtendSelection:
                {
                    var granularlyExtendSelection__59358 = ((GranularlyExtendSelectionEvent?)(object?)@event)!;
                    result__57092 = _handleGranularlyExtendSelection(((GranularlyExtendSelectionEvent)granularlyExtendSelection__59358).forward, ((GranularlyExtendSelectionEvent)granularlyExtendSelection__59358).isEnd, ((GranularlyExtendSelectionEvent)granularlyExtendSelection__59358).granularity);
                    break;
                }
            case SelectionEventType.directionallyExtendSelection:
                {
                    var directionallyExtendSelection__59700 = ((DirectionallyExtendSelectionEvent?)(object?)@event)!;
                    result__57092 = _handleDirectionallyExtendSelection(((DirectionallyExtendSelectionEvent)directionallyExtendSelection__59700).dx, ((DirectionallyExtendSelectionEvent)directionallyExtendSelection__59700).isEnd, ((DirectionallyExtendSelectionEvent)directionallyExtendSelection__59700).direction);
                    break;
                }
        }
        if (((!object.Equals(existingSelectionStart__57124, this._textSelectionStart)) || (!object.Equals(existingSelectionEnd__57194, this._textSelectionEnd))))
        {
            _didChangeSelection();
        }
        return result__57092;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SelectedContent? getSelectedContent()
    {
        if (((this._textSelectionStart is null) || (this._textSelectionEnd is null)))
        {
            return null;
        }
        long start__60316 = Math.Min(this._textSelectionStart!.offset, this._textSelectionEnd!.offset);
        long end__60404 = Math.Max(this._textSelectionStart!.offset, this._textSelectionEnd!.offset);
        return new SelectedContent(plainText: this.fullText.substring(start__60316, end__60404));
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
        global::Doroti.Ui.TextPosition? targetPosition__61208 = default!;
        if ((textBoundary is not null))
        {
            (TextPosition boundaryEnd, TextPosition boundaryStart) textBoundary__value61232 = DartRuntimePrimitives.RequireValue(textBoundary);
            DartRuntimePrimitives.Assert(() => ((DartRuntimePrimitives.RequireValue(textBoundary__value61232).boundaryStart.offset >= this.range.start) && (DartRuntimePrimitives.RequireValue(textBoundary__value61232).boundaryEnd.offset <= this.range.end)));
            if (((this._selectableContainsOriginTextBoundary && (existingSelectionStart is not null)) && (existingSelectionEnd is not null)))
            {
                var isSamePosition__61548 = (position.offset == existingSelectionEnd.offset);
                bool isSelectionInverted__61632 = (existingSelectionStart.offset > existingSelectionEnd.offset);
                bool shouldSwapEdges__61746 = (!isSamePosition__61548 && ((isSelectionInverted__61632 != ((position.offset > existingSelectionEnd.offset)))));
                if (shouldSwapEdges__61746)
                {
                    if ((position.offset < existingSelectionEnd.offset))
                    {
                        targetPosition__61208 = DartRuntimePrimitives.RequireValue(textBoundary__value61232).boundaryStart;
                    }
                    else
                    {
                        targetPosition__61208 = DartRuntimePrimitives.RequireValue(textBoundary__value61232).boundaryEnd;
                    }
                    (TextPosition boundaryEnd, TextPosition boundaryStart) localTextBoundary__62388 = getTextBoundary(existingSelectionEnd);
                    DartRuntimePrimitives.Assert(() => ((localTextBoundary__62388.boundaryStart.offset >= this.range.start) && (localTextBoundary__62388.boundaryEnd.offset <= this.range.end)));
                    _setSelectionPosition(((existingSelectionEnd.offset == localTextBoundary__62388.boundaryStart.offset) ? localTextBoundary__62388.boundaryEnd : localTextBoundary__62388.boundaryStart), isEnd: true);
                }
                else
                {
                    if ((position.offset < existingSelectionEnd.offset))
                    {
                        targetPosition__61208 = DartRuntimePrimitives.RequireValue(textBoundary__value61232).boundaryStart;
                    }
                    else
                    {
                        if ((position.offset > existingSelectionEnd.offset))
                        {
                            targetPosition__61208 = DartRuntimePrimitives.RequireValue(textBoundary__value61232).boundaryEnd;
                        }
                        else
                        {
                            targetPosition__61208 = existingSelectionStart;
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
                        targetPosition__61208 = DartRuntimePrimitives.RequireValue(textBoundary__value61232).boundaryStart;
                    }
                    else
                    {
                        targetPosition__61208 = DartRuntimePrimitives.RequireValue(textBoundary__value61232).boundaryEnd;
                    }
                }
                else
                {
                    targetPosition__61208 = _closestTextBoundary(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(textBoundary__value61232)), position);
                }
            }
        }
        else
        {
            if (((this._selectableContainsOriginTextBoundary && (existingSelectionStart is not null)) && (existingSelectionEnd is not null)))
            {
                var isSamePosition__64565 = (position.offset == existingSelectionEnd.offset);
                bool isSelectionInverted__64649 = (existingSelectionStart.offset > existingSelectionEnd.offset);
                bool shouldSwapEdges__64763 = (!isSamePosition__64565 && ((isSelectionInverted__64649 != ((position.offset > existingSelectionEnd.offset)))));
                if (shouldSwapEdges__64763)
                {
                    (TextPosition boundaryEnd, TextPosition boundaryStart) localTextBoundary__64966 = getTextBoundary(existingSelectionEnd);
                    DartRuntimePrimitives.Assert(() => ((localTextBoundary__64966.boundaryStart.offset >= this.range.start) && (localTextBoundary__64966.boundaryEnd.offset <= this.range.end)));
                    _setSelectionPosition((isSelectionInverted__64649 ? localTextBoundary__64966.boundaryEnd : localTextBoundary__64966.boundaryStart), isEnd: true);
                }
            }
        }
        return (targetPosition__61208 ?? position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _updateSelectionEndEdgeByTextBoundary((TextPosition boundaryEnd, TextPosition boundaryStart)? textBoundary, Func<TextPosition, (TextPosition boundaryEnd, TextPosition boundaryStart)> getTextBoundary, TextPosition position, TextPosition? existingSelectionStart, TextPosition? existingSelectionEnd)
    {
        global::Doroti.Ui.TextPosition? targetPosition__65701 = default!;
        if ((textBoundary is not null))
        {
            (TextPosition boundaryEnd, TextPosition boundaryStart) textBoundary__value65725 = DartRuntimePrimitives.RequireValue(textBoundary);
            DartRuntimePrimitives.Assert(() => ((DartRuntimePrimitives.RequireValue(textBoundary__value65725).boundaryStart.offset >= this.range.start) && (DartRuntimePrimitives.RequireValue(textBoundary__value65725).boundaryEnd.offset <= this.range.end)));
            if (((this._selectableContainsOriginTextBoundary && (existingSelectionStart is not null)) && (existingSelectionEnd is not null)))
            {
                var isSamePosition__66041 = (position.offset == existingSelectionStart.offset);
                bool isSelectionInverted__66127 = (existingSelectionStart.offset > existingSelectionEnd.offset);
                bool shouldSwapEdges__66241 = (!isSamePosition__66041 && ((isSelectionInverted__66127 != ((position.offset < existingSelectionStart.offset)))));
                if (shouldSwapEdges__66241)
                {
                    if ((position.offset < existingSelectionStart.offset))
                    {
                        targetPosition__65701 = DartRuntimePrimitives.RequireValue(textBoundary__value65725).boundaryStart;
                    }
                    else
                    {
                        targetPosition__65701 = DartRuntimePrimitives.RequireValue(textBoundary__value65725).boundaryEnd;
                    }
                    (TextPosition boundaryEnd, TextPosition boundaryStart) localTextBoundary__66887 = getTextBoundary(existingSelectionStart);
                    DartRuntimePrimitives.Assert(() => ((localTextBoundary__66887.boundaryStart.offset >= this.range.start) && (localTextBoundary__66887.boundaryEnd.offset <= this.range.end)));
                    _setSelectionPosition(((existingSelectionStart.offset == localTextBoundary__66887.boundaryStart.offset) ? localTextBoundary__66887.boundaryEnd : localTextBoundary__66887.boundaryStart), isEnd: false);
                }
                else
                {
                    if ((position.offset < existingSelectionStart.offset))
                    {
                        targetPosition__65701 = DartRuntimePrimitives.RequireValue(textBoundary__value65725).boundaryStart;
                    }
                    else
                    {
                        if ((position.offset > existingSelectionStart.offset))
                        {
                            targetPosition__65701 = DartRuntimePrimitives.RequireValue(textBoundary__value65725).boundaryEnd;
                        }
                        else
                        {
                            targetPosition__65701 = existingSelectionEnd;
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
                        targetPosition__65701 = DartRuntimePrimitives.RequireValue(textBoundary__value65725).boundaryStart;
                    }
                    else
                    {
                        targetPosition__65701 = DartRuntimePrimitives.RequireValue(textBoundary__value65725).boundaryEnd;
                    }
                }
                else
                {
                    targetPosition__65701 = _closestTextBoundary(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(textBoundary__value65725)), position);
                }
            }
        }
        else
        {
            if (((this._selectableContainsOriginTextBoundary && (existingSelectionStart is not null)) && (existingSelectionEnd is not null)))
            {
                var isSamePosition__69071 = (position.offset == existingSelectionStart.offset);
                bool isSelectionInverted__69157 = (existingSelectionStart.offset > existingSelectionEnd.offset);
                bool shouldSwapEdges__69271 = ((isSelectionInverted__69157 != ((position.offset < existingSelectionStart.offset))) || isSamePosition__69071);
                if (shouldSwapEdges__69271)
                {
                    (TextPosition boundaryEnd, TextPosition boundaryStart) localTextBoundary__69472 = getTextBoundary(existingSelectionStart);
                    DartRuntimePrimitives.Assert(() => ((localTextBoundary__69472.boundaryStart.offset >= this.range.start) && (localTextBoundary__69472.boundaryEnd.offset <= this.range.end)));
                    _setSelectionPosition((isSelectionInverted__69157 ? localTextBoundary__69472.boundaryStart : localTextBoundary__69472.boundaryEnd), isEnd: false);
                }
            }
        }
        return (targetPosition__65701 ?? position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _updateSelectionEdgeByTextBoundary(Offset globalPosition, bool isEnd, Func<TextPosition, (TextPosition boundaryEnd, TextPosition boundaryStart)> getTextBoundary)
    {
        global::Doroti.Ui.TextPosition? existingSelectionStart__70398 = this._textSelectionStart;
        global::Doroti.Ui.TextPosition? existingSelectionEnd__70468 = this._textSelectionEnd;
        _setSelectionPosition(null, isEnd: isEnd);
        Matrix4 transform__70576 = this.paragraph.getTransformTo(null);
        transform__70576.invert();
        global::Doroti.Ui.Offset localPosition__70661 = MatrixUtils.transformPoint(transform__70576, globalPosition);
        if (this._rect.isEmpty)
        {
            SelectionResult result__70785 = SelectionUtils.getResultBasedOnRect(this._rect, localPosition__70661);
            _setSelectionPosition(((object.Equals(result__70785, SelectionResult.next)) ? new global::Doroti.Ui.TextPosition(offset: this.range.end) : new global::Doroti.Ui.TextPosition(offset: this.range.start, affinity: TextAffinity.upstream)), isEnd: isEnd);
            return result__70785;
        }
        global::Doroti.Ui.Offset adjustedOffset__71124 = SelectionUtils.adjustDragOffset(this._rect, localPosition__70661, direction: ((RenderParagraph)this.paragraph).textDirection);
        global::Doroti.Ui.TextPosition position__71281 = this.paragraph.getPositionForOffset(adjustedOffset__71124);
        (TextPosition boundaryEnd, TextPosition boundaryStart)? textBoundary__71646 = (this._rect.contains(localPosition__70661) ? getTextBoundary(position__71281) : null);
        if (((textBoundary__71646 is not null) && ((((DartRuntimePrimitives.RequireValue(textBoundary__71646).boundaryStart.offset < this.range.start) && (DartRuntimePrimitives.RequireValue(textBoundary__71646).boundaryEnd.offset <= this.range.start)) || ((DartRuntimePrimitives.RequireValue(textBoundary__71646).boundaryStart.offset >= this.range.end) && (DartRuntimePrimitives.RequireValue(textBoundary__71646).boundaryEnd.offset > this.range.end))))))
        {
            (TextPosition boundaryEnd, TextPosition boundaryStart) textBoundary__71646__value71751 = DartRuntimePrimitives.RequireValue(textBoundary__71646);
            textBoundary__71646 = null;
        }
        global::Doroti.Ui.TextPosition targetPosition__72403 = _clampTextPosition((isEnd ? _updateSelectionEndEdgeByTextBoundary(textBoundary__71646, (Func<TextPosition, (TextPosition boundaryEnd, TextPosition boundaryStart)>)getTextBoundary, position__71281, existingSelectionStart__70398, existingSelectionEnd__70468) : _updateSelectionStartEdgeByTextBoundary(textBoundary__71646, (Func<TextPosition, (TextPosition boundaryEnd, TextPosition boundaryStart)>)getTextBoundary, position__71281, existingSelectionStart__70398, existingSelectionEnd__70468)));
        _setSelectionPosition(targetPosition__72403, isEnd: isEnd);
        if ((targetPosition__72403.offset == this.range.end))
        {
            return SelectionResult.next;
        }
        if ((targetPosition__72403.offset == this.range.start))
        {
            return SelectionResult.previous;
        }
        return SelectionUtils.getResultBasedOnRect(this._rect, localPosition__70661);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _updateSelectionEdge(Offset globalPosition, bool isEnd)
    {
        _setSelectionPosition(null, isEnd: isEnd);
        Matrix4 transform__73640 = this.paragraph.getTransformTo(null);
        transform__73640.invert();
        global::Doroti.Ui.Offset localPosition__73725 = MatrixUtils.transformPoint(transform__73640, globalPosition);
        if (this._rect.isEmpty)
        {
            SelectionResult result__73849 = SelectionUtils.getResultBasedOnRect(this._rect, localPosition__73725);
            _setSelectionPosition(((object.Equals(result__73849, SelectionResult.next)) ? new global::Doroti.Ui.TextPosition(offset: this.range.end) : new global::Doroti.Ui.TextPosition(offset: this.range.start, affinity: TextAffinity.upstream)), isEnd: isEnd);
            return result__73849;
        }
        global::Doroti.Ui.Offset adjustedOffset__74188 = SelectionUtils.adjustDragOffset(this._rect, localPosition__73725, direction: ((RenderParagraph)this.paragraph).textDirection);
        global::Doroti.Ui.TextPosition position__74345 = _clampTextPosition(this.paragraph.getPositionForOffset(adjustedOffset__74188));
        _setSelectionPosition(position__74345, isEnd: isEnd);
        if ((position__74345.offset == this.range.end))
        {
            return SelectionResult.next;
        }
        if ((position__74345.offset == this.range.start))
        {
            return SelectionResult.previous;
        }
        return SelectionUtils.getResultBasedOnRect(this._rect, localPosition__73725);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult? _updateSelectionStartEdgeByMultiSelectableTextBoundary(Func<TextPosition, string, (TextPosition boundaryEnd, TextPosition boundaryStart)> getTextBoundary, bool paragraphContainsPosition, TextPosition position, TextPosition? existingSelectionStart, TextPosition? existingSelectionEnd)
    {
        var isEnd__75897 = false;
        if (((this._selectableContainsOriginTextBoundary && (existingSelectionStart is not null)) && (existingSelectionEnd is not null)))
        {
            bool forwardSelection__76160 = (existingSelectionEnd.offset >= existingSelectionStart.offset);
            if (paragraphContainsPosition)
            {
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPosition__76445 = getTextBoundary(position, this.fullText);
                (TextPosition boundaryEnd, TextPosition boundaryStart) originTextBoundary__76901 = getTextBoundary((forwardSelection__76160 ? new global::Doroti.Ui.TextPosition(offset: (existingSelectionEnd.offset - 1L), affinity: existingSelectionEnd.affinity) : existingSelectionEnd), this.fullText);
                global::Doroti.Ui.TextPosition targetPosition__77228 = default!;
                long pivotOffset__77262 = (forwardSelection__76160 ? originTextBoundary__76901.boundaryEnd.offset : originTextBoundary__76901.boundaryStart.offset);
                var shouldSwapEdges__77414 = (!forwardSelection__76160 != ((position.offset > pivotOffset__77262)));
                if ((position.offset < pivotOffset__77262))
                {
                    targetPosition__77228 = boundaryAtPosition__76445.boundaryStart;
                }
                else
                {
                    if ((position.offset > pivotOffset__77262))
                    {
                        targetPosition__77228 = boundaryAtPosition__76445.boundaryEnd;
                    }
                    else
                    {
                        targetPosition__77228 = (forwardSelection__76160 ? existingSelectionStart : existingSelectionEnd);
                    }
                }
                if (shouldSwapEdges__77414)
                {
                    _setSelectionPosition(_clampTextPosition((forwardSelection__76160 ? originTextBoundary__76901.boundaryStart : originTextBoundary__76901.boundaryEnd)), isEnd: true);
                }
                _setSelectionPosition(_clampTextPosition(targetPosition__77228), isEnd: isEnd__75897);
                bool finalSelectionIsForward__78272 = (this._textSelectionEnd!.offset >= this._textSelectionStart!.offset);
                if (((boundaryAtPosition__76445.boundaryStart.offset > this.range.end) && (boundaryAtPosition__76445.boundaryEnd.offset > this.range.end)))
                {
                    return SelectionResult.next;
                }
                if (((boundaryAtPosition__76445.boundaryStart.offset < this.range.start) && (boundaryAtPosition__76445.boundaryEnd.offset < this.range.start)))
                {
                    return SelectionResult.previous;
                }
                if (finalSelectionIsForward__78272)
                {
                    if ((boundaryAtPosition__76445.boundaryStart.offset >= originTextBoundary__76901.boundaryStart.offset))
                    {
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPosition__76445.boundaryStart.offset < originTextBoundary__76901.boundaryStart.offset))
                    {
                        return SelectionResult.previous;
                    }
                }
                else
                {
                    if ((boundaryAtPosition__76445.boundaryEnd.offset <= originTextBoundary__76901.boundaryEnd.offset))
                    {
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPosition__76445.boundaryEnd.offset > originTextBoundary__76901.boundaryEnd.offset))
                    {
                        return SelectionResult.next;
                    }
                }
            }
            else
            {
                global::Doroti.Ui.TextPosition clampedPosition__79592 = _clampTextPosition(position);
                (TextPosition boundaryEnd, TextPosition boundaryStart) originTextBoundary__80038 = getTextBoundary((forwardSelection__76160 ? new global::Doroti.Ui.TextPosition(offset: (existingSelectionEnd.offset - 1L), affinity: existingSelectionEnd.affinity) : existingSelectionEnd), this.fullText);
                if ((forwardSelection__76160 && (clampedPosition__79592.offset == this.range.start)))
                {
                    _setSelectionPosition(clampedPosition__79592, isEnd: isEnd__75897);
                    return SelectionResult.previous;
                }
                if ((!forwardSelection__76160 && (clampedPosition__79592.offset == this.range.end)))
                {
                    _setSelectionPosition(clampedPosition__79592, isEnd: isEnd__75897);
                    return SelectionResult.next;
                }
                if ((forwardSelection__76160 && (clampedPosition__79592.offset == this.range.end)))
                {
                    _setSelectionPosition(_clampTextPosition(originTextBoundary__80038.boundaryStart), isEnd: true);
                    _setSelectionPosition(clampedPosition__79592, isEnd: isEnd__75897);
                    return SelectionResult.next;
                }
                if ((!forwardSelection__76160 && (clampedPosition__79592.offset == this.range.start)))
                {
                    _setSelectionPosition(_clampTextPosition(originTextBoundary__80038.boundaryEnd), isEnd: true);
                    _setSelectionPosition(clampedPosition__79592, isEnd: isEnd__75897);
                    return SelectionResult.previous;
                }
            }
        }
        else
        {
            var positionOnPlaceholder__81611 = (this.paragraph.getWordBoundary(position).textInside(this.fullText) == _placeholderCharacter);
            if ((!paragraphContainsPosition || positionOnPlaceholder__81611))
            {
                return null;
            }
            if ((existingSelectionEnd is not null))
            {
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPosition__81898 = getTextBoundary(position, this.fullText);
                bool backwardSelection__81975 = ((((existingSelectionStart is null) && (existingSelectionEnd.offset == this.range.start)) || ((object.Equals(existingSelectionStart, existingSelectionEnd)) && (existingSelectionEnd.offset == this.range.start))) || ((existingSelectionStart is not null) && (existingSelectionStart.offset > existingSelectionEnd.offset)));
                if (((boundaryAtPosition__81898.boundaryStart.offset < this.range.start) && (boundaryAtPosition__81898.boundaryEnd.offset < this.range.start)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEnd__75897);
                    return SelectionResult.previous;
                }
                if (((boundaryAtPosition__81898.boundaryStart.offset > this.range.end) && (boundaryAtPosition__81898.boundaryEnd.offset > this.range.end)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEnd__75897);
                    return SelectionResult.next;
                }
                if (backwardSelection__81975)
                {
                    if ((boundaryAtPosition__81898.boundaryEnd.offset <= this.range.end))
                    {
                        _setSelectionPosition(_clampTextPosition(boundaryAtPosition__81898.boundaryEnd), isEnd: isEnd__75897);
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPosition__81898.boundaryEnd.offset > this.range.end))
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEnd__75897);
                        return SelectionResult.next;
                    }
                }
                else
                {
                    _setSelectionPosition(_clampTextPosition(boundaryAtPosition__81898.boundaryStart), isEnd: isEnd__75897);
                    if ((boundaryAtPosition__81898.boundaryStart.offset < this.range.start))
                    {
                        return SelectionResult.previous;
                    }
                    if ((boundaryAtPosition__81898.boundaryStart.offset >= this.range.start))
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
        var isEnd__84632 = true;
        if (((this._selectableContainsOriginTextBoundary && (existingSelectionStart is not null)) && (existingSelectionEnd is not null)))
        {
            bool forwardSelection__84894 = (existingSelectionEnd.offset >= existingSelectionStart.offset);
            if (paragraphContainsPosition)
            {
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPosition__85179 = getTextBoundary(position, this.fullText);
                (TextPosition boundaryEnd, TextPosition boundaryStart) originTextBoundary__85642 = getTextBoundary((forwardSelection__84894 ? existingSelectionStart : new global::Doroti.Ui.TextPosition(offset: (existingSelectionStart.offset - 1L), affinity: existingSelectionStart.affinity)), this.fullText);
                global::Doroti.Ui.TextPosition targetPosition__85975 = default!;
                long pivotOffset__86009 = (forwardSelection__84894 ? originTextBoundary__85642.boundaryStart.offset : originTextBoundary__85642.boundaryEnd.offset);
                var shouldSwapEdges__86161 = (!forwardSelection__84894 != ((position.offset < pivotOffset__86009)));
                if ((position.offset < pivotOffset__86009))
                {
                    targetPosition__85975 = boundaryAtPosition__85179.boundaryStart;
                }
                else
                {
                    if ((position.offset > pivotOffset__86009))
                    {
                        targetPosition__85975 = boundaryAtPosition__85179.boundaryEnd;
                    }
                    else
                    {
                        targetPosition__85975 = (forwardSelection__84894 ? existingSelectionEnd : existingSelectionStart);
                    }
                }
                if (shouldSwapEdges__86161)
                {
                    _setSelectionPosition(_clampTextPosition((forwardSelection__84894 ? originTextBoundary__85642.boundaryEnd : originTextBoundary__85642.boundaryStart)), isEnd: false);
                }
                _setSelectionPosition(_clampTextPosition(targetPosition__85975), isEnd: isEnd__84632);
                bool finalSelectionIsForward__87020 = (this._textSelectionEnd!.offset >= this._textSelectionStart!.offset);
                if (((boundaryAtPosition__85179.boundaryStart.offset > this.range.end) && (boundaryAtPosition__85179.boundaryEnd.offset > this.range.end)))
                {
                    return SelectionResult.next;
                }
                if (((boundaryAtPosition__85179.boundaryStart.offset < this.range.start) && (boundaryAtPosition__85179.boundaryEnd.offset < this.range.start)))
                {
                    return SelectionResult.previous;
                }
                if (finalSelectionIsForward__87020)
                {
                    if ((boundaryAtPosition__85179.boundaryEnd.offset <= originTextBoundary__85642.boundaryEnd.offset))
                    {
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPosition__85179.boundaryEnd.offset > originTextBoundary__85642.boundaryEnd.offset))
                    {
                        return SelectionResult.next;
                    }
                }
                else
                {
                    if ((boundaryAtPosition__85179.boundaryStart.offset >= originTextBoundary__85642.boundaryStart.offset))
                    {
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPosition__85179.boundaryStart.offset < originTextBoundary__85642.boundaryStart.offset))
                    {
                        return SelectionResult.previous;
                    }
                }
            }
            else
            {
                global::Doroti.Ui.TextPosition clampedPosition__88340 = _clampTextPosition(position);
                (TextPosition boundaryEnd, TextPosition boundaryStart) originTextBoundary__88793 = getTextBoundary((forwardSelection__84894 ? existingSelectionStart : new global::Doroti.Ui.TextPosition(offset: (existingSelectionStart.offset - 1L), affinity: existingSelectionStart.affinity)), this.fullText);
                if ((forwardSelection__84894 && (clampedPosition__88340.offset == this.range.start)))
                {
                    _setSelectionPosition(_clampTextPosition(originTextBoundary__88793.boundaryEnd), isEnd: false);
                    _setSelectionPosition(clampedPosition__88340, isEnd: isEnd__84632);
                    return SelectionResult.previous;
                }
                if ((!forwardSelection__84894 && (clampedPosition__88340.offset == this.range.end)))
                {
                    _setSelectionPosition(_clampTextPosition(originTextBoundary__88793.boundaryStart), isEnd: false);
                    _setSelectionPosition(clampedPosition__88340, isEnd: isEnd__84632);
                    return SelectionResult.next;
                }
                if ((forwardSelection__84894 && (clampedPosition__88340.offset == this.range.end)))
                {
                    _setSelectionPosition(clampedPosition__88340, isEnd: isEnd__84632);
                    return SelectionResult.next;
                }
                if ((!forwardSelection__84894 && (clampedPosition__88340.offset == this.range.start)))
                {
                    _setSelectionPosition(clampedPosition__88340, isEnd: isEnd__84632);
                    return SelectionResult.previous;
                }
            }
        }
        else
        {
            var positionOnPlaceholder__90374 = (this.paragraph.getWordBoundary(position).textInside(this.fullText) == _placeholderCharacter);
            if ((!paragraphContainsPosition || positionOnPlaceholder__90374))
            {
                return null;
            }
            if ((existingSelectionStart is not null))
            {
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPosition__90663 = getTextBoundary(position, this.fullText);
                bool backwardSelection__90740 = ((((existingSelectionEnd is null) && (existingSelectionStart.offset == this.range.end)) || ((object.Equals(existingSelectionStart, existingSelectionEnd)) && (existingSelectionStart.offset == this.range.end))) || ((existingSelectionEnd is not null) && (existingSelectionStart.offset > existingSelectionEnd.offset)));
                if (((boundaryAtPosition__90663.boundaryStart.offset < this.range.start) && (boundaryAtPosition__90663.boundaryEnd.offset < this.range.start)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEnd__84632);
                    return SelectionResult.previous;
                }
                if (((boundaryAtPosition__90663.boundaryStart.offset > this.range.end) && (boundaryAtPosition__90663.boundaryEnd.offset > this.range.end)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEnd__84632);
                    return SelectionResult.next;
                }
                if (backwardSelection__90740)
                {
                    _setSelectionPosition(_clampTextPosition(boundaryAtPosition__90663.boundaryStart), isEnd: isEnd__84632);
                    if ((boundaryAtPosition__90663.boundaryStart.offset < this.range.start))
                    {
                        return SelectionResult.previous;
                    }
                    if ((boundaryAtPosition__90663.boundaryStart.offset >= this.range.start))
                    {
                        return SelectionResult.end;
                    }
                }
                else
                {
                    if ((boundaryAtPosition__90663.boundaryEnd.offset <= this.range.end))
                    {
                        _setSelectionPosition(_clampTextPosition(boundaryAtPosition__90663.boundaryEnd), isEnd: isEnd__84632);
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPosition__90663.boundaryEnd.offset > this.range.end))
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEnd__84632);
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
        var isEnd__93752 = false;
        if (((this._selectableContainsOriginTextBoundary && (existingSelectionStart is not null)) && (existingSelectionEnd is not null)))
        {
            bool forwardSelection__94015 = (existingSelectionEnd.offset >= existingSelectionStart.offset);
            RenderParagraph originParagraph__94124 = _getOriginParagraph();
            var fragmentBelongsToOriginParagraph__94177 = (object.Equals(originParagraph__94124, this.paragraph));
            if (fragmentBelongsToOriginParagraph__94177)
            {
                return _updateSelectionStartEdgeByMultiSelectableTextBoundary((Func<TextPosition, string, (TextPosition boundaryEnd, TextPosition boundaryStart)>)getTextBoundary, paragraphContainsPosition, position, existingSelectionStart, existingSelectionEnd);
            }
            Matrix4 originTransform__94548 = originParagraph__94124.getTransformTo(null);
            originTransform__94548.invert();
            global::Doroti.Ui.Offset originParagraphLocalPosition__94655 = MatrixUtils.transformPoint(originTransform__94548, globalPosition);
            bool positionWithinOriginParagraph__94789 = originParagraph__94124.paintBounds.contains(originParagraphLocalPosition__94655);
            global::Doroti.Ui.TextPosition positionRelativeToOriginParagraph__94931 = originParagraph__94124.getPositionForOffset(originParagraphLocalPosition__94655);
            if (positionWithinOriginParagraph__94789)
            {
                string originText__95344 = ((RenderParagraph)originParagraph__94124).text.toPlainText(includeSemanticsLabels: false);
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPosition__95456 = getTextBoundary(positionRelativeToOriginParagraph__94931, originText__95344);
                (TextPosition boundaryEnd, TextPosition boundaryStart) originTextBoundary__95606 = getTextBoundary(_getPositionInParagraph(originParagraph__94124), originText__95344);
                global::Doroti.Ui.TextPosition targetPosition__95756 = default!;
                long pivotOffset__95790 = (forwardSelection__94015 ? originTextBoundary__95606.boundaryEnd.offset : originTextBoundary__95606.boundaryStart.offset);
                var shouldSwapEdges__95942 = (!forwardSelection__94015 != ((positionRelativeToOriginParagraph__94931.offset > pivotOffset__95790)));
                if ((positionRelativeToOriginParagraph__94931.offset < pivotOffset__95790))
                {
                    targetPosition__95756 = boundaryAtPosition__95456.boundaryStart;
                }
                else
                {
                    if ((positionRelativeToOriginParagraph__94931.offset > pivotOffset__95790))
                    {
                        targetPosition__95756 = boundaryAtPosition__95456.boundaryEnd;
                    }
                    else
                    {
                        targetPosition__95756 = existingSelectionStart;
                    }
                }
                if (shouldSwapEdges__95942)
                {
                    _setSelectionPosition(existingSelectionStart, isEnd: true);
                }
                _setSelectionPosition(_clampTextPosition(targetPosition__95756), isEnd: isEnd__93752);
                bool finalSelectionIsForward__96697 = (this._textSelectionEnd!.offset >= this._textSelectionStart!.offset);
                global::Doroti.Ui.TextPosition originParagraphPlaceholderTextPosition__96820 = _getPositionInParagraph(originParagraph__94124);
                var originParagraphPlaceholderRange__96938 = new global::Doroti.Ui.TextRange(start: originParagraphPlaceholderTextPosition__96820.offset, end: (originParagraphPlaceholderTextPosition__96820.offset + _placeholderLength));
                if (((boundaryAtPosition__95456.boundaryStart.offset > originParagraphPlaceholderRange__96938.end) && (boundaryAtPosition__95456.boundaryEnd.offset > originParagraphPlaceholderRange__96938.end)))
                {
                    return SelectionResult.next;
                }
                if (((boundaryAtPosition__95456.boundaryStart.offset < originParagraphPlaceholderRange__96938.start) && (boundaryAtPosition__95456.boundaryEnd.offset < originParagraphPlaceholderRange__96938.start)))
                {
                    return SelectionResult.previous;
                }
                if (finalSelectionIsForward__96697)
                {
                    if ((boundaryAtPosition__95456.boundaryEnd.offset <= originTextBoundary__95606.boundaryEnd.offset))
                    {
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPosition__95456.boundaryEnd.offset > originTextBoundary__95606.boundaryEnd.offset))
                    {
                        return SelectionResult.next;
                    }
                }
                else
                {
                    if ((boundaryAtPosition__95456.boundaryStart.offset >= originTextBoundary__95606.boundaryStart.offset))
                    {
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPosition__95456.boundaryStart.offset < originTextBoundary__95606.boundaryStart.offset))
                    {
                        return SelectionResult.previous;
                    }
                }
            }
            else
            {
                global::Doroti.Ui.Offset adjustedOffset__98687 = SelectionUtils.adjustDragOffset(originParagraph__94124.paintBounds, originParagraphLocalPosition__94655, direction: ((RenderParagraph)this.paragraph).textDirection);
                global::Doroti.Ui.TextPosition adjustedPositionRelativeToOriginParagraph__98900 = originParagraph__94124.getPositionForOffset(adjustedOffset__98687);
                global::Doroti.Ui.TextPosition originParagraphPlaceholderTextPosition__99038 = _getPositionInParagraph(originParagraph__94124);
                var originParagraphPlaceholderRange__99156 = new global::Doroti.Ui.TextRange(start: originParagraphPlaceholderTextPosition__99038.offset, end: (originParagraphPlaceholderTextPosition__99038.offset + _placeholderLength));
                if ((forwardSelection__94015 && (adjustedPositionRelativeToOriginParagraph__98900.offset <= originParagraphPlaceholderRange__99156.start)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEnd__93752);
                    return SelectionResult.previous;
                }
                if ((!forwardSelection__94015 && (adjustedPositionRelativeToOriginParagraph__98900.offset >= originParagraphPlaceholderRange__99156.end)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEnd__93752);
                    return SelectionResult.next;
                }
                if ((forwardSelection__94015 && (adjustedPositionRelativeToOriginParagraph__98900.offset >= originParagraphPlaceholderRange__99156.end)))
                {
                    _setSelectionPosition(existingSelectionStart, isEnd: true);
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEnd__93752);
                    return SelectionResult.next;
                }
                if ((!forwardSelection__94015 && (adjustedPositionRelativeToOriginParagraph__98900.offset <= originParagraphPlaceholderRange__99156.start)))
                {
                    _setSelectionPosition(existingSelectionStart, isEnd: true);
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEnd__93752);
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
                (RenderParagraph paragraph, global::Doroti.Ui.Offset localPosition)? targetDetails__101250 = _getParagraphContainingPosition(globalPosition);
                if ((targetDetails__101250 is null))
                {
                    return null;
                }
                RenderParagraph targetParagraph__101427 = DartRuntimePrimitives.RequireValue(targetDetails__101250).paragraph;
                global::Doroti.Ui.TextPosition positionRelativeToTargetParagraph__101497 = targetParagraph__101427.getPositionForOffset(DartRuntimePrimitives.RequireValue(targetDetails__101250).localPosition);
                string targetText__101642 = ((RenderParagraph)targetParagraph__101427).text.toPlainText(includeSemanticsLabels: false);
                var positionOnPlaceholder__101734 = (targetParagraph__101427.getWordBoundary(positionRelativeToTargetParagraph__101497).textInside(targetText__101642) == _placeholderCharacter);
                if (positionOnPlaceholder__101734)
                {
                    return null;
                }
                bool backwardSelection__102021 = ((((existingSelectionStart is null) && (existingSelectionEnd.offset == this.range.start)) || ((object.Equals(existingSelectionStart, existingSelectionEnd)) && (existingSelectionEnd.offset == this.range.start))) || ((existingSelectionStart is not null) && (existingSelectionStart.offset > existingSelectionEnd.offset)));
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPositionRelativeToTargetParagraph__102414 = getTextBoundary(positionRelativeToTargetParagraph__101497, targetText__101642);
                global::Doroti.Ui.TextPosition targetParagraphPlaceholderTextPosition__102582 = _getPositionInParagraph(targetParagraph__101427);
                var targetParagraphPlaceholderRange__102700 = new global::Doroti.Ui.TextRange(start: targetParagraphPlaceholderTextPosition__102582.offset, end: (targetParagraphPlaceholderTextPosition__102582.offset + _placeholderLength));
                if (((boundaryAtPositionRelativeToTargetParagraph__102414.boundaryStart.offset < targetParagraphPlaceholderRange__102700.start) && (boundaryAtPositionRelativeToTargetParagraph__102414.boundaryEnd.offset < targetParagraphPlaceholderRange__102700.start)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEnd__93752);
                    return SelectionResult.previous;
                }
                if (((boundaryAtPositionRelativeToTargetParagraph__102414.boundaryStart.offset > targetParagraphPlaceholderRange__102700.end) && (boundaryAtPositionRelativeToTargetParagraph__102414.boundaryEnd.offset > targetParagraphPlaceholderRange__102700.end)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEnd__93752);
                    return SelectionResult.next;
                }
                if (backwardSelection__102021)
                {
                    if ((boundaryAtPositionRelativeToTargetParagraph__102414.boundaryEnd.offset <= targetParagraphPlaceholderRange__102700.end))
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEnd__93752);
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPositionRelativeToTargetParagraph__102414.boundaryEnd.offset > targetParagraphPlaceholderRange__102700.end))
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEnd__93752);
                        return SelectionResult.next;
                    }
                }
                else
                {
                    if ((boundaryAtPositionRelativeToTargetParagraph__102414.boundaryStart.offset >= targetParagraphPlaceholderRange__102700.start))
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEnd__93752);
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPositionRelativeToTargetParagraph__102414.boundaryStart.offset < targetParagraphPlaceholderRange__102700.start))
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEnd__93752);
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
        var isEnd__105892 = true;
        if (((this._selectableContainsOriginTextBoundary && (existingSelectionStart is not null)) && (existingSelectionEnd is not null)))
        {
            bool forwardSelection__106154 = (existingSelectionEnd.offset >= existingSelectionStart.offset);
            RenderParagraph originParagraph__106263 = _getOriginParagraph();
            var fragmentBelongsToOriginParagraph__106316 = (object.Equals(originParagraph__106263, this.paragraph));
            if (fragmentBelongsToOriginParagraph__106316)
            {
                return _updateSelectionEndEdgeByMultiSelectableTextBoundary((Func<TextPosition, string, (TextPosition boundaryEnd, TextPosition boundaryStart)>)getTextBoundary, paragraphContainsPosition, position, existingSelectionStart, existingSelectionEnd);
            }
            Matrix4 originTransform__106685 = originParagraph__106263.getTransformTo(null);
            originTransform__106685.invert();
            global::Doroti.Ui.Offset originParagraphLocalPosition__106792 = MatrixUtils.transformPoint(originTransform__106685, globalPosition);
            bool positionWithinOriginParagraph__106926 = originParagraph__106263.paintBounds.contains(originParagraphLocalPosition__106792);
            global::Doroti.Ui.TextPosition positionRelativeToOriginParagraph__107068 = originParagraph__106263.getPositionForOffset(originParagraphLocalPosition__106792);
            if (positionWithinOriginParagraph__106926)
            {
                string originText__107481 = ((RenderParagraph)originParagraph__106263).text.toPlainText(includeSemanticsLabels: false);
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPosition__107593 = getTextBoundary(positionRelativeToOriginParagraph__107068, originText__107481);
                (TextPosition boundaryEnd, TextPosition boundaryStart) originTextBoundary__107743 = getTextBoundary(_getPositionInParagraph(originParagraph__106263), originText__107481);
                global::Doroti.Ui.TextPosition targetPosition__107893 = default!;
                long pivotOffset__107927 = (forwardSelection__106154 ? originTextBoundary__107743.boundaryStart.offset : originTextBoundary__107743.boundaryEnd.offset);
                var shouldSwapEdges__108079 = (!forwardSelection__106154 != ((positionRelativeToOriginParagraph__107068.offset < pivotOffset__107927)));
                if ((positionRelativeToOriginParagraph__107068.offset < pivotOffset__107927))
                {
                    targetPosition__107893 = boundaryAtPosition__107593.boundaryStart;
                }
                else
                {
                    if ((positionRelativeToOriginParagraph__107068.offset > pivotOffset__107927))
                    {
                        targetPosition__107893 = boundaryAtPosition__107593.boundaryEnd;
                    }
                    else
                    {
                        targetPosition__107893 = existingSelectionEnd;
                    }
                }
                if (shouldSwapEdges__108079)
                {
                    _setSelectionPosition(existingSelectionEnd, isEnd: false);
                }
                _setSelectionPosition(_clampTextPosition(targetPosition__107893), isEnd: isEnd__105892);
                bool finalSelectionIsForward__108831 = (this._textSelectionEnd!.offset >= this._textSelectionStart!.offset);
                global::Doroti.Ui.TextPosition originParagraphPlaceholderTextPosition__108954 = _getPositionInParagraph(originParagraph__106263);
                var originParagraphPlaceholderRange__109072 = new global::Doroti.Ui.TextRange(start: originParagraphPlaceholderTextPosition__108954.offset, end: (originParagraphPlaceholderTextPosition__108954.offset + _placeholderLength));
                if (((boundaryAtPosition__107593.boundaryStart.offset > originParagraphPlaceholderRange__109072.end) && (boundaryAtPosition__107593.boundaryEnd.offset > originParagraphPlaceholderRange__109072.end)))
                {
                    return SelectionResult.next;
                }
                if (((boundaryAtPosition__107593.boundaryStart.offset < originParagraphPlaceholderRange__109072.start) && (boundaryAtPosition__107593.boundaryEnd.offset < originParagraphPlaceholderRange__109072.start)))
                {
                    return SelectionResult.previous;
                }
                if (finalSelectionIsForward__108831)
                {
                    if ((boundaryAtPosition__107593.boundaryEnd.offset <= originTextBoundary__107743.boundaryEnd.offset))
                    {
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPosition__107593.boundaryEnd.offset > originTextBoundary__107743.boundaryEnd.offset))
                    {
                        return SelectionResult.next;
                    }
                }
                else
                {
                    if ((boundaryAtPosition__107593.boundaryStart.offset >= originTextBoundary__107743.boundaryStart.offset))
                    {
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPosition__107593.boundaryStart.offset < originTextBoundary__107743.boundaryStart.offset))
                    {
                        return SelectionResult.previous;
                    }
                }
            }
            else
            {
                global::Doroti.Ui.Offset adjustedOffset__110821 = SelectionUtils.adjustDragOffset(originParagraph__106263.paintBounds, originParagraphLocalPosition__106792, direction: ((RenderParagraph)this.paragraph).textDirection);
                global::Doroti.Ui.TextPosition adjustedPositionRelativeToOriginParagraph__111034 = originParagraph__106263.getPositionForOffset(adjustedOffset__110821);
                global::Doroti.Ui.TextPosition originParagraphPlaceholderTextPosition__111172 = _getPositionInParagraph(originParagraph__106263);
                var originParagraphPlaceholderRange__111290 = new global::Doroti.Ui.TextRange(start: originParagraphPlaceholderTextPosition__111172.offset, end: (originParagraphPlaceholderTextPosition__111172.offset + _placeholderLength));
                if ((forwardSelection__106154 && (adjustedPositionRelativeToOriginParagraph__111034.offset <= originParagraphPlaceholderRange__111290.start)))
                {
                    _setSelectionPosition(existingSelectionEnd, isEnd: false);
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEnd__105892);
                    return SelectionResult.previous;
                }
                if ((!forwardSelection__106154 && (adjustedPositionRelativeToOriginParagraph__111034.offset >= originParagraphPlaceholderRange__111290.end)))
                {
                    _setSelectionPosition(existingSelectionEnd, isEnd: false);
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEnd__105892);
                    return SelectionResult.next;
                }
                if ((forwardSelection__106154 && (adjustedPositionRelativeToOriginParagraph__111034.offset >= originParagraphPlaceholderRange__111290.end)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEnd__105892);
                    return SelectionResult.next;
                }
                if ((!forwardSelection__106154 && (adjustedPositionRelativeToOriginParagraph__111034.offset <= originParagraphPlaceholderRange__111290.start)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEnd__105892);
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
                (RenderParagraph paragraph, global::Doroti.Ui.Offset localPosition)? targetDetails__113382 = _getParagraphContainingPosition(globalPosition);
                if ((targetDetails__113382 is null))
                {
                    return null;
                }
                RenderParagraph targetParagraph__113559 = DartRuntimePrimitives.RequireValue(targetDetails__113382).paragraph;
                global::Doroti.Ui.TextPosition positionRelativeToTargetParagraph__113629 = targetParagraph__113559.getPositionForOffset(DartRuntimePrimitives.RequireValue(targetDetails__113382).localPosition);
                string targetText__113774 = ((RenderParagraph)targetParagraph__113559).text.toPlainText(includeSemanticsLabels: false);
                var positionOnPlaceholder__113866 = (targetParagraph__113559.getWordBoundary(positionRelativeToTargetParagraph__113629).textInside(targetText__113774) == _placeholderCharacter);
                if (positionOnPlaceholder__113866)
                {
                    return null;
                }
                bool backwardSelection__114153 = ((((existingSelectionEnd is null) && (existingSelectionStart.offset == this.range.end)) || ((object.Equals(existingSelectionStart, existingSelectionEnd)) && (existingSelectionStart.offset == this.range.end))) || ((existingSelectionEnd is not null) && (existingSelectionStart.offset > existingSelectionEnd.offset)));
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPositionRelativeToTargetParagraph__114542 = getTextBoundary(positionRelativeToTargetParagraph__113629, targetText__113774);
                global::Doroti.Ui.TextPosition targetParagraphPlaceholderTextPosition__114710 = _getPositionInParagraph(targetParagraph__113559);
                var targetParagraphPlaceholderRange__114828 = new global::Doroti.Ui.TextRange(start: targetParagraphPlaceholderTextPosition__114710.offset, end: (targetParagraphPlaceholderTextPosition__114710.offset + _placeholderLength));
                if (((boundaryAtPositionRelativeToTargetParagraph__114542.boundaryStart.offset < targetParagraphPlaceholderRange__114828.start) && (boundaryAtPositionRelativeToTargetParagraph__114542.boundaryEnd.offset < targetParagraphPlaceholderRange__114828.start)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEnd__105892);
                    return SelectionResult.previous;
                }
                if (((boundaryAtPositionRelativeToTargetParagraph__114542.boundaryStart.offset > targetParagraphPlaceholderRange__114828.end) && (boundaryAtPositionRelativeToTargetParagraph__114542.boundaryEnd.offset > targetParagraphPlaceholderRange__114828.end)))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEnd__105892);
                    return SelectionResult.next;
                }
                if (backwardSelection__114153)
                {
                    if ((boundaryAtPositionRelativeToTargetParagraph__114542.boundaryStart.offset >= targetParagraphPlaceholderRange__114828.start))
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEnd__105892);
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPositionRelativeToTargetParagraph__114542.boundaryStart.offset < targetParagraphPlaceholderRange__114828.start))
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start), isEnd: isEnd__105892);
                        return SelectionResult.previous;
                    }
                }
                else
                {
                    if ((boundaryAtPositionRelativeToTargetParagraph__114542.boundaryEnd.offset <= targetParagraphPlaceholderRange__114828.end))
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEnd__105892);
                        return SelectionResult.end;
                    }
                    if ((boundaryAtPositionRelativeToTargetParagraph__114542.boundaryEnd.offset > targetParagraphPlaceholderRange__114828.end))
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: this.range.end), isEnd: isEnd__105892);
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
        global::Doroti.Ui.TextPosition? existingSelectionStart__117552 = this._textSelectionStart;
        global::Doroti.Ui.TextPosition? existingSelectionEnd__117622 = this._textSelectionEnd;
        _setSelectionPosition(null, isEnd: isEnd);
        Matrix4 transform__117730 = this.paragraph.getTransformTo(null);
        transform__117730.invert();
        global::Doroti.Ui.Offset localPosition__117815 = MatrixUtils.transformPoint(transform__117730, globalPosition);
        if (this._rect.isEmpty)
        {
            SelectionResult result__117939 = SelectionUtils.getResultBasedOnRect(this._rect, localPosition__117815);
            _setSelectionPosition(((object.Equals(result__117939, SelectionResult.next)) ? new global::Doroti.Ui.TextPosition(offset: this.range.end) : new global::Doroti.Ui.TextPosition(offset: this.range.start, affinity: TextAffinity.upstream)), isEnd: isEnd);
            return result__117939;
        }
        global::Doroti.Ui.Offset adjustedOffset__118278 = SelectionUtils.adjustDragOffset(this._rect, localPosition__117815, direction: ((RenderParagraph)this.paragraph).textDirection);
        global::Doroti.Ui.Offset adjustedOffsetRelativeToParagraph__118428 = SelectionUtils.adjustDragOffset(this.paragraph.paintBounds, localPosition__117815, direction: ((RenderParagraph)this.paragraph).textDirection);
        global::Doroti.Ui.TextPosition position__118620 = this.paragraph.getPositionForOffset(adjustedOffset__118278);
        global::Doroti.Ui.TextPosition positionInFullText__118702 = this.paragraph.getPositionForOffset(adjustedOffsetRelativeToParagraph__118428);
        SelectionResult? result__118831 = default!;
        if (_isPlaceholder())
        {
            result__118831 = (isEnd ? _updateSelectionEndEdgeAtPlaceholderByMultiSelectableTextBoundary((Func<TextPosition, string, (TextPosition boundaryEnd, TextPosition boundaryStart)>)getTextBoundary, globalPosition, this.paragraph.paintBounds.contains(localPosition__117815), positionInFullText__118702, existingSelectionStart__117552, existingSelectionEnd__117622) : _updateSelectionStartEdgeAtPlaceholderByMultiSelectableTextBoundary((Func<TextPosition, string, (TextPosition boundaryEnd, TextPosition boundaryStart)>)getTextBoundary, globalPosition, this.paragraph.paintBounds.contains(localPosition__117815), positionInFullText__118702, existingSelectionStart__117552, existingSelectionEnd__117622));
        }
        else
        {
            result__118831 = (isEnd ? _updateSelectionEndEdgeByMultiSelectableTextBoundary((Func<TextPosition, string, (TextPosition boundaryEnd, TextPosition boundaryStart)>)getTextBoundary, this.paragraph.paintBounds.contains(localPosition__117815), positionInFullText__118702, existingSelectionStart__117552, existingSelectionEnd__117622) : _updateSelectionStartEdgeByMultiSelectableTextBoundary((Func<TextPosition, string, (TextPosition boundaryEnd, TextPosition boundaryStart)>)getTextBoundary, this.paragraph.paintBounds.contains(localPosition__117815), positionInFullText__118702, existingSelectionStart__117552, existingSelectionEnd__117622));
        }
        if ((result__118831 is not null))
        {
            SelectionResult result__118831__value120148 = DartRuntimePrimitives.RequireValue(result__118831);
            return DartRuntimePrimitives.RequireValue(result__118831__value120148);
        }
        (TextPosition boundaryEnd, TextPosition boundaryStart)? textBoundary__120500 = (_boundingBoxesContains(localPosition__117815) ? getClampedTextBoundary(position__118620) : null);
        if (((textBoundary__120500 is not null) && ((((DartRuntimePrimitives.RequireValue(textBoundary__120500).boundaryStart.offset < this.range.start) && (DartRuntimePrimitives.RequireValue(textBoundary__120500).boundaryEnd.offset <= this.range.start)) || ((DartRuntimePrimitives.RequireValue(textBoundary__120500).boundaryStart.offset >= this.range.end) && (DartRuntimePrimitives.RequireValue(textBoundary__120500).boundaryEnd.offset > this.range.end))))))
        {
            (TextPosition boundaryEnd, TextPosition boundaryStart) textBoundary__120500__value120620 = DartRuntimePrimitives.RequireValue(textBoundary__120500);
            textBoundary__120500 = null;
        }
        global::Doroti.Ui.TextPosition targetPosition__121272 = _clampTextPosition((isEnd ? _updateSelectionEndEdgeByTextBoundary(textBoundary__120500, (Func<TextPosition, (TextPosition boundaryEnd, TextPosition boundaryStart)>)getClampedTextBoundary, position__118620, existingSelectionStart__117552, existingSelectionEnd__117622) : _updateSelectionStartEdgeByTextBoundary(textBoundary__120500, (Func<TextPosition, (TextPosition boundaryEnd, TextPosition boundaryStart)>)getClampedTextBoundary, position__118620, existingSelectionStart__117552, existingSelectionEnd__117622)));
        _setSelectionPosition(targetPosition__121272, isEnd: isEnd);
        if ((targetPosition__121272.offset == this.range.end))
        {
            return SelectionResult.next;
        }
        if ((targetPosition__121272.offset == this.range.start))
        {
            return SelectionResult.previous;
        }
        return SelectionUtils.getResultBasedOnRect(this._rect, localPosition__117815);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _closestTextBoundary((TextPosition boundaryEnd, TextPosition boundaryStart) textBoundary, TextPosition position)
    {
        long differenceA__122480 = ((position.offset - DartRuntimePrimitives.RequireValue(textBoundary).boundaryStart.offset)).abs();
        long differenceB__122569 = ((position.offset - DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd.offset)).abs();
        return ((differenceA__122480 < differenceB__122569) ? DartRuntimePrimitives.RequireValue(textBoundary).boundaryStart : DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isPlaceholder()
    {
        RenderObject? current__122853 = this.paragraph.parent;
        while ((current__122853 is not null))
        {
            if ((current__122853 is RenderParagraph))
            {
                RenderParagraph current__122853__as122921 = (RenderParagraph)current__122853;
                return true;
            }
            current__122853 = ((RenderObject)current__122853).parent;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual RenderParagraph _getOriginParagraph()
    {
        DartRuntimePrimitives.Assert(() => this._selectableContainsOriginTextBoundary);
        RenderObject? current__123508 = this.paragraph.parent;
        RenderParagraph? originParagraph__123557 = default!;
        while ((current__123508 is not null))
        {
            if ((current__123508 is RenderParagraph))
            {
                RenderParagraph current__123508__as123614 = (RenderParagraph)current__123508;
                if ((((RenderParagraph)((RenderParagraph)current__123508__as123614))._lastSelectableFragments is not null))
                {
                    var paragraphContainsOriginTextBoundary__123714 = false;
                    foreach (_SelectableFragment__paragraph fragment__123800 in ((RenderParagraph)((RenderParagraph)current__123508__as123614))._lastSelectableFragments!)
                    {
                        if (((_SelectableFragment__paragraph)fragment__123800)._selectableContainsOriginTextBoundary)
                        {
                            paragraphContainsOriginTextBoundary__123714 = true;
                            originParagraph__123557 = ((RenderParagraph)current__123508__as123614);
                            break;
                        }
                    }
                    if (!paragraphContainsOriginTextBoundary__123714)
                    {
                        return (originParagraph__123557 ?? this.paragraph);
                    }
                }
            }
            current__123508 = ((RenderObject)current__123508).parent;
        }
        return (originParagraph__123557 ?? this.paragraph);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (RenderParagraph paragraph, global::Doroti.Ui.Offset localPosition)? _getParagraphContainingPosition(Offset globalPosition)
    {
        RenderObject? current__124717 = this.paragraph;
        while ((current__124717 is not null))
        {
            if ((current__124717 is RenderParagraph))
            {
                RenderParagraph current__124717__as124778 = (RenderParagraph)current__124717;
                Matrix4 currentTransform__124830 = ((RenderParagraph)current__124717__as124778).getTransformTo(null);
                currentTransform__124830.invert();
                global::Doroti.Ui.Offset currentParagraphLocalPosition__124935 = MatrixUtils.transformPoint(currentTransform__124830, globalPosition);
                bool positionWithinCurrentParagraph__125079 = ((RenderParagraph)current__124717__as124778).paintBounds.contains(currentParagraphLocalPosition__124935);
                if (positionWithinCurrentParagraph__125079)
                {
                    return (paragraph: ((RenderParagraph)current__124717__as124778), localPosition: currentParagraphLocalPosition__124935);
                }
            }
            current__124717 = ((RenderObject)current__124717).parent;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _boundingBoxesContains(Offset position)
    {
        foreach (global::Doroti.Ui.Rect rect__125472 in this.boundingBoxes)
        {
            if (rect__125472.contains(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(position))))
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
        long startMax__127878 = Math.Max(a.start, b.start);
        long endMin__127931 = Math.Min(a.end, b.end);
        if ((startMax__127878 <= endMin__127931))
        {
            return new global::Doroti.Ui.TextRange(start: startMax__127878, end: endMin__127931);
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
        var boundaryAsRange__128798 = new global::Doroti.Ui.TextRange(start: DartRuntimePrimitives.RequireValue(textBoundary).boundaryStart.offset, end: DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd.offset);
        global::Doroti.Ui.TextRange? intersectRange__128947 = _intersect(this.range, boundaryAsRange__128798);
        if ((intersectRange__128947 is not null))
        {
            _textSelectionStart = new global::Doroti.Ui.TextPosition(offset: intersectRange__128947.start);
            _textSelectionEnd = new global::Doroti.Ui.TextPosition(offset: intersectRange__128947.end);
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
        global::Doroti.Ui.TextPosition start__129535 = default!;
        global::Doroti.Ui.TextPosition end__129570 = default!;
        if ((position.offset > textBoundary.end))
        {
            start__129535 = end__129570 = new global::Doroti.Ui.TextPosition(offset: position.offset);
        }
        else
        {
            start__129535 = new global::Doroti.Ui.TextPosition(offset: textBoundary.start);
            end__129570 = new global::Doroti.Ui.TextPosition(offset: textBoundary.end, affinity: TextAffinity.upstream);
        }
        return (boundaryEnd: end__129570, boundaryStart: start__129535);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _handleSelectWord(Offset globalPosition)
    {
        global::Doroti.Ui.TextPosition position__129982 = this.paragraph.getPositionForOffset(this.paragraph.globalToLocal(globalPosition));
        if ((_positionIsWithinCurrentSelection(position__129982) && (!object.Equals(this._textSelectionStart, this._textSelectionEnd))))
        {
            return SelectionResult.end;
        }
        (TextPosition boundaryEnd, TextPosition boundaryStart) wordBoundary__130248 = _getWordBoundaryAtPosition(position__129982);
        return _handleSelectTextBoundary(wordBoundary__130248);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (TextPosition boundaryEnd, TextPosition boundaryStart) _getWordBoundaryAtPosition(TextPosition position)
    {
        global::Doroti.Ui.TextRange word__130452 = this.paragraph.getWordBoundary(position);
        DartRuntimePrimitives.Assert(() => word__130452.isNormalized);
        return _adjustTextBoundaryAtPosition(word__130452, position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _handleSelectParagraph(Offset globalPosition)
    {
        global::Doroti.Ui.Offset localPosition__130673 = this.paragraph.globalToLocal(globalPosition);
        global::Doroti.Ui.TextPosition position__130753 = this.paragraph.getPositionForOffset(localPosition__130673);
        (TextPosition boundaryEnd, TextPosition boundaryStart) paragraphBoundary__130841 = _getParagraphBoundaryAtPosition(position__130753, this.fullText);
        return _handleSelectMultiFragmentTextBoundary(paragraphBoundary__130841);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _getPositionInParagraph(RenderParagraph targetParagraph)
    {
        Matrix4 transform__131100 = this.paragraph.getTransformTo(targetParagraph);
        global::Doroti.Ui.Offset localCenter__131172 = this.paragraph.paintBounds.centerLeft;
        global::Doroti.Ui.Offset localPos__131237 = MatrixUtils.transformPoint(transform__131100, localCenter__131172);
        global::Doroti.Ui.TextPosition position__131323 = targetParagraph.getPositionForOffset(localPos__131237);
        return position__131323;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (TextPosition boundaryEnd, TextPosition boundaryStart) _getParagraphBoundaryAtPosition(TextPosition position, string text)
    {
        var paragraphBoundary__131510 = new ParagraphBoundary(text);
        long paragraphStart__131706 = (paragraphBoundary__131510.getLeadingTextBoundaryAt((((position.offset == text.Length) || (object.Equals(position.affinity, TextAffinity.upstream))) ? (position.offset - 1L) : position.offset)) ?? 0L);
        long paragraphEnd__131969 = (paragraphBoundary__131510.getTrailingTextBoundaryAt(position.offset) ?? text.Length);
        var paragraphRange__132079 = new global::Doroti.Ui.TextRange(start: paragraphStart__131706, end: paragraphEnd__131969);
        DartRuntimePrimitives.Assert(() => paragraphRange__132079.isNormalized);
        return _adjustTextBoundaryAtPosition(paragraphRange__132079, position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (TextPosition boundaryEnd, TextPosition boundaryStart) _getClampedParagraphBoundaryAtPosition(TextPosition position)
    {
        var paragraphBoundary__132359 = new ParagraphBoundary(this.fullText);
        long paragraphStart__132553 = (paragraphBoundary__132359.getLeadingTextBoundaryAt((((position.offset == this.fullText.Length) || (object.Equals(position.affinity, TextAffinity.upstream))) ? (position.offset - 1L) : position.offset)) ?? 0L);
        long paragraphEnd__132814 = (paragraphBoundary__132359.getTrailingTextBoundaryAt(position.offset) ?? this.fullText.Length);
        paragraphStart__132553 = ((paragraphStart__132553 < this.range.start) ? this.range.start : ((paragraphStart__132553 > this.range.end) ? this.range.end : paragraphStart__132553));
        paragraphEnd__132814 = ((paragraphEnd__132814 > this.range.end) ? this.range.end : ((paragraphEnd__132814 < this.range.start) ? this.range.start : paragraphEnd__132814));
        var paragraphRange__133230 = new global::Doroti.Ui.TextRange(start: paragraphStart__132553, end: paragraphEnd__132814);
        DartRuntimePrimitives.Assert(() => paragraphRange__133230.isNormalized);
        return _adjustTextBoundaryAtPosition(paragraphRange__133230, position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _handleDirectionallyExtendSelection(double horizontalBaseline, bool isExtent, SelectionExtendDirection movement)
    {
        Matrix4 transform__133582 = this.paragraph.getTransformTo(null);
        if ((transform__133582.invert() == 0.0))
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
        double baselineInParagraphCoordinates__133997 = MatrixUtils.transformPoint(transform__133582, new global::Doroti.Ui.Offset(horizontalBaseline, 0)).dx;
        DartRuntimePrimitives.Assert(() => !double.IsNaN(baselineInParagraphCoordinates__133997));
        global::Doroti.Ui.TextPosition newPosition__134196 = default!;
        SelectionResult result__134235 = default!;
        switch (movement)
        {
            case SelectionExtendDirection.previousLine:
            case SelectionExtendDirection.nextLine:
                {
                    DartRuntimePrimitives.Assert(() => ((this._textSelectionEnd is not null) && (this._textSelectionStart is not null)));
                    global::Doroti.Ui.TextPosition targetedEdge__134464 = (isExtent ? this._textSelectionEnd! : this._textSelectionStart!);
                    MapEntry<global::Doroti.Ui.TextPosition, SelectionResult> moveResult__134587 = _handleVerticalMovement(targetedEdge__134464, horizontalBaselineInParagraphCoordinates: baselineInParagraphCoordinates__133997, below: (object.Equals(movement, SelectionExtendDirection.nextLine)));
                    newPosition__134196 = moveResult__134587.key;
                    result__134235 = moveResult__134587.value;
                    break;
                }
            case SelectionExtendDirection.forward:
            case SelectionExtendDirection.backward:
                {
                    _textSelectionEnd ??= ((object.Equals(movement, SelectionExtendDirection.forward)) ? new global::Doroti.Ui.TextPosition(offset: this.range.start) : new global::Doroti.Ui.TextPosition(offset: this.range.end, affinity: TextAffinity.upstream));
                    _textSelectionStart ??= this._textSelectionEnd;
                    global::Doroti.Ui.TextPosition targetedEdge__135253 = (isExtent ? this._textSelectionEnd! : this._textSelectionStart!);
                    global::Doroti.Ui.Offset edgeOffsetInParagraphCoordinates__135343 = this.paragraph._getOffsetForPosition(targetedEdge__135253);
                    var baselineOffsetInParagraphCoordinates__135460 = new global::Doroti.Ui.Offset(baselineInParagraphCoordinates__133997, (edgeOffsetInParagraphCoordinates__135343.dy - (((RenderParagraph)this.paragraph)._textPainter.preferredLineHeight / 2L)));
                    newPosition__134196 = this.paragraph.getPositionForOffset(baselineOffsetInParagraphCoordinates__135460);
                    result__134235 = SelectionResult.end;
                    break;
                }
        }
        if (isExtent)
        {
            _textSelectionEnd = newPosition__134196;
        }
        else
        {
            _textSelectionStart = newPosition__134196;
        }
        return result__134235;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _handleGranularlyExtendSelection(bool forward, bool isExtent, TextGranularity granularity)
    {
        _textSelectionEnd ??= (forward ? new global::Doroti.Ui.TextPosition(offset: this.range.start) : new global::Doroti.Ui.TextPosition(offset: this.range.end, affinity: TextAffinity.upstream));
        _textSelectionStart ??= this._textSelectionEnd;
        global::Doroti.Ui.TextPosition targetedEdge__136360 = (isExtent ? this._textSelectionEnd! : this._textSelectionStart!);
        if ((forward && ((targetedEdge__136360.offset == this.range.end))))
        {
            return SelectionResult.next;
        }
        if ((!forward && ((targetedEdge__136360.offset == this.range.start))))
        {
            return SelectionResult.previous;
        }
        SelectionResult result__136658 = default!;
        global::Doroti.Ui.TextPosition newPosition__136689 = default!;
        switch (granularity)
        {
            case TextGranularity.character:
                {
                    string text__136788 = this.range.textInside(this.fullText);
                    newPosition__136689 = _moveBeyondTextBoundaryAtDirection(targetedEdge__136360, forward, new CharacterBoundary(text__136788));
                    result__136658 = SelectionResult.end;
                    break;
                }
            case TextGranularity.word:
                {
                    TextBoundary textBoundary__137068 = ((RenderParagraph)this.paragraph)._textPainter.wordBoundaries.moveByWordBoundary;
                    newPosition__136689 = _moveBeyondTextBoundaryAtDirection(targetedEdge__136360, forward, textBoundary__137068);
                    result__136658 = SelectionResult.end;
                    break;
                }
            case TextGranularity.paragraph:
                {
                    string text__137333 = this.range.textInside(this.fullText);
                    newPosition__136689 = _moveBeyondTextBoundaryAtDirection(targetedEdge__136360, forward, new ParagraphBoundary(text__137333));
                    result__136658 = SelectionResult.end;
                    break;
                }
            case TextGranularity.line:
                {
                    newPosition__136689 = _moveToTextBoundaryAtDirection(targetedEdge__136360, forward, new LineBoundary(this));
                    result__136658 = SelectionResult.end;
                    break;
                }
            case TextGranularity.document:
                {
                    string text__137779 = this.range.textInside(this.fullText);
                    newPosition__136689 = _moveBeyondTextBoundaryAtDirection(targetedEdge__136360, forward, new DocumentBoundary(text__137779));
                    if ((forward && (newPosition__136689.offset == this.range.end)))
                    {
                        result__136658 = SelectionResult.next;
                    }
                    else
                    {
                        if ((!forward && (newPosition__136689.offset == this.range.start)))
                        {
                            result__136658 = SelectionResult.previous;
                        }
                        else
                        {
                            result__136658 = SelectionResult.end;
                        }
                    }
                    break;
                }
        }
        if (isExtent)
        {
            _textSelectionEnd = newPosition__136689;
        }
        else
        {
            _textSelectionStart = newPosition__136689;
        }
        return result__136658;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _moveBeyondTextBoundaryAtDirection(TextPosition end, bool forward, TextBoundary textBoundary)
    {
        long newOffset__138794 = (forward ? (textBoundary.getTrailingTextBoundaryAt(end.offset) ?? this.range.end) : (textBoundary.getLeadingTextBoundaryAt((end.offset - 1L)) ?? this.range.start));
        return new global::Doroti.Ui.TextPosition(offset: newOffset__138794);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _moveToTextBoundaryAtDirection(TextPosition end, bool forward, TextBoundary textBoundary)
    {
        DartRuntimePrimitives.Assert(() => (end.offset >= 0L));
        long caretOffset__139390 = default!;
        switch (end.affinity)
        {
            case TextAffinity.upstream:
                {
                    if (((end.offset < 1L) && !forward))
                    {
                        DartRuntimePrimitives.Assert(() => (end.offset == 0L));
                        return new global::Doroti.Ui.TextPosition(offset: 0L);
                    }
                    var characterBoundary__139614 = new CharacterBoundary(this.fullText);
                    caretOffset__139390 = (Math.Max(0L, (characterBoundary__139614.getLeadingTextBoundaryAt((this.range.start + end.offset)) ?? this.range.start)) - 1L);
                    break;
                }
            case TextAffinity.downstream:
                {
                    caretOffset__139390 = end.offset;
                    break;
                }
        }
        long offset__139944 = (forward ? (textBoundary.getTrailingTextBoundaryAt(caretOffset__139390) ?? this.range.end) : (textBoundary.getLeadingTextBoundaryAt(caretOffset__139390) ?? this.range.start));
        return new global::Doroti.Ui.TextPosition(offset: offset__139944);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual MapEntry<global::Doroti.Ui.TextPosition, SelectionResult> _handleVerticalMovement(TextPosition position, double horizontalBaselineInParagraphCoordinates, bool below)
    {
        List<global::Doroti.Ui.LineMetrics> lines__140380 = ((RenderParagraph)this.paragraph)._textPainter.computeLineMetrics();
        global::Doroti.Ui.Offset offset__140450 = this.paragraph.getOffsetForCaret(position, Rect.zero);
        long currentLine__140517 = (checked((long)(lines__140380.Count)) - 1L);
        foreach (var lineMetrics__140564 in lines__140380)
        {
            if ((lineMetrics__140564.baseline > offset__140450.dy))
            {
                currentLine__140517 = lineMetrics__140564.lineNumber;
                break;
            }
        }
        global::Doroti.Ui.TextPosition newPosition__140732 = default!;
        if ((below && (currentLine__140517 == (checked((long)(lines__140380.Count)) - 1L))))
        {
            newPosition__140732 = new global::Doroti.Ui.TextPosition(offset: this.range.end, affinity: TextAffinity.upstream);
        }
        else
        {
            if ((!below && (currentLine__140517 == 0L)))
            {
                newPosition__140732 = new global::Doroti.Ui.TextPosition(offset: this.range.start);
            }
            else
            {
                long newLine__141012 = (below ? (currentLine__140517 + 1L) : (currentLine__140517 - 1L));
                newPosition__140732 = _clampTextPosition(this.paragraph.getPositionForOffset(new global::Doroti.Ui.Offset(horizontalBaselineInParagraphCoordinates, lines__140380[(int)(newLine__141012)].baseline)));
            }
        }
        SelectionResult result__141282 = default!;
        if ((newPosition__140732.offset == this.range.start))
        {
            result__141282 = SelectionResult.previous;
        }
        else
        {
            if ((newPosition__140732.offset == this.range.end))
            {
                result__141282 = SelectionResult.next;
            }
            else
            {
                result__141282 = SelectionResult.end;
            }
        }
        DartRuntimePrimitives.Assert(() => ((!object.Equals(result__141282, SelectionResult.next)) || below));
        DartRuntimePrimitives.Assert(() => ((!object.Equals(result__141282, SelectionResult.previous)) || !below));
        return new MapEntry<global::Doroti.Ui.TextPosition, SelectionResult>(newPosition__140732, result__141282);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _positionIsWithinCurrentSelection(TextPosition position)
    {
        if (((this._textSelectionStart is null) || (this._textSelectionEnd is null)))
        {
            return false;
        }
        global::Doroti.Ui.TextPosition currentStart__142072 = default!;
        global::Doroti.Ui.TextPosition currentEnd__142108 = default!;
        if ((_compareTextPositions(this._textSelectionStart!, this._textSelectionEnd!) > 0L))
        {
            currentStart__142072 = this._textSelectionStart!;
            currentEnd__142108 = this._textSelectionEnd!;
        }
        else
        {
            currentStart__142072 = this._textSelectionEnd!;
            currentEnd__142108 = this._textSelectionStart!;
        }
        return ((_compareTextPositions(currentStart__142072, position) >= 0L) && (_compareTextPositions(currentEnd__142108, position) <= 0L));
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
                List<global::Doroti.Ui.TextBox> boxes__143807 = this.paragraph.getBoxesForSelection(new TextSelection(baseOffset: this.range.start, extentOffset: this.range.end), boxHeightStyle: BoxHeightStyle.max);
                if ((checked((long)(boxes__143807.Count)) != 0))
                {
                    _cachedBoundingBoxes = new List<global::Doroti.Ui.Rect>();
                    foreach (var textBox__144049 in boxes__143807)
                    {
                        this._cachedBoundingBoxes!.Add(textBox__144049.toRect());
                    }
                }
                else
                {
                    global::Doroti.Ui.Offset offset__144170 = this.paragraph._getOffsetForPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start));
                    var rect__144261 = global::Doroti.Ui.Rect.fromPoints(offset__144170, offset__144170.translate(0, -((RenderParagraph)this.paragraph)._textPainter.preferredLineHeight));
                    _cachedBoundingBoxes = new List<global::Doroti.Ui.Rect> { rect__144261 };
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
                List<global::Doroti.Ui.TextBox> boxes__144586 = this.paragraph.getBoxesForSelection(new TextSelection(baseOffset: this.range.start, extentOffset: this.range.end), boxHeightStyle: BoxHeightStyle.max);
                if ((checked((long)(boxes__144586.Count)) != 0))
                {
                    global::Doroti.Ui.Rect result__144781 = boxes__144586.First().toRect();
                    for (var index__144829 = 1L; (index__144829 < checked((long)(boxes__144586.Count))); index__144829 += 1L)
                    {
                        result__144781 = result__144781.expandToInclude(boxes__144586[(int)(index__144829)].toRect());
                    }
                    _cachedRect = result__144781;
                }
                else
                {
                    global::Doroti.Ui.Offset offset__145018 = this.paragraph._getOffsetForPosition(new global::Doroti.Ui.TextPosition(offset: this.range.start));
                    _cachedRect = global::Doroti.Ui.Rect.fromPoints(offset__145018, offset__145018.translate(0, -((RenderParagraph)this.paragraph)._textPainter.preferredLineHeight));
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
            var selection__145712 = new TextSelection(baseOffset: this._textSelectionStart!.offset, extentOffset: this._textSelectionEnd!.offset);
            var selectionPaint__145858 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.style = PaintingStyle.fill;
    __cascade.color = ((RenderParagraph)this.paragraph).selectionColor!;
    return __cascade;
}))();
            foreach (global::Doroti.Ui.TextBox textBox__145990 in this.paragraph.getBoxesForSelection(selection__145712))
            {
                ((PaintingContext)context).canvas.drawRect(textBox__145990.toRect().shift(offset), selectionPaint__145858);
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
        global::Doroti.Ui.TextRange line__147048 = this.paragraph._getLineAtOffset(position);
        long start__147107 = line__147048.start.clamp(this.range.start, this.range.end);
        long end__147171 = line__147048.end.clamp(this.range.start, this.range.end);
        return new TextSelection(baseOffset: start__147107, extentOffset: end__147171);
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

