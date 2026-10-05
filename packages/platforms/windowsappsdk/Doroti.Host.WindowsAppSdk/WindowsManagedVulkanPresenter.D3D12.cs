using System.Runtime.InteropServices;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using VkSemaphore = Silk.NET.Vulkan.Semaphore;

namespace Doroti.Host.WindowsAppSdk;

internal sealed unsafe partial class WindowsManagedVulkanPresenter
{
    private VkSemaphore _d3d12ProducerSemaphore;
    private ulong _d3d12ProducerValue;
    private D3D12OutputSnapshot? _lastD3D12Snapshot;

    private D3D12OutputSnapshot? D3D12Snapshot()
    {
        if (_presentationContext == 0)
        {
            return _lastD3D12Snapshot;
        }

        Marshal.ThrowExceptionForHR(GetD3D12Snapshot(_presentationContext, out var native));
        ResizeBuffersCount = native.SwapchainResizes;
        OperationalDebugErrorCount = native.DebugErrors;
        OperationalDebugWarningCount = native.DebugWarnings;
        return _lastD3D12Snapshot = new(
            native.SubmittedCopies,
            native.CompletedCopies,
            native.ResourceAllocations,
            native.SwapchainResizes,
            native.DebugEnabled != 0,
            native.DebugErrors,
            native.DebugWarnings,
            native.ActiveSwapchains,
            "D3D12 copy fence for shared-source reuse; DXGI owns back-buffer display retirement",
            "DXGI present-count statistics for prepared resize; DwmFlush boundary for ordinary admission"
        );
    }

    private void ImportD3D12ProducerFence(ulong sharedHandle)
    {
        var type = new SemaphoreTypeCreateInfo
        {
            SType = StructureType.SemaphoreTypeCreateInfo,
            SemaphoreType = SemaphoreType.Timeline,
        };
        var external = new PhysicalDeviceExternalSemaphoreInfo
        {
            SType = StructureType.PhysicalDeviceExternalSemaphoreInfo,
            PNext = &type,
            HandleType = ExternalSemaphoreHandleTypeFlags.D3D12FenceBit,
        };
        var properties = new ExternalSemaphoreProperties
        {
            SType = StructureType.ExternalSemaphoreProperties,
        };
        _vk.GetPhysicalDeviceExternalSemaphoreProperties(_physicalDevice, &external, &properties);
        if (
            (properties.ExternalSemaphoreFeatures & ExternalSemaphoreFeatureFlags.ImportableBit)
                == 0
            || (properties.CompatibleHandleTypes & ExternalSemaphoreHandleTypeFlags.D3D12FenceBit)
                == 0
        )
        {
            throw new PlatformNotSupportedException(
                "The selected Vulkan device cannot import a D3D12 timeline fence."
            );
        }

        var create = new SemaphoreCreateInfo
        {
            SType = StructureType.SemaphoreCreateInfo,
            PNext = &type,
        };
        Check(
            _vk.CreateSemaphore(_device, &create, null, out _d3d12ProducerSemaphore),
            "vkCreateSemaphore(D3D12 timeline)"
        );
        if (!_vk.TryGetDeviceExtension<KhrExternalSemaphoreWin32>(_instance, _device, out var api))
        {
            throw new PlatformNotSupportedException(
                "VK_KHR_external_semaphore_win32 is unavailable."
            );
        }

        using (api)
        {
            var import = new ImportSemaphoreWin32HandleInfoKHR
            {
                SType = StructureType.ImportSemaphoreWin32HandleInfoKhr,
                Semaphore = _d3d12ProducerSemaphore,
                HandleType = ExternalSemaphoreHandleTypeFlags.D3D12FenceBit,
                Handle = unchecked((nint)sharedHandle),
            };
            Check(
                api.ImportSemaphoreWin32Handle(_device, &import),
                "vkImportSemaphoreWin32HandleKHR(D3D12 fence)"
            );
        }
        _d3d12ProducerValue = 0;
        RecordEvent(
            "D3D12 output: same-LUID resources, shared timeline fence, DIRECT queue, DXGI flip-sequential"
        );
    }

    private void ReleaseD3D12ProducerFence()
    {
        if (_d3d12ProducerSemaphore.Handle != 0)
        {
            _vk.DestroySemaphore(_device, _d3d12ProducerSemaphore, null);
        }

        _d3d12ProducerSemaphore = default;
        _d3d12ProducerValue = 0;
    }

    [LibraryImport(
        WindowsNativeV1.LibraryName,
        EntryPoint = "doroti_windows_d3d12_output_ready_v1"
    )]
    private static partial int SetD3D12OutputReady(nint context, uint slot, ulong producerValue);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeD3D12Snapshot
    {
        internal ulong SubmittedCopies,
            CompletedCopies,
            ResourceAllocations,
            SwapchainResizes;
        internal ulong DebugEnabled,
            DebugErrors,
            DebugWarnings,
            ActiveSwapchains;
    }

    [LibraryImport(
        WindowsNativeV1.LibraryName,
        EntryPoint = "doroti_windows_d3d12_output_snapshot_v1"
    )]
    private static partial int GetD3D12Snapshot(nint context, out NativeD3D12Snapshot snapshot);
}

internal sealed record D3D12OutputSnapshot(
    ulong SubmittedCopies,
    ulong CompletedCopies,
    ulong ResourceAllocations,
    ulong SwapchainResizes,
    bool DebugEnabled,
    ulong DebugErrors,
    ulong DebugWarnings,
    ulong ActiveSwapchains,
    string Retirement,
    string DisplayWait
);
