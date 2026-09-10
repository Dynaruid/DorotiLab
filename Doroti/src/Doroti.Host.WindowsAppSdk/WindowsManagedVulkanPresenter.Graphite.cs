using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using Doroti.Skia.Rendering;
using Silk.NET.Vulkan;
using SkiaSharp;

namespace Doroti.Host.WindowsAppSdk;

internal sealed unsafe partial class WindowsManagedVulkanPresenter
{
    internal static bool GraphiteEnabled => Environment.GetEnvironmentVariable("DOROTI_WINDOWS_GRAPHITE") != "0";
    private readonly bool _useGraphite = GraphiteEnabled;
    private SkiaGraphiteSession? _graphite;
    private SkiaGraphiteSession.VulkanTarget? _graphiteTarget;
    private SkiaGraphiteSession.Frame? _graphiteFrame;
    private bool _graphiteSubmissionAttempted;
    private static readonly object GraphiteLibraryGate = new();
    private static string? _graphiteLibraryPath;
    private static nint _graphiteLibraryModule;

    // Select one packaged module before Skia calls. An explicit absolute override
    // is retained for qualification; no fallback to a different native module.
    internal static void ConfigureGraphiteLibrary()
    {
        var requested = Environment.GetEnvironmentVariable("DOROTI_WINDOWS_GRAPHITE_NATIVE");
        var packaged = string.IsNullOrWhiteSpace(requested);
        if (packaged) requested = Path.Combine(AppContext.BaseDirectory, "graphite", "libSkiaSharp.dll");
        if (!Path.IsPathFullyQualified(requested!))
            throw new InvalidOperationException("DOROTI_WINDOWS_GRAPHITE_NATIVE must be an absolute ABI 3 library path.");
        var path = Path.GetFullPath(requested!);
        lock (GraphiteLibraryGate)
        {
            if (_graphiteLibraryPath is not null)
            {
                if (!path.Equals(_graphiteLibraryPath, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("A process cannot change its Graphite native module.");
                return;
            }
            if (packaged)
            {
                var manifestPath = Path.Combine(AppContext.BaseDirectory, "graphite", "build-provenance.json");
                using var manifest = JsonDocument.Parse(File.ReadAllText(manifestPath));
                if (manifest.RootElement.GetProperty("bridgeAbi").GetInt32() != 3 ||
                    manifest.RootElement.GetProperty("rid").GetString() != "win-x64")
                    throw new InvalidDataException("Packaged Graphite native ABI/RID differs from the Windows host.");
                var expectedHash = manifest.RootElement.GetProperty("files").GetProperty("libSkiaSharp.dll").GetString();
                using var input = File.OpenRead(path);
                if (!Convert.ToHexString(SHA256.HashData(input)).Equals(expectedHash, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("Packaged Graphite native library hash differs from its build provenance.");
            }
            var alreadyLoaded = Process.GetCurrentProcess().Modules.Cast<ProcessModule>()
                .FirstOrDefault(module => module.ModuleName.Equals("libSkiaSharp.dll", StringComparison.OrdinalIgnoreCase));
            if (alreadyLoaded is not null && !path.Equals(alreadyLoaded.FileName, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Skia was already loaded from another path; start a fresh process for Graphite qualification.");
            var module = NativeLibrary.Load(path);
            // Validate the bridge before installing any resolver; failed selection
            // is fatal to the candidate, never a fallback to another renderer.
            var version = (delegate* unmanaged[Cdecl]<uint>)NativeLibrary.GetExport(module, "doroti_graphite_interop_version");
            if (version() != 3) throw new NotSupportedException("Windows Graphite requires native bridge ABI 3.");
            _ = NativeLibrary.GetExport(module, "doroti_graphite_vk_context_report_device_lost");
            nint Resolve(string name, Assembly assembly, DllImportSearchPath? search) => name is "libSkiaSharp" or "libSkiaSharp.dll" ? module : 0;
            NativeLibrary.SetDllImportResolver(typeof(SKGraphiteContext).Assembly, Resolve);
            _graphiteLibraryPath = path;
            _graphiteLibraryModule = module;
        }
    }

    private void CreateGraphiteContext(IReadOnlyList<string> deviceExtensions)
    {
        // vkCreateDevice enables no optional feature bits on this host. Forward
        // those zeros and the exact enabled extension list, not queried features.
        _graphite = SkiaGraphiteSession.CreateVulkan(new(_instance.Handle, _physicalDevice.Handle,
            _device.Handle, _queue.Handle, _queueFamily, Math.Min(VulkanApiVersion11, _deviceApiVersion),
            (name, instance, device) => GetVulkanProcedureAddress(name, new(instance), new(device)),
            [], deviceExtensions, ResolveNativeSymbol: name => NativeLibrary.GetExport(_graphiteLibraryModule, name)),
            checked((long)DeviceGeneration + 1), maxFrames: 1);
        RecordEvent($"Graphite/Vulkan ABI 3 context generation={_graphite.Generation} native={_graphiteLibraryPath}");
    }

    private void WrapGraphiteBacking(int width, int height)
    {
        _graphiteTarget = _graphite!.CreateVulkanTarget(width, height, new SKGraphiteVkTextureInfo
        {
            SampleCount = 1, Format = (int)_format, ImageTiling = (int)ImageTiling.Optimal,
            ImageUsageFlags = (uint)(ImageUsageFlags.ColorAttachmentBit | ImageUsageFlags.InputAttachmentBit |
                ImageUsageFlags.TransferSrcBit | ImageUsageFlags.TransferDstBit | ImageUsageFlags.SampledBit),
            SharingMode = (int)SharingMode.Exclusive, AspectMask = (uint)ImageAspectFlags.ColorBit,
        }, (int)ImageLayout.ColorAttachmentOptimal, _queueFamily, (nint)_backingImage.Handle,
            _format == Format.B8G8R8A8Unorm ? SKColorType.Bgra8888 : SKColorType.Rgba8888);
    }

    private void ReturnGraphiteFrameAfterGpuCompletion()
    {
        if (_graphiteFrame is null) return;
        if (_graphiteSubmissionAttempted) _graphiteFrame.CompleteGpuWork();
        else _graphiteFrame.CancelRecording();
        _graphiteFrame = null;
        _graphiteSubmissionAttempted = false;
    }

    // Called only after native Run returns (StopRenderWorker has joined it).
    internal void TakeGraphiteShutdownOwnershipAfterRenderWorkerJoined()
    {
        if (_graphite is null) return;
        var lost = false;
        try { WaitIdle(); }
        catch (WindowsManagedVulkanDeviceLostException) { lost = true; }
        _graphite.TakeVulkanShutdownOwnershipAfterGpuDrain();
        if (lost) _graphite.NotifyVulkanDeviceLost();
        ReturnGraphiteFrameAfterGpuCompletion();
    }
}
