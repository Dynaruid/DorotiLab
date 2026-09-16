// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/widget_span.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class WidgetSpan : PlaceholderSpan
{
    public virtual Widget child { get; private set; } = default!;

    public WidgetSpan(Widget child, PlaceholderAlignment alignment = PlaceholderAlignment.bottom, TextBaseline? baseline = null, TextStyle? style = null) : base(alignment: alignment, baseline: DartRuntimePrimitives.RequireValue(baseline), style: style)
    {
        this.child = child;
        System.Diagnostics.Debug.Assert((baseline is not null) || !(DartRuntimePrimitives.Identical(alignment, PlaceholderAlignment.aboveBaseline) || DartRuntimePrimitives.Identical(alignment, PlaceholderAlignment.belowBaseline) || DartRuntimePrimitives.Identical(alignment, PlaceholderAlignment.baseline)));
    }

    public static List<Widget> extractFromInlineSpan(InlineSpan span, TextScaler textScaler)
    {
        var widgets = new List<Widget>();
        var fontSizeStack = new List<double> { Text_painterLibrary.kDefaultFontSize };
        var index = 0L;
        bool visitSubtree(InlineSpan span)
        {
            double? fontSizeToPush = span.style?.fontSize switch { double size when size != fontSizeStack.Last() => size, _ => DartRuntimePrimitives.ConvertValue<double>(null) };
            if (fontSizeToPush is not null)
            {
                double fontSizeToPush__3823__value3977 = DartRuntimePrimitives.RequireValue(fontSizeToPush);
                fontSizeStack.Add(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(fontSizeToPush__3823__value3977)));
            }
            if (span is WidgetSpan)
            {
                WidgetSpan span__as4064 = (WidgetSpan)span;
                double fontSizeLocal = fontSizeStack.Last();
                double textScaleFactorLocal = (fontSizeLocal == 0L) ? 0 : (textScaler.scale(fontSizeLocal) / fontSizeLocal);
                widgets.Add(new _WidgetSpanParentData__widget_span(span: span__as4064, child: new Semantics(tagForChildren: new PlaceholderSpanIndexSemanticsTag(index++), child: new _AutoScaleInlineWidget__widget_span(span: span__as4064, textScaleFactor: textScaleFactorLocal, child: span__as4064.child))));
            }
            DartRuntimePrimitives.Assert(() => (span is WidgetSpan) || (span is not PlaceholderSpan), () => (object?)$"{((PlaceholderSpan)span)} is a PlaceholderSpan but not a WidgetSpan subclass. This is currently not supported.");
            span.visitDirectChildren(visitSubtree);
            if (fontSizeToPush is not null)
            {
                double fontSizeToPush__3823__value4876 = DartRuntimePrimitives.RequireValue(fontSizeToPush);
                double poppedFontSize = fontSizeStack.removeLast();
                DartRuntimePrimitives.Assert(() => Enumerable.Any(fontSizeStack));
                DartRuntimePrimitives.Assert(() => poppedFontSize == DartRuntimePrimitives.RequireValue(fontSizeToPush__3823__value4876));
            }
            return true;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        visitSubtree(span);
        return widgets;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void build(ParagraphBuilder builder, TextScaler textScaler = default!, List<PlaceholderDimensions>? dimensions = null)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        DartRuntimePrimitives.Assert(() => dimensions is not null);
        var hasStyle = style is not null;
        if (hasStyle)
        {
            builder.pushStyle(style!.getTextStyle(textScaler: textScaler));
        }
        DartRuntimePrimitives.Assert(() => builder.placeholderCount < checked(dimensions!.Count));
        PlaceholderDimensions currentDimensions = dimensions![(int)builder.placeholderCount];
        builder.addPlaceholder(currentDimensions.size.width, currentDimensions.size.height, alignment, baseline: currentDimensions.baseline, baselineOffset: currentDimensions.baselineOffset);
        if (hasStyle)
        {
            builder.pop();
        }
    }

    public override bool visitChildren(Func<InlineSpan, bool> visitor) => visitor(this);
    public override bool visitDirectChildren(Func<InlineSpan, bool> visitor) => true;
    public override InlineSpan? getSpanForPositionVisitor(TextPosition position, Accumulator offset)
    {
        if (position.offset == offset.value)
        {
            return (InlineSpan?)this;
        }
        offset.increment(1L);
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long? codeUnitAtVisitor(long index, Accumulator offset)
    {
        long localOffset = index - offset.value;
        DartRuntimePrimitives.Assert(() => localOffset >= 0L);
        offset.increment(1L);
        return (localOffset == 0L) ? global::Doroti.Framework.Painting.PlaceholderSpan.placeholderCodeUnit : null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RenderComparison compareTo(InlineSpan other)
    {
        if (DartRuntimePrimitives.Identical(this, other))
        {
            return RenderComparison.identical;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(other), GetType()))
        {
            return RenderComparison.layout;
        }
        if (style is null != other.style is null)
        {
            return RenderComparison.layout;
        }
        var typedOther = ((WidgetSpan?)other)!;
        if ((!Equals(child, typedOther.child)) || (!Equals(alignment, typedOther.alignment)))
        {
            return RenderComparison.layout;
        }
        RenderComparison result = RenderComparison.identical;
        if (style is not null)
        {
            RenderComparison candidate = style!.compareTo(((WidgetSpan)other).style!);
            if (FoundationRuntimePorts.EnumIndex(candidate) > FoundationRuntimePorts.EnumIndex(result))
            {
                result = candidate;
            }
            if (Equals(result, RenderComparison.layout))
            {
                return result;
            }
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as WidgetSpan;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        if (!base.Equals(__other))
        {
            return false;
        }
        return (__other is WidgetSpan) && Equals(__other.child, child) && Equals(__other.alignment, alignment) && Equals(__other.baseline, baseline);
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(base.GetHashCode(), child, alignment, baseline));
    public override InlineSpan? getSpanForPosition(TextPosition position)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool debugAssertIsValid()
    {
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Widget>("widget", child));
    }

}

internal class _WidgetSpanParentData__widget_span : ParentDataWidget<TextParentData>
{
    public virtual WidgetSpan span { get; private set; } = default!;

    internal _WidgetSpanParentData__widget_span(WidgetSpan span, Widget child) : base(child: child)
    {
        this.span = span;
    }

    public override void applyParentData(RenderObject renderObject)
    {
        var parentDataLocal = ((TextParentData?)renderObject.parentData!)!;
        parentDataLocal.span = span;
    }

    public override Type debugTypicalAncestorWidgetClass => typeof(RichText);
}

internal class _AutoScaleInlineWidget__widget_span : SingleChildRenderObjectWidget
{
    public virtual WidgetSpan span { get; private set; } = default!;
    public virtual double textScaleFactor { get; private set; } = default!;

    internal _AutoScaleInlineWidget__widget_span(WidgetSpan span, double textScaleFactor, Widget? child) : base(child: child)
    {
        this.span = span;
        this.textScaleFactor = textScaleFactor;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderScaledInlineWidget__widget_span(span.alignment, span.baseline, textScaleFactor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderScaledInlineWidget__widget_span)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderScaledInlineWidget__widget_span>)(() =>
{
    var __cascade = __renderObject;
    __cascade.alignment = span.alignment;
    __cascade.baseline = span.baseline;
    __cascade.scale = textScaleFactor;
    return __cascade;
}))());
    }

}

