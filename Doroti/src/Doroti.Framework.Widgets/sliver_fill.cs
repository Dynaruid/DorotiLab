// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/sliver_fill.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public class SliverFillViewport : StatelessWidget
{
    public virtual double viewportFraction { get; private set; } = default!;
    public virtual bool padEnds { get; private set; } = default!;
    public virtual SliverChildDelegate @delegate { get; private set; } = default!;
    public virtual bool allowImplicitScrolling { get; private set; } = default!;

    public SliverFillViewport(global::Doroti.Framework.Foundation.Key? key = null, SliverChildDelegate @delegate = default!, double viewportFraction = 1.0, bool padEnds = true, bool allowImplicitScrolling = true) : base(key: key)
    {
        this.@delegate = @delegate;
        this.viewportFraction = viewportFraction;
        this.padEnds = padEnds;
        this.allowImplicitScrolling = allowImplicitScrolling;
        System.Diagnostics.Debug.Assert((viewportFraction > 0.0));
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new _SliverFractionalPadding__sliver_fill(viewportFraction: (this.padEnds ? (Dart_uiLibrary.clampDouble((1L - this.viewportFraction), 0, 1) / 2L) : 0), sliver: new _SliverFillViewportRenderObjectWidget__sliver_fill(viewportFraction: this.viewportFraction, allowImplicitScrolling: this.allowImplicitScrolling, @delegate: this.@delegate)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SliverFillViewportRenderObjectWidget__sliver_fill : SliverMultiBoxAdaptorWidget
{
    public virtual double viewportFraction { get; private set; } = default!;
    public virtual bool allowImplicitScrolling { get; private set; } = default!;

    internal _SliverFillViewportRenderObjectWidget__sliver_fill(SliverChildDelegate @delegate, double viewportFraction = 1.0, bool allowImplicitScrolling = true) : base(@delegate: @delegate)
    {
        this.viewportFraction = viewportFraction;
        this.allowImplicitScrolling = allowImplicitScrolling;
        System.Diagnostics.Debug.Assert((viewportFraction > 0.0));
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        var element__3536 = ((SliverMultiBoxAdaptorElement?)(object?)context)!;
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new global::Doroti.Framework.Rendering.RenderSliverFillViewport(childManager: element__3536, viewportFraction: this.viewportFraction, allowImplicitScrolling: this.allowImplicitScrolling));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderSliverFillViewport)(object)renderObject;
        __renderObject.viewportFraction = this.viewportFraction;
        __renderObject.allowImplicitScrolling = this.allowImplicitScrolling;
    }

}

internal class _SliverFractionalPadding__sliver_fill : SingleChildRenderObjectWidget
{
    public virtual double viewportFraction { get; private set; } = default!;

    internal _SliverFractionalPadding__sliver_fill(double viewportFraction = 0, Widget? sliver = null) : base(child: sliver)
    {
        this.viewportFraction = viewportFraction;
        System.Diagnostics.Debug.Assert((viewportFraction >= 0L));
        System.Diagnostics.Debug.Assert((viewportFraction <= 0.5));
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(new _RenderSliverFractionalPadding__sliver_fill(viewportFraction: this.viewportFraction));
    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSliverFractionalPadding__sliver_fill)(object)renderObject;
        __renderObject.viewportFraction = this.viewportFraction;
    }

}

public class _RenderSliverFractionalPadding__sliver_fill : global::Doroti.Framework.Rendering.RenderSliverEdgeInsetsPadding
{
    internal virtual global::Doroti.Framework.Rendering.SliverConstraints? _lastResolvedConstraints { get; set; } = default;
    internal virtual double _viewportFraction { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.EdgeInsets? _resolvedPadding { get; set; } = default;

    internal _RenderSliverFractionalPadding__sliver_fill(double viewportFraction = 0)
    {
        this._viewportFraction = viewportFraction;
        System.Diagnostics.Debug.Assert((viewportFraction <= 0.5));
        System.Diagnostics.Debug.Assert((viewportFraction >= 0L));
    }

    public virtual double viewportFraction
    {
        get => this._viewportFraction;
        set
        {
            var newValue = value;
            if ((this._viewportFraction == newValue))
            {
                return;
            }
            _viewportFraction = newValue;
            _markNeedsResolution();
        }
    }
    public override global::Doroti.Framework.Painting.EdgeInsets? resolvedPadding => this._resolvedPadding;
    internal virtual void _markNeedsResolution()
    {
        _resolvedPadding = null;
        markNeedsLayout();
    }

    internal virtual void _resolve()
    {
        if (((this._resolvedPadding is not null) && (object.Equals(this._lastResolvedConstraints, this.constraints))))
        {
            return;
        }
        double paddingValue__5477 = (((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).viewportMainAxisExtent * this.viewportFraction);
        _lastResolvedConstraints = this.constraints;
        _resolvedPadding = (((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).axis switch { global::Doroti.Framework.Painting.Axis.horizontal => global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: paddingValue__5477), global::Doroti.Framework.Painting.Axis.vertical => global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: paddingValue__5477), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return;
    }

    public override void performLayout()
    {
        _resolve();
        base.performLayout();
    }

}

public class SliverFillRemaining : StatelessWidget
{
    public virtual Widget? child { get; private set; }
    public virtual bool hasScrollBody { get; private set; } = default!;
    public virtual bool fillOverscroll { get; private set; } = default!;

    public SliverFillRemaining(global::Doroti.Framework.Foundation.Key? key = null, Widget? child = null, bool hasScrollBody = true, bool fillOverscroll = false) : base(key: key)
    {
        this.child = child;
        this.hasScrollBody = hasScrollBody;
        this.fillOverscroll = fillOverscroll;
    }

    public override Widget build(BuildContext context)
    {
        if (this.hasScrollBody)
        {
            return ((Widget)(object?)new _SliverFillRemainingWithScrollable__sliver_fill(child: this.child));
        }
        if (!this.fillOverscroll)
        {
            return ((Widget)(object?)new _SliverFillRemainingWithoutScrollable__sliver_fill(child: this.child));
        }
        return ((Widget)(object?)new _SliverFillRemainingAndOverscroll__sliver_fill(child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Widget>("child", this.child));
        var flags__12078 = new List<string>();
        if (!System.Linq.Enumerable.Any(flags__12078))
        {
            flags__12078.Add("nonscrollable");
        }
        properties.add(new global::Doroti.Framework.Foundation.IterableProperty<string>("mode", flags__12078.Cast<string>()));
    }

}

internal class _SliverFillRemainingWithScrollable__sliver_fill : SingleChildRenderObjectWidget
{
    internal _SliverFillRemainingWithScrollable__sliver_fill(Widget? child = null) : base(child: child)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(new global::Doroti.Framework.Rendering.RenderSliverFillRemainingWithScrollable());
}

internal class _SliverFillRemainingWithoutScrollable__sliver_fill : SingleChildRenderObjectWidget
{
    internal _SliverFillRemainingWithoutScrollable__sliver_fill(Widget? child = null) : base(child: child)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(new global::Doroti.Framework.Rendering.RenderSliverFillRemaining());
}

internal class _SliverFillRemainingAndOverscroll__sliver_fill : SingleChildRenderObjectWidget
{
    internal _SliverFillRemainingAndOverscroll__sliver_fill(Widget? child = null) : base(child: child)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(new global::Doroti.Framework.Rendering.RenderSliverFillRemainingAndOverscroll());
}

