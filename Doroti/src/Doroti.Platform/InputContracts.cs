using Doroti.Graphics;

namespace Doroti.Platform;

public enum PointerDeviceKind
{
    Mouse,
    Touch,
    Pen,
}

public enum PointerPhase
{
    Added,
    Hover,
    Down,
    Move,
    Up,
    Removed,
    Cancelled,
}

[Flags]
public enum InputModifiers
{
    None = 0,
    Shift = 1,
    Control = 2,
    Alt = 4,
    Meta = 8,
}

public sealed record InputCapabilities(
    bool Mouse,
    bool Touch,
    bool Pen,
    bool Wheel,
    bool PointerCapture,
    bool PhysicalKeys,
    bool TextInput);

public readonly record struct RawPointerEvent(
    WindowId Window,
    ulong DeviceId,
    PointerDeviceKind DeviceKind,
    PointerPhase Phase,
    Offset Position,
    uint Buttons,
    TimeSpan Timestamp,
    Offset ScrollDelta = default,
    InputModifiers Modifiers = InputModifiers.None);

/// <summary>The native convention used before a wheel/trackpad delta becomes logical pixels.</summary>
public enum PlatformScrollConvention
{
    WindowsWheel,
    LinuxWheel,
    MacOsDiscrete,
    MacOsPrecise,
    AvaloniaMacOs,
    LogicalPixels,
}

/// <summary>
/// Converts platform wheel conventions to Flutter-compatible logical pixel deltas. Positive values
/// move a scroll position toward its trailing extent; timestamps and fractional values are not
/// quantized here or later in the input pipeline.
/// </summary>
public static class PointerScrollNormalizer
{
    public const double LinuxPixelsPerTick = 53;

    public const double MacOsPixelsPerLine = 40;

    public static Offset Normalize(
        Offset platformDelta,
        PlatformScrollConvention convention,
        uint windowsLinesPerScroll = 3)
    {
        if (!platformDelta.IsFinite)
        {
            throw new ArgumentOutOfRangeException(nameof(platformDelta), "Pointer scroll deltas must be finite.");
        }
        var multiplier = convention switch
        {
            PlatformScrollConvention.WindowsWheel => windowsLinesPerScroll * (100d / 3d),
            PlatformScrollConvention.LinuxWheel => LinuxPixelsPerTick,
            PlatformScrollConvention.MacOsDiscrete => MacOsPixelsPerLine,
            PlatformScrollConvention.AvaloniaMacOs => 50,
            PlatformScrollConvention.MacOsPrecise or PlatformScrollConvention.LogicalPixels => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(convention)),
        };
        return convention is PlatformScrollConvention.LogicalPixels
            ? platformDelta
            : new(-platformDelta.X * multiplier, -platformDelta.Y * multiplier);
    }

    /// <summary>Normalizes Avalonia's up-positive wheel vector for the current desktop platform.</summary>
    public static Offset NormalizeAvalonia(Offset platformDelta, uint windowsLinesPerScroll = 3)
    {
        if (OperatingSystem.IsWindows())
        {
            return Normalize(platformDelta, PlatformScrollConvention.WindowsWheel, windowsLinesPerScroll);
        }
        if (OperatingSystem.IsMacOS())
        {
            // Avalonia.Native divides precise NSEvent pixels by 50 before raising PointerWheelChanged.
            return Normalize(platformDelta, PlatformScrollConvention.AvaloniaMacOs);
        }
        return Normalize(platformDelta, PlatformScrollConvention.LinuxWheel);
    }

}

public enum KeyPhase
{
    Down,
    Repeat,
    Up,
}

public readonly record struct RawKeyEvent(
    WindowId Window,
    uint PhysicalKey,
    uint LogicalKey,
    KeyPhase Phase,
    TimeSpan Timestamp,
    InputModifiers Modifiers = InputModifiers.None);

/// <summary>Host-neutral focus transition emitted by the platform input source.</summary>
public readonly record struct RawFocusEvent(
    WindowId Window,
    bool IsFocused,
    TimeSpan Timestamp);

public interface IRawInputSink
{
    void OnPointer(RawPointerEvent input);

    void OnKey(RawKeyEvent input);

    void OnFocus(RawFocusEvent input)
    {
    }
}

public interface IRawInputSource
{
    InputCapabilities Capabilities { get; }

    void Attach(IRawInputSink sink);

    void Detach(IRawInputSink sink);
}
