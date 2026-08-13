// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/focus_traversal.dart
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

public static partial class Focus_traversalLibrary
{
    internal static BuildContext? _getAncestor(BuildContext context, long count = 1)
    {
        BuildContext? target__1080 = default!;
        context.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) => {
count--;
if ((count == 0L))
{
    target__1080 = DartRuntimePrimitives.ConvertValue<BuildContext>(ancestor);
    return false;
}
return true;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        return target__1080;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public delegate void TraversalRequestFocusCallback(FocusNode node, ScrollPositionAlignmentPolicy? alignmentPolicy = null, double? alignment = null, Duration? duration = null, global::Doroti.Generated.Framework.Animation.Curve? curve = null);

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
    leaveFlutterView,
    parentScope,
    stop
}

public abstract class FocusTraversalPolicy : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual TraversalRequestFocusCallback requestFocusCallback { get; private set; } = default!;

    protected FocusTraversalPolicy(TraversalRequestFocusCallback? requestFocusCallback = null)
    {
        this.requestFocusCallback = ((requestFocusCallback ?? (TraversalRequestFocusCallback)defaultTraversalRequestFocusCallback));
    }

    public static void defaultTraversalRequestFocusCallback(FocusNode node, ScrollPositionAlignmentPolicy? alignmentPolicy = null, double? alignment = null, Duration? duration = null, global::Doroti.Generated.Framework.Animation.Curve? curve = null)
    {
        node.requestFocus();
        DartRuntimePrimitives.Ignore(Scrollable.ensureVisible(((FocusNode)node).context!, alignment: (alignment ?? 1), alignmentPolicy: (alignmentPolicy ?? ScrollPositionAlignmentPolicy.@explicit), duration: (duration ?? Duration.zero), curve: (curve ?? global::Doroti.Generated.Framework.Animation.Curves.ease)));
    }

