// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/physics/spring_simulation.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Physics;

public class SpringDescription
{
    public virtual double mass { get; private set; } = default!;
    public virtual double stiffness { get; private set; } = default!;
    public virtual double damping { get; private set; } = default!;

    public SpringDescription(double mass, double stiffness, double damping)
    {
        this.mass = mass;
        this.stiffness = stiffness;
        this.damping = damping;
    }

    public static SpringDescription CreateWithDampingRatio(double mass, double stiffness, double ratio = 1.0)
    {
        var __instance = new SpringDescription(default!, default!, default!);
        __instance.mass = mass;
        __instance.stiffness = stiffness;
        __instance.damping = ((ratio * 2.0) * global::Doroti.Flutter.Runtime.Dart_mathLibrary.sqrt((mass * stiffness)));
        return __instance;
    }

    public static SpringDescription CreateWithDurationAndBounce(Duration? duration = null, double bounce = 0.0)
    {
        Duration __duration = duration ?? Duration.Create(milliseconds: 500);
        DartRuntimePrimitives.Assert(() => (__duration.inMilliseconds > 0L));
        double durationInSeconds__2760 = (__duration.inMilliseconds / Duration.millisecondsPerSecond);
        var mass__2848 = 1.0;
        double stiffness__2877 = (((((4L * Dart_mathLibrary.pi) * Dart_mathLibrary.pi) * mass__2848)) / global::Doroti.Flutter.Runtime.Dart_mathLibrary.pow(durationInSeconds__2760, 2L));
        double dampingRatio__2971 = ((bounce > 0L) ? ((1.0 - bounce)) : ((1L / ((bounce + 1L)))));
        double damping__3053 = ((dampingRatio__2971 * 2.0) * global::Doroti.Flutter.Runtime.Dart_mathLibrary.sqrt((mass__2848 * stiffness__2877)));
        return new SpringDescription(mass: mass__2848, stiffness: stiffness__2877, damping: damping__3053);
    }

    public virtual Duration duration
    {
        get
        {
            double durationInSeconds__5454 = global::Doroti.Flutter.Runtime.Dart_mathLibrary.sqrt((((((4L * Dart_mathLibrary.pi) * Dart_mathLibrary.pi) * this.mass)) / this.stiffness));
            long milliseconds__5543 = ((durationInSeconds__5454 * Duration.millisecondsPerSecond)).round();
            return Duration.Create(milliseconds: milliseconds__5543);
            return default!;
        }
    }
    public virtual double bounce
    {
        get
        {
            double dampingRatio__6296 = (this.damping / ((2.0 * global::Doroti.Flutter.Runtime.Dart_mathLibrary.sqrt((this.mass * this.stiffness)))));
            return ((dampingRatio__6296 < 1.0) ? ((1.0 - dampingRatio__6296)) : ((((1L / dampingRatio__6296)) - 1L)));
            return default!;
        }
    }
    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SpringDescription"))}(mass: {this.mass.toStringAsFixed(1L)}, stiffness: {this.stiffness.toStringAsFixed(1L)}, damping: {this.damping.toStringAsFixed(1L)})";
}

public enum SpringType
{
    criticallyDamped,
    underDamped,
    overDamped
}

public class SpringSimulation : Simulation
{
    internal virtual double _endPosition { get; private set; } = default!;
    internal virtual _SpringSolution__spring_simulation _solution { get; private set; } = default!;
    internal virtual bool _snapToEnd { get; private set; } = default!;

    public SpringSimulation(SpringDescription spring, double start, double end, double velocity, bool snapToEnd = false, Tolerance tolerance = default!) : base(tolerance: tolerance ?? Tolerance.defaultTolerance)
    {
        this._endPosition = end;
        this._solution = _SpringSolution__spring_simulation.Create(spring, (start - end), velocity);
        this._snapToEnd = snapToEnd;
    }

