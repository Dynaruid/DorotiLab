using System.Diagnostics;
using Doroti.Ui;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>Windows shell URL adapter. Acceptance does not imply that a page loaded.</summary>
public sealed class WindowsUrlLauncher : IUrlLauncherHostCapability
{
    public ValueTask<UrlLaunchResult> LaunchUrlAsync(string absoluteUrl, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!Uri.TryCreate(absoluteUrl, UriKind.Absolute, out var uri))
            return ValueTask.FromResult(new UrlLaunchResult(UrlLaunchStatus.invalidUrl));
        if (uri.Scheme is not ("http" or "https" or "mailto"))
            return ValueTask.FromResult(new UrlLaunchResult(UrlLaunchStatus.unsupported));
        try
        {
            using var process = Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true });
            return ValueTask.FromResult(new UrlLaunchResult(UrlLaunchStatus.opened));
        }
        catch (UnauthorizedAccessException error)
        { return ValueTask.FromResult(new UrlLaunchResult(UrlLaunchStatus.blocked, error.Message)); }
        catch (System.ComponentModel.Win32Exception error)
        { return ValueTask.FromResult(new UrlLaunchResult(UrlLaunchStatus.failed, error.Message)); }
    }
}
