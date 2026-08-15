#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/raw_keyboard_windows.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Services;

public static partial class Raw_keyboard_windowsLibrary
{
    internal static long _vkProcessKey = 229L;
}

public class RawKeyEventDataWindows : RawKeyEventData
{
    public virtual long keyCode { get; private set; } = default!;
    public virtual long scanCode { get; private set; } = default!;
    public virtual long characterCodePoint { get; private set; } = default!;
    public virtual long modifiers { get; private set; } = default!;
    public static long modifierShift = (1L << (int)(0L));
    public static long modifierLeftShift = (1L << (int)(1L));
    public static long modifierRightShift = (1L << (int)(2L));
    public static long modifierControl = (1L << (int)(3L));
    public static long modifierLeftControl = (1L << (int)(4L));
    public static long modifierRightControl = (1L << (int)(5L));
    public static long modifierAlt = (1L << (int)(6L));
    public static long modifierLeftAlt = (1L << (int)(7L));
    public static long modifierRightAlt = (1L << (int)(8L));
    public static long modifierLeftMeta = (1L << (int)(9L));
    public static long modifierRightMeta = (1L << (int)(10L));
    public static long modifierCaps = (1L << (int)(11L));
    public static long modifierNumLock = (1L << (int)(12L));
    public static long modifierScrollLock = (1L << (int)(13L));

    public RawKeyEventDataWindows(long keyCode = 0, long scanCode = 0, long characterCodePoint = 0, long modifiers = 0)
    {
        this.keyCode = keyCode;
        this.scanCode = scanCode;
        this.characterCodePoint = characterCodePoint;
        this.modifiers = modifiers;
    }

