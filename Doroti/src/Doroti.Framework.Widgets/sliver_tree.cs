// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/sliver_tree.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public static partial class Sliver_treeLibrary
{
    internal static double _kDefaultRowExtent = 40.0;
}

public interface ITreeSliverNode
{
    object? content { get; }
    IEnumerable<ITreeSliverNode> children { get; }
    bool isExpanded { get; }
    long? depth { get; }
}

internal interface ITreeSliverState : IState
{
    TreeSliverController controller { get; }
    bool isExpanded(ITreeSliverNode node);
    bool isActive(ITreeSliverNode node);
    void toggleNode(ITreeSliverNode node);
    void collapseAll();
    void expandAll();
    ITreeSliverNode? getNodeFor(object? content);
    long? getActiveIndexFor(ITreeSliverNode node);
}

public class TreeSliverNode<T> : ITreeSliverNode
{
    object? ITreeSliverNode.content => content;
    IEnumerable<ITreeSliverNode> ITreeSliverNode.children => children;

    internal virtual T _content { get; private set; } = default!;
    internal virtual List<TreeSliverNode<T>> _children { get; private set; } = default!;
    internal virtual bool _expanded { get; set; } = default!;
    internal virtual long? _depth { get; set; } = default;
    internal virtual TreeSliverNode<T>? _parent { get; set; } = default;

    public TreeSliverNode(T content, List<TreeSliverNode<T>>? children = null, bool expanded = false)
    {
        _expanded = ((children is { } __items1104 ? System.Linq.Enumerable.Any(__items1104) : (bool?)null) ?? false) && expanded;
        _content = content;
        _children = children ?? new List<TreeSliverNode<T>>();
    }

