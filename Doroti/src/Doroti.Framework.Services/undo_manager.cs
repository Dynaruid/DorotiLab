// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/undo_manager.dart
using Doroti.Runtime;

namespace Doroti.Framework.Services;

public enum UndoDirection
{
    undo,
    redo,
}

public class UndoManager
{
    internal static UndoManager _instance = new UndoManager();
    internal virtual MethodChannel _channel { get; set; } = default!;
    internal virtual UndoManagerClient? _currentClient { get; set; } = default;

    public UndoManager()
    {
        _channel = SystemChannels.undoManager;
        _channel.setMethodCallHandler(_handleUndoManagerInvocation);
    }

    public static void setChannel(MethodChannel newChannel)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            _instance._channel = (
                (Func<MethodChannel>)(
                    () =>
                    {
                        var __cascade = newChannel;
                        __cascade.setMethodCallHandler(_instance._handleUndoManagerInvocation);
                        return __cascade;
                    }
                )
            )();
            return true;
        });
    }

    public static UndoManagerClient? client
    {
        get => _instance._currentClient;
        set
        {
            var client = value;
            _instance._currentClient = client;
        }
    }

    public static void setUndoState(bool canUndo = false, bool canRedo = false)
    {
        _instance._setUndoState(canUndo: canUndo, canRedo: canRedo);
    }

    internal virtual async Future<object> _handleUndoManagerInvocation(MethodCall methodCall)
    {
        string method = methodCall.method;
        var args = ((List<object>?)methodCall.arguments)!;
        if (method == "UndoManagerClient.handleUndo")
        {
            DartRuntimePrimitives.Assert(() => _currentClient is not null);
            _currentClient!.handlePlatformUndo(_toUndoDirection(((string?)args[(int)0L])!));
            return default!;
        }
        throw new MissingPluginException();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _setUndoState(bool canUndo = false, bool canRedo = false)
    {
        _ = _channel
            .invokeMethod<object?>(
                "UndoManager.setUndoState",
                new DartMap<string, bool> { ["canUndo"] = canUndo, ["canRedo"] = canRedo }
            )
            .then(
                (_) => { },
                onError: (error, stack) =>
                {
                    FlutterError.reportError(
                        new FlutterErrorDetails(
                            exception: error,
                            stack: stack,
                            library: "services library",
                            context: new ErrorDescription(
                                "while sending the UndoManager.setUndoState event"
                            )
                        )
                    );
                }
            );
    }

    internal virtual UndoDirection _toUndoDirection(string direction)
    {
        return direction switch
        {
            var __case4108 when Equals(__case4108, "undo") => UndoDirection.undo,
            var __case4144 when Equals(__case4144, "redo") => UndoDirection.redo,
            _ => throw new FlutterError(
                new List<DiagnosticsNode>
                {
                    new ErrorSummary($"Unknown undo direction: {direction}"),
                }
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public interface UndoManagerClient
{
    public void handlePlatformUndo(UndoDirection direction);
    public void undo();
    public void redo();
    public bool canUndo { get; }
    public bool canRedo { get; }
}
