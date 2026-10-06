using Doroti.Framework.Widgets;
using Doroti.Testing;
using Doroti.Ui;

internal static class ActiveScrollSemanticsRegression
{
    public static void Run()
    {
        using var tester = new WidgetTester();
        var semantics = ((WidgetsFlutterBinding)WidgetsBinding.instance).ensureSemantics();
        tester.pumpWidget(new Semantics(label: "Blur page", textDirection: TextDirection.ltr, container: true,
            child: new SizedBox(width: 100, height: 100)));
        tester.pump();
        var trace = PlatformDispatcher.instance.frameTrace;
        trace.RecordScroll(DorotiFramePhase.scrollStart, tester.View.viewId, 99,
            0, null, "BallisticScrollActivity");
        try
        {
            if (!trace.HasActiveScrollActivity) throw new Exception("Regression did not retain active scrolling.");
            tester.pumpWidget(new Semantics(label: "Components page", textDirection: TextDirection.ltr, selected: true, container: true,
                child: new SizedBox(width: 100, height: 100)));
            tester.pump();
            if (tester.Semantics?.nodes.Any(node => node.label == "Components page"
                && node.flags?.isSelected == Tristate.isTrue) != true)
                throw new Exception("Active scrolling deferred a new page's semantics content.");
            if (!trace.HasActiveScrollActivity) throw new Exception("Content flush incorrectly ended scrolling.");
        }
        finally
        {
            trace.RecordScroll(DorotiFramePhase.scrollEnd, tester.View.viewId, 99,
                0, null, "IdleScrollActivity");
            semantics.dispose();
        }
        Console.WriteLine("Active-scroll semantics: PASS; changed page content is delivered before scrolling ends.");
    }
}
