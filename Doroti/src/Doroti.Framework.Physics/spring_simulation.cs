// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/physics/spring_simulation.dart
using Doroti.Runtime;

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

    public static SpringDescription CreateWithDampingRatio(
        double mass,
        double stiffness,
        double ratio = 1.0
    )
    {
        var __instance = new SpringDescription(mass, stiffness, default!);
        __instance.mass = mass;
        __instance.stiffness = stiffness;
        __instance.damping = ratio * 2.0 * Dart_mathLibrary.sqrt(mass * stiffness);
        return __instance;
    }

    public static SpringDescription CreateWithDurationAndBounce(
        Duration? duration = null,
        double bounce = 0.0
    )
    {
        Duration __duration = duration ?? Duration.Create(milliseconds: 500);
        DartRuntimePrimitives.Assert(() => __duration.inMilliseconds > 0L);
        double durationInSeconds = __duration.inMilliseconds / Duration.millisecondsPerSecond;
        var massLocal = 1.0;
        double stiffnessLocal =
            4L
            * Dart_mathLibrary.pi
            * Dart_mathLibrary.pi
            * massLocal
            / Dart_mathLibrary.pow(durationInSeconds, 2L);
        double dampingRatio = (bounce > 0L) ? (1.0 - bounce) : (1L / (bounce + 1L));
        double dampingLocal =
            dampingRatio * 2.0 * Dart_mathLibrary.sqrt(massLocal * stiffnessLocal);
        return new SpringDescription(
            mass: massLocal,
            stiffness: stiffnessLocal,
            damping: dampingLocal
        );
    }

    public virtual Duration duration
    {
        get
        {
            double durationInSeconds = Dart_mathLibrary.sqrt(
                4L * Dart_mathLibrary.pi * Dart_mathLibrary.pi * mass / stiffness
            );
            long millisecondsLocal = (durationInSeconds * Duration.millisecondsPerSecond).round();
            return Duration.Create(milliseconds: millisecondsLocal);
        }
    }
    public virtual double bounce
    {
        get
        {
            double dampingRatio = damping / (2.0 * Dart_mathLibrary.sqrt(mass * stiffness));
            return (dampingRatio < 1.0) ? (1.0 - dampingRatio) : ((1L / dampingRatio) - 1L);
        }
    }

    public override string ToString() =>
        $"{Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SpringDescription")}(mass: {mass.toStringAsFixed(1L)}, stiffness: {stiffness.toStringAsFixed(1L)}, damping: {damping.toStringAsFixed(1L)})";
}

public enum SpringType
{
    criticallyDamped,
    underDamped,
    overDamped,
}

public class SpringSimulation : Simulation
{
    internal virtual double _endPosition { get; private set; } = default!;
    internal virtual _SpringSolution__spring_simulation _solution { get; private set; } = default!;
    internal virtual bool _snapToEnd { get; private set; } = default!;

    public SpringSimulation(
        SpringDescription spring,
        double start,
        double end,
        double velocity,
        bool snapToEnd = false,
        Tolerance tolerance = default!
    )
        : base(tolerance: tolerance ?? Tolerance.defaultTolerance)
    {
        _endPosition = end;
        _solution = _SpringSolution__spring_simulation.Create(spring, start - end, velocity);
        _snapToEnd = snapToEnd;
    }

    public virtual SpringType type => _solution.type;

