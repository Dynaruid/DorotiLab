using System.IO;
using Doroti.Ui;
using Microsoft.Web.WebView2.Core;

namespace Doroti.Host.WindowsAppSdk;

internal sealed partial class WindowsWebViewComposition
{
    private async Task<Dictionary<string, byte[]>> LoadContent(
        WebViewOptions options,
        CancellationToken token
    )
    {
        var content = new Dictionary<string, byte[]>(StringComparer.Ordinal);
        long size = 0;
        foreach (var (path, route) in options.Resources ?? [])
        {
            var manifest =
                _resources.Resources.SingleOrDefault(item => item.Key == route.ResourceKey)
                ?? throw new WebViewException(
                    WebViewError.InvalidRequest,
                    "App resource is not in the manifest: " + route.ResourceKey
                );
            if (manifest.Length > 8 * 1024 * 1024 || (size += manifest.Length) > 64 * 1024 * 1024)
            {
                throw new WebViewException(
                    WebViewError.InvalidRequest,
                    "App content exceeds 8 MiB per resource or 64 MiB per view."
                );
            }

            var bytes = await _resources.LoadAsync(route.ResourceKey, token);
            if (bytes.Length != manifest.Length)
            {
                throw new WebViewException(
                    WebViewError.InvalidRequest,
                    "App resource length differs from manifest."
                );
            }

            content.Add(path, bytes.ToArray());
        }
        return content;
    }

    private sealed partial class Instance
    {
        private readonly Dictionary<string, byte[]> _content;

        private void ResourceRequested(
            object? sender,
            CoreWebView2WebResourceRequestedEventArgs args
        )
        {
            // Immutable manifest bytes are loaded before native creation. No filesystem,
            // network, asynchronous deferral, or placement reservation is needed here.
            var request = args.Request;
            if (
                _commandsDisabled
                || !Uri.TryCreate(request.Uri, UriKind.Absolute, out var uri)
                || uri.Scheme != "doroti-app"
                || uri.Host != "content"
                || !uri.IsDefaultPort
                || !string.IsNullOrEmpty(uri.UserInfo)
                || uri.AbsolutePath.Contains('%')
                || !_content.TryGetValue(uri.AbsolutePath, out var bytes)
                || _options.Resources?.TryGetValue(uri.AbsolutePath, out var route) != true
                || route is null
                || request.Method is not ("GET" or "HEAD")
            )
            {
                args.Response = _environment.CreateWebResourceResponse(
                    null,
                    404,
                    "Not Found",
                    "Content-Length: 0\r\n"
                );
                return;
            }
            var start = 0;
            var end = bytes.Length - 1;
            var status = 200;
            var headers =
                "Content-Type: "
                + route.MimeType
                + "\r\nAccept-Ranges: bytes\r\nCache-Control: no-store\r\n";
            if (request.Headers.Contains("Range"))
            {
                var range = request.Headers.GetHeader("Range");
                var parts = range.StartsWith("bytes=", StringComparison.Ordinal)
                    ? range[6..].Split('-')
                    : [];
                var valid = parts.Length == 2 && !range.Contains(',');
                if (valid && parts[0].Length == 0)
                {
                    valid = int.TryParse(parts[1], out var suffix) && suffix > 0;
                    if (valid)
                    {
                        start = Math.Max(0, bytes.Length - suffix);
                    }
                }
                else if (valid)
                {
                    valid = int.TryParse(parts[0], out start) && start >= 0;
                    if (valid && parts[1].Length > 0)
                    {
                        valid = int.TryParse(parts[1], out end);
                    }

                    end = Math.Min(end, bytes.Length - 1);
                }
                if (!valid || start > end || start >= bytes.Length)
                {
                    args.Response = _environment.CreateWebResourceResponse(
                        null,
                        416,
                        "Range Not Satisfiable",
                        $"Content-Range: bytes */{bytes.Length}\r\nContent-Length: 0\r\n"
                    );
                    return;
                }
                status = 206;
                headers += $"Content-Range: bytes {start}-{end}/{bytes.Length}\r\n";
            }
            var length = Math.Max(0, end - start + 1);
            headers += $"Content-Length: {length}\r\n";
            // WebView2 retains the COM stream until the response is consumed.
            var stream = new MemoryStream(
                bytes,
                start,
                request.Method == "HEAD" ? 0 : length,
                writable: false
            ).AsRandomAccessStream();
            args.Response = _environment.CreateWebResourceResponse(
                stream,
                status,
                status == 206 ? "Partial Content" : "OK",
                headers
            );
        }
    }
}
