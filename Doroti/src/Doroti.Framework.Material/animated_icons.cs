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

namespace Doroti.Framework.Material;

public class AnimatedIcon : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Animation.Animation<double> progress { get; private set; } = default!;
    public virtual Color? color { get; private set; }
    public virtual double? size { get; private set; }
    public virtual AnimatedIconData icon { get; private set; } = default!;
    public virtual string? semanticLabel { get; private set; }
    public virtual TextDirection? textDirection { get; private set; }

    public AnimatedIcon(global::Doroti.Framework.Foundation.Key? key = null, AnimatedIconData icon = default!, global::Doroti.Framework.Animation.Animation<double> progress = default!, Color? color = null, double? size = null, string? semanticLabel = null, TextDirection? textDirection = null) : base(key: key)
    {
        this.icon = icon;
        this.progress = progress;
        this.color = color;
        this.size = size;
        this.semanticLabel = semanticLabel;
        this.textDirection = textDirection;
    }

    internal static global::Doroti.Ui.Path _pathFactory() => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Path>(new global::Doroti.Ui.Path());
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        var iconData = ((_AnimatedIconData__animated_icons_data?)(object?)icon)!;
        global::Doroti.Framework.Widgets.IconThemeData iconTheme = ((global::Doroti.Framework.Widgets.IconThemeData)(object?)IconTheme.of(context));
        DartRuntimePrimitives.Assert(() => ((global::Doroti.Framework.Widgets.IconThemeData)iconTheme).isConcrete);
        double iconSize = (size ?? DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.IconThemeData)iconTheme).size));
        global::Doroti.Ui.TextDirection textDirectionLocal = ((((TextDirection?)((dynamic)this).textDirection) ?? (TextDirection)Directionality.of(context)));
        double iconOpacity = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Widgets.IconThemeData)iconTheme).opacity);
        global::Doroti.Ui.Color iconColor = ((global::Doroti.Ui.Color)(object?)(color ?? ((global::Doroti.Framework.Widgets.IconThemeData)iconTheme).color!));
        if ((iconOpacity != 1.0))
        {
            iconColor = iconColor.withOpacity((iconColor.opacity * iconOpacity));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(label: semanticLabel, child: new global::Doroti.Framework.Widgets.CustomPaint(size: new global::Doroti.Ui.Size(iconSize, iconSize), painter: new _AnimatedIconPainter__animated_icons(paths: ((_AnimatedIconData__animated_icons_data)iconData).paths, progress: progress, color: iconColor, scale: (iconSize / ((_AnimatedIconData__animated_icons_data)iconData).size.width), shouldMirror: ((object.Equals(DartRuntimePrimitives.RequireValue(textDirectionLocal), TextDirection.rtl)) && ((_AnimatedIconData__animated_icons_data)iconData).matchTextDirection), uiPathFactory: (global::System.Func<Path>)_pathFactory))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate Path _UiPathFactory__animated_icons();

internal class _AnimatedIconPainter__animated_icons : global::Doroti.Framework.Rendering.CustomPainter
{
    public virtual List<_PathFrames__animated_icons> paths { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> progress { get; private set; } = default!;
    public virtual Color color { get; private set; } = default!;
    public virtual double scale { get; private set; } = default!;
    public virtual bool shouldMirror { get; private set; } = default!;
    public virtual global::System.Func<Path> uiPathFactory { get; private set; } = default!;

    internal _AnimatedIconPainter__animated_icons(List<_PathFrames__animated_icons> paths, global::Doroti.Framework.Animation.Animation<double> progress, Color color, double scale, bool shouldMirror, global::System.Func<Path> uiPathFactory) : base(repaint: progress)
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
        double clampedProgress = Dart_uiLibrary.clampDouble(((global::Doroti.Framework.Animation.Animation<double>)progress).value, 0.0, 1.0);
        foreach (_PathFrames__animated_icons path in paths)
        {
            path.paint(canvas, color, (global::System.Func<Path>)uiPathFactory, clampedProgress);
        }
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldDelegate = (_AnimatedIconPainter__animated_icons)(object)oldDelegate;
        return (((((((_AnimatedIconPainter__animated_icons)__oldDelegate).progress.value != ((global::Doroti.Framework.Animation.Animation<double>)progress).value) || (!object.Equals(((_AnimatedIconPainter__animated_icons)__oldDelegate).color, color))) || (!object.Equals(((_AnimatedIconPainter__animated_icons)__oldDelegate).paths, paths))) || (((_AnimatedIconPainter__animated_icons)__oldDelegate).scale != scale)) || (!object.Equals((global::System.Func<Path>)((_AnimatedIconPainter__animated_icons)__oldDelegate).uiPathFactory, (global::System.Func<Path>)uiPathFactory)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool? hitTest(Offset position) => DartRuntimePrimitives.ConvertValue<bool>(null);
    public override bool shouldRebuildSemantics(global::Doroti.Framework.Rendering.CustomPainter oldDelegate) => false;
    public override global::System.Func<Size, List<global::Doroti.Framework.Rendering.CustomPainterSemantics>>? semanticsBuilder => DartRuntimePrimitives.ConvertValue<global::System.Func<Size, List<global::Doroti.Framework.Rendering.CustomPainterSemantics>>>(null);
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
        double opacityLocal = DartRuntimePrimitives.RequireValue(Animated_iconsLibrary._interpolate<double>(opacities, progress, (a, b, t) => Dart_uiLibrary.lerpDouble(a, b, t) ?? 0.0));
        var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.style = PaintingStyle.fill;
    __cascade.color = color.withOpacity((color.opacity * opacityLocal));
    return __cascade;
}))();
        global::Doroti.Ui.Path path = ((global::Doroti.Ui.Path)(object?)uiPathFactory());
        foreach (_PathCommand__animated_icons command in commands)
        {
            command.apply(path, progress);
        }
        canvas.drawPath(path, paintLocal);
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
        global::Doroti.Ui.Offset offset = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(Animated_iconsLibrary._interpolate<global::Doroti.Ui.Offset>(points, progress, (a, b, t) => Offset.lerp(a, b, t)!.Value)));
        path.moveTo(offset.dx, offset.dy);
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
        global::Doroti.Ui.Offset controlPoint1 = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(Animated_iconsLibrary._interpolate<global::Doroti.Ui.Offset>(controlPoints1, progress, (a, b, t) => Offset.lerp(a, b, t)!.Value)));
        global::Doroti.Ui.Offset controlPoint2 = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(Animated_iconsLibrary._interpolate<global::Doroti.Ui.Offset>(controlPoints2, progress, (a, b, t) => Offset.lerp(a, b, t)!.Value)));
        global::Doroti.Ui.Offset targetPoint = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(Animated_iconsLibrary._interpolate<global::Doroti.Ui.Offset>(targetPoints, progress, (a, b, t) => Offset.lerp(a, b, t)!.Value)));
        path.cubicTo(controlPoint1.dx, controlPoint1.dy, controlPoint2.dx, controlPoint2.dy, targetPoint.dx, targetPoint.dy);
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
        global::Doroti.Ui.Offset point = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(Animated_iconsLibrary._interpolate<global::Doroti.Ui.Offset>(points, progress, (a, b, t) => Offset.lerp(a, b, t)!.Value)));
        path.lineTo(point.dx, point.dy);
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
        double targetIdx = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0L, (checked((long)(values.Count)) - 1L), progress));
        long lowIdx = targetIdx.floor();
        long highIdx = targetIdx.ceil();
        double t = (targetIdx - lowIdx);
        return interpolator(values[(int)(lowIdx)], values[(int)(highIdx)], t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal delegate T _Interpolator__animated_icons<T>(T a, T b, double progress);
