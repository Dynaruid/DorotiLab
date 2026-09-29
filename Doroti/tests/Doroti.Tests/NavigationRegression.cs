using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Hosting;
using Doroti.Runtime;
using Doroti.Testing;
using Doroti.Ui;

internal static class NavigationRegression
{
    private static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
    public static void Run()
    {
        string? checkpoint = null;
        using (var navigation = new ApplicationNavigationHost(save: value => checkpoint = value))
        {
            navigation.Activate(new("one", "/first", ApplicationActivationSource.Protocol, false));
            navigation.Activate(new("two", "/second", ApplicationActivationSource.Protocol, false));
            Require(!navigation.Activate(new("one", "/first", ApplicationActivationSource.Protocol, false)), "Duplicate delivery accepted.");
            using var tester = new WidgetTester(navigation: navigation);
            tester.pumpWidget(new Doroti.Framework.Cupertino.CupertinoApp(home: new SizedBox()));
            tester.pumpAndSettle();
            Require(navigation.Current.Location == "/", "Navigator reported RouteSettings.ToString instead of its route name.");
            var provider = new PlatformRouteInformationProvider(new(uri: DartUri.parse("/")));
            var received = new List<string>();
            Action listener = () => received.Add(provider.value.uri.ToString());
            provider.addListener(listener);
            Require(received.SequenceEqual(new[] { "/first", "/second" }), "Pre-Router activations lost or reordered.");
            navigation.Activate(new("three", "/first", ApplicationActivationSource.BrowserHistory, false, "{\"selection\":2}"));
            Require(provider.value.uri.ToString() == "/first" && provider.value.state is not null, "Back navigation/state not delivered.");
            provider.routerReportsNewRouteInformation(new(uri: DartUri.parse("/edited")), RouteInformationReportingType.navigate);
            Require(navigation.Current.Location == "/edited", "Router did not update host route.");
            provider.removeListener(listener);
            provider.dispose();
            var manager = ServicesBinding.instance.restorationManager;
            var root = manager.rootBucket;
            tester.pump();
            var bucket = root.GetAwaiter().GetResult()!;
            bucket.write("text", "한글");
            tester.pump();
            manager.flushData();
            Require(navigation.ReadRestoration() is { Length: > 0 }, "Framework restoration did not reach checkpoint.");
        }
        using (var restored = new ApplicationNavigationHost(checkpoint: checkpoint, save: _ => { }))
        using (var tester = new WidgetTester(navigation: restored))
        {
            Require(restored.PreviousShutdownWasClean && restored.Current.Location == "/edited", "Graceful route checkpoint not restored.");
            var root = ServicesBinding.instance.restorationManager.rootBucket;
            tester.pump();
            Require(root.GetAwaiter().GetResult()!.read<string>("text") == "한글", "Framework Korean state was not restored.");
        }
        using (var linked = new ApplicationNavigationHost("/explicit", checkpoint))
            Require(linked.Current.Location == "/explicit" && linked.ReadRestoration() is null, "Saved stack overrode explicit cold link.");
        foreach (var invalid in new[] { "{", "{\"version\":99}", "{\"version\":1,\"location\":\"javascript:alert(1)\"}",
            "{\"version\":1,\"location\":\"/\",\"cleanShutdown\":true,\"data\":\"!invalid-base64!\"}",
            "{\"version\":1,\"location\":\"/\",\"cleanShutdown\":true,\"data\":42}" })
        using (var fallback = new ApplicationNavigationHost(checkpoint: invalid))
            Require(fallback.Current.Location == "/" && fallback.RestoreFailure is not null && !fallback.PreviousShutdownWasClean,
                "Corrupt/unsupported state did not discard the complete checkpoint, including its clean-shutdown flag.");
        using (var host = new ApplicationNavigationHost())
        {
            for (var index = 0; index < 32; index++) host.Activate(new(index.ToString(), "/", ApplicationActivationSource.Protocol, false));
            try { host.Activate(new("overflow", "/", ApplicationActivationSource.Protocol, false)); throw new Exception("Unbounded activation queue."); }
            catch (InvalidOperationException) { }
        }
        Console.WriteLine("PASS: activation queue/dedup/back state; Router report; version/corruption/cold-link precedence; framework restoration round trip.");
    }
}
