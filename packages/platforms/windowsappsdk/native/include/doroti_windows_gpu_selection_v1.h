#pragma once

#include "doroti_windows_host_v1.h"

#ifdef __cplusplus
extern "C" {
#endif

// preference: 0 = system default, 1 = minimum power, 2 = high performance.
// eligible_luids uses Windows LUID bits (LowPart in bits 0..31).
// An empty eligibility list accepts any hardware adapter. Returns an HRESULT.
DOROTI_WINDOWS_API int32_t DOROTI_WINDOWS_CALL
doroti_windows_gpu_select_adapter_v1(
    uint32_t preference, const uint64_t* eligible_luids,
    uint32_t eligible_count, uint64_t* selected_luid);

#ifdef __cplusplus
}
#endif
