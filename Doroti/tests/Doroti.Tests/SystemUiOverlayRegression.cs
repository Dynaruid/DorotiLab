using System.Text;
using Doroti.Framework.Rendering;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Hosting;
using Doroti.Runtime;
using Doroti.Testing;
using Doroti.Ui;

internal static class SystemUiOverlayRegression
{
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    internal static void Run()
    {
        CheckChannel();
        foreach (var platform in new[] { HostOperatingSystem.android, HostOperatingSystem.iOS })
            CheckRegions(platform);
        CheckViewOwnership();
        Console.WriteLine("PASS: system UI overlay channel validation, partial settings, cancellation, region selection/DPR, updates/removal, navigation restoration, coalescing, lifecycle and view ownership (CPU/synthetic).");
    }

    private static void CheckChannel()
    {
        var fallback = new Fallback();
        var applied = new List<SystemUiOverlaySettings>();
        var channel = new SystemUiOverlayPlatformMessageCapability(fallback, (style, _) =>
        { applied.Add(style); return ValueTask.CompletedTask; });
        ReadOnlyMemory<byte>? Send(string json, string name = "flutter/platform", CancellationToken token = default) =>
            channel.SendAsync(name, Encoding.UTF8.GetBytes(json), token).AsTask().GetAwaiter().GetResult();
        var codec = new JSONMethodCodec();
        codec.decodeEnvelope((ByteData)Send("""
            {"method":"SystemChrome.setSystemUIOverlayStyle","args":{
              "statusBarColor":4294901760,"statusBarBrightness":"Brightness.dark","statusBarIconBrightness":"light",
              "systemStatusBarContrastEnforced":false,"systemNavigationBarColor":4278190335,
              "systemNavigationBarDividerColor":0,"systemNavigationBarIconBrightness":"Brightness.dark",
              "systemNavigationBarContrastEnforced":true}}
            """)!.Value);
        Require(applied.Single() == new SystemUiOverlaySettings(0xffff0000, Brightness.dark, Brightness.light,
            false, 0xff0000ff, 0, Brightness.dark, true), "System UI style fields were lost or inverted by the channel.");
        codec.decodeEnvelope((ByteData)Send("""{"method":"SystemChrome.setSystemUIOverlayStyle","args":{"statusBarIconBrightness":"dark","statusBarColor":null}}""")!.Value);
        var merged = applied[0].Merge(applied[1]);
        Require(merged.StatusBarColor == 0xffff0000 && merged.StatusBarIconBrightness == Brightness.dark
            && merged.SystemStatusBarContrastEnforced == false && merged.SystemNavigationBarContrastEnforced == true,
            "Null/omitted style fields must preserve previous native values, including false.");
        foreach (var args in new[] { "null", "[]", "{\"statusBarColor\":-1}", "{\"statusBarColor\":4294967296}",
            "{\"statusBarIconBrightness\":\"invalid\"}", "{\"systemNavigationBarContrastEnforced\":0}" })
        {
            var response = Send("{\"method\":\"SystemChrome.setSystemUIOverlayStyle\",\"args\":" + args + "}");
            try { codec.decodeEnvelope((ByteData)response!.Value); throw new InvalidOperationException("Invalid overlay settings succeeded."); }
            catch (PlatformException) { }
        }
        Require(applied.Count == 2, "Invalid styles reached the native callback.");
        Send("{\"method\":\"SystemNavigator.pop\"}");
        Send("opaque payload", "plugin/channel");
        Require(fallback.Calls == 2, "Unrelated messages were swallowed by the overlay bridge.");
        channel.SetMessageHandler("flutter/platform", null);
        Require(fallback.Handlers == 1, "Incoming message registration was not forwarded.");
        using var cancelled = new CancellationTokenSource(); cancelled.Cancel();
        try { Send("{}", token: cancelled.Token); throw new InvalidOperationException("Cancellation was ignored."); }
        catch (OperationCanceledException) { }
    }

