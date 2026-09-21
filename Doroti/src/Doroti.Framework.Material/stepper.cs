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
    error,
}

public enum StepperType
{
    vertical,
    horizontal,
}

public class ControlsDetails
{
    public virtual long currentStep { get; private set; } = default!;
    public virtual long stepIndex { get; private set; } = default!;
    public virtual Action? onStepContinue { get; private set; }
    public virtual Action? onStepCancel { get; private set; }

    public ControlsDetails(
        long currentStep,
        long stepIndex,
        Action? onStepCancel = null,
        Action? onStepContinue = null
    )
    {
        this.currentStep = currentStep;
        this.stepIndex = stepIndex;
        this.onStepCancel = onStepCancel;
        this.onStepContinue = onStepContinue;
    }

    public virtual bool isActive =>
        DartRuntimePrimitives.ConvertValue<bool>(currentStep == stepIndex);
}

public delegate Widget ControlsWidgetBuilder(BuildContext context, ControlsDetails details);

public delegate Widget? StepIconBuilder(long stepIndex, StepState stepState);

public static partial class StepperLibrary
{
    internal static TextStyle _kStepStyle = new TextStyle(fontSize: 12.0, color: Colors.white);
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
    internal static EdgeInsetsDirectional _kDefaultVerticalContentPadding =
        EdgeInsetsDirectional.CreateOnly(start: 60.0, end: 24.0, bottom: 24.0);
}

public static partial class StepperLibrary
{
    internal static EdgeInsets _kDefaultHorizontalContentPadding = EdgeInsets.CreateAll(24.0);
}

public static partial class StepperLibrary
{
    internal static EdgeInsetsGeometry _kDefaultHeaderPadding = EdgeInsets.CreateSymmetric(
        horizontal: 24.0
    );
}

public class Step
{
    public virtual Widget title { get; private set; } = default!;
    public virtual Widget? subtitle { get; private set; }
    public virtual Widget content { get; private set; } = default!;
    public virtual StepState state { get; private set; } = default!;
    public virtual bool isActive { get; private set; } = default!;
    public virtual Widget? label { get; private set; }
    public virtual StepStyle? stepStyle { get; private set; }

    public Step(
        Widget title,
        Widget? subtitle = null,
        Widget content = default!,
        StepState state = StepState.indexed,
        bool isActive = false,
        Widget? label = null,
        StepStyle? stepStyle = null
    )
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

public class Stepper : StatefulWidget
{
    public virtual List<Step> steps { get; private set; } = default!;
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual ScrollController? controller { get; private set; }
    public virtual StepperType type { get; private set; } = default!;
    public virtual long currentStep { get; private set; } = default!;
    public virtual Action<long>? onStepTapped { get; private set; }
    public virtual Action? onStepContinue { get; private set; }
    public virtual Action? onStepCancel { get; private set; }
    public virtual Func<BuildContext, ControlsDetails, Widget>? controlsBuilder
    {
        get;
        private set;
    }
    public virtual double? elevation { get; private set; }
    public virtual EdgeInsetsGeometry? margin { get; private set; }
    public virtual WidgetStateProperty<Color>? connectorColor { get; private set; }
    public virtual double? connectorThickness { get; private set; }
    public virtual Func<long, StepState, Widget?>? stepIconBuilder { get; private set; }
    public virtual double? stepIconHeight { get; private set; }
    public virtual double? stepIconWidth { get; private set; }
    public virtual EdgeInsets? stepIconMargin { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual EdgeInsetsGeometry? headerPadding { get; private set; }
    public virtual EdgeInsetsGeometry? contentPadding { get; private set; }

    public Stepper(
        Key? key = null,
        List<Step> steps = default!,
        ScrollController? controller = null,
        ScrollPhysics? physics = null,
        StepperType type = StepperType.vertical,
        long currentStep = 0,
        Action<long>? onStepTapped = null,
        Action? onStepContinue = null,
        Action? onStepCancel = null,
        Func<BuildContext, ControlsDetails, Widget>? controlsBuilder = null,
        double? elevation = null,
        EdgeInsetsGeometry? margin = null,
        WidgetStateProperty<Color>? connectorColor = null,
        double? connectorThickness = null,
        Func<long, StepState, Widget?>? stepIconBuilder = null,
        double? stepIconHeight = null,
        double? stepIconWidth = null,
        EdgeInsets? stepIconMargin = null,
        Clip clipBehavior = Clip.none,
        EdgeInsetsGeometry? headerPadding = null,
        EdgeInsetsGeometry? contentPadding = null
    )
        : base(key: key)
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
        System.Diagnostics.Debug.Assert(
            (0L <= currentStep) && (currentStep < checked(steps.Count))
        );
        System.Diagnostics.Debug.Assert(
            (stepIconHeight is null)
                || (
                    (stepIconHeight >= StepperLibrary._kStepSize)
                    && (stepIconHeight <= StepperLibrary._kMaxStepSize)
                )
        );
        System.Diagnostics.Debug.Assert(
            (stepIconWidth is null)
                || (
                    (stepIconWidth >= StepperLibrary._kStepSize)
                    && (stepIconWidth <= StepperLibrary._kMaxStepSize)
                )
        );
        System.Diagnostics.Debug.Assert(
            (stepIconHeight is null)
                || (stepIconWidth is null)
                || (
                    (
                        stepIconHeight
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
                    == (
                        stepIconWidth
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
                )
        );
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _StepperState__stepper());
}

internal class _StepperState__stepper : State<Stepper>, TickerProviderStateMixin<Stepper>
{
    internal virtual List<GlobalKey<IState>> _keys { get; set; } = default!;
    internal virtual DartMap<long, StepState> _oldStates { get; private set; } =
        new DartMap<long, StepState>();
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _keys = DartRuntimePrimitives.CreateList(
            checked(widget.steps.Count),
            (i) => GlobalKey<IState>.Create()
        );
        for (var iLocal = 0L; iLocal < checked(widget.steps.Count); iLocal += 1L)
        {
            _oldStates[iLocal] = widget.steps[(int)iLocal].state;
        }
    }

    public override void didUpdateWidget(Stepper oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() =>
            checked(widget.steps.Count) == checked((long)oldWidget.steps.Count)
        );
        for (var i = 0L; i < checked(oldWidget.steps.Count); i += 1L)
        {
            _oldStates[i] = oldWidget.steps[(int)i].state;
        }
    }

