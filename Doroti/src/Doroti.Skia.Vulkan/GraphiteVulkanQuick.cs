using Doroti.Skia.Rendering;
using System.Diagnostics;
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
    private const ulong Budget = 128UL * 1024 * 1024;
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
        internal ulong Bytes;
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
                1
            );
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
            var allocation = new CommandBufferAllocateInfo
            {
                SType = StructureType.CommandBufferAllocateInfo,
                CommandPool = _pool,
                Level = CommandBufferLevel.Primary,
                CommandBufferCount = 1,
            };
            Check(
                _vk.AllocateCommandBuffers(_device, &allocation, out _command),
                "Quick copy command"
            );
            _observer.Journal.Allocate(_command.Handle, _pool.Handle);
            var fence = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
            Check(_vk.CreateFence(_device, &fence, null, out _fence), "Quick copy fence");
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
        // Qt submitted the preceding scene-graph frame before this GUI-thread sync.
        // Do not overwrite or free any P while Qt can still sample it.
        WaitQueue("Qt sampling retirement", _queueIdleMs);
        _borrowed.Clear();
        // Never render/copy into the published bank, even at the same extent.
        // A rejected native commit must leave its pixels as well as geometry intact.
        _retired.AddRange(_layers);
        PeakRetiringLayers = Math.Max(PeakRetiringLayers, _retired.Count);
        _layers.Clear();
        var resized = _width != width || _height != height;
        _width = width;
        _height = height;
        foreach (var old in _retired.Where(layer => !_published.Contains(layer)).ToArray())
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

    public void Complete()
    {
        try { CompleteCore(); }
        catch
        {
            RecordFailure(FailureKind.SubmissionError, "Quick frame submission");
            throw;
        }
    }

    private void CompleteCore()
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
        _frame.CompleteGpuWork();
        _frame = null;
        _submitted = false;
        for (int i = 0; i < _used; i++)
        {
            if (_borrowed.ContainsKey(i))
            {
                continue;
            }

            var layer = _layers[i];
            var state = layer.Target!.GetState();
            layer.Target.SetStateAfterGpuCompletion(state.Layout, state.QueueFamily);
            layer.Initialized = true;
        }
        Frames++;
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
            Allocate(info, out layer.R, out layer.RMemory, layer);
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
            Allocate(info, out layer.P, out layer.PMemory, layer);
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
        Layer layer
    )
    {
        memory = default;
        Check(_vk.CreateImage(_device, &info, null, out image), "Quick image");
        _vk.GetImageMemoryRequirements(_device, image, out var requirements);
        if (requirements.Size > Budget || _bytes > Budget - requirements.Size)
        {
            throw new NotSupportedException(
                "Quick R/P and retiring images exceed the 128 MiB budget."
            );
        }

        _vk.GetPhysicalDeviceMemoryProperties(_physical, out var properties);
        uint type = uint.MaxValue;
        for (uint i = 0; i < properties.MemoryTypeCount; i++)
        {
            if (
                (requirements.MemoryTypeBits & (1u << (int)i)) != 0
                && (
                    properties.MemoryTypes[(int)i].PropertyFlags
                    & MemoryPropertyFlags.DeviceLocalBit
                ) != 0
            )
            {
                type = i;
                break;
            }
        }

        if (type == uint.MaxValue)
        {
            throw new NotSupportedException("No device-local Quick image memory.");
        }

        var allocate = new MemoryAllocateInfo
        {
            SType = StructureType.MemoryAllocateInfo,
            AllocationSize = requirements.Size,
            MemoryTypeIndex = type,
        };
        Check(_vk.AllocateMemory(_device, &allocate, null, out memory), "Quick image memory");
        _bytes += requirements.Size;
        layer.Bytes += requirements.Size;
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

        _bytes -= layer.Bytes;
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
            if (_fence.Handle != 0) _vk.DestroyFence(_device, _fence, null);
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
