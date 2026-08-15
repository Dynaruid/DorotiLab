// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/performance_overlay.dart
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

namespace Doroti.Generated.Framework.Widgets;

public class PerformanceOverlay : LeafRenderObjectWidget
{
    public virtual long optionsMask { get; private set; } = default!;

    public PerformanceOverlay(global::Doroti.Generated.Framework.Foundation.Key? key = null, long optionsMask = 0) : base(key: key)
    {
        this.optionsMask = optionsMask;
    }

    public static PerformanceOverlay CreateAllEnabled(global::Doroti.Generated.Framework.Foundation.Key? key = null)
    {
        var __instance = new PerformanceOverlay(default!, default!);
        __instance.optionsMask = ((((1L << (int)(FoundationRuntimePorts.EnumIndex(global::Doroti.Generated.Framework.Rendering.PerformanceOverlayOption.displayRasterizerStatistics))) | (1L << (int)(FoundationRuntimePorts.EnumIndex(global::Doroti.Generated.Framework.Rendering.PerformanceOverlayOption.visualizeRasterizerStatistics)))) | (1L << (int)(FoundationRuntimePorts.EnumIndex(global::Doroti.Generated.Framework.Rendering.PerformanceOverlayOption.displayEngineStatistics)))) | (1L << (int)(FoundationRuntimePorts.EnumIndex(global::Doroti.Generated.Framework.Rendering.PerformanceOverlayOption.visualizeEngineStatistics))));
        return __instance;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(new global::Doroti.Generated.Framework.Rendering.RenderPerformanceOverlay(optionsMask: this.optionsMask));
    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderPerformanceOverlay)(object)renderObject;
        __renderObject.optionsMask = this.optionsMask;
    }

}

