using Doroti.Skia.Rendering;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;
using SkiaSharp;
using VkImage = Silk.NET.Vulkan.Image;

namespace Doroti.Skia.Vulkan;

/// <summary>Qt owns the device, queue and swapchain. Graphite owns private R images;
/// QSG samples separate P images after same-queue GPU copies. No CPU pixel transfer.
/// The Qt basic render loop serializes all access on its GUI/render owner.</summary>
public sealed unsafe class GraphiteVulkanQuick : IDisposable
{
    public readonly record struct Percentiles(int Samples, double? P50Ms, double? P95Ms, double? P99Ms);
    public readonly record struct TimingSummary(Percentiles QueueIdle, Percentiles Recording,
        Percentiles CopySubmit, Percentiles FenceWait);
    private readonly List<double> _queueIdleMs = [], _recordingMs = [], _copySubmitMs = [], _fenceWaitMs = [];
    private long _recordingStarted;
    public TimingSummary Timings => new(Percentile(_queueIdleMs), Percentile(_recordingMs),
        Percentile(_copySubmitMs), Percentile(_fenceWaitMs));
    private static Percentiles Percentile(List<double> samples)
    {
        if (samples.Count == 0) return new(0, null, null, null);
        var ordered = samples.Order().ToArray();
        double At(double p) => ordered[Math.Clamp((int)Math.Ceiling(p * ordered.Length) - 1, 0, ordered.Length - 1)];
        return new(ordered.Length, At(.5), At(.95), At(.99));
    }
    private static void Sample(List<double> samples, long start)
    {
        if (samples.Count < 10000) samples.Add(Stopwatch.GetElapsedTime(start).TotalMilliseconds);
    }
    public enum FailureKind { None, DeviceLost, Timeout, VulkanError, SubmissionError }
    // A timed-out fence does not prove that Qt has stopped sampling its images.
    // Keep those allocations and their dispatch owners alive until process exit.
    private static readonly List<GraphiteVulkanQuick> Quarantined = [];
    private const ulong Timeout = 5_000_000_000;
    private readonly Dictionary<uint, DynamicTextureBudget> _textureBudgets = [];
    private readonly ulong[] _heapBytes = new ulong[16];
    private readonly bool _supportsMemoryBudget;
    public sealed record HeapTextureBudget(uint HeapIndex, TextureBudgetSnapshot Budget);
    public IReadOnlyList<HeapTextureBudget> TextureBudgets =>
        _textureBudgets.OrderBy(pair => pair.Key).Select(pair => new HeapTextureBudget(pair.Key, pair.Value.Snapshot)).ToArray();
    public ulong TextureBudgetBytes => _textureBudgets.Values.Aggregate(0UL, (bytes, value) => checked(bytes + value.Snapshot.BudgetBytes));
    public ulong PeakTextureBudgetBytes { get; private set; }
    private readonly int _owner = Environment.CurrentManagedThreadId;
    private readonly Vk _vk = Vk.GetApi();
    private readonly Device _device;
    private readonly PhysicalDevice _physical;
    private readonly Queue _queue;
    private readonly uint _family;
    private readonly VulkanObserver _observer;
    private readonly SkiaGraphiteSession _session;
    private CommandPool _pool;
    private CommandBuffer _command;
    private Fence _fence;
    private readonly List<CopySlot> _copySlots = [];
    private readonly List<Consumer> _consumers = [];
    private CopySlot? _activeSlot;
    private CopySlot? _publicationSlot;
    private CopySlot? _publishedSlot;
    public int FramesInFlight => _copySlots.Count(slot => slot.Pending);
    public bool HasSerialFrames => _copySlots.Any(slot => slot.Pending && slot.Serial);
    public int MaximumFramesInFlight { get; private set; }
    public long ConsumerSubmissions { get; private set; }
    public long CompletedConsumers { get; private set; }
    private sealed class CopySlot
    {
        internal CommandBuffer Command;
        internal Fence Fence;
        internal SkiaGraphiteSession.Frame? Frame;
        internal Layer[] Layers = [];
        internal long SubmittedAt;
        internal bool Serial;
        internal bool AwaitingConsumer;
        internal Consumer? Consumer;
        internal bool Pending => Frame is not null || AwaitingConsumer || Consumer is not null;
    }
    private sealed record Consumer(Fence Fence, Layer[] Layers, long SubmittedAt);
    private SkiaGraphiteSession.Frame? _frame;
    private bool _submitted,
        _disposed;
    private bool _disposing;
    public FailureKind Failure { get; private set; }
    public ulong FrameToken { get; set; }
    public string? FailureOperation { get; private set; }
    public Result? FailureResult { get; private set; }
    private readonly List<Layer> _layers = [];
    private readonly List<Layer> _retired = [];
    private HashSet<Layer> _published = [];
    private readonly Dictionary<int, Layer> _borrowed = [];
    private ulong _nextIdentity;
    private ulong _bytes;
    private int _used;
    private int _width,
        _height;
    public bool IsSoftwareDevice { get; }
    public long Frames { get; private set; }
    public ulong ReservedBytes => _bytes;
    public ulong PeakReservedBytes { get; private set; }
    public int RetiringLayers => _retired.Count;
    public int PeakRetiringLayers { get; private set; }
    public event Action? ResourcesReleasing;

