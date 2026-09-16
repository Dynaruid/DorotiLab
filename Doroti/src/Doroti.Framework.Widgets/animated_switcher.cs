// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/animated_switcher.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

internal class _ChildEntry__animated_switcher
{
    public virtual AnimationController controller { get; private set; } = default!;
    public virtual CurvedAnimation animation { get; private set; } = default!;
    public virtual Widget transition { get; set; } = default!;
    public virtual Widget widgetChild { get; set; } = default!;

    internal _ChildEntry__animated_switcher(AnimationController controller, CurvedAnimation animation, Widget transition, Widget widgetChild)
    {
        this.controller = controller;
        this.animation = animation;
        this.transition = transition;
        this.widgetChild = widgetChild;
    }

    public override string ToString() => $"Entry#{DiagnosticsLibrary.shortHash(this)}({widgetChild})";
}

public delegate Widget AnimatedSwitcherTransitionBuilder(Widget child, Animation<double> animation);

public delegate Widget AnimatedSwitcherLayoutBuilder(Widget? currentChild, List<Widget> previousChildren);

public class AnimatedSwitcher : StatefulWidget
{
    public virtual Widget? child { get; private set; }
    public virtual Duration duration { get; private set; } = default!;
    public virtual Duration? reverseDuration { get; private set; }
    public virtual Curve switchInCurve { get; private set; } = default!;
    public virtual Curve switchOutCurve { get; private set; } = default!;
    public virtual Func<Widget, Animation<double>, Widget> transitionBuilder { get; private set; } = default!;
    public virtual Func<Widget?, List<Widget>, Widget> layoutBuilder { get; private set; } = default!;

