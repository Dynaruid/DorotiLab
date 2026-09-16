// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/badge_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class BadgeThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? textColor { get; private set; }
    public virtual double? smallSize { get; private set; }
    public virtual double? largeSize { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? textStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry? alignment { get; private set; }
    public virtual Offset? offset { get; private set; }

    public BadgeThemeData(Color? backgroundColor = null, Color? textColor = null, double? smallSize = null, double? largeSize = null, global::Doroti.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, Offset? offset = null)
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

    public virtual BadgeThemeData copyWith(Color? backgroundColor = null, Color? textColor = null, double? smallSize = null, double? largeSize = null, global::Doroti.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, Offset? offset = null)
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
        if ((!Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((__other is BadgeThemeData) && (Equals(((BadgeThemeData)((BadgeThemeData)__other)).backgroundColor, this.backgroundColor))) && (Equals(((BadgeThemeData)((BadgeThemeData)__other)).textColor, this.textColor))) && (((BadgeThemeData)((BadgeThemeData)__other)).smallSize == this.smallSize)) && (((BadgeThemeData)((BadgeThemeData)__other)).largeSize == this.largeSize)) && (Equals(((BadgeThemeData)((BadgeThemeData)__other)).textStyle, this.textStyle))) && (Equals(((BadgeThemeData)((BadgeThemeData)__other)).padding, this.padding))) && (Equals(((BadgeThemeData)((BadgeThemeData)__other)).alignment, this.alignment))) && (Equals(((BadgeThemeData)((BadgeThemeData)__other)).offset, this.offset)));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("backgroundColor", this.backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("textColor", this.textColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("smallSize", this.smallSize, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("largeSize", this.largeSize, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("textStyle", this.textStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.AlignmentGeometry>("alignment", this.alignment, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Offset>("offset", this.offset, defaultValue: null));
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
        return ((fullString ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class BadgeTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual BadgeThemeData data { get; private set; } = default!;

    public BadgeTheme(global::Doroti.Framework.Foundation.Key? key = null, BadgeThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static BadgeThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        BadgeTheme? badgeThemeLocal = ((BadgeTheme?)context.dependOnInheritedWidgetOfExactType<BadgeTheme>());
        return (badgeThemeLocal?.data ?? Theme.of(context).badgeTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)new BadgeTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!Equals(this.data, ((BadgeTheme)oldWidget).data)));
}
