// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/app_lifecycle_listener.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public delegate Future<AppExitResponse> AppExitRequestCallback();

public class AppLifecycleListener : WidgetsBindingObserver, global::Doroti.Framework.Foundation.Diagnosticable
{
    internal virtual AppLifecycleState? _lifecycleState { get; set; } = default;
    public virtual WidgetsBinding binding { get; private set; } = default!;
    public virtual global::System.Action<AppLifecycleState>? onStateChange { get; private set; }
    public virtual global::System.Action? onInactive { get; private set; }
    public virtual global::System.Action? onResume { get; private set; }
    public virtual global::System.Action? onHide { get; private set; }
    public virtual global::System.Action? onShow { get; private set; }
    public virtual global::System.Action? onPause { get; private set; }
    public virtual global::System.Action? onRestart { get; private set; }
    public virtual global::System.Func<Future<AppExitResponse>>? onExitRequested { get; private set; }
    public virtual global::System.Action? onDetach { get; private set; }
    internal virtual bool _debugDisposed { get; set; } = false;

    public AppLifecycleListener(WidgetsBinding? binding = null, global::System.Action? onResume = null, global::System.Action? onInactive = null, global::System.Action? onHide = null, global::System.Action? onShow = null, global::System.Action? onPause = null, global::System.Action? onRestart = null, global::System.Action? onDetach = null, global::System.Func<Future<AppExitResponse>>? onExitRequested = null, global::System.Action<AppLifecycleState>? onStateChange = null)
    {
        this.onResume = onResume;
        this.onInactive = onInactive;
        this.onHide = onHide;
        this.onShow = onShow;
        this.onPause = onPause;
        this.onRestart = onRestart;
        this.onDetach = onDetach;
        this.onExitRequested = onExitRequested;
        this.onStateChange = onStateChange;
        this.binding = binding ?? WidgetsBinding.instance;
        _lifecycleState = (binding ?? WidgetsBinding.instance).lifecycleState;
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        binding.removeObserver(this);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugDisposed = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    internal virtual bool _debugAssertNotDisposed()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (_debugDisposed)
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create($"A {GetType()} was used after being disposed.\n" + $"Once you have called dispose() on a {GetType()}, it " + "can no longer be used."));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<AppExitResponse> didRequestAppExit()
    {
        DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
        if (onExitRequested is null)
        {
            return AppExitResponse.exit;
        }
        return await onExitRequested!();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void didChangeAppLifecycleState(AppLifecycleState state)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
        global::Doroti.Ui.AppLifecycleState? previousState = _lifecycleState;
        if (Equals(state, previousState))
        {
            return;
        }
        _lifecycleState = state;
        switch (state)
        {
            case var __constant8105 when Equals(__constant8105, AppLifecycleState.resumed):
                {
                    DartRuntimePrimitives.Assert(() => (previousState is null) || Equals(previousState, AppLifecycleState.inactive) || Equals(previousState, AppLifecycleState.detached), () => (object?)$"Invalid state transition from {previousState} to {state}");
                    onResume?.Invoke();
                    break;
                }
            case var __constant8419 when Equals(__constant8419, AppLifecycleState.inactive):
                {
                    DartRuntimePrimitives.Assert(() => (previousState is null) || Equals(previousState, AppLifecycleState.hidden) || Equals(previousState, AppLifecycleState.resumed), () => (object?)$"Invalid state transition from {previousState} to {state}");
                    if (Equals(previousState, AppLifecycleState.hidden))
                    {
                        onShow?.Invoke();
                    }
                    else
                    {
                        if ((previousState is null) || Equals(previousState, AppLifecycleState.resumed))
                        {
                            onInactive?.Invoke();
                        }
                    }
                    break;
                }
            case var __constant8918 when Equals(__constant8918, AppLifecycleState.hidden):
                {
                    DartRuntimePrimitives.Assert(() => (previousState is null) || Equals(previousState, AppLifecycleState.paused) || Equals(previousState, AppLifecycleState.inactive), () => (object?)$"Invalid state transition from {previousState} to {state}");
                    if (Equals(previousState, AppLifecycleState.paused))
                    {
                        onRestart?.Invoke();
                    }
                    else
                    {
                        if ((previousState is null) || Equals(previousState, AppLifecycleState.inactive))
                        {
                            onHide?.Invoke();
                        }
                    }
                    break;
                }
            case var __constant9416 when Equals(__constant9416, AppLifecycleState.paused):
                {
                    DartRuntimePrimitives.Assert(() => (previousState is null) || Equals(previousState, AppLifecycleState.hidden), () => (object?)$"Invalid state transition from {previousState} to {state}");
                    if ((previousState is null) || Equals(previousState, AppLifecycleState.hidden))
                    {
                        onPause?.Invoke();
                    }
                    break;
                }
            case var __constant9745 when Equals(__constant9745, AppLifecycleState.detached):
                {
                    DartRuntimePrimitives.Assert(() => (previousState is null) || Equals(previousState, AppLifecycleState.paused), () => (object?)$"Invalid state transition from {previousState} to {state}");
                    onDetach?.Invoke();
                    break;
                }
        }
        onStateChange?.Invoke(DartRuntimePrimitives.RequireValue(_lifecycleState));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<WidgetsBinding>("binding", binding));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("onStateChange", value: onStateChange is not null, ifTrue: "onStateChange"));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("onInactive", value: onInactive is not null, ifTrue: "onInactive"));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("onResume", value: onResume is not null, ifTrue: "onResume"));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("onHide", value: onHide is not null, ifTrue: "onHide"));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("onShow", value: onShow is not null, ifTrue: "onShow"));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("onPause", value: onPause is not null, ifTrue: "onPause"));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("onRestart", value: onRestart is not null, ifTrue: "onRestart"));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("onExitRequested", value: onExitRequested is not null, ifTrue: "onExitRequested"));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("onDetach", value: onDetach is not null, ifTrue: "onDetach"));
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

