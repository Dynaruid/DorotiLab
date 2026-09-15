// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scroll_context.dart
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

public interface ScrollContext
{
    public BuildContext? notificationContext { get; }
    public BuildContext storageContext { get; }
    public global::Doroti.Framework.Scheduler.TickerProvider vsync { get; }
    public global::Doroti.Framework.Painting.AxisDirection axisDirection { get; }
    public double devicePixelRatio { get; }
    public void setIgnorePointer(bool value);
    public void setCanDrag(bool value);
    public void setSemanticsActions(HashSet<SemanticsAction> actions);
    public void saveOffset(double offset);
}

