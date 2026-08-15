using System.Runtime.InteropServices;
using System.Text.Json;
using Doroti.Flutter.Hosting;
using Doroti.Flutter.Ui;
using Doroti.Host.Desktop;
using Doroti.Host.Desktop.Flutter;
using Doroti.Platform;
using Doroti.Vendor.Avalonia.Win32;

namespace Doroti.Target.Windows;

public sealed record WindowsTargetPackageManifest(
    string SchemaVersion,
    string PackageId,
    string PackageVersion,
    string Rid,
    string OperatingSystem,
    string WindowBackend,
    string GraphicsBackend,
    string FlutterRevision,
    string AvaloniaRevision,
    string SourcePortSelectionSha256,
    string SourcePortProvenanceSha256,
    string CapabilitySchema,
    string DiagnosticSchema,
    string TargetIdentitySchema);

public sealed record WindowsTargetIdentity(
    string SchemaVersion,
    string Rid,
    string OperatingSystem,
    string OperatingSystemDescription,
    string ProcessArchitecture,
    string FrameworkDescription,
    string WindowBackend,
    string GraphicsBackend,
    string PackageId,
    string PackageVersion,
    string FlutterRevision,
    string AvaloniaRevision,
    string SourcePortSelectionSha256,
    string SourcePortProvenanceSha256);

/// <summary>Packaged win-x64 composition root. Framework packages remain independent of this RID choice.</summary>
public sealed class WindowsFlutterTarget : IDesktopFlutterTarget
{
    private readonly DesktopWindowBackend _backend;
    private readonly DesktopFlutterHost _host;
    private bool _disposed;

    public WindowsFlutterTarget()
    {
        if (!OperatingSystem.IsWindows() || RuntimeInformation.ProcessArchitecture != Architecture.X64)
        {
            throw new PlatformNotSupportedException("Doroti.Target.Windows.win-x64 requires a Windows x64 process.");
        }
        Manifest = LoadManifest();
        _backend = new(Win32ShellPlatformFactory.Create());
        _host = new(_backend, $"{RuntimeInformation.RuntimeIdentifier}/win32-wgl");
        Identity = new(
            Manifest.TargetIdentitySchema,
            RuntimeInformation.RuntimeIdentifier,
            "windows",
            RuntimeInformation.OSDescription,
            RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant(),
            RuntimeInformation.FrameworkDescription,
            Manifest.WindowBackend,
            Manifest.GraphicsBackend,
            Manifest.PackageId,
            Manifest.PackageVersion,
            Manifest.FlutterRevision,
            Manifest.AvaloniaRevision,
            Manifest.SourcePortSelectionSha256,
            Manifest.SourcePortProvenanceSha256);
    }

    public WindowsTargetPackageManifest Manifest { get; }

    public WindowsTargetIdentity Identity { get; }

    public string Rid => Identity.Rid;

    public string GraphicsBackend => Identity.GraphicsBackend;

    public FlutterView CreateView(
        FlutterHostSession session,
        ulong viewId,
        FlutterViewConfiguration configuration,
        FlutterApplicationBoundary? application = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _host.CreateView(session, viewId, configuration, application);
    }

    public IPlatformServicesHostCapability GetPlatformServicesForValidation(ulong viewId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _host.GetPlatformServicesForValidation(viewId);
    }

    public void PumpPendingMessages()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _backend.PumpPendingMessages();
    }

    public DesktopFlutterTargetDiagnostics CaptureDiagnostics(ulong viewId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _host.GetTargetDiagnostics(viewId);
    }

    public DesktopFlutterRetainedDiagnostics CaptureRetainedDiagnosticsForValidation(ulong viewId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _host.GetRetainedDiagnosticsForValidation(viewId);
    }

    public SemanticsTreeSnapshot? GetSemanticsSnapshotForValidation(ulong viewId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _host.GetSemanticsSnapshotForValidation(viewId);
    }

    public nint GetNativeWindowHandle(ulong viewId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _host.GetNativeWindowHandle(viewId);
    }

    public Task<DesktopFlutterPixelReadback> CaptureNextFrameAsync(ulong viewId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _host.CaptureNextFrameAsync(viewId);
    }

    public NativeResourceSnapshot CaptureResourceSnapshot()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _backend.CaptureResourceSnapshot();
    }

    public void FailNextGpuFrameForValidation(ulong viewId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _host.FailNextGpuFrameForValidation(viewId);
    }

    public void PostPointerTapForValidation(ulong viewId, double logicalX, double logicalY)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _host.PostPointerTapForValidation(viewId, logicalX, logicalY);
    }

    public void PostPointerMoveForValidation(ulong viewId, double logicalX, double logicalY)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _host.PostPointerMoveForValidation(viewId, logicalX, logicalY);
    }

    public void PostPointerLeaveForValidation(ulong viewId, double logicalX, double logicalY)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _host.PostPointerLeaveForValidation(viewId, logicalX, logicalY);
    }

    public void PostPointerDownForValidation(ulong viewId, double logicalX, double logicalY)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _host.PostPointerDownForValidation(viewId, logicalX, logicalY);
    }

    public void PostPointerUpForValidation(ulong viewId, double logicalX, double logicalY)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _host.PostPointerUpForValidation(viewId, logicalX, logicalY);
    }

    public void PostPointerDragForValidation(
        ulong viewId,
        double logicalStartX,
        double logicalStartY,
        double logicalEndX,
        double logicalEndY)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _host.PostPointerDragForValidation(
            viewId,
            logicalStartX,
            logicalStartY,
            logicalEndX,
            logicalEndY);
    }

    public void PostPointerWheelForValidation(
        ulong viewId,
        double logicalX,
        double logicalY,
        double wheelDeltaX,
        double wheelDeltaY)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _host.PostPointerWheelForValidation(
            viewId,
            logicalX,
            logicalY,
            wheelDeltaX,
            wheelDeltaY);
    }

    public void PostKeyboardActivationForValidation(ulong viewId, uint logicalKey)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _host.PostKeyboardActivationForValidation(viewId, logicalKey);
    }

    public void PostTextInputForValidation(ulong viewId, string text)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _host.PostTextInputForValidation(viewId, text);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _host.Dispose();
        _backend.Dispose();
    }

    private static WindowsTargetPackageManifest LoadManifest()
    {
        using var stream = typeof(WindowsFlutterTarget).Assembly.GetManifestResourceStream("Doroti.Target.Manifest")
            ?? throw new InvalidDataException("The Windows target package manifest is missing.");
        return JsonSerializer.Deserialize<WindowsTargetPackageManifest>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        }) ?? throw new InvalidDataException("The Windows target package manifest is invalid.");
    }
}
