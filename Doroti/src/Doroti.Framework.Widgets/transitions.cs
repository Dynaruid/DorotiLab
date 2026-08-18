// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/transitions.dart
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
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Foundation.Listenable>("listenable", this.listenable));
    }

}

internal class _AnimatedState__transitions : State<AnimatedWidget>
{
    public override void initState()
    {
        base.initState();
        ((AnimatedWidget)this.widget).listenable.addListener(() => this._handleChange());
    }

    public override void didUpdateWidget(AnimatedWidget oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((AnimatedWidget)this.widget).listenable, ((AnimatedWidget)oldWidget).listenable)))
        {
            ((AnimatedWidget)oldWidget).listenable.removeListener(() => this._handleChange());
            ((AnimatedWidget)this.widget).listenable.addListener(() => this._handleChange());
        }
    }

    public override void dispose()
    {
        ((AnimatedWidget)this.widget).listenable.removeListener(() => this._handleChange());
        base.dispose();
    }

    internal virtual void _handleChange()
    {
        if (!this.mounted)
        {
            return;
        }
        setState(((global::System.Action)(() =>
        {
        })));
    }

    public override Widget build(BuildContext context) => this.widget.build(context);
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

    public virtual global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Offset> position => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Offset>>(((global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Offset>?)(object?)this.listenable)!);
    public override Widget build(BuildContext context)
    {
        global::Doroti.Ui.Offset offset__9531 = ((global::Doroti.Ui.Offset)(object?)((global::Doroti.Framework.Animation.Animation<Offset>)this.position).value);
        if ((object.Equals(this.textDirection, TextDirection.rtl)))
        {
            offset__9531 = new global::Doroti.Ui.Offset(-offset__9531.dx, offset__9531.dy);
        }
        return ((Widget)(object?)new FractionalTranslation(translation: offset__9531, transformHitTests: this.transformHitTests, child: this.child));
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
        global::Doroti.Framework.Painting.Alignment __alignment = alignment ?? global::Doroti.Framework.Painting.Alignment.center;
        this.onTransform = onTransform;
        this.alignment = __alignment;
        this.filterQuality = filterQuality;
        this.child = child;
    }

    public virtual global::Doroti.Framework.Animation.Animation<double> animation => ((global::Doroti.Framework.Animation.Animation<double>?)(object?)this.listenable)!;
    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new Transform(transform: this.onTransform(((global::Doroti.Framework.Animation.Animation<double>)this.animation).value), alignment: this.alignment, filterQuality: (((global::Doroti.Framework.Animation.Animation<double>)this.animation).isAnimating ? this.filterQuality : null), child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ScaleTransition : MatrixTransition
{
    public ScaleTransition(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Animation<double> scale = default!, global::Doroti.Framework.Painting.Alignment alignment = default!, FilterQuality? filterQuality = null, Widget? child = null) : base(key: key, alignment: alignment ?? global::Doroti.Framework.Painting.Alignment.center, filterQuality: filterQuality, child: child, animation: scale, onTransform: (global::System.Func<double, Matrix4>)_handleScaleMatrix)
    {
    }

    public virtual global::Doroti.Framework.Animation.Animation<double> scale => this.animation;
    internal static Matrix4 _handleScaleMatrix(double value) => Matrix4.diagonal3Values(value, value, 1.0);
}

public class RotationTransition : MatrixTransition
{
    public RotationTransition(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Animation<double> turns = default!, global::Doroti.Framework.Painting.Alignment alignment = default!, FilterQuality? filterQuality = null, Widget? child = null) : base(key: key, alignment: alignment ?? global::Doroti.Framework.Painting.Alignment.center, filterQuality: filterQuality, child: child, animation: turns, onTransform: (global::System.Func<double, Matrix4>)_handleTurnsMatrix)
    {
    }

    public virtual global::Doroti.Framework.Animation.Animation<double> turns => this.animation;
    internal static Matrix4 _handleTurnsMatrix(double value) => Matrix4.rotationZ(((value * Dart_mathLibrary.pi) * 2.0));
}

public class SizeTransition : AnimatedWidget
{
    public virtual global::Doroti.Framework.Painting.Axis axis { get; private set; } = default!;
    public virtual double? axisAlignment { get; private set; }
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry? alignment { get; private set; }
    public virtual double? fixedCrossAxisSizeFactor { get; private set; }
    public virtual Widget? child { get; private set; }

    public SizeTransition(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.Axis axis = global::Doroti.Framework.Painting.Axis.vertical, global::Doroti.Framework.Animation.Animation<double> sizeFactor = default!, double? axisAlignment = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, double? fixedCrossAxisSizeFactor = null, Widget? child = null) : base(key: key, listenable: sizeFactor)
    {
        this.axis = axis;
        this.axisAlignment = axisAlignment;
        this.alignment = alignment;
        this.fixedCrossAxisSizeFactor = fixedCrossAxisSizeFactor;
        this.child = child;
        System.Diagnostics.Debug.Assert(((fixedCrossAxisSizeFactor is null) || (fixedCrossAxisSizeFactor >= 0.0)));
        System.Diagnostics.Debug.Assert(((axisAlignment is null) || (alignment is null)));
    }

    public virtual global::Doroti.Framework.Animation.Animation<double> sizeFactor => ((global::Doroti.Framework.Animation.Animation<double>?)(object?)this.listenable)!;
    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new ClipRect(child: new Align(alignment: (this.alignment ?? (this.axis switch { global::Doroti.Framework.Painting.Axis.horizontal => new global::Doroti.Framework.Painting.AlignmentDirectional((this.axisAlignment ?? 0.0), -1.0), global::Doroti.Framework.Painting.Axis.vertical => new global::Doroti.Framework.Painting.AlignmentDirectional(-1.0, (this.axisAlignment ?? 0.0)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })), heightFactor: ((object.Equals(this.axis, global::Doroti.Framework.Painting.Axis.vertical)) ? Math.Max(((global::Doroti.Framework.Animation.Animation<double>)this.sizeFactor).value, 0.0) : this.fixedCrossAxisSizeFactor), widthFactor: ((object.Equals(this.axis, global::Doroti.Framework.Painting.Axis.horizontal)) ? Math.Max(((global::Doroti.Framework.Animation.Animation<double>)this.sizeFactor).value, 0.0) : this.fixedCrossAxisSizeFactor), child: this.child)));
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
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new global::Doroti.Framework.Rendering.RenderAnimatedOpacity(opacity: this.opacity, alwaysIncludeSemantics: this.alwaysIncludeSemantics));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderAnimatedOpacity)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.RenderAnimatedOpacity>)(() =>
{
    var __cascade = __renderObject;
    __cascade.opacity = this.opacity;
    __cascade.alwaysIncludeSemantics = this.alwaysIncludeSemantics;
    return __cascade;
}))());
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Animation.Animation<double>>("opacity", this.opacity));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("alwaysIncludeSemantics", value: this.alwaysIncludeSemantics, ifTrue: "alwaysIncludeSemantics"));
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
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new global::Doroti.Framework.Rendering.RenderSliverAnimatedOpacity(opacity: this.opacity, alwaysIncludeSemantics: this.alwaysIncludeSemantics));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderSliverAnimatedOpacity)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.RenderSliverAnimatedOpacity>)(() =>
{
    var __cascade = __renderObject;
    __cascade.opacity = this.opacity;
    __cascade.alwaysIncludeSemantics = this.alwaysIncludeSemantics;
    return __cascade;
}))());
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Animation.Animation<double>>("opacity", this.opacity));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("alwaysIncludeSemantics", value: this.alwaysIncludeSemantics, ifTrue: "alwaysIncludeSemantics"));
    }

}

