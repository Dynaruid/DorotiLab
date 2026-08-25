#pragma once

#include <stdint.h>

#include "doroti_windows_host_v1.h"

#if defined(_WIN32)
#define DOROTI_LEASE_PROBE_API __declspec(dllexport)
#else
#define DOROTI_LEASE_PROBE_API
#endif

#pragma pack(push, 8)
typedef struct doroti_windows_lease_probe_result_v1 {
  uint32_t abi_version;
  uint32_t struct_size;
  uint32_t status;
  uint32_t reserved;
  uint64_t context_acquire_count;
  uint64_t context_release_count;
  uint64_t render_callback_count;
  uint64_t presented_count;
  uint64_t superseded_count;
  uint64_t failed_count;
  uint64_t fence_after_submit_count;
  uint64_t resize_buffers_count;
  uint64_t resize_invalid_call_count;
  uint64_t per_frame_reference_leak_count;
  uint64_t debug_error_count;
  uint64_t debug_corruption_count;
  int32_t live_object_report_hresult;
  uint32_t reserved2;
} doroti_windows_lease_probe_result_v1;
#pragma pack(pop)

extern "C" DOROTI_LEASE_PROBE_API uint32_t DOROTI_WINDOWS_CALL
doroti_windows_run_lease_probe_v1(
    const doroti_windows_callbacks_v1* callbacks,
    doroti_windows_lease_probe_result_v1* result);
