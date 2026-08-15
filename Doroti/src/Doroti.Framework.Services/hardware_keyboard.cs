#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/hardware_keyboard.dart
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

public static partial class Hardware_keyboardLibrary
{
    internal static bool _keyboardDebug(Func<string> messageFunc, Func<IEnumerable<object>>? detailsFunc = null)
    {
        if (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            throw new InvalidOperationException("_keyboardDebug was called in Release mode, which means they are called " + "without being wrapped in an assert. Always call _keyboardDebug like so:\n" + "  assert(_keyboardDebug(() => 'Blah $foo'));");
        }
        if (!global::Doroti.Generated.Framework.Services.DebugLibrary.debugPrintKeyboardEvents)
        {
            return true;
        }
        global::Doroti.Generated.Framework.Foundation.PrintLibrary.debugPrint($"KEYBOARD: {messageFunc()}");
        IEnumerable<object> details = (detailsFunc?.Invoke() ?? new List<object>());
        if ((details.Count() != 0))
        {
            foreach (var detail in details)
            {
                global::Doroti.Generated.Framework.Foundation.PrintLibrary.debugPrint($"    {detail}");
            }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public enum KeyboardLockMode
{
    numLock,
    scrollLock,
    capsLock
}

public abstract class KeyEvent : Diagnosticable
{
    public virtual PhysicalKeyboardKey physicalKey { get; private set; } = default!;
    public virtual LogicalKeyboardKey logicalKey { get; private set; } = default!;
    public virtual string? character { get; private set; }
    public virtual Duration timeStamp { get; private set; } = default!;
    public virtual KeyEventDeviceType deviceType { get; private set; } = default!;
    public virtual bool synthesized { get; private set; } = default!;

    protected KeyEvent(PhysicalKeyboardKey physicalKey, LogicalKeyboardKey logicalKey, string? character = null, Duration timeStamp = default!, KeyEventDeviceType deviceType = Dart_uiLibrary.KeyEventDeviceType.keyboard, bool synthesized = false)
    {
        this.physicalKey = physicalKey;
        this.logicalKey = logicalKey;
        this.character = character;
        this.timeStamp = timeStamp;
        this.deviceType = deviceType;
        this.synthesized = synthesized;
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.Add(new DiagnosticsProperty<PhysicalKeyboardKey>("physicalKey", physicalKey));
        properties.Add(new DiagnosticsProperty<LogicalKeyboardKey>("logicalKey", logicalKey));
        properties.Add(new StringProperty("character", character));
        properties.Add(new DiagnosticsProperty<Duration>("timeStamp", timeStamp));
        properties.Add(new FlagProperty("synthesized", value: synthesized, ifTrue: "synthesized"));
    }

}

public class KeyDownEvent : KeyEvent
{
    public KeyDownEvent(PhysicalKeyboardKey physicalKey, LogicalKeyboardKey logicalKey, string? character = null, Duration timeStamp = default!, bool synthesized = false, KeyEventDeviceType deviceType = Dart_uiLibrary.KeyEventDeviceType.keyboard) : base(physicalKey: physicalKey, logicalKey: logicalKey, character: character, timeStamp: timeStamp, synthesized: synthesized, deviceType: deviceType)
    {
    }

}

public class KeyUpEvent : KeyEvent
{
    public KeyUpEvent(PhysicalKeyboardKey physicalKey, LogicalKeyboardKey logicalKey, Duration timeStamp, bool synthesized = false, KeyEventDeviceType deviceType = Dart_uiLibrary.KeyEventDeviceType.keyboard) : base(physicalKey: physicalKey, logicalKey: logicalKey, timeStamp: timeStamp, synthesized: synthesized, deviceType: deviceType)
    {
    }

}

public class KeyRepeatEvent : KeyEvent
{
    public KeyRepeatEvent(PhysicalKeyboardKey physicalKey, LogicalKeyboardKey logicalKey, string? character = null, Duration timeStamp = default!, KeyEventDeviceType deviceType = Dart_uiLibrary.KeyEventDeviceType.keyboard) : base(physicalKey: physicalKey, logicalKey: logicalKey, character: character, timeStamp: timeStamp, deviceType: deviceType)
    {
    }

}

public delegate bool KeyEventCallback(KeyEvent @event);

public class HardwareKeyboard
{
    internal virtual DartMap<PhysicalKeyboardKey, LogicalKeyboardKey> _pressedKeys { get; private set; } = new DartMap<PhysicalKeyboardKey, LogicalKeyboardKey>();
    internal virtual HashSet<KeyboardLockMode> _lockModes { get; private set; } = new HashSet<KeyboardLockMode>();
    internal virtual List<Func<KeyEvent, bool>> _handlers { get; set; } = new List<Func<KeyEvent, bool>>();
    internal virtual bool _duringDispatch { get; set; } = false;
    internal virtual List<Func<KeyEvent, bool>>? _modifiedHandlers { get; set; } = default;

    public static HardwareKeyboard instance => ServicesBinding.instance.keyboard;
    public virtual HashSet<PhysicalKeyboardKey> physicalKeysPressed => _pressedKeys.Keys.toSet();
    public virtual HashSet<LogicalKeyboardKey> logicalKeysPressed => _pressedKeys.Values.toSet();
    public virtual LogicalKeyboardKey? lookUpLayout(PhysicalKeyboardKey physicalKey) => _pressedKeys.GetValueOrDefault(physicalKey);
    public virtual HashSet<KeyboardLockMode> lockModesEnabled => _lockModes;
    public virtual bool isLogicalKeyPressed(LogicalKeyboardKey key) => _pressedKeys.Values.Contains(key);
    public virtual bool isPhysicalKeyPressed(PhysicalKeyboardKey key) => _pressedKeys.ContainsKey(key);
    public virtual bool isControlPressed
    {
        get
        {
            return (isLogicalKeyPressed(LogicalKeyboardKey.controlLeft) || isLogicalKeyPressed(LogicalKeyboardKey.controlRight));
        }
    }
    public virtual bool isShiftPressed
    {
        get
        {
            return (isLogicalKeyPressed(LogicalKeyboardKey.shiftLeft) || isLogicalKeyPressed(LogicalKeyboardKey.shiftRight));
        }
    }
    public virtual bool isAltPressed
    {
        get
        {
            return (isLogicalKeyPressed(LogicalKeyboardKey.altLeft) || isLogicalKeyPressed(LogicalKeyboardKey.altRight));
        }
    }
    public virtual bool isMetaPressed
    {
        get
        {
            return (isLogicalKeyPressed(LogicalKeyboardKey.metaLeft) || isLogicalKeyPressed(LogicalKeyboardKey.metaRight));
        }
    }
    internal virtual void _logEventIfIrregular(KeyEvent @event)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                var common = "This is typically either due to https://github.com/flutter/flutter/issues/125975, " + "or a bug in the embedding's key event conciliation logic.";
                if (@event is KeyDownEvent @event__as22052)
                {
                    if (_pressedKeys.ContainsKey(@event__as22052.physicalKey))
                    {
                        Hardware_keyboardLibrary._keyboardDebug((() => $"ERROR: Received unexpected {@event__as22052.GetType()} for key that is already pressed.\n" + $"{common}\n" + $"    Event: {@event__as22052}\n" + $"    Pressed logical key: {_pressedKeys.GetValueOrDefault(@event__as22052.physicalKey)}"));
                    }
                }
                else
                {
                    if (((@event is KeyRepeatEvent) || (@event is KeyUpEvent)))
                    {
                        if (!_pressedKeys.ContainsKey(@event.physicalKey))
                        {
                            Hardware_keyboardLibrary._keyboardDebug((() => $"ERROR: Received unexpected {@event.GetType()} for key that is not pressed:\n" + $"{common}\n" + $"    Event: {@event}"));
                        }
                        else
                        {
                            if ((!object.Equals(_pressedKeys.GetValueOrDefault(@event.physicalKey), @event.logicalKey)))
                            {
                                Hardware_keyboardLibrary._keyboardDebug((() => $"ERROR: Received unexpected {@event.GetType()} for key with mismatched logical key:\n" + $"{common}\n" + $"    Event: {@event}\n" + $"    Pressed logical key: {_pressedKeys.GetValueOrDefault(@event.physicalKey)}"));
                            }
                        }
                    }
                    else
                    {
                        DartRuntimePrimitives.Assert(() => false);
                    }
                }
                return true;
            });
    }