public class RelativeRectTween : global::Doroti.Framework.Animation.Tween<global::Doroti.Framework.Rendering.RelativeRect>
{
    public RelativeRectTween(global::Doroti.Framework.Rendering.RelativeRect? begin = null, global::Doroti.Framework.Rendering.RelativeRect? end = null) : base(begin: begin, end: end)
    {
    }

    public override global::Doroti.Framework.Rendering.RelativeRect lerp(double t) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RelativeRect>(RelativeRect.lerp(this.begin, this.end, t)!);
}

public class PositionedTransition : AnimatedWidget
{
    public virtual Widget child { get; private set; } = default!;

    public PositionedTransition(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Rendering.RelativeRect> rect = default!, Widget child = default!) : base(key: key, listenable: rect)
    {
        this.child = child;
    }

    public virtual global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Rendering.RelativeRect> rect => ((global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Rendering.RelativeRect>?)(object?)this.listenable)!;
    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)Positioned.CreateFromRelativeRect(rect: ((global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Rendering.RelativeRect>)this.rect).value, child: this.child));
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

    public virtual global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Rect?> rect => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Rect?>>(((global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Rect?>?)(object?)this.listenable)!);
    public override Widget build(BuildContext context)
    {
        var offsets__34419 = global::Doroti.Framework.Rendering.RelativeRect.CreateFromSize((((global::Doroti.Framework.Animation.Animation<Rect?>)this.rect).value ?? Rect.zero), this.size);
        return ((Widget)(object?)new Positioned(top: ((global::Doroti.Framework.Rendering.RelativeRect)offsets__34419).top, right: ((global::Doroti.Framework.Rendering.RelativeRect)offsets__34419).right, bottom: ((global::Doroti.Framework.Rendering.RelativeRect)offsets__34419).bottom, left: ((global::Doroti.Framework.Rendering.RelativeRect)offsets__34419).left, child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DecoratedBoxTransition : AnimatedWidget
{
    public virtual global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.Decoration> decoration { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.DecorationPosition position { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public DecoratedBoxTransition(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.Decoration> decoration = default!, global::Doroti.Framework.Rendering.DecorationPosition position = global::Doroti.Framework.Rendering.DecorationPosition.background, Widget child = default!) : base(key: key, listenable: decoration)
    {
        this.decoration = decoration;
        this.position = position;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new DecoratedBox(decoration: ((global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.Decoration>)this.decoration).value, position: this.position, child: this.child));
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

    public virtual global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.AlignmentGeometry> alignment => ((global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.AlignmentGeometry>?)(object?)this.listenable)!;
    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new Align(alignment: ((global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.AlignmentGeometry>)this.alignment).value, widthFactor: this.widthFactor, heightFactor: this.heightFactor, child: this.child));
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

    public DefaultTextStyleTransition(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.TextStyle> style = default!, Widget child = default!, TextAlign? textAlign = null, bool softWrap = true, global::Doroti.Framework.Painting.TextOverflow overflow = global::Doroti.Framework.Painting.TextOverflow.clip, long? maxLines = null) : base(key: key, listenable: style)
    {
        this.child = child;
        this.textAlign = textAlign;
        this.softWrap = softWrap;
        this.overflow = overflow;
        this.maxLines = maxLines;
    }

    public virtual global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.TextStyle> style => ((global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.TextStyle>?)(object?)this.listenable)!;
    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new DefaultTextStyle(style: ((global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.TextStyle>)this.style).value, textAlign: this.textAlign, softWrap: this.softWrap, overflow: this.overflow, maxLines: this.maxLines, child: this.child));
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
    public override Widget build(BuildContext context) => this.builder(context, this.child);
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
