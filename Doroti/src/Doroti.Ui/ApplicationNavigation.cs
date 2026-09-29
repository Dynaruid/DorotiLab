namespace Doroti.Ui;

/// <summary>Opt-in persistent state namespace and optional native protocol scheme.</summary>
public sealed record ApplicationNavigationOptions(string? RestorationId = null, string? ProtocolScheme = null);

public enum ApplicationActivationSource { Launch, Protocol, BrowserHistory, AndroidIntent, UniversalLink }

/// <summary>Id identifies a delivery, not a URI. Back/forward to the same URI is a new delivery.</summary>
public sealed record ApplicationActivation(string Id, string Location,
    ApplicationActivationSource Source, bool ColdStart, string? StateJson = null);

/// <summary>Per-view navigation. Host callbacks and subscribers run on the owning framework thread.</summary>
public interface IApplicationNavigationHostCapability
{
    ApplicationActivation Current { get; }
    IDisposable Subscribe(Action<ApplicationActivation> handler);
    void ReportRoute(string location, string? stateJson, bool replace);
    bool RestorationEnabled { get; }
    ReadOnlyMemory<byte>? ReadRestoration();
    void WriteRestoration(ReadOnlyMemory<byte> data);
    void DiscardRestoration();
}
