// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/app_lifecycle_listener.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

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
        this.binding = ((binding ?? (WidgetsBinding)WidgetsBinding.instance));
        this._lifecycleState = (((binding ?? (WidgetsBinding)WidgetsBinding.instance))).lifecycleState;
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this.binding.removeObserver(this);
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
                if (this._debugDisposed)
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create($"A {this.GetType()} was used after being disposed.\n" + $"Once you have called dispose() on a {this.GetType()}, it " + "can no longer be used."));
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
        if ((this.onExitRequested is null))
        {
            return AppExitResponse.exit;
        }
        return await this.onExitRequested!();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void didChangeAppLifecycleState(AppLifecycleState state)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
        global::Doroti.Ui.AppLifecycleState? previousState = ((global::Doroti.Ui.AppLifecycleState?)(object?)this._lifecycleState);
        if ((object.Equals(state, previousState)))
        {
            return;
        }
        _lifecycleState = state;
        switch (state)
        {
            case var __constant8105 when (object.Equals(__constant8105, AppLifecycleState.resumed)):
                {
                    DartRuntimePrimitives.Assert(() => (((previousState is null) || (object.Equals(previousState, AppLifecycleState.inactive))) || (object.Equals(previousState, AppLifecycleState.detached))), () => (object?)$"Invalid state transition from {previousState} to {state}");
                    this.onResume?.Invoke();
                    break;
                }
            case var __constant8419 when (object.Equals(__constant8419, AppLifecycleState.inactive)):
                {
                    DartRuntimePrimitives.Assert(() => (((previousState is null) || (object.Equals(previousState, AppLifecycleState.hidden))) || (object.Equals(previousState, AppLifecycleState.resumed))), () => (object?)$"Invalid state transition from {previousState} to {state}");
                    if ((object.Equals(previousState, AppLifecycleState.hidden)))
                    {
                        this.onShow?.Invoke();
                    }
                    else
                    {
                        if (((previousState is null) || (object.Equals(previousState, AppLifecycleState.resumed))))
                        {
                            this.onInactive?.Invoke();
                        }
                    }
                    break;
                }
            case var __constant8918 when (object.Equals(__constant8918, AppLifecycleState.hidden)):
                {
                    DartRuntimePrimitives.Assert(() => (((previousState is null) || (object.Equals(previousState, AppLifecycleState.paused))) || (object.Equals(previousState, AppLifecycleState.inactive))), () => (object?)$"Invalid state transition from {previousState} to {state}");
                    if ((object.Equals(previousState, AppLifecycleState.paused)))
                    {
                        this.onRestart?.Invoke();
                    }
                    else
                    {
                        if (((previousState is null) || (object.Equals(previousState, AppLifecycleState.inactive))))
                        {
                            this.onHide?.Invoke();
                        }
                    }
                    break;
                }
            case var __constant9416 when (object.Equals(__constant9416, AppLifecycleState.paused)):
                {
                    DartRuntimePrimitives.Assert(() => ((previousState is null) || (object.Equals(previousState, AppLifecycleState.hidden))), () => (object?)$"Invalid state transition from {previousState} to {state}");
                    if (((previousState is null) || (object.Equals(previousState, AppLifecycleState.hidden))))
                    {
                        this.onPause?.Invoke();
                    }
                    break;
                }
            case var __constant9745 when (object.Equals(__constant9745, AppLifecycleState.detached)):
                {
                    DartRuntimePrimitives.Assert(() => ((previousState is null) || (object.Equals(previousState, AppLifecycleState.paused))), () => (object?)$"Invalid state transition from {previousState} to {state}");
                    this.onDetach?.Invoke();
                    break;
                }
        }
        this.onStateChange?.Invoke(DartRuntimePrimitives.RequireValue(this._lifecycleState));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<WidgetsBinding>("binding", this.binding));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("onStateChange", value: (this.onStateChange is not null), ifTrue: "onStateChange"));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("onInactive", value: (this.onInactive is not null), ifTrue: "onInactive"));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("onResume", value: (this.onResume is not null), ifTrue: "onResume"));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("onHide", value: (this.onHide is not null), ifTrue: "onHide"));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("onShow", value: (this.onShow is not null), ifTrue: "onShow"));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("onPause", value: (this.onPause is not null), ifTrue: "onPause"));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("onRestart", value: (this.onRestart is not null), ifTrue: "onRestart"));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("onExitRequested", value: (this.onExitRequested is not null), ifTrue: "onExitRequested"));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("onDetach", value: (this.onDetach is not null), ifTrue: "onDetach"));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((fullString ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