public class _RenderScaledInlineWidget__widget_span : RenderBox, RenderObjectWithChildMixin<RenderBox>
{
    internal virtual double _scale { get; set; } = default!;
    internal virtual PlaceholderAlignment _alignment { get; set; } = default!;
    internal virtual TextBaseline? _baseline { get; set; } = default;
    public virtual RenderBox? _child { get; set; } = default;

    internal _RenderScaledInlineWidget__widget_span(PlaceholderAlignment _alignment, TextBaseline? _baseline, double _scale)
    {
        this._alignment = _alignment;
        this._baseline = _baseline;
        this._scale = _scale;
    }

    public virtual double scale
    {
        get => _scale;
        set
        {
            var __value = value;
            if (DartRuntimePrimitives.RequireValue(__value) == _scale)
            {
                return;
            }
            DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.RequireValue(__value) > 0L);
            DartRuntimePrimitives.Assert(() => double.IsFinite(DartRuntimePrimitives.RequireValue(__value)));
            _scale = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual PlaceholderAlignment alignment
    {
        get => _alignment;
        set
        {
            var __value = value;
            if (Equals(_alignment, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _alignment = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual TextBaseline? baseline
    {
        get => _baseline;
        set
        {
            var __value = value;
            if (Equals(__value, _baseline))
            {
                return;
            }
            _baseline = __value;
            markNeedsLayout();
        }
    }
    public override double computeMaxIntrinsicHeight(double width)
    {
        return (child?.getMaxIntrinsicHeight(width / scale) ?? 0.0) * scale;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return (child?.getMaxIntrinsicWidth(height / scale) ?? 0.0) * scale;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return (child?.getMinIntrinsicHeight(width / scale) ?? 0.0) * scale;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return (child?.getMinIntrinsicWidth(height / scale) ?? 0.0) * scale;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        return child?.getDistanceToActualBaseline(DartRuntimePrimitives.RequireValue(baseline)) switch { null => base.computeDistanceToActualBaseline(DartRuntimePrimitives.RequireValue(baseline)), double childBaseline => scale * childBaseline };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        double? distance = child?.getDryBaseline(new BoxConstraints(maxWidth: constraints.maxWidth / scale), DartRuntimePrimitives.RequireValue(baseline));
        return (distance is null) ? null : (scale * DartRuntimePrimitives.RequireValue(distance));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => !constraints.hasBoundedHeight);
        Size unscaledSize = child?.getDryLayout(new BoxConstraints(maxWidth: constraints.maxWidth / scale)) ?? Size.zero;
        return constraints.constrain(unscaledSize * scale);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => !constraints.hasBoundedHeight);
        childLocal.layout(new BoxConstraints(maxWidth: constraints.maxWidth / scale), parentUsesSize: true);
        size = constraints.constrain(childLocal.size * scale);
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)child;
        transform.scaleByDouble(scale, scale, scale, 1);
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            layer = null;
            return;
        }
        if (scale == 1.0)
        {
            context.paintChild(childLocal, offset);
            layer = null;
            return;
        }
        layer = context.pushTransform(needsCompositing, offset, Matrix4.diagonal3Values(scale, scale, 1.0), (context, offset) => { context.paintChild(childLocal, offset); }, oldLayer: ((TransformLayer?)layer)!);
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return false;
        }
        return result.addWithPaintTransform(transform: Matrix4.diagonal3Values(scale, scale, 1.0), position: position, hitTest: (result, transformedOffset) => childLocal.hitTest(result, position: transformedOffset));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (child is not RenderBox)
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {GetType()} that expected a {typeof(RenderBox)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", child.debugCreator, style: DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? child
    {
        get => _child;
        set
        {
            var __value = value;
            if (_child is not null)
            {
                dropChild(_child!);
            }
            _child = __value;
            if (_child is not null)
            {
                adoptChild(_child!);
            }
        }
    }
    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        _child?.attach(owner);
    }

    public override void detach()
    {
        base.detach();
        _child?.detach();
    }

    public override void redepthChildren()
    {
        if (_child is not null)
        {
            redepthChild(_child!);
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        if (_child is not null)
        {
            visitor(_child!);
        }
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        return (child is not null) ? new List<DiagnosticsNode> { ((Diagnosticable)child!).toDiagnosticsNode(name: "child") } : new List<DiagnosticsNode>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

