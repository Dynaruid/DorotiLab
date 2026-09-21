// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/tooltip_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class TooltipThemeData : Diagnosticable
{
    public virtual double? height { get; private set; }
    public virtual BoxConstraints? constraints { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual EdgeInsetsGeometry? margin { get; private set; }
    public virtual double? verticalOffset { get; private set; }
    public virtual bool? preferBelow { get; private set; }
    public virtual bool? excludeFromSemantics { get; private set; }
    public virtual Decoration? decoration { get; private set; }
    public virtual TextStyle? textStyle { get; private set; }
    public virtual TextAlign? textAlign { get; private set; }
    public virtual Duration? waitDuration { get; private set; }
    public virtual Duration? showDuration { get; private set; }
    public virtual Duration? exitDuration { get; private set; }
    public virtual TooltipTriggerMode? triggerMode { get; private set; }
    public virtual bool? enableFeedback { get; private set; }

    public TooltipThemeData(
        double? height = null,
        BoxConstraints? constraints = null,
        EdgeInsetsGeometry? padding = null,
        EdgeInsetsGeometry? margin = null,
        double? verticalOffset = null,
        bool? preferBelow = null,
        bool? excludeFromSemantics = null,
        Decoration? decoration = null,
        TextStyle? textStyle = null,
        TextAlign? textAlign = null,
        Duration? waitDuration = null,
        Duration? showDuration = null,
        Duration? exitDuration = null,
        TooltipTriggerMode? triggerMode = null,
        bool? enableFeedback = null
    )
    {
        this.height = height;
        this.constraints = constraints;
        this.padding = padding;
        this.margin = margin;
        this.verticalOffset = verticalOffset;
        this.preferBelow = preferBelow;
        this.excludeFromSemantics = excludeFromSemantics;
        this.decoration = decoration;
        this.textStyle = textStyle;
        this.textAlign = textAlign;
        this.waitDuration = waitDuration;
        this.showDuration = showDuration;
        this.exitDuration = exitDuration;
        this.triggerMode = triggerMode;
        this.enableFeedback = enableFeedback;
        System.Diagnostics.Debug.Assert((height is null) || (constraints is null));
    }

    public virtual TooltipThemeData copyWith(
        double? height = null,
        BoxConstraints? constraints = null,
        EdgeInsetsGeometry? padding = null,
        EdgeInsetsGeometry? margin = null,
        double? verticalOffset = null,
        bool? preferBelow = null,
        bool? excludeFromSemantics = null,
        Decoration? decoration = null,
        TextStyle? textStyle = null,
        TextAlign? textAlign = null,
        Duration? waitDuration = null,
        Duration? showDuration = null,
        Duration? exitDuration = null,
        TooltipTriggerMode? triggerMode = null,
        bool? enableFeedback = null
    )
    {
        return new TooltipThemeData(
            height: height ?? this.height,
            constraints: constraints ?? this.constraints,
            padding: padding ?? this.padding,
            margin: margin ?? this.margin,
            verticalOffset: verticalOffset ?? this.verticalOffset,
            preferBelow: preferBelow ?? this.preferBelow,
            excludeFromSemantics: excludeFromSemantics ?? this.excludeFromSemantics,
            decoration: decoration ?? this.decoration,
            textStyle: textStyle ?? this.textStyle,
            textAlign: textAlign ?? this.textAlign,
            waitDuration: waitDuration ?? this.waitDuration,
            showDuration: showDuration ?? this.showDuration,
            triggerMode: triggerMode ?? this.triggerMode,
            enableFeedback: enableFeedback ?? this.enableFeedback
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TooltipThemeData? lerp(TooltipThemeData? a, TooltipThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new TooltipThemeData(
            height: Dart_uiLibrary.lerpDouble(a?.height, b?.height, t),
            constraints: BoxConstraints.lerp(a?.constraints, b?.constraints, t),
            padding: EdgeInsetsGeometry.lerp(a?.padding, b?.padding, t),
            margin: EdgeInsetsGeometry.lerp(a?.margin, b?.margin, t),
            verticalOffset: Dart_uiLibrary.lerpDouble(a?.verticalOffset, b?.verticalOffset, t),
            preferBelow: (t < 0.5) ? a?.preferBelow : b?.preferBelow,
            excludeFromSemantics: (t < 0.5) ? a?.excludeFromSemantics : b?.excludeFromSemantics,
            decoration: Decoration.lerp(a?.decoration, b?.decoration, t),
            textStyle: TextStyle.lerp(a?.textStyle, b?.textStyle, t),
            textAlign: (t < 0.5) ? a?.textAlign : b?.textAlign
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                height,
                constraints,
                padding,
                margin,
                verticalOffset,
                preferBelow,
                excludeFromSemantics,
                decoration,
                textStyle,
                textAlign,
                waitDuration,
                showDuration,
                exitDuration,
                triggerMode,
                enableFeedback
            )
        );

    public override bool Equals(object? other)
    {
        var __other = other as TooltipThemeData;
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
        return (__other is TooltipThemeData)
            && (__other.height == height)
            && Equals(__other.constraints, constraints)
            && Equals(__other.padding, padding)
            && Equals(__other.margin, margin)
            && (__other.verticalOffset == verticalOffset)
            && (__other.preferBelow == preferBelow)
            && (__other.excludeFromSemantics == excludeFromSemantics)
            && Equals(__other.decoration, decoration)
            && Equals(__other.textStyle, textStyle)
            && Equals(__other.textAlign, textAlign)
            && Equals(__other.waitDuration, waitDuration)
            && Equals(__other.showDuration, showDuration)
            && Equals(__other.exitDuration, exitDuration)
            && Equals(__other.triggerMode, triggerMode)
            && (__other.enableFeedback == enableFeedback);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new DoubleProperty("height", height, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<BoxConstraints>("constraints", constraints, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<EdgeInsetsGeometry>("padding", padding, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<EdgeInsetsGeometry>("margin", margin, defaultValue: null)
        );
        properties.add(new DoubleProperty("vertical offset", verticalOffset, defaultValue: null));
        properties.add(
            new FlagProperty(
                "position",
                value: preferBelow,
                ifTrue: "below",
                ifFalse: "above",
                showName: true
            )
        );
        properties.add(
            new FlagProperty(
                "semantics",
                value: excludeFromSemantics,
                ifTrue: "excluded",
                showName: true
            )
        );
        properties.add(
            new DiagnosticsProperty<Decoration>("decoration", decoration, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>("textStyle", textStyle, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<TextAlign>("textAlign", textAlign, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<Duration>("wait duration", waitDuration, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<Duration>("show duration", showDuration, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<Duration>("exit duration", exitDuration, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<TooltipTriggerMode>(
                "triggerMode",
                triggerMode,
                defaultValue: null
            )
        );
        properties.add(
            new FlagProperty(
                "enableFeedback",
                value: enableFeedback,
                ifTrue: "true",
                showName: true
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

public class TooltipTheme : InheritedTheme
{
    public virtual TooltipThemeData data { get; private set; } = default!;

    public TooltipTheme(Key? key = null, TooltipThemeData data = default!, Widget child = default!)
        : base(key: key, child: child)
    {
        this.data = data;
    }

    public static TooltipThemeData of(BuildContext context)
    {
        TooltipTheme? tooltipThemeLocal =
            context.dependOnInheritedWidgetOfExactType<TooltipTheme>();
        return tooltipThemeLocal?.data ?? Theme.of(context).tooltipTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new TooltipTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((TooltipTheme)oldWidget).data));
}
