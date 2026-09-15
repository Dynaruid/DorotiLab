// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/performance_overlay.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class PerformanceOverlay : LeafRenderObjectWidget
{
    public virtual long optionsMask { get; private set; } = default!;

    public PerformanceOverlay(global::Doroti.Framework.Foundation.Key? key = null, long optionsMask = 0) : base(key: key)
    {
        this.optionsMask = optionsMask;
    }

    public static PerformanceOverlay CreateAllEnabled(global::Doroti.Framework.Foundation.Key? key = null)
    {
        var __instance = new PerformanceOverlay(key, default!);
        __instance.optionsMask = ((((1L << (int)(FoundationRuntimePorts.EnumIndex(global::Doroti.Framework.Rendering.PerformanceOverlayOption.displayRasterizerStatistics))) | (1L << (int)(FoundationRuntimePorts.EnumIndex(global::Doroti.Framework.Rendering.PerformanceOverlayOption.visualizeRasterizerStatistics)))) | (1L << (int)(FoundationRuntimePorts.EnumIndex(global::Doroti.Framework.Rendering.PerformanceOverlayOption.displayEngineStatistics)))) | (1L << (int)(FoundationRuntimePorts.EnumIndex(global::Doroti.Framework.Rendering.PerformanceOverlayOption.visualizeEngineStatistics))));
        return __instance;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(new global::Doroti.Framework.Rendering.RenderPerformanceOverlay(optionsMask: this.optionsMask));
    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderPerformanceOverlay)(object)renderObject;
        __renderObject.optionsMask = this.optionsMask;
    }

}

