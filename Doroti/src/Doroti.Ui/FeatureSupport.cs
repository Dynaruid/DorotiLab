namespace Doroti.Ui;

public sealed record FeatureSupport(bool Supported, string? Reason = null);

/// <summary>Query the registered owner provider again after attach/recreation/loss.</summary>
public sealed record GraphicsFeatureSupport(string Backend, long Generation = 0,
    bool SkSL = false, bool Wgsl = false, bool VariableBlur = false, long EffectBudgetBytes = 0,
    string? Reason = null);

public sealed record TextureFeatureSupport(bool CpuPixelUpload = true, bool NativeHandleImport = false,
    bool AndroidProducerSurface = false, bool BrowserVideo = false, NativeTextureDevice? Device = null,
    int MaximumExtent = 8192, NativeTextureFormat[]? Formats = null, string? Reason = null, long Generation = 0);

public sealed record SemanticsFeatureSupport(bool Hierarchy = false, bool TextGeometry = false,
    SemanticsRole[]? Roles = null, SemanticsAction[]? Actions = null, string? Reason = null);

public sealed record PlatformSceneItem(PlatformViewRequest View, Rect Bounds,
    PlatformViewTransform Transform, Rect? Clip = null, int PaintOrder = 0);
public sealed record PlatformSceneRequest(IReadOnlyList<PlatformViewRequest> Views,
    IReadOnlyList<PlatformSceneItem>? Items = null, bool HasForegroundRaster = false, bool HasBackdrop = false);
public sealed record PlatformSceneSupport(bool Supported, string? Reason = null);

public sealed record EffectAllocationSupport(bool Supported, Rect PhysicalBounds, long RequiredBytes,
    long AvailableBytes, string? Reason = null);

/// <summary>Conservative allocation preflight; never substitutes for backend allocation requirements/headroom.</summary>
public static class EffectAllocationPreflight
{
    public static EffectAllocationSupport Evaluate(Rect bounds, PlatformViewTransform transform,
        double dprX, double dprY, Rect? physicalClip, int bytesPerPixel, int intermediateImages,
        long availableBytes)
    {
        if (!bounds.IsFinite || !transform.IsFinite || !double.IsFinite(dprX) || !double.IsFinite(dprY)
            || dprX <= 0 || dprY <= 0 || bytesPerPixel < 1 || intermediateImages < 1 || availableBytes < 0
            || physicalClip is { IsFinite: false })
            return new(false, Rect.zero, 0, availableBytes, "Invalid device geometry or allocation policy.");
        var corners = new[] { bounds.topLeft, bounds.topRight, bounds.bottomLeft, bounds.bottomRight }
            .Select(transform.Map).Select(p => new Offset(p.dx * dprX, p.dy * dprY)).ToArray();
        if (corners.Any(p => !double.IsFinite(p.dx) || !double.IsFinite(p.dy)))
            return new(false, Rect.zero, 0, availableBytes, "Physical transform overflow.");
        var physical = Rect.fromLTRB(corners.Min(p => p.dx), corners.Min(p => p.dy),
            corners.Max(p => p.dx), corners.Max(p => p.dy));
        if (physicalClip is { } clip) physical = physical.intersect(clip);
        try
        {
            // Preserve negative coordinates: an offscreen target may cover them before final clip.
            var width = checked((long)Math.Ceiling(physical.right) - (long)Math.Floor(physical.left));
            var height = checked((long)Math.Ceiling(physical.bottom) - (long)Math.Floor(physical.top));
            var bytes = checked(Math.Max(0, width) * Math.Max(0, height) * bytesPerPixel * intermediateImages);
            return new(bytes <= availableBytes, physical, bytes, availableBytes,
                bytes <= availableBytes ? null : "Effect exceeds this backend's remaining allocation budget.");
        }
        catch (OverflowException) { return new(false, physical, long.MaxValue, availableBytes, "Effect allocation size overflow."); }
    }
}
