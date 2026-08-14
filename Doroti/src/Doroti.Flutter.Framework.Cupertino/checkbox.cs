// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/cupertino/checkbox.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Cupertino;

public static partial class CheckboxLibrary
{
    internal static Color _kDisabledCheckColor = ((Color)(object?)new CupertinoDynamicColor(color: global::Doroti.Flutter.Ui.Color.fromARGB(64L, 0L, 0L, 0L), darkColor: global::Doroti.Flutter.Ui.Color.fromARGB(64L, 255L, 255L, 255L)));
}

public static partial class CheckboxLibrary
{
    internal static Color _kDisabledBorderColor = ((Color)(object?)new CupertinoDynamicColor(color: global::Doroti.Flutter.Ui.Color.fromARGB(13L, 0L, 0L, 0L), darkColor: global::Doroti.Flutter.Ui.Color.fromARGB(13L, 0L, 0L, 0L)));
}

public static partial class CheckboxLibrary
{
    internal static CupertinoDynamicColor _kDefaultBorderColor = new CupertinoDynamicColor(color: global::Doroti.Flutter.Ui.Color.fromARGB(255L, 209L, 209L, 214L), darkColor: global::Doroti.Flutter.Ui.Color.fromARGB(50L, 128L, 128L, 128L));
}

public static partial class CheckboxLibrary
{
    internal static CupertinoDynamicColor _kDefaultFillColor = new CupertinoDynamicColor(color: CupertinoColors.activeBlue, darkColor: global::Doroti.Flutter.Ui.Color.fromARGB(255L, 50L, 100L, 215L));
}

public static partial class CheckboxLibrary
{
    internal static Color _kDefaultCheckColor = ((Color)(object?)new CupertinoDynamicColor(color: CupertinoColors.white, darkColor: global::Doroti.Flutter.Ui.Color.fromARGB(255L, 222L, 232L, 248L)));
}

public static partial class CheckboxLibrary
{
    internal static double _kPressedOverlayOpacity = 0.15;
}

public static partial class CheckboxLibrary
{
    internal static List<double> _kDarkGradientOpacities = new List<double> { 0.14, 0.29 };
}

public static partial class CheckboxLibrary
{
    internal static List<double> _kDisabledDarkGradientOpacities = new List<double> { 0.08, 0.14 };
}

