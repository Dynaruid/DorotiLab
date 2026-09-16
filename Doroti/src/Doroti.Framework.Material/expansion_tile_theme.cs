// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/expansion_tile_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class ExpansionTileThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? collapsedBackgroundColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? tilePadding { get; private set; }
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry? expandedAlignment { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? childrenPadding { get; private set; }
    public virtual Color? iconColor { get; private set; }
    public virtual Color? collapsedIconColor { get; private set; }
    public virtual Color? textColor { get; private set; }
    public virtual Color? collapsedTextColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? collapsedShape { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual global::Doroti.Framework.Animation.AnimationStyle? expansionAnimationStyle { get; private set; }

    public ExpansionTileThemeData(Color? backgroundColor = null, Color? collapsedBackgroundColor = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? tilePadding = null, global::Doroti.Framework.Painting.AlignmentGeometry? expandedAlignment = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? childrenPadding = null, Color? iconColor = null, Color? collapsedIconColor = null, Color? textColor = null, Color? collapsedTextColor = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Framework.Painting.ShapeBorder? collapsedShape = null, Clip? clipBehavior = null, global::Doroti.Framework.Animation.AnimationStyle? expansionAnimationStyle = null)
    {
        this.backgroundColor = backgroundColor;
        this.collapsedBackgroundColor = collapsedBackgroundColor;
        this.tilePadding = tilePadding;
        this.expandedAlignment = expandedAlignment;
        this.childrenPadding = childrenPadding;
        this.iconColor = iconColor;
        this.collapsedIconColor = collapsedIconColor;
        this.textColor = textColor;
        this.collapsedTextColor = collapsedTextColor;
        this.shape = shape;
        this.collapsedShape = collapsedShape;
        this.clipBehavior = clipBehavior;
        this.expansionAnimationStyle = expansionAnimationStyle;
    }

    public virtual ExpansionTileThemeData copyWith(Color? backgroundColor = null, Color? collapsedBackgroundColor = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? tilePadding = null, global::Doroti.Framework.Painting.AlignmentGeometry? expandedAlignment = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? childrenPadding = null, Color? iconColor = null, Color? collapsedIconColor = null, Color? textColor = null, Color? collapsedTextColor = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Framework.Painting.ShapeBorder? collapsedShape = null, Clip? clipBehavior = null, global::Doroti.Framework.Animation.AnimationStyle? expansionAnimationStyle = null)
    {
        return new ExpansionTileThemeData(backgroundColor: backgroundColor ?? this.backgroundColor, collapsedBackgroundColor: collapsedBackgroundColor ?? this.collapsedBackgroundColor, tilePadding: tilePadding ?? this.tilePadding, expandedAlignment: expandedAlignment ?? this.expandedAlignment, childrenPadding: childrenPadding ?? this.childrenPadding, iconColor: iconColor ?? this.iconColor, collapsedIconColor: collapsedIconColor ?? this.collapsedIconColor, textColor: textColor ?? this.textColor, collapsedTextColor: collapsedTextColor ?? this.collapsedTextColor, shape: shape ?? this.shape, collapsedShape: collapsedShape ?? this.collapsedShape, clipBehavior: clipBehavior ?? this.clipBehavior, expansionAnimationStyle: expansionAnimationStyle ?? this.expansionAnimationStyle);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ExpansionTileThemeData? lerp(ExpansionTileThemeData? a, ExpansionTileThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new ExpansionTileThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), collapsedBackgroundColor: Dart_uiLibrary.Color.lerp(a?.collapsedBackgroundColor, b?.collapsedBackgroundColor, t), tilePadding: EdgeInsetsGeometry.lerp(a?.tilePadding, b?.tilePadding, t), expandedAlignment: AlignmentGeometry.lerp(a?.expandedAlignment, b?.expandedAlignment, t), childrenPadding: EdgeInsetsGeometry.lerp(a?.childrenPadding, b?.childrenPadding, t), iconColor: Dart_uiLibrary.Color.lerp(a?.iconColor, b?.iconColor, t), collapsedIconColor: Dart_uiLibrary.Color.lerp(a?.collapsedIconColor, b?.collapsedIconColor, t), textColor: Dart_uiLibrary.Color.lerp(a?.textColor, b?.textColor, t), collapsedTextColor: Dart_uiLibrary.Color.lerp(a?.collapsedTextColor, b?.collapsedTextColor, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t), collapsedShape: ShapeBorder.lerp(a?.collapsedShape, b?.collapsedShape, t), clipBehavior: (t < 0.5) ? a?.clipBehavior : b?.clipBehavior, expansionAnimationStyle: (t < 0.5) ? a?.expansionAnimationStyle : b?.expansionAnimationStyle);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode()
    {
        return FoundationRuntimePorts.ObjectHash(backgroundColor, collapsedBackgroundColor, tilePadding, expandedAlignment, childrenPadding, iconColor, collapsedIconColor, textColor, collapsedTextColor, shape, collapsedShape, clipBehavior, expansionAnimationStyle);
    }
    public override bool Equals(object? other)
    {
        var __other = other as ExpansionTileThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ExpansionTileThemeData) && Equals(__other.backgroundColor, backgroundColor) && Equals(__other.collapsedBackgroundColor, collapsedBackgroundColor) && Equals(__other.tilePadding, tilePadding) && Equals(__other.expandedAlignment, expandedAlignment) && Equals(__other.childrenPadding, childrenPadding) && Equals(__other.iconColor, iconColor) && Equals(__other.collapsedIconColor, collapsedIconColor) && Equals(__other.textColor, textColor) && Equals(__other.collapsedTextColor, collapsedTextColor) && Equals(__other.shape, shape) && Equals(__other.collapsedShape, collapsedShape) && Equals(__other.clipBehavior, clipBehavior) && Equals(__other.expansionAnimationStyle, expansionAnimationStyle);
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("collapsedBackgroundColor", collapsedBackgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("tilePadding", tilePadding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.AlignmentGeometry>("expandedAlignment", expandedAlignment, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("childrenPadding", childrenPadding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("iconColor", iconColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("collapsedIconColor", collapsedIconColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("textColor", textColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("collapsedTextColor", collapsedTextColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("shape", shape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("collapsedShape", collapsedShape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Clip>("clipBehavior", clipBehavior, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Animation.AnimationStyle>("expansionAnimationStyle", expansionAnimationStyle, defaultValue: null));
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

public class ExpansionTileTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual ExpansionTileThemeData data { get; private set; } = default!;

    public ExpansionTileTheme(global::Doroti.Framework.Foundation.Key? key = null, ExpansionTileThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static ExpansionTileThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ExpansionTileTheme? inheritedTheme = context.dependOnInheritedWidgetOfExactType<ExpansionTileTheme>();
        return inheritedTheme?.data ?? Theme.of(context).expansionTileTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return new ExpansionTileTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((ExpansionTileTheme)oldWidget).data));
}
