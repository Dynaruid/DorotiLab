// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/card.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

internal enum _CardVariant__card
{
    elevated,
    filled,
    outlined
}

public class Card : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual Color? color { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual bool borderOnForeground { get; private set; } = default!;
    public virtual Clip? clipBehavior { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin { get; private set; }
    public virtual bool semanticContainer { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }
    internal virtual _CardVariant__card _variant { get; private set; } = default!;

    public Card(global::Doroti.Framework.Foundation.Key? key = null, Color? color = null, Color? shadowColor = null, Color? surfaceTintColor = null, double? elevation = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, bool borderOnForeground = true, global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin = null, Clip? clipBehavior = null, global::Doroti.Framework.Widgets.Widget? child = null, bool semanticContainer = true) : base(key: key)
    {
        this.color = color;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.elevation = elevation;
        this.shape = shape;
        this.borderOnForeground = borderOnForeground;
        this.margin = margin;
        this.clipBehavior = clipBehavior;
        this.child = child;
        this.semanticContainer = semanticContainer;
        _variant = _CardVariant__card.elevated;
        System.Diagnostics.Debug.Assert((elevation is null) || (elevation >= 0.0));
    }

    public static Card CreateFilled(global::Doroti.Framework.Foundation.Key? key = null, Color? color = null, Color? shadowColor = null, Color? surfaceTintColor = null, double? elevation = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, bool borderOnForeground = true, global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin = null, Clip? clipBehavior = null, global::Doroti.Framework.Widgets.Widget? child = null, bool semanticContainer = true)
    {
        var __instance = new Card(key: key, color: color, shadowColor: shadowColor, surfaceTintColor: surfaceTintColor, elevation: elevation, shape: shape, borderOnForeground: borderOnForeground, margin: margin, clipBehavior: clipBehavior, child: child, semanticContainer: semanticContainer);
        __instance.color = color;
        __instance.shadowColor = shadowColor;
        __instance.surfaceTintColor = surfaceTintColor;
        __instance.elevation = elevation;
        __instance.shape = shape;
        __instance.borderOnForeground = borderOnForeground;
        __instance.margin = margin;
        __instance.clipBehavior = clipBehavior;
        __instance.child = child;
        __instance.semanticContainer = semanticContainer;
        __instance._variant = _CardVariant__card.filled;
        return __instance;
    }

    public static Card CreateOutlined(global::Doroti.Framework.Foundation.Key? key = null, Color? color = null, Color? shadowColor = null, Color? surfaceTintColor = null, double? elevation = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, bool borderOnForeground = true, global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin = null, Clip? clipBehavior = null, global::Doroti.Framework.Widgets.Widget? child = null, bool semanticContainer = true)
    {
        var __instance = new Card(key: key, color: color, shadowColor: shadowColor, surfaceTintColor: surfaceTintColor, elevation: elevation, shape: shape, borderOnForeground: borderOnForeground, margin: margin, clipBehavior: clipBehavior, child: child, semanticContainer: semanticContainer);
        __instance.color = color;
        __instance.shadowColor = shadowColor;
        __instance.surfaceTintColor = surfaceTintColor;
        __instance.elevation = elevation;
        __instance.shape = shape;
        __instance.borderOnForeground = borderOnForeground;
        __instance.margin = margin;
        __instance.clipBehavior = clipBehavior;
        __instance.child = child;
        __instance.semanticContainer = semanticContainer;
        __instance._variant = _CardVariant__card.outlined;
        return __instance;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        CardThemeData cardTheme = CardTheme.of(context);
        CardThemeData defaults = default!;
        {
            defaults = _variant switch { _CardVariant__card.elevated => DartRuntimePrimitives.ConvertValue<CardThemeData>(new _CardDefaultsM3__card(context)), _CardVariant__card.filled => DartRuntimePrimitives.ConvertValue<CardThemeData>(new _FilledCardDefaultsM3__card(context)), _CardVariant__card.outlined => DartRuntimePrimitives.ConvertValue<CardThemeData>(new _OutlinedCardDefaultsM3__card(context)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        }
        return new global::Doroti.Framework.Widgets.Semantics(container: semanticContainer, child: new global::Doroti.Framework.Widgets.Padding(padding: (margin ?? cardTheme.margin) ?? defaults.margin!, child: new Material(type: MaterialType.card, color: (color ?? cardTheme.color) ?? defaults.color, shadowColor: (shadowColor ?? cardTheme.shadowColor) ?? defaults.shadowColor, surfaceTintColor: (surfaceTintColor ?? cardTheme.surfaceTintColor) ?? defaults.surfaceTintColor, elevation: (elevation ?? cardTheme.elevation) ?? DartRuntimePrimitives.RequireValue(defaults.elevation), shape: (shape ?? cardTheme.shape) ?? defaults.shape, borderOnForeground: borderOnForeground, clipBehavior: (clipBehavior ?? cardTheme.clipBehavior) ?? DartRuntimePrimitives.RequireValue(defaults.clipBehavior), child: new global::Doroti.Framework.Widgets.Semantics(explicitChildNodes: !semanticContainer, child: child))));
    }

}

internal class _CardDefaultsM3__card : CardThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _CardDefaultsM3__card(global::Doroti.Framework.Widgets.BuildContext context) : base(clipBehavior: Clip.none, elevation: 1.0, margin: EdgeInsets.CreateAll(4.0))
    {
        this.context = context;
    }

    public override global::Doroti.Ui.Color? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.surfaceContainerLow);
    public override global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.shadow);
    public override global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public override global::Doroti.Framework.Painting.ShapeBorder? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.ShapeBorder>(new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(12.0))));
}

internal class _FilledCardDefaultsM3__card : CardThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _FilledCardDefaultsM3__card(global::Doroti.Framework.Widgets.BuildContext context) : base(clipBehavior: Clip.none, elevation: 0.0, margin: EdgeInsets.CreateAll(4.0))
    {
        this.context = context;
    }

    public override global::Doroti.Ui.Color? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.surfaceContainerHighest);
    public override global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.shadow);
    public override global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public override global::Doroti.Framework.Painting.ShapeBorder? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.ShapeBorder>(new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(12.0))));
}

internal class _OutlinedCardDefaultsM3__card : CardThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _OutlinedCardDefaultsM3__card(global::Doroti.Framework.Widgets.BuildContext context) : base(clipBehavior: Clip.none, elevation: 0.0, margin: EdgeInsets.CreateAll(4.0))
    {
        this.context = context;
    }

    public override global::Doroti.Ui.Color? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.surface);
    public override global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.shadow);
    public override global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public override global::Doroti.Framework.Painting.ShapeBorder? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.ShapeBorder>(new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(12.0))).copyWith(side: new global::Doroti.Framework.Painting.BorderSide(color: _colors.outlineVariant)));
}
