// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/stepper.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

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

    public virtual bool isActive => DartRuntimePrimitives.ConvertValue<bool>((this.currentStep == this.stepIndex));
}

public delegate global::Doroti.Framework.Widgets.Widget ControlsWidgetBuilder(global::Doroti.Framework.Widgets.BuildContext context, ControlsDetails details);

public delegate global::Doroti.Framework.Widgets.Widget? StepIconBuilder(long stepIndex, StepState stepState);

public static partial class StepperLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _kStepStyle = new global::Doroti.Framework.Painting.TextStyle(fontSize: 12.0, color: Colors.white);
}

public static partial class StepperLibrary
{
    internal static Color _kErrorLight = ((Color)(object?)Colors.red);
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
    internal static double _kTriangleHeight = (StepperLibrary._kStepSize * StepperLibrary._kTriangleSqrt);
}

public static partial class StepperLibrary
{
    internal static double _kMaxStepSize = 80.0;
}

public static partial class StepperLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsetsDirectional _kDefaultVerticalContentPadding = global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 60.0, end: 24.0, bottom: 24.0);
}

public static partial class StepperLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsets _kDefaultHorizontalContentPadding = global::Doroti.Framework.Painting.EdgeInsets.CreateAll(24.0);
}

public static partial class StepperLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsetsGeometry _kDefaultHeaderPadding = ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)(object?)global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 24.0));
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
        System.Diagnostics.Debug.Assert(((0L <= currentStep) && (currentStep < checked((long)(steps.Count)))));
        System.Diagnostics.Debug.Assert(((stepIconHeight is null) || (((stepIconHeight >= StepperLibrary._kStepSize) && (stepIconHeight <= StepperLibrary._kMaxStepSize)))));
        System.Diagnostics.Debug.Assert(((stepIconWidth is null) || (((stepIconWidth >= StepperLibrary._kStepSize) && (stepIconWidth <= StepperLibrary._kMaxStepSize)))));
        System.Diagnostics.Debug.Assert((((stepIconHeight is null) || (stepIconWidth is null)) || (DartRuntimePrimitives.RequireValue(stepIconHeight) == DartRuntimePrimitives.RequireValue(stepIconWidth))));
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
        _keys = DartRuntimePrimitives.CreateList<global::Doroti.Framework.Widgets.GlobalKey<IState>>(checked((long)(((Stepper)this.widget).steps.Count)), ((i) => global::Doroti.Framework.Widgets.GlobalKey<IState>.Create()));
        for (var i__14368 = 0L; (i__14368 < checked((long)(((Stepper)this.widget).steps.Count))); i__14368 += 1L)
        {
            this._oldStates[i__14368] = ((Stepper)this.widget).steps[(int)(i__14368)].state;
        }
    }

    public override void didUpdateWidget(Stepper oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() => (checked((long)(((Stepper)this.widget).steps.Count)) == checked((long)(((Stepper)oldWidget).steps.Count))));
        for (var i__14633 = 0L; (i__14633 < checked((long)(((Stepper)oldWidget).steps.Count))); i__14633 += 1L)
        {
            this._oldStates[i__14633] = ((Stepper)oldWidget).steps[(int)(i__14633)].state;
        }
    }

    internal virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? _stepIconMargin => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(((Stepper)this.widget).stepIconMargin);
    internal virtual double? _stepIconHeight => ((Stepper)this.widget).stepIconHeight;
    internal virtual double? _stepIconWidth => ((Stepper)this.widget).stepIconWidth;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry effectiveHeaderPadding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>((((Stepper)this.widget).headerPadding ?? StepperLibrary._kDefaultHeaderPadding));
    internal virtual double _heightFactor
    {
        get
        {
            return (((_isLabel() && (this._stepIconHeight is not null))) ? 2.5 : 2.0);
            return default!;
        }
    }
    internal virtual bool _isFirst(long index)
    {
        return (index == 0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isLast(long index)
    {
        return ((checked((long)(((Stepper)this.widget).steps.Count)) - 1L) == index);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isCurrent(long index)
    {
        return (((Stepper)this.widget).currentStep == index);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isDark()
    {
        return (object.Equals(Theme.brightnessOf(this.context), Brightness.dark));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isLabel()
    {
        return ((Stepper)this.widget).steps.any(((step) => (((Step)step).label is not null)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual StepStyle? _stepStyle(long index)
    {
        return ((Stepper)this.widget).steps[(int)(index)].stepStyle;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Color _connectorColor(bool isActive)
    {
        ColorScheme colorScheme__15646 = Theme.of(this.context).colorScheme;
        var states__15701 = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection15710 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (isActive) { __collection15710.Add(global::Doroti.Framework.Widgets.WidgetState.selected); } else { __collection15710.Add(global::Doroti.Framework.Widgets.WidgetState.disabled); } return __collection15710; }))();
        global::Doroti.Ui.Color? resolvedConnectorColor__15804 = ((global::Doroti.Ui.Color?)(object?)((Stepper)this.widget).connectorColor?.resolve(states__15701));
        return ((global::Doroti.Ui.Color)(object?)(resolvedConnectorColor__15804 ?? ((isActive ? colorScheme__15646.primary : Colors.grey.shade400))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildLine(bool visible, bool isActive)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ColoredBox(color: _connectorColor(isActive), child: new global::Doroti.Framework.Widgets.SizedBox(width: (visible ? (((Stepper)this.widget).connectorThickness ?? 1.0) : 0.0), height: 16.0)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildCircleChild(long index, bool oldState)
    {
        StepState state__16264 = (oldState ? DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<StepState>(this._oldStates, index)) : ((Stepper)this.widget).steps[(int)(index)].state);
        if (((Stepper)this.widget).stepIconBuilder?.Invoke(index, state__16264) is global::Doroti.Framework.Widgets.Widget icon__16400)
        {
            return icon__16400;
        }
        global::Doroti.Framework.Painting.TextStyle? textStyle__16448 = _stepStyle(index)?.indexStyle;
        bool isDarkActive__16506 = (_isDark() && ((Stepper)this.widget).steps[(int)(index)].isActive);
        global::Doroti.Ui.Color iconColor__16580 = ((global::Doroti.Ui.Color)(object?)(isDarkActive__16506 ? StepperLibrary._kCircleActiveDark : StepperLibrary._kCircleActiveLight));
        textStyle__16448 ??= (isDarkActive__16506 ? StepperLibrary._kStepStyle.copyWith(color: Colors.black87) : StepperLibrary._kStepStyle);
        return (state__16264 switch { StepState.indexed => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Text($"{(index + 1L)}", style: textStyle__16448)), StepState.disabled => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Text($"{(index + 1L)}", style: textStyle__16448)), StepState.editing => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Icon(Icons.edit, color: iconColor__16580, size: 18.0)), StepState.complete => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Icon(Icons.check, color: iconColor__16580, size: 18.0)), StepState.error => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Center(child: new global::Doroti.Framework.Widgets.Text("!", style: StepperLibrary._kStepStyle))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Color _circleColor(long index)
    {
        bool isActive__17149 = ((Stepper)this.widget).steps[(int)(index)].isActive;
        ColorScheme colorScheme__17212 = Theme.of(this.context).colorScheme;
        var states__17267 = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection17276 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (isActive__17149) { __collection17276.Add(global::Doroti.Framework.Widgets.WidgetState.selected); } else { __collection17276.Add(global::Doroti.Framework.Widgets.WidgetState.disabled); } return __collection17276; }))();
        global::Doroti.Ui.Color? resolvedConnectorColor__17370 = ((global::Doroti.Ui.Color?)(object?)((Stepper)this.widget).connectorColor?.resolve(states__17267));
        if ((resolvedConnectorColor__17370 is not null))
        {
            return ((global::Doroti.Ui.Color)(object?)resolvedConnectorColor__17370);
        }
        if (!_isDark())
        {
            return ((global::Doroti.Ui.Color)(object?)(isActive__17149 ? colorScheme__17212.primary : colorScheme__17212.onSurface.withOpacity(0.38)));
        }
        else
        {
            return ((global::Doroti.Ui.Color)(object?)(isActive__17149 ? colorScheme__17212.secondary : colorScheme__17212.background));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildCircle(long index, bool oldState)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: (this._stepIconMargin ?? global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: 8.0)), child: new global::Doroti.Framework.Widgets.SizedBox(width: (this._stepIconWidth ?? StepperLibrary._kStepSize), height: (this._stepIconHeight ?? StepperLibrary._kStepSize), child: new global::Doroti.Framework.Widgets.AnimatedContainer(curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn, duration: ThemeLibrary.kThemeAnimationDuration, decoration: new global::Doroti.Framework.Painting.BoxDecoration(color: ((_stepStyle(index)?.color ?? (Color)_circleColor(index))), shape: global::Doroti.Framework.Painting.BoxShape.circle, border: _stepStyle(index)?.border, boxShadow: ((_stepStyle(index)?.boxShadow is not null) ? new List<global::Doroti.Framework.Painting.BoxShadow> { _stepStyle(index)!.boxShadow! } : null), gradient: _stepStyle(index)?.gradient), child: new global::Doroti.Framework.Widgets.Center(child: _buildCircleChild(index, (oldState && (object.Equals(((Stepper)this.widget).steps[(int)(index)].state, StepState.error)))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildTriangle(long index, bool oldState)
    {
        global::Doroti.Ui.Color? color__18781 = ((global::Doroti.Ui.Color?)(object?)_stepStyle(index)?.errorColor);
        color__18781 ??= (_isDark() ? StepperLibrary._kErrorDark : StepperLibrary._kErrorLight);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: (this._stepIconMargin ?? global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: 8.0)), child: new global::Doroti.Framework.Widgets.SizedBox(width: (this._stepIconWidth ?? StepperLibrary._kStepSize), height: (this._stepIconHeight ?? StepperLibrary._kStepSize), child: new global::Doroti.Framework.Widgets.Center(child: new global::Doroti.Framework.Widgets.SizedBox(width: (this._stepIconWidth ?? StepperLibrary._kStepSize), height: ((this._stepIconHeight is not null) ? (DartRuntimePrimitives.RequireValue(this._stepIconHeight) * StepperLibrary._kTriangleSqrt) : StepperLibrary._kTriangleHeight), child: new global::Doroti.Framework.Widgets.CustomPaint(painter: new _TrianglePainter__stepper(color: color__18781), child: new global::Doroti.Framework.Widgets.Align(alignment: new global::Doroti.Framework.Painting.Alignment(0.0, 0.8), child: _buildCircleChild(index, (oldState && (!object.Equals(((Stepper)this.widget).steps[(int)(index)].state, StepState.error)))))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildIcon(long index)
    {
        if ((!object.Equals(((Stepper)this.widget).steps[(int)(index)].state, DartCollectionRuntime.NullableMapValue<StepState>(this._oldStates, index))))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.AnimatedCrossFade(firstChild: _buildCircle(index, true), secondChild: _buildTriangle(index, true), firstCurve: new global::Doroti.Framework.Animation.Interval(0.0, 0.6, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn), secondCurve: new global::Doroti.Framework.Animation.Interval(0.4, 1.0, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn), sizeCurve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn, crossFadeState: ((object.Equals(((Stepper)this.widget).steps[(int)(index)].state, StepState.error)) ? global::Doroti.Framework.Widgets.CrossFadeState.showSecond : global::Doroti.Framework.Widgets.CrossFadeState.showFirst), duration: ThemeLibrary.kThemeAnimationDuration));
        }
        else
        {
            if ((!object.Equals(((Stepper)this.widget).steps[(int)(index)].state, StepState.error)))
            {
                return ((global::Doroti.Framework.Widgets.Widget)(object?)_buildCircle(index, false));
            }
            else
            {
                return ((global::Doroti.Framework.Widgets.Widget)(object?)_buildTriangle(index, false));
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildVerticalControls(long stepIndex)
    {
        if ((((Stepper)this.widget).controlsBuilder is not null))
        {
            return ((Stepper)this.widget).controlsBuilder!(this.context, new ControlsDetails(currentStep: ((Stepper)this.widget).currentStep, onStepContinue: () => ((Stepper)this.widget).onStepContinue(), onStepCancel: () => ((Stepper)this.widget).onStepCancel(), stepIndex: stepIndex));
        }
        global::Doroti.Ui.Color cancelColor__20930 = ((global::Doroti.Ui.Color)(object?)(Theme.brightnessOf(this.context) switch { Brightness.light => Colors.black54, Brightness.dark => Colors.white70, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        ThemeData themeData__21094 = Theme.of(this.context);
        ColorScheme colorScheme__21147 = themeData__21094.colorScheme;
        MaterialLocalizations localizations__21216 = MaterialLocalizations.of(this.context);
        global::Doroti.Framework.Painting.OutlinedBorder buttonShape__21293 = ((global::Doroti.Framework.Painting.OutlinedBorder)(object?)new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(2))));
        var buttonPadding__21406 = global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16.0);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(top: 16.0), child: new global::Doroti.Framework.Widgets.SizedBox(height: 48.0, child: new global::Doroti.Framework.Widgets.Row(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new TextButton(onPressed: () => ((Stepper)this.widget).onStepContinue(), style: new ButtonStyle(foregroundColor: WidgetStateProperty.resolveWith<global::Doroti.Ui.Color?>((states) => {
return ((states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled) ? null : ((_isDark() ? colorScheme__21147.onSurface : colorScheme__21147.onPrimary))));
throw new InvalidOperationException("Dart closure completed without a value.");
}), backgroundColor: WidgetStateProperty.resolveWith<global::Doroti.Ui.Color?>((states) => {
return (((_isDark() || states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled)) ? null : colorScheme__21147.primary));
throw new InvalidOperationException("Dart closure completed without a value.");
}), padding: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(buttonPadding__21406), shape: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.OutlinedBorder>(buttonShape__21293)), child: new global::Doroti.Framework.Widgets.Text((themeData__21094.useMaterial3 ? localizations__21216.continueButtonLabel : localizations__21216.continueButtonLabel.toUpperCase())))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 8.0), child: new TextButton(onPressed: () => ((Stepper)this.widget).onStepCancel(), style: TextButton.styleFrom(foregroundColor: cancelColor__20930, padding: buttonPadding__21406, shape: buttonShape__21293), child: new global::Doroti.Framework.Widgets.Text((themeData__21094.useMaterial3 ? localizations__21216.cancelButtonLabel : localizations__21216.cancelButtonLabel.toUpperCase()))))) }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.TextStyle _titleStyle(long index)
    {
        ThemeData themeData__23632 = Theme.of(this.context);
        TextTheme textTheme__23683 = themeData__23632.textTheme;
        switch (((Stepper)this.widget).steps[(int)(index)].state)
        {
            case StepState.indexed:
            case StepState.editing:
            case StepState.complete:
                {
                    return textTheme__23683.bodyLarge!;
                }
            case StepState.disabled:
                {
                    return ((global::Doroti.Framework.Painting.TextStyle)(object?)textTheme__23683.bodyLarge!.copyWith(color: (_isDark() ? StepperLibrary._kDisabledDark : StepperLibrary._kDisabledLight)));
                }
            case StepState.error:
                {
                    return ((global::Doroti.Framework.Painting.TextStyle)(object?)textTheme__23683.bodyLarge!.copyWith(color: (_isDark() ? StepperLibrary._kErrorDark : StepperLibrary._kErrorLight)));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.TextStyle _subtitleStyle(long index)
    {
        ThemeData themeData__24208 = Theme.of(this.context);
        TextTheme textTheme__24259 = themeData__24208.textTheme;
        switch (((Stepper)this.widget).steps[(int)(index)].state)
        {
            case StepState.indexed:
            case StepState.editing:
            case StepState.complete:
                {
                    return textTheme__24259.bodySmall!;
                }
            case StepState.disabled:
                {
                    return ((global::Doroti.Framework.Painting.TextStyle)(object?)textTheme__24259.bodySmall!.copyWith(color: (_isDark() ? StepperLibrary._kDisabledDark : StepperLibrary._kDisabledLight)));
                }
            case StepState.error:
                {
                    return ((global::Doroti.Framework.Painting.TextStyle)(object?)textTheme__24259.bodySmall!.copyWith(color: (_isDark() ? StepperLibrary._kErrorDark : StepperLibrary._kErrorLight)));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.TextStyle _labelStyle(long index)
    {
        ThemeData themeData__24781 = Theme.of(this.context);
        TextTheme textTheme__24832 = themeData__24781.textTheme;
        switch (((Stepper)this.widget).steps[(int)(index)].state)
        {
            case StepState.indexed:
            case StepState.editing:
            case StepState.complete:
                {
                    return textTheme__24832.bodyLarge!;
                }
            case StepState.disabled:
                {
                    return ((global::Doroti.Framework.Painting.TextStyle)(object?)textTheme__24832.bodyLarge!.copyWith(color: (_isDark() ? StepperLibrary._kDisabledDark : StepperLibrary._kDisabledLight)));
                }
            case StepState.error:
                {
                    return ((global::Doroti.Framework.Painting.TextStyle)(object?)textTheme__24832.bodyLarge!.copyWith(color: (_isDark() ? StepperLibrary._kErrorDark : StepperLibrary._kErrorLight)));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildHeaderText(long index)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Column(crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.start, mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection25461 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection25461.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnimatedDefaultTextStyle(style: _titleStyle(index), duration: ThemeLibrary.kThemeAnimationDuration, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn, child: ((Stepper)this.widget).steps[(int)(index)].title))); if ((((Stepper)this.widget).steps[(int)(index)].subtitle is not null)) { __collection25461.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(top: 2.0), child: new global::Doroti.Framework.Widgets.AnimatedDefaultTextStyle(style: _subtitleStyle(index), duration: ThemeLibrary.kThemeAnimationDuration, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn, child: ((Stepper)this.widget).steps[(int)(index)].subtitle!)))); } return __collection25461; }))()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildLabelText(long index)
    {
        if ((((Stepper)this.widget).steps[(int)(index)].label is not null))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.AnimatedDefaultTextStyle(style: _labelStyle(index), duration: ThemeLibrary.kThemeAnimationDuration, child: ((Stepper)this.widget).steps[(int)(index)].label!));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildVerticalHeader(long index)
    {
        bool isActive__26443 = ((Stepper)this.widget).steps[(int)(index)].isActive;
        bool isPreviousActive__26499 = ((index > 0L) && ((Stepper)this.widget).steps[(int)((index - 1L))].isActive);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: this.effectiveHeaderPadding, child: new global::Doroti.Framework.Widgets.Row(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Column(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildLine(!_isFirst(index), isPreviousActive__26499)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildIcon(index)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildLine(!_isLast(index), isActive__26443)) })), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 12.0), child: _buildHeaderText(index)))) })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildVerticalBody(long index)
    {
        double? marginLeft__27311 = this._stepIconMargin?.resolve(TextDirection.ltr).left;
        double? marginRight__27392 = this._stepIconMargin?.resolve(TextDirection.ltr).right;
        double? additionalMarginLeft__27475 = ((marginLeft__27311 is not null) ? (DartRuntimePrimitives.RequireValue(marginLeft__27311) / 2.0) : null);
        double? additionalMarginRight__27562 = ((marginRight__27392 is not null) ? (DartRuntimePrimitives.RequireValue(marginRight__27392) / 2.0) : null);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry effectiveVerticalContentPadding__27754 = ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)(object?)((((Stepper)this.widget).contentPadding ?? StepperLibrary._kDefaultVerticalContentPadding)).add(global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: (marginLeft__27311 ?? 0.0))));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.PositionedDirectional(start: ((24.0 + ((additionalMarginLeft__27475 ?? 0.0))) + ((additionalMarginRight__27562 ?? 0.0))), top: 0.0, bottom: 0.0, width: (this._stepIconWidth ?? StepperLibrary._kStepSize), child: new global::Doroti.Framework.Widgets.Center(child: new global::Doroti.Framework.Widgets.SizedBox(width: (!_isLast(index) ? ((((Stepper)this.widget).connectorThickness ?? 1.0)) : 0.0), height: double.PositiveInfinity, child: new global::Doroti.Framework.Widgets.ColoredBox(color: _connectorColor(((Stepper)this.widget).steps[(int)(index)].isActive)))))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnimatedCrossFade(firstChild: new global::Doroti.Framework.Widgets.SizedBox(width: double.PositiveInfinity, height: 0), secondChild: new global::Doroti.Framework.Widgets.Padding(padding: effectiveVerticalContentPadding__27754, child: new global::Doroti.Framework.Widgets.Column(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ClipRect(clipBehavior: ((Stepper)this.widget).clipBehavior, child: ((Stepper)this.widget).steps[(int)(index)].content)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildVerticalControls(index)) })), firstCurve: new global::Doroti.Framework.Animation.Interval(0.0, 0.6, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn), secondCurve: new global::Doroti.Framework.Animation.Interval(0.4, 1.0, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn), sizeCurve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn, crossFadeState: (_isCurrent(index) ? global::Doroti.Framework.Widgets.CrossFadeState.showSecond : global::Doroti.Framework.Widgets.CrossFadeState.showFirst), duration: ThemeLibrary.kThemeAnimationDuration)) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildVertical()
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ListView(controller: ((Stepper)this.widget).controller, shrinkWrap: true, physics: ((Stepper)this.widget).physics, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() =>
        {
            var __collection29851 = new List<global::Doroti.Framework.Widgets.Widget>(); for (long i__29878 = 0L; (i__29878 < checked((long)(((Stepper)this.widget).steps.Count))); i__29878 += 1L)
            {
                __collection29851.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Column(key: this._keys[(int)(i__29878)], children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new InkWell(onTap: ((global::System.Action)((!object.Equals(((Stepper)this.widget).steps[(int)(i__29878)].state, StepState.disabled)) ? (() => {
DartRuntimePrimitives.Ignore(Scrollable.ensureVisible(this._keys[(int)(i__29878)].currentContext!, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn, duration: ThemeLibrary.kThemeAnimationDuration));
((Stepper)this.widget).onStepTapped?.Invoke(i__29878);
}) : null)), canRequestFocus: (!object.Equals(((Stepper)this.widget).steps[(int)(i__29878)].state, StepState.disabled)), child: _buildVerticalHeader(i__29878))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildVerticalBody(i__29878)) })));
            }
            return __collection29851;
        }))()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildHorizontal()
    {
        global::Doroti.Framework.Painting.EdgeInsetsGeometry effectiveHorizontalContentPadding__30936 = (((Stepper)this.widget).contentPadding ?? StepperLibrary._kDefaultHorizontalContentPadding);
        var children__31051 = ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() =>
        {
            var __collection31062 = new List<global::Doroti.Framework.Widgets.Widget>(); for (long i__31087 = 0L; (i__31087 < checked((long)(((Stepper)this.widget).steps.Count))); i__31087 += 1L)
            {
                __collection31062.AddRange(((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() =>
                {
                    var __collection31130 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection31130.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new InkResponse(onTap: ((global::System.Action)((!object.Equals(((Stepper)this.widget).steps[(int)(i__31087)].state, StepState.disabled)) ? (() =>
                    {
                        ((Stepper)this.widget).onStepTapped?.Invoke(i__31087);
                    }) : null)), canRequestFocus: (!object.Equals(((Stepper)this.widget).steps[(int)(i__31087)].state, StepState.disabled)), child: new global::Doroti.Framework.Widgets.Row(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: (_isLabel() ? 104.0 : 72.0), child: new global::Doroti.Framework.Widgets.Column(mainAxisAlignment: global::Doroti.Framework.Rendering.MainAxisAlignment.center, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection31654 = new List<global::Doroti.Framework.Widgets.Widget>(); if ((((Stepper)this.widget).steps[(int)(i__31087)].label is not null)) { __collection31654.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: 24.0))); } __collection31654.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Center(child: _buildIcon(i__31087)))); if ((((Stepper)this.widget).steps[(int)(i__31087)].label is not null)) { __collection31654.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: 24.0, child: _buildLabelText(i__31087)))); } return __collection31654; }))()))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: (this._stepIconMargin ?? global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 12.0)), child: _buildHeaderText(i__31087))) })))); if (!_isLast(i__31087)) { __collection31130.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Padding(padding: (this._stepIconMargin ?? global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 8.0)), child: new global::Doroti.Framework.Widgets.SizedBox(height: ((((Stepper)this.widget).steps[(int)(i__31087)].stepStyle?.connectorThickness ?? ((Stepper)this.widget).connectorThickness) ?? 1.0), child: new global::Doroti.Framework.Widgets.ColoredBox(color: ((((Stepper)this.widget).steps[(int)(i__31087)].stepStyle?.connectorColor ?? (Color)_connectorColor(((Stepper)this.widget).steps[(int)(i__31087)].isActive))))))))); }
                    return __collection31130;
                }))());
            }
            return __collection31062;
        }))();
        var stepPanels__32839 = new List<global::Doroti.Framework.Widgets.Widget>();
        for (var i__32877 = 0L; (i__32877 < checked((long)(((Stepper)this.widget).steps.Count))); i__32877 += 1L)
        {
            stepPanels__32839.Add(new global::Doroti.Framework.Widgets.Visibility(maintainState: true, visible: (i__32877 == ((Stepper)this.widget).currentStep), child: new global::Doroti.Framework.Widgets.ClipRect(clipBehavior: ((Stepper)this.widget).clipBehavior, child: ((Stepper)this.widget).steps[(int)(i__32877)].content)));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Column(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new Material(elevation: (((Stepper)this.widget).elevation ?? 2), child: new global::Doroti.Framework.Widgets.Padding(padding: this.effectiveHeaderPadding, child: new global::Doroti.Framework.Widgets.SizedBox(height: ((this._stepIconHeight is not null) ? (DartRuntimePrimitives.RequireValue(this._stepIconHeight) * this._heightFactor) : null), child: new global::Doroti.Framework.Widgets.Row(children: children__31051))))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.ListView(controller: ((Stepper)this.widget).controller, physics: ((Stepper)this.widget).physics, padding: effectiveHorizontalContentPadding__30936, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnimatedSize(curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn, duration: ThemeLibrary.kThemeAnimationDuration, child: new global::Doroti.Framework.Widgets.Column(crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.stretch, children: stepPanels__32839))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildVerticalControls(((Stepper)this.widget).currentStep)) }))) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((context.findAncestorWidgetOfExactType<Stepper>() is not null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("Steppers must not be nested.\n" + "The material specification advises that one should avoid embedding " + "steppers within steppers. " + "https://material.io/archive/guidelines/components/steppers.html#steppers-usage"));
                }
                return true;
            });
        return (((Stepper)this.widget).type switch { StepperType.vertical => _buildVertical(), StepperType.horizontal => _buildHorizontal(), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
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
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values__17506).enabled;
    __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
    return __cascade;
}))();
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

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        this._tickerModeNotifier = null;
        base.dispose();
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
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
        var __oldPainter = (_TrianglePainter__stepper)(object)oldDelegate;
        return (!object.Equals(((_TrianglePainter__stepper)__oldPainter).color, this.color));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(Canvas canvas, Size size)
    {
        double @base__35326 = size.width;
        double halfBase__35362 = (size.width / 2.0);
        double height__35408 = size.height;
        var points__35440 = new List<global::Doroti.Ui.Offset> { new global::Doroti.Ui.Offset(0.0, height__35408), new global::Doroti.Ui.Offset(@base__35326, height__35408), new global::Doroti.Ui.Offset(halfBase__35362, 0.0) };
        canvas.drawPath(((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addPolygon(points__35440, true);
    return __cascade;
}))(), ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = this.color;
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
        return new StepStyle(color: (color ?? this.color), errorColor: (errorColor ?? this.errorColor), connectorColor: (connectorColor ?? this.connectorColor), connectorThickness: (connectorThickness ?? this.connectorThickness), border: (border ?? this.border), boxShadow: (boxShadow ?? this.boxShadow), gradient: (gradient ?? this.gradient), indexStyle: (indexStyle ?? this.indexStyle));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual StepStyle merge(StepStyle? stepStyle)
    {
        if ((stepStyle is null))
        {
            return this;
        }
        return ((StepStyle)(object?)copyWith(color: ((StepStyle)stepStyle).color, errorColor: ((StepStyle)stepStyle).errorColor, connectorColor: ((StepStyle)stepStyle).connectorColor, connectorThickness: ((StepStyle)stepStyle).connectorThickness, border: ((StepStyle)stepStyle).border, boxShadow: ((StepStyle)stepStyle).boxShadow, gradient: ((StepStyle)stepStyle).gradient, indexStyle: ((StepStyle)stepStyle).indexStyle));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode()
    {
        return FoundationRuntimePorts.ObjectHash(this.color, this.errorColor, this.connectorColor, this.connectorThickness, this.border, this.boxShadow, this.gradient, this.indexStyle);
        return default!;
    }
    public override bool Equals(object? other)
    {
        var __other = other as StepStyle;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((__other is StepStyle) && (object.Equals(((StepStyle)((StepStyle)__other)).color, this.color))) && (object.Equals(((StepStyle)((StepStyle)__other)).errorColor, this.errorColor))) && (object.Equals(((StepStyle)((StepStyle)__other)).connectorColor, this.connectorColor))) && (((StepStyle)((StepStyle)__other)).connectorThickness == this.connectorThickness)) && (object.Equals(((StepStyle)((StepStyle)__other)).border, this.border))) && (object.Equals(((StepStyle)((StepStyle)__other)).boxShadow, this.boxShadow))) && (object.Equals(((StepStyle)((StepStyle)__other)).gradient, this.gradient))) && (object.Equals(((StepStyle)((StepStyle)__other)).indexStyle, this.indexStyle)));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        var theme__40232 = ThemeData.Create();
        TextTheme defaultTextTheme__40282 = theme__40232.textTheme;
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("color", this.color, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("errorColor", this.errorColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("connectorColor", this.connectorColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("connectorThickness", this.connectorThickness, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.BoxBorder>("border", this.border, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.BoxShadow>("boxShadow", this.boxShadow, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.Gradient>("gradient", this.gradient, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("indexStyle", this.indexStyle, defaultValue: defaultTextTheme__40282.bodyLarge));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
