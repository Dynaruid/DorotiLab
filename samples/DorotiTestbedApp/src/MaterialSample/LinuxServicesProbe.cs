using System.Text;
using System.Text.Json;
using Doroti.Plugins;
using Doroti.Ui;

namespace MaterialSample;

internal static class LinuxServicesProbe
{
    internal static async Task RunAsync(DorotiView view, string path)
    {
        try
        {
            using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            var token = deadline.Token;
            var plugin = NativeFeatures.ForView(view);
            var support = await plugin.GetCapabilitiesAsync(token);
            if (!support.FilePicker || !support.UrlLauncher) throw new Exception("Missing native service capabilities.");
            await using (var result = await plugin.PickFilesAsync(new(Extensions: [".txt"]), token))
            {
                if (result.Status != FilePickStatus.selected || result.Files.Count != 1) throw new Exception("Picker did not select one file.");
                var bytes = new byte[32];
                var count = await result.Files[0].ReadAsync(0, bytes, token);
                if (Encoding.UTF8.GetString(bytes, 0, count) != "Qt 한글 read grant") throw new Exception("Incorrect picked bytes.");
            }
            using (var cancel = new CancellationTokenSource(TimeSpan.FromMilliseconds(500)))
            {
                try { await plugin.PickFilesAsync(new(), cancel.Token); throw new Exception("Picker cancellation ignored."); }
                catch (OperationCanceledException) when (cancel.IsCancellationRequested) { }
            }
            var drop = view.RequireCapability<IOsDragDropHostCapability>(DorotiCapabilityIds.OsDragDrop, DorotiUiInvocation.Managed("LinuxServicesProbe"));
            var delivered = new TaskCompletionSource<OsDropData>(TaskCreationOptions.RunContinuationsAsynchronously);
            using var receiver = drop.Register(new(OsDropAction.Copy, [OsDropFormats.Files, OsDropFormats.Text, OsDropFormats.UriList]), value =>
            { if (value.Phase == OsDropPhase.Drop) delivered.TrySetResult(value.Data!); });
            File.WriteAllText(path + ".drop-ready", "ready");
            using var payload = await delivered.Task.WaitAsync(token);
            if (payload.Text != "Qt drop 한글" || payload.Files.Count != 2 || payload.Files[1].Length != 5L * 1024 * 1024 * 1024
                || !payload.Uris.Any(uri => uri.AbsoluteUri == "https://example.com/qt")) throw new Exception("Incorrect dropped payload.");
            var buffer = new byte[32];
            var read = await payload.Files[0].ReadAsync(0, buffer, token);
            if (Encoding.UTF8.GetString(buffer, 0, read) != "Qt 한글 read grant") throw new Exception("Incorrect dropped bytes.");
            receiver.Dispose();
            if (!payload.IsDisposed) throw new Exception("Drop receiver retained payload after disposal.");
            try { await payload.Files[0].ReadAsync(0, buffer, token); throw new Exception("Disposed grant remained readable."); }
            catch (ObjectDisposedException) { }
            var source = view.RequireCapability<IOsDragSourceHostCapability>(DorotiCapabilityIds.OsDragSource, DorotiUiInvocation.Managed("LinuxServicesProbe"));
            using (var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(500)))
            {
                try
                {
                    var drag = await source.StartDragAsync(new(Text: "Qt source 한글", Uris: [new Uri("https://example.com/qt")]), cancellationToken: cancellation.Token);
                    if (!drag.Canceled || drag.Action != OsDropAction.None) throw new Exception("Unattended source drag was unexpectedly accepted.");
                }
                catch (OperationCanceledException) when (cancellation.IsCancellationRequested) { }
            }
            var urlLaunched = false;
            if (Environment.GetEnvironmentVariable("DOROTI_QT_URL_PROBE") is { Length: > 0 } url)
            {
                var launched = await plugin.LaunchUrlAsync(url, token);
                if (!launched.Succeeded) throw new Exception($"URL handler failed: {launched.Status}: {launched.Message}");
                urlLaunched = true;
            }
            File.WriteAllText(path, JsonSerializer.Serialize(new { picker = true, cancellation = true, copyDrop = true,
                sparse5GiB = true, revoked = true, urlLaunched, dragSourceCancellation = true, externalFileManager = "notVerified" }));
        }
        catch (Exception error) { File.WriteAllText(path + ".error", error.ToString()); }
    }
}
