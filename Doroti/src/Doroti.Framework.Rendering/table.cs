// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/table.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Rendering;

public class TableCellParentData : BoxParentData
{
    public virtual TableCellVerticalAlignment? verticalAlignment { get; set; } = default;
    public virtual long? x { get; set; } = default;
    public virtual long? y { get; set; } = default;

    public override string ToString() => $"{base.ToString()}; {((this.verticalAlignment is null) ? "default vertical alignment" : $"{this.verticalAlignment}")}";
}

public abstract class TableColumnWidth
{
    protected TableColumnWidth()
    {
    }

    public abstract double minIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth);
    public abstract double maxIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth);
    public virtual double? flex(IEnumerable<RenderBox> cells) => null;
    public override string ToString() => global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "TableColumnWidth");
}

public class IntrinsicColumnWidth : TableColumnWidth
{
    internal virtual double? _flex { get; private set; }

    public IntrinsicColumnWidth(double? flex = null)
    {
        this._flex = flex;
    }

    public override double minIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth)
    {
        var result = 0.0;
        foreach (var cell in cells)
        {
            result = Math.Max(result, cell.getMinIntrinsicWidth(double.PositiveInfinity));
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double maxIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth)
    {
        var result = 0.0;
        foreach (var cell in cells)
        {
            result = Math.Max(result, cell.getMaxIntrinsicWidth(double.PositiveInfinity));
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? flex(IEnumerable<RenderBox> cells) => this._flex;
    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "IntrinsicColumnWidth"))}(flex: {this._flex?.toStringAsFixed(1L)})";
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
        return this.value;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double maxIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth)
    {
        return this.value;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "FixedColumnWidth"))}({(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(this.value))})";
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
        return (this.value * containerWidth);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double maxIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth)
    {
        if (!double.IsFinite(containerWidth))
        {
            return 0.0;
        }
        return (this.value * containerWidth);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "FractionColumnWidth"))}({this.value})";
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
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double maxIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth)
    {
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? flex(IEnumerable<RenderBox> cells)
    {
        return this.value;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "FlexColumnWidth"))}({(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(this.value))})";
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
        return Math.Max(this.a.minIntrinsicWidth(cells, containerWidth), this.b.minIntrinsicWidth(cells, containerWidth));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double maxIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth)
    {
        return Math.Max(this.a.maxIntrinsicWidth(cells, containerWidth), this.b.maxIntrinsicWidth(cells, containerWidth));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? flex(IEnumerable<RenderBox> cells)
    {
        double? aFlex = this.a.flex(cells);
        double? bFlex = this.b.flex(cells);
        if ((aFlex is null))
        {
            return bFlex;
        }
        else
        {
            if ((bFlex is null))
            {
                return DartRuntimePrimitives.RequireValue(aFlex);
            }
        }
        return Math.Max(DartRuntimePrimitives.RequireValue(aFlex), DartRuntimePrimitives.RequireValue(bFlex));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "MaxColumnWidth"))}({this.a}, {this.b})";
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
        return Math.Min(this.a.minIntrinsicWidth(cells, containerWidth), this.b.minIntrinsicWidth(cells, containerWidth));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double maxIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth)
    {
        return Math.Min(this.a.maxIntrinsicWidth(cells, containerWidth), this.b.maxIntrinsicWidth(cells, containerWidth));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? flex(IEnumerable<RenderBox> cells)
    {
        double? aFlex = this.a.flex(cells);
        double? bFlex = this.b.flex(cells);
        if ((aFlex is null))
        {
            return bFlex;
        }
        else
        {
            if ((bFlex is null))
            {
                return DartRuntimePrimitives.RequireValue(aFlex);
            }
        }
        return Math.Min(DartRuntimePrimitives.RequireValue(aFlex), DartRuntimePrimitives.RequireValue(bFlex));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "MinColumnWidth"))}({this.a}, {this.b})";
}

