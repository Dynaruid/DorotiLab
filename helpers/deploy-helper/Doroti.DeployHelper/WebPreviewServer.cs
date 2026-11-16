using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Doroti.DeployHelper;

public interface IWebPreviewServer
{
    Task RunAsync(string webRoot, int port, bool isolated, bool launchBrowser, CancellationToken ct);
}

public sealed class WebPreviewServer(IUserInterface ui) : IWebPreviewServer
{
    public async Task RunAsync(string webRoot, int port, bool isolated, bool launchBrowser, CancellationToken ct)
    {
        var root = Path.GetFullPath(webRoot);
        if (!File.Exists(Path.Combine(root, "index.html"))) throw new DeployException($"Published index.html not found in {root}.");
        var builder = WebApplication.CreateSlimBuilder(new WebApplicationOptions
        {
            Args = [], ContentRootPath = root, WebRootPath = root, EnvironmentName = Environments.Production,
        });
        builder.Logging.ClearProviders();
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.AddServerHeader = false;
            options.Listen(IPAddress.Loopback, port);
        });
        await using var app = builder.Build();
        app.Use(async (context, next) =>
        {
            if (isolated)
            {
                context.Response.Headers["Cross-Origin-Opener-Policy"] = "same-origin";
                context.Response.Headers["Cross-Origin-Embedder-Policy"] = "require-corp";
                context.Response.Headers["Cross-Origin-Resource-Policy"] = "same-origin";
            }
            context.Response.Headers.CacheControl = "no-store";
            await next(context);
        });
        var contentTypes = new FileExtensionContentTypeProvider();
        contentTypes.Mappings[".wasm"] = "application/wasm";
        contentTypes.Mappings[".mjs"] = "text/javascript";
        app.UseDefaultFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
            ContentTypeProvider = contentTypes, ServeUnknownFileTypes = true, DefaultContentType = "application/octet-stream",
        });
        app.Run(async context =>
        {
            var path = context.Request.Path;
            // Missing runtime files must stay 404, including extensionless paths.
            if ((HttpMethods.IsGet(context.Request.Method) || HttpMethods.IsHead(context.Request.Method)) &&
                !Path.HasExtension(path.Value) && !path.StartsWithSegments("/_framework") &&
                !path.StartsWithSegments("/_content") && !path.StartsWithSegments("/runtimes"))
            {
                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync(Path.Combine(root, "index.html"), context.RequestAborted);
            }
            else context.Response.StatusCode = StatusCodes.Status404NotFound;
        });
        await app.StartAsync(ct);
        var address = app.Urls.Single();
        ui.WriteLine($"\nPreview URL: {address}/\nServing published files: {root}\nPress Ctrl+C to stop the server and exit deploy-helper.");
        if (launchBrowser)
        {
            try { Process.Start(new ProcessStartInfo(address + "/") { UseShellExecute = true })?.Dispose(); }
            catch (Exception error) when (error is Win32Exception or InvalidOperationException)
            {
                ui.WriteLine($"Could not open the browser: {error.Message}\nOpen the preview URL manually.");
            }
        }
        // The helper owns this in-process server and awaits its entire lifetime.
        // No detached server or build output is used after shutdown.
        await app.WaitForShutdownAsync(ct);
        ui.WriteLine("Preview server stopped.");
    }
}
