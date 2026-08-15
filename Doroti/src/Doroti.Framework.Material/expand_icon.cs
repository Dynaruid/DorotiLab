// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/expand_icon.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class ExpandIcon : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual bool isExpanded { get; private set; } = default!;
    public virtual double size { get; private set; } = default!;
    public virtual global::System.Action<bool>? onPressed { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual Color? color { get; private set; }
    public virtual Color? disabledColor { get; private set; }
    public virtual Color? expandedColor { get; private set; }
    public virtual Color? splashColor { get; private set; }
    public virtual Color? highlightColor { get; private set; }

    public ExpandIcon(global::Doroti.Generated.Framework.Foundation.Key? key = null, bool isExpanded = false, double size = 24.0, global::System.Action<bool>? onPressed = default!, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding = default!, Color? color = null, Color? disabledColor = null, Color? expandedColor = null, Color? splashColor = null, Color? highlightColor = null) : base(key: key)
    {
        global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry __padding = padding ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(8.0);
        this.isExpanded = isExpanded;
        this.size = size;
        this.onPressed = onPressed;
        this.padding = __padding;
        this.color = color;
        this.disabledColor = disabledColor;
        this.expandedColor = expandedColor;
        this.splashColor = splashColor;
        this.highlightColor = highlightColor;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ExpandIconState__expand_icon());
}

internal class _ExpandIconState__expand_icon : global::Doroti.Generated.Framework.Widgets.State<ExpandIcon>, global::Doroti.Generated.Framework.Widgets.SingleTickerProviderStateMixin<ExpandIcon>
{
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController _controller { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<double> _iconTurns { get; set; } = default!;
    internal static global::Doroti.Generated.Framework.Animation.Animatable<double> _iconTurnTween = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 0.0, end: 0.5).chain(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn));
    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Generated.Framework.Animation.AnimationController(duration: ThemeLibrary.kThemeAnimationDuration, vsync: this);
        _iconTurns = this._controller.drive(_iconTurnTween);
        if (((ExpandIcon)this.widget).isExpanded)
        {
            this._controller.value = Dart_mathLibrary.pi;
        }
    }

    public override void dispose()
    {
        this._controller.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this._ticker is null) || !this._ticker!.isActive))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override void didUpdateWidget(ExpandIcon oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((ExpandIcon)this.widget).isExpanded != ((ExpandIcon)oldWidget).isExpanded))
        {
            if (((ExpandIcon)this.widget).isExpanded)
            {
                this._controller.forward();
            }
            else
            {
                this._controller.reverse();
            }
        }
    }

    internal virtual void _handlePressed()
    {
        ((ExpandIcon)this.widget).onPressed?.Invoke(((ExpandIcon)this.widget).isExpanded);
    }

    internal virtual global::Doroti.Ui.Color _iconColor
    {
        get
        {
            if ((((ExpandIcon)this.widget).isExpanded && (((ExpandIcon)this.widget).expandedColor is not null)))
            {
                return ((ExpandIcon)this.widget).expandedColor!;
            }
            if ((((ExpandIcon)this.widget).color is not null))
            {
                return ((ExpandIcon)this.widget).color!;
            }
            return (Theme.brightnessOf(this.context) switch { Brightness.light => Colors.black54, Brightness.dark => Colors.white60, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        MaterialLocalizations localizations__6314 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        string onTapHint__6382 = (((ExpandIcon)this.widget).isExpanded ? ((MaterialLocalizations)localizations__6314).expandedIconTapHint : ((MaterialLocalizations)localizations__6314).collapsedIconTapHint);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(onTapHint: ((((ExpandIcon)this.widget).onPressed is null) ? null : onTapHint__6382), child: new IconButton(padding: ((ExpandIcon)this.widget).padding, iconSize: ((ExpandIcon)this.widget).size, highlightColor: ((ExpandIcon)this.widget).highlightColor, splashColor: ((ExpandIcon)this.widget).splashColor, color: this._iconColor, disabledColor: ((ExpandIcon)this.widget).disabledColor, onPressed: ((global::System.Action)((((ExpandIcon)this.widget).onPressed is null) ? null : this._handlePressed)), icon: new global::Doroti.Generated.Framework.Widgets.RotationTransition(turns: this._iconTurns, child: new global::Doroti.Generated.Framework.Widgets.Icon(Icons.expand_more)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._ticker is null))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this.GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
            });
        this._ticker = new global::Doroti.Generated.Framework.Scheduler.Ticker((global::System.Action<Duration>)onTick, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
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
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__15400 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__15400, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        newNotifier__15400.addListener(() => this._updateTicker());
        this._tickerModeNotifier = newNotifier__15400;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription__15805 = ((this._ticker?.isActive, this._ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) });
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Scheduler.Ticker>("ticker", this._ticker, description: tickerDescription__15805, showSeparator: false, defaultValue: default));
    }

}
