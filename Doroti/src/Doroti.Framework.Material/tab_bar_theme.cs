// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/tab_bar_theme.dart
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

public class TabBarTheme : global::Doroti.Generated.Framework.Widgets.InheritedTheme, global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    internal virtual TabBarThemeData? _data { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Painting.Decoration? _indicator { get; private set; }
    internal virtual Color? _indicatorColor { get; private set; }
    internal virtual TabBarIndicatorSize? _indicatorSize { get; private set; }
    internal virtual Color? _dividerColor { get; private set; }
    internal virtual double? _dividerHeight { get; private set; }
    internal virtual Color? _labelColor { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? _labelPadding { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Painting.TextStyle? _labelStyle { get; private set; }
    internal virtual Color? _unselectedLabelColor { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Painting.TextStyle? _unselectedLabelStyle { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? _overlayColor { get; private set; }
    internal virtual InteractiveInkFeatureFactory? _splashFactory { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? _mouseCursor { get; private set; }
    internal virtual TabAlignment? _tabAlignment { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Painting.TextScaler? _textScaler { get; private set; }
    internal virtual TabIndicatorAnimation? _indicatorAnimation { get; private set; }

    public TabBarTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.Decoration? indicator = null, Color? indicatorColor = null, TabBarIndicatorSize? indicatorSize = null, Color? dividerColor = null, double? dividerHeight = null, Color? labelColor = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? labelPadding = null, global::Doroti.Generated.Framework.Painting.TextStyle? labelStyle = null, Color? unselectedLabelColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? unselectedLabelStyle = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, InteractiveInkFeatureFactory? splashFactory = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor = null, TabAlignment? tabAlignment = null, global::Doroti.Generated.Framework.Painting.TextScaler? textScaler = null, TabIndicatorAnimation? indicatorAnimation = null, TabBarThemeData? data = null, global::Doroti.Generated.Framework.Widgets.Widget? child = null) : base(key: key, child: (child ?? new global::Doroti.Generated.Framework.Widgets.SizedBox()))
    {
        this._indicator = indicator;
        this._indicatorColor = indicatorColor;
        this._indicatorSize = indicatorSize;
        this._dividerColor = dividerColor;
        this._dividerHeight = dividerHeight;
        this._labelColor = labelColor;
        this._labelPadding = labelPadding;
        this._labelStyle = labelStyle;
        this._unselectedLabelColor = unselectedLabelColor;
        this._unselectedLabelStyle = unselectedLabelStyle;
        this._overlayColor = overlayColor;
        this._splashFactory = splashFactory;
        this._mouseCursor = mouseCursor;
        this._tabAlignment = tabAlignment;
        this._textScaler = textScaler;
        this._indicatorAnimation = indicatorAnimation;
        this._data = data;
        System.Diagnostics.Debug.Assert(((data is null) || (((((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)indicator ?? (object?)indicatorColor)) ?? (object?)indicatorSize)) ?? (object?)dividerColor)) ?? (object?)dividerHeight)) ?? (object?)labelColor)) ?? (object?)labelPadding)) ?? (object?)labelStyle)) ?? (object?)unselectedLabelColor)) ?? (object?)unselectedLabelStyle)) ?? (object?)overlayColor)) ?? (object?)splashFactory)) ?? (object?)mouseCursor)) ?? (object?)tabAlignment)) ?? (object?)textScaler)) ?? (object?)indicatorAnimation))) is null)));
    }

    public virtual global::Doroti.Generated.Framework.Painting.Decoration? indicator => ((this._data is not null) ? ((TabBarThemeData)this._data).indicator : this._indicator);
    public virtual global::Doroti.Ui.Color? indicatorColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((TabBarThemeData)this._data).indicatorColor : this._indicatorColor));
    public virtual TabBarIndicatorSize? indicatorSize => ((this._data is not null) ? ((TabBarThemeData)this._data).indicatorSize : this._indicatorSize);
    public virtual global::Doroti.Ui.Color? dividerColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((TabBarThemeData)this._data).dividerColor : this._dividerColor));
    public virtual double? dividerHeight => ((this._data is not null) ? ((TabBarThemeData)this._data).dividerHeight : this._dividerHeight);
    public virtual global::Doroti.Ui.Color? labelColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((TabBarThemeData)this._data).labelColor : this._labelColor));
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? labelPadding => ((this._data is not null) ? ((TabBarThemeData)this._data).labelPadding : this._labelPadding);
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? labelStyle => ((this._data is not null) ? ((TabBarThemeData)this._data).labelStyle : this._labelStyle);
    public virtual global::Doroti.Ui.Color? unselectedLabelColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((TabBarThemeData)this._data).unselectedLabelColor : this._unselectedLabelColor));
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? unselectedLabelStyle => ((this._data is not null) ? ((TabBarThemeData)this._data).unselectedLabelStyle : this._unselectedLabelStyle);
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(((this._data is not null) ? ((TabBarThemeData)this._data).overlayColor : this._overlayColor));
    public virtual InteractiveInkFeatureFactory? splashFactory => ((this._data is not null) ? ((TabBarThemeData)this._data).splashFactory : this._splashFactory);
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor => ((this._data is not null) ? ((TabBarThemeData)this._data).mouseCursor : this._mouseCursor);
    public virtual TabAlignment? tabAlignment => ((this._data is not null) ? ((TabBarThemeData)this._data).tabAlignment : this._tabAlignment);
    public virtual global::Doroti.Generated.Framework.Painting.TextScaler? textScaler => ((this._data is not null) ? ((TabBarThemeData)this._data).textScaler : this._textScaler);
    public virtual TabIndicatorAnimation? indicatorAnimation => ((this._data is not null) ? ((TabBarThemeData)this._data).indicatorAnimation : this._indicatorAnimation);
    public virtual TabBarThemeData data => DartRuntimePrimitives.ConvertValue<TabBarThemeData>((this._data ?? new TabBarThemeData(indicator: this._indicator, indicatorColor: this._indicatorColor, indicatorSize: this._indicatorSize, dividerColor: this._dividerColor, dividerHeight: this._dividerHeight, labelColor: this._labelColor, labelPadding: this._labelPadding, labelStyle: this._labelStyle, unselectedLabelColor: this._unselectedLabelColor, unselectedLabelStyle: this._unselectedLabelStyle, overlayColor: this._overlayColor, splashFactory: this._splashFactory, mouseCursor: this._mouseCursor, tabAlignment: this._tabAlignment, textScaler: this._textScaler, indicatorAnimation: this._indicatorAnimation)));
    public virtual TabBarTheme copyWith(global::Doroti.Generated.Framework.Painting.Decoration? indicator = null, Color? indicatorColor = null, TabBarIndicatorSize? indicatorSize = null, Color? dividerColor = null, double? dividerHeight = null, Color? labelColor = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? labelPadding = null, global::Doroti.Generated.Framework.Painting.TextStyle? labelStyle = null, Color? unselectedLabelColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? unselectedLabelStyle = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, InteractiveInkFeatureFactory? splashFactory = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor = null, TabAlignment? tabAlignment = null, global::Doroti.Generated.Framework.Painting.TextScaler? textScaler = null, TabIndicatorAnimation? indicatorAnimation = null)
    {
        return new TabBarTheme(indicator: (indicator ?? this.indicator), indicatorColor: (indicatorColor ?? this.indicatorColor), indicatorSize: (indicatorSize ?? this.indicatorSize), dividerColor: (dividerColor ?? this.dividerColor), dividerHeight: (dividerHeight ?? this.dividerHeight), labelColor: (labelColor ?? this.labelColor), labelPadding: (labelPadding ?? this.labelPadding), labelStyle: (labelStyle ?? this.labelStyle), unselectedLabelColor: (unselectedLabelColor ?? this.unselectedLabelColor), unselectedLabelStyle: (unselectedLabelStyle ?? this.unselectedLabelStyle), overlayColor: (overlayColor ?? this.overlayColor), splashFactory: (splashFactory ?? this.splashFactory), mouseCursor: (mouseCursor ?? this.mouseCursor), tabAlignment: (tabAlignment ?? this.tabAlignment), textScaler: (textScaler ?? this.textScaler), indicatorAnimation: (indicatorAnimation ?? this.indicatorAnimation));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TabBarThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        TabBarTheme? tabBarTheme__11741 = ((TabBarTheme?)(object?)context.dependOnInheritedWidgetOfExactType<TabBarTheme>());
        return (tabBarTheme__11741?.data ?? Theme.of(context).tabBarTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TabBarTheme lerp(TabBarTheme a, TabBarTheme b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new TabBarTheme(indicator: Decoration.lerp(((TabBarTheme)a).indicator, ((TabBarTheme)b).indicator, t), indicatorColor: Dart_uiLibrary.Color.lerp(((TabBarTheme)a).indicatorColor, ((TabBarTheme)b).indicatorColor, t), indicatorSize: ((t < 0.5) ? ((TabBarTheme)a).indicatorSize : ((TabBarTheme)b).indicatorSize), dividerColor: Dart_uiLibrary.Color.lerp(((TabBarTheme)a).dividerColor, ((TabBarTheme)b).dividerColor, t), dividerHeight: ((t < 0.5) ? ((TabBarTheme)a).dividerHeight : ((TabBarTheme)b).dividerHeight), labelColor: Dart_uiLibrary.Color.lerp(((TabBarTheme)a).labelColor, ((TabBarTheme)b).labelColor, t), labelPadding: EdgeInsetsGeometry.lerp(((TabBarTheme)a).labelPadding, ((TabBarTheme)b).labelPadding, t), labelStyle: TextStyle.lerp(((TabBarTheme)a).labelStyle, ((TabBarTheme)b).labelStyle, t), unselectedLabelColor: Dart_uiLibrary.Color.lerp(((TabBarTheme)a).unselectedLabelColor, ((TabBarTheme)b).unselectedLabelColor, t), unselectedLabelStyle: TextStyle.lerp(((TabBarTheme)a).unselectedLabelStyle, ((TabBarTheme)b).unselectedLabelStyle, t), overlayColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(((TabBarTheme)a).overlayColor, ((TabBarTheme)b).overlayColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), splashFactory: ((t < 0.5) ? ((TabBarTheme)a).splashFactory : ((TabBarTheme)b).splashFactory), mouseCursor: ((t < 0.5) ? ((TabBarTheme)a).mouseCursor : ((TabBarTheme)b).mouseCursor), tabAlignment: ((t < 0.5) ? ((TabBarTheme)a).tabAlignment : ((TabBarTheme)b).tabAlignment), textScaler: ((t < 0.5) ? ((TabBarTheme)a).textScaler : ((TabBarTheme)b).textScaler), indicatorAnimation: ((t < 0.5) ? ((TabBarTheme)a).indicatorAnimation : ((TabBarTheme)b).indicatorAnimation));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((TabBarTheme)oldWidget).data)));
    public override global::Doroti.Generated.Framework.Widgets.Widget wrap(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new TabBarTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string toStringShort() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
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

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
    }

}