public enum TableCellVerticalAlignment
{
    top,
    middle,
    bottom,
    baseline,
    fill,
    intrinsicHeight
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
    internal virtual List<global::Doroti.Framework.Painting.Decoration?>? _rowDecorations { get; set; } = default;
    internal virtual List<global::Doroti.Framework.Painting.BoxPainter?>? _rowDecorationPainters { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.ImageConfiguration _configuration { get; set; } = default!;
    internal virtual TableCellVerticalAlignment _defaultVerticalAlignment { get; set; } = default!;
    internal virtual TextBaseline? _textBaseline { get; set; } = default;
    internal virtual DartMap<long, _Index__table> _idToIndexMap { get; private set; } = new DartMap<long, _Index__table>();
    internal virtual DartMap<long, global::Doroti.Framework.Semantics.SemanticsNode> _cachedRows { get; private set; } = new DartMap<long, global::Doroti.Framework.Semantics.SemanticsNode>();
    internal virtual DartMap<_Index__table, global::Doroti.Framework.Semantics.SemanticsNode> _cachedCells { get; private set; } = new DartMap<_Index__table, global::Doroti.Framework.Semantics.SemanticsNode>();
    internal virtual double? _baselineDistance { get; set; } = default;
    internal virtual List<double> _rowTops { get; private set; } = new List<double>();
    internal virtual IEnumerable<double>? _columnLefts { get; set; } = default;
    internal virtual double _tableWidth { get; set; } = default!;

    public RenderTable(long? columns = null, long? rows = null, DartMap<long, TableColumnWidth>? columnWidths = null, TableColumnWidth defaultColumnWidth = default!, TextDirection textDirection = default!, TableBorder? border = null, List<global::Doroti.Framework.Painting.Decoration?>? rowDecorations = null, global::Doroti.Framework.Painting.ImageConfiguration configuration = default!, TableCellVerticalAlignment defaultVerticalAlignment = TableCellVerticalAlignment.top, TextBaseline? textBaseline = null, List<List<RenderBox>>? children = null)
    {
        TableColumnWidth __defaultColumnWidth = defaultColumnWidth ?? new FlexColumnWidth();
        global::Doroti.Framework.Painting.ImageConfiguration __configuration = configuration ?? global::Doroti.Framework.Painting.ImageConfiguration.empty;
        this._textDirection = textDirection;
        this._columns = (columns ?? ((((children is not null) && (checked((long)(children.Count)) != 0)) ? checked((long)(children.First().Count)) : 0L)));
        this._rows = (rows ?? 0L);
        this._columnWidths = (columnWidths ?? new DartMap<long, TableColumnWidth>());
        this._defaultColumnWidth = __defaultColumnWidth;
        this._border = border;
        this._textBaseline = textBaseline;
        this._defaultVerticalAlignment = defaultVerticalAlignment;
        this._configuration = __configuration;
        System.Diagnostics.Debug.Assert(((columns is null) || (columns >= 0L)));
        System.Diagnostics.Debug.Assert(((rows is null) || (rows >= 0L)));
        System.Diagnostics.Debug.Assert(((rows is null) || (children is null)));
        _children = new List<RenderBox?>(System.Linq.Enumerable.Repeat<RenderBox?>(null, checked((int)(this._columns * this._rows))));
        this.rowDecorations = rowDecorations;
        children?.forEach(this.addRow);
    }

    public virtual long columns
    {
        get => this._columns;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value >= 0L));
            if ((DartRuntimePrimitives.RequireValue(__value) == this.columns))
            {
                return;
            }
            long oldColumns = this.columns;
            List<RenderBox?> oldChildren = this._children;
            _columns = DartRuntimePrimitives.RequireValue(__value);
            _children = new List<RenderBox?>(System.Linq.Enumerable.Repeat<RenderBox?>(null, checked((int)(this.columns * this.rows))));
            long columnsToCopy = Math.Min(this.columns, oldColumns);
            for (var y = 0L; (y < this.rows); y += 1L)
            {
                for (var x = 0L; (x < columnsToCopy); x += 1L)
                {
                    this._children[(int)((x + (y * this.columns)))] = oldChildren[(int)((x + (y * oldColumns)))];
                }
            }
            if ((oldColumns > this.columns))
            {
                for (var yLocal = 0L; (yLocal < this.rows); yLocal += 1L)
                {
                    for (long xLocal = this.columns; (xLocal < oldColumns); xLocal += 1L)
                    {
                        long xy = (xLocal + (yLocal * oldColumns));
                        if ((oldChildren[(int)(xy)] is not null))
                        {
                            dropChild(oldChildren[(int)(xy)]!);
                        }
                    }
                }
            }
            markNeedsLayout();
        }
    }
    public virtual long rows
    {
        get => this._rows;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value >= 0L));
            if ((DartRuntimePrimitives.RequireValue(__value) == this.rows))
            {
                return;
            }
            if ((this._rows > DartRuntimePrimitives.RequireValue(__value)))
            {
                for (long xy = (this.columns * DartRuntimePrimitives.RequireValue(__value)); (xy < checked((long)(this._children.Count))); xy += 1L)
                {
                    if ((this._children[(int)(xy)] is not null))
                    {
                        dropChild(this._children[(int)(xy)]!);
                    }
                }
            }
            _rows = DartRuntimePrimitives.RequireValue(__value);
            this._children.setLength((this.columns * this.rows));
            markNeedsLayout();
        }
    }
    public virtual DartMap<long, TableColumnWidth>? columnWidths
    {
        get => new DartMap<long, TableColumnWidth>(this._columnWidths);
        set
        {
            var __value = value;
            if ((object.Equals(this._columnWidths, __value)))
            {
                return;
            }
            if (((checked((long)(this._columnWidths.Count)) == 0) && (__value is null)))
            {
                return;
            }
            _columnWidths = (__value ?? new DartMap<long, TableColumnWidth>());
            markNeedsLayout();
        }
    }
    public virtual void setColumnWidth(long column, TableColumnWidth value)
    {
        if ((object.Equals(this._columnWidths.GetValueOrDefault(column), value)))
        {
            return;
        }
        this._columnWidths[column] = value;
        markNeedsLayout();
    }

    public virtual TableColumnWidth defaultColumnWidth
    {
        get => this._defaultColumnWidth;
        set
        {
            var __value = value;
            if ((object.Equals(this.defaultColumnWidth, __value)))
            {
                return;
            }
            _defaultColumnWidth = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => this._textDirection;
        set
        {
            var __value = value;
            if ((object.Equals(this._textDirection, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _textDirection = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual TableBorder? border
    {
        get => this._border;
        set
        {
            var __value = value;
            if ((object.Equals(this.border, __value)))
            {
                return;
            }
            _border = __value;
            markNeedsPaint();
        }
    }
    public virtual List<global::Doroti.Framework.Painting.Decoration?> rowDecorations
    {
        get => new List<global::Doroti.Framework.Painting.Decoration?>((this._rowDecorations ?? new List<global::Doroti.Framework.Painting.Decoration>()));
        set
        {
            var __value = value;
            if ((object.Equals(this._rowDecorations, __value)))
            {
                return;
            }
            _rowDecorations = __value;
            if ((this._rowDecorationPainters is not null))
            {
                foreach (global::Doroti.Framework.Painting.BoxPainter? painter in this._rowDecorationPainters!)
                {
                    painter?.dispose();
                }
            }
            _rowDecorationPainters = ((this._rowDecorations is not null) ? new List<global::Doroti.Framework.Painting.BoxPainter?>(System.Linq.Enumerable.Repeat<global::Doroti.Framework.Painting.BoxPainter?>(null, checked((int)checked((long)(this._rowDecorations!.Count))))) : null);
        }
    }
    public virtual global::Doroti.Framework.Painting.ImageConfiguration configuration
    {
        get => this._configuration;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._configuration)))
            {
                return;
            }
            _configuration = __value;
            markNeedsPaint();
        }
    }
    public virtual TableCellVerticalAlignment defaultVerticalAlignment
    {
        get => this._defaultVerticalAlignment;
        set
        {
            var __value = value;
            if ((object.Equals(this._defaultVerticalAlignment, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _defaultVerticalAlignment = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextBaseline? textBaseline
    {
        get => this._textBaseline;
        set
        {
            var __value = value;
            if ((object.Equals(this._textBaseline, __value)))
            {
                return;
            }
            _textBaseline = __value;
            markNeedsLayout();
        }
    }
    public override void setupParentData(RenderObject child)
    {
        if ((((RenderObject)child).parentData is not TableCellParentData))
        {
            child.parentData = new TableCellParentData();
        }
    }

    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.role = SemanticsRole.table;
        config.isSemanticBoundary = true;
        config.explicitChildNodes = true;
    }

    public override void clearSemantics()
    {
        base.clearSemantics();
        this._cachedRows.Clear();
        this._cachedCells.Clear();
    }

    public override void assembleSemanticsNode(global::Doroti.Framework.Semantics.SemanticsNode node, global::Doroti.Framework.Semantics.SemanticsConfiguration config, IEnumerable<global::Doroti.Framework.Semantics.SemanticsNode> children)
    {
        var rows = new List<global::Doroti.Framework.Semantics.SemanticsNode>();
        var rawCells = new List<List<List<global::Doroti.Framework.Semantics.SemanticsNode>>>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)this._rows)), ((rowIndex) => new List<List<global::Doroti.Framework.Semantics.SemanticsNode>>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)this._columns)), ((columnIndex) => new List<global::Doroti.Framework.Semantics.SemanticsNode>()))))));
        Rect rectWithOffset(global::Doroti.Framework.Semantics.SemanticsNode node)
        {
            global::Doroti.Ui.Offset offset = ((((((global::Doroti.Framework.Semantics.SemanticsNode)node).transform is not null) ? MatrixUtils.getAsTranslation(((global::Doroti.Framework.Semantics.SemanticsNode)node).transform!) : null)) ?? Offset.zero);
            return ((global::Doroti.Framework.Semantics.SemanticsNode)node).rect.shift(offset);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        long findRowIndex(double top)
        {
            for (long i = (this._rows - 1L); (i >= 0L); i--)
            {
                if ((this._rowTops[(int)(i)] <= top))
                {
                    return i;
                }
            }
            return -1L;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        long findColumnIndex(double left)
        {
            if ((this._columnLefts is null))
            {
                return -1L;
            }
            for (long iLocal = (this._columnLefts!.Count() - 1L); (iLocal >= 0L); iLocal--)
            {
                if ((this._columnLefts!.elementAt(iLocal) <= left))
                {
                    return iLocal;
                }
            }
            return -1L;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        void shiftTransform(global::Doroti.Framework.Semantics.SemanticsNode node, double dx, double dy)
        {
            Matrix4? previousTransform = ((global::Doroti.Framework.Semantics.SemanticsNode)node).transform;
            global::Doroti.Ui.Offset offsetLocal = ((((previousTransform is not null) ? MatrixUtils.getAsTranslation(previousTransform) : null)) ?? Offset.zero);
            var newTransform = Matrix4.translationValues((offsetLocal.dx + dx), (offsetLocal.dy + dy), 0);
            node.transform = newTransform;
        }
        foreach (var child in children)
        {
            if (this._idToIndexMap.ContainsKey(((global::Doroti.Framework.Semantics.SemanticsNode)child).id))
            {
                _Index__table index = this._idToIndexMap.GetValueOrDefault(((global::Doroti.Framework.Semantics.SemanticsNode)child).id)!;
                long yLocal = ((_Index__table)index).y;
                long xLocal = ((_Index__table)index).x;
                if (((yLocal < this._rows) && (xLocal < this._columns)))
                {
                    rawCells[(int)(yLocal)][(int)(xLocal)].Add(child);
                }
            }
            else
            {
                global::Doroti.Ui.Rect rectLocal = rectWithOffset(child);
                long yAlternate = findRowIndex(rectLocal.top);
                long xAlternate = findColumnIndex(rectLocal.left);
                if (((yAlternate != -1L) && (xAlternate != -1L)))
                {
                    rawCells[(int)(yAlternate)][(int)(xAlternate)].Add(child);
                }
            }
        }
        for (var yNested = 0L; (yNested < this._rows); yNested++)
        {
            global::Doroti.Ui.Rect rowBox = getRowBox(yNested);
            if ((rowBox.height == 0L))
            {
                continue;
            }
            global::Doroti.Framework.Semantics.SemanticsNode newRow = (this._cachedRows.GetValueOrDefault(yNested) ?? (this._cachedRows[yNested] = new global::Doroti.Framework.Semantics.SemanticsNode(showOnScreen: (() =>
            {
                showOnScreen(descendant: this, rect: rowBox);
            }))));
            var cells = new List<global::Doroti.Framework.Semantics.SemanticsNode>();
            for (var xNested = 0L; (xNested < this.columns); xNested++)
            {
                List<global::Doroti.Framework.Semantics.SemanticsNode> rawChildrens = rawCells[(int)(yNested)][(int)(xNested)];
                if ((checked((long)(rawChildrens.Count)) == 0))
                {
                    continue;
                }
                bool addCellWrapper = ((checked((long)(rawChildrens.Count)) > 1L) || (((!object.Equals(rawChildrens.Single().role, SemanticsRole.cell)) && (!object.Equals(rawChildrens.Single().role, SemanticsRole.columnHeader)))));
                global::Doroti.Framework.Semantics.SemanticsNode cellLocal = default!;
                if (!addCellWrapper)
                {
                    cellLocal = rawChildrens.Single();
                }
                else
                {
                    var indexLocal = new _Index__table(yNested, xNested);
                    cellLocal = ((Func<global::Doroti.Framework.Semantics.SemanticsNode>)(() =>
{
    var __cascade = this._cachedCells.putIfAbsent(indexLocal, (() => new global::Doroti.Framework.Semantics.SemanticsNode()));
    __cascade.updateWith(config: ((Func<global::Doroti.Framework.Semantics.SemanticsConfiguration>)(() =>
{
    var __cascade = new global::Doroti.Framework.Semantics.SemanticsConfiguration();
    __cascade.role = SemanticsRole.cell;
    return __cascade;
}))(), childrenInInversePaintOrder: rawChildrens);
    return __cascade;
}))();
                }
                double cellWidth = ((xNested == (this._columns - 1L)) ? (rowBox.width - this._columnLefts!.elementAt(xNested)) : (this._columnLefts!.elementAt((xNested + 1L)) - this._columnLefts!.elementAt(xNested)));
                if ((cellWidth <= 0.0))
                {
                    continue;
                }
                if (addCellWrapper)
                {
                    ((Func<global::Doroti.Framework.Semantics.SemanticsNode>)(() =>
{
    var __cascade = cellLocal;
    __cascade.transform = Matrix4.translationValues(this._columnLefts!.elementAt(xNested), 0, 0);
    __cascade.rect = global::Doroti.Ui.Rect.fromLTWH(0, 0, cellWidth, rowBox.height);
    return __cascade;
}))();
                }
                foreach (var childLocal in rawChildrens)
                {
                    this._idToIndexMap[((global::Doroti.Framework.Semantics.SemanticsNode)childLocal).id] = new _Index__table(yNested, xNested);
                    global::Doroti.Ui.Rect localRect = rectWithOffset(childLocal);
                    double dyLocal = ((localRect.bottom > (rowBox.height + global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)) ? -this._rowTops.elementAt(yNested) : 0.0);
                    double dxLocal = (addCellWrapper ? ((((localRect.left >= cellWidth)) ? -this._columnLefts!.elementAt(xNested) : 0.0)) : (((localRect.right <= this._columnLefts!.elementAt(xNested)) ? this._columnLefts!.elementAt(xNested) : 0.0)));
                    if (((dxLocal != 0L) || (dyLocal != 0L)))
                    {
                        shiftTransform(childLocal, dxLocal, dyLocal);
                    }
                }
                cellLocal.indexInParent = xNested;
                cells.Add(cellLocal);
            }
            ((Func<global::Doroti.Framework.Semantics.SemanticsNode>)(() =>
{
    var __cascade = newRow;
    __cascade.updateWith(config: ((Func<global::Doroti.Framework.Semantics.SemanticsConfiguration>)(() =>
{
    var __cascade = new global::Doroti.Framework.Semantics.SemanticsConfiguration();
    __cascade.indexInParent = yNested;
    __cascade.role = SemanticsRole.row;
    return __cascade;
}))(), childrenInInversePaintOrder: cells);
    __cascade.transform = Matrix4.translationValues(rowBox.left, rowBox.top, 0);
    __cascade.rect = global::Doroti.Ui.Rect.fromLTWH(0, 0, rowBox.width, rowBox.height);
    return __cascade;
}))();
            rows.Add(newRow);
        }
        node.updateWith(config: config, childrenInInversePaintOrder: rows);
    }

    public virtual void setFlatChildren(long columns, List<RenderBox?> cells)
    {
        if (((object.Equals(cells, this._children)) && (DartRuntimePrimitives.RequireValue(columns) == this._columns)))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => (columns >= 0L));
        if (((DartRuntimePrimitives.RequireValue(columns) == 0L) || (checked((long)(cells.Count)) == 0)))
        {
            DartRuntimePrimitives.Assert(() => (checked((long)(cells.Count)) == 0));
            _columns = DartRuntimePrimitives.RequireValue(columns);
            if ((checked((long)(this._children.Count)) == 0))
            {
                DartRuntimePrimitives.Assert(() => (this._rows == 0L));
                return;
            }
            foreach (RenderBox? oldChild in this._children)
            {
                if ((oldChild is not null))
                {
                    dropChild(oldChild);
                }
            }
            _rows = 0L;
            this._children.Clear();
            markNeedsLayout();
            return;
        }
        DartRuntimePrimitives.Assert(() => ((checked((long)(cells.Count)) % DartRuntimePrimitives.RequireValue(columns)) == 0L));
        HashSet<RenderBox> lostChildren = new HashSet<RenderBox>();
        for (var y = 0L; (y < this._rows); y += 1L)
        {
            for (var x = 0L; (x < this._columns); x += 1L)
            {
                long xyOld = (x + (y * this._columns));
                long xyNew = (x + (y * DartRuntimePrimitives.RequireValue(columns)));
                if (((xyOld < checked((long)(this._children.Count))) && (this._children[(int)(xyOld)] is not null) && ((((x >= DartRuntimePrimitives.RequireValue(columns)) || (xyNew >= checked((long)(cells.Count)))) || (!object.Equals(this._children[(int)(xyOld)], cells[(int)(xyNew)]))))))
                {
                    lostChildren.Add(this._children[(int)(xyOld)]!);
                }
            }
        }
        var yLocal = 0L;
        while (((yLocal * DartRuntimePrimitives.RequireValue(columns)) < checked((long)(cells.Count))))
        {
            for (var xLocal = 0L; (xLocal < DartRuntimePrimitives.RequireValue(columns)); xLocal += 1L)
            {
                long xyNewLocal = (xLocal + (yLocal * DartRuntimePrimitives.RequireValue(columns)));
                long xyOldLocal = (xLocal + (yLocal * this._columns));
                if (((cells[(int)(xyNewLocal)] is not null) && ((((xLocal >= this._columns) || (yLocal >= this._rows)) || (xyOldLocal >= checked((long)(this._children.Count))) || (!object.Equals(this._children[(int)(xyOldLocal)], cells[(int)(xyNewLocal)]))))))
                {
                    if (!lostChildren.Remove(cells[(int)(xyNewLocal)]))
                    {
                        adoptChild(cells[(int)(xyNewLocal)]!);
                    }
                }
            }
            yLocal += 1L;
        }
        lostChildren.forEach(dropChild);
        _columns = DartRuntimePrimitives.RequireValue(columns);
        _rows = (checked((long)(checked((long)(cells.Count)) / DartRuntimePrimitives.RequireValue(columns))));
        _children = new List<RenderBox?>(cells);
        DartRuntimePrimitives.Assert(() => (checked((long)(this._children.Count)) == (this.rows * DartRuntimePrimitives.RequireValue(columns))));
        markNeedsLayout();
    }

    public virtual void setChildren(List<List<RenderBox>>? cells)
    {
        if ((cells is null))
        {
            setFlatChildren(0L, new List<RenderBox?>());
            return;
        }
        foreach (RenderBox? oldChild in this._children)
        {
            if ((oldChild is not null))
            {
                dropChild(oldChild);
            }
        }
        this._children.Clear();
        _columns = ((checked((long)(cells.Count)) != 0) ? checked((long)(cells.First().Count)) : 0L);
        _rows = 0L;
        cells.forEach(this.addRow);
        DartRuntimePrimitives.Assert(() => (checked((long)(this._children.Count)) == (this.rows * this.columns)));
    }

    public virtual void addRow(List<RenderBox?> cells)
    {
        DartRuntimePrimitives.Assert(() => (checked((long)(cells.Count)) == this.columns));
        DartRuntimePrimitives.Assert(() => (checked((long)(this._children.Count)) == (this.rows * this.columns)));
        _rows += 1L;
        this._children.AddRange(cells);
        foreach (var cell in cells)
        {
            if ((cell is not null))
            {
                adoptChild(cell);
            }
        }
        markNeedsLayout();
    }

    public virtual void setChild(long x, long y, RenderBox? value)
    {
        DartRuntimePrimitives.Assert(() => ((((x >= 0L) && (x < this.columns)) && (y >= 0L)) && (y < this.rows)));
        DartRuntimePrimitives.Assert(() => (checked((long)(this._children.Count)) == (this.rows * this.columns)));
        long xy = (x + (y * this.columns));
        RenderBox? oldChild = this._children[(int)(xy)];
        if ((object.Equals(oldChild, value)))
        {
            return;
        }
        if ((oldChild is not null))
        {
            dropChild(oldChild);
        }
        this._children[(int)(xy)] = value;
        if ((value is not null))
        {
            adoptChild(value);
        }
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        foreach (RenderBox? child in this._children)
        {
            child?.attach(owner);
        }
    }

    public override void detach()
    {
        base.detach();
        if ((this._rowDecorationPainters is not null))
        {
            foreach (global::Doroti.Framework.Painting.BoxPainter? painter in this._rowDecorationPainters!)
            {
                painter?.dispose();
            }
            _rowDecorationPainters = new List<global::Doroti.Framework.Painting.BoxPainter?>(System.Linq.Enumerable.Repeat<global::Doroti.Framework.Painting.BoxPainter?>(null, checked((int)checked((long)(this._rowDecorations!.Count)))));
        }
        foreach (RenderBox? child in this._children)
        {
            child?.detach();
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        DartRuntimePrimitives.Assert(() => (checked((long)(this._children.Count)) == (this.rows * this.columns)));
        foreach (RenderBox? child in this._children)
        {
            if ((child is not null))
            {
                visitor(child);
            }
        }
    }

    public override void redepthChildren()
    {
        visitChildren((Action<RenderObject>)redepthChild);
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        DartRuntimePrimitives.Assert(() => (checked((long)(this._children.Count)) == (this.rows * this.columns)));
        if (((this.rows * this.columns) == 0L))
        {
            return 0.0;
        }
        var totalMinWidth = 0.0;
        for (var x = 0L; (x < this.columns); x += 1L)
        {
            TableColumnWidth columnWidth = (this._columnWidths.GetValueOrDefault(x) ?? this.defaultColumnWidth);
            IEnumerable<RenderBox> columnCells = column(x);
            totalMinWidth += columnWidth.minIntrinsicWidth(columnCells, double.PositiveInfinity);
        }
        return totalMinWidth;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        DartRuntimePrimitives.Assert(() => (checked((long)(this._children.Count)) == (this.rows * this.columns)));
        if (((this.rows * this.columns) == 0L))
        {
            return 0.0;
        }
        var totalMaxWidth = 0.0;
        for (var x = 0L; (x < this.columns); x += 1L)
        {
            TableColumnWidth columnWidth = (this._columnWidths.GetValueOrDefault(x) ?? this.defaultColumnWidth);
            IEnumerable<RenderBox> columnCells = column(x);
            totalMaxWidth += columnWidth.maxIntrinsicWidth(columnCells, double.PositiveInfinity);
        }
        return totalMaxWidth;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        DartRuntimePrimitives.Assert(() => (checked((long)(this._children.Count)) == (this.rows * this.columns)));
        if (((this.rows * this.columns) == 0L))
        {
            return 0.0;
        }
        List<double> widths = _computeColumnWidths(BoxConstraints.CreateTightForFinite(width: width));
        var rowTop = 0.0;
        for (var y = 0L; (y < this.rows); y += 1L)
        {
            var rowHeight = 0.0;
            for (var x = 0L; (x < this.columns); x += 1L)
            {
                long xy = (x + (y * this.columns));
                RenderBox? child = this._children[(int)(xy)];
                if ((child is not null))
                {
                    rowHeight = Math.Max(rowHeight, child.getMaxIntrinsicHeight(widths[(int)(x)]));
                }
            }
            rowTop += rowHeight;
        }
        return rowTop;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return getMinIntrinsicHeight(width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        return this._baselineDistance;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual IEnumerable<RenderBox> column(long x)
    {
        for (var y = 0L; (y < this.rows); y += 1L)
        {
            long xy = (x + (y * this.columns));
            RenderBox? child = this._children[(int)(xy)];
            if ((child is not null))
            {
                yield return child;
            }
        }
    }

    public virtual IEnumerable<RenderBox> row(long y)
    {
        long start = (y * this.columns);
        long end = (((y + 1L)) * this.columns);
        for (var xy = start; (xy < end); xy += 1L)
        {
            RenderBox? child = this._children[(int)(xy)];
            if ((child is not null))
            {
                yield return child;
            }
        }
    }

    internal virtual List<double> _computeColumnWidths(BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => (checked((long)(this._children.Count)) == (this.rows * this.columns)));
        var widths = new List<double>(System.Linq.Enumerable.Repeat<double>(0.0, checked((int)this.columns)));
        var minWidths = new List<double>(System.Linq.Enumerable.Repeat<double>(0.0, checked((int)this.columns)));
        var flexes = new List<double?>(System.Linq.Enumerable.Repeat<double?>(null, checked((int)this.columns)));
        var tableWidth = 0.0;
        var unflexedTableWidth = 0.0;
        var totalFlex = 0.0;
        for (var x = 0L; (x < this.columns); x += 1L)
        {
            TableColumnWidth columnWidth = (this._columnWidths.GetValueOrDefault(x) ?? this.defaultColumnWidth);
            IEnumerable<RenderBox> columnCells = column(x);
            double maxIntrinsicWidthLocal = columnWidth.maxIntrinsicWidth(columnCells, ((BoxConstraints)constraints).maxWidth);
            DartRuntimePrimitives.Assert(() => double.IsFinite(maxIntrinsicWidthLocal));
            DartRuntimePrimitives.Assert(() => (maxIntrinsicWidthLocal >= 0.0));
            widths[(int)(x)] = maxIntrinsicWidthLocal;
            tableWidth += maxIntrinsicWidthLocal;
            double minIntrinsicWidthLocal = columnWidth.minIntrinsicWidth(columnCells, ((BoxConstraints)constraints).maxWidth);
            DartRuntimePrimitives.Assert(() => double.IsFinite(minIntrinsicWidthLocal));
            DartRuntimePrimitives.Assert(() => (minIntrinsicWidthLocal >= 0.0));
            minWidths[(int)(x)] = minIntrinsicWidthLocal;
            DartRuntimePrimitives.Assert(() => (maxIntrinsicWidthLocal >= minIntrinsicWidthLocal));
            double? flexLocal = columnWidth.flex(columnCells);
            if ((flexLocal is not null))
            {
                double flex__38095__value38143 = DartRuntimePrimitives.RequireValue(flexLocal);
                DartRuntimePrimitives.Assert(() => double.IsFinite(DartRuntimePrimitives.RequireValue(flex__38095__value38143)));
                DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.RequireValue(flex__38095__value38143) > 0.0));
                flexes[(int)(x)] = DartRuntimePrimitives.RequireValue(flex__38095__value38143);
                totalFlex += DartRuntimePrimitives.RequireValue(flex__38095__value38143);
            }
            else
            {
                unflexedTableWidth = (unflexedTableWidth + maxIntrinsicWidthLocal);
            }
        }
        double maxWidthConstraint = ((BoxConstraints)constraints).maxWidth;
        double minWidthConstraint = ((BoxConstraints)constraints).minWidth;
        if ((totalFlex > 0.0))
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
            if ((tableWidth < targetWidth))
            {
                double remainingWidth = (targetWidth - unflexedTableWidth);
                DartRuntimePrimitives.Assert(() => double.IsFinite(remainingWidth));
                DartRuntimePrimitives.Assert(() => (remainingWidth >= 0.0));
                for (var xLocal = 0L; (xLocal < this.columns); xLocal += 1L)
                {
                    if ((flexes[(int)(xLocal)] is not null))
                    {
                        double flexedWidth = ((remainingWidth * DartRuntimePrimitives.RequireValue(flexes[(int)(xLocal)])) / totalFlex);
                        DartRuntimePrimitives.Assert(() => double.IsFinite(flexedWidth));
                        DartRuntimePrimitives.Assert(() => (flexedWidth >= 0.0));
                        if ((widths[(int)(xLocal)] < flexedWidth))
                        {
                            double delta = (flexedWidth - widths[(int)(xLocal)]);
                            tableWidth += delta;
                            widths[(int)(xLocal)] = flexedWidth;
                        }
                    }
                }
                DartRuntimePrimitives.Assert(() => ((tableWidth + global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance) >= targetWidth));
            }
        }
        else
        {
            if ((tableWidth < minWidthConstraint))
            {
                double deltaLocal = (((minWidthConstraint - tableWidth)) / this.columns);
                for (var xAlternate = 0L; (xAlternate < this.columns); xAlternate += 1L)
                {
                    widths[(int)(xAlternate)] = (widths[(int)(xAlternate)] + deltaLocal);
                }
                tableWidth = minWidthConstraint;
            }
        }
        if ((tableWidth > maxWidthConstraint))
        {
            double deficit = (tableWidth - maxWidthConstraint);
            long availableColumns = this.columns;
            while (((deficit > global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance) && (totalFlex > global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)))
            {
                var newTotalFlex = 0.0;
                for (var xNested = 0L; (xNested < this.columns); xNested += 1L)
                {
                    if ((flexes[(int)(xNested)] is not null))
                    {
                        double newWidth = (widths[(int)(xNested)] - ((deficit * DartRuntimePrimitives.RequireValue(flexes[(int)(xNested)])) / totalFlex));
                        DartRuntimePrimitives.Assert(() => double.IsFinite(newWidth));
                        if ((newWidth <= minWidths[(int)(xNested)]))
                        {
                            deficit -= (widths[(int)(xNested)] - minWidths[(int)(xNested)]);
                            widths[(int)(xNested)] = minWidths[(int)(xNested)];
                            flexes[(int)(xNested)] = null;
                            availableColumns -= 1L;
                        }
                        else
                        {
                            deficit -= (widths[(int)(xNested)] - newWidth);
                            widths[(int)(xNested)] = newWidth;
                            newTotalFlex += DartRuntimePrimitives.RequireValue(flexes[(int)(xNested)]);
                        }
                        DartRuntimePrimitives.Assert(() => (widths[(int)(xNested)] >= 0.0));
                    }
                }
                totalFlex = newTotalFlex;
            }
            while (((deficit > global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance) && (availableColumns > 0L)))
            {
                double deltaAlternate = (deficit / availableColumns);
                DartRuntimePrimitives.Assert(() => (deltaAlternate != 0L));
                var newAvailableColumns = 0L;
                for (var xCurrent = 0L; (xCurrent < this.columns); xCurrent += 1L)
                {
                    double availableDelta = (widths[(int)(xCurrent)] - minWidths[(int)(xCurrent)]);
                    if ((availableDelta > 0.0))
                    {
                        if ((availableDelta <= deltaAlternate))
                        {
                            deficit -= (widths[(int)(xCurrent)] - minWidths[(int)(xCurrent)]);
                            widths[(int)(xCurrent)] = minWidths[(int)(xCurrent)];
                        }
                        else
                        {
                            deficit -= deltaAlternate;
                            widths[(int)(xCurrent)] = (widths[(int)(xCurrent)] - deltaAlternate);
                            newAvailableColumns += 1L;
                        }
                    }
                }
                availableColumns = newAvailableColumns;
            }
        }
        return widths;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Rect getRowBox(long row)
    {
        DartRuntimePrimitives.Assert(() => (row >= 0L));
        DartRuntimePrimitives.Assert(() => (row < this.rows));
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        return global::Doroti.Ui.Rect.fromLTRB(0.0, this._rowTops[(int)(row)], size.width, this._rowTops[(int)((row + 1L))]);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        if (((this.rows * this.columns) == 0L))
        {
            return null;
        }
        List<double> widths = _computeColumnWidths(constraints);
        double? baselineOffset = default!;
        for (var col = 0L; (col < this.columns); col += 1L)
        {
            RenderBox? child = this._children[(int)(col)];
            var childConstraints = BoxConstraints.CreateTightFor(width: widths[(int)(col)]);
            if ((child is null))
            {
                continue;
            }
            var childParentData = ((TableCellParentData?)(object?)child.parentData!)!;
            double? childBaseline = ((((TableCellParentData)childParentData).verticalAlignment ?? this.defaultVerticalAlignment) switch { TableCellVerticalAlignment.baseline => child.getDryBaseline(childConstraints, baseline), TableCellVerticalAlignment.baseline or TableCellVerticalAlignment.top or TableCellVerticalAlignment.middle or TableCellVerticalAlignment.bottom or TableCellVerticalAlignment.fill => null, TableCellVerticalAlignment.intrinsicHeight => null, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            if (((childBaseline is not null) && (((baselineOffset is null) || (DartRuntimePrimitives.RequireValue(baselineOffset) < DartRuntimePrimitives.RequireValue(childBaseline))))))
            {
                double childBaseline__44477__value44974 = DartRuntimePrimitives.RequireValue(childBaseline);
                baselineOffset = DartRuntimePrimitives.RequireValue(childBaseline__44477__value44974);
            }
        }
        return baselineOffset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        if (((this.rows * this.columns) == 0L))
        {
            return constraints.constrain(Size.zero);
        }
        List<double> widths = _computeColumnWidths(constraints);
        double tableWidth = System.Linq.Enumerable.Aggregate(widths, (double)0.0, ((a, b) => (a + b)));
        var rowTop = 0.0;
        for (var y = 0L; (y < this.rows); y += 1L)
        {
            var rowHeight = 0.0;
            for (var x = 0L; (x < this.columns); x += 1L)
            {
                long xy = (x + (y * this.columns));
                RenderBox? child = this._children[(int)(xy)];
                if ((child is not null))
                {
                    var childParentData = ((TableCellParentData?)(object?)child.parentData!)!;
                    switch ((((TableCellParentData)childParentData).verticalAlignment ?? this.defaultVerticalAlignment))
                    {
                        case TableCellVerticalAlignment.baseline:
                            {
                                DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(reason: "TableCellVerticalAlignment.baseline requires a full layout for baseline metrics to be available."));
                                return Size.zero;
                            }
                        case TableCellVerticalAlignment.top:
                        case TableCellVerticalAlignment.middle:
                        case TableCellVerticalAlignment.bottom:
                        case TableCellVerticalAlignment.intrinsicHeight:
                            {
                                global::Doroti.Ui.Size childSize = child.getDryLayout(BoxConstraints.CreateTightFor(width: widths[(int)(x)]));
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
        return constraints.constrain(new global::Doroti.Ui.Size(tableWidth, rowTop));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraintsLocal = this.constraints;
        long rowsLocal = this.rows;
        long columnsLocal = this.columns;
        DartRuntimePrimitives.Assert(() => (checked((long)(this._children.Count)) == (DartRuntimePrimitives.RequireValue(rowsLocal) * DartRuntimePrimitives.RequireValue(columnsLocal))));
        if (((DartRuntimePrimitives.RequireValue(rowsLocal) * DartRuntimePrimitives.RequireValue(columnsLocal)) == 0L))
        {
            _tableWidth = 0.0;
            size = constraintsLocal.constrain(Size.zero);
            return;
        }
        List<double> widths = _computeColumnWidths(constraintsLocal);
        var positions = new List<double>(System.Linq.Enumerable.Repeat<double>(0.0, checked((int)DartRuntimePrimitives.RequireValue(columnsLocal))));
        switch (this.textDirection)
        {
            case TextDirection.rtl:
                {
                    positions[(int)((columnsLocal - 1L))] = 0.0;
                    for (long xLocal = (DartRuntimePrimitives.RequireValue(columnsLocal) - 2L); (xLocal >= 0L); xLocal -= 1L)
                    {
                        positions[(int)(xLocal)] = (positions[(int)((xLocal + 1L))] + widths[(int)((xLocal + 1L))]);
                    }
                    _columnLefts = System.Linq.Enumerable.Reverse(positions);
                    _tableWidth = (positions.First() + widths.First());
                    break;
                }
            case TextDirection.ltr:
                {
                    positions[(int)(0L)] = 0.0;
                    for (var xAlternate = 1L; (xAlternate < DartRuntimePrimitives.RequireValue(columnsLocal)); xAlternate += 1L)
                    {
                        positions[(int)(xAlternate)] = (positions[(int)((xAlternate - 1L))] + widths[(int)((xAlternate - 1L))]);
                    }
                    _columnLefts = positions;
                    _tableWidth = (positions.Last() + widths.Last());
                    break;
                }
        }
        this._rowTops.Clear();
        _baselineDistance = null;
        var rowTop = 0.0;
        for (var yLocal = 0L; (yLocal < DartRuntimePrimitives.RequireValue(rowsLocal)); yLocal += 1L)
        {
            this._rowTops.Add(rowTop);
            var rowHeight = 0.0;
            var haveBaseline = false;
            var beforeBaselineDistance = 0.0;
            var afterBaselineDistance = 0.0;
            var baselines = new List<double>(System.Linq.Enumerable.Repeat<double>(0.0, checked((int)DartRuntimePrimitives.RequireValue(columnsLocal))));
            for (var xNested = 0L; (xNested < DartRuntimePrimitives.RequireValue(columnsLocal)); xNested += 1L)
            {
                long xy = (xNested + (yLocal * DartRuntimePrimitives.RequireValue(columnsLocal)));
                RenderBox? child = this._children[(int)(xy)];
                if ((child is not null))
                {
                    var childParentData = ((TableCellParentData?)(object?)child.parentData!)!;
                    childParentData.x = xNested;
                    childParentData.y = yLocal;
                    switch ((((TableCellParentData)childParentData).verticalAlignment ?? this.defaultVerticalAlignment))
                    {
                        case TableCellVerticalAlignment.baseline:
                            {
                                DartRuntimePrimitives.Assert(() => (this.textBaseline is not null));
                                child.layout(BoxConstraints.CreateTightFor(width: widths[(int)(xNested)]), parentUsesSize: true);
                                double? childBaseline = child.getDistanceToBaseline(DartRuntimePrimitives.RequireValue(this.textBaseline), onlyReal: true);
                                if ((childBaseline is not null))
                                {
                                    double childBaseline__49094__value49237 = DartRuntimePrimitives.RequireValue(childBaseline);
                                    beforeBaselineDistance = Math.Max(beforeBaselineDistance, DartRuntimePrimitives.RequireValue(childBaseline__49094__value49237));
                                    afterBaselineDistance = Math.Max(afterBaselineDistance, (((RenderBox)child).size.height - DartRuntimePrimitives.RequireValue(childBaseline__49094__value49237)));
                                    baselines[(int)(xNested)] = DartRuntimePrimitives.RequireValue(childBaseline__49094__value49237);
                                    haveBaseline = true;
                                }
                                else
                                {
                                    rowHeight = Math.Max(rowHeight, ((RenderBox)child).size.height);
                                    childParentData.offset = new global::Doroti.Ui.Offset(positions[(int)(xNested)], rowTop);
                                }
                                break;
                            }
                        case TableCellVerticalAlignment.top:
                        case TableCellVerticalAlignment.middle:
                        case TableCellVerticalAlignment.bottom:
                        case TableCellVerticalAlignment.intrinsicHeight:
                            {
                                child.layout(BoxConstraints.CreateTightFor(width: widths[(int)(xNested)]), parentUsesSize: true);
                                rowHeight = Math.Max(rowHeight, ((RenderBox)child).size.height);
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
                if ((yLocal == 0L))
                {
                    _baselineDistance = beforeBaselineDistance;
                }
                rowHeight = Math.Max(rowHeight, (beforeBaselineDistance + afterBaselineDistance));
            }
            for (var xCurrent = 0L; (xCurrent < DartRuntimePrimitives.RequireValue(columnsLocal)); xCurrent += 1L)
            {
                long xyLocal = (xCurrent + (yLocal * DartRuntimePrimitives.RequireValue(columnsLocal)));
                RenderBox? childLocal = this._children[(int)(xyLocal)];
                if ((childLocal is not null))
                {
                    var childParentDataLocal = ((TableCellParentData?)(object?)childLocal.parentData!)!;
                    switch ((((TableCellParentData)childParentDataLocal).verticalAlignment ?? this.defaultVerticalAlignment))
                    {
                        case TableCellVerticalAlignment.baseline:
                            {
                                childParentDataLocal.offset = new global::Doroti.Ui.Offset(positions[(int)(xCurrent)], ((rowTop + beforeBaselineDistance) - baselines[(int)(xCurrent)]));
                                break;
                            }
                        case TableCellVerticalAlignment.top:
                            {
                                childParentDataLocal.offset = new global::Doroti.Ui.Offset(positions[(int)(xCurrent)], rowTop);
                                break;
                            }
                        case TableCellVerticalAlignment.middle:
                            {
                                childParentDataLocal.offset = new global::Doroti.Ui.Offset(positions[(int)(xCurrent)], (rowTop + (((rowHeight - ((RenderBox)childLocal).size.height)) / 2.0)));
                                break;
                            }
                        case TableCellVerticalAlignment.bottom:
                            {
                                childParentDataLocal.offset = new global::Doroti.Ui.Offset(positions[(int)(xCurrent)], ((rowTop + rowHeight) - ((RenderBox)childLocal).size.height));
                                break;
                            }
                        case TableCellVerticalAlignment.fill:
                        case TableCellVerticalAlignment.intrinsicHeight:
                            {
                                childLocal.layout(BoxConstraints.CreateTightFor(width: widths[(int)(xCurrent)], height: rowHeight));
                                childParentDataLocal.offset = new global::Doroti.Ui.Offset(positions[(int)(xCurrent)], rowTop);
                                break;
                            }
                    }
                }
            }
            rowTop += rowHeight;
        }
        this._rowTops.Add(rowTop);
        size = constraintsLocal.constrain(new global::Doroti.Ui.Size(this._tableWidth, rowTop));
        DartRuntimePrimitives.Assert(() => (checked((long)(this._rowTops.Count)) == (DartRuntimePrimitives.RequireValue(rowsLocal) + 1L)));
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        DartRuntimePrimitives.Assert(() => (checked((long)(this._children.Count)) == (this.rows * this.columns)));
        for (long index = (checked((long)(this._children.Count)) - 1L); (index >= 0L); index -= 1L)
        {
            RenderBox? child = this._children[(int)(index)];
            if ((child is not null))
            {
                var childParentData = ((BoxParentData?)(object?)child.parentData!)!;
                bool isHit = result.addWithPaintOffset(offset: ((BoxParentData)childParentData).offset, position: position, hitTest: ((Func<BoxHitTestResult, Offset, bool>)((result, transformed) =>
                {
                    DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - ((BoxParentData)childParentData).offset))));
                    return child.hitTest(result, position: transformed);
                    return default;
                })));
                if (isHit)
                {
                    return true;
                }
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() => (checked((long)(this._children.Count)) == (this.rows * this.columns)));
        if (((this.rows * this.columns) == 0L))
        {
            if ((this.border is not null))
            {
                var borderRect = global::Doroti.Ui.Rect.fromLTWH(offset.dx, offset.dy, this._tableWidth, 0.0);
                this.border!.paint(((PaintingContext)context).canvas, borderRect, rows: new List<double>(), columns: new List<double>());
            }
            return;
        }
        DartRuntimePrimitives.Assert(() => (checked((long)(this._rowTops.Count)) == (this.rows + 1L)));
        if ((this._rowDecorations is not null))
        {
            DartRuntimePrimitives.Assert(() => (checked((long)(this._rowDecorations!.Count)) == checked((long)(this._rowDecorationPainters!.Count))));
            global::Doroti.Ui.Canvas canvasLocal = ((PaintingContext)context).canvas;
            for (var y = 0L; (y < this.rows); y += 1L)
            {
                if ((checked((long)(this._rowDecorations!.Count)) <= y))
                {
                    break;
                }
                if ((this._rowDecorations![(int)(y)] is not null))
                {
                    this._rowDecorationPainters![(int)(y)] ??= this._rowDecorations![(int)(y)]!.createBoxPainter((Action)markNeedsPaint);
                    this._rowDecorationPainters![(int)(y)]!.paint(canvasLocal, new global::Doroti.Ui.Offset(offset.dx, (offset.dy + this._rowTops[(int)(y)])), this.configuration.copyWith(size: new global::Doroti.Ui.Size(size.width, (this._rowTops[(int)((y + 1L))] - this._rowTops[(int)(y)]))));
                }
            }
        }
        for (var index = 0L; (index < checked((long)(this._children.Count))); index += 1L)
        {
            RenderBox? child = this._children[(int)(index)];
            if ((child is not null))
            {
                var childParentData = ((BoxParentData?)(object?)child.parentData!)!;
                context.paintChild(child, (((BoxParentData)childParentData).offset + offset));
            }
        }
        DartRuntimePrimitives.Assert(() => (this._rows == (checked((long)(this._rowTops.Count)) - 1L)));
        DartRuntimePrimitives.Assert(() => (this._columns == this._columnLefts!.Count()));
        if ((this.border is not null))
        {
            var borderRectLocal = global::Doroti.Ui.Rect.fromLTWH(offset.dx, offset.dy, this._tableWidth, this._rowTops.Last());
            IEnumerable<double> rowsLocal = this._rowTops.GetRange(1L, (checked((long)(this._rowTops.Count)) - 1L));
            IEnumerable<double> columnsLocal = this._columnLefts!.skip(1L);
            this.border!.paint(((PaintingContext)context).canvas, borderRectLocal, rows: rowsLocal, columns: columnsLocal);
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<TableBorder>("border", this.border, defaultValue: null));
        properties.add(new DiagnosticsProperty<DartMap<long, TableColumnWidth>>("specified column widths", this._columnWidths, level: ((checked((long)(this._columnWidths.Count)) == 0) ? DiagnosticLevel.hidden : DiagnosticLevel.info)));
        properties.add(new DiagnosticsProperty<TableColumnWidth>("default column width", this.defaultColumnWidth));
        properties.add(new MessageProperty("table size", $"{this.columns}×{this.rows}"));
        properties.add(new IterableProperty<string>("column offsets", this._columnLefts?.map<double, string>((value => global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(value))), ifNull: "unknown"));
        properties.add(new IterableProperty<string>("row offsets", this._rowTops.map<double, string>((value => global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(value))), ifNull: "unknown"));
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        if ((checked((long)(this._children.Count)) == 0))
        {
            return new List<DiagnosticsNode> { new DiagnosticsNode("table is empty") };
        }
        return new List<DiagnosticsNode>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((__other is not _Index__table))
        {
            return false;
        }
        return ((this.y == ((_Index__table)((_Index__table)__other)).y) && (this.x == ((_Index__table)((_Index__table)__other)).x));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.y, this.x);
}
