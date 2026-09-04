#pragma once

#include <stdint.h>

#if defined(_WIN32)
#define DOROTI_WINDOWS_VULKAN_COMPOSITION_API __declspec(dllexport)
#define DOROTI_WINDOWS_VULKAN_COMPOSITION_CALL __cdecl
#else
#define DOROTI_WINDOWS_VULKAN_COMPOSITION_API
#define DOROTI_WINDOWS_VULKAN_COMPOSITION_CALL
#endif

#ifdef __cplusplus
extern "C" {
#endif

enum { DOROTI_WINDOWS_VULKAN_COMPOSITION_ABI_VERSION_V1 = 1 };
enum { DOROTI_WINDOWS_VULKAN_COMPOSITION_BUFFER_COUNT_V1 = 3 };

DOROTI_WINDOWS_VULKAN_COMPOSITION_API void
DOROTI_WINDOWS_VULKAN_COMPOSITION_CALL
doroti_windows_vulkan_composition_trace_prepared_v1(void);

#pragma pack(push, 8)

typedef struct doroti_windows_vulkan_composition_probe_v1 {
  uint32_t abi_version;
  uint32_t struct_size;
  int32_t dxgi_factory_hresult;
  int32_t adapter_enumeration_hresult;
  int32_t d3d11_device_hresult;
  int32_t presentation_factory_hresult;
  int32_t presentation_manager_hresult;
  int32_t surface_handle_hresult;
  int32_t presentation_surface_hresult;
  int32_t retiring_fence_hresult;
  int32_t requested_adapter_luid_low;
  int32_t requested_adapter_luid_high;
  int32_t actual_adapter_luid_low;
  int32_t actual_adapter_luid_high;
  uint32_t adapter_vendor_id;
  uint32_t adapter_device_id;
  uint32_t adapter_flags;
  uint32_t device_creation_flags;
  uint32_t device_feature_level;
  uint32_t adapter_luid_matched;
  uint32_t presentation_supported;
  uint32_t independent_flip_supported;
  uint64_t retiring_fence_completed_value;
} doroti_windows_vulkan_composition_probe_v1;

typedef struct doroti_windows_vulkan_composition_buffer_v1 {
  uint32_t abi_version;
  uint32_t struct_size;
  int32_t texture_hresult;
  int32_t dxgi_resource_hresult;
  int32_t shared_handle_hresult;
  int32_t add_buffer_hresult;
  int32_t available_event_hresult;
  uint32_t width;
  uint32_t height;
  uint32_t format;
  uint32_t bind_flags;
  uint32_t misc_flags;
  uint32_t initially_available;
} doroti_windows_vulkan_composition_buffer_v1;

#pragma pack(pop)

// Creates a D3D11 hardware device on the adapter identified by the exact LUID.
// The returned composition surface handle is borrowed from context and remains
// valid until doroti_windows_vulkan_composition_destroy_v1.
DOROTI_WINDOWS_VULKAN_COMPOSITION_API int32_t
DOROTI_WINDOWS_VULKAN_COMPOSITION_CALL
doroti_windows_vulkan_composition_create_v1(
    int32_t adapter_luid_low, int32_t adapter_luid_high, void** context,
    uint64_t* composition_surface_handle,
    doroti_windows_vulkan_composition_probe_v1* snapshot);

// Binds the Presentation surface handle to a product HWND through a native
// DirectComposition target. The current product passes its top-level HWND.
DOROTI_WINDOWS_VULKAN_COMPOSITION_API int32_t
DOROTI_WINDOWS_VULKAN_COMPOSITION_CALL
doroti_windows_vulkan_composition_attach_window_v1(
    void* context, uint64_t target_window);

// Selects premultiplied-alpha composition before the first Presentation
// buffer is committed. This lets Vulkan app pixels reveal a system backdrop
// owned by the non-topmost DesktopWindowTarget below the topmost native target.
DOROTI_WINDOWS_VULKAN_COMPOSITION_API int32_t
DOROTI_WINDOWS_VULKAN_COMPOSITION_CALL
doroti_windows_vulkan_composition_set_premultiplied_alpha_v1(
    void* context, uint32_t enabled);

DOROTI_WINDOWS_VULKAN_COMPOSITION_API void
DOROTI_WINDOWS_VULKAN_COMPOSITION_CALL
doroti_windows_vulkan_composition_destroy_v1(void* context);

// Replaces one of the three retained presentation buffers. shared_texture_handle
// is a caller-owned NT handle and must be closed after Vulkan imports it.
// available_event is borrowed from context and must not be closed by the caller.
DOROTI_WINDOWS_VULKAN_COMPOSITION_API int32_t
DOROTI_WINDOWS_VULKAN_COMPOSITION_CALL
doroti_windows_vulkan_composition_replace_buffer_v1(
    void* context, uint32_t slot_index, uint32_t width, uint32_t height,
    uint64_t* shared_texture_handle, uint64_t* available_event,
    doroti_windows_vulkan_composition_buffer_v1* snapshot);

DOROTI_WINDOWS_VULKAN_COMPOSITION_API int32_t
DOROTI_WINDOWS_VULKAN_COMPOSITION_CALL
doroti_windows_vulkan_composition_is_available_v1(
    void* context, uint32_t slot_index, uint32_t* available);

// The Vulkan producer must complete all writes to the selected texture before
// calling this function. The following IPresentationManager::Present call
// atomically selects a full-capacity identity source. The top-level HWND clips
// its exact viewport while app-background overscan remains around it.
// wait_for_composition_frame: 0 submits only; 1 retains the legacy DWM boundary
// path; 2 waits at most wait_timeout_ms for matching present ID + content tag
// CompositionFrame statistics with a display instance (no DwmFlush fallback).
DOROTI_WINDOWS_VULKAN_COMPOSITION_API int32_t
DOROTI_WINDOWS_VULKAN_COMPOSITION_CALL
doroti_windows_vulkan_composition_present_cropped_v1(
    void* context, uint32_t slot_index, uint32_t source_x,
    uint32_t source_y, uint32_t width, uint32_t height, uint64_t tag,
    uint32_t wait_for_composition_frame, uint32_t wait_timeout_ms,
    uint32_t* composition_frame_observed,
    uint64_t* present_id, uint64_t* retiring_fence_value);

// Re-commits the retained identity-transformed source of the currently bound
// buffer while a new Vulkan frame is still being produced.
DOROTI_WINDOWS_VULKAN_COMPOSITION_API int32_t
DOROTI_WINDOWS_VULKAN_COMPOSITION_CALL
doroti_windows_vulkan_composition_crop_v1(
    void* context, uint32_t source_x, uint32_t source_y,
    uint32_t width, uint32_t height, uint64_t tag);

// Commits a native-only drain buffer so the final Vulkan-imported buffer can
// retire. A caller must do this before waiting for every imported-buffer
// availability event during reset or shutdown.
DOROTI_WINDOWS_VULKAN_COMPOSITION_API int32_t
DOROTI_WINDOWS_VULKAN_COMPOSITION_CALL
doroti_windows_vulkan_composition_retire_buffers_v1(
    void* context, uint64_t tag, uint64_t* present_id);

#ifdef __cplusplus
}
#endif
