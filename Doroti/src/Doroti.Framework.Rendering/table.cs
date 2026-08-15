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

namespace Doroti.Generated.Framework.Rendering;

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
    public override string ToString() => global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "TableColumnWidth");
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
        var result__4413 = 0.0;
        foreach (var cell__4442 in cells)
        {
            result__4413 = Math.Max(result__4413, cell__4442.getMinIntrinsicWidth(double.PositiveInfinity));
        }
        return result__4413;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double maxIntrinsicWidth(IEnumerable<RenderBox> cells, double containerWidth)
    {
        var result__4665 = 0.0;
        foreach (var cell__4694 in cells)
        {
            result__4665 = Math.Max(result__4665, cell__4694.getMaxIntrinsicWidth(double.PositiveInfinity));
        }
        return result__4665;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? flex(IEnumerable<RenderBox> cells) => this._flex;
    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "IntrinsicColumnWidth"))}(flex: {this._flex?.toStringAsFixed(1L)})";
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

    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "FixedColumnWidth"))}({(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(this.value))})";
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

    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "FractionColumnWidth"))}({this.value})";
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

    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "FlexColumnWidth"))}({(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(this.value))})";
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
        double? aFlex__8998 = this.a.flex(cells);
        double? bFlex__9039 = this.b.flex(cells);
        if ((aFlex__8998 is null))
        {
            return bFlex__9039;
        }
        else
        {
            if ((bFlex__9039 is null))
            {
                return DartRuntimePrimitives.RequireValue(aFlex__8998);
            }
        }
        return Math.Max(DartRuntimePrimitives.RequireValue(aFlex__8998), DartRuntimePrimitives.RequireValue(bFlex__9039));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "MaxColumnWidth"))}({this.a}, {this.b})";
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
        double? aFlex__10564 = this.a.flex(cells);
        double? bFlex__10605 = this.b.flex(cells);
        if ((aFlex__10564 is null))
        {
            return bFlex__10605;
        }
        else
        {
            if ((bFlex__10605 is null))
            {
                return DartRuntimePrimitives.RequireValue(aFlex__10564);
            }
        }
        return Math.Min(DartRuntimePrimitives.RequireValue(aFlex__10564), DartRuntimePrimitives.RequireValue(bFlex__10605));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "MinColumnWidth"))}({this.a}, {this.b})";
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
    internal virtual List<global::Doroti.Generated.Framework.Painting.Decoration?>? _rowDecorations { get; set; } = default;
    internal virtual List<global::Doroti.Generated.Framework.Painting.BoxPainter?>? _rowDecorationPainters { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Painting.ImageConfiguration _configuration { get; set; } = default!;
    internal virtual TableCellVerticalAlignment _defaultVerticalAlignment { get; set; } = default!;
    internal virtual TextBaseline? _textBaseline { get; set; } = default;
    internal virtual DartMap<long, _Index__table> _idToIndexMap { get; private set; } = new DartMap<long, _Index__table>();
    internal virtual DartMap<long, global::Doroti.Generated.Framework.Semantics.SemanticsNode> _cachedRows { get; private set; } = new DartMap<long, global::Doroti.Generated.Framework.Semantics.SemanticsNode>();
    internal virtual DartMap<_Index__table, global::Doroti.Generated.Framework.Semantics.SemanticsNode> _cachedCells { get; private set; } = new DartMap<_Index__table, global::Doroti.Generated.Framework.Semantics.SemanticsNode>();
    internal virtual double? _baselineDistance { get; set; } = default;
    internal virtual List<double> _rowTops { get; private set; } = new List<double>();
    internal virtual IEnumerable<double>? _columnLefts { get; set; } = default;
    internal virtual double _tableWidth { get; set; } = default!;

    public RenderTable(long? columns = null, long? rows = null, DartMap<long, TableColumnWidth>? columnWidths = null, TableColumnWidth defaultColumnWidth = default!, TextDirection textDirection = default!, TableBorder? border = null, List<global::Doroti.Generated.Framework.Painting.Decoration?>? rowDecorations = null, global::Doroti.Generated.Framework.Painting.ImageConfiguration configuration = default!, TableCellVerticalAlignment defaultVerticalAlignment = TableCellVerticalAlignment.top, TextBaseline? textBaseline = null, List<List<RenderBox>>? children = null)
    {
        TableColumnWidth __defaultColumnWidth = defaultColumnWidth ?? new FlexColumnWidth();
        global::Doroti.Generated.Framework.Painting.ImageConfiguration __configuration = configuration ?? global::Doroti.Generated.Framework.Painting.ImageConfiguration.empty;
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
            long oldColumns__14785 = this.columns;
            List<RenderBox?> oldChildren__14834 = this._children;
            _columns = DartRuntimePrimitives.RequireValue(__value);
            _children = new List<RenderBox?>(System.Linq.Enumerable.Repeat<RenderBox?>(null, checked((int)(this.columns * this.rows))));
            long columnsToCopy__14958 = Math.Min(this.columns, oldColumns__14785);
            for (var y__15018 = 0L; (y__15018 < this.rows); y__15018 += 1L)
            {
                for (var x__15060 = 0L; (x__15060 < columnsToCopy__14958); x__15060 += 1L)
                {
                    this._children[(int)((x__15060 + (y__15018 * this.columns)))] = oldChildren__14834[(int)((x__15060 + (y__15018 * oldColumns__14785)))];
                }
            }
            if ((oldColumns__14785 > this.columns))
            {
                for (var y__15227 = 0L; (y__15227 < this.rows); y__15227 += 1L)
                {
                    for (long x__15271 = this.columns; (x__15271 < oldColumns__14785); x__15271 += 1L)
                    {
                        long xy__15330 = (x__15271 + (y__15227 * oldColumns__14785));
                        if ((oldChildren__14834[(int)(xy__15330)] is not null))
                        {
                            dropChild(oldChildren__14834[(int)(xy__15330)]!);
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
                for (long xy__15838 = (this.columns * DartRuntimePrimitives.RequireValue(__value)); (xy__15838 < checked((long)(this._children.Count))); xy__15838 += 1L)
                {
                    if ((this._children[(int)(xy__15838)] is not null))
                    {
                        dropChild(this._children[(int)(xy__15838)]!);
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
    public virtual List<global::Doroti.Generated.Framework.Painting.Decoration?> rowDecorations
    {
        get => new List<global::Doroti.Generated.Framework.Painting.Decoration?>((this._rowDecorations ?? new List<global::Doroti.Generated.Framework.Painting.Decoration>()));
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
                foreach (global::Doroti.Generated.Framework.Painting.BoxPainter? painter__19329 in this._rowDecorationPainters!)
                {
                    painter__19329?.dispose();
                }
            }
            _rowDecorationPainters = ((this._rowDecorations is not null) ? new List<global::Doroti.Generated.Framework.Painting.BoxPainter?>(System.Linq.Enumerable.Repeat<global::Doroti.Generated.Framework.Painting.BoxPainter?>(null, checked((int)checked((long)(this._rowDecorations!.Count))))) : null);
        }
    }
    public virtual global::Doroti.Generated.Framework.Painting.ImageConfiguration configuration
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

    public override void describeSemanticsConfiguration(global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration config)
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

    public override void assembleSemanticsNode(global::Doroti.Generated.Framework.Semantics.SemanticsNode node, global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration config, IEnumerable<global::Doroti.Generated.Framework.Semantics.SemanticsNode> children)
    {
        var rows__21966 = new List<global::Doroti.Generated.Framework.Semantics.SemanticsNode>();
        var rawCells__22003 = new List<List<List<global::Doroti.Generated.Framework.Semantics.SemanticsNode>>>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)this._rows)), ((rowIndex) => new List<List<global::Doroti.Generated.Framework.Semantics.SemanticsNode>>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)this._columns)), ((columnIndex) => new List<global::Doroti.Generated.Framework.Semantics.SemanticsNode>()))))));
        Rect rectWithOffset(global::Doroti.Generated.Framework.Semantics.SemanticsNode node)
        {
            global::Doroti.Ui.Offset offset__22262 = ((((((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).transform is not null) ? MatrixUtils.getAsTranslation(((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).transform!) : null)) ?? Offset.zero);
            return ((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).rect.shift(offset__22262);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        long findRowIndex(double top)
        {
            for (long i__22637 = (this._rows - 1L); (i__22637 >= 0L); i__22637--)
            {
                if ((this._rowTops[(int)(i__22637)] <= top))
                {
                    return i__22637;
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
            for (long i__22878 = (this._columnLefts!.Count() - 1L); (i__22878 >= 0L); i__22878--)
            {
                if ((this._columnLefts!.elementAt(i__22878) <= left))
                {
                    return i__22878;
                }
            }
            return -1L;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        void shiftTransform(global::Doroti.Generated.Framework.Semantics.SemanticsNode node, double dx, double dy)
        {
            Matrix4? previousTransform__23124 = ((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).transform;
            global::Doroti.Ui.Offset offset__23179 = ((((previousTransform__23124 is not null) ? MatrixUtils.getAsTranslation(previousTransform__23124) : null)) ?? Offset.zero);
            var newTransform__23321 = Matrix4.translationValues((offset__23179.dx + dx), (offset__23179.dy + dy), 0);
            node.transform = newTransform__23321;
        }
        foreach (var child__23457 in children)
        {
            if (this._idToIndexMap.ContainsKey(((global::Doroti.Generated.Framework.Semantics.SemanticsNode)child__23457).id))
            {
                _Index__table index__23548 = this._idToIndexMap.GetValueOrDefault(((global::Doroti.Generated.Framework.Semantics.SemanticsNode)child__23457).id)!;
                long y__23600 = ((_Index__table)index__23548).y;
                long x__23631 = ((_Index__table)index__23548).x;
                if (((y__23600 < this._rows) && (x__23631 < this._columns)))
                {
                    rawCells__22003[(int)(y__23600)][(int)(x__23631)].Add(child__23457);
                }
            }
            else
            {
                global::Doroti.Ui.Rect rect__23766 = rectWithOffset(child__23457);
                long y__23814 = findRowIndex(rect__23766.top);
                long x__23860 = findColumnIndex(rect__23766.left);
                if (((y__23814 != -1L) && (x__23860 != -1L)))
                {
                    rawCells__22003[(int)(y__23814)][(int)(x__23860)].Add(child__23457);
                }
            }
        }
        for (var y__24001 = 0L; (y__24001 < this._rows); y__24001++)
        {
            global::Doroti.Ui.Rect rowBox__24043 = getRowBox(y__24001);
            if ((rowBox__24043.height == 0L))
            {
                continue;
            }
            global::Doroti.Generated.Framework.Semantics.SemanticsNode newRow__24183 = (this._cachedRows.GetValueOrDefault(y__24001) ?? (this._cachedRows[y__24001] = new global::Doroti.Generated.Framework.Semantics.SemanticsNode(showOnScreen: (() =>
            {
                showOnScreen(descendant: this, rect: rowBox__24043);
            }))));
            var cells__24436 = new List<global::Doroti.Generated.Framework.Semantics.SemanticsNode>();
            for (var x__24479 = 0L; (x__24479 < this.columns); x__24479++)
            {
                List<global::Doroti.Generated.Framework.Semantics.SemanticsNode> rawChildrens__24540 = rawCells__22003[(int)(y__24001)][(int)(x__24479)];
                if ((checked((long)(rawChildrens__24540.Count)) == 0))
                {
                    continue;
                }
                bool addCellWrapper__24990 = ((checked((long)(rawChildrens__24540.Count)) > 1L) || (((!object.Equals(rawChildrens__24540.Single().role, SemanticsRole.cell)) && (!object.Equals(rawChildrens__24540.Single().role, SemanticsRole.columnHeader)))));
                global::Doroti.Generated.Framework.Semantics.SemanticsNode cell__25216 = default!;
                if (!addCellWrapper__24990)
                {
                    cell__25216 = rawChildrens__24540.Single();
                }
                else
                {
                    var index__25324 = new _Index__table(y__24001, x__24479);
                    cell__25216 = ((Func<global::Doroti.Generated.Framework.Semantics.SemanticsNode>)(() =>
{
    var __cascade = this._cachedCells.putIfAbsent(index__25324, (() => new global::Doroti.Generated.Framework.Semantics.SemanticsNode()));
    __cascade.updateWith(config: ((Func<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration>)(() =>
{
    var __cascade = new global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration();
    __cascade.role = SemanticsRole.cell;
    return __cascade;
}))(), childrenInInversePaintOrder: rawChildrens__24540);
    return __cascade;
}))();
                }
                double cellWidth__25623 = ((x__24479 == (this._columns - 1L)) ? (rowBox__24043.width - this._columnLefts!.elementAt(x__24479)) : (this._columnLefts!.elementAt((x__24479 + 1L)) - this._columnLefts!.elementAt(x__24479)));
                if ((cellWidth__25623 <= 0.0))
                {
                    continue;
                }
                if (addCellWrapper__24990)
                {
                    ((Func<global::Doroti.Generated.Framework.Semantics.SemanticsNode>)(() =>
{
    var __cascade = cell__25216;
    __cascade.transform = Matrix4.translationValues(this._columnLefts!.elementAt(x__24479), 0, 0);
    __cascade.rect = global::Doroti.Ui.Rect.fromLTWH(0, 0, cellWidth__25623, rowBox__24043.height);
    return __cascade;
}))();
                }
                foreach (var child__26147 in rawChildrens__24540)
                {
                    this._idToIndexMap[((global::Doroti.Generated.Framework.Semantics.SemanticsNode)child__26147).id] = new _Index__table(y__24001, x__24479);
                    global::Doroti.Ui.Rect localRect__26280 = rectWithOffset(child__26147);
                    double dy__26430 = ((localRect__26280.bottom > (rowBox__24043.height + global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)) ? -this._rowTops.elementAt(y__24001) : 0.0);
                    double dx__26944 = (addCellWrapper__24990 ? ((((localRect__26280.left >= cellWidth__25623)) ? -this._columnLefts!.elementAt(x__24479) : 0.0)) : (((localRect__26280.right <= this._columnLefts!.elementAt(x__24479)) ? this._columnLefts!.elementAt(x__24479) : 0.0)));
                    if (((dx__26944 != 0L) || (dy__26430 != 0L)))
                    {
                        shiftTransform(child__26147, dx__26944, dy__26430);
                    }
                }
                cell__25216.indexInParent = x__24479;
                cells__24436.Add(cell__25216);
            }
            ((Func<global::Doroti.Generated.Framework.Semantics.SemanticsNode>)(() =>
{
    var __cascade = newRow__24183;
    __cascade.updateWith(config: ((Func<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration>)(() =>
{
    var __cascade = new global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration();
    __cascade.indexInParent = y__24001;
    __cascade.role = SemanticsRole.row;
    return __cascade;
}))(), childrenInInversePaintOrder: cells__24436);
    __cascade.transform = Matrix4.translationValues(rowBox__24043.left, rowBox__24043.top, 0);
    __cascade.rect = global::Doroti.Ui.Rect.fromLTWH(0, 0, rowBox__24043.width, rowBox__24043.height);
    return __cascade;
}))();
            rows__21966.Add(newRow__24183);
        }
        node.updateWith(config: config, childrenInInversePaintOrder: rows__21966);
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
            foreach (RenderBox? oldChild__28566 in this._children)
            {
                if ((oldChild__28566 is not null))
                {
                    dropChild(oldChild__28566);
                }
            }
            _rows = 0L;
            this._children.Clear();
            markNeedsLayout();
            return;
        }
        DartRuntimePrimitives.Assert(() => ((checked((long)(cells.Count)) % DartRuntimePrimitives.RequireValue(columns)) == 0L));
        HashSet<RenderBox> lostChildren__29004 = new HashSet<RenderBox>();
        for (var y__29054 = 0L; (y__29054 < this._rows); y__29054 += 1L)
        {
            for (var x__29097 = 0L; (x__29097 < this._columns); x__29097 += 1L)
            {
                long xyOld__29146 = (x__29097 + (y__29054 * this._columns));
                long xyNew__29190 = (x__29097 + (y__29054 * DartRuntimePrimitives.RequireValue(columns)));
                if (((xyOld__29146 < checked((long)(this._children.Count))) && (this._children[(int)(xyOld__29146)] is not null) && ((((x__29097 >= DartRuntimePrimitives.RequireValue(columns)) || (xyNew__29190 >= checked((long)(cells.Count)))) || (!object.Equals(this._children[(int)(xyOld__29146)], cells[(int)(xyNew__29190)]))))))
                {
                    lostChildren__29004.Add(this._children[(int)(xyOld__29146)]!);
                }
            }
        }
        var y__29529 = 0L;
        while (((y__29529 * DartRuntimePrimitives.RequireValue(columns)) < checked((long)(cells.Count))))
        {
            for (var x__29592 = 0L; (x__29592 < DartRuntimePrimitives.RequireValue(columns)); x__29592 += 1L)
            {
                long xyNew__29640 = (x__29592 + (y__29529 * DartRuntimePrimitives.RequireValue(columns)));
                long xyOld__29683 = (x__29592 + (y__29529 * this._columns));
                if (((cells[(int)(xyNew__29640)] is not null) && ((((x__29592 >= this._columns) || (y__29529 >= this._rows)) || (xyOld__29683 >= checked((long)(this._children.Count))) || (!object.Equals(this._children[(int)(xyOld__29683)], cells[(int)(xyNew__29640)]))))))
                {
                    if (!lostChildren__29004.Remove(cells[(int)(xyNew__29640)]))
                    {
                        adoptChild(cells[(int)(xyNew__29640)]!);
                    }
                }
            }
            y__29529 += 1L;
        }
        lostChildren__29004.forEach(dropChild);
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
        foreach (RenderBox? oldChild__30546 in this._children)
        {
            if ((oldChild__30546 is not null))
            {
                dropChild(oldChild__30546);
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
        foreach (var cell__31115 in cells)
        {
            if ((cell__31115 is not null))
            {
                adoptChild(cell__31115);
            }
        }
        markNeedsLayout();
    }

    public virtual void setChild(long x, long y, RenderBox? value)
    {
        DartRuntimePrimitives.Assert(() => ((((x >= 0L) && (x < this.columns)) && (y >= 0L)) && (y < this.rows)));
        DartRuntimePrimitives.Assert(() => (checked((long)(this._children.Count)) == (this.rows * this.columns)));
        long xy__31649 = (x + (y * this.columns));
        RenderBox? oldChild__31692 = this._children[(int)(xy__31649)];
        if ((object.Equals(oldChild__31692, value)))
        {
            return;
        }
        if ((oldChild__31692 is not null))
        {
            dropChild(oldChild__31692);
        }
        this._children[(int)(xy__31649)] = value;
        if ((value is not null))
        {
            adoptChild(value);
        }
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        foreach (RenderBox? child__32016 in this._children)
        {
            child__32016?.attach(owner);
        }
    }

    public override void detach()
    {
        base.detach();
        if ((this._rowDecorationPainters is not null))
        {
            foreach (global::Doroti.Generated.Framework.Painting.BoxPainter? painter__32198 in this._rowDecorationPainters!)
            {
                painter__32198?.dispose();
            }
            _rowDecorationPainters = new List<global::Doroti.Generated.Framework.Painting.BoxPainter?>(System.Linq.Enumerable.Repeat<global::Doroti.Generated.Framework.Painting.BoxPainter?>(null, checked((int)checked((long)(this._rowDecorations!.Count)))));
        }
        foreach (RenderBox? child__32392 in this._children)
        {
            child__32392?.detach();
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        DartRuntimePrimitives.Assert(() => (checked((long)(this._children.Count)) == (this.rows * this.columns)));
        foreach (RenderBox? child__32586 in this._children)
        {
            if ((child__32586 is not null))
            {
                visitor(child__32586);
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
        var totalMinWidth__32942 = 0.0;
        for (var x__32976 = 0L; (x__32976 < this.columns); x__32976 += 1L)
        {
            TableColumnWidth columnWidth__33035 = (this._columnWidths.GetValueOrDefault(x__32976) ?? this.defaultColumnWidth);
            IEnumerable<RenderBox> columnCells__33121 = column(x__32976);
            totalMinWidth__32942 += columnWidth__33035.minIntrinsicWidth(columnCells__33121, double.PositiveInfinity);
        }
        return totalMinWidth__32942;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        DartRuntimePrimitives.Assert(() => (checked((long)(this._children.Count)) == (this.rows * this.columns)));
        if (((this.rows * this.columns) == 0L))
        {
            return 0.0;
        }
        var totalMaxWidth__33441 = 0.0;
        for (var x__33475 = 0L; (x__33475 < this.columns); x__33475 += 1L)
        {
            TableColumnWidth columnWidth__33534 = (this._columnWidths.GetValueOrDefault(x__33475) ?? this.defaultColumnWidth);
            IEnumerable<RenderBox> columnCells__33620 = column(x__33475);
            totalMaxWidth__33441 += columnWidth__33534.maxIntrinsicWidth(columnCells__33620, double.PositiveInfinity);
        }
        return totalMaxWidth__33441;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        DartRuntimePrimitives.Assert(() => (checked((long)(this._children.Count)) == (this.rows * this.columns)));
        if (((this.rows * this.columns) == 0L))
        {
            return 0.0;
        }
        List<double> widths__34122 = _computeColumnWidths(BoxConstraints.CreateTightForFinite(width: width));
        var rowTop__34206 = 0.0;
        for (var y__34233 = 0L; (y__34233 < this.rows); y__34233 += 1L)
        {
            var rowHeight__34270 = 0.0;
            for (var x__34302 = 0L; (x__34302 < this.columns); x__34302 += 1L)
            {
                long xy__34350 = (x__34302 + (y__34233 * this.columns));
                RenderBox? child__34397 = this._children[(int)(xy__34350)];
                if ((child__34397 is not null))
                {
                    rowHeight__34270 = Math.Max(rowHeight__34270, child__34397.getMaxIntrinsicHeight(widths__34122[(int)(x__34302)]));
                }
            }
            rowTop__34206 += rowHeight__34270;
        }
        return rowTop__34206;
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
        for (var y__35364 = 0L; (y__35364 < this.rows); y__35364 += 1L)
        {
            long xy__35407 = (x + (y__35364 * this.columns));
            RenderBox? child__35452 = this._children[(int)(xy__35407)];
            if ((child__35452 is not null))
            {
                yield return child__35452;
            }
        }
    }

    public virtual IEnumerable<RenderBox> row(long y)
    {
        long start__35872 = (y * this.columns);
        long end__35907 = (((y + 1L)) * this.columns);
        for (var xy__35945 = start__35872; (xy__35945 < end__35907); xy__35945 += 1L)
        {
            RenderBox? child__36001 = this._children[(int)(xy__35945)];
            if ((child__36001 is not null))
            {
                yield return child__36001;
            }
        }
    }

    internal virtual List<double> _computeColumnWidths(BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => (checked((long)(this._children.Count)) == (this.rows * this.columns)));
        var widths__36783 = new List<double>(System.Linq.Enumerable.Repeat<double>(0.0, checked((int)this.columns)));
        var minWidths__36837 = new List<double>(System.Linq.Enumerable.Repeat<double>(0.0, checked((int)this.columns)));
        var flexes__36894 = new List<double?>(System.Linq.Enumerable.Repeat<double?>(null, checked((int)this.columns)));
        var tableWidth__36948 = 0.0;
        var unflexedTableWidth__37025 = 0.0;
        var totalFlex__37125 = 0.0;
        for (var x__37155 = 0L; (x__37155 < this.columns); x__37155 += 1L)
        {
            TableColumnWidth columnWidth__37214 = (this._columnWidths.GetValueOrDefault(x__37155) ?? this.defaultColumnWidth);
            IEnumerable<RenderBox> columnCells__37300 = column(x__37155);
            double maxIntrinsicWidth__37391 = columnWidth__37214.maxIntrinsicWidth(columnCells__37300, ((BoxConstraints)constraints).maxWidth);
            DartRuntimePrimitives.Assert(() => double.IsFinite(maxIntrinsicWidth__37391));
            DartRuntimePrimitives.Assert(() => (maxIntrinsicWidth__37391 >= 0.0));
            widths__36783[(int)(x__37155)] = maxIntrinsicWidth__37391;
            tableWidth__36948 += maxIntrinsicWidth__37391;
            double minIntrinsicWidth__37736 = columnWidth__37214.minIntrinsicWidth(columnCells__37300, ((BoxConstraints)constraints).maxWidth);
            DartRuntimePrimitives.Assert(() => double.IsFinite(minIntrinsicWidth__37736));
            DartRuntimePrimitives.Assert(() => (minIntrinsicWidth__37736 >= 0.0));
            minWidths__36837[(int)(x__37155)] = minIntrinsicWidth__37736;
            DartRuntimePrimitives.Assert(() => (maxIntrinsicWidth__37391 >= minIntrinsicWidth__37736));
            double? flex__38095 = columnWidth__37214.flex(columnCells__37300);
            if ((flex__38095 is not null))
            {
                double flex__38095__value38143 = DartRuntimePrimitives.RequireValue(flex__38095);
                DartRuntimePrimitives.Assert(() => double.IsFinite(DartRuntimePrimitives.RequireValue(flex__38095__value38143)));
                DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.RequireValue(flex__38095__value38143) > 0.0));
                flexes__36894[(int)(x__37155)] = DartRuntimePrimitives.RequireValue(flex__38095__value38143);
                totalFlex__37125 += DartRuntimePrimitives.RequireValue(flex__38095__value38143);
            }
            else
            {
                unflexedTableWidth__37025 = (unflexedTableWidth__37025 + maxIntrinsicWidth__37391);
            }
        }
        double maxWidthConstraint__38386 = ((BoxConstraints)constraints).maxWidth;
        double minWidthConstraint__38446 = ((BoxConstraints)constraints).minWidth;
        if ((totalFlex__37125 > 0.0))
        {
            double targetWidth__38764 = default!;
            if (double.IsFinite(maxWidthConstraint__38386))
            {
                targetWidth__38764 = maxWidthConstraint__38386;
            }
            else
            {
                targetWidth__38764 = minWidthConstraint__38446;
            }
            if ((tableWidth__36948 < targetWidth__38764))
            {
                double remainingWidth__38984 = (targetWidth__38764 - unflexedTableWidth__37025);
                DartRuntimePrimitives.Assert(() => double.IsFinite(remainingWidth__38984));
                DartRuntimePrimitives.Assert(() => (remainingWidth__38984 >= 0.0));
                for (var x__39132 = 0L; (x__39132 < this.columns); x__39132 += 1L)
                {
                    if ((flexes__36894[(int)(x__39132)] is not null))
                    {
                        double flexedWidth__39222 = ((remainingWidth__38984 * DartRuntimePrimitives.RequireValue(flexes__36894[(int)(x__39132)])) / totalFlex__37125);
                        DartRuntimePrimitives.Assert(() => double.IsFinite(flexedWidth__39222));
                        DartRuntimePrimitives.Assert(() => (flexedWidth__39222 >= 0.0));
                        if ((widths__36783[(int)(x__39132)] < flexedWidth__39222))
                        {
                            double delta__39429 = (flexedWidth__39222 - widths__36783[(int)(x__39132)]);
                            tableWidth__36948 += delta__39429;
                            widths__36783[(int)(x__39132)] = flexedWidth__39222;
                        }
                    }
                }
                DartRuntimePrimitives.Assert(() => ((tableWidth__36948 + global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance) >= targetWidth__38764));
            }
        }
        else
        {
            if ((tableWidth__36948 < minWidthConstraint__38446))
            {
                double delta__39849 = (((minWidthConstraint__38446 - tableWidth__36948)) / this.columns);
                for (var x__39917 = 0L; (x__39917 < this.columns); x__39917 += 1L)
                {
                    widths__36783[(int)(x__39917)] = (widths__36783[(int)(x__39917)] + delta__39849);
                }
                tableWidth__36948 = minWidthConstraint__38446;
            }
        }
        if ((tableWidth__36948 > maxWidthConstraint__38386))
        {
            double deficit__40291 = (tableWidth__36948 - maxWidthConstraint__38386);
            long availableColumns__41363 = this.columns;
            while (((deficit__40291 > global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance) && (totalFlex__37125 > global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)))
            {
                var newTotalFlex__41492 = 0.0;
                for (var x__41529 = 0L; (x__41529 < this.columns); x__41529 += 1L)
                {
                    if ((flexes__36894[(int)(x__41529)] is not null))
                    {
                        double newWidth__41619 = (widths__36783[(int)(x__41529)] - ((deficit__40291 * DartRuntimePrimitives.RequireValue(flexes__36894[(int)(x__41529)])) / totalFlex__37125));
                        DartRuntimePrimitives.Assert(() => double.IsFinite(newWidth__41619));
                        if ((newWidth__41619 <= minWidths__36837[(int)(x__41529)]))
                        {
                            deficit__40291 -= (widths__36783[(int)(x__41529)] - minWidths__36837[(int)(x__41529)]);
                            widths__36783[(int)(x__41529)] = minWidths__36837[(int)(x__41529)];
                            flexes__36894[(int)(x__41529)] = null;
                            availableColumns__41363 -= 1L;
                        }
                        else
                        {
                            deficit__40291 -= (widths__36783[(int)(x__41529)] - newWidth__41619);
                            widths__36783[(int)(x__41529)] = newWidth__41619;
                            newTotalFlex__41492 += DartRuntimePrimitives.RequireValue(flexes__36894[(int)(x__41529)]);
                        }
                        DartRuntimePrimitives.Assert(() => (widths__36783[(int)(x__41529)] >= 0.0));
                    }
                }
                totalFlex__37125 = newTotalFlex__41492;
            }
            while (((deficit__40291 > global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance) && (availableColumns__41363 > 0L)))
            {
                double delta__42615 = (deficit__40291 / availableColumns__41363);
                DartRuntimePrimitives.Assert(() => (delta__42615 != 0L));
                var newAvailableColumns__42691 = 0L;
                for (var x__42733 = 0L; (x__42733 < this.columns); x__42733 += 1L)
                {
                    double availableDelta__42786 = (widths__36783[(int)(x__42733)] - minWidths__36837[(int)(x__42733)]);
                    if ((availableDelta__42786 > 0.0))
                    {
                        if ((availableDelta__42786 <= delta__42615))
                        {
                            deficit__40291 -= (widths__36783[(int)(x__42733)] - minWidths__36837[(int)(x__42733)]);
                            widths__36783[(int)(x__42733)] = minWidths__36837[(int)(x__42733)];
                        }
                        else
                        {
                            deficit__40291 -= delta__42615;
                            widths__36783[(int)(x__42733)] = (widths__36783[(int)(x__42733)] - delta__42615);
                            newAvailableColumns__42691 += 1L;
                        }
                    }
                }
                availableColumns__41363 = newAvailableColumns__42691;
            }
        }
        return widths__36783;
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
        List<double> widths__44088 = _computeColumnWidths(constraints);
        double? baselineOffset__44144 = default!;
        for (var col__44173 = 0L; (col__44173 < this.columns); col__44173 += 1L)
        {
            RenderBox? child__44232 = this._children[(int)(col__44173)];
            var childConstraints__44268 = BoxConstraints.CreateTightFor(width: widths__44088[(int)(col__44173)]);
            if ((child__44232 is null))
            {
                continue;
            }
            var childParentData__44397 = ((TableCellParentData?)(object?)child__44232.parentData!)!;
            double? childBaseline__44477 = ((((TableCellParentData)childParentData__44397).verticalAlignment ?? this.defaultVerticalAlignment) switch { TableCellVerticalAlignment.baseline => child__44232.getDryBaseline(childConstraints__44268, baseline), TableCellVerticalAlignment.baseline or TableCellVerticalAlignment.top or TableCellVerticalAlignment.middle or TableCellVerticalAlignment.bottom or TableCellVerticalAlignment.fill => null, TableCellVerticalAlignment.intrinsicHeight => null, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            if (((childBaseline__44477 is not null) && (((baselineOffset__44144 is null) || (DartRuntimePrimitives.RequireValue(baselineOffset__44144) < DartRuntimePrimitives.RequireValue(childBaseline__44477))))))
            {
                double childBaseline__44477__value44974 = DartRuntimePrimitives.RequireValue(childBaseline__44477);
                baselineOffset__44144 = DartRuntimePrimitives.RequireValue(childBaseline__44477__value44974);
            }
        }
        return baselineOffset__44144;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        if (((this.rows * this.columns) == 0L))
        {
            return constraints.constrain(Size.zero);
        }
        List<double> widths__45343 = _computeColumnWidths(constraints);
        double tableWidth__45404 = System.Linq.Enumerable.Aggregate(widths__45343, (double)0.0, ((a, b) => (a + b)));
        var rowTop__45474 = 0.0;
        for (var y__45501 = 0L; (y__45501 < this.rows); y__45501 += 1L)
        {
            var rowHeight__45538 = 0.0;
            for (var x__45570 = 0L; (x__45570 < this.columns); x__45570 += 1L)
            {
                long xy__45618 = (x__45570 + (y__45501 * this.columns));
                RenderBox? child__45665 = this._children[(int)(xy__45618)];
                if ((child__45665 is not null))
                {
                    var childParentData__45733 = ((TableCellParentData?)(object?)child__45665.parentData!)!;
                    switch ((((TableCellParentData)childParentData__45733).verticalAlignment ?? this.defaultVerticalAlignment))
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
                                global::Doroti.Ui.Size childSize__46452 = child__45665.getDryLayout(BoxConstraints.CreateTightFor(width: widths__45343[(int)(x__45570)]));
                                rowHeight__45538 = Math.Max(rowHeight__45538, childSize__46452.height);
                                break;
                            }
                        case TableCellVerticalAlignment.fill:
                            {
                                break;
                            }
                    }
                }
            }
            rowTop__45474 += rowHeight__45538;
        }
        return constraints.constrain(new global::Doroti.Ui.Size(tableWidth__45404, rowTop__45474));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraints__46853 = this.constraints;
        long rows__46899 = this.rows;
        long columns__46931 = this.columns;
        DartRuntimePrimitives.Assert(() => (checked((long)(this._children.Count)) == (DartRuntimePrimitives.RequireValue(rows__46899) * DartRuntimePrimitives.RequireValue(columns__46931))));
        if (((DartRuntimePrimitives.RequireValue(rows__46899) * DartRuntimePrimitives.RequireValue(columns__46931)) == 0L))
        {
            _tableWidth = 0.0;
            size = constraints__46853.constrain(Size.zero);
            return;
        }
        List<double> widths__47317 = _computeColumnWidths(constraints__46853);
        var positions__47371 = new List<double>(System.Linq.Enumerable.Repeat<double>(0.0, checked((int)DartRuntimePrimitives.RequireValue(columns__46931))));
        switch (this.textDirection)
        {
            case TextDirection.rtl:
                {
                    positions__47371[(int)((columns__46931 - 1L))] = 0.0;
                    for (long x__47532 = (DartRuntimePrimitives.RequireValue(columns__46931) - 2L); (x__47532 >= 0L); x__47532 -= 1L)
                    {
                        positions__47371[(int)(x__47532)] = (positions__47371[(int)((x__47532 + 1L))] + widths__47317[(int)((x__47532 + 1L))]);
                    }
                    _columnLefts = System.Linq.Enumerable.Reverse(positions__47371);
                    _tableWidth = (positions__47371.First() + widths__47317.First());
                    break;
                }
            case TextDirection.ltr:
                {
                    positions__47371[(int)(0L)] = 0.0;
                    for (var x__47808 = 1L; (x__47808 < DartRuntimePrimitives.RequireValue(columns__46931)); x__47808 += 1L)
                    {
                        positions__47371[(int)(x__47808)] = (positions__47371[(int)((x__47808 - 1L))] + widths__47317[(int)((x__47808 - 1L))]);
                    }
                    _columnLefts = positions__47371;
                    _tableWidth = (positions__47371.Last() + widths__47317.Last());
                    break;
                }
        }
        this._rowTops.Clear();
        _baselineDistance = null;
        var rowTop__48089 = 0.0;
        for (var y__48116 = 0L; (y__48116 < DartRuntimePrimitives.RequireValue(rows__46899)); y__48116 += 1L)
        {
            this._rowTops.Add(rowTop__48089);
            var rowHeight__48181 = 0.0;
            var haveBaseline__48208 = false;
            var beforeBaselineDistance__48240 = 0.0;
            var afterBaselineDistance__48280 = 0.0;
            var baselines__48321 = new List<double>(System.Linq.Enumerable.Repeat<double>(0.0, checked((int)DartRuntimePrimitives.RequireValue(columns__46931))));
            for (var x__48383 = 0L; (x__48383 < DartRuntimePrimitives.RequireValue(columns__46931)); x__48383 += 1L)
            {
                long xy__48431 = (x__48383 + (y__48116 * DartRuntimePrimitives.RequireValue(columns__46931)));
                RenderBox? child__48478 = this._children[(int)(xy__48431)];
                if ((child__48478 is not null))
                {
                    var childParentData__48546 = ((TableCellParentData?)(object?)child__48478.parentData!)!;
                    childParentData__48546.x = x__48383;
                    childParentData__48546.y = y__48116;
                    switch ((((TableCellParentData)childParentData__48546).verticalAlignment ?? this.defaultVerticalAlignment))
                    {
                        case TableCellVerticalAlignment.baseline:
                            {
                                DartRuntimePrimitives.Assert(() => (this.textBaseline is not null));
                                child__48478.layout(BoxConstraints.CreateTightFor(width: widths__47317[(int)(x__48383)]), parentUsesSize: true);
                                double? childBaseline__49094 = child__48478.getDistanceToBaseline(DartRuntimePrimitives.RequireValue(this.textBaseline), onlyReal: true);
                                if ((childBaseline__49094 is not null))
                                {
                                    double childBaseline__49094__value49237 = DartRuntimePrimitives.RequireValue(childBaseline__49094);
                                    beforeBaselineDistance__48240 = Math.Max(beforeBaselineDistance__48240, DartRuntimePrimitives.RequireValue(childBaseline__49094__value49237));
                                    afterBaselineDistance__48280 = Math.Max(afterBaselineDistance__48280, (((RenderBox)child__48478).size.height - DartRuntimePrimitives.RequireValue(childBaseline__49094__value49237)));
                                    baselines__48321[(int)(x__48383)] = DartRuntimePrimitives.RequireValue(childBaseline__49094__value49237);
                                    haveBaseline__48208 = true;
                                }
                                else
                                {
                                    rowHeight__48181 = Math.Max(rowHeight__48181, ((RenderBox)child__48478).size.height);
                                    childParentData__48546.offset = new global::Doroti.Ui.Offset(positions__47371[(int)(x__48383)], rowTop__48089);
                                }
                                break;
                            }
                        case TableCellVerticalAlignment.top:
                        case TableCellVerticalAlignment.middle:
                        case TableCellVerticalAlignment.bottom:
                        case TableCellVerticalAlignment.intrinsicHeight:
                            {
                                child__48478.layout(BoxConstraints.CreateTightFor(width: widths__47317[(int)(x__48383)]), parentUsesSize: true);
                                rowHeight__48181 = Math.Max(rowHeight__48181, ((RenderBox)child__48478).size.height);
                                break;
                            }
                        case TableCellVerticalAlignment.fill:
                            {
                                break;
                            }
                    }
                }
            }
            if (haveBaseline__48208)
            {
                if ((y__48116 == 0L))
                {
                    _baselineDistance = beforeBaselineDistance__48240;
                }
                rowHeight__48181 = Math.Max(rowHeight__48181, (beforeBaselineDistance__48240 + afterBaselineDistance__48280));
            }
            for (var x__50474 = 0L; (x__50474 < DartRuntimePrimitives.RequireValue(columns__46931)); x__50474 += 1L)
            {
                long xy__50522 = (x__50474 + (y__48116 * DartRuntimePrimitives.RequireValue(columns__46931)));
                RenderBox? child__50569 = this._children[(int)(xy__50522)];
                if ((child__50569 is not null))
                {
                    var childParentData__50637 = ((TableCellParentData?)(object?)child__50569.parentData!)!;
                    switch ((((TableCellParentData)childParentData__50637).verticalAlignment ?? this.defaultVerticalAlignment))
                    {
                        case TableCellVerticalAlignment.baseline:
                            {
                                childParentData__50637.offset = new global::Doroti.Ui.Offset(positions__47371[(int)(x__50474)], ((rowTop__48089 + beforeBaselineDistance__48240) - baselines__48321[(int)(x__50474)]));
                                break;
                            }
                        case TableCellVerticalAlignment.top:
                            {
                                childParentData__50637.offset = new global::Doroti.Ui.Offset(positions__47371[(int)(x__50474)], rowTop__48089);
                                break;
                            }
                        case TableCellVerticalAlignment.middle:
                            {
                                childParentData__50637.offset = new global::Doroti.Ui.Offset(positions__47371[(int)(x__50474)], (rowTop__48089 + (((rowHeight__48181 - ((RenderBox)child__50569).size.height)) / 2.0)));
                                break;
                            }
                        case TableCellVerticalAlignment.bottom:
                            {
                                childParentData__50637.offset = new global::Doroti.Ui.Offset(positions__47371[(int)(x__50474)], ((rowTop__48089 + rowHeight__48181) - ((RenderBox)child__50569).size.height));
                                break;
                            }
                        case TableCellVerticalAlignment.fill:
                        case TableCellVerticalAlignment.intrinsicHeight:
                            {
                                child__50569.layout(BoxConstraints.CreateTightFor(width: widths__47317[(int)(x__50474)], height: rowHeight__48181));
                                childParentData__50637.offset = new global::Doroti.Ui.Offset(positions__47371[(int)(x__50474)], rowTop__48089);
                                break;
                            }
                    }
                }
            }
            rowTop__48089 += rowHeight__48181;
        }
        this._rowTops.Add(rowTop__48089);
        size = constraints__46853.constrain(new global::Doroti.Ui.Size(this._tableWidth, rowTop__48089));
        DartRuntimePrimitives.Assert(() => (checked((long)(this._rowTops.Count)) == (DartRuntimePrimitives.RequireValue(rows__46899) + 1L)));
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        DartRuntimePrimitives.Assert(() => (checked((long)(this._children.Count)) == (this.rows * this.columns)));
        for (long index__52090 = (checked((long)(this._children.Count)) - 1L); (index__52090 >= 0L); index__52090 -= 1L)
        {
            RenderBox? child__52169 = this._children[(int)(index__52090)];
            if ((child__52169 is not null))
            {
                var childParentData__52236 = ((BoxParentData?)(object?)child__52169.parentData!)!;
                bool isHit__52309 = result.addWithPaintOffset(offset: ((BoxParentData)childParentData__52236).offset, position: position, hitTest: ((Func<BoxHitTestResult, Offset, bool>)((result, transformed) =>
                {
                    DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - ((BoxParentData)childParentData__52236).offset))));
                    return child__52169.hitTest(result, position: transformed);
                    return default;
                })));
                if (isHit__52309)
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
                var borderRect__52921 = global::Doroti.Ui.Rect.fromLTWH(offset.dx, offset.dy, this._tableWidth, 0.0);
                this.border!.paint(((PaintingContext)context).canvas, borderRect__52921, rows: new List<double>(), columns: new List<double>());
            }
            return;
        }
        DartRuntimePrimitives.Assert(() => (checked((long)(this._rowTops.Count)) == (this.rows + 1L)));
        if ((this._rowDecorations is not null))
        {
            DartRuntimePrimitives.Assert(() => (checked((long)(this._rowDecorations!.Count)) == checked((long)(this._rowDecorationPainters!.Count))));
            global::Doroti.Ui.Canvas canvas__53338 = ((PaintingContext)context).canvas;
            for (var y__53378 = 0L; (y__53378 < this.rows); y__53378 += 1L)
            {
                if ((checked((long)(this._rowDecorations!.Count)) <= y__53378))
                {
                    break;
                }
                if ((this._rowDecorations![(int)(y__53378)] is not null))
                {
                    this._rowDecorationPainters![(int)(y__53378)] ??= this._rowDecorations![(int)(y__53378)]!.createBoxPainter((Action)markNeedsPaint);
                    this._rowDecorationPainters![(int)(y__53378)]!.paint(canvas__53338, new global::Doroti.Ui.Offset(offset.dx, (offset.dy + this._rowTops[(int)(y__53378)])), this.configuration.copyWith(size: new global::Doroti.Ui.Size(size.width, (this._rowTops[(int)((y__53378 + 1L))] - this._rowTops[(int)(y__53378)]))));
                }
            }
        }
        for (var index__53877 = 0L; (index__53877 < checked((long)(this._children.Count))); index__53877 += 1L)
        {
            RenderBox? child__53951 = this._children[(int)(index__53877)];
            if ((child__53951 is not null))
            {
                var childParentData__54018 = ((BoxParentData?)(object?)child__53951.parentData!)!;
                context.paintChild(child__53951, (((BoxParentData)childParentData__54018).offset + offset));
            }
        }
        DartRuntimePrimitives.Assert(() => (this._rows == (checked((long)(this._rowTops.Count)) - 1L)));
        DartRuntimePrimitives.Assert(() => (this._columns == this._columnLefts!.Count()));
        if ((this.border is not null))
        {
            var borderRect__54511 = global::Doroti.Ui.Rect.fromLTWH(offset.dx, offset.dy, this._tableWidth, this._rowTops.Last());
            IEnumerable<double> rows__54618 = this._rowTops.GetRange(1L, (checked((long)(this._rowTops.Count)) - 1L));
            IEnumerable<double> columns__54697 = this._columnLefts!.skip(1L);
            this.border!.paint(((PaintingContext)context).canvas, borderRect__54511, rows: rows__54618, columns: columns__54697);
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<TableBorder>("border", this.border, defaultValue: null));
        properties.add(new DiagnosticsProperty<DartMap<long, TableColumnWidth>>("specified column widths", this._columnWidths, level: ((checked((long)(this._columnWidths.Count)) == 0) ? DiagnosticLevel.hidden : DiagnosticLevel.info)));
        properties.add(new DiagnosticsProperty<TableColumnWidth>("default column width", this.defaultColumnWidth));
        properties.add(new MessageProperty("table size", $"{this.columns}×{this.rows}"));
        properties.add(new IterableProperty<string>("column offsets", this._columnLefts?.map<double, string>((value => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(value))), ifNull: "unknown"));
        properties.add(new IterableProperty<string>("row offsets", this._rowTops.map<double, string>((value => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(value))), ifNull: "unknown"));
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
