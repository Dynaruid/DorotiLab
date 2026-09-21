// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/paginated_data_table.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class PaginatedDataTable : StatefulWidget
{
    public virtual Widget? header { get; private set; }
    public virtual List<Widget>? actions { get; private set; }
    public virtual List<DataColumn> columns { get; private set; } = default!;
    public virtual long? sortColumnIndex { get; private set; }
    public virtual bool sortAscending { get; private set; } = default!;
    public virtual Action<bool?>? onSelectAll { get; private set; }
    public virtual double? dataRowMinHeight { get; private set; }
    public virtual double? dataRowMaxHeight { get; private set; }
    public virtual double headingRowHeight { get; private set; } = default!;
    public virtual double horizontalMargin { get; private set; } = default!;
    public virtual double columnSpacing { get; private set; } = default!;
    public virtual bool showCheckboxColumn { get; private set; } = default!;
    public virtual bool showFirstLastButtons { get; private set; } = default!;
    public virtual long? initialFirstRowIndex { get; private set; }
    public virtual double? dividerThickness { get; private set; }
    public virtual Action<long>? onPageChanged { get; private set; }
    public virtual long rowsPerPage { get; private set; } = default!;
    public const long defaultRowsPerPage = 10L;
    public virtual List<long> availableRowsPerPage { get; private set; } = default!;
    public virtual Action<long?>? onRowsPerPageChanged { get; private set; }
    public virtual DataTableSource source { get; private set; } = default!;
    public virtual Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual double? checkboxHorizontalMargin { get; private set; }
    public virtual Color? arrowHeadColor { get; private set; }
    public virtual ScrollController? controller { get; private set; }
    public virtual bool? primary { get; private set; }
    public virtual WidgetStateProperty<Color?>? headingRowColor { get; private set; }
    public virtual bool showEmptyRows { get; private set; } = default!;

    public PaginatedDataTable(
        Key? key = null,
        Widget? header = null,
        List<Widget>? actions = null,
        List<DataColumn> columns = default!,
        long? sortColumnIndex = null,
        bool sortAscending = true,
        Action<bool?>? onSelectAll = null,
        double? dataRowHeight = null,
        double? dataRowMinHeight = null,
        double? dataRowMaxHeight = null,
        double headingRowHeight = 56.0,
        double horizontalMargin = 24.0,
        double columnSpacing = 56.0,
        bool showCheckboxColumn = true,
        bool showFirstLastButtons = false,
        long? initialFirstRowIndex = 0,
        Action<long>? onPageChanged = null,
        long? rowsPerPage = null,
        List<long> availableRowsPerPage = default!,
        Action<long?>? onRowsPerPageChanged = null,
        Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start,
        Color? arrowHeadColor = null,
        DataTableSource source = default!,
        double? checkboxHorizontalMargin = null,
        ScrollController? controller = null,
        bool? primary = null,
        WidgetStateProperty<Color?>? headingRowColor = null,
        double? dividerThickness = null,
        bool showEmptyRows = true
    )
        : base(key: key)
    {
        long __rowsPerPage = rowsPerPage ?? defaultRowsPerPage;
        List<long> __availableRowsPerPage =
            availableRowsPerPage
            ?? new List<long>
            {
                defaultRowsPerPage,
                defaultRowsPerPage * 2,
                defaultRowsPerPage * 5,
                defaultRowsPerPage * 10,
            };
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
        this.dataRowMinHeight = dataRowHeight ?? dataRowMinHeight;
        this.dataRowMaxHeight = dataRowHeight ?? dataRowMaxHeight;
        System.Diagnostics.Debug.Assert((actions is null) || header is not null);
        System.Diagnostics.Debug.Assert(Enumerable.Any(columns));
        System.Diagnostics.Debug.Assert(
            (sortColumnIndex is null)
                || (
                    (sortColumnIndex >= 0L)
                    && (
                        (
                            sortColumnIndex
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        ) < checked(columns.Count)
                    )
                )
        );
        System.Diagnostics.Debug.Assert(
            (dataRowMinHeight is null)
                || (dataRowMaxHeight is null)
                || (
                    dataRowMaxHeight
                    >= (
                        dataRowMinHeight
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                )
        );
        System.Diagnostics.Debug.Assert(
            (dataRowHeight is null) || ((dataRowMinHeight is null) && (dataRowMaxHeight is null))
        );
        System.Diagnostics.Debug.Assert(__rowsPerPage > 0L);
        System.Diagnostics.Debug.Assert((dividerThickness is null) || (dividerThickness >= 0L));
        System.Diagnostics.Debug.Assert(
            (
                (Func<bool>)(
                    () =>
                    {
                        if (onRowsPerPageChanged is not null)
                        {
                            DartRuntimePrimitives.Assert(() =>
                                __availableRowsPerPage.Contains((__rowsPerPage))
                            );
                        }
                        return true;
                        throw new InvalidOperationException(
                            "Callback completed without returning a value."
                        );
                    }
                )
            )()
        );
        System.Diagnostics.Debug.Assert(!((controller is not null) && (primary ?? false)));
    }

    public virtual double? dataRowHeight =>
        (dataRowMinHeight == dataRowMaxHeight) ? dataRowMinHeight : null;

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new PaginatedDataTableState());
}

