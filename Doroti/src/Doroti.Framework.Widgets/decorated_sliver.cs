// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/decorated_sliver.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class DecoratedSliver : SingleChildRenderObjectWidget
{
    public virtual Decoration decoration { get; private set; } = default!;
    public virtual DecorationPosition position { get; private set; } = default!;

    public DecoratedSliver(Key? key = null, Decoration decoration = default!, DecorationPosition position = DecorationPosition.background, Widget? sliver = null) : base(key: key, child: sliver)
    {
        this.decoration = decoration;
        this.position = position;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderDecoratedSliver(decoration: decoration, position: position, configuration: ImageLibrary.createLocalImageConfiguration(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderDecoratedSliver)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderDecoratedSliver>)(() =>
{
    var __cascade = __renderObject;
    __cascade.decoration = decoration;
    __cascade.position = position;
    __cascade.configuration = ImageLibrary.createLocalImageConfiguration(context);
    return __cascade;
}))());
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string label = position switch { DecorationPosition.background => "bg", DecorationPosition.foreground => "fg", _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        properties.add(new EnumProperty<DecorationPosition>("position", position, level: DiagnosticLevel.hidden));
        properties.add(new DiagnosticsProperty<Decoration>(label, decoration));
    }

}

