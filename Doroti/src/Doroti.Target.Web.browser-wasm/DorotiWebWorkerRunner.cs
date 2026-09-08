using Doroti.Host.Web;
using Doroti.Hosting;
using Doroti.Ui;

namespace Doroti.Target.Web;

[System.Runtime.Versioning.SupportedOSPlatform("browser")]
public static class DorotiWebWorkerRunner
{
    private const ulong ViewId = 7301;
    private const string FallbackFontUrl = "_content/Doroti.Host.Web/fonts/NanumGothic-Regular.ttf";
    private static BrowserWasmTarget? _target;
    private static DorotiApplicationBoundary? _boundary;
    private static DorotiHostSession? _session;
    private static DorotiView? _view;
    private static HttpClient? _http;

    public static async Task<string> RunAsync<TStartup>(
        System.Reflection.Assembly manifestAssembly,
        IEnumerable<DorotiApplicationPluginRegistration>? plugins = null)
        where TStartup : IDorotiApplicationStartup, new()
    {
        if (_session is not null) return "already-running";
        await BrowserHostRuntime.EnsureInitializedAsync();
        var baseAddress = new Uri(BrowserHostRuntime.ResolveResourceUrl("./"));
        var descriptor = DorotiApplicationFactory.Create<TStartup>(
            DorotiLaunchContext.Create("Web", "browser-wasm", [], baseAddress),
            plugins, manifestAssembly);
        _target = new BrowserWasmTarget();
        _http = new HttpClient();
        var fontUrl = BrowserHostRuntime.ResolveResourceUrl(FallbackFontUrl);
        _target.RegisterFont(await _http.GetByteArrayAsync(fontUrl));
        _session = new DorotiHostSession(descriptor.EntrypointFactory());
        using var dispatcherScope = _session.dispatcher.EnterScope();
        _session.Start(deferFrameworkBootstrap: true);
        var browserPlugins = descriptor.PluginRegistrations.Select(item =>
            new BrowserJavaScriptPluginDescriptor(
                item.Id, item.Channel, item.Adapter, item.Module, item.ExportName));
        _boundary = _target.LoadApplicationBoundary(
            descriptor.ManifestAssembly, descriptor.ApplicationAssembly, browserPlugins);
        _view = _target.CreateView(
            _session, ViewId, "doroti-surface", descriptor.ViewConfiguration, _boundary);
        BrowserHostRuntime.SetApplicationTitle(1, descriptor.ViewConfiguration.title);
        DorotiWebWorkerSurface.Initialize(_target, ViewId);
        _view.Show();
        _session.dispatcher.setSemanticsTreeEnabled(true);
        return "started";
    }

    public static void Dispose()
    {
        if (_session is null) return;
        using var dispatcherScope = _session.dispatcher.EnterScope();
        DorotiWebWorkerSurface.Dispose();
        _view?.Dispose();
        _boundary?.Dispose();
        _target?.Dispose();
        _session.Dispose();
        _http?.Dispose();
        _view = null;
        _boundary = null;
        _target = null;
        _session = null;
        _http = null;
    }
}
