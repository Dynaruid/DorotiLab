using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Yarp.ReverseProxy.Forwarder;

namespace Doroti.Web.DevServer;

public static class DevelopmentProxy
{
    public static void UseDevelopmentProxy(this IApplicationBuilder app, WebDevConfig config)
    {
        app.UseWebSockets();
        var client = app.ApplicationServices.GetRequiredService<HttpMessageInvoker>();
        var forwarder = app.ApplicationServices.GetRequiredService<IHttpForwarder>();
        app.Use(async (context, next) =>
        {
            // Leave debugger, runtime and SDK browser-refresh endpoints with their owners.
            if (context.Request.Path.StartsWithSegments("/_framework") ||
                context.Request.Path.StartsWithSegments("/_content") ||
                context.Request.Path.StartsWithSegments("/_vs") ||
                context.Request.Path.StartsWithSegments("/_blazor"))
            {
                await next(context);
                return;
            }
            Uri? destination = null;
            try
            {
                foreach (var rule in config.Proxy)
                    if ((destination = rule.Destination(context.Request.Path.ToUriComponent(), context.Request.QueryString.Value ?? "")) is not null) break;
            }
            catch (RegexMatchTimeoutException)
            {
                context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
                return;
            }
            if (destination is null) { await next(context); return; }
            await forwarder.SendAsync(context, destination.GetLeftPart(UriPartial.Authority), client,
                new ForwarderRequestConfig { ActivityTimeout = TimeSpan.FromSeconds(100) }, new DestinationTransformer(destination));
        });
    }

    public static HttpMessageInvoker CreateClient() => new(new SocketsHttpHandler
    {
        UseProxy = false,
        AllowAutoRedirect = false,
        AutomaticDecompression = DecompressionMethods.None,
        UseCookies = false,
        EnableMultipleHttp2Connections = true,
        ConnectTimeout = TimeSpan.FromSeconds(15),
    });

    private sealed class DestinationTransformer(Uri destination) : HttpTransformer
    {
        public override async ValueTask TransformRequestAsync(HttpContext context, HttpRequestMessage request, string prefix, CancellationToken token)
        {
            await base.TransformRequestAsync(context, request, prefix, token);
            request.RequestUri = destination;
            request.Headers.Host = null;
        }
    }
}
