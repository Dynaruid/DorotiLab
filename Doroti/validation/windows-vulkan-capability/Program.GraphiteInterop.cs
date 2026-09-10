using System.Runtime.InteropServices;
using Silk.NET.Vulkan;
using SkiaSharp;
using Doroti.Skia.Rendering;
using Doroti.Skia.RuntimeEffects;
using VkDevice = Silk.NET.Vulkan.Device;
using VkBuffer = Silk.NET.Vulkan.Buffer;
using VkImage = Silk.NET.Vulkan.Image;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace Doroti.Validation.WindowsVulkanCapability;

internal static unsafe partial class Program
{
    private static class GraphiteInterop
    {
        [DllImport("libSkiaSharp", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint doroti_graphite_interop_version();
        [DllImport("libSkiaSharp", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool doroti_graphite_vk_texture_get_state(nint texture, out int layout, out uint family);
        [DllImport("libSkiaSharp", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool doroti_graphite_vk_texture_set_state(nint texture, int layout, uint family);
        [DllImport("libSkiaSharp", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool doroti_graphite_vk_insert_recording(nint context, nint recording,
            uint waitCount, ulong* waits, uint signalCount, ulong* signals);
        [DllImport("libSkiaSharp", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool doroti_graphite_has_unfinished_gpu_work(nint context);
    }

    private static object ProbeExternalTexture(Vk vk, PhysicalDevice physicalDevice, VkDevice device,
        Queue queue, uint family, SKGraphiteContext context, int size, bool failAfterSubmit = false,
        SkiaGraphiteSession? session = null, string? compositionLuid = null, Instance instance = default,
        bool failAfterPresent = false)
    {
        VkImage image = default;
        DeviceMemory imageMemory = default, bufferMemory = default;
        VkBuffer buffer = default;
        CommandPool pool = default;
        Fence fence = default;
        Semaphore beforeGraphite = default, afterGraphite = default;
        SKGraphiteRecorder? recorder = null;
        SKGraphiteBackendTexture? backend = null;
        SKSurface? surface = null;
        SKGraphiteRecording? recording = null;
        SkiaGraphiteSession.VulkanTarget? sessionTarget = null;
        SkiaGraphiteSession.Frame? sessionFrame = null;
        bool sessionSubmitted = false;
        GraphiteCompositionOutput? outputOwner = null;
        var imageFormat = compositionLuid is null ? Format.R8G8B8A8Unorm : Format.B8G8R8A8Unorm;
        var colorType = compositionLuid is null ? SKColorType.Rgba8888 : SKColorType.Bgra8888;
        using var imagePixels = new SKBitmap(new SKImageInfo(32, 32, SKColorType.Rgba8888, SKAlphaType.Premul));
        imagePixels.Erase(SKColors.Lime);
        imagePixels.SetImmutable();
        using var testImage = SKImage.FromBitmap(imagePixels);
        using var imageScene = session is not null ? new GraphiteImageScene(size) : null;
        vk.GetPhysicalDeviceMemoryProperties(physicalDevice, out var memoryProperties);
        uint FindMemory(uint allowed, MemoryPropertyFlags required)
        {
            for (uint i = 0; i < memoryProperties.MemoryTypeCount; i++)
                if ((allowed & (1u << (int)i)) != 0 && (memoryProperties.MemoryTypes[(int)i].PropertyFlags & required) == required) return i;
            throw new NotSupportedException($"No memory type for {required}.");
        }
        try
        {
            // VulkanCaps requires INPUT_ATTACHMENT as well as COLOR_ATTACHMENT
            // for a Graphite renderable texture (pinned m154 source contract).
            const ImageUsageFlags usage = ImageUsageFlags.ColorAttachmentBit | ImageUsageFlags.InputAttachmentBit | ImageUsageFlags.SampledBit |
                ImageUsageFlags.TransferSrcBit | ImageUsageFlags.TransferDstBit;
            var imageInfo = new ImageCreateInfo { SType = StructureType.ImageCreateInfo,
                ImageType = ImageType.Type2D, Format = imageFormat, Extent = new((uint)size, (uint)size, 1),
                MipLevels = 1, ArrayLayers = 1, Samples = SampleCountFlags.Count1Bit,
                Tiling = ImageTiling.Optimal, Usage = usage, SharingMode = SharingMode.Exclusive };
            Check(vk.CreateImage(device, &imageInfo, null, out image), "vkCreateImage(external)");
            vk.GetImageMemoryRequirements(device, image, out var imageRequirements);
            var allocation = new MemoryAllocateInfo { SType = StructureType.MemoryAllocateInfo,
                AllocationSize = imageRequirements.Size, MemoryTypeIndex = FindMemory(imageRequirements.MemoryTypeBits, MemoryPropertyFlags.DeviceLocalBit) };
            Check(vk.AllocateMemory(device, &allocation, null, out imageMemory), "vkAllocateMemory(image)");
            Check(vk.BindImageMemory(device, image, imageMemory, 0), "vkBindImageMemory");
            var bufferInfo = new BufferCreateInfo { SType = StructureType.BufferCreateInfo,
                Size = (ulong)(size * size * 4), Usage = BufferUsageFlags.TransferDstBit, SharingMode = SharingMode.Exclusive };
            Check(vk.CreateBuffer(device, &bufferInfo, null, out buffer), "vkCreateBuffer(readback)");
            vk.GetBufferMemoryRequirements(device, buffer, out var bufferRequirements);
            allocation.AllocationSize = bufferRequirements.Size;
            allocation.MemoryTypeIndex = FindMemory(bufferRequirements.MemoryTypeBits,
                MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit);
            Check(vk.AllocateMemory(device, &allocation, null, out bufferMemory), "vkAllocateMemory(buffer)");
            Check(vk.BindBufferMemory(device, buffer, bufferMemory, 0), "vkBindBufferMemory");
            var poolInfo = new CommandPoolCreateInfo { SType = StructureType.CommandPoolCreateInfo,
                QueueFamilyIndex = family, Flags = CommandPoolCreateFlags.ResetCommandBufferBit };
            Check(vk.CreateCommandPool(device, &poolInfo, null, out pool), "vkCreateCommandPool");
            var commandInfo = new CommandBufferAllocateInfo { SType = StructureType.CommandBufferAllocateInfo,
                CommandPool = pool, Level = CommandBufferLevel.Primary, CommandBufferCount = 1 };
            Check(vk.AllocateCommandBuffers(device, &commandInfo, out var command), "vkAllocateCommandBuffers");
            var fenceInfo = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
            Check(vk.CreateFence(device, &fenceInfo, null, out fence), "vkCreateFence");
            var semaphoreInfo = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo };
            Check(vk.CreateSemaphore(device, &semaphoreInfo, null, out beforeGraphite), "vkCreateSemaphore(before)");
            Check(vk.CreateSemaphore(device, &semaphoreInfo, null, out afterGraphite), "vkCreateSemaphore(after)");
            if (session is null)
            {
            recorder = context.CreateRecorder(64L * 1024 * 1024)
                ?? throw new InvalidOperationException("External recorder failed.");
            backend = SKGraphiteBackendTexture.CreateVulkan(size, size,
                new SKGraphiteVkTextureInfo { SampleCount = 1, Format = (int)Format.R8G8B8A8Unorm,
                    ImageTiling = (int)ImageTiling.Optimal, ImageUsageFlags = (uint)usage,
                    SharingMode = (int)SharingMode.Exclusive, AspectMask = (uint)ImageAspectFlags.ColorBit },
                (int)ImageLayout.Undefined, family, (nint)image.Handle)
                ?? throw new InvalidOperationException("External texture wrapping failed.");
            surface = SKSurface.Create(recorder, backend, SKColorType.Rgba8888)
                ?? throw new InvalidOperationException("External texture surface failed.");
            }
            else
            {
                sessionTarget = session.CreateVulkanTarget(size, size,
                    new SKGraphiteVkTextureInfo { SampleCount = 1, Format = (int)imageFormat,
                        ImageTiling = (int)ImageTiling.Optimal, ImageUsageFlags = (uint)usage,
                        SharingMode = (int)SharingMode.Exclusive, AspectMask = (uint)ImageAspectFlags.ColorBit },
                    (int)ImageLayout.Undefined, family, (nint)image.Handle, colorType);
                // Discard a stale frame before it can submit, and exercise ownership/admission guards.
                var cancelled = session.BeginVulkanFrame(sessionTarget);
                // Warm up then promote the real sample image into the product
                // raster cache, all inside the recording that will be discarded.
                var firstPaint = imageScene!.Draw(cancelled.Surface);
                imageScene.Cancel(firstPaint);
                var promotedPaint = imageScene.Draw(cancelled.Surface);
                imageScene.Cancel(promotedPaint);
                if (imageScene.Promotions == 0) throw new InvalidOperationException("Image cancellation did not exercise raster promotion.");
                cancelled.Surface.Canvas.Clear(SKColors.Magenta);
                cancelled.Surface.Canvas.DrawImage(testImage, new SKRect(0, 0, 8, 8),
                    new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));
                if (session.CanBeginFrame || session.OutstandingFrames != 1)
                    throw new InvalidOperationException("Shared session did not bound admission.");
                static void Reject(Action action)
                {
                    try { action(); }
                    catch (InvalidOperationException) { return; }
                    throw new InvalidOperationException("Expected session ownership rejection.");
                }
                Reject(() => session.BeginVulkanFrame(sessionTarget));
                Reject(sessionTarget.Dispose);
                Task.Run(() => Reject(() => session.BeginVulkanFrame(sessionTarget))).GetAwaiter().GetResult();
                cancelled.CancelRecording();
                if (session.OutstandingFrames != 0)
                    throw new InvalidOperationException("Cancelled session frame was retained.");
            }
            if (compositionLuid is not null)
                outputOwner = new(vk, instance, physicalDevice, device, family, compositionLuid, size);
            var layouts = new List<string>();
            for (var frame = 0; frame < 12; frame++)
            {
                var color = session is not null ? SKColors.Black : frame % 2 == 0 ? SKColors.Red : SKColors.Blue;
                if (session is not null)
                {
                    if (frame >= 4)
                    {
                        // Routine superseded paints must preserve already-submitted
                        // uploads and raster caches. Also discard one fresh upload.
                        var before = session.ImageCacheDiagnostics;
                        var promotions = imageScene!.Promotions;
                        var stale = session.BeginVulkanFrame(sessionTarget!);
                        var stalePaint = imageScene.Draw(stale.Surface);
                        stale.Surface.Canvas.DrawImage(testImage, new SKRect(0, 0, 8, 8),
                            new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));
                        using var freshPixels = new SKBitmap(4, 4);
                        freshPixels.Erase(SKColors.White);
                        using var fresh = SKImage.FromBitmap(freshPixels);
                        stale.Surface.Canvas.DrawImage(fresh, 0, 0, SKSamplingOptions.Default);
                        imageScene.Cancel(stalePaint);
                        stale.CancelRecording();
                        if (session.ImageCacheDiagnostics.Uploads != before.Uploads + 1 ||
                            session.ImageCacheDiagnostics.Discarded != before.Discarded + 1 ||
                            imageScene.Promotions != promotions)
                            throw new InvalidOperationException("Cancellation churned submitted image caches.");
                    }
                    sessionFrame = session.BeginVulkanFrame(sessionTarget!);
                    sessionSubmitted = false;
                }
                var output = sessionFrame?.Surface ?? surface!;
                var uploadsBeforePaint = session?.ImageCacheDiagnostics.Uploads;
                var promotionsBeforePaint = imageScene?.Promotions;
                output.Canvas.Clear(color);
                var scenePaint = imageScene?.Draw(output);
                if (sessionFrame is not null)
                {
                    // Exercise the same recorder registration/image-provider route
                    // used by scene captures, raster images and runtime shaders.
                    output.Canvas.DrawImage(testImage, new SKRect(0, 0, 8, 8),
                        new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));
                    using var offscreen = SkiaGpuSurfaces.CreateCompatible(output.Canvas, new(8, 8), null);
                    offscreen.Canvas.Clear(SKColors.Yellow);
                    using var captureImage = offscreen.Snapshot();
                    output.Canvas.DrawImage(captureImage, 8, 0, SKSamplingOptions.Default);
                    using var effect = SKRuntimeEffect.CreateShader(
                        "half4 main(float2 p) { return half4(0,1,1,1); }",
                        out var shaderError) ?? throw new InvalidOperationException(shaderError);
                    using var uniforms = new SKRuntimeEffectUniforms(effect);
                    using var children = new SKRuntimeEffectChildren(effect);
                    using var shader = effect.ToShader(uniforms, children);
                    using var shaderPaint = new SKPaint { Shader = shader };
                    output.Canvas.DrawRect(16, 0, 8, 8, shaderPaint);
                    if (frame >= 4 && (session!.ImageCacheDiagnostics.Uploads != uploadsBeforePaint ||
                        imageScene!.Promotions != promotionsBeforePaint))
                        throw new InvalidOperationException("Cancelled paint caused a submitted image to be uploaded or rasterized again.");
                }
                Task<SkiaGraphiteReadback>? readback = sessionFrame?.RequestReadback(new(size, size, SKColorType.Rgba8888, SKAlphaType.Premul));
                if (recorder is not null) recording = recorder.Snap() ?? throw new InvalidOperationException("External Snap failed.");
                var signalBefore = new SubmitInfo { SType = StructureType.SubmitInfo,
                    SignalSemaphoreCount = 1, PSignalSemaphores = &beforeGraphite };
                Check(vk.QueueSubmit(queue, 1, &signalBefore, default), "vkQueueSubmit(signal-before)");
                ulong waitHandle = beforeGraphite.Handle, signalHandle = afterGraphite.Handle;
                if (sessionFrame is not null)
                {
                    sessionSubmitted = true;
                    sessionFrame.SubmitVulkan([waitHandle], [signalHandle]);
                }
                else if (!GraphiteInterop.doroti_graphite_vk_insert_recording(context.Handle, recording!.Handle, 1, &waitHandle, 1, &signalHandle) ||
                    !context.Submit(new SKGraphiteSubmitInfo { Sync = false }))
                    throw new InvalidOperationException("External insert/submit failed.");
                if (failAfterSubmit)
                    throw new InvalidOperationException("Injected Graphite failure after asynchronous submit; diagnostic teardown must retain live resources until drain.");
                int layout;
                uint actualFamily;
                if (sessionTarget is not null) (layout, actualFamily) = sessionTarget.GetState();
                else if (!GraphiteInterop.doroti_graphite_vk_texture_get_state(backend!.Handle, out layout, out actualFamily))
                    throw new InvalidOperationException("Cannot query actual texture state.");
                if (actualFamily != family) throw new InvalidOperationException("Unexpected queue ownership.");
                layouts.Add(((ImageLayout)layout).ToString());
                Check(vk.ResetCommandBuffer(command, 0), "vkResetCommandBuffer");
                var begin = new CommandBufferBeginInfo { SType = StructureType.CommandBufferBeginInfo };
                Check(vk.BeginCommandBuffer(command, &begin), "vkBeginCommandBuffer");
                var barrier = new ImageMemoryBarrier { SType = StructureType.ImageMemoryBarrier,
                    SrcAccessMask = AccessFlags.MemoryWriteBit, DstAccessMask = AccessFlags.TransferReadBit,
                    OldLayout = (ImageLayout)layout, NewLayout = ImageLayout.TransferSrcOptimal,
                    SrcQueueFamilyIndex = Vk.QueueFamilyIgnored, DstQueueFamilyIndex = Vk.QueueFamilyIgnored,
                    Image = image, SubresourceRange = new(ImageAspectFlags.ColorBit, 0, 1, 0, 1) };
                vk.CmdPipelineBarrier(command, PipelineStageFlags.AllCommandsBit, PipelineStageFlags.TransferBit,
                    0, 0, null, 0, null, 1, &barrier);
                var copy = new BufferImageCopy { ImageSubresource = new(ImageAspectFlags.ColorBit, 0, 0, 1),
                    ImageExtent = new((uint)size, (uint)size, 1) };
                vk.CmdCopyImageToBuffer(command, image, ImageLayout.TransferSrcOptimal, buffer, 1, &copy);
                outputOwner?.RecordCopy(command, image, frame % CompositionBufferCount);
                barrier.SrcAccessMask = AccessFlags.TransferReadBit;
                barrier.DstAccessMask = AccessFlags.MemoryReadBit | AccessFlags.MemoryWriteBit;
                barrier.OldLayout = ImageLayout.TransferSrcOptimal;
                barrier.NewLayout = ImageLayout.General;
                vk.CmdPipelineBarrier(command, PipelineStageFlags.TransferBit, PipelineStageFlags.AllCommandsBit,
                    0, 0, null, 0, null, 1, &barrier);
                var hostBarrier = new MemoryBarrier { SType = StructureType.MemoryBarrier,
                    SrcAccessMask = AccessFlags.TransferWriteBit, DstAccessMask = AccessFlags.HostReadBit };
                vk.CmdPipelineBarrier(command, PipelineStageFlags.TransferBit, PipelineStageFlags.HostBit,
                    0, 1, &hostBarrier, 0, null, 0, null);
                Check(vk.EndCommandBuffer(command), "vkEndCommandBuffer");
                var waitStage = PipelineStageFlags.AllCommandsBit;
                var copySubmit = new SubmitInfo { SType = StructureType.SubmitInfo, CommandBufferCount = 1,
                    PCommandBuffers = &command, WaitSemaphoreCount = 1,
                    PWaitSemaphores = &afterGraphite, PWaitDstStageMask = &waitStage };
                Check(vk.QueueSubmit(queue, 1, &copySubmit, fence), "vkQueueSubmit(copy)");
                Check(vk.WaitForFences(device, 1, &fence, true, FenceTimeoutNanoseconds), "vkWaitForFences(copy)");
                if (sessionFrame is not null)
                {
                    sessionFrame.CompleteGpuWork();
                    imageScene!.Complete(scenePaint!.Value);
                    sessionFrame = null;
                    sessionTarget!.SetStateAfterGpuCompletion((int)ImageLayout.General, family);
                    if (!readback!.IsCompletedSuccessfully)
                        throw new InvalidOperationException("Shared session did not finish readback at copy completion.");
                    var capture = readback.Result;
                    if (capture.Pixels[0] != 0 || capture.Pixels[1] != 255 || capture.Pixels[2] != 0)
                        throw new InvalidOperationException("Shared session capture pixels differ from rendered content.");
                }
                else if (!GraphiteInterop.doroti_graphite_vk_texture_set_state(backend!.Handle, (int)ImageLayout.General, family))
                    throw new InvalidOperationException("Cannot publish externally transitioned state to Graphite.");
                void* pixels = null;
                Check(vk.MapMemory(device, bufferMemory, 0, bufferInfo.Size, 0, &pixels), "vkMapMemory");
                try
                {
                    var data = new ReadOnlySpan<byte>(pixels, checked((int)bufferInfo.Size));
                    for (var pixel = 0; pixel < size * size; pixel++)
                    {
                        var expected = session is not null && pixel / size < 8 && pixel % size < 24
                            ? pixel % size < 8 ? SKColors.Lime : pixel % size < 16 ? SKColors.Yellow : SKColors.Cyan
                            : color;
                        var samplePixel = imageScene is not null && pixel / size >= 16 && pixel / size < 48 && pixel % size < 32;
                        if (samplePixel)
                            expected = imageScene!.Reference.GetPixel(pixel % size, pixel / size);
                        var tolerance = samplePixel ? 2 : 0;
                        if (Math.Abs(data[pixel * 4] - (compositionLuid is null ? expected.Red : expected.Blue)) > tolerance || Math.Abs(data[pixel * 4 + 1] - expected.Green) > tolerance ||
                            Math.Abs(data[pixel * 4 + 2] - (compositionLuid is null ? expected.Blue : expected.Red)) > tolerance || data[pixel * 4 + 3] != 255)
                            throw new InvalidOperationException($"External copy pixel mismatch: frame {frame}, pixel {pixel}.");
                    }
                }
                finally { vk.UnmapMemory(device, bufferMemory); }
                outputOwner?.Present(frame % CompositionBufferCount);
                if (failAfterPresent)
                    throw new InvalidOperationException("Injected Graphite failure after Composition present; teardown must retire platform buffers independently of GPU completion.");
                context.CheckAsyncWorkCompletion();
                if (GraphiteInterop.doroti_graphite_has_unfinished_gpu_work(context.Handle))
                    throw new InvalidOperationException("Graphite completion remained pending after the copy fence.");
                Check(vk.ResetFences(device, 1, &fence), "vkResetFences");
                recording?.Dispose();
                recording = null;
            }
            return new { size, frames = 12, persistentWrapper = true, wrapperRecreationsPerFrame = 0,
                sharedSession = session is not null, cancelledBeforeSubmit = session is not null,
                sharedAsyncReadback = session is not null,
                recorderOffscreenRasterUploadRuntimeShader = session is not null,
                cancelledUploadAndSampleRasterCache = session is not null,
                sampleWebpCpuGpuPixelComparison = session is not null,
                repeatedCancellationPreservesSubmittedCaches = session is not null,
                imageUploads = session?.ImageCacheDiagnostics.Uploads,
                imageCacheHits = session?.ImageCacheDiagnostics.Hits,
                discardedImageUploads = session?.ImageCacheDiagnostics.Discarded,
                dwmBoundaryCompletions = outputOwner?.CompletedPresents ?? 0,
                outputCompletionMechanism = outputOwner is null ? "none" : "DwmFlush (native wait mode 1), not per-present display statistics",
                platformRetirement = outputOwner?.Retirement,
                actualLayouts = layouts, returnLayout = "General", pixelCheck = "PASS",
                waitSignalSemaphores = "PASS", gpuCompletion = "PASS", maximumInFlight = 1 };
        }
        finally
        {
            // Failure cleanup is a diagnostic-only drain, never a per-frame path.
            // The parent runner imposes a hard process-tree timeout on driver hangs.
            _ = vk.DeviceWaitIdle(device);
            // Keep wrappers and recording alive until the failure drain as well
            // as on success; using declarations inside try dispose before finally.
            context.CheckAsyncWorkCompletion();
            if (sessionFrame is not null)
            {
                if (sessionSubmitted) sessionFrame.CompleteGpuWork();
                else sessionFrame.CancelRecording();
            }
            sessionTarget?.Dispose();
            outputOwner?.Dispose();
            recording?.Dispose();
            surface?.Dispose();
            backend?.Dispose();
            recorder?.Dispose();
            if (fence.Handle != 0) vk.DestroyFence(device, fence, null);
            if (beforeGraphite.Handle != 0) vk.DestroySemaphore(device, beforeGraphite, null);
            if (afterGraphite.Handle != 0) vk.DestroySemaphore(device, afterGraphite, null);
            if (pool.Handle != 0) vk.DestroyCommandPool(device, pool, null);
            if (image.Handle != 0) vk.DestroyImage(device, image, null);
            if (buffer.Handle != 0) vk.DestroyBuffer(device, buffer, null);
            if (imageMemory.Handle != 0) vk.FreeMemory(device, imageMemory, null);
            if (bufferMemory.Handle != 0) vk.FreeMemory(device, bufferMemory, null);
        }
    }
}