    private static void CheckRegions(HostOperatingSystem platform)
    {
        var received = new List<SystemUiOverlaySettings>();
        var channel = new SystemUiOverlayPlatformMessageCapability(new Fallback(), (style, _) =>
        { received.Add(style); return ValueTask.CompletedTask; });
        using var tester = new WidgetTester(size: new Size(320, 600), devicePixelRatio: 2,
            operatingSystem: platform, platformMessages: channel);
        var outer = new SystemUiOverlayStyle(statusBarColor: new Color(0xff112233), statusBarBrightness: Brightness.light,
            statusBarIconBrightness: Brightness.dark, systemNavigationBarColor: new Color(0xff445566),
            systemNavigationBarIconBrightness: Brightness.light);
        var upper = new SystemUiOverlayStyle(statusBarColor: new Color(0xffaabbcc), statusBarBrightness: Brightness.dark,
            statusBarIconBrightness: Brightness.light, systemNavigationBarColor: new Color(0xffdeadbe));
        Widget Region(SystemUiOverlayStyle value) => new AnnotatedRegion<SystemUiOverlayStyle>(value: outer,
            child: new Column(children: [new AnnotatedRegion<SystemUiOverlayStyle>(value: value,
                child: new SizedBox(height: 80, width: 320)), new Expanded(child: new SizedBox())]));
        tester.pumpWidget(Region(upper));
        Require(received.Count > 0, "AnnotatedRegion did not send a system bar style.");
        Require(received[^1].StatusBarColor == 0xffaabbcc && received[^1].StatusBarBrightness == Brightness.dark
            && received[^1].StatusBarIconBrightness == Brightness.light, "The topmost upper region did not own the status bar.");
        Require(received[^1].SystemNavigationBarColor == (platform == HostOperatingSystem.android ? 0xff445566u : null),
            "Navigation properties must come from the lower Android region and be omitted on iOS.");
        var count = received.Count;
        tester.View.DispatchPlatformEvent(() => RendererBinding.instance.renderViews.Single().compositeFrame());
        Require(received.Count == count, "An unchanged frame resent the system bar style.");
        tester.pumpWidget(Region(outer));
        Require(received[^1].StatusBarColor == 0xff112233, "Updating the annotation did not repaint the system bar style.");
        tester.pumpWidget(new SizedBox());
        count = received.Count;
        tester.View.DispatchPlatformEvent(() => RendererBinding.instance.renderViews.Single().compositeFrame());
        Require(received.Count == count, "Removing all annotations must leave the last system style unchanged.");
        tester.pumpWidget(new Directionality(textDirection: TextDirection.ltr,
            child: new Center(child: new AnnotatedRegion<SystemUiOverlayStyle>(value: upper,
                sized: false, child: new SizedBox(width: 10, height: 10)))));
        Require(received[^1].StatusBarColor == 0xffaabbcc, "An unsized annotation must apply outside its child's bounds.");
        count = received.Count;
        tester.pumpWidget(new Directionality(textDirection: TextDirection.ltr,
            child: new Center(child: new AnnotatedRegion<SystemUiOverlayStyle>(value: outer,
                sized: true, child: new SizedBox(width: 10, height: 10)))));
        Require(received.Count == count, "A sized annotation away from the bars must not change their style.");
        tester.View.DispatchPlatformEvent(() =>
        {
            SystemChrome.setSystemUIOverlayStyle(outer);
            SystemChrome.setSystemUIOverlayStyle(upper);
            SystemChrome.setSystemUIOverlayStyle(upper.copyWith(statusBarColor: new Color(0xff010203)));
        });
        Require(received.Count == count + 1 && received[^1].StatusBarColor == 0xff010203,
            "Only the last imperative style in an event should reach the host.");
        tester.View.DispatchPlatformEvent(() =>
        {
            SystemChrome.handleAppLifecycleStateChanged(AppLifecycleState.detached);
        });
        tester.View.DispatchPlatformEvent(() => SystemChrome.setSystemUIOverlayStyle(
            upper.copyWith(statusBarColor: new Color(0xff010203))));
        Require(received.Count == count + 2, "A detached view did not invalidate its style cache.");

        var navigator = new GlobalKey<NavigatorState>();
        tester.pumpWidget(new WidgetsApp(color: new Color(0xff000000), navigatorKey: navigator,
            pageRouteBuilder: (settings, builder) => Page(builder, settings),
            home: new AnnotatedRegion<SystemUiOverlayStyle>(value: outer, child: new SizedBox(width: double.PositiveInfinity, height: double.PositiveInfinity))));
        tester.pumpAndSettle();
        var route = navigator.currentState!.push(Page(_ =>
            new AnnotatedRegion<SystemUiOverlayStyle>(value: upper, child: new SizedBox(width: double.PositiveInfinity, height: double.PositiveInfinity)))).asTask();
        tester.pumpAndSettle();
        Require(received[^1].StatusBarColor == 0xffaabbcc, "The pushed page did not own the system style.");
        navigator.currentState.pop<object>(); tester.pumpAndSettle();
        Require(route.IsCompletedSuccessfully && received[^1].StatusBarColor == 0xff112233,
            "Popping a page did not restore the underlying page's system style.");
    }

