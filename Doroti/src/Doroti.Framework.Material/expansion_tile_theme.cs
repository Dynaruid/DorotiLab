// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/expansion_tile_theme.dart
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

public class ExpansionTileThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? collapsedBackgroundColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? tilePadding { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry? expandedAlignment { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? childrenPadding { get; private set; }
    public virtual Color? iconColor { get; private set; }
    public virtual Color? collapsedIconColor { get; private set; }
    public virtual Color? textColor { get; private set; }
    public virtual Color? collapsedTextColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? collapsedShape { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual global::Doroti.Generated.Framework.Animation.AnimationStyle? expansionAnimationStyle { get; private set; }

    public ExpansionTileThemeData(Color? backgroundColor = null, Color? collapsedBackgroundColor = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? tilePadding = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? expandedAlignment = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? childrenPadding = null, Color? iconColor = null, Color? collapsedIconColor = null, Color? textColor = null, Color? collapsedTextColor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? collapsedShape = null, Clip? clipBehavior = null, global::Doroti.Generated.Framework.Animation.AnimationStyle? expansionAnimationStyle = null)
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

    public virtual ExpansionTileThemeData copyWith(Color? backgroundColor = null, Color? collapsedBackgroundColor = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? tilePadding = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? expandedAlignment = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? childrenPadding = null, Color? iconColor = null, Color? collapsedIconColor = null, Color? textColor = null, Color? collapsedTextColor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? collapsedShape = null, Clip? clipBehavior = null, global::Doroti.Generated.Framework.Animation.AnimationStyle? expansionAnimationStyle = null)
    {
        return new ExpansionTileThemeData(backgroundColor: (backgroundColor ?? this.backgroundColor), collapsedBackgroundColor: (collapsedBackgroundColor ?? this.collapsedBackgroundColor), tilePadding: (tilePadding ?? this.tilePadding), expandedAlignment: (expandedAlignment ?? this.expandedAlignment), childrenPadding: (childrenPadding ?? this.childrenPadding), iconColor: (iconColor ?? this.iconColor), collapsedIconColor: (collapsedIconColor ?? this.collapsedIconColor), textColor: (textColor ?? this.textColor), collapsedTextColor: (collapsedTextColor ?? this.collapsedTextColor), shape: (shape ?? this.shape), collapsedShape: (collapsedShape ?? this.collapsedShape), clipBehavior: (clipBehavior ?? this.clipBehavior), expansionAnimationStyle: (expansionAnimationStyle ?? this.expansionAnimationStyle));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ExpansionTileThemeData? lerp(ExpansionTileThemeData? a, ExpansionTileThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new ExpansionTileThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), collapsedBackgroundColor: Dart_uiLibrary.Color.lerp(a?.collapsedBackgroundColor, b?.collapsedBackgroundColor, t), tilePadding: EdgeInsetsGeometry.lerp(a?.tilePadding, b?.tilePadding, t), expandedAlignment: AlignmentGeometry.lerp(a?.expandedAlignment, b?.expandedAlignment, t), childrenPadding: EdgeInsetsGeometry.lerp(a?.childrenPadding, b?.childrenPadding, t), iconColor: Dart_uiLibrary.Color.lerp(a?.iconColor, b?.iconColor, t), collapsedIconColor: Dart_uiLibrary.Color.lerp(a?.collapsedIconColor, b?.collapsedIconColor, t), textColor: Dart_uiLibrary.Color.lerp(a?.textColor, b?.textColor, t), collapsedTextColor: Dart_uiLibrary.Color.lerp(a?.collapsedTextColor, b?.collapsedTextColor, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t), collapsedShape: ShapeBorder.lerp(a?.collapsedShape, b?.collapsedShape, t), clipBehavior: ((t < 0.5) ? a?.clipBehavior : b?.clipBehavior), expansionAnimationStyle: ((t < 0.5) ? a?.expansionAnimationStyle : b?.expansionAnimationStyle));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode()
    {
        return FoundationRuntimePorts.ObjectHash(this.backgroundColor, this.collapsedBackgroundColor, this.tilePadding, this.expandedAlignment, this.childrenPadding, this.iconColor, this.collapsedIconColor, this.textColor, this.collapsedTextColor, this.shape, this.collapsedShape, this.clipBehavior, this.expansionAnimationStyle);
        return default!;
    }
    public override bool Equals(object? other)
    {
        var __other = other as ExpansionTileThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((((((__other is ExpansionTileThemeData) && (object.Equals(((ExpansionTileThemeData)((ExpansionTileThemeData)__other)).backgroundColor, this.backgroundColor))) && (object.Equals(((ExpansionTileThemeData)((ExpansionTileThemeData)__other)).collapsedBackgroundColor, this.collapsedBackgroundColor))) && (object.Equals(((ExpansionTileThemeData)((ExpansionTileThemeData)__other)).tilePadding, this.tilePadding))) && (object.Equals(((ExpansionTileThemeData)((ExpansionTileThemeData)__other)).expandedAlignment, this.expandedAlignment))) && (object.Equals(((ExpansionTileThemeData)((ExpansionTileThemeData)__other)).childrenPadding, this.childrenPadding))) && (object.Equals(((ExpansionTileThemeData)((ExpansionTileThemeData)__other)).iconColor, this.iconColor))) && (object.Equals(((ExpansionTileThemeData)((ExpansionTileThemeData)__other)).collapsedIconColor, this.collapsedIconColor))) && (object.Equals(((ExpansionTileThemeData)((ExpansionTileThemeData)__other)).textColor, this.textColor))) && (object.Equals(((ExpansionTileThemeData)((ExpansionTileThemeData)__other)).collapsedTextColor, this.collapsedTextColor))) && (object.Equals(((ExpansionTileThemeData)((ExpansionTileThemeData)__other)).shape, this.shape))) && (object.Equals(((ExpansionTileThemeData)((ExpansionTileThemeData)__other)).collapsedShape, this.collapsedShape))) && (object.Equals(((ExpansionTileThemeData)((ExpansionTileThemeData)__other)).clipBehavior, this.clipBehavior))) && (object.Equals(((ExpansionTileThemeData)((ExpansionTileThemeData)__other)).expansionAnimationStyle, this.expansionAnimationStyle)));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("backgroundColor", this.backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("collapsedBackgroundColor", this.collapsedBackgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("tilePadding", this.tilePadding, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>("expandedAlignment", this.expandedAlignment, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("childrenPadding", this.childrenPadding, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("iconColor", this.iconColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("collapsedIconColor", this.collapsedIconColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("textColor", this.textColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("collapsedTextColor", this.collapsedTextColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ShapeBorder>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ShapeBorder>("collapsedShape", this.collapsedShape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Animation.AnimationStyle>("expansionAnimationStyle", this.expansionAnimationStyle, defaultValue: null));
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

public class ExpansionTileTheme : global::Doroti.Generated.Framework.Widgets.InheritedTheme
{
    public virtual ExpansionTileThemeData data { get; private set; } = default!;

    public ExpansionTileTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, ExpansionTileThemeData data = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static ExpansionTileThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ExpansionTileTheme? inheritedTheme__10178 = ((ExpansionTileTheme?)(object?)context.dependOnInheritedWidgetOfExactType<ExpansionTileTheme>());
        return (inheritedTheme__10178?.data ?? Theme.of(context).expansionTileTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget wrap(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new ExpansionTileTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((ExpansionTileTheme)oldWidget).data)));
}
