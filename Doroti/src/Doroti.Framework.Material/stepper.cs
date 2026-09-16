// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/stepper.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public enum StepState
{
    indexed,
    editing,
    complete,
    disabled,
    error
}

public enum StepperType
{
    vertical,
    horizontal
}

public class ControlsDetails
{
    public virtual long currentStep { get; private set; } = default!;
    public virtual long stepIndex { get; private set; } = default!;
    public virtual global::System.Action? onStepContinue { get; private set; }
    public virtual global::System.Action? onStepCancel { get; private set; }

    public ControlsDetails(long currentStep, long stepIndex, global::System.Action? onStepCancel = null, global::System.Action? onStepContinue = null)
    {
        this.currentStep = currentStep;
        this.stepIndex = stepIndex;
        this.onStepCancel = onStepCancel;
        this.onStepContinue = onStepContinue;
    }

    public virtual bool isActive => DartRuntimePrimitives.ConvertValue<bool>(currentStep == stepIndex);
}

public delegate global::Doroti.Framework.Widgets.Widget ControlsWidgetBuilder(global::Doroti.Framework.Widgets.BuildContext context, ControlsDetails details);

public delegate global::Doroti.Framework.Widgets.Widget? StepIconBuilder(long stepIndex, StepState stepState);

public static partial class StepperLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _kStepStyle = new global::Doroti.Framework.Painting.TextStyle(fontSize: 12.0, color: Colors.white);
}

public static partial class StepperLibrary
{
    internal static Color _kErrorLight = Colors.red;
}

public static partial class StepperLibrary
{
    internal static Color _kErrorDark = Colors.red.shade400;
}

public static partial class StepperLibrary
{
    internal static Color _kCircleActiveLight = Colors.white;
}

public static partial class StepperLibrary
{
    internal static Color _kCircleActiveDark = Colors.black87;
}

public static partial class StepperLibrary
{
    internal static Color _kDisabledLight = Colors.black38;
}

public static partial class StepperLibrary
{
    internal static Color _kDisabledDark = Colors.white38;
}

public static partial class StepperLibrary
{
    internal static double _kStepSize = 24.0;
}

public static partial class StepperLibrary
{
    internal static double _kTriangleSqrt = 0.866025;
}

public static partial class StepperLibrary
{
    internal static double _kTriangleHeight = _kStepSize * _kTriangleSqrt;
}

public static partial class StepperLibrary
{
    internal static double _kMaxStepSize = 80.0;
}

public static partial class StepperLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsetsDirectional _kDefaultVerticalContentPadding = EdgeInsetsDirectional.CreateOnly(start: 60.0, end: 24.0, bottom: 24.0);
}

public static partial class StepperLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsets _kDefaultHorizontalContentPadding = EdgeInsets.CreateAll(24.0);
}

public static partial class StepperLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsetsGeometry _kDefaultHeaderPadding = EdgeInsets.CreateSymmetric(horizontal: 24.0);
}

