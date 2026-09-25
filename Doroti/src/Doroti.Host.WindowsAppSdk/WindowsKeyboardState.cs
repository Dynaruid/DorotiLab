using Doroti.Ui;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>Tracks a key's initial identity until release or focus loss.</summary>
internal sealed class WindowsKeyboardState
{
    private readonly Dictionary<long, long> _pressed = [];

    internal KeyData Apply(TimeSpan timestamp, KeyEventType type, long scanCode,
        long virtualKey, string character)
    {
        var physical = WindowsKeyMap.Physical(scanCode, virtualKey);
        var logical = WindowsKeyMap.Logical(scanCode, virtualKey, character);
        lock (_pressed)
        {
            if (type == KeyEventType.up)
            {
                if (_pressed.Remove(physical, out var initial)) logical = initial;
            }
            else
            {
                if (_pressed.TryGetValue(physical, out var initial)) logical = initial;
                _pressed[physical] = logical;
            }
        }
        return new(1, timestamp, type, physical, logical, false,
            type == KeyEventType.up || string.IsNullOrEmpty(character) ? null : character);
    }

    internal KeyData[] ReleaseAll(TimeSpan timestamp)
    {
        lock (_pressed)
        {
            var released = _pressed.Select(pair => new KeyData(
                1, timestamp, KeyEventType.up, pair.Key, pair.Value, true)).ToArray();
            _pressed.Clear();
            return released;
        }
    }
}
