using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Doroti.Ui;

public enum WebViewProfile { Ephemeral, SharedPersistent }
public enum WebViewError { NotReady, Closed, Unsupported, InvalidRequest, NavigationChanged, JavaScript, ProcessFailed, Busy }
public sealed class WebViewException(WebViewError code, string message) : Exception(message)
{
    public WebViewError Code { get; } = code;
}
public enum WebViewOperation { Features, State, Navigate, LoadHtml, Reload, Stop, Back, Forward, EvaluateJavaScript, ClearData }
public enum WebViewEventKind { Started, Committed, Completed, Failed, ProcessFailed, Message }
public sealed record WebViewEvent(PlatformViewHandle Handle, long NavigationId, long DocumentGeneration,
    WebViewEventKind Kind, string? Url, string? Error = null, string? MessageName = null, string? MessageJson = null, long MessageRequestId = 0);
public sealed record WebViewCommand(WebViewOperation Operation, string? Text = null, long DocumentGeneration = 0);
public sealed record WebViewFeatures(bool Navigation, bool JavaScript, bool EphemeralProfile, bool SharedPersistentProfile,
    bool ClearAllData, bool ScriptMessages = false, bool AppContentScheme = false,
    bool IsolatedPersistentProfile = false, bool FirstContentFrame = false);
public sealed record WebViewResult(long RequestId, long NavigationId, long DocumentGeneration,
    string? Json = null, bool IsUndefined = false, string? Url = null, string? Title = null,
    bool IsLoading = false, bool CanGoBack = false, bool CanGoForward = false, WebViewFeatures? Features = null);

public sealed record WebViewResource(string ResourceKey, string MimeType);

/// <summary>Copied, versioned creation settings. Default profile is private and isolated per view.
/// Remote navigation is limited to HTTP(S). No file access, popup or external protocol launch.</summary>
public sealed record WebViewOptions(string? Html = null, WebViewProfile Profile = WebViewProfile.Ephemeral,
    string[]? AllowedOrigins = null, Dictionary<string, WebViewResource>? Resources = null,
    string[]? MessageOrigins = null)
{
    public const string Prefix = "doroti-webview:1\n";
    public ReadOnlyMemory<byte> Encode()
    {
        Validate();
        return Encoding.UTF8.GetBytes(Prefix + JsonSerializer.Serialize(this, WebViewJsonContext.Default.WebViewOptions));
    }
    public void Validate()
    {
        if (!Enum.IsDefined(Profile) || Encoding.UTF8.GetByteCount(Html ?? "") > 2 * 1024 * 1024)
            throw new WebViewException(WebViewError.InvalidRequest, "Invalid WebView profile or HTML larger than 2 MiB.");
        if ((Resources?.Count ?? 0) > 256 || (AllowedOrigins?.Length ?? 0) > 128 || (MessageOrigins?.Length ?? 0) > 128)
            throw new WebViewException(WebViewError.InvalidRequest, "Too many content routes or origins.");
        foreach (var (path, resource) in Resources ?? [])
            if (resource is null || !path.StartsWith('/') || path.Contains("..") || path.IndexOfAny(['%', '\\', '?', '#']) >= 0 ||
                string.IsNullOrWhiteSpace(resource.ResourceKey) || string.IsNullOrWhiteSpace(resource.MimeType) || resource.MimeType.Any(char.IsControl))
                throw new WebViewException(WebViewError.InvalidRequest, "App content requires an exact absolute URL path and a manifest resource key/MIME type.");
        foreach (var origin in MessageOrigins ?? [])
            if (origin != "doroti-app://content" && (!Uri.TryCreate(origin, UriKind.Absolute, out var uri) ||
                uri.Scheme is not ("https" or "http") || uri.GetLeftPart(UriPartial.Authority) != origin))
                throw new WebViewException(WebViewError.InvalidRequest, "MessageOrigins must contain canonical HTTP(S) or doroti-app://content origins.");
        foreach (var origin in AllowedOrigins ?? [])
            if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri) || uri.Scheme is not ("https" or "http") ||
                uri.GetLeftPart(UriPartial.Authority) != origin)
                throw new WebViewException(WebViewError.InvalidRequest, "AllowedOrigins must contain canonical HTTP(S) origins without paths.");
    }
}

public interface IWebViewHostCapability
{
    Task<WebViewResult> ExecuteWebViewAsync(PlatformViewHandle handle, WebViewCommand command, CancellationToken cancellationToken = default);
    event Action<WebViewEvent>? WebViewChanged;
}

[JsonSerializable(typeof(WebViewOptions))]
public sealed partial class WebViewJsonContext : JsonSerializerContext;
