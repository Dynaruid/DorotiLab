#pragma once
#include <stdint.h>
extern "C" __declspec(dllexport) int32_t __cdecl doroti_windows_composition_raster_update_v1(
    void* raster, const void* pixels, uint32_t width, uint32_t height, uint32_t row_bytes);
extern "C" __declspec(dllexport) void __cdecl doroti_windows_composition_raster_destroy_v1(void* raster);
extern "C" __declspec(dllexport) void __cdecl doroti_windows_d3d12_output_trace_prepared_v1(void);
