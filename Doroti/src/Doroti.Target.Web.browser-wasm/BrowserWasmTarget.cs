using System.Text.Json;
using System.Reflection;
using Doroti.Flutter.Hosting;
using Doroti.Flutter.Ui;
using Doroti.Host.Web;

namespace Doroti.Target.Web;

public sealed record BrowserTargetPackageManifest(
    string SchemaVersion,
    string PackageId,
    string PackageVersion,
    string Rid,
    string Host,
    string GraphicsBackend,
    string FlutterRevision,
    string ManagedCallbackAbi,
    string JavaScriptPluginAbi,
    string ResourceBasePolicy,
    string[] ResourceKinds,
    string[] SupportedPublishModes,
    string[] RequiredCapabilities,
    string[] UnsupportedModes);

public sealed record BrowserTargetIdentity(
    string SchemaVersion,
    string Rid,
    string Host,
    string GraphicsBackend,
    string PackageId,
    string PackageVersion,
    string FlutterRevision);

/// <summary>Package composition root for the required browser-wasm target.</summary>
[System.Runtime.Versioning.SupportedOSPlatform("browser")]
public sealed class BrowserWasmTarget : IDisposable
{
    private readonly BrowserFlutterHost _host;
    private bool _disposed;

    public BrowserWasmTarget()
    {
        if (!OperatingSystem.IsBrowser())
            throw new PlatformNotSupportedException("Doroti.Target.Web.browser-wasm requires a browser-wasm process.");
        Manifest = LoadManifest();
        Identity = new("doroti.target-identity/v1", Manifest.Rid, Manifest.Host,
            Manifest.GraphicsBackend, Manifest.PackageId, Manifest.PackageVersion, Manifest.FlutterRevision);
        _host = new($"{Identity.Rid}/document-canvas-webgl2");
    }

    public BrowserTargetPackageManifest Manifest { get; }
    public BrowserTargetIdentity Identity { get; }
    public string Rid => Identity.Rid;
    public string GraphicsBackend => Identity.GraphicsBackend;

    public FlutterView CreateView(
        FlutterHostSession session,
        ulong viewId,
        string canvasId,
        FlutterViewConfiguration configuration,
        FlutterApplicationBoundary? application = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _host.CreateView(session, viewId, canvasId, configuration, application);
    }

    public BrowserHostSnapshot CaptureSnapshot(ulong viewId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _host.CaptureSnapshot(viewId);
    }

    public string ResolveResourceUrl(ulong viewId, string relativeUrl)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _host.ResolveResourceUrl(viewId, relativeUrl);
    }

    public FlutterApplicationBoundary LoadApplicationBoundary(
        Assembly applicationAssembly,
        IEnumerable<BrowserJavaScriptPluginDescriptor>? plugins = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return FlutterApplicationBoundary.Load(
            applicationAssembly,
            Rid,
            (plugins ?? []).Select(descriptor => new BrowserJavaScriptPluginHandler(descriptor)));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _host.Dispose();
    }

    private static BrowserTargetPackageManifest LoadManifest()
    {
        using var stream = typeof(BrowserWasmTarget).Assembly.GetManifestResourceStream("Doroti.Target.Manifest")
            ?? throw new InvalidDataException("The browser target package manifest is missing.");
        return JsonSerializer.Deserialize<BrowserTargetPackageManifest>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        }) ?? throw new InvalidDataException("The browser target package manifest is invalid.");
    }
}
