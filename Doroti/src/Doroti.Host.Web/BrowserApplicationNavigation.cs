using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Text.Json;
using Doroti.Hosting;
using Doroti.Ui;

namespace Doroti.Host.Web;

[SupportedOSPlatform("browser")]
internal static partial class BrowserInterop
{
    [JSImport("openApplicationNavigation", Module)]
    internal static partial string? OpenApplicationNavigation(int hostId, string? restorationId);
    [JSImport("reportApplicationRoute", Module)]
    internal static partial void ReportApplicationRoute(int hostId, string route, string? state, bool replace);
    [JSImport("saveApplicationRestoration", Module)]
    internal static partial string? SaveApplicationRestoration(int hostId, string checkpoint);
    [JSImport("closeApplicationNavigation", Module)]
    internal static partial void CloseApplicationNavigation(int hostId);
    [JSExport]
    internal static void DispatchApplicationNavigation(int hostId, string json) => BrowserApplicationNavigation.Dispatch(hostId, json);
}

[SupportedOSPlatform("browser")]
internal sealed class BrowserApplicationNavigation : IDisposable
{
    private static readonly Dictionary<int, BrowserApplicationNavigation> Owners = [];
    private readonly int _hostId;
    private DorotiView? _view;
    public ApplicationNavigationHost Navigation { get; }
    private BrowserApplicationNavigation(int hostId, ApplicationNavigationOptions options, JsonElement initial)
    {
        _hostId = hostId;
        Navigation = new(initial.GetProperty("location").GetString(), initial.GetProperty("checkpoint").GetString(),
            options.RestorationId is null ? null : checkpoint =>
            {
                var error = BrowserInterop.SaveApplicationRestoration(hostId, checkpoint);
                if (error is not null) throw new IOException(error);
            }, (route, state, replace) => BrowserInterop.ReportApplicationRoute(hostId, route, state, replace),
            initial.GetProperty("state").GetString(), ApplicationActivationSource.BrowserHistory);
        Owners.Add(hostId, this);
    }
    public static BrowserApplicationNavigation? Create(int hostId, ApplicationNavigationOptions options)
    {
        var json = BrowserInterop.OpenApplicationNavigation(hostId, options.RestorationId);
        if (json is null) return null;
        try
        {
            using var initial = JsonDocument.Parse(json);
            return new(hostId, options, initial.RootElement);
        }
        catch { BrowserInterop.CloseApplicationNavigation(hostId); throw; }
    }
    public void Attach(DorotiView view) => _view = view;
    public static void Dispatch(int hostId, string json)
    {
        if (!Owners.TryGetValue(hostId, out var owner)) return;
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        var activation = new ApplicationActivation(root.GetProperty("id").GetString()!,
            root.GetProperty("location").GetString()!, ApplicationActivationSource.BrowserHistory, false,
            root.GetProperty("state").GetString());
        if (owner._view is { } view) view.DispatchPlatformEvent(() => owner.Navigation.Activate(activation));
        else owner.Navigation.Activate(activation);
    }
    public void Dispose()
    {
        if (!Owners.Remove(_hostId)) return;
        _view = null;
        Navigation.Dispose();
        BrowserInterop.CloseApplicationNavigation(_hostId);
    }
}
