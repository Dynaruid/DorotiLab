// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/focus_traversal.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public static partial class Focus_traversalLibrary
{
    internal static BuildContext? _getAncestor(BuildContext context, long count = 1)
    {
        BuildContext? target = default!;
        context.visitAncestorElements(
            (ancestor) =>
            {
                count--;
                if (count == 0L)
                {
                    target = DartRuntimePrimitives.ConvertValue<BuildContext>(ancestor);
                    return false;
                }
                return true;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        return target;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public delegate void TraversalRequestFocusCallback(
    FocusNode node,
    ScrollPositionAlignmentPolicy? alignmentPolicy = null,
    double? alignment = null,
    Duration? duration = null,
    Curve? curve = null
);

internal class _FocusTraversalGroupInfo__focus_traversal
{
    public virtual FocusNode? groupNode { get; private set; }
    public virtual FocusTraversalPolicy policy { get; private set; } = default!;
    public virtual List<FocusNode> members { get; private set; } = default!;

    internal _FocusTraversalGroupInfo__focus_traversal(
        _FocusTraversalGroupNode__focus_traversal? group,
        FocusTraversalPolicy? defaultPolicy = null,
        List<FocusNode>? members = null
    )
    {
        groupNode = group;
        policy = (group?.policy ?? defaultPolicy) ?? new ReadingOrderTraversalPolicy();
        this.members = members ?? new List<FocusNode>();
    }
}

public enum TraversalDirection
{
    up,
    right,
    down,
    left,
}

public enum TraversalEdgeBehavior
{
    closedLoop,
    leaveDorotiView,
    parentScope,
    stop,
}

public abstract class FocusTraversalPolicy : Diagnosticable
{
    public virtual TraversalRequestFocusCallback requestFocusCallback { get; private set; } =
        default!;

    protected FocusTraversalPolicy(TraversalRequestFocusCallback? requestFocusCallback = null)
    {
        this.requestFocusCallback = requestFocusCallback ?? defaultTraversalRequestFocusCallback;
    }

    public static void defaultTraversalRequestFocusCallback(
        FocusNode node,
        ScrollPositionAlignmentPolicy? alignmentPolicy = null,
        double? alignment = null,
        Duration? duration = null,
        Curve? curve = null
    )
    {
        node.requestFocus();
        DartRuntimePrimitives.Ignore(
            Scrollable.ensureVisible(
                node.context!,
                alignment: alignment ?? 1,
                alignmentPolicy: alignmentPolicy ?? ScrollPositionAlignmentPolicy.@explicit,
                duration: duration ?? Duration.zero,
                curve: curve ?? Curves.ease
            )
        );
    }

    internal virtual bool _requestTabTraversalFocus(
        FocusNode node,
        ScrollPositionAlignmentPolicy? alignmentPolicy = null,
        double? alignment = null,
        Duration? duration = null,
        Curve? curve = null,
        bool forward = default!
    )
    {
        if (node is FocusScopeNode)
        {
            FocusScopeNode node__as9364 = (FocusScopeNode)node;
            if (node__as9364.focusedChild is not null)
            {
                return _requestTabTraversalFocus(
                    node__as9364.focusedChild!,
                    alignmentPolicy: alignmentPolicy,
                    alignment: alignment,
                    duration: duration,
                    curve: curve,
                    forward: forward
                );
            }
            List<FocusNode> sortedChildren = _sortAllDescendants(node__as9364, node__as9364);
            if (Enumerable.Any(sortedChildren))
            {
                _requestTabTraversalFocus(
                    forward ? sortedChildren.First() : sortedChildren.Last(),
                    alignmentPolicy: alignmentPolicy,
                    alignment: alignment,
                    duration: duration,
                    curve: curve,
                    forward: forward
                );
                return true;
            }
        }
        bool nodeHadPrimaryFocus = node.hasPrimaryFocus;
        requestFocusCallback(
            node,
            alignmentPolicy: alignmentPolicy,
            alignment: alignment,
            duration: duration,
            curve: curve
        );
        return !nodeHadPrimaryFocus;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual FocusNode? findFirstFocus(FocusNode currentNode, bool ignoreCurrentFocus = false)
    {
        return (FocusNode?)_findInitialFocus(currentNode, ignoreCurrentFocus: ignoreCurrentFocus);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual FocusNode findLastFocus(FocusNode currentNode, bool ignoreCurrentFocus = false)
    {
        return _findInitialFocus(
            currentNode,
            fromEnd: true,
            ignoreCurrentFocus: ignoreCurrentFocus
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual FocusNode _findInitialFocus(
        FocusNode currentNode,
        bool fromEnd = false,
        bool ignoreCurrentFocus = false
    )
    {
        FocusScopeNode scope = currentNode.nearestScope!;
        FocusNode? candidate = scope.focusedChild;
        if (ignoreCurrentFocus || ((candidate is null) && Enumerable.Any(scope.descendants)))
        {
            IEnumerable<FocusNode> sorted = _sortAllDescendants(scope, currentNode)
                .where((node) => _canRequestTraversalFocus(node));
            if (!Enumerable.Any(sorted))
            {
                candidate = null;
            }
            else
            {
                candidate = fromEnd ? sorted.Last() : sorted.First();
            }
        }
        candidate ??= currentNode;
        return candidate;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public abstract FocusNode? findFirstFocusInDirection(
        FocusNode currentNode,
        TraversalDirection direction
    );

    public virtual void invalidateScopeData(FocusScopeNode node) { }

    public virtual void changedScope(FocusNode? node = null, FocusScopeNode? oldScope = null) { }

    public virtual bool next(FocusNode currentNode) => _moveFocus(currentNode, forward: true);

    public virtual bool previous(FocusNode currentNode) => _moveFocus(currentNode, forward: false);

    public abstract bool inDirection(FocusNode currentNode, TraversalDirection direction);
    public abstract IEnumerable<FocusNode> sortDescendants(
        IEnumerable<FocusNode> descendants,
        FocusNode currentNode
    );

    internal static bool _canRequestTraversalFocus(FocusNode node)
    {
        return node.canRequestFocus && !node.skipTraversal;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static IEnumerable<FocusNode> _getDescendantsWithoutExpandingScope(FocusNode node)
    {
        var result = new List<FocusNode>();
        foreach (FocusNode child in node.children)
        {
            // Do not sort a cached subtree (including its traversal groups)
            // whose replacement render children have not been laid out yet.
            if (child._isInKeptAliveSliver)
            {
                continue;
            }

            result.Add(child);
            if (child is not FocusScopeNode)
            {
                result.AddRange(_getDescendantsWithoutExpandingScope(child));
            }
        }
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static DartMap<FocusNode?, _FocusTraversalGroupInfo__focus_traversal> _findGroups(
        FocusScopeNode scope,
        _FocusTraversalGroupNode__focus_traversal? scopeGroupNode,
        FocusNode currentNode
    )
    {
        FocusTraversalPolicy defaultPolicyLocal =
            scopeGroupNode?.policy ?? new ReadingOrderTraversalPolicy();
        var groups = new DartMap<FocusNode?, _FocusTraversalGroupInfo__focus_traversal>();
        foreach (FocusNode node in _getDescendantsWithoutExpandingScope(scope))
        {
            _FocusTraversalGroupNode__focus_traversal? groupNode =
                FocusTraversalGroup._getGroupNode(node);
            if (Equals(node, groupNode))
            {
                _FocusTraversalGroupNode__focus_traversal? parentGroup =
                    FocusTraversalGroup._getGroupNode(groupNode!.parent!);
                groups.putIfAbsent(
                    parentGroup,
                    () =>
                        new _FocusTraversalGroupInfo__focus_traversal(
                            parentGroup,
                            members: new List<FocusNode>(),
                            defaultPolicy: defaultPolicyLocal
                        )
                );
                DartRuntimePrimitives.Assert(() => !groups[parentGroup].members.Contains(node));
                groups[parentGroup].members.Add(groupNode);
                continue;
            }
            if (Equals(node, currentNode) || (node.canRequestFocus && !node.skipTraversal))
            {
                groups.putIfAbsent(
                    groupNode,
                    () =>
                        new _FocusTraversalGroupInfo__focus_traversal(
                            groupNode,
                            members: new List<FocusNode>(),
                            defaultPolicy: defaultPolicyLocal
                        )
                );
                DartRuntimePrimitives.Assert(() => !groups[groupNode].members.Contains(node));
                groups[groupNode].members.Add(node);
            }
        }
        return groups;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static List<FocusNode> _sortAllDescendants(FocusScopeNode scope, FocusNode currentNode)
    {
        _FocusTraversalGroupNode__focus_traversal? scopeGroupNode =
            FocusTraversalGroup._getGroupNode(scope);
        DartMap<FocusNode?, _FocusTraversalGroupInfo__focus_traversal> groups = _findGroups(
            scope,
            scopeGroupNode,
            currentNode
        );
        foreach (FocusNode? key in groups.Keys)
        {
            List<FocusNode> sortedMembers = groups[key]
                .policy.sortDescendants(groups[key].members.Cast<FocusNode>(), currentNode)
                .ToList()
                .ToList();
            groups[key].members.Clear();
            groups[key].members.AddRange(sortedMembers.Cast<FocusNode>());
        }
        var sortedDescendants = new List<FocusNode>();
        void visitGroups(_FocusTraversalGroupInfo__focus_traversal info)
        {
            foreach (FocusNode nodeLocal in info.members)
            {
                if (groups.ContainsKey(nodeLocal))
                {
                    visitGroups(groups.GetValueOrDefault(nodeLocal)!);
                }
                else
                {
                    sortedDescendants.Add(nodeLocal);
                }
            }
        }
        if (Enumerable.Any(groups) && groups.ContainsKey(scopeGroupNode))
        {
            visitGroups(groups[scopeGroupNode]);
        }
        sortedDescendants.removeWhere(
            (node) =>
            {
                return (!Equals(node, currentNode)) && !_canRequestTraversalFocus(node);
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        DartRuntimePrimitives.Assert(() =>
        {
            HashSet<FocusNode> differenceLocal = sortedDescendants
                .toSet()
                .difference(scope.traversalDescendants.toSet());
            if (!_canRequestTraversalFocus(currentNode))
            {
                DartRuntimePrimitives.Assert(
                    () =>
                        !Enumerable.Any(differenceLocal)
                        || (
                            (checked(differenceLocal.Count) == 1L)
                            && differenceLocal.Contains(currentNode)
                        ),
                    () =>
                        (object?)
                            "Difference between sorted descendants and FocusScopeNode.traversalDescendants contains "
                        + $"something other than the current skipped node. This is the difference: {differenceLocal}"
                );
                return true;
            }
            DartRuntimePrimitives.Assert(
                () => !Enumerable.Any(differenceLocal),
                () =>
                    (object?)
                        "Sorted descendants contains different nodes than FocusScopeNode.traversalDescendants would. "
                    + $"These are the different nodes: {differenceLocal}"
            );
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return sortedDescendants;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual bool _moveFocus(FocusNode currentNode, bool forward)
    {
        FocusScopeNode nearestScopeLocal = currentNode.nearestScope!;
        invalidateScopeData(nearestScopeLocal);
        FocusNode? focusedChildLocal = nearestScopeLocal.focusedChild;
        if (focusedChildLocal is null)
        {
            FocusNode? firstFocus = forward
                ? findFirstFocus(currentNode)
                : findLastFocus(currentNode);
            if (firstFocus is not null)
            {
                return _requestTabTraversalFocus(
                    firstFocus,
                    alignmentPolicy: forward
                        ? ScrollPositionAlignmentPolicy.keepVisibleAtEnd
                        : ScrollPositionAlignmentPolicy.keepVisibleAtStart,
                    forward: forward
                );
            }
        }
        focusedChildLocal ??= nearestScopeLocal;
        List<FocusNode> sortedNodes = _sortAllDescendants(nearestScopeLocal, focusedChildLocal);
        DartRuntimePrimitives.Assert(() => sortedNodes.Contains(focusedChildLocal));
        if (forward && Equals(focusedChildLocal, sortedNodes.Last()))
        {
            switch (nearestScopeLocal.traversalEdgeBehavior)
            {
                case TraversalEdgeBehavior.leaveDorotiView:
                {
                    focusedChildLocal.unfocus();
                    return false;
                }
                case TraversalEdgeBehavior.parentScope:
                {
                    FocusScopeNode? parentScopeLocal = nearestScopeLocal.enclosingScope;
                    if (
                        (parentScopeLocal is not null)
                        && (!Equals(parentScopeLocal, FocusManager.instance.rootScope))
                    )
                    {
                        focusedChildLocal.unfocus();
                        parentScopeLocal.nextFocus();
                        return !Equals(
                            focusedChildLocal.enclosingScope?.focusedChild,
                            focusedChildLocal
                        );
                    }
                    return _requestTabTraversalFocus(
                        sortedNodes.First(),
                        alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd,
                        forward: forward
                    );
                }
                case TraversalEdgeBehavior.closedLoop:
                {
                    return _requestTabTraversalFocus(
                        sortedNodes.First(),
                        alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd,
                        forward: forward
                    );
                }
                case TraversalEdgeBehavior.stop:
                {
                    return false;
                }
                default:
                    throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    );
            }
        }
        if (!forward && Equals(focusedChildLocal, sortedNodes.First()))
        {
            switch (nearestScopeLocal.traversalEdgeBehavior)
            {
                case TraversalEdgeBehavior.leaveDorotiView:
                {
                    focusedChildLocal.unfocus();
                    return false;
                }
                case TraversalEdgeBehavior.parentScope:
                {
                    FocusScopeNode? parentScopeAlternate = nearestScopeLocal.enclosingScope;
                    if (
                        (parentScopeAlternate is not null)
                        && (!Equals(parentScopeAlternate, FocusManager.instance.rootScope))
                    )
                    {
                        focusedChildLocal.unfocus();
                        parentScopeAlternate.previousFocus();
                        return !Equals(
                            focusedChildLocal.enclosingScope?.focusedChild,
                            focusedChildLocal
                        );
                    }
                    return _requestTabTraversalFocus(
                        sortedNodes.Last(),
                        alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart,
                        forward: forward
                    );
                }
                case TraversalEdgeBehavior.closedLoop:
                {
                    return _requestTabTraversalFocus(
                        sortedNodes.Last(),
                        alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart,
                        forward: forward
                    );
                }
                case TraversalEdgeBehavior.stop:
                {
                    return false;
                }
                default:
                    throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    );
            }
        }
        IEnumerable<FocusNode> maybeFlipped = forward
            ? sortedNodes
            : Enumerable.Reverse(sortedNodes);
        FocusNode? previousNode = default!;
        foreach (var node in maybeFlipped)
        {
            if (Equals(previousNode, focusedChildLocal))
            {
                return _requestTabTraversalFocus(
                    node,
                    alignmentPolicy: forward
                        ? ScrollPositionAlignmentPolicy.keepVisibleAtEnd
                        : ScrollPositionAlignmentPolicy.keepVisibleAtStart,
                    forward: forward
                );
            }
            previousNode = node;
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);

    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine)
                .toDiagnosticsNode()
                .toStringDeep(minLevel: minLevel);
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = null
    )
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties) { }
}

public class _DirectionalPolicyDataEntry__focus_traversal
{
    public virtual TraversalDirection direction { get; private set; } = default!;
    public virtual FocusNode node { get; private set; } = default!;

    internal _DirectionalPolicyDataEntry__focus_traversal(
        TraversalDirection direction,
        FocusNode node
    )
    {
        this.direction = direction;
        this.node = node;
    }
}

public class _DirectionalPolicyData__focus_traversal
{
    public virtual List<_DirectionalPolicyDataEntry__focus_traversal> history
    {
        get;
        private set;
    } = default!;

    internal _DirectionalPolicyData__focus_traversal(
        List<_DirectionalPolicyDataEntry__focus_traversal> history
    )
    {
        this.history = history;
    }
}

public interface DirectionalFocusTraversalPolicyMixin
{
    DartMap<FocusScopeNode, _DirectionalPolicyData__focus_traversal> _policyData { get; }

    public void invalidateScopeData(FocusScopeNode node);
    public void changedScope(FocusNode? node = null, FocusScopeNode? oldScope = null);
    public FocusNode? findFirstFocusInDirection(
        FocusNode currentNode,
        TraversalDirection direction
    );
    public FocusNode? _findNextFocusInDirection(
        FocusNode focusedChild,
        IEnumerable<FocusNode> traversalDescendants,
        TraversalDirection direction,
        bool forward = true
    );
    public static long _verticalCompare(Offset target, Offset a, Offset b)
    {
        return (a.dy - target.dy).abs().CompareTo((b.dy - target.dy).abs());
    }
    public static long _horizontalCompare(Offset target, Offset a, Offset b)
    {
        return (a.dx - target.dx).abs().CompareTo((b.dx - target.dx).abs());
    }
    public static IEnumerable<FocusNode> _sortByDistancePreferVertical(
        Offset target,
        IEnumerable<FocusNode> nodes
    )
    {
        List<FocusNode> sorted = nodes.ToList().ToList();
        CollectionsLibrary.mergeSort(
            sorted,
            compare: (nodeA, nodeB) =>
            {
                Offset a = nodeA.rect.center;
                Offset b = nodeB.rect.center;
                long vertical = _verticalCompare(target, a, b);
                if (vertical == 0L)
                {
                    return _horizontalCompare(target, a, b);
                }
                return vertical;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        return sorted;
    }
    public static IEnumerable<FocusNode> _sortByDistancePreferHorizontal(
        Offset target,
        IEnumerable<FocusNode> nodes
    )
    {
        List<FocusNode> sorted = nodes.ToList().ToList();
        CollectionsLibrary.mergeSort(
            sorted,
            compare: (nodeA, nodeB) =>
            {
                Offset a = nodeA.rect.center;
                Offset b = nodeB.rect.center;
                long horizontal = _horizontalCompare(target, a, b);
                if (horizontal == 0L)
                {
                    return _verticalCompare(target, a, b);
                }
                return horizontal;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        return sorted;
    }
    public static long _verticalCompareClosestEdge(Offset target, Rect a, Rect b)
    {
        double aCoord =
            ((a.top - target.dy).abs() < (a.bottom - target.dy).abs()) ? a.top : a.bottom;
        double bCoord =
            ((b.top - target.dy).abs() < (b.bottom - target.dy).abs()) ? b.top : b.bottom;
        return (aCoord - target.dy).abs().CompareTo((bCoord - target.dy).abs());
    }
    public static long _horizontalCompareClosestEdge(Offset target, Rect a, Rect b)
    {
        double aCoord =
            ((a.left - target.dx).abs() < (a.right - target.dx).abs()) ? a.left : a.right;
        double bCoord =
            ((b.left - target.dx).abs() < (b.right - target.dx).abs()) ? b.left : b.right;
        return (aCoord - target.dx).abs().CompareTo((bCoord - target.dx).abs());
    }
    public static IEnumerable<FocusNode> _sortClosestEdgesByDistancePreferHorizontal(
        Offset target,
        IEnumerable<FocusNode> nodes
    )
    {
        List<FocusNode> sorted = nodes.ToList().ToList();
        CollectionsLibrary.mergeSort(
            sorted,
            compare: (nodeA, nodeB) =>
            {
                long horizontal = _horizontalCompareClosestEdge(target, nodeA.rect, nodeB.rect);
                if (horizontal == 0L)
                {
                    return _verticalCompare(target, nodeA.rect.center, nodeB.rect.center);
                }
                return horizontal;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        return sorted;
    }
    public static IEnumerable<FocusNode> _sortClosestEdgesByDistancePreferVertical(
        Offset target,
        IEnumerable<FocusNode> nodes
    )
    {
        List<FocusNode> sorted = nodes.ToList().ToList();
        CollectionsLibrary.mergeSort(
            sorted,
            compare: (nodeA, nodeB) =>
            {
                long vertical = _verticalCompareClosestEdge(target, nodeA.rect, nodeB.rect);
                if (vertical == 0L)
                {
                    return _horizontalCompare(target, nodeA.rect.center, nodeB.rect.center);
                }
                return vertical;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        return sorted;
    }
    public IEnumerable<FocusNode> _sortAndFilterHorizontally(
        TraversalDirection direction,
        Rect target,
        IEnumerable<FocusNode> nodes,
        bool forward = true
    );
    public IEnumerable<FocusNode> _sortAndFilterVertically(
        TraversalDirection direction,
        Rect target,
        IEnumerable<FocusNode> nodes,
        bool forward = true
    );
    public bool _popPolicyDataIfNeeded(
        TraversalDirection direction,
        FocusScopeNode nearestScope,
        FocusNode focusedChild,
        _FocusTraversalGroupNode__focus_traversal? groupNode
    );
    public void _pushPolicyData(
        TraversalDirection direction,
        FocusScopeNode nearestScope,
        FocusNode focusedChild
    );
    public bool _requestTraversalFocusInDirection(
        FocusNode currentNode,
        FocusNode node,
        FocusScopeNode nearestScope,
        TraversalDirection direction,
        _FocusTraversalGroupNode__focus_traversal? groupNode
    );
    public void _requestFocus(
        FocusNode node,
        _FocusTraversalGroupNode__focus_traversal? groupNode,
        ScrollPositionAlignmentPolicy? alignmentPolicy = null,
        double? alignment = null,
        Duration? duration = null,
        Curve? curve = null
    );
    public bool _onEdgeForDirection(
        FocusNode currentNode,
        FocusNode focusedChild,
        _FocusTraversalGroupNode__focus_traversal? groupNode,
        TraversalDirection direction,
        FocusScopeNode? scope = null
    );
    public bool inDirection(FocusNode currentNode, TraversalDirection direction);
}

public class WidgetOrderTraversalPolicy : FocusTraversalPolicy, DirectionalFocusTraversalPolicyMixin
{
    public virtual DartMap<
        FocusScopeNode,
        _DirectionalPolicyData__focus_traversal
    > _policyData { get; set; } =
        new DartMap<FocusScopeNode, _DirectionalPolicyData__focus_traversal>();

    public WidgetOrderTraversalPolicy(TraversalRequestFocusCallback? requestFocusCallback = null)
        : base(requestFocusCallback: requestFocusCallback) { }

    public override IEnumerable<FocusNode> sortDescendants(
        IEnumerable<FocusNode> descendants,
        FocusNode currentNode
    ) => descendants;

    public override void invalidateScopeData(FocusScopeNode node)
    {
        base.invalidateScopeData(node);
        _policyData.remove(node);
    }

    public override void changedScope(FocusNode? node = null, FocusScopeNode? oldScope = null)
    {
        base.changedScope(node: node, oldScope: oldScope);
        if (oldScope is not null)
        {
            _policyData
                .GetValueOrDefault(oldScope)
                ?.history.removeWhere(
                    (entry) =>
                    {
                        return Equals(entry.node, node);
                        throw new InvalidOperationException(
                            "Callback completed without returning a value."
                        );
                    }
                );
        }
    }

    public override FocusNode? findFirstFocusInDirection(
        FocusNode currentNode,
        TraversalDirection direction
    )
    {
        IEnumerable<FocusNode> nodes = currentNode.nearestScope!.traversalDescendants;
        List<FocusNode> sorted = nodes.ToList().ToList();
        var (vertical, first) = direction switch
        {
            TraversalDirection.up => (true, false),
            TraversalDirection.down => (true, true),
            TraversalDirection.left => (false, false),
            TraversalDirection.right => (false, true),
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        CollectionsLibrary.mergeSort(
            sorted,
            compare: (a, b) =>
            {
                if (vertical)
                {
                    if (first)
                    {
                        return a.rect.top.CompareTo(b.rect.top);
                    }
                    else
                    {
                        return b.rect.bottom.CompareTo(a.rect.bottom);
                    }
                }
                else
                {
                    if (first)
                    {
                        return a.rect.left.CompareTo(b.rect.left);
                    }
                    else
                    {
                        return b.rect.right.CompareTo(a.rect.right);
                    }
                }
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        return sorted.FirstOrDefault();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual FocusNode? _findNextFocusInDirection(
        FocusNode focusedChild,
        IEnumerable<FocusNode> traversalDescendants,
        TraversalDirection direction,
        bool forward = true
    )
    {
        switch (direction)
        {
            case TraversalDirection.down:
            case TraversalDirection.up:
            {
                IEnumerable<FocusNode> eligibleNodes = _sortAndFilterVertically(
                    direction,
                    focusedChild.rect,
                    traversalDescendants.Cast<FocusNode>(),
                    forward: forward
                );
                if (!Enumerable.Any(eligibleNodes))
                {
                    break;
                }
                ScrollableState? focusedScrollable = Scrollable.maybeOf(
                    focusedChild.context!,
                    axis: Axis.vertical
                );
                if (focusedScrollable is not null)
                {
                    IEnumerable<FocusNode> filteredEligibleNodes = eligibleNodes.where(
                        (node) =>
                            Equals(
                                Scrollable.maybeOf(node.context!, axis: Axis.vertical),
                                focusedScrollable
                            )
                    );
                    if (Enumerable.Any(filteredEligibleNodes))
                    {
                        eligibleNodes = filteredEligibleNodes;
                    }
                }
                if (Equals(direction, TraversalDirection.up))
                {
                    eligibleNodes = Enumerable.Reverse(eligibleNodes.ToList());
                }
                var band = Rect.fromLTRB(
                    focusedChild.rect.left,
                    -double.PositiveInfinity,
                    focusedChild.rect.right,
                    double.PositiveInfinity
                );
                IEnumerable<FocusNode> inBand = eligibleNodes.where(
                    (node) => !node.rect.intersect(band).isEmpty
                );
                if (Enumerable.Any(inBand))
                {
                    if (forward)
                    {
                        return DirectionalFocusTraversalPolicyMixin
                            ._sortByDistancePreferVertical(
                                focusedChild.rect.center,
                                inBand.Cast<FocusNode>()
                            )
                            .First();
                    }
                    return DirectionalFocusTraversalPolicyMixin
                        ._sortByDistancePreferVertical(
                            focusedChild.rect.center,
                            inBand.Cast<FocusNode>()
                        )
                        .Last();
                }
                if (forward)
                {
                    return DirectionalFocusTraversalPolicyMixin
                        ._sortClosestEdgesByDistancePreferHorizontal(
                            focusedChild.rect.center,
                            eligibleNodes.Cast<FocusNode>()
                        )
                        .First();
                }
                return DirectionalFocusTraversalPolicyMixin
                    ._sortClosestEdgesByDistancePreferHorizontal(
                        focusedChild.rect.center,
                        eligibleNodes.Cast<FocusNode>()
                    )
                    .Last();
            }
            case TraversalDirection.right:
            case TraversalDirection.left:
            {
                IEnumerable<FocusNode> eligibleNodesLocal = _sortAndFilterHorizontally(
                    direction,
                    focusedChild.rect,
                    traversalDescendants.Cast<FocusNode>(),
                    forward: forward
                );
                if (!Enumerable.Any(eligibleNodesLocal))
                {
                    break;
                }
                ScrollableState? focusedScrollableLocal = Scrollable.maybeOf(
                    focusedChild.context!,
                    axis: Axis.horizontal
                );
                if (focusedScrollableLocal is not null)
                {
                    IEnumerable<FocusNode> filteredEligibleNodesLocal = eligibleNodesLocal.where(
                        (node) =>
                            Equals(
                                Scrollable.maybeOf(node.context!, axis: Axis.horizontal),
                                focusedScrollableLocal
                            )
                    );
                    if (Enumerable.Any(filteredEligibleNodesLocal))
                    {
                        eligibleNodesLocal = filteredEligibleNodesLocal;
                    }
                }
                if (Equals(direction, TraversalDirection.left))
                {
                    eligibleNodesLocal = Enumerable.Reverse(eligibleNodesLocal.ToList());
                }
                var bandLocal = Rect.fromLTRB(
                    -double.PositiveInfinity,
                    focusedChild.rect.top,
                    double.PositiveInfinity,
                    focusedChild.rect.bottom
                );
                IEnumerable<FocusNode> inBandLocal = eligibleNodesLocal.where(
                    (node) => !node.rect.intersect(bandLocal).isEmpty
                );
                if (Enumerable.Any(inBandLocal))
                {
                    if (forward)
                    {
                        return DirectionalFocusTraversalPolicyMixin
                            ._sortByDistancePreferHorizontal(
                                focusedChild.rect.center,
                                inBandLocal.Cast<FocusNode>()
                            )
                            .First();
                    }
                    return DirectionalFocusTraversalPolicyMixin
                        ._sortByDistancePreferHorizontal(
                            focusedChild.rect.center,
                            inBandLocal.Cast<FocusNode>()
                        )
                        .Last();
                }
                if (forward)
                {
                    return DirectionalFocusTraversalPolicyMixin
                        ._sortClosestEdgesByDistancePreferVertical(
                            focusedChild.rect.center,
                            eligibleNodesLocal.Cast<FocusNode>()
                        )
                        .First();
                }
                return DirectionalFocusTraversalPolicyMixin
                    ._sortClosestEdgesByDistancePreferVertical(
                        focusedChild.rect.center,
                        eligibleNodesLocal.Cast<FocusNode>()
                    )
                    .Last();
            }
            default:
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                );
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual IEnumerable<FocusNode> _sortAndFilterHorizontally(
        TraversalDirection direction,
        Rect target,
        IEnumerable<FocusNode> nodes,
        bool forward = true
    )
    {
        DartRuntimePrimitives.Assert(() =>
            Equals(direction, TraversalDirection.left)
            || Equals(direction, TraversalDirection.right)
        );
        List<FocusNode> sorted = nodes
            .where(
                direction switch
                {
                    TraversalDirection.left => (node) =>
                        (!Equals(node.rect, target))
                        && (
                            forward
                                ? (node.rect.center.dx <= target.left)
                                : (node.rect.center.dx >= target.left)
                        ),
                    TraversalDirection.right => (node) =>
                        (!Equals(node.rect, target))
                        && (
                            forward
                                ? (node.rect.center.dx >= target.right)
                                : (node.rect.center.dx <= target.right)
                        ),
                    TraversalDirection.up => throw DartRuntimePrimitives.AsException(
                        new DartArgumentError($"Invalid direction {direction}")
                    ),
                    TraversalDirection.down => throw DartRuntimePrimitives.AsException(
                        new DartArgumentError($"Invalid direction {direction}")
                    ),
                    _ => throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
                }
            )
            .ToList()
            .ToList();
        CollectionsLibrary.mergeSort(
            sorted,
            compare: (a, b) => a.rect.center.dx.CompareTo(b.rect.center.dx)
        );
        return sorted;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual IEnumerable<FocusNode> _sortAndFilterVertically(
        TraversalDirection direction,
        Rect target,
        IEnumerable<FocusNode> nodes,
        bool forward = true
    )
    {
        DartRuntimePrimitives.Assert(() =>
            Equals(direction, TraversalDirection.up) || Equals(direction, TraversalDirection.down)
        );
        List<FocusNode> sorted = nodes
            .where(
                direction switch
                {
                    TraversalDirection.up => (node) =>
                        (!Equals(node.rect, target))
                        && (
                            forward
                                ? (node.rect.center.dy <= target.top)
                                : (node.rect.center.dy >= target.top)
                        ),
                    TraversalDirection.down => (node) =>
                        (!Equals(node.rect, target))
                        && (
                            forward
                                ? (node.rect.center.dy >= target.bottom)
                                : (node.rect.center.dy <= target.bottom)
                        ),
                    TraversalDirection.left => throw DartRuntimePrimitives.AsException(
                        new DartArgumentError($"Invalid direction {direction}")
                    ),
                    TraversalDirection.right => throw DartRuntimePrimitives.AsException(
                        new DartArgumentError($"Invalid direction {direction}")
                    ),
                    _ => throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
                }
            )
            .ToList()
            .ToList();
        CollectionsLibrary.mergeSort(
            sorted,
            compare: (a, b) => a.rect.center.dy.CompareTo(b.rect.center.dy)
        );
        return sorted;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool _popPolicyDataIfNeeded(
        TraversalDirection direction,
        FocusScopeNode nearestScope,
        FocusNode focusedChild,
        _FocusTraversalGroupNode__focus_traversal? groupNode
    )
    {
        _DirectionalPolicyData__focus_traversal? policyData = _policyData.GetValueOrDefault(
            nearestScope
        );
        if (
            (policyData is not null)
            && Enumerable.Any(policyData.history)
            && (!Equals(policyData.history.First().direction, direction))
        )
        {
            if (policyData.history.Last().node.parent is null)
            {
                invalidateScopeData(nearestScope);
                return false;
            }
            bool popOrInvalidate(TraversalDirection direction)
            {
                FocusNode lastNode = policyData.history.removeLast().node;
                if (
                    !Equals(
                        Scrollable.maybeOf(lastNode.context!),
                        Scrollable.maybeOf(Focus_managerLibrary.primaryFocus!.context!)
                    )
                )
                {
                    invalidateScopeData(nearestScope);
                    return false;
                }
                ScrollPositionAlignmentPolicy alignmentPolicyLocal = default!;
                switch (direction)
                {
                    case TraversalDirection.up:
                    case TraversalDirection.left:
                    {
                        alignmentPolicyLocal = ScrollPositionAlignmentPolicy.keepVisibleAtStart;
                        break;
                    }
                    case TraversalDirection.right:
                    case TraversalDirection.down:
                    {
                        alignmentPolicyLocal = ScrollPositionAlignmentPolicy.keepVisibleAtEnd;
                        break;
                    }
                }
                _requestFocus(
                    lastNode,
                    alignmentPolicy: (alignmentPolicyLocal),
                    groupNode: groupNode
                );
                return true;
                throw new InvalidOperationException(
                    "Control flow completed without returning a value."
                );
            }
            switch (direction)
            {
                case TraversalDirection.down:
                case TraversalDirection.up:
                {
                    switch (policyData.history.First().direction)
                    {
                        case TraversalDirection.left:
                        case TraversalDirection.right:
                        {
                            invalidateScopeData(nearestScope);
                            break;
                        }
                        case TraversalDirection.up:
                        case TraversalDirection.down:
                        {
                            if (popOrInvalidate(direction))
                            {
                                return true;
                            }
                            break;
                        }
                    }
                    break;
                }
                case TraversalDirection.left:
                case TraversalDirection.right:
                {
                    switch (policyData.history.First().direction)
                    {
                        case TraversalDirection.left:
                        case TraversalDirection.right:
                        {
                            if (popOrInvalidate(direction))
                            {
                                return true;
                            }
                            break;
                        }
                        case TraversalDirection.up:
                        case TraversalDirection.down:
                        {
                            invalidateScopeData(nearestScope);
                            break;
                        }
                    }
                    break;
                }
            }
        }
        if ((policyData is not null) && !Enumerable.Any(policyData.history))
        {
            invalidateScopeData(nearestScope);
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void _pushPolicyData(
        TraversalDirection direction,
        FocusScopeNode nearestScope,
        FocusNode focusedChild
    )
    {
        _DirectionalPolicyData__focus_traversal? policyData = _policyData.GetValueOrDefault(
            nearestScope
        );
        var newEntry = new _DirectionalPolicyDataEntry__focus_traversal(
            node: focusedChild,
            direction: direction
        );
        if (policyData is not null)
        {
            policyData.history.Add(newEntry);
        }
        else
        {
            _policyData[nearestScope] = new _DirectionalPolicyData__focus_traversal(
                history: new List<_DirectionalPolicyDataEntry__focus_traversal> { newEntry }
            );
        }
    }

    public virtual bool _requestTraversalFocusInDirection(
        FocusNode currentNode,
        FocusNode node,
        FocusScopeNode nearestScope,
        TraversalDirection direction,
        _FocusTraversalGroupNode__focus_traversal? groupNode
    )
    {
        if (node is FocusScopeNode)
        {
            if (((FocusScopeNode)node).focusedChild is not null)
            {
                return _requestTraversalFocusInDirection(
                    currentNode,
                    ((FocusScopeNode)node).focusedChild!,
                    DartRuntimePrimitives.ConvertValue<FocusScopeNode>(node),
                    direction,
                    groupNode
                );
            }
            FocusNode firstNode = findFirstFocusInDirection(node, direction) ?? currentNode;
            switch (direction)
            {
                case TraversalDirection.up:
                case TraversalDirection.left:
                {
                    _requestFocus(
                        firstNode,
                        groupNode,
                        alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart
                    );
                    break;
                }
                case TraversalDirection.right:
                case TraversalDirection.down:
                {
                    _requestFocus(
                        firstNode,
                        groupNode,
                        alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd
                    );
                    break;
                }
            }
            return true;
        }
        bool nodeHadPrimaryFocus = node.hasPrimaryFocus;
        switch (direction)
        {
            case TraversalDirection.up:
            case TraversalDirection.left:
            {
                _requestFocus(
                    node,
                    groupNode,
                    alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart
                );
                break;
            }
            case TraversalDirection.right:
            case TraversalDirection.down:
            {
                _requestFocus(
                    node,
                    groupNode,
                    alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd
                );
                break;
            }
        }
        return !nodeHadPrimaryFocus;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void _requestFocus(
        FocusNode node,
        _FocusTraversalGroupNode__focus_traversal? groupNode,
        ScrollPositionAlignmentPolicy? alignmentPolicy = null,
        double? alignment = null,
        Duration? duration = null,
        Curve? curve = null
    )
    {
        groupNode?.lastRequestedFocus = node;
        requestFocusCallback(
            node,
            alignmentPolicy: alignmentPolicy,
            alignment: alignment,
            duration: duration,
            curve: curve
        );
    }

    public virtual bool _onEdgeForDirection(
        FocusNode currentNode,
        FocusNode focusedChild,
        _FocusTraversalGroupNode__focus_traversal? groupNode,
        TraversalDirection direction,
        FocusScopeNode? scope = null
    )
    {
        FocusScopeNode nearestScopeLocal = scope ?? currentNode.nearestScope!;
        FocusNode? found = default!;
        switch (nearestScopeLocal.directionalTraversalEdgeBehavior)
        {
            case TraversalEdgeBehavior.leaveDorotiView:
            {
                focusedChild.unfocus();
                return false;
            }
            case TraversalEdgeBehavior.parentScope:
            {
                FocusScopeNode? parentScopeLocal = nearestScopeLocal.enclosingScope;
                if (
                    (parentScopeLocal is not null)
                    && (!Equals(parentScopeLocal, FocusManager.instance.rootScope))
                )
                {
                    invalidateScopeData(nearestScopeLocal);
                    nearestScopeLocal = parentScopeLocal;
                    invalidateScopeData(nearestScopeLocal);
                    found = _findNextFocusInDirection(
                        focusedChild,
                        nearestScopeLocal.traversalDescendants.Cast<FocusNode>(),
                        direction
                    );
                    if (found is null)
                    {
                        return _onEdgeForDirection(
                            currentNode,
                            focusedChild,
                            groupNode,
                            direction,
                            scope: nearestScopeLocal
                        );
                    }
                }
                else
                {
                    found = _findNextFocusInDirection(
                        focusedChild,
                        nearestScopeLocal.traversalDescendants.Cast<FocusNode>(),
                        direction,
                        forward: false
                    );
                }
                break;
            }
            case TraversalEdgeBehavior.closedLoop:
            {
                found = _findNextFocusInDirection(
                    focusedChild,
                    nearestScopeLocal.traversalDescendants.Cast<FocusNode>(),
                    direction,
                    forward: false
                );
                break;
            }
            case TraversalEdgeBehavior.stop:
            {
                return false;
            }
        }
        if (found is not null)
        {
            return _requestTraversalFocusInDirection(
                currentNode,
                found,
                DartRuntimePrimitives.ConvertValue<FocusScopeNode>(nearestScopeLocal),
                direction,
                groupNode
            );
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool inDirection(FocusNode currentNode, TraversalDirection direction)
    {
        _FocusTraversalGroupNode__focus_traversal? groupNodeLocal =
            FocusTraversalGroup._getGroupNode(currentNode);
        FocusScopeNode nearestScopeLocal = currentNode.nearestScope!;
        FocusNode? focusedChildLocal = nearestScopeLocal.focusedChild;
        if (focusedChildLocal is null)
        {
            FocusNode firstFocus = findFirstFocusInDirection(currentNode, direction) ?? currentNode;
            switch (direction)
            {
                case TraversalDirection.up:
                case TraversalDirection.left:
                {
                    _requestFocus(
                        firstFocus,
                        alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart,
                        groupNode: groupNodeLocal
                    );
                    break;
                }
                case TraversalDirection.right:
                case TraversalDirection.down:
                {
                    _requestFocus(
                        firstFocus,
                        alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd,
                        groupNode: groupNodeLocal
                    );
                    break;
                }
            }
            return true;
        }
        if (_popPolicyDataIfNeeded(direction, nearestScopeLocal, focusedChildLocal, groupNodeLocal))
        {
            return true;
        }
        FocusNode? found = _findNextFocusInDirection(
            focusedChildLocal,
            nearestScopeLocal.traversalDescendants.Cast<FocusNode>(),
            direction
        );
        if (found is not null)
        {
            _pushPolicyData(direction, nearestScopeLocal, focusedChildLocal);
            return _requestTraversalFocusInDirection(
                currentNode,
                found,
                DartRuntimePrimitives.ConvertValue<FocusScopeNode>(nearestScopeLocal),
                direction,
                groupNodeLocal
            );
        }
        return _onEdgeForDirection(currentNode, focusedChildLocal, groupNodeLocal, direction);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class _ReadingOrderSortData__focus_traversal : Diagnosticable
{
    public virtual TextDirection? directionality { get; private set; }
    public virtual Rect rect { get; private set; } = default!;
    public virtual FocusNode node { get; private set; } = default!;
    internal virtual List<Directionality>? _directionalAncestors { get; set; } = default;

    internal _ReadingOrderSortData__focus_traversal(FocusNode node)
    {
        this.node = node;
        rect = node.rect;
        directionality = _findDirectionality(node.context!);
    }

    internal static TextDirection? _findDirectionality(BuildContext context)
    {
        return context.getInheritedWidgetOfExactType<Directionality>()?.textDirection;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static TextDirection? commonDirectionalityOf(
        List<_ReadingOrderSortData__focus_traversal> list
    )
    {
        IEnumerable<HashSet<Directionality>> allAncestors = list.map(
            (member) => member.directionalAncestors.toSet()
        );
        HashSet<Directionality>? common = default!;
        foreach (var ancestorSet in allAncestors)
        {
            common ??= ancestorSet;
            common = common.intersection(ancestorSet);
        }
        if (!Enumerable.Any(common!))
        {
            return list.First().directionality;
        }
        return list.First().directionalAncestors.firstWhere(common.Contains).textDirection;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static void sortWithDirectionality(
        List<_ReadingOrderSortData__focus_traversal> list,
        TextDirection directionality
    )
    {
        CollectionsLibrary.mergeSort(
            list,
            compare: (a, b) =>
                directionality switch
                {
                    TextDirection.ltr => a.rect.left.CompareTo(b.rect.left),
                    TextDirection.rtl => b.rect.right.CompareTo(a.rect.right),
                    _ => throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
                }
        );
    }

    public virtual IEnumerable<Directionality> directionalAncestors
    {
        get
        {
            List<Directionality> getDirectionalityAncestors(BuildContext context)
            {
                var result = new List<Directionality>();
                InheritedElement? directionalityElement =
                    context.getElementForInheritedWidgetOfExactType<Directionality>();
                while (directionalityElement is not null)
                {
                    result.Add(((Directionality?)directionalityElement.widget)!);
                    directionalityElement = Focus_traversalLibrary
                        ._getAncestor(directionalityElement)
                        ?.getElementForInheritedWidgetOfExactType<Directionality>();
                }
                return result;
                throw new InvalidOperationException(
                    "Control flow completed without returning a value."
                );
            }
            _directionalAncestors ??= getDirectionalityAncestors(node.context!);
            return _directionalAncestors!;
        }
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new DiagnosticsProperty<TextDirection>("directionality", directionality));
        properties.add(new StringProperty("name", node.debugLabel, defaultValue: null));
        properties.add(new DiagnosticsProperty<Rect>("rect", rect));
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);

    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine)
                .toDiagnosticsNode()
                .toStringDeep(minLevel: minLevel);
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = null
    )
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _ReadingOrderDirectionalGroupData__focus_traversal : Diagnosticable
{
    public virtual List<_ReadingOrderSortData__focus_traversal> members { get; private set; } =
        default!;
    internal virtual Rect? _rect { get; set; } = default;
    internal virtual List<Directionality>? _memberAncestors { get; set; } = default;

    internal _ReadingOrderDirectionalGroupData__focus_traversal(
        List<_ReadingOrderSortData__focus_traversal> members
    )
    {
        this.members = members;
    }

    public virtual TextDirection? directionality =>
        DartRuntimePrimitives.ConvertValue<TextDirection>(members.First().directionality);
    public virtual Rect rect
    {
        get
        {
            if (_rect is null)
            {
                foreach (Rect rectLocal in members.map((data) => data.rect))
                {
                    _rect ??= rectLocal;
                    _rect = (
                        _rect
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ).expandToInclude(rectLocal);
                }
            }
            return (
                _rect
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        }
    }
    public virtual List<Directionality> memberAncestors
    {
        get
        {
            if (_memberAncestors is null)
            {
                _memberAncestors = new List<Directionality>();
                foreach (_ReadingOrderSortData__focus_traversal member in members)
                {
                    _memberAncestors!.AddRange(member.directionalAncestors.Cast<Directionality>());
                }
            }
            return _memberAncestors!;
        }
    }

    public static void sortWithDirectionality(
        List<_ReadingOrderDirectionalGroupData__focus_traversal> list,
        TextDirection directionality
    )
    {
        CollectionsLibrary.mergeSort(
            list,
            compare: (a, b) =>
                directionality switch
                {
                    TextDirection.ltr => a.rect.left.CompareTo(b.rect.left),
                    TextDirection.rtl => b.rect.right.CompareTo(a.rect.right),
                    _ => throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
                }
        );
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new DiagnosticsProperty<TextDirection>("directionality", directionality));
        properties.add(new DiagnosticsProperty<Rect>("rect", rect));
        properties.add(
            new IterableProperty<string>(
                "members",
                members
                    .map(
                        (member) =>
                        {
                            return $"\"{member.node.debugLabel}\"({member.rect})";
                            throw new InvalidOperationException(
                                "Callback completed without returning a value."
                            );
                        }
                    )
                    .Cast<string>()
            )
        );
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);

    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine)
                .toDiagnosticsNode()
                .toStringDeep(minLevel: minLevel);
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = null
    )
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class ReadingOrderTraversalPolicy
    : FocusTraversalPolicy,
        DirectionalFocusTraversalPolicyMixin
{
    public virtual DartMap<
        FocusScopeNode,
        _DirectionalPolicyData__focus_traversal
    > _policyData { get; set; } =
        new DartMap<FocusScopeNode, _DirectionalPolicyData__focus_traversal>();

    public ReadingOrderTraversalPolicy(TraversalRequestFocusCallback? requestFocusCallback = null)
        : base(requestFocusCallback: requestFocusCallback) { }

    public static IEnumerable<FocusNode> sort(IEnumerable<FocusNode> nodes)
    {
        if (nodes.Count() <= 1L)
        {
            return nodes;
        }
        var data = nodes.Select(node => new _ReadingOrderSortData__focus_traversal(node)).ToList();
        var sortedList = new List<FocusNode>();
        var unplaced = data;
        _ReadingOrderSortData__focus_traversal current = _pickNext(unplaced);
        sortedList.Add(current.node);
        unplaced.Remove(current);
        while (Enumerable.Any(unplaced))
        {
            _ReadingOrderSortData__focus_traversal next = _pickNext(unplaced);
            current = next;
            sortedList.Add(current.node);
            unplaced.Remove(current);
        }
        return sortedList;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static List<_ReadingOrderDirectionalGroupData__focus_traversal> _collectDirectionalityGroups(
        IEnumerable<_ReadingOrderSortData__focus_traversal> candidates
    )
    {
        TextDirection? currentDirection = candidates.First().directionality;
        var currentGroup = new List<_ReadingOrderSortData__focus_traversal>();
        var result = new List<_ReadingOrderDirectionalGroupData__focus_traversal>();
        foreach (var candidate in candidates)
        {
            if (Equals(candidate.directionality, currentDirection))
            {
                currentGroup.Add(candidate);
                continue;
            }
            currentDirection = candidate.directionality;
            result.Add(new _ReadingOrderDirectionalGroupData__focus_traversal(currentGroup));
            currentGroup = new List<_ReadingOrderSortData__focus_traversal> { candidate };
        }
        if (Enumerable.Any(currentGroup))
        {
            result.Add(new _ReadingOrderDirectionalGroupData__focus_traversal(currentGroup));
        }
        foreach (var bandGroup in result)
        {
            if (checked(bandGroup.members.Count) == 1L)
            {
                continue;
            }
            _ReadingOrderSortData__focus_traversal.sortWithDirectionality(
                bandGroup.members,
                (
                    bandGroup.directionality
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            );
        }
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static _ReadingOrderSortData__focus_traversal _pickNext(
        List<_ReadingOrderSortData__focus_traversal> candidates
    )
    {
        CollectionsLibrary.mergeSort(
            candidates,
            compare: (a, b) => a.rect.top.CompareTo(b.rect.top)
        );
        _ReadingOrderSortData__focus_traversal topmost = candidates.First();
        List<_ReadingOrderSortData__focus_traversal> inBand(
            _ReadingOrderSortData__focus_traversal current,
            IEnumerable<_ReadingOrderSortData__focus_traversal> candidates
        )
        {
            var band = Rect.fromLTRB(
                double.NegativeInfinity,
                current.rect.top,
                double.PositiveInfinity,
                current.rect.bottom
            );
            return candidates
                .where(
                    (item) =>
                    {
                        return !item.rect.intersect(band).isEmpty;
                        throw new InvalidOperationException(
                            "Callback completed without returning a value."
                        );
                    }
                )
                .ToList();
            throw new InvalidOperationException(
                "Control flow completed without returning a value."
            );
        }
        List<_ReadingOrderSortData__focus_traversal> inBandOfTop = inBand(
                topmost,
                candidates.Cast<_ReadingOrderSortData__focus_traversal>()
            )
            .ToList();
        DartRuntimePrimitives.Assert(() => topmost.rect.isEmpty || Enumerable.Any(inBandOfTop));
        if (checked(inBandOfTop.Count) <= 1L)
        {
            return topmost;
        }
        TextDirection? nearestCommonDirectionality =
            _ReadingOrderSortData__focus_traversal.commonDirectionalityOf(inBandOfTop);
        _ReadingOrderSortData__focus_traversal.sortWithDirectionality(
            inBandOfTop,
            (
                nearestCommonDirectionality
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        );
        List<_ReadingOrderDirectionalGroupData__focus_traversal> bandGroups =
            _collectDirectionalityGroups(
                inBandOfTop.Cast<_ReadingOrderSortData__focus_traversal>()
            );
        if (checked(bandGroups.Count) == 1L)
        {
            return bandGroups.First().members.First();
        }
        _ReadingOrderDirectionalGroupData__focus_traversal.sortWithDirectionality(
            bandGroups,
            (
                (
                    nearestCommonDirectionality
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            )
        );
        return bandGroups.First().members.First();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override IEnumerable<FocusNode> sortDescendants(
        IEnumerable<FocusNode> descendants,
        FocusNode currentNode
    ) => sort(descendants.Cast<FocusNode>());

    public override void invalidateScopeData(FocusScopeNode node)
    {
        base.invalidateScopeData(node);
        _policyData.remove(node);
    }

    public override void changedScope(FocusNode? node = null, FocusScopeNode? oldScope = null)
    {
        base.changedScope(node: node, oldScope: oldScope);
        if (oldScope is not null)
        {
            _policyData
                .GetValueOrDefault(oldScope)
                ?.history.removeWhere(
                    (entry) =>
                    {
                        return Equals(entry.node, node);
                        throw new InvalidOperationException(
                            "Callback completed without returning a value."
                        );
                    }
                );
        }
    }

    public override FocusNode? findFirstFocusInDirection(
        FocusNode currentNode,
        TraversalDirection direction
    )
    {
        IEnumerable<FocusNode> nodes = currentNode.nearestScope!.traversalDescendants;
        List<FocusNode> sorted = nodes.ToList().ToList();
        var (vertical, first) = direction switch
        {
            TraversalDirection.up => (true, false),
            TraversalDirection.down => (true, true),
            TraversalDirection.left => (false, false),
            TraversalDirection.right => (false, true),
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        CollectionsLibrary.mergeSort(
            sorted,
            compare: (a, b) =>
            {
                if (vertical)
                {
                    if (first)
                    {
                        return a.rect.top.CompareTo(b.rect.top);
                    }
                    else
                    {
                        return b.rect.bottom.CompareTo(a.rect.bottom);
                    }
                }
                else
                {
                    if (first)
                    {
                        return a.rect.left.CompareTo(b.rect.left);
                    }
                    else
                    {
                        return b.rect.right.CompareTo(a.rect.right);
                    }
                }
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        return sorted.FirstOrDefault();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual FocusNode? _findNextFocusInDirection(
        FocusNode focusedChild,
        IEnumerable<FocusNode> traversalDescendants,
        TraversalDirection direction,
        bool forward = true
    )
    {
        switch (direction)
        {
            case TraversalDirection.down:
            case TraversalDirection.up:
            {
                IEnumerable<FocusNode> eligibleNodes = _sortAndFilterVertically(
                    direction,
                    focusedChild.rect,
                    traversalDescendants.Cast<FocusNode>(),
                    forward: forward
                );
                if (!Enumerable.Any(eligibleNodes))
                {
                    break;
                }
                ScrollableState? focusedScrollable = Scrollable.maybeOf(
                    focusedChild.context!,
                    axis: Axis.vertical
                );
                if (focusedScrollable is not null)
                {
                    IEnumerable<FocusNode> filteredEligibleNodes = eligibleNodes.where(
                        (node) =>
                            Equals(
                                Scrollable.maybeOf(node.context!, axis: Axis.vertical),
                                focusedScrollable
                            )
                    );
                    if (Enumerable.Any(filteredEligibleNodes))
                    {
                        eligibleNodes = filteredEligibleNodes;
                    }
                }
                if (Equals(direction, TraversalDirection.up))
                {
                    eligibleNodes = Enumerable.Reverse(eligibleNodes.ToList());
                }
                var band = Rect.fromLTRB(
                    focusedChild.rect.left,
                    -double.PositiveInfinity,
                    focusedChild.rect.right,
                    double.PositiveInfinity
                );
                IEnumerable<FocusNode> inBand = eligibleNodes.where(
                    (node) => !node.rect.intersect(band).isEmpty
                );
                if (Enumerable.Any(inBand))
                {
                    if (forward)
                    {
                        return DirectionalFocusTraversalPolicyMixin
                            ._sortByDistancePreferVertical(
                                focusedChild.rect.center,
                                inBand.Cast<FocusNode>()
                            )
                            .First();
                    }
                    return DirectionalFocusTraversalPolicyMixin
                        ._sortByDistancePreferVertical(
                            focusedChild.rect.center,
                            inBand.Cast<FocusNode>()
                        )
                        .Last();
                }
                if (forward)
                {
                    return DirectionalFocusTraversalPolicyMixin
                        ._sortClosestEdgesByDistancePreferHorizontal(
                            focusedChild.rect.center,
                            eligibleNodes.Cast<FocusNode>()
                        )
                        .First();
                }
                return DirectionalFocusTraversalPolicyMixin
                    ._sortClosestEdgesByDistancePreferHorizontal(
                        focusedChild.rect.center,
                        eligibleNodes.Cast<FocusNode>()
                    )
                    .Last();
            }
            case TraversalDirection.right:
            case TraversalDirection.left:
            {
                IEnumerable<FocusNode> eligibleNodesLocal = _sortAndFilterHorizontally(
                    direction,
                    focusedChild.rect,
                    traversalDescendants.Cast<FocusNode>(),
                    forward: forward
                );
                if (!Enumerable.Any(eligibleNodesLocal))
                {
                    break;
                }
                ScrollableState? focusedScrollableLocal = Scrollable.maybeOf(
                    focusedChild.context!,
                    axis: Axis.horizontal
                );
                if (focusedScrollableLocal is not null)
                {
                    IEnumerable<FocusNode> filteredEligibleNodesLocal = eligibleNodesLocal.where(
                        (node) =>
                            Equals(
                                Scrollable.maybeOf(node.context!, axis: Axis.horizontal),
                                focusedScrollableLocal
                            )
                    );
                    if (Enumerable.Any(filteredEligibleNodesLocal))
                    {
                        eligibleNodesLocal = filteredEligibleNodesLocal;
                    }
                }
                if (Equals(direction, TraversalDirection.left))
                {
                    eligibleNodesLocal = Enumerable.Reverse(eligibleNodesLocal.ToList());
                }
                var bandLocal = Rect.fromLTRB(
                    -double.PositiveInfinity,
                    focusedChild.rect.top,
                    double.PositiveInfinity,
                    focusedChild.rect.bottom
                );
                IEnumerable<FocusNode> inBandLocal = eligibleNodesLocal.where(
                    (node) => !node.rect.intersect(bandLocal).isEmpty
                );
                if (Enumerable.Any(inBandLocal))
                {
                    if (forward)
                    {
                        return DirectionalFocusTraversalPolicyMixin
                            ._sortByDistancePreferHorizontal(
                                focusedChild.rect.center,
                                inBandLocal.Cast<FocusNode>()
                            )
                            .First();
                    }
                    return DirectionalFocusTraversalPolicyMixin
                        ._sortByDistancePreferHorizontal(
                            focusedChild.rect.center,
                            inBandLocal.Cast<FocusNode>()
                        )
                        .Last();
                }
                if (forward)
                {
                    return DirectionalFocusTraversalPolicyMixin
                        ._sortClosestEdgesByDistancePreferVertical(
                            focusedChild.rect.center,
                            eligibleNodesLocal.Cast<FocusNode>()
                        )
                        .First();
                }
                return DirectionalFocusTraversalPolicyMixin
                    ._sortClosestEdgesByDistancePreferVertical(
                        focusedChild.rect.center,
                        eligibleNodesLocal.Cast<FocusNode>()
                    )
                    .Last();
            }
            default:
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                );
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual IEnumerable<FocusNode> _sortAndFilterHorizontally(
        TraversalDirection direction,
        Rect target,
        IEnumerable<FocusNode> nodes,
        bool forward = true
    )
    {
        DartRuntimePrimitives.Assert(() =>
            Equals(direction, TraversalDirection.left)
            || Equals(direction, TraversalDirection.right)
        );
        List<FocusNode> sorted = nodes
            .where(
                direction switch
                {
                    TraversalDirection.left => (node) =>
                        (!Equals(node.rect, target))
                        && (
                            forward
                                ? (node.rect.center.dx <= target.left)
                                : (node.rect.center.dx >= target.left)
                        ),
                    TraversalDirection.right => (node) =>
                        (!Equals(node.rect, target))
                        && (
                            forward
                                ? (node.rect.center.dx >= target.right)
                                : (node.rect.center.dx <= target.right)
                        ),
                    TraversalDirection.up => throw DartRuntimePrimitives.AsException(
                        new DartArgumentError($"Invalid direction {direction}")
                    ),
                    TraversalDirection.down => throw DartRuntimePrimitives.AsException(
                        new DartArgumentError($"Invalid direction {direction}")
                    ),
                    _ => throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
                }
            )
            .ToList()
            .ToList();
        CollectionsLibrary.mergeSort(
            sorted,
            compare: (a, b) => a.rect.center.dx.CompareTo(b.rect.center.dx)
        );
        return sorted;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual IEnumerable<FocusNode> _sortAndFilterVertically(
        TraversalDirection direction,
        Rect target,
        IEnumerable<FocusNode> nodes,
        bool forward = true
    )
    {
        DartRuntimePrimitives.Assert(() =>
            Equals(direction, TraversalDirection.up) || Equals(direction, TraversalDirection.down)
        );
        List<FocusNode> sorted = nodes
            .where(
                direction switch
                {
                    TraversalDirection.up => (node) =>
                        (!Equals(node.rect, target))
                        && (
                            forward
                                ? (node.rect.center.dy <= target.top)
                                : (node.rect.center.dy >= target.top)
                        ),
                    TraversalDirection.down => (node) =>
                        (!Equals(node.rect, target))
                        && (
                            forward
                                ? (node.rect.center.dy >= target.bottom)
                                : (node.rect.center.dy <= target.bottom)
                        ),
                    TraversalDirection.left => throw DartRuntimePrimitives.AsException(
                        new DartArgumentError($"Invalid direction {direction}")
                    ),
                    TraversalDirection.right => throw DartRuntimePrimitives.AsException(
                        new DartArgumentError($"Invalid direction {direction}")
                    ),
                    _ => throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
                }
            )
            .ToList()
            .ToList();
        CollectionsLibrary.mergeSort(
            sorted,
            compare: (a, b) => a.rect.center.dy.CompareTo(b.rect.center.dy)
        );
        return sorted;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool _popPolicyDataIfNeeded(
        TraversalDirection direction,
        FocusScopeNode nearestScope,
        FocusNode focusedChild,
        _FocusTraversalGroupNode__focus_traversal? groupNode
    )
    {
        _DirectionalPolicyData__focus_traversal? policyData = _policyData.GetValueOrDefault(
            nearestScope
        );
        if (
            (policyData is not null)
            && Enumerable.Any(policyData.history)
            && (!Equals(policyData.history.First().direction, direction))
        )
        {
            if (policyData.history.Last().node.parent is null)
            {
                invalidateScopeData(nearestScope);
                return false;
            }
            bool popOrInvalidate(TraversalDirection direction)
            {
                FocusNode lastNode = policyData.history.removeLast().node;
                if (
                    !Equals(
                        Scrollable.maybeOf(lastNode.context!),
                        Scrollable.maybeOf(Focus_managerLibrary.primaryFocus!.context!)
                    )
                )
                {
                    invalidateScopeData(nearestScope);
                    return false;
                }
                ScrollPositionAlignmentPolicy alignmentPolicyLocal = default!;
                switch (direction)
                {
                    case TraversalDirection.up:
                    case TraversalDirection.left:
                    {
                        alignmentPolicyLocal = ScrollPositionAlignmentPolicy.keepVisibleAtStart;
                        break;
                    }
                    case TraversalDirection.right:
                    case TraversalDirection.down:
                    {
                        alignmentPolicyLocal = ScrollPositionAlignmentPolicy.keepVisibleAtEnd;
                        break;
                    }
                }
                _requestFocus(
                    lastNode,
                    alignmentPolicy: (alignmentPolicyLocal),
                    groupNode: groupNode
                );
                return true;
                throw new InvalidOperationException(
                    "Control flow completed without returning a value."
                );
            }
            switch (direction)
            {
                case TraversalDirection.down:
                case TraversalDirection.up:
                {
                    switch (policyData.history.First().direction)
                    {
                        case TraversalDirection.left:
                        case TraversalDirection.right:
                        {
                            invalidateScopeData(nearestScope);
                            break;
                        }
                        case TraversalDirection.up:
                        case TraversalDirection.down:
                        {
                            if (popOrInvalidate(direction))
                            {
                                return true;
                            }
                            break;
                        }
                    }
                    break;
                }
                case TraversalDirection.left:
                case TraversalDirection.right:
                {
                    switch (policyData.history.First().direction)
                    {
                        case TraversalDirection.left:
                        case TraversalDirection.right:
                        {
                            if (popOrInvalidate(direction))
                            {
                                return true;
                            }
                            break;
                        }
                        case TraversalDirection.up:
                        case TraversalDirection.down:
                        {
                            invalidateScopeData(nearestScope);
                            break;
                        }
                    }
                    break;
                }
            }
        }
        if ((policyData is not null) && !Enumerable.Any(policyData.history))
        {
            invalidateScopeData(nearestScope);
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void _pushPolicyData(
        TraversalDirection direction,
        FocusScopeNode nearestScope,
        FocusNode focusedChild
    )
    {
        _DirectionalPolicyData__focus_traversal? policyData = _policyData.GetValueOrDefault(
            nearestScope
        );
        var newEntry = new _DirectionalPolicyDataEntry__focus_traversal(
            node: focusedChild,
            direction: direction
        );
        if (policyData is not null)
        {
            policyData.history.Add(newEntry);
        }
        else
        {
            _policyData[nearestScope] = new _DirectionalPolicyData__focus_traversal(
                history: new List<_DirectionalPolicyDataEntry__focus_traversal> { newEntry }
            );
        }
    }

    public virtual bool _requestTraversalFocusInDirection(
        FocusNode currentNode,
        FocusNode node,
        FocusScopeNode nearestScope,
        TraversalDirection direction,
        _FocusTraversalGroupNode__focus_traversal? groupNode
    )
    {
        if (node is FocusScopeNode)
        {
            if (((FocusScopeNode)node).focusedChild is not null)
            {
                return _requestTraversalFocusInDirection(
                    currentNode,
                    ((FocusScopeNode)node).focusedChild!,
                    DartRuntimePrimitives.ConvertValue<FocusScopeNode>(node),
                    direction,
                    groupNode
                );
            }
            FocusNode firstNode = findFirstFocusInDirection(node, direction) ?? currentNode;
            switch (direction)
            {
                case TraversalDirection.up:
                case TraversalDirection.left:
                {
                    _requestFocus(
                        firstNode,
                        groupNode,
                        alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart
                    );
                    break;
                }
                case TraversalDirection.right:
                case TraversalDirection.down:
                {
                    _requestFocus(
                        firstNode,
                        groupNode,
                        alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd
                    );
                    break;
                }
            }
            return true;
        }
        bool nodeHadPrimaryFocus = node.hasPrimaryFocus;
        switch (direction)
        {
            case TraversalDirection.up:
            case TraversalDirection.left:
            {
                _requestFocus(
                    node,
                    groupNode,
                    alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart
                );
                break;
            }
            case TraversalDirection.right:
            case TraversalDirection.down:
            {
                _requestFocus(
                    node,
                    groupNode,
                    alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd
                );
                break;
            }
        }
        return !nodeHadPrimaryFocus;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void _requestFocus(
        FocusNode node,
        _FocusTraversalGroupNode__focus_traversal? groupNode,
        ScrollPositionAlignmentPolicy? alignmentPolicy = null,
        double? alignment = null,
        Duration? duration = null,
        Curve? curve = null
    )
    {
        groupNode?.lastRequestedFocus = node;
        requestFocusCallback(
            node,
            alignmentPolicy: alignmentPolicy,
            alignment: alignment,
            duration: duration,
            curve: curve
        );
    }

    public virtual bool _onEdgeForDirection(
        FocusNode currentNode,
        FocusNode focusedChild,
        _FocusTraversalGroupNode__focus_traversal? groupNode,
        TraversalDirection direction,
        FocusScopeNode? scope = null
    )
    {
        FocusScopeNode nearestScopeLocal = scope ?? currentNode.nearestScope!;
        FocusNode? found = default!;
        switch (nearestScopeLocal.directionalTraversalEdgeBehavior)
        {
            case TraversalEdgeBehavior.leaveDorotiView:
            {
                focusedChild.unfocus();
                return false;
            }
            case TraversalEdgeBehavior.parentScope:
            {
                FocusScopeNode? parentScopeLocal = nearestScopeLocal.enclosingScope;
                if (
                    (parentScopeLocal is not null)
                    && (!Equals(parentScopeLocal, FocusManager.instance.rootScope))
                )
                {
                    invalidateScopeData(nearestScopeLocal);
                    nearestScopeLocal = parentScopeLocal;
                    invalidateScopeData(nearestScopeLocal);
                    found = _findNextFocusInDirection(
                        focusedChild,
                        nearestScopeLocal.traversalDescendants.Cast<FocusNode>(),
                        direction
                    );
                    if (found is null)
                    {
                        return _onEdgeForDirection(
                            currentNode,
                            focusedChild,
                            groupNode,
                            direction,
                            scope: nearestScopeLocal
                        );
                    }
                }
                else
                {
                    found = _findNextFocusInDirection(
                        focusedChild,
                        nearestScopeLocal.traversalDescendants.Cast<FocusNode>(),
                        direction,
                        forward: false
                    );
                }
                break;
            }
            case TraversalEdgeBehavior.closedLoop:
            {
                found = _findNextFocusInDirection(
                    focusedChild,
                    nearestScopeLocal.traversalDescendants.Cast<FocusNode>(),
                    direction,
                    forward: false
                );
                break;
            }
            case TraversalEdgeBehavior.stop:
            {
                return false;
            }
        }
        if (found is not null)
        {
            return _requestTraversalFocusInDirection(
                currentNode,
                found,
                DartRuntimePrimitives.ConvertValue<FocusScopeNode>(nearestScopeLocal),
                direction,
                groupNode
            );
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool inDirection(FocusNode currentNode, TraversalDirection direction)
    {
        _FocusTraversalGroupNode__focus_traversal? groupNodeLocal =
            FocusTraversalGroup._getGroupNode(currentNode);
        FocusScopeNode nearestScopeLocal = currentNode.nearestScope!;
        FocusNode? focusedChildLocal = nearestScopeLocal.focusedChild;
        if (focusedChildLocal is null)
        {
            FocusNode firstFocus = findFirstFocusInDirection(currentNode, direction) ?? currentNode;
            switch (direction)
            {
                case TraversalDirection.up:
                case TraversalDirection.left:
                {
                    _requestFocus(
                        firstFocus,
                        alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart,
                        groupNode: groupNodeLocal
                    );
                    break;
                }
                case TraversalDirection.right:
                case TraversalDirection.down:
                {
                    _requestFocus(
                        firstFocus,
                        alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd,
                        groupNode: groupNodeLocal
                    );
                    break;
                }
            }
            return true;
        }
        if (_popPolicyDataIfNeeded(direction, nearestScopeLocal, focusedChildLocal, groupNodeLocal))
        {
            return true;
        }
        FocusNode? found = _findNextFocusInDirection(
            focusedChildLocal,
            nearestScopeLocal.traversalDescendants.Cast<FocusNode>(),
            direction
        );
        if (found is not null)
        {
            _pushPolicyData(direction, nearestScopeLocal, focusedChildLocal);
            return _requestTraversalFocusInDirection(
                currentNode,
                found,
                DartRuntimePrimitives.ConvertValue<FocusScopeNode>(nearestScopeLocal),
                direction,
                groupNodeLocal
            );
        }
        return _onEdgeForDirection(currentNode, focusedChildLocal, groupNodeLocal, direction);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public abstract class FocusOrder : Diagnosticable, IComparable<FocusOrder>
{
    protected FocusOrder() { }

    public virtual long compareTo(FocusOrder other)
    {
        DartRuntimePrimitives.Assert(
            () => Equals(GetType(), DartRuntimePrimitives.RuntimeType(other)),
            () =>
                (object?)
                    "The sorting algorithm must not compare incomparable keys, since they don't "
                + $"know how to order themselves relative to each other. Comparing {this} with {other}"
        );
        return doCompare(other);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public abstract long doCompare(FocusOrder other);

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);

    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine)
                .toDiagnosticsNode()
                .toStringDeep(minLevel: minLevel);
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = null
    )
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties) { }

    public int CompareTo(FocusOrder? other) => checked((int)compareTo(other!));
}

public class NumericFocusOrder : FocusOrder
{
    public virtual double order { get; private set; } = default!;

    public NumericFocusOrder(double order)
    {
        this.order = order;
    }

    public override long doCompare(FocusOrder other) =>
        order.CompareTo(((NumericFocusOrder)other).order);

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("order", order));
    }
}

public class LexicalFocusOrder : FocusOrder
{
    public virtual string order { get; private set; } = default!;

    public LexicalFocusOrder(string order)
    {
        this.order = order;
    }

    public override long doCompare(FocusOrder other) =>
        order.CompareTo(((LexicalFocusOrder)other).order);

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new StringProperty("order", order));
    }
}

internal class _OrderedFocusInfo__focus_traversal
{
    public virtual FocusNode node { get; private set; } = default!;
    public virtual FocusOrder order { get; private set; } = default!;

    internal _OrderedFocusInfo__focus_traversal(FocusNode node, FocusOrder order)
    {
        this.node = node;
        this.order = order;
    }
}

public class OrderedTraversalPolicy : FocusTraversalPolicy, DirectionalFocusTraversalPolicyMixin
{
    public virtual FocusTraversalPolicy? secondary { get; private set; }
    public virtual DartMap<
        FocusScopeNode,
        _DirectionalPolicyData__focus_traversal
    > _policyData { get; set; } =
        new DartMap<FocusScopeNode, _DirectionalPolicyData__focus_traversal>();

    public OrderedTraversalPolicy(
        FocusTraversalPolicy? secondary = null,
        TraversalRequestFocusCallback? requestFocusCallback = null
    )
        : base(requestFocusCallback: requestFocusCallback)
    {
        this.secondary = secondary;
    }

    public override IEnumerable<FocusNode> sortDescendants(
        IEnumerable<FocusNode> descendants,
        FocusNode currentNode
    )
    {
        FocusTraversalPolicy secondaryPolicy = secondary ?? new ReadingOrderTraversalPolicy();
        IEnumerable<FocusNode> sortedDescendants = secondaryPolicy.sortDescendants(
            descendants.Cast<FocusNode>(),
            currentNode
        );
        var unordered = new List<FocusNode>();
        var ordered = new List<_OrderedFocusInfo__focus_traversal>();
        foreach (var nodeLocal in sortedDescendants)
        {
            FocusOrder? orderLocal = FocusTraversalOrder.maybeOf(nodeLocal.context!);
            if (orderLocal is not null)
            {
                ordered.Add(
                    new _OrderedFocusInfo__focus_traversal(node: nodeLocal, order: orderLocal)
                );
            }
            else
            {
                unordered.Add(nodeLocal);
            }
        }
        CollectionsLibrary.mergeSort(
            ordered,
            compare: (a, b) =>
            {
                DartRuntimePrimitives.Assert(
                    () =>
                        Equals(
                            DartRuntimePrimitives.RuntimeType(a.order),
                            DartRuntimePrimitives.RuntimeType(b.order)
                        ),
                    () =>
                        (object?)
                            $"When sorting nodes for determining focus order, the order ({a.order}) of "
                        + $"node {a.node}, isn't the same type as the order ({b.order}) of {b.node}. "
                        + "Incompatible order types can't be compared. Use a FocusTraversalGroup to group "
                        + "similar orders together."
                );
                return a.order.compareTo(b.order);
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        return ordered.map((info) => info.node).followedBy(unordered.Cast<FocusNode>());
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void invalidateScopeData(FocusScopeNode node)
    {
        base.invalidateScopeData(node);
        _policyData.remove(node);
    }

    public override void changedScope(FocusNode? node = null, FocusScopeNode? oldScope = null)
    {
        base.changedScope(node: node, oldScope: oldScope);
        if (oldScope is not null)
        {
            _policyData
                .GetValueOrDefault(oldScope)
                ?.history.removeWhere(
                    (entry) =>
                    {
                        return Equals(entry.node, node);
                        throw new InvalidOperationException(
                            "Callback completed without returning a value."
                        );
                    }
                );
        }
    }

    public override FocusNode? findFirstFocusInDirection(
        FocusNode currentNode,
        TraversalDirection direction
    )
    {
        IEnumerable<FocusNode> nodes = currentNode.nearestScope!.traversalDescendants;
        List<FocusNode> sorted = nodes.ToList().ToList();
        var (vertical, first) = direction switch
        {
            TraversalDirection.up => (true, false),
            TraversalDirection.down => (true, true),
            TraversalDirection.left => (false, false),
            TraversalDirection.right => (false, true),
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        CollectionsLibrary.mergeSort(
            sorted,
            compare: (a, b) =>
            {
                if (vertical)
                {
                    if (first)
                    {
                        return a.rect.top.CompareTo(b.rect.top);
                    }
                    else
                    {
                        return b.rect.bottom.CompareTo(a.rect.bottom);
                    }
                }
                else
                {
                    if (first)
                    {
                        return a.rect.left.CompareTo(b.rect.left);
                    }
                    else
                    {
                        return b.rect.right.CompareTo(a.rect.right);
                    }
                }
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        return sorted.FirstOrDefault();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual FocusNode? _findNextFocusInDirection(
        FocusNode focusedChild,
        IEnumerable<FocusNode> traversalDescendants,
        TraversalDirection direction,
        bool forward = true
    )
    {
        switch (direction)
        {
            case TraversalDirection.down:
            case TraversalDirection.up:
            {
                IEnumerable<FocusNode> eligibleNodes = _sortAndFilterVertically(
                    direction,
                    focusedChild.rect,
                    traversalDescendants.Cast<FocusNode>(),
                    forward: forward
                );
                if (!Enumerable.Any(eligibleNodes))
                {
                    break;
                }
                ScrollableState? focusedScrollable = Scrollable.maybeOf(
                    focusedChild.context!,
                    axis: Axis.vertical
                );
                if (focusedScrollable is not null)
                {
                    IEnumerable<FocusNode> filteredEligibleNodes = eligibleNodes.where(
                        (node) =>
                            Equals(
                                Scrollable.maybeOf(node.context!, axis: Axis.vertical),
                                focusedScrollable
                            )
                    );
                    if (Enumerable.Any(filteredEligibleNodes))
                    {
                        eligibleNodes = filteredEligibleNodes;
                    }
                }
                if (Equals(direction, TraversalDirection.up))
                {
                    eligibleNodes = Enumerable.Reverse(eligibleNodes.ToList());
                }
                var band = Rect.fromLTRB(
                    focusedChild.rect.left,
                    -double.PositiveInfinity,
                    focusedChild.rect.right,
                    double.PositiveInfinity
                );
                IEnumerable<FocusNode> inBand = eligibleNodes.where(
                    (node) => !node.rect.intersect(band).isEmpty
                );
                if (Enumerable.Any(inBand))
                {
                    if (forward)
                    {
                        return DirectionalFocusTraversalPolicyMixin
                            ._sortByDistancePreferVertical(
                                focusedChild.rect.center,
                                inBand.Cast<FocusNode>()
                            )
                            .First();
                    }
                    return DirectionalFocusTraversalPolicyMixin
                        ._sortByDistancePreferVertical(
                            focusedChild.rect.center,
                            inBand.Cast<FocusNode>()
                        )
                        .Last();
                }
                if (forward)
                {
                    return DirectionalFocusTraversalPolicyMixin
                        ._sortClosestEdgesByDistancePreferHorizontal(
                            focusedChild.rect.center,
                            eligibleNodes.Cast<FocusNode>()
                        )
                        .First();
                }
                return DirectionalFocusTraversalPolicyMixin
                    ._sortClosestEdgesByDistancePreferHorizontal(
                        focusedChild.rect.center,
                        eligibleNodes.Cast<FocusNode>()
                    )
                    .Last();
            }
            case TraversalDirection.right:
            case TraversalDirection.left:
            {
                IEnumerable<FocusNode> eligibleNodesLocal = _sortAndFilterHorizontally(
                    direction,
                    focusedChild.rect,
                    traversalDescendants.Cast<FocusNode>(),
                    forward: forward
                );
                if (!Enumerable.Any(eligibleNodesLocal))
                {
                    break;
                }
                ScrollableState? focusedScrollableLocal = Scrollable.maybeOf(
                    focusedChild.context!,
                    axis: Axis.horizontal
                );
                if (focusedScrollableLocal is not null)
                {
                    IEnumerable<FocusNode> filteredEligibleNodesLocal = eligibleNodesLocal.where(
                        (node) =>
                            Equals(
                                Scrollable.maybeOf(node.context!, axis: Axis.horizontal),
                                focusedScrollableLocal
                            )
                    );
                    if (Enumerable.Any(filteredEligibleNodesLocal))
                    {
                        eligibleNodesLocal = filteredEligibleNodesLocal;
                    }
                }
                if (Equals(direction, TraversalDirection.left))
                {
                    eligibleNodesLocal = Enumerable.Reverse(eligibleNodesLocal.ToList());
                }
                var bandLocal = Rect.fromLTRB(
                    -double.PositiveInfinity,
                    focusedChild.rect.top,
                    double.PositiveInfinity,
                    focusedChild.rect.bottom
                );
                IEnumerable<FocusNode> inBandLocal = eligibleNodesLocal.where(
                    (node) => !node.rect.intersect(bandLocal).isEmpty
                );
                if (Enumerable.Any(inBandLocal))
                {
                    if (forward)
                    {
                        return DirectionalFocusTraversalPolicyMixin
                            ._sortByDistancePreferHorizontal(
                                focusedChild.rect.center,
                                inBandLocal.Cast<FocusNode>()
                            )
                            .First();
                    }
                    return DirectionalFocusTraversalPolicyMixin
                        ._sortByDistancePreferHorizontal(
                            focusedChild.rect.center,
                            inBandLocal.Cast<FocusNode>()
                        )
                        .Last();
                }
                if (forward)
                {
                    return DirectionalFocusTraversalPolicyMixin
                        ._sortClosestEdgesByDistancePreferVertical(
                            focusedChild.rect.center,
                            eligibleNodesLocal.Cast<FocusNode>()
                        )
                        .First();
                }
                return DirectionalFocusTraversalPolicyMixin
                    ._sortClosestEdgesByDistancePreferVertical(
                        focusedChild.rect.center,
                        eligibleNodesLocal.Cast<FocusNode>()
                    )
                    .Last();
            }
            default:
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                );
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual IEnumerable<FocusNode> _sortAndFilterHorizontally(
        TraversalDirection direction,
        Rect target,
        IEnumerable<FocusNode> nodes,
        bool forward = true
    )
    {
        DartRuntimePrimitives.Assert(() =>
            Equals(direction, TraversalDirection.left)
            || Equals(direction, TraversalDirection.right)
        );
        List<FocusNode> sorted = nodes
            .where(
                direction switch
                {
                    TraversalDirection.left => (node) =>
                        (!Equals(node.rect, target))
                        && (
                            forward
                                ? (node.rect.center.dx <= target.left)
                                : (node.rect.center.dx >= target.left)
                        ),
                    TraversalDirection.right => (node) =>
                        (!Equals(node.rect, target))
                        && (
                            forward
                                ? (node.rect.center.dx >= target.right)
                                : (node.rect.center.dx <= target.right)
                        ),
                    TraversalDirection.up => throw DartRuntimePrimitives.AsException(
                        new DartArgumentError($"Invalid direction {direction}")
                    ),
                    TraversalDirection.down => throw DartRuntimePrimitives.AsException(
                        new DartArgumentError($"Invalid direction {direction}")
                    ),
                    _ => throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
                }
            )
            .ToList()
            .ToList();
        CollectionsLibrary.mergeSort(
            sorted,
            compare: (a, b) => a.rect.center.dx.CompareTo(b.rect.center.dx)
        );
        return sorted;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual IEnumerable<FocusNode> _sortAndFilterVertically(
        TraversalDirection direction,
        Rect target,
        IEnumerable<FocusNode> nodes,
        bool forward = true
    )
    {
        DartRuntimePrimitives.Assert(() =>
            Equals(direction, TraversalDirection.up) || Equals(direction, TraversalDirection.down)
        );
        List<FocusNode> sorted = nodes
            .where(
                direction switch
                {
                    TraversalDirection.up => (node) =>
                        (!Equals(node.rect, target))
                        && (
                            forward
                                ? (node.rect.center.dy <= target.top)
                                : (node.rect.center.dy >= target.top)
                        ),
                    TraversalDirection.down => (node) =>
                        (!Equals(node.rect, target))
                        && (
                            forward
                                ? (node.rect.center.dy >= target.bottom)
                                : (node.rect.center.dy <= target.bottom)
                        ),
                    TraversalDirection.left => throw DartRuntimePrimitives.AsException(
                        new DartArgumentError($"Invalid direction {direction}")
                    ),
                    TraversalDirection.right => throw DartRuntimePrimitives.AsException(
                        new DartArgumentError($"Invalid direction {direction}")
                    ),
                    _ => throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
                }
            )
            .ToList()
            .ToList();
        CollectionsLibrary.mergeSort(
            sorted,
            compare: (a, b) => a.rect.center.dy.CompareTo(b.rect.center.dy)
        );
        return sorted;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool _popPolicyDataIfNeeded(
        TraversalDirection direction,
        FocusScopeNode nearestScope,
        FocusNode focusedChild,
        _FocusTraversalGroupNode__focus_traversal? groupNode
    )
    {
        _DirectionalPolicyData__focus_traversal? policyData = _policyData.GetValueOrDefault(
            nearestScope
        );
        if (
            (policyData is not null)
            && Enumerable.Any(policyData.history)
            && (!Equals(policyData.history.First().direction, direction))
        )
        {
            if (policyData.history.Last().node.parent is null)
            {
                invalidateScopeData(nearestScope);
                return false;
            }
            bool popOrInvalidate(TraversalDirection direction)
            {
                FocusNode lastNode = policyData.history.removeLast().node;
                if (
                    !Equals(
                        Scrollable.maybeOf(lastNode.context!),
                        Scrollable.maybeOf(Focus_managerLibrary.primaryFocus!.context!)
                    )
                )
                {
                    invalidateScopeData(nearestScope);
                    return false;
                }
                ScrollPositionAlignmentPolicy alignmentPolicyLocal = default!;
                switch (direction)
                {
                    case TraversalDirection.up:
                    case TraversalDirection.left:
                    {
                        alignmentPolicyLocal = ScrollPositionAlignmentPolicy.keepVisibleAtStart;
                        break;
                    }
                    case TraversalDirection.right:
                    case TraversalDirection.down:
                    {
                        alignmentPolicyLocal = ScrollPositionAlignmentPolicy.keepVisibleAtEnd;
                        break;
                    }
                }
                _requestFocus(
                    lastNode,
                    alignmentPolicy: (alignmentPolicyLocal),
                    groupNode: groupNode
                );
                return true;
                throw new InvalidOperationException(
                    "Control flow completed without returning a value."
                );
            }
            switch (direction)
            {
                case TraversalDirection.down:
                case TraversalDirection.up:
                {
                    switch (policyData.history.First().direction)
                    {
                        case TraversalDirection.left:
                        case TraversalDirection.right:
                        {
                            invalidateScopeData(nearestScope);
                            break;
                        }
                        case TraversalDirection.up:
                        case TraversalDirection.down:
                        {
                            if (popOrInvalidate(direction))
                            {
                                return true;
                            }
                            break;
                        }
                    }
                    break;
                }
                case TraversalDirection.left:
                case TraversalDirection.right:
                {
                    switch (policyData.history.First().direction)
                    {
                        case TraversalDirection.left:
                        case TraversalDirection.right:
                        {
                            if (popOrInvalidate(direction))
                            {
                                return true;
                            }
                            break;
                        }
                        case TraversalDirection.up:
                        case TraversalDirection.down:
                        {
                            invalidateScopeData(nearestScope);
                            break;
                        }
                    }
                    break;
                }
            }
        }
        if ((policyData is not null) && !Enumerable.Any(policyData.history))
        {
            invalidateScopeData(nearestScope);
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void _pushPolicyData(
        TraversalDirection direction,
        FocusScopeNode nearestScope,
        FocusNode focusedChild
    )
    {
        _DirectionalPolicyData__focus_traversal? policyData = _policyData.GetValueOrDefault(
            nearestScope
        );
        var newEntry = new _DirectionalPolicyDataEntry__focus_traversal(
            node: focusedChild,
            direction: direction
        );
        if (policyData is not null)
        {
            policyData.history.Add(newEntry);
        }
        else
        {
            _policyData[nearestScope] = new _DirectionalPolicyData__focus_traversal(
                history: new List<_DirectionalPolicyDataEntry__focus_traversal> { newEntry }
            );
        }
    }

    public virtual bool _requestTraversalFocusInDirection(
        FocusNode currentNode,
        FocusNode node,
        FocusScopeNode nearestScope,
        TraversalDirection direction,
        _FocusTraversalGroupNode__focus_traversal? groupNode
    )
    {
        if (node is FocusScopeNode)
        {
            if (((FocusScopeNode)node).focusedChild is not null)
            {
                return _requestTraversalFocusInDirection(
                    currentNode,
                    ((FocusScopeNode)node).focusedChild!,
                    DartRuntimePrimitives.ConvertValue<FocusScopeNode>(node),
                    direction,
                    groupNode
                );
            }
            FocusNode firstNode = findFirstFocusInDirection(node, direction) ?? currentNode;
            switch (direction)
            {
                case TraversalDirection.up:
                case TraversalDirection.left:
                {
                    _requestFocus(
                        firstNode,
                        groupNode,
                        alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart
                    );
                    break;
                }
                case TraversalDirection.right:
                case TraversalDirection.down:
                {
                    _requestFocus(
                        firstNode,
                        groupNode,
                        alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd
                    );
                    break;
                }
            }
            return true;
        }
        bool nodeHadPrimaryFocus = node.hasPrimaryFocus;
        switch (direction)
        {
            case TraversalDirection.up:
            case TraversalDirection.left:
            {
                _requestFocus(
                    node,
                    groupNode,
                    alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart
                );
                break;
            }
            case TraversalDirection.right:
            case TraversalDirection.down:
            {
                _requestFocus(
                    node,
                    groupNode,
                    alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd
                );
                break;
            }
        }
        return !nodeHadPrimaryFocus;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void _requestFocus(
        FocusNode node,
        _FocusTraversalGroupNode__focus_traversal? groupNode,
        ScrollPositionAlignmentPolicy? alignmentPolicy = null,
        double? alignment = null,
        Duration? duration = null,
        Curve? curve = null
    )
    {
        groupNode?.lastRequestedFocus = node;
        requestFocusCallback(
            node,
            alignmentPolicy: alignmentPolicy,
            alignment: alignment,
            duration: duration,
            curve: curve
        );
    }

    public virtual bool _onEdgeForDirection(
        FocusNode currentNode,
        FocusNode focusedChild,
        _FocusTraversalGroupNode__focus_traversal? groupNode,
        TraversalDirection direction,
        FocusScopeNode? scope = null
    )
    {
        FocusScopeNode nearestScopeLocal = scope ?? currentNode.nearestScope!;
        FocusNode? found = default!;
        switch (nearestScopeLocal.directionalTraversalEdgeBehavior)
        {
            case TraversalEdgeBehavior.leaveDorotiView:
            {
                focusedChild.unfocus();
                return false;
            }
            case TraversalEdgeBehavior.parentScope:
            {
                FocusScopeNode? parentScopeLocal = nearestScopeLocal.enclosingScope;
                if (
                    (parentScopeLocal is not null)
                    && (!Equals(parentScopeLocal, FocusManager.instance.rootScope))
                )
                {
                    invalidateScopeData(nearestScopeLocal);
                    nearestScopeLocal = parentScopeLocal;
                    invalidateScopeData(nearestScopeLocal);
                    found = _findNextFocusInDirection(
                        focusedChild,
                        nearestScopeLocal.traversalDescendants.Cast<FocusNode>(),
                        direction
                    );
                    if (found is null)
                    {
                        return _onEdgeForDirection(
                            currentNode,
                            focusedChild,
                            groupNode,
                            direction,
                            scope: nearestScopeLocal
                        );
                    }
                }
                else
                {
                    found = _findNextFocusInDirection(
                        focusedChild,
                        nearestScopeLocal.traversalDescendants.Cast<FocusNode>(),
                        direction,
                        forward: false
                    );
                }
                break;
            }
            case TraversalEdgeBehavior.closedLoop:
            {
                found = _findNextFocusInDirection(
                    focusedChild,
                    nearestScopeLocal.traversalDescendants.Cast<FocusNode>(),
                    direction,
                    forward: false
                );
                break;
            }
            case TraversalEdgeBehavior.stop:
            {
                return false;
            }
        }
        if (found is not null)
        {
            return _requestTraversalFocusInDirection(
                currentNode,
                found,
                DartRuntimePrimitives.ConvertValue<FocusScopeNode>(nearestScopeLocal),
                direction,
                groupNode
            );
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool inDirection(FocusNode currentNode, TraversalDirection direction)
    {
        _FocusTraversalGroupNode__focus_traversal? groupNodeLocal =
            FocusTraversalGroup._getGroupNode(currentNode);
        FocusScopeNode nearestScopeLocal = currentNode.nearestScope!;
        FocusNode? focusedChildLocal = nearestScopeLocal.focusedChild;
        if (focusedChildLocal is null)
        {
            FocusNode firstFocus = findFirstFocusInDirection(currentNode, direction) ?? currentNode;
            switch (direction)
            {
                case TraversalDirection.up:
                case TraversalDirection.left:
                {
                    _requestFocus(
                        firstFocus,
                        alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart,
                        groupNode: groupNodeLocal
                    );
                    break;
                }
                case TraversalDirection.right:
                case TraversalDirection.down:
                {
                    _requestFocus(
                        firstFocus,
                        alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd,
                        groupNode: groupNodeLocal
                    );
                    break;
                }
            }
            return true;
        }
        if (_popPolicyDataIfNeeded(direction, nearestScopeLocal, focusedChildLocal, groupNodeLocal))
        {
            return true;
        }
        FocusNode? found = _findNextFocusInDirection(
            focusedChildLocal,
            nearestScopeLocal.traversalDescendants.Cast<FocusNode>(),
            direction
        );
        if (found is not null)
        {
            _pushPolicyData(direction, nearestScopeLocal, focusedChildLocal);
            return _requestTraversalFocusInDirection(
                currentNode,
                found,
                DartRuntimePrimitives.ConvertValue<FocusScopeNode>(nearestScopeLocal),
                direction,
                groupNodeLocal
            );
        }
        return _onEdgeForDirection(currentNode, focusedChildLocal, groupNodeLocal, direction);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class FocusTraversalOrder : InheritedWidget
{
    public virtual FocusOrder order { get; private set; } = default!;

    public FocusTraversalOrder(
        Key? key = null,
        FocusOrder order = default!,
        Widget child = default!
    )
        : base(key: key, child: child)
    {
        this.order = order;
    }

    public static FocusOrder of(BuildContext context)
    {
        FocusTraversalOrder? marker = context.getInheritedWidgetOfExactType<FocusTraversalOrder>();
        DartRuntimePrimitives.Assert(() =>
        {
            if (marker is null)
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create(
                        "FocusTraversalOrder.of() was called with a context that "
                            + "does not contain a FocusTraversalOrder widget. No TraversalOrder widget "
                            + "ancestor could be found starting from the context that was passed to "
                            + "FocusTraversalOrder.of().\n"
                            + "The context used was:\n"
                            + $"  {context}"
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return marker!.order;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static FocusOrder? maybeOf(BuildContext context)
    {
        FocusTraversalOrder? marker = context.getInheritedWidgetOfExactType<FocusTraversalOrder>();
        return marker?.order;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => false;

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<FocusOrder>("order", order));
    }
}

public class FocusTraversalGroup : StatefulWidget
{
    public virtual FocusTraversalPolicy policy { get; private set; } = default!;
    public virtual bool descendantsAreFocusable { get; private set; } = default!;
    public virtual bool descendantsAreTraversable { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;
    public virtual Action<FocusNode>? onFocusNodeCreated { get; private set; }
    public virtual FocusNode? parentNode { get; private set; }

    public FocusTraversalGroup(
        Key? key = null,
        FocusTraversalPolicy? policy = null,
        bool descendantsAreFocusable = true,
        bool descendantsAreTraversable = true,
        Action<FocusNode>? onFocusNodeCreated = null,
        FocusNode? parentNode = null,
        Widget child = default!
    )
        : base(key: key)
    {
        this.descendantsAreFocusable = descendantsAreFocusable;
        this.descendantsAreTraversable = descendantsAreTraversable;
        this.onFocusNodeCreated = onFocusNodeCreated;
        this.parentNode = parentNode;
        this.child = child;
        this.policy = policy ?? new ReadingOrderTraversalPolicy();
    }

    public static FocusTraversalPolicy? maybeOfNode(FocusNode node)
    {
        return _getGroupNode(node)?.policy;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static _FocusTraversalGroupNode__focus_traversal? _getGroupNode(FocusNode node)
    {
        while (node.parent is not null)
        {
            if (node.context is null)
            {
                return null;
            }
            if (node is _FocusTraversalGroupNode__focus_traversal)
            {
                _FocusTraversalGroupNode__focus_traversal node__as86847 =
                    (_FocusTraversalGroupNode__focus_traversal)node;
                return node__as86847;
            }
            node = node.parent!;
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static FocusTraversalPolicy of(BuildContext context)
    {
        FocusTraversalPolicy? policy = maybeOf(context);
        DartRuntimePrimitives.Assert(() =>
        {
            if (policy is null)
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create(
                        "Unable to find a Focus or FocusScope widget in the given context, or the FocusNode "
                            + "from with the widget that was found is not associated with a FocusTraversalPolicy.\n"
                            + "FocusTraversalGroup.of() was called with a context that does not contain a "
                            + "Focus or FocusScope widget, or there was no FocusTraversalPolicy in effect.\n"
                            + "This can happen if there is not a FocusTraversalGroup that defines the policy, "
                            + "or if the context comes from a widget that is above the WidgetsApp, MaterialApp, "
                            + "or CupertinoApp widget (those widgets introduce an implicit default policy) \n"
                            + "The context used was:\n"
                            + $"  {context}"
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return policy!;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static FocusTraversalPolicy? maybeOf(BuildContext context)
    {
        FocusNode? node = Focus.maybeOf(context, scopeOk: true, createDependency: false);
        if (node is null)
        {
            return null;
        }
        return maybeOfNode(node);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _FocusTraversalGroupState__focus_traversal()
        );

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<FocusTraversalPolicy>("policy", policy));
    }
}

public class _FocusTraversalGroupNode__focus_traversal : FocusNode
{
    public virtual FocusTraversalPolicy policy { get; set; } = default!;
    public virtual FocusNode? lastRequestedFocus { get; set; } = default;

    internal _FocusTraversalGroupNode__focus_traversal(
        string? debugLabel = null,
        FocusTraversalPolicy policy = default!
    )
        : base(debugLabel: debugLabel)
    {
        this.policy = policy;
    }
}

internal class _FocusTraversalGroupState__focus_traversal : State<FocusTraversalGroup>
{
    private bool __late_focusNode_initialized;
    private _FocusTraversalGroupNode__focus_traversal __late_focusNode = default!;
    public virtual _FocusTraversalGroupNode__focus_traversal focusNode
    {
        get
        {
            if (!__late_focusNode_initialized)
            {
                __late_focusNode = new _FocusTraversalGroupNode__focus_traversal(
                    debugLabel: "FocusTraversalGroup",
                    policy: widget.policy
                );
                __late_focusNode_initialized = true;
            }
            return __late_focusNode;
        }
    }

    public override void initState()
    {
        base.initState();
        FocusManager.instance.addListener(_handleFocusChanged);
        widget.onFocusNodeCreated?.Invoke(focusNode);
    }

    public override void dispose()
    {
        FocusManager.instance.removeListener(_handleFocusChanged);
        focusNode.dispose();
        base.dispose();
    }

    public override void didUpdateWidget(FocusTraversalGroup oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(oldWidget.policy, widget.policy))
        {
            focusNode.policy = widget.policy;
        }
    }

    public override Widget build(BuildContext context)
    {
        return new Focus(
            focusNode: focusNode,
            parentNode: widget.parentNode,
            canRequestFocus: false,
            skipTraversal: true,
            includeSemantics: false,
            descendantsAreFocusable: widget.descendantsAreFocusable,
            descendantsAreTraversable: widget.descendantsAreTraversable,
            child: widget.child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _handleFocusChanged()
    {
        FocusNode? primaryFocusLocal = FocusManager.instance.primaryFocus;
        FocusNode? lastRequestedFocusLocal = focusNode.lastRequestedFocus;
        if (lastRequestedFocusLocal is null)
        {
            return;
        }
        if (!Equals(primaryFocusLocal, lastRequestedFocusLocal))
        {
            FocusScopeNode? scope = primaryFocusLocal?.nearestScope;
            while (scope is not null)
            {
                widget.policy.invalidateScopeData(scope);
                scope = scope.enclosingScope;
            }
            focusNode.lastRequestedFocus = null;
        }
    }
}

public class RequestFocusIntent : Intent
{
    public virtual TraversalRequestFocusCallback requestFocusCallback { get; private set; } =
        default!;
    public virtual FocusNode focusNode { get; private set; } = default!;

    public RequestFocusIntent(
        FocusNode focusNode,
        TraversalRequestFocusCallback? requestFocusCallback = null
    )
    {
        this.focusNode = focusNode;
        this.requestFocusCallback =
            requestFocusCallback ?? FocusTraversalPolicy.defaultTraversalRequestFocusCallback;
    }
}

public class RequestFocusAction : IntentAction<RequestFocusIntent>
{
    public override object? invoke(RequestFocusIntent intent, BuildContext? context = null)
    {
        intent.requestFocusCallback(intent.focusNode);
        return null;
    }
}

public class NextFocusIntent : Intent
{
    public NextFocusIntent() { }
}

public class NextFocusAction : IntentAction<NextFocusIntent>
{
    public override object? invoke(NextFocusIntent intent, BuildContext? context = null)
    {
        return Focus_managerLibrary.primaryFocus!.nextFocus();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override KeyEventResult toKeyEventResult(NextFocusIntent intent, object? invokeResult)
    {
        bool __invokeResult = DartRuntimePrimitives.ConvertValue<bool>(invokeResult);
        return __invokeResult ? KeyEventResult.handled : KeyEventResult.skipRemainingHandlers;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class PreviousFocusIntent : Intent
{
    public PreviousFocusIntent() { }
}

public class PreviousFocusAction : IntentAction<PreviousFocusIntent>
{
    public override object? invoke(PreviousFocusIntent intent, BuildContext? context = null)
    {
        return Focus_managerLibrary.primaryFocus!.previousFocus();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override KeyEventResult toKeyEventResult(
        PreviousFocusIntent intent,
        object? invokeResult
    )
    {
        bool __invokeResult = DartRuntimePrimitives.ConvertValue<bool>(invokeResult);
        return __invokeResult ? KeyEventResult.handled : KeyEventResult.skipRemainingHandlers;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class DirectionalFocusIntent : Intent
{
    public virtual TraversalDirection direction { get; private set; } = default!;
    public virtual bool ignoreTextFields { get; private set; } = default!;

    public DirectionalFocusIntent(TraversalDirection direction, bool ignoreTextFields = true)
    {
        this.direction = direction;
        this.ignoreTextFields = ignoreTextFields;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<TraversalDirection>("direction", direction));
    }
}

public class DirectionalFocusAction : IntentAction<DirectionalFocusIntent>
{
    internal virtual bool _isForTextField { get; private set; } = default!;

    public DirectionalFocusAction()
    {
        _isForTextField = false;
    }

    public static DirectionalFocusAction CreateForTextField()
    {
        var __instance = new DirectionalFocusAction();
        __instance._isForTextField = true;
        return __instance;
    }

    public override object? invoke(DirectionalFocusIntent intent, BuildContext? context = null)
    {
        if (!intent.ignoreTextFields || !_isForTextField)
        {
            Focus_managerLibrary.primaryFocus!.focusInDirection(intent.direction);
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class ExcludeFocusTraversal : StatelessWidget
{
    public virtual bool excluding { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public ExcludeFocusTraversal(Key? key = null, bool excluding = true, Widget child = default!)
        : base(key: key)
    {
        this.excluding = excluding;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return new Focus(
            canRequestFocus: false,
            skipTraversal: true,
            includeSemantics: false,
            descendantsAreTraversable: !excluding,
            child: child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
