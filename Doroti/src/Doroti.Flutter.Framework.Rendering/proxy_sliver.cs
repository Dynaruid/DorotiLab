// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/proxy_sliver.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Rendering;

public abstract class RenderProxySliver : RenderSliver, RenderObjectWithChildMixin<RenderSliver>
{
    public virtual RenderSliver? _child { get; set; } = default;

    protected RenderProxySliver(RenderSliver? child = null)
    {
    }

    public override Rect semanticBounds
    {
        get
        {
            if ((child is not null))
            {
                return child!.semanticBounds;
            }
            return base.semanticBounds;
            return default!;
        }
    }
    public override void setupParentData(RenderObject child)
    {
        if ((((RenderObject)child).parentData is not SliverPhysicalParentData))
        {
            child.parentData = new SliverPhysicalParentData();
        }
    }

    public override void performLayout()
    {
        DartRuntimePrimitives.Assert(() => (child is not null));
        child!.layout(constraints, parentUsesSize: true);
        geometry = child!.geometry;
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((child is not null))
        {
            context.paintChild(child!, offset);
        }
    }

    public override bool hitTestChildren(SliverHitTestResult result, double mainAxisPosition, double crossAxisPosition)
    {
        return (((child is not null) && (child!.geometry!.hitTestExtent > 0L)) && child!.hitTest(result, mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double childMainAxisPosition(RenderObject child)
    {
        var __child = (RenderSliver)(object)child;
        DartRuntimePrimitives.Assert(() => (object.Equals(__child, this.child)));
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var childParentData__2838 = ((SliverPhysicalParentData?)(object?)((RenderObject)child).parentData!)!;
        childParentData__2838.applyPaintTransform(transform);
    }

    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((child is not RenderSliver))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderSliver)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderSliver)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((RenderObject)child).debugCreator, style: DiagnosticsTreeStyle.errorProperty) });
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderSliver? child
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
        this._child?.attach(owner);
    }

    public override void detach()
    {
        base.detach();
        this._child?.detach();
    }

    public override void redepthChildren()
    {
        if ((this._child is not null))
        {
            redepthChild(this._child!);
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        if ((this._child is not null))
        {
            visitor(this._child!);
        }
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        return ((this.child is not null) ? new List<DiagnosticsNode> { ((Diagnosticable)this.child!).toDiagnosticsNode(name: "child") } : new List<DiagnosticsNode>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RenderSliverOpacity : RenderProxySliver
{
    internal virtual long _alpha { get; set; } = default!;
    internal virtual double _opacity { get; set; } = default!;
    internal virtual bool _alwaysIncludeSemantics { get; set; } = default!;

    public RenderSliverOpacity(double opacity = 1.0, bool alwaysIncludeSemantics = false, RenderSliver? sliver = null)
    {
        this._opacity = opacity;
        this._alwaysIncludeSemantics = alwaysIncludeSemantics;
        this._alpha = Dart_uiLibrary.Color.getAlphaFromOpacity(opacity);
        System.Diagnostics.Debug.Assert(((opacity >= 0.0) && (opacity <= 1.0)));
    }

    public override bool alwaysNeedsCompositing => ((child is not null) && ((this._alpha > 0L)));
    public virtual double opacity
    {
        get => this._opacity;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((__value >= 0.0) && (__value <= 1.0)));
            if ((this._opacity == __value))
            {
                return;
            }
            bool didNeedCompositing__4614 = this.alwaysNeedsCompositing;
            var wasVisible__4669 = (this._alpha != 0L);
            _opacity = __value;
            _alpha = Dart_uiLibrary.Color.getAlphaFromOpacity(this._opacity);
            if ((didNeedCompositing__4614 != this.alwaysNeedsCompositing))
            {
                markNeedsCompositingBitsUpdate();
            }
            markNeedsPaint();
            if (((wasVisible__4669 != ((this._alpha != 0L))) && !this.alwaysIncludeSemantics))
            {
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual bool alwaysIncludeSemantics
    {
        get => this._alwaysIncludeSemantics;
        set
        {
            var __value = value;
            if ((__value == this._alwaysIncludeSemantics))
            {
                return;
            }
            _alwaysIncludeSemantics = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public override void paint(PaintingContext context, Offset offset)
    {
        if (((child is not null) && child!.geometry!.visible))
        {
            if ((this._alpha == 0L))
            {
                layer = null;
                return;
            }
            DartRuntimePrimitives.Assert(() => needsCompositing);
            layer = context.pushOpacity(offset, this._alpha, (Action<PaintingContext, Offset>)base.paint, oldLayer: ((OpacityLayer?)(object?)layer)!);
            DartRuntimePrimitives.Assert(() =>
                {
                    layer!.debugCreator = debugCreator;
                    return true;
                });
        }
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if (((child is not null) && (((this._alpha != 0L) || this.alwaysIncludeSemantics))))
        {
            visitor(child!);
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("opacity", this.opacity));
        properties.add(new FlagProperty("alwaysIncludeSemantics", value: this.alwaysIncludeSemantics, ifTrue: "alwaysIncludeSemantics"));
    }

}

public class RenderSliverIgnorePointer : RenderProxySliver
{
    internal virtual bool _ignoring { get; set; } = default!;
    internal virtual bool? _ignoringSemantics { get; set; } = default;

    public RenderSliverIgnorePointer(RenderSliver? sliver = null, bool ignoring = true, bool? ignoringSemantics = null)
    {
        this._ignoring = ignoring;
        this._ignoringSemantics = ignoringSemantics;
    }

    public virtual bool ignoring
    {
        get => this._ignoring;
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._ignoring))
            {
                return;
            }
            _ignoring = DartRuntimePrimitives.RequireValue(__value);
            if ((this.ignoringSemantics is null))
            {
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual bool? ignoringSemantics
    {
        get => this._ignoringSemantics;
        set
        {
            var __value = value;
            if ((__value == this._ignoringSemantics))
            {
                return;
            }
            _ignoringSemantics = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public override bool hitTest(SliverHitTestResult result, double mainAxisPosition, double crossAxisPosition)
    {
        return (!this.ignoring && base.hitTest(result, mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if ((this._ignoringSemantics ?? false))
        {
            return;
        }
        base.visitChildrenForSemantics((Action<RenderObject>)visitor);
    }

    public override void describeSemanticsConfiguration(global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isBlockingUserActions = (this.ignoring && ((this._ignoringSemantics ?? true)));
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("ignoring", this.ignoring));
        properties.add(new DiagnosticsProperty<bool>("ignoringSemantics", this.ignoringSemantics, description: ((this.ignoringSemantics is null) ? null : $"implicitly {this.ignoringSemantics}")));
    }

}

public class RenderSliverOffstage : RenderProxySliver
{
    internal virtual bool _offstage { get; set; } = default!;

    public RenderSliverOffstage(bool offstage = true, RenderSliver? sliver = null)
    {
        this._offstage = offstage;
    }

    public virtual bool offstage
    {
        get => this._offstage;
        set
        {
            var __value = value;
            if ((__value == this._offstage))
            {
                return;
            }
            _offstage = __value;
            markNeedsLayoutForSizedByParentChange();
        }
    }
    public override void performLayout()
    {
        DartRuntimePrimitives.Assert(() => (child is not null));
        child!.layout(constraints, parentUsesSize: true);
        if (!this.offstage)
        {
            geometry = child!.geometry;
        }
        else
        {
            geometry = SliverGeometry.zero;
        }
    }

    public override bool hitTest(SliverHitTestResult result, double mainAxisPosition, double crossAxisPosition)
    {
        return (!this.offstage && base.hitTest(result, mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(SliverHitTestResult result, double mainAxisPosition, double crossAxisPosition)
    {
        return (((!this.offstage && (child is not null)) && (child!.geometry!.hitTestExtent > 0L)) && child!.hitTest(result, mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (this.offstage)
        {
            return;
        }
        context.paintChild(child!, offset);
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if (this.offstage)
        {
            return;
        }
        base.visitChildrenForSemantics((Action<RenderObject>)visitor);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("offstage", this.offstage));
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        if ((child is null))
        {
            return new List<DiagnosticsNode>();
        }
        return new List<DiagnosticsNode> { ((Diagnosticable)child!).toDiagnosticsNode(name: "child", style: (this.offstage ? DiagnosticsTreeStyle.offstage : DiagnosticsTreeStyle.sparse)) };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RenderSliverAnimatedOpacity : RenderProxySliver, RenderAnimatedOpacityMixin<RenderSliver>
{
    public virtual long? _alpha { get; set; } = default;
    public virtual bool? _currentlyIsRepaintBoundary { get; set; } = default;
    public virtual Animation<double>? _opacity { get; set; } = default;
    public virtual bool? _alwaysIncludeSemantics { get; set; } = default;

    public RenderSliverAnimatedOpacity(Animation<double> opacity, bool alwaysIncludeSemantics = false, RenderSliver? sliver = null)
    {
        this.opacity = opacity;
        this.alwaysIncludeSemantics = alwaysIncludeSemantics;
        child = sliver;
    }

    public override bool isRepaintBoundary => ((child is not null) && DartRuntimePrimitives.RequireValue(this._currentlyIsRepaintBoundary));
    public override OffsetLayer updateCompositedLayer(OffsetLayer? oldLayer)
    {
        var __oldLayer = oldLayer is null ? null : (OpacityLayer)(object)oldLayer;
        OpacityLayer updatedLayer__33945 = (__oldLayer ?? new OpacityLayer());
        updatedLayer__33945.alpha = this._alpha;
        return updatedLayer__33945;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Animation<double> opacity
    {
        get => this._opacity!;
        set
        {
            var __value = value;
            if ((object.Equals(this._opacity, __value)))
            {
                return;
            }
            if ((attached && (this._opacity is not null)))
            {
                this.opacity.removeListener(this._updateOpacity);
            }
            this._opacity = __value;
            if (attached)
            {
                this.opacity.addListener(this._updateOpacity);
            }
            _updateOpacity();
        }
    }
    public virtual bool alwaysIncludeSemantics
    {
        get => DartRuntimePrimitives.RequireValue(this._alwaysIncludeSemantics);
        set
        {
            var __value = value;
            if ((__value == this._alwaysIncludeSemantics))
            {
                return;
            }
            this._alwaysIncludeSemantics = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        this.opacity.addListener(this._updateOpacity);
        _updateOpacity();
    }

    public override void detach()
    {
        this.opacity.removeListener(this._updateOpacity);
        base.detach();
    }

    public virtual void _updateOpacity()
    {
        long? oldAlpha__35811 = this._alpha;
        this._alpha = Dart_uiLibrary.Color.getAlphaFromOpacity(this.opacity.value);
        if ((oldAlpha__35811 != this._alpha))
        {
            bool? wasRepaintBoundary__35936 = this._currentlyIsRepaintBoundary;
            this._currentlyIsRepaintBoundary = (DartRuntimePrimitives.RequireValue(this._alpha) > 0L);
            if (((child is not null) && (wasRepaintBoundary__35936 != this._currentlyIsRepaintBoundary)))
            {
                markNeedsCompositingBitsUpdate();
            }
            markNeedsCompositedLayerUpdate();
            if (((oldAlpha__35811 == 0L) || (this._alpha == 0L)))
            {
                markNeedsSemanticsUpdate();
            }
        }
    }

    public override bool paintsChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((RenderObject)child).parent, this)));
        return (this.opacity.value > 0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((this._alpha == 0L))
        {
            return;
        }
        base.paint(context, offset);
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if (((child is not null) && (((this._alpha != 0L) || this.alwaysIncludeSemantics))))
        {
            visitor(child!);
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Animation<double>>("opacity", this.opacity));
        properties.add(new FlagProperty("alwaysIncludeSemantics", value: this.alwaysIncludeSemantics, ifTrue: "alwaysIncludeSemantics"));
    }

}

public class RenderSliverConstrainedCrossAxis : RenderProxySliver
{
    internal virtual double _maxExtent { get; set; } = default!;

    public RenderSliverConstrainedCrossAxis(double maxExtent)
    {
        this._maxExtent = maxExtent;
        System.Diagnostics.Debug.Assert((maxExtent >= 0.0));
    }

    public virtual double maxExtent
    {
        get => this._maxExtent;
        set
        {
            var __value = value;
            if ((this._maxExtent == __value))
            {
                return;
            }
            _maxExtent = __value;
            markNeedsLayout();
        }
    }
    public override void performLayout()
    {
        DartRuntimePrimitives.Assert(() => (child is not null));
        DartRuntimePrimitives.Assert(() => (this.maxExtent >= 0.0));
        child!.layout(constraints.copyWith(crossAxisExtent: Math.Min(this._maxExtent, ((SliverConstraints)constraints).crossAxisExtent)), parentUsesSize: true);
        SliverGeometry childLayoutGeometry__14492 = child!.geometry!;
        geometry = childLayoutGeometry__14492.copyWith(crossAxisExtent: Math.Min(this._maxExtent, ((SliverConstraints)constraints).crossAxisExtent));
    }

}

public class RenderSliverSemanticsAnnotations : RenderProxySliver, SemanticsAnnotationsMixin
{
    public virtual global::Doroti.Generated.Framework.Semantics.SemanticsProperties _properties { get; set; } = default!;
    public virtual bool _container { get; set; } = default!;
    public virtual bool _explicitChildNodes { get; set; } = default!;
    public virtual bool _excludeSemantics { get; set; } = default!;
    public virtual bool _blockUserActions { get; set; } = default!;
    public virtual Locale? _localeForSubtree { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Semantics.AttributedString? _attributedLabel { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Semantics.AttributedString? _attributedValue { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Semantics.AttributedString? _attributedIncreasedValue { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Semantics.AttributedString? _attributedDecreasedValue { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Semantics.AttributedString? _attributedHint { get; set; } = default;
    public virtual TextDirection? _textDirection { get; set; } = default;

    public RenderSliverSemanticsAnnotations(RenderSliver? child = null, global::Doroti.Generated.Framework.Semantics.SemanticsProperties properties = default!, bool container = false, bool explicitChildNodes = false, bool excludeSemantics = false, bool blockUserActions = false, Locale? localeForSubtree = null, TextDirection? textDirection = null) : base(child)
    {
    }

    public virtual void initSemanticsAnnotations(global::Doroti.Generated.Framework.Semantics.SemanticsProperties properties, bool container, bool explicitChildNodes, bool excludeSemantics, bool blockUserActions, Locale? localeForSubtree, TextDirection? textDirection)
    {
        this._properties = properties;
        this._container = container;
        this._explicitChildNodes = explicitChildNodes;
        this._excludeSemantics = excludeSemantics;
        this._blockUserActions = blockUserActions;
        this._localeForSubtree = localeForSubtree;
        this._textDirection = textDirection;
        _updateAttributedFields(this._properties);
    }

    public virtual global::Doroti.Generated.Framework.Semantics.SemanticsProperties properties
    {
        get => this._properties;
        set
        {
            var __value = value;
            if ((object.Equals(this._properties, __value)))
            {
                return;
            }
            this._properties = __value;
            _updateAttributedFields(this._properties);
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool container
    {
        get => this._container;
        set
        {
            var __value = value;
            if ((this.container == __value))
            {
                return;
            }
            this._container = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool explicitChildNodes
    {
        get => this._explicitChildNodes;
        set
        {
            var __value = value;
            if ((this._explicitChildNodes == __value))
            {
                return;
            }
            this._explicitChildNodes = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool excludeSemantics
    {
        get => this._excludeSemantics;
        set
        {
            var __value = value;
            if ((this._excludeSemantics == __value))
            {
                return;
            }
            this._excludeSemantics = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool blockUserActions
    {
        get => this._blockUserActions;
        set
        {
            var __value = value;
            if ((this._blockUserActions == __value))
            {
                return;
            }
            this._blockUserActions = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual Locale? localeForSubtree
    {
        get => this._localeForSubtree;
        set
        {
            var __value = value;
            if ((object.Equals(this._localeForSubtree, __value)))
            {
                return;
            }
            this._localeForSubtree = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual void _updateAttributedFields(global::Doroti.Generated.Framework.Semantics.SemanticsProperties value)
    {
        this._attributedLabel = _effectiveAttributedLabel(value);
        this._attributedValue = _effectiveAttributedValue(value);
        this._attributedIncreasedValue = _effectiveAttributedIncreasedValue(value);
        this._attributedDecreasedValue = _effectiveAttributedDecreasedValue(value);
        this._attributedHint = _effectiveAttributedHint(value);
    }

    public virtual global::Doroti.Generated.Framework.Semantics.AttributedString? _effectiveAttributedLabel(global::Doroti.Generated.Framework.Semantics.SemanticsProperties value)
    {
        return (((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)value).attributedLabel ?? (((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)value).label is null) ? null : new global::Doroti.Generated.Framework.Semantics.AttributedString(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)value).label!))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Semantics.AttributedString? _effectiveAttributedValue(global::Doroti.Generated.Framework.Semantics.SemanticsProperties value)
    {
        return (((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)value).attributedValue ?? (((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)value).value is null) ? null : new global::Doroti.Generated.Framework.Semantics.AttributedString(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)value).value!))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Semantics.AttributedString? _effectiveAttributedIncreasedValue(global::Doroti.Generated.Framework.Semantics.SemanticsProperties value)
    {
        return (((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)value).attributedIncreasedValue ?? (((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)value).increasedValue is null) ? null : new global::Doroti.Generated.Framework.Semantics.AttributedString(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)value).increasedValue!))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Semantics.AttributedString? _effectiveAttributedDecreasedValue(global::Doroti.Generated.Framework.Semantics.SemanticsProperties value)
    {
        return (((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this.properties).attributedDecreasedValue ?? (((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)value).decreasedValue is null) ? null : new global::Doroti.Generated.Framework.Semantics.AttributedString(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)value).decreasedValue!))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Semantics.AttributedString? _effectiveAttributedHint(global::Doroti.Generated.Framework.Semantics.SemanticsProperties value)
    {
        return (((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)value).attributedHint ?? (((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)value).hint is null) ? null : new global::Doroti.Generated.Framework.Semantics.AttributedString(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)value).hint!))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TextDirection? textDirection
    {
        get => this._textDirection;
        set
        {
            var __value = value;
            if ((object.Equals(this.textDirection, __value)))
            {
                return;
            }
            this._textDirection = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if (this.excludeSemantics)
        {
            return;
        }
        base.visitChildrenForSemantics((Action<RenderObject>)visitor);
    }

    public override void describeSemanticsConfiguration(global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isSemanticBoundary = (this.container || ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).identifier is not null)));
        config.explicitChildNodes = this.explicitChildNodes;
        config.isBlockingUserActions = this.blockUserActions;
        if ((this.localeForSubtree is not null))
        {
            config.localeForSubtree = this.localeForSubtree;
        }
        DartRuntimePrimitives.Assert(() => (((((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).scopesRoute ?? false)) && this.explicitChildNodes)) || !((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).scopesRoute ?? false))));
        DartRuntimePrimitives.Assert(() => !((((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).toggled ?? false)) && ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).@checked ?? false)))));
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).enabled is not null))
        {
            config.isEnabled = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).enabled;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).@checked is not null))
        {
            config.isChecked = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).@checked;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).mixed is not null))
        {
            config.isCheckStateMixed = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).mixed;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).toggled is not null))
        {
            config.isToggled = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).toggled;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).selected is not null))
        {
            config.isSelected = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).selected);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).button is not null))
        {
            config.isButton = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).button);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).expanded is not null))
        {
            config.isExpanded = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).expanded;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).link is not null))
        {
            config.isLink = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).link);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).linkUrl is not null))
        {
            config.linkUrl = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).linkUrl;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).slider is not null))
        {
            config.isSlider = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).slider);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).keyboardKey is not null))
        {
            config.isKeyboardKey = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).keyboardKey);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).header is not null))
        {
            config.isHeader = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).header);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).headingLevel is not null))
        {
            config.headingLevel = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).headingLevel);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).textField is not null))
        {
            config.isTextField = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).textField);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).readOnly is not null))
        {
            config.isReadOnly = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).readOnly);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).focusable is not null))
        {
            config.isFocusable = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).focusable);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).focused is not null))
        {
            config.isFocused = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).focused;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).accessibilityFocusBlockType is not null))
        {
            config.accessibilityFocusBlockType = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).accessibilityFocusBlockType);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).inMutuallyExclusiveGroup is not null))
        {
            config.isInMutuallyExclusiveGroup = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).inMutuallyExclusiveGroup);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).obscured is not null))
        {
            config.isObscured = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).obscured);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).multiline is not null))
        {
            config.isMultiline = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).multiline);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).hidden is not null))
        {
            config.isHidden = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).hidden);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).image is not null))
        {
            config.isImage = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).image);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).isRequired is not null))
        {
            config.isRequired = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).isRequired;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).identifier is not null))
        {
            config.identifier = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).identifier!;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).traversalParentIdentifier is not null))
        {
            config.traversalParentIdentifier = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).traversalParentIdentifier;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).traversalChildIdentifier is not null))
        {
            config.traversalChildIdentifier = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).traversalChildIdentifier;
        }
        if ((this._attributedLabel is not null))
        {
            config.attributedLabel = this._attributedLabel!;
        }
        if ((this._attributedValue is not null))
        {
            config.attributedValue = this._attributedValue!;
        }
        if ((this._attributedIncreasedValue is not null))
        {
            config.attributedIncreasedValue = this._attributedIncreasedValue!;
        }
        if ((this._attributedDecreasedValue is not null))
        {
            config.attributedDecreasedValue = this._attributedDecreasedValue!;
        }
        if ((this._attributedHint is not null))
        {
            config.attributedHint = this._attributedHint!;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).tooltip is not null))
        {
            config.tooltip = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).tooltip!;
        }
        if (((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).hintOverrides is not null) && ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).hintOverrides!.isNotEmpty))
        {
            config.hintOverrides = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).hintOverrides;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).scopesRoute is not null))
        {
            config.scopesRoute = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).scopesRoute);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).namesRoute is not null))
        {
            config.namesRoute = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).namesRoute);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).liveRegion is not null))
        {
            config.liveRegion = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).liveRegion);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).maxValueLength is not null))
        {
            config.maxValueLength = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).maxValueLength;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).currentValueLength is not null))
        {
            config.currentValueLength = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).currentValueLength;
        }
        if ((this.textDirection is not null))
        {
            config.textDirection = this.textDirection;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).sortKey is not null))
        {
            config.sortKey = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).sortKey;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).tagForChildren is not null))
        {
            config.addTagForChildren(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).tagForChildren!);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this.properties).role is not null))
        {
            config.role = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).role);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).controlsNodes is not null))
        {
            config.controlsNodes = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).controlsNodes;
        }
        if ((!object.Equals(((global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration)config).validationResult, ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).validationResult)))
        {
            config.validationResult = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).validationResult;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).hitTestBehavior is not null))
        {
            config.hitTestBehavior = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).hitTestBehavior);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).inputType is not null))
        {
            config.inputType = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).inputType);
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).minValue is not null))
        {
            config.minValue = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).minValue;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).maxValue is not null))
        {
            config.maxValue = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).maxValue;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onTap is not null))
        {
            config.onTap = this._performTap;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onLongPress is not null))
        {
            config.onLongPress = this._performLongPress;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onDismiss is not null))
        {
            config.onDismiss = this._performDismiss;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onScrollLeft is not null))
        {
            config.onScrollLeft = this._performScrollLeft;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onScrollRight is not null))
        {
            config.onScrollRight = this._performScrollRight;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onScrollUp is not null))
        {
            config.onScrollUp = this._performScrollUp;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onScrollDown is not null))
        {
            config.onScrollDown = this._performScrollDown;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onIncrease is not null))
        {
            config.onIncrease = this._performIncrease;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onDecrease is not null))
        {
            config.onDecrease = this._performDecrease;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onCopy is not null))
        {
            config.onCopy = this._performCopy;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onCut is not null))
        {
            config.onCut = this._performCut;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onPaste is not null))
        {
            config.onPaste = this._performPaste;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onMoveCursorForwardByCharacter is not null))
        {
            config.onMoveCursorForwardByCharacter = this._performMoveCursorForwardByCharacter;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onMoveCursorBackwardByCharacter is not null))
        {
            config.onMoveCursorBackwardByCharacter = this._performMoveCursorBackwardByCharacter;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onMoveCursorForwardByWord is not null))
        {
            config.onMoveCursorForwardByWord = this._performMoveCursorForwardByWord;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onMoveCursorBackwardByWord is not null))
        {
            config.onMoveCursorBackwardByWord = this._performMoveCursorBackwardByWord;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onSetSelection is not null))
        {
            config.onSetSelection = this._performSetSelection;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onSetText is not null))
        {
            config.onSetText = this._performSetText;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onDidGainAccessibilityFocus is not null))
        {
            config.onDidGainAccessibilityFocus = this._performDidGainAccessibilityFocus;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onDidLoseAccessibilityFocus is not null))
        {
            config.onDidLoseAccessibilityFocus = this._performDidLoseAccessibilityFocus;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onFocus is not null))
        {
            config.onFocus = this._performFocus;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onExpand is not null))
        {
            config.onExpand = this._performExpand;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onCollapse is not null))
        {
            config.onCollapse = this._performCollapse;
        }
        if ((((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).customSemanticsActions is not null))
        {
            config.customSemanticsActions = ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).customSemanticsActions!;
        }
    }

    public virtual void _performTap()
    {
        ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onTap?.Invoke();
    }

    public virtual void _performLongPress()
    {
        ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onLongPress?.Invoke();
    }

    public virtual void _performDismiss()
    {
        ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onDismiss?.Invoke();
    }

    public virtual void _performScrollLeft()
    {
        ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onScrollLeft?.Invoke();
    }

    public virtual void _performScrollRight()
    {
        ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onScrollRight?.Invoke();
    }

    public virtual void _performScrollUp()
    {
        ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onScrollUp?.Invoke();
    }

    public virtual void _performScrollDown()
    {
        ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onScrollDown?.Invoke();
    }

    public virtual void _performIncrease()
    {
        ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onIncrease?.Invoke();
    }

    public virtual void _performDecrease()
    {
        ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onDecrease?.Invoke();
    }

    public virtual void _performCopy()
    {
        ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onCopy?.Invoke();
    }

    public virtual void _performCut()
    {
        ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onCut?.Invoke();
    }

    public virtual void _performPaste()
    {
        ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onPaste?.Invoke();
    }

    public virtual void _performMoveCursorForwardByCharacter(bool extendSelection)
    {
        ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onMoveCursorForwardByCharacter?.Invoke(extendSelection);
    }

    public virtual void _performMoveCursorBackwardByCharacter(bool extendSelection)
    {
        ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onMoveCursorBackwardByCharacter?.Invoke(extendSelection);
    }

    public virtual void _performMoveCursorForwardByWord(bool extendSelection)
    {
        ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onMoveCursorForwardByWord?.Invoke(extendSelection);
    }

    public virtual void _performMoveCursorBackwardByWord(bool extendSelection)
    {
        ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onMoveCursorBackwardByWord?.Invoke(extendSelection);
    }

    public virtual void _performSetSelection(TextSelection selection)
    {
        ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onSetSelection?.Invoke(selection);
    }

    public virtual void _performSetText(string text)
    {
        ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onSetText?.Invoke(text);
    }

    public virtual void _performDidGainAccessibilityFocus()
    {
        ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onDidGainAccessibilityFocus?.Invoke();
    }

    public virtual void _performDidLoseAccessibilityFocus()
    {
        ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onDidLoseAccessibilityFocus?.Invoke();
    }

    public virtual void _performFocus()
    {
        ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onFocus?.Invoke();
    }

    public virtual void _performExpand()
    {
        ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onExpand?.Invoke();
    }

    public virtual void _performCollapse()
    {
        ((global::Doroti.Generated.Framework.Semantics.SemanticsProperties)this._properties).onCollapse?.Invoke();
    }

}
