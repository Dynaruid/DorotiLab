// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scroll_controller.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public delegate void ScrollControllerCallback(ScrollPosition position);

public class ScrollController : global::Doroti.Framework.Foundation.ChangeNotifier
{
    internal virtual double _initialScrollOffset { get; private set; } = default!;
    public virtual bool keepScrollOffset { get; private set; } = default!;
    public virtual global::System.Action<ScrollPosition>? onAttach { get; private set; }
    public virtual global::System.Action<ScrollPosition>? onDetach { get; private set; }
    public virtual string? debugLabel { get; private set; }
    internal virtual List<ScrollPosition> _positions { get; private set; } = new List<ScrollPosition>();

    public ScrollController(double initialScrollOffset = 0.0, bool keepScrollOffset = true, string? debugLabel = null, global::System.Action<ScrollPosition>? onAttach = null, global::System.Action<ScrollPosition>? onDetach = null)
    {
        this.keepScrollOffset = keepScrollOffset;
        this.debugLabel = debugLabel;
        this.onAttach = onAttach;
        this.onDetach = onDetach;
        this._initialScrollOffset = initialScrollOffset;
    }

    public virtual double initialScrollOffset => this._initialScrollOffset;
    public virtual IEnumerable<ScrollPosition> positions => DartRuntimePrimitives.ConvertValue<IEnumerable<ScrollPosition>>(this._positions);
    public virtual bool hasClients => Enumerable.Any(this._positions);
    public virtual ScrollPosition position
    {
        get
        {
            DartRuntimePrimitives.Assert(() => Enumerable.Any(this._positions), () => (object?)"ScrollController not attached to any scroll views.");
            DartRuntimePrimitives.Assert(() => (checked((long)(this._positions.Count)) == 1L), () => (object?)"ScrollController attached to multiple scroll views.");
            return this._positions.Single();
        }
    }
    public virtual double offset => ((ScrollPosition)this.position).pixels;
    public async virtual Future animateTo(double offset, Duration duration, global::Doroti.Framework.Animation.Curve curve)
    {
        DartRuntimePrimitives.Assert(() => Enumerable.Any(this._positions), () => (object?)"ScrollController not attached to any scroll views.");
        // Snapshot before starting an animation. A position may detach while a
        // sibling is animating, but Flutter's controller waits for every
        // position that was attached at invocation time.
        var futures = new List<Future>();
        foreach (var position in this._positions.ToArray())
        {
            futures.Add(position.animateTo(offset, duration: duration, curve: curve));
        }
        await DartAsyncRuntime.wait<object?>(futures);
    }

    public virtual void jumpTo(double value)
    {
        DartRuntimePrimitives.Assert(() => Enumerable.Any(this._positions), () => (object?)"ScrollController not attached to any scroll views.");
        foreach (var position in new List<ScrollPosition>(DartRuntimePrimitives.ConvertEnumerable<ScrollPosition>(this._positions)))
        {
            position.jumpTo(value);
        }
    }

    public virtual void attach(ScrollPosition position)
    {
        DartRuntimePrimitives.Assert(() => !this._positions.Contains(position));
        this._positions.Add(position);
        position.addListener(this.notifyListeners);
        this.onAttach?.Invoke(position);
    }

    public virtual void detach(ScrollPosition position)
    {
        DartRuntimePrimitives.Assert(() => this._positions.Contains(position));
        this.onDetach?.Invoke(position);
        position.removeListener(this.notifyListeners);
        this._positions.Remove(position);
    }

    public override void dispose()
    {
        foreach (ScrollPosition position in this._positions)
        {
            position.removeListener(this.notifyListeners);
        }
        base.dispose();
    }

    public virtual ScrollPosition createScrollPosition(ScrollPhysics physics, ScrollContext context, ScrollPosition? oldPosition)
    {
        return ((ScrollPosition)new ScrollPositionWithSingleContext(physics: physics, context: context, initialPixels: this.initialScrollOffset, keepScrollOffset: this.keepScrollOffset, oldPosition: oldPosition, debugLabel: this.debugLabel));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        var description = new List<string>();
        debugFillDescription(description);
        return $"{(DiagnosticsLibrary.describeIdentity(this))}({string.Join(", ", description)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillDescription(List<string> description)
    {
        if ((this.debugLabel is not null))
        {
            description.Add(this.debugLabel!);
        }
        if ((this.initialScrollOffset != 0.0))
        {
            description.Add($"initialScrollOffset: {this.initialScrollOffset.toStringAsFixed(1L)}, ");
        }
        if (!Enumerable.Any(this._positions))
        {
            description.Add("no clients");
        }
        else
        {
            if ((checked((long)(this._positions.Count)) == 1L))
            {
                description.Add($"one client, offset {this.offset.toStringAsFixed(1L)}");
            }
            else
            {
                description.Add($"{checked((long)(this._positions.Count))} clients");
            }
        }
    }

}

public class TrackingScrollController : ScrollController
{
    internal virtual DartMap<ScrollPosition, global::System.Action> _positionToListener { get; private set; } = new DartMap<ScrollPosition, global::System.Action>();
    internal virtual ScrollPosition? _lastUpdated { get; set; } = default;
    internal virtual double? _lastUpdatedOffset { get; set; } = default;

    public TrackingScrollController(double initialScrollOffset = 0.0, bool keepScrollOffset = true, string? debugLabel = null, global::System.Action<ScrollPosition>? onAttach = null, global::System.Action<ScrollPosition>? onDetach = null) : base(initialScrollOffset: initialScrollOffset, keepScrollOffset: keepScrollOffset, debugLabel: debugLabel, onAttach: onAttach, onDetach: onDetach)
    {
    }

    public virtual ScrollPosition? mostRecentlyUpdatedPosition => this._lastUpdated;
    public override double initialScrollOffset => DartRuntimePrimitives.ConvertValue<double>((this._lastUpdatedOffset ?? base.initialScrollOffset));
    public override void attach(ScrollPosition position)
    {
        base.attach(position);
        DartRuntimePrimitives.Assert(() => !this._positionToListener.ContainsKey(position));
        this._positionToListener[position] = (global::System.Action)(() =>
        {
            _lastUpdated = position;
            _lastUpdatedOffset = ((ScrollPosition)position).pixels;
        });
        position.addListener(this._positionToListener.GetValueOrDefault(position)!);
    }

    public override void detach(ScrollPosition position)
    {
        base.detach(position);
        DartRuntimePrimitives.Assert(() => this._positionToListener.ContainsKey(position));
        position.removeListener(this._positionToListener.GetValueOrDefault(position)!);
        this._positionToListener.remove(position);
        if ((Equals(this._lastUpdated, position)))
        {
            _lastUpdated = null;
        }
        if (!Enumerable.Any(this._positionToListener))
        {
            _lastUpdatedOffset = null;
        }
    }

    public override void dispose()
    {
        foreach (ScrollPosition position in this.positions)
        {
            DartRuntimePrimitives.Assert(() => this._positionToListener.ContainsKey(position));
            position.removeListener(this._positionToListener.GetValueOrDefault(position)!);
        }
        base.dispose();
    }

}
