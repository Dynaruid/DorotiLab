using System.Collections.Concurrent;
using System.Text.Json;
using Doroti.Framework.Services;
using Doroti.Ui;

// Opt-in product fixture. Uses the public controller and the same native instance
// as the displayed effect scene; never creates a separate SDK/compositor harness.
internal static class QtWebViewEvidence
{
    private static int _started;

    internal static void Start(DorotiView owner, WebViewController primary)
    {
        var path = Environment.GetEnvironmentVariable("DOROTI_QT_WEBVIEW_EVIDENCE");
        if (string.IsNullOrEmpty(path) || Interlocked.Exchange(ref _started, 1) != 0)
        {
            return;
        }

        _ = Task.Run(() => Run(owner, primary, path));
    }

    private static async Task Run(DorotiView owner, WebViewController primary, string path)
    {
        var lines = new List<string>
        {
            "Linux Qt product WebViewController / Quick scene",
            "physical=notVerified; nativeAot=notVerified",
        };
        void Check(bool value, string name)
        {
            if (!value)
            {
                throw new InvalidOperationException(name);
            }
            lines.Add("PASS " + name);
        }
        async Task Loaded(WebViewController controller, string? title = null)
        {
            for (var i = 0; i < 100; i++)
            {
                var state = await controller.ExecuteAsync(new(WebViewOperation.State));
                if (
                    !state.IsLoading
                    && state.DocumentGeneration > 0
                    && (title is null || state.Title == title)
                )
                {
                    return;
                }

                await Task.Delay(100);
            }
            throw new TimeoutException(
                "Document did not load: "
                    + title
                    + "; last state="
                    + await controller.ExecuteAsync(new(WebViewOperation.State))
            );
        }
        async Task Reject(Func<Task> action, WebViewError error)
        {
            try
            {
                await action();
            }
            catch (WebViewException exception) when (exception.Code == error)
            {
                lines.Add("PASS rejected " + error);
                return;
            }
            throw new Exception("Expected " + error);
        }
        Task<WebViewController> Create(WebViewOptions options)
        {
            var completion = new TaskCompletionSource<WebViewController>(
                TaskCreationOptions.RunContinuationsAsynchronously
            );
            owner.DispatchPlatformEvent(() =>
            {
                try
                {
                    completion.SetResult(new(owner, options));
                }
                catch (Exception error)
                {
                    completion.SetException(error);
                }
            });
            return completion.Task;
        }
        try
        {
            var handle = await primary.Ready;
            await Loaded(primary);
            var features = (await primary.ExecuteAsync(new(WebViewOperation.Features))).Features;
            Check(
                features
                    is {
                        Navigation: true,
                        JavaScript: true,
                        EphemeralProfile: true,
                        SharedPersistentProfile: true,
                        ClearAllData: false
                    },
                "feature query"
            );
            Task<WebViewResult> Js(string script) =>
                primary.ExecuteAsync(new(WebViewOperation.EvaluateJavaScript, script));
            Check(
                (await Js("({text:'한글',items:[1,true,null]})")).Json
                    == "{\"text\":\"한글\",\"items\":[1,true,null]}",
                "typed JSON and Korean text"
            );
            Check(
                (await Js("/*" + new string('한', 600_000) + "*/7")).Json == "7",
                "UTF-8 command limit survives JSON escaping"
            );
            Check((await Js("undefined")).IsUndefined, "undefined distinct from null");
            Check((await Js("null")).Json == "null", "null result");
            await Reject(() => Js("throw Error('probe')"), WebViewError.JavaScript);
            await Reject(() => Js("Promise.resolve(1)"), WebViewError.JavaScript);
            await Reject(() => Js("let x={};x.self=x;x"), WebViewError.JavaScript);
            await Reject(
                () =>
                    primary.ExecuteAsync(
                        new(WebViewOperation.Navigate, "file:///C:/Windows/win.ini")
                    ),
                WebViewError.InvalidRequest
            );
            var prior = await primary.ExecuteAsync(new(WebViewOperation.State));
            var pending = Js(
                "(()=>{const end=Date.now()+500;while(Date.now()<end){};return 'old'})()"
            );
            await primary.ExecuteAsync(
                new(
                    WebViewOperation.LoadHtml,
                    "<!doctype html><title>replacement</title><input value='retained'>"
                )
            );
            await Reject(() => pending, WebViewError.NavigationChanged);
            await Loaded(primary, "replacement");
            await Reject(
                () =>
                    primary.ExecuteAsync(
                        new(WebViewOperation.EvaluateJavaScript, "1", prior.DocumentGeneration)
                    ),
                WebViewError.NavigationChanged
            );
            Check(await primary.Ready == handle, "navigation preserves attachment identity");
            using (var cancel = new CancellationTokenSource())
            {
                var canceled = primary.ExecuteAsync(
                    new(
                        WebViewOperation.EvaluateJavaScript,
                        "(()=>{const end=Date.now()+300;while(Date.now()<end){};return 1})()"
                    ),
                    cancel.Token
                );
                cancel.Cancel();
                try
                {
                    await canceled;
                    throw new Exception("Expected cancellation.");
                }
                catch (OperationCanceledException)
                {
                    lines.Add("PASS caller cancellation");
                }
            }
            await Reject(
                () => primary.ExecuteAsync(new(WebViewOperation.ClearData)),
                WebViewError.Unsupported
            );

            var routes = new Dictionary<string, WebViewResource>
            {
                ["/index.html"] = new("webview/index.html", "text/html"),
                ["/style.css"] = new("webview/style.css", "text/css"),
            };
            var options = new WebViewOptions(
                AllowedOrigins: [],
                Resources: routes,
                MessageOrigins: ["doroti-app://content"]
            );
            await using var content = await Create(options);
            await content.Ready;
            var messages = new ConcurrentQueue<WebViewEvent>();
            content.Changed += value =>
            {
                if (value.Kind == WebViewEventKind.Message)
                {
                    messages.Enqueue(value);
                }
            };
            await content.LoadAppContentAsync("webview/index.html");
            await Loaded(content, "App content");
            Task<WebViewResult> ContentJs(string script) =>
                content.ExecuteAsync(new(WebViewOperation.EvaluateJavaScript, script));
            Check(
                (await ContentJs("getComputedStyle(document.querySelector('#content')).color")).Json
                    == "\"rgb(12, 34, 56)\"",
                "manifest HTML and relative CSS"
            );
            await content.ExecuteAsync(
                new(WebViewOperation.Navigate, "doroti-app://content/index.html?second=1")
            );
            await Loaded(content, "App content");
            Check(
                (await content.ExecuteAsync(new(WebViewOperation.State))).CanGoBack,
                "navigation history"
            );
            await content.ExecuteAsync(new(WebViewOperation.Back));
            await Loaded(content, "App content");
            Check(
                (await content.ExecuteAsync(new(WebViewOperation.State))).CanGoForward,
                "back enables forward"
            );
            await content.ExecuteAsync(new(WebViewOperation.Forward));
            await Loaded(content, "App content");
            Check(
                (await content.ExecuteAsync(new(WebViewOperation.State))).Url!.EndsWith(
                    "?second=1"
                ),
                "forward restores URL"
            );
            var beforeReload = (
                await content.ExecuteAsync(new(WebViewOperation.State))
            ).DocumentGeneration;
            await content.ExecuteAsync(new(WebViewOperation.Reload));
            await Loaded(content, "App content");
            Check(
                (await content.ExecuteAsync(new(WebViewOperation.State))).DocumentGeneration
                    > beforeReload,
                "reload advances document generation"
            );
            await Reject(() => ContentJs("'x'.repeat(2*1024*1024+1)"), WebViewError.JavaScript);
            await ContentJs(
                "window.rangeResult=null;fetch('style.css',{headers:{Range:'bytes=0-3'}}).then(async r=>window.rangeResult=[r.status,(await r.text()).length],()=>window.rangeResult='denied');undefined"
            );
            for (var i = 0; i < 30 && (await ContentJs("window.rangeResult")).Json == "null"; i++)
            {
                await Task.Delay(50);
            }

            Check(
                (await ContentJs("window.rangeResult")).Json == "\"denied\"",
                "unsupported Range rejected explicitly: "
                    + (await ContentJs("window.rangeResult")).Json
            );
            for (var i = 0; i < 30 && (await ContentJs("typeof doroti")).Json != "\"object\""; i++)
            {
                await Task.Delay(50);
            }

            await ContentJs("doroti.postMessage('probe',{value:'한글'})");
            for (var i = 0; i < 30 && messages.IsEmpty; i++)
            {
                await Task.Delay(50);
            }

            Check(
                messages.TryDequeue(out var message)
                    && message.MessageName == "probe"
                    && message.MessageJson?.Contains("한글") == true,
                "trusted app isolated-world transport"
            );
            await ContentJs("doroti.postMessage('oversized','x'.repeat(70000));undefined");
            await ContentJs(
                "window.dispatchEvent(new CustomEvent('doroti-message',{detail:JSON.stringify({generation:0,requestId:1,name:'stale',payload:null})}));undefined"
            );
            await Task.Delay(300);
            Check(messages.IsEmpty, "oversized and stale messages rejected");
            await ContentJs("localStorage.setItem('doroti-probe','private');undefined");
            await using var isolated = await Create(options);
            await isolated.Ready;
            await isolated.ExecuteAsync(
                new(WebViewOperation.Navigate, "doroti-app://content/index.html")
            );
            await Loaded(isolated, "App content");
            Check(
                (
                    await isolated.ExecuteAsync(
                        new(
                            WebViewOperation.EvaluateJavaScript,
                            "localStorage.getItem('doroti-probe')"
                        )
                    )
                ).Json == "null",
                "ephemeral profiles isolated"
            );
            await content.ExecuteAsync(
                new(WebViewOperation.LoadHtml, "<!doctype html><title>untrusted</title>")
            );
            await Loaded(content, "untrusted");
            var state = await content.ExecuteAsync(new(WebViewOperation.State));
            Check(
                (await ContentJs("typeof doroti")).Json == "\"undefined\"",
                "untrusted document has no message facade"
            );
            await Task.Delay(150);
            Check(messages.IsEmpty, "untrusted about:blank origin rejected");
            await Reject(
                () => content.ExecuteAsync(new(WebViewOperation.Navigate, "https://example.com")),
                WebViewError.InvalidRequest
            );
            await content.DisposeAsync();
            await Reject(
                () => content.ExecuteAsync(new(WebViewOperation.State)),
                WebViewError.Closed
            );
            await using (var shared = await Create(new(Profile: WebViewProfile.SharedPersistent)))
            {
                await shared.Ready;
                await Loaded(shared);
                Check(
                    (await shared.ExecuteAsync(new(WebViewOperation.Features)))
                        .Features!
                        .SharedPersistentProfile,
                    "shared persistent profile creation"
                );
            }
            for (var iteration = 0; iteration < 10; iteration++)
            {
                var late = await Create(new());
                await late.DisposeAsync();
            }
            lines.Add("PASS ten create/dispose races");
            var retiring = await Create(options);
            await retiring.Ready;
            await retiring.LoadAppContentAsync("webview/index.html");
            await Loaded(retiring, "App content");
            var lateResult = retiring.ExecuteAsync(
                new(
                    WebViewOperation.EvaluateJavaScript,
                    "(()=>{const end=Date.now()+500;while(Date.now()<end){};return 'late'})()"
                )
            );
            await Task.Delay(50);
            await retiring.DisposeAsync();
            await Reject(() => lateResult, WebViewError.Closed);
            // Restore the original live scene for the separate pixel/input gate.
            lines.Add("PASS Linux Qt WebView product commands");
        }
        catch (Exception error)
        {
            lines.Add("FAIL " + error);
        }
        finally
        {
            File.WriteAllLines(path, lines);
        }
    }
}