    internal virtual EdgeInsetsGeometry? _stepIconMargin =>
        DartRuntimePrimitives.ConvertValue<EdgeInsetsGeometry>(widget.stepIconMargin);
    internal virtual double? _stepIconHeight => widget.stepIconHeight;
    internal virtual double? _stepIconWidth => widget.stepIconWidth;
    public virtual EdgeInsetsGeometry effectiveHeaderPadding =>
        DartRuntimePrimitives.ConvertValue<EdgeInsetsGeometry>(
            widget.headerPadding ?? StepperLibrary._kDefaultHeaderPadding
        );
    internal virtual double _heightFactor
    {
        get { return (_isLabel() && (_stepIconHeight is not null)) ? 2.5 : 2.0; }
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

    internal virtual Color _connectorColor(bool isActive)
    {
        ColorScheme colorSchemeLocal = Theme.of(context).colorScheme;
        var states = (
            (Func<HashSet<WidgetState>>)(
                () =>
                {
                    var __collection15710 = new HashSet<WidgetState>();
                    if (isActive)
                    {
                        __collection15710.Add(WidgetState.selected);
                    }
                    else
                    {
                        __collection15710.Add(WidgetState.disabled);
                    }
                    return __collection15710;
                }
            )
        )();
        Color? resolvedConnectorColor = widget.connectorColor?.resolve(states);
        return resolvedConnectorColor
            ?? (isActive ? colorSchemeLocal.primary : Colors.grey.shade400);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildLine(bool visible, bool isActive)
    {
        return new ColoredBox(
            color: _connectorColor(isActive),
            child: new SizedBox(
                width: visible ? (widget.connectorThickness ?? 1.0) : 0.0,
                height: 16.0
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildCircleChild(long index, bool oldState)
    {
        StepState stateLocal = oldState
            ? (
                DartCollectionRuntime.NullableMapValue<StepState>(_oldStates, index)
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            )
            : widget.steps[(int)index].state;
        if (widget.stepIconBuilder?.Invoke(index, stateLocal) is Widget icon)
        {
            return icon;
        }
        TextStyle? textStyle = _stepStyle(index)?.indexStyle;
        bool isDarkActive = _isDark() && widget.steps[(int)index].isActive;
        Color iconColor = isDarkActive
            ? StepperLibrary._kCircleActiveDark
            : StepperLibrary._kCircleActiveLight;
        textStyle ??= (
            isDarkActive
                ? StepperLibrary._kStepStyle.copyWith(color: Colors.black87)
                : StepperLibrary._kStepStyle
        );
        return stateLocal switch
        {
            StepState.indexed => DartRuntimePrimitives.ConvertValue<Widget>(
                new Text($"{index + 1L}", style: textStyle)
            ),
            StepState.disabled => DartRuntimePrimitives.ConvertValue<Widget>(
                new Text($"{index + 1L}", style: textStyle)
            ),
            StepState.editing => DartRuntimePrimitives.ConvertValue<Widget>(
                new Icon(Icons.edit, color: iconColor, size: 18.0)
            ),
            StepState.complete => DartRuntimePrimitives.ConvertValue<Widget>(
                new Icon(Icons.check, color: iconColor, size: 18.0)
            ),
            StepState.error => DartRuntimePrimitives.ConvertValue<Widget>(
                new Center(child: new Text("!", style: StepperLibrary._kStepStyle))
            ),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Color _circleColor(long index)
    {
        bool isActiveLocal = widget.steps[(int)index].isActive;
        ColorScheme colorSchemeLocal = Theme.of(context).colorScheme;
        var states = (
            (Func<HashSet<WidgetState>>)(
                () =>
                {
                    var __collection17276 = new HashSet<WidgetState>();
                    if (isActiveLocal)
                    {
                        __collection17276.Add(WidgetState.selected);
                    }
                    else
                    {
                        __collection17276.Add(WidgetState.disabled);
                    }
                    return __collection17276;
                }
            )
        )();
        Color? resolvedConnectorColor = widget.connectorColor?.resolve(states);
        if (resolvedConnectorColor is not null)
        {
            return resolvedConnectorColor;
        }
        if (!_isDark())
        {
            return isActiveLocal
                ? colorSchemeLocal.primary
                : colorSchemeLocal.onSurface.withOpacity(0.38);
        }
        else
        {
            return isActiveLocal ? colorSchemeLocal.secondary : colorSchemeLocal.background;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildCircle(long index, bool oldState)
    {
        return new Padding(
            padding: _stepIconMargin ?? EdgeInsets.CreateSymmetric(vertical: 8.0),
            child: new SizedBox(
                width: _stepIconWidth ?? StepperLibrary._kStepSize,
                height: _stepIconHeight ?? StepperLibrary._kStepSize,
                child: new AnimatedContainer(
                    curve: Curves.fastOutSlowIn,
                    duration: ThemeLibrary.kThemeAnimationDuration,
                    decoration: new BoxDecoration(
                        color: _stepStyle(index)?.color ?? _circleColor(index),
                        shape: BoxShape.circle,
                        border: _stepStyle(index)?.border,
                        boxShadow: (_stepStyle(index)?.boxShadow is not null)
                            ? new List<BoxShadow> { _stepStyle(index)!.boxShadow! }
                            : null,
                        gradient: _stepStyle(index)?.gradient
                    ),
                    child: new Center(
                        child: _buildCircleChild(
                            index,
                            oldState && Equals(widget.steps[(int)index].state, StepState.error)
                        )
                    )
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildTriangle(long index, bool oldState)
    {
        Color? colorLocal = _stepStyle(index)?.errorColor;
        colorLocal ??= (_isDark() ? StepperLibrary._kErrorDark : StepperLibrary._kErrorLight);
        return new Padding(
            padding: _stepIconMargin ?? EdgeInsets.CreateSymmetric(vertical: 8.0),
            child: new SizedBox(
                width: _stepIconWidth ?? StepperLibrary._kStepSize,
                height: _stepIconHeight ?? StepperLibrary._kStepSize,
                child: new Center(
                    child: new SizedBox(
                        width: _stepIconWidth ?? StepperLibrary._kStepSize,
                        height: (_stepIconHeight is not null)
                            ? (
                                (
                                    _stepIconHeight
                                    ?? throw new global::System.NullReferenceException(
                                        "Dart null assertion failed."
                                    )
                                ) * StepperLibrary._kTriangleSqrt
                            )
                            : StepperLibrary._kTriangleHeight,
                        child: new CustomPaint(
                            painter: new _TrianglePainter__stepper(color: colorLocal),
                            child: new Align(
                                alignment: new Alignment(0.0, 0.8),
                                child: _buildCircleChild(
                                    index,
                                    oldState
                                        && (
                                            !Equals(widget.steps[(int)index].state, StepState.error)
                                        )
                                )
                            )
                        )
                    )
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildIcon(long index)
    {
        if (
            !Equals(
                widget.steps[(int)index].state,
                DartCollectionRuntime.NullableMapValue<StepState>(_oldStates, index)
            )
        )
        {
            return new AnimatedCrossFade(
                firstChild: _buildCircle(index, true),
                secondChild: _buildTriangle(index, true),
                firstCurve: new Interval(0.0, 0.6, curve: Curves.fastOutSlowIn),
                secondCurve: new Interval(0.4, 1.0, curve: Curves.fastOutSlowIn),
                sizeCurve: Curves.fastOutSlowIn,
                crossFadeState: Equals(widget.steps[(int)index].state, StepState.error)
                    ? CrossFadeState.showSecond
                    : CrossFadeState.showFirst,
                duration: ThemeLibrary.kThemeAnimationDuration
            );
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

    internal virtual Widget _buildVerticalControls(long stepIndex)
    {
        if (widget.controlsBuilder is not null)
        {
            return widget.controlsBuilder!(
                context,
                new ControlsDetails(
                    currentStep: widget.currentStep,
                    onStepContinue: widget.onStepContinue,
                    onStepCancel: widget.onStepCancel,
                    stepIndex: stepIndex
                )
            );
        }
        Color cancelColor = Theme.brightnessOf(context) switch
        {
            Brightness.light => Colors.black54,
            Brightness.dark => Colors.white70,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        ThemeData themeData = Theme.of(context);
        ColorScheme colorSchemeLocal = themeData.colorScheme;
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        OutlinedBorder buttonShape = new RoundedRectangleBorder(
            borderRadius: BorderRadius.CreateAll(Radius.circular(2))
        );
        var buttonPadding = EdgeInsets.CreateSymmetric(horizontal: 16.0);
        return new Padding(
            padding: EdgeInsets.CreateOnly(top: 16.0),
            child: new SizedBox(
                height: 48.0,
                child: new Row(
                    children: new List<Widget>
                    {
                        DartRuntimePrimitives.ConvertValue<Widget>(
                            new TextButton(
                                onPressed: widget.onStepContinue,
                                style: new ButtonStyle(
                                    foregroundColor: WidgetStateProperty.resolveWith(
                                        (states) =>
                                        {
                                            return states.Contains(WidgetState.disabled)
                                                ? null
                                                : (
                                                    _isDark()
                                                        ? colorSchemeLocal.onSurface
                                                        : colorSchemeLocal.onPrimary
                                                );
                                            throw new InvalidOperationException(
                                                "Dart closure completed without a value."
                                            );
                                        }
                                    ),
                                    backgroundColor: WidgetStateProperty.resolveWith(
                                        (states) =>
                                        {
                                            return (
                                                _isDark() || states.Contains(WidgetState.disabled)
                                            )
                                                ? null
                                                : colorSchemeLocal.primary;
                                            throw new InvalidOperationException(
                                                "Dart closure completed without a value."
                                            );
                                        }
                                    ),
                                    padding: new WidgetStatePropertyAll<EdgeInsetsGeometry>(
                                        buttonPadding
                                    ),
                                    shape: new WidgetStatePropertyAll<OutlinedBorder>(buttonShape)
                                ),
                                child: new Text(localizations.continueButtonLabel)
                            )
                        ),
                        DartRuntimePrimitives.ConvertValue<Widget>(
                            new Padding(
                                padding: EdgeInsetsDirectional.CreateOnly(start: 8.0),
                                child: new TextButton(
                                    onPressed: widget.onStepCancel,
                                    style: TextButton.styleFrom(
                                        foregroundColor: cancelColor,
                                        padding: buttonPadding,
                                        shape: buttonShape
                                    ),
                                    child: new Text(localizations.cancelButtonLabel)
                                )
                            )
                        ),
                    }
                )
            )
        );
    }

    internal virtual TextStyle _titleStyle(long index)
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
                return textThemeLocal.bodyLarge!.copyWith(
                    color: _isDark()
                        ? StepperLibrary._kDisabledDark
                        : StepperLibrary._kDisabledLight
                );
            }
            case StepState.error:
            {
                return textThemeLocal.bodyLarge!.copyWith(
                    color: _isDark() ? StepperLibrary._kErrorDark : StepperLibrary._kErrorLight
                );
            }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual TextStyle _subtitleStyle(long index)
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
                return textThemeLocal.bodySmall!.copyWith(
                    color: _isDark()
                        ? StepperLibrary._kDisabledDark
                        : StepperLibrary._kDisabledLight
                );
            }
            case StepState.error:
            {
                return textThemeLocal.bodySmall!.copyWith(
                    color: _isDark() ? StepperLibrary._kErrorDark : StepperLibrary._kErrorLight
                );
            }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual TextStyle _labelStyle(long index)
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
                return textThemeLocal.bodyLarge!.copyWith(
                    color: _isDark()
                        ? StepperLibrary._kDisabledDark
                        : StepperLibrary._kDisabledLight
                );
            }
            case StepState.error:
            {
                return textThemeLocal.bodyLarge!.copyWith(
                    color: _isDark() ? StepperLibrary._kErrorDark : StepperLibrary._kErrorLight
                );
            }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildHeaderText(long index)
    {
        return new Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: (
                (Func<List<Widget>>)(
                    () =>
                    {
                        var __collection25461 = new List<Widget>();
                        __collection25461.Add(
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                new AnimatedDefaultTextStyle(
                                    style: _titleStyle(index),
                                    duration: ThemeLibrary.kThemeAnimationDuration,
                                    curve: Curves.fastOutSlowIn,
                                    child: widget.steps[(int)index].title
                                )
                            )
                        );
                        if (widget.steps[(int)index].subtitle is not null)
                        {
                            __collection25461.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    new Padding(
                                        padding: EdgeInsets.CreateOnly(top: 2.0),
                                        child: new AnimatedDefaultTextStyle(
                                            style: _subtitleStyle(index),
                                            duration: ThemeLibrary.kThemeAnimationDuration,
                                            curve: Curves.fastOutSlowIn,
                                            child: widget.steps[(int)index].subtitle!
                                        )
                                    )
                                )
                            );
                        }
                        return __collection25461;
                    }
                )
            )()
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildLabelText(long index)
    {
        if (widget.steps[(int)index].label is not null)
        {
            return new AnimatedDefaultTextStyle(
                style: _labelStyle(index),
                duration: ThemeLibrary.kThemeAnimationDuration,
                child: widget.steps[(int)index].label!
            );
        }
        return SizedBox.CreateShrink();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildVerticalHeader(long index)
    {
        bool isActiveLocal = widget.steps[(int)index].isActive;
        bool isPreviousActive = (index > 0L) && widget.steps[(int)(index - 1L)].isActive;
        return new Padding(
            padding: effectiveHeaderPadding,
            child: new Row(
                children: new List<Widget>
                {
                    DartRuntimePrimitives.ConvertValue<Widget>(
                        new Column(
                            children: new List<Widget>
                            {
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    _buildLine(!_isFirst(index), isPreviousActive)
                                ),
                                DartRuntimePrimitives.ConvertValue<Widget>(_buildIcon(index)),
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    _buildLine(!_isLast(index), isActiveLocal)
                                ),
                            }
                        )
                    ),
                    DartRuntimePrimitives.ConvertValue<Widget>(
                        new Expanded(
                            child: new Padding(
                                padding: EdgeInsetsDirectional.CreateOnly(start: 12.0),
                                child: _buildHeaderText(index)
                            )
                        )
                    ),
                }
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildVerticalBody(long index)
    {
        double? marginLeft = _stepIconMargin?.resolve(TextDirection.ltr).left;
        double? marginRight = _stepIconMargin?.resolve(TextDirection.ltr).right;
        double? additionalMarginLeft =
            (marginLeft is not null)
                ? (
                    (
                        marginLeft
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) / 2.0
                )
                : null;
        double? additionalMarginRight =
            (marginRight is not null)
                ? (
                    (
                        marginRight
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) / 2.0
                )
                : null;
        EdgeInsetsGeometry effectiveVerticalContentPadding = (
            widget.contentPadding ?? StepperLibrary._kDefaultVerticalContentPadding
        ).add(EdgeInsetsDirectional.CreateOnly(start: marginLeft ?? 0.0));
        return new Stack(
            children: new List<Widget>
            {
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new PositionedDirectional(
                        start: 24.0
                            + (additionalMarginLeft ?? 0.0)
                            + (additionalMarginRight ?? 0.0),
                        top: 0.0,
                        bottom: 0.0,
                        width: _stepIconWidth ?? StepperLibrary._kStepSize,
                        child: new Center(
                            child: new SizedBox(
                                width: !_isLast(index) ? (widget.connectorThickness ?? 1.0) : 0.0,
                                height: double.PositiveInfinity,
                                child: new ColoredBox(
                                    color: _connectorColor(widget.steps[(int)index].isActive)
                                )
                            )
                        )
                    )
                ),
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new AnimatedCrossFade(
                        firstChild: new SizedBox(width: double.PositiveInfinity, height: 0),
                        secondChild: new Padding(
                            padding: effectiveVerticalContentPadding,
                            child: new Column(
                                children: new List<Widget>
                                {
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        new ClipRect(
                                            clipBehavior: widget.clipBehavior,
                                            child: widget.steps[(int)index].content
                                        )
                                    ),
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        _buildVerticalControls(index)
                                    ),
                                }
                            )
                        ),
                        firstCurve: new Interval(0.0, 0.6, curve: Curves.fastOutSlowIn),
                        secondCurve: new Interval(0.4, 1.0, curve: Curves.fastOutSlowIn),
                        sizeCurve: Curves.fastOutSlowIn,
                        crossFadeState: _isCurrent(index)
                            ? CrossFadeState.showSecond
                            : CrossFadeState.showFirst,
                        duration: ThemeLibrary.kThemeAnimationDuration
                    )
                ),
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildVertical()
    {
        return new ListView(
            controller: widget.controller,
            shrinkWrap: true,
            physics: widget.physics,
            children: (
                (Func<List<Widget>>)(
                    () =>
                    {
                        var __collection29851 = new List<Widget>();
                        for (long i = 0L; i < checked(widget.steps.Count); i += 1L)
                        {
                            __collection29851.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    new Column(
                                        key: _keys[(int)i],
                                        children: new List<Widget>
                                        {
                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                new InkWell(
                                                    onTap: (
                                                        !Equals(
                                                            widget.steps[(int)i].state,
                                                            StepState.disabled
                                                        )
                                                    )
                                                        ? (
                                                            () =>
                                                            {
                                                                DartRuntimePrimitives.Ignore(
                                                                    Scrollable.ensureVisible(
                                                                        _keys[
                                                                            (int)i
                                                                        ].currentContext!,
                                                                        curve: Curves.fastOutSlowIn,
                                                                        duration: ThemeLibrary.kThemeAnimationDuration
                                                                    )
                                                                );
                                                                widget.onStepTapped?.Invoke(i);
                                                            }
                                                        )
                                                        : null,
                                                    canRequestFocus: !Equals(
                                                        widget.steps[(int)i].state,
                                                        StepState.disabled
                                                    ),
                                                    child: _buildVerticalHeader(i)
                                                )
                                            ),
                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                _buildVerticalBody(i)
                                            ),
                                        }
                                    )
                                )
                            );
                        }
                        return __collection29851;
                    }
                )
            )()
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildHorizontal()
    {
        EdgeInsetsGeometry effectiveHorizontalContentPadding =
            widget.contentPadding ?? StepperLibrary._kDefaultHorizontalContentPadding;
        var childrenLocal = (
            (Func<List<Widget>>)(
                () =>
                {
                    var __collection31062 = new List<Widget>();
                    for (long i = 0L; i < checked(widget.steps.Count); i += 1L)
                    {
                        __collection31062.AddRange(
                            (
                                (Func<List<Widget>>)(
                                    () =>
                                    {
                                        var __collection31130 = new List<Widget>();
                                        __collection31130.Add(
                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                new InkResponse(
                                                    onTap: (
                                                        !Equals(
                                                            widget.steps[(int)i].state,
                                                            StepState.disabled
                                                        )
                                                    )
                                                        ? (
                                                            () =>
                                                            {
                                                                widget.onStepTapped?.Invoke(i);
                                                            }
                                                        )
                                                        : null,
                                                    canRequestFocus: !Equals(
                                                        widget.steps[(int)i].state,
                                                        StepState.disabled
                                                    ),
                                                    child: new Row(
                                                        children: new List<Widget>
                                                        {
                                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                                new SizedBox(
                                                                    height: _isLabel()
                                                                        ? 104.0
                                                                        : 72.0,
                                                                    child: new Column(
                                                                        mainAxisAlignment: MainAxisAlignment.center,
                                                                        children: (
                                                                            (Func<List<Widget>>)(
                                                                                () =>
                                                                                {
                                                                                    var __collection31654 =
                                                                                        new List<Widget>();
                                                                                    if (
                                                                                        widget
                                                                                            .steps[
                                                                                                (int)i
                                                                                            ]
                                                                                            .label
                                                                                        is not null
                                                                                    )
                                                                                    {
                                                                                        __collection31654.Add(
                                                                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                                                                new SizedBox(
                                                                                                    height: 24.0
                                                                                                )
                                                                                            )
                                                                                        );
                                                                                    }
                                                                                    __collection31654.Add(
                                                                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                                                                            new Center(
                                                                                                child: _buildIcon(
                                                                                                    i
                                                                                                )
                                                                                            )
                                                                                        )
                                                                                    );
                                                                                    if (
                                                                                        widget
                                                                                            .steps[
                                                                                                (int)i
                                                                                            ]
                                                                                            .label
                                                                                        is not null
                                                                                    )
                                                                                    {
                                                                                        __collection31654.Add(
                                                                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                                                                new SizedBox(
                                                                                                    height: 24.0,
                                                                                                    child: _buildLabelText(
                                                                                                        i
                                                                                                    )
                                                                                                )
                                                                                            )
                                                                                        );
                                                                                    }
                                                                                    return __collection31654;
                                                                                }
                                                                            )
                                                                        )()
                                                                    )
                                                                )
                                                            ),
                                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                                new Padding(
                                                                    padding: _stepIconMargin
                                                                        ?? EdgeInsetsDirectional.CreateOnly(
                                                                            start: 12.0
                                                                        ),
                                                                    child: _buildHeaderText(i)
                                                                )
                                                            ),
                                                        }
                                                    )
                                                )
                                            )
                                        );
                                        if (!_isLast(i))
                                        {
                                            __collection31130.Add(
                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                    new Expanded(
                                                        child: new Padding(
                                                            padding: _stepIconMargin
                                                                ?? EdgeInsets.CreateSymmetric(
                                                                    horizontal: 8.0
                                                                ),
                                                            child: new SizedBox(
                                                                height: (
                                                                    widget
                                                                        .steps[(int)i]
                                                                        .stepStyle
                                                                        ?.connectorThickness
                                                                    ?? widget.connectorThickness
                                                                ) ?? 1.0,
                                                                child: new ColoredBox(
                                                                    color: widget
                                                                        .steps[(int)i]
                                                                        .stepStyle
                                                                        ?.connectorColor
                                                                        ?? _connectorColor(
                                                                            widget
                                                                                .steps[(int)i]
                                                                                .isActive
                                                                        )
                                                                )
                                                            )
                                                        )
                                                    )
                                                )
                                            );
                                        }
                                        return __collection31130;
                                    }
                                )
                            )()
                        );
                    }
                    return __collection31062;
                }
            )
        )();
        var stepPanels = new List<Widget>();
        for (var iLocal = 0L; iLocal < checked(widget.steps.Count); iLocal += 1L)
        {
            stepPanels.Add(
                new Visibility(
                    maintainState: true,
                    visible: iLocal == widget.currentStep,
                    child: new ClipRect(
                        clipBehavior: widget.clipBehavior,
                        child: widget.steps[(int)iLocal].content
                    )
                )
            );
        }
        return new Column(
            children: new List<Widget>
            {
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new Material(
                        elevation: widget.elevation ?? 2,
                        child: new Padding(
                            padding: effectiveHeaderPadding,
                            child: new SizedBox(
                                height: (_stepIconHeight is not null)
                                    ? (
                                        (
                                            _stepIconHeight
                                            ?? throw new global::System.NullReferenceException(
                                                "Dart null assertion failed."
                                            )
                                        ) * _heightFactor
                                    )
                                    : null,
                                child: new Row(children: childrenLocal)
                            )
                        )
                    )
                ),
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new Expanded(
                        child: new ListView(
                            controller: widget.controller,
                            physics: widget.physics,
                            padding: effectiveHorizontalContentPadding,
                            children: new List<Widget>
                            {
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    new AnimatedSize(
                                        curve: Curves.fastOutSlowIn,
                                        duration: ThemeLibrary.kThemeAnimationDuration,
                                        child: new Column(
                                            crossAxisAlignment: CrossAxisAlignment.stretch,
                                            children: stepPanels
                                        )
                                    )
                                ),
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    _buildVerticalControls(widget.currentStep)
                                ),
                            }
                        )
                    )
                ),
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckHasMaterialLocalizations(context)
        );
        DartRuntimePrimitives.Assert(() =>
        {
            if (context.findAncestorWidgetOfExactType<Stepper>() is not null)
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create(
                        "Steppers must not be nested.\n"
                            + "The material specification advises that one should avoid embedding "
                            + "steppers within steppers. "
                            + "https://material.io/archive/guidelines/components/steppers.html#steppers-usage"
                    )
                );
            }
            return true;
        });
        return widget.type switch
        {
            StepperType.vertical => _buildVertical(),
            StepperType.horizontal => _buildHorizontal(),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
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
        var result = (
            (Func<_WidgetTicker__ticker_provider>)(
                () =>
                {
                    var __cascade = new _WidgetTicker__ticker_provider(
                        onTick,
                        this,
                        debugLabel: Foundation.ConstantsLibrary.kDebugMode
                            ? $"created by {DiagnosticsLibrary.describeIdentity(this)}"
                            : null
                    );
                    __cascade.muted = !values.enabled;
                    __cascade.forceFrames = values.forceFrames;
                    return __cascade;
                }
            )
        )();
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

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (_tickers is not null)
            {
                foreach (Scheduler.Ticker ticker in _tickers!)
                {
                    if (ticker.isActive)
                    {
                        throw DartRuntimePrimitives.AsException(
                            new FlutterError(
                                new List<DiagnosticsNode>
                                {
                                    new ErrorSummary($"{this} was disposed with an active Ticker."),
                                    new ErrorDescription(
                                        $"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time "
                                            + "dispose() was called on the mixin, that Ticker was still active. All Tickers must "
                                            + "be disposed before calling super.dispose()."
                                    ),
                                    new ErrorHint(
                                        "Tickers used by AnimationControllers "
                                            + "should be disposed by calling dispose() on the AnimationController itself. "
                                            + "Otherwise, the ticker will leak."
                                    ),
                                    ticker.describeForError("The offending ticker was"),
                                }
                            )
                        );
                    }
                }
            }
            return true;
        });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<HashSet<Scheduler.Ticker>>(
                "tickers",
                _tickers,
                description: (_tickers is not null)
                    ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}"
                    : null,
                defaultValue: default
            )
        );
    }
}