    public virtual T content => _content;
    public virtual List<TreeSliverNode<T>> children => _children;
    public virtual bool isExpanded => _expanded;
    public virtual long? depth => _depth;
    public virtual TreeSliverNode<T>? parent => _parent;
    public override string ToString()
    {
        return $"TreeSliverNode: {content}, depth: {((depth == 0L) ? "root" : depth)}, " + $"{(!Enumerable.Any(children) ? "leaf" : $"parent, expanded: {isExpanded}")}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate Widget TreeSliverNodeBuilder(BuildContext context, ITreeSliverNode node, global::Doroti.Framework.Animation.AnimationStyle animationStyle);

public delegate double TreeSliverRowExtentBuilder(ITreeSliverNode node, global::Doroti.Framework.Rendering.SliverLayoutDimensions dimensions);

public delegate void TreeSliverNodeCallback(ITreeSliverNode node);

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
    internal virtual ITreeSliverState? _state { get; set; } = default;

    public TreeSliverController()
    {
    }

    public virtual bool isExpanded(ITreeSliverNode node)
    {
        DartRuntimePrimitives.Assert(() => _state is not null);
        return _state!.isExpanded(node);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isActive(ITreeSliverNode node)
    {
        DartRuntimePrimitives.Assert(() => _state is not null);
        return _state!.isActive(node);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ITreeSliverNode? getNodeFor(object? content)
    {
        DartRuntimePrimitives.Assert(() => _state is not null);
        return _state!.getNodeFor(content);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void toggleNode(ITreeSliverNode node)
    {
        DartRuntimePrimitives.Assert(() => _state is not null);
        _state!.toggleNode(node);
        return;
    }

    public virtual void expandNode(ITreeSliverNode node)
    {
        DartRuntimePrimitives.Assert(() => _state is not null);
        if (!node.isExpanded)
        {
            _state!.toggleNode(node);
        }
    }

    public virtual void expandAll()
    {
        DartRuntimePrimitives.Assert(() => _state is not null);
        _state!.expandAll();
    }

    public virtual void collapseAll()
    {
        DartRuntimePrimitives.Assert(() => _state is not null);
        _state!.collapseAll();
    }

    public virtual void collapseNode(ITreeSliverNode node)
    {
        DartRuntimePrimitives.Assert(() => _state is not null);
        if (node.isExpanded)
        {
            _state!.toggleNode(node);
        }
    }

    public virtual long? getActiveIndexFor(ITreeSliverNode node)
    {
        DartRuntimePrimitives.Assert(() => _state is not null);
        return _state!.getActiveIndexFor(node);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TreeSliverController of(BuildContext context)
    {
        ITreeSliverState? result = context.findAncestorStateOfType<ITreeSliverState>();
        if (result is not null)
        {
            return result.controller;
        }
        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("TreeController.of() called with a context that does not contain a " + "TreeSliver."), new global::Doroti.Framework.Foundation.ErrorDescription("No TreeSliver ancestor could be found starting from the context that " + "was passed to TreeController.of(). " + "This usually happens when the context provided is from the same " + "StatefulWidget as that whose build function actually creates the " + "TreeSliver widget being sought."), new global::Doroti.Framework.Foundation.ErrorHint("There are several ways to avoid this problem. The simplest is to use " + "a Builder to get a context that is \"under\" the TreeSliver."), new global::Doroti.Framework.Foundation.ErrorHint("A more efficient solution is to split your build function into " + "several widgets. This introduces a new context from which you can " + "obtain the TreeSliver. In this solution, you would have an outer " + "widget that creates the TreeSliver populated by instances of your new " + "inner widgets, and then in these inner widgets you would use " + "TreeController.of()."), context.describeElement("The context used was") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TreeSliverController? maybeOf(BuildContext context)
    {
        return context.findAncestorStateOfType<ITreeSliverState>()?.controller;
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
    public virtual global::System.Func<BuildContext, TreeSliverNode<T>, global::Doroti.Framework.Animation.AnimationStyle, Widget> treeNodeBuilder { get; private set; } = default!;
    public virtual global::System.Func<TreeSliverNode<T>, global::Doroti.Framework.Rendering.SliverLayoutDimensions, double?> treeRowExtentBuilder { get; private set; } = default!;
    public virtual TreeSliverController? controller { get; private set; }
    public virtual global::System.Action<TreeSliverNode<T>>? onNodeToggle { get; private set; }
    public virtual global::Doroti.Framework.Animation.AnimationStyle? toggleAnimationStyle { get; private set; }
    public virtual global::Doroti.Framework.Rendering.TreeSliverIndentationType indentation { get; private set; } = default!;
    public virtual bool addAutomaticKeepAlives { get; private set; } = default!;
    public virtual bool addRepaintBoundaries { get; private set; } = default!;
    public virtual bool addSemanticIndexes { get; private set; } = default!;
    public virtual global::System.Func<Widget, long, long?> semanticIndexCallback { get; private set; } = default!;
    public virtual long semanticIndexOffset { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Foundation.Key, long?>? findChildIndexCallback { get; private set; }
    public static global::Doroti.Framework.Animation.AnimationStyle defaultToggleAnimationStyle = new global::Doroti.Framework.Animation.AnimationStyle(curve: defaultAnimationCurve, duration: defaultAnimationDuration);
    public static global::Doroti.Framework.Animation.Curve defaultAnimationCurve = Curves.linear;
    public static Duration defaultAnimationDuration = Duration.Create(milliseconds: 150L);

    public TreeSliver(global::Doroti.Framework.Foundation.Key? key = null, List<TreeSliverNode<T>> tree = default!, global::System.Func<BuildContext, TreeSliverNode<T>, global::Doroti.Framework.Animation.AnimationStyle, Widget> treeNodeBuilder = default!, global::System.Func<TreeSliverNode<T>, global::Doroti.Framework.Rendering.SliverLayoutDimensions, double?> treeRowExtentBuilder = default!, TreeSliverController? controller = null, global::System.Action<TreeSliverNode<T>>? onNodeToggle = null, global::Doroti.Framework.Animation.AnimationStyle? toggleAnimationStyle = null, global::Doroti.Framework.Rendering.TreeSliverIndentationType indentation = default!, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, global::System.Func<Widget, long, long?> semanticIndexCallback = default!, long semanticIndexOffset = 0, global::System.Func<global::Doroti.Framework.Foundation.Key, long?>? findChildIndexCallback = null) : base(key: key)
    {
        global::System.Func<BuildContext, TreeSliverNode<T>, global::Doroti.Framework.Animation.AnimationStyle, Widget> __treeNodeBuilder = treeNodeBuilder ?? TreeSliver<T>.defaultTreeNodeBuilder;
        global::System.Func<TreeSliverNode<T>, global::Doroti.Framework.Rendering.SliverLayoutDimensions, double?> __treeRowExtentBuilder = treeRowExtentBuilder ?? ((node, dimensions) => TreeSliver<T>.defaultTreeRowExtentBuilder(node, dimensions));
        global::Doroti.Framework.Rendering.TreeSliverIndentationType __indentation = indentation ?? TreeSliverIndentationType.standard;
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

    public static Widget wrapChildToToggleNode(TreeSliverNode<T> node, Widget child)
    {
        return new Builder(builder: (context) =>
        {
            return new GestureDetector(onTap: () =>
            {
                TreeSliverController.of(context).toggleNode(node);
            }, child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static double defaultTreeRowExtentBuilder(TreeSliverNode<T> node, global::Doroti.Framework.Rendering.SliverLayoutDimensions dimensions)
    {
        return Sliver_treeLibrary._kDefaultRowExtent;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Widget defaultTreeNodeBuilder(BuildContext context, TreeSliverNode<T> node, global::Doroti.Framework.Animation.AnimationStyle toggleAnimationStyle)
    {
        Duration animationDuration = toggleAnimationStyle.duration ?? defaultAnimationDuration;
        global::Doroti.Framework.Animation.Curve animationCurve = toggleAnimationStyle.curve ?? defaultAnimationCurve;
        long index = DartRuntimePrimitives.RequireValue(TreeSliverController.of(context).getActiveIndexFor(node));
        return new Padding(padding: EdgeInsets.CreateAll(8.0), child: new Row(children: new List<Widget> { TreeSliver<T>.wrapChildToToggleNode(node: node, child: SizedBox.CreateSquare(dimension: 30.0, child: Enumerable.Any(node.children) ? new AnimatedRotation(key: new global::Doroti.Framework.Foundation.ValueKey<long>(index), turns: node.isExpanded ? 0.25 : 0.0, duration: animationDuration, curve: animationCurve, child: new Icon(new IconData(9658L), size: 14)) : null)), new SizedBox(width: 8.0), new Text($"{node.content}") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _TreeSliverState__sliver_tree<T>());
}

internal delegate void _AnimationRecord__sliver_tree();

internal class _TreeSliverState__sliver_tree<T> : State<TreeSliver<T>>, TickerProviderStateMixin<TreeSliver<T>>, TreeSliverStateMixin<T>, ITreeSliverState
{
    bool ITreeSliverState.isExpanded(ITreeSliverNode node) => node is TreeSliverNode<T> typed && isExpanded(typed);
    bool ITreeSliverState.isActive(ITreeSliverNode node) => node is TreeSliverNode<T> typed && isActive(typed);
    long? ITreeSliverState.getActiveIndexFor(ITreeSliverNode node) => node is TreeSliverNode<T> typed ? getActiveIndexFor(typed) : null;
    void ITreeSliverState.toggleNode(ITreeSliverNode node) => toggleNode((TreeSliverNode<T>)node);
    ITreeSliverNode? ITreeSliverState.getNodeFor(object? content) => content is T typed ? getNodeFor(typed) : content is null && default(T) is null ? getNodeFor(default!) : null;

    internal virtual TreeSliverController? _treeController { get; set; } = default;
    internal virtual List<TreeSliverNode<T>> _activeNodes { get; private set; } = new List<TreeSliverNode<T>>();
    internal virtual DartMap<TreeSliverNode<T>, (global::Doroti.Framework.Animation.CurvedAnimation animation, global::Doroti.Framework.Animation.AnimationController controller, global::Doroti.Framework.Foundation.UniqueKey key)> _currentAnimationForParent { get; private set; } = new DartMap<TreeSliverNode<T>, (global::Doroti.Framework.Animation.CurvedAnimation animation, global::Doroti.Framework.Animation.AnimationController controller, global::Doroti.Framework.Foundation.UniqueKey key)>();
    internal virtual DartMap<global::Doroti.Framework.Foundation.UniqueKey, global::Doroti.Framework.Rendering.TreeSliverNodesAnimation> _activeAnimations { get; private set; } = new DartMap<global::Doroti.Framework.Foundation.UniqueKey, global::Doroti.Framework.Rendering.TreeSliverNodesAnimation>().cast<global::Doroti.Framework.Foundation.UniqueKey, global::Doroti.Framework.Rendering.TreeSliverNodesAnimation>();
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public virtual TreeSliverController controller => DartRuntimePrimitives.ConvertValue<TreeSliverController>(_treeController!);
    internal virtual bool _shouldUnpackNode(TreeSliverNode<T> node)
    {
        if (!Enumerable.Any(node.children))
        {
            return false;
        }
        if (_currentAnimationForParent.ContainsKey(node))
        {
            return true;
        }
        return node.isExpanded;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _unpackActiveNodes(long depth = 0, List<TreeSliverNode<T>>? nodes = null, TreeSliverNode<T>? parent = null)
    {
        if (nodes is null)
        {
            _activeNodes.Clear();
            nodes = widget.tree;
        }
        foreach (TreeSliverNode<T> node in nodes)
        {
            node._depth = depth;
            node._parent = parent;
            _activeNodes.Add(node);
            if (_shouldUnpackNode(node))
            {
                _unpackActiveNodes(depth: depth + 1L, nodes: node.children, parent: node);
            }
        }
    }

    public override void initState()
    {
        _unpackActiveNodes();
        DartRuntimePrimitives.Assert(() => widget.controller?._state is null, () => (object?)"The provided TreeSliverController is already associated with another " + "TreeSliver. A TreeSliverController can only be associated with one " + "TreeSliver.");
        _treeController = widget.controller ?? new TreeSliverController();
        _treeController!._state = this;
        base.initState();
    }

    public override void didUpdateWidget(TreeSliver<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() => _treeController is not null);
        if ((oldWidget.controller is null) && (widget.controller is not null))
        {
            _treeController!._state = null;
            _treeController = widget.controller;
            _treeController!._state = this;
        }
        else
        {
            if ((oldWidget.controller is not null) && (widget.controller is null))
            {
                DartRuntimePrimitives.Assert(() => Equals(oldWidget.controller, _treeController));
                oldWidget.controller!._state = null;
                _treeController = new TreeSliverController();
                _treeController!._state = this;
            }
            else
            {
                if (!Equals(oldWidget.controller, widget.controller))
                {
                    DartRuntimePrimitives.Assert(() => oldWidget.controller is not null);
                    DartRuntimePrimitives.Assert(() => widget.controller is not null);
                    DartRuntimePrimitives.Assert(() => Equals(oldWidget.controller, _treeController));
                    _treeController!._state = null;
                    _treeController = widget.controller;
                    _treeController!._state = this;
                }
            }
        }
        DartRuntimePrimitives.Assert(() => _treeController is not null);
        DartRuntimePrimitives.Assert(() => _treeController!._state is not null);
        _unpackActiveNodes();
    }

    public override void dispose()
    {
        _treeController!._state = null;
        foreach ((global::Doroti.Framework.Animation.CurvedAnimation animation, global::Doroti.Framework.Animation.AnimationController controller, global::Doroti.Framework.Foundation.UniqueKey key) @record in _currentAnimationForParent.Values)
        {
            @record.animation.dispose();
            @record.controller.dispose();
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if (_tickers is not null)
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
                    {
                        if (ticker.isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return new _SliverTree__sliver_tree(itemCount: checked(_activeNodes.Count), activeAnimations: _activeAnimations.cast<global::Doroti.Framework.Foundation.UniqueKey, global::Doroti.Framework.Rendering.TreeSliverNodesAnimation>(), itemBuilder: (context, index) =>
        {
            TreeSliverNode<T> node = _activeNodes[(int)index];
            Widget childLocal = widget.treeNodeBuilder(context, node, widget.toggleAnimationStyle ?? TreeSliver<object>.defaultToggleAnimationStyle);
            if (widget.addRepaintBoundaries)
            {
                childLocal = DartRuntimePrimitives.ConvertValue<Widget>(new RepaintBoundary(child: childLocal));
            }
            if (widget.addSemanticIndexes)
            {
                long? semanticIndex = widget.semanticIndexCallback(childLocal, index);
                if (semanticIndex is not null)
                {
                    long semanticIndex__26512__value26586 = DartRuntimePrimitives.RequireValue(semanticIndex);
                    childLocal = DartRuntimePrimitives.ConvertValue<Widget>(new IndexedSemantics(index: DartRuntimePrimitives.RequireValue(semanticIndex__26512__value26586) + widget.semanticIndexOffset, child: childLocal));
                }
            }
            return (Widget?)new _TreeNodeParentDataWidget__sliver_tree(depth: DartRuntimePrimitives.RequireValue(node.depth), child: childLocal);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, itemExtentBuilder: (index, dimensions) =>
        {
            return widget.treeRowExtentBuilder(_activeNodes[(int)index], dimensions);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, addAutomaticKeepAlives: widget.addAutomaticKeepAlives, findChildIndexCallback: widget.findChildIndexCallback, indentation: widget.indentation.value);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isExpanded(TreeSliverNode<T> node)
    {
        return _getNode(node.content, widget.tree)?.isExpanded ?? false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isActive(TreeSliverNode<T> node) => _activeNodes.Contains(node);
    public virtual TreeSliverNode<T>? getNodeFor(T content) => _getNode(content, widget.tree);
    internal virtual TreeSliverNode<T>? _getNode(T content, List<TreeSliverNode<T>> tree)
    {
        var nextDepth = new List<TreeSliverNode<T>>();
        foreach (var node in tree)
        {
            if (EqualityComparer<T>.Default.Equals(node.content, content))
            {
                return node;
            }
            if (Enumerable.Any(node.children))
            {
                nextDepth.AddRange(node.children.Cast<TreeSliverNode<T>>());
            }
        }
        if (Enumerable.Any(nextDepth))
        {
            return _getNode(content, nextDepth);
        }
        return default;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long? getActiveIndexFor(TreeSliverNode<T> node)
    {
        if (_activeNodes.Contains(node))
        {
            return _activeNodes.IndexOf(node);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void expandAll()
    {
        var activeNodesToExpand = new List<TreeSliverNode<T>>();
        _expandAll(widget.tree, activeNodesToExpand);
        Enumerable.Reverse(activeNodesToExpand).forEach((__arg0) => ((global::System.Action<TreeSliverNode<T>>)toggleNode)(__arg0));
    }

    internal virtual void _expandAll(List<TreeSliverNode<T>> tree, List<TreeSliverNode<T>> activeNodesToExpand)
    {
        foreach (var node in tree)
        {
            if (Enumerable.Any(node.children))
            {
                _expandAll(node.children, activeNodesToExpand);
                if (!node.isExpanded)
                {
                    if (_activeNodes.Contains(node))
                    {
                        activeNodesToExpand.Add(node);
                    }
                    else
                    {
                        node._expanded = true;
                    }
                }
            }
        }
    }

    public virtual void collapseAll()
    {
        var activeNodesToCollapse = new List<TreeSliverNode<T>>();
        _collapseAll(widget.tree, activeNodesToCollapse);
        Enumerable.Reverse(activeNodesToCollapse).forEach((__arg0) => ((global::System.Action<TreeSliverNode<T>>)toggleNode)(__arg0));
    }

    internal virtual void _collapseAll(List<TreeSliverNode<T>> tree, List<TreeSliverNode<T>> activeNodesToCollapse)
    {
        foreach (var node in tree)
        {
            if (Enumerable.Any(node.children))
            {
                _collapseAll(node.children, activeNodesToCollapse);
                if (node.isExpanded)
                {
                    if (_activeNodes.Contains(node))
                    {
                        activeNodesToCollapse.Add(node);
                    }
                    else
                    {
                        node._expanded = false;
                    }
                }
            }
        }
    }

    internal virtual void _updateActiveAnimations()
    {
        _activeAnimations.Clear();
        foreach (TreeSliverNode<T> node in _currentAnimationForParent.Keys)
        {
            (global::Doroti.Framework.Animation.CurvedAnimation animation, global::Doroti.Framework.Animation.AnimationController controller, global::Doroti.Framework.Foundation.UniqueKey key) animationRecord = DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<(global::Doroti.Framework.Animation.CurvedAnimation animation, global::Doroti.Framework.Animation.AnimationController controller, global::Doroti.Framework.Foundation.UniqueKey key)>(_currentAnimationForParent, node));
            long leadingChildIndex = _activeNodes.IndexOf(node) + 1L;
            global::Doroti.Framework.Rendering.TreeSliverNodesAnimation animatingChildren = new global::Doroti.Framework.Rendering.TreeSliverNodesAnimation(fromIndex: leadingChildIndex, toIndex: leadingChildIndex + checked(node.children.Count) - 1L, value: animationRecord.animation.value);
            _activeAnimations[animationRecord.key] = animatingChildren;
        }
    }

    public virtual void toggleNode(TreeSliverNode<T> node)
    {
        DartRuntimePrimitives.Assert(() => _activeNodes.Contains(node));
        if (!Enumerable.Any(node.children))
        {
            return;
        }
        setState(() =>
        {
            node._expanded = !node._expanded;
            if (widget.onNodeToggle is not null)
            {
                widget.onNodeToggle!(node);
            }
            if (_currentAnimationForParent.ContainsKey(node))
            {
                DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<(global::Doroti.Framework.Animation.CurvedAnimation animation, global::Doroti.Framework.Animation.AnimationController controller, global::Doroti.Framework.Foundation.UniqueKey key)>(_currentAnimationForParent, node)).animation.dispose();
            }
            if (Equals(widget.toggleAnimationStyle, AnimationStyle.noAnimation) || Equals(widget.toggleAnimationStyle?.duration, Duration.zero))
            {
                _unpackActiveNodes();
                return;
            }
            global::Doroti.Framework.Animation.AnimationController controllerLocal = ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
            {
                var __cascade = DartCollectionRuntime.NullableMapValue<(global::Doroti.Framework.Animation.CurvedAnimation animation, global::Doroti.Framework.Animation.AnimationController controller, global::Doroti.Framework.Foundation.UniqueKey key)>(_currentAnimationForParent, node)?.controller ?? new global::Doroti.Framework.Animation.AnimationController(value: node._expanded ? 0.0 : 1.0, vsync: this, duration: widget.toggleAnimationStyle?.duration ?? TreeSliver<object>.defaultAnimationDuration);
                __cascade.addStatusListener((status) =>
                {
                    switch (status)
                    {
                        case AnimationStatus.dismissed:
                        case AnimationStatus.completed:
                            {
                                DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<(global::Doroti.Framework.Animation.CurvedAnimation animation, global::Doroti.Framework.Animation.AnimationController controller, global::Doroti.Framework.Foundation.UniqueKey key)>(_currentAnimationForParent, node)).animation.dispose();
                                DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<(global::Doroti.Framework.Animation.CurvedAnimation animation, global::Doroti.Framework.Animation.AnimationController controller, global::Doroti.Framework.Foundation.UniqueKey key)>(_currentAnimationForParent, node)).controller.dispose();
                                _currentAnimationForParent.remove(node);
                                _updateActiveAnimations();
                                if (!node._expanded)
                                {
                                    _unpackActiveNodes();
                                }
                                break;
                            }
                        case AnimationStatus.forward:
                        case AnimationStatus.reverse:
                            break;
                    }
                });
                __cascade.addListener(() =>
                {
                    setState(() =>
                    {
                        _updateActiveAnimations();
                    });
                });
                return __cascade;
            }))();
            switch (controllerLocal.status)
            {
                case AnimationStatus.forward:
                case AnimationStatus.reverse:
                    {
                        controllerLocal.stop();
                        break;
                    }
                case AnimationStatus.dismissed:
                case AnimationStatus.completed:
                    break;
            }
            var newAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: controllerLocal, curve: widget.toggleAnimationStyle?.curve ?? TreeSliver<object>.defaultAnimationCurve);
            _currentAnimationForParent[node] = (animation: newAnimation, controller: controllerLocal, key: new global::Doroti.Framework.Foundation.UniqueKey());
            switch (node._expanded)
            {
                case true:
                    {
                        _unpackActiveNodes();
                        controllerLocal.forward();
                        break;
                    }
                case false:
                    {
                        controllerLocal.reverse();
                        break;
                    }
            }
        });
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = ((Func<_WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider(onTick, this, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
    __cascade.muted = !values.enabled;
    __cascade.forceFrames = values.forceFrames;
    return __cascade;
}))();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", _tickers, description: (_tickers is not null) ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}" : null, defaultValue: default));
    }

}

internal class _TreeNodeParentDataWidget__sliver_tree : ParentDataWidget<global::Doroti.Framework.Rendering.TreeSliverNodeParentData>
{
    public virtual long depth { get; private set; } = default!;

    internal _TreeNodeParentDataWidget__sliver_tree(long depth, Widget child) : base(child: child)
    {
        this.depth = depth;
        System.Diagnostics.Debug.Assert(depth >= 0L);
    }

    public override void applyParentData(global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var parentDataLocal = ((global::Doroti.Framework.Rendering.TreeSliverNodeParentData?)renderObject.parentData!)!;
        var needsLayout = false;
        if (parentDataLocal.depth != depth)
        {
            DartRuntimePrimitives.Assert(() => depth >= 0L);
            parentDataLocal.depth = depth;
            needsLayout = true;
        }
        if (needsLayout)
        {
            renderObject.parent?.markNeedsLayout();
        }
    }

    public override Type debugTypicalAncestorWidgetClass => typeof(_SliverTree__sliver_tree);
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("depth", depth));
    }

}

internal class _SliverTree__sliver_tree : SliverVariedExtentList
{
    public virtual DartMap<global::Doroti.Framework.Foundation.UniqueKey, global::Doroti.Framework.Rendering.TreeSliverNodesAnimation> activeAnimations { get; private set; } = default!;
    public virtual double indentation { get; private set; } = default!;

    internal _SliverTree__sliver_tree(global::System.Func<BuildContext, long, Widget?> itemBuilder, ItemExtentBuilder itemExtentBuilder, DartMap<global::Doroti.Framework.Foundation.UniqueKey, global::Doroti.Framework.Rendering.TreeSliverNodesAnimation> activeAnimations, double indentation, global::System.Func<global::Doroti.Framework.Foundation.Key, long?>? findChildIndexCallback = null, long itemCount = default!, bool addAutomaticKeepAlives = true) : base(itemExtentBuilder: itemExtentBuilder, @delegate: new SliverChildBuilderDelegate(itemBuilder, findChildIndexCallback: findChildIndexCallback, childCount: itemCount, addAutomaticKeepAlives: addAutomaticKeepAlives, addRepaintBoundaries: false, addSemanticIndexes: false))
    {
        this.activeAnimations = activeAnimations;
        this.indentation = indentation;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        var element = ((SliverMultiBoxAdaptorElement?)context)!;
        return new global::Doroti.Framework.Rendering.RenderTreeSliver(itemExtentBuilder: itemExtentBuilder, activeAnimations: activeAnimations, indentation: indentation, childManager: element);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderTreeSliver)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.RenderTreeSliver>)(() =>
{
    var __cascade = __renderObject;
    __cascade.itemExtentBuilder = itemExtentBuilder;
    __cascade.activeAnimations = activeAnimations;
    __cascade.indentation = indentation;
    return __cascade;
}))());
    }

}

