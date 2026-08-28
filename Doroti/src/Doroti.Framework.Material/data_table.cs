// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/data_table.dart
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

public delegate void DataColumnSortCallback(long columnIndex, bool ascending);

public class DataColumn
{
    public virtual global::Doroti.Framework.Widgets.Widget label { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.TableColumnWidth? columnWidth { get; private set; }
    public virtual string? tooltip { get; private set; }
    public virtual bool numeric { get; private set; } = default!;
    public virtual global::System.Action<long, bool>? onSort { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor { get; private set; }
    public virtual global::Doroti.Framework.Rendering.MainAxisAlignment? headingRowAlignment { get; private set; }

    public DataColumn(global::Doroti.Framework.Widgets.Widget label, global::Doroti.Framework.Rendering.TableColumnWidth? columnWidth = null, string? tooltip = null, bool numeric = false, global::System.Action<long, bool>? onSort = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null, global::Doroti.Framework.Rendering.MainAxisAlignment? headingRowAlignment = null)
    {
        this.label = label;
        this.columnWidth = columnWidth;
        this.tooltip = tooltip;
        this.numeric = numeric;
        this.onSort = onSort;
        this.mouseCursor = mouseCursor;
        this.headingRowAlignment = headingRowAlignment;
    }

    internal virtual bool _debugInteractive => DartRuntimePrimitives.ConvertValue<bool>((this.onSort is not null));
}

public class DataRow
{
    public virtual global::Doroti.Framework.Foundation.LocalKey? key { get; private set; }
    public virtual global::System.Action<bool?>? onSelectChanged { get; private set; }
    public virtual global::System.Action? onLongPress { get; private set; }
    public virtual global::System.Action<bool>? onHover { get; private set; }
    public virtual bool selected { get; private set; } = default!;
    public virtual List<DataCell> cells { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? color { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor { get; private set; }

    public DataRow(global::Doroti.Framework.Foundation.LocalKey? key = null, bool selected = false, global::System.Action<bool?>? onSelectChanged = null, global::System.Action? onLongPress = null, global::System.Action<bool>? onHover = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? color = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null, List<DataCell> cells = default!)
    {
        this.key = key;
        this.selected = selected;
        this.onSelectChanged = onSelectChanged;
        this.onLongPress = onLongPress;
        this.onHover = onHover;
        this.color = color;
        this.mouseCursor = mouseCursor;
        this.cells = cells;
    }

    public static DataRow CreateByIndex(long? index = null, bool selected = false, global::System.Action<bool?>? onSelectChanged = null, global::System.Action? onLongPress = null, global::System.Action<bool>? onHover = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? color = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null, List<DataCell> cells = default!)
    {
        var __instance = new DataRow(selected: selected, onSelectChanged: onSelectChanged, onLongPress: onLongPress, onHover: onHover, color: color, mouseCursor: mouseCursor, cells: cells);
        __instance.selected = selected;
        __instance.onSelectChanged = onSelectChanged;
        __instance.onLongPress = onLongPress;
        __instance.onHover = onHover;
        __instance.color = color;
        __instance.mouseCursor = mouseCursor;
        __instance.cells = cells;
        __instance.key = new global::Doroti.Framework.Foundation.ValueKey<long?>(index);
        return __instance;
    }

    internal virtual bool _debugInteractive => DartRuntimePrimitives.ConvertValue<bool>(((this.onSelectChanged is not null) || this.cells.any(((cell) => ((DataCell)cell)._debugInteractive))));
}

public class DataCell
{
    public static DataCell empty = new DataCell(global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual bool placeholder { get; private set; } = default!;
    public virtual bool showEditIcon { get; private set; } = default!;
    public virtual global::System.Action? onTap { get; private set; }
    public virtual global::System.Action? onDoubleTap { get; private set; }
    public virtual global::System.Action? onLongPress { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>? onTapDown { get; private set; }
    public virtual global::System.Action? onTapCancel { get; private set; }

    public DataCell(global::Doroti.Framework.Widgets.Widget child, bool placeholder = false, bool showEditIcon = false, global::System.Action? onTap = null, global::System.Action? onLongPress = null, global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>? onTapDown = null, global::System.Action? onDoubleTap = null, global::System.Action? onTapCancel = null)
    {
        this.child = child;
        this.placeholder = placeholder;
        this.showEditIcon = showEditIcon;
        this.onTap = onTap;
        this.onLongPress = onLongPress;
        this.onTapDown = onTapDown;
        this.onDoubleTap = onDoubleTap;
        this.onTapCancel = onTapCancel;
    }

    internal virtual bool _debugInteractive => DartRuntimePrimitives.ConvertValue<bool>((((((this.onTap is not null) || (this.onDoubleTap is not null)) || (this.onLongPress is not null)) || (this.onTapDown is not null)) || (this.onTapCancel is not null)));
}

public class DataTable : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual List<DataColumn> columns { get; private set; } = default!;
    public virtual long? sortColumnIndex { get; private set; }
    public virtual bool sortAscending { get; private set; } = default!;
    public virtual global::System.Action<bool?>? onSelectAll { get; private set; }
    public virtual global::Doroti.Framework.Painting.Decoration? decoration { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? dataRowColor { get; private set; }
    public virtual double? dataRowMinHeight { get; private set; }
    public virtual double? dataRowMaxHeight { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? dataTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? headingRowColor { get; private set; }
    public virtual double? headingRowHeight { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? headingTextStyle { get; private set; }
    public virtual double? horizontalMargin { get; private set; }
    public virtual double? columnSpacing { get; private set; }
    public virtual bool showCheckboxColumn { get; private set; } = default!;
    public virtual List<DataRow> rows { get; private set; } = default!;
    public virtual double? dividerThickness { get; private set; }
    public virtual bool showBottomBorder { get; private set; } = default!;
    public virtual double? checkboxHorizontalMargin { get; private set; }
    public virtual global::Doroti.Framework.Rendering.TableBorder? border { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    internal virtual long? _onlyTextColumn { get; private set; }
    internal static global::Doroti.Framework.Foundation.LocalKey _headingRowKey = ((global::Doroti.Framework.Foundation.LocalKey)(object?)new global::Doroti.Framework.Foundation.UniqueKey());
    internal const double _headingRowHeight = 56.0;
    internal const double _horizontalMargin = 24.0;
    internal const double _columnSpacing = 56.0;
    internal const double _sortArrowPadding = 2.0;
    internal const double _dividerThickness = 1.0;
    internal static Duration _sortArrowAnimationDuration = Duration.Create(milliseconds: 150L);

    public DataTable(global::Doroti.Framework.Foundation.Key? key = null, List<DataColumn> columns = default!, long? sortColumnIndex = null, bool sortAscending = true, global::System.Action<bool?>? onSelectAll = null, global::Doroti.Framework.Painting.Decoration? decoration = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? dataRowColor = null, double? dataRowHeight = null, double? dataRowMinHeight = null, double? dataRowMaxHeight = null, global::Doroti.Framework.Painting.TextStyle? dataTextStyle = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? headingRowColor = null, double? headingRowHeight = null, global::Doroti.Framework.Painting.TextStyle? headingTextStyle = null, double? horizontalMargin = null, double? columnSpacing = null, bool showCheckboxColumn = true, bool showBottomBorder = false, double? dividerThickness = null, List<DataRow> rows = default!, double? checkboxHorizontalMargin = null, global::Doroti.Framework.Rendering.TableBorder? border = null, Clip clipBehavior = Clip.none) : base(key: key)
    {
        this.columns = columns;
        this.sortColumnIndex = sortColumnIndex;
        this.sortAscending = sortAscending;
        this.onSelectAll = onSelectAll;
        this.decoration = decoration;
        this.dataRowColor = dataRowColor;
        this.dataTextStyle = dataTextStyle;
        this.headingRowColor = headingRowColor;
        this.headingRowHeight = headingRowHeight;
        this.headingTextStyle = headingTextStyle;
        this.horizontalMargin = horizontalMargin;
        this.columnSpacing = columnSpacing;
        this.showCheckboxColumn = showCheckboxColumn;
        this.showBottomBorder = showBottomBorder;
        this.dividerThickness = dividerThickness;
        this.rows = rows;
        this.checkboxHorizontalMargin = checkboxHorizontalMargin;
        this.border = border;
        this.clipBehavior = clipBehavior;
        this.dataRowMinHeight = (dataRowHeight ?? dataRowMinHeight);
        this.dataRowMaxHeight = (dataRowHeight ?? dataRowMaxHeight);
        this._onlyTextColumn = DataTable._initOnlyTextColumn(columns);
        System.Diagnostics.Debug.Assert(System.Linq.Enumerable.Any(columns));
        System.Diagnostics.Debug.Assert(((sortColumnIndex is null) || (((sortColumnIndex >= 0L) && (DartRuntimePrimitives.RequireValue(sortColumnIndex) < checked((long)(columns.Count)))))));
        System.Diagnostics.Debug.Assert(!rows.any(((row) => (checked((long)(((DataRow)row).cells.Count)) != checked((long)(columns.Count))))));
        System.Diagnostics.Debug.Assert(((dividerThickness is null) || (dividerThickness >= 0L)));
        System.Diagnostics.Debug.Assert((((dataRowMinHeight is null) || (dataRowMaxHeight is null)) || (dataRowMaxHeight >= DartRuntimePrimitives.RequireValue(dataRowMinHeight))));
        System.Diagnostics.Debug.Assert(((dataRowHeight is null) || (((dataRowMinHeight is null) && (dataRowMaxHeight is null)))));
    }

    public virtual double? dataRowHeight => ((this.dataRowMinHeight == this.dataRowMaxHeight) ? this.dataRowMinHeight : null);
    internal static long? _initOnlyTextColumn(List<DataColumn> columns)
    {
        long? result = default!;
        for (var index = 0L; (index < checked((long)(columns.Count))); index += 1L)
        {
            DataColumn column = columns[(int)(index)];
            if (!((DataColumn)column).numeric)
            {
                if ((result is not null))
                {
                    long result__30086__value30245 = DartRuntimePrimitives.RequireValue(result);
                    return null;
                }
                result = index;
            }
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugInteractive
    {
        get
        {
            return (this.columns.any(((column) => ((DataColumn)column)._debugInteractive)) || this.rows.any(((row) => ((DataRow)row)._debugInteractive)));
            return default!;
        }
    }
    internal virtual void _handleSelectAll(bool? @checked, bool someChecked)
    {
        bool effectiveChecked = (someChecked || ((@checked ?? false)));
        if ((this.onSelectAll is not null))
        {
            this.onSelectAll!(effectiveChecked);
        }
        else
        {
            foreach (DataRow row in this.rows)
            {
                if (((((DataRow)row).onSelectChanged is not null) && (((DataRow)row).selected != effectiveChecked)))
                {
                    ((DataRow)row).onSelectChanged!(effectiveChecked);
                }
            }
        }
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildCheckbox(global::Doroti.Framework.Widgets.BuildContext context, bool? @checked, global::System.Action? onRowTap, global::System.Action<bool?>? onCheckboxChanged, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor, bool tristate, global::Doroti.Framework.Services.MouseCursor? rowMouseCursor = null)
    {
        ThemeData themeData = Theme.of(context);
        double effectiveHorizontalMargin = ((this.horizontalMargin ?? themeData.dataTableTheme.horizontalMargin) ?? _horizontalMargin);
        double effectiveCheckboxHorizontalMarginStart = ((this.checkboxHorizontalMargin ?? themeData.dataTableTheme.checkboxHorizontalMargin) ?? effectiveHorizontalMargin);
        double effectiveCheckboxHorizontalMarginEnd = ((this.checkboxHorizontalMargin ?? themeData.dataTableTheme.checkboxHorizontalMargin) ?? (effectiveHorizontalMargin / 2.0));
        global::Doroti.Framework.Widgets.Widget contents = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(container: true, child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: effectiveCheckboxHorizontalMarginStart, end: effectiveCheckboxHorizontalMarginEnd), child: new global::Doroti.Framework.Widgets.Center(child: new Checkbox(value: @checked, onChanged: onCheckboxChanged, tristate: tristate)))));
        if ((onRowTap is not null))
        {
            contents = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new TableRowInkWell(onTap: () => onRowTap(), overlayColor: overlayColor, mouseCursor: rowMouseCursor, child: contents));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.TableCell(verticalAlignment: global::Doroti.Framework.Rendering.TableCellVerticalAlignment.fill, child: contents));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildHeadingCell(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Painting.EdgeInsetsGeometry padding, global::Doroti.Framework.Widgets.Widget label, string? tooltip, bool numeric, global::System.Action? onSort, bool sorted, bool ascending, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor, global::Doroti.Framework.Services.MouseCursor? mouseCursor, global::Doroti.Framework.Rendering.MainAxisAlignment headingRowAlignment)
    {
        ThemeData themeData = Theme.of(context);
        DataTableThemeData dataTableThemeLocal = DataTableTheme.of(context);
        label = new global::Doroti.Framework.Widgets.Semantics(role: SemanticsRole.columnHeader, child: new global::Doroti.Framework.Widgets.Row(textDirection: (numeric ? TextDirection.rtl : null), mainAxisAlignment: headingRowAlignment, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection34143 = new List<global::Doroti.Framework.Widgets.Widget>(); if (((object.Equals(headingRowAlignment, global::Doroti.Framework.Rendering.MainAxisAlignment.center)) && (onSort is not null))) { __collection34143.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: (_SortArrowState__data_table._arrowIconSize + _sortArrowPadding)))); } __collection34143.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(label)); if ((onSort is not null)) { __collection34143.AddRange(new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _SortArrow__data_table(visible: sorted, up: (sorted ? ascending : null), duration: _sortArrowAnimationDuration)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: _sortArrowPadding)) }); } return __collection34143; }))()));
        global::Doroti.Framework.Painting.TextStyle effectiveHeadingTextStyle = (((this.headingTextStyle ?? dataTableThemeLocal.headingTextStyle) ?? themeData.dataTableTheme.headingTextStyle) ?? themeData.textTheme.titleSmall!);
        double effectiveHeadingRowHeight = (((this.headingRowHeight ?? dataTableThemeLocal.headingRowHeight) ?? themeData.dataTableTheme.headingRowHeight) ?? _headingRowHeight);
        label = new global::Doroti.Framework.Widgets.Container(padding: padding, height: effectiveHeadingRowHeight, alignment: (numeric ? global::Doroti.Framework.Painting.Alignment.centerRight : global::Doroti.Framework.Painting.AlignmentDirectional.centerStart), child: new global::Doroti.Framework.Widgets.AnimatedDefaultTextStyle(style: DefaultTextStyle.of(context).style.merge(effectiveHeadingTextStyle), softWrap: false, duration: _sortArrowAnimationDuration, child: label));
        if ((tooltip is not null))
        {
            label = new Tooltip(message: tooltip, child: label);
        }
        label = new InkWell(onTap: onSort, overlayColor: overlayColor, mouseCursor: mouseCursor, child: label);
        return label;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildDataCell(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Painting.EdgeInsetsGeometry padding, global::Doroti.Framework.Widgets.Widget label, bool numeric, bool placeholder, bool showEditIcon, global::System.Action? onTap, global::System.Action? onSelectChanged, global::System.Action? onDoubleTap, global::System.Action? onLongPress, global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>? onTapDown, global::System.Action? onTapCancel, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor, global::System.Action? onRowLongPress, global::System.Action<bool>? onRowHover, global::Doroti.Framework.Services.MouseCursor? mouseCursor)
    {
        ThemeData themeData = Theme.of(context);
        DataTableThemeData dataTableThemeLocal = DataTableTheme.of(context);
        if (showEditIcon)
        {
            global::Doroti.Framework.Widgets.Widget icon = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Icon(Icons.edit, size: 18.0));
            label = new global::Doroti.Framework.Widgets.Expanded(child: label);
            label = new global::Doroti.Framework.Widgets.Row(textDirection: (numeric ? TextDirection.rtl : null), children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(label), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(icon) });
        }
        global::Doroti.Framework.Painting.TextStyle effectiveDataTextStyle = (((this.dataTextStyle ?? dataTableThemeLocal.dataTextStyle) ?? themeData.dataTableTheme.dataTextStyle) ?? themeData.textTheme.bodyMedium!);
        double effectiveDataRowMinHeight = (((this.dataRowMinHeight ?? dataTableThemeLocal.dataRowMinHeight) ?? themeData.dataTableTheme.dataRowMinHeight) ?? ConstantsLibrary.kMinInteractiveDimension);
        double effectiveDataRowMaxHeight = (((this.dataRowMaxHeight ?? dataTableThemeLocal.dataRowMaxHeight) ?? themeData.dataTableTheme.dataRowMaxHeight) ?? ConstantsLibrary.kMinInteractiveDimension);
        label = new global::Doroti.Framework.Widgets.Container(padding: padding, constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minHeight: effectiveDataRowMinHeight, maxHeight: effectiveDataRowMaxHeight), alignment: (numeric ? global::Doroti.Framework.Painting.Alignment.centerRight : global::Doroti.Framework.Painting.AlignmentDirectional.centerStart), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: DefaultTextStyle.of(context).style.merge(effectiveDataTextStyle).copyWith(color: (placeholder ? ((global::Doroti.Framework.Painting.TextStyle)effectiveDataTextStyle).color!.withOpacity(0.6) : null)), child: new DropdownButtonHideUnderline(child: label)));
        if ((((((onTap is not null) || (onDoubleTap is not null)) || (onLongPress is not null)) || (onTapDown is not null)) || (onTapCancel is not null)))
        {
            label = new InkWell(onTap: onTap, onDoubleTap: onDoubleTap, onLongPress: onLongPress, onTapCancel: onTapCancel, onTapDown: onTapDown, overlayColor: overlayColor, child: label);
        }
        else
        {
            if ((((onSelectChanged is not null) || (onRowLongPress is not null)) || (onRowHover is not null)))
            {
                label = new TableRowInkWell(onTap: () => onSelectChanged(), onLongPress: () => onRowLongPress(), onHover: (global::System.Action<bool>?)onRowHover, overlayColor: overlayColor, mouseCursor: mouseCursor, child: label);
            }
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.TableCell(child: label));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => (!this._debugInteractive || DebugLibrary.debugCheckHasMaterial(context)));
        ThemeData theme = Theme.of(context);
        DataTableThemeData dataTableThemeLocal = DataTableTheme.of(context);
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? effectiveHeadingRowColor = ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>?)(object?)((this.headingRowColor ?? dataTableThemeLocal.headingRowColor) ?? theme.dataTableTheme.headingRowColor));
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? effectiveDataRowColor = ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>?)(object?)((this.dataRowColor ?? dataTableThemeLocal.dataRowColor) ?? theme.dataTableTheme.dataRowColor));
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> defaultRowColor = ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>)(object?)WidgetStateProperty.resolveWith((states) =>
        {
            if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
            {
                return (theme.colorScheme.primary.withOpacity(0.08));
            }
            return null;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        bool anyRowSelectable = this.rows.any(((row) => (((DataRow)row).onSelectChanged is not null)));
        bool displayCheckboxColumn = (this.showCheckboxColumn && anyRowSelectable);
        IEnumerable<DataRow> rowsWithCheckbox = (displayCheckboxColumn ? this.rows.where(((row) => (((DataRow)row).onSelectChanged is not null))) : new List<DataRow>());
        IEnumerable<DataRow> rowsChecked = rowsWithCheckbox.where(((row) => ((DataRow)row).selected));
        bool allChecked = (displayCheckboxColumn && (rowsChecked.Count() == rowsWithCheckbox.Count()));
        bool anyChecked = (displayCheckboxColumn && System.Linq.Enumerable.Any(rowsChecked));
        bool someChecked = (anyChecked && !allChecked);
        double effectiveHorizontalMargin = (((this.horizontalMargin ?? dataTableThemeLocal.horizontalMargin) ?? theme.dataTableTheme.horizontalMargin) ?? _horizontalMargin);
        double effectiveCheckboxHorizontalMarginStart = (((this.checkboxHorizontalMargin ?? dataTableThemeLocal.checkboxHorizontalMargin) ?? theme.dataTableTheme.checkboxHorizontalMargin) ?? effectiveHorizontalMargin);
        double effectiveCheckboxHorizontalMarginEnd = (((this.checkboxHorizontalMargin ?? dataTableThemeLocal.checkboxHorizontalMargin) ?? theme.dataTableTheme.checkboxHorizontalMargin) ?? (effectiveHorizontalMargin / 2.0));
        double effectiveColumnSpacing = (((this.columnSpacing ?? dataTableThemeLocal.columnSpacing) ?? theme.dataTableTheme.columnSpacing) ?? _columnSpacing);
        var tableColumns = new List<global::Doroti.Framework.Rendering.TableColumnWidth>(System.Linq.Enumerable.Repeat<global::Doroti.Framework.Rendering.TableColumnWidth>(new _NullTableColumnWidth__data_table(), checked((int)(checked((long)(this.columns.Count)) + ((displayCheckboxColumn ? 1L : 0L))))));
        var tableRows = new List<global::Doroti.Framework.Widgets.TableRow>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)(checked((long)(this.rows.Count)) + 1L))), ((index) =>
        {
            bool isSelected = ((index > 0L) && this.rows[(int)((index - 1L))].selected);
            bool isDisabled = (((index > 0L) && anyRowSelectable) && (this.rows[(int)((index - 1L))].onSelectChanged is null));
            var statesLocal = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection41545 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (isSelected) { __collection41545.Add(global::Doroti.Framework.Widgets.WidgetState.selected); } if (isDisabled) { __collection41545.Add(global::Doroti.Framework.Widgets.WidgetState.disabled); } return __collection41545; }))();
            global::Doroti.Ui.Color? resolvedDataRowColor = ((global::Doroti.Ui.Color?)(object?)((index > 0L) ? ((this.rows[(int)((index - 1L))].color ?? effectiveDataRowColor))?.resolve(statesLocal) : null));
            global::Doroti.Ui.Color? resolvedHeadingRowColor = ((global::Doroti.Ui.Color?)(object?)effectiveHeadingRowColor?.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>()));
            var rowColor = ((index > 0L) ? resolvedDataRowColor : resolvedHeadingRowColor);
            global::Doroti.Framework.Painting.BorderSide borderSide = ((global::Doroti.Framework.Painting.BorderSide)(object?)Divider.createBorderSide(context, width: (((this.dividerThickness ?? dataTableThemeLocal.dividerThickness) ?? theme.dataTableTheme.dividerThickness) ?? _dividerThickness)));
            global::Doroti.Framework.Painting.Border? borderLocal = (this.showBottomBorder ? new global::Doroti.Framework.Painting.Border(bottom: borderSide) : ((index == 0L) ? null : new global::Doroti.Framework.Painting.Border(top: borderSide)));
            return new global::Doroti.Framework.Widgets.TableRow(key: ((index == 0L) ? _headingRowKey : this.rows[(int)((index - 1L))].key), decoration: new global::Doroti.Framework.Painting.BoxDecoration(border: borderLocal, color: ((rowColor ?? (Color)defaultRowColor.resolve(statesLocal)))), children: new List<global::Doroti.Framework.Widgets.Widget>(System.Linq.Enumerable.Repeat<global::Doroti.Framework.Widgets.Widget>(new _NullWidget__data_table(), checked((int)checked((long)(tableColumns.Count))))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        long rowIndex = default!;
        var displayColumnIndex = 0L;
        if (displayCheckboxColumn)
        {
            tableColumns[(int)(0L)] = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.TableColumnWidth>(new global::Doroti.Framework.Rendering.FixedColumnWidth(((effectiveCheckboxHorizontalMarginStart + Checkbox.width) + effectiveCheckboxHorizontalMarginEnd)));
            tableRows[(int)(0L)].children[(int)(0L)] = _buildCheckbox(context: context, @checked: (someChecked ? null : allChecked), onRowTap: null, onCheckboxChanged: ((global::System.Action<bool?>)((@checked) => { _handleSelectAll(@checked, someChecked); })), overlayColor: null, tristate: true);
            rowIndex = 1L;
            foreach (DataRow rowLocal in this.rows)
            {
                var statesAlternate = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection43445 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (((DataRow)rowLocal).selected) { __collection43445.Add(global::Doroti.Framework.Widgets.WidgetState.selected); } return __collection43445; }))();
                tableRows[(int)(rowIndex)].children[(int)(0L)] = _buildCheckbox(context: context, @checked: ((DataRow)rowLocal).selected, onRowTap: ((global::System.Action)((((DataRow)rowLocal).onSelectChanged is null) ? null : (() => { ((DataRow)rowLocal).onSelectChanged?.Invoke(!((DataRow)rowLocal).selected); }))), onCheckboxChanged: (global::System.Action<bool?>?)((DataRow)rowLocal).onSelectChanged, overlayColor: (((DataRow)rowLocal).color ?? effectiveDataRowColor), rowMouseCursor: ((((DataRow)rowLocal).mouseCursor?.resolve(statesAlternate) ?? (global::Doroti.Framework.Services.MouseCursor)dataTableThemeLocal.dataRowCursor?.resolve(statesAlternate))), tristate: false);
                rowIndex += 1L;
            }
            displayColumnIndex += 1L;
        }
        for (var dataColumnIndex = 0L; (dataColumnIndex < checked((long)(this.columns.Count))); dataColumnIndex += 1L)
        {
            DataColumn column = this.columns[(int)(dataColumnIndex)];
            double paddingStart = (dataColumnIndex switch { 0L when ((displayCheckboxColumn && (this.checkboxHorizontalMargin is null))) => (effectiveHorizontalMargin / 2.0), 0L => effectiveHorizontalMargin, _ => (effectiveColumnSpacing / 2.0) });
            double paddingEnd = default!;
            if ((dataColumnIndex == (checked((long)(this.columns.Count)) - 1L)))
            {
                paddingEnd = effectiveHorizontalMargin;
            }
            else
            {
                paddingEnd = (effectiveColumnSpacing / 2.0);
            }
            var paddingLocal = global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: paddingStart, end: paddingEnd);
            if ((((DataColumn)column).columnWidth is not null))
            {
                tableColumns[(int)(displayColumnIndex)] = ((DataColumn)column).columnWidth!;
            }
            else
            {
                if ((dataColumnIndex == this._onlyTextColumn))
                {
                    tableColumns[(int)(displayColumnIndex)] = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.TableColumnWidth>(new global::Doroti.Framework.Rendering.IntrinsicColumnWidth(flex: 1.0));
                }
                else
                {
                    tableColumns[(int)(displayColumnIndex)] = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.TableColumnWidth>(new global::Doroti.Framework.Rendering.IntrinsicColumnWidth());
                }
            }
            var headerStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection45174 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if ((((DataColumn)column).onSort is null)) { __collection45174.Add(global::Doroti.Framework.Widgets.WidgetState.disabled); } return __collection45174; }))();
            tableRows[(int)(0L)].children[(int)(displayColumnIndex)] = _buildHeadingCell(context: context, padding: paddingLocal, label: ((DataColumn)column).label, tooltip: ((DataColumn)column).tooltip, numeric: ((DataColumn)column).numeric, onSort: ((global::System.Action)((((DataColumn)column).onSort is not null) ? (() => { ((DataColumn)column).onSort!(dataColumnIndex, ((this.sortColumnIndex != dataColumnIndex) || !this.sortAscending)); }) : null)), sorted: (dataColumnIndex == this.sortColumnIndex), ascending: this.sortAscending, overlayColor: effectiveHeadingRowColor, mouseCursor: ((((DataColumn)column).mouseCursor?.resolve(headerStates) ?? (global::Doroti.Framework.Services.MouseCursor)dataTableThemeLocal.headingCellCursor?.resolve(headerStates))), headingRowAlignment: ((((DataColumn)column).headingRowAlignment ?? dataTableThemeLocal.headingRowAlignment) ?? global::Doroti.Framework.Rendering.MainAxisAlignment.start));
            rowIndex = 1L;
            foreach (DataRow rowAlternate in this.rows)
            {
                var statesNested = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection46198 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (((DataRow)rowAlternate).selected) { __collection46198.Add(global::Doroti.Framework.Widgets.WidgetState.selected); } return __collection46198; }))();
                DataCell cell = ((DataRow)rowAlternate).cells[(int)(dataColumnIndex)];
                tableRows[(int)(rowIndex)].children[(int)(displayColumnIndex)] = _buildDataCell(context: context, padding: paddingLocal, label: ((DataCell)cell).child, numeric: ((DataColumn)column).numeric, placeholder: ((DataCell)cell).placeholder, showEditIcon: ((DataCell)cell).showEditIcon, onTap: () => ((DataCell)cell).onTap(), onDoubleTap: () => ((DataCell)cell).onDoubleTap(), onLongPress: () => ((DataCell)cell).onLongPress(), onTapCancel: () => ((DataCell)cell).onTapCancel(), onTapDown: (global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>?)((DataCell)cell).onTapDown, onSelectChanged: ((global::System.Action)((((DataRow)rowAlternate).onSelectChanged is null) ? null : (() => { ((DataRow)rowAlternate).onSelectChanged?.Invoke(!((DataRow)rowAlternate).selected); }))), overlayColor: (((DataRow)rowAlternate).color ?? effectiveDataRowColor), onRowLongPress: () => ((DataRow)rowAlternate).onLongPress(), onRowHover: (global::System.Action<bool>?)((DataRow)rowAlternate).onHover, mouseCursor: ((((DataRow)rowAlternate).mouseCursor?.resolve(statesNested) ?? (global::Doroti.Framework.Services.MouseCursor)dataTableThemeLocal.dataRowCursor?.resolve(statesNested))));
                rowIndex += 1L;
            }
            displayColumnIndex += 1L;
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Container(decoration: ((this.decoration ?? dataTableThemeLocal.decoration) ?? theme.dataTableTheme.decoration), child: new Material(type: MaterialType.transparency, borderRadius: this.border?.borderRadius, clipBehavior: this.clipBehavior, child: new global::Doroti.Framework.Widgets.Table(columnWidths: tableColumns.asMap(), defaultVerticalAlignment: global::Doroti.Framework.Rendering.TableCellVerticalAlignment.middle, children: tableRows, border: this.border))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class TableRowInkWell : InkResponse
{
    public TableRowInkWell(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? child = null, global::System.Action? onTap = null, global::System.Action? onDoubleTap = null, global::System.Action? onLongPress = null, global::System.Action<bool>? onHighlightChanged = null, global::System.Action<bool>? onHover = null, global::System.Action? onSecondaryTap = null, global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>? onSecondaryTapDown = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null) : base(key: key, child: child, onTap: onTap, onDoubleTap: onDoubleTap, onLongPress: onLongPress, onHighlightChanged: onHighlightChanged, onHover: onHover, onSecondaryTap: onSecondaryTap, onSecondaryTapDown: onSecondaryTapDown, overlayColor: overlayColor, mouseCursor: mouseCursor, containedInkWell: true, highlightShape: global::Doroti.Framework.Painting.BoxShape.rectangle)
    {
    }

    public virtual RectCallback getRectCallback(global::Doroti.Framework.Rendering.RenderBox referenceBox)
    {
        return ((RectCallback)(object?)(() =>
        {
            global::Doroti.Framework.Rendering.RenderObject cell = ((global::Doroti.Framework.Rendering.RenderObject)(object?)referenceBox);
            global::Doroti.Framework.Rendering.RenderObject? table = ((global::Doroti.Framework.Rendering.RenderObject)cell).parent;
            var transform = Matrix4.identity();
            while ((true && (table is not global::Doroti.Framework.Rendering.RenderTable)))
            {
                ((dynamic)table).applyPaintTransform(cell, transform);
                DartRuntimePrimitives.Assert(() => (object.Equals(table, ((global::Doroti.Framework.Rendering.RenderObject)cell).parent)));
                cell = table;
                table = ((global::Doroti.Framework.Rendering.RenderObject)table).parent;
            }
            if ((table is global::Doroti.Framework.Rendering.RenderTable))
            {
                global::Doroti.Framework.Rendering.RenderTable table__49236__as49524 = (global::Doroti.Framework.Rendering.RenderTable)table;
                var cellParentData = ((global::Doroti.Framework.Rendering.TableCellParentData?)(object?)((global::Doroti.Framework.Rendering.RenderObject)cell).parentData!)!;
                DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Rendering.TableCellParentData)cellParentData).y is not null));
                global::Doroti.Ui.Rect rect = ((global::Doroti.Ui.Rect)(object?)((global::Doroti.Framework.Rendering.RenderTable)table__49236__as49524).getRowBox(DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Rendering.TableCellParentData)cellParentData).y)));
                ((global::Doroti.Framework.Rendering.RenderTable)table__49236__as49524).applyPaintTransform(cell, transform);
                global::Doroti.Ui.Offset? offset = ((global::Doroti.Ui.Offset?)(object?)MatrixUtils.getAsTranslation(transform));
                if ((offset is not null))
                {
                    Offset offset__49929__value49991 = DartRuntimePrimitives.RequireValue(offset);
                    return rect.shift(-DartRuntimePrimitives.RequireValue(offset__49929__value49991));
                }
            }
            return Rect.zero;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool debugCheckContext(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasTable(context));
        return base.debugCheckContext(context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _SortArrow__data_table : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual bool visible { get; private set; } = default!;
    public virtual bool? up { get; private set; }
    public virtual Duration duration { get; private set; } = default!;

    internal _SortArrow__data_table(bool visible, bool? up, Duration duration)
    {
        this.visible = visible;
        this.up = up;
        this.duration = duration;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SortArrowState__data_table());
}

