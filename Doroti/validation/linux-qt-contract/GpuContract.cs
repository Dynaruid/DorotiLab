using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Doroti.Host.Qt;
using Doroti.Skia.Vulkan;
using SkiaSharp;
using Silk.NET.Vulkan;

internal static unsafe class GpuContract
{
    private static Exception? _failure;

    internal static void Run(string library)
    {
        var module = NativeLibrary.Load(library);
        // Qt plugins/QML type registrations can retain code until process exit.
        var run = (delegate* unmanaged[Cdecl]<
            delegate* unmanaged[Cdecl]<QtQuickNative.Gpu*, int>,
            int>)
            NativeLibrary.GetExport(module, "doroti_test_gpu");
        var result = run(&Validate);
        if (_failure is not null)
            throw _failure;
        if (result != 0)
            throw new InvalidOperationException($"Qt GPU contract failed: {result}");
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static int Validate(QtQuickNative.Gpu* descriptor)
    {
        try
        {
            var d = *descriptor;
            using var gpu = new GraphiteVulkanQuick(
                d.Instance,
                d.Physical,
                d.Device,
                d.Queue,
                d.Family,
                d.ApiVersion
            );
            gpu.Begin(64, 64).Canvas.Clear(SKColors.Red);
            var front = gpu.Image(0);
            gpu.Complete();
            gpu.MarkPublished();
            for (var i = 0; i < 10; i++)
            {
                gpu.Begin(64, 64).Canvas.Clear(SKColors.Blue);
                if (gpu.Image(0) == front)
                    throw new InvalidOperationException("Staging overwrote a published P image.");
                gpu.Complete(); // Simulate native commit rejection AFTER successful copy.
                gpu.Cancel();
                gpu.Begin(65, 67).Canvas.Clear(SKColors.Green);
                if (gpu.Image(0) == front)
                    throw new InvalidOperationException(
                        "Superseded resize reused the published image."
                    );
                gpu.Cancel(); // Recording cancelled without any submission.
                gpu.Begin(64, 64).Canvas.Clear(SKColors.Red);
                if (gpu.Image(0) == front)
                    throw new InvalidOperationException(
                        "Cancelled resize lost its published generation."
                    );
                gpu.Complete();
                gpu.MarkPublished();
                front = gpu.Image(0);
                if (gpu.ReservedBytes > 1024 * 1024 || gpu.RetiringLayers > 2)
                    throw new InvalidOperationException(
                        "Repeated cancellation retained unbounded banks."
                    );
            }
            try
            {
                gpu.Begin(16384, 16384);
                throw new InvalidOperationException("GPU allocation budget was not enforced.");
            }
            catch (NotSupportedException) { }
            gpu.Begin(64, 64);
            if (gpu.Image(0) == front)
                throw new InvalidOperationException(
                    "Budget rejection released the published bank."
                );
            gpu.Cancel();
            for (var cycle = 0; cycle < 10; cycle++)
            {
                using var temporary = new GraphiteVulkanQuick(
                    d.Instance, d.Physical, d.Device, d.Queue, d.Family, d.ApiVersion);
                temporary.Begin(32, 32).Canvas.Clear(SKColors.Blue);
                temporary.Complete();
                temporary.MarkPublished();
            }
            Console.WriteLine("PASS: 10 Quick GPU create/submit/dispose cycles");
            using (var failed = new GraphiteVulkanQuick(
                d.Instance, d.Physical, d.Device, d.Queue, d.Family, d.ApiVersion))
            {
                try { failed.InjectFailureForContract(Result.Timeout); }
                catch (InvalidOperationException) { }
                if (failed.Failure != GraphiteVulkanQuick.FailureKind.Timeout)
                    throw new InvalidOperationException("Fence timeout was not terminal.");
                try { failed.Begin(32, 32); throw new InvalidOperationException("Faulted GPU accepted a frame."); }
                catch (InvalidOperationException error) when (error.Message.StartsWith("Quick GPU is terminal:")) { }
                failed.Dispose(); // A second disposal must be safe after quarantine.
            }
            using (var lost = new GraphiteVulkanQuick(
                d.Instance, d.Physical, d.Device, d.Queue, d.Family, d.ApiVersion))
            {
                try { lost.InjectFailureForContract(Result.ErrorDeviceLost); }
                catch (InvalidOperationException) { }
                if (lost.Failure != GraphiteVulkanQuick.FailureKind.DeviceLost)
                    throw new InvalidOperationException("Device loss was not terminal.");
                lost.Dispose();
            }
            Console.WriteLine("PASS: injected Quick timeout/device-loss terminal and idempotent disposal");
            Console.WriteLine(
                $"PASS: 10 Qt GPU submit/rejected-commit/cancelled-resize cycles; bytes={gpu.ReservedBytes}, retiring={gpu.RetiringLayers}"
            );
            return 0;
        }
        catch (Exception error)
        {
            _failure = error;
            return 1;
        }
    }
}
