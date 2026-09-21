// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/container.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class DecoratedBox : SingleChildRenderObjectWidget
{
    public virtual Decoration decoration { get; private set; } = default!;
    public virtual DecorationPosition position { get; private set; } = default!;

    public DecoratedBox(
        Key? key = null,
        Decoration decoration = default!,
        DecorationPosition position = DecorationPosition.background,
        Widget? child = null
    )
        : base(key: key, child: child)
    {
        this.decoration = decoration;
        this.position = position;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderDecoratedBox(
            decoration: decoration,
            position: position,
            configuration: ImageLibrary.createLocalImageConfiguration(context)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderDecoratedBox)renderObject;
        DartRuntimePrimitives.Ignore(
            (
                (Func<RenderDecoratedBox>)(
                    () =>
                    {
                        var __cascade = __renderObject;
                        __cascade.decoration = decoration;
                        __cascade.configuration = ImageLibrary.createLocalImageConfiguration(
                            context
                        );
                        __cascade.position = position;
                        return __cascade;
                    }
                )
            )()
        );
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string label = position switch
        {
            DecorationPosition.background => "bg",
            DecorationPosition.foreground => "fg",
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        properties.add(
            new EnumProperty<DecorationPosition>(
                "position",
                position,
                level: DiagnosticLevel.hidden
            )
        );
        properties.add(new DiagnosticsProperty<Decoration>(label, decoration));
    }
}

public class Container : StatelessWidget
{
    public virtual Widget? child { get; private set; }
    public virtual AlignmentGeometry? alignment { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual bool isAntiAlias { get; private set; } = default!;
    public virtual Decoration? decoration { get; private set; }
    public virtual Decoration? foregroundDecoration { get; private set; }
    public virtual BoxConstraints? constraints { get; private set; }
    public virtual EdgeInsetsGeometry? margin { get; private set; }
    public virtual Matrix4? transform { get; private set; }
    public virtual AlignmentGeometry? transformAlignment { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    public Container(
        Key? key = null,
        AlignmentGeometry? alignment = null,
        EdgeInsetsGeometry? padding = null,
        Color? color = null,
        bool isAntiAlias = true,
        Decoration? decoration = null,
        Decoration? foregroundDecoration = null,
        double? width = null,
        double? height = null,
        BoxConstraints? constraints = null,
        EdgeInsetsGeometry? margin = null,
        Matrix4? transform = null,
        AlignmentGeometry? transformAlignment = null,
        Widget? child = null,
        Clip clipBehavior = Clip.none
    )
        : base(key: key)
    {
        this.alignment = alignment;
        this.padding = padding;
        this.color = color;
        this.isAntiAlias = isAntiAlias;
        this.decoration = decoration;
        this.foregroundDecoration = foregroundDecoration;
        this.margin = margin;
        this.transform = transform;
        this.transformAlignment = transformAlignment;
        this.child = child;
        this.clipBehavior = clipBehavior;
        this.constraints =
            ((width is not null) || (height is not null))
                ? (
                    constraints?.tighten(width: width, height: height)
                    ?? BoxConstraints.CreateTightFor(width: width, height: height)
                )
                : constraints;
        System.Diagnostics.Debug.Assert((margin is null) || margin.isNonNegative);
        System.Diagnostics.Debug.Assert((padding is null) || padding.isNonNegative);
        System.Diagnostics.Debug.Assert((decoration is null) || decoration.debugAssertIsValid());
        System.Diagnostics.Debug.Assert((constraints is null) || constraints.debugAssertIsValid());
        System.Diagnostics.Debug.Assert(
            (decoration is not null) || Equals(clipBehavior, Clip.none)
        );
        System.Diagnostics.Debug.Assert((color is null) || (decoration is null));
    }

    internal virtual EdgeInsetsGeometry? _paddingIncludingDecoration
    {
        get
        {
            return (padding, decoration?.padding) switch
            {
                (null, null) => null,
                (null, EdgeInsetsGeometry decorationPadding) => decorationPadding,
                (EdgeInsetsGeometry widgetPadding, null) => widgetPadding,
                (EdgeInsetsGeometry widgetPadding, EdgeInsetsGeometry decorationPadding) =>
                    widgetPadding.add(decorationPadding),
            };
        }
    }

    public override Widget build(BuildContext context)
    {
        Widget? current = child;
        if ((child is null) && ((constraints is null) || !constraints!.isTight))
        {
            current = DartRuntimePrimitives.ConvertValue<Widget>(
                new LimitedBox(
                    maxWidth: 0.0,
                    maxHeight: 0.0,
                    child: new ConstrainedBox(constraints: BoxConstraints.CreateExpand())
                )
            );
        }
        else
        {
            if (alignment is not null)
            {
                current = DartRuntimePrimitives.ConvertValue<Widget>(
                    new Align(alignment: alignment!, child: current)
                );
            }
        }
        EdgeInsetsGeometry? effectivePadding = _paddingIncludingDecoration;
        if (effectivePadding is not null)
        {
            current = DartRuntimePrimitives.ConvertValue<Widget>(
                new Padding(padding: effectivePadding, child: current)
            );
        }
        if (color is not null)
        {
            current = DartRuntimePrimitives.ConvertValue<Widget>(
                new ColoredBox(color: color!, isAntiAlias: isAntiAlias, child: current)
            );
        }
        if (!Equals(clipBehavior, Clip.none))
        {
            DartRuntimePrimitives.Assert(() => decoration is not null);
            current = DartRuntimePrimitives.ConvertValue<Widget>(
                new ClipPath(
                    clipper: new _DecorationClipper__container(
                        textDirection: Directionality.maybeOf(context),
                        decoration: decoration!
                    ),
                    clipBehavior: clipBehavior,
                    child: current
                )
            );
        }
        if (decoration is not null)
        {
            current = DartRuntimePrimitives.ConvertValue<Widget>(
                new DecoratedBox(decoration: decoration!, child: current)
            );
        }
        if (foregroundDecoration is not null)
        {
            current = DartRuntimePrimitives.ConvertValue<Widget>(
                new DecoratedBox(
                    decoration: foregroundDecoration!,
                    position: DecorationPosition.foreground,
                    child: current
                )
            );
        }
        if (constraints is not null)
        {
            current = DartRuntimePrimitives.ConvertValue<Widget>(
                new ConstrainedBox(constraints: constraints!, child: current)
            );
        }
        if (margin is not null)
        {
            current = DartRuntimePrimitives.ConvertValue<Widget>(
                new Padding(padding: margin!, child: current)
            );
        }
        if (transform is not null)
        {
            current = DartRuntimePrimitives.ConvertValue<Widget>(
                new Transform(transform: transform!, alignment: transformAlignment, child: current)
            );
        }
        return current!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<AlignmentGeometry>(
                "alignment",
                alignment,
                showName: false,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<EdgeInsetsGeometry>("padding", padding, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<Clip>("clipBehavior", clipBehavior, defaultValue: Clip.none)
        );
        if (color is not null)
        {
            properties.add(new DiagnosticsProperty<Color>("bg", color));
        }
        else
        {
            properties.add(
                new DiagnosticsProperty<Decoration>("bg", decoration, defaultValue: null)
            );
        }
        properties.add(
            new DiagnosticsProperty<Decoration>("fg", foregroundDecoration, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<BoxConstraints>("constraints", constraints, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<EdgeInsetsGeometry>("margin", margin, defaultValue: null)
        );
        properties.add(ObjectFlagProperty<Matrix4>.CreateHas("transform", transform));
    }
}

internal class _DecorationClipper__container : CustomClipper<Path>
{
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual Decoration decoration { get; private set; } = default!;

    internal _DecorationClipper__container(
        TextDirection? textDirection = null,
        Decoration decoration = default!
    )
    {
        this.decoration = decoration;
        this.textDirection = textDirection ?? TextDirection.ltr;
    }

    public override Path getClip(Size size)
    {
        return decoration.getClipPath(Offset.zero & size, (textDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldReclip(CustomClipper<Path> oldClipper)
    {
        var __oldClipper = (_DecorationClipper__container)oldClipper;
        return (!Equals(__oldClipper.decoration, decoration))
            || (!Equals(__oldClipper.textDirection, textDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
