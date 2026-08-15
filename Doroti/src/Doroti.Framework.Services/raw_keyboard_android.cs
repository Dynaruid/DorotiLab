#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/raw_keyboard_android.dart
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

public static partial class Raw_keyboard_androidLibrary
{
    internal static long _kCombiningCharacterMask = 2147483647L;
}

public class RawKeyEventDataAndroid : RawKeyEventData
{
    public virtual long flags { get; private set; } = default!;
    public virtual long codePoint { get; private set; } = default!;
    public virtual long plainCodePoint { get; private set; } = default!;
    public virtual long keyCode { get; private set; } = default!;
    public virtual long scanCode { get; private set; } = default!;
    public virtual long metaState { get; private set; } = default!;
    public virtual long eventSource { get; private set; } = default!;
    public virtual long vendorId { get; private set; } = default!;
    public virtual long productId { get; private set; } = default!;
    public virtual long deviceId { get; private set; } = default!;
    public virtual long repeatCount { get; private set; } = default!;
    internal const long _sourceJoystick = 16777232L;
    public const long modifierNone = 0L;
    public const long modifierAlt = 2L;
    public const long modifierLeftAlt = 16L;
    public const long modifierRightAlt = 32L;
    public const long modifierShift = 1L;
    public const long modifierLeftShift = 64L;
    public const long modifierRightShift = 128L;
    public const long modifierSym = 4L;
    public const long modifierFunction = 8L;
    public const long modifierControl = 4096L;
    public const long modifierLeftControl = 8192L;
    public const long modifierRightControl = 16384L;
    public const long modifierMeta = 65536L;
    public const long modifierLeftMeta = 131072L;
    public const long modifierRightMeta = 262144L;
    public const long modifierCapsLock = 1048576L;
    public const long modifierNumLock = 2097152L;
    public const long modifierScrollLock = 4194304L;

    public RawKeyEventDataAndroid(long flags = 0, long codePoint = 0, long plainCodePoint = 0, long keyCode = 0, long scanCode = 0, long metaState = 0, long eventSource = 0, long vendorId = 0, long productId = 0, long deviceId = 0, long repeatCount = 0)
    {
        this.flags = flags;
        this.codePoint = codePoint;
        this.plainCodePoint = plainCodePoint;
        this.keyCode = keyCode;
        this.scanCode = scanCode;
        this.metaState = metaState;
        this.eventSource = eventSource;
        this.vendorId = vendorId;
        this.productId = productId;
        this.deviceId = deviceId;
        this.repeatCount = repeatCount;
    }

