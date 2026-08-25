#pragma once

#include <stdint.h>

#if defined(_WIN32)
#define DOROTI_MANAGED_PRESENTER_PROBE_API __declspec(dllexport)
#define DOROTI_MANAGED_PRESENTER_PROBE_CALL __cdecl
#else
#define DOROTI_MANAGED_PRESENTER_PROBE_API
#define DOROTI_MANAGED_PRESENTER_PROBE_CALL
#endif

enum { DOROTI_MANAGED_PRESENTER_PROBE_ABI_V1 = 1 };

#pragma pack(push, 8)
typedef uint32_t(DOROTI_MANAGED_PRESENTER_PROBE_CALL*
                     doroti_managed_presenter_request_resize_v1)(
    void* host_context, uint32_t width_px, uint32_t height_px,
    uint64_t generation);

typedef struct doroti_managed_presenter_host_v1 {
  uint32_t abi_version;
  uint32_t struct_size;
  void* host_context;
  void* top_level_hwnd;
  void* child_hwnd;
  void* task_hwnd;
  doroti_managed_presenter_request_resize_v1 request_resize;
} doroti_managed_presenter_host_v1;

typedef uint32_t(DOROTI_MANAGED_PRESENTER_PROBE_CALL*
                     doroti_managed_presenter_run_callback_v1)(
    void* callback_context, const doroti_managed_presenter_host_v1* host);

typedef struct doroti_managed_presenter_callbacks_v1 {
  uint32_t abi_version;
  uint32_t struct_size;
  void* callback_context;
  doroti_managed_presenter_run_callback_v1 run_presenter;
} doroti_managed_presenter_callbacks_v1;

typedef struct doroti_managed_presenter_probe_result_v1 {
  uint32_t abi_version;
  uint32_t struct_size;
  uint32_t status;
  uint32_t reserved;
  uint64_t platform_thread_id;
  uint64_t presenter_thread_id;
  uint64_t top_level_created_count;
  uint64_t child_created_count;
  uint64_t task_window_created_count;
  uint64_t resize_command_count;
  uint64_t task_dispatch_count;
  uint64_t child_extent_mismatch_count;
  uint64_t callback_status;
  uint32_t gdi_start;
  uint32_t gdi_end;
  uint32_t user_start;
  uint32_t user_end;
} doroti_managed_presenter_probe_result_v1;

typedef struct doroti_filtered_wait_probe_result_v1 {
  uint32_t abi_version;
  uint32_t struct_size;
  uint32_t status;
  uint32_t reserved;
  uint64_t successful_wait_count;
  uint64_t timeout_wait_count;
  uint64_t task_completion_dispatch_count;
  uint64_t top_level_recursive_dispatch_count;
  uint64_t child_recursive_dispatch_count;
  uint64_t maximum_wait_elapsed_ms;
  uint32_t gdi_start;
  uint32_t gdi_end;
  uint32_t user_start;
  uint32_t user_end;
} doroti_filtered_wait_probe_result_v1;
#pragma pack(pop)

extern "C" DOROTI_MANAGED_PRESENTER_PROBE_API uint32_t
DOROTI_MANAGED_PRESENTER_PROBE_CALL doroti_run_managed_presenter_probe_v1(
    const doroti_managed_presenter_callbacks_v1* callbacks,
    doroti_managed_presenter_probe_result_v1* result);

extern "C" DOROTI_MANAGED_PRESENTER_PROBE_API uint32_t
DOROTI_MANAGED_PRESENTER_PROBE_CALL doroti_run_filtered_wait_probe_v1(
    doroti_filtered_wait_probe_result_v1* result);
