using Doroti.Hosting;
using Microsoft.Maui.ApplicationModel;
#if ANDROID
using Microsoft.Maui.ApplicationModel.DataTransfer;
#elif IOS || MACCATALYST
using CoreGraphics;
using Foundation;
using UIKit;
#endif

namespace Doroti.Host.Maui;

public sealed partial class MauiTextInputBridge
{
    internal async ValueTask PerformTextActionAsync(PlatformTextAction action, string text,
        CancellationToken cancellationToken)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            cancellationToken.ThrowIfCancellationRequested();
            if (action == PlatformTextAction.SearchWeb)
            {
                if (!await Launcher.Default.OpenAsync("https://www.google.com/search?q=" + Uri.EscapeDataString(text)))
                    throw new InvalidOperationException("No application could open web search.");
                return;
            }
#if ANDROID
            if (action == PlatformTextAction.Share)
            {
                await Share.Default.RequestAsync(new ShareTextRequest(text));
                return;
            }
#elif IOS || MACCATALYST
            // Use this view's window, including apps with more than one scene.
            var source = _visualHost?.Handler?.PlatformView as UIView
                ?? _active?.Handler?.PlatformView as UIView;
            var presenter = source?.Window?.RootViewController
                ?? throw new InvalidOperationException("Text action has no attached UIKit window.");
            while (presenter.PresentedViewController is { } next) presenter = next;
            if (action == PlatformTextAction.LookUp)
            {
                await presenter.PresentViewControllerAsync(new UIReferenceLibraryViewController(text), true);
                return;
            }
            if (action == PlatformTextAction.Share)
            {
                var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                var sheet = new UIActivityViewController([new NSString(text)], null)
                {
                    CompletionWithItemsHandler = (_, _, _, _) => completion.TrySetResult(),
                };
                if (sheet.PopoverPresentationController is { } popover)
                {
                    popover.SourceView = source;
                    // A valid anchor is required on iPad, even for a custom toolbar.
                    popover.SourceRect = new CGRect(source!.Bounds.GetMidX(), source.Bounds.GetMidY(), 1, 1);
                }
                await presenter.PresentViewControllerAsync(sheet, true);
                await completion.Task.WaitAsync(cancellationToken);
                return;
            }
#endif
            throw new NotSupportedException($"Text action '{action}' is unavailable on this platform.");
        });
    }
}
