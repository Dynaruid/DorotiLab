// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/switch.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Cupertino;

public static partial class SwitchLibrary
{
    internal static double _kDisabledOpacity = 0.5;
}

public static partial class SwitchLibrary
{
    internal static double _kThumbRadius = 14.0;
}

public static partial class SwitchLibrary
{
    internal static double _kTrackHeight = 31.0;
}

public static partial class SwitchLibrary
{
    internal static double _kTrackWidth = 51.0;
}

public static partial class SwitchLibrary
{
    internal static Size _kSwitchSize = new global::Doroti.Ui.Size(59.0, 39.0);
}

public static partial class SwitchLibrary
{
    internal static double _kThumbExtensionFactor = 7.0;
}

public static partial class SwitchLibrary
{
    internal static List<global::Doroti.Framework.Painting.BoxShadow> _kSwitchBoxShadows = new List<global::Doroti.Framework.Painting.BoxShadow> { new global::Doroti.Framework.Painting.BoxShadow(color: new global::Doroti.Ui.Color(637534208L), offset: new global::Doroti.Ui.Offset(0, 3), blurRadius: 8.0), new global::Doroti.Framework.Painting.BoxShadow(color: new global::Doroti.Ui.Color(251658240L), offset: new global::Doroti.Ui.Offset(0, 3), blurRadius: 1.0) };
}

public static partial class SwitchLibrary
{
    internal static double _kDragCommitThreshold = 0.7;
}

public static partial class SwitchLibrary
{
    internal static double _kDragReverseThreshold = 0.2;
}

public static partial class SwitchLibrary
{
    internal static double _kOnLabelWidth = 1.0;
}

public static partial class SwitchLibrary
{
    internal static double _kOnLabelHeight = 10.0;
}

public static partial class SwitchLibrary
{
    internal static double _kOnLabelPaddingHorizontal = 11.0;
}

public static partial class SwitchLibrary
{
    internal static double _kOffLabelWidth = 1.0;
}

public static partial class SwitchLibrary
{
    internal static double _kOffLabelPaddingHorizontal = 12.0;
}

public static partial class SwitchLibrary
{
    internal static double _kOffLabelRadius = 5.0;
}

public static partial class SwitchLibrary
{
    internal static CupertinoDynamicColor _kOffLabelColor = new CupertinoDynamicColor(debugLabel: "offSwitchLabel", color: global::Doroti.Ui.Color.fromARGB(255L, 179L, 179L, 179L), darkColor: global::Doroti.Ui.Color.fromARGB(255L, 179L, 179L, 179L), highContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 255L, 255L, 255L), darkHighContrastColor: global::Doroti.Ui.Color.fromARGB(255L, 255L, 255L, 255L));
}

public class CupertinoSwitch : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual bool value { get; private set; } = default!;
    public virtual global::System.Action<bool>? onChanged { get; private set; }
    public virtual Color? activeTrackColor { get; private set; }
    public virtual Color? inactiveTrackColor { get; private set; }
    public virtual Color? thumbColor { get; private set; }
    public virtual Color? inactiveThumbColor { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual Color? onLabelColor { get; private set; }
    public virtual Color? offLabelColor { get; private set; }
    public virtual dynamic activeThumbImage { get; private set; } = default!;
    public virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? onActiveThumbImageError { get; private set; }
    public virtual dynamic inactiveThumbImage { get; private set; } = default!;
    public virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? onInactiveThumbImageError { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackOutlineColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? trackOutlineWidth { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.Icon?>? thumbIcon { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor>? mouseCursor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual bool? applyTheme { get; private set; }
    public virtual global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;

    public CupertinoSwitch(global::Doroti.Framework.Foundation.Key? key = null, bool value = default!, global::System.Action<bool>? onChanged = default!, Color? activeColor = null, Color? trackColor = null, Color? activeTrackColor = null, Color? inactiveTrackColor = null, Color? thumbColor = null, Color? inactiveThumbColor = null, bool? applyTheme = null, Color? focusColor = null, Color? onLabelColor = null, Color? offLabelColor = null, dynamic activeThumbImage = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onActiveThumbImageError = null, dynamic inactiveThumbImage = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onInactiveThumbImageError = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackOutlineColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? trackOutlineWidth = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.Icon?>? thumbIcon = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor>? mouseCursor = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start) : base(key: key)
    {
        this.value = value;
        this.onChanged = onChanged;
        this.thumbColor = thumbColor;
        this.inactiveThumbColor = inactiveThumbColor;
        this.applyTheme = applyTheme;
        this.focusColor = focusColor;
        this.onLabelColor = onLabelColor;
        this.offLabelColor = offLabelColor;
        this.activeThumbImage = activeThumbImage;
        this.onActiveThumbImageError = onActiveThumbImageError;
        this.inactiveThumbImage = inactiveThumbImage;
        this.onInactiveThumbImageError = onInactiveThumbImageError;
        this.trackOutlineColor = trackOutlineColor;
        this.trackOutlineWidth = trackOutlineWidth;
        this.thumbIcon = thumbIcon;
        this.mouseCursor = mouseCursor;
        this.focusNode = focusNode;
        this.onFocusChange = onFocusChange;
        this.autofocus = autofocus;
        this.dragStartBehavior = dragStartBehavior;
        this.activeTrackColor = (activeTrackColor ?? activeColor);
        this.inactiveTrackColor = (inactiveTrackColor ?? trackColor);
        System.Diagnostics.Debug.Assert(((activeThumbImage is not null) || (onActiveThumbImageError is null)));
        System.Diagnostics.Debug.Assert(((inactiveThumbImage is not null) || (onInactiveThumbImageError is null)));
        System.Diagnostics.Debug.Assert(((activeTrackColor is null) || (activeColor is null)));
        System.Diagnostics.Debug.Assert(((inactiveTrackColor is null) || (trackColor is null)));
    }

    public virtual global::Doroti.Ui.Color? activeColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this.activeTrackColor);
    public virtual global::Doroti.Ui.Color? trackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this.inactiveTrackColor);
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoSwitchState__switch());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("value", value: this.value, ifTrue: "on", ifFalse: "off", showName: true));
        properties.add(new global::Doroti.Framework.Foundation.ObjectFlagProperty<global::System.Action<bool>>("onChanged", (global::System.Action<bool>?)this.onChanged, ifNull: "disabled"));
    }

}

internal class _CupertinoSwitchState__switch : global::Doroti.Framework.Widgets.State<CupertinoSwitch>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<CupertinoSwitch>, global::Doroti.Framework.Widgets.ToggleableStateMixin<CupertinoSwitch>
{
    internal virtual _SwitchPainter__switch _painter { get; private set; } = new _SwitchPainter__switch();
    internal virtual Offset _dragStartPosition { get; set; } = Offset.zero;
    internal virtual double _dragDelta { get; set; } = 0;
    internal virtual bool? _dragValue { get; set; } = default;
    internal virtual bool _needsPositionAnimation { get; set; } = false;
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

