// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/app_bar_theme.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class AppBarTheme : global::Doroti.Generated.Framework.Widgets.InheritedTheme, global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    internal virtual AppBarThemeData? _data { get; private set; }
    internal virtual Color? _backgroundColor { get; private set; }
    internal virtual Color? _foregroundColor { get; private set; }
    internal virtual double? _elevation { get; private set; }
    internal virtual double? _scrolledUnderElevation { get; private set; }
    internal virtual Color? _shadowColor { get; private set; }
    internal virtual Color? _surfaceTintColor { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? _shape { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Widgets.IconThemeData? _iconTheme { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Widgets.IconThemeData? _actionsIconTheme { get; private set; }
    internal virtual bool? _centerTitle { get; private set; }
    internal virtual double? _titleSpacing { get; private set; }
    internal virtual double? _leadingWidth { get; private set; }
    internal virtual double? _toolbarHeight { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Painting.TextStyle? _toolbarTextStyle { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Painting.TextStyle? _titleTextStyle { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Services.SystemUiOverlayStyle? _systemOverlayStyle { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? _actionsPadding { get; private set; }

    public AppBarTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, Color? color = null, Color? backgroundColor = null, Color? foregroundColor = null, double? elevation = null, double? scrolledUnderElevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Generated.Framework.Widgets.IconThemeData? iconTheme = null, global::Doroti.Generated.Framework.Widgets.IconThemeData? actionsIconTheme = null, bool? centerTitle = null, double? titleSpacing = null, double? leadingWidth = null, double? toolbarHeight = null, global::Doroti.Generated.Framework.Painting.TextStyle? toolbarTextStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Generated.Framework.Services.SystemUiOverlayStyle? systemOverlayStyle = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? actionsPadding = null, AppBarThemeData? data = null, global::Doroti.Generated.Framework.Widgets.Widget? child = null) : base(key: key, child: (child ?? new global::Doroti.Generated.Framework.Widgets.SizedBox()))
    {
        this._backgroundColor = (backgroundColor ?? color);
        this._foregroundColor = foregroundColor;
        this._elevation = elevation;
        this._scrolledUnderElevation = scrolledUnderElevation;
        this._shadowColor = shadowColor;
        this._surfaceTintColor = surfaceTintColor;
        this._shape = shape;
        this._iconTheme = iconTheme;
        this._actionsIconTheme = actionsIconTheme;
        this._centerTitle = centerTitle;
        this._titleSpacing = titleSpacing;
        this._leadingWidth = leadingWidth;
        this._toolbarHeight = toolbarHeight;
        this._toolbarTextStyle = toolbarTextStyle;
        this._titleTextStyle = titleTextStyle;
        this._systemOverlayStyle = systemOverlayStyle;
        this._actionsPadding = actionsPadding;
        this._data = data;
        System.Diagnostics.Debug.Assert(((color is null) || (backgroundColor is null)));
        System.Diagnostics.Debug.Assert(((data is null) || (((((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)((color ?? backgroundColor) ?? foregroundColor) ?? (object?)elevation)) ?? (object?)scrolledUnderElevation)) ?? (object?)shadowColor)) ?? (object?)surfaceTintColor)) ?? (object?)shape)) ?? (object?)iconTheme)) ?? (object?)actionsIconTheme)) ?? (object?)centerTitle)) ?? (object?)titleSpacing)) ?? (object?)leadingWidth)) ?? (object?)toolbarHeight)) ?? (object?)toolbarTextStyle)) ?? (object?)titleTextStyle)) ?? (object?)systemOverlayStyle)) ?? (object?)actionsPadding))) is null)));
    }

    public virtual global::Doroti.Flutter.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(((this._data is not null) ? ((AppBarThemeData)this._data).backgroundColor : this._backgroundColor));
    public virtual global::Doroti.Flutter.Ui.Color? foregroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(((this._data is not null) ? ((AppBarThemeData)this._data).foregroundColor : this._foregroundColor));
    public virtual double? elevation => ((this._data is not null) ? ((AppBarThemeData)this._data).elevation : this._elevation);
    public virtual double? scrolledUnderElevation => ((this._data is not null) ? ((AppBarThemeData)this._data).scrolledUnderElevation : this._scrolledUnderElevation);
    public virtual global::Doroti.Flutter.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(((this._data is not null) ? ((AppBarThemeData)this._data).shadowColor : this._shadowColor));
    public virtual global::Doroti.Flutter.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(((this._data is not null) ? ((AppBarThemeData)this._data).surfaceTintColor : this._surfaceTintColor));
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape => ((this._data is not null) ? ((AppBarThemeData)this._data).shape : this._shape);
    public virtual global::Doroti.Generated.Framework.Widgets.IconThemeData? iconTheme => ((this._data is not null) ? ((AppBarThemeData)this._data).iconTheme : this._iconTheme);
    public virtual global::Doroti.Generated.Framework.Widgets.IconThemeData? actionsIconTheme => ((this._data is not null) ? ((AppBarThemeData)this._data).actionsIconTheme : this._actionsIconTheme);
    public virtual bool? centerTitle => ((this._data is not null) ? ((AppBarThemeData)this._data).centerTitle : this._centerTitle);
    public virtual double? titleSpacing => ((this._data is not null) ? ((AppBarThemeData)this._data).titleSpacing : this._titleSpacing);
    public virtual double? leadingWidth => ((this._data is not null) ? ((AppBarThemeData)this._data).leadingWidth : this._leadingWidth);
    public virtual double? toolbarHeight => ((this._data is not null) ? ((AppBarThemeData)this._data).toolbarHeight : this._toolbarHeight);
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? toolbarTextStyle => ((this._data is not null) ? ((AppBarThemeData)this._data).toolbarTextStyle : this._toolbarTextStyle);
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle => ((this._data is not null) ? ((AppBarThemeData)this._data).titleTextStyle : this._titleTextStyle);
    public virtual global::Doroti.Generated.Framework.Services.SystemUiOverlayStyle? systemOverlayStyle => ((this._data is not null) ? ((AppBarThemeData)this._data).systemOverlayStyle : this._systemOverlayStyle);
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? actionsPadding => ((this._data is not null) ? ((AppBarThemeData)this._data).actionsPadding : this._actionsPadding);
    public virtual AppBarThemeData data => DartRuntimePrimitives.ConvertValue<AppBarThemeData>((this._data ?? new AppBarThemeData(backgroundColor: this._backgroundColor, foregroundColor: this._foregroundColor, elevation: this._elevation, scrolledUnderElevation: this._scrolledUnderElevation, shadowColor: this._shadowColor, surfaceTintColor: this._surfaceTintColor, shape: this._shape, iconTheme: this._iconTheme, actionsIconTheme: this._actionsIconTheme, centerTitle: this._centerTitle, titleSpacing: this._titleSpacing, leadingWidth: this._leadingWidth, toolbarHeight: this._toolbarHeight, toolbarTextStyle: this._toolbarTextStyle, titleTextStyle: this._titleTextStyle, systemOverlayStyle: this._systemOverlayStyle, actionsPadding: this._actionsPadding)));
    public virtual AppBarTheme copyWith(global::Doroti.Generated.Framework.Widgets.IconThemeData? actionsIconTheme = null, Color? color = null, Color? backgroundColor = null, Color? foregroundColor = null, double? elevation = null, double? scrolledUnderElevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Generated.Framework.Widgets.IconThemeData? iconTheme = null, bool? centerTitle = null, double? titleSpacing = null, double? leadingWidth = null, double? toolbarHeight = null, global::Doroti.Generated.Framework.Painting.TextStyle? toolbarTextStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Generated.Framework.Services.SystemUiOverlayStyle? systemOverlayStyle = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? actionsPadding = null)
    {
        DartRuntimePrimitives.Assert(() => ((color is null) || (backgroundColor is null)), () => (object?)"The color and backgroundColor parameters mean the same thing. Only specify one.");
        return new AppBarTheme(backgroundColor: ((backgroundColor ?? color) ?? this.backgroundColor), foregroundColor: (foregroundColor ?? this.foregroundColor), elevation: (elevation ?? this.elevation), scrolledUnderElevation: (scrolledUnderElevation ?? this.scrolledUnderElevation), shadowColor: (shadowColor ?? this.shadowColor), surfaceTintColor: (surfaceTintColor ?? this.surfaceTintColor), shape: (shape ?? this.shape), iconTheme: (iconTheme ?? this.iconTheme), actionsIconTheme: (actionsIconTheme ?? this.actionsIconTheme), centerTitle: (centerTitle ?? this.centerTitle), titleSpacing: (titleSpacing ?? this.titleSpacing), leadingWidth: (leadingWidth ?? this.leadingWidth), toolbarHeight: (toolbarHeight ?? this.toolbarHeight), toolbarTextStyle: (toolbarTextStyle ?? this.toolbarTextStyle), titleTextStyle: (titleTextStyle ?? this.titleTextStyle), systemOverlayStyle: (systemOverlayStyle ?? this.systemOverlayStyle), actionsPadding: (actionsPadding ?? this.actionsPadding));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static AppBarThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        AppBarTheme? appBarTheme__12409 = ((AppBarTheme?)(object?)context.dependOnInheritedWidgetOfExactType<AppBarTheme>());
        return (appBarTheme__12409?.data ?? Theme.of(context).appBarTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static AppBarTheme lerp(AppBarTheme? a, AppBarTheme? b, double t)
    {
        if ((DartRuntimePrimitives.Identical(a, b) && (a is not null)))
        {
            return a;
        }
        return new AppBarTheme(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), foregroundColor: Dart_uiLibrary.Color.lerp(a?.foregroundColor, b?.foregroundColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), scrolledUnderElevation: Dart_uiLibrary.lerpDouble(a?.scrolledUnderElevation, b?.scrolledUnderElevation, t), shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t), iconTheme: IconThemeData.lerp(a?.iconTheme, b?.iconTheme, t), actionsIconTheme: IconThemeData.lerp(a?.actionsIconTheme, b?.actionsIconTheme, t), centerTitle: ((t < 0.5) ? a?.centerTitle : b?.centerTitle), titleSpacing: Dart_uiLibrary.lerpDouble(a?.titleSpacing, b?.titleSpacing, t), leadingWidth: Dart_uiLibrary.lerpDouble(a?.leadingWidth, b?.leadingWidth, t), toolbarHeight: Dart_uiLibrary.lerpDouble(a?.toolbarHeight, b?.toolbarHeight, t), toolbarTextStyle: TextStyle.lerp(a?.toolbarTextStyle, b?.toolbarTextStyle, t), titleTextStyle: TextStyle.lerp(a?.titleTextStyle, b?.titleTextStyle, t), systemOverlayStyle: ((t < 0.5) ? a?.systemOverlayStyle : b?.systemOverlayStyle), actionsPadding: EdgeInsetsGeometry.lerp(a?.actionsPadding, b?.actionsPadding, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((AppBarTheme)oldWidget).data)));
    public override global::Doroti.Generated.Framework.Widgets.Widget wrap(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new AppBarTheme(data: this.data, child: child));
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

