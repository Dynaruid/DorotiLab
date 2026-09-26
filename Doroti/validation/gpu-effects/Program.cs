using System.Runtime.InteropServices;
using Doroti.Skia.Rendering;
using Doroti.Skia.Vulkan;
using Silk.NET.Vulkan;
using SkiaSharp;

unsafe class Program
{
    static void Check(Result result)
    {
        if (result != Result.Success) throw new Exception(result.ToString());
    }

    static void Main(string[] args)
    {
        GraphiteNativeLibrary.Configure(GraphiteNativeLibrary.GetPackagedAsset());
        using var vk = Vk.GetApi();
        var app = new ApplicationInfo { SType = StructureType.ApplicationInfo, ApiVersion = Vk.Version12 };
        var create = new InstanceCreateInfo { SType = StructureType.InstanceCreateInfo, PApplicationInfo = &app };
        Check(vk.CreateInstance(&create, null, out var instance));
        uint count = 0;
        Check(vk.EnumeratePhysicalDevices(instance, &count, null));
        var physicals = new PhysicalDevice[count];
        fixed (PhysicalDevice* p = physicals) Check(vk.EnumeratePhysicalDevices(instance, &count, p));
        var physical = physicals[0];
        vk.GetPhysicalDeviceProperties(physical, out var properties);
        Console.WriteLine($"GPU={Marshal.PtrToStringUTF8((nint)properties.DeviceName)} api={properties.ApiVersion}");
        if (properties.DeviceType == PhysicalDeviceType.Cpu) throw new Exception("Hardware GPU required.");
        uint families = 0;
        vk.GetPhysicalDeviceQueueFamilyProperties(physical, &families, null);
        var queueProperties = new QueueFamilyProperties[families];
        fixed (QueueFamilyProperties* p = queueProperties) vk.GetPhysicalDeviceQueueFamilyProperties(physical, &families, p);
        var family = (uint)Array.FindIndex(queueProperties, p => (p.QueueFlags & QueueFlags.GraphicsBit) != 0);
        var priority = 1f;
        var queueInfo = new DeviceQueueCreateInfo { SType = StructureType.DeviceQueueCreateInfo, QueueFamilyIndex = family, QueueCount = 1, PQueuePriorities = &priority };
        var deviceInfo = new DeviceCreateInfo { SType = StructureType.DeviceCreateInfo, QueueCreateInfoCount = 1, PQueueCreateInfos = &queueInfo };
        Check(vk.CreateDevice(physical, &deviceInfo, null, out var device));
        vk.GetDeviceQueue(device, family, 0, out var queue);
        using (var observer = new VulkanObserver(vk, instance, device, queue, family))
        using (var session = SkiaGraphiteSession.CreateVulkan(new(instance.Handle, physical.Handle,
            device.Handle, queue.Handle, family, Vk.Version11, observer.Resolve,
            image => { var s = observer.State((ulong)image); return ((int)s.Layout, s.Family); }, observer.Check), 1))
        {
            const int width = 64, height = 48;
            var imageInfo = new ImageCreateInfo
            {
                SType = StructureType.ImageCreateInfo, ImageType = ImageType.Type2D, Format = Format.R8G8B8A8Unorm,
                Extent = new(width, height, 1), MipLevels = 1, ArrayLayers = 1, Samples = SampleCountFlags.Count1Bit,
                Tiling = ImageTiling.Optimal, Usage = ImageUsageFlags.ColorAttachmentBit | ImageUsageFlags.InputAttachmentBit |
                    ImageUsageFlags.SampledBit | ImageUsageFlags.TransferSrcBit | ImageUsageFlags.TransferDstBit,
            };
            Check(vk.CreateImage(device, &imageInfo, null, out var image));
            vk.GetImageMemoryRequirements(device, image, out var requirements);
            vk.GetPhysicalDeviceMemoryProperties(physical, out var memoryProperties);
            uint type = 0;
            while ((requirements.MemoryTypeBits & (1u << (int)type)) == 0 ||
                (memoryProperties.MemoryTypes[(int)type].PropertyFlags & MemoryPropertyFlags.DeviceLocalBit) == 0) type++;
            var allocation = new MemoryAllocateInfo { SType = StructureType.MemoryAllocateInfo, AllocationSize = requirements.Size, MemoryTypeIndex = type };
            Check(vk.AllocateMemory(device, &allocation, null, out var memory));
            Check(vk.BindImageMemory(device, image, memory, 0));
            observer.RegisterHostTarget(image.Handle, imageInfo);
            using (var target = session.CreateVulkanTarget(width, height,
                new SKGraphiteVkTextureInfo { SampleCount = 1, Format = (int)imageInfo.Format, ImageUsageFlags = (uint)imageInfo.Usage, AspectMask = (uint)ImageAspectFlags.ColorBit },
                (int)ImageLayout.Undefined, family, (nint)image.Handle, SKColorType.Rgba8888))
            {
                using var effects = new VulkanGpuEffect(vk, physical, device, queue, family, observer, session);
                var vs = File.ReadAllBytes(args.Length == 0 ? Path.Combine(AppContext.BaseDirectory, "shaders/fullscreen.vert.spv") : Path.Combine(args[0], "vertex.spv"));
                var fs = File.ReadAllBytes(args.Length == 0 ? Path.Combine(AppContext.BaseDirectory, "shaders/swap.frag.spv") : Path.Combine(args[0], "fragment.spv"));
                var withUniform = args.Contains("--uniform");
                var withFrame = args.Contains("--frame-time");
                var scaled = withUniform || withFrame;
                var uniform = withUniform ? new byte[16] : [];
                if (withUniform)
                    for (var channel = 0; channel < 4; channel++)
                        System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(uniform.AsSpan(channel * 4), channel == 0 ? .5f : 1);
                for (int iteration = 0; iteration < 6; iteration++)
                {
                    var frame = session.BeginVulkanFrame(target);
                    var canvas = frame.Surface.Canvas;
                    canvas.Clear(SKColors.Transparent);
                    void Pattern(SKCanvas child)
                    {
                        child.Clear(SKColors.Transparent);
                        using var paint = new SKPaint();
                        for (var y = 0; y < height; y++)
                        for (var x = 0; x < width; x++)
                        {
                            var alpha = iteration >= 2 && x >= width / 2 ? (byte)128 : (byte)255;
                            paint.Color = new SKColor((byte)(x * 3), (byte)(y * 4), (byte)(x + y), alpha);
                            child.DrawRect(x, y, 1, 1, paint);
                        }
                    }
                    var nested = iteration >= 4 && !scaled;
                    effects.Draw(canvas, width, height, child =>
                    {
                        if (nested)
                        {
                            child.Clear(SKColors.Transparent);
                            effects.Draw(child, width, height, Pattern, vs, fs, args.Length == 0 ? "main" : "fs_main");
                        }
                        else Pattern(child);
                    }, vs, fs, args.Length == 0 ? "main" : "fs_main", uniform,
                        time: withFrame ? .5f : 0, deltaTime: withFrame ? .25f : 0,
                        logicalWidth: width / 2f, logicalHeight: height / 2f);
                    if (frame.SubmittedSegments != (nested ? 2 : 1) || session.OutstandingFrames != 1 || session.CanBeginFrame)
                        throw new Exception("Segment ended logical frame or changed admission.");
                    if (iteration % 2 == 1)
                    {
                        frame.CancelRecording();
                    }
                    else
                    {
                        // Readback belongs only to this independent diagnostic oracle.
                        var pixels = frame.RequestReadback(new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul));
                        frame.Submit();
                        var fenceInfo = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
                        Check(vk.CreateFence(device, &fenceInfo, null, out var fence));
                        var submit = new SubmitInfo { SType = StructureType.SubmitInfo };
                        Check(vk.QueueSubmit(queue, 1, &submit, fence));
                        Check(vk.WaitForFences(device, 1, &fence, true, 5_000_000_000));
                        frame.CompleteGpuWork();
                        vk.DestroyFence(device, fence, null);
                        var result = pixels.GetAwaiter().GetResult();
                        for (var y = 0; y < height; y++)
                        for (var x = 0; x < width; x++)
                        {
                            var p = y * result.RowBytes + x * 4;
                            var alpha = iteration >= 2 && x >= width / 2 ? 128 : 255;
                            int Premul(int channel) => (channel * alpha + 127) / 255;
                            var expectedRed = Premul(nested ? x * 3 : x + y) * (scaled ? .5 : 1);
                            if (Math.Abs(result.Pixels[p] - expectedRed) > (scaled ? 1 : 0) || result.Pixels[p + 1] != Premul(y * 4) ||
                                result.Pixels[p + 2] != Premul(nested ? x + y : x * 3) || result.Pixels[p + 3] != alpha)
                                throw new Exception($"Pixel mismatch ({x},{y}): {string.Join(',', result.Pixels.AsSpan(p, 4).ToArray())}");
                        }
                    }
                    if (effects.LiveBytes != 0 || session.OutstandingFrames != 0 || !session.CanBeginFrame)
                        throw new Exception("GPU resources did not retire.");
                    observer.Check();
                }
                if (effects.PipelineCreations != 1 || effects.PipelineHits != effects.Draws - 1)
                    throw new Exception("Effect pipeline cache did not reuse the compiled pipeline.");
                Console.WriteLine($"PASS: pixelTolerance={(scaled ? 1 : 0)}; draws={effects.Draws}; pipelines={effects.PipelineCreations}; pipelineHits={effects.PipelineHits}; cancelled={effects.CancellationFences}; liveBytes={effects.LiveBytes}; peakBytes={effects.PeakBytes}; diagnosticReadbacks=3");
            }
            observer.ForgetHostTarget(image.Handle);
            vk.DestroyImage(device, image, null);
            vk.FreeMemory(device, memory, null);
        }
        vk.DestroyDevice(device, null);
        vk.DestroyInstance(instance, null);
    }
}
