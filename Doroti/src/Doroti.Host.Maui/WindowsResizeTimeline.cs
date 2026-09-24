#if WINDOWS
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace Doroti.Host.Maui;

/// <summary>Opt-in, bounded QPC observations; these are not display receipts.</summary>
internal static class WindowsResizeTimeline
{
    private static readonly string? Output = Environment.GetEnvironmentVariable(
        "DOROTI_WINDOWS_RESIZE_TIMELINE"
    );
    private static readonly ConcurrentQueue<object> Events = new();
    private static int _count;

    internal static void Record(
        nint window,
        string phase,
        uint message = 0,
        long generation = 0,
        int bufferWidth = 0,
        int bufferHeight = 0,
        string? detail = null
    )
    {
        if (string.IsNullOrWhiteSpace(Output) || Interlocked.Increment(ref _count) > 8192)
            return;
        var qpc = Stopwatch.GetTimestamp();
        GetWindowRect(window, out var outer);
        GetClientRect(window, out var client);
        var children = new List<object>();
        EnumChildWindows(
            window,
            (child, _) =>
            {
                var name = new StringBuilder(128);
                GetClassNameW(child, name, name.Capacity);
                GetWindowRect(child, out var bounds);
                children.Add(
                    new
                    {
                        hwnd = (long)child,
                        name = name.ToString(),
                        bounds,
                    }
                );
                return true;
            },
            0
        );
        Events.Enqueue(
            new
            {
                qpc,
                frequency = Stopwatch.Frequency,
                phase,
                message,
                generation,
                bufferWidth,
                bufferHeight,
                detail,
                hwnd = (long)window,
                outer,
                client,
                children,
            }
        );
    }

    internal static void Save()
    {
        if (string.IsNullOrWhiteSpace(Output))
            return;
        try
        {
            File.WriteAllText(
                Output,
                JsonSerializer.Serialize(
                    new
                    {
                        schema = "doroti.windows-resize-timeline/v1",
                        truncated = _count > 8192,
                        events = Events.ToArray(),
                    }
                )
            );
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        {
            // Optional evidence must not prevent the window from closing.
            Debug.WriteLine($"Could not save resize timeline: {error.Message}");
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
    }

    private delegate bool EnumProc(nint window, nint parameter);

    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumChildWindows(nint parent, EnumProc callback, nint parameter);

    [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
    private static extern int GetClassNameW(nint window, StringBuilder name, int count);

    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(nint window, out Rect rect);

    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetClientRect(nint window, out Rect rect);
}
#endif
