// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/animated_switcher.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

internal class _ChildEntry__animated_switcher
{
    public virtual global::Doroti.Generated.Framework.Animation.AnimationController controller { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation animation { get; private set; } = default!;
    public virtual Widget transition { get; set; } = default!;
    public virtual Widget widgetChild { get; set; } = default!;

    internal _ChildEntry__animated_switcher(global::Doroti.Generated.Framework.Animation.AnimationController controller, global::Doroti.Generated.Framework.Animation.CurvedAnimation animation, Widget transition, Widget widgetChild)
    {
        this.controller = controller;
        this.animation = animation;
        this.transition = transition;
        this.widgetChild = widgetChild;
    }

    public override string ToString() => $"Entry#{(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.shortHash(this))}({this.widgetChild})";
}

public delegate Widget AnimatedSwitcherTransitionBuilder(Widget child, global::Doroti.Generated.Framework.Animation.Animation<double> animation);

public delegate Widget AnimatedSwitcherLayoutBuilder(Widget? currentChild, List<Widget> previousChildren);

public class AnimatedSwitcher : StatefulWidget
{
    public virtual Widget? child { get; private set; }
    public virtual Duration duration { get; private set; } = default!;
    public virtual Duration? reverseDuration { get; private set; }
    public virtual global::Doroti.Generated.Framework.Animation.Curve switchInCurve { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Curve switchOutCurve { get; private set; } = default!;
    public virtual global::System.Func<Widget, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget> transitionBuilder { get; private set; } = default!;
    public virtual global::System.Func<Widget?, List<Widget>, Widget> layoutBuilder { get; private set; } = default!;

    public AnimatedSwitcher(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? child = null, Duration duration = default!, Duration? reverseDuration = null, global::Doroti.Generated.Framework.Animation.Curve switchInCurve = default!, global::Doroti.Generated.Framework.Animation.Curve switchOutCurve = default!, global::System.Func<Widget, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget> transitionBuilder = default!, global::System.Func<Widget?, List<Widget>, Widget> layoutBuilder = default!) : base(key: key)
    {
        global::Doroti.Generated.Framework.Animation.Curve __switchInCurve = switchInCurve ?? global::Doroti.Generated.Framework.Animation.Curves.linear;
        global::Doroti.Generated.Framework.Animation.Curve __switchOutCurve = switchOutCurve ?? global::Doroti.Generated.Framework.Animation.Curves.linear;
        global::System.Func<Widget, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget> __transitionBuilder = transitionBuilder ?? AnimatedSwitcher.defaultTransitionBuilder;
        global::System.Func<Widget?, List<Widget>, Widget> __layoutBuilder = layoutBuilder ?? AnimatedSwitcher.defaultLayoutBuilder;
        this.child = child;
        this.duration = duration;
        this.reverseDuration = reverseDuration;
        this.switchInCurve = __switchInCurve;
        this.switchOutCurve = __switchOutCurve;
        this.transitionBuilder = __transitionBuilder;
        this.layoutBuilder = __layoutBuilder;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _AnimatedSwitcherState__animated_switcher());
    public static Widget defaultTransitionBuilder(Widget child, global::Doroti.Generated.Framework.Animation.Animation<double> animation)
    {
        return ((Widget)(object?)new FadeTransition(key: new global::Doroti.Generated.Framework.Foundation.ValueKey<global::Doroti.Generated.Framework.Foundation.Key?>(((Widget)child).key), opacity: animation, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Widget defaultLayoutBuilder(Widget? currentChild, List<Widget> previousChildren)
    {
        return ((Widget)(object?)new Stack(alignment: global::Doroti.Generated.Framework.Painting.Alignment.center, children: new List<Widget>()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.IntProperty("duration", this.duration.inMilliseconds, unit: "ms"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.IntProperty("reverseDuration", this.reverseDuration?.inMilliseconds, unit: "ms", defaultValue: null));
    }

}

internal class _AnimatedSwitcherState__animated_switcher : State<AnimatedSwitcher>, TickerProviderStateMixin<AnimatedSwitcher>
{
    internal virtual _ChildEntry__animated_switcher? _currentEntry { get; set; } = default;
    internal virtual HashSet<_ChildEntry__animated_switcher> _outgoingEntries { get; private set; } = new HashSet<_ChildEntry__animated_switcher>();
    internal virtual List<Widget>? _outgoingWidgets { get; set; } = new List<Widget>();
    internal virtual long _childNumber { get; set; } = 0L;
    public virtual HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _addEntryForNewChild(animate: false);
    }

    public override void didUpdateWidget(AnimatedSwitcher oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals((global::System.Func<Widget, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget>)((AnimatedSwitcher)this.widget).transitionBuilder, (global::System.Func<Widget, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget>)((AnimatedSwitcher)oldWidget).transitionBuilder)))
        {
            this._outgoingEntries.forEach((__arg0) => ((global::System.Action<_ChildEntry__animated_switcher>)this._updateTransitionForEntry)(__arg0));
            if ((this._currentEntry is not null))
            {
                _updateTransitionForEntry(this._currentEntry!);
            }
            _markChildWidgetCacheAsDirty();
        }
        var hasNewChild__11346 = (((AnimatedSwitcher)this.widget).child is not null);
        var hasOldChild__11392 = (this._currentEntry is not null);
        if (((hasNewChild__11346 != hasOldChild__11392) || (hasNewChild__11346 && !Widget.canUpdate(((AnimatedSwitcher)this.widget).child!, this._currentEntry!.widgetChild))))
        {
            _childNumber += 1L;
            _addEntryForNewChild(animate: true);
        }
        else
        {
            if ((this._currentEntry is not null))
            {
                DartRuntimePrimitives.Assert(() => (hasOldChild__11392 && hasNewChild__11346));
                DartRuntimePrimitives.Assert(() => Widget.canUpdate(((AnimatedSwitcher)this.widget).child!, this._currentEntry!.widgetChild));
                this._currentEntry!.widgetChild = ((AnimatedSwitcher)this.widget).child!;
                _updateTransitionForEntry(this._currentEntry!);
                _markChildWidgetCacheAsDirty();
            }
        }
    }

    internal virtual void _addEntryForNewChild(bool animate)
    {
        DartRuntimePrimitives.Assert(() => (animate || (this._currentEntry is null)));
        if ((this._currentEntry is not null))
        {
            DartRuntimePrimitives.Assert(() => animate);
            DartRuntimePrimitives.Assert(() => !this._outgoingEntries.Contains(this._currentEntry));
            this._outgoingEntries.Add(this._currentEntry!);
            this._currentEntry!.controller.reverse();
            _markChildWidgetCacheAsDirty();
            _currentEntry = null;
        }
        if ((((AnimatedSwitcher)this.widget).child is null))
        {
            return;
        }
        var controller__12745 = new global::Doroti.Generated.Framework.Animation.AnimationController(duration: ((AnimatedSwitcher)this.widget).duration, reverseDuration: ((AnimatedSwitcher)this.widget).reverseDuration, vsync: this);
        var animation__12895 = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: controller__12745, curve: ((AnimatedSwitcher)this.widget).switchInCurve, reverseCurve: ((AnimatedSwitcher)this.widget).switchOutCurve);
        _currentEntry = _newEntry(child: ((AnimatedSwitcher)this.widget).child!, controller: controller__12745, animation: animation__12895, builder: (global::System.Func<Widget, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget>)((AnimatedSwitcher)this.widget).transitionBuilder);
        if (animate)
        {
            controller__12745.forward();
        }
        else
        {
            DartRuntimePrimitives.Assert(() => !System.Linq.Enumerable.Any(this._outgoingEntries));
            controller__12745.value = 1.0;
        }
    }

    internal virtual _ChildEntry__animated_switcher _newEntry(Widget child, global::System.Func<Widget, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget> builder, global::Doroti.Generated.Framework.Animation.AnimationController controller, global::Doroti.Generated.Framework.Animation.CurvedAnimation animation)
    {
        var entry__13552 = new _ChildEntry__animated_switcher(widgetChild: child, transition: KeyedSubtree.CreateWrap(builder(child, animation), this._childNumber), animation: animation, controller: controller);
        animation.addStatusListener(((AnimationStatusListener)((status) => {
if (global::Doroti.Generated.Framework.Animation.AnimationStatusMembers.isDismissed(status))
{
    setState(((global::System.Action)(() => {
DartRuntimePrimitives.Assert(() => this.mounted);
DartRuntimePrimitives.Assert(() => this._outgoingEntries.Contains(entry__13552));
this._outgoingEntries.Remove(entry__13552);
_markChildWidgetCacheAsDirty();
})));
    controller.dispose();
    animation.dispose();
}
})));
        return entry__13552;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _markChildWidgetCacheAsDirty()
    {
        _outgoingWidgets = null;
    }

    internal virtual void _updateTransitionForEntry(_ChildEntry__animated_switcher entry)
    {
        entry.transition = DartRuntimePrimitives.ConvertValue<Widget>(new KeyedSubtree(key: ((_ChildEntry__animated_switcher)entry).transition.key, child: this.widget.transitionBuilder(((_ChildEntry__animated_switcher)entry).widgetChild, ((_ChildEntry__animated_switcher)entry).animation)));
    }

    internal virtual void _rebuildOutgoingWidgetsIfNeeded()
    {
        _outgoingWidgets ??= new List<Widget>(DartRuntimePrimitives.ConvertEnumerable<Widget>(this._outgoingEntries.map<_ChildEntry__animated_switcher, Widget>(((entry) => ((_ChildEntry__animated_switcher)entry).transition))));
        DartRuntimePrimitives.Assert(() => (checked((long)(this._outgoingEntries.Count)) == checked((long)(this._outgoingWidgets!.Count))));
        DartRuntimePrimitives.Assert(() => (!System.Linq.Enumerable.Any(this._outgoingEntries) || (object.Equals(this._outgoingEntries.Last().transition, this._outgoingWidgets!.Last()))));
    }

    public override void dispose()
    {
        this._currentEntry?.controller.dispose();
        this._currentEntry?.animation.dispose();
        foreach (_ChildEntry__animated_switcher entry__14901 in this._outgoingEntries)
        {
            ((_ChildEntry__animated_switcher)entry__14901).controller.dispose();
            ((_ChildEntry__animated_switcher)entry__14901).animation.dispose();
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Generated.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        _rebuildOutgoingWidgetsIfNeeded();
        return this.widget.layoutBuilder(this._currentEntry?.transition, this._outgoingWidgets!.where(((outgoing) => (!object.Equals(((Widget)outgoing).key, this._currentEntry?.transition.key)))).toSet().ToList());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>();
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<_WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Generated.Framework.Scheduler.Ticker)(object?)result__17553);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => (this._tickers is not null));
        DartRuntimePrimitives.Assert(() => this._tickers!.Contains(ticker));
        this._tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if ((this._tickers is not null))
        {
            TickerModeData values__18318 = this._tickerModeNotifier!.value;
            bool muted__18372 = !((TickerModeData)values__18318).enabled;
            foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

