using System.Globalization;
using Doroti.Framework.Foundation;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Ui;
using WidgetLocalizations = Doroti.Framework.Widgets.Localizations;

namespace Doroti.Localization.Tests;

internal static class LocalizationScopeRegression
{
    private static readonly Locale En = new("en", "US");
    private static readonly Locale Ko = new("ko", "KR");
    private static readonly Locale Fr = new("fr", "FR");

    internal static void Run()
    {
        var preferences = Locale.ParseLanguageTags(["zh-hant", "SR_latn_rs", "ES-419", "iw-IL", "HE-il", "sv--SE", "en-US-u-hc-h23"]);
        Check(preferences.SequenceEqual([new("zh", scriptCode: "Hant"), new("sr", "RS", "Latn"), new("es", "419"), new("he", "IL")]),
            "tag parsing, aliases, unsupported suffixes, preference order and deduplication");
        Check(AppLibrary.basicLocaleListResolution([new("ZH", scriptCode: "hAnt")], [new("zh", "CN", "Hans"), new("zh", "TW", "Hant")])
            == new Locale("zh", "TW", "Hant"), "language/script matching");
        Check(AppLibrary.basicLocaleListResolution([new("de", "DE"), Ko], [En, Ko]) == Ko, "next preferred supported language");

        using var dispatcher = new PlatformDispatcher();
        using var scope = dispatcher.EnterScope();
        var firstHost = new Host(1, En);
        var secondHost = new Host(2, Ko);
        using var first = Register(dispatcher, firstHost);
        using var second = Register(dispatcher, secondHost);
        WidgetsFlutterBinding binding;
        using (first.EnterInvocationScope()) binding = new WidgetsFlutterBinding(dispatcher);
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;
        var previousError = FlutterError.onError;
        var errors = new List<FlutterErrorDetails>();
        FlutterError.onError = errors.Add;
        try
        {
            Locale? primary = null, secondary = null, subtree = null;
            var callbackOrder = new List<string>();
            void Mount(Widget widget) => first.DispatchPlatformEvent(() =>
            {
                binding.attachRootWidget(widget);
                Pump();
            });
            void Pump()
            {
                binding.buildOwner!.buildScope(binding.rootElement!);
                binding.buildOwner.finalizeTree();
            }
            void Tick() => first.DispatchPlatformEvent(Pump);
            void MountFirst(Widget child) => Mount(new ViewCollection(views:
                [new View(view: first, child: child), new View(view: second, child: SizedBox.CreateShrink())]));

            Mount(new ViewCollection(views:
            [
                new View(view: first, child: new WidgetsApp(color: new Color(0xff000000),
                    supportedLocales: [En, Ko, Fr],
                    localeListResolutionCallback: (_, _) => { callbackOrder.Add("list"); return null; },
                    localeResolutionCallback: (_, _) => { callbackOrder.Add("single"); return null; },
                    builder: (context, _) =>
                    {
                        primary = WidgetLocalizations.localeOf(context);
                        return WidgetLocalizations.CreateOverride(context: context, locale: Fr, child: new Builder(builder: inner =>
                        {
                            subtree = WidgetLocalizations.localeOf(inner);
                            Check(dispatcher.applicationLocale == primary, "subtree must not publish its locale to the application");
                            return SizedBox.CreateShrink();
                        }));
                    })),
                new View(view: second, child: new WidgetsApp(color: new Color(0xff000000),
                    supportedLocales: [En, Ko, Fr], builder: (context, _) =>
                    {
                        secondary = WidgetLocalizations.localeOf(context);
                        return SizedBox.CreateShrink();
                    })),
            ]));
            Check(callbackOrder.SequenceEqual(["list", "single"]), "locale callback priority");
            Check(primary == En && secondary == Ko && subtree == Fr && errors.Count == 0,
                $"Independent locales: primary={primary?.toLanguageTag()}, secondary={secondary?.toLanguageTag()}, "
                + $"subtree={subtree?.toLanguageTag()}. {string.Join(" | ", errors.Select(error => error.exception))}");
            using (first.EnterInvocationScope()) Check(dispatcher.applicationLocale == En, "first app locale");
            using (second.EnterInvocationScope()) Check(dispatcher.applicationLocale == Ko, "second app locale");
            secondHost.ChangeLocales(Fr, Ko);
            Tick();
            Check(primary == En && secondary == Fr && callbackOrder.Count == 2, "view language change reaches only its app resolver");

            Locale? committed = null;
            string? marker = null;
            Widget Load(Locale locale, List<object> delegates) => new WidgetLocalizations(
                locale: locale, delegates: [.. delegates, DefaultWidgetsLocalizations.@delegate], isApplicationLevel: true,
                child: new Builder(builder: context =>
                {
                    committed = WidgetLocalizations.localeOf(context);
                    marker = WidgetLocalizations.of<Payload>(context, typeof(Payload))?.Value;
                    return SizedBox.CreateShrink();
                }));
            binding.resetFirstFrameSent();
            var firstLoad = new TaskCompletionSource<Payload>();
            var winningLoad = new TaskCompletionSource<Payload>();
            MountFirst(Load(En, [new Delegate<Payload>(_ => Future<Payload>.fromTask(firstLoad.Task))]));
            Check(!binding.sendFramesToEngine && errors.Count == 0,
                "Initial asynchronous load must defer the first frame. " + string.Join(" | ", errors.Select(error => error.exception)));
            MountFirst(Load(Ko, [new Delegate<Payload>(_ => Future<Payload>.fromTask(winningLoad.Task)),
                new Delegate<Payload>(_ => new SynchronousFuture<Payload>(new("shadowed")))]));
            winningLoad.SetResult(new("latest"));
            Until(Tick, () => committed == Ko);
            Check(marker == "latest" && binding.sendFramesToEngine, "latest load wins and releases first frame");
            firstLoad.SetResult(new("stale"));

            var transitionFailure = new TaskCompletionSource<Payload>();
            MountFirst(Load(Fr, [new Delegate<Payload>(_ => Future<Payload>.fromTask(transitionFailure.Task))]));
            transitionFailure.SetException(new InvalidOperationException("transition failed"));
            Until(Tick, () => errors.Count == 1);
            Check(committed == Ko && marker == "latest" && binding.sendFramesToEngine,
                "failed transition retains last successful locale and resources");
            using (first.EnterInvocationScope()) Check(dispatcher.applicationLocale == Ko, "failed transition preserves published locale");

            MountFirst(SizedBox.CreateShrink());
            binding.resetFirstFrameSent();
            var earlier = new TaskCompletionSource<Payload>();
            var later = new TaskCompletionSource<SecondaryPayload>();
            MountFirst(Load(En, [new Delegate<Payload>(_ => Future<Payload>.fromTask(earlier.Task)),
                new Delegate<SecondaryPayload>(_ => Future<SecondaryPayload>.fromTask(later.Task))]));
            Check(!binding.sendFramesToEngine, "new scope holds the first frame");
            later.SetException(new InvalidOperationException("initial failure"));
            Until(Tick, () => errors.Any(error => error.exception is Exception { Message: "initial failure" }),
                () => string.Join(" | ", errors.Select(error => error.exception)));
            Check(binding.sendFramesToEngine && !earlier.Task.IsCompleted, "failure releases defer with an earlier pending delegate");

            MountFirst(SizedBox.CreateShrink());
            binding.resetFirstFrameSent();
            var disposed = new TaskCompletionSource<Payload>();
            MountFirst(Load(En, [new Delegate<Payload>(_ => Future<Payload>.fromTask(disposed.Task))]));
            Check(!binding.sendFramesToEngine, "last scope has a pending load");
            MountFirst(SizedBox.CreateShrink());
            Check(binding.sendFramesToEngine, "dispose releases first-frame defer");
            disposed.SetException(new InvalidOperationException("stale after dispose"));
            earlier.SetResult(new("ignored"));
            Tick();
            Check(errors.Count == 2,
                "Expected only the two delegate failures. " + string.Join(" | ", errors.Select(error => error.exception)));
            Check(CultureInfo.CurrentCulture == originalCulture && CultureInfo.CurrentUICulture == originalUiCulture,
                "widget presenters do not change global culture");
        }
        finally
        {
            FlutterError.onError = previousError;
            using (first.EnterInvocationScope()) binding.Dispose();
        }
        Console.WriteLine("PASS: locale parsing, resolver order, multiple views, subtree override, delegate priority and asynchronous load ownership");
    }