public class CupertinoCheckbox : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual bool? value { get; private set; }
    public virtual global::System.Action<bool?>? onChanged { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual Color? activeColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? fillColor { get; private set; }
    public virtual Color? inactiveColor { get; private set; }
    public virtual Color? checkColor { get; private set; }
    public virtual bool tristate { get; private set; } = default!;
    public virtual Color? focusColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BorderSide? side { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.OutlinedBorder? shape { get; private set; }
    public virtual Size? tapTargetSize { get; private set; }
    public virtual string? semanticLabel { get; private set; }
    public const double width = 14.0;

    public CupertinoCheckbox(global::Doroti.Generated.Framework.Foundation.Key? key = null, bool? value = default!, bool tristate = false, global::System.Action<bool?>? onChanged = default!, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, Color? activeColor = null, Color? inactiveColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? fillColor = null, Color? checkColor = null, Color? focusColor = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, global::Doroti.Generated.Framework.Painting.BorderSide? side = null, global::Doroti.Generated.Framework.Painting.OutlinedBorder? shape = null, Size? tapTargetSize = null, string? semanticLabel = null) : base(key: key)
    {
        this.value = value;
        this.tristate = tristate;
        this.onChanged = onChanged;
        this.mouseCursor = mouseCursor;
        this.activeColor = activeColor;
        this.inactiveColor = inactiveColor;
        this.fillColor = fillColor;
        this.checkColor = checkColor;
        this.focusColor = focusColor;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.side = side;
        this.shape = shape;
        this.tapTargetSize = tapTargetSize;
        this.semanticLabel = semanticLabel;
        System.Diagnostics.Debug.Assert((tristate || (value is not null)));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoCheckboxState__checkbox());
}

internal class _CupertinoCheckboxState__checkbox : global::Doroti.Generated.Framework.Widgets.State<CupertinoCheckbox>, global::Doroti.Generated.Framework.Widgets.TickerProviderStateMixin<CupertinoCheckbox>, global::Doroti.Generated.Framework.Widgets.ToggleableStateMixin<CupertinoCheckbox>
{
    internal virtual _CheckboxPainter__checkbox _painter { get; private set; } = new _CheckboxPainter__checkbox();
    internal virtual bool? _previousValue { get; set; } = default;
    public virtual bool focused { get; set; } = false;
    public virtual HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Animation.AnimationController _positionController { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation _position { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.AnimationController _reactionController { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation _reaction { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation _reactionHoverFade { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.AnimationController _reactionHoverFadeController { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation _reactionFocusFade { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.AnimationController _reactionFocusFadeController { get; set; } = default!;
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

    public override void initState()
    {
        base.initState();
        _positionController = new global::Doroti.Generated.Framework.Animation.AnimationController(duration: global::Doroti.Generated.Framework.Widgets.ToggleableLibrary._kToggleDuration, value: ((this.value == false) ? 0.0 : 1.0), vsync: this);
        _position = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: this._positionController, curve: global::Doroti.Generated.Framework.Animation.Curves.easeIn, reverseCurve: global::Doroti.Generated.Framework.Animation.Curves.easeOut);
        _reactionController = new global::Doroti.Generated.Framework.Animation.AnimationController(duration: this._reactionAnimationDuration, vsync: this);
        _reaction = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: this._reactionController, curve: global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn);
        _reactionHoverFadeController = new global::Doroti.Generated.Framework.Animation.AnimationController(duration: global::Doroti.Generated.Framework.Widgets.ToggleableLibrary._kReactionFadeDuration, value: ((this._hovering || this._focused) ? 1.0 : 0.0), vsync: this);
        _reactionHoverFade = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: this._reactionHoverFadeController, curve: global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn);
        _reactionFocusFadeController = new global::Doroti.Generated.Framework.Animation.AnimationController(duration: global::Doroti.Generated.Framework.Widgets.ToggleableLibrary._kReactionFadeDuration, value: ((this._hovering || this._focused) ? 1.0 : 0.0), vsync: this);
        _reactionFocusFade = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: this._reactionFocusFadeController, curve: global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn);
        _previousValue = ((CupertinoCheckbox)this.widget).value;
    }

    public override void didUpdateWidget(CupertinoCheckbox oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((CupertinoCheckbox)oldWidget).value != ((CupertinoCheckbox)this.widget).value))
        {
            _previousValue = ((CupertinoCheckbox)oldWidget).value;
        }
    }

    public override void dispose()
    {
        this._painter.dispose();
        this._positionController.dispose();
        this._position.dispose();
        this._reactionController.dispose();
        this._reaction.dispose();
        this._reactionHoverFadeController.dispose();
        this._reactionHoverFade.dispose();
        this._reactionFocusFadeController.dispose();
        this._reactionFocusFade.dispose();
        base.dispose();
    }

    public virtual global::System.Action<bool?>? onChanged => ((CupertinoCheckbox)this.widget).onChanged;
    public virtual bool tristate => ((CupertinoCheckbox)this.widget).tristate;
    public virtual bool? value => ((CupertinoCheckbox)this.widget).value;
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color> _defaultFillColor
    {
        get
        {
            return WidgetStateProperty.resolveWith<Color>((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return CupertinoColors.white.withOpacity(0.5);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return ((CupertinoCheckbox)this.widget).activeColor ?? CupertinoDynamicColor.resolve(CheckboxLibrary._kDefaultFillColor, this.context);
}
return CupertinoColors.white;
throw new InvalidOperationException("Dart closure completed without a value.");
});
            return default!;
        }
    }
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color> _defaultCheckColor
    {
        get
        {
            return WidgetStateProperty.resolveWith<Color>((states) => {
if ((states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled) && states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected)))
{
    return ((CupertinoCheckbox)this.widget).checkColor ?? CupertinoDynamicColor.resolve(CheckboxLibrary._kDisabledCheckColor, this.context);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return ((CupertinoCheckbox)this.widget).checkColor ?? CupertinoDynamicColor.resolve(CheckboxLibrary._kDefaultCheckColor, this.context);
}
return CupertinoColors.white;
throw new InvalidOperationException("Dart closure completed without a value.");
});
            return default!;
        }
    }
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide> _defaultSide
    {
        get
        {
            return WidgetStateProperty.resolveWith<global::Doroti.Generated.Framework.Painting.BorderSide>((states) => {
if ((((states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected) || states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))) && !states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled)))
{
    return new global::Doroti.Generated.Framework.Painting.BorderSide(width: 0.0, color: CupertinoColors.transparent);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return new global::Doroti.Generated.Framework.Painting.BorderSide(color: CupertinoDynamicColor.resolve(CheckboxLibrary._kDisabledBorderColor, this.context));
}
return new global::Doroti.Generated.Framework.Painting.BorderSide(color: CupertinoDynamicColor.resolve(CheckboxLibrary._kDefaultBorderColor, this.context));
throw new InvalidOperationException("Dart closure completed without a value.");
});
            return default!;
        }
    }
    internal virtual global::Doroti.Generated.Framework.Painting.BorderSide? _resolveSide(global::Doroti.Generated.Framework.Painting.BorderSide? side, HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> states)
    {
        if ((side is global::Doroti.Generated.Framework.Widgets.WidgetStateBorderSide))
        {
            global::Doroti.Generated.Framework.Widgets.WidgetStateBorderSide side__as14535 = (global::Doroti.Generated.Framework.Widgets.WidgetStateBorderSide)side;
            return ((global::Doroti.Generated.Framework.Painting.BorderSide?)(object?)WidgetStateProperty.resolveAs<global::Doroti.Generated.Framework.Painting.BorderSide?>(((global::Doroti.Generated.Framework.Widgets.WidgetStateBorderSide)side__as14535), states));
        }
        if (!states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
        {
            return side;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> activeStates__14995 = ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() =>
{            var __cascade = this.states;
            __cascade.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.selected);
            return __cascade;        }))();
        HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> inactiveStates__15072 = ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() =>
{            var __cascade = this.states;
            __cascade.Remove(global::Doroti.Generated.Framework.Widgets.WidgetState.selected);
            return __cascade;        }))();
        HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> currentStates__15283 = this.states;
        global::Doroti.Flutter.Ui.Color effectiveActiveColor__15324 = ((global::Doroti.Flutter.Ui.Color)(object?)((((CupertinoCheckbox)this.widget).fillColor?.resolve(activeStates__14995) ?? (Color)this._defaultFillColor.resolve(activeStates__14995))));
        global::Doroti.Flutter.Ui.Color effectiveInactiveColor__15456 = ((global::Doroti.Flutter.Ui.Color)(object?)((((CupertinoCheckbox)this.widget).fillColor?.resolve(inactiveStates__15072) ?? (Color)this._defaultFillColor.resolve(inactiveStates__15072))));
        global::Doroti.Generated.Framework.Painting.BorderSide effectiveBorderSide__15599 = ((_resolveSide(((CupertinoCheckbox)this.widget).side, currentStates__15283) ?? (global::Doroti.Generated.Framework.Painting.BorderSide)this._defaultSide.resolve(currentStates__15283)));
        global::Doroti.Flutter.Ui.Color effectiveFocusOverlayColor__15727 = ((global::Doroti.Flutter.Ui.Color)(object?)((((CupertinoCheckbox)this.widget).focusColor ?? (Color)global::Doroti.Generated.Framework.Painting.HSLColor.CreateFromColor(effectiveActiveColor__15324.withOpacity(ConstantsLibrary.kCupertinoFocusColorOpacity)).withLightness(ConstantsLibrary.kCupertinoFocusColorBrightness).withSaturation(ConstantsLibrary.kCupertinoFocusColorSaturation).toColor())));
        global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor> effectiveMouseCursor__16062 = ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor>)(object?)WidgetStateProperty.resolveWith<global::Doroti.Generated.Framework.Services.MouseCursor>(((global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, global::Doroti.Generated.Framework.Services.MouseCursor>)((states) => {
return ((global::Doroti.Generated.Framework.Services.MouseCursor)(object?)(WidgetStateProperty.resolveAs<global::Doroti.Generated.Framework.Services.MouseCursor?>(((CupertinoCheckbox)this.widget).mouseCursor, states) ?? (((global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb && !states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled)) ? global::Doroti.Generated.Framework.Services.SystemMouseCursors.click : global::Doroti.Generated.Framework.Services.SystemMouseCursors.basic))));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        global::Doroti.Flutter.Ui.Size effectiveSize__16442 = ((global::Doroti.Flutter.Ui.Size)(object?)(((CupertinoCheckbox)this.widget).tapTargetSize ?? (global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform switch { global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS or global::Doroti.Generated.Framework.Foundation.TargetPlatform.android => new global::Doroti.Flutter.Ui.Size(ConstantsLibrary.kMinInteractiveDimensionCupertino), global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia => new global::Doroti.Flutter.Ui.Size(ConstantsLibrary.kMinInteractiveDimensionCupertino), global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS or global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux => new global::Doroti.Flutter.Ui.Size(CupertinoCheckbox.width), global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows => new global::Doroti.Flutter.Ui.Size(CupertinoCheckbox.width), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(label: ((CupertinoCheckbox)this.widget).semanticLabel, @checked: (((CupertinoCheckbox)this.widget).value ?? false), mixed: (((CupertinoCheckbox)this.widget).tristate ? (((CupertinoCheckbox)this.widget).value is null) : null), child: buildToggleable(mouseCursor: effectiveMouseCursor__16062, focusNode: ((CupertinoCheckbox)this.widget).focusNode, autofocus: ((CupertinoCheckbox)this.widget).autofocus, size: effectiveSize__16442, painter: ((Func<_CheckboxPainter__checkbox>)(() =>
{            var __cascade = this._painter;
            __cascade.position = this.position;
            __cascade.reaction = this.reaction;
            __cascade.focusColor = effectiveFocusOverlayColor__15727;
            __cascade.downPosition = this.downPosition;
            __cascade.isFocused = currentStates__15283.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused);
            __cascade.isHovered = currentStates__15283.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered);
            __cascade.activeColor = effectiveActiveColor__15324;
            __cascade.inactiveColor = effectiveInactiveColor__15456;
            __cascade.checkColor = this._defaultCheckColor.resolve(currentStates__15283);
            __cascade.value = this.value;
            __cascade.previousValue = this._previousValue;
            __cascade.isActive = (((CupertinoCheckbox)this.widget).onChanged is not null);
            __cascade.shape = (((CupertinoCheckbox)this.widget).shape ?? new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Flutter.Ui.Radius.circular(4.0))));
            __cascade.side = effectiveBorderSide__15599;
            __cascade.brightness = CupertinoTheme.of(context).brightness;
            return __cascade;        }))())));
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
        var result__17553 = ((Func<global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Generated.Framework.Scheduler.Ticker)(object?)result__17553);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider ticker)
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

    public virtual global::Doroti.Generated.Framework.Animation.AnimationController positionController => this._positionController;
    public virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation position => this._position;
    public virtual global::Doroti.Generated.Framework.Animation.AnimationController reactionController => this._reactionController;
    public virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation reaction => this._reaction;
    public virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation reactionHoverFade => this._reactionHoverFade;
    public virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation reactionFocusFade => this._reactionFocusFade;
    public virtual Duration? reactionAnimationDuration => this._reactionAnimationDuration;
    public virtual bool isInteractive => DartRuntimePrimitives.ConvertValue<bool>((this.onChanged is not null));
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
    public virtual void _handleTapDown(global::Doroti.Generated.Framework.Gestures.TapDownDetails details)
    {
        if (this.isInteractive)
        {
            setState(((global::System.Action)(() => {
this._downPosition = ((global::Doroti.Generated.Framework.Gestures.TapDownDetails)details).localPosition;
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
        ((dynamic)this.context.findRenderObject()!).sendSemanticsEvent(new global::Doroti.Generated.Framework.Semantics.TapSemanticEvent());
    }

    public virtual void _handleTapEnd(global::Doroti.Generated.Framework.Gestures.TapUpDetails? __unused0 = null)
    {
        if ((this._downPosition is not null))
        {
            setState(((global::System.Action)(() => {
this._downPosition = null;
})));
        }
        this._reactionController.reverse();
    }

    public virtual void _handleFocusHighlightChanged(bool focused)
    {
        if ((focused != this._focused))
        {
            setState(((global::System.Action)(() => {
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
            setState(((global::System.Action)(() => {
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

    public virtual HashSet<WidgetState> states => ((Func<HashSet<WidgetState>>)(() => { var __collection10795 = new HashSet<WidgetState>(); if (!this.isInteractive) { __collection10795.Add(WidgetState.disabled); } if (this._hovering) { __collection10795.Add(WidgetState.hovered); } if (this._focused) { __collection10795.Add(WidgetState.focused); } if ((this.value ?? true)) { __collection10795.Add(WidgetState.selected); } return __collection10795; }))();
    public virtual Widget buildToggleable(FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor>? mouseCursor = null, Size size = default!, dynamic painter = default!)
    {
        return ((Widget)(object?)buildToggleableWithChild(focusNode: focusNode, onFocusChange: (global::System.Action<bool>?)onFocusChange, autofocus: autofocus, mouseCursor: mouseCursor, child: new CustomPaint(size: size, painter: painter)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Widget buildToggleableWithChild(FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor>? mouseCursor = null, Widget child = default!)
    {
        return ((Widget)(object?)new FocusableActionDetector(actions: this._actionMap, focusNode: focusNode, autofocus: autofocus, onFocusChange: (global::System.Action<bool>?)onFocusChange, enabled: this.isInteractive, onShowFocusHighlight: (global::System.Action<bool>)this._handleFocusHighlightChanged, onShowHoverHighlight: (global::System.Action<bool>)this._handleHoverChanged, mouseCursor: (mouseCursor?.resolve(this.states) ?? global::Doroti.Generated.Framework.Services.SystemMouseCursors.basic), child: new GestureDetector(excludeFromSemantics: !this.isInteractive, onTapDown: ((global::System.Action<global::Doroti.Generated.Framework.Gestures.TapDownDetails>)(this.isInteractive ? this._handleTapDown : null)), onTap: () => ((global::System.Action<Intent?>)(this.isInteractive ? this._handleTap : null))(default), onTapUp: ((global::System.Action<global::Doroti.Generated.Framework.Gestures.TapUpDetails?>)(this.isInteractive ? this._handleTapEnd : null)), onTapCancel: () => ((global::System.Action<global::Doroti.Generated.Framework.Gestures.TapUpDetails?>)(this.isInteractive ? this._handleTapEnd : null))(default), child: new global::Doroti.Generated.Framework.Widgets.Semantics(enabled: this.isInteractive, child: child))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CheckboxPainter__checkbox : global::Doroti.Generated.Framework.Widgets.ToggleablePainter
{
    internal virtual Color? _checkColor { get; set; } = default;
    internal virtual bool? _value { get; set; } = default;
    internal virtual bool? _previousValue { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Painting.OutlinedBorder? _shape { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Painting.BorderSide? _side { get; set; } = default;
    internal virtual Brightness? _brightness { get; set; } = default;

    public virtual global::Doroti.Flutter.Ui.Color checkColor
    {
        get => this._checkColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(this._checkColor, __value)))
            {
                return;
            }
            _checkColor = __value;
            notifyListeners();
        }
    }
    public virtual bool? value
    {
        get => this._value;
        set
        {
            var __value = value;
            if ((this._value == __value))
            {
                return;
            }
            _value = __value;
            notifyListeners();
        }
    }
    public virtual bool? previousValue
    {
        get => this._previousValue;
        set
        {
            var __value = value;
            if ((this._previousValue == __value))
            {
                return;
            }
            _previousValue = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Generated.Framework.Painting.OutlinedBorder shape
    {
        get => this._shape!;
        set
        {
            var __value = value;
            if ((object.Equals(this._shape, __value)))
            {
                return;
            }
            _shape = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Generated.Framework.Painting.BorderSide side
    {
        get => this._side!;
        set
        {
            var __value = value;
            if ((object.Equals(this._side, __value)))
            {
                return;
            }
            _side = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Flutter.Ui.Brightness? brightness
    {
        get => this._brightness;
        set
        {
            var __value = value;
            if ((object.Equals(this._brightness, __value)))
            {
                return;
            }
            _brightness = __value;
            notifyListeners();
        }
    }
    internal virtual global::Doroti.Flutter.Ui.Rect _outerRectAt(Offset origin)
    {
        double size__19328 = CupertinoCheckbox.width;
        var rect__19370 = global::Doroti.Flutter.Ui.Rect.fromLTWH(origin.dx, origin.dy, size__19328, size__19328);
        return rect__19370;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Flutter.Ui.Color _colorAt(bool value)
    {
        return ((global::Doroti.Flutter.Ui.Color)(object?)((DartRuntimePrimitives.RequireValue(value) && this.isActive) ? this.activeColor : this.inactiveColor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Flutter.Ui.Paint _createStrokePaint()
    {
        return ((global::Doroti.Flutter.Ui.Paint)(object?)((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Paint();
            __cascade.color = this.checkColor;
            __cascade.style = PaintingStyle.stroke;
            __cascade.strokeWidth = 2.0;
            __cascade.strokeCap = StrokeCap.round;
            return __cascade;        }))());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _drawFillGradient(Canvas canvas, Rect outer, Color topColor, Color bottomColor)
    {
        var fillGradient__20045 = new global::Doroti.Generated.Framework.Painting.LinearGradient(begin: global::Doroti.Generated.Framework.Painting.Alignment.topCenter, end: global::Doroti.Generated.Framework.Painting.Alignment.bottomCenter, colors: new List<global::Doroti.Flutter.Ui.Color> { topColor, bottomColor });
        var gradientPaint__20297 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Paint();
            __cascade.shader = fillGradient__20045.createShader(outer);
            return __cascade;        }))();
        if (this.shape.preferPaintInterior)
        {
            this.shape.paintInterior(canvas, outer, gradientPaint__20297);
        }
        else
        {
            canvas.drawPath(this.shape.getOuterPath(outer), gradientPaint__20297);
        }
    }

    internal virtual void _drawBox(Canvas canvas, Rect outer, Paint paint, global::Doroti.Generated.Framework.Painting.BorderSide? side, bool value)
    {
        if (((object.Equals(this.brightness, Brightness.dark)) && !((this.isActive && DartRuntimePrimitives.RequireValue(value)))))
        {
            _drawFillGradient(canvas, outer, paint.color.withOpacity((this.isActive ? CheckboxLibrary._kDarkGradientOpacities[(int)(0L)] : CheckboxLibrary._kDisabledDarkGradientOpacities[(int)(0L)])), paint.color.withOpacity((this.isActive ? CheckboxLibrary._kDarkGradientOpacities[(int)(1L)] : CheckboxLibrary._kDisabledDarkGradientOpacities[(int)(1L)])));
        }
        else
        {
            if (this.shape.preferPaintInterior)
            {
                this.shape.paintInterior(canvas, outer, paint);
            }
            else
            {
                canvas.drawPath(this.shape.getOuterPath(outer), paint);
            }
        }
        if ((side is not null))
        {
            this.shape.copyWith(side: side).paint(canvas, outer);
        }
    }

    internal virtual void _drawCheck(Canvas canvas, Offset origin, Paint paint)
    {
        var path__21443 = new global::Doroti.Flutter.Ui.Path();
        var start__21685 = new global::Doroti.Flutter.Ui.Offset((CupertinoCheckbox.width * 0.22), (CupertinoCheckbox.width * 0.54));
        var mid__21775 = new global::Doroti.Flutter.Ui.Offset((CupertinoCheckbox.width * 0.4), (CupertinoCheckbox.width * 0.75));
        var end__21863 = new global::Doroti.Flutter.Ui.Offset((CupertinoCheckbox.width * 0.78), (CupertinoCheckbox.width * 0.25));
        path__21443.moveTo((origin.dx + start__21685.dx), (origin.dy + start__21685.dy));
        path__21443.lineTo((origin.dx + mid__21775.dx), (origin.dy + mid__21775.dy));
        path__21443.moveTo((origin.dx + mid__21775.dx), (origin.dy + mid__21775.dy));
        path__21443.lineTo((origin.dx + end__21863.dx), (origin.dy + end__21863.dy));
        canvas.drawPath(path__21443, paint);
    }

    internal virtual void _drawDash(Canvas canvas, Offset origin, Paint paint)
    {
        var start__22422 = new global::Doroti.Flutter.Ui.Offset((CupertinoCheckbox.width * 0.25), (CupertinoCheckbox.width * 0.5));
        var end__22511 = new global::Doroti.Flutter.Ui.Offset((CupertinoCheckbox.width * 0.75), (CupertinoCheckbox.width * 0.5));
        canvas.drawLine((origin + start__22422), (origin + end__22511), paint);
    }

    public virtual void paint(Canvas canvas, Size size)
    {
        global::Doroti.Flutter.Ui.Paint strokePaint__22720 = ((global::Doroti.Flutter.Ui.Paint)(object?)_createStrokePaint());
        var origin__22766 = ((size / 2.0) - (new global::Doroti.Flutter.Ui.Size(CupertinoCheckbox.width) / 2.0));
        global::Doroti.Flutter.Ui.Rect outer__22863 = ((global::Doroti.Flutter.Ui.Rect)(object?)_outerRectAt(origin__22766));
        var paint__22903 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Paint();
            __cascade.color = _colorAt((this.value ?? true));
            return __cascade;        }))();
        switch (this.value)
        {
            case false:
                {
                    _drawBox(canvas, outer__22863, paint__22903, this.side, (this.value ?? true));
                    break;
                }
            case true:
                {
                    _drawBox(canvas, outer__22863, paint__22903, this.side, (this.value ?? true));
                    _drawCheck(canvas, origin__22766, strokePaint__22720);
                    break;
                }
            case null:
                {
                    _drawBox(canvas, outer__22863, paint__22903, this.side, (this.value ?? true));
                    _drawDash(canvas, origin__22766, strokePaint__22720);
                    break;
                }
        }
        if ((this.downPosition is not null))
        {
            var pressedPaint__23409 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Paint();
            __cascade.color = ((object.Equals(this.brightness, Brightness.light)) ? CupertinoColors.black.withOpacity(CheckboxLibrary._kPressedOverlayOpacity) : CupertinoColors.white.withOpacity(CheckboxLibrary._kPressedOverlayOpacity));
            return __cascade;        }))();
            if (this.shape.preferPaintInterior)
            {
                this.shape.paintInterior(canvas, outer__22863, pressedPaint__23409);
            }
            else
            {
                canvas.drawPath(this.shape.getOuterPath(outer__22863), pressedPaint__23409);
            }
        }
        if (this.isFocused)
        {
            global::Doroti.Flutter.Ui.Rect focusOuter__23858 = ((global::Doroti.Flutter.Ui.Rect)(object?)outer__22863.inflate(1));
            var borderPaint__23901 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Paint();
            __cascade.color = this.focusColor;
            __cascade.style = PaintingStyle.stroke;
            __cascade.strokeWidth = 3.5;
            return __cascade;        }))();
            _drawBox(canvas, focusOuter__23858, borderPaint__23901, this.side, (this.value ?? true));
        }
    }

}
