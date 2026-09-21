// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/data_table_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class DataTableThemeData : Diagnosticable
{
    public virtual Decoration? decoration { get; private set; }
    public virtual WidgetStateProperty<Color?>? dataRowColor { get; private set; }
    public virtual double? dataRowMinHeight { get; private set; }
    public virtual double? dataRowMaxHeight { get; private set; }
    public virtual TextStyle? dataTextStyle { get; private set; }
    public virtual WidgetStateProperty<Color?>? headingRowColor { get; private set; }
    public virtual double? headingRowHeight { get; private set; }
    public virtual TextStyle? headingTextStyle { get; private set; }
    public virtual double? horizontalMargin { get; private set; }
    public virtual double? columnSpacing { get; private set; }
    public virtual double? dividerThickness { get; private set; }
    public virtual double? checkboxHorizontalMargin { get; private set; }
    public virtual WidgetStateProperty<MouseCursor?>? headingCellCursor { get; private set; }
    public virtual WidgetStateProperty<MouseCursor?>? dataRowCursor { get; private set; }
    public virtual MainAxisAlignment? headingRowAlignment { get; private set; }

    public DataTableThemeData(
        Decoration? decoration = null,
        WidgetStateProperty<Color?>? dataRowColor = null,
        double? dataRowHeight = null,
        double? dataRowMinHeight = null,
        double? dataRowMaxHeight = null,
        TextStyle? dataTextStyle = null,
        WidgetStateProperty<Color?>? headingRowColor = null,
        double? headingRowHeight = null,
        TextStyle? headingTextStyle = null,
        double? horizontalMargin = null,
        double? columnSpacing = null,
        double? dividerThickness = null,
        double? checkboxHorizontalMargin = null,
        WidgetStateProperty<MouseCursor?>? headingCellCursor = null,
        WidgetStateProperty<MouseCursor?>? dataRowCursor = null,
        MainAxisAlignment? headingRowAlignment = null
    )
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
        this.dataRowMinHeight = dataRowHeight ?? dataRowMinHeight;
        this.dataRowMaxHeight = dataRowHeight ?? dataRowMaxHeight;
        System.Diagnostics.Debug.Assert(
            (dataRowMinHeight is null)
                || (dataRowMaxHeight is null)
                || (dataRowMaxHeight >= DartRuntimePrimitives.RequireValue(dataRowMinHeight))
        );
        System.Diagnostics.Debug.Assert(
            (dataRowHeight is null) || ((dataRowMinHeight is null) && (dataRowMaxHeight is null))
        );
    }

    public virtual double? dataRowHeight =>
        (dataRowMinHeight == dataRowMaxHeight) ? dataRowMinHeight : null;

    public virtual DataTableThemeData copyWith(
        Decoration? decoration = null,
        WidgetStateProperty<Color?>? dataRowColor = null,
        double? dataRowHeight = null,
        double? dataRowMinHeight = null,
        double? dataRowMaxHeight = null,
        TextStyle? dataTextStyle = null,
        WidgetStateProperty<Color?>? headingRowColor = null,
        double? headingRowHeight = null,
        TextStyle? headingTextStyle = null,
        double? horizontalMargin = null,
        double? columnSpacing = null,
        double? dividerThickness = null,
        double? checkboxHorizontalMargin = null,
        WidgetStateProperty<MouseCursor?>? headingCellCursor = null,
        WidgetStateProperty<MouseCursor?>? dataRowCursor = null,
        MainAxisAlignment? headingRowAlignment = null
    )
    {
        DartRuntimePrimitives.Assert(
            () =>
                (dataRowHeight is null)
                || ((dataRowMinHeight is null) && (dataRowMaxHeight is null)),
            () =>
                (object?)
                    $"dataRowHeight ({DartRuntimePrimitives.RequireValue(dataRowHeight)}) must not be set if dataRowMinHeight ({dataRowMinHeight}) or dataRowMaxHeight ({dataRowMaxHeight}) are set."
        );
        dataRowMinHeight = dataRowHeight ?? dataRowMinHeight;
        dataRowMaxHeight = dataRowHeight ?? dataRowMaxHeight;
        return new DataTableThemeData(
            decoration: decoration ?? this.decoration,
            dataRowColor: dataRowColor ?? this.dataRowColor,
            dataRowMinHeight: dataRowMinHeight ?? this.dataRowMinHeight,
            dataRowMaxHeight: dataRowMaxHeight ?? this.dataRowMaxHeight,
            dataTextStyle: dataTextStyle ?? this.dataTextStyle,
            headingRowColor: headingRowColor ?? this.headingRowColor,
            headingRowHeight: headingRowHeight ?? this.headingRowHeight,
            headingTextStyle: headingTextStyle ?? this.headingTextStyle,
            horizontalMargin: horizontalMargin ?? this.horizontalMargin,
            columnSpacing: columnSpacing ?? this.columnSpacing,
            dividerThickness: dividerThickness ?? this.dividerThickness,
            checkboxHorizontalMargin: checkboxHorizontalMargin ?? this.checkboxHorizontalMargin,
            headingCellCursor: headingCellCursor ?? this.headingCellCursor,
            dataRowCursor: dataRowCursor ?? this.dataRowCursor,
            headingRowAlignment: headingRowAlignment ?? this.headingRowAlignment
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DataTableThemeData lerp(DataTableThemeData a, DataTableThemeData b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new DataTableThemeData(
            decoration: Decoration.lerp(a.decoration, b.decoration, t),
            dataRowColor: WidgetStateProperty.lerp(a.dataRowColor, b.dataRowColor, t, Color.lerp),
            dataRowMinHeight: Dart_uiLibrary.lerpDouble(a.dataRowMinHeight, b.dataRowMinHeight, t),
            dataRowMaxHeight: Dart_uiLibrary.lerpDouble(a.dataRowMaxHeight, b.dataRowMaxHeight, t),
            dataTextStyle: TextStyle.lerp(a.dataTextStyle, b.dataTextStyle, t),
            headingRowColor: WidgetStateProperty.lerp(
                a.headingRowColor,
                b.headingRowColor,
                t,
                Color.lerp
            ),
            headingRowHeight: Dart_uiLibrary.lerpDouble(a.headingRowHeight, b.headingRowHeight, t),
            headingTextStyle: TextStyle.lerp(a.headingTextStyle, b.headingTextStyle, t),
            horizontalMargin: Dart_uiLibrary.lerpDouble(a.horizontalMargin, b.horizontalMargin, t),
            columnSpacing: Dart_uiLibrary.lerpDouble(a.columnSpacing, b.columnSpacing, t),
            dividerThickness: Dart_uiLibrary.lerpDouble(a.dividerThickness, b.dividerThickness, t),
            checkboxHorizontalMargin: Dart_uiLibrary.lerpDouble(
                a.checkboxHorizontalMargin,
                b.checkboxHorizontalMargin,
                t
            ),
            headingCellCursor: (t < 0.5) ? a.headingCellCursor : b.headingCellCursor,
            dataRowCursor: (t < 0.5) ? a.dataRowCursor : b.dataRowCursor,
            headingRowAlignment: (t < 0.5) ? a.headingRowAlignment : b.headingRowAlignment
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                decoration,
                dataRowColor,
                dataRowMinHeight,
                dataRowMaxHeight,
                dataTextStyle,
                headingRowColor,
                headingRowHeight,
                headingTextStyle,
                horizontalMargin,
                columnSpacing,
                dividerThickness,
                checkboxHorizontalMargin,
                headingCellCursor,
                dataRowCursor,
                headingRowAlignment
            )
        );

    public override bool Equals(object? other)
    {
        var __other = other as DataTableThemeData;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is DataTableThemeData)
            && Equals(__other.decoration, decoration)
            && Equals(__other.dataRowColor, dataRowColor)
            && (__other.dataRowMinHeight == dataRowMinHeight)
            && (__other.dataRowMaxHeight == dataRowMaxHeight)
            && Equals(__other.dataTextStyle, dataTextStyle)
            && Equals(__other.headingRowColor, headingRowColor)
            && (__other.headingRowHeight == headingRowHeight)
            && Equals(__other.headingTextStyle, headingTextStyle)
            && (__other.horizontalMargin == horizontalMargin)
            && (__other.columnSpacing == columnSpacing)
            && (__other.dividerThickness == dividerThickness)
            && (__other.checkboxHorizontalMargin == checkboxHorizontalMargin)
            && Equals(__other.headingCellCursor, headingCellCursor)
            && Equals(__other.dataRowCursor, dataRowCursor)
            && Equals(__other.headingRowAlignment, headingRowAlignment);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(
            new DiagnosticsProperty<Decoration>("decoration", decoration, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "dataRowColor",
                dataRowColor,
                defaultValue: null
            )
        );
        properties.add(
            new DoubleProperty("dataRowMinHeight", dataRowMinHeight, defaultValue: null)
        );
        properties.add(
            new DoubleProperty("dataRowMaxHeight", dataRowMaxHeight, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>("dataTextStyle", dataTextStyle, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "headingRowColor",
                headingRowColor,
                defaultValue: null
            )
        );
        properties.add(
            new DoubleProperty("headingRowHeight", headingRowHeight, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "headingTextStyle",
                headingTextStyle,
                defaultValue: null
            )
        );
        properties.add(
            new DoubleProperty("horizontalMargin", horizontalMargin, defaultValue: null)
        );
        properties.add(new DoubleProperty("columnSpacing", columnSpacing, defaultValue: null));
        properties.add(
            new DoubleProperty("dividerThickness", dividerThickness, defaultValue: null)
        );
        properties.add(
            new DoubleProperty(
                "checkboxHorizontalMargin",
                checkboxHorizontalMargin,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<MouseCursor?>?>(
                "headingCellCursor",
                headingCellCursor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<MouseCursor?>?>(
                "dataRowCursor",
                dataRowCursor,
                defaultValue: null
            )
        );
        properties.add(
            new EnumProperty<MainAxisAlignment>(
                "headingRowAlignment",
                headingRowAlignment,
                defaultValue: null
            )
        );
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);

    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine)
                .toDiagnosticsNode()
                .toStringDeep(minLevel: minLevel);
            return true;
        });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = null
    )
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class DataTableTheme : InheritedWidget
{
    public virtual DataTableThemeData data { get; private set; } = default!;

    public DataTableTheme(
        Key? key = null,
        DataTableThemeData data = default!,
        Widget child = default!
    )
        : base(key: key, child: child)
    {
        this.data = data;
    }

    public static DataTableThemeData of(BuildContext context)
    {
        DataTableTheme? dataTableThemeLocal =
            context.dependOnInheritedWidgetOfExactType<DataTableTheme>();
        return dataTableThemeLocal?.data ?? Theme.of(context).dataTableTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((DataTableTheme)oldWidget).data));
}
