// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/transitions.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public abstract class AnimatedWidget : StatefulWidget
{
    public virtual global::Doroti.Framework.Foundation.Listenable listenable { get; private set; } = default!;

    protected AnimatedWidget(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Foundation.Listenable listenable = default!) : base(key: key)
    {
        this.listenable = listenable;
    }

    public abstract Widget build(BuildContext context);
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _AnimatedState__transitions());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Foundation.Listenable>("listenable", listenable));
    }

}

internal class _AnimatedState__transitions : State<AnimatedWidget>
{
    public override void initState()
    {
        base.initState();
        widget.listenable.addListener(_handleChange);
    }

    public override void didUpdateWidget(AnimatedWidget oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.listenable, oldWidget.listenable))
        {
            oldWidget.listenable.removeListener(_handleChange);
            widget.listenable.addListener(_handleChange);
        }
    }

    public override void dispose()
    {
        widget.listenable.removeListener(_handleChange);
        base.dispose();
    }

    internal virtual void _handleChange()
    {
        if (!mounted)
        {
            return;
        }
        setState(() =>
        {
        });
    }

    public override Widget build(BuildContext context) => widget.build(context);
}

public delegate Widget? DelegatedTransitionBuilder(BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, bool allowSnapshotting, Widget? child);

public class SlideTransition : AnimatedWidget
{
    public virtual TextDirection? textDirection { get; private set; }
    public virtual bool transformHitTests { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    public SlideTransition(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Animation<Offset> position = default!, bool transformHitTests = true, TextDirection? textDirection = null, Widget? child = null) : base(key: key, listenable: position)
    {
        this.transformHitTests = transformHitTests;
        this.textDirection = textDirection;
        this.child = child;
    }

    public virtual global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Offset> position => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Offset>>(((global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Offset>?)listenable)!);
    public override Widget build(BuildContext context)
    {
        global::Doroti.Ui.Offset offset = position.value;
        if (Equals(textDirection, TextDirection.rtl))
        {
            offset = new global::Doroti.Ui.Offset(-offset.dx, offset.dy);
        }
        return new FractionalTranslation(translation: offset, transformHitTests: transformHitTests, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate Matrix4 TransformCallback(double animationValue);

public class MatrixTransition : AnimatedWidget
{
    public virtual global::System.Func<double, Matrix4> onTransform { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.Alignment alignment { get; private set; } = default!;
    public virtual FilterQuality? filterQuality { get; private set; }
    public virtual Widget? child { get; private set; }

    public MatrixTransition(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Animation<double> animation = default!, global::System.Func<double, Matrix4> onTransform = default!, global::Doroti.Framework.Painting.Alignment alignment = default!, FilterQuality? filterQuality = null, Widget? child = null) : base(key: key, listenable: animation)
    {
        global::Doroti.Framework.Painting.Alignment __alignment = alignment ?? Alignment.center;
        this.onTransform = onTransform;
        this.alignment = __alignment;
        this.filterQuality = filterQuality;
        this.child = child;
    }

    public virtual global::Doroti.Framework.Animation.Animation<double> animation => ((global::Doroti.Framework.Animation.Animation<double>?)listenable)!;
    public override Widget build(BuildContext context)
    {
        return new Transform(transform: onTransform(animation.value), alignment: alignment, filterQuality: animation.isAnimating ? filterQuality : null, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ScaleTransition : MatrixTransition
{
    public ScaleTransition(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Animation<double> scale = default!, global::Doroti.Framework.Painting.Alignment alignment = default!, FilterQuality? filterQuality = null, Widget? child = null) : base(key: key, alignment: alignment ?? Alignment.center, filterQuality: filterQuality, child: child, animation: scale, onTransform: _handleScaleMatrix)
    {
    }

    public virtual global::Doroti.Framework.Animation.Animation<double> scale => animation;
    internal static Matrix4 _handleScaleMatrix(double value) => Matrix4.diagonal3Values(value, value, 1.0);
}

public class RotationTransition : MatrixTransition
{
    public RotationTransition(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Animation<double> turns = default!, global::Doroti.Framework.Painting.Alignment alignment = default!, FilterQuality? filterQuality = null, Widget? child = null) : base(key: key, alignment: alignment ?? Alignment.center, filterQuality: filterQuality, child: child, animation: turns, onTransform: _handleTurnsMatrix)
    {
    }

    public virtual global::Doroti.Framework.Animation.Animation<double> turns => animation;
    internal static Matrix4 _handleTurnsMatrix(double value) => Matrix4.rotationZ(value * Dart_mathLibrary.pi * 2.0);
}

public class SizeTransition : AnimatedWidget
{
    public virtual global::Doroti.Framework.Painting.Axis axis { get; private set; } = default!;
    public virtual double? axisAlignment { get; private set; }
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry? alignment { get; private set; }
    public virtual double? fixedCrossAxisSizeFactor { get; private set; }
    public virtual Widget? child { get; private set; }

    public SizeTransition(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.Axis axis = Axis.vertical, global::Doroti.Framework.Animation.Animation<double> sizeFactor = default!, double? axisAlignment = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, double? fixedCrossAxisSizeFactor = null, Widget? child = null) : base(key: key, listenable: sizeFactor)
    {
        this.axis = axis;
        this.axisAlignment = axisAlignment;
        this.alignment = alignment;
        this.fixedCrossAxisSizeFactor = fixedCrossAxisSizeFactor;
        this.child = child;
        System.Diagnostics.Debug.Assert((fixedCrossAxisSizeFactor is null) || (fixedCrossAxisSizeFactor >= 0.0));
        System.Diagnostics.Debug.Assert((axisAlignment is null) || (alignment is null));
    }

    public virtual global::Doroti.Framework.Animation.Animation<double> sizeFactor => ((global::Doroti.Framework.Animation.Animation<double>?)listenable)!;
    public override Widget build(BuildContext context)
    {
        return new ClipRect(child: new Align(alignment: alignment ?? (axis switch { Axis.horizontal => new global::Doroti.Framework.Painting.AlignmentDirectional(axisAlignment ?? 0.0, -1.0), Axis.vertical => new global::Doroti.Framework.Painting.AlignmentDirectional(-1.0, axisAlignment ?? 0.0), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }), heightFactor: Equals(axis, Axis.vertical) ? Math.Max(sizeFactor.value, 0.0) : fixedCrossAxisSizeFactor, widthFactor: Equals(axis, Axis.horizontal) ? Math.Max(sizeFactor.value, 0.0) : fixedCrossAxisSizeFactor, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class FadeTransition : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Framework.Animation.Animation<double> opacity { get; private set; } = default!;
    public virtual bool alwaysIncludeSemantics { get; private set; } = default!;

    public FadeTransition(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Animation<double> opacity = default!, bool alwaysIncludeSemantics = false, Widget? child = null) : base(key: key, child: child)
    {
        this.opacity = opacity;
        this.alwaysIncludeSemantics = alwaysIncludeSemantics;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return new global::Doroti.Framework.Rendering.RenderAnimatedOpacity(opacity: opacity, alwaysIncludeSemantics: alwaysIncludeSemantics);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderAnimatedOpacity)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.RenderAnimatedOpacity>)(() =>
{
    var __cascade = __renderObject;
    __cascade.opacity = opacity;
    __cascade.alwaysIncludeSemantics = alwaysIncludeSemantics;
    return __cascade;
}))());
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Animation.Animation<double>>("opacity", opacity));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("alwaysIncludeSemantics", value: alwaysIncludeSemantics, ifTrue: "alwaysIncludeSemantics"));
    }

}

public class SliverFadeTransition : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Framework.Animation.Animation<double> opacity { get; private set; } = default!;
    public virtual bool alwaysIncludeSemantics { get; private set; } = default!;

    public SliverFadeTransition(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Animation<double> opacity = default!, bool alwaysIncludeSemantics = false, Widget? sliver = null) : base(key: key, child: sliver)
    {
        this.opacity = opacity;
        this.alwaysIncludeSemantics = alwaysIncludeSemantics;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return new global::Doroti.Framework.Rendering.RenderSliverAnimatedOpacity(opacity: opacity, alwaysIncludeSemantics: alwaysIncludeSemantics);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderSliverAnimatedOpacity)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.RenderSliverAnimatedOpacity>)(() =>
{
    var __cascade = __renderObject;
    __cascade.opacity = opacity;
    __cascade.alwaysIncludeSemantics = alwaysIncludeSemantics;
    return __cascade;
}))());
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Animation.Animation<double>>("opacity", opacity));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("alwaysIncludeSemantics", value: alwaysIncludeSemantics, ifTrue: "alwaysIncludeSemantics"));
    }

}

