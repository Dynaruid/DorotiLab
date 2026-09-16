// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/expand_icon.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class ExpandIcon : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual bool isExpanded { get; private set; } = default!;
    public virtual double size { get; private set; } = default!;
    public virtual global::System.Action<bool>? onPressed { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual Color? color { get; private set; }
    public virtual Color? disabledColor { get; private set; }
    public virtual Color? expandedColor { get; private set; }
    public virtual Color? splashColor { get; private set; }
    public virtual Color? highlightColor { get; private set; }

    public ExpandIcon(global::Doroti.Framework.Foundation.Key? key = null, bool isExpanded = false, double size = 24.0, global::System.Action<bool>? onPressed = default!, global::Doroti.Framework.Painting.EdgeInsetsGeometry padding = default!, Color? color = null, Color? disabledColor = null, Color? expandedColor = null, Color? splashColor = null, Color? highlightColor = null) : base(key: key)
    {
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __padding = padding ?? EdgeInsets.CreateAll(8.0);
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

internal class _ExpandIconState__expand_icon : global::Doroti.Framework.Widgets.State<ExpandIcon>, global::Doroti.Framework.Widgets.SingleTickerProviderStateMixin<ExpandIcon>
{
    internal virtual global::Doroti.Framework.Animation.AnimationController _controller { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _iconTurns { get; set; } = default!;
    internal static global::Doroti.Framework.Animation.Animatable<double> _iconTurnTween = new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: 0.5).chain(new global::Doroti.Framework.Animation.CurveTween(curve: Curves.fastOutSlowIn));
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Framework.Animation.AnimationController(duration: ThemeLibrary.kThemeAnimationDuration, vsync: this);
        _iconTurns = _controller.drive(_iconTurnTween);
        if (widget.isExpanded)
        {
            _controller.value = Dart_mathLibrary.pi;
        }
    }

    public override void dispose()
    {
        _controller.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((_ticker is null) || !_ticker!.isActive)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), _ticker!.describeForError("The offending ticker was") }));
            });
        _tickerModeNotifier?.removeListener(_updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override void didUpdateWidget(ExpandIcon oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (widget.isExpanded != oldWidget.isExpanded)
        {
            if (widget.isExpanded)
            {
                _controller.forward();
            }
            else
            {
                _controller.reverse();
            }
        }
    }

    internal virtual void _handlePressed()
    {
        widget.onPressed?.Invoke(widget.isExpanded);
    }

    internal virtual global::Doroti.Ui.Color _iconColor
    {
        get
        {
            if (widget.isExpanded && (widget.expandedColor is not null))
            {
                return widget.expandedColor!;
            }
            if (widget.color is not null)
            {
                return widget.color!;
            }
            return Theme.brightnessOf(context) switch { Brightness.light => Colors.black54, Brightness.dark => Colors.white60, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        }
    }
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        string onTapHintLocal = widget.isExpanded ? localizations.expandedIconTapHint : localizations.collapsedIconTapHint;
        return new global::Doroti.Framework.Widgets.Semantics(onTapHint: (widget.onPressed is null) ? null : onTapHintLocal, child: new IconButton(padding: widget.padding, iconSize: widget.size, highlightColor: widget.highlightColor, splashColor: widget.splashColor, color: _iconColor, disabledColor: widget.disabledColor, onPressed: (widget.onPressed is null) ? null : _handlePressed, icon: new global::Doroti.Framework.Widgets.RotationTransition(turns: _iconTurns, child: new global::Doroti.Framework.Widgets.Icon(Icons.expand_more))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (_ticker is null)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
            });
        _ticker = new global::Doroti.Framework.Scheduler.Ticker(onTick, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
        _updateTickerModeNotifier();
        _updateTicker();
        return _ticker!;
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
        TickerModeData values = _tickerModeNotifier!.value;
        if (_ticker is not null)
        {
            _ticker!.muted = !values.enabled;
            _ticker!.forceFrames = values.forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTicker);
        newNotifier.addListener(_updateTicker);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) };
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", _ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
    }

}
