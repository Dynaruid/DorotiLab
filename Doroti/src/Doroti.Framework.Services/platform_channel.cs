#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/platform_channel.dart
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

public static partial class Platform_channelLibrary
{
    public static bool shouldProfilePlatformChannels => (Platform_channelLibrary.kProfilePlatformChannels || ((!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode && global::Doroti.Framework.Services.DebugLibrary.debugProfilePlatformChannels)));
}

public static partial class Platform_channelLibrary
{
    public static bool kProfilePlatformChannels = false;
}

public static partial class Platform_channelLibrary
{
    internal static bool _profilePlatformChannelsIsRunning = false;
}

public static partial class Platform_channelLibrary
{
    internal static Duration _profilePlatformChannelsRate = new Duration(seconds: 1L);
}

public static partial class Platform_channelLibrary
{
    internal static Expando<BinaryMessenger> _profiledBinaryMessengers = new Expando<BinaryMessenger>();
}

internal class _ProfiledBinaryMessenger : BinaryMessenger
{
    public virtual BinaryMessenger proxy { get; private set; } = default!;
    public virtual string channelTypeName { get; private set; } = default!;
    public virtual string codecTypeName { get; private set; } = default!;

    internal _ProfiledBinaryMessenger(BinaryMessenger proxy, string channelTypeName, string codecTypeName)
    {
        this.proxy = proxy;
        this.channelTypeName = channelTypeName;
        this.codecTypeName = codecTypeName;
    }

