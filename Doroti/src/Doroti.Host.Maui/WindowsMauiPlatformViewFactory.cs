#if WINDOWS
using System.Text;
using System.Text.Json;
using Doroti.Host.SharedWindows;
using Doroti.Hosting;
using Doroti.Ui;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Input;
using Microsoft.Web.WebView2.Core;
using Border = Microsoft.UI.Xaml.Controls.Border;
using Canvas = Microsoft.UI.Xaml.Controls.Canvas;
using TextBox = Microsoft.UI.Xaml.Controls.TextBox;
using Path = System.IO.Path;
using Visibility = Microsoft.UI.Xaml.Visibility;

namespace Doroti.Host.Maui;

internal sealed class WindowsMauiPlatformViewFactory(
    WindowsMauiPlatformViewHost owner, string viewType, Func<IApplicationResourceHostCapability> resources)
    : IPlatformViewFactory
{
    public string ViewType => viewType;

    public PlatformViewSupport QuerySupport(PlatformViewRequest request)
    {
        var supported = owner.Enabled && WindowsCompositionSurfaceFeature.Enabled && request.ViewType == ViewType
            && request.Composition == PlatformViewComposition.NativeOverlay
            && (request.Effects & ~PlatformViewEffects.RectClip) == 0;
        return new("Windows-MAUI-WinUI-overlay", Environment.OSVersion.VersionString, ViewType,
            supported, PlatformViewComposition.NativeOverlay, PlatformViewEffects.RectClip,
            Capabilities: new(PlatformViewRepresentation.NativeHierarchy, PlatformViewTransport.Native,
                PlatformViewInputPolicy.DirectNative, PlatformEffectSupport.Unsupported),
            WebViewCommands: ViewType == "doroti/webview",
            MixedScene: supported,
            Reason: supported ? null : "Windows MAUI requires its WinUI Composition surface for disjoint native overlays with translation/rectangular clip. Interleaving, native backdrop, affine transforms, overlapping native regions and the legacy child-HWND raster path are unsupported.");
    }

    public async ValueTask<IPlatformViewInstance> CreateAsync(PlatformViewHandle handle,
        ReadOnlyMemory<byte> parameters, Action<PlatformViewHandle> onFocused, CancellationToken cancellationToken)
    {
        owner.Dispatcher.VerifyThread();
        cancellationToken.ThrowIfCancellationRequested();
        var container = owner.Container;
        if (ViewType != "doroti/webview")
        {
            var text = parameters.IsEmpty ? "Native control" : Encoding.UTF8.GetString(parameters.Span);
            if (text.StartsWith('{'))
            {
                using var json = JsonDocument.Parse(text);
                text = json.RootElement.TryGetProperty("text", out var value) ? value.GetString() ?? "" : text;
            }
            Control control = ViewType == "doroti/native-editor"
                ? new TextBox { Text = text, AcceptsReturn = true, TextWrapping = TextWrapping.Wrap }
                : new Microsoft.UI.Xaml.Controls.Button { Content = text };
            return new Instance(owner, container, control, handle, onFocused);
        }

        var options = WindowsWebViewSession.ParseOptions(parameters);
        // The existing input lifetime sample also uses the older {"html": ...} descriptor.
        if (!parameters.IsEmpty && Encoding.UTF8.GetString(parameters.Span).StartsWith('{'))
        {
            using var legacy = JsonDocument.Parse(parameters);
            if (legacy.RootElement.TryGetProperty("html", out var html))
            {
                options = new WebViewOptions(Html: html.GetString());
                options.Validate();
            }
        }
        var content = await WindowsWebViewSession.LoadContent(resources(), options, cancellationToken);
        var folder = Environment.GetEnvironmentVariable("DOROTI_WEBVIEW_USER_DATA")
            ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Doroti", "WebView2");
        var environment = await WindowsWebViewSession.CreateEnvironment(folder);
        cancellationToken.ThrowIfCancellationRequested();
        _ = owner.Container; // Reject detach/close during environment creation.
        var profile = environment.CreateCoreWebView2ControllerOptions();
        profile.ProfileName = options.Profile == WebViewProfile.Ephemeral ? "private-" + Guid.NewGuid().ToString("N") : "DorotiShared";
        profile.IsInPrivateModeEnabled = options.Profile == WebViewProfile.Ephemeral;
        var web = new Microsoft.UI.Xaml.Controls.WebView2 { Width = 1, Height = 1 };
        var instance = new WebInstance(owner, container, web, handle, onFocused);
        try
        {
            // WinUI owns the CompositionController and its Microsoft.UI.Composition target.
            // Attach invisibly before Ensure; close the control on every failed/cancelled creation.
            await web.EnsureCoreWebView2Async(environment, profile);
            cancellationToken.ThrowIfCancellationRequested();
            _ = owner.Container;
            instance.Connect(environment, options, content);
            return instance;
        }
        catch { await instance.DisposeAsync(); throw; }
    }

    private class Instance : IPlatformViewInstance
    {
        protected readonly WindowsMauiPlatformViewHost Owner;
        protected readonly FrameworkElement Control;
        protected readonly PlatformViewHandle Handle;
        private readonly Canvas _container;
        private readonly Border _slot;
        private readonly Action<PlatformViewHandle> _onFocused;
        private bool _disabled;
        private bool _disposed;

        internal Instance(WindowsMauiPlatformViewHost owner, Canvas container, FrameworkElement control,
            PlatformViewHandle handle, Action<PlatformViewHandle> onFocused)
        {
            Owner = owner;
            Control = control;
            Handle = handle;
            _onFocused = onFocused;
            _container = container;
            _slot = new Border { Child = control, Opacity = 0, IsHitTestVisible = false };
            if (control is Control nativeControl) nativeControl.IsEnabled = false;
            control.GotFocus += Focused;
            container.Children.Add(_slot);
        }

        private void Focused(object sender, RoutedEventArgs args)
        {
            if (_disabled || _disposed || !_slot.IsHitTestVisible) return;
            Owner.YieldTextInput();
            _onFocused(Handle);
        }

        public virtual ValueTask ApplyAsync(PlatformViewPlacement placement)
        {
            Owner.Dispatcher.VerifyThread();
            ObjectDisposedException.ThrowIf(_disposed, this);
            placement.Validate();
            if (placement.Handle != Handle || placement.Transform is not { M11: 1, M12: 0, M21: 0, M22: 1 })
                throw new NotSupportedException("MAUI native overlays require owner identity and translation-only placement.");
            var origin = placement.Transform.Map(placement.Bounds.topLeft);
            _slot.Width = Control.Width = placement.Bounds.width;
            _slot.Height = Control.Height = placement.Bounds.height;
            Canvas.SetLeft(_slot, origin.dx);
            Canvas.SetTop(_slot, origin.dy);
            Canvas.SetZIndex(_slot, placement.PaintOrder);
            var bounds = placement.Bounds.shift(new(placement.Transform.Dx, placement.Transform.Dy));
            var clip = placement.Clip is { } requested ? bounds.intersect(requested) : bounds;
            _slot.Clip = new RectangleGeometry { Rect = new(Math.Max(0, clip.left - origin.dx), Math.Max(0, clip.top - origin.dy),
                Math.Max(0, clip.width), Math.Max(0, clip.height)) };
            var visible = placement.Visible && !bounds.isEmpty && !clip.isEmpty;
            _slot.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            _slot.Opacity = visible ? 1 : 0;
            _slot.IsHitTestVisible = visible && !_disabled;
            if (Control is Control nativeControl) nativeControl.IsEnabled = visible && !_disabled;
            return ValueTask.CompletedTask;
        }

        public ValueTask DetachAsync()
        {
            Owner.Dispatcher.VerifyThread();
            _slot.Opacity = 0;
            _slot.IsHitTestVisible = false;
            var result = SetFocusAsync(false);
            _slot.Visibility = Visibility.Collapsed;
            return result;
        }

        public ValueTask SetFocusAsync(bool focused)
        {
            Owner.Dispatcher.VerifyThread();
            if (focused && !_disabled && !_disposed && _slot.IsHitTestVisible)
            {
                Owner.YieldTextInput();
                Control.Focus(FocusState.Programmatic);
            }
            else if (OwnsFocus()) Owner.RestoreFocus();
            return ValueTask.CompletedTask;
        }

        private bool OwnsFocus()
        {
            if (Control.XamlRoot is null) return false;
            var focused = FocusManager.GetFocusedElement(Control.XamlRoot) as DependencyObject;
            while (focused is not null)
            {
                if (focused == Control) return true;
                focused = VisualTreeHelper.GetParent(focused);
            }
            return false;
        }

        public virtual ValueTask DisableInputAsync()
        {
            Owner.Dispatcher.VerifyThread();
            _disabled = true;
            if (Control is Control nativeControl) nativeControl.IsEnabled = false;
            return DetachAsync();
        }

        public virtual ValueTask DisposeAsync()
        {
            Owner.Dispatcher.VerifyThread();
            if (_disposed) return ValueTask.CompletedTask;
            _disposed = true;
            Control.GotFocus -= Focused;
            _container.Children.Remove(_slot);
            _slot.Child = null;
            return ValueTask.CompletedTask;
        }
    }

    private sealed class WebInstance(WindowsMauiPlatformViewHost owner, Canvas container,
        Microsoft.UI.Xaml.Controls.WebView2 web, PlatformViewHandle handle, Action<PlatformViewHandle> focused)
        : Instance(owner, container, web, handle, focused), IPlatformWebViewInstance
    {
        private XamlSession? _session;
        private bool _closed;
        internal void Connect(CoreWebView2Environment environment, WebViewOptions options, Dictionary<string, byte[]> content) =>
            _session = new(Handle, environment, web.CoreWebView2, options, content);
        public event Action<WebViewEvent>? WebViewChanged
        {
            add => _session!.WebViewChanged += value;
            remove { if (_session is { } session) session.WebViewChanged -= value; }
        }
        public Task<WebViewResult> ExecuteAsync(WebViewCommand command, CancellationToken token) =>
            _session!.ExecuteAsync(command, token);
        public override ValueTask ApplyAsync(PlatformViewPlacement placement)
        {
            var result = base.ApplyAsync(placement);
            if (placement.Visible) _session?.LoadInitial();
            return result;
        }
        public override ValueTask DisableInputAsync()
        {
            _session?.Close();
            return base.DisableInputAsync();
        }
        public override ValueTask DisposeAsync()
        {
            if (!_closed)
            {
                _closed = true;
                _session?.Close();
                web.Close();
            }
            return base.DisposeAsync();
        }
    }

    private sealed class XamlSession : WindowsWebViewSession
    {
        internal XamlSession(PlatformViewHandle handle, CoreWebView2Environment environment,
            CoreWebView2 core, WebViewOptions options, Dictionary<string, byte[]> content)
            : base(handle, environment, core, options, content)
        {
            Core.Settings.IsWebMessageEnabled = true;
            Core.Settings.IsScriptEnabled = true;
            Core.NavigationCompleted += Completed;
            Core.WebMessageReceived += Message;
            InitialHtml = options.Html ?? "<!doctype html><input value='Native WebView2'>";
        }
        internal void LoadInitial()
        {
            if (InitialHtml is not { } html) return;
            InitialHtml = null;
            NavigateHtml(html);
        }
        private void Completed(object? sender, CoreWebView2NavigationCompletedEventArgs args) => CompleteNavigation(args);
        private void Message(object? sender, CoreWebView2WebMessageReceivedEventArgs args) => ReceiveMessage(args);
        internal void Close()
        {
            CloseCommands();
            Core.NavigationCompleted -= Completed;
            Core.WebMessageReceived -= Message;
        }
    }
}
#endif
