// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/table.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class TableRow
{
    public virtual global::Doroti.Framework.Foundation.LocalKey? key { get; private set; }
    public virtual global::Doroti.Framework.Painting.Decoration? decoration { get; private set; }
    public virtual List<Widget> children { get; private set; } = default!;

    public TableRow(global::Doroti.Framework.Foundation.LocalKey? key = null, global::Doroti.Framework.Painting.Decoration? decoration = null, List<Widget> children = default!)
    {
        List<Widget> __children = children ?? new List<Widget>();
        this.key = key;
        this.decoration = decoration;
        this.children = __children;
    }

    public override string ToString()
    {
        var result = new StringBuffer();
        result.write("TableRow(");
        if ((this.key is not null))
        {
            result.write($"{this.key}, ");
        }
        if ((this.decoration is not null))
        {
            result.write($"{this.decoration}, ");
        }
        if (!Enumerable.Any(this.children))
        {
            result.write("no children");
        }
        else
        {
            result.write($"{this.children}");
        }
        result.write(")");
        return ((string)result.ToString());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TableElementRow__table
{
    public virtual global::Doroti.Framework.Foundation.LocalKey? key { get; private set; }
    public virtual List<Element> children { get; private set; } = default!;

    internal _TableElementRow__table(global::Doroti.Framework.Foundation.LocalKey? key = null, List<Element> children = default!)
    {
        this.key = key;
        this.children = children;
    }

}

public class Table : RenderObjectWidget
{
    public virtual List<TableRow> children { get; private set; } = default!;
    public virtual DartMap<long, global::Doroti.Framework.Rendering.TableColumnWidth>? columnWidths { get; private set; }
    public virtual global::Doroti.Framework.Rendering.TableColumnWidth defaultColumnWidth { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }
    public virtual global::Doroti.Framework.Rendering.TableBorder? border { get; private set; }
    public virtual global::Doroti.Framework.Rendering.TableCellVerticalAlignment defaultVerticalAlignment { get; private set; } = default!;
    public virtual TextBaseline? textBaseline { get; private set; }
    internal virtual List<global::Doroti.Framework.Painting.Decoration?>? _rowDecorations { get; private set; }

    public Table(global::Doroti.Framework.Foundation.Key? key = null, List<TableRow> children = default!, DartMap<long, global::Doroti.Framework.Rendering.TableColumnWidth>? columnWidths = null, global::Doroti.Framework.Rendering.TableColumnWidth defaultColumnWidth = default!, TextDirection? textDirection = null, global::Doroti.Framework.Rendering.TableBorder? border = null, global::Doroti.Framework.Rendering.TableCellVerticalAlignment defaultVerticalAlignment = TableCellVerticalAlignment.top, TextBaseline? textBaseline = null) : base(key: key)
    {
        List<TableRow> __children = children ?? new List<TableRow>();
        global::Doroti.Framework.Rendering.TableColumnWidth __defaultColumnWidth = defaultColumnWidth ?? new global::Doroti.Framework.Rendering.FlexColumnWidth();
        this.children = __children;
        this.columnWidths = columnWidths;
        this.defaultColumnWidth = __defaultColumnWidth;
        this.textDirection = textDirection;
        this.border = border;
        this.defaultVerticalAlignment = defaultVerticalAlignment;
        this.textBaseline = textBaseline;
        this._rowDecorations = (this.children.any(((row) => (((TableRow)row).decoration is not null))) ? this.children.map<TableRow, global::Doroti.Framework.Painting.Decoration?>(((row) => ((TableRow)row).decoration)).ToList() : null);
        System.Diagnostics.Debug.Assert(((!Equals(defaultVerticalAlignment, TableCellVerticalAlignment.baseline)) || (textBaseline is not null)));
        System.Diagnostics.Debug.Assert(((global::System.Func<bool>)(() =>
        {
            if (__children.any(((row1) => ((((TableRow)row1).key is not null) && __children.any(((row2) => ((!Equals(row1, row2)) && (Equals(((TableRow)row1).key, ((TableRow)row2).key)))))))))
            {
                throw DartRuntimePrimitives.AsException(FlutterError.Create("Two or more TableRow children of this Table had the same key.\n" + "All the keyed TableRow children of a Table must have different Keys."));
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))());
        System.Diagnostics.Debug.Assert(((global::System.Func<bool>)(() =>
        {
            if (Enumerable.Any(__children))
            {
                long cellCount = checked((long)(__children.First().children.Count));
                if (__children.any(((row) => (checked((long)(((TableRow)row).children.Count)) != cellCount))))
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create("Table contains irregular row lengths.\n" + "Every TableRow in a Table must have the same number of children, so that every cell is filled. " + "Otherwise, the table will contain holes."));
                }
                if (__children.any(((row) => !Enumerable.Any(((TableRow)row).children))))
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create("One or more TableRow have no children.\n" + "Every TableRow in a Table must have at least one child, so there is no empty row. "));
                }
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))());
    }

    public override RenderObjectElement createElement() => DartRuntimePrimitives.ConvertValue<RenderObjectElement>(new _TableElement__table(this));
    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasDirectionality(context));
        return ((global::Doroti.Framework.Rendering.RenderObject)new global::Doroti.Framework.Rendering.RenderTable(columns: (Enumerable.Any(this.children) ? checked((long)(this.children[(int)(0L)].children.Count)) : 0L), rows: checked((long)(this.children.Count)), columnWidths: this.columnWidths, defaultColumnWidth: this.defaultColumnWidth, textDirection: ((this.textDirection ?? (TextDirection)Directionality.of(context))), border: this.border, rowDecorations: this._rowDecorations, configuration: ImageLibrary.createLocalImageConfiguration(context), defaultVerticalAlignment: this.defaultVerticalAlignment, textBaseline: this.textBaseline));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderTable)renderObject;
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasDirectionality(context));
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Rendering.RenderTable)__renderObject).columns == ((Enumerable.Any(this.children) ? checked((long)(this.children[(int)(0L)].children.Count)) : 0L))));
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Rendering.RenderTable)__renderObject).rows == checked((long)(this.children.Count))));
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.RenderTable>)(() =>
{
    var __cascade = __renderObject;
    __cascade.columnWidths = this.columnWidths;
    __cascade.defaultColumnWidth = this.defaultColumnWidth;
    __cascade.textDirection = ((this.textDirection ?? (TextDirection)Directionality.of(context)));
    __cascade.border = this.border;
    __cascade.rowDecorations = this._rowDecorations;
    __cascade.configuration = ImageLibrary.createLocalImageConfiguration(context);
    __cascade.defaultVerticalAlignment = this.defaultVerticalAlignment;
    __cascade.textBaseline = this.textBaseline;
    return __cascade;
}))());
    }

}

