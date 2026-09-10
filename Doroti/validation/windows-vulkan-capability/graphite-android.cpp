// NG1 Android C ABI/device probe. No Activity, Surface or product renderer is replaced.
#include <vulkan/vulkan.h>
#include <dlfcn.h>
#include "include/c/sk_graphite_vulkan.h"
#include "include/c/sk_surface.h"
#include "include/c/sk_canvas.h"
#include "include/c/sk_image.h"
#include <chrono>
#include <cstdio>
#include <cstring>
#include <stdexcept>
#include <thread>
#include <vector>

extern "C" uint32_t doroti_graphite_interop_version();
extern "C" sk_graphite_context_t* doroti_graphite_vk_context_create(
    const sk_graphite_vk_backend_context_init_t*, const sk_graphite_context_options_t*,
    const VkPhysicalDeviceFeatures*, const VkPhysicalDeviceFeatures2*,
    uint32_t, const char* const*, uint32_t, const char* const*);
extern "C" bool doroti_graphite_vk_texture_get_state(const sk_graphite_backend_texture_t*, int32_t*, uint32_t*);
extern "C" bool doroti_graphite_has_unfinished_gpu_work(sk_graphite_context_t*);
extern "C" bool doroti_graphite_vk_context_report_device_lost(sk_graphite_context_t*);

static void require(bool condition, const char* operation) {
    if (!condition) throw std::runtime_error(operation);
}
static void check(VkResult result, const char* operation) {
    if (result != VK_SUCCESS) {
        std::fprintf(stderr, "%s: VkResult=%d\n", operation, result);
        throw std::runtime_error(operation);
    }
}

struct Readback {
    bool done = false;
    bool valid = false;
    int size;
    unsigned char red, blue;
};

struct Generation {
    Readback capture{};
    VkInstance instance{};
    VkDevice device{};
    VkImage image{};
    VkDeviceMemory memory{};
    sk_graphite_context_t* context{};
    sk_graphite_recorder_t* recorder{};
    sk_graphite_backend_texture_t* texture{};
    sk_surface_t* surface{};
    sk_graphite_recording_t* recording{};
    ~Generation() {
        // Diagnostic teardown only. The on-device and host runners both impose
        // an external timeout. No device-idle is used in the frame loop.
        if (device) vkDeviceWaitIdle(device);
        if (context) sk_graphite_context_check_async_work_completion(context);
        if (recording) sk_graphite_recording_delete(recording);
        if (surface) sk_surface_unref(surface);
        if (texture) sk_graphite_backend_texture_delete(texture);
        if (recorder) sk_graphite_recorder_delete(recorder);
        if (context) sk_graphite_context_delete(context);
        if (image) vkDestroyImage(device, image, nullptr);
        if (memory) vkFreeMemory(device, memory, nullptr);
        if (device) vkDestroyDevice(device, nullptr);
        if (instance) vkDestroyInstance(instance, nullptr);
    }
};

static void readback(void* user, const sk_image_async_read_result_t* result) {
    auto& state = *static_cast<Readback*>(user);
    state.done = true;
    if (!result || sk_image_async_read_result_get_count(result) != 1) return;
    const auto* pixels = static_cast<const unsigned char*>(sk_image_async_read_result_get_data(result, 0));
    const auto stride = sk_image_async_read_result_get_row_bytes(result, 0);
    if (!pixels || stride < static_cast<size_t>(state.size * 4)) return;
    for (int y = 0; y < state.size; ++y)
        for (int x = 0; x < state.size; ++x) {
            const auto* p = pixels + y * stride + x * 4;
            if (p[0] != state.red || p[1] != 0 || p[2] != state.blue || p[3] != 255) return;
        }
    state.valid = true;
}

