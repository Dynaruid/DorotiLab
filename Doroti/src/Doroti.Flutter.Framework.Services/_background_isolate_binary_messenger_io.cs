#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/_background_isolate_binary_messenger_io.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Services;

internal class BackgroundIsolateBinaryMessenger : BinaryMessenger
{
    internal virtual ReceivePort _receivePort { get; private set; } = new ReceivePort();
    internal virtual DartMap<long, Completer<ByteData?>> _completers { get; private set; } = new DartMap<long, Completer<ByteData?>>();
    internal virtual long _messageCount { get; set; } = 0L;
    internal static BinaryMessenger? _instance = default;

    public BackgroundIsolateBinaryMessenger()
    {
    }

    public static BinaryMessenger instance
    {
        get
        {
            if ((_instance is null))
            {
                throw new InvalidOperationException("The BackgroundIsolateBinaryMessenger.instance value is invalid " + "until BackgroundIsolateBinaryMessenger.ensureInitialized is " + "executed.");
            }
            return _instance!;
        }
    }
    public static void ensureInitialized(RootIsolateToken token)
    {
        if ((_instance is null))
        {
            Dart_uiLibrary.PlatformDispatcher.instance.registerBackgroundIsolate(token);
            var portBinaryMessenger__1643 = new BackgroundIsolateBinaryMessenger();
            _instance = portBinaryMessenger__1643;
            portBinaryMessenger__1643._receivePort.listen(((message) =>
            {
                try
                {
                    var args__1838 = ((List<object>?)message)!;
                    var identifier__1887 = ((long)args__1838[(int)(0L)]);
                    var bytes__1932 = ((Uint8List?)args__1838[(int)(1L)])!;
                    var byteData__1978 = new ByteData(bytes__1932);
                    portBinaryMessenger__1643._completers.remove(identifier__1887)!.complete(byteData__1978);
                }
                catch (Exception exception)
                {
                    var stack = new System.Diagnostics.StackTrace();
                    FlutterError.reportError(new FlutterErrorDetails(exception: exception, stack: stack, library: "services library", context: new ErrorDescription("during a platform message response callback")));
                }
            }));
        }
    }

    public virtual Future handlePlatformMessage(string channel, ByteData? data, Action<ByteData?>? callback)
    {
        throw new NotImplementedException("handlePlatformMessage is deprecated.");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<ByteData?>? send(string channel, ByteData? message)
    {
        var completer = new Completer<ByteData?>();
        _messageCount += 1L;
        long messageIdentifier = _messageCount;
        _completers[messageIdentifier] = completer;
        Dart_uiLibrary.PlatformDispatcher.instance.sendPortPlatformMessage(channel, message, messageIdentifier, _receivePort.sendPort);
        return completer.future;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void setMessageHandler(string channel, Func<ByteData?, Future<ByteData?>?>? handler)
    {
        throw new NotSupportedException("Background isolates do not support setMessageHandler(). Messages from the host platform always go to the root isolate.");
    }

}

