using System.Runtime.InteropServices;
using System.Text.Json;
using Doroti.Skia.Vulkan;
using Doroti.Ui;
using MaterialSample;
using Microsoft.Maui.ApplicationModel;
namespace DorotiTestbedApp.WinUI;

internal static class WindowsNativeTextureProbe
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int Device(long luid, out nint device);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int Frame(nint device, uint step, out nint frame);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int Info(nint frame, out FrameInfo info);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void Release(nint frame);
    [StructLayout(LayoutKind.Sequential)] private struct FrameInfo { internal nint Handle; internal uint Width, Height; internal long Luid; }
    internal static void Start(Microsoft.UI.Xaml.Window window)
    {
        if (Environment.GetEnvironmentVariable("DOROTI_MAUI_TEXTURE_PROBE") is not { Length: > 0 } report) return;
        _ = Task.Run(async () =>
        {
            nint library = 0, device = 0;
            NativeTextureEntry? entry = null;
            int released = 0;
            try
            {
                var deadline = DateTime.UtcNow.AddSeconds(30);
                while (TextureSampleProbe.OwnerContext is null || TextureSampleProbe.SetTexture is null)
                { if (DateTime.UtcNow > deadline) throw new TimeoutException("Texture sample did not attach."); await Task.Delay(50); }
                Task<T> OnOwner<T>(Func<T> action)
                {
                    var result = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
                    TextureSampleProbe.OwnerContext!.Post(_ => { try { result.SetResult(action()); } catch (Exception e) { result.SetException(e); } }, null);
                    return result.Task;
                }
                entry = await OnOwner(() => TextureRegistry.ForView(TextureSampleProbe.Owner!).CreateNativeTexture());
                if (entry.Device.AdapterLuid == 0) throw new Exception("Native importer has no actual adapter.");
                library = NativeLibrary.Load(Environment.GetEnvironmentVariable("DOROTI_TEXTURE_TEST_LIBRARY")
                    ?? throw new Exception("Supply the test producer library; the MAUI host has no App SDK assembly dependency."));
                T Export<T>(string name) where T : Delegate => Marshal.GetDelegateForFunctionPointer<T>(NativeLibrary.GetExport(library, name));
                var create = Export<Device>("doroti_texture_test_device"); var frame = Export<Frame>("doroti_texture_test_frame");
                var info = Export<Info>("doroti_texture_frame_info"); var release = Export<Release>("doroti_texture_frame_release");
                Marshal.ThrowExceptionForHR(create(entry.Device.AdapterLuid, out device));
                await OnOwner(() => { TextureSampleProbe.SetTexture!(entry.Id, false, 0); return true; });
                for (uint index = 0; index < 12; index++)
                {
                    Marshal.ThrowExceptionForHR(frame(device, index * 20, out var native));
                    Marshal.ThrowExceptionForHR(info(native, out var metadata));
                    using var owned = new NativeTextureFrame(new WindowsSharedTextureBuffer((int)metadata.Width,
                        (int)metadata.Height, NativeTextureFormat.Bgra8888, metadata.Handle, metadata.Luid),
                        () => { release(native); Interlocked.Increment(ref released); });
                    // Wrong-adapter descriptors must fail before retaining/importing.
                    using var foreign = new NativeTextureFrame(new WindowsSharedTextureBuffer(320, 180,
                        NativeTextureFormat.Bgra8888, metadata.Handle, metadata.Luid ^ 1), () => {});
                    try { entry.PushFrame(foreign); throw new Exception("Wrong adapter admitted."); }
                    catch (PlatformNotSupportedException) { }
                    entry.PushFrame(owned);
                    await Task.Delay(100);
                }
                await OnOwner(() => { TextureSampleProbe.SetTexture!(0, false, 0); entry.Dispose(); return true; });
                entry = null;
                while (Volatile.Read(ref released) != 12)
                { if (DateTime.UtcNow > deadline) throw new TimeoutException("GPU consumer did not release producer frames."); await Task.Delay(20); }
                File.WriteAllText(report, JsonSerializer.Serialize(new { status = "PASS", released, wrongAdapterRejected = true,
                    producer = "D3D11 GPU-complete shared BGRA", consumer = "Windows MAUI Graphite Vulkan", visiblePixels = "notVerified" }));
                await MainThread.InvokeOnMainThreadAsync(window.Close);
            }
            catch (Exception error) { File.WriteAllText(report + ".error", error.ToString()); }
            finally
            {
                entry?.Dispose();
                if (device != 0) Marshal.Release(device);
                // Keep producer code resident if a failure leaves consumers alive.
                if (library != 0 && Volatile.Read(ref released) == 12) NativeLibrary.Free(library);
            }
        });
    }
}
