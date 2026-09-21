namespace Doroti.Ui;

/// <summary>Optional, view-owned external frame capability.</summary>
public interface ITextureHostCapability
{
    TextureRegistry Textures { get; }
}

public abstract class TextureRegistry
{
    public static TextureRegistry ForView(DorotiView view) =>
        view.RequireCapability<ITextureHostCapability>(
            DorotiCapabilityIds.GraphicsTexture,
            DorotiUiInvocation.Managed("TextureRegistry.ForView")
        ).Textures;

    /// <summary>Creates an initially blank texture. Dispose the entry to unregister it.</summary>
    public abstract TextureEntry CreateTexture();

    public virtual NativeTextureEntry CreateNativeTexture() =>
        throw new PlatformNotSupportedException("This host has no native GPU buffer importer.");

    /// <summary>Creates a native producer surface, or fails explicitly on unsupported hosts.</summary>
    public virtual ValueTask<SurfaceTextureEntry> CreateSurfaceTextureAsync(
        int width,
        int height,
        CancellationToken cancellationToken = default
    ) =>
        ValueTask.FromException<SurfaceTextureEntry>(
            new PlatformNotSupportedException("This host has no native surface texture producer.")
        );
}

/// <summary>Stop camera/decoder production before disposal. Surface is an Android.Views.Surface on Android.</summary>
public abstract class SurfaceTextureEntry : IDisposable
{
    public abstract long Id { get; }
    public abstract object Surface { get; }
    public abstract string? Error { get; }
    public abstract void Dispose();
}

/// <summary>A producer-owned registration. Frames are copied before PushFrame returns.</summary>
public abstract class TextureEntry : IDisposable
{
    public abstract long Id { get; }

    /// <summary>
    /// Publishes sRGB RGBA8888 pixels with premultiplied alpha. RowBytes = 0 means width * 4.
    /// May be called from a producer thread. Only the newest pending frame is retained.
    /// This is a CPU pixel upload API, not a native GPU handle import API.
    /// </summary>
    public abstract void PushFrame(
        ReadOnlySpan<byte> pixels,
        int width,
        int height,
        int rowBytes = 0
    );

    public abstract void Dispose();
}

internal sealed record SceneTexturePayload(
    long TextureId,
    Rect Bounds,
    bool Freeze,
    FilterQuality FilterQuality
);
