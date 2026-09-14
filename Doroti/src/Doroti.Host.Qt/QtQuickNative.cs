using System.Runtime.InteropServices;

namespace Doroti.Host.Qt;

internal static partial class QtQuickNative
{
    internal const ulong Feature = 1UL << 17;
    [StructLayout(LayoutKind.Sequential)]
    internal struct Gpu
    {
        internal uint Version, Size;
        internal nint Instance, Physical, Device, Queue;
        internal uint Family, ApiVersion;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct Part
    {
        internal uint Size, Kind;
        internal ulong Id, Image;
        internal uint PixelWidth, PixelHeight;
        internal QtPlatformViewHost.NativeRect Bounds, Clip;
    }
    [LibraryImport("doroti_qt_host", EntryPoint="doroti_qt_quick_get_gpu")]
    private static partial int GetGpu(nint window, ref Gpu gpu);
    [LibraryImport("doroti_qt_host", EntryPoint="doroti_qt_quick_commit")]
    private static unsafe partial int CommitParts(nint window, Part* parts, ulong count, uint apply);
    internal static unsafe Gpu Get(nint window)
    {
        var gpu = new Gpu { Version=1, Size=(uint)sizeof(Gpu) };
        QtPlatformViewHost.Check(GetGpu(window,ref gpu));
        if(gpu.Instance==0||gpu.Physical==0||gpu.Device==0||gpu.Queue==0)throw new InvalidDataException("Qt Quick GPU descriptor is incomplete.");
        return gpu;
    }
    internal static unsafe void Commit(nint window, Part[] parts, bool apply)
    {
        fixed(Part* data=parts)QtPlatformViewHost.Check(CommitParts(window,data,(ulong)parts.Length,apply?1u:0u));
    }
}
