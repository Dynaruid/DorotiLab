using Doroti.Cupertino;
using Doroti.Framework.Widgets;
using Doroti.Testing;
using Doroti.Ui;

internal static class CupertinoContextMenuRegression
{
    internal static void Run()
    {
        foreach (var presentation in new[] { WindowPresentation.Auto, WindowPresentation.Overlay })
        {
            using var tester = new WidgetTester(new Size(420, 720));
            var key = GlobalKey<IState>.Create();
            BuildContext? caller = null;
            var selected = 0;
            tester.pumpWidget(new CupertinoApp(home: new Builder(builder: context =>
            {
                caller = context;
                return new Center(child: new CupertinoContextMenu(key: key, presentation: presentation,
                    child: new Container(width: 80, height: 60, color: new Color(0xff3377aa), child: new Text("Preview")),
                    actions: [new CupertinoContextMenuAction(child: new Text("Choose"), onPressed: () =>
                    { selected++; Navigator.of(caller!, rootNavigator: true).pop<object?>(null); })]));
            })));
            tester.longPress(tester.byKey(key).Single());
            tester.pumpAndSettle();
            if (tester.text("Choose").Count != 1) throw new Exception("Cupertino context menu did not open through pointer input.");
            var point = tester.center(tester.text("Choose").Single());
            tester.tap(tester.text("Choose").Single());
            tester.pumpAndSettle();
            if (selected != 1 || tester.text("Choose").Count != 0 || tester.byKey(key).Count != 1)
                throw new Exception($"Context menu selection did not restore its caller exactly once: policy={presentation}, selected={selected}, actions={tester.text("Choose").Count}, owners={tester.byKey(key).Count}, point={point}.");
            tester.pumpWidget(new SizedBox());
            tester.pumpAndSettle();
        }
        Console.WriteLine("PASS: pointer-opened Cupertino context menu, Auto unsupported fallback, Overlay selection/closing animation and caller restoration (CPU).");
    }
}
