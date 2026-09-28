using System.Runtime.InteropServices;
using System.Text;
using Doroti.Host.WindowsAppSdk;
using Doroti.Ui;
using Forms = System.Windows.Forms;
using ComData = System.Runtime.InteropServices.ComTypes.IDataObject;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        if (args.Length == 0) throw new ArgumentException("Supply a temp/testing directory.");
        var folder = System.IO.Path.GetFullPath(args[0]);
        if (!folder.Replace('\\', '/').Contains("/temp/testing/", StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("Expected temp/testing path.");
        Directory.CreateDirectory(folder);
        var file = System.IO.Path.Combine(folder, "한글.txt");
        File.WriteAllText(file, "Doroti drop 한글", new UTF8Encoding(false));
        var other = System.IO.Path.Combine(folder, "second.txt");
        File.WriteAllText(other, "second");
        Forms.Application.SetHighDpiMode(args.Contains("--dpi-unaware") ? Forms.HighDpiMode.DpiUnaware : Forms.HighDpiMode.PerMonitorV2);
        using var window = new Forms.Form { Text = "Doroti M5-A OLE fixture", Width = 900, Height = 600 };
        var queued = new Queue<Action>();
        var events = new List<OsDropEvent>();
        var interactive = args.Contains("--interactive");
        var targetPanel = new Forms.Panel { Left = 320, Top = 30, Width = 530, Height = 500, BackColor = System.Drawing.Color.LightCyan };
        window.Controls.Add(targetPanel);
        var targetWindow = targetPanel.Handle;
        using var target = new WindowsOsDropTarget(targetWindow, action =>
        {
            if (interactive) window.BeginInvoke(action); else queued.Enqueue(action);
        });
        using var subscription = target.Register(new(OsDropAction.Copy, [OsDropFormats.Files, OsDropFormats.Text, OsDropFormats.UriList]), item =>
        {
            events.Add(item);
            if (interactive)
            {
                using var payload = item.Data;
                var details = $"{item.Phase}: {item.Action} @ {item.Offer.Position.dx:F1},{item.Offer.Position.dy:F1}";
                if (payload is not null)
                {
                    details += $"\r\nFiles: {string.Join(", ", payload.Files.Select(f => f.Name))}\r\nText: {payload.Text}\r\nURI: {string.Join(", ", payload.Uris)}";
                    File.AppendAllText(System.IO.Path.Combine(folder, "interactive.log"), details + "\n");
                }
                targetPanel.Controls[0].Text = details;
            }
        });
        var status = new Forms.Label { Dock = Forms.DockStyle.Fill, Text = "Drop here (copy)", AutoSize = false };
        targetPanel.Controls.Add(status);
        var files = new Forms.DataObject(); files.SetData(Forms.DataFormats.FileDrop, new[] { file, other });
        var text = new Forms.DataObject(Forms.DataFormats.UnicodeText, "한글 native OLE text");
        var url = new Forms.DataObject(); url.SetData("UniformResourceLocatorW", new MemoryStream(Encoding.Unicode.GetBytes("https://example.com/drop?q=한글\0")));
        if (interactive)
        {
            window.FormClosing += (_, _) => target.Dispose();
            var sources = new[] { ("Drag two files", files), ("Drag Korean text", text), ("Drag URI", url) };
            for (var index = 0; index < sources.Length; index++)
            {
                var source = sources[index];
                var label = new Forms.Label { Left = 20, Top = 50 + index * 110, Width = 260, Height = 80, Text = source.Item1, BackColor = System.Drawing.Color.LightYellow };
                label.MouseDown += (_, _) => label.DoDragDrop(source.Item2, Forms.DragDropEffects.Copy);
                window.Controls.Add(label);
            }
            Forms.Application.Run(window);
            return;
        }

        var native = (IOleDropTarget)target;
        void Drain() { while (queued.TryDequeue(out var action)) action(); }
        OleDropPoint Point(int x, int y)
        {
            var p = new OleDropPoint { X = x, Y = y }; ClientToScreen(targetWindow, ref p); return p;
        }
        uint effect = 3;
        native.DragEnter((ComData)files, 0, Point(40, 60), ref effect);
        Require(effect == 1 && events.Count == 0, "Enter negotiation/native-stack dispatch.");
        effect = 3; native.DragOver(4, Point(50, 70), ref effect);
        Require(effect == 0, "Shift-move should reject without deleting source.");
        effect = 3; native.DragOver(0, Point(80, 100), ref effect);
        Require(effect == 1, "Over copy.");
        native.DragLeave(); Drain();
        Require(events[^1].Phase == OsDropPhase.Leave, "Leave was missing.");
        var dpi = GetDpiForWindow(targetWindow);
        Require(Math.Abs(events[0].Offer.Position.dx - 40 * 96d / dpi) < .001, "Screen/client/DPI conversion.");
        effect = 1; native.Drop((ComData)files, 0, Point(40, 60), ref effect); Drain();
        Require(effect == 1, "File drop rejected.");
        var payload = events[^1].Data!;
        Require(payload.Files.Count == 2, "Multiple file drop changed.");
        var buffer = new byte[128];
        var count = payload.Files[0].ReadAsync(0, buffer).AsTask().GetAwaiter().GetResult();
        Require(Encoding.UTF8.GetString(buffer, 0, count) == "Doroti drop 한글", "File contents changed.");
        payload.Dispose();
        effect = 1; native.Drop((ComData)text, 0, Point(40, 60), ref effect); Drain();
        Require(effect == 1 && events[^1].Data?.Text == "한글 native OLE text", "Unicode text changed.");
        events[^1].Data!.Dispose();
        var large = System.IO.Path.Combine(folder, "large-sparse.bin");
        using (var handle = File.OpenHandle(large, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            if (!DeviceIoControl(handle, 0x900c4, 0, 0, 0, 0, out _, 0)) throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            RandomAccess.SetLength(handle, 5_000_000_000);
            RandomAccess.Write(handle, new byte[] { 42, 43, 44 }, 4_000_000_000);
        }
        var largeData = new Forms.DataObject(); largeData.SetData(Forms.DataFormats.FileDrop, new[] { large });
        effect = 1; native.Drop((ComData)largeData, 0, Point(40, 60), ref effect); Drain();
        var largePayload = events[^1].Data!;
        Require(effect == 1 && largePayload.Files[0].Length == 5_000_000_000, "Large file length narrowed.");
        Require(largePayload.Files[0].ReadAsync(4_000_000_000, buffer.AsMemory(0, 3)).AsTask().GetAwaiter().GetResult() == 3
            && buffer[0] == 42 && buffer[2] == 44, "Large file offset/data changed.");
        using (var cancellation = new CancellationTokenSource())
        {
            cancellation.Cancel();
            try { largePayload.Files[0].ReadAsync(0, buffer, cancellation.Token).AsTask().GetAwaiter().GetResult(); throw new Exception("Cancelled read succeeded."); }
            catch (OperationCanceledException) { }
        }
        largePayload.Dispose();
        effect = 1; native.Drop((ComData)url, 0, Point(40, 60), ref effect); Drain();
        Require(effect == 1 && events[^1].Data?.Uris.Single().Host == "example.com", "URI changed.");
        events[^1].Data!.Dispose();
        var missing = new Forms.DataObject(); missing.SetData(Forms.DataFormats.FileDrop, new[] { file, System.IO.Path.Combine(folder, "missing.txt") });
        effect = 1; native.Drop((ComData)missing, 0, Point(40, 60), ref effect); Drain();
        Require(effect == 0 && events[^1].Phase == OsDropPhase.Error, "Partial acquisition should reject and clean up.");
        effect = 1; native.Drop((ComData)files, 0, Point(40, 60), ref effect);
        target.Dispose(); Drain();
        using var replacement = new WindowsOsDropTarget(targetWindow, queued.Enqueue);
        Require(File.Exists(file) && File.Exists(other), "Receiving modified/deleted source files.");
        Console.WriteLine($"PASS: registered/revoked OLE target; CF_HDROP multiple files/read/release; real 5GB sparse file/4GB offset/cancelled read; Unicode text/URI; screen/client conversion at {dpi} DPI; copy/move/leave/error/owner close. Native callback fixture, not Explorer input.");
    }
    private static void Require(bool value, string message) { if (!value) throw new Exception(message); }
    [DllImport("user32.dll")] private static extern bool ClientToScreen(nint window, ref OleDropPoint point);
    [DllImport("user32.dll")] private static extern uint GetDpiForWindow(nint window);
    [DllImport("kernel32.dll", SetLastError = true)] private static extern bool DeviceIoControl(Microsoft.Win32.SafeHandles.SafeFileHandle handle, uint code,
        nint input, uint inputSize, nint output, uint outputSize, out uint returned, nint overlapped);
}
