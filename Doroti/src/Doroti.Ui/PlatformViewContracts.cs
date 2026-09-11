namespace Doroti.Ui;

/// <summary>Native instance identity. Independent of surface, scene and frame generations.</summary>
public readonly record struct PlatformViewHandle(ulong OwnerViewId, long InstanceId, long InstanceGeneration);

public enum PlatformViewState { Requested, Creating, Ready, Attached, Hidden, Detached, Disposing, Disposed, Failed }
public enum PlatformViewComposition { NativeOverlay, InterleavedComposition, ExternalTexture, Snapshot }

[Flags]
public enum PlatformViewEffects
{
    None = 0, RectClip = 1, RoundedClip = 2, PathClip = 4, AffineTransform = 8,
    Perspective = 16, Opacity = 32, Backdrop = 64,
}

public sealed record PlatformViewRequest(
    long InstanceId, string ViewType,
    PlatformViewComposition Composition = PlatformViewComposition.NativeOverlay,
    PlatformViewEffects Effects = PlatformViewEffects.None,
    ReadOnlyMemory<byte> CreationParameters = default);

/// <summary>Describes this backend/runtime/control combination, never other runners.</summary>
public sealed record PlatformViewSupport(
    string Backend, string Runtime, string ViewType, bool Supported,
    PlatformViewComposition Composition, PlatformViewEffects Effects,
    bool CaptureIncludesNative = false, bool GestureMediation = false,
    bool Accessibility = false, bool SynchronizedPlacement = false, string? Reason = null);

/// <summary>Column-vector 2D affine transform in logical pixels.</summary>
public readonly record struct PlatformViewTransform(double M11, double M12, double M21, double M22, double Dx, double Dy)
{
    public static PlatformViewTransform Identity => new(1, 0, 0, 1, 0, 0);
    public bool IsFinite => double.IsFinite(M11) && double.IsFinite(M12) && double.IsFinite(M21) &&
        double.IsFinite(M22) && double.IsFinite(Dx) && double.IsFinite(Dy);
    public bool IsAxisAligned => M12 == 0 && M21 == 0 && M11 > 0 && M22 > 0;
    public Offset Map(Offset point) => new(M11 * point.dx + M21 * point.dy + Dx, M12 * point.dx + M22 * point.dy + Dy);
    public PlatformViewTransform ThenLocal(PlatformViewTransform b) => new(
        M11 * b.M11 + M21 * b.M12, M12 * b.M11 + M22 * b.M12,
        M11 * b.M21 + M21 * b.M22, M12 * b.M21 + M22 * b.M22,
        M11 * b.Dx + M21 * b.Dy + Dx, M12 * b.Dx + M22 * b.Dy + Dy);
}

public sealed record PlatformViewPlacement(
    PlatformViewHandle Handle, Rect Bounds, PlatformViewTransform Transform,
    Rect? Clip, int PaintOrder, bool Visible = true)
{
    public void Validate()
    {
        if (!Bounds.isFinite || Bounds.width < 0 || Bounds.height < 0 || !Transform.IsFinite ||
            Clip is { } clip && !clip.isFinite)
            throw new ArgumentOutOfRangeException(nameof(Bounds), "PlatformView layout and clip must be finite and nonnegative.");
    }
}

public sealed record ScenePlatformViewPayload(PlatformViewHandle Handle, Rect Bounds);
public sealed record SceneInputShieldPayload(Rect Bounds, bool Debug);
public sealed record PlatformInputShield(Rect Bounds, PlatformViewTransform Transform, Rect? Clip, int PaintOrder, bool Debug);
public readonly record struct PlatformCompositionToken(ulong OwnerViewId, long ViewEpoch, long FrameNumber, long SurfaceGeneration,
    double DeviceScaleX = 1, double DeviceScaleY = 1);

/// <summary>Owner-local native view API. Missing registrations fail through DorotiCapabilityException.</summary>
public interface IPlatformViewHostCapability
{
    ulong OwnerViewId { get; }
    PlatformViewSupport QuerySupport(PlatformViewRequest request);
    ValueTask<PlatformViewHandle> CreateAsync(PlatformViewRequest request, CancellationToken cancellationToken = default);
    PlatformViewHandle Resolve(long instanceId);
    PlatformViewState GetState(PlatformViewHandle handle);
    ValueTask AttachAsync(PlatformViewPlacement placement, CancellationToken cancellationToken = default);
    ValueTask DetachAsync(PlatformViewHandle handle, CancellationToken cancellationToken = default);
    ValueTask SetFocusAsync(PlatformViewHandle handle, bool focused, CancellationToken cancellationToken = default);
    ValueTask DisposeAsync(PlatformViewHandle handle);
    event Action<PlatformViewHandle>? ViewFocused;
}
