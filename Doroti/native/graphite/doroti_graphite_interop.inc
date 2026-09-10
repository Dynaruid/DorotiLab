// Doroti NG1 bridge, compiled INSIDE the pinned libSkiaSharp translation unit.
// Never build this as a DLL that links a second copy of Skia.
#include "include/c/sk_graphite.h"
#include <cstdint>

extern "C" SK_C_API uint32_t doroti_graphite_interop_version() { return 3; }

#if defined(SK_GRAPHITE) && defined(SK_VULKAN)
#include "include/gpu/MutableTextureState.h"
#include "include/gpu/graphite/BackendSemaphore.h"
#include "include/gpu/vk/VulkanMutableTextureState.h"
#include "include/gpu/vk/VulkanExtensions.h"
#include "src/gpu/graphite/vk/VulkanGraphiteUtils.h"
#include "src/gpu/graphite/ContextPriv.h"
#pragma push_macro("interface")
#undef interface
#include "src/gpu/graphite/vk/VulkanSharedContext.h"
#pragma pop_macro("interface")
#include <vector>

// ABI 3: the host may observe device loss first on its external copy queue/fence.
// Forward that exact result to Graphite's shared context before resource release.
// This is not an abandon operation for an unknown/timeout result on a live GPU.
extern "C" SK_C_API bool doroti_graphite_vk_context_report_device_lost(sk_graphite_context_t* context) {
    if (!context || AsGraphiteContext(context)->backend() != skgpu::BackendApi::kVulkan) return false;
    auto* shared = static_cast<gr::VulkanSharedContext*>(AsGraphiteContext(context)->priv().sharedContext());
    shared->checkVkResult(VK_ERROR_DEVICE_LOST);
    return shared->isDeviceLost();
}

// ABI 2 is additive. Feature structures and extension arrays are borrowed only
// during creation. They must describe EXACTLY what vkCreateInstance/vkCreateDevice
// enabled, not a supported-feature query. Skia copies its caps and extension table.
// The procedure callback/user data remain borrowed until context destruction.
extern "C" SK_C_API sk_graphite_context_t* doroti_graphite_vk_context_create(
        const sk_graphite_vk_backend_context_init_t* init,
        const sk_graphite_context_options_t* options,
        const VkPhysicalDeviceFeatures* features,
        const VkPhysicalDeviceFeatures2* features2,
        uint32_t instanceCount, const char* const* instanceExtensions,
        uint32_t deviceCount, const char* const* deviceExtensions) {
    if (!init || !init->fInstance || !init->fPhysicalDevice || !init->fDevice ||
        !init->fQueue || !init->fGetProc || init->fProtectedContext ||
        (features && features2) || (instanceCount && !instanceExtensions) ||
        (deviceCount && !deviceExtensions) ||
        (features2 && features2->sType != VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FEATURES_2)) return nullptr;
    for (uint32_t i = 0; i < instanceCount; ++i) if (!instanceExtensions[i]) return nullptr;
    for (uint32_t i = 0; i < deviceCount; ++i) if (!deviceExtensions[i]) return nullptr;
    skgpu::VulkanBackendContext backend;
    backend.fInstance = reinterpret_cast<VkInstance>(init->fInstance);
    backend.fPhysicalDevice = reinterpret_cast<VkPhysicalDevice>(init->fPhysicalDevice);
    backend.fDevice = reinterpret_cast<VkDevice>(init->fDevice);
    backend.fQueue = reinterpret_cast<VkQueue>(init->fQueue);
    backend.fGraphicsQueueIndex = init->fGraphicsQueueIndex;
    backend.fMaxAPIVersion = init->fMaxAPIVersion;
    const auto getProc = init->fGetProc;
    const auto userData = init->fGetProcUserData;
    backend.fGetProc = [getProc, userData](const char* name, VkInstance instance, VkDevice device) {
        return reinterpret_cast<PFN_vkVoidFunction>(getProc(userData, name,
            reinterpret_cast<vk_instance_t*>(instance), reinterpret_cast<vk_device_t*>(device)));
    };
    skgpu::VulkanExtensions extensions;
    extensions.init(backend.fGetProc, backend.fInstance, backend.fPhysicalDevice,
                    instanceCount, instanceExtensions, deviceCount, deviceExtensions);
    backend.fVkExtensions = &extensions;
    backend.fDeviceFeatures = features;
    backend.fDeviceFeatures2 = features2;
    // Only the pinned built-in VMA allocator is supported. It sees the same
    // enabled-feature/extension descriptor as Graphite; no foreign allocator ABI.
    backend.fMemoryAllocator = skgpu::VulkanMemoryAllocators::Make(backend, skgpu::ThreadSafe::kNo);
    if (!backend.fMemoryAllocator) return nullptr;
    gr::ContextOptions nativeOptions;
    if (!sk_graphite_make_context_options(options, &nativeOptions)) return nullptr;
    return ToGraphiteContext(gr::ContextFactory::MakeVulkan(backend, nativeOptions).release());
}

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
extern "C" SK_C_API bool doroti_graphite_vk_context_report_device_lost(sk_graphite_context_t*) { return false; }
extern "C" SK_C_API sk_graphite_context_t* doroti_graphite_vk_context_create(
        const sk_graphite_vk_backend_context_init_t*, const sk_graphite_context_options_t*,
        const void*, const void*, uint32_t, const char* const*, uint32_t, const char* const*) { return nullptr; }
extern "C" SK_C_API bool doroti_graphite_vk_texture_get_state(const sk_graphite_backend_texture_t*, int32_t*, uint32_t*) { return false; }
extern "C" SK_C_API bool doroti_graphite_vk_texture_set_state(sk_graphite_backend_texture_t*, int32_t, uint32_t) { return false; }
extern "C" SK_C_API bool doroti_graphite_vk_insert_recording(sk_graphite_context_t*, sk_graphite_recording_t*, uint32_t, const uint64_t*, uint32_t, const uint64_t*) { return false; }
extern "C" SK_C_API bool doroti_graphite_has_unfinished_gpu_work(sk_graphite_context_t*) { return false; }
#endif
