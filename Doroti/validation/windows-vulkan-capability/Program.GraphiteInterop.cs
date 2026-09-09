using System.Runtime.InteropServices;
using Silk.NET.Vulkan;
using SkiaSharp;
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
        Queue queue, uint family, SKGraphiteContext context, int size)
    {
        VkImage image = default;
        DeviceMemory imageMemory = default, bufferMemory = default;
        VkBuffer buffer = default;
        CommandPool pool = default;
        Fence fence = default;
        Semaphore beforeGraphite = default, afterGraphite = default;
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
                ImageType = ImageType.Type2D, Format = Format.R8G8B8A8Unorm, Extent = new((uint)size, (uint)size, 1),
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
            using var recorder = context.CreateRecorder(64L * 1024 * 1024)
                ?? throw new InvalidOperationException("External recorder failed.");
            using var backend = SKGraphiteBackendTexture.CreateVulkan(size, size,
                new SKGraphiteVkTextureInfo { SampleCount = 1, Format = (int)Format.R8G8B8A8Unorm,
                    ImageTiling = (int)ImageTiling.Optimal, ImageUsageFlags = (uint)usage,
                    SharingMode = (int)SharingMode.Exclusive, AspectMask = (uint)ImageAspectFlags.ColorBit },
                (int)ImageLayout.Undefined, family, (nint)image.Handle)
                ?? throw new InvalidOperationException("External texture wrapping failed.");
            using var surface = SKSurface.Create(recorder, backend, SKColorType.Rgba8888)
                ?? throw new InvalidOperationException("External texture surface failed.");
            var layouts = new List<string>();
            for (var frame = 0; frame < 12; frame++)
            {
                var color = frame % 2 == 0 ? SKColors.Red : SKColors.Blue;
                surface.Canvas.Clear(color);
                using var recording = recorder.Snap() ?? throw new InvalidOperationException("External Snap failed.");
                var signalBefore = new SubmitInfo { SType = StructureType.SubmitInfo,
                    SignalSemaphoreCount = 1, PSignalSemaphores = &beforeGraphite };
                Check(vk.QueueSubmit(queue, 1, &signalBefore, default), "vkQueueSubmit(signal-before)");
                ulong waitHandle = beforeGraphite.Handle, signalHandle = afterGraphite.Handle;
                if (!GraphiteInterop.doroti_graphite_vk_insert_recording(context.Handle, recording.Handle, 1, &waitHandle, 1, &signalHandle) ||
                    !context.Submit(new SKGraphiteSubmitInfo { Sync = false }))
                    throw new InvalidOperationException("External insert/submit failed.");
                if (!GraphiteInterop.doroti_graphite_vk_texture_get_state(backend.Handle, out var layout, out var actualFamily))
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
                if (!GraphiteInterop.doroti_graphite_vk_texture_set_state(backend.Handle, (int)ImageLayout.General, family))
                    throw new InvalidOperationException("Cannot publish externally transitioned state to Graphite.");
                void* pixels = null;
                Check(vk.MapMemory(device, bufferMemory, 0, bufferInfo.Size, 0, &pixels), "vkMapMemory");
                try
                {
                    var data = new ReadOnlySpan<byte>(pixels, checked((int)bufferInfo.Size));
                    for (var pixel = 0; pixel < size * size; pixel++)
                        if (data[pixel * 4] != color.Red || data[pixel * 4 + 1] != color.Green ||
                            data[pixel * 4 + 2] != color.Blue || data[pixel * 4 + 3] != 255)
                            throw new InvalidOperationException($"External copy pixel mismatch: frame {frame}, pixel {pixel}.");
                }
                finally { vk.UnmapMemory(device, bufferMemory); }
                context.CheckAsyncWorkCompletion();
                if (GraphiteInterop.doroti_graphite_has_unfinished_gpu_work(context.Handle))
                    throw new InvalidOperationException("Graphite completion remained pending after the copy fence.");
                Check(vk.ResetFences(device, 1, &fence), "vkResetFences");
            }
            return new { size, frames = 12, persistentWrapper = true, wrapperRecreationsPerFrame = 0,
                actualLayouts = layouts, returnLayout = "General", pixelCheck = "PASS",
                waitSignalSemaphores = "PASS", gpuCompletion = "PASS", maximumInFlight = 1 };
        }
        finally
        {
            // Failure cleanup is a diagnostic-only drain, never a per-frame path.
            // The parent runner imposes a hard process-tree timeout on driver hangs.
            _ = vk.DeviceWaitIdle(device);
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
