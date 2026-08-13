namespace Doroti.Vendor.Avalonia.Skia;

// Doroti-owned R4 seam. Avalonia-derived files are tracked by the selection manifest.
internal readonly record struct NativeSurfaceDescriptor(ulong Generation, double Width, double Height);

internal static class VendorBoundary
{
    internal static string ContractVersion => "doroti.vendor.skia/r4";
}
