#if SHARED_SESSION
using Doroti.Skia.Rendering;
using Silk.NET.Vulkan;
using SkiaSharp;
using VkDevice = Silk.NET.Vulkan.Device;
using VkImage = Silk.NET.Vulkan.Image;

internal static unsafe partial class Program
{
    private static void ProbeSharedSession(Vk vk, Instance instance, PhysicalDevice physical, VkDevice device, Queue queue, uint family, VulkanObserver observer)
    {
        var session = SkiaGraphiteSession.CreateOfficialVulkan(new(instance.Handle, physical.Handle, device.Handle, queue.Handle,
            family, ApiVersion, observer.Resolve, image => { var s = observer.State((ulong)image); return ((int)s.Layout, s.Family); }, observer.Check),
            Convert.ToInt64(Report["generation"]), maxFrames: 1);
        VkImage image = default; DeviceMemory memory = default; Fence fence = default;
        SkiaGraphiteSession.VulkanTarget? target = null;
        SkiaGraphiteSession.Frame? frame = null;
        bool submissionAttempted = false;
        int completed = 0, cancelled = 0, rejectedStateChanges = 0, readbacks = 0;
        try
        {
            var ci = new ImageCreateInfo { SType = StructureType.ImageCreateInfo, ImageType = ImageType.Type2D, Format = Format.R8G8B8A8Unorm,
                Extent = new(64, 64, 1), MipLevels = 1, ArrayLayers = 1, Samples = SampleCountFlags.Count1Bit,
                Usage = ImageUsageFlags.ColorAttachmentBit | ImageUsageFlags.InputAttachmentBit | ImageUsageFlags.SampledBit | ImageUsageFlags.TransferSrcBit | ImageUsageFlags.TransferDstBit };
            Check(observer.Call<VulkanObserver.CreateImageDelegate>("vkCreateImage")(device, &ci, null, &image), "session target image");
            vk.GetImageMemoryRequirements(device, image, out var requirements);
            vk.GetPhysicalDeviceMemoryProperties(physical, out var properties);
            uint memoryType = 0; while (memoryType < properties.MemoryTypeCount && (requirements.MemoryTypeBits & (1u << (int)memoryType)) == 0) memoryType++;
            if (memoryType == properties.MemoryTypeCount) throw new NotSupportedException("No target memory type.");
            var allocate = new MemoryAllocateInfo { SType = StructureType.MemoryAllocateInfo, AllocationSize = requirements.Size, MemoryTypeIndex = memoryType };
            Check(vk.AllocateMemory(device, &allocate, null, out memory), "session target allocation");
            Check(vk.BindImageMemory(device, image, memory, 0), "session target bind");
            observer.RegisterTarget(image.Handle);
            var textureInfo = new SKGraphiteVkTextureInfo { SampleCount = 1, Format = (int)ci.Format, ImageUsageFlags = (uint)ci.Usage, AspectMask = (uint)ImageAspectFlags.ColorBit };
            target = session.CreateVulkanTarget(64, 64, textureInfo, (int)ImageLayout.Undefined, family, (nint)image.Handle, SKColorType.Rgba8888);
            var fci = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
            Check(vk.CreateFence(device, &fci, null, out fence), "session fence");
            using var bitmap = new SKBitmap(8, 8); bitmap.Erase(new SKColor(0, 0, 255, 128)); using var raster = SKImage.FromBitmap(bitmap);
            for (int index = 0; index < 120; index++)
            {
                if (index % 17 == 0)
                {
                    frame = session.BeginVulkanFrame(target);
                    frame.Surface.Canvas.Clear(SKColors.Magenta); frame.Surface.Canvas.DrawImage(raster, 8, 8, SKSamplingOptions.Default);
                    var cancelledReadback = frame.RequestReadback(new SKImageInfo(64, 64, SKColorType.Rgba8888, SKAlphaType.Premul));
                    frame.CancelRecording(); frame = null; cancelled++;
                    if (!cancelledReadback.IsCanceled || !session.CanBeginFrame || session.OutstandingFrames != 0) throw new InvalidOperationException("Cancelled product frame retained admission/readback.");
                }
                frame = session.BeginVulkanFrame(target);
                var background = index % 2 == 0 ? SKColors.White : SKColors.Black;
                frame.Surface.Canvas.Clear(background); frame.Surface.Canvas.DrawImage(raster, 8, 8, SKSamplingOptions.Default);
                var readback = frame.RequestReadback(new SKImageInfo(64, 64, SKColorType.Rgba8888, SKAlphaType.Premul));
                if (session.CanBeginFrame) throw new InvalidOperationException("Session admitted a concurrent recording at capacity.");
                submissionAttempted = true; frame.Submit(); observer.Check();
                var si = new SubmitInfo { SType = StructureType.SubmitInfo };
                Check(vk.ResetFences(device, 1, in fence), "session fence reset");
                Check(observer.Call<VulkanObserver.QueueSubmitDelegate>("vkQueueSubmit")(queue, 1, &si, fence), "session terminal submit");
                var timer = System.Diagnostics.Stopwatch.StartNew();
                while (vk.GetFenceStatus(device, fence) == Result.NotReady && timer.Elapsed.TotalSeconds < 5) Thread.Sleep(1);
                Check(vk.GetFenceStatus(device, fence), "session completion");
                frame.CompleteGpuWork(); frame = null; submissionAttempted = false; completed++;
                if (!readback.IsCompletedSuccessfully) throw new InvalidOperationException("Product session readback pending after completion.");
                var result = readback.GetAwaiter().GetResult();
                int offset = 10 * result.RowBytes + 10 * 4;
                int expectedRed = index % 2 == 0 ? 127 : 0, expectedBlue = index % 2 == 0 ? 255 : 128;
                if (Math.Abs(result.Pixels[offset] - expectedRed) > 1 || Math.Abs(result.Pixels[offset + 1] - expectedRed) > 1 || Math.Abs(result.Pixels[offset + 2] - expectedBlue) > 1 || result.Pixels[offset + 3] != 255)
                    throw new InvalidOperationException("Cancelled upload reused stale pixels on changed background.");
                readbacks++;
                var state = target.GetState(); target.SetStateAfterGpuCompletion(state.Layout, state.QueueFamily);
                try { target.SetStateAfterGpuCompletion((int)ImageLayout.Undefined, family); }
                catch (InvalidOperationException) { rejectedStateChanges++; }
                if (target.GetState() != state || !session.CanBeginFrame || session.OutstandingFrames != 0) throw new InvalidOperationException("Product target state/admission changed incorrectly.");
            }
            // Unsupported binary semaphore submission is rejected at the managed
            // host boundary; no semaphore handle is passed to the Vulkan driver.
            frame = session.BeginVulkanFrame(target); frame.Surface.Canvas.Clear(SKColors.Red);
            bool rejected = false;
            submissionAttempted = true;
            try { frame.SubmitVulkan([1UL], []); } catch (NotSupportedException) { rejected = true; }
            if (!rejected || !session.IsFaulted || session.CanBeginFrame) throw new InvalidOperationException("Invalid official semaphore path did not fault admission.");
            var terminal = new SubmitInfo { SType = StructureType.SubmitInfo };
            Check(vk.ResetFences(device, 1, in fence), "failure terminal reset");
            Check(observer.Call<VulkanObserver.QueueSubmitDelegate>("vkQueueSubmit")(queue, 1, &terminal, fence), "failure terminal submit");
            Check(vk.WaitForFences(device, 1, in fence, true, 5_000_000_000), "failure terminal wait");
            frame.CompleteGpuWork(); frame = null; submissionAttempted = false;
            var cache = session.ImageCacheDiagnostics;
            if (rejectedStateChanges != 120) throw new InvalidOperationException("Official API allowed private-state mutation.");
            if (session.IsDeviceLost) throw new InvalidOperationException("A managed submission fault was incorrectly reported as device loss.");
            target.Dispose(); target = null;
            // Managed notification contract only, after actual GPU drain. This
            // explicitly simulates a host notification, never a driver result.
            session.NotifyVulkanDeviceLost();
            if (!session.IsDeviceLost || session.CanBeginFrame || vk.GetFenceStatus(device, fence) != Result.Success)
                throw new InvalidOperationException("Host loss notification was not latched independently of native driver state.");
            Report["sessionResult"] = new { completed, cancelled, readbacks, rejectedStateChanges, rejectedBinarySemaphores = rejected,
                cache.Uploads, cache.Hits, cache.Discarded, session.OutstandingFrames, session.IsFaulted,
                hostLossNotification = "PASS-simulated-managed-contract-after-GPU-drain", actualDeviceLoss = "notTested",
                frameLimit = session.MaxFrames, gpuOnlyProductPresentation = "notVerified", contextsPerDevice = "outer diagnostic context plus product session" };
        }
        finally
        {
            if (submissionAttempted) { Check(vk.DeviceWaitIdle(device), "session exceptional drain"); frame?.CompleteGpuWork(); }
            else frame?.CancelRecording();
            target?.Dispose(); session.Dispose();
            if (image.Handle != 0) observer.Call<VulkanObserver.DestroyImageDelegate>("vkDestroyImage")(device, image, null);
            if (memory.Handle != 0) vk.FreeMemory(device, memory, null);
            if (fence.Handle != 0) vk.DestroyFence(device, fence, null);
        }
    }
}
#endif
