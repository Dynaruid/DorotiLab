// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/semantics/semantics.dart
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

namespace Doroti.Framework.Semantics;

public delegate bool SemanticsNodeVisitor(SemanticsNode node);

public delegate void MoveCursorHandler(bool extendSelection);

public delegate void SetSelectionHandler(TextSelection selection);

public delegate void SetTextHandler(string text);

public delegate void ScrollToOffsetHandler(Offset targetOffset);

public delegate void SemanticsActionHandler(object? args);

public delegate void SemanticsUpdateCallback(SemanticsUpdate update);

public delegate ChildSemanticsConfigurationsResult ChildSemanticsConfigurationsDelegate(List<SemanticsConfiguration> __unnamed_);

public enum AccessibilityFocusBlockType
{
    none,
    blockSubtree,
    blockNode
}

public static class AccessibilityFocusBlockTypeMembers
{
    internal static AccessibilityFocusBlockType _merge(this AccessibilityFocusBlockType value, AccessibilityFocusBlockType other)
    {
        if (((object.Equals(value, AccessibilityFocusBlockType.blockSubtree)) || (object.Equals(other, AccessibilityFocusBlockType.blockSubtree))))
        {
            return AccessibilityFocusBlockType.blockSubtree;
        }
        if (((object.Equals(value, AccessibilityFocusBlockType.blockNode)) || (object.Equals(other, AccessibilityFocusBlockType.blockNode))))
        {
            return AccessibilityFocusBlockType.blockNode;
        }
        return AccessibilityFocusBlockType.none;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class SemanticsLibrary
{
    internal static long _kUnblockedUserActions = ((long)SemanticsAction.didGainAccessibilityFocus | (long)SemanticsAction.didLoseAccessibilityFocus);
}

internal abstract class _DebugSemanticsRoleChecks__semantics
{
    internal static FlutterError? _checkSemanticsData(SemanticsNode node)
    {
        FlutterError? error = ((Func<SemanticsNode, FlutterError?>)(((SemanticsNode)node).role switch { SemanticsRole.alertDialog => _noCheckRequired, SemanticsRole.dialog => _noCheckRequired, SemanticsRole.none => _noCheckRequired, SemanticsRole.tab => _semanticsTab, SemanticsRole.tabBar => _semanticsTabBar, SemanticsRole.tabPanel => _noCheckRequired, SemanticsRole.table => _semanticsTable, SemanticsRole.cell => _semanticsCell, SemanticsRole.row => _semanticsRow, SemanticsRole.columnHeader => _semanticsColumnHeader, SemanticsRole.radioGroup => _semanticsRadioGroup, SemanticsRole.menu => _semanticsMenu, SemanticsRole.menuBar => _semanticsMenuBar, SemanticsRole.menuItem => _semanticsMenuItem, SemanticsRole.menuItemCheckbox => _semanticsMenuItemCheckbox, SemanticsRole.menuItemRadio => _semanticsMenuItemRadio, SemanticsRole.alert => _noLiveRegion, SemanticsRole.status => _noLiveRegion, SemanticsRole.list => _noCheckRequired, SemanticsRole.listItem => _semanticsListItem, SemanticsRole.complementary => _semanticsComplementary, SemanticsRole.contentInfo => _semanticsContentInfo, SemanticsRole.main => _semanticsMain, SemanticsRole.navigation => _semanticsNavigation, SemanticsRole.region => _semanticsRegion, SemanticsRole.form => _noCheckRequired, SemanticsRole.loadingSpinner => _noCheckRequired, SemanticsRole.progressBar => _semanticsProgressBar, SemanticsRole.dragHandle => _unimplemented, SemanticsRole.spinButton => _unimplemented, SemanticsRole.comboBox => _unimplemented, SemanticsRole.tooltip => _unimplemented, SemanticsRole.hotKey => _unimplemented, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }))(node);
        if ((error is not null))
        {
            return error;
        }
        return _semanticsGeneral(node);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _unimplemented(SemanticsNode node) => new FlutterError($"Missing checks for role {node.getSemanticsData().role}");
    internal static FlutterError? _noCheckRequired(SemanticsNode node) => null;
    internal static FlutterError? _semanticsProgressBar(SemanticsNode node)
    {
        SemanticsData data = node.getSemanticsData();
        if ((((((SemanticsData)data).value.Length == 0) || (((((SemanticsData)data).minValue is null ? (bool?)null : ((SemanticsData)data).minValue.Length == 0) ?? true))) || (((((SemanticsData)data).maxValue is null ? (bool?)null : ((SemanticsData)data).maxValue.Length == 0) ?? true))))
        {
            return new FlutterError("A progress bar must have a value, a minValue, a maxValue.");
        }
        double? minVal = Dart_coreLibrary.tryParse(((SemanticsData)data).minValue!);
        double? maxVal = Dart_coreLibrary.tryParse(((SemanticsData)data).maxValue!);
        double? currentValue = Dart_coreLibrary.tryParse(((SemanticsData)data).value);
        double? percentValue = (((SemanticsData)data).value.endsWith("%") ? Dart_coreLibrary.tryParse(((SemanticsData)data).value.substring(0L, (((SemanticsData)data).value.Length - 1L))) : null);
        if ((((minVal is null) || (maxVal is null)) || (((currentValue is null) && (percentValue is null)))))
        {
            return new FlutterError("Progress bar value, minValue, and maxValue must be valid numbers. " + $"value: \"{((SemanticsData)data).value}\", minValue: \"{((SemanticsData)data).minValue}\", maxValue: \"{((SemanticsData)data).maxValue}\"");
        }
        if ((minVal >= DartRuntimePrimitives.RequireValue(maxVal)))
        {
            return new FlutterError($"Progress bar minValue ({((SemanticsData)data).minValue}) must be less than maxValue ({((SemanticsData)data).maxValue})");
        }
        if ((currentValue is not null))
        {
            double currentValue__8479__value9301 = DartRuntimePrimitives.RequireValue(currentValue);
            if (((DartRuntimePrimitives.RequireValue(currentValue__8479__value9301) < DartRuntimePrimitives.RequireValue(minVal)) || (DartRuntimePrimitives.RequireValue(currentValue__8479__value9301) > DartRuntimePrimitives.RequireValue(maxVal))))
            {
                return new FlutterError($"Progress bar value ({((SemanticsData)data).value}) must be between minValue ({((SemanticsData)data).minValue}) and maxValue ({((SemanticsData)data).maxValue})");
            }
            return null;
        }
        if (((percentValue is not null) && (((DartRuntimePrimitives.RequireValue(percentValue) < 0L) || (DartRuntimePrimitives.RequireValue(percentValue) > 100L)))))
        {
            double percentValue__8541__value9681 = DartRuntimePrimitives.RequireValue(percentValue);
            return new FlutterError($"Progress bar percentage value ({((SemanticsData)data).value}) must be between 0% and 100%");
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsTab(SemanticsNode node)
    {
        SemanticsData data = node.getSemanticsData();
        if ((object.Equals(((SemanticsData)data).flagsCollection.isSelected, Tristate.none)))
        {
            return new FlutterError("A tab needs selected states");
        }
        if (((SemanticsNode)node).areUserActionsBlocked)
        {
            return null;
        }
        if (((!object.Equals(((SemanticsData)data).flagsCollection.isEnabled, Tristate.isFalse)) && !data.hasAction(SemanticsAction.tap)))
        {
            return new FlutterError("A tab must have a tap action");
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsTabBar(SemanticsNode node)
    {
        if ((((SemanticsNode)node).childrenCount < 1L))
        {
            return new FlutterError("a TabBar cannot be empty");
        }
        FlutterError? error = default!;
        node.visitChildren(((Func<SemanticsNode, bool>)((child) =>
        {
            if ((!object.Equals(child.getSemanticsData().role, SemanticsRole.tab)))
            {
                error = new FlutterError("Children of TabBar must have the tab role");
            }
            return (error is null);
            return default;
        })));
        return error;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsTable(SemanticsNode node)
    {
        FlutterError? error = default!;
        node.visitChildren(((Func<SemanticsNode, bool>)((child) =>
        {
            if ((!object.Equals(child.getSemanticsData().role, SemanticsRole.row)))
            {
                error = new FlutterError("Children of Table must have the row role");
            }
            return (error is null);
            return default;
        })));
        return error;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsRow(SemanticsNode node)
    {
        if ((!object.Equals(((SemanticsNode)node).parent?.role, SemanticsRole.table)))
        {
            return new FlutterError("A row must be a child of a table");
        }
        FlutterError? error = default!;
        node.visitChildren(((Func<SemanticsNode, bool>)((child) =>
        {
            if (((!object.Equals(child.getSemanticsData().role, SemanticsRole.cell)) && (!object.Equals(child.getSemanticsData().role, SemanticsRole.columnHeader))))
            {
                error = new FlutterError("Children of Row must have the cell or columnHeader role");
            }
            return (error is null);
            return default;
        })));
        return error;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsCell(SemanticsNode node)
    {
        if (((!object.Equals(((SemanticsNode)node).parent?.role, SemanticsRole.row)) && (!object.Equals(((SemanticsNode)node).parent?.role, SemanticsRole.cell))))
        {
            return new FlutterError("A cell must be a child of a row or another cell");
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsColumnHeader(SemanticsNode node)
    {
        if (((!object.Equals(((SemanticsNode)node).parent?.role, SemanticsRole.row)) && (!object.Equals(((SemanticsNode)node).parent?.role, SemanticsRole.cell))))
        {
            return new FlutterError("A columnHeader must be a child or another cell");
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsRadioGroup(SemanticsNode node)
    {
        FlutterError? error = default!;
        var hasCheckedChild = false;
        bool validateRadioGroupChildren(SemanticsNode node)
        {
            SemanticsData data = node.getSemanticsData();
            if ((object.Equals(((SemanticsData)data).role, SemanticsRole.radioGroup)))
            {
                return (error is null);
            }
            if (!((SemanticsData)data).flagsCollection.isInMutuallyExclusiveGroup)
            {
                node.visitChildren((Func<SemanticsNode, bool>)validateRadioGroupChildren);
                return (error is null);
            }
            if ((object.Equals(((SemanticsData)data).flagsCollection.isChecked, CheckedState.isTrue)))
            {
                if (hasCheckedChild)
                {
                    error = new FlutterError("Radio groups must not have multiple checked children");
                    return false;
                }
                hasCheckedChild = true;
            }
            DartRuntimePrimitives.Assert(() => (error is null));
            return true;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        node.visitChildren((Func<SemanticsNode, bool>)validateRadioGroupChildren);
        return error;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsMenu(SemanticsNode node)
    {
        if ((((SemanticsNode)node).childrenCount < 1L))
        {
            return new FlutterError("a menu cannot be empty");
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsMenuBar(SemanticsNode node)
    {
        if ((((SemanticsNode)node).childrenCount < 1L))
        {
            return new FlutterError("a menu bar cannot be empty");
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsMenuItem(SemanticsNode node)
    {
        SemanticsNode? currentNode = node;
        while ((currentNode?.parent is not null))
        {
            if (((object.Equals(currentNode?.parent?.role, SemanticsRole.menu)) || (object.Equals(currentNode?.parent?.role, SemanticsRole.menuBar))))
            {
                return null;
            }
            currentNode = currentNode?.parent;
        }
        return new FlutterError("A menu item must be a child of a menu or a menu bar");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsMenuItemCheckbox(SemanticsNode node)
    {
        SemanticsData data = node.getSemanticsData();
        if ((object.Equals(((SemanticsData)data).flagsCollection.isChecked, CheckedState.none)))
        {
            return new FlutterError("a menu item checkbox must be checkable");
        }
        SemanticsNode? currentNode = node;
        while ((currentNode?.parent is not null))
        {
            if (((object.Equals(currentNode?.parent?.role, SemanticsRole.menu)) || (object.Equals(currentNode?.parent?.role, SemanticsRole.menuBar))))
            {
                return null;
            }
            currentNode = currentNode?.parent;
        }
        return new FlutterError("A menu item checkbox must be a child of a menu or a menu bar");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsMenuItemRadio(SemanticsNode node)
    {
        SemanticsData data = node.getSemanticsData();
        if ((object.Equals(((SemanticsData)data).flagsCollection.isChecked, CheckedState.none)))
        {
            return new FlutterError("a menu item radio must be checkable");
        }
        SemanticsNode? currentNode = node;
        while ((currentNode?.parent is not null))
        {
            if (((object.Equals(currentNode?.parent?.role, SemanticsRole.menu)) || (object.Equals(currentNode?.parent?.role, SemanticsRole.menuBar))))
            {
                return null;
            }
            currentNode = currentNode?.parent;
        }
        return new FlutterError("A menu item radio must be a child of a menu or a menu bar");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _noLiveRegion(SemanticsNode node)
    {
        SemanticsData data = node.getSemanticsData();
        if (((SemanticsData)data).flagsCollection.isLiveRegion)
        {
            return new FlutterError($"Node {((SemanticsNode)node).id} has role {((SemanticsData)data).role} but is also a live region. " + $"A node can not have {((SemanticsData)data).role} and be live region at the same time. " + "Either remove the role or the live region");
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsListItem(SemanticsNode node)
    {
        SemanticsData data = node.getSemanticsData();
        SemanticsNode? parentLocal = ((SemanticsNode)node).parent;
        if ((parentLocal is null))
        {
            return new FlutterError($"Semantics node {((SemanticsNode)node).id} has role {((SemanticsData)data).role} but doesn't have a parent");
        }
        SemanticsData parentSemanticsData = parentLocal.getSemanticsData();
        if ((!object.Equals(((SemanticsData)parentSemanticsData).role, SemanticsRole.list)))
        {
            return new FlutterError($"Semantics node {((SemanticsNode)node).id} has role {((SemanticsData)data).role}, but its " + $"parent node {((SemanticsNode)parentLocal).id} doesn't have the role {SemanticsRole.list}. " + $"Please assign the {SemanticsRole.list} to node {((SemanticsNode)parentLocal).id}");
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static bool _isLandmarkRole(SemanticsData nodeData) => (((((object.Equals(((SemanticsData)nodeData).role, SemanticsRole.complementary)) || (object.Equals(((SemanticsData)nodeData).role, SemanticsRole.contentInfo))) || (object.Equals(((SemanticsData)nodeData).role, SemanticsRole.main))) || (object.Equals(((SemanticsData)nodeData).role, SemanticsRole.navigation))) || (object.Equals(((SemanticsData)nodeData).role, SemanticsRole.region)));
    internal static bool _isSameRoleExisted(SemanticsNode semanticsNode)
    {
        DartMap<long, SemanticsNode> treeNodes = ((SemanticsNode)semanticsNode).owner!._nodes;
        var sameRoleCount = 0L;
        foreach (long id in treeNodes.Keys)
        {
            if ((object.Equals(treeNodes.GetValueOrDefault(id)?.getSemanticsData().role, ((SemanticsNode)semanticsNode).role)))
            {
                sameRoleCount++;
                if ((sameRoleCount > 1L))
                {
                    return true;
                }
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsComplementary(SemanticsNode node)
    {
        SemanticsNode? currentNode = ((SemanticsNode)node).parent;
        while ((currentNode is not null))
        {
            if (_isLandmarkRole(currentNode.getSemanticsData()))
            {
                return new FlutterError("The complementary landmark role should not contained within any other landmark roles.");
            }
            currentNode = ((SemanticsNode)currentNode).parent;
        }
        SemanticsData data = node.getSemanticsData();
        if ((_isSameRoleExisted(node) && (((SemanticsData)data).label.Length == 0)))
        {
            return new FlutterError("The complementary landmark role should have a unique label as it is used more than once.");
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsContentInfo(SemanticsNode node)
    {
        SemanticsNode? currentNode = ((SemanticsNode)node).parent;
        while ((currentNode is not null))
        {
            if (_isLandmarkRole(currentNode.getSemanticsData()))
            {
                return new FlutterError("The contentInfo landmark role should not contained within any other landmark roles.");
            }
            currentNode = ((SemanticsNode)currentNode).parent;
        }
        SemanticsData data = node.getSemanticsData();
        if ((_isSameRoleExisted(node) && (((SemanticsData)data).label.Length == 0)))
        {
            return new FlutterError("The contentInfo landmark role should have a unique label as it is used more than once.");
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsMain(SemanticsNode node)
    {
        SemanticsNode? currentNode = ((SemanticsNode)node).parent;
        while ((currentNode is not null))
        {
            if (_isLandmarkRole(currentNode.getSemanticsData()))
            {
                return new FlutterError("The main landmark role should not contained within any other landmark roles.");
            }
            currentNode = ((SemanticsNode)currentNode).parent;
        }
        SemanticsData data = node.getSemanticsData();
        if ((_isSameRoleExisted(node) && (((SemanticsData)data).label.Length == 0)))
        {
            return new FlutterError("The main landmark role should have a unique label as it is used more than once.");
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsNavigation(SemanticsNode node)
    {
        SemanticsData data = node.getSemanticsData();
        if ((_isSameRoleExisted(node) && (((SemanticsData)data).label.Length == 0)))
        {
            return new FlutterError("The navigation landmark role should have a unique label as it is used more than once.");
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsRegion(SemanticsNode node)
    {
        SemanticsData data = node.getSemanticsData();
        if ((((SemanticsData)data).label.Length == 0))
        {
            return new FlutterError("A region role should include a label that describes the purpose of the content.");
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsGeneral(SemanticsNode node)
    {
        SemanticsData data = node.getSemanticsData();
        bool? isExpandedLocal = ((SemanticsData)data).flagsCollection.isExpanded.toBoolOrNull();
        if ((isExpandedLocal is not null))
        {
            bool isExpanded__19946__value20016 = DartRuntimePrimitives.RequireValue(isExpandedLocal);
            bool hasExpandAction = data.hasAction(SemanticsAction.expand);
            bool hasCollapseAction = data.hasAction(SemanticsAction.collapse);
            if ((hasExpandAction && hasCollapseAction))
            {
                return new FlutterError("An expandable node cannot have both expand and collapse actions set at the same time.");
            }
            if ((DartRuntimePrimitives.RequireValue(isExpanded__19946__value20016) && hasExpandAction))
            {
                return new FlutterError("An expanded node cannot have an expand action.");
            }
            if ((!DartRuntimePrimitives.RequireValue(isExpanded__19946__value20016) && hasCollapseAction))
            {
                return new FlutterError("A collapsed node cannot have a collapse action.");
            }
        }
        if ((((SemanticsData)data).flagsCollection.isAccessibilityFocusBlocked && (!object.Equals(((SemanticsData)data).flagsCollection.isFocused, Tristate.none))))
        {
            return new FlutterError("A node that is keyboard focusable cannot be set to accessibility unfocusable");
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SemanticsTag
{
    public virtual string name { get; private set; } = default!;

    public SemanticsTag(string name)
    {
        this.name = name;
    }

    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SemanticsTag"))}({this.name})";
}

public class ChildSemanticsConfigurationsResult
{
    public virtual List<SemanticsConfiguration> mergeUp { get; private set; } = default!;
    public virtual List<List<SemanticsConfiguration>> siblingMergeGroups { get; private set; } = default!;

    public ChildSemanticsConfigurationsResult(List<SemanticsConfiguration> mergeUp, List<List<SemanticsConfiguration>> siblingMergeGroups)
    {
        this.mergeUp = mergeUp;
        this.siblingMergeGroups = siblingMergeGroups;
    }

}

public class ChildSemanticsConfigurationsResultBuilder
{
    internal virtual List<SemanticsConfiguration> _mergeUp { get; private set; } = new List<SemanticsConfiguration>();
    internal virtual List<List<SemanticsConfiguration>> _siblingMergeGroups { get; private set; } = new List<List<SemanticsConfiguration>>();

    public ChildSemanticsConfigurationsResultBuilder()
    {
    }

    public virtual void markAsMergeUp(SemanticsConfiguration config) => this._mergeUp.Add(config);
    public virtual void markAsSiblingMergeGroup(List<SemanticsConfiguration> configs) => this._siblingMergeGroups.Add(configs);
    public virtual ChildSemanticsConfigurationsResult build()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                var seenConfigs = new HashSet<SemanticsConfiguration>();
                foreach (var config in new List<SemanticsConfiguration>())
                {
                    DartRuntimePrimitives.Assert(() => seenConfigs.Add(config));
                }
                return true;
            });
        return new ChildSemanticsConfigurationsResult(this._mergeUp, this._siblingMergeGroups);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CustomSemanticsAction
{
    public virtual string? label { get; private set; }
    public virtual string? hint { get; private set; }
    public virtual SemanticsAction? action { get; private set; }
    internal static long _nextId = 0L;
    internal static DartMap<long, CustomSemanticsAction> _actions = new DartMap<long, CustomSemanticsAction>();
    internal static DartMap<CustomSemanticsAction, long> _ids = new DartMap<CustomSemanticsAction, long>();

    public CustomSemanticsAction(string label)
    {
        this.label = label;
        this.hint = null;
        this.action = null;
        System.Diagnostics.Debug.Assert((label != ""));
    }

    public static CustomSemanticsAction CreateOverridingAction(string hint, SemanticsAction action)
    {
        var __instance = new CustomSemanticsAction(default!);
        __instance.hint = hint;
        __instance.action = action;
        __instance.label = null;
        return __instance;
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.label, this.hint, this.action);
    public override bool Equals(object? other)
    {
        var __other = other as CustomSemanticsAction;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((__other is CustomSemanticsAction) && (((CustomSemanticsAction)((CustomSemanticsAction)__other)).label == this.label)) && (((CustomSemanticsAction)((CustomSemanticsAction)__other)).hint == this.hint)) && (object.Equals(((CustomSemanticsAction)((CustomSemanticsAction)__other)).action, this.action)));
    }

    public override string ToString()
    {
        return $"CustomSemanticsAction({_ids.GetValueOrDefault(this)}, label:{this.label}, hint:{this.hint}, action:{this.action})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static long getIdentifier(CustomSemanticsAction action)
    {
        if (!_ids.TryGetValue(action, out var result))
        {
            result = _nextId++;
            _ids[DartRuntimePrimitives.RequireReference(action)] = result;
            _actions[result] = action;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static CustomSemanticsAction? getAction(long id)
    {
        return _actions.GetValueOrDefault(id);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static void resetForTests()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                _actions.Clear();
                _ids.Clear();
                _nextId = 0L;
                return true;
            });
    }

}

public class AttributedString
{
    public virtual string @string { get; private set; } = default!;
    public virtual List<StringAttribute> attributes { get; private set; } = default!;

    public AttributedString(string @string, List<StringAttribute> attributes = default!)
    {
        List<StringAttribute> __attributes = attributes ?? new List<StringAttribute>();
        this.@string = @string;
        this.attributes = __attributes;
        System.Diagnostics.Debug.Assert(((@string.Length != 0) || (checked((long)(__attributes.Count)) == 0)));
        System.Diagnostics.Debug.Assert(((Func<bool>)(() =>
        {
            foreach (var attribute in __attributes)
            {
                DartRuntimePrimitives.Assert(() => ((@string.Length >= attribute.range.start) && (@string.Length >= attribute.range.end)));
            }
            return true;
            return default;
        }))());
    }

    public virtual AttributedString op_Add(AttributedString other)
    {
        if ((this.@string.Length == 0))
        {
            return other;
        }
        if ((((AttributedString)other).@string.Length == 0))
        {
            return this;
        }
        string newString = (this.@string + ((AttributedString)other).@string);
        var newAttributes = new List<global::Doroti.Ui.StringAttribute>(this.attributes);
        if ((checked((long)(((AttributedString)other).attributes.Count)) != 0))
        {
            long offset = this.@string.Length;
            foreach (global::Doroti.Ui.StringAttribute attribute in ((AttributedString)other).attributes)
            {
                var newRange = new global::Doroti.Ui.TextRange(start: (attribute.range.start + offset), end: (attribute.range.end + offset));
                global::Doroti.Ui.StringAttribute adjustedAttribute = attribute.copy(range: newRange);
                newAttributes.Add(adjustedAttribute);
            }
        }
        return new AttributedString(newString, attributes: newAttributes);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as AttributedString;
        if (__other is null) return false;
        return ((((object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())) && (__other is AttributedString)) && (((AttributedString)((AttributedString)__other)).@string == this.@string)) && global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals<global::Doroti.Ui.StringAttribute>(((AttributedString)((AttributedString)__other)).attributes, this.attributes));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.@string, this.attributes);
    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "AttributedString"))}('{this.@string}', attributes: {this.attributes})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class AttributedStringProperty : DiagnosticsProperty<AttributedString>
{
    public virtual bool showWhenEmpty { get; private set; } = default!;

    public AttributedStringProperty(string name, AttributedString? value, bool showName = true, bool showWhenEmpty = false, object? defaultValue = default!, DiagnosticLevel level = DiagnosticLevel.info, string? description = null) : base(name, value, showName: showName, defaultValue: defaultValue ?? global::Doroti.Framework.Foundation.DiagnosticsLibrary.kNoDefaultValue, level: level, description: description)
    {
        this.showWhenEmpty = showWhenEmpty;
    }

    public virtual bool isInteresting => (base.isInteresting && ((this.showWhenEmpty || (((value is not null) && (value!.@string.Length != 0))))));
    public virtual string valueToString(TextTreeConfiguration? parentConfiguration = null)
    {
        if ((value is null))
        {
            return "null";
        }
        string text = value!.@string;
        if (((parentConfiguration is not null) && !parentConfiguration.lineBreakProperties))
        {
            text = text.replaceAll("\n", "\\n");
        }
        if ((checked((long)(value!.attributes.Count)) == 0))
        {
            return $"\"{text}\"";
        }
        return $"\"{text}\" {value!.attributes}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate void _LabelPart__semantics();

public class SemanticsLabelBuilder
{
    public virtual string separator { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }
    internal virtual List<(string, TextDirection?)> _parts { get; private set; } = new List<(string, TextDirection?)>();

    public SemanticsLabelBuilder(string separator = " ", TextDirection? textDirection = null)
    {
        this.separator = separator;
        this.textDirection = textDirection;
    }

    public virtual void addPart(string label, TextDirection? textDirection = null)
    {
        if ((label.Length != 0))
        {
            this._parts.Add((label, textDirection));
        }
    }

    public virtual bool isEmpty => (checked((long)(this._parts.Count)) == 0);
    public virtual long length => checked((long)(this._parts.Count));
    public virtual string build()
    {
        if ((checked((long)(this._parts.Count)) == 0))
        {
            return "";
        }
        if ((checked((long)(this._parts.Count)) == 1L))
        {
            var (text, _) = this._parts.First();
            return text;
        }
        var buffer = new StringBuffer();
        var (firstText, _) = this._parts.First();
        buffer.write(firstText);
        foreach (var (partText, partTextDirection) in this._parts.skip(1L))
        {
            global::Doroti.Ui.TextDirection? partDirection = (partTextDirection ?? this.textDirection);
            if ((this.separator.Length != 0))
            {
                buffer.write(this.separator);
            }
            var processedText = partText;
            if ((((this.textDirection is not null) && (partDirection is not null)) && (!object.Equals(this.textDirection, DartRuntimePrimitives.RequireValue(partDirection)))))
            {
                TextDirection textDirection__value36162 = DartRuntimePrimitives.RequireValue(textDirection);
                TextDirection partDirection__35987__value36187 = DartRuntimePrimitives.RequireValue(partDirection);
                string directionalEmbedding = (DartRuntimePrimitives.RequireValue(partDirection__35987__value36187) switch { TextDirection.rtl => Unicode.RLE, TextDirection.ltr => Unicode.LRE, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                processedText = ((directionalEmbedding + partText) + Unicode.PDF);
            }
            buffer.write(processedText);
        }
        return buffer.ToString();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void clear()
    {
        this._parts.Clear();
    }

}

public class SemanticsData : Diagnosticable
{
    public virtual SemanticsFlags flagsCollection { get; private set; } = default!;
    public virtual long actions { get; private set; } = default!;
    public virtual string identifier { get; private set; } = default!;
    public virtual object? traversalParentIdentifier { get; private set; }
    public virtual object? traversalChildIdentifier { get; private set; }
    public virtual AttributedString attributedLabel { get; private set; } = default!;
    public virtual AttributedString attributedValue { get; private set; } = default!;
    public virtual AttributedString attributedIncreasedValue { get; private set; } = default!;
    public virtual AttributedString attributedDecreasedValue { get; private set; } = default!;
    public virtual AttributedString attributedHint { get; private set; } = default!;
    public virtual string tooltip { get; private set; } = default!;
    public virtual long headingLevel { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }
    public virtual TextSelection? textSelection { get; private set; }
    public virtual long? scrollChildCount { get; private set; }
    public virtual long? scrollIndex { get; private set; }
    public virtual double? scrollPosition { get; private set; }
    public virtual double? scrollExtentMax { get; private set; }
    public virtual double? scrollExtentMin { get; private set; }
    public virtual long? platformViewId { get; private set; }
    public virtual long? maxValueLength { get; private set; }
    public virtual long? currentValueLength { get; private set; }
    public virtual DartUri? linkUrl { get; private set; }
    public virtual Rect rect { get; private set; } = default!;
    public virtual HashSet<SemanticsTag>? tags { get; private set; }
    public virtual Matrix4? transform { get; private set; }
    public virtual List<long>? customSemanticsActionIds { get; private set; }
    public virtual SemanticsRole role { get; private set; } = default!;
    public virtual HashSet<string>? controlsNodes { get; private set; }
    public virtual SemanticsValidationResult validationResult { get; private set; } = default!;
    public virtual SemanticsHitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual SemanticsInputType inputType { get; private set; } = default!;
    public virtual Locale? locale { get; private set; }
    public virtual string? maxValue { get; private set; }
    public virtual string? minValue { get; private set; }

    public SemanticsData(SemanticsFlags flagsCollection, long actions, string identifier, object? traversalParentIdentifier, object? traversalChildIdentifier, AttributedString attributedLabel, AttributedString attributedValue, AttributedString attributedIncreasedValue, AttributedString attributedDecreasedValue, AttributedString attributedHint, string tooltip, TextDirection? textDirection, Rect rect, TextSelection? textSelection, long? scrollIndex, long? scrollChildCount, double? scrollPosition, double? scrollExtentMax, double? scrollExtentMin, long? platformViewId, long? maxValueLength, long? currentValueLength, long headingLevel, DartUri? linkUrl, SemanticsRole role, HashSet<string>? controlsNodes, SemanticsValidationResult validationResult, SemanticsHitTestBehavior hitTestBehavior, SemanticsInputType inputType, Locale? locale, string? minValue, string? maxValue, HashSet<SemanticsTag>? tags = null, Matrix4? transform = null, List<long>? customSemanticsActionIds = null)
    {
        this.flagsCollection = flagsCollection;
        this.actions = actions;
        this.identifier = identifier;
        this.traversalParentIdentifier = traversalParentIdentifier;
        this.traversalChildIdentifier = traversalChildIdentifier;
        this.attributedLabel = attributedLabel;
        this.attributedValue = attributedValue;
        this.attributedIncreasedValue = attributedIncreasedValue;
        this.attributedDecreasedValue = attributedDecreasedValue;
        this.attributedHint = attributedHint;
        this.tooltip = tooltip;
        this.textDirection = textDirection;
        this.rect = rect;
        this.textSelection = textSelection;
        this.scrollIndex = scrollIndex;
        this.scrollChildCount = scrollChildCount;
        this.scrollPosition = scrollPosition;
        this.scrollExtentMax = scrollExtentMax;
        this.scrollExtentMin = scrollExtentMin;
        this.platformViewId = platformViewId;
        this.maxValueLength = maxValueLength;
        this.currentValueLength = currentValueLength;
        this.headingLevel = headingLevel;
        this.linkUrl = linkUrl;
        this.role = role;
        this.controlsNodes = controlsNodes;
        this.validationResult = validationResult;
        this.hitTestBehavior = hitTestBehavior;
        this.inputType = inputType;
        this.locale = locale;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.tags = tags;
        this.transform = transform;
        this.customSemanticsActionIds = customSemanticsActionIds;
        System.Diagnostics.Debug.Assert(((tooltip == "") || (textDirection is not null)));
        System.Diagnostics.Debug.Assert(((((AttributedString)attributedLabel).@string == "") || (textDirection is not null)));
        System.Diagnostics.Debug.Assert(((((AttributedString)attributedValue).@string == "") || (textDirection is not null)));
        System.Diagnostics.Debug.Assert(((((AttributedString)attributedDecreasedValue).@string == "") || (textDirection is not null)));
        System.Diagnostics.Debug.Assert(((((AttributedString)attributedIncreasedValue).@string == "") || (textDirection is not null)));
        System.Diagnostics.Debug.Assert(((((AttributedString)attributedHint).@string == "") || (textDirection is not null)));
        System.Diagnostics.Debug.Assert(((headingLevel >= 0L) && (headingLevel <= 6L)));
        System.Diagnostics.Debug.Assert(((linkUrl is null) || flagsCollection.isLink));
    }

    public virtual long flags => SemanticsLibrary._toBitMask(this.flagsCollection);
    public virtual string label => ((AttributedString)this.attributedLabel).@string;
    public virtual string value => ((AttributedString)this.attributedValue).@string;
    public virtual string increasedValue => ((AttributedString)this.attributedIncreasedValue).@string;
    public virtual string decreasedValue => ((AttributedString)this.attributedDecreasedValue).@string;
    public virtual string hint => ((AttributedString)this.attributedHint).@string;
    public virtual bool hasFlag(SemanticsFlag flag) => (((this.flags & FoundationRuntimePorts.EnumIndex(flag))) != 0L);
    public virtual bool hasAction(SemanticsAction action) => ((this.actions & (long)action) != 0L);
    public virtual string toStringShort() => global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SemanticsData");
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Rect>("rect", this.rect, showName: false));
        properties.add(new global::Doroti.Framework.Painting.TransformProperty("transform", this.transform, showName: false, defaultValue: null));
        var actionSummary = new List<string>();
        List<string?> customSemanticsActionSummary = this.customSemanticsActionIds!.map<long, string?>(((actionId) => CustomSemanticsAction.getAction(actionId)!.label)).ToList();
        properties.add(new IterableProperty<string>("actions", actionSummary, ifEmpty: null));
        properties.add(new IterableProperty<string?>("customActions", customSemanticsActionSummary, ifEmpty: null));
        List<string> flagSummary = this.flagsCollection.toStrings();
        properties.add(new IterableProperty<string>("flags", flagSummary, ifEmpty: null));
        properties.add(new StringProperty("identifier", this.identifier, defaultValue: ""));
        properties.add(new DiagnosticsProperty<object>("traversalParentIdentifier", this.traversalParentIdentifier, defaultValue: null));
        properties.add(new DiagnosticsProperty<object>("traversalChildIdentifier", this.traversalChildIdentifier, defaultValue: null));
        properties.add(new AttributedStringProperty("label", this.attributedLabel));
        properties.add(new AttributedStringProperty("value", this.attributedValue));
        properties.add(new AttributedStringProperty("increasedValue", this.attributedIncreasedValue));
        properties.add(new AttributedStringProperty("decreasedValue", this.attributedDecreasedValue));
        properties.add(new AttributedStringProperty("hint", this.attributedHint));
        properties.add(new StringProperty("tooltip", this.tooltip, defaultValue: ""));
        properties.add(new EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
        if ((this.textSelection?.isValid ?? false))
        {
            properties.add(new MessageProperty("textSelection", $"[{this.textSelection!.start}, {this.textSelection!.end}]"));
        }
        properties.add(new IntProperty("platformViewId", this.platformViewId, defaultValue: null));
        properties.add(new IntProperty("maxValueLength", this.maxValueLength, defaultValue: null));
        properties.add(new IntProperty("currentValueLength", this.currentValueLength, defaultValue: null));
        properties.add(new IntProperty("scrollChildren", this.scrollChildCount, defaultValue: null));
        properties.add(new IntProperty("scrollIndex", this.scrollIndex, defaultValue: null));
        properties.add(new DoubleProperty("scrollExtentMin", this.scrollExtentMin, defaultValue: null));
        properties.add(new DoubleProperty("scrollPosition", this.scrollPosition, defaultValue: null));
        properties.add(new DoubleProperty("scrollExtentMax", this.scrollExtentMax, defaultValue: null));
        properties.add(new IntProperty("headingLevel", this.headingLevel, defaultValue: 0L));
        properties.add(new DiagnosticsProperty<DartUri>("linkUrl", this.linkUrl, defaultValue: null));
        if ((this.controlsNodes is not null))
        {
            properties.add(new IterableProperty<string>("controls", this.controlsNodes, ifEmpty: null));
        }
        if ((!object.Equals(this.role, SemanticsRole.none)))
        {
            properties.add(new EnumProperty<global::Doroti.Ui.SemanticsRole>("role", this.role, defaultValue: SemanticsRole.none));
        }
        if ((!object.Equals(this.validationResult, SemanticsValidationResult.none)))
        {
            properties.add(new EnumProperty<global::Doroti.Ui.SemanticsValidationResult>("validationResult", this.validationResult, defaultValue: SemanticsValidationResult.none));
        }
        properties.add(new StringProperty("minValue", this.minValue, defaultValue: null));
        properties.add(new StringProperty("maxValue", this.maxValue, defaultValue: null));
    }

    public override bool Equals(object? other)
    {
        var __other = other as SemanticsData;
        if (__other is null) return false;
        return (((((((((((((((((((((((((((((((((((((__other is SemanticsData) && (((SemanticsData)((SemanticsData)__other)).flags == this.flags)) && (((SemanticsData)((SemanticsData)__other)).actions == this.actions)) && (((SemanticsData)((SemanticsData)__other)).identifier == this.identifier)) && (object.Equals(((SemanticsData)((SemanticsData)__other)).traversalParentIdentifier, this.traversalParentIdentifier))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).traversalChildIdentifier, this.traversalChildIdentifier))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).attributedLabel, this.attributedLabel))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).attributedValue, this.attributedValue))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).attributedIncreasedValue, this.attributedIncreasedValue))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).attributedDecreasedValue, this.attributedDecreasedValue))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).attributedHint, this.attributedHint))) && (((SemanticsData)((SemanticsData)__other)).tooltip == this.tooltip)) && (object.Equals(((SemanticsData)((SemanticsData)__other)).textDirection, this.textDirection))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).rect, this.rect))) && global::Doroti.Framework.Foundation.CollectionsLibrary.setEquals(((SemanticsData)((SemanticsData)__other)).tags, this.tags)) && (((SemanticsData)((SemanticsData)__other)).scrollChildCount == this.scrollChildCount)) && (((SemanticsData)((SemanticsData)__other)).scrollIndex == this.scrollIndex)) && (object.Equals(((SemanticsData)((SemanticsData)__other)).textSelection, this.textSelection))) && (((SemanticsData)((SemanticsData)__other)).scrollPosition == this.scrollPosition)) && (((SemanticsData)((SemanticsData)__other)).scrollExtentMax == this.scrollExtentMax)) && (((SemanticsData)((SemanticsData)__other)).scrollExtentMin == this.scrollExtentMin)) && (((SemanticsData)((SemanticsData)__other)).platformViewId == this.platformViewId)) && (((SemanticsData)((SemanticsData)__other)).maxValueLength == this.maxValueLength)) && (((SemanticsData)((SemanticsData)__other)).currentValueLength == this.currentValueLength)) && (object.Equals(((SemanticsData)((SemanticsData)__other)).transform, this.transform))) && (((SemanticsData)((SemanticsData)__other)).headingLevel == this.headingLevel)) && (object.Equals(((SemanticsData)((SemanticsData)__other)).linkUrl, this.linkUrl))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).role, this.role))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).validationResult, this.validationResult))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).inputType, this.inputType))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).hitTestBehavior, this.hitTestBehavior))) && _sortedListsEqual(((SemanticsData)((SemanticsData)__other)).customSemanticsActionIds, this.customSemanticsActionIds)) && global::Doroti.Framework.Foundation.CollectionsLibrary.setEquals<string>(this.controlsNodes, ((SemanticsData)((SemanticsData)__other)).controlsNodes)) && (object.Equals(((SemanticsData)((SemanticsData)__other)).traversalParentIdentifier, this.traversalParentIdentifier))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).traversalChildIdentifier, this.traversalChildIdentifier))) && (((SemanticsData)((SemanticsData)__other)).minValue == this.minValue)) && (((SemanticsData)((SemanticsData)__other)).maxValue == this.maxValue));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.flags, this.actions, this.identifier, this.attributedLabel, this.attributedValue, this.attributedIncreasedValue, this.attributedDecreasedValue, this.attributedHint, this.tooltip, this.textDirection, this.rect, this.tags, this.textSelection, this.scrollChildCount, this.scrollIndex, this.scrollPosition, this.scrollExtentMax, this.scrollExtentMin, this.platformViewId, FoundationRuntimePorts.ObjectHash(this.maxValueLength, this.currentValueLength, this.transform, this.headingLevel, this.linkUrl, ((this.customSemanticsActionIds is null) ? null : FoundationRuntimePorts.ObjectHashAll(this.customSemanticsActionIds!)), this.role, this.validationResult, ((this.controlsNodes is null) ? null : FoundationRuntimePorts.ObjectHashAll(this.controlsNodes!)), this.inputType, this.hitTestBehavior, this.traversalParentIdentifier, this.traversalChildIdentifier, this.minValue, this.maxValue));
    internal static bool _sortedListsEqual(List<long>? left, List<long>? right)
    {
        if (((left is null) && (right is null)))
        {
            return true;
        }
        if (((left is not null) && (right is not null)))
        {
            if ((checked((long)(left.Count)) != checked((long)(right.Count))))
            {
                return false;
            }
            for (var i = 0L; (i < checked((long)(left.Count))); i++)
            {
                if ((left[(int)(i)] != right[(int)(i)]))
                {
                    return false;
                }
            }
            return true;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SemanticsDiagnosticableNode__semantics : DiagnosticableNode<SemanticsNode>
{
    public virtual DebugSemanticsDumpOrder childOrder { get; private set; } = default!;

    internal _SemanticsDiagnosticableNode__semantics(string? name = null, SemanticsNode value = default!, DiagnosticsTreeStyle? style = default!, DebugSemanticsDumpOrder childOrder = default!) : base(name: name, value: value, style: DartRuntimePrimitives.RequireValue(style))
    {
        this.childOrder = childOrder;
    }

    public virtual List<DiagnosticsNode> getChildren() => value.debugDescribeChildren(childOrder: this.childOrder);
}

public class SemanticsHintOverrides : DiagnosticableTree
{
    public virtual string? onTapHint { get; private set; }
    public virtual string? onLongPressHint { get; private set; }

    public SemanticsHintOverrides(string? onTapHint = null, string? onLongPressHint = null)
    {
        this.onTapHint = onTapHint;
        this.onLongPressHint = onLongPressHint;
        System.Diagnostics.Debug.Assert((onTapHint != ""));
        System.Diagnostics.Debug.Assert((onLongPressHint != ""));
    }

    public virtual bool isNotEmpty => ((this.onTapHint is not null) || (this.onLongPressHint is not null));
    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.onTapHint, this.onLongPressHint);
    public override bool Equals(object? other)
    {
        var __other = other as SemanticsHintOverrides;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is SemanticsHintOverrides) && (((SemanticsHintOverrides)((SemanticsHintOverrides)__other)).onTapHint == this.onTapHint)) && (((SemanticsHintOverrides)((SemanticsHintOverrides)__other)).onLongPressHint == this.onLongPressHint));
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new StringProperty("onTapHint", this.onTapHint, defaultValue: null));
        properties.add(new StringProperty("onLongPressHint", this.onLongPressHint, defaultValue: null));
    }

    public virtual string toStringDeep(string prefixLineOne = "", string? prefixOtherLines = null, DiagnosticLevel minLevel = DiagnosticLevel.debug, long? wrapWidth = null) =>
        ((DiagnosticableTree)this).toStringDeep(prefixLineOne, prefixOtherLines, minLevel, wrapWidth);
}

public class SemanticsProperties : DiagnosticableTree
{
    public virtual bool? enabled { get; private set; }
    public virtual bool? @checked { get; private set; }
    public virtual bool? mixed { get; private set; }
    public virtual bool? expanded { get; private set; }
    public virtual bool? toggled { get; private set; }
    public virtual bool? selected { get; private set; }
    public virtual bool? button { get; private set; }
    public virtual bool? link { get; private set; }
    public virtual bool? header { get; private set; }
    public virtual bool? textField { get; private set; }
    public virtual bool? slider { get; private set; }
    public virtual bool? keyboardKey { get; private set; }
    public virtual bool? readOnly { get; private set; }
    public virtual bool? focusable { get; private set; }
    public virtual bool? focused { get; private set; }
    public virtual AccessibilityFocusBlockType? accessibilityFocusBlockType { get; private set; }
    public virtual bool? inMutuallyExclusiveGroup { get; private set; }
    public virtual bool? hidden { get; private set; }
    public virtual bool? obscured { get; private set; }
    public virtual bool? multiline { get; private set; }
    public virtual bool? scopesRoute { get; private set; }
    public virtual bool? namesRoute { get; private set; }
    public virtual bool? image { get; private set; }
    public virtual bool? liveRegion { get; private set; }
    public virtual bool? isRequired { get; private set; }
    public virtual long? maxValueLength { get; private set; }
    public virtual long? currentValueLength { get; private set; }
    public virtual string? identifier { get; private set; }
    public virtual object? traversalParentIdentifier { get; private set; }
    public virtual object? traversalChildIdentifier { get; private set; }
    public virtual string? label { get; private set; }
    public virtual AttributedString? attributedLabel { get; private set; }
    public virtual string? value { get; private set; }
    public virtual AttributedString? attributedValue { get; private set; }
    public virtual string? increasedValue { get; private set; }
    public virtual AttributedString? attributedIncreasedValue { get; private set; }
    public virtual string? decreasedValue { get; private set; }
    public virtual AttributedString? attributedDecreasedValue { get; private set; }
    public virtual string? hint { get; private set; }
    public virtual AttributedString? attributedHint { get; private set; }
    public virtual string? tooltip { get; private set; }
    public virtual long? headingLevel { get; private set; }
    public virtual SemanticsHintOverrides? hintOverrides { get; private set; }
    public virtual TextDirection? textDirection { get; private set; }
    public virtual SemanticsSortKey? sortKey { get; private set; }
    public virtual SemanticsTag? tagForChildren { get; private set; }
    public virtual DartUri? linkUrl { get; private set; }
    public virtual Action? onTap { get; private set; }
    public virtual Action? onLongPress { get; private set; }
    public virtual Action? onScrollLeft { get; private set; }
    public virtual Action? onScrollRight { get; private set; }
    public virtual Action? onScrollUp { get; private set; }
    public virtual Action? onScrollDown { get; private set; }
    public virtual Action? onIncrease { get; private set; }
    public virtual Action? onDecrease { get; private set; }
    public virtual Action? onCopy { get; private set; }
    public virtual Action? onCut { get; private set; }
    public virtual Action? onPaste { get; private set; }
    public virtual Action<bool>? onMoveCursorForwardByCharacter { get; private set; }
    public virtual Action<bool>? onMoveCursorBackwardByCharacter { get; private set; }
    public virtual Action<bool>? onMoveCursorForwardByWord { get; private set; }
    public virtual Action<bool>? onMoveCursorBackwardByWord { get; private set; }
    public virtual Action<TextSelection>? onSetSelection { get; private set; }
    public virtual Action<string>? onSetText { get; private set; }
    public virtual Action? onDidGainAccessibilityFocus { get; private set; }
    public virtual Action? onDidLoseAccessibilityFocus { get; private set; }
    public virtual Action? onFocus { get; private set; }
    public virtual Action? onDismiss { get; private set; }
    public virtual Action? onExpand { get; private set; }
    public virtual Action? onCollapse { get; private set; }
    public virtual DartMap<CustomSemanticsAction, Action>? customSemanticsActions { get; private set; }
    public virtual SemanticsRole? role { get; private set; }
    public virtual HashSet<string>? controlsNodes { get; private set; }
    public virtual SemanticsValidationResult validationResult { get; private set; } = default!;
    public virtual SemanticsHitTestBehavior? hitTestBehavior { get; private set; }
    public virtual SemanticsInputType? inputType { get; private set; }
    public virtual string? maxValue { get; private set; }
    public virtual string? minValue { get; private set; }

    public SemanticsProperties(bool? enabled = null, bool? @checked = null, bool? mixed = null, bool? expanded = null, bool? selected = null, bool? toggled = null, bool? button = null, bool? link = null, DartUri? linkUrl = null, bool? header = null, long? headingLevel = null, bool? textField = null, bool? slider = null, bool? keyboardKey = null, bool? readOnly = null, bool? focusable = null, bool? focused = null, AccessibilityFocusBlockType? accessibilityFocusBlockType = null, bool? inMutuallyExclusiveGroup = null, bool? hidden = null, bool? obscured = null, bool? multiline = null, bool? scopesRoute = null, bool? namesRoute = null, bool? image = null, bool? liveRegion = null, bool? isRequired = null, long? maxValueLength = null, long? currentValueLength = null, string? identifier = null, object? traversalParentIdentifier = null, object? traversalChildIdentifier = null, string? label = null, AttributedString? attributedLabel = null, string? value = null, AttributedString? attributedValue = null, string? increasedValue = null, AttributedString? attributedIncreasedValue = null, string? decreasedValue = null, AttributedString? attributedDecreasedValue = null, string? hint = null, string? tooltip = null, AttributedString? attributedHint = null, SemanticsHintOverrides? hintOverrides = null, TextDirection? textDirection = null, SemanticsSortKey? sortKey = null, SemanticsTag? tagForChildren = null, SemanticsRole? role = null, HashSet<string>? controlsNodes = null, SemanticsInputType? inputType = null, SemanticsValidationResult validationResult = SemanticsValidationResult.none, SemanticsHitTestBehavior? hitTestBehavior = null, Action? onTap = null, Action? onLongPress = null, Action? onScrollLeft = null, Action? onScrollRight = null, Action? onScrollUp = null, Action? onScrollDown = null, Action? onIncrease = null, Action? onDecrease = null, Action? onCopy = null, Action? onCut = null, Action? onPaste = null, Action<bool>? onMoveCursorForwardByCharacter = null, Action<bool>? onMoveCursorBackwardByCharacter = null, Action<bool>? onMoveCursorForwardByWord = null, Action<bool>? onMoveCursorBackwardByWord = null, Action<TextSelection>? onSetSelection = null, Action<string>? onSetText = null, Action? onDidGainAccessibilityFocus = null, Action? onDidLoseAccessibilityFocus = null, Action? onFocus = null, Action? onDismiss = null, Action? onExpand = null, Action? onCollapse = null, DartMap<CustomSemanticsAction, Action>? customSemanticsActions = null, string? minValue = null, string? maxValue = null)
    {
        this.enabled = enabled;
        this.@checked = @checked;
        this.mixed = mixed;
        this.expanded = expanded;
        this.selected = selected;
        this.toggled = toggled;
        this.button = button;
        this.link = link;
        this.linkUrl = linkUrl;
        this.header = header;
        this.headingLevel = headingLevel;
        this.textField = textField;
        this.slider = slider;
        this.keyboardKey = keyboardKey;
        this.readOnly = readOnly;
        this.focusable = focusable;
        this.focused = focused;
        this.accessibilityFocusBlockType = accessibilityFocusBlockType;
        this.inMutuallyExclusiveGroup = inMutuallyExclusiveGroup;
        this.hidden = hidden;
        this.obscured = obscured;
        this.multiline = multiline;
        this.scopesRoute = scopesRoute;
        this.namesRoute = namesRoute;
        this.image = image;
        this.liveRegion = liveRegion;
        this.isRequired = isRequired;
        this.maxValueLength = maxValueLength;
        this.currentValueLength = currentValueLength;
        this.identifier = identifier;
        this.traversalParentIdentifier = traversalParentIdentifier;
        this.traversalChildIdentifier = traversalChildIdentifier;
        this.label = label;
        this.attributedLabel = attributedLabel;
        this.value = value;
        this.attributedValue = attributedValue;
        this.increasedValue = increasedValue;
        this.attributedIncreasedValue = attributedIncreasedValue;
        this.decreasedValue = decreasedValue;
        this.attributedDecreasedValue = attributedDecreasedValue;
        this.hint = hint;
        this.tooltip = tooltip;
        this.attributedHint = attributedHint;
        this.hintOverrides = hintOverrides;
        this.textDirection = textDirection;
        this.sortKey = sortKey;
        this.tagForChildren = tagForChildren;
        this.role = role;
        this.controlsNodes = controlsNodes;
        this.inputType = inputType;
        this.validationResult = validationResult;
        this.hitTestBehavior = hitTestBehavior;
        this.onTap = onTap;
        this.onLongPress = onLongPress;
        this.onScrollLeft = onScrollLeft;
        this.onScrollRight = onScrollRight;
        this.onScrollUp = onScrollUp;
        this.onScrollDown = onScrollDown;
        this.onIncrease = onIncrease;
        this.onDecrease = onDecrease;
        this.onCopy = onCopy;
        this.onCut = onCut;
        this.onPaste = onPaste;
        this.onMoveCursorForwardByCharacter = onMoveCursorForwardByCharacter;
        this.onMoveCursorBackwardByCharacter = onMoveCursorBackwardByCharacter;
        this.onMoveCursorForwardByWord = onMoveCursorForwardByWord;
        this.onMoveCursorBackwardByWord = onMoveCursorBackwardByWord;
        this.onSetSelection = onSetSelection;
        this.onSetText = onSetText;
        this.onDidGainAccessibilityFocus = onDidGainAccessibilityFocus;
        this.onDidLoseAccessibilityFocus = onDidLoseAccessibilityFocus;
        this.onFocus = onFocus;
        this.onDismiss = onDismiss;
        this.onExpand = onExpand;
        this.onCollapse = onCollapse;
        this.customSemanticsActions = customSemanticsActions;
        this.minValue = minValue;
        this.maxValue = maxValue;
        System.Diagnostics.Debug.Assert(((label is null) || (attributedLabel is null)));
        System.Diagnostics.Debug.Assert(((value is null) || (attributedValue is null)));
        System.Diagnostics.Debug.Assert(((increasedValue is null) || (attributedIncreasedValue is null)));
        System.Diagnostics.Debug.Assert(((decreasedValue is null) || (attributedDecreasedValue is null)));
        System.Diagnostics.Debug.Assert(((hint is null) || (attributedHint is null)));
        System.Diagnostics.Debug.Assert(((headingLevel is null) || (((DartRuntimePrimitives.RequireValue(headingLevel) > 0L) && (headingLevel <= 6L)))));
        System.Diagnostics.Debug.Assert(((linkUrl is null) || ((link ?? false))));
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("checked", this.@checked, defaultValue: null));
        properties.add(new DiagnosticsProperty<bool>("mixed", this.mixed, defaultValue: null));
        properties.add(new DiagnosticsProperty<bool>("expanded", this.expanded, defaultValue: null));
        properties.add(new DiagnosticsProperty<bool>("selected", this.selected, defaultValue: null));
        properties.add(new DiagnosticsProperty<bool>("isRequired", this.isRequired, defaultValue: null));
        properties.add(new StringProperty("identifier", this.identifier, defaultValue: null));
        properties.add(new DiagnosticsProperty<object>("traversalParentIdentifier", this.traversalParentIdentifier, defaultValue: null));
        properties.add(new DiagnosticsProperty<object>("traversalChildIdentifier", this.traversalChildIdentifier, defaultValue: null));
        properties.add(new StringProperty("label", this.label, defaultValue: null));
        properties.add(new AttributedStringProperty("attributedLabel", this.attributedLabel, defaultValue: null));
        properties.add(new StringProperty("value", this.value, defaultValue: null));
        properties.add(new AttributedStringProperty("attributedValue", this.attributedValue, defaultValue: null));
        properties.add(new StringProperty("increasedValue", this.value, defaultValue: null));
        properties.add(new AttributedStringProperty("attributedIncreasedValue", this.attributedIncreasedValue, defaultValue: null));
        properties.add(new StringProperty("decreasedValue", this.value, defaultValue: null));
        properties.add(new AttributedStringProperty("attributedDecreasedValue", this.attributedDecreasedValue, defaultValue: null));
        properties.add(new StringProperty("hint", this.hint, defaultValue: null));
        properties.add(new AttributedStringProperty("attributedHint", this.attributedHint, defaultValue: null));
        properties.add(new StringProperty("tooltip", this.tooltip, defaultValue: null));
        properties.add(new EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
        properties.add(new EnumProperty<global::Doroti.Ui.SemanticsRole>("role", this.role, defaultValue: null));
        properties.add(new EnumProperty<global::Doroti.Ui.SemanticsValidationResult>("validationResult", this.validationResult, defaultValue: SemanticsValidationResult.none));
        properties.add(new DiagnosticsProperty<SemanticsSortKey>("sortKey", this.sortKey, defaultValue: null));
        properties.add(new DiagnosticsProperty<SemanticsHintOverrides>("hintOverrides", this.hintOverrides, defaultValue: null));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SemanticsProperties");
    public virtual string toStringDeep(string prefixLineOne = "", string? prefixOtherLines = null, DiagnosticLevel minLevel = DiagnosticLevel.debug, long? wrapWidth = null) =>
        ((DiagnosticableTree)this).toStringDeep(prefixLineOne, prefixOtherLines, minLevel, wrapWidth);
}

public static partial class SemanticsLibrary
{
    public static void debugResetSemanticsIdCounter()
    {
        SemanticsNode._lastIdentifier = 0L;
    }
}

public class SemanticsNode : DiagnosticableTreeMixin
{
    internal static long _maxFrameworkAccessibilityIdentifier = (((1L << (int)(16L))) - 1L);
    internal static long _lastIdentifier = 0L;
    public virtual Key? key { get; private set; }
    internal virtual long _id { get; set; } = default!;
    internal virtual Action? _showOnScreen { get; private set; }
    internal virtual Matrix4? _transform { get; set; } = default;
    internal virtual Matrix4? _traversalChildTransform { get; set; } = default;
    internal virtual Rect _rect { get; set; } = Rect.zero;
    public virtual Rect? parentSemanticsClipRect { get; set; } = default;
    public virtual Rect? parentPaintClipRect { get; set; } = default;
    public virtual long? indexInParent { get; set; } = default;
    internal virtual bool _isMergedIntoParent { get; set; } = false;
    internal virtual bool _areUserActionsBlocked { get; set; } = false;
    internal virtual bool _mergeAllDescendantsIntoThisNode { get; set; } = ((SemanticsConfiguration)_kEmptyConfig).isMergingSemanticsOfDescendants;
    internal virtual List<SemanticsNode>? _children { get; set; } = default;
    internal virtual List<SemanticsNode> _debugPreviousSnapshot { get; set; } = default!;
    internal virtual bool _dead { get; set; } = false;
    internal virtual SemanticsOwner? _owner { get; set; } = default;
    internal virtual SemanticsNode? _parent { get; set; } = default;
    internal virtual SemanticsNode? _traversalParent { get; set; } = default;
    internal virtual long _depth { get; set; } = 0L;
    internal virtual Locale? _locale { get; set; } = default;
    internal virtual bool _dirty { get; set; } = false;
    internal virtual DartMap<SemanticsAction, Action<object?>> _actions { get; set; } = ((SemanticsConfiguration)_kEmptyConfig)._actions;
    internal virtual DartMap<CustomSemanticsAction, Action> _customSemanticsActions { get; set; } = ((SemanticsConfiguration)_kEmptyConfig)._customSemanticsActions;
    internal virtual long _actionsAsBits { get; set; } = ((SemanticsConfiguration)_kEmptyConfig)._actionsAsBits;
    public virtual HashSet<SemanticsTag>? tags { get; set; } = default;
    internal virtual SemanticsFlags _flags { get; set; } = SemanticsFlags.none;
    internal virtual string _identifier { get; set; } = ((SemanticsConfiguration)_kEmptyConfig).identifier;
    internal virtual object? _traversalParentIdentifier { get; set; } = default;
    internal virtual object? _traversalChildIdentifier { get; set; } = default;
    internal virtual AttributedString _attributedLabel { get; set; } = ((SemanticsConfiguration)_kEmptyConfig).attributedLabel;
    internal virtual AttributedString _attributedValue { get; set; } = ((SemanticsConfiguration)_kEmptyConfig).attributedValue;
    internal virtual AttributedString _attributedIncreasedValue { get; set; } = ((SemanticsConfiguration)_kEmptyConfig).attributedIncreasedValue;
    internal virtual AttributedString _attributedDecreasedValue { get; set; } = ((SemanticsConfiguration)_kEmptyConfig).attributedDecreasedValue;
    internal virtual AttributedString _attributedHint { get; set; } = ((SemanticsConfiguration)_kEmptyConfig).attributedHint;
    internal virtual string _tooltip { get; set; } = ((SemanticsConfiguration)_kEmptyConfig).tooltip;
    internal virtual SemanticsHintOverrides? _hintOverrides { get; set; } = default;
    internal virtual global::Doroti.Ui.TextDirection? _textDirection { get; set; } = ((SemanticsConfiguration)_kEmptyConfig).textDirection;
    internal virtual SemanticsSortKey? _sortKey { get; set; } = default;
    internal virtual TextSelection? _textSelection { get; set; } = default;
    internal virtual bool? _isMultiline { get; set; } = default;
    internal virtual long? _scrollChildCount { get; set; } = default;
    internal virtual long? _scrollIndex { get; set; } = default;
    internal virtual double? _scrollPosition { get; set; } = default;
    internal virtual double? _scrollExtentMax { get; set; } = default;
    internal virtual double? _scrollExtentMin { get; set; } = default;
    internal virtual long? _platformViewId { get; set; } = default;
    internal virtual long? _maxValueLength { get; set; } = default;
    internal virtual long? _currentValueLength { get; set; } = default;
    internal virtual long _headingLevel { get; set; } = ((SemanticsConfiguration)_kEmptyConfig)._headingLevel;
    internal virtual DartUri? _linkUrl { get; set; } = ((SemanticsConfiguration)_kEmptyConfig)._linkUrl;
    internal virtual global::Doroti.Ui.SemanticsRole _role { get; set; } = ((SemanticsConfiguration)_kEmptyConfig).role;
    internal virtual HashSet<string>? _controlsNodes { get; set; } = ((SemanticsConfiguration)_kEmptyConfig).controlsNodes;
    internal virtual string? _minValue { get; set; } = default;
    internal virtual string? _maxValue { get; set; } = default;
    internal virtual global::Doroti.Ui.SemanticsValidationResult _validationResult { get; set; } = ((SemanticsConfiguration)_kEmptyConfig).validationResult;
    internal virtual SemanticsHitTestBehavior _hitTestBehavior { get; set; } = Dart_uiLibrary.SemanticsHitTestBehavior.defer;
    internal virtual global::Doroti.Ui.SemanticsInputType _inputType { get; set; } = ((SemanticsConfiguration)_kEmptyConfig).inputType;
    internal static SemanticsConfiguration _kEmptyConfig = new SemanticsConfiguration();
    internal static Int32List _kEmptyChildList = new Int32List(0L);
    internal static Int32List _kEmptyCustomSemanticsActionsList = new Int32List(0L);
    internal static Matrix4 _kIdentityTransform = Matrix4.identity();

    public SemanticsNode(Key? key = null, Action? showOnScreen = null)
    {
        this.key = key;
        this._id = _generateNewId();
        this._showOnScreen = showOnScreen;
    }

    public static SemanticsNode CreateRoot(Key? key = null, Action? showOnScreen = null, SemanticsOwner owner = default!)
    {
        var __instance = new SemanticsNode(key, showOnScreen);
        __instance.key = key;
        __instance._id = 0L;
        __instance._showOnScreen = showOnScreen;
        __instance.attach(owner);
        return __instance;
    }

    internal static long _generateNewId()
    {
        _lastIdentifier = (((_lastIdentifier + 1L)) % _maxFrameworkAccessibilityIdentifier);
        return _lastIdentifier;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long id => this._id;
    public virtual Matrix4? transform
    {
        get => this._transform;
        set
        {
            var __value = value;
            if (!MatrixUtils.matrixEquals(this._transform, __value))
            {
                _transform = (((__value is null) || MatrixUtils.isIdentity(__value)) ? null : __value);
                _markDirty();
            }
        }
    }
    internal virtual Matrix4? _traversalTransform
    {
        get
        {
            return (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb ? this.transform : ((this._traversalChildTransform ?? this.transform)));
            return default!;
        }
    }
    public virtual global::Doroti.Ui.Rect rect
    {
        get => this._rect;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.RequireValue(__value).isFinite);
            if ((!object.Equals(this._rect, DartRuntimePrimitives.RequireValue(__value))))
            {
                _rect = DartRuntimePrimitives.RequireValue(__value);
                _markDirty();
            }
        }
    }
    public virtual bool isInvisible => (!this.isMergedIntoParent && ((this.rect.isEmpty || ((this.transform?.isZero() ?? false)))));
    public virtual bool isMergedIntoParent
    {
        get => this._isMergedIntoParent;
        set
        {
            var __value = value;
            if ((this._isMergedIntoParent == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _isMergedIntoParent = DartRuntimePrimitives.RequireValue(__value);
            this.parent?._markDirty();
        }
    }
    public virtual bool areUserActionsBlocked
    {
        get => this._areUserActionsBlocked;
        set
        {
            var __value = value;
            if ((this._areUserActionsBlocked == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _areUserActionsBlocked = DartRuntimePrimitives.RequireValue(__value);
            _markDirty();
        }
    }
    public virtual bool isPartOfNodeMerging => (this.mergeAllDescendantsIntoThisNode || this.isMergedIntoParent);
    public virtual bool mergeAllDescendantsIntoThisNode => this._mergeAllDescendantsIntoThisNode;
    internal virtual void _replaceChildren(List<SemanticsNode> newChildren)
    {
        DartRuntimePrimitives.Assert(() => !newChildren.any(((child) => (object.Equals(child, this)))));
        DartRuntimePrimitives.Assert(() =>
            {
                var seenChildren = new HashSet<SemanticsNode>();
                foreach (var childLocal in newChildren)
                {
                    DartRuntimePrimitives.Assert(() => seenChildren.Add(childLocal));
                }
                return true;
            });
        if ((this._children is not null))
        {
            foreach (SemanticsNode childAlternate in this._children!)
            {
                childAlternate._dead = true;
            }
        }
        foreach (var childNested in newChildren)
        {
            childNested._dead = false;
        }
        var sawChange = false;
        if ((this._children is not null))
        {
            foreach (SemanticsNode childCurrent in this._children!)
            {
                if (((SemanticsNode)childCurrent)._dead)
                {
                    if ((object.Equals(((SemanticsNode)childCurrent).parent, this)))
                    {
                        _dropChild(childCurrent);
                    }
                    sawChange = true;
                }
            }
        }
        foreach (var childNext in newChildren)
        {
            if ((!object.Equals(((SemanticsNode)childNext).parent, this)))
            {
                if ((((SemanticsNode)childNext).parent is not null))
                {
                    ((SemanticsNode)childNext).parent?._dropChild(childNext);
                }
                DartRuntimePrimitives.Assert(() => !((SemanticsNode)childNext).attached);
                _adoptChild(childNext);
                sawChange = true;
            }
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if (DartRuntimePrimitives.Identical(newChildren, this._children))
                {
                    var mutationErrors = new List<DiagnosticsNode>();
                    if ((checked((long)(newChildren.Count)) != checked((long)(this._debugPreviousSnapshot.Count))))
                    {
                        mutationErrors.Add(new ErrorDescription($"The list's length has changed from {checked((long)(this._debugPreviousSnapshot.Count))} " + $"to {checked((long)(newChildren.Count))}."));
                    }
                    else
                    {
                        for (var i = 0L; (i < checked((long)(newChildren.Count))); i++)
                        {
                            if (!DartRuntimePrimitives.Identical(newChildren[(int)(i)], this._debugPreviousSnapshot[(int)(i)]))
                            {
                                if ((checked((long)(mutationErrors.Count)) != 0))
                                {
                                    mutationErrors.Add(new ErrorSpacer());
                                }
                                mutationErrors.Add(new ErrorDescription($"Child node at position {i} was replaced:"));
                                mutationErrors.Add(((Diagnosticable)this._debugPreviousSnapshot[(int)(i)]).toDiagnosticsNode(name: "Previous child", style: DiagnosticsTreeStyle.singleLine));
                                mutationErrors.Add(((Diagnosticable)newChildren[(int)(i)]).toDiagnosticsNode(name: "New child", style: DiagnosticsTreeStyle.singleLine));
                            }
                        }
                    }
                    if ((checked((long)(mutationErrors.Count)) != 0))
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Failed to replace child semantics nodes because the list of `SemanticsNode`s was mutated."), new ErrorHint("Instead of mutating the existing list, create a new list containing the desired `SemanticsNode`s."), new ErrorDescription("Error details:") });
                    }
                }
                _debugPreviousSnapshot = new List<SemanticsNode>(newChildren);
                var ancestor = this;
                while ((((SemanticsNode)ancestor).parent is SemanticsNode))
                {
                    ancestor = ((SemanticsNode)ancestor).parent!;
                }
                DartRuntimePrimitives.Assert(() => !newChildren.any(((child) => (object.Equals(child, ancestor)))));
                return true;
            });
        if ((!sawChange && (this._children is not null)))
        {
            DartRuntimePrimitives.Assert(() => (checked((long)(newChildren.Count)) == checked((long)(this._children!.Count))));
            for (var iLocal = 0L; (iLocal < checked((long)(this._children!.Count))); iLocal++)
            {
                if ((this._children![(int)(iLocal)].id != newChildren[(int)(iLocal)].id))
                {
                    sawChange = true;
                    break;
                }
            }
        }
        _children = newChildren;
        if (sawChange)
        {
            _markDirty();
        }
    }

    public virtual bool hasChildren => ((((long?)(this._children?.Count)) is { } __count116564 ? __count116564 != 0 : (bool?)null) ?? false);
    public virtual long childrenCount => (this.hasChildren ? checked((long)(this._children!.Count)) : 0L);
    public virtual long childrenCountInTraversalOrder => checked((long)(_childrenInTraversalOrder().Count));
    public virtual void visitChildren(Func<SemanticsNode, bool> visitor)
    {
        if ((this._children is not null))
        {
            foreach (SemanticsNode child in this._children!)
            {
                if (!visitor(child))
                {
                    return;
                }
            }
        }
    }

    internal virtual bool _visitDescendants(Func<SemanticsNode, bool> visitor)
    {
        if ((this._children is not null))
        {
            foreach (SemanticsNode child in this._children!)
            {
                if ((!visitor(child) || !child._visitDescendants((Func<SemanticsNode, bool>)visitor)))
                {
                    return false;
                }
            }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SemanticsOwner? owner => this._owner;
    public virtual bool attached => (this._owner is not null);
    public virtual SemanticsNode? parent => this._parent;
    public virtual SemanticsNode? traversalParent
    {
        get => (this._traversalParent ?? this.parent);
        set
        {
            var __value = value;
            if ((object.Equals(this._traversalParent, __value)))
            {
                return;
            }
            _traversalParent = __value;
            _markDirty();
        }
    }
    public virtual long depth => this._depth;
    internal virtual void _redepthChild(SemanticsNode child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((SemanticsNode)child).owner, this.owner)));
        if ((((SemanticsNode)child)._depth <= this._depth))
        {
            child._depth = (this._depth + 1L);
            child._redepthChildren();
        }
    }

    internal virtual void _redepthChildren()
    {
        this._children?.forEach(this._redepthChild);
    }

    internal virtual void _updateChildMergeFlagRecursively(SemanticsNode child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((SemanticsNode)child).owner, this.owner)));
        bool childShouldMergeToParent = this.isPartOfNodeMerging;
        if ((childShouldMergeToParent == ((SemanticsNode)child).isMergedIntoParent))
        {
            return;
        }
        child.isMergedIntoParent = childShouldMergeToParent;
        if (((SemanticsNode)child).mergeAllDescendantsIntoThisNode)
        {
        }
        else
        {
            child._updateChildrenMergeFlags();
        }
    }

    internal virtual void _updateChildrenMergeFlags()
    {
        this._children?.forEach(this._updateChildMergeFlagRecursively);
    }

    internal virtual void _adoptChild(SemanticsNode child)
    {
        DartRuntimePrimitives.Assert(() => (((SemanticsNode)child)._parent is null));
        DartRuntimePrimitives.Assert(() =>
            {
                var node = this;
                while ((((SemanticsNode)node).parent is not null))
                {
                    node = ((SemanticsNode)node).parent!;
                }
                DartRuntimePrimitives.Assert(() => (!object.Equals(node, child)));
                return true;
            });
        child._parent = this;
        if (this.attached)
        {
            child.attach(this._owner!);
        }
        _redepthChild(child);
        _updateChildMergeFlagRecursively(child);
    }

    internal virtual void _dropChild(SemanticsNode child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((SemanticsNode)child)._parent, this)));
        DartRuntimePrimitives.Assert(() => (((SemanticsNode)child).attached == this.attached));
        child._parent = null;
        if (this.attached)
        {
            child.detach();
        }
    }

    public virtual void attach(SemanticsOwner owner)
    {
        DartRuntimePrimitives.Assert(() => (this._owner is null));
        _owner = owner;
        while (((SemanticsOwner)owner)._nodes.ContainsKey(this.id))
        {
            _id = _generateNewId();
        }
        ((SemanticsOwner)owner)._nodes[this.id] = this;
        ((SemanticsOwner)owner)._detachedNodes.Remove(this);
        if (this._dirty)
        {
            _dirty = false;
            _markDirty();
        }
        if ((this._children is not null))
        {
            foreach (SemanticsNode child in this._children!)
            {
                child.attach(owner);
            }
        }
    }

    public virtual void detach()
    {
        DartRuntimePrimitives.Assert(() => (this._owner is not null));
        DartRuntimePrimitives.Assert(() => this.owner!._nodes.ContainsKey(this.id));
        DartRuntimePrimitives.Assert(() => !this.owner!._detachedNodes.Contains(this));
        this.owner!._nodes.remove(this.id);
        this.owner!._detachedNodes.Add(this);
        if (this._traversalChildIdentifier is object identifier)
        {
            this.owner!._traversalParentNodes.GetValueOrDefault(identifier)?._markDirty();
        }
        this.owner!._traversalParentNodes.removeWhere(((key, node) => (object.Equals(node, this))));
        foreach (HashSet<SemanticsNode> childSet in this.owner!._traversalChildNodes.Values)
        {
            childSet.removeWhere(((node) => (object.Equals(node, this))));
        }
        this.owner!._traversalChildNodes.removeWhere(((key, value) => (checked((long)(value.Count)) == 0)));
        _owner = null;
        DartRuntimePrimitives.Assert(() => ((this.parent is null) || (this.attached == this.parent!.attached)));
        if ((this._children is not null))
        {
            foreach (SemanticsNode child in this._children!)
            {
                if ((object.Equals(((SemanticsNode)child).parent, this)))
                {
                    child.detach();
                }
            }
        }
        _markDirty();
    }

    internal virtual void _markDirty()
    {
        if (this._dirty)
        {
            return;
        }
        _dirty = true;
        if (this.attached)
        {
            DartRuntimePrimitives.Assert(() => !this.owner!._detachedNodes.Contains(this));
            this.owner!._dirtyNodes.Add(this);
        }
    }

    public virtual bool? debugIsDirty
    {
        get
        {
            bool? isDirty = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    isDirty = this._dirty;
                    return true;
                });
            return isDirty;
            return default!;
        }
    }
    internal virtual bool _isDifferentFromCurrentSemanticAnnotation(SemanticsConfiguration config)
    {
        return ((((((((((((((((((((((((((((((!object.Equals(this._attributedLabel, ((SemanticsConfiguration)config).attributedLabel)) || (!object.Equals(this._attributedHint, ((SemanticsConfiguration)config).attributedHint))) || (!object.Equals(this._attributedValue, ((SemanticsConfiguration)config).attributedValue))) || (!object.Equals(this._attributedIncreasedValue, ((SemanticsConfiguration)config).attributedIncreasedValue))) || (!object.Equals(this._attributedDecreasedValue, ((SemanticsConfiguration)config).attributedDecreasedValue))) || (this._tooltip != ((SemanticsConfiguration)config).tooltip)) || (!object.Equals(this._flags, ((SemanticsConfiguration)config)._flags))) || (!object.Equals(this._textDirection, ((SemanticsConfiguration)config).textDirection))) || (!object.Equals(this._sortKey, ((SemanticsConfiguration)config)._sortKey))) || (!object.Equals(this._textSelection, ((SemanticsConfiguration)config)._textSelection))) || (this._scrollPosition != ((SemanticsConfiguration)config)._scrollPosition)) || (this._scrollExtentMax != ((SemanticsConfiguration)config)._scrollExtentMax)) || (this._scrollExtentMin != ((SemanticsConfiguration)config)._scrollExtentMin)) || (this._actionsAsBits != ((SemanticsConfiguration)config)._actionsAsBits)) || (this.indexInParent != ((SemanticsConfiguration)config).indexInParent)) || (this.platformViewId != ((SemanticsConfiguration)config).platformViewId)) || (this._maxValueLength != ((SemanticsConfiguration)config)._maxValueLength)) || (this._currentValueLength != ((SemanticsConfiguration)config)._currentValueLength)) || (this._mergeAllDescendantsIntoThisNode != ((SemanticsConfiguration)config).isMergingSemanticsOfDescendants)) || (this._areUserActionsBlocked != ((SemanticsConfiguration)config).isBlockingUserActions)) || (this._headingLevel != ((SemanticsConfiguration)config)._headingLevel)) || (!object.Equals(this._linkUrl, ((SemanticsConfiguration)config)._linkUrl))) || (!object.Equals(this._role, ((SemanticsConfiguration)config).role))) || (!object.Equals(this._validationResult, ((SemanticsConfiguration)config).validationResult))) || (!object.Equals(this._hitTestBehavior, ((SemanticsConfiguration)config).hitTestBehavior))) || (!object.Equals(this._traversalChildIdentifier, ((SemanticsConfiguration)config)._traversalChildIdentifier))) || (!object.Equals(this._traversalParentIdentifier, ((SemanticsConfiguration)config)._traversalParentIdentifier))) || (this._minValue != ((SemanticsConfiguration)config)._minValue)) || (this._maxValue != ((SemanticsConfiguration)config)._maxValue)) || !global::Doroti.Framework.Foundation.CollectionsLibrary.mapEquals<CustomSemanticsAction, Action>(this._customSemanticsActions, ((SemanticsConfiguration)config)._customSemanticsActions));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long _effectiveActionsAsBits => (this._areUserActionsBlocked ? (this._actionsAsBits & SemanticsLibrary._kUnblockedUserActions) : this._actionsAsBits);
    public virtual bool isTagged(SemanticsTag tag) => ((this.tags is not null) && this.tags!.Contains(tag));
    public virtual global::Doroti.Ui.SemanticsFlags flagsCollection => this._flags;
    internal virtual long _flagsBitMask => SemanticsLibrary._toBitMask(this.flagsCollection);
    public virtual bool hasFlag(SemanticsFlag flag) => (((this._flagsBitMask & FoundationRuntimePorts.EnumIndex(flag))) != 0L);
    public virtual string identifier => this._identifier;
    public virtual object? traversalParentIdentifier => this._traversalParentIdentifier;
    public virtual object? traversalChildIdentifier => this._traversalChildIdentifier;
    internal virtual bool _isTraversalParent => (this._traversalParentIdentifier is not null);
    internal virtual bool _isTraversalChild => (this._traversalChildIdentifier is not null);
    public virtual string label => ((AttributedString)this._attributedLabel).@string;
    public virtual AttributedString attributedLabel => this._attributedLabel;
    public virtual string value => ((AttributedString)this._attributedValue).@string;
    public virtual AttributedString attributedValue => this._attributedValue;
    public virtual string increasedValue => ((AttributedString)this._attributedIncreasedValue).@string;
    public virtual AttributedString attributedIncreasedValue => this._attributedIncreasedValue;
    public virtual string decreasedValue => ((AttributedString)this._attributedDecreasedValue).@string;
    public virtual AttributedString attributedDecreasedValue => this._attributedDecreasedValue;
    public virtual string hint => ((AttributedString)this._attributedHint).@string;
    public virtual AttributedString attributedHint => this._attributedHint;
    public virtual string tooltip => this._tooltip;
    public virtual SemanticsHintOverrides? hintOverrides => this._hintOverrides;
    public virtual global::Doroti.Ui.TextDirection? textDirection => this._textDirection;
    public virtual SemanticsSortKey? sortKey => this._sortKey;
    public virtual TextSelection? textSelection => this._textSelection;
    public virtual bool? isMultiline => this._isMultiline;
    public virtual long? scrollChildCount => this._scrollChildCount;
    public virtual long? scrollIndex => this._scrollIndex;
    public virtual double? scrollPosition => this._scrollPosition;
    public virtual double? scrollExtentMax => this._scrollExtentMax;
    public virtual double? scrollExtentMin => this._scrollExtentMin;
    public virtual long? platformViewId => this._platformViewId;
    public virtual long? maxValueLength => this._maxValueLength;
    public virtual long? currentValueLength => this._currentValueLength;
    public virtual long headingLevel => this._headingLevel;
    public virtual DartUri? linkUrl => this._linkUrl;
    public virtual global::Doroti.Ui.SemanticsRole role => this._role;
    public virtual HashSet<string>? controlsNodes => this._controlsNodes;
    public virtual string? minValue => this._minValue;
    public virtual string? maxValue => this._maxValue;
    public virtual global::Doroti.Ui.SemanticsValidationResult validationResult => this._validationResult;
    public virtual global::Doroti.Ui.SemanticsHitTestBehavior hitTestBehavior => this._hitTestBehavior;
    public virtual global::Doroti.Ui.SemanticsInputType inputType => this._inputType;
    internal virtual bool _canPerformAction(SemanticsAction action) => this._actions.ContainsKey(action);
    internal virtual bool _canPerformCustomAction(long actionId)
    {
        CustomSemanticsAction? customAction = CustomSemanticsAction.getAction(actionId);
        return ((customAction is not null) && this._customSemanticsActions.ContainsKey(customAction));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _canHandleAction(SemanticsAction action, object? args)
    {
        if ((object.Equals(action, SemanticsAction.customAction)))
        {
            return ((args is long) && _canPerformCustomAction(DartRuntimePrimitives.RequireValue(((long)args))));
        }
        return _canPerformAction(action);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void updateWith(SemanticsConfiguration? config, List<SemanticsNode>? childrenInInversePaintOrder = null)
    {
        config ??= _kEmptyConfig;
        if (_isDifferentFromCurrentSemanticAnnotation(config))
        {
            _markDirty();
        }
        DartRuntimePrimitives.Assert(() => (((((SemanticsConfiguration)config).platformViewId is null) || (childrenInInversePaintOrder is null)) || (checked((long)(childrenInInversePaintOrder.Count)) == 0)));
        var mergeAllDescendantsIntoThisNodeValueChanged = (this._mergeAllDescendantsIntoThisNode != ((SemanticsConfiguration)config).isMergingSemanticsOfDescendants);
        _identifier = ((SemanticsConfiguration)config).identifier;
        _traversalParentIdentifier = ((SemanticsConfiguration)config).traversalParentIdentifier;
        _traversalChildIdentifier = ((SemanticsConfiguration)config).traversalChildIdentifier;
        _attributedLabel = ((SemanticsConfiguration)config).attributedLabel;
        _attributedValue = ((SemanticsConfiguration)config).attributedValue;
        _attributedIncreasedValue = ((SemanticsConfiguration)config).attributedIncreasedValue;
        _attributedDecreasedValue = ((SemanticsConfiguration)config).attributedDecreasedValue;
        _attributedHint = ((SemanticsConfiguration)config).attributedHint;
        _tooltip = ((SemanticsConfiguration)config).tooltip;
        _hintOverrides = ((SemanticsConfiguration)config).hintOverrides;
        _flags = ((SemanticsConfiguration)config)._flags;
        _textDirection = ((SemanticsConfiguration)config).textDirection;
        _sortKey = ((SemanticsConfiguration)config).sortKey;
        _actions = new DartMap<global::Doroti.Ui.SemanticsAction, Action<object?>>(((SemanticsConfiguration)config)._actions);
        _customSemanticsActions = new DartMap<CustomSemanticsAction, Action>(((SemanticsConfiguration)config)._customSemanticsActions);
        _actionsAsBits = ((SemanticsConfiguration)config)._actionsAsBits;
        _textSelection = ((SemanticsConfiguration)config)._textSelection;
        _isMultiline = ((SemanticsConfiguration)config).isMultiline;
        _scrollPosition = ((SemanticsConfiguration)config)._scrollPosition;
        _scrollExtentMax = ((SemanticsConfiguration)config)._scrollExtentMax;
        _scrollExtentMin = ((SemanticsConfiguration)config)._scrollExtentMin;
        _mergeAllDescendantsIntoThisNode = ((SemanticsConfiguration)config).isMergingSemanticsOfDescendants;
        _scrollChildCount = ((SemanticsConfiguration)config).scrollChildCount;
        _scrollIndex = ((SemanticsConfiguration)config).scrollIndex;
        indexInParent = ((SemanticsConfiguration)config).indexInParent;
        _platformViewId = ((SemanticsConfiguration)config)._platformViewId;
        _maxValueLength = ((SemanticsConfiguration)config)._maxValueLength;
        _currentValueLength = ((SemanticsConfiguration)config)._currentValueLength;
        _areUserActionsBlocked = ((SemanticsConfiguration)config).isBlockingUserActions;
        _headingLevel = ((SemanticsConfiguration)config)._headingLevel;
        _linkUrl = ((SemanticsConfiguration)config)._linkUrl;
        _role = ((SemanticsConfiguration)config)._role;
        _controlsNodes = ((SemanticsConfiguration)config)._controlsNodes;
        _validationResult = ((SemanticsConfiguration)config)._validationResult;
        _hitTestBehavior = ((SemanticsConfiguration)config)._hitTestBehavior;
        _inputType = ((SemanticsConfiguration)config)._inputType;
        _locale = ((SemanticsConfiguration)config).locale;
        _minValue = ((SemanticsConfiguration)config).minValue;
        _maxValue = ((SemanticsConfiguration)config).maxValue;
        _replaceChildren((childrenInInversePaintOrder ?? new List<SemanticsNode>()));
        if (mergeAllDescendantsIntoThisNodeValueChanged)
        {
            _updateChildrenMergeFlags();
        }
        DartRuntimePrimitives.Assert(() => (!_canPerformAction(SemanticsAction.increase) || (((this.value == "")) == ((this.increasedValue == "")))));
        DartRuntimePrimitives.Assert(() => (!_canPerformAction(SemanticsAction.decrease) || (((this.value == "")) == ((this.decreasedValue == "")))));
    }

    public virtual SemanticsData getSemanticsData()
    {
        global::Doroti.Ui.SemanticsFlags flags = this._flags;
        long actionsLocal = this._actionsAsBits;
        string identifierLocal = this._identifier;
        object? traversalParentIdentifierLocal = this._traversalParentIdentifier;
        object? traversalChildIdentifierLocal = this._traversalChildIdentifier;
        AttributedString attributedLabelLocal = this._attributedLabel;
        AttributedString attributedValueLocal = this._attributedValue;
        AttributedString attributedIncreasedValueLocal = this._attributedIncreasedValue;
        AttributedString attributedDecreasedValueLocal = this._attributedDecreasedValue;
        AttributedString attributedHintLocal = this._attributedHint;
        string tooltipLocal = this._tooltip;
        global::Doroti.Ui.TextDirection? textDirectionLocal = this._textDirection;
        HashSet<SemanticsTag>? mergedTags = ((this.tags is null) ? null : new HashSet<SemanticsTag>(this.tags!));
        TextSelection? textSelectionLocal = this._textSelection;
        long? scrollChildCountLocal = this._scrollChildCount;
        long? scrollIndexLocal = this._scrollIndex;
        double? scrollPositionLocal = this._scrollPosition;
        double? scrollExtentMaxLocal = this._scrollExtentMax;
        double? scrollExtentMinLocal = this._scrollExtentMin;
        long? platformViewIdLocal = this._platformViewId;
        long? maxValueLengthLocal = this._maxValueLength;
        long? currentValueLengthLocal = this._currentValueLength;
        long headingLevelLocal = this._headingLevel;
        DartUri? linkUrlLocal = this._linkUrl;
        global::Doroti.Ui.SemanticsRole roleLocal = this._role;
        HashSet<string>? controlsNodesLocal = this._controlsNodes;
        global::Doroti.Ui.SemanticsValidationResult validationResultLocal = this._validationResult;
        global::Doroti.Ui.SemanticsHitTestBehavior hitTestBehaviorLocal = this._hitTestBehavior;
        global::Doroti.Ui.SemanticsInputType inputTypeLocal = this._inputType;
        global::Doroti.Ui.Locale? localeLocal = this._locale;
        var customSemanticsActionIdsLocal = new HashSet<long>();
        string? minValueLocal = this._minValue;
        string? maxValueLocal = this._maxValue;
        foreach (CustomSemanticsAction actionLocal in this._customSemanticsActions.Keys)
        {
            customSemanticsActionIdsLocal.Add(CustomSemanticsAction.getIdentifier(actionLocal));
        }
        if ((this.hintOverrides is not null))
        {
            if ((this.hintOverrides!.onTapHint is not null))
            {
                var actionAlternate = CustomSemanticsAction.CreateOverridingAction(hint: this.hintOverrides!.onTapHint!, action: SemanticsAction.tap);
                customSemanticsActionIdsLocal.Add(CustomSemanticsAction.getIdentifier(actionAlternate));
            }
            if ((this.hintOverrides!.onLongPressHint is not null))
            {
                var actionNested = CustomSemanticsAction.CreateOverridingAction(hint: this.hintOverrides!.onLongPressHint!, action: SemanticsAction.longPress);
                customSemanticsActionIdsLocal.Add(CustomSemanticsAction.getIdentifier(actionNested));
            }
        }
        if (this.mergeAllDescendantsIntoThisNode)
        {
            _visitDescendants(((Func<SemanticsNode, bool>)((node) =>
            {
                DartRuntimePrimitives.Assert(() => ((SemanticsNode)node).isMergedIntoParent);
                flags = flags.merge(((SemanticsNode)node)._flags);
                actionsLocal |= ((SemanticsNode)node)._effectiveActionsAsBits;
                textDirectionLocal ??= ((SemanticsNode)node)._textDirection;
                textSelectionLocal ??= ((SemanticsNode)node)._textSelection;
                scrollChildCountLocal ??= ((SemanticsNode)node)._scrollChildCount;
                scrollIndexLocal ??= ((SemanticsNode)node)._scrollIndex;
                scrollPositionLocal ??= ((SemanticsNode)node)._scrollPosition;
                scrollExtentMaxLocal ??= ((SemanticsNode)node)._scrollExtentMax;
                scrollExtentMinLocal ??= ((SemanticsNode)node)._scrollExtentMin;
                platformViewIdLocal ??= ((SemanticsNode)node)._platformViewId;
                maxValueLengthLocal ??= ((SemanticsNode)node)._maxValueLength;
                currentValueLengthLocal ??= ((SemanticsNode)node)._currentValueLength;
                linkUrlLocal ??= ((SemanticsNode)node)._linkUrl;
                headingLevelLocal = SemanticsLibrary._mergeHeadingLevels(sourceLevel: ((SemanticsNode)node)._headingLevel, targetLevel: headingLevelLocal);
                if ((identifierLocal == ""))
                {
                    identifierLocal = ((SemanticsNode)node)._identifier;
                }
                traversalParentIdentifierLocal ??= ((SemanticsNode)node).traversalParentIdentifier;
                traversalChildIdentifierLocal ??= ((SemanticsNode)node).traversalChildIdentifier;
                if ((((AttributedString)attributedValueLocal).@string == ""))
                {
                    attributedValueLocal = ((SemanticsNode)node)._attributedValue;
                }
                if ((((AttributedString)attributedIncreasedValueLocal).@string == ""))
                {
                    attributedIncreasedValueLocal = ((SemanticsNode)node)._attributedIncreasedValue;
                }
                if ((((AttributedString)attributedDecreasedValueLocal).@string == ""))
                {
                    attributedDecreasedValueLocal = ((SemanticsNode)node)._attributedDecreasedValue;
                }
                if ((object.Equals(roleLocal, SemanticsRole.none)))
                {
                    roleLocal = ((SemanticsNode)node)._role;
                }
                if ((object.Equals(inputTypeLocal, SemanticsInputType.none)))
                {
                    inputTypeLocal = ((SemanticsNode)node)._inputType;
                }
                if ((object.Equals(hitTestBehaviorLocal, Dart_uiLibrary.SemanticsHitTestBehavior.defer)))
                {
                    hitTestBehaviorLocal = ((SemanticsNode)node)._hitTestBehavior;
                }
                if ((tooltipLocal == ""))
                {
                    tooltipLocal = ((SemanticsNode)node)._tooltip;
                }
                if ((((SemanticsNode)node).tags is not null))
                {
                    mergedTags ??= new HashSet<SemanticsTag>();
                    mergedTags!.UnionWith(((SemanticsNode)node).tags!);
                }
                foreach (CustomSemanticsAction actionCurrent in ((SemanticsNode)node)._customSemanticsActions.Keys)
                {
                    customSemanticsActionIdsLocal.Add(CustomSemanticsAction.getIdentifier(actionCurrent));
                }
                if ((((SemanticsNode)node).hintOverrides is not null))
                {
                    if ((((SemanticsNode)node).hintOverrides!.onTapHint is not null))
                    {
                        var actionNext = CustomSemanticsAction.CreateOverridingAction(hint: ((SemanticsNode)node).hintOverrides!.onTapHint!, action: SemanticsAction.tap);
                        customSemanticsActionIdsLocal.Add(CustomSemanticsAction.getIdentifier(actionNext));
                    }
                    if ((((SemanticsNode)node).hintOverrides!.onLongPressHint is not null))
                    {
                        var actionCandidate = CustomSemanticsAction.CreateOverridingAction(hint: ((SemanticsNode)node).hintOverrides!.onLongPressHint!, action: SemanticsAction.longPress);
                        customSemanticsActionIdsLocal.Add(CustomSemanticsAction.getIdentifier(actionCandidate));
                    }
                }
                attributedLabelLocal = SemanticsLibrary._concatAttributedString(thisAttributedString: attributedLabelLocal, thisTextDirection: textDirectionLocal, otherAttributedString: ((SemanticsNode)node)._attributedLabel, otherTextDirection: ((SemanticsNode)node)._textDirection);
                attributedHintLocal = SemanticsLibrary._concatAttributedString(thisAttributedString: attributedHintLocal, thisTextDirection: textDirectionLocal, otherAttributedString: ((SemanticsNode)node)._attributedHint, otherTextDirection: ((SemanticsNode)node)._textDirection);
                if ((controlsNodesLocal is null))
                {
                    controlsNodesLocal = ((SemanticsNode)node)._controlsNodes;
                }
                else
                {
                    if ((((SemanticsNode)node)._controlsNodes is not null))
                    {
                        controlsNodesLocal = new HashSet<string>();
                    }
                }
                minValueLocal ??= ((SemanticsNode)node)._minValue;
                maxValueLocal ??= ((SemanticsNode)node)._maxValue;
                if ((object.Equals(validationResultLocal, SemanticsValidationResult.none)))
                {
                    validationResultLocal = ((SemanticsNode)node)._validationResult;
                }
                else
                {
                    if ((object.Equals(validationResultLocal, SemanticsValidationResult.valid)))
                    {
                        if (((!object.Equals(((SemanticsNode)node)._validationResult, SemanticsValidationResult.none)) && (!object.Equals(((SemanticsNode)node)._validationResult, SemanticsValidationResult.valid))))
                        {
                            validationResultLocal = ((SemanticsNode)node)._validationResult;
                        }
                    }
                }
                return true;
                return default;
            })));
        }
        return new SemanticsData(flagsCollection: flags, actions: (this._areUserActionsBlocked ? (actionsLocal & SemanticsLibrary._kUnblockedUserActions) : actionsLocal), identifier: identifierLocal, traversalParentIdentifier: traversalParentIdentifierLocal, traversalChildIdentifier: traversalChildIdentifierLocal, attributedLabel: attributedLabelLocal, attributedValue: attributedValueLocal, attributedIncreasedValue: attributedIncreasedValueLocal, attributedDecreasedValue: attributedDecreasedValueLocal, attributedHint: attributedHintLocal, tooltip: tooltipLocal, textDirection: textDirectionLocal, rect: this.rect, transform: this.transform, tags: mergedTags, textSelection: textSelectionLocal, scrollChildCount: scrollChildCountLocal, scrollIndex: scrollIndexLocal, scrollPosition: scrollPositionLocal, scrollExtentMax: scrollExtentMaxLocal, scrollExtentMin: scrollExtentMinLocal, platformViewId: platformViewIdLocal, maxValueLength: maxValueLengthLocal, currentValueLength: currentValueLengthLocal, customSemanticsActionIds: ((Func<List<long>>)(() =>
{
    var __cascade = customSemanticsActionIdsLocal.ToList();
    __cascade.sort();
    return __cascade;
}))(), headingLevel: headingLevelLocal, linkUrl: linkUrlLocal, role: roleLocal, controlsNodes: controlsNodesLocal, validationResult: validationResultLocal, hitTestBehavior: hitTestBehaviorLocal, inputType: inputTypeLocal, locale: localeLocal, minValue: minValueLocal, maxValue: maxValueLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static Matrix4 _computeTraversalTransform(SemanticsNode parent, SemanticsNode child)
    {
        var traversalTransform = Matrix4.identity();
        Matrix4? parentToCommonAncestorTransform = default!;
        var fromNode = child;
        var toNode = parent;
        while (!DartRuntimePrimitives.Identical(fromNode, toNode))
        {
            long fromDepth = ((SemanticsNode)fromNode).depth;
            long toDepth = ((SemanticsNode)toNode).depth;
            if ((fromDepth >= toDepth))
            {
                if (((SemanticsNode)fromNode).transform is Matrix4 transformLocal)
                {
                    traversalTransform.multiply(transformLocal);
                }
                fromNode = ((SemanticsNode)fromNode).parent!;
            }
            if ((fromDepth <= toDepth))
            {
                parentToCommonAncestorTransform ??= Matrix4.identity();
                if (((SemanticsNode)toNode).transform is Matrix4 transformAlternate)
                {
                    parentToCommonAncestorTransform.multiply(transformAlternate);
                }
                toNode = ((SemanticsNode)toNode).parent!;
            }
        }
        if ((parentToCommonAncestorTransform is not null))
        {
            if ((parentToCommonAncestorTransform.invert() != 0L))
            {
                traversalTransform.multiply(parentToCommonAncestorTransform);
            }
            else
            {
                traversalTransform.setZero();
            }
        }
        return traversalTransform;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Int32List _childrenIdInTraversalOrder()
    {
        List<SemanticsNode> sortedChildren = _childrenInTraversalOrder();
        var childrenInTraversalOrder = new Int32List(checked((long)(sortedChildren.Count)));
        for (var i = 0L; (i < checked((long)(sortedChildren.Count))); i += 1L)
        {
            childrenInTraversalOrder[i] = checked((int)(sortedChildren[(int)(i)].id));
        }
        return childrenInTraversalOrder;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<SemanticsNode> _childrenInHitTestOrder()
    {
        if ((this._children is null))
        {
            return new List<SemanticsNode>();
        }
        if ((global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb || this._isTraversalParent))
        {
            return this._children!;
        }
        bool shouldNotSkipInHitTest(SemanticsNode child)
        {
            if (((SemanticsNode)child)._isTraversalChild)
            {
                SemanticsNode? traversalParent = this.owner!._traversalParentNodes.GetValueOrDefault(DartRuntimePrimitives.RequireReference(child.getSemanticsData().traversalChildIdentifier));
                return (traversalParent is not null);
            }
            return true;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        return this._children!.where(shouldNotSkipInHitTest).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Int32List _childrenIdInHitTestOrder()
    {
        List<SemanticsNode> children = _childrenInHitTestOrder();
        return new Int32List(System.Linq.Enumerable.Reverse(children).map<SemanticsNode, long>(((node) => ((SemanticsNode)node).id)).ToList());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _addToUpdate(SemanticsUpdateBuilder builder, HashSet<long> customSemanticsActionIdsUpdate)
    {
        DartRuntimePrimitives.Assert(() => this._dirty);
        SemanticsData data = getSemanticsData();
        DartRuntimePrimitives.Assert(() =>
            {
                FlutterError? error = _DebugSemanticsRoleChecks__semantics._checkSemanticsData(this);
                if ((error is not null))
                {
                    throw error;
                }
                return true;
            });
        Int32List childrenInTraversalOrderLocal = default!;
        Int32List childrenInHitTestOrderLocal = default!;
        if ((!this.hasChildren || this.mergeAllDescendantsIntoThisNode))
        {
            if ((this._isTraversalParent && !global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb))
            {
                if (((this.owner is not null) && this.owner!._traversalChildNodes.ContainsKey(this.traversalParentIdentifier)))
                {
                    HashSet<SemanticsNode> traversalChildren = this.owner!._traversalChildNodes.GetValueOrDefault(DartRuntimePrimitives.RequireReference(this.traversalParentIdentifier))!;
                    var index = 0L;
                    childrenInTraversalOrderLocal = new Int32List(checked((long)(traversalChildren.Count)));
                    foreach (var node in traversalChildren)
                    {
                        if (((SemanticsNode)node).attached)
                        {
                            childrenInTraversalOrderLocal[index] = checked((int)(((SemanticsNode)node).id));
                            index += 1L;
                        }
                    }
                }
                else
                {
                    childrenInTraversalOrderLocal = _kEmptyChildList;
                }
                childrenInHitTestOrderLocal = _kEmptyChildList;
            }
            else
            {
                childrenInTraversalOrderLocal = _kEmptyChildList;
                childrenInHitTestOrderLocal = _kEmptyChildList;
            }
        }
        else
        {
            childrenInTraversalOrderLocal = _childrenIdInTraversalOrder();
            childrenInHitTestOrderLocal = _childrenIdInHitTestOrder();
        }
        Int32List? customSemanticsActionIdsLocal = default!;
        if (((((long?)(((SemanticsData)data).customSemanticsActionIds?.Count)) is { } __count156027 ? __count156027 != 0 : (bool?)null) ?? false))
        {
            customSemanticsActionIdsLocal = new Int32List(checked((long)(((SemanticsData)data).customSemanticsActionIds!.Count)));
            for (var i = 0L; (i < checked((long)(((SemanticsData)data).customSemanticsActionIds!.Count))); i++)
            {
                customSemanticsActionIdsLocal[i] = checked((int)(((SemanticsData)data).customSemanticsActionIds![(int)(i)]));
                customSemanticsActionIdsUpdate.Add(((SemanticsData)data).customSemanticsActionIds![(int)(i)]);
            }
        }
        var traversalParentId = -1L;
        if (((SemanticsData)data).traversalChildIdentifier is object identifierLocal)
        {
            if (this.owner!._traversalParentNodes.GetValueOrDefault(identifierLocal) is SemanticsNode parentNode)
            {
                traversalParentId = ((SemanticsNode)parentNode).id;
            }
        }
        object? childIdentifier = this.traversalChildIdentifier;
        if ((childIdentifier is not null))
        {
            traversalParent = this.owner!._traversalParentNodes.GetValueOrDefault(childIdentifier);
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb)
            {
                _traversalChildTransform = _computeTraversalTransform(parent: this.traversalParent!, child: this);
            }
        }
        builder.updateNode(id: this.id, flags: ((SemanticsData)data).flagsCollection, actions: ((SemanticsData)data).actions, rect: ((SemanticsData)data).rect, identifier: ((SemanticsData)data).identifier, label: ((SemanticsData)data).attributedLabel.@string, labelAttributes: ((SemanticsData)data).attributedLabel.attributes, value: ((SemanticsData)data).attributedValue.@string, valueAttributes: ((SemanticsData)data).attributedValue.attributes, increasedValue: ((SemanticsData)data).attributedIncreasedValue.@string, increasedValueAttributes: ((SemanticsData)data).attributedIncreasedValue.attributes, decreasedValue: ((SemanticsData)data).attributedDecreasedValue.@string, decreasedValueAttributes: ((SemanticsData)data).attributedDecreasedValue.attributes, hint: ((SemanticsData)data).attributedHint.@string, hintAttributes: ((SemanticsData)data).attributedHint.attributes, tooltip: ((SemanticsData)data).tooltip, textDirection: ((SemanticsData)data).textDirection, textSelectionBase: ((((SemanticsData)data).textSelection is not null) ? ((SemanticsData)data).textSelection!.baseOffset : -1L), textSelectionExtent: ((((SemanticsData)data).textSelection is not null) ? ((SemanticsData)data).textSelection!.extentOffset : -1L), platformViewId: (((SemanticsData)data).platformViewId ?? -1L), maxValueLength: (((SemanticsData)data).maxValueLength ?? -1L), currentValueLength: (((SemanticsData)data).currentValueLength ?? -1L), scrollChildren: (((SemanticsData)data).scrollChildCount ?? 0L), scrollIndex: (((SemanticsData)data).scrollIndex ?? 0L), scrollPosition: (((SemanticsData)data).scrollPosition ?? double.NaN), scrollExtentMax: (((SemanticsData)data).scrollExtentMax ?? double.NaN), scrollExtentMin: (((SemanticsData)data).scrollExtentMin ?? double.NaN), transform: ((this._traversalTransform ?? _kIdentityTransform)).storage, traversalParent: traversalParentId, hitTestTransform: ((((SemanticsData)data).transform ?? _kIdentityTransform)).storage, childrenInTraversalOrder: childrenInTraversalOrderLocal, childrenInHitTestOrder: childrenInHitTestOrderLocal, additionalActions: (customSemanticsActionIdsLocal ?? _kEmptyCustomSemanticsActionsList), headingLevel: ((SemanticsData)data).headingLevel, linkUrl: (((SemanticsData)data).linkUrl?.ToString() ?? ""), role: ((SemanticsData)data).role, controlsNodes: ((SemanticsData)data).controlsNodes?.ToList(), validationResult: ((SemanticsData)data).validationResult, hitTestBehavior: ((SemanticsData)data).hitTestBehavior, inputType: ((SemanticsData)data).inputType, locale: ((SemanticsData)data).locale, minValue: (((SemanticsData)data).minValue ?? ""), maxValue: (((SemanticsData)data).maxValue ?? ""));
        _dirty = false;
    }

    internal virtual List<SemanticsNode>? _updateChildrenInTraversalOrder()
    {
        if (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb)
        {
            return this._children;
        }
        var updatedChildren = new List<SemanticsNode>();
        foreach (SemanticsNode child in this._children!)
        {
            if ((((SemanticsNode)child)._isTraversalChild && !this._isTraversalParent))
            {
                SemanticsNode? traversalParent = this.owner!._traversalParentNodes.GetValueOrDefault(DartRuntimePrimitives.RequireReference(child.getSemanticsData().traversalChildIdentifier));
                long? traversalParentId = traversalParent?.id;
                while ((traversalParent is not null))
                {
                    if ((object.Equals(traversalParent, child)))
                    {
                        throw new FlutterError($"The traversalParent__160618 {traversalParentId} cannot be the child of the traversalChild {((SemanticsNode)child).id} in hit-test order");
                    }
                    traversalParent = ((SemanticsNode)traversalParent).parent;
                }
                continue;
            }
            updatedChildren.Add(child);
        }
        if (this._isTraversalParent)
        {
            HashSet<SemanticsNode>? traversalChildren = this.owner?._traversalChildNodes.GetValueOrDefault(this.traversalParentIdentifier!);
            if ((traversalChildren is not null))
            {
                var currentNode = this;
                while ((((SemanticsNode)currentNode).parent is not null))
                {
                    currentNode = ((SemanticsNode)currentNode).parent!;
                    if (traversalChildren.Contains(currentNode))
                    {
                        throw new FlutterError($"The traversalParent {this.id} cannot be the child of the traversalChild {((SemanticsNode)currentNode).id} in hit-test order");
                    }
                }
                foreach (SemanticsNode node in traversalChildren)
                {
                    if (((SemanticsNode)node).attached)
                    {
                        updatedChildren.Add(node);
                    }
                }
            }
        }
        return updatedChildren;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<SemanticsNode> _childrenInTraversalOrder()
    {
        List<SemanticsNode>? updatedChildren = _updateChildrenInTraversalOrder();
        global::Doroti.Ui.TextDirection? inheritedTextDirection = this.textDirection;
        SemanticsNode? ancestor = this.parent;
        while (((inheritedTextDirection is null) && (ancestor is not null)))
        {
            inheritedTextDirection = ((SemanticsNode)ancestor).textDirection;
            ancestor = ((SemanticsNode)ancestor).parent;
        }
        List<SemanticsNode>? childrenInDefaultOrder = default!;
        if ((inheritedTextDirection is not null))
        {
            TextDirection inheritedTextDirection__162729__value163025 = DartRuntimePrimitives.RequireValue(inheritedTextDirection);
            childrenInDefaultOrder = SemanticsLibrary._childrenInDefaultOrder(updatedChildren!, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(inheritedTextDirection__162729__value163025)));
        }
        else
        {
            childrenInDefaultOrder = updatedChildren;
        }
        var everythingSorted = new List<_TraversalSortNode__semantics>();
        var sortNodes = new List<_TraversalSortNode__semantics>();
        SemanticsSortKey? lastSortKey = default!;
        for (var positionLocal = 0L; (positionLocal < checked((long)(childrenInDefaultOrder!.Count))); positionLocal += 1L)
        {
            SemanticsNode child = childrenInDefaultOrder[(int)(positionLocal)];
            SemanticsSortKey? sortKeyLocal = ((SemanticsNode)child).sortKey;
            lastSortKey = ((positionLocal > 0L) ? childrenInDefaultOrder[(int)((positionLocal - 1L))].sortKey : null);
            bool isCompatibleWithPreviousSortKey = ((positionLocal == 0L) || ((object.Equals(DartRuntimePrimitives.RuntimeType(sortKeyLocal), DartRuntimePrimitives.RuntimeType(lastSortKey))) && (((sortKeyLocal is null) || (((SemanticsSortKey)sortKeyLocal).name == lastSortKey!.ToString())))));
            if ((!isCompatibleWithPreviousSortKey && (checked((long)(sortNodes.Count)) != 0)))
            {
                if ((lastSortKey is not null))
                {
                    sortNodes.sort();
                }
                everythingSorted.AddRange(sortNodes);
                sortNodes.Clear();
            }
            sortNodes.Add(new _TraversalSortNode__semantics(node: child, sortKey: sortKeyLocal, position: positionLocal));
        }
        if ((lastSortKey is not null))
        {
            sortNodes.sort();
        }
        everythingSorted.AddRange(sortNodes);
        return everythingSorted.map<_TraversalSortNode__semantics, SemanticsNode>(((sortNode) => ((_TraversalSortNode__semantics)sortNode).node)).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void sendEvent(SemanticsEvent @event)
    {
        if (!this.attached)
        {
            return;
        }
        _ = SystemChannels.accessibility.send(@event.toMap(nodeId: this.id)).then(((_) =>
        {
        }), onError: ((error, stack) =>
        {
            FlutterError.reportError(new FlutterErrorDetails(exception: error, stack: stack, library: "semantics library", context: new ErrorDescription("while sending accessibility event"), informationCollector: (() => new List<DiagnosticsNode> { new DiagnosticsProperty<SemanticsEvent>("event", @event), new DiagnosticsProperty<SemanticsNode>("node", this) })));
        }));
    }

    internal virtual bool _debugIsActionBlocked(SemanticsAction action)
    {
        var result = false;
        DartRuntimePrimitives.Assert(() =>
            {
                result = ((this._effectiveActionsAsBits & (long)action) == 0L);
                return true;
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string toStringShort() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SemanticsNode"))}#{this.id}";
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        var hideOwner = true;
        if (this._dirty)
        {
            bool inDirtyNodes = ((this.owner is not null) && this.owner!._dirtyNodes.Contains(this));
            properties.add(new FlagProperty("inDirtyNodes", value: inDirtyNodes, ifTrue: "dirty", ifFalse: "STALE"));
            hideOwner = inDirtyNodes;
        }
        properties.add(new DiagnosticsProperty<SemanticsOwner>("owner", this.owner, level: (hideOwner ? DiagnosticLevel.hidden : DiagnosticLevel.info)));
        properties.add(new FlagProperty("isMergedIntoParent", value: this.isMergedIntoParent, ifTrue: "merged up ⬆️"));
        properties.add(new FlagProperty("mergeAllDescendantsIntoThisNode", value: this.mergeAllDescendantsIntoThisNode, ifTrue: "merge boundary ⛔️"));
        if ((this._locale is not null))
        {
            properties.add(new StringProperty("locale", this._locale.ToString()));
        }
        global::Doroti.Ui.Offset? offset = ((this.transform is not null) ? MatrixUtils.getAsTranslation(this.transform!) : null);
        if ((offset is not null))
        {
            Offset offset__167351__value167437 = DartRuntimePrimitives.RequireValue(offset);
            properties.add(new DiagnosticsProperty<global::Doroti.Ui.Rect>("rect", this.rect.shift(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(offset__167351__value167437))), showName: false));
        }
        else
        {
            double? scale = ((this.transform is not null) ? MatrixUtils.getAsScale(this.transform!) : null);
            string? descriptionLocal = default!;
            if ((scale is not null))
            {
                double scale__167582__value167690 = DartRuntimePrimitives.RequireValue(scale);
                descriptionLocal = $"{this.rect} scaled by {DartRuntimePrimitives.RequireValue(scale__167582__value167690).toStringAsFixed(1L)}x";
            }
            else
            {
                if (((this.transform is not null) && !MatrixUtils.isIdentity(this.transform!)))
                {
                    string matrix = string.Join("; ", this.transform.ToString().split("\n").take(4L).map<string, string>(((line) => line.substring(4L))));
                    descriptionLocal = $"{this.rect} with transform [{matrix}]";
                }
            }
            properties.add(new DiagnosticsProperty<global::Doroti.Ui.Rect>("rect", this.rect, description: descriptionLocal, showName: false));
        }
        properties.add(new IterableProperty<string>("tags", this.tags?.map<SemanticsTag, string>(((tag) => ((SemanticsTag)tag).name)), defaultValue: null));
        List<string> actions = ((Func<List<string>>)(() =>
{
    var __cascade = this._actions.Keys.map<SemanticsAction, string>(((action) => $"{action.ToString()}{(_debugIsActionBlocked(action) ? "🚫️" : "")}")).ToList();
    __cascade.sort();
    return __cascade;
}))();
        List<string?> customSemanticsActions = this._customSemanticsActions.Keys.map<CustomSemanticsAction, string?>(((action) => ((CustomSemanticsAction)action).label)).ToList();
        properties.add(new IterableProperty<string>("actions", actions, ifEmpty: null));
        properties.add(new IterableProperty<string?>("customActions", customSemanticsActions, ifEmpty: null));
        properties.add(new IterableProperty<string>("flags", this.flagsCollection.toStrings(), ifEmpty: null));
        properties.add(new FlagProperty("isInvisible", value: this.isInvisible, ifTrue: "invisible"));
        properties.add(new FlagProperty("isHidden", value: this.flagsCollection.isHidden, ifTrue: "HIDDEN"));
        properties.add(new StringProperty("identifier", this._identifier, defaultValue: ""));
        properties.add(new DiagnosticsProperty<object>("traversalParentIdentifier", this.traversalParentIdentifier, defaultValue: null));
        properties.add(new DiagnosticsProperty<object>("traversalChildIdentifier", this.traversalChildIdentifier, defaultValue: null));
        properties.add(new AttributedStringProperty("label", this._attributedLabel));
        properties.add(new AttributedStringProperty("value", this._attributedValue));
        properties.add(new AttributedStringProperty("increasedValue", this._attributedIncreasedValue));
        properties.add(new AttributedStringProperty("decreasedValue", this._attributedDecreasedValue));
        properties.add(new AttributedStringProperty("hint", this._attributedHint));
        properties.add(new StringProperty("tooltip", this._tooltip, defaultValue: ""));
        properties.add(new EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this._textDirection, defaultValue: null));
        if ((!object.Equals(this._role, SemanticsRole.none)))
        {
            properties.add(new EnumProperty<global::Doroti.Ui.SemanticsRole>("role", this._role));
        }
        properties.add(new DiagnosticsProperty<SemanticsSortKey>("sortKey", this.sortKey, defaultValue: null));
        if ((this._textSelection?.isValid ?? false))
        {
            properties.add(new MessageProperty("text selection", $"[{this._textSelection!.start}, {this._textSelection!.end}]"));
        }
        properties.add(new IntProperty("platformViewId", this.platformViewId, defaultValue: null));
        properties.add(new IntProperty("maxValueLength", this.maxValueLength, defaultValue: null));
        properties.add(new IntProperty("currentValueLength", this.currentValueLength, defaultValue: null));
        properties.add(new IntProperty("scrollChildren", this.scrollChildCount, defaultValue: null));
        properties.add(new IntProperty("scrollIndex", this.scrollIndex, defaultValue: null));
        properties.add(new DoubleProperty("scrollExtentMin", this.scrollExtentMin, defaultValue: null));
        properties.add(new DoubleProperty("scrollPosition", this.scrollPosition, defaultValue: null));
        properties.add(new DoubleProperty("scrollExtentMax", this.scrollExtentMax, defaultValue: null));
        properties.add(new IntProperty("indexInParent", this.indexInParent, defaultValue: null));
        properties.add(new IntProperty("headingLevel", this._headingLevel, defaultValue: 0L));
        if ((!object.Equals(this._inputType, SemanticsInputType.none)))
        {
            properties.add(new EnumProperty<global::Doroti.Ui.SemanticsInputType>("inputType", this._inputType));
        }
        if ((!object.Equals(this.validationResult, SemanticsValidationResult.none)))
        {
            properties.add(new EnumProperty<global::Doroti.Ui.SemanticsValidationResult>("validationResult", this.validationResult, defaultValue: SemanticsValidationResult.none));
        }
        properties.add(new StringProperty("minValue", this._minValue, defaultValue: null));
        properties.add(new StringProperty("maxValue", this._maxValue, defaultValue: null));
    }

    public virtual string toStringDeep(string prefixLineOne = "", string? prefixOtherLines = null, DiagnosticLevel minLevel = DiagnosticLevel.debug, DebugSemanticsDumpOrder childOrder = DebugSemanticsDumpOrder.traversalOrder, long wrapWidth = 65)
    {
        return toDiagnosticsNode(childOrder: childOrder).toStringDeep(prefixLineOne: prefixLineOne, prefixOtherLines: prefixOtherLines, minLevel: minLevel, wrapWidth: wrapWidth);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = DiagnosticsTreeStyle.sparse, DebugSemanticsDumpOrder childOrder = DebugSemanticsDumpOrder.traversalOrder)
    {
        return new _SemanticsDiagnosticableNode__semantics(name: name, value: this, style: style, childOrder: childOrder);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<DiagnosticsNode> debugDescribeChildren(DebugSemanticsDumpOrder childOrder = DebugSemanticsDumpOrder.traversalOrder)
    {
        return debugListChildrenInOrder(childOrder).map<SemanticsNode, DiagnosticsNode>(((node) => node.toDiagnosticsNode(childOrder: childOrder))).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<SemanticsNode> debugListChildrenInOrder(DebugSemanticsDumpOrder childOrder)
    {
        if ((this._children is null))
        {
            return new List<SemanticsNode>();
        }
        return (childOrder switch { DebugSemanticsDumpOrder.inverseHitTest => _childrenInHitTestOrder(), DebugSemanticsDumpOrder.traversalOrder => _childrenInTraversalOrder(), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _BoxEdge__semantics : IComparable<_BoxEdge__semantics>
{
    public virtual bool isLeadingEdge { get; private set; } = default!;
    public virtual double offset { get; private set; } = default!;
    public virtual SemanticsNode node { get; private set; } = default!;

    internal _BoxEdge__semantics(bool isLeadingEdge, double offset, SemanticsNode node)
    {
        this.isLeadingEdge = isLeadingEdge;
        this.offset = offset;
        this.node = node;
        System.Diagnostics.Debug.Assert(double.IsFinite(offset));
    }

    public virtual long compareTo(_BoxEdge__semantics other)
    {
        return this.offset.CompareTo(((_BoxEdge__semantics)other).offset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public int CompareTo(_BoxEdge__semantics? other) => checked((int)compareTo(other!));
}

internal class _SemanticsSortGroup__semantics : IComparable<_SemanticsSortGroup__semantics>
{
    public virtual double startOffset { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual List<SemanticsNode> nodes { get; private set; } = new List<SemanticsNode>();

    internal _SemanticsSortGroup__semantics(double startOffset, TextDirection textDirection)
    {
        this.startOffset = startOffset;
        this.textDirection = textDirection;
    }

    public virtual long compareTo(_SemanticsSortGroup__semantics other)
    {
        return this.startOffset.CompareTo(((_SemanticsSortGroup__semantics)other).startOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<SemanticsNode> sortedWithinVerticalGroup()
    {
        var edges = new List<_BoxEdge__semantics>();
        foreach (SemanticsNode child in this.nodes)
        {
            global::Doroti.Ui.Rect childRect = ((SemanticsNode)child).rect.deflate(0.1);
            edges.Add(new _BoxEdge__semantics(isLeadingEdge: true, offset: SemanticsLibrary._pointInParentCoordinates(child, childRect.topLeft).dx, node: child));
            edges.Add(new _BoxEdge__semantics(isLeadingEdge: false, offset: SemanticsLibrary._pointInParentCoordinates(child, childRect.bottomRight).dx, node: child));
        }
        edges.sort();
        var horizontalGroups = new List<_SemanticsSortGroup__semantics>();
        _SemanticsSortGroup__semantics? groupLocal = default!;
        var depth = 0L;
        foreach (var edge in edges)
        {
            if (((_BoxEdge__semantics)edge).isLeadingEdge)
            {
                depth += 1L;
                groupLocal ??= new _SemanticsSortGroup__semantics(startOffset: ((_BoxEdge__semantics)edge).offset, textDirection: this.textDirection);
                ((_SemanticsSortGroup__semantics)groupLocal).nodes.Add(((_BoxEdge__semantics)edge).node);
            }
            else
            {
                depth -= 1L;
            }
            if ((depth == 0L))
            {
                horizontalGroups.Add(groupLocal!);
                groupLocal = null;
            }
        }
        horizontalGroups.sort();
        if ((object.Equals(this.textDirection, TextDirection.rtl)))
        {
            horizontalGroups = System.Linq.Enumerable.Reverse(horizontalGroups).ToList();
        }
        return horizontalGroups.expand(((group) => group.sortedWithinKnot())).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<SemanticsNode> sortedWithinKnot()
    {
        if ((checked((long)(this.nodes.Count)) <= 1L))
        {
            return this.nodes;
        }
        var nodeMap = new DartMap<long, SemanticsNode>();
        var edges = new DartMap<long, long>();
        foreach (SemanticsNode nodeLocal in this.nodes)
        {
            nodeMap[((SemanticsNode)nodeLocal).id] = nodeLocal;
            global::Doroti.Ui.Offset centerLocal = SemanticsLibrary._pointInParentCoordinates(nodeLocal, ((SemanticsNode)nodeLocal).rect.center);
            foreach (SemanticsNode nextNode in this.nodes)
            {
                if ((DartRuntimePrimitives.Identical(nodeLocal, nextNode) || (edges.GetValueOrDefault(((SemanticsNode)nextNode).id) == ((SemanticsNode)nodeLocal).id)))
                {
                    continue;
                }
                global::Doroti.Ui.Offset nextCenter = SemanticsLibrary._pointInParentCoordinates(nextNode, ((SemanticsNode)nextNode).rect.center);
                global::Doroti.Ui.Offset centerDelta = (nextCenter - centerLocal);
                double directionLocal = centerDelta.direction;
                bool isLtrAndForward = (((object.Equals(this.textDirection, TextDirection.ltr)) && ((-Dart_mathLibrary.pi / 4L) < directionLocal)) && (directionLocal < ((3L * Dart_mathLibrary.pi) / 4L)));
                bool isRtlAndForward = ((object.Equals(this.textDirection, TextDirection.rtl)) && (((directionLocal < ((-3L * Dart_mathLibrary.pi) / 4L)) || (directionLocal > ((3L * Dart_mathLibrary.pi) / 4L)))));
                if ((isLtrAndForward || isRtlAndForward))
                {
                    edges[((SemanticsNode)nodeLocal).id] = ((SemanticsNode)nextNode).id;
                }
            }
        }
        var sortedIds = new List<long>();
        var visitedIds = new HashSet<long>();
        List<SemanticsNode> startNodes = ((Func<List<SemanticsNode>>)(() =>
{
    var __cascade = this.nodes.ToList();
    __cascade.sort(((a, b) =>
    {
        global::Doroti.Ui.Offset aTopLeft = SemanticsLibrary._pointInParentCoordinates(a, ((SemanticsNode)a).rect.topLeft);
        global::Doroti.Ui.Offset bTopLeft = SemanticsLibrary._pointInParentCoordinates(b, ((SemanticsNode)b).rect.topLeft);
        long verticalDiff = aTopLeft.dy.CompareTo(bTopLeft.dy);
        if ((verticalDiff != 0L))
        {
            return -verticalDiff;
        }
        return -aTopLeft.dx.CompareTo(bTopLeft.dx);
        return default;
    }));
    return __cascade;
}))();
        void search(long id)
        {
            if (visitedIds.Contains(id))
            {
                return;
            }
            visitedIds.Add(id);
            if (edges.ContainsKey(id))
            {
                search(DartRuntimePrimitives.RequireValue(edges.GetValueOrDefault(id)));
            }
            sortedIds.Add(id);
        }
        startNodes.map<SemanticsNode, long>(((node) => ((SemanticsNode)node).id)).forEach(search);
        return System.Linq.Enumerable.Reverse(sortedIds.map<long, SemanticsNode>(((id) => nodeMap.GetValueOrDefault(id)!)).ToList()).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public int CompareTo(_SemanticsSortGroup__semantics? other) => checked((int)compareTo(other!));
}

public static partial class SemanticsLibrary
{
    internal static Offset _pointInParentCoordinates(SemanticsNode node, Offset point)
    {
        Matrix4? traversalTransform = ((SemanticsNode)node)._traversalTransform;
        if ((traversalTransform is null))
        {
            return point;
        }
        var vector = new Vector3(point.dx, point.dy, 0.0);
        traversalTransform.transform3(vector);
        return new global::Doroti.Ui.Offset(vector.x, vector.y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class SemanticsLibrary
{
    internal static List<SemanticsNode> _childrenInDefaultOrder(List<SemanticsNode> children, TextDirection textDirection)
    {
        var edges = new List<_BoxEdge__semantics>();
        foreach (var child in children)
        {
            DartRuntimePrimitives.Assert(() => ((SemanticsNode)child).rect.isFinite);
            global::Doroti.Ui.Rect childRect = ((SemanticsNode)child).rect.deflate(0.1);
            edges.Add(new _BoxEdge__semantics(isLeadingEdge: true, offset: SemanticsLibrary._pointInParentCoordinates(child, childRect.topLeft).dy, node: child));
            edges.Add(new _BoxEdge__semantics(isLeadingEdge: false, offset: SemanticsLibrary._pointInParentCoordinates(child, childRect.bottomRight).dy, node: child));
        }
        edges.sort();
        var verticalGroups = new List<_SemanticsSortGroup__semantics>();
        _SemanticsSortGroup__semantics? groupLocal = default!;
        var depth = 0L;
        foreach (var edge in edges)
        {
            if (((_BoxEdge__semantics)edge).isLeadingEdge)
            {
                depth += 1L;
                groupLocal ??= new _SemanticsSortGroup__semantics(startOffset: ((_BoxEdge__semantics)edge).offset, textDirection: textDirection);
                ((_SemanticsSortGroup__semantics)groupLocal).nodes.Add(((_BoxEdge__semantics)edge).node);
            }
            else
            {
                depth -= 1L;
            }
            if ((depth == 0L))
            {
                verticalGroups.Add(groupLocal!);
                groupLocal = null;
            }
        }
        verticalGroups.sort();
        return verticalGroups.expand(((group) => group.sortedWithinVerticalGroup())).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _TraversalSortNode__semantics : IComparable<_TraversalSortNode__semantics>
{
    public virtual SemanticsNode node { get; private set; } = default!;
    public virtual SemanticsSortKey? sortKey { get; private set; }
    public virtual long position { get; private set; } = default!;

    internal _TraversalSortNode__semantics(SemanticsNode node, SemanticsSortKey? sortKey = null, long position = default!)
    {
        this.node = node;
        this.sortKey = sortKey;
        this.position = position;
    }

    public virtual long compareTo(_TraversalSortNode__semantics other)
    {
        if (((this.sortKey is null) || (((_TraversalSortNode__semantics)other).sortKey is null)))
        {
            return (this.position - ((_TraversalSortNode__semantics)other).position);
        }
        return this.sortKey!.compareTo(((_TraversalSortNode__semantics)other).sortKey!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public int CompareTo(_TraversalSortNode__semantics? other) => checked((int)compareTo(other!));
}

public class SemanticsOwner : ChangeNotifier
{
    public virtual Action<SemanticsUpdate> onSemanticsUpdate { get; private set; } = default!;
    internal virtual HashSet<SemanticsNode> _dirtyNodes { get; private set; } = new HashSet<SemanticsNode>();
    internal virtual DartMap<long, SemanticsNode> _nodes { get; private set; } = new DartMap<long, SemanticsNode>();
    internal virtual HashSet<SemanticsNode> _detachedNodes { get; private set; } = new HashSet<SemanticsNode>();
    internal virtual DartMap<object, SemanticsNode> _traversalParentNodes { get; private set; } = new DartMap<object, SemanticsNode>();
    internal virtual DartMap<object, HashSet<SemanticsNode>> _traversalChildNodes { get; private set; } = new DartMap<object, HashSet<SemanticsNode>>();

    public SemanticsOwner(Action<SemanticsUpdate> onSemanticsUpdate)
    {
        this.onSemanticsUpdate = onSemanticsUpdate;
    }

    public virtual SemanticsNode? rootSemanticsNode => this._nodes.GetValueOrDefault(0L);
    public virtual SemanticsNode? getSemanticsNode(long id) => this._nodes.GetValueOrDefault(id);
    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this._dirtyNodes.Clear();
        this._nodes.Clear();
        this._detachedNodes.Clear();
        this._traversalChildNodes.Clear();
        this._traversalParentNodes.Clear();
        base.dispose();
    }

    public virtual void sendSemanticsUpdate()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                var invisibleNodes = new List<SemanticsNode>();
                bool findInvisibleNodes(SemanticsNode node)
                {
                    if (((SemanticsNode)node).rect.isEmpty)
                    {
                        invisibleNodes.Add(node);
                    }
                    else
                    {
                        if (!((SemanticsNode)node).mergeAllDescendantsIntoThisNode)
                        {
                            node.visitChildren((Func<SemanticsNode, bool>)findInvisibleNodes);
                        }
                    }
                    return true;
                    throw new InvalidOperationException("Dart control flow completed without a value.");
                }
                SemanticsNode? rootSemanticsNodeLocal = this.rootSemanticsNode;
                if ((rootSemanticsNodeLocal is not null))
                {
                    if (((((SemanticsNode)rootSemanticsNodeLocal).childrenCount > 0L) && ((SemanticsNode)rootSemanticsNodeLocal).rect.isEmpty))
                    {
                        invisibleNodes.Add(rootSemanticsNodeLocal);
                    }
                    else
                    {
                        if (!((SemanticsNode)rootSemanticsNodeLocal).mergeAllDescendantsIntoThisNode)
                        {
                            rootSemanticsNodeLocal.visitChildren((Func<SemanticsNode, bool>)findInvisibleNodes);
                        }
                    }
                }
                if ((checked((long)(invisibleNodes.Count)) == 0))
                {
                    return true;
                }
                List<DiagnosticsNode> nodeToMessage(SemanticsNode invisibleNode)
                {
                    SemanticsNode? parentLocal = ((SemanticsNode)invisibleNode).parent;
                    return new List<DiagnosticsNode> { ((Diagnosticable)invisibleNode).toDiagnosticsNode(style: DiagnosticsTreeStyle.errorProperty), (((Diagnosticable)parentLocal).toDiagnosticsNode(name: "which was added as a child of", style: DiagnosticsTreeStyle.errorProperty) ?? new ErrorDescription("which was added as the root SemanticsNode")) };
                    throw new InvalidOperationException("Dart control flow completed without a value.");
                }
                throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Invisible SemanticsNodes should not be added to the tree."), new ErrorDescription("The following invisible SemanticsNodes were added to the tree:"), new ErrorHint("An invisible SemanticsNode is one whose rect is not on screen hence not reachable for users, " + "and its semantic information is not merged into a visible parent."), new ErrorHint("An invisible SemanticsNode makes the accessibility experience confusing, " + "as it does not provide any visual indication when the user selects it " + "via accessibility technologies."), new ErrorHint("Consider removing the above invisible SemanticsNodes if they were added by your " + "RenderObject.assembleSemanticsNode implementation, or filing a bug on GitHub:\n" + "  https://github.com/flutter/flutter/issues/new?template=02_bug.yml") });
            });
        if ((checked((long)(this._dirtyNodes.Count)) == 0))
        {
            return;
        }
        var customSemanticsActionIds = new HashSet<long>();
        var visitedNodes = new List<SemanticsNode>();
        while ((checked((long)(this._dirtyNodes.Count)) != 0))
        {
            List<SemanticsNode> localDirtyNodes = this._dirtyNodes.where(((node) => !this._detachedNodes.Contains(node))).ToList();
            this._dirtyNodes.Clear();
            this._detachedNodes.Clear();
            localDirtyNodes.sort(((a, b) => (((SemanticsNode)a).depth - ((SemanticsNode)b).depth)));
            visitedNodes.AddRange(localDirtyNodes);
            foreach (var nodeLocal in localDirtyNodes)
            {
                DartRuntimePrimitives.Assert(() => ((SemanticsNode)nodeLocal)._dirty);
                DartRuntimePrimitives.Assert(() => (((((SemanticsNode)nodeLocal).parent is null) || !((SemanticsNode)nodeLocal).parent!.isPartOfNodeMerging) || ((SemanticsNode)nodeLocal).isMergedIntoParent));
                if (((SemanticsNode)nodeLocal).isPartOfNodeMerging)
                {
                    DartRuntimePrimitives.Assert(() => (((SemanticsNode)nodeLocal).mergeAllDescendantsIntoThisNode || (((SemanticsNode)nodeLocal).parent is not null)));
                    if (((((SemanticsNode)nodeLocal).parent is not null) && ((SemanticsNode)nodeLocal).parent!.isPartOfNodeMerging))
                    {
                        ((SemanticsNode)nodeLocal).parent!._markDirty();
                        nodeLocal._dirty = false;
                    }
                }
                this._traversalParentNodes.removeWhere(((key, oldNode) => (object.Equals(nodeLocal, oldNode))));
                foreach (HashSet<SemanticsNode> childSet in this._traversalChildNodes.Values)
                {
                    childSet.removeWhere(((oldNode) => (object.Equals(nodeLocal, oldNode))));
                }
                this._traversalChildNodes.removeWhere(((key, value) => (checked((long)(value.Count)) == 0)));
                bool isTraversalParent = ((SemanticsNode)nodeLocal)._isTraversalParent;
                bool isTraversalChild = ((SemanticsNode)nodeLocal)._isTraversalChild;
                if (isTraversalParent)
                {
                    DartRuntimePrimitives.Assert(() => (!this._traversalParentNodes.ContainsKey(((SemanticsNode)nodeLocal)._traversalParentIdentifier) || (object.Equals(this._traversalParentNodes.GetValueOrDefault(((SemanticsNode)nodeLocal).traversalParentIdentifier!), nodeLocal))));
                    this._traversalParentNodes[((SemanticsNode)nodeLocal).traversalParentIdentifier!] = nodeLocal;
                }
                else
                {
                    if (isTraversalChild)
                    {
                        this._traversalChildNodes.putIfAbsent(((SemanticsNode)nodeLocal).traversalChildIdentifier!, (() => new HashSet<SemanticsNode>())).Add(nodeLocal);
                    }
                }
                if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb)
                {
                    if (((SemanticsNode)nodeLocal)._isTraversalChild)
                    {
                        SemanticsNode? parentNode = this._traversalParentNodes.GetValueOrDefault(DartRuntimePrimitives.RequireReference(((SemanticsNode)nodeLocal).traversalChildIdentifier));
                        if (((parentNode is not null) && !visitedNodes.Contains(parentNode)))
                        {
                            parentNode._markDirty();
                        }
                    }
                }
            }
        }
        visitedNodes.sort(((a, b) => (((SemanticsNode)a).depth - ((SemanticsNode)b).depth)));
        global::Doroti.Ui.SemanticsUpdateBuilder builder = global::Doroti.Framework.Semantics.SemanticsBinding.instance.createSemanticsUpdateBuilder();
        foreach (var nodeAlternate in visitedNodes)
        {
            DartRuntimePrimitives.Assert(() => (((SemanticsNode)nodeAlternate).parent?._dirty != true));
            if ((((SemanticsNode)nodeAlternate)._dirty && ((SemanticsNode)nodeAlternate).attached))
            {
                nodeAlternate._addToUpdate(builder, customSemanticsActionIds);
            }
        }
        this._dirtyNodes.Clear();
        foreach (var actionId in customSemanticsActionIds)
        {
            CustomSemanticsAction actionLocal = CustomSemanticsAction.getAction(actionId)!;
            builder.updateCustomAction(id: actionId, label: ((CustomSemanticsAction)actionLocal).label, hint: ((CustomSemanticsAction)actionLocal).hint, overrideId: (FoundationRuntimePorts.EnumIndexNullable(((CustomSemanticsAction)actionLocal).action) ?? -1L));
        }
        this.onSemanticsUpdate(builder.build());
        notifyListeners();
    }

    internal virtual Action<object?>? _getSemanticsActionHandlerForId(long id, SemanticsAction action, object? args = null)
    {
        SemanticsNode? result = this._nodes.GetValueOrDefault(id);
        if ((result is null))
        {
            return null;
        }
        if ((((SemanticsNode)result).isPartOfNodeMerging && !result._canHandleAction(action, args)))
        {
            SemanticsNode? found = default!;
            result._visitDescendants(((Func<SemanticsNode, bool>)((node) =>
            {
                if (node._canHandleAction(action, args))
                {
                    found = node;
                    return false;
                }
                return true;
                return default;
            })));
            result = found;
        }
        if (((result is null) || !result._canHandleAction(action, args)))
        {
            return null;
        }
        return ((SemanticsNode)result)._actions.GetValueOrDefault(action);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void performAction(long id, SemanticsAction action, object? args = null)
    {
        Action<object?>? handler = _getSemanticsActionHandlerForId(id, action, args);
        if ((handler is not null))
        {
            handler(args);
            return;
        }
        if (((object.Equals(action, SemanticsAction.showOnScreen)) && (this._nodes.GetValueOrDefault(id)?._showOnScreen is not null)))
        {
            this._nodes.GetValueOrDefault(id)!._showOnScreen!();
        }
    }

    internal virtual Action<object?>? _getSemanticsActionHandlerForPosition(SemanticsNode node, Offset position, SemanticsAction action, object? args = null)
    {
        if ((((SemanticsNode)node).transform is not null))
        {
            var inverse = Matrix4.identity();
            if ((inverse.copyInverse(((SemanticsNode)node).transform!) == 0.0))
            {
                return null;
            }
            position = MatrixUtils.transformPoint(inverse, position);
        }
        if (!((SemanticsNode)node).rect.contains(position))
        {
            return null;
        }
        if (((SemanticsNode)node).mergeAllDescendantsIntoThisNode)
        {
            if (node._canHandleAction(action, args))
            {
                return ((SemanticsNode)node)._actions.GetValueOrDefault(action);
            }
            SemanticsNode? result = default!;
            node._visitDescendants(((Func<SemanticsNode, bool>)((child) =>
            {
                if (child._canHandleAction(action, args))
                {
                    result = child;
                    return false;
                }
                return true;
                return default;
            })));
            return result?._actions.GetValueOrDefault(action);
        }
        if (((SemanticsNode)node).hasChildren)
        {
            foreach (SemanticsNode childLocal in System.Linq.Enumerable.Reverse(((SemanticsNode)node)._children!))
            {
                Action<object?>? handler = _getSemanticsActionHandlerForPosition(childLocal, position, action, args);
                if ((handler is not null))
                {
                    return handler;
                }
            }
        }
        return ((SemanticsNode)node)._actions.GetValueOrDefault(action);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void performActionAt(Offset position, SemanticsAction action, object? args = null)
    {
        SemanticsNode? node = this.rootSemanticsNode;
        if ((node is null))
        {
            return;
        }
        Action<object?>? handler = _getSemanticsActionHandlerForPosition(node, position, action, args);
        if ((handler is not null))
        {
            handler(args);
        }
    }

    public override string ToString() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
}

public class SemanticsConfiguration
{
    internal virtual bool _isSemanticBoundary { get; set; } = false;
    internal virtual Locale? _localeForSubtree { get; set; } = default;
    public virtual Locale? locale { get; set; } = default;
    public virtual bool isBlockingUserActions { get; set; } = false;
    public virtual bool explicitChildNodes { get; set; } = false;
    public virtual bool isBlockingSemanticsOfPreviouslyPaintedNodes { get; set; } = false;
    internal virtual bool _hasBeenAnnotated { get; set; } = false;
    internal virtual DartMap<SemanticsAction, Action<object?>> _actions { get; private set; } = new DartMap<SemanticsAction, Action<object?>>();
    internal virtual long _actionsAsBits { get; set; } = 0L;
    internal virtual Action? _onTap { get; set; } = default;
    internal virtual Action? _onLongPress { get; set; } = default;
    internal virtual Action? _onScrollLeft { get; set; } = default;
    internal virtual Action? _onDismiss { get; set; } = default;
    internal virtual Action? _onScrollRight { get; set; } = default;
    internal virtual Action? _onScrollUp { get; set; } = default;
    internal virtual Action? _onScrollDown { get; set; } = default;
    internal virtual Action<Offset>? _onScrollToOffset { get; set; } = default;
    internal virtual Action? _onIncrease { get; set; } = default;
    internal virtual Action? _onDecrease { get; set; } = default;
    internal virtual Action? _onCopy { get; set; } = default;
    internal virtual Action? _onCut { get; set; } = default;
    internal virtual Action? _onPaste { get; set; } = default;
    internal virtual Action? _onShowOnScreen { get; set; } = default;
    internal virtual Action<bool>? _onMoveCursorForwardByCharacter { get; set; } = default;
    internal virtual Action<bool>? _onMoveCursorBackwardByCharacter { get; set; } = default;
    internal virtual Action<bool>? _onMoveCursorForwardByWord { get; set; } = default;
    internal virtual Action<bool>? _onMoveCursorBackwardByWord { get; set; } = default;
    internal virtual Action<TextSelection>? _onSetSelection { get; set; } = default;
    internal virtual Action<string>? _onSetText { get; set; } = default;
    internal virtual Action? _onDidGainAccessibilityFocus { get; set; } = default;
    internal virtual Action? _onDidLoseAccessibilityFocus { get; set; } = default;
    internal virtual Action? _onFocus { get; set; } = default;
    internal virtual Action? _onExpand { get; set; } = default;
    internal virtual Action? _onCollapse { get; set; } = default;
    internal virtual Func<List<SemanticsConfiguration>, ChildSemanticsConfigurationsResult>? _childConfigurationsDelegate { get; set; } = default;
    internal virtual SemanticsSortKey? _sortKey { get; set; } = default;
    internal virtual long? _indexInParent { get; set; } = default;
    internal virtual long? _scrollChildCount { get; set; } = default;
    internal virtual long? _scrollIndex { get; set; } = default;
    internal virtual long? _platformViewId { get; set; } = default;
    internal virtual long? _maxValueLength { get; set; } = default;
    internal virtual long? _currentValueLength { get; set; } = default;
    internal virtual bool _isMergingSemanticsOfDescendants { get; set; } = false;
    internal virtual DartMap<CustomSemanticsAction, Action> _customSemanticsActions { get; set; } = new DartMap<CustomSemanticsAction, Action>();
    internal virtual string _identifier { get; set; } = "";
    internal virtual object? _traversalParentIdentifier { get; set; } = default;
    internal virtual object? _traversalChildIdentifier { get; set; } = default;
    internal virtual SemanticsRole _role { get; set; } = SemanticsRole.none;
    internal virtual AttributedString _attributedLabel { get; set; } = new AttributedString("");
    internal virtual AttributedString _attributedValue { get; set; } = new AttributedString("");
    internal virtual AttributedString _attributedIncreasedValue { get; set; } = new AttributedString("");
    internal virtual AttributedString _attributedDecreasedValue { get; set; } = new AttributedString("");
    internal virtual AttributedString _attributedHint { get; set; } = new AttributedString("");
    internal virtual string _tooltip { get; set; } = "";
    internal virtual SemanticsHintOverrides? _hintOverrides { get; set; } = default;
    internal virtual TextDirection? _textDirection { get; set; } = default;
    internal virtual AccessibilityFocusBlockType _accessibilityFocusBlockType { get; set; } = AccessibilityFocusBlockType.none;
    internal virtual DartUri? _linkUrl { get; set; } = default;
    internal virtual long _headingLevel { get; set; } = 0L;
    internal virtual TextSelection? _textSelection { get; set; } = default;
    internal virtual double? _scrollPosition { get; set; } = default;
    internal virtual double? _scrollExtentMax { get; set; } = default;
    internal virtual double? _scrollExtentMin { get; set; } = default;
    internal virtual HashSet<string>? _controlsNodes { get; set; } = default;
    internal virtual SemanticsValidationResult _validationResult { get; set; } = SemanticsValidationResult.none;
    internal virtual SemanticsHitTestBehavior _hitTestBehavior { get; set; } = Dart_uiLibrary.SemanticsHitTestBehavior.defer;
    internal virtual SemanticsInputType _inputType { get; set; } = SemanticsInputType.none;
    internal virtual string? _maxValue { get; set; } = default;
    internal virtual string? _minValue { get; set; } = default;
    internal virtual HashSet<SemanticsTag>? _tagsForChildren { get; set; } = default;
    internal virtual SemanticsFlags _flags { get; set; } = SemanticsFlags.none;

    public virtual bool isSemanticBoundary
    {
        get => this._isSemanticBoundary;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (!this.isMergingSemanticsOfDescendants || DartRuntimePrimitives.RequireValue(__value)));
            _isSemanticBoundary = DartRuntimePrimitives.RequireValue(__value);
        }
    }
    public virtual global::Doroti.Ui.Locale? localeForSubtree
    {
        get => this._localeForSubtree;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is not null));
            _localeForSubtree = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool hasBeenAnnotated => this._hasBeenAnnotated;
    internal virtual long _effectiveActionsAsBits => (this.isBlockingUserActions ? (this._actionsAsBits & SemanticsLibrary._kUnblockedUserActions) : this._actionsAsBits);
    internal virtual void _addAction(SemanticsAction action, Action<object?> handler)
    {
        this._actions[DartRuntimePrimitives.RequireValue(action)] = handler;
        _actionsAsBits |= (long)action;
        _hasBeenAnnotated = true;
    }

    internal virtual void _addArgumentlessAction(SemanticsAction action, Action handler)
    {
        _addAction(action, ((Action<object?>)((args) =>
        {
            DartRuntimePrimitives.Assert(() => (args is null));
            handler();
        })));
    }

    public virtual Action? onTap
    {
        get => this._onTap;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.tap, __value!);
            _onTap = __value;
        }
    }
    public virtual Action? onLongPress
    {
        get => this._onLongPress;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.longPress, __value!);
            _onLongPress = __value;
        }
    }
    public virtual Action? onScrollLeft
    {
        get => this._onScrollLeft;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.scrollLeft, __value!);
            _onScrollLeft = __value;
        }
    }
    public virtual Action? onDismiss
    {
        get => this._onDismiss;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.dismiss, __value!);
            _onDismiss = __value;
        }
    }
    public virtual Action? onScrollRight
    {
        get => this._onScrollRight;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.scrollRight, __value!);
            _onScrollRight = __value;
        }
    }
    public virtual Action? onScrollUp
    {
        get => this._onScrollUp;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.scrollUp, __value!);
            _onScrollUp = __value;
        }
    }
    public virtual Action? onScrollDown
    {
        get => this._onScrollDown;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.scrollDown, __value!);
            _onScrollDown = __value;
        }
    }
    public virtual Action<Offset>? onScrollToOffset
    {
        get => this._onScrollToOffset;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is not null));
            _addAction(SemanticsAction.scrollToOffset, ((Action<object?>)((args) =>
            {
                var list = ((Float64List?)(object?)args!)!;
                __value!(new global::Doroti.Ui.Offset(list[0L], list[1L]));
            })));
            _onScrollToOffset = __value;
        }
    }
    public virtual Action? onIncrease
    {
        get => this._onIncrease;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.increase, __value!);
            _onIncrease = __value;
        }
    }
    public virtual Action? onDecrease
    {
        get => this._onDecrease;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.decrease, __value!);
            _onDecrease = __value;
        }
    }
    public virtual Action? onCopy
    {
        get => this._onCopy;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.copy, __value!);
            _onCopy = __value;
        }
    }
    public virtual Action? onCut
    {
        get => this._onCut;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.cut, __value!);
            _onCut = __value;
        }
    }
    public virtual Action? onPaste
    {
        get => this._onPaste;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.paste, __value!);
            _onPaste = __value;
        }
    }
    public virtual Action? onShowOnScreen
    {
        get => this._onShowOnScreen;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.showOnScreen, __value!);
            _onShowOnScreen = __value;
        }
    }
    public virtual Action<bool>? onMoveCursorForwardByCharacter
    {
        get => this._onMoveCursorForwardByCharacter;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is not null));
            _addAction(SemanticsAction.moveCursorForwardByCharacter, ((Action<object?>)((args) =>
            {
                var extendSelection = ((bool)args!);
                __value!(extendSelection);
            })));
            _onMoveCursorForwardByCharacter = __value;
        }
    }
    public virtual Action<bool>? onMoveCursorBackwardByCharacter
    {
        get => this._onMoveCursorBackwardByCharacter;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is not null));
            _addAction(SemanticsAction.moveCursorBackwardByCharacter, ((Action<object?>)((args) =>
            {
                var extendSelection = ((bool)args!);
                __value!(extendSelection);
            })));
            _onMoveCursorBackwardByCharacter = __value;
        }
    }
    public virtual Action<bool>? onMoveCursorForwardByWord
    {
        get => this._onMoveCursorForwardByWord;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is not null));
            _addAction(SemanticsAction.moveCursorForwardByWord, ((Action<object?>)((args) =>
            {
                var extendSelection = ((bool)args!);
                __value!(extendSelection);
            })));
            _onMoveCursorForwardByCharacter = __value;
        }
    }
    public virtual Action<bool>? onMoveCursorBackwardByWord
    {
        get => this._onMoveCursorBackwardByWord;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is not null));
            _addAction(SemanticsAction.moveCursorBackwardByWord, ((Action<object?>)((args) =>
            {
                var extendSelection = ((bool)args!);
                __value!(extendSelection);
            })));
            _onMoveCursorBackwardByCharacter = __value;
        }
    }
    public virtual Action<TextSelection>? onSetSelection
    {
        get => this._onSetSelection;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is not null));
            _addAction(SemanticsAction.setSelection, ((Action<object?>)((args) =>
            {
                DartRuntimePrimitives.Assert(() => ((args is not null) && (args is System.Collections.IDictionary)));
                DartMap<string, long> selection = (DartRuntimePrimitives.ConvertMap<object, object>((System.Collections.IDictionary)args!)).cast<string, long>();
                DartRuntimePrimitives.Assert(() => ((selection.ContainsKey("base")) && (selection.ContainsKey("extent"))));
                __value!(new TextSelection(baseOffset: DartRuntimePrimitives.RequireValue(selection.GetValueOrDefault("base")), extentOffset: DartRuntimePrimitives.RequireValue(selection.GetValueOrDefault("extent"))));
            })));
            _onSetSelection = __value;
        }
    }
    public virtual Action<string>? onSetText
    {
        get => this._onSetText;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is not null));
            _addAction(SemanticsAction.setText, ((Action<object?>)((args) =>
            {
                DartRuntimePrimitives.Assert(() => ((args is not null) && (args is string)));
                var text = ((string?)(object?)args!)!;
                __value!(text);
            })));
            _onSetText = __value;
        }
    }
    public virtual Action? onDidGainAccessibilityFocus
    {
        get => this._onDidGainAccessibilityFocus;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.didGainAccessibilityFocus, __value!);
            _onDidGainAccessibilityFocus = __value;
        }
    }
    public virtual Action? onDidLoseAccessibilityFocus
    {
        get => this._onDidLoseAccessibilityFocus;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.didLoseAccessibilityFocus, __value!);
            _onDidLoseAccessibilityFocus = __value;
        }
    }
    public virtual Action? onFocus
    {
        get => this._onFocus;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.focus, __value!);
            _onFocus = __value;
        }
    }
    public virtual Action? onExpand
    {
        get => this._onExpand;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is not null));
            _addArgumentlessAction(SemanticsAction.expand, __value!);
            _onExpand = __value;
        }
    }
    public virtual Action? onCollapse
    {
        get => this._onCollapse;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is not null));
            _addArgumentlessAction(SemanticsAction.collapse, __value!);
            _onCollapse = __value;
        }
    }
    public virtual Func<List<SemanticsConfiguration>, ChildSemanticsConfigurationsResult>? childConfigurationsDelegate
    {
        get => this._childConfigurationsDelegate;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is not null));
            _childConfigurationsDelegate = __value;
        }
    }
    public virtual Action<object?>? getActionHandler(SemanticsAction action) => this._actions.GetValueOrDefault(action);
    public virtual SemanticsSortKey? sortKey
    {
        get => this._sortKey;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is not null));
            _sortKey = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual long? indexInParent
    {
        get => this._indexInParent;
        set
        {
            var __value = value;
            _indexInParent = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual long? scrollChildCount
    {
        get => this._scrollChildCount;
        set
        {
            var __value = value;
            if ((__value == this.scrollChildCount))
            {
                return;
            }
            _scrollChildCount = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual long? scrollIndex
    {
        get => this._scrollIndex;
        set
        {
            var __value = value;
            if ((__value == this.scrollIndex))
            {
                return;
            }
            _scrollIndex = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual long? platformViewId
    {
        get => this._platformViewId;
        set
        {
            var __value = value;
            if ((__value == this.platformViewId))
            {
                return;
            }
            _platformViewId = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual long? maxValueLength
    {
        get => this._maxValueLength;
        set
        {
            var __value = value;
            if ((__value == this.maxValueLength))
            {
                return;
            }
            _maxValueLength = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual long? currentValueLength
    {
        get => this._currentValueLength;
        set
        {
            var __value = value;
            if ((__value == this.currentValueLength))
            {
                return;
            }
            _currentValueLength = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isMergingSemanticsOfDescendants
    {
        get => this._isMergingSemanticsOfDescendants;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => this.isSemanticBoundary);
            _isMergingSemanticsOfDescendants = DartRuntimePrimitives.RequireValue(__value);
            _hasBeenAnnotated = true;
        }
    }
    public virtual DartMap<CustomSemanticsAction, Action> customSemanticsActions
    {
        get => this._customSemanticsActions;
        set
        {
            var __value = value;
            _hasBeenAnnotated = true;
            _actionsAsBits |= (long)SemanticsAction.customAction;
            _customSemanticsActions = __value;
            this._actions[SemanticsAction.customAction] = this._onCustomSemanticsAction;
        }
    }
    internal virtual void _onCustomSemanticsAction(object? args)
    {
        CustomSemanticsAction? action = CustomSemanticsAction.getAction(((long)args!));
        if ((action is null))
        {
            return;
        }
        Action? callback = this._customSemanticsActions.GetValueOrDefault(action);
        if ((callback is not null))
        {
            callback();
        }
    }

    public virtual string identifier
    {
        get => this._identifier;
        set
        {
            var identifier = value;
            _identifier = identifier;
            _hasBeenAnnotated = true;
        }
    }
    public virtual object? traversalParentIdentifier
    {
        get => this._traversalParentIdentifier;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this.traversalParentIdentifier)))
            {
                return;
            }
            _traversalParentIdentifier = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual object? traversalChildIdentifier
    {
        get => this._traversalChildIdentifier;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this.traversalChildIdentifier)))
            {
                return;
            }
            _traversalChildIdentifier = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual global::Doroti.Ui.SemanticsRole role
    {
        get => this._role;
        set
        {
            var __value = value;
            _role = DartRuntimePrimitives.RequireValue(__value);
            _hasBeenAnnotated = true;
        }
    }
    public virtual string label
    {
        get => ((AttributedString)this._attributedLabel).@string;
        set
        {
            var label = value;
            _attributedLabel = new AttributedString(label);
            _hasBeenAnnotated = true;
        }
    }
    public virtual AttributedString attributedLabel
    {
        get => this._attributedLabel;
        set
        {
            var attributedLabel = value;
            _attributedLabel = attributedLabel;
            _hasBeenAnnotated = true;
        }
    }
    public virtual string value
    {
        get => ((AttributedString)this._attributedValue).@string;
        set
        {
            var __value = value;
            _attributedValue = new AttributedString(__value);
            _hasBeenAnnotated = true;
        }
    }
    public virtual AttributedString attributedValue
    {
        get => this._attributedValue;
        set
        {
            var attributedValue = value;
            _attributedValue = attributedValue;
            _hasBeenAnnotated = true;
        }
    }
    public virtual string increasedValue
    {
        get => ((AttributedString)this._attributedIncreasedValue).@string;
        set
        {
            var increasedValue = value;
            _attributedIncreasedValue = new AttributedString(increasedValue);
            _hasBeenAnnotated = true;
        }
    }
    public virtual AttributedString attributedIncreasedValue
    {
        get => this._attributedIncreasedValue;
        set
        {
            var attributedIncreasedValue = value;
            _attributedIncreasedValue = attributedIncreasedValue;
            _hasBeenAnnotated = true;
        }
    }
    public virtual string decreasedValue
    {
        get => ((AttributedString)this._attributedDecreasedValue).@string;
        set
        {
            var decreasedValue = value;
            _attributedDecreasedValue = new AttributedString(decreasedValue);
            _hasBeenAnnotated = true;
        }
    }
    public virtual AttributedString attributedDecreasedValue
    {
        get => this._attributedDecreasedValue;
        set
        {
            var attributedDecreasedValue = value;
            _attributedDecreasedValue = attributedDecreasedValue;
            _hasBeenAnnotated = true;
        }
    }
    public virtual string hint
    {
        get => ((AttributedString)this._attributedHint).@string;
        set
        {
            var hint = value;
            _attributedHint = new AttributedString(hint);
            _hasBeenAnnotated = true;
        }
    }
    public virtual AttributedString attributedHint
    {
        get => this._attributedHint;
        set
        {
            var attributedHint = value;
            _attributedHint = attributedHint;
            _hasBeenAnnotated = true;
        }
    }
    public virtual string tooltip
    {
        get => this._tooltip;
        set
        {
            var tooltip = value;
            _tooltip = tooltip;
            _hasBeenAnnotated = true;
        }
    }
    public virtual SemanticsHintOverrides? hintOverrides
    {
        get => this._hintOverrides;
        set
        {
            var __value = value;
            if ((__value is null))
            {
                return;
            }
            _hintOverrides = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool scopesRoute
    {
        get => this._flags.scopesRoute;
        set
        {
            var __value = value;
            _flags = this._flags.copyWith(scopesRoute: DartRuntimePrimitives.RequireValue(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool namesRoute
    {
        get => this._flags.namesRoute;
        set
        {
            var __value = value;
            _flags = this._flags.copyWith(namesRoute: DartRuntimePrimitives.RequireValue(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isImage
    {
        get => this._flags.isImage;
        set
        {
            var __value = value;
            _flags = this._flags.copyWith(isImage: DartRuntimePrimitives.RequireValue(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool liveRegion
    {
        get => this._flags.isLiveRegion;
        set
        {
            var __value = value;
            _flags = this._flags.copyWith(isLiveRegion: DartRuntimePrimitives.RequireValue(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual global::Doroti.Ui.TextDirection? textDirection
    {
        get => this._textDirection;
        set
        {
            var textDirection = value;
            _textDirection = textDirection;
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isSelected
    {
        get => (object.Equals(this._flags.isSelected, Tristate.isTrue));
        set
        {
            var __value = value;
            _flags = this._flags.copyWith(isSelected: SemanticsLibrary._tristateFromBoolOrNull(DartRuntimePrimitives.RequireValue(__value)));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool? isExpanded
    {
        get => this._flags.isExpanded.toBoolOrNull();
        set
        {
            var __value = value;
            _flags = this._flags.copyWith(isExpanded: SemanticsLibrary._tristateFromBoolOrNull(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool? isEnabled
    {
        get => this._flags.isEnabled.toBoolOrNull();
        set
        {
            var __value = value;
            _flags = this._flags.copyWith(isEnabled: SemanticsLibrary._tristateFromBoolOrNull(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool? isChecked
    {
        get => ((object.Equals(this._flags.isChecked, CheckedState.none)) ? null : (object.Equals(this._flags.isChecked, CheckedState.isTrue)));
        set
        {
            var __value = value;
            if ((__value is not null))
            {
                bool value__value243016 = DartRuntimePrimitives.RequireValue(__value);
                _flags = this._flags.copyWith(isChecked: (DartRuntimePrimitives.RequireValue(value__value243016) ? CheckedState.isTrue : CheckedState.isFalse));
            }
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool? isCheckStateMixed
    {
        get => ((object.Equals(this._flags.isChecked, CheckedState.none)) ? null : (object.Equals(this._flags.isChecked, CheckedState.mixed)));
        set
        {
            var __value = value;
            if ((__value ?? false))
            {
                _flags = this._flags.copyWith(isChecked: CheckedState.mixed);
            }
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool? isToggled
    {
        get => this._flags.isToggled.toBoolOrNull();
        set
        {
            var __value = value;
            _flags = this._flags.copyWith(isToggled: SemanticsLibrary._tristateFromBoolOrNull(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isInMutuallyExclusiveGroup
    {
        get => this._flags.isInMutuallyExclusiveGroup;
        set
        {
            var __value = value;
            _flags = this._flags.copyWith(isInMutuallyExclusiveGroup: DartRuntimePrimitives.RequireValue(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isFocusable
    {
        get => (!object.Equals(this._flags.isFocused, Tristate.none));
        set
        {
            var __value = value;
            if (!DartRuntimePrimitives.RequireValue(__value))
            {
                _flags = this._flags.copyWith(isFocused: Tristate.none);
            }
            else
            {
                if ((object.Equals(this._flags.isFocused, Tristate.none)))
                {
                    _flags = this._flags.copyWith(isFocused: Tristate.isFalse);
                }
            }
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool? isFocused
    {
        get => this._flags.isFocused.toBoolOrNull();
        set
        {
            var __value = value;
            _flags = this._flags.copyWith(isFocused: SemanticsLibrary._tristateFromBoolOrNull(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual AccessibilityFocusBlockType accessibilityFocusBlockType
    {
        get => this._accessibilityFocusBlockType;
        set
        {
            var __value = value;
            _accessibilityFocusBlockType = DartRuntimePrimitives.RequireValue(__value);
            _flags = this._flags.copyWith(isAccessibilityFocusBlocked: (!object.Equals(DartRuntimePrimitives.RequireValue(__value), AccessibilityFocusBlockType.none)));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isButton
    {
        get => this._flags.isButton;
        set
        {
            var __value = value;
            _flags = this._flags.copyWith(isButton: DartRuntimePrimitives.RequireValue(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isLink
    {
        get => this._flags.isLink;
        set
        {
            var __value = value;
            _flags = this._flags.copyWith(isLink: DartRuntimePrimitives.RequireValue(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual DartUri? linkUrl
    {
        get => this._linkUrl;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._linkUrl)))
            {
                return;
            }
            _linkUrl = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isHeader
    {
        get => this._flags.isHeader;
        set
        {
            var __value = value;
            _flags = this._flags.copyWith(isHeader: DartRuntimePrimitives.RequireValue(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual long headingLevel
    {
        get => this._headingLevel;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((__value >= 0L) && (__value <= 6L)));
            if ((DartRuntimePrimitives.RequireValue(__value) == this.headingLevel))
            {
                return;
            }
            _headingLevel = DartRuntimePrimitives.RequireValue(__value);
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isSlider
    {
        get => this._flags.isSlider;
        set
        {
            var __value = value;
            _flags = this._flags.copyWith(isSlider: DartRuntimePrimitives.RequireValue(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isKeyboardKey
    {
        get => this._flags.isKeyboardKey;
        set
        {
            var __value = value;
            _flags = this._flags.copyWith(isKeyboardKey: DartRuntimePrimitives.RequireValue(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isHidden
    {
        get => this._flags.isHidden;
        set
        {
            var __value = value;
            _flags = this._flags.copyWith(isHidden: DartRuntimePrimitives.RequireValue(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isTextField
    {
        get => this._flags.isTextField;
        set
        {
            var __value = value;
            _flags = this._flags.copyWith(isTextField: DartRuntimePrimitives.RequireValue(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isReadOnly
    {
        get => this._flags.isReadOnly;
        set
        {
            var __value = value;
            _flags = this._flags.copyWith(isReadOnly: DartRuntimePrimitives.RequireValue(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isObscured
    {
        get => this._flags.isObscured;
        set
        {
            var __value = value;
            _flags = this._flags.copyWith(isObscured: DartRuntimePrimitives.RequireValue(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isMultiline
    {
        get => this._flags.isMultiline;
        set
        {
            var __value = value;
            _flags = this._flags.copyWith(isMultiline: DartRuntimePrimitives.RequireValue(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool? isRequired
    {
        get => this._flags.isRequired.toBoolOrNull();
        set
        {
            var __value = value;
            _flags = this._flags.copyWith(isRequired: SemanticsLibrary._tristateFromBoolOrNull(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool hasImplicitScrolling
    {
        get => this._flags.hasImplicitScrolling;
        set
        {
            var __value = value;
            _flags = this._flags.copyWith(hasImplicitScrolling: DartRuntimePrimitives.RequireValue(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual TextSelection? textSelection
    {
        get => this._textSelection;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is not null));
            _textSelection = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual double? scrollPosition
    {
        get => this._scrollPosition;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is not null));
            _scrollPosition = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual double? scrollExtentMax
    {
        get => this._scrollExtentMax;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is not null));
            _scrollExtentMax = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual double? scrollExtentMin
    {
        get => this._scrollExtentMin;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is not null));
            _scrollExtentMin = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual HashSet<string>? controlsNodes
    {
        get => this._controlsNodes;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is not null));
            _controlsNodes = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual global::Doroti.Ui.SemanticsValidationResult validationResult
    {
        get => this._validationResult;
        set
        {
            var __value = value;
            _validationResult = DartRuntimePrimitives.RequireValue(__value);
            _hasBeenAnnotated = true;
        }
    }
    public virtual global::Doroti.Ui.SemanticsHitTestBehavior hitTestBehavior
    {
        get => this._hitTestBehavior;
        set
        {
            var __value = value;
            _hitTestBehavior = DartRuntimePrimitives.RequireValue(__value);
            _hasBeenAnnotated = true;
        }
    }
    public virtual global::Doroti.Ui.SemanticsInputType inputType
    {
        get => this._inputType;
        set
        {
            var __value = value;
            _inputType = DartRuntimePrimitives.RequireValue(__value);
            _hasBeenAnnotated = true;
        }
    }
    public virtual string? maxValue
    {
        get => this._maxValue;
        set
        {
            var __value = value;
            _maxValue = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual string? minValue
    {
        get => this._minValue;
        set
        {
            var __value = value;
            _minValue = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual IEnumerable<SemanticsTag>? tagsForChildren => this._tagsForChildren;
    public virtual bool tagsChildrenWith(SemanticsTag tag) => (this._tagsForChildren?.Contains(tag) ?? false);
    public virtual void addTagForChildren(SemanticsTag tag)
    {
        _tagsForChildren ??= new HashSet<SemanticsTag>();
        this._tagsForChildren!.Add(tag);
    }

    internal virtual bool _hasExplicitRole
    {
        get
        {
            if ((!object.Equals(this._role, SemanticsRole.none)))
            {
                return true;
            }
            if (((((((this._flags.isTextField || ((this._flags.isHeader && global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb))) || this._flags.isSlider) || this._flags.isLink) || this._flags.scopesRoute) || this._flags.isImage) || this._flags.isKeyboardKey))
            {
                return true;
            }
            return false;
            return default!;
        }
    }
    public virtual bool isCompatibleWith(SemanticsConfiguration? other)
    {
        if (((other is null) || !((SemanticsConfiguration)other).hasBeenAnnotated))
        {
            return true;
        }
        if ((!object.Equals(this._traversalChildIdentifier, ((SemanticsConfiguration)other)._traversalChildIdentifier)))
        {
            return false;
        }
        if (!this.hasBeenAnnotated)
        {
            return true;
        }
        if (((this._actionsAsBits & ((SemanticsConfiguration)other)._actionsAsBits) != 0L))
        {
            return false;
        }
        if (this._flags.hasConflictingFlags(((SemanticsConfiguration)other)._flags))
        {
            return false;
        }
        if (((this._platformViewId is not null) && (((SemanticsConfiguration)other)._platformViewId is not null)))
        {
            return false;
        }
        if (((this._maxValueLength is not null) && (((SemanticsConfiguration)other)._maxValueLength is not null)))
        {
            return false;
        }
        if (((this._currentValueLength is not null) && (((SemanticsConfiguration)other)._currentValueLength is not null)))
        {
            return false;
        }
        if (((((AttributedString)this._attributedValue).@string.Length != 0) && (((SemanticsConfiguration)other)._attributedValue.@string.Length != 0)))
        {
            return false;
        }
        if ((!object.Equals(this._localeForSubtree, ((SemanticsConfiguration)other)._localeForSubtree)))
        {
            return false;
        }
        if ((this._hasExplicitRole && ((SemanticsConfiguration)other)._hasExplicitRole))
        {
            return false;
        }
        if (((!object.Equals(this._hitTestBehavior, Dart_uiLibrary.SemanticsHitTestBehavior.defer)) || (!object.Equals(((SemanticsConfiguration)other)._hitTestBehavior, Dart_uiLibrary.SemanticsHitTestBehavior.defer))))
        {
            return false;
        }
        if (((this._minValue is not null) && (((SemanticsConfiguration)other)._minValue is not null)))
        {
            return false;
        }
        if (((this._maxValue is not null) && (((SemanticsConfiguration)other)._maxValue is not null)))
        {
            return false;
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void absorb(SemanticsConfiguration child)
    {
        DartRuntimePrimitives.Assert(() => !this.explicitChildNodes);
        if (!((SemanticsConfiguration)child).hasBeenAnnotated)
        {
            return;
        }
        if (((SemanticsConfiguration)child).isBlockingUserActions)
        {
            ((SemanticsConfiguration)child)._actions.forEach(((key, value) =>
            {
                if (((SemanticsLibrary._kUnblockedUserActions & FoundationRuntimePorts.EnumIndex(key)) > 0L))
                {
                    this._actions[key] = value;
                }
            }));
        }
        else
        {
            this._actions.AddRange(((SemanticsConfiguration)child)._actions);
        }
        _actionsAsBits |= ((SemanticsConfiguration)child)._effectiveActionsAsBits;
        this._customSemanticsActions.AddRange(((SemanticsConfiguration)child)._customSemanticsActions);
        _flags = this._flags.merge(((SemanticsConfiguration)child)._flags);
        _linkUrl ??= ((SemanticsConfiguration)child)._linkUrl;
        _textSelection ??= ((SemanticsConfiguration)child)._textSelection;
        _scrollPosition ??= ((SemanticsConfiguration)child)._scrollPosition;
        _scrollExtentMax ??= ((SemanticsConfiguration)child)._scrollExtentMax;
        _scrollExtentMin ??= ((SemanticsConfiguration)child)._scrollExtentMin;
        _hintOverrides ??= ((SemanticsConfiguration)child)._hintOverrides;
        _indexInParent ??= ((SemanticsConfiguration)child).indexInParent;
        _scrollIndex ??= ((SemanticsConfiguration)child)._scrollIndex;
        _scrollChildCount ??= ((SemanticsConfiguration)child)._scrollChildCount;
        _platformViewId ??= ((SemanticsConfiguration)child)._platformViewId;
        _maxValueLength ??= ((SemanticsConfiguration)child)._maxValueLength;
        _currentValueLength ??= ((SemanticsConfiguration)child)._currentValueLength;
        if ((this._traversalChildIdentifier is null))
        {
            _traversalParentIdentifier ??= ((SemanticsConfiguration)child)._traversalParentIdentifier;
        }
        _traversalChildIdentifier ??= ((SemanticsConfiguration)child)._traversalChildIdentifier;
        _headingLevel = SemanticsLibrary._mergeHeadingLevels(sourceLevel: ((SemanticsConfiguration)child)._headingLevel, targetLevel: this._headingLevel);
        textDirection ??= ((SemanticsConfiguration)child).textDirection;
        _sortKey ??= ((SemanticsConfiguration)child)._sortKey;
        if ((this._identifier == ""))
        {
            _identifier = ((SemanticsConfiguration)child)._identifier;
        }
        _attributedLabel = SemanticsLibrary._concatAttributedString(thisAttributedString: this._attributedLabel, thisTextDirection: this.textDirection, otherAttributedString: ((SemanticsConfiguration)child)._attributedLabel, otherTextDirection: ((SemanticsConfiguration)child).textDirection);
        if ((((AttributedString)this._attributedValue).@string == ""))
        {
            _attributedValue = ((SemanticsConfiguration)child)._attributedValue;
        }
        if ((((AttributedString)this._attributedIncreasedValue).@string == ""))
        {
            _attributedIncreasedValue = ((SemanticsConfiguration)child)._attributedIncreasedValue;
        }
        if ((((AttributedString)this._attributedDecreasedValue).@string == ""))
        {
            _attributedDecreasedValue = ((SemanticsConfiguration)child)._attributedDecreasedValue;
        }
        if ((object.Equals(this._role, SemanticsRole.none)))
        {
            _role = ((SemanticsConfiguration)child)._role;
        }
        if ((object.Equals(this._inputType, SemanticsInputType.none)))
        {
            _inputType = ((SemanticsConfiguration)child)._inputType;
        }
        _attributedHint = SemanticsLibrary._concatAttributedString(thisAttributedString: this._attributedHint, thisTextDirection: this.textDirection, otherAttributedString: ((SemanticsConfiguration)child)._attributedHint, otherTextDirection: ((SemanticsConfiguration)child).textDirection);
        if ((this._tooltip == ""))
        {
            _tooltip = ((SemanticsConfiguration)child)._tooltip;
        }
        if ((this._controlsNodes is null))
        {
            _controlsNodes = ((SemanticsConfiguration)child)._controlsNodes;
        }
        else
        {
            if ((((SemanticsConfiguration)child)._controlsNodes is not null))
            {
                _controlsNodes = new HashSet<string>();
            }
        }
        if ((!object.Equals(((SemanticsConfiguration)child)._validationResult, this._validationResult)))
        {
            if ((object.Equals(((SemanticsConfiguration)child)._validationResult, SemanticsValidationResult.invalid)))
            {
                _validationResult = SemanticsValidationResult.invalid;
            }
            else
            {
                if ((object.Equals(this._validationResult, SemanticsValidationResult.none)))
                {
                    _validationResult = ((SemanticsConfiguration)child)._validationResult;
                }
            }
        }
        _accessibilityFocusBlockType = this._accessibilityFocusBlockType._merge(((SemanticsConfiguration)child)._accessibilityFocusBlockType);
        _minValue ??= ((SemanticsConfiguration)child)._minValue;
        _maxValue ??= ((SemanticsConfiguration)child)._maxValue;
        if (((object.Equals(this._hitTestBehavior, Dart_uiLibrary.SemanticsHitTestBehavior.defer)) && (!object.Equals(((SemanticsConfiguration)child)._hitTestBehavior, Dart_uiLibrary.SemanticsHitTestBehavior.defer))))
        {
            _hitTestBehavior = ((SemanticsConfiguration)child)._hitTestBehavior;
        }
        _hasBeenAnnotated = (this.hasBeenAnnotated || ((SemanticsConfiguration)child).hasBeenAnnotated);
    }

    public virtual SemanticsConfiguration copy()
    {
        return ((Func<SemanticsConfiguration>)(() =>
{
    var __cascade = new SemanticsConfiguration();
    __cascade._isSemanticBoundary = this._isSemanticBoundary;
    __cascade.explicitChildNodes = this.explicitChildNodes;
    __cascade.isBlockingSemanticsOfPreviouslyPaintedNodes = this.isBlockingSemanticsOfPreviouslyPaintedNodes;
    __cascade._hasBeenAnnotated = this.hasBeenAnnotated;
    __cascade._isMergingSemanticsOfDescendants = this._isMergingSemanticsOfDescendants;
    __cascade._textDirection = this._textDirection;
    __cascade._sortKey = this._sortKey;
    __cascade._identifier = this._identifier;
    __cascade._traversalParentIdentifier = this._traversalParentIdentifier;
    __cascade._traversalChildIdentifier = this._traversalChildIdentifier;
    __cascade._attributedLabel = this._attributedLabel;
    __cascade._attributedIncreasedValue = this._attributedIncreasedValue;
    __cascade._attributedValue = this._attributedValue;
    __cascade._attributedDecreasedValue = this._attributedDecreasedValue;
    __cascade._attributedHint = this._attributedHint;
    __cascade._accessibilityFocusBlockType = this._accessibilityFocusBlockType;
    __cascade._hintOverrides = this._hintOverrides;
    __cascade._tooltip = this._tooltip;
    __cascade._flags = this._flags;
    __cascade._tagsForChildren = this._tagsForChildren;
    __cascade._textSelection = this._textSelection;
    __cascade._scrollPosition = this._scrollPosition;
    __cascade._scrollExtentMax = this._scrollExtentMax;
    __cascade._scrollExtentMin = this._scrollExtentMin;
    __cascade._actionsAsBits = this._actionsAsBits;
    __cascade._indexInParent = this.indexInParent;
    __cascade._scrollIndex = this._scrollIndex;
    __cascade._scrollChildCount = this._scrollChildCount;
    __cascade._platformViewId = this._platformViewId;
    __cascade._maxValueLength = this._maxValueLength;
    __cascade._currentValueLength = this._currentValueLength;
    __cascade._actions.AddRange(this._actions);
    __cascade._customSemanticsActions.AddRange(this._customSemanticsActions);
    __cascade.isBlockingUserActions = this.isBlockingUserActions;
    __cascade._headingLevel = this._headingLevel;
    __cascade._linkUrl = this._linkUrl;
    __cascade._role = this._role;
    __cascade._controlsNodes = this._controlsNodes;
    __cascade._validationResult = this._validationResult;
    __cascade._inputType = this._inputType;
    __cascade._hitTestBehavior = this._hitTestBehavior;
    __cascade._traversalChildIdentifier = this._traversalChildIdentifier;
    __cascade._traversalParentIdentifier = this._traversalParentIdentifier;
    __cascade._minValue = this._minValue;
    __cascade._maxValue = this._maxValue;
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum DebugSemanticsDumpOrder
{
    inverseHitTest,
    traversalOrder
}

public static partial class SemanticsLibrary
{
    internal static AttributedString _concatAttributedString(AttributedString thisAttributedString, AttributedString otherAttributedString, TextDirection? thisTextDirection, TextDirection? otherTextDirection)
    {
        if ((((AttributedString)otherAttributedString).@string.Length == 0))
        {
            return thisAttributedString;
        }
        if (((!object.Equals(thisTextDirection, otherTextDirection)) && (otherTextDirection is not null)))
        {
            TextDirection otherTextDirection__value266687 = DartRuntimePrimitives.RequireValue(otherTextDirection);
            AttributedString directionEmbedding = (DartRuntimePrimitives.RequireValue(otherTextDirection__value266687) switch { TextDirection.rtl => new AttributedString(Unicode.RLE), TextDirection.ltr => new AttributedString(Unicode.LRE), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            otherAttributedString = ((directionEmbedding.op_Add(otherAttributedString)).op_Add(new AttributedString(Unicode.PDF)));
        }
        if ((((AttributedString)thisAttributedString).@string.Length == 0))
        {
            return otherAttributedString;
        }
        return ((thisAttributedString.op_Add(new AttributedString("\n"))).op_Add(otherAttributedString));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public abstract class SemanticsSortKey : Diagnosticable, IComparable<SemanticsSortKey>
{
    public virtual string? name { get; private set; }

    protected SemanticsSortKey(string? name = null)
    {
        this.name = name;
    }

    public virtual long compareTo(SemanticsSortKey other)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(this.GetType(), DartRuntimePrimitives.RuntimeType(other))));
        if ((this.name == ((SemanticsSortKey)other).name))
        {
            return doCompare(other);
        }
        if (((this.name is null) && (((SemanticsSortKey)other).name is not null)))
        {
            return -1L;
        }
        else
        {
            if (((this.name is not null) && (((SemanticsSortKey)other).name is null)))
            {
                return 1L;
            }
        }
        return this.name!.CompareTo(((SemanticsSortKey)other).name!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract long doCompare(SemanticsSortKey other);
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new StringProperty("name", this.name, defaultValue: null));
    }

    public int CompareTo(SemanticsSortKey? other) => checked((int)compareTo(other!));
}

public class OrdinalSortKey : SemanticsSortKey
{
    public virtual double order { get; private set; } = default!;

    public OrdinalSortKey(double order, string? name = null) : base(name: name)
    {
        this.order = order;
        System.Diagnostics.Debug.Assert((order > double.NegativeInfinity));
        System.Diagnostics.Debug.Assert((order < double.PositiveInfinity));
    }

    public override long doCompare(SemanticsSortKey other)
    {
        var __other = (OrdinalSortKey)(object)other;
        if ((((OrdinalSortKey)__other).order == this.order))
        {
            return 0L;
        }
        return this.order.CompareTo(((OrdinalSortKey)__other).order);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("order", this.order, defaultValue: null));
    }

}

public static partial class SemanticsLibrary
{
    internal static long _mergeHeadingLevels(long sourceLevel, long targetLevel)
    {
        return ((targetLevel == 0L) ? sourceLevel : targetLevel);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class SemanticsLibrary
{
    internal static Tristate _tristateFromBoolOrNull(bool? value)
    {
        if ((value is null))
        {
            return Tristate.none;
        }
        if (DartRuntimePrimitives.RequireValue(value))
        {
            return Tristate.isTrue;
        }
        return Tristate.isFalse;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class SemanticsLibrary
{
    internal static long _toBitMask(SemanticsFlags flags)
    {
        var bitmask = 0L;
        if ((!object.Equals(flags.isChecked, CheckedState.none)))
        {
            bitmask |= (1L << (int)(0L));
        }
        if ((object.Equals(flags.isChecked, CheckedState.isTrue)))
        {
            bitmask |= (1L << (int)(1L));
        }
        if ((object.Equals(flags.isSelected, Tristate.isTrue)))
        {
            bitmask |= (1L << (int)(2L));
        }
        if (flags.isButton)
        {
            bitmask |= (1L << (int)(3L));
        }
        if (flags.isTextField)
        {
            bitmask |= (1L << (int)(4L));
        }
        if ((object.Equals(flags.isFocused, Tristate.isTrue)))
        {
            bitmask |= (1L << (int)(5L));
        }
        if ((!object.Equals(flags.isEnabled, Tristate.none)))
        {
            bitmask |= (1L << (int)(6L));
        }
        if ((object.Equals(flags.isEnabled, Tristate.isTrue)))
        {
            bitmask |= (1L << (int)(7L));
        }
        if (flags.isInMutuallyExclusiveGroup)
        {
            bitmask |= (1L << (int)(8L));
        }
        if (flags.isHeader)
        {
            bitmask |= (1L << (int)(9L));
        }
        if (flags.isObscured)
        {
            bitmask |= (1L << (int)(10L));
        }
        if (flags.scopesRoute)
        {
            bitmask |= (1L << (int)(11L));
        }
        if (flags.namesRoute)
        {
            bitmask |= (1L << (int)(12L));
        }
        if (flags.isHidden)
        {
            bitmask |= (1L << (int)(13L));
        }
        if (flags.isImage)
        {
            bitmask |= (1L << (int)(14L));
        }
        if (flags.isLiveRegion)
        {
            bitmask |= (1L << (int)(15L));
        }
        if ((!object.Equals(flags.isToggled, Tristate.none)))
        {
            bitmask |= (1L << (int)(16L));
        }
        if ((object.Equals(flags.isToggled, Tristate.isTrue)))
        {
            bitmask |= (1L << (int)(17L));
        }
        if (flags.hasImplicitScrolling)
        {
            bitmask |= (1L << (int)(18L));
        }
        if (flags.isMultiline)
        {
            bitmask |= (1L << (int)(19L));
        }
        if (flags.isReadOnly)
        {
            bitmask |= (1L << (int)(20L));
        }
        if ((!object.Equals(flags.isFocused, Tristate.none)))
        {
            bitmask |= (1L << (int)(21L));
        }
        if (flags.isLink)
        {
            bitmask |= (1L << (int)(22L));
        }
        if (flags.isSlider)
        {
            bitmask |= (1L << (int)(23L));
        }
        if (flags.isKeyboardKey)
        {
            bitmask |= (1L << (int)(24L));
        }
        if ((object.Equals(flags.isChecked, CheckedState.mixed)))
        {
            bitmask |= (1L << (int)(25L));
        }
        if ((!object.Equals(flags.isExpanded, Tristate.none)))
        {
            bitmask |= (1L << (int)(26L));
        }
        if ((object.Equals(flags.isExpanded, Tristate.isTrue)))
        {
            bitmask |= (1L << (int)(27L));
        }
        if ((!object.Equals(flags.isSelected, Tristate.none)))
        {
            bitmask |= (1L << (int)(28L));
        }
        if ((!object.Equals(flags.isRequired, Tristate.none)))
        {
            bitmask |= (1L << (int)(29L));
        }
        if ((object.Equals(flags.isRequired, Tristate.isTrue)))
        {
            bitmask |= (1L << (int)(30L));
        }
        return bitmask;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
