using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Host.Maui;
using Doroti.Runtime;
using Doroti.Testing;
using Doroti.Ui;

internal static class AndroidBackNavigationRegression
{
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    public static void Run()
    {
        var fallback = new Fallback();
        using var back = new AndroidBackNavigation(fallback);
        using (var tester = new WidgetTester(operatingSystem: HostOperatingSystem.android, platformMessages: back))
        {
            back.Attach(tester.View);
            Require(WidgetsBinding.instance.lifecycleState == AppLifecycleState.resumed,
                "The binding did not read the host's initial lifecycle state.");
            var systemPops = 0;
            back.SystemPopRequested += () => systemPops++;
            var key = new GlobalKey<NavigatorState>();
            tester.pumpWidget(new WidgetsApp(navigatorKey: key, color: new Color(0xff000000),
                textStyle: new Doroti.Framework.Painting.TextStyle(color: new Color(0xffffffff)),
                pageRouteBuilder: (settings, builder) => Page(builder, settings), home: new Text("home")));
            tester.pumpAndSettle();
            Require(!back.FrameworkHandlesBack, "The root route must allow native back-to-home.");
            var lifecycle = new LifecycleObserver();
            WidgetsBinding.instance.addObserver(lifecycle);
            tester.View.DispatchPlatformEvent(() =>
                tester.View.platformDispatcher.onAppLifecycleStateChanged!(tester.View, AppLifecycleState.paused));
            tester.View.DispatchPlatformEvent(() =>
                tester.View.platformDispatcher.onAppLifecycleStateChanged!(tester.View, AppLifecycleState.resumed));
            Require(WidgetsBinding.instance.lifecycleState == AppLifecycleState.resumed &&
                lifecycle.States.SequenceEqual(new[] { AppLifecycleState.inactive, AppLifecycleState.hidden,
                    AppLifecycleState.paused, AppLifecycleState.hidden, AppLifecycleState.inactive, AppLifecycleState.resumed }),
                "Typed host lifecycle events did not reach binding observers in order.");
            WidgetsBinding.instance.removeObserver(lifecycle);

            var page = Page(_ => new Text("page"));
            var pageResult = key.currentState!.push(page).asTask();
            tester.pumpAndSettle();
            Require(back.FrameworkHandlesBack, $"A pushed page did not enable Android Back (lifecycle={WidgetsBinding.instance.lifecycleState}, canPop={key.currentState.canPop()}).");
            var dialog = key.currentState.push(new RawDialogRoute<object>(
                pageBuilder: (_, _, _) => new Text("dialog"), barrierDismissible: false,
                transitionDuration: Duration.zero)).asTask();
            var nested = key.currentState.push(new RawDialogRoute<object>(
                pageBuilder: (_, _, _) => new Text("nested"), transitionDuration: Duration.zero)).asTask();
            tester.pumpAndSettle();
            Task delivery;
            using (var foreignDispatcher = new PlatformDispatcher())
            using (foreignDispatcher.EnterScope()) delivery = back.RequestBack();
            tester.pumpUntilComplete(delivery); tester.pumpAndSettle();
            Require(nested.IsCompletedSuccessfully && !dialog.IsCompleted && !pageResult.IsCompleted && systemPops == 0,
                "Back must dismiss only the topmost dialog.");
            tester.pumpUntilComplete(back.RequestBack()); tester.pumpAndSettle();
            Require(dialog.IsCompletedSuccessfully && !pageResult.IsCompleted && systemPops == 0,
                "A non-dismissible barrier incorrectly prevented system Back or closed the page.");

            var removed = 0;
            page.addLocalHistoryEntry(new LocalHistoryEntry(onRemove: () => removed++));
            tester.pumpAndSettle();
            tester.pumpUntilComplete(back.RequestBack()); tester.pumpAndSettle();
            Require(removed == 1 && !pageResult.IsCompleted && systemPops == 0,
                "Local history (for example a persistent bottom sheet) must close before its page.");
            tester.pumpUntilComplete(back.RequestBack()); tester.pumpAndSettle();
            Require(pageResult.IsCompletedSuccessfully && !back.FrameworkHandlesBack && systemPops == 0,
                "Page Back did not return to the root without exiting.");

            var blocked = 0;
            var protectedResult = key.currentState.push(Page(_ => new PopScope<object>(canPop: false,
                onPopInvokedWithResult: (didPop, _) => { if (!didPop) blocked++; }, child: new Text("protected")))).asTask();
            tester.pumpAndSettle();
            tester.pumpUntilComplete(back.RequestBack()); tester.pumpAndSettle();
            Require(back.FrameworkHandlesBack && blocked == 1 && !protectedResult.IsCompleted && systemPops == 0,
                "PopScope veto was bypassed by Android Back.");
            key.currentState.pop<object>(); tester.pumpAndSettle();
            tester.pumpUntilComplete(back.RequestBack()); tester.pumpAndSettle();
            Require(systemPops == 1, "Unhandled framework Back must request native fallback exactly once.");
            tester.View.DispatchPlatformEvent(() => DartRuntimePrimitives.Ignore(SystemNavigator.pop()));
            Require(systemPops == 2, "Explicit SystemNavigator.pop did not reach the native host.");

            var previousCalls = fallback.Calls;
            tester.View.DispatchPlatformEvent(() => DartRuntimePrimitives.Ignore(
                SystemChannels.platform.invokeMethod<object?>("unrelated")));
            Require(fallback.Calls == previousCalls + 1, "Unrelated platform methods were swallowed by the Back bridge.");

            tester.pumpWidget(new WidgetsApp(navigatorKey: new GlobalKey<NavigatorState>(), color: new Color(0xff000000),
                textStyle: new Doroti.Framework.Painting.TextStyle(color: new Color(0xffffffff)),
                pageRouteBuilder: (settings, builder) => Page(builder, settings),
                home: new PopScope<object>(canPop: false,
                    onPopInvokedWithResult: (didPop, _) => { if (!didPop) blocked++; }, child: new Text("protected root"))));
            tester.pumpAndSettle();
            tester.pumpUntilComplete(back.RequestBack()); tester.pumpAndSettle();
            Require(back.FrameworkHandlesBack && blocked == 2 && systemPops == 2,
                "A PopScope on the root page must keep Android Back inside the app.");
        }

        using var pendingBack = new AndroidBackNavigation(new Fallback());
        using (var tester = new WidgetTester(operatingSystem: HostOperatingSystem.android, platformMessages: pendingBack))
        {
            pendingBack.Attach(tester.View);
            tester.pumpWidget(new SizedBox());
            var observer = new PendingObserver();
            WidgetsBinding.instance.addObserver(observer);
            var pending = pendingBack.RequestBack(); pendingBack.RequestBack();
            Require(observer.Calls == 1, "Repeated Back raced an outstanding async route decision.");
            tester.View.DispatchPlatformEvent(() => observer.Decision.complete(true));
            tester.pumpUntilComplete(pending);
            WidgetsBinding.instance.removeObserver(observer);
            var router = new RootBackButtonDispatcher();
            var routerCalls = 0;
            Func<Future<bool>> callback = () => { routerCalls++; return Future<bool>.value(true); };
            router.addCallback(callback);
            tester.pumpUntilComplete(pendingBack.RequestBack());
            Require(routerCalls == 1, "Router's BackButtonDispatcher did not receive system Back.");
            router.removeCallback(callback);
            pendingBack.Dispose();
            pendingBack.RequestBack();
            Require(observer.Calls == 1 && routerCalls == 1, "A disposed view retained Back callbacks.");
        }
        Console.WriteLine("PASS: Android Back channel; stacked dialogs, barrier policy, page/local history, PopScope, root fallback, Router, async coalescing, owner scope, lifecycle and disposal (CPU/synthetic).");
    }

    private static PageRouteBuilder<object> Page(Func<BuildContext, Widget> builder, RouteSettings? settings = null) =>
        new(settings: settings, pageBuilder: (context, _, _) => builder(context),
            transitionDuration: Duration.zero, reverseTransitionDuration: Duration.zero);

    private sealed class PendingObserver : WidgetsBindingObserver
    {
        internal int Calls;
        internal readonly Completer<bool> Decision = new();
        public Future<bool> didPopRoute() { Calls++; return Decision.future; }
    }

    private sealed class LifecycleObserver : WidgetsBindingObserver
    {
        internal readonly List<AppLifecycleState> States = [];
        public void didChangeAppLifecycleState(AppLifecycleState state) => States.Add(state);
    }

    private sealed class Fallback : IPlatformMessageHostCapability
    {
        internal int Calls;
        public ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data,
            CancellationToken cancellationToken = default)
        { Calls++; return ValueTask.FromResult<ReadOnlyMemory<byte>?>(null); }
        public void SetMessageHandler(string channel, PlatformMessageHandler? handler) { }
    }
}
