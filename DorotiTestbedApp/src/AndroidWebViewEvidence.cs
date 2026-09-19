using System.Collections.Concurrent;
using System.Text.Json;
using Doroti.Framework.Services;
using Doroti.Ui;

// Opt-in product fixture. Uses the public controller and the same native instance
// as the displayed effect scene; never creates a separate SDK/compositor harness.
internal static class AndroidWebViewEvidence
{
    private static int _started;
    internal static void Start(DorotiView owner, WebViewController primary)
    {
        var path = Environment.GetEnvironmentVariable("DOROTI_ANDROID_WEBVIEW_EVIDENCE");
        if (string.IsNullOrEmpty(path) || Interlocked.Exchange(ref _started, 1) != 0) return;
        _ = Task.Run(() => Run(owner, primary, path));
    }
    private static async Task Run(DorotiView owner, WebViewController primary, string path)
    {
        var lines = new List<string> { "Android product WebViewController / native hierarchy", "physical=notVerified; nativeAot=notVerified" };
        void Check(bool value, string name) { if (!value) throw new InvalidOperationException(name); lines.Add("PASS " + name); }
        async Task Loaded(WebViewController controller, string? title = null)
        {
            for (var i = 0; i < 100; i++)
            {
                var state = await controller.ExecuteAsync(new(WebViewOperation.State));
                if (!state.IsLoading && state.DocumentGeneration > 0 && (title is null || state.Title == title)) return;
                await Task.Delay(100);
            }
            throw new TimeoutException("Document did not load: " + title + "; last state=" + await controller.ExecuteAsync(new(WebViewOperation.State)));
        }
        async Task Reject(Func<Task> action, WebViewError error)
        {
            try { await action(); }
            catch (WebViewException exception) when (exception.Code == error) { lines.Add("PASS rejected " + error); return; }
            throw new Exception("Expected " + error);
        }
        Task<WebViewController> Create(WebViewOptions options)
        {
            var completion = new TaskCompletionSource<WebViewController>(TaskCreationOptions.RunContinuationsAsynchronously);
            owner.DispatchPlatformEvent(() =>
            {
                try { completion.SetResult(new(owner, options)); }
                catch (Exception error) { completion.SetException(error); }
            });
            return completion.Task;
        }
        try
        {
            var handle = await primary.Ready;
            await Loaded(primary);
            var features = (await primary.ExecuteAsync(new(WebViewOperation.Features))).Features;
            Check(features is { Navigation: true, JavaScript: true, EphemeralProfile: true, SharedPersistentProfile: true, ClearAllData: true }, "feature query");
            Task<WebViewResult> Js(string script) => primary.ExecuteAsync(new(WebViewOperation.EvaluateJavaScript, script));
            Check((await Js("({text:'한글',items:[1,true,null]})")).Json == "{\"text\":\"한글\",\"items\":[1,true,null]}", "typed JSON and Korean text");
            Check((await Js("undefined")).IsUndefined, "undefined distinct from null");
            Check((await Js("null")).Json == "null", "null result");
            await Reject(() => Js("throw Error('probe')"), WebViewError.JavaScript);
            await Reject(() => Js("Promise.resolve(1)"), WebViewError.JavaScript);
            await Reject(() => Js("let x={};x.self=x;x"), WebViewError.JavaScript);
            await Reject(() => primary.ExecuteAsync(new(WebViewOperation.Navigate, "file:///C:/Windows/win.ini")), WebViewError.InvalidRequest);
            var prior = await primary.ExecuteAsync(new(WebViewOperation.State));
            var pending = Js("(()=>{const end=Date.now()+500;while(Date.now()<end){};return 'old'})()");
            await primary.ExecuteAsync(new(WebViewOperation.LoadHtml, "<!doctype html><title>replacement</title><input value='retained'>"));
            await Reject(() => pending, WebViewError.NavigationChanged);
            await Loaded(primary, "replacement");
            await Reject(() => primary.ExecuteAsync(new(WebViewOperation.EvaluateJavaScript, "1", prior.DocumentGeneration)), WebViewError.NavigationChanged);
            Check(await primary.Ready == handle, "navigation preserves attachment identity");
            using (var cancel = new CancellationTokenSource())
            {
                var canceled = primary.ExecuteAsync(new(WebViewOperation.EvaluateJavaScript, "(()=>{const end=Date.now()+300;while(Date.now()<end){};return 1})()"), cancel.Token);
                cancel.Cancel();
                try { await canceled; throw new Exception("Expected cancellation."); }
                catch (OperationCanceledException) { lines.Add("PASS caller cancellation"); }
            }
            await primary.ExecuteAsync(new(WebViewOperation.ClearData));
            lines.Add("PASS explicit profile clear completion");

            var routes = new Dictionary<string, WebViewResource>
            {
                ["/index.html"] = new("webview/index.html", "text/html"),
                ["/style.css"] = new("webview/style.css", "text/css")
            };
            var options = new WebViewOptions(AllowedOrigins: [], Resources: routes, MessageOrigins: ["doroti-app://content"]);
            await using var content = await Create(options);
            await content.Ready;
            var messages = new ConcurrentQueue<WebViewEvent>();
            content.Changed += value => { if (value.Kind == WebViewEventKind.Message) messages.Enqueue(value); };
            await content.LoadAppContentAsync("webview/index.html");
            await Loaded(content, "App content");
            Task<WebViewResult> ContentJs(string script) => content.ExecuteAsync(new(WebViewOperation.EvaluateJavaScript, script));
            Check((await ContentJs("getComputedStyle(document.querySelector('#content')).color")).Json == "\"rgb(12, 34, 56)\"", "manifest HTML and relative CSS");
            for (var i = 0; i < 30 && (await ContentJs("typeof doroti")).Json != "\"object\""; i++) await Task.Delay(50);
            await ContentJs("doroti.postMessage('probe',{value:'한글'})");
            for (var i = 0; i < 30 && messages.IsEmpty; i++) await Task.Delay(50);
            Check(messages.TryDequeue(out var message) && message.MessageName == "probe" && message.MessageJson?.Contains("한글") == true, "trusted native-origin main-frame bridge");
            await ContentJs("doroti.postMessage('oversized','x'.repeat(70000));undefined");
            await ContentJs("dorotiNative.postMessage(JSON.stringify({version:1,documentGeneration:0,requestId:1,name:'stale',payload:null}));undefined");
            var messageGeneration = (await content.ExecuteAsync(new(WebViewOperation.State))).DocumentGeneration;
            await ContentJs("(()=>{const f=document.createElement('iframe');f.id='message-frame';f.srcdoc=\"<script>dorotiNative.postMessage(JSON.stringify({version:1,documentGeneration:" + messageGeneration +
                ",requestId:1,name:'child-frame',payload:null}))<" + "/script>\";document.body.append(f)})()");
            await Task.Delay(300);
            Check(messages.IsEmpty, "oversized, stale and child-frame messages rejected");
            await ContentJs("document.querySelector('#message-frame').remove();undefined");
            await ContentJs("window.rangeResult=null;fetch('style.css',{headers:{Range:'bytes=0-3'}}).then(async r=>window.rangeResult=[r.status,(await r.text()).length]);undefined");
            string? range = null;
            for (var i = 0; i < 30; i++) { range = (await ContentJs("window.rangeResult")).Json; if (range != "null") break; await Task.Delay(50); }
            Check(range == "[206,4]", "app content Range bytes");
            await ContentJs("localStorage.setItem('doroti-probe','private');undefined");
            await using var isolated = await Create(options);
            await isolated.Ready;
            await isolated.ExecuteAsync(new(WebViewOperation.Navigate, "doroti-app://content/index.html"));
            await Loaded(isolated, "App content");
            Check((await isolated.ExecuteAsync(new(WebViewOperation.EvaluateJavaScript, "localStorage.getItem('doroti-probe')"))).Json == "null", "ephemeral profiles isolated");
            await content.ExecuteAsync(new(WebViewOperation.ClearData));
            await content.ExecuteAsync(new(WebViewOperation.Reload)); await Loaded(content, "App content");
            Check((await ContentJs("localStorage.getItem('doroti-probe')")).Json == "null", "clear removes local storage");
            await content.ExecuteAsync(new(WebViewOperation.LoadHtml, "<!doctype html><title>untrusted</title>")); await Loaded(content, "untrusted");
            var state = await content.ExecuteAsync(new(WebViewOperation.State));
            Check((await ContentJs("typeof dorotiNative")).Json == "\"undefined\"", "untrusted document has no native message object");
            await Task.Delay(150);
            Check(messages.IsEmpty, "untrusted about:blank origin rejected");
            await Reject(() => content.ExecuteAsync(new(WebViewOperation.Navigate, "https://example.com")), WebViewError.InvalidRequest);
            await content.DisposeAsync();
            await Reject(() => content.ExecuteAsync(new(WebViewOperation.State)), WebViewError.Closed);
            await using (var sharedA = await Create(options with { Profile = WebViewProfile.SharedPersistent }))
            await using (var sharedB = await Create(options with { Profile = WebViewProfile.SharedPersistent }))
            {
                await Task.WhenAll(sharedA.Ready, sharedB.Ready);
                await sharedA.LoadAppContentAsync("webview/index.html"); await Loaded(sharedA, "App content");
                await sharedA.ExecuteAsync(new(WebViewOperation.EvaluateJavaScript, "localStorage.setItem('shared-probe','shared');undefined"));
                await sharedB.LoadAppContentAsync("webview/index.html"); await Loaded(sharedB, "App content");
                Check((await sharedB.ExecuteAsync(new(WebViewOperation.EvaluateJavaScript, "localStorage.getItem('shared-probe')"))).Json == "\"shared\"", "explicit shared persistent profile");
                await sharedA.ExecuteAsync(new(WebViewOperation.ClearData));
            }
            for (var iteration = 0; iteration < 10; iteration++)
            {
                var late = await Create(new());
                await late.DisposeAsync();
            }
            lines.Add("PASS ten create/dispose races");
            var retiring = await Create(options);
            await retiring.Ready;
            await retiring.LoadAppContentAsync("webview/index.html"); await Loaded(retiring, "App content");
            var lateResult = retiring.ExecuteAsync(new(WebViewOperation.EvaluateJavaScript, "(()=>{const end=Date.now()+500;while(Date.now()<end){};return 'late'})()"));
            await Task.Delay(50);
            await retiring.DisposeAsync();
            await Reject(() => lateResult, WebViewError.Closed);
            // Restore the original live scene for the separate pixel/input gate.
            lines.Add("PASS Android WebView product commands");
        }
        catch (Exception error) { lines.Add("FAIL " + error); }
        finally { System.IO.File.WriteAllLines(path, lines); }
    }
}
