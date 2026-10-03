namespace Doroti.Ui;

public enum UrlLaunchStatus
{
    opened,
    blocked,
    unsupported,
    invalidUrl,
    failed,
}

public readonly record struct UrlLaunchResult(UrlLaunchStatus Status, string? Message = null)
{
    /// <summary>The OS/browser accepted the request; this does not imply remote page loading.</summary>
    public bool Succeeded => Status == UrlLaunchStatus.opened;
}

public interface IUrlLauncherHostCapability
{
    UrlSchemeSupport QueryScheme(string scheme) => UrlLaunchPolicy.QueryScheme(scheme);

    ValueTask<UrlLaunchResult> LaunchUrlAsync(
        string absoluteUrl,
        CancellationToken cancellationToken = default
    );
}

public sealed record UrlSchemeSupport(string Scheme, bool Supported, bool ObservesExternalCompletion = false, string? Reason = null);

public static class UrlLaunchPolicy
{
    public static UrlSchemeSupport QueryScheme(string scheme)
    {
        var normalized = scheme.TrimEnd(':').ToLowerInvariant();
        return new(normalized, normalized is "http" or "https" or "mailto", Reason:
            normalized is "http" or "https" or "mailto" ? null : "Supported schemes: http, https, mailto.");
    }

    public static UrlLaunchResult? Validate(string? value, out Uri? uri)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out uri)) return new(UrlLaunchStatus.invalidUrl);
        if (!QueryScheme(uri.Scheme).Supported) return new(UrlLaunchStatus.unsupported, "Supported schemes: http, https, mailto.");
        if (uri.Scheme is "http" or "https" && string.IsNullOrEmpty(uri.Host)) return new(UrlLaunchStatus.invalidUrl);
        return null;
    }

    public static UrlLaunchResult FromStatus(string status) => Enum.TryParse<UrlLaunchStatus>(status, out var value)
        && Enum.IsDefined(value) ? new(value) : new(UrlLaunchStatus.failed, "Unknown URL launch result: " + status);
}
