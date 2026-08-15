// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/data_table_theme.dart
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

public class DataTableThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual global::Doroti.Generated.Framework.Painting.Decoration? decoration { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? dataRowColor { get; private set; }
    public virtual double? dataRowMinHeight { get; private set; }
    public virtual double? dataRowMaxHeight { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? dataTextStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? headingRowColor { get; private set; }
    public virtual double? headingRowHeight { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? headingTextStyle { get; private set; }
    public virtual double? horizontalMargin { get; private set; }
    public virtual double? columnSpacing { get; private set; }
    public virtual double? dividerThickness { get; private set; }
    public virtual double? checkboxHorizontalMargin { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? headingCellCursor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? dataRowCursor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.MainAxisAlignment? headingRowAlignment { get; private set; }

    public DataTableThemeData(global::Doroti.Generated.Framework.Painting.Decoration? decoration = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? dataRowColor = null, double? dataRowHeight = null, double? dataRowMinHeight = null, double? dataRowMaxHeight = null, global::Doroti.Generated.Framework.Painting.TextStyle? dataTextStyle = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? headingRowColor = null, double? headingRowHeight = null, global::Doroti.Generated.Framework.Painting.TextStyle? headingTextStyle = null, double? horizontalMargin = null, double? columnSpacing = null, double? dividerThickness = null, double? checkboxHorizontalMargin = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? headingCellCursor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? dataRowCursor = null, global::Doroti.Generated.Framework.Rendering.MainAxisAlignment? headingRowAlignment = null)
    {
        this.decoration = decoration;
        this.dataRowColor = dataRowColor;
        this.dataTextStyle = dataTextStyle;
        this.headingRowColor = headingRowColor;
        this.headingRowHeight = headingRowHeight;
        this.headingTextStyle = headingTextStyle;
        this.horizontalMargin = horizontalMargin;
        this.columnSpacing = columnSpacing;
        this.dividerThickness = dividerThickness;
        this.checkboxHorizontalMargin = checkboxHorizontalMargin;
        this.headingCellCursor = headingCellCursor;
        this.dataRowCursor = dataRowCursor;
        this.headingRowAlignment = headingRowAlignment;
        this.dataRowMinHeight = (dataRowHeight ?? dataRowMinHeight);
        this.dataRowMaxHeight = (dataRowHeight ?? dataRowMaxHeight);
        System.Diagnostics.Debug.Assert((((dataRowMinHeight is null) || (dataRowMaxHeight is null)) || (dataRowMaxHeight >= DartRuntimePrimitives.RequireValue(dataRowMinHeight))));
        System.Diagnostics.Debug.Assert(((dataRowHeight is null) || (((dataRowMinHeight is null) && (dataRowMaxHeight is null)))));
    }

    public virtual double? dataRowHeight => ((this.dataRowMinHeight == this.dataRowMaxHeight) ? this.dataRowMinHeight : null);
    public virtual DataTableThemeData copyWith(global::Doroti.Generated.Framework.Painting.Decoration? decoration = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? dataRowColor = null, double? dataRowHeight = null, double? dataRowMinHeight = null, double? dataRowMaxHeight = null, global::Doroti.Generated.Framework.Painting.TextStyle? dataTextStyle = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? headingRowColor = null, double? headingRowHeight = null, global::Doroti.Generated.Framework.Painting.TextStyle? headingTextStyle = null, double? horizontalMargin = null, double? columnSpacing = null, double? dividerThickness = null, double? checkboxHorizontalMargin = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? headingCellCursor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? dataRowCursor = null, global::Doroti.Generated.Framework.Rendering.MainAxisAlignment? headingRowAlignment = null)
    {
        DartRuntimePrimitives.Assert(() => ((dataRowHeight is null) || (((dataRowMinHeight is null) && (dataRowMaxHeight is null)))), () => (object?)$"dataRowHeight ({DartRuntimePrimitives.RequireValue(dataRowHeight)}) must not be set if dataRowMinHeight ({dataRowMinHeight}) or dataRowMaxHeight ({dataRowMaxHeight}) are set.");
        dataRowMinHeight = (dataRowHeight ?? dataRowMinHeight);
        dataRowMaxHeight = (dataRowHeight ?? dataRowMaxHeight);
        return new DataTableThemeData(decoration: (decoration ?? this.decoration), dataRowColor: (dataRowColor ?? this.dataRowColor), dataRowMinHeight: (dataRowMinHeight ?? this.dataRowMinHeight), dataRowMaxHeight: (dataRowMaxHeight ?? this.dataRowMaxHeight), dataTextStyle: (dataTextStyle ?? this.dataTextStyle), headingRowColor: (headingRowColor ?? this.headingRowColor), headingRowHeight: (headingRowHeight ?? this.headingRowHeight), headingTextStyle: (headingTextStyle ?? this.headingTextStyle), horizontalMargin: (horizontalMargin ?? this.horizontalMargin), columnSpacing: (columnSpacing ?? this.columnSpacing), dividerThickness: (dividerThickness ?? this.dividerThickness), checkboxHorizontalMargin: (checkboxHorizontalMargin ?? this.checkboxHorizontalMargin), headingCellCursor: (headingCellCursor ?? this.headingCellCursor), dataRowCursor: (dataRowCursor ?? this.dataRowCursor), headingRowAlignment: (headingRowAlignment ?? this.headingRowAlignment));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DataTableThemeData lerp(DataTableThemeData a, DataTableThemeData b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new DataTableThemeData(decoration: Decoration.lerp(((DataTableThemeData)a).decoration, ((DataTableThemeData)b).decoration, t), dataRowColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(((DataTableThemeData)a).dataRowColor, ((DataTableThemeData)b).dataRowColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), dataRowMinHeight: Dart_uiLibrary.lerpDouble(((DataTableThemeData)a).dataRowMinHeight, ((DataTableThemeData)b).dataRowMinHeight, t), dataRowMaxHeight: Dart_uiLibrary.lerpDouble(((DataTableThemeData)a).dataRowMaxHeight, ((DataTableThemeData)b).dataRowMaxHeight, t), dataTextStyle: TextStyle.lerp(((DataTableThemeData)a).dataTextStyle, ((DataTableThemeData)b).dataTextStyle, t), headingRowColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(((DataTableThemeData)a).headingRowColor, ((DataTableThemeData)b).headingRowColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), headingRowHeight: Dart_uiLibrary.lerpDouble(((DataTableThemeData)a).headingRowHeight, ((DataTableThemeData)b).headingRowHeight, t), headingTextStyle: TextStyle.lerp(((DataTableThemeData)a).headingTextStyle, ((DataTableThemeData)b).headingTextStyle, t), horizontalMargin: Dart_uiLibrary.lerpDouble(((DataTableThemeData)a).horizontalMargin, ((DataTableThemeData)b).horizontalMargin, t), columnSpacing: Dart_uiLibrary.lerpDouble(((DataTableThemeData)a).columnSpacing, ((DataTableThemeData)b).columnSpacing, t), dividerThickness: Dart_uiLibrary.lerpDouble(((DataTableThemeData)a).dividerThickness, ((DataTableThemeData)b).dividerThickness, t), checkboxHorizontalMargin: Dart_uiLibrary.lerpDouble(((DataTableThemeData)a).checkboxHorizontalMargin, ((DataTableThemeData)b).checkboxHorizontalMargin, t), headingCellCursor: ((t < 0.5) ? ((DataTableThemeData)a).headingCellCursor : ((DataTableThemeData)b).headingCellCursor), dataRowCursor: ((t < 0.5) ? ((DataTableThemeData)a).dataRowCursor : ((DataTableThemeData)b).dataRowCursor), headingRowAlignment: ((t < 0.5) ? ((DataTableThemeData)a).headingRowAlignment : ((DataTableThemeData)b).headingRowAlignment));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.decoration, this.dataRowColor, this.dataRowMinHeight, this.dataRowMaxHeight, this.dataTextStyle, this.headingRowColor, this.headingRowHeight, this.headingTextStyle, this.horizontalMargin, this.columnSpacing, this.dividerThickness, this.checkboxHorizontalMargin, this.headingCellCursor, this.dataRowCursor, this.headingRowAlignment));
    public override bool Equals(object? other)
    {
        var __other = other as DataTableThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((((((((__other is DataTableThemeData) && (object.Equals(((DataTableThemeData)((DataTableThemeData)__other)).decoration, this.decoration))) && (object.Equals(((DataTableThemeData)((DataTableThemeData)__other)).dataRowColor, this.dataRowColor))) && (((DataTableThemeData)((DataTableThemeData)__other)).dataRowMinHeight == this.dataRowMinHeight)) && (((DataTableThemeData)((DataTableThemeData)__other)).dataRowMaxHeight == this.dataRowMaxHeight)) && (object.Equals(((DataTableThemeData)((DataTableThemeData)__other)).dataTextStyle, this.dataTextStyle))) && (object.Equals(((DataTableThemeData)((DataTableThemeData)__other)).headingRowColor, this.headingRowColor))) && (((DataTableThemeData)((DataTableThemeData)__other)).headingRowHeight == this.headingRowHeight)) && (object.Equals(((DataTableThemeData)((DataTableThemeData)__other)).headingTextStyle, this.headingTextStyle))) && (((DataTableThemeData)((DataTableThemeData)__other)).horizontalMargin == this.horizontalMargin)) && (((DataTableThemeData)((DataTableThemeData)__other)).columnSpacing == this.columnSpacing)) && (((DataTableThemeData)((DataTableThemeData)__other)).dividerThickness == this.dividerThickness)) && (((DataTableThemeData)((DataTableThemeData)__other)).checkboxHorizontalMargin == this.checkboxHorizontalMargin)) && (object.Equals(((DataTableThemeData)((DataTableThemeData)__other)).headingCellCursor, this.headingCellCursor))) && (object.Equals(((DataTableThemeData)((DataTableThemeData)__other)).dataRowCursor, this.dataRowCursor))) && (object.Equals(((DataTableThemeData)((DataTableThemeData)__other)).headingRowAlignment, this.headingRowAlignment)));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.Decoration>("decoration", this.decoration, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("dataRowColor", this.dataRowColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("dataRowMinHeight", this.dataRowMinHeight, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("dataRowMaxHeight", this.dataRowMaxHeight, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("dataTextStyle", this.dataTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("headingRowColor", this.headingRowColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("headingRowHeight", this.headingRowHeight, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("headingTextStyle", this.headingTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("horizontalMargin", this.horizontalMargin, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("columnSpacing", this.columnSpacing, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("dividerThickness", this.dividerThickness, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("checkboxHorizontalMargin", this.checkboxHorizontalMargin, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>?>("headingCellCursor", this.headingCellCursor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>?>("dataRowCursor", this.dataRowCursor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Rendering.MainAxisAlignment>("headingRowAlignment", this.headingRowAlignment, defaultValue: null));
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

public class DataTableTheme : global::Doroti.Generated.Framework.Widgets.InheritedWidget
{
    public virtual DataTableThemeData data { get; private set; } = default!;

    public DataTableTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, DataTableThemeData data = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static DataTableThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DataTableTheme? dataTableTheme__13030 = ((DataTableTheme?)(object?)context.dependOnInheritedWidgetOfExactType<DataTableTheme>());
        return (dataTableTheme__13030?.data ?? Theme.of(context).dataTableTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((DataTableTheme)oldWidget).data)));
}
