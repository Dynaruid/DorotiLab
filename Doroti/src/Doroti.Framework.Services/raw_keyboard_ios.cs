// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/raw_keyboard_ios.dart
using Doroti.Runtime;

namespace Doroti.Framework.Services;

public class RawKeyEventDataIos : RawKeyEventData
{
    public virtual string characters { get; private set; } = default!;
    public virtual string charactersIgnoringModifiers { get; private set; } = default!;
    public virtual long keyCode { get; private set; } = default!;
    public virtual long modifiers { get; private set; } = default!;
    public const long modifierCapsLock = 65536L;
    public const long modifierShift = 131072L;
    public const long modifierLeftShift = 2L;
    public const long modifierRightShift = 4L;
    public const long modifierControl = 262144L;
    public const long modifierLeftControl = 1L;
    public const long modifierRightControl = 8192L;
    public const long modifierOption = 524288L;
    public const long modifierLeftOption = 32L;
    public const long modifierRightOption = 64L;
    public const long modifierCommand = 1048576L;
    public const long modifierLeftCommand = 8L;
    public const long modifierRightCommand = 16L;
    public const long modifierNumericPad = 2097152L;
    public const long modifierHelp = 4194304L;
    public const long modifierFunction = 8388608L;
    public const long deviceIndependentMask = 4294901760L;

    public RawKeyEventDataIos(
        string characters = "",
        string charactersIgnoringModifiers = "",
        long keyCode = 0,
        long modifiers = 0
    )
    {
        this.characters = characters;
        this.charactersIgnoringModifiers = charactersIgnoringModifiers;
        this.keyCode = keyCode;
        this.modifiers = modifiers;
    }

    public override string keyLabel => charactersIgnoringModifiers;
    public override PhysicalKeyboardKey physicalKey =>
        Keyboard_maps_gLibrary.kIosToPhysicalKey.GetValueOrDefault(keyCode)
        ?? new PhysicalKeyboardKey(LogicalKeyboardKey.iosPlane + keyCode);
    public override LogicalKeyboardKey logicalKey
    {
        get
        {
            LogicalKeyboardKey? numPadKey = Keyboard_maps_gLibrary.kIosNumPadMap.GetValueOrDefault(
                keyCode
            );
            if (numPadKey is not null)
            {
                return numPadKey;
            }
            LogicalKeyboardKey? specialKey =
                Keyboard_maps_gLibrary.kIosSpecialLogicalMap.GetValueOrDefault(keyLabel);
            if (specialKey is not null)
            {
                return specialKey;
            }
            LogicalKeyboardKey? knownKey =
                Keyboard_maps_gLibrary.kIosToLogicalKey.GetValueOrDefault(keyCode);
            if (knownKey is not null)
            {
                return knownKey;
            }
            if (
                (keyLabel.Length != 0)
                && !LogicalKeyboardKey.isControlCharacter(keyLabel)
                && !_isUnprintableKey(keyLabel)
            )
            {
                DartRuntimePrimitives.Assert(() => charactersIgnoringModifiers.Length <= 2L);
                long codeUnit = charactersIgnoringModifiers.codeUnitAt(0L);
                if (charactersIgnoringModifiers.Length == 2L)
                {
                    long secondCode = charactersIgnoringModifiers.codeUnitAt(1L);
                    codeUnit = (codeUnit << (int)16L) | secondCode;
                }
                long keyId =
                    LogicalKeyboardKey.unicodePlane | (codeUnit & LogicalKeyboardKey.valueMask);
                return LogicalKeyboardKey.findKeyByKeyId(keyId) ?? new LogicalKeyboardKey(keyId);
            }
            return new LogicalKeyboardKey(keyCode | LogicalKeyboardKey.iosPlane);
        }
    }