    private sealed class Layer
    {
        internal VkImage R,
            P;
        internal DeviceMemory RMemory,
            PMemory;
        internal ulong RBytes, PBytes;
        internal uint RHeap, PHeap;
        internal SkiaGraphiteSession.VulkanTarget? Target;
        internal bool Initialized;
        internal ulong Identity;
        internal int Width,
            Height;
    }

    public GraphiteVulkanQuick(
        nint instance,
        nint physical,
        nint device,
        nint queue,
        uint family,
        uint apiVersion,
        bool nativeTextureExtensionsRequested = false
    )
    {
        GraphiteNativeLibrary.Configure(GraphiteNativeLibrary.GetPackagedAsset());
        if (apiVersion < ((1u << 22) | (2u << 12)))
        {
            throw new NotSupportedException("Qt Quick requires Vulkan 1.2.");
        }

        _device = new(device);
        _physical = new(physical);
        _queue = new(queue);
        _family = family;
        try
        {
            _vk.GetPhysicalDeviceProperties(_physical, out var properties);
            if (properties.ApiVersion < ((1u << 22) | (2u << 12)))
            {
                throw new NotSupportedException("Qt selected a device below Vulkan 1.2.");
            }

            IsSoftwareDevice = properties.DeviceType == PhysicalDeviceType.Cpu;
            _supportsMemoryBudget = SupportsMemoryBudget();
            _observer = new(_vk, new(instance), _device, _queue, family);
            _session = SkiaGraphiteSession.CreateVulkan(
                new(
                    instance,
                    physical,
                    device,
                    queue,
                    family,
                    apiVersion,
                    _observer.Resolve,
                    image =>
                    {
                        var s = _observer.State((ulong)image);
                        return ((int)s.Layout, s.Family);
                    },
                    _observer.Check
                ),
                1,
                NativeFrameAdmissionPolicy.ShaderFrameLimit
            );
            _session.GpuEffects = new VulkanGpuEffect(_vk, _physical, _device, _queue,
                _family, _observer, _session);
            if (
                nativeTextureExtensionsRequested
                && VulkanNativeTextureImporter.SupportsLinux(_vk, _physical)
            )
                _session.NativeTextureImporter = new VulkanNativeTextureImporter(
                    _session,
                    _vk,
                    _physical,
                    _device,
                    _queue,
                    _family,
                    _observer,
                    Doroti.Ui.NativeTexturePlatform.Linux
                );
            var pool = new CommandPoolCreateInfo
            {
                SType = StructureType.CommandPoolCreateInfo,
                QueueFamilyIndex = family,
                Flags = CommandPoolCreateFlags.ResetCommandBufferBit,
            };
            Check(_vk.CreateCommandPool(_device, &pool, null, out _pool), "Quick copy pool");
            for (var index = 0; index < NativeFrameAdmissionPolicy.ShaderFrameLimit; index++)
            {
                var slot = new CopySlot();
                _copySlots.Add(slot);
                var allocation = new CommandBufferAllocateInfo
                {
                    SType = StructureType.CommandBufferAllocateInfo, CommandPool = _pool,
                    Level = CommandBufferLevel.Primary, CommandBufferCount = 1,
                };
                Check(_vk.AllocateCommandBuffers(_device, &allocation, out slot.Command), "Quick copy command");
                _observer.Journal.Allocate(slot.Command.Handle, _pool.Handle);
                var fence = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
                Check(_vk.CreateFence(_device, &fence, null, out slot.Fence), "Quick copy fence");
            }
        }
        catch
        {
            if (_session is not null)
            {
                try { Dispose(); }
                catch { /* Preserve the initialization error. */ }
            }
            else
            {
                _observer?.Dispose();
                _vk.Dispose();
            }
            throw;
        }
    }

