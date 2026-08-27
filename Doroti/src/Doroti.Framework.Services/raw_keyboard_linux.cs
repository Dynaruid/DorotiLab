#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/raw_keyboard_linux.dart
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

public class RawKeyEventDataLinux : RawKeyEventData
{
    public virtual KeyHelper keyHelper { get; private set; } = default!;
    public virtual long unicodeScalarValues { get; private set; } = default!;
    public virtual long scanCode { get; private set; } = default!;
    public virtual long keyCode { get; private set; } = default!;
    public virtual long modifiers { get; private set; } = default!;
    public virtual bool isDown { get; private set; } = default!;
    public virtual long? specifiedLogicalKey { get; private set; }

    public RawKeyEventDataLinux(KeyHelper keyHelper, long unicodeScalarValues = 0, long scanCode = 0, long keyCode = 0, long modifiers = 0, bool isDown = default!, long? specifiedLogicalKey = null)
    {
        this.keyHelper = keyHelper;
        this.unicodeScalarValues = unicodeScalarValues;
        this.scanCode = scanCode;
        this.keyCode = keyCode;
        this.modifiers = modifiers;
        this.isDown = isDown;
        this.specifiedLogicalKey = specifiedLogicalKey;
        System.Diagnostics.Debug.Assert((((unicodeScalarValues & ~LogicalKeyboardKey.valueMask)) == 0L));
    }

