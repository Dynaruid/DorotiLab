using System.Diagnostics;
using System.Runtime.InteropServices;
using Doroti.Skia.Rendering;
using Doroti.Skia.Vulkan;
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
    private const bool _officialGraphiteSelected = true;
    private static string? _actualGraphiteLibraryPath, _officialGraphiteNativeHash;
    private VulkanObserver? _stockObserver;
    private ImageLayout _graphiteCopyRestoreLayout = ImageLayout.ColorAttachmentOptimal;
    private uint RequiredVulkanApiVersion => _officialGraphiteSelected ? (1u << 22) | (2u << 12) : VulkanApiVersion11;

    // Select one packaged module before Skia calls. An explicit absolute override
    // is retained for qualification; no fallback to a different native module.
    internal static void ConfigureGraphiteLibrary()
    {
        var manifest = Environment.GetEnvironmentVariable("DOROTI_WINDOWS_GRAPHITE_OFFICIAL_MANIFEST");
        var asset = string.IsNullOrWhiteSpace(manifest) ? GraphiteNativeLibrary.PackagedOfficialAsset()
            : GraphiteNativeLibrary.ReadOfficialManifest(manifest);
        lock (GraphiteLibraryGate)
        {
            GraphiteNativeLibrary.ConfigureOfficial(asset);
            _graphiteLibraryPath = Path.GetFullPath(asset.NativePath);
            _actualGraphiteLibraryPath = Process.GetCurrentProcess().Modules.Cast<ProcessModule>()
                .Single(module => module.ModuleName.Equals("libSkiaSharp.dll", StringComparison.OrdinalIgnoreCase)).FileName;
            if (!_actualGraphiteLibraryPath.Equals(_graphiteLibraryPath, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Actual loaded official Graphite path differs from provenance.");
            _officialGraphiteNativeHash = asset.Sha256;
        }
    }

    private void CreateGraphiteContext(IReadOnlyList<string> deviceExtensions)
    {
        _stockObserver = new VulkanObserver(_vk, _instance, _device, _queue, _queueFamily,
            deviceExtensions.Contains("VK_KHR_create_renderpass2"));
        _graphite = SkiaGraphiteSession.CreateOfficialVulkan(new(_instance.Handle, _physicalDevice.Handle,
            _device.Handle, _queue.Handle, _queueFamily, RequiredVulkanApiVersion,
            _stockObserver.Resolve, image => { var s = _stockObserver.State((ulong)image); return ((int)s.Layout, s.Family); }, _stockObserver.Check),
            checked((long)DeviceGeneration + 1), maxFrames: 1);
        RecordEvent($"Graphite/Vulkan official context generation={_graphite.Generation} native={_graphiteLibraryPath}");
    }

    private void WrapGraphiteBacking(int width, int height)
    {
        var layout = ImageLayout.ColorAttachmentOptimal;
        if (_stockObserver != null)
        {
            _stockObserver.RegisterHostTarget(_backingImage.Handle, new ImageCreateInfo {
                ImageType = ImageType.Type2D, Format = _format, Extent = new((uint)_backingCapacityWidth, (uint)_backingCapacityHeight, 1),
                MipLevels = 1, ArrayLayers = 1, Samples = SampleCountFlags.Count1Bit, InitialLayout = layout });
            layout = _stockObserver.State(_backingImage.Handle).Layout;
        }
        _graphiteTarget = _graphite!.CreateVulkanTarget(width, height, new SKGraphiteVkTextureInfo
        {
            SampleCount = 1, Format = (int)_format, ImageTiling = (int)ImageTiling.Optimal,
            ImageUsageFlags = (uint)(ImageUsageFlags.ColorAttachmentBit | ImageUsageFlags.InputAttachmentBit |
                ImageUsageFlags.TransferSrcBit | ImageUsageFlags.TransferDstBit | ImageUsageFlags.SampledBit),
            SharingMode = (int)SharingMode.Exclusive, AspectMask = (uint)ImageAspectFlags.ColorBit,
        }, (int)layout, _queueFamily, (nint)_backingImage.Handle,
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
        _stockObserver?.TakeShutdownOwnershipAfterGpuDrain();
        _graphite.TakeVulkanShutdownOwnershipAfterGpuDrain();
        if (lost) _graphite.NotifyVulkanDeviceLost();
        ReturnGraphiteFrameAfterGpuCompletion();
    }

    private void ObservedPipelineBarrier(CommandBuffer command, PipelineStageFlags source, PipelineStageFlags destination, DependencyFlags flags,
        uint memoryCount, MemoryBarrier* memory, uint bufferCount, BufferMemoryBarrier* buffers, uint imageCount, ImageMemoryBarrier* images)
    {
        if (_stockObserver == null) _vk.CmdPipelineBarrier(command, source, destination, flags, memoryCount, memory, bufferCount, buffers, imageCount, images);
        else _stockObserver.Call<VulkanObserver.CmdPipelineBarrierDelegate>("vkCmdPipelineBarrier")(command, source, destination, flags, memoryCount, memory, bufferCount, buffers, imageCount, images);
    }
}
