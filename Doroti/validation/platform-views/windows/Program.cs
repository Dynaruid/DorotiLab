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
                coordinator = new(1, "Windows-HWND-probe", new([button, editor]), new Dispatcher(SynchronizationContext.Current!));
                var handle = await coordinator.CreateAsync(new(1, button.ViewType));
                var text = await coordinator.CreateAsync(new(2, editor.ViewType));
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
                await coordinator.AttachAsync(bounds with { Bounds = Rect.fromLTWH(70, 40, 180, 45), Clip = Rect.fromLTWH(80, 45, 100, 30) });
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
                Console.WriteLine("{\"automated\":\"passed\",\"nativeAttachment\":true,\"focusCallbacks\":1,\"createDisposeCycles\":100,\"productLive\":\"notVerified\",\"physical\":\"notVerified\",\"ime\":\"notVerified\"}");
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
}
