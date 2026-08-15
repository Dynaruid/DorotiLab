// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/list_tile_theme.dart
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

public class ListTileThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual bool? dense { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual ListTileStyle? style { get; private set; }
    public virtual Color? selectedColor { get; private set; }
    public virtual Color? iconColor { get; private set; }
    public virtual Color? textColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? subtitleTextStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? leadingAndTrailingTextStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? contentPadding { get; private set; }
    public virtual Color? tileColor { get; private set; }
    public virtual Color? selectedTileColor { get; private set; }
    public virtual double? horizontalTitleGap { get; private set; }
    public virtual double? minVerticalPadding { get; private set; }
    public virtual double? minLeadingWidth { get; private set; }
    public virtual double? minTileHeight { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual ListTileTitleAlignment? titleAlignment { get; private set; }
    public virtual ListTileControlAffinity? controlAffinity { get; private set; }
    public virtual bool? isThreeLine { get; private set; }

    public ListTileThemeData(bool? dense = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, ListTileStyle? style = null, Color? selectedColor = null, Color? iconColor = null, Color? textColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? subtitleTextStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? leadingAndTrailingTextStyle = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, Color? tileColor = null, Color? selectedTileColor = null, double? horizontalTitleGap = null, double? minVerticalPadding = null, double? minLeadingWidth = null, bool? enableFeedback = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor = null, VisualDensity? visualDensity = null, double? minTileHeight = null, ListTileTitleAlignment? titleAlignment = null, ListTileControlAffinity? controlAffinity = null, bool? isThreeLine = null)
    {
        this.dense = dense;
        this.shape = shape;
        this.style = style;
        this.selectedColor = selectedColor;
        this.iconColor = iconColor;
        this.textColor = textColor;
        this.titleTextStyle = titleTextStyle;
        this.subtitleTextStyle = subtitleTextStyle;
        this.leadingAndTrailingTextStyle = leadingAndTrailingTextStyle;
        this.contentPadding = contentPadding;
        this.tileColor = tileColor;
        this.selectedTileColor = selectedTileColor;
        this.horizontalTitleGap = horizontalTitleGap;
        this.minVerticalPadding = minVerticalPadding;
        this.minLeadingWidth = minLeadingWidth;
        this.enableFeedback = enableFeedback;
        this.mouseCursor = mouseCursor;
        this.visualDensity = visualDensity;
        this.minTileHeight = minTileHeight;
        this.titleAlignment = titleAlignment;
        this.controlAffinity = controlAffinity;
        this.isThreeLine = isThreeLine;
    }

    public virtual ListTileThemeData copyWith(bool? dense = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, ListTileStyle? style = null, Color? selectedColor = null, Color? iconColor = null, Color? textColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? subtitleTextStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? leadingAndTrailingTextStyle = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, Color? tileColor = null, Color? selectedTileColor = null, double? horizontalTitleGap = null, double? minVerticalPadding = null, double? minLeadingWidth = null, double? minTileHeight = null, bool? enableFeedback = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor = null, bool? isThreeLine = null, VisualDensity? visualDensity = null, ListTileTitleAlignment? titleAlignment = null, ListTileControlAffinity? controlAffinity = null)
    {
        return new ListTileThemeData(dense: (dense ?? this.dense), shape: (shape ?? this.shape), style: (style ?? this.style), selectedColor: (selectedColor ?? this.selectedColor), iconColor: (iconColor ?? this.iconColor), textColor: (textColor ?? this.textColor), titleTextStyle: (titleTextStyle ?? this.titleTextStyle), subtitleTextStyle: (subtitleTextStyle ?? this.subtitleTextStyle), leadingAndTrailingTextStyle: (leadingAndTrailingTextStyle ?? this.leadingAndTrailingTextStyle), contentPadding: (contentPadding ?? this.contentPadding), tileColor: (tileColor ?? this.tileColor), selectedTileColor: (selectedTileColor ?? this.selectedTileColor), horizontalTitleGap: (horizontalTitleGap ?? this.horizontalTitleGap), minVerticalPadding: (minVerticalPadding ?? this.minVerticalPadding), minLeadingWidth: (minLeadingWidth ?? this.minLeadingWidth), minTileHeight: (minTileHeight ?? this.minTileHeight), enableFeedback: (enableFeedback ?? this.enableFeedback), mouseCursor: (mouseCursor ?? this.mouseCursor), visualDensity: (visualDensity ?? this.visualDensity), titleAlignment: (titleAlignment ?? this.titleAlignment), controlAffinity: (controlAffinity ?? this.controlAffinity), isThreeLine: (isThreeLine ?? this.isThreeLine));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ListTileThemeData? lerp(ListTileThemeData? a, ListTileThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new ListTileThemeData(dense: ((t < 0.5) ? a?.dense : b?.dense), shape: ShapeBorder.lerp(a?.shape, b?.shape, t), style: ((t < 0.5) ? a?.style : b?.style), selectedColor: Dart_uiLibrary.Color.lerp(a?.selectedColor, b?.selectedColor, t), iconColor: Dart_uiLibrary.Color.lerp(a?.iconColor, b?.iconColor, t), textColor: Dart_uiLibrary.Color.lerp(a?.textColor, b?.textColor, t), titleTextStyle: TextStyle.lerp(a?.titleTextStyle, b?.titleTextStyle, t), subtitleTextStyle: TextStyle.lerp(a?.subtitleTextStyle, b?.subtitleTextStyle, t), leadingAndTrailingTextStyle: TextStyle.lerp(a?.leadingAndTrailingTextStyle, b?.leadingAndTrailingTextStyle, t), contentPadding: EdgeInsetsGeometry.lerp(a?.contentPadding, b?.contentPadding, t), tileColor: Dart_uiLibrary.Color.lerp(a?.tileColor, b?.tileColor, t), selectedTileColor: Dart_uiLibrary.Color.lerp(a?.selectedTileColor, b?.selectedTileColor, t), horizontalTitleGap: Dart_uiLibrary.lerpDouble(a?.horizontalTitleGap, b?.horizontalTitleGap, t), minVerticalPadding: Dart_uiLibrary.lerpDouble(a?.minVerticalPadding, b?.minVerticalPadding, t), minLeadingWidth: Dart_uiLibrary.lerpDouble(a?.minLeadingWidth, b?.minLeadingWidth, t), minTileHeight: Dart_uiLibrary.lerpDouble(a?.minTileHeight, b?.minTileHeight, t), enableFeedback: ((t < 0.5) ? a?.enableFeedback : b?.enableFeedback), mouseCursor: ((t < 0.5) ? a?.mouseCursor : b?.mouseCursor), visualDensity: ((t < 0.5) ? a?.visualDensity : b?.visualDensity), titleAlignment: ((t < 0.5) ? a?.titleAlignment : b?.titleAlignment), controlAffinity: ((t < 0.5) ? a?.controlAffinity : b?.controlAffinity), isThreeLine: ((t < 0.5) ? a?.isThreeLine : b?.isThreeLine));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHashAll(new List<object?> { this.dense, this.shape, this.style, this.selectedColor, this.iconColor, this.textColor, this.titleTextStyle, this.subtitleTextStyle, this.leadingAndTrailingTextStyle, this.contentPadding, this.tileColor, this.selectedTileColor, this.horizontalTitleGap, this.minVerticalPadding, this.minLeadingWidth, this.minTileHeight, this.enableFeedback, this.mouseCursor, this.visualDensity, this.titleAlignment, this.controlAffinity, this.isThreeLine }));
    public override bool Equals(object? other)
    {
        var __other = other as ListTileThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((((((((((((((((__other is ListTileThemeData) && (((ListTileThemeData)((ListTileThemeData)__other)).dense == this.dense)) && (object.Equals(((ListTileThemeData)((ListTileThemeData)__other)).shape, this.shape))) && (object.Equals(((ListTileThemeData)((ListTileThemeData)__other)).style, this.style))) && (object.Equals(((ListTileThemeData)((ListTileThemeData)__other)).selectedColor, this.selectedColor))) && (object.Equals(((ListTileThemeData)((ListTileThemeData)__other)).iconColor, this.iconColor))) && (object.Equals(((ListTileThemeData)((ListTileThemeData)__other)).titleTextStyle, this.titleTextStyle))) && (object.Equals(((ListTileThemeData)((ListTileThemeData)__other)).subtitleTextStyle, this.subtitleTextStyle))) && (object.Equals(((ListTileThemeData)((ListTileThemeData)__other)).leadingAndTrailingTextStyle, this.leadingAndTrailingTextStyle))) && (object.Equals(((ListTileThemeData)((ListTileThemeData)__other)).textColor, this.textColor))) && (object.Equals(((ListTileThemeData)((ListTileThemeData)__other)).contentPadding, this.contentPadding))) && (object.Equals(((ListTileThemeData)((ListTileThemeData)__other)).tileColor, this.tileColor))) && (object.Equals(((ListTileThemeData)((ListTileThemeData)__other)).selectedTileColor, this.selectedTileColor))) && (((ListTileThemeData)((ListTileThemeData)__other)).horizontalTitleGap == this.horizontalTitleGap)) && (((ListTileThemeData)((ListTileThemeData)__other)).minVerticalPadding == this.minVerticalPadding)) && (((ListTileThemeData)((ListTileThemeData)__other)).minLeadingWidth == this.minLeadingWidth)) && (((ListTileThemeData)((ListTileThemeData)__other)).minTileHeight == this.minTileHeight)) && (((ListTileThemeData)((ListTileThemeData)__other)).enableFeedback == this.enableFeedback)) && (object.Equals(((ListTileThemeData)((ListTileThemeData)__other)).mouseCursor, this.mouseCursor))) && (object.Equals(((ListTileThemeData)((ListTileThemeData)__other)).visualDensity, this.visualDensity))) && (object.Equals(((ListTileThemeData)((ListTileThemeData)__other)).titleAlignment, this.titleAlignment))) && (object.Equals(((ListTileThemeData)((ListTileThemeData)__other)).controlAffinity, this.controlAffinity))) && (((ListTileThemeData)((ListTileThemeData)__other)).isThreeLine == this.isThreeLine));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("dense", this.dense, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ShapeBorder>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<ListTileStyle>("style", this.style, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("selectedColor", this.selectedColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("iconColor", this.iconColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("textColor", this.textColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("titleTextStyle", this.titleTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("subtitleTextStyle", this.subtitleTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("leadingAndTrailingTextStyle", this.leadingAndTrailingTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("contentPadding", this.contentPadding, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("tileColor", this.tileColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("selectedTileColor", this.selectedTileColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("horizontalTitleGap", this.horizontalTitleGap, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("minVerticalPadding", this.minVerticalPadding, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("minLeadingWidth", this.minLeadingWidth, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("minTileHeight", this.minTileHeight, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("enableFeedback", this.enableFeedback, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>>("mouseCursor", this.mouseCursor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<VisualDensity>("visualDensity", this.visualDensity, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<ListTileTitleAlignment>("titleAlignment", this.titleAlignment, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<ListTileControlAffinity>("controlAffinity", this.controlAffinity, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("isThreeLine", this.isThreeLine, defaultValue: null));
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

public class ListTileTheme : global::Doroti.Generated.Framework.Widgets.InheritedTheme
{
    internal virtual ListTileThemeData? _data { get; private set; }
    internal virtual bool? _dense { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? _shape { get; private set; }
    internal virtual ListTileStyle? _style { get; private set; }
    internal virtual Color? _selectedColor { get; private set; }
    internal virtual Color? _iconColor { get; private set; }
    internal virtual Color? _textColor { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? _contentPadding { get; private set; }
    internal virtual Color? _tileColor { get; private set; }
    internal virtual Color? _selectedTileColor { get; private set; }
    internal virtual double? _horizontalTitleGap { get; private set; }
    internal virtual double? _minVerticalPadding { get; private set; }
    internal virtual double? _minLeadingWidth { get; private set; }
    internal virtual bool? _enableFeedback { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? _mouseCursor { get; private set; }
    internal virtual ListTileControlAffinity? _controlAffinity { get; private set; }

    public ListTileTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, ListTileThemeData? data = null, bool? dense = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, ListTileStyle? style = null, Color? selectedColor = null, Color? iconColor = null, Color? textColor = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, Color? tileColor = null, Color? selectedTileColor = null, bool? enableFeedback = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor = null, double? horizontalTitleGap = null, double? minVerticalPadding = null, double? minLeadingWidth = null, ListTileControlAffinity? controlAffinity = null, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this._data = data;
        this._dense = dense;
        this._shape = shape;
        this._style = style;
        this._selectedColor = selectedColor;
        this._iconColor = iconColor;
        this._textColor = textColor;
        this._contentPadding = contentPadding;
        this._tileColor = tileColor;
        this._selectedTileColor = selectedTileColor;
        this._enableFeedback = enableFeedback;
        this._mouseCursor = mouseCursor;
        this._horizontalTitleGap = horizontalTitleGap;
        this._minVerticalPadding = minVerticalPadding;
        this._minLeadingWidth = minLeadingWidth;
        this._controlAffinity = controlAffinity;
        System.Diagnostics.Debug.Assert(((data is null) || (((((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)shape ?? (object?)selectedColor)) ?? (object?)iconColor)) ?? (object?)textColor)) ?? (object?)contentPadding)) ?? (object?)tileColor)) ?? (object?)selectedTileColor)) ?? (object?)enableFeedback)) ?? (object?)mouseCursor)) ?? (object?)horizontalTitleGap)) ?? (object?)minVerticalPadding)) ?? (object?)minLeadingWidth)) ?? (object?)controlAffinity))) is null)));
    }

    public virtual ListTileThemeData data
    {
        get
        {
            return (this._data ?? new ListTileThemeData(dense: this._dense, shape: this._shape, style: this._style, selectedColor: this._selectedColor, iconColor: this._iconColor, textColor: this._textColor, contentPadding: this._contentPadding, tileColor: this._tileColor, selectedTileColor: this._selectedTileColor, enableFeedback: this._enableFeedback, mouseCursor: this._mouseCursor, horizontalTitleGap: this._horizontalTitleGap, minVerticalPadding: this._minVerticalPadding, minLeadingWidth: this._minLeadingWidth, controlAffinity: this._controlAffinity));
            return default!;
        }
    }
    public virtual bool? dense => ((this._data is not null) ? ((ListTileThemeData)this._data).dense : this._dense);
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape => ((this._data is not null) ? ((ListTileThemeData)this._data).shape : this._shape);
    public virtual ListTileStyle? style => ((this._data is not null) ? ((ListTileThemeData)this._data).style : this._style);
    public virtual global::Doroti.Ui.Color? selectedColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((ListTileThemeData)this._data).selectedColor : this._selectedColor));
    public virtual global::Doroti.Ui.Color? iconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((ListTileThemeData)this._data).iconColor : this._iconColor));
    public virtual global::Doroti.Ui.Color? textColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((ListTileThemeData)this._data).textColor : this._textColor));
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? contentPadding => ((this._data is not null) ? ((ListTileThemeData)this._data).contentPadding : this._contentPadding);
    public virtual global::Doroti.Ui.Color? tileColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((ListTileThemeData)this._data).tileColor : this._tileColor));
    public virtual global::Doroti.Ui.Color? selectedTileColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((ListTileThemeData)this._data).selectedTileColor : this._selectedTileColor));
    public virtual double? horizontalTitleGap => ((this._data is not null) ? ((ListTileThemeData)this._data).horizontalTitleGap : this._horizontalTitleGap);
    public virtual double? minVerticalPadding => ((this._data is not null) ? ((ListTileThemeData)this._data).minVerticalPadding : this._minVerticalPadding);
    public virtual double? minLeadingWidth => ((this._data is not null) ? ((ListTileThemeData)this._data).minLeadingWidth : this._minLeadingWidth);
    public virtual bool? enableFeedback => ((this._data is not null) ? ((ListTileThemeData)this._data).enableFeedback : this._enableFeedback);
    public virtual ListTileControlAffinity? controlAffinity => ((this._data is not null) ? ((ListTileThemeData)this._data).controlAffinity : this._controlAffinity);
    public static ListTileThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ListTileTheme? result__20973 = ((ListTileTheme?)(object?)context.dependOnInheritedWidgetOfExactType<ListTileTheme>());
        return (result__20973?.data ?? Theme.of(context).listTileTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Generated.Framework.Widgets.Widget merge(global::Doroti.Generated.Framework.Foundation.Key? key = null, bool? dense = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, ListTileStyle? style = null, Color? selectedColor = null, Color? iconColor = null, Color? textColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? subtitleTextStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? leadingAndTrailingTextStyle = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, Color? tileColor = null, Color? selectedTileColor = null, bool? enableFeedback = null, double? horizontalTitleGap = null, double? minVerticalPadding = null, double? minLeadingWidth = null, double? minTileHeight = null, ListTileTitleAlignment? titleAlignment = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor = null, VisualDensity? visualDensity = null, ListTileControlAffinity? controlAffinity = null, bool? isThreeLine = null, global::Doroti.Generated.Framework.Widgets.Widget child = default!)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Builder(builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>)((context) => {
ListTileThemeData parent__22106 = ((ListTileThemeData)(object?)ListTileTheme.of(context));
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new ListTileTheme(key: key, data: new ListTileThemeData(dense: (dense ?? ((ListTileThemeData)parent__22106).dense), shape: (shape ?? ((ListTileThemeData)parent__22106).shape), style: (style ?? ((ListTileThemeData)parent__22106).style), selectedColor: (selectedColor ?? ((ListTileThemeData)parent__22106).selectedColor), iconColor: (iconColor ?? ((ListTileThemeData)parent__22106).iconColor), textColor: (textColor ?? ((ListTileThemeData)parent__22106).textColor), titleTextStyle: (titleTextStyle ?? ((ListTileThemeData)parent__22106).titleTextStyle), subtitleTextStyle: (subtitleTextStyle ?? ((ListTileThemeData)parent__22106).subtitleTextStyle), leadingAndTrailingTextStyle: (leadingAndTrailingTextStyle ?? ((ListTileThemeData)parent__22106).leadingAndTrailingTextStyle), contentPadding: (contentPadding ?? ((ListTileThemeData)parent__22106).contentPadding), tileColor: (tileColor ?? ((ListTileThemeData)parent__22106).tileColor), selectedTileColor: (selectedTileColor ?? ((ListTileThemeData)parent__22106).selectedTileColor), enableFeedback: (enableFeedback ?? ((ListTileThemeData)parent__22106).enableFeedback), horizontalTitleGap: (horizontalTitleGap ?? ((ListTileThemeData)parent__22106).horizontalTitleGap), minVerticalPadding: (minVerticalPadding ?? ((ListTileThemeData)parent__22106).minVerticalPadding), minLeadingWidth: (minLeadingWidth ?? ((ListTileThemeData)parent__22106).minLeadingWidth), minTileHeight: (minTileHeight ?? ((ListTileThemeData)parent__22106).minTileHeight), titleAlignment: (titleAlignment ?? ((ListTileThemeData)parent__22106).titleAlignment), mouseCursor: (mouseCursor ?? ((ListTileThemeData)parent__22106).mouseCursor), visualDensity: (visualDensity ?? ((ListTileThemeData)parent__22106).visualDensity), controlAffinity: (controlAffinity ?? ((ListTileThemeData)parent__22106).controlAffinity), isThreeLine: (isThreeLine ?? ((ListTileThemeData)parent__22106).isThreeLine)), child: child));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget wrap(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new ListTileTheme(data: new ListTileThemeData(dense: this.dense, shape: this.shape, style: this.style, selectedColor: this.selectedColor, iconColor: this.iconColor, textColor: this.textColor, contentPadding: this.contentPadding, tileColor: this.tileColor, selectedTileColor: this.selectedTileColor, enableFeedback: this.enableFeedback, horizontalTitleGap: this.horizontalTitleGap, minVerticalPadding: this.minVerticalPadding, minLeadingWidth: this.minLeadingWidth, isThreeLine: this._data?.isThreeLine), child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((ListTileTheme)oldWidget).data)));
}