    internal static bool _isUnprintableKey(string label)
    {
        if (label.Length != 1L)
        {
            return false;
        }
        long codeUnit = label.codeUnitAt(0L);
        return (codeUnit >= 63232L) && (codeUnit <= 63743L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isLeftRightModifierPressed(
        KeyboardSide side,
        long anyMask,
        long leftMask,
        long rightMask
    )
    {
        if ((modifiers & anyMask) == 0L)
        {
            return false;
        }
        if ((modifiers & (leftMask | rightMask | anyMask)) == anyMask)
        {
            return true;
        }
        return side switch
        {
            var __case6195 when Equals(__case6195, KeyboardSide.any) => true,
            var __case6227 when Equals(__case6227, KeyboardSide.all) => (
                (modifiers & leftMask) != 0L
            ) && ((modifiers & rightMask) != 0L),
            var __case6310 when Equals(__case6310, KeyboardSide.left) => (modifiers & leftMask)
                != 0L,
            var __case6364 when Equals(__case6364, KeyboardSide.right) => (modifiers & rightMask)
                != 0L,
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isModifierPressed(ModifierKey key, KeyboardSide side = KeyboardSide.any)
    {
        long independentModifier = modifiers & deviceIndependentMask;
        bool result = default!;
        switch (key)
        {
            case var __case6640 when Equals(__case6640, ModifierKey.controlModifier):
            {
                result = _isLeftRightModifierPressed(
                    side,
                    independentModifier & modifierControl,
                    modifierLeftControl,
                    modifierRightControl
                );
                break;
            }
            case var __case6865 when Equals(__case6865, ModifierKey.shiftModifier):
            {
                result = _isLeftRightModifierPressed(
                    side,
                    independentModifier & modifierShift,
                    modifierLeftShift,
                    modifierRightShift
                );
                break;
            }
            case var __case7082 when Equals(__case7082, ModifierKey.altModifier):
            {
                result = _isLeftRightModifierPressed(
                    side,
                    independentModifier & modifierOption,
                    modifierLeftOption,
                    modifierRightOption
                );
                break;
            }
            case var __case7300 when Equals(__case7300, ModifierKey.metaModifier):
            {
                result = _isLeftRightModifierPressed(
                    side,
                    independentModifier & modifierCommand,
                    modifierLeftCommand,
                    modifierRightCommand
                );
                break;
            }
            case var __case7522 when Equals(__case7522, ModifierKey.capsLockModifier):
            {
                result = (independentModifier & modifierCapsLock) != 0L;
                break;
            }
            case var __case7881 when Equals(__case7881, ModifierKey.functionModifier):
            case var __case7922 when Equals(__case7922, ModifierKey.numLockModifier):
            case var __case7962 when Equals(__case7962, ModifierKey.symbolModifier):
            case var __case8001 when Equals(__case8001, ModifierKey.scrollLockModifier):
            {
                result = false;
                break;
            }
        }
        DartRuntimePrimitives.Assert(() => !result || (getModifierSide(key) is not null));
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override KeyboardSide? getModifierSide(ModifierKey key)
    {
        KeyboardSide? findSide(long anyMask, long leftMask, long rightMask)
        {
            long combinedMask = leftMask | rightMask;
            long combined = modifiers & combinedMask;
            if (combined == leftMask)
            {
                return KeyboardSide.left;
            }
            else
            {
                if (combined == rightMask)
                {
                    return KeyboardSide.right;
                }
                else
                {
                    if (
                        (combined == combinedMask)
                        || ((modifiers & (combinedMask | anyMask)) == anyMask)
                    )
                    {
                        return KeyboardSide.all;
                    }
                }
            }
            return null;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        switch (key)
        {
            case var __case9158 when Equals(__case9158, ModifierKey.controlModifier):
            {
                return findSide(modifierControl, modifierLeftControl, modifierRightControl);
            }
            case var __case9283 when Equals(__case9283, ModifierKey.shiftModifier):
            {
                return findSide(modifierShift, modifierLeftShift, modifierRightShift);
            }
            case var __case9400 when Equals(__case9400, ModifierKey.altModifier):
            {
                return findSide(modifierOption, modifierLeftOption, modifierRightOption);
            }
            case var __case9518 when Equals(__case9518, ModifierKey.metaModifier):
            {
                return findSide(modifierCommand, modifierLeftCommand, modifierRightCommand);
            }
            case var __case9640 when Equals(__case9640, ModifierKey.capsLockModifier):
            case var __case9681 when Equals(__case9681, ModifierKey.numLockModifier):
            case var __case9721 when Equals(__case9721, ModifierKey.scrollLockModifier):
            case var __case9764 when Equals(__case9764, ModifierKey.functionModifier):
            case var __case9805 when Equals(__case9805, ModifierKey.symbolModifier):
            {
                return KeyboardSide.all;
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.Add(new DiagnosticsProperty<string>("characters", characters));
        properties.Add(
            new DiagnosticsProperty<string>(
                "charactersIgnoringModifiers",
                charactersIgnoringModifiers
            )
        );
        properties.Add(new DiagnosticsProperty<long>("keyCode", keyCode));
        properties.Add(new DiagnosticsProperty<long>("modifiers", modifiers));
    }

    public override bool Equals(object? other)
    {
        var __other = other as RawKeyEventDataIos;
        if (__other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, __other))
        {
            return true;
        }
        if (!Equals(__other.GetType(), GetType()))
        {
            return false;
        }
        return (__other is RawKeyEventDataIos)
            && (__other.characters == characters)
            && (__other.charactersIgnoringModifiers == charactersIgnoringModifiers)
            && (__other.keyCode == keyCode)
            && (__other.modifiers == modifiers);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(
            characters,
            charactersIgnoringModifiers,
            keyCode,
            modifiers
        );
}