    public SKSurface Begin(int width, int height)
    {
        Verify();
        if (_frame is not null)
        {
            throw new InvalidOperationException("Unretired Quick frame.");
        }

        if (width < 1 || height < 1 || width > 16384 || height > 16384)
        {
            throw new NotSupportedException("Quick raster extent exceeds limits.");
        }
        PollGpuWork();
        _activeSlot = _copySlots.FirstOrDefault(slot => !slot.Pending)
            ?? throw new InvalidOperationException("Quick GPU admission is full.");
        _command = _activeSlot.Command;
        _fence = _activeSlot.Fence;
        _borrowed.Clear();
        // Never render/copy into the published bank, even at the same extent.
        // A rejected native commit must leave its pixels as well as geometry intact.
        _retired.AddRange(_layers);
        PeakRetiringLayers = Math.Max(PeakRetiringLayers, _retired.Count);
        _layers.Clear();
        var resized = _width != width || _height != height;
        _width = width;
        _height = height;
        foreach (var old in _retired.Where(layer => !_published.Contains(layer) && !IsGpuOwned(layer)).ToArray())
        {
            _retired.Remove(old);
            if (resized)
            {
                Destroy(old);
            }
            else
            {
                _layers.Add(old);
            }
        }
        _used = 0;
        Canvas(0);
        _frame = _session.BeginVulkanFrame(_layers[0].Target!);
        _recordingStarted = Stopwatch.GetTimestamp();
        return _frame.Surface;
    }

    public SKCanvas Canvas(int index) => Canvas(index, _width, _height);

    public SKCanvas Canvas(int index, int width, int height)
    {
        Verify();
        if (index < 0 || index >= 17)
        {
            throw new NotSupportedException("Quick raster segment limit exceeded.");
        }

        if (width <= 0 || height <= 0 || width > _width || height > _height)
        {
            throw new ArgumentOutOfRangeException(nameof(width));
        }

        if (index == 0 && _frame is not null && (width != _width || height != _height))
        {
            throw new InvalidOperationException("Cannot resize the active Quick recorder target.");
        }

        _borrowed.Remove(index);
        while (_layers.Count <= index)
        {
            _layers.Add(CreateLayer(width, height));
        }

        if (_layers[index].Width != width || _layers[index].Height != height)
        {
            // Only unpublished staging images can be resized or overwritten.
            Destroy(_layers[index]);
            _layers.RemoveAt(index);
            _layers.Insert(index, CreateLayer(width, height));
        }
        _used = Math.Max(_used, index + 1);
        var canvas = _layers[index].Target!.Canvas;
        canvas.Clear(SKColors.Transparent);
        return canvas;
    }

    public bool TryReuse(int index, ulong identity, int width, int height)
    {
        Verify();
        if (_frame is null || index < 0 || index >= 17)
        {
            throw new InvalidOperationException("No valid Quick recording slot.");
        }

        var layer = _published.FirstOrDefault(layer =>
            layer.Identity == identity && layer.Width == width && layer.Height == height
        );
        if (layer is null)
        {
            return false;
        }
        // Keep staging indices dense for the ordinary canvas/caption path. These
        // spare images are never copied or published for a borrowed slot.
        while (_layers.Count <= index)
        {
            _layers.Add(CreateLayer(width, height));
        }

        _borrowed.Add(index, layer);
        _used = Math.Max(_used, index + 1);
        return true;
    }

    private Layer Output(int index) => _borrowed.GetValueOrDefault(index) ?? _layers[index];

    public ulong Image(int index) => Output(index).P.Handle;

    public ulong Identity(int index) => Output(index).Identity;

    public void MarkPublished()
    {
        Verify();
        _published = Enumerable.Range(0, _used).Select(Output).ToHashSet();
        _publishedSlot = _publicationSlot;
        _publishedSlot!.AwaitingConsumer = true;
        _publicationSlot = null;
        // Unused staging targets have never been handed to QSG this frame.
        foreach (var unused in _layers.Skip(_used).ToArray())
        {
            Destroy(unused);
            _layers.Remove(unused);
        }
    }

