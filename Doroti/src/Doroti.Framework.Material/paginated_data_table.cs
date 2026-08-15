// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/paginated_data_table.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class PaginatedDataTable : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? header { get; private set; }
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget>? actions { get; private set; }
    public virtual List<DataColumn> columns { get; private set; } = default!;
    public virtual long? sortColumnIndex { get; private set; }
    public virtual bool sortAscending { get; private set; } = default!;
    public virtual global::System.Action<bool?>? onSelectAll { get; private set; }
    public virtual double? dataRowMinHeight { get; private set; }
    public virtual double? dataRowMaxHeight { get; private set; }
    public virtual double headingRowHeight { get; private set; } = default!;
    public virtual double horizontalMargin { get; private set; } = default!;
    public virtual double columnSpacing { get; private set; } = default!;
    public virtual bool showCheckboxColumn { get; private set; } = default!;
    public virtual bool showFirstLastButtons { get; private set; } = default!;
    public virtual long? initialFirstRowIndex { get; private set; }
    public virtual double? dividerThickness { get; private set; }
    public virtual global::System.Action<long>? onPageChanged { get; private set; }
    public virtual long rowsPerPage { get; private set; } = default!;
    public const long defaultRowsPerPage = 10L;
    public virtual List<long> availableRowsPerPage { get; private set; } = default!;
    public virtual global::System.Action<long?>? onRowsPerPageChanged { get; private set; }
    public virtual DataTableSource source { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual double? checkboxHorizontalMargin { get; private set; }
    public virtual Color? arrowHeadColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.ScrollController? controller { get; private set; }
    public virtual bool? primary { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? headingRowColor { get; private set; }
    public virtual bool showEmptyRows { get; private set; } = default!;

    public PaginatedDataTable(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget? header = null, List<global::Doroti.Generated.Framework.Widgets.Widget>? actions = null, List<DataColumn> columns = default!, long? sortColumnIndex = null, bool sortAscending = true, global::System.Action<bool?>? onSelectAll = null, double? dataRowHeight = null, double? dataRowMinHeight = null, double? dataRowMaxHeight = null, double headingRowHeight = 56.0, double horizontalMargin = 24.0, double columnSpacing = 56.0, bool showCheckboxColumn = true, bool showFirstLastButtons = false, long? initialFirstRowIndex = 0, global::System.Action<long>? onPageChanged = null, long? rowsPerPage = null, List<long> availableRowsPerPage = default!, global::System.Action<long?>? onRowsPerPageChanged = null, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, Color? arrowHeadColor = null, DataTableSource source = default!, double? checkboxHorizontalMargin = null, global::Doroti.Generated.Framework.Widgets.ScrollController? controller = null, bool? primary = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? headingRowColor = null, double? dividerThickness = null, bool showEmptyRows = true) : base(key: key)
    {
        long __rowsPerPage = rowsPerPage ?? defaultRowsPerPage;
        List<long> __availableRowsPerPage = availableRowsPerPage ?? new List<long> { defaultRowsPerPage, defaultRowsPerPage * 2, defaultRowsPerPage * 5, defaultRowsPerPage * 10 };
        this.header = header;
        this.actions = actions;
        this.columns = columns;
        this.sortColumnIndex = sortColumnIndex;
        this.sortAscending = sortAscending;
        this.onSelectAll = onSelectAll;
        this.headingRowHeight = headingRowHeight;
        this.horizontalMargin = horizontalMargin;
        this.columnSpacing = columnSpacing;
        this.showCheckboxColumn = showCheckboxColumn;
        this.showFirstLastButtons = showFirstLastButtons;
        this.initialFirstRowIndex = initialFirstRowIndex;
        this.onPageChanged = onPageChanged;
        this.rowsPerPage = __rowsPerPage;
        this.availableRowsPerPage = __availableRowsPerPage;
        this.onRowsPerPageChanged = onRowsPerPageChanged;
        this.dragStartBehavior = dragStartBehavior;
        this.arrowHeadColor = arrowHeadColor;
        this.source = source;
        this.checkboxHorizontalMargin = checkboxHorizontalMargin;
        this.controller = controller;
        this.primary = primary;
        this.headingRowColor = headingRowColor;
        this.dividerThickness = dividerThickness;
        this.showEmptyRows = showEmptyRows;
        this.dataRowMinHeight = (dataRowHeight ?? dataRowMinHeight);
        this.dataRowMaxHeight = (dataRowHeight ?? dataRowMaxHeight);
        System.Diagnostics.Debug.Assert(((actions is null) || ((header is not null))));
        System.Diagnostics.Debug.Assert(System.Linq.Enumerable.Any(columns));
        System.Diagnostics.Debug.Assert(((sortColumnIndex is null) || (((sortColumnIndex >= 0L) && (DartRuntimePrimitives.RequireValue(sortColumnIndex) < checked((long)(columns.Count)))))));
        System.Diagnostics.Debug.Assert((((dataRowMinHeight is null) || (dataRowMaxHeight is null)) || (dataRowMaxHeight >= DartRuntimePrimitives.RequireValue(dataRowMinHeight))));
        System.Diagnostics.Debug.Assert(((dataRowHeight is null) || (((dataRowMinHeight is null) && (dataRowMaxHeight is null)))));
        System.Diagnostics.Debug.Assert((__rowsPerPage > 0L));
        System.Diagnostics.Debug.Assert(((dividerThickness is null) || (dividerThickness >= 0L)));
        System.Diagnostics.Debug.Assert(((global::System.Func<bool>)(() => {
if ((onRowsPerPageChanged is not null))
{
    DartRuntimePrimitives.Assert(() => __availableRowsPerPage.Contains(DartRuntimePrimitives.RequireValue(__rowsPerPage)));
}
return true;
throw new InvalidOperationException("Dart closure completed without a value.");
}))());
        System.Diagnostics.Debug.Assert(!(((controller is not null) && ((primary ?? false)))));
    }

    public virtual double? dataRowHeight => ((this.dataRowMinHeight == this.dataRowMaxHeight) ? this.dataRowMinHeight : null);
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new PaginatedDataTableState());
}

