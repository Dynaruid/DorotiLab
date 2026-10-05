#pragma once
#include <stdint.h>

#define DOROTI_D3D12_API extern "C" __declspec(dllexport) int32_t __cdecl

// Same-adapter D3D12 output. Returned resource/fence NT handles are caller-owned;
// slot availability events are borrowed. Vulkan releases images to EXTERNAL /
// GENERAL and signals the imported timeline before publishing their ready value.
DOROTI_D3D12_API doroti_windows_d3d12_output_create_v1(
    uint32_t luid_low, int32_t luid_high, void** context, uint64_t* fence_handle);
DOROTI_D3D12_API doroti_windows_d3d12_output_attach_v1(void* context, uint64_t hwnd);
DOROTI_D3D12_API doroti_windows_d3d12_output_alpha_v1(void* context, uint32_t enabled);
DOROTI_D3D12_API doroti_windows_d3d12_output_replace_v1(
    void* context, uint32_t slot, uint32_t width, uint32_t height,
    uint64_t* resource_handle, uint64_t* available_event);
DOROTI_D3D12_API doroti_windows_d3d12_output_available_v1(
    void* context, uint32_t slot, uint32_t* available);
DOROTI_D3D12_API doroti_windows_d3d12_output_ready_v1(
    void* context, uint32_t slot, uint64_t producer_value);
DOROTI_D3D12_API doroti_windows_d3d12_output_present_v1(
    void* context, uint32_t slot, uint32_t source_x, uint32_t source_y,
    uint32_t width, uint32_t height, uint64_t tag, uint32_t wait_for_frame,
    uint32_t timeout_ms, uint32_t* observed, uint64_t* present_id, uint64_t* completed);
// Drain proves only D3D12 copy completion. DXGI owns displayed back buffers.
DOROTI_D3D12_API doroti_windows_d3d12_output_drain_v1(
    void* context, uint64_t tag, uint64_t* completed);
DOROTI_D3D12_API doroti_windows_d3d12_output_destroy_v1(void* context);
DOROTI_D3D12_API doroti_windows_d3d12_output_raster_create_v1(
    void* context, uint64_t hwnd, void** raster);

struct doroti_windows_d3d12_output_snapshot_v1 {
  uint64_t submitted_copies, completed_copies, resource_allocations, swapchain_resizes;
  uint64_t debug_enabled, debug_errors, debug_warnings, active_swapchains;
};
DOROTI_D3D12_API doroti_windows_d3d12_output_snapshot_v1(
    void* context, struct doroti_windows_d3d12_output_snapshot_v1* snapshot);