    public void Cancel()
    {
        Verify();
        _recordingStarted = 0;
        if (_frame is null)
        {
            return;
        }

        if (_submitted)
        {
            WaitQueue("failed Quick submission drain");
            _frame.CompleteGpuWork();
        }
        else
        {
            _frame.CancelRecording();
        }

        _frame = null;
        _submitted = false;
    }

    public void Complete() => Complete(asynchronous: false);

    public void Complete(bool asynchronous)
    {
        try { CompleteCore(asynchronous); }
        catch
        {
            RecordFailure(FailureKind.SubmissionError, "Quick frame submission");
            throw;
        }
    }

    private void CompleteCore(bool asynchronous)
    {
        Verify();
        if (_frame is null)
        {
            throw new InvalidOperationException("No Quick recording.");
        }

        if (_recordingStarted != 0) Sample(_recordingMs, _recordingStarted);
        _recordingStarted = 0;
        _submitted = true;
        _frame.Submit();
        var reset = _observer.Call<VulkanObserver.ResetCommandBufferDelegate>(
            "vkResetCommandBuffer"
        );
        Check(reset(_command, 0), "Quick copy reset");
        var begin = new CommandBufferBeginInfo
        {
            SType = StructureType.CommandBufferBeginInfo,
            Flags = CommandBufferUsageFlags.OneTimeSubmitBit,
        };
        Check(
            _observer.Call<VulkanObserver.BeginCommandBufferDelegate>("vkBeginCommandBuffer")(
                _command,
                &begin
            ),
            "Quick copy begin"
        );
        var barrier = _observer.Call<VulkanObserver.CmdPipelineBarrierDelegate>(
            "vkCmdPipelineBarrier"
        );
        var transitions = stackalloc ImageMemoryBarrier[2];
        for (int i = 0; i < _used; i++)
        {
            if (_borrowed.ContainsKey(i))
            {
                continue;
            }

            var layer = _layers[i];
            var state = layer.Target!.GetState();
            transitions[0] = Transition(
                layer.R,
                (ImageLayout)state.Layout,
                ImageLayout.TransferSrcOptimal,
                AccessFlags.MemoryWriteBit | AccessFlags.MemoryReadBit,
                AccessFlags.TransferReadBit
            );
            transitions[1] = Transition(
                layer.P,
                layer.Initialized ? ImageLayout.ShaderReadOnlyOptimal : ImageLayout.Undefined,
                ImageLayout.TransferDstOptimal,
                layer.Initialized ? AccessFlags.ShaderReadBit : 0,
                AccessFlags.TransferWriteBit
            );
            barrier(
                _command,
                PipelineStageFlags.AllCommandsBit,
                PipelineStageFlags.AllCommandsBit,
                0,
                0,
                null,
                0,
                null,
                2,
                transitions
            );
            var copy = new ImageCopy
            {
                SrcSubresource = new(ImageAspectFlags.ColorBit, 0, 0, 1),
                DstSubresource = new(ImageAspectFlags.ColorBit, 0, 0, 1),
                Extent = new((uint)layer.Width, (uint)layer.Height, 1),
            };
            _vk.CmdCopyImage(
                _command,
                layer.R,
                ImageLayout.TransferSrcOptimal,
                layer.P,
                ImageLayout.TransferDstOptimal,
                1,
                &copy
            );
            transitions[0] = Transition(
                layer.R,
                ImageLayout.TransferSrcOptimal,
                (ImageLayout)state.Layout,
                AccessFlags.TransferReadBit,
                AccessFlags.MemoryReadBit | AccessFlags.MemoryWriteBit
            );
            transitions[1] = Transition(
                layer.P,
                ImageLayout.TransferDstOptimal,
                ImageLayout.ShaderReadOnlyOptimal,
                AccessFlags.TransferWriteBit,
                AccessFlags.ShaderReadBit
            );
            barrier(
                _command,
                PipelineStageFlags.AllCommandsBit,
                PipelineStageFlags.AllCommandsBit,
                0,
                0,
                null,
                0,
                null,
                2,
                transitions
            );
        }
        Check(
            _observer.Call<VulkanObserver.EndCommandBufferDelegate>("vkEndCommandBuffer")(_command),
            "Quick copy end"
        );
        Check(_vk.ResetFences(_device, 1, in _fence), "Quick copy fence reset");
        var command = _command;
        var submit = new SubmitInfo
        {
            SType = StructureType.SubmitInfo,
            CommandBufferCount = 1,
            PCommandBuffers = &command,
        };
        var submitStart = Stopwatch.GetTimestamp();
        try {
            Check(
                _observer.Call<VulkanObserver.QueueSubmitDelegate>("vkQueueSubmit")(
                    _queue, 1, &submit, _fence
                ), "Quick copy submit"
            );
        }
        finally { Sample(_copySubmitMs, submitStart); }
        var slot = _activeSlot ?? throw new InvalidOperationException("No Quick copy slot.");
        slot.Frame = _frame;
        slot.Layers = _layers.Take(_used).Where((_, index) => !_borrowed.ContainsKey(index)).ToArray();
        slot.SubmittedAt = Stopwatch.GetTimestamp();
        slot.Serial = !asynchronous;
        _publicationSlot = slot;
        _frame = null;
        _submitted = false;
        _activeSlot = null;
        MaximumFramesInFlight = Math.Max(MaximumFramesInFlight, FramesInFlight);
        if (asynchronous)
        {
            Frames++;
            return;
        }
        var fenceStart = Stopwatch.GetTimestamp();
        Result fenceResult;
        try { fenceResult = _vk.WaitForFences(_device, 1, in _fence, true, Timeout); }
        catch (Exception error)
        {
            RecordFailure(FailureKind.VulkanError, "Quick copy completion");
            throw new InvalidOperationException(
                $"Quick copy completion threw after {Stopwatch.GetElapsedTime(fenceStart).TotalMilliseconds} ms; owner={_owner}; token={FrameToken}.", error);
        }
        var fenceMs = Stopwatch.GetElapsedTime(fenceStart).TotalMilliseconds;
        if (_fenceWaitMs.Count < 10000) _fenceWaitMs.Add(fenceMs);
        Check(fenceResult, "Quick copy completion", fenceMs);
        _observer.Check();
        Retire(slot);
        Frames++;
    }

