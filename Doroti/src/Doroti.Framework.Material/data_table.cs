// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/data_table.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public delegate void DataColumnSortCallback(long columnIndex, bool ascending);

public class DataColumn
{
    public virtual Widget label { get; private set; } = default!;
    public virtual TableColumnWidth? columnWidth { get; private set; }
    public virtual string? tooltip { get; private set; }
    public virtual bool numeric { get; private set; } = default!;
    public virtual Action<long, bool>? onSort { get; private set; }
    public virtual WidgetStateProperty<MouseCursor?>? mouseCursor { get; private set; }
    public virtual MainAxisAlignment? headingRowAlignment { get; private set; }

    public DataColumn(
        Widget label,
        TableColumnWidth? columnWidth = null,
        string? tooltip = null,
        bool numeric = false,
        Action<long, bool>? onSort = null,
        WidgetStateProperty<MouseCursor?>? mouseCursor = null,
        MainAxisAlignment? headingRowAlignment = null
    )
    {
        this.label = label;
        this.columnWidth = columnWidth;
        this.tooltip = tooltip;
        this.numeric = numeric;
        this.onSort = onSort;
        this.mouseCursor = mouseCursor;
        this.headingRowAlignment = headingRowAlignment;
    }

    internal virtual bool _debugInteractive =>
        DartRuntimePrimitives.ConvertValue<bool>(onSort is not null);
}

public class DataRow
{
    public virtual LocalKey? key { get; private set; }
    public virtual Action<bool?>? onSelectChanged { get; private set; }
    public virtual Action? onLongPress { get; private set; }
    public virtual Action<bool>? onHover { get; private set; }
    public virtual bool selected { get; private set; } = default!;
    public virtual List<DataCell> cells { get; private set; } = default!;
    public virtual WidgetStateProperty<Color?>? color { get; private set; }
    public virtual WidgetStateProperty<MouseCursor?>? mouseCursor { get; private set; }

    public DataRow(
        LocalKey? key = null,
        bool selected = false,
        Action<bool?>? onSelectChanged = null,
        Action? onLongPress = null,
        Action<bool>? onHover = null,
        WidgetStateProperty<Color?>? color = null,
        WidgetStateProperty<MouseCursor?>? mouseCursor = null,
        List<DataCell> cells = default!
    )
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

    public static DataRow CreateByIndex(
        long? index = null,
        bool selected = false,
        Action<bool?>? onSelectChanged = null,
        Action? onLongPress = null,
        Action<bool>? onHover = null,
        WidgetStateProperty<Color?>? color = null,
        WidgetStateProperty<MouseCursor?>? mouseCursor = null,
        List<DataCell> cells = default!
    )
    {
        var __instance = new DataRow(
            selected: selected,
            onSelectChanged: onSelectChanged,
            onLongPress: onLongPress,
            onHover: onHover,
            color: color,
            mouseCursor: mouseCursor,
            cells: cells
        );
        __instance.selected = selected;
        __instance.onSelectChanged = onSelectChanged;
        __instance.onLongPress = onLongPress;
        __instance.onHover = onHover;
        __instance.color = color;
        __instance.mouseCursor = mouseCursor;
        __instance.cells = cells;
        __instance.key = new ValueKey<long?>(index);
        return __instance;
    }

    internal virtual bool _debugInteractive =>
        DartRuntimePrimitives.ConvertValue<bool>(
            (onSelectChanged is not null) || cells.any((cell) => cell._debugInteractive)
        );
}

public class DataCell
{
    public static DataCell empty = new DataCell(SizedBox.CreateShrink());
    public virtual Widget child { get; private set; } = default!;
    public virtual bool placeholder { get; private set; } = default!;
    public virtual bool showEditIcon { get; private set; } = default!;
    public virtual Action? onTap { get; private set; }
    public virtual Action? onDoubleTap { get; private set; }
    public virtual Action? onLongPress { get; private set; }
    public virtual Action<Gestures.TapDownDetails>? onTapDown { get; private set; }
    public virtual Action? onTapCancel { get; private set; }

    public DataCell(
        Widget child,
        bool placeholder = false,
        bool showEditIcon = false,
        Action? onTap = null,
        Action? onLongPress = null,
        Action<Gestures.TapDownDetails>? onTapDown = null,
        Action? onDoubleTap = null,
        Action? onTapCancel = null
    )
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

    internal virtual bool _debugInteractive =>
        DartRuntimePrimitives.ConvertValue<bool>(
            (onTap is not null)
                || (onDoubleTap is not null)
                || (onLongPress is not null)
                || (onTapDown is not null)
                || (onTapCancel is not null)
        );
}

