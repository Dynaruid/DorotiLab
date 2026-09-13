using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Doroti.Hosting;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>Owner UI queue independent of the render worker and WinForms/WinUI contexts.</summary>
internal sealed class WindowsPlatformViewDispatcher : SynchronizationContext, IPlatformViewDispatcher, IDisposable
{
    private readonly ConcurrentQueue<Action> _queue = new();
    private readonly object _gate = new();
    private readonly uint _thread = Native.GetCurrentThreadId();
    private readonly Native.SubclassProc _callback;
    private nint _window;
    private const uint DispatchMessage = 0x8000 + 0x731;

    internal WindowsPlatformViewDispatcher()
    {
        _callback = WindowProc;
        _window = Native.CreateWindowExW(0, "STATIC", "Doroti PlatformView dispatcher", 0,
            0, 0, 0, 0, -3, 0, 0, 0);
        if (_window == 0) throw new Win32Exception(Marshal.GetLastWin32Error());
        if (!Native.SetWindowSubclass(_window, _callback, 1, 0))
        { Native.DestroyWindow(_window); _window = 0; throw new Win32Exception(); }
    }
    internal void VerifyThread()
    {
        if (Native.GetCurrentThreadId() != _thread) throw new InvalidOperationException("PlatformView requires its HWND UI thread.");
    }
    public override void Post(SendOrPostCallback callback, object? state) => Enqueue(() => callback(state));
    public override void Send(SendOrPostCallback callback, object? state) =>
        throw new NotSupportedException("Synchronous UI dispatch can deadlock Windows render shutdown.");
    private void Enqueue(Action action)
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_window == 0, this);
            _queue.Enqueue(action);
            if (!Native.PostMessageW(_window, DispatchMessage, 0, 0))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
    public ValueTask InvokeAsync(Func<ValueTask> action)
    {
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        async void Run()
        {
            var previous = Current;
            SetSynchronizationContext(this);
            try { await action(); completion.TrySetResult(); }
            catch (Exception error) { completion.TrySetException(error); }
            finally { SetSynchronizationContext(previous); }
        }
        if (Native.GetCurrentThreadId() == _thread) Run();
        else Enqueue(Run);
        return new(completion.Task);
    }
    private nint WindowProc(nint hwnd, uint message, nuint wparam, nint lparam, nuint id, nuint data)
    {
        if (message == DispatchMessage)
        {
            try { Drain(); } catch (Exception error) { System.Diagnostics.Trace.TraceError(error.ToString()); }
            return 0;
        }
        return Native.DefSubclassProc(hwnd, message, wparam, lparam);
    }
    private void Drain()
    {
        VerifyThread();
        var previous = Current;
        SetSynchronizationContext(this);
        try { while (_queue.TryDequeue(out var action)) action(); }
        finally { SetSynchronizationContext(previous); }
    }
    // Only after the native render worker has joined. Pump our cleanup queue, never
    // arbitrary application input or render work, while async coordinator leases drain.
    internal void DrainShutdown(Task completion)
    {
        VerifyThread();
        var timer = System.Diagnostics.Stopwatch.StartNew();
        while (!completion.IsCompleted)
        {
            Drain();
            if (timer.Elapsed > TimeSpan.FromSeconds(10)) throw new TimeoutException("PlatformView owner cleanup did not drain.");
            Thread.Sleep(1);
        }
        Drain();
        completion.GetAwaiter().GetResult();
    }
    public void Dispose()
    {
        VerifyThread();
        lock (_gate)
        {
            if (_window == 0) return;
            if (!_queue.IsEmpty) throw new InvalidOperationException("PlatformView UI queue must drain before destruction.");
            Native.RemoveWindowSubclass(_window, _callback, 1);
            Native.DestroyWindow(_window);
            _window = 0;
        }
        GC.KeepAlive(_callback);
    }
    private static class Native
    {
        internal delegate nint SubclassProc(nint hwnd, uint message, nuint wparam, nint lparam, nuint id, nuint data);
        [DllImport("kernel32.dll")] internal static extern uint GetCurrentThreadId();
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)] internal static extern nint CreateWindowExW(uint ex, string cls, string text, uint style, int x, int y, int w, int h, nint parent, nint menu, nint instance, nint parameter);
        [DllImport("user32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool PostMessageW(nint hwnd, uint message, nuint wparam, nint lparam);
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool DestroyWindow(nint hwnd);
        [DllImport("comctl32.dll")] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool SetWindowSubclass(nint hwnd, SubclassProc callback, nuint id, nuint data);
        [DllImport("comctl32.dll")] [return: MarshalAs(UnmanagedType.Bool)] internal static extern bool RemoveWindowSubclass(nint hwnd, SubclassProc callback, nuint id);
        [DllImport("comctl32.dll")] internal static extern nint DefSubclassProc(nint hwnd, uint message, nuint wparam, nint lparam);
    }
}
