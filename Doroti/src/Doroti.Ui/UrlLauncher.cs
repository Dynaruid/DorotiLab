namespace Doroti.Ui;

public enum UrlLaunchStatus { opened, blocked, unsupported, invalidUrl, failed }
public readonly record struct UrlLaunchResult(UrlLaunchStatus Status, string? Message = null)
{
    /// <summary>The OS/browser accepted the request; this does not imply remote page loading.</summary>
    public bool Succeeded => Status == UrlLaunchStatus.opened;
}
public interface IUrlLauncherHostCapability
{
    ValueTask<UrlLaunchResult> LaunchUrlAsync(string absoluteUrl, CancellationToken cancellationToken = default);
}