    public virtual void addHandler(Func<KeyEvent, bool> handler)
    {
        if (_duringDispatch)
        {
            _modifiedHandlers ??= new List<Func<KeyEvent, bool>>();
            _modifiedHandlers!.Add(handler);
        }
        else
        {
            _handlers.Add(handler);
        }
    }

    public virtual void removeHandler(Func<KeyEvent, bool> handler)
    {
        if (_duringDispatch)
        {
            _modifiedHandlers ??= new List<Func<KeyEvent, bool>>();
            _modifiedHandlers!.Remove(handler);
        }
        else
        {
            _handlers.Remove(handler);
        }
    }

    public async virtual Future syncKeyboardState()
    {
        DartMap<long, long>? keyboardState = await SystemChannels.keyboard.invokeMapMethod<long, long>("getKeyboardState");
        if ((keyboardState is not null))
        {
            foreach (long key in keyboardState.Keys)
            {
                var physicalKey__25590 = new PhysicalKeyboardKey(key);
                var logicalKey__25644 = new LogicalKeyboardKey(DartRuntimePrimitives.RequireValue(keyboardState.GetValueOrDefault(key)));
                _pressedKeys[physicalKey__25590] = logicalKey__25644;
            }
        }
    }

    internal virtual bool _dispatchKeyEvent(KeyEvent @event)
    {
        DartRuntimePrimitives.Assert(() => !_duringDispatch);
        _duringDispatch = true;
        var handled = false;
        foreach (Func<KeyEvent, bool> handler in _handlers)
        {
            try
            {
                bool thisResult__26313 = handler(@event);
                handled = (handled || thisResult__26313);
            }
            catch (Exception exception)
            {
                var stack = new System.Diagnostics.StackTrace();
                InformationCollector? collector = default!;
                DartRuntimePrimitives.Assert(() =>
                    {
                        collector = (() => new List<DiagnosticsNode> { new DiagnosticsProperty<KeyEvent>("Event", @event) });
                        return true;
                    });
                FlutterError.reportError(new FlutterErrorDetails(exception: exception, stack: stack, library: "services library", context: new ErrorDescription("while processing a key handler"), informationCollector: collector));
            }
        }
        _duringDispatch = false;
        if ((_modifiedHandlers is not null))
        {
            _handlers = _modifiedHandlers!;
            _modifiedHandlers = null;
        }
        return handled;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<string> _debugPressedKeysDetails()
    {
        return new List<string>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool handleKeyEvent(KeyEvent @event)
    {
        DartRuntimePrimitives.Assert(() => Hardware_keyboardLibrary._keyboardDebug((() => $"Key event received: {@event}")));
        DartRuntimePrimitives.Assert(() => Hardware_keyboardLibrary._keyboardDebug((() => "Pressed state before processing the event:"), _debugPressedKeysDetails));
        _logEventIfIrregular(@event);
        PhysicalKeyboardKey physicalKey = @event.physicalKey;
        LogicalKeyboardKey logicalKey = @event.logicalKey;
        if (@event is KeyDownEvent @event__as27907)
        {
            _pressedKeys[physicalKey] = logicalKey;
            KeyboardLockMode? lockMode__28008 = (@event__as27907.logicalKey.keyId switch { var id when id == LogicalKeyboardKey.numLock.keyId => KeyboardLockMode.numLock, var id when id == LogicalKeyboardKey.scrollLock.keyId => KeyboardLockMode.scrollLock, var id when id == LogicalKeyboardKey.capsLock.keyId => KeyboardLockMode.capsLock, _ => (KeyboardLockMode?)null });
            if ((lockMode__28008 is not null))
            {
                if (_lockModes.Contains(DartRuntimePrimitives.RequireValue(lockMode__28008)))
                {
                    _lockModes.Remove(DartRuntimePrimitives.RequireValue(lockMode__28008));
                }
                else
                {
                    _lockModes.Add(DartRuntimePrimitives.RequireValue(lockMode__28008));
                }
            }
        }
        else
        {
            if (@event is KeyUpEvent @event__as28276)
            {
                _pressedKeys.remove(physicalKey);
            }
            else
            {
                if (@event is KeyRepeatEvent @event__as28354)
                {
                    _pressedKeys[physicalKey] = logicalKey;
                }
            }
        }
        DartRuntimePrimitives.Assert(() => Hardware_keyboardLibrary._keyboardDebug((() => "Pressed state after processing the event:"), _debugPressedKeysDetails));
        return _dispatchKeyEvent(@event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void clearState()
    {
        _pressedKeys.Clear();
        _lockModes.Clear();
        _handlers.Clear();
        DartRuntimePrimitives.Assert(() => (_modifiedHandlers is null));
    }

}

public enum KeyDataTransitMode
{
    rawKeyData,
    keyDataThenRawKeyData
}

public class KeyMessage
{
    public virtual List<KeyEvent> events { get; private set; } = default!;
    public virtual RawKeyEvent? rawEvent { get; private set; }

    public KeyMessage(List<KeyEvent> events, RawKeyEvent? rawEvent)
    {
        this.events = events;
        this.rawEvent = rawEvent;
    }

    public override string ToString()
    {
        return $"KeyMessage({events})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate bool KeyMessageHandler(KeyMessage message);

public class KeyEventManager
{
    public virtual Func<KeyMessage, bool>? keyMessageHandler { get; set; } = default;
    internal virtual HardwareKeyboard _hardwareKeyboard { get; private set; } = default!;
    internal virtual RawKeyboard _rawKeyboard { get; private set; } = default!;
    internal virtual KeyDataTransitMode? _transitMode { get; set; } = default;
    internal virtual List<KeyEvent> _keyEventsSinceLastMessage { get; private set; } = new List<KeyEvent>();
    internal virtual HashSet<PhysicalKeyboardKey> _skippedRawKeysPressed { get; private set; } = new HashSet<PhysicalKeyboardKey>();

    public KeyEventManager(HardwareKeyboard _hardwareKeyboard, RawKeyboard _rawKeyboard)
    {
        this._hardwareKeyboard = _hardwareKeyboard;
        this._rawKeyboard = _rawKeyboard;
    }

    public virtual bool handleKeyData(KeyData data)
    {
        _transitMode ??= KeyDataTransitMode.keyDataThenRawKeyData;
        switch (_transitMode!)
        {
            case var __case46602 when object.Equals(__case46602, KeyDataTransitMode.rawKeyData):
                {
                    DartRuntimePrimitives.Assert(() => false);
                    return false;
                }
            case var __case46755 when object.Equals(__case46755, KeyDataTransitMode.keyDataThenRawKeyData):
                {
                    if (((data.physical == 0L) && (data.logical == 0L)))
                    {
                        return false;
                    }
                    DartRuntimePrimitives.Assert(() => ((data.physical != 0L) && (data.logical != 0L)));
                    KeyEvent @event__47220 = _eventFromData(data);
                    if ((data.synthesized && (_keyEventsSinceLastMessage.Count == 0)))
                    {
                        _hardwareKeyboard.handleKeyEvent(@event__47220);
                        _dispatchKeyMessage(new List<KeyEvent> { @event__47220 }, null);
                    }
                    else
                    {
                        _keyEventsSinceLastMessage.Add(@event__47220);
                    }
                    return false;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _dispatchKeyMessage(List<KeyEvent> keyEvents, RawKeyEvent? rawEvent)
    {
        if ((keyMessageHandler is not null))
        {
            var message__48244 = new KeyMessage(keyEvents, rawEvent);
            try
            {
                return keyMessageHandler!(message__48244);
            }
            catch (Exception exception)
            {
                var stack = new System.Diagnostics.StackTrace();
                InformationCollector? collector = default!;
                DartRuntimePrimitives.Assert(() =>
                    {
                        collector = (() => new List<DiagnosticsNode> { new DiagnosticsProperty<KeyMessage>("KeyMessage", message__48244) });
                        return true;
                    });
                FlutterError.reportError(new FlutterErrorDetails(exception: exception, stack: stack, library: "services library", context: new ErrorDescription("while processing the key message__48244 handler"), informationCollector: collector));
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<DartMap<string, object>> handleRawKeyMessage(object message)
    {
        if ((_transitMode is null))
        {
            _transitMode = KeyDataTransitMode.rawKeyData;
            _rawKeyboard.addListener(_convertRawEventAndStore);
        }
        var rawEvent = RawKeyEvent.CreateFromMessage(DartRuntimePrimitives.ConvertMap<string, object>((System.Collections.IDictionary)message));
        var shouldDispatch = true;
        if (rawEvent is RawKeyDownEvent rawEvent__as49898)
        {
            if (!rawEvent__as49898.data.shouldDispatchEvent())
            {
                shouldDispatch = false;
                _skippedRawKeysPressed.Add(rawEvent__as49898.physicalKey);
            }
            else
            {
                _skippedRawKeysPressed.Remove(rawEvent__as49898.physicalKey);
            }
        }
        else
        {
            if (rawEvent is RawKeyUpEvent rawEvent__as50168)
            {
                if (_skippedRawKeysPressed.Contains(rawEvent__as50168.physicalKey))
                {
                    _skippedRawKeysPressed.Remove(rawEvent__as50168.physicalKey);
                    shouldDispatch = false;
                }
            }
        }
        var handled = true;
        if (shouldDispatch)
        {
            handled = _rawKeyboard.handleRawKeyEvent(rawEvent);
            foreach (KeyEvent @event in _keyEventsSinceLastMessage)
            {
                handled = (_hardwareKeyboard.handleKeyEvent(@event) || handled);
            }
            if ((object.Equals(_transitMode, KeyDataTransitMode.rawKeyData)))
            {
                DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.setEquals(_rawKeyboard.physicalKeysPressed, _hardwareKeyboard.physicalKeysPressed));
            }
            handled = (_dispatchKeyMessage(_keyEventsSinceLastMessage, rawEvent) || handled);
            _keyEventsSinceLastMessage.Clear();
        }
        return new DartMap<string, object> { ["handled"] = handled };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual KeyEventDeviceType _convertDeviceType(RawKeyEvent rawEvent)
    {
        RawKeyEventData data = rawEvent.data;
        if ((data is not RawKeyEventDataAndroid))
        {
            return Dart_uiLibrary.KeyEventDeviceType.keyboard;
        }
        switch (((RawKeyEventDataAndroid)data).eventSource)
        {
            case var __case51679 when object.Equals(__case51679, 257L):
                {
                    return Dart_uiLibrary.KeyEventDeviceType.keyboard;
                }
            case var __case51835 when object.Equals(__case51835, 513L):
                {
                    return Dart_uiLibrary.KeyEventDeviceType.directionalPad;
                }
            case var __case52000 when object.Equals(__case52000, 1025L):
                {
                    return Dart_uiLibrary.KeyEventDeviceType.gamepad;
                }
            case var __case52159 when object.Equals(__case52159, 16777232L):
                {
                    return Dart_uiLibrary.KeyEventDeviceType.joystick;
                }
            case var __case52315 when object.Equals(__case52315, 33554433L):
                {
                    return Dart_uiLibrary.KeyEventDeviceType.hdmi;
                }
        }
        return Dart_uiLibrary.KeyEventDeviceType.keyboard;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _convertRawEventAndStore(RawKeyEvent rawEvent)
    {
        PhysicalKeyboardKey physicalKey = rawEvent.physicalKey;
        LogicalKeyboardKey logicalKey = rawEvent.logicalKey;
        HashSet<PhysicalKeyboardKey> physicalKeysPressed = _hardwareKeyboard.physicalKeysPressed;
        var eventAfterwards = new List<KeyEvent>();
        KeyEvent? mainEvent = default!;
        LogicalKeyboardKey? recordedLogicalMain = _hardwareKeyboard.lookUpLayout(physicalKey);
        Duration timeStamp = ServicesBinding.instance.currentSystemFrameTimeStamp;
        string? character = ((rawEvent.character == "") ? null : rawEvent.character);
        global::Doroti.Ui.KeyEventDeviceType deviceType = _convertDeviceType(rawEvent);
        if (rawEvent is RawKeyDownEvent rawEvent__as53392)
        {
            if ((recordedLogicalMain is null))
            {
                mainEvent = new KeyDownEvent(physicalKey: physicalKey, logicalKey: logicalKey, character: character, timeStamp: timeStamp, deviceType: deviceType);
                physicalKeysPressed.Add(physicalKey);
            }
            else
            {
                DartRuntimePrimitives.Assert(() => physicalKeysPressed.Contains(physicalKey));
                mainEvent = new KeyRepeatEvent(physicalKey: physicalKey, logicalKey: recordedLogicalMain, character: character, timeStamp: timeStamp, deviceType: deviceType);
            }
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (rawEvent is RawKeyUpEvent));
            if ((recordedLogicalMain is null))
            {
                mainEvent = null;
            }
            else
            {
                mainEvent = new KeyUpEvent(logicalKey: recordedLogicalMain, physicalKey: physicalKey, timeStamp: timeStamp, deviceType: deviceType);
                physicalKeysPressed.Remove(physicalKey);
            }
        }
        foreach (PhysicalKeyboardKey key in physicalKeysPressed.difference(_rawKeyboard.physicalKeysPressed))
        {
            if ((object.Equals(key, physicalKey)))
            {
                eventAfterwards.Add(new KeyUpEvent(physicalKey: key, logicalKey: logicalKey, timeStamp: timeStamp, synthesized: true, deviceType: deviceType));
            }
            else
            {
                _keyEventsSinceLastMessage.Add(new KeyUpEvent(physicalKey: key, logicalKey: _hardwareKeyboard.lookUpLayout(key)!, timeStamp: timeStamp, synthesized: true, deviceType: deviceType));
            }
        }
        foreach (PhysicalKeyboardKey key in _rawKeyboard.physicalKeysPressed.difference(physicalKeysPressed))
        {
            _keyEventsSinceLastMessage.Add(new KeyDownEvent(physicalKey: key, logicalKey: _rawKeyboard.lookUpLayout(key)!, timeStamp: timeStamp, synthesized: true, deviceType: deviceType));
        }
        if ((mainEvent is not null))
        {
            _keyEventsSinceLastMessage.Add(mainEvent);
        }
        _keyEventsSinceLastMessage.AddRange(eventAfterwards);
    }

    public virtual void clearState()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                _transitMode = null;
                _rawKeyboard.removeListener(_convertRawEventAndStore);
                _keyEventsSinceLastMessage.Clear();
                return true;
            });
    }

    internal static KeyEvent _eventFromData(KeyData keyData)
    {
        PhysicalKeyboardKey physicalKey = (PhysicalKeyboardKey.findKeyByCode(keyData.physical) ?? new PhysicalKeyboardKey(keyData.physical));
        LogicalKeyboardKey logicalKey = (LogicalKeyboardKey.findKeyByKeyId(keyData.logical) ?? new LogicalKeyboardKey(keyData.logical));
        Duration timeStamp = keyData.timeStamp;
        switch (keyData.type)
        {
            case var __case56760 when object.Equals(__case56760, Dart_uiLibrary.KeyEventType.down):
                {
                    return new KeyDownEvent(physicalKey: physicalKey, logicalKey: logicalKey, timeStamp: timeStamp, character: keyData.character, synthesized: keyData.synthesized, deviceType: keyData.deviceType);
                }
            case var __case57061 when object.Equals(__case57061, Dart_uiLibrary.KeyEventType.up):
                {
                    DartRuntimePrimitives.Assert(() => (keyData.character is null));
                    return new KeyUpEvent(physicalKey: physicalKey, logicalKey: logicalKey, timeStamp: timeStamp, synthesized: keyData.synthesized, deviceType: keyData.deviceType);
                }
            case var __case57361 when object.Equals(__case57361, Dart_uiLibrary.KeyEventType.repeat):
                {
                    return new KeyRepeatEvent(physicalKey: physicalKey, logicalKey: logicalKey, timeStamp: timeStamp, character: keyData.character, deviceType: keyData.deviceType);
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
