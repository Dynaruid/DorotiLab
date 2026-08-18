// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/button_style.dart
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Doroti.Framework.Material;

public delegate global::Doroti.Framework.Widgets.Widget ButtonLayerBuilder(global::Doroti.Framework.Widgets.BuildContext context, HashSet<global::Doroti.Framework.Widgets.WidgetState> states, global::Doroti.Framework.Widgets.Widget? child);

public class ButtonStyle : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? textStyle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? foregroundColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? shadowColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? surfaceTintColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? elevation { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>? padding { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Size?>? minimumSize { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Size?>? fixedSize { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Size?>? maximumSize { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? iconColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? iconSize { get; private set; }
    public virtual IconAlignment? iconAlignment { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide?>? side { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder?>? shape { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual MaterialTapTargetSize? tapTargetSize { get; private set; }
    public virtual Duration? animationDuration { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry? alignment { get; private set; }
    public virtual InteractiveInkFeatureFactory? splashFactory { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>? backgroundBuilder { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>? foregroundBuilder { get; private set; }

    public ButtonStyle(global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? textStyle = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? foregroundColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? shadowColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? surfaceTintColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? elevation = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>? padding = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Size?>? minimumSize = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Size?>? fixedSize = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Size?>? maximumSize = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? iconColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? iconSize = null, IconAlignment? iconAlignment = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide?>? side = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder?>? shape = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? tapTargetSize = null, Duration? animationDuration = null, bool? enableFeedback = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, InteractiveInkFeatureFactory? splashFactory = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>? backgroundBuilder = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>? foregroundBuilder = null)
    {
        this.textStyle = textStyle;
        this.backgroundColor = backgroundColor;
        this.foregroundColor = foregroundColor;
        this.overlayColor = overlayColor;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.elevation = elevation;
        this.padding = padding;
        this.minimumSize = minimumSize;
        this.fixedSize = fixedSize;
        this.maximumSize = maximumSize;
        this.iconColor = iconColor;
        this.iconSize = iconSize;
        this.iconAlignment = iconAlignment;
        this.side = side;
        this.shape = shape;
        this.mouseCursor = mouseCursor;
        this.visualDensity = visualDensity;
        this.tapTargetSize = tapTargetSize;
        this.animationDuration = animationDuration;
        this.enableFeedback = enableFeedback;
        this.alignment = alignment;
        this.splashFactory = splashFactory;
        this.backgroundBuilder = backgroundBuilder;
        this.foregroundBuilder = foregroundBuilder;
    }

    public virtual ButtonStyle copyWith(global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? textStyle = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? foregroundColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? shadowColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? surfaceTintColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? elevation = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>? padding = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Size?>? minimumSize = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Size?>? fixedSize = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Size?>? maximumSize = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? iconColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? iconSize = null, IconAlignment? iconAlignment = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide?>? side = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder?>? shape = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? tapTargetSize = null, Duration? animationDuration = null, bool? enableFeedback = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, InteractiveInkFeatureFactory? splashFactory = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>? backgroundBuilder = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>? foregroundBuilder = null)
    {
        return new ButtonStyle(textStyle: (textStyle ?? this.textStyle), backgroundColor: (backgroundColor ?? this.backgroundColor), foregroundColor: (foregroundColor ?? this.foregroundColor), overlayColor: (overlayColor ?? this.overlayColor), shadowColor: (shadowColor ?? this.shadowColor), surfaceTintColor: (surfaceTintColor ?? this.surfaceTintColor), elevation: (elevation ?? this.elevation), padding: (padding ?? this.padding), minimumSize: (minimumSize ?? this.minimumSize), fixedSize: (fixedSize ?? this.fixedSize), maximumSize: (maximumSize ?? this.maximumSize), iconColor: (iconColor ?? this.iconColor), iconSize: (iconSize ?? this.iconSize), iconAlignment: (iconAlignment ?? this.iconAlignment), side: (side ?? this.side), shape: (shape ?? this.shape), mouseCursor: (mouseCursor ?? this.mouseCursor), visualDensity: (visualDensity ?? this.visualDensity), tapTargetSize: (tapTargetSize ?? this.tapTargetSize), animationDuration: (animationDuration ?? this.animationDuration), enableFeedback: (enableFeedback ?? this.enableFeedback), alignment: (alignment ?? this.alignment), splashFactory: (splashFactory ?? this.splashFactory), backgroundBuilder: ((backgroundBuilder ?? (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)this.backgroundBuilder)), foregroundBuilder: ((foregroundBuilder ?? (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)this.foregroundBuilder)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ButtonStyle merge(ButtonStyle? style)
    {
        if ((style is null))
        {
            return this;
        }
        return ((ButtonStyle)(object?)copyWith(textStyle: (this.textStyle ?? ((ButtonStyle)style).textStyle), backgroundColor: (this.backgroundColor ?? ((ButtonStyle)style).backgroundColor), foregroundColor: (this.foregroundColor ?? ((ButtonStyle)style).foregroundColor), overlayColor: (this.overlayColor ?? ((ButtonStyle)style).overlayColor), shadowColor: (this.shadowColor ?? ((ButtonStyle)style).shadowColor), surfaceTintColor: (this.surfaceTintColor ?? ((ButtonStyle)style).surfaceTintColor), elevation: (this.elevation ?? ((ButtonStyle)style).elevation), padding: (this.padding ?? ((ButtonStyle)style).padding), minimumSize: (this.minimumSize ?? ((ButtonStyle)style).minimumSize), fixedSize: (this.fixedSize ?? ((ButtonStyle)style).fixedSize), maximumSize: (this.maximumSize ?? ((ButtonStyle)style).maximumSize), iconColor: (this.iconColor ?? ((ButtonStyle)style).iconColor), iconSize: (this.iconSize ?? ((ButtonStyle)style).iconSize), iconAlignment: (this.iconAlignment ?? ((ButtonStyle)style).iconAlignment), side: (this.side ?? ((ButtonStyle)style).side), shape: (this.shape ?? ((ButtonStyle)style).shape), mouseCursor: (this.mouseCursor ?? ((ButtonStyle)style).mouseCursor), visualDensity: (this.visualDensity ?? ((ButtonStyle)style).visualDensity), tapTargetSize: (this.tapTargetSize ?? ((ButtonStyle)style).tapTargetSize), animationDuration: (this.animationDuration ?? ((ButtonStyle)style).animationDuration), enableFeedback: (this.enableFeedback ?? ((ButtonStyle)style).enableFeedback), alignment: (this.alignment ?? ((ButtonStyle)style).alignment), splashFactory: (this.splashFactory ?? ((ButtonStyle)style).splashFactory), backgroundBuilder: ((this.backgroundBuilder ?? (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((ButtonStyle)style).backgroundBuilder)), foregroundBuilder: ((this.foregroundBuilder ?? (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((ButtonStyle)style).foregroundBuilder))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode()
    {
        var values__20150 = new List<object?> { this.textStyle, this.backgroundColor, this.foregroundColor, this.overlayColor, this.shadowColor, this.surfaceTintColor, this.elevation, this.padding, this.minimumSize, this.fixedSize, this.maximumSize, this.iconColor, this.iconSize, this.iconAlignment, this.side, this.shape, this.mouseCursor, this.visualDensity, this.tapTargetSize, this.animationDuration, this.enableFeedback, this.alignment, this.splashFactory, this.backgroundBuilder, this.foregroundBuilder };
        return FoundationRuntimePorts.ObjectHashAll(values__20150);
        return default!;
    }
    public override bool Equals(object? other)
    {
        var __other = other as ButtonStyle;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((((((((((((((((((__other is ButtonStyle) && (object.Equals(((ButtonStyle)((ButtonStyle)__other)).textStyle, this.textStyle))) && (object.Equals(((ButtonStyle)((ButtonStyle)__other)).backgroundColor, this.backgroundColor))) && (object.Equals(((ButtonStyle)((ButtonStyle)__other)).foregroundColor, this.foregroundColor))) && (object.Equals(((ButtonStyle)((ButtonStyle)__other)).overlayColor, this.overlayColor))) && (object.Equals(((ButtonStyle)((ButtonStyle)__other)).shadowColor, this.shadowColor))) && (object.Equals(((ButtonStyle)((ButtonStyle)__other)).surfaceTintColor, this.surfaceTintColor))) && (object.Equals(((ButtonStyle)((ButtonStyle)__other)).elevation, this.elevation))) && (object.Equals(((ButtonStyle)((ButtonStyle)__other)).padding, this.padding))) && (object.Equals(((ButtonStyle)((ButtonStyle)__other)).minimumSize, this.minimumSize))) && (object.Equals(((ButtonStyle)((ButtonStyle)__other)).fixedSize, this.fixedSize))) && (object.Equals(((ButtonStyle)((ButtonStyle)__other)).maximumSize, this.maximumSize))) && (object.Equals(((ButtonStyle)((ButtonStyle)__other)).iconColor, this.iconColor))) && (object.Equals(((ButtonStyle)((ButtonStyle)__other)).iconSize, this.iconSize))) && (object.Equals(((ButtonStyle)((ButtonStyle)__other)).iconAlignment, this.iconAlignment))) && (object.Equals(((ButtonStyle)((ButtonStyle)__other)).side, this.side))) && (object.Equals(((ButtonStyle)((ButtonStyle)__other)).shape, this.shape))) && (object.Equals(((ButtonStyle)((ButtonStyle)__other)).mouseCursor, this.mouseCursor))) && (object.Equals(((ButtonStyle)((ButtonStyle)__other)).visualDensity, this.visualDensity))) && (object.Equals(((ButtonStyle)((ButtonStyle)__other)).tapTargetSize, this.tapTargetSize))) && (object.Equals(((ButtonStyle)((ButtonStyle)__other)).animationDuration, this.animationDuration))) && (((ButtonStyle)((ButtonStyle)__other)).enableFeedback == this.enableFeedback)) && (object.Equals(((ButtonStyle)((ButtonStyle)__other)).alignment, this.alignment))) && (object.Equals(((ButtonStyle)((ButtonStyle)__other)).splashFactory, this.splashFactory))) && (object.Equals((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>?)((ButtonStyle)((ButtonStyle)__other)).backgroundBuilder, (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>?)this.backgroundBuilder))) && (object.Equals((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>?)((ButtonStyle)((ButtonStyle)__other)).foregroundBuilder, (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>?)this.foregroundBuilder)));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>>("textStyle", this.textStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("backgroundColor", this.backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("foregroundColor", this.foregroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("overlayColor", this.overlayColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("shadowColor", this.shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("surfaceTintColor", this.surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<double?>>("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>>("padding", this.padding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size?>>("minimumSize", this.minimumSize, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size?>>("fixedSize", this.fixedSize, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size?>>("maximumSize", this.maximumSize, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("iconColor", this.iconColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<double?>>("iconSize", this.iconSize, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<IconAlignment>("iconAlignment", this.iconAlignment, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide?>>("side", this.side, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder?>>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>>("mouseCursor", this.mouseCursor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<VisualDensity>("visualDensity", this.visualDensity, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<MaterialTapTargetSize>("tapTargetSize", this.tapTargetSize, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Duration>("animationDuration", this.animationDuration, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("enableFeedback", this.enableFeedback, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.AlignmentGeometry>("alignment", this.alignment, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>>("backgroundBuilder", this.backgroundBuilder, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>>("foregroundBuilder", this.foregroundBuilder, defaultValue: null));
    }

    public static ButtonStyle? lerp(ButtonStyle? a, ButtonStyle? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new ButtonStyle(textStyle: WidgetStateProperty.lerp<global::Doroti.Framework.Painting.TextStyle?>(a?.textStyle, b?.textStyle, t, (global::System.Func<global::Doroti.Framework.Painting.TextStyle?, global::Doroti.Framework.Painting.TextStyle?, double, global::Doroti.Framework.Painting.TextStyle?>)global::Doroti.Framework.Painting.TextStyle.lerp), backgroundColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.backgroundColor, b?.backgroundColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), foregroundColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.foregroundColor, b?.foregroundColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), overlayColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.overlayColor, b?.overlayColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), shadowColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.shadowColor, b?.shadowColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), surfaceTintColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.surfaceTintColor, b?.surfaceTintColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), elevation: WidgetStateProperty.lerp<double?>(a?.elevation, b?.elevation, t, (global::System.Func<double?, double?, double, double?>)Dart_uiLibrary.lerpDouble), padding: WidgetStateProperty.lerp<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>(a?.padding, b?.padding, t, (global::System.Func<global::Doroti.Framework.Painting.EdgeInsetsGeometry?, global::Doroti.Framework.Painting.EdgeInsetsGeometry?, double, global::Doroti.Framework.Painting.EdgeInsetsGeometry?>)global::Doroti.Framework.Painting.EdgeInsetsGeometry.lerp), minimumSize: WidgetStateProperty.lerp<global::Doroti.Ui.Size?>(a?.minimumSize, b?.minimumSize, t, (global::System.Func<Size?, Size?, double, Size?>)Size.lerp), fixedSize: WidgetStateProperty.lerp<global::Doroti.Ui.Size?>(a?.fixedSize, b?.fixedSize, t, (global::System.Func<Size?, Size?, double, Size?>)Size.lerp), maximumSize: WidgetStateProperty.lerp<global::Doroti.Ui.Size?>(a?.maximumSize, b?.maximumSize, t, (global::System.Func<Size?, Size?, double, Size?>)Size.lerp), iconColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.iconColor, b?.iconColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), iconSize: WidgetStateProperty.lerp<double?>(a?.iconSize, b?.iconSize, t, (global::System.Func<double?, double?, double, double?>)Dart_uiLibrary.lerpDouble), iconAlignment: ((t < 0.5) ? a?.iconAlignment : b?.iconAlignment), side: WidgetStateBorderSide.lerp(a?.side, b?.side, t), shape: WidgetStateProperty.lerp<global::Doroti.Framework.Painting.OutlinedBorder?>(a?.shape, b?.shape, t, (global::System.Func<global::Doroti.Framework.Painting.OutlinedBorder?, global::Doroti.Framework.Painting.OutlinedBorder?, double, global::Doroti.Framework.Painting.OutlinedBorder?>)global::Doroti.Framework.Painting.OutlinedBorder.lerp), mouseCursor: ((t < 0.5) ? a?.mouseCursor : b?.mouseCursor), visualDensity: ((t < 0.5) ? a?.visualDensity : b?.visualDensity), tapTargetSize: ((t < 0.5) ? a?.tapTargetSize : b?.tapTargetSize), animationDuration: ((t < 0.5) ? a?.animationDuration : b?.animationDuration), enableFeedback: ((t < 0.5) ? a?.enableFeedback : b?.enableFeedback), alignment: AlignmentGeometry.lerp(a?.alignment, b?.alignment, t), splashFactory: ((t < 0.5) ? a?.splashFactory : b?.splashFactory), backgroundBuilder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((t < 0.5) ? a?.backgroundBuilder : b?.backgroundBuilder)), foregroundBuilder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((t < 0.5) ? a?.foregroundBuilder : b?.foregroundBuilder)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