    private bool IsGpuOwned(Layer layer) => _copySlots.Any(slot => slot.Frame is not null && slot.Layers.Contains(layer))
        || _consumers.Any(consumer => consumer.Layers.Contains(layer));

    private void Retire(CopySlot slot)
    {
        slot.Frame!.CompleteGpuWork();
        foreach (var layer in slot.Layers)
        {
            var state = layer.Target!.GetState();
            layer.Target.SetStateAfterGpuCompletion(state.Layout, state.QueueFamily);
            layer.Initialized = true;
        }
        slot.Frame = null;
        slot.Layers = [];
    }

    /// <summary>Called from Qt afterFrameEnd, after Qt submitted sampling to this same queue.
    /// A frameSwapped receipt alone never authorizes image reuse.</summary>
    public void QtConsumerSubmitted()
    {
        Verify();
        if (_published.Count == 0) return;
        var info = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
        Check(_vk.CreateFence(_device, &info, null, out var fence), "Qt consumer fence");
        var result = _observer.Call<VulkanObserver.QueueSubmitDelegate>("vkQueueSubmit")(_queue, 0, null, fence);
        if (result != Result.Success)
        {
            _vk.DestroyFence(_device, fence, null);
            Check(result, "Qt consumer retirement marker");
        }
        var consumer = new Consumer(fence, _published.ToArray(), Stopwatch.GetTimestamp());
        _consumers.Add(consumer);
        if (_publishedSlot is { AwaitingConsumer: true } slot)
        {
            slot.Consumer = consumer;
            slot.AwaitingConsumer = false;
        }
        ConsumerSubmissions++;
        PollGpuWork();
    }

    /// <summary>Nonblocking producer/copy and Qt consumer retirement on the basic render-loop owner.</summary>
    public bool PollGpuWork()
    {
        Verify();
        bool Ready(Fence fence, long started)
        {
            var result = _vk.GetFenceStatus(_device, fence);
            if (result == Result.NotReady)
            {
                if (Stopwatch.GetElapsedTime(started) > TimeSpan.FromSeconds(5))
                {
                    RecordFailure(FailureKind.Timeout, "Quick retirement timeout");
                    throw new TimeoutException("Quick GPU retirement exceeded five seconds; allocations remain held.");
                }
                return false;
            }
            Check(result, "Quick retirement poll");
            return true;
        }
        foreach (var slot in _copySlots)
            if (slot.Frame is not null && Ready(slot.Fence, slot.SubmittedAt)) Retire(slot);
        foreach (var consumer in _consumers.ToArray())
        {
            if (!Ready(consumer.Fence, consumer.SubmittedAt)) continue;
            _vk.DestroyFence(_device, consumer.Fence, null);
            _consumers.Remove(consumer);
            foreach (var slot in _copySlots)
                if (ReferenceEquals(slot.Consumer, consumer)) slot.Consumer = null;
            CompletedConsumers++;
        }
        return FramesInFlight == 0 && _consumers.Count == 0;
    }

