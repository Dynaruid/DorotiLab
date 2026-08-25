using System.Text;
using Doroti.Ui;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>
/// Serializes the child HWND keyboard stream into Doroti key records and text
/// commits.  This is deliberately the only F7 keyboard owner: key messages,
/// dead keys, and UTF-16 surrogate pairs do not take independent side paths.
/// </summary>
internal sealed class FlutterWindowsKeyboardManager : IDisposable
{
    private const uint WmKeyDown = 0x0100;
    private const uint WmKeyUp = 0x0101;
    private const uint WmChar = 0x0102;
    private const uint WmDeadChar = 0x0103;
    private const uint WmSysKeyDown = 0x0104;
    private const uint WmSysKeyUp = 0x0105;
    private const uint WmSysChar = 0x0106;
    private const uint WmSysDeadChar = 0x0107;
    private const uint WmUniChar = 0x0109;
    private const uint UniCharNoChar = 0xffff;
    private const uint VkReturn = 0x0d;
    private const uint VkShift = 0x10;
    private const uint VkControl = 0x11;
    private const uint VkMenu = 0x12;
    private const uint VkLeftWin = 0x5b;
    private const uint VkRightWin = 0x5c;
    private const long ModifierShift = 1L << 0;
    private const long ModifierControl = 1L << 1;
    private const long ModifierAlt = 1L << 2;
    private const long ModifierMeta = 1L << 3;

    private readonly ulong _viewId;
    private readonly Action<KeyData> _keyData;
    private readonly Func<string, bool> _commitText;
    private readonly Func<bool> _performTextAction;
    private readonly object _gate = new();
    private char? _pendingHighSurrogate;
    private bool _pendingDeadKey;
    private bool _suppressNextReturnCharacter;
    private long _modifierMask;
    private bool _disposed;
    private long _keyDownCount;
    private long _keyUpCount;
    private long _repeatCount;
    private long _systemKeyCount;
    private long _characterMessageCount;
    private long _deadKeyCount;
    private long _textCodeUnitCount;
    private long _textCommitCount;
    private long _surrogatePairCount;
    private long _invalidSurrogateCount;
    private long _unhandledSystemMessageCount;
    private long _suppressedShortcutCharacterCount;
    private long _suppressedActionCharacterCount;
    private long _suppressedControlCharacterCount;

    internal FlutterWindowsKeyboardManager(
        ulong viewId,
        Action<KeyData> keyData,
        Func<string, bool> commitText,
        Func<bool> performTextAction)
    {
        if (viewId == 0) throw new ArgumentOutOfRangeException(nameof(viewId));
        _viewId = viewId;
        _keyData = keyData ?? throw new ArgumentNullException(nameof(keyData));
        _commitText = commitText ?? throw new ArgumentNullException(nameof(commitText));
        _performTextAction = performTextAction ?? throw new ArgumentNullException(nameof(performTextAction));
    }

    /// <summary>
    /// Handles only keyboard messages from the child message router.  System
    /// key messages are observed and serialized but intentionally remain
    /// unhandled so normal Windows Alt/menu behavior can continue through
    /// DefWindowProc.
    /// </summary>
    internal FlutterWindowsChildMessageResult HandleMessage(FlutterWindowsChildMessage message)
    {
        lock (_gate)
        {
            if (_disposed) return FlutterWindowsChildMessageResult.Unhandled;
            return message.Message switch
            {
                WmKeyDown => HandleKey(message, KeyEventType.down, isSystem: false),
                WmSysKeyDown => HandleKey(message, KeyEventType.down, isSystem: true),
                WmKeyUp => HandleKey(message, KeyEventType.up, isSystem: false),
                WmSysKeyUp => HandleKey(message, KeyEventType.up, isSystem: true),
                WmChar => HandleCharacter(message, isSystem: false),
                WmSysChar => HandleCharacter(message, isSystem: true),
                WmDeadChar => HandleDeadKey(message, isSystem: false),
                WmSysDeadChar => HandleDeadKey(message, isSystem: true),
                WmUniChar => HandleUnicodeCharacter(message),
                _ => FlutterWindowsChildMessageResult.Unhandled,
            };
        }
    }

    /// <summary>Clears incomplete text state when the child loses focus.</summary>
    internal void ResetForFocusLoss()
    {
        lock (_gate)
        {
            if (_pendingHighSurrogate is not null)
            {
                _pendingHighSurrogate = null;
                Interlocked.Increment(ref _invalidSurrogateCount);
            }
            _pendingDeadKey = false;
            _suppressNextReturnCharacter = false;
            _modifierMask = 0;
        }
    }

