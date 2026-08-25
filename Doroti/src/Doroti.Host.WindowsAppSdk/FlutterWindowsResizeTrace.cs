using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>
/// Opt-in, non-blocking QPC trace used by the F6-R output observer. The native,
/// framework, and raster owners enqueue immutable JSONL records; one background
/// writer owns the file so resize WndProc and GPU work never wait for I/O.
/// </summary>
internal sealed class FlutterWindowsResizeTrace : IDisposable
{
    private const int MaximumQueuedEvents = 16_384;
    private readonly ulong _viewId;
    private readonly nint _topLevelHwnd;
    private readonly nint _childHwnd;
    private readonly string _runId;
    private readonly string? _path;
    private readonly ConcurrentQueue<string> _queue = new();
    private readonly AutoResetEvent _signal = new(false);
    private readonly Thread? _writer;
    private int _queueDepth;
    private long _dropped;
    private int _disposed;

    internal FlutterWindowsResizeTrace(ulong viewId, nint topLevelHwnd, nint childHwnd)
    {
        _viewId = viewId;
        _topLevelHwnd = topLevelHwnd;
        _childHwnd = childHwnd;
        _runId = Environment.GetEnvironmentVariable("DOROTI_WINDOWS_RESIZE_TRACE_RUN_ID") ??
            Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
        _path = Environment.GetEnvironmentVariable("DOROTI_WINDOWS_RESIZE_TRACE");
        if (string.IsNullOrWhiteSpace(_path)) return;

        _path = Path.GetFullPath(_path);
        var directory = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
        _writer = new Thread(WriteLoop)
        {
            IsBackground = true,
            Name = "Doroti Windows F6-R causal trace writer",
        };
        _writer.Start();
        Record("traceStarted", null, null,
            $"qpcFrequency={Stopwatch.Frequency};processId={Environment.ProcessId}");
    }

    internal bool Enabled => _writer is not null;
    internal long DroppedEventCount => Interlocked.Read(ref _dropped);

    internal void Record(
        string eventName,
        WindowsViewMetrics? metrics = null,
        long? causalFrameId = null,
        string? detail = null,
        bool captureGeometry = false)
    {
        if (_writer is null || Volatile.Read(ref _disposed) != 0) return;
        ArgumentException.ThrowIfNullOrWhiteSpace(eventName);
        if (Interlocked.Increment(ref _queueDepth) > MaximumQueuedEvents)
        {
            Interlocked.Decrement(ref _queueDepth);
            Interlocked.Increment(ref _dropped);
            return;
        }

        var topLevel = captureGeometry ? GetWindowRect(_topLevelHwnd) : default;
        var child = captureGeometry ? GetClientScreenRect(_childHwnd) : default;
        var cursor = default(NativePoint);
        if (captureGeometry) _ = NativeMethods.GetCursorPos(out cursor);
        var builder = new StringBuilder(512);
        builder.Append('{');
        AppendString(builder, "schemaVersion", "doroti.windowsappsdk.f6r-causal/v1");
        AppendString(builder, "runId", _runId);
        AppendString(builder, "event", eventName);
        AppendNumber(builder, "qpc", Stopwatch.GetTimestamp());
        AppendNumber(builder, "qpcFrequency", Stopwatch.Frequency);
        AppendNumber(builder, "viewId", _viewId);
        AppendNumber(builder, "resizeGeneration", metrics?.ResizeGeneration ?? 0);
        AppendNumber(builder, "targetWidth", metrics?.PhysicalWidth ?? 0);
        AppendNumber(builder, "targetHeight", metrics?.PhysicalHeight ?? 0);
        AppendNumber(builder, "causalFrameId", causalFrameId ?? 0);
        AppendNumber(builder, "managedThreadId", Environment.CurrentManagedThreadId);
        AppendNumber(builder, "nativeThreadId", NativeMethods.GetCurrentThreadId());
        AppendRect(builder, "topLevelRect", topLevel);
        AppendRect(builder, "childRect", child);
        builder.Append(",\"cursor\":{\"x\":").Append(cursor.X)
            .Append(",\"y\":").Append(cursor.Y).Append('}');
        if (!string.IsNullOrEmpty(detail)) AppendString(builder, "detail", detail);
        builder.Append('}');
        _queue.Enqueue(builder.ToString());
        _signal.Set();
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        if (_writer is not null)
        {
            _signal.Set();
            if (!_writer.Join(TimeSpan.FromSeconds(5)))
                Console.Error.WriteLine("doroti.windowsappsdk.f6r.trace=writer-timeout");
        }
        _signal.Dispose();
    }

