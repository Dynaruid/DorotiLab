// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scroll_context.dart
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public interface ScrollContext
{
    public BuildContext? notificationContext { get; }
    public BuildContext storageContext { get; }
    public Scheduler.TickerProvider vsync { get; }
    public AxisDirection axisDirection { get; }
    public double devicePixelRatio { get; }
    public void setIgnorePointer(bool value);
    public void setCanDrag(bool value);
    public void setSemanticsActions(HashSet<SemanticsAction> actions);
    public void saveOffset(double offset);
}
