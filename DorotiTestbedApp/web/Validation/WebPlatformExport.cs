using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Text.Json;
using Doroti.Ui;
using Doroti.Framework.Services;

namespace DorotiTestbedApp.Web.Validation;

/// <summary>Validation uses the live product owner/controller on its dispatch context.</summary>
[SupportedOSPlatform("browser")]
public static partial class WebPlatformExport
{
    private static Task<string> OnOwner(Func<Task<string>> action)
    {
        var completion = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        var owner = PlatformEffectFixtureProbe.Owner ?? throw new InvalidOperationException("Platform fixture is not mounted.");
        owner.DispatchPlatformEvent(async () => { try { completion.SetResult(await action()); } catch (Exception error) { completion.SetException(error); } });
        return completion.Task;
    }
    [JSExport]
    public static Task<string> Stage(int stage) => OnOwner(() => { PlatformEffectFixtureProbe.SetStage!(stage); return Task.FromResult("queued"); });
    [JSExport]
    public static Task<string> Style(double strength, double saturation, int tint, bool raster) => OnOwner(() =>
    {
        PlatformEffectFixtureProbe.SetStyle!(new(strength, unchecked((uint)tint), saturation));
        PlatformEffectFixtureProbe.SetRasterSource!(raster);
        return Task.FromResult("queued");
    });
    [JSExport]
    public static Task<string> Command(int operation, string text, int documentGeneration) => OnOwner(async () =>
    {
        var web = PlatformEffectFixtureProbe.WebView ?? throw new InvalidOperationException("WebView is not mounted.");
        await web.Ready;
        try { return JsonSerializer.Serialize(await web.ExecuteAsync(new((WebViewOperation)operation, text, documentGeneration))); }
        catch (WebViewException error) { return JsonSerializer.Serialize(new { error = error.Code.ToString(), message = error.Message }); }
    });
    [JSExport]
    public static Task<string> Policy(string origin) => OnOwner(async () =>
    {
        var owner = PlatformEffectFixtureProbe.Owner!;
        var passed = new List<string>();
        await using (var rejected = new WebViewController(owner, new(Html: "private")))
        {
            try { await rejected.Ready; throw new Exception("Private iframe profile was accepted."); }
            catch (WebViewException error) when (error.Code == WebViewError.Unsupported) { passed.Add("private profile rejected"); }
        }
        await using var web = new WebViewController(owner, new(Html: """
            <!doctype html><title>policy</title><script>
            addEventListener('message', event => {
              if(event.source!==parent || event.data.type!=='doroti-webview-init')return;
              window.bridge=event.data;
              window.send=(patch={})=>parent.postMessage({...bridge,type:'doroti-webview-message',requestId:1,name:'accepted',json:'{"ok":true}',...patch},event.origin);
              send();
            });
            </script>
            """, Profile: WebViewProfile.BrowserDefault, AllowedOrigins: [origin], MessageOrigins: [origin]));
        var messages = 0;
        web.Changed += e => { if (e.Kind == WebViewEventKind.Message) messages++; };
        await web.Ready;
        for (var i = 0; i < 30 && messages == 0; i++) await Task.Delay(50);
        if (messages != 1) throw new Exception($"Expected one trusted message, got {messages}.");
        passed.Add("trusted message with native source and origin");
        await web.ExecuteAsync(new(WebViewOperation.EvaluateJavaScript,
            "send();send({requestId:2,nonce:'wrong'});send({requestId:3,documentGeneration:0});send({requestId:4,json:'x'.repeat(65537)});"));
        await Task.Delay(100);
        if (messages != 1) throw new Exception("Invalid or repeated message accepted.");
        passed.Add("replayed wrong nonce stale and oversized messages rejected");
        try { await web.ExecuteAsync(new(WebViewOperation.Navigate, "https://example.invalid/")); throw new Exception("Origin policy ignored."); }
        catch (WebViewException error) when (error.Code == WebViewError.InvalidRequest) { passed.Add("command navigation allowlist"); }
        using var cancel = new CancellationTokenSource(50);
        try { await web.ExecuteAsync(new(WebViewOperation.EvaluateJavaScript, "new Promise(r=>setTimeout(()=>r(42),1000))"), cancel.Token); throw new Exception("Cancellation ignored."); }
        catch (OperationCanceledException) { passed.Add("caller cancellation"); }
        var pending = web.ExecuteAsync(new(WebViewOperation.EvaluateJavaScript, "new Promise(()=>{})"));
        await web.DisposeAsync();
        try { await pending; throw new Exception("Close did not cancel JavaScript."); }
        catch (WebViewException error) when (error.Code == WebViewError.Closed) { passed.Add("close cancels pending JavaScript"); }
        return JsonSerializer.Serialize(passed);
    });
}
