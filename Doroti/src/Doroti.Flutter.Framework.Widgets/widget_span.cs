// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/widget_span.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public class WidgetSpan : global::Doroti.Generated.Framework.Painting.PlaceholderSpan
{
    public virtual Widget child { get; private set; } = default!;

    public WidgetSpan(Widget child, PlaceholderAlignment alignment = PlaceholderAlignment.bottom, TextBaseline? baseline = null, global::Doroti.Generated.Framework.Painting.TextStyle? style = null) : base(alignment: alignment, baseline: DartRuntimePrimitives.RequireValue(baseline), style: style)
    {
        this.child = child;
        System.Diagnostics.Debug.Assert(((baseline is not null) || !(((DartRuntimePrimitives.Identical(alignment, PlaceholderAlignment.aboveBaseline) || DartRuntimePrimitives.Identical(alignment, PlaceholderAlignment.belowBaseline)) || DartRuntimePrimitives.Identical(alignment, PlaceholderAlignment.baseline)))));
    }

    public static List<Widget> extractFromInlineSpan(global::Doroti.Generated.Framework.Painting.InlineSpan span, global::Doroti.Generated.Framework.Painting.TextScaler textScaler)
    {
        var widgets__3468 = new List<Widget>();
        var fontSizeStack__3617 = new List<double> { global::Doroti.Generated.Framework.Painting.Text_painterLibrary.kDefaultFontSize };
        var index__3669 = 0L;
        bool visitSubtree(global::Doroti.Generated.Framework.Painting.InlineSpan span)
        {
            double? fontSizeToPush__3823 = (((global::Doroti.Generated.Framework.Painting.InlineSpan)span).style?.fontSize switch { double size__3893 when ((size__3893 != fontSizeStack__3617.Last())) => size__3893, _ => DartRuntimePrimitives.ConvertValue<double>(null) });
            if ((fontSizeToPush__3823 is not null))
            {
                double fontSizeToPush__3823__value3977 = DartRuntimePrimitives.RequireValue(fontSizeToPush__3823);
                fontSizeStack__3617.Add(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(fontSizeToPush__3823__value3977)));
            }
            if ((span is WidgetSpan))
            {
                WidgetSpan span__as4064 = (WidgetSpan)span;
                double fontSize__4107 = fontSizeStack__3617.Last();
                double textScaleFactor__4159 = ((fontSize__4107 == 0L) ? 0 : (textScaler.scale(fontSize__4107) / fontSize__4107));
                widgets__3468.Add(new _WidgetSpanParentData__widget_span(span: ((WidgetSpan)span__as4064), child: new Semantics(tagForChildren: new global::Doroti.Generated.Framework.Rendering.PlaceholderSpanIndexSemanticsTag(index__3669++), child: new _AutoScaleInlineWidget__widget_span(span: ((WidgetSpan)span__as4064), textScaleFactor: textScaleFactor__4159, child: ((WidgetSpan)((WidgetSpan)span__as4064)).child))));
            }
            DartRuntimePrimitives.Assert(() => ((span is WidgetSpan) || (span is not global::Doroti.Generated.Framework.Painting.PlaceholderSpan)), () => (object?)$"{(((global::Doroti.Generated.Framework.Painting.PlaceholderSpan)span))} is a PlaceholderSpan but not a WidgetSpan subclass. This is currently not supported.");
            span.visitDirectChildren((global::System.Func<global::Doroti.Generated.Framework.Painting.InlineSpan, bool>)visitSubtree);
            if ((fontSizeToPush__3823 is not null))
            {
                double fontSizeToPush__3823__value4876 = DartRuntimePrimitives.RequireValue(fontSizeToPush__3823);
                double poppedFontSize__4923 = fontSizeStack__3617.removeLast<double>();
                DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(fontSizeStack__3617));
                DartRuntimePrimitives.Assert(() => (poppedFontSize__4923 == DartRuntimePrimitives.RequireValue(fontSizeToPush__3823__value4876)));
            }
            return true;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        visitSubtree(span);
        return widgets__3468;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void build(ParagraphBuilder builder, global::Doroti.Generated.Framework.Painting.TextScaler textScaler = default!, List<global::Doroti.Generated.Framework.Painting.PlaceholderDimensions>? dimensions = null)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        DartRuntimePrimitives.Assert(() => (dimensions is not null));
        var hasStyle__5843 = (this.style is not null);
        if (hasStyle__5843)
        {
            builder.pushStyle(this.style!.getTextStyle(textScaler: textScaler));
        }
        DartRuntimePrimitives.Assert(() => (builder.placeholderCount < checked((long)(dimensions!.Count))));
        global::Doroti.Generated.Framework.Painting.PlaceholderDimensions currentDimensions__6056 = dimensions![(int)(builder.placeholderCount)];
        builder.addPlaceholder(((global::Doroti.Generated.Framework.Painting.PlaceholderDimensions)currentDimensions__6056).size.width, ((global::Doroti.Generated.Framework.Painting.PlaceholderDimensions)currentDimensions__6056).size.height, this.alignment, baseline: ((global::Doroti.Generated.Framework.Painting.PlaceholderDimensions)currentDimensions__6056).baseline, baselineOffset: ((global::Doroti.Generated.Framework.Painting.PlaceholderDimensions)currentDimensions__6056).baselineOffset);
        if (hasStyle__5843)
        {
            builder.pop();
        }
    }

    public override bool visitChildren(global::System.Func<global::Doroti.Generated.Framework.Painting.InlineSpan, bool> visitor) => visitor(this);
    public override bool visitDirectChildren(global::System.Func<global::Doroti.Generated.Framework.Painting.InlineSpan, bool> visitor) => true;
    public override global::Doroti.Generated.Framework.Painting.InlineSpan? getSpanForPositionVisitor(TextPosition position, global::Doroti.Generated.Framework.Painting.Accumulator offset)
    {
        if ((position.offset == ((global::Doroti.Generated.Framework.Painting.Accumulator)offset).value))
        {
            return ((global::Doroti.Generated.Framework.Painting.InlineSpan?)(object?)this);
        }
        offset.increment(1L);
        return ((global::Doroti.Generated.Framework.Painting.InlineSpan)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long? codeUnitAtVisitor(long index, global::Doroti.Generated.Framework.Painting.Accumulator offset)
    {
        long localOffset__6924 = (index - ((global::Doroti.Generated.Framework.Painting.Accumulator)offset).value);
        DartRuntimePrimitives.Assert(() => (localOffset__6924 >= 0L));
        offset.increment(1L);
        return ((localOffset__6924 == 0L) ? global::Doroti.Generated.Framework.Painting.PlaceholderSpan.placeholderCodeUnit : null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Painting.RenderComparison compareTo(global::Doroti.Generated.Framework.Painting.InlineSpan other)
    {
        if (DartRuntimePrimitives.Identical(this, other))
        {
            return global::Doroti.Generated.Framework.Painting.RenderComparison.identical;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(other), this.GetType())))
        {
            return global::Doroti.Generated.Framework.Painting.RenderComparison.layout;
        }
        if ((((this.style is null)) != ((((global::Doroti.Generated.Framework.Painting.InlineSpan)other).style is null))))
        {
            return global::Doroti.Generated.Framework.Painting.RenderComparison.layout;
        }
        var typedOther__7430 = ((WidgetSpan?)(object?)other)!;
        if (((!object.Equals(this.child, ((WidgetSpan)typedOther__7430).child)) || (!object.Equals(this.alignment, typedOther__7430.alignment))))
        {
            return global::Doroti.Generated.Framework.Painting.RenderComparison.layout;
        }
        global::Doroti.Generated.Framework.Painting.RenderComparison result__7603 = global::Doroti.Generated.Framework.Painting.RenderComparison.identical;
        if ((this.style is not null))
        {
            global::Doroti.Generated.Framework.Painting.RenderComparison candidate__7694 = this.style!.compareTo(((WidgetSpan)other).style!);
            if ((FoundationRuntimePorts.EnumIndex(candidate__7694) > FoundationRuntimePorts.EnumIndex(result__7603)))
            {
                result__7603 = candidate__7694;
            }
            if ((object.Equals(result__7603, global::Doroti.Generated.Framework.Painting.RenderComparison.layout)))
            {
                return result__7603;
            }
        }
        return result__7603;
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
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        if (!base.Equals(__other))
        {
            return false;
        }
        return ((((__other is WidgetSpan) && (object.Equals(((WidgetSpan)((WidgetSpan)__other)).child, this.child))) && (object.Equals(((WidgetSpan)__other).alignment, this.alignment))) && (object.Equals(((WidgetSpan)__other).baseline, this.baseline)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(base.GetHashCode(), this.child, this.alignment, this.baseline));
    public override global::Doroti.Generated.Framework.Painting.InlineSpan? getSpanForPosition(TextPosition position)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        return ((global::Doroti.Generated.Framework.Painting.InlineSpan)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool debugAssertIsValid()
    {
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<Widget>("widget", this.child));
    }

}

internal class _WidgetSpanParentData__widget_span : ParentDataWidget<global::Doroti.Generated.Framework.Rendering.TextParentData>
{
    public virtual WidgetSpan span { get; private set; } = default!;

    internal _WidgetSpanParentData__widget_span(WidgetSpan span, Widget child) : base(child: child)
    {
        this.span = span;
    }

    public override void applyParentData(global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var parentData__9510 = ((global::Doroti.Generated.Framework.Rendering.TextParentData?)(object?)((global::Doroti.Generated.Framework.Rendering.RenderObject)renderObject).parentData!)!;
        parentData__9510.span = this.span;
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

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderScaledInlineWidget__widget_span(this.span.alignment, this.span.baseline, this.textScaleFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderScaledInlineWidget__widget_span)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderScaledInlineWidget__widget_span>)(() =>
{            var __cascade = __renderObject;
            __cascade.alignment = this.span.alignment;
            __cascade.baseline = this.span.baseline;
            __cascade.scale = this.textScaleFactor;
            return __cascade;        }))());
    }

}