    private static ImageMemoryBarrier Transition(
        VkImage image,
        ImageLayout from,
        ImageLayout to,
        AccessFlags source,
        AccessFlags destination
    ) =>
        new()
        {
            SType = StructureType.ImageMemoryBarrier,
            Image = image,
            OldLayout = from,
            NewLayout = to,
            SrcAccessMask = source,
            DstAccessMask = destination,
            SrcQueueFamilyIndex = uint.MaxValue,
            DstQueueFamilyIndex = uint.MaxValue,
            SubresourceRange = new(ImageAspectFlags.ColorBit, 0, 1, 0, 1),
        };

    private Layer CreateLayer(int width, int height)
    {
        var layer = new Layer
        {
            Identity = ++_nextIdentity,
            Width = width,
            Height = height,
        };
        try
        {
            var info = new ImageCreateInfo
            {
                SType = StructureType.ImageCreateInfo,
                ImageType = ImageType.Type2D,
                Format = Format.R8G8B8A8Unorm,
                Extent = new((uint)width, (uint)height, 1),
                MipLevels = 1,
                ArrayLayers = 1,
                Samples = SampleCountFlags.Count1Bit,
                Tiling = ImageTiling.Optimal,
                SharingMode = SharingMode.Exclusive,
                Usage =
                    ImageUsageFlags.ColorAttachmentBit
                    | ImageUsageFlags.InputAttachmentBit
                    | ImageUsageFlags.TransferSrcBit
                    | ImageUsageFlags.TransferDstBit
                    | ImageUsageFlags.SampledBit,
            };
            Allocate(info, out layer.R, out layer.RMemory, out layer.RHeap, out layer.RBytes);
            _observer.RegisterHostTarget(layer.R.Handle, info);
            layer.Target = _session.CreateVulkanTarget(
                width,
                height,
                new SKGraphiteVkTextureInfo
                {
                    SampleCount = 1,
                    Format = (int)info.Format,
                    ImageTiling = (int)info.Tiling,
                    ImageUsageFlags = (uint)info.Usage,
                    AspectMask = (uint)ImageAspectFlags.ColorBit,
                },
                (int)ImageLayout.Undefined,
                _family,
                (nint)layer.R.Handle,
                SKColorType.Rgba8888
            );
            info.Usage = ImageUsageFlags.TransferDstBit | ImageUsageFlags.SampledBit;
            Allocate(info, out layer.P, out layer.PMemory, out layer.PHeap, out layer.PBytes);
            // P participates only in host copy observation. Qt promises to sample
            // the imported RGBA texture in the supplied SHADER_READ_ONLY layout.
            _observer.RegisterHostTarget(layer.P.Handle, info);
            return layer;
        }
        catch
        {
            Destroy(layer);
            throw;
        }
    }

