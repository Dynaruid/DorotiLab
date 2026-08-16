// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/expansible.dart
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

namespace Doroti.Framework.Widgets;

public delegate Widget ExpansibleComponentBuilder(BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation);

public delegate Widget ExpansibleBuilder(BuildContext context, Widget header, Widget body, global::Doroti.Framework.Animation.Animation<double> animation);

public class ExpansibleController : global::Doroti.Framework.Foundation.ChangeNotifier
{
    internal virtual bool _isExpanded { get; set; } = false;

    public ExpansibleController()
    {
    }

    internal virtual void _setExpansionState(bool newValue)
    {
        if ((newValue != this._isExpanded))
        {
            _isExpanded = newValue;
            notifyListeners();
        }
    }

    public virtual bool isExpanded => this._isExpanded;
    public virtual void expand()
    {
        _setExpansionState(true);
    }

    public virtual void collapse()
    {
        _setExpansionState(false);
    }

    public virtual void toggle() => ((Action)(() => { if (this.isExpanded) { collapse(); } else { expand(); } }))();
    public static ExpansibleController of(BuildContext context)
    {
        _ExpansibleState__expansible? result__5929 = ((_ExpansibleState__expansible?)(object?)context.findAncestorStateOfType<_ExpansibleState__expansible>());
        DartRuntimePrimitives.Assert(() =>
            {
                if ((result__5929 is null))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("ExpansibleController.of() called with a context that does not contain a Expansible."), new global::Doroti.Framework.Foundation.ErrorDescription("No Expansible ancestor could be found starting from the context that was passed to ExpansibleController.of(). " + "This usually happens when the context provided is from the same StatefulWidget as that " + "whose build function actually creates the Expansible widget being sought."), new global::Doroti.Framework.Foundation.ErrorHint("There are several ways to avoid this problem. The simplest is to use a Builder to get a " + "context that is \"under\" the Expansible. "), new global::Doroti.Framework.Foundation.ErrorHint("A more efficient solution is to split your build function into several widgets. This " + "introduces a new context from which you can obtain the Expansible. In this solution, " + "you would have an outer widget that creates the Expansible populated by instances of " + "your new inner widgets, and then in these inner widgets you would use ExpansibleController.of().\n" + "An other solution is assign a GlobalKey to the Expansible, " + "then use the key.currentState property to obtain the Expansible rather than " + "using the ExpansibleController.of() function."), context.describeElement("The context used was") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result__5929!.widget.controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ExpansibleController? maybeOf(BuildContext context)
    {
        return context.findAncestorStateOfType<_ExpansibleState__expansible>()?.widget.controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class Expansible : StatefulWidget
{
    public virtual ExpansibleController controller { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget> headerBuilder { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget> bodyBuilder { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationStyle? animationStyle { get; private set; }
    public virtual Duration duration { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Curve curve { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Curve? reverseCurve { get; private set; }
    public virtual bool maintainState { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, Widget, Widget, global::Doroti.Framework.Animation.Animation<double>, Widget> expansibleBuilder { get; private set; } = default!;

    public Expansible(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget> headerBuilder = default!, global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget> bodyBuilder = default!, ExpansibleController controller = default!, global::System.Func<BuildContext, Widget, Widget, global::Doroti.Framework.Animation.Animation<double>, Widget> expansibleBuilder = default!, global::Doroti.Framework.Animation.AnimationStyle? animationStyle = null, Duration? duration = null, global::Doroti.Framework.Animation.Curve curve = default!, global::Doroti.Framework.Animation.Curve? reverseCurve = null, bool maintainState = true) : base(key: key)
    {
        global::System.Func<BuildContext, Widget, Widget, global::Doroti.Framework.Animation.Animation<double>, Widget> __expansibleBuilder = expansibleBuilder ?? _defaultExpansibleBuilder;
        Duration __duration = duration ?? Duration.Create(milliseconds: 200);
        global::Doroti.Framework.Animation.Curve __curve = curve ?? global::Doroti.Framework.Animation.Curves.ease;
        this.headerBuilder = headerBuilder;
        this.bodyBuilder = bodyBuilder;
        this.controller = controller;
        this.expansibleBuilder = __expansibleBuilder;
        this.animationStyle = animationStyle;
        this.duration = __duration;
        this.curve = __curve;
        this.reverseCurve = reverseCurve;
        this.maintainState = maintainState;
    }

    internal static Widget _defaultExpansibleBuilder(BuildContext context, Widget header, Widget body, global::Doroti.Framework.Animation.Animation<double> animation)
    {
        return ((Widget)(object?)new Column(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, children: new List<Widget> { header, body }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ExpansibleState__expansible());
}

internal class _ExpansibleState__expansible : State<Expansible>, SingleTickerProviderStateMixin<Expansible>
{
    internal virtual global::Doroti.Framework.Animation.AnimationController _animationController { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _heightFactor { get; set; } = default!;
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual Duration _duration
    {
        get
        {
            return (((Expansible)this.widget).animationStyle?.duration ?? ((Expansible)this.widget).duration);
            return default!;
        }
    }
    internal virtual global::Doroti.Framework.Animation.Curve _curve
    {
        get
        {
            return (((Expansible)this.widget).animationStyle?.curve ?? ((Expansible)this.widget).curve);
            return default!;
        }
    }
    internal virtual global::Doroti.Framework.Animation.Curve? _reverseCurve
    {
        get
        {
            return (((Expansible)this.widget).animationStyle?.reverseCurve ?? ((Expansible)this.widget).reverseCurve);
            return default!;
        }
    }
    public override void initState()
    {
        base.initState();
        _animationController = new global::Doroti.Framework.Animation.AnimationController(duration: this._duration, vsync: this);
        bool initiallyExpanded__14334 = (((bool?)PageStorage.maybeOf(this.context)?.readState(this.context)) ?? ((Expansible)this.widget).controller.isExpanded);
        if (initiallyExpanded__14334)
        {
            this._animationController.value = 1.0;
            ((Expansible)this.widget).controller.expand();
        }
        else
        {
            ((Expansible)this.widget).controller.collapse();
        }
        var heightFactorTween__14621 = new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: 1.0);
        _heightFactor = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._animationController.drive(heightFactorTween__14621), curve: this._curve, reverseCurve: this._reverseCurve);
        ((Expansible)this.widget).controller.addListener(() => this._toggleExpansion());
    }

    public override void didUpdateWidget(Expansible oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        Duration oldDuration__15023 = (((Expansible)oldWidget).animationStyle?.duration ?? ((Expansible)oldWidget).duration);
        global::Doroti.Framework.Animation.Curve oldCurve__15111 = (((Expansible)oldWidget).animationStyle?.curve ?? ((Expansible)oldWidget).curve);
        global::Doroti.Framework.Animation.Curve? oldReverseCurve__15191 = (((Expansible)oldWidget).animationStyle?.reverseCurve ?? ((Expansible)oldWidget).reverseCurve);
        if ((!object.Equals(this._curve, oldCurve__15111)))
        {
            this._heightFactor.curve = this._curve;
        }
        if ((!object.Equals(this._reverseCurve, oldReverseCurve__15191)))
        {
            this._heightFactor.reverseCurve = this._reverseCurve;
        }
        if ((!object.Equals(this._duration, oldDuration__15023)))
        {
            this._animationController.duration = this._duration;
        }
        if ((!object.Equals(((Expansible)this.widget).controller, ((Expansible)oldWidget).controller)))
        {
            ((Expansible)oldWidget).controller.removeListener(() => this._toggleExpansion());
            ((Expansible)this.widget).controller.addListener(() => this._toggleExpansion());
            if ((((Expansible)oldWidget).controller.isExpanded != ((Expansible)this.widget).controller.isExpanded))
            {
                _toggleExpansion();
            }
        }
    }

    public override void dispose()
    {
        ((Expansible)this.widget).controller.removeListener(() => this._toggleExpansion());
        this._animationController.dispose();
        this._heightFactor.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this._ticker is null) || !this._ticker!.isActive))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _toggleExpansion()
    {
        setState(((global::System.Action)(() => {
if (((Expansible)this.widget).controller.isExpanded)
{
    this._animationController.forward();
}
else
{
    DartRuntimePrimitives.Ignore(this._animationController.reverse().then(((global::System.Func<object?, object>)((value) => {
if (!this.mounted)
{
    return default!;
}
setState(((global::System.Action)(() => {
})));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
}
PageStorage.maybeOf(this.context)?.writeState(this.context, ((Expansible)this.widget).controller.isExpanded);
})));
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => (!this._animationController.isDismissed || !((Expansible)this.widget).controller.isExpanded));
        bool closed__16678 = (!((Expansible)this.widget).controller.isExpanded && this._animationController.isDismissed);
        bool shouldRemoveBody__16769 = (closed__16678 && !((Expansible)this.widget).maintainState);
        Widget result__16839 = ((Widget)(object?)new Offstage(offstage: closed__16678, child: new TickerMode(enabled: !closed__16678, child: this.widget.bodyBuilder(context, this._animationController))));
        return ((Widget)(object?)new AnimatedBuilder(animation: ((global::Doroti.Framework.Animation.AnimationController)this._animationController).view, builder: ((global::System.Func<BuildContext, Widget?, Widget>)((context, child) => {
Widget header__17139 = this.widget.headerBuilder(context, this._animationController);
Widget body__17222 = ((Widget)(object?)new ClipRect(child: new Align(heightFactor: ((global::Doroti.Framework.Animation.CurvedAnimation)this._heightFactor).value, child: child)));
return this.widget.expansibleBuilder(context, header__17139, body__17222, this._animationController);
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: (shouldRemoveBody__16769 ? null : result__16839)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._ticker is null))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this.GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._ticker = new global::Doroti.Framework.Scheduler.Ticker((global::System.Action<Duration>)onTick, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
        _updateTickerModeNotifier();
        _updateTicker();
        return this._ticker!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTicker();
    }

    public virtual void _updateTicker()
    {
        TickerModeData values__15157 = this._tickerModeNotifier!.value;
        if ((this._ticker is not null))
        {
            this._ticker!.muted = !((TickerModeData)values__15157).enabled;
            this._ticker!.forceFrames = ((TickerModeData)values__15157).forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__15400 = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__15400, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        newNotifier__15400.addListener(() => this._updateTicker());
        this._tickerModeNotifier = newNotifier__15400;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription__15805 = ((this._ticker?.isActive, this._ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) });
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", this._ticker, description: tickerDescription__15805, showSeparator: false, defaultValue: default));
    }

}

