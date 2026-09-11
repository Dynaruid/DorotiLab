using System.Diagnostics;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;
using SkiaSharp;
using VkDevice = Silk.NET.Vulkan.Device;
using VkImage = Silk.NET.Vulkan.Image;
using VkBuffer = Silk.NET.Vulkan.Buffer;

internal static unsafe partial class Program
{
    private sealed class CopySlot
    {
        public VkImage Render, Output;
        public DeviceMemory RenderMemory, OutputMemory, ReadMemory;
        public VkBuffer ReadBuffer;
        public nint Mapped;
        public CommandBuffer Command;
        public Fence Fence;
        public SKGraphiteBackendTexture? Backend;
        public SKSurface? Surface;
        public SKGraphiteRecording? Recording;
        public bool Pending, ReadRequested, ReadCompleted;
        public byte[]? ReadPixels;
        public byte[]? BaselinePixels;
        public Exception? ReadError;
        public int Frame, Uses, Index;
        public ImageLayout RestoreLayout;
        public long SubmittedAt;
    }
    private static void ProbeCopy(Vk vk, PhysicalDevice physical, VkDevice device, Queue queue, uint family, SKGraphiteContext context, VulkanObserver observer)
    {
        const uint size = 128;
        const int bytes = (int)(size * size * 4);
        int slotCount = Mode == "copy-one" ? 1 : 2;
        int frames = Mode == "copy-two" ? 1000 : 120;
        var slots = Enumerable.Range(0, slotCount).Select(i => new CopySlot { Index = i }).ToArray();
        var layouts = new HashSet<string>();
        var metadataSamples = new List<object>();
        var recorder = context.CreateRecorder(64L * 1024 * 1024,
            (owner, raster, mipmapped) => raster.ToTextureImage(owner, mipmapped)) ?? throw new InvalidOperationException("Recorder null.");
        CommandPool pool = default;
        int submitted = 0, completed = 0, maximumPending = 0, capacityRejections = 0, cancelled = 0, callbackChecks = 0;
        ulong allocatedBytes = 0;
        var timer = Stopwatch.StartNew();
        Report["unsupportedProfileTest"] = observer.ProbeUnsupportedImage();
        vk.GetPhysicalDeviceMemoryProperties(physical, out var memoryProperties);
        uint MemoryType(uint bits, MemoryPropertyFlags flags)
        {
            for (uint i = 0; i < memoryProperties.MemoryTypeCount; i++)
                if ((bits & (1u << (int)i)) != 0 && (memoryProperties.MemoryTypes[(int)i].PropertyFlags & flags) == flags) return i;
            throw new NotSupportedException("Required memory type missing: " + flags);
        }
        DeviceMemory Allocate(MemoryRequirements requirements, MemoryPropertyFlags flags)
        {
            var ai = new MemoryAllocateInfo { SType = StructureType.MemoryAllocateInfo, AllocationSize = requirements.Size, MemoryTypeIndex = MemoryType(requirements.MemoryTypeBits, flags) };
            Check(vk.AllocateMemory(device, &ai, null, out var memory), "copy allocate"); allocatedBytes += requirements.Size; return memory;
        }
        var createImage = observer.Call<VulkanObserver.CreateImageDelegate>("vkCreateImage");
        var destroyImage = observer.Call<VulkanObserver.DestroyImageDelegate>("vkDestroyImage");
        var allocateCommands = observer.Call<VulkanObserver.AllocateCommandBuffersDelegate>("vkAllocateCommandBuffers");
        var beginCommand = observer.Call<VulkanObserver.BeginCommandBufferDelegate>("vkBeginCommandBuffer");
        var endCommand = observer.Call<VulkanObserver.EndCommandBufferDelegate>("vkEndCommandBuffer");
        var resetCommand = observer.Call<VulkanObserver.ResetCommandBufferDelegate>("vkResetCommandBuffer");
        var barrier = observer.Call<VulkanObserver.CmdPipelineBarrierDelegate>("vkCmdPipelineBarrier");
        var submit = observer.Call<VulkanObserver.QueueSubmitDelegate>("vkQueueSubmit");
        var barrier2 = Mode == "copy-v2" ? observer.Call<VulkanObserver.CmdPipelineBarrier2Delegate>("vkCmdPipelineBarrier2KHR") : null;
        var submit2 = Mode == "copy-v2" ? observer.Call<VulkanObserver.QueueSubmit2Delegate>("vkQueueSubmit2KHR") : null;
        var destroyPool = observer.Call<VulkanObserver.DestroyCommandPoolDelegate>("vkDestroyCommandPool");
        void Image(CopySlot slot, bool render)
        {
            var ci = new ImageCreateInfo { SType = StructureType.ImageCreateInfo, ImageType = ImageType.Type2D,
                Format = Format.R8G8B8A8Unorm, Extent = new(size, size, 1), MipLevels = 1, ArrayLayers = 1, Samples = SampleCountFlags.Count1Bit,
                Tiling = ImageTiling.Optimal, SharingMode = SharingMode.Exclusive,
                Usage = ImageUsageFlags.TransferSrcBit | ImageUsageFlags.TransferDstBit | (render || Mode == "copy-v2" ? ImageUsageFlags.ColorAttachmentBit | ImageUsageFlags.SampledBit | ImageUsageFlags.InputAttachmentBit : 0) };
            VkImage image;
            Check(createImage(device, &ci, null, &image), "copy create image");
            if (render) slot.Render = image; else slot.Output = image;
            vk.GetImageMemoryRequirements(device, image, out var requirements);
            var memory = Allocate(requirements, MemoryPropertyFlags.DeviceLocalBit);
            if (render) slot.RenderMemory = memory; else slot.OutputMemory = memory;
            Check(vk.BindImageMemory(device, image, memory, 0), "copy bind image");
            observer.RegisterTarget(image.Handle);
            if (render)
            {
                slot.Backend = SKGraphiteBackendTexture.CreateVulkan((int)size, (int)size, new SKGraphiteVkTextureInfo {
                    Format = (int)ci.Format, SampleCount = 1, ImageUsageFlags = (uint)ci.Usage, AspectMask = (uint)ImageAspectFlags.ColorBit },
                    (int)ImageLayout.Undefined, family, (nint)image.Handle) ?? throw new InvalidOperationException("Persistent texture wrap failed.");
                slot.Surface = SKSurface.Create(recorder, slot.Backend, SKColorType.Rgba8888) ?? throw new InvalidOperationException("Persistent surface failed.");
            }
        }
        void Transition(CopySlot slot, VkImage image, ImageLayout oldLayout, ImageLayout newLayout,
            AccessFlags source, AccessFlags destination, PipelineStageFlags sourceStage, PipelineStageFlags destinationStage)
        {
            var b = new ImageMemoryBarrier { SType = StructureType.ImageMemoryBarrier, Image = image, OldLayout = oldLayout, NewLayout = newLayout,
                SrcAccessMask = source, DstAccessMask = destination, SrcQueueFamilyIndex = Vk.QueueFamilyIgnored, DstQueueFamilyIndex = Vk.QueueFamilyIgnored,
                SubresourceRange = new(ImageAspectFlags.ColorBit, 0, 1, 0, 1) };
            if (barrier2 == null) barrier(slot.Command, sourceStage, destinationStage, 0, 0, null, 0, null, 1, &b);
            else
            {
                var b2 = new ImageMemoryBarrier2 { SType = StructureType.ImageMemoryBarrier2, Image = image, OldLayout = oldLayout, NewLayout = newLayout,
                    SrcAccessMask = (AccessFlags2)source, DstAccessMask = (AccessFlags2)destination, SrcStageMask = (PipelineStageFlags2)sourceStage,
                    DstStageMask = (PipelineStageFlags2)destinationStage, SrcQueueFamilyIndex = uint.MaxValue, DstQueueFamilyIndex = uint.MaxValue,
                    SubresourceRange = b.SubresourceRange };
                var dependency = new DependencyInfo { SType = StructureType.DependencyInfo, ImageMemoryBarrierCount = 1, PImageMemoryBarriers = &b2 };
                barrier2(slot.Command, &dependency);
            }
        }
        bool Poll(CopySlot slot)
        {
            if (!slot.Pending) return true;
            var result = vk.GetFenceStatus(device, slot.Fence);
            if (result == Result.NotReady)
            {
                if (Stopwatch.GetElapsedTime(slot.SubmittedAt).TotalSeconds > 5) throw new TimeoutException("Copy host fence exceeded five seconds.");
                return false;
            }
            Check(result, "copy poll fence"); context.CheckAsyncWorkCompletion(); observer.Check();
            if (observer.State(slot.Render.Handle).Layout != slot.RestoreLayout) throw new InvalidOperationException("R scheduled state was not restored.");
            var pixels = new ReadOnlySpan<byte>((void*)slot.Mapped, bytes);
            void Pixel(int x, int y, byte r, byte g, byte b)
            {
                var p = (byte*)slot.Mapped + (y * size + x) * 4;
                if (p[0] != r || p[1] != g || p[2] != b || p[3] != 255)
                    throw new InvalidOperationException($"GPU-copy pixel slot={slot.Index} frame={slot.Frame} ({x},{y}) got {p[0]},{p[1]},{p[2]},{p[3]} expected {r},{g},{b},255.");
            }
            Pixel(8, 8, (byte)(slot.Frame % 2 == 0 ? 255 : 0), (byte)(slot.Frame % 2 == 0 ? 0 : 255), 0);
            var background = (byte)(slot.Index == 0 ? 255 : 0);
            Pixel(122, 2, background, background, background); // Cancelled recording must not paint this corner.
            Pixel(114, 114, 0, 0, 255); Pixel(85, 80, 255, 255, 0); // Preserved images/runtime shader after partial repaint.
            if (slot.BaselinePixels == null)
            {
                int textPixels = 0;
                for (int y = 43; y < 60; y++) for (int x = 5; x < 75; x++)
                { var p = (byte*)slot.Mapped + (y * size + x) * 4; if (p[0] > p[1] + 30 && p[2] > p[1] + 30) textPixels++; }
                if (textPixels < 40) throw new InvalidOperationException("Copied text missing.");
                slot.BaselinePixels = pixels.ToArray();
            }
            else
            {
                for (int y = 0; y < size; y++) for (int x = 0; x < size; x++)
                {
                    if (x < 16 && y < 16 || Mode == "copy-msaa" && x >= 32 && x <= 78 && y >= 62 && y <= 108) continue;
                    int offset = (int)((y * size + x) * 4);
                    if (!pixels.Slice(offset, 4).SequenceEqual(slot.BaselinePixels.AsSpan(offset, 4)))
                        throw new InvalidOperationException($"Partial repaint changed preserved pixel ({x},{y}), frame {slot.Frame}.");
                }
            }
            if (slot.ReadRequested)
            {
                if (!slot.ReadCompleted || slot.ReadError != null || slot.ReadPixels == null || slot.ReadPixels[3] != 255)
                    throw new InvalidOperationException("Graphite readback callback lifetime failed.", slot.ReadError);
                callbackChecks++;
            }
            if (slot.Frame >= frames - slotCount)
            {
                using var bitmap = new SKBitmap(new SKImageInfo((int)size, (int)size, SKColorType.Rgba8888, SKAlphaType.Premul));
                pixels.CopyTo(new Span<byte>((void*)bitmap.GetPixels(), bytes));
                using var png = bitmap.Encode(SKEncodedImageFormat.Png, 100);
                using var file = File.Create(Path.Combine(Path.GetDirectoryName(Output)!, $"copy-g{Report["generation"]}-slot{slot.Index}.png")); png.SaveTo(file);
            }
            slot.Recording!.Dispose(); slot.Recording = null; slot.Pending = false; completed++;
            return true;
        }
        try
        {
            var pci = new CommandPoolCreateInfo { SType = StructureType.CommandPoolCreateInfo, QueueFamilyIndex = family, Flags = CommandPoolCreateFlags.ResetCommandBufferBit };
            Check(vk.CreateCommandPool(device, &pci, null, out pool), "host copy pool");
            foreach (var slot in slots)
            {
                Image(slot, true); Image(slot, false);
                var bci = new BufferCreateInfo { SType = StructureType.BufferCreateInfo, Size = bytes, Usage = BufferUsageFlags.TransferDstBit, SharingMode = SharingMode.Exclusive };
                Check(vk.CreateBuffer(device, &bci, null, out slot.ReadBuffer), "diagnostic readback buffer");
                vk.GetBufferMemoryRequirements(device, slot.ReadBuffer, out var requirements);
                slot.ReadMemory = Allocate(requirements, MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit);
                Check(vk.BindBufferMemory(device, slot.ReadBuffer, slot.ReadMemory, 0), "readback bind");
                void* mapped; Check(vk.MapMemory(device, slot.ReadMemory, 0, bytes, 0, &mapped), "readback map"); slot.Mapped = (nint)mapped;
                var ai = new CommandBufferAllocateInfo { SType = StructureType.CommandBufferAllocateInfo, CommandPool = pool, CommandBufferCount = 1, Level = CommandBufferLevel.Primary };
                CommandBuffer cb; Check(allocateCommands(device, &ai, &cb), "host command"); slot.Command = cb;
                var fci = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
                Check(vk.CreateFence(device, &fci, null, out slot.Fence), "host copy fence");
            }
            observer.Check();
            if (Mode == "copy-v2") ProbeRenderPass2(vk, device, queue, observer, slots[0]);
            for (int frame = 0; frame < frames; frame++)
            {
                var slot = slots[frame % slotCount];
                if (slots.All(s => s.Pending)) capacityRejections++; // Reject admission before any new recording/submit.
                while (!Poll(slot)) Thread.Sleep(1);
                var canvas = slot.Surface!.Canvas;
                if (slot.Uses == 0) DrawCopyScene(canvas, slot.Index == 0 ? SKColors.White : SKColors.Black);
                else if (frame % 37 == 0)
                {
                    var before = observer.State(slot.Render.Handle);
                    using var paint = new SKPaint { Color = SKColors.Magenta }; canvas.DrawRect(120, 0, 8, 8, paint);
                    using var cancelledRecording = recorder.Snap() ?? throw new InvalidOperationException("Cancelled snap null.");
                    if (observer.State(slot.Render.Handle) != before) throw new InvalidOperationException("Unsubmitted recording changed scheduled state.");
                    cancelled++;
                }
                using (var paint = new SKPaint { Color = frame % 2 == 0 ? SKColors.Red : SKColors.Lime }) canvas.DrawRect(0, 0, 16, 16, paint);
                if (Mode == "copy-msaa") DrawCopyPath(canvas);
                slot.Recording = recorder.Snap() ?? throw new InvalidOperationException("Copy Snap null.");
                if (context.InsertRecording(slot.Recording) != SKGraphiteInsertStatus.Success) throw new InvalidOperationException("Copy Insert failed.");
                slot.ReadRequested = frame % 29 == 0; slot.ReadCompleted = false; slot.ReadPixels = null; slot.ReadError = null;
                if (slot.ReadRequested)
                    context.RequestReadPixels(slot.Surface, new SKImageInfo((int)size, (int)size, SKColorType.Rgba8888, SKAlphaType.Premul), new(0, 0, (int)size, (int)size), SKImageRescaleGamma.Src, SKImageRescaleMode.Nearest,
                        r => { try { slot.ReadPixels = r?.ToArray(0); } catch (Exception e) { slot.ReadError = e; } finally { slot.ReadCompleted = true; } });
                observer.Check();
                if (!context.Submit(new SKGraphiteSubmitInfo { Sync = false })) throw new InvalidOperationException("Copy Graphite Submit failed.");
                observer.Check();
                slot.RestoreLayout = observer.State(slot.Render.Handle).Layout; layouts.Add(slot.RestoreLayout.ToString());
                Check(resetCommand(slot.Command, 0), "reset copy command");
                var begin = new CommandBufferBeginInfo { SType = StructureType.CommandBufferBeginInfo, Flags = CommandBufferUsageFlags.OneTimeSubmitBit };
                Check(beginCommand(slot.Command, &begin), "begin copy command");
                Transition(slot, slot.Render, slot.RestoreLayout, ImageLayout.TransferSrcOptimal, AccessFlags.MemoryReadBit | AccessFlags.MemoryWriteBit, AccessFlags.TransferReadBit, PipelineStageFlags.AllCommandsBit, PipelineStageFlags.TransferBit);
                var outputLayout = observer.State(slot.Output.Handle).Layout;
                Transition(slot, slot.Output, outputLayout, ImageLayout.TransferDstOptimal, outputLayout == ImageLayout.Undefined ? 0 : AccessFlags.MemoryReadBit | AccessFlags.MemoryWriteBit,
                    AccessFlags.TransferWriteBit, PipelineStageFlags.AllCommandsBit, PipelineStageFlags.TransferBit);
                var region = new ImageCopy { SrcSubresource = new(ImageAspectFlags.ColorBit, 0, 0, 1), DstSubresource = new(ImageAspectFlags.ColorBit, 0, 0, 1), Extent = new(size, size, 1) };
                vk.CmdCopyImage(slot.Command, slot.Render, ImageLayout.TransferSrcOptimal, slot.Output, ImageLayout.TransferDstOptimal, 1, &region);
                Transition(slot, slot.Render, ImageLayout.TransferSrcOptimal, slot.RestoreLayout, AccessFlags.TransferReadBit, AccessFlags.MemoryReadBit | AccessFlags.MemoryWriteBit, PipelineStageFlags.TransferBit, PipelineStageFlags.AllCommandsBit);
                Transition(slot, slot.Output, ImageLayout.TransferDstOptimal, ImageLayout.TransferSrcOptimal, AccessFlags.TransferWriteBit, AccessFlags.TransferReadBit, PipelineStageFlags.TransferBit, PipelineStageFlags.TransferBit);
                var copy = new BufferImageCopy { ImageSubresource = new(ImageAspectFlags.ColorBit, 0, 0, 1), ImageExtent = new(size, size, 1) };
                vk.CmdCopyImageToBuffer(slot.Command, slot.Output, ImageLayout.TransferSrcOptimal, slot.ReadBuffer, 1, &copy);
                var hostRead = new BufferMemoryBarrier { SType = StructureType.BufferMemoryBarrier, Buffer = slot.ReadBuffer, Size = bytes,
                    SrcAccessMask = AccessFlags.TransferWriteBit, DstAccessMask = AccessFlags.HostReadBit, SrcQueueFamilyIndex = uint.MaxValue, DstQueueFamilyIndex = uint.MaxValue };
                if (barrier2 == null) barrier(slot.Command, PipelineStageFlags.TransferBit, PipelineStageFlags.HostBit, 0, 0, null, 1, &hostRead, 0, null);
                else
                {
                    var b2 = new BufferMemoryBarrier2 { SType = StructureType.BufferMemoryBarrier2, Buffer = slot.ReadBuffer, Size = bytes,
                        SrcAccessMask = (AccessFlags2)AccessFlags.TransferWriteBit, DstAccessMask = (AccessFlags2)AccessFlags.HostReadBit,
                        SrcStageMask = (PipelineStageFlags2)PipelineStageFlags.TransferBit, DstStageMask = (PipelineStageFlags2)PipelineStageFlags.HostBit,
                        SrcQueueFamilyIndex = uint.MaxValue, DstQueueFamilyIndex = uint.MaxValue };
                    var dependency = new DependencyInfo { SType = StructureType.DependencyInfo, BufferMemoryBarrierCount = 1, PBufferMemoryBarriers = &b2 };
                    barrier2(slot.Command, &dependency);
                }
                Check(endCommand(slot.Command), "end copy command"); observer.Check();
                Check(vk.ResetFences(device, 1, in slot.Fence), "reset copy fence");
                var command = slot.Command;
                var si = new SubmitInfo { SType = StructureType.SubmitInfo, CommandBufferCount = 1, PCommandBuffers = &command };
                if (submit2 == null) Check(submit(queue, 1, &si, slot.Fence), "submit host copy");
                else
                {
                    var cbi = new CommandBufferSubmitInfo { SType = StructureType.CommandBufferSubmitInfo, CommandBuffer = command, DeviceMask = 1 };
                    var si2 = new SubmitInfo2 { SType = StructureType.SubmitInfo2, CommandBufferInfoCount = 1, PCommandBufferInfos = &cbi };
                    Check(submit2(queue, 1, &si2, slot.Fence), "submit2 host copy");
                }
                slot.Pending = true; slot.SubmittedAt = Stopwatch.GetTimestamp(); slot.Frame = frame; slot.Uses++; submitted++;
                observer.Check(); maximumPending = Math.Max(maximumPending, slots.Count(s => s.Pending));
                if (frame % 100 == 99) metadataSamples.Add(new { frame = frame + 1, observer.MetadataCount });
            }
            foreach (var slot in slots) while (!Poll(slot)) Thread.Sleep(1);
            if (submitted != completed || maximumPending != slotCount) throw new InvalidOperationException("Bounded slot completion imbalance.");
            if (Mode == "copy-msaa" && (observer.ResolveReferences == 0 || observer.NextSubpasses == 0))
                throw new InvalidOperationException("Requested MSAA did not exercise resolve and load subpasses.");
            Report["copyResult"] = new { frames, slotCount, submitted, completed, maximumPending, capacityRejections, cancelled, callbackChecks,
                persistentWraps = slotCount, layouts = layouts.Order().ToArray(), allocatedHostBytes = allocatedBytes, metadataSamples,
                elapsedMilliseconds = timer.Elapsed.TotalMilliseconds, readbackBoundary = "diagnostic-only P-to-buffer; no product presentation", performance = "notQualified", pixelChecks = "PASS" };
        }
        finally
        {
            if (slots.Any(s => s.Pending) || submitted != completed) { Check(vk.DeviceWaitIdle(device), "copy exceptional diagnostic drain"); context.CheckAsyncWorkCompletion(); }
            foreach (var slot in slots) { slot.Recording?.Dispose(); slot.Surface?.Dispose(); slot.Backend?.Dispose(); }
            recorder.Dispose(); context.Dispose(); // Keep image allocations until all Skia users are gone.
            foreach (var slot in slots)
            {
                if (slot.Mapped != 0) vk.UnmapMemory(device, slot.ReadMemory);
                if (slot.ReadBuffer.Handle != 0) vk.DestroyBuffer(device, slot.ReadBuffer, null);
                if (slot.Render.Handle != 0) destroyImage(device, slot.Render, null);
                if (slot.Output.Handle != 0) destroyImage(device, slot.Output, null);
                if (slot.ReadMemory.Handle != 0) vk.FreeMemory(device, slot.ReadMemory, null);
                if (slot.RenderMemory.Handle != 0) vk.FreeMemory(device, slot.RenderMemory, null);
                if (slot.OutputMemory.Handle != 0) vk.FreeMemory(device, slot.OutputMemory, null);
                if (slot.Fence.Handle != 0) vk.DestroyFence(device, slot.Fence, null);
            }
            if (pool.Handle != 0) destroyPool(device, pool, null);
            Report["observerAfterContext"] = observer.Summary();
        }
    }
    private static void DrawCopyScene(SKCanvas canvas, SKColor background)
    {
        canvas.Clear(background);
        using var gradient = SKShader.CreateLinearGradient(new(20, 20), new(100, 20), [SKColors.Red, SKColors.Blue], null, SKShaderTileMode.Clamp);
        using var gp = new SKPaint { Shader = gradient }; canvas.DrawRect(20, 20, 80, 12, gp);
        using var font = new SKFont(SKTypeface.Default, 16); using var text = new SKPaint { Color = SKColors.Magenta, IsAntialias = true };
        canvas.DrawText("Graphite", 5, 60, SKTextAlign.Left, font, text);
        using var bitmap = new SKBitmap(8, 8); bitmap.Erase(SKColors.Blue); using var image = SKImage.FromBitmap(bitmap);
        canvas.DrawImage(image, 112, 112, SKSamplingOptions.Default);
        using var effect = SKRuntimeEffect.CreateShader("half4 main(float2 p) { return half4(1, 1, 0, 1); }", out var error) ?? throw new InvalidOperationException(error);
        using var shader = effect.ToShader(); using var ep = new SKPaint { Shader = shader }; canvas.DrawRect(80, 75, 15, 15, ep);
        using var blur = SKImageFilter.CreateBlur(2, 2); using var bp = new SKPaint { Color = SKColors.Cyan, ImageFilter = blur }; canvas.DrawCircle(100, 50, 8, bp);
        DrawCopyPath(canvas);
        canvas.Save(); canvas.ClipRect(new(20, 70, 60, 100)); using var alpha = new SKPaint { Color = new(255, 0, 0, 128) }; canvas.DrawRect(10, 65, 60, 45, alpha); canvas.Restore();
    }
    private static void DrawCopyPath(SKCanvas canvas)
    {
        using var builder = new SKPathBuilder(); builder.MoveTo(75, 105); builder.CubicTo(35, 105, 65, 65, 75, 105); builder.Close(); using var path = builder.Detach();
        using var pathPaint = new SKPaint { Color = SKColors.Orange, IsAntialias = true }; canvas.DrawPath(path, pathPaint);
    }
}
