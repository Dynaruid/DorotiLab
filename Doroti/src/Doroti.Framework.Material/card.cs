// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/card.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

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
        this._variant = _CardVariant__card.elevated;
        System.Diagnostics.Debug.Assert(((elevation is null) || (elevation >= 0.0)));
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
        CardThemeData cardTheme__8905 = CardTheme.of(context);
        CardThemeData defaults__8964 = default!;
        if (Theme.of(context).useMaterial3)
        {
            defaults__8964 = (this._variant switch { _CardVariant__card.elevated => DartRuntimePrimitives.ConvertValue<CardThemeData>(new _CardDefaultsM3__card(context)), _CardVariant__card.filled => DartRuntimePrimitives.ConvertValue<CardThemeData>(new _FilledCardDefaultsM3__card(context)), _CardVariant__card.outlined => DartRuntimePrimitives.ConvertValue<CardThemeData>(new _OutlinedCardDefaultsM3__card(context)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        }
        else
        {
            defaults__8964 = DartRuntimePrimitives.ConvertValue<CardThemeData>(new _CardDefaultsM2__card(context));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(container: this.semanticContainer, child: new global::Doroti.Framework.Widgets.Padding(padding: ((this.margin ?? cardTheme__8905.margin) ?? defaults__8964.margin!), child: new Material(type: MaterialType.card, color: ((this.color ?? cardTheme__8905.color) ?? defaults__8964.color), shadowColor: ((this.shadowColor ?? cardTheme__8905.shadowColor) ?? defaults__8964.shadowColor), surfaceTintColor: ((this.surfaceTintColor ?? cardTheme__8905.surfaceTintColor) ?? defaults__8964.surfaceTintColor), elevation: ((this.elevation ?? cardTheme__8905.elevation) ?? DartRuntimePrimitives.RequireValue(defaults__8964.elevation)), shape: ((this.shape ?? cardTheme__8905.shape) ?? defaults__8964.shape), borderOnForeground: this.borderOnForeground, clipBehavior: ((this.clipBehavior ?? cardTheme__8905.clipBehavior) ?? DartRuntimePrimitives.RequireValue(defaults__8964.clipBehavior)), child: new global::Doroti.Framework.Widgets.Semantics(explicitChildNodes: !this.semanticContainer, child: this.child)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CardDefaultsM2__card : CardThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;

    internal _CardDefaultsM2__card(global::Doroti.Framework.Widgets.BuildContext context) : base(clipBehavior: Clip.none, elevation: 1.0, margin: global::Doroti.Framework.Painting.EdgeInsets.CreateAll(4.0), shape: new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(4.0))))
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Theme.of(this.context).cardColor);
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Theme.of(this.context).shadowColor);
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
                __late__colors = Theme.of(this.context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _CardDefaultsM3__card(global::Doroti.Framework.Widgets.BuildContext context) : base(clipBehavior: Clip.none, elevation: 1.0, margin: global::Doroti.Framework.Painting.EdgeInsets.CreateAll(4.0))
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.surfaceContainerLow);
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.shadow);
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public override global::Doroti.Framework.Painting.ShapeBorder? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.ShapeBorder>(new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(12.0))));
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
                __late__colors = Theme.of(this.context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _FilledCardDefaultsM3__card(global::Doroti.Framework.Widgets.BuildContext context) : base(clipBehavior: Clip.none, elevation: 0.0, margin: global::Doroti.Framework.Painting.EdgeInsets.CreateAll(4.0))
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.surfaceContainerHighest);
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.shadow);
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public override global::Doroti.Framework.Painting.ShapeBorder? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.ShapeBorder>(new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(12.0))));
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
                __late__colors = Theme.of(this.context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _OutlinedCardDefaultsM3__card(global::Doroti.Framework.Widgets.BuildContext context) : base(clipBehavior: Clip.none, elevation: 0.0, margin: global::Doroti.Framework.Painting.EdgeInsets.CreateAll(4.0))
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.surface);
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.shadow);
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public override global::Doroti.Framework.Painting.ShapeBorder? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.ShapeBorder>(new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(12.0))).copyWith(side: new global::Doroti.Framework.Painting.BorderSide(color: this._colors.outlineVariant)));
}