public class DataTable : StatelessWidget
{
    public virtual List<DataColumn> columns { get; private set; } = default!;
    public virtual long? sortColumnIndex { get; private set; }
    public virtual bool sortAscending { get; private set; } = default!;
    public virtual Action<bool?>? onSelectAll { get; private set; }
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
    public virtual bool showCheckboxColumn { get; private set; } = default!;
    public virtual List<DataRow> rows { get; private set; } = default!;
    public virtual double? dividerThickness { get; private set; }
    public virtual bool showBottomBorder { get; private set; } = default!;
    public virtual double? checkboxHorizontalMargin { get; private set; }
    public virtual TableBorder? border { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    internal virtual long? _onlyTextColumn { get; private set; }
    internal static LocalKey _headingRowKey = new UniqueKey();
    internal const double _headingRowHeight = 56.0;
    internal const double _horizontalMargin = 24.0;
    internal const double _columnSpacing = 56.0;
    internal const double _sortArrowPadding = 2.0;
    internal const double _dividerThickness = 1.0;
    internal static Duration _sortArrowAnimationDuration = Duration.Create(milliseconds: 150L);

    public DataTable(
        Key? key = null,
        List<DataColumn> columns = default!,
        long? sortColumnIndex = null,
        bool sortAscending = true,
        Action<bool?>? onSelectAll = null,
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
        bool showCheckboxColumn = true,
        bool showBottomBorder = false,
        double? dividerThickness = null,
        List<DataRow> rows = default!,
        double? checkboxHorizontalMargin = null,
        TableBorder? border = null,
        Clip clipBehavior = Clip.none
    )
        : base(key: key)
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
        this.dataRowMinHeight = dataRowHeight ?? dataRowMinHeight;
        this.dataRowMaxHeight = dataRowHeight ?? dataRowMaxHeight;
        _onlyTextColumn = _initOnlyTextColumn(columns);
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
            !rows.any((row) => checked(row.cells.Count) != checked((long)columns.Count))
        );
        System.Diagnostics.Debug.Assert((dividerThickness is null) || (dividerThickness >= 0L));
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
    }

    public virtual double? dataRowHeight =>
        (dataRowMinHeight == dataRowMaxHeight) ? dataRowMinHeight : null;

    internal static long? _initOnlyTextColumn(List<DataColumn> columns)
    {
        long? result = default!;
        for (var index = 0L; index < checked(columns.Count); index += 1L)
        {
            DataColumn column = columns[(int)index];
            if (!column.numeric)
            {
                if (result is not null)
                {
                    long result__30086__value30245 = (
                        result
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    );
                    return null;
                }
                result = index;
            }
        }
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual bool _debugInteractive
    {
        get
        {
            return columns.any((column) => column._debugInteractive)
                || rows.any((row) => row._debugInteractive);
        }
    }

    internal virtual void _handleSelectAll(bool? @checked, bool someChecked)
    {
        bool effectiveChecked = someChecked || (@checked ?? false);
        if (onSelectAll is not null)
        {
            onSelectAll!(effectiveChecked);
        }
        else
        {
            foreach (DataRow row in rows)
            {
                if ((row.onSelectChanged is not null) && (row.selected != effectiveChecked))
                {
                    row.onSelectChanged!(effectiveChecked);
                }
            }
        }
    }

    internal virtual Widget _buildCheckbox(
        BuildContext context,
        bool? @checked,
        Action? onRowTap,
        Action<bool?>? onCheckboxChanged,
        WidgetStateProperty<Color?>? overlayColor,
        bool tristate,
        MouseCursor? rowMouseCursor = null
    )
    {
        ThemeData themeData = Theme.of(context);
        double effectiveHorizontalMargin =
            (horizontalMargin ?? themeData.dataTableTheme.horizontalMargin) ?? _horizontalMargin;
        double effectiveCheckboxHorizontalMarginStart =
            (checkboxHorizontalMargin ?? themeData.dataTableTheme.checkboxHorizontalMargin)
            ?? effectiveHorizontalMargin;
        double effectiveCheckboxHorizontalMarginEnd =
            (checkboxHorizontalMargin ?? themeData.dataTableTheme.checkboxHorizontalMargin)
            ?? (effectiveHorizontalMargin / 2.0);
        Widget contents = new Widgets.Semantics(
            container: true,
            child: new Padding(
                padding: EdgeInsetsDirectional.CreateOnly(
                    start: effectiveCheckboxHorizontalMarginStart,
                    end: effectiveCheckboxHorizontalMarginEnd
                ),
                child: new Center(
                    child: new Checkbox(
                        value: @checked,
                        onChanged: onCheckboxChanged,
                        tristate: tristate
                    )
                )
            )
        );
        if (onRowTap is not null)
        {
            contents = DartRuntimePrimitives.ConvertValue<Widget>(
                new TableRowInkWell(
                    onTap: () => onRowTap(),
                    overlayColor: overlayColor,
                    mouseCursor: rowMouseCursor,
                    child: contents
                )
            );
        }
        return new TableCell(verticalAlignment: TableCellVerticalAlignment.fill, child: contents);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildHeadingCell(
        BuildContext context,
        EdgeInsetsGeometry padding,
        Widget label,
        string? tooltip,
        bool numeric,
        Action? onSort,
        bool sorted,
        bool ascending,
        WidgetStateProperty<Color?>? overlayColor,
        MouseCursor? mouseCursor,
        MainAxisAlignment headingRowAlignment
    )
    {
        ThemeData themeData = Theme.of(context);
        DataTableThemeData dataTableThemeLocal = DataTableTheme.of(context);
        label = new Widgets.Semantics(
            role: SemanticsRole.columnHeader,
            child: new Row(
                textDirection: numeric ? TextDirection.rtl : null,
                mainAxisAlignment: headingRowAlignment,
                children: (
                    (Func<List<Widget>>)(
                        () =>
                        {
                            var __collection34143 = new List<Widget>();
                            if (
                                Equals(headingRowAlignment, MainAxisAlignment.center)
                                && (onSort is not null)
                            )
                            {
                                __collection34143.Add(
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        new SizedBox(
                                            width: _SortArrowState__data_table._arrowIconSize
                                                + _sortArrowPadding
                                        )
                                    )
                                );
                            }
                            __collection34143.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(label)
                            );
                            if (onSort is not null)
                            {
                                __collection34143.AddRange(
                                    new List<Widget>
                                    {
                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                            new _SortArrow__data_table(
                                                visible: sorted,
                                                up: sorted ? ascending : null,
                                                duration: _sortArrowAnimationDuration
                                            )
                                        ),
                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                            new SizedBox(width: _sortArrowPadding)
                                        ),
                                    }
                                );
                            }
                            return __collection34143;
                        }
                    )
                )()
            )
        );
        TextStyle effectiveHeadingTextStyle =
            (
                (headingTextStyle ?? dataTableThemeLocal.headingTextStyle)
                ?? themeData.dataTableTheme.headingTextStyle
            ) ?? themeData.textTheme.titleSmall!;
        double effectiveHeadingRowHeight =
            (
                (headingRowHeight ?? dataTableThemeLocal.headingRowHeight)
                ?? themeData.dataTableTheme.headingRowHeight
            ) ?? _headingRowHeight;
        label = new Container(
            padding: padding,
            height: effectiveHeadingRowHeight,
            alignment: numeric
                ? global::Doroti.Framework.Painting.Alignment.centerRight
                : global::Doroti.Framework.Painting.AlignmentDirectional.centerStart,
            child: new AnimatedDefaultTextStyle(
                style: DefaultTextStyle.of(context).style.merge(effectiveHeadingTextStyle),
                softWrap: false,
                duration: _sortArrowAnimationDuration,
                child: label
            )
        );
        if (tooltip is not null)
        {
            label = new Tooltip(message: tooltip, child: label);
        }
        label = new InkWell(
            onTap: onSort,
            overlayColor: overlayColor,
            mouseCursor: mouseCursor,
            child: label
        );
        return label;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildDataCell(
        BuildContext context,
        EdgeInsetsGeometry padding,
        Widget label,
        bool numeric,
        bool placeholder,
        bool showEditIcon,
        Action? onTap,
        Action? onSelectChanged,
        Action? onDoubleTap,
        Action? onLongPress,
        Action<Gestures.TapDownDetails>? onTapDown,
        Action? onTapCancel,
        WidgetStateProperty<Color?>? overlayColor,
        Action? onRowLongPress,
        Action<bool>? onRowHover,
        MouseCursor? mouseCursor
    )
    {
        ThemeData themeData = Theme.of(context);
        DataTableThemeData dataTableThemeLocal = DataTableTheme.of(context);
        if (showEditIcon)
        {
            Widget icon = new Icon(Icons.edit, size: 18.0);
            label = new Expanded(child: label);
            label = new Row(
                textDirection: numeric ? TextDirection.rtl : null,
                children: new List<Widget>
                {
                    DartRuntimePrimitives.ConvertValue<Widget>(label),
                    DartRuntimePrimitives.ConvertValue<Widget>(icon),
                }
            );
        }
        TextStyle effectiveDataTextStyle =
            (
                (dataTextStyle ?? dataTableThemeLocal.dataTextStyle)
                ?? themeData.dataTableTheme.dataTextStyle
            ) ?? themeData.textTheme.bodyMedium!;
        double effectiveDataRowMinHeight =
            (
                (dataRowMinHeight ?? dataTableThemeLocal.dataRowMinHeight)
                ?? themeData.dataTableTheme.dataRowMinHeight
            ) ?? ConstantsLibrary.kMinInteractiveDimension;
        double effectiveDataRowMaxHeight =
            (
                (dataRowMaxHeight ?? dataTableThemeLocal.dataRowMaxHeight)
                ?? themeData.dataTableTheme.dataRowMaxHeight
            ) ?? ConstantsLibrary.kMinInteractiveDimension;
        label = new Container(
            padding: padding,
            constraints: new BoxConstraints(
                minHeight: effectiveDataRowMinHeight,
                maxHeight: effectiveDataRowMaxHeight
            ),
            alignment: numeric
                ? global::Doroti.Framework.Painting.Alignment.centerRight
                : global::Doroti.Framework.Painting.AlignmentDirectional.centerStart,
            child: new DefaultTextStyle(
                style: DefaultTextStyle
                    .of(context)
                    .style.merge(effectiveDataTextStyle)
                    .copyWith(
                        color: placeholder ? effectiveDataTextStyle.color!.withOpacity(0.6) : null
                    ),
                child: new DropdownButtonHideUnderline(child: label)
            )
        );
        if (
            (onTap is not null)
            || (onDoubleTap is not null)
            || (onLongPress is not null)
            || (onTapDown is not null)
            || (onTapCancel is not null)
        )
        {
            label = new InkWell(
                onTap: onTap,
                onDoubleTap: onDoubleTap,
                onLongPress: onLongPress,
                onTapCancel: onTapCancel,
                onTapDown: onTapDown,
                overlayColor: overlayColor,
                child: label
            );
        }
        else
        {
            if (
                (onSelectChanged is not null)
                || (onRowLongPress is not null)
                || (onRowHover is not null)
            )
            {
                label = new TableRowInkWell(
                    onTap: onSelectChanged,
                    onLongPress: onRowLongPress,
                    onHover: onRowHover,
                    overlayColor: overlayColor,
                    mouseCursor: mouseCursor,
                    child: label
                );
            }
        }
        return new TableCell(child: label);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            !_debugInteractive || DebugLibrary.debugCheckHasMaterial(context)
        );
        ThemeData theme = Theme.of(context);
        DataTableThemeData dataTableThemeLocal = DataTableTheme.of(context);
        WidgetStateProperty<Color?>? effectiveHeadingRowColor =
            (headingRowColor ?? dataTableThemeLocal.headingRowColor)
            ?? theme.dataTableTheme.headingRowColor;
        WidgetStateProperty<Color?>? effectiveDataRowColor =
            (dataRowColor ?? dataTableThemeLocal.dataRowColor) ?? theme.dataTableTheme.dataRowColor;
        WidgetStateProperty<Color?> defaultRowColor = WidgetStateProperty.resolveWith(
            (states) =>
            {
                if (states.Contains(WidgetState.selected))
                {
                    return theme.colorScheme.primary.withOpacity(0.08);
                }
                return null;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        bool anyRowSelectable = rows.any((row) => row.onSelectChanged is not null);
        bool displayCheckboxColumn = showCheckboxColumn && anyRowSelectable;
        IEnumerable<DataRow> rowsWithCheckbox = displayCheckboxColumn
            ? rows.where((row) => row.onSelectChanged is not null)
            : new List<DataRow>();
        IEnumerable<DataRow> rowsChecked = rowsWithCheckbox.where((row) => row.selected);
        bool allChecked =
            displayCheckboxColumn && (rowsChecked.Count() == rowsWithCheckbox.Count());
        bool anyChecked = displayCheckboxColumn && Enumerable.Any(rowsChecked);
        bool someChecked = anyChecked && !allChecked;
        double effectiveHorizontalMargin =
            (
                (horizontalMargin ?? dataTableThemeLocal.horizontalMargin)
                ?? theme.dataTableTheme.horizontalMargin
            ) ?? _horizontalMargin;
        double effectiveCheckboxHorizontalMarginStart =
            (
                (checkboxHorizontalMargin ?? dataTableThemeLocal.checkboxHorizontalMargin)
                ?? theme.dataTableTheme.checkboxHorizontalMargin
            ) ?? effectiveHorizontalMargin;
        double effectiveCheckboxHorizontalMarginEnd =
            (
                (checkboxHorizontalMargin ?? dataTableThemeLocal.checkboxHorizontalMargin)
                ?? theme.dataTableTheme.checkboxHorizontalMargin
            ) ?? (effectiveHorizontalMargin / 2.0);
        double effectiveColumnSpacing =
            (
                (columnSpacing ?? dataTableThemeLocal.columnSpacing)
                ?? theme.dataTableTheme.columnSpacing
            ) ?? _columnSpacing;
        var tableColumns = new List<TableColumnWidth>(
            Enumerable.Repeat<TableColumnWidth>(
                new _NullTableColumnWidth__data_table(),
                checked((int)(checked(columns.Count) + (displayCheckboxColumn ? 1L : 0L)))
            )
        );
        var tableRows = new List<TableRow>(
            Enumerable.Select(
                Enumerable.Range(0, checked((int)(checked(rows.Count) + 1L))),
                (index) =>
                {
                    bool isSelected = (index > 0L) && rows[(int)(index - 1L)].selected;
                    bool isDisabled =
                        (index > 0L)
                        && anyRowSelectable
                        && (rows[(int)(index - 1L)].onSelectChanged is null);
                    var statesLocal = (
                        (Func<HashSet<WidgetState>>)(
                            () =>
                            {
                                var __collection41545 = new HashSet<WidgetState>();
                                if (isSelected)
                                {
                                    __collection41545.Add(WidgetState.selected);
                                }
                                if (isDisabled)
                                {
                                    __collection41545.Add(WidgetState.disabled);
                                }
                                return __collection41545;
                            }
                        )
                    )();
                    Color? resolvedDataRowColor =
                        (index > 0L)
                            ? (rows[(int)(index - 1L)].color ?? effectiveDataRowColor)?.resolve(
                                statesLocal
                            )
                            : null;
                    Color? resolvedHeadingRowColor = effectiveHeadingRowColor?.resolve(
                        new HashSet<WidgetState>()
                    );
                    var rowColor = (index > 0L) ? resolvedDataRowColor : resolvedHeadingRowColor;
                    BorderSide borderSide = Divider.createBorderSide(
                        context,
                        width: (
                            (dividerThickness ?? dataTableThemeLocal.dividerThickness)
                            ?? theme.dataTableTheme.dividerThickness
                        ) ?? _dividerThickness
                    );
                    Border? borderLocal = showBottomBorder
                        ? new Border(bottom: borderSide)
                        : ((index == 0L) ? null : new Border(top: borderSide));
                    return new TableRow(
                        key: (index == 0L) ? _headingRowKey : rows[(int)(index - 1L)].key,
                        decoration: new BoxDecoration(
                            border: borderLocal,
                            color: rowColor ?? defaultRowColor.resolve(statesLocal)
                        ),
                        children: new List<Widget>(
                            Enumerable.Repeat<Widget>(
                                new _NullWidget__data_table(),
                                checked((int)checked((long)tableColumns.Count))
                            )
                        )
                    );
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            )
        );
        long rowIndex = default!;
        var displayColumnIndex = 0L;
        if (displayCheckboxColumn)
        {
            tableColumns[(int)0L] = DartRuntimePrimitives.ConvertValue<TableColumnWidth>(
                new FixedColumnWidth(
                    effectiveCheckboxHorizontalMarginStart
                        + Checkbox.width
                        + effectiveCheckboxHorizontalMarginEnd
                )
            );
            tableRows[(int)0L].children[(int)0L] = _buildCheckbox(
                context: context,
                @checked: someChecked ? null : allChecked,
                onRowTap: null,
                onCheckboxChanged: (@checked) =>
                {
                    _handleSelectAll(@checked, someChecked);
                },
                overlayColor: null,
                tristate: true
            );
            rowIndex = 1L;
            foreach (DataRow rowLocal in rows)
            {
                var statesAlternate = (
                    (Func<HashSet<WidgetState>>)(
                        () =>
                        {
                            var __collection43445 = new HashSet<WidgetState>();
                            if (rowLocal.selected)
                            {
                                __collection43445.Add(WidgetState.selected);
                            }
                            return __collection43445;
                        }
                    )
                )();
                tableRows[(int)rowIndex].children[(int)0L] = _buildCheckbox(
                    context: context,
                    @checked: rowLocal.selected,
                    onRowTap: (rowLocal.onSelectChanged is null)
                        ? null
                        : (
                            () =>
                            {
                                rowLocal.onSelectChanged?.Invoke(!rowLocal.selected);
                            }
                        ),
                    onCheckboxChanged: rowLocal.onSelectChanged,
                    overlayColor: rowLocal.color ?? effectiveDataRowColor,
                    rowMouseCursor: rowLocal.mouseCursor?.resolve(statesAlternate)
                        ?? (dataTableThemeLocal.dataRowCursor?.resolve(statesAlternate)),
                    tristate: false
                );
                rowIndex += 1L;
            }
            displayColumnIndex += 1L;
        }
        for (
            var dataColumnIndex = 0L;
            dataColumnIndex < checked(columns.Count);
            dataColumnIndex += 1L
        )
        {
            DataColumn column = columns[(int)dataColumnIndex];
            double paddingStart = dataColumnIndex switch
            {
                0L when displayCheckboxColumn && (checkboxHorizontalMargin is null) =>
                    effectiveHorizontalMargin / 2.0,
                0L => effectiveHorizontalMargin,
                _ => effectiveColumnSpacing / 2.0,
            };
            double paddingEnd = default!;
            if (dataColumnIndex == (checked(columns.Count) - 1L))
            {
                paddingEnd = effectiveHorizontalMargin;
            }
            else
            {
                paddingEnd = effectiveColumnSpacing / 2.0;
            }
            var paddingLocal = EdgeInsetsDirectional.CreateOnly(
                start: paddingStart,
                end: paddingEnd
            );
            if (column.columnWidth is not null)
            {
                tableColumns[(int)displayColumnIndex] = column.columnWidth!;
            }
            else
            {
                if (dataColumnIndex == _onlyTextColumn)
                {
                    tableColumns[(int)displayColumnIndex] =
                        DartRuntimePrimitives.ConvertValue<TableColumnWidth>(
                            new IntrinsicColumnWidth(flex: 1.0)
                        );
                }
                else
                {
                    tableColumns[(int)displayColumnIndex] =
                        DartRuntimePrimitives.ConvertValue<TableColumnWidth>(
                            new IntrinsicColumnWidth()
                        );
                }
            }
            var headerStates = (
                (Func<HashSet<WidgetState>>)(
                    () =>
                    {
                        var __collection45174 = new HashSet<WidgetState>();
                        if (column.onSort is null)
                        {
                            __collection45174.Add(WidgetState.disabled);
                        }
                        return __collection45174;
                    }
                )
            )();
            tableRows[(int)0L].children[(int)displayColumnIndex] = _buildHeadingCell(
                context: context,
                padding: paddingLocal,
                label: column.label,
                tooltip: column.tooltip,
                numeric: column.numeric,
                onSort: (column.onSort is not null)
                    ? (
                        () =>
                        {
                            column.onSort!(
                                dataColumnIndex,
                                (sortColumnIndex != dataColumnIndex) || !sortAscending
                            );
                        }
                    )
                    : null,
                sorted: dataColumnIndex == sortColumnIndex,
                ascending: sortAscending,
                overlayColor: effectiveHeadingRowColor,
                mouseCursor: column.mouseCursor?.resolve(headerStates)
                    ?? (dataTableThemeLocal.headingCellCursor?.resolve(headerStates)),
                headingRowAlignment: (
                    column.headingRowAlignment ?? dataTableThemeLocal.headingRowAlignment
                ) ?? MainAxisAlignment.start
            );
            rowIndex = 1L;
            foreach (DataRow rowAlternate in rows)
            {
                var statesNested = (
                    (Func<HashSet<WidgetState>>)(
                        () =>
                        {
                            var __collection46198 = new HashSet<WidgetState>();
                            if (rowAlternate.selected)
                            {
                                __collection46198.Add(WidgetState.selected);
                            }
                            return __collection46198;
                        }
                    )
                )();
                DataCell cell = rowAlternate.cells[(int)dataColumnIndex];
                tableRows[(int)rowIndex].children[(int)displayColumnIndex] = _buildDataCell(
                    context: context,
                    padding: paddingLocal,
                    label: cell.child,
                    numeric: column.numeric,
                    placeholder: cell.placeholder,
                    showEditIcon: cell.showEditIcon,
                    onTap: cell.onTap,
                    onDoubleTap: cell.onDoubleTap,
                    onLongPress: cell.onLongPress,
                    onTapCancel: cell.onTapCancel,
                    onTapDown: cell.onTapDown,
                    onSelectChanged: (rowAlternate.onSelectChanged is null)
                        ? null
                        : (
                            () =>
                            {
                                rowAlternate.onSelectChanged?.Invoke(!rowAlternate.selected);
                            }
                        ),
                    overlayColor: rowAlternate.color ?? effectiveDataRowColor,
                    onRowLongPress: rowAlternate.onLongPress,
                    onRowHover: rowAlternate.onHover,
                    mouseCursor: rowAlternate.mouseCursor?.resolve(statesNested)
                        ?? (dataTableThemeLocal.dataRowCursor?.resolve(statesNested))
                );
                rowIndex += 1L;
            }
            displayColumnIndex += 1L;
        }
        return new Container(
            decoration: (decoration ?? dataTableThemeLocal.decoration)
                ?? theme.dataTableTheme.decoration,
            child: new Material(
                type: MaterialType.transparency,
                borderRadius: border?.borderRadius,
                clipBehavior: clipBehavior,
                child: new Table(
                    columnWidths: tableColumns.asMap(),
                    defaultVerticalAlignment: TableCellVerticalAlignment.middle,
                    children: tableRows,
                    border: border
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class TableRowInkWell : InkResponse
{
    public TableRowInkWell(
        Key? key = null,
        Widget? child = null,
        Action? onTap = null,
        Action? onDoubleTap = null,
        Action? onLongPress = null,
        Action<bool>? onHighlightChanged = null,
        Action<bool>? onHover = null,
        Action? onSecondaryTap = null,
        Action<Gestures.TapDownDetails>? onSecondaryTapDown = null,
        WidgetStateProperty<Color?>? overlayColor = null,
        MouseCursor? mouseCursor = null
    )
        : base(
            key: key,
            child: child,
            onTap: onTap,
            onDoubleTap: onDoubleTap,
            onLongPress: onLongPress,
            onHighlightChanged: onHighlightChanged,
            onHover: onHover,
            onSecondaryTap: onSecondaryTap,
            onSecondaryTapDown: onSecondaryTapDown,
            overlayColor: overlayColor,
            mouseCursor: mouseCursor,
            containedInkWell: true,
            highlightShape: BoxShape.rectangle
        ) { }

    public override Func<Rect>? getRectCallback(RenderBox referenceBox)
    {
        return () =>
        {
            RenderObject cell = referenceBox;
            RenderObject? table = cell.parent;
            var transform = Matrix4.identity();
            while (table is not null && table is not RenderTable)
            {
                table.applyPaintTransform(cell, transform);
                DartRuntimePrimitives.Assert(() => Equals(table, cell.parent));
                cell = table;
                table = table.parent;
            }
            if (table is RenderTable)
            {
                RenderTable table__49236__as49524 = (RenderTable)table;
                var cellParentData = ((TableCellParentData?)cell.parentData!)!;
                DartRuntimePrimitives.Assert(() => cellParentData.y is not null);
                Rect rect = table__49236__as49524.getRowBox(
                    (
                        cellParentData.y
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                );
                table__49236__as49524.applyPaintTransform(cell, transform);
                Offset? offset = MatrixUtils.getAsTranslation(transform);
                if (offset is not null)
                {
                    Offset offset__49929__value49991 = (
                        offset
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    );
                    return rect.shift(-(offset__49929__value49991));
                }
            }
            return Rect.zero;
            throw new InvalidOperationException("Callback completed without returning a value.");
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool debugCheckContext(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasTable(context));
        return base.debugCheckContext(context);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class _SortArrow__data_table : StatefulWidget
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

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _SortArrowState__data_table());
}

public class _SortArrowState__data_table
    : State<_SortArrow__data_table>,
        TickerProviderStateMixin<_SortArrow__data_table>
{
    internal virtual AnimationController _opacityController { get; private set; } = default!;
    internal virtual CurvedAnimation _opacityAnimation { get; private set; } = default!;
    internal virtual AnimationController _orientationController { get; private set; } = default!;
    internal virtual Animation<double> _orientationAnimation { get; private set; } = default!;
    internal virtual double _orientationOffset { get; set; } = 0.0;
    internal virtual bool? _up { get; set; } = default;
    internal static Animatable<double> _turnTween = new Tween<double>(
        begin: 0.0,
        end: Dart_mathLibrary.pi
    ).chain(new CurveTween(curve: Curves.easeIn));
    internal static double _arrowIconBaselineOffset = -1.5;
    internal const double _arrowIconSize = 16.0;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _up = widget.up;
        _opacityAnimation = (
            (Func<CurvedAnimation>)(
                () =>
                {
                    var __cascade = new CurvedAnimation(
                        parent: _opacityController = new AnimationController(
                            duration: widget.duration,
                            vsync: this
                        ),
                        curve: Curves.fastOutSlowIn
                    );
                    __cascade.addListener(_rebuild);
                    return __cascade;
                }
            )
        )();
        _opacityController.value = widget.visible ? 1.0 : 0.0;
        _orientationController = new AnimationController(duration: widget.duration, vsync: this);
        _orientationAnimation = (
            (Func<Animation<double>>)(
                () =>
                {
                    var __cascade = _orientationController.drive(_turnTween);
                    __cascade.addListener(_rebuild);
                    __cascade.addStatusListener(_resetOrientationAnimation);
                    return __cascade;
                }
            )
        )();
        if (widget.visible)
        {
            _orientationOffset =
                (
                    widget.up
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
                    ? 0.0
                    : Dart_mathLibrary.pi;
        }
    }

    internal virtual void _rebuild()
    {
        setState(() => { });
    }

    internal virtual void _resetOrientationAnimation(AnimationStatus status)
    {
        if (AnimationStatusMembers.isCompleted(status))
        {
            DartRuntimePrimitives.Assert(() => _orientationAnimation.value == Dart_mathLibrary.pi);
            _orientationOffset += Dart_mathLibrary.pi;
            _orientationController.value = 0.0;
        }
    }

    public override void didUpdateWidget(_SortArrow__data_table oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        var skipArrow = false;
        bool? newUp = widget.up ?? _up;
        if (oldWidget.visible != widget.visible)
        {
            if (widget.visible && _opacityController.isDismissed)
            {
                _orientationController.stop();
                _orientationController.value = 0.0;
                _orientationOffset =
                    (
                        newUp
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                        ? 0.0
                        : Dart_mathLibrary.pi;
                skipArrow = true;
            }
            if (widget.visible)
            {
                _opacityController.forward();
            }
            else
            {
                _opacityController.reverse();
            }
        }
        if (_up != newUp && !skipArrow)
        {
            if (_orientationController.isDismissed)
            {
                _orientationController.forward();
            }
            else
            {
                _orientationController.reverse();
            }
        }
        _up = newUp;
    }

    public override void dispose()
    {
        _opacityController.dispose();
        _orientationController.dispose();
        _opacityAnimation.dispose();
        DartRuntimePrimitives.Assert(() =>
        {
            if (_tickers is not null)
            {
                foreach (Scheduler.Ticker ticker in _tickers!)
                {
                    if (ticker.isActive)
                    {
                        throw DartRuntimePrimitives.AsException(
                            new FlutterError(
                                new List<DiagnosticsNode>
                                {
                                    new ErrorSummary($"{this} was disposed with an active Ticker."),
                                    new ErrorDescription(
                                        $"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time "
                                            + "dispose() was called on the mixin, that Ticker was still active. All Tickers must "
                                            + "be disposed before calling super.dispose()."
                                    ),
                                    new ErrorHint(
                                        "Tickers used by AnimationControllers "
                                            + "should be disposed by calling dispose() on the AnimationController itself. "
                                            + "Otherwise, the ticker will leak."
                                    ),
                                    ticker.describeForError("The offending ticker was"),
                                }
                            )
                        );
                    }
                }
            }
            return true;
        });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return new FadeTransition(
            opacity: _opacityAnimation,
            child: new Transform(
                transform: (
                    (Func<Matrix4>)(
                        () =>
                        {
                            var __cascade = Matrix4.rotationZ(
                                _orientationOffset + _orientationAnimation.value
                            );
                            __cascade.setTranslationRaw(0.0, _arrowIconBaselineOffset, 0.0);
                            return __cascade;
                        }
                    )
                )(),
                alignment: Alignment.center,
                child: new Icon(Icons.arrow_upward, size: _arrowIconSize)
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = (
            (Func<_WidgetTicker__ticker_provider>)(
                () =>
                {
                    var __cascade = new _WidgetTicker__ticker_provider(
                        onTick,
                        this,
                        debugLabel: Foundation.ConstantsLibrary.kDebugMode
                            ? $"created by {DiagnosticsLibrary.describeIdentity(this)}"
                            : null
                    );
                    __cascade.muted = !values.enabled;
                    __cascade.forceFrames = values.forceFrames;
                    return __cascade;
                }
            )
        )();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => _tickers is not null);
        DartRuntimePrimitives.Assert(() => _tickers!.Contains(ticker));
        _tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if (_tickers is not null)
        {
            TickerModeData values = _tickerModeNotifier!.value;
            bool mutedLocal = !values.enabled;
            foreach (Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<HashSet<Scheduler.Ticker>>(
                "tickers",
                _tickers,
                description: (_tickers is not null)
                    ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}"
                    : null,
                defaultValue: default
            )
        );
    }
}

internal class _NullTableColumnWidth__data_table : TableColumnWidth
{
    internal _NullTableColumnWidth__data_table() { }

    public override double maxIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth) =>
        throw new NotImplementedException();

    public override double minIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth) =>
        throw new NotImplementedException();
}

internal class _NullWidget__data_table : Widget
{
    internal _NullWidget__data_table() { }

    public override Element createElement() => throw new NotImplementedException();
}
