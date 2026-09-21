// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/transitions.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public abstract class AnimatedWidget : StatefulWidget
{
    public virtual Listenable listenable { get; private set; } = default!;

    protected AnimatedWidget(Key? key = null, Listenable listenable = default!)
        : base(key: key)
    {
        this.listenable = listenable;
    }

    public abstract Widget build(BuildContext context);

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _AnimatedState__transitions());

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Listenable>("listenable", listenable));
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
        setState(() => { });
    }

    public override Widget build(BuildContext context) => widget.build(context);
}

public delegate Widget? DelegatedTransitionBuilder(
    BuildContext context,
    Animation<double> animation,
    Animation<double> secondaryAnimation,
    bool allowSnapshotting,
    Widget? child
);

public class SlideTransition : AnimatedWidget
{
    public virtual TextDirection? textDirection { get; private set; }
    public virtual bool transformHitTests { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    public SlideTransition(
        Key? key = null,
        Animation<Offset> position = default!,
        bool transformHitTests = true,
        TextDirection? textDirection = null,
        Widget? child = null
    )
        : base(key: key, listenable: position)
    {
        this.transformHitTests = transformHitTests;
        this.textDirection = textDirection;
        this.child = child;
    }

    public virtual Animation<Offset> position =>
        DartRuntimePrimitives.ConvertValue<Animation<Offset>>(((Animation<Offset>?)listenable)!);

    public override Widget build(BuildContext context)
    {
        Offset offset = position.value;
        if (Equals(textDirection, TextDirection.rtl))
        {
            offset = new Offset(-offset.dx, offset.dy);
        }
        return new FractionalTranslation(
            translation: offset,
            transformHitTests: transformHitTests,
            child: child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public delegate Matrix4 TransformCallback(double animationValue);

public class MatrixTransition : AnimatedWidget
{
    public virtual Func<double, Matrix4> onTransform { get; private set; } = default!;
    public virtual Alignment alignment { get; private set; } = default!;
    public virtual FilterQuality? filterQuality { get; private set; }
    public virtual Widget? child { get; private set; }

    public MatrixTransition(
        Key? key = null,
        Animation<double> animation = default!,
        Func<double, Matrix4> onTransform = default!,
        Alignment alignment = default!,
        FilterQuality? filterQuality = null,
        Widget? child = null
    )
        : base(key: key, listenable: animation)
    {
        Alignment __alignment = alignment ?? Alignment.center;
        this.onTransform = onTransform;
        this.alignment = __alignment;
        this.filterQuality = filterQuality;
        this.child = child;
    }

    public virtual Animation<double> animation => ((Animation<double>?)listenable)!;

    public override Widget build(BuildContext context)
    {
        return new Transform(
            transform: onTransform(animation.value),
            alignment: alignment,
            filterQuality: animation.isAnimating ? filterQuality : null,
            child: child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class ScaleTransition : MatrixTransition
{
    public ScaleTransition(
        Key? key = null,
        Animation<double> scale = default!,
        Alignment alignment = default!,
        FilterQuality? filterQuality = null,
        Widget? child = null
    )
        : base(
            key: key,
            alignment: alignment ?? Alignment.center,
            filterQuality: filterQuality,
            child: child,
            animation: scale,
            onTransform: _handleScaleMatrix
        ) { }

    public virtual Animation<double> scale => animation;

    internal static Matrix4 _handleScaleMatrix(double value) =>
        Matrix4.diagonal3Values(value, value, 1.0);
}

public class RotationTransition : MatrixTransition
{
    public RotationTransition(
        Key? key = null,
        Animation<double> turns = default!,
        Alignment alignment = default!,
        FilterQuality? filterQuality = null,
        Widget? child = null
    )
        : base(
            key: key,
            alignment: alignment ?? Alignment.center,
            filterQuality: filterQuality,
            child: child,
            animation: turns,
            onTransform: _handleTurnsMatrix
        ) { }

    public virtual Animation<double> turns => animation;

    internal static Matrix4 _handleTurnsMatrix(double value) =>
        Matrix4.rotationZ(value * Dart_mathLibrary.pi * 2.0);
}

public class SizeTransition : AnimatedWidget
{
    public virtual Axis axis { get; private set; } = default!;
    public virtual double? axisAlignment { get; private set; }
    public virtual AlignmentGeometry? alignment { get; private set; }
    public virtual double? fixedCrossAxisSizeFactor { get; private set; }
    public virtual Widget? child { get; private set; }

    public SizeTransition(
        Key? key = null,
        Axis axis = Axis.vertical,
        Animation<double> sizeFactor = default!,
        double? axisAlignment = null,
        AlignmentGeometry? alignment = null,
        double? fixedCrossAxisSizeFactor = null,
        Widget? child = null
    )
        : base(key: key, listenable: sizeFactor)
    {
        this.axis = axis;
        this.axisAlignment = axisAlignment;
        this.alignment = alignment;
        this.fixedCrossAxisSizeFactor = fixedCrossAxisSizeFactor;
        this.child = child;
        System.Diagnostics.Debug.Assert(
            (fixedCrossAxisSizeFactor is null) || (fixedCrossAxisSizeFactor >= 0.0)
        );
        System.Diagnostics.Debug.Assert((axisAlignment is null) || (alignment is null));
    }

    public virtual Animation<double> sizeFactor => ((Animation<double>?)listenable)!;

    public override Widget build(BuildContext context)
    {
        return new ClipRect(
            child: new Align(
                alignment: alignment
                    ?? (
                        axis switch
                        {
                            Axis.horizontal => new AlignmentDirectional(axisAlignment ?? 0.0, -1.0),
                            Axis.vertical => new AlignmentDirectional(-1.0, axisAlignment ?? 0.0),
                            _ => throw new InvalidOperationException(
                                "Switch expression did not handle the supplied value."
                            ),
                        }
                    ),
                heightFactor: Equals(axis, Axis.vertical)
                    ? Math.Max(sizeFactor.value, 0.0)
                    : fixedCrossAxisSizeFactor,
                widthFactor: Equals(axis, Axis.horizontal)
                    ? Math.Max(sizeFactor.value, 0.0)
                    : fixedCrossAxisSizeFactor,
                child: child
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class FadeTransition : SingleChildRenderObjectWidget
{
    public virtual Animation<double> opacity { get; private set; } = default!;
    public virtual bool alwaysIncludeSemantics { get; private set; } = default!;

    public FadeTransition(
        Key? key = null,
        Animation<double> opacity = default!,
        bool alwaysIncludeSemantics = false,
        Widget? child = null
    )
        : base(key: key, child: child)
    {
        this.opacity = opacity;
        this.alwaysIncludeSemantics = alwaysIncludeSemantics;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderAnimatedOpacity(
            opacity: opacity,
            alwaysIncludeSemantics: alwaysIncludeSemantics
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderAnimatedOpacity)renderObject;
        DartRuntimePrimitives.Ignore(
            (
                (Func<RenderAnimatedOpacity>)(
                    () =>
                    {
                        var __cascade = __renderObject;
                        __cascade.opacity = opacity;
                        __cascade.alwaysIncludeSemantics = alwaysIncludeSemantics;
                        return __cascade;
                    }
                )
            )()
        );
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Animation<double>>("opacity", opacity));
        properties.add(
            new FlagProperty(
                "alwaysIncludeSemantics",
                value: alwaysIncludeSemantics,
                ifTrue: "alwaysIncludeSemantics"
            )
        );
    }
}

public class SliverFadeTransition : SingleChildRenderObjectWidget
{
    public virtual Animation<double> opacity { get; private set; } = default!;
    public virtual bool alwaysIncludeSemantics { get; private set; } = default!;

    public SliverFadeTransition(
        Key? key = null,
        Animation<double> opacity = default!,
        bool alwaysIncludeSemantics = false,
        Widget? sliver = null
    )
        : base(key: key, child: sliver)
    {
        this.opacity = opacity;
        this.alwaysIncludeSemantics = alwaysIncludeSemantics;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderSliverAnimatedOpacity(
            opacity: opacity,
            alwaysIncludeSemantics: alwaysIncludeSemantics
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderSliverAnimatedOpacity)renderObject;
        DartRuntimePrimitives.Ignore(
            (
                (Func<RenderSliverAnimatedOpacity>)(
                    () =>
                    {
                        var __cascade = __renderObject;
                        __cascade.opacity = opacity;
                        __cascade.alwaysIncludeSemantics = alwaysIncludeSemantics;
                        return __cascade;
                    }
                )
            )()
        );
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Animation<double>>("opacity", opacity));
        properties.add(
            new FlagProperty(
                "alwaysIncludeSemantics",
                value: alwaysIncludeSemantics,
                ifTrue: "alwaysIncludeSemantics"
            )
        );
    }
}

public class RelativeRectTween : Tween<RelativeRect>
{
    public RelativeRectTween(RelativeRect? begin = null, RelativeRect? end = null)
        : base(begin: begin, end: end) { }

    public override RelativeRect lerp(double t) =>
        DartRuntimePrimitives.ConvertValue<RelativeRect>(RelativeRect.lerp(begin, end, t)!);
}

public class PositionedTransition : AnimatedWidget
{
    public virtual Widget child { get; private set; } = default!;

    public PositionedTransition(
        Key? key = null,
        Animation<RelativeRect> rect = default!,
        Widget child = default!
    )
        : base(key: key, listenable: rect)
    {
        this.child = child;
    }

    public virtual Animation<RelativeRect> rect => ((Animation<RelativeRect>?)listenable)!;

    public override Widget build(BuildContext context)
    {
        return Positioned.CreateFromRelativeRect(rect: rect.value, child: child);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class RelativePositionedTransition : AnimatedWidget
{
    public virtual Size size { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public RelativePositionedTransition(
        Key? key = null,
        Animation<Rect?> rect = default!,
        Size size = default!,
        Widget child = default!
    )
        : base(key: key, listenable: rect)
    {
        this.size = size;
        this.child = child;
    }

    public virtual Animation<Rect?> rect =>
        DartRuntimePrimitives.ConvertValue<Animation<Rect?>>(((Animation<Rect?>?)listenable)!);

    public override Widget build(BuildContext context)
    {
        var offsets = RelativeRect.CreateFromSize(rect.value ?? Rect.zero, size);
        return new Positioned(
            top: offsets.top,
            right: offsets.right,
            bottom: offsets.bottom,
            left: offsets.left,
            child: child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class DecoratedBoxTransition : AnimatedWidget
{
    public virtual Animation<Decoration> decoration { get; private set; } = default!;
    public virtual DecorationPosition position { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public DecoratedBoxTransition(
        Key? key = null,
        Animation<Decoration> decoration = default!,
        DecorationPosition position = DecorationPosition.background,
        Widget child = default!
    )
        : base(key: key, listenable: decoration)
    {
        this.decoration = decoration;
        this.position = position;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return new DecoratedBox(decoration: decoration.value, position: position, child: child);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class AlignTransition : AnimatedWidget
{
    public virtual double? widthFactor { get; private set; }
    public virtual double? heightFactor { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public AlignTransition(
        Key? key = null,
        Animation<AlignmentGeometry> alignment = default!,
        Widget child = default!,
        double? widthFactor = null,
        double? heightFactor = null
    )
        : base(key: key, listenable: alignment)
    {
        this.child = child;
        this.widthFactor = widthFactor;
        this.heightFactor = heightFactor;
    }

    public virtual Animation<AlignmentGeometry> alignment =>
        ((Animation<AlignmentGeometry>?)listenable)!;

    public override Widget build(BuildContext context)
    {
        return new Align(
            alignment: alignment.value,
            widthFactor: widthFactor,
            heightFactor: heightFactor,
            child: child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class DefaultTextStyleTransition : AnimatedWidget
{
    public virtual TextAlign? textAlign { get; private set; }
    public virtual bool softWrap { get; private set; } = default!;
    public virtual TextOverflow overflow { get; private set; } = default!;
    public virtual long? maxLines { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public DefaultTextStyleTransition(
        Key? key = null,
        Animation<TextStyle> style = default!,
        Widget child = default!,
        TextAlign? textAlign = null,
        bool softWrap = true,
        TextOverflow overflow = TextOverflow.clip,
        long? maxLines = null
    )
        : base(key: key, listenable: style)
    {
        this.child = child;
        this.textAlign = textAlign;
        this.softWrap = softWrap;
        this.overflow = overflow;
        this.maxLines = maxLines;
    }

    public virtual Animation<TextStyle> style => ((Animation<TextStyle>?)listenable)!;

    public override Widget build(BuildContext context)
    {
        return new DefaultTextStyle(
            style: style.value,
            textAlign: textAlign,
            softWrap: softWrap,
            overflow: overflow,
            maxLines: maxLines,
            child: child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class ListenableBuilder : AnimatedWidget
{
    public virtual Func<BuildContext, Widget?, Widget> builder { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    public ListenableBuilder(
        Key? key = null,
        Listenable listenable = default!,
        Func<BuildContext, Widget?, Widget> builder = default!,
        Widget? child = null
    )
        : base(key: key, listenable: listenable)
    {
        this.builder = builder;
        this.child = child;
    }

    public override Listenable listenable => base.listenable;

    public override Widget build(BuildContext context) => builder(context, child);
}

public class AnimatedBuilder : ListenableBuilder
{
    public AnimatedBuilder(
        Key? key = null,
        Listenable animation = default!,
        Func<BuildContext, Widget?, Widget> builder = default!,
        Widget? child = null
    )
        : base(key: key, builder: builder, child: child, listenable: animation) { }

    public virtual Listenable animation => base.listenable;
    public override Listenable listenable => base.listenable;
    public override Func<BuildContext, Widget?, Widget> builder => base.builder;
}
