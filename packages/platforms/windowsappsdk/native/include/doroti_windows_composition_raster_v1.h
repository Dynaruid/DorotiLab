#pragma once
#include <stdint.h>
struct DorotiCompositionSourceV1 {
    void* raster;
    uint64_t hwnd;
    uint64_t generation;
    float x, y, left, top, right, bottom;
};
extern "C" __declspec(dllexport) int32_t __cdecl doroti_windows_composition_raster_create_shared_v1(
    void* shared, uint64_t hwnd, void** raster);
extern "C" __declspec(dllexport) int32_t __cdecl doroti_windows_composition_backdrop_update_v1(
    void* raster, const DorotiCompositionSourceV1* sources, uint32_t count,
    float left, float top, float right, float bottom, float sigma);
extern "C" __declspec(dllexport) int32_t __cdecl doroti_windows_composition_raster_update_v1(
    void* raster, const void* pixels, uint32_t width, uint32_t height, uint32_t row_bytes);
extern "C" __declspec(dllexport) int32_t __cdecl doroti_windows_composition_raster_update_region_v1(
    void* raster, const void* pixels, uint32_t width, uint32_t height, uint32_t row_bytes, int32_t x, int32_t y);
extern "C" __declspec(dllexport) int32_t __cdecl doroti_windows_composition_commit_v1(void* raster);
extern "C" __declspec(dllexport) int32_t __cdecl doroti_windows_composition_wait_commit_v1(void* raster);
extern "C" __declspec(dllexport) int32_t __cdecl doroti_windows_composition_scene_update_v1(
    void* raster, const DorotiCompositionSourceV1* sources, uint32_t count, float width, float height);
extern "C" __declspec(dllexport) void __cdecl doroti_windows_composition_raster_destroy_v1(void* raster);
extern "C" __declspec(dllexport) void __cdecl doroti_windows_d3d12_output_trace_prepared_v1(void);
