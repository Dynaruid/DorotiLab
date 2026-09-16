// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/app_bar_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class AppBarTheme : InheritedTheme, Diagnosticable
{
    internal virtual AppBarThemeData? _data { get; private set; }
    internal virtual Color? _backgroundColor { get; private set; }
    internal virtual Color? _foregroundColor { get; private set; }
    internal virtual double? _elevation { get; private set; }
    internal virtual double? _scrolledUnderElevation { get; private set; }
    internal virtual Color? _shadowColor { get; private set; }
    internal virtual Color? _surfaceTintColor { get; private set; }
    internal virtual ShapeBorder? _shape { get; private set; }
    internal virtual IconThemeData? _iconTheme { get; private set; }
    internal virtual IconThemeData? _actionsIconTheme { get; private set; }
    internal virtual bool? _centerTitle { get; private set; }
    internal virtual double? _titleSpacing { get; private set; }
    internal virtual double? _leadingWidth { get; private set; }
    internal virtual double? _toolbarHeight { get; private set; }
    internal virtual TextStyle? _toolbarTextStyle { get; private set; }
    internal virtual TextStyle? _titleTextStyle { get; private set; }
    internal virtual SystemUiOverlayStyle? _systemOverlayStyle { get; private set; }
    internal virtual EdgeInsetsGeometry? _actionsPadding { get; private set; }

    public AppBarTheme(Key? key = null, Color? color = null, Color? backgroundColor = null, Color? foregroundColor = null, double? elevation = null, double? scrolledUnderElevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, ShapeBorder? shape = null, IconThemeData? iconTheme = null, IconThemeData? actionsIconTheme = null, bool? centerTitle = null, double? titleSpacing = null, double? leadingWidth = null, double? toolbarHeight = null, TextStyle? toolbarTextStyle = null, TextStyle? titleTextStyle = null, SystemUiOverlayStyle? systemOverlayStyle = null, EdgeInsetsGeometry? actionsPadding = null, AppBarThemeData? data = null, Widget? child = null) : base(key: key, child: child ?? new SizedBox())
    {
        _backgroundColor = backgroundColor ?? color;
        _foregroundColor = foregroundColor;
        _elevation = elevation;
        _scrolledUnderElevation = scrolledUnderElevation;
        _shadowColor = shadowColor;
        _surfaceTintColor = surfaceTintColor;
        _shape = shape;
        _iconTheme = iconTheme;
        _actionsIconTheme = actionsIconTheme;
        _centerTitle = centerTitle;
        _titleSpacing = titleSpacing;
        _leadingWidth = leadingWidth;
        _toolbarHeight = toolbarHeight;
        _toolbarTextStyle = toolbarTextStyle;
        _titleTextStyle = titleTextStyle;
        _systemOverlayStyle = systemOverlayStyle;
        _actionsPadding = actionsPadding;
        _data = data;
        System.Diagnostics.Debug.Assert((color is null) || (backgroundColor is null));
        System.Diagnostics.Debug.Assert((data is null) || (((((((((((((((((object?)((color ?? backgroundColor) ?? foregroundColor) ?? elevation) ?? scrolledUnderElevation) ?? shadowColor) ?? surfaceTintColor) ?? shape) ?? iconTheme) ?? actionsIconTheme) ?? centerTitle) ?? titleSpacing) ?? leadingWidth) ?? toolbarHeight) ?? toolbarTextStyle) ?? titleTextStyle) ?? systemOverlayStyle) ?? actionsPadding) is null));
    }

    public virtual Color? backgroundColor => DartRuntimePrimitives.ConvertValue<Color>((_data is not null) ? _data.backgroundColor : _backgroundColor);
    public virtual Color? foregroundColor => DartRuntimePrimitives.ConvertValue<Color>((_data is not null) ? _data.foregroundColor : _foregroundColor);
    public virtual double? elevation => (_data is not null) ? _data.elevation : _elevation;
    public virtual double? scrolledUnderElevation => (_data is not null) ? _data.scrolledUnderElevation : _scrolledUnderElevation;
    public virtual Color? shadowColor => DartRuntimePrimitives.ConvertValue<Color>((_data is not null) ? _data.shadowColor : _shadowColor);
    public virtual Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<Color>((_data is not null) ? _data.surfaceTintColor : _surfaceTintColor);
    public virtual ShapeBorder? shape => (_data is not null) ? _data.shape : _shape;
    public virtual IconThemeData? iconTheme => (_data is not null) ? _data.iconTheme : _iconTheme;
    public virtual IconThemeData? actionsIconTheme => (_data is not null) ? _data.actionsIconTheme : _actionsIconTheme;
    public virtual bool? centerTitle => (_data is not null) ? _data.centerTitle : _centerTitle;
    public virtual double? titleSpacing => (_data is not null) ? _data.titleSpacing : _titleSpacing;
    public virtual double? leadingWidth => (_data is not null) ? _data.leadingWidth : _leadingWidth;
    public virtual double? toolbarHeight => (_data is not null) ? _data.toolbarHeight : _toolbarHeight;
    public virtual TextStyle? toolbarTextStyle => (_data is not null) ? _data.toolbarTextStyle : _toolbarTextStyle;
    public virtual TextStyle? titleTextStyle => (_data is not null) ? _data.titleTextStyle : _titleTextStyle;
    public virtual SystemUiOverlayStyle? systemOverlayStyle => (_data is not null) ? _data.systemOverlayStyle : _systemOverlayStyle;
    public virtual EdgeInsetsGeometry? actionsPadding => (_data is not null) ? _data.actionsPadding : _actionsPadding;
    public virtual AppBarThemeData data => DartRuntimePrimitives.ConvertValue<AppBarThemeData>(_data ?? new AppBarThemeData(backgroundColor: _backgroundColor, foregroundColor: _foregroundColor, elevation: _elevation, scrolledUnderElevation: _scrolledUnderElevation, shadowColor: _shadowColor, surfaceTintColor: _surfaceTintColor, shape: _shape, iconTheme: _iconTheme, actionsIconTheme: _actionsIconTheme, centerTitle: _centerTitle, titleSpacing: _titleSpacing, leadingWidth: _leadingWidth, toolbarHeight: _toolbarHeight, toolbarTextStyle: _toolbarTextStyle, titleTextStyle: _titleTextStyle, systemOverlayStyle: _systemOverlayStyle, actionsPadding: _actionsPadding));
    public virtual AppBarTheme copyWith(IconThemeData? actionsIconTheme = null, Color? color = null, Color? backgroundColor = null, Color? foregroundColor = null, double? elevation = null, double? scrolledUnderElevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, ShapeBorder? shape = null, IconThemeData? iconTheme = null, bool? centerTitle = null, double? titleSpacing = null, double? leadingWidth = null, double? toolbarHeight = null, TextStyle? toolbarTextStyle = null, TextStyle? titleTextStyle = null, SystemUiOverlayStyle? systemOverlayStyle = null, EdgeInsetsGeometry? actionsPadding = null)
    {
        DartRuntimePrimitives.Assert(() => (color is null) || (backgroundColor is null), () => (object?)"The color and backgroundColor parameters mean the same thing. Only specify one.");
        return new AppBarTheme(backgroundColor: (backgroundColor ?? color) ?? this.backgroundColor, foregroundColor: foregroundColor ?? this.foregroundColor, elevation: elevation ?? this.elevation, scrolledUnderElevation: scrolledUnderElevation ?? this.scrolledUnderElevation, shadowColor: shadowColor ?? this.shadowColor, surfaceTintColor: surfaceTintColor ?? this.surfaceTintColor, shape: shape ?? this.shape, iconTheme: iconTheme ?? this.iconTheme, actionsIconTheme: actionsIconTheme ?? this.actionsIconTheme, centerTitle: centerTitle ?? this.centerTitle, titleSpacing: titleSpacing ?? this.titleSpacing, leadingWidth: leadingWidth ?? this.leadingWidth, toolbarHeight: toolbarHeight ?? this.toolbarHeight, toolbarTextStyle: toolbarTextStyle ?? this.toolbarTextStyle, titleTextStyle: titleTextStyle ?? this.titleTextStyle, systemOverlayStyle: systemOverlayStyle ?? this.systemOverlayStyle, actionsPadding: actionsPadding ?? this.actionsPadding);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static AppBarThemeData of(BuildContext context)
    {
        AppBarTheme? appBarThemeLocal = context.dependOnInheritedWidgetOfExactType<AppBarTheme>();
        return appBarThemeLocal?.data ?? Theme.of(context).appBarTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static AppBarTheme lerp(AppBarTheme? a, AppBarTheme? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new AppBarTheme(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), foregroundColor: Dart_uiLibrary.Color.lerp(a?.foregroundColor, b?.foregroundColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), scrolledUnderElevation: Dart_uiLibrary.lerpDouble(a?.scrolledUnderElevation, b?.scrolledUnderElevation, t), shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t), iconTheme: IconThemeData.lerp(a?.iconTheme, b?.iconTheme, t), actionsIconTheme: IconThemeData.lerp(a?.actionsIconTheme, b?.actionsIconTheme, t), centerTitle: (t < 0.5) ? a?.centerTitle : b?.centerTitle, titleSpacing: Dart_uiLibrary.lerpDouble(a?.titleSpacing, b?.titleSpacing, t), leadingWidth: Dart_uiLibrary.lerpDouble(a?.leadingWidth, b?.leadingWidth, t), toolbarHeight: Dart_uiLibrary.lerpDouble(a?.toolbarHeight, b?.toolbarHeight, t), toolbarTextStyle: TextStyle.lerp(a?.toolbarTextStyle, b?.toolbarTextStyle, t), titleTextStyle: TextStyle.lerp(a?.titleTextStyle, b?.titleTextStyle, t), systemOverlayStyle: (t < 0.5) ? a?.systemOverlayStyle : b?.systemOverlayStyle, actionsPadding: EdgeInsetsGeometry.lerp(a?.actionsPadding, b?.actionsPadding, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((AppBarTheme)oldWidget).data));
    public override Widget wrap(BuildContext context, Widget child)
    {
        return new AppBarTheme(data: data, child: child);
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

public class AppBarThemeData : Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? foregroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual double? scrolledUnderElevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual ShapeBorder? shape { get; private set; }
    public virtual IconThemeData? iconTheme { get; private set; }
    public virtual IconThemeData? actionsIconTheme { get; private set; }
    public virtual bool? centerTitle { get; private set; }
    public virtual double? titleSpacing { get; private set; }
    public virtual double? leadingWidth { get; private set; }
    public virtual double? toolbarHeight { get; private set; }
    public virtual TextStyle? toolbarTextStyle { get; private set; }
    public virtual TextStyle? titleTextStyle { get; private set; }
    public virtual SystemUiOverlayStyle? systemOverlayStyle { get; private set; }
    public virtual EdgeInsetsGeometry? actionsPadding { get; private set; }

    public AppBarThemeData(Color? backgroundColor = null, Color? foregroundColor = null, Color? color = null, double? elevation = null, double? scrolledUnderElevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, ShapeBorder? shape = null, IconThemeData? iconTheme = null, IconThemeData? actionsIconTheme = null, bool? centerTitle = null, double? titleSpacing = null, double? leadingWidth = null, double? toolbarHeight = null, TextStyle? toolbarTextStyle = null, TextStyle? titleTextStyle = null, SystemUiOverlayStyle? systemOverlayStyle = null, EdgeInsetsGeometry? actionsPadding = null)
    {
        this.backgroundColor = backgroundColor;
        this.foregroundColor = foregroundColor;
        this.elevation = elevation;
        this.scrolledUnderElevation = scrolledUnderElevation;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.shape = shape;
        this.iconTheme = iconTheme;
        this.actionsIconTheme = actionsIconTheme;
        this.centerTitle = centerTitle;
        this.titleSpacing = titleSpacing;
        this.leadingWidth = leadingWidth;
        this.toolbarHeight = toolbarHeight;
        this.toolbarTextStyle = toolbarTextStyle;
        this.titleTextStyle = titleTextStyle;
        this.systemOverlayStyle = systemOverlayStyle;
        this.actionsPadding = actionsPadding;
        System.Diagnostics.Debug.Assert((color is null) || (backgroundColor is null));
    }

    public virtual AppBarThemeData copyWith(Color? backgroundColor = null, Color? foregroundColor = null, Color? color = null, double? elevation = null, double? scrolledUnderElevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, ShapeBorder? shape = null, IconThemeData? iconTheme = null, IconThemeData? actionsIconTheme = null, bool? centerTitle = null, double? titleSpacing = null, double? leadingWidth = null, double? toolbarHeight = null, TextStyle? toolbarTextStyle = null, TextStyle? titleTextStyle = null, SystemUiOverlayStyle? systemOverlayStyle = null, EdgeInsetsGeometry? actionsPadding = null)
    {
        return new AppBarThemeData(backgroundColor: (backgroundColor ?? color) ?? this.backgroundColor, foregroundColor: foregroundColor ?? this.foregroundColor, elevation: elevation ?? this.elevation, scrolledUnderElevation: scrolledUnderElevation ?? this.scrolledUnderElevation, shadowColor: shadowColor ?? this.shadowColor, surfaceTintColor: surfaceTintColor ?? this.surfaceTintColor, shape: shape ?? this.shape, iconTheme: iconTheme ?? this.iconTheme, actionsIconTheme: actionsIconTheme ?? this.actionsIconTheme, centerTitle: centerTitle ?? this.centerTitle, titleSpacing: titleSpacing ?? this.titleSpacing, leadingWidth: leadingWidth ?? this.leadingWidth, toolbarHeight: toolbarHeight ?? this.toolbarHeight, toolbarTextStyle: toolbarTextStyle ?? this.toolbarTextStyle, titleTextStyle: titleTextStyle ?? this.titleTextStyle, systemOverlayStyle: systemOverlayStyle ?? this.systemOverlayStyle, actionsPadding: actionsPadding ?? this.actionsPadding);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static AppBarThemeData lerp(AppBarThemeData a, AppBarThemeData b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new AppBarThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(a.backgroundColor, b.backgroundColor, t), foregroundColor: Dart_uiLibrary.Color.lerp(a.foregroundColor, b.foregroundColor, t), elevation: Dart_uiLibrary.lerpDouble(a.elevation, b.elevation, t), scrolledUnderElevation: Dart_uiLibrary.lerpDouble(a.scrolledUnderElevation, b.scrolledUnderElevation, t), shadowColor: Dart_uiLibrary.Color.lerp(a.shadowColor, b.shadowColor, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a.surfaceTintColor, b.surfaceTintColor, t), shape: ShapeBorder.lerp(a.shape, b.shape, t), iconTheme: IconThemeData.lerp(a.iconTheme, b.iconTheme, t), actionsIconTheme: IconThemeData.lerp(a.actionsIconTheme, b.actionsIconTheme, t), centerTitle: (t < 0.5) ? a.centerTitle : b.centerTitle, titleSpacing: Dart_uiLibrary.lerpDouble(a.titleSpacing, b.titleSpacing, t), leadingWidth: Dart_uiLibrary.lerpDouble(a.leadingWidth, b.leadingWidth, t), toolbarHeight: Dart_uiLibrary.lerpDouble(a.toolbarHeight, b.toolbarHeight, t), toolbarTextStyle: TextStyle.lerp(a.toolbarTextStyle, b.toolbarTextStyle, t), titleTextStyle: TextStyle.lerp(a.titleTextStyle, b.titleTextStyle, t), systemOverlayStyle: (t < 0.5) ? a.systemOverlayStyle : b.systemOverlayStyle, actionsPadding: EdgeInsetsGeometry.lerp(a.actionsPadding, b.actionsPadding, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(backgroundColor, foregroundColor, elevation, scrolledUnderElevation, shadowColor, surfaceTintColor, shape, iconTheme, actionsIconTheme, centerTitle, titleSpacing, leadingWidth, toolbarHeight, toolbarTextStyle, titleTextStyle, systemOverlayStyle, actionsPadding));
    public override bool Equals(object? other)
    {
        var __other = other as AppBarThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is AppBarThemeData) && Equals(__other.backgroundColor, backgroundColor) && Equals(__other.foregroundColor, foregroundColor) && (__other.elevation == elevation) && (__other.scrolledUnderElevation == scrolledUnderElevation) && Equals(__other.shadowColor, shadowColor) && Equals(__other.surfaceTintColor, surfaceTintColor) && Equals(__other.shape, shape) && Equals(__other.iconTheme, iconTheme) && Equals(__other.actionsIconTheme, actionsIconTheme) && (__other.centerTitle == centerTitle) && (__other.titleSpacing == titleSpacing) && (__other.leadingWidth == leadingWidth) && (__other.toolbarHeight == toolbarHeight) && Equals(__other.toolbarTextStyle, toolbarTextStyle) && Equals(__other.titleTextStyle, titleTextStyle) && Equals(__other.systemOverlayStyle, systemOverlayStyle) && Equals(__other.actionsPadding, actionsPadding);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new ColorProperty("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new ColorProperty("foregroundColor", foregroundColor, defaultValue: null));
        properties.add(new DoubleProperty("elevation", elevation, defaultValue: null));
        properties.add(new DoubleProperty("scrolledUnderElevation", scrolledUnderElevation, defaultValue: null));
        properties.add(new ColorProperty("shadowColor", shadowColor, defaultValue: null));
        properties.add(new ColorProperty("surfaceTintColor", surfaceTintColor, defaultValue: null));
        properties.add(new DiagnosticsProperty<ShapeBorder>("shape", shape, defaultValue: null));
        properties.add(new DiagnosticsProperty<IconThemeData>("iconTheme", iconTheme, defaultValue: null));
        properties.add(new DiagnosticsProperty<IconThemeData>("actionsIconTheme", actionsIconTheme, defaultValue: null));
        properties.add(new DiagnosticsProperty<bool>("centerTitle", centerTitle, defaultValue: null));
        properties.add(new DoubleProperty("titleSpacing", titleSpacing, defaultValue: null));
        properties.add(new DoubleProperty("leadingWidth", leadingWidth, defaultValue: null));
        properties.add(new DoubleProperty("toolbarHeight", toolbarHeight, defaultValue: null));
        properties.add(new DiagnosticsProperty<TextStyle>("toolbarTextStyle", toolbarTextStyle, defaultValue: null));
        properties.add(new DiagnosticsProperty<TextStyle>("titleTextStyle", titleTextStyle, defaultValue: null));
        properties.add(new DiagnosticsProperty<SystemUiOverlayStyle?>("systemOverlayStyle", systemOverlayStyle, defaultValue: null, description: (systemOverlayStyle is null) ? null : $"SystemUiOverlayStyle({string.Join(", ", ((Func<List<string>>)(() => { var __collection24269 = new List<string>(); if (systemOverlayStyle?.systemNavigationBarColor is not null) { __collection24269.Add($"systemNavigationBarColor: {systemOverlayStyle?.systemNavigationBarColor}"); } if (systemOverlayStyle?.systemNavigationBarDividerColor is not null) { __collection24269.Add($"systemNavigationBarDividerColor: {systemOverlayStyle?.systemNavigationBarDividerColor}"); } if (systemOverlayStyle?.systemNavigationBarIconBrightness is not null) { __collection24269.Add($"systemNavigationBarIconBrightness: {systemOverlayStyle?.systemNavigationBarIconBrightness}"); } if (systemOverlayStyle?.statusBarColor is not null) { __collection24269.Add($"statusBarColor: {systemOverlayStyle?.statusBarColor}"); } if (systemOverlayStyle?.statusBarBrightness is not null) { __collection24269.Add($"statusBarBrightness: {systemOverlayStyle?.statusBarBrightness}"); } if (systemOverlayStyle?.statusBarIconBrightness is not null) { __collection24269.Add($"statusBarIconBrightness: {systemOverlayStyle?.statusBarIconBrightness}"); } if (systemOverlayStyle?.systemStatusBarContrastEnforced is not null) { __collection24269.Add($"systemStatusBarContrastEnforced: {systemOverlayStyle?.systemStatusBarContrastEnforced}"); } if (systemOverlayStyle?.systemNavigationBarContrastEnforced is not null) { __collection24269.Add($"systemNavigationBarContrastEnforced: {systemOverlayStyle?.systemNavigationBarContrastEnforced}"); } return __collection24269; }))().where((s) => s.Length != 0))})"));
        properties.add(new DiagnosticsProperty<EdgeInsetsGeometry?>("actionsPadding", actionsPadding, defaultValue: null));
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
