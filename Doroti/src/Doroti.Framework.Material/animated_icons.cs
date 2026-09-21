// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/animated_icons/animated_icons.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class AnimatedIcon : StatelessWidget
{
    public virtual Animation<double> progress { get; private set; } = default!;
    public virtual Color? color { get; private set; }
    public virtual double? size { get; private set; }
    public virtual AnimatedIconData icon { get; private set; } = default!;
    public virtual string? semanticLabel { get; private set; }
    public virtual TextDirection? textDirection { get; private set; }

    public AnimatedIcon(
        Key? key = null,
        AnimatedIconData icon = default!,
        Animation<double> progress = default!,
        Color? color = null,
        double? size = null,
        string? semanticLabel = null,
        TextDirection? textDirection = null
    )
        : base(key: key)
    {
        this.icon = icon;
        this.progress = progress;
        this.color = color;
        this.size = size;
        this.semanticLabel = semanticLabel;
        this.textDirection = textDirection;
    }

    internal static Path _pathFactory() => DartRuntimePrimitives.ConvertValue<Path>(new Path());

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            Widgets.DebugLibrary.debugCheckHasDirectionality(context)
        );
        var iconData = ((_AnimatedIconData__animated_icons_data?)icon)!;
        IconThemeData iconTheme = IconTheme.of(context);
        DartRuntimePrimitives.Assert(() => iconTheme.isConcrete);
        double iconSize =
            size
            ?? (
                iconTheme.size
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        TextDirection textDirectionLocal = textDirection ?? Directionality.of(context);
        double iconOpacity = (
            iconTheme.opacity
            ?? throw new global::System.NullReferenceException("A required value was null.")
        );
        Color iconColor = color ?? iconTheme.color!;
        if (iconOpacity != 1.0)
        {
            iconColor = iconColor.withOpacity(iconColor.opacity * iconOpacity);
        }
        return new Widgets.Semantics(
            label: semanticLabel,
            child: new CustomPaint(
                size: new Size(iconSize, iconSize),
                painter: new _AnimatedIconPainter__animated_icons(
                    paths: iconData.paths,
                    progress: progress,
                    color: iconColor,
                    scale: iconSize / iconData.size.width,
                    shouldMirror: Equals((textDirectionLocal), TextDirection.rtl)
                        && iconData.matchTextDirection,
                    uiPathFactory: _pathFactory
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal delegate Path _UiPathFactory__animated_icons();

internal class _AnimatedIconPainter__animated_icons : CustomPainter
{
    public virtual List<_PathFrames__animated_icons> paths { get; private set; } = default!;
    public virtual Animation<double> progress { get; private set; } = default!;
    public virtual Color color { get; private set; } = default!;
    public virtual double scale { get; private set; } = default!;
    public virtual bool shouldMirror { get; private set; } = default!;
    public virtual Func<Path> uiPathFactory { get; private set; } = default!;

    internal _AnimatedIconPainter__animated_icons(
        List<_PathFrames__animated_icons> paths,
        Animation<double> progress,
        Color color,
        double scale,
        bool shouldMirror,
        Func<Path> uiPathFactory
    )
        : base(repaint: progress)
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
        double clampedProgress = DorotiUiLibrary.clampDouble(progress.value, 0.0, 1.0);
        foreach (_PathFrames__animated_icons path in paths)
        {
            path.paint(canvas, color, uiPathFactory, clampedProgress);
        }
    }

    public override bool shouldRepaint(CustomPainter oldDelegate)
    {
        var __oldDelegate = (_AnimatedIconPainter__animated_icons)oldDelegate;
        return (__oldDelegate.progress.value != progress.value)
            || (!Equals(__oldDelegate.color, color))
            || (!Equals(__oldDelegate.paths, paths))
            || (__oldDelegate.scale != scale)
            || (!Equals(__oldDelegate.uiPathFactory, uiPathFactory));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool? hitTest(Offset position) =>
        DartRuntimePrimitives.ConvertValue<bool>(null);

    public override bool shouldRebuildSemantics(CustomPainter oldDelegate) => false;

    public override Func<Size, List<CustomPainterSemantics>>? semanticsBuilder =>
        DartRuntimePrimitives.ConvertValue<Func<Size, List<CustomPainterSemantics>>>(null);
}

public class _PathFrames__animated_icons
{
    public virtual List<_PathCommand__animated_icons> commands { get; private set; } = default!;
    public virtual List<double> opacities { get; private set; } = default!;

    internal _PathFrames__animated_icons(
        List<_PathCommand__animated_icons> commands,
        List<double> opacities
    )
    {
        this.commands = commands;
        this.opacities = opacities;
    }

    public virtual void paint(Canvas canvas, Color color, Func<Path> uiPathFactory, double progress)
    {
        double opacityLocal = (
            Animated_iconsLibrary._interpolate(
                opacities,
                progress,
                (a, b, t) => DorotiUiLibrary.lerpDouble(a, b, t) ?? 0.0
            )
        );
        var paintLocal = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.style = PaintingStyle.fill;
                    __cascade.color = color.withOpacity(color.opacity * opacityLocal);
                    return __cascade;
                }
            )
        )();
        Path path = uiPathFactory();
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
        Offset offset = (
            Animated_iconsLibrary._interpolate(
                points,
                progress,
                (a, b, t) => Offset.lerp(a, b, t)!.Value
            )
        );
        path.moveTo(offset.dx, offset.dy);
    }
}

internal class _PathCubicTo__animated_icons : _PathCommand__animated_icons
{
    public virtual List<Offset> controlPoints2 { get; private set; } = default!;
    public virtual List<Offset> controlPoints1 { get; private set; } = default!;
    public virtual List<Offset> targetPoints { get; private set; } = default!;

    internal _PathCubicTo__animated_icons(
        List<Offset> controlPoints1,
        List<Offset> controlPoints2,
        List<Offset> targetPoints
    )
    {
        this.controlPoints1 = controlPoints1;
        this.controlPoints2 = controlPoints2;
        this.targetPoints = targetPoints;
    }

    public virtual void apply(Path path, double progress)
    {
        Offset controlPoint1 = (
            Animated_iconsLibrary._interpolate(
                controlPoints1,
                progress,
                (a, b, t) => Offset.lerp(a, b, t)!.Value
            )
        );
        Offset controlPoint2 = (
            Animated_iconsLibrary._interpolate(
                controlPoints2,
                progress,
                (a, b, t) => Offset.lerp(a, b, t)!.Value
            )
        );
        Offset targetPoint = (
            Animated_iconsLibrary._interpolate(
                targetPoints,
                progress,
                (a, b, t) => Offset.lerp(a, b, t)!.Value
            )
        );
        path.cubicTo(
            controlPoint1.dx,
            controlPoint1.dy,
            controlPoint2.dx,
            controlPoint2.dy,
            targetPoint.dx,
            targetPoint.dy
        );
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
        Offset point = (
            Animated_iconsLibrary._interpolate(
                points,
                progress,
                (a, b, t) => Offset.lerp(a, b, t)!.Value
            )
        );
        path.lineTo(point.dx, point.dy);
    }
}

internal class _PathClose__animated_icons : _PathCommand__animated_icons
{
    internal _PathClose__animated_icons() { }

    public virtual void apply(Path path, double progress)
    {
        path.close();
    }
}

public static partial class Animated_iconsLibrary
{
    internal static T _interpolate<T>(
        List<T> values,
        double progress,
        Func<T, T, double, T> interpolator
    )
    {
        DartRuntimePrimitives.Assert(() => progress <= 1.0);
        DartRuntimePrimitives.Assert(() => progress >= 0.0);
        if (checked(values.Count) == 1L)
        {
            return values[(int)0L];
        }
        double targetIdx = (
            DorotiUiLibrary.lerpDouble(0L, checked(values.Count) - 1L, progress)
            ?? throw new global::System.NullReferenceException("A required value was null.")
        );
        long lowIdx = targetIdx.floor();
        long highIdx = targetIdx.ceil();
        double t = targetIdx - lowIdx;
        return interpolator(values[(int)lowIdx], values[(int)highIdx], t);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal delegate T _Interpolator__animated_icons<T>(T a, T b, double progress);
