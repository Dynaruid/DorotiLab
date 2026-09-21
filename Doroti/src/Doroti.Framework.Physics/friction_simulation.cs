// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/physics/friction_simulation.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Physics;

public static partial class Friction_simulationLibrary
{
    internal static double _newtonsMethod(
        double initialGuess,
        double target,
        Func<double, double> f,
        Func<double, double> df,
        long iterations
    )
    {
        var guess = initialGuess;
        for (var i = 0L; i < iterations; i++)
        {
            guess = guess - ((f(guess) - target) / df(guess));
        }
        return guess;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class FrictionSimulation : Simulation
{
    internal virtual double _drag { get; private set; } = default!;
    internal virtual double _dragLog { get; private set; } = default!;
    internal virtual double _x { get; private set; } = default!;
    internal virtual double _v { get; private set; } = default!;
    internal virtual double _constantDeceleration { get; private set; } = default!;
    internal virtual double _finalTime { get; set; } = double.PositiveInfinity;

    public FrictionSimulation(
        double drag,
        double position,
        double velocity,
        Tolerance tolerance = default!,
        double constantDeceleration = 0
    )
        : base(tolerance: tolerance ?? Tolerance.defaultTolerance)
    {
        _drag = drag;
        _dragLog = Dart_mathLibrary.log(drag);
        _x = position;
        _v = velocity;
        _constantDeceleration = constantDeceleration * Math.Sign(velocity);
    }

    public static FrictionSimulation CreateThrough(
        double startPosition,
        double endPosition,
        double startVelocity,
        double endVelocity
    )
    {
        DartRuntimePrimitives.Assert(() =>
            (startVelocity == 0.0)
            || (endVelocity == 0.0)
            || (Math.Sign(startVelocity) == Math.Sign(endVelocity))
        );
        DartRuntimePrimitives.Assert(() => startVelocity.abs() >= endVelocity.abs());
        DartRuntimePrimitives.Assert(() =>
            Math.Sign(endPosition - startPosition) == Math.Sign(startVelocity)
        );
        return new FrictionSimulation(
            _dragFor(startPosition, endPosition, startVelocity, endVelocity),
            startPosition,
            startVelocity,
            tolerance: new Tolerance(velocity: endVelocity.abs())
        );
    }

    internal static double _dragFor(
        double startPosition,
        double endPosition,
        double startVelocity,
        double endVelocity
    )
    {
        return (double)
            Dart_mathLibrary.pow(
                Dart_mathLibrary.e,
                (startVelocity - endVelocity) / (startPosition - endPosition)
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double x(double time)
    {
        if (time > _finalTime)
        {
            return finalX;
        }
        return _x
            + (_v * Dart_mathLibrary.pow(_drag, time) / _dragLog)
            - (_v / _dragLog)
            - (_constantDeceleration / 2L * time * time);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double dx(double time)
    {
        if (time > _finalTime)
        {
            return 0;
        }
        return (_v * Dart_mathLibrary.pow(_drag, time)) - (_constantDeceleration * time);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual double finalX
    {
        get
        {
            if (_constantDeceleration == 0L)
            {
                return _x - (_v / _dragLog);
            }
            return x(_finalTime);
        }
    }

    public virtual double timeAtX(double x)
    {
        if (x == _x)
        {
            return 0.0;
        }
        if ((_v == 0.0) || ((_v > 0L) ? ((x < _x) || (x > finalX)) : ((x > _x) || (x < finalX))))
        {
            return double.PositiveInfinity;
        }
        return Friction_simulationLibrary._newtonsMethod(
            target: x,
            initialGuess: 0,
            f: this.x,
            df: dx,
            iterations: 10L
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool isDone(double time)
    {
        return dx(time).abs() < tolerance.velocity;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override string ToString() =>
        $"{Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "FrictionSimulation")}(cₓ: {_drag.toStringAsFixed(1L)}, x₀: {_x.toStringAsFixed(1L)}, dx₀: {_v.toStringAsFixed(1L)})";
}

public class BoundedFrictionSimulation : FrictionSimulation
{
    internal virtual double _minX { get; private set; } = default!;
    internal virtual double _maxX { get; private set; } = default!;

    public BoundedFrictionSimulation(
        double drag,
        double position,
        double velocity,
        double _minX,
        double _maxX
    )
        : base(drag, position, velocity)
    {
        this._minX = _minX;
        this._maxX = _maxX;
        System.Diagnostics.Debug.Assert(
            DorotiUiLibrary.clampDouble(position, _minX, _maxX) == position
        );
    }

    public override double x(double time)
    {
        return DorotiUiLibrary.clampDouble(base.x(time), _minX, _maxX);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool isDone(double time)
    {
        return base.isDone(time)
            || ((x(time) - _minX).abs() < tolerance.distance)
            || ((x(time) - _maxX).abs() < tolerance.distance);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override string ToString() =>
        $"{Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "BoundedFrictionSimulation")}(cₓ: {_drag.toStringAsFixed(1L)}, x₀: {_x.toStringAsFixed(1L)}, dx₀: {_v.toStringAsFixed(1L)}, x: {_minX.toStringAsFixed(1L)}..{_maxX.toStringAsFixed(1L)})";
}
