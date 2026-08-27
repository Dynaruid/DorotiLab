#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/raw_keyboard_macos.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Services;

public static partial class Raw_keyboard_macosLibrary
{
    public static long runeToLowerCase(long rune)
    {
        var utf16BmpUpperBound = 55295L;
        if ((rune > utf16BmpUpperBound))
        {
            return rune;
        }
        return char.ConvertFromUtf32(checked((int)rune)).toLowerCase().codeUnitAt(0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class RawKeyEventDataMacOs : RawKeyEventData
{
    public virtual string characters { get; private set; } = default!;
    public virtual string charactersIgnoringModifiers { get; private set; } = default!;
    public virtual long keyCode { get; private set; } = default!;
    public virtual long modifiers { get; private set; } = default!;
    public virtual long? specifiedLogicalKey { get; private set; }
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

    public RawKeyEventDataMacOs(string characters = "", string charactersIgnoringModifiers = "", long keyCode = 0, long modifiers = 0, long? specifiedLogicalKey = null)
    {
        this.characters = characters;
        this.charactersIgnoringModifiers = charactersIgnoringModifiers;
        this.keyCode = keyCode;
        this.modifiers = modifiers;
        this.specifiedLogicalKey = specifiedLogicalKey;
    }

    public override string keyLabel => charactersIgnoringModifiers;
    public override PhysicalKeyboardKey physicalKey => (global::Doroti.Framework.Services.Keyboard_maps_gLibrary.kMacOsToPhysicalKey.GetValueOrDefault(keyCode) ?? new PhysicalKeyboardKey((LogicalKeyboardKey.windowsPlane + keyCode)));
    public override LogicalKeyboardKey logicalKey
    {
        get
        {
            if (specifiedLogicalKey is long specifiedLogicalKey__value3625)
            {
                long key = DartRuntimePrimitives.RequireValue(specifiedLogicalKey__value3625);
                return (LogicalKeyboardKey.findKeyByKeyId(key) ?? new LogicalKeyboardKey(key));
            }
            LogicalKeyboardKey? numPadKey = global::Doroti.Framework.Services.Keyboard_maps_gLibrary.kMacOsNumPadMap.GetValueOrDefault(keyCode);
            if ((numPadKey is not null))
            {
                return numPadKey;
            }
            LogicalKeyboardKey? knownKey = global::Doroti.Framework.Services.Keyboard_maps_gLibrary.kMacOsToLogicalKey.GetValueOrDefault(keyCode);
            if ((knownKey is not null))
            {
                return knownKey;
            }
            long? character = default!;
            if ((keyLabel.Length != 0))
            {
                List<long> codePoints = keyLabel.runes().ToList();
                if ((((codePoints.Count == 1L) && !LogicalKeyboardKey.isControlCharacter(keyLabel)) && !_isUnprintableKey(keyLabel)))
                {
                    character = Raw_keyboard_macosLibrary.runeToLowerCase(codePoints[(int)(0L)]);
                }
            }
            if (character is long character__value5291)
            {
                long keyId = (LogicalKeyboardKey.unicodePlane | ((DartRuntimePrimitives.RequireValue(character__value5291) & LogicalKeyboardKey.valueMask)));
                return (LogicalKeyboardKey.findKeyByKeyId(keyId) ?? new LogicalKeyboardKey(keyId));
            }
            return new LogicalKeyboardKey((keyCode | LogicalKeyboardKey.macosPlane));
        }
    }
    internal virtual bool _isLeftRightModifierPressed(KeyboardSide side, long anyMask, long leftMask, long rightMask)
    {
        if (((modifiers & anyMask) == 0L))
        {
            return false;
        }
        if (((modifiers & (((leftMask | rightMask) | anyMask))) == anyMask))
        {
            return true;
        }
        return (side switch { var __case6264 when object.Equals(__case6264, KeyboardSide.any) => true, var __case6296 when object.Equals(__case6296, KeyboardSide.all) => (((modifiers & leftMask) != 0L) && ((modifiers & rightMask) != 0L)), var __case6379 when object.Equals(__case6379, KeyboardSide.left) => ((modifiers & leftMask) != 0L), var __case6433 when object.Equals(__case6433, KeyboardSide.right) => ((modifiers & rightMask) != 0L), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isModifierPressed(ModifierKey key, KeyboardSide side = KeyboardSide.any)
    {
        long independentModifier = (modifiers & deviceIndependentMask);
        bool result = default!;
        switch (key)
        {
            case var __case6715 when object.Equals(__case6715, ModifierKey.controlModifier):
                {
                    result = _isLeftRightModifierPressed(side, (independentModifier & modifierControl), modifierLeftControl, modifierRightControl);
                    break;
                }
            case var __case6940 when object.Equals(__case6940, ModifierKey.shiftModifier):
                {
                    result = _isLeftRightModifierPressed(side, (independentModifier & modifierShift), modifierLeftShift, modifierRightShift);
                    break;
                }
            case var __case7157 when object.Equals(__case7157, ModifierKey.altModifier):
                {
                    result = _isLeftRightModifierPressed(side, (independentModifier & modifierOption), modifierLeftOption, modifierRightOption);
                    break;
                }
            case var __case7375 when object.Equals(__case7375, ModifierKey.metaModifier):
                {
                    result = _isLeftRightModifierPressed(side, (independentModifier & modifierCommand), modifierLeftCommand, modifierRightCommand);
                    break;
                }
            case var __case7597 when object.Equals(__case7597, ModifierKey.capsLockModifier):
                {
                    result = ((independentModifier & modifierCapsLock) != 0L);
                    break;
                }
            case var __case7960 when object.Equals(__case7960, ModifierKey.functionModifier):
            case var __case8001 when object.Equals(__case8001, ModifierKey.numLockModifier):
            case var __case8041 when object.Equals(__case8041, ModifierKey.symbolModifier):
            case var __case8080 when object.Equals(__case8080, ModifierKey.scrollLockModifier):
                {
                    result = false;
                    break;
                }
        }
        DartRuntimePrimitives.Assert(() => (!result || (getModifierSide(key) is not null)));
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override KeyboardSide? getModifierSide(ModifierKey key)
    {
        KeyboardSide? findSide(long anyMask, long leftMask, long rightMask)
        {
            long combinedMask = (leftMask | rightMask);
            long combined = (modifiers & combinedMask);
            if ((combined == leftMask))
            {
                return KeyboardSide.left;
            }
            else
            {
                if ((combined == rightMask))
                {
                    return KeyboardSide.right;
                }
                else
                {
                    if (((combined == combinedMask) || ((modifiers & ((combinedMask | anyMask))) == anyMask)))
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
            case var __case9241 when object.Equals(__case9241, ModifierKey.controlModifier):
                {
                    return findSide(modifierControl, modifierLeftControl, modifierRightControl);
                }
            case var __case9366 when object.Equals(__case9366, ModifierKey.shiftModifier):
                {
                    return findSide(modifierShift, modifierLeftShift, modifierRightShift);
                }
            case var __case9483 when object.Equals(__case9483, ModifierKey.altModifier):
                {
                    return findSide(modifierOption, modifierLeftOption, modifierRightOption);
                }
            case var __case9601 when object.Equals(__case9601, ModifierKey.metaModifier):
                {
                    return findSide(modifierCommand, modifierLeftCommand, modifierRightCommand);
                }
            case var __case9723 when object.Equals(__case9723, ModifierKey.capsLockModifier):
            case var __case9764 when object.Equals(__case9764, ModifierKey.numLockModifier):
            case var __case9804 when object.Equals(__case9804, ModifierKey.scrollLockModifier):
            case var __case9847 when object.Equals(__case9847, ModifierKey.functionModifier):
            case var __case9888 when object.Equals(__case9888, ModifierKey.symbolModifier):
                {
                    return KeyboardSide.all;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldDispatchEvent()
    {
        return (!object.Equals(logicalKey, LogicalKeyboardKey.fn));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.Add(new DiagnosticsProperty<string>("characters", characters));
        properties.Add(new DiagnosticsProperty<string>("charactersIgnoringModifiers", charactersIgnoringModifiers));
        properties.Add(new DiagnosticsProperty<long>("keyCode", keyCode));
        properties.Add(new DiagnosticsProperty<long>("modifiers", modifiers));
        properties.Add(new DiagnosticsProperty<long?>("specifiedLogicalKey", specifiedLogicalKey, defaultValue: null));
    }

    public override bool Equals(object? other)
    {
        var __other = other as RawKeyEventDataMacOs;
        if (__other is null) return false;
        if (ReferenceEquals(this, __other))
        {
            return true;
        }
        if ((!object.Equals(__other.GetType(), this.GetType())))
        {
            return false;
        }
        return (((((__other is RawKeyEventDataMacOs) && (((RawKeyEventDataMacOs)__other).characters == characters)) && (((RawKeyEventDataMacOs)__other).charactersIgnoringModifiers == charactersIgnoringModifiers)) && (((RawKeyEventDataMacOs)__other).keyCode == keyCode)) && (((RawKeyEventDataMacOs)__other).modifiers == modifiers));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(characters, charactersIgnoringModifiers, keyCode, modifiers);
    internal static bool _isUnprintableKey(string label)
    {
        if ((label.Length != 1L))
        {
            return false;
        }
        long codeUnit = label.codeUnitAt(0L);
        return ((codeUnit >= 63232L) && (codeUnit <= 63743L));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