    public override string keyLabel => ((characterCodePoint == 0L) ? "" : char.ConvertFromUtf32(checked((int)characterCodePoint)));
    public override PhysicalKeyboardKey physicalKey => (global::Doroti.Generated.Framework.Services.Keyboard_maps_gLibrary.kWindowsToPhysicalKey.GetValueOrDefault(scanCode) ?? new PhysicalKeyboardKey((LogicalKeyboardKey.windowsPlane + scanCode)));
    public override LogicalKeyboardKey logicalKey
    {
        get
        {
            LogicalKeyboardKey? numPadKey = global::Doroti.Generated.Framework.Services.Keyboard_maps_gLibrary.kWindowsNumPadMap.GetValueOrDefault(keyCode);
            if ((numPadKey is not null))
            {
                return numPadKey;
            }
            if (((keyLabel.Length != 0) && !LogicalKeyboardKey.isControlCharacter(keyLabel)))
            {
                long keyId__3527 = (LogicalKeyboardKey.unicodePlane | ((characterCodePoint & LogicalKeyboardKey.valueMask)));
                return (LogicalKeyboardKey.findKeyByKeyId(keyId__3527) ?? new LogicalKeyboardKey(keyId__3527));
            }
            LogicalKeyboardKey? newKey = global::Doroti.Generated.Framework.Services.Keyboard_maps_gLibrary.kWindowsToLogicalKey.GetValueOrDefault(keyCode);
            if ((newKey is not null))
            {
                return newKey;
            }
            return new LogicalKeyboardKey((keyCode | LogicalKeyboardKey.windowsPlane));
        }
    }
    internal virtual bool _isLeftRightModifierPressed(KeyboardSide side, long anyMask, long leftMask, long rightMask)
    {
        if (((modifiers & (((leftMask | rightMask) | anyMask))) == 0L))
        {
            return false;
        }
        if (((modifiers & (((leftMask | rightMask) | anyMask))) == anyMask))
        {
            return true;
        }
        return (side switch { var __case4703 when object.Equals(__case4703, KeyboardSide.any) => true, var __case4735 when object.Equals(__case4735, KeyboardSide.all) => (((modifiers & leftMask) != 0L) && ((modifiers & rightMask) != 0L)), var __case4818 when object.Equals(__case4818, KeyboardSide.left) => ((modifiers & leftMask) != 0L), var __case4872 when object.Equals(__case4872, KeyboardSide.right) => ((modifiers & rightMask) != 0L), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isModifierPressed(ModifierKey key, KeyboardSide side = KeyboardSide.any)
    {
        bool result = default!;
        switch (key)
        {
            case var __case5083 when object.Equals(__case5083, ModifierKey.controlModifier):
                {
                    result = _isLeftRightModifierPressed(side, modifierControl, modifierLeftControl, modifierRightControl);
                    break;
                }
            case var __case5286 when object.Equals(__case5286, ModifierKey.shiftModifier):
                {
                    result = _isLeftRightModifierPressed(side, modifierShift, modifierLeftShift, modifierRightShift);
                    break;
                }
            case var __case5481 when object.Equals(__case5481, ModifierKey.altModifier):
                {
                    result = _isLeftRightModifierPressed(side, modifierAlt, modifierLeftAlt, modifierRightAlt);
                    break;
                }
            case var __case5617 when object.Equals(__case5617, ModifierKey.metaModifier):
                {
                    result = _isLeftRightModifierPressed(side, (modifierLeftMeta | modifierRightMeta), modifierLeftMeta, modifierRightMeta);
                    break;
                }
            case var __case5900 when object.Equals(__case5900, ModifierKey.capsLockModifier):
                {
                    result = ((modifiers & modifierCaps) != 0L);
                    break;
                }
            case var __case5989 when object.Equals(__case5989, ModifierKey.scrollLockModifier):
                {
                    result = ((modifiers & modifierScrollLock) != 0L);
                    break;
                }
            case var __case6086 when object.Equals(__case6086, ModifierKey.numLockModifier):
                {
                    result = ((modifiers & modifierNumLock) != 0L);
                    break;
                }
            case var __case6271 when object.Equals(__case6271, ModifierKey.functionModifier):
            case var __case6312 when object.Equals(__case6312, ModifierKey.symbolModifier):
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
        KeyboardSide? findSide(long leftMask, long rightMask, long anyMask)
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
            case var __case7385 when object.Equals(__case7385, ModifierKey.controlModifier):
                {
                    return findSide(modifierLeftControl, modifierRightControl, modifierControl);
                }
            case var __case7510 when object.Equals(__case7510, ModifierKey.shiftModifier):
                {
                    return findSide(modifierLeftShift, modifierRightShift, modifierShift);
                }
            case var __case7627 when object.Equals(__case7627, ModifierKey.altModifier):
                {
                    return findSide(modifierLeftAlt, modifierRightAlt, modifierAlt);
                }
            case var __case7736 when object.Equals(__case7736, ModifierKey.metaModifier):
                {
                    return findSide(modifierLeftMeta, modifierRightMeta, 0L);
                }
            case var __case7838 when object.Equals(__case7838, ModifierKey.capsLockModifier):
            case var __case7879 when object.Equals(__case7879, ModifierKey.numLockModifier):
            case var __case7919 when object.Equals(__case7919, ModifierKey.scrollLockModifier):
            case var __case7962 when object.Equals(__case7962, ModifierKey.functionModifier):
            case var __case8003 when object.Equals(__case8003, ModifierKey.symbolModifier):
                {
                    return KeyboardSide.all;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldDispatchEvent()
    {
        return (keyCode != Raw_keyboard_windowsLibrary._vkProcessKey);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.Add(new DiagnosticsProperty<long>("keyCode", keyCode));
        properties.Add(new DiagnosticsProperty<long>("scanCode", scanCode));
        properties.Add(new DiagnosticsProperty<long>("characterCodePoint", characterCodePoint));
        properties.Add(new DiagnosticsProperty<long>("modifiers", modifiers));
    }

    public override bool Equals(object? other)
    {
        var __other = other as RawKeyEventDataWindows;
        if (__other is null) return false;
        if (ReferenceEquals(this, __other))
        {
            return true;
        }
        if ((!object.Equals(__other.GetType(), this.GetType())))
        {
            return false;
        }
        return (((((__other is RawKeyEventDataWindows) && (((RawKeyEventDataWindows)__other).keyCode == keyCode)) && (((RawKeyEventDataWindows)__other).scanCode == scanCode)) && (((RawKeyEventDataWindows)__other).characterCodePoint == characterCodePoint)) && (((RawKeyEventDataWindows)__other).modifiers == modifiers));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(keyCode, scanCode, characterCodePoint, modifiers);
}

