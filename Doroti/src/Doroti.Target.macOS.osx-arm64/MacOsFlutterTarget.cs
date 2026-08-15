using System.Runtime.InteropServices;
using System.Text.Json;
using Doroti.Flutter.Hosting;
using Doroti.Flutter.Ui;
using Doroti.Host.Desktop;
using Doroti.Host.Desktop.Flutter;
using Doroti.Platform;
using Doroti.Vendor.Avalonia.Native;

namespace Doroti.Target.macOS;

public sealed record MacOsTargetPackageManifest(
    string SchemaVersion, string PackageId, string PackageVersion, string Rid,
    string OperatingSystem, string WindowBackend, string GraphicsBackend,
    string FlutterRevision, string AvaloniaRevision, string NativeAsset,
    string NativeArchitecture, string CapabilitySchema, string DiagnosticSchema,
    string TargetIdentitySchema);

public sealed record MacOsTargetIdentity(
    string SchemaVersion, string Rid, string OperatingSystem, string OperatingSystemDescription,
    string ProcessArchitecture, string FrameworkDescription, string WindowBackend,
    string GraphicsBackend, string PackageId, string PackageVersion, string FlutterRevision,
    string AvaloniaRevision, string NativeAsset);

/// <summary>Package composition root for the required G7 osx-arm64 desktop target.</summary>
public sealed class MacOsFlutterTarget : IDesktopFlutterTarget
{
    private readonly DesktopWindowBackend _backend;
    private readonly DesktopFlutterHost _host;
    private bool _disposed;

    public MacOsFlutterTarget()
    {
        if (!OperatingSystem.IsMacOS() || RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
            throw new PlatformNotSupportedException("Doroti.Target.macOS.osx-arm64 requires a macOS arm64 process.");
        Manifest = LoadManifest();
        _backend = new(MacOsShellPlatformFactory.Create());
        _host = new(_backend, $"{RuntimeInformation.RuntimeIdentifier}/appkit-nsopengl");
        Identity = new(Manifest.TargetIdentitySchema, RuntimeInformation.RuntimeIdentifier, "macos",
            RuntimeInformation.OSDescription, RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant(),
            RuntimeInformation.FrameworkDescription, Manifest.WindowBackend, Manifest.GraphicsBackend,
            Manifest.PackageId, Manifest.PackageVersion, Manifest.FlutterRevision, Manifest.AvaloniaRevision,
            Manifest.NativeAsset);
    }

    public MacOsTargetPackageManifest Manifest { get; }
    public MacOsTargetIdentity Identity { get; }
    public string Rid => Identity.Rid;
    public string GraphicsBackend => Identity.GraphicsBackend;

    public FlutterView CreateView(FlutterHostSession session, ulong viewId, FlutterViewConfiguration configuration,
        FlutterApplicationBoundary? application = null)
    { ObjectDisposedException.ThrowIf(_disposed, this); return _host.CreateView(session, viewId, configuration, application); }

    public void PumpPendingMessages() { ObjectDisposedException.ThrowIf(_disposed, this); _backend.PumpPendingMessages(); }
    public DesktopFlutterTargetDiagnostics CaptureDiagnostics(ulong viewId) => _host.GetTargetDiagnostics(viewId);
    public SemanticsTreeSnapshot? GetSemanticsSnapshotForValidation(ulong viewId) => _host.GetSemanticsSnapshotForValidation(viewId);
    public Task<DesktopFlutterPixelReadback> CaptureNextFrameAsync(ulong viewId) => _host.CaptureNextFrameAsync(viewId);
    public NativeResourceSnapshot CaptureResourceSnapshot() => _backend.CaptureResourceSnapshot();
    public IPlatformServicesHostCapability GetPlatformServicesForValidation(ulong viewId) => _host.GetPlatformServicesForValidation(viewId);
    public void PostPointerTapForValidation(ulong viewId, double x, double y) => _host.PostPointerTapForValidation(viewId, x, y);
    public void PostPointerMoveForValidation(ulong viewId, double x, double y) => _host.PostPointerMoveForValidation(viewId, x, y);
    public void PostPointerDownForValidation(ulong viewId, double x, double y) => _host.PostPointerDownForValidation(viewId, x, y);
    public void PostPointerUpForValidation(ulong viewId, double x, double y) => _host.PostPointerUpForValidation(viewId, x, y);
    public void PostPointerWheelForValidation(ulong viewId, double x, double y, double dx, double dy) => _host.PostPointerWheelForValidation(viewId, x, y, dx, dy);
    public void PostKeyboardActivationForValidation(ulong viewId, uint key) => _host.PostKeyboardActivationForValidation(viewId, key);
    public void PostTextInputForValidation(ulong viewId, string text) => _host.PostTextInputForValidation(viewId, text);
    public nint GetNativeWindowHandle(ulong viewId) => _host.GetNativeWindowHandle(viewId);

    public void Dispose()
    {
        if (_disposed) return; _disposed = true; _host.Dispose(); _backend.Dispose();
    }

    private static MacOsTargetPackageManifest LoadManifest()
    {
        using var stream = typeof(MacOsFlutterTarget).Assembly.GetManifestResourceStream("Doroti.Target.Manifest")
            ?? throw new InvalidDataException("The macOS target package manifest is missing.");
        return JsonSerializer.Deserialize<MacOsTargetPackageManifest>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidDataException("The macOS target package manifest is invalid.");
    }
}
