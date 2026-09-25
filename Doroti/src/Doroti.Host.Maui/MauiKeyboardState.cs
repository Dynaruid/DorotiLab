using Doroti.Ui;

namespace Doroti.Host.Maui;

internal sealed class MauiKeyboardState
{
    private readonly Dictionary<long, long> _pressed = [];

    internal KeyData? Apply(KeyData key)
    {
        if (key.type == KeyEventType.up)
        {
            // Focus loss may already have synthesized this release.
            return _pressed.Remove(key.physical, out var logical)
                ? key with { logical = logical, character = null } : null;
        }
        if (_pressed.TryGetValue(key.physical, out var initial))
        {
            return key with { type = KeyEventType.repeat, logical = initial };
        }
        _pressed.Add(key.physical, key.logical);
        // Reacquiring focus while a key is held can first deliver a repeat.
        return key with { type = KeyEventType.down };
    }

    internal KeyData[] ReleaseAll(ulong viewId, TimeSpan timestamp)
    {
        var result = _pressed.Select(pair => new KeyData(
            viewId, timestamp, KeyEventType.up, pair.Key, pair.Value, true)).ToArray();
        _pressed.Clear();
        return result;
    }
}