public class AppBarThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? foregroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual double? scrolledUnderElevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.IconThemeData? iconTheme { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.IconThemeData? actionsIconTheme { get; private set; }
    public virtual bool? centerTitle { get; private set; }
    public virtual double? titleSpacing { get; private set; }
    public virtual double? leadingWidth { get; private set; }
    public virtual double? toolbarHeight { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? toolbarTextStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.SystemUiOverlayStyle? systemOverlayStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? actionsPadding { get; private set; }

    public AppBarThemeData(Color? backgroundColor = null, Color? foregroundColor = null, Color? color = null, double? elevation = null, double? scrolledUnderElevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Generated.Framework.Widgets.IconThemeData? iconTheme = null, global::Doroti.Generated.Framework.Widgets.IconThemeData? actionsIconTheme = null, bool? centerTitle = null, double? titleSpacing = null, double? leadingWidth = null, double? toolbarHeight = null, global::Doroti.Generated.Framework.Painting.TextStyle? toolbarTextStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Generated.Framework.Services.SystemUiOverlayStyle? systemOverlayStyle = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? actionsPadding = null)
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
        System.Diagnostics.Debug.Assert(((color is null) || (backgroundColor is null)));
    }

    public virtual AppBarThemeData copyWith(Color? backgroundColor = null, Color? foregroundColor = null, Color? color = null, double? elevation = null, double? scrolledUnderElevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Generated.Framework.Widgets.IconThemeData? iconTheme = null, global::Doroti.Generated.Framework.Widgets.IconThemeData? actionsIconTheme = null, bool? centerTitle = null, double? titleSpacing = null, double? leadingWidth = null, double? toolbarHeight = null, global::Doroti.Generated.Framework.Painting.TextStyle? toolbarTextStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Generated.Framework.Services.SystemUiOverlayStyle? systemOverlayStyle = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? actionsPadding = null)
    {
        return new AppBarThemeData(backgroundColor: ((backgroundColor ?? color) ?? this.backgroundColor), foregroundColor: (foregroundColor ?? this.foregroundColor), elevation: (elevation ?? this.elevation), scrolledUnderElevation: (scrolledUnderElevation ?? this.scrolledUnderElevation), shadowColor: (shadowColor ?? this.shadowColor), surfaceTintColor: (surfaceTintColor ?? this.surfaceTintColor), shape: (shape ?? this.shape), iconTheme: (iconTheme ?? this.iconTheme), actionsIconTheme: (actionsIconTheme ?? this.actionsIconTheme), centerTitle: (centerTitle ?? this.centerTitle), titleSpacing: (titleSpacing ?? this.titleSpacing), leadingWidth: (leadingWidth ?? this.leadingWidth), toolbarHeight: (toolbarHeight ?? this.toolbarHeight), toolbarTextStyle: (toolbarTextStyle ?? this.toolbarTextStyle), titleTextStyle: (titleTextStyle ?? this.titleTextStyle), systemOverlayStyle: (systemOverlayStyle ?? this.systemOverlayStyle), actionsPadding: (actionsPadding ?? this.actionsPadding));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static AppBarThemeData lerp(AppBarThemeData a, AppBarThemeData b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new AppBarThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(((AppBarThemeData)a).backgroundColor, ((AppBarThemeData)b).backgroundColor, t), foregroundColor: Dart_uiLibrary.Color.lerp(((AppBarThemeData)a).foregroundColor, ((AppBarThemeData)b).foregroundColor, t), elevation: Dart_uiLibrary.lerpDouble(((AppBarThemeData)a).elevation, ((AppBarThemeData)b).elevation, t), scrolledUnderElevation: Dart_uiLibrary.lerpDouble(((AppBarThemeData)a).scrolledUnderElevation, ((AppBarThemeData)b).scrolledUnderElevation, t), shadowColor: Dart_uiLibrary.Color.lerp(((AppBarThemeData)a).shadowColor, ((AppBarThemeData)b).shadowColor, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(((AppBarThemeData)a).surfaceTintColor, ((AppBarThemeData)b).surfaceTintColor, t), shape: ShapeBorder.lerp(((AppBarThemeData)a).shape, ((AppBarThemeData)b).shape, t), iconTheme: IconThemeData.lerp(((AppBarThemeData)a).iconTheme, ((AppBarThemeData)b).iconTheme, t), actionsIconTheme: IconThemeData.lerp(((AppBarThemeData)a).actionsIconTheme, ((AppBarThemeData)b).actionsIconTheme, t), centerTitle: ((t < 0.5) ? ((AppBarThemeData)a).centerTitle : ((AppBarThemeData)b).centerTitle), titleSpacing: Dart_uiLibrary.lerpDouble(((AppBarThemeData)a).titleSpacing, ((AppBarThemeData)b).titleSpacing, t), leadingWidth: Dart_uiLibrary.lerpDouble(((AppBarThemeData)a).leadingWidth, ((AppBarThemeData)b).leadingWidth, t), toolbarHeight: Dart_uiLibrary.lerpDouble(((AppBarThemeData)a).toolbarHeight, ((AppBarThemeData)b).toolbarHeight, t), toolbarTextStyle: TextStyle.lerp(((AppBarThemeData)a).toolbarTextStyle, ((AppBarThemeData)b).toolbarTextStyle, t), titleTextStyle: TextStyle.lerp(((AppBarThemeData)a).titleTextStyle, ((AppBarThemeData)b).titleTextStyle, t), systemOverlayStyle: ((t < 0.5) ? ((AppBarThemeData)a).systemOverlayStyle : ((AppBarThemeData)b).systemOverlayStyle), actionsPadding: EdgeInsetsGeometry.lerp(((AppBarThemeData)a).actionsPadding, ((AppBarThemeData)b).actionsPadding, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.backgroundColor, this.foregroundColor, this.elevation, this.scrolledUnderElevation, this.shadowColor, this.surfaceTintColor, this.shape, this.iconTheme, this.actionsIconTheme, this.centerTitle, this.titleSpacing, this.leadingWidth, this.toolbarHeight, this.toolbarTextStyle, this.titleTextStyle, this.systemOverlayStyle, this.actionsPadding));
    public override bool Equals(object? other)
    {
        var __other = other as AppBarThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((((((((((__other is AppBarThemeData) && (object.Equals(((AppBarThemeData)((AppBarThemeData)__other)).backgroundColor, this.backgroundColor))) && (object.Equals(((AppBarThemeData)((AppBarThemeData)__other)).foregroundColor, this.foregroundColor))) && (((AppBarThemeData)((AppBarThemeData)__other)).elevation == this.elevation)) && (((AppBarThemeData)((AppBarThemeData)__other)).scrolledUnderElevation == this.scrolledUnderElevation)) && (object.Equals(((AppBarThemeData)((AppBarThemeData)__other)).shadowColor, this.shadowColor))) && (object.Equals(((AppBarThemeData)((AppBarThemeData)__other)).surfaceTintColor, this.surfaceTintColor))) && (object.Equals(((AppBarThemeData)((AppBarThemeData)__other)).shape, this.shape))) && (object.Equals(((AppBarThemeData)((AppBarThemeData)__other)).iconTheme, this.iconTheme))) && (object.Equals(((AppBarThemeData)((AppBarThemeData)__other)).actionsIconTheme, this.actionsIconTheme))) && (((AppBarThemeData)((AppBarThemeData)__other)).centerTitle == this.centerTitle)) && (((AppBarThemeData)((AppBarThemeData)__other)).titleSpacing == this.titleSpacing)) && (((AppBarThemeData)((AppBarThemeData)__other)).leadingWidth == this.leadingWidth)) && (((AppBarThemeData)((AppBarThemeData)__other)).toolbarHeight == this.toolbarHeight)) && (object.Equals(((AppBarThemeData)((AppBarThemeData)__other)).toolbarTextStyle, this.toolbarTextStyle))) && (object.Equals(((AppBarThemeData)((AppBarThemeData)__other)).titleTextStyle, this.titleTextStyle))) && (object.Equals(((AppBarThemeData)((AppBarThemeData)__other)).systemOverlayStyle, this.systemOverlayStyle))) && (object.Equals(((AppBarThemeData)((AppBarThemeData)__other)).actionsPadding, this.actionsPadding)));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("backgroundColor", this.backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("foregroundColor", this.foregroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("scrolledUnderElevation", this.scrolledUnderElevation, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("shadowColor", this.shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("surfaceTintColor", this.surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ShapeBorder>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.IconThemeData>("iconTheme", this.iconTheme, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.IconThemeData>("actionsIconTheme", this.actionsIconTheme, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("centerTitle", this.centerTitle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("titleSpacing", this.titleSpacing, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("leadingWidth", this.leadingWidth, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("toolbarHeight", this.toolbarHeight, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("toolbarTextStyle", this.toolbarTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("titleTextStyle", this.titleTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Services.SystemUiOverlayStyle?>("systemOverlayStyle", this.systemOverlayStyle, defaultValue: null, description: ((this.systemOverlayStyle is null) ? null : $"SystemUiOverlayStyle({string.Join(", ", ((Func<List<string>>)(() => { var __collection24269 = new List<string>(); if ((this.systemOverlayStyle?.systemNavigationBarColor is not null)) { __collection24269.Add($"systemNavigationBarColor: {this.systemOverlayStyle?.systemNavigationBarColor}"); } if ((this.systemOverlayStyle?.systemNavigationBarDividerColor is not null)) { __collection24269.Add($"systemNavigationBarDividerColor: {this.systemOverlayStyle?.systemNavigationBarDividerColor}"); } if ((this.systemOverlayStyle?.systemNavigationBarIconBrightness is not null)) { __collection24269.Add($"systemNavigationBarIconBrightness: {this.systemOverlayStyle?.systemNavigationBarIconBrightness}"); } if ((this.systemOverlayStyle?.statusBarColor is not null)) { __collection24269.Add($"statusBarColor: {this.systemOverlayStyle?.statusBarColor}"); } if ((this.systemOverlayStyle?.statusBarBrightness is not null)) { __collection24269.Add($"statusBarBrightness: {this.systemOverlayStyle?.statusBarBrightness}"); } if ((this.systemOverlayStyle?.statusBarIconBrightness is not null)) { __collection24269.Add($"statusBarIconBrightness: {this.systemOverlayStyle?.statusBarIconBrightness}"); } if ((this.systemOverlayStyle?.systemStatusBarContrastEnforced is not null)) { __collection24269.Add($"systemStatusBarContrastEnforced: {this.systemOverlayStyle?.systemStatusBarContrastEnforced}"); } if ((this.systemOverlayStyle?.systemNavigationBarContrastEnforced is not null)) { __collection24269.Add($"systemNavigationBarContrastEnforced: {this.systemOverlayStyle?.systemNavigationBarContrastEnforced}"); } return __collection24269; }))().where(((s) => (s.Length != 0))))})")));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>("actionsPadding", this.actionsPadding, defaultValue: null));
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
