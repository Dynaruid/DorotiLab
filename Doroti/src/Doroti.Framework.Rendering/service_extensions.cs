// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/service_extensions.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Rendering;

public enum RenderingServiceExtensions
{
    invertOversizedImages,
    debugPaint,
    debugPaintBaselinesEnabled,
    repaintRainbow,
    debugDumpLayerTree,
    debugDisableClipLayers,
    debugDisablePhysicalShapeLayers,
    debugDisableOpacityLayers,
    debugDumpRenderTree,
    debugDumpSemanticsTreeInTraversalOrder,
    debugDumpSemanticsTreeInInverseHitTestOrder,
    profileRenderObjectPaints,
    profileRenderObjectLayouts
}

