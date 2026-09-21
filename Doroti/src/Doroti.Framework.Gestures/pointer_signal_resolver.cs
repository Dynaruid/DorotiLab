// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/pointer_signal_resolver.dart
using Doroti.Runtime;

namespace Doroti.Framework.Gestures;

public delegate void PointerSignalResolvedCallback(PointerSignalEvent @event);

public static partial class Pointer_signal_resolverLibrary
{
    internal static bool _isSameEvent(PointerSignalEvent event1, PointerSignalEvent event2)
    {
        return Equals(event1.original ?? event1, event2.original ?? event2);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class PointerSignalResolver
{
    internal virtual Action<PointerSignalEvent>? _firstRegisteredCallback { get; set; } = default;
    internal virtual PointerSignalEvent? _currentEvent { get; set; } = default;

    public virtual void register(PointerSignalEvent @event, Action<PointerSignalEvent> callback)
    {
        DartRuntimePrimitives.Assert(() =>
            (_currentEvent is null)
            || Pointer_signal_resolverLibrary._isSameEvent(_currentEvent!, @event)
        );
        if (_firstRegisteredCallback is not null)
        {
            return;
        }
        _currentEvent = @event;
        _firstRegisteredCallback = callback;
    }

    public virtual void resolve(PointerSignalEvent @event)
    {
        if (_firstRegisteredCallback is null)
        {
            DartRuntimePrimitives.Assert(() => _currentEvent is null);
            @event.respond(allowPlatformDefault: true);
            return;
        }
        DartRuntimePrimitives.Assert(() =>
            Pointer_signal_resolverLibrary._isSameEvent(_currentEvent!, @event)
        );
        try
        {
            _firstRegisteredCallback!(_currentEvent!);
        }
        catch (Exception exceptionLocal)
        {
            var stackLocal = new System.Diagnostics.StackTrace();
            InformationCollector? collector = default!;
            DartRuntimePrimitives.Assert(() =>
            {
                collector = () =>
                    new List<DiagnosticsNode>
                    {
                        new DiagnosticsProperty<PointerSignalEvent>(
                            "Event",
                            @event,
                            style: DiagnosticsTreeStyle.errorProperty
                        ),
                    };
                return true;
            });
            FlutterError.reportError(
                new FlutterErrorDetails(
                    exception: exceptionLocal,
                    stack: stackLocal,
                    library: "gesture library",
                    context: new ErrorDescription("while resolving a PointerSignalEvent"),
                    informationCollector: collector
                )
            );
        }
        _firstRegisteredCallback = null;
        _currentEvent = null;
    }
}
