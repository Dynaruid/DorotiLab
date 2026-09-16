// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/tab_bar_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class TabBarTheme : global::Doroti.Framework.Widgets.InheritedTheme, global::Doroti.Framework.Foundation.Diagnosticable
{
    internal virtual TabBarThemeData? _data { get; private set; }
    internal virtual global::Doroti.Framework.Painting.Decoration? _indicator { get; private set; }
    internal virtual Color? _indicatorColor { get; private set; }
    internal virtual TabBarIndicatorSize? _indicatorSize { get; private set; }
    internal virtual Color? _dividerColor { get; private set; }
    internal virtual double? _dividerHeight { get; private set; }
    internal virtual Color? _labelColor { get; private set; }
    internal virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? _labelPadding { get; private set; }
    internal virtual global::Doroti.Framework.Painting.TextStyle? _labelStyle { get; private set; }
    internal virtual Color? _unselectedLabelColor { get; private set; }
    internal virtual global::Doroti.Framework.Painting.TextStyle? _unselectedLabelStyle { get; private set; }
    internal virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? _overlayColor { get; private set; }
    internal virtual InteractiveInkFeatureFactory? _splashFactory { get; private set; }
    internal virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? _mouseCursor { get; private set; }
    internal virtual TabAlignment? _tabAlignment { get; private set; }
    internal virtual global::Doroti.Framework.Painting.TextScaler? _textScaler { get; private set; }
    internal virtual TabIndicatorAnimation? _indicatorAnimation { get; private set; }

    public TabBarTheme(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.Decoration? indicator = null, Color? indicatorColor = null, TabBarIndicatorSize? indicatorSize = null, Color? dividerColor = null, double? dividerHeight = null, Color? labelColor = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding = null, global::Doroti.Framework.Painting.TextStyle? labelStyle = null, Color? unselectedLabelColor = null, global::Doroti.Framework.Painting.TextStyle? unselectedLabelStyle = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, InteractiveInkFeatureFactory? splashFactory = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null, TabAlignment? tabAlignment = null, global::Doroti.Framework.Painting.TextScaler? textScaler = null, TabIndicatorAnimation? indicatorAnimation = null, TabBarThemeData? data = null, global::Doroti.Framework.Widgets.Widget? child = null) : base(key: key, child: child ?? new global::Doroti.Framework.Widgets.SizedBox())
    {
        _indicator = indicator;
        _indicatorColor = indicatorColor;
        _indicatorSize = indicatorSize;
        _dividerColor = dividerColor;
        _dividerHeight = dividerHeight;
        _labelColor = labelColor;
        _labelPadding = labelPadding;
        _labelStyle = labelStyle;
        _unselectedLabelColor = unselectedLabelColor;
        _unselectedLabelStyle = unselectedLabelStyle;
        _overlayColor = overlayColor;
        _splashFactory = splashFactory;
        _mouseCursor = mouseCursor;
        _tabAlignment = tabAlignment;
        _textScaler = textScaler;
        _indicatorAnimation = indicatorAnimation;
        _data = data;
        System.Diagnostics.Debug.Assert((data is null) || (((((((((((((((((object?)indicator ?? indicatorColor) ?? indicatorSize) ?? dividerColor) ?? dividerHeight) ?? labelColor) ?? labelPadding) ?? labelStyle) ?? unselectedLabelColor) ?? unselectedLabelStyle) ?? overlayColor) ?? splashFactory) ?? mouseCursor) ?? tabAlignment) ?? textScaler) ?? indicatorAnimation) is null));
    }

    public virtual global::Doroti.Framework.Painting.Decoration? indicator => (_data is not null) ? _data.indicator : _indicator;
    public virtual global::Doroti.Ui.Color? indicatorColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((_data is not null) ? _data.indicatorColor : _indicatorColor);
    public virtual TabBarIndicatorSize? indicatorSize => (_data is not null) ? _data.indicatorSize : _indicatorSize;
    public virtual global::Doroti.Ui.Color? dividerColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((_data is not null) ? _data.dividerColor : _dividerColor);
    public virtual double? dividerHeight => (_data is not null) ? _data.dividerHeight : _dividerHeight;
    public virtual global::Doroti.Ui.Color? labelColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((_data is not null) ? _data.labelColor : _labelColor);
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding => (_data is not null) ? _data.labelPadding : _labelPadding;
    public virtual global::Doroti.Framework.Painting.TextStyle? labelStyle => (_data is not null) ? _data.labelStyle : _labelStyle;
    public virtual global::Doroti.Ui.Color? unselectedLabelColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((_data is not null) ? _data.unselectedLabelColor : _unselectedLabelColor);
    public virtual global::Doroti.Framework.Painting.TextStyle? unselectedLabelStyle => (_data is not null) ? _data.unselectedLabelStyle : _unselectedLabelStyle;
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>((_data is not null) ? _data.overlayColor : _overlayColor);
    public virtual InteractiveInkFeatureFactory? splashFactory => (_data is not null) ? _data.splashFactory : _splashFactory;
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor => (_data is not null) ? _data.mouseCursor : _mouseCursor;
    public virtual TabAlignment? tabAlignment => (_data is not null) ? _data.tabAlignment : _tabAlignment;
    public virtual global::Doroti.Framework.Painting.TextScaler? textScaler => (_data is not null) ? _data.textScaler : _textScaler;
    public virtual TabIndicatorAnimation? indicatorAnimation => (_data is not null) ? _data.indicatorAnimation : _indicatorAnimation;
    public virtual TabBarThemeData data => DartRuntimePrimitives.ConvertValue<TabBarThemeData>(_data ?? new TabBarThemeData(indicator: _indicator, indicatorColor: _indicatorColor, indicatorSize: _indicatorSize, dividerColor: _dividerColor, dividerHeight: _dividerHeight, labelColor: _labelColor, labelPadding: _labelPadding, labelStyle: _labelStyle, unselectedLabelColor: _unselectedLabelColor, unselectedLabelStyle: _unselectedLabelStyle, overlayColor: _overlayColor, splashFactory: _splashFactory, mouseCursor: _mouseCursor, tabAlignment: _tabAlignment, textScaler: _textScaler, indicatorAnimation: _indicatorAnimation));
    public virtual TabBarTheme copyWith(global::Doroti.Framework.Painting.Decoration? indicator = null, Color? indicatorColor = null, TabBarIndicatorSize? indicatorSize = null, Color? dividerColor = null, double? dividerHeight = null, Color? labelColor = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding = null, global::Doroti.Framework.Painting.TextStyle? labelStyle = null, Color? unselectedLabelColor = null, global::Doroti.Framework.Painting.TextStyle? unselectedLabelStyle = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, InteractiveInkFeatureFactory? splashFactory = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null, TabAlignment? tabAlignment = null, global::Doroti.Framework.Painting.TextScaler? textScaler = null, TabIndicatorAnimation? indicatorAnimation = null)
    {
        return new TabBarTheme(indicator: indicator ?? this.indicator, indicatorColor: indicatorColor ?? this.indicatorColor, indicatorSize: indicatorSize ?? this.indicatorSize, dividerColor: dividerColor ?? this.dividerColor, dividerHeight: dividerHeight ?? this.dividerHeight, labelColor: labelColor ?? this.labelColor, labelPadding: labelPadding ?? this.labelPadding, labelStyle: labelStyle ?? this.labelStyle, unselectedLabelColor: unselectedLabelColor ?? this.unselectedLabelColor, unselectedLabelStyle: unselectedLabelStyle ?? this.unselectedLabelStyle, overlayColor: overlayColor ?? this.overlayColor, splashFactory: splashFactory ?? this.splashFactory, mouseCursor: mouseCursor ?? this.mouseCursor, tabAlignment: tabAlignment ?? this.tabAlignment, textScaler: textScaler ?? this.textScaler, indicatorAnimation: indicatorAnimation ?? this.indicatorAnimation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TabBarThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        TabBarTheme? tabBarThemeLocal = context.dependOnInheritedWidgetOfExactType<TabBarTheme>();
        return tabBarThemeLocal?.data ?? Theme.of(context).tabBarTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TabBarTheme lerp(TabBarTheme a, TabBarTheme b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new TabBarTheme(indicator: Decoration.lerp(a.indicator, b.indicator, t), indicatorColor: Dart_uiLibrary.Color.lerp(a.indicatorColor, b.indicatorColor, t), indicatorSize: (t < 0.5) ? a.indicatorSize : b.indicatorSize, dividerColor: Dart_uiLibrary.Color.lerp(a.dividerColor, b.dividerColor, t), dividerHeight: (t < 0.5) ? a.dividerHeight : b.dividerHeight, labelColor: Dart_uiLibrary.Color.lerp(a.labelColor, b.labelColor, t), labelPadding: EdgeInsetsGeometry.lerp(a.labelPadding, b.labelPadding, t), labelStyle: TextStyle.lerp(a.labelStyle, b.labelStyle, t), unselectedLabelColor: Dart_uiLibrary.Color.lerp(a.unselectedLabelColor, b.unselectedLabelColor, t), unselectedLabelStyle: TextStyle.lerp(a.unselectedLabelStyle, b.unselectedLabelStyle, t), overlayColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a.overlayColor, b.overlayColor, t, Color.lerp), splashFactory: (t < 0.5) ? a.splashFactory : b.splashFactory, mouseCursor: (t < 0.5) ? a.mouseCursor : b.mouseCursor, tabAlignment: (t < 0.5) ? a.tabAlignment : b.tabAlignment, textScaler: (t < 0.5) ? a.textScaler : b.textScaler, indicatorAnimation: (t < 0.5) ? a.indicatorAnimation : b.indicatorAnimation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((TabBarTheme)oldWidget).data));
    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return new TabBarTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
    public override string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
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

    public override DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
    }

}

