// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/annotated_region.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class AnnotatedRegion<T> : SingleChildRenderObjectWidget
{
    public virtual T value { get; private set; } = default!;
    public virtual bool sized { get; private set; } = default!;

    public AnnotatedRegion(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!, T value = default!, bool sized = true) : base(key: key, child: child)
    {
        this.value = value;
        this.sized = sized;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return new global::Doroti.Framework.Rendering.RenderAnnotatedRegion<T>(value: value, sized: sized);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderAnnotatedRegion<T>)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.RenderAnnotatedRegion<T>>)(() =>
{
    var __cascade = __renderObject;
    __cascade.value = value;
    __cascade.sized = sized;
    return __cascade;
}))());
    }

}

