using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Doroti.Host.WindowsAppSdk;
using Doroti.Skia.Vulkan;
using Doroti.Ui;

internal sealed class WindowsTextureProbeProducer : IDisposable
{
    private const string Library = "doroti_windows_appsdk_host_v1";
    private readonly CancellationTokenSource _stop = new();
    private readonly Task _worker;

    [ModuleInitializer]
    internal static void Install()
    {
        if (Environment.GetEnvironmentVariable("DOROTI_TESTBED_MODE") == "texture-native")
            NativeTextureFixtureProbe.StartNativeProducer =
                entry => new WindowsTextureProbeProducer(entry);
    }

    private WindowsTextureProbeProducer(NativeTextureEntry entry) =>
        _worker = Task.Run(async () =>
        {
            nint device = 0;
            try
            {
                var deadline = Environment.TickCount64 + 5000;
                while (entry.Device.AdapterLuid == 0)
                {
                    if (Environment.TickCount64 > deadline)
                        throw new TimeoutException("GPU adapter not ready");
                    await Task.Delay(10, _stop.Token);
                }
                if (Environment.GetEnvironmentVariable("DOROTI_TEXTURE_CAMERA") == "1")
                {
                    await using var camera = await WindowsGpuTextureCamera.StartAsync(
                        entry,
                        cancellationToken: _stop.Token
                    );
                    Console.WriteLine("doroti.texture.camera.started");
                    while (!_stop.IsCancellationRequested)
                    {
                        await Task.Delay(200, _stop.Token);
                        if (camera.Completion.IsCompleted)
                            await camera.Completion;
                        Console.WriteLine($"doroti.texture.camera.frames={camera.PublishedFrames}");
                    }
                    return;
                }
                Marshal.ThrowExceptionForHR(CreateDevice(entry.Device.AdapterLuid, out device));
                uint step = 0;
                while (!_stop.IsCancellationRequested)
                {
                    Marshal.ThrowExceptionForHR(CreateFrame(device, step++, out var native));
                    try
                    {
                        Marshal.ThrowExceptionForHR(GetInfo(native, out var info));
                        var allocation = native;
                        using var frame = new NativeTextureFrame(
                            new WindowsSharedTextureBuffer(
                                (int)info.Width,
                                (int)info.Height,
                                NativeTextureFormat.Bgra8888,
                                info.Handle,
                                info.Luid
                            ),
                            () => Release(allocation)
                        );
                        native = 0;
                        entry.PushFrame(frame);
                    }
                    finally
                    {
                        if (native != 0)
                            Release(native);
                    }
                    if (step % 30 == 0)
                        Console.WriteLine($"doroti.texture.synthetic.frames={step}");
                    await Task.Delay(33, _stop.Token);
                }
            }
            catch (OperationCanceledException) when (_stop.IsCancellationRequested) { }
            catch (ObjectDisposedException) { }
            catch (Exception error)
            {
                Console.WriteLine($"doroti.texture.producer.error={error}");
            }
            finally
            {
                if (device != 0)
                    Marshal.Release(device);
            }
        });

    public void Dispose()
    {
        _stop.Cancel();
        _worker.GetAwaiter().GetResult();
        _stop.Dispose();
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Info
    {
        internal nint Handle;
        internal uint Width,
            Height;
        internal long Luid;
    }

    [DllImport(Library, EntryPoint = "doroti_texture_test_device")]
    private static extern int CreateDevice(long luid, out nint device);

    [DllImport(Library, EntryPoint = "doroti_texture_test_frame")]
    private static extern int CreateFrame(nint device, uint step, out nint frame);

    [DllImport(Library, EntryPoint = "doroti_texture_frame_info")]
    private static extern int GetInfo(nint frame, out Info info);

    [DllImport(Library, EntryPoint = "doroti_texture_frame_release")]
    private static extern void Release(nint frame);
}
