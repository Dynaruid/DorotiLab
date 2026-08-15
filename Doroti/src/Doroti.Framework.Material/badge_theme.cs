// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/badge_theme.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class BadgeThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? textColor { get; private set; }
    public virtual double? smallSize { get; private set; }
    public virtual double? largeSize { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? textStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment { get; private set; }
    public virtual Offset? offset { get; private set; }

    public BadgeThemeData(Color? backgroundColor = null, Color? textColor = null, double? smallSize = null, double? largeSize = null, global::Doroti.Generated.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null, Offset? offset = null)
    {
        this.backgroundColor = backgroundColor;
        this.textColor = textColor;
        this.smallSize = smallSize;
        this.largeSize = largeSize;
        this.textStyle = textStyle;
        this.padding = padding;
        this.alignment = alignment;
        this.offset = offset;
    }

    public virtual BadgeThemeData copyWith(Color? backgroundColor = null, Color? textColor = null, double? smallSize = null, double? largeSize = null, global::Doroti.Generated.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null, Offset? offset = null)
    {
        return new BadgeThemeData(backgroundColor: (backgroundColor ?? this.backgroundColor), textColor: (textColor ?? this.textColor), smallSize: (smallSize ?? this.smallSize), largeSize: (largeSize ?? this.largeSize), textStyle: (textStyle ?? this.textStyle), padding: (padding ?? this.padding), alignment: (alignment ?? this.alignment), offset: (offset ?? this.offset));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BadgeThemeData lerp(BadgeThemeData? a, BadgeThemeData? b, double t)
    {
        if ((DartRuntimePrimitives.Identical(a, b) && (a is not null)))
        {
            return a;
        }
        return new BadgeThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), textColor: Dart_uiLibrary.Color.lerp(a?.textColor, b?.textColor, t), smallSize: Dart_uiLibrary.lerpDouble(a?.smallSize, b?.smallSize, t), largeSize: Dart_uiLibrary.lerpDouble(a?.largeSize, b?.largeSize, t), textStyle: TextStyle.lerp(a?.textStyle, b?.textStyle, t), padding: EdgeInsetsGeometry.lerp(a?.padding, b?.padding, t), alignment: AlignmentGeometry.lerp(a?.alignment, b?.alignment, t), offset: Dart_uiLibrary.Offset.lerp(a?.offset, b?.offset, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.backgroundColor, this.textColor, this.smallSize, this.largeSize, this.textStyle, this.padding, this.alignment, this.offset));
    public override bool Equals(object? other)
    {
        var __other = other as BadgeThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((__other is BadgeThemeData) && (object.Equals(((BadgeThemeData)((BadgeThemeData)__other)).backgroundColor, this.backgroundColor))) && (object.Equals(((BadgeThemeData)((BadgeThemeData)__other)).textColor, this.textColor))) && (((BadgeThemeData)((BadgeThemeData)__other)).smallSize == this.smallSize)) && (((BadgeThemeData)((BadgeThemeData)__other)).largeSize == this.largeSize)) && (object.Equals(((BadgeThemeData)((BadgeThemeData)__other)).textStyle, this.textStyle))) && (object.Equals(((BadgeThemeData)((BadgeThemeData)__other)).padding, this.padding))) && (object.Equals(((BadgeThemeData)((BadgeThemeData)__other)).alignment, this.alignment))) && (object.Equals(((BadgeThemeData)((BadgeThemeData)__other)).offset, this.offset)));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("backgroundColor", this.backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("textColor", this.textColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("smallSize", this.smallSize, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("largeSize", this.largeSize, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("textStyle", this.textStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>("alignment", this.alignment, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Offset>("offset", this.offset, defaultValue: null));
    }

    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
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

public class BadgeTheme : global::Doroti.Generated.Framework.Widgets.InheritedTheme
{
    public virtual BadgeThemeData data { get; private set; } = default!;

    public BadgeTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, BadgeThemeData data = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static BadgeThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        BadgeTheme? badgeTheme__6162 = ((BadgeTheme?)(object?)context.dependOnInheritedWidgetOfExactType<BadgeTheme>());
        return (badgeTheme__6162?.data ?? Theme.of(context).badgeTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget wrap(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new BadgeTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((BadgeTheme)oldWidget).data)));
}
