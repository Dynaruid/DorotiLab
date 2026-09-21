#if IOS || MACCATALYST || MACOS
using System.Runtime.InteropServices;
using AVFoundation;
using CoreFoundation;
using CoreMedia;
using CoreVideo;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Host.Maui;

public sealed record ApplePixelBufferTexture(int Width, int Height, nint PixelBuffer)
    : NativeTextureBuffer(Width, Height, NativeTextureFormat.Bgra8888)
{
    public override NativeTexturePlatform Platform => NativeTexturePlatform.Apple;
}

/// <summary>Retains an immutable producer-complete BGRA IOSurface pixel buffer without locking or reading pixels.
/// Camera/decoder callbacks must publish before returning their sample to the producer pool.</summary>
public static class AppleGpuTextureFrames
{
    public static NativeTextureFrame FromPixelBuffer(CVPixelBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        var handle = (nint)buffer.Handle;
        if (
            handle == 0
            || buffer.PixelFormatType != CVPixelFormatType.CV32BGRA
            || Native.CVPixelBufferGetIOSurface(handle) == 0
        )
            throw new NotSupportedException(
                "A live BGRA IOSurface-backed CVPixelBuffer is required."
            );
        Native.CFRetain(handle);
        try
        {
            return new(
                new ApplePixelBufferTexture(
                    checked((int)buffer.Width),
                    checked((int)buffer.Height),
                    handle
                ),
                () => Native.CFRelease(handle)
            );
        }
        catch
        {
            Native.CFRelease(handle);
            throw;
        }
    }

    internal static class Native
    {
        private const string CV = "/System/Library/Frameworks/CoreVideo.framework/CoreVideo";
        private const string CF =
            "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

        [DllImport(CF)]
        internal static extern nint CFRetain(nint value);

        [DllImport(CF)]
        internal static extern void CFRelease(nint value);

        [DllImport(CV)]
        internal static extern nint CVPixelBufferGetIOSurface(nint value);

        [DllImport(CV)]
        internal static extern int CVMetalTextureCacheCreate(
            nint allocator,
            nint attributes,
            nint device,
            nint textureAttributes,
            out nint cache
        );

        [DllImport(CV)]
        internal static extern int CVMetalTextureCacheCreateTextureFromImage(
            nint allocator,
            nint cache,
            nint buffer,
            nint attributes,
            ulong format,
            nuint width,
            nuint height,
            nuint plane,
            out nint texture
        );

        [DllImport(CV)]
        internal static extern nint CVMetalTextureGetTexture(nint texture);
    }
}

internal sealed class AppleNativeTextureImporter : ISkiaNativeTextureImporter
{
    private readonly SkiaGraphiteSession _session;
    private nint _cache;

    internal AppleNativeTextureImporter(SkiaGraphiteSession session, nint device)
    {
        _session = session;
        var result = AppleGpuTextureFrames.Native.CVMetalTextureCacheCreate(
            0,
            0,
            device,
            0,
            out _cache
        );
        if (result != 0)
            throw new InvalidOperationException($"CVMetalTextureCacheCreate: {result}");
    }

    public SkiaNativeTextureImage Import(NativeTextureFrame frame)
    {
        ObjectDisposedException.ThrowIf(_cache == 0, this);
        if (frame.Buffer is not ApplePixelBufferTexture buffer)
            throw new NotSupportedException("An Apple pixel buffer is required.");
        var lease = frame.Retain();
        nint texture = 0;
        try
        {
            // MTLPixelFormatBGRA8Unorm = 80; the camera delivers sRGB BGRA bytes.
            var result = AppleGpuTextureFrames.Native.CVMetalTextureCacheCreateTextureFromImage(
                0,
                _cache,
                buffer.PixelBuffer,
                0,
                80,
                (nuint)buffer.Width,
                (nuint)buffer.Height,
                0,
                out texture
            );
            if (result != 0 || texture == 0)
                throw new InvalidOperationException($"CVMetalTextureCache import: {result}");
            var metal = AppleGpuTextureFrames.Native.CVMetalTextureGetTexture(texture);
            if (metal == 0)
                throw new InvalidOperationException("CVMetalTexture has no Metal allocation.");
            return new Imported(
                _session.WrapMetalImage(buffer.Width, buffer.Height, metal, SKColorType.Bgra8888),
                texture,
                lease
            );
        }
        catch
        {
            if (texture != 0)
                AppleGpuTextureFrames.Native.CFRelease(texture);
            lease.Dispose();
            throw;
        }
    }

