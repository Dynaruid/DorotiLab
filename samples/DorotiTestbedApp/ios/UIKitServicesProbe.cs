#if IOS || MACCATALYST
using Doroti.Host.Maui;
using Doroti.Ui;
using Foundation;
using UIKit;

namespace DorotiTestbedApp.Apple;

// Native API qualification only; physical picker input and VoiceOver remain separate.
internal static class UIKitServicesProbe
{
    internal static async Task DeliverLinkAsync(string link)
    {
        try
        {
        await Task.Delay(2000);
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            var scene = UIApplication.SharedApplication.ConnectedScenes.OfType<UIWindowScene>().First();
            using var activity = new NSUserActivity(NSUserActivityType.BrowsingWeb) { WebPageUrl = new NSUrl(link) };
#if MACCATALYST
            ((DorotiMacCatalystSceneDelegate)scene.Delegate!).ContinueUserActivity(scene, activity);
#else
            ((DorotiMauiSceneDelegate)scene.Delegate!).ContinueUserActivity(scene, activity);
#endif
        });
        }
        catch (Exception error)
        {
            if (Environment.GetEnvironmentVariable("DOROTI_NAVIGATION_PROBE") is { } output)
                File.WriteAllText(output + ".error", error.ToString());
        }
    }
    internal static async Task RunAsync(string output)
    {
        try
        {
            await Task.Delay(2000);
            UIViewController? owner = null;
            await MainThread.InvokeOnMainThreadAsync(() => owner = UIApplication.SharedApplication.ConnectedScenes
                .OfType<UIWindowScene>().SelectMany(scene => scene.Windows).First(window => window.IsKeyWindow).RootViewController);
            var path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(output)!, "한글-large.bin");
            await using (var stream = File.Create(path))
            {
                stream.SetLength(128 * 1024 * 1024);
                stream.Position = stream.Length - 4;
                await stream.WriteAsync(new byte[] { 1, 2, 3, 4 });
            }
            using var url = NSUrl.FromFilename(path);
            var grant = new AppleReadFile(url);
            var buffer = new byte[16];
            if (grant.Length != 128 * 1024 * 1024 || await grant.ReadAsync(grant.Length - 4, buffer) != 4 || buffer[3] != 4)
                throw new Exception("Native file offset read failed.");
            grant.Dispose();
            try { await grant.ReadAsync(0, buffer); throw new Exception("Disposed grant readable."); }
            catch (ObjectDisposedException) { }
            using var picker = new UIKitFilePicker(() => owner);
            using var canceled = new CancellationTokenSource();
            canceled.Cancel();
            try { await picker.PickFilesAsync(new(), canceled.Token); throw new Exception("Canceled picker opened."); }
            catch (OperationCanceledException) { }
            using var deadline = new CancellationTokenSource(TimeSpan.FromMilliseconds(750));
            try { await picker.PickFilesAsync(new(), deadline.Token); throw new Exception("Picker cancellation ignored."); }
            catch (OperationCanceledException) { }
            var pending = picker.PickFilesAsync(new()).AsTask();
            await Task.Delay(250);
            picker.Dispose();
            try { await pending; throw new Exception("Owner disposal ignored."); }
            catch (OperationCanceledException) { }
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                using var drop = new UIKitOsDrop(owner!.View!, action => action());
                if (!drop.Support.CanReceive || drop.Support.CanSend || drop.Support.Actions != OsDropAction.Copy || drop.Support.Formats.Contains(OsDropFormats.Files))
                    throw new Exception("Drop capability mismatch.");
                using var registration = drop.Register(new(OsDropAction.Copy, [OsDropFormats.Text]), _ => { });
            });
            File.Delete(path);
            File.WriteAllText(output, "{\"fileRead128MiB\":true,\"cancel\":true,\"ownerDispose\":true,\"dropCapability\":true,\"physicalInput\":\"notVerified\"}");
        }
        catch (Exception error) { File.WriteAllText(output + ".error", error.ToString()); }
    }
}
#endif
