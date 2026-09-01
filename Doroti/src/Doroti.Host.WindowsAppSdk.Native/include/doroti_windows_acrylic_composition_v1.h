#pragma once

#include <stdint.h>

#if defined(_WIN32)
#define DOROTI_WINDOWS_ACRYLIC_API __declspec(dllexport)
#define DOROTI_WINDOWS_ACRYLIC_CALL __cdecl
#else
#define DOROTI_WINDOWS_ACRYLIC_API
#define DOROTI_WINDOWS_ACRYLIC_CALL
#endif

#ifdef __cplusplus
extern "C" {
#endif

#pragma pack(push, 8)

typedef struct doroti_windows_acrylic_probe_v1 {
  uint32_t abi_version;
  uint32_t struct_size;
  int32_t factory_hresult;
  int32_t manager_hresult;
  int32_t surface_handle_hresult;
  int32_t presentation_surface_hresult;
  int32_t retiring_fence_hresult;
  uint32_t device_creation_flags;
  uint32_t presentation_supported;
  uint32_t independent_flip_supported;
  int32_t adapter_luid_low;
  int32_t adapter_luid_high;
  uint32_t adapter_vendor_id;
  uint32_t adapter_device_id;
  uint64_t retiring_fence_completed_value;
} doroti_windows_acrylic_probe_v1;

typedef struct doroti_windows_acrylic_buffer_v1 {
  uint32_t abi_version;
  uint32_t struct_size;
  int32_t texture_hresult;
  int32_t add_buffer_hresult;
  int32_t available_event_hresult;
  uint32_t width;
  uint32_t height;
  uint32_t format;
  uint32_t bind_flags;
  uint32_t misc_flags;
  uint32_t initially_available;
} doroti_windows_acrylic_buffer_v1;

#pragma pack(pop)

DOROTI_WINDOWS_ACRYLIC_API int32_t DOROTI_WINDOWS_ACRYLIC_CALL
doroti_windows_acrylic_create_v1(
    void* d3d11_device, void** context, uint64_t* composition_surface_handle,
    doroti_windows_acrylic_probe_v1* snapshot);
DOROTI_WINDOWS_ACRYLIC_API void DOROTI_WINDOWS_ACRYLIC_CALL
doroti_windows_acrylic_destroy_v1(void* context);
DOROTI_WINDOWS_ACRYLIC_API int32_t DOROTI_WINDOWS_ACRYLIC_CALL
doroti_windows_acrylic_replace_buffer_v1(
    void* context, uint32_t slot_index, uint32_t width, uint32_t height,
    void** texture, uint64_t* available_event,
    doroti_windows_acrylic_buffer_v1* snapshot);
DOROTI_WINDOWS_ACRYLIC_API int32_t DOROTI_WINDOWS_ACRYLIC_CALL
doroti_windows_acrylic_is_available_v1(
    void* context, uint32_t slot_index, uint32_t* available);
DOROTI_WINDOWS_ACRYLIC_API int32_t DOROTI_WINDOWS_ACRYLIC_CALL
doroti_windows_acrylic_present_v1(
    void* context, uint32_t slot_index, uint32_t width, uint32_t height,
    uint64_t tag, uint64_t* present_id, uint64_t* retiring_fence_value);
DOROTI_WINDOWS_ACRYLIC_API int32_t DOROTI_WINDOWS_ACRYLIC_CALL
doroti_windows_acrylic_present_positioned_v1(
    void* context, uint32_t slot_index, uint32_t width, uint32_t height,
    float offset_x, float offset_y, uint64_t tag,
    uint64_t* present_id, uint64_t* retiring_fence_value);
DOROTI_WINDOWS_ACRYLIC_API int32_t DOROTI_WINDOWS_ACRYLIC_CALL
doroti_windows_acrylic_present_cropped_v1(
    void* context, uint32_t slot_index,
    uint32_t source_x, uint32_t source_y, uint32_t width, uint32_t height,
    uint64_t tag, uint64_t* present_id, uint64_t* retiring_fence_value);
DOROTI_WINDOWS_ACRYLIC_API int32_t DOROTI_WINDOWS_ACRYLIC_CALL
doroti_windows_acrylic_crop_v1(
    void* context, uint32_t source_x, uint32_t source_y,
    uint32_t width, uint32_t height, uint64_t tag);
DOROTI_WINDOWS_ACRYLIC_API int32_t DOROTI_WINDOWS_ACRYLIC_CALL
doroti_windows_acrylic_place_v1(
    void* context, float offset_x, float offset_y, uint64_t tag);

#ifdef __cplusplus
}
#endif
