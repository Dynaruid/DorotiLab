using Doroti.Composition;
using Doroti.Graphics;
using Doroti.Vendor.Avalonia.Skia;

namespace Doroti.Backends.Skia;

/// <summary>Converts internal vendor surface metadata into backend-neutral Doroti contracts.</summary>
public static class SkiaAdapterBoundary
{
    public static string ContractVersion => VendorBoundary.ContractVersion;

    internal static (SurfaceGeneration Generation, Size PixelSize) Convert(NativeSurfaceDescriptor input) =>
        (new(input.Generation), new(input.Width, input.Height));
}
