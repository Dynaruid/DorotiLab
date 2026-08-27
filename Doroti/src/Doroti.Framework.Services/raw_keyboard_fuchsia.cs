#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/raw_keyboard_fuchsia.dart
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

public class RawKeyEventDataFuchsia : RawKeyEventData
{
    public virtual long hidUsage { get; private set; } = default!;
    public virtual long codePoint { get; private set; } = default!;
    public virtual long modifiers { get; private set; } = default!;
    public const long modifierNone = 0L;
    public const long modifierCapsLock = 1L;
    public const long modifierLeftShift = 2L;
    public const long modifierRightShift = 4L;
    public static long modifierShift = (modifierLeftShift | modifierRightShift);
    public const long modifierLeftControl = 8L;
    public const long modifierRightControl = 16L;
    public static long modifierControl = (modifierLeftControl | modifierRightControl);
    public const long modifierLeftAlt = 32L;
    public const long modifierRightAlt = 64L;
    public static long modifierAlt = (modifierLeftAlt | modifierRightAlt);
    public const long modifierLeftMeta = 128L;
    public const long modifierRightMeta = 256L;
    public static long modifierMeta = (modifierLeftMeta | modifierRightMeta);

    public RawKeyEventDataFuchsia(long hidUsage = 0, long codePoint = 0, long modifiers = 0)
    {
        this.hidUsage = hidUsage;
        this.codePoint = codePoint;
        this.modifiers = modifiers;
    }

    public override string keyLabel => ((codePoint == 0L) ? "" : char.ConvertFromUtf32(checked((int)codePoint)));
    public override LogicalKeyboardKey logicalKey
    {
        get
        {
            if ((codePoint != 0L))
            {
                long flutterId = (LogicalKeyboardKey.unicodePlane | (codePoint & LogicalKeyboardKey.valueMask));
                return (global::Doroti.Framework.Services.Keyboard_maps_gLibrary.kFuchsiaToLogicalKey.GetValueOrDefault(flutterId) ?? new LogicalKeyboardKey((LogicalKeyboardKey.unicodePlane | (codePoint & LogicalKeyboardKey.valueMask))));
            }
            LogicalKeyboardKey? newKey = global::Doroti.Framework.Services.Keyboard_maps_gLibrary.kFuchsiaToLogicalKey.GetValueOrDefault((hidUsage | LogicalKeyboardKey.fuchsiaPlane));
            if ((newKey is not null))
            {
                return newKey;
            }
            return new LogicalKeyboardKey((hidUsage | LogicalKeyboardKey.fuchsiaPlane));
        }
    }
    public override PhysicalKeyboardKey physicalKey => (global::Doroti.Framework.Services.Keyboard_maps_gLibrary.kFuchsiaToPhysicalKey.GetValueOrDefault(hidUsage) ?? new PhysicalKeyboardKey((LogicalKeyboardKey.fuchsiaPlane + hidUsage)));
    internal virtual bool _isLeftRightModifierPressed(KeyboardSide side, long anyMask, long leftMask, long rightMask)
    {
        if (((modifiers & anyMask) == 0L))
        {
            return false;
        }
        return (side switch { var __case4021 when object.Equals(__case4021, KeyboardSide.any) => true, var __case4053 when object.Equals(__case4053, KeyboardSide.all) => ((((modifiers & leftMask) != 0L)) && (((modifiers & rightMask) != 0L))), var __case4140 when object.Equals(__case4140, KeyboardSide.left) => ((modifiers & leftMask) != 0L), var __case4194 when object.Equals(__case4194, KeyboardSide.right) => ((modifiers & rightMask) != 0L), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isModifierPressed(ModifierKey key, KeyboardSide side = KeyboardSide.any)
    {
        switch (key)
        {
            case var __case4382 when object.Equals(__case4382, ModifierKey.controlModifier):
                {
                    return _isLeftRightModifierPressed(side, modifierControl, modifierLeftControl, modifierRightControl);
                }
            case var __case4583 when object.Equals(__case4583, ModifierKey.shiftModifier):
                {
                    return _isLeftRightModifierPressed(side, modifierShift, modifierLeftShift, modifierRightShift);
                }
            case var __case4776 when object.Equals(__case4776, ModifierKey.altModifier):
                {
                    return _isLeftRightModifierPressed(side, modifierAlt, modifierLeftAlt, modifierRightAlt);
                }
            case var __case4910 when object.Equals(__case4910, ModifierKey.metaModifier):
                {
                    return _isLeftRightModifierPressed(side, modifierMeta, modifierLeftMeta, modifierRightMeta);
                }
            case var __case5048 when object.Equals(__case5048, ModifierKey.capsLockModifier):
                {
                    return ((modifiers & modifierCapsLock) != 0L);
                }
            case var __case5139 when object.Equals(__case5139, ModifierKey.numLockModifier):
            case var __case5179 when object.Equals(__case5179, ModifierKey.scrollLockModifier):
            case var __case5222 when object.Equals(__case5222, ModifierKey.functionModifier):
            case var __case5263 when object.Equals(__case5263, ModifierKey.symbolModifier):
                {
                    return false;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override KeyboardSide? getModifierSide(ModifierKey key)
    {
        KeyboardSide? findSide(long anyMask, long leftMask, long rightMask)
        {
            long combined = (modifiers & anyMask);
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
                    if ((combined == anyMask))
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
            case var __case5848 when object.Equals(__case5848, ModifierKey.controlModifier):
                {
                    return findSide(modifierControl, modifierLeftControl, modifierRightControl);
                }
            case var __case5973 when object.Equals(__case5973, ModifierKey.shiftModifier):
                {
                    return findSide(modifierShift, modifierLeftShift, modifierRightShift);
                }
            case var __case6090 when object.Equals(__case6090, ModifierKey.altModifier):
                {
                    return findSide(modifierAlt, modifierLeftAlt, modifierRightAlt);
                }
            case var __case6199 when object.Equals(__case6199, ModifierKey.metaModifier):
                {
                    return findSide(modifierMeta, modifierLeftMeta, modifierRightMeta);
                }
            case var __case6312 when object.Equals(__case6312, ModifierKey.capsLockModifier):
                {
                    return ((((modifiers & modifierCapsLock) == 0L)) ? null : KeyboardSide.all);
                }
            case var __case6431 when object.Equals(__case6431, ModifierKey.numLockModifier):
            case var __case6471 when object.Equals(__case6471, ModifierKey.scrollLockModifier):
            case var __case6514 when object.Equals(__case6514, ModifierKey.functionModifier):
            case var __case6555 when object.Equals(__case6555, ModifierKey.symbolModifier):
                {
                    return null;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.Add(new DiagnosticsProperty<long>("hidUsage", hidUsage));
        properties.Add(new DiagnosticsProperty<long>("codePoint", codePoint));
        properties.Add(new DiagnosticsProperty<long>("modifiers", modifiers));
    }

    public override bool Equals(object? other)
    {
        var __other = other as RawKeyEventDataFuchsia;
        if (__other is null) return false;
        if (ReferenceEquals(this, __other))
        {
            return true;
        }
        if ((!object.Equals(__other.GetType(), this.GetType())))
        {
            return false;
        }
        return ((((__other is RawKeyEventDataFuchsia) && (((RawKeyEventDataFuchsia)__other).hidUsage == hidUsage)) && (((RawKeyEventDataFuchsia)__other).codePoint == codePoint)) && (((RawKeyEventDataFuchsia)__other).modifiers == modifiers));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(hidUsage, codePoint, modifiers);
}

