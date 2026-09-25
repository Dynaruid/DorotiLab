using System.Runtime.InteropServices;
using Doroti.Host.WindowsAppSdk;
using Doroti.Ui;

var checks = 0;
void Check(bool condition, string name)
{
    if (!condition) throw new InvalidOperationException(name);
    checks++;
}

// Layout changes alter logical identity, never physical key position.
Check(WindowsKeyMap.Physical(0x1e, 'Q') == 0x70004, "AZERTY physical A position");
Check(WindowsKeyMap.Physical(0x10, 'A') == 0x70014, "AZERTY physical Q position");
Check(WindowsKeyMap.Physical(0x15, 'Z') == 0x7001c, "QWERTZ physical Y position");
Check(WindowsKeyMap.Physical(0x27, 0xc0) == 0x70033, "OEM punctuation position");
Check(WindowsKeyMap.Physical(0x4f, 0x23) == WindowsKeyMap.Physical(0x4f, 0x61), "NumLock independent keypad 1");
Check(WindowsKeyMap.Physical(0x4f, 0x23) != WindowsKeyMap.Physical(0x14f, 0x23), "Keypad End versus navigation End");
Check(WindowsKeyMap.Physical(0x1c, 0x0d) != WindowsKeyMap.Physical(0x11c, 0x0d), "Main and keypad Enter");
Check(WindowsKeyMap.Physical(0x2a, 0x10) == 0x700e1 && WindowsKeyMap.Physical(0x36, 0x10) == 0x700e5, "Both Shift positions");
Check(WindowsKeyMap.Physical(0x11d, 0x11) == 0x700e4, "Right Control");
Check(WindowsKeyMap.Physical(0x138, 0x12) == 0x700e6, "Right Alt");
Check(WindowsKeyMap.Physical(0x45, 0x13) == 0x70048 && WindowsKeyMap.Physical(0x145, 0x90) == 0x70053, "Pause and NumLock");
Check(WindowsKeyMap.Physical(0, 0x88) != WindowsKeyMap.Physical(0, 0x89), "Distinct synthetic unknown keys");
Check(WindowsKeyMap.Physical(0, 0x88) != WindowsKeyMap.Physical(0x88, 0), "Synthetic and unknown scan namespaces");
Check(WindowsKeyMap.Physical(0, 'A') == 0x70004 && WindowsKeyMap.Physical(0, 0x7c) == 0x70068, "Synthetic known fallback");
Check(WindowsKeyMap.Logical(0x1e, 'Q', "q") == 'q', "Logical follows layout");
Check(WindowsKeyMap.Logical(0x1e, 'A', "\u0001") == 'a', "Ctrl+A retains logical letter");
Check(WindowsKeyMap.Logical(0x4f, 0x61, "1") == 8589935153L, "Printable numpad 1 identity");
Check(WindowsKeyMap.Logical(0x4e, 0x6b, "+") == 8589935147L, "Printable numpad add identity");

var keyboard = new WindowsKeyboardState();
var down = keyboard.Apply(TimeSpan.Zero, KeyEventType.down, 0x1e, 'A', "a");
var repeat = keyboard.Apply(TimeSpan.Zero, KeyEventType.repeat, 0x1e, 'Q', "q");
var up = keyboard.Apply(TimeSpan.Zero, KeyEventType.up, 0x1e, 'Q', "");
Check(down.logical == repeat.logical && repeat.logical == up.logical && repeat.character == "q", "Initial logical identity retained across repeat/release");
Check(keyboard.ReleaseAll(TimeSpan.Zero).Length == 0, "Released key absent on focus loss");
keyboard.Apply(TimeSpan.Zero, KeyEventType.down, 0x2a, 0x10, "");
keyboard.Apply(TimeSpan.Zero, KeyEventType.down, 0x36, 0x10, "");
keyboard.Apply(TimeSpan.Zero, KeyEventType.up, 0x2a, 0x10, "");
var released = keyboard.ReleaseAll(TimeSpan.FromMilliseconds(7));
Check(released.Length == 1 && released[0].physical == 0x700e5 && released[0].synthesized && released[0].type == KeyEventType.up, "Focus loss releases remaining right Shift");
Check(keyboard.ReleaseAll(TimeSpan.Zero).Length == 0, "Duplicate focus loss releases no keys");
Check(keyboard.Apply(TimeSpan.Zero, KeyEventType.down, 0x1e, 'Q', "q").logical == 'q', "New focus session uses new layout");

