using System.Security.Cryptography.X509Certificates;
using Doroti.Web.DevServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

try
{
    string RequiredArgument(string name)
    {
        var index = Array.IndexOf(args, name);
        return index >= 0 && index + 1 < args.Length ? args[index + 1] : throw new InvalidDataException($"Missing argument {name}.");
    }
    var applicationPath = Path.GetFullPath(RequiredArgument("--applicationpath"));
    var configPath = args.Contains("--web-dev-config") ? Path.GetFullPath(RequiredArgument("--web-dev-config")) : null;
    var config = configPath is null ? new WebDevConfig(null, null, null, null, new Dictionary<string, string>(), []) : WebDevConfig.Load(configPath);
    var suppressIsolation = args.Contains("--no-cop-headers");
    if (suppressIsolation && args.Contains("--apply-cop-headers"))
        throw new InvalidDataException("Specify only one isolation header option.");
    var isolated = args.Contains("--apply-cop-headers");
    var hostArguments = new List<string>();
    for (var index = 0; index < args.Length; index++)
    {
        if (args[index] is "--applicationpath" or "--web-dev-config") { index++; continue; }
        if (args[index] is not ("--apply-cop-headers" or "--no-cop-headers")) hostArguments.Add(args[index]);
    }
    var cliUrls = new ConfigurationBuilder().AddCommandLine(hostArguments.ToArray()).Build()["urls"];
    var builder = Host.CreateDefaultBuilder(hostArguments.ToArray())
        .ConfigureHostConfiguration(settings =>
        {
            settings.AddInMemoryCollection(new Dictionary<string, string?>
            {
                [WebHostDefaults.EnvironmentKey] = Environments.Development,
                [WebHostDefaults.StaticWebAssetsKey] = Path.ChangeExtension(applicationPath, ".staticwebassets.runtime.json"),
                ["Logging:LogLevel:Microsoft"] = "Warning",
                ["Logging:LogLevel:Microsoft.Hosting.Lifetime"] = "Information",
            });
            settings.AddJsonFile(Path.Combine(Path.GetDirectoryName(applicationPath)!, "blazor-devserversettings.json"), optional: true);
        })
        .ConfigureWebHostDefaults(web =>
        {
            web.UseStaticWebAssets();
            // Explicit --urls wins; JSONC wins over launchSettings' default URL.
            if (cliUrls is not null) web.UseUrls(cliUrls);
            else if (config.Host is not null || config.Port is not null || config.Certificate is not null)
            {
                var fallback = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(';')[0] ?? "http://localhost:5000";
                var address = new UriBuilder(fallback);
                address.Host = config.Host ?? address.Host;
                address.Port = config.Port ?? address.Port;
                if (config.Certificate is not null) address.Scheme = "https";
                web.UseUrls(address.Uri.GetLeftPart(UriPartial.Authority));
            }
            if (config.Certificate is not null)
            {
                using var pem = X509Certificate2.CreateFromPemFile(config.Certificate, config.CertificateKey);
                // Re-import for Windows Kestrel's persistent private-key requirements.
                var certificate = X509CertificateLoader.LoadPkcs12(pem.Export(X509ContentType.Pkcs12), null);
                web.ConfigureKestrel(options => options.ConfigureHttpsDefaults(https => https.ServerCertificate = certificate));
            }
            web.ConfigureServices(services =>
            {
                services.AddRouting();
                services.AddHttpForwarder();
                services.AddSingleton(_ => DevelopmentProxy.CreateClient());
            });
            web.Configure((context, app) =>
            {
                app.UseDeveloperExceptionPage();
                var pathBase = context.Configuration["pathbase"];
                if (!string.IsNullOrEmpty(pathBase))
                {
                    app.UsePathBase(pathBase);
                    app.Use(async (request, next) =>
                    {
                        if (request.Request.PathBase != pathBase) { request.Response.StatusCode = 404; return; }
                        await next(request);
                    });
                }
                app.Use(async (request, next) =>
                {
                    request.Response.OnStarting(() =>
                    {
                        foreach (var header in config.Headers) request.Response.Headers[header.Key] = header.Value;
                        if (isolated)
                        {
                            request.Response.Headers["Cross-Origin-Opener-Policy"] = "same-origin";
                            request.Response.Headers["Cross-Origin-Embedder-Policy"] = "require-corp";
                        }
                        else if (suppressIsolation)
                        {
                            request.Response.Headers.Remove("Cross-Origin-Opener-Policy");
                            request.Response.Headers.Remove("Cross-Origin-Embedder-Policy");
                        }
                        return Task.CompletedTask;
                    });
                    await next(request);
                });
                app.UseDevelopmentProxy(config);
                app.UseWebAssemblyDebugging();
                app.UseRouting();
                app.UseStaticFiles(new StaticFileOptions { ServeUnknownFileTypes = true });
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapStaticAssets(Path.ChangeExtension(applicationPath, ".staticwebassets.endpoints.json"));
                    endpoints.MapFallback("/runtimes/{**path}", context => { context.Response.StatusCode = 404; return Task.CompletedTask; });
                    endpoints.MapFallback("/_framework/{**path}", context => { context.Response.StatusCode = 404; return Task.CompletedTask; });
                    endpoints.MapFallbackToFile("index.html", new StaticFileOptions
                    {
                        OnPrepareResponse = response => response.Context.Response.Headers.CacheControl = "no-store",
                    });
                });
            });
        });
    Console.WriteLine($"Doroti Web development configuration: {configPath ?? "defaults"} ({config.Proxy.Count} proxy rules), isolation: {(isolated ? "on" : suppressIsolation ? "off" : "configured headers")}");
    await builder.Build().RunAsync();
    return 0;
}
catch (Exception error) when (error is InvalidDataException or ArgumentException or IOException or System.Security.Cryptography.CryptographicException)
{
    Console.Error.WriteLine(error.Message);
    return 1;
}
