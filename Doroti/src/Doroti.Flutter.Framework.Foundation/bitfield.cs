// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/foundation/bitfield.dart and _bitfield_io.dart
using Doroti.Flutter.Runtime;

namespace Doroti.Generated.Framework.Foundation;

public static class BitfieldLibrary
{
    public const long kMaxUnsignedSMI = 0x3FFFFFFFFFFFFFFF;
}

public sealed class BitField<T>
    where T : struct, Enum
{
    private readonly int _length;
    private ulong _bits;

    public BitField(int length, bool value = false)
    {
        if (length is < 0 or > 62)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }
        _length = length;
        _bits = value ? (ulong)BitfieldLibrary.kMaxUnsignedSMI : 0;
    }

    public static BitField<T> filled(int length, bool value) => new(length, value);

    public bool this[T index]
    {
        get
        {
            var value = checked((int)FoundationRuntimePorts.EnumIndex(index));
            ValidateIndex(value);
            return (_bits & (1UL << value)) != 0;
        }
        set
        {
            var enumIndex = checked((int)FoundationRuntimePorts.EnumIndex(index));
            ValidateIndex(enumIndex);
            if (value)
            {
                _bits |= 1UL << enumIndex;
            }
            else
            {
                _bits &= ~(1UL << enumIndex);
            }
        }
    }

    public void reset(bool value = false) => _bits = value ? (ulong)BitfieldLibrary.kMaxUnsignedSMI : 0;

    private void ValidateIndex(int index)
    {
        if ((uint)index >= (uint)_length)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
    }
}

internal static class _bitfield_ioLibrary
{
    internal const long kMaxUnsignedSMI = BitfieldLibrary.kMaxUnsignedSMI;
}
