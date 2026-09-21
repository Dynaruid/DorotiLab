// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/table.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public class TableCellParentData : BoxParentData
{
    public virtual TableCellVerticalAlignment? verticalAlignment { get; set; } = default;
    public virtual long? x { get; set; } = default;
    public virtual long? y { get; set; } = default;

    public override string ToString() =>
        $"{base.ToString()}; {((verticalAlignment is null) ? "default vertical alignment" : $"{verticalAlignment}")}";
}

public abstract class TableColumnWidth
{
    protected TableColumnWidth() { }

    public abstract double minIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth);
    public abstract double maxIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth);

    public virtual double? flex(IEnumerable<RenderBox> cells) => null;

    public override string ToString() =>
        objectRuntimeTypeFunctions.objectRuntimeType(this, "TableColumnWidth");
}

public class IntrinsicColumnWidth : TableColumnWidth
{
    internal virtual double? _flex { get; private set; }

    public IntrinsicColumnWidth(double? flex = null)
    {
        _flex = flex;
    }

    public override double minIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth)
    {
        var result = 0.0;
        foreach (var cell in cells)
        {
            result = Math.Max(result, cell.getMinIntrinsicWidth(double.PositiveInfinity));
        }
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double maxIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth)
    {
        var result = 0.0;
        foreach (var cell in cells)
        {
            result = Math.Max(result, cell.getMaxIntrinsicWidth(double.PositiveInfinity));
        }
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double? flex(IEnumerable<RenderBox> cells) => _flex;

    public override string ToString() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "IntrinsicColumnWidth")}(flex: {_flex?.toStringAsFixed(1L)})";
}

public class FixedColumnWidth : TableColumnWidth
{
    public virtual double value { get; private set; } = default!;

    public FixedColumnWidth(double value)
    {
        this.value = value;
    }

    public override double minIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth)
    {
        return value;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double maxIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth)
    {
        return value;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override string ToString() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "FixedColumnWidth")}({Foundation.DebugLibrary.debugFormatDouble(value)})";
}

public class FractionColumnWidth : TableColumnWidth
{
    public virtual double value { get; private set; } = default!;

    public FractionColumnWidth(double value)
    {
        this.value = value;
    }

    public override double minIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth)
    {
        if (!double.IsFinite(containerWidth))
        {
            return 0.0;
        }
        return value * containerWidth;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double maxIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth)
    {
        if (!double.IsFinite(containerWidth))
        {
            return 0.0;
        }
        return value * containerWidth;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override string ToString() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "FractionColumnWidth")}({value})";
}

public class FlexColumnWidth : TableColumnWidth
{
    public virtual double value { get; private set; } = default!;

    public FlexColumnWidth(double value = 1.0)
    {
        this.value = value;
    }

    public override double minIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth)
    {
        return 0.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double maxIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth)
    {
        return 0.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double? flex(IEnumerable<RenderBox> cells)
    {
        return value;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override string ToString() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "FlexColumnWidth")}({Foundation.DebugLibrary.debugFormatDouble(value)})";
}

public class MaxColumnWidth : TableColumnWidth
{
    public virtual TableColumnWidth a { get; private set; } = default!;
    public virtual TableColumnWidth b { get; private set; } = default!;

    public MaxColumnWidth(TableColumnWidth a, TableColumnWidth b)
    {
        this.a = a;
        this.b = b;
    }

