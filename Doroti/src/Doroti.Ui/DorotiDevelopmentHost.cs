namespace Doroti.Ui;

/// <summary>Provider connection to the active SDK development receipt and prepared request.
/// Request correlation does not apply code or grant process ownership.</summary>
public static class DorotiDevelopmentHost
{
    public static string Status => DorotiDevelopmentSession.Status;
    public static bool Prepare(string runtimeId, string requestId) => DorotiDevelopmentSession.Prepare(runtimeId, requestId);
}
