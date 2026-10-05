using Doroti.Host.Web;
using Doroti.Hosting;
using Doroti.Ui;

namespace Doroti.Target.Web;

[System.Runtime.Versioning.SupportedOSPlatform("browser")]
public static class DorotiWebWorkerRunner
{
    private const ulong ViewId = 7301;
    private static BrowserWasmTarget? _target;
    private static DorotiApplicationBoundary? _boundary;
    private static DorotiHostSession? _session;
    private static DorotiSharedHostSession? _framework;
    private static DorotiView? _view;
    private static HttpClient? _http;
    private static BrowserTimeProvider? _timeProvider;

    public static async Task<string> RunAsync<TStartup>(
        System.Reflection.Assembly manifestAssembly,
        IEnumerable<DorotiApplicationPluginRegistration>? plugins = null,
        BrowserFontFallbackOptions? fontFallbackOptions = null,
        IEnumerable<IDorotiNativePluginHandler>? nativePlugins = null
    )
        where TStartup : IDorotiApplicationStartup, new()
    {
        if (_session is not null)
        {
            return "already-running";
        }

        BrowserOwnerSynchronizationContext.EnsureInstalled();
        await BrowserHostRuntime.EnsureInitializedAsync();
        _timeProvider = new BrowserTimeProvider();
        using var timeScope = Runtime.DorotiExecutionContext.EnterTimeProvider(
            _timeProvider
        );
        var baseAddress = new Uri(BrowserHostRuntime.ResolveResourceUrl("./"));
        var descriptor = DorotiApplicationFactory.Create<TStartup>(
            DorotiLaunchContext.Create("Web", "browser-wasm", [], baseAddress),
            plugins,
            manifestAssembly,
            nativePlugins
        );
        fontFallbackOptions ??= new();
        if (fontFallbackOptions.DecoderUrl is { IsAbsoluteUri: false })
            fontFallbackOptions = fontFallbackOptions with { DecoderUrl = new Uri(baseAddress, fontFallbackOptions.DecoderUrl) };
        _target = new BrowserWasmTarget(defaultFontFamily: fontFallbackOptions.DefaultFamily);
        _http = new HttpClient { Timeout = Timeout.InfiniteTimeSpan };
        try
        {
            using (
                var fontTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(100), _timeProvider)
            )
            {
                await BrowserStartupFonts.LoadBrowserAsync(fontFallbackOptions, _http, baseAddress,
                    (bytes, family) => _target.RegisterFont(bytes, family), fontTimeout.Token,
                    (bytes, family, face) => _target.RegisterCssFont(bytes, family, face));
            }
        }
        catch
        {
            Dispose();
            throw;
        }

        if (descriptor.ViewConfiguration.Navigation is { } navigation)
            await BrowserFrameworkHost.PrepareNavigationAsync(navigation.RestorationId);
        _framework = new DorotiSharedHostSession(descriptor);
        _session = _framework.Session;
        using var dispatcherScope = _session.dispatcher.EnterScope();
        _framework.Start();
        var browserPlugins = descriptor.PluginRegistrations.Select(
            item => new BrowserJavaScriptPluginDescriptor(
                item.Id,
                item.Channel,
                item.Adapter,
                item.Module,
                item.ExportName
            )
        );
        _boundary = _target.LoadApplicationBoundary(
            descriptor.ManifestAssembly,
            descriptor.ApplicationAssembly,
            browserPlugins,
            descriptor.NativePluginHandlers
        );
        _view = _target.CreateView(
            _session,
            ViewId,
            "doroti-surface",
            descriptor.ViewConfiguration,
            _boundary, _framework
        );
        _target.EnableFontFallbacks(_http, fontFallbackOptions, _timeProvider);
        BrowserHostRuntime.SetApplicationTitle(1, descriptor.ViewConfiguration.title);
        DorotiWebWorkerSurface.Initialize(_target, ViewId);
        _view.Show();
        _session.dispatcher.setSemanticsTreeEnabled(true);
        return "started";
    }

    public static void Dispose()
    {
        if (_session is null && _timeProvider is null)
        {
            return;
        }

        using var dispatcherScope = _session?.dispatcher.EnterScope();
        try
        {
            _session?.ShutdownFramework();
            DorotiWebWorkerSurface.Dispose();
            _target?.Dispose();
            _framework?.Dispose();
            _boundary?.Dispose();
        }
        finally
        {
            _http?.Dispose();
            _timeProvider?.Dispose();
            _view = null;
            _boundary = null;
            _target = null;
            _session = null;
            _framework = null;
            _http = null;
            _timeProvider = null;
        }
    }
}
