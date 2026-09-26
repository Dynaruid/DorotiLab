#if IOS || MACCATALYST || MACOS
using System.Text;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using Foundation;
using Metal;
using SkiaSharp;

namespace Doroti.Host.Maui;

/// <summary>Fragment effects on the host's existing Metal device and command queue.</summary>
internal sealed unsafe class AppleGpuEffects(IMTLDevice device, IMTLCommandQueue queue,
    SkiaGraphiteSession session) : ISkiaGpuEffectBackend, IDisposable
{
    private readonly Dictionary<string, IMTLRenderPipelineState> _pipelines = [];
    private const long Budget = 128L * 1024 * 1024;
    private long _liveBytes;
    private IMTLCommandBuffer? _cancellation;
    private TaskCompletionSource? _cancellationCompleted;

    public void Draw(SKCanvas destination, int width, int height, Action<SKCanvas> capture,
        GpuEffectProgram program, GpuEffectParameters parameters, float logicalWidth = 0, float logicalHeight = 0)
    {
        var variant = program.GetVariant("metal-fragment");
        if (parameters.Bytes.Length != program.ParameterByteCount || parameters.Bytes.Length > 65536)
            throw new ArgumentException("Metal effect parameter ABI/limit mismatch.");
        var size = checked((long)width * height * 8 + 32 + Math.Max(16, parameters.Bytes.Length));
        if (width < 1 || height < 1 || size > Budget - _liveBytes)
            throw new NotSupportedException("Metal effect exceeds its memory budget.");
        var frame = session.RecordingFrame;
        var lease = new Lease(this, size);
        _liveBytes += size;
        frame.RetainUntilGpuCompletion(lease);
        using var descriptor = MTLTextureDescriptor.CreateTexture2DDescriptor(MTLPixelFormat.RGBA8Unorm,
            (nuint)width, (nuint)height, false);
        descriptor.StorageMode = MTLStorageMode.Private;
        descriptor.Usage = MTLTextureUsage.RenderTarget | MTLTextureUsage.ShaderRead;
        var input = lease.Keep(device.CreateTexture(descriptor)
            ?? throw new InvalidOperationException("Metal input allocation failed."));
        var output = lease.Keep(device.CreateTexture(descriptor)
            ?? throw new InvalidOperationException("Metal output allocation failed."));
        var surface = session.WrapMetalEffectSurface(width, height, input.Handle);
        capture(surface.Canvas);
        var pipeline = Pipeline(program, variant);
        frame.SubmitSegment(CompleteCancelledSegments);
        var command = lease.Keep(queue.CommandBuffer()
            ?? throw new InvalidOperationException("Metal effect command buffer unavailable."));
        using var pass = new MTLRenderPassDescriptor();
        pass.ColorAttachments[0].Texture = output;
        pass.ColorAttachments[0].LoadAction = MTLLoadAction.Clear;
        pass.ColorAttachments[0].StoreAction = MTLStoreAction.Store;
        pass.ColorAttachments[0].ClearColor = new MTLClearColor(0, 0, 0, 0);
        using var encoder = command.CreateRenderCommandEncoder(pass)
            ?? throw new InvalidOperationException("Metal effect encoder unavailable.");
        encoder.SetRenderPipelineState(pipeline);
        encoder.SetFragmentTexture(input, 0);
        using var samplerDescriptor = new MTLSamplerDescriptor
        {
            MinFilter = MTLSamplerMinMagFilter.Nearest, MagFilter = MTLSamplerMinMagFilter.Nearest,
            SAddressMode = MTLSamplerAddressMode.ClampToEdge, TAddressMode = MTLSamplerAddressMode.ClampToEdge,
        };
        var sampler = lease.Keep(device.CreateSamplerState(samplerDescriptor)
            ?? throw new InvalidOperationException("Metal effect sampler unavailable."));
        encoder.SetFragmentSamplerState(sampler, 0);
        Span<byte> frameBytes = stackalloc byte[32];
        frameBytes.Clear();
        System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(frameBytes, (uint)width);
        System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(frameBytes[4..], (uint)height);
        frameBytes[..8].CopyTo(frameBytes[8..]);
        System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(frameBytes[16..], logicalWidth > 0 ? logicalWidth : width);
        System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(frameBytes[20..], logicalHeight > 0 ? logicalHeight : height);
        System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(frameBytes[24..], parameters.Time);
        System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(frameBytes[28..], parameters.DeltaTime);
        var frameBuffer = Buffer(lease, frameBytes);
        var userBuffer = Buffer(lease, parameters.Bytes);
        encoder.SetFragmentBuffer(frameBuffer, 0, 0);
        encoder.SetFragmentBuffer(userBuffer, 0, 1);
        encoder.DrawPrimitives(MTLPrimitiveType.Triangle, 0, 3);
        encoder.EndEncoding();
        command.Commit();
        var wrapped = session.WrapMetalImage(width, height, output.Handle, SKColorType.Rgba8888);
        frame.RetainUntilGpuCompletion(wrapped);
        destination.DrawImage(wrapped.Image, 0, 0, new SKSamplingOptions(SKFilterMode.Nearest));
    }

    private IMTLBuffer Buffer(Lease lease, ReadOnlySpan<byte> bytes)
    {
        var storage = new byte[Math.Max(16, bytes.Length)];
        bytes.CopyTo(storage);
        fixed (byte* pointer = storage)
            return lease.Keep(device.CreateBuffer((nint)pointer, (nuint)storage.Length, MTLResourceOptions.StorageModeShared)
                ?? throw new InvalidOperationException("Metal effect uniform allocation failed."));
    }

    private IMTLRenderPipelineState Pipeline(GpuEffectProgram program, GpuEffectVariant variant)
    {
        if (_pipelines.TryGetValue(program.ContentHash, out var existing)) return existing;
        using var options = new MTLCompileOptions();
        using var vertexLibrary = device.CreateLibrary(Encoding.UTF8.GetString(variant.Vertex), options, out var vertexError)
            ?? throw new InvalidOperationException($"Metal effect vertex compilation failed: {vertexError}");
        using var fragmentLibrary = device.CreateLibrary(Encoding.UTF8.GetString(variant.Fragment), options, out var fragmentError)
            ?? throw new InvalidOperationException($"Metal effect fragment compilation failed: {fragmentError}");
        using var vertex = vertexLibrary.CreateFunction("main_")
            ?? throw new InvalidOperationException("Metal ABI 1 fullscreen vertex entry is missing.");
        using var fragment = fragmentLibrary.CreateFunction(variant.EntryPoint)
            ?? throw new InvalidOperationException("Metal fragment entry is missing.");
        using var descriptor = new MTLRenderPipelineDescriptor { VertexFunction = vertex, FragmentFunction = fragment };
        descriptor.ColorAttachments[0].PixelFormat = MTLPixelFormat.RGBA8Unorm;
        var pipeline = device.CreateRenderPipelineState(descriptor, out var error)
            ?? throw new InvalidOperationException($"Metal effect pipeline failed: {error}");
        if (_pipelines.Count >= 64)
        {
            var oldest = _pipelines.First();
            _pipelines.Remove(oldest.Key); oldest.Value.Dispose();
        }
        _pipelines.Add(program.ContentHash, pipeline);
        return pipeline;
    }

    private void CompleteCancelledSegments()
    {
        if (_cancellation is null)
        {
            _cancellation = queue.CommandBuffer()
                ?? throw new InvalidOperationException("Metal cancellation fence unavailable.");
            _cancellationCompleted = new(TaskCreationOptions.RunContinuationsAsynchronously);
            var completion = _cancellationCompleted;
            _cancellation.AddCompletedHandler(_ => completion.TrySetResult());
            _cancellation.Commit();
        }
        if (!_cancellationCompleted!.Task.Wait(TimeSpan.FromSeconds(5)))
            throw new TimeoutException("Metal effect cancellation timed out; frame resources remain retained.");
        var error = _cancellation.Status != MTLCommandBufferStatus.Completed ? _cancellation.Error?.ToString() : null;
        _cancellation.Dispose(); _cancellation = null; _cancellationCompleted = null;
        if (error is not null) throw new InvalidOperationException($"Metal cancellation completion failed: {error}");
    }

    public void Dispose()
    {
        if (_liveBytes != 0) throw new InvalidOperationException("Metal effect resources have not retired.");
        foreach (var pipeline in _pipelines.Values) pipeline.Dispose();
        _pipelines.Clear();
        _cancellation?.Dispose(); _cancellation = null;
    }

    private sealed class Lease(AppleGpuEffects owner, long bytes) : IDisposable
    {
        private readonly List<IDisposable> _items = [];
        private bool _disposed;
        internal T Keep<T>(T value) where T : IDisposable { _items.Add(value); return value; }
        public void Dispose()
        {
            if (_disposed) return;
            for (var i = _items.Count - 1; i >= 0; i--) _items[i].Dispose();
            _items.Clear(); owner._liveBytes -= bytes; _disposed = true;
        }
    }
}
#endif
