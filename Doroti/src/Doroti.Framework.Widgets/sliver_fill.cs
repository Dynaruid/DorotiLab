// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/sliver_fill.dart
using Doroti.Runtime;
using Doroti.Ui;

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
        System.Diagnostics.Debug.Assert(viewportFraction > 0.0);
    }

    public override Widget build(BuildContext context)
    {
        return new _SliverFractionalPadding__sliver_fill(viewportFraction: padEnds ? (Dart_uiLibrary.clampDouble(1L - viewportFraction, 0, 1) / 2L) : 0, sliver: new _SliverFillViewportRenderObjectWidget__sliver_fill(viewportFraction: viewportFraction, allowImplicitScrolling: allowImplicitScrolling, @delegate: @delegate));
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
        System.Diagnostics.Debug.Assert(viewportFraction > 0.0);
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        var element = ((SliverMultiBoxAdaptorElement?)context)!;
        return new global::Doroti.Framework.Rendering.RenderSliverFillViewport(childManager: element, viewportFraction: viewportFraction, allowImplicitScrolling: allowImplicitScrolling);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderSliverFillViewport)renderObject;
        __renderObject.viewportFraction = viewportFraction;
        __renderObject.allowImplicitScrolling = allowImplicitScrolling;
    }

}

internal class _SliverFractionalPadding__sliver_fill : SingleChildRenderObjectWidget
{
    public virtual double viewportFraction { get; private set; } = default!;

    internal _SliverFractionalPadding__sliver_fill(double viewportFraction = 0, Widget? sliver = null) : base(child: sliver)
    {
        this.viewportFraction = viewportFraction;
        System.Diagnostics.Debug.Assert(viewportFraction >= 0L);
        System.Diagnostics.Debug.Assert(viewportFraction <= 0.5);
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(new _RenderSliverFractionalPadding__sliver_fill(viewportFraction: viewportFraction));
    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSliverFractionalPadding__sliver_fill)renderObject;
        __renderObject.viewportFraction = viewportFraction;
    }

}

public class _RenderSliverFractionalPadding__sliver_fill : global::Doroti.Framework.Rendering.RenderSliverEdgeInsetsPadding
{
    internal virtual global::Doroti.Framework.Rendering.SliverConstraints? _lastResolvedConstraints { get; set; } = default;
    internal virtual double _viewportFraction { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.EdgeInsets? _resolvedPadding { get; set; } = default;

    internal _RenderSliverFractionalPadding__sliver_fill(double viewportFraction = 0)
    {
        _viewportFraction = viewportFraction;
        System.Diagnostics.Debug.Assert(viewportFraction <= 0.5);
        System.Diagnostics.Debug.Assert(viewportFraction >= 0L);
    }

    public virtual double viewportFraction
    {
        get => _viewportFraction;
        set
        {
            var newValue = value;
            if (_viewportFraction == newValue)
            {
                return;
            }
            _viewportFraction = newValue;
            _markNeedsResolution();
        }
    }
    public override global::Doroti.Framework.Painting.EdgeInsets? resolvedPadding => _resolvedPadding;
    internal virtual void _markNeedsResolution()
    {
        _resolvedPadding = null;
        markNeedsLayout();
    }

    internal virtual void _resolve()
    {
        if ((_resolvedPadding is not null) && Equals(_lastResolvedConstraints, constraints))
        {
            return;
        }
        double paddingValue = constraints.viewportMainAxisExtent * viewportFraction;
        _lastResolvedConstraints = constraints;
        _resolvedPadding = constraints.axis switch { Axis.horizontal => EdgeInsets.CreateSymmetric(horizontal: paddingValue), Axis.vertical => EdgeInsets.CreateSymmetric(vertical: paddingValue), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
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
        if (hasScrollBody)
        {
            return new _SliverFillRemainingWithScrollable__sliver_fill(child: child);
        }
        if (!fillOverscroll)
        {
            return new _SliverFillRemainingWithoutScrollable__sliver_fill(child: child);
        }
        return new _SliverFillRemainingAndOverscroll__sliver_fill(child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Widget>("child", child));
        var flags = new List<string>();
        if (!Enumerable.Any(flags))
        {
            flags.Add("nonscrollable");
        }
        properties.add(new global::Doroti.Framework.Foundation.IterableProperty<string>("mode", flags.Cast<string>()));
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