internal class _TrianglePainter__stepper : CustomPainter
{
    public virtual Color color { get; private set; } = default!;

    internal _TrianglePainter__stepper(Color color)
    {
        this.color = color;
    }

    public override bool? hitTest(Offset position) => true;

    public override bool shouldRepaint(CustomPainter oldDelegate)
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
        var points = new List<Offset>
        {
            new Offset(0.0, heightLocal),
            new Offset(@base, heightLocal),
            new Offset(halfBase, 0.0),
        };
        canvas.drawPath(
            (
                (Func<Path>)(
                    () =>
                    {
                        var __cascade = new Path();
                        __cascade.addPolygon(points, true);
                        return __cascade;
                    }
                )
            )(),
            (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.color = color;
                        return __cascade;
                    }
                )
            )()
        );
    }
}

public class StepStyle : Diagnosticable
{
    public virtual Color? color { get; private set; }
    public virtual Color? errorColor { get; private set; }
    public virtual Color? connectorColor { get; private set; }
    public virtual double? connectorThickness { get; private set; }
    public virtual BoxBorder? border { get; private set; }
    public virtual BoxShadow? boxShadow { get; private set; }
    public virtual Painting.Gradient? gradient { get; private set; }
    public virtual TextStyle? indexStyle { get; private set; }