    public AnimatedSwitcher(Key? key = null, Widget? child = null, Duration duration = default!, Duration? reverseDuration = null, Curve switchInCurve = default!, Curve switchOutCurve = default!, Func<Widget, Animation<double>, Widget> transitionBuilder = default!, Func<Widget?, List<Widget>, Widget> layoutBuilder = default!) : base(key: key)
    {
        Curve __switchInCurve = switchInCurve ?? Curves.linear;
        Curve __switchOutCurve = switchOutCurve ?? Curves.linear;
        Func<Widget, Animation<double>, Widget> __transitionBuilder = transitionBuilder ?? defaultTransitionBuilder;
        Func<Widget?, List<Widget>, Widget> __layoutBuilder = layoutBuilder ?? defaultLayoutBuilder;
        this.child = child;
        this.duration = duration;
        this.reverseDuration = reverseDuration;
        this.switchInCurve = __switchInCurve;
        this.switchOutCurve = __switchOutCurve;
        this.transitionBuilder = __transitionBuilder;
        this.layoutBuilder = __layoutBuilder;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _AnimatedSwitcherState__animated_switcher());
    public static Widget defaultTransitionBuilder(Widget child, Animation<double> animation)
    {
        return new FadeTransition(key: new ValueKey<Key?>(child.key), opacity: animation, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Widget defaultLayoutBuilder(Widget? currentChild, List<Widget> previousChildren)
    {
        return new Stack(alignment: Alignment.center, children: new List<Widget>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new IntProperty("duration", duration.inMilliseconds, unit: "ms"));
        properties.add(new IntProperty("reverseDuration", reverseDuration?.inMilliseconds, unit: "ms", defaultValue: null));
    }

}

internal class _AnimatedSwitcherState__animated_switcher : State<AnimatedSwitcher>, TickerProviderStateMixin<AnimatedSwitcher>
{
    internal virtual _ChildEntry__animated_switcher? _currentEntry { get; set; } = default;
    internal virtual HashSet<_ChildEntry__animated_switcher> _outgoingEntries { get; private set; } = new HashSet<_ChildEntry__animated_switcher>();
    internal virtual List<Widget>? _outgoingWidgets { get; set; } = new List<Widget>();
    internal virtual long _childNumber { get; set; } = 0L;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _addEntryForNewChild(animate: false);
    }

    public override void didUpdateWidget(AnimatedSwitcher oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.transitionBuilder, oldWidget.transitionBuilder))
        {
            _outgoingEntries.forEach((__arg0) => ((Action<_ChildEntry__animated_switcher>)_updateTransitionForEntry)(__arg0));
            if (_currentEntry is not null)
            {
                _updateTransitionForEntry(_currentEntry!);
            }
            _markChildWidgetCacheAsDirty();
        }
        var hasNewChild = widget.child is not null;
        var hasOldChild = _currentEntry is not null;
        if ((hasNewChild != hasOldChild) || (hasNewChild && !Widget.canUpdate(widget.child!, _currentEntry!.widgetChild)))
        {
            _childNumber += 1L;
            _addEntryForNewChild(animate: true);
        }
        else
        {
            if (_currentEntry is not null)
            {
                DartRuntimePrimitives.Assert(() => hasOldChild && hasNewChild);
                DartRuntimePrimitives.Assert(() => Widget.canUpdate(widget.child!, _currentEntry!.widgetChild));
                _currentEntry!.widgetChild = widget.child!;
                _updateTransitionForEntry(_currentEntry!);
                _markChildWidgetCacheAsDirty();
            }
        }
    }

    internal virtual void _addEntryForNewChild(bool animate)
    {
        DartRuntimePrimitives.Assert(() => animate || (_currentEntry is null));
        if (_currentEntry is not null)
        {
            DartRuntimePrimitives.Assert(() => animate);
            DartRuntimePrimitives.Assert(() => !_outgoingEntries.Contains(_currentEntry));
            _outgoingEntries.Add(_currentEntry!);
            _currentEntry!.controller.reverse();
            _markChildWidgetCacheAsDirty();
            _currentEntry = null;
        }
        if (widget.child is null)
        {
            return;
        }
        var controllerLocal = new AnimationController(duration: widget.duration, reverseDuration: widget.reverseDuration, vsync: this);
        var animationLocal = new CurvedAnimation(parent: controllerLocal, curve: widget.switchInCurve, reverseCurve: widget.switchOutCurve);
        _currentEntry = _newEntry(child: widget.child!, controller: controllerLocal, animation: animationLocal, builder: widget.transitionBuilder);
        if (animate)
        {
            controllerLocal.forward();
        }
        else
        {
            DartRuntimePrimitives.Assert(() => !Enumerable.Any(_outgoingEntries));
            controllerLocal.value = 1.0;
        }
    }

    internal virtual _ChildEntry__animated_switcher _newEntry(Widget child, Func<Widget, Animation<double>, Widget> builder, AnimationController controller, CurvedAnimation animation)
    {
        var entry = new _ChildEntry__animated_switcher(widgetChild: child, transition: KeyedSubtree.CreateWrap(builder(child, animation), _childNumber), animation: animation, controller: controller);
        animation.addStatusListener((status) =>
        {
            if (AnimationStatusMembers.isDismissed(status))
            {
                setState(() =>
                {
                    DartRuntimePrimitives.Assert(() => mounted);
                    DartRuntimePrimitives.Assert(() => _outgoingEntries.Contains(entry));
                    _outgoingEntries.Remove(entry);
                    _markChildWidgetCacheAsDirty();
                });
                controller.dispose();
                animation.dispose();
            }
        });
        return entry;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _markChildWidgetCacheAsDirty()
    {
        _outgoingWidgets = null;
    }

    internal virtual void _updateTransitionForEntry(_ChildEntry__animated_switcher entry)
    {
        entry.transition = DartRuntimePrimitives.ConvertValue<Widget>(new KeyedSubtree(key: entry.transition.key, child: widget.transitionBuilder(entry.widgetChild, entry.animation)));
    }

    internal virtual void _rebuildOutgoingWidgetsIfNeeded()
    {
        _outgoingWidgets ??= new List<Widget>(DartRuntimePrimitives.ConvertEnumerable<Widget>(_outgoingEntries.map((entry) => entry.transition)));
        DartRuntimePrimitives.Assert(() => checked(_outgoingEntries.Count) == checked((long)_outgoingWidgets!.Count));
        DartRuntimePrimitives.Assert(() => !Enumerable.Any(_outgoingEntries) || Equals(_outgoingEntries.Last().transition, _outgoingWidgets!.Last()));
    }

    public override void dispose()
    {
        _currentEntry?.controller.dispose();
        _currentEntry?.animation.dispose();
        foreach (_ChildEntry__animated_switcher entry in _outgoingEntries)
        {
            entry.controller.dispose();
            entry.animation.dispose();
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if (_tickers is not null)
                {
                    foreach (Scheduler.Ticker ticker in _tickers!)
                    {
                        if (ticker.isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{this} was disposed with an active Ticker."), new ErrorDescription($"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        _rebuildOutgoingWidgetsIfNeeded();
        return widget.layoutBuilder(_currentEntry?.transition, _outgoingWidgets!.where((outgoing) => !Equals(outgoing.key, _currentEntry?.transition.key)).toSet().ToList());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = ((Func<_WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider(onTick, this, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
    __cascade.muted = !values.enabled;
    __cascade.forceFrames = values.forceFrames;
    return __cascade;
}))();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => _tickers is not null);
        DartRuntimePrimitives.Assert(() => _tickers!.Contains(ticker));
        _tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if (_tickers is not null)
        {
            TickerModeData values = _tickerModeNotifier!.value;
            bool mutedLocal = !values.enabled;
            foreach (Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<HashSet<Scheduler.Ticker>>("tickers", _tickers, description: (_tickers is not null) ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}" : null, defaultValue: default));
    }

}

