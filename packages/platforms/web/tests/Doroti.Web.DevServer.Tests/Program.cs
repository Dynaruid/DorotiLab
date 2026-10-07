using System.Net;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using Doroti.Web.DevServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

if (args is ["--write-test-certificate", var directory])
{
    Directory.CreateDirectory(directory);
    using var key = RSA.Create(2048);
    var certificateRequest = new CertificateRequest("CN=localhost", key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    var names = new SubjectAlternativeNameBuilder();
    names.AddDnsName("localhost");
    names.AddIpAddress(IPAddress.Loopback);
    certificateRequest.CertificateExtensions.Add(names.Build());
    using var certificate = certificateRequest.CreateSelfSigned(DateTimeOffset.UtcNow.AddMinutes(-5), DateTimeOffset.UtcNow.AddDays(1));
    File.WriteAllText(Path.Combine(directory, "localhost.pem"), certificate.ExportCertificatePem());
    File.WriteAllText(Path.Combine(directory, "localhost-key.pem"), key.ExportPkcs8PrivateKeyPem());
    return;
}

static void Check(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
static WebDevConfig Parse(string text) => WebDevConfig.Parse(new StringReader(text), Environment.CurrentDirectory);
static string Address(WebApplication app) => app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single();
static WebApplicationBuilder Builder()
{
    var builder = WebApplication.CreateBuilder();
    builder.Logging.ClearProviders();
    builder.WebHost.UseUrls("http://127.0.0.1:0");
    return builder;
}

var config = Parse("""
{
  // Both comment styles and trailing commas are accepted.
  "server": {
    "host": "localhost",
    "port": 8080,
    "headers": [{"name": "X-Custom", "value": "ok"}],
    "proxy": [
      {"target": "http://localhost:5000/base/", "prefix": "/api/", "replace": "/report/"},
      {"target": "http://localhost:5001/", "regex": "^/versioned/(v\\d+)/(.*)", "replace": "/$2?apiVersion=$1"},
      /* Keep this prefix. */
      {"target": "http://localhost:5002/", "prefix": "/raw/"},
    ],
  },
}
""");
Check(config.Host == "localhost" && config.Port == 8080 && config.Headers["X-Custom"] == "ok", "Server settings");
Check(config.Proxy[0].Destination("/api/hello%20world", "?a=1&a=2")!.AbsoluteUri == "http://localhost:5000/base/report/hello%20world?a=1&a=2", "Prefix/query/base path");
Check(config.Proxy[1].Destination("/versioned/v2/users", "?search=a%2Fb")!.AbsoluteUri == "http://localhost:5001/users?apiVersion=v2&search=a%2Fb", "Regex captures/query");
Check(config.Proxy[2].Destination("/raw/item", "")!.AbsoluteUri == "http://localhost:5002/raw/item", "Unchanged prefix");
Check(config.Proxy[0].Destination("/other", "") is null, "No match");
Check(Parse("""{"server":{"proxy":[{"target":"http://localhost/","prefix":"/api/","replace":""}]}}""").Proxy[0].Destination("/api/item", "")!.AbsolutePath == "/item", "Empty replacement");
Check(Parse("""{"server":{"proxy":[]}}""").Proxy.Count == 0, "Empty config");
foreach (var invalid in new[] {
    """{"server":{"proxy":[{"target":"ftp://localhost/","prefix":"/api"}]}}""",
    """{"server":{"proxy":[{"target":"http://localhost/"}]}}""",
    """{"server":{"proxy":[{"target":"http://localhost/","prefix":"/api","regex":"api"}]}}""",
    """{"server":{"proxy":[{"target":"http://localhost/","regex":"["}]}}""",
    """{"server":{"port":70000}}""", """{"server":{"port":"8080"}}""", """{"server":{"host":"http://localhost"}}""",
    """{"server":{"porxy":[]}}""", """{"server":{"proxy":false}}""", """{"server":{"proxy":[{"target":"http://localhost/?secret=x","prefix":"/"}]}}""",
    """{"server":{"headers":[{"name":"Content-Length","value":"1"}]}}""", """{"server":{"headers":[{"name":"X Bad","value":"ok"}]}}""",
    """{"server":{"port":1,"port":2}}""", """{"server":{"proxy":null}}""", """{"server":{"proxy":[{"target":"http://localhost/","prefix":12}]}}""",
    """{"server": {"proxy": [}}""",
})
{
    try { Parse(invalid); throw new InvalidOperationException("Accepted invalid config: " + invalid); }
    catch (Exception error) when (error is InvalidDataException or ArgumentException or JsonException) { }
}
Console.WriteLine("Configuration, rewrite and rejection checks: PASS");

await using var backend = Builder().Build();
backend.UseWebSockets();
backend.Run(async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var socket = await context.WebSockets.AcceptWebSocketAsync();
        var bytes = new byte[256];
        var received = await socket.ReceiveAsync(bytes, CancellationToken.None);
        await socket.SendAsync(bytes.AsMemory(0, received.Count), received.MessageType, received.EndOfMessage, CancellationToken.None);
        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "done", CancellationToken.None);
        return;
    }
    if (context.Request.Path == "/redirect")
    {
        context.Response.StatusCode = 302;
        context.Response.Headers.Location = "/login";
        return;
    }
    context.Response.Headers.Append("Set-Cookie", "one=1; Path=/");
    context.Response.Headers.Append("Set-Cookie", "two=2; Path=/");
    context.Response.Headers["X-Backend"] = "yes";
    context.Response.StatusCode = context.Request.Method == "DELETE" ? 418 : 200;
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();
    await context.Response.WriteAsync($"{context.Request.Method}|{context.Request.Path.ToUriComponent()}{context.Request.QueryString}|{body}|{context.Request.Headers["X-Request"]}|{context.Request.Headers.Cookie}|{context.Request.Host}");
});
await backend.StartAsync();
var backendAddress = Address(backend);
var liveConfig = Parse($$"""
{
  "server": {
    "proxy": [
      {"target":"{{backendAddress}}/", "prefix":"/api/specific/", "replace":"/first/"},
      {"target":"{{backendAddress}}/", "prefix":"/api/", "replace":"/"},
      {"target":"{{backendAddress}}/", "regex":"^/versioned/(v\\d+)/(.*)", "replace":"/$2?apiVersion=$1"},
      {"target":"{{backendAddress}}/", "prefix":"/_framework/"}
    ]
  }
}
""");
var proxyBuilder = Builder();
proxyBuilder.Services.AddHttpForwarder();
proxyBuilder.Services.AddSingleton(_ => DevelopmentProxy.CreateClient());
await using var proxy = proxyBuilder.Build();
proxy.UseDevelopmentProxy(liveConfig);
proxy.Run(context => context.Response.WriteAsync("static fallback"));
await proxy.StartAsync();
var proxyAddress = Address(proxy);
using var client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false }) { BaseAddress = new Uri(proxyAddress) };
using var request = new HttpRequestMessage(HttpMethod.Post, "/api/hello%20world?a=1&a=2") { Content = new StringContent("payload") };
request.Headers.Add("X-Request", "custom");
request.Headers.Add("Cookie", "session=123");
using var response = await client.SendAsync(request);
Check(await response.Content.ReadAsStringAsync() == $"POST|/hello%20world?a=1&a=2|payload|custom|session=123|{new Uri(backendAddress).Authority}", "Method/body/headers/cookie/Host/escaped path forwarding");
Check(response.Headers.GetValues("Set-Cookie").Count() == 2 && response.Headers.GetValues("X-Backend").Single() == "yes", "Response headers/cookies");
Check((await client.GetStringAsync("/api/specific/item")).StartsWith("GET|/first/item|", StringComparison.Ordinal), "First matching rule wins");
Check((await client.GetStringAsync("/versioned/v3/users?q=a%2Fb")).StartsWith("GET|/users?apiVersion=v3&q=a%2Fb|", StringComparison.Ordinal), "Live regex rewrite");
Check(await client.GetStringAsync("/other") == "static fallback", "Unmatched static fallback");
Check(await client.GetStringAsync("/_framework/test") == "static fallback", "Framework route stays local");
using var redirect = await client.GetAsync("/api/redirect");
Check(redirect.StatusCode == HttpStatusCode.Found && redirect.Headers.Location?.OriginalString == "/login", "Backend redirects stay visible");
using var deleted = await client.DeleteAsync("/api/resource");
Check((int)deleted.StatusCode == 418, "Backend status preserved");
using var websocket = new ClientWebSocket();
await websocket.ConnectAsync(new Uri(proxyAddress.Replace("http://", "ws://", StringComparison.Ordinal) + "/api/socket"), CancellationToken.None);
await websocket.SendAsync(Encoding.UTF8.GetBytes("echo"), WebSocketMessageType.Text, true, CancellationToken.None);
var buffer = new byte[32];
var message = await websocket.ReceiveAsync(buffer, CancellationToken.None);
Check(Encoding.UTF8.GetString(buffer, 0, message.Count) == "echo", "WebSocket tunnel");
await websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "done", CancellationToken.None);
await proxy.StopAsync();
await backend.StopAsync();
Console.WriteLine("Live HTTP, ordered routing, status, cookies, static fallback and WebSocket forwarding: PASS");
