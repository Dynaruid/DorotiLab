#include <vulkan/vulkan.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

// Run on the target Android device; no rendering or capability emulation.
int main(int argc, char **argv) {
    VkApplicationInfo app = { .sType = VK_STRUCTURE_TYPE_APPLICATION_INFO,
        .apiVersion = VK_API_VERSION_1_2 };
    VkInstanceCreateInfo ici = { .sType = VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO,
        .pApplicationInfo = &app };
    VkInstance instance;
    VkResult result = vkCreateInstance(&ici, NULL, &instance);
    printf("instance result=%d\n", result);
    if (result) return 1;
    uint32_t count = 0;
    vkEnumeratePhysicalDevices(instance, &count, NULL);
    VkPhysicalDevice *devices = calloc(count, sizeof(*devices));
    vkEnumeratePhysicalDevices(instance, &count, devices);
    for (uint32_t d = 0; d < count; d++) {
        VkPhysicalDeviceProperties properties;
        vkGetPhysicalDeviceProperties(devices[d], &properties);
        printf("device=%s api=%u.%u.%u\n", properties.deviceName,
            VK_VERSION_MAJOR(properties.apiVersion), VK_VERSION_MINOR(properties.apiVersion),
            VK_VERSION_PATCH(properties.apiVersion));
        uint32_t n = 0;
        vkEnumerateDeviceExtensionProperties(devices[d], NULL, &n, NULL);
        VkExtensionProperties *extensions = calloc(n, sizeof(*extensions));
        vkEnumerateDeviceExtensionProperties(devices[d], NULL, &n, extensions);
        const char *enabled[2];
        uint32_t enabledCount = 0;
        for (uint32_t i = 0; i < n; i++) {
            printf("extension=%s\n", extensions[i].extensionName);
            if (!strcmp(extensions[i].extensionName, "VK_KHR_create_renderpass2") ||
                !strcmp(extensions[i].extensionName, "VK_KHR_driver_properties"))
                enabled[enabledCount++] = extensions[i].extensionName;
        }
        uint32_t qn = 0, family = 0;
        vkGetPhysicalDeviceQueueFamilyProperties(devices[d], &qn, NULL);
        VkQueueFamilyProperties *queues = calloc(qn, sizeof(*queues));
        vkGetPhysicalDeviceQueueFamilyProperties(devices[d], &qn, queues);
        for (; family < qn; family++) if (queues[family].queueFlags & VK_QUEUE_GRAPHICS_BIT) break;
        float priority = 1;
        VkDeviceQueueCreateInfo qci = { .sType = VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO,
            .queueFamilyIndex = family, .queueCount = 1, .pQueuePriorities = &priority };
        VkDeviceCreateInfo dci = { .sType = VK_STRUCTURE_TYPE_DEVICE_CREATE_INFO,
            .queueCreateInfoCount = 1, .pQueueCreateInfos = &qci,
            .enabledExtensionCount = enabledCount, .ppEnabledExtensionNames = enabled };
        VkDevice device;
        result = vkCreateDevice(devices[d], &dci, NULL, &device);
        printf("device result=%d\n", result);
        if (result) return 2;
        const char *names[] = { "vkCreateRenderPass2", "vkCreateRenderPass2KHR",
            "vkCmdBeginRenderPass2", "vkCmdBeginRenderPass2KHR",
            "vkCmdNextSubpass2", "vkCmdNextSubpass2KHR", "vkCmdEndRenderPass2", "vkCmdEndRenderPass2KHR" };
        for (uint32_t i = 0; i < sizeof(names)/sizeof(names[0]); i++)
            printf("procedure=%s instance=%p device=%p\n", names[i],
                (void*)vkGetInstanceProcAddr(instance, names[i]), (void*)vkGetDeviceProcAddr(device, names[i]));
        fflush(stdout);
        // Opt-in reproducer: Android 13 goldfish returns a non-null loader
        // trampoline here whose device dispatch is null, so this can SIGSEGV.
        if (argc > 1 && !strcmp(argv[1], "--invoke-instance-renderpass2")) {
        PFN_vkCreateRenderPass2 createPass = (PFN_vkCreateRenderPass2)vkGetInstanceProcAddr(instance, "vkCreateRenderPass2");
        VkSubpassDescription2 subpass = { .sType = VK_STRUCTURE_TYPE_SUBPASS_DESCRIPTION_2,
            .pipelineBindPoint = VK_PIPELINE_BIND_POINT_GRAPHICS };
        VkRenderPassCreateInfo2 pci = { .sType = VK_STRUCTURE_TYPE_RENDER_PASS_CREATE_INFO_2,
            .subpassCount = 1, .pSubpasses = &subpass };
        VkRenderPass pass = VK_NULL_HANDLE;
        if (!createPass) return 3;
        result = createPass(device, &pci, NULL, &pass);
        printf("createRenderPass2 result=%d pass=%p\n", result, (void*)pass);
        fflush(stdout);
        if (result) return 4;
        vkDestroyRenderPass(device, pass, NULL);
        }
        vkDestroyDevice(device, NULL);
        free(queues); free(extensions);
    }
    free(devices);
    vkDestroyInstance(instance, NULL);
    return 0;
}
