#if IOS || MACCATALYST
using Foundation;
using UIKit;
using Doroti.Ui;

namespace Doroti.Host.Maui;

internal static class UIKitApplicationActivation
{
    internal static void Connect(UISceneConnectionOptions options, bool cold = true)
    {
        foreach (var context in options.UrlContexts?.ToArray() ?? []) Deliver(context.Url, false, cold);
        foreach (var activity in options.UserActivities?.ToArray() ?? []) Continue(activity, cold);
    }

    internal static void Continue(NSUserActivity activity, bool cold)
    {
        if (activity.ActivityType == NSUserActivityType.BrowsingWeb)
            Deliver(activity.WebPageUrl, true, cold);
    }

    internal static void Deliver(NSUrl? url, bool universal, bool cold)
    {
        if (url?.AbsoluteString is not { } location) return;
        try
        {
            MauiApplicationActivation.Deliver(location,
                universal ? ApplicationActivationSource.UniversalLink : ApplicationActivationSource.Protocol, cold);
        }
        catch (ArgumentException) { /* Invalid external links leave the current route unchanged. */ }
        catch (InvalidOperationException error) { System.Diagnostics.Trace.TraceWarning(error.Message); }
    }
}
#endif