public class PaginatedDataTableState : State<PaginatedDataTable>
{
    internal virtual long _firstRowIndex { get; set; } = default!;
    internal virtual long _rowCount { get; set; } = default!;
    internal virtual bool _rowCountApproximate { get; set; } = default!;
    internal virtual long _selectedRowCount { get; set; } = 0L;
    internal virtual DartMap<long, DataRow?> _rows { get; private set; } =
        new DartMap<long, DataRow?>();
    internal virtual GlobalKey<IState> _tableKey { get; private set; } = GlobalKey<IState>.Create();

    public override void initState()
    {
        base.initState();
        _firstRowIndex =
            (
                ((long?)PageStorage.maybeOf(context)?.readState(context))
                ?? widget.initialFirstRowIndex
            ) ?? 0L;
        widget.source.addListener(_handleDataSourceChanged);
        _handleDataSourceChanged();
    }

    public override void didUpdateWidget(PaginatedDataTable oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(oldWidget.source, widget.source))
        {
            oldWidget.source.removeListener(_handleDataSourceChanged);
            widget.source.addListener(_handleDataSourceChanged);
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
        widget.source.removeListener(_handleDataSourceChanged);
        base.dispose();
    }

    internal virtual void _handleDataSourceChanged()
    {
        setState(() => _updateCaches());
    }

    internal virtual void _updateCaches()
    {
        _rowCount = widget.source.rowCount;
        _rowCountApproximate = widget.source.isRowCountApproximate;
        _selectedRowCount = widget.source.selectedRowCount;
        _rows.Clear();
    }

    public virtual void pageTo(long rowIndex)
    {
        long oldFirstRowIndex = _firstRowIndex;
        setState(() =>
        {
            long rowsPerPageLocal = widget.rowsPerPage;
            _firstRowIndex = checked(rowIndex / rowsPerPageLocal) * rowsPerPageLocal;
        });
        if (widget.onPageChanged is not null && oldFirstRowIndex != _firstRowIndex)
        {
            widget.onPageChanged!(_firstRowIndex);
        }
    }

