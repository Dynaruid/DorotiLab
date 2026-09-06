using Doroti.Ui;

namespace Doroti.Host.Qt;

internal static class QtUrlLauncher
{
    internal static async ValueTask<UrlLaunchResult> LaunchUrlAsync(string absoluteUrl, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            var info = new System.Diagnostics.ProcessStartInfo("xdg-open") { UseShellExecute = false, CreateNoWindow = true };
            info.ArgumentList.Add(absoluteUrl);
            using var process = System.Diagnostics.Process.Start(info);
            if (process is null) return new(UrlLaunchStatus.failed, "Could not start xdg-open.");
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(TimeSpan.FromSeconds(15));
            try { await process.WaitForExitAsync(timeout.Token); }
            catch (OperationCanceledException) { if (!process.HasExited) process.Kill(); throw; }
            return process.ExitCode == 0 ? new(UrlLaunchStatus.opened) : new(UrlLaunchStatus.failed, $"xdg-open exited with {process.ExitCode}.");
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested) { return new(UrlLaunchStatus.failed, "xdg-open did not complete within 15 seconds."); }
        catch (OperationCanceledException) { throw; }
        catch (Exception error) { return new(UrlLaunchStatus.failed, error.Message); }
    }

}