    public void Dispose()
    {
        var cache = _cache;
        _cache = 0;
        if (cache != 0)
            AppleGpuTextureFrames.Native.CFRelease(cache);
    }

    private sealed class Imported(
        SkiaGraphiteSession.VulkanImage wrapped,
        nint texture,
        NativeTextureFrame lease
    ) : SkiaNativeTextureImage
    {
        private bool _disposed;
        public override SKImage Image => wrapped.Image;

        public override void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            wrapped.Dispose();
            AppleGpuTextureFrames.Native.CFRelease(texture);
            lease.Dispose();
        }
    }
}

/// <summary>AVFoundation camera input; request access explicitly with StartAsync.
/// Stop and drain its serial callback queue before releasing the entry. Requires NSCameraUsageDescription.</summary>
public sealed class AppleGpuTextureCamera : IAsyncDisposable
{
    private readonly AVCaptureSession _session = new();
    private readonly DispatchQueue _queue = new("doroti.texture.camera");
    private readonly AVCaptureVideoDataOutput _output = new();
    private readonly Receiver _receiver;
    private AVCaptureDeviceInput? _input;
    private int _disposed;
    public long PublishedFrames => Interlocked.Read(ref _receiver.Published);
    public Exception? Error => Volatile.Read(ref _receiver.Error);

    private AppleGpuTextureCamera(NativeTextureEntry entry) => _receiver = new(entry);

    public static async Task<AppleGpuTextureCamera> StartAsync(
        NativeTextureEntry entry,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(entry);
        if (entry.Device.Platform != NativeTexturePlatform.Apple)
            throw new ArgumentException("An Apple native texture is required.", nameof(entry));
        if (!await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVAuthorizationMediaType.Video))
            throw new UnauthorizedAccessException("Camera access was denied.");
        cancellationToken.ThrowIfCancellationRequested();
        var camera = new AppleGpuTextureCamera(entry);
        try
        {
            await Task.Run(() => camera.Start(), cancellationToken).ConfigureAwait(false);
            return camera;
        }
        catch
        {
            await camera.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }

    private void Start()
    {
        using var device =
            AVCaptureDevice.GetDefaultDevice(AVMediaTypes.Video)
            ?? throw new NotSupportedException("No video camera is available.");
        _input =
            AVCaptureDeviceInput.FromDevice(device)
            ?? throw new InvalidOperationException("Could not open camera input.");
        var attributes = new CVPixelBufferAttributes
        {
            PixelFormatType = CVPixelFormatType.CV32BGRA,
            MetalCompatibility = true,
            AllocateWithIOSurface = true,
        };
        _output.WeakVideoSettings = attributes.Dictionary;
        _output.AlwaysDiscardsLateVideoFrames = true;
        _output.SetSampleBufferDelegate(_receiver, _queue);
        _session.BeginConfiguration();
        try
        {
            if (!_session.CanAddInput(_input) || !_session.CanAddOutput(_output))
                throw new NotSupportedException(
                    "Camera does not support this capture configuration."
                );
            _session.AddInput(_input);
            _session.AddOutput(_output);
        }
        finally
        {
            _session.CommitConfiguration();
        }
        _session.StartRunning();
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;
        await Task.Run(() =>
            {
                _session.StopRunning();
                _output.SetSampleBufferDelegate(null, null);
                _queue.DispatchSync(() => { });
                _receiver.Dispose();
                _output.Dispose();
                _input?.Dispose();
                _session.Dispose();
                _queue.Dispose();
            })
            .ConfigureAwait(false);
    }

    private sealed class Receiver(NativeTextureEntry entry)
        : AVCaptureVideoDataOutputSampleBufferDelegate
    {
        internal long Published;
        internal Exception? Error;

        public override void DidOutputSampleBuffer(
            AVCaptureOutput output,
            CMSampleBuffer sampleBuffer,
            AVCaptureConnection connection
        )
        {
            try
            {
                if (Volatile.Read(ref Error) is not null)
                    return;
                using var image = sampleBuffer.GetImageBuffer();
                if (image is not CVPixelBuffer pixels)
                    throw new NotSupportedException("Camera did not deliver a CVPixelBuffer.");
                using var frame = AppleGpuTextureFrames.FromPixelBuffer(pixels);
                entry.PushFrame(frame);
                Interlocked.Increment(ref Published);
            }
            catch (Exception error)
            {
                Volatile.Write(ref Error, error);
            }
            finally
            {
                sampleBuffer.Dispose();
            }
        }
    }
}
#endif