public class PaginatedDataTableState : global::Doroti.Generated.Framework.Widgets.State<PaginatedDataTable>
{
    internal virtual long _firstRowIndex { get; set; } = default!;
    internal virtual long _rowCount { get; set; } = default!;
    internal virtual bool _rowCountApproximate { get; set; } = default!;
    internal virtual long _selectedRowCount { get; set; } = 0L;
    internal virtual DartMap<long, DataRow?> _rows { get; private set; } = new DartMap<long, DataRow?>();
    internal virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> _tableKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create();

    public override void initState()
    {
        base.initState();
        _firstRowIndex = ((((long?)PageStorage.maybeOf(this.context)?.readState(this.context)) ?? ((PaginatedDataTable)this.widget).initialFirstRowIndex) ?? 0L);
        ((PaginatedDataTable)this.widget).source.addListener(() => this._handleDataSourceChanged());
        _handleDataSourceChanged();
    }

    public override void didUpdateWidget(PaginatedDataTable oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((PaginatedDataTable)oldWidget).source, ((PaginatedDataTable)this.widget).source)))
        {
            ((PaginatedDataTable)oldWidget).source.removeListener(() => this._handleDataSourceChanged());
            ((PaginatedDataTable)this.widget).source.addListener(() => this._handleDataSourceChanged());
            _updateCaches();
        }
    }

    public override void reassemble()
    {
        base.reassemble();
        _updateCaches();
    }

    public override void dispose()
    {
        ((PaginatedDataTable)this.widget).source.removeListener(() => this._handleDataSourceChanged());
        base.dispose();
    }

    internal virtual void _handleDataSourceChanged()
    {
        setState(() => this._updateCaches());
    }

    internal virtual void _updateCaches()
    {
        _rowCount = ((PaginatedDataTable)this.widget).source.rowCount;
        _rowCountApproximate = ((PaginatedDataTable)this.widget).source.isRowCountApproximate;
        _selectedRowCount = ((PaginatedDataTable)this.widget).source.selectedRowCount;
        this._rows.Clear();
    }

    public virtual void pageTo(long rowIndex)
    {
        long oldFirstRowIndex__14719 = this._firstRowIndex;
        setState(((global::System.Action)(() => {
long rowsPerPage__14788 = ((PaginatedDataTable)this.widget).rowsPerPage;
_firstRowIndex = (((checked((long)(rowIndex / rowsPerPage__14788)))) * rowsPerPage__14788);
})));
        if ((((((PaginatedDataTable)this.widget).onPageChanged is not null)) && ((oldFirstRowIndex__14719 != this._firstRowIndex))))
        {
            ((PaginatedDataTable)this.widget).onPageChanged!(this._firstRowIndex);
        }
    }

    internal virtual DataRow _getBlankRowFor(long index)
    {
        return DataRow.CreateByIndex(index: index, cells: ((PaginatedDataTable)this.widget).columns.map<DataColumn, DataCell>(((column) => DataCell.empty)).ToList());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual DataRow _getProgressIndicatorRowFor(long index)
    {
        var haveProgressIndicator__15281 = false;
        List<DataCell> cells__15337 = ((PaginatedDataTable)this.widget).columns.map<DataColumn, DataCell>(((column) => {
if (!column.numeric)
{
    haveProgressIndicator__15281 = true;
    return new DataCell(new CircularProgressIndicator());
}
return DataCell.empty;
throw new InvalidOperationException("Dart closure completed without a value.");
})).ToList().ToList();
        if (!haveProgressIndicator__15281)
        {
            haveProgressIndicator__15281 = true;
            cells__15337[(int)(0L)] = new DataCell(new CircularProgressIndicator());
        }
        return DataRow.CreateByIndex(index: index, cells: cells__15337);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<DataRow> _getRows(long firstRowIndex, long rowsPerPage)
    {
        var result__15849 = new List<DataRow>();
        long nextPageFirstRowIndex__15885 = (firstRowIndex + rowsPerPage);
        var haveProgressIndicator__15946 = false;
        for (var index__15990 = firstRowIndex; (index__15990 < nextPageFirstRowIndex__15885); index__15990 += 1L)
        {
            DataRow? row__16073 = default!;
            if (((index__15990 < this._rowCount) || this._rowCountApproximate))
            {
                row__16073 = this._rows.putIfAbsent(index__15990, (() => ((PaginatedDataTable)this.widget).source.getRow(index__15990)));
                if (((row__16073 is null) && !haveProgressIndicator__15946))
                {
                    row__16073 ??= _getProgressIndicatorRowFor(index__15990);
                    haveProgressIndicator__15946 = true;
                }
            }
            if (((PaginatedDataTable)this.widget).showEmptyRows)
            {
                row__16073 ??= _getBlankRowFor(index__15990);
            }
            if ((row__16073 is not null))
            {
                result__15849.Add(row__16073);
            }
        }
        return result__15849;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleFirst()
    {
        pageTo(0L);
    }

    internal virtual void _handlePrevious()
    {
        pageTo(Math.Max((this._firstRowIndex - ((PaginatedDataTable)this.widget).rowsPerPage), 0L));
    }

    internal virtual void _handleNext()
    {
        pageTo((this._firstRowIndex + ((PaginatedDataTable)this.widget).rowsPerPage));
    }

    internal virtual void _handleLast()
    {
        pageTo((((((this._rowCount - 1L)) / ((PaginatedDataTable)this.widget).rowsPerPage)).floor() * ((PaginatedDataTable)this.widget).rowsPerPage));
    }

    internal virtual bool _isNextPageUnavailable() => DartRuntimePrimitives.ConvertValue<bool>((!this._rowCountApproximate && (((this._firstRowIndex + ((PaginatedDataTable)this.widget).rowsPerPage) >= this._rowCount))));
    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        ThemeData themeData__17242 = Theme.of(context);
        MaterialLocalizations localizations__17305 = MaterialLocalizations.of(context);
        var headerWidgets__17380 = new List<global::Doroti.Generated.Framework.Widgets.Widget>();
        if (((this._selectedRowCount == 0L) && (((PaginatedDataTable)this.widget).header is not null)))
        {
            headerWidgets__17380.Add(new global::Doroti.Generated.Framework.Widgets.Expanded(child: ((PaginatedDataTable)this.widget).header!));
        }
        else
        {
            if ((((PaginatedDataTable)this.widget).header is not null))
            {
                headerWidgets__17380.Add(new global::Doroti.Generated.Framework.Widgets.Expanded(child: new global::Doroti.Generated.Framework.Widgets.Text(localizations__17305.selectedRowCountTitle(this._selectedRowCount))));
            }
        }
        if ((((PaginatedDataTable)this.widget).actions is not null))
        {
            headerWidgets__17380.AddRange(((PaginatedDataTable)this.widget).actions!.map<global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.Widget>(((action) => {
return new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: (24.0 - (8.0 * 2.0))), child: action);
throw new InvalidOperationException("Dart closure completed without a value.");
})).ToList().Cast<global::Doroti.Generated.Framework.Widgets.Widget>());
        }
        global::Doroti.Generated.Framework.Painting.TextStyle? footerTextStyle__18086 = themeData__17242.textTheme.bodySmall;
        var footerWidgets__18145 = new List<global::Doroti.Generated.Framework.Widgets.Widget>();
        if ((((PaginatedDataTable)this.widget).onRowsPerPageChanged is not null))
        {
            List<global::Doroti.Generated.Framework.Widgets.Widget> availableRowsPerPage__18245 = ((PaginatedDataTable)this.widget).availableRowsPerPage.where(((value) => ((value <= this._rowCount) || (value == ((PaginatedDataTable)this.widget).rowsPerPage)))).map<long, DropdownMenuItem<long>>(((value) => {
return new DropdownMenuItem<long>(value: value, child: new global::Doroti.Generated.Framework.Widgets.Text($"{value}"));
throw new InvalidOperationException("Dart closure completed without a value.");
})).ToList().Cast<global::Doroti.Generated.Framework.Widgets.Widget>().ToList();
            footerWidgets__18145.AddRange(new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(width: 14.0)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Text(localizations__17305.rowsPerPageTitle)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(minWidth: 64.0), child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerEnd, child: new DropdownButtonHideUnderline(child: new DropdownButton<long>(items: availableRowsPerPage__18245.cast<DropdownMenuItem<long>>().ToList(), value: ((PaginatedDataTable)this.widget).rowsPerPage, onChanged: (value => ((PaginatedDataTable)this.widget).onRowsPerPageChanged?.Invoke(value)), style: footerTextStyle__18086))))) }.Cast<global::Doroti.Generated.Framework.Widgets.Widget>());
        }
        footerWidgets__18145.AddRange(((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection19346 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection19346.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(width: 32.0))); __collection19346.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Text(localizations__17305.pageRowsInfoTitle((this._firstRowIndex + 1L), Math.Min((this._firstRowIndex + ((PaginatedDataTable)this.widget).rowsPerPage), this._rowCount), this._rowCount, this._rowCountApproximate)))); __collection19346.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(width: 32.0))); if (((PaginatedDataTable)this.widget).showFirstLastButtons) { __collection19346.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new IconButton(icon: new global::Doroti.Generated.Framework.Widgets.Icon(Icons.skip_previous), padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.zero, color: ((PaginatedDataTable)this.widget).arrowHeadColor, tooltip: localizations__17305.firstPageTooltip, onPressed: ((global::System.Action)((this._firstRowIndex <= 0L) ? null : this._handleFirst))))); } __collection19346.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new IconButton(icon: new global::Doroti.Generated.Framework.Widgets.Icon(Icons.chevron_left), padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.zero, color: ((PaginatedDataTable)this.widget).arrowHeadColor, tooltip: localizations__17305.previousPageTooltip, onPressed: ((global::System.Action)((this._firstRowIndex <= 0L) ? null : this._handlePrevious))))); __collection19346.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(width: 24.0))); __collection19346.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new IconButton(icon: new global::Doroti.Generated.Framework.Widgets.Icon(Icons.chevron_right), padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.zero, color: ((PaginatedDataTable)this.widget).arrowHeadColor, tooltip: localizations__17305.nextPageTooltip, onPressed: ((global::System.Action)(_isNextPageUnavailable() ? null : this._handleNext))))); if (((PaginatedDataTable)this.widget).showFirstLastButtons) { __collection19346.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new IconButton(icon: new global::Doroti.Generated.Framework.Widgets.Icon(Icons.skip_next), padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.zero, color: ((PaginatedDataTable)this.widget).arrowHeadColor, tooltip: localizations__17305.lastPageTooltip, onPressed: ((global::System.Action)(_isNextPageUnavailable() ? null : this._handleLast))))); } __collection19346.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(width: 14.0))); return __collection19346; }))().Cast<global::Doroti.Generated.Framework.Widgets.Widget>());
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new Card(semanticContainer: false, child: new global::Doroti.Generated.Framework.Widgets.LayoutBuilder(builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Widgets.Widget>)((context, constraints) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Column(crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.stretch, children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection21136 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); if (System.Linq.Enumerable.Any(headerWidgets__17380)) { __collection21136.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Semantics(container: true, child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: ((this._selectedRowCount > 0L) ? themeData__17242.textTheme.titleMedium!.copyWith(color: themeData__17242.colorScheme.secondary) : themeData__17242.textTheme.titleLarge!.copyWith(fontWeight: FontWeight.w400)), child: IconTheme.merge(data: new global::Doroti.Generated.Framework.Widgets.IconThemeData(opacity: 0.54), child: new Ink(height: 64.0, color: ((this._selectedRowCount > 0L) ? themeData__17242.secondaryHeaderColor : null), child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 24, end: 14.0), child: new global::Doroti.Generated.Framework.Widgets.Row(mainAxisAlignment: global::Doroti.Generated.Framework.Rendering.MainAxisAlignment.end, children: headerWidgets__17380)))))))); } __collection21136.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SingleChildScrollView(scrollDirection: global::Doroti.Generated.Framework.Painting.Axis.horizontal, primary: ((PaginatedDataTable)this.widget).primary, controller: ((PaginatedDataTable)this.widget).controller, dragStartBehavior: ((PaginatedDataTable)this.widget).dragStartBehavior, child: new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(minWidth: ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).minWidth), child: new DataTable(key: this._tableKey, columns: ((PaginatedDataTable)this.widget).columns, sortColumnIndex: ((PaginatedDataTable)this.widget).sortColumnIndex, sortAscending: ((PaginatedDataTable)this.widget).sortAscending, onSelectAll: ((PaginatedDataTable)this.widget).onSelectAll, dividerThickness: ((PaginatedDataTable)this.widget).dividerThickness, decoration: new global::Doroti.Generated.Framework.Painting.BoxDecoration(), dataRowMinHeight: ((PaginatedDataTable)this.widget).dataRowMinHeight, dataRowMaxHeight: ((PaginatedDataTable)this.widget).dataRowMaxHeight, headingRowHeight: ((PaginatedDataTable)this.widget).headingRowHeight, horizontalMargin: ((PaginatedDataTable)this.widget).horizontalMargin, checkboxHorizontalMargin: ((PaginatedDataTable)this.widget).checkboxHorizontalMargin, columnSpacing: ((PaginatedDataTable)this.widget).columnSpacing, showCheckboxColumn: ((PaginatedDataTable)this.widget).showCheckboxColumn, showBottomBorder: true, rows: _getRows(this._firstRowIndex, ((PaginatedDataTable)this.widget).rowsPerPage), headingRowColor: ((PaginatedDataTable)this.widget).headingRowColor))))); if (!((PaginatedDataTable)this.widget).showEmptyRows) { __collection21136.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(height: (((((PaginatedDataTable)this.widget).dataRowMaxHeight ?? global::Doroti.Generated.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension)) * (((((PaginatedDataTable)this.widget).rowsPerPage - this._rowCount) + this._firstRowIndex)).clamp(0L, ((PaginatedDataTable)this.widget).rowsPerPage))))); } __collection21136.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: footerTextStyle__18086!, child: IconTheme.merge(data: new global::Doroti.Generated.Framework.Widgets.IconThemeData(opacity: 0.54), child: new global::Doroti.Generated.Framework.Widgets.SizedBox(height: 56.0, child: new global::Doroti.Generated.Framework.Widgets.SingleChildScrollView(dragStartBehavior: ((PaginatedDataTable)this.widget).dragStartBehavior, scrollDirection: global::Doroti.Generated.Framework.Painting.Axis.horizontal, reverse: true, child: new global::Doroti.Generated.Framework.Widgets.Row(children: footerWidgets__18145))))))); return __collection21136; }))()));
throw new InvalidOperationException("Dart closure completed without a value.");
})))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