    internal virtual bool _requestTabTraversalFocus(FocusNode node, ScrollPositionAlignmentPolicy? alignmentPolicy = null, double? alignment = null, Duration? duration = null, global::Doroti.Generated.Framework.Animation.Curve? curve = null, bool forward = default!)
    {
        if ((node is FocusScopeNode))
        {
            FocusScopeNode node__as9364 = (FocusScopeNode)node;
            if ((((FocusScopeNode)((FocusScopeNode)node__as9364)).focusedChild is not null))
            {
                return _requestTabTraversalFocus(((FocusScopeNode)((FocusScopeNode)node__as9364)).focusedChild!, alignmentPolicy: alignmentPolicy, alignment: alignment, duration: duration, curve: curve, forward: forward);
            }
            List<FocusNode> sortedChildren__9880 = ((List<FocusNode>)(object?)FocusTraversalPolicy._sortAllDescendants(((FocusScopeNode)node__as9364), ((FocusScopeNode)node__as9364)));
            if (System.Linq.Enumerable.Any(sortedChildren__9880))
            {
                _requestTabTraversalFocus((forward ? sortedChildren__9880.First() : sortedChildren__9880.Last()), alignmentPolicy: alignmentPolicy, alignment: alignment, duration: duration, curve: curve, forward: forward);
                return true;
            }
        }
        bool nodeHadPrimaryFocus__10402 = ((FocusNode)node).hasPrimaryFocus;
        this.requestFocusCallback(node, alignmentPolicy: alignmentPolicy, alignment: alignment, duration: duration, curve: curve);
        return !nodeHadPrimaryFocus__10402;
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
        FocusScopeNode scope__13361 = ((FocusNode)currentNode).nearestScope!;
        FocusNode? candidate__13411 = ((FocusScopeNode)scope__13361).focusedChild;
        if ((ignoreCurrentFocus || ((candidate__13411 is null) && System.Linq.Enumerable.Any(scope__13361.descendants))))
        {
            IEnumerable<FocusNode> sorted__13558 = FocusTraversalPolicy._sortAllDescendants(scope__13361, currentNode).where(((node) => FocusTraversalPolicy._canRequestTraversalFocus(node)));
            if (!System.Linq.Enumerable.Any(sorted__13558))
            {
                candidate__13411 = null;
            }
            else
            {
                candidate__13411 = (fromEnd ? sorted__13558.Last() : sorted__13558.First());
            }
        }
        candidate__13411 ??= currentNode;
        return candidate__13411;
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
        var result__18310 = new List<FocusNode>();
        foreach (FocusNode child__18359 in ((FocusNode)node).children)
        {
            result__18310.Add(child__18359);
            if ((child__18359 is not FocusScopeNode))
            {
                result__18310.AddRange(FocusTraversalPolicy._getDescendantsWithoutExpandingScope(child__18359));
            }
        }
        return ((IEnumerable<FocusNode>)(object?)result__18310);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static DartMap<FocusNode?, _FocusTraversalGroupInfo__focus_traversal> _findGroups(FocusScopeNode scope, _FocusTraversalGroupNode__focus_traversal? scopeGroupNode, FocusNode currentNode)
    {
        FocusTraversalPolicy defaultPolicy__18754 = (scopeGroupNode?.policy ?? new ReadingOrderTraversalPolicy());
        var groups__18845 = new DartMap<FocusNode?, _FocusTraversalGroupInfo__focus_traversal>();
        foreach (FocusNode node__18921 in FocusTraversalPolicy._getDescendantsWithoutExpandingScope(scope))
        {
            _FocusTraversalGroupNode__focus_traversal? groupNode__19014 = ((_FocusTraversalGroupNode__focus_traversal?)(object?)FocusTraversalGroup._getGroupNode(node__18921));
            if ((object.Equals(node__18921, groupNode__19014)))
            {
                _FocusTraversalGroupNode__focus_traversal? parentGroup__19706 = ((_FocusTraversalGroupNode__focus_traversal?)(object?)FocusTraversalGroup._getGroupNode(groupNode__19014!.parent!));
                groups__18845.putIfAbsent(parentGroup__19706, () => new _FocusTraversalGroupInfo__focus_traversal(parentGroup__19706, members: new List<FocusNode>(), defaultPolicy: defaultPolicy__18754));
                DartRuntimePrimitives.Assert(() => !groups__18845.GetValueOrDefault(DartRuntimePrimitives.RequireReference(parentGroup__19706))!.members.Contains(node__18921));
                groups__18845.GetValueOrDefault(DartRuntimePrimitives.RequireReference(parentGroup__19706))!.members.Add(groupNode__19014);
                continue;
            }
            if (((object.Equals(node__18921, currentNode)) || ((((FocusNode)node__18921).canRequestFocus && !((FocusNode)node__18921).skipTraversal))))
            {
                groups__18845.putIfAbsent(groupNode__19014, () => new _FocusTraversalGroupInfo__focus_traversal(groupNode__19014, members: new List<FocusNode>(), defaultPolicy: defaultPolicy__18754));
                DartRuntimePrimitives.Assert(() => !groups__18845.GetValueOrDefault(DartRuntimePrimitives.RequireReference(groupNode__19014))!.members.Contains(node__18921));
                groups__18845.GetValueOrDefault(DartRuntimePrimitives.RequireReference(groupNode__19014))!.members.Add(node__18921);
            }
        }
        return groups__18845;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static List<FocusNode> _sortAllDescendants(FocusScopeNode scope, FocusNode currentNode)
    {
        _FocusTraversalGroupNode__focus_traversal? scopeGroupNode__21054 = ((_FocusTraversalGroupNode__focus_traversal?)(object?)FocusTraversalGroup._getGroupNode(scope));
        DartMap<FocusNode?, _FocusTraversalGroupInfo__focus_traversal> groups__21242 = ((DartMap<FocusNode?, _FocusTraversalGroupInfo__focus_traversal>)(object?)FocusTraversalPolicy._findGroups(scope, scopeGroupNode__21054, currentNode));
        foreach (FocusNode? key__21416 in groups__21242.Keys)
        {
            List<FocusNode> sortedMembers__21466 = groups__21242.GetValueOrDefault(DartRuntimePrimitives.RequireReference(key__21416))!.policy.sortDescendants(groups__21242.GetValueOrDefault(DartRuntimePrimitives.RequireReference(key__21416))!.members.Cast<FocusNode>(), currentNode).ToList().ToList();
            groups__21242.GetValueOrDefault(DartRuntimePrimitives.RequireReference(key__21416))!.members.Clear();
            groups__21242.GetValueOrDefault(DartRuntimePrimitives.RequireReference(key__21416))!.members.AddRange(sortedMembers__21466.Cast<FocusNode>());
        }
        var sortedDescendants__21804 = new List<FocusNode>();
        void visitGroups(_FocusTraversalGroupInfo__focus_traversal info)
        {
            foreach (FocusNode node__21920 in ((_FocusTraversalGroupInfo__focus_traversal)info).members)
            {
                if (groups__21242.ContainsKey(node__21920))
                {
                    visitGroups(groups__21242.GetValueOrDefault(node__21920)!);
                }
                else
                {
                    sortedDescendants__21804.Add(node__21920);
                }
            }
        }
        if ((System.Linq.Enumerable.Any(groups__21242) && groups__21242.ContainsKey(scopeGroupNode__21054)))
        {
            visitGroups(groups__21242.GetValueOrDefault(DartRuntimePrimitives.RequireReference(scopeGroupNode__21054))!);
        }
        sortedDescendants__21804.removeWhere(((node) => {
return ((!object.Equals(node, currentNode)) && !FocusTraversalPolicy._canRequestTraversalFocus(node));
throw new InvalidOperationException("Dart closure completed without a value.");
}));
        DartRuntimePrimitives.Assert(() =>
            {
                HashSet<FocusNode> difference__22923 = sortedDescendants__21804.toSet().difference<FocusNode>(((FocusScopeNode)scope).traversalDescendants.toSet());
                if (!FocusTraversalPolicy._canRequestTraversalFocus(currentNode))
                {
                    DartRuntimePrimitives.Assert(() => (!System.Linq.Enumerable.Any(difference__22923) || (((checked((long)(difference__22923.Count)) == 1L) && difference__22923.Contains(currentNode)))), () => (object?)"Difference between sorted descendants and FocusScopeNode.traversalDescendants contains " + $"something other than the current skipped node. This is the difference: {difference__22923}");
                    return true;
                }
                DartRuntimePrimitives.Assert(() => !System.Linq.Enumerable.Any(difference__22923), () => (object?)"Sorted descendants contains different nodes than FocusScopeNode.traversalDescendants would. " + $"These are the different nodes: {difference__22923}");
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return sortedDescendants__21804;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _moveFocus(FocusNode currentNode, bool forward)
    {
        FocusScopeNode nearestScope__24698 = ((FocusNode)currentNode).nearestScope!;
        invalidateScopeData(nearestScope__24698);
        FocusNode? focusedChild__24794 = ((FocusScopeNode)nearestScope__24698).focusedChild;
        if ((focusedChild__24794 is null))
        {
            FocusNode? firstFocus__24891 = (forward ? findFirstFocus(currentNode) : findLastFocus(currentNode));
            if ((firstFocus__24891 is not null))
            {
                return _requestTabTraversalFocus(firstFocus__24891, alignmentPolicy: (forward ? ScrollPositionAlignmentPolicy.keepVisibleAtEnd : ScrollPositionAlignmentPolicy.keepVisibleAtStart), forward: forward);
            }
        }
        focusedChild__24794 ??= nearestScope__24698;
        List<FocusNode> sortedNodes__25366 = ((List<FocusNode>)(object?)FocusTraversalPolicy._sortAllDescendants(nearestScope__24698, focusedChild__24794));
        DartRuntimePrimitives.Assert(() => sortedNodes__25366.Contains(focusedChild__24794));
        if ((forward && (object.Equals(focusedChild__24794, sortedNodes__25366.Last()))))
        {
            switch (((FocusScopeNode)nearestScope__24698).traversalEdgeBehavior)
            {
                case TraversalEdgeBehavior.leaveFlutterView:
                    {
                        focusedChild__24794.unfocus();
                        return false;
                    }
                case TraversalEdgeBehavior.parentScope:
                    {
                        FocusScopeNode? parentScope__25776 = nearestScope__24698.enclosingScope;
                        if (((parentScope__25776 is not null) && (!object.Equals(parentScope__25776, FocusManager.instance.rootScope))))
                        {
                            focusedChild__24794.unfocus();
                            parentScope__25776.nextFocus();
                            return (!object.Equals(((FocusNode)focusedChild__24794).enclosingScope?.focusedChild, focusedChild__24794));
                        }
                        return _requestTabTraversalFocus(sortedNodes__25366.First(), alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd, forward: forward);
                    }
                case TraversalEdgeBehavior.closedLoop:
                    {
                        return _requestTabTraversalFocus(sortedNodes__25366.First(), alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd, forward: forward);
                    }
                case TraversalEdgeBehavior.stop:
                    {
                        return false;
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
        if ((!forward && (object.Equals(focusedChild__24794, sortedNodes__25366.First()))))
        {
            switch (((FocusScopeNode)nearestScope__24698).traversalEdgeBehavior)
            {
                case TraversalEdgeBehavior.leaveFlutterView:
                    {
                        focusedChild__24794.unfocus();
                        return false;
                    }
                case TraversalEdgeBehavior.parentScope:
                    {
                        FocusScopeNode? parentScope__27007 = nearestScope__24698.enclosingScope;
                        if (((parentScope__27007 is not null) && (!object.Equals(parentScope__27007, FocusManager.instance.rootScope))))
                        {
                            focusedChild__24794.unfocus();
                            parentScope__27007.previousFocus();
                            return (!object.Equals(((FocusNode)focusedChild__24794).enclosingScope?.focusedChild, focusedChild__24794));
                        }
                        return _requestTabTraversalFocus(sortedNodes__25366.Last(), alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart, forward: forward);
                    }
                case TraversalEdgeBehavior.closedLoop:
                    {
                        return _requestTabTraversalFocus(sortedNodes__25366.Last(), alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart, forward: forward);
                    }
                case TraversalEdgeBehavior.stop:
                    {
                        return false;
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
        IEnumerable<FocusNode> maybeFlipped__27975 = (forward ? sortedNodes__25366 : System.Linq.Enumerable.Reverse(sortedNodes__25366));
        FocusNode? previousNode__28051 = default!;
        foreach (var node__28080 in maybeFlipped__27975)
        {
            if ((object.Equals(previousNode__28051, focusedChild__24794)))
            {
                return _requestTabTraversalFocus(node__28080, alignmentPolicy: (forward ? ScrollPositionAlignmentPolicy.keepVisibleAtEnd : ScrollPositionAlignmentPolicy.keepVisibleAtStart), forward: forward);
            }
            previousNode__28051 = node__28080;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((fullString__105654 ?? (string)toStringShort()));
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
        List<FocusNode> sorted__39291 = nodes.ToList().ToList();
        global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted__39291, compare: ((nodeA, nodeB) => {
global::Doroti.Flutter.Ui.Offset a__39429 = ((global::Doroti.Flutter.Ui.Offset)(object?)((Offset)((dynamic)((FocusNode)nodeA).rect).center));
global::Doroti.Flutter.Ui.Offset b__39473 = ((global::Doroti.Flutter.Ui.Offset)(object?)((Offset)((dynamic)((FocusNode)nodeB).rect).center));
long vertical__39514 = DirectionalFocusTraversalPolicyMixin._verticalCompare(target, a__39429, b__39473);
if ((vertical__39514 == 0L))
{
    return DirectionalFocusTraversalPolicyMixin._horizontalCompare(target, a__39429, b__39473);
}
return vertical__39514;
throw new InvalidOperationException("Dart closure completed without a value.");
}));
        return ((IEnumerable<FocusNode>)(object?)sorted__39291);
    }
    public static IEnumerable<FocusNode> _sortByDistancePreferHorizontal(Offset target, IEnumerable<FocusNode> nodes)
    {
        List<FocusNode> sorted__40003 = nodes.ToList().ToList();
        global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted__40003, compare: ((nodeA, nodeB) => {
global::Doroti.Flutter.Ui.Offset a__40141 = ((global::Doroti.Flutter.Ui.Offset)(object?)((Offset)((dynamic)((FocusNode)nodeA).rect).center));
global::Doroti.Flutter.Ui.Offset b__40185 = ((global::Doroti.Flutter.Ui.Offset)(object?)((Offset)((dynamic)((FocusNode)nodeB).rect).center));
long horizontal__40226 = DirectionalFocusTraversalPolicyMixin._horizontalCompare(target, a__40141, b__40185);
if ((horizontal__40226 == 0L))
{
    return DirectionalFocusTraversalPolicyMixin._verticalCompare(target, a__40141, b__40185);
}
return horizontal__40226;
throw new InvalidOperationException("Dart closure completed without a value.");
}));
        return ((IEnumerable<FocusNode>)(object?)sorted__40003);
    }
    public static long _verticalCompareClosestEdge(Offset target, Rect a, Rect b)
    {
        double aCoord__40579 = ((((a.top - target.dy)).abs() < ((a.bottom - target.dy)).abs()) ? a.top : a.bottom);
        double bCoord__40698 = ((((b.top - target.dy)).abs() < ((b.bottom - target.dy)).abs()) ? b.top : b.bottom);
        return ((aCoord__40579 - target.dy)).abs().CompareTo(((bCoord__40698 - target.dy)).abs());
    }
    public static long _horizontalCompareClosestEdge(Offset target, Rect a, Rect b)
    {
        double aCoord__41033 = ((((a.left - target.dx)).abs() < ((a.right - target.dx)).abs()) ? a.left : a.right);
        double bCoord__41152 = ((((b.left - target.dx)).abs() < ((b.right - target.dx)).abs()) ? b.left : b.right);
        return ((aCoord__41033 - target.dx)).abs().CompareTo(((bCoord__41152 - target.dx)).abs());
    }
    public static IEnumerable<FocusNode> _sortClosestEdgesByDistancePreferHorizontal(Offset target, IEnumerable<FocusNode> nodes)
    {
        List<FocusNode> sorted__41660 = nodes.ToList().ToList();
        global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted__41660, compare: ((nodeA, nodeB) => {
long horizontal__41795 = DirectionalFocusTraversalPolicyMixin._horizontalCompareClosestEdge(target, ((FocusNode)nodeA).rect, ((FocusNode)nodeB).rect);
if ((horizontal__41795 == 0L))
{
    return DirectionalFocusTraversalPolicyMixin._verticalCompare(target, ((Offset)((dynamic)((FocusNode)nodeA).rect).center), ((Offset)((dynamic)((FocusNode)nodeB).rect).center));
}
return horizontal__41795;
throw new InvalidOperationException("Dart closure completed without a value.");
}));
        return ((IEnumerable<FocusNode>)(object?)sorted__41660);
    }
    public static IEnumerable<FocusNode> _sortClosestEdgesByDistancePreferVertical(Offset target, IEnumerable<FocusNode> nodes)
    {
        List<FocusNode> sorted__42482 = nodes.ToList().ToList();
        global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted__42482, compare: ((nodeA, nodeB) => {
long vertical__42617 = DirectionalFocusTraversalPolicyMixin._verticalCompareClosestEdge(target, ((FocusNode)nodeA).rect, ((FocusNode)nodeB).rect);
if ((vertical__42617 == 0L))
{
    return DirectionalFocusTraversalPolicyMixin._horizontalCompare(target, ((Offset)((dynamic)((FocusNode)nodeA).rect).center), ((Offset)((dynamic)((FocusNode)nodeB).rect).center));
}
return vertical__42617;
throw new InvalidOperationException("Dart closure completed without a value.");
}));
        return ((IEnumerable<FocusNode>)(object?)sorted__42482);
    }
    public IEnumerable<FocusNode> _sortAndFilterHorizontally(TraversalDirection direction, Rect target, IEnumerable<FocusNode> nodes, bool forward = true);
    public IEnumerable<FocusNode> _sortAndFilterVertically(TraversalDirection direction, Rect target, IEnumerable<FocusNode> nodes, bool forward = true);
    public bool _popPolicyDataIfNeeded(TraversalDirection direction, FocusScopeNode nearestScope, FocusNode focusedChild, _FocusTraversalGroupNode__focus_traversal? groupNode);
    public void _pushPolicyData(TraversalDirection direction, FocusScopeNode nearestScope, FocusNode focusedChild);
    public bool _requestTraversalFocusInDirection(FocusNode currentNode, FocusNode node, FocusScopeNode nearestScope, TraversalDirection direction, _FocusTraversalGroupNode__focus_traversal? groupNode);
    public void _requestFocus(FocusNode node, _FocusTraversalGroupNode__focus_traversal? groupNode, ScrollPositionAlignmentPolicy? alignmentPolicy = null, double? alignment = null, Duration? duration = null, global::Doroti.Generated.Framework.Animation.Curve? curve = null);
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
            this._policyData.GetValueOrDefault(oldScope)?.history.removeWhere(((entry) => {
return (object.Equals(((_DirectionalPolicyDataEntry__focus_traversal)entry).node, node));
throw new InvalidOperationException("Dart closure completed without a value.");
}));
        }
    }

    public override FocusNode? findFirstFocusInDirection(FocusNode currentNode, TraversalDirection direction)
    {
        IEnumerable<FocusNode> nodes__33125 = ((FocusNode)currentNode).nearestScope!.traversalDescendants;
        List<FocusNode> sorted__33207 = nodes__33125.ToList().ToList();
        var (vertical__33248, first__33263) = (direction switch { TraversalDirection.up => (((bool, bool))((true, false))), TraversalDirection.down => (((bool, bool))((true, true))), TraversalDirection.left => (((bool, bool))((false, false))), TraversalDirection.right => (((bool, bool))((false, true))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted__33207, compare: ((a, b) => {
if (vertical__33248)
{
    if (first__33263)
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
    if (first__33263)
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
        return sorted__33207.FirstOrDefault();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual FocusNode? _findNextFocusInDirection(FocusNode focusedChild, IEnumerable<FocusNode> traversalDescendants, TraversalDirection direction, bool forward = true)
    {
        switch (direction)
        {
            case TraversalDirection.down:
            case TraversalDirection.up:
                {
                    IEnumerable<FocusNode> eligibleNodes__34451 = ((IEnumerable<FocusNode>)(object?)_sortAndFilterVertically(direction, ((FocusNode)focusedChild).rect, traversalDescendants.Cast<FocusNode>(), forward: forward));
                    if (!System.Linq.Enumerable.Any(eligibleNodes__34451))
                    {
                        break;
                    }
                    ScrollableState? focusedScrollable__34709 = ((ScrollableState?)(object?)Scrollable.maybeOf(((FocusNode)focusedChild).context!, axis: global::Doroti.Generated.Framework.Painting.Axis.vertical));
                    if ((focusedScrollable__34709 is not null))
                    {
                        IEnumerable<FocusNode> filteredEligibleNodes__34901 = eligibleNodes__34451.where(((node) => (object.Equals(Scrollable.maybeOf(((FocusNode)node).context!, axis: global::Doroti.Generated.Framework.Painting.Axis.vertical), focusedScrollable__34709))));
                        if (System.Linq.Enumerable.Any(filteredEligibleNodes__34901))
                        {
                            eligibleNodes__34451 = filteredEligibleNodes__34901;
                        }
                    }
                    if ((object.Equals(direction, TraversalDirection.up)))
                    {
                        eligibleNodes__34451 = System.Linq.Enumerable.Reverse(eligibleNodes__34451.ToList());
                    }
                    var band__35412 = global::Doroti.Flutter.Ui.Rect.fromLTRB(((FocusNode)focusedChild).rect.left, -double.PositiveInfinity, ((FocusNode)focusedChild).rect.right, double.PositiveInfinity);
                    IEnumerable<FocusNode> inBand__35603 = eligibleNodes__34451.where(((node) => !((FocusNode)node).rect.intersect(band__35412).isEmpty));
                    if (System.Linq.Enumerable.Any(inBand__35603))
                    {
                        if (forward)
                        {
                            return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBand__35603.Cast<FocusNode>()).First();
                        }
                        return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBand__35603.Cast<FocusNode>()).Last();
                    }
                    if (forward)
                    {
                        return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodes__34451.Cast<FocusNode>()).First();
                    }
                    return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodes__34451.Cast<FocusNode>()).Last();
                }
            case TraversalDirection.right:
            case TraversalDirection.left:
                {
                    IEnumerable<FocusNode> eligibleNodes__36610 = ((IEnumerable<FocusNode>)(object?)_sortAndFilterHorizontally(direction, ((FocusNode)focusedChild).rect, traversalDescendants.Cast<FocusNode>(), forward: forward));
                    if (!System.Linq.Enumerable.Any(eligibleNodes__36610))
                    {
                        break;
                    }
                    ScrollableState? focusedScrollable__36870 = ((ScrollableState?)(object?)Scrollable.maybeOf(((FocusNode)focusedChild).context!, axis: global::Doroti.Generated.Framework.Painting.Axis.horizontal));
                    if ((focusedScrollable__36870 is not null))
                    {
                        IEnumerable<FocusNode> filteredEligibleNodes__37064 = eligibleNodes__36610.where(((node) => (object.Equals(Scrollable.maybeOf(((FocusNode)node).context!, axis: global::Doroti.Generated.Framework.Painting.Axis.horizontal), focusedScrollable__36870))));
                        if (System.Linq.Enumerable.Any(filteredEligibleNodes__37064))
                        {
                            eligibleNodes__36610 = filteredEligibleNodes__37064;
                        }
                    }
                    if ((object.Equals(direction, TraversalDirection.left)))
                    {
                        eligibleNodes__36610 = System.Linq.Enumerable.Reverse(eligibleNodes__36610.ToList());
                    }
                    var band__37579 = global::Doroti.Flutter.Ui.Rect.fromLTRB(-double.PositiveInfinity, ((FocusNode)focusedChild).rect.top, double.PositiveInfinity, ((FocusNode)focusedChild).rect.bottom);
                    IEnumerable<FocusNode> inBand__37770 = eligibleNodes__36610.where(((node) => !((FocusNode)node).rect.intersect(band__37579).isEmpty));
                    if (System.Linq.Enumerable.Any(inBand__37770))
                    {
                        if (forward)
                        {
                            return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBand__37770.Cast<FocusNode>()).First();
                        }
                        return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBand__37770.Cast<FocusNode>()).Last();
                    }
                    if (forward)
                    {
                        return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodes__36610.Cast<FocusNode>()).First();
                    }
                    return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodes__36610.Cast<FocusNode>()).Last();
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
        List<FocusNode> sorted__43670 = nodes.where((direction switch { TraversalDirection.left => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dx <= target.left) : (((dynamic)((FocusNode)node).rect).center.dx >= target.left))))), TraversalDirection.right => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dx >= target.right) : (((dynamic)((FocusNode)node).rect).center.dx <= target.right))))), TraversalDirection.up => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), TraversalDirection.down => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })).ToList().ToList();
        global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted__43670, compare: ((a, b) => ((Offset)((dynamic)((FocusNode)a).rect).center).dx.CompareTo(((Offset)((dynamic)((FocusNode)b).rect).center).dx)));
        return ((IEnumerable<FocusNode>)(object?)sorted__43670);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual IEnumerable<FocusNode> _sortAndFilterVertically(TraversalDirection direction, Rect target, IEnumerable<FocusNode> nodes, bool forward = true)
    {
        DartRuntimePrimitives.Assert(() => ((object.Equals(direction, TraversalDirection.up)) || (object.Equals(direction, TraversalDirection.down))));
        List<FocusNode> sorted__44921 = nodes.where((direction switch { TraversalDirection.up => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dy <= target.top) : (((dynamic)((FocusNode)node).rect).center.dy >= target.top))))), TraversalDirection.down => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dy >= target.bottom) : (((dynamic)((FocusNode)node).rect).center.dy <= target.bottom))))), TraversalDirection.left => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), TraversalDirection.right => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })).ToList().ToList();
        global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted__44921, compare: ((a, b) => ((Offset)((dynamic)((FocusNode)a).rect).center).dy.CompareTo(((Offset)((dynamic)((FocusNode)b).rect).center).dy)));
        return ((IEnumerable<FocusNode>)(object?)sorted__44921);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _popPolicyDataIfNeeded(TraversalDirection direction, FocusScopeNode nearestScope, FocusNode focusedChild, _FocusTraversalGroupNode__focus_traversal? groupNode)
    {
        _DirectionalPolicyData__focus_traversal? policyData__46064 = this._policyData.GetValueOrDefault(nearestScope);
        if ((((policyData__46064 is not null) && System.Linq.Enumerable.Any(((_DirectionalPolicyData__focus_traversal)policyData__46064).history)) && (!object.Equals(((_DirectionalPolicyData__focus_traversal)policyData__46064).history.First().direction, direction))))
        {
            if ((((_DirectionalPolicyData__focus_traversal)policyData__46064).history.Last().node.parent is null))
            {
                invalidateScopeData(nearestScope);
                return false;
            }
            bool popOrInvalidate(TraversalDirection direction)
            {
                FocusNode lastNode__46887 = ((_DirectionalPolicyData__focus_traversal)policyData__46064).history.removeLast<_DirectionalPolicyDataEntry__focus_traversal>().node;
                if ((!object.Equals(Scrollable.maybeOf(((FocusNode)lastNode__46887).context!), Scrollable.maybeOf(global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus!.context!))))
                {
                    invalidateScopeData(nearestScope);
                    return false;
                }
                ScrollPositionAlignmentPolicy alignmentPolicy__47158 = default!;
                switch (direction)
                {
                    case TraversalDirection.up:
                    case TraversalDirection.left:
                        {
                            alignmentPolicy__47158 = ScrollPositionAlignmentPolicy.keepVisibleAtStart;
                            break;
                        }
                    case TraversalDirection.right:
                    case TraversalDirection.down:
                        {
                            alignmentPolicy__47158 = ScrollPositionAlignmentPolicy.keepVisibleAtEnd;
                            break;
                        }
                }
                _requestFocus(lastNode__46887, alignmentPolicy: DartRuntimePrimitives.RequireValue(alignmentPolicy__47158), groupNode: groupNode);
                return true;
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            switch (direction)
            {
                case TraversalDirection.down:
                case TraversalDirection.up:
                    {
                        switch (((_DirectionalPolicyData__focus_traversal)policyData__46064).history.First().direction)
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
                        switch (((_DirectionalPolicyData__focus_traversal)policyData__46064).history.First().direction)
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
        if (((policyData__46064 is not null) && !System.Linq.Enumerable.Any(((_DirectionalPolicyData__focus_traversal)policyData__46064).history)))
        {
            invalidateScopeData(nearestScope);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _pushPolicyData(TraversalDirection direction, FocusScopeNode nearestScope, FocusNode focusedChild)
    {
        _DirectionalPolicyData__focus_traversal? policyData__49002 = this._policyData.GetValueOrDefault(nearestScope);
        var newEntry__49052 = new _DirectionalPolicyDataEntry__focus_traversal(node: focusedChild, direction: direction);
        if ((policyData__49002 is not null))
        {
            ((_DirectionalPolicyData__focus_traversal)policyData__49002).history.Add(newEntry__49052);
        }
        else
        {
            this._policyData[nearestScope] = new _DirectionalPolicyData__focus_traversal(history: new List<_DirectionalPolicyDataEntry__focus_traversal> { newEntry__49052 });
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
            FocusNode firstNode__49831 = (findFirstFocusInDirection(node, direction) ?? currentNode);
            switch (direction)
            {
                case TraversalDirection.up:
                case TraversalDirection.left:
                    {
                        _requestFocus(firstNode__49831, groupNode, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart);
                        break;
                    }
                case TraversalDirection.right:
                case TraversalDirection.down:
                    {
                        _requestFocus(firstNode__49831, groupNode, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd);
                        break;
                    }
            }
            return true;
        }
        bool nodeHadPrimaryFocus__50452 = ((FocusNode)node).hasPrimaryFocus;
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
        return !nodeHadPrimaryFocus__50452;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _requestFocus(FocusNode node, _FocusTraversalGroupNode__focus_traversal? groupNode, ScrollPositionAlignmentPolicy? alignmentPolicy = null, double? alignment = null, Duration? duration = null, global::Doroti.Generated.Framework.Animation.Curve? curve = null)
    {
        groupNode?.lastRequestedFocus = node;
        this.requestFocusCallback(node, alignmentPolicy: alignmentPolicy, alignment: alignment, duration: duration, curve: curve);
    }

    public virtual bool _onEdgeForDirection(FocusNode currentNode, FocusNode focusedChild, _FocusTraversalGroupNode__focus_traversal? groupNode, TraversalDirection direction, FocusScopeNode? scope = null)
    {
        FocusScopeNode nearestScope__51630 = (scope ?? ((FocusNode)currentNode).nearestScope!);
        FocusNode? found__51696 = default!;
        switch (((FocusScopeNode)nearestScope__51630).directionalTraversalEdgeBehavior)
        {
            case TraversalEdgeBehavior.leaveFlutterView:
                {
                    focusedChild.unfocus();
                    return false;
                }
            case TraversalEdgeBehavior.parentScope:
                {
                    FocusScopeNode? parentScope__51945 = nearestScope__51630.enclosingScope;
                    if (((parentScope__51945 is not null) && (!object.Equals(parentScope__51945, FocusManager.instance.rootScope))))
                    {
                        invalidateScopeData(nearestScope__51630);
                        nearestScope__51630 = parentScope__51945;
                        invalidateScopeData(nearestScope__51630);
                        found__51696 = _findNextFocusInDirection(focusedChild, ((FocusScopeNode)nearestScope__51630).traversalDescendants.Cast<FocusNode>(), direction);
                        if ((found__51696 is null))
                        {
                            return _onEdgeForDirection(currentNode, focusedChild, groupNode, direction, scope: nearestScope__51630);
                        }
                    }
                    else
                    {
                        found__51696 = _findNextFocusInDirection(focusedChild, ((FocusScopeNode)nearestScope__51630).traversalDescendants.Cast<FocusNode>(), direction, forward: false);
                    }
                    break;
                }
            case TraversalEdgeBehavior.closedLoop:
                {
                    found__51696 = _findNextFocusInDirection(focusedChild, ((FocusScopeNode)nearestScope__51630).traversalDescendants.Cast<FocusNode>(), direction, forward: false);
                    break;
                }
            case TraversalEdgeBehavior.stop:
                {
                    return false;
                }
        }
        if ((found__51696 is not null))
        {
            return _requestTraversalFocusInDirection(currentNode, found__51696, DartRuntimePrimitives.ConvertValue<FocusScopeNode>(nearestScope__51630), direction, groupNode);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool inDirection(FocusNode currentNode, TraversalDirection direction)
    {
        _FocusTraversalGroupNode__focus_traversal? groupNode__54345 = ((_FocusTraversalGroupNode__focus_traversal?)(object?)FocusTraversalGroup._getGroupNode(currentNode));
        FocusScopeNode nearestScope__54430 = ((FocusNode)currentNode).nearestScope!;
        FocusNode? focusedChild__54493 = ((FocusScopeNode)nearestScope__54430).focusedChild;
        if ((focusedChild__54493 is null))
        {
            FocusNode firstFocus__54589 = (findFirstFocusInDirection(currentNode, direction) ?? currentNode);
            switch (direction)
            {
                case TraversalDirection.up:
                case TraversalDirection.left:
                    {
                        _requestFocus(firstFocus__54589, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart, groupNode: groupNode__54345);
                        break;
                    }
                case TraversalDirection.right:
                case TraversalDirection.down:
                    {
                        _requestFocus(firstFocus__54589, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd, groupNode: groupNode__54345);
                        break;
                    }
            }
            return true;
        }
        if (_popPolicyDataIfNeeded(direction, nearestScope__54430, focusedChild__54493, groupNode__54345))
        {
            return true;
        }
        FocusNode? found__55345 = ((FocusNode?)(object?)_findNextFocusInDirection(focusedChild__54493, ((FocusScopeNode)nearestScope__54430).traversalDescendants.Cast<FocusNode>(), direction));
        if ((found__55345 is not null))
        {
            _pushPolicyData(direction, nearestScope__54430, focusedChild__54493);
            return _requestTraversalFocusInDirection(currentNode, found__55345, DartRuntimePrimitives.ConvertValue<FocusScopeNode>(nearestScope__54430), direction, groupNode__54345);
        }
        return _onEdgeForDirection(currentNode, focusedChild__54493, groupNode__54345, direction);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _ReadingOrderSortData__focus_traversal : global::Doroti.Generated.Framework.Foundation.Diagnosticable
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

    internal static global::Doroti.Flutter.Ui.TextDirection? _findDirectionality(BuildContext context)
    {
        return ((TextDirection?)((dynamic)context.getInheritedWidgetOfExactType<Directionality>())?.textDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Flutter.Ui.TextDirection? commonDirectionalityOf(List<_ReadingOrderSortData__focus_traversal> list)
    {
        IEnumerable<HashSet<Directionality>> allAncestors__58247 = list.map<_ReadingOrderSortData__focus_traversal, HashSet<Directionality>>(((member) => ((_ReadingOrderSortData__focus_traversal)member).directionalAncestors.toSet()));
        HashSet<Directionality>? common__58402 = default!;
        foreach (var ancestorSet__58425 in allAncestors__58247)
        {
            common__58402 ??= ancestorSet__58425;
            common__58402 = common__58402.intersection(ancestorSet__58425);
        }
        if (!System.Linq.Enumerable.Any(common__58402!))
        {
            return list.First().directionality;
        }
        return ((TextDirection)((dynamic)list.First().directionalAncestors.firstWhere(common__58402.Contains)).textDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static void sortWithDirectionality(List<_ReadingOrderSortData__focus_traversal> list, TextDirection directionality)
    {
        global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.mergeSort<_ReadingOrderSortData__focus_traversal>(list, compare: ((a, b) => (directionality switch { TextDirection.ltr => ((_ReadingOrderSortData__focus_traversal)a).rect.left.CompareTo(((_ReadingOrderSortData__focus_traversal)b).rect.left), TextDirection.rtl => ((_ReadingOrderSortData__focus_traversal)b).rect.right.CompareTo(((_ReadingOrderSortData__focus_traversal)a).rect.right), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })));
    }

    public virtual IEnumerable<Directionality> directionalAncestors
    {
        get
        {
            List<Directionality> getDirectionalityAncestors(BuildContext context)
            {
                var result__59822 = new List<Directionality>();
                InheritedElement? directionalityElement__59875 = ((InheritedElement?)(object?)context.getElementForInheritedWidgetOfExactType<Directionality>());
                while ((directionalityElement__59875 is not null))
                {
                    result__59822.Add(((Directionality?)(object?)directionalityElement__59875.widget)!);
                    directionalityElement__59875 = Focus_traversalLibrary._getAncestor(directionalityElement__59875)?.getElementForInheritedWidgetOfExactType<Directionality>();
                }
                return result__59822;
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            _directionalAncestors ??= getDirectionalityAncestors(((FocusNode)this.node).context!);
            return ((IEnumerable<Directionality>)(object?)this._directionalAncestors!);
            return default!;
        }
    }
    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Flutter.Ui.TextDirection>("directionality", this.directionality));
        properties.add(new global::Doroti.Generated.Framework.Foundation.StringProperty("name", ((FocusNode)this.node).debugLabel, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Flutter.Ui.Rect>("rect", this.rect));
    }

    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ReadingOrderDirectionalGroupData__focus_traversal : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual List<_ReadingOrderSortData__focus_traversal> members { get; private set; } = default!;
    internal virtual Rect? _rect { get; set; } = default;
    internal virtual List<Directionality>? _memberAncestors { get; set; } = default;

    internal _ReadingOrderDirectionalGroupData__focus_traversal(List<_ReadingOrderSortData__focus_traversal> members)
    {
        this.members = members;
    }

    public virtual global::Doroti.Flutter.Ui.TextDirection? directionality => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.TextDirection>(this.members.First().directionality);
    public virtual global::Doroti.Flutter.Ui.Rect rect
    {
        get
        {
            if ((this._rect is null))
            {
                foreach (global::Doroti.Flutter.Ui.Rect rect__61235 in this.members.map<_ReadingOrderSortData__focus_traversal, Rect>(((data) => ((_ReadingOrderSortData__focus_traversal)data).rect)))
                {
                    _rect ??= rect__61235;
                    _rect = DartRuntimePrimitives.RequireValue(this._rect).expandToInclude(rect__61235);
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
                foreach (_ReadingOrderSortData__focus_traversal member__61580 in this.members)
                {
                    this._memberAncestors!.AddRange(((_ReadingOrderSortData__focus_traversal)member__61580).directionalAncestors.Cast<Directionality>());
                }
            }
            return this._memberAncestors!;
            return default!;
        }
    }
    public static void sortWithDirectionality(List<_ReadingOrderDirectionalGroupData__focus_traversal> list, TextDirection directionality)
    {
        global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.mergeSort<_ReadingOrderDirectionalGroupData__focus_traversal>(list, compare: ((a, b) => (directionality switch { TextDirection.ltr => ((_ReadingOrderDirectionalGroupData__focus_traversal)a).rect.left.CompareTo(((_ReadingOrderDirectionalGroupData__focus_traversal)b).rect.left), TextDirection.rtl => ((_ReadingOrderDirectionalGroupData__focus_traversal)b).rect.right.CompareTo(((_ReadingOrderDirectionalGroupData__focus_traversal)a).rect.right), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Flutter.Ui.TextDirection>("directionality", this.directionality));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Flutter.Ui.Rect>("rect", this.rect));
        properties.add(new global::Doroti.Generated.Framework.Foundation.IterableProperty<string>("members", this.members.map<_ReadingOrderSortData__focus_traversal, string>(((member) => {
return $"\"{((_ReadingOrderSortData__focus_traversal)member).node.debugLabel}\"({((_ReadingOrderSortData__focus_traversal)member).rect})";
throw new InvalidOperationException("Dart closure completed without a value.");
})).Cast<string>()));
    }

    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((fullString__105654 ?? (string)toStringShort()));
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
        var data__64502 = new List<_ReadingOrderSortData__focus_traversal>();
        var sortedList__64623 = new List<FocusNode>();
        var unplaced__64661 = data__64502;
        _ReadingOrderSortData__focus_traversal current__64859 = ((_ReadingOrderSortData__focus_traversal)(object?)ReadingOrderTraversalPolicy._pickNext(unplaced__64661));
        sortedList__64623.Add(((_ReadingOrderSortData__focus_traversal)current__64859).node);
        unplaced__64661.Remove(current__64859);
        while (System.Linq.Enumerable.Any(unplaced__64661))
        {
            _ReadingOrderSortData__focus_traversal next__65219 = ((_ReadingOrderSortData__focus_traversal)(object?)ReadingOrderTraversalPolicy._pickNext(unplaced__64661));
            current__64859 = next__65219;
            sortedList__64623.Add(((_ReadingOrderSortData__focus_traversal)current__64859).node);
            unplaced__64661.Remove(current__64859);
        }
        return ((IEnumerable<FocusNode>)(object?)sortedList__64623);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static List<_ReadingOrderDirectionalGroupData__focus_traversal> _collectDirectionalityGroups(IEnumerable<_ReadingOrderSortData__focus_traversal> candidates)
    {
        global::Doroti.Flutter.Ui.TextDirection? currentDirection__65717 = candidates.First().directionality;
        var currentGroup__65777 = new List<_ReadingOrderSortData__focus_traversal>();
        var result__65829 = new List<_ReadingOrderDirectionalGroupData__focus_traversal>();
        foreach (var candidate__65954 in candidates)
        {
            if ((object.Equals(((_ReadingOrderSortData__focus_traversal)candidate__65954).directionality, currentDirection__65717)))
            {
                currentGroup__65777.Add(candidate__65954);
                continue;
            }
            currentDirection__65717 = ((_ReadingOrderSortData__focus_traversal)candidate__65954).directionality;
            result__65829.Add(new _ReadingOrderDirectionalGroupData__focus_traversal(currentGroup__65777));
            currentGroup__65777 = new List<_ReadingOrderSortData__focus_traversal> { candidate__65954 };
        }
        if (System.Linq.Enumerable.Any(currentGroup__65777))
        {
            result__65829.Add(new _ReadingOrderDirectionalGroupData__focus_traversal(currentGroup__65777));
        }
        foreach (var bandGroup__66481 in result__65829)
        {
            if ((checked((long)(((_ReadingOrderDirectionalGroupData__focus_traversal)bandGroup__66481).members.Count)) == 1L))
            {
                continue;
            }
            _ReadingOrderSortData__focus_traversal.sortWithDirectionality(((_ReadingOrderDirectionalGroupData__focus_traversal)bandGroup__66481).members, DartRuntimePrimitives.RequireValue(((_ReadingOrderDirectionalGroupData__focus_traversal)bandGroup__66481).directionality));
        }
        return result__65829;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static _ReadingOrderSortData__focus_traversal _pickNext(List<_ReadingOrderSortData__focus_traversal> candidates)
    {
        global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.mergeSort<_ReadingOrderSortData__focus_traversal>(candidates, compare: ((a, b) => ((_ReadingOrderSortData__focus_traversal)a).rect.top.CompareTo(((_ReadingOrderSortData__focus_traversal)b).rect.top)));
        _ReadingOrderSortData__focus_traversal topmost__67091 = candidates.First();
        List<_ReadingOrderSortData__focus_traversal> inBand(_ReadingOrderSortData__focus_traversal current, IEnumerable<_ReadingOrderSortData__focus_traversal> candidates)
        {
            var band__67351 = global::Doroti.Flutter.Ui.Rect.fromLTRB(double.NegativeInfinity, ((_ReadingOrderSortData__focus_traversal)current).rect.top, double.PositiveInfinity, ((_ReadingOrderSortData__focus_traversal)current).rect.bottom);
            return candidates.where(((item) => {
return !((_ReadingOrderSortData__focus_traversal)item).rect.intersect(band__67351).isEmpty;
throw new InvalidOperationException("Dart closure completed without a value.");
})).ToList();
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        List<_ReadingOrderSortData__focus_traversal> inBandOfTop__67671 = inBand(topmost__67091, candidates.Cast<_ReadingOrderSortData__focus_traversal>()).ToList();
        DartRuntimePrimitives.Assert(() => (((_ReadingOrderSortData__focus_traversal)topmost__67091).rect.isEmpty || System.Linq.Enumerable.Any(inBandOfTop__67671)));
        if ((checked((long)(inBandOfTop__67671.Count)) <= 1L))
        {
            return topmost__67091;
        }
        global::Doroti.Flutter.Ui.TextDirection? nearestCommonDirectionality__68361 = _ReadingOrderSortData__focus_traversal.commonDirectionalityOf(inBandOfTop__67671);
        _ReadingOrderSortData__focus_traversal.sortWithDirectionality(inBandOfTop__67671, DartRuntimePrimitives.RequireValue(nearestCommonDirectionality__68361));
        List<_ReadingOrderDirectionalGroupData__focus_traversal> bandGroups__69049 = ((List<_ReadingOrderDirectionalGroupData__focus_traversal>)(object?)ReadingOrderTraversalPolicy._collectDirectionalityGroups(inBandOfTop__67671.Cast<_ReadingOrderSortData__focus_traversal>()));
        if ((checked((long)(bandGroups__69049.Count)) == 1L))
        {
            return bandGroups__69049.First().members.First();
        }
        _ReadingOrderDirectionalGroupData__focus_traversal.sortWithDirectionality(bandGroups__69049, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(nearestCommonDirectionality__68361)));
        return bandGroups__69049.First().members.First();
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
            this._policyData.GetValueOrDefault(oldScope)?.history.removeWhere(((entry) => {
return (object.Equals(((_DirectionalPolicyDataEntry__focus_traversal)entry).node, node));
throw new InvalidOperationException("Dart closure completed without a value.");
}));
        }
    }

    public override FocusNode? findFirstFocusInDirection(FocusNode currentNode, TraversalDirection direction)
    {
        IEnumerable<FocusNode> nodes__33125 = ((FocusNode)currentNode).nearestScope!.traversalDescendants;
        List<FocusNode> sorted__33207 = nodes__33125.ToList().ToList();
        var (vertical__33248, first__33263) = (direction switch { TraversalDirection.up => (((bool, bool))((true, false))), TraversalDirection.down => (((bool, bool))((true, true))), TraversalDirection.left => (((bool, bool))((false, false))), TraversalDirection.right => (((bool, bool))((false, true))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted__33207, compare: ((a, b) => {
if (vertical__33248)
{
    if (first__33263)
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
    if (first__33263)
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
        return sorted__33207.FirstOrDefault();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual FocusNode? _findNextFocusInDirection(FocusNode focusedChild, IEnumerable<FocusNode> traversalDescendants, TraversalDirection direction, bool forward = true)
    {
        switch (direction)
        {
            case TraversalDirection.down:
            case TraversalDirection.up:
                {
                    IEnumerable<FocusNode> eligibleNodes__34451 = ((IEnumerable<FocusNode>)(object?)_sortAndFilterVertically(direction, ((FocusNode)focusedChild).rect, traversalDescendants.Cast<FocusNode>(), forward: forward));
                    if (!System.Linq.Enumerable.Any(eligibleNodes__34451))
                    {
                        break;
                    }
                    ScrollableState? focusedScrollable__34709 = ((ScrollableState?)(object?)Scrollable.maybeOf(((FocusNode)focusedChild).context!, axis: global::Doroti.Generated.Framework.Painting.Axis.vertical));
                    if ((focusedScrollable__34709 is not null))
                    {
                        IEnumerable<FocusNode> filteredEligibleNodes__34901 = eligibleNodes__34451.where(((node) => (object.Equals(Scrollable.maybeOf(((FocusNode)node).context!, axis: global::Doroti.Generated.Framework.Painting.Axis.vertical), focusedScrollable__34709))));
                        if (System.Linq.Enumerable.Any(filteredEligibleNodes__34901))
                        {
                            eligibleNodes__34451 = filteredEligibleNodes__34901;
                        }
                    }
                    if ((object.Equals(direction, TraversalDirection.up)))
                    {
                        eligibleNodes__34451 = System.Linq.Enumerable.Reverse(eligibleNodes__34451.ToList());
                    }
                    var band__35412 = global::Doroti.Flutter.Ui.Rect.fromLTRB(((FocusNode)focusedChild).rect.left, -double.PositiveInfinity, ((FocusNode)focusedChild).rect.right, double.PositiveInfinity);
                    IEnumerable<FocusNode> inBand__35603 = eligibleNodes__34451.where(((node) => !((FocusNode)node).rect.intersect(band__35412).isEmpty));
                    if (System.Linq.Enumerable.Any(inBand__35603))
                    {
                        if (forward)
                        {
                            return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBand__35603.Cast<FocusNode>()).First();
                        }
                        return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBand__35603.Cast<FocusNode>()).Last();
                    }
                    if (forward)
                    {
                        return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodes__34451.Cast<FocusNode>()).First();
                    }
                    return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodes__34451.Cast<FocusNode>()).Last();
                }
            case TraversalDirection.right:
            case TraversalDirection.left:
                {
                    IEnumerable<FocusNode> eligibleNodes__36610 = ((IEnumerable<FocusNode>)(object?)_sortAndFilterHorizontally(direction, ((FocusNode)focusedChild).rect, traversalDescendants.Cast<FocusNode>(), forward: forward));
                    if (!System.Linq.Enumerable.Any(eligibleNodes__36610))
                    {
                        break;
                    }
                    ScrollableState? focusedScrollable__36870 = ((ScrollableState?)(object?)Scrollable.maybeOf(((FocusNode)focusedChild).context!, axis: global::Doroti.Generated.Framework.Painting.Axis.horizontal));
                    if ((focusedScrollable__36870 is not null))
                    {
                        IEnumerable<FocusNode> filteredEligibleNodes__37064 = eligibleNodes__36610.where(((node) => (object.Equals(Scrollable.maybeOf(((FocusNode)node).context!, axis: global::Doroti.Generated.Framework.Painting.Axis.horizontal), focusedScrollable__36870))));
                        if (System.Linq.Enumerable.Any(filteredEligibleNodes__37064))
                        {
                            eligibleNodes__36610 = filteredEligibleNodes__37064;
                        }
                    }
                    if ((object.Equals(direction, TraversalDirection.left)))
                    {
                        eligibleNodes__36610 = System.Linq.Enumerable.Reverse(eligibleNodes__36610.ToList());
                    }
                    var band__37579 = global::Doroti.Flutter.Ui.Rect.fromLTRB(-double.PositiveInfinity, ((FocusNode)focusedChild).rect.top, double.PositiveInfinity, ((FocusNode)focusedChild).rect.bottom);
                    IEnumerable<FocusNode> inBand__37770 = eligibleNodes__36610.where(((node) => !((FocusNode)node).rect.intersect(band__37579).isEmpty));
                    if (System.Linq.Enumerable.Any(inBand__37770))
                    {
                        if (forward)
                        {
                            return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBand__37770.Cast<FocusNode>()).First();
                        }
                        return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBand__37770.Cast<FocusNode>()).Last();
                    }
                    if (forward)
                    {
                        return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodes__36610.Cast<FocusNode>()).First();
                    }
                    return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodes__36610.Cast<FocusNode>()).Last();
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
        List<FocusNode> sorted__43670 = nodes.where((direction switch { TraversalDirection.left => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dx <= target.left) : (((dynamic)((FocusNode)node).rect).center.dx >= target.left))))), TraversalDirection.right => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dx >= target.right) : (((dynamic)((FocusNode)node).rect).center.dx <= target.right))))), TraversalDirection.up => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), TraversalDirection.down => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })).ToList().ToList();
        global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted__43670, compare: ((a, b) => ((Offset)((dynamic)((FocusNode)a).rect).center).dx.CompareTo(((Offset)((dynamic)((FocusNode)b).rect).center).dx)));
        return ((IEnumerable<FocusNode>)(object?)sorted__43670);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual IEnumerable<FocusNode> _sortAndFilterVertically(TraversalDirection direction, Rect target, IEnumerable<FocusNode> nodes, bool forward = true)
    {
        DartRuntimePrimitives.Assert(() => ((object.Equals(direction, TraversalDirection.up)) || (object.Equals(direction, TraversalDirection.down))));
        List<FocusNode> sorted__44921 = nodes.where((direction switch { TraversalDirection.up => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dy <= target.top) : (((dynamic)((FocusNode)node).rect).center.dy >= target.top))))), TraversalDirection.down => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dy >= target.bottom) : (((dynamic)((FocusNode)node).rect).center.dy <= target.bottom))))), TraversalDirection.left => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), TraversalDirection.right => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })).ToList().ToList();
        global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted__44921, compare: ((a, b) => ((Offset)((dynamic)((FocusNode)a).rect).center).dy.CompareTo(((Offset)((dynamic)((FocusNode)b).rect).center).dy)));
        return ((IEnumerable<FocusNode>)(object?)sorted__44921);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _popPolicyDataIfNeeded(TraversalDirection direction, FocusScopeNode nearestScope, FocusNode focusedChild, _FocusTraversalGroupNode__focus_traversal? groupNode)
    {
        _DirectionalPolicyData__focus_traversal? policyData__46064 = this._policyData.GetValueOrDefault(nearestScope);
        if ((((policyData__46064 is not null) && System.Linq.Enumerable.Any(((_DirectionalPolicyData__focus_traversal)policyData__46064).history)) && (!object.Equals(((_DirectionalPolicyData__focus_traversal)policyData__46064).history.First().direction, direction))))
        {
            if ((((_DirectionalPolicyData__focus_traversal)policyData__46064).history.Last().node.parent is null))
            {
                invalidateScopeData(nearestScope);
                return false;
            }
            bool popOrInvalidate(TraversalDirection direction)
            {
                FocusNode lastNode__46887 = ((_DirectionalPolicyData__focus_traversal)policyData__46064).history.removeLast<_DirectionalPolicyDataEntry__focus_traversal>().node;
                if ((!object.Equals(Scrollable.maybeOf(((FocusNode)lastNode__46887).context!), Scrollable.maybeOf(global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus!.context!))))
                {
                    invalidateScopeData(nearestScope);
                    return false;
                }
                ScrollPositionAlignmentPolicy alignmentPolicy__47158 = default!;
                switch (direction)
                {
                    case TraversalDirection.up:
                    case TraversalDirection.left:
                        {
                            alignmentPolicy__47158 = ScrollPositionAlignmentPolicy.keepVisibleAtStart;
                            break;
                        }
                    case TraversalDirection.right:
                    case TraversalDirection.down:
                        {
                            alignmentPolicy__47158 = ScrollPositionAlignmentPolicy.keepVisibleAtEnd;
                            break;
                        }
                }
                _requestFocus(lastNode__46887, alignmentPolicy: DartRuntimePrimitives.RequireValue(alignmentPolicy__47158), groupNode: groupNode);
                return true;
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            switch (direction)
            {
                case TraversalDirection.down:
                case TraversalDirection.up:
                    {
                        switch (((_DirectionalPolicyData__focus_traversal)policyData__46064).history.First().direction)
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
                        switch (((_DirectionalPolicyData__focus_traversal)policyData__46064).history.First().direction)
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
        if (((policyData__46064 is not null) && !System.Linq.Enumerable.Any(((_DirectionalPolicyData__focus_traversal)policyData__46064).history)))
        {
            invalidateScopeData(nearestScope);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _pushPolicyData(TraversalDirection direction, FocusScopeNode nearestScope, FocusNode focusedChild)
    {
        _DirectionalPolicyData__focus_traversal? policyData__49002 = this._policyData.GetValueOrDefault(nearestScope);
        var newEntry__49052 = new _DirectionalPolicyDataEntry__focus_traversal(node: focusedChild, direction: direction);
        if ((policyData__49002 is not null))
        {
            ((_DirectionalPolicyData__focus_traversal)policyData__49002).history.Add(newEntry__49052);
        }
        else
        {
            this._policyData[nearestScope] = new _DirectionalPolicyData__focus_traversal(history: new List<_DirectionalPolicyDataEntry__focus_traversal> { newEntry__49052 });
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
            FocusNode firstNode__49831 = (findFirstFocusInDirection(node, direction) ?? currentNode);
            switch (direction)
            {
                case TraversalDirection.up:
                case TraversalDirection.left:
                    {
                        _requestFocus(firstNode__49831, groupNode, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart);
                        break;
                    }
                case TraversalDirection.right:
                case TraversalDirection.down:
                    {
                        _requestFocus(firstNode__49831, groupNode, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd);
                        break;
                    }
            }
            return true;
        }
        bool nodeHadPrimaryFocus__50452 = ((FocusNode)node).hasPrimaryFocus;
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
        return !nodeHadPrimaryFocus__50452;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _requestFocus(FocusNode node, _FocusTraversalGroupNode__focus_traversal? groupNode, ScrollPositionAlignmentPolicy? alignmentPolicy = null, double? alignment = null, Duration? duration = null, global::Doroti.Generated.Framework.Animation.Curve? curve = null)
    {
        groupNode?.lastRequestedFocus = node;
        this.requestFocusCallback(node, alignmentPolicy: alignmentPolicy, alignment: alignment, duration: duration, curve: curve);
    }

    public virtual bool _onEdgeForDirection(FocusNode currentNode, FocusNode focusedChild, _FocusTraversalGroupNode__focus_traversal? groupNode, TraversalDirection direction, FocusScopeNode? scope = null)
    {
        FocusScopeNode nearestScope__51630 = (scope ?? ((FocusNode)currentNode).nearestScope!);
        FocusNode? found__51696 = default!;
        switch (((FocusScopeNode)nearestScope__51630).directionalTraversalEdgeBehavior)
        {
            case TraversalEdgeBehavior.leaveFlutterView:
                {
                    focusedChild.unfocus();
                    return false;
                }
            case TraversalEdgeBehavior.parentScope:
                {
                    FocusScopeNode? parentScope__51945 = nearestScope__51630.enclosingScope;
                    if (((parentScope__51945 is not null) && (!object.Equals(parentScope__51945, FocusManager.instance.rootScope))))
                    {
                        invalidateScopeData(nearestScope__51630);
                        nearestScope__51630 = parentScope__51945;
                        invalidateScopeData(nearestScope__51630);
                        found__51696 = _findNextFocusInDirection(focusedChild, ((FocusScopeNode)nearestScope__51630).traversalDescendants.Cast<FocusNode>(), direction);
                        if ((found__51696 is null))
                        {
                            return _onEdgeForDirection(currentNode, focusedChild, groupNode, direction, scope: nearestScope__51630);
                        }
                    }
                    else
                    {
                        found__51696 = _findNextFocusInDirection(focusedChild, ((FocusScopeNode)nearestScope__51630).traversalDescendants.Cast<FocusNode>(), direction, forward: false);
                    }
                    break;
                }
            case TraversalEdgeBehavior.closedLoop:
                {
                    found__51696 = _findNextFocusInDirection(focusedChild, ((FocusScopeNode)nearestScope__51630).traversalDescendants.Cast<FocusNode>(), direction, forward: false);
                    break;
                }
            case TraversalEdgeBehavior.stop:
                {
                    return false;
                }
        }
        if ((found__51696 is not null))
        {
            return _requestTraversalFocusInDirection(currentNode, found__51696, DartRuntimePrimitives.ConvertValue<FocusScopeNode>(nearestScope__51630), direction, groupNode);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool inDirection(FocusNode currentNode, TraversalDirection direction)
    {
        _FocusTraversalGroupNode__focus_traversal? groupNode__54345 = ((_FocusTraversalGroupNode__focus_traversal?)(object?)FocusTraversalGroup._getGroupNode(currentNode));
        FocusScopeNode nearestScope__54430 = ((FocusNode)currentNode).nearestScope!;
        FocusNode? focusedChild__54493 = ((FocusScopeNode)nearestScope__54430).focusedChild;
        if ((focusedChild__54493 is null))
        {
            FocusNode firstFocus__54589 = (findFirstFocusInDirection(currentNode, direction) ?? currentNode);
            switch (direction)
            {
                case TraversalDirection.up:
                case TraversalDirection.left:
                    {
                        _requestFocus(firstFocus__54589, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart, groupNode: groupNode__54345);
                        break;
                    }
                case TraversalDirection.right:
                case TraversalDirection.down:
                    {
                        _requestFocus(firstFocus__54589, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd, groupNode: groupNode__54345);
                        break;
                    }
            }
            return true;
        }
        if (_popPolicyDataIfNeeded(direction, nearestScope__54430, focusedChild__54493, groupNode__54345))
        {
            return true;
        }
        FocusNode? found__55345 = ((FocusNode?)(object?)_findNextFocusInDirection(focusedChild__54493, ((FocusScopeNode)nearestScope__54430).traversalDescendants.Cast<FocusNode>(), direction));
        if ((found__55345 is not null))
        {
            _pushPolicyData(direction, nearestScope__54430, focusedChild__54493);
            return _requestTraversalFocusInDirection(currentNode, found__55345, DartRuntimePrimitives.ConvertValue<FocusScopeNode>(nearestScope__54430), direction, groupNode__54345);
        }
        return _onEdgeForDirection(currentNode, focusedChild__54493, groupNode__54345, direction);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class FocusOrder : global::Doroti.Generated.Framework.Foundation.Diagnosticable, IComparable<FocusOrder>
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
    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((fullString__105654 ?? (string)toStringShort()));
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
    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("order", this.order));
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
    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.StringProperty("order", this.order));
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
        FocusTraversalPolicy secondaryPolicy__77892 = (this.secondary ?? new ReadingOrderTraversalPolicy());
        IEnumerable<FocusNode> sortedDescendants__77984 = ((IEnumerable<FocusNode>)(object?)secondaryPolicy__77892.sortDescendants(descendants.Cast<FocusNode>(), currentNode));
        var unordered__78092 = new List<FocusNode>();
        var ordered__78129 = new List<_OrderedFocusInfo__focus_traversal>();
        foreach (var node__78177 in sortedDescendants__77984)
        {
            FocusOrder? order__78230 = ((FocusOrder?)(object?)FocusTraversalOrder.maybeOf(((FocusNode)node__78177).context!));
            if ((order__78230 is not null))
            {
                ordered__78129.Add(new _OrderedFocusInfo__focus_traversal(node: node__78177, order: order__78230));
            }
            else
            {
                unordered__78092.Add(node__78177);
            }
        }
        global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.mergeSort<_OrderedFocusInfo__focus_traversal>(ordered__78129, compare: ((a, b) => {
DartRuntimePrimitives.Assert(() => (object.Equals(DartRuntimePrimitives.RuntimeType(((_OrderedFocusInfo__focus_traversal)a).order), DartRuntimePrimitives.RuntimeType(((_OrderedFocusInfo__focus_traversal)b).order))), () => (object?)$"When sorting nodes for determining focus order, the order ({((_OrderedFocusInfo__focus_traversal)a).order}) of " + $"node {((_OrderedFocusInfo__focus_traversal)a).node}, isn't the same type as the order ({((_OrderedFocusInfo__focus_traversal)b).order}) of {((_OrderedFocusInfo__focus_traversal)b).node}. " + "Incompatible order types can't be compared. Use a FocusTraversalGroup to group " + "similar orders together.");
return ((_OrderedFocusInfo__focus_traversal)a).order.compareTo(((_OrderedFocusInfo__focus_traversal)b).order);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
        return ordered__78129.map<_OrderedFocusInfo__focus_traversal, FocusNode>(((info) => ((_OrderedFocusInfo__focus_traversal)info).node)).followedBy(unordered__78092.Cast<FocusNode>());
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
            this._policyData.GetValueOrDefault(oldScope)?.history.removeWhere(((entry) => {
return (object.Equals(((_DirectionalPolicyDataEntry__focus_traversal)entry).node, node));
throw new InvalidOperationException("Dart closure completed without a value.");
}));
        }
    }

    public override FocusNode? findFirstFocusInDirection(FocusNode currentNode, TraversalDirection direction)
    {
        IEnumerable<FocusNode> nodes__33125 = ((FocusNode)currentNode).nearestScope!.traversalDescendants;
        List<FocusNode> sorted__33207 = nodes__33125.ToList().ToList();
        var (vertical__33248, first__33263) = (direction switch { TraversalDirection.up => (((bool, bool))((true, false))), TraversalDirection.down => (((bool, bool))((true, true))), TraversalDirection.left => (((bool, bool))((false, false))), TraversalDirection.right => (((bool, bool))((false, true))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted__33207, compare: ((a, b) => {
if (vertical__33248)
{
    if (first__33263)
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
    if (first__33263)
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
        return sorted__33207.FirstOrDefault();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual FocusNode? _findNextFocusInDirection(FocusNode focusedChild, IEnumerable<FocusNode> traversalDescendants, TraversalDirection direction, bool forward = true)
    {
        switch (direction)
        {
            case TraversalDirection.down:
            case TraversalDirection.up:
                {
                    IEnumerable<FocusNode> eligibleNodes__34451 = ((IEnumerable<FocusNode>)(object?)_sortAndFilterVertically(direction, ((FocusNode)focusedChild).rect, traversalDescendants.Cast<FocusNode>(), forward: forward));
                    if (!System.Linq.Enumerable.Any(eligibleNodes__34451))
                    {
                        break;
                    }
                    ScrollableState? focusedScrollable__34709 = ((ScrollableState?)(object?)Scrollable.maybeOf(((FocusNode)focusedChild).context!, axis: global::Doroti.Generated.Framework.Painting.Axis.vertical));
                    if ((focusedScrollable__34709 is not null))
                    {
                        IEnumerable<FocusNode> filteredEligibleNodes__34901 = eligibleNodes__34451.where(((node) => (object.Equals(Scrollable.maybeOf(((FocusNode)node).context!, axis: global::Doroti.Generated.Framework.Painting.Axis.vertical), focusedScrollable__34709))));
                        if (System.Linq.Enumerable.Any(filteredEligibleNodes__34901))
                        {
                            eligibleNodes__34451 = filteredEligibleNodes__34901;
                        }
                    }
                    if ((object.Equals(direction, TraversalDirection.up)))
                    {
                        eligibleNodes__34451 = System.Linq.Enumerable.Reverse(eligibleNodes__34451.ToList());
                    }
                    var band__35412 = global::Doroti.Flutter.Ui.Rect.fromLTRB(((FocusNode)focusedChild).rect.left, -double.PositiveInfinity, ((FocusNode)focusedChild).rect.right, double.PositiveInfinity);
                    IEnumerable<FocusNode> inBand__35603 = eligibleNodes__34451.where(((node) => !((FocusNode)node).rect.intersect(band__35412).isEmpty));
                    if (System.Linq.Enumerable.Any(inBand__35603))
                    {
                        if (forward)
                        {
                            return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBand__35603.Cast<FocusNode>()).First();
                        }
                        return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBand__35603.Cast<FocusNode>()).Last();
                    }
                    if (forward)
                    {
                        return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodes__34451.Cast<FocusNode>()).First();
                    }
                    return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodes__34451.Cast<FocusNode>()).Last();
                }
            case TraversalDirection.right:
            case TraversalDirection.left:
                {
                    IEnumerable<FocusNode> eligibleNodes__36610 = ((IEnumerable<FocusNode>)(object?)_sortAndFilterHorizontally(direction, ((FocusNode)focusedChild).rect, traversalDescendants.Cast<FocusNode>(), forward: forward));
                    if (!System.Linq.Enumerable.Any(eligibleNodes__36610))
                    {
                        break;
                    }
                    ScrollableState? focusedScrollable__36870 = ((ScrollableState?)(object?)Scrollable.maybeOf(((FocusNode)focusedChild).context!, axis: global::Doroti.Generated.Framework.Painting.Axis.horizontal));
                    if ((focusedScrollable__36870 is not null))
                    {
                        IEnumerable<FocusNode> filteredEligibleNodes__37064 = eligibleNodes__36610.where(((node) => (object.Equals(Scrollable.maybeOf(((FocusNode)node).context!, axis: global::Doroti.Generated.Framework.Painting.Axis.horizontal), focusedScrollable__36870))));
                        if (System.Linq.Enumerable.Any(filteredEligibleNodes__37064))
                        {
                            eligibleNodes__36610 = filteredEligibleNodes__37064;
                        }
                    }
                    if ((object.Equals(direction, TraversalDirection.left)))
                    {
                        eligibleNodes__36610 = System.Linq.Enumerable.Reverse(eligibleNodes__36610.ToList());
                    }
                    var band__37579 = global::Doroti.Flutter.Ui.Rect.fromLTRB(-double.PositiveInfinity, ((FocusNode)focusedChild).rect.top, double.PositiveInfinity, ((FocusNode)focusedChild).rect.bottom);
                    IEnumerable<FocusNode> inBand__37770 = eligibleNodes__36610.where(((node) => !((FocusNode)node).rect.intersect(band__37579).isEmpty));
                    if (System.Linq.Enumerable.Any(inBand__37770))
                    {
                        if (forward)
                        {
                            return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBand__37770.Cast<FocusNode>()).First();
                        }
                        return DirectionalFocusTraversalPolicyMixin._sortByDistancePreferHorizontal(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), inBand__37770.Cast<FocusNode>()).Last();
                    }
                    if (forward)
                    {
                        return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodes__36610.Cast<FocusNode>()).First();
                    }
                    return DirectionalFocusTraversalPolicyMixin._sortClosestEdgesByDistancePreferVertical(((Offset)((dynamic)((FocusNode)focusedChild).rect).center), eligibleNodes__36610.Cast<FocusNode>()).Last();
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
        List<FocusNode> sorted__43670 = nodes.where((direction switch { TraversalDirection.left => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dx <= target.left) : (((dynamic)((FocusNode)node).rect).center.dx >= target.left))))), TraversalDirection.right => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dx >= target.right) : (((dynamic)((FocusNode)node).rect).center.dx <= target.right))))), TraversalDirection.up => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), TraversalDirection.down => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })).ToList().ToList();
        global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted__43670, compare: ((a, b) => ((Offset)((dynamic)((FocusNode)a).rect).center).dx.CompareTo(((Offset)((dynamic)((FocusNode)b).rect).center).dx)));
        return ((IEnumerable<FocusNode>)(object?)sorted__43670);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual IEnumerable<FocusNode> _sortAndFilterVertically(TraversalDirection direction, Rect target, IEnumerable<FocusNode> nodes, bool forward = true)
    {
        DartRuntimePrimitives.Assert(() => ((object.Equals(direction, TraversalDirection.up)) || (object.Equals(direction, TraversalDirection.down))));
        List<FocusNode> sorted__44921 = nodes.where((direction switch { TraversalDirection.up => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dy <= target.top) : (((dynamic)((FocusNode)node).rect).center.dy >= target.top))))), TraversalDirection.down => ((node) => ((!object.Equals(((FocusNode)node).rect, target)) && ((forward ? (((dynamic)((FocusNode)node).rect).center.dy >= target.bottom) : (((dynamic)((FocusNode)node).rect).center.dy <= target.bottom))))), TraversalDirection.left => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), TraversalDirection.right => throw DartRuntimePrimitives.AsException(new DartArgumentError($"Invalid direction {direction}")), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })).ToList().ToList();
        global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.mergeSort<FocusNode>(sorted__44921, compare: ((a, b) => ((Offset)((dynamic)((FocusNode)a).rect).center).dy.CompareTo(((Offset)((dynamic)((FocusNode)b).rect).center).dy)));
        return ((IEnumerable<FocusNode>)(object?)sorted__44921);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _popPolicyDataIfNeeded(TraversalDirection direction, FocusScopeNode nearestScope, FocusNode focusedChild, _FocusTraversalGroupNode__focus_traversal? groupNode)
    {
        _DirectionalPolicyData__focus_traversal? policyData__46064 = this._policyData.GetValueOrDefault(nearestScope);
        if ((((policyData__46064 is not null) && System.Linq.Enumerable.Any(((_DirectionalPolicyData__focus_traversal)policyData__46064).history)) && (!object.Equals(((_DirectionalPolicyData__focus_traversal)policyData__46064).history.First().direction, direction))))
        {
            if ((((_DirectionalPolicyData__focus_traversal)policyData__46064).history.Last().node.parent is null))
            {
                invalidateScopeData(nearestScope);
                return false;
            }
            bool popOrInvalidate(TraversalDirection direction)
            {
                FocusNode lastNode__46887 = ((_DirectionalPolicyData__focus_traversal)policyData__46064).history.removeLast<_DirectionalPolicyDataEntry__focus_traversal>().node;
                if ((!object.Equals(Scrollable.maybeOf(((FocusNode)lastNode__46887).context!), Scrollable.maybeOf(global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus!.context!))))
                {
                    invalidateScopeData(nearestScope);
                    return false;
                }
                ScrollPositionAlignmentPolicy alignmentPolicy__47158 = default!;
                switch (direction)
                {
                    case TraversalDirection.up:
                    case TraversalDirection.left:
                        {
                            alignmentPolicy__47158 = ScrollPositionAlignmentPolicy.keepVisibleAtStart;
                            break;
                        }
                    case TraversalDirection.right:
                    case TraversalDirection.down:
                        {
                            alignmentPolicy__47158 = ScrollPositionAlignmentPolicy.keepVisibleAtEnd;
                            break;
                        }
                }
                _requestFocus(lastNode__46887, alignmentPolicy: DartRuntimePrimitives.RequireValue(alignmentPolicy__47158), groupNode: groupNode);
                return true;
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            switch (direction)
            {
                case TraversalDirection.down:
                case TraversalDirection.up:
                    {
                        switch (((_DirectionalPolicyData__focus_traversal)policyData__46064).history.First().direction)
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
                        switch (((_DirectionalPolicyData__focus_traversal)policyData__46064).history.First().direction)
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
        if (((policyData__46064 is not null) && !System.Linq.Enumerable.Any(((_DirectionalPolicyData__focus_traversal)policyData__46064).history)))
        {
            invalidateScopeData(nearestScope);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _pushPolicyData(TraversalDirection direction, FocusScopeNode nearestScope, FocusNode focusedChild)
    {
        _DirectionalPolicyData__focus_traversal? policyData__49002 = this._policyData.GetValueOrDefault(nearestScope);
        var newEntry__49052 = new _DirectionalPolicyDataEntry__focus_traversal(node: focusedChild, direction: direction);
        if ((policyData__49002 is not null))
        {
            ((_DirectionalPolicyData__focus_traversal)policyData__49002).history.Add(newEntry__49052);
        }
        else
        {
            this._policyData[nearestScope] = new _DirectionalPolicyData__focus_traversal(history: new List<_DirectionalPolicyDataEntry__focus_traversal> { newEntry__49052 });
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
            FocusNode firstNode__49831 = (findFirstFocusInDirection(node, direction) ?? currentNode);
            switch (direction)
            {
                case TraversalDirection.up:
                case TraversalDirection.left:
                    {
                        _requestFocus(firstNode__49831, groupNode, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart);
                        break;
                    }
                case TraversalDirection.right:
                case TraversalDirection.down:
                    {
                        _requestFocus(firstNode__49831, groupNode, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd);
                        break;
                    }
            }
            return true;
        }
        bool nodeHadPrimaryFocus__50452 = ((FocusNode)node).hasPrimaryFocus;
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
        return !nodeHadPrimaryFocus__50452;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _requestFocus(FocusNode node, _FocusTraversalGroupNode__focus_traversal? groupNode, ScrollPositionAlignmentPolicy? alignmentPolicy = null, double? alignment = null, Duration? duration = null, global::Doroti.Generated.Framework.Animation.Curve? curve = null)
    {
        groupNode?.lastRequestedFocus = node;
        this.requestFocusCallback(node, alignmentPolicy: alignmentPolicy, alignment: alignment, duration: duration, curve: curve);
    }

    public virtual bool _onEdgeForDirection(FocusNode currentNode, FocusNode focusedChild, _FocusTraversalGroupNode__focus_traversal? groupNode, TraversalDirection direction, FocusScopeNode? scope = null)
    {
        FocusScopeNode nearestScope__51630 = (scope ?? ((FocusNode)currentNode).nearestScope!);
        FocusNode? found__51696 = default!;
        switch (((FocusScopeNode)nearestScope__51630).directionalTraversalEdgeBehavior)
        {
            case TraversalEdgeBehavior.leaveFlutterView:
                {
                    focusedChild.unfocus();
                    return false;
                }
            case TraversalEdgeBehavior.parentScope:
                {
                    FocusScopeNode? parentScope__51945 = nearestScope__51630.enclosingScope;
                    if (((parentScope__51945 is not null) && (!object.Equals(parentScope__51945, FocusManager.instance.rootScope))))
                    {
                        invalidateScopeData(nearestScope__51630);
                        nearestScope__51630 = parentScope__51945;
                        invalidateScopeData(nearestScope__51630);
                        found__51696 = _findNextFocusInDirection(focusedChild, ((FocusScopeNode)nearestScope__51630).traversalDescendants.Cast<FocusNode>(), direction);
                        if ((found__51696 is null))
                        {
                            return _onEdgeForDirection(currentNode, focusedChild, groupNode, direction, scope: nearestScope__51630);
                        }
                    }
                    else
                    {
                        found__51696 = _findNextFocusInDirection(focusedChild, ((FocusScopeNode)nearestScope__51630).traversalDescendants.Cast<FocusNode>(), direction, forward: false);
                    }
                    break;
                }
            case TraversalEdgeBehavior.closedLoop:
                {
                    found__51696 = _findNextFocusInDirection(focusedChild, ((FocusScopeNode)nearestScope__51630).traversalDescendants.Cast<FocusNode>(), direction, forward: false);
                    break;
                }
            case TraversalEdgeBehavior.stop:
                {
                    return false;
                }
        }
        if ((found__51696 is not null))
        {
            return _requestTraversalFocusInDirection(currentNode, found__51696, DartRuntimePrimitives.ConvertValue<FocusScopeNode>(nearestScope__51630), direction, groupNode);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool inDirection(FocusNode currentNode, TraversalDirection direction)
    {
        _FocusTraversalGroupNode__focus_traversal? groupNode__54345 = ((_FocusTraversalGroupNode__focus_traversal?)(object?)FocusTraversalGroup._getGroupNode(currentNode));
        FocusScopeNode nearestScope__54430 = ((FocusNode)currentNode).nearestScope!;
        FocusNode? focusedChild__54493 = ((FocusScopeNode)nearestScope__54430).focusedChild;
        if ((focusedChild__54493 is null))
        {
            FocusNode firstFocus__54589 = (findFirstFocusInDirection(currentNode, direction) ?? currentNode);
            switch (direction)
            {
                case TraversalDirection.up:
                case TraversalDirection.left:
                    {
                        _requestFocus(firstFocus__54589, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtStart, groupNode: groupNode__54345);
                        break;
                    }
                case TraversalDirection.right:
                case TraversalDirection.down:
                    {
                        _requestFocus(firstFocus__54589, alignmentPolicy: ScrollPositionAlignmentPolicy.keepVisibleAtEnd, groupNode: groupNode__54345);
                        break;
                    }
            }
            return true;
        }
        if (_popPolicyDataIfNeeded(direction, nearestScope__54430, focusedChild__54493, groupNode__54345))
        {
            return true;
        }
        FocusNode? found__55345 = ((FocusNode?)(object?)_findNextFocusInDirection(focusedChild__54493, ((FocusScopeNode)nearestScope__54430).traversalDescendants.Cast<FocusNode>(), direction));
        if ((found__55345 is not null))
        {
            _pushPolicyData(direction, nearestScope__54430, focusedChild__54493);
            return _requestTraversalFocusInDirection(currentNode, found__55345, DartRuntimePrimitives.ConvertValue<FocusScopeNode>(nearestScope__54430), direction, groupNode__54345);
        }
        return _onEdgeForDirection(currentNode, focusedChild__54493, groupNode__54345, direction);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class FocusTraversalOrder : InheritedWidget
{
    public virtual FocusOrder order { get; private set; } = default!;

    public FocusTraversalOrder(global::Doroti.Generated.Framework.Foundation.Key? key = null, FocusOrder order = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.order = order;
    }

    public static FocusOrder of(BuildContext context)
    {
        FocusTraversalOrder? marker__80229 = ((FocusTraversalOrder?)(object?)context.getInheritedWidgetOfExactType<FocusTraversalOrder>());
        DartRuntimePrimitives.Assert(() =>
            {
                if ((marker__80229 is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create("FocusTraversalOrder.of() was called with a context that " + "does not contain a FocusTraversalOrder widget. No TraversalOrder widget " + "ancestor could be found starting from the context that was passed to " + "FocusTraversalOrder.of().\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return marker__80229!.order;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static FocusOrder? maybeOf(BuildContext context)
    {
        FocusTraversalOrder? marker__81252 = ((FocusTraversalOrder?)(object?)context.getInheritedWidgetOfExactType<FocusTraversalOrder>());
        return marker__81252?.order;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => false;
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<FocusOrder>("order", this.order));
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

    public FocusTraversalGroup(global::Doroti.Generated.Framework.Foundation.Key? key = null, FocusTraversalPolicy? policy = null, bool descendantsAreFocusable = true, bool descendantsAreTraversable = true, global::System.Action<FocusNode>? onFocusNodeCreated = null, FocusNode? parentNode = null, Widget child = default!) : base(key: key)
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
        FocusTraversalPolicy? policy__88461 = ((FocusTraversalPolicy?)(object?)FocusTraversalGroup.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((policy__88461 is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create("Unable to find a Focus or FocusScope widget in the given context, or the FocusNode " + "from with the widget that was found is not associated with a FocusTraversalPolicy.\n" + "FocusTraversalGroup.of() was called with a context that does not contain a " + "Focus or FocusScope widget, or there was no FocusTraversalPolicy in effect.\n" + "This can happen if there is not a FocusTraversalGroup that defines the policy, " + "or if the context comes from a widget that is above the WidgetsApp, MaterialApp, " + "or CupertinoApp widget (those widgets introduce an implicit default policy) \n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return policy__88461!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static FocusTraversalPolicy? maybeOf(BuildContext context)
    {
        FocusNode? node__90052 = ((FocusNode?)(object?)Focus.maybeOf(context, scopeOk: true, createDependency: false));
        if ((node__90052 is null))
        {
            return ((FocusTraversalPolicy)(object)null);
        }
        return ((FocusTraversalPolicy?)(object?)FocusTraversalGroup.maybeOfNode(node__90052));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _FocusTraversalGroupState__focus_traversal());
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<FocusTraversalPolicy>("policy", this.policy));
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
        FocusManager.instance.addListener(() => this._handleFocusChanged());
        ((FocusTraversalGroup)this.widget).onFocusNodeCreated?.Invoke(this.focusNode);
    }

    public override void dispose()
    {
        FocusManager.instance.removeListener(() => this._handleFocusChanged());
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
        FocusNode? primaryFocus__92710 = FocusManager.instance.primaryFocus;
        FocusNode? lastRequestedFocus__92782 = ((_FocusTraversalGroupNode__focus_traversal)this.focusNode).lastRequestedFocus;
        if ((lastRequestedFocus__92782 is null))
        {
            return;
        }
        if ((!object.Equals(primaryFocus__92710, lastRequestedFocus__92782)))
        {
            FocusScopeNode? scope__92961 = primaryFocus__92710?.nearestScope;
            while ((scope__92961 is not null))
            {
                ((FocusTraversalGroup)this.widget).policy.invalidateScopeData(scope__92961);
                scope__92961 = scope__92961.enclosingScope;
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
        return global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus!.nextFocus();
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
        return global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus!.previousFocus();
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

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<TraversalDirection>("direction", this.direction));
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
            global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus!.focusInDirection(((DirectionalFocusIntent)intent).direction);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ExcludeFocusTraversal : StatelessWidget
{
    public virtual bool excluding { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public ExcludeFocusTraversal(global::Doroti.Generated.Framework.Foundation.Key? key = null, bool excluding = true, Widget child = default!) : base(key: key)
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