    internal FlutterWindowsKeyboardManagerSnapshot Snapshot
    {
        get
        {
            lock (_gate)
            {
                return new(
                    _viewId,
                    Interlocked.Read(ref _keyDownCount),
                    Interlocked.Read(ref _keyUpCount),
                    Interlocked.Read(ref _repeatCount),
                    Interlocked.Read(ref _systemKeyCount),
                    Interlocked.Read(ref _characterMessageCount),
                    Interlocked.Read(ref _deadKeyCount),
                    Interlocked.Read(ref _textCodeUnitCount),
                    Interlocked.Read(ref _textCommitCount),
                    Interlocked.Read(ref _surrogatePairCount),
                    Interlocked.Read(ref _invalidSurrogateCount),
                    Interlocked.Read(ref _unhandledSystemMessageCount),
                    Interlocked.Read(ref _suppressedShortcutCharacterCount),
                    Interlocked.Read(ref _suppressedActionCharacterCount),
                    Interlocked.Read(ref _suppressedControlCharacterCount),
                    _modifierMask,
                    _pendingDeadKey,
                    _pendingHighSurrogate is not null,
                    _suppressNextReturnCharacter,
                    _disposed);
            }
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed) return;
            _disposed = true;
            _pendingHighSurrogate = null;
            _pendingDeadKey = false;
            _suppressNextReturnCharacter = false;
            _modifierMask = 0;
        }
    }

    private FlutterWindowsChildMessageResult HandleKey(
        FlutterWindowsChildMessage message,
        KeyEventType requestedType,
        bool isSystem)
    {
        var virtualKey = unchecked((uint)message.WParam);
        var lParam = message.LParam.ToInt64();
        var repeat = requestedType == KeyEventType.down &&
            (((lParam >> 30) & 1) != 0 || (lParam & 0xffff) > 1);
        var type = repeat ? KeyEventType.repeat : requestedType;
        if (type == KeyEventType.up)
            Interlocked.Increment(ref _keyUpCount);
        else if (type == KeyEventType.repeat)
            Interlocked.Increment(ref _repeatCount);
        else
            Interlocked.Increment(ref _keyDownCount);
        if (isSystem) Interlocked.Increment(ref _systemKeyCount);

        UpdateModifier(virtualKey, type != KeyEventType.up);
        _keyData(new(
            _viewId,
            message.Timestamp,
            type,
            PhysicalKeyFromLParam(virtualKey, lParam),
            virtualKey,
            synthesized: false,
            modifiers: _modifierMask));

        if (!isSystem && (type is KeyEventType.down or KeyEventType.repeat) && virtualKey == VkReturn)
            _suppressNextReturnCharacter = _performTextAction();

        if (!isSystem) return FlutterWindowsChildMessageResult.HandledResult();
        Interlocked.Increment(ref _unhandledSystemMessageCount);
        return FlutterWindowsChildMessageResult.Unhandled;
    }

    private FlutterWindowsChildMessageResult HandleDeadKey(
        FlutterWindowsChildMessage message,
        bool isSystem)
    {
        _ = message;
        Interlocked.Increment(ref _deadKeyCount);
        if (!isSystem) _pendingDeadKey = true;
        if (!isSystem) return FlutterWindowsChildMessageResult.HandledResult();
        Interlocked.Increment(ref _unhandledSystemMessageCount);
        return FlutterWindowsChildMessageResult.Unhandled;
    }

    private FlutterWindowsChildMessageResult HandleCharacter(
        FlutterWindowsChildMessage message,
        bool isSystem)
    {
        Interlocked.Increment(ref _characterMessageCount);
        if (!isSystem)
        {
            var codeUnit = unchecked((char)message.WParam);
            if (_suppressNextReturnCharacter && codeUnit is '\r' or '\n')
            {
                _suppressNextReturnCharacter = false;
                Interlocked.Increment(ref _suppressedActionCharacterCount);
                return FlutterWindowsChildMessageResult.HandledResult();
            }
            _suppressNextReturnCharacter = false;

            // Win32 produces C0 WM_CHAR values for Ctrl+A..Z shortcuts. Those
            // key messages already reached the framework through KeyData and
            // must never become literal editing-state text. Alt/system text
            // remains delegated to DefWindowProc through the system path.
            if (char.IsControl(codeUnit) && (_modifierMask & (ModifierControl | ModifierAlt)) != 0)
            {
                Interlocked.Increment(ref _suppressedShortcutCharacterCount);
                return FlutterWindowsChildMessageResult.HandledResult();
            }

            if (codeUnit is '\r' or '\n')
            {
                _ = CommitText("\n");
                return FlutterWindowsChildMessageResult.HandledResult();
            }
            if (char.IsControl(codeUnit))
            {
                Interlocked.Increment(ref _suppressedControlCharacterCount);
                return FlutterWindowsChildMessageResult.HandledResult();
            }

            _ = CommitUtf16CodeUnit(codeUnit);
            return FlutterWindowsChildMessageResult.HandledResult();
        }
        Interlocked.Increment(ref _unhandledSystemMessageCount);
        return FlutterWindowsChildMessageResult.Unhandled;
    }

    private FlutterWindowsChildMessageResult HandleUnicodeCharacter(FlutterWindowsChildMessage message)
    {
        if (message.WParam == UniCharNoChar)
            return FlutterWindowsChildMessageResult.HandledResult(1);

        var scalar = unchecked((int)message.WParam);
        if (!Rune.IsValid(scalar))
        {
            Interlocked.Increment(ref _invalidSurrogateCount);
            return FlutterWindowsChildMessageResult.HandledResult();
        }

        Interlocked.Increment(ref _characterMessageCount);
        var text = new Rune(scalar).ToString();
        CommitText(text);
        return FlutterWindowsChildMessageResult.HandledResult();
    }

    private bool CommitUtf16CodeUnit(char codeUnit)
    {
        if (char.IsHighSurrogate(codeUnit))
        {
            if (_pendingHighSurrogate is { } previous)
            {
                Interlocked.Increment(ref _invalidSurrogateCount);
                CommitText("\ufffd");
                _ = previous;
            }
            _pendingHighSurrogate = codeUnit;
            return true;
        }

        if (char.IsLowSurrogate(codeUnit))
        {
            if (_pendingHighSurrogate is { } high)
            {
                _pendingHighSurrogate = null;
                Interlocked.Increment(ref _surrogatePairCount);
                return CommitText(new string([high, codeUnit]));
            }

            Interlocked.Increment(ref _invalidSurrogateCount);
            return CommitText("\ufffd");
        }

        if (_pendingHighSurrogate is not null)
        {
            _pendingHighSurrogate = null;
            Interlocked.Increment(ref _invalidSurrogateCount);
            CommitText("\ufffd");
        }

        return CommitText(codeUnit.ToString());
    }

    private bool CommitText(string text)
    {
        _pendingDeadKey = false;
        if (!_commitText(text)) return false;
        Interlocked.Add(ref _textCodeUnitCount, text.Length);
        Interlocked.Increment(ref _textCommitCount);
        return true;
    }

    private void UpdateModifier(uint virtualKey, bool down)
    {
        var flag = virtualKey switch
        {
            VkShift or 0xa0 or 0xa1 => ModifierShift,
            VkControl or 0xa2 or 0xa3 => ModifierControl,
            VkMenu or 0xa4 or 0xa5 => ModifierAlt,
            VkLeftWin or VkRightWin => ModifierMeta,
            _ => 0,
        };
        if (flag == 0) return;
        _modifierMask = down ? _modifierMask | flag : _modifierMask & ~flag;
    }

    private static long PhysicalKeyFromLParam(uint virtualKey, long lParam)
    {
        var scanCode = (lParam >> 16) & 0xff;
        if (scanCode == 0) scanCode = virtualKey;
        var extended = (lParam & (1L << 24)) != 0;
        return 0x00070000L | scanCode | (extended ? 0x01000000L : 0L);
    }
}

/// <summary>Per-view keyboard evidence for key, dead-key, and surrogate ownership.</summary>
internal sealed record FlutterWindowsKeyboardManagerSnapshot(
    ulong ViewId,
    long KeyDownCount,
    long KeyUpCount,
    long RepeatCount,
    long SystemKeyCount,
    long CharacterMessageCount,
    long DeadKeyCount,
    long TextCodeUnitCount,
    long TextCommitCount,
    long SurrogatePairCount,
    long InvalidSurrogateCount,
    long UnhandledSystemMessageCount,
    long SuppressedShortcutCharacterCount,
    long SuppressedActionCharacterCount,
    long SuppressedControlCharacterCount,
    long ModifierMask,
    bool HasPendingDeadKey,
    bool HasPendingHighSurrogate,
    bool SuppressNextReturnCharacter,
    bool IsDisposed);
