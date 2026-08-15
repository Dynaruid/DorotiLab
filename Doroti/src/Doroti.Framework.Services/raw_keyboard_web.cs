#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/raw_keyboard_web.dart
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

public static partial class Raw_keyboard_webLibrary
{
    internal static string? _unicodeChar(string key)
    {
        if ((key.Length == 1L))
        {
            return key.substring(0L, 1L);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class RawKeyEventDataWeb : RawKeyEventData
{
    public virtual string code { get; private set; } = default!;
    public virtual string key { get; private set; } = default!;
    public virtual long location { get; private set; } = default!;
    public virtual long metaState { get; private set; } = default!;
    public virtual long keyCode { get; private set; } = default!;
    public const long modifierNone = 0L;
    public const long modifierShift = 1L;
    public const long modifierAlt = 2L;
    public const long modifierControl = 4L;
    public const long modifierMeta = 8L;
    public const long modifierNumLock = 16L;
    public const long modifierCapsLock = 32L;
    public const long modifierScrollLock = 64L;

    public RawKeyEventDataWeb(string code, string key, long location = 0, long? metaState = null, long keyCode = 0)
    {
        this.code = code;
        this.key = key;
        this.location = location;
        this.metaState = metaState ?? modifierNone;
        this.keyCode = keyCode;
    }

    public override string keyLabel => ((key == "Unidentified") ? "" : (Raw_keyboard_webLibrary._unicodeChar(key) ?? ""));
    public override PhysicalKeyboardKey physicalKey
    {
        get
        {
            return (global::Doroti.Generated.Framework.Services.Keyboard_maps_gLibrary.kWebToPhysicalKey.GetValueOrDefault(code) ?? new PhysicalKeyboardKey((LogicalKeyboardKey.webPlane + code.GetHashCode())));
        }
    }
    public override LogicalKeyboardKey logicalKey
    {
        get
        {
            LogicalKeyboardKey? maybeLocationKey = global::Doroti.Generated.Framework.Services.Keyboard_maps_gLibrary.kWebLocationMap.GetValueOrDefault(key)[(int)(location)];
            if ((maybeLocationKey is not null))
            {
                return maybeLocationKey;
            }
            LogicalKeyboardKey? newKey = global::Doroti.Generated.Framework.Services.Keyboard_maps_gLibrary.kWebToLogicalKey.GetValueOrDefault(key);
            if ((newKey is not null))
            {
                return newKey;
            }
            var isPrintable = (key.Length == 1L);
            if (isPrintable)
            {
                return new LogicalKeyboardKey(key.toLowerCase().codeUnitAt(0L));
            }
            return new LogicalKeyboardKey((code.GetHashCode() + LogicalKeyboardKey.webPlane));
        }
    }
    public override bool isModifierPressed(ModifierKey key, KeyboardSide side = KeyboardSide.any)
    {
        return (key switch { var __case4855 when object.Equals(__case4855, ModifierKey.controlModifier) => ((metaState & modifierControl) != 0L), var __case4926 when object.Equals(__case4926, ModifierKey.shiftModifier) => ((metaState & modifierShift) != 0L), var __case4993 when object.Equals(__case4993, ModifierKey.altModifier) => ((metaState & modifierAlt) != 0L), var __case5056 when object.Equals(__case5056, ModifierKey.metaModifier) => ((metaState & modifierMeta) != 0L), var __case5121 when object.Equals(__case5121, ModifierKey.numLockModifier) => ((metaState & modifierNumLock) != 0L), var __case5192 when object.Equals(__case5192, ModifierKey.capsLockModifier) => ((metaState & modifierCapsLock) != 0L), var __case5265 when object.Equals(__case5265, ModifierKey.scrollLockModifier) => ((metaState & modifierScrollLock) != 0L), var __case5425 when object.Equals(__case5425, ModifierKey.functionModifier) || object.Equals(__case5425, ModifierKey.symbolModifier) => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override KeyboardSide? getModifierSide(ModifierKey key)
    {
        return KeyboardSide.any;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.Add(new DiagnosticsProperty<string>("code", code));
        properties.Add(new DiagnosticsProperty<string>("key", key));
        properties.Add(new DiagnosticsProperty<long>("location", location));
        properties.Add(new DiagnosticsProperty<long>("metaState", metaState));
        properties.Add(new DiagnosticsProperty<long>("keyCode", keyCode));
    }

    public override bool Equals(object? other)
    {
        var __other = other as RawKeyEventDataWeb;
        if (__other is null) return false;
        if (ReferenceEquals(this, __other))
        {
            return true;
        }
        if ((!object.Equals(__other.GetType(), this.GetType())))
        {
            return false;
        }
        return ((((((__other is RawKeyEventDataWeb) && (((RawKeyEventDataWeb)__other).code == code)) && (((RawKeyEventDataWeb)__other).key == key)) && (((RawKeyEventDataWeb)__other).location == location)) && (((RawKeyEventDataWeb)__other).metaState == metaState)) && (((RawKeyEventDataWeb)__other).keyCode == keyCode));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(code, key, location, metaState, keyCode);
}
