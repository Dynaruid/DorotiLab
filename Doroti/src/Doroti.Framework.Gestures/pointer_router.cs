// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/pointer_router.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Gestures;

public delegate void PointerRoute(PointerEvent @event);

public class PointerRouter
{
    internal virtual DartMap<long, DartMap<Action<PointerEvent>, Matrix4?>> _routeMap { get; private set; } = new DartMap<long, DartMap<Action<PointerEvent>, Matrix4?>>();
    internal virtual DartMap<Action<PointerEvent>, Matrix4?> _globalRoutes { get; private set; } = new DartMap<Action<PointerEvent>, Matrix4?>();

    public virtual void addRoute(long pointer, Action<PointerEvent> route, Matrix4? transform = null)
    {
        DartMap<Action<PointerEvent>, Matrix4?> routes__1058 = this._routeMap.putIfAbsent(pointer, (() => new DartMap<Action<PointerEvent>, Matrix4?>()));
        DartRuntimePrimitives.Assert(() => !routes__1058.ContainsKey(route));
        routes__1058[route] = transform;
    }

    public virtual void removeRoute(long pointer, Action<PointerEvent> route)
    {
        DartRuntimePrimitives.Assert(() => this._routeMap.ContainsKey(pointer));
        DartMap<Action<PointerEvent>, Matrix4?> routes__1670 = this._routeMap.GetValueOrDefault(pointer)!;
        DartRuntimePrimitives.Assert(() => routes__1670.ContainsKey(route));
        routes__1670.remove(route);
        if ((checked((long)(routes__1670.Count)) == 0))
        {
            this._routeMap.remove(pointer);
        }
    }

    public virtual void addGlobalRoute(Action<PointerEvent> route, Matrix4? transform = null)
    {
        DartRuntimePrimitives.Assert(() => !this._globalRoutes.ContainsKey(route));
        this._globalRoutes[route] = transform;
    }

    public virtual void removeGlobalRoute(Action<PointerEvent> route)
    {
        DartRuntimePrimitives.Assert(() => this._globalRoutes.ContainsKey(route));
        this._globalRoutes.remove(route);
    }

    public virtual long debugGlobalRouteCount
    {
        get
        {
            long? count__2895 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    count__2895 = checked((long)(this._globalRoutes.Count));
                    return true;
                });
            if ((count__2895 is not null))
            {
                long count__2895__value2991 = DartRuntimePrimitives.RequireValue(count__2895);
                return DartRuntimePrimitives.RequireValue(count__2895__value2991);
            }
            throw new NotSupportedException("debugGlobalRouteCount is not supported in release builds");
            return default!;
        }
    }
    internal virtual void _dispatch(PointerEvent @event, Action<PointerEvent> route, Matrix4? transform)
    {
        try
        {
            @event = @event.transformed(transform);
            route(@event);
        }
        catch (Exception exception__3339)
        {
            var stack__3350 = new System.Diagnostics.StackTrace();
            InformationCollector? collector__3387 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    collector__3387 = (() => new List<DiagnosticsNode> { new DiagnosticsProperty<PointerRouter>("router", this, level: DiagnosticLevel.debug), new DiagnosticsProperty<Action<PointerEvent>>("route", route, level: DiagnosticLevel.debug), new DiagnosticsProperty<PointerEvent>("event", @event, level: DiagnosticLevel.debug) });
                    return true;
                });
            FlutterError.reportError(new FlutterErrorDetails(exception: exception__3339, stack: stack__3350, library: "gesture library", context: new ErrorDescription("while routing a pointer event"), informationCollector: collector__3387));
        }
    }

    public virtual void route(PointerEvent @event)
    {
        DartMap<Action<PointerEvent>, Matrix4?>? routes__4313 = this._routeMap.GetValueOrDefault(((PointerEvent)@event).pointer);
        var copiedGlobalRoutes__4358 = new DartMap<Action<PointerEvent>, Matrix4?>(this._globalRoutes);
        if ((routes__4313 is not null))
        {
            _dispatchEventToRoutes(@event, (DartMap<Action<PointerEvent>, Matrix4?>)routes__4313, new DartMap<Action<PointerEvent>, Matrix4?>(routes__4313));
        }
        _dispatchEventToRoutes(@event, (DartMap<Action<PointerEvent>, Matrix4?>)this._globalRoutes, (DartMap<Action<PointerEvent>, Matrix4?>)copiedGlobalRoutes__4358);
    }

    internal virtual void _dispatchEventToRoutes(PointerEvent @event, DartMap<Action<PointerEvent>, Matrix4?> referenceRoutes, DartMap<Action<PointerEvent>, Matrix4?> copiedRoutes)
    {
        copiedRoutes.forEach(((route, transform) =>
        {
            if (referenceRoutes.ContainsKey(route))
            {
                _dispatch(@event, (Action<PointerEvent>)route, transform);
            }
        }));
    }

}

