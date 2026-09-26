using Doroti.Skia.Rendering;
using Silk.NET.Vulkan;
using SkiaSharp;
using VkImage = Silk.NET.Vulkan.Image;

namespace Doroti.Skia.Vulkan;

// All resources belong to the existing host device and queue. This executor
// deliberately owns its input allocation; Skia saveLayer textures are private.
internal sealed unsafe class VulkanGpuEffect(
    Vk vk, PhysicalDevice physical, Device device, Queue queue, uint family,
    VulkanObserver observer, SkiaGraphiteSession session) : ISkiaGpuEffectBackend, IDisposable
{
    private Fence _cancellationFence;
    private readonly Dictionary<string, CachedPipeline> _pipelines = [];
    internal long PipelineCreations { get; private set; }
    internal long PipelineHits { get; private set; }
    public void Draw(SKCanvas destination, int width, int height, Action<SKCanvas> capture,
        Doroti.Ui.GpuEffectProgram program, Doroti.Ui.GpuEffectParameters parameters, float logicalWidth = 0, float logicalHeight = 0)
    {
        var variant = program.GetVariant("vulkan-fragment");
        if (parameters.Bytes.Length != program.ParameterByteCount)
            throw new ArgumentException($"Effect '{program.AssetId}' requires {program.ParameterByteCount} uniform bytes, received {parameters.Bytes.Length}.");
        Draw(destination, width, height, capture, variant.Vertex, variant.Fragment, variant.EntryPoint, parameters.Bytes, parameters.Time, parameters.DeltaTime, logicalWidth, logicalHeight);
    }
    internal long Draws { get; private set; }
    internal long LiveBytes { get; private set; }
    internal long PeakBytes { get; private set; }
    internal long CancellationFences { get; private set; }
    private const long Budget = 128L * 1024 * 1024;

    internal void Draw(SKCanvas destination, int width, int height, Action<SKCanvas> capture,
        ReadOnlySpan<byte> vertex, ReadOnlySpan<byte> fragment, string entryPoint = "main",
        ReadOnlySpan<byte> parameters = default, float time = 0, float deltaTime = 0, float logicalWidth = 0, float logicalHeight = 0)
    {
        if (width < 1 || height < 1 || checked((long)width * height * 8) > Budget - LiveBytes)
            throw new NotSupportedException("GPU effect exceeds the texture memory budget.");
        var frame = session.RecordingFrame;
        var resources = new Resources(this);
        frame.RetainUntilGpuCompletion(resources);
        resources.Input = Allocate(width, height, resources);
        resources.Output = Allocate(width, height, resources);
        resources.InputTarget = session.CreateVulkanTarget(width, height, TextureInfo,
            (int)ImageLayout.Undefined, family, (nint)resources.Input.Handle, SKColorType.Rgba8888);
        capture(resources.InputTarget.Canvas);
        CreatePipeline(resources, width, height, vertex, fragment, entryPoint, parameters, time, deltaTime, logicalWidth, logicalHeight);
        frame.SubmitSegment(CompleteCancelledSegments);
        var inputState = resources.InputTarget.GetState();
        RecordAndSubmit(resources, width, height, (ImageLayout)inputState.Layout);
        resources.OutputImage = session.WrapVulkanImage(width, height, TextureInfo,
            (int)ImageLayout.ShaderReadOnlyOptimal, family, (nint)resources.Output.Handle);
        destination.DrawImage(resources.OutputImage.Image, 0, 0, new SKSamplingOptions(SKFilterMode.Nearest));
        Draws++;
    }

    private static readonly ImageUsageFlags Usage = ImageUsageFlags.ColorAttachmentBit |
        ImageUsageFlags.InputAttachmentBit | ImageUsageFlags.SampledBit |
        ImageUsageFlags.TransferSrcBit | ImageUsageFlags.TransferDstBit;
    private static SKGraphiteVkTextureInfo TextureInfo => new()
    {
        SampleCount = 1, Format = (int)Format.R8G8B8A8Unorm,
        ImageTiling = (int)ImageTiling.Optimal, ImageUsageFlags = (uint)Usage,
        AspectMask = (uint)ImageAspectFlags.ColorBit,
    };

    private VkImage Allocate(int width, int height, Resources resources)
    {
        var create = new ImageCreateInfo
        {
            SType = StructureType.ImageCreateInfo, ImageType = ImageType.Type2D,
            Format = Format.R8G8B8A8Unorm, Extent = new((uint)width, (uint)height, 1),
            MipLevels = 1, ArrayLayers = 1, Samples = SampleCountFlags.Count1Bit,
            Tiling = ImageTiling.Optimal, Usage = Usage, SharingMode = SharingMode.Exclusive,
        };
        Check(vk.CreateImage(device, &create, null, out var image));
        resources.Images.Add(image);
        vk.GetImageMemoryRequirements(device, image, out var requirements);
        if (requirements.Size > (ulong)(Budget - LiveBytes))
            throw new NotSupportedException("GPU effect allocation exceeds memory budget.");
        vk.GetPhysicalDeviceMemoryProperties(physical, out var properties);
        uint type = uint.MaxValue;
        for (uint i = 0; i < properties.MemoryTypeCount; i++)
            if ((requirements.MemoryTypeBits & (1u << (int)i)) != 0 &&
                (properties.MemoryTypes[(int)i].PropertyFlags & MemoryPropertyFlags.DeviceLocalBit) != 0)
            { type = i; break; }
        if (type == uint.MaxValue) throw new NotSupportedException("No device-local effect memory.");
        var allocate = new MemoryAllocateInfo
        {
            SType = StructureType.MemoryAllocateInfo, AllocationSize = requirements.Size,
            MemoryTypeIndex = type,
        };
        Check(vk.AllocateMemory(device, &allocate, null, out var memory));
        resources.Memory.Add(memory);
        resources.Bytes += (long)requirements.Size;
        LiveBytes += (long)requirements.Size;
        PeakBytes = Math.Max(PeakBytes, LiveBytes);
        Check(vk.BindImageMemory(device, image, memory, 0));
        observer.RegisterHostTarget(image.Handle, create);
        resources.Tracked.Add(image.Handle);
        return image;
    }

    private ImageView View(VkImage image, Resources r)
    {
        var info = new ImageViewCreateInfo
        {
            SType = StructureType.ImageViewCreateInfo, Image = image,
            ViewType = ImageViewType.Type2D, Format = Format.R8G8B8A8Unorm,
            SubresourceRange = new(ImageAspectFlags.ColorBit, 0, 1, 0, 1),
        };
        Check(vk.CreateImageView(device, &info, null, out var view));
        r.Views.Add(view);
        return view;
    }

    private ShaderModule Shader(ReadOnlySpan<byte> bytes, Resources r)
    {
        if (bytes.Length < 20 || bytes.Length % 4 != 0 ||
            System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bytes) != 0x07230203)
            throw new InvalidDataException("Invalid SPIR-V effect module.");
        fixed (byte* code = bytes)
        {
            var info = new ShaderModuleCreateInfo
            { SType = StructureType.ShaderModuleCreateInfo, CodeSize = (nuint)bytes.Length, PCode = (uint*)code };
            Check(vk.CreateShaderModule(device, &info, null, out var shader));
            r.Shaders.Add(shader);
            return shader;
        }
    }

    private void CreatePipeline(Resources r, int width, int height,
        ReadOnlySpan<byte> vertex, ReadOnlySpan<byte> fragment, string entryPoint, ReadOnlySpan<byte> parameters, float time, float deltaTime, float logicalWidth, float logicalHeight)
    {
        using var hash = System.Security.Cryptography.IncrementalHash.CreateHash(System.Security.Cryptography.HashAlgorithmName.SHA256);
        hash.AppendData(vertex); hash.AppendData(fragment);
        var pipelineKey = Convert.ToHexStringLower(hash.GetHashAndReset()) + $"/{entryPoint}/{width}/{height}";
        var inputView = View(r.Input, r);
        var outputView = View(r.Output, r);
        var samplerInfo = new SamplerCreateInfo
        {
            SType = StructureType.SamplerCreateInfo, MagFilter = Filter.Nearest, MinFilter = Filter.Nearest,
            AddressModeU = SamplerAddressMode.ClampToEdge, AddressModeV = SamplerAddressMode.ClampToEdge,
            AddressModeW = SamplerAddressMode.ClampToEdge, MaxLod = 0,
        };
        Check(vk.CreateSampler(device, &samplerInfo, null, out r.Sampler));
        DescriptorSetLayoutBinding* bindings = stackalloc DescriptorSetLayoutBinding[3];
        bindings[0] = new() { Binding = 0, DescriptorCount = 1, DescriptorType = DescriptorType.SampledImage, StageFlags = ShaderStageFlags.FragmentBit };
        bindings[1] = new() { Binding = 1, DescriptorCount = 1, DescriptorType = DescriptorType.Sampler, StageFlags = ShaderStageFlags.FragmentBit };
        bindings[2] = new() { Binding = 2, DescriptorCount = 1, DescriptorType = DescriptorType.UniformBuffer, StageFlags = ShaderStageFlags.FragmentBit };
        var layout = new DescriptorSetLayoutCreateInfo
        { SType = StructureType.DescriptorSetLayoutCreateInfo, BindingCount = 3, PBindings = bindings };
        Check(vk.CreateDescriptorSetLayout(device, &layout, null, out r.SetLayout));
        var parameterBinding = new DescriptorSetLayoutBinding(0, DescriptorType.UniformBuffer, 1, ShaderStageFlags.FragmentBit);
        var parameterLayout = new DescriptorSetLayoutCreateInfo
        { SType = StructureType.DescriptorSetLayoutCreateInfo, BindingCount = 1, PBindings = &parameterBinding };
        Check(vk.CreateDescriptorSetLayout(device, &parameterLayout, null, out r.ParameterLayout));
        DescriptorSetLayout* setLayouts = stackalloc DescriptorSetLayout[2] { r.SetLayout, r.ParameterLayout };
        var pipelineLayout = new PipelineLayoutCreateInfo
        { SType = StructureType.PipelineLayoutCreateInfo, SetLayoutCount = 2, PSetLayouts = setLayouts };
        Check(vk.CreatePipelineLayout(device, &pipelineLayout, null, out r.PipelineLayout));
        DescriptorPoolSize* sizes = stackalloc DescriptorPoolSize[3];
        sizes[0] = new(DescriptorType.SampledImage, 1);
        sizes[1] = new(DescriptorType.Sampler, 1);
        sizes[2] = new(DescriptorType.UniformBuffer, 2);
        var pool = new DescriptorPoolCreateInfo
        { SType = StructureType.DescriptorPoolCreateInfo, MaxSets = 2, PoolSizeCount = 3, PPoolSizes = sizes };
        Check(vk.CreateDescriptorPool(device, &pool, null, out r.DescriptorPool));
        var allocate = new DescriptorSetAllocateInfo
        { SType = StructureType.DescriptorSetAllocateInfo, DescriptorPool = r.DescriptorPool, DescriptorSetCount = 2, PSetLayouts = setLayouts };
        DescriptorSet* sets = stackalloc DescriptorSet[2];
        Check(vk.AllocateDescriptorSets(device, &allocate, sets));
        r.Set = sets[0];
        r.ParameterSet = sets[1];
        var inputInfo = new DescriptorImageInfo(default, inputView, ImageLayout.ShaderReadOnlyOptimal);
        var sampler = new DescriptorImageInfo(r.Sampler, default, ImageLayout.Undefined);
        Span<byte> frameBytes = stackalloc byte[32];
        frameBytes.Clear();
        System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(frameBytes, (uint)width);
        System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(frameBytes[4..], (uint)height);
        frameBytes[..8].CopyTo(frameBytes[8..]);
        System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(frameBytes[16..], logicalWidth > 0 ? logicalWidth : width);
        System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(frameBytes[20..], logicalHeight > 0 ? logicalHeight : height);
        System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(frameBytes[24..], time);
        System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(frameBytes[28..], deltaTime);
        var frameBuffer = Uniform(r, frameBytes);
        var userBuffer = Uniform(r, parameters);
        WriteDescriptorSet* writes = stackalloc WriteDescriptorSet[4];
        writes[0] = new() { SType = StructureType.WriteDescriptorSet, DstSet = r.Set, DstBinding = 0, DescriptorCount = 1, DescriptorType = DescriptorType.SampledImage, PImageInfo = &inputInfo };
        writes[1] = new() { SType = StructureType.WriteDescriptorSet, DstSet = r.Set, DstBinding = 1, DescriptorCount = 1, DescriptorType = DescriptorType.Sampler, PImageInfo = &sampler };
        writes[2] = new() { SType = StructureType.WriteDescriptorSet, DstSet = r.Set, DstBinding = 2, DescriptorCount = 1, DescriptorType = DescriptorType.UniformBuffer, PBufferInfo = &frameBuffer };
        writes[3] = new() { SType = StructureType.WriteDescriptorSet, DstSet = r.ParameterSet, DstBinding = 0, DescriptorCount = 1, DescriptorType = DescriptorType.UniformBuffer, PBufferInfo = &userBuffer };
        vk.UpdateDescriptorSets(device, 4, writes, 0, null);

        var attachment = new AttachmentDescription
        {
            Format = Format.R8G8B8A8Unorm, Samples = SampleCountFlags.Count1Bit,
            LoadOp = AttachmentLoadOp.DontCare, StoreOp = AttachmentStoreOp.Store,
            StencilLoadOp = AttachmentLoadOp.DontCare, StencilStoreOp = AttachmentStoreOp.DontCare,
            InitialLayout = ImageLayout.ColorAttachmentOptimal, FinalLayout = ImageLayout.ColorAttachmentOptimal,
        };
        var reference = new AttachmentReference(0, ImageLayout.ColorAttachmentOptimal);
        var subpass = new SubpassDescription
        { PipelineBindPoint = PipelineBindPoint.Graphics, ColorAttachmentCount = 1, PColorAttachments = &reference };
        var renderPass = new RenderPassCreateInfo
        { SType = StructureType.RenderPassCreateInfo, AttachmentCount = 1, PAttachments = &attachment, SubpassCount = 1, PSubpasses = &subpass };
        Check(vk.CreateRenderPass(device, &renderPass, null, out r.Pass));
        var framebuffer = new FramebufferCreateInfo
        { SType = StructureType.FramebufferCreateInfo, RenderPass = r.Pass, AttachmentCount = 1, PAttachments = &outputView, Width = (uint)width, Height = (uint)height, Layers = 1 };
        Check(vk.CreateFramebuffer(device, &framebuffer, null, out r.Framebuffer));
        if (_pipelines.TryGetValue(pipelineKey, out var cached))
        {
            r.CachedPipeline = cached.Retain();
            r.Pipeline = cached.Pipeline;
            PipelineHits++;
        }
        else
        {
        var vs = Shader(vertex, r);
        var fs = Shader(fragment, r);
        var entry = System.Text.Encoding.UTF8.GetBytes(entryPoint + "\0");
        var vertexEntry = "main\0"u8;
        fixed (byte* name = entry)
        fixed (byte* vertexName = vertexEntry)
        {
            PipelineShaderStageCreateInfo* stages = stackalloc PipelineShaderStageCreateInfo[2];
            stages[0] = new() { SType = StructureType.PipelineShaderStageCreateInfo, Stage = ShaderStageFlags.VertexBit, Module = vs, PName = vertexName };
            stages[1] = new() { SType = StructureType.PipelineShaderStageCreateInfo, Stage = ShaderStageFlags.FragmentBit, Module = fs, PName = name };
            var vertexInput = new PipelineVertexInputStateCreateInfo { SType = StructureType.PipelineVertexInputStateCreateInfo };
            var assembly = new PipelineInputAssemblyStateCreateInfo { SType = StructureType.PipelineInputAssemblyStateCreateInfo, Topology = PrimitiveTopology.TriangleList };
            var viewport = new Viewport(0, 0, width, height, 0, 1);
            var scissor = new Rect2D(default, new((uint)width, (uint)height));
            var viewports = new PipelineViewportStateCreateInfo { SType = StructureType.PipelineViewportStateCreateInfo, ViewportCount = 1, PViewports = &viewport, ScissorCount = 1, PScissors = &scissor };
            var raster = new PipelineRasterizationStateCreateInfo { SType = StructureType.PipelineRasterizationStateCreateInfo, PolygonMode = PolygonMode.Fill, CullMode = CullModeFlags.None, FrontFace = FrontFace.CounterClockwise, LineWidth = 1 };
            var multisample = new PipelineMultisampleStateCreateInfo { SType = StructureType.PipelineMultisampleStateCreateInfo, RasterizationSamples = SampleCountFlags.Count1Bit };
            var blendAttachment = new PipelineColorBlendAttachmentState { ColorWriteMask = ColorComponentFlags.RBit | ColorComponentFlags.GBit | ColorComponentFlags.BBit | ColorComponentFlags.ABit };
            var blend = new PipelineColorBlendStateCreateInfo { SType = StructureType.PipelineColorBlendStateCreateInfo, AttachmentCount = 1, PAttachments = &blendAttachment };
            var pipeline = new GraphicsPipelineCreateInfo
            {
                SType = StructureType.GraphicsPipelineCreateInfo, StageCount = 2, PStages = stages,
                PVertexInputState = &vertexInput, PInputAssemblyState = &assembly, PViewportState = &viewports,
                PRasterizationState = &raster, PMultisampleState = &multisample, PColorBlendState = &blend,
                Layout = r.PipelineLayout, RenderPass = r.Pass,
            };
            Check(vk.CreateGraphicsPipelines(device, default, 1, &pipeline, null, out r.Pipeline));
        }
        var created = new CachedPipeline(this, r.Pipeline);
        r.CachedPipeline = created.Retain();
        if (_pipelines.Count >= 32)
        {
            var oldest = _pipelines.First();
            _pipelines.Remove(oldest.Key); oldest.Value.Release();
        }
        _pipelines.Add(pipelineKey, created);
        PipelineCreations++;
        }
        var commandPool = new CommandPoolCreateInfo
        { SType = StructureType.CommandPoolCreateInfo, QueueFamilyIndex = family, Flags = CommandPoolCreateFlags.TransientBit };
        Check(vk.CreateCommandPool(device, &commandPool, null, out r.CommandPool));
        var command = new CommandBufferAllocateInfo
        { SType = StructureType.CommandBufferAllocateInfo, CommandPool = r.CommandPool, Level = CommandBufferLevel.Primary, CommandBufferCount = 1 };
        Check(vk.AllocateCommandBuffers(device, &command, out r.Command));
        observer.Journal.Allocate(r.Command.Handle, r.CommandPool.Handle);
    }

    private void Barrier(CommandBuffer command, VkImage image, ImageLayout from, ImageLayout to,
        AccessFlags source, AccessFlags destination)
    {
        var barrier = new ImageMemoryBarrier
        {
            SType = StructureType.ImageMemoryBarrier, Image = image, OldLayout = from, NewLayout = to,
            SrcAccessMask = source, DstAccessMask = destination,
            SrcQueueFamilyIndex = uint.MaxValue, DstQueueFamilyIndex = uint.MaxValue,
            SubresourceRange = new(ImageAspectFlags.ColorBit, 0, 1, 0, 1),
        };
        observer.Call<VulkanObserver.CmdPipelineBarrierDelegate>("vkCmdPipelineBarrier")(
            command, PipelineStageFlags.AllCommandsBit, PipelineStageFlags.AllCommandsBit, 0, 0, null, 0, null, 1, &barrier);
    }

    private void RecordAndSubmit(Resources r, int width, int height, ImageLayout inputLayout)
    {
        var begin = new CommandBufferBeginInfo { SType = StructureType.CommandBufferBeginInfo, Flags = CommandBufferUsageFlags.OneTimeSubmitBit };
        Check(observer.Call<VulkanObserver.BeginCommandBufferDelegate>("vkBeginCommandBuffer")(r.Command, &begin));
        Barrier(r.Command, r.Input, inputLayout, ImageLayout.ShaderReadOnlyOptimal,
            AccessFlags.MemoryWriteBit, AccessFlags.ShaderReadBit);
        Barrier(r.Command, r.Output, ImageLayout.Undefined, ImageLayout.ColorAttachmentOptimal,
            0, AccessFlags.ColorAttachmentWriteBit);
        var pass = new RenderPassBeginInfo
        { SType = StructureType.RenderPassBeginInfo, RenderPass = r.Pass, Framebuffer = r.Framebuffer, RenderArea = new(default, new((uint)width, (uint)height)) };
        vk.CmdBeginRenderPass(r.Command, &pass, SubpassContents.Inline);
        vk.CmdBindPipeline(r.Command, PipelineBindPoint.Graphics, r.Pipeline);
        DescriptorSet* sets = stackalloc DescriptorSet[2] { r.Set, r.ParameterSet };
        vk.CmdBindDescriptorSets(r.Command, PipelineBindPoint.Graphics, r.PipelineLayout, 0, 2, sets, 0, null);
        vk.CmdDraw(r.Command, 3, 1, 0, 0);
        vk.CmdEndRenderPass(r.Command);
        Barrier(r.Command, r.Input, ImageLayout.ShaderReadOnlyOptimal, inputLayout,
            AccessFlags.ShaderReadBit, AccessFlags.MemoryReadBit | AccessFlags.MemoryWriteBit);
        Barrier(r.Command, r.Output, ImageLayout.ColorAttachmentOptimal, ImageLayout.ShaderReadOnlyOptimal,
            AccessFlags.ColorAttachmentWriteBit, AccessFlags.ShaderReadBit);
        Check(observer.Call<VulkanObserver.EndCommandBufferDelegate>("vkEndCommandBuffer")(r.Command));
        observer.Check();
        var command = r.Command;
        var submit = new SubmitInfo { SType = StructureType.SubmitInfo, CommandBufferCount = 1, PCommandBuffers = &command };
        Check(observer.Call<VulkanObserver.QueueSubmitDelegate>("vkQueueSubmit")(queue, 1, &submit, default));
        observer.Check();
    }

    private void CompleteCancelledSegments()
    {
        // Cancellation is exceptional. A queue fence protects already submitted
        // captures; no device/queue idle or CPU pixel transfer is used.
        if (_cancellationFence.Handle == 0)
        {
            var info = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
            Check(vk.CreateFence(device, &info, null, out _cancellationFence));
            var submit = new SubmitInfo { SType = StructureType.SubmitInfo };
            var submitted = vk.QueueSubmit(queue, 1, &submit, _cancellationFence);
            if (submitted != Result.Success)
            {
                vk.DestroyFence(device, _cancellationFence, null);
                _cancellationFence = default;
                if (submitted == Result.ErrorDeviceLost) session.NotifyVulkanDeviceLost();
                Check(submitted);
            }
        }
        var result = vk.WaitForFences(device, 1, in _cancellationFence, true, 5_000_000_000);
        if (result == Result.ErrorDeviceLost) session.NotifyVulkanDeviceLost();
        else Check(result); // A timeout keeps the fence and frame resources alive for a later drain.
        vk.DestroyFence(device, _cancellationFence, null);
        _cancellationFence = default;
        CancellationFences++;
    }

    public void Dispose()
    {
        if (LiveBytes != 0) throw new InvalidOperationException("Return effect frame resources before disposing the backend.");
        foreach (var pipeline in _pipelines.Values) pipeline.Release();
        _pipelines.Clear();
        if (_cancellationFence.Handle != 0)
        {
            vk.DestroyFence(device, _cancellationFence, null);
            _cancellationFence = default;
        }
    }

    private static void Check(Result result)
    {
        if (result != Result.Success) throw new InvalidOperationException($"GPU effect Vulkan failure: {result}");
    }

    private sealed class CachedPipeline(VulkanGpuEffect owner, Pipeline pipeline)
    {
        private int _references = 1; // cache ownership; frames own additional references
        internal Pipeline Pipeline => pipeline;
        internal CachedPipeline Retain() { _references++; return this; }
        internal void Release()
        {
            if (--_references == 0) owner.DestroyPipeline(pipeline);
        }
    }
    private void DestroyPipeline(Pipeline pipeline) => vk.DestroyPipeline(device, pipeline, null);

    private sealed class Resources(VulkanGpuEffect owner) : IDisposable
    {
        internal VkImage Input, Output;
        internal readonly List<VkImage> Images = [];
        internal readonly List<ulong> Tracked = [];
        internal readonly List<DeviceMemory> Memory = [];
        internal readonly List<ImageView> Views = [];
        internal readonly List<ShaderModule> Shaders = [];
        internal readonly List<Silk.NET.Vulkan.Buffer> Buffers = [];
        internal long Bytes;
        internal SkiaGraphiteSession.VulkanTarget? InputTarget;
        internal SkiaGraphiteSession.VulkanImage? OutputImage;
        internal Sampler Sampler;
        internal DescriptorSetLayout SetLayout;
        internal DescriptorSetLayout ParameterLayout;
        internal DescriptorPool DescriptorPool;
        internal DescriptorSet Set;
        internal DescriptorSet ParameterSet;
        internal PipelineLayout PipelineLayout;
        internal RenderPass Pass;
        internal Framebuffer Framebuffer;
        internal Pipeline Pipeline;
        internal CachedPipeline? CachedPipeline;
        internal CommandPool CommandPool;
        internal CommandBuffer Command;
        private bool disposed;
        public void Dispose()
        {
            if (disposed) return;
            InputTarget?.Dispose();
            OutputImage?.Dispose();
            owner.Release(this);
            disposed = true;
        }
    }

    private void Release(Resources r)
    {
        if (r.CommandPool.Handle != 0)
        {
            observer.Journal.FreePool(r.CommandPool.Handle);
            vk.DestroyCommandPool(device, r.CommandPool, null);
        }
        if (r.Pipeline.Handle != 0 && r.CachedPipeline is null) vk.DestroyPipeline(device, r.Pipeline, null);
        if (r.Framebuffer.Handle != 0) vk.DestroyFramebuffer(device, r.Framebuffer, null);
        if (r.Pass.Handle != 0) vk.DestroyRenderPass(device, r.Pass, null);
        if (r.DescriptorPool.Handle != 0) vk.DestroyDescriptorPool(device, r.DescriptorPool, null);
        if (r.PipelineLayout.Handle != 0) vk.DestroyPipelineLayout(device, r.PipelineLayout, null);
        if (r.SetLayout.Handle != 0) vk.DestroyDescriptorSetLayout(device, r.SetLayout, null);
        if (r.ParameterLayout.Handle != 0) vk.DestroyDescriptorSetLayout(device, r.ParameterLayout, null);
        if (r.Sampler.Handle != 0) vk.DestroySampler(device, r.Sampler, null);
        foreach (var shader in r.Shaders) vk.DestroyShaderModule(device, shader, null);
        foreach (var view in r.Views) vk.DestroyImageView(device, view, null);
        foreach (var image in r.Tracked) observer.ForgetHostTarget(image);
        foreach (var image in r.Images) vk.DestroyImage(device, image, null);
        foreach (var buffer in r.Buffers) vk.DestroyBuffer(device, buffer, null);
        foreach (var memory in r.Memory) vk.FreeMemory(device, memory, null);
        r.CachedPipeline?.Release();
        LiveBytes -= r.Bytes;
    }

    private DescriptorBufferInfo Uniform(Resources r, ReadOnlySpan<byte> bytes)
    {
        vk.GetPhysicalDeviceProperties(physical, out var limits);
        var size = Math.Max(16, bytes.Length);
        if (size > limits.Limits.MaxUniformBufferRange)
            throw new NotSupportedException("Effect uniform exceeds the physical device limit.");
        var info = new BufferCreateInfo { SType = StructureType.BufferCreateInfo, Size = (ulong)size, Usage = BufferUsageFlags.UniformBufferBit };
        Check(vk.CreateBuffer(device, &info, null, out var buffer));
        r.Buffers.Add(buffer);
        vk.GetBufferMemoryRequirements(device, buffer, out var requirements);
        if (requirements.Size > (ulong)(Budget - LiveBytes)) throw new NotSupportedException("Effect buffer budget exceeded.");
        vk.GetPhysicalDeviceMemoryProperties(physical, out var memoryProperties);
        const MemoryPropertyFlags required = MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit;
        uint type = uint.MaxValue;
        for (uint i = 0; i < memoryProperties.MemoryTypeCount; i++)
            if ((requirements.MemoryTypeBits & (1u << (int)i)) != 0 &&
                (memoryProperties.MemoryTypes[(int)i].PropertyFlags & required) == required) { type = i; break; }
        if (type == uint.MaxValue) throw new NotSupportedException("No host coherent uniform memory.");
        var allocation = new MemoryAllocateInfo { SType = StructureType.MemoryAllocateInfo, AllocationSize = requirements.Size, MemoryTypeIndex = type };
        Check(vk.AllocateMemory(device, &allocation, null, out var memory));
        r.Memory.Add(memory);
        r.Bytes += (long)requirements.Size;
        LiveBytes += (long)requirements.Size;
        PeakBytes = Math.Max(PeakBytes, LiveBytes);
        Check(vk.BindBufferMemory(device, buffer, memory, 0));
        void* mapped;
        Check(vk.MapMemory(device, memory, 0, (ulong)size, 0, &mapped));
        try
        {
            var destination = new Span<byte>(mapped, size);
            destination.Clear();
            bytes.CopyTo(destination);
        }
        finally { vk.UnmapMemory(device, memory); }
        return new DescriptorBufferInfo(buffer, 0, (ulong)size);
    }
}
