using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace Doroti.Host.WindowsAppSdk;

internal sealed unsafe partial class WindowsManagedVulkanPresenter
{
    private readonly bool _validationEnabled = Environment.GetEnvironmentVariable("DOROTI_WINDOWS_VULKAN_VALIDATION") == "1";
    private DebugUtilsMessengerEXT _validationMessenger;
    private DebugUtilsMessengerCallbackFunctionEXT? _validationCallback;
    private int _validationErrors, _validationWarnings;
    private bool _validationCallbackFault;
    private readonly List<string> _validationMessages = [];

    private DebugUtilsMessengerCreateInfoEXT ValidationDebugInfo()
    {
        _validationCallback ??= OnValidation;
        return new() { SType = StructureType.DebugUtilsMessengerCreateInfoExt,
            MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.WarningBitExt | DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt,
            MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt | DebugUtilsMessageTypeFlagsEXT.ValidationBitExt | DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt,
            PfnUserCallback = new PfnDebugUtilsMessengerCallbackEXT(_validationCallback) };
    }
    private uint OnValidation(DebugUtilsMessageSeverityFlagsEXT severity, DebugUtilsMessageTypeFlagsEXT types, DebugUtilsMessengerCallbackDataEXT* data, void* user)
    {
        try
        {
            if ((severity & DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt) != 0) Interlocked.Increment(ref _validationErrors);
            else Interlocked.Increment(ref _validationWarnings);
            var message = data == null ? severity.ToString() : Marshal.PtrToStringUTF8((nint)data->PMessage) ?? severity.ToString();
            lock (_validationMessages) if (_validationMessages.Count < 128) _validationMessages.Add(message);
            Console.Error.WriteLine("Vulkan validation: " + message);
        }
        catch { _validationCallbackFault = true; }
        return 0;
    }
    private string[] ValidationMessages() { lock (_validationMessages) return _validationMessages.ToArray(); }
    private void CreateValidationMessenger(DebugUtilsMessengerCreateInfoEXT* info)
    {
        var create = (delegate* unmanaged<nint, DebugUtilsMessengerCreateInfoEXT*, void*, DebugUtilsMessengerEXT*, Result>)
            (nint)_vk.GetInstanceProcAddr(_instance, "vkCreateDebugUtilsMessengerEXT");
        if (create == null) throw new PlatformNotSupportedException("Vulkan debug utils missing.");
        DebugUtilsMessengerEXT messenger;
        Check(create(_instance.Handle, info, null, &messenger), "vkCreateDebugUtilsMessengerEXT");
        _validationMessenger = messenger;
    }
    private void DestroyValidationMessenger()
    {
        if (_validationMessenger.Handle == 0) return;
        var destroy = (delegate* unmanaged<nint, DebugUtilsMessengerEXT, void*, void>)(nint)_vk.GetInstanceProcAddr(_instance, "vkDestroyDebugUtilsMessengerEXT");
        destroy(_instance.Handle, _validationMessenger, null); _validationMessenger = default;
    }
}
