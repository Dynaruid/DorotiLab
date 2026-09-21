// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/button_style.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public delegate Widget ButtonLayerBuilder(
    BuildContext context,
    HashSet<WidgetState> states,
    Widget? child
);

public class ButtonStyle : Diagnosticable
{
    public virtual WidgetStateProperty<TextStyle?>? textStyle { get; private set; }
    public virtual WidgetStateProperty<Color?>? backgroundColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? foregroundColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? shadowColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? surfaceTintColor { get; private set; }
    public virtual WidgetStateProperty<double?>? elevation { get; private set; }
    public virtual WidgetStateProperty<EdgeInsetsGeometry?>? padding { get; private set; }
    public virtual WidgetStateProperty<Size?>? minimumSize { get; private set; }
    public virtual WidgetStateProperty<Size?>? fixedSize { get; private set; }
    public virtual WidgetStateProperty<Size?>? maximumSize { get; private set; }
    public virtual WidgetStateProperty<Color?>? iconColor { get; private set; }
    public virtual WidgetStateProperty<double?>? iconSize { get; private set; }
    public virtual IconAlignment? iconAlignment { get; private set; }
    public virtual WidgetStateProperty<BorderSide?>? side { get; private set; }
    public virtual WidgetStateProperty<OutlinedBorder?>? shape { get; private set; }
    public virtual WidgetStateProperty<MouseCursor?>? mouseCursor { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual MaterialTapTargetSize? tapTargetSize { get; private set; }
    public virtual Duration? animationDuration { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual AlignmentGeometry? alignment { get; private set; }
    public virtual InteractiveInkFeatureFactory? splashFactory { get; private set; }
    public virtual Func<BuildContext, HashSet<WidgetState>, Widget?, Widget>? backgroundBuilder
    {
        get;
        private set;
    }
    public virtual Func<BuildContext, HashSet<WidgetState>, Widget?, Widget>? foregroundBuilder
    {
        get;
        private set;
    }

    public ButtonStyle(
        WidgetStateProperty<TextStyle?>? textStyle = null,
        WidgetStateProperty<Color?>? backgroundColor = null,
        WidgetStateProperty<Color?>? foregroundColor = null,
        WidgetStateProperty<Color?>? overlayColor = null,
        WidgetStateProperty<Color?>? shadowColor = null,
        WidgetStateProperty<Color?>? surfaceTintColor = null,
        WidgetStateProperty<double?>? elevation = null,
        WidgetStateProperty<EdgeInsetsGeometry?>? padding = null,
        WidgetStateProperty<Size?>? minimumSize = null,
        WidgetStateProperty<Size?>? fixedSize = null,
        WidgetStateProperty<Size?>? maximumSize = null,
        WidgetStateProperty<Color?>? iconColor = null,
        WidgetStateProperty<double?>? iconSize = null,
        IconAlignment? iconAlignment = null,
        WidgetStateProperty<BorderSide?>? side = null,
        WidgetStateProperty<OutlinedBorder?>? shape = null,
        WidgetStateProperty<MouseCursor?>? mouseCursor = null,
        VisualDensity? visualDensity = null,
        MaterialTapTargetSize? tapTargetSize = null,
        Duration? animationDuration = null,
        bool? enableFeedback = null,
        AlignmentGeometry? alignment = null,
        InteractiveInkFeatureFactory? splashFactory = null,
        Func<BuildContext, HashSet<WidgetState>, Widget?, Widget>? backgroundBuilder = null,
        Func<BuildContext, HashSet<WidgetState>, Widget?, Widget>? foregroundBuilder = null
    )
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

    public virtual ButtonStyle copyWith(
        WidgetStateProperty<TextStyle?>? textStyle = null,
        WidgetStateProperty<Color?>? backgroundColor = null,
        WidgetStateProperty<Color?>? foregroundColor = null,
        WidgetStateProperty<Color?>? overlayColor = null,
        WidgetStateProperty<Color?>? shadowColor = null,
        WidgetStateProperty<Color?>? surfaceTintColor = null,
        WidgetStateProperty<double?>? elevation = null,
        WidgetStateProperty<EdgeInsetsGeometry?>? padding = null,
        WidgetStateProperty<Size?>? minimumSize = null,
        WidgetStateProperty<Size?>? fixedSize = null,
        WidgetStateProperty<Size?>? maximumSize = null,
        WidgetStateProperty<Color?>? iconColor = null,
        WidgetStateProperty<double?>? iconSize = null,
        IconAlignment? iconAlignment = null,
        WidgetStateProperty<BorderSide?>? side = null,
        WidgetStateProperty<OutlinedBorder?>? shape = null,
        WidgetStateProperty<MouseCursor?>? mouseCursor = null,
        VisualDensity? visualDensity = null,
        MaterialTapTargetSize? tapTargetSize = null,
        Duration? animationDuration = null,
        bool? enableFeedback = null,
        AlignmentGeometry? alignment = null,
        InteractiveInkFeatureFactory? splashFactory = null,
        Func<BuildContext, HashSet<WidgetState>, Widget?, Widget>? backgroundBuilder = null,
        Func<BuildContext, HashSet<WidgetState>, Widget?, Widget>? foregroundBuilder = null
    )
    {
        return new ButtonStyle(
            textStyle: textStyle ?? this.textStyle,
            backgroundColor: backgroundColor ?? this.backgroundColor,
            foregroundColor: foregroundColor ?? this.foregroundColor,
            overlayColor: overlayColor ?? this.overlayColor,
            shadowColor: shadowColor ?? this.shadowColor,
            surfaceTintColor: surfaceTintColor ?? this.surfaceTintColor,
            elevation: elevation ?? this.elevation,
            padding: padding ?? this.padding,
            minimumSize: minimumSize ?? this.minimumSize,
            fixedSize: fixedSize ?? this.fixedSize,
            maximumSize: maximumSize ?? this.maximumSize,
            iconColor: iconColor ?? this.iconColor,
            iconSize: iconSize ?? this.iconSize,
            iconAlignment: iconAlignment ?? this.iconAlignment,
            side: side ?? this.side,
            shape: shape ?? this.shape,
            mouseCursor: mouseCursor ?? this.mouseCursor,
            visualDensity: visualDensity ?? this.visualDensity,
            tapTargetSize: tapTargetSize ?? this.tapTargetSize,
            animationDuration: animationDuration ?? this.animationDuration,
            enableFeedback: enableFeedback ?? this.enableFeedback,
            alignment: alignment ?? this.alignment,
            splashFactory: splashFactory ?? this.splashFactory,
            backgroundBuilder: backgroundBuilder ?? this.backgroundBuilder,
            foregroundBuilder: foregroundBuilder ?? this.foregroundBuilder
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual ButtonStyle merge(ButtonStyle? style)
    {
        if (style is null)
        {
            return this;
        }
        return copyWith(
            textStyle: textStyle ?? style.textStyle,
            backgroundColor: backgroundColor ?? style.backgroundColor,
            foregroundColor: foregroundColor ?? style.foregroundColor,
            overlayColor: overlayColor ?? style.overlayColor,
            shadowColor: shadowColor ?? style.shadowColor,
            surfaceTintColor: surfaceTintColor ?? style.surfaceTintColor,
            elevation: elevation ?? style.elevation,
            padding: padding ?? style.padding,
            minimumSize: minimumSize ?? style.minimumSize,
            fixedSize: fixedSize ?? style.fixedSize,
            maximumSize: maximumSize ?? style.maximumSize,
            iconColor: iconColor ?? style.iconColor,
            iconSize: iconSize ?? style.iconSize,
            iconAlignment: iconAlignment ?? style.iconAlignment,
            side: side ?? style.side,
            shape: shape ?? style.shape,
            mouseCursor: mouseCursor ?? style.mouseCursor,
            visualDensity: visualDensity ?? style.visualDensity,
            tapTargetSize: tapTargetSize ?? style.tapTargetSize,
            animationDuration: animationDuration ?? style.animationDuration,
            enableFeedback: enableFeedback ?? style.enableFeedback,
            alignment: alignment ?? style.alignment,
            splashFactory: splashFactory ?? style.splashFactory,
            backgroundBuilder: backgroundBuilder ?? style.backgroundBuilder,
            foregroundBuilder: foregroundBuilder ?? style.foregroundBuilder
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override int GetHashCode()
    {
        var values = new List<object?>
        {
            textStyle,
            backgroundColor,
            foregroundColor,
            overlayColor,
            shadowColor,
            surfaceTintColor,
            elevation,
            padding,
            minimumSize,
            fixedSize,
            maximumSize,
            iconColor,
            iconSize,
            iconAlignment,
            side,
            shape,
            mouseCursor,
            visualDensity,
            tapTargetSize,
            animationDuration,
            enableFeedback,
            alignment,
            splashFactory,
            backgroundBuilder,
            foregroundBuilder,
        };
        return FoundationRuntimePorts.ObjectHashAll(values);
    }

    public override bool Equals(object? other)
    {
        var __other = other as ButtonStyle;
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
        return (__other is ButtonStyle)
            && Equals(__other.textStyle, textStyle)
            && Equals(__other.backgroundColor, backgroundColor)
            && Equals(__other.foregroundColor, foregroundColor)
            && Equals(__other.overlayColor, overlayColor)
            && Equals(__other.shadowColor, shadowColor)
            && Equals(__other.surfaceTintColor, surfaceTintColor)
            && Equals(__other.elevation, elevation)
            && Equals(__other.padding, padding)
            && Equals(__other.minimumSize, minimumSize)
            && Equals(__other.fixedSize, fixedSize)
            && Equals(__other.maximumSize, maximumSize)
            && Equals(__other.iconColor, iconColor)
            && Equals(__other.iconSize, iconSize)
            && Equals(__other.iconAlignment, iconAlignment)
            && Equals(__other.side, side)
            && Equals(__other.shape, shape)
            && Equals(__other.mouseCursor, mouseCursor)
            && Equals(__other.visualDensity, visualDensity)
            && Equals(__other.tapTargetSize, tapTargetSize)
            && Equals(__other.animationDuration, animationDuration)
            && (__other.enableFeedback == enableFeedback)
            && Equals(__other.alignment, alignment)
            && Equals(__other.splashFactory, splashFactory)
            && Equals(__other.backgroundBuilder, backgroundBuilder)
            && Equals(__other.foregroundBuilder, foregroundBuilder);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<TextStyle?>>(
                "textStyle",
                textStyle,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "backgroundColor",
                backgroundColor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "foregroundColor",
                foregroundColor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "overlayColor",
                overlayColor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "shadowColor",
                shadowColor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "surfaceTintColor",
                surfaceTintColor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<double?>>(
                "elevation",
                elevation,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<EdgeInsetsGeometry?>>(
                "padding",
                padding,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Size?>>(
                "minimumSize",
                minimumSize,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Size?>>(
                "fixedSize",
                fixedSize,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Size?>>(
                "maximumSize",
                maximumSize,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "iconColor",
                iconColor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<double?>>(
                "iconSize",
                iconSize,
                defaultValue: null
            )
        );
        properties.add(
            new EnumProperty<IconAlignment>("iconAlignment", iconAlignment, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<BorderSide?>>(
                "side",
                side,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<OutlinedBorder?>>(
                "shape",
                shape,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<MouseCursor?>>(
                "mouseCursor",
                mouseCursor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<VisualDensity>(
                "visualDensity",
                visualDensity,
                defaultValue: null
            )
        );
        properties.add(
            new EnumProperty<MaterialTapTargetSize>(
                "tapTargetSize",
                tapTargetSize,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<Duration>(
                "animationDuration",
                animationDuration,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<bool>("enableFeedback", enableFeedback, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<AlignmentGeometry>("alignment", alignment, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<Func<BuildContext, HashSet<WidgetState>, Widget?, Widget>>(
                "backgroundBuilder",
                backgroundBuilder,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<Func<BuildContext, HashSet<WidgetState>, Widget?, Widget>>(
                "foregroundBuilder",
                foregroundBuilder,
                defaultValue: null
            )
        );
    }

    public static ButtonStyle? lerp(ButtonStyle? a, ButtonStyle? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new ButtonStyle(
            textStyle: WidgetStateProperty.lerp(a?.textStyle, b?.textStyle, t, TextStyle.lerp),
            backgroundColor: WidgetStateProperty.lerp(
                a?.backgroundColor,
                b?.backgroundColor,
                t,
                Color.lerp
            ),
            foregroundColor: WidgetStateProperty.lerp(
                a?.foregroundColor,
                b?.foregroundColor,
                t,
                Color.lerp
            ),
            overlayColor: WidgetStateProperty.lerp(a?.overlayColor, b?.overlayColor, t, Color.lerp),
            shadowColor: WidgetStateProperty.lerp(a?.shadowColor, b?.shadowColor, t, Color.lerp),
            surfaceTintColor: WidgetStateProperty.lerp(
                a?.surfaceTintColor,
                b?.surfaceTintColor,
                t,
                Color.lerp
            ),
            elevation: WidgetStateProperty.lerp(
                a?.elevation,
                b?.elevation,
                t,
                Dart_uiLibrary.lerpDouble
            ),
            padding: WidgetStateProperty.lerp(a?.padding, b?.padding, t, EdgeInsetsGeometry.lerp),
            minimumSize: WidgetStateProperty.lerp(a?.minimumSize, b?.minimumSize, t, Size.lerp),
            fixedSize: WidgetStateProperty.lerp(a?.fixedSize, b?.fixedSize, t, Size.lerp),
            maximumSize: WidgetStateProperty.lerp(a?.maximumSize, b?.maximumSize, t, Size.lerp),
            iconColor: WidgetStateProperty.lerp(a?.iconColor, b?.iconColor, t, Color.lerp),
            iconSize: WidgetStateProperty.lerp(
                a?.iconSize,
                b?.iconSize,
                t,
                Dart_uiLibrary.lerpDouble
            ),
            iconAlignment: (t < 0.5) ? a?.iconAlignment : b?.iconAlignment,
            side: WidgetStateBorderSide.lerp(a?.side, b?.side, t),
            shape: WidgetStateProperty.lerp(a?.shape, b?.shape, t, OutlinedBorder.lerp),
            mouseCursor: (t < 0.5) ? a?.mouseCursor : b?.mouseCursor,
            visualDensity: (t < 0.5) ? a?.visualDensity : b?.visualDensity,
            tapTargetSize: (t < 0.5) ? a?.tapTargetSize : b?.tapTargetSize,
            animationDuration: (t < 0.5) ? a?.animationDuration : b?.animationDuration,
            enableFeedback: (t < 0.5) ? a?.enableFeedback : b?.enableFeedback,
            alignment: AlignmentGeometry.lerp(a?.alignment, b?.alignment, t),
            splashFactory: (t < 0.5) ? a?.splashFactory : b?.splashFactory,
            backgroundBuilder: (t < 0.5) ? a?.backgroundBuilder : b?.backgroundBuilder,
            foregroundBuilder: (t < 0.5) ? a?.foregroundBuilder : b?.foregroundBuilder
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = null
    )
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
