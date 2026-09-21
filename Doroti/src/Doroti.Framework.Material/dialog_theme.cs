// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/dialog_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class DialogTheme : InheritedTheme, Diagnosticable
{
    internal virtual DialogThemeData? _data { get; private set; }
    internal virtual Color? _backgroundColor { get; private set; }
    internal virtual double? _elevation { get; private set; }
    internal virtual Color? _shadowColor { get; private set; }
    internal virtual Color? _surfaceTintColor { get; private set; }
    internal virtual ShapeBorder? _shape { get; private set; }
    internal virtual AlignmentGeometry? _alignment { get; private set; }
    internal virtual TextStyle? _titleTextStyle { get; private set; }
    internal virtual TextStyle? _contentTextStyle { get; private set; }
    internal virtual EdgeInsetsGeometry? _actionsPadding { get; private set; }
    internal virtual Color? _iconColor { get; private set; }
    internal virtual Color? _barrierColor { get; private set; }
    internal virtual EdgeInsets? _insetPadding { get; private set; }
    internal virtual Clip? _clipBehavior { get; private set; }

    public DialogTheme(
        Key? key = null,
        Color? backgroundColor = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        ShapeBorder? shape = null,
        AlignmentGeometry? alignment = null,
        Color? iconColor = null,
        TextStyle? titleTextStyle = null,
        TextStyle? contentTextStyle = null,
        EdgeInsetsGeometry? actionsPadding = null,
        Color? barrierColor = null,
        EdgeInsets? insetPadding = null,
        Clip? clipBehavior = null,
        DialogThemeData? data = null,
        Widget? child = null
    )
        : base(key: key, child: child ?? new SizedBox())
    {
        _data = data;
        _backgroundColor = backgroundColor;
        _elevation = elevation;
        _shadowColor = shadowColor;
        _surfaceTintColor = surfaceTintColor;
        _shape = shape;
        _alignment = alignment;
        _iconColor = iconColor;
        _titleTextStyle = titleTextStyle;
        _contentTextStyle = contentTextStyle;
        _actionsPadding = actionsPadding;
        _barrierColor = barrierColor;
        _insetPadding = insetPadding;
        _clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert(
            (data is null)
                || (
                    (
                        (
                            (
                                (
                                    (
                                        (
                                            (
                                                (
                                                    (
                                                        (
                                                            (
                                                                (
                                                                    (object?)backgroundColor
                                                                    ?? elevation
                                                                ) ?? shadowColor
                                                            ) ?? surfaceTintColor
                                                        ) ?? shape
                                                    ) ?? alignment
                                                ) ?? iconColor
                                            ) ?? titleTextStyle
                                        ) ?? contentTextStyle
                                    ) ?? actionsPadding
                                ) ?? barrierColor
                            ) ?? insetPadding
                        ) ?? clipBehavior
                    )
                    is null
                )
        );
    }

    public virtual Color? backgroundColor =>
        DartRuntimePrimitives.ConvertValue<Color>(
            (_data is not null) ? _data.backgroundColor : _backgroundColor
        );
    public virtual double? elevation => (_data is not null) ? _data.elevation : _elevation;
    public virtual Color? shadowColor =>
        DartRuntimePrimitives.ConvertValue<Color>(
            (_data is not null) ? _data.shadowColor : _shadowColor
        );
    public virtual Color? surfaceTintColor =>
        DartRuntimePrimitives.ConvertValue<Color>(
            (_data is not null) ? _data.surfaceTintColor : _surfaceTintColor
        );
    public virtual ShapeBorder? shape => (_data is not null) ? _data.shape : _shape;
    public virtual AlignmentGeometry? alignment =>
        (_data is not null) ? _data.alignment : _alignment;
    public virtual TextStyle? titleTextStyle =>
        (_data is not null) ? _data.titleTextStyle : _titleTextStyle;
    public virtual TextStyle? contentTextStyle =>
        (_data is not null) ? _data.contentTextStyle : _contentTextStyle;
    public virtual EdgeInsetsGeometry? actionsPadding =>
        (_data is not null) ? _data.actionsPadding : _actionsPadding;
    public virtual Color? iconColor =>
        DartRuntimePrimitives.ConvertValue<Color>(
            (_data is not null) ? _data.iconColor : _iconColor
        );
    public virtual Color? barrierColor =>
        DartRuntimePrimitives.ConvertValue<Color>(
            (_data is not null) ? _data.barrierColor : _barrierColor
        );
    public virtual EdgeInsets? insetPadding =>
        (_data is not null) ? _data.insetPadding : _insetPadding;
    public virtual Clip? clipBehavior =>
        DartRuntimePrimitives.ConvertValue<Clip>(
            (_data is not null) ? _data.clipBehavior : _clipBehavior
        );
    public virtual DialogThemeData data
    {
        get
        {
            return _data
                ?? new DialogThemeData(
                    backgroundColor: _backgroundColor,
                    elevation: _elevation,
                    shadowColor: _shadowColor,
                    surfaceTintColor: _surfaceTintColor,
                    shape: _shape,
                    alignment: _alignment,
                    iconColor: _iconColor,
                    titleTextStyle: _titleTextStyle,
                    contentTextStyle: _contentTextStyle,
                    actionsPadding: _actionsPadding,
                    barrierColor: _barrierColor,
                    insetPadding: _insetPadding,
                    clipBehavior: _clipBehavior
                );
        }
    }

    public static DialogThemeData of(BuildContext context)
    {
        DialogTheme? dialogThemeLocal = context.dependOnInheritedWidgetOfExactType<DialogTheme>();
        return dialogThemeLocal?.data ?? Theme.of(context).dialogTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new DialogTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((DialogTheme)oldWidget).data));

    public virtual DialogTheme copyWith(
        Color? backgroundColor = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        ShapeBorder? shape = null,
        AlignmentGeometry? alignment = null,
        Color? iconColor = null,
        TextStyle? titleTextStyle = null,
        TextStyle? contentTextStyle = null,
        EdgeInsetsGeometry? actionsPadding = null,
        Color? barrierColor = null,
        EdgeInsets? insetPadding = null,
        Clip? clipBehavior = null
    )
    {
        return new DialogTheme(
            backgroundColor: backgroundColor ?? this.backgroundColor,
            elevation: elevation ?? this.elevation,
            shadowColor: shadowColor ?? this.shadowColor,
            surfaceTintColor: surfaceTintColor ?? this.surfaceTintColor,
            shape: shape ?? this.shape,
            alignment: alignment ?? this.alignment,
            iconColor: iconColor ?? this.iconColor,
            titleTextStyle: titleTextStyle ?? this.titleTextStyle,
            contentTextStyle: contentTextStyle ?? this.contentTextStyle,
            actionsPadding: actionsPadding ?? this.actionsPadding,
            barrierColor: barrierColor ?? this.barrierColor,
            insetPadding: insetPadding ?? this.insetPadding,
            clipBehavior: clipBehavior ?? this.clipBehavior
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DialogTheme lerp(DialogTheme? a, DialogTheme? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new DialogTheme(
            backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t),
            elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t),
            shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t),
            surfaceTintColor: Dart_uiLibrary.Color.lerp(
                a?.surfaceTintColor,
                b?.surfaceTintColor,
                t
            ),
            shape: ShapeBorder.lerp(a?.shape, b?.shape, t),
            alignment: AlignmentGeometry.lerp(a?.alignment, b?.alignment, t),
            iconColor: Dart_uiLibrary.Color.lerp(a?.iconColor, b?.iconColor, t),
            titleTextStyle: TextStyle.lerp(a?.titleTextStyle, b?.titleTextStyle, t),
            contentTextStyle: TextStyle.lerp(a?.contentTextStyle, b?.contentTextStyle, t),
            actionsPadding: EdgeInsetsGeometry.lerp(a?.actionsPadding, b?.actionsPadding, t),
            barrierColor: Dart_uiLibrary.Color.lerp(a?.barrierColor, b?.barrierColor, t),
            insetPadding: EdgeInsets.lerp(a?.insetPadding, b?.insetPadding, t),
            clipBehavior: (t < 0.5) ? a?.clipBehavior : b?.clipBehavior
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new ColorProperty("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new DoubleProperty("elevation", elevation, defaultValue: null));
        properties.add(new ColorProperty("shadowColor", shadowColor, defaultValue: null));
        properties.add(new ColorProperty("surfaceTintColor", surfaceTintColor, defaultValue: null));
        properties.add(new DiagnosticsProperty<ShapeBorder>("shape", shape, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<AlignmentGeometry>("alignment", alignment, defaultValue: null)
        );
        properties.add(new ColorProperty("iconColor", iconColor, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<TextStyle>("titleTextStyle", titleTextStyle, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "contentTextStyle",
                contentTextStyle,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<EdgeInsetsGeometry>(
                "actionsPadding",
                actionsPadding,
                defaultValue: null
            )
        );
        properties.add(new ColorProperty("barrierColor", barrierColor, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<EdgeInsets>("insetPadding", insetPadding, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<Clip>("clipBehavior", clipBehavior, defaultValue: null)
        );
    }

    public override string toStringShort() => DiagnosticsLibrary.describeIdentity(this);

    public override string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
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

    public override DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = null
    )
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class DialogThemeData : Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual ShapeBorder? shape { get; private set; }
    public virtual AlignmentGeometry? alignment { get; private set; }
    public virtual TextStyle? titleTextStyle { get; private set; }
    public virtual TextStyle? contentTextStyle { get; private set; }
    public virtual EdgeInsetsGeometry? actionsPadding { get; private set; }
    public virtual Color? iconColor { get; private set; }
    public virtual Color? barrierColor { get; private set; }
    public virtual EdgeInsets? insetPadding { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual BoxConstraints? constraints { get; private set; }

    public DialogThemeData(
        Color? backgroundColor = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        ShapeBorder? shape = null,
        AlignmentGeometry? alignment = null,
        Color? iconColor = null,
        TextStyle? titleTextStyle = null,
        TextStyle? contentTextStyle = null,
        EdgeInsetsGeometry? actionsPadding = null,
        Color? barrierColor = null,
        EdgeInsets? insetPadding = null,
        Clip? clipBehavior = null,
        BoxConstraints? constraints = null
    )
    {
        this.backgroundColor = backgroundColor;
        this.elevation = elevation;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.shape = shape;
        this.alignment = alignment;
        this.iconColor = iconColor;
        this.titleTextStyle = titleTextStyle;
        this.contentTextStyle = contentTextStyle;
        this.actionsPadding = actionsPadding;
        this.barrierColor = barrierColor;
        this.insetPadding = insetPadding;
        this.clipBehavior = clipBehavior;
        this.constraints = constraints;
    }

    public virtual DialogThemeData copyWith(
        Color? backgroundColor = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        ShapeBorder? shape = null,
        AlignmentGeometry? alignment = null,
        Color? iconColor = null,
        TextStyle? titleTextStyle = null,
        TextStyle? contentTextStyle = null,
        EdgeInsetsGeometry? actionsPadding = null,
        Color? barrierColor = null,
        EdgeInsets? insetPadding = null,
        Clip? clipBehavior = null,
        BoxConstraints? constraints = null
    )
    {
        return new DialogThemeData(
            backgroundColor: backgroundColor ?? this.backgroundColor,
            elevation: elevation ?? this.elevation,
            shadowColor: shadowColor ?? this.shadowColor,
            surfaceTintColor: surfaceTintColor ?? this.surfaceTintColor,
            shape: shape ?? this.shape,
            alignment: alignment ?? this.alignment,
            iconColor: iconColor ?? this.iconColor,
            titleTextStyle: titleTextStyle ?? this.titleTextStyle,
            contentTextStyle: contentTextStyle ?? this.contentTextStyle,
            actionsPadding: actionsPadding ?? this.actionsPadding,
            barrierColor: barrierColor ?? this.barrierColor,
            insetPadding: insetPadding ?? this.insetPadding,
            clipBehavior: clipBehavior ?? this.clipBehavior,
            constraints: constraints ?? this.constraints
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DialogThemeData lerp(DialogThemeData? a, DialogThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new DialogThemeData(
            backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t),
            elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t),
            shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t),
            surfaceTintColor: Dart_uiLibrary.Color.lerp(
                a?.surfaceTintColor,
                b?.surfaceTintColor,
                t
            ),
            shape: ShapeBorder.lerp(a?.shape, b?.shape, t),
            alignment: AlignmentGeometry.lerp(a?.alignment, b?.alignment, t),
            iconColor: Dart_uiLibrary.Color.lerp(a?.iconColor, b?.iconColor, t),
            titleTextStyle: TextStyle.lerp(a?.titleTextStyle, b?.titleTextStyle, t),
            contentTextStyle: TextStyle.lerp(a?.contentTextStyle, b?.contentTextStyle, t),
            actionsPadding: EdgeInsetsGeometry.lerp(a?.actionsPadding, b?.actionsPadding, t),
            barrierColor: Dart_uiLibrary.Color.lerp(a?.barrierColor, b?.barrierColor, t),
            insetPadding: EdgeInsets.lerp(a?.insetPadding, b?.insetPadding, t),
            clipBehavior: (t < 0.5) ? a?.clipBehavior : b?.clipBehavior,
            constraints: BoxConstraints.lerp(a?.constraints, b?.constraints, t)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHashAll(
                new List<object?>
                {
                    backgroundColor,
                    elevation,
                    shadowColor,
                    surfaceTintColor,
                    shape,
                    alignment,
                    iconColor,
                    titleTextStyle,
                    contentTextStyle,
                    actionsPadding,
                    barrierColor,
                    insetPadding,
                    clipBehavior,
                    constraints,
                }
            )
        );

    public override bool Equals(object? other)
    {
        var __other = other as DialogThemeData;
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
        return (__other is DialogThemeData)
            && Equals(__other.backgroundColor, backgroundColor)
            && (__other.elevation == elevation)
            && Equals(__other.shadowColor, shadowColor)
            && Equals(__other.surfaceTintColor, surfaceTintColor)
            && Equals(__other.shape, shape)
            && Equals(__other.alignment, alignment)
            && Equals(__other.iconColor, iconColor)
            && Equals(__other.titleTextStyle, titleTextStyle)
            && Equals(__other.contentTextStyle, contentTextStyle)
            && Equals(__other.actionsPadding, actionsPadding)
            && Equals(__other.barrierColor, barrierColor)
            && Equals(__other.insetPadding, insetPadding)
            && Equals(__other.clipBehavior, clipBehavior)
            && Equals(__other.constraints, constraints);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new ColorProperty("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new DoubleProperty("elevation", elevation, defaultValue: null));
        properties.add(new ColorProperty("shadowColor", shadowColor, defaultValue: null));
        properties.add(new ColorProperty("surfaceTintColor", surfaceTintColor, defaultValue: null));
        properties.add(new DiagnosticsProperty<ShapeBorder>("shape", shape, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<AlignmentGeometry>("alignment", alignment, defaultValue: null)
        );
        properties.add(new ColorProperty("iconColor", iconColor, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<TextStyle>("titleTextStyle", titleTextStyle, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "contentTextStyle",
                contentTextStyle,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<EdgeInsetsGeometry>(
                "actionsPadding",
                actionsPadding,
                defaultValue: null
            )
        );
        properties.add(new ColorProperty("barrierColor", barrierColor, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<EdgeInsets>("insetPadding", insetPadding, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<Clip>("clipBehavior", clipBehavior, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<BoxConstraints>("constraints", constraints, defaultValue: null)
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
