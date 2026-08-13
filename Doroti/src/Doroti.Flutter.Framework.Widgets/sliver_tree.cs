// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/sliver_tree.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public static partial class Sliver_treeLibrary
{
    internal static double _kDefaultRowExtent = 40.0;
}

public class TreeSliverNode<T>
{
    internal virtual T _content { get; private set; } = default!;
    internal virtual List<TreeSliverNode<T>> _children { get; private set; } = default!;
    internal virtual bool _expanded { get; set; } = default!;
    internal virtual long? _depth { get; set; } = default;
    internal virtual TreeSliverNode<T>? _parent { get; set; } = default;

    public TreeSliverNode(T content, List<TreeSliverNode<T>>? children = null, bool expanded = false)
    {
        this._expanded = ((((children is { } __items1104 ? System.Linq.Enumerable.Any(__items1104) : (bool?)null) ?? false)) && expanded);
        this._content = content;
        this._children = (children ?? new List<TreeSliverNode<T>>());
    }

    public virtual T content => this._content;
    public virtual List<TreeSliverNode<T>> children => this._children;
    public virtual bool isExpanded => this._expanded;
    public virtual long? depth => this._depth;
    public virtual TreeSliverNode<T>? parent => this._parent;
    public override string ToString()
    {
        return $"TreeSliverNode: {this.content}, depth: {((this.depth == 0L) ? "root" : this.depth)}, " + $"{(!System.Linq.Enumerable.Any(this.children) ? "leaf" : $"parent, expanded: {this.isExpanded}")}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate Widget TreeSliverNodeBuilder(BuildContext context, dynamic node, global::Doroti.Generated.Framework.Animation.AnimationStyle animationStyle);

public delegate double TreeSliverRowExtentBuilder(dynamic node, global::Doroti.Generated.Framework.Rendering.SliverLayoutDimensions dimensions);

public delegate void TreeSliverNodeCallback(dynamic node);

public interface TreeSliverStateMixin<T>
{
    public bool isExpanded(TreeSliverNode<T> node);
    public bool isActive(TreeSliverNode<T> node);
    public void toggleNode(TreeSliverNode<T> node);
    public void collapseAll();
    public void expandAll();
    public TreeSliverNode<T>? getNodeFor(T content);
    public long? getActiveIndexFor(TreeSliverNode<T> node);
}

public class TreeSliverController
{
    internal virtual TreeSliverStateMixin<object>? _state { get; set; } = default;

    public TreeSliverController()
    {
    }

    public virtual bool isExpanded(dynamic node)
    {
        DartRuntimePrimitives.Assert(() => (this._state is not null));
        return this._state!.isExpanded(node);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isActive(dynamic node)
    {
        DartRuntimePrimitives.Assert(() => (this._state is not null));
        return this._state!.isActive(node);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual dynamic getNodeFor(object? content)
    {
        DartRuntimePrimitives.Assert(() => (this._state is not null));
        return this._state!.getNodeFor(content);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void toggleNode(dynamic node)
    {
        DartRuntimePrimitives.Assert(() => (this._state is not null));
        this._state!.toggleNode(node);
        return;
    }

    public virtual void expandNode(dynamic node)
    {
        DartRuntimePrimitives.Assert(() => (this._state is not null));
        if (!((bool)((dynamic)node).isExpanded))
        {
            this._state!.toggleNode(node);
        }
    }

    public virtual void expandAll()
    {
        DartRuntimePrimitives.Assert(() => (this._state is not null));
        this._state!.expandAll();
    }

    public virtual void collapseAll()
    {
        DartRuntimePrimitives.Assert(() => (this._state is not null));
        this._state!.collapseAll();
    }

    public virtual void collapseNode(dynamic node)
    {
        DartRuntimePrimitives.Assert(() => (this._state is not null));
        if (((bool)((dynamic)node).isExpanded))
        {
            this._state!.toggleNode(node);
        }
    }

    public virtual long? getActiveIndexFor(dynamic node)
    {
        DartRuntimePrimitives.Assert(() => (this._state is not null));
        return this._state!.getActiveIndexFor(node);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TreeSliverController of(BuildContext context)
    {
        _TreeSliverState__sliver_tree<object?>? result__11194 = ((_TreeSliverState__sliver_tree<object?>?)(object?)context.findAncestorStateOfType<_TreeSliverState__sliver_tree<object?>>());
        if ((result__11194 is not null))
        {
            return ((_TreeSliverState__sliver_tree<object>)result__11194).controller;
        }
        throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("TreeController.of() called with a context that does not contain a " + "TreeSliver."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("No TreeSliver ancestor could be found starting from the context that " + "was passed to TreeController.of(). " + "This usually happens when the context provided is from the same " + "StatefulWidget as that whose build function actually creates the " + "TreeSliver widget being sought."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("There are several ways to avoid this problem. The simplest is to use " + "a Builder to get a context that is \"under\" the TreeSliver."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("A more efficient solution is to split your build function into " + "several widgets. This introduces a new context from which you can " + "obtain the TreeSliver. In this solution, you would have an outer " + "widget that creates the TreeSliver populated by instances of your new " + "inner widgets, and then in these inner widgets you would use " + "TreeController.of()."), context.describeElement("The context used was") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TreeSliverController? maybeOf(BuildContext context)
    {
        return context.findAncestorStateOfType<_TreeSliverState__sliver_tree<object?>>()?.controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Sliver_treeLibrary
{
    internal static long _kDefaultSemanticIndexCallback(Widget __unused0, long localIndex) => localIndex;
}

public class TreeSliver<T> : StatefulWidget
{
    public virtual List<TreeSliverNode<T>> tree { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, dynamic, global::Doroti.Generated.Framework.Animation.AnimationStyle, Widget> treeNodeBuilder { get; private set; } = default!;
    public virtual global::System.Func<TreeSliverNode<T>, global::Doroti.Generated.Framework.Rendering.SliverLayoutDimensions, double?> treeRowExtentBuilder { get; private set; } = default!;
    public virtual TreeSliverController? controller { get; private set; }
    public virtual global::System.Action<dynamic>? onNodeToggle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Animation.AnimationStyle? toggleAnimationStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.TreeSliverIndentationType indentation { get; private set; } = default!;
    public virtual bool addAutomaticKeepAlives { get; private set; } = default!;
    public virtual bool addRepaintBoundaries { get; private set; } = default!;
    public virtual bool addSemanticIndexes { get; private set; } = default!;
    public virtual global::System.Func<Widget, long, long?> semanticIndexCallback { get; private set; } = default!;
    public virtual long semanticIndexOffset { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Generated.Framework.Foundation.Key, long?>? findChildIndexCallback { get; private set; }
    public static global::Doroti.Generated.Framework.Animation.AnimationStyle defaultToggleAnimationStyle = new global::Doroti.Generated.Framework.Animation.AnimationStyle(curve: defaultAnimationCurve, duration: defaultAnimationDuration);
    public static global::Doroti.Generated.Framework.Animation.Curve defaultAnimationCurve = global::Doroti.Generated.Framework.Animation.Curves.linear;
    public static Duration defaultAnimationDuration = Duration.Create(milliseconds: 150L);

    public TreeSliver(global::Doroti.Generated.Framework.Foundation.Key? key = null, List<TreeSliverNode<T>> tree = default!, global::System.Func<BuildContext, dynamic, global::Doroti.Generated.Framework.Animation.AnimationStyle, Widget> treeNodeBuilder = default!, global::System.Func<TreeSliverNode<T>, global::Doroti.Generated.Framework.Rendering.SliverLayoutDimensions, double?> treeRowExtentBuilder = default!, TreeSliverController? controller = null, global::System.Action<dynamic>? onNodeToggle = null, global::Doroti.Generated.Framework.Animation.AnimationStyle? toggleAnimationStyle = null, global::Doroti.Generated.Framework.Rendering.TreeSliverIndentationType indentation = default!, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, global::System.Func<Widget, long, long?> semanticIndexCallback = default!, long semanticIndexOffset = 0, global::System.Func<global::Doroti.Generated.Framework.Foundation.Key, long?>? findChildIndexCallback = null) : base(key: key)
    {
        global::System.Func<BuildContext, dynamic, global::Doroti.Generated.Framework.Animation.AnimationStyle, Widget> __treeNodeBuilder = treeNodeBuilder ?? TreeSliver<T>.defaultTreeNodeBuilder;
        global::System.Func<TreeSliverNode<T>, global::Doroti.Generated.Framework.Rendering.SliverLayoutDimensions, double?> __treeRowExtentBuilder = treeRowExtentBuilder ?? ((node, dimensions) => TreeSliver<T>.defaultTreeRowExtentBuilder(node, dimensions));
        global::Doroti.Generated.Framework.Rendering.TreeSliverIndentationType __indentation = indentation ?? global::Doroti.Generated.Framework.Rendering.TreeSliverIndentationType.standard;
        global::System.Func<Widget, long, long?> __semanticIndexCallback = semanticIndexCallback ?? ((widget, index) => Sliver_treeLibrary._kDefaultSemanticIndexCallback(widget, index));
        this.tree = tree;
        this.treeNodeBuilder = __treeNodeBuilder;
        this.treeRowExtentBuilder = __treeRowExtentBuilder;
        this.controller = controller;
        this.onNodeToggle = onNodeToggle;
        this.toggleAnimationStyle = toggleAnimationStyle;
        this.indentation = __indentation;
        this.addAutomaticKeepAlives = addAutomaticKeepAlives;
        this.addRepaintBoundaries = addRepaintBoundaries;
        this.addSemanticIndexes = addSemanticIndexes;
        this.semanticIndexCallback = __semanticIndexCallback;
        this.semanticIndexOffset = semanticIndexOffset;
        this.findChildIndexCallback = findChildIndexCallback;
    }

    public static Widget wrapChildToToggleNode(dynamic node, Widget child)
    {
        return ((Widget)(object?)new Builder(builder: ((global::System.Func<BuildContext, Widget>)((context) => {
return ((Widget)(object?)new GestureDetector(onTap: ((global::System.Action)(() => {
TreeSliverController.of(context).toggleNode(node);
})), child: child));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static double defaultTreeRowExtentBuilder(dynamic node, global::Doroti.Generated.Framework.Rendering.SliverLayoutDimensions dimensions)
    {
        return Sliver_treeLibrary._kDefaultRowExtent;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Widget defaultTreeNodeBuilder(BuildContext context, dynamic node, global::Doroti.Generated.Framework.Animation.AnimationStyle toggleAnimationStyle)
    {
        Duration animationDuration__20900 = (((global::Doroti.Generated.Framework.Animation.AnimationStyle)toggleAnimationStyle).duration ?? TreeSliver<T>.defaultAnimationDuration);
        global::Doroti.Generated.Framework.Animation.Curve animationCurve__21014 = (((global::Doroti.Generated.Framework.Animation.AnimationStyle)toggleAnimationStyle).curve ?? TreeSliver<T>.defaultAnimationCurve);
        long index__21109 = DartRuntimePrimitives.RequireValue(TreeSliverController.of(context).getActiveIndexFor(node));
        return ((Widget)(object?)new Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(8.0), child: new Row(children: new List<Widget> { TreeSliver<T>.wrapChildToToggleNode(node: node, child: SizedBox.CreateSquare(dimension: 30.0, child: (System.Linq.Enumerable.Any(((List<dynamic>)((dynamic)node).children)) ? new AnimatedRotation(key: new global::Doroti.Generated.Framework.Foundation.ValueKey<long>(index__21109), turns: (((bool)((dynamic)node).isExpanded) ? 0.25 : 0.0), duration: animationDuration__20900, curve: animationCurve__21014, child: new Icon(new IconData(9658L), size: 14)) : null))), new SizedBox(width: 8.0), new Text(((string)((dynamic)((dynamic)node).content).ToString())) })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _TreeSliverState__sliver_tree<T>());
}

internal delegate void _AnimationRecord__sliver_tree();

internal class _TreeSliverState__sliver_tree<T> : State<TreeSliver<T>>, TickerProviderStateMixin<TreeSliver<T>>, TreeSliverStateMixin<T>
{
    internal virtual TreeSliverController? _treeController { get; set; } = default;
    internal virtual List<TreeSliverNode<T>> _activeNodes { get; private set; } = new List<TreeSliverNode<T>>();
    internal virtual DartMap<TreeSliverNode<T>, (global::Doroti.Generated.Framework.Animation.CurvedAnimation animation, global::Doroti.Generated.Framework.Animation.AnimationController controller, global::Doroti.Generated.Framework.Foundation.UniqueKey key)> _currentAnimationForParent { get; private set; } = new DartMap<TreeSliverNode<T>, (global::Doroti.Generated.Framework.Animation.CurvedAnimation animation, global::Doroti.Generated.Framework.Animation.AnimationController controller, global::Doroti.Generated.Framework.Foundation.UniqueKey key)>();
    internal virtual DartMap<global::Doroti.Generated.Framework.Foundation.UniqueKey, global::Doroti.Generated.Framework.Rendering.TreeSliverNodesAnimation> _activeAnimations { get; private set; } = new DartMap<global::Doroti.Generated.Framework.Foundation.UniqueKey, (long fromIndex, long toIndex, double value)>().cast<global::Doroti.Generated.Framework.Foundation.UniqueKey, global::Doroti.Generated.Framework.Rendering.TreeSliverNodesAnimation>();
    public virtual HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public virtual TreeSliverController controller => DartRuntimePrimitives.ConvertValue<TreeSliverController>(this._treeController!);
    internal virtual bool _shouldUnpackNode(TreeSliverNode<T> node)
    {
        if (!System.Linq.Enumerable.Any(((TreeSliverNode<T>)node).children))
        {
            return false;
        }
        if ((this._currentAnimationForParent.ContainsKey(node)))
        {
            return true;
        }
        return ((TreeSliverNode<T>)node).isExpanded;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _unpackActiveNodes(long depth = 0, List<TreeSliverNode<T>>? nodes = null, TreeSliverNode<T>? parent = null)
    {
        if ((nodes is null))
        {
            this._activeNodes.Clear();
            nodes = ((TreeSliver<T>)(object)this.widget).tree;
        }
        foreach (TreeSliverNode<T> node__23278 in nodes)
        {
            ((ViewportNotificationMixin)node__23278)._depth = depth;
            node__23278._parent = parent;
            this._activeNodes.Add(node__23278);
            if (_shouldUnpackNode(node__23278))
            {
                _unpackActiveNodes(depth: (depth + 1L), nodes: ((TreeSliverNode<T>)node__23278).children, parent: node__23278);
            }
        }
    }

    public override void initState()
    {
        _unpackActiveNodes();
        DartRuntimePrimitives.Assert(() => (((TreeSliver<T>)(object)this.widget).controller?._state is null), () => (object?)"The provided TreeSliverController is already associated with another " + "TreeSliver. A TreeSliverController can only be associated with one " + "TreeSliver.");
        _treeController = (((TreeSliver<T>)(object)this.widget).controller ?? new TreeSliverController());
        this._treeController!._state = DartRuntimePrimitives.ConvertValue<TreeSliverStateMixin<object>>(this);
        base.initState();
    }

    public override void didUpdateWidget(TreeSliver<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() => (this._treeController is not null));
        if (((((TreeSliver<T>)oldWidget).controller is null) && (((TreeSliver<T>)(object)this.widget).controller is not null)))
        {
            this._treeController!._state = null;
            _treeController = ((TreeSliver<T>)(object)this.widget).controller;
            this._treeController!._state = DartRuntimePrimitives.ConvertValue<TreeSliverStateMixin<object>>(this);
        }
        else
        {
            if (((((TreeSliver<T>)oldWidget).controller is not null) && (((TreeSliver<T>)(object)this.widget).controller is null)))
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(((TreeSliver<T>)oldWidget).controller, this._treeController)));
                ((TreeSliver<T>)oldWidget).controller!._state = null;
                _treeController = new TreeSliverController();
                this._treeController!._state = DartRuntimePrimitives.ConvertValue<TreeSliverStateMixin<object>>(this);
            }
            else
            {
                if ((!object.Equals(((TreeSliver<T>)oldWidget).controller, ((TreeSliver<T>)(object)this.widget).controller)))
                {
                    DartRuntimePrimitives.Assert(() => (((TreeSliver<T>)oldWidget).controller is not null));
                    DartRuntimePrimitives.Assert(() => (((TreeSliver<T>)(object)this.widget).controller is not null));
                    DartRuntimePrimitives.Assert(() => (object.Equals(((TreeSliver<T>)oldWidget).controller, this._treeController)));
                    this._treeController!._state = null;
                    _treeController = ((TreeSliver<T>)(object)this.widget).controller;
                    this._treeController!._state = DartRuntimePrimitives.ConvertValue<TreeSliverStateMixin<object>>(this);
                }
            }
        }
        DartRuntimePrimitives.Assert(() => (this._treeController is not null));
        DartRuntimePrimitives.Assert(() => (this._treeController!._state is not null));
        _unpackActiveNodes();
    }

    public override void dispose()
    {
        this._treeController!._state = null;
        foreach ((global::Doroti.Generated.Framework.Animation.CurvedAnimation animation, global::Doroti.Generated.Framework.Animation.AnimationController controller, global::Doroti.Generated.Framework.Foundation.UniqueKey key) record__25754 in this._currentAnimationForParent.Values)
        {
            record__25754.animation.dispose();
            record__25754.controller.dispose();
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Generated.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new _SliverTree__sliver_tree(itemCount: checked((long)(this._activeNodes.Count)), activeAnimations: this._activeAnimations.cast<global::Doroti.Generated.Framework.Foundation.UniqueKey, (long fromIndex, long toIndex, double value)>(), itemBuilder: ((global::System.Func<BuildContext, long, Widget?>)((context, index) => {
TreeSliverNode<T> node__26145 = this._activeNodes[(int)(index)];
Widget child__26188 = this.widget.treeNodeBuilder(context, node__26145, (((TreeSliver<T>)(object)this.widget).toggleAnimationStyle ?? TreeSliver<object>.defaultToggleAnimationStyle));
if (((TreeSliver<T>)(object)this.widget).addRepaintBoundaries)
{
    child__26188 = DartRuntimePrimitives.ConvertValue<Widget>(new RepaintBoundary(child: child__26188));
}
if (((TreeSliver<T>)(object)this.widget).addSemanticIndexes)
{
    long? semanticIndex__26512 = this.widget.semanticIndexCallback(child__26188, index);
    if ((semanticIndex__26512 is not null))
    {
        long semanticIndex__26512__value26586 = DartRuntimePrimitives.RequireValue(semanticIndex__26512);
        child__26188 = DartRuntimePrimitives.ConvertValue<Widget>(new IndexedSemantics(index: (DartRuntimePrimitives.RequireValue(semanticIndex__26512__value26586) + ((TreeSliver<T>)(object)this.widget).semanticIndexOffset), child: child__26188));
    }
}
return ((Widget?)(object?)new _TreeNodeParentDataWidget__sliver_tree(depth: DartRuntimePrimitives.RequireValue(((TreeSliverNode<T>)node__26145).depth), child: child__26188));
throw new InvalidOperationException("Dart closure completed without a value.");
})), itemExtentBuilder: ((ItemExtentBuilder)((index, dimensions) => {
return this.widget.treeRowExtentBuilder(this._activeNodes[(int)(index)], dimensions);
throw new InvalidOperationException("Dart closure completed without a value.");
})), addAutomaticKeepAlives: ((TreeSliver<T>)(object)this.widget).addAutomaticKeepAlives, findChildIndexCallback: (global::System.Func<global::Doroti.Generated.Framework.Foundation.Key, long?>?)((TreeSliver<T>)(object)this.widget).findChildIndexCallback, indentation: ((TreeSliver<T>)(object)this.widget).indentation.value));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isExpanded(TreeSliverNode<T> node)
    {
        return (_getNode(((TreeSliverNode<T>)node).content, ((TreeSliver<T>)(object)this.widget).tree)?.isExpanded ?? false);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isActive(TreeSliverNode<T> node) => this._activeNodes.Contains(node);
    public virtual TreeSliverNode<T>? getNodeFor(T content) => _getNode(content, ((TreeSliver<T>)(object)this.widget).tree);
    internal virtual TreeSliverNode<T>? _getNode(T content, List<TreeSliverNode<T>> tree)
    {
        var nextDepth__27628 = new List<TreeSliverNode<T>>();
        foreach (var node__27678 in tree)
        {
            if (EqualityComparer<T>.Default.Equals(((TreeSliverNode<T>)node__27678).content, content))
            {
                return node__27678;
            }
            if (System.Linq.Enumerable.Any(((TreeSliverNode<T>)node__27678).children))
            {
                nextDepth__27628.AddRange(((TreeSliverNode<T>)node__27678).children.Cast<TreeSliverNode<T>>());
            }
        }
        if (System.Linq.Enumerable.Any(nextDepth__27628))
        {
            return ((TreeSliverNode<T>?)(object?)_getNode(content, nextDepth__27628));
        }
        return default;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long? getActiveIndexFor(TreeSliverNode<T> node)
    {
        if (this._activeNodes.Contains(node))
        {
            return ((long)((dynamic)this._activeNodes).IndexOf(node));
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void expandAll()
    {
        var activeNodesToExpand__28170 = new List<TreeSliverNode<T>>();
        _expandAll(((TreeSliver<T>)(object)this.widget).tree, activeNodesToExpand__28170);
        System.Linq.Enumerable.Reverse(activeNodesToExpand__28170).forEach((__arg0) => ((global::System.Action<TreeSliverNode<T>>)this.toggleNode)(__arg0));
    }

    internal virtual void _expandAll(List<TreeSliverNode<T>> tree, List<TreeSliverNode<T>> activeNodesToExpand)
    {
        foreach (var node__28434 in tree)
        {
            if (System.Linq.Enumerable.Any(((TreeSliverNode<T>)node__28434).children))
            {
                _expandAll(((TreeSliverNode<T>)node__28434).children, activeNodesToExpand);
                if (!((TreeSliverNode<T>)node__28434).isExpanded)
                {
                    if (this._activeNodes.Contains(node__28434))
                    {
                        activeNodesToExpand.Add(node__28434);
                    }
                    else
                    {
                        node__28434._expanded = true;
                    }
                }
            }
        }
    }

    public virtual void collapseAll()
    {
        var activeNodesToCollapse__29158 = new List<TreeSliverNode<T>>();
        _collapseAll(((TreeSliver<T>)(object)this.widget).tree, activeNodesToCollapse__29158);
        System.Linq.Enumerable.Reverse(activeNodesToCollapse__29158).forEach((__arg0) => ((global::System.Action<TreeSliverNode<T>>)this.toggleNode)(__arg0));
    }

    internal virtual void _collapseAll(List<TreeSliverNode<T>> tree, List<TreeSliverNode<T>> activeNodesToCollapse)
    {
        foreach (var node__29434 in tree)
        {
            if (System.Linq.Enumerable.Any(((TreeSliverNode<T>)node__29434).children))
            {
                _collapseAll(((TreeSliverNode<T>)node__29434).children, activeNodesToCollapse);
                if (((TreeSliverNode<T>)node__29434).isExpanded)
                {
                    if (this._activeNodes.Contains(node__29434))
                    {
                        activeNodesToCollapse.Add(node__29434);
                    }
                    else
                    {
                        node__29434._expanded = false;
                    }
                }
            }
        }
    }

    internal virtual void _updateActiveAnimations()
    {
        this._activeAnimations.Clear();
        foreach (TreeSliverNode<T> node__30457 in this._currentAnimationForParent.Keys)
        {
            (global::Doroti.Generated.Framework.Animation.CurvedAnimation animation, global::Doroti.Generated.Framework.Animation.AnimationController controller, global::Doroti.Generated.Framework.Foundation.UniqueKey key) animationRecord__30529 = DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<(global::Doroti.Generated.Framework.Animation.CurvedAnimation animation, global::Doroti.Generated.Framework.Animation.AnimationController controller, global::Doroti.Generated.Framework.Foundation.UniqueKey key)>(this._currentAnimationForParent, node__30457));
            long leadingChildIndex__30598 = (((long)((dynamic)this._activeNodes).IndexOf(node__30457)) + 1L);
            global::Doroti.Generated.Framework.Rendering.TreeSliverNodesAnimation animatingChildren__30687 = ((global::Doroti.Generated.Framework.Rendering.TreeSliverNodesAnimation)(object?)(fromIndex: leadingChildIndex__30598, toIndex: ((leadingChildIndex__30598 + checked((long)(((TreeSliverNode<T>)node__30457).children.Count))) - 1L), value: animationRecord__30529.animation.value));
            this._activeAnimations[animationRecord__30529.key] = animatingChildren__30687;
        }
    }

    public virtual void toggleNode(TreeSliverNode<T> node)
    {
        DartRuntimePrimitives.Assert(() => this._activeNodes.Contains(node));
        if (!System.Linq.Enumerable.Any(((TreeSliverNode<T>)node).children))
        {
            return;
        }
        setState(((global::System.Action)(() => {
node._expanded = !((TreeSliverNode<T>)node)._expanded;
if ((((TreeSliver<T>)(object)this.widget).onNodeToggle is not null))
{
    ((TreeSliver<T>)(object)this.widget).onNodeToggle!(node);
}
if ((this._currentAnimationForParent.ContainsKey(node)))
{
    DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<(global::Doroti.Generated.Framework.Animation.CurvedAnimation animation, global::Doroti.Generated.Framework.Animation.AnimationController controller, global::Doroti.Generated.Framework.Foundation.UniqueKey key)>(this._currentAnimationForParent, node)).animation.dispose();
}
if (((object.Equals(((TreeSliver<T>)(object)this.widget).toggleAnimationStyle, global::Doroti.Generated.Framework.Animation.AnimationStyle.noAnimation)) || (object.Equals(((TreeSliver<T>)(object)this.widget).toggleAnimationStyle?.duration, Duration.zero))))
{
    _unpackActiveNodes();
    return;
}
global::Doroti.Generated.Framework.Animation.AnimationController controller__32032 = ((Func<global::Doroti.Generated.Framework.Animation.AnimationController>)(() =>
{            var __cascade = (DartCollectionRuntime.NullableMapValue<(global::Doroti.Generated.Framework.Animation.CurvedAnimation animation, global::Doroti.Generated.Framework.Animation.AnimationController controller, global::Doroti.Generated.Framework.Foundation.UniqueKey key)>(this._currentAnimationForParent, node)?.controller ?? new global::Doroti.Generated.Framework.Animation.AnimationController(value: (((TreeSliverNode<T>)node)._expanded ? 0.0 : 1.0), vsync: this, duration: (((TreeSliver<T>)(object)this.widget).toggleAnimationStyle?.duration ?? TreeSliver<object>.defaultAnimationDuration)));
            __cascade.addStatusListener(((AnimationStatusListener)((status) => {
switch (status)
{
    case global::Doroti.Generated.Framework.Animation.AnimationStatus.dismissed:
    case global::Doroti.Generated.Framework.Animation.AnimationStatus.completed:
        {
            DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<(global::Doroti.Generated.Framework.Animation.CurvedAnimation animation, global::Doroti.Generated.Framework.Animation.AnimationController controller, global::Doroti.Generated.Framework.Foundation.UniqueKey key)>(this._currentAnimationForParent, node)).animation.dispose();
            DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<(global::Doroti.Generated.Framework.Animation.CurvedAnimation animation, global::Doroti.Generated.Framework.Animation.AnimationController controller, global::Doroti.Generated.Framework.Foundation.UniqueKey key)>(this._currentAnimationForParent, node)).controller.dispose();
            this._currentAnimationForParent.remove(node);
            _updateActiveAnimations();
            if (!((TreeSliverNode<T>)node)._expanded)
            {
                _unpackActiveNodes();
            }
            break;
        }
    case global::Doroti.Generated.Framework.Animation.AnimationStatus.forward:
    case global::Doroti.Generated.Framework.Animation.AnimationStatus.reverse:
        break;
}
})));
            __cascade.addListener(((global::System.Action)(() => {
setState(((global::System.Action)(() => {
_updateActiveAnimations();
})));
})));
            return __cascade;        }))();
switch (((global::Doroti.Generated.Framework.Animation.AnimationController)controller__32032).status)
{
    case global::Doroti.Generated.Framework.Animation.AnimationStatus.forward:
    case global::Doroti.Generated.Framework.Animation.AnimationStatus.reverse:
        {
            controller__32032.stop();
            break;
        }
    case global::Doroti.Generated.Framework.Animation.AnimationStatus.dismissed:
    case global::Doroti.Generated.Framework.Animation.AnimationStatus.completed:
        break;
}
var newAnimation__33705 = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: controller__32032, curve: (((TreeSliver<T>)(object)this.widget).toggleAnimationStyle?.curve ?? TreeSliver<object>.defaultAnimationCurve));
this._currentAnimationForParent[node] = (animation: newAnimation__33705, controller: controller__32032, key: new global::Doroti.Generated.Framework.Foundation.UniqueKey());
switch (((TreeSliverNode<T>)node)._expanded)
{
    case true:
        {
            _unpackActiveNodes();
            controller__32032.forward();
            break;
        }
    case false:
        {
            controller__32032.reverse();
            break;
        }
}
})));
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>();
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<_WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Generated.Framework.Scheduler.Ticker)(object?)result__17553);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
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
            TickerModeData values__18318 = this._tickerModeNotifier!.value;
            bool muted__18372 = !((TickerModeData)values__18318).enabled;
            foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

internal class _TreeNodeParentDataWidget__sliver_tree : ParentDataWidget<global::Doroti.Generated.Framework.Rendering.TreeSliverNodeParentData>
{
    public virtual long depth { get; private set; } = default!;

    internal _TreeNodeParentDataWidget__sliver_tree(long depth, Widget child) : base(child: child)
    {
        this.depth = depth;
        System.Diagnostics.Debug.Assert((depth >= 0L));
    }

    public override void applyParentData(global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var parentData__34671 = ((global::Doroti.Generated.Framework.Rendering.TreeSliverNodeParentData?)(object?)((global::Doroti.Generated.Framework.Rendering.RenderObject)renderObject).parentData!)!;
        var needsLayout__34746 = false;
        if ((((global::Doroti.Generated.Framework.Rendering.TreeSliverNodeParentData)parentData__34671).depth != this.depth))
        {
            DartRuntimePrimitives.Assert(() => (this.depth >= 0L));
            parentData__34671.depth = this.depth;
            needsLayout__34746 = true;
        }
        if (needsLayout__34746)
        {
            ((dynamic)((global::Doroti.Generated.Framework.Rendering.RenderObject)renderObject).parent)?.markNeedsLayout();
        }
    }

    public override Type debugTypicalAncestorWidgetClass => typeof(_SliverTree__sliver_tree);
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.IntProperty("depth", this.depth));
    }

}

internal class _SliverTree__sliver_tree : SliverVariedExtentList
{
    public virtual DartMap<global::Doroti.Generated.Framework.Foundation.UniqueKey, (long fromIndex, long toIndex, double value)> activeAnimations { get; private set; } = default!;
    public virtual double indentation { get; private set; } = default!;

    internal _SliverTree__sliver_tree(global::System.Func<BuildContext, long, Widget?> itemBuilder, ItemExtentBuilder itemExtentBuilder, DartMap<global::Doroti.Generated.Framework.Foundation.UniqueKey, (long fromIndex, long toIndex, double value)> activeAnimations, double indentation, global::System.Func<global::Doroti.Generated.Framework.Foundation.Key, long?>? findChildIndexCallback = null, long itemCount = default!, bool addAutomaticKeepAlives = true) : base(itemExtentBuilder: itemExtentBuilder, @delegate: new SliverChildBuilderDelegate((global::System.Func<BuildContext, long, Widget?>)itemBuilder, findChildIndexCallback: (global::System.Func<global::Doroti.Generated.Framework.Foundation.Key, long?>?)findChildIndexCallback, childCount: itemCount, addAutomaticKeepAlives: addAutomaticKeepAlives, addRepaintBoundaries: false, addSemanticIndexes: false))
    {
        this.activeAnimations = activeAnimations;
        this.indentation = indentation;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        var element__36153 = ((SliverMultiBoxAdaptorElement?)(object?)context)!;
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderTreeSliver(itemExtentBuilder: (ItemExtentBuilder)this.itemExtentBuilder, activeAnimations: this.activeAnimations, indentation: this.indentation, childManager: element__36153));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderTreeSliver)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.RenderTreeSliver>)(() =>
{            var __cascade = __renderObject;
            __cascade.itemExtentBuilder = this.itemExtentBuilder;
            __cascade.activeAnimations = this.activeAnimations;
            __cascade.indentation = this.indentation;
            return __cascade;        }))());
    }

}

