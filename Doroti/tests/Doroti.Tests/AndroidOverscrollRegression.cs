using Doroti.Framework.Painting;
using Doroti.Framework.Scheduler;
using Doroti.Framework.Widgets;
using Doroti.Testing;
using Doroti.Ui;

internal static class AndroidOverscrollRegression
{
    public static void Run()
    {
        foreach (var axis in new[] { Axis.vertical, Axis.horizontal })
        {
            using var tester = new WidgetTester(new Size(320, 480), operatingSystem: HostOperatingSystem.android);
            var controller = new ScrollController();
            try
            {
                tester.pumpWidget(new Directionality(textDirection: TextDirection.ltr,
                    child: new SingleChildScrollView(controller: controller, scrollDirection: axis,
                        physics: new ClampingScrollPhysics(), child: new SizedBox(
                            width: axis == Axis.horizontal ? 1000 : 320,
                            height: axis == Axis.vertical ? 1000 : 480,
                            child: new ColoredBox(color: new Color(0xff336699))))));
                if (tester.byType<GlowingOverscrollIndicator>().Count != 1)
                    throw new Exception($"Android {axis} scroll view did not create its edge glow.");

                var scrollable = tester.byType<SingleChildScrollView>().Single();
                var pull = axis == Axis.vertical ? new Offset(40, 150) : new Offset(150, 40);
                tester.drag(scrollable, pull);
                tester.pumpAndSettle(step: TimeSpan.FromMilliseconds(100));
                if (controller.position.pixels != controller.position.minScrollExtent)
                    throw new Exception($"Android {axis} overscroll changed the clamped scroll position.");

                // Removing an actively glowing view must retire its animation,
                // displacement ticker and delayed recede callback.
                tester.drag(scrollable, pull);
                tester.pump(TimeSpan.FromMilliseconds(50));
                tester.pumpWidget(new SizedBox());
                tester.pump();
                if (tester.Clock.PendingTimers != 0 || SchedulerBinding.instance.transientCallbackCount != 0)
                    throw new Exception($"Android {axis} edge glow retained callbacks after unmount.");
            }
            finally
            {
                controller.dispose();
            }
        }
        Console.WriteLine("Android overscroll: PASS; idle paint, pull/recede and active unmount on both axes.");
    }
}
