using System.Runtime.InteropServices;
namespace Doroti.Validation.WindowsVulkanCapability;
internal static unsafe partial class Program
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    private struct VulkanCompositionProbe
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal int DxgiFactoryHresult;
        internal int AdapterEnumerationHresult;
        internal int D3D11DeviceHresult;
        internal int PresentationFactoryHresult;
        internal int PresentationManagerHresult;
        internal int SurfaceHandleHresult;
        internal int PresentationSurfaceHresult;
        internal int RetiringFenceHresult;
        internal int RequestedAdapterLuidLow;
        internal int RequestedAdapterLuidHigh;
        internal int ActualAdapterLuidLow;
        internal int ActualAdapterLuidHigh;
        internal uint AdapterVendorId;
        internal uint AdapterDeviceId;
        internal uint AdapterFlags;
        internal uint DeviceCreationFlags;
        internal uint DeviceFeatureLevel;
        internal uint AdapterLuidMatched;
        internal uint PresentationSupported;
        internal uint IndependentFlipSupported;
        internal ulong RetiringFenceCompletedValue;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    private struct VulkanCompositionBuffer
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal int TextureHresult;
        internal int DxgiResourceHresult;
        internal int SharedHandleHresult;
        internal int AddBufferHresult;
        internal int AvailableEventHresult;
        internal uint Width;
        internal uint Height;
        internal uint Format;
        internal uint BindFlags;
        internal uint MiscFlags;
        internal uint InitiallyAvailable;
    }

    [LibraryImport("doroti_windows_appsdk_host_v1",
        EntryPoint = "doroti_windows_vulkan_composition_create_v1")]
    private static partial int CreateComposition(
        uint adapterLuidLow, int adapterLuidHigh,
        out nint context, out ulong compositionSurfaceHandle,
        ref VulkanCompositionProbe snapshot);

    [LibraryImport("doroti_windows_appsdk_host_v1",
        EntryPoint = "doroti_windows_vulkan_composition_set_premultiplied_alpha_v1")]
    private static partial int SetCompositionPremultipliedAlpha(
        nint context, uint enabled);

    [LibraryImport("doroti_windows_appsdk_host_v1",
        EntryPoint = "doroti_windows_vulkan_composition_attach_window_v1")]
    private static partial int AttachCompositionWindow(
        nint context, ulong targetWindow);

    [LibraryImport("doroti_windows_appsdk_host_v1",
        EntryPoint = "doroti_windows_vulkan_composition_destroy_v1")]
    private static partial void DestroyComposition(nint context);

    [LibraryImport("doroti_windows_appsdk_host_v1",
        EntryPoint = "doroti_windows_vulkan_composition_replace_buffer_v1")]
    private static partial int ReplaceCompositionBuffer(
        nint context, uint slotIndex, uint width, uint height,
        out ulong sharedTextureHandle, out ulong availableEvent,
        ref VulkanCompositionBuffer snapshot);

    [LibraryImport("doroti_windows_appsdk_host_v1",
        EntryPoint = "doroti_windows_vulkan_composition_is_available_v1")]
    private static partial int IsCompositionBufferAvailable(
        nint context, uint slotIndex, out uint available);

    [LibraryImport("doroti_windows_appsdk_host_v1",
        EntryPoint = "doroti_windows_vulkan_composition_present_cropped_v1")]
    private static partial int PresentCropped(
        nint context, uint slotIndex,
        uint sourceX, uint sourceY, uint width, uint height, ulong tag,
        uint waitForCompositionFrame, uint waitTimeoutMilliseconds,
        out uint compositionFrameObserved,
        out ulong presentId, out ulong retiringFenceValue);

    [LibraryImport("doroti_windows_appsdk_host_v1",
        EntryPoint = "doroti_windows_vulkan_composition_retire_buffers_v1")]
    private static partial int UnbindCompositionBuffer(
        nint context, ulong tag, out ulong presentId);

}
