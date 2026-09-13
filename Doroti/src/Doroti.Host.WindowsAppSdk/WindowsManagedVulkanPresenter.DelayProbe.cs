using Silk.NET.Vulkan;
using VkSemaphore = Silk.NET.Vulkan.Semaphore;

namespace Doroti.Host.WindowsAppSdk;

internal sealed unsafe partial class WindowsManagedVulkanPresenter
{
    private readonly int _diagnosticDelayMilliseconds = int.TryParse(Environment.GetEnvironmentVariable("DOROTI_WINDOWS_GRAPHITE_DELAY_MS"), out var delay) ? Math.Clamp(delay, 0, 15000) : 0;
    private VkSemaphore _diagnosticDelaySemaphore;
    private Thread? _diagnosticProducer;
    private bool _diagnosticDelayUsed;
    private Result _diagnosticSignalResult = Result.NotReady;

    // Test-only real Vulkan producer delay. No modified driver results, GPU spin,
    // reset, or false device loss. Normal product execution never enters this path.
    private void DelayGraphiteProducerForQualification()
    {
        if (_diagnosticDelayMilliseconds == 0 || _diagnosticDelayUsed || PresentCount == 0) return;
        if (!_usesPackagedGraphiteAsset) throw new InvalidOperationException("The delay fixture requires the official Vulkan 1.2 path.");
        _diagnosticDelayUsed = true;
        var type = new SemaphoreTypeCreateInfo { SType = StructureType.SemaphoreTypeCreateInfo, SemaphoreType = SemaphoreType.Timeline };
        var ci = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo, PNext = &type };
        Check(_vk.CreateSemaphore(_device, &ci, null, out _diagnosticDelaySemaphore), "qualification timeline");
        _diagnosticProducer = new Thread(() =>
        {
            Thread.Sleep(_diagnosticDelayMilliseconds);
            var signal = (delegate* unmanaged<nint, SemaphoreSignalInfo*, Result>)(nint)_vk.GetDeviceProcAddr(_device, "vkSignalSemaphore");
            var info = new SemaphoreSignalInfo { SType = StructureType.SemaphoreSignalInfo, Semaphore = _diagnosticDelaySemaphore, Value = 1 };
            _diagnosticSignalResult = signal(_device.Handle, &info);
        });
        _diagnosticProducer.Start();
        ulong value = 1;
        var timeline = new TimelineSemaphoreSubmitInfo { SType = StructureType.TimelineSemaphoreSubmitInfo, WaitSemaphoreValueCount = 1, PWaitSemaphoreValues = &value };
        var stage = PipelineStageFlags.AllCommandsBit;
        var semaphore = _diagnosticDelaySemaphore;
        var submit = new SubmitInfo { SType = StructureType.SubmitInfo, PNext = &timeline,
            WaitSemaphoreCount = 1, PWaitSemaphores = &semaphore, PWaitDstStageMask = &stage };
        Check(_vk.QueueSubmit(_queue, 1, &submit, default), "qualification producer wait");
        var marker = Environment.GetEnvironmentVariable("DOROTI_WINDOWS_GRAPHITE_DELAY_MARKER");
        if (!string.IsNullOrWhiteSpace(marker)) File.WriteAllText(marker, "real-timeline-wait-submitted");
    }
    private void ReleaseDiagnosticProducer()
    {
        _diagnosticProducer?.Join(); _diagnosticProducer = null;
        if (_diagnosticDelaySemaphore.Handle == 0) return;
        _vk.DestroySemaphore(_device, _diagnosticDelaySemaphore, null); _diagnosticDelaySemaphore = default;
        RecordEvent($"qualification producer completed with {_diagnosticSignalResult}");
        Console.Error.WriteLine($"qualification producer completed with {_diagnosticSignalResult}");
    }
}