    private void WriteLoop()
    {
        try
        {
            using var stream = new FileStream(
                _path!, FileMode.Create, FileAccess.Write, FileShare.Read,
                64 * 1024, FileOptions.SequentialScan);
            using var writer = new StreamWriter(stream, new UTF8Encoding(false), 64 * 1024);
            while (Volatile.Read(ref _disposed) == 0 || !_queue.IsEmpty)
            {
                while (_queue.TryDequeue(out var record))
                {
                    Interlocked.Decrement(ref _queueDepth);
                    writer.WriteLine(record);
                }
                writer.Flush();
                if (Volatile.Read(ref _disposed) == 0) _signal.WaitOne(50);
            }
            writer.WriteLine(
                $"{{\"schemaVersion\":\"doroti.windowsappsdk.f6r-causal/v1\"," +
                $"\"runId\":\"{Escape(_runId)}\",\"event\":\"traceStopped\"," +
                $"\"qpc\":{Stopwatch.GetTimestamp()},\"qpcFrequency\":{Stopwatch.Frequency}," +
                $"\"viewId\":{_viewId},\"droppedEvents\":{DroppedEventCount}}}");
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(
                $"doroti.windowsappsdk.f6r.trace=failed;type={exception.GetType().Name};message={exception.Message}");
        }
    }

    private static NativeRect GetWindowRect(nint hwnd) =>
        hwnd != 0 && NativeMethods.GetWindowRect(hwnd, out var rect) ? rect : default;

    private static NativeRect GetClientScreenRect(nint hwnd)
    {
        if (hwnd == 0 || !NativeMethods.GetClientRect(hwnd, out var rect)) return default;
        var topLeft = new NativePoint(rect.Left, rect.Top);
        var bottomRight = new NativePoint(rect.Right, rect.Bottom);
        if (!NativeMethods.ClientToScreen(hwnd, ref topLeft) ||
            !NativeMethods.ClientToScreen(hwnd, ref bottomRight)) return default;
        return new(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
    }

    private static void AppendString(StringBuilder builder, string name, string value)
    {
        if (builder.Length > 1) builder.Append(',');
        builder.Append('\"').Append(name).Append("\":\"").Append(Escape(value)).Append('\"');
    }

    private static void AppendNumber(StringBuilder builder, string name, long value) =>
        builder.Append(",\"").Append(name).Append("\":").Append(value);

    private static void AppendNumber(StringBuilder builder, string name, ulong value) =>
        builder.Append(",\"").Append(name).Append("\":").Append(value);

    private static void AppendRect(StringBuilder builder, string name, NativeRect rect) =>
        builder.Append(",\"").Append(name).Append("\":{\"left\":").Append(rect.Left)
            .Append(",\"top\":").Append(rect.Top).Append(",\"right\":").Append(rect.Right)
            .Append(",\"bottom\":").Append(rect.Bottom).Append(",\"width\":").Append(rect.Width)
            .Append(",\"height\":").Append(rect.Height).Append('}');

    private static string Escape(string value) => value
        .Replace("\\", "\\\\", StringComparison.Ordinal)
        .Replace("\"", "\\\"", StringComparison.Ordinal)
        .Replace("\r", "\\r", StringComparison.Ordinal)
        .Replace("\n", "\\n", StringComparison.Ordinal);

    [StructLayout(LayoutKind.Sequential)]
    private readonly record struct NativePoint(int X, int Y);

    [StructLayout(LayoutKind.Sequential)]
    private readonly record struct NativeRect(int Left, int Top, int Right, int Bottom)
    {
        internal int Width => Right - Left;
        internal int Height => Bottom - Top;
    }

    private static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        internal static extern uint GetCurrentThreadId();
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(nint hwnd, out NativeRect rect);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetClientRect(nint hwnd, out NativeRect rect);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ClientToScreen(nint hwnd, ref NativePoint point);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(out NativePoint point);
    }
}