    public override double x(double time)
    {
        if (_snapToEnd && isDone(time))
        {
            return _endPosition;
        }
        else
        {
            return _endPosition + _solution.x(time);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double dx(double time)
    {
        if (_snapToEnd && isDone(time))
        {
            return 0;
        }
        else
        {
            return _solution.dx(time);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isDone(double time)
    {
        return UtilsLibrary.nearZero(_solution.x(time), tolerance.distance)
            && UtilsLibrary.nearZero(_solution.dx(time), tolerance.velocity);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() =>
        $"{Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SpringSimulation")}(end: {_endPosition.toStringAsFixed(1L)}, {type})";
}

public class ScrollSpringSimulation : SpringSimulation
{
    public ScrollSpringSimulation(
        SpringDescription spring,
        double start,
        double end,
        double velocity,
        Tolerance tolerance = default!
    )
        : base(spring, start, end, velocity, tolerance: tolerance ?? Tolerance.defaultTolerance) { }

    public override double x(double time) => isDone(time) ? _endPosition : base.x(time);
}

internal interface _SpringSolution__spring_simulation
{
    internal static _SpringSolution__spring_simulation Create(
        SpringDescription spring,
        double initialPosition,
        double initialVelocity
    )
    {
        return ((spring.damping * spring.damping) - (4L * spring.mass * spring.stiffness)) switch
        {
            > 0.0 => _OverdampedSolution__spring_simulation.Create(
                spring,
                initialPosition,
                initialVelocity
            ),
            < 0.0 => _UnderdampedSolution__spring_simulation.Create(
                spring,
                initialPosition,
                initialVelocity
            ),
            _ => _CriticalSolution__spring_simulation.Create(
                spring,
                initialPosition,
                initialVelocity
            ),
        };
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

    internal static _CriticalSolution__spring_simulation Create(
        SpringDescription spring,
        double distance,
        double velocity
    )
    {
        double r = -spring.damping / (2.0 * spring.mass);
        var c1 = distance;
        double c2 = velocity - (r * distance);
        return new _CriticalSolution__spring_simulation(r, c1, c2);
    }

    internal _CriticalSolution__spring_simulation(double r, double c1, double c2)
    {
        _r = r;
        _c1 = c1;
        _c2 = c2;
    }

    public virtual double x(double time)
    {
        return (_c1 + (_c2 * time)) * Dart_mathLibrary.pow(Dart_mathLibrary.e, _r * time);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double dx(double time)
    {
        var power = (double)Dart_mathLibrary.pow(Dart_mathLibrary.e, _r * time);
        return (_r * (_c1 + (_c2 * time)) * power) + (_c2 * power);
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

    internal static _OverdampedSolution__spring_simulation Create(
        SpringDescription spring,
        double distance,
        double velocity
    )
    {
        double cmk = (spring.damping * spring.damping) - (4L * spring.mass * spring.stiffness);
        double r1 = (-spring.damping - Dart_mathLibrary.sqrt(cmk)) / (2.0 * spring.mass);
        double r2 = (-spring.damping + Dart_mathLibrary.sqrt(cmk)) / (2.0 * spring.mass);
        double c2 = (velocity - (r1 * distance)) / (r2 - r1);
        double c1 = distance - c2;
        return new _OverdampedSolution__spring_simulation(r1, r2, c1, c2);
    }

    internal _OverdampedSolution__spring_simulation(double r1, double r2, double c1, double c2)
    {
        _r1 = r1;
        _r2 = r2;
        _c1 = c1;
        _c2 = c2;
    }

    public virtual double x(double time)
    {
        return (_c1 * Dart_mathLibrary.pow(Dart_mathLibrary.e, _r1 * time))
            + (_c2 * Dart_mathLibrary.pow(Dart_mathLibrary.e, _r2 * time));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double dx(double time)
    {
        return (_c1 * _r1 * Dart_mathLibrary.pow(Dart_mathLibrary.e, _r1 * time))
            + (_c2 * _r2 * Dart_mathLibrary.pow(Dart_mathLibrary.e, _r2 * time));
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

    internal static _UnderdampedSolution__spring_simulation Create(
        SpringDescription spring,
        double distance,
        double velocity
    )
    {
        double w =
            Dart_mathLibrary.sqrt(
                (4.0 * spring.mass * spring.stiffness) - (spring.damping * spring.damping)
            ) / (2.0 * spring.mass);
        double r = -(spring.damping / 2.0 / spring.mass);
        var c1 = distance;
        double c2 = (velocity - (r * distance)) / w;
        return new _UnderdampedSolution__spring_simulation(w, r, c1, c2);
    }

    internal _UnderdampedSolution__spring_simulation(double w, double r, double c1, double c2)
    {
        _w = w;
        _r = r;
        _c1 = c1;
        _c2 = c2;
    }

    public virtual double x(double time)
    {
        return (double)Dart_mathLibrary.pow(Dart_mathLibrary.e, _r * time)
            * ((_c1 * Dart_mathLibrary.cos(_w * time)) + (_c2 * Dart_mathLibrary.sin(_w * time)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double dx(double time)
    {
        var power = (double)Dart_mathLibrary.pow(Dart_mathLibrary.e, _r * time);
        double cosine = Dart_mathLibrary.cos(_w * time);
        double sine = Dart_mathLibrary.sin(_w * time);
        return (power * ((_c2 * _w * cosine) - (_c1 * _w * sine)))
            + (_r * power * ((_c2 * sine) + (_c1 * cosine)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SpringType type => SpringType.underDamped;
}
