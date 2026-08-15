// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/animation/tween_sequence.dart
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

namespace Doroti.Generated.Framework.Animation;

public class TweenSequence<T> : Animatable<T>
{
    internal virtual List<TweenSequenceItem<T>> _items { get; private set; } = new List<TweenSequenceItem<T>>();
    internal virtual List<_Interval__tween_sequence> _intervals { get; private set; } = new List<_Interval__tween_sequence>();

    public TweenSequence(List<TweenSequenceItem<T>> items)
    {
        System.Diagnostics.Debug.Assert((checked((long)(items.Count)) != 0));
        _items.AddRange(items);
        var totalWeight = _items.Sum(item => item.weight);
        System.Diagnostics.Debug.Assert(totalWeight > 0.0);
        var start = 0.0;
        for (var index = 0; index < _items.Count; index++)
        {
            var end = index == _items.Count - 1 ? 1.0 : start + (_items[index].weight / totalWeight);
            _intervals.Add(new _Interval__tween_sequence(start, end));
            start = end;
        }
    }

    internal virtual T _evaluateAt(double t, long index)
    {
        TweenSequenceItem<T> element__2527 = this._items[(int)(index)];
        double tInterval__2569 = this._intervals[(int)(index)].value(t);
        return ((TweenSequenceItem<T>)element__2527).tween.transform(tInterval__2569);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override T transform(double t)
    {
        DartRuntimePrimitives.Assert(() => ((t >= 0.0) && (t <= 1.0)));
        if ((t == 1.0))
        {
            return _evaluateAt(t, (checked((long)(this._items.Count)) - 1L));
        }
        for (var index__2820 = 0L; (index__2820 < checked((long)(this._items.Count))); index__2820++)
        {
            if (this._intervals[(int)(index__2820)].contains(t))
            {
                return _evaluateAt(t, index__2820);
            }
        }
        throw new InvalidOperationException($"TweenSequence.evaluate() could not find an interval for {t}");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"TweenSequence({checked((long)(this._items.Count))} items)";
}

public class FlippedTweenSequence : TweenSequence<double>
{
    public FlippedTweenSequence(List<TweenSequenceItem<double>> items) : base(items)
    {
    }

    public override double transform(double t) => (1L - base.transform((1L - t)));
}

public class TweenSequenceItem<T>
{
    public virtual Animatable<T> tween { get; private set; } = default!;
    public virtual double weight { get; private set; } = default!;

    public TweenSequenceItem(Animatable<T> tween, double weight)
    {
        this.tween = tween;
        this.weight = weight;
        System.Diagnostics.Debug.Assert((weight > 0.0));
    }

}

internal class _Interval__tween_sequence
{
    public virtual double start { get; private set; } = default!;
    public virtual double end { get; private set; } = default!;

    internal _Interval__tween_sequence(double start, double end)
    {
        this.start = start;
        this.end = end;
        System.Diagnostics.Debug.Assert((end > start));
    }

    public virtual bool contains(double t) => ((t >= this.start) && (t < this.end));
    public virtual double value(double t) => (((t - this.start)) / ((this.end - this.start)));
    public override string ToString() => $"<{this.start}, {this.end}>";
}