public class RelativeRectTween : global::Doroti.Framework.Animation.Tween<global::Doroti.Framework.Rendering.RelativeRect>
{
    public RelativeRectTween(global::Doroti.Framework.Rendering.RelativeRect? begin = null, global::Doroti.Framework.Rendering.RelativeRect? end = null) : base(begin: begin, end: end)
    {
    }

    public override global::Doroti.Framework.Rendering.RelativeRect lerp(double t) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RelativeRect>(RelativeRect.lerp(begin, end, t)!);
}

public class PositionedTransition : AnimatedWidget
{
    public virtual Widget child { get; private set; } = default!;

    public PositionedTransition(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Rendering.RelativeRect> rect = default!, Widget child = default!) : base(key: key, listenable: rect)
    {
        this.child = child;
    }

    public virtual global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Rendering.RelativeRect> rect => ((global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Rendering.RelativeRect>?)listenable)!;
    public override Widget build(BuildContext context)
    {
        return Positioned.CreateFromRelativeRect(rect: rect.value, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RelativePositionedTransition : AnimatedWidget
{
    public virtual Size size { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public RelativePositionedTransition(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Animation<Rect?> rect = default!, Size size = default!, Widget child = default!) : base(key: key, listenable: rect)
    {
        this.size = size;
        this.child = child;
    }

    public virtual global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Rect?> rect => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Rect?>>(((global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Rect?>?)listenable)!);
    public override Widget build(BuildContext context)
    {
        var offsets = RelativeRect.CreateFromSize(rect.value ?? Rect.zero, size);
        return new Positioned(top: offsets.top, right: offsets.right, bottom: offsets.bottom, left: offsets.left, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DecoratedBoxTransition : AnimatedWidget
{
    public virtual global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.Decoration> decoration { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.DecorationPosition position { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public DecoratedBoxTransition(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.Decoration> decoration = default!, global::Doroti.Framework.Rendering.DecorationPosition position = DecorationPosition.background, Widget child = default!) : base(key: key, listenable: decoration)
    {
        this.decoration = decoration;
        this.position = position;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return new DecoratedBox(decoration: decoration.value, position: position, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class AlignTransition : AnimatedWidget
{
    public virtual double? widthFactor { get; private set; }
    public virtual double? heightFactor { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public AlignTransition(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.AlignmentGeometry> alignment = default!, Widget child = default!, double? widthFactor = null, double? heightFactor = null) : base(key: key, listenable: alignment)
    {
        this.child = child;
        this.widthFactor = widthFactor;
        this.heightFactor = heightFactor;
    }

    public virtual global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.AlignmentGeometry> alignment => ((global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.AlignmentGeometry>?)listenable)!;
    public override Widget build(BuildContext context)
    {
        return new Align(alignment: alignment.value, widthFactor: widthFactor, heightFactor: heightFactor, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DefaultTextStyleTransition : AnimatedWidget
{
    public virtual TextAlign? textAlign { get; private set; }
    public virtual bool softWrap { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextOverflow overflow { get; private set; } = default!;
    public virtual long? maxLines { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public DefaultTextStyleTransition(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.TextStyle> style = default!, Widget child = default!, TextAlign? textAlign = null, bool softWrap = true, global::Doroti.Framework.Painting.TextOverflow overflow = TextOverflow.clip, long? maxLines = null) : base(key: key, listenable: style)
    {
        this.child = child;
        this.textAlign = textAlign;
        this.softWrap = softWrap;
        this.overflow = overflow;
        this.maxLines = maxLines;
    }

    public virtual global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.TextStyle> style => ((global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.TextStyle>?)listenable)!;
    public override Widget build(BuildContext context)
    {
        return new DefaultTextStyle(style: style.value, textAlign: textAlign, softWrap: softWrap, overflow: overflow, maxLines: maxLines, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ListenableBuilder : AnimatedWidget
{
    public virtual global::System.Func<BuildContext, Widget?, Widget> builder { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    public ListenableBuilder(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Foundation.Listenable listenable = default!, global::System.Func<BuildContext, Widget?, Widget> builder = default!, Widget? child = null) : base(key: key, listenable: listenable)
    {
        this.builder = builder;
        this.child = child;
    }

    public override global::Doroti.Framework.Foundation.Listenable listenable => base.listenable;
    public override Widget build(BuildContext context) => builder(context, child);
}

public class AnimatedBuilder : ListenableBuilder
{
    public AnimatedBuilder(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Foundation.Listenable animation = default!, global::System.Func<BuildContext, Widget?, Widget> builder = default!, Widget? child = null) : base(key: key, builder: builder, child: child, listenable: animation)
    {
    }

    public virtual global::Doroti.Framework.Foundation.Listenable animation => base.listenable;
    public override global::Doroti.Framework.Foundation.Listenable listenable => base.listenable;
    public override global::System.Func<BuildContext, Widget?, Widget> builder => base.builder;
}
