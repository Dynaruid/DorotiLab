using SkiaSharp;

namespace Doroti.Host.WindowsAppSdk;

internal abstract class WindowsManagedHwndPresenterBase : IDisposable
{
    internal abstract string BackendName { get; }
    internal abstract string RuntimeEffectsBackend { get; }
    internal abstract string DiagnosticCoverage { get; }
    internal virtual ulong NativeRequiredFeatures => 0;
    internal virtual bool InvalidatesRendererSurfaceResourcesOnResize => true;
    internal abstract int Width { get; set; }
    internal abstract int Height { get; set; }
    internal abstract ulong DeviceGeneration { get; set; }
    internal abstract ulong ResizeBuffersCount { get; set; }
    internal abstract ulong ResizeInvalidCallCount { get; set; }
    internal abstract ulong PresentCount { get; set; }
    internal abstract ulong GpuSubmitCount { get; set; }
    internal abstract ulong GpuCopyCount { get; set; }
    internal abstract bool LastPresentSucceeded { get; set; }
    internal abstract ulong InitializationDebugMessageCount { get; set; }
    internal abstract ulong InitializationDebugErrorCount { get; set; }
    internal abstract ulong OperationalDebugMessageCount { get; set; }
    internal abstract ulong OperationalDebugErrorCount { get; set; }
    internal abstract ulong OperationalDebugWarningCount { get; set; }
    internal abstract string AdapterDescription { get; set; }

    internal abstract bool EnsureTarget(nint childWindow, int width, int height);
    internal abstract void SealInitializationDebugBaseline();
    internal abstract void CaptureOperationalDebugMessages();
    internal abstract T RenderAndPresent<T>(Func<SKSurface, T> paint, Predicate<T> shouldPresent);
    internal virtual bool PrepareForRendererGpuResourceRelease() => false;
    internal virtual bool TryAbandonGpuContextAfterRendererReleasePreflightFailure() => false;
    internal virtual void ResetDeviceAfterRendererGpuResourceRelease(bool deviceLost) => ResetDevice();
    internal virtual void DisposeAfterRendererGpuResourceRelease(bool deviceLost) => Dispose();
    internal virtual void DisposeAfterRendererGpuResourceReleaseFailure() => Dispose();
    internal abstract void ResetDevice();
    public abstract void Dispose();
}
