// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/raw_radio.dart
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

public delegate Widget RadioBuilder(BuildContext context, dynamic state);

public class RawRadio<T> : StatefulWidget
{
    public virtual T value { get; private set; } = default!;
    public virtual WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor> mouseCursor { get; private set; } = default!;
    public virtual bool toggleable { get; private set; } = default!;
    public virtual FocusNode focusNode { get; private set; } = default!;
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, dynamic, Widget> builder { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;
    public virtual RadioGroupRegistry<T>? groupRegistry { get; private set; }

    public RawRadio(global::Doroti.Framework.Foundation.Key? key = null, T value = default!, WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor> mouseCursor = default!, bool toggleable = default!, FocusNode focusNode = default!, bool autofocus = default!, RadioGroupRegistry<T>? groupRegistry = default!, bool enabled = default!, global::System.Func<BuildContext, dynamic, Widget> builder = default!) : base(key: key)
    {
        this.value = value;
        this.mouseCursor = mouseCursor;
        this.toggleable = toggleable;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.groupRegistry = groupRegistry;
        this.enabled = enabled;
        this.builder = builder;
        System.Diagnostics.Debug.Assert((!enabled || (groupRegistry is not null)));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _RawRadioState__raw_radio<T>());
}

public class _RawRadioState__raw_radio<T> : State<RawRadio<T>>, TickerProviderStateMixin<RawRadio<T>>, ToggleableStateMixin<RawRadio<T>>, RadioClient<T>
{
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;
    public virtual global::Doroti.Framework.Animation.AnimationController _positionController { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation _position { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationController _reactionController { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation _reaction { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation _reactionHoverFade { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationController _reactionHoverFadeController { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation _reactionFocusFade { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationController _reactionFocusFadeController { get; set; } = default!;
    public virtual Duration _reactionAnimationDuration { get; set; } = Duration.Create(milliseconds: 100L);
    private bool __late__actionMap_initialized;
    private DartMap<Type, dynamic> __late__actionMap = default!;
    public virtual DartMap<Type, dynamic> _actionMap
    {
        get
        {
            if (!__late__actionMap_initialized)
            {
                __late__actionMap = new DartMap<Type, dynamic> { [typeof(ActivateIntent)] = new CallbackAction<ActivateIntent>(onInvoke: (__arg0) => { ((global::System.Action<Intent?>)this._handleTap)(__arg0); return default!; }) };
                __late__actionMap_initialized = true;
            }
            return __late__actionMap;
        }
    }
    public virtual Offset? _downPosition { get; set; } = default;
    public virtual bool _focused { get; set; } = false;
    public virtual bool _hovering { get; set; } = false;
    public virtual RadioGroupRegistry<T>? _registry { get; set; } = default;

    public virtual FocusNode focusNode => ((RawRadio<T>)(object)this.widget).focusNode;
    public virtual bool enabled => this.isInteractive;
    public virtual T radioValue => ((RawRadio<T>)(object)this.widget).value;
    public override void initState()
    {
        registry = ((RawRadio<T>)(object)this.widget).groupRegistry;
        base.initState();
        _positionController = new global::Doroti.Framework.Animation.AnimationController(duration: global::Doroti.Framework.Widgets.ToggleableLibrary._kToggleDuration, value: ((this.value == false) ? 0.0 : 1.0), vsync: this);
        _position = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._positionController, curve: global::Doroti.Framework.Animation.Curves.easeIn, reverseCurve: global::Doroti.Framework.Animation.Curves.easeOut);
        _reactionController = new global::Doroti.Framework.Animation.AnimationController(duration: this._reactionAnimationDuration, vsync: this);
        _reaction = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._reactionController, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
        _reactionHoverFadeController = new global::Doroti.Framework.Animation.AnimationController(duration: global::Doroti.Framework.Widgets.ToggleableLibrary._kReactionFadeDuration, value: ((this._hovering || this._focused) ? 1.0 : 0.0), vsync: this);
        _reactionHoverFade = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._reactionHoverFadeController, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
        _reactionFocusFadeController = new global::Doroti.Framework.Animation.AnimationController(duration: global::Doroti.Framework.Widgets.ToggleableLibrary._kReactionFadeDuration, value: ((this._hovering || this._focused) ? 1.0 : 0.0), vsync: this);
        _reactionFocusFade = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._reactionFocusFadeController, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
    }

    internal virtual void _handleChanged(bool? selected)
    {
        DartRuntimePrimitives.Assert(() => (this.registry is not null));
        if (!((selected ?? true)))
        {
            return;
        }
        if ((selected ?? false))
        {
            this.registry!.onChanged(((RawRadio<T>)(object)this.widget).value);
        }
        else
        {
            this.registry!.onChanged(default);
        }
    }

    public override void didUpdateWidget(RawRadio<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        registry = ((RawRadio<T>)(object)this.widget).groupRegistry;
        animateToValue();
    }

    public override void dispose()
    {
        this._positionController.dispose();
        this._position.dispose();
        this._reactionController.dispose();
        this._reaction.dispose();
        this._reactionHoverFadeController.dispose();
        this._reactionHoverFade.dispose();
        this._reactionFocusFadeController.dispose();
        this._reactionFocusFade.dispose();
        base.dispose();
        registry = null;
    }

    public virtual global::System.Action<bool?>? onChanged => ((global::System.Action<bool?>)((this.registry is not null) ? this._handleChanged : null));
    public virtual bool tristate => ((RawRadio<T>)(object)this.widget).toggleable;
    public virtual bool? value => DartRuntimePrimitives.ConvertValue<bool>(EqualityComparer<T>.Default.Equals(((RawRadio<T>)(object)this.widget).value, DartRuntimePrimitives.NullAware(this.registry, __target => __target.groupValue)));
    public virtual bool isInteractive => ((RawRadio<T>)(object)this.widget).enabled;
    public override Widget build(BuildContext context)
    {
        bool? accessibilitySelected = default!;
        string? semanticsHint = default!;
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    accessibilitySelected = null;
                    semanticsHint = null;
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    accessibilitySelected = this.value;
                    if (!((this.value ?? false)))
                    {
                        WidgetsLocalizations localizations = ((WidgetsLocalizations)(object?)WidgetsLocalizations.of(context));
                        semanticsHint = ((WidgetsLocalizations)localizations).radioButtonUnselectedLabel;
                    }
                    break;
                }
        }
        return ((Widget)(object?)new Semantics(inMutuallyExclusiveGroup: true, @checked: this.value, selected: accessibilitySelected, hint: semanticsHint, child: buildToggleableWithChild(focusNode: this.focusNode, autofocus: ((RawRadio<T>)(object)this.widget).autofocus, mouseCursor: ((RawRadio<T>)(object)this.widget).mouseCursor, child: this.widget.builder(context, this))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = this._tickerModeNotifier!.value;
        var result = ((Func<_WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values).enabled;
    __cascade.forceFrames = ((TickerModeData)values).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result);
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
            TickerModeData values = this._tickerModeNotifier!.value;
            bool mutedLocal = !((TickerModeData)values).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = ((TickerModeData)values).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

    public virtual global::Doroti.Framework.Animation.AnimationController positionController => this._positionController;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation position => this._position;
    public virtual global::Doroti.Framework.Animation.AnimationController reactionController => this._reactionController;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation reaction => this._reaction;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation reactionHoverFade => this._reactionHoverFade;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation reactionFocusFade => this._reactionFocusFade;
    public virtual Duration? reactionAnimationDuration => this._reactionAnimationDuration;
    public virtual void animateToValue()
    {
        if (this.tristate)
        {
            if ((this.value is null))
            {
                this._positionController.value = 0.0;
            }
            if ((this.value ?? true))
            {
                this._positionController.forward();
            }
            else
            {
                this._positionController.reverse();
            }
        }
        else
        {
            if ((this.value ?? false))
            {
                this._positionController.forward();
            }
            else
            {
                this._positionController.reverse();
            }
        }
    }

    public virtual Offset? downPosition => this._downPosition;
    public virtual void _handleTapDown(global::Doroti.Framework.Gestures.TapDownDetails details)
    {
        if (this.isInteractive)
        {
            setState(((global::System.Action)(() =>
            {
                this._downPosition = ((global::Doroti.Framework.Gestures.TapDownDetails)details).localPosition;
            })));
            this._reactionController.forward();
        }
    }

    public virtual void _handleTap(Intent? __unused0 = null)
    {
        if (!this.isInteractive)
        {
            return;
        }
        switch (this.value)
        {
            case false:
                {
                    this.onChanged!(true);
                    break;
                }
            case true:
                {
                    this.onChanged!((this.tristate ? null : false));
                    break;
                }
            case null:
                {
                    this.onChanged!(false);
                    break;
                }
        }
        ((dynamic)this.context.findRenderObject()!).sendSemanticsEvent(new global::Doroti.Framework.Semantics.TapSemanticEvent());
    }

    public virtual void _handleTapEnd(global::Doroti.Framework.Gestures.TapUpDetails? __unused0 = null)
    {
        if ((this._downPosition is not null))
        {
            setState(((global::System.Action)(() =>
            {
                this._downPosition = null;
            })));
        }
        this._reactionController.reverse();
    }

    public virtual void _handleFocusHighlightChanged(bool focused)
    {
        if ((focused != this._focused))
        {
            setState(((global::System.Action)(() =>
            {
                this._focused = focused;
            })));
            if (focused)
            {
                this._reactionFocusFadeController.forward();
            }
            else
            {
                this._reactionFocusFadeController.reverse();
            }
        }
    }

    public virtual void _handleHoverChanged(bool hovering)
    {
        if ((hovering != this._hovering))
        {
            setState(((global::System.Action)(() =>
            {
                this._hovering = hovering;
            })));
            if (hovering)
            {
                this._reactionHoverFadeController.forward();
            }
            else
            {
                this._reactionHoverFadeController.reverse();
            }
        }
    }

    public virtual HashSet<WidgetState> states => new HashSet<WidgetState>();
    public virtual Widget buildToggleable(FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor>? mouseCursor = null, Size size = default!, global::Doroti.Framework.Rendering.CustomPainter painter = default!)
    {
        return ((Widget)(object?)buildToggleableWithChild(focusNode: focusNode, onFocusChange: (global::System.Action<bool>?)onFocusChange, autofocus: autofocus, mouseCursor: mouseCursor, child: new CustomPaint(size: size, painter: painter)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Widget buildToggleable(FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor>? mouseCursor = null, Size size = default!, ToggleablePainter painter = default!)
    {
        return buildToggleableWithChild(focusNode: focusNode, onFocusChange: onFocusChange, autofocus: autofocus, mouseCursor: mouseCursor, child: new SizedBox(width: size.width, height: size.height));
    }

    public virtual Widget buildToggleableWithChild(FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor>? mouseCursor = null, Widget child = default!)
    {
        return ((Widget)(object?)new FocusableActionDetector(actions: this._actionMap, focusNode: focusNode, autofocus: autofocus, onFocusChange: (global::System.Action<bool>?)onFocusChange, enabled: this.isInteractive, onShowFocusHighlight: (global::System.Action<bool>)this._handleFocusHighlightChanged, onShowHoverHighlight: (global::System.Action<bool>)this._handleHoverChanged, mouseCursor: (mouseCursor?.resolve(this.states) ?? global::Doroti.Framework.Services.SystemMouseCursors.basic), child: new GestureDetector(excludeFromSemantics: !this.isInteractive, onTapDown: ((global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>)(this.isInteractive ? this._handleTapDown : null)), onTap: () => ((global::System.Action<Intent?>)(this.isInteractive ? this._handleTap : null))(default), onTapUp: ((global::System.Action<global::Doroti.Framework.Gestures.TapUpDetails?>)(this.isInteractive ? this._handleTapEnd : null)), onTapCancel: () => ((global::System.Action<global::Doroti.Framework.Gestures.TapUpDetails?>)(this.isInteractive ? this._handleTapEnd : null))(default), child: new Semantics(enabled: this.isInteractive, child: child))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RadioGroupRegistry<T>? registry
    {
        get => this._registry;
        set
        {
            var newRegistry = value;
            if ((!object.Equals(this._registry, newRegistry)))
            {
                this._registry?.unregisterClient(this);
            }
            this._registry = newRegistry;
            this._registry?.registerClient(this);
        }
    }
}
