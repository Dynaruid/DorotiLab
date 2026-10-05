using System.Runtime.InteropServices;
using Doroti.Skia.Vulkan;
using Doroti.Ui;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>Copies/converts an owned camera/decoder D3D11 subresource to an immutable shared BGRA GPU allocation.
/// Call on a producer thread. The source must remain owned and producer-ready until this method returns.</summary>
public static class WindowsGpuTextureFrames
{
    public static NativeTextureFrame FromD3D11Texture(nint texture, uint subresource = 0)
    {
        if (texture == 0)
            throw new ArgumentNullException(nameof(texture));
        Marshal.ThrowExceptionForHR(Copy(texture, subresource, out var frame));
        return Adopt(frame);
    }

    internal static NativeTextureFrame Adopt(nint frame)
    {
        try
        {
            Marshal.ThrowExceptionForHR(GetInfo(frame, out var info));
            return new(
                new WindowsSharedTextureBuffer(
                    (int)info.Width,
                    (int)info.Height,
                    NativeTextureFormat.Bgra8888,
                    info.Handle,
                    info.AdapterLuid
                ),
                () => Release(frame)
            );
        }
        catch
        {
            Release(frame);
            throw;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Info
    {
        internal nint Handle;
        internal uint Width,
            Height;
        internal long AdapterLuid;
    }

    [DllImport(
        WindowsNativeV1.LibraryName,
        EntryPoint = "doroti_texture_copy_d3d11",
        CallingConvention = CallingConvention.Cdecl
    )]
    private static extern int Copy(nint texture, uint subresource, out nint frame);

    [DllImport(
        WindowsNativeV1.LibraryName,
        EntryPoint = "doroti_texture_frame_info",
        CallingConvention = CallingConvention.Cdecl
    )]
    private static extern int GetInfo(nint frame, out Info info);

    [DllImport(
        WindowsNativeV1.LibraryName,
        EntryPoint = "doroti_texture_frame_release",
        CallingConvention = CallingConvention.Cdecl
    )]
    private static extern void Release(nint frame);
}

/// <summary>Media Foundation camera producer using the renderer's adapter and DXGI device manager.
/// CPU-only samples fail explicitly. Dispose before disposing the texture entry.</summary>
public sealed class WindowsGpuTextureCamera : IAsyncDisposable
{
    private readonly object _gate = new();
    private nint _camera;
    private int _stop;
    private readonly TaskCompletionSource _started = new(
        TaskCreationOptions.RunContinuationsAsynchronously
    );
    private readonly Task _worker;
    public Task Completion => _worker;
    public long PublishedFrames => Interlocked.Read(ref _published);
    private long _published;

    private WindowsGpuTextureCamera(NativeTextureEntry entry, uint cameraIndex)
    {
        _worker = Task.Factory.StartNew(
            () => Run(entry, cameraIndex),
            CancellationToken.None,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default
        );
    }

    public static async Task<WindowsGpuTextureCamera> StartAsync(
        NativeTextureEntry entry,
        uint cameraIndex = 0,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(entry);
        if (entry.Device.Platform != NativeTexturePlatform.Windows)
            throw new ArgumentException("A Windows native texture is required.", nameof(entry));
        var result = new WindowsGpuTextureCamera(entry, cameraIndex);
        try
        {
            await result._started.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }
        catch
        {
            await result.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }

    private void Run(NativeTextureEntry entry, uint index)
    {
        nint camera = 0;
        try
        {
            var deadline = Environment.TickCount64 + 5000;
            while (entry.Device.AdapterLuid == 0 && Volatile.Read(ref _stop) == 0)
            {
                if (Environment.TickCount64 >= deadline)
                    throw new TimeoutException("Renderer GPU adapter is not ready.");
                Thread.Sleep(10);
            }
            if (Volatile.Read(ref _stop) != 0)
            {
                _started.TrySetCanceled();
                return;
            }
            Marshal.ThrowExceptionForHR(Open(entry.Device.AdapterLuid, index, out camera));
            lock (_gate)
                _camera = camera;
            _started.TrySetResult();
            while (Volatile.Read(ref _stop) == 0)
            {
                var hr = Read(camera, out var frame);
                if (Volatile.Read(ref _stop) != 0)
                {
                    if (frame != 0)
                        using (WindowsGpuTextureFrames.Adopt(frame)) { }
                    break;
                }
                Marshal.ThrowExceptionForHR(hr);
                if (frame == 0)
                    continue;
                using var owned = WindowsGpuTextureFrames.Adopt(frame);
                entry.PushFrame(owned);
                Interlocked.Increment(ref _published);
            }
        }
        catch (Exception exception)
        {
            _started.TrySetException(exception);
            if (Volatile.Read(ref _stop) == 0)
                throw;
        }
        finally
        {
            lock (_gate)
            {
                _camera = 0;
                if (camera != 0)
                    Close(camera);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        Interlocked.Exchange(ref _stop, 1);
        await Task.Run(() =>
            {
                lock (_gate)
                    if (_camera != 0)
                        Stop(_camera);
            })
            .WaitAsync(TimeSpan.FromSeconds(6))
            .ConfigureAwait(false);
        // A stuck native source stays owned by the worker; a deadline never authorizes freeing it.
        await _worker.WaitAsync(TimeSpan.FromSeconds(6)).ConfigureAwait(false);
    }

    [DllImport(
        WindowsNativeV1.LibraryName,
        EntryPoint = "doroti_texture_camera_open",
        CallingConvention = CallingConvention.Cdecl
    )]
    private static extern int Open(long luid, uint index, out nint camera);

    [DllImport(
        WindowsNativeV1.LibraryName,
        EntryPoint = "doroti_texture_camera_read",
        CallingConvention = CallingConvention.Cdecl
    )]
    private static extern int Read(nint camera, out nint frame);

    [DllImport(
        WindowsNativeV1.LibraryName,
        EntryPoint = "doroti_texture_camera_stop",
        CallingConvention = CallingConvention.Cdecl
    )]
    private static extern void Stop(nint camera);

    [DllImport(
        WindowsNativeV1.LibraryName,
        EntryPoint = "doroti_texture_camera_close",
        CallingConvention = CallingConvention.Cdecl
    )]
    private static extern void Close(nint camera);
}
