// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/service_extensions.dart
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

public enum WidgetsServiceExtensions
{
    debugDumpApp,
    debugDumpFocusTree,
    showPerformanceOverlay,
    didSendFirstFrameEvent,
    didSendFirstFrameRasterizedEvent,
    fastReassemble,
    profileWidgetBuilds,
    profileUserWidgetBuilds,
    debugAllowBanner,
    accessibilityEvaluations
}

public enum WidgetInspectorServiceExtensions
{
    structuredErrors,
    show,
    trackRebuildDirtyWidgets,
    widgetLocationIdMap,
    trackRepaintWidgets,
    disposeAllGroups,
    disposeGroup,
    isWidgetTreeReady,
    disposeId,
    setPubRootDirectories,
    addPubRootDirectories,
    removePubRootDirectories,
    getPubRootDirectories,
    setSelectionById,
    getParentChain,
    getProperties,
    getChildren,
    getChildrenSummaryTree,
    getChildrenDetailsSubtree,
    getRootWidget,
    getRootWidgetTree,
    getRootWidgetSummaryTree,
    getRootWidgetSummaryTreeWithPreviews,
    getDetailsSubtree,
    getSelectedWidget,
    getSelectedSummaryWidget,
    isWidgetCreationTracked,
    screenshot,
    getLayoutExplorerNode,
    setFlexFit,
    setFlexFactor,
    setFlexProperties
}

