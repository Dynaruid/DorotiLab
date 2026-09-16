// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/button_style.dart

using Doroti.Runtime;
using Doroti.Ui;

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
        return new ButtonStyle(textStyle: textStyle ?? this.textStyle, backgroundColor: backgroundColor ?? this.backgroundColor, foregroundColor: foregroundColor ?? this.foregroundColor, overlayColor: overlayColor ?? this.overlayColor, shadowColor: shadowColor ?? this.shadowColor, surfaceTintColor: surfaceTintColor ?? this.surfaceTintColor, elevation: elevation ?? this.elevation, padding: padding ?? this.padding, minimumSize: minimumSize ?? this.minimumSize, fixedSize: fixedSize ?? this.fixedSize, maximumSize: maximumSize ?? this.maximumSize, iconColor: iconColor ?? this.iconColor, iconSize: iconSize ?? this.iconSize, iconAlignment: iconAlignment ?? this.iconAlignment, side: side ?? this.side, shape: shape ?? this.shape, mouseCursor: mouseCursor ?? this.mouseCursor, visualDensity: visualDensity ?? this.visualDensity, tapTargetSize: tapTargetSize ?? this.tapTargetSize, animationDuration: animationDuration ?? this.animationDuration, enableFeedback: enableFeedback ?? this.enableFeedback, alignment: alignment ?? this.alignment, splashFactory: splashFactory ?? this.splashFactory, backgroundBuilder: backgroundBuilder ?? this.backgroundBuilder, foregroundBuilder: foregroundBuilder ?? this.foregroundBuilder);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ButtonStyle merge(ButtonStyle? style)
    {
        if (style is null)
        {
            return this;
        }
        return copyWith(textStyle: textStyle ?? style.textStyle, backgroundColor: backgroundColor ?? style.backgroundColor, foregroundColor: foregroundColor ?? style.foregroundColor, overlayColor: overlayColor ?? style.overlayColor, shadowColor: shadowColor ?? style.shadowColor, surfaceTintColor: surfaceTintColor ?? style.surfaceTintColor, elevation: elevation ?? style.elevation, padding: padding ?? style.padding, minimumSize: minimumSize ?? style.minimumSize, fixedSize: fixedSize ?? style.fixedSize, maximumSize: maximumSize ?? style.maximumSize, iconColor: iconColor ?? style.iconColor, iconSize: iconSize ?? style.iconSize, iconAlignment: iconAlignment ?? style.iconAlignment, side: side ?? style.side, shape: shape ?? style.shape, mouseCursor: mouseCursor ?? style.mouseCursor, visualDensity: visualDensity ?? style.visualDensity, tapTargetSize: tapTargetSize ?? style.tapTargetSize, animationDuration: animationDuration ?? style.animationDuration, enableFeedback: enableFeedback ?? style.enableFeedback, alignment: alignment ?? style.alignment, splashFactory: splashFactory ?? style.splashFactory, backgroundBuilder: backgroundBuilder ?? style.backgroundBuilder, foregroundBuilder: foregroundBuilder ?? style.foregroundBuilder);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode()
    {
        var values = new List<object?> { textStyle, backgroundColor, foregroundColor, overlayColor, shadowColor, surfaceTintColor, elevation, padding, minimumSize, fixedSize, maximumSize, iconColor, iconSize, iconAlignment, side, shape, mouseCursor, visualDensity, tapTargetSize, animationDuration, enableFeedback, alignment, splashFactory, backgroundBuilder, foregroundBuilder };
        return FoundationRuntimePorts.ObjectHashAll(values);
    }
    public override bool Equals(object? other)
    {
        var __other = other as ButtonStyle;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ButtonStyle) && Equals(__other.textStyle, textStyle) && Equals(__other.backgroundColor, backgroundColor) && Equals(__other.foregroundColor, foregroundColor) && Equals(__other.overlayColor, overlayColor) && Equals(__other.shadowColor, shadowColor) && Equals(__other.surfaceTintColor, surfaceTintColor) && Equals(__other.elevation, elevation) && Equals(__other.padding, padding) && Equals(__other.minimumSize, minimumSize) && Equals(__other.fixedSize, fixedSize) && Equals(__other.maximumSize, maximumSize) && Equals(__other.iconColor, iconColor) && Equals(__other.iconSize, iconSize) && Equals(__other.iconAlignment, iconAlignment) && Equals(__other.side, side) && Equals(__other.shape, shape) && Equals(__other.mouseCursor, mouseCursor) && Equals(__other.visualDensity, visualDensity) && Equals(__other.tapTargetSize, tapTargetSize) && Equals(__other.animationDuration, animationDuration) && (__other.enableFeedback == enableFeedback) && Equals(__other.alignment, alignment) && Equals(__other.splashFactory, splashFactory) && Equals(__other.backgroundBuilder, backgroundBuilder) && Equals(__other.foregroundBuilder, foregroundBuilder);
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>>("textStyle", textStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("foregroundColor", foregroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("overlayColor", overlayColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("shadowColor", shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("surfaceTintColor", surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<double?>>("elevation", elevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>>("padding", padding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size?>>("minimumSize", minimumSize, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size?>>("fixedSize", fixedSize, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size?>>("maximumSize", maximumSize, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("iconColor", iconColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<double?>>("iconSize", iconSize, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<IconAlignment>("iconAlignment", iconAlignment, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide?>>("side", side, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder?>>("shape", shape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>>("mouseCursor", mouseCursor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<VisualDensity>("visualDensity", visualDensity, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<MaterialTapTargetSize>("tapTargetSize", tapTargetSize, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Duration>("animationDuration", animationDuration, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("enableFeedback", enableFeedback, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.AlignmentGeometry>("alignment", alignment, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>>("backgroundBuilder", backgroundBuilder, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>>("foregroundBuilder", foregroundBuilder, defaultValue: null));
    }

    public static ButtonStyle? lerp(ButtonStyle? a, ButtonStyle? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new ButtonStyle(textStyle: WidgetStateProperty.lerp<global::Doroti.Framework.Painting.TextStyle?>(a?.textStyle, b?.textStyle, t, TextStyle.lerp), backgroundColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.backgroundColor, b?.backgroundColor, t, Color.lerp), foregroundColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.foregroundColor, b?.foregroundColor, t, Color.lerp), overlayColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.overlayColor, b?.overlayColor, t, Color.lerp), shadowColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.shadowColor, b?.shadowColor, t, Color.lerp), surfaceTintColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.surfaceTintColor, b?.surfaceTintColor, t, Color.lerp), elevation: WidgetStateProperty.lerp<double?>(a?.elevation, b?.elevation, t, Dart_uiLibrary.lerpDouble), padding: WidgetStateProperty.lerp<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>(a?.padding, b?.padding, t, EdgeInsetsGeometry.lerp), minimumSize: WidgetStateProperty.lerp<global::Doroti.Ui.Size?>(a?.minimumSize, b?.minimumSize, t, Size.lerp), fixedSize: WidgetStateProperty.lerp<global::Doroti.Ui.Size?>(a?.fixedSize, b?.fixedSize, t, Size.lerp), maximumSize: WidgetStateProperty.lerp<global::Doroti.Ui.Size?>(a?.maximumSize, b?.maximumSize, t, Size.lerp), iconColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.iconColor, b?.iconColor, t, Color.lerp), iconSize: WidgetStateProperty.lerp<double?>(a?.iconSize, b?.iconSize, t, Dart_uiLibrary.lerpDouble), iconAlignment: (t < 0.5) ? a?.iconAlignment : b?.iconAlignment, side: WidgetStateBorderSide.lerp(a?.side, b?.side, t), shape: WidgetStateProperty.lerp<global::Doroti.Framework.Painting.OutlinedBorder?>(a?.shape, b?.shape, t, OutlinedBorder.lerp), mouseCursor: (t < 0.5) ? a?.mouseCursor : b?.mouseCursor, visualDensity: (t < 0.5) ? a?.visualDensity : b?.visualDensity, tapTargetSize: (t < 0.5) ? a?.tapTargetSize : b?.tapTargetSize, animationDuration: (t < 0.5) ? a?.animationDuration : b?.animationDuration, enableFeedback: (t < 0.5) ? a?.enableFeedback : b?.enableFeedback, alignment: AlignmentGeometry.lerp(a?.alignment, b?.alignment, t), splashFactory: (t < 0.5) ? a?.splashFactory : b?.splashFactory, backgroundBuilder: (t < 0.5) ? a?.backgroundBuilder : b?.backgroundBuilder, foregroundBuilder: (t < 0.5) ? a?.foregroundBuilder : b?.foregroundBuilder);
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