public class Step
{
    public virtual global::Doroti.Framework.Widgets.Widget title { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? subtitle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget content { get; private set; } = default!;
    public virtual StepState state { get; private set; } = default!;
    public virtual bool isActive { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? label { get; private set; }
    public virtual StepStyle? stepStyle { get; private set; }

    public Step(global::Doroti.Framework.Widgets.Widget title, global::Doroti.Framework.Widgets.Widget? subtitle = null, global::Doroti.Framework.Widgets.Widget content = default!, StepState state = StepState.indexed, bool isActive = false, global::Doroti.Framework.Widgets.Widget? label = null, StepStyle? stepStyle = null)
    {
        this.title = title;
        this.subtitle = subtitle;
        this.content = content;
        this.state = state;
        this.isActive = isActive;
        this.label = label;
        this.stepStyle = stepStyle;
    }

}

public class Stepper : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual List<Step> steps { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.ScrollPhysics? physics { get; private set; }
    public virtual global::Doroti.Framework.Widgets.ScrollController? controller { get; private set; }
    public virtual StepperType type { get; private set; } = default!;
    public virtual long currentStep { get; private set; } = default!;
    public virtual global::System.Action<long>? onStepTapped { get; private set; }
    public virtual global::System.Action? onStepContinue { get; private set; }
    public virtual global::System.Action? onStepCancel { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, ControlsDetails, global::Doroti.Framework.Widgets.Widget>? controlsBuilder { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color>? connectorColor { get; private set; }
    public virtual double? connectorThickness { get; private set; }
    public virtual global::System.Func<long, StepState, global::Doroti.Framework.Widgets.Widget?>? stepIconBuilder { get; private set; }
    public virtual double? stepIconHeight { get; private set; }
    public virtual double? stepIconWidth { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsets? stepIconMargin { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? headerPadding { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding { get; private set; }

    public Stepper(global::Doroti.Framework.Foundation.Key? key = null, List<Step> steps = default!, global::Doroti.Framework.Widgets.ScrollController? controller = null, global::Doroti.Framework.Widgets.ScrollPhysics? physics = null, StepperType type = StepperType.vertical, long currentStep = 0, global::System.Action<long>? onStepTapped = null, global::System.Action? onStepContinue = null, global::System.Action? onStepCancel = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, ControlsDetails, global::Doroti.Framework.Widgets.Widget>? controlsBuilder = null, double? elevation = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color>? connectorColor = null, double? connectorThickness = null, global::System.Func<long, StepState, global::Doroti.Framework.Widgets.Widget?>? stepIconBuilder = null, double? stepIconHeight = null, double? stepIconWidth = null, global::Doroti.Framework.Painting.EdgeInsets? stepIconMargin = null, Clip clipBehavior = Clip.none, global::Doroti.Framework.Painting.EdgeInsetsGeometry? headerPadding = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding = null) : base(key: key)
    {
        this.steps = steps;
        this.controller = controller;
        this.physics = physics;
        this.type = type;
        this.currentStep = currentStep;
        this.onStepTapped = onStepTapped;
        this.onStepContinue = onStepContinue;
        this.onStepCancel = onStepCancel;
        this.controlsBuilder = controlsBuilder;
        this.elevation = elevation;
        this.margin = margin;
        this.connectorColor = connectorColor;
        this.connectorThickness = connectorThickness;
        this.stepIconBuilder = stepIconBuilder;
        this.stepIconHeight = stepIconHeight;
        this.stepIconWidth = stepIconWidth;
        this.stepIconMargin = stepIconMargin;
        this.clipBehavior = clipBehavior;
        this.headerPadding = headerPadding;
        this.contentPadding = contentPadding;
        System.Diagnostics.Debug.Assert((0L <= currentStep) && (currentStep < checked(steps.Count)));
        System.Diagnostics.Debug.Assert((stepIconHeight is null) || (stepIconHeight >= StepperLibrary._kStepSize) && (stepIconHeight <= StepperLibrary._kMaxStepSize));
        System.Diagnostics.Debug.Assert((stepIconWidth is null) || (stepIconWidth >= StepperLibrary._kStepSize) && (stepIconWidth <= StepperLibrary._kMaxStepSize));
        System.Diagnostics.Debug.Assert((stepIconHeight is null) || (stepIconWidth is null) || (DartRuntimePrimitives.RequireValue(stepIconHeight) == DartRuntimePrimitives.RequireValue(stepIconWidth)));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _StepperState__stepper());
}

internal class _StepperState__stepper : global::Doroti.Framework.Widgets.State<Stepper>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<Stepper>
{
    internal virtual List<global::Doroti.Framework.Widgets.GlobalKey<IState>> _keys { get; set; } = default!;
    internal virtual DartMap<long, StepState> _oldStates { get; private set; } = new DartMap<long, StepState>();
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _keys = DartRuntimePrimitives.CreateList<global::Doroti.Framework.Widgets.GlobalKey<IState>>(checked(widget.steps.Count), (i) => GlobalKey<IState>.Create());
        for (var iLocal = 0L; iLocal < checked(widget.steps.Count); iLocal += 1L)
        {
            _oldStates[iLocal] = widget.steps[(int)iLocal].state;
        }
    }

    public override void didUpdateWidget(Stepper oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() => checked(widget.steps.Count) == checked((long)oldWidget.steps.Count));
        for (var i = 0L; i < checked(oldWidget.steps.Count); i += 1L)
        {
            _oldStates[i] = oldWidget.steps[(int)i].state;
        }
    }

    internal virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? _stepIconMargin => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(widget.stepIconMargin);
    internal virtual double? _stepIconHeight => widget.stepIconHeight;
    internal virtual double? _stepIconWidth => widget.stepIconWidth;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry effectiveHeaderPadding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(widget.headerPadding ?? StepperLibrary._kDefaultHeaderPadding);
    internal virtual double _heightFactor
    {
        get
        {
            return (_isLabel() && (_stepIconHeight is not null)) ? 2.5 : 2.0;
        }
    }
    internal virtual bool _isFirst(long index)
    {
        return index == 0L;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isLast(long index)
    {
        return (checked(widget.steps.Count) - 1L) == index;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isCurrent(long index)
    {
        return widget.currentStep == index;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isDark()
    {
        return Equals(Theme.brightnessOf(context), Brightness.dark);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isLabel()
    {
        return widget.steps.any((step) => step.label is not null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual StepStyle? _stepStyle(long index)
    {
        return widget.steps[(int)index].stepStyle;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Color _connectorColor(bool isActive)
    {
        ColorScheme colorSchemeLocal = Theme.of(context).colorScheme;
        var states = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection15710 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (isActive) { __collection15710.Add(WidgetState.selected); } else { __collection15710.Add(WidgetState.disabled); } return __collection15710; }))();
        global::Doroti.Ui.Color? resolvedConnectorColor = widget.connectorColor?.resolve(states);
        return resolvedConnectorColor ?? (isActive ? colorSchemeLocal.primary : Colors.grey.shade400);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildLine(bool visible, bool isActive)
    {
        return new global::Doroti.Framework.Widgets.ColoredBox(color: _connectorColor(isActive), child: new global::Doroti.Framework.Widgets.SizedBox(width: visible ? (widget.connectorThickness ?? 1.0) : 0.0, height: 16.0));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildCircleChild(long index, bool oldState)
    {
        StepState stateLocal = oldState ? DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<StepState>(_oldStates, index)) : widget.steps[(int)index].state;
        if (widget.stepIconBuilder?.Invoke(index, stateLocal) is global::Doroti.Framework.Widgets.Widget icon)
        {
            return icon;
        }
        global::Doroti.Framework.Painting.TextStyle? textStyle = _stepStyle(index)?.indexStyle;
        bool isDarkActive = _isDark() && widget.steps[(int)index].isActive;
        global::Doroti.Ui.Color iconColor = isDarkActive ? StepperLibrary._kCircleActiveDark : StepperLibrary._kCircleActiveLight;
        textStyle ??= (isDarkActive ? StepperLibrary._kStepStyle.copyWith(color: Colors.black87) : StepperLibrary._kStepStyle);
        return stateLocal switch { StepState.indexed => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Text($"{index + 1L}", style: textStyle)), StepState.disabled => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Text($"{index + 1L}", style: textStyle)), StepState.editing => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Icon(Icons.edit, color: iconColor, size: 18.0)), StepState.complete => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Icon(Icons.check, color: iconColor, size: 18.0)), StepState.error => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Center(child: new global::Doroti.Framework.Widgets.Text("!", style: StepperLibrary._kStepStyle))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Color _circleColor(long index)
    {
        bool isActiveLocal = widget.steps[(int)index].isActive;
        ColorScheme colorSchemeLocal = Theme.of(context).colorScheme;
        var states = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection17276 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (isActiveLocal) { __collection17276.Add(WidgetState.selected); } else { __collection17276.Add(WidgetState.disabled); } return __collection17276; }))();
        global::Doroti.Ui.Color? resolvedConnectorColor = widget.connectorColor?.resolve(states);
        if (resolvedConnectorColor is not null)
        {
            return resolvedConnectorColor;
        }
        if (!_isDark())
        {
            return isActiveLocal ? colorSchemeLocal.primary : colorSchemeLocal.onSurface.withOpacity(0.38);
        }
        else
        {
            return isActiveLocal ? colorSchemeLocal.secondary : colorSchemeLocal.background;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildCircle(long index, bool oldState)
    {
        return new global::Doroti.Framework.Widgets.Padding(padding: _stepIconMargin ?? EdgeInsets.CreateSymmetric(vertical: 8.0), child: new global::Doroti.Framework.Widgets.SizedBox(width: _stepIconWidth ?? StepperLibrary._kStepSize, height: _stepIconHeight ?? StepperLibrary._kStepSize, child: new global::Doroti.Framework.Widgets.AnimatedContainer(curve: Curves.fastOutSlowIn, duration: ThemeLibrary.kThemeAnimationDuration, decoration: new global::Doroti.Framework.Painting.BoxDecoration(color: _stepStyle(index)?.color ?? _circleColor(index), shape: BoxShape.circle, border: _stepStyle(index)?.border, boxShadow: (_stepStyle(index)?.boxShadow is not null) ? new List<global::Doroti.Framework.Painting.BoxShadow> { _stepStyle(index)!.boxShadow! } : null, gradient: _stepStyle(index)?.gradient), child: new global::Doroti.Framework.Widgets.Center(child: _buildCircleChild(index, oldState && Equals(widget.steps[(int)index].state, StepState.error))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildTriangle(long index, bool oldState)
    {
        global::Doroti.Ui.Color? colorLocal = _stepStyle(index)?.errorColor;
        colorLocal ??= (_isDark() ? StepperLibrary._kErrorDark : StepperLibrary._kErrorLight);
        return new global::Doroti.Framework.Widgets.Padding(padding: _stepIconMargin ?? EdgeInsets.CreateSymmetric(vertical: 8.0), child: new global::Doroti.Framework.Widgets.SizedBox(width: _stepIconWidth ?? StepperLibrary._kStepSize, height: _stepIconHeight ?? StepperLibrary._kStepSize, child: new global::Doroti.Framework.Widgets.Center(child: new global::Doroti.Framework.Widgets.SizedBox(width: _stepIconWidth ?? StepperLibrary._kStepSize, height: (_stepIconHeight is not null) ? (DartRuntimePrimitives.RequireValue(_stepIconHeight) * StepperLibrary._kTriangleSqrt) : StepperLibrary._kTriangleHeight, child: new global::Doroti.Framework.Widgets.CustomPaint(painter: new _TrianglePainter__stepper(color: colorLocal), child: new global::Doroti.Framework.Widgets.Align(alignment: new global::Doroti.Framework.Painting.Alignment(0.0, 0.8), child: _buildCircleChild(index, oldState && (!Equals(widget.steps[(int)index].state, StepState.error)))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildIcon(long index)
    {
        if (!Equals(widget.steps[(int)index].state, DartCollectionRuntime.NullableMapValue<StepState>(_oldStates, index)))
        {
            return new global::Doroti.Framework.Widgets.AnimatedCrossFade(firstChild: _buildCircle(index, true), secondChild: _buildTriangle(index, true), firstCurve: new global::Doroti.Framework.Animation.Interval(0.0, 0.6, curve: Curves.fastOutSlowIn), secondCurve: new global::Doroti.Framework.Animation.Interval(0.4, 1.0, curve: Curves.fastOutSlowIn), sizeCurve: Curves.fastOutSlowIn, crossFadeState: Equals(widget.steps[(int)index].state, StepState.error) ? CrossFadeState.showSecond : CrossFadeState.showFirst, duration: ThemeLibrary.kThemeAnimationDuration);
        }
        else
        {
            if (!Equals(widget.steps[(int)index].state, StepState.error))
            {
                return _buildCircle(index, false);
            }
            else
            {
                return _buildTriangle(index, false);
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildVerticalControls(long stepIndex)
    {
        if (widget.controlsBuilder is not null)
        {
            return widget.controlsBuilder!(context, new ControlsDetails(currentStep: widget.currentStep, onStepContinue: widget.onStepContinue, onStepCancel: widget.onStepCancel, stepIndex: stepIndex));
        }
        global::Doroti.Ui.Color cancelColor = Theme.brightnessOf(context) switch { Brightness.light => Colors.black54, Brightness.dark => Colors.white70, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        ThemeData themeData = Theme.of(context);
        ColorScheme colorSchemeLocal = themeData.colorScheme;
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        global::Doroti.Framework.Painting.OutlinedBorder buttonShape = new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(2)));
        var buttonPadding = EdgeInsets.CreateSymmetric(horizontal: 16.0);
        return new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateOnly(top: 16.0), child: new global::Doroti.Framework.Widgets.SizedBox(height: 48.0, child: new global::Doroti.Framework.Widgets.Row(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new TextButton(onPressed: widget.onStepContinue, style: new ButtonStyle(foregroundColor: WidgetStateProperty.resolveWith<global::Doroti.Ui.Color?>((states) => {
return states.Contains(WidgetState.disabled) ? null : (_isDark() ? colorSchemeLocal.onSurface : colorSchemeLocal.onPrimary);
throw new InvalidOperationException("Dart closure completed without a value.");
}), backgroundColor: WidgetStateProperty.resolveWith<global::Doroti.Ui.Color?>((states) => {
return (_isDark() || states.Contains(WidgetState.disabled)) ? null : colorSchemeLocal.primary;
throw new InvalidOperationException("Dart closure completed without a value.");
}), padding: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(buttonPadding), shape: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.OutlinedBorder>(buttonShape)), child: new global::Doroti.Framework.Widgets.Text(localizations.continueButtonLabel))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsetsDirectional.CreateOnly(start: 8.0), child: new TextButton(onPressed: widget.onStepCancel, style: TextButton.styleFrom(foregroundColor: cancelColor, padding: buttonPadding, shape: buttonShape), child: new global::Doroti.Framework.Widgets.Text(localizations.cancelButtonLabel)))) })));
    }

    internal virtual global::Doroti.Framework.Painting.TextStyle _titleStyle(long index)
    {
        ThemeData themeData = Theme.of(context);
        TextTheme textThemeLocal = themeData.textTheme;
        switch (widget.steps[(int)index].state)
        {
            case StepState.indexed:
            case StepState.editing:
            case StepState.complete:
                {
                    return textThemeLocal.bodyLarge!;
                }
            case StepState.disabled:
                {
                    return textThemeLocal.bodyLarge!.copyWith(color: _isDark() ? StepperLibrary._kDisabledDark : StepperLibrary._kDisabledLight);
                }
            case StepState.error:
                {
                    return textThemeLocal.bodyLarge!.copyWith(color: _isDark() ? StepperLibrary._kErrorDark : StepperLibrary._kErrorLight);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.TextStyle _subtitleStyle(long index)
    {
        ThemeData themeData = Theme.of(context);
        TextTheme textThemeLocal = themeData.textTheme;
        switch (widget.steps[(int)index].state)
        {
            case StepState.indexed:
            case StepState.editing:
            case StepState.complete:
                {
                    return textThemeLocal.bodySmall!;
                }
            case StepState.disabled:
                {
                    return textThemeLocal.bodySmall!.copyWith(color: _isDark() ? StepperLibrary._kDisabledDark : StepperLibrary._kDisabledLight);
                }
            case StepState.error:
                {
                    return textThemeLocal.bodySmall!.copyWith(color: _isDark() ? StepperLibrary._kErrorDark : StepperLibrary._kErrorLight);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.TextStyle _labelStyle(long index)
    {
        ThemeData themeData = Theme.of(context);
        TextTheme textThemeLocal = themeData.textTheme;
        switch (widget.steps[(int)index].state)
        {
            case StepState.indexed:
            case StepState.editing:
            case StepState.complete:
                {
                    return textThemeLocal.bodyLarge!;
                }
            case StepState.disabled:
                {
                    return textThemeLocal.bodyLarge!.copyWith(color: _isDark() ? StepperLibrary._kDisabledDark : StepperLibrary._kDisabledLight);
                }
            case StepState.error:
                {
                    return textThemeLocal.bodyLarge!.copyWith(color: _isDark() ? StepperLibrary._kErrorDark : StepperLibrary._kErrorLight);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildHeaderText(long index)
    {
        return new global::Doroti.Framework.Widgets.Column(crossAxisAlignment: CrossAxisAlignment.start, mainAxisSize: MainAxisSize.min, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection25461 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection25461.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnimatedDefaultTextStyle(style: _titleStyle(index), duration: ThemeLibrary.kThemeAnimationDuration, curve: Curves.fastOutSlowIn, child: widget.steps[(int)index].title))); if (widget.steps[(int)index].subtitle is not null) { __collection25461.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateOnly(top: 2.0), child: new global::Doroti.Framework.Widgets.AnimatedDefaultTextStyle(style: _subtitleStyle(index), duration: ThemeLibrary.kThemeAnimationDuration, curve: Curves.fastOutSlowIn, child: widget.steps[(int)index].subtitle!)))); } return __collection25461; }))());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildLabelText(long index)
    {
        if (widget.steps[(int)index].label is not null)
        {
            return new global::Doroti.Framework.Widgets.AnimatedDefaultTextStyle(style: _labelStyle(index), duration: ThemeLibrary.kThemeAnimationDuration, child: widget.steps[(int)index].label!);
        }
        return SizedBox.CreateShrink();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildVerticalHeader(long index)
    {
        bool isActiveLocal = widget.steps[(int)index].isActive;
        bool isPreviousActive = (index > 0L) && widget.steps[(int)(index - 1L)].isActive;
        return new global::Doroti.Framework.Widgets.Padding(padding: effectiveHeaderPadding, child: new global::Doroti.Framework.Widgets.Row(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Column(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildLine(!_isFirst(index), isPreviousActive)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildIcon(index)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildLine(!_isLast(index), isActiveLocal)) })), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsetsDirectional.CreateOnly(start: 12.0), child: _buildHeaderText(index)))) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildVerticalBody(long index)
    {
        double? marginLeft = _stepIconMargin?.resolve(TextDirection.ltr).left;
        double? marginRight = _stepIconMargin?.resolve(TextDirection.ltr).right;
        double? additionalMarginLeft = (marginLeft is not null) ? (DartRuntimePrimitives.RequireValue(marginLeft) / 2.0) : null;
        double? additionalMarginRight = (marginRight is not null) ? (DartRuntimePrimitives.RequireValue(marginRight) / 2.0) : null;
        global::Doroti.Framework.Painting.EdgeInsetsGeometry effectiveVerticalContentPadding = (widget.contentPadding ?? StepperLibrary._kDefaultVerticalContentPadding).add(EdgeInsetsDirectional.CreateOnly(start: marginLeft ?? 0.0));
        return new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.PositionedDirectional(start: 24.0 + (additionalMarginLeft ?? 0.0) + (additionalMarginRight ?? 0.0), top: 0.0, bottom: 0.0, width: _stepIconWidth ?? StepperLibrary._kStepSize, child: new global::Doroti.Framework.Widgets.Center(child: new global::Doroti.Framework.Widgets.SizedBox(width: !_isLast(index) ? (widget.connectorThickness ?? 1.0) : 0.0, height: double.PositiveInfinity, child: new global::Doroti.Framework.Widgets.ColoredBox(color: _connectorColor(widget.steps[(int)index].isActive)))))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnimatedCrossFade(firstChild: new global::Doroti.Framework.Widgets.SizedBox(width: double.PositiveInfinity, height: 0), secondChild: new global::Doroti.Framework.Widgets.Padding(padding: effectiveVerticalContentPadding, child: new global::Doroti.Framework.Widgets.Column(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ClipRect(clipBehavior: widget.clipBehavior, child: widget.steps[(int)index].content)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildVerticalControls(index)) })), firstCurve: new global::Doroti.Framework.Animation.Interval(0.0, 0.6, curve: Curves.fastOutSlowIn), secondCurve: new global::Doroti.Framework.Animation.Interval(0.4, 1.0, curve: Curves.fastOutSlowIn), sizeCurve: Curves.fastOutSlowIn, crossFadeState: _isCurrent(index) ? CrossFadeState.showSecond : CrossFadeState.showFirst, duration: ThemeLibrary.kThemeAnimationDuration)) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildVertical()
    {
        return new global::Doroti.Framework.Widgets.ListView(controller: widget.controller, shrinkWrap: true, physics: widget.physics, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() =>
        {
            var __collection29851 = new List<global::Doroti.Framework.Widgets.Widget>(); for (long i = 0L; i < checked(widget.steps.Count); i += 1L)
            {
                __collection29851.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Column(key: _keys[(int)i], children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new InkWell(onTap: (!Equals(widget.steps[(int)i].state, StepState.disabled)) ? (() => {
DartRuntimePrimitives.Ignore(Scrollable.ensureVisible(_keys[(int)i].currentContext!, curve: Curves.fastOutSlowIn, duration: ThemeLibrary.kThemeAnimationDuration));
widget.onStepTapped?.Invoke(i);
}) : null, canRequestFocus: !Equals(widget.steps[(int)i].state, StepState.disabled), child: _buildVerticalHeader(i))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildVerticalBody(i)) })));
            }
            return __collection29851;
        }))());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildHorizontal()
    {
        global::Doroti.Framework.Painting.EdgeInsetsGeometry effectiveHorizontalContentPadding = widget.contentPadding ?? StepperLibrary._kDefaultHorizontalContentPadding;
        var childrenLocal = ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() =>
        {
            var __collection31062 = new List<global::Doroti.Framework.Widgets.Widget>(); for (long i = 0L; i < checked(widget.steps.Count); i += 1L)
            {
                __collection31062.AddRange(((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() =>
                {
                    var __collection31130 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection31130.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new InkResponse(onTap: (!Equals(widget.steps[(int)i].state, StepState.disabled)) ? (() =>
                    {
                        widget.onStepTapped?.Invoke(i);
                    }) : null, canRequestFocus: !Equals(widget.steps[(int)i].state, StepState.disabled), child: new global::Doroti.Framework.Widgets.Row(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: _isLabel() ? 104.0 : 72.0, child: new global::Doroti.Framework.Widgets.Column(mainAxisAlignment: MainAxisAlignment.center, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection31654 = new List<global::Doroti.Framework.Widgets.Widget>(); if (widget.steps[(int)i].label is not null) { __collection31654.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: 24.0))); } __collection31654.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Center(child: _buildIcon(i)))); if (widget.steps[(int)i].label is not null) { __collection31654.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: 24.0, child: _buildLabelText(i)))); } return __collection31654; }))()))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: _stepIconMargin ?? EdgeInsetsDirectional.CreateOnly(start: 12.0), child: _buildHeaderText(i))) })))); if (!_isLast(i)) { __collection31130.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Padding(padding: _stepIconMargin ?? EdgeInsets.CreateSymmetric(horizontal: 8.0), child: new global::Doroti.Framework.Widgets.SizedBox(height: (widget.steps[(int)i].stepStyle?.connectorThickness ?? widget.connectorThickness) ?? 1.0, child: new global::Doroti.Framework.Widgets.ColoredBox(color: widget.steps[(int)i].stepStyle?.connectorColor ?? _connectorColor(widget.steps[(int)i].isActive))))))); }
                    return __collection31130;
                }))());
            }
            return __collection31062;
        }))();
        var stepPanels = new List<global::Doroti.Framework.Widgets.Widget>();
        for (var iLocal = 0L; iLocal < checked(widget.steps.Count); iLocal += 1L)
        {
            stepPanels.Add(new global::Doroti.Framework.Widgets.Visibility(maintainState: true, visible: iLocal == widget.currentStep, child: new global::Doroti.Framework.Widgets.ClipRect(clipBehavior: widget.clipBehavior, child: widget.steps[(int)iLocal].content)));
        }
        return new global::Doroti.Framework.Widgets.Column(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new Material(elevation: widget.elevation ?? 2, child: new global::Doroti.Framework.Widgets.Padding(padding: effectiveHeaderPadding, child: new global::Doroti.Framework.Widgets.SizedBox(height: (_stepIconHeight is not null) ? (DartRuntimePrimitives.RequireValue(_stepIconHeight) * _heightFactor) : null, child: new global::Doroti.Framework.Widgets.Row(children: childrenLocal))))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.ListView(controller: widget.controller, physics: widget.physics, padding: effectiveHorizontalContentPadding, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnimatedSize(curve: Curves.fastOutSlowIn, duration: ThemeLibrary.kThemeAnimationDuration, child: new global::Doroti.Framework.Widgets.Column(crossAxisAlignment: CrossAxisAlignment.stretch, children: stepPanels))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildVerticalControls(widget.currentStep)) }))) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if (context.findAncestorWidgetOfExactType<Stepper>() is not null)
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create("Steppers must not be nested.\n" + "The material specification advises that one should avoid embedding " + "steppers within steppers. " + "https://material.io/archive/guidelines/components/steppers.html#steppers-usage"));
                }
                return true;
            });
        return widget.type switch { StepperType.vertical => _buildVertical(), StepperType.horizontal => _buildHorizontal(), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
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

    public virtual void _removeTicker(global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider ticker)
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
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (_tickers is not null)
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
                    {
                        if (ticker.isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", _tickers, description: (_tickers is not null) ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}" : null, defaultValue: default));
    }

}

