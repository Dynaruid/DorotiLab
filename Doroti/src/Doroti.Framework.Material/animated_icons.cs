// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/animated_icons/animated_icons.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class AnimatedIcon : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> progress { get; private set; } = default!;
    public virtual Color? color { get; private set; }
    public virtual double? size { get; private set; }
    public virtual AnimatedIconData icon { get; private set; } = default!;
    public virtual string? semanticLabel { get; private set; }
    public virtual TextDirection? textDirection { get; private set; }

    public AnimatedIcon(global::Doroti.Generated.Framework.Foundation.Key? key = null, AnimatedIconData icon = default!, global::Doroti.Generated.Framework.Animation.Animation<double> progress = default!, Color? color = null, double? size = null, string? semanticLabel = null, TextDirection? textDirection = null) : base(key: key)
    {
        this.icon = icon;
        this.progress = progress;
        this.color = color;
        this.size = size;
        this.semanticLabel = semanticLabel;
        this.textDirection = textDirection;
    }

    internal static global::Doroti.Ui.Path _pathFactory() => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Path>(new global::Doroti.Ui.Path());
    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        var iconData__3427 = ((_AnimatedIconData__animated_icons_data?)(object?)icon)!;
        global::Doroti.Generated.Framework.Widgets.IconThemeData iconTheme__3489 = ((global::Doroti.Generated.Framework.Widgets.IconThemeData)(object?)IconTheme.of(context));
        DartRuntimePrimitives.Assert(() => ((global::Doroti.Generated.Framework.Widgets.IconThemeData)iconTheme__3489).isConcrete);
        double iconSize__3575 = (size ?? DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Widgets.IconThemeData)iconTheme__3489).size));
        global::Doroti.Ui.TextDirection textDirection__3635 = ((((TextDirection?)((dynamic)this).textDirection) ?? (TextDirection)Directionality.of(context)));
        double iconOpacity__3718 = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Widgets.IconThemeData)iconTheme__3489).opacity);
        global::Doroti.Ui.Color iconColor__3762 = ((global::Doroti.Ui.Color)(object?)(color ?? ((global::Doroti.Generated.Framework.Widgets.IconThemeData)iconTheme__3489).color!));
        if ((iconOpacity__3718 != 1.0))
        {
            iconColor__3762 = iconColor__3762.withOpacity((iconColor__3762.opacity * iconOpacity__3718));
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(label: semanticLabel, child: new global::Doroti.Generated.Framework.Widgets.CustomPaint(size: new global::Doroti.Ui.Size(iconSize__3575, iconSize__3575), painter: new _AnimatedIconPainter__animated_icons(paths: ((_AnimatedIconData__animated_icons_data)iconData__3427).paths, progress: progress, color: iconColor__3762, scale: (iconSize__3575 / ((_AnimatedIconData__animated_icons_data)iconData__3427).size.width), shouldMirror: ((object.Equals(DartRuntimePrimitives.RequireValue(textDirection__3635), TextDirection.rtl)) && ((_AnimatedIconData__animated_icons_data)iconData__3427).matchTextDirection), uiPathFactory: (global::System.Func<Path>)_pathFactory))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate Path _UiPathFactory__animated_icons();

internal class _AnimatedIconPainter__animated_icons : global::Doroti.Generated.Framework.Rendering.CustomPainter
{
    public virtual List<_PathFrames__animated_icons> paths { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> progress { get; private set; } = default!;
    public virtual Color color { get; private set; } = default!;
    public virtual double scale { get; private set; } = default!;
    public virtual bool shouldMirror { get; private set; } = default!;
    public virtual global::System.Func<Path> uiPathFactory { get; private set; } = default!;

    internal _AnimatedIconPainter__animated_icons(List<_PathFrames__animated_icons> paths, global::Doroti.Generated.Framework.Animation.Animation<double> progress, Color color, double scale, bool shouldMirror, global::System.Func<Path> uiPathFactory) : base(repaint: progress)
    {
        this.paths = paths;
        this.progress = progress;
        this.color = color;
        this.scale = scale;
        this.shouldMirror = shouldMirror;
        this.uiPathFactory = uiPathFactory;
    }

    public override void paint(Canvas canvas, Size size)
    {
        if (shouldMirror)
        {
            canvas.rotate(Dart_mathLibrary.pi);
            canvas.translate(-size.width, -size.height);
        }
        canvas.scale(scale, scale);
        double clampedProgress__5457 = Dart_uiLibrary.clampDouble(((global::Doroti.Generated.Framework.Animation.Animation<double>)progress).value, 0.0, 1.0);
        foreach (_PathFrames__animated_icons path__5541 in paths)
        {
            path__5541.paint(canvas, color, (global::System.Func<Path>)uiPathFactory, clampedProgress__5457);
        }
    }

    public override bool shouldRepaint(global::Doroti.Generated.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldDelegate = (_AnimatedIconPainter__animated_icons)(object)oldDelegate;
        return (((((((_AnimatedIconPainter__animated_icons)__oldDelegate).progress.value != ((global::Doroti.Generated.Framework.Animation.Animation<double>)progress).value) || (!object.Equals(((_AnimatedIconPainter__animated_icons)__oldDelegate).color, color))) || (!object.Equals(((_AnimatedIconPainter__animated_icons)__oldDelegate).paths, paths))) || (((_AnimatedIconPainter__animated_icons)__oldDelegate).scale != scale)) || (!object.Equals((global::System.Func<Path>)((_AnimatedIconPainter__animated_icons)__oldDelegate).uiPathFactory, (global::System.Func<Path>)uiPathFactory)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool? hitTest(Offset position) => DartRuntimePrimitives.ConvertValue<bool>(null);
    public override bool shouldRebuildSemantics(global::Doroti.Generated.Framework.Rendering.CustomPainter oldDelegate) => false;
    public override global::System.Func<Size, List<global::Doroti.Generated.Framework.Rendering.CustomPainterSemantics>>? semanticsBuilder => DartRuntimePrimitives.ConvertValue<global::System.Func<Size, List<global::Doroti.Generated.Framework.Rendering.CustomPainterSemantics>>>(null);
}

public class _PathFrames__animated_icons
{
    public virtual List<_PathCommand__animated_icons> commands { get; private set; } = default!;
    public virtual List<double> opacities { get; private set; } = default!;

    internal _PathFrames__animated_icons(List<_PathCommand__animated_icons> commands, List<double> opacities)
    {
        this.commands = commands;
        this.opacities = opacities;
    }

    public virtual void paint(Canvas canvas, Color color, global::System.Func<Path> uiPathFactory, double progress)
    {
        double opacity__6554 = DartRuntimePrimitives.RequireValue(Animated_iconsLibrary._interpolate<double>(opacities, progress, (a, b, t) => Dart_uiLibrary.lerpDouble(a, b, t) ?? 0.0));
        var paint__6634 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.style = PaintingStyle.fill;
            __cascade.color = color.withOpacity((color.opacity * opacity__6554));
            return __cascade;        }))();
        global::Doroti.Ui.Path path__6766 = ((global::Doroti.Ui.Path)(object?)uiPathFactory());
        foreach (_PathCommand__animated_icons command__6818 in commands)
        {
            command__6818.apply(path__6766, progress);
        }
        canvas.drawPath(path__6766, paint__6634);
    }

}

public interface _PathCommand__animated_icons
{
    public void apply(Path path, double progress);
}

internal class _PathMoveTo__animated_icons : _PathCommand__animated_icons
{
    public virtual List<Offset> points { get; private set; } = default!;

    internal _PathMoveTo__animated_icons(List<Offset> points)
    {
        this.points = points;
    }

    public virtual void apply(Path path, double progress)
    {
        global::Doroti.Ui.Offset offset__7529 = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(Animated_iconsLibrary._interpolate<global::Doroti.Ui.Offset>(points, progress, (a, b, t) => Offset.lerp(a, b, t)!.Value)));
        path.moveTo(offset__7529.dx, offset__7529.dy);
    }

}

internal class _PathCubicTo__animated_icons : _PathCommand__animated_icons
{
    public virtual List<Offset> controlPoints2 { get; private set; } = default!;
    public virtual List<Offset> controlPoints1 { get; private set; } = default!;
    public virtual List<Offset> targetPoints { get; private set; } = default!;

    internal _PathCubicTo__animated_icons(List<Offset> controlPoints1, List<Offset> controlPoints2, List<Offset> targetPoints)
    {
        this.controlPoints1 = controlPoints1;
        this.controlPoints2 = controlPoints2;
        this.targetPoints = targetPoints;
    }

    public virtual void apply(Path path, double progress)
    {
        global::Doroti.Ui.Offset controlPoint1__7947 = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(Animated_iconsLibrary._interpolate<global::Doroti.Ui.Offset>(controlPoints1, progress, (a, b, t) => Offset.lerp(a, b, t)!.Value)));
        global::Doroti.Ui.Offset controlPoint2__8043 = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(Animated_iconsLibrary._interpolate<global::Doroti.Ui.Offset>(controlPoints2, progress, (a, b, t) => Offset.lerp(a, b, t)!.Value)));
        global::Doroti.Ui.Offset targetPoint__8139 = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(Animated_iconsLibrary._interpolate<global::Doroti.Ui.Offset>(targetPoints, progress, (a, b, t) => Offset.lerp(a, b, t)!.Value)));
        path.cubicTo(controlPoint1__7947.dx, controlPoint1__7947.dy, controlPoint2__8043.dx, controlPoint2__8043.dy, targetPoint__8139.dx, targetPoint__8139.dy);
    }

}