public class _SortArrowState__data_table : global::Doroti.Framework.Widgets.State<_SortArrow__data_table>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<_SortArrow__data_table>
{
    internal virtual global::Doroti.Framework.Animation.AnimationController _opacityController { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _opacityAnimation { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Animation.AnimationController _orientationController { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _orientationAnimation { get; private set; } = default!;
    internal virtual double _orientationOffset { get; set; } = 0.0;
    internal virtual bool? _up { get; set; } = default;
    internal static global::Doroti.Framework.Animation.Animatable<double> _turnTween = new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: Dart_mathLibrary.pi).chain(new global::Doroti.Framework.Animation.CurveTween(curve: global::Doroti.Framework.Animation.Curves.easeIn));
    internal static double _arrowIconBaselineOffset = -1.5;
    internal const double _arrowIconSize = 16.0;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _up = ((_SortArrow__data_table)this.widget).up;
        _opacityAnimation = ((Func<global::Doroti.Framework.Animation.CurvedAnimation>)(() =>
{
    var __cascade = new global::Doroti.Framework.Animation.CurvedAnimation(parent: _opacityController = new global::Doroti.Framework.Animation.AnimationController(duration: ((_SortArrow__data_table)this.widget).duration, vsync: this), curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
    __cascade.addListener(this._rebuild);
    return __cascade;
}))();
        this._opacityController.value = (((_SortArrow__data_table)this.widget).visible ? 1.0 : 0.0);
        _orientationController = new global::Doroti.Framework.Animation.AnimationController(duration: ((_SortArrow__data_table)this.widget).duration, vsync: this);
        _orientationAnimation = ((Func<global::Doroti.Framework.Animation.Animation<double>>)(() =>
{
    var __cascade = this._orientationController.drive(_turnTween);
    __cascade.addListener(this._rebuild);
    __cascade.addStatusListener((AnimationStatusListener)this._resetOrientationAnimation);
    return __cascade;
}))();
        if (((_SortArrow__data_table)this.widget).visible)
        {
            _orientationOffset = (DartRuntimePrimitives.RequireValue(((_SortArrow__data_table)this.widget).up) ? 0.0 : Dart_mathLibrary.pi);
        }
    }

    internal virtual void _rebuild()
    {
        setState(((global::System.Action)(() =>
        {
        })));
    }

    internal virtual void _resetOrientationAnimation(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        if (global::Doroti.Framework.Animation.AnimationStatusMembers.isCompleted(status))
        {
            DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Animation.Animation<double>)this._orientationAnimation).value == Dart_mathLibrary.pi));
            _orientationOffset += Dart_mathLibrary.pi;
            this._orientationController.value = 0.0;
        }
    }

    public override void didUpdateWidget(_SortArrow__data_table oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        var skipArrow = false;
        bool? newUp = (((_SortArrow__data_table)this.widget).up ?? this._up);
        if ((((_SortArrow__data_table)oldWidget).visible != ((_SortArrow__data_table)this.widget).visible))
        {
            if ((((_SortArrow__data_table)this.widget).visible && this._opacityController.isDismissed))
            {
                this._orientationController.stop();
                this._orientationController.value = 0.0;
                _orientationOffset = (DartRuntimePrimitives.RequireValue(newUp) ? 0.0 : Dart_mathLibrary.pi);
                skipArrow = true;
            }
            if (((_SortArrow__data_table)this.widget).visible)
            {
                this._opacityController.forward();
            }
            else
            {
                this._opacityController.reverse();
            }
        }
        if ((((this._up != newUp)) && !skipArrow))
        {
            if (this._orientationController.isDismissed)
            {
                this._orientationController.forward();
            }
            else
            {
                this._orientationController.reverse();
            }
        }
        _up = newUp;
    }

    public override void dispose()
    {
        this._opacityController.dispose();
        this._orientationController.dispose();
        this._opacityAnimation.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.FadeTransition(opacity: this._opacityAnimation, child: new global::Doroti.Framework.Widgets.Transform(transform: ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.rotationZ((this._orientationOffset + ((global::Doroti.Framework.Animation.Animation<double>)this._orientationAnimation).value));
    __cascade.setTranslationRaw(0.0, _arrowIconBaselineOffset, 0.0);
    return __cascade;
}))(), alignment: global::Doroti.Framework.Painting.Alignment.center, child: new global::Doroti.Framework.Widgets.Icon(Icons.arrow_upward, size: _arrowIconSize))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = this._tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values).enabled;
    __cascade.forceFrames = ((TickerModeData)values).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => (this._tickers is not null));
        DartRuntimePrimitives.Assert(() => this._tickers!.Contains(ticker));
        this._tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if ((this._tickers is not null))
        {
            TickerModeData values = this._tickerModeNotifier!.value;
            bool mutedLocal = !((TickerModeData)values).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = ((TickerModeData)values).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        newNotifier.addListener(this._updateTickers);
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

internal class _NullTableColumnWidth__data_table : global::Doroti.Framework.Rendering.TableColumnWidth
{
    internal _NullTableColumnWidth__data_table()
    {
    }

    public override double maxIntrinsicWidth(IEnumerable<global::Doroti.Framework.Rendering.RenderBox> cells, double containerWidth) => throw new NotImplementedException();
    public override double minIntrinsicWidth(IEnumerable<global::Doroti.Framework.Rendering.RenderBox> cells, double containerWidth) => throw new NotImplementedException();
}

internal class _NullWidget__data_table : global::Doroti.Framework.Widgets.Widget
{
    internal _NullWidget__data_table()
    {
    }

    public override global::Doroti.Framework.Widgets.Element createElement() => throw new NotImplementedException();
}
