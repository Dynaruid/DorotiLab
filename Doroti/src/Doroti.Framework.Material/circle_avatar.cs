// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/circle_avatar.dart
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

public class CircleAvatar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? foregroundColor { get; private set; }
    public virtual dynamic backgroundImage { get; private set; } = default!;
    public virtual dynamic foregroundImage { get; private set; } = default!;
    public virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? onBackgroundImageError { get; private set; }
    public virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? onForegroundImageError { get; private set; }
    public virtual double? radius { get; private set; }
    public virtual double? minRadius { get; private set; }
    public virtual double? maxRadius { get; private set; }
    internal const double _defaultRadius = 20.0;
    internal const double _defaultMinRadius = 0.0;
    internal static double _defaultMaxRadius = double.PositiveInfinity;

    public CircleAvatar(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? child = null, Color? backgroundColor = null, dynamic backgroundImage = null, dynamic foregroundImage = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onBackgroundImageError = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onForegroundImageError = null, Color? foregroundColor = null, double? radius = null, double? minRadius = null, double? maxRadius = null) : base(key: key)
    {
        this.child = child;
        this.backgroundColor = backgroundColor;
        this.backgroundImage = backgroundImage;
        this.foregroundImage = foregroundImage;
        this.onBackgroundImageError = onBackgroundImageError;
        this.onForegroundImageError = onForegroundImageError;
        this.foregroundColor = foregroundColor;
        this.radius = radius;
        this.minRadius = minRadius;
        this.maxRadius = maxRadius;
        System.Diagnostics.Debug.Assert(((radius is null) || (((minRadius is null) && (maxRadius is null)))));
        System.Diagnostics.Debug.Assert(((backgroundImage is not null) || (onBackgroundImageError is null)));
        System.Diagnostics.Debug.Assert(((foregroundImage is not null) || (onForegroundImageError is null)));
    }

    internal virtual double _minDiameter
    {
        get
        {
            if ((((this.radius is null) && (this.minRadius is null)) && (this.maxRadius is null)))
            {
                return (_defaultRadius * 2.0);
            }
            return (2.0 * (((this.radius ?? this.minRadius) ?? _defaultMinRadius)));
            return default!;
        }
    }
    internal virtual double _maxDiameter
    {
        get
        {
            if ((((this.radius is null) && (this.minRadius is null)) && (this.maxRadius is null)))
            {
                return (_defaultRadius * 2.0);
            }
            return (2.0 * (((this.radius ?? this.maxRadius) ?? _defaultMaxRadius)));
            return default!;
        }
    }
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        ThemeData theme__7412 = Theme.of(context);
        global::Doroti.Ui.Color? effectiveForegroundColor__7456 = ((global::Doroti.Ui.Color?)(object?)(this.foregroundColor ?? ((theme__7412.useMaterial3 ? theme__7412.colorScheme.onPrimaryContainer : null))));
        global::Doroti.Framework.Painting.TextStyle effectiveTextStyle__7598 = (theme__7412.useMaterial3 ? theme__7412.textTheme.titleMedium! : theme__7412.primaryTextTheme.titleMedium!);
        global::Doroti.Framework.Painting.TextStyle textStyle__7738 = ((global::Doroti.Framework.Painting.TextStyle)(object?)effectiveTextStyle__7598.copyWith(color: effectiveForegroundColor__7456));
        global::Doroti.Ui.Color? effectiveBackgroundColor__7823 = ((global::Doroti.Ui.Color?)(object?)(this.backgroundColor ?? ((theme__7412.useMaterial3 ? theme__7412.colorScheme.primaryContainer : null))));
        if ((effectiveBackgroundColor__7823 is null))
        {
            effectiveBackgroundColor__7823 = (ThemeData.estimateBrightnessForColor(((global::Doroti.Framework.Painting.TextStyle)textStyle__7738).color!) switch { Brightness.dark => theme__7412.primaryColorLight, Brightness.light => theme__7412.primaryColorDark, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        }
        else
        {
            if ((effectiveForegroundColor__7456 is null))
            {
                textStyle__7738 = (ThemeData.estimateBrightnessForColor(this.backgroundColor!) switch { Brightness.dark => textStyle__7738.copyWith(color: theme__7412.primaryColorLight), Brightness.light => textStyle__7738.copyWith(color: theme__7412.primaryColorDark), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            }
        }
        double minDiameter__8524 = this._minDiameter;
        double maxDiameter__8569 = this._maxDiameter;
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.AnimatedContainer(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minHeight: minDiameter__8524, minWidth: minDiameter__8524, maxWidth: maxDiameter__8569, maxHeight: maxDiameter__8569), duration: ConstantsLibrary.kThemeChangeDuration, decoration: new global::Doroti.Framework.Painting.BoxDecoration(color: effectiveBackgroundColor__7823, image: ((this.backgroundImage is not null) ? new global::Doroti.Framework.Painting.DecorationImage(image: this.backgroundImage!, onError: (global::System.Action<object, global::System.Diagnostics.StackTrace?>?)this.onBackgroundImageError, fit: global::Doroti.Framework.Painting.BoxFit.cover) : null), shape: global::Doroti.Framework.Painting.BoxShape.circle), foregroundDecoration: ((this.foregroundImage is not null) ? new global::Doroti.Framework.Painting.BoxDecoration(image: new global::Doroti.Framework.Painting.DecorationImage(image: this.foregroundImage!, onError: (global::System.Action<object, global::System.Diagnostics.StackTrace?>?)this.onForegroundImageError, fit: global::Doroti.Framework.Painting.BoxFit.cover), shape: global::Doroti.Framework.Painting.BoxShape.circle) : null), child: ((this.child is null) ? null : new global::Doroti.Framework.Widgets.Center(child: MediaQuery.withNoTextScaling(child: new global::Doroti.Framework.Widgets.IconTheme(data: theme__7412.iconTheme.copyWith(color: ((global::Doroti.Framework.Painting.TextStyle)textStyle__7738).color), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: textStyle__7738, child: this.child!)))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
