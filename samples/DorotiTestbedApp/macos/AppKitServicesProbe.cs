#if MACOS
using AppKit;
using Foundation;
using Doroti.Host.Maui;
using Doroti.Ui;
using System.Text.Json;

namespace DorotiTestbedApp.MacOS;

// Compiled only into the testbed runner. Native API evidence, not physical picker/Finder input.
internal static class AppKitServicesProbe
{
    internal static async Task RunAsync(string output)
    {
        try
        {
            NSWindow? window = null;
            for (var attempt = 0; attempt < 30 && window is null; attempt++)
            {
                await Task.Delay(100);
                await AppKitUi.Invoke(() => window = NSApplication.SharedApplication.KeyWindow);
            }
            if (window is null) throw new Exception("No visible native window.");
            var directory = System.IO.Path.GetDirectoryName(output)!;
            var path = System.IO.Path.Combine(directory, "한글-large.bin");
            await using (var stream = File.Create(path))
            {
                stream.SetLength(128 * 1024 * 1024);
                stream.Position = stream.Length - 4;
                await stream.WriteAsync(new byte[] { 1, 2, 3, 4 });
            }
            AppKitReadFile? grant = null;
            await AppKitUi.Invoke(() => { using var url = NSUrl.FromFilename(path); grant = new(url); });
            var buffer = new byte[16];
            if (grant!.Length != 128 * 1024 * 1024 || await grant.ReadAsync(grant.Length - 4, buffer) != 4 || buffer[3] != 4)
                throw new Exception("Native file grant random read failed.");
            grant.Dispose();
            try { await grant.ReadAsync(0, buffer); throw new Exception("Disposed grant remained readable."); }
            catch (ObjectDisposedException) { }
            await AppKitUi.Invoke(() =>
            {
                using var directoryUrl = NSUrl.FromFilename(directory);
                try { using var invalid = new AppKitReadFile(directoryUrl); throw new Exception("Directory grant accepted."); }
                catch (NotSupportedException) { }
                File.SetUnixFileMode(path, UnixFileMode.None);
                try
                {
                    using var url = NSUrl.FromFilename(path);
                    try { using var denied = new AppKitReadFile(url); throw new Exception("Unreadable file accepted."); }
                    catch (UnauthorizedAccessException) { }
                }
                finally { File.SetUnixFileMode(path, UnixFileMode.UserRead | UnixFileMode.UserWrite); }
            });

            using var picker = new AppKitFilePicker(() => window);
            using var canceled = new CancellationTokenSource();
            canceled.Cancel();
            try { await picker.PickFilesAsync(new(), canceled.Token); throw new Exception("Pre-canceled picker opened."); }
            catch (OperationCanceledException) { }
            using var deadline = new CancellationTokenSource(TimeSpan.FromMilliseconds(750));
            try { await picker.PickFilesAsync(new(), deadline.Token); throw new Exception("Picker cancellation not propagated."); }
            catch (OperationCanceledException) { }
            var pending = picker.PickFilesAsync(new()).AsTask();
            await Task.Delay(250);
            picker.Dispose();
            try { await pending; throw new Exception("Owner disposal did not cancel picker."); }
            catch (OperationCanceledException) { }

            await AppKitUi.Invoke(() =>
            {
                using var board = NSPasteboard.CreateWithUniqueName();
                using var item = new NSPasteboardItem();
                using var url = NSUrl.FromFilename(path);
                item.SetStringForType(url.AbsoluteString!, AppKitOsDrop.FileType);
                item.SetStringForType("한글 drop text", AppKitOsDrop.TextType);
                if (!board.WriteObjects([item])) throw new Exception("Native pasteboard write failed.");
                using var data = AppKitOsDrop.Read(board, [OsDropFormats.Files, OsDropFormats.Text, OsDropFormats.UriList]);
                if (data.Files.Count != 1 || data.Files[0].Length != 128 * 1024 * 1024 || data.Text != "한글 drop text" || data.Uris.Count != 1)
                    throw new Exception("Native pasteboard decoding failed.");
                board.ReleaseGlobally();
            });
            File.WriteAllText(output, JsonSerializer.Serialize(new { fileGrant = "PASS", pickerCancellation = "PASS",
                pickerOwnerClose = "PASS", pasteboard = "PASS", deniedFile = "PASS", directoryRejected = "PASS",
                fileBytes = 128 * 1024 * 1024, physicalInput = "notVerified" }));
            File.Delete(path);
            await AppKitUi.Invoke(() => NSApplication.SharedApplication.Terminate(NSApplication.SharedApplication));
        }
        catch (Exception error)
        {
            File.WriteAllText(output + ".error", error.ToString());
            await AppKitUi.Invoke(() => NSApplication.SharedApplication.Terminate(NSApplication.SharedApplication));
        }
    }
}
#endif
