// Doroti-Adapted-From: engine/src/flutter/lib/ui/painting.dart
namespace Doroti.Graphics;

/// <summary>An immutable, backend-neutral 32-bit ARGB color.</summary>
public readonly record struct Color
{
    public Color(uint value)
    {
        Value = value;
    }

    public uint Value { get; }

    public byte Alpha => (byte)(Value >> 24);

    public byte Red => (byte)(Value >> 16);

    public byte Green => (byte)(Value >> 8);

    public byte Blue => (byte)Value;

    public static Color Transparent { get; } = new(0x00000000);

    public static Color FromArgb(byte alpha, byte red, byte green, byte blue) =>
        new(((uint)alpha << 24) | ((uint)red << 16) | ((uint)green << 8) | blue);

    public static Color FromRgba(byte red, byte green, byte blue, byte alpha) => FromArgb(alpha, red, green, blue);
}
