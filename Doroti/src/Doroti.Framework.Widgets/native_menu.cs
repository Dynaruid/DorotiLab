using Doroti.Runtime;
using Doroti.Ui;
namespace Doroti.Framework.Widgets;

/// <summary>Chooses native menus only after capability evaluation; native failures are propagated.</summary>
public static class NativeMenuLibrary
{
    public static bool TryShow(BuildContext context, IReadOnlyList<Doroti.Ui.PlatformMenuItem> items, Rect anchor,
        PlatformMenuPresentation presentation, out Future<PlatformMenuResult>? result)
    {
        result = null;
        if (!Enum.IsDefined(presentation)) throw new ArgumentOutOfRangeException(nameof(presentation));
        if (presentation == PlatformMenuPresentation.Overlay) return false;
        var view = View.of(context);
        var invocation = DorotiUiInvocation.Managed("Widgets.NativeMenu.show");
        var windows = view.GetCapabilityOrDefault<IWindowService>(DorotiCapabilityIds.WindowService);
        var owner = WindowScope.maybeOf(context) ?? windows?.GetWindows().SingleOrDefault(x => x.ViewId == view.viewId && !x.Closed);
        if (windows is not null && (owner is null || owner.Closed || owner.ViewId != view.viewId || !windows.GetWindows().Any(x => x.Id == owner.Id && !x.Closed)))
            throw new InvalidOperationException("The menu caller does not belong to a live application window.");
        var capability = view.GetCapabilityOrDefault<IPlatformMenuHostCapability>(DorotiCapabilityIds.PlatformMenu);
        if (capability is null || owner is null)
        {
            if (presentation == PlatformMenuPresentation.Native) throw new NotSupportedException("This view has no native menu capability.");
            return false;
        }
        var request = new PlatformMenuRequest(owner.Id, items, anchor, presentation);
        var evaluation = capability.Evaluate(request);
        if (evaluation.Availability == WindowAvailability.Unsupported)
        {
            if (presentation == PlatformMenuPresentation.Native) evaluation.RequireSupported();
            return false;
        }
        result = Future<PlatformMenuResult>.fromTask(view.InvokeCapabilityAsync<IPlatformMenuHostCapability, PlatformMenuResult>(
            DorotiCapabilityIds.PlatformMenu, invocation, (host, token) => host.ShowAsync(request, token)).AsTask());
        return true;
    }
}