internal class _TableElement__table : RenderObjectElement
{
    internal virtual List<_TableElementRow__table> _children { get; set; } = new List<_TableElementRow__table>();
    internal virtual bool _doingMountOrUpdate { get; set; } = false;
    internal virtual HashSet<Element> _forgottenChildren { get; private set; } = new HashSet<Element>();

    internal _TableElement__table(Table widget) : base(widget)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderTable renderObject => (global::Doroti.Framework.Rendering.RenderTable)base.renderObject;
    public override void mount(Element? parent, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => !this._doingMountOrUpdate);
        _doingMountOrUpdate = true;
        base.mount(parent, newSlot);
        var rowIndex = -1L;
        _children = (((Table?)this.widget)!).children.map<TableRow, _TableElementRow__table>(((row) =>
        {
            var columnIndex = 0L;
            rowIndex += 1L;
            return new _TableElementRow__table(key: ((TableRow)row).key, children: ((TableRow)row).children.map<Widget, Element>(((child) =>
            {
                return ((Element)inflateWidget(child, new _TableSlot__table(columnIndex++, rowIndex)));
                throw new InvalidOperationException("Dart closure completed without a value.");
            })).ToList());
            throw new InvalidOperationException("Dart closure completed without a value.");
        })).ToList();
        _updateRenderObjectChildren();
        DartRuntimePrimitives.Assert(() => this._doingMountOrUpdate);
        _doingMountOrUpdate = false;
    }

    public override void insertRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)child;
        var __slot = slot as _TableSlot__table ?? throw new ArgumentException("A table cell requires a table slot.", nameof(slot));
        this.renderObject.setupParentData(__child);
        if (!this._doingMountOrUpdate)
        {
            this.renderObject.setChild(__slot.column, ((_TableSlot__table)__slot).row, __child);
        }
    }

    public override void moveRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)child;
        var __oldSlot = (_TableSlot__table?)oldSlot;
        var __newSlot = (_TableSlot__table?)newSlot;
        DartRuntimePrimitives.Assert(() => this._doingMountOrUpdate);
    }

    public override void removeRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)child;
        var __slot = slot as _TableSlot__table ?? throw new ArgumentException("A table cell requires a table slot.", nameof(slot));
        this.renderObject.setChild(__slot.column, ((_TableSlot__table)__slot).row, ((global::Doroti.Framework.Rendering.RenderBox?)null));
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (Table)newWidget;
        DartRuntimePrimitives.Assert(() => !this._doingMountOrUpdate);
        _doingMountOrUpdate = true;
        var oldKeyedRows = new DartMap<global::Doroti.Framework.Foundation.LocalKey, List<Element>>();
        foreach (_TableElementRow__table rowLocal in this._children)
        {
            if ((((_TableElementRow__table)rowLocal).key is not null))
            {
                oldKeyedRows[((_TableElementRow__table)rowLocal).key!] = ((_TableElementRow__table)rowLocal).children;
            }
        }
        IEnumerator<_TableElementRow__table> oldUnkeyedRows = this._children.where(((row) => (((_TableElementRow__table)row).key is null))).GetEnumerator();
        var newChildren = new List<_TableElementRow__table>();
        var taken = new HashSet<List<Element>>();
        for (var rowIndex = 0L; (rowIndex < checked((long)(((Table)__newWidget).children.Count))); rowIndex++)
        {
            TableRow rowAlternate = ((Table)__newWidget).children[(int)(rowIndex)];
            List<Element> oldChildren = default!;
            if (((((TableRow)rowAlternate).key is not null) && oldKeyedRows.ContainsKey(((TableRow)rowAlternate).key)))
            {
                oldChildren = oldKeyedRows.GetValueOrDefault(DartRuntimePrimitives.RequireReference(((TableRow)rowAlternate).key))!;
                taken.Add(oldChildren);
            }
            else
            {
                if (((((TableRow)rowAlternate).key is null) && oldUnkeyedRows.MoveNext()))
                {
                    oldChildren = oldUnkeyedRows.Current.children;
                }
                else
                {
                    oldChildren = new List<Element>();
                }
            }
            var slotsLocal = new List<_TableSlot__table>(Enumerable.Select(Enumerable.Range(0, checked((int)checked((long)(((TableRow)rowAlternate).children.Count)))), ((columnIndex) => new _TableSlot__table(columnIndex, rowIndex))));
            newChildren.Add(new _TableElementRow__table(key: ((TableRow)rowAlternate).key, children: updateChildren(oldChildren, ((TableRow)rowAlternate).children, forgottenChildren: this._forgottenChildren, slots: slotsLocal.Cast<object>().ToList())));
        }
        while (oldUnkeyedRows.MoveNext())
        {
            updateChildren(oldUnkeyedRows.Current.children, new List<Widget>(), forgottenChildren: this._forgottenChildren);
        }
        foreach (List<Element> oldChildrenLocal in oldKeyedRows.Values.where(((list) => !taken.Contains(list))))
        {
            updateChildren(oldChildrenLocal, new List<Widget>(), forgottenChildren: this._forgottenChildren);
        }
        _children = newChildren;
        _updateRenderObjectChildren();
        this._forgottenChildren.Clear();
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => (Equals(this.widget, __newWidget)));
        DartRuntimePrimitives.Assert(() => this._doingMountOrUpdate);
        _doingMountOrUpdate = false;
    }

    internal virtual void _updateRenderObjectChildren()
    {
        this.renderObject.setFlatChildren((Enumerable.Any(this._children) ? checked((long)(this._children[(int)(0L)].children.Count)) : 0L), this._children.expand(((row) =>
        {
            return ((_TableElementRow__table)row).children.map<Element, global::Doroti.Framework.Rendering.RenderBox>(((child) =>
            {
                var box = ((global::Doroti.Framework.Rendering.RenderBox?)((Element)child).renderObject!)!;
                return box;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })).ToList().Cast<global::Doroti.Framework.Rendering.RenderBox?>().ToList());
    }

    public override void visitChildren(global::System.Action<Element> visitor)
    {
        foreach (Element child in this._children.expand(((row) => ((_TableElementRow__table)row).children)))
        {
            if (!this._forgottenChildren.Contains(child))
            {
                visitor(child);
            }
        }
    }

    public override void forgetChild(Element child)
    {
        this._forgottenChildren.Add(child);
        base.forgetChild(child);
        _ = true;
        return;
    }

}

public class TableCell : StatelessWidget
{
    public virtual global::Doroti.Framework.Rendering.TableCellVerticalAlignment? verticalAlignment { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public TableCell(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Rendering.TableCellVerticalAlignment? verticalAlignment = null, Widget child = default!) : base(key: key)
    {
        this.verticalAlignment = verticalAlignment;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)new _TableCell__table(verticalAlignment: this.verticalAlignment, child: new Semantics(role: SemanticsRole.cell, child: this.child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TableCell__table : ParentDataWidget<global::Doroti.Framework.Rendering.TableCellParentData>
{
    public virtual global::Doroti.Framework.Rendering.TableCellVerticalAlignment? verticalAlignment { get; private set; }

    internal _TableCell__table(global::Doroti.Framework.Rendering.TableCellVerticalAlignment? verticalAlignment = null, Widget child = default!) : base(child: child)
    {
        this.verticalAlignment = verticalAlignment;
    }

    public override void applyParentData(global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var parentDataLocal = ((global::Doroti.Framework.Rendering.TableCellParentData?)((global::Doroti.Framework.Rendering.RenderObject)renderObject).parentData!)!;
        if ((!Equals(((global::Doroti.Framework.Rendering.TableCellParentData)parentDataLocal).verticalAlignment, this.verticalAlignment)))
        {
            parentDataLocal.verticalAlignment = this.verticalAlignment;
            (((global::Doroti.Framework.Rendering.RenderObject)renderObject).parent)?.markNeedsLayout();
        }
    }

    public override Type debugTypicalAncestorWidgetClass => typeof(Table);
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Rendering.TableCellVerticalAlignment>("verticalAlignment", this.verticalAlignment));
    }

}

public class _TableSlot__table : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual long column { get; private set; } = default!;
    public virtual long row { get; private set; } = default!;

    internal _TableSlot__table(long column, long row)
    {
        this.column = column;
        this.row = row;
    }

    public override bool Equals(object? other)
    {
        var __other = other as _TableSlot__table;
        if (__other is null) return false;
        if ((!Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is _TableSlot__table) && (this.column == ((_TableSlot__table)((_TableSlot__table)__other)).column)) && (this.row == ((_TableSlot__table)((_TableSlot__table)__other)).row));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.column, this.row));
    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("x", this.column));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("y", this.row));
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
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((fullString ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