    private static void Until(Action tick, Func<bool> condition, Func<string>? diagnostics = null)
    {
        for (var i = 0; i < 24; i++)
        {
            tick();
            if (condition()) return;
            Thread.Sleep(10);
        }
        throw new InvalidOperationException("The expected localization completion did not arrive. " + diagnostics?.Invoke());
    }

    private static void Check(bool value, string message)
    {
        if (!value) throw new InvalidOperationException(message);
    }

    private static DorotiView Register(PlatformDispatcher dispatcher, Host host) => dispatcher.RegisterView(host.Id,
        new DorotiViewCapabilities("localization-contract")
            .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
            .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, host)
            .Register<ISceneHostCapability>(DorotiCapabilityIds.GraphicsScene, host)
            .Register<IPlatformEnvironmentHostCapability>(DorotiCapabilityIds.PlatformEnvironment, host));

    private sealed record Payload(string Value);
    private sealed class SecondaryPayload;

    private sealed class Delegate<T>(Func<Locale, Future<T>> loader) : LocalizationsDelegate<T>
    {
        public override bool isSupported(Locale locale) => true;
        public override Future<T> load(Locale locale) => loader(locale);
        public override bool shouldReload(LocalizationsDelegate<T> old) => !ReferenceEquals(this, old);
    }

    private sealed class Host(ulong id, Locale initial) : IViewHostCapability, IFrameHostCapability,
        ISceneHostCapability, IPlatformEnvironmentHostCapability
    {
        internal ulong Id => id;
        public PlatformConfiguration Configuration { get; private set; } = new([initial], Brightness.light, false, false,
            OperatingSystem.IsWindows() ? HostOperatingSystem.windows : HostOperatingSystem.linux);
        public ViewMetrics Metrics { get; } = new(new Size(320, 240), 1, new(), new(), new(), AppLifecycleState.resumed, 1, 1);
        public DorotiViewEpoch ViewEpoch { get; } = new(id, 1, 1, 320, 240, 320, 240, 1, 1, 0);
        public event Action<PlatformConfiguration>? ConfigurationChanged;
        public event Action<ViewMetrics>? MetricsChanged { add { } remove { } }
        public event Action<AppLifecycleState>? LifecycleChanged { add { } remove { } }
        public event Action? CloseRequested { add { } remove { } }
        public event Action? Closed { add { } remove { } }
        internal void ChangeLocales(params Locale[] locales)
        {
            Configuration = Configuration with { locales = locales };
            ConfigurationChanged?.Invoke(Configuration);
        }
        public void ScheduleFrame(Action<TimeSpan> callback) { }
        public void Submit(ulong viewId, DorotiSceneSubmission submission, DorotiUiInvocation invocation) { }
        public void Show() { }
        public void Resize(Size logicalSize) { }
        public void Close() { }
        public void Dispose() { }
    }
}