    public StepStyle(
        Color? color = null,
        Color? errorColor = null,
        Color? connectorColor = null,
        double? connectorThickness = null,
        BoxBorder? border = null,
        BoxShadow? boxShadow = null,
        Painting.Gradient? gradient = null,
        TextStyle? indexStyle = null
    )
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

    public virtual StepStyle copyWith(
        Color? color = null,
        Color? errorColor = null,
        Color? connectorColor = null,
        double? connectorThickness = null,
        BoxBorder? border = null,
        BoxShadow? boxShadow = null,
        Painting.Gradient? gradient = null,
        TextStyle? indexStyle = null
    )
    {
        return new StepStyle(
            color: color ?? this.color,
            errorColor: errorColor ?? this.errorColor,
            connectorColor: connectorColor ?? this.connectorColor,
            connectorThickness: connectorThickness ?? this.connectorThickness,
            border: border ?? this.border,
            boxShadow: boxShadow ?? this.boxShadow,
            gradient: gradient ?? this.gradient,
            indexStyle: indexStyle ?? this.indexStyle
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual StepStyle merge(StepStyle? stepStyle)
    {
        if (stepStyle is null)
        {
            return this;
        }
        return copyWith(
            color: stepStyle.color,
            errorColor: stepStyle.errorColor,
            connectorColor: stepStyle.connectorColor,
            connectorThickness: stepStyle.connectorThickness,
            border: stepStyle.border,
            boxShadow: stepStyle.boxShadow,
            gradient: stepStyle.gradient,
            indexStyle: stepStyle.indexStyle
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode()
    {
        return FoundationRuntimePorts.ObjectHash(
            color,
            errorColor,
            connectorColor,
            connectorThickness,
            border,
            boxShadow,
            gradient,
            indexStyle
        );
    }

    public override bool Equals(object? other)
    {
        var __other = other as StepStyle;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is StepStyle)
            && Equals(__other.color, color)
            && Equals(__other.errorColor, errorColor)
            && Equals(__other.connectorColor, connectorColor)
            && (__other.connectorThickness == connectorThickness)
            && Equals(__other.border, border)
            && Equals(__other.boxShadow, boxShadow)
            && Equals(__other.gradient, gradient)
            && Equals(__other.indexStyle, indexStyle);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        var theme = ThemeData.Create();
        TextTheme defaultTextTheme = theme.textTheme;
        properties.add(new ColorProperty("color", color, defaultValue: null));
        properties.add(new ColorProperty("errorColor", errorColor, defaultValue: null));
        properties.add(new ColorProperty("connectorColor", connectorColor, defaultValue: null));
        properties.add(
            new DoubleProperty("connectorThickness", connectorThickness, defaultValue: null)
        );
        properties.add(new DiagnosticsProperty<BoxBorder>("border", border, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<BoxShadow>("boxShadow", boxShadow, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<Painting.Gradient>("gradient", gradient, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "indexStyle",
                indexStyle,
                defaultValue: defaultTextTheme.bodyLarge
            )
        );
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);

    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine)
                .toDiagnosticsNode()
                .toStringDeep(minLevel: minLevel);
            return true;
        });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = null
    )
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
