using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Testing;
using Doroti.Ui;

internal static class ModalDismissalRegression
{
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    public static void Run()
    {
        foreach (var platform in new[] { HostOperatingSystem.windows, HostOperatingSystem.linux,
            HostOperatingSystem.macOS, HostOperatingSystem.iOS })
        {
            using var tester = new WidgetTester(operatingSystem: platform);
            var navigator = new GlobalKey<NavigatorState>();
            using var homeFocus = new FocusNode();
            tester.pumpWidget(new WidgetsApp(navigatorKey: navigator, color: new Color(0xff000000),
                textStyle: new Doroti.Framework.Painting.TextStyle(color: new Color(0xffffffff)),
                pageRouteBuilder: (settings, builder) => new PageRouteBuilder<object>(settings: settings,
                    pageBuilder: (context, _, _) => builder(context), transitionDuration: Duration.zero),
                home: new Focus(focusNode: homeFocus, autofocus: true, child: new Text("home"))));
            tester.pumpAndSettle();
            var first = navigator.currentState!.push(Dialog()).asTask();
            var second = navigator.currentState.push(Dialog()).asTask();
            tester.pumpAndSettle();
            Escape(tester);
            tester.pumpUntilComplete(second);
            tester.pumpAndSettle();
            Require(second.IsCompletedSuccessfully && !first.IsCompleted,
                $"{platform}: Escape did not dismiss only the topmost modal.");
            Escape(tester);
            tester.pumpUntilComplete(first);
            tester.pumpAndSettle();
            Require(first.IsCompletedSuccessfully && !navigator.currentState.canPop(),
                $"{platform}: Escape did not return to the home route.");
            Require(homeFocus.hasFocus, $"{platform}: Modal dismissal did not restore the page's focus.");
            var locked = navigator.currentState.push(Dialog(dismissible: false)).asTask();
            tester.pumpAndSettle(); Escape(tester);
            Require(!locked.IsCompleted, $"{platform}: Escape bypassed barrierDismissible=false.");
            navigator.currentState.pop<object>(); tester.pumpAndSettle();
            var vetoes = 0;
            var vetoed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var guarded = navigator.currentState.push(Dialog(child: new PopScope<object>(canPop: false,
                onPopInvokedWithResult: (didPop, _) => { if (!didPop) { vetoes++; vetoed.TrySetResult(); } },
                child: new Text("guarded")))).asTask();
            tester.pumpAndSettle(); Escape(tester);
            tester.pumpUntilComplete(vetoed.Task);
            Require(!guarded.IsCompleted && vetoes == 1, $"{platform}: Escape bypassed PopScope.");
            navigator.currentState.pop<object>(); tester.pumpAndSettle();
            Escape(tester);
            Require(!navigator.currentState.canPop(), $"{platform}: Escape removed the root route.");
        }
        Console.WriteLine("PASS: Windows/Linux/macOS/iOS modal Escape; stacked dialogs, barrier policy, PopScope and root (CPU/synthetic).");
    }

    private static RawDialogRoute<object> Dialog(bool dismissible = true, Widget? child = null) =>
        new(pageBuilder: (_, _, _) => child ?? new Text("dialog"), barrierDismissible: dismissible,
            transitionDuration: Duration.zero);

    private static void Escape(WidgetTester tester)
    {
        tester.sendKey(new(1, tester.Clock.Elapsed, KeyEventType.down, 0x70029, 0x10000001b, false));
        tester.sendKey(new(1, tester.Clock.Elapsed, KeyEventType.up, 0x70029, 0x10000001b, false));
        tester.pumpAndSettle();
    }
}