public class TabBarThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual global::Doroti.Generated.Framework.Painting.Decoration? indicator { get; private set; }
    public virtual Color? indicatorColor { get; private set; }
    public virtual TabBarIndicatorSize? indicatorSize { get; private set; }
    public virtual Color? dividerColor { get; private set; }
    public virtual double? dividerHeight { get; private set; }
    public virtual Color? labelColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? labelPadding { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? labelStyle { get; private set; }
    public virtual Color? unselectedLabelColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? unselectedLabelStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual InteractiveInkFeatureFactory? splashFactory { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor { get; private set; }
    public virtual TabAlignment? tabAlignment { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextScaler? textScaler { get; private set; }
    public virtual TabIndicatorAnimation? indicatorAnimation { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadius? splashBorderRadius { get; private set; }

    public TabBarThemeData(global::Doroti.Generated.Framework.Painting.Decoration? indicator = null, Color? indicatorColor = null, TabBarIndicatorSize? indicatorSize = null, Color? dividerColor = null, double? dividerHeight = null, Color? labelColor = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? labelPadding = null, global::Doroti.Generated.Framework.Painting.TextStyle? labelStyle = null, Color? unselectedLabelColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? unselectedLabelStyle = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, InteractiveInkFeatureFactory? splashFactory = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor = null, TabAlignment? tabAlignment = null, global::Doroti.Generated.Framework.Painting.TextScaler? textScaler = null, TabIndicatorAnimation? indicatorAnimation = null, global::Doroti.Generated.Framework.Painting.BorderRadius? splashBorderRadius = null)
    {
        this.indicator = indicator;
        this.indicatorColor = indicatorColor;
        this.indicatorSize = indicatorSize;
        this.dividerColor = dividerColor;
        this.dividerHeight = dividerHeight;
        this.labelColor = labelColor;
        this.labelPadding = labelPadding;
        this.labelStyle = labelStyle;
        this.unselectedLabelColor = unselectedLabelColor;
        this.unselectedLabelStyle = unselectedLabelStyle;
        this.overlayColor = overlayColor;
        this.splashFactory = splashFactory;
        this.mouseCursor = mouseCursor;
        this.tabAlignment = tabAlignment;
        this.textScaler = textScaler;
        this.indicatorAnimation = indicatorAnimation;
        this.splashBorderRadius = splashBorderRadius;
    }

    public virtual TabBarThemeData copyWith(global::Doroti.Generated.Framework.Painting.Decoration? indicator = null, Color? indicatorColor = null, TabBarIndicatorSize? indicatorSize = null, Color? dividerColor = null, double? dividerHeight = null, Color? labelColor = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? labelPadding = null, global::Doroti.Generated.Framework.Painting.TextStyle? labelStyle = null, Color? unselectedLabelColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? unselectedLabelStyle = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, InteractiveInkFeatureFactory? splashFactory = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor = null, TabAlignment? tabAlignment = null, global::Doroti.Generated.Framework.Painting.TextScaler? textScaler = null, TabIndicatorAnimation? indicatorAnimation = null, global::Doroti.Generated.Framework.Painting.BorderRadius? splashBorderRadius = null)
    {
        return new TabBarThemeData(indicator: (indicator ?? this.indicator), indicatorColor: (indicatorColor ?? this.indicatorColor), indicatorSize: (indicatorSize ?? this.indicatorSize), dividerColor: (dividerColor ?? this.dividerColor), dividerHeight: (dividerHeight ?? this.dividerHeight), labelColor: (labelColor ?? this.labelColor), labelPadding: (labelPadding ?? this.labelPadding), labelStyle: (labelStyle ?? this.labelStyle), unselectedLabelColor: (unselectedLabelColor ?? this.unselectedLabelColor), unselectedLabelStyle: (unselectedLabelStyle ?? this.unselectedLabelStyle), overlayColor: (overlayColor ?? this.overlayColor), splashFactory: (splashFactory ?? this.splashFactory), mouseCursor: (mouseCursor ?? this.mouseCursor), tabAlignment: (tabAlignment ?? this.tabAlignment), textScaler: (textScaler ?? this.textScaler), indicatorAnimation: (indicatorAnimation ?? this.indicatorAnimation), splashBorderRadius: (splashBorderRadius ?? this.splashBorderRadius));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TabBarThemeData lerp(TabBarThemeData a, TabBarThemeData b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new TabBarThemeData(indicator: Decoration.lerp(((TabBarThemeData)a).indicator, ((TabBarThemeData)b).indicator, t), indicatorColor: Dart_uiLibrary.Color.lerp(((TabBarThemeData)a).indicatorColor, ((TabBarThemeData)b).indicatorColor, t), indicatorSize: ((t < 0.5) ? ((TabBarThemeData)a).indicatorSize : ((TabBarThemeData)b).indicatorSize), dividerColor: Dart_uiLibrary.Color.lerp(((TabBarThemeData)a).dividerColor, ((TabBarThemeData)b).dividerColor, t), dividerHeight: ((t < 0.5) ? ((TabBarThemeData)a).dividerHeight : ((TabBarThemeData)b).dividerHeight), labelColor: Dart_uiLibrary.Color.lerp(((TabBarThemeData)a).labelColor, ((TabBarThemeData)b).labelColor, t), labelPadding: EdgeInsetsGeometry.lerp(((TabBarThemeData)a).labelPadding, ((TabBarThemeData)b).labelPadding, t), labelStyle: TextStyle.lerp(((TabBarThemeData)a).labelStyle, ((TabBarThemeData)b).labelStyle, t), unselectedLabelColor: Dart_uiLibrary.Color.lerp(((TabBarThemeData)a).unselectedLabelColor, ((TabBarThemeData)b).unselectedLabelColor, t), unselectedLabelStyle: TextStyle.lerp(((TabBarThemeData)a).unselectedLabelStyle, ((TabBarThemeData)b).unselectedLabelStyle, t), overlayColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(((TabBarThemeData)a).overlayColor, ((TabBarThemeData)b).overlayColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), splashFactory: ((t < 0.5) ? ((TabBarThemeData)a).splashFactory : ((TabBarThemeData)b).splashFactory), mouseCursor: ((t < 0.5) ? ((TabBarThemeData)a).mouseCursor : ((TabBarThemeData)b).mouseCursor), tabAlignment: ((t < 0.5) ? ((TabBarThemeData)a).tabAlignment : ((TabBarThemeData)b).tabAlignment), textScaler: ((t < 0.5) ? ((TabBarThemeData)a).textScaler : ((TabBarThemeData)b).textScaler), indicatorAnimation: ((t < 0.5) ? ((TabBarThemeData)a).indicatorAnimation : ((TabBarThemeData)b).indicatorAnimation), splashBorderRadius: BorderRadius.lerp(((TabBarThemeData)a).splashBorderRadius, ((TabBarThemeData)a).splashBorderRadius, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.indicator, this.indicatorColor, this.indicatorSize, this.dividerColor, this.dividerHeight, this.labelColor, this.labelPadding, this.labelStyle, this.unselectedLabelColor, this.unselectedLabelStyle, this.overlayColor, this.splashFactory, this.mouseCursor, this.tabAlignment, this.textScaler, this.indicatorAnimation, this.splashBorderRadius));
    public override bool Equals(object? other)
    {
        var __other = other as TabBarThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((((((((((__other is TabBarThemeData) && (object.Equals(((TabBarThemeData)((TabBarThemeData)__other)).indicator, this.indicator))) && (object.Equals(((TabBarThemeData)((TabBarThemeData)__other)).indicatorColor, this.indicatorColor))) && (object.Equals(((TabBarThemeData)((TabBarThemeData)__other)).indicatorSize, this.indicatorSize))) && (object.Equals(((TabBarThemeData)((TabBarThemeData)__other)).dividerColor, this.dividerColor))) && (((TabBarThemeData)((TabBarThemeData)__other)).dividerHeight == this.dividerHeight)) && (object.Equals(((TabBarThemeData)((TabBarThemeData)__other)).labelColor, this.labelColor))) && (object.Equals(((TabBarThemeData)((TabBarThemeData)__other)).labelPadding, this.labelPadding))) && (object.Equals(((TabBarThemeData)((TabBarThemeData)__other)).labelStyle, this.labelStyle))) && (object.Equals(((TabBarThemeData)((TabBarThemeData)__other)).unselectedLabelColor, this.unselectedLabelColor))) && (object.Equals(((TabBarThemeData)((TabBarThemeData)__other)).unselectedLabelStyle, this.unselectedLabelStyle))) && (object.Equals(((TabBarThemeData)((TabBarThemeData)__other)).overlayColor, this.overlayColor))) && (object.Equals(((TabBarThemeData)((TabBarThemeData)__other)).splashFactory, this.splashFactory))) && (object.Equals(((TabBarThemeData)((TabBarThemeData)__other)).mouseCursor, this.mouseCursor))) && (object.Equals(((TabBarThemeData)((TabBarThemeData)__other)).tabAlignment, this.tabAlignment))) && (object.Equals(((TabBarThemeData)((TabBarThemeData)__other)).textScaler, this.textScaler))) && (object.Equals(((TabBarThemeData)((TabBarThemeData)__other)).indicatorAnimation, this.indicatorAnimation))) && (object.Equals(((TabBarThemeData)((TabBarThemeData)__other)).splashBorderRadius, this.splashBorderRadius)));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.Decoration?>("indicator", this.indicator, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Color?>("indicatorColor", this.indicatorColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<TabBarIndicatorSize?>("indicatorSize", this.indicatorSize, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Color?>("dividerColor", this.dividerColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<double?>("dividerHeight", this.dividerHeight, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Color?>("labelColor", this.labelColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>("labelPadding", this.labelPadding, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>("labelStyle", this.labelStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Color?>("unselectedLabelColor", this.unselectedLabelColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>("unselectedLabelStyle", this.unselectedLabelStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>?>("overlayColor", this.overlayColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<InteractiveInkFeatureFactory?>("splashFactory", this.splashFactory, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>?>("mouseCursor", this.mouseCursor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<TabAlignment?>("tabAlignment", this.tabAlignment, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextScaler?>("textScaler", this.textScaler, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<TabIndicatorAnimation?>("indicatorAnimation", this.indicatorAnimation, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.BorderRadius?>("splashBorderRadius", this.splashBorderRadius, defaultValue: null));
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
