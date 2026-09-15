// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/decorated_sliver.dart
#pragma warning disable CS8600, CS8603
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class DecoratedSliver : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Framework.Painting.Decoration decoration { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.DecorationPosition position { get; private set; } = default!;

    public DecoratedSliver(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.Decoration decoration = default!, global::Doroti.Framework.Rendering.DecorationPosition position = global::Doroti.Framework.Rendering.DecorationPosition.background, Widget? sliver = null) : base(key: key, child: sliver)
    {
        this.decoration = decoration;
        this.position = position;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new global::Doroti.Framework.Rendering.RenderDecoratedSliver(decoration: this.decoration, position: this.position, configuration: global::Doroti.Framework.Widgets.ImageLibrary.createLocalImageConfiguration(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderDecoratedSliver)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.RenderDecoratedSliver>)(() =>
{
    var __cascade = __renderObject;
    __cascade.decoration = this.decoration;
    __cascade.position = this.position;
    __cascade.configuration = global::Doroti.Framework.Widgets.ImageLibrary.createLocalImageConfiguration(context);
    return __cascade;
}))());
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string label = (this.position switch { global::Doroti.Framework.Rendering.DecorationPosition.background => "bg", global::Doroti.Framework.Rendering.DecorationPosition.foreground => "fg", _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Rendering.DecorationPosition>("position", this.position, level: global::Doroti.Framework.Foundation.DiagnosticLevel.hidden));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.Decoration>(label, this.decoration));
    }

}

