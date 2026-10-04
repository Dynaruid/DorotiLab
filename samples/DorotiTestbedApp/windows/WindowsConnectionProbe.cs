using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Doroti.Framework.Services;
using Doroti.Host.Maui;
using Doroti.Hosting;
using Doroti.Plugins;
using Doroti.Ui;
using Microsoft.Maui.ApplicationModel;
using Rect = Doroti.Ui.Rect;
using TextBox = Microsoft.UI.Xaml.Controls.TextBox;
using Path = System.IO.Path;

namespace DorotiTestbedApp.WinUI;

/// <summary>Opt-in real MAUI owner integration; synthetic controls/dialog commands, never physical IME evidence.</summary>
internal static class WindowsConnectionProbe
{
    internal static void Start(Microsoft.UI.Xaml.Window window)
    {
        if (Environment.GetEnvironmentVariable("DOROTI_MAUI_CONNECTION_PROBE") is not { Length: > 0 } path) return;
        _ = Task.Run(async () =>
        {
            try
            {
                DorotiMauiSurface? surface = null;
                await Wait(async () => await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    surface = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault()?.Page is Microsoft.Maui.Controls.ContentPage page
                        ? FindSurface(page.Content) : null;
                    return surface?.FrameworkView is not null && surface.WindowsSurface.NativeHost?.IsLoaded == true;
                }));
                var owner = surface!.FrameworkView!;
                await MainThread.InvokeOnMainThreadAsync(() => MauiSemanticsRegression.Run(new Microsoft.Maui.Controls.AbsoluteLayout()));
                var native = surface.WindowsSurface;
                if (Environment.GetEnvironmentVariable("DOROTI_MAUI_EMBEDDED_PROBE") == "1")
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        var page = (Microsoft.Maui.Controls.ContentPage)surface.Window.Page!;
                        page.Content = null;
                        native.OwnsWindowContent = false;
                        var grid = new Microsoft.Maui.Controls.Grid
                        {
                            RowDefinitions = [new(new Microsoft.Maui.GridLength(48)), new(Microsoft.Maui.GridLength.Star)],
                        };
                        grid.Add(new Microsoft.Maui.Controls.Label { Text = "MAUI parent content", HeightRequest = 48 }, 0, 0);
                        grid.Add(surface, 0, 1);
                        page.Content = grid;
                    });
                await Wait(async () => await MainThread.InvokeOnMainThreadAsync(() => native.NativeHost?.IsLoaded == true));
                var continuation = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                ValueTask scopedDispatch = default;
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    var before = SynchronizationContext.Current;
                    scopedDispatch = native.PlatformViews!.Dispatcher.InvokeAsync(async () => await continuation.Task);
                    Require(ReferenceEquals(before, SynchronizationContext.Current), "An asynchronous native operation leaked its owner synchronization context.");
                    continuation.SetResult();
                });
                await scopedDispatch;
                var host = owner.RequireCapability<IPlatformViewHostCapability>(DorotiCapabilityIds.PlatformViews, new("WindowsMauiProbe"));
                Require(!host.QuerySupport(new(0, "doroti/webview", PlatformViewComposition.InterleavedComposition)).Supported, "Interleaving was advertised.");
                Require(host.QuerySupport(new(0, "doroti/webview")).WebViewCommands, "WebView commands missing.");
                var created = new List<PlatformViewHandle>();
                foreach (var type in new[] { "doroti/native-button", "doroti/native-editor" })
                {
                    var handle = await host.CreateAsync(new PlatformViewDescriptor(type, Encoding.UTF8.GetBytes("Native MAUI test")));
                    created.Add(handle);
                    await host.AttachAsync(new(handle, Rect.fromLTWH(12, 12, 240, 80), PlatformViewTransform.Identity, Rect.fromLTWH(20, 20, 200, 60), 1));
                    await host.SetFocusAsync(handle, true);
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Require(native.PlatformViews!.Container.Children.Count == 1, "Native slot was not attached to the surface.");
                        var slot = (Microsoft.UI.Xaml.Controls.Border)native.PlatformViews.Container.Children[0];
                        Require(slot.Opacity == 1 && slot.Clip is not null && slot.Width == 240, "Native placement/clip missing.");
                        if (slot.Child is TextBox editor) { editor.Text = "한글 synthetic"; Require(editor.Text == "한글 synthetic", "Native editor text missing."); }
                    });
                    await host.DetachAsync(handle);
                    await host.AttachAsync(new(handle, Rect.fromLTWH(24, 24, 220, 80), PlatformViewTransform.Identity, null, 1));
                    await host.DisposeAsync(handle);
                    await host.GetDisposalCompletion(handle.InstanceId);
                }
                await using (var web = new WebViewController(owner, new(Html: "<title>MAUI native</title><input value='한글'>")))
                {
                    var handle = await web.Ready;
                    await host.AttachAsync(new(handle, Rect.fromLTWH(12, 12, 300, 160), PlatformViewTransform.Identity, null, 2));
                    await Wait(async () => !(await web.ExecuteAsync(new(WebViewOperation.State))).IsLoading);
                    var result = await web.ExecuteAsync(new(WebViewOperation.EvaluateJavaScript, "({value:document.querySelector('input').value,title:document.title})"));
                    Require(result.Json?.Contains("MAUI native") == true && result.Json.Contains("한글"), "Live WebView JavaScript result differs.");
                    var oldGeneration = result.DocumentGeneration;
                    await web.ExecuteAsync(new(WebViewOperation.LoadHtml, "<title>recreated document</title>"));
                    await Wait(async () => !(await web.ExecuteAsync(new(WebViewOperation.State))).IsLoading);
                    try { await web.ExecuteAsync(new(WebViewOperation.EvaluateJavaScript, "1", oldGeneration)); throw new Exception("Old document command succeeded."); }
                    catch (WebViewException error) when (error.Code == WebViewError.NavigationChanged) { }
                    await web.ExecuteAsync(new(WebViewOperation.ClearData));
                    await host.DetachAsync(handle);
                }
                await using (var recreated = new WebViewController(owner, new(Profile: WebViewProfile.SharedPersistent,
                    Resources: new Dictionary<string, WebViewResource>
                    {
                        ["/index.html"] = new("webview/index.html", "text/html; charset=utf-8"),
                        ["/style.css"] = new("webview/style.css", "text/css"),
                    })))
                {
                    var handle = await recreated.Ready;
                    await host.AttachAsync(new(handle, Rect.fromLTWH(12, 12, 300, 160), PlatformViewTransform.Identity, null, 2));
                    await Wait(async () => !(await recreated.ExecuteAsync(new(WebViewOperation.State))).IsLoading);
                    await recreated.LoadAppContentAsync("webview/index.html");
                    await Wait(async () => !(await recreated.ExecuteAsync(new(WebViewOperation.State))).IsLoading);
                    var appContent = await recreated.ExecuteAsync(new(WebViewOperation.EvaluateJavaScript, "document.title"));
                    Require(appContent.Json == "\"App content\"", "Manifest app content did not reach WebView2.");
                    await recreated.ExecuteAsync(new(WebViewOperation.ClearData));
                }
                var features = NativeFeatures.ForView(owner);
                Require((await features.GetCapabilitiesAsync()).FilePicker, "NativeFeatures cannot reach the registered picker.");
                var selectedPath = Path.Combine(Path.GetDirectoryName(path)!, "picked.txt");
                File.WriteAllText(selectedPath, "MAUI HWND file grant");
                var selectionTask = features.PickFilesAsync(new(Extensions: [".txt"])).AsTask();
                await SelectDialog(selectedPath);
                await using (var selection = await selectionTask.WaitAsync(TimeSpan.FromSeconds(30)))
                {
                    Require(selection.Status == FilePickStatus.selected && selection.Files.Count == 1, "Real picker selection failed.");
                    var bytes = new byte[128];
                    var count = await selection.Files[0].ReadAsync(0, bytes);
                    Require(Encoding.UTF8.GetString(bytes, 0, count) == "MAUI HWND file grant", "Read grant differs.");
                }
                using (var cancellation = new CancellationTokenSource())
                {
                    File.WriteAllText(path + ".stage", "caller-cancellation-open");
                    var picker = owner.RequireCapability<IFilePickerHostCapability>(DorotiCapabilityIds.FilePicker, new("WindowsMauiProbe"));
                    var pending = picker.PickFilesAsync(new(), cancellation.Token).AsTask();
                    await Wait(() => Task.FromResult(Dialog() != 0));
                    File.WriteAllText(path + ".stage", "concurrent-picker");
                    var concurrent = await picker.PickFilesAsync(new());
                    File.WriteAllText(path + ".stage", "caller-cancellation");
                    Require(concurrent.Status == FilePickStatus.failed, "Concurrent picker was admitted.");
                    cancellation.Cancel();
                    try { await pending.WaitAsync(TimeSpan.FromSeconds(30)); throw new Exception("Cancelled picker returned success."); }
                    catch (OperationCanceledException) { }
                }
                var closingPicker = owner.RequireCapability<IFilePickerHostCapability>(DorotiCapabilityIds.FilePicker, new("WindowsMauiProbe.close"));
                var closingRequest = closingPicker.PickFilesAsync(new()).AsTask();
                await Wait(() => Task.FromResult(Dialog() != 0));
                await MainThread.InvokeOnMainThreadAsync(() => ((IDisposable)closingPicker).Dispose());
                try { await closingRequest.WaitAsync(TimeSpan.FromSeconds(30)); throw new Exception("Owner disposal returned picker success."); }
                catch (OperationCanceledException) { }
                var actual = await MainThread.InvokeOnMainThreadAsync(() => new
                {
                    renderer = surface.Diagnostics!.Surface.GraphicsBackend,
                    fullWindowOwner = native.OwnsWindowContent,
                    nativeWindowOutput = native.UsesNativeWindowOutput,
                    nativeChildren = native.PlatformViews!.Container.Children.Count,
                    dpr = native.NativeHost!.XamlRoot.RasterizationScale,
                });
                Require(actual.nativeChildren == 0 && !actual.nativeWindowOutput, "Native instances leaked or full-window raster covered XAML.");
                File.WriteAllText(path, JsonSerializer.Serialize(new { status = "PASS", backend = "Windows-MAUI", actual,
                    nativeButton = true, nativeEditor = true, webCommands = true, staleDocumentRejected = true,
                    dispatcherScopeRestored = true,
                    webViewRecreated = true, appContent = true, profiles = "Ephemeral,SharedPersistent", ownerPickerCancellation = true,
                    filePicker = true, readGrant = true, concurrentPickerRejected = true, callerCancellation = true,
                    physicalIme = "notVerified", physicalTab = "notVerified", displayedPixels = "notVerified" }));
                // Close through the product desktop manager after the probe has retired all native instances.
                await MainThread.InvokeOnMainThreadAsync(() => window.Close());
            }
            catch (Exception error) { File.WriteAllText(path + ".error", error.ToString()); }
        });
    }

    private static DorotiMauiSurface? FindSurface(Microsoft.Maui.Controls.View? view) => view as DorotiMauiSurface
        ?? (view as Microsoft.Maui.Controls.Layout)?.Children.OfType<Microsoft.Maui.Controls.View>().Select(FindSurface).FirstOrDefault(s => s is not null);
    private static async Task Wait(Func<Task<bool>> condition)
    {
        var deadline = DateTime.UtcNow.AddSeconds(30);
        while (!await condition()) { if (DateTime.UtcNow > deadline) throw new TimeoutException("MAUI native probe condition did not complete."); await Task.Delay(50); }
    }
    private static void Require(bool value, string message) { if (!value) throw new Exception(message); }
    private static async Task SelectDialog(string path)
    {
        await Wait(() => Task.FromResult(Dialog() != 0));
        await Task.Delay(500);
        var dialog = Dialog();
        Require(SetDlgItemTextW(dialog, 1148, path), "Picker filename control missing.");
        Require(PostMessageW(dialog, 0x111, 1, 0), "Picker accept command failed.");
    }
    private static nint Dialog()
    {
        nint found = 0;
        EnumWindows((window, _) =>
        {
            GetWindowThreadProcessId(window, out var process);
            var name = new StringBuilder(128); GetClassNameW(window, name, name.Capacity);
            if (process == Environment.ProcessId && name.ToString() == "#32770" && IsWindowVisible(window)) { found = window; return false; }
            return true;
        }, 0);
        return found;
    }
    private delegate bool EnumProc(nint window, nint parameter);
    [DllImport("user32.dll")] private static extern bool EnumWindows(EnumProc callback, nint parameter);
    [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(nint window, out int process);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetClassNameW(nint window, StringBuilder text, int length);
    [DllImport("user32.dll")] private static extern bool IsWindowVisible(nint window);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern bool SetDlgItemTextW(nint window, int id, string text);
    [DllImport("user32.dll")] private static extern bool PostMessageW(nint window, uint message, nuint wparam, nint lparam);
}
