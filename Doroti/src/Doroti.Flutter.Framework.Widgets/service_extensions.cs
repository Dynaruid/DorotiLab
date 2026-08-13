// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/service_extensions.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

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

