using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

public interface ISkiaNativeTextureImporter : IDisposable
{
    /// <summary>Creates a frame resource retaining its own source lease until Dispose after GPU completion.</summary>
    SkiaNativeTextureImage Import(NativeTextureFrame frame);
}

public abstract class SkiaNativeTextureImage : IDisposable
{
    public abstract SKImage Image { get; }
    public abstract void Dispose();
}

public sealed partial class SkiaGraphiteSession
{
    [ThreadStatic]
    private static SkiaGraphiteSession? _currentRecording;
    public static SkiaGraphiteSession? CurrentRecording => _currentRecording;
    public ISkiaNativeTextureImporter? NativeTextureImporter { get; set; }
    private readonly Dictionary<object, SharedNativeImage> _nativeImages = new(
        ReferenceEqualityComparer.Instance
    );

    private sealed class SharedNativeImage(
        SkiaGraphiteSession owner,
        object key,
        SkiaNativeTextureImage image
    )
    {
        private int _references;

        internal SkiaNativeTextureImage Retain()
        {
            _references++;
            return new Lease(this);
        }

        private sealed class Lease(SharedNativeImage shared) : SkiaNativeTextureImage
        {
            private bool _disposed;
            public override SKImage Image => shared.Image;

            public override void Dispose()
            {
                if (_disposed)
                    return;
                shared.Release();
                _disposed = true;
            }
        }

        private SKImage Image => image.Image;

        private void Release()
        {
            if (_references == 1)
            {
                image.Dispose(); // a failed drain leaves both the reference and allocation rooted
                owner._nativeImages.Remove(key);
            }
            _references--;
        }
    }

    public void DrawNativeTexture(
        SKCanvas canvas,
        NativeTextureFrame frame,
        SKRect destination,
        SKSamplingOptions sampling
    )
    {
        CheckOwner();
        var recording =
            _recordingFrame ?? throw new InvalidOperationException("No active GPU recording.");
        if (!recording.NativeTextures.TryGetValue(frame.Buffer, out var image))
        {
            if (!_nativeImages.TryGetValue(frame.Buffer, out var shared))
            {
                var imported = (
                    NativeTextureImporter
                    ?? throw new PlatformNotSupportedException(
                        "This GPU context has no native buffer importer."
                    )
                ).Import(frame);
                shared = new(this, frame.Buffer, imported);
                _nativeImages.Add(frame.Buffer, shared);
            }
            image = shared.Retain();
            recording.NativeTextures.Add(frame.Buffer, image);
        }
        canvas.DrawImage(image.Image, destination, sampling);
    }

    public VulkanImage WrapMetalImage(int width, int height, nint texture, SKColorType colorType)
    {
        CheckOwner();
        if (_recordingFrame is null || _vulkanOwner is not null)
            throw new InvalidOperationException("No active Metal recording.");
        var backend =
            SKGraphiteBackendTexture.CreateMetal(width, height, texture)
            ?? throw new InvalidOperationException("Metal texture wrapping failed.");
        try
        {
            using var color = SKColorSpace.CreateSrgb();
            var image =
                SKImage.FromTexture(_recorder, backend, colorType, SKAlphaType.Premul, color)
                ?? throw new InvalidOperationException("Metal native image creation failed.");
            return new(backend, image);
        }
        catch
        {
            backend.Dispose();
            throw;
        }
    }
}
