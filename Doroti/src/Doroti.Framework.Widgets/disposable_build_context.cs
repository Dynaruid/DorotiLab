// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/disposable_build_context.dart
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

namespace Doroti.Generated.Framework.Widgets;

public class DisposableBuildContext<T> where T : IState
{
    internal virtual T? _state { get; set; } = default;

    public DisposableBuildContext(T _state)
    {
        this._state = _state;
        System.Diagnostics.Debug.Assert(_state.mounted);
    }

    public virtual BuildContext? context
    {
        get
        {
            DartRuntimePrimitives.Assert(() => _debugValidate());
            return this._state?.context;
            return default!;
        }
    }
    internal virtual bool _debugValidate()
    {
        DartRuntimePrimitives.Assert(() => ((this._state is null) || this._state!.mounted), () => (object?)"A DisposableBuildContext tried to access the BuildContext of a disposed " + "State object. This can happen when the creator of this " + "DisposableBuildContext fails to call dispose when it is disposed.");
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        _state = default(T);
    }

}

