using Doroti.Ui;

namespace Doroti.Hosting;

/// <summary>Implemented by the same native instance owned by the composition coordinator.
/// Begin commands on the UI thread; completion must never hold a placement reservation.</summary>
public interface IPlatformWebViewInstance
{
    Task<WebViewResult> ExecuteAsync(WebViewCommand command, CancellationToken cancellationToken);
    event Action<WebViewEvent>? WebViewChanged;
}
