// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/badge_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class BadgeThemeData : Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? textColor { get; private set; }
    public virtual double? smallSize { get; private set; }
    public virtual double? largeSize { get; private set; }
    public virtual TextStyle? textStyle { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual AlignmentGeometry? alignment { get; private set; }
    public virtual Offset? offset { get; private set; }

    public BadgeThemeData(Color? backgroundColor = null, Color? textColor = null, double? smallSize = null, double? largeSize = null, TextStyle? textStyle = null, EdgeInsetsGeometry? padding = null, AlignmentGeometry? alignment = null, Offset? offset = null)
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

    public virtual BadgeThemeData copyWith(Color? backgroundColor = null, Color? textColor = null, double? smallSize = null, double? largeSize = null, TextStyle? textStyle = null, EdgeInsetsGeometry? padding = null, AlignmentGeometry? alignment = null, Offset? offset = null)
    {
        return new BadgeThemeData(backgroundColor: backgroundColor ?? this.backgroundColor, textColor: textColor ?? this.textColor, smallSize: smallSize ?? this.smallSize, largeSize: largeSize ?? this.largeSize, textStyle: textStyle ?? this.textStyle, padding: padding ?? this.padding, alignment: alignment ?? this.alignment, offset: offset ?? this.offset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BadgeThemeData lerp(BadgeThemeData? a, BadgeThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new BadgeThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), textColor: Dart_uiLibrary.Color.lerp(a?.textColor, b?.textColor, t), smallSize: Dart_uiLibrary.lerpDouble(a?.smallSize, b?.smallSize, t), largeSize: Dart_uiLibrary.lerpDouble(a?.largeSize, b?.largeSize, t), textStyle: TextStyle.lerp(a?.textStyle, b?.textStyle, t), padding: EdgeInsetsGeometry.lerp(a?.padding, b?.padding, t), alignment: AlignmentGeometry.lerp(a?.alignment, b?.alignment, t), offset: Dart_uiLibrary.Offset.lerp(a?.offset, b?.offset, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(backgroundColor, textColor, smallSize, largeSize, textStyle, padding, alignment, offset));
    public override bool Equals(object? other)
    {
        var __other = other as BadgeThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is BadgeThemeData) && Equals(__other.backgroundColor, backgroundColor) && Equals(__other.textColor, textColor) && (__other.smallSize == smallSize) && (__other.largeSize == largeSize) && Equals(__other.textStyle, textStyle) && Equals(__other.padding, padding) && Equals(__other.alignment, alignment) && Equals(__other.offset, offset);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new ColorProperty("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new ColorProperty("textColor", textColor, defaultValue: null));
        properties.add(new DoubleProperty("smallSize", smallSize, defaultValue: null));
        properties.add(new DoubleProperty("largeSize", largeSize, defaultValue: null));
        properties.add(new DiagnosticsProperty<TextStyle>("textStyle", textStyle, defaultValue: null));
        properties.add(new DiagnosticsProperty<EdgeInsetsGeometry>("padding", padding, defaultValue: null));
        properties.add(new DiagnosticsProperty<AlignmentGeometry>("alignment", alignment, defaultValue: null));
        properties.add(new DiagnosticsProperty<Offset>("offset", offset, defaultValue: null));
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

public class BadgeTheme : InheritedTheme
{
    public virtual BadgeThemeData data { get; private set; } = default!;

    public BadgeTheme(Key? key = null, BadgeThemeData data = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static BadgeThemeData of(BuildContext context)
    {
        BadgeTheme? badgeThemeLocal = context.dependOnInheritedWidgetOfExactType<BadgeTheme>();
        return badgeThemeLocal?.data ?? Theme.of(context).badgeTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new BadgeTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((BadgeTheme)oldWidget).data));
}
