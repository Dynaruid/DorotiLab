using System.Runtime.InteropServices;
using Doroti.Skia.Vulkan;
using Doroti.Ui;

namespace Doroti.Host.Qt;

/// <summary>Single-plane RGBA/BGRA DMA-BUF input. Never maps pixels. The allocation must remain immutable
/// until release is called; retaining only the fd does not stop a camera pool from overwriting it.</summary>
public static class LinuxGpuTextureFrames
{
    /// <summary>Duplicates the fd and waits for producer writes on the calling producer thread.
    /// acquireFenceFd is borrowed; -1 exports the DMA-BUF implicit write fence. On failure ownership stays with the caller.</summary>
    public static NativeTextureFrame FromDmaBuf(
        LinuxDmaBufTextureBuffer buffer,
        Action release,
        int acquireFenceFd = -1
    )
    {
        if (!OperatingSystem.IsLinux())
            throw new PlatformNotSupportedException();
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentNullException.ThrowIfNull(release);
        if (
            buffer.FileDescriptor < 0
            || buffer.RowPitch < (ulong)buffer.Width * 4
            || buffer.AllocationSize == 0
            || buffer.Offset >= buffer.AllocationSize
        )
            throw new ArgumentException("Invalid DMA-BUF descriptor.", nameof(buffer));
        var fd = Dup(buffer.FileDescriptor);
        if (fd < 0)
            throw new IOException("dup(DMA-BUF) failed.");
        var exported = -1;
        try
        {
            if (acquireFenceFd < 0)
            {
                var fence = new ExportSyncFile { Flags = 1, Fd = -1 }; // DMA_BUF_SYNC_READ: wait for previous writes
                if (Ioctl(fd, 0xC0086202, ref fence) != 0)
                    throw new NotSupportedException(
                        "DMA_BUF_IOCTL_EXPORT_SYNC_FILE unavailable; supply an explicit acquire sync-file."
                    );
                exported = fence.Fd;
                acquireFenceFd = exported;
            }
            var poll = new PollFd { Fd = acquireFenceFd, Events = 1 };
            if (
                Poll(ref poll, 1, 5000) != 1
                || (poll.ReturnedEvents & 1) == 0
                || (poll.ReturnedEvents & (8 | 32)) != 0
            )
                throw new IOException(
                    "DMA-BUF producer fence did not complete successfully within 5 seconds."
                );
            return new(
                buffer with
                {
                    FileDescriptor = fd,
                },
                () =>
                {
                    Close(fd);
                    release();
                }
            );
        }
        catch
        {
            Close(fd);
            throw;
        }
        finally
        {
            if (exported >= 0)
                Close(exported);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ExportSyncFile
    {
        internal uint Flags;
        internal int Fd;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PollFd
    {
        internal int Fd;
        internal short Events,
            ReturnedEvents;
    }

    [DllImport("libc", EntryPoint = "dup")]
    private static extern int Dup(int fd);

    [DllImport("libc", EntryPoint = "close")]
    private static extern int Close(int fd);

    [DllImport("libc", EntryPoint = "ioctl")]
    private static extern int Ioctl(int fd, nuint request, ref ExportSyncFile data);

    [DllImport("libc", EntryPoint = "poll")]
    private static extern int Poll(ref PollFd fd, nuint count, int milliseconds);
}

/// <summary>GStreamer appsink camera/video adapter. The caller supplies a locally controlled pipeline ending in
/// appsink name=doroti_texture. It must negotiate one RGBA/BGRA DMA-BUF plane; CPU/multiplane samples fail.</summary>
public sealed class LinuxGpuTextureCamera : IAsyncDisposable
{
    private const string Library = "doroti_texture_gstreamer";
    private readonly Task _worker;
    private readonly CancellationTokenSource _stop = new();
    private readonly TaskCompletionSource _started = new(
        TaskCreationOptions.RunContinuationsAsynchronously
    );
    private long _published;
    public long PublishedFrames => Interlocked.Read(ref _published);
    public Task Completion => _worker;

    private LinuxGpuTextureCamera(NativeTextureEntry entry, string pipeline) =>
        _worker = Task.Factory.StartNew(
            () => Run(entry, pipeline),
            CancellationToken.None,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default
        );

    public static async Task<LinuxGpuTextureCamera> StartAsync(
        NativeTextureEntry entry,
        string pipeline,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentException.ThrowIfNullOrWhiteSpace(pipeline);
        if (!OperatingSystem.IsLinux() || entry.Device.Platform != NativeTexturePlatform.Linux)
            throw new PlatformNotSupportedException();
        var camera = new LinuxGpuTextureCamera(entry, pipeline);
        try
        {
            await camera._started.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
            return camera;
        }
        catch
        {
            await camera.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }

    private void Run(NativeTextureEntry entry, string pipeline)
    {
        nint camera = 0;
        try
        {
            var error = new byte[1024];
            if (Open(pipeline, out camera, error, (uint)error.Length) != 0)
                throw Failure(error);
            _started.TrySetResult();
            while (!_stop.IsCancellationRequested)
            {
                Array.Clear(error);
                var status = Read(camera, out var info, error, (uint)error.Length);
                if (status < 0)
                    throw Failure(error);
                if (status == 0)
                    continue;
                var sample = info.Sample;
                try
                {
                    using var frame = LinuxGpuTextureFrames.FromDmaBuf(
                        new(
                            (int)info.Width,
                            (int)info.Height,
                            info.Format == 0
                                ? NativeTextureFormat.Rgba8888
                                : NativeTextureFormat.Bgra8888,
                            info.Fd,
                            info.AllocationSize,
                            info.Offset,
                            info.RowPitch,
                            info.Modifier
                        ),
                        () => Release(info.Sample)
                    );
                    sample = 0;
                    entry.PushFrame(frame);
                    Interlocked.Increment(ref _published);
                }
                finally
                {
                    if (sample != 0)
                        Release(sample);
                }
            }
        }
        catch (Exception error)
        {
            _started.TrySetException(error);
            throw;
        }
        finally
        {
            if (camera != 0)
                Close(camera);
        }
    }

    private static Exception Failure(byte[] error) =>
        new InvalidOperationException(System.Text.Encoding.UTF8.GetString(error).TrimEnd('\0'));

    public async ValueTask DisposeAsync()
    {
        _stop.Cancel();
        // Keep native pipeline owned by its worker if a driver stalls; never free while read is active.
        await _worker.WaitAsync(TimeSpan.FromSeconds(6)).ConfigureAwait(false);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct FrameInfo
    {
        internal nint Sample;
        internal int Fd;
        internal uint Width,
            Height,
            Format;
        internal ulong AllocationSize,
            Offset,
            RowPitch,
            Modifier;
    }

    [DllImport(Library, EntryPoint = "doroti_texture_gst_open")]
    private static extern int Open(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string pipeline,
        out nint camera,
        [Out] byte[] error,
        uint size
    );

    [DllImport(Library, EntryPoint = "doroti_texture_gst_read")]
    private static extern int Read(nint camera, out FrameInfo info, byte[] error, uint size);

    [DllImport(Library, EntryPoint = "doroti_texture_gst_release")]
    private static extern void Release(nint sample);

    [DllImport(Library, EntryPoint = "doroti_texture_gst_close")]
    private static extern void Close(nint camera);
}