public class TabBarThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual global::Doroti.Framework.Painting.Decoration? indicator { get; private set; }
    public virtual Color? indicatorColor { get; private set; }
    public virtual TabBarIndicatorSize? indicatorSize { get; private set; }
    public virtual Color? dividerColor { get; private set; }
    public virtual double? dividerHeight { get; private set; }
    public virtual Color? labelColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? labelStyle { get; private set; }
    public virtual Color? unselectedLabelColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? unselectedLabelStyle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual InteractiveInkFeatureFactory? splashFactory { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor { get; private set; }
    public virtual TabAlignment? tabAlignment { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextScaler? textScaler { get; private set; }
    public virtual TabIndicatorAnimation? indicatorAnimation { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderRadius? splashBorderRadius { get; private set; }

    public TabBarThemeData(global::Doroti.Framework.Painting.Decoration? indicator = null, Color? indicatorColor = null, TabBarIndicatorSize? indicatorSize = null, Color? dividerColor = null, double? dividerHeight = null, Color? labelColor = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding = null, global::Doroti.Framework.Painting.TextStyle? labelStyle = null, Color? unselectedLabelColor = null, global::Doroti.Framework.Painting.TextStyle? unselectedLabelStyle = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, InteractiveInkFeatureFactory? splashFactory = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null, TabAlignment? tabAlignment = null, global::Doroti.Framework.Painting.TextScaler? textScaler = null, TabIndicatorAnimation? indicatorAnimation = null, global::Doroti.Framework.Painting.BorderRadius? splashBorderRadius = null)
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

    public virtual TabBarThemeData copyWith(global::Doroti.Framework.Painting.Decoration? indicator = null, Color? indicatorColor = null, TabBarIndicatorSize? indicatorSize = null, Color? dividerColor = null, double? dividerHeight = null, Color? labelColor = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding = null, global::Doroti.Framework.Painting.TextStyle? labelStyle = null, Color? unselectedLabelColor = null, global::Doroti.Framework.Painting.TextStyle? unselectedLabelStyle = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, InteractiveInkFeatureFactory? splashFactory = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null, TabAlignment? tabAlignment = null, global::Doroti.Framework.Painting.TextScaler? textScaler = null, TabIndicatorAnimation? indicatorAnimation = null, global::Doroti.Framework.Painting.BorderRadius? splashBorderRadius = null)
    {
        return new TabBarThemeData(indicator: indicator ?? this.indicator, indicatorColor: indicatorColor ?? this.indicatorColor, indicatorSize: indicatorSize ?? this.indicatorSize, dividerColor: dividerColor ?? this.dividerColor, dividerHeight: dividerHeight ?? this.dividerHeight, labelColor: labelColor ?? this.labelColor, labelPadding: labelPadding ?? this.labelPadding, labelStyle: labelStyle ?? this.labelStyle, unselectedLabelColor: unselectedLabelColor ?? this.unselectedLabelColor, unselectedLabelStyle: unselectedLabelStyle ?? this.unselectedLabelStyle, overlayColor: overlayColor ?? this.overlayColor, splashFactory: splashFactory ?? this.splashFactory, mouseCursor: mouseCursor ?? this.mouseCursor, tabAlignment: tabAlignment ?? this.tabAlignment, textScaler: textScaler ?? this.textScaler, indicatorAnimation: indicatorAnimation ?? this.indicatorAnimation, splashBorderRadius: splashBorderRadius ?? this.splashBorderRadius);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TabBarThemeData lerp(TabBarThemeData a, TabBarThemeData b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new TabBarThemeData(indicator: Decoration.lerp(a.indicator, b.indicator, t), indicatorColor: Dart_uiLibrary.Color.lerp(a.indicatorColor, b.indicatorColor, t), indicatorSize: (t < 0.5) ? a.indicatorSize : b.indicatorSize, dividerColor: Dart_uiLibrary.Color.lerp(a.dividerColor, b.dividerColor, t), dividerHeight: (t < 0.5) ? a.dividerHeight : b.dividerHeight, labelColor: Dart_uiLibrary.Color.lerp(a.labelColor, b.labelColor, t), labelPadding: EdgeInsetsGeometry.lerp(a.labelPadding, b.labelPadding, t), labelStyle: TextStyle.lerp(a.labelStyle, b.labelStyle, t), unselectedLabelColor: Dart_uiLibrary.Color.lerp(a.unselectedLabelColor, b.unselectedLabelColor, t), unselectedLabelStyle: TextStyle.lerp(a.unselectedLabelStyle, b.unselectedLabelStyle, t), overlayColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a.overlayColor, b.overlayColor, t, Color.lerp), splashFactory: (t < 0.5) ? a.splashFactory : b.splashFactory, mouseCursor: (t < 0.5) ? a.mouseCursor : b.mouseCursor, tabAlignment: (t < 0.5) ? a.tabAlignment : b.tabAlignment, textScaler: (t < 0.5) ? a.textScaler : b.textScaler, indicatorAnimation: (t < 0.5) ? a.indicatorAnimation : b.indicatorAnimation, splashBorderRadius: BorderRadius.lerp(a.splashBorderRadius, a.splashBorderRadius, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(indicator, indicatorColor, indicatorSize, dividerColor, dividerHeight, labelColor, labelPadding, labelStyle, unselectedLabelColor, unselectedLabelStyle, overlayColor, splashFactory, mouseCursor, tabAlignment, textScaler, indicatorAnimation, splashBorderRadius));
    public override bool Equals(object? other)
    {
        var __other = other as TabBarThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is TabBarThemeData) && Equals(__other.indicator, indicator) && Equals(__other.indicatorColor, indicatorColor) && Equals(__other.indicatorSize, indicatorSize) && Equals(__other.dividerColor, dividerColor) && (__other.dividerHeight == dividerHeight) && Equals(__other.labelColor, labelColor) && Equals(__other.labelPadding, labelPadding) && Equals(__other.labelStyle, labelStyle) && Equals(__other.unselectedLabelColor, unselectedLabelColor) && Equals(__other.unselectedLabelStyle, unselectedLabelStyle) && Equals(__other.overlayColor, overlayColor) && Equals(__other.splashFactory, splashFactory) && Equals(__other.mouseCursor, mouseCursor) && Equals(__other.tabAlignment, tabAlignment) && Equals(__other.textScaler, textScaler) && Equals(__other.indicatorAnimation, indicatorAnimation) && Equals(__other.splashBorderRadius, splashBorderRadius);
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.Decoration?>("indicator", indicator, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Color?>("indicatorColor", indicatorColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<TabBarIndicatorSize?>("indicatorSize", indicatorSize, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Color?>("dividerColor", dividerColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<double?>("dividerHeight", dividerHeight, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Color?>("labelColor", labelColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>("labelPadding", labelPadding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle?>("labelStyle", labelStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Color?>("unselectedLabelColor", unselectedLabelColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle?>("unselectedLabelStyle", unselectedLabelStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>?>("overlayColor", overlayColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<InteractiveInkFeatureFactory?>("splashFactory", splashFactory, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>?>("mouseCursor", mouseCursor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<TabAlignment?>("tabAlignment", tabAlignment, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextScaler?>("textScaler", textScaler, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<TabIndicatorAnimation?>("indicatorAnimation", indicatorAnimation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.BorderRadius?>("splashBorderRadius", splashBorderRadius, defaultValue: null));
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