    private void Allocate(
        ImageCreateInfo info,
        out VkImage image,
        out DeviceMemory memory,
        out uint heapIndex,
        out ulong allocatedBytes
    )
    {
        memory = default;
        heapIndex = 0;
        allocatedBytes = 0;
        Check(_vk.CreateImage(_device, &info, null, out image), "Quick image");
        _vk.GetImageMemoryRequirements(_device, image, out var requirements);
        _vk.GetPhysicalDeviceMemoryProperties(_physical, out var properties);
        var current = new PhysicalDeviceMemoryBudgetPropertiesEXT
        { SType = StructureType.PhysicalDeviceMemoryBudgetPropertiesExt };
        if (_supportsMemoryBudget)
        {
            var extended = new PhysicalDeviceMemoryProperties2
            { SType = StructureType.PhysicalDeviceMemoryProperties2, PNext = &current };
            _vk.GetPhysicalDeviceMemoryProperties2(_physical, &extended);
        }
        uint type = uint.MaxValue;
        DynamicTextureBudget? budget = null;
        for (uint i = 0; i < properties.MemoryTypeCount; i++)
        {
            var flags = properties.MemoryTypes[(int)i].PropertyFlags;
            if ((requirements.MemoryTypeBits & (1u << (int)i)) == 0
                || (flags & MemoryPropertyFlags.DeviceLocalBit) == 0) continue;
            var heap = properties.MemoryTypes[(int)i].HeapIndex;
            if (!_textureBudgets.TryGetValue(heap, out var candidate))
                _textureBudgets.Add(heap, candidate = new());
            // UMA/software also consumes host RAM. This is a capacity estimate,
            // not a claim to have measured current free system memory.
            var hostCapacity = IsSoftwareDevice || (flags & MemoryPropertyFlags.HostVisibleBit) != 0
                ? GC.GetGCMemoryInfo().TotalAvailableMemoryBytes : 0;
            if (!candidate.TryReserve(_heapBytes[heap], requirements.Size,
                properties.MemoryHeaps[(int)heap].Size,
                _supportsMemoryBudget ? current.HeapBudget[(int)heap] : null,
                _supportsMemoryBudget ? current.HeapUsage[(int)heap] : null,
                hostCapacity > 0 ? (ulong)hostCapacity : null)) continue;
            type = i;
            heapIndex = heap;
            budget = candidate;
            break;
        }
        PeakTextureBudgetBytes = Math.Max(PeakTextureBudgetBytes, TextureBudgetBytes);
        if (budget is null)
        {
            var limits = string.Join("; ", _textureBudgets.Select(pair =>
                $"heap={pair.Key}, owned={pair.Value.Snapshot.AllocatedBytes}, limit={pair.Value.Snapshot.LimitBytes}, source={pair.Value.Snapshot.Source}"));
            throw new NotSupportedException($"Quick texture allocation needs {requirements.Size} bytes; no compatible device-local heap has headroom. "
                + limits + ". Published and GPU-owned images remain held until consumer completion.");
        }

        var allocate = new MemoryAllocateInfo
        {
            SType = StructureType.MemoryAllocateInfo,
            AllocationSize = requirements.Size,
            MemoryTypeIndex = type,
        };
        Check(_vk.AllocateMemory(_device, &allocate, null, out memory), "Quick image memory");
        allocatedBytes = requirements.Size;
        _heapBytes[heapIndex] += allocatedBytes;
        _bytes += allocatedBytes;
        budget.ObserveAllocated(_heapBytes[heapIndex]);
        PeakReservedBytes = Math.Max(PeakReservedBytes, _bytes);
        Check(_vk.BindImageMemory(_device, image, memory, 0), "Quick image bind");
    }

    private void Destroy(Layer layer)
    {
        layer.Target?.Dispose();
        foreach (var image in new[] { layer.R, layer.P })
        {
            if (image.Handle != 0)
            {
                _observer.ForgetHostTarget(image.Handle);
                _vk.DestroyImage(_device, image, null);
            }
        }

        if (layer.RMemory.Handle != 0)
        {
            _vk.FreeMemory(_device, layer.RMemory, null);
        }

        if (layer.PMemory.Handle != 0)
        {
            _vk.FreeMemory(_device, layer.PMemory, null);
        }

        _heapBytes[layer.RHeap] -= layer.RBytes;
        _heapBytes[layer.PHeap] -= layer.PBytes;
        _bytes -= layer.RBytes + layer.PBytes;
        foreach (var heap in new[] { layer.RHeap, layer.PHeap }.Distinct())
            if (_textureBudgets.TryGetValue(heap, out var budget)) budget.ObserveAllocated(_heapBytes[heap]);
    }

    private bool SupportsMemoryBudget()
    {
        uint count = 0;
        if (_vk.EnumerateDeviceExtensionProperties(_physical, (byte*)null, &count, null) != Result.Success)
            return false;
        var properties = new ExtensionProperties[count];
        fixed (ExtensionProperties* data = properties)
            if (_vk.EnumerateDeviceExtensionProperties(_physical, (byte*)null, &count, data) != Result.Success)
                return false;
        foreach (var value in properties)
        {
            var copy = value;
            if (Marshal.PtrToStringUTF8((nint)copy.ExtensionName) == "VK_EXT_memory_budget") return true;
        }
        return false;
    }