internal class _TrianglePainter__stepper : global::Doroti.Framework.Rendering.CustomPainter
{
    public virtual Color color { get; private set; } = default!;

    internal _TrianglePainter__stepper(Color color)
    {
        this.color = color;
    }

    public override bool? hitTest(Offset position) => true;
    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldPainter = (_TrianglePainter__stepper)oldDelegate;
        return !Equals(__oldPainter.color, color);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(Canvas canvas, Size size)
    {
        double @base = size.width;
        double halfBase = size.width / 2.0;
        double heightLocal = size.height;
        var points = new List<global::Doroti.Ui.Offset> { new global::Doroti.Ui.Offset(0.0, heightLocal), new global::Doroti.Ui.Offset(@base, heightLocal), new global::Doroti.Ui.Offset(halfBase, 0.0) };
        canvas.drawPath(((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addPolygon(points, true);
    return __cascade;
}))(), ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = color;
    return __cascade;
}))());
    }

}

public class StepStyle : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual Color? color { get; private set; }
    public virtual Color? errorColor { get; private set; }
    public virtual Color? connectorColor { get; private set; }
    public virtual double? connectorThickness { get; private set; }
    public virtual global::Doroti.Framework.Painting.BoxBorder? border { get; private set; }
    public virtual global::Doroti.Framework.Painting.BoxShadow? boxShadow { get; private set; }
    public virtual global::Doroti.Framework.Painting.Gradient? gradient { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? indexStyle { get; private set; }

    public StepStyle(Color? color = null, Color? errorColor = null, Color? connectorColor = null, double? connectorThickness = null, global::Doroti.Framework.Painting.BoxBorder? border = null, global::Doroti.Framework.Painting.BoxShadow? boxShadow = null, global::Doroti.Framework.Painting.Gradient? gradient = null, global::Doroti.Framework.Painting.TextStyle? indexStyle = null)
    {
        this.color = color;
        this.errorColor = errorColor;
        this.connectorColor = connectorColor;
        this.connectorThickness = connectorThickness;
        this.border = border;
        this.boxShadow = boxShadow;
        this.gradient = gradient;
        this.indexStyle = indexStyle;
    }

    public virtual StepStyle copyWith(Color? color = null, Color? errorColor = null, Color? connectorColor = null, double? connectorThickness = null, global::Doroti.Framework.Painting.BoxBorder? border = null, global::Doroti.Framework.Painting.BoxShadow? boxShadow = null, global::Doroti.Framework.Painting.Gradient? gradient = null, global::Doroti.Framework.Painting.TextStyle? indexStyle = null)
    {
        return new StepStyle(color: color ?? this.color, errorColor: errorColor ?? this.errorColor, connectorColor: connectorColor ?? this.connectorColor, connectorThickness: connectorThickness ?? this.connectorThickness, border: border ?? this.border, boxShadow: boxShadow ?? this.boxShadow, gradient: gradient ?? this.gradient, indexStyle: indexStyle ?? this.indexStyle);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual StepStyle merge(StepStyle? stepStyle)
    {
        if (stepStyle is null)
        {
            return this;
        }
        return copyWith(color: stepStyle.color, errorColor: stepStyle.errorColor, connectorColor: stepStyle.connectorColor, connectorThickness: stepStyle.connectorThickness, border: stepStyle.border, boxShadow: stepStyle.boxShadow, gradient: stepStyle.gradient, indexStyle: stepStyle.indexStyle);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode()
    {
        return FoundationRuntimePorts.ObjectHash(color, errorColor, connectorColor, connectorThickness, border, boxShadow, gradient, indexStyle);
    }
    public override bool Equals(object? other)
    {
        var __other = other as StepStyle;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is StepStyle) && Equals(__other.color, color) && Equals(__other.errorColor, errorColor) && Equals(__other.connectorColor, connectorColor) && (__other.connectorThickness == connectorThickness) && Equals(__other.border, border) && Equals(__other.boxShadow, boxShadow) && Equals(__other.gradient, gradient) && Equals(__other.indexStyle, indexStyle);
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        var theme = ThemeData.Create();
        TextTheme defaultTextTheme = theme.textTheme;
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("color", color, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("errorColor", errorColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("connectorColor", connectorColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("connectorThickness", connectorThickness, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.BoxBorder>("border", border, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.BoxShadow>("boxShadow", boxShadow, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.Gradient>("gradient", gradient, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("indexStyle", indexStyle, defaultValue: defaultTextTheme.bodyLarge));
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
