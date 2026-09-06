using Doroti.Ui;

namespace Doroti.Framework.Services;

/// <summary>Opens an absolute HTTP(S) URL through the current view's host.</summary>
public static class UrlLauncher
{
    public static async ValueTask<UrlLaunchResult> launchUrl(string url, CancellationToken cancellationToken = default)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp) || string.IsNullOrEmpty(uri.Host) || !string.IsNullOrEmpty(uri.UserInfo))
            return new(UrlLaunchStatus.invalidUrl, "An absolute HTTP or HTTPS URL without credentials is required.");
        var dispatcher = PlatformDispatcher.instance;
        var view = dispatcher.implicitView ?? dispatcher.views.FirstOrDefault();
        if (view is null) return new(UrlLaunchStatus.unsupported, "URL launch requires an attached view.");
        IUrlLauncherHostCapability host;
        try { host = view.RequireCapability<IUrlLauncherHostCapability>(DorotiCapabilityIds.UrlLauncher, DartUiInvocation.Managed("UrlLauncher.launchUrl")); }
        catch (DorotiCapabilityException) { return new(UrlLaunchStatus.unsupported, "This host does not support opening external URLs."); }
        try { return await host.LaunchUrlAsync(uri.AbsoluteUri, cancellationToken); }
        catch (OperationCanceledException) { throw; }
        catch (Exception error) { return new(UrlLaunchStatus.failed, error.Message); }
    }
}
