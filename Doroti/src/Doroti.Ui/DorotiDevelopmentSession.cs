namespace Doroti.Ui;

// Shared managed state crosses the browser main/render threads without exposing
// framework types to the host. It correlates updates; it cannot apply code.
internal static class DorotiDevelopmentSession
{
    private static readonly object Gate = new();
    private static string? _runtimeId;
    private static string? _requestId;
    private static string _status = "{}";
    internal static string Status { get { lock (Gate) return _status; } }
    internal static void Publish(string runtimeId, string json)
    {
        lock (Gate)
        {
            if (_runtimeId != runtimeId) _requestId = null;
            _runtimeId = runtimeId;
            _status = json;
        }
    }
    internal static bool Prepare(string runtimeId, string requestId)
    {
        if (!Guid.TryParse(requestId, out _)) return false;
        lock (Gate)
        {
            if (_runtimeId != runtimeId) return false;
            _requestId = requestId;
            return true;
        }
    }
    internal static string? Request(string runtimeId)
    {
        lock (Gate) return _runtimeId == runtimeId ? _requestId : null;
    }
}
