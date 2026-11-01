namespace Doroti.Host.SharedWindows;

// A stable child list also has a stable WinRT wrapper. Recreating it for every
// UIA query creates COM wrappers even when the accessibility topology is idle.
internal sealed class WindowsPeerChildrenCache<TSource, TPeer> where TPeer : class
{
    private List<object> _identities = [];
    private IList<TPeer> _peers = Array.Empty<TPeer>();
    private bool _complete;

    internal IList<TPeer> Capture(IList<TSource> source, Func<TSource, object?> identity,
        Func<object, TPeer?> resolve)
    {
        if (_complete)
        {
            var matched = 0;
            var same = true;
            for (var i = 0; i < source.Count; i++)
            {
                if (identity(source[i]) is not { } element) continue;
                if (matched >= _identities.Count || !ReferenceEquals(_identities[matched], element))
                {
                    same = false;
                    break;
                }
                matched++;
            }
            if (same && matched == _identities.Count) return _peers;
        }

        var identities = new List<object>();
        var peers = new List<TPeer>();
        var complete = true;
        for (var i = 0; i < source.Count; i++)
        {
            if (identity(source[i]) is not { } element) continue;
            identities.Add(element);
            if (resolve(element) is { } peer) peers.Add(peer);
            else complete = false; // A not-yet-ready native child must be retried.
        }
        _identities = identities;
        _peers = peers.Count == 0 ? Array.Empty<TPeer>() : peers.AsReadOnly();
        _complete = complete;
        return _peers;
    }
}