    public override double minIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth)
    {
        return Math.Max(
            a.minIntrinsicWidth(cells, containerWidth),
            b.minIntrinsicWidth(cells, containerWidth)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double maxIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth)
    {
        return Math.Max(
            a.maxIntrinsicWidth(cells, containerWidth),
            b.maxIntrinsicWidth(cells, containerWidth)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double? flex(IEnumerable<RenderBox> cells)
    {
        double? aFlex = a.flex(cells);
        double? bFlex = b.flex(cells);
        if (aFlex is null)
        {
            return bFlex;
        }
        else
        {
            if (bFlex is null)
            {
                return (
                    aFlex
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
            }
        }
        return Math.Max(
            (
                aFlex
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            (bFlex ?? throw new global::System.NullReferenceException("A required value was null."))
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override string ToString() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "MaxColumnWidth")}({a}, {b})";
}

public class MinColumnWidth : TableColumnWidth
{
    public virtual TableColumnWidth a { get; private set; } = default!;
    public virtual TableColumnWidth b { get; private set; } = default!;

    public MinColumnWidth(TableColumnWidth a, TableColumnWidth b)
    {
        this.a = a;
        this.b = b;
    }

    public override double minIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth)
    {
        return Math.Min(
            a.minIntrinsicWidth(cells, containerWidth),
            b.minIntrinsicWidth(cells, containerWidth)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double maxIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth)
    {
        return Math.Min(
            a.maxIntrinsicWidth(cells, containerWidth),
            b.maxIntrinsicWidth(cells, containerWidth)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double? flex(IEnumerable<RenderBox> cells)
    {
        double? aFlex = a.flex(cells);
        double? bFlex = b.flex(cells);
        if (aFlex is null)
        {
            return bFlex;
        }
        else
        {
            if (bFlex is null)
            {
                return (
                    aFlex
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
            }
        }
        return Math.Min(
            (
                aFlex
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            (bFlex ?? throw new global::System.NullReferenceException("A required value was null."))
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override string ToString() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "MinColumnWidth")}({a}, {b})";
}

public enum TableCellVerticalAlignment
{
    top,
    middle,
    bottom,
    baseline,
    fill,
    intrinsicHeight,
}

public class RenderTable : RenderBox
{
    internal virtual List<RenderBox?> _children { get; set; } = new List<RenderBox?>();
    internal virtual long _columns { get; set; } = default!;
    internal virtual long _rows { get; set; } = default!;
    internal virtual DartMap<long, TableColumnWidth> _columnWidths { get; set; } = default!;
    internal virtual TableColumnWidth _defaultColumnWidth { get; set; } = default!;
    internal virtual TextDirection _textDirection { get; set; } = default!;
    internal virtual TableBorder? _border { get; set; } = default;
    internal virtual List<Decoration?>? _rowDecorations { get; set; } = default;
    internal virtual List<BoxPainter?>? _rowDecorationPainters { get; set; } = default;
    internal virtual ImageConfiguration _configuration { get; set; } = default!;
    internal virtual TableCellVerticalAlignment _defaultVerticalAlignment { get; set; } = default!;
    internal virtual TextBaseline? _textBaseline { get; set; } = default;
    internal virtual DartMap<long, _Index__table> _idToIndexMap { get; private set; } =
        new DartMap<long, _Index__table>();
    internal virtual DartMap<long, SemanticsNode> _cachedRows { get; private set; } =
        new DartMap<long, SemanticsNode>();
    internal virtual DartMap<_Index__table, SemanticsNode> _cachedCells { get; private set; } =
        new DartMap<_Index__table, SemanticsNode>();
    internal virtual double? _baselineDistance { get; set; } = default;
    internal virtual List<double> _rowTops { get; private set; } = new List<double>();
    internal virtual IEnumerable<double>? _columnLefts { get; set; } = default;
    internal virtual double _tableWidth { get; set; } = default!;

    public RenderTable(
        long? columns = null,
        long? rows = null,
        DartMap<long, TableColumnWidth>? columnWidths = null,
        TableColumnWidth defaultColumnWidth = default!,
        TextDirection textDirection = default!,
        TableBorder? border = null,
        List<Decoration?>? rowDecorations = null,
        ImageConfiguration configuration = default!,
        TableCellVerticalAlignment defaultVerticalAlignment = TableCellVerticalAlignment.top,
        TextBaseline? textBaseline = null,
        List<List<RenderBox>>? children = null
    )
    {
        TableColumnWidth __defaultColumnWidth = defaultColumnWidth ?? new FlexColumnWidth();
        ImageConfiguration __configuration = configuration ?? ImageConfiguration.empty;
        _textDirection = textDirection;
        _columns =
            columns
            ?? (
                ((children is not null) && (checked((long)children.Count) != 0))
                    ? checked(children.First().Count)
                    : 0L
            );
        _rows = rows ?? 0L;
        _columnWidths = columnWidths ?? new DartMap<long, TableColumnWidth>();
        _defaultColumnWidth = __defaultColumnWidth;
        _border = border;
        _textBaseline = textBaseline;
        _defaultVerticalAlignment = defaultVerticalAlignment;
        _configuration = __configuration;
        System.Diagnostics.Debug.Assert((columns is null) || (columns >= 0L));
        System.Diagnostics.Debug.Assert((rows is null) || (rows >= 0L));
        System.Diagnostics.Debug.Assert((rows is null) || (children is null));
        _children = new List<RenderBox?>(
            Enumerable.Repeat<RenderBox?>(null, checked((int)(_columns * _rows)))
        );
        this.rowDecorations = rowDecorations;
        children?.forEach(row => addRow(new List<RenderBox?>(row)));
    }

    public virtual long columns
    {
        get => _columns;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value >= 0L);
            if ((__value) == columns)
            {
                return;
            }
            long oldColumns = columns;
            List<RenderBox?> oldChildren = _children;
            _columns = (__value);
            _children = new List<RenderBox?>(
                Enumerable.Repeat<RenderBox?>(null, checked((int)(columns * rows)))
            );
            long columnsToCopy = Math.Min(columns, oldColumns);
            for (var y = 0L; y < rows; y += 1L)
            {
                for (var x = 0L; x < columnsToCopy; x += 1L)
                {
                    _children[(int)(x + (y * columns))] = oldChildren[(int)(x + (y * oldColumns))];
                }
            }
            if (oldColumns > columns)
            {
                for (var yLocal = 0L; yLocal < rows; yLocal += 1L)
                {
                    for (long xLocal = columns; xLocal < oldColumns; xLocal += 1L)
                    {
                        long xy = xLocal + (yLocal * oldColumns);
                        if (oldChildren[(int)xy] is not null)
                        {
                            dropChild(oldChildren[(int)xy]!);
                        }
                    }
                }
            }
            markNeedsLayout();
        }
    }
    public virtual long rows
    {
        get => _rows;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value >= 0L);
            if ((__value) == rows)
            {
                return;
            }
            if (_rows > (__value))
            {
                for (long xy = columns * (__value); xy < checked(_children.Count); xy += 1L)
                {
                    if (_children[(int)xy] is not null)
                    {
                        dropChild(_children[(int)xy]!);
                    }
                }
            }
            _rows = (__value);
            _children.setLength(columns * rows);
            markNeedsLayout();
        }
    }
    public virtual DartMap<long, TableColumnWidth>? columnWidths
    {
        get => new DartMap<long, TableColumnWidth>(_columnWidths);
        set
        {
            var __value = value;
            if (Equals(_columnWidths, __value))
            {
                return;
            }
            if ((checked((long)_columnWidths.Count) == 0) && (__value is null))
            {
                return;
            }
            _columnWidths = __value ?? new DartMap<long, TableColumnWidth>();
            markNeedsLayout();
        }
    }

    public virtual void setColumnWidth(long column, TableColumnWidth value)
    {
        if (Equals(_columnWidths.GetValueOrDefault(column), value))
        {
            return;
        }
        _columnWidths[column] = value;
        markNeedsLayout();
    }

    public virtual TableColumnWidth defaultColumnWidth
    {
        get => _defaultColumnWidth;
        set
        {
            var __value = value;
            if (Equals(defaultColumnWidth, __value))
            {
                return;
            }
            _defaultColumnWidth = __value;
            markNeedsLayout();
        }
    }
    public virtual TextDirection textDirection
    {
        get => _textDirection;
        set
        {
            var __value = value;
            if (Equals(_textDirection, (__value)))
            {
                return;
            }
            _textDirection = (__value);
            markNeedsLayout();
        }
    }
    public virtual TableBorder? border
    {
        get => _border;
        set
        {
            var __value = value;
            if (Equals(border, __value))
            {
                return;
            }
            _border = __value;
            markNeedsPaint();
        }
    }

    [System.Diagnostics.CodeAnalysis.AllowNull]
    public virtual List<Decoration?> rowDecorations
    {
        get => new List<Decoration?>(_rowDecorations ?? new List<Decoration?>());
        set
        {
            var __value = value;
            if (Equals(_rowDecorations, __value))
            {
                return;
            }
            _rowDecorations = __value;
            if (_rowDecorationPainters is not null)
            {
                foreach (BoxPainter? painter in _rowDecorationPainters!)
                {
                    painter?.dispose();
                }
            }
            _rowDecorationPainters =
                (_rowDecorations is not null)
                    ? new List<BoxPainter?>(
                        Enumerable.Repeat<BoxPainter?>(
                            null,
                            checked((int)checked((long)_rowDecorations!.Count))
                        )
                    )
                    : null;
        }
    }
    public virtual ImageConfiguration configuration
    {
        get => _configuration;
        set
        {
            var __value = value;
            if (Equals(__value, _configuration))
            {
                return;
            }
            _configuration = __value;
            markNeedsPaint();
        }
    }
    public virtual TableCellVerticalAlignment defaultVerticalAlignment
    {
        get => _defaultVerticalAlignment;
        set
        {
            var __value = value;
            if (Equals(_defaultVerticalAlignment, (__value)))
            {
                return;
            }
            _defaultVerticalAlignment = (__value);
            markNeedsLayout();
        }
    }
    public virtual TextBaseline? textBaseline
    {
        get => _textBaseline;
        set
        {
            var __value = value;
            if (Equals(_textBaseline, __value))
            {
                return;
            }
            _textBaseline = __value;
            markNeedsLayout();
        }
    }

    public override void setupParentData(RenderObject child)
    {
        if (child.parentData is not TableCellParentData)
        {
            child.parentData = new TableCellParentData();
        }
    }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.role = SemanticsRole.table;
        config.isSemanticBoundary = true;
        config.explicitChildNodes = true;
    }

    public override void clearSemantics()
    {
        base.clearSemantics();
        _cachedRows.Clear();
        _cachedCells.Clear();
    }

    public override void assembleSemanticsNode(
        SemanticsNode node,
        SemanticsConfiguration config,
        IEnumerable<SemanticsNode> children
    )
    {
        var rows = new List<SemanticsNode>();
        var rawCells = new List<List<List<SemanticsNode>>>(
            Enumerable.Select(
                Enumerable.Range(0, checked((int)_rows)),
                (rowIndex) =>
                    new List<List<SemanticsNode>>(
                        Enumerable.Select(
                            Enumerable.Range(0, checked((int)_columns)),
                            (columnIndex) => new List<SemanticsNode>()
                        )
                    )
            )
        );
        Rect rectWithOffset(SemanticsNode node)
        {
            Offset offset =
                (
                    (node.transform is not null)
                        ? MatrixUtils.getAsTranslation(node.transform!)
                        : null
                ) ?? Offset.zero;
            return node.rect.shift(offset);
            throw new InvalidOperationException(
                "Control flow completed without returning a value."
            );
        }
        long findRowIndex(double top)
        {
            for (long i = _rows - 1L; i >= 0L; i--)
            {
                if (_rowTops[(int)i] <= top)
                {
                    return i;
                }
            }
            return -1L;
            throw new InvalidOperationException(
                "Control flow completed without returning a value."
            );
        }
        long findColumnIndex(double left)
        {
            if (_columnLefts is null)
            {
                return -1L;
            }
            for (long iLocal = _columnLefts!.Count() - 1L; iLocal >= 0L; iLocal--)
            {
                if (_columnLefts!.elementAt(iLocal) <= left)
                {
                    return iLocal;
                }
            }
            return -1L;
            throw new InvalidOperationException(
                "Control flow completed without returning a value."
            );
        }
        void shiftTransform(SemanticsNode node, double dx, double dy)
        {
            Matrix4? previousTransform = node.transform;
            Offset offsetLocal =
                (
                    (previousTransform is not null)
                        ? MatrixUtils.getAsTranslation(previousTransform)
                        : null
                ) ?? Offset.zero;
            var newTransform = Matrix4.translationValues(
                offsetLocal.dx + dx,
                offsetLocal.dy + dy,
                0
            );
            node.transform = newTransform;
        }
        foreach (var child in children)
        {
            if (_idToIndexMap.ContainsKey(child.id))
            {
                _Index__table index = _idToIndexMap.GetValueOrDefault(child.id)!;
                long yLocal = index.y;
                long xLocal = index.x;
                if ((yLocal < _rows) && (xLocal < _columns))
                {
                    rawCells[(int)yLocal][(int)xLocal].Add(child);
                }
            }
            else
            {
                Rect rectLocal = rectWithOffset(child);
                long yAlternate = findRowIndex(rectLocal.top);
                long xAlternate = findColumnIndex(rectLocal.left);
                if ((yAlternate != -1L) && (xAlternate != -1L))
                {
                    rawCells[(int)yAlternate][(int)xAlternate].Add(child);
                }
            }
        }
        for (var yNested = 0L; yNested < _rows; yNested++)
        {
            Rect rowBox = getRowBox(yNested);
            if (rowBox.height == 0L)
            {
                continue;
            }
            SemanticsNode newRow =
                _cachedRows.GetValueOrDefault(yNested)
                ?? (
                    _cachedRows[yNested] = new SemanticsNode(showOnScreen: () =>
                    {
                        showOnScreen(descendant: this, rect: rowBox);
                    })
                );
            var cells = new List<SemanticsNode>();
            for (var xNested = 0L; xNested < columns; xNested++)
            {
                List<SemanticsNode> rawChildrens = rawCells[(int)yNested][(int)xNested];
                if (checked((long)rawChildrens.Count) == 0)
                {
                    continue;
                }
                bool addCellWrapper =
                    (checked(rawChildrens.Count) > 1L)
                    || (
                        (!Equals(rawChildrens.Single().role, SemanticsRole.cell))
                        && (!Equals(rawChildrens.Single().role, SemanticsRole.columnHeader))
                    );
                SemanticsNode cellLocal = default!;
                if (!addCellWrapper)
                {
                    cellLocal = rawChildrens.Single();
                }
                else
                {
                    var indexLocal = new _Index__table(yNested, xNested);
                    cellLocal = (
                        (Func<SemanticsNode>)(
                            () =>
                            {
                                var __cascade = _cachedCells.putIfAbsent(
                                    indexLocal,
                                    () => new SemanticsNode()
                                );
                                __cascade.updateWith(
                                    config: (
                                        (Func<SemanticsConfiguration>)(
                                            () =>
                                            {
                                                var __cascade = new SemanticsConfiguration();
                                                __cascade.role = SemanticsRole.cell;
                                                return __cascade;
                                            }
                                        )
                                    )(),
                                    childrenInInversePaintOrder: rawChildrens
                                );
                                return __cascade;
                            }
                        )
                    )();
                }
                double cellWidth =
                    (xNested == (_columns - 1L))
                        ? (rowBox.width - _columnLefts!.elementAt(xNested))
                        : (
                            _columnLefts!.elementAt(xNested + 1L) - _columnLefts!.elementAt(xNested)
                        );
                if (cellWidth <= 0.0)
                {
                    continue;
                }
                if (addCellWrapper)
                {
                    (
                        (Func<SemanticsNode>)(
                            () =>
                            {
                                var __cascade = cellLocal;
                                __cascade.transform = Matrix4.translationValues(
                                    _columnLefts!.elementAt(xNested),
                                    0,
                                    0
                                );
                                __cascade.rect = Rect.fromLTWH(0, 0, cellWidth, rowBox.height);
                                return __cascade;
                            }
                        )
                    )();
                }
                foreach (var childLocal in rawChildrens)
                {
                    _idToIndexMap[childLocal.id] = new _Index__table(yNested, xNested);
                    Rect localRect = rectWithOffset(childLocal);
                    double dyLocal =
                        (
                            localRect.bottom
                            > (rowBox.height + Foundation.ConstantsLibrary.precisionErrorTolerance)
                        )
                            ? -_rowTops.elementAt(yNested)
                            : 0.0;
                    double dxLocal = addCellWrapper
                        ? ((localRect.left >= cellWidth) ? -_columnLefts!.elementAt(xNested) : 0.0)
                        : (
                            (localRect.right <= _columnLefts!.elementAt(xNested))
                                ? _columnLefts!.elementAt(xNested)
                                : 0.0
                        );
                    if ((dxLocal != 0L) || (dyLocal != 0L))
                    {
                        shiftTransform(childLocal, dxLocal, dyLocal);
                    }
                }
                cellLocal.indexInParent = xNested;
                cells.Add(cellLocal);
            }
            (
                (Func<SemanticsNode>)(
                    () =>
                    {
                        var __cascade = newRow;
                        __cascade.updateWith(
                            config: (
                                (Func<SemanticsConfiguration>)(
                                    () =>
                                    {
                                        var __cascade = new SemanticsConfiguration();
                                        __cascade.indexInParent = yNested;
                                        __cascade.role = SemanticsRole.row;
                                        return __cascade;
                                    }
                                )
                            )(),
                            childrenInInversePaintOrder: cells
                        );
                        __cascade.transform = Matrix4.translationValues(rowBox.left, rowBox.top, 0);
                        __cascade.rect = Rect.fromLTWH(0, 0, rowBox.width, rowBox.height);
                        return __cascade;
                    }
                )
            )();
            rows.Add(newRow);
        }
        node.updateWith(config: config, childrenInInversePaintOrder: rows);
    }

    public virtual void setFlatChildren(long columns, List<RenderBox?> cells)
    {
        if (Equals(cells, _children) && ((columns) == _columns))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => columns >= 0L);
        if (((columns) == 0L) || (checked((long)cells.Count) == 0))
        {
            DartRuntimePrimitives.Assert(() => checked((long)cells.Count) == 0);
            _columns = (columns);
            if (checked((long)_children.Count) == 0)
            {
                DartRuntimePrimitives.Assert(() => _rows == 0L);
                return;
            }
            foreach (RenderBox? oldChild in _children)
            {
                if (oldChild is not null)
                {
                    dropChild(oldChild);
                }
            }
            _rows = 0L;
            _children.Clear();
            markNeedsLayout();
            return;
        }
        DartRuntimePrimitives.Assert(() => (checked(cells.Count) % (columns)) == 0L);
        HashSet<RenderBox> lostChildren = new HashSet<RenderBox>();
        for (var y = 0L; y < _rows; y += 1L)
        {
            for (var x = 0L; x < _columns; x += 1L)
            {
                long xyOld = x + (y * _columns);
                long xyNew = x + (y * (columns));
                if (
                    (xyOld < checked(_children.Count))
                    && (_children[(int)xyOld] is not null)
                    && (
                        (x >= (columns))
                        || (xyNew >= checked(cells.Count))
                        || (!Equals(_children[(int)xyOld], cells[(int)xyNew]))
                    )
                )
                {
                    lostChildren.Add(_children[(int)xyOld]!);
                }
            }
        }
        var yLocal = 0L;
        while ((yLocal * (columns)) < checked(cells.Count))
        {
            for (var xLocal = 0L; xLocal < (columns); xLocal += 1L)
            {
                long xyNewLocal = xLocal + (yLocal * (columns));
                long xyOldLocal = xLocal + (yLocal * _columns);
                if (
                    (cells[(int)xyNewLocal] is { } newChild)
                    && (
                        (xLocal >= _columns)
                        || (yLocal >= _rows)
                        || (xyOldLocal >= checked(_children.Count))
                        || (!Equals(_children[(int)xyOldLocal], cells[(int)xyNewLocal]))
                    )
                )
                {
                    if (!lostChildren.Remove(newChild))
                    {
                        adoptChild(newChild);
                    }
                }
            }
            yLocal += 1L;
        }
        lostChildren.forEach(dropChild);
        _columns = (columns);
        _rows = checked(checked(cells.Count) / (columns));
        _children = new List<RenderBox?>(cells);
        DartRuntimePrimitives.Assert(() => checked(_children.Count) == (rows * (columns)));
        markNeedsLayout();
    }

    public virtual void setChildren(List<List<RenderBox>>? cells)
    {
        if (cells is null)
        {
            setFlatChildren(0L, new List<RenderBox?>());
            return;
        }
        foreach (RenderBox? oldChild in _children)
        {
            if (oldChild is not null)
            {
                dropChild(oldChild);
            }
        }
        _children.Clear();
        _columns = (checked((long)cells.Count) != 0) ? checked(cells.First().Count) : 0L;
        _rows = 0L;
        cells.forEach(row => addRow(new List<RenderBox?>(row)));
        DartRuntimePrimitives.Assert(() => checked(_children.Count) == (rows * columns));
    }

    public virtual void addRow(List<RenderBox?> cells)
    {
        DartRuntimePrimitives.Assert(() => checked(cells.Count) == columns);
        DartRuntimePrimitives.Assert(() => checked(_children.Count) == (rows * columns));
        _rows += 1L;
        _children.AddRange(cells);
        foreach (var cell in cells)
        {
            if (cell is not null)
            {
                adoptChild(cell);
            }
        }
        markNeedsLayout();
    }

    public virtual void setChild(long x, long y, RenderBox? value)
    {
        DartRuntimePrimitives.Assert(() => (x >= 0L) && (x < columns) && (y >= 0L) && (y < rows));
        DartRuntimePrimitives.Assert(() => checked(_children.Count) == (rows * columns));
        long xy = x + (y * columns);
        RenderBox? oldChild = _children[(int)xy];
        if (Equals(oldChild, value))
        {
            return;
        }
        if (oldChild is not null)
        {
            dropChild(oldChild);
        }
        _children[(int)xy] = value;
        if (value is not null)
        {
            adoptChild(value);
        }
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        foreach (RenderBox? child in _children)
        {
            child?.attach(owner);
        }
    }

    public override void detach()
    {
        base.detach();
        if (_rowDecorationPainters is not null)
        {
            foreach (BoxPainter? painter in _rowDecorationPainters!)
            {
                painter?.dispose();
            }
            _rowDecorationPainters = new List<BoxPainter?>(
                Enumerable.Repeat<BoxPainter?>(
                    null,
                    checked((int)checked((long)_rowDecorations!.Count))
                )
            );
        }
        foreach (RenderBox? child in _children)
        {
            child?.detach();
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        DartRuntimePrimitives.Assert(() => checked(_children.Count) == (rows * columns));
        foreach (RenderBox? child in _children)
        {
            if (child is not null)
            {
                visitor(child);
            }
        }
    }

    public override void redepthChildren()
    {
        visitChildren(redepthChild);
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        DartRuntimePrimitives.Assert(() => checked(_children.Count) == (rows * columns));
        if ((rows * columns) == 0L)
        {
            return 0.0;
        }
        var totalMinWidth = 0.0;
        for (var x = 0L; x < columns; x += 1L)
        {
            TableColumnWidth columnWidth = _columnWidths.GetValueOrDefault(x) ?? defaultColumnWidth;
            IEnumerable<RenderBox> columnCells = column(x);
            totalMinWidth += columnWidth.minIntrinsicWidth(columnCells, double.PositiveInfinity);
        }
        return totalMinWidth;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        DartRuntimePrimitives.Assert(() => checked(_children.Count) == (rows * columns));
        if ((rows * columns) == 0L)
        {
            return 0.0;
        }
        var totalMaxWidth = 0.0;
        for (var x = 0L; x < columns; x += 1L)
        {
            TableColumnWidth columnWidth = _columnWidths.GetValueOrDefault(x) ?? defaultColumnWidth;
            IEnumerable<RenderBox> columnCells = column(x);
            totalMaxWidth += columnWidth.maxIntrinsicWidth(columnCells, double.PositiveInfinity);
        }
        return totalMaxWidth;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        DartRuntimePrimitives.Assert(() => checked(_children.Count) == (rows * columns));
        if ((rows * columns) == 0L)
        {
            return 0.0;
        }
        List<double> widths = _computeColumnWidths(
            BoxConstraints.CreateTightForFinite(width: width)
        );
        var rowTop = 0.0;
        for (var y = 0L; y < rows; y += 1L)
        {
            var rowHeight = 0.0;
            for (var x = 0L; x < columns; x += 1L)
            {
                long xy = x + (y * columns);
                RenderBox? child = _children[(int)xy];
                if (child is not null)
                {
                    rowHeight = Math.Max(rowHeight, child.getMaxIntrinsicHeight(widths[(int)x]));
                }
            }
            rowTop += rowHeight;
        }
        return rowTop;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return getMinIntrinsicHeight(width);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        return _baselineDistance;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual IEnumerable<RenderBox> column(long x)
    {
        for (var y = 0L; y < rows; y += 1L)
        {
            long xy = x + (y * columns);
            RenderBox? child = _children[(int)xy];
            if (child is not null)
            {
                yield return child;
            }
        }
    }

    public virtual IEnumerable<RenderBox> row(long y)
    {
        long start = y * columns;
        long end = (y + 1L) * columns;
        for (var xy = start; xy < end; xy += 1L)
        {
            RenderBox? child = _children[(int)xy];
            if (child is not null)
            {
                yield return child;
            }
        }
    }

    internal virtual List<double> _computeColumnWidths(BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => checked(_children.Count) == (rows * columns));
        var widths = new List<double>(Enumerable.Repeat(0.0, checked((int)columns)));
        var minWidths = new List<double>(Enumerable.Repeat(0.0, checked((int)columns)));
        var flexes = new List<double?>(Enumerable.Repeat<double?>(null, checked((int)columns)));
        var tableWidth = 0.0;
        var unflexedTableWidth = 0.0;
        var totalFlex = 0.0;
        for (var x = 0L; x < columns; x += 1L)
        {
            TableColumnWidth columnWidth = _columnWidths.GetValueOrDefault(x) ?? defaultColumnWidth;
            IEnumerable<RenderBox> columnCells = column(x);
            double maxIntrinsicWidthLocal = columnWidth.maxIntrinsicWidth(
                columnCells,
                constraints.maxWidth
            );
            DartRuntimePrimitives.Assert(() => double.IsFinite(maxIntrinsicWidthLocal));
            DartRuntimePrimitives.Assert(() => maxIntrinsicWidthLocal >= 0.0);
            widths[(int)x] = maxIntrinsicWidthLocal;
            tableWidth += maxIntrinsicWidthLocal;
            double minIntrinsicWidthLocal = columnWidth.minIntrinsicWidth(
                columnCells,
                constraints.maxWidth
            );
            DartRuntimePrimitives.Assert(() => double.IsFinite(minIntrinsicWidthLocal));
            DartRuntimePrimitives.Assert(() => minIntrinsicWidthLocal >= 0.0);
            minWidths[(int)x] = minIntrinsicWidthLocal;
            DartRuntimePrimitives.Assert(() => maxIntrinsicWidthLocal >= minIntrinsicWidthLocal);
            double? flexLocal = columnWidth.flex(columnCells);
            if (flexLocal is not null)
            {
                double flex__38095__value38143 = (
                    flexLocal
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
                DartRuntimePrimitives.Assert(() => double.IsFinite((flex__38095__value38143)));
                DartRuntimePrimitives.Assert(() => (flex__38095__value38143) > 0.0);
                flexes[(int)x] = (flex__38095__value38143);
                totalFlex += (flex__38095__value38143);
            }
            else
            {
                unflexedTableWidth = unflexedTableWidth + maxIntrinsicWidthLocal;
            }
        }
        double maxWidthConstraint = constraints.maxWidth;
        double minWidthConstraint = constraints.minWidth;
        if (totalFlex > 0.0)
        {
            double targetWidth = default!;
            if (double.IsFinite(maxWidthConstraint))
            {
                targetWidth = maxWidthConstraint;
            }
            else
            {
                targetWidth = minWidthConstraint;
            }
            if (tableWidth < targetWidth)
            {
                double remainingWidth = targetWidth - unflexedTableWidth;
                DartRuntimePrimitives.Assert(() => double.IsFinite(remainingWidth));
                DartRuntimePrimitives.Assert(() => remainingWidth >= 0.0);
                for (var xLocal = 0L; xLocal < columns; xLocal += 1L)
                {
                    if (flexes[(int)xLocal] is not null)
                    {
                        double flexedWidth =
                            remainingWidth
                            * (
                                flexes[(int)xLocal]
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            )
                            / totalFlex;
                        DartRuntimePrimitives.Assert(() => double.IsFinite(flexedWidth));
                        DartRuntimePrimitives.Assert(() => flexedWidth >= 0.0);
                        if (widths[(int)xLocal] < flexedWidth)
                        {
                            double delta = flexedWidth - widths[(int)xLocal];
                            tableWidth += delta;
                            widths[(int)xLocal] = flexedWidth;
                        }
                    }
                }
                DartRuntimePrimitives.Assert(() =>
                    (tableWidth + Foundation.ConstantsLibrary.precisionErrorTolerance)
                    >= targetWidth
                );
            }
        }
        else
        {
            if (tableWidth < minWidthConstraint)
            {
                double deltaLocal = (minWidthConstraint - tableWidth) / columns;
                for (var xAlternate = 0L; xAlternate < columns; xAlternate += 1L)
                {
                    widths[(int)xAlternate] = widths[(int)xAlternate] + deltaLocal;
                }
                tableWidth = minWidthConstraint;
            }
        }
        if (tableWidth > maxWidthConstraint)
        {
            double deficit = tableWidth - maxWidthConstraint;
            long availableColumns = columns;
            while (
                (deficit > Foundation.ConstantsLibrary.precisionErrorTolerance)
                && (totalFlex > Foundation.ConstantsLibrary.precisionErrorTolerance)
            )
            {
                var newTotalFlex = 0.0;
                for (var xNested = 0L; xNested < columns; xNested += 1L)
                {
                    if (flexes[(int)xNested] is not null)
                    {
                        double newWidth =
                            widths[(int)xNested]
                            - (
                                deficit
                                * (
                                    flexes[(int)xNested]
                                    ?? throw new global::System.NullReferenceException(
                                        "A required value was null."
                                    )
                                )
                                / totalFlex
                            );
                        DartRuntimePrimitives.Assert(() => double.IsFinite(newWidth));
                        if (newWidth <= minWidths[(int)xNested])
                        {
                            deficit -= widths[(int)xNested] - minWidths[(int)xNested];
                            widths[(int)xNested] = minWidths[(int)xNested];
                            flexes[(int)xNested] = null;
                            availableColumns -= 1L;
                        }
                        else
                        {
                            deficit -= widths[(int)xNested] - newWidth;
                            widths[(int)xNested] = newWidth;
                            newTotalFlex += (
                                flexes[(int)xNested]
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            );
                        }
                        DartRuntimePrimitives.Assert(() => widths[(int)xNested] >= 0.0);
                    }
                }
                totalFlex = newTotalFlex;
            }
            while (
                (deficit > Foundation.ConstantsLibrary.precisionErrorTolerance)
                && (availableColumns > 0L)
            )
            {
                double deltaAlternate = deficit / availableColumns;
                DartRuntimePrimitives.Assert(() => deltaAlternate != 0L);
                var newAvailableColumns = 0L;
                for (var xCurrent = 0L; xCurrent < columns; xCurrent += 1L)
                {
                    double availableDelta = widths[(int)xCurrent] - minWidths[(int)xCurrent];
                    if (availableDelta > 0.0)
                    {
                        if (availableDelta <= deltaAlternate)
                        {
                            deficit -= widths[(int)xCurrent] - minWidths[(int)xCurrent];
                            widths[(int)xCurrent] = minWidths[(int)xCurrent];
                        }
                        else
                        {
                            deficit -= deltaAlternate;
                            widths[(int)xCurrent] = widths[(int)xCurrent] - deltaAlternate;
                            newAvailableColumns += 1L;
                        }
                    }
                }
                availableColumns = newAvailableColumns;
            }
        }
        return widths;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Rect getRowBox(long row)
    {
        DartRuntimePrimitives.Assert(() => row >= 0L);
        DartRuntimePrimitives.Assert(() => row < rows);
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        return Rect.fromLTRB(0.0, _rowTops[(int)row], size.width, _rowTops[(int)(row + 1L)]);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        if ((rows * columns) == 0L)
        {
            return null;
        }
        List<double> widths = _computeColumnWidths(constraints);
        double? baselineOffset = default!;
        for (var col = 0L; col < columns; col += 1L)
        {
            RenderBox? child = _children[(int)col];
            var childConstraints = BoxConstraints.CreateTightFor(width: widths[(int)col]);
            if (child is null)
            {
                continue;
            }
            var childParentData = ((TableCellParentData?)child.parentData!)!;
            double? childBaseline = (
                childParentData.verticalAlignment ?? defaultVerticalAlignment
            ) switch
            {
                TableCellVerticalAlignment.baseline => child.getDryBaseline(
                    childConstraints,
                    baseline
                ),
                TableCellVerticalAlignment.baseline
                or TableCellVerticalAlignment.top
                or TableCellVerticalAlignment.middle
                or TableCellVerticalAlignment.bottom
                or TableCellVerticalAlignment.fill => null,
                TableCellVerticalAlignment.intrinsicHeight => null,
                _ => throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
            };
            if (
                (childBaseline is not null)
                && (
                    (baselineOffset is null)
                    || (
                        (
                            baselineOffset
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        )
                        < (
                            childBaseline
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        )
                    )
                )
            )
            {
                double childBaseline__44477__value44974 = (
                    childBaseline
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
                baselineOffset = (childBaseline__44477__value44974);
            }
        }
        return baselineOffset;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        if ((rows * columns) == 0L)
        {
            return constraints.constrain(Size.zero);
        }
        List<double> widths = _computeColumnWidths(constraints);
        double tableWidth = Enumerable.Aggregate(widths, (double)0.0, (a, b) => a + b);
        var rowTop = 0.0;
        for (var y = 0L; y < rows; y += 1L)
        {
            var rowHeight = 0.0;
            for (var x = 0L; x < columns; x += 1L)
            {
                long xy = x + (y * columns);
                RenderBox? child = _children[(int)xy];
                if (child is not null)
                {
                    var childParentData = ((TableCellParentData?)child.parentData!)!;
                    switch (childParentData.verticalAlignment ?? defaultVerticalAlignment)
                    {
                        case TableCellVerticalAlignment.baseline:
                        {
                            DartRuntimePrimitives.Assert(() =>
                                debugCannotComputeDryLayout(
                                    reason: "TableCellVerticalAlignment.baseline requires a full layout for baseline metrics to be available."
                                )
                            );
                            return Size.zero;
                        }
                        case TableCellVerticalAlignment.top:
                        case TableCellVerticalAlignment.middle:
                        case TableCellVerticalAlignment.bottom:
                        case TableCellVerticalAlignment.intrinsicHeight:
                        {
                            Size childSize = child.getDryLayout(
                                BoxConstraints.CreateTightFor(width: widths[(int)x])
                            );
                            rowHeight = Math.Max(rowHeight, childSize.height);
                            break;
                        }
                        case TableCellVerticalAlignment.fill:
                        {
                            break;
                        }
                    }
                }
            }
            rowTop += rowHeight;
        }
        return constraints.constrain(new Size(tableWidth, rowTop));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraintsLocal = constraints;
        long rowsLocal = rows;
        long columnsLocal = columns;
        DartRuntimePrimitives.Assert(() =>
            checked(_children.Count) == ((rowsLocal) * (columnsLocal))
        );
        if (((rowsLocal) * (columnsLocal)) == 0L)
        {
            _tableWidth = 0.0;
            size = constraintsLocal.constrain(Size.zero);
            return;
        }
        List<double> widths = _computeColumnWidths(constraintsLocal);
        var positions = new List<double>(Enumerable.Repeat(0.0, checked((int)(columnsLocal))));
        switch (textDirection)
        {
            case TextDirection.rtl:
            {
                positions[(int)(columnsLocal - 1L)] = 0.0;
                for (long xLocal = (columnsLocal) - 2L; xLocal >= 0L; xLocal -= 1L)
                {
                    positions[(int)xLocal] =
                        positions[(int)(xLocal + 1L)] + widths[(int)(xLocal + 1L)];
                }
                _columnLefts = Enumerable.Reverse(positions);
                _tableWidth = positions.First() + widths.First();
                break;
            }
            case TextDirection.ltr:
            {
                positions[(int)0L] = 0.0;
                for (var xAlternate = 1L; xAlternate < (columnsLocal); xAlternate += 1L)
                {
                    positions[(int)xAlternate] =
                        positions[(int)(xAlternate - 1L)] + widths[(int)(xAlternate - 1L)];
                }
                _columnLefts = positions;
                _tableWidth = positions.Last() + widths.Last();
                break;
            }
        }
        _rowTops.Clear();
        _baselineDistance = null;
        var rowTop = 0.0;
        for (var yLocal = 0L; yLocal < (rowsLocal); yLocal += 1L)
        {
            _rowTops.Add(rowTop);
            var rowHeight = 0.0;
            var haveBaseline = false;
            var beforeBaselineDistance = 0.0;
            var afterBaselineDistance = 0.0;
            var baselines = new List<double>(Enumerable.Repeat(0.0, checked((int)(columnsLocal))));
            for (var xNested = 0L; xNested < (columnsLocal); xNested += 1L)
            {
                long xy = xNested + (yLocal * (columnsLocal));
                RenderBox? child = _children[(int)xy];
                if (child is not null)
                {
                    var childParentData = ((TableCellParentData?)child.parentData!)!;
                    childParentData.x = xNested;
                    childParentData.y = yLocal;
                    switch (childParentData.verticalAlignment ?? defaultVerticalAlignment)
                    {
                        case TableCellVerticalAlignment.baseline:
                        {
                            DartRuntimePrimitives.Assert(() => textBaseline is not null);
                            child.layout(
                                BoxConstraints.CreateTightFor(width: widths[(int)xNested]),
                                parentUsesSize: true
                            );
                            double? childBaseline = child.getDistanceToBaseline(
                                (
                                    textBaseline
                                    ?? throw new global::System.NullReferenceException(
                                        "A required value was null."
                                    )
                                ),
                                onlyReal: true
                            );
                            if (childBaseline is not null)
                            {
                                double childBaseline__49094__value49237 = (
                                    childBaseline
                                    ?? throw new global::System.NullReferenceException(
                                        "A required value was null."
                                    )
                                );
                                beforeBaselineDistance = Math.Max(
                                    beforeBaselineDistance,
                                    (childBaseline__49094__value49237)
                                );
                                afterBaselineDistance = Math.Max(
                                    afterBaselineDistance,
                                    child.size.height - (childBaseline__49094__value49237)
                                );
                                baselines[(int)xNested] = (childBaseline__49094__value49237);
                                haveBaseline = true;
                            }
                            else
                            {
                                rowHeight = Math.Max(rowHeight, child.size.height);
                                childParentData.offset = new Offset(
                                    positions[(int)xNested],
                                    rowTop
                                );
                            }
                            break;
                        }
                        case TableCellVerticalAlignment.top:
                        case TableCellVerticalAlignment.middle:
                        case TableCellVerticalAlignment.bottom:
                        case TableCellVerticalAlignment.intrinsicHeight:
                        {
                            child.layout(
                                BoxConstraints.CreateTightFor(width: widths[(int)xNested]),
                                parentUsesSize: true
                            );
                            rowHeight = Math.Max(rowHeight, child.size.height);
                            break;
                        }
                        case TableCellVerticalAlignment.fill:
                        {
                            break;
                        }
                    }
                }
            }
            if (haveBaseline)
            {
                if (yLocal == 0L)
                {
                    _baselineDistance = beforeBaselineDistance;
                }
                rowHeight = Math.Max(rowHeight, beforeBaselineDistance + afterBaselineDistance);
            }
            for (var xCurrent = 0L; xCurrent < (columnsLocal); xCurrent += 1L)
            {
                long xyLocal = xCurrent + (yLocal * (columnsLocal));
                RenderBox? childLocal = _children[(int)xyLocal];
                if (childLocal is not null)
                {
                    var childParentDataLocal = ((TableCellParentData?)childLocal.parentData!)!;
                    switch (childParentDataLocal.verticalAlignment ?? defaultVerticalAlignment)
                    {
                        case TableCellVerticalAlignment.baseline:
                        {
                            childParentDataLocal.offset = new Offset(
                                positions[(int)xCurrent],
                                rowTop + beforeBaselineDistance - baselines[(int)xCurrent]
                            );
                            break;
                        }
                        case TableCellVerticalAlignment.top:
                        {
                            childParentDataLocal.offset = new Offset(
                                positions[(int)xCurrent],
                                rowTop
                            );
                            break;
                        }
                        case TableCellVerticalAlignment.middle:
                        {
                            childParentDataLocal.offset = new Offset(
                                positions[(int)xCurrent],
                                rowTop + ((rowHeight - childLocal.size.height) / 2.0)
                            );
                            break;
                        }
                        case TableCellVerticalAlignment.bottom:
                        {
                            childParentDataLocal.offset = new Offset(
                                positions[(int)xCurrent],
                                rowTop + rowHeight - childLocal.size.height
                            );
                            break;
                        }
                        case TableCellVerticalAlignment.fill:
                        case TableCellVerticalAlignment.intrinsicHeight:
                        {
                            childLocal.layout(
                                BoxConstraints.CreateTightFor(
                                    width: widths[(int)xCurrent],
                                    height: rowHeight
                                )
                            );
                            childParentDataLocal.offset = new Offset(
                                positions[(int)xCurrent],
                                rowTop
                            );
                            break;
                        }
                    }
                }
            }
            rowTop += rowHeight;
        }
        _rowTops.Add(rowTop);
        size = constraintsLocal.constrain(new Size(_tableWidth, rowTop));
        DartRuntimePrimitives.Assert(() => checked(_rowTops.Count) == ((rowsLocal) + 1L));
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        DartRuntimePrimitives.Assert(() => checked(_children.Count) == (rows * columns));
        for (long index = checked(_children.Count) - 1L; index >= 0L; index -= 1L)
        {
            RenderBox? child = _children[(int)index];
            if (child is not null)
            {
                var childParentData = ((BoxParentData?)child.parentData!)!;
                bool isHit = result.addWithPaintOffset(
                    offset: childParentData.offset,
                    position: position,
                    hitTest: (result, transformed) =>
                    {
                        DartRuntimePrimitives.Assert(() =>
                            Equals(transformed, position - childParentData.offset)
                        );
                        return child.hitTest(result, position: transformed);
                    }
                );
                if (isHit)
                {
                    return true;
                }
            }
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() => checked(_children.Count) == (rows * columns));
        if ((rows * columns) == 0L)
        {
            if (border is not null)
            {
                var borderRect = Rect.fromLTWH(offset.dx, offset.dy, _tableWidth, 0.0);
                border!.paint(
                    context.canvas,
                    borderRect,
                    rows: new List<double>(),
                    columns: new List<double>()
                );
            }
            return;
        }
        DartRuntimePrimitives.Assert(() => checked(_rowTops.Count) == (rows + 1L));
        if (_rowDecorations is not null)
        {
            DartRuntimePrimitives.Assert(() =>
                checked(_rowDecorations!.Count) == checked((long)_rowDecorationPainters!.Count)
            );
            Canvas canvasLocal = context.canvas;
            for (var y = 0L; y < rows; y += 1L)
            {
                if (checked(_rowDecorations!.Count) <= y)
                {
                    break;
                }
                if (_rowDecorations![(int)y] is not null)
                {
                    _rowDecorationPainters![(int)y] ??= _rowDecorations![(int)y]!.createBoxPainter(
                        markNeedsPaint
                    );
                    _rowDecorationPainters![(int)y]!.paint(
                        canvasLocal,
                        new Offset(offset.dx, offset.dy + _rowTops[(int)y]),
                        configuration.copyWith(
                            size: new Size(size.width, _rowTops[(int)(y + 1L)] - _rowTops[(int)y])
                        )
                    );
                }
            }
        }
        for (var index = 0L; index < checked(_children.Count); index += 1L)
        {
            RenderBox? child = _children[(int)index];
            if (child is not null)
            {
                var childParentData = ((BoxParentData?)child.parentData!)!;
                context.paintChild(child, childParentData.offset + offset);
            }
        }
        DartRuntimePrimitives.Assert(() => _rows == (checked(_rowTops.Count) - 1L));
        DartRuntimePrimitives.Assert(() => _columns == _columnLefts!.Count());
        if (border is not null)
        {
            var borderRectLocal = Rect.fromLTWH(offset.dx, offset.dy, _tableWidth, _rowTops.Last());
            IEnumerable<double> rowsLocal = _rowTops.GetRange(1L, checked(_rowTops.Count) - 1L);
            IEnumerable<double> columnsLocal = _columnLefts!.skip(1L);
            border!.paint(context.canvas, borderRectLocal, rows: rowsLocal, columns: columnsLocal);
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<TableBorder>("border", border, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<DartMap<long, TableColumnWidth>>(
                "specified column widths",
                _columnWidths,
                level: (checked((long)_columnWidths.Count) == 0)
                    ? DiagnosticLevel.hidden
                    : DiagnosticLevel.info
            )
        );
        properties.add(
            new DiagnosticsProperty<TableColumnWidth>("default column width", defaultColumnWidth)
        );
        properties.add(new MessageProperty("table size", $"{columns}×{rows}"));
        properties.add(
            new IterableProperty<string>(
                "column offsets",
                _columnLefts?.map(value => Foundation.DebugLibrary.debugFormatDouble(value)),
                ifNull: "unknown"
            )
        );
        properties.add(
            new IterableProperty<string>(
                "row offsets",
                _rowTops.map(value => Foundation.DebugLibrary.debugFormatDouble(value)),
                ifNull: "unknown"
            )
        );
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        if (checked((long)_children.Count) == 0)
        {
            return new List<DiagnosticsNode> { new DiagnosticsNode("table is empty") };
        }
        return new List<DiagnosticsNode>();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _Index__table
{
    public virtual long y { get; private set; } = default!;
    public virtual long x { get; private set; } = default!;

    internal _Index__table(long y, long x)
    {
        this.y = y;
        this.x = x;
    }

    public override bool Equals(object? other)
    {
        var __other = other as _Index__table;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (__other is not _Index__table)
        {
            return false;
        }
        return (y == __other.y) && (x == __other.x);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(y, x);
}