    private static PageRouteBuilder<object> Page(Func<BuildContext, Widget> builder, RouteSettings? settings = null) =>
        new(settings: settings, pageBuilder: (context, _, _) => builder(context),
            transitionDuration: Duration.zero, reverseTransitionDuration: Duration.zero);

    private static void CheckViewOwnership()
    {
        var first = new Fallback(); var second = new Fallback();
        using var tester = new WidgetTester(platformMessages: first);
        var dispatcher = tester.View.platformDispatcher;
        var a = tester.View;
        using var b = dispatcher.RegisterView(2, new DorotiViewCapabilities("overlay-test")
            .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, new ViewHost())
            .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, second));
        first.Calls = 0;
        a.DispatchPlatformEvent(() => SystemChrome.setSystemUIOverlayStyle(SystemUiOverlayStyle.dark));
        b.DispatchPlatformEvent(() => SystemChrome.setSystemUIOverlayStyle(SystemUiOverlayStyle.dark));
        Require(first.Calls == 1 && second.Calls == 1, "Equal styles on separate views shared the cache or message owner.");
        a.DispatchPlatformEvent(() =>
        {
            SystemChrome.setSystemUIOverlayStyle(SystemUiOverlayStyle.light);
            b.DispatchPlatformEvent(() => SystemChrome.setSystemUIOverlayStyle(SystemUiOverlayStyle.light));
        });
        Require(first.Calls == 2 && second.Calls == 2, "Pending styles escaped their originating view during a nested event.");
        a.DispatchPlatformEvent(() => SystemChrome.handleAppLifecycleStateChanged(AppLifecycleState.detached));
        b.DispatchPlatformEvent(() => SystemChrome.setSystemUIOverlayStyle(SystemUiOverlayStyle.light));
        a.DispatchPlatformEvent(() => SystemChrome.setSystemUIOverlayStyle(SystemUiOverlayStyle.light));
        Require(first.Calls == 3 && second.Calls == 2, "Lifecycle invalidation affected another view's style cache.");
    }

    private sealed class Fallback : IPlatformMessageHostCapability
    {
        internal int Calls, Handlers;
        public ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data,
            CancellationToken cancellationToken = default)
        { Calls++; return ValueTask.FromResult<ReadOnlyMemory<byte>?>("[null]"u8.ToArray()); }
        public void SetMessageHandler(string channel, PlatformMessageHandler? handler) => Handlers++;
    }

    private sealed class ViewHost : IViewHostCapability
    {
        public ViewMetrics Metrics => new(new Size(320, 600), 1, ViewPadding.zero, ViewPadding.zero,
            ViewPadding.zero, AppLifecycleState.resumed, 0, 0);
        public DorotiViewEpoch ViewEpoch => default!;
        public event Action<ViewMetrics>? MetricsChanged { add { } remove { } }
        public event Action<AppLifecycleState>? LifecycleChanged { add { } remove { } }
        public event Action? CloseRequested { add { } remove { } }
        public event Action? Closed { add { } remove { } }
        public void Show() { }
        public void Resize(Size size) { }
        public void Close() { }
        public void Dispose() { }
    }
}
