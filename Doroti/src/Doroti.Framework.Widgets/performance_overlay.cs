// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/performance_overlay.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class PerformanceOverlay : LeafRenderObjectWidget
{
    public virtual long optionsMask { get; private set; } = default!;

    public PerformanceOverlay(Key? key = null, long optionsMask = 0) : base(key: key)
    {
        this.optionsMask = optionsMask;
    }

    public static PerformanceOverlay CreateAllEnabled(Key? key = null)
    {
        var __instance = new PerformanceOverlay(key, default!);
        __instance.optionsMask = (1L << (int)FoundationRuntimePorts.EnumIndex(PerformanceOverlayOption.displayRasterizerStatistics)) | (1L << (int)FoundationRuntimePorts.EnumIndex(PerformanceOverlayOption.visualizeRasterizerStatistics)) | (1L << (int)FoundationRuntimePorts.EnumIndex(PerformanceOverlayOption.displayEngineStatistics)) | (1L << (int)FoundationRuntimePorts.EnumIndex(PerformanceOverlayOption.visualizeEngineStatistics));
        return __instance;
    }

    public override RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<RenderObject>(new RenderPerformanceOverlay(optionsMask: optionsMask));
    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderPerformanceOverlay)renderObject;
        __renderObject.optionsMask = optionsMask;
    }

}

