// Doroti NG1 bridge, compiled INSIDE the pinned libSkiaSharp translation unit.
// Never build this as a DLL that links a second copy of Skia.
#include "include/c/sk_graphite.h"
#include <cstdint>

extern "C" SK_C_API uint32_t doroti_graphite_interop_version() { return 1; }

#if defined(SK_GRAPHITE) && defined(SK_VULKAN)
#include "include/gpu/MutableTextureState.h"
#include "include/gpu/graphite/BackendSemaphore.h"
#include "include/gpu/vk/VulkanMutableTextureState.h"
#include "src/gpu/graphite/vk/VulkanGraphiteUtils.h"
#include <vector>

extern "C" SK_C_API bool doroti_graphite_vk_texture_get_state(
        const sk_graphite_backend_texture_t* texture, int32_t* layout, uint32_t* family) {
    if (!texture || !layout || !family) return false;
    const auto* native = AsGraphiteBackendTexture(texture);
    if (!native->isValid() || native->backend() != skgpu::BackendApi::kVulkan) return false;
    *layout = static_cast<int32_t>(gr::BackendTextures::GetVkImageLayout(*native));
    *family = gr::BackendTextures::GetVkQueueFamilyIndex(*native);
    return true;
}

// This updates Skia's tracking ONLY. The caller must already have queued the
// matching Vulkan barrier and synchronized all access on the owning queue.
extern "C" SK_C_API bool doroti_graphite_vk_texture_set_state(
        sk_graphite_backend_texture_t* texture, int32_t layout, uint32_t family) {
    if (!texture) return false;
    auto* native = AsGraphiteBackendTexture(texture);
    if (!native->isValid() || native->backend() != skgpu::BackendApi::kVulkan) return false;
    gr::BackendTextures::SetMutableState(native,
        skgpu::MutableTextureStates::MakeVulkan(static_cast<VkImageLayout>(layout), family));
    return true;
}

// The caller owns the underlying semaphores through their GPU/presentation
// consumption. BackendSemaphore wrappers may die after insertRecording returns.
extern "C" SK_C_API bool doroti_graphite_vk_insert_recording(
        sk_graphite_context_t* context, sk_graphite_recording_t* recording,
        uint32_t waitCount, const uint64_t* waits, uint32_t signalCount, const uint64_t* signals) {
    if (!context || !recording || (waitCount && !waits) || (signalCount && !signals)) return false;
    if (AsGraphiteContext(context)->backend() != skgpu::BackendApi::kVulkan) return false;
    std::vector<gr::BackendSemaphore> waitSemaphores, signalSemaphores;
    for (uint32_t i = 0; i < waitCount; ++i)
        waitSemaphores.push_back(gr::BackendSemaphores::MakeVulkan((VkSemaphore)waits[i]));
    for (uint32_t i = 0; i < signalCount; ++i)
        signalSemaphores.push_back(gr::BackendSemaphores::MakeVulkan((VkSemaphore)signals[i]));
    gr::InsertRecordingInfo info;
    info.fRecording = AsGraphiteRecording(recording);
    info.fNumWaitSemaphores = waitCount;
    info.fWaitSemaphores = waitSemaphores.data();
    info.fNumSignalSemaphores = signalCount;
    info.fSignalSemaphores = signalSemaphores.data();
    return AsGraphiteContext(context)->insertRecording(info) == gr::InsertStatus::kSuccess;
}

// Polling completion avoids crossing native callbacks with an unrooted managed
// delegate. False after Submit is GPU completion, never platform presentation.
extern "C" SK_C_API bool doroti_graphite_has_unfinished_gpu_work(sk_graphite_context_t* context) {
    return context && AsGraphiteContext(context)->hasUnfinishedGpuWork();
}
#else
extern "C" SK_C_API bool doroti_graphite_vk_texture_get_state(const sk_graphite_backend_texture_t*, int32_t*, uint32_t*) { return false; }
extern "C" SK_C_API bool doroti_graphite_vk_texture_set_state(sk_graphite_backend_texture_t*, int32_t, uint32_t) { return false; }
extern "C" SK_C_API bool doroti_graphite_vk_insert_recording(sk_graphite_context_t*, sk_graphite_recording_t*, uint32_t, const uint64_t*, uint32_t, const uint64_t*) { return false; }
extern "C" SK_C_API bool doroti_graphite_has_unfinished_gpu_work(sk_graphite_context_t*) { return false; }
#endif