    public override string keyLabel => ((plainCodePoint == 0L) ? "" : char.ConvertFromUtf32(checked((int)(plainCodePoint & Raw_keyboard_androidLibrary._kCombiningCharacterMask))));
    public override PhysicalKeyboardKey physicalKey
    {
        get
        {
            if (global::Doroti.Generated.Framework.Services.Keyboard_maps_gLibrary.kAndroidToPhysicalKey.ContainsKey(scanCode))
            {
                return global::Doroti.Generated.Framework.Services.Keyboard_maps_gLibrary.kAndroidToPhysicalKey.GetValueOrDefault(scanCode)!;
            }
            if (((eventSource & _sourceJoystick) == _sourceJoystick))
            {
                LogicalKeyboardKey? foundKey__6524 = global::Doroti.Generated.Framework.Services.Keyboard_maps_gLibrary.kAndroidToLogicalKey.GetValueOrDefault(keyCode);
                if ((object.Equals(foundKey__6524, LogicalKeyboardKey.arrowUp)))
                {
                    return PhysicalKeyboardKey.arrowUp;
                }
                if ((object.Equals(foundKey__6524, LogicalKeyboardKey.arrowDown)))
                {
                    return PhysicalKeyboardKey.arrowDown;
                }
                if ((object.Equals(foundKey__6524, LogicalKeyboardKey.arrowLeft)))
                {
                    return PhysicalKeyboardKey.arrowLeft;
                }
                if ((object.Equals(foundKey__6524, LogicalKeyboardKey.arrowRight)))
                {
                    return PhysicalKeyboardKey.arrowRight;
                }
            }
            return new PhysicalKeyboardKey((LogicalKeyboardKey.androidPlane + scanCode));
        }
    }
    public override LogicalKeyboardKey logicalKey
    {
        get
        {
            LogicalKeyboardKey? numPadKey = global::Doroti.Generated.Framework.Services.Keyboard_maps_gLibrary.kAndroidNumPadMap.GetValueOrDefault(keyCode);
            if ((numPadKey is not null))
            {
                return numPadKey;
            }
            if (((keyLabel.Length != 0) && !LogicalKeyboardKey.isControlCharacter(keyLabel)))
            {
                long combinedCodePoint__7828 = (plainCodePoint & Raw_keyboard_androidLibrary._kCombiningCharacterMask);
                long keyId__7907 = (LogicalKeyboardKey.unicodePlane | ((combinedCodePoint__7828 & LogicalKeyboardKey.valueMask)));
                return (LogicalKeyboardKey.findKeyByKeyId(keyId__7907) ?? new LogicalKeyboardKey(keyId__7907));
            }
            LogicalKeyboardKey? newKey = global::Doroti.Generated.Framework.Services.Keyboard_maps_gLibrary.kAndroidToLogicalKey.GetValueOrDefault(keyCode);
            if ((newKey is not null))
            {
                return newKey;
            }
            return new LogicalKeyboardKey((keyCode | LogicalKeyboardKey.androidPlane));
        }
    }
    internal virtual bool _isLeftRightModifierPressed(KeyboardSide side, long anyMask, long leftMask, long rightMask)
    {
        if (((metaState & anyMask) == 0L))
        {
            return false;
        }
        return (side switch { var __case8577 when object.Equals(__case8577, KeyboardSide.any) => true, var __case8609 when object.Equals(__case8609, KeyboardSide.all) => ((((metaState & leftMask) != 0L)) && (((metaState & rightMask) != 0L))), var __case8696 when object.Equals(__case8696, KeyboardSide.left) => ((metaState & leftMask) != 0L), var __case8750 when object.Equals(__case8750, KeyboardSide.right) => ((metaState & rightMask) != 0L), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isModifierPressed(ModifierKey key, KeyboardSide side = KeyboardSide.any)
    {
        return (key switch { var __case8940 when object.Equals(__case8940, ModifierKey.controlModifier) => _isLeftRightModifierPressed(side, modifierControl, modifierLeftControl, modifierRightControl), var __case9113 when object.Equals(__case9113, ModifierKey.shiftModifier) => _isLeftRightModifierPressed(side, modifierShift, modifierLeftShift, modifierRightShift), var __case9278 when object.Equals(__case9278, ModifierKey.altModifier) => _isLeftRightModifierPressed(side, modifierAlt, modifierLeftAlt, modifierRightAlt), var __case9435 when object.Equals(__case9435, ModifierKey.metaModifier) => _isLeftRightModifierPressed(side, modifierMeta, modifierLeftMeta, modifierRightMeta), var __case9596 when object.Equals(__case9596, ModifierKey.capsLockModifier) => ((metaState & modifierCapsLock) != 0L), var __case9669 when object.Equals(__case9669, ModifierKey.numLockModifier) => ((metaState & modifierNumLock) != 0L), var __case9740 when object.Equals(__case9740, ModifierKey.scrollLockModifier) => ((metaState & modifierScrollLock) != 0L), var __case9817 when object.Equals(__case9817, ModifierKey.functionModifier) => ((metaState & modifierFunction) != 0L), var __case9890 when object.Equals(__case9890, ModifierKey.symbolModifier) => ((metaState & modifierSym) != 0L), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override KeyboardSide? getModifierSide(ModifierKey key)
    {
        KeyboardSide? findSide(long anyMask, long leftMask, long rightMask)
        {
            long combinedMask = (leftMask | rightMask);
            long combined = (metaState & combinedMask);
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
                    if ((combined == combinedMask))
                    {
                        return KeyboardSide.all;
                    }
                }
            }
            if (((metaState & anyMask) != 0L))
            {
                return KeyboardSide.all;
            }
            return null;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        switch (key)
        {
            case var __case10758 when object.Equals(__case10758, ModifierKey.controlModifier):
                {
                    return findSide(modifierControl, modifierLeftControl, modifierRightControl);
                }
            case var __case10883 when object.Equals(__case10883, ModifierKey.shiftModifier):
                {
                    return findSide(modifierShift, modifierLeftShift, modifierRightShift);
                }
            case var __case11000 when object.Equals(__case11000, ModifierKey.altModifier):
                {
                    return findSide(modifierAlt, modifierLeftAlt, modifierRightAlt);
                }
            case var __case11109 when object.Equals(__case11109, ModifierKey.metaModifier):
                {
                    return findSide(modifierMeta, modifierLeftMeta, modifierRightMeta);
                }
            case var __case11222 when object.Equals(__case11222, ModifierKey.capsLockModifier):
            case var __case11263 when object.Equals(__case11263, ModifierKey.numLockModifier):
            case var __case11303 when object.Equals(__case11303, ModifierKey.scrollLockModifier):
            case var __case11346 when object.Equals(__case11346, ModifierKey.functionModifier):
            case var __case11387 when object.Equals(__case11387, ModifierKey.symbolModifier):
                {
                    return KeyboardSide.all;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.Add(new DiagnosticsProperty<long>("flags", flags));
        properties.Add(new DiagnosticsProperty<long>("codePoint", codePoint));
        properties.Add(new DiagnosticsProperty<long>("plainCodePoint", plainCodePoint));
        properties.Add(new DiagnosticsProperty<long>("keyCode", keyCode));
        properties.Add(new DiagnosticsProperty<long>("scanCode", scanCode));
        properties.Add(new DiagnosticsProperty<long>("metaState", metaState));
    }

    public override bool Equals(object? other)
    {
        var __other = other as RawKeyEventDataAndroid;
        if (__other is null) return false;
        if (ReferenceEquals(this, __other))
        {
            return true;
        }
        if ((!object.Equals(__other.GetType(), this.GetType())))
        {
            return false;
        }
        return (((((((__other is RawKeyEventDataAndroid) && (((RawKeyEventDataAndroid)__other).flags == flags)) && (((RawKeyEventDataAndroid)__other).codePoint == codePoint)) && (((RawKeyEventDataAndroid)__other).plainCodePoint == plainCodePoint)) && (((RawKeyEventDataAndroid)__other).keyCode == keyCode)) && (((RawKeyEventDataAndroid)__other).scanCode == scanCode)) && (((RawKeyEventDataAndroid)__other).metaState == metaState));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(flags, codePoint, plainCodePoint, keyCode, scanCode, metaState);
}

