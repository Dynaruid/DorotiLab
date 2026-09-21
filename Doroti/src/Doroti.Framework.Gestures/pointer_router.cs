// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/pointer_router.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Gestures;

public delegate void PointerRoute(PointerEvent @event);

public class PointerRouter
{
    internal virtual DartMap<long, DartMap<Action<PointerEvent>, Matrix4?>> _routeMap
    {
        get;
        private set;
    } = new DartMap<long, DartMap<Action<PointerEvent>, Matrix4?>>();
    internal virtual DartMap<Action<PointerEvent>, Matrix4?> _globalRoutes { get; private set; } =
        new DartMap<Action<PointerEvent>, Matrix4?>();

    public virtual void addRoute(
        long pointer,
        Action<PointerEvent> route,
        Matrix4? transform = null
    )
    {
        DartMap<Action<PointerEvent>, Matrix4?> routes = _routeMap.putIfAbsent(
            pointer,
            () => new DartMap<Action<PointerEvent>, Matrix4?>()
        );
        DartRuntimePrimitives.Assert(() => !routes.ContainsKey(route));
        routes[route] = transform;
    }

    public virtual void removeRoute(long pointer, Action<PointerEvent> route)
    {
        DartRuntimePrimitives.Assert(() => _routeMap.ContainsKey(pointer));
        DartMap<Action<PointerEvent>, Matrix4?> routes = _routeMap.GetValueOrDefault(pointer)!;
        DartRuntimePrimitives.Assert(() => routes.ContainsKey(route));
        routes.remove(route);
        if (checked((long)routes.Count) == 0)
        {
            _routeMap.remove(pointer);
        }
    }

    public virtual void addGlobalRoute(Action<PointerEvent> route, Matrix4? transform = null)
    {
        DartRuntimePrimitives.Assert(() => !_globalRoutes.ContainsKey(route));
        _globalRoutes[route] = transform;
    }

    public virtual void removeGlobalRoute(Action<PointerEvent> route)
    {
        DartRuntimePrimitives.Assert(() => _globalRoutes.ContainsKey(route));
        _globalRoutes.remove(route);
    }

    public virtual long debugGlobalRouteCount
    {
        get
        {
            long? count = default!;
            DartRuntimePrimitives.Assert(() =>
            {
                count = checked(_globalRoutes.Count);
                return true;
            });
            if (count is not null)
            {
                long count__2895__value2991 = (
                    count
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
                return (count__2895__value2991);
            }
            throw new NotSupportedException(
                "debugGlobalRouteCount is not supported in release builds"
            );
        }
    }

    internal virtual void _dispatch(
        PointerEvent @event,
        Action<PointerEvent> route,
        Matrix4? transform
    )
    {
        try
        {
            @event = @event.transformed(transform);
            route(@event);
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
                        new DiagnosticsProperty<PointerRouter>(
                            "router",
                            this,
                            level: DiagnosticLevel.debug
                        ),
                        new DiagnosticsProperty<Action<PointerEvent>>(
                            "route",
                            route,
                            level: DiagnosticLevel.debug
                        ),
                        new DiagnosticsProperty<PointerEvent>(
                            "event",
                            @event,
                            level: DiagnosticLevel.debug
                        ),
                    };
                return true;
            });
            FlutterError.reportError(
                new FlutterErrorDetails(
                    exception: exceptionLocal,
                    stack: stackLocal,
                    library: "gesture library",
                    context: new ErrorDescription("while routing a pointer event"),
                    informationCollector: collector
                )
            );
        }
    }

    public virtual void route(PointerEvent @event)
    {
        DartMap<Action<PointerEvent>, Matrix4?>? routes = _routeMap.GetValueOrDefault(
            @event.pointer
        );
        var copiedGlobalRoutes = new DartMap<Action<PointerEvent>, Matrix4?>(_globalRoutes);
        if (routes is not null)
        {
            _dispatchEventToRoutes(
                @event,
                routes,
                new DartMap<Action<PointerEvent>, Matrix4?>(routes)
            );
        }
        _dispatchEventToRoutes(@event, _globalRoutes, copiedGlobalRoutes);
    }

    internal virtual void _dispatchEventToRoutes(
        PointerEvent @event,
        DartMap<Action<PointerEvent>, Matrix4?> referenceRoutes,
        DartMap<Action<PointerEvent>, Matrix4?> copiedRoutes
    )
    {
        copiedRoutes.forEach(
            (route, transform) =>
            {
                if (referenceRoutes.ContainsKey(route))
                {
                    _dispatch(@event, route, transform);
                }
            }
        );
    }
}
