#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/binding.dart
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

public abstract class ServicesBinding : SchedulerBinding
{
    internal static ServicesBinding? _instance = default;
    internal virtual HardwareKeyboard _keyboard { get; private set; } = default!;
    internal virtual KeyEventManager _keyEventManager { get; private set; } = default!;
    internal virtual BinaryMessenger _defaultBinaryMessenger { get; private set; } = default!;
    public virtual ValueNotifier<long?> accessibilityFocus { get; private set; } = new ValueNotifier<long?>(null);
    internal virtual RestorationManager _restorationManager { get; set; } = default!;
    internal virtual Func<bool, Future>? _systemUiChangeCallback { get; set; } = default;
    internal virtual SystemContextMenuClient? _systemContextMenuClient { get; set; } = default;

    protected ServicesBinding(PlatformDispatcher? platformDispatcher = null)
        : base(platformDispatcher)
    {
    }

    protected override void initInstances()
    {
        base.initInstances();
        _instance = this;
        _defaultBinaryMessenger = createBinaryMessenger();
        _restorationManager = createRestorationManager();
        _initKeyboard();
        initLicenses();
        SystemChannels.system.setMessageHandler(((message) => handleSystemMessage(((object?)message)!)));
        SystemChannels.accessibility.setMessageHandler(((message) => _handleAccessibilityMessage(((object?)message)!)));
        SystemChannels.lifecycle.setMessageHandler(_handleLifecycleMessage);
        SystemChannels.platform.setMethodCallHandler(_handlePlatformMessage);
        platformDispatcher.onViewFocusChange = handleViewFocusChanged;
        TextInput.ensureInitialized();
        readInitialLifecycleStateFromNativeWindow();
        _ = initializationComplete();
    }

    public static new ServicesBinding instance => BindingBase.checkInstance(_instance);
    public virtual HardwareKeyboard keyboard => _keyboard;
    public virtual KeyEventManager keyEventManager => _keyEventManager;
    internal virtual void _initKeyboard()
    {
        _keyboard = new HardwareKeyboard();
        _keyEventManager = new KeyEventManager(_keyboard, RawKeyboard.instance);
        _ = _keyboard.syncKeyboardState().then(((_) =>
        {
            platformDispatcher.onKeyData = _keyEventManager.handleKeyData;
            SystemChannels.keyEvent.setMessageHandler(_keyEventManager.handleRawKeyMessage);
        }));
    }

