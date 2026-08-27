// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/physics/friction_simulation.dart
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

public static partial class Friction_simulationLibrary
{
    internal static double _newtonsMethod(double initialGuess, double target, Func<double, double> f, Func<double, double> df, long iterations)
    {
        var guess = initialGuess;
        for (var i = 0L; (i < iterations); i++)
        {
            guess = (guess - (((f(guess) - target)) / df(guess)));
        }
        return guess;
        throw new InvalidOperationException("Dart control flow completed without a value.");
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

    public FrictionSimulation(double drag, double position, double velocity, Tolerance tolerance = default!, double constantDeceleration = 0) : base(tolerance: tolerance ?? Tolerance.defaultTolerance)
    {
        this._drag = drag;
        this._dragLog = global::Doroti.Runtime.Dart_mathLibrary.log(drag);
        this._x = position;
        this._v = velocity;
        this._constantDeceleration = (constantDeceleration * Math.Sign(velocity));
    }

    public static FrictionSimulation CreateThrough(double startPosition, double endPosition, double startVelocity, double endVelocity)
    {
        DartRuntimePrimitives.Assert(() => (((startVelocity == 0.0) || (endVelocity == 0.0)) || (Math.Sign(startVelocity) == Math.Sign(endVelocity))));
        DartRuntimePrimitives.Assert(() => (startVelocity.abs() >= endVelocity.abs()));
        DartRuntimePrimitives.Assert(() => (Math.Sign(((endPosition - startPosition))) == Math.Sign(startVelocity)));
        return new FrictionSimulation(_dragFor(startPosition, endPosition, startVelocity, endVelocity), startPosition, startVelocity, tolerance: new Tolerance(velocity: endVelocity.abs()));
    }

    internal static double _dragFor(double startPosition, double endPosition, double startVelocity, double endVelocity)
    {
        return ((double)global::Doroti.Runtime.Dart_mathLibrary.pow(global::Doroti.Runtime.Dart_mathLibrary.e, (((startVelocity - endVelocity)) / ((startPosition - endPosition)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double x(double time)
    {
        if ((time > this._finalTime))
        {
            return this.finalX;
        }
        return (((this._x + ((this._v * global::Doroti.Runtime.Dart_mathLibrary.pow(this._drag, time)) / this._dragLog)) - (this._v / this._dragLog)) - (((((this._constantDeceleration / 2L)) * time) * time)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double dx(double time)
    {
        if ((time > this._finalTime))
        {
            return 0;
        }
        return ((this._v * global::Doroti.Runtime.Dart_mathLibrary.pow(this._drag, time)) - (this._constantDeceleration * time));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double finalX
    {
        get
        {
            if ((this._constantDeceleration == 0L))
            {
                return (this._x - (this._v / this._dragLog));
            }
            return x(this._finalTime);
            return default!;
        }
    }
    public virtual double timeAtX(double x)
    {
        if ((x == this._x))
        {
            return 0.0;
        }
        if (((this._v == 0.0) || (((this._v > 0L) ? (((x < this._x) || (x > this.finalX))) : (((x > this._x) || (x < this.finalX)))))))
        {
            return double.PositiveInfinity;
        }
        return Friction_simulationLibrary._newtonsMethod(target: x, initialGuess: 0, f: this.x, df: this.dx, iterations: 10L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isDone(double time)
    {
        return (dx(time).abs() < ((Tolerance)tolerance).velocity);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "FrictionSimulation"))}(cₓ: {this._drag.toStringAsFixed(1L)}, x₀: {this._x.toStringAsFixed(1L)}, dx₀: {this._v.toStringAsFixed(1L)})";
}

public class BoundedFrictionSimulation : FrictionSimulation
{
    internal virtual double _minX { get; private set; } = default!;
    internal virtual double _maxX { get; private set; } = default!;

    public BoundedFrictionSimulation(double drag, double position, double velocity, double _minX, double _maxX) : base(drag, position, velocity)
    {
        this._minX = _minX;
        this._maxX = _maxX;
        System.Diagnostics.Debug.Assert((Dart_uiLibrary.clampDouble(position, _minX, _maxX) == position));
    }

    public override double x(double time)
    {
        return Dart_uiLibrary.clampDouble(base.x(time), this._minX, this._maxX);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isDone(double time)
    {
        return ((base.isDone(time) || (((x(time) - this._minX)).abs() < ((Tolerance)tolerance).distance)) || (((x(time) - this._maxX)).abs() < ((Tolerance)tolerance).distance));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "BoundedFrictionSimulation"))}(cₓ: {_drag.toStringAsFixed(1L)}, x₀: {_x.toStringAsFixed(1L)}, dx₀: {_v.toStringAsFixed(1L)}, x: {this._minX.toStringAsFixed(1L)}..{this._maxX.toStringAsFixed(1L)})";
}

