// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/disposable_build_context.dart
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

public interface IDisposableBuildContext
{
    BuildContext? context { get; }
}

public class DisposableBuildContext<T> : IDisposableBuildContext where T : IState
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
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        _state = default(T);
    }

}

