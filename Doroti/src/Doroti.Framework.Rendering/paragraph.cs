// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/paragraph.dart
using Doroti.Runtime;
using Doroti.Ui;

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
        return (__other is PlaceholderSpanIndexSemanticsTag) && (__other.index == index);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(typeof(PlaceholderSpanIndexSemanticsTag), index);
}

public class TextParentData : ParentData, ContainerParentDataMixin<RenderBox>
{
    internal virtual Offset? _offset { get; set; } = default;
    public virtual global::Doroti.Framework.Painting.PlaceholderSpan? span { get; set; } = default;
    public virtual RenderBox? previousSibling { get; set; } = default;
    public virtual RenderBox? nextSibling { get; set; } = default;

    public virtual global::Doroti.Ui.Offset? offset => _offset;
    public override void detach()
    {
        span = null;
        _offset = null;
        DartRuntimePrimitives.Assert(() => previousSibling is null);
        DartRuntimePrimitives.Assert(() => nextSibling is null);
        base.detach();
    }

    public override string ToString() => $"widget: {span}, {((offset is null) ? "not laid out" : $"offset: {offset}")}";
}

public interface RenderInlineChildrenContainerDefaults
{
    public void setupParentData(RenderObject child);
    public static global::Doroti.Framework.Painting.PlaceholderDimensions _layoutChild(RenderBox child, BoxConstraints childConstraints, Func<RenderBox, BoxConstraints, Size> layoutChild, Func<RenderBox, BoxConstraints, TextBaseline, double?> getBaseline)
    {
        var parentDataLocal = ((TextParentData?)child.parentData!)!;
        global::Doroti.Framework.Painting.PlaceholderSpan? spanLocal = parentDataLocal.span;
        DartRuntimePrimitives.Assert(() => spanLocal is not null);
        return (spanLocal is null) ? PlaceholderDimensions.empty : new global::Doroti.Framework.Painting.PlaceholderDimensions(size: layoutChild(child, childConstraints), alignment: spanLocal.alignment, baseline: spanLocal.baseline, baselineOffset: spanLocal.alignment switch { Dart_uiLibrary.PlaceholderAlignment.aboveBaseline or Dart_uiLibrary.PlaceholderAlignment.belowBaseline or Dart_uiLibrary.PlaceholderAlignment.bottom or Dart_uiLibrary.PlaceholderAlignment.middle => null, Dart_uiLibrary.PlaceholderAlignment.top => null, Dart_uiLibrary.PlaceholderAlignment.baseline => getBaseline(child, childConstraints, DartRuntimePrimitives.RequireValue(spanLocal.baseline)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
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
    internal static string _placeholderCharacter = char.ConvertFromUtf32(checked((int)PlaceholderSpan.placeholderCodeUnit));
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
        _softWrap = softWrap;
        _overflow = overflow;
        _devicePixelRatio = devicePixelRatio;
        _selectionColor = selectionColor;
        _textPainter = new global::Doroti.Framework.Painting.TextPainter(text: text, textAlign: textAlign, textDirection: textDirection, textScaler: Equals(textScaler, new _UnspecifiedTextScaler__paragraph()) ? TextScaler.CreateLinear(textScaleFactor) : textScaler, maxLines: maxLines, ellipsis: Equals(overflow, TextOverflow.ellipsis) ? ParagraphLibrary._kEllipsis : null, locale: locale, strutStyle: strutStyle, textWidthBasis: textWidthBasis, textHeightBehavior: textHeightBehavior);
        System.Diagnostics.Debug.Assert(text.debugAssertIsValid());
        System.Diagnostics.Debug.Assert((maxLines is null) || (DartRuntimePrimitives.RequireValue(maxLines) > 0L));
        System.Diagnostics.Debug.Assert(DartRuntimePrimitives.Identical(__textScaler, new _UnspecifiedTextScaler__paragraph()) || (textScaleFactor == 1.0));
    }

    internal virtual global::Doroti.Framework.Painting.TextPainter _textIntrinsics
    {
        get
        {
            return ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = _textIntrinsicsCache ??= new global::Doroti.Framework.Painting.TextPainter();
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
}))();
        }
    }
    public virtual global::Doroti.Framework.Painting.InlineSpan text
    {
        get => _textPainter.text!;
        set
        {
            var __value = value;
            switch (_textPainter.text!.compareTo(__value))
            {
                case RenderComparison.identical:
                    {
                        return;
                    }
                case RenderComparison.metadata:
                    {
                        _textPainter.text = __value;
                        _cachedCombinedSemanticsInfos = null;
                        markNeedsSemanticsUpdate();
                        break;
                    }
                case RenderComparison.paint:
                    {
                        _textPainter.text = __value;
                        _cachedAttributedLabels = null;
                        _cachedCombinedSemanticsInfos = null;
                        markNeedsPaint();
                        markNeedsSemanticsUpdate();
                        break;
                    }
                case RenderComparison.layout:
                    {
                        _textPainter.text = __value;
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
            if (_lastSelectableFragments is null)
            {
                return new List<TextSelection>();
            }
            var results = new List<TextSelection>();
            foreach (_SelectableFragment__paragraph fragment in _lastSelectableFragments!)
            {
                if ((fragment._textSelectionStart is not null) && (fragment._textSelectionEnd is not null))
                {
                    results.Add(new TextSelection(baseOffset: fragment._textSelectionStart!.offset, extentOffset: fragment._textSelectionEnd!.offset));
                }
            }
            return results;
        }
    }
    public virtual SelectionRegistrar? registrar
    {
        get => _registrar;
        set
        {
            var __value = value;
            if (Equals(__value, _registrar))
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
        if (_registrar is null)
        {
            return;
        }
        _lastSelectableFragments ??= _getSelectableFragments();
        _lastSelectableFragments!.forEach(_registrar!.add);
        if (checked((long)_lastSelectableFragments!.Count) != 0)
        {
            markNeedsCompositingBitsUpdate();
        }
    }

    internal virtual void _removeSelectionRegistrarSubscription()
    {
        if ((_registrar is null) || (_lastSelectableFragments is null))
        {
            return;
        }
        _lastSelectableFragments!.forEach(_registrar!.remove);
    }

    internal virtual List<_SelectableFragment__paragraph> _getSelectableFragments()
    {
        string plainText = text.toPlainText(includeSemanticsLabels: false);
        var result = new List<_SelectableFragment__paragraph>();
        var startLocal = 0L;
        while (startLocal < plainText.Length)
        {
            long endLocal = plainText.IndexOf(_placeholderCharacter, checked((int)startLocal));
            if (startLocal != endLocal)
            {
                if (endLocal == -1L)
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
        if (_lastSelectableFragments is null)
        {
            return false;
        }
        return _lastSelectableFragments!.Contains(selectable);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _disposeSelectableFragments()
    {
        if (_lastSelectableFragments is null)
        {
            return;
        }
        foreach (_SelectableFragment__paragraph fragment in _lastSelectableFragments!)
        {
            fragment.dispose();
        }
        _lastSelectableFragments = null;
    }

    public override bool alwaysNeedsCompositing => (((long?)(_lastSelectableFragments?.Count)) is { } __count19316 ? __count19316 != 0 : (bool?)null) ?? false;
    public override void markNeedsLayout()
    {
        _lastSelectableFragments?.forEach((element) => element.didChangeParagraphLayout());
        base.markNeedsLayout();
    }

    public override void dispose()
    {
        _removeSelectionRegistrarSubscription();
        _disposeSelectableFragments();
        _textPainter.dispose();
        _textIntrinsicsCache?.dispose();
        base.dispose();
    }

    public virtual global::Doroti.Ui.TextAlign textAlign
    {
        get => _textPainter.textAlign;
        set
        {
            var __value = value;
            if (Equals(_textPainter.textAlign, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _textPainter.textAlign = DartRuntimePrimitives.RequireValue(__value);
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => DartRuntimePrimitives.RequireValue(_textPainter.textDirection);
        set
        {
            var __value = value;
            if (Equals(_textPainter.textDirection, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _textPainter.textDirection = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual bool softWrap
    {
        get => _softWrap;
        set
        {
            var __value = value;
            if (_softWrap == DartRuntimePrimitives.RequireValue(__value))
            {
                return;
            }
            _softWrap = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.TextOverflow overflow
    {
        get => _overflow;
        set
        {
            var __value = value;
            if (Equals(_overflow, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _overflow = DartRuntimePrimitives.RequireValue(__value);
            _textPainter.ellipsis = Equals(DartRuntimePrimitives.RequireValue(__value), TextOverflow.ellipsis) ? ParagraphLibrary._kEllipsis : null;
            markNeedsLayout();
        }
    }
    public virtual double textScaleFactor
    {
        get => _textPainter.textScaleFactor;
        set
        {
            var __value = value;
            textScaler = TextScaler.CreateLinear(DartRuntimePrimitives.RequireValue(__value));
        }
    }
    public virtual global::Doroti.Framework.Painting.TextScaler textScaler
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
            _overflowShader = null;
            markNeedsLayout();
        }
    }
    public virtual double devicePixelRatio
    {
        get => _devicePixelRatio;
        set
        {
            var __value = value;
            if (_devicePixelRatio == DartRuntimePrimitives.RequireValue(__value))
            {
                return;
            }
            _devicePixelRatio = DartRuntimePrimitives.RequireValue(__value);
            if (Foundation.ConstantsLibrary.kIsWeb)
            {
                markNeedsPaint();
            }
        }
    }
    public virtual long? maxLines
    {
        get => _textPainter.maxLines;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is null) || (DartRuntimePrimitives.RequireValue(__value) > 0L));
            if (_textPainter.maxLines == __value)
            {
                return;
            }
            _textPainter.maxLines = __value;
            _overflowShader = null;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.Locale? locale
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
            _overflowShader = null;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.StrutStyle? strutStyle
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
            _overflowShader = null;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.TextWidthBasis textWidthBasis
    {
        get => _textPainter.textWidthBasis;
        set
        {
            var __value = value;
            if (Equals(_textPainter.textWidthBasis, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _textPainter.textWidthBasis = DartRuntimePrimitives.RequireValue(__value);
            _overflowShader = null;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextHeightBehavior? textHeightBehavior
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
            _overflowShader = null;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.Color? selectionColor
    {
        get => _selectionColor;
        set
        {
            var __value = value is null ? null : value;
            if (Equals(_selectionColor, __value))
            {
                return;
            }
            _selectionColor = __value;
            if (_lastSelectableFragments?.any((fragment) => fragment.value.hasSelection) ?? false)
            {
                markNeedsPaint();
            }
        }
    }
    internal virtual global::Doroti.Ui.Offset _getOffsetForPosition(TextPosition position)
    {
        return getOffsetForCaret(position, Rect.zero) + new global::Doroti.Ui.Offset(0, getFullHeightForCaret(position));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        List<global::Doroti.Framework.Painting.PlaceholderDimensions> placeholderDimensions = layoutInlineChildren(double.PositiveInfinity, (child, constraints) => new global::Doroti.Ui.Size(child.getMinIntrinsicWidth(double.PositiveInfinity), 0.0), ChildLayoutHelper.getDryBaseline);
        return ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = _textIntrinsics;
    __cascade.setPlaceholderDimensions(placeholderDimensions);
    __cascade.layout();
    return __cascade;
}))().minIntrinsicWidth;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        List<global::Doroti.Framework.Painting.PlaceholderDimensions> placeholderDimensions = layoutInlineChildren(double.PositiveInfinity, (child, constraints) => new global::Doroti.Ui.Size(child.getMaxIntrinsicWidth(double.PositiveInfinity), 0.0), ChildLayoutHelper.getDryBaseline);
        return ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = _textIntrinsics;
    __cascade.setPlaceholderDimensions(placeholderDimensions);
    __cascade.layout();
    return __cascade;
}))().maxIntrinsicWidth;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double preferredLineHeight => _textPainter.preferredLineHeight;
    internal virtual double _computeIntrinsicHeight(double width)
    {
        return ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = _textIntrinsics;
    __cascade.setPlaceholderDimensions(layoutInlineChildren(width, ChildLayoutHelper.dryLayoutChild, ChildLayoutHelper.getDryBaseline));
    __cascade.layout(minWidth: width, maxWidth: _adjustMaxWidth(width));
    return __cascade;
}))().height;
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
        global::Doroti.Ui.GlyphInfo? glyph = _textPainter.getClosestGlyphForOffset(position);
        global::Doroti.Framework.Painting.InlineSpan? spanHit = ((glyph is not null) && glyph.graphemeClusterLayoutBounds.contains(position)) ? _textPainter.text!.getSpanForPosition(new global::Doroti.Ui.TextPosition(offset: glyph.graphemeClusterCodeUnitRange.start)) : null;
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

    public virtual bool debugHasOverflowShader => _overflowShader is not null;
    public virtual void systemFontsDidChange()
    {
        markNeedsLayout();
        _textPainter.markNeedsLayout();
    }

    internal virtual double _adjustMaxWidth(double maxWidth)
    {
        return (softWrap || Equals(overflow, TextOverflow.ellipsis)) ? maxWidth : double.PositiveInfinity;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _layoutTextWithConstraints(BoxConstraints constraints)
    {
        ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = _textPainter;
    __cascade.setPlaceholderDimensions(_placeholderDimensions);
    __cascade.layout(minWidth: constraints.minWidth, maxWidth: _adjustMaxWidth(constraints.maxWidth));
    return __cascade;
}))();
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        global::Doroti.Ui.Size sizeLocal = ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = _textIntrinsics;
    __cascade.setPlaceholderDimensions(layoutInlineChildren(constraints.maxWidth, ChildLayoutHelper.dryLayoutChild, ChildLayoutHelper.getDryBaseline));
    __cascade.layout(minWidth: constraints.minWidth, maxWidth: _adjustMaxWidth(constraints.maxWidth));
    return __cascade;
}))().size;
        return constraints.constrain(sizeLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        DartRuntimePrimitives.Assert(() => constraints.debugAssertIsValid());
        _layoutTextWithConstraints(constraints);
        return _textPainter.computeDistanceToActualBaseline(TextBaseline.alphabetic);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => constraints.debugAssertIsValid());
        ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = _textIntrinsics;
    __cascade.setPlaceholderDimensions(layoutInlineChildren(constraints.maxWidth, ChildLayoutHelper.dryLayoutChild, ChildLayoutHelper.getDryBaseline));
    __cascade.layout(minWidth: constraints.minWidth, maxWidth: _adjustMaxWidth(constraints.maxWidth));
    return __cascade;
}))();
        return _textIntrinsics.computeDistanceToActualBaseline(TextBaseline.alphabetic);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        _lastSelectableFragments?.forEach((element) => element.didChangeParagraphLayout());
        BoxConstraints constraintsLocal = constraints;
        _placeholderDimensions = layoutInlineChildren(constraintsLocal.maxWidth, ChildLayoutHelper.layoutChild, ChildLayoutHelper.getBaseline);
        _layoutTextWithConstraints(constraintsLocal);
        positionInlineChildren(_textPainter.inlinePlaceholderBoxes!);
        global::Doroti.Ui.Size textSize = _textPainter.size;
        size = constraintsLocal.constrain(textSize);
        bool didOverflowHeight = (size.height < textSize.height) || _textPainter.didExceedMaxLines;
        bool didOverflowWidth = size.width < textSize.width;
        bool hasVisualOverflow = didOverflowWidth || didOverflowHeight;
        if (hasVisualOverflow)
        {
            switch (_overflow)
            {
                case TextOverflow.visible:
                    {
                        _needsClipping = false;
                        _overflowShader = null;
                        break;
                    }
                case TextOverflow.clip:
                case TextOverflow.ellipsis:
                    {
                        _needsClipping = true;
                        _overflowShader = null;
                        break;
                    }
                case TextOverflow.fade:
                    {
                        _needsClipping = true;
                        var fadeSizePainter = ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = new global::Doroti.Framework.Painting.TextPainter(text: new global::Doroti.Framework.Painting.TextSpan(style: _textPainter.text!.style, text: "…"), textDirection: textDirection, textScaler: textScaler, locale: locale);
    __cascade.layout();
    return __cascade;
}))();
                        if (didOverflowWidth)
                        {
                            var (fadeStart, fadeEnd) = textDirection switch { TextDirection.rtl => (fadeSizePainter.width, 0.0), TextDirection.ltr => (size.width - fadeSizePainter.width, size.width), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
                            _overflowShader = Ui.Gradient.linear(new global::Doroti.Ui.Offset(fadeStart, 0.0), new global::Doroti.Ui.Offset(fadeEnd, 0.0), new List<global::Doroti.Ui.Color> { new global::Doroti.Ui.Color(4294967295L), new global::Doroti.Ui.Color(16777215L) });
                        }
                        else
                        {
                            double fadeEndLocal = size.height;
                            double fadeStartLocal = fadeEndLocal - (fadeSizePainter.height / 2.0);
                            _overflowShader = Ui.Gradient.linear(new global::Doroti.Ui.Offset(0.0, fadeStartLocal), new global::Doroti.Ui.Offset(0.0, fadeEndLocal), new List<global::Doroti.Ui.Color> { new global::Doroti.Ui.Color(4294967295L), new global::Doroti.Ui.Color(16777215L) });
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
        var __child = (RenderBox)child;
        defaultApplyPaintTransform(__child, transform);
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        _layoutTextWithConstraints(constraints);
        DartRuntimePrimitives.Assert(() =>
            {
                if (DebugLibrary.debugRepaintTextRainbowEnabled)
                {
                    var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = DebugLibrary.debugCurrentRepaintColor.toColor();
    return __cascade;
}))();
                    context.canvas.drawRect(offset & size, paintLocal);
                }
                return true;
            });
        if (_lastSelectableFragments is not null)
        {
            if (_needsClipping)
            {
                context.canvas.save();
                context.canvas.clipRect(offset & size);
            }
            foreach (_SelectableFragment__paragraph fragment in _lastSelectableFragments!)
            {
                fragment.paintSelection(context, offset);
            }
            if (_needsClipping)
            {
                context.canvas.restore();
            }
        }
        if (_needsClipping)
        {
            global::Doroti.Ui.Rect bounds = offset & size;
            if (_overflowShader is not null)
            {
                context.canvas.saveLayer(bounds, new global::Doroti.Ui.Paint());
            }
            else
            {
                context.canvas.save();
            }
            context.canvas.clipRect(bounds);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                _textPainter.debugPaintTextLayoutBoxes = DebugLibrary.debugPaintTextLayoutBoxes;
                return true;
            });
        _textPainter.paint(context.canvas, offset);
        paintInlineChildren(context, offset);
        if (_needsClipping)
        {
            if (_overflowShader is not null)
            {
                context.canvas.translate(offset.dx, offset.dy);
                var paintAlternate = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.blendMode = BlendMode.modulate;
    __cascade.shader = _overflowShader;
    return __cascade;
}))();
                context.canvas.drawRect(Offset.zero & size, paintAlternate);
            }
            context.canvas.restore();
        }
        if (_lastSelectableFragments is not null)
        {
            foreach (_SelectableFragment__paragraph fragmentLocal in _lastSelectableFragments!)
            {
                fragmentLocal.paintHandles(context, offset);
            }
        }
    }

    public virtual global::Doroti.Ui.Offset getOffsetForCaret(TextPosition position, Rect caretPrototype)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        _layoutTextWithConstraints(constraints);
        return _textPainter.getOffsetForCaret(position, caretPrototype);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getFullHeightForCaret(TextPosition position)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        _layoutTextWithConstraints(constraints);
        return _textPainter.getFullHeightForCaret(position, Rect.zero);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<global::Doroti.Ui.TextBox> getBoxesForSelection(TextSelection selection, BoxHeightStyle boxHeightStyle = BoxHeightStyle.tight, BoxWidthStyle boxWidthStyle = BoxWidthStyle.tight)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        _layoutTextWithConstraints(constraints);
        return _textPainter.getBoxesForSelection(selection, boxHeightStyle: boxHeightStyle, boxWidthStyle: boxWidthStyle);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.TextPosition getPositionForOffset(Offset offset)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        _layoutTextWithConstraints(constraints);
        return _textPainter.getPositionForOffset(offset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.TextRange getWordBoundary(TextPosition position)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        _layoutTextWithConstraints(constraints);
        return _textPainter.getWordBoundary(position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextRange _getLineAtOffset(TextPosition position) => _textPainter.getLineBoundary(position);
    internal virtual global::Doroti.Ui.TextPosition _getTextPositionAbove(TextPosition position)
    {
        double preferredLineHeightLocal = _textPainter.preferredLineHeight;
        double verticalOffset = -0.5 * preferredLineHeightLocal;
        return _getTextPositionVertical(position, verticalOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _getTextPositionBelow(TextPosition position)
    {
        double preferredLineHeightLocal = _textPainter.preferredLineHeight;
        double verticalOffset = 1.5 * preferredLineHeightLocal;
        return _getTextPositionVertical(position, verticalOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _getTextPositionVertical(TextPosition position, double verticalOffset)
    {
        global::Doroti.Ui.Offset caretOffset = _textPainter.getOffsetForCaret(position, Rect.zero);
        global::Doroti.Ui.Offset caretOffsetTranslated = caretOffset.translate(0.0, verticalOffset);
        return _textPainter.getPositionForOffset(caretOffsetTranslated);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Size textSize
    {
        get
        {
            DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
            return _textPainter.size;
        }
    }
    public virtual bool didExceedMaxLines
    {
        get
        {
            DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
            return _textPainter.didExceedMaxLines;
        }
    }
    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        _semanticsInfo = text.getSemanticsInformation();
        var needsAssembleSemanticsNode = false;
        var needsChildConfigurationsDelegate = false;
        foreach (global::Doroti.Framework.Painting.InlineSpanSemanticsInformation info in _semanticsInfo!)
        {
            if ((info.recognizer is not null) || (info.semanticsIdentifier is not null))
            {
                needsAssembleSemanticsNode = true;
                break;
            }
            needsChildConfigurationsDelegate = needsChildConfigurationsDelegate || info.isPlaceholder;
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
                config.childConfigurationsDelegate = _childSemanticsConfigurationsDelegate;
            }
            else
            {
                if (_cachedAttributedLabels is null)
                {
                    var buffer = new StringBuffer();
                    var offset = 0L;
                    var attributesLocal = new List<global::Doroti.Ui.StringAttribute>();
                    foreach (global::Doroti.Framework.Painting.InlineSpanSemanticsInformation infoLocal in _semanticsInfo!)
                    {
                        string label = infoLocal.semanticsLabel ?? infoLocal.text;
                        foreach (global::Doroti.Ui.StringAttribute infoAttribute in infoLocal.stringAttributes)
                        {
                            global::Doroti.Ui.TextRange originalRange = infoAttribute.range;
                            attributesLocal.Add(infoAttribute.copy(range: new global::Doroti.Ui.TextRange(start: offset + originalRange.start, end: offset + originalRange.end)));
                        }
                        buffer.write(label);
                        offset += label.Length;
                    }
                    _cachedAttributedLabels = new List<global::Doroti.Framework.Semantics.AttributedString> { new global::Doroti.Framework.Semantics.AttributedString(buffer.ToString(), attributes: attributesLocal) };
                }
                config.attributedLabel = _cachedAttributedLabels![(int)0L];
                config.textDirection = textDirection;
            }
        }
    }

    internal virtual global::Doroti.Framework.Semantics.ChildSemanticsConfigurationsResult _childSemanticsConfigurationsDelegate(List<global::Doroti.Framework.Semantics.SemanticsConfiguration> childConfigs)
    {
        var builder = new global::Doroti.Framework.Semantics.ChildSemanticsConfigurationsResultBuilder();
        var placeholderIndex = 0L;
        var childConfigsIndex = 0L;
        var attributedLabelCacheIndex = 0L;
        _cachedCombinedSemanticsInfos ??= Inline_spanLibrary.combineSemanticsInfo(_semanticsInfo!);
        foreach (global::Doroti.Framework.Painting.InlineSpanSemanticsInformation info in _cachedCombinedSemanticsInfos!)
        {
            if (info.isPlaceholder)
            {
                while ((childConfigsIndex < checked(childConfigs.Count)) && _childConfigBelongsToPlaceholder(childConfigs[(int)childConfigsIndex], placeholderIndex))
                {
                    builder.markAsMergeUp(childConfigs[(int)childConfigsIndex]);
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
        IEnumerable<global::Doroti.Framework.Semantics.SemanticsTag>? tags = childConfig.tagsForChildren;
        if (tags is null)
        {
            return false;
        }
        foreach (global::Doroti.Framework.Semantics.SemanticsTag tag in tags)
        {
            if (tag is PlaceholderSpanIndexSemanticsTag)
            {
                PlaceholderSpanIndexSemanticsTag tag__45698__as45723 = (PlaceholderSpanIndexSemanticsTag)tag;
                return tag__45698__as45723.index == placeholderIndex;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Semantics.SemanticsConfiguration _createSemanticsConfigForTextInfo(global::Doroti.Framework.Painting.InlineSpanSemanticsInformation textInfo, long cacheIndex)
    {
        DartRuntimePrimitives.Assert(() => !textInfo.requiresOwnNode);
        List<global::Doroti.Framework.Semantics.AttributedString> cachedStrings = _cachedAttributedLabels ??= new List<global::Doroti.Framework.Semantics.AttributedString>();
        DartRuntimePrimitives.Assert(() => cacheIndex <= checked(cachedStrings.Count));
        bool hasCache = cacheIndex < checked(cachedStrings.Count);
        global::Doroti.Framework.Semantics.AttributedString attributedLabelLocal = default!;
        if (hasCache)
        {
            attributedLabelLocal = cachedStrings[(int)cacheIndex];
        }
        else
        {
            DartRuntimePrimitives.Assert(() => checked(cachedStrings.Count) == cacheIndex);
            attributedLabelLocal = new global::Doroti.Framework.Semantics.AttributedString(textInfo.semanticsLabel ?? textInfo.text, attributes: textInfo.stringAttributes);
            cachedStrings.Add(attributedLabelLocal);
        }
        return ((Func<global::Doroti.Framework.Semantics.SemanticsConfiguration>)(() =>
{
    var __cascade = new global::Doroti.Framework.Semantics.SemanticsConfiguration();
    __cascade.textDirection = textDirection;
    __cascade.attributedLabel = attributedLabelLocal;
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void assembleSemanticsNode(global::Doroti.Framework.Semantics.SemanticsNode node, global::Doroti.Framework.Semantics.SemanticsConfiguration config, IEnumerable<global::Doroti.Framework.Semantics.SemanticsNode> children)
    {
        DartRuntimePrimitives.Assert(() => (_semanticsInfo is not null) && (checked((long)_semanticsInfo!.Count) != 0));
        var newChildren = new List<global::Doroti.Framework.Semantics.SemanticsNode>();
        global::Doroti.Ui.TextDirection currentDirection = textDirection;
        global::Doroti.Ui.Rect currentRect = default!;
        var ordinal = 0.0;
        var start = 0L;
        var placeholderIndex = 0L;
        var childIndex = 0L;
        RenderBox? child = firstChild;
        var newChildCache = new DartMap<Key, global::Doroti.Framework.Semantics.SemanticsNode>();
        _cachedCombinedSemanticsInfos ??= Inline_spanLibrary.combineSemanticsInfo(_semanticsInfo!);
        foreach (global::Doroti.Framework.Painting.InlineSpanSemanticsInformation info in _cachedCombinedSemanticsInfos!)
        {
            var selection = new TextSelection(baseOffset: start, extentOffset: start + info.text.Length);
            start += info.text.Length;
            if (info.isPlaceholder)
            {
                while ((children.Count() > childIndex) && children.elementAt(childIndex).isTagged(new PlaceholderSpanIndexSemanticsTag(placeholderIndex)))
                {
                    global::Doroti.Framework.Semantics.SemanticsNode childNode = children.elementAt(childIndex);
                    var parentDataLocal = ((TextParentData?)child!.parentData!)!;
                    if (parentDataLocal.offset is not null)
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
                if (checked((long)rects.Count) == 0)
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
                rectLocal = Rect.fromLTWH(Math.Max(0.0, rectLocal.left), Math.Max(0.0, rectLocal.top), Math.Min(rectLocal.width, constraints.maxWidth), Math.Min(rectLocal.height, constraints.maxHeight));
                currentRect = Rect.fromLTRB(rectLocal.left.floorToDouble() - 4.0, rectLocal.top.floorToDouble() - 4.0, rectLocal.right.ceilToDouble() + 4.0, rectLocal.bottom.ceilToDouble() + 4.0);
                var configuration = ((Func<global::Doroti.Framework.Semantics.SemanticsConfiguration>)(() =>
{
    var __cascade = new global::Doroti.Framework.Semantics.SemanticsConfiguration();
    __cascade.sortKey = new global::Doroti.Framework.Semantics.OrdinalSortKey(ordinal++);
    __cascade.textDirection = initialDirection;
    __cascade.identifier = info.semanticsIdentifier ?? "";
    __cascade.attributedLabel = new global::Doroti.Framework.Semantics.AttributedString(info.semanticsLabel ?? info.text, attributes: info.stringAttributes);
    return __cascade;
}))();
                switch (info.recognizer)
                {
                    case TapGestureRecognizer { onTap: Action handler } __object50228:
                        {
                            if (handler is not null)
                            {
                                configuration.onTap = handler;
                                configuration.isLink = true;
                            }
                            break;
                        }
                    case DoubleTapGestureRecognizer { onDoubleTap: Action handlerLocal } __object50301:
                        {
                            if (handlerLocal is not null)
                            {
                                configuration.onTap = handlerLocal;
                                configuration.isLink = true;
                            }
                            break;
                        }
                    case LongPressGestureRecognizer { onLongPress: Action onLongPressLocal } __object50523:
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
                    global::Doroti.Ui.Rect paintRect = DartRuntimePrimitives.RequireValue(node.parentPaintClipRect).intersect(currentRect);
                    configuration.isHidden = paintRect.isEmpty && !currentRect.isEmpty;
                }
                global::Doroti.Framework.Semantics.SemanticsNode newChild = default!;
                if ((((long?)(_cachedChildNodes?.Count)) is { } __count51134 ? __count51134 != 0 : (bool?)null) ?? false)
                {
                    newChild = _cachedChildNodes!.remove(_cachedChildNodes!.Keys.First())!;
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
                newChildCache[newChild.key!] = newChild;
                newChildren.Add(newChild);
            }
        }
        DartRuntimePrimitives.Assert(() => childIndex == children.Count());
        DartRuntimePrimitives.Assert(() => child is null);
        _cachedChildNodes = newChildCache.cast<Key, global::Doroti.Framework.Semantics.SemanticsNode>();
        node.updateWith(config: config, childrenInInversePaintOrder: newChildren);
    }

    internal virtual Action? _createShowOnScreenFor(Key key)
    {
        return () =>
        {
            global::Doroti.Framework.Semantics.SemanticsNode node = _cachedChildNodes!.GetValueOrDefault(key)!;
            showOnScreen(descendant: this, rect: node.rect);
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void clearSemantics()
    {
        base.clearSemantics();
        _cachedChildNodes = null;
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        return new List<DiagnosticsNode> { ((Diagnosticable)text).toDiagnosticsNode(name: "text", style: DiagnosticsTreeStyle.transition) };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<global::Doroti.Ui.TextAlign>("textAlign", textAlign));
        properties.add(new EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", textDirection));
        properties.add(new FlagProperty("softWrap", value: softWrap, ifTrue: "wrapping at box width", ifFalse: "no wrapping except at line break characters", showName: true));
        properties.add(new EnumProperty<global::Doroti.Framework.Painting.TextOverflow>("overflow", overflow));
        properties.add(new DiagnosticsProperty<global::Doroti.Framework.Painting.TextScaler>("textScaler", textScaler, defaultValue: TextScaler.noScaling));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Locale>("locale", locale, defaultValue: null));
        properties.add(new IntProperty("maxLines", maxLines, ifNull: "unlimited"));
        properties.add(new DoubleProperty("devicePixelRatio", devicePixelRatio, defaultValue: 1.0));
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
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long childCount => _childCount;
    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (child is not RenderBox)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {GetType()} that expected a {typeof(RenderBox)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", child.debugCreator, style: DiagnosticsTreeStyle.errorProperty) });
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
            DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(after, equals: _firstChild));
            DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(after, equals: _lastChild));
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
                var childPreviousSiblingParentData = ((TextParentData?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((TextParentData?)childParentData.nextSibling!.parentData!)!;
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
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: _firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: _lastChild));
        DartRuntimePrimitives.Assert(() => _childCount >= 0L);
        if (childParentData.previousSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_firstChild, child));
            _firstChild = childParentData.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData = ((TextParentData?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if (childParentData.nextSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_lastChild, child));
            _lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((TextParentData?)childParentData.nextSibling!.parentData!)!;
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
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            child.detach();
            var childParentData = ((TextParentData?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void redepthChildren()
    {
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
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            visitor(child);
            var childParentData = ((TextParentData?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => _firstChild;
    public virtual RenderBox? lastChild => _lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((TextParentData?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((TextParentData?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void setupParentData(RenderObject child)
    {
        var __child = (RenderBox)child;
        if (__child.parentData is not TextParentData)
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
            if (child is null)
            {
                DartRuntimePrimitives.Assert(() =>
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Invalid number of boxes provided to positionInlineChildren."), new ErrorDescription($"The number of boxes ({checked((long)boxes.Count)}) exceeds the number of child render objects ({childCount}). " + "Each box corresponds to a child, but there are not enough children to position all boxes."), new ErrorHint("This error typically occurs when a custom InlineSpan implementation returns a list of boxes " + "that is longer than the number of inline children. Ensure that the number of boxes returned " + "by `computeLineMetrics` or similar methods does not exceed the number of children."), new DiagnosticsProperty<RenderObject>("The RenderParagraph receiving the boxes", this, style: DiagnosticsTreeStyle.errorProperty) });
                    });
                return;
            }
            var textParentData = ((TextParentData?)child.parentData!)!;
            textParentData._offset = new global::Doroti.Ui.Offset(box.left, box.top);
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
        global::Doroti.Ui.Offset? offsetLocal = childParentData.offset;
        if (offsetLocal is null)
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
        while (child is not null)
        {
            var childParentData = ((TextParentData?)child.parentData!)!;
            global::Doroti.Ui.Offset? childOffset = childParentData.offset;
            if (childOffset is null)
            {
                return;
            }
            context.paintChild(child, DartRuntimePrimitives.RequireValue(childOffset) + offset);
            child = childAfter(child);
        }
    }

    public virtual bool hitTestInlineChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((TextParentData?)child.parentData!)!;
            global::Doroti.Ui.Offset? childOffset = childParentData.offset;
            if (childOffset is null)
            {
                return false;
            }
            bool isHit = result.addWithPaintOffset(offset: DartRuntimePrimitives.RequireValue(childOffset), position: position, hitTest: (result, transformed) => child!.hitTest(result, position: transformed));
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
        if (_hasPendingSystemFontsDidChangeCallBack)
        {
            return;
        }
        _hasPendingSystemFontsDidChangeCallBack = true;
        SchedulerBinding.instance.scheduleFrameCallback((timeStamp) =>
        {
            DartRuntimePrimitives.Assert(() => _hasPendingSystemFontsDidChangeCallBack);
            _hasPendingSystemFontsDidChangeCallBack = false;
            DartRuntimePrimitives.Assert(() => attached || (debugDisposed ?? true));
            if (attached)
            {
                systemFontsDidChange();
            }
        });
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
    internal static string _placeholderCharacter = char.ConvertFromUtf32(checked((int)PlaceholderSpan.placeholderCodeUnit));
    internal static long _placeholderLength = _placeholderCharacter.Length;
    internal virtual List<Rect>? _cachedBoundingBoxes { get; set; } = default;
    internal virtual Rect? _cachedRect { get; set; } = default;

    internal _SelectableFragment__paragraph(RenderParagraph paragraph, string fullText, TextRange range)
    {
        this.paragraph = paragraph;
        this.fullText = fullText;
        this.range = range;
        System.Diagnostics.Debug.Assert(range.isValid && !range.isCollapsed && range.isNormalized);
    }

    public virtual SelectionGeometry value => _selectionGeometry;
    internal virtual void _updateSelectionGeometry()
    {
        SelectionGeometry newValue = _getSelectionGeometry();
        if (Equals(_selectionGeometry, newValue))
        {
            return;
        }
        _selectionGeometry = newValue;
        notifyListeners();
    }

    internal virtual SelectionGeometry _getSelectionGeometry()
    {
        if ((_textSelectionStart is null) || (_textSelectionEnd is null))
        {
            return new SelectionGeometry(status: SelectionStatus.none, hasContent: true);
        }
        long selectionStart = _textSelectionStart!.offset;
        long selectionEnd = _textSelectionEnd!.offset;
        bool isReversed = selectionStart > selectionEnd;
        global::Doroti.Ui.Offset startOffsetInParagraphCoordinates = paragraph._getOffsetForPosition(_textSelectionStart!);
        global::Doroti.Ui.Offset endOffsetInParagraphCoordinates = (selectionStart == selectionEnd) ? startOffsetInParagraphCoordinates : paragraph._getOffsetForPosition(_textSelectionEnd!);
        var flipHandles = isReversed != Equals(TextDirection.rtl, paragraph.textDirection);
        var selection = new TextSelection(baseOffset: selectionStart, extentOffset: selectionEnd);
        var selectionRectsLocal = new List<global::Doroti.Ui.Rect>();
        foreach (global::Doroti.Ui.TextBox textBox in paragraph.getBoxesForSelection(selection))
        {
            selectionRectsLocal.Add(textBox.toRect());
        }
        var selectionCollapsed = selectionStart == selectionEnd;
        var (startSelectionHandleType, endSelectionHandleType) = (selectionCollapsed, flipHandles) switch { (true, _) => (TextSelectionHandleType.collapsed, TextSelectionHandleType.collapsed), (false, true) => (TextSelectionHandleType.right, TextSelectionHandleType.left), (false, false) => (TextSelectionHandleType.left, TextSelectionHandleType.right) };
        return new SelectionGeometry(startSelectionPoint: new SelectionPoint(localPosition: startOffsetInParagraphCoordinates, lineHeight: paragraph._textPainter.preferredLineHeight, handleType: startSelectionHandleType), endSelectionPoint: new SelectionPoint(localPosition: endOffsetInParagraphCoordinates, lineHeight: paragraph._textPainter.preferredLineHeight, handleType: endSelectionHandleType), selectionRects: selectionRectsLocal, status: selectionCollapsed ? SelectionStatus.collapsed : SelectionStatus.uncollapsed, hasContent: true);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SelectionResult dispatchSelectionEvent(SelectionEvent @event)
    {
        SelectionResult result = default!;
        global::Doroti.Ui.TextPosition? existingSelectionStart = _textSelectionStart;
        global::Doroti.Ui.TextPosition? existingSelectionEnd = _textSelectionEnd;
        switch (@event.type)
        {
            case SelectionEventType.startEdgeUpdate:
            case SelectionEventType.endEdgeUpdate:
                {
                    var edgeUpdate = ((SelectionEdgeUpdateEvent?)@event)!;
                    TextGranularity granularityLocal = ((SelectionEdgeUpdateEvent)@event).granularity;
                    switch (granularityLocal)
                    {
                        case TextGranularity.character:
                            {
                                result = _updateSelectionEdge(edgeUpdate.globalPosition, isEnd: Equals(edgeUpdate.type, SelectionEventType.endEdgeUpdate));
                                break;
                            }
                        case TextGranularity.word:
                            {
                                result = _updateSelectionEdgeByTextBoundary(edgeUpdate.globalPosition, isEnd: Equals(edgeUpdate.type, SelectionEventType.endEdgeUpdate), getTextBoundary: _getWordBoundaryAtPosition);
                                break;
                            }
                        case TextGranularity.paragraph:
                            {
                                result = _updateSelectionEdgeByMultiSelectableTextBoundary(edgeUpdate.globalPosition, isEnd: Equals(edgeUpdate.type, SelectionEventType.endEdgeUpdate), getTextBoundary: _getParagraphBoundaryAtPosition, getClampedTextBoundary: _getClampedParagraphBoundaryAtPosition);
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
                    var selectWordLocal = ((SelectWordSelectionEvent?)@event)!;
                    result = _handleSelectWord(selectWordLocal.globalPosition);
                    break;
                }
            case SelectionEventType.selectParagraph:
                {
                    var selectParagraphLocal = ((SelectParagraphSelectionEvent?)@event)!;
                    if (selectParagraphLocal.absorb)
                    {
                        _handleSelectAll();
                        result = SelectionResult.next;
                        _selectableContainsOriginTextBoundary = true;
                    }
                    else
                    {
                        result = _handleSelectParagraph(selectParagraphLocal.globalPosition);
                    }
                    break;
                }
            case SelectionEventType.granularlyExtendSelection:
                {
                    var granularlyExtendSelectionLocal = ((GranularlyExtendSelectionEvent?)@event)!;
                    result = _handleGranularlyExtendSelection(granularlyExtendSelectionLocal.forward, granularlyExtendSelectionLocal.isEnd, granularlyExtendSelectionLocal.granularity);
                    break;
                }
            case SelectionEventType.directionallyExtendSelection:
                {
                    var directionallyExtendSelectionLocal = ((DirectionallyExtendSelectionEvent?)@event)!;
                    result = _handleDirectionallyExtendSelection(directionallyExtendSelectionLocal.dx, directionallyExtendSelectionLocal.isEnd, directionallyExtendSelectionLocal.direction);
                    break;
                }
        }
        if ((!Equals(existingSelectionStart, _textSelectionStart)) || (!Equals(existingSelectionEnd, _textSelectionEnd)))
        {
            _didChangeSelection();
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SelectedContent? getSelectedContent()
    {
        if ((_textSelectionStart is null) || (_textSelectionEnd is null))
        {
            return null;
        }
        long start = Math.Min(_textSelectionStart!.offset, _textSelectionEnd!.offset);
        long end = Math.Max(_textSelectionStart!.offset, _textSelectionEnd!.offset);
        return new SelectedContent(plainText: fullText.substring(start, end));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SelectedContentRange? getSelection()
    {
        if ((_textSelectionStart is null) || (_textSelectionEnd is null))
        {
            return null;
        }
        return new SelectedContentRange(startOffset: _textSelectionStart!.offset, endOffset: _textSelectionEnd!.offset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _didChangeSelection()
    {
        paragraph.markNeedsPaint();
        _updateSelectionGeometry();
    }

    internal virtual global::Doroti.Ui.TextPosition _updateSelectionStartEdgeByTextBoundary((TextPosition boundaryEnd, TextPosition boundaryStart)? textBoundary, Func<TextPosition, (TextPosition boundaryEnd, TextPosition boundaryStart)> getTextBoundary, TextPosition position, TextPosition? existingSelectionStart, TextPosition? existingSelectionEnd)
    {
        global::Doroti.Ui.TextPosition? targetPosition = default!;
        if (textBoundary is not null)
        {
            (TextPosition boundaryEnd, TextPosition boundaryStart) textBoundary__value61232 = DartRuntimePrimitives.RequireValue(textBoundary);
            DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.RequireValue(textBoundary__value61232).boundaryStart.offset >= range.start) && (DartRuntimePrimitives.RequireValue(textBoundary__value61232).boundaryEnd.offset <= range.end));
            if (_selectableContainsOriginTextBoundary && (existingSelectionStart is not null) && (existingSelectionEnd is not null))
            {
                var isSamePosition = position.offset == existingSelectionEnd.offset;
                bool isSelectionInverted = existingSelectionStart.offset > existingSelectionEnd.offset;
                bool shouldSwapEdges = !isSamePosition && isSelectionInverted != position.offset > existingSelectionEnd.offset;
                if (shouldSwapEdges)
                {
                    if (position.offset < existingSelectionEnd.offset)
                    {
                        targetPosition = DartRuntimePrimitives.RequireValue(textBoundary__value61232).boundaryStart;
                    }
                    else
                    {
                        targetPosition = DartRuntimePrimitives.RequireValue(textBoundary__value61232).boundaryEnd;
                    }
                    (TextPosition boundaryEnd, TextPosition boundaryStart) localTextBoundary = getTextBoundary(existingSelectionEnd);
                    DartRuntimePrimitives.Assert(() => (localTextBoundary.boundaryStart.offset >= range.start) && (localTextBoundary.boundaryEnd.offset <= range.end));
                    _setSelectionPosition((existingSelectionEnd.offset == localTextBoundary.boundaryStart.offset) ? localTextBoundary.boundaryEnd : localTextBoundary.boundaryStart, isEnd: true);
                }
                else
                {
                    if (position.offset < existingSelectionEnd.offset)
                    {
                        targetPosition = DartRuntimePrimitives.RequireValue(textBoundary__value61232).boundaryStart;
                    }
                    else
                    {
                        if (position.offset > existingSelectionEnd.offset)
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
                if (existingSelectionEnd is not null)
                {
                    if (position.offset < existingSelectionEnd.offset)
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
            if (_selectableContainsOriginTextBoundary && (existingSelectionStart is not null) && (existingSelectionEnd is not null))
            {
                var isSamePositionLocal = position.offset == existingSelectionEnd.offset;
                bool isSelectionInvertedLocal = existingSelectionStart.offset > existingSelectionEnd.offset;
                bool shouldSwapEdgesLocal = !isSamePositionLocal && isSelectionInvertedLocal != position.offset > existingSelectionEnd.offset;
                if (shouldSwapEdgesLocal)
                {
                    (TextPosition boundaryEnd, TextPosition boundaryStart) localTextBoundaryLocal = getTextBoundary(existingSelectionEnd);
                    DartRuntimePrimitives.Assert(() => (localTextBoundaryLocal.boundaryStart.offset >= range.start) && (localTextBoundaryLocal.boundaryEnd.offset <= range.end));
                    _setSelectionPosition(isSelectionInvertedLocal ? localTextBoundaryLocal.boundaryEnd : localTextBoundaryLocal.boundaryStart, isEnd: true);
                }
            }
        }
        return targetPosition ?? position;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _updateSelectionEndEdgeByTextBoundary((TextPosition boundaryEnd, TextPosition boundaryStart)? textBoundary, Func<TextPosition, (TextPosition boundaryEnd, TextPosition boundaryStart)> getTextBoundary, TextPosition position, TextPosition? existingSelectionStart, TextPosition? existingSelectionEnd)
    {
        global::Doroti.Ui.TextPosition? targetPosition = default!;
        if (textBoundary is not null)
        {
            (TextPosition boundaryEnd, TextPosition boundaryStart) textBoundary__value65725 = DartRuntimePrimitives.RequireValue(textBoundary);
            DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.RequireValue(textBoundary__value65725).boundaryStart.offset >= range.start) && (DartRuntimePrimitives.RequireValue(textBoundary__value65725).boundaryEnd.offset <= range.end));
            if (_selectableContainsOriginTextBoundary && (existingSelectionStart is not null) && (existingSelectionEnd is not null))
            {
                var isSamePosition = position.offset == existingSelectionStart.offset;
                bool isSelectionInverted = existingSelectionStart.offset > existingSelectionEnd.offset;
                bool shouldSwapEdges = !isSamePosition && isSelectionInverted != position.offset < existingSelectionStart.offset;
                if (shouldSwapEdges)
                {
                    if (position.offset < existingSelectionStart.offset)
                    {
                        targetPosition = DartRuntimePrimitives.RequireValue(textBoundary__value65725).boundaryStart;
                    }
                    else
                    {
                        targetPosition = DartRuntimePrimitives.RequireValue(textBoundary__value65725).boundaryEnd;
                    }
                    (TextPosition boundaryEnd, TextPosition boundaryStart) localTextBoundary = getTextBoundary(existingSelectionStart);
                    DartRuntimePrimitives.Assert(() => (localTextBoundary.boundaryStart.offset >= range.start) && (localTextBoundary.boundaryEnd.offset <= range.end));
                    _setSelectionPosition((existingSelectionStart.offset == localTextBoundary.boundaryStart.offset) ? localTextBoundary.boundaryEnd : localTextBoundary.boundaryStart, isEnd: false);
                }
                else
                {
                    if (position.offset < existingSelectionStart.offset)
                    {
                        targetPosition = DartRuntimePrimitives.RequireValue(textBoundary__value65725).boundaryStart;
                    }
                    else
                    {
                        if (position.offset > existingSelectionStart.offset)
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
                if (existingSelectionStart is not null)
                {
                    if (position.offset < existingSelectionStart.offset)
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
            if (_selectableContainsOriginTextBoundary && (existingSelectionStart is not null) && (existingSelectionEnd is not null))
            {
                var isSamePositionLocal = position.offset == existingSelectionStart.offset;
                bool isSelectionInvertedLocal = existingSelectionStart.offset > existingSelectionEnd.offset;
                bool shouldSwapEdgesLocal = (isSelectionInvertedLocal != position.offset < existingSelectionStart.offset) || isSamePositionLocal;
                if (shouldSwapEdgesLocal)
                {
                    (TextPosition boundaryEnd, TextPosition boundaryStart) localTextBoundaryLocal = getTextBoundary(existingSelectionStart);
                    DartRuntimePrimitives.Assert(() => (localTextBoundaryLocal.boundaryStart.offset >= range.start) && (localTextBoundaryLocal.boundaryEnd.offset <= range.end));
                    _setSelectionPosition(isSelectionInvertedLocal ? localTextBoundaryLocal.boundaryStart : localTextBoundaryLocal.boundaryEnd, isEnd: false);
                }
            }
        }
        return targetPosition ?? position;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _updateSelectionEdgeByTextBoundary(Offset globalPosition, bool isEnd, Func<TextPosition, (TextPosition boundaryEnd, TextPosition boundaryStart)> getTextBoundary)
    {
        global::Doroti.Ui.TextPosition? existingSelectionStart = _textSelectionStart;
        global::Doroti.Ui.TextPosition? existingSelectionEnd = _textSelectionEnd;
        _setSelectionPosition(null, isEnd: isEnd);
        Matrix4 transform = paragraph.getTransformTo(null);
        transform.invert();
        global::Doroti.Ui.Offset localPosition = MatrixUtils.transformPoint(transform, globalPosition);
        if (_rect.isEmpty)
        {
            SelectionResult result = SelectionUtils.getResultBasedOnRect(_rect, localPosition);
            _setSelectionPosition(Equals(result, SelectionResult.next) ? new global::Doroti.Ui.TextPosition(offset: range.end) : new global::Doroti.Ui.TextPosition(offset: range.start, affinity: TextAffinity.upstream), isEnd: isEnd);
            return result;
        }
        global::Doroti.Ui.Offset adjustedOffset = SelectionUtils.adjustDragOffset(_rect, localPosition, direction: paragraph.textDirection);
        global::Doroti.Ui.TextPosition position = paragraph.getPositionForOffset(adjustedOffset);
        (TextPosition boundaryEnd, TextPosition boundaryStart)? textBoundary = _rect.contains(localPosition) ? getTextBoundary(position) : null;
        if ((textBoundary is not null) && (((DartRuntimePrimitives.RequireValue(textBoundary).boundaryStart.offset < range.start) && (DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd.offset <= range.start)) || ((DartRuntimePrimitives.RequireValue(textBoundary).boundaryStart.offset >= range.end) && (DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd.offset > range.end))))
        {
            (TextPosition boundaryEnd, TextPosition boundaryStart) textBoundary__71646__value71751 = DartRuntimePrimitives.RequireValue(textBoundary);
            textBoundary = null;
        }
        global::Doroti.Ui.TextPosition targetPosition = _clampTextPosition(isEnd ? _updateSelectionEndEdgeByTextBoundary(textBoundary, getTextBoundary, position, existingSelectionStart, existingSelectionEnd) : _updateSelectionStartEdgeByTextBoundary(textBoundary, getTextBoundary, position, existingSelectionStart, existingSelectionEnd));
        _setSelectionPosition(targetPosition, isEnd: isEnd);
        if (targetPosition.offset == range.end)
        {
            return SelectionResult.next;
        }
        if (targetPosition.offset == range.start)
        {
            return SelectionResult.previous;
        }
        return SelectionUtils.getResultBasedOnRect(_rect, localPosition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _updateSelectionEdge(Offset globalPosition, bool isEnd)
    {
        _setSelectionPosition(null, isEnd: isEnd);
        Matrix4 transform = paragraph.getTransformTo(null);
        transform.invert();
        global::Doroti.Ui.Offset localPosition = MatrixUtils.transformPoint(transform, globalPosition);
        if (_rect.isEmpty)
        {
            SelectionResult result = SelectionUtils.getResultBasedOnRect(_rect, localPosition);
            _setSelectionPosition(Equals(result, SelectionResult.next) ? new global::Doroti.Ui.TextPosition(offset: range.end) : new global::Doroti.Ui.TextPosition(offset: range.start, affinity: TextAffinity.upstream), isEnd: isEnd);
            return result;
        }
        global::Doroti.Ui.Offset adjustedOffset = SelectionUtils.adjustDragOffset(_rect, localPosition, direction: paragraph.textDirection);
        global::Doroti.Ui.TextPosition position = _clampTextPosition(paragraph.getPositionForOffset(adjustedOffset));
        _setSelectionPosition(position, isEnd: isEnd);
        if (position.offset == range.end)
        {
            return SelectionResult.next;
        }
        if (position.offset == range.start)
        {
            return SelectionResult.previous;
        }
        return SelectionUtils.getResultBasedOnRect(_rect, localPosition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult? _updateSelectionStartEdgeByMultiSelectableTextBoundary(Func<TextPosition, string, (TextPosition boundaryEnd, TextPosition boundaryStart)> getTextBoundary, bool paragraphContainsPosition, TextPosition position, TextPosition? existingSelectionStart, TextPosition? existingSelectionEnd)
    {
        var isEndLocal = false;
        if (_selectableContainsOriginTextBoundary && (existingSelectionStart is not null) && (existingSelectionEnd is not null))
        {
            bool forwardSelection = existingSelectionEnd.offset >= existingSelectionStart.offset;
            if (paragraphContainsPosition)
            {
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPosition = getTextBoundary(position, fullText);
                (TextPosition boundaryEnd, TextPosition boundaryStart) originTextBoundary = getTextBoundary(forwardSelection ? new global::Doroti.Ui.TextPosition(offset: existingSelectionEnd.offset - 1L, affinity: existingSelectionEnd.affinity) : existingSelectionEnd, fullText);
                global::Doroti.Ui.TextPosition targetPosition = default!;
                long pivotOffset = forwardSelection ? originTextBoundary.boundaryEnd.offset : originTextBoundary.boundaryStart.offset;
                var shouldSwapEdges = !forwardSelection != position.offset > pivotOffset;
                if (position.offset < pivotOffset)
                {
                    targetPosition = boundaryAtPosition.boundaryStart;
                }
                else
                {
                    if (position.offset > pivotOffset)
                    {
                        targetPosition = boundaryAtPosition.boundaryEnd;
                    }
                    else
                    {
                        targetPosition = forwardSelection ? existingSelectionStart : existingSelectionEnd;
                    }
                }
                if (shouldSwapEdges)
                {
                    _setSelectionPosition(_clampTextPosition(forwardSelection ? originTextBoundary.boundaryStart : originTextBoundary.boundaryEnd), isEnd: true);
                }
                _setSelectionPosition(_clampTextPosition(targetPosition), isEnd: isEndLocal);
                bool finalSelectionIsForward = _textSelectionEnd!.offset >= _textSelectionStart!.offset;
                if ((boundaryAtPosition.boundaryStart.offset > range.end) && (boundaryAtPosition.boundaryEnd.offset > range.end))
                {
                    return SelectionResult.next;
                }
                if ((boundaryAtPosition.boundaryStart.offset < range.start) && (boundaryAtPosition.boundaryEnd.offset < range.start))
                {
                    return SelectionResult.previous;
                }
                if (finalSelectionIsForward)
                {
                    if (boundaryAtPosition.boundaryStart.offset >= originTextBoundary.boundaryStart.offset)
                    {
                        return SelectionResult.end;
                    }
                    if (boundaryAtPosition.boundaryStart.offset < originTextBoundary.boundaryStart.offset)
                    {
                        return SelectionResult.previous;
                    }
                }
                else
                {
                    if (boundaryAtPosition.boundaryEnd.offset <= originTextBoundary.boundaryEnd.offset)
                    {
                        return SelectionResult.end;
                    }
                    if (boundaryAtPosition.boundaryEnd.offset > originTextBoundary.boundaryEnd.offset)
                    {
                        return SelectionResult.next;
                    }
                }
            }
            else
            {
                global::Doroti.Ui.TextPosition clampedPosition = _clampTextPosition(position);
                (TextPosition boundaryEnd, TextPosition boundaryStart) originTextBoundaryLocal = getTextBoundary(forwardSelection ? new global::Doroti.Ui.TextPosition(offset: existingSelectionEnd.offset - 1L, affinity: existingSelectionEnd.affinity) : existingSelectionEnd, fullText);
                if (forwardSelection && (clampedPosition.offset == range.start))
                {
                    _setSelectionPosition(clampedPosition, isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
                if (!forwardSelection && (clampedPosition.offset == range.end))
                {
                    _setSelectionPosition(clampedPosition, isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if (forwardSelection && (clampedPosition.offset == range.end))
                {
                    _setSelectionPosition(_clampTextPosition(originTextBoundaryLocal.boundaryStart), isEnd: true);
                    _setSelectionPosition(clampedPosition, isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if (!forwardSelection && (clampedPosition.offset == range.start))
                {
                    _setSelectionPosition(_clampTextPosition(originTextBoundaryLocal.boundaryEnd), isEnd: true);
                    _setSelectionPosition(clampedPosition, isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
            }
        }
        else
        {
            var positionOnPlaceholder = paragraph.getWordBoundary(position).textInside(fullText) == _placeholderCharacter;
            if (!paragraphContainsPosition || positionOnPlaceholder)
            {
                return null;
            }
            if (existingSelectionEnd is not null)
            {
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPositionLocal = getTextBoundary(position, fullText);
                bool backwardSelection = ((existingSelectionStart is null) && (existingSelectionEnd.offset == range.start)) || (Equals(existingSelectionStart, existingSelectionEnd) && (existingSelectionEnd.offset == range.start)) || ((existingSelectionStart is not null) && (existingSelectionStart.offset > existingSelectionEnd.offset));
                if ((boundaryAtPositionLocal.boundaryStart.offset < range.start) && (boundaryAtPositionLocal.boundaryEnd.offset < range.start))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.start), isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
                if ((boundaryAtPositionLocal.boundaryStart.offset > range.end) && (boundaryAtPositionLocal.boundaryEnd.offset > range.end))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.end), isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if (backwardSelection)
                {
                    if (boundaryAtPositionLocal.boundaryEnd.offset <= range.end)
                    {
                        _setSelectionPosition(_clampTextPosition(boundaryAtPositionLocal.boundaryEnd), isEnd: isEndLocal);
                        return SelectionResult.end;
                    }
                    if (boundaryAtPositionLocal.boundaryEnd.offset > range.end)
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.end), isEnd: isEndLocal);
                        return SelectionResult.next;
                    }
                }
                else
                {
                    _setSelectionPosition(_clampTextPosition(boundaryAtPositionLocal.boundaryStart), isEnd: isEndLocal);
                    if (boundaryAtPositionLocal.boundaryStart.offset < range.start)
                    {
                        return SelectionResult.previous;
                    }
                    if (boundaryAtPositionLocal.boundaryStart.offset >= range.start)
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
        if (_selectableContainsOriginTextBoundary && (existingSelectionStart is not null) && (existingSelectionEnd is not null))
        {
            bool forwardSelection = existingSelectionEnd.offset >= existingSelectionStart.offset;
            if (paragraphContainsPosition)
            {
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPosition = getTextBoundary(position, fullText);
                (TextPosition boundaryEnd, TextPosition boundaryStart) originTextBoundary = getTextBoundary(forwardSelection ? existingSelectionStart : new global::Doroti.Ui.TextPosition(offset: existingSelectionStart.offset - 1L, affinity: existingSelectionStart.affinity), fullText);
                global::Doroti.Ui.TextPosition targetPosition = default!;
                long pivotOffset = forwardSelection ? originTextBoundary.boundaryStart.offset : originTextBoundary.boundaryEnd.offset;
                var shouldSwapEdges = !forwardSelection != position.offset < pivotOffset;
                if (position.offset < pivotOffset)
                {
                    targetPosition = boundaryAtPosition.boundaryStart;
                }
                else
                {
                    if (position.offset > pivotOffset)
                    {
                        targetPosition = boundaryAtPosition.boundaryEnd;
                    }
                    else
                    {
                        targetPosition = forwardSelection ? existingSelectionEnd : existingSelectionStart;
                    }
                }
                if (shouldSwapEdges)
                {
                    _setSelectionPosition(_clampTextPosition(forwardSelection ? originTextBoundary.boundaryEnd : originTextBoundary.boundaryStart), isEnd: false);
                }
                _setSelectionPosition(_clampTextPosition(targetPosition), isEnd: isEndLocal);
                bool finalSelectionIsForward = _textSelectionEnd!.offset >= _textSelectionStart!.offset;
                if ((boundaryAtPosition.boundaryStart.offset > range.end) && (boundaryAtPosition.boundaryEnd.offset > range.end))
                {
                    return SelectionResult.next;
                }
                if ((boundaryAtPosition.boundaryStart.offset < range.start) && (boundaryAtPosition.boundaryEnd.offset < range.start))
                {
                    return SelectionResult.previous;
                }
                if (finalSelectionIsForward)
                {
                    if (boundaryAtPosition.boundaryEnd.offset <= originTextBoundary.boundaryEnd.offset)
                    {
                        return SelectionResult.end;
                    }
                    if (boundaryAtPosition.boundaryEnd.offset > originTextBoundary.boundaryEnd.offset)
                    {
                        return SelectionResult.next;
                    }
                }
                else
                {
                    if (boundaryAtPosition.boundaryStart.offset >= originTextBoundary.boundaryStart.offset)
                    {
                        return SelectionResult.end;
                    }
                    if (boundaryAtPosition.boundaryStart.offset < originTextBoundary.boundaryStart.offset)
                    {
                        return SelectionResult.previous;
                    }
                }
            }
            else
            {
                global::Doroti.Ui.TextPosition clampedPosition = _clampTextPosition(position);
                (TextPosition boundaryEnd, TextPosition boundaryStart) originTextBoundaryLocal = getTextBoundary(forwardSelection ? existingSelectionStart : new global::Doroti.Ui.TextPosition(offset: existingSelectionStart.offset - 1L, affinity: existingSelectionStart.affinity), fullText);
                if (forwardSelection && (clampedPosition.offset == range.start))
                {
                    _setSelectionPosition(_clampTextPosition(originTextBoundaryLocal.boundaryEnd), isEnd: false);
                    _setSelectionPosition(clampedPosition, isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
                if (!forwardSelection && (clampedPosition.offset == range.end))
                {
                    _setSelectionPosition(_clampTextPosition(originTextBoundaryLocal.boundaryStart), isEnd: false);
                    _setSelectionPosition(clampedPosition, isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if (forwardSelection && (clampedPosition.offset == range.end))
                {
                    _setSelectionPosition(clampedPosition, isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if (!forwardSelection && (clampedPosition.offset == range.start))
                {
                    _setSelectionPosition(clampedPosition, isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
            }
        }
        else
        {
            var positionOnPlaceholder = paragraph.getWordBoundary(position).textInside(fullText) == _placeholderCharacter;
            if (!paragraphContainsPosition || positionOnPlaceholder)
            {
                return null;
            }
            if (existingSelectionStart is not null)
            {
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPositionLocal = getTextBoundary(position, fullText);
                bool backwardSelection = ((existingSelectionEnd is null) && (existingSelectionStart.offset == range.end)) || (Equals(existingSelectionStart, existingSelectionEnd) && (existingSelectionStart.offset == range.end)) || ((existingSelectionEnd is not null) && (existingSelectionStart.offset > existingSelectionEnd.offset));
                if ((boundaryAtPositionLocal.boundaryStart.offset < range.start) && (boundaryAtPositionLocal.boundaryEnd.offset < range.start))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.start), isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
                if ((boundaryAtPositionLocal.boundaryStart.offset > range.end) && (boundaryAtPositionLocal.boundaryEnd.offset > range.end))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.end), isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if (backwardSelection)
                {
                    _setSelectionPosition(_clampTextPosition(boundaryAtPositionLocal.boundaryStart), isEnd: isEndLocal);
                    if (boundaryAtPositionLocal.boundaryStart.offset < range.start)
                    {
                        return SelectionResult.previous;
                    }
                    if (boundaryAtPositionLocal.boundaryStart.offset >= range.start)
                    {
                        return SelectionResult.end;
                    }
                }
                else
                {
                    if (boundaryAtPositionLocal.boundaryEnd.offset <= range.end)
                    {
                        _setSelectionPosition(_clampTextPosition(boundaryAtPositionLocal.boundaryEnd), isEnd: isEndLocal);
                        return SelectionResult.end;
                    }
                    if (boundaryAtPositionLocal.boundaryEnd.offset > range.end)
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.end), isEnd: isEndLocal);
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
        if (_selectableContainsOriginTextBoundary && (existingSelectionStart is not null) && (existingSelectionEnd is not null))
        {
            bool forwardSelection = existingSelectionEnd.offset >= existingSelectionStart.offset;
            RenderParagraph originParagraph = _getOriginParagraph();
            var fragmentBelongsToOriginParagraph = Equals(originParagraph, paragraph);
            if (fragmentBelongsToOriginParagraph)
            {
                return _updateSelectionStartEdgeByMultiSelectableTextBoundary(getTextBoundary, paragraphContainsPosition, position, existingSelectionStart, existingSelectionEnd);
            }
            Matrix4 originTransform = originParagraph.getTransformTo(null);
            originTransform.invert();
            global::Doroti.Ui.Offset originParagraphLocalPosition = MatrixUtils.transformPoint(originTransform, globalPosition);
            bool positionWithinOriginParagraph = originParagraph.paintBounds.contains(originParagraphLocalPosition);
            global::Doroti.Ui.TextPosition positionRelativeToOriginParagraph = originParagraph.getPositionForOffset(originParagraphLocalPosition);
            if (positionWithinOriginParagraph)
            {
                string originText = originParagraph.text.toPlainText(includeSemanticsLabels: false);
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPosition = getTextBoundary(positionRelativeToOriginParagraph, originText);
                (TextPosition boundaryEnd, TextPosition boundaryStart) originTextBoundary = getTextBoundary(_getPositionInParagraph(originParagraph), originText);
                global::Doroti.Ui.TextPosition targetPosition = default!;
                long pivotOffset = forwardSelection ? originTextBoundary.boundaryEnd.offset : originTextBoundary.boundaryStart.offset;
                var shouldSwapEdges = !forwardSelection != positionRelativeToOriginParagraph.offset > pivotOffset;
                if (positionRelativeToOriginParagraph.offset < pivotOffset)
                {
                    targetPosition = boundaryAtPosition.boundaryStart;
                }
                else
                {
                    if (positionRelativeToOriginParagraph.offset > pivotOffset)
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
                bool finalSelectionIsForward = _textSelectionEnd!.offset >= _textSelectionStart!.offset;
                global::Doroti.Ui.TextPosition originParagraphPlaceholderTextPosition = _getPositionInParagraph(originParagraph);
                var originParagraphPlaceholderRange = new global::Doroti.Ui.TextRange(start: originParagraphPlaceholderTextPosition.offset, end: originParagraphPlaceholderTextPosition.offset + _placeholderLength);
                if ((boundaryAtPosition.boundaryStart.offset > originParagraphPlaceholderRange.end) && (boundaryAtPosition.boundaryEnd.offset > originParagraphPlaceholderRange.end))
                {
                    return SelectionResult.next;
                }
                if ((boundaryAtPosition.boundaryStart.offset < originParagraphPlaceholderRange.start) && (boundaryAtPosition.boundaryEnd.offset < originParagraphPlaceholderRange.start))
                {
                    return SelectionResult.previous;
                }
                if (finalSelectionIsForward)
                {
                    if (boundaryAtPosition.boundaryEnd.offset <= originTextBoundary.boundaryEnd.offset)
                    {
                        return SelectionResult.end;
                    }
                    if (boundaryAtPosition.boundaryEnd.offset > originTextBoundary.boundaryEnd.offset)
                    {
                        return SelectionResult.next;
                    }
                }
                else
                {
                    if (boundaryAtPosition.boundaryStart.offset >= originTextBoundary.boundaryStart.offset)
                    {
                        return SelectionResult.end;
                    }
                    if (boundaryAtPosition.boundaryStart.offset < originTextBoundary.boundaryStart.offset)
                    {
                        return SelectionResult.previous;
                    }
                }
            }
            else
            {
                global::Doroti.Ui.Offset adjustedOffset = SelectionUtils.adjustDragOffset(originParagraph.paintBounds, originParagraphLocalPosition, direction: paragraph.textDirection);
                global::Doroti.Ui.TextPosition adjustedPositionRelativeToOriginParagraph = originParagraph.getPositionForOffset(adjustedOffset);
                global::Doroti.Ui.TextPosition originParagraphPlaceholderTextPositionLocal = _getPositionInParagraph(originParagraph);
                var originParagraphPlaceholderRangeLocal = new global::Doroti.Ui.TextRange(start: originParagraphPlaceholderTextPositionLocal.offset, end: originParagraphPlaceholderTextPositionLocal.offset + _placeholderLength);
                if (forwardSelection && (adjustedPositionRelativeToOriginParagraph.offset <= originParagraphPlaceholderRangeLocal.start))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.start), isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
                if (!forwardSelection && (adjustedPositionRelativeToOriginParagraph.offset >= originParagraphPlaceholderRangeLocal.end))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.end), isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if (forwardSelection && (adjustedPositionRelativeToOriginParagraph.offset >= originParagraphPlaceholderRangeLocal.end))
                {
                    _setSelectionPosition(existingSelectionStart, isEnd: true);
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.end), isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if (!forwardSelection && (adjustedPositionRelativeToOriginParagraph.offset <= originParagraphPlaceholderRangeLocal.start))
                {
                    _setSelectionPosition(existingSelectionStart, isEnd: true);
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.start), isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
            }
        }
        else
        {
            if (paragraphContainsPosition)
            {
                return _updateSelectionStartEdgeByMultiSelectableTextBoundary(getTextBoundary, paragraphContainsPosition, position, existingSelectionStart, existingSelectionEnd);
            }
            if (existingSelectionEnd is not null)
            {
                (RenderParagraph paragraph, global::Doroti.Ui.Offset localPosition)? targetDetails = _getParagraphContainingPosition(globalPosition);
                if (targetDetails is null)
                {
                    return null;
                }
                RenderParagraph targetParagraph = DartRuntimePrimitives.RequireValue(targetDetails).paragraph;
                global::Doroti.Ui.TextPosition positionRelativeToTargetParagraph = targetParagraph.getPositionForOffset(DartRuntimePrimitives.RequireValue(targetDetails).localPosition);
                string targetText = targetParagraph.text.toPlainText(includeSemanticsLabels: false);
                var positionOnPlaceholder = targetParagraph.getWordBoundary(positionRelativeToTargetParagraph).textInside(targetText) == _placeholderCharacter;
                if (positionOnPlaceholder)
                {
                    return null;
                }
                bool backwardSelection = ((existingSelectionStart is null) && (existingSelectionEnd.offset == range.start)) || (Equals(existingSelectionStart, existingSelectionEnd) && (existingSelectionEnd.offset == range.start)) || ((existingSelectionStart is not null) && (existingSelectionStart.offset > existingSelectionEnd.offset));
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPositionRelativeToTargetParagraph = getTextBoundary(positionRelativeToTargetParagraph, targetText);
                global::Doroti.Ui.TextPosition targetParagraphPlaceholderTextPosition = _getPositionInParagraph(targetParagraph);
                var targetParagraphPlaceholderRange = new global::Doroti.Ui.TextRange(start: targetParagraphPlaceholderTextPosition.offset, end: targetParagraphPlaceholderTextPosition.offset + _placeholderLength);
                if ((boundaryAtPositionRelativeToTargetParagraph.boundaryStart.offset < targetParagraphPlaceholderRange.start) && (boundaryAtPositionRelativeToTargetParagraph.boundaryEnd.offset < targetParagraphPlaceholderRange.start))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.start), isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
                if ((boundaryAtPositionRelativeToTargetParagraph.boundaryStart.offset > targetParagraphPlaceholderRange.end) && (boundaryAtPositionRelativeToTargetParagraph.boundaryEnd.offset > targetParagraphPlaceholderRange.end))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.end), isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if (backwardSelection)
                {
                    if (boundaryAtPositionRelativeToTargetParagraph.boundaryEnd.offset <= targetParagraphPlaceholderRange.end)
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.end), isEnd: isEndLocal);
                        return SelectionResult.end;
                    }
                    if (boundaryAtPositionRelativeToTargetParagraph.boundaryEnd.offset > targetParagraphPlaceholderRange.end)
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.end), isEnd: isEndLocal);
                        return SelectionResult.next;
                    }
                }
                else
                {
                    if (boundaryAtPositionRelativeToTargetParagraph.boundaryStart.offset >= targetParagraphPlaceholderRange.start)
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.start), isEnd: isEndLocal);
                        return SelectionResult.end;
                    }
                    if (boundaryAtPositionRelativeToTargetParagraph.boundaryStart.offset < targetParagraphPlaceholderRange.start)
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.start), isEnd: isEndLocal);
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
        if (_selectableContainsOriginTextBoundary && (existingSelectionStart is not null) && (existingSelectionEnd is not null))
        {
            bool forwardSelection = existingSelectionEnd.offset >= existingSelectionStart.offset;
            RenderParagraph originParagraph = _getOriginParagraph();
            var fragmentBelongsToOriginParagraph = Equals(originParagraph, paragraph);
            if (fragmentBelongsToOriginParagraph)
            {
                return _updateSelectionEndEdgeByMultiSelectableTextBoundary(getTextBoundary, paragraphContainsPosition, position, existingSelectionStart, existingSelectionEnd);
            }
            Matrix4 originTransform = originParagraph.getTransformTo(null);
            originTransform.invert();
            global::Doroti.Ui.Offset originParagraphLocalPosition = MatrixUtils.transformPoint(originTransform, globalPosition);
            bool positionWithinOriginParagraph = originParagraph.paintBounds.contains(originParagraphLocalPosition);
            global::Doroti.Ui.TextPosition positionRelativeToOriginParagraph = originParagraph.getPositionForOffset(originParagraphLocalPosition);
            if (positionWithinOriginParagraph)
            {
                string originText = originParagraph.text.toPlainText(includeSemanticsLabels: false);
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPosition = getTextBoundary(positionRelativeToOriginParagraph, originText);
                (TextPosition boundaryEnd, TextPosition boundaryStart) originTextBoundary = getTextBoundary(_getPositionInParagraph(originParagraph), originText);
                global::Doroti.Ui.TextPosition targetPosition = default!;
                long pivotOffset = forwardSelection ? originTextBoundary.boundaryStart.offset : originTextBoundary.boundaryEnd.offset;
                var shouldSwapEdges = !forwardSelection != positionRelativeToOriginParagraph.offset < pivotOffset;
                if (positionRelativeToOriginParagraph.offset < pivotOffset)
                {
                    targetPosition = boundaryAtPosition.boundaryStart;
                }
                else
                {
                    if (positionRelativeToOriginParagraph.offset > pivotOffset)
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
                bool finalSelectionIsForward = _textSelectionEnd!.offset >= _textSelectionStart!.offset;
                global::Doroti.Ui.TextPosition originParagraphPlaceholderTextPosition = _getPositionInParagraph(originParagraph);
                var originParagraphPlaceholderRange = new global::Doroti.Ui.TextRange(start: originParagraphPlaceholderTextPosition.offset, end: originParagraphPlaceholderTextPosition.offset + _placeholderLength);
                if ((boundaryAtPosition.boundaryStart.offset > originParagraphPlaceholderRange.end) && (boundaryAtPosition.boundaryEnd.offset > originParagraphPlaceholderRange.end))
                {
                    return SelectionResult.next;
                }
                if ((boundaryAtPosition.boundaryStart.offset < originParagraphPlaceholderRange.start) && (boundaryAtPosition.boundaryEnd.offset < originParagraphPlaceholderRange.start))
                {
                    return SelectionResult.previous;
                }
                if (finalSelectionIsForward)
                {
                    if (boundaryAtPosition.boundaryEnd.offset <= originTextBoundary.boundaryEnd.offset)
                    {
                        return SelectionResult.end;
                    }
                    if (boundaryAtPosition.boundaryEnd.offset > originTextBoundary.boundaryEnd.offset)
                    {
                        return SelectionResult.next;
                    }
                }
                else
                {
                    if (boundaryAtPosition.boundaryStart.offset >= originTextBoundary.boundaryStart.offset)
                    {
                        return SelectionResult.end;
                    }
                    if (boundaryAtPosition.boundaryStart.offset < originTextBoundary.boundaryStart.offset)
                    {
                        return SelectionResult.previous;
                    }
                }
            }
            else
            {
                global::Doroti.Ui.Offset adjustedOffset = SelectionUtils.adjustDragOffset(originParagraph.paintBounds, originParagraphLocalPosition, direction: paragraph.textDirection);
                global::Doroti.Ui.TextPosition adjustedPositionRelativeToOriginParagraph = originParagraph.getPositionForOffset(adjustedOffset);
                global::Doroti.Ui.TextPosition originParagraphPlaceholderTextPositionLocal = _getPositionInParagraph(originParagraph);
                var originParagraphPlaceholderRangeLocal = new global::Doroti.Ui.TextRange(start: originParagraphPlaceholderTextPositionLocal.offset, end: originParagraphPlaceholderTextPositionLocal.offset + _placeholderLength);
                if (forwardSelection && (adjustedPositionRelativeToOriginParagraph.offset <= originParagraphPlaceholderRangeLocal.start))
                {
                    _setSelectionPosition(existingSelectionEnd, isEnd: false);
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.start), isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
                if (!forwardSelection && (adjustedPositionRelativeToOriginParagraph.offset >= originParagraphPlaceholderRangeLocal.end))
                {
                    _setSelectionPosition(existingSelectionEnd, isEnd: false);
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.end), isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if (forwardSelection && (adjustedPositionRelativeToOriginParagraph.offset >= originParagraphPlaceholderRangeLocal.end))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.end), isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if (!forwardSelection && (adjustedPositionRelativeToOriginParagraph.offset <= originParagraphPlaceholderRangeLocal.start))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.start), isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
            }
        }
        else
        {
            if (paragraphContainsPosition)
            {
                return _updateSelectionEndEdgeByMultiSelectableTextBoundary(getTextBoundary, paragraphContainsPosition, position, existingSelectionStart, existingSelectionEnd);
            }
            if (existingSelectionStart is not null)
            {
                (RenderParagraph paragraph, global::Doroti.Ui.Offset localPosition)? targetDetails = _getParagraphContainingPosition(globalPosition);
                if (targetDetails is null)
                {
                    return null;
                }
                RenderParagraph targetParagraph = DartRuntimePrimitives.RequireValue(targetDetails).paragraph;
                global::Doroti.Ui.TextPosition positionRelativeToTargetParagraph = targetParagraph.getPositionForOffset(DartRuntimePrimitives.RequireValue(targetDetails).localPosition);
                string targetText = targetParagraph.text.toPlainText(includeSemanticsLabels: false);
                var positionOnPlaceholder = targetParagraph.getWordBoundary(positionRelativeToTargetParagraph).textInside(targetText) == _placeholderCharacter;
                if (positionOnPlaceholder)
                {
                    return null;
                }
                bool backwardSelection = ((existingSelectionEnd is null) && (existingSelectionStart.offset == range.end)) || (Equals(existingSelectionStart, existingSelectionEnd) && (existingSelectionStart.offset == range.end)) || ((existingSelectionEnd is not null) && (existingSelectionStart.offset > existingSelectionEnd.offset));
                (TextPosition boundaryEnd, TextPosition boundaryStart) boundaryAtPositionRelativeToTargetParagraph = getTextBoundary(positionRelativeToTargetParagraph, targetText);
                global::Doroti.Ui.TextPosition targetParagraphPlaceholderTextPosition = _getPositionInParagraph(targetParagraph);
                var targetParagraphPlaceholderRange = new global::Doroti.Ui.TextRange(start: targetParagraphPlaceholderTextPosition.offset, end: targetParagraphPlaceholderTextPosition.offset + _placeholderLength);
                if ((boundaryAtPositionRelativeToTargetParagraph.boundaryStart.offset < targetParagraphPlaceholderRange.start) && (boundaryAtPositionRelativeToTargetParagraph.boundaryEnd.offset < targetParagraphPlaceholderRange.start))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.start), isEnd: isEndLocal);
                    return SelectionResult.previous;
                }
                if ((boundaryAtPositionRelativeToTargetParagraph.boundaryStart.offset > targetParagraphPlaceholderRange.end) && (boundaryAtPositionRelativeToTargetParagraph.boundaryEnd.offset > targetParagraphPlaceholderRange.end))
                {
                    _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.end), isEnd: isEndLocal);
                    return SelectionResult.next;
                }
                if (backwardSelection)
                {
                    if (boundaryAtPositionRelativeToTargetParagraph.boundaryStart.offset >= targetParagraphPlaceholderRange.start)
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.start), isEnd: isEndLocal);
                        return SelectionResult.end;
                    }
                    if (boundaryAtPositionRelativeToTargetParagraph.boundaryStart.offset < targetParagraphPlaceholderRange.start)
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.start), isEnd: isEndLocal);
                        return SelectionResult.previous;
                    }
                }
                else
                {
                    if (boundaryAtPositionRelativeToTargetParagraph.boundaryEnd.offset <= targetParagraphPlaceholderRange.end)
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.end), isEnd: isEndLocal);
                        return SelectionResult.end;
                    }
                    if (boundaryAtPositionRelativeToTargetParagraph.boundaryEnd.offset > targetParagraphPlaceholderRange.end)
                    {
                        _setSelectionPosition(new global::Doroti.Ui.TextPosition(offset: range.end), isEnd: isEndLocal);
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
        global::Doroti.Ui.TextPosition? existingSelectionStart = _textSelectionStart;
        global::Doroti.Ui.TextPosition? existingSelectionEnd = _textSelectionEnd;
        _setSelectionPosition(null, isEnd: isEnd);
        Matrix4 transform = paragraph.getTransformTo(null);
        transform.invert();
        global::Doroti.Ui.Offset localPosition = MatrixUtils.transformPoint(transform, globalPosition);
        if (_rect.isEmpty)
        {
            SelectionResult result = SelectionUtils.getResultBasedOnRect(_rect, localPosition);
            _setSelectionPosition(Equals(result, SelectionResult.next) ? new global::Doroti.Ui.TextPosition(offset: range.end) : new global::Doroti.Ui.TextPosition(offset: range.start, affinity: TextAffinity.upstream), isEnd: isEnd);
            return result;
        }
        global::Doroti.Ui.Offset adjustedOffset = SelectionUtils.adjustDragOffset(_rect, localPosition, direction: paragraph.textDirection);
        global::Doroti.Ui.Offset adjustedOffsetRelativeToParagraph = SelectionUtils.adjustDragOffset(paragraph.paintBounds, localPosition, direction: paragraph.textDirection);
        global::Doroti.Ui.TextPosition position = paragraph.getPositionForOffset(adjustedOffset);
        global::Doroti.Ui.TextPosition positionInFullText = paragraph.getPositionForOffset(adjustedOffsetRelativeToParagraph);
        SelectionResult? resultLocal = default!;
        if (_isPlaceholder())
        {
            resultLocal = isEnd ? _updateSelectionEndEdgeAtPlaceholderByMultiSelectableTextBoundary(getTextBoundary, globalPosition, paragraph.paintBounds.contains(localPosition), positionInFullText, existingSelectionStart, existingSelectionEnd) : _updateSelectionStartEdgeAtPlaceholderByMultiSelectableTextBoundary(getTextBoundary, globalPosition, paragraph.paintBounds.contains(localPosition), positionInFullText, existingSelectionStart, existingSelectionEnd);
        }
        else
        {
            resultLocal = isEnd ? _updateSelectionEndEdgeByMultiSelectableTextBoundary(getTextBoundary, paragraph.paintBounds.contains(localPosition), positionInFullText, existingSelectionStart, existingSelectionEnd) : _updateSelectionStartEdgeByMultiSelectableTextBoundary(getTextBoundary, paragraph.paintBounds.contains(localPosition), positionInFullText, existingSelectionStart, existingSelectionEnd);
        }
        if (resultLocal is not null)
        {
            SelectionResult result__118831__value120148 = DartRuntimePrimitives.RequireValue(resultLocal);
            return DartRuntimePrimitives.RequireValue(result__118831__value120148);
        }
        (TextPosition boundaryEnd, TextPosition boundaryStart)? textBoundary = _boundingBoxesContains(localPosition) ? getClampedTextBoundary(position) : null;
        if ((textBoundary is not null) && (((DartRuntimePrimitives.RequireValue(textBoundary).boundaryStart.offset < range.start) && (DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd.offset <= range.start)) || ((DartRuntimePrimitives.RequireValue(textBoundary).boundaryStart.offset >= range.end) && (DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd.offset > range.end))))
        {
            (TextPosition boundaryEnd, TextPosition boundaryStart) textBoundary__120500__value120620 = DartRuntimePrimitives.RequireValue(textBoundary);
            textBoundary = null;
        }
        global::Doroti.Ui.TextPosition targetPosition = _clampTextPosition(isEnd ? _updateSelectionEndEdgeByTextBoundary(textBoundary, getClampedTextBoundary, position, existingSelectionStart, existingSelectionEnd) : _updateSelectionStartEdgeByTextBoundary(textBoundary, getClampedTextBoundary, position, existingSelectionStart, existingSelectionEnd));
        _setSelectionPosition(targetPosition, isEnd: isEnd);
        if (targetPosition.offset == range.end)
        {
            return SelectionResult.next;
        }
        if (targetPosition.offset == range.start)
        {
            return SelectionResult.previous;
        }
        return SelectionUtils.getResultBasedOnRect(_rect, localPosition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _closestTextBoundary((TextPosition boundaryEnd, TextPosition boundaryStart) textBoundary, TextPosition position)
    {
        long differenceA = (position.offset - DartRuntimePrimitives.RequireValue(textBoundary).boundaryStart.offset).abs();
        long differenceB = (position.offset - DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd.offset).abs();
        return (differenceA < differenceB) ? DartRuntimePrimitives.RequireValue(textBoundary).boundaryStart : DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isPlaceholder()
    {
        RenderObject? current = paragraph.parent;
        while (current is not null)
        {
            if (current is RenderParagraph)
            {
                RenderParagraph current__122853__as122921 = (RenderParagraph)current;
                return true;
            }
            current = current.parent;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual RenderParagraph _getOriginParagraph()
    {
        DartRuntimePrimitives.Assert(() => _selectableContainsOriginTextBoundary);
        RenderObject? current = paragraph.parent;
        RenderParagraph? originParagraph = default!;
        while (current is not null)
        {
            if (current is RenderParagraph)
            {
                RenderParagraph current__123508__as123614 = (RenderParagraph)current;
                if (current__123508__as123614._lastSelectableFragments is not null)
                {
                    var paragraphContainsOriginTextBoundary = false;
                    foreach (_SelectableFragment__paragraph fragment in current__123508__as123614._lastSelectableFragments!)
                    {
                        if (fragment._selectableContainsOriginTextBoundary)
                        {
                            paragraphContainsOriginTextBoundary = true;
                            originParagraph = current__123508__as123614;
                            break;
                        }
                    }
                    if (!paragraphContainsOriginTextBoundary)
                    {
                        return originParagraph ?? paragraph;
                    }
                }
            }
            current = current.parent;
        }
        return originParagraph ?? paragraph;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (RenderParagraph paragraph, global::Doroti.Ui.Offset localPosition)? _getParagraphContainingPosition(Offset globalPosition)
    {
        RenderObject? current = paragraph;
        while (current is not null)
        {
            if (current is RenderParagraph)
            {
                RenderParagraph current__124717__as124778 = (RenderParagraph)current;
                Matrix4 currentTransform = current__124717__as124778.getTransformTo(null);
                currentTransform.invert();
                global::Doroti.Ui.Offset currentParagraphLocalPosition = MatrixUtils.transformPoint(currentTransform, globalPosition);
                bool positionWithinCurrentParagraph = current__124717__as124778.paintBounds.contains(currentParagraphLocalPosition);
                if (positionWithinCurrentParagraph)
                {
                    return (paragraph: current__124717__as124778, localPosition: currentParagraphLocalPosition);
                }
            }
            current = current.parent;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _boundingBoxesContains(Offset position)
    {
        foreach (global::Doroti.Ui.Rect rect in boundingBoxes)
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
        if ((position.offset > range.end) || (position.offset == range.end) && Equals(position.affinity, TextAffinity.downstream))
        {
            return new global::Doroti.Ui.TextPosition(offset: range.end, affinity: TextAffinity.upstream);
        }
        if (position.offset < range.start)
        {
            return new global::Doroti.Ui.TextPosition(offset: range.start);
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
        _textSelectionStart = new global::Doroti.Ui.TextPosition(offset: range.start);
        _textSelectionEnd = new global::Doroti.Ui.TextPosition(offset: range.end, affinity: TextAffinity.upstream);
        return SelectionResult.none;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _handleSelectTextBoundary((TextPosition boundaryEnd, TextPosition boundaryStart) textBoundary)
    {
        if ((DartRuntimePrimitives.RequireValue(textBoundary).boundaryStart.offset < range.start) && (textBoundary.boundaryEnd.offset <= range.start))
        {
            return SelectionResult.previous;
        }
        else
        {
            if ((textBoundary.boundaryStart.offset >= range.end) && (DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd.offset > range.end))
            {
                return SelectionResult.next;
            }
        }
        DartRuntimePrimitives.Assert(() => (textBoundary.boundaryStart.offset >= range.start) && (textBoundary.boundaryEnd.offset <= range.end));
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
        if (startMax <= endMin)
        {
            return new global::Doroti.Ui.TextRange(start: startMax, end: endMin);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _handleSelectMultiFragmentTextBoundary((TextPosition boundaryEnd, TextPosition boundaryStart) textBoundary)
    {
        if ((DartRuntimePrimitives.RequireValue(textBoundary).boundaryStart.offset < range.start) && (textBoundary.boundaryEnd.offset <= range.start))
        {
            return SelectionResult.previous;
        }
        else
        {
            if ((textBoundary.boundaryStart.offset >= range.end) && (DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd.offset > range.end))
            {
                return SelectionResult.next;
            }
        }
        var boundaryAsRange = new global::Doroti.Ui.TextRange(start: DartRuntimePrimitives.RequireValue(textBoundary).boundaryStart.offset, end: DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd.offset);
        global::Doroti.Ui.TextRange? intersectRange = _intersect(range, boundaryAsRange);
        if (intersectRange is not null)
        {
            _textSelectionStart = new global::Doroti.Ui.TextPosition(offset: intersectRange.start);
            _textSelectionEnd = new global::Doroti.Ui.TextPosition(offset: intersectRange.end);
            _selectableContainsOriginTextBoundary = true;
            if (range.end < DartRuntimePrimitives.RequireValue(textBoundary).boundaryEnd.offset)
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
        if (position.offset > textBoundary.end)
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
        global::Doroti.Ui.TextPosition position = paragraph.getPositionForOffset(paragraph.globalToLocal(globalPosition));
        if (_positionIsWithinCurrentSelection(position) && (!Equals(_textSelectionStart, _textSelectionEnd)))
        {
            return SelectionResult.end;
        }
        (TextPosition boundaryEnd, TextPosition boundaryStart) wordBoundary = _getWordBoundaryAtPosition(position);
        return _handleSelectTextBoundary(wordBoundary);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (TextPosition boundaryEnd, TextPosition boundaryStart) _getWordBoundaryAtPosition(TextPosition position)
    {
        global::Doroti.Ui.TextRange word = paragraph.getWordBoundary(position);
        DartRuntimePrimitives.Assert(() => word.isNormalized);
        return _adjustTextBoundaryAtPosition(word, position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _handleSelectParagraph(Offset globalPosition)
    {
        global::Doroti.Ui.Offset localPosition = paragraph.globalToLocal(globalPosition);
        global::Doroti.Ui.TextPosition position = paragraph.getPositionForOffset(localPosition);
        (TextPosition boundaryEnd, TextPosition boundaryStart) paragraphBoundary = _getParagraphBoundaryAtPosition(position, fullText);
        return _handleSelectMultiFragmentTextBoundary(paragraphBoundary);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _getPositionInParagraph(RenderParagraph targetParagraph)
    {
        Matrix4 transform = paragraph.getTransformTo(targetParagraph);
        global::Doroti.Ui.Offset localCenter = paragraph.paintBounds.centerLeft;
        global::Doroti.Ui.Offset localPos = MatrixUtils.transformPoint(transform, localCenter);
        global::Doroti.Ui.TextPosition position = targetParagraph.getPositionForOffset(localPos);
        return position;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (TextPosition boundaryEnd, TextPosition boundaryStart) _getParagraphBoundaryAtPosition(TextPosition position, string text)
    {
        var paragraphBoundary = new ParagraphBoundary(text);
        long paragraphStart = paragraphBoundary.getLeadingTextBoundaryAt(((position.offset == text.Length) || Equals(position.affinity, TextAffinity.upstream)) ? (position.offset - 1L) : position.offset) ?? 0L;
        long paragraphEnd = paragraphBoundary.getTrailingTextBoundaryAt(position.offset) ?? text.Length;
        var paragraphRange = new global::Doroti.Ui.TextRange(start: paragraphStart, end: paragraphEnd);
        DartRuntimePrimitives.Assert(() => paragraphRange.isNormalized);
        return _adjustTextBoundaryAtPosition(paragraphRange, position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (TextPosition boundaryEnd, TextPosition boundaryStart) _getClampedParagraphBoundaryAtPosition(TextPosition position)
    {
        var paragraphBoundary = new ParagraphBoundary(fullText);
        long paragraphStart = paragraphBoundary.getLeadingTextBoundaryAt(((position.offset == fullText.Length) || Equals(position.affinity, TextAffinity.upstream)) ? (position.offset - 1L) : position.offset) ?? 0L;
        long paragraphEnd = paragraphBoundary.getTrailingTextBoundaryAt(position.offset) ?? fullText.Length;
        paragraphStart = (paragraphStart < range.start) ? range.start : ((paragraphStart > range.end) ? range.end : paragraphStart);
        paragraphEnd = (paragraphEnd > range.end) ? range.end : ((paragraphEnd < range.start) ? range.start : paragraphEnd);
        var paragraphRange = new global::Doroti.Ui.TextRange(start: paragraphStart, end: paragraphEnd);
        DartRuntimePrimitives.Assert(() => paragraphRange.isNormalized);
        return _adjustTextBoundaryAtPosition(paragraphRange, position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SelectionResult _handleDirectionallyExtendSelection(double horizontalBaseline, bool isExtent, SelectionExtendDirection movement)
    {
        Matrix4 transform = paragraph.getTransformTo(null);
        if (transform.invert() == 0.0)
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
                    DartRuntimePrimitives.Assert(() => (_textSelectionEnd is not null) && (_textSelectionStart is not null));
                    global::Doroti.Ui.TextPosition targetedEdge = isExtent ? _textSelectionEnd! : _textSelectionStart!;
                    MapEntry<global::Doroti.Ui.TextPosition, SelectionResult> moveResult = _handleVerticalMovement(targetedEdge, horizontalBaselineInParagraphCoordinates: baselineInParagraphCoordinates, below: Equals(movement, SelectionExtendDirection.nextLine));
                    newPosition = moveResult.key;
                    result = moveResult.value;
                    break;
                }
            case SelectionExtendDirection.forward:
            case SelectionExtendDirection.backward:
                {
                    _textSelectionEnd ??= (Equals(movement, SelectionExtendDirection.forward) ? new global::Doroti.Ui.TextPosition(offset: range.start) : new global::Doroti.Ui.TextPosition(offset: range.end, affinity: TextAffinity.upstream));
                    _textSelectionStart ??= _textSelectionEnd;
                    global::Doroti.Ui.TextPosition targetedEdgeLocal = isExtent ? _textSelectionEnd! : _textSelectionStart!;
                    global::Doroti.Ui.Offset edgeOffsetInParagraphCoordinates = paragraph._getOffsetForPosition(targetedEdgeLocal);
                    var baselineOffsetInParagraphCoordinates = new global::Doroti.Ui.Offset(baselineInParagraphCoordinates, edgeOffsetInParagraphCoordinates.dy - (paragraph._textPainter.preferredLineHeight / 2L));
                    newPosition = paragraph.getPositionForOffset(baselineOffsetInParagraphCoordinates);
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
        _textSelectionEnd ??= (forward ? new global::Doroti.Ui.TextPosition(offset: range.start) : new global::Doroti.Ui.TextPosition(offset: range.end, affinity: TextAffinity.upstream));
        _textSelectionStart ??= _textSelectionEnd;
        global::Doroti.Ui.TextPosition targetedEdge = isExtent ? _textSelectionEnd! : _textSelectionStart!;
        if (forward && targetedEdge.offset == range.end)
        {
            return SelectionResult.next;
        }
        if (!forward && targetedEdge.offset == range.start)
        {
            return SelectionResult.previous;
        }
        SelectionResult result = default!;
        global::Doroti.Ui.TextPosition newPosition = default!;
        switch (granularity)
        {
            case TextGranularity.character:
                {
                    string text = range.textInside(fullText);
                    newPosition = _moveBeyondTextBoundaryAtDirection(targetedEdge, forward, new CharacterBoundary(text));
                    result = SelectionResult.end;
                    break;
                }
            case TextGranularity.word:
                {
                    TextBoundary textBoundary = paragraph._textPainter.wordBoundaries.moveByWordBoundary;
                    newPosition = _moveBeyondTextBoundaryAtDirection(targetedEdge, forward, textBoundary);
                    result = SelectionResult.end;
                    break;
                }
            case TextGranularity.paragraph:
                {
                    string textLocal = range.textInside(fullText);
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
                    string textAlternate = range.textInside(fullText);
                    newPosition = _moveBeyondTextBoundaryAtDirection(targetedEdge, forward, new DocumentBoundary(textAlternate));
                    if (forward && (newPosition.offset == range.end))
                    {
                        result = SelectionResult.next;
                    }
                    else
                    {
                        if (!forward && (newPosition.offset == range.start))
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
        long newOffset = forward ? (textBoundary.getTrailingTextBoundaryAt(end.offset) ?? range.end) : (textBoundary.getLeadingTextBoundaryAt(end.offset - 1L) ?? range.start);
        return new global::Doroti.Ui.TextPosition(offset: newOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.TextPosition _moveToTextBoundaryAtDirection(TextPosition end, bool forward, TextBoundary textBoundary)
    {
        DartRuntimePrimitives.Assert(() => end.offset >= 0L);
        long caretOffset = default!;
        switch (end.affinity)
        {
            case TextAffinity.upstream:
                {
                    if ((end.offset < 1L) && !forward)
                    {
                        DartRuntimePrimitives.Assert(() => end.offset == 0L);
                        return new global::Doroti.Ui.TextPosition(offset: 0L);
                    }
                    var characterBoundary = new CharacterBoundary(fullText);
                    caretOffset = Math.Max(0L, characterBoundary.getLeadingTextBoundaryAt(range.start + end.offset) ?? range.start) - 1L;
                    break;
                }
            case TextAffinity.downstream:
                {
                    caretOffset = end.offset;
                    break;
                }
        }
        long offsetLocal = forward ? (textBoundary.getTrailingTextBoundaryAt(caretOffset) ?? range.end) : (textBoundary.getLeadingTextBoundaryAt(caretOffset) ?? range.start);
        return new global::Doroti.Ui.TextPosition(offset: offsetLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual MapEntry<global::Doroti.Ui.TextPosition, SelectionResult> _handleVerticalMovement(TextPosition position, double horizontalBaselineInParagraphCoordinates, bool below)
    {
        List<global::Doroti.Ui.LineMetrics> lines = paragraph._textPainter.computeLineMetrics();
        global::Doroti.Ui.Offset offsetLocal = paragraph.getOffsetForCaret(position, Rect.zero);
        long currentLine = checked(lines.Count) - 1L;
        foreach (var lineMetrics in lines)
        {
            if (lineMetrics.baseline > offsetLocal.dy)
            {
                currentLine = lineMetrics.lineNumber;
                break;
            }
        }
        global::Doroti.Ui.TextPosition newPosition = default!;
        if (below && (currentLine == (checked(lines.Count) - 1L)))
        {
            newPosition = new global::Doroti.Ui.TextPosition(offset: range.end, affinity: TextAffinity.upstream);
        }
        else
        {
            if (!below && (currentLine == 0L))
            {
                newPosition = new global::Doroti.Ui.TextPosition(offset: range.start);
            }
            else
            {
                long newLine = below ? (currentLine + 1L) : (currentLine - 1L);
                newPosition = _clampTextPosition(paragraph.getPositionForOffset(new global::Doroti.Ui.Offset(horizontalBaselineInParagraphCoordinates, lines[(int)newLine].baseline)));
            }
        }
        SelectionResult result = default!;
        if (newPosition.offset == range.start)
        {
            result = SelectionResult.previous;
        }
        else
        {
            if (newPosition.offset == range.end)
            {
                result = SelectionResult.next;
            }
            else
            {
                result = SelectionResult.end;
            }
        }
        DartRuntimePrimitives.Assert(() => (!Equals(result, SelectionResult.next)) || below);
        DartRuntimePrimitives.Assert(() => (!Equals(result, SelectionResult.previous)) || !below);
        return new MapEntry<global::Doroti.Ui.TextPosition, SelectionResult>(newPosition, result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _positionIsWithinCurrentSelection(TextPosition position)
    {
        if ((_textSelectionStart is null) || (_textSelectionEnd is null))
        {
            return false;
        }
        global::Doroti.Ui.TextPosition currentStart = default!;
        global::Doroti.Ui.TextPosition currentEnd = default!;
        if (_compareTextPositions(_textSelectionStart!, _textSelectionEnd!) > 0L)
        {
            currentStart = _textSelectionStart!;
            currentEnd = _textSelectionEnd!;
        }
        else
        {
            currentStart = _textSelectionEnd!;
            currentEnd = _textSelectionStart!;
        }
        return (_compareTextPositions(currentStart, position) >= 0L) && (_compareTextPositions(currentEnd, position) <= 0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static long _compareTextPositions(TextPosition position, TextPosition otherPosition)
    {
        if (position.offset < otherPosition.offset)
        {
            return 1L;
        }
        else
        {
            if (position.offset > otherPosition.offset)
            {
                return -1L;
            }
            else
            {
                if (Equals(position.affinity, otherPosition.affinity))
                {
                    return 0L;
                }
                else
                {
                    return Equals(position.affinity, TextAffinity.upstream) ? 1L : -1L;
                }
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Matrix4 getTransformTo(RenderObject? ancestor)
    {
        return paragraph.getTransformTo(ancestor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void pushHandleLayers(LayerLink? startHandle, LayerLink? endHandle)
    {
        if (!paragraph.attached)
        {
            DartRuntimePrimitives.Assert(() => (startHandle is null) && (endHandle is null));
            return;
        }
        if (!Equals(_startHandleLayerLink, startHandle))
        {
            _startHandleLayerLink = startHandle;
            paragraph.markNeedsPaint();
        }
        if (!Equals(_endHandleLayerLink, endHandle))
        {
            _endHandleLayerLink = endHandle;
            paragraph.markNeedsPaint();
        }
    }

    public virtual List<Rect> boundingBoxes
    {
        get
        {
            if (_cachedBoundingBoxes is null)
            {
                List<global::Doroti.Ui.TextBox> boxes = paragraph.getBoxesForSelection(new TextSelection(baseOffset: range.start, extentOffset: range.end), boxHeightStyle: BoxHeightStyle.max);
                if (checked((long)boxes.Count) != 0)
                {
                    _cachedBoundingBoxes = new List<global::Doroti.Ui.Rect>();
                    foreach (var textBox in boxes)
                    {
                        _cachedBoundingBoxes!.Add(textBox.toRect());
                    }
                }
                else
                {
                    global::Doroti.Ui.Offset offsetLocal = paragraph._getOffsetForPosition(new global::Doroti.Ui.TextPosition(offset: range.start));
                    var rect = Rect.fromPoints(offsetLocal, offsetLocal.translate(0, -paragraph._textPainter.preferredLineHeight));
                    _cachedBoundingBoxes = new List<global::Doroti.Ui.Rect> { rect };
                }
            }
            return _cachedBoundingBoxes!;
        }
    }
    internal virtual global::Doroti.Ui.Rect _rect
    {
        get
        {
            if (_cachedRect is null)
            {
                List<global::Doroti.Ui.TextBox> boxes = paragraph.getBoxesForSelection(new TextSelection(baseOffset: range.start, extentOffset: range.end), boxHeightStyle: BoxHeightStyle.max);
                if (checked((long)boxes.Count) != 0)
                {
                    global::Doroti.Ui.Rect result = boxes.First().toRect();
                    for (var index = 1L; index < checked(boxes.Count); index += 1L)
                    {
                        result = result.expandToInclude(boxes[(int)index].toRect());
                    }
                    _cachedRect = result;
                }
                else
                {
                    global::Doroti.Ui.Offset offsetLocal = paragraph._getOffsetForPosition(new global::Doroti.Ui.TextPosition(offset: range.start));
                    _cachedRect = Rect.fromPoints(offsetLocal, offsetLocal.translate(0, -paragraph._textPainter.preferredLineHeight));
                }
            }
            return DartRuntimePrimitives.RequireValue(_cachedRect);
        }
    }
    public virtual void didChangeParagraphLayout()
    {
        _cachedRect = null;
        _cachedBoundingBoxes = null;
    }

    public virtual long contentLength => range.end - range.start;
    public virtual Size size
    {
        get
        {
            return _rect.size;
        }
    }
    public virtual void paintSelection(PaintingContext context, Offset offset)
    {
        if ((_textSelectionStart is null) || (_textSelectionEnd is null))
        {
            return;
        }
        if (paragraph.selectionColor is not null)
        {
            var selection = new TextSelection(baseOffset: _textSelectionStart!.offset, extentOffset: _textSelectionEnd!.offset);
            var selectionPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.style = PaintingStyle.fill;
    __cascade.color = paragraph.selectionColor!;
    return __cascade;
}))();
            foreach (global::Doroti.Ui.TextBox textBox in paragraph.getBoxesForSelection(selection))
            {
                context.canvas.drawRect(textBox.toRect().shift(offset), selectionPaint);
            }
        }
    }

    public virtual void paintHandles(PaintingContext context, Offset offset)
    {
        if ((_textSelectionStart is null) || (_textSelectionEnd is null))
        {
            return;
        }
        if ((_startHandleLayerLink is not null) && (value.startSelectionPoint is not null))
        {
            context.pushLayer(new LeaderLayer(link: _startHandleLayerLink!, offset: offset + value.startSelectionPoint!.localPosition), (context, offset) =>
            {
            }, Offset.zero);
        }
        if ((_endHandleLayerLink is not null) && (value.endSelectionPoint is not null))
        {
            context.pushLayer(new LeaderLayer(link: _endHandleLayerLink!, offset: offset + value.endSelectionPoint!.localPosition), (context, offset) =>
            {
            }, Offset.zero);
        }
    }

    public virtual TextSelection getLineAtOffset(TextPosition position)
    {
        global::Doroti.Ui.TextRange line = paragraph._getLineAtOffset(position);
        long startLocal = line.start.clamp(range.start, range.end);
        long endLocal = line.end.clamp(range.start, range.end);
        return new TextSelection(baseOffset: startLocal, extentOffset: endLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.TextPosition getTextPositionAbove(TextPosition position)
    {
        return _clampTextPosition(paragraph._getTextPositionAbove(position));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.TextPosition getTextPositionBelow(TextPosition position)
    {
        return _clampTextPosition(paragraph._getTextPositionBelow(position));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.TextRange getWordBoundary(TextPosition position) => paragraph.getWordBoundary(position);
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<string>("textInsideRange", range.textInside(fullText)));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.TextRange>("range", range));
        properties.add(new DiagnosticsProperty<string>("fullText", fullText));
    }

}