    internal virtual DataRow _getBlankRowFor(long index)
    {
        return DataRow.CreateByIndex(
            index: index,
            cells: widget.columns.map((column) => DataCell.empty).ToList()
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual DataRow _getProgressIndicatorRowFor(long index)
    {
        var haveProgressIndicator = false;
        List<DataCell> cellsLocal = widget
            .columns.map(
                (column) =>
                {
                    if (!column.numeric)
                    {
                        haveProgressIndicator = true;
                        return new DataCell(new CircularProgressIndicator());
                    }
                    return DataCell.empty;
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            )
            .ToList()
            .ToList();
        if (!haveProgressIndicator)
        {
            haveProgressIndicator = true;
            cellsLocal[(int)0L] = new DataCell(new CircularProgressIndicator());
        }
        return DataRow.CreateByIndex(index: index, cells: cellsLocal);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual List<DataRow> _getRows(long firstRowIndex, long rowsPerPage)
    {
        var result = new List<DataRow>();
        long nextPageFirstRowIndex = firstRowIndex + rowsPerPage;
        var haveProgressIndicator = false;
        for (var index = firstRowIndex; index < nextPageFirstRowIndex; index += 1L)
        {
            DataRow? row = default!;
            if ((index < _rowCount) || _rowCountApproximate)
            {
                row = _rows.putIfAbsent(index, () => widget.source.getRow(index));
                if ((row is null) && !haveProgressIndicator)
                {
                    row ??= _getProgressIndicatorRowFor(index);
                    haveProgressIndicator = true;
                }
            }
            if (widget.showEmptyRows)
            {
                row ??= _getBlankRowFor(index);
            }
            if (row is not null)
            {
                result.Add(row);
            }
        }
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _handleFirst()
    {
        pageTo(0L);
    }

    internal virtual void _handlePrevious()
    {
        pageTo(Math.Max(_firstRowIndex - widget.rowsPerPage, 0L));
    }

    internal virtual void _handleNext()
    {
        pageTo(_firstRowIndex + widget.rowsPerPage);
    }

    internal virtual void _handleLast()
    {
        pageTo(((_rowCount - 1L) / widget.rowsPerPage).floor() * widget.rowsPerPage);
    }

    internal virtual bool _isNextPageUnavailable() =>
        DartRuntimePrimitives.ConvertValue<bool>(
            !_rowCountApproximate && (_firstRowIndex + widget.rowsPerPage) >= _rowCount
        );

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckHasMaterialLocalizations(context)
        );
        ThemeData themeData = Theme.of(context);
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        var headerWidgets = new List<Widget>();
        if ((_selectedRowCount == 0L) && (widget.header is not null))
        {
            headerWidgets.Add(new Expanded(child: widget.header!));
        }
        else
        {
            if (widget.header is not null)
            {
                headerWidgets.Add(
                    new Expanded(
                        child: new Text(localizations.selectedRowCountTitle(_selectedRowCount))
                    )
                );
            }
        }
        if (widget.actions is not null)
        {
            headerWidgets.AddRange(
                widget
                    .actions!.map<Widget, Widget>(
                        (action) =>
                        {
                            return new Padding(
                                padding: EdgeInsetsDirectional.CreateOnly(
                                    start: 24.0 - (8.0 * 2.0)
                                ),
                                child: action
                            );
                            throw new InvalidOperationException(
                                "Callback completed without returning a value."
                            );
                        }
                    )
                    .ToList()
                    .Cast<Widget>()
            );
        }
        TextStyle? footerTextStyle = themeData.textTheme.bodySmall;
        var footerWidgets = new List<Widget>();
        if (widget.onRowsPerPageChanged is not null)
        {
            List<Widget> availableRowsPerPageLocal = widget
                .availableRowsPerPage.where(
                    (value) => (value <= _rowCount) || (value == widget.rowsPerPage)
                )
                .map(
                    (value) =>
                    {
                        return new DropdownMenuItem<long>(
                            value: value,
                            child: new Text($"{value}")
                        );
                        throw new InvalidOperationException(
                            "Callback completed without returning a value."
                        );
                    }
                )
                .ToList()
                .Cast<Widget>()
                .ToList();
            footerWidgets.AddRange(
                new List<Widget>
                {
                    DartRuntimePrimitives.ConvertValue<Widget>(new SizedBox(width: 14.0)),
                    DartRuntimePrimitives.ConvertValue<Widget>(
                        new Text(localizations.rowsPerPageTitle)
                    ),
                    DartRuntimePrimitives.ConvertValue<Widget>(
                        new ConstrainedBox(
                            constraints: new BoxConstraints(minWidth: 64.0),
                            child: new Align(
                                alignment: AlignmentDirectional.centerEnd,
                                child: new DropdownButtonHideUnderline(
                                    child: new DropdownButton<long>(
                                        items: availableRowsPerPageLocal
                                            .cast<DropdownMenuItem<long>>()
                                            .ToList(),
                                        value: widget.rowsPerPage,
                                        onChanged: value =>
                                            widget.onRowsPerPageChanged?.Invoke(value),
                                        style: footerTextStyle
                                    )
                                )
                            )
                        )
                    ),
                }.Cast<Widget>()
            );
        }
        footerWidgets.AddRange((
                (Func<List<Widget>>)(
                    () =>
                    {
                        var __collection19346 = new List<Widget>();
                        __collection19346.Add(
                            DartRuntimePrimitives.ConvertValue<Widget>(new SizedBox(width: 32.0))
                        );
                        __collection19346.Add(
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                new Text(
                                    localizations.pageRowsInfoTitle(
                                        _firstRowIndex + 1L,
                                        Math.Min(_firstRowIndex + widget.rowsPerPage, _rowCount),
                                        _rowCount,
                                        _rowCountApproximate
                                    )
                                )
                            )
                        );
                        __collection19346.Add(
                            DartRuntimePrimitives.ConvertValue<Widget>(new SizedBox(width: 32.0))
                        );
                        if (widget.showFirstLastButtons)
                        {
                            __collection19346.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    new IconButton(
                                        icon: new Icon(Icons.skip_previous),
                                        padding: EdgeInsets.zero,
                                        color: widget.arrowHeadColor,
                                        tooltip: localizations.firstPageTooltip,
                                        onPressed: (_firstRowIndex <= 0L) ? null : _handleFirst
                                    )
                                )
                            );
                        }
                        __collection19346.Add(
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                new IconButton(
                                    icon: new Icon(Icons.chevron_left),
                                    padding: EdgeInsets.zero,
                                    color: widget.arrowHeadColor,
                                    tooltip: localizations.previousPageTooltip,
                                    onPressed: (_firstRowIndex <= 0L) ? null : _handlePrevious
                                )
                            )
                        );
                        __collection19346.Add(
                            DartRuntimePrimitives.ConvertValue<Widget>(new SizedBox(width: 24.0))
                        );
                        __collection19346.Add(
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                new IconButton(
                                    icon: new Icon(Icons.chevron_right),
                                    padding: EdgeInsets.zero,
                                    color: widget.arrowHeadColor,
                                    tooltip: localizations.nextPageTooltip,
                                    onPressed: _isNextPageUnavailable() ? null : _handleNext
                                )
                            )
                        );
                        if (widget.showFirstLastButtons)
                        {
                            __collection19346.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    new IconButton(
                                        icon: new Icon(Icons.skip_next),
                                        padding: EdgeInsets.zero,
                                        color: widget.arrowHeadColor,
                                        tooltip: localizations.lastPageTooltip,
                                        onPressed: _isNextPageUnavailable() ? null : _handleLast
                                    )
                                )
                            );
                        }
                        __collection19346.Add(
                            DartRuntimePrimitives.ConvertValue<Widget>(new SizedBox(width: 14.0))
                        );
                        return __collection19346;
                    }
                )
            )().Cast<Widget>());
        return new Card(
            semanticContainer: false,
            child: new LayoutBuilder(
                builder: (context, constraints) =>
                {
                    return new Column(
                        crossAxisAlignment: CrossAxisAlignment.stretch,
                        children: (
                            (Func<List<Widget>>)(
                                () =>
                                {
                                    var __collection21136 = new List<Widget>();
                                    if (Enumerable.Any(headerWidgets))
                                    {
                                        __collection21136.Add(
                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                new Widgets.Semantics(
                                                    container: true,
                                                    child: new DefaultTextStyle(
                                                        style: (_selectedRowCount > 0L)
                                                            ? themeData.textTheme.titleMedium!.copyWith(
                                                                color: themeData
                                                                    .colorScheme
                                                                    .secondary
                                                            )
                                                            : themeData.textTheme.titleLarge!.copyWith(
                                                                fontWeight: FontWeight.w400
                                                            ),
                                                        child: IconTheme.merge(
                                                            data: new IconThemeData(opacity: 0.54),
                                                            child: new Ink(
                                                                height: 64.0,
                                                                color: (_selectedRowCount > 0L)
                                                                    ? themeData.secondaryHeaderColor
                                                                    : null,
                                                                child: new Padding(
                                                                    padding: EdgeInsetsDirectional.CreateOnly(
                                                                        start: 24,
                                                                        end: 14.0
                                                                    ),
                                                                    child: new Row(
                                                                        mainAxisAlignment: MainAxisAlignment.end,
                                                                        children: headerWidgets
                                                                    )
                                                                )
                                                            )
                                                        )
                                                    )
                                                )
                                            )
                                        );
                                    }
                                    __collection21136.Add(
                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                            new SingleChildScrollView(
                                                scrollDirection: Axis.horizontal,
                                                primary: widget.primary,
                                                controller: widget.controller,
                                                dragStartBehavior: widget.dragStartBehavior,
                                                child: new ConstrainedBox(
                                                    constraints: new BoxConstraints(
                                                        minWidth: constraints.minWidth
                                                    ),
                                                    child: new DataTable(
                                                        key: _tableKey,
                                                        columns: widget.columns,
                                                        sortColumnIndex: widget.sortColumnIndex,
                                                        sortAscending: widget.sortAscending,
                                                        onSelectAll: widget.onSelectAll,
                                                        dividerThickness: widget.dividerThickness,
                                                        decoration: new BoxDecoration(),
                                                        dataRowMinHeight: widget.dataRowMinHeight,
                                                        dataRowMaxHeight: widget.dataRowMaxHeight,
                                                        headingRowHeight: widget.headingRowHeight,
                                                        horizontalMargin: widget.horizontalMargin,
                                                        checkboxHorizontalMargin: widget.checkboxHorizontalMargin,
                                                        columnSpacing: widget.columnSpacing,
                                                        showCheckboxColumn: widget.showCheckboxColumn,
                                                        showBottomBorder: true,
                                                        rows: _getRows(
                                                            _firstRowIndex,
                                                            widget.rowsPerPage
                                                        ),
                                                        headingRowColor: widget.headingRowColor
                                                    )
                                                )
                                            )
                                        )
                                    );
                                    if (!widget.showEmptyRows)
                                    {
                                        __collection21136.Add(
                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                new SizedBox(
                                                    height: (
                                                        widget.dataRowMaxHeight
                                                        ?? Widgets
                                                            .ConstantsLibrary
                                                            .kMinInteractiveDimension
                                                    )
                                                        * (
                                                            widget.rowsPerPage
                                                            - _rowCount
                                                            + _firstRowIndex
                                                        ).clamp(0L, widget.rowsPerPage)
                                                )
                                            )
                                        );
                                    }
                                    __collection21136.Add(
                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                            new DefaultTextStyle(
                                                style: footerTextStyle!,
                                                child: IconTheme.merge(
                                                    data: new IconThemeData(opacity: 0.54),
                                                    child: new SizedBox(
                                                        height: 56.0,
                                                        child: new SingleChildScrollView(
                                                            dragStartBehavior: widget.dragStartBehavior,
                                                            scrollDirection: Axis.horizontal,
                                                            reverse: true,
                                                            child: new Row(children: footerWidgets)
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                    );
                                    return __collection21136;
                                }
                            )
                        )()
                    );
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