    public override void initState()
    {
        base.initState();
        _positionController = new global::Doroti.Framework.Animation.AnimationController(duration: global::Doroti.Framework.Widgets.ToggleableLibrary._kToggleDuration, value: ((this.value == false) ? 0.0 : 1.0), vsync: this);
        _position = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._positionController, curve: global::Doroti.Framework.Animation.Curves.easeIn, reverseCurve: global::Doroti.Framework.Animation.Curves.easeOut);
        _reactionController = new global::Doroti.Framework.Animation.AnimationController(duration: this._reactionAnimationDuration, vsync: this);
        _reaction = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._reactionController, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
        _reactionHoverFadeController = new global::Doroti.Framework.Animation.AnimationController(duration: global::Doroti.Framework.Widgets.ToggleableLibrary._kReactionFadeDuration, value: ((this._hovering || this._focused) ? 1.0 : 0.0), vsync: this);
        _reactionHoverFade = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._reactionHoverFadeController, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
        _reactionFocusFadeController = new global::Doroti.Framework.Animation.AnimationController(duration: global::Doroti.Framework.Widgets.ToggleableLibrary._kReactionFadeDuration, value: ((this._hovering || this._focused) ? 1.0 : 0.0), vsync: this);
        _reactionFocusFade = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._reactionFocusFadeController, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
        this.positionController.duration = Duration.Create(milliseconds: 200L);
        this.reactionController.duration = Duration.Create(milliseconds: 300L);
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.CurvedAnimation>)(() =>
{            var __cascade = this.position;
            __cascade.curve = global::Doroti.Framework.Animation.Curves.ease;
            __cascade.reverseCurve = global::Doroti.Framework.Animation.Curves.ease.flipped;
            return __cascade;        }))());
    }

    public override void didUpdateWidget(CupertinoSwitch oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((CupertinoSwitch)oldWidget).value != ((CupertinoSwitch)this.widget).value))
        {
            animateToValue();
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

    public virtual global::System.Action<bool?>? onChanged => ((global::System.Action<bool?>)((((CupertinoSwitch)this.widget).onChanged is not null) ? this._handleChanged : null));
    public virtual bool tristate => false;
    public virtual bool? value => ((CupertinoSwitch)this.widget).value;
    internal virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> _widgetThumbColor
    {
        get
        {
            return WidgetStateProperty.resolveWith<Color?>((states) => {
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
{
    return ((CupertinoSwitch)this.widget).thumbColor;
}
return ((CupertinoSwitch)this.widget).inactiveThumbColor;
throw new InvalidOperationException("Dart closure completed without a value.");
});
            return default!;
        }
    }
    internal virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> _widgetTrackColor
    {
        get
        {
            return WidgetStateProperty.resolveWith<Color?>((states) => {
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
{
    return ((CupertinoSwitch)this.widget).activeTrackColor;
}
return ((CupertinoSwitch)this.widget).inactiveTrackColor;
throw new InvalidOperationException("Dart closure completed without a value.");
});
            return default!;
        }
    }
    internal virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor> _defaultMouseCursor => WidgetStateProperty.resolveWith(((global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Services.MouseCursor>)((states) => {
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
{
    return global::Doroti.Framework.Services.MouseCursor.defer;
}
return ((global::Doroti.Framework.Services.MouseCursor)(object?)(global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb ? global::Doroti.Framework.Services.SystemMouseCursors.click : global::Doroti.Framework.Services.MouseCursor.defer));
throw new InvalidOperationException("Dart closure completed without a value.");
})));
    internal virtual global::Doroti.Ui.Color? _resolveTrackColor(Color? trackColor, HashSet<global::Doroti.Framework.Widgets.WidgetState> states)
    {
        if ((trackColor is global::Doroti.Framework.Widgets.WidgetStateColor))
        {
            global::Doroti.Framework.Widgets.WidgetStateColor trackColor__as19180 = (global::Doroti.Framework.Widgets.WidgetStateColor)trackColor;
            return ((global::Doroti.Ui.Color?)(object?)WidgetStateProperty.resolveAs<global::Doroti.Ui.Color?>(((global::Doroti.Framework.Widgets.WidgetStateColor)trackColor__as19180), states));
        }
        return ((global::Doroti.Ui.Color?)(object?)trackColor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Color? _resolveThumbColor(Color? thumbColor, HashSet<global::Doroti.Framework.Widgets.WidgetState> states)
    {
        if ((thumbColor is global::Doroti.Framework.Widgets.WidgetStateColor))
        {
            global::Doroti.Framework.Widgets.WidgetStateColor thumbColor__as19402 = (global::Doroti.Framework.Widgets.WidgetStateColor)thumbColor;
            return ((global::Doroti.Ui.Color?)(object?)WidgetStateProperty.resolveAs<global::Doroti.Ui.Color?>(((global::Doroti.Framework.Widgets.WidgetStateColor)thumbColor__as19402), states));
        }
        return ((global::Doroti.Ui.Color?)(object?)thumbColor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _trackInnerLength
    {
        get
        {
            double trackInnerStart__19592 = (SwitchLibrary._kTrackHeight / 2.0);
            double trackInnerEnd__19648 = (SwitchLibrary._kTrackWidth - trackInnerStart__19592);
            double trackInnerLength__19713 = (trackInnerEnd__19648 - trackInnerStart__19592);
            return trackInnerLength__19713;
            return default!;
        }
    }
    internal virtual void _handleOnTapDown(global::Doroti.Framework.Gestures.TapDownDetails details)
    {
        if (this.isInteractive)
        {
            _dragStartPosition = ((global::Doroti.Framework.Gestures.TapDownDetails)details).globalPosition;
        }
    }

    internal virtual void _handleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        if (this.isInteractive)
        {
            this.reactionController.forward();
            if ((!object.Equals(this._dragStartPosition, Offset.zero)))
            {
                double delta__20118 = (((((global::Doroti.Framework.Gestures.DragStartDetails)details).globalPosition - this._dragStartPosition)).dx / SwitchLibrary._kTrackWidth);
                _dragDelta = (Directionality.of(this.context) switch { TextDirection.rtl => -delta__20118, TextDirection.ltr => delta__20118, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            }
            _dragValue = this.value;
        }
    }

    internal virtual void _handleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        if (this.isInteractive)
        {
            double delta__20482 = (DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).primaryDelta) / SwitchLibrary._kTrackWidth);
            _dragDelta += (Directionality.of(this.context) switch { TextDirection.rtl => -delta__20482, TextDirection.ltr => delta__20482, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            var valueChangedWhileDragging__20681 = (((CupertinoSwitch)this.widget).value != this._dragValue);
            double threshold__20757 = (valueChangedWhileDragging__20681 ? SwitchLibrary._kDragReverseThreshold : SwitchLibrary._kDragCommitThreshold);
            double effectiveThreshold__20884 = (((CupertinoSwitch)this.widget).value ? -threshold__20757 : threshold__20757);
            bool newDragValue__20962 = (this._dragDelta >= effectiveThreshold__20884);
            if ((this._dragValue != newDragValue__20962))
            {
                _emitVibration();
                if (newDragValue__20962)
                {
                    this.positionController.forward();
                }
                else
                {
                    this.positionController.reverse();
                }
                _dragValue = newDragValue__20962;
            }
        }
    }

    internal virtual void _handleDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        if ((this._dragValue != ((CupertinoSwitch)this.widget).value))
        {
            ((CupertinoSwitch)this.widget).onChanged?.Invoke(!((CupertinoSwitch)this.widget).value);
            setState(((global::System.Action)(() => {
_needsPositionAnimation = true;
})));
        }
        _dragStartPosition = Offset.zero;
        _dragDelta = 0;
        _dragValue = null;
        this.reactionController.reverse();
    }

    internal virtual void _handleChanged(bool? value)
    {
        DartRuntimePrimitives.Assert(() => (value is not null));
        DartRuntimePrimitives.Assert(() => (((CupertinoSwitch)this.widget).onChanged is not null));
        ((CupertinoSwitch)this.widget).onChanged?.Invoke(DartRuntimePrimitives.RequireValue(value));
        _emitVibration();
    }

    internal virtual void _emitVibration()
    {
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    DartRuntimePrimitives.Ignore(HapticFeedback.lightImpact());
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    break;
                }
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (this._needsPositionAnimation)
        {
            _needsPositionAnimation = false;
            animateToValue();
        }
        CupertinoThemeData theme__22457 = CupertinoTheme.of(context);
        global::Doroti.Ui.Color activeColor__22510 = ((global::Doroti.Ui.Color)(object?)CupertinoDynamicColor.resolve(((((CupertinoSwitch)this.widget).activeTrackColor ?? ((((((CupertinoSwitch)this.widget).applyTheme ?? theme__22457.applyThemeToAll) == true) ? theme__22457.primaryColor : null))) ?? CupertinoColors.systemGreen), context));
        (global::Doroti.Ui.Color, global::Doroti.Ui.Color)? onOffLabelColors__22791 = (MediaQuery.onOffSwitchLabelsOf(context) ? (CupertinoDynamicColor.resolve((((CupertinoSwitch)this.widget).onLabelColor ?? CupertinoColors.white), context), CupertinoDynamicColor.resolve((((CupertinoSwitch)this.widget).offLabelColor ?? SwitchLibrary._kOffLabelColor), context)) : null);
        HashSet<global::Doroti.Framework.Widgets.WidgetState> activeStates__23241 = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{            var __cascade = this.states;
            __cascade.Add(global::Doroti.Framework.Widgets.WidgetState.selected);
            return __cascade;        }))();
        HashSet<global::Doroti.Framework.Widgets.WidgetState> inactiveStates__23318 = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{            var __cascade = this.states;
            __cascade.Remove(global::Doroti.Framework.Widgets.WidgetState.selected);
            return __cascade;        }))();
        global::Doroti.Ui.Color effectiveActiveThumbColor__23390 = ((global::Doroti.Ui.Color)(object?)(((_resolveThumbColor(((CupertinoSwitch)this.widget).thumbColor, activeStates__23241) ?? (Color)this._widgetThumbColor.resolve(activeStates__23241))) ?? CupertinoColors.white));
        global::Doroti.Ui.Color effectiveInactiveThumbColor__23580 = ((global::Doroti.Ui.Color)(object?)(((_resolveThumbColor(((CupertinoSwitch)this.widget).inactiveThumbColor, inactiveStates__23318) ?? (Color)this._widgetThumbColor.resolve(inactiveStates__23318))) ?? effectiveActiveThumbColor__23390));
        global::Doroti.Ui.Color effectiveActiveTrackColor__23788 = ((global::Doroti.Ui.Color)(object?)(this._widgetTrackColor.resolve(activeStates__23241) ?? activeColor__22510));
        global::Doroti.Ui.Color? effectiveActiveTrackOutlineColor__23890 = ((global::Doroti.Ui.Color?)(object?)((CupertinoSwitch)this.widget).trackOutlineColor?.resolve(activeStates__23241));
        double? effectiveActiveTrackOutlineWidth__23993 = ((CupertinoSwitch)this.widget).trackOutlineWidth?.resolve(activeStates__23241);
        global::Doroti.Ui.Color effectiveInactiveTrackColor__24107 = ((global::Doroti.Ui.Color)(object?)(_resolveTrackColor(((CupertinoSwitch)this.widget).trackColor, inactiveStates__23318) ?? CupertinoDynamicColor.resolve(CupertinoColors.secondarySystemFill, context)));
        global::Doroti.Ui.Color? effectiveInactiveTrackOutlineColor__24305 = ((global::Doroti.Ui.Color?)(object?)((CupertinoSwitch)this.widget).trackOutlineColor?.resolve(inactiveStates__23318));
        double? effectiveInactiveTrackOutlineWidth__24425 = ((CupertinoSwitch)this.widget).trackOutlineWidth?.resolve(inactiveStates__23318);
        global::Doroti.Framework.Widgets.Icon? effectiveActiveIcon__24543 = ((global::Doroti.Framework.Widgets.Icon?)(object?)((CupertinoSwitch)this.widget).thumbIcon?.resolve(activeStates__23241));
        global::Doroti.Framework.Widgets.Icon? effectiveInactiveIcon__24623 = ((global::Doroti.Framework.Widgets.Icon?)(object?)((CupertinoSwitch)this.widget).thumbIcon?.resolve(inactiveStates__23318));
        global::Doroti.Ui.Color effectiveActiveIconColor__24707 = ((global::Doroti.Ui.Color)(object?)(effectiveActiveIcon__24543?.color ?? CupertinoColors.black));
        global::Doroti.Ui.Color effectiveInactiveIconColor__24804 = ((global::Doroti.Ui.Color)(object?)(effectiveInactiveIcon__24623?.color ?? CupertinoColors.black));
        var activePressedStates__24899 = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{            var __cascade = activeStates__23241;
            __cascade.Add(global::Doroti.Framework.Widgets.WidgetState.pressed);
            return __cascade;        }))();
        global::Doroti.Ui.Color effectiveActivePressedThumbColor__24977 = ((global::Doroti.Ui.Color)(object?)(((_resolveThumbColor(((CupertinoSwitch)this.widget).thumbColor, activePressedStates__24899) ?? (Color)this._widgetThumbColor.resolve(activePressedStates__24899))) ?? CupertinoColors.white));
        var inactivePressedStates__25182 = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() =>
{            var __cascade = inactiveStates__23318;
            __cascade.Add(global::Doroti.Framework.Widgets.WidgetState.pressed);
            return __cascade;        }))();
        global::Doroti.Ui.Color effectiveInactivePressedThumbColor__25264 = ((global::Doroti.Ui.Color)(object?)(((_resolveThumbColor(((CupertinoSwitch)this.widget).thumbColor, inactivePressedStates__25182) ?? (Color)this._widgetThumbColor.resolve(inactivePressedStates__25182))) ?? CupertinoColors.white));
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor> effectiveMouseCursor__25508 = ((((CupertinoSwitch)this.widget).mouseCursor ?? (global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor>)this._defaultMouseCursor));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(toggled: ((CupertinoSwitch)this.widget).value, child: new global::Doroti.Framework.Widgets.GestureDetector(excludeFromSemantics: true, onTapDown: (global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>)this._handleOnTapDown, onHorizontalDragStart: (global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>)this._handleDragStart, onHorizontalDragUpdate: (global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)this._handleDragUpdate, onHorizontalDragEnd: (global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>)this._handleDragEnd, dragStartBehavior: ((CupertinoSwitch)this.widget).dragStartBehavior, child: new global::Doroti.Framework.Widgets.Opacity(opacity: ((this.onChanged is null) ? SwitchLibrary._kDisabledOpacity : 1), child: buildToggleable(mouseCursor: effectiveMouseCursor__25508, focusNode: ((CupertinoSwitch)this.widget).focusNode, onFocusChange: (global::System.Action<bool>?)((CupertinoSwitch)this.widget).onFocusChange, autofocus: ((CupertinoSwitch)this.widget).autofocus, size: SwitchLibrary._kSwitchSize, painter: ((Func<_SwitchPainter__switch>)(() =>
{            var __cascade = this._painter;
            __cascade.position = this.position;
            __cascade.reaction = this.reaction;
            __cascade.reactionFocusFade = this.reactionFocusFade;
            __cascade.reactionHoverFade = this.reactionHoverFade;
            __cascade.focusColor = CupertinoDynamicColor.resolve(((((CupertinoSwitch)this.widget).focusColor ?? (Color)global::Doroti.Framework.Painting.HSLColor.CreateFromColor(activeColor__22510.withOpacity(ConstantsLibrary.kCupertinoFocusColorOpacity)).withLightness(ConstantsLibrary.kCupertinoFocusColorBrightness).withSaturation(ConstantsLibrary.kCupertinoFocusColorSaturation).toColor())), context);
            __cascade.downPosition = this.downPosition;
            __cascade.isFocused = this.states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused);
            __cascade.isHovered = this.states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered);
            __cascade.activeColor = effectiveActiveThumbColor__23390;
            __cascade.inactiveColor = effectiveInactiveThumbColor__23580;
            __cascade.activePressedColor = effectiveActivePressedThumbColor__24977;
            __cascade.onOffLabelColors = onOffLabelColors__22791;
            __cascade.inactivePressedColor = effectiveInactivePressedThumbColor__25264;
            __cascade.activeThumbImage = ((CupertinoSwitch)this.widget).activeThumbImage;
            __cascade.onActiveThumbImageError = ((CupertinoSwitch)this.widget).onActiveThumbImageError;
            __cascade.inactiveThumbImage = ((CupertinoSwitch)this.widget).inactiveThumbImage;
            __cascade.onInactiveThumbImageError = ((CupertinoSwitch)this.widget).onInactiveThumbImageError;
            __cascade.activeTrackColor = effectiveActiveTrackColor__23788;
            __cascade.activeTrackOutlineColor = effectiveActiveTrackOutlineColor__23890;
            __cascade.activeTrackOutlineWidth = effectiveActiveTrackOutlineWidth__23993;
            __cascade.inactiveTrackColor = effectiveInactiveTrackColor__24107;
            __cascade.inactiveTrackOutlineColor = effectiveInactiveTrackOutlineColor__24305;
            __cascade.inactiveTrackOutlineWidth = effectiveInactiveTrackOutlineWidth__24425;
            __cascade.configuration = global::Doroti.Framework.Widgets.ImageLibrary.createLocalImageConfiguration(context);
            __cascade.isInteractive = this.isInteractive;
            __cascade.trackInnerLength = this._trackInnerLength;
            __cascade.textDirection = Directionality.of(context);
            __cascade.activeIconColor = effectiveActiveIconColor__24707;
            __cascade.inactiveIconColor = effectiveInactiveIconColor__24804;
            __cascade.activeIcon = effectiveActiveIcon__24543;
            __cascade.inactiveIcon = effectiveInactiveIcon__24623;
            __cascade.iconTheme = IconTheme.of(context);
            __cascade.surfaceColor = theme__22457.scaffoldBackgroundColor;
            __cascade.positionController = this.positionController;
            return __cascade;        }))())))));
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
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result__17553);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider ticker)
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
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
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
    public virtual void _handleTapDown(global::Doroti.Framework.Gestures.TapDownDetails details)
    {
        if (this.isInteractive)
        {
            setState(((global::System.Action)(() => {
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
    public virtual Widget buildToggleable(FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor>? mouseCursor = null, Size size = default!, dynamic painter = default!)
    {
        return ((Widget)(object?)buildToggleableWithChild(focusNode: focusNode, onFocusChange: (global::System.Action<bool>?)onFocusChange, autofocus: autofocus, mouseCursor: mouseCursor, child: new CustomPaint(size: size, painter: painter)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Widget buildToggleableWithChild(FocusNode? focusNode = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor>? mouseCursor = null, Widget child = default!)
    {
        return ((Widget)(object?)new FocusableActionDetector(actions: this._actionMap, focusNode: focusNode, autofocus: autofocus, onFocusChange: (global::System.Action<bool>?)onFocusChange, enabled: this.isInteractive, onShowFocusHighlight: (global::System.Action<bool>)this._handleFocusHighlightChanged, onShowHoverHighlight: (global::System.Action<bool>)this._handleHoverChanged, mouseCursor: (mouseCursor?.resolve(this.states) ?? global::Doroti.Framework.Services.SystemMouseCursors.basic), child: new GestureDetector(excludeFromSemantics: !this.isInteractive, onTapDown: ((global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>)(this.isInteractive ? this._handleTapDown : null)), onTap: () => ((global::System.Action<Intent?>)(this.isInteractive ? this._handleTap : null))(default), onTapUp: ((global::System.Action<global::Doroti.Framework.Gestures.TapUpDetails?>)(this.isInteractive ? this._handleTapEnd : null)), onTapCancel: () => ((global::System.Action<global::Doroti.Framework.Gestures.TapUpDetails?>)(this.isInteractive ? this._handleTapEnd : null))(default), child: new global::Doroti.Framework.Widgets.Semantics(enabled: this.isInteractive, child: child))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SwitchPainter__switch : global::Doroti.Framework.Widgets.ToggleablePainter
{
    internal virtual global::Doroti.Framework.Animation.AnimationController? _positionController { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _colorAnimation { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.Icon? _activeIcon { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.Icon? _inactiveIcon { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.IconThemeData? _iconTheme { get; set; } = default;
    internal virtual Color? _activeIconColor { get; set; } = default;
    internal virtual Color? _inactiveIconColor { get; set; } = default;
    internal virtual Color? _activePressedColor { get; set; } = default;
    internal virtual Color? _inactivePressedColor { get; set; } = default;
    internal virtual dynamic _activeThumbImage { get; set; } = default!;
    internal virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? _onActiveThumbImageError { get; set; } = default;
    internal virtual dynamic _inactiveThumbImage { get; set; } = default!;
    internal virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? _onInactiveThumbImageError { get; set; } = default;
    internal virtual Color? _activeTrackColor { get; set; } = default;
    internal virtual Color? _activeTrackOutlineColor { get; set; } = default;
    internal virtual Color? _inactiveTrackOutlineColor { get; set; } = default;
    internal virtual double? _activeTrackOutlineWidth { get; set; } = default;
    internal virtual double? _inactiveTrackOutlineWidth { get; set; } = default;
    internal virtual Color? _inactiveTrackColor { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.ImageConfiguration? _configuration { get; set; } = default;
    internal virtual TextDirection? _textDirection { get; set; } = default;
    internal virtual Color? _surfaceColor { get; set; } = default;
    internal virtual bool? _isInteractive { get; set; } = default;
    internal virtual double? _trackInnerLength { get; set; } = default;
    internal virtual (Color, Color)? _onOffLabelColors { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.TextPainter _textPainter { get; private set; } = new global::Doroti.Framework.Painting.TextPainter();
    internal virtual Color? _cachedThumbColor { get; set; } = default;
    internal virtual dynamic _cachedThumbImage { get; set; } = default!;
    internal virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? _cachedThumbErrorListener { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.BoxPainter? _cachedThumbPainter { get; set; } = default;
    internal virtual bool _isPainting { get; set; } = false;
    internal virtual bool _stopPressAnimation { get; set; } = false;
    internal virtual double? _pressedThumbExtension { get; set; } = default;

    public virtual global::Doroti.Framework.Animation.AnimationController positionController
    {
        get => this._positionController!;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._positionController)))
            {
                return;
            }
            _positionController = __value;
            this._colorAnimation?.dispose();
            _colorAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this.positionController, curve: global::Doroti.Framework.Animation.Curves.easeOut, reverseCurve: global::Doroti.Framework.Animation.Curves.easeIn);
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Widgets.Icon? activeIcon
    {
        get => this._activeIcon;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._activeIcon)))
            {
                return;
            }
            _activeIcon = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Widgets.Icon? inactiveIcon
    {
        get => this._inactiveIcon;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._inactiveIcon)))
            {
                return;
            }
            _inactiveIcon = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? iconTheme
    {
        get => this._iconTheme;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._iconTheme)))
            {
                return;
            }
            _iconTheme = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color activeIconColor
    {
        get => this._activeIconColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._activeIconColor)))
            {
                return;
            }
            _activeIconColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color inactiveIconColor
    {
        get => this._inactiveIconColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._inactiveIconColor)))
            {
                return;
            }
            _inactiveIconColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color activePressedColor
    {
        get => this._activePressedColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._activePressedColor)))
            {
                return;
            }
            _activePressedColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color inactivePressedColor
    {
        get => this._inactivePressedColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._inactivePressedColor)))
            {
                return;
            }
            _inactivePressedColor = __value;
            notifyListeners();
        }
    }
    public virtual dynamic activeThumbImage
    {
        get => this._activeThumbImage;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._activeThumbImage)))
            {
                return;
            }
            _activeThumbImage = __value;
            notifyListeners();
        }
    }
    public virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? onActiveThumbImageError
    {
        get => this._onActiveThumbImageError;
        set
        {
            var __value = value;
            if ((object.Equals((global::System.Action<object, global::System.Diagnostics.StackTrace?>?)__value, (global::System.Action<object, global::System.Diagnostics.StackTrace?>?)this._onActiveThumbImageError)))
            {
                return;
            }
            _onActiveThumbImageError = (global::System.Action<object, global::System.Diagnostics.StackTrace?>)__value;
            notifyListeners();
        }
    }
    public virtual dynamic inactiveThumbImage
    {
        get => this._inactiveThumbImage;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._inactiveThumbImage)))
            {
                return;
            }
            _inactiveThumbImage = __value;
            notifyListeners();
        }
    }
    public virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? onInactiveThumbImageError
    {
        get => this._onInactiveThumbImageError;
        set
        {
            var __value = value;
            if ((object.Equals((global::System.Action<object, global::System.Diagnostics.StackTrace?>?)__value, (global::System.Action<object, global::System.Diagnostics.StackTrace?>?)this._onInactiveThumbImageError)))
            {
                return;
            }
            _onInactiveThumbImageError = (global::System.Action<object, global::System.Diagnostics.StackTrace?>)__value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color activeTrackColor
    {
        get => this._activeTrackColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._activeTrackColor)))
            {
                return;
            }
            _activeTrackColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color? activeTrackOutlineColor
    {
        get => this._activeTrackOutlineColor;
        set
        {
            var __value = value is null ? null : (Color)(object)value;
            if ((object.Equals(__value, this._activeTrackOutlineColor)))
            {
                return;
            }
            _activeTrackOutlineColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color? inactiveTrackOutlineColor
    {
        get => this._inactiveTrackOutlineColor;
        set
        {
            var __value = value is null ? null : (Color)(object)value;
            if ((object.Equals(__value, this._inactiveTrackOutlineColor)))
            {
                return;
            }
            _inactiveTrackOutlineColor = __value;
            notifyListeners();
        }
    }
    public virtual double? activeTrackOutlineWidth
    {
        get => this._activeTrackOutlineWidth;
        set
        {
            var __value = value;
            if ((__value == this._activeTrackOutlineWidth))
            {
                return;
            }
            _activeTrackOutlineWidth = __value;
            notifyListeners();
        }
    }
    public virtual double? inactiveTrackOutlineWidth
    {
        get => this._inactiveTrackOutlineWidth;
        set
        {
            var __value = value;
            if ((__value == this._inactiveTrackOutlineWidth))
            {
                return;
            }
            _inactiveTrackOutlineWidth = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color inactiveTrackColor
    {
        get => this._inactiveTrackColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._inactiveTrackColor)))
            {
                return;
            }
            _inactiveTrackColor = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Painting.ImageConfiguration configuration
    {
        get => this._configuration!;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._configuration)))
            {
                return;
            }
            _configuration = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => DartRuntimePrimitives.RequireValue(this._textDirection);
        set
        {
            var __value = value;
            if ((object.Equals(this._textDirection, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _textDirection = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual global::Doroti.Ui.Color surfaceColor
    {
        get => this._surfaceColor!;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._surfaceColor)))
            {
                return;
            }
            _surfaceColor = __value;
            notifyListeners();
        }
    }
    public virtual bool isInteractive
    {
        get => DartRuntimePrimitives.RequireValue(this._isInteractive);
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._isInteractive))
            {
                return;
            }
            _isInteractive = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual double trackInnerLength
    {
        get => DartRuntimePrimitives.RequireValue(this._trackInnerLength);
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._trackInnerLength))
            {
                return;
            }
            _trackInnerLength = DartRuntimePrimitives.RequireValue(__value);
            notifyListeners();
        }
    }
    public virtual (global::Doroti.Ui.Color, global::Doroti.Ui.Color)? onOffLabelColors
    {
        get => this._onOffLabelColors;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._onOffLabelColors)))
            {
                return;
            }
            _onOffLabelColors = __value;
            notifyListeners();
        }
    }
    internal virtual global::Doroti.Framework.Painting.ShapeDecoration _createDefaultThumbDecoration(Color color, dynamic image, global::System.Action<object, global::System.Diagnostics.StackTrace?>? errorListener)
    {
        return new global::Doroti.Framework.Painting.ShapeDecoration(color: color, image: ((image is null) ? null : new global::Doroti.Framework.Painting.DecorationImage(image: image, onError: (global::System.Action<object, global::System.Diagnostics.StackTrace?>?)errorListener)), shape: new global::Doroti.Framework.Painting.StadiumBorder());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleDecorationChanged()
    {
        if (!this._isPainting)
        {
            notifyListeners();
        }
    }

    public virtual void paint(Canvas canvas, Size size)
    {
        double currentValue__36255 = ((global::Doroti.Framework.Animation.Animation<double>)this.position).value;
        double visualPosition__36304 = (this.textDirection switch { TextDirection.rtl => (1.0 - currentValue__36255), TextDirection.ltr => currentValue__36255, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        if (((object.Equals(((global::Doroti.Framework.Animation.Animation<double>)this.reaction).status, global::Doroti.Framework.Animation.AnimationStatus.reverse)) && !this._stopPressAnimation))
        {
            _stopPressAnimation = true;
        }
        else
        {
            _stopPressAnimation = false;
        }
        _pressedThumbExtension = (((global::Doroti.Framework.Animation.Animation<double>)this.reaction).value * SwitchLibrary._kThumbExtensionFactor);
        var thumbSize__36688 = new global::Doroti.Ui.Size(((SwitchLibrary._kThumbRadius * 2L) + DartRuntimePrimitives.RequireValue(this._pressedThumbExtension)), (SwitchLibrary._kThumbRadius * 2L));
        double colorValue__36788 = this._colorAnimation!.value;
        global::Doroti.Ui.Color trackColor__36841 = ((global::Doroti.Ui.Color)(object?)Dart_uiLibrary.Color.lerp(this.inactiveTrackColor, this.activeTrackColor, ((global::Doroti.Framework.Animation.Animation<double>)this.position).value)!);
        global::Doroti.Ui.Color? trackOutlineColor__36938 = ((global::Doroti.Ui.Color?)(object?)(((this.inactiveTrackOutlineColor is null) || (this.activeTrackOutlineColor is null)) ? null : Dart_uiLibrary.Color.lerp(this.inactiveTrackOutlineColor, this.activeTrackOutlineColor, colorValue__36788)));
        double? trackOutlineWidth__37154 = Dart_uiLibrary.lerpDouble(this.inactiveTrackOutlineWidth, this.activeTrackOutlineWidth, colorValue__36788);
        global::Doroti.Ui.Color lerpedThumbColor__37292 = default!;
        if (!((global::Doroti.Framework.Animation.Animation<double>)this.reaction).isDismissed)
        {
            lerpedThumbColor__37292 = Dart_uiLibrary.Color.lerp(this.inactivePressedColor, this.activePressedColor, colorValue__36788)!;
        }
        else
        {
            if ((object.Equals(((global::Doroti.Framework.Animation.AnimationController)this.positionController).status, global::Doroti.Framework.Animation.AnimationStatus.forward)))
            {
                lerpedThumbColor__37292 = Dart_uiLibrary.Color.lerp(this.inactivePressedColor, this.activeColor, colorValue__36788)!;
            }
            else
            {
                if ((object.Equals(((global::Doroti.Framework.Animation.AnimationController)this.positionController).status, global::Doroti.Framework.Animation.AnimationStatus.reverse)))
                {
                    lerpedThumbColor__37292 = Dart_uiLibrary.Color.lerp(this.inactiveColor, this.activePressedColor, colorValue__36788)!;
                }
                else
                {
                    lerpedThumbColor__37292 = Dart_uiLibrary.Color.lerp(this.inactiveColor, this.activeColor, colorValue__36788)!;
                }
            }
        }
        global::Doroti.Ui.Color thumbColor__38044 = ((global::Doroti.Ui.Color)(object?)Dart_uiLibrary.Color.alphaBlend(lerpedThumbColor__37292, this.surfaceColor));
        global::Doroti.Framework.Widgets.Icon? thumbIcon__38124 = ((currentValue__36255 < 0.5) ? this.inactiveIcon : this.activeIcon);
        dynamic thumbImage__38210 = ((currentValue__36255 < 0.5) ? this.inactiveThumbImage : this.activeThumbImage);
        global::System.Action<object, global::System.Diagnostics.StackTrace?>? thumbErrorListener__38314 = ((global::System.Action<object, global::System.Diagnostics.StackTrace?>)((currentValue__36255 < 0.5) ? this.onInactiveThumbImageError : this.onActiveThumbImageError));
        var paint__38436 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = trackColor__36841;
            return __cascade;        }))();
        global::Doroti.Ui.Offset trackPaintOffset__38491 = ((global::Doroti.Ui.Offset)(object?)_SwitchPainter__switch._computeTrackPaintOffset(size));
        global::Doroti.Ui.Offset thumbPaintOffset__38559 = ((global::Doroti.Ui.Offset)(object?)_computeThumbPaintOffset(trackPaintOffset__38491, thumbSize__36688, visualPosition__36304));
        var trackRect__38685 = global::Doroti.Ui.Rect.fromLTWH(trackPaintOffset__38491.dx, trackPaintOffset__38491.dy, SwitchLibrary._kTrackWidth, SwitchLibrary._kTrackHeight);
        _paintTrackWith(canvas, paint__38436, trackPaintOffset__38491, trackOutlineColor__36938, trackOutlineWidth__37154, trackRect__38685);
        double currentReactionValue__38979 = ((global::Doroti.Framework.Animation.Animation<double>)this.reaction).value;
        if ((this._onOffLabelColors is not null))
        {
            var (onLabelColor__39074, offLabelColor__39094) = DartRuntimePrimitives.RequireValue(this.onOffLabelColors);
            double leftLabelOpacity__39150 = (visualPosition__36304 * ((1.0 - currentReactionValue__38979)));
            double rightLabelOpacity__39235 = (((1.0 - visualPosition__36304)) * ((1.0 - currentReactionValue__38979)));
            var (onLabelOpacity__39330, offLabelOpacity__39353) = (this.textDirection switch { TextDirection.ltr => (((double, double))((leftLabelOpacity__39150, rightLabelOpacity__39235))), TextDirection.rtl => (((double, double))((rightLabelOpacity__39235, leftLabelOpacity__39150))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            var (onLabelOffset__39563, offLabelOffset__39585) = (this.textDirection switch { TextDirection.ltr => (((Offset, Offset))((trackRect__38685.centerLeft.translate(SwitchLibrary._kOnLabelPaddingHorizontal, 0), trackRect__38685.centerRight.translate(-SwitchLibrary._kOffLabelPaddingHorizontal, 0)))), TextDirection.rtl => (((Offset, Offset))((trackRect__38685.centerRight.translate(-SwitchLibrary._kOnLabelPaddingHorizontal, 0), trackRect__38685.centerLeft.translate(SwitchLibrary._kOffLabelPaddingHorizontal, 0)))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            var onLabelRect__40058 = global::Doroti.Ui.Rect.fromCenter(center: onLabelOffset__39563, width: SwitchLibrary._kOnLabelWidth, height: SwitchLibrary._kOnLabelHeight);
            var onLabelPaint__40205 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = onLabelColor__39074.withOpacity(onLabelOpacity__39330);
            __cascade.style = PaintingStyle.fill;
            return __cascade;        }))();
            canvas.drawRect(onLabelRect__40058, onLabelPaint__40205);
            var offLabelPaint__40414 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = offLabelColor__39094.withOpacity(offLabelOpacity__39353);
            __cascade.style = PaintingStyle.stroke;
            __cascade.strokeWidth = SwitchLibrary._kOffLabelWidth;
            return __cascade;        }))();
            canvas.drawCircle(offLabelOffset__39585, SwitchLibrary._kOffLabelRadius, offLabelPaint__40414);
        }
        _paintThumbWith(thumbPaintOffset__38559, canvas, colorValue__36788, thumbColor__38044, thumbImage__38210, (global::System.Action<object, global::System.Diagnostics.StackTrace?>?)thumbErrorListener__38314, thumbIcon__38124, thumbSize__36688);
    }

    internal static global::Doroti.Ui.Offset _computeTrackPaintOffset(Size canvasSize)
    {
        double horizontalOffset__40981 = (((canvasSize.width - SwitchLibrary._kTrackWidth)) / 2.0);
        double verticalOffset__41058 = (((canvasSize.height - SwitchLibrary._kTrackHeight)) / 2.0);
        return new global::Doroti.Ui.Offset(horizontalOffset__40981, verticalOffset__41058);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _computeThumbPaintOffset(Offset trackPaintOffset, Size thumbSize, double visualPosition)
    {
        double trackRadius__41442 = (SwitchLibrary._kTrackHeight / 2L);
        double additionalThumbRadius__41492 = ((thumbSize.height / 2L) - trackRadius__41442);
        double horizontalProgress__41570 = (visualPosition * ((this.trackInnerLength - DartRuntimePrimitives.RequireValue(this._pressedThumbExtension))));
        double thumbHorizontalOffset__41671 = ((((trackPaintOffset.dx + trackRadius__41442) + ((DartRuntimePrimitives.RequireValue(this._pressedThumbExtension) / 2L))) - (thumbSize.width / 2L)) + horizontalProgress__41570);
        double thumbVerticalOffset__41862 = (trackPaintOffset.dy - additionalThumbRadius__41492);
        return new global::Doroti.Ui.Offset(thumbHorizontalOffset__41671, thumbVerticalOffset__41862);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paintTrackWith(Canvas canvas, Paint paint, Offset trackPaintOffset, Color? trackOutlineColor, double? trackOutlineWidth, Rect trackRect)
    {
        double trackRadius__42190 = (SwitchLibrary._kTrackHeight / 2L);
        var trackRRect__42233 = global::Doroti.Ui.RRect.fromRectAndRadius(trackRect, global::Doroti.Ui.Radius.circular(trackRadius__42190));
        canvas.drawRRect(trackRRect__42233, paint);
        if ((trackOutlineColor is not null))
        {
            var outlineTrackRect__42442 = global::Doroti.Ui.Rect.fromLTWH((trackPaintOffset.dx + 1L), (trackPaintOffset.dy + 1L), (SwitchLibrary._kTrackWidth - 2L), (SwitchLibrary._kTrackHeight - 2L));
            var outlineTrackRRect__42616 = global::Doroti.Ui.RRect.fromRectAndRadius(outlineTrackRect__42442, global::Doroti.Ui.Radius.circular(trackRadius__42190));
            var outlinePaint__42753 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.style = PaintingStyle.stroke;
            __cascade.strokeWidth = (trackOutlineWidth ?? 2.0);
            __cascade.color = trackOutlineColor;
            return __cascade;        }))();
            canvas.drawRRect(outlineTrackRRect__42616, outlinePaint__42753);
        }
        if (this.isFocused)
        {
            global::Doroti.Ui.RRect focusedOutline__43005 = ((global::Doroti.Ui.RRect)(object?)trackRRect__42233.inflate(1.75));
            var focusedPaint__43060 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.style = PaintingStyle.stroke;
            __cascade.color = this.focusColor;
            __cascade.strokeWidth = 3.5;
            return __cascade;        }))();
            canvas.drawRRect(focusedOutline__43005, focusedPaint__43060);
        }
        canvas.clipRRect(trackRRect__42233);
    }

    internal virtual void _paintThumbWith(Offset thumbPaintOffset, Canvas canvas, double currentValue, Color thumbColor, dynamic thumbImage, global::System.Action<object, global::System.Diagnostics.StackTrace?>? thumbErrorListener, global::Doroti.Framework.Widgets.Icon? thumbIcon, Size thumbSize)
    {
        try
        {
            _isPainting = true;
            if (((((this._cachedThumbPainter is null) || (!object.Equals(thumbColor, this._cachedThumbColor))) || (!object.Equals(thumbImage, this._cachedThumbImage))) || (!object.Equals((global::System.Action<object, global::System.Diagnostics.StackTrace?>?)thumbErrorListener, (global::System.Action<object, global::System.Diagnostics.StackTrace?>?)this._cachedThumbErrorListener))))
            {
                _cachedThumbColor = thumbColor;
                _cachedThumbImage = thumbImage;
                _cachedThumbErrorListener = (global::System.Action<object, global::System.Diagnostics.StackTrace?>)thumbErrorListener;
                this._cachedThumbPainter?.dispose();
                _cachedThumbPainter = _createDefaultThumbDecoration(thumbColor, thumbImage, (global::System.Action<object, global::System.Diagnostics.StackTrace?>?)thumbErrorListener).createBoxPainter((global::System.Action)(() => this._handleDecorationChanged()));
            }
            global::Doroti.Framework.Painting.BoxPainter thumbPainter__44144 = this._cachedThumbPainter!;
            _paintCupertinoThumbShadowAndBorder(canvas, thumbPaintOffset, thumbSize);
            thumbPainter__44144.paint(canvas, thumbPaintOffset, this.configuration.copyWith(size: thumbSize));
            if (((thumbIcon is not null) && (((global::Doroti.Framework.Widgets.Icon)thumbIcon).icon is not null)))
            {
                global::Doroti.Ui.Color iconColor__44434 = ((global::Doroti.Ui.Color)(object?)Dart_uiLibrary.Color.lerp(this.inactiveIconColor, this.activeIconColor, currentValue)!);
                double iconSize__44530 = (((global::Doroti.Framework.Widgets.Icon)thumbIcon).size ?? 16.0);
                global::Doroti.Framework.Widgets.IconData iconData__44588 = ((global::Doroti.Framework.Widgets.Icon)thumbIcon).icon!;
                double? iconWeight__44638 = (((global::Doroti.Framework.Widgets.Icon)thumbIcon).weight ?? this.iconTheme?.weight);
                double? iconFill__44712 = (((global::Doroti.Framework.Widgets.Icon)thumbIcon).fill ?? this.iconTheme?.fill);
                double? iconGrade__44780 = (((global::Doroti.Framework.Widgets.Icon)thumbIcon).grade ?? this.iconTheme?.grade);
                double? iconOpticalSize__44851 = (((global::Doroti.Framework.Widgets.Icon)thumbIcon).opticalSize ?? this.iconTheme?.opticalSize);
                List<global::Doroti.Ui.Shadow>? iconShadows__44946 = (((global::Doroti.Framework.Widgets.Icon)thumbIcon).shadows ?? this.iconTheme?.shadows).ToList();
                var textSpan__45016 = new global::Doroti.Framework.Painting.TextSpan(text: char.ConvertFromUtf32(checked((int)((global::Doroti.Framework.Widgets.IconData)iconData__44588).codePoint)), style: new global::Doroti.Framework.Painting.TextStyle(fontVariations: ((Func<List<global::Doroti.Ui.FontVariation>>)(() => { var __collection45150 = new List<global::Doroti.Ui.FontVariation>(); if ((iconFill__44712 is not null)) { __collection45150.Add(new global::Doroti.Ui.FontVariation("FILL", DartRuntimePrimitives.RequireValue(iconFill__44712))); } if ((iconWeight__44638 is not null)) { __collection45150.Add(new global::Doroti.Ui.FontVariation("wght", DartRuntimePrimitives.RequireValue(iconWeight__44638))); } if ((iconGrade__44780 is not null)) { __collection45150.Add(new global::Doroti.Ui.FontVariation("GRAD", DartRuntimePrimitives.RequireValue(iconGrade__44780))); } if ((iconOpticalSize__44851 is not null)) { __collection45150.Add(new global::Doroti.Ui.FontVariation("opsz", DartRuntimePrimitives.RequireValue(iconOpticalSize__44851))); } return __collection45150; }))(), color: iconColor__44434, fontSize: iconSize__44530, inherit: false, fontFamily: ((global::Doroti.Framework.Widgets.IconData)iconData__44588).fontFamily, package: ((global::Doroti.Framework.Widgets.IconData)iconData__44588).fontPackage, shadows: iconShadows__44946));
                DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{            var __cascade = this._textPainter;
            __cascade.textDirection = this.textDirection;
            __cascade.text = textSpan__45016;
            return __cascade;        }))());
                this._textPainter.layout();
                double additionalHorizontalOffset__45858 = (((thumbSize.width - iconSize__44530)) / 2L);
                double additionalVerticalOffset__45942 = (((thumbSize.height - iconSize__44530)) / 2L);
                global::Doroti.Ui.Offset offset__46025 = ((global::Doroti.Ui.Offset)(object?)(thumbPaintOffset + new global::Doroti.Ui.Offset(additionalHorizontalOffset__45858, additionalVerticalOffset__45942)));
                this._textPainter.paint(canvas, offset__46025);
            }
        }
        finally
        {
            _isPainting = false;
        }
    }

    internal virtual void _paintCupertinoThumbShadowAndBorder(Canvas canvas, Offset thumbPaintOffset, Size thumbSize)
    {
        var thumbBounds__46345 = global::Doroti.Ui.RRect.fromLTRBR(thumbPaintOffset.dx, thumbPaintOffset.dy, (thumbPaintOffset.dx + thumbSize.width), (thumbPaintOffset.dy + thumbSize.height), global::Doroti.Ui.Radius.circular((thumbSize.height / 2.0)));
        foreach (global::Doroti.Framework.Painting.BoxShadow shadow__46600 in SwitchLibrary._kSwitchBoxShadows)
        {
            canvas.drawRRect(thumbBounds__46345.shift(shadow__46600.offset), shadow__46600.toPaint());
        }
        canvas.drawRRect(thumbBounds__46345.inflate(0.5), ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = new global::Doroti.Ui.Color(167772160L);
            return __cascade;        }))());
    }

    public override void dispose()
    {
        this._textPainter.dispose();
        this._cachedThumbPainter?.dispose();
        _cachedThumbPainter = null;
        _cachedThumbColor = null;
        _cachedThumbImage = null;
        _cachedThumbErrorListener = null;
        this._colorAnimation?.dispose();
        base.dispose();
    }

}