    public virtual BinaryMessenger defaultBinaryMessenger => _defaultBinaryMessenger;
    public static RootIsolateToken? rootIsolateToken => Dart_uiLibrary.RootIsolateToken.instance;
    public virtual ChannelBuffers channelBuffers => Dart_uiLibrary.channelBuffers;
    public virtual BinaryMessenger createBinaryMessenger()
    {
        return new _DefaultBinaryMessenger();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleMemoryPressure()
    {
        global::Doroti.Generated.Framework.Services.Asset_bundleLibrary.rootBundle.clear();
    }

    public async virtual Future handleSystemMessage(object systemMessage)
    {
        var message = DartRuntimePrimitives.ConvertMap<string, object>((System.Collections.IDictionary)systemMessage);
        var type = ((string?)message.GetValueOrDefault("type"))!;
        switch (type)
        {
            case var __case6961 when object.Equals(__case6961, "memoryPressure"):
                {
                    handleMemoryPressure();
                    break;
                }
        }
        return;
    }

    public virtual void initLicenses()
    {
        LicenseRegistry.addLicense(_addLicenses);
    }

    internal virtual Stream<LicenseEntry> _addLicenses()
    {
        StreamController<LicenseEntry> controller = default!;
        controller = new StreamController<LicenseEntry>(onListen: (async () =>
        {
            string rawLicenses = default!;
            if (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb)
            {
                rawLicenses = await global::Doroti.Generated.Framework.Services.Asset_bundleLibrary.rootBundle.loadString("NOTICES", cache: false);
            }
            else
            {
                ByteData licenseBytes__8056 = await global::Doroti.Generated.Framework.Services.Asset_bundleLibrary.rootBundle.load("NOTICES.Z");
                List<long> unzippedBytes__8133 = await global::Doroti.Generated.Framework.Foundation.IsolatesLibrary.compute<List<long>, List<long>>(Dart_ioLibrary.gzip.decode, licenseBytes__8056.buffer.asUint8List(), debugLabel: "decompressLicenses");
                rawLicenses = await global::Doroti.Generated.Framework.Foundation.IsolatesLibrary.compute<List<long>, string>(global::Doroti.Flutter.Runtime.Dart_convertLibrary.utf8.decode, unzippedBytes__8133, debugLabel: "utf8DecodeLicenses");
            }
            List<LicenseEntry> licenses = await global::Doroti.Generated.Framework.Foundation.IsolatesLibrary.compute<string, List<LicenseEntry>>(_parseLicenses, rawLicenses, debugLabel: "parseLicenses");
            licenses.forEach(controller.add);
            await controller.close();
        }));
        return controller.stream;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static List<LicenseEntry> _parseLicenses(string rawLicenses)
    {
        var licenseSeparator = $"\n{DartCoreExtensions.repeat("-", 80L)}\n";
        return new List<LicenseEntry>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    protected override void initServiceExtensions()
    {
        base.initServiceExtensions();
        DartRuntimePrimitives.Assert(() =>
            {
                registerStringServiceExtension(name: ServicesServiceExtensions.evict.ToString(), getter: (() => ""), setter: ((value) =>
                {
                    evict(value);
                }));
                return true;
            });
        if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            registerBoolServiceExtension(name: ServicesServiceExtensions.profilePlatformChannels.ToString(), getter: (() => global::Doroti.Generated.Framework.Services.DebugLibrary.debugProfilePlatformChannels), setter: ((value) =>
            {
                global::Doroti.Generated.Framework.Services.DebugLibrary.debugProfilePlatformChannels = value;
            }));
        }
    }

    public virtual void evict(string asset)
    {
        global::Doroti.Generated.Framework.Services.Asset_bundleLibrary.rootBundle.evict(asset);
    }

    public virtual void readInitialLifecycleStateFromNativeWindow()
    {
        if (((lifecycleState is not null) || (platformDispatcher.initialLifecycleState.Length == 0)))
        {
            return;
        }
        _ = _handleLifecycleMessage(platformDispatcher.initialLifecycleState);
    }

    internal async virtual Future<string?> _handleLifecycleMessage(string? message)
    {
        global::Doroti.Flutter.Ui.AppLifecycleState? state = _parseAppLifecycleMessage(message!);
        List<global::Doroti.Flutter.Ui.AppLifecycleState> generated = _generateStateTransitions(lifecycleState, DartRuntimePrimitives.RequireValue(state));
        foreach (var stateChange in generated)
        {
            handleAppLifecycleStateChanged(stateChange);
            SystemChrome.handleAppLifecycleStateChanged(stateChange);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<AppLifecycleState> _generateStateTransitions(AppLifecycleState? previousState, AppLifecycleState state)
    {
        if ((object.Equals(previousState, state)))
        {
            return new List<global::Doroti.Flutter.Ui.AppLifecycleState>();
        }
        var stateChanges = new List<global::Doroti.Flutter.Ui.AppLifecycleState>();
        if ((previousState is null))
        {
            stateChanges.Add(state);
        }
        else
        {
            long previousStateIndex__12032 = System.Enum.GetValues<AppLifecycleState>().ToList().IndexOf(DartRuntimePrimitives.RequireValue(previousState));
            long stateIndex__12118 = System.Enum.GetValues<AppLifecycleState>().ToList().IndexOf(state);
            DartRuntimePrimitives.Assert(() => (previousStateIndex__12032 != -1L));
            DartRuntimePrimitives.Assert(() => (stateIndex__12118 != -1L));
            if ((object.Equals(state, AppLifecycleState.detached)))
            {
                for (long i = (previousStateIndex__12032 + 1L); (i < System.Enum.GetValues<AppLifecycleState>().ToList().Count); ++i)
                {
                    stateChanges.Add(System.Enum.GetValues<AppLifecycleState>().ToList()[(int)(i)]);
                }
                stateChanges.Add(AppLifecycleState.detached);
            }
            else
            {
                if ((previousStateIndex__12032 > stateIndex__12118))
                {
                    for (var i = stateIndex__12118; (i < previousStateIndex__12032); ++i)
                    {
                        stateChanges.Insert(checked((int)0L), System.Enum.GetValues<AppLifecycleState>().ToList()[(int)(i)]);
                    }
                }
                else
                {
                    for (long i = (previousStateIndex__12032 + 1L); (i <= stateIndex__12118); ++i)
                    {
                        stateChanges.Add(System.Enum.GetValues<AppLifecycleState>().ToList()[(int)(i)]);
                    }
                }
            }
        }
        DartRuntimePrimitives.Assert(() =>
            {
                var starting = previousState;
                foreach (var ending in stateChanges)
                {
                    if (!_debugVerifyLifecycleChange(starting, ending))
                    {
                        return false;
                    }
                    starting = ending;
                }
                return true;
            });
        return stateChanges;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static bool _debugVerifyLifecycleChange(AppLifecycleState? starting, AppLifecycleState ending)
    {
        if ((starting is null))
        {
            return true;
        }
        if ((object.Equals(starting, ending)))
        {
            return false;
        }
        return (starting switch { var __case13833 when object.Equals(__case13833, AppLifecycleState.resumed) => (object.Equals(ending, AppLifecycleState.inactive)), var __case13906 when object.Equals(__case13906, AppLifecycleState.detached) => ((object.Equals(ending, AppLifecycleState.resumed)) || (object.Equals(ending, AppLifecycleState.paused))), var __case14025 when object.Equals(__case14025, AppLifecycleState.inactive) => ((object.Equals(ending, AppLifecycleState.resumed)) || (object.Equals(ending, AppLifecycleState.hidden))), var __case14144 when object.Equals(__case14144, AppLifecycleState.hidden) => ((object.Equals(ending, AppLifecycleState.paused)) || (object.Equals(ending, AppLifecycleState.inactive))), var __case14262 when object.Equals(__case14262, AppLifecycleState.paused) => ((object.Equals(ending, AppLifecycleState.hidden)) || (object.Equals(ending, AppLifecycleState.detached))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal async virtual Future _handleAccessibilityMessage(object accessibilityMessage)
    {
        DartMap<string, object> message = (DartRuntimePrimitives.ConvertMap<object?, object?>((System.Collections.IDictionary)accessibilityMessage)).cast<string, object>();
        var type = ((string?)message.GetValueOrDefault("type"))!;
        switch (type)
        {
            case var __case14829 when object.Equals(__case14829, "didGainFocus"):
                {
                    accessibilityFocus.value = ((long)message.GetValueOrDefault("nodeId"));
                    break;
                }
        }
        return;
    }

    public virtual void handleViewFocusChanged(ViewFocusEvent @event)
    {
    }

    internal async virtual Future<object> _handlePlatformMessage(MethodCall methodCall)
    {
        string method = methodCall.method;
        switch (method)
        {
            case var __case15705 when object.Equals(__case15705, "ContextMenu.onDismissSystemContextMenu"):
                {
                    if ((_systemContextMenuClient is null))
                    {
                        DartRuntimePrimitives.Assert(() => false);
                        return default!;
                    }
                    _systemContextMenuClient!.handleSystemHide();
                    _systemContextMenuClient = null;
                    break;
                }
            case var __case16083 when object.Equals(__case16083, "ContextMenu.onPerformCustomAction"):
                {
                    if ((_systemContextMenuClient is null))
                    {
                        DartRuntimePrimitives.Assert(() => false);
                        return default!;
                    }
                    var args__16462 = ((List<object>?)methodCall.arguments)!;
                    var callbackId__16522 = ((string?)args__16462[(int)(1L)])!;
                    _systemContextMenuClient!.handleCustomContextMenuAction(callbackId__16522);
                    break;
                }
            case var __case16642 when object.Equals(__case16642, "SystemChrome.systemUIChange"):
                {
                    var args__16687 = ((List<object>?)methodCall.arguments)!;
                    await _systemUiChangeCallback?.Invoke(((bool)args__16687[(int)(0L)]));
                    break;
                }
            case var __case16806 when object.Equals(__case16806, "System.requestAppExit"):
                {
                    return new DartMap<string, object> { ["response"] = (await handleRequestAppExit()).ToString() };
                }
            default:
                {
                    throw new AssertionError($"Method \"{method}\" not handled.");
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static AppLifecycleState? _parseAppLifecycleMessage(string message)
    {
        return (message switch { var __case17111 when object.Equals(__case17111, "AppLifecycleState.resumed") => AppLifecycleState.resumed, var __case17175 when object.Equals(__case17175, "AppLifecycleState.inactive") => AppLifecycleState.inactive, var __case17241 when object.Equals(__case17241, "AppLifecycleState.hidden") => AppLifecycleState.hidden, var __case17303 when object.Equals(__case17303, "AppLifecycleState.paused") => AppLifecycleState.paused, var __case17365 when object.Equals(__case17365, "AppLifecycleState.detached") => AppLifecycleState.detached, _ => null });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<AppExitResponse> handleRequestAppExit()
    {
        return Dart_uiLibrary.AppExitResponse.exit;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<AppExitResponse> exitApplication(AppExitType exitType, long exitCode = 0)
    {
        DartMap<string, object?>? result = await SystemChannels.platform.invokeMethod<DartMap<string, object?>>("System.exitApplication", new DartMap<string, object?> { ["type"] = exitType.ToString(), ["exitCode"] = exitCode });
        if ((result is null))
        {
            return Dart_uiLibrary.AppExitResponse.cancel;
        }
        switch (result.GetValueOrDefault("response"))
        {
            case var __case21059 when object.Equals(__case21059, "cancel"):
                {
                    return Dart_uiLibrary.AppExitResponse.cancel;
                }
            case var __case21122 when object.Equals(__case21122, "exit"):
            default:
                {
                    return Dart_uiLibrary.AppExitResponse.exit;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RestorationManager restorationManager => _restorationManager;
    public virtual RestorationManager createRestorationManager()
    {
        return new RestorationManager();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void setSystemUiChangeCallback(Func<bool, Future>? callback)
    {
        _ = _systemUiChangeCallback = callback;
    }

    public async virtual Future initializationComplete()
    {
        await SystemChannels.platform.invokeMethod<object>("System.initializationComplete");
    }

    public static SystemContextMenuClient? systemContextMenuClient
    {
        set
        {
            var client = value;
            instance._systemContextMenuClient = client;
        }
    }
}

public delegate Future SystemUiChangeCallback(bool systemOverlaysAreVisible);

internal class _DefaultBinaryMessenger : BinaryMessenger
{
    internal _DefaultBinaryMessenger()
    {
    }

    public async virtual Future handlePlatformMessage(string channel, ByteData? data, Action<ByteData?>? callback)
    {
        Dart_uiLibrary.channelBuffers.push(channel, data, ((data) => callback?.Invoke(data)));
    }

    public virtual Future<ByteData?>? send(string channel, ByteData? message)
    {
        var completer = new Completer<ByteData?>();
        Dart_uiLibrary.PlatformDispatcher.instance.sendPlatformMessage(channel, message, ((reply) =>
        {
            try
            {
                completer.complete(reply);
            }
            catch (Exception exception)
            {
                var stack = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new FlutterErrorDetails(exception: exception, stack: stack, library: "services library", context: new ErrorDescription("during a platform message response callback")));
            }
        }));
        return completer.future;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void setMessageHandler(string channel, Func<ByteData?, Future<ByteData?>?>? handler)
    {
        if ((handler is null))
        {
            Dart_uiLibrary.channelBuffers.clearListener(channel);
        }
        else
        {
            Dart_uiLibrary.channelBuffers.setListener(channel, (async (data, callback) =>
            {
                ByteData? response = default!;
                try
                {
                    response = await handler(data);
                }
                catch (Exception exception)
                {
                    var stack = new System.Diagnostics.StackTrace();
                    FlutterError.reportError(new FlutterErrorDetails(exception: exception, stack: stack, library: "services library", context: new ErrorDescription("during a platform message callback")));
                }
            }));
        }
    }

}

public abstract class SystemContextMenuClient
{
    public abstract void handleSystemHide();
    public abstract void handleCustomContextMenuAction(string actionId);
}
