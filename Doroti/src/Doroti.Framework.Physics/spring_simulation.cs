// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/physics/spring_simulation.dart
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

namespace Doroti.Framework.Physics;

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
        var __instance = new SpringDescription(mass, stiffness, default!);
        __instance.mass = mass;
        __instance.stiffness = stiffness;
        __instance.damping = ((ratio * 2.0) * global::Doroti.Runtime.Dart_mathLibrary.sqrt((mass * stiffness)));
        return __instance;
    }

    public static SpringDescription CreateWithDurationAndBounce(Duration? duration = null, double bounce = 0.0)
    {
        Duration __duration = duration ?? Duration.Create(milliseconds: 500);
        DartRuntimePrimitives.Assert(() => (__duration.inMilliseconds > 0L));
        double durationInSeconds = (__duration.inMilliseconds / Duration.millisecondsPerSecond);
        var massLocal = 1.0;
        double stiffnessLocal = (((((4L * Dart_mathLibrary.pi) * Dart_mathLibrary.pi) * massLocal)) / global::Doroti.Runtime.Dart_mathLibrary.pow(durationInSeconds, 2L));
        double dampingRatio = ((bounce > 0L) ? ((1.0 - bounce)) : ((1L / ((bounce + 1L)))));
        double dampingLocal = ((dampingRatio * 2.0) * global::Doroti.Runtime.Dart_mathLibrary.sqrt((massLocal * stiffnessLocal)));
        return new SpringDescription(mass: massLocal, stiffness: stiffnessLocal, damping: dampingLocal);
    }

    public virtual Duration duration
    {
        get
        {
            double durationInSeconds = global::Doroti.Runtime.Dart_mathLibrary.sqrt((((((4L * Dart_mathLibrary.pi) * Dart_mathLibrary.pi) * this.mass)) / this.stiffness));
            long millisecondsLocal = ((durationInSeconds * Duration.millisecondsPerSecond)).round();
            return Duration.Create(milliseconds: millisecondsLocal);
            return default!;
        }
    }
    public virtual double bounce
    {
        get
        {
            double dampingRatio = (this.damping / ((2.0 * global::Doroti.Runtime.Dart_mathLibrary.sqrt((this.mass * this.stiffness)))));
            return ((dampingRatio < 1.0) ? ((1.0 - dampingRatio)) : ((((1L / dampingRatio)) - 1L)));
            return default!;
        }
    }
    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SpringDescription"))}(mass: {this.mass.toStringAsFixed(1L)}, stiffness: {this.stiffness.toStringAsFixed(1L)}, damping: {this.damping.toStringAsFixed(1L)})";
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
        return (global::Doroti.Framework.Physics.UtilsLibrary.nearZero(this._solution.x(time), ((Tolerance)tolerance).distance) && global::Doroti.Framework.Physics.UtilsLibrary.nearZero(this._solution.dx(time), ((Tolerance)tolerance).velocity));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SpringSimulation"))}(end: {this._endPosition.toStringAsFixed(1L)}, {this.type})";
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
        double r = (-((SpringDescription)spring).damping / ((2.0 * ((SpringDescription)spring).mass)));
        var c1 = distance;
        double c2 = (velocity - ((r * distance)));
        return new _CriticalSolution__spring_simulation(r, c1, c2);
    }

    internal _CriticalSolution__spring_simulation(double r, double c1, double c2)
    {
        this._r = r;
        this._c1 = c1;
        this._c2 = c2;
    }

    public virtual double x(double time)
    {
        return (((this._c1 + (this._c2 * time))) * global::Doroti.Runtime.Dart_mathLibrary.pow(global::Doroti.Runtime.Dart_mathLibrary.e, (this._r * time)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double dx(double time)
    {
        var power = ((double)global::Doroti.Runtime.Dart_mathLibrary.pow(global::Doroti.Runtime.Dart_mathLibrary.e, (this._r * time)));
        return (((this._r * ((this._c1 + (this._c2 * time)))) * power) + (this._c2 * power));
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
        double cmk = ((((SpringDescription)spring).damping * ((SpringDescription)spring).damping) - ((4L * ((SpringDescription)spring).mass) * ((SpringDescription)spring).stiffness));
        double r1 = (((-((SpringDescription)spring).damping - global::Doroti.Runtime.Dart_mathLibrary.sqrt(cmk))) / ((2.0 * ((SpringDescription)spring).mass)));
        double r2 = (((-((SpringDescription)spring).damping + global::Doroti.Runtime.Dart_mathLibrary.sqrt(cmk))) / ((2.0 * ((SpringDescription)spring).mass)));
        double c2 = (((velocity - (r1 * distance))) / ((r2 - r1)));
        double c1 = (distance - c2);
        return new _OverdampedSolution__spring_simulation(r1, r2, c1, c2);
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
        return ((this._c1 * global::Doroti.Runtime.Dart_mathLibrary.pow(global::Doroti.Runtime.Dart_mathLibrary.e, (this._r1 * time))) + (this._c2 * global::Doroti.Runtime.Dart_mathLibrary.pow(global::Doroti.Runtime.Dart_mathLibrary.e, (this._r2 * time))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double dx(double time)
    {
        return (((this._c1 * this._r1) * global::Doroti.Runtime.Dart_mathLibrary.pow(global::Doroti.Runtime.Dart_mathLibrary.e, (this._r1 * time))) + ((this._c2 * this._r2) * global::Doroti.Runtime.Dart_mathLibrary.pow(global::Doroti.Runtime.Dart_mathLibrary.e, (this._r2 * time))));
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
        double w = (global::Doroti.Runtime.Dart_mathLibrary.sqrt((((4.0 * ((SpringDescription)spring).mass) * ((SpringDescription)spring).stiffness) - (((SpringDescription)spring).damping * ((SpringDescription)spring).damping))) / ((2.0 * ((SpringDescription)spring).mass)));
        double r = -(((((SpringDescription)spring).damping / 2.0) / ((SpringDescription)spring).mass));
        var c1 = distance;
        double c2 = (((velocity - (r * distance))) / w);
        return new _UnderdampedSolution__spring_simulation(w, r, c1, c2);
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
        return ((((double)global::Doroti.Runtime.Dart_mathLibrary.pow(global::Doroti.Runtime.Dart_mathLibrary.e, (this._r * time)))) * (((this._c1 * global::Doroti.Runtime.Dart_mathLibrary.cos((this._w * time))) + (this._c2 * global::Doroti.Runtime.Dart_mathLibrary.sin((this._w * time))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double dx(double time)
    {
        var power = ((double)global::Doroti.Runtime.Dart_mathLibrary.pow(global::Doroti.Runtime.Dart_mathLibrary.e, (this._r * time)));
        double cosine = global::Doroti.Runtime.Dart_mathLibrary.cos((this._w * time));
        double sine = global::Doroti.Runtime.Dart_mathLibrary.sin((this._w * time));
        return ((power * ((((this._c2 * this._w) * cosine) - ((this._c1 * this._w) * sine)))) + ((this._r * power) * (((this._c2 * sine) + (this._c1 * cosine)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SpringType type => SpringType.underDamped;
}