public class _RenderScaledInlineWidget__widget_span : global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.RenderObjectWithChildMixin<global::Doroti.Generated.Framework.Rendering.RenderBox>
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
        get => this._scale;
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._scale))
            {
                return;
            }
            DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.RequireValue(__value) > 0L));
            DartRuntimePrimitives.Assert(() => double.IsFinite(DartRuntimePrimitives.RequireValue(__value)));
            _scale = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Flutter.Ui.PlaceholderAlignment alignment
    {
        get => this._alignment;
        set
        {
            var __value = value;
            if ((object.Equals(this._alignment, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _alignment = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Flutter.Ui.TextBaseline? baseline
    {
        get => this._baseline;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._baseline)))
            {
                return;
            }
            _baseline = __value;
            markNeedsLayout();
        }
    }
    public override double computeMaxIntrinsicHeight(double width)
    {
        return (((this.child?.getMaxIntrinsicHeight((width / this.scale)) ?? 0.0)) * this.scale);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return (((this.child?.getMaxIntrinsicWidth((height / this.scale)) ?? 0.0)) * this.scale);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return (((this.child?.getMinIntrinsicHeight((width / this.scale)) ?? 0.0)) * this.scale);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return (((this.child?.getMinIntrinsicWidth((height / this.scale)) ?? 0.0)) * this.scale);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        return (this.child?.getDistanceToActualBaseline(DartRuntimePrimitives.RequireValue(baseline)) switch { null => base.computeDistanceToActualBaseline(DartRuntimePrimitives.RequireValue(baseline)), double childBaseline__12209 => (this.scale * childBaseline__12209) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        double? distance__12373 = this.child?.getDryBaseline(new global::Doroti.Generated.Framework.Rendering.BoxConstraints(maxWidth: (((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxWidth / this.scale)), DartRuntimePrimitives.RequireValue(baseline));
        return ((distance__12373 is null) ? null : (this.scale * DartRuntimePrimitives.RequireValue(distance__12373)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => !((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).hasBoundedHeight);
        global::Doroti.Flutter.Ui.Size unscaledSize__12676 = ((global::Doroti.Flutter.Ui.Size)(object?)(this.child?.getDryLayout(new global::Doroti.Generated.Framework.Rendering.BoxConstraints(maxWidth: (((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxWidth / this.scale))) ?? Size.zero));
        return constraints.constrain((unscaledSize__12676 * this.scale));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__12908 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((child__12908 is null))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => !((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).hasBoundedHeight);
        child__12908.layout(new global::Doroti.Generated.Framework.Rendering.BoxConstraints(maxWidth: (((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).maxWidth / this.scale)), parentUsesSize: true);
        size = this.constraints.constrain((((global::Doroti.Generated.Framework.Rendering.RenderBox)child__12908).size * this.scale));
    }

    public override void applyPaintTransform(global::Doroti.Generated.Framework.Rendering.RenderObject child, Matrix4 transform)
    {
        var __child = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)child;
        transform.scaleByDouble(this.scale, this.scale, this.scale, 1);
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__13538 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((child__13538 is null))
        {
            layer = null;
            return;
        }
        if ((this.scale == 1.0))
        {
            context.paintChild(child__13538, offset);
            layer = null;
            return;
        }
        layer = context.pushTransform(this.needsCompositing, offset, Matrix4.diagonal3Values(this.scale, this.scale, 1.0), ((global::System.Action<global::Doroti.Generated.Framework.Rendering.PaintingContext, Offset>)((context, offset) => { context.paintChild(child__13538, offset); })), oldLayer: ((global::Doroti.Generated.Framework.Rendering.TransformLayer?)(object?)this.layer)!);
    }

    public override bool hitTestChildren(global::Doroti.Generated.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__14101 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((child__14101 is null))
        {
            return false;
        }
        return result.addWithPaintTransform(transform: Matrix4.diagonal3Values(this.scale, this.scale, 1.0), position: position, hitTest: ((global::System.Func<global::Doroti.Generated.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, transformedOffset) => child__14101.hitTest(result, position: transformedOffset))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((child is not RenderBox))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new global::Doroti.Generated.Framework.Foundation.ErrorSpacer(), new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderBox)} child was created by", this.debugCreator, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Generated.Framework.Foundation.ErrorSpacer(), new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((dynamic)child).debugCreator, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? child
    {
        get => this._child;
        set
        {
            var __value = value;
            if ((this._child is not null))
            {
                dropChild(this._child!);
            }
            this._child = __value;
            if ((this._child is not null))
            {
                adoptChild(this._child!);
            }
        }
    }
    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        ((dynamic)this._child)?.attach(owner);
    }

    public override void detach()
    {
        base.detach();
        ((dynamic)this._child)?.detach();
    }

    public override void redepthChildren()
    {
        if ((this._child is not null))
        {
            redepthChild(this._child!);
        }
    }

    public override void visitChildren(global::System.Action<RenderObject> visitor)
    {
        if ((this._child is not null))
        {
            visitor(this._child!);
        }
    }

    public override List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        return ((this.child is not null) ? new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { ((Diagnosticable)this.child!).toDiagnosticsNode(name: "child") } : new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

