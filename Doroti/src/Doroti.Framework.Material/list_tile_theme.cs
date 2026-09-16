// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/list_tile_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class ListTileThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual bool? dense { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual ListTileStyle? style { get; private set; }
    public virtual Color? selectedColor { get; private set; }
    public virtual Color? iconColor { get; private set; }
    public virtual Color? textColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? titleTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? subtitleTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? leadingAndTrailingTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding { get; private set; }
    public virtual Color? tileColor { get; private set; }
    public virtual Color? selectedTileColor { get; private set; }
    public virtual double? horizontalTitleGap { get; private set; }
    public virtual double? minVerticalPadding { get; private set; }
    public virtual double? minLeadingWidth { get; private set; }
    public virtual double? minTileHeight { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual ListTileTitleAlignment? titleAlignment { get; private set; }
    public virtual ListTileControlAffinity? controlAffinity { get; private set; }
    public virtual bool? isThreeLine { get; private set; }

    public ListTileThemeData(bool? dense = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, ListTileStyle? style = null, Color? selectedColor = null, Color? iconColor = null, Color? textColor = null, global::Doroti.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Framework.Painting.TextStyle? subtitleTextStyle = null, global::Doroti.Framework.Painting.TextStyle? leadingAndTrailingTextStyle = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, Color? tileColor = null, Color? selectedTileColor = null, double? horizontalTitleGap = null, double? minVerticalPadding = null, double? minLeadingWidth = null, bool? enableFeedback = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null, VisualDensity? visualDensity = null, double? minTileHeight = null, ListTileTitleAlignment? titleAlignment = null, ListTileControlAffinity? controlAffinity = null, bool? isThreeLine = null)
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

    public virtual ListTileThemeData copyWith(bool? dense = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, ListTileStyle? style = null, Color? selectedColor = null, Color? iconColor = null, Color? textColor = null, global::Doroti.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Framework.Painting.TextStyle? subtitleTextStyle = null, global::Doroti.Framework.Painting.TextStyle? leadingAndTrailingTextStyle = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, Color? tileColor = null, Color? selectedTileColor = null, double? horizontalTitleGap = null, double? minVerticalPadding = null, double? minLeadingWidth = null, double? minTileHeight = null, bool? enableFeedback = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null, bool? isThreeLine = null, VisualDensity? visualDensity = null, ListTileTitleAlignment? titleAlignment = null, ListTileControlAffinity? controlAffinity = null)
    {
        return new ListTileThemeData(dense: dense ?? this.dense, shape: shape ?? this.shape, style: style ?? this.style, selectedColor: selectedColor ?? this.selectedColor, iconColor: iconColor ?? this.iconColor, textColor: textColor ?? this.textColor, titleTextStyle: titleTextStyle ?? this.titleTextStyle, subtitleTextStyle: subtitleTextStyle ?? this.subtitleTextStyle, leadingAndTrailingTextStyle: leadingAndTrailingTextStyle ?? this.leadingAndTrailingTextStyle, contentPadding: contentPadding ?? this.contentPadding, tileColor: tileColor ?? this.tileColor, selectedTileColor: selectedTileColor ?? this.selectedTileColor, horizontalTitleGap: horizontalTitleGap ?? this.horizontalTitleGap, minVerticalPadding: minVerticalPadding ?? this.minVerticalPadding, minLeadingWidth: minLeadingWidth ?? this.minLeadingWidth, minTileHeight: minTileHeight ?? this.minTileHeight, enableFeedback: enableFeedback ?? this.enableFeedback, mouseCursor: mouseCursor ?? this.mouseCursor, visualDensity: visualDensity ?? this.visualDensity, titleAlignment: titleAlignment ?? this.titleAlignment, controlAffinity: controlAffinity ?? this.controlAffinity, isThreeLine: isThreeLine ?? this.isThreeLine);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ListTileThemeData? lerp(ListTileThemeData? a, ListTileThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new ListTileThemeData(dense: (t < 0.5) ? a?.dense : b?.dense, shape: ShapeBorder.lerp(a?.shape, b?.shape, t), style: (t < 0.5) ? a?.style : b?.style, selectedColor: Dart_uiLibrary.Color.lerp(a?.selectedColor, b?.selectedColor, t), iconColor: Dart_uiLibrary.Color.lerp(a?.iconColor, b?.iconColor, t), textColor: Dart_uiLibrary.Color.lerp(a?.textColor, b?.textColor, t), titleTextStyle: TextStyle.lerp(a?.titleTextStyle, b?.titleTextStyle, t), subtitleTextStyle: TextStyle.lerp(a?.subtitleTextStyle, b?.subtitleTextStyle, t), leadingAndTrailingTextStyle: TextStyle.lerp(a?.leadingAndTrailingTextStyle, b?.leadingAndTrailingTextStyle, t), contentPadding: EdgeInsetsGeometry.lerp(a?.contentPadding, b?.contentPadding, t), tileColor: Dart_uiLibrary.Color.lerp(a?.tileColor, b?.tileColor, t), selectedTileColor: Dart_uiLibrary.Color.lerp(a?.selectedTileColor, b?.selectedTileColor, t), horizontalTitleGap: Dart_uiLibrary.lerpDouble(a?.horizontalTitleGap, b?.horizontalTitleGap, t), minVerticalPadding: Dart_uiLibrary.lerpDouble(a?.minVerticalPadding, b?.minVerticalPadding, t), minLeadingWidth: Dart_uiLibrary.lerpDouble(a?.minLeadingWidth, b?.minLeadingWidth, t), minTileHeight: Dart_uiLibrary.lerpDouble(a?.minTileHeight, b?.minTileHeight, t), enableFeedback: (t < 0.5) ? a?.enableFeedback : b?.enableFeedback, mouseCursor: (t < 0.5) ? a?.mouseCursor : b?.mouseCursor, visualDensity: (t < 0.5) ? a?.visualDensity : b?.visualDensity, titleAlignment: (t < 0.5) ? a?.titleAlignment : b?.titleAlignment, controlAffinity: (t < 0.5) ? a?.controlAffinity : b?.controlAffinity, isThreeLine: (t < 0.5) ? a?.isThreeLine : b?.isThreeLine);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHashAll(new List<object?> { dense, shape, style, selectedColor, iconColor, textColor, titleTextStyle, subtitleTextStyle, leadingAndTrailingTextStyle, contentPadding, tileColor, selectedTileColor, horizontalTitleGap, minVerticalPadding, minLeadingWidth, minTileHeight, enableFeedback, mouseCursor, visualDensity, titleAlignment, controlAffinity, isThreeLine }));
    public override bool Equals(object? other)
    {
        var __other = other as ListTileThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ListTileThemeData) && (__other.dense == dense) && Equals(__other.shape, shape) && Equals(__other.style, style) && Equals(__other.selectedColor, selectedColor) && Equals(__other.iconColor, iconColor) && Equals(__other.titleTextStyle, titleTextStyle) && Equals(__other.subtitleTextStyle, subtitleTextStyle) && Equals(__other.leadingAndTrailingTextStyle, leadingAndTrailingTextStyle) && Equals(__other.textColor, textColor) && Equals(__other.contentPadding, contentPadding) && Equals(__other.tileColor, tileColor) && Equals(__other.selectedTileColor, selectedTileColor) && (__other.horizontalTitleGap == horizontalTitleGap) && (__other.minVerticalPadding == minVerticalPadding) && (__other.minLeadingWidth == minLeadingWidth) && (__other.minTileHeight == minTileHeight) && (__other.enableFeedback == enableFeedback) && Equals(__other.mouseCursor, mouseCursor) && Equals(__other.visualDensity, visualDensity) && Equals(__other.titleAlignment, titleAlignment) && Equals(__other.controlAffinity, controlAffinity) && (__other.isThreeLine == isThreeLine);
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("dense", dense, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("shape", shape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<ListTileStyle>("style", style, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("selectedColor", selectedColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("iconColor", iconColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("textColor", textColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("titleTextStyle", titleTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("subtitleTextStyle", subtitleTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("leadingAndTrailingTextStyle", leadingAndTrailingTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("contentPadding", contentPadding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("tileColor", tileColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("selectedTileColor", selectedTileColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("horizontalTitleGap", horizontalTitleGap, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("minVerticalPadding", minVerticalPadding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("minLeadingWidth", minLeadingWidth, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("minTileHeight", minTileHeight, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("enableFeedback", enableFeedback, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>>("mouseCursor", mouseCursor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<VisualDensity>("visualDensity", visualDensity, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ListTileTitleAlignment>("titleAlignment", titleAlignment, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ListTileControlAffinity>("controlAffinity", controlAffinity, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("isThreeLine", isThreeLine, defaultValue: null));
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

public class ListTileTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    internal virtual ListTileThemeData? _data { get; private set; }
    internal virtual bool? _dense { get; private set; }
    internal virtual global::Doroti.Framework.Painting.ShapeBorder? _shape { get; private set; }
    internal virtual ListTileStyle? _style { get; private set; }
    internal virtual Color? _selectedColor { get; private set; }
    internal virtual Color? _iconColor { get; private set; }
    internal virtual Color? _textColor { get; private set; }
    internal virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? _contentPadding { get; private set; }
    internal virtual Color? _tileColor { get; private set; }
    internal virtual Color? _selectedTileColor { get; private set; }
    internal virtual double? _horizontalTitleGap { get; private set; }
    internal virtual double? _minVerticalPadding { get; private set; }
    internal virtual double? _minLeadingWidth { get; private set; }
    internal virtual bool? _enableFeedback { get; private set; }
    internal virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? _mouseCursor { get; private set; }
    internal virtual ListTileControlAffinity? _controlAffinity { get; private set; }

    public ListTileTheme(global::Doroti.Framework.Foundation.Key? key = null, ListTileThemeData? data = null, bool? dense = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, ListTileStyle? style = null, Color? selectedColor = null, Color? iconColor = null, Color? textColor = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, Color? tileColor = null, Color? selectedTileColor = null, bool? enableFeedback = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null, double? horizontalTitleGap = null, double? minVerticalPadding = null, double? minLeadingWidth = null, ListTileControlAffinity? controlAffinity = null, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        _data = data;
        _dense = dense;
        _shape = shape;
        _style = style;
        _selectedColor = selectedColor;
        _iconColor = iconColor;
        _textColor = textColor;
        _contentPadding = contentPadding;
        _tileColor = tileColor;
        _selectedTileColor = selectedTileColor;
        _enableFeedback = enableFeedback;
        _mouseCursor = mouseCursor;
        _horizontalTitleGap = horizontalTitleGap;
        _minVerticalPadding = minVerticalPadding;
        _minLeadingWidth = minLeadingWidth;
        _controlAffinity = controlAffinity;
        System.Diagnostics.Debug.Assert((data is null) || ((((((((((((((object?)shape ?? selectedColor) ?? iconColor) ?? textColor) ?? contentPadding) ?? tileColor) ?? selectedTileColor) ?? enableFeedback) ?? mouseCursor) ?? horizontalTitleGap) ?? minVerticalPadding) ?? minLeadingWidth) ?? controlAffinity) is null));
    }

    public virtual ListTileThemeData data
    {
        get
        {
            return _data ?? new ListTileThemeData(dense: _dense, shape: _shape, style: _style, selectedColor: _selectedColor, iconColor: _iconColor, textColor: _textColor, contentPadding: _contentPadding, tileColor: _tileColor, selectedTileColor: _selectedTileColor, enableFeedback: _enableFeedback, mouseCursor: _mouseCursor, horizontalTitleGap: _horizontalTitleGap, minVerticalPadding: _minVerticalPadding, minLeadingWidth: _minLeadingWidth, controlAffinity: _controlAffinity);
        }
    }
    public virtual bool? dense => (_data is not null) ? _data.dense : _dense;
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape => (_data is not null) ? _data.shape : _shape;
    public virtual ListTileStyle? style => (_data is not null) ? _data.style : _style;
    public virtual global::Doroti.Ui.Color? selectedColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((_data is not null) ? _data.selectedColor : _selectedColor);
    public virtual global::Doroti.Ui.Color? iconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((_data is not null) ? _data.iconColor : _iconColor);
    public virtual global::Doroti.Ui.Color? textColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((_data is not null) ? _data.textColor : _textColor);
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding => (_data is not null) ? _data.contentPadding : _contentPadding;
    public virtual global::Doroti.Ui.Color? tileColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((_data is not null) ? _data.tileColor : _tileColor);
    public virtual global::Doroti.Ui.Color? selectedTileColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((_data is not null) ? _data.selectedTileColor : _selectedTileColor);
    public virtual double? horizontalTitleGap => (_data is not null) ? _data.horizontalTitleGap : _horizontalTitleGap;
    public virtual double? minVerticalPadding => (_data is not null) ? _data.minVerticalPadding : _minVerticalPadding;
    public virtual double? minLeadingWidth => (_data is not null) ? _data.minLeadingWidth : _minLeadingWidth;
    public virtual bool? enableFeedback => (_data is not null) ? _data.enableFeedback : _enableFeedback;
    public virtual ListTileControlAffinity? controlAffinity => (_data is not null) ? _data.controlAffinity : _controlAffinity;
    public static ListTileThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ListTileTheme? result = context.dependOnInheritedWidgetOfExactType<ListTileTheme>();
        return result?.data ?? Theme.of(context).listTileTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Widgets.Widget merge(global::Doroti.Framework.Foundation.Key? key = null, bool? dense = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, ListTileStyle? style = null, Color? selectedColor = null, Color? iconColor = null, Color? textColor = null, global::Doroti.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Framework.Painting.TextStyle? subtitleTextStyle = null, global::Doroti.Framework.Painting.TextStyle? leadingAndTrailingTextStyle = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, Color? tileColor = null, Color? selectedTileColor = null, bool? enableFeedback = null, double? horizontalTitleGap = null, double? minVerticalPadding = null, double? minLeadingWidth = null, double? minTileHeight = null, ListTileTitleAlignment? titleAlignment = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null, VisualDensity? visualDensity = null, ListTileControlAffinity? controlAffinity = null, bool? isThreeLine = null, global::Doroti.Framework.Widgets.Widget child = default!)
    {
        return new global::Doroti.Framework.Widgets.Builder(builder: (context) =>
        {
            ListTileThemeData parent = of(context);
            return new ListTileTheme(key: key, data: new ListTileThemeData(dense: dense ?? parent.dense, shape: shape ?? parent.shape, style: style ?? parent.style, selectedColor: selectedColor ?? parent.selectedColor, iconColor: iconColor ?? parent.iconColor, textColor: textColor ?? parent.textColor, titleTextStyle: titleTextStyle ?? parent.titleTextStyle, subtitleTextStyle: subtitleTextStyle ?? parent.subtitleTextStyle, leadingAndTrailingTextStyle: leadingAndTrailingTextStyle ?? parent.leadingAndTrailingTextStyle, contentPadding: contentPadding ?? parent.contentPadding, tileColor: tileColor ?? parent.tileColor, selectedTileColor: selectedTileColor ?? parent.selectedTileColor, enableFeedback: enableFeedback ?? parent.enableFeedback, horizontalTitleGap: horizontalTitleGap ?? parent.horizontalTitleGap, minVerticalPadding: minVerticalPadding ?? parent.minVerticalPadding, minLeadingWidth: minLeadingWidth ?? parent.minLeadingWidth, minTileHeight: minTileHeight ?? parent.minTileHeight, titleAlignment: titleAlignment ?? parent.titleAlignment, mouseCursor: mouseCursor ?? parent.mouseCursor, visualDensity: visualDensity ?? parent.visualDensity, controlAffinity: controlAffinity ?? parent.controlAffinity, isThreeLine: isThreeLine ?? parent.isThreeLine), child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return new ListTileTheme(data: new ListTileThemeData(dense: dense, shape: shape, style: style, selectedColor: selectedColor, iconColor: iconColor, textColor: textColor, contentPadding: contentPadding, tileColor: tileColor, selectedTileColor: selectedTileColor, enableFeedback: enableFeedback, horizontalTitleGap: horizontalTitleGap, minVerticalPadding: minVerticalPadding, minLeadingWidth: minLeadingWidth, isThreeLine: _data?.isThreeLine), child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((ListTileTheme)oldWidget).data));
}