static void run(int ordinal) {
    Generation owner;
    VkApplicationInfo application{VK_STRUCTURE_TYPE_APPLICATION_INFO};
    application.pApplicationName = "Doroti Graphite ABI 3 diagnostic";
    application.apiVersion = VK_API_VERSION_1_1;
    VkInstanceCreateInfo instanceInfo{VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO};
    instanceInfo.pApplicationInfo = &application;
    check(vkCreateInstance(&instanceInfo, nullptr, &owner.instance), "vkCreateInstance");
    uint32_t count = 0;
    check(vkEnumeratePhysicalDevices(owner.instance, &count, nullptr), "vkEnumeratePhysicalDevices");
    require(count == 1, "Select exactly one physical GPU for this diagnostic");
    VkPhysicalDevice physical{};
    check(vkEnumeratePhysicalDevices(owner.instance, &count, &physical), "vkEnumeratePhysicalDevices(device)");
    VkPhysicalDeviceProperties properties{};
    vkGetPhysicalDeviceProperties(physical, &properties);
    require(properties.deviceType != VK_PHYSICAL_DEVICE_TYPE_CPU && properties.apiVersion >= VK_API_VERSION_1_1,
        "Hardware Vulkan 1.1 is required");
    std::printf("generation=%d device=%s vendor=%u deviceId=%u driver=%u api=%u\n",
        ordinal, properties.deviceName, properties.vendorID, properties.deviceID, properties.driverVersion, properties.apiVersion);
    vkGetPhysicalDeviceQueueFamilyProperties(physical, &count, nullptr);
    std::vector<VkQueueFamilyProperties> queues(count);
    vkGetPhysicalDeviceQueueFamilyProperties(physical, &count, queues.data());
    uint32_t family = count;
    for (uint32_t i = 0; i < count; ++i) if (queues[i].queueFlags & VK_QUEUE_GRAPHICS_BIT) { family = i; break; }
    require(family < count, "No graphics queue");
    auto getFeatures = reinterpret_cast<PFN_vkGetPhysicalDeviceFeatures2>(vkGetInstanceProcAddr(owner.instance, "vkGetPhysicalDeviceFeatures2"));
    require(getFeatures != nullptr, "Missing Vulkan 1.1 Features2 procedure");
    VkPhysicalDevice16BitStorageFeatures storage{VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_16BIT_STORAGE_FEATURES};
    VkPhysicalDeviceFeatures2 supported{VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FEATURES_2};
    supported.pNext = &storage;
    getFeatures(physical, &supported);
    VkPhysicalDevice16BitStorageFeatures enabledStorage{VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_16BIT_STORAGE_FEATURES};
    enabledStorage.storageBuffer16BitAccess = storage.storageBuffer16BitAccess;
    VkPhysicalDeviceFeatures2 enabled{VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FEATURES_2};
    enabled.pNext = &enabledStorage;
    enabled.features.robustBufferAccess = supported.features.robustBufferAccess;
    const char* extensions[] = {"VK_KHR_maintenance1", "VK_KHR_driver_properties"};
    float priority = 1;
    VkDeviceQueueCreateInfo queueInfo{VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO};
    queueInfo.queueFamilyIndex = family; queueInfo.queueCount = 1; queueInfo.pQueuePriorities = &priority;
    VkDeviceCreateInfo deviceInfo{VK_STRUCTURE_TYPE_DEVICE_CREATE_INFO};
    deviceInfo.pNext = &enabled; deviceInfo.queueCreateInfoCount = 1; deviceInfo.pQueueCreateInfos = &queueInfo;
    deviceInfo.enabledExtensionCount = 2; deviceInfo.ppEnabledExtensionNames = extensions;
    check(vkCreateDevice(physical, &deviceInfo, nullptr, &owner.device), "vkCreateDevice");
    VkQueue queue{};
    vkGetDeviceQueue(owner.device, family, 0, &queue);
    sk_graphite_vk_backend_context_init_t backend{};
    backend.fInstance = reinterpret_cast<vk_instance_t*>(owner.instance);
    backend.fPhysicalDevice = reinterpret_cast<vk_physical_device_t*>(physical);
    backend.fDevice = reinterpret_cast<vk_device_t*>(owner.device);
    backend.fQueue = reinterpret_cast<vk_queue_t*>(queue);
    backend.fGraphicsQueueIndex = family; backend.fMaxAPIVersion = VK_API_VERSION_1_1;
    backend.fGetProc = [](void*, const char* name, vk_instance_t* instance, vk_device_t* device) {
        return reinterpret_cast<sk_graphite_vk_func_ptr>(device
            ? vkGetDeviceProcAddr(reinterpret_cast<VkDevice>(device), name)
            : vkGetInstanceProcAddr(reinterpret_cast<VkInstance>(instance), name));
    };
    sk_graphite_context_options_t contextOptions{};
    sk_graphite_context_options_init_defaults(&contextOptions);
    contextOptions.fGpuBudgetInBytes = 256LL * 1024 * 1024;
    owner.context = doroti_graphite_vk_context_create(&backend, &contextOptions, nullptr, &enabled, 0, nullptr, 2, extensions);
    require(owner.context != nullptr, "ABI 3 Graphite context creation failed");
    owner.recorder = sk_graphite_context_make_recorder(owner.context, 64LL * 1024 * 1024, nullptr);
    require(owner.recorder != nullptr, "Recorder creation failed");
    const int size = 32 + 32 * ordinal;
    const VkImageUsageFlags usage = VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT | VK_IMAGE_USAGE_INPUT_ATTACHMENT_BIT |
        VK_IMAGE_USAGE_SAMPLED_BIT | VK_IMAGE_USAGE_TRANSFER_SRC_BIT | VK_IMAGE_USAGE_TRANSFER_DST_BIT;
    VkImageCreateInfo imageInfo{VK_STRUCTURE_TYPE_IMAGE_CREATE_INFO};
    imageInfo.imageType = VK_IMAGE_TYPE_2D; imageInfo.format = VK_FORMAT_R8G8B8A8_UNORM;
    imageInfo.extent = {static_cast<uint32_t>(size), static_cast<uint32_t>(size), 1};
    imageInfo.mipLevels = 1; imageInfo.arrayLayers = 1; imageInfo.samples = VK_SAMPLE_COUNT_1_BIT;
    imageInfo.tiling = VK_IMAGE_TILING_OPTIMAL; imageInfo.usage = usage;
    check(vkCreateImage(owner.device, &imageInfo, nullptr, &owner.image), "vkCreateImage");
    VkMemoryRequirements requirements{};
    vkGetImageMemoryRequirements(owner.device, owner.image, &requirements);
    VkPhysicalDeviceMemoryProperties memoryProperties{};
    vkGetPhysicalDeviceMemoryProperties(physical, &memoryProperties);
    uint32_t memoryType = memoryProperties.memoryTypeCount;
    for (uint32_t i = 0; i < memoryProperties.memoryTypeCount; ++i)
        if ((requirements.memoryTypeBits & (1u << i)) && (memoryProperties.memoryTypes[i].propertyFlags & VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT)) {
            memoryType = i; break;
        }
    require(memoryType < memoryProperties.memoryTypeCount, "No device-local memory");
    VkMemoryAllocateInfo allocate{VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_INFO};
    allocate.allocationSize = requirements.size; allocate.memoryTypeIndex = memoryType;
    check(vkAllocateMemory(owner.device, &allocate, nullptr, &owner.memory), "vkAllocateMemory");
    check(vkBindImageMemory(owner.device, owner.image, owner.memory, 0), "vkBindImageMemory");
    sk_graphite_vk_texture_info_t textureInfo{};
    textureInfo.fSampleCount = 1; textureInfo.fFormat = VK_FORMAT_R8G8B8A8_UNORM;
    textureInfo.fImageTiling = VK_IMAGE_TILING_OPTIMAL; textureInfo.fImageUsageFlags = usage;
    textureInfo.fSharingMode = VK_SHARING_MODE_EXCLUSIVE; textureInfo.fAspectMask = VK_IMAGE_ASPECT_COLOR_BIT;
    owner.texture = sk_graphite_vk_backend_texture_new(size, size, &textureInfo, VK_IMAGE_LAYOUT_UNDEFINED,
        family, reinterpret_cast<void*>(owner.image));
    require(owner.texture != nullptr, "Texture wrapper failed");
    owner.surface = sk_graphite_surface_wrap_backend_texture(owner.recorder, owner.texture,
        RGBA_8888_SK_COLORTYPE, nullptr, nullptr, nullptr, nullptr);
    require(owner.surface != nullptr, "Graphite external surface failed");
    // State object outlives context callbacks even if a frame throws.
    auto& capture = owner.capture;
    for (int frame = 0; frame < 12; ++frame) {
        capture = {false, false, size, static_cast<unsigned char>(frame % 2 == 0 ? 255 : 0),
            static_cast<unsigned char>(frame % 2 == 0 ? 0 : 255)};
        sk_canvas_clear(sk_surface_get_canvas(owner.surface), frame % 2 == 0 ? 0xffff0000u : 0xff0000ffu);
        owner.recording = sk_graphite_recorder_snap(owner.recorder);
        require(owner.recording != nullptr, "Snap failed");
        sk_graphite_insert_recording_info_t insert{}; insert.fRecording = owner.recording;
        require(sk_graphite_context_insert_recording(owner.context, &insert) == SUCCESS_SK_GRAPHITE_INSERT_STATUS, "Insert failed");
        sk_imageinfo_t info{nullptr, size, size, RGBA_8888_SK_COLORTYPE, PREMUL_SK_ALPHATYPE};
        sk_irect_t rect{0, 0, size, size};
        sk_graphite_context_async_rescale_and_read_pixels_surface(owner.context, owner.surface, &info, &rect,
            SRC_SK_IMAGE_RESCALE_GAMMA, NEAREST_SK_IMAGE_RESCALE_MODE, readback, &capture);
        sk_graphite_submit_info_t submit{};
        require(sk_graphite_context_submit(owner.context, &submit), "Asynchronous submit failed");
        const auto deadline = std::chrono::steady_clock::now() + std::chrono::seconds(10);
        while (!capture.done && std::chrono::steady_clock::now() < deadline) {
            sk_graphite_context_check_async_work_completion(owner.context);
            std::this_thread::sleep_for(std::chrono::milliseconds(1));
        }
        require(capture.done && capture.valid, "Asynchronous readback pixel contract failed");
        sk_graphite_context_check_async_work_completion(owner.context);
        require(!doroti_graphite_has_unfinished_gpu_work(owner.context), "GPU completion remains pending");
        int32_t layout = 0; uint32_t actualFamily = 0;
        require(doroti_graphite_vk_texture_get_state(owner.texture, &layout, &actualFamily) && actualFamily == family,
            "External texture state query failed");
        sk_graphite_recording_delete(owner.recording); owner.recording = nullptr;
    }
    require(!sk_graphite_context_is_device_lost(owner.context), "Unexpected device loss before notification test");
    require(doroti_graphite_vk_context_report_device_lost(owner.context) && sk_graphite_context_is_device_lost(owner.context),
        "External device-loss notification failed");
    std::printf("generation=%d frames=12 persistentWrapper=PASS pixels=PASS gpuCompletion=PASS simulatedExternalLoss=PASS\n", ordinal);
}

int main() {
    std::setvbuf(stdout, nullptr, _IONBF, 0);
    try {
        Dl_info module{};
        require(dladdr(reinterpret_cast<void*>(&doroti_graphite_interop_version), &module) != 0 && module.dli_fname,
            "Cannot identify the loaded Graphite native library");
        std::printf("nativeLibraryPath=%s\n", module.dli_fname);
        require(doroti_graphite_interop_version() == 3, "Native ABI is not 3");
        require(sk_graphite_backend_is_available(VULKAN_SK_GRAPHITE_BACKEND), "Graphite Vulkan not compiled");
        for (int generation = 1; generation <= 3; ++generation) run(generation);
        std::puts("status=PARTIAL contextGenerations=3 frames=36 normalTeardown=PASS validation=notVerified surfacePresent=notVerified productQualified=false");
        return 2;
    } catch (const std::exception& exception) {
        std::fprintf(stderr, "status=FAIL error=%s\n", exception.what());
        return 1;
    }
}
