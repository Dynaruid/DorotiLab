// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/container.dart
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

public class DecoratedBox : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Framework.Painting.Decoration decoration { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.DecorationPosition position { get; private set; } = default!;

    public DecoratedBox(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.Decoration decoration = default!, global::Doroti.Framework.Rendering.DecorationPosition position = global::Doroti.Framework.Rendering.DecorationPosition.background, Widget? child = null) : base(key: key, child: child)
    {
        this.decoration = decoration;
        this.position = position;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new global::Doroti.Framework.Rendering.RenderDecoratedBox(decoration: this.decoration, position: this.position, configuration: global::Doroti.Framework.Widgets.ImageLibrary.createLocalImageConfiguration(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderDecoratedBox)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.RenderDecoratedBox>)(() =>
{
    var __cascade = __renderObject;
    __cascade.decoration = this.decoration;
    __cascade.configuration = global::Doroti.Framework.Widgets.ImageLibrary.createLocalImageConfiguration(context);
    __cascade.position = this.position;
    return __cascade;
}))());
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string label__3031 = (this.position switch { global::Doroti.Framework.Rendering.DecorationPosition.background => "bg", global::Doroti.Framework.Rendering.DecorationPosition.foreground => "fg", _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Rendering.DecorationPosition>("position", this.position, level: global::Doroti.Framework.Foundation.DiagnosticLevel.hidden));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.Decoration>(label__3031, this.decoration));
    }

}