internal class _PathLineTo__animated_icons : _PathCommand__animated_icons
{
    public virtual List<Offset> points { get; private set; } = default!;

    internal _PathLineTo__animated_icons(List<Offset> points)
    {
        this.points = points;
    }

    public virtual void apply(Path path, double progress)
    {
        global::Doroti.Ui.Offset point__8590 = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(Animated_iconsLibrary._interpolate<global::Doroti.Ui.Offset>(points, progress, (a, b, t) => Offset.lerp(a, b, t)!.Value)));
        path.lineTo(point__8590.dx, point__8590.dy);
    }

}

internal class _PathClose__animated_icons : _PathCommand__animated_icons
{
    internal _PathClose__animated_icons()
    {
    }

    public virtual void apply(Path path, double progress)
    {
        path.close();
    }

}

public static partial class Animated_iconsLibrary
{
    internal static T _interpolate<T>(List<T> values, double progress, global::System.Func<T, T, double, T> interpolator)
    {
        DartRuntimePrimitives.Assert(() => (progress <= 1.0));
        DartRuntimePrimitives.Assert(() => (progress >= 0.0));
        if ((checked((long)(values.Count)) == 1L))
        {
            return values[(int)(0L)];
        }
        double targetIdx__9581 = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0L, (checked((long)(values.Count)) - 1L), progress));
        long lowIdx__9653 = targetIdx__9581.floor();
        long highIdx__9693 = targetIdx__9581.ceil();
        double t__9736 = (targetIdx__9581 - lowIdx__9653);
        return interpolator(values[(int)(lowIdx__9653)], values[(int)(highIdx__9693)], t__9736);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal delegate T _Interpolator__animated_icons<T>(T a, T b, double progress);
