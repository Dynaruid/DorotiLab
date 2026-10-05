using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

/// <summary>Backend adapter invoked on the raster owner. It must retain native buffers through GPU retirement.</summary>
public interface ISkiaExternalTextureSource : IDisposable
{
    void Draw(SKCanvas canvas, SKRect destination, SKSamplingOptions sampling, bool freeze);
    /// <summary>Captures immutable producer storage without importing it on the
    /// framework thread. The returned lease survives source/registration disposal.</summary>
    SkiaExternalTextureFrame CaptureFrame(bool freeze) =>
        throw new PlatformNotSupportedException("This external texture source has no immutable submission snapshot.");
}

public abstract class SkiaExternalTextureFrame : IDisposable
{
    public abstract void Draw(SKCanvas canvas, SKRect destination, SKSamplingOptions sampling);
    public abstract void Dispose();
}

public abstract class SkiaExternalTextureRegistration : IDisposable
{
    public abstract long Id { get; }
    public abstract void MarkFrameAvailable();
    public abstract void Dispose();
}

public sealed partial class SkiaSceneRenderer
{
    public SkiaExternalTextureRegistration RegisterExternalTexture(
        ISkiaExternalTextureSource source
    ) => _textures.RegisterExternal(source);

    public void SetSurfaceTextureFactory(
        Func<int, int, CancellationToken, ValueTask<SurfaceTextureEntry>> factory
    ) => _textures.SurfaceFactory = factory;
}
