// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/tooltip_theme.dart
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

public class TooltipThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual double? height { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin { get; private set; }
    public virtual double? verticalOffset { get; private set; }
    public virtual bool? preferBelow { get; private set; }
    public virtual bool? excludeFromSemantics { get; private set; }
    public virtual global::Doroti.Framework.Painting.Decoration? decoration { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? textStyle { get; private set; }
    public virtual TextAlign? textAlign { get; private set; }
    public virtual Duration? waitDuration { get; private set; }
    public virtual Duration? showDuration { get; private set; }
    public virtual Duration? exitDuration { get; private set; }
    public virtual global::Doroti.Framework.Widgets.TooltipTriggerMode? triggerMode { get; private set; }
    public virtual bool? enableFeedback { get; private set; }

    public TooltipThemeData(double? height = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin = null, double? verticalOffset = null, bool? preferBelow = null, bool? excludeFromSemantics = null, global::Doroti.Framework.Painting.Decoration? decoration = null, global::Doroti.Framework.Painting.TextStyle? textStyle = null, TextAlign? textAlign = null, Duration? waitDuration = null, Duration? showDuration = null, Duration? exitDuration = null, global::Doroti.Framework.Widgets.TooltipTriggerMode? triggerMode = null, bool? enableFeedback = null)
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
        System.Diagnostics.Debug.Assert(((height is null) || (constraints is null)));
    }

    public virtual TooltipThemeData copyWith(double? height = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin = null, double? verticalOffset = null, bool? preferBelow = null, bool? excludeFromSemantics = null, global::Doroti.Framework.Painting.Decoration? decoration = null, global::Doroti.Framework.Painting.TextStyle? textStyle = null, TextAlign? textAlign = null, Duration? waitDuration = null, Duration? showDuration = null, Duration? exitDuration = null, global::Doroti.Framework.Widgets.TooltipTriggerMode? triggerMode = null, bool? enableFeedback = null)
    {
        return new TooltipThemeData(height: (height ?? this.height), constraints: (constraints ?? this.constraints), padding: (padding ?? this.padding), margin: (margin ?? this.margin), verticalOffset: (verticalOffset ?? this.verticalOffset), preferBelow: (preferBelow ?? this.preferBelow), excludeFromSemantics: (excludeFromSemantics ?? this.excludeFromSemantics), decoration: (decoration ?? this.decoration), textStyle: (textStyle ?? this.textStyle), textAlign: (textAlign ?? this.textAlign), waitDuration: (waitDuration ?? this.waitDuration), showDuration: (showDuration ?? this.showDuration), triggerMode: (triggerMode ?? this.triggerMode), enableFeedback: (enableFeedback ?? this.enableFeedback));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TooltipThemeData? lerp(TooltipThemeData? a, TooltipThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new TooltipThemeData(height: Dart_uiLibrary.lerpDouble(a?.height, b?.height, t), constraints: BoxConstraints.lerp(a?.constraints, b?.constraints, t), padding: EdgeInsetsGeometry.lerp(a?.padding, b?.padding, t), margin: EdgeInsetsGeometry.lerp(a?.margin, b?.margin, t), verticalOffset: Dart_uiLibrary.lerpDouble(a?.verticalOffset, b?.verticalOffset, t), preferBelow: ((t < 0.5) ? a?.preferBelow : b?.preferBelow), excludeFromSemantics: ((t < 0.5) ? a?.excludeFromSemantics : b?.excludeFromSemantics), decoration: Decoration.lerp(a?.decoration, b?.decoration, t), textStyle: TextStyle.lerp(a?.textStyle, b?.textStyle, t), textAlign: ((t < 0.5) ? a?.textAlign : b?.textAlign));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.height, this.constraints, this.padding, this.margin, this.verticalOffset, this.preferBelow, this.excludeFromSemantics, this.decoration, this.textStyle, this.textAlign, this.waitDuration, this.showDuration, this.exitDuration, this.triggerMode, this.enableFeedback));
    public override bool Equals(object? other)
    {
        var __other = other as TooltipThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((((((((__other is TooltipThemeData) && (((TooltipThemeData)((TooltipThemeData)__other)).height == this.height)) && (object.Equals(((TooltipThemeData)((TooltipThemeData)__other)).constraints, this.constraints))) && (object.Equals(((TooltipThemeData)((TooltipThemeData)__other)).padding, this.padding))) && (object.Equals(((TooltipThemeData)((TooltipThemeData)__other)).margin, this.margin))) && (((TooltipThemeData)((TooltipThemeData)__other)).verticalOffset == this.verticalOffset)) && (((TooltipThemeData)((TooltipThemeData)__other)).preferBelow == this.preferBelow)) && (((TooltipThemeData)((TooltipThemeData)__other)).excludeFromSemantics == this.excludeFromSemantics)) && (object.Equals(((TooltipThemeData)((TooltipThemeData)__other)).decoration, this.decoration))) && (object.Equals(((TooltipThemeData)((TooltipThemeData)__other)).textStyle, this.textStyle))) && (object.Equals(((TooltipThemeData)((TooltipThemeData)__other)).textAlign, this.textAlign))) && (object.Equals(((TooltipThemeData)((TooltipThemeData)__other)).waitDuration, this.waitDuration))) && (object.Equals(((TooltipThemeData)((TooltipThemeData)__other)).showDuration, this.showDuration))) && (object.Equals(((TooltipThemeData)((TooltipThemeData)__other)).exitDuration, this.exitDuration))) && (object.Equals(((TooltipThemeData)((TooltipThemeData)__other)).triggerMode, this.triggerMode))) && (((TooltipThemeData)((TooltipThemeData)__other)).enableFeedback == this.enableFeedback));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("height", this.height, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("constraints", this.constraints, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("margin", this.margin, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("vertical offset", this.verticalOffset, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("position", value: this.preferBelow, ifTrue: "below", ifFalse: "above", showName: true));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("semantics", value: this.excludeFromSemantics, ifTrue: "excluded", showName: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.Decoration>("decoration", this.decoration, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("textStyle", this.textStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.TextAlign>("textAlign", this.textAlign, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Duration>("wait duration", this.waitDuration, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Duration>("show duration", this.showDuration, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Duration>("exit duration", this.exitDuration, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.TooltipTriggerMode>("triggerMode", this.triggerMode, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("enableFeedback", value: this.enableFeedback, ifTrue: "true", showName: true));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return ((fullString ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class TooltipTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual TooltipThemeData data { get; private set; } = default!;

    public TooltipTheme(global::Doroti.Framework.Foundation.Key? key = null, TooltipThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static TooltipThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        TooltipTheme? tooltipThemeLocal = ((TooltipTheme?)(object?)context.dependOnInheritedWidgetOfExactType<TooltipTheme>());
        return (tooltipThemeLocal?.data ?? Theme.of(context).tooltipTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new TooltipTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((TooltipTheme)oldWidget).data)));
}