// Exercise the real message-only HWND, including worker enqueue and async continuation.
using var dispatcher = new WindowsPlatformViewDispatcher();
var order = new List<int>();
Task.Run(() =>
{
    dispatcher.Post(_ => order.Add(1), null);
    dispatcher.Post(_ => throw new InvalidOperationException("expected callback failure"), null);
    dispatcher.Post(_ => order.Add(2), null);
}).GetAwaiter().GetResult();
Task? invoked = null;
Task.Run(() => { invoked = dispatcher.InvokeAsync(async () =>
{
    dispatcher.VerifyThread();
    order.Add(3);
    await Task.Yield();
    dispatcher.VerifyThread();
    order.Add(4);
}).AsTask(); }).GetAwaiter().GetResult();
var wakes = PumpUntil(() => invoked!.IsCompleted);
invoked!.GetAwaiter().GetResult();
Check(order.SequenceEqual([1, 2, 3, 4]), "FIFO and continuation survive callback failure");
Check(wakes == 1, "Batch shares one native wakeup");
var secondWake = false;
Task.Run(() => dispatcher.Post(_ => secondWake = true, null)).GetAwaiter().GetResult();
PumpUntil(() => secondWake);
Check(secondWake, "Idle drain rearms wakeup");
Task? shutdownWork = null;
Task.Run(() => { shutdownWork = dispatcher.InvokeAsync(() => ValueTask.CompletedTask).AsTask(); }).GetAwaiter().GetResult();
try
{
    dispatcher.Dispose();
    throw new InvalidOperationException("Queued work was discarded on disposal");
}
catch (InvalidOperationException error) when (error.Message.Contains("must drain")) { }
dispatcher.DrainShutdown(shutdownWork!);
Check(shutdownWork!.IsCompletedSuccessfully, "Shutdown drains accepted work before disposal");
dispatcher.Dispose();
var ranAfterDispose = false;
try
{
    dispatcher.InvokeAsync(() => { ranAfterDispose = true; return ValueTask.CompletedTask; }).GetAwaiter().GetResult();
    throw new InvalidOperationException("Disposed owner invocation was accepted");
}
catch (ObjectDisposedException) { }
Check(!ranAfterDispose, "Disposed UI owner rejects invocation");
Task.Run(() =>
{
    try
    {
        dispatcher.Post(_ => { }, null);
        throw new InvalidOperationException("Disposed worker post was accepted");
    }
    catch (ObjectDisposedException) { }
}).GetAwaiter().GetResult();
Check(true, "Disposed worker rejects post");
Console.WriteLine($"PASS WindowsAppSDK contracts: {checks}");

static int PumpUntil(Func<bool> complete)
{
    var wakes = 0;
    var timer = System.Diagnostics.Stopwatch.StartNew();
    while (!complete())
    {
        if (timer.Elapsed > TimeSpan.FromSeconds(10)) throw new TimeoutException("UI dispatch did not complete");
        if (Native.PeekMessageW(out var message, 0, 0, 0, 1))
        {
            if (message.Id == 0x8731) wakes++;
            Native.DispatchMessageW(ref message);
        }
        else Thread.Sleep(1);
    }
    return wakes;
}

internal static class Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Message
    {
        internal nint Window;
        internal uint Id;
        internal nuint WParam;
        internal nint LParam;
        internal uint Time;
        internal int X;
        internal int Y;
        internal uint Private;
    }
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool PeekMessageW(out Message message, nint window, uint min, uint max, uint remove);
    [DllImport("user32.dll")]
    internal static extern nint DispatchMessageW(ref Message message);
}
