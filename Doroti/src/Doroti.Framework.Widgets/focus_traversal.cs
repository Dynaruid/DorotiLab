// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/focus_traversal.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public static partial class Focus_traversalLibrary
{
    internal static BuildContext? _getAncestor(BuildContext context, long count = 1)
    {
        BuildContext? target = default!;
        context.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) =>
        {
            count--;
            if ((count == 0L))
            {
                target = DartRuntimePrimitives.ConvertValue<BuildContext>(ancestor);
                return false;
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        return target;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public delegate void TraversalRequestFocusCallback(FocusNode node, ScrollPositionAlignmentPolicy? alignmentPolicy = null, double? alignment = null, Duration? duration = null, global::Doroti.Framework.Animation.Curve? curve = null);

internal class _FocusTraversalGroupInfo__focus_traversal
{
    public virtual FocusNode? groupNode { get; private set; }
    public virtual FocusTraversalPolicy policy { get; private set; } = default!;
    public virtual List<FocusNode> members { get; private set; } = default!;

    internal _FocusTraversalGroupInfo__focus_traversal(_FocusTraversalGroupNode__focus_traversal? group, FocusTraversalPolicy? defaultPolicy = null, List<FocusNode>? members = null)
    {
        this.groupNode = group;
        this.policy = ((group?.policy ?? defaultPolicy) ?? new ReadingOrderTraversalPolicy());
        this.members = (members ?? new List<FocusNode>());
    }

}

public enum TraversalDirection
{
    up,
    right,
    down,
    left
}

public enum TraversalEdgeBehavior
{
    closedLoop,
    leaveDorotiView,
    parentScope,
    stop
}

public abstract class FocusTraversalPolicy : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual TraversalRequestFocusCallback requestFocusCallback { get; private set; } = default!;

    protected FocusTraversalPolicy(TraversalRequestFocusCallback? requestFocusCallback = null)
    {
        this.requestFocusCallback = ((requestFocusCallback ?? (TraversalRequestFocusCallback)defaultTraversalRequestFocusCallback));
    }

    public static void defaultTraversalRequestFocusCallback(FocusNode node, ScrollPositionAlignmentPolicy? alignmentPolicy = null, double? alignment = null, Duration? duration = null, global::Doroti.Framework.Animation.Curve? curve = null)
    {
        node.requestFocus();
        DartRuntimePrimitives.Ignore(Scrollable.ensureVisible(((FocusNode)node).context!, alignment: (alignment ?? 1), alignmentPolicy: (alignmentPolicy ?? ScrollPositionAlignmentPolicy.@explicit), duration: (duration ?? Duration.zero), curve: (curve ?? global::Doroti.Framework.Animation.Curves.ease)));
    }

    internal virtual bool _requestTabTraversalFocus(FocusNode node, ScrollPositionAlignmentPolicy? alignmentPolicy = null, double? alignment = null, Duration? duration = null, global::Doroti.Framework.Animation.Curve? curve = null, bool forward = default!)
    {
        if ((node is FocusScopeNode))
        {
            FocusScopeNode node__as9364 = (FocusScopeNode)node;
            if ((((FocusScopeNode)((FocusScopeNode)node__as9364)).focusedChild is not null))
            {
                return _requestTabTraversalFocus(((FocusScopeNode)((FocusScopeNode)node__as9364)).focusedChild!, alignmentPolicy: alignmentPolicy, alignment: alignment, duration: duration, curve: curve, forward: forward);
            }
            List<FocusNode> sortedChildren = ((List<FocusNode>)(object?)FocusTraversalPolicy._sortAllDescendants(((FocusScopeNode)node__as9364), ((FocusScopeNode)node__as9364)));
            if (System.Linq.Enumerable.Any(sortedChildren))
            {
                _requestTabTraversalFocus((forward ? sortedChildren.First() : sortedChildren.Last()), alignmentPolicy: alignmentPolicy, alignment: alignment, duration: duration, curve: curve, forward: forward);
                return true;
            }
        }
        bool nodeHadPrimaryFocus = ((FocusNode)node).hasPrimaryFocus;
        this.requestFocusCallback(node, alignmentPolicy: alignmentPolicy, alignment: alignment, duration: duration, curve: curve);
        return !nodeHadPrimaryFocus;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual FocusNode? findFirstFocus(FocusNode currentNode, bool ignoreCurrentFocus = false)
    {
        return ((FocusNode?)(object?)_findInitialFocus(currentNode, ignoreCurrentFocus: ignoreCurrentFocus));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual FocusNode findLastFocus(FocusNode currentNode, bool ignoreCurrentFocus = false)
    {
        return ((FocusNode)(object?)_findInitialFocus(currentNode, fromEnd: true, ignoreCurrentFocus: ignoreCurrentFocus));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual FocusNode _findInitialFocus(FocusNode currentNode, bool fromEnd = false, bool ignoreCurrentFocus = false)
    {
        FocusScopeNode scope = ((FocusNode)currentNode).nearestScope!;
        FocusNode? candidate = ((FocusScopeNode)scope).focusedChild;
        if ((ignoreCurrentFocus || ((candidate is null) && System.Linq.Enumerable.Any(scope.descendants))))
        {
            IEnumerable<FocusNode> sorted = FocusTraversalPolicy._sortAllDescendants(scope, currentNode).where(((node) => FocusTraversalPolicy._canRequestTraversalFocus(node)));
            if (!System.Linq.Enumerable.Any(sorted))
            {
                candidate = null;
            }
            else
            {
                candidate = (fromEnd ? sorted.Last() : sorted.First());
            }
        }
        candidate ??= currentNode;
        return candidate;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract FocusNode? findFirstFocusInDirection(FocusNode currentNode, TraversalDirection direction);
    public virtual void invalidateScopeData(FocusScopeNode node)
    {
    }

    public virtual void changedScope(FocusNode? node = null, FocusScopeNode? oldScope = null)
    {
    }

    public virtual bool next(FocusNode currentNode) => _moveFocus(currentNode, forward: true);
    public virtual bool previous(FocusNode currentNode) => _moveFocus(currentNode, forward: false);
    public abstract bool inDirection(FocusNode currentNode, TraversalDirection direction);
    public abstract IEnumerable<FocusNode> sortDescendants(IEnumerable<FocusNode> descendants, FocusNode currentNode);
    internal static bool _canRequestTraversalFocus(FocusNode node)
    {
        return (((FocusNode)node).canRequestFocus && !((FocusNode)node).skipTraversal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static IEnumerable<FocusNode> _getDescendantsWithoutExpandingScope(FocusNode node)
    {
        var result = new List<FocusNode>();
        foreach (FocusNode child in ((FocusNode)node).children)
        {
            result.Add(child);
            if ((child is not FocusScopeNode))
            {
                result.AddRange(FocusTraversalPolicy._getDescendantsWithoutExpandingScope(child));
            }
        }
        return ((IEnumerable<FocusNode>)(object?)result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static DartMap<FocusNode?, _FocusTraversalGroupInfo__focus_traversal> _findGroups(FocusScopeNode scope, _FocusTraversalGroupNode__focus_traversal? scopeGroupNode, FocusNode currentNode)
    {
        FocusTraversalPolicy defaultPolicyLocal = (scopeGroupNode?.policy ?? new ReadingOrderTraversalPolicy());
        var groups = new DartMap<FocusNode?, _FocusTraversalGroupInfo__focus_traversal>();
        foreach (FocusNode node in FocusTraversalPolicy._getDescendantsWithoutExpandingScope(scope))
        {
            _FocusTraversalGroupNode__focus_traversal? groupNode = ((_FocusTraversalGroupNode__focus_traversal?)(object?)FocusTraversalGroup._getGroupNode(node));
            if ((object.Equals(node, groupNode)))
            {
                _FocusTraversalGroupNode__focus_traversal? parentGroup = ((_FocusTraversalGroupNode__focus_traversal?)(object?)FocusTraversalGroup._getGroupNode(groupNode!.parent!));
                groups.putIfAbsent(parentGroup, () => new _FocusTraversalGroupInfo__focus_traversal(parentGroup, members: new List<FocusNode>(), defaultPolicy: defaultPolicyLocal));
                DartRuntimePrimitives.Assert(() => !groups.GetValueOrDefault(DartRuntimePrimitives.RequireReference(parentGroup))!.members.Contains(node));
                groups.GetValueOrDefault(DartRuntimePrimitives.RequireReference(parentGroup))!.members.Add(groupNode);
                continue;
            }
            if (((object.Equals(node, currentNode)) || ((((FocusNode)node).canRequestFocus && !((FocusNode)node).skipTraversal))))
            {
                groups.putIfAbsent(groupNode, () => new _FocusTraversalGroupInfo__focus_traversal(groupNode, members: new List<FocusNode>(), defaultPolicy: defaultPolicyLocal));
                DartRuntimePrimitives.Assert(() => !groups.GetValueOrDefault(DartRuntimePrimitives.RequireReference(groupNode))!.members.Contains(node));
                groups.GetValueOrDefault(DartRuntimePrimitives.RequireReference(groupNode))!.members.Add(node);
            }
        }
        return groups;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static List<FocusNode> _sortAllDescendants(FocusScopeNode scope, FocusNode currentNode)
    {
        _FocusTraversalGroupNode__focus_traversal? scopeGroupNode = ((_FocusTraversalGroupNode__focus_traversal?)(object?)FocusTraversalGroup._getGroupNode(scope));
        DartMap<FocusNode?, _FocusTraversalGroupInfo__focus_traversal> groups = ((DartMap<FocusNode?, _FocusTraversalGroupInfo__focus_traversal>)(object?)FocusTraversalPolicy._findGroups(scope, scopeGroupNode, currentNode));
        foreach (FocusNode? key in groups.Keys)
        {
            List<FocusNode> sortedMembers = groups.GetValueOrDefault(DartRuntimePrimitives.RequireReference(key))!.policy.sortDescendants(groups.GetValueOrDefault(DartRuntimePrimitives.RequireReference(key))!.members.Cast<FocusNode>(), currentNode).ToList().ToList();
            groups.GetValueOrDefault(DartRuntimePrimitives.RequireReference(key))!.members.Clear();
            groups.GetValueOrDefault(DartRuntimePrimitives.RequireReference(key))!.members.AddRange(sortedMembers.Cast<FocusNode>());
        }
        var sortedDescendants = new List<FocusNode>();
        void visitGroups(_FocusTraversalGroupInfo__focus_traversal info)
        {
            foreach (FocusNode nodeLocal in ((_FocusTraversalGroupInfo__focus_traversal)info).members)
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
        if ((System.Linq.Enumerable.Any(groups) && groups.ContainsKey(scopeGroupNode)))
        {
            visitGroups(groups.GetValueOrDefault(DartRuntimePrimitives.RequireReference(scopeGroupNode))!);
        }
        sortedDescendants.removeWhere(((node) =>
        {
            return ((!object.Equals(node, currentNode)) && !FocusTraversalPolicy._canRequestTraversalFocus(node));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        DartRuntimePrimitives.Assert(() =>
            {
                HashSet<FocusNode> differenceLocal = sortedDescendants.toSet().difference<FocusNode>(((FocusScopeNode)scope).traversalDescendants.toSet());
                if (!FocusTraversalPolicy._canRequestTraversalFocus(currentNode))
                {
                    DartRuntimePrimitives.Assert(() => (!System.Linq.Enumerable.Any(differenceLocal) || (((checked((long)(differenceLocal.Count)) == 1L) && differenceLocal.Contains(currentNode)))), () => (object?)"Difference between sorted descendants and FocusScopeNode.traversalDescendants contains " + $"something other than the current skipped node. This is the difference: {differenceLocal}");
                    return true;
                }
                DartRuntimePrimitives.Assert(() => !System.Linq.Enumerable.Any(differenceLocal), () => (object?)"Sorted descendants contains different nodes than FocusScopeNode.traversalDescendants would. " + $"These are the different nodes: {differenceLocal}");
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return sortedDescendants;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _moveFocus(FocusNode currentNode, bool forward)
    {
        FocusScopeNode nearestScopeLocal = ((FocusNode)currentNode).nearestScope!;
        invalidateScopeData(nearestScopeLocal);
        FocusNode? focusedChildLocal = ((FocusScopeNode)nearestScopeLocal).focusedChild;
        if ((focusedChildLocal is null))
        {
            FocusNode? firstFocus = (forward ? findFirstFocus(currentNode) : findLastFocus(currentNode));
            if ((firstFocus is not null))
            {
                return _requestTabTraversalFocus(firstFocus, alignmentPolicy: (forward ? ScrollPositionAlignmentPolicy.keepVisibleAtEnd : ScrollPositionAlignmentPolicy.keepVisibleAtStart), forward: forward);
            }
        }
        focusedChildLocal ??= nearestScopeLocal;
        List<FocusNode> sortedNodes = ((List<FocusNode>)(object?)FocusTraversalPolicy._sortAllDescendants(nearestScopeLocal, focusedChildLocal));
        DartRuntimePrimitives.Assert(() => sortedNodes.Contains(focusedChildLocal));
        if ((forward && (object.Equals(focusedChildLocal, sortedNodes.Last()))))
        {
            switch (((FocusScopeNode)nearestScopeLocal).traversalEdgeBehavior)
            {
                case TraversalEdgeBehavior.leaveDorotiView:
                    {
                        focusedChildLocal.unfocus();
                        return false;
                    }
                case TraversalEdgeBehavior.parentScope:
                    {
                        FocusScopeNode? parentScopeLocal = nearestScopeLocal.enclosingScope;
                        if (((parentScopeLocal is not null) && (!object.Equals(parentScopeLocal, FocusManager.instance.rootScope))))
                        {
                            focusedChildLocal.unfocus();
                            parentScopeLocal.nextFocus();
                            return (!object.Equals(((FocusNode)focusedChildLocal).enclosingScope?.focusedChild, focusedChildLocal));
                        }
                        return _requestTabTraversalFocus(sortedNodes.First(), alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd, forward: forward);
                    }
                case TraversalEdgeBehavior.closedLoop:
                    {
                        return _requestTabTraversalFocus(sortedNodes.First(), alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd, forward: forward);
                    }
                case TraversalEdgeBehavior.stop:
                    {
                        return false;
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
        if ((!forward && (object.Equals(focusedChildLocal, sortedNodes.First()))))
        {
            switch (((FocusScopeNode)nearestScopeLocal).traversalEdgeBehavior)
            {
                case TraversalEdgeBehavior.leaveDorotiView:
                    {
                        focusedChildLocal.unfocus();
                        return false;
                    }
                case TraversalEdgeBehavior.parentScope:
                    {
                        FocusScopeNode? parentScopeAlternate = nearestScopeLocal.enclosingScope;
                        if (((parentScopeAlternate is not null) && (!object.Equals(parentScopeAlternate, FocusManager.instance.rootScope))))
                        {
                            focusedChildLocal.unfocus();
                            parentScopeAlternate.previousFocus();
                            return (!object.Equals(((FocusNode)focusedChildLocal).enclosingScope?.focusedChild, focusedChildLocal));
                        }
                        return _requestTabTraversalFocus(sortedNodes.Last(), alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart, forward: forward);
                    }
                case TraversalEdgeBehavior.closedLoop:
                    {
                        return _requestTabTraversalFocus(sortedNodes.Last(), alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart, forward: forward);
                    }
                case TraversalEdgeBehavior.stop:
                    {
                        return false;
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
        IEnumerable<FocusNode> maybeFlipped = (forward ? sortedNodes : System.Linq.Enumerable.Reverse(sortedNodes));
        FocusNode? previousNode = default!;
        foreach (var node in maybeFlipped)
        {
            if ((object.Equals(previousNode, focusedChildLocal)))
            {
                return _requestTabTraversalFocus(node, alignmentPolicy: (forward ? ScrollPositionAlignmentPolicy.keepVisibleAtEnd : ScrollPositionAlignmentPolicy.keepVisibleAtStart), forward: forward);
            }
            previousNode = node;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
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
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
    }

}

public class _DirectionalPolicyDataEntry__focus_traversal
{
    public virtual TraversalDirection direction { get; private set; } = default!;
    public virtual FocusNode node { get; private set; } = default!;

    internal _DirectionalPolicyDataEntry__focus_traversal(TraversalDirection direction, FocusNode node)
    {
        this.direction = direction;
        this.node = node;
    }

}

public class _DirectionalPolicyData__focus_traversal
{
    public virtual List<_DirectionalPolicyDataEntry__focus_traversal> history { get; private set; } = default!;

    internal _DirectionalPolicyData__focus_traversal(List<_DirectionalPolicyDataEntry__focus_traversal> history)
    {
        this.history = history;
    }

}

public interface DirectionalFocusTraversalPolicyMixin
{
    DartMap<FocusScopeNode, _DirectionalPolicyData__focus_traversal> _policyData { get; }

    public void invalidateScopeData(FocusScopeNode node);
    public void changedScope(FocusNode? node = null, FocusScopeNode? oldScope = null);
    public FocusNode? findFirstFocusInDirection(FocusNode currentNode, TraversalDirection direction);
    public FocusNode? _findNextFocusInDirection(FocusNode focusedChild, IEnumerable<FocusNode> traversalDescendants, TraversalDirection direction, bool forward = true);
    public static long _verticalCompare(Offset target, Offset a, Offset b)
    {
        return ((a.dy - target.dy)).abs().CompareTo(((b.dy - target.dy)).abs());
    }
    public static long _horizontalCompare(Offset target, Offset a, Offset b)
    {
        return ((a.dx - target.dx)).abs().CompareTo(((b.dx - target.dx)).abs());
    }
    public static IEnumerable<FocusNode> _sortByDistancePreferVertical(Offset target, IEnumerable<FocusNode> nodes)
    {
        List<FocusNode> sorted = nodes.ToList().ToList();
        global::Doroti.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted, compare: ((nodeA, nodeB) =>
        {
            global::Doroti.Ui.Offset a = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)((FocusNode)nodeA).rect).center));
            global::Doroti.Ui.Offset b = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)((FocusNode)nodeB).rect).center));
            long vertical = DirectionalFocusTraversalPolicyMixin._verticalCompare(target, a, b);
            if ((vertical == 0L))
            {
                return DirectionalFocusTraversalPolicyMixin._horizontalCompare(target, a, b);
            }
            return vertical;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        return ((IEnumerable<FocusNode>)(object?)sorted);
    }
    public static IEnumerable<FocusNode> _sortByDistancePreferHorizontal(Offset target, IEnumerable<FocusNode> nodes)
    {
        List<FocusNode> sorted = nodes.ToList().ToList();
        global::Doroti.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted, compare: ((nodeA, nodeB) =>
        {
            global::Doroti.Ui.Offset a = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)((FocusNode)nodeA).rect).center));
            global::Doroti.Ui.Offset b = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)((FocusNode)nodeB).rect).center));
            long horizontal = DirectionalFocusTraversalPolicyMixin._horizontalCompare(target, a, b);
            if ((horizontal == 0L))
            {
                return DirectionalFocusTraversalPolicyMixin._verticalCompare(target, a, b);
            }
            return horizontal;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        return ((IEnumerable<FocusNode>)(object?)sorted);
    }
    public static long _verticalCompareClosestEdge(Offset target, Rect a, Rect b)
    {
        double aCoord = ((((a.top - target.dy)).abs() < ((a.bottom - target.dy)).abs()) ? a.top : a.bottom);
        double bCoord = ((((b.top - target.dy)).abs() < ((b.bottom - target.dy)).abs()) ? b.top : b.bottom);
        return ((aCoord - target.dy)).abs().CompareTo(((bCoord - target.dy)).abs());
    }
    public static long _horizontalCompareClosestEdge(Offset target, Rect a, Rect b)
    {
        double aCoord = ((((a.left - target.dx)).abs() < ((a.right - target.dx)).abs()) ? a.left : a.right);
        double bCoord = ((((b.left - target.dx)).abs() < ((b.right - target.dx)).abs()) ? b.left : b.right);
        return ((aCoord - target.dx)).abs().CompareTo(((bCoord - target.dx)).abs());
    }
    public static IEnumerable<FocusNode> _sortClosestEdgesByDistancePreferHorizontal(Offset target, IEnumerable<FocusNode> nodes)
    {
        List<FocusNode> sorted = nodes.ToList().ToList();
        global::Doroti.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted, compare: ((nodeA, nodeB) =>
        {
            long horizontal = DirectionalFocusTraversalPolicyMixin._horizontalCompareClosestEdge(target, ((FocusNode)nodeA).rect, ((FocusNode)nodeB).rect);
            if ((horizontal == 0L))
            {
                return DirectionalFocusTraversalPolicyMixin._verticalCompare(target, ((Offset)((dynamic)((FocusNode)nodeA).rect).center), ((Offset)((dynamic)((FocusNode)nodeB).rect).center));
            }
            return horizontal;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        return ((IEnumerable<FocusNode>)(object?)sorted);
    }
    public static IEnumerable<FocusNode> _sortClosestEdgesByDistancePreferVertical(Offset target, IEnumerable<FocusNode> nodes)
    {
        List<FocusNode> sorted = nodes.ToList().ToList();
        global::Doroti.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted, compare: ((nodeA, nodeB) =>
        {
            long vertical = DirectionalFocusTraversalPolicyMixin._verticalCompareClosestEdge(target, ((FocusNode)nodeA).rect, ((FocusNode)nodeB).rect);
            if ((vertical == 0L))
            {
                return DirectionalFocusTraversalPolicyMixin._horizontalCompare(target, ((Offset)((dynamic)((FocusNode)nodeA).rect).center), ((Offset)((dynamic)((FocusNode)nodeB).rect).center));
            }
            return vertical;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        return ((IEnumerable<FocusNode>)(object?)sorted);
    }
    public IEnumerable<FocusNode> _sortAndFilterHorizontally(TraversalDirection direction, Rect target, IEnumerable<FocusNode> nodes, bool forward = true);
    public IEnumerable<FocusNode> _sortAndFilterVertically(TraversalDirection direction, Rect target, IEnumerable<FocusNode> nodes, bool forward = true);
    public bool _popPolicyDataIfNeeded(TraversalDirection direction, FocusScopeNode nearestScope, FocusNode focusedChild, _FocusTraversalGroupNode__focus_traversal? groupNode);
    public void _pushPolicyData(TraversalDirection direction, FocusScopeNode nearestScope, FocusNode focusedChild);
    public bool _requestTraversalFocusInDirection(FocusNode currentNode, FocusNode node, FocusScopeNode nearestScope, TraversalDirection direction, _FocusTraversalGroupNode__focus_traversal? groupNode);
    public void _requestFocus(FocusNode node, _FocusTraversalGroupNode__focus_traversal? groupNode, ScrollPositionAlignmentPolicy? alignmentPolicy = null, double? alignment = null, Duration? duration = null, global::Doroti.Framework.Animation.Curve? curve = null);
    public bool _onEdgeForDirection(FocusNode currentNode, FocusNode focusedChild, _FocusTraversalGroupNode__focus_traversal? groupNode, TraversalDirection direction, FocusScopeNode? scope = null);
    public bool inDirection(FocusNode currentNode, TraversalDirection direction);
}

public class WidgetOrderTraversalPolicy : FocusTraversalPolicy, DirectionalFocusTraversalPolicyMixin
{
    public virtual DartMap<FocusScopeNode, _DirectionalPolicyData__focus_traversal> _policyData { get; set; } = new DartMap<FocusScopeNode, _DirectionalPolicyData__focus_traversal>();

    public WidgetOrderTraversalPolicy(TraversalRequestFocusCallback? requestFocusCallback = null) : base(requestFocusCallback: requestFocusCallback)
    {
    }

    public override IEnumerable<FocusNode> sortDescendants(IEnumerable<FocusNode> descendants, FocusNode currentNode) => descendants;
    public override void invalidateScopeData(FocusScopeNode node)
    {
        base.invalidateScopeData(node);
        this._policyData.remove(node);
    }

    public override void changedScope(FocusNode? node = null, FocusScopeNode? oldScope = null)
    {
        base.changedScope(node: node, oldScope: oldScope);
        if ((oldScope is not null))
        {
            this._policyData.GetValueOrDefault(oldScope)?.history.removeWhere(((entry) =>
            {
                return (object.Equals(((_DirectionalPolicyDataEntry__focus_traversal)entry).node, node));
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
        }
    }

    public override FocusNode? findFirstFocusInDirection(FocusNode currentNode, TraversalDirection direction)
    {
        IEnumerable<FocusNode> nodes = ((FocusNode)currentNode).nearestScope!.traversalDescendants;
        List<FocusNode> sorted = nodes.ToList().ToList();
        var (vertical, first) = (direction switch { TraversalDirection.up => (((bool, bool))((true, false))), TraversalDirection.down => (((bool, bool))((true, true))), TraversalDirection.left => (((bool, bool))((false, false))), TraversalDirection.right => (((bool, bool))((false, true))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted, compare: ((a, b) =>
        {
            if (vertical)
            {
                if (first)
                {
                    return ((FocusNode)a).rect.top.CompareTo(((FocusNode)b).rect.top);
                }
                else
                {
                    return ((FocusNode)b).rect.bottom.CompareTo(((FocusNode)a).rect.bottom);
                }
            }
            else
            {
                if (first)
                {
                    return ((FocusNode)a).rect.left.CompareTo(((FocusNode)b).rect.left);
                }
                else
                {
                    return ((FocusNode)b).rect.right.CompareTo(((FocusNode)a).rect.right);
                }
            }
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        return sorted.FirstOrDefault();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual FocusNode? _findNextFocusInDirection(FocusNode focusedChild, IEnumerable<FocusNode> traversalDescendants, TraversalDirection direction, bool forward = true)
    {
        switch (direction)
        {
            case TraversalDirection.down:
            case TraversalDirection.up:
                {
                    IEnumerable<FocusNode> eligibleNodes = ((IEnumerable<FocusNode>)(object?)_sortAndFilterVertically(direction, ((FocusNode)focusedChild).rect, traversalDescendants.Cast<FocusNode>(), forward: forward));
                    if (!System.Linq.Enumerable.Any(eligibleNodes))
                    {
                        break;
                    }
                    ScrollableState? focusedScrollable = ((ScrollableState?)(object?)Scrollable.maybeOf(((FocusNode)focusedChild).context!, axis: global::Doroti.Framework.Painting.Axis.vertical));
                    if ((focusedScrollable is not null))
                    {
                        IEnumerable<FocusNode> filteredEligibleNodes = eligibleNodes.where(((node) => (object.Equals(Scrollable.maybeOf(((FocusNode)node).context!, axis: global::Doroti.Framework.Painting.Axis.vertical), focusedScrollable))));
                        if (System.Linq.Enumerable.Any(filteredEligibleNodes))
                        {
                            eligibleNodes = filteredEligibleNodes;
                        }
                    }
                    if ((object.Equals(direction, TraversalDirection.up)))
                    {
                        eligibleNodes = System.Linq.Enumerable.Reverse(eligibleNodes.ToList());
                    }
                    var band = global::Doroti.Ui.Rect.fromLTRB(((FocusNode)focusedChild).rect.left, -double.PositiveInfinity, ((FocusNode)focusedChild).rect.right, double.PositiveInfinity);
                    IEnumerable<FocusNode> inBand = eligibleNodes.where(((node) => !((FocusNode)node).rect.intersect(band).isEmpty));
                    if (System.Linq.Enumerable.Any(inBand))
                    {
                        if (forward)
                        {
                            return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBand.Cast<FocusNode>()).First();
                        }
                        return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBand.Cast<FocusNode>()).Last();
                    }
                    if (forward)
                    {
                        return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodes.Cast<FocusNode>()).First();
                    }
                    return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodes.Cast<FocusNode>()).Last();
                }
            case TraversalDirection.right:
            case TraversalDirection.left:
                {
                    IEnumerable<FocusNode> eligibleNodesLocal = ((IEnumerable<FocusNode>)(object?)_sortAndFilterHorizontally(direction, ((FocusNode)focusedChild).rect, traversalDescendants.Cast<FocusNode>(), forward: forward));
                    if (!System.Linq.Enumerable.Any(eligibleNodesLocal))
                    {
                        break;
                    }
                    ScrollableState? focusedScrollableLocal = ((ScrollableState?)(object?)Scrollable.maybeOf(((FocusNode)focusedChild).context!, axis: global::Doroti.Framework.Painting.Axis.horizontal));
                    if ((focusedScrollableLocal is not null))
                    {
                        IEnumerable<FocusNode> filteredEligibleNodesLocal = eligibleNodesLocal.where(((node) => (object.Equals(Scrollable.maybeOf(((FocusNode)node).context!, axis: global::Doroti.Framework.Painting.Axis.horizontal), focusedScrollableLocal))));
                        if (System.Linq.Enumerable.Any(filteredEligibleNodesLocal))
                        {
                            eligibleNodesLocal = filteredEligibleNodesLocal;
                        }
                    }
                    if ((object.Equals(direction, TraversalDirection.left)))
                    {
                        eligibleNodesLocal = System.Linq.Enumerable.Reverse(eligibleNodesLocal.ToList());
                    }
                    var bandLocal = global::Doroti.Ui.Rect.fromLTRB(-double.PositiveInfinity, ((FocusNode)focusedChild).rect.top, double.PositiveInfinity, ((FocusNode)focusedChild).rect.bottom);
                    IEnumerable<FocusNode> inBandLocal = eligibleNodesLocal.where(((node) => !((FocusNode)node).rect.intersect(bandLocal).isEmpty));
                    if (System.Linq.Enumerable.Any(inBandLocal))
                    {
                        if (forward)
                        {
                            return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBandLocal.Cast<FocusNode>()).First();
                        }
                        return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBandLocal.Cast<FocusNode>()).Last();
                    }
                    if (forward)
                    {
                        return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodesLocal.Cast<FocusNode>()).First();
                    }
                    return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodesLocal.Cast<FocusNode>()).Last();
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        return ((FocusNode)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual IEnumerable<FocusNode> _sortAndFilterHorizontally(TraversalDirection direction, Rect target, IEnumerable<FocusNode> nodes, bool forward = true)
    {
        DartRuntimePrimitives.Assert(() => ((object.Equals(direction, TraversalDirection.left)) || (object.Equals(direction, TraversalDirection.right))));
        List<FocusNode> sorted = nodes.where((direction switch { TraversalDirection.left => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dx <= target.left) : (((dynamic)((FocusNode)node).rect).center.dx >= target.left))))), TraversalDirection.right => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dx >= target.right) : (((dynamic)((FocusNode)node).rect).center.dx <= target.right))))), TraversalDirection.up => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), TraversalDirection.down => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })).ToList().ToList();
        global::Doroti.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted, compare: ((a, b) => ((Offset)((dynamic)((FocusNode)a).rect).center).dx.CompareTo(((Offset)((dynamic)((FocusNode)b).rect).center).dx)));
        return ((IEnumerable<FocusNode>)(object?)sorted);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual IEnumerable<FocusNode> _sortAndFilterVertically(TraversalDirection direction, Rect target, IEnumerable<FocusNode> nodes, bool forward = true)
    {
        DartRuntimePrimitives.Assert(() => ((object.Equals(direction, TraversalDirection.up)) || (object.Equals(direction, TraversalDirection.down))));
        List<FocusNode> sorted = nodes.where((direction switch { TraversalDirection.up => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dy <= target.top) : (((dynamic)((FocusNode)node).rect).center.dy >= target.top))))), TraversalDirection.down => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dy >= target.bottom) : (((dynamic)((FocusNode)node).rect).center.dy <= target.bottom))))), TraversalDirection.left => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), TraversalDirection.right => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })).ToList().ToList();
        global::Doroti.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted, compare: ((a, b) => ((Offset)((dynamic)((FocusNode)a).rect).center).dy.CompareTo(((Offset)((dynamic)((FocusNode)b).rect).center).dy)));
        return ((IEnumerable<FocusNode>)(object?)sorted);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _popPolicyDataIfNeeded(TraversalDirection direction, FocusScopeNode nearestScope, FocusNode focusedChild, _FocusTraversalGroupNode__focus_traversal? groupNode)
    {
        _DirectionalPolicyData__focus_traversal? policyData = this._policyData.GetValueOrDefault(nearestScope);
        if ((((policyData is not null) && System.Linq.Enumerable.Any(((_DirectionalPolicyData__focus_traversal)policyData).history)) && (!object.Equals(((_DirectionalPolicyData__focus_traversal)policyData).history.First().direction, direction))))
        {
            if ((((_DirectionalPolicyData__focus_traversal)policyData).history.Last().node.parent is null))
            {
                invalidateScopeData(nearestScope);
                return false;
            }
            bool popOrInvalidate(TraversalDirection direction)
            {
                FocusNode lastNode = ((_DirectionalPolicyData__focus_traversal)policyData).history.removeLast<_DirectionalPolicyDataEntry__focus_traversal>().node;
                if ((!object.Equals(Scrollable.maybeOf(((FocusNode)lastNode).context!), Scrollable.maybeOf(global::Doroti.Framework.Widgets.Focus_managerLibrary.primaryFocus!.context!))))
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
                _requestFocus(lastNode, alignmentPolicy: DartRuntimePrimitives.RequireValue(alignmentPolicyLocal), groupNode: groupNode);
                return true;
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            switch (direction)
            {
                case TraversalDirection.down:
                case TraversalDirection.up:
                    {
                        switch (((_DirectionalPolicyData__focus_traversal)policyData).history.First().direction)
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
                        switch (((_DirectionalPolicyData__focus_traversal)policyData).history.First().direction)
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
        if (((policyData is not null) && !System.Linq.Enumerable.Any(((_DirectionalPolicyData__focus_traversal)policyData).history)))
        {
            invalidateScopeData(nearestScope);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _pushPolicyData(TraversalDirection direction, FocusScopeNode nearestScope, FocusNode focusedChild)
    {
        _DirectionalPolicyData__focus_traversal? policyData = this._policyData.GetValueOrDefault(nearestScope);
        var newEntry = new _DirectionalPolicyDataEntry__focus_traversal(node: focusedChild, direction: direction);
        if ((policyData is not null))
        {
            ((_DirectionalPolicyData__focus_traversal)policyData).history.Add(newEntry);
        }
        else
        {
            this._policyData[nearestScope] = new _DirectionalPolicyData__focus_traversal(history: new List<_DirectionalPolicyDataEntry__focus_traversal> { newEntry });
        }
    }

    public virtual bool _requestTraversalFocusInDirection(FocusNode currentNode, FocusNode node, FocusScopeNode nearestScope, TraversalDirection direction, _FocusTraversalGroupNode__focus_traversal? groupNode)
    {
        if ((node is FocusScopeNode))
        {
            if ((((FocusScopeNode)node).focusedChild is not null))
            {
                return _requestTraversalFocusInDirection(currentNode, ((FocusScopeNode)node).focusedChild!, DartRuntimePrimitives.ConvertValue<FocusScopeNode>(node), direction, groupNode);
            }
            FocusNode firstNode = (findFirstFocusInDirection(node, direction) ?? currentNode);
            switch (direction)
            {
                case TraversalDirection.up:
                case TraversalDirection.left:
                    {
                        _requestFocus(firstNode, groupNode, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart);
                        break;
                    }
                case TraversalDirection.right:
                case TraversalDirection.down:
                    {
                        _requestFocus(firstNode, groupNode, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd);
                        break;
                    }
            }
            return true;
        }
        bool nodeHadPrimaryFocus = ((FocusNode)node).hasPrimaryFocus;
        switch (direction)
        {
            case TraversalDirection.up:
            case TraversalDirection.left:
                {
                    _requestFocus(node, groupNode, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart);
                    break;
                }
            case TraversalDirection.right:
            case TraversalDirection.down:
                {
                    _requestFocus(node, groupNode, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd);
                    break;
                }
        }
        return !nodeHadPrimaryFocus;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _requestFocus(FocusNode node, _FocusTraversalGroupNode__focus_traversal? groupNode, ScrollPositionAlignmentPolicy? alignmentPolicy = null, double? alignment = null, Duration? duration = null, global::Doroti.Framework.Animation.Curve? curve = null)
    {
        groupNode?.lastRequestedFocus = node;
        this.requestFocusCallback(node, alignmentPolicy: alignmentPolicy, alignment: alignment, duration: duration, curve: curve);
    }

    public virtual bool _onEdgeForDirection(FocusNode currentNode, FocusNode focusedChild, _FocusTraversalGroupNode__focus_traversal? groupNode, TraversalDirection direction, FocusScopeNode? scope = null)
    {
        FocusScopeNode nearestScopeLocal = (scope ?? ((FocusNode)currentNode).nearestScope!);
        FocusNode? found = default!;
        switch (((FocusScopeNode)nearestScopeLocal).directionalTraversalEdgeBehavior)
        {
            case TraversalEdgeBehavior.leaveDorotiView:
                {
                    focusedChild.unfocus();
                    return false;
                }
            case TraversalEdgeBehavior.parentScope:
                {
                    FocusScopeNode? parentScopeLocal = nearestScopeLocal.enclosingScope;
                    if (((parentScopeLocal is not null) && (!object.Equals(parentScopeLocal, FocusManager.instance.rootScope))))
                    {
                        invalidateScopeData(nearestScopeLocal);
                        nearestScopeLocal = parentScopeLocal;
                        invalidateScopeData(nearestScopeLocal);
                        found = _findNextFocusInDirection(focusedChild, ((FocusScopeNode)nearestScopeLocal).traversalDescendants.Cast<FocusNode>(), direction);
                        if ((found is null))
                        {
                            return _onEdgeForDirection(currentNode, focusedChild, groupNode, direction, scope: nearestScopeLocal);
                        }
                    }
                    else
                    {
                        found = _findNextFocusInDirection(focusedChild, ((FocusScopeNode)nearestScopeLocal).traversalDescendants.Cast<FocusNode>(), direction, forward: false);
                    }
                    break;
                }
            case TraversalEdgeBehavior.closedLoop:
                {
                    found = _findNextFocusInDirection(focusedChild, ((FocusScopeNode)nearestScopeLocal).traversalDescendants.Cast<FocusNode>(), direction, forward: false);
                    break;
                }
            case TraversalEdgeBehavior.stop:
                {
                    return false;
                }
        }
        if ((found is not null))
        {
            return _requestTraversalFocusInDirection(currentNode, found, DartRuntimePrimitives.ConvertValue<FocusScopeNode>(nearestScopeLocal), direction, groupNode);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool inDirection(FocusNode currentNode, TraversalDirection direction)
    {
        _FocusTraversalGroupNode__focus_traversal? groupNodeLocal = ((_FocusTraversalGroupNode__focus_traversal?)(object?)FocusTraversalGroup._getGroupNode(currentNode));
        FocusScopeNode nearestScopeLocal = ((FocusNode)currentNode).nearestScope!;
        FocusNode? focusedChildLocal = ((FocusScopeNode)nearestScopeLocal).focusedChild;
        if ((focusedChildLocal is null))
        {
            FocusNode firstFocus = (findFirstFocusInDirection(currentNode, direction) ?? currentNode);
            switch (direction)
            {
                case TraversalDirection.up:
                case TraversalDirection.left:
                    {
                        _requestFocus(firstFocus, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart, groupNode: groupNodeLocal);
                        break;
                    }
                case TraversalDirection.right:
                case TraversalDirection.down:
                    {
                        _requestFocus(firstFocus, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd, groupNode: groupNodeLocal);
                        break;
                    }
            }
            return true;
        }
        if (_popPolicyDataIfNeeded(direction, nearestScopeLocal, focusedChildLocal, groupNodeLocal))
        {
            return true;
        }
        FocusNode? found = ((FocusNode?)(object?)_findNextFocusInDirection(focusedChildLocal, ((FocusScopeNode)nearestScopeLocal).traversalDescendants.Cast<FocusNode>(), direction));
        if ((found is not null))
        {
            _pushPolicyData(direction, nearestScopeLocal, focusedChildLocal);
            return _requestTraversalFocusInDirection(currentNode, found, DartRuntimePrimitives.ConvertValue<FocusScopeNode>(nearestScopeLocal), direction, groupNodeLocal);
        }
        return _onEdgeForDirection(currentNode, focusedChildLocal, groupNodeLocal, direction);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _ReadingOrderSortData__focus_traversal : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual TextDirection? directionality { get; private set; }
    public virtual Rect rect { get; private set; } = default!;
    public virtual FocusNode node { get; private set; } = default!;
    internal virtual List<Directionality>? _directionalAncestors { get; set; } = default;

    internal _ReadingOrderSortData__focus_traversal(FocusNode node)
    {
        this.node = node;
        this.rect = ((FocusNode)node).rect;
        this.directionality = _ReadingOrderSortData__focus_traversal._findDirectionality(((FocusNode)node).context!);
    }

    internal static global::Doroti.Ui.TextDirection? _findDirectionality(BuildContext context)
    {
        return ((TextDirection?)((dynamic)context.getInheritedWidgetOfExactType<Directionality>())?.textDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.TextDirection? commonDirectionalityOf(List<_ReadingOrderSortData__focus_traversal> list)
    {
        IEnumerable<HashSet<Directionality>> allAncestors = list.map<_ReadingOrderSortData__focus_traversal, HashSet<Directionality>>(((member) => ((_ReadingOrderSortData__focus_traversal)member).directionalAncestors.toSet()));
        HashSet<Directionality>? common = default!;
        foreach (var ancestorSet in allAncestors)
        {
            common ??= ancestorSet;
            common = common.intersection(ancestorSet);
        }
        if (!System.Linq.Enumerable.Any(common!))
        {
            return list.First().directionality;
        }
        return ((TextDirection)((dynamic)list.First().directionalAncestors.firstWhere(common.Contains)).textDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static void sortWithDirectionality(List<_ReadingOrderSortData__focus_traversal> list, TextDirection directionality)
    {
        global::Doroti.Framework.Foundation.CollectionsLibrary.mergeSort<_ReadingOrderSortData__focus_traversal>(list, compare: ((a, b) => (directionality switch { TextDirection.ltr => ((_ReadingOrderSortData__focus_traversal)a).rect.left.CompareTo(((_ReadingOrderSortData__focus_traversal)b).rect.left), TextDirection.rtl => ((_ReadingOrderSortData__focus_traversal)b).rect.right.CompareTo(((_ReadingOrderSortData__focus_traversal)a).rect.right), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })));
    }

    public virtual IEnumerable<Directionality> directionalAncestors
    {
        get
        {
            List<Directionality> getDirectionalityAncestors(BuildContext context)
            {
                var result = new List<Directionality>();
                InheritedElement? directionalityElement = ((InheritedElement?)(object?)context.getElementForInheritedWidgetOfExactType<Directionality>());
                while ((directionalityElement is not null))
                {
                    result.Add(((Directionality?)(object?)directionalityElement.widget)!);
                    directionalityElement = Focus_traversalLibrary._getAncestor(directionalityElement)?.getElementForInheritedWidgetOfExactType<Directionality>();
                }
                return result;
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            _directionalAncestors ??= getDirectionalityAncestors(((FocusNode)this.node).context!);
            return ((IEnumerable<Directionality>)(object?)this._directionalAncestors!);
            return default!;
        }
    }
    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.TextDirection>("directionality", this.directionality));
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("name", ((FocusNode)this.node).debugLabel, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Rect>("rect", this.rect));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
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
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ReadingOrderDirectionalGroupData__focus_traversal : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual List<_ReadingOrderSortData__focus_traversal> members { get; private set; } = default!;
    internal virtual Rect? _rect { get; set; } = default;
    internal virtual List<Directionality>? _memberAncestors { get; set; } = default;

    internal _ReadingOrderDirectionalGroupData__focus_traversal(List<_ReadingOrderSortData__focus_traversal> members)
    {
        this.members = members;
    }

    public virtual global::Doroti.Ui.TextDirection? directionality => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.TextDirection>(this.members.First().directionality);
    public virtual global::Doroti.Ui.Rect rect
    {
        get
        {
            if ((this._rect is null))
            {
                foreach (global::Doroti.Ui.Rect rectLocal in this.members.map<_ReadingOrderSortData__focus_traversal, Rect>(((data) => ((_ReadingOrderSortData__focus_traversal)data).rect)))
                {
                    _rect ??= rectLocal;
                    _rect = DartRuntimePrimitives.RequireValue(this._rect).expandToInclude(rectLocal);
                }
            }
            return DartRuntimePrimitives.RequireValue(this._rect);
            return default!;
        }
    }
    public virtual List<Directionality> memberAncestors
    {
        get
        {
            if ((this._memberAncestors is null))
            {
                _memberAncestors = new List<Directionality>();
                foreach (_ReadingOrderSortData__focus_traversal member in this.members)
                {
                    this._memberAncestors!.AddRange(((_ReadingOrderSortData__focus_traversal)member).directionalAncestors.Cast<Directionality>());
                }
            }
            return this._memberAncestors!;
            return default!;
        }
    }
    public static void sortWithDirectionality(List<_ReadingOrderDirectionalGroupData__focus_traversal> list, TextDirection directionality)
    {
        global::Doroti.Framework.Foundation.CollectionsLibrary.mergeSort<_ReadingOrderDirectionalGroupData__focus_traversal>(list, compare: ((a, b) => (directionality switch { TextDirection.ltr => ((_ReadingOrderDirectionalGroupData__focus_traversal)a).rect.left.CompareTo(((_ReadingOrderDirectionalGroupData__focus_traversal)b).rect.left), TextDirection.rtl => ((_ReadingOrderDirectionalGroupData__focus_traversal)b).rect.right.CompareTo(((_ReadingOrderDirectionalGroupData__focus_traversal)a).rect.right), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.TextDirection>("directionality", this.directionality));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Rect>("rect", this.rect));
        properties.add(new global::Doroti.Framework.Foundation.IterableProperty<string>("members", this.members.map<_ReadingOrderSortData__focus_traversal, string>(((member) =>
        {
            return $"\"{((_ReadingOrderSortData__focus_traversal)member).node.debugLabel}\"({((_ReadingOrderSortData__focus_traversal)member).rect})";
            throw new InvalidOperationException("Dart closure completed without a value.");
        })).Cast<string>()));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
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
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ReadingOrderTraversalPolicy : FocusTraversalPolicy, DirectionalFocusTraversalPolicyMixin
{
    public virtual DartMap<FocusScopeNode, _DirectionalPolicyData__focus_traversal> _policyData { get; set; } = new DartMap<FocusScopeNode, _DirectionalPolicyData__focus_traversal>();

    public ReadingOrderTraversalPolicy(TraversalRequestFocusCallback? requestFocusCallback = null) : base(requestFocusCallback: requestFocusCallback)
    {
    }

    public static IEnumerable<FocusNode> sort(IEnumerable<FocusNode> nodes)
    {
        if ((nodes.Count() <= 1L))
        {
            return nodes;
        }
        var data = new List<_ReadingOrderSortData__focus_traversal>();
        var sortedList = new List<FocusNode>();
        var unplaced = data;
        _ReadingOrderSortData__focus_traversal current = ((_ReadingOrderSortData__focus_traversal)(object?)ReadingOrderTraversalPolicy._pickNext(unplaced));
        sortedList.Add(((_ReadingOrderSortData__focus_traversal)current).node);
        unplaced.Remove(current);
        while (System.Linq.Enumerable.Any(unplaced))
        {
            _ReadingOrderSortData__focus_traversal next = ((_ReadingOrderSortData__focus_traversal)(object?)ReadingOrderTraversalPolicy._pickNext(unplaced));
            current = next;
            sortedList.Add(((_ReadingOrderSortData__focus_traversal)current).node);
            unplaced.Remove(current);
        }
        return ((IEnumerable<FocusNode>)(object?)sortedList);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static List<_ReadingOrderDirectionalGroupData__focus_traversal> _collectDirectionalityGroups(IEnumerable<_ReadingOrderSortData__focus_traversal> candidates)
    {
        global::Doroti.Ui.TextDirection? currentDirection = candidates.First().directionality;
        var currentGroup = new List<_ReadingOrderSortData__focus_traversal>();
        var result = new List<_ReadingOrderDirectionalGroupData__focus_traversal>();
        foreach (var candidate in candidates)
        {
            if ((object.Equals(((_ReadingOrderSortData__focus_traversal)candidate).directionality, currentDirection)))
            {
                currentGroup.Add(candidate);
                continue;
            }
            currentDirection = ((_ReadingOrderSortData__focus_traversal)candidate).directionality;
            result.Add(new _ReadingOrderDirectionalGroupData__focus_traversal(currentGroup));
            currentGroup = new List<_ReadingOrderSortData__focus_traversal> { candidate };
        }
        if (System.Linq.Enumerable.Any(currentGroup))
        {
            result.Add(new _ReadingOrderDirectionalGroupData__focus_traversal(currentGroup));
        }
        foreach (var bandGroup in result)
        {
            if ((checked((long)(((_ReadingOrderDirectionalGroupData__focus_traversal)bandGroup).members.Count)) == 1L))
            {
                continue;
            }
            _ReadingOrderSortData__focus_traversal.sortWithDirectionality(((_ReadingOrderDirectionalGroupData__focus_traversal)bandGroup).members, DartRuntimePrimitives.RequireValue(((_ReadingOrderDirectionalGroupData__focus_traversal)bandGroup).directionality));
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static _ReadingOrderSortData__focus_traversal _pickNext(List<_ReadingOrderSortData__focus_traversal> candidates)
    {
        global::Doroti.Framework.Foundation.CollectionsLibrary.mergeSort<_ReadingOrderSortData__focus_traversal>(candidates, compare: ((a, b) => ((_ReadingOrderSortData__focus_traversal)a).rect.top.CompareTo(((_ReadingOrderSortData__focus_traversal)b).rect.top)));
        _ReadingOrderSortData__focus_traversal topmost = candidates.First();
        List<_ReadingOrderSortData__focus_traversal> inBand(_ReadingOrderSortData__focus_traversal current, IEnumerable<_ReadingOrderSortData__focus_traversal> candidates)
        {
            var band = global::Doroti.Ui.Rect.fromLTRB(double.NegativeInfinity, ((_ReadingOrderSortData__focus_traversal)current).rect.top, double.PositiveInfinity, ((_ReadingOrderSortData__focus_traversal)current).rect.bottom);
            return candidates.where(((item) =>
            {
                return !((_ReadingOrderSortData__focus_traversal)item).rect.intersect(band).isEmpty;
                throw new InvalidOperationException("Dart closure completed without a value.");
            })).ToList();
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        List<_ReadingOrderSortData__focus_traversal> inBandOfTop = inBand(topmost, candidates.Cast<_ReadingOrderSortData__focus_traversal>()).ToList();
        DartRuntimePrimitives.Assert(() => (((_ReadingOrderSortData__focus_traversal)topmost).rect.isEmpty || System.Linq.Enumerable.Any(inBandOfTop)));
        if ((checked((long)(inBandOfTop.Count)) <= 1L))
        {
            return topmost;
        }
        global::Doroti.Ui.TextDirection? nearestCommonDirectionality = _ReadingOrderSortData__focus_traversal.commonDirectionalityOf(inBandOfTop);
        _ReadingOrderSortData__focus_traversal.sortWithDirectionality(inBandOfTop, DartRuntimePrimitives.RequireValue(nearestCommonDirectionality));
        List<_ReadingOrderDirectionalGroupData__focus_traversal> bandGroups = ((List<_ReadingOrderDirectionalGroupData__focus_traversal>)(object?)ReadingOrderTraversalPolicy._collectDirectionalityGroups(inBandOfTop.Cast<_ReadingOrderSortData__focus_traversal>()));
        if ((checked((long)(bandGroups.Count)) == 1L))
        {
            return bandGroups.First().members.First();
        }
        _ReadingOrderDirectionalGroupData__focus_traversal.sortWithDirectionality(bandGroups, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(nearestCommonDirectionality)));
        return bandGroups.First().members.First();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IEnumerable<FocusNode> sortDescendants(IEnumerable<FocusNode> descendants, FocusNode currentNode) => ReadingOrderTraversalPolicy.sort(descendants.Cast<FocusNode>());
    public override void invalidateScopeData(FocusScopeNode node)
    {
        base.invalidateScopeData(node);
        this._policyData.remove(node);
    }

    public override void changedScope(FocusNode? node = null, FocusScopeNode? oldScope = null)
    {
        base.changedScope(node: node, oldScope: oldScope);
        if ((oldScope is not null))
        {
            this._policyData.GetValueOrDefault(oldScope)?.history.removeWhere(((entry) =>
            {
                return (object.Equals(((_DirectionalPolicyDataEntry__focus_traversal)entry).node, node));
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
        }
    }

    public override FocusNode? findFirstFocusInDirection(FocusNode currentNode, TraversalDirection direction)
    {
        IEnumerable<FocusNode> nodes = ((FocusNode)currentNode).nearestScope!.traversalDescendants;
        List<FocusNode> sorted = nodes.ToList().ToList();
        var (vertical, first) = (direction switch { TraversalDirection.up => (((bool, bool))((true, false))), TraversalDirection.down => (((bool, bool))((true, true))), TraversalDirection.left => (((bool, bool))((false, false))), TraversalDirection.right => (((bool, bool))((false, true))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted, compare: ((a, b) =>
        {
            if (vertical)
            {
                if (first)
                {
                    return ((FocusNode)a).rect.top.CompareTo(((FocusNode)b).rect.top);
                }
                else
                {
                    return ((FocusNode)b).rect.bottom.CompareTo(((FocusNode)a).rect.bottom);
                }
            }
            else
            {
                if (first)
                {
                    return ((FocusNode)a).rect.left.CompareTo(((FocusNode)b).rect.left);
                }
                else
                {
                    return ((FocusNode)b).rect.right.CompareTo(((FocusNode)a).rect.right);
                }
            }
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        return sorted.FirstOrDefault();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual FocusNode? _findNextFocusInDirection(FocusNode focusedChild, IEnumerable<FocusNode> traversalDescendants, TraversalDirection direction, bool forward = true)
    {
        switch (direction)
        {
            case TraversalDirection.down:
            case TraversalDirection.up:
                {
                    IEnumerable<FocusNode> eligibleNodes = ((IEnumerable<FocusNode>)(object?)_sortAndFilterVertically(direction, ((FocusNode)focusedChild).rect, traversalDescendants.Cast<FocusNode>(), forward: forward));
                    if (!System.Linq.Enumerable.Any(eligibleNodes))
                    {
                        break;
                    }
                    ScrollableState? focusedScrollable = ((ScrollableState?)(object?)Scrollable.maybeOf(((FocusNode)focusedChild).context!, axis: global::Doroti.Framework.Painting.Axis.vertical));
                    if ((focusedScrollable is not null))
                    {
                        IEnumerable<FocusNode> filteredEligibleNodes = eligibleNodes.where(((node) => (object.Equals(Scrollable.maybeOf(((FocusNode)node).context!, axis: global::Doroti.Framework.Painting.Axis.vertical), focusedScrollable))));
                        if (System.Linq.Enumerable.Any(filteredEligibleNodes))
                        {
                            eligibleNodes = filteredEligibleNodes;
                        }
                    }
                    if ((object.Equals(direction, TraversalDirection.up)))
                    {
                        eligibleNodes = System.Linq.Enumerable.Reverse(eligibleNodes.ToList());
                    }
                    var band = global::Doroti.Ui.Rect.fromLTRB(((FocusNode)focusedChild).rect.left, -double.PositiveInfinity, ((FocusNode)focusedChild).rect.right, double.PositiveInfinity);
                    IEnumerable<FocusNode> inBand = eligibleNodes.where(((node) => !((FocusNode)node).rect.intersect(band).isEmpty));
                    if (System.Linq.Enumerable.Any(inBand))
                    {
                        if (forward)
                        {
                            return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBand.Cast<FocusNode>()).First();
                        }
                        return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBand.Cast<FocusNode>()).Last();
                    }
                    if (forward)
                    {
                        return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodes.Cast<FocusNode>()).First();
                    }
                    return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodes.Cast<FocusNode>()).Last();
                }
            case TraversalDirection.right:
            case TraversalDirection.left:
                {
                    IEnumerable<FocusNode> eligibleNodesLocal = ((IEnumerable<FocusNode>)(object?)_sortAndFilterHorizontally(direction, ((FocusNode)focusedChild).rect, traversalDescendants.Cast<FocusNode>(), forward: forward));
                    if (!System.Linq.Enumerable.Any(eligibleNodesLocal))
                    {
                        break;
                    }
                    ScrollableState? focusedScrollableLocal = ((ScrollableState?)(object?)Scrollable.maybeOf(((FocusNode)focusedChild).context!, axis: global::Doroti.Framework.Painting.Axis.horizontal));
                    if ((focusedScrollableLocal is not null))
                    {
                        IEnumerable<FocusNode> filteredEligibleNodesLocal = eligibleNodesLocal.where(((node) => (object.Equals(Scrollable.maybeOf(((FocusNode)node).context!, axis: global::Doroti.Framework.Painting.Axis.horizontal), focusedScrollableLocal))));
                        if (System.Linq.Enumerable.Any(filteredEligibleNodesLocal))
                        {
                            eligibleNodesLocal = filteredEligibleNodesLocal;
                        }
                    }
                    if ((object.Equals(direction, TraversalDirection.left)))
                    {
                        eligibleNodesLocal = System.Linq.Enumerable.Reverse(eligibleNodesLocal.ToList());
                    }
                    var bandLocal = global::Doroti.Ui.Rect.fromLTRB(-double.PositiveInfinity, ((FocusNode)focusedChild).rect.top, double.PositiveInfinity, ((FocusNode)focusedChild).rect.bottom);
                    IEnumerable<FocusNode> inBandLocal = eligibleNodesLocal.where(((node) => !((FocusNode)node).rect.intersect(bandLocal).isEmpty));
                    if (System.Linq.Enumerable.Any(inBandLocal))
                    {
                        if (forward)
                        {
                            return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBandLocal.Cast<FocusNode>()).First();
                        }
                        return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBandLocal.Cast<FocusNode>()).Last();
                    }
                    if (forward)
                    {
                        return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodesLocal.Cast<FocusNode>()).First();
                    }
                    return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodesLocal.Cast<FocusNode>()).Last();
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        return ((FocusNode)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual IEnumerable<FocusNode> _sortAndFilterHorizontally(TraversalDirection direction, Rect target, IEnumerable<FocusNode> nodes, bool forward = true)
    {
        DartRuntimePrimitives.Assert(() => ((object.Equals(direction, TraversalDirection.left)) || (object.Equals(direction, TraversalDirection.right))));
        List<FocusNode> sorted = nodes.where((direction switch { TraversalDirection.left => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dx <= target.left) : (((dynamic)((FocusNode)node).rect).center.dx >= target.left))))), TraversalDirection.right => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dx >= target.right) : (((dynamic)((FocusNode)node).rect).center.dx <= target.right))))), TraversalDirection.up => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), TraversalDirection.down => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })).ToList().ToList();
        global::Doroti.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted, compare: ((a, b) => ((Offset)((dynamic)((FocusNode)a).rect).center).dx.CompareTo(((Offset)((dynamic)((FocusNode)b).rect).center).dx)));
        return ((IEnumerable<FocusNode>)(object?)sorted);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual IEnumerable<FocusNode> _sortAndFilterVertically(TraversalDirection direction, Rect target, IEnumerable<FocusNode> nodes, bool forward = true)
    {
        DartRuntimePrimitives.Assert(() => ((object.Equals(direction, TraversalDirection.up)) || (object.Equals(direction, TraversalDirection.down))));
        List<FocusNode> sorted = nodes.where((direction switch { TraversalDirection.up => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dy <= target.top) : (((dynamic)((FocusNode)node).rect).center.dy >= target.top))))), TraversalDirection.down => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dy >= target.bottom) : (((dynamic)((FocusNode)node).rect).center.dy <= target.bottom))))), TraversalDirection.left => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), TraversalDirection.right => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })).ToList().ToList();
        global::Doroti.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted, compare: ((a, b) => ((Offset)((dynamic)((FocusNode)a).rect).center).dy.CompareTo(((Offset)((dynamic)((FocusNode)b).rect).center).dy)));
        return ((IEnumerable<FocusNode>)(object?)sorted);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _popPolicyDataIfNeeded(TraversalDirection direction, FocusScopeNode nearestScope, FocusNode focusedChild, _FocusTraversalGroupNode__focus_traversal? groupNode)
    {
        _DirectionalPolicyData__focus_traversal? policyData = this._policyData.GetValueOrDefault(nearestScope);
        if ((((policyData is not null) && System.Linq.Enumerable.Any(((_DirectionalPolicyData__focus_traversal)policyData).history)) && (!object.Equals(((_DirectionalPolicyData__focus_traversal)policyData).history.First().direction, direction))))
        {
            if ((((_DirectionalPolicyData__focus_traversal)policyData).history.Last().node.parent is null))
            {
                invalidateScopeData(nearestScope);
                return false;
            }
            bool popOrInvalidate(TraversalDirection direction)
            {
                FocusNode lastNode = ((_DirectionalPolicyData__focus_traversal)policyData).history.removeLast<_DirectionalPolicyDataEntry__focus_traversal>().node;
                if ((!object.Equals(Scrollable.maybeOf(((FocusNode)lastNode).context!), Scrollable.maybeOf(global::Doroti.Framework.Widgets.Focus_managerLibrary.primaryFocus!.context!))))
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
                _requestFocus(lastNode, alignmentPolicy: DartRuntimePrimitives.RequireValue(alignmentPolicyLocal), groupNode: groupNode);
                return true;
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            switch (direction)
            {
                case TraversalDirection.down:
                case TraversalDirection.up:
                    {
                        switch (((_DirectionalPolicyData__focus_traversal)policyData).history.First().direction)
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
                        switch (((_DirectionalPolicyData__focus_traversal)policyData).history.First().direction)
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
        if (((policyData is not null) && !System.Linq.Enumerable.Any(((_DirectionalPolicyData__focus_traversal)policyData).history)))
        {
            invalidateScopeData(nearestScope);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _pushPolicyData(TraversalDirection direction, FocusScopeNode nearestScope, FocusNode focusedChild)
    {
        _DirectionalPolicyData__focus_traversal? policyData = this._policyData.GetValueOrDefault(nearestScope);
        var newEntry = new _DirectionalPolicyDataEntry__focus_traversal(node: focusedChild, direction: direction);
        if ((policyData is not null))
        {
            ((_DirectionalPolicyData__focus_traversal)policyData).history.Add(newEntry);
        }
        else
        {
            this._policyData[nearestScope] = new _DirectionalPolicyData__focus_traversal(history: new List<_DirectionalPolicyDataEntry__focus_traversal> { newEntry });
        }
    }

    public virtual bool _requestTraversalFocusInDirection(FocusNode currentNode, FocusNode node, FocusScopeNode nearestScope, TraversalDirection direction, _FocusTraversalGroupNode__focus_traversal? groupNode)
    {
        if ((node is FocusScopeNode))
        {
            if ((((FocusScopeNode)node).focusedChild is not null))
            {
                return _requestTraversalFocusInDirection(currentNode, ((FocusScopeNode)node).focusedChild!, DartRuntimePrimitives.ConvertValue<FocusScopeNode>(node), direction, groupNode);
            }
            FocusNode firstNode = (findFirstFocusInDirection(node, direction) ?? currentNode);
            switch (direction)
            {
                case TraversalDirection.up:
                case TraversalDirection.left:
                    {
                        _requestFocus(firstNode, groupNode, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart);
                        break;
                    }
                case TraversalDirection.right:
                case TraversalDirection.down:
                    {
                        _requestFocus(firstNode, groupNode, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd);
                        break;
                    }
            }
            return true;
        }
        bool nodeHadPrimaryFocus = ((FocusNode)node).hasPrimaryFocus;
        switch (direction)
        {
            case TraversalDirection.up:
            case TraversalDirection.left:
                {
                    _requestFocus(node, groupNode, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart);
                    break;
                }
            case TraversalDirection.right:
            case TraversalDirection.down:
                {
                    _requestFocus(node, groupNode, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd);
                    break;
                }
        }
        return !nodeHadPrimaryFocus;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _requestFocus(FocusNode node, _FocusTraversalGroupNode__focus_traversal? groupNode, ScrollPositionAlignmentPolicy? alignmentPolicy = null, double? alignment = null, Duration? duration = null, global::Doroti.Framework.Animation.Curve? curve = null)
    {
        groupNode?.lastRequestedFocus = node;
        this.requestFocusCallback(node, alignmentPolicy: alignmentPolicy, alignment: alignment, duration: duration, curve: curve);
    }

    public virtual bool _onEdgeForDirection(FocusNode currentNode, FocusNode focusedChild, _FocusTraversalGroupNode__focus_traversal? groupNode, TraversalDirection direction, FocusScopeNode? scope = null)
    {
        FocusScopeNode nearestScopeLocal = (scope ?? ((FocusNode)currentNode).nearestScope!);
        FocusNode? found = default!;
        switch (((FocusScopeNode)nearestScopeLocal).directionalTraversalEdgeBehavior)
        {
            case TraversalEdgeBehavior.leaveDorotiView:
                {
                    focusedChild.unfocus();
                    return false;
                }
            case TraversalEdgeBehavior.parentScope:
                {
                    FocusScopeNode? parentScopeLocal = nearestScopeLocal.enclosingScope;
                    if (((parentScopeLocal is not null) && (!object.Equals(parentScopeLocal, FocusManager.instance.rootScope))))
                    {
                        invalidateScopeData(nearestScopeLocal);
                        nearestScopeLocal = parentScopeLocal;
                        invalidateScopeData(nearestScopeLocal);
                        found = _findNextFocusInDirection(focusedChild, ((FocusScopeNode)nearestScopeLocal).traversalDescendants.Cast<FocusNode>(), direction);
                        if ((found is null))
                        {
                            return _onEdgeForDirection(currentNode, focusedChild, groupNode, direction, scope: nearestScopeLocal);
                        }
                    }
                    else
                    {
                        found = _findNextFocusInDirection(focusedChild, ((FocusScopeNode)nearestScopeLocal).traversalDescendants.Cast<FocusNode>(), direction, forward: false);
                    }
                    break;
                }
            case TraversalEdgeBehavior.closedLoop:
                {
                    found = _findNextFocusInDirection(focusedChild, ((FocusScopeNode)nearestScopeLocal).traversalDescendants.Cast<FocusNode>(), direction, forward: false);
                    break;
                }
            case TraversalEdgeBehavior.stop:
                {
                    return false;
                }
        }
        if ((found is not null))
        {
            return _requestTraversalFocusInDirection(currentNode, found, DartRuntimePrimitives.ConvertValue<FocusScopeNode>(nearestScopeLocal), direction, groupNode);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool inDirection(FocusNode currentNode, TraversalDirection direction)
    {
        _FocusTraversalGroupNode__focus_traversal? groupNodeLocal = ((_FocusTraversalGroupNode__focus_traversal?)(object?)FocusTraversalGroup._getGroupNode(currentNode));
        FocusScopeNode nearestScopeLocal = ((FocusNode)currentNode).nearestScope!;
        FocusNode? focusedChildLocal = ((FocusScopeNode)nearestScopeLocal).focusedChild;
        if ((focusedChildLocal is null))
        {
            FocusNode firstFocus = (findFirstFocusInDirection(currentNode, direction) ?? currentNode);
            switch (direction)
            {
                case TraversalDirection.up:
                case TraversalDirection.left:
                    {
                        _requestFocus(firstFocus, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart, groupNode: groupNodeLocal);
                        break;
                    }
                case TraversalDirection.right:
                case TraversalDirection.down:
                    {
                        _requestFocus(firstFocus, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd, groupNode: groupNodeLocal);
                        break;
                    }
            }
            return true;
        }
        if (_popPolicyDataIfNeeded(direction, nearestScopeLocal, focusedChildLocal, groupNodeLocal))
        {
            return true;
        }
        FocusNode? found = ((FocusNode?)(object?)_findNextFocusInDirection(focusedChildLocal, ((FocusScopeNode)nearestScopeLocal).traversalDescendants.Cast<FocusNode>(), direction));
        if ((found is not null))
        {
            _pushPolicyData(direction, nearestScopeLocal, focusedChildLocal);
            return _requestTraversalFocusInDirection(currentNode, found, DartRuntimePrimitives.ConvertValue<FocusScopeNode>(nearestScopeLocal), direction, groupNodeLocal);
        }
        return _onEdgeForDirection(currentNode, focusedChildLocal, groupNodeLocal, direction);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class FocusOrder : global::Doroti.Framework.Foundation.Diagnosticable, IComparable<FocusOrder>
{

    protected FocusOrder()
    {
    }

    public virtual long compareTo(FocusOrder other)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(this.GetType(), DartRuntimePrimitives.RuntimeType(other))), () => (object?)"The sorting algorithm must not compare incomparable keys, since they don't " + $"know how to order themselves relative to each other. Comparing {this} with {other}");
        return doCompare(other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract long doCompare(FocusOrder other);
    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
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
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
    }

    public int CompareTo(FocusOrder? other) => checked((int)compareTo(other!));
}

public class NumericFocusOrder : FocusOrder
{
    public virtual double order { get; private set; } = default!;

    public NumericFocusOrder(double order)
    {
        this.order = order;
    }

    public override long doCompare(FocusOrder other) => this.order.CompareTo(((NumericFocusOrder)other).order);
    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("order", this.order));
    }

}

public class LexicalFocusOrder : FocusOrder
{
    public virtual string order { get; private set; } = default!;

    public LexicalFocusOrder(string order)
    {
        this.order = order;
    }

    public override long doCompare(FocusOrder other) => this.order.CompareTo(((LexicalFocusOrder)other).order);
    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("order", this.order));
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
    public virtual DartMap<FocusScopeNode, _DirectionalPolicyData__focus_traversal> _policyData { get; set; } = new DartMap<FocusScopeNode, _DirectionalPolicyData__focus_traversal>();

    public OrderedTraversalPolicy(FocusTraversalPolicy? secondary = null, TraversalRequestFocusCallback? requestFocusCallback = null) : base(requestFocusCallback: requestFocusCallback)
    {
        this.secondary = secondary;
    }

    public override IEnumerable<FocusNode> sortDescendants(IEnumerable<FocusNode> descendants, FocusNode currentNode)
    {
        FocusTraversalPolicy secondaryPolicy = (this.secondary ?? new ReadingOrderTraversalPolicy());
        IEnumerable<FocusNode> sortedDescendants = ((IEnumerable<FocusNode>)(object?)secondaryPolicy.sortDescendants(descendants.Cast<FocusNode>(), currentNode));
        var unordered = new List<FocusNode>();
        var ordered = new List<_OrderedFocusInfo__focus_traversal>();
        foreach (var nodeLocal in sortedDescendants)
        {
            FocusOrder? orderLocal = ((FocusOrder?)(object?)FocusTraversalOrder.maybeOf(((FocusNode)nodeLocal).context!));
            if ((orderLocal is not null))
            {
                ordered.Add(new _OrderedFocusInfo__focus_traversal(node: nodeLocal, order: orderLocal));
            }
            else
            {
                unordered.Add(nodeLocal);
            }
        }
        global::Doroti.Framework.Foundation.CollectionsLibrary.mergeSort<_OrderedFocusInfo__focus_traversal>(ordered, compare: ((a, b) =>
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(DartRuntimePrimitives.RuntimeType(((_OrderedFocusInfo__focus_traversal)a).order), DartRuntimePrimitives.RuntimeType(((_OrderedFocusInfo__focus_traversal)b).order))), () => (object?)$"When sorting nodes for determining focus order, the order ({((_OrderedFocusInfo__focus_traversal)a).order}) of " + $"node {((_OrderedFocusInfo__focus_traversal)a).node}, isn't the same type as the order ({((_OrderedFocusInfo__focus_traversal)b).order}) of {((_OrderedFocusInfo__focus_traversal)b).node}. " + "Incompatible order types can't be compared. Use a FocusTraversalGroup to group " + "similar orders together.");
            return ((_OrderedFocusInfo__focus_traversal)a).order.compareTo(((_OrderedFocusInfo__focus_traversal)b).order);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        return ordered.map<_OrderedFocusInfo__focus_traversal, FocusNode>(((info) => ((_OrderedFocusInfo__focus_traversal)info).node)).followedBy(unordered.Cast<FocusNode>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void invalidateScopeData(FocusScopeNode node)
    {
        base.invalidateScopeData(node);
        this._policyData.remove(node);
    }

    public override void changedScope(FocusNode? node = null, FocusScopeNode? oldScope = null)
    {
        base.changedScope(node: node, oldScope: oldScope);
        if ((oldScope is not null))
        {
            this._policyData.GetValueOrDefault(oldScope)?.history.removeWhere(((entry) =>
            {
                return (object.Equals(((_DirectionalPolicyDataEntry__focus_traversal)entry).node, node));
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
        }
    }

    public override FocusNode? findFirstFocusInDirection(FocusNode currentNode, TraversalDirection direction)
    {
        IEnumerable<FocusNode> nodes = ((FocusNode)currentNode).nearestScope!.traversalDescendants;
        List<FocusNode> sorted = nodes.ToList().ToList();
        var (vertical, first) = (direction switch { TraversalDirection.up => (((bool, bool))((true, false))), TraversalDirection.down => (((bool, bool))((true, true))), TraversalDirection.left => (((bool, bool))((false, false))), TraversalDirection.right => (((bool, bool))((false, true))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted, compare: ((a, b) =>
        {
            if (vertical)
            {
                if (first)
                {
                    return ((FocusNode)a).rect.top.CompareTo(((FocusNode)b).rect.top);
                }
                else
                {
                    return ((FocusNode)b).rect.bottom.CompareTo(((FocusNode)a).rect.bottom);
                }
            }
            else
            {
                if (first)
                {
                    return ((FocusNode)a).rect.left.CompareTo(((FocusNode)b).rect.left);
                }
                else
                {
                    return ((FocusNode)b).rect.right.CompareTo(((FocusNode)a).rect.right);
                }
            }
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        return sorted.FirstOrDefault();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual FocusNode? _findNextFocusInDirection(FocusNode focusedChild, IEnumerable<FocusNode> traversalDescendants, TraversalDirection direction, bool forward = true)
    {
        switch (direction)
        {
            case TraversalDirection.down:
            case TraversalDirection.up:
                {
                    IEnumerable<FocusNode> eligibleNodes = ((IEnumerable<FocusNode>)(object?)_sortAndFilterVertically(direction, ((FocusNode)focusedChild).rect, traversalDescendants.Cast<FocusNode>(), forward: forward));
                    if (!System.Linq.Enumerable.Any(eligibleNodes))
                    {
                        break;
                    }
                    ScrollableState? focusedScrollable = ((ScrollableState?)(object?)Scrollable.maybeOf(((FocusNode)focusedChild).context!, axis: global::Doroti.Framework.Painting.Axis.vertical));
                    if ((focusedScrollable is not null))
                    {
                        IEnumerable<FocusNode> filteredEligibleNodes = eligibleNodes.where(((node) => (object.Equals(Scrollable.maybeOf(((FocusNode)node).context!, axis: global::Doroti.Framework.Painting.Axis.vertical), focusedScrollable))));
                        if (System.Linq.Enumerable.Any(filteredEligibleNodes))
                        {
                            eligibleNodes = filteredEligibleNodes;
                        }
                    }
                    if ((object.Equals(direction, TraversalDirection.up)))
                    {
                        eligibleNodes = System.Linq.Enumerable.Reverse(eligibleNodes.ToList());
                    }
                    var band = global::Doroti.Ui.Rect.fromLTRB(((FocusNode)focusedChild).rect.left, -double.PositiveInfinity, ((FocusNode)focusedChild).rect.right, double.PositiveInfinity);
                    IEnumerable<FocusNode> inBand = eligibleNodes.where(((node) => !((FocusNode)node).rect.intersect(band).isEmpty));
                    if (System.Linq.Enumerable.Any(inBand))
                    {
                        if (forward)
                        {
                            return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBand.Cast<FocusNode>()).First();
                        }
                        return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBand.Cast<FocusNode>()).Last();
                    }
                    if (forward)
                    {
                        return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodes.Cast<FocusNode>()).First();
                    }
                    return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodes.Cast<FocusNode>()).Last();
                }
            case TraversalDirection.right:
            case TraversalDirection.left:
                {
                    IEnumerable<FocusNode> eligibleNodesLocal = ((IEnumerable<FocusNode>)(object?)_sortAndFilterHorizontally(direction, ((FocusNode)focusedChild).rect, traversalDescendants.Cast<FocusNode>(), forward: forward));
                    if (!System.Linq.Enumerable.Any(eligibleNodesLocal))
                    {
                        break;
                    }
                    ScrollableState? focusedScrollableLocal = ((ScrollableState?)(object?)Scrollable.maybeOf(((FocusNode)focusedChild).context!, axis: global::Doroti.Framework.Painting.Axis.horizontal));
                    if ((focusedScrollableLocal is not null))
                    {
                        IEnumerable<FocusNode> filteredEligibleNodesLocal = eligibleNodesLocal.where(((node) => (object.Equals(Scrollable.maybeOf(((FocusNode)node).context!, axis: global::Doroti.Framework.Painting.Axis.horizontal), focusedScrollableLocal))));
                        if (System.Linq.Enumerable.Any(filteredEligibleNodesLocal))
                        {
                            eligibleNodesLocal = filteredEligibleNodesLocal;
                        }
                    }
                    if ((object.Equals(direction, TraversalDirection.left)))
                    {
                        eligibleNodesLocal = System.Linq.Enumerable.Reverse(eligibleNodesLocal.ToList());
                    }
                    var bandLocal = global::Doroti.Ui.Rect.fromLTRB(-double.PositiveInfinity, ((FocusNode)focusedChild).rect.top, double.PositiveInfinity, ((FocusNode)focusedChild).rect.bottom);
                    IEnumerable<FocusNode> inBandLocal = eligibleNodesLocal.where(((node) => !((FocusNode)node).rect.intersect(bandLocal).isEmpty));
                    if (System.Linq.Enumerable.Any(inBandLocal))
                    {
                        if (forward)
                        {
                            return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBandLocal.Cast<FocusNode>()).First();
                        }
                        return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBandLocal.Cast<FocusNode>()).Last();
                    }
                    if (forward)
                    {
                        return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodesLocal.Cast<FocusNode>()).First();
                    }
                    return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodesLocal.Cast<FocusNode>()).Last();
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        return ((FocusNode)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual IEnumerable<FocusNode> _sortAndFilterHorizontally(TraversalDirection direction, Rect target, IEnumerable<FocusNode> nodes, bool forward = true)
    {
        DartRuntimePrimitives.Assert(() => ((object.Equals(direction, TraversalDirection.left)) || (object.Equals(direction, TraversalDirection.right))));
        List<FocusNode> sorted = nodes.where((direction switch { TraversalDirection.left => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dx <= target.left) : (((dynamic)((FocusNode)node).rect).center.dx >= target.left))))), TraversalDirection.right => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dx >= target.right) : (((dynamic)((FocusNode)node).rect).center.dx <= target.right))))), TraversalDirection.up => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), TraversalDirection.down => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })).ToList().ToList();
        global::Doroti.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted, compare: ((a, b) => ((Offset)((dynamic)((FocusNode)a).rect).center).dx.CompareTo(((Offset)((dynamic)((FocusNode)b).rect).center).dx)));
        return ((IEnumerable<FocusNode>)(object?)sorted);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual IEnumerable<FocusNode> _sortAndFilterVertically(TraversalDirection direction, Rect target, IEnumerable<FocusNode> nodes, bool forward = true)
    {
        DartRuntimePrimitives.Assert(() => ((object.Equals(direction, TraversalDirection.up)) || (object.Equals(direction, TraversalDirection.down))));
        List<FocusNode> sorted = nodes.where((direction switch { TraversalDirection.up => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dy <= target.top) : (((dynamic)((FocusNode)node).rect).center.dy >= target.top))))), TraversalDirection.down => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dy >= target.bottom) : (((dynamic)((FocusNode)node).rect).center.dy <= target.bottom))))), TraversalDirection.left => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), TraversalDirection.right => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })).ToList().ToList();
        global::Doroti.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted, compare: ((a, b) => ((Offset)((dynamic)((FocusNode)a).rect).center).dy.CompareTo(((Offset)((dynamic)((FocusNode)b).rect).center).dy)));
        return ((IEnumerable<FocusNode>)(object?)sorted);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _popPolicyDataIfNeeded(TraversalDirection direction, FocusScopeNode nearestScope, FocusNode focusedChild, _FocusTraversalGroupNode__focus_traversal? groupNode)
    {
        _DirectionalPolicyData__focus_traversal? policyData = this._policyData.GetValueOrDefault(nearestScope);
        if ((((policyData is not null) && System.Linq.Enumerable.Any(((_DirectionalPolicyData__focus_traversal)policyData).history)) && (!object.Equals(((_DirectionalPolicyData__focus_traversal)policyData).history.First().direction, direction))))
        {
            if ((((_DirectionalPolicyData__focus_traversal)policyData).history.Last().node.parent is null))
            {
                invalidateScopeData(nearestScope);
                return false;
            }
            bool popOrInvalidate(TraversalDirection direction)
            {
                FocusNode lastNode = ((_DirectionalPolicyData__focus_traversal)policyData).history.removeLast<_DirectionalPolicyDataEntry__focus_traversal>().node;
                if ((!object.Equals(Scrollable.maybeOf(((FocusNode)lastNode).context!), Scrollable.maybeOf(global::Doroti.Framework.Widgets.Focus_managerLibrary.primaryFocus!.context!))))
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
                _requestFocus(lastNode, alignmentPolicy: DartRuntimePrimitives.RequireValue(alignmentPolicyLocal), groupNode: groupNode);
                return true;
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            switch (direction)
            {
                case TraversalDirection.down:
                case TraversalDirection.up:
                    {
                        switch (((_DirectionalPolicyData__focus_traversal)policyData).history.First().direction)
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
                        switch (((_DirectionalPolicyData__focus_traversal)policyData).history.First().direction)
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
        if (((policyData is not null) && !System.Linq.Enumerable.Any(((_DirectionalPolicyData__focus_traversal)policyData).history)))
        {
            invalidateScopeData(nearestScope);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _pushPolicyData(TraversalDirection direction, FocusScopeNode nearestScope, FocusNode focusedChild)
    {
        _DirectionalPolicyData__focus_traversal? policyData = this._policyData.GetValueOrDefault(nearestScope);
        var newEntry = new _DirectionalPolicyDataEntry__focus_traversal(node: focusedChild, direction: direction);
        if ((policyData is not null))
        {
            ((_DirectionalPolicyData__focus_traversal)policyData).history.Add(newEntry);
        }
        else
        {
            this._policyData[nearestScope] = new _DirectionalPolicyData__focus_traversal(history: new List<_DirectionalPolicyDataEntry__focus_traversal> { newEntry });
        }
    }

    public virtual bool _requestTraversalFocusInDirection(FocusNode currentNode, FocusNode node, FocusScopeNode nearestScope, TraversalDirection direction, _FocusTraversalGroupNode__focus_traversal? groupNode)
    {
        if ((node is FocusScopeNode))
        {
            if ((((FocusScopeNode)node).focusedChild is not null))
            {
                return _requestTraversalFocusInDirection(currentNode, ((FocusScopeNode)node).focusedChild!, DartRuntimePrimitives.ConvertValue<FocusScopeNode>(node), direction, groupNode);
            }
            FocusNode firstNode = (findFirstFocusInDirection(node, direction) ?? currentNode);
            switch (direction)
            {
                case TraversalDirection.up:
                case TraversalDirection.left:
                    {
                        _requestFocus(firstNode, groupNode, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart);
                        break;
                    }
                case TraversalDirection.right:
                case TraversalDirection.down:
                    {
                        _requestFocus(firstNode, groupNode, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd);
                        break;
                    }
            }
            return true;
        }
        bool nodeHadPrimaryFocus = ((FocusNode)node).hasPrimaryFocus;
        switch (direction)
        {
            case TraversalDirection.up:
            case TraversalDirection.left:
                {
                    _requestFocus(node, groupNode, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart);
                    break;
                }
            case TraversalDirection.right:
            case TraversalDirection.down:
                {
                    _requestFocus(node, groupNode, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd);
                    break;
                }
        }
        return !nodeHadPrimaryFocus;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _requestFocus(FocusNode node, _FocusTraversalGroupNode__focus_traversal? groupNode, ScrollPositionAlignmentPolicy? alignmentPolicy = null, double? alignment = null, Duration? duration = null, global::Doroti.Framework.Animation.Curve? curve = null)
    {
        groupNode?.lastRequestedFocus = node;
        this.requestFocusCallback(node, alignmentPolicy: alignmentPolicy, alignment: alignment, duration: duration, curve: curve);
    }

    public virtual bool _onEdgeForDirection(FocusNode currentNode, FocusNode focusedChild, _FocusTraversalGroupNode__focus_traversal? groupNode, TraversalDirection direction, FocusScopeNode? scope = null)
    {
        FocusScopeNode nearestScopeLocal = (scope ?? ((FocusNode)currentNode).nearestScope!);
        FocusNode? found = default!;
        switch (((FocusScopeNode)nearestScopeLocal).directionalTraversalEdgeBehavior)
        {
            case TraversalEdgeBehavior.leaveDorotiView:
                {
                    focusedChild.unfocus();
                    return false;
                }
            case TraversalEdgeBehavior.parentScope:
                {
                    FocusScopeNode? parentScopeLocal = nearestScopeLocal.enclosingScope;
                    if (((parentScopeLocal is not null) && (!object.Equals(parentScopeLocal, FocusManager.instance.rootScope))))
                    {
                        invalidateScopeData(nearestScopeLocal);
                        nearestScopeLocal = parentScopeLocal;
                        invalidateScopeData(nearestScopeLocal);
                        found = _findNextFocusInDirection(focusedChild, ((FocusScopeNode)nearestScopeLocal).traversalDescendants.Cast<FocusNode>(), direction);
                        if ((found is null))
                        {
                            return _onEdgeForDirection(currentNode, focusedChild, groupNode, direction, scope: nearestScopeLocal);
                        }
                    }
                    else
                    {
                        found = _findNextFocusInDirection(focusedChild, ((FocusScopeNode)nearestScopeLocal).traversalDescendants.Cast<FocusNode>(), direction, forward: false);
                    }
                    break;
                }
            case TraversalEdgeBehavior.closedLoop:
                {
                    found = _findNextFocusInDirection(focusedChild, ((FocusScopeNode)nearestScopeLocal).traversalDescendants.Cast<FocusNode>(), direction, forward: false);
                    break;
                }
            case TraversalEdgeBehavior.stop:
                {
                    return false;
                }
        }
        if ((found is not null))
        {
            return _requestTraversalFocusInDirection(currentNode, found, DartRuntimePrimitives.ConvertValue<FocusScopeNode>(nearestScopeLocal), direction, groupNode);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool inDirection(FocusNode currentNode, TraversalDirection direction)
    {
        _FocusTraversalGroupNode__focus_traversal? groupNodeLocal = ((_FocusTraversalGroupNode__focus_traversal?)(object?)FocusTraversalGroup._getGroupNode(currentNode));
        FocusScopeNode nearestScopeLocal = ((FocusNode)currentNode).nearestScope!;
        FocusNode? focusedChildLocal = ((FocusScopeNode)nearestScopeLocal).focusedChild;
        if ((focusedChildLocal is null))
        {
            FocusNode firstFocus = (findFirstFocusInDirection(currentNode, direction) ?? currentNode);
            switch (direction)
            {
                case TraversalDirection.up:
                case TraversalDirection.left:
                    {
                        _requestFocus(firstFocus, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart, groupNode: groupNodeLocal);
                        break;
                    }
                case TraversalDirection.right:
                case TraversalDirection.down:
                    {
                        _requestFocus(firstFocus, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd, groupNode: groupNodeLocal);
                        break;
                    }
            }
            return true;
        }
        if (_popPolicyDataIfNeeded(direction, nearestScopeLocal, focusedChildLocal, groupNodeLocal))
        {
            return true;
        }
        FocusNode? found = ((FocusNode?)(object?)_findNextFocusInDirection(focusedChildLocal, ((FocusScopeNode)nearestScopeLocal).traversalDescendants.Cast<FocusNode>(), direction));
        if ((found is not null))
        {
            _pushPolicyData(direction, nearestScopeLocal, focusedChildLocal);
            return _requestTraversalFocusInDirection(currentNode, found, DartRuntimePrimitives.ConvertValue<FocusScopeNode>(nearestScopeLocal), direction, groupNodeLocal);
        }
        return _onEdgeForDirection(currentNode, focusedChildLocal, groupNodeLocal, direction);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class FocusTraversalOrder : InheritedWidget
{
    public virtual FocusOrder order { get; private set; } = default!;

    public FocusTraversalOrder(global::Doroti.Framework.Foundation.Key? key = null, FocusOrder order = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.order = order;
    }

    public static FocusOrder of(BuildContext context)
    {
        FocusTraversalOrder? marker = ((FocusTraversalOrder?)(object?)context.getInheritedWidgetOfExactType<FocusTraversalOrder>());
        DartRuntimePrimitives.Assert(() =>
            {
                if ((marker is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("FocusTraversalOrder.of() was called with a context that " + "does not contain a FocusTraversalOrder widget. No TraversalOrder widget " + "ancestor could be found starting from the context that was passed to " + "FocusTraversalOrder.of().\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return marker!.order;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static FocusOrder? maybeOf(BuildContext context)
    {
        FocusTraversalOrder? marker = ((FocusTraversalOrder?)(object?)context.getInheritedWidgetOfExactType<FocusTraversalOrder>());
        return marker?.order;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => false;
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<FocusOrder>("order", this.order));
    }

}

public class FocusTraversalGroup : StatefulWidget
{
    public virtual FocusTraversalPolicy policy { get; private set; } = default!;
    public virtual bool descendantsAreFocusable { get; private set; } = default!;
    public virtual bool descendantsAreTraversable { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;
    public virtual global::System.Action<FocusNode>? onFocusNodeCreated { get; private set; }
    public virtual FocusNode? parentNode { get; private set; }

    public FocusTraversalGroup(global::Doroti.Framework.Foundation.Key? key = null, FocusTraversalPolicy? policy = null, bool descendantsAreFocusable = true, bool descendantsAreTraversable = true, global::System.Action<FocusNode>? onFocusNodeCreated = null, FocusNode? parentNode = null, Widget child = default!) : base(key: key)
    {
        this.descendantsAreFocusable = descendantsAreFocusable;
        this.descendantsAreTraversable = descendantsAreTraversable;
        this.onFocusNodeCreated = onFocusNodeCreated;
        this.parentNode = parentNode;
        this.child = child;
        this.policy = (policy ?? new ReadingOrderTraversalPolicy());
    }

    public static FocusTraversalPolicy? maybeOfNode(FocusNode node)
    {
        return FocusTraversalGroup._getGroupNode(node)?.policy;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static _FocusTraversalGroupNode__focus_traversal? _getGroupNode(FocusNode node)
    {
        while ((((FocusNode)node).parent is not null))
        {
            if ((((FocusNode)node).context is null))
            {
                return ((_FocusTraversalGroupNode__focus_traversal)(object)null);
            }
            if ((node is _FocusTraversalGroupNode__focus_traversal))
            {
                _FocusTraversalGroupNode__focus_traversal node__as86847 = (_FocusTraversalGroupNode__focus_traversal)node;
                return ((_FocusTraversalGroupNode__focus_traversal)node__as86847);
            }
            node = ((FocusNode)node).parent!;
        }
        return ((_FocusTraversalGroupNode__focus_traversal)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static FocusTraversalPolicy of(BuildContext context)
    {
        FocusTraversalPolicy? policy = ((FocusTraversalPolicy?)(object?)FocusTraversalGroup.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((policy is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("Unable to find a Focus or FocusScope widget in the given context, or the FocusNode " + "from with the widget that was found is not associated with a FocusTraversalPolicy.\n" + "FocusTraversalGroup.of() was called with a context that does not contain a " + "Focus or FocusScope widget, or there was no FocusTraversalPolicy in effect.\n" + "This can happen if there is not a FocusTraversalGroup that defines the policy, " + "or if the context comes from a widget that is above the WidgetsApp, MaterialApp, " + "or CupertinoApp widget (those widgets introduce an implicit default policy) \n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return policy!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static FocusTraversalPolicy? maybeOf(BuildContext context)
    {
        FocusNode? node = ((FocusNode?)(object?)Focus.maybeOf(context, scopeOk: true, createDependency: false));
        if ((node is null))
        {
            return ((FocusTraversalPolicy)(object)null);
        }
        return ((FocusTraversalPolicy?)(object?)FocusTraversalGroup.maybeOfNode(node));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _FocusTraversalGroupState__focus_traversal());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<FocusTraversalPolicy>("policy", this.policy));
    }

}

public class _FocusTraversalGroupNode__focus_traversal : FocusNode
{
    public virtual FocusTraversalPolicy policy { get; set; } = default!;
    public virtual FocusNode? lastRequestedFocus { get; set; } = default;

    internal _FocusTraversalGroupNode__focus_traversal(string? debugLabel = null, FocusTraversalPolicy policy = default!) : base(debugLabel: debugLabel)
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
                __late_focusNode = new _FocusTraversalGroupNode__focus_traversal(debugLabel: "FocusTraversalGroup", policy: ((FocusTraversalGroup)this.widget).policy);
                __late_focusNode_initialized = true;
            }
            return __late_focusNode;
        }
    }

    public override void initState()
    {
        base.initState();
        FocusManager.instance.addListener(this._handleFocusChanged);
        ((FocusTraversalGroup)this.widget).onFocusNodeCreated?.Invoke(this.focusNode);
    }

    public override void dispose()
    {
        FocusManager.instance.removeListener(this._handleFocusChanged);
        this.focusNode.dispose();
        base.dispose();
    }

    public override void didUpdateWidget(FocusTraversalGroup oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((FocusTraversalGroup)oldWidget).policy, ((FocusTraversalGroup)this.widget).policy)))
        {
            this.focusNode.policy = ((FocusTraversalGroup)this.widget).policy;
        }
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new Focus(focusNode: this.focusNode, parentNode: ((FocusTraversalGroup)this.widget).parentNode, canRequestFocus: false, skipTraversal: true, includeSemantics: false, descendantsAreFocusable: ((FocusTraversalGroup)this.widget).descendantsAreFocusable, descendantsAreTraversable: ((FocusTraversalGroup)this.widget).descendantsAreTraversable, child: ((FocusTraversalGroup)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleFocusChanged()
    {
        FocusNode? primaryFocusLocal = FocusManager.instance.primaryFocus;
        FocusNode? lastRequestedFocusLocal = ((_FocusTraversalGroupNode__focus_traversal)this.focusNode).lastRequestedFocus;
        if ((lastRequestedFocusLocal is null))
        {
            return;
        }
        if ((!object.Equals(primaryFocusLocal, lastRequestedFocusLocal)))
        {
            FocusScopeNode? scope = primaryFocusLocal?.nearestScope;
            while ((scope is not null))
            {
                ((FocusTraversalGroup)this.widget).policy.invalidateScopeData(scope);
                scope = scope.enclosingScope;
            }
            this.focusNode.lastRequestedFocus = null;
        }
    }

}

public class RequestFocusIntent : Intent
{
    public virtual TraversalRequestFocusCallback requestFocusCallback { get; private set; } = default!;
    public virtual FocusNode focusNode { get; private set; } = default!;

    public RequestFocusIntent(FocusNode focusNode, TraversalRequestFocusCallback? requestFocusCallback = null)
    {
        this.focusNode = focusNode;
        this.requestFocusCallback = ((requestFocusCallback ?? (TraversalRequestFocusCallback)FocusTraversalPolicy.defaultTraversalRequestFocusCallback));
    }

}

public class RequestFocusAction : Action<RequestFocusIntent>
{
    public override object? invoke(RequestFocusIntent intent, BuildContext? context = null)
    {
        intent.requestFocusCallback(((RequestFocusIntent)intent).focusNode);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class NextFocusIntent : Intent
{
    public NextFocusIntent()
    {
    }

}

public class NextFocusAction : Action<NextFocusIntent>
{
    public override object? invoke(NextFocusIntent intent, BuildContext? context = null)
    {
        return global::Doroti.Framework.Widgets.Focus_managerLibrary.primaryFocus!.nextFocus();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override KeyEventResult toKeyEventResult(NextFocusIntent intent, object? invokeResult)
    {
        bool __invokeResult = DartRuntimePrimitives.ConvertValue<bool>(invokeResult);
        return (__invokeResult ? KeyEventResult.handled : KeyEventResult.skipRemainingHandlers);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class PreviousFocusIntent : Intent
{
    public PreviousFocusIntent()
    {
    }

}

public class PreviousFocusAction : Action<PreviousFocusIntent>
{
    public override object? invoke(PreviousFocusIntent intent, BuildContext? context = null)
    {
        return global::Doroti.Framework.Widgets.Focus_managerLibrary.primaryFocus!.previousFocus();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override KeyEventResult toKeyEventResult(PreviousFocusIntent intent, object? invokeResult)
    {
        bool __invokeResult = DartRuntimePrimitives.ConvertValue<bool>(invokeResult);
        return (__invokeResult ? KeyEventResult.handled : KeyEventResult.skipRemainingHandlers);
        throw new InvalidOperationException("Dart control flow completed without a value.");
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

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<TraversalDirection>("direction", this.direction));
    }

}

public class DirectionalFocusAction : Action<DirectionalFocusIntent>
{
    internal virtual bool _isForTextField { get; private set; } = default!;

    public DirectionalFocusAction()
    {
        this._isForTextField = false;
    }

    public static DirectionalFocusAction CreateForTextField()
    {
        var __instance = new DirectionalFocusAction();
        __instance._isForTextField = true;
        return __instance;
    }

    public override object? invoke(DirectionalFocusIntent intent, BuildContext? context = null)
    {
        if ((!((DirectionalFocusIntent)intent).ignoreTextFields || !this._isForTextField))
        {
            global::Doroti.Framework.Widgets.Focus_managerLibrary.primaryFocus!.focusInDirection(((DirectionalFocusIntent)intent).direction);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ExcludeFocusTraversal : StatelessWidget
{
    public virtual bool excluding { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public ExcludeFocusTraversal(global::Doroti.Framework.Foundation.Key? key = null, bool excluding = true, Widget child = default!) : base(key: key)
    {
        this.excluding = excluding;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new Focus(canRequestFocus: false, skipTraversal: true, includeSemantics: false, descendantsAreTraversable: !this.excluding, child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

