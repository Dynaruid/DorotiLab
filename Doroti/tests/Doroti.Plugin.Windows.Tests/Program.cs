using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Doroti.Host.WindowsAppSdk;
using Doroti.Hosting;
using Doroti.Plugins;
using Doroti.Ui;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        if (args.Length != 1) throw new ArgumentException("Supply a temp/testing evidence directory.");
        var directory = System.IO.Path.GetFullPath(args[0]);
        if (!directory.Replace('\\', '/').Contains("/temp/testing/", StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("Expected temp/testing path.");
        Directory.CreateDirectory(directory);
        var path = System.IO.Path.Combine(directory, "picked.txt");
        File.WriteAllText(path, "Doroti OS picker read grant");
        var initialized = RoInitialize(0);
        Marshal.ThrowExceptionForHR(initialized);
        var owner = CreateWindowExW(0, "STATIC", "Doroti M4 picker regression", 0x10cf0000, 100, 100, 500, 300, 0, 0, 0, 0);
        if (owner == 0) throw new InvalidOperationException("Owner HWND missing.");
        try
        {
            using var picker = new WindowsFilePicker(owner);
            using var boundary = DorotiApplicationBoundary.Create(new("doroti.application-capabilities/v1", "native.test", "win-x64", [],
                [new("doroti.native-features", NativeFeaturesHandler.Channel, "json", DorotiCapabilityIds.PlatformPlugins,
                    new("win-x64", "Doroti.Plugins", "0.3.0-beta", "1", typeof(NativeFeaturesHandler).FullName!))]),
                typeof(Program).Assembly, [new NativeFeaturesHandler()]);
            using var capabilities = new DorotiViewCapabilities().Register<IFilePickerHostCapability>(DorotiCapabilityIds.FilePicker, picker)
                .Register<IUrlLauncherHostCapability>(DorotiCapabilityIds.UrlLauncher, new WindowsUrlLauncher());
            boundary.Configure(capabilities);
            var client = new NativeFeatures(capabilities.Require<IPlatformMessageHostCapability>(1, DorotiCapabilityIds.PlatformMessaging, new("test")));
            var selected = client.PickFilesAsync(new(Extensions: [".txt"])).AsTask();
            PumpUntil(selected, dialog =>
            {
                if (!SetDlgItemTextW(dialog, 1148, path)) return false;
                PostMessageW(dialog, 0x111, 1, 0);
                return true;
            });
            var result = selected.GetAwaiter().GetResult();
            Require(result.Status == FilePickStatus.selected && result.Files.Count == 1, $"Select failed: {result.Status} {result.Message}");
            var bytes = new byte[100];
            var read = result.Files[0].ReadAsync(0, bytes).AsTask().GetAwaiter().GetResult();
            Require(Encoding.UTF8.GetString(bytes, 0, read) == "Doroti OS picker read grant", "Selected file bytes differ.");
            result.DisposeAsync().AsTask().GetAwaiter().GetResult();
            try { result.Files[0].ReadAsync(0, bytes).AsTask().GetAwaiter().GetResult(); throw new Exception("Disposed grant still readable."); }
            catch (ObjectDisposedException) { }
            Console.WriteLine("PASS: real HWND picker selection and OS handle read/release (synthetic dialog commands).");

            var cancelled = client.PickFilesAsync(new()).AsTask();
            PumpUntil(cancelled, dialog => PostMessageW(dialog, 0x111, 2, 0));
            Require(cancelled.GetAwaiter().GetResult().Status == FilePickStatus.cancelled, "Dialog cancel failed.");
            Console.WriteLine("PASS: real dialog user-cancel result.");

            using (var cancellation = new CancellationTokenSource())
            {
                var request = client.PickFilesAsync(token: cancellation.Token).AsTask();
                PumpUntilDialog();
                cancellation.Cancel();
                try { request.GetAwaiter().GetResult(); throw new Exception("Caller cancellation returned a result."); }
                catch (OperationCanceledException) { }
                // Let the OS cancellation complete before opening another dialog.
                var timer = Stopwatch.StartNew();
                while (Dialog() != 0) { Pump(); if (timer.Elapsed.TotalSeconds > 10) throw new TimeoutException("Cancelled dialog remained open."); }
                Pump();
                Console.WriteLine("PASS: caller cancellation closes the OS picker.");
            }

            using (var server = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0))
            {
                server.Start();
                var port = ((System.Net.IPEndPoint)server.LocalEndpoint).Port;
                var receive = Task.Run(async () =>
                {
                    using var connection = await server.AcceptTcpClientAsync().WaitAsync(TimeSpan.FromSeconds(20));
                    await using var stream = connection.GetStream();
                    var request = new byte[8192];
                    var count = await stream.ReadAsync(request);
                    Require(Encoding.ASCII.GetString(request, 0, count).StartsWith("GET /doroti-plugin-test ", StringComparison.Ordinal), "Unexpected browser request.");
                    var body = "Doroti M4 URL launcher verification passed. You can close this tab.";
                    var response = Encoding.ASCII.GetBytes($"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {body.Length}\r\nConnection: close\r\n\r\n{body}");
                    await stream.WriteAsync(response);
                });
                Require(client.LaunchUrlAsync($"http://127.0.0.1:{port}/doroti-plugin-test").AsTask().GetAwaiter().GetResult().Succeeded, "Shell URL launch failed.");
                PumpUntil(receive, _ => false);
                receive.GetAwaiter().GetResult();
                Console.WriteLine("PASS: URL launch reached loopback HTTP listener through the default browser.");
            }

            var pending = client.PickFilesAsync(new()).AsTask();
            PumpUntilDialog();
            (capabilities.Require<IPlatformPluginHostCapability>(1, DorotiCapabilityIds.PlatformPlugins, new("shutdown")) as IDisposable)!.Dispose();
            picker.Dispose();
            try { pending.GetAwaiter().GetResult(); throw new Exception("Owner close returned a picker result."); }
            catch (OperationCanceledException) { }
            Console.WriteLine("PASS: owner shutdown cancels and drains real pending picker.");
        }
        finally { DestroyWindow(owner); RoUninitialize(); }
    }
    private static void Require(bool value, string message) { if (!value) throw new Exception(message); }
    private static nint Dialog()
    {
        nint found = 0;
        EnumWindows((window, _) =>
        {
            GetWindowThreadProcessId(window, out var process);
            var name = new StringBuilder(128); GetClassNameW(window, name, name.Capacity);
            if (process == Environment.ProcessId && name.ToString() == "#32770" && IsWindowVisible(window)) { found = window; return false; }
            return true;
        }, 0);
        return found;
    }
    private static void Pump()
    {
        while (PeekMessageW(out var message, 0, 0, 0, 1)) { TranslateMessage(in message); DispatchMessageW(in message); }
        Thread.Sleep(5);
    }
    private static void PumpUntilDialog()
    {
        var timer = Stopwatch.StartNew();
        while (Dialog() == 0) { Pump(); if (timer.Elapsed.TotalSeconds > 20) throw new TimeoutException("Picker dialog not displayed."); }
    }
    private static void PumpUntil(Task completion, Func<nint, bool> action)
    {
        var timer = Stopwatch.StartNew();
        var sent = false;
        while (!completion.IsCompleted)
        {
            Pump();
            if (!sent && timer.ElapsedMilliseconds > 500 && Dialog() is var dialog && dialog != 0) sent = action(dialog);
            if (timer.Elapsed.TotalSeconds > 30) throw new TimeoutException($"Picker did not complete; dialog={Dialog()}, sent={sent}.");
        }
    }
    [StructLayout(LayoutKind.Sequential)] private struct Message { public nint Window; public uint Id; public nuint WParam; public nint LParam; public uint Time; public int X, Y; public uint Private; }
    private delegate bool EnumProc(nint window, nint parameter);
    [DllImport("combase.dll")] private static extern int RoInitialize(uint mode);
    [DllImport("combase.dll")] private static extern void RoUninitialize();
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern nint CreateWindowExW(uint ex, string cls, string title, uint style, int x, int y, int width, int height, nint parent, nint menu, nint instance, nint param);
    [DllImport("user32.dll")] private static extern bool DestroyWindow(nint window);
    [DllImport("user32.dll")] private static extern bool EnumWindows(EnumProc callback, nint parameter);
    [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(nint window, out int process);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetClassNameW(nint window, StringBuilder text, int length);
    [DllImport("user32.dll")] private static extern bool IsWindowVisible(nint window);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern bool SetDlgItemTextW(nint window, int id, string text);
    [DllImport("user32.dll")] private static extern bool PostMessageW(nint window, uint message, nuint wparam, nint lparam);
    [DllImport("user32.dll")] private static extern bool PeekMessageW(out Message message, nint window, uint min, uint max, uint remove);
    [DllImport("user32.dll")] private static extern bool TranslateMessage(in Message message);
    [DllImport("user32.dll")] private static extern nint DispatchMessageW(in Message message);
}