    public override string keyLabel => ((unicodeScalarValues == 0L) ? "" : char.ConvertFromUtf32(checked((int)unicodeScalarValues)));
    public override PhysicalKeyboardKey physicalKey => (global::Doroti.Framework.Services.Keyboard_maps_gLibrary.kLinuxToPhysicalKey.GetValueOrDefault(scanCode) ?? new PhysicalKeyboardKey((LogicalKeyboardKey.webPlane + scanCode)));
    public override LogicalKeyboardKey logicalKey
    {
        get
        {
            if (specifiedLogicalKey is long specifiedLogicalKey__value3838)
            {
                long key = DartRuntimePrimitives.RequireValue(specifiedLogicalKey__value3838);
                return (LogicalKeyboardKey.findKeyByKeyId(key) ?? new LogicalKeyboardKey(key));
            }
            LogicalKeyboardKey? numPadKey = keyHelper.numpadKey(keyCode);
            if ((numPadKey is not null))
            {
                return numPadKey;
            }
            if (((keyLabel.Length != 0) && !LogicalKeyboardKey.isControlCharacter(keyLabel)))
            {
                long keyId = (LogicalKeyboardKey.unicodePlane | ((unicodeScalarValues & LogicalKeyboardKey.valueMask)));
                return (LogicalKeyboardKey.findKeyByKeyId(keyId) ?? new LogicalKeyboardKey(keyId));
            }
            LogicalKeyboardKey? newKey = keyHelper.logicalKey(keyCode);
            if ((newKey is not null))
            {
                return newKey;
            }
            return new LogicalKeyboardKey((keyCode | keyHelper.platformPlane));
        }
    }
    public override bool isModifierPressed(ModifierKey key, KeyboardSide side = KeyboardSide.any)
    {
        return keyHelper.isModifierPressed(key, modifiers, side: side, keyCode: keyCode, isDown: isDown);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override KeyboardSide? getModifierSide(ModifierKey key)
    {
        return keyHelper.getModifierSide(key);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.Add(new DiagnosticsProperty<string>("toolkit", keyHelper.debugToolkit));
        properties.Add(new DiagnosticsProperty<long>("unicodeScalarValues", unicodeScalarValues));
        properties.Add(new DiagnosticsProperty<long>("scanCode", scanCode));
        properties.Add(new DiagnosticsProperty<long>("keyCode", keyCode));
        properties.Add(new DiagnosticsProperty<long>("modifiers", modifiers));
        properties.Add(new DiagnosticsProperty<bool>("isDown", isDown));
        properties.Add(new DiagnosticsProperty<long?>("specifiedLogicalKey", specifiedLogicalKey, defaultValue: null));
    }

    public override bool Equals(object? other)
    {
        var __other = other as RawKeyEventDataLinux;
        if (__other is null) return false;
        if (ReferenceEquals(this, __other))
        {
            return true;
        }
        if ((!object.Equals(__other.GetType(), this.GetType())))
        {
            return false;
        }
        return (((((((__other is RawKeyEventDataLinux) && (object.Equals(((RawKeyEventDataLinux)__other).keyHelper.GetType(), keyHelper.GetType()))) && (((RawKeyEventDataLinux)__other).unicodeScalarValues == unicodeScalarValues)) && (((RawKeyEventDataLinux)__other).scanCode == scanCode)) && (((RawKeyEventDataLinux)__other).keyCode == keyCode)) && (((RawKeyEventDataLinux)__other).modifiers == modifiers)) && (((RawKeyEventDataLinux)__other).isDown == isDown));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(keyHelper.GetType(), unicodeScalarValues, scanCode, keyCode, modifiers, isDown);
}

public interface KeyHelper
{
    public static KeyHelper Create(string toolkit)
    {
        if ((toolkit == "glfw"))
        {
            return new GLFWKeyHelper();
        }
        else
        {
            if ((toolkit == "gtk"))
            {
                return new GtkKeyHelper();
            }
            else
            {
                throw new FlutterError($"Window toolkit not recognized: {toolkit}");
            }
        }
    }

    public string debugToolkit { get; }
    public KeyboardSide getModifierSide(ModifierKey key);
    public bool isModifierPressed(ModifierKey key, long modifiers, KeyboardSide side = KeyboardSide.any, long keyCode = default!, bool isDown = default!);
    public LogicalKeyboardKey? numpadKey(long keyCode);
    public LogicalKeyboardKey? logicalKey(long keyCode);
    public long platformPlane { get; }
}

public class GLFWKeyHelper : KeyHelper
{
    public const long modifierCapsLock = 16L;
    public const long modifierShift = 1L;
    public const long modifierControl = 2L;
    public const long modifierAlt = 4L;
    public const long modifierMeta = 8L;
    public const long modifierNumericPad = 32L;

    public virtual string debugToolkit => "GLFW";
    internal virtual long _mergeModifiers(long modifiers, long keyCode, bool isDown)
    {
        var shiftLeftKeyCode = 340L;
        var shiftRightKeyCode = 344L;
        var controlLeftKeyCode = 341L;
        var controlRightKeyCode = 345L;
        var altLeftKeyCode = 342L;
        var altRightKeyCode = 346L;
        var metaLeftKeyCode = 343L;
        var metaRightKeyCode = 347L;
        var capsLockKeyCode = 280L;
        var numLockKeyCode = 282L;
        long modifierChange = (keyCode switch { var __case12068 when object.Equals(__case12068, shiftLeftKeyCode) || object.Equals(__case12068, shiftRightKeyCode) => modifierShift, var __case12130 when object.Equals(__case12130, controlLeftKeyCode) || object.Equals(__case12130, controlRightKeyCode) => modifierControl, var __case12198 when object.Equals(__case12198, altLeftKeyCode) || object.Equals(__case12198, altRightKeyCode) => modifierAlt, var __case12254 when object.Equals(__case12254, metaLeftKeyCode) || object.Equals(__case12254, metaRightKeyCode) => modifierMeta, var __case12313 when object.Equals(__case12313, capsLockKeyCode) => modifierCapsLock, var __case12356 when object.Equals(__case12356, numLockKeyCode) => modifierNumericPad, _ => 0L });
        return (isDown ? (modifiers | modifierChange) : (modifiers & ~modifierChange));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isModifierPressed(ModifierKey key, long modifiers, KeyboardSide side = KeyboardSide.any, long keyCode = default!, bool isDown = default!)
    {
        modifiers = _mergeModifiers(modifiers: modifiers, keyCode: keyCode, isDown: isDown);
        return (key switch { var __case12801 when object.Equals(__case12801, ModifierKey.controlModifier) => ((modifiers & modifierControl) != 0L), var __case12872 when object.Equals(__case12872, ModifierKey.shiftModifier) => ((modifiers & modifierShift) != 0L), var __case12939 when object.Equals(__case12939, ModifierKey.altModifier) => ((modifiers & modifierAlt) != 0L), var __case13002 when object.Equals(__case13002, ModifierKey.metaModifier) => ((modifiers & modifierMeta) != 0L), var __case13067 when object.Equals(__case13067, ModifierKey.capsLockModifier) => ((modifiers & modifierCapsLock) != 0L), var __case13140 when object.Equals(__case13140, ModifierKey.numLockModifier) => ((modifiers & modifierNumericPad) != 0L), var __case13261 when object.Equals(__case13261, ModifierKey.functionModifier) => false, var __case13306 when object.Equals(__case13306, ModifierKey.symbolModifier) => false, var __case13349 when object.Equals(__case13349, ModifierKey.scrollLockModifier) => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual KeyboardSide getModifierSide(ModifierKey key)
    {
        return KeyboardSide.all;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual LogicalKeyboardKey? numpadKey(long keyCode)
    {
        return global::Doroti.Framework.Services.Keyboard_maps_gLibrary.kGlfwNumpadMap.GetValueOrDefault(keyCode);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual LogicalKeyboardKey? logicalKey(long keyCode)
    {
        return global::Doroti.Framework.Services.Keyboard_maps_gLibrary.kGlfwToLogicalKey.GetValueOrDefault(keyCode);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long platformPlane => LogicalKeyboardKey.glfwPlane;
}

public class GtkKeyHelper : KeyHelper
{
    public static long modifierShift = (1L << (int)(0L));
    public static long modifierCapsLock = (1L << (int)(1L));
    public static long modifierControl = (1L << (int)(2L));
    public static long modifierMod1 = (1L << (int)(3L));
    public static long modifierMod2 = (1L << (int)(4L));
    public static long modifierMeta = (1L << (int)(26L));

    public virtual string debugToolkit => "GTK";
    internal virtual long _mergeModifiers(long modifiers, long keyCode, bool isDown)
    {
        var shiftLeftKeyCode = 65505L;
        var shiftRightKeyCode = 65506L;
        var controlLeftKeyCode = 65507L;
        var controlRightKeyCode = 65508L;
        var capsLockKeyCode = 65509L;
        var shiftLockKeyCode = 65510L;
        var altLeftKeyCode = 65513L;
        var altRightKeyCode = 65514L;
        var metaLeftKeyCode = 65515L;
        var metaRightKeyCode = 65516L;
        var numLockKeyCode = 65407L;
        long modifierChange = (keyCode switch { var __case17133 when object.Equals(__case17133, shiftLeftKeyCode) || object.Equals(__case17133, shiftRightKeyCode) => modifierShift, var __case17195 when object.Equals(__case17195, controlLeftKeyCode) || object.Equals(__case17195, controlRightKeyCode) => modifierControl, var __case17263 when object.Equals(__case17263, altLeftKeyCode) || object.Equals(__case17263, altRightKeyCode) => modifierMod1, var __case17320 when object.Equals(__case17320, metaLeftKeyCode) || object.Equals(__case17320, metaRightKeyCode) => modifierMeta, var __case17379 when object.Equals(__case17379, capsLockKeyCode) || object.Equals(__case17379, shiftLockKeyCode) => modifierCapsLock, var __case17442 when object.Equals(__case17442, numLockKeyCode) => modifierMod2, _ => 0L });
        return (isDown ? (modifiers | modifierChange) : (modifiers & ~modifierChange));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isModifierPressed(ModifierKey key, long modifiers, KeyboardSide side = KeyboardSide.any, long keyCode = default!, bool isDown = default!)
    {
        modifiers = _mergeModifiers(modifiers: modifiers, keyCode: keyCode, isDown: isDown);
        return (key switch { var __case17881 when object.Equals(__case17881, ModifierKey.controlModifier) => ((modifiers & modifierControl) != 0L), var __case17952 when object.Equals(__case17952, ModifierKey.shiftModifier) => ((modifiers & modifierShift) != 0L), var __case18019 when object.Equals(__case18019, ModifierKey.altModifier) => ((modifiers & modifierMod1) != 0L), var __case18083 when object.Equals(__case18083, ModifierKey.metaModifier) => ((modifiers & modifierMeta) != 0L), var __case18148 when object.Equals(__case18148, ModifierKey.capsLockModifier) => ((modifiers & modifierCapsLock) != 0L), var __case18221 when object.Equals(__case18221, ModifierKey.numLockModifier) => ((modifiers & modifierMod2) != 0L), var __case18335 when object.Equals(__case18335, ModifierKey.functionModifier) => false, var __case18380 when object.Equals(__case18380, ModifierKey.symbolModifier) => false, var __case18423 when object.Equals(__case18423, ModifierKey.scrollLockModifier) => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual KeyboardSide getModifierSide(ModifierKey key)
    {
        return KeyboardSide.all;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual LogicalKeyboardKey? numpadKey(long keyCode)
    {
        return global::Doroti.Framework.Services.Keyboard_maps_gLibrary.kGtkNumpadMap.GetValueOrDefault(keyCode);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual LogicalKeyboardKey? logicalKey(long keyCode)
    {
        return global::Doroti.Framework.Services.Keyboard_maps_gLibrary.kGtkToLogicalKey.GetValueOrDefault(keyCode);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long platformPlane => LogicalKeyboardKey.gtkPlane;
}

