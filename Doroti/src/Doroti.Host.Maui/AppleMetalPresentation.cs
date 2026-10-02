#if IOS || MACCATALYST || MACOS
using CoreAnimation;
using Metal;
using ObjCRuntime;
using System.Runtime.InteropServices;

namespace Doroti.Host.Maui;

internal static class AppleMetalPresentation
{
    private static readonly Selector PresentedHandler = new("addPresentedHandler:");
    private static readonly Selector GpuEndTime = new("GPUEndTime");
    private static readonly Selector RespondsToSelector = new("respondsToSelector:");

    // Simulator/remote Metal drawables can omit the optional observation API.
    // GPU completion still owns retirement; missing scanout evidence is not a
    // reason to stop rendering or manufacture a displayed-frame timestamp.
    internal static bool CanObserve(ICAMetalDrawable drawable) =>
        Supports(drawable, PresentedHandler);

    internal static bool CanReadGpuEndTime(IMTLCommandBuffer buffer) =>
        Supports(buffer, GpuEndTime);

    // Protocol wrappers implement INativeObject without deriving from NSObject.
    // Query the actual Objective-C object so supported physical drawables retain
    // their display observations as well as rejecting unsupported simulators.
    private static bool Supports(INativeObject value, Selector selector) =>
        Responds(value.Handle, RespondsToSelector.Handle, selector.Handle);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Responds(nint receiver, nint selector, nint argument);
}
#endif
