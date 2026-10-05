using Doroti.Ui;
using Doroti.Hosting;
using Microsoft.Web.WebView2.Core;
using WebView2 = Microsoft.UI.Xaml.Controls.WebView2;
namespace Doroti.Host.WindowsAppSdk;

/// <summary>Opt-in XAML WebView island on the same sibling composition path as native editors.</summary>
internal sealed class WindowsXamlWebViewFactory(WindowsHwndPlatformViewFactory attachment,
    Func<PlatformViewHandle, WebView2> control, IApplicationResourceHostCapability resources) : IPlatformViewFactory
{
    public string ViewType => "doroti/webview";
    public PlatformViewSupport QuerySupport(PlatformViewRequest request) => attachment.QuerySupport(request) with
    { WebViewCommands = true, MixedScene = true,
      Capabilities = new(PlatformViewRepresentation.NativeHierarchy, PlatformViewTransport.Native,
          PlatformViewInputPolicy.DirectNative, PlatformEffectSupport.Unsupported), NativeBackdropBlur = false };
    public async ValueTask<IPlatformViewInstance> CreateAsync(PlatformViewHandle handle, ReadOnlyMemory<byte> parameters,
        Action<PlatformViewHandle> onFocused, CancellationToken cancellationToken)
    {
        var options = WindowsWebViewSession.ParseOptions(parameters);
        if (!parameters.IsEmpty && System.Text.Encoding.UTF8.GetString(parameters.Span).StartsWith('{'))
        {
            using var legacy = System.Text.Json.JsonDocument.Parse(parameters);
            if (legacy.RootElement.TryGetProperty("html", out var html)) options = new(html.GetString());
        }
        var content = await WindowsWebViewSession.LoadContent(resources, options, cancellationToken);
        var folder = Environment.GetEnvironmentVariable("DOROTI_WEBVIEW_USER_DATA") ?? System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Doroti", "WebView2");
        var environment = await WindowsWebViewSession.CreateEnvironment(folder);
        cancellationToken.ThrowIfCancellationRequested();
        var native = await attachment.CreateAsync(handle, ReadOnlyMemory<byte>.Empty, onFocused, cancellationToken);
        var web = control(handle);
        try
        {
            var profile = environment.CreateCoreWebView2ControllerOptions();
            profile.ProfileName = options.Profile == WebViewProfile.Ephemeral ? "private-" + Guid.NewGuid().ToString("N") : "DorotiShared";
            profile.IsInPrivateModeEnabled = options.Profile == WebViewProfile.Ephemeral;
            await web.EnsureCoreWebView2Async(environment, profile);
            cancellationToken.ThrowIfCancellationRequested();
            return new Instance(native, web, new Session(handle, environment, web.CoreWebView2, options, content));
        }
        catch { web.Close(); await native.DisposeAsync(); throw; }
    }
    private sealed class Instance(IPlatformViewInstance native, WebView2 web, Session session) : IPlatformViewInstance, IPlatformWebViewInstance
    {
        public event Action<WebViewEvent>? WebViewChanged { add => session.WebViewChanged += value; remove => session.WebViewChanged -= value; }
        public Task<WebViewResult> ExecuteAsync(WebViewCommand command, CancellationToken token) => session.ExecuteAsync(command, token);
        public async ValueTask ApplyAsync(PlatformViewPlacement placement) { await native.ApplyAsync(placement); if (placement.Visible) session.LoadInitial(); }
        public ValueTask DetachAsync() => native.DetachAsync();
        public ValueTask SetFocusAsync(bool focused) => native.SetFocusAsync(focused);
        public async ValueTask DisableInputAsync() { session.Close(); await native.DisableInputAsync(); }
        public async ValueTask DisposeAsync() { session.Close(); web.Close(); await native.DisposeAsync(); }
    }
    private sealed class Session : WindowsWebViewSession
    {
        internal Session(PlatformViewHandle handle, CoreWebView2Environment environment, CoreWebView2 core,
            WebViewOptions options, Dictionary<string, byte[]> content) : base(handle, environment, core, options, content)
        {
            Core.Settings.IsWebMessageEnabled = true; Core.Settings.IsScriptEnabled = true;
            Core.NavigationCompleted += Completed; Core.WebMessageReceived += Message;
            InitialHtml = options.Html ?? "<!doctype html><input value='Native WebView2'>";
        }
        private bool _closed;
        internal void LoadInitial() { if (InitialHtml is not { } html) return; InitialHtml = null; NavigateHtml(html); }
        private void Completed(object? sender, CoreWebView2NavigationCompletedEventArgs args) => CompleteNavigation(args);
        private void Message(object? sender, CoreWebView2WebMessageReceivedEventArgs args) => ReceiveMessage(args);
        internal void Close() { if (_closed) return; _closed = true; CloseCommands(); Core.NavigationCompleted -= Completed; Core.WebMessageReceived -= Message; }
    }
}
