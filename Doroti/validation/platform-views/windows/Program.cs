using System.Runtime.InteropServices;
using Doroti.Hosting;
using Doroti.Host.WindowsAppSdk;
using Doroti.Ui;
using Rect = Doroti.Ui.Rect;

internal static class Program
{
    [STAThread]
    private static int Main()
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        var exitCode = 0;
        using var window = new Form { Text = "Doroti PlatformView native attachment validation", Width = 600, Height = 350 };
        window.Shown += async (_, _) =>
        {
            PlatformViewCoordinator? coordinator = null;
            try
            {
                var button = new WindowsHwndPlatformViewFactory(window.Handle, false, true);
                var editor = new WindowsHwndPlatformViewFactory(window.Handle, true, true);
                var parent = window.Handle;
                await Task.Run(() => Reject(() => new WindowsHwndPlatformViewFactory(parent, false, true),
                    "foreign UI thread accepted"));
                Reject(() => new WindowsHwndPlatformViewFactory(0, false, true), "invalid parent accepted");
                if (button.QuerySupport(new(1, editor.ViewType)).Supported ||
                    new WindowsHwndPlatformViewFactory(parent, false, false).QuerySupport(new(1, button.ViewType)).Supported)
                    throw new Exception("unsupported view type/topology advertised");
                coordinator = new(1, "Windows-HWND-probe", new([button, editor]), new Dispatcher(SynchronizationContext.Current!));
                var handle = await coordinator.CreateAsync(new(1, button.ViewType));
                var text = await coordinator.CreateAsync(new(2, editor.ViewType));
                await RejectAsync(() => coordinator.SetFocusAsync(text, true), "unattached HWND accepted focus");
                if (button.QuerySupport(new(1, button.ViewType, PlatformViewComposition.InterleavedComposition)).Supported)
                    throw new Exception("generic HWND incorrectly advertised interleaved composition");
                var bounds = new PlatformViewPlacement(handle, Rect.fromLTWH(30, 30, 160, 50), PlatformViewTransform.Identity, null, 0);
                await coordinator.AttachAsync(bounds);
                await coordinator.AttachAsync(bounds with { Handle = text, Bounds = Rect.fromLTWH(30, 100, 200, 50) });
                var buttonHwnd = FindWindowExW(window.Handle, 0, "BUTTON", null);
                var editorHwnd = FindWindowExW(window.Handle, 0, "EDIT", null);
                if (buttonHwnd == 0 || editorHwnd == 0 || !IsWindowVisible(buttonHwnd)) throw new Exception("native attachment missing");
                int focusCount = 0;
                coordinator.ViewFocused += focused => { if (focused == text) focusCount++; };
                await coordinator.SetFocusAsync(text, true);
                if (GetFocus() != editorHwnd || focusCount != 1) throw new Exception("focus callback routing failed");
                await coordinator.SetFocusAsync(text, false);
                SetWindowTextW(editorHwnd, "preserved native text");
                await coordinator.SetFocusAsync(text, true);
                var editorBounds = bounds with { Handle = text, Bounds = Rect.fromLTWH(30, 100, 200, 50) };
                await coordinator.AttachAsync(editorBounds with { Clip = Rect.fromLTWH(500, 500, 10, 10) });
                if (IsWindowVisible(editorHwnd) || GetFocus() == editorHwnd)
                    throw new Exception("fully clipped HWND remained visible/focused");
                await RejectAsync(() => coordinator.SetFocusAsync(text, true), "fully clipped HWND accepted focus");
                await coordinator.AttachAsync(editorBounds with { Clip = Rect.fromLTWH(30, 100, 0.000001, 50) });
                if (IsWindowVisible(editorHwnd)) throw new Exception("zero-pixel clip remained visible");
                await RejectAsync(() => coordinator.SetFocusAsync(text, true), "zero-pixel clip accepted focus");
                await coordinator.AttachAsync(editorBounds with { Clip = Rect.fromLTWH(1e20, 1e20, 1e10, 1e10) });
                if (IsWindowVisible(editorHwnd)) throw new Exception("far disjoint clip remained visible");
                await coordinator.AttachAsync(editorBounds);
                await coordinator.SetFocusAsync(text, true);
                await coordinator.AttachAsync(editorBounds with { Visible = false });
                if (IsWindowVisible(editorHwnd) || GetFocus() == editorHwnd)
                    throw new Exception("hidden HWND remained visible/focused");
                await coordinator.AttachAsync(editorBounds);
                await coordinator.SetFocusAsync(text, true);
                await coordinator.DetachAsync(text);
                if (GetFocus() == editorHwnd) throw new Exception("detached HWND retained focus");
                await RejectAsync(() => coordinator.SetFocusAsync(text, true), "detached HWND accepted focus");
                await coordinator.AttachAsync(editorBounds);
                var preserved = new System.Text.StringBuilder(128);
                GetWindowTextW(editorHwnd, preserved, preserved.Capacity);
                if (FindWindowExW(parent, 0, "EDIT", null) != editorHwnd || preserved.ToString() != "preserved native text")
                    throw new Exception("hide/clip/detach lost native identity or editing state");
                using (var otherWindow = new Form())
                {
                    SetParent(editorHwnd, otherWindow.Handle);
                    try { await RejectAsync(() => coordinator.AttachAsync(editorBounds), "foreign reparent accepted"); }
                    finally { SetParent(editorHwnd, parent); }
                }
                await coordinator.AttachAsync(editorBounds);
                await coordinator.AttachAsync(bounds with { Bounds = Rect.fromLTWH(70, 40, 180, 45), Clip = Rect.fromLTWH(80, 45, 100, 30) });
                var dpi = GetDpiForWindow(parent) / 96d;
                GetWindowRect(buttonHwnd, out var nativeBounds);
                var origin = new Point(nativeBounds.Left, nativeBounds.Top);
                ScreenToClient(parent, ref origin);
                if (origin.X != (int)Math.Round(70 * dpi) || origin.Y != (int)Math.Round(40 * dpi) ||
                    nativeBounds.Right - nativeBounds.Left != (int)Math.Round(250 * dpi) - origin.X)
                    throw new Exception("HWND physical bounds applied DPI incorrectly");
                await coordinator.DetachAsync(handle);
                if (IsWindowVisible(buttonHwnd)) throw new Exception("detach did not hide HWND");
                await coordinator.AttachAsync(bounds);
                await coordinator.DisposeAsync(handle);
                if (IsWindow(buttonHwnd)) throw new Exception("HWND survived native disposal");
                await coordinator.DisposeAsync(text);
                for (int i = 0; i < 100; i++)
                {
                    var current = await coordinator.CreateAsync(new(1, button.ViewType));
                    await coordinator.AttachAsync(bounds with { Handle = current });
                    await coordinator.DisposeAsync(current);
                }
                if (coordinator.LiveInstanceCount != 0 || FindWindowExW(window.Handle, 0, "BUTTON", null) != 0)
                    throw new Exception("native 100-cycle leak");
                using (var closingParent = new Form())
                {
                    var closingFactory = new WindowsHwndPlatformViewFactory(closingParent.Handle, false, true);
                    await using var closingOwner = new PlatformViewCoordinator(2, "Windows-HWND-close-probe",
                        new([closingFactory]), new Dispatcher(SynchronizationContext.Current!));
                    var closingHandle = await closingOwner.CreateAsync(new(1, closingFactory.ViewType));
                    var closingChild = FindWindowExW(closingParent.Handle, 0, "BUTTON", null);
                    closingParent.Dispose();
                    if (IsWindow(closingChild)) throw new Exception("child survived parent destruction");
                    await closingOwner.DisposeAsync(closingHandle);
                    if (closingOwner.LiveInstanceCount != 0) throw new Exception("parent close leaked coordinator entry");
                }
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(new {
                    automated = "passed", nativeAttachment = true, focusCallbacks = focusCount,
                    hiddenFocusRejected = true, nativeStatePreserved = true, parentThreadAndReparentChecks = true,
                    actualDpi = GetDpiForWindow(parent), createDisposeCycles = 100, parentDestroyedBeforeCleanup = true,
                    productLive = "notVerified", physical = "notVerified", ime = "notVerified" }));
            }
            catch (Exception exception) { Console.Error.WriteLine(exception); exitCode = 1; }
            finally
            {
                try { if (coordinator is not null) await coordinator.DisposeAsync(); }
                catch (Exception exception) { Console.Error.WriteLine(exception); exitCode = 1; }
                window.Close();
            }
        };
        Application.Run(window);
        return exitCode;
    }
    private static void Reject(Action action, string failure)
    {
        try { action(); } catch (ArgumentException) { return; }
        throw new Exception(failure);
    }
    private static async Task RejectAsync(Func<ValueTask> action, string failure)
    {
        try { await action(); } catch (InvalidOperationException) { return; }
        throw new Exception(failure);
    }
    private sealed class Dispatcher(SynchronizationContext context) : IPlatformViewDispatcher
    {
        public ValueTask InvokeAsync(Func<ValueTask> action)
        {
            if (SynchronizationContext.Current == context) return action();
            var done = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            context.Post(async _ => { try { await action(); done.SetResult(); } catch (Exception error) { done.SetException(error); } }, null);
            return new(done.Task);
        }
    }
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern nint FindWindowExW(nint parent, nint after, string className, string? title);
    [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool IsWindowVisible(nint hwnd);
    [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool IsWindow(nint hwnd);
    [DllImport("user32.dll")] private static extern nint GetFocus();
    [DllImport("user32.dll")] private static extern nint SetParent(nint hwnd, nint parent);
    [DllImport("user32.dll")] private static extern uint GetDpiForWindow(nint hwnd);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern bool SetWindowTextW(nint hwnd, string text);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetWindowTextW(nint hwnd, System.Text.StringBuilder text, int maximum);
    [StructLayout(LayoutKind.Sequential)] private struct NativeRect { public int Left, Top, Right, Bottom; }
    [DllImport("user32.dll")] private static extern bool GetWindowRect(nint hwnd, out NativeRect bounds);
    [DllImport("user32.dll")] private static extern bool ScreenToClient(nint hwnd, ref Point point);
}