    public virtual SpringType type => ((_SpringSolution__spring_simulation)this._solution).type;
    public override double x(double time)
    {
        if ((this._snapToEnd && isDone(time)))
        {
            return this._endPosition;
        }
        else
        {
            return (this._endPosition + this._solution.x(time));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double dx(double time)
    {
        if ((this._snapToEnd && isDone(time)))
        {
            return 0;
        }
        else
        {
            return this._solution.dx(time);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isDone(double time)
    {
        return (global::Doroti.Generated.Framework.Physics.UtilsLibrary.nearZero(this._solution.x(time), ((Tolerance)tolerance).distance) && global::Doroti.Generated.Framework.Physics.UtilsLibrary.nearZero(this._solution.dx(time), ((Tolerance)tolerance).velocity));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SpringSimulation"))}(end: {this._endPosition.toStringAsFixed(1L)}, {this.type})";
}

public class ScrollSpringSimulation : SpringSimulation
{
    public ScrollSpringSimulation(SpringDescription spring, double start, double end, double velocity, Tolerance tolerance = default!) : base(spring, start, end, velocity, tolerance: tolerance ?? Tolerance.defaultTolerance)
    {
    }

    public override double x(double time) => (isDone(time) ? _endPosition : base.x(time));
}

internal interface _SpringSolution__spring_simulation
{
    internal static _SpringSolution__spring_simulation Create(SpringDescription spring, double initialPosition, double initialVelocity)
    {
        return (((((SpringDescription)spring).damping * ((SpringDescription)spring).damping) - ((4L * ((SpringDescription)spring).mass) * ((SpringDescription)spring).stiffness)) switch { > 0.0 => _OverdampedSolution__spring_simulation.Create(spring, initialPosition, initialVelocity), < 0.0 => _UnderdampedSolution__spring_simulation.Create(spring, initialPosition, initialVelocity), _ => _CriticalSolution__spring_simulation.Create(spring, initialPosition, initialVelocity) });
    }

    public double x(double time);
    public double dx(double time);
    public SpringType type { get; }
}

internal class _CriticalSolution__spring_simulation : _SpringSolution__spring_simulation
{
    internal virtual double _r { get; private set; } = default!;
    internal virtual double _c1 { get; private set; } = default!;
    internal virtual double _c2 { get; private set; } = default!;

    internal static _CriticalSolution__spring_simulation Create(SpringDescription spring, double distance, double velocity)
    {
        double r__11282 = (-((SpringDescription)spring).damping / ((2.0 * ((SpringDescription)spring).mass)));
        var c1__11335 = distance;
        double c2__11367 = (velocity - ((r__11282 * distance)));
        return new _CriticalSolution__spring_simulation(r__11282, c1__11335, c2__11367);
    }

    internal _CriticalSolution__spring_simulation(double r, double c1, double c2)
    {
        this._r = r;
        this._c1 = c1;
        this._c2 = c2;
    }

    public virtual double x(double time)
    {
        return (((this._c1 + (this._c2 * time))) * global::Doroti.Flutter.Runtime.Dart_mathLibrary.pow(global::Doroti.Flutter.Runtime.Dart_mathLibrary.e, (this._r * time)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double dx(double time)
    {
        var power__11729 = ((double)global::Doroti.Flutter.Runtime.Dart_mathLibrary.pow(global::Doroti.Flutter.Runtime.Dart_mathLibrary.e, (this._r * time)));
        return (((this._r * ((this._c1 + (this._c2 * time)))) * power__11729) + (this._c2 * power__11729));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SpringType type => SpringType.criticallyDamped;
}

internal class _OverdampedSolution__spring_simulation : _SpringSolution__spring_simulation
{
    internal virtual double _r1 { get; private set; } = default!;
    internal virtual double _r2 { get; private set; } = default!;
    internal virtual double _c1 { get; private set; } = default!;
    internal virtual double _c2 { get; private set; } = default!;

    internal static _OverdampedSolution__spring_simulation Create(SpringDescription spring, double distance, double velocity)
    {
        double cmk__12072 = ((((SpringDescription)spring).damping * ((SpringDescription)spring).damping) - ((4L * ((SpringDescription)spring).mass) * ((SpringDescription)spring).stiffness));
        double r1__12165 = (((-((SpringDescription)spring).damping - global::Doroti.Flutter.Runtime.Dart_mathLibrary.sqrt(cmk__12072))) / ((2.0 * ((SpringDescription)spring).mass)));
        double r2__12245 = (((-((SpringDescription)spring).damping + global::Doroti.Flutter.Runtime.Dart_mathLibrary.sqrt(cmk__12072))) / ((2.0 * ((SpringDescription)spring).mass)));
        double c2__12325 = (((velocity - (r1__12165 * distance))) / ((r2__12245 - r1__12165)));
        double c1__12387 = (distance - c2__12325);
        return new _OverdampedSolution__spring_simulation(r1__12165, r2__12245, c1__12387, c2__12325);
    }

    internal _OverdampedSolution__spring_simulation(double r1, double r2, double c1, double c2)
    {
        this._r1 = r1;
        this._r2 = r2;
        this._c1 = c1;
        this._c2 = c2;
    }

    public virtual double x(double time)
    {
        return ((this._c1 * global::Doroti.Flutter.Runtime.Dart_mathLibrary.pow(global::Doroti.Flutter.Runtime.Dart_mathLibrary.e, (this._r1 * time))) + (this._c2 * global::Doroti.Flutter.Runtime.Dart_mathLibrary.pow(global::Doroti.Flutter.Runtime.Dart_mathLibrary.e, (this._r2 * time))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double dx(double time)
    {
        return (((this._c1 * this._r1) * global::Doroti.Flutter.Runtime.Dart_mathLibrary.pow(global::Doroti.Flutter.Runtime.Dart_mathLibrary.e, (this._r1 * time))) + ((this._c2 * this._r2) * global::Doroti.Flutter.Runtime.Dart_mathLibrary.pow(global::Doroti.Flutter.Runtime.Dart_mathLibrary.e, (this._r2 * time))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SpringType type => SpringType.overDamped;
}

internal class _UnderdampedSolution__spring_simulation : _SpringSolution__spring_simulation
{
    internal virtual double _w { get; private set; } = default!;
    internal virtual double _r { get; private set; } = default!;
    internal virtual double _c1 { get; private set; } = default!;
    internal virtual double _c2 { get; private set; } = default!;

    internal static _UnderdampedSolution__spring_simulation Create(SpringDescription spring, double distance, double velocity)
    {
        double w__13141 = (global::Doroti.Flutter.Runtime.Dart_mathLibrary.sqrt((((4.0 * ((SpringDescription)spring).mass) * ((SpringDescription)spring).stiffness) - (((SpringDescription)spring).damping * ((SpringDescription)spring).damping))) / ((2.0 * ((SpringDescription)spring).mass)));
        double r__13283 = -(((((SpringDescription)spring).damping / 2.0) / ((SpringDescription)spring).mass));
        var c1__13336 = distance;
        double c2__13368 = (((velocity - (r__13283 * distance))) / w__13141);
        return new _UnderdampedSolution__spring_simulation(w__13141, r__13283, c1__13336, c2__13368);
    }

    internal _UnderdampedSolution__spring_simulation(double w, double r, double c1, double c2)
    {
        this._w = w;
        this._r = r;
        this._c1 = c1;
        this._c2 = c2;
    }

    public virtual double x(double time)
    {
        return ((((double)global::Doroti.Flutter.Runtime.Dart_mathLibrary.pow(global::Doroti.Flutter.Runtime.Dart_mathLibrary.e, (this._r * time)))) * (((this._c1 * global::Doroti.Flutter.Runtime.Dart_mathLibrary.cos((this._w * time))) + (this._c2 * global::Doroti.Flutter.Runtime.Dart_mathLibrary.sin((this._w * time))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double dx(double time)
    {
        var power__13844 = ((double)global::Doroti.Flutter.Runtime.Dart_mathLibrary.pow(global::Doroti.Flutter.Runtime.Dart_mathLibrary.e, (this._r * time)));
        double cosine__13908 = global::Doroti.Flutter.Runtime.Dart_mathLibrary.cos((this._w * time));
        double sine__13955 = global::Doroti.Flutter.Runtime.Dart_mathLibrary.sin((this._w * time));
        return ((power__13844 * ((((this._c2 * this._w) * cosine__13908) - ((this._c1 * this._w) * sine__13955)))) + ((this._r * power__13844) * (((this._c2 * sine__13955) + (this._c1 * cosine__13908)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SpringType type => SpringType.underDamped;
}