    private void WaitQueue(string operation, List<double>? samples = null)
    {
        var start = Stopwatch.GetTimestamp();
        Result result;
        try { result = _vk.QueueWaitIdle(_queue); }
        catch (Exception error)
        {
            RecordFailure(FailureKind.VulkanError, operation);
            throw new InvalidOperationException(
                $"{operation} threw after {Stopwatch.GetElapsedTime(start).TotalMilliseconds} ms; owner={_owner}; token={FrameToken}.", error);
        }
        var elapsed = Stopwatch.GetElapsedTime(start).TotalMilliseconds;
        if (samples is not null && samples.Count < 10000) samples.Add(elapsed);
        Check(result, operation, elapsed);
    }

    private void Check(Result result, string operation, double? waitMs = null)
    {
        if (result == Result.Success)
        {
            return;
        }

        RecordFailure(result == Result.ErrorDeviceLost ? FailureKind.DeviceLost
            : result == Result.Timeout ? FailureKind.Timeout : FailureKind.VulkanError,
            operation, result);
        if (result == Result.ErrorDeviceLost)
        {
            _session?.NotifyVulkanDeviceLost();
        }

        throw new InvalidOperationException($"{operation}: {result}; waitMs={waitMs}; owner={_owner}; token={FrameToken}; frame={Frames}; failure={Failure}");
    }

    internal void InjectFailureForContract(Result result) => Check(result, "injected Quick failure");

    private void RecordFailure(FailureKind kind, string operation, Result? result = null)
    {
        if (Failure != FailureKind.None) return;
        Failure = kind;
        FailureOperation = operation;
        FailureResult = result;
    }

    private void Verify()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_disposing || Failure != FailureKind.None)
        {
            throw new InvalidOperationException(
                $"Quick GPU is terminal: {FailureOperation ?? "disposing"}, {FailureResult}; owner={_owner}; token={FrameToken}; frame={Frames}."
            );
        }
        if (Environment.CurrentManagedThreadId != _owner)
        {
            throw new InvalidOperationException("Quick GPU owner thread mismatch.");
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        if (Environment.CurrentManagedThreadId != _owner)
            throw new InvalidOperationException("Quick GPU owner thread mismatch.");
        _disposing = true;
        Exception? cleanupError = null;
        try
        {
            if (Failure == FailureKind.None)
            {
                try { WaitQueue("Quick terminal GPU drain"); }
                catch (Exception error) { cleanupError = error; }
            }
            // Do not free images or a submitted recording after an unproven drain.
            if (Failure != FailureKind.None)
            {
                if (_frame is not null && !_submitted)
                {
                    try { _frame.CancelRecording(); _frame = null; }
                    catch (Exception error) { cleanupError ??= error; }
                }
                lock (Quarantined) Quarantined.Add(this);
                return;
            }
            if (_frame is not null)
            {
                try
                {
                    if (_submitted) _frame.CompleteGpuWork();
                    else _frame.CancelRecording();
                    _frame = null;
                    _submitted = false;
                }
                catch (Exception error) { cleanupError ??= error; }
            }
            foreach (var slot in _copySlots)
                if (slot.Frame is not null) Retire(slot);
            try { ResourcesReleasing?.Invoke(); }
            catch (Exception error) { cleanupError ??= error; }
            foreach (var layer in _layers.Concat(_retired).Distinct())
            {
                try { Destroy(layer); }
                catch (Exception error) { cleanupError ??= error; }
            }
            _layers.Clear();
            _retired.Clear();
            try { _session?.Dispose(); }
            catch (Exception error) { cleanupError ??= error; }
            foreach (var slot in _copySlots)
            {
                slot.AwaitingConsumer = false;
                slot.Consumer = null;
                if (slot.Fence.Handle != 0) _vk.DestroyFence(_device, slot.Fence, null);
            }
            foreach (var consumer in _consumers) _vk.DestroyFence(_device, consumer.Fence, null);
            CompletedConsumers += _consumers.Count;
            _consumers.Clear();
            if (_pool.Handle != 0) _vk.DestroyCommandPool(_device, _pool, null);
            _observer?.Journal.FreePool(_pool.Handle);
            try { _observer?.Check(); }
            catch (Exception error) { cleanupError ??= error; }
            _observer?.Dispose();
            _vk.Dispose(); // Qt's borrowed device/instance are never destroyed here.
        }
        finally
        {
            _disposed = true;
            _disposing = false;
        }
        if (cleanupError is not null) throw cleanupError;
    }
}