    public virtual Future handlePlatformMessage(string channel, ByteData? data, Action<ByteData?>? callback)
    {
        return proxy.handlePlatformMessage(channel, data, callback);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<ByteData?>? sendWithPostfix(string channel, string postfix, ByteData? message)
    {
        Platform_channelLibrary._debugRecordUpStream(channelTypeName, $"{channel}{postfix}", codecTypeName, message);
        var timelineTask = ((Func<TimelineTask>)(() =>
{
    var __cascade = new TimelineTask();
    __cascade.start($"Platform Channel send {channel}{postfix}");
    return __cascade;
}))();
        ByteData? result = default!;
        try
        {
            result = await proxy.send(channel, message);
        }
        finally
        {
            timelineTask.finish();
        }
        Platform_channelLibrary._debugRecordDownStream(channelTypeName, $"{channel}{postfix}", codecTypeName, result);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<ByteData?>? send(string channel, ByteData? message) => sendWithPostfix(channel, "", message);
    public virtual void setMessageHandler(string channel, Func<ByteData?, Future<ByteData?>?>? handler)
    {
        proxy.setMessageHandler(channel, handler);
    }

}

internal class _PlatformChannelStats
{
    public virtual string channel { get; private set; } = default!;
    public virtual string codec { get; private set; } = default!;
    public virtual string type { get; private set; } = default!;
    internal virtual long _upCount { get; set; } = 0L;
    internal virtual long _upBytes { get; set; } = 0L;
    internal virtual long _downCount { get; set; } = 0L;
    internal virtual long _downBytes { get; set; } = 0L;

    internal _PlatformChannelStats(string channel, string codec, string type)
    {
        this.channel = channel;
        this.codec = codec;
        this.type = type;
    }

    public virtual long upBytes => _upBytes;
    public virtual void addUpStream(long bytes)
    {
        _upCount += 1L;
        _upBytes += bytes;
    }

    public virtual long downBytes => _downBytes;
    public virtual void addDownStream(long bytes)
    {
        _downCount += 1L;
        _downBytes += bytes;
    }

    public virtual double averageUpPayload => (_upBytes / _upCount);
    public virtual double averageDownPayload => (_downBytes / _downCount);
}

public static partial class Platform_channelLibrary
{
    internal static DartMap<string, _PlatformChannelStats> _profilePlatformChannelsStats = new DartMap<string, _PlatformChannelStats>();
}

public static partial class Platform_channelLibrary
{
    internal static async Future _debugLaunchProfilePlatformChannels()
    {
        if (!Platform_channelLibrary._profilePlatformChannelsIsRunning)
        {
            Platform_channelLibrary._profilePlatformChannelsIsRunning = true;
            await new Future<object>(Platform_channelLibrary._profilePlatformChannelsRate);
            Platform_channelLibrary._profilePlatformChannelsIsRunning = false;
            var log = new StringBuffer();
            log.writeln("Platform Channel Stats:");
            List<_PlatformChannelStats> allStats = Platform_channelLibrary._profilePlatformChannelsStats.Values.ToList();
            allStats.sort(((x, y) => (((y.upBytes + y.downBytes)) - ((x.upBytes + x.downBytes)))));
            foreach (var stats in allStats)
            {
                log.writeln($"  (name:\"{stats.channel}\" type:\"{stats.type}\" codec:\"{stats.codec}\" upBytes:{stats.upBytes} upBytes_avg:{stats.averageUpPayload.toStringAsFixed(1L)} downBytes:{stats.downBytes} downBytes_avg:{stats.averageDownPayload.toStringAsFixed(1L)})");
            }
            global::Doroti.Framework.Foundation.PrintLibrary.debugPrint(log.ToString());
            Platform_channelLibrary._profilePlatformChannelsStats.Clear();
        }
    }
}

public static partial class Platform_channelLibrary
{
    internal static void _debugRecordUpStream(string channelTypeName, string name, string codecTypeName, ByteData? bytes)
    {
        _PlatformChannelStats stats = Platform_channelLibrary._profilePlatformChannelsStats[name] ??= new _PlatformChannelStats(name, codecTypeName, channelTypeName);
        stats.addUpStream((bytes?.lengthInBytes ?? 0L));
        _ = Platform_channelLibrary._debugLaunchProfilePlatformChannels();
    }
}

public static partial class Platform_channelLibrary
{
    internal static void _debugRecordDownStream(string channelTypeName, string name, string codecTypeName, ByteData? bytes)
    {
        _PlatformChannelStats stats = Platform_channelLibrary._profilePlatformChannelsStats[name] ??= new _PlatformChannelStats(name, codecTypeName, channelTypeName);
        stats.addDownStream((bytes?.lengthInBytes ?? 0L));
        _ = Platform_channelLibrary._debugLaunchProfilePlatformChannels();
    }
}

public static partial class Platform_channelLibrary
{
    internal static BinaryMessenger _findBinaryMessenger()
    {
        return ((!global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb && (ServicesBinding.rootIsolateToken is null)) ? BackgroundIsolateBinaryMessenger.instance : ServicesBinding.instance.defaultBinaryMessenger);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class BasicMessageChannel<T>
{
    public virtual string name { get; private set; } = default!;
    public virtual MessageCodec<T> codec { get; private set; } = default!;
    internal virtual BinaryMessenger? _binaryMessenger { get; private set; }

    public BasicMessageChannel(string name, MessageCodec<T> codec, BinaryMessenger? binaryMessenger = null)
    {
        this.name = name;
        this.codec = codec;
        this._binaryMessenger = binaryMessenger;
    }

    public virtual BinaryMessenger binaryMessenger
    {
        get
        {
            BinaryMessenger result = (_binaryMessenger ?? Platform_channelLibrary._findBinaryMessenger());
            return (Platform_channelLibrary.shouldProfilePlatformChannels ? Platform_channelLibrary._profiledBinaryMessengers[this] ??= new _ProfiledBinaryMessenger(result, this.GetType().ToString(), DartRuntimePrimitives.RuntimeTypeName(codec)) : result);
        }
    }
    public async virtual Future<T?> send(T message)
    {
        return codec.decodeMessage(await binaryMessenger.send(name, codec.encodeMessage(message)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void setMessageHandler(Func<T, Future>? handler)
    {
        if ((handler is null))
        {
            binaryMessenger.setMessageHandler(name, null);
        }
        else
        {
            binaryMessenger.setMessageHandler(name, (async (message) =>
            {
                return codec.encodeMessage(((T)await DartAsyncRuntime.AwaitObject(handler(codec.decodeMessage(message))))!);
            }));
        }
    }

}

public class MethodChannel
{
    public virtual string name { get; private set; } = default!;
    public virtual MethodCodec codec { get; private set; } = default!;
    internal virtual BinaryMessenger? _binaryMessenger { get; private set; }

    public MethodChannel(string name, MethodCodec codec = default!, BinaryMessenger? binaryMessenger = null)
    {
        this.name = name;
        this.codec = codec ?? new StandardMethodCodec(new StandardMessageCodec());
        this._binaryMessenger = binaryMessenger;
    }

    public virtual BinaryMessenger binaryMessenger
    {
        get
        {
            BinaryMessenger result = (_binaryMessenger ?? Platform_channelLibrary._findBinaryMessenger());
            return (Platform_channelLibrary.shouldProfilePlatformChannels ? Platform_channelLibrary._profiledBinaryMessengers[this] ??= new _ProfiledBinaryMessenger(result, this.GetType().ToString(), DartRuntimePrimitives.RuntimeTypeName(codec)) : result);
        }
    }
    internal async virtual Future<T?> _invokeMethod<T>(string method, bool missingOk, object arguments = default!)
    {
        ByteData input = codec.encodeMethodCall(new MethodCall(method, arguments));
        ByteData? result = (Platform_channelLibrary.shouldProfilePlatformChannels ? await (((_ProfiledBinaryMessenger?)binaryMessenger)!).sendWithPostfix(name, $"#{method}", input) : await binaryMessenger.send(name, input));
        if ((result is null))
        {
            if (missingOk)
            {
                return default;
            }
            throw new MissingPluginException($"No implementation found for method {method} on channel {name}");
        }
        return ((T?)codec.decodeEnvelope(result))!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<T?> invokeMethod<T>(string method, object arguments = default!)
    {
        return _invokeMethod<T>(method, missingOk: false, arguments: arguments);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<List<T>?> invokeListMethod<T>(string method, object arguments = default!)
    {
        List<object>? result = await invokeMethod<List<object>>(method, arguments);
        return result?.cast<T>().ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<DartMap<K, V>?> invokeMapMethod<K, V>(string method, object arguments = default!)
    {
        DartMap<object, object>? result = await invokeMethod<DartMap<object, object>>(method, arguments);
        return result?.cast<K, V>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void setMethodCallHandler(Func<MethodCall, Future> handler)
    {
        DartRuntimePrimitives.Assert(() => ((_binaryMessenger is not null) || (BindingBase.debugBindingType() is not null)));
        binaryMessenger.setMessageHandler(name, ((handler is null) ? null : ((message) => _handleAsMethodCall(message, handler))));
    }

    internal async virtual Future<ByteData?> _handleAsMethodCall(ByteData? message, Func<MethodCall, Future> handler)
    {
        MethodCall call = codec.decodeMethodCall(message);
        try
        {
            return codec.encodeSuccessEnvelope(((object)await DartAsyncRuntime.AwaitObject(handler(call)))!);
        }
        catch (PlatformException e)
        {
            return codec.encodeErrorEnvelope(code: e.code, message: e.message, details: e.details);
        }
        catch (MissingPluginException)
        {
            return null;
        }
        catch (Exception error)
        {
            return codec.encodeErrorEnvelope(code: "error", message: error.ToString());
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class OptionalMethodChannel : MethodChannel
{
    public OptionalMethodChannel(string name, MethodCodec codec = default!, BinaryMessenger? binaryMessenger = null) : base(name: name, codec: codec, binaryMessenger: binaryMessenger)
    {
    }

    public async override Future<T?> invokeMethod<T>(string method, object arguments = default!) where T : default
    {
        return await base._invokeMethod<T>(method, missingOk: true, arguments: arguments);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class EventChannel
{
    public virtual string name { get; private set; } = default!;
    public virtual MethodCodec codec { get; private set; } = default!;
    internal virtual BinaryMessenger? _binaryMessenger { get; private set; }

    public EventChannel(string name, MethodCodec codec = default!, BinaryMessenger? binaryMessenger = null)
    {
        this.name = name;
        this.codec = codec;
        this._binaryMessenger = binaryMessenger;
    }

    public virtual BinaryMessenger binaryMessenger => (_binaryMessenger ?? Platform_channelLibrary._findBinaryMessenger());
    public virtual Stream<object> receiveBroadcastStream(object arguments = default!)
    {
        var methodChannel = new MethodChannel(name, codec);
        StreamController<object> controller = default!;
        controller = new StreamController<object>(onListen: (async () =>
        {
            binaryMessenger.setMessageHandler(name, (async (reply) =>
            {
                if ((reply is null))
                {
                    await controller.close();
                }
                else
                {
                    try
                    {
                        controller.Add(codec.decodeEnvelope(reply));
                    }
                    catch (PlatformException e)
                    {
                        controller.addError(e);
                    }
                }
                return null;
            }));
            try
            {
                await methodChannel.invokeMethod<object?>("listen", arguments);
            }
            catch (Exception exception)
            {
                var stack = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new FlutterErrorDetails(exception: exception, stack: stack, library: "services library", context: new ErrorDescription($"while activating platform stream on channel {name}")));
            }
        }), onCancel: (async () =>
        {
            binaryMessenger.setMessageHandler(name, null);
            try
            {
                await methodChannel.invokeMethod<object?>("cancel", arguments);
            }
            catch (Exception exception)
            {
                var stack = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new FlutterErrorDetails(exception: exception, stack: stack, library: "services library", context: new ErrorDescription($"while de-activating platform stream on channel {name}")));
            }
        }));
        return controller.stream;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