public class Container : StatelessWidget
{
    public virtual Widget? child { get; private set; }
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry? alignment { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual bool isAntiAlias { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.Decoration? decoration { get; private set; }
    public virtual global::Doroti.Framework.Painting.Decoration? foregroundDecoration { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin { get; private set; }
    public virtual Matrix4? transform { get; private set; }
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry? transformAlignment { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    public Container(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, Color? color = null, bool isAntiAlias = true, global::Doroti.Framework.Painting.Decoration? decoration = null, global::Doroti.Framework.Painting.Decoration? foregroundDecoration = null, double? width = null, double? height = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin = null, Matrix4? transform = null, global::Doroti.Framework.Painting.AlignmentGeometry? transformAlignment = null, Widget? child = null, Clip clipBehavior = Clip.none) : base(key: key)
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
        this.constraints = ((((width is not null) || (height is not null))) ? (constraints?.tighten(width: width, height: height) ?? global::Doroti.Framework.Rendering.BoxConstraints.CreateTightFor(width: width, height: height)) : constraints);
        System.Diagnostics.Debug.Assert(((margin is null) || ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)margin).isNonNegative));
        System.Diagnostics.Debug.Assert(((padding is null) || ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)padding).isNonNegative));
        System.Diagnostics.Debug.Assert(((decoration is null) || decoration.debugAssertIsValid()));
        System.Diagnostics.Debug.Assert(((constraints is null) || constraints.debugAssertIsValid()));
        System.Diagnostics.Debug.Assert(((decoration is not null) || (object.Equals(clipBehavior, Clip.none))));
        System.Diagnostics.Debug.Assert(((color is null) || (decoration is null)));
    }

    internal virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? _paddingIncludingDecoration
    {
        get
        {
            return ((this.padding, this.decoration?.padding) switch
            {
                (null, null) => null,
                (null, global::Doroti.Framework.Painting.EdgeInsetsGeometry decorationPadding) => decorationPadding,
                (global::Doroti.Framework.Painting.EdgeInsetsGeometry widgetPadding, null) => widgetPadding,
                (global::Doroti.Framework.Painting.EdgeInsetsGeometry widgetPadding, global::Doroti.Framework.Painting.EdgeInsetsGeometry decorationPadding) => widgetPadding.add(decorationPadding),
            });
            return default!;
        }
    }
    public override Widget build(BuildContext context)
    {
        Widget? current__14769 = this.child;
        if (((this.child is null) && (((this.constraints is null) || !this.constraints!.isTight))))
        {
            current__14769 = DartRuntimePrimitives.ConvertValue<Widget>(new LimitedBox(maxWidth: 0.0, maxHeight: 0.0, child: new ConstrainedBox(constraints: global::Doroti.Framework.Rendering.BoxConstraints.CreateExpand())));
        }
        else
        {
            if ((this.alignment is not null))
            {
                current__14769 = DartRuntimePrimitives.ConvertValue<Widget>(new Align(alignment: this.alignment!, child: current__14769));
            }
        }
        global::Doroti.Framework.Painting.EdgeInsetsGeometry? effectivePadding__15156 = this._paddingIncludingDecoration;
        if ((effectivePadding__15156 is not null))
        {
            current__14769 = DartRuntimePrimitives.ConvertValue<Widget>(new Padding(padding: effectivePadding__15156, child: current__14769));
        }
        if ((this.color is not null))
        {
            current__14769 = DartRuntimePrimitives.ConvertValue<Widget>(new ColoredBox(color: this.color!, isAntiAlias: this.isAntiAlias, child: current__14769));
        }
        if ((!object.Equals(this.clipBehavior, Clip.none)))
        {
            DartRuntimePrimitives.Assert(() => (this.decoration is not null));
            current__14769 = DartRuntimePrimitives.ConvertValue<Widget>(new ClipPath(clipper: new _DecorationClipper__container(textDirection: Directionality.maybeOf(context), decoration: this.decoration!), clipBehavior: this.clipBehavior, child: current__14769));
        }
        if ((this.decoration is not null))
        {
            current__14769 = DartRuntimePrimitives.ConvertValue<Widget>(new DecoratedBox(decoration: this.decoration!, child: current__14769));
        }
        if ((this.foregroundDecoration is not null))
        {
            current__14769 = DartRuntimePrimitives.ConvertValue<Widget>(new DecoratedBox(decoration: this.foregroundDecoration!, position: global::Doroti.Framework.Rendering.DecorationPosition.foreground, child: current__14769));
        }
        if ((this.constraints is not null))
        {
            current__14769 = DartRuntimePrimitives.ConvertValue<Widget>(new ConstrainedBox(constraints: this.constraints!, child: current__14769));
        }
        if ((this.margin is not null))
        {
            current__14769 = DartRuntimePrimitives.ConvertValue<Widget>(new Padding(padding: this.margin!, child: current__14769));
        }
        if ((this.transform is not null))
        {
            current__14769 = DartRuntimePrimitives.ConvertValue<Widget>(new Transform(transform: this.transform!, alignment: this.transformAlignment, child: current__14769));
        }
        return current__14769!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.AlignmentGeometry>("alignment", this.alignment, showName: false, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior, defaultValue: Clip.none));
        if ((this.color is not null))
        {
            properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Color>("bg", this.color));
        }
        else
        {
            properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.Decoration>("bg", this.decoration, defaultValue: null));
        }
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.Decoration>("fg", this.foregroundDecoration, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("constraints", this.constraints, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("margin", this.margin, defaultValue: null));
        properties.add(global::Doroti.Framework.Foundation.ObjectFlagProperty<Matrix4>.CreateHas("transform", this.transform));
    }

}

internal class _DecorationClipper__container : global::Doroti.Framework.Rendering.CustomClipper<Path>
{
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.Decoration decoration { get; private set; } = default!;

    internal _DecorationClipper__container(TextDirection? textDirection = null, global::Doroti.Framework.Painting.Decoration decoration = default!)
    {
        this.decoration = decoration;
        this.textDirection = (textDirection ?? TextDirection.ltr);
    }

    public override Path getClip(Size size)
    {
        return ((Path)(object?)this.decoration.getClipPath((Offset.zero & size), DartRuntimePrimitives.RequireValue(this.textDirection)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldReclip(global::Doroti.Framework.Rendering.CustomClipper<Path> oldClipper)
    {
        var __oldClipper = (_DecorationClipper__container)(object)oldClipper;
        return ((!object.Equals(((_DecorationClipper__container)__oldClipper).decoration, this.decoration)) || (!object.Equals(((_DecorationClipper__container)__oldClipper).textDirection, this.textDirection)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
